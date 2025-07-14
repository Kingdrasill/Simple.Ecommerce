using MongoDB.Bson.Serialization.Attributes;

namespace Simple.Ecommerce.Domain.OrderProcessing.ReadModels
{
    public class OrderSummaryReadModel
    {
        [BsonId]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        [BsonElement("orderDate")]
        public DateTime OrderDate { get; set; }
        [BsonElement("status")]
        public string Status { get; set; }
        [BsonElement("totalAmount")]
        public decimal TotalAmount { get; set; }
        [BsonElement("itemsCount")]
        public int ItemsCount { get; set; }
    }
}
