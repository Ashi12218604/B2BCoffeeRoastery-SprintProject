using AuthService.Application.Interfaces;
using AuthService.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Queries.GetApprovedClients;

public class GetApprovedClientsQueryHandler
    : IRequestHandler<GetApprovedClientsQuery, List<ApprovedClientDto>>
{
    private readonly IAuthDbContext _db;

    public GetApprovedClientsQueryHandler(IAuthDbContext db) => _db = db;

    public async Task<List<ApprovedClientDto>> Handle(
        GetApprovedClientsQuery request, CancellationToken ct) =>
        await _db.Users
            .Where(u => u.Role == UserRole.Client
                     && u.Status == UserStatus.Approved)
            .Select(u => new ApprovedClientDto(
                u.Id, u.FullName, u.Email,
                u.CompanyName, u.PhoneNumber, (u.ApprovedAt ?? u.CreatedAt)))
            .ToListAsync(ct);
}
