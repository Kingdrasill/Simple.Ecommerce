using MongoDB.Bson;
using MongoDB.Driver;
using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent;
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
            var lastEvent = await _collection
                .Find(c => c.OrderId == orderId)
                .SortByDescending(c => c.Version)
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

        public async Task<List<OrderEventStreamReadModel>> GetEventsForOrder(int orderId)
        {
            return await _collection
                .Find(c => c.OrderId == orderId)
                .SortBy(c => c.Timestamp)
                .ToListAsync();
        }

        public async Task<(OrderEventStreamReadModel Start, OrderEventStreamReadModel End)?> GetLastProcessingCycle(int orderId)
        {
            var events = await _collection
                .Find(c => c.OrderId == orderId
                    && (c.EventType == typeof(OrderProcessingStartedEvent).Name || c.EventType == typeof(OrderProcessedEvent).Name))
                .SortByDescending(c => c.Timestamp)
                .ToListAsync();

            var lastProcessed = events.FirstOrDefault(e => e.EventType == typeof(OrderProcessedEvent).Name);
            if (lastProcessed == null) return null;

            var lastStart = events
                .Where(e => e.EventType == typeof(OrderProcessingStartedEvent).Name && e.Timestamp < lastProcessed.Timestamp)
                .OrderByDescending(e => e.Timestamp)
                .FirstOrDefault();
            if (lastStart == null) return null;
            return (lastStart, lastProcessed);
        }

        public async Task<List<OrderEventStreamReadModel>> GetEventsInWindow(int orderId, DateTime start, DateTime end)
        {
            return await _collection
                .Find(c => c.OrderId == orderId &&
                           c.Timestamp >= start &&
                           c.Timestamp <= end)
                .SortBy(c => c.Timestamp)
                .ToListAsync();
        }
    }
}
