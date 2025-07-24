using MongoDB.Bson;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Interfaces.Services.OrderProcessing
{
    public interface IOrderRevertingHandler
    {
        string EventType { get; }
        Task<Result<bool>> Revert(OrderInProcess order, BsonDocument eventData);
    }
}
