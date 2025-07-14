using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Simple.Ecommerce.Domain.OrderProcessing.ReadModels
{
    public class StockMovementReadModel
    {
        [BsonId]
        public Guid MovementId { get; set; }
        public int ProductId { get; set; }
        [BsonElement("quantityChanged")]
        public int QuantityChanged { get; set; }
        [BsonElement("movementType")]
        public string MovementType { get; set; }
        public int OrderId { get; set; }
        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }
    }
}
