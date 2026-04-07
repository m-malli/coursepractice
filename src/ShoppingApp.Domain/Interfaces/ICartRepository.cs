using ShoppingApp.Domain.Entities;

namespace ShoppingApp.Domain.Interfaces;

public interface ICartRepository
{
    Task<IEnumerable<CartItem>> GetByUserIdAsync(Guid userId);
    Task<CartItem?> GetItemAsync(Guid userId, Guid productId);
    Task<CartItem> AddAsync(CartItem item);
    Task UpdateAsync(CartItem item);
    Task RemoveAsync(Guid userId, Guid productId);
    Task ClearAsync(Guid userId);
}
