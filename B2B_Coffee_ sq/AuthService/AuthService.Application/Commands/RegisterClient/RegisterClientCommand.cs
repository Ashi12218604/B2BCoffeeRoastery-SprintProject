using MediatR;
using System;

namespace AuthService.Application.Commands.RegisterClient;

public record RegisterClientCommand(
    string FullName,
    string Email,
    string Password,
    string PhoneNumber,
    string CompanyName
) : IRequest<RegisterClientResult>;

public record RegisterClientResult(bool Success, string Message, Guid? UserId = null);