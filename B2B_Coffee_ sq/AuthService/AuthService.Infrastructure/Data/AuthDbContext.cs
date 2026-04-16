using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Data;

public class AuthDbContext : DbContext, IAuthDbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options)
        : base(options) { }

    public DbSet<ApplicationUser> Users { get; set; } = null!;
    public DbSet<OtpRecord> OtpRecords { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Keeps .NET 10 Preview from crashing on minor model mismatches
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(RelationalEventId.PendingModelChangesWarning));

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ApplicationUser>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Email).IsUnique();
            e.Property(x => x.Role).HasConversion<int>();
            e.Property(x => x.Status).HasConversion<int>();
            e.HasMany(x => x.OtpRecords)
             .WithOne(x => x.User)
             .HasForeignKey(x => x.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OtpRecord>(e => e.HasKey(x => x.Id));

        // Seed SuperAdmin - Keeping this here is fine as it's the core system user
        modelBuilder.Entity<ApplicationUser>().HasData(new ApplicationUser
        {
            Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
            FullName = "Super Admin",
            Email = "superadmin@b2bcoffee.com",
            PasswordHash = "$2a$11$hhLH.mbgOD75YxVkUbIVwumhfMXxLKzB5PPragDAaekWNolt.Ufx6",
            Role = UserRole.SuperAdmin,
            Status = UserStatus.Approved,
            IsEmailVerified = true,
            PhoneNumber = "0000000000",
            CompanyName = "Ember&Bean Roastery"
        });
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}