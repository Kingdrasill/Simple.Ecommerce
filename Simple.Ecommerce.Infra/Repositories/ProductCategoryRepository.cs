using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.ProductCategoryEntity;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    internal class ProductCategoryRepository : IProductCategoryRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<ProductCategory> _createRepository;
        private readonly IGenericDeleteRepository<ProductCategory> _deleteRepository;
        private readonly IGenericGetRepository<ProductCategory> _getRepository;
        private readonly IGenericListRepository<ProductCategory> _listRepository;

        public ProductCategoryRepository(
            TesteDbContext context, 
            IGenericCreateRepository<ProductCategory> createRepository, 
            IGenericDeleteRepository<ProductCategory> deleteRepository,
            IGenericGetRepository<ProductCategory> getRepository, 
            IGenericListRepository<ProductCategory> listRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
        }

        public async Task<Result<ProductCategory>> Create(ProductCategory entity, bool skipSave = false)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id, bool skipSave = false)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<ProductCategory>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<ProductCategory>>> GetByCategoryId(int categoryId)
        {
            var list = await _context.ProductCategories
                                        .Where(pc => pc.CategoryId == categoryId && !pc.Deleted)
                                        .ToListAsync();

            return Result<List<ProductCategory>>.Success(list);
        }

        public async Task<Result<List<ProductCategory>>> GetByProductId(int productId)
        {
            var list = await _context.ProductCategories
                                        .Where(pc => pc.ProductId == productId && !pc.Deleted)
                                        .ToListAsync();

            return Result<List<ProductCategory>>.Success(list);
        }

        public async Task<Result<List<ProductCategory>>> List()
        {
            return await _listRepository.List(_context);
        }
    }
}
