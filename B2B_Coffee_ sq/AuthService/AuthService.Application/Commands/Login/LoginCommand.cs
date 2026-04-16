using MediatR;

namespace AuthService.Application.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;
public record LoginResult(bool Success, string Message, string? Token = null, string? Role = null);