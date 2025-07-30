using Microsoft.EntityFrameworkCore;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Contracts.DiscountTierContracts;
using Simple.Ecommerce.Contracts.OrderContracts.CompleteDTO;
using Simple.Ecommerce.Contracts.OrderContracts.Discounts;
using Simple.Ecommerce.Contracts.OrderItemContracts;
using Simple.Ecommerce.Contracts.PaymentInformationContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Enums.PaymentMethod;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Infra.Interfaces.Generic;

namespace Simple.Ecommerce.Infra.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly TesteDbContext _context;
        private readonly IGenericCreateRepository<Order> _createRepository;
        private readonly IGenericDeleteRepository<Order> _deleteRepository;
        private readonly IGenericDetachRepository<Order> _detachRepository;
        private readonly IGenericGetRepository<Order> _getRepository;
        private readonly IGenericListRepository<Order> _listRepository;
        private readonly IGenericUpdateRepository<Order> _updateRepository;

        public OrderRepository(
            TesteDbContext context, 
            IGenericCreateRepository<Order> createRepository, 
            IGenericDeleteRepository<Order> deleteRepository, 
            IGenericDetachRepository<Order> detachRepository,
            IGenericGetRepository<Order> getRepository, 
            IGenericListRepository<Order> listRepository, 
            IGenericUpdateRepository<Order> updateRepository
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

        public async Task<Result<Order>> Create(Order entity, bool skipSave = false)
        {
            return await _createRepository.Create(_context, entity, skipSave);
        }

        public async Task<Result<bool>> Delete(int id, bool skipSave = false)
        {
            return await _deleteRepository.Delete(_context, id);
        }

        public async Task<Result<bool>> DeletePaymentMethod(int id)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(p => p.Id == id && !p.Deleted);
            if (order is null)
            {
                return Result<bool>.Failure(new List<Error> { new Error("NotFound", "Pedido não encontrado!") });
            }

            order.UpdatePaymentInformation(null);
            order.UpdateStatus("Altered", order.OrderLock);
            if (order.Validate() is { IsFailure: true } result)
            {
                return Result<bool>.Failure(result.Errors!);
            }

            _context.Entry(order).Reference(o => o.PaymentInformation).IsModified = true;
            await _context.SaveChangesAsync();
            return Result<bool>.Success(true);
        }

        public void Detach(Order entity)
        {
            _detachRepository.Detach(_context, entity);
        }

        public async Task<Result<Order>> Get(int id, bool NoTracking = true)
        {
            return await _getRepository.Get(_context, id, NoTracking);
        }

        public async Task<Result<OrderCompleteDTO>> GetCompleteOrder(int id)
        {
            var order = await (
                from o in _context.Orders
                join u in _context.Users on o.UserId equals u.Id
                join d in _context.Discounts on o.DiscountId equals d.Id into discountJoin
                from dj in discountJoin.DefaultIfEmpty()
                where o.Id == id && !o.Deleted
                select new
                {
                    Order = o,
                    UserName = u.Name,
                    AppliedDiscount = dj != null ? new DiscountItemDTO(
                        dj.Id,
                        dj.Name,
                        dj.DiscountType,
                        dj.DiscountScope,
                        dj.DiscountValueType,
                        dj.Value,
                        dj.ValidFrom,
                        dj.ValidTo,
                        dj.IsActive,
                        null
                    ) : null
                }).FirstOrDefaultAsync();

            if (order is null)
            {
                return Result<OrderCompleteDTO>.Failure(new List<Error> { new("NotFound", "Pedido não encontrado!") });
            }

            var orderItemsData = await (
                from oi in _context.OrderItems
                join p in _context.Products on oi.ProductId equals p.Id
                join d in _context.Discounts on oi.DiscountId equals d.Id into discountJoin
                from dj in discountJoin.DefaultIfEmpty()
                where oi.OrderId == id && !oi.Deleted
                select new
                {
                    OrderItem = oi,
                    ProductName = p.Name,
                    Discount = dj
                }).ToListAsync();

            var tieredDiscountIds = orderItemsData
                .Where(oi => oi.Discount != null && oi.Discount.DiscountType == DiscountType.Tiered)
                .Select(oi => oi.Discount.Id)
                .Distinct()
                .ToList();

            var allDiscountTiers = await _context.DiscountTiers
                .Where(dt => tieredDiscountIds.Contains(dt.DiscountId) && !dt.Deleted)
                .Select(dt => new
                {
                    dt.DiscountId,
                    Tier = new DiscountTierResponse(
                        dt.Id,
                        dt.Name,
                        dt.MinQuantity,
                        dt.Value,
                        null
                    )
                }).ToListAsync();

            var groupedTiers = allDiscountTiers
                .GroupBy(x => x.DiscountId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Tier).ToList());

            var items = new List<OrderItemDTO>();
            var bundledItems = new List<BundleItemsDTO>();

            foreach (var item in orderItemsData)
            {
                DiscountItemDTO discountItemDTO = null;

                if (item.Discount != null)
                {
                    List<DiscountTierResponse> tiers = null;
                    if (item.Discount.DiscountType == DiscountType.Tiered)
                    {
                        groupedTiers.TryGetValue(item.Discount.Id, out tiers);
                    }

                    if (item.Discount.DiscountType is DiscountType.Percentage or
                                                      DiscountType.FixedAmount or
                                                      DiscountType.BuyOneGetOne or
                                                      DiscountType.Tiered)
                    {
                        discountItemDTO = new DiscountItemDTO(
                            item.Discount.Id,
                            item.Discount.Name,
                            item.Discount.DiscountType,
                            item.Discount.DiscountScope,
                            item.Discount.DiscountValueType,
                            item.Discount.Value,
                            item.Discount.ValidFrom,
                            item.Discount.ValidTo,
                            item.Discount.IsActive,
                            tiers
                        );
                        items.Add(new OrderItemDTO(
                            item.OrderItem.Id,
                            item.OrderItem.ProductId,
                            item.ProductName,
                            item.OrderItem.Quantity,
                            item.OrderItem.Price,
                            discountItemDTO
                        ));
                    }
                    else if (item.Discount.DiscountType == DiscountType.Bundle)
                    {
                        var existingBundle = bundledItems
                            .Where(bi => bi.Id == item.Discount.Id)
                            .FirstOrDefault(bi => !bi.BundleItems.Any(bid => bid.ProductId == item.OrderItem.ProductId));

                        if (existingBundle is not null)
                        {
                            existingBundle.BundleItems.Add(new BundleItemDTO(
                                item.OrderItem.Id,
                                item.OrderItem.ProductId,
                                item.ProductName,
                                item.OrderItem.Quantity,
                                item.OrderItem.Price
                            ));
                        }
                        else
                        {
                            bundledItems.Add(new BundleItemsDTO(
                                item.Discount.Id,
                                item.Discount.Name,
                                item.Discount.DiscountType,
                                item.Discount.DiscountScope,
                                item.Discount.DiscountValueType,
                                item.Discount.Value,
                                item.Discount.ValidFrom,
                                item.Discount.ValidTo,
                                item.Discount.IsActive,
                                new List<BundleItemDTO>{
                                    new BundleItemDTO(
                                        item.OrderItem.Id,
                                        item.OrderItem.ProductId,
                                        item.ProductName,
                                        item.OrderItem.Quantity,
                                        item.OrderItem.Price
                                    )}
                            ));
                        }
                    }
                }
                else
                {
                    items.Add(new OrderItemDTO(
                        item.OrderItem.Id,
                        item.OrderItem.ProductId,
                        item.ProductName,
                        item.OrderItem.Quantity,
                        item.OrderItem.Price,
                        null
                    ));
                }
            }

            var resultDTO = new OrderCompleteDTO(
                order.Order.Id,
                order.Order.UserId,
                order.UserName,
                order.Order.OrderType,
                new OrderAddressResponse(
                    order.Order.Address.Number,
                    order.Order.Address.Street,
                    order.Order.Address.Neighbourhood,
                    order.Order.Address.City,
                    order.Order.Address.Country,
                    order.Order.Address.Complement,
                    order.Order.Address.CEP
                ),
                order.Order.PaymentInformation is null
                    ? null
                    : new PaymentInformationOrderResponse(
                        order.Order.PaymentInformation.PaymentMethod,
                        order.Order.PaymentInformation.PaymentName,
                        order.Order.PaymentInformation.PaymentMethod is not (PaymentMethod.CreditCard or PaymentMethod.CreditCard)
                            ? order.Order.PaymentInformation.PaymentKey
                            : null,
                        order.Order.PaymentInformation.ExpirationMonth,
                        order.Order.PaymentInformation.ExpirationYear,
                        order.Order.PaymentInformation.CardFlag,
                        order.Order.PaymentInformation.Last4Digits
                    ),
                order.Order.TotalPrice,
                order.Order.OrderDate,
                order.Order.Confirmation,
                order.Order.Status,
                order.AppliedDiscount,
                items,
                bundledItems
            );

            return Result<OrderCompleteDTO>.Success(resultDTO);
        }

        public async Task<Result<OrderDiscountDTO?>> GetDiscountDTOById(int id)
        {
            var result = await (
                from o in _context.Orders
                join d in _context.Discounts on o.DiscountId equals d.Id
                where o.Id == id && !o.Deleted
                select new OrderDiscountDTO(
                    o.Id,
                    d.Id,
                    d.Name,
                    d.DiscountType,
                    d.DiscountScope,
                    d.DiscountValueType,
                    d.Value,
                    d.ValidFrom,
                    d.ValidTo,
                    d.IsActive
                )).FirstOrDefaultAsync();

            return Result<OrderDiscountDTO?>.Success(result);
        }

        public async Task<Result<bool>> GetFirstPurchase(int userId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.UserId == userId && !o.Deleted);
            if (order is null)
            {
                return Result<bool>.Failure(new List<Error> { new("NotFound", "O usuário não tem nenhum pedido!") });
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<List<Order>>> List()
        {
            return await _listRepository.List(_context);
        }

        public async Task<Result<Order>> Update(Order entity, bool skipSave = false)
        {
            return await _updateRepository.Update(_context, entity, skipSave);
        }
    }
}
