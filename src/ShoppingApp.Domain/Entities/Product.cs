namespace ShoppingApp.Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string SKU { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();

    public decimal EffectivePrice => DiscountPrice ?? Price;

    public bool HasSufficientStock(int quantity) => StockQuantity >= quantity;

    public void ReduceStock(int quantity)
    {
        if (!HasSufficientStock(quantity))
            throw new InvalidOperationException($"Insufficient stock for product {Name}.");
        StockQuantity -= quantity;
    }
}
