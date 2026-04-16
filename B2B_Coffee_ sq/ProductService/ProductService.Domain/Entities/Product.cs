using ProductService.Domain.Enums;

namespace ProductService.Domain.Entities;

public class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;          // Stock Keeping Unit
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public string Origin { get; set; } = string.Empty;       // e.g. "Ethiopia", "Colombia"
    public ProductCategory Category { get; set; }
    public RoastLevel RoastLevel { get; set; }
    public double WeightInGrams { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public int MinimumOrderQuantity { get; set; } = 1;       // B2B min qty
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }                      // Admin who created

    // Navigation
    public ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
}