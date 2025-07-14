using MongoDB.Bson;
using MongoDB.Driver;
using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;
using Simple.Ecommerce.Infra.ReadModelRepositories.BaseReadModelRepository;

namespace Simple.Ecommerce.Infra.ReadModelRepositories
{
    public class OrderEventStreamMongoDBReadModelRepository : MongoDBReadModelRepository<OrderEventStreamReadModel, Guid>, IOrderEventStreamReadModelRepository
    {
        public OrderEventStreamMongoDBReadModelRepository(IMongoDatabase database)
            : base(database, "orders_events_stream") { }

        public async Task<int> GetLastVersionForOrder(int orderId)
        {
            var lastEvent = await _collection.Find(e => e.OrderId == orderId)
                .SortByDescending(e => e.Version)
                .Limit(1)
                .FirstOrDefaultAsync();
            return lastEvent?.Version ?? 0;
        }

        public async Task<OrderEventStreamReadModel> GetById(ObjectId id)
        {
            return await _collection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task Delete(ObjectId id)
        {
            await _collection.DeleteOneAsync(c => c.Id == id);
        }
    }
}
