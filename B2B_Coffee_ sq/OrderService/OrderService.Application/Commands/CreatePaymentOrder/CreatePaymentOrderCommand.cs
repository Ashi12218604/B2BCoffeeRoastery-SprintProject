using MediatR;
using OrderService.Application.DTOs;
using System;

namespace OrderService.Application.Commands.CreatePaymentOrder;

public record CreatePaymentOrderCommand(
    Guid OrderId,
    Guid ClientId
) : IRequest<PaymentResponseDto>;
