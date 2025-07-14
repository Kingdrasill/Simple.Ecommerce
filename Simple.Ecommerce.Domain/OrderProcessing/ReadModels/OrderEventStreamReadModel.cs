using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Simple.Ecommerce.Domain.OrderProcessing.ReadModels
{
    public class OrderEventStreamReadModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        [BsonElement("eventId")]
        public Guid EventId { get; set; }
        [BsonElement("orderId")]
        public int OrderId { get; set; }
        [BsonElement("eventType")]
        public string EventType { get; set; }
        [BsonElement("timestamp")]
        public DateTime Timestamp { get; set; }
        [BsonElement("eventData")]
        public BsonDocument EventData { get; set; }
        [BsonElement("version")]
        public int Version { get; set; }

        public OrderEventStreamReadModel() { }

        public OrderEventStreamReadModel(Guid eventId, int orderId, string eventType, DateTime timestamp, BsonDocument eventData, int version)
        {
            EventId = eventId;
            OrderId = orderId;
            EventType = eventType;
            Timestamp = timestamp;
            EventData = eventData;
            Version = version;
        }
    }
}
