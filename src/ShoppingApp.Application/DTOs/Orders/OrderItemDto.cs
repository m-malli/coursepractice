namespace ShoppingApp.Application.DTOs.Orders;

public record OrderItemDto(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity, decimal LineTotal);
