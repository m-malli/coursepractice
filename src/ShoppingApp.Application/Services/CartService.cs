using ShoppingApp.Application.Common;
using ShoppingApp.Application.DTOs.Cart;
using ShoppingApp.Application.Interfaces;
using ShoppingApp.Application.Mappings;
using ShoppingApp.Domain.Entities;
using ShoppingApp.Domain.Interfaces;

namespace ShoppingApp.Application.Services;

public class CartService : ICartService
{
    private readonly IUnitOfWork _uow;
    public CartService(IUnitOfWork uow) => _uow = uow;

    public async Task<ServiceResult<IEnumerable<CartItemDto>>> GetCartAsync(Guid userId)
    {
        var items = await _uow.Cart.GetByUserIdAsync(userId);
        return ServiceResult<IEnumerable<CartItemDto>>.Ok(items.Select(i => i.ToDto()));
    }

    public async Task<ServiceResult<CartItemDto>> AddToCartAsync(Guid userId, AddToCartDto dto)
    {
        var product = await _uow.Products.GetByIdAsync(dto.ProductId);
        if (product is null) return ServiceResult<CartItemDto>.Fail("Product not found.");
        if (!product.HasSufficientStock(dto.Quantity))
            return ServiceResult<CartItemDto>.Fail("Insufficient stock.");

        var existing = await _uow.Cart.GetItemAsync(userId, dto.ProductId);
        if (existing is not null)
        {
            existing.Quantity += dto.Quantity;
            await _uow.Cart.UpdateAsync(existing);
        }
        else
        {
            existing = new CartItem { UserId = userId, ProductId = dto.ProductId, Quantity = dto.Quantity };
            await _uow.Cart.AddAsync(existing);
        }
        await _uow.SaveChangesAsync();
        existing.Product = product;
        return ServiceResult<CartItemDto>.Ok(existing.ToDto());
    }

    public async Task<ServiceResult<CartItemDto>> UpdateQuantityAsync(Guid userId, Guid productId, UpdateCartItemDto dto)
    {
        var item = await _uow.Cart.GetItemAsync(userId, productId);
        if (item is null) return ServiceResult<CartItemDto>.Fail("Item not in cart.");
        item.Quantity = dto.Quantity;
        await _uow.Cart.UpdateAsync(item);
        await _uow.SaveChangesAsync();
        return ServiceResult<CartItemDto>.Ok(item.ToDto());
    }

    public async Task<ServiceResult<bool>> RemoveFromCartAsync(Guid userId, Guid productId)
    {
        await _uow.Cart.RemoveAsync(userId, productId);
        await _uow.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<bool>> ClearCartAsync(Guid userId)
    {
        await _uow.Cart.ClearAsync(userId);
        await _uow.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }
}
