using MongoDB.Driver;
using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;
using Simple.Ecommerce.Infra.ReadModelRepositories.BaseReadModelRepository;

namespace Simple.Ecommerce.Infra.ReadModelRepositories
{
    public class OrderDetailMongoDBReadModelRepository : MongoDBReadModelRepository<OrderDetailReadModel, int>, IOrderDetailReadModelRepository
    {
        public OrderDetailMongoDBReadModelRepository(IMongoDatabase database)
            : base(database, "orders_details") { }

        public async Task<OrderDetailReadModel?> GetOrderDetails(int orderId)
        {
            return await _collection
                .Find(c => c.OrderId == orderId)
                .FirstOrDefaultAsync();
        }
    }
}
