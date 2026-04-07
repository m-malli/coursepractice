namespace ShoppingApp.Application.DTOs.Products;

public record UpdateProductDto(
    string? Name, string? Description, decimal? Price,
    decimal? DiscountPrice, string? SKU, int? StockQuantity,
    bool? IsActive, Guid? CategoryId);
