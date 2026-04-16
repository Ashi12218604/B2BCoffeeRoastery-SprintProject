using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using NotificationService.Infrastructure.Data;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly NotificationDbContext _db;

    public EmailService(NotificationDbContext db) => _db = db;

    // ── Core send method ──────────────────────────────────────────────────
    private async Task SendAsync(
        string toEmail, string toName,
        string subject, string htmlBody,
        string notificationType,
        byte[]? attachmentBytes = null,
        string? attachmentName = null)
    {
        var smtpCfg = await _db.SmtpConfigs
            .FirstOrDefaultAsync(x => x.IsActive)
            ?? throw new InvalidOperationException(
                "No active SMTP config found.");

        var log = new NotificationLog
        {
            RecipientEmail = toEmail,
            RecipientName = toName,
            Subject = subject,
            NotificationType = notificationType,
            Status = "Pending"
        };

        try
        {
            var message = new MimeMessage();
            message.From.Add(
                new MailboxAddress(smtpCfg.SenderName, smtpCfg.SenderEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };

            if (attachmentBytes is not null && attachmentName is not null)
                bodyBuilder.Attachments.Add(
                    attachmentName, attachmentBytes,
                    ContentType.Parse("application/pdf"));

            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                smtpCfg.Host, smtpCfg.Port, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(
                smtpCfg.SenderEmail, smtpCfg.Password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            log.Status = "Sent";
            log.SentAt = DateTime.UtcNow;
        }
        catch (Exception ex)
        {
            log.Status = "Failed";
            log.ErrorMessage = ex.Message;
            throw;
        }
        finally
        {
            _db.NotificationLogs.Add(log);
            await _db.SaveChangesAsync();
        }
    }

    // ── OTP Email ─────────────────────────────────────────────────────────
    public async Task SendOtpEmailAsync(
        string toEmail, string toName, string otp) =>
        await SendAsync(toEmail, toName,
            "☕ Your B2B Coffee OTP Code",
            $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;
                        margin:auto;padding:30px;background:#f9f5f0;
                        border-radius:10px;border:1px solid #d4a96a;">
              <h2 style="color:#4a2c0a;margin-bottom:5px;">
                ☕ B2B Coffee Roastery</h2>
              <p style="color:#666;">Email Verification</p>
              <hr style="border-color:#d4a96a;">
              <p>Hello <strong>{toName}</strong>,</p>
              <p>Use the code below to verify your email address:</p>
              <div style="text-align:center;margin:28px 0;">
                <span style="font-size:20px;font-weight:bold;
                             letter-spacing:4px;color:#4a2c0a;
                             background:#fff8f0;padding:12px 24px;
                             border-radius:10px;
                             border:2px solid #4a2c0a;display:inline-block;">{otp}</span>
              </div>
              <p>⏰ This code expires in <strong>10 minutes</strong>.</p>
              <p style="color:#999;font-size:12px;">
                If you didn't register, ignore this email.</p>
              <hr style="border-color:#d4a96a;">
              <p style="color:#4a2c0a;font-size:12px;text-align:center;">
                © B2B Coffee Roastery Platform</p>
            </div>
            """,
            "OTP");

    // ── Welcome Email ─────────────────────────────────────────────────────
    public async Task SendWelcomeEmailAsync(
        string toEmail, string toName) =>
        await SendAsync(toEmail, toName,
            "🎉 Welcome to B2B Coffee Roastery — You're Approved!",
            $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;
                        margin:auto;padding:30px;background:#f9f5f0;
                        border-radius:10px;border:1px solid #d4a96a;">
              <h2 style="color:#4a2c0a;">🎉 Welcome, {toName}!</h2>
              <p>Your B2B Coffee Roastery account has been
                <strong style="color:#27ae60;">approved and activated</strong>.
              </p>
              <div style="background:#fff8f0;padding:20px;
                          border-radius:8px;margin:20px 0;">
                <h3 style="color:#4a2c0a;margin-top:0;">
                  What you can do now:</h3>
                <ul style="color:#555;line-height:2;">
                  <li>☕ Browse our premium coffee catalog</li>
                  <li>📦 Place bulk B2B orders</li>
                  <li>🚚 Track deliveries in real-time</li>
                  <li>📄 Download invoices as PDF</li>
                </ul>
              </div>
              <div style="text-align:center;margin:25px 0;">
                <a href="http://localhost:4200"
                   style="background:#4a2c0a;color:white;
                          padding:14px 35px;border-radius:8px;
                          text-decoration:none;font-weight:bold;
                          font-size:16px;">
                  Start Shopping ☕
                </a>
              </div>
              <hr style="border-color:#d4a96a;">
              <p style="color:#4a2c0a;font-size:12px;text-align:center;">
                © B2B Coffee Roastery Platform</p>
            </div>
            """,
            "Welcome");

    // ── Order Received + Invoice PDF ──────────────────────────────────────
    public async Task SendOrderReceivedEmailAsync(
        string toEmail, string toName,
        string orderId, decimal total,
        List<OrderItemDto> items)
    {
        var pdf = GenerateInvoicePdf(orderId, toName, items, total);
        var shortId = orderId[..8].ToUpper();

        var itemRows = string.Join("", items.Select(i =>
            $"""
            <tr>
              <td style="padding:10px;border:1px solid #d4a96a;">
                {i.ProductName}</td>
              <td style="padding:10px;border:1px solid #d4a96a;
                         text-align:center;">{i.Quantity}</td>
              <td style="padding:10px;border:1px solid #d4a96a;
                         text-align:right;">₹{i.UnitPrice:F2}</td>
              <td style="padding:10px;border:1px solid #d4a96a;
                         text-align:right;">
                ₹{(i.Quantity * i.UnitPrice):F2}</td>
            </tr>
            """));

        await SendAsync(toEmail, toName,
            $"📦 Order Received — Invoice #{shortId}",
            $"""
            <div style="font-family:Arial,sans-serif;max-width:650px;
                        margin:auto;padding:30px;background:#f9f5f0;
                        border-radius:10px;border:1px solid #d4a96a;">
              <h2 style="color:#4a2c0a;">📦 Order Received!</h2>
              <p>Hello <strong>{toName}</strong>,
                we've received your order.</p>
              <div style="background:#fff8f0;padding:15px;
                          border-radius:8px;margin:15px 0;">
                <strong>Order ID:</strong> #{shortId}<br/>
                <strong>Date:</strong>
                  {DateTime.UtcNow:dd MMM yyyy HH:mm} UTC
              </div>
              <table style="width:100%;border-collapse:collapse;
                            margin:20px 0;">
                <thead>
                  <tr style="background:#4a2c0a;color:white;">
                    <th style="padding:10px;text-align:left;">Product</th>
                    <th style="padding:10px;">Qty</th>
                    <th style="padding:10px;text-align:right;">Price</th>
                    <th style="padding:10px;text-align:right;">Subtotal</th>
                  </tr>
                </thead>
                <tbody>{itemRows}</tbody>
                <tfoot>
                  <tr style="background:#fff8f0;">
                    <td colspan="3"
                        style="padding:10px;text-align:right;
                               font-weight:bold;border:1px solid #d4a96a;">
                      Total</td>
                    <td style="padding:10px;text-align:right;
                               font-weight:bold;color:#4a2c0a;
                               border:1px solid #d4a96a;">
                      ₹{total:F2}</td>
                  </tr>
                </tfoot>
              </table>
              <p>📎 Your invoice PDF is attached.</p>
              <p>We'll update you when your order is processed.</p>
              <hr style="border-color:#d4a96a;">
              <p style="color:#4a2c0a;font-size:12px;text-align:center;">
                © B2B Coffee Roastery Platform</p>
            </div>
            """,
            "OrderReceived",
            pdf,
            $"Invoice_{shortId}.pdf");
    }

    // ── Order Status Update Email ─────────────────────────────────────────
    public async Task SendOrderStatusEmailAsync(
        string toEmail, string toName,
        string orderId, string status,
        string? trackingNumber = null)
    {
        var (icon, color, message) = status switch
        {
            "In-Process" => ("🔄", "#e67e22",
                "Your order is being prepared by our team."),
            "Dispatched" => ("🚚", "#2980b9",
                "Your order is on its way!"),
            "Delivered" => ("✅", "#27ae60",
                "Your order has been delivered. Enjoy your coffee!"),
            _ => ("📦", "#4a2c0a", "Order status updated.")
        };

        var shortId = orderId[..8].ToUpper();

        await SendAsync(toEmail, toName,
            $"{icon} Order #{shortId} — {status}",
            $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;
                        margin:auto;padding:30px;background:#f9f5f0;
                        border-radius:10px;border:1px solid #d4a96a;">
              <h2 style="color:#4a2c0a;">Order Status Update</h2>
              <p>Hello <strong>{toName}</strong>,</p>
              <div style="background:#fff8f0;padding:25px;
                          border-radius:12px;text-align:center;
                          margin:20px 0;border:2px solid {color};">
                <div style="font-size:48px;">{icon}</div>
                <div style="font-size:22px;font-weight:bold;
                             color:{color};margin:10px 0;">{status}</div>
                <div style="color:#555;">Order #{shortId}</div>
              </div>
              <p style="color:#555;">{message}</p>
              {(trackingNumber is not null
                  ? $"""
                    <div style="background:#e8f5e9;padding:15px;
                                border-radius:8px;margin:15px 0;">
                      <strong>🔍 Tracking Number:</strong>
                      <span style="font-family:monospace;font-size:16px;
                                   color:#2e7d32;"> {trackingNumber}</span>
                    </div>
                    """
                  : "")}
              <hr style="border-color:#d4a96a;">
              <p style="color:#4a2c0a;font-size:12px;text-align:center;">
                © B2B Coffee Roastery Platform</p>
            </div>
            """,
            $"OrderStatus_{status}");
    }

    // ── Order Rejected Email ──────────────────────────────────────────────
    public async Task SendOrderRejectedEmailAsync(
        string toEmail, string toName,
        string orderId, string reason) =>
        await SendAsync(toEmail, toName,
            $"❌ Order #{orderId[..8].ToUpper()} Rejected",
            $"""
            <div style="font-family:Arial,sans-serif;max-width:600px;
                        margin:auto;padding:30px;background:#f9f5f0;
                        border-radius:10px;border:1px solid #e74c3c;">
              <h2 style="color:#c0392b;">❌ Order Rejected</h2>
              <p>Hello <strong>{toName}</strong>,</p>
              <p>Unfortunately your order
                <strong>#{orderId[..8].ToUpper()}</strong>
                could not be processed.</p>
              <div style="background:#fdecea;padding:15px;
                          border-radius:8px;border-left:4px solid #e74c3c;
                          margin:15px 0;">
                <strong>Reason:</strong> {reason}
              </div>
              <p>Please contact support or place a new order.</p>
              <hr style="border-color:#d4a96a;">
              <p style="color:#4a2c0a;font-size:12px;text-align:center;">
                © B2B Coffee Roastery Platform</p>
            </div>
            """,
            "OrderRejected");

    // ── PDF Invoice Generator (QuestPDF) ──────────────────────────────────
    private static byte[] GenerateInvoicePdf(
        string orderId, string clientName,
        List<OrderItemDto> items, decimal total)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Text("☕ B2B Coffee Roastery")
                            .FontSize(22).Bold()
                            .FontColor(Color.FromHex("#4a2c0a"));
                        row.ConstantItem(150).AlignRight()
                            .Text($"INVOICE")
                            .FontSize(18).Bold()
                            .FontColor(Color.FromHex("#4a2c0a"));
                    });
                    col.Item().PaddingTop(5)
                        .LineHorizontal(1)
                        .LineColor(Color.FromHex("#d4a96a"));
                });

                page.Content().PaddingTop(20).Column(col =>
                {
                    // Invoice meta
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text($"Bill To: {clientName}").Bold();
                            c.Item().Text(
                                $"Order ID: #{orderId[..8].ToUpper()}");
                            c.Item().Text(
                                $"Date: {DateTime.UtcNow:dd MMM yyyy}");
                        });
                        row.ConstantItem(150).Column(c =>
                        {
                            c.Item().AlignRight()
                                .Text("Status: Confirmed")
                                .FontColor(Color.FromHex("#27ae60")).Bold();
                        });
                    });

                    col.Item().PaddingTop(20).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.RelativeColumn(4);
                            cols.RelativeColumn(1);
                            cols.RelativeColumn(2);
                            cols.RelativeColumn(2);
                        });

                        // Header
                        static IContainer HeaderCell(IContainer c) =>
                            c.Background(Color.FromHex("#4a2c0a"))
                             .Padding(8).AlignCenter();

                        table.Header(h =>
                        {
                            h.Cell().Element(HeaderCell)
                                .Text("Product").FontColor(Colors.White).Bold();
                            h.Cell().Element(HeaderCell)
                                .Text("Qty").FontColor(Colors.White).Bold();
                            h.Cell().Element(HeaderCell)
                                .Text("Unit Price").FontColor(Colors.White).Bold();
                            h.Cell().Element(HeaderCell)
                                .Text("Subtotal").FontColor(Colors.White).Bold();
                        });

                        // Rows
                        foreach (var item in items)
                        {
                            static IContainer BodyCell(IContainer c) =>
                                c.BorderBottom(1)
                                 .BorderColor(Color.FromHex("#d4a96a"))
                                 .Padding(8);

                            table.Cell().Element(BodyCell)
                                .Text(item.ProductName);
                            table.Cell().Element(BodyCell)
                                .AlignCenter().Text(item.Quantity.ToString());
                            table.Cell().Element(BodyCell)
                                .AlignRight().Text($"₹{item.UnitPrice:F2}");
                            table.Cell().Element(BodyCell).AlignRight()
                                .Text($"₹{item.Quantity * item.UnitPrice:F2}");
                        }
                    });

                    // Total
                    col.Item().PaddingTop(10).AlignRight()
                        .Text($"Total: ₹{total:F2}")
                        .FontSize(16).Bold()
                        .FontColor(Color.FromHex("#4a2c0a"));
                });

                page.Footer().AlignCenter()
                    .Text("Thank you for your business — B2B Coffee Roastery")
                    .FontSize(10).FontColor(Color.FromHex("#888888"));
            });
        }).GeneratePdf();
    }
}