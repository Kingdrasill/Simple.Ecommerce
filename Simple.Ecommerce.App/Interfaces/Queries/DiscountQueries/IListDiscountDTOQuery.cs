using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries
{
    public interface IListDiscountDTOQuery
    {
        Task<Result<List<DiscountCompleteDTO>>> Execute();
    }
}
