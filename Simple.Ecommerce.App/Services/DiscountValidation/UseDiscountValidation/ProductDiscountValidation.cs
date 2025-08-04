using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Contracts.OrderItemContracts.Discounts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;

namespace Simple.Ecommerce.App.Services.DiscountValidation.UseDiscountValidation
{
    public static class ProductDiscountValidation
    {
        public static async Task<Result<bool>> Validate(IDiscountBundleItemRepository dbiRepo, Coupon? coupon, Discount discount, Product product, List<OrderItemInfoDTO> orderItemsDiscountInfo, string errorTypeComplement, int? productId)
        {
            var simpleValidation = SimpleDiscountValidation.Validate(discount, DiscountScope.Product, errorTypeComplement, productId);
            if (simpleValidation.IsFailure)
            {
                return Result<bool>.Failure(simpleValidation.Errors!);
            }

            List<Error> errors = new();
            var errorMessageComplement = productId is not null ? $" do item {productId.Value} " : " ";

            if (discount.DiscountType == DiscountType.Bundle)
            {
                var bundleProductIds = new HashSet<int>(
                    (await dbiRepo.GetProductIdsByDiscountId(discount.Id)).GetValue()
                );
                foreach (var item in orderItemsDiscountInfo.Where(i => bundleProductIds.Contains(i.ProductId)))
                {
                    if (item.Discount is null) continue;

                    if (item.Discount.DiscountType != DiscountType.Bundle)
                        errors.Add(new($"{errorTypeComplement}.Conflict.DiscountType", $"O produto {item.ProductId} que faz parte do desconto de pacote{errorMessageComplement}foi inserido com outro tipo de desconto!"));
                    else if (item.Discount.Id != discount.Id)
                        errors.Add(new($"{errorTypeComplement}.Conflict.DiscountType", $"O produto {item.ProductId} que faz parte do desconto de pacote{errorMessageComplement}foi inserido para outro desconto de pacote!"));
                
                    if (coupon is not null && item.Coupon is not null)
                    {
                        if (coupon.Code != item.Coupon.Code)
                            errors.Add(new($"{errorTypeComplement}.Conflict.Coupon", $"O produto {item.ProductId} usa o cupom com código {item.Coupon.Code} mas o cupom{errorMessageComplement} utiliza o código {coupon.Code}"));
                    }
                }
            }
            else
            {
                var bundlesOfProduct = await dbiRepo.GetByProductId(product.Id);
                foreach (var bundle in bundlesOfProduct.GetValue().DistinctBy(b => b.DiscountId))
                {
                    var bundleProductIds = new HashSet<int>(
                        (await dbiRepo.GetProductIdsByDiscountId(bundle.DiscountId)).GetValue()
                    );
                    foreach (var item in orderItemsDiscountInfo.Where(i => bundleProductIds.Contains(i.ProductId)))
                    {
                        if (item.Discount is null) continue;

                        if (item.Discount.DiscountType == DiscountType.Bundle)
                            errors.Add(new($"{errorTypeComplement}.Conflict.DiscountType", $"O desconto {discount.Name}{errorMessageComplement}está em conflito com o desconto de pacote {item.Discount.Id} do item {item.ProductId}, por ser parte do pacote!"));
                    }
                }
            }
            if (errors.Count != 0)
                return Result<bool>.Failure(errors);
            return Result<bool>.Success(true);
        }
    }
}
