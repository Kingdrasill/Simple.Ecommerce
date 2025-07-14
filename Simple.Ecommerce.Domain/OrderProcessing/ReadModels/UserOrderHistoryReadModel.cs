using MongoDB.Bson.Serialization.Attributes;

namespace Simple.Ecommerce.Domain.OrderProcessing.ReadModels
{
    public class UserOrderHistoryReadModel
    {
        [BsonId]
        public int UserId { get; set; }
        [BsonElement("userName")]
        public string UserName { get; set; }
        [BsonElement("orders")]
        public List<UserOrderHistoryEntry> Orders { get; set; }

        public UserOrderHistoryReadModel()
        {
            Orders = new List<UserOrderHistoryEntry>();
        }
    }
}
