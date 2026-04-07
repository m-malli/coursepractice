using ShoppingApp.Domain.Entities;

namespace ShoppingApp.Domain.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id);
    Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId);
    Task<Order> AddAsync(Order order);
    Task UpdateAsync(Order order);
}
