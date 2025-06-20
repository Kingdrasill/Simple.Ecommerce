using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Domain.Entities.OrderDiscountEnity;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Infra.Interfaces.Generic;
using Microsoft.EntityFrameworkCore;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class OrderDiscountRepository : IOrderDiscountRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<OrderDiscount> _createRepository;
        private readonly IGenericDeleteRepository<OrderDiscount> _deleteRepository;
        private readonly IGenericGetRepository<OrderDiscount> _getRepository;
        private readonly IGenericListRepository<OrderDiscount> _listRepository;

        public OrderDiscountRepository(
            TesteDbContext context, 
            IGenericCreateRepository<OrderDiscount> createRepository, 
            IGenericDeleteRepository<OrderDiscount> deleteRepository, 
            IGenericGetRepository<OrderDiscount> getRepository, 
            IGenericListRepository<OrderDiscount> listRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
        }

        public async Task<Result<OrderDiscount>> Create(OrderDiscount entity)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<OrderDiscount>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<OrderDiscount>>> GetByDiscountId(int discountId)
        {
            var discountOrders = await _context.OrderDiscounts
                                                    .Where(od => od.DiscountId == discountId && !od.Deleted)
                                                    .ToListAsync();

            return Result<List<OrderDiscount>>.Success(discountOrders);
        }

        public async Task<Result<List<OrderDiscount>>> GetByOrderId(int ordereId)
        {
            var orderDiscounts = await _context.OrderDiscounts
                                                    .Where(od => od.OrderId == ordereId && !od.Deleted)
                                                    .ToListAsync();

            return Result<List<OrderDiscount>>.Success(orderDiscounts);
        }

        public async Task<Result<List<OrderDiscount>>> List()
        {
            return await _listRepository.List(_context);
        }
    }
}
