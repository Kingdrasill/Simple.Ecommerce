using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.ChainOfResponsibility
{
    public interface IOrderProcessingChain
    {
        Task<Result<bool>> Process(OrderInProcess order, bool skipDiscounts = false);
    }
}
