using B2B.Contracts.Events.Delivery;
using MassTransit;
using NotificationService.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Consumers;

public class DeliveryStatusChangedConsumer
    : IConsumer<IDeliveryStatusChangedEvent>
{
    private readonly IEmailService _email;

    public DeliveryStatusChangedConsumer(IEmailService email)
        => _email = email;

    public async Task Consume(
        ConsumeContext<IDeliveryStatusChangedEvent> context)
    {
        var msg = context.Message;

        await _email.SendOrderStatusEmailAsync(
            msg.ClientEmail,
            msg.ClientName,
            msg.OrderId.ToString(),
            msg.Status,
            msg.TrackingNumber);

        Console.WriteLine(
            $"[NotificationService] Delivery '{msg.Status}' " +
            $"email sent to {msg.ClientEmail}");
    }
}