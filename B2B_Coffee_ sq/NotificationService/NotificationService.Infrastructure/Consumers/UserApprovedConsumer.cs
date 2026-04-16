using B2B.Contracts.Events.Auth;
using MassTransit;
using NotificationService.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Consumers;

public class UserApprovedConsumer : IConsumer<IUserApprovedEvent>
{
    private readonly IEmailService _email;

    public UserApprovedConsumer(IEmailService email) => _email = email;

    public async Task Consume(ConsumeContext<IUserApprovedEvent> context)
    {
        var msg = context.Message;
        await _email.SendWelcomeEmailAsync(msg.Email, msg.FullName);

        Console.WriteLine(
            $"[NotificationService] Welcome email sent to {msg.Email}");
    }
}