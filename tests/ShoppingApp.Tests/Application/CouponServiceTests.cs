using Moq;
using ShoppingApp.Application.DTOs.Coupons;
using ShoppingApp.Application.Services;
using ShoppingApp.Domain.Entities;
using ShoppingApp.Domain.Interfaces;

namespace ShoppingApp.Tests.Application;

public class CouponServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();

    [Fact]
    public async Task ValidateAsync_NotFound_ReturnsFail()
    {
        _uow.Setup(u => u.Coupons.GetByCodeAsync(It.IsAny<string>())).ReturnsAsync((Coupon?)null);

        var svc = new CouponService(_uow.Object);
        var result = await svc.ValidateAsync("INVALID", 100);

        Assert.False(result.Success);
        Assert.Equal("Coupon not found.", result.Error);
    }

    [Fact]
    public async Task ValidateAsync_ExpiredCoupon_ReturnsFail()
    {
        var coupon = new Coupon
        {
            Code = "EXPIRED", DiscountPercent = 10,
            UsageLimit = 10, TimesUsed = 0,
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };
        _uow.Setup(u => u.Coupons.GetByCodeAsync("EXPIRED")).ReturnsAsync(coupon);

        var svc = new CouponService(_uow.Object);
        var result = await svc.ValidateAsync("EXPIRED", 100);

        Assert.False(result.Success);
        Assert.Contains("expired", result.Error, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ValidateAsync_ExhaustedCoupon_ReturnsFail()
    {
        var coupon = new Coupon
        {
            Code = "USED", DiscountPercent = 10,
            UsageLimit = 5, TimesUsed = 5,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        _uow.Setup(u => u.Coupons.GetByCodeAsync("USED")).ReturnsAsync(coupon);

        var svc = new CouponService(_uow.Object);
        var result = await svc.ValidateAsync("USED", 100);

        Assert.False(result.Success);
    }

    [Fact]
    public async Task ValidateAsync_ValidCoupon_ReturnsOk()
    {
        var coupon = new Coupon
        {
            Code = "SAVE20", DiscountPercent = 20,
            UsageLimit = 100, TimesUsed = 5,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        };
        _uow.Setup(u => u.Coupons.GetByCodeAsync("SAVE20")).ReturnsAsync(coupon);

        var svc = new CouponService(_uow.Object);
        var result = await svc.ValidateAsync("SAVE20", 100);

        Assert.True(result.Success);
        Assert.Equal("SAVE20", result.Data!.Code);
    }

    [Fact]
    public async Task CreateAsync_ValidDto_ReturnsOkWithUpperCaseCode()
    {
        _uow.Setup(u => u.Coupons.AddAsync(It.IsAny<Coupon>())).ReturnsAsync((Coupon c) => c);
        _uow.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        var svc = new CouponService(_uow.Object);
        var result = await svc.CreateAsync(new CreateCouponDto(
            "summer", 15, 50, 100, 10, DateTime.UtcNow.AddDays(30)));

        Assert.True(result.Success);
        Assert.Equal("SUMMER", result.Data!.Code); // uppercased
    }
}
