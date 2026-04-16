using MediatR;
using OrderService.Application.DTOs;
using System;

namespace OrderService.Application.Commands.VerifyPayment;

public record VerifyPaymentCommand(
    Guid OrderId,
    Guid ClientId,
    string RazorpayPaymentId,
    string RazorpaySignature
) : IRequest<PaymentResponseDto>;
