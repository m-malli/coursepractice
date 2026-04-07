using ShoppingApp.Application.Common;
using ShoppingApp.Application.DTOs.Orders;
using ShoppingApp.Application.Interfaces;
using ShoppingApp.Application.Mappings;
using ShoppingApp.Domain.Entities;
using ShoppingApp.Domain.Interfaces;

namespace ShoppingApp.Application.Services;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _uow;
    private readonly IPaymentService _payment;

    public OrderService(IUnitOfWork uow, IPaymentService payment)
    {
        _uow = uow;
        _payment = payment;
    }

    public async Task<ServiceResult<OrderDto>> CreateFromCartAsync(Guid userId, CreateOrderDto dto)
    {
        var cartItems = (await _uow.Cart.GetByUserIdAsync(userId)).ToList();
        if (cartItems.Count == 0) return ServiceResult<OrderDto>.Fail("Cart is empty.");

        decimal subTotal = 0;
        var orderItems = new List<OrderItem>();
        foreach (var ci in cartItems)
        {
            var product = await _uow.Products.GetByIdAsync(ci.ProductId);
            if (product is null) return ServiceResult<OrderDto>.Fail($"Product {ci.ProductId} not found.");
            if (!product.HasSufficientStock(ci.Quantity))
                return ServiceResult<OrderDto>.Fail($"Insufficient stock for {product.Name}.");

            product.ReduceStock(ci.Quantity);
            await _uow.Products.UpdateAsync(product);

            var lineTotal = product.EffectivePrice * ci.Quantity;
            subTotal += lineTotal;
            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.EffectivePrice,
                Quantity = ci.Quantity
            });
        }

        decimal discount = 0;
        if (!string.IsNullOrWhiteSpace(dto.CouponCode))
        {
            var coupon = await _uow.Coupons.GetByCodeAsync(dto.CouponCode);
            if (coupon is not null && coupon.IsValid())
            {
                discount = coupon.CalculateDiscount(subTotal);
                coupon.TimesUsed++;
                await _uow.Coupons.UpdateAsync(coupon);
            }
        }

        var paymentResult = await _payment.ProcessPaymentAsync(subTotal - discount);
        if (!paymentResult.Success)
            return ServiceResult<OrderDto>.Fail($"Payment failed: {paymentResult.Error}");

        var order = new Order
        {
            OrderNumber = Order.GenerateOrderNumber(),
            UserId = userId,
            SubTotal = subTotal,
            DiscountAmount = discount,
            TotalAmount = subTotal - discount,
            ShippingAddress = dto.ShippingAddress,
            CouponCode = dto.CouponCode,
            PaymentTransactionId = paymentResult.Data,
            Status = OrderStatus.Confirmed,
            Items = orderItems
        };

        await _uow.Orders.AddAsync(order);
        await _uow.Cart.ClearAsync(userId);
        await _uow.SaveChangesAsync();
        return ServiceResult<OrderDto>.Ok(order.ToDto());
    }

    public async Task<ServiceResult<OrderDto>> GetByIdAsync(Guid id, Guid userId)
    {
        var order = await _uow.Orders.GetByIdAsync(id);
        if (order is null || order.UserId != userId) return ServiceResult<OrderDto>.Fail("Order not found.");
        return ServiceResult<OrderDto>.Ok(order.ToDto());
    }

    public async Task<ServiceResult<IEnumerable<OrderDto>>> GetUserOrdersAsync(Guid userId)
    {
        var orders = await _uow.Orders.GetByUserIdAsync(userId);
        return ServiceResult<IEnumerable<OrderDto>>.Ok(orders.Select(o => o.ToDto()));
    }

    public async Task<ServiceResult<OrderDto>> UpdateStatusAsync(Guid orderId, string status)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order is null) return ServiceResult<OrderDto>.Fail("Order not found.");
        if (!Enum.TryParse<OrderStatus>(status, true, out var newStatus))
            return ServiceResult<OrderDto>.Fail("Invalid status.");
        order.Status = newStatus;
        order.UpdatedAt = DateTime.UtcNow;
        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();
        return ServiceResult<OrderDto>.Ok(order.ToDto());
    }

    public async Task<ServiceResult<OrderDto>> CancelOrderAsync(Guid orderId, Guid userId)
    {
        var order = await _uow.Orders.GetByIdAsync(orderId);
        if (order is null || order.UserId != userId) return ServiceResult<OrderDto>.Fail("Order not found.");
        try
        {
            order.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            return ServiceResult<OrderDto>.Fail(ex.Message);
        }
        await _uow.Orders.UpdateAsync(order);
        await _uow.SaveChangesAsync();
        return ServiceResult<OrderDto>.Ok(order.ToDto());
    }
}
