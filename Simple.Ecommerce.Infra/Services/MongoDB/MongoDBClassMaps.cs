using MongoDB.Bson.Serialization;
using Simple.Ecommerce.Domain.OrderProcessing.Events;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBOGOEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemSimpleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemTieredEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ShippingEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.StockEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.TaxEvent;

namespace Simple.Ecommerce.Infra.Services.MongoDB
{
    public static class MongoDBClassMaps
    {
        public static void RegisterClassMaps()
        {
            BsonClassMap.RegisterClassMap<OrderProcessingEvent>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                cm.AddKnownType(typeof(OrderProcessingStartedEvent));
                cm.AddKnownType(typeof(OrderStatusChangedEvent));
                cm.AddKnownType(typeof(StockReservedEvent));
                cm.AddKnownType(typeof(ShippingFeeAppliedEvent));
                cm.AddKnownType(typeof(SimpleItemDiscountAppliedEvent));
                cm.AddKnownType(typeof(TieredItemDiscountAppliedEvent));
                cm.AddKnownType(typeof(BOGOItemDiscountAppliedEvent));
                cm.AddKnownType(typeof(BundleDiscountAppliedEvent));
                cm.AddKnownType(typeof(OrderDiscountAppliedEvent));
                cm.AddKnownType(typeof(TaxAppliedEvent));
                cm.AddKnownType(typeof(OrderProcessedEvent));
            });

            BsonClassMap.RegisterClassMap<OrderProcessingStartedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<OrderStatusChangedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<StockReservedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<ShippingFeeAppliedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<SimpleItemDiscountAppliedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<TieredItemDiscountAppliedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<BOGOItemDiscountAppliedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<BundleDiscountAppliedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<OrderDiscountAppliedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<TaxAppliedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<OrderProcessedEvent>(cm => cm.AutoMap());
        }
    }
}
