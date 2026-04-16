using B2B.Contracts.Events.Order;
using DeliveryService.Application.Commands.UpdateDeliveryStatus;
using DeliveryService.Application.Commands.CreateDelivery;
using DeliveryService.Application.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DeliveryService.Infrastructure.Consumers;

public class OrderStatusChangedDeliveryConsumer : IConsumer<IOrderStatusChangedEvent>
{
    private readonly IMediator _mediator;
    private readonly IDeliveryDbContext _db;

    public OrderStatusChangedDeliveryConsumer(IMediator mediator, IDeliveryDbContext db)
    {
        _mediator = mediator;
        _db = db;
    }

    public async Task Consume(ConsumeContext<IOrderStatusChangedEvent> context)
    {
        var msg = context.Message;
        
        // Find existing delivery
        var existingDelivery = await _db.Deliveries.FirstOrDefaultAsync(d => d.OrderId == msg.OrderId);
        
        if (existingDelivery == null)
        {
            // If the order reached confirmed or above without a delivery, create one.
            if (msg.NewStatus == "Confirmed" || msg.NewStatus == "InProcess" || msg.NewStatus == "Dispatched" || msg.NewStatus == "In-Process" || msg.NewStatus == "Delivered")
            {
                await _mediator.Send(new CreateDeliveryCommand(
                    OrderId: msg.OrderId,
                    ClientId: msg.ClientId,
                    ClientEmail: msg.ClientEmail,
                    ClientName: msg.ClientName,
                    DeliveryAddress: "Address Pending",
                    City: "Pending",
                    State: "Pending",
                    PinCode: "000000",
                    ApprovedByAdminName: "SuperAdmin Oversight",
                    ProductNames: "Items updated manually",
                    EstimatedDeliveryDate: DateTime.UtcNow.AddDays(5),
                    Notes: $"Auto-created on manual status update to {msg.NewStatus}."
                ));
                Console.WriteLine($"[DeliveryService] Auto-created delivery for Order {msg.OrderId} due to status shift to {msg.NewStatus}");
            }
        }
        else
        {
            // Update existing delivery
            int newDeliveryStatus = -1;
            
            // Note: Delivered=4, Dispatched=3, InProcess=2, Assigned=1, Pending=0
            if (msg.NewStatus == "Delivered") newDeliveryStatus = 4;
            else if (msg.NewStatus == "Dispatched") newDeliveryStatus = 3;
            else if (msg.NewStatus == "In-Process" || msg.NewStatus == "InProcess") newDeliveryStatus = 2;
            
            if (newDeliveryStatus != -1 && (int)existingDelivery.Status != newDeliveryStatus)
            {
                await _mediator.Send(new UpdateDeliveryStatusCommand(
                    existingDelivery.Id, 
                    (DeliveryService.Domain.Enums.DeliveryStatus)newDeliveryStatus,
                    $"Status auto-updated to {msg.NewStatus} from order system.",
                    null, null, null, null));
                Console.WriteLine($"[DeliveryService] Updated delivery {existingDelivery.Id} status to {msg.NewStatus} based on Order update.");
            }
        }
    }
}
