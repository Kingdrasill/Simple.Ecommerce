using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<OrderItem> _createRepository;
        private readonly IGenericDeleteRepository<OrderItem> _deleteRepository;
        private readonly IGenericGetRepository<OrderItem> _getRepository;
        private readonly IGenericListRepository<OrderItem> _listRepository;
        private readonly IGenericUpdateRepository<OrderItem> _updateRepository;

        public OrderItemRepository(
            TesteDbContext context, 
            IGenericCreateRepository<OrderItem> createRepository,
            IGenericDeleteRepository<OrderItem> deleteRepository,
            IGenericGetRepository<OrderItem> getRepository,
            IGenericListRepository<OrderItem> listRepository,
            IGenericUpdateRepository<OrderItem> updateRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
            _updateRepository = updateRepository;
        }

        public async Task<Result<OrderItem>> Create(OrderItem entity)
        {
            return await _createRepository.Create(_context, entity);
        }

        public async Task<Result<bool>> Delete(int id)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<OrderItem>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<List<OrderItem>>> GetByOrderId(int orderId)
        {
            var listOrderItem = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId && !oi.Deleted)
                .ToListAsync();

            return Result<List<OrderItem>>.Success(listOrderItem);
        }

        public async Task<Result<OrderItem>> GetByOrderIdAndProductId(int orderId, int productId)
        {
            IQueryable<OrderItem> query = _context.Set<OrderItem>().AsNoTracking();

            var entity = await query.FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.ProductId == productId && !oi.Deleted);

            if (entity is null)
                return Result<OrderItem>.Failure(new List<Error> { new("OrderItem.NotFound", "O item do pedido não foi encontrado!") });

            return Result<OrderItem>.Success(entity);
        }

        public async Task<Result<List<OrderItemDiscountInfoDTO>>> GetOrdemItemsDiscountInfo(int orderId)
        {
            var result = await (
                from oi in _context.OrderItems
                join d in _context.Discounts on oi.DiscountId equals d.Id into discountJoin
                from discount in discountJoin.DefaultIfEmpty()
                where
                        oi.OrderId == orderId
                    && !oi.Deleted
                select new OrderItemDiscountInfoDTO(
                    oi.Id,
                    oi.ProductId,
                    oi.DiscountId,
                    discount != null ? discount.DiscountType : null
                )).ToListAsync();

            return Result<List<OrderItemDiscountInfoDTO>>.Success(result);
        }

        public async Task<Result<List<OrderItem>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<OrderItem>> Update(OrderItem entity)
        {
            return await _updateRepository.Update(_context, entity);
        }
    }
}
