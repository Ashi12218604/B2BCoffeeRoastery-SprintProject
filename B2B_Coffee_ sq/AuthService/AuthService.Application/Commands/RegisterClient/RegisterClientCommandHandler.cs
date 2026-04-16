using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using B2B.Contracts.Events.Auth;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.RegisterClient;

public class RegisterClientCommandHandler
    : IRequestHandler<RegisterClientCommand, RegisterClientResult>
{
    private readonly IAuthDbContext _db;
    private readonly IOtpService _otpService;
    private readonly IPublishEndpoint _publishEndpoint;

    public RegisterClientCommandHandler(
        IAuthDbContext db,
        IOtpService otpService,
        IPublishEndpoint publishEndpoint)
    {
        _db = db;
        _otpService = otpService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<RegisterClientResult> Handle(
        RegisterClientCommand request, CancellationToken ct)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email, ct))
            return new RegisterClientResult(false, "Email already registered.");

        var user = new ApplicationUser
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber,
            CompanyName = request.CompanyName,
            Role = UserRole.Client,
            Status = UserStatus.PendingOtp
        };

        var otpCode = _otpService.GenerateOtp();
        var otpRecord = new OtpRecord
        {
            UserId = user.Id,
            OtpCode = otpCode,
            ExpiresAt = _otpService.GetExpiry(10)
        };

        _db.Users.Add(user);
        _db.OtpRecords.Add(otpRecord);
        await _db.SaveChangesAsync(ct);

        // Publish → NotificationService will send OTP email via RabbitMQ
        await _publishEndpoint.Publish<IUserRegisteredEvent>(new
        {
            UserId = user.Id,
            user.Email,
            user.FullName,
            OtpCode = otpCode,
            RegisteredAt = DateTime.UtcNow
        }, ct);

        return new RegisterClientResult(
            true,
            "Registration successful. Please check your email for OTP.",
            user.Id);
    }
}