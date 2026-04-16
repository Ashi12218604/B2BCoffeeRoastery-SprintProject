using MediatR;
using System;

namespace AuthService.Application.Commands.ApproveClient;

public record ApproveClientCommand(Guid ClientId, Guid ApprovedBy, bool Approve, string? RejectionReason = null)
	: IRequest<ApproveClientResult>;

public record ApproveClientResult(bool Success, string Message);