using B2B.Contracts.Events.Order;
using MassTransit;
using NotificationService.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Consumers;

public class OrderStatusChangedConsumer : IConsumer<IOrderStatusChangedEvent>
{
    private readonly IEmailService _email;

    public OrderStatusChangedConsumer(IEmailService email) => _email = email;

    public async Task Consume(
        ConsumeContext<IOrderStatusChangedEvent> context)
    {
        var msg = context.Message;

        await _email.SendOrderStatusEmailAsync(
            msg.ClientEmail, msg.ClientName,
            msg.OrderId.ToString(),
            msg.NewStatus,
            msg.TrackingNumber);

        Console.WriteLine(
            $"[NotificationService] Status '{msg.NewStatus}' email sent to {msg.ClientEmail}");
    }
}