using MediatR;
using System;

namespace AuthService.Application.Commands.CreateAdmin;

public record CreateAdminCommand(
    string FullName,
    string Email,
    string Password,
    string PhoneNumber
) : IRequest<CreateAdminResult>;

public record CreateAdminResult(bool Success, string Message, Guid? UserId = null);
