using Microsoft.EntityFrameworkCore;
using ShoppingApp.Domain.Entities;
using ShoppingApp.Domain.Interfaces;
using ShoppingApp.Infrastructure.Data;

namespace ShoppingApp.Infrastructure.Repositories;

public class CouponRepository : ICouponRepository
{
    private readonly AppDbContext _db;
    public CouponRepository(AppDbContext db) => _db = db;

    public async Task<Coupon?> GetByCodeAsync(string code) =>
        await _db.Coupons.FirstOrDefaultAsync(c => c.Code == code.ToUpperInvariant());

    public async Task<IEnumerable<Coupon>> GetAllAsync() =>
        await _db.Coupons.ToListAsync();

    public async Task<Coupon> AddAsync(Coupon coupon)
    {
        await _db.Coupons.AddAsync(coupon);
        return coupon;
    }

    public Task UpdateAsync(Coupon coupon)
    {
        _db.Coupons.Update(coupon);
        return Task.CompletedTask;
    }
}
