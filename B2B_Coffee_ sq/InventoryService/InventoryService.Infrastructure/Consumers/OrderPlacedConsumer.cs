using B2B.Contracts.Events.Order;
using InventoryService.Application.Commands.DeductInventory;
using MassTransit;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Consumers;

// This consumer triggers inventory deduction when an order is placed
public class OrderPlacedInventoryConsumer : IConsumer<IOrderPlacedEvent>
{
    private readonly IMediator _mediator;

    public OrderPlacedInventoryConsumer(IMediator mediator)
        => _mediator = mediator;

    public async Task Consume(ConsumeContext<IOrderPlacedEvent> context)
    {
        var msg = context.Message;

        var items = msg.Items
            .Select(i => new DeductInventoryItemRequest(
                i.ProductId, i.Quantity))
            .ToList();

        await _mediator.Send(new DeductInventoryCommand(
            msg.OrderId,
            Guid.NewGuid(),
            items));

        Console.WriteLine(
            $"[InventoryService] Processed deduction for Order {msg.OrderId}");
    }
}