using MongoDB.Bson;
using Simple.Ecommerce.App.Interfaces.ReadData.BaseReadModelRepository;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;

namespace Simple.Ecommerce.App.Interfaces.ReadData
{
    public interface IOrderEventStreamReadModelRepository 
        : IReadModelRepository<OrderEventStreamReadModel, ObjectId>
    {
        Task<int> GetLastVersionForOrder(int orderId);
        Task<List<OrderEventStreamReadModel>> GetEventsForOrder(int orderId);
        Task<(OrderEventStreamReadModel Start, OrderEventStreamReadModel End)?> GetLastProcessingCycle(int orderId);
        Task<List<OrderEventStreamReadModel>> GetEventsInWindow(int orderId, DateTime start, DateTime end);
    }
}
