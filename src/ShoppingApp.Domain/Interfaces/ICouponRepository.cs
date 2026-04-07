using ShoppingApp.Domain.Entities;

namespace ShoppingApp.Domain.Interfaces;

public interface ICouponRepository
{
    Task<Coupon?> GetByCodeAsync(string code);
    Task<IEnumerable<Coupon>> GetAllAsync();
    Task<Coupon> AddAsync(Coupon coupon);
    Task UpdateAsync(Coupon coupon);
}
