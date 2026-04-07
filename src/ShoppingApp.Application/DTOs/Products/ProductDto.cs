namespace ShoppingApp.Application.DTOs.Products;

public record ProductDto(
    Guid Id, string Name, string? Description, decimal Price,
    decimal? DiscountPrice, string SKU, int StockQuantity,
    bool IsActive, Guid CategoryId, string CategoryName,
    IEnumerable<string> ImageUrls);
