using B2B.Contracts.Events.Order;
using DeliveryService.Application.Commands.CreateDelivery;
using MassTransit;
using MediatR;
using System;
using System.Threading.Tasks;

namespace DeliveryService.Infrastructure.Consumers;

// When order is confirmed by saga → auto-create delivery record
public class OrderConfirmedDeliveryConsumer : IConsumer<IOrderConfirmedEvent>
{
    private readonly IMediator _mediator;

    public OrderConfirmedDeliveryConsumer(IMediator mediator)
        => _mediator = mediator;

    public async Task Consume(ConsumeContext<IOrderConfirmedEvent> context)
    {
        var msg = context.Message;

        // Create delivery automatically on order confirmation
        // Address will need to be updated via API
        // (in real world, client provides address at checkout)
        await _mediator.Send(new CreateDeliveryCommand(
            OrderId: msg.OrderId,
            ClientId: msg.ClientId,
            ClientEmail: msg.ClientEmail,
            ClientName: msg.ClientName,
            DeliveryAddress: string.IsNullOrWhiteSpace(msg.DeliveryAddress) ? "Address Pending" : msg.DeliveryAddress,
            City: string.IsNullOrWhiteSpace(msg.City) ? "Pending" : msg.City,
            State: string.IsNullOrWhiteSpace(msg.State) ? "Pending" : msg.State,
            PinCode: string.IsNullOrWhiteSpace(msg.PinCode) ? "000000" : msg.PinCode,
            ApprovedByAdminName: msg.AdminName,
            ProductNames: string.Join(", ", msg.ProductNames),
            EstimatedDeliveryDate: DateTime.UtcNow.AddDays(5),
            Notes: "Auto-created on order confirmation."
        ));

        Console.WriteLine(
            $"[DeliveryService] Delivery created for Order {msg.OrderId}");
    }
}