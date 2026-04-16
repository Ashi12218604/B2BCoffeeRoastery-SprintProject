using B2B.Contracts.Events.Order;
using MassTransit;
using NotificationService.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Consumers;

public class OrderConfirmedConsumer : IConsumer<IOrderConfirmedEvent>
{
    private readonly IEmailService _email;

    public OrderConfirmedConsumer(IEmailService email) => _email = email;

    public async Task Consume(ConsumeContext<IOrderConfirmedEvent> context)
    {
        var msg = context.Message;

        await _email.SendOrderStatusEmailAsync(
            msg.ClientEmail, msg.ClientName,
            msg.OrderId.ToString(),
            "In-Process");

        Console.WriteLine(
            $"[NotificationService] Order confirmed email sent to {msg.ClientEmail}");
    }
}