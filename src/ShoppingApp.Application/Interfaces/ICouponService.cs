using ShoppingApp.Application.Common;
using ShoppingApp.Application.DTOs.Coupons;

namespace ShoppingApp.Application.Interfaces;

public interface ICouponService
{
    Task<ServiceResult<CouponDto>> ValidateAsync(string code, decimal orderTotal);
    Task<ServiceResult<IEnumerable<CouponDto>>> GetAllAsync();
    Task<ServiceResult<CouponDto>> CreateAsync(CreateCouponDto dto);
}
