using AuthService.Application.Interfaces;
using AuthService.Domain.Enums;
using B2B.Contracts.Events.Auth;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.VerifyOtp;

public class VerifyOtpCommandHandler
    : IRequestHandler<VerifyOtpCommand, VerifyOtpResult>
{
    private readonly IAuthDbContext _db;
    private readonly IPublishEndpoint _publishEndpoint;

    public VerifyOtpCommandHandler(
        IAuthDbContext db, IPublishEndpoint publishEndpoint)
    {
        _db = db;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<VerifyOtpResult> Handle(
        VerifyOtpCommand request, CancellationToken ct)
    {
        var user = await _db.Users
            .Include(u => u.OtpRecords)
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (user is null)
            return new VerifyOtpResult(false, "User not found.");

        if (user.Status != UserStatus.PendingOtp)
            return new VerifyOtpResult(false, "OTP already verified.");

        var otp = user.OtpRecords
            .Where(o => !o.IsUsed
                     && o.OtpCode == request.OtpCode
                     && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefault();

        if (otp is null)
            return new VerifyOtpResult(false, "Invalid or expired OTP.");

        otp.IsUsed = true;
        user.Status = UserStatus.Pending;
        user.IsEmailVerified = true;

        await _db.SaveChangesAsync(ct);

        await _publishEndpoint.Publish<IOtpVerifiedEvent>(new
        {
            UserId = user.Id,
            user.Email,
            VerifiedAt = DateTime.UtcNow
        }, ct);

        return new VerifyOtpResult(
            true,
            "OTP verified. Account is pending Admin approval.");
    }
}