using Microsoft.EntityFrameworkCore;
using ShoppingApp.Domain.Entities;
using ShoppingApp.Domain.Interfaces;
using ShoppingApp.Infrastructure.Data;

namespace ShoppingApp.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _db;
    public OrderRepository(AppDbContext db) => _db = db;

    public async Task<Order?> GetByIdAsync(Guid id) =>
        await _db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == id);

    public async Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId) =>
        await _db.Orders.Include(o => o.Items)
            .Where(o => o.UserId == userId).OrderByDescending(o => o.CreatedAt).ToListAsync();

    public async Task<Order> AddAsync(Order order)
    {
        await _db.Orders.AddAsync(order);
        return order;
    }

    public Task UpdateAsync(Order order)
    {
        _db.Orders.Update(order);
        return Task.CompletedTask;
    }
}
