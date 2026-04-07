using Microsoft.EntityFrameworkCore;
using ShoppingApp.Domain.Entities;
using ShoppingApp.Domain.Interfaces;
using ShoppingApp.Infrastructure.Data;

namespace ShoppingApp.Infrastructure.Repositories;

public class CartRepository : ICartRepository
{
    private readonly AppDbContext _db;
    public CartRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<CartItem>> GetByUserIdAsync(Guid userId) =>
        await _db.CartItems.Include(c => c.Product).Where(c => c.UserId == userId).ToListAsync();

    public async Task<CartItem?> GetItemAsync(Guid userId, Guid productId) =>
        await _db.CartItems.Include(c => c.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

    public async Task<CartItem> AddAsync(CartItem item)
    {
        await _db.CartItems.AddAsync(item);
        return item;
    }

    public Task UpdateAsync(CartItem item)
    {
        _db.CartItems.Update(item);
        return Task.CompletedTask;
    }

    public async Task RemoveAsync(Guid userId, Guid productId)
    {
        var item = await _db.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);
        if (item is not null) _db.CartItems.Remove(item);
    }

    public async Task ClearAsync(Guid userId)
    {
        var items = await _db.CartItems.Where(c => c.UserId == userId).ToListAsync();
        _db.CartItems.RemoveRange(items);
    }
}
