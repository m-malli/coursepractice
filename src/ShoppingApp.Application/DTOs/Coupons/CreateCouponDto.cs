namespace ShoppingApp.Application.DTOs.Coupons;

public record CreateCouponDto(string Code, decimal DiscountPercent, decimal? MaxDiscountAmount,
    decimal MinOrderAmount, int UsageLimit, DateTime ExpiresAt);
