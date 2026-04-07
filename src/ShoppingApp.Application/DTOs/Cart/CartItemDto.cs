namespace ShoppingApp.Application.DTOs.Cart;

public record CartItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);
