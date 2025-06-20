using Simple.Ecommerce.App.Interfaces.ReadData.BaseReadRepository;
using Simple.Ecommerce.Domain.ReadModels;

namespace Simple.Ecommerce.App.Interfaces.ReadData
{
    public interface IStockReadRepository :
        IBaseSaveReadRepository<StockReadModel>
    {
        Task<StockReadModel> GetByProductId(string productId);
    }
}
