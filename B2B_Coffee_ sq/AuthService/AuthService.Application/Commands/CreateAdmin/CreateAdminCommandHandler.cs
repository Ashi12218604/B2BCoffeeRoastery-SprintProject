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

namespace AuthService.Application.Commands.CreateAdmin;

public class CreateAdminCommandHandler
    : IRequestHandler<CreateAdminCommand, CreateAdminResult>
{
    private readonly IAuthDbContext _db;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateAdminCommandHandler(IAuthDbContext db, IPublishEndpoint publishEndpoint)
    {
        _db = db;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<CreateAdminResult> Handle(
        CreateAdminCommand request, CancellationToken ct)
    {
        // Check if email already exists
        if (await _db.Users.AnyAsync(u => u.Email == request.Email, ct))
            return new CreateAdminResult(false, "Email already registered.");

        var admin = new ApplicationUser
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber,
            CompanyName = "B2B Coffee Roastery",
            Role = UserRole.Admin,
            Status = UserStatus.Approved,
            IsEmailVerified = true,
            ApprovedAt = DateTime.UtcNow
        };

        _db.Users.Add(admin);
        await _db.SaveChangesAsync(ct);

        // Notify via email (using an empty OTP for pre-approved admins)
        await _publishEndpoint.Publish<IUserRegisteredEvent>(new
        {
            UserId = admin.Id,
            Email = admin.Email,
            FullName = admin.FullName,
            OtpCode = "ADMIN_ACCESS",
            RegisteredAt = DateTime.UtcNow
        }, ct);

        return new CreateAdminResult(
            true,
            $"Admin account created successfully for {request.Email}.",
            admin.Id);
    }
}
