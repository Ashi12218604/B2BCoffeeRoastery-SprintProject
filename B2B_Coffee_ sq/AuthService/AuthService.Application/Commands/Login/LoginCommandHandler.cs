using AuthService.Application.Interfaces;
using AuthService.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.Login;

public class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IAuthDbContext _db;
    private readonly IJwtService _jwt;

    public LoginCommandHandler(IAuthDbContext db, IJwtService jwt)
    {
        _db = db;
        _jwt = jwt;
    }

    public async Task<LoginResult> Handle(
        LoginCommand request, CancellationToken ct)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, ct);

        if (user is null ||
            !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return new LoginResult(false, "Invalid email or password.");

        if (user.Status == UserStatus.Rejected)
        {
            return new LoginResult(
                false, "Your account has been terminated or rejected. Please contact support.");
        }

        if (user.Role == UserRole.Client)
        {
            if (user.Status == UserStatus.PendingOtp)
                return new LoginResult(false, "Please verify your OTP first.");
            if (user.Status == UserStatus.Pending)
                return new LoginResult(
                    false, "Your account is pending Admin approval.");
        }

        var token = _jwt.GenerateToken(user);
        return new LoginResult(true, "Login successful.", token,
            user.Role.ToString());
    }
}