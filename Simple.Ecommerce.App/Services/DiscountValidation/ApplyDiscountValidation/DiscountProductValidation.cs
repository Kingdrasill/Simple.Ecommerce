using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;

namespace Simple.Ecommerce.App.Services.DiscountValidation.ApplyDiscountValidation
{
    public static class DiscountProductValidation
    {
        public static async Task<Result<bool>> Validate(Discount discount, Product product, string errotTypeComplement, IDiscountTierRepository dtRepo)
        {
            if (discount.DiscountScope != DiscountScope.Product)
            {
                return Result<bool>.Failure(new List<Error> { new($"{errotTypeComplement}.IncorrectType.DiscountScope", $"Não se pode adicionar o desconto {discount.Name} ao produto pode não ser desconto de produto!") });
            }

            switch (discount.DiscountType)
            {
                case DiscountType.FixedAmount:
                    var value = discount.Value!.Value;
                    if (product.Price < value)
                    {
                        return Result<bool>.Failure(new List<Error> { new($"{errotTypeComplement}.NegativeDiscount.Price", $"Não se pode adicionar o desconto {discount.Name} ao produto por resultar em preço negativo quando descontado!") });
                    }
                    break;
                case DiscountType.Tiered:
                    if (discount.DiscountValueType is DiscountValueType.FixedAmount)
                    {
                        var getDiscountTiers = await dtRepo.GetByDiscountId(discount.Id);
                        if (getDiscountTiers.IsFailure)
                        {
                            return Result<bool>.Failure(getDiscountTiers.Errors!);
                        }
                        List<Error> errors = new();
                        foreach (var dt in getDiscountTiers.GetValue())
                        {
                            if (product.Price < dt.Value)
                            {
                                errors.Add(new Error($"{errotTypeComplement}.NegativeDiscount.Price", $"Não se pode adicionar o desconto {discount.Name} de nível {dt.Name} ao produto por resultar em preço negativo quando descontado!"));
                            }
                        }
                        if (errors.Count != 0)
                        {
                            return Result<bool>.Failure(errors);
                        }
                    }
                    break;
                case DiscountType.Bundle:
                    if (discount.DiscountValueType is DiscountValueType.FixedAmount)
                    {
                        if (product.Price < discount.Value!.Value)
                        {
                            return Result<bool>.Failure(new List<Error> { new($"{errotTypeComplement}.NegativeDiscount.Price", $"Não se pode adicionar o desconto {discount.Name} ao produto por resultar em preço negativo quando descontado!") });
                        }
                    }
                    break;
            }

            return Result<bool>.Success(true);
        }
    }
}
