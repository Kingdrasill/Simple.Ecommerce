using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Contracts.CouponContracts;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Contracts.OrderItemContracts.Discounts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CouponEntity;
using Simple.Ecommerce.Domain.Entities.OrderItemEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<OrderItem> _createRepository;
        private readonly IGenericDeleteRepository<OrderItem> _deleteRepository;
        private readonly IGenericDetachRepository<OrderItem> _detachRepository;
        private readonly IGenericDetachRepository<Coupon> _detachCouponRepository;
        private readonly IGenericGetRepository<OrderItem> _getRepository;
        private readonly IGenericListRepository<OrderItem> _listRepository;
        private readonly IGenericUpdateRepository<OrderItem> _updateRepository;

        public OrderItemRepository(
            TesteDbContext context, 
            IGenericCreateRepository<OrderItem> createRepository,
            IGenericDeleteRepository<OrderItem> deleteRepository,
            IGenericDetachRepository<OrderItem> detachRepository,
            IGenericDetachRepository<Coupon> detachCouponRepository,
            IGenericGetRepository<OrderItem> getRepository,
            IGenericListRepository<OrderItem> listRepository,
            IGenericUpdateRepository<OrderItem> updateRepository
        )
        {
            _context = context;
            _createRepository = createRepository;
            _deleteRepository = deleteRepository;
            _detachRepository = detachRepository;
            _detachCouponRepository = detachCouponRepository;
            _getRepository = getRepository;
            _listRepository = listRepository;
            _updateRepository = updateRepository;
        }

        public async Task<Result<OrderItem>> Create(OrderItem entity, bool skipSave = false)
        {
            return await _createRepository.Create(_context, entity, skipSave);
        }

        public async Task<Result<bool>> Delete(int id, bool skipSave = false)
        {
            return await _deleteRepository.Delete(_context, id, skipSave);
        }

        public void Detach(OrderItem entity)
        {
            _detachRepository.Detach(_context, entity);
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

        public async Task<Result<List<OrderItem>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<List<OrderItemInfoDTO>>> ListByOrderIdOrderItemInfoDTO(int orderId)
        {
            var result = await (
                from oi in _context.OrderItems
                join d in _context.Discounts on oi.DiscountId equals d.Id into discountJoin
                from dj in discountJoin.DefaultIfEmpty()
                join c in _context.Coupons on oi.CouponId equals c.Id into couponJoin
                from cj in couponJoin.DefaultIfEmpty()
                where oi.OrderId == orderId && !oi.Deleted
                select new
                {
                    oi.Id,
                    oi.ProductId,
                    Discount = dj,
                    Coupon = cj
                }).ToListAsync();

            if (!result.Any())
            {
                return Result<List<OrderItemInfoDTO>>.Failure(new List<Error> { new("OrderItem.NotFound", "Nenhum item de pedido foi encontrado para o pedido!") });
            }

            List<Error> errors = new List<Error>();
            foreach (var item in result)
            {
                if (item.Coupon is not null && item.Discount is not null)
                {
                    if (item.Coupon.DiscountId != item.Discount.Id)
                    {
                        errors.Add(new("OrderItem.Conflict.DiscountId", $"O cupom aplicado ao item do pedido {item.ProductId} não pertence ao desconto aplicado a ele!"));
                    }
                    _detachCouponRepository.Detach(_context, item.Coupon);
                }
            }
            if (errors.Any())
            {
                return Result<List<OrderItemInfoDTO>>.Failure(errors);
            }

            return Result<List<OrderItemInfoDTO>>.Success(result.Select(item => new OrderItemInfoDTO(
                item.Id,
                item.ProductId,
                item.Discount == null
                    ? null
                    : new DiscountInfoDTO(
                        item.Discount.Id,
                        item.Discount.Name,
                        item.Discount.DiscountType
                    ),
                item.Coupon == null
                    ? null
                    : new CouponInfoDTO(
                        item.Coupon.Id,
                        item.Coupon.Code
                    ))).ToList());
        }

        public async Task<Result<List<OrderItemWithDiscountDTO>>> ListByOrderIdOrderItemWithDiscountDTO(int orderId)
        {
            var result = await (
                from oi in _context.OrderItems
                join p in _context.Products on oi.ProductId equals p.Id
                join d in _context.Discounts on oi.DiscountId equals d.Id into discountJoin
                from dj in discountJoin.DefaultIfEmpty()
                join c in _context.Coupons on oi.CouponId equals c.Id into couponJoin
                from cj in couponJoin.DefaultIfEmpty()
                where oi.OrderId == orderId && !oi.Deleted
                select new
                {
                    oi.Id,
                    ProductId = p.Id,
                    ProductName = p.Name,
                    oi.Quantity,
                    oi.Price,
                    Discount = dj,
                    Coupon = cj
                }).ToListAsync();

            if (!result.Any())
            {
                return Result<List<OrderItemWithDiscountDTO>>.Failure(new List<Error>{ new("OrderItem.NotFound", "Nenhum item de pedido foi encontrado para o pedido!") });
            }

            List<Error> errors = new List<Error>();
            foreach (var item in result)
            {
                if (item.Coupon is not null && item.Discount is not null)
                {
                    if (item.Coupon.DiscountId != item.Discount.Id)
                    {
                        errors.Add(new("OrderItem.Conflict.DiscountId", $"O cupom aplicado ao item do pedido {item.ProductId} não pertence ao desconto aplicado a ele!"));
                    }
                    _detachCouponRepository.Detach(_context, item.Coupon);
                }
            }
            if (errors.Any())
            {
                return Result<List<OrderItemWithDiscountDTO>>.Failure(errors);
            }

            return Result<List<OrderItemWithDiscountDTO>>.Success(result.Select(item => new OrderItemWithDiscountDTO(
                item.Id,
                item.ProductId,
                item.ProductName,
                item.Quantity,
                item.Price,
                item.Discount == null
                    ? null
                    : new DiscountDTO(
                        item.Discount.Id,
                        item.Discount.Name,
                        item.Discount.DiscountType,
                        item.Discount.DiscountScope,
                        item.Discount.DiscountValueType,
                        item.Discount.Value,
                        item.Discount.ValidFrom,
                        item.Discount.ValidTo,
                        item.Discount.IsActive,
                        item.Coupon == null
                            ? null
                            : new CouponDTO(
                                item.Coupon.Id,
                                item.Coupon.DiscountId,
                                item.Coupon.Code,
                                item.Coupon.ExpirationAt,
                                item.Coupon.IsUsed
                            )))).ToList());
        }

        public async Task<Result<OrderItem>> Update(OrderItem entity, bool skipSave = false)
        {
            return await _updateRepository.Update(_context, entity, skipSave);
        }
    }
}
