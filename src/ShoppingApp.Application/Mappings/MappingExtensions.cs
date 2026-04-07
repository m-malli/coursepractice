using ShoppingApp.Application.DTOs.Cart;
using ShoppingApp.Application.DTOs.Categories;
using ShoppingApp.Application.DTOs.Coupons;
using ShoppingApp.Application.DTOs.Orders;
using ShoppingApp.Application.DTOs.Products;
using ShoppingApp.Domain.Entities;

namespace ShoppingApp.Application.Mappings;

public static class MappingExtensions
{
    public static ProductDto ToDto(this Product p) => new(
        p.Id, p.Name, p.Description, p.Price, p.DiscountPrice,
        p.SKU, p.StockQuantity, p.IsActive, p.CategoryId,
        p.Category?.Name ?? string.Empty,
        p.Images.Select(i => i.Url));

    public static CategoryDto ToDto(this Category c) => new(
        c.Id, c.Name, c.Description, c.Slug, c.Products.Count);

    public static CartItemDto ToDto(this CartItem ci) => new(
        ci.ProductId, ci.Product?.Name ?? string.Empty,
        ci.Product?.EffectivePrice ?? 0, ci.Quantity,
        (ci.Product?.EffectivePrice ?? 0) * ci.Quantity);

    public static OrderDto ToDto(this Order o) => new(
        o.Id, o.OrderNumber, o.Status.ToString(),
        o.SubTotal, o.DiscountAmount, o.TotalAmount,
        o.ShippingAddress, o.CouponCode, o.CreatedAt,
        o.Items.Select(i => i.ToDto()));

    public static OrderItemDto ToDto(this OrderItem oi) => new(
        oi.ProductId, oi.ProductName, oi.UnitPrice, oi.Quantity, oi.LineTotal);

    public static CouponDto ToDto(this Coupon c) => new(
        c.Id, c.Code, c.DiscountPercent, c.MaxDiscountAmount,
        c.MinOrderAmount, c.UsageLimit, c.TimesUsed, c.ExpiresAt, c.IsValid());
}
