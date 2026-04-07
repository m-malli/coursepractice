using ShoppingApp.Application.Common;
using ShoppingApp.Application.DTOs.Coupons;
using ShoppingApp.Application.Interfaces;
using ShoppingApp.Application.Mappings;
using ShoppingApp.Domain.Entities;
using ShoppingApp.Domain.Interfaces;

namespace ShoppingApp.Application.Services;

public class CouponService : ICouponService
{
    private readonly IUnitOfWork _uow;
    public CouponService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<CouponDto>> ValidateAsync(string code, decimal orderTotal)
    {
        var coupon = await _uow.Coupons.GetByCodeAsync(code);
        if (coupon is null) return ServiceResult<CouponDto>.Fail("Coupon not found.");
        if (!coupon.IsValid()) return ServiceResult<CouponDto>.Fail("Coupon is expired or fully used.");
        return ServiceResult<CouponDto>.Ok(coupon.ToDto());
    }

    public async Task<ServiceResult<IEnumerable<CouponDto>>> GetAllAsync()
    {
        var coupons = await _uow.Coupons.GetAllAsync();
        return ServiceResult<IEnumerable<CouponDto>>.Ok(coupons.Select(c => c.ToDto()));
    }

    public async Task<ServiceResult<CouponDto>> CreateAsync(CreateCouponDto dto)
    {
        var coupon = new Coupon
        {
            Code = dto.Code.ToUpperInvariant(),
            DiscountPercent = dto.DiscountPercent,
            MaxDiscountAmount = dto.MaxDiscountAmount,
            MinOrderAmount = dto.MinOrderAmount,
            UsageLimit = dto.UsageLimit,
            ExpiresAt = dto.ExpiresAt
        };
        await _uow.Coupons.AddAsync(coupon);
        await _uow.SaveChangesAsync();
        return ServiceResult<CouponDto>.Ok(coupon.ToDto());
    }
}
