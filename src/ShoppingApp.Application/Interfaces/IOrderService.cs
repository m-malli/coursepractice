using ShoppingApp.Application.Common;
using ShoppingApp.Application.DTOs.Orders;

namespace ShoppingApp.Application.Interfaces;

public interface IOrderService
{
    Task<ServiceResult<OrderDto>> CreateFromCartAsync(Guid userId, CreateOrderDto dto);
    Task<ServiceResult<OrderDto>> GetByIdAsync(Guid id, Guid userId);
    Task<ServiceResult<IEnumerable<OrderDto>>> GetUserOrdersAsync(Guid userId);
    Task<ServiceResult<OrderDto>> UpdateStatusAsync(Guid orderId, string status);
    Task<ServiceResult<OrderDto>> CancelOrderAsync(Guid orderId, Guid userId);
}
