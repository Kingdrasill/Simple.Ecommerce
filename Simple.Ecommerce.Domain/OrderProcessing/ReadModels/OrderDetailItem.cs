using MongoDB.Bson.Serialization.Attributes;

namespace Simple.Ecommerce.Domain.OrderProcessing.ReadModels
{
    public class OrderDetailItem
    {
        [BsonElement("orderItemId")]
        public int OrderItemId { get; set; }
        [BsonElement("productId")]
        public int ProductId { get; set; }
        [BsonElement("productName")]
        public string ProductName { get; set; }
        [BsonElement("quantity")]
        public int Quantity { get; set; }
        [BsonElement("unitPrice")]
        public decimal UnitPrice { get; set; }
        [BsonElement("currentPrice")]
        public decimal CurrentPrice { get; set; }
        [BsonElement("amountDiscountedPrice")]
        public decimal AmountDiscountedPrice { get; set; }
        [BsonElement("appliedDiscount")]
        public (int DiscountId, string DiscountName)? AppliedDiscount { get; set; }
        [BsonElement("isTieredItem")]
        public bool IsTieredItem { get; set; }
        [BsonElement("TierName")]
        public string? TierName { get; set; }
        [BsonElement("isFreeItem")]
        public bool IsFreeItem { get; set; }
        [BsonElement("isBundleItem")]
        public bool IsBundleItem { get; set; }
        [BsonElement("bundleId")]
        public Guid? BundleId { get; set; }
    }
}
