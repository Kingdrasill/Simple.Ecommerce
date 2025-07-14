using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.ItemsTieredHandler
{
    public class TieredItemsDiscountHandler : BaseOrderProcessingHandler
    {
        private readonly IDiscountTierRepository _tierRepository;

        public TieredItemsDiscountHandler(
            IDiscountTierRepository tierRepository
        ) : base()
        {
            _tierRepository = tierRepository;
        }

        public override async Task Handle(OrderInProcess orderInProcess, bool skipDiscounts = false)
        {
            if (!skipDiscounts)
            {
                var tieredDiscounts = orderInProcess.UnAppliedDiscounts
                    .Where(uad => uad.DiscountType == DiscountType.Tiered && uad.DiscountScope == DiscountScope.Product)
                    .ToList();
                int index = tieredDiscounts.Count - 1;

                while (index >= 0)
                {
                    var discount = tieredDiscounts[index];
                    var item = orderInProcess.Items.First(i => i.Id == discount.OwnerId);
                    decimal amountDiscountedPrice = 0;

                    var getItemTiers = await _tierRepository.GetByDiscountId(discount.Id);
                    if (getItemTiers.IsFailure)
                    {
                        throw new ResultException(new Error("TieredItemDiscountsHandler.NotFound", $"Os tiers para o desconto {discount.Name} do item {item.ProductName} não foram encontrados!"));
                    }

                    var tierToApply = getItemTiers.GetValue()
                        .Where(it => it.MinQuantity <= item.Quantity)
                        .OrderByDescending(it  => it.MinQuantity)
                        .LastOrDefault();

                    if (tierToApply is not null)
                    {

                        if (discount.DiscountValueType == DiscountValueType.Percentage)
                        {
                            amountDiscountedPrice = item.CurrentPrice * tierToApply!.Value;
                        }
                        else if (discount.DiscountValueType == DiscountValueType.FixedAmount)
                        {
                            amountDiscountedPrice = discount.Value!.Value;
                        }
                        var publishEvent = orderInProcess.ApplyTieredItemDiscount(item.Id, discount.Id, discount.Name, discount.DiscountType, tierToApply.Name, amountDiscountedPrice);
                        Console.WriteLine($"\t[TieredItemDiscountsHandler] O desconto {discount.Name} foi aplicado ao item {item.ProductName} do pedido. Novo preço do item: {publishEvent.NewItemPrice:C}. Total descontado do pedido: {publishEvent.AmountDiscountedTotal:C}. Novo total do pedido: {orderInProcess.CurrentTotalPrice:C}.");
                    }
                    else
                    {
                        Console.WriteLine($"\t[TieredItemDiscountsHandler] O desconto {discount.Name} não foi aplicado ao item {item.ProductName} por não ter a quantidade miníma do tier mais baixo.");
                    }

                    orderInProcess.UnAppliedDiscounts.Remove(discount);
                    tieredDiscounts.RemoveAt(index);
                    index = tieredDiscounts.Count - 1;
                }
            }

            await base.Handle(orderInProcess, skipDiscounts);
        }
    }
}
