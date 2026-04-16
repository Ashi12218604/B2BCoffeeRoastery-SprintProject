using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.DTOs;
using OrderService.Application.Interfaces;
using OrderService.Domain.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrderService.Application.Commands.CreatePaymentOrder;

public class CreatePaymentOrderCommandHandler : IRequestHandler<CreatePaymentOrderCommand, PaymentResponseDto>
{
    private readonly IOrderDbContext _db;
    private readonly IDemoPaymentService _paymentService;

    public CreatePaymentOrderCommandHandler(IOrderDbContext db, IDemoPaymentService paymentService)
    {
        _db = db;
        _paymentService = paymentService;
    }

    public async Task<PaymentResponseDto> Handle(CreatePaymentOrderCommand request, CancellationToken ct)
    {
        var order = await _db.Orders
            .FirstOrDefaultAsync(o => o.Id == request.OrderId && o.ClientId == request.ClientId, ct);

        if (order is null)
            return new PaymentResponseDto(false, "Order not found or access denied.");

        if (order.PaymentStatus == PaymentStatus.Paid)
            return new PaymentResponseDto(false, "Order is already paid.");

        if (order.Status == OrderStatus.Cancelled || order.Status == OrderStatus.Rejected)
            return new PaymentResponseDto(false, "Cannot pay for cancelled or rejected orders.");

        // Generate Razorpay style order ID
        string razorpayOrderId = _paymentService.GenerateRazorpayOrderId(order.Id, order.TotalAmount);

        // Update DB directly to avoid concurrency issues during rapid checkout flow
        await _db.Orders
            .Where(o => o.Id == order.Id)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.RazorpayOrderId, razorpayOrderId), ct);

        // Generate demo success payload for frontend test convenience
        var demoResponse = _paymentService.GenerateFakeSuccessPaymentDetails(razorpayOrderId);


        return new PaymentResponseDto(
            true, 
            "Payment order generated successfully.",
            razorpayOrderId,
            demoResponse.PaymentId,
            demoResponse.Signature
        );
    }
}
