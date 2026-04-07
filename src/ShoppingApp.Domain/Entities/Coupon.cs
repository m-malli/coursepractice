namespace ShoppingApp.Domain.Entities;

public class Coupon : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public decimal DiscountPercent { get; set; }
    public decimal? MaxDiscountAmount { get; set; }
    public decimal MinOrderAmount { get; set; }
    public int UsageLimit { get; set; }
    public int TimesUsed { get; set; }
    public DateTime ExpiresAt { get; set; }

    public bool IsValid() => TimesUsed < UsageLimit && ExpiresAt > DateTime.UtcNow;

    public decimal CalculateDiscount(decimal orderTotal)
    {
        if (!IsValid() || orderTotal < MinOrderAmount) return 0;
        var discount = orderTotal * (DiscountPercent / 100m);
        return MaxDiscountAmount.HasValue ? Math.Min(discount, MaxDiscountAmount.Value) : discount;
    }
}
