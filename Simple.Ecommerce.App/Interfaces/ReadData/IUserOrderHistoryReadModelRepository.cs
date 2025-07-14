using Simple.Ecommerce.App.Interfaces.ReadData.BaseReadModelRepository;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;

namespace Simple.Ecommerce.App.Interfaces.ReadData
{
    public interface IUserOrderHistoryReadModelRepository 
        : IReadModelRepository<UserOrderHistoryReadModel, int>
    {
        Task<UserOrderHistoryReadModel> GetByUserId(int userId);
        Task<UserOrderHistoryReadModel> GetByOrderId(int orderId);
    }
}
