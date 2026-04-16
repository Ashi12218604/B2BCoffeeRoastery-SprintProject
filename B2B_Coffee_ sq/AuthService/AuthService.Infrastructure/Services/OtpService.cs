using AuthService.Application.Interfaces;
using System;

namespace AuthService.Infrastructure.Services;

public class OtpService : IOtpService
{
    private readonly Random _random = new();

    public string GenerateOtp() =>
        _random.Next(100000, 999999).ToString();

    public DateTime GetExpiry(int minutes = 10) =>
        DateTime.UtcNow.AddMinutes(minutes);
}