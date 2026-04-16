using System.Threading.Tasks;

namespace AuthService.Application.Interfaces;

public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string toName, string otp);
    Task SendWelcomeEmailAsync(string toEmail, string toName);
    Task SendOrderConfirmedEmailAsync(string toEmail, string toName, string orderId, decimal total);
    Task SendOrderStatusEmailAsync(string toEmail, string toName, string orderId, string status, string? trackingNumber);
    Task SendOrderRejectedEmailAsync(string toEmail, string toName, string orderId, string reason);
}