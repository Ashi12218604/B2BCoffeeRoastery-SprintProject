using System.Collections.Generic;
using System.Threading.Tasks;

namespace NotificationService.Application.Interfaces;

public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string toName, string otp);
    Task SendWelcomeEmailAsync(string toEmail, string toName);
    Task SendOrderReceivedEmailAsync(
        string toEmail, string toName,
        string orderId, decimal total,
        List<OrderItemDto> items);
    Task SendOrderStatusEmailAsync(
        string toEmail, string toName,
        string orderId, string status,
        string? trackingNumber = null);
    Task SendOrderRejectedEmailAsync(
        string toEmail, string toName,
        string orderId, string reason);
}

public record OrderItemDto(
    string ProductName, int Quantity, decimal UnitPrice);