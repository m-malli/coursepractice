namespace ShoppingApp.Application.DTOs.Orders;

public record OrderDto(
    Guid Id, string OrderNumber, string Status,
    decimal SubTotal, decimal DiscountAmount, decimal TotalAmount,
    string ShippingAddress, string? CouponCode,
    DateTime CreatedAt, IEnumerable<OrderItemDto> Items);
