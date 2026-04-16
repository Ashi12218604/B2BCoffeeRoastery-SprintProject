using AuthService.Application.Interfaces;
using AuthService.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Queries.GetPendingClients;

public class GetPendingClientsQueryHandler
    : IRequestHandler<GetPendingClientsQuery, List<PendingClientDto>>
{
    private readonly IAuthDbContext _db;

    public GetPendingClientsQueryHandler(IAuthDbContext db) => _db = db;

    public async Task<List<PendingClientDto>> Handle(
        GetPendingClientsQuery request, CancellationToken ct) =>
        await _db.Users
            .Where(u => u.Role == UserRole.Client
                     && u.Status == UserStatus.Pending)
            .Select(u => new PendingClientDto(
                u.Id, u.FullName, u.Email,
                u.CompanyName, u.PhoneNumber, u.CreatedAt))
            .ToListAsync(ct);
}