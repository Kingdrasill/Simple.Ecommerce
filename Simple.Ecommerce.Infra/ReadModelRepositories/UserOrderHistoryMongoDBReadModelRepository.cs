using MongoDB.Driver;
using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;
using Simple.Ecommerce.Infra.ReadModelRepositories.BaseReadModelRepository;

namespace Simple.Ecommerce.Infra.ReadModelRepositories
{
    public class UserOrderHistoryMongoDBReadModelRepository : MongoDBReadModelRepository<UserOrderHistoryReadModel, int>, IUserOrderHistoryReadModelRepository
    {
        public UserOrderHistoryMongoDBReadModelRepository(IMongoDatabase database)
            : base(database, "user_order_history") { }

        public async Task<UserOrderHistoryReadModel> GetByUserId(int userId)
        {
            return await _collection.Find(c => c.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<UserOrderHistoryReadModel> GetByOrderId(int orderId)
        {
            var filter = Builders<UserOrderHistoryReadModel>.Filter.ElemMatch(uoh => uoh.Orders, o => o.OrderId == orderId);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
