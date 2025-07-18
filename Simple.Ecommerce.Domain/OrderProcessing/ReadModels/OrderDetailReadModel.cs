using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Simple.Ecommerce.Domain.Enums.OrderLock;
using Simple.Ecommerce.Domain.Enums.OrderType;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject;

namespace Simple.Ecommerce.Domain.OrderProcessing.ReadModels
{
    public class OrderDetailReadModel
    {
        [BsonId]
        public int OrderId { get; set; }
        public int UserId { get; set; }
        [BsonElement("orderType")]
        [BsonRepresentation(BsonType.Int32)]
        public OrderType OrderType { get; set; }
        [BsonElement("address")]
        public Address Address { get; set; }
        [BsonElement("paymentInformation")]
        public PaymentInformation? PaymentInformation { get; set; }
        [BsonElement("orderLock")]
        [BsonRepresentation(BsonType.Int32)]
        public OrderLock OrderLock { get; set; }
        [BsonElement("orderDate")]
        public DateTime OrderDate { get; set; }
        [BsonElement("status")]
        public string Status { get; set; }
        [BsonElement("originalTotal")]
        public decimal OriginalTotal { get; set; }
        [BsonElement("currentTotal")]
        public decimal CurrentTotal { get; set; }
        [BsonElement("amountDiscounted")]
        public decimal AmountDiscounted { get; set; }
        [BsonElement("shippingFee")]
        public decimal ShippingFee { get; set; }
        [BsonElement("taxAmount")]
        public decimal TaxAmount { get; set; }
        [BsonElement("items")]
        public List<OrderDetailItem> Items { get; set; }
        [BsonElement("appliedDiscount")]
        public (int DiscountId, string DiscountName)? AppliedDiscount { get; set; }
        [BsonElement("appliedDiscounts")]
        public List<string> AppliedDiscounts { get; set; }

        public OrderDetailReadModel()
        {
            Items = new List<OrderDetailItem>();
            AppliedDiscounts = new List<string>();
        }
    }
}
