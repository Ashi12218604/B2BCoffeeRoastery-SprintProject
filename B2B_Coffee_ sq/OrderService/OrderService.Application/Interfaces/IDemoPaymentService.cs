using System;

namespace OrderService.Application.Interfaces;

public interface IDemoPaymentService
{
    string GenerateRazorpayOrderId(Guid orderId, decimal amount);
    (string PaymentId, string Signature) GenerateFakeSuccessPaymentDetails(string razorpayOrderId);
    bool VerifySignature(string razorpayOrderId, string razorpayPaymentId, string signature);
}
