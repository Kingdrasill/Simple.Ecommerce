using MongoDB.Bson.Serialization;
using Simple.Ecommerce.Domain.OrderProcessing.Events;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBOGOEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemSimpleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemTieredEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderProcessEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderRevertEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ShippingEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.StockEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.TaxEvent;

namespace Simple.Ecommerce.Infra.Services.MongoDB
{
    public static class MongoDBClassMaps
    {
        public static void RegisterClassMaps()
        {
            BsonClassMap.RegisterClassMap<BaseOrderProcessingEvent>(cm =>
            {
                cm.AutoMap();
                cm.SetIsRootClass(true);
                // Confirmação
                cm.AddKnownType(typeof(OrderProcessingStartedEvent));
                cm.AddKnownType(typeof(OrderStatusChangedEvent));
                cm.AddKnownType(typeof(StockReservedEvent));
                cm.AddKnownType(typeof(ShippingFeeAppliedEvent));
                cm.AddKnownType(typeof(SimpleItemDiscountAppliedEvent));
                cm.AddKnownType(typeof(TieredItemDiscountAppliedEvent));
                cm.AddKnownType(typeof(BOGODiscountAppliedEvent));
                cm.AddKnownType(typeof(BundleDiscountAppliedEvent));
                cm.AddKnownType(typeof(OrderDiscountAppliedEvent));
                cm.AddKnownType(typeof(TaxAppliedEvent));
                cm.AddKnownType(typeof(OrderProcessedEvent));
                // Reversão
                cm.AddKnownType(typeof(OrderRevertingStartedEvent));
                cm.AddKnownType(typeof(TaxRevertedEvent));
                cm.AddKnownType(typeof(OrderDiscountRevertedEvent));
                cm.AddKnownType(typeof(BundleDiscountRevertEvent));
                cm.AddKnownType(typeof(BOGODiscountRevertEvent));
                cm.AddKnownType(typeof(TieredItemDiscountRevertEvent));
                cm.AddKnownType(typeof(SimpleItemDiscountRevertEvent));
                cm.AddKnownType(typeof(ShippingFeeRevertedEvent));
                cm.AddKnownType(typeof(StockReleasedEvent));
                cm.AddKnownType(typeof(OrderRevertedEvent));
            });

            // Confirmação
            BsonClassMap.RegisterClassMap<OrderProcessingStartedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<OrderStatusChangedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<StockReservedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<ShippingFeeAppliedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<SimpleItemDiscountAppliedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<TieredItemDiscountAppliedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<BOGODiscountAppliedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<BundleDiscountAppliedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<OrderDiscountAppliedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<TaxAppliedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<OrderProcessedEvent>(cm => cm.AutoMap());
            // Reversão
            BsonClassMap.RegisterClassMap<OrderRevertingStartedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<TaxRevertedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<OrderDiscountRevertedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<BundleDiscountRevertEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<BOGODiscountRevertEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<TieredItemDiscountRevertEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<SimpleItemDiscountRevertEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<ShippingFeeRevertedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<StockReleasedEvent>(cm => cm.AutoMap());
            BsonClassMap.RegisterClassMap<OrderRevertedEvent>(cm => cm.AutoMap());
        }
    }
}
