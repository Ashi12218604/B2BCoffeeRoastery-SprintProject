using MediatR;
using System;

namespace AuthService.Application.Commands.TerminateClient;

public record TerminateClientCommand(Guid ClientId, Guid TerminatedBy) : IRequest<TerminateClientResult>;

public record TerminateClientResult(bool Success, string Message);
