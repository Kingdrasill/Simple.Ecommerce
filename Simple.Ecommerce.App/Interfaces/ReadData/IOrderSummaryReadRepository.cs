using Simple.Ecommerce.App.Interfaces.ReadData.BaseReadRepository;
using Simple.Ecommerce.Domain.ReadModels;

namespace Simple.Ecommerce.App.Interfaces.ReadData
{
    public interface IOrderSummaryReadRepository :
        IBaseSaveReadRepository<OrderSummaryReadModel>
    {
        Task<OrderSummaryReadModel> GetByOrderId(string orderId);
    }
}
