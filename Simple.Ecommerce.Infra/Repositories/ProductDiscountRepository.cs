using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.Entities.ProductDiscountEntity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Infra.Interfaces.Generic;
using Microsoft.EntityFrameworkCore;

namespace Simple.Ecommerce.Infra.Repositories
{
    internal class ProductDiscountRepository : IProductDiscountRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<ProductDiscount> _createRepository;
        private readonly IGenericDeleteRepository<ProductDiscount> _deleteRepository;
        private readonly IGenericGetRepository<ProductDiscount> _getRepository;
        private readonly IGenericListRepository<ProductDiscount> _listRepository;

        public ProductDiscountRepository(
            TesteDbContext context, 
            IGenericCreateRepository<ProductDiscount> createRepository, 
            IGenericDeleteRepository<ProductDiscount> deleteRepository, 
            IGenericGetRepository<ProductDiscount> getRepository, 
            IGenericListRepository<ProductDiscount> listRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
        }

        public async Task<Result<ProductDiscount>> Create(ProductDiscount entity)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<ProductDiscount>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<ProductDiscount>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<List<ProductDiscount>>> GetByDiscountId(int discountId)
        {
            var discountProducts = await _context.ProductDiscounts
                                                    .Where(pd => pd.DiscountId == discountId && !pd.Deleted)
                                                    .ToListAsync();

            return Result<List<ProductDiscount>>.Success(discountProducts);
        }

        public async Task<Result<List<ProductDiscount>>> GetByProductId(int productId)
        {
            var productDiscounts = await _context.ProductDiscounts
                                                    .Where(pd => pd.ProductId == productId && !pd.Deleted)
                                                    .ToListAsync();

            return Result<List<ProductDiscount>>.Success(productDiscounts);
        }
    }
}
