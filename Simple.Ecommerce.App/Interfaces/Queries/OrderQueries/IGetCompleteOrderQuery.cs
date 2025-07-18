using Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO;
using Simple.Ecommerce.Domain;

namespace Simple.Ecommerce.App.Interfaces.Queries.OrderQueries
{
    public interface IGetCompleteOrderQuery
    {
        Task<Result<OrderCompleteDTO>> Execute(int id);
    }
}
