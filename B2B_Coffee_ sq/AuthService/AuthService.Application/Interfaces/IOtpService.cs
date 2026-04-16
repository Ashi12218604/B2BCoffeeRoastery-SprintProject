using System;

namespace AuthService.Application.Interfaces;

public interface IOtpService
{
    string GenerateOtp();
    DateTime GetExpiry(int minutes = 10);
}