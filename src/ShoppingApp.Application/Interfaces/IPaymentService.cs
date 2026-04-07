using ShoppingApp.Application.Common;

namespace ShoppingApp.Application.Interfaces;

public interface IPaymentService
{
    Task<ServiceResult<string>> ProcessPaymentAsync(decimal amount, string currency = "usd");
}
