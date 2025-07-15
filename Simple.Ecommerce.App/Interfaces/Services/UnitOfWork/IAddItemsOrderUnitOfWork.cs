using Simple.Ecommerce.App.Interfaces.Data;

namespace Simple.Ecommerce.App.Interfaces.Services.UnitOfWork
{
    public interface IAddItemsOrderUnitOfWork : IBaseUnitOfWork
    {
        IOrderItemRepository OrderItems { get; }
        IOrderRepository Orders { get; }
        IProductRepository Products { get; }
        IDiscountRepository Discounts { get; }
        IDiscountBundleItemRepository DiscountBundleItems { get; }
    }
}
