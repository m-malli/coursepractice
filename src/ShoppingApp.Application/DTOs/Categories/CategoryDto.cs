namespace ShoppingApp.Application.DTOs.Categories;

public record CategoryDto(Guid Id, string Name, string? Description, string Slug, int ProductCount);
