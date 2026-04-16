using AuthService.Application.Interfaces;
using AuthService.Domain.Enums;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Commands.TerminateClient;

public class TerminateClientCommandHandler
    : IRequestHandler<TerminateClientCommand, TerminateClientResult>
{
    private readonly IAuthDbContext _db;

    public TerminateClientCommandHandler(IAuthDbContext db)
    {
        _db = db;
    }

    public async Task<TerminateClientResult> Handle(
        TerminateClientCommand request, CancellationToken ct)
    {
        var client = await _db.Users.FindAsync(
            new object[] { request.ClientId }, ct);

        if (client is null)
            return new TerminateClientResult(false, "Client not found.");

        if (client.Role != UserRole.Client)
            return new TerminateClientResult(false, "Can only terminate clients.");

        client.Status = UserStatus.Rejected;
        await _db.SaveChangesAsync(ct);

        return new TerminateClientResult(true, "Client terminated successfully.");
    }
}
