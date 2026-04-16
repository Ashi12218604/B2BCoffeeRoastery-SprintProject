using AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces;

public interface IAuthDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<OtpRecord> OtpRecords { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}