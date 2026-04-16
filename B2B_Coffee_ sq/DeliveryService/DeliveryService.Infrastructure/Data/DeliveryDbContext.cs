using DeliveryService.Application.Interfaces;
using DeliveryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Infrastructure.Data;

public class DeliveryDbContext : DbContext, IDeliveryDbContext
{
    public DeliveryDbContext(
        DbContextOptions<DeliveryDbContext> options) : base(options) { }

    public DbSet<Delivery> Deliveries { get; set; } = null!;
    public DbSet<DeliveryStatusHistory> DeliveryStatusHistories { get; set; }
        = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Delivery>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.OrderId).IsUnique();
            e.HasIndex(x => x.TrackingNumber).IsUnique();
            e.Property(x => x.Status).HasConversion<int>();
            e.HasMany(x => x.StatusHistory)
             .WithOne(x => x.Delivery)
             .HasForeignKey(x => x.DeliveryId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DeliveryStatusHistory>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Status).HasConversion<int>();
        });
    }
}