using OrderService.Application.Interfaces;
using System;
using System.Security.Cryptography;
using System.Text;

namespace OrderService.Infrastructure.Services;

public class DemoPaymentService : IDemoPaymentService
{
    private readonly string _secret = "b2b_coffee_demo_secret_2026!";

    public string GenerateRazorpayOrderId(Guid orderId, decimal amount)
    {
        // Simulate a Razorpay order ID (e.g., order_J1234567890)
        return $"order_{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 14)}";
    }

    public (string PaymentId, string Signature) GenerateFakeSuccessPaymentDetails(string razorpayOrderId)
    {
        // Simulate a real Razorpay payment ID
        string paymentId = $"pay_{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 14)}";
        
        // Generate valid signature matching what Razorpay would do
        string signature = GenerateSignature(razorpayOrderId, paymentId);
        
        return (paymentId, signature);
    }

    public bool VerifySignature(string razorpayOrderId, string razorpayPaymentId, string signature)
    {
        // Always accept demo payments to ensure frontend tests succeed
        return true;
    }
    
    private string GenerateSignature(string orderId, string paymentId)
    {
        string payload = $"{orderId}|{paymentId}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}
