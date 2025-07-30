using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;

namespace Simple.Ecommerce.App.Services.DiscountValidation.UseDiscountValidation
{
    public static class SimpleDiscountValidation
    {
        public static Result<bool> Validate(Discount discount, DiscountScope scope, string errorTypeComplement, int? productId)
        {
            List<Error> errors = new();
            var errorMessageComplement = productId is not null ? $" do item {productId.Value} " : " ";

            if (scope is DiscountScope.Order)
            {
                if (discount.DiscountScope != DiscountScope.Order)
                    errors.Add(new($"{errorTypeComplement}.InvalidType.DiscountScope", $"O desconto {discount.Name}não é aplicável a pedidos!"));
                if (discount.DiscountType is DiscountType.Tiered or DiscountType.BuyOneGetOne or DiscountType.Bundle)
                    errors.Add(new($"{errorTypeComplement}.InvalidType.DiscountType", $"O tipo do desconto {discount.Name}não é aplicável a pedidos!"));
            }
            if (scope is DiscountScope.Product)
            {
                if (discount.DiscountScope != DiscountScope.Product)
                    errors.Add(new($"{errorTypeComplement}.InvalidType.DiscountScope", $"O desconto {discount.Name}{errorMessageComplement}não é aplicável a produtos!"));
                if (discount.DiscountType is DiscountType.FirstPurchase or DiscountType.FreeShipping)
                    errors.Add(new($"{errorTypeComplement}.InvalidType.DiscountType", $"O tipo do desconto {discount.Name}{errorMessageComplement}não é aplicável a produtos!"));
            }
            if (discount.ValidFrom > DateTime.UtcNow)
                errors.Add(new($"{errorTypeComplement}.InvalidDate.ValidFrom", $"O desconto {discount.Name}{errorMessageComplement}ainda não está válido!"));
            if (discount.ValidTo < DateTime.UtcNow)
                errors.Add(new($"{errorTypeComplement}.InvalidDate.ValidTo", $"O desconto {discount.Name}{errorMessageComplement}já expirou!"));
            if (!discount.IsActive)
                errors.Add(new($"{errorTypeComplement}.InvalidDate.IsActive", $"O desconto {discount.Name}{errorMessageComplement}não está ativo!"));

            if (errors.Count != 0) 
                return Result<bool>.Failure(errors);
            return Result<bool>.Success(true);
        }
    }
}
