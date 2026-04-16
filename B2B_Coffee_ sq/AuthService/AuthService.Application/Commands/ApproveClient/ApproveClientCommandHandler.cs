using AuthService.Application.Interfaces;
using AuthService.Domain.Enums;
using B2B.Contracts.Events.Auth;
using MassTransit;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.ApproveClient;

public class ApproveClientCommandHandler
    : IRequestHandler<ApproveClientCommand, ApproveClientResult>
{
    private readonly IAuthDbContext _db;
    private readonly IPublishEndpoint _publishEndpoint;

    public ApproveClientCommandHandler(
        IAuthDbContext db, IPublishEndpoint publishEndpoint)
    {
        _db = db;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<ApproveClientResult> Handle(
        ApproveClientCommand request, CancellationToken ct)
    {
        var client = await _db.Users.FindAsync(
            new object[] { request.ClientId }, ct);

        if (client is null)
            return new ApproveClientResult(false, "Client not found.");

        if (client.Status != UserStatus.Pending)
            return new ApproveClientResult(
                false, "Client is not in Pending state.");

        if (request.Approve)
        {
            client.Status = UserStatus.Approved;
            client.ApprovedAt = DateTime.UtcNow;
            client.ApprovedBy = request.ApprovedBy;
            await _db.SaveChangesAsync(ct);

            // Publish → NotificationService sends Welcome email
            await _publishEndpoint.Publish<IUserApprovedEvent>(new
            {
                UserId = client.Id,
                client.Email,
                client.FullName,
                ApprovedAt = DateTime.UtcNow
            }, ct);

            return new ApproveClientResult(true, "Client approved.");
        }

        client.Status = UserStatus.Rejected;
        await _db.SaveChangesAsync(ct);
        return new ApproveClientResult(
            true, $"Client rejected. Reason: {request.RejectionReason}");
    }
}