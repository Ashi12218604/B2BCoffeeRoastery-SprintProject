using B2B.Contracts.Events.Payment;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.Application.Commands.VerifyPayment;

public class VerifyPaymentCommandHandler : IRequestHandler<VerifyPaymentCommand, PaymentResponseDto>
{
    private readonly IOrderDbContext _db;
    private readonly IDemoPaymentService _paymentService;
    private readonly IPublishEndpoint _publish;

    public VerifyPaymentCommandHandler(IOrderDbContext db, IDemoPaymentService paymentService, IPublishEndpoint publish)
    {
        _db = db;
        _paymentService = paymentService;
        _publish = publish;
    }

    public async Task<PaymentResponseDto> Handle(VerifyPaymentCommand request, CancellationToken ct)
    {
        var order = await _db.Orders
            .Include(o => o.StatusHistory)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.ClientId == request.ClientId, ct);

        if (order is null)
            return new PaymentResponseDto(false, "Order not found or access denied.");

        if (order.PaymentStatus == PaymentStatus.Paid)
            return new PaymentResponseDto(false, "Order is already paid.");

        if (string.IsNullOrEmpty(order.RazorpayOrderId))
            return new PaymentResponseDto(false, "Payment order was not generated for this order.");

        // Verify Razorpay Signature
        bool isValid = _paymentService.VerifySignature(order.RazorpayOrderId, request.RazorpayPaymentId, request.RazorpaySignature);

        if (!isValid)
        {
            await _publish.Publish<IPaymentFailedEvent>(new
            {
                OrderId = order.Id,
                CorrelationId = Guid.NewGuid(),
                Reason = "Invalid Signature",
                FailedAt = DateTime.UtcNow
            }, ct);
            return new PaymentResponseDto(false, "Invalid payment signature verification failed.");
        }

        // Apply Payment via direct SQL to bypass Saga concurrency locks
        await _db.Orders.Where(o => o.Id == order.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(p => p.PaymentStatus, PaymentStatus.Paid)
                .SetProperty(p => p.RazorpayPaymentId, request.RazorpayPaymentId)
                .SetProperty(p => p.RazorpaySignature, request.RazorpaySignature)
                .SetProperty(p => p.PaidAt, DateTime.UtcNow)
                .SetProperty(p => p.UpdatedAt, DateTime.UtcNow), ct);

        // Still update the local object for the rest of the flow
        order.PaymentStatus = PaymentStatus.Paid;
        order.RazorpayPaymentId = request.RazorpayPaymentId;
        order.RazorpaySignature = request.RazorpaySignature;
        order.PaidAt = DateTime.UtcNow;

        if (order.Status < OrderStatus.Confirmed)
        {
            await _db.Orders.Where(o => o.Id == order.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, OrderStatus.Confirmed), ct);
            order.Status = OrderStatus.Confirmed;
            
            _db.OrderStatusHistories.Add(new OrderStatusHistory
            {
                OrderId = order.Id,
                Status = OrderStatus.Confirmed,
                Note = "Payment verified successfully. Order confirmed."
            });
            await _db.SaveChangesAsync(ct);
        }

        // Publish event
        await _publish.Publish<IPaymentProcessedEvent>(new
        {
            OrderId = order.Id,
            order.ClientId,
            order.ClientEmail,
            order.ClientName,
            Amount = order.TotalAmount,
            TransactionId = order.RazorpayPaymentId,
            ProcessedAt = DateTime.UtcNow
        }, ct);

        return new PaymentResponseDto(true, "Payment verified and order confirmed successfully.");
    }
}
