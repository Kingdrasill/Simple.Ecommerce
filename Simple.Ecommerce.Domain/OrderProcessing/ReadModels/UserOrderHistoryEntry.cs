using MongoDB.Bson.Serialization.Attributes;

namespace Simple.Ecommerce.Domain.OrderProcessing.ReadModels
{
    public class UserOrderHistoryEntry
    {
        public int OrderId { get; set; }
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
