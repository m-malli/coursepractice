using ShoppingApp.Domain.Entities;

namespace ShoppingApp.Tests.Domain;

public class OrderTests
{
    // --- Order.Cancel tests ---

    [Fact]
    public void Cancel_PendingOrder_SetsStatusToCancelled()
    {
        var order = new Order { Status = OrderStatus.Pending };
        order.Cancel();
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Theory]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Processing)]
    public void Cancel_CancellableStatuses_SetsStatusToCancelled(OrderStatus status)
    {
        var order = new Order { Status = status };
        order.Cancel();
        Assert.Equal(OrderStatus.Cancelled, order.Status);
    }

    [Fact]
    public void Cancel_ShippedOrder_Throws()
    {
        var order = new Order { Status = OrderStatus.Shipped };
        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void Cancel_DeliveredOrder_Throws()
    {
        var order = new Order { Status = OrderStatus.Delivered };
        Assert.Throws<InvalidOperationException>(() => order.Cancel());
    }

    [Fact]
    public void GenerateOrderNumber_HasExpectedFormat()
    {
        var orderNumber = Order.GenerateOrderNumber();
        Assert.StartsWith("ORD-", orderNumber);
        Assert.Matches(@"^ORD-\d{8}-[A-F0-9]{8}$", orderNumber);
    }

    // --- Product tests ---

    [Fact]
    public void Product_ReduceStock_DecreasesQuantity()
    {
        var product = new Product { StockQuantity = 10 };
        product.ReduceStock(3);
        Assert.Equal(7, product.StockQuantity);
    }

    [Fact]
    public void Product_ReduceStock_InsufficientStock_Throws()
    {
        var product = new Product { StockQuantity = 2 };
        Assert.Throws<InvalidOperationException>(() => product.ReduceStock(5));
    }

    [Fact]
    public void Product_ReduceStock_ExactStock_Succeeds()
    {
        var product = new Product { StockQuantity = 5 };
        product.ReduceStock(5);
        Assert.Equal(0, product.StockQuantity);
    }

    [Fact]
    public void Product_HasSufficientStock_BoundaryTrue()
    {
        var product = new Product { StockQuantity = 3 };
        Assert.True(product.HasSufficientStock(3));
    }

    [Fact]
    public void Product_HasSufficientStock_OneLess_ReturnsFalse()
    {
        var product = new Product { StockQuantity = 2 };
        Assert.False(product.HasSufficientStock(3));
    }

    [Fact]
    public void Product_EffectivePrice_ReturnsDiscountPriceWhenSet()
    {
        var product = new Product { Price = 100m, DiscountPrice = 75m };
        Assert.Equal(75m, product.EffectivePrice);
    }

    [Fact]
    public void Product_EffectivePrice_ReturnsPriceWhenNoDiscount()
    {
        var product = new Product { Price = 100m, DiscountPrice = null };
        Assert.Equal(100m, product.EffectivePrice);
    }

    // --- Coupon tests ---

    [Fact]
    public void Coupon_CalculateDiscount_CappedByMaxDiscount()
    {
        var coupon = new Coupon
        {
            DiscountPercent = 10, MaxDiscountAmount = 50,
            MinOrderAmount = 100, UsageLimit = 10, TimesUsed = 0,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        var discount = coupon.CalculateDiscount(500);
        Assert.Equal(50m, discount); // 10% of 500 = 50, capped at 50
    }

    [Fact]
    public void Coupon_CalculateDiscount_NoCap_ReturnsFullPercent()
    {
        var coupon = new Coupon
        {
            DiscountPercent = 10, MaxDiscountAmount = null,
            MinOrderAmount = 0, UsageLimit = 10, TimesUsed = 0,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        Assert.Equal(50m, coupon.CalculateDiscount(500)); // 10% of 500
    }

    [Fact]
    public void Coupon_CalculateDiscount_BelowMinOrder_ReturnsZero()
    {
        var coupon = new Coupon
        {
            DiscountPercent = 10, MinOrderAmount = 200,
            UsageLimit = 10, TimesUsed = 0,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        Assert.Equal(0m, coupon.CalculateDiscount(100));
    }

    [Fact]
    public void Coupon_CalculateDiscount_Expired_ReturnsZero()
    {
        var coupon = new Coupon
        {
            DiscountPercent = 10, MinOrderAmount = 0,
            UsageLimit = 10, TimesUsed = 0,
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };
        Assert.Equal(0, coupon.CalculateDiscount(100));
    }

    [Fact]
    public void Coupon_CalculateDiscount_UsageLimitExhausted_ReturnsZero()
    {
        var coupon = new Coupon
        {
            DiscountPercent = 20, MinOrderAmount = 0,
            UsageLimit = 5, TimesUsed = 5,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        Assert.Equal(0m, coupon.CalculateDiscount(100));
    }

    [Fact]
    public void Coupon_IsValid_ValidCoupon_ReturnsTrue()
    {
        var coupon = new Coupon
        {
            UsageLimit = 10, TimesUsed = 0,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        Assert.True(coupon.IsValid());
    }

    [Fact]
    public void Coupon_IsValid_Exhausted_ReturnsFalse()
    {
        var coupon = new Coupon
        {
            UsageLimit = 5, TimesUsed = 5,
            ExpiresAt = DateTime.UtcNow.AddDays(1)
        };
        Assert.False(coupon.IsValid());
    }

    [Fact]
    public void Coupon_IsValid_Expired_ReturnsFalse()
    {
        var coupon = new Coupon
        {
            UsageLimit = 10, TimesUsed = 0,
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };
        Assert.False(coupon.IsValid());
    }
}
