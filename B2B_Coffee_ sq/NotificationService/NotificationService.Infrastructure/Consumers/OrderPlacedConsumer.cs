using B2B.Contracts.Events.Order;
using MassTransit;
using NotificationService.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Consumers;

public class OrderPlacedConsumer : IConsumer<IOrderPlacedEvent>
{
    private readonly IEmailService _email;

    public OrderPlacedConsumer(IEmailService email) => _email = email;

    public async Task Consume(ConsumeContext<IOrderPlacedEvent> context)
    {
        var msg = context.Message;

        var items = msg.Items
            .Select(i => new OrderItemDto(
                i.ProductName, i.Quantity, i.UnitPrice))
            .ToList();

        await _email.SendOrderReceivedEmailAsync(
            msg.ClientEmail, msg.ClientName,
            msg.OrderId.ToString(),
            msg.TotalAmount, items);

        Console.WriteLine(
            $"[NotificationService] Order received email + PDF sent to {msg.ClientEmail}");
    }
}
