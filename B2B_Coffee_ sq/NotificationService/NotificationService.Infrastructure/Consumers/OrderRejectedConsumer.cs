using B2B.Contracts.Events.Order;
using MassTransit;
using NotificationService.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Consumers;

public class OrderRejectedConsumer : IConsumer<IOrderRejectedEvent>
{
    private readonly IEmailService _email;

    public OrderRejectedConsumer(IEmailService email) => _email = email;

    public async Task Consume(ConsumeContext<IOrderRejectedEvent> context)
    {
        var msg = context.Message;

        await _email.SendOrderRejectedEmailAsync(
            msg.ClientEmail, msg.ClientName ?? "Customer",
            msg.OrderId.ToString(),
            msg.Reason);

        Console.WriteLine(
            $"[NotificationService] Rejection email sent to {msg.ClientEmail}");
    }
}