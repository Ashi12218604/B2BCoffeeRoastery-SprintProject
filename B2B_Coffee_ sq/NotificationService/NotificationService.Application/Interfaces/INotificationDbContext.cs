using Microsoft.EntityFrameworkCore;
using NotificationService.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace NotificationService.Application.Interfaces;

public interface INotificationDbContext
{
    DbSet<NotificationLog> NotificationLogs { get; }
    DbSet<SmtpConfig> SmtpConfigs { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}