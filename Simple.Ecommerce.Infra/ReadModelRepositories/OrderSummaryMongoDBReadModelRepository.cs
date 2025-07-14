using MongoDB.Driver;
using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;
using Simple.Ecommerce.Infra.ReadModelRepositories.BaseReadModelRepository;

namespace Simple.Ecommerce.Infra.ReadModelRepositories
{
    public class OrderSummaryMongoDBReadModelRepository : MongoDBReadModelRepository<OrderSummaryReadModel, int>, IOrderSummaryReadModelRepository
    {
        public OrderSummaryMongoDBReadModelRepository(IMongoDatabase database)
            : base(database, "orders_summary") { }
    }
}
