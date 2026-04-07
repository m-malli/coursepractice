using ShoppingApp.Application.Common;
using ShoppingApp.Application.DTOs.Cart;

namespace ShoppingApp.Application.Interfaces;

public interface ICartService
{
    Task<ServiceResult<IEnumerable<CartItemDto>>> GetCartAsync(Guid userId);
    Task<ServiceResult<CartItemDto>> AddToCartAsync(Guid userId, AddToCartDto dto);
    Task<ServiceResult<CartItemDto>> UpdateQuantityAsync(Guid userId, Guid productId, UpdateCartItemDto dto);
    Task<ServiceResult<bool>> RemoveFromCartAsync(Guid userId, Guid productId);
    Task<ServiceResult<bool>> ClearCartAsync(Guid userId);
}
