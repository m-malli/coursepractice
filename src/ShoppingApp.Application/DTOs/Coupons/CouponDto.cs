namespace ShoppingApp.Application.DTOs.Coupons;

public record CouponDto(Guid Id, string Code, decimal DiscountPercent, decimal? MaxDiscountAmount,
    decimal MinOrderAmount, int UsageLimit, int TimesUsed, DateTime ExpiresAt, bool IsValid);
