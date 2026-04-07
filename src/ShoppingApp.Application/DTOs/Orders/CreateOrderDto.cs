namespace ShoppingApp.Application.DTOs.Orders;

public record CreateOrderDto(string ShippingAddress, string? CouponCode);
