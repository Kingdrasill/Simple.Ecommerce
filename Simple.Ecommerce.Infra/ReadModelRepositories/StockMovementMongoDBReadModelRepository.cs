using MongoDB.Driver;
using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;
using Simple.Ecommerce.Infra.ReadModelRepositories.BaseReadModelRepository;

namespace Simple.Ecommerce.Infra.ReadModelRepositories
{
    public class StockMovementMongoDBReadModelRepository : MongoDBReadModelRepository<StockMovementReadModel, int>, IStockMovementReadModelRepository
    {
        public StockMovementMongoDBReadModelRepository(IMongoDatabase database)
            : base(database, "stock_movements") { }
    }
}
