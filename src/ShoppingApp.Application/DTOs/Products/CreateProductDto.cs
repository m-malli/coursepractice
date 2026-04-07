namespace ShoppingApp.Application.DTOs.Products;

public record CreateProductDto(
    string Name, string? Description, decimal Price,
    decimal? DiscountPrice, string SKU, int StockQuantity,
    Guid CategoryId);
