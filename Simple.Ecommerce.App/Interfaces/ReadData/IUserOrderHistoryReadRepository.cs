using Simple.Ecommerce.App.Interfaces.ReadData.BaseReadRepository;
using Simple.Ecommerce.Domain.ReadModels;

namespace Simple.Ecommerce.App.Interfaces.ReadData
{
    public interface IUserOrderHistoryReadRepository :
        IBaseSaveReadRepository<UserOrderHistoryReadModel>
    {
        Task<UserOrderHistoryReadModel> GetByUserId(string userId);
    }
}
