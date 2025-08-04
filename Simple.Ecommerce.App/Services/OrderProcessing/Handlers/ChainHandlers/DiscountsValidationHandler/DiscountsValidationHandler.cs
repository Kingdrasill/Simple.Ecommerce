using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.OrderProcessing.Models;

namespace Simple.Ecommerce.App.Services.OrderProcessing.Handlers.ChainHandlers.DiscountsValidationHandler
{
    public class DiscountsValidationHandler : BaseOrderProcessingHandler
    {
        private readonly IConfirmOrderUnitOfWork _confirmOrderUoW;

        public DiscountsValidationHandler(
            IConfirmOrderUnitOfWork confirmOrderUoW
        ) : base() 
        {
            _confirmOrderUoW = confirmOrderUoW;
        }

        public override async Task Handle(OrderInProcess orderInProcess, bool skipDiscounts = false)
        {
            if (!skipDiscounts)
            {
                if (orderInProcess.UnAppliedDiscounts.Count == 0)
                {
                    Console.WriteLine("\t[DiscountsValidationHandler] Nenhum desconto a ser a aplicado para o pedido. Pulando cadeia de descontos.");
                    skipDiscounts = true;
                }

                foreach (var unAppliedDiscount in orderInProcess.UnAppliedDiscounts)
                {
                    if (unAppliedDiscount.DiscountScope == DiscountScope.Order || unAppliedDiscount.DiscountScope == DiscountScope.Product)
                    {
                        if (!unAppliedDiscount.IsActive)
                        {
                            throw new ArgumentException($"O desconto {unAppliedDiscount.Name} não pode ser aplicado por estar inativo!", nameof(unAppliedDiscount.IsActive));
                        }
                        else if (unAppliedDiscount.ValidFrom is not null && unAppliedDiscount.ValidTo is not null && (unAppliedDiscount.ValidFrom > DateTime.UtcNow || unAppliedDiscount.ValidTo < DateTime.UtcNow))
                        {
                            throw new ArgumentException($"O desconto {unAppliedDiscount.Name} não pode ser aplicado por não estar no seu período válido!", nameof(unAppliedDiscount.IsActive));
                        }

                        if (unAppliedDiscount.DiscountValueType is not null && unAppliedDiscount.DiscountValueType != DiscountValueType.Percentage && unAppliedDiscount.DiscountValueType != DiscountValueType.FixedAmount)
                        {
                            throw new ArgumentException($"O desconto {unAppliedDiscount.Name} tem um tipo de valor que não é nem percentual ou fixo!");
                        }

                        if (unAppliedDiscount.DiscountScope == DiscountScope.Order)
                        {
                            if (unAppliedDiscount.DiscountType is DiscountType.Tiered or DiscountType.BuyOneGetOne or DiscountType.Bundle)
                            {
                                throw new ArgumentException($"O desconto {unAppliedDiscount.Name} não pode ser aplicado para pedidos!", nameof(unAppliedDiscount.DiscountType));
                            }
                            if (unAppliedDiscount.DiscountType == DiscountType.FirstPurchase)
                            {
                                var getFirstPurchase = await _confirmOrderUoW.Orders.GetFirstPurchase(orderInProcess.UserId);
                                if (getFirstPurchase.IsSuccess)
                                {
                                    throw new ArgumentException($"O desconto {unAppliedDiscount.Name} não pode ser aplicado por está não ser sua primeira compra!", nameof(unAppliedDiscount.DiscountType));
                                }
                            }
                        }
                        if (unAppliedDiscount.DiscountScope == DiscountScope.Product)
                        {
                            if (unAppliedDiscount.DiscountType is DiscountType.FirstPurchase or DiscountType.FreeShipping)
                            {
                                throw new ArgumentException($"O desconto {unAppliedDiscount.Name} não pode ser aplicado para items de pedidos!", nameof(unAppliedDiscount.DiscountType));
                            }

                            var item = orderInProcess.Items.FirstOrDefault(i => i.Id == unAppliedDiscount.OwnerId);
                            if (item is  null)
                            {
                                throw new InvalidOperationException($"O desconto {unAppliedDiscount.Name} foir aplicado para um item que não existe no pedido!");
                            }
                        }
                    }
                    else
                    {
                        throw new ArgumentException($"O desconto {unAppliedDiscount.Name} é para um escopo inválido!", nameof(unAppliedDiscount.DiscountScope));
                    }
                }
            }

            await base.Handle(orderInProcess, skipDiscounts);
        }
    }
}
