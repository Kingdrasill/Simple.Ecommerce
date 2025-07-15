using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.UnitOfWork;

namespace Simple.Ecommerce.Infra.Services.UnitOfWork
{
    public class AddPhotoProductUnitOfWork : BaseUnitOfWork, IAddPhotoProductUnitOfWork
    {
        public IProductPhotoRepository ProductPhotos { get; }
        public IProductRepository Products { get; }

        public AddPhotoProductUnitOfWork(
            TesteDbContext context, 
            IProductPhotoRepository productPhotos, 
            IProductRepository products
        ) : base(context) 
        { 
            ProductPhotos = productPhotos;
            Products = products;
        }
    }
}
