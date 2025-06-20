using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.ReadModels;
using MongoDB.Driver;

namespace Simple.Ecommerce.Infra.ReadRepositories
{
    public class OrderSummaryReadRepository : IOrderSummaryReadRepository
    {
        private readonly IMongoCollection<OrderSummaryReadModel> _collection;

        public OrderSummaryReadRepository(
            IMongoDatabase database
        )
        {
            _collection = database.GetCollection<OrderSummaryReadModel>("OrderSummaryReadModels");
        }

        public async Task<OrderSummaryReadModel> GetByOrderId(string orderId)
        {
            var filter = Builders<OrderSummaryReadModel>.Filter.Eq(o => o.OrderId, orderId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task Save(OrderSummaryReadModel model)
        {
            var filter = Builders<OrderSummaryReadModel>.Filter.Eq(o => o.OrderId, model.OrderId);
            await _collection.ReplaceOneAsync(filter, model, new ReplaceOptions { IsUpsert = true });
        }
    }
}
