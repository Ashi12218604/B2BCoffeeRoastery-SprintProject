using AuthService.Application.Interfaces;
using AuthService.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Queries.GetAdmins;

public class GetAdminsQueryHandler
    : IRequestHandler<GetAdminsQuery, List<AdminDto>>
{
    private readonly IAuthDbContext _db;

    public GetAdminsQueryHandler(IAuthDbContext db) => _db = db;

    public async Task<List<AdminDto>> Handle(
        GetAdminsQuery request, CancellationToken ct) =>
        await _db.Users
            .Where(u => u.Role == UserRole.Admin)
            .Select(u => new AdminDto(
                u.Id, u.FullName, u.Email, u.PhoneNumber, (u.ApprovedAt ?? u.CreatedAt)))
            .ToListAsync(ct);
}
