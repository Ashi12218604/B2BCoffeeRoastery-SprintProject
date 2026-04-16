using B2B.Contracts.Events.Order;
using InventoryService.Application.Commands.RestoreInventory;
using MassTransit;
using MediatR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryService.Infrastructure.Consumers;

public class OrderCancelledConsumer : IConsumer<IOrderCancelledEvent>
{
    private readonly IMediator _mediator;

    public OrderCancelledConsumer(IMediator mediator) => _mediator = mediator;

    public async Task Consume(ConsumeContext<IOrderCancelledEvent> context)
    {
        var msg = context.Message;

        var items = msg.Items
            .Select(i => new InventoryRestoreItem(i.ProductId, i.Quantity))
            .ToList();

        await _mediator.Send(new RestoreInventoryCommand(
            msg.OrderId,
            items,
            msg.Reason));

        Console.WriteLine($"[InventoryService] Restored inventory for cancelled Order {msg.OrderId}");
    }
}
