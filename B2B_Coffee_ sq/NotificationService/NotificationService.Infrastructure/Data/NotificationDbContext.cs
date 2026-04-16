using Microsoft.EntityFrameworkCore;
using NotificationService.Application.Interfaces;
using NotificationService.Domain.Entities;
using System;

namespace NotificationService.Infrastructure.Data;

public class NotificationDbContext : DbContext, INotificationDbContext
{
    public NotificationDbContext(
        DbContextOptions<NotificationDbContext> options) : base(options) { }

    public DbSet<NotificationLog> NotificationLogs { get; set; } = null!;
    public DbSet<SmtpConfig> SmtpConfigs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NotificationLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.RecipientEmail).IsRequired().HasMaxLength(256);
            e.Property(x => x.Subject).IsRequired().HasMaxLength(512);
            e.Property(x => x.NotificationType).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<SmtpConfig>(e => e.HasKey(x => x.Id));

        // Seed SMTP — same Gmail App Password as AuthService
        modelBuilder.Entity<SmtpConfig>().HasData(new SmtpConfig
        {
            Id = 1,
            Host = "smtp.gmail.com",
            Port = 587,
            SenderEmail = "ashigupta2809@gmail.com",
            SenderName = "B2B Coffee Roastery",
            Password = "labu hrjv njfs zsxs",
            EnableSsl = true,
            IsActive = true,
            UpdatedAt = new DateTime(2024, 1, 1)
        });
    }
}