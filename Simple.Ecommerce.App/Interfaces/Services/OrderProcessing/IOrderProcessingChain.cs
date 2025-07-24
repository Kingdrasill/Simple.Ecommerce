using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Interfaces.Services.OrderProcessing
{
    public interface IOrderProcessingChain
    {
        Task<Result<bool>> Process(OrderInProcess order, bool skipDiscounts = false);
    }
}
