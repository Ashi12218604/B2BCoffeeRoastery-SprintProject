using B2B.Contracts.Events.Auth;
using MassTransit;
using NotificationService.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Consumers;

public class UserRegisteredConsumer : IConsumer<IUserRegisteredEvent>
{
    private readonly IEmailService _email;

    public UserRegisteredConsumer(IEmailService email) => _email = email;

    public async Task Consume(ConsumeContext<IUserRegisteredEvent> context)
    {
        var msg = context.Message;
        await _email.SendOtpEmailAsync(
            msg.Email, msg.FullName, msg.OtpCode);

        Console.WriteLine(
            $"[NotificationService] OTP email sent to {msg.Email}");
    }
}