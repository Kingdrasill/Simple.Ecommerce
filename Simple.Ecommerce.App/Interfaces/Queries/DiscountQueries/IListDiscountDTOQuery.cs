using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.Interfaces.Queries.DiscountQueries
{
    public interface IListDiscountDTOQuery
    {
        Task<Result<List<DiscountDTO>>> Execute();
    }
}
