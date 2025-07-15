using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class ProductPhotoRepository : IProductPhotoRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<ProductPhoto> _createRepository;
        private readonly IGenericDeleteRepository<ProductPhoto> _deleteRepository;
        private readonly IGenericGetRepository<ProductPhoto> _getRepository;
        private readonly IGenericListRepository<ProductPhoto> _listRepository;

        public ProductPhotoRepository(
            TesteDbContext context, 
            IGenericCreateRepository<ProductPhoto> createRepository, 
            IGenericDeleteRepository<ProductPhoto> deleteRepository, 
            IGenericGetRepository<ProductPhoto> getRepository, 
            IGenericListRepository<ProductPhoto> listRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
        }

        public async Task<Result<ProductPhoto>> Create(ProductPhoto entity, bool skipSave = false)
        {
            return await _createRepository.Create(_context, entity, skipSave);
        }

        public async Task<Result<bool>> Delete(int id, bool skipSave = false)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<ProductPhoto>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }
        public async Task<Result<List<ProductPhoto>>> GetByImageNames(List<string> imageNames)
        {
            var list = await _context.ProductPhotos
                .Where(pp => imageNames.Contains(pp.Photo.FileName) && !pp.Deleted)
                .ToListAsync();

            return Result<List<ProductPhoto>>.Success(list);
        }

        public async Task<Result<List<ProductPhoto>>> ListByProductId(int productId)
        {
            var list = await _context.ProductPhotos
                .Where(pp => pp.ProductId == productId && !pp.Deleted)
                .ToListAsync();

            return Result<List<ProductPhoto>>.Success(list);
        }

        public async Task<Result<List<ProductPhoto>>> List()
        {
            return await _listRepository.List(_context);
        }
    }
}
