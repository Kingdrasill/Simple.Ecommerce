using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.OrderDiscountContracts;
using Simple.Ecommerce.Domain.Entities.OrderDiscountEnity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.ValueObjects.UseCacheObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Commands
{
    public class AddDiscountOrderCommand : IAddDiscountOrderCommand
    {
        private readonly IOrderRepository _repository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IOrderDiscountRepository _orderDiscountRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public AddDiscountOrderCommand(
            IOrderRepository repository, 
            IDiscountRepository discountRepository,
            IOrderDiscountRepository orderDiscountRepository, 
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _discountRepository = discountRepository;
            _orderDiscountRepository = orderDiscountRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<OrderDiscountResponse>> Execute(OrderDiscountRequest request)
        {
            var getOrder = await _repository.Get(request.OrderId);
            if (getOrder.IsFailure)
            {
                return Result<OrderDiscountResponse>.Failure(getOrder.Errors!);
            }

            var getDiscount = await _discountRepository.Get(request.DiscountId);
            if (getDiscount.IsFailure)
            {
                return Result<OrderDiscountResponse>.Failure(getDiscount.Errors!);
            }

            if (getDiscount.GetValue().DiscountScope != DiscountScope.Order)
            {
                return Result<OrderDiscountResponse>.Failure(new List<Error> { new("AddDiscountOrderCommand.IncorrectType.DiscountScope", "Não se pode adicionar a um pedido um desconto que não seja para pedido!") });
            }

            var instance = new OrderDiscount().Create(
                0,
                request.OrderId,
                request.DiscountId
            );
            if (instance.IsFailure)
            {
                return Result<OrderDiscountResponse>.Failure(instance.Errors!);
            }

            var createResult = await _orderDiscountRepository.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<OrderDiscountResponse>.Failure(createResult.Errors!);
            }

            var orderDiscount = createResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<OrderDiscount>();

            var response = new OrderDiscountResponse(
                orderDiscount.Id,
                orderDiscount.OrderId,
                orderDiscount.DiscountId
            );

            return Result<OrderDiscountResponse>.Success(response);
        }
    }
}
