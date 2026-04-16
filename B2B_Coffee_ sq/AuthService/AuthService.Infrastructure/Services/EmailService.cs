using AuthService.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Threading.Tasks;

// Using alias to resolve the conflict with System.Net.Mail
using MailKitSmtp = MailKit.Net.Smtp.SmtpClient;

namespace AuthService.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration config) => _config = config;

    private async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
    {
        // Added fallbacks to prevent null reference warnings (CS8604)
        var host = _config["SmtpSettings:Host"] ?? "smtp.gmail.com";
        var port = int.Parse(_config["SmtpSettings:Port"] ?? "587");
        var senderEmail = _config["SmtpSettings:SenderEmail"] ?? throw new InvalidOperationException("SenderEmail not configured.");
        var senderName = _config["SmtpSettings:SenderName"] ?? "Ember&Bean";
        var password = _config["SmtpSettings:Password"] ?? throw new InvalidOperationException("SMTP Password not configured.");

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(senderName, senderEmail));
        message.To.Add(new MailboxAddress(toName, toEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new MailKitSmtp();
        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(senderEmail, password);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendOtpEmailAsync(string toEmail, string toName, string otp) =>
        await SendEmailAsync(toEmail, toName, "☕ Your B2B Coffee OTP", $"<h1>{otp}</h1>");

    public async Task SendWelcomeEmailAsync(string toEmail, string toName) =>
        await SendEmailAsync(toEmail, toName, "🎉 Welcome!", $"<p>Hello {toName}, welcome to Ember&Bean!</p>");

    public async Task SendOrderConfirmedEmailAsync(string toEmail, string toName, string orderId, decimal total) =>
        await SendEmailAsync(toEmail, toName, "✅ Order Confirmed", $"<p>Order {orderId} confirmed for ₹{total}.</p>");

    public async Task SendOrderStatusEmailAsync(string toEmail, string toName, string orderId, string status, string? trackingNumber) =>
        await SendEmailAsync(toEmail, toName, "📦 Order Status Update", $"<p>Order {orderId} is now {status}.</p>");

    public async Task SendOrderRejectedEmailAsync(string toEmail, string toName, string orderId, string reason) =>
        await SendEmailAsync(toEmail, toName, "❌ Order Rejected", $"<p>Order {orderId} was rejected. Reason: {reason}</p>");
}