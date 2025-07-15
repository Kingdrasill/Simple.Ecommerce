using Simple.Ecommerce.App.Interfaces.Data;

namespace Simple.Ecommerce.App.Interfaces.Services.UnitOfWork
{
    public interface IAddPhotoProductUnitOfWork : IBaseUnitOfWork
    {
        IProductPhotoRepository ProductPhotos { get; }
        IProductRepository Products { get; }
    }
}
