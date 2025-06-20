using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.ReadModels;
using MongoDB.Driver;

namespace Simple.Ecommerce.Infra.ReadRepositories
{
    public class UserOrderHistoryReadRepository : IUserOrderHistoryReadRepository
    {
        private readonly IMongoCollection<UserOrderHistoryReadModel> _collection;

        public UserOrderHistoryReadRepository(
            IMongoDatabase database    
        )
        {
            _collection = database.GetCollection<UserOrderHistoryReadModel>("UserOrderHistoryReadModels");
        }

        public async Task<UserOrderHistoryReadModel> GetByUserId(string userId)
        {
            var filter = Builders<UserOrderHistoryReadModel>.Filter.Eq(u => u.UserId, userId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task Save(UserOrderHistoryReadModel model)
        {
            var filter = Builders<UserOrderHistoryReadModel>.Filter.Eq(u => u.UserName, model.UserId);
            await _collection.ReplaceOneAsync(filter, model, new ReplaceOptions { IsUpsert = true });
        }
    }
}
