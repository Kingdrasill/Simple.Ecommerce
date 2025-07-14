using Simple.Ecommerce.App.Interfaces.ReadData.BaseReadModelRepository;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;

namespace Simple.Ecommerce.App.Interfaces.ReadData
{
    public interface IStockMovementReadModelRepository 
        : IReadModelRepository<StockMovementReadModel, int>
    {
    }
}
