using MongoDB.Bson;
using Simple.Ecommerce.App.Interfaces.ReadData.BaseReadModelRepository;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;

namespace Simple.Ecommerce.App.Interfaces.ReadData
{
    public interface IOrderEventStreamReadModelRepository 
        : IReadModelRepository<OrderEventStreamReadModel, ObjectId>
    {
        Task<int> GetLastVersionForOrder(int orderId);
    }
}
