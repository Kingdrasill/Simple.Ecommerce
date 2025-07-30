using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<Product> _createRepository;
        private readonly IGenericDeleteRepository<Product> _deleteRepository;
        private readonly IGenericDetachRepository<Product> _detachRepository;
        private readonly IGenericGetRepository<Product> _getRepository;
        private readonly IGenericListRepository<Product> _listRepository;
        private readonly IGenericUpdateRepository<Product> _updateRepository;

        public ProductRepository(
            TesteDbContext context, 
            IGenericCreateRepository<Product> createRepository, 
            IGenericDeleteRepository<Product> deleteRepository, 
            IGenericDetachRepository<Product> detachRepository,
            IGenericGetRepository<Product> getRepository, 
            IGenericListRepository<Product> listRepository, 
            IGenericUpdateRepository<Product> updateRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _detachRepository = detachRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
            _updateRepository = updateRepository;
        }

        public async Task<Result<Product>> Create(Product entity, bool skipSave = false)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id, bool skipSave = false)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public void Detach(Product entity)
        {
            _detachRepository.Detach(_context, entity);
        }

        public async Task<Result<Product>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<Product>>> GetProductsByIds(List<int> ids)
        {
            return Result<List<Product>>.Success(await _context.Products
                .Where(p => ids.Contains(p.Id) && !p.Deleted)
                .ToListAsync());
        }

        public async Task<Result<List<Product>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<Product>> Update(Product entity, bool skipSave = false)
        {
            return await _updateRepository.Update(_context, entity);
        }
    }
}
