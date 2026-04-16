using Microsoft.EntityFrameworkCore;
using ProductService.Application.Interfaces;
using ProductService.Domain.Entities;
using ProductService.Domain.Enums;
using System;

namespace ProductService.Infrastructure.Data;

public class ProductDbContext : DbContext, IProductDbContext
{
    public ProductDbContext(
        DbContextOptions<ProductDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<ProductReview> ProductReviews { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(e =>
        {
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.SKU).IsUnique();
            e.Property(x => x.Price).HasPrecision(18, 2);
            e.Property(x => x.DiscountedPrice).HasPrecision(18, 2);
            e.Property(x => x.Category).HasConversion<int>();
            e.Property(x => x.RoastLevel).HasConversion<int>();
            e.HasMany(x => x.Reviews)
             .WithOne(x => x.Product)
             .HasForeignKey(x => x.ProductId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ProductReview>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Rating)
             .IsRequired();
        });

        // Seed sample products
        var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = Guid.Parse("A1B2C3D4-1111-4AAA-BBBB-000000000001"),
                Name = "Ethiopian Yirgacheffe",
                Description = "Bright, floral, and fruity with a light body. Notes of jasmine, lemon, and blueberry.",
                SKU = "ETH-YIR-01",
                Price = 1850.00m,
                Origin = "Ethiopia",
                Category = ProductCategory.SingleOrigin,
                RoastLevel = RoastLevel.Light,
                WeightInGrams = 1000,
                ImageUrl = "/assets/images/products/ethiopian-yirgacheffe.png",
                IsFeatured = true,
                MinimumOrderQuantity = 5,
                CreatedBy = adminId,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = Guid.Parse("A1B2C3D4-2222-4AAA-BBBB-000000000002"),
                Name = "Colombian Supremo",
                Description = "Well-balanced with mild acidity. Notes of caramel, chocolate, and walnut.",
                Price = 1650.00m,
                Origin = "Colombia",
                Category = ProductCategory.SingleOrigin,
                RoastLevel = RoastLevel.Medium,
                WeightInGrams = 1000,
                ImageUrl = "/assets/images/products/colombian-supremo.png",
                SKU = "COL-SUP-01",
                IsFeatured = true,
                MinimumOrderQuantity = 5,
                CreatedBy = adminId,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = Guid.Parse("A1B2C3D4-3333-4AAA-BBBB-000000000003"),
                Name = "Italian Espresso Classico",
                Description = "Authentic dark roast espresso blend with a thick crema and hints of spice and cocoa.",
                SKU = "ESP-ITA-01",
                Price = 1550.00m,
                Origin = "Italy/Brazil",
                Category = ProductCategory.EspressoBlend,
                RoastLevel = RoastLevel.Dark,
                WeightInGrams = 1000,
                ImageUrl = "/assets/images/products/italian-espresso.png",
                IsFeatured = true,
                MinimumOrderQuantity = 10,
                CreatedBy = adminId,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = Guid.Parse("A1B2C3D4-5555-4AAA-BBBB-000000000005"),
                Name = "Guatemala Antigua",
                Description = "Famous for its complex flavor profile. Spicy, smokey, and chocolaty with floral hints.",
                SKU = "GUA-ANT-01",
                Price = 1950.00m,
                Origin = "Guatemala",
                Category = ProductCategory.SingleOrigin,
                RoastLevel = RoastLevel.Medium,
                WeightInGrams = 1000,
                ImageUrl = "/assets/images/products/guatemala-antigua.png",
                IsFeatured = false,
                MinimumOrderQuantity = 5,
                CreatedBy = adminId,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = Guid.Parse("A1B2C3D4-7777-4AAA-BBBB-000000000007"),
                Name = "Kenya AA Peaberry",
                Description = "Bright acidity and intense fruitiness. Notes of blackcurrant, wine, and citrus.",
                SKU = "KEN-PEA-01",
                Price = 2250.00m,
                Origin = "Kenya",
                Category = ProductCategory.SingleOrigin,
                RoastLevel = RoastLevel.Light,
                WeightInGrams = 1000,
                ImageUrl = "/assets/images/products/kenya-aa.png",
                IsFeatured = true,
                MinimumOrderQuantity = 5,
                CreatedBy = adminId,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = Guid.Parse("A1B2C3D4-4444-4AAA-BBBB-000000000004"),
                Name = "Sumatra Mandheling",
                Description = "Full-bodied Indonesian coffee with earthy, herbal complexity. Low acidity with a lingering dark chocolate and cedar finish.",
                SKU = "SUM-MAN-01",
                Price = 1920.00m,
                DiscountedPrice = 1880.00m,
                Origin = "Indonesia",
                Category = ProductCategory.SingleOrigin,
                RoastLevel = RoastLevel.Dark,
                WeightInGrams = 1000,
                ImageUrl = "/assets/images/products/sumatra-mandheling.png",
                IsFeatured = false,
                MinimumOrderQuantity = 5,
                CreatedBy = adminId,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = Guid.Parse("A1B2C3D4-6666-4AAA-BBBB-000000000006"),
                Name = "Swiss Water Decaf Blend",
                Description = "Chemically-free Swiss Water Process decaf with all the flavor, none of the caffeine. Smooth, mellow, and perfect for evening cups.",
                SKU = "DEC-SWS-01",
                Price = 1650.00m,
                Origin = "Brazil/Peru",
                Category = ProductCategory.Decaf,
                RoastLevel = RoastLevel.Medium,
                WeightInGrams = 1000,
                ImageUrl = "/assets/images/products/swiss-decaf.png",
                IsFeatured = false,
                MinimumOrderQuantity = 10,
                CreatedBy = adminId,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = Guid.Parse("A1B2C3D4-8888-4AAA-BBBB-000000000008"),
                Name = "Cold Brew Concentrate",
                Description = "Pre-ground coarse blend optimized for 12-24hr cold extraction. Yields a smooth, low-acid concentrate with natural sweetness and chocolate notes.",
                SKU = "CLD-BRW-01",
                Price = 1550.00m,
                DiscountedPrice = 1499.00m,
                Origin = "Brazil",
                Category = ProductCategory.ColdBrew,
                RoastLevel = RoastLevel.MediumDark,
                WeightInGrams = 1000,
                ImageUrl = "/assets/images/products/cold-brew.png",
                IsFeatured = false,
                MinimumOrderQuantity = 10,
                CreatedBy = adminId,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = Guid.Parse("A1B2C3D4-9999-4AAA-BBBB-000000000009"),
                Name = "Costa Rica Tarrazu",
                Description = "Bright and clean Costa Rican beans from the famed Tarrazu highlands. Honey-processed with notes of apricot, brown sugar, and a silky mouthfeel.",
                SKU = "CRC-TAR-01",
                Price = 1890.00m,
                Origin = "Costa Rica",
                Category = ProductCategory.SingleOrigin,
                RoastLevel = RoastLevel.Medium,
                WeightInGrams = 1000,
                ImageUrl = "/assets/images/products/costa-rica-tarrazu.png",
                IsFeatured = false,
                MinimumOrderQuantity = 5,
                CreatedBy = adminId,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            },
            new Product
            {
                Id = Guid.Parse("A1B2C3D4-AAAA-4AAA-BBBB-000000000010"),
                Name = "Midnight Mocha Blend",
                Description = "A luxurious house blend of Ethiopian and Brazilian beans with rich cocoa, toasted almond, and a whisper of vanilla. Perfect for lattes.",
                SKU = "BLD-MOC-01",
                Price = 1690.00m,
                DiscountedPrice = 1650.00m,
                Origin = "Ethiopia/Brazil",
                Category = ProductCategory.EspressoBlend,
                RoastLevel = RoastLevel.MediumDark,
                WeightInGrams = 1000,
                ImageUrl = "/assets/images/products/midnight-mocha.png",
                IsFeatured = true,
                MinimumOrderQuantity = 5,
                CreatedBy = adminId,
                CreatedAt = new DateTime(2024, 1, 1),
                UpdatedAt = new DateTime(2024, 1, 1)
            }
        );
    }
}