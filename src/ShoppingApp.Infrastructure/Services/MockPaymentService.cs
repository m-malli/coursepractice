using ShoppingApp.Application.Common;
using ShoppingApp.Application.Interfaces;

namespace ShoppingApp.Infrastructure.Services;

public class MockPaymentService : IPaymentService
{
    public Task<ServiceResult<string>> ProcessPaymentAsync(decimal amount, string currency = "usd")
    {
        var transactionId = $"MOCK-{Guid.NewGuid():N}";
        return Task.FromResult(ServiceResult<string>.Ok(transactionId));
    }
}
