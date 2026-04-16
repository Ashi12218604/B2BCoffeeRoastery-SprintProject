using MediatR;
using System;

namespace AuthService.Application.Commands.DeleteAdmin;

public record DeleteAdminCommand(Guid AdminId) : IRequest<DeleteAdminResult>;

public record DeleteAdminResult(bool Success, string Message);
