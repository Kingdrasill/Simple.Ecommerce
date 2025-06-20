using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.ReadModels;
using MongoDB.Driver;

namespace Simple.Ecommerce.Infra.ReadRepositories
{
    public class StockReadRepository : IStockReadRepository
    {
        private readonly IMongoCollection<StockReadModel> _collection;

        public StockReadRepository(
            IMongoDatabase database
        )
        {
            _collection = database.GetCollection<StockReadModel>("StockReadModels");
        }

        public async Task<StockReadModel> GetByProductId(string productId)
        {
            var filter = Builders<StockReadModel>.Filter.Eq(s => s.ProductId, productId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task Save(StockReadModel model)
        {
            var filter = Builders<StockReadModel>.Filter.Eq(s => s.ProductId, model.ProductId);
            await _collection.ReplaceOneAsync(filter, model, new ReplaceOptions { IsUpsert = true });
        }
    }
}
