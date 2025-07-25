﻿using Simple.Ecommerce.App.Interfaces.ReadData;
using Simple.Ecommerce.Domain.Interfaces.OrderProcessingEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBOGOEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemBundleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemSimpleEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ItemTieredEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderDiscountEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.OrderEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.ShippingEvent;
using Simple.Ecommerce.Domain.OrderProcessing.Events.TaxEvent;
using Simple.Ecommerce.Domain.OrderProcessing.ReadModels;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Projectors
{
    public class OrderDetailProjector :
        // Confirmação
        IOrderProcessingEventHandler<OrderProcessingStartedEvent>,
        IOrderProcessingEventHandler<OrderStatusChangedEvent>,
        IOrderProcessingEventHandler<ShippingFeeAppliedEvent>,
        IOrderProcessingEventHandler<SimpleItemDiscountAppliedEvent>,
        IOrderProcessingEventHandler<TieredItemDiscountAppliedEvent>,
        IOrderProcessingEventHandler<BOGOItemDiscountAppliedEvent>,
        IOrderProcessingEventHandler<BundleDiscountAppliedEvent>,
        IOrderProcessingEventHandler<OrderDiscountAppliedEvent>,
        IOrderProcessingEventHandler<TaxAppliedEvent>,
        IOrderProcessingEventHandler<OrderProcessedEvent>,
        // Reversão
        IOrderProcessingEventHandler<OrderRevertingStartedEvent>,
        IOrderProcessingEventHandler<TaxRevertedEvent>,
        IOrderProcessingEventHandler<OrderDiscountRevertedEvent>,
        IOrderProcessingEventHandler<BundleDiscountRevertEvent>,
        IOrderProcessingEventHandler<BOGOItemDiscountRevertEvent>,
        IOrderProcessingEventHandler<TieredItemDiscountRevertEvent>,
        IOrderProcessingEventHandler<SimpleItemDiscountRevertEvent>,
        IOrderProcessingEventHandler<ShippingFeeRevertedEvent>,
        IOrderProcessingEventHandler<OrderRevertedEvent>
    {
        private readonly IOrderDetailReadModelRepository _orderDetailReadModelRepository;

        public OrderDetailProjector(
            IOrderDetailReadModelRepository orderDetailReadModelRepository
        )
        {
            _orderDetailReadModelRepository = orderDetailReadModelRepository;
        }

        // Confirmação
        public async Task Handle(OrderProcessingStartedEvent @event)
        {
            var readModel = new OrderDetailReadModel
            {
                OrderId = @event.AggregateId,
                UserId = @event.UserId,
                OrderType = @event.OrderType,
                Address = @event.Address,
                PaymentInformation = @event.PaymentInformation,
                OrderLock = @event.OrderLock,
                OrderDate = @event.OrderDate,
                Status = @event.Status,
                OriginalTotal = @event.InitialTotal,
                CurrentTotal = @event.InitialTotal,
                AmountDiscounted = 0,
                ShippingFee = 0,
                TaxAmount = 0,
                Items = @event.Items.Select(item => new OrderDetailItem
                {
                    OrderItemId = item.OrderItemId,
                    ProductId = item.ProductId,
                    ProductName = item.ProductName,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price,
                    CurrentPrice = item.Price,
                    AmountDiscountedPrice = 0,
                    AppliedDiscount = null,
                    IsTieredItem = false,
                    TierName = null,
                    IsFreeItem = false,
                    IsBundleItem = false
                }).ToList(),
                AppliedDiscount = null,
                AppliedDiscounts = new List<string>()
            };
            await _orderDetailReadModelRepository.Upsert(readModel);
        }

        public async Task Handle(OrderStatusChangedEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                readModel.Status = @event.NewStatus;
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(ShippingFeeAppliedEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                readModel.ShippingFee = @event.ShippingFee;
                readModel.CurrentTotal = @event.CurrentTotal;
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(SimpleItemDiscountAppliedEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                var itemToUpdate = readModel.Items.FirstOrDefault(i => i.OrderItemId == @event.OrderItemId);
                if (itemToUpdate != null)
                {
                    itemToUpdate.CurrentPrice = @event.NewItemPrice;
                    itemToUpdate.AmountDiscountedPrice = @event.AmountDiscountedPrice;
                    itemToUpdate.AppliedDiscount = (@event.DiscountId, @event.DiscountName);
                }
                readModel.AmountDiscounted += @event.AmountDiscountedTotal;
                readModel.CurrentTotal = @event.CurrentTotal;
                readModel.AppliedDiscounts.Add(@event.DiscountName);
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(TieredItemDiscountAppliedEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                var itemToUpdate = readModel.Items.FirstOrDefault(i => i.OrderItemId == @event.OrderItemId);
                if (itemToUpdate != null)
                {
                    itemToUpdate.CurrentPrice = @event.NewItemPrice;
                    itemToUpdate.AmountDiscountedPrice = @event.AmountDiscountedPrice;
                    itemToUpdate.AppliedDiscount = (@event.DiscountId, @event.DiscountName);
                    itemToUpdate.IsTieredItem = true;
                    itemToUpdate.TierName = @event.TierName;
                }
                readModel.AmountDiscounted += @event.AmountDiscountedTotal;
                readModel.CurrentTotal = @event.CurrentTotal;
                readModel.AppliedDiscounts.Add(@event.DiscountName);
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(BOGOItemDiscountAppliedEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                var itemToUpdate = readModel.Items.FirstOrDefault(i => i.OrderItemId == @event.OriginalOrderItemId);
                if (itemToUpdate != null)
                {
                    itemToUpdate.Quantity -= 1;
                }
                readModel.Items.Add(new OrderDetailItem
                {
                    OrderItemId = @event.OriginalOrderItemId,
                    ProductId = @event.ProductId,
                    ProductName = @event.ProductName,
                    Quantity = 1,
                    UnitPrice = 0,
                    CurrentPrice = 0,
                    AmountDiscountedPrice = @event.AmountDiscounted,
                    AppliedDiscount = (@event.DiscountId, @event.DiscountName),
                    IsTieredItem = false,
                    TierName = null,
                    IsFreeItem = true,
                    IsBundleItem = false
                });
                readModel.AmountDiscounted += @event.AmountDiscounted;
                readModel.CurrentTotal = @event.CurrentTotal;
                readModel.AppliedDiscounts.Add(@event.DiscountName);
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(BundleDiscountAppliedEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                foreach (var bundleItem in @event.BundleItems)
                {
                    var itemToUpdate = readModel.Items.FirstOrDefault(i => i.OrderItemId == bundleItem.OriginalOrderItemId);
                    if (itemToUpdate != null)
                    {
                        itemToUpdate.Quantity -= bundleItem.Quantity;
                    }
                    readModel.Items.Add(new OrderDetailItem
                    {
                        OrderItemId = bundleItem.OriginalOrderItemId,
                        ProductId = bundleItem.ProductId,
                        ProductName = bundleItem.ProductName,
                        Quantity = bundleItem.Quantity,
                        UnitPrice = 0,
                        CurrentPrice = bundleItem.NewItemPrice,
                        AmountDiscountedPrice = bundleItem.AmountDiscountedPrice,
                        AppliedDiscount = (@event.DiscountId, @event.DiscountName),
                        IsTieredItem = false,
                        TierName = null,
                        IsFreeItem = false,
                        IsBundleItem = true,
                        BundleId = @event.BundleId
                    });
                }
                readModel.AmountDiscounted += @event.AmountDiscountedTotal;
                readModel.CurrentTotal = @event.CurrentTotal;
                readModel.AppliedDiscounts.Add(@event.DiscountName);
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(OrderDiscountAppliedEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                readModel.AmountDiscounted += @event.AmountDiscounted;
                readModel.CurrentTotal = @event.CurrentTotal;
                readModel.AppliedDiscount = (@event.DiscountId, @event.DiscountName);
                readModel.AppliedDiscounts.Add(@event.DiscountName);
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(TaxAppliedEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                readModel.TaxAmount = @event.TaxAmount;
                readModel.CurrentTotal = @event.CurrentTotal;
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(OrderProcessedEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                readModel.CurrentTotal = @event.FinalTotal;
                readModel.Status = @event.Status;
                readModel.OrderLock = @event.OrderLock;
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        // Reversão
        public async Task Handle(OrderRevertingStartedEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                readModel.UserId = @event.UserId;
                readModel.OrderType = @event.OrderType;
                readModel.Address = @event.Address;
                readModel.PaymentInformation = @event.PaymentInformation;
                readModel.OrderLock = @event.OrderLock;
                readModel.OrderDate = @event.OrderDate;
                readModel.Status = @event.Status;
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(TaxRevertedEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                readModel.TaxAmount -= @event.TaxAmount;
                readModel.CurrentTotal = @event.CurrentTotal;
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(OrderDiscountRevertedEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                readModel.AmountDiscounted -= @event.AmountReverted;
                readModel.CurrentTotal = @event.CurrentTotal;
                readModel.AppliedDiscount = null;
                readModel.AppliedDiscounts.Remove(@event.DiscountName);
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(BundleDiscountRevertEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                foreach (var bundleItem in @event.BundleItems)
                {
                    var sameItems = readModel.Items.Where(i => i.OrderItemId == bundleItem.OriginalOrderItemId).ToList();
                    var oItem = sameItems.First(i => !i.IsBundleItem);
                    var bItem = sameItems.First(i => i.IsBundleItem);
                    oItem.Quantity += bItem.Quantity;
                    readModel.Items.Remove(bItem);
                }
                readModel.AmountDiscounted -= @event.AmountRevertedTotal;
                readModel.CurrentTotal = @event.CurrentTotal;
                readModel.AppliedDiscounts.Remove(@event.DiscountName);
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(BOGOItemDiscountRevertEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                var sameItems = readModel.Items.Where(i => i.OrderItemId == @event.OriginalOrderItemId).ToList();
                var oItem = sameItems.First(i => !i.IsFreeItem);
                var fItem = sameItems.First(i => i.IsFreeItem);
                oItem.Quantity += 1;
                readModel.Items.Remove(fItem);
                readModel.AmountDiscounted -= @event.AmountReverted;
                readModel.CurrentTotal = @event.CurrentTotal;
                readModel.AppliedDiscounts.Remove(@event.DiscountName);
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(TieredItemDiscountRevertEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                var itemToUpdate = readModel.Items.FirstOrDefault(i => i.OrderItemId == @event.OrderItemId);
                if (itemToUpdate != null)
                {
                    itemToUpdate.CurrentPrice = @event.ItemPrice;
                    itemToUpdate.AmountDiscountedPrice -= @event.AmountRevertedTotal;
                    itemToUpdate.AppliedDiscount = null;
                    itemToUpdate.IsTieredItem = false;
                    itemToUpdate.TierName = null;
                }
                readModel.AmountDiscounted -= @event.AmountRevertedTotal;
                readModel.CurrentTotal = @event.CurrentTotal;
                readModel.AppliedDiscounts.Remove(@event.DiscountName);
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(SimpleItemDiscountRevertEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                var itemToUpdate = readModel.Items.FirstOrDefault(i => i.OrderItemId == @event.OrderItemId);
                if (itemToUpdate != null)
                {
                    itemToUpdate.CurrentPrice = @event.ItemPrice;
                    itemToUpdate.AmountDiscountedPrice -= @event.AmountRevertedPrice;
                    itemToUpdate.AppliedDiscount = null;
                }
                readModel.AmountDiscounted -= @event.AmountRevertedTotal;
                readModel.CurrentTotal = @event.CurrentTotal;
                readModel.AppliedDiscounts.Remove(@event.DiscountName);
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(ShippingFeeRevertedEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                readModel.ShippingFee -= @event.ShippingFee;
                readModel.CurrentTotal = @event.CurrentTotal;
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }

        public async Task Handle(OrderRevertedEvent @event)
        {
            var readModel = await _orderDetailReadModelRepository.GetById(@event.AggregateId);
            if (readModel != null)
            {
                readModel.CurrentTotal = @event.OriginalTotal;
                readModel.Status = @event.Status;
                readModel.OrderLock = @event.OrderLock;
                await _orderDetailReadModelRepository.Upsert(readModel);
            }
        }
    }
}
