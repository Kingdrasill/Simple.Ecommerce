using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.AddressContracts;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Commands
{
    public class CreateOrderCommand : ICreateOrderCommand
    {
        private readonly IOrderRepository _repository;
        private readonly IUserRepository _userRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CreateOrderCommand(
            IOrderRepository repository, 
            IUserRepository userRepository,
            IDiscountRepository discountRepository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _userRepository = userRepository;
            _discountRepository = discountRepository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<OrderResponse>> Execute(OrderRequest request)
        {
            var getOrder = await _repository.Get(request.Id);
            if (getOrder.IsSuccess)
            {
                return Result<OrderResponse>.Failure(new List<Error> { new("CreateOrderCommand.AlreadyExists", "O pedido já existe!") });
            }

            var getUser = await _userRepository.Get(request.UserId);
            if (getUser.IsFailure)
            {
                return Result<OrderResponse>.Failure(getUser.Errors!);
            }

            var getDiscount = request.DiscountId is null ? null : await _discountRepository.Get(request.DiscountId.Value);
            if (getDiscount is not null)
            {
                if (getDiscount.IsFailure)
                    return Result<OrderResponse>.Failure(getDiscount.Errors!);

                List<Error> errors = new();
                
                if (getDiscount.GetValue().DiscountScope != DiscountScope.Order)
                    errors.Add(new("CreateOrderCommand.InvalidDiscountScope", "O desconto não é aplicável a pedidos!"));
                if (getDiscount.GetValue().DiscountType == DiscountType.Bundle)
                    errors.Add(new("CreateOrderCommand.InvalidDiscountType", "O desconto de pacote não é aplicável a pedidos!"));
                if (getDiscount.GetValue().ValidFrom > DateTime.UtcNow)
                    errors.Add(new("CreateOrderCommand.DiscountNotValidYet", "O desconto ainda não está válido!"));
                if (getDiscount.GetValue().ValidTo < DateTime.UtcNow)
                    errors.Add(new("CreateOrderCommand.DiscountExpired", "O desconto já expirou!"));
                if (!getDiscount.GetValue().IsActive)
                    errors.Add(new("CreateOrderCommand.InactiveDiscount", "O desconto não está ativo!"));

                if (errors.Count != 0)
                {
                    return Result<OrderResponse>.Failure(errors);
                }
            }

            var address = new Address().Create(
                request.Address.Number,
                request.Address.Street,
                request.Address.Neighbourhood,
                request.Address.City,
                request.Address.Country,
                request.Address.Complement,
                request.Address.CEP
            );
            if (address.IsFailure)
            {
                return Result<OrderResponse>.Failure(address.Errors!);
            }

            var instance = new Order().Create(
                request.Id,
                request.UserId,
                request.OrderType,
                address.GetValue(),
                request.PaymentMethod,
                request.TotalPrice,
                request.OrderDate,
                false,
                "Not Confirmed",
                request.DiscountId
            );
            if (instance.IsFailure) 
            {
                return Result<OrderResponse>.Failure(instance.Errors!);
            }

            var createResult = await _repository.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<OrderResponse>.Failure(createResult.Errors!);
            }

            var order = createResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Order>();

            var addressResponse = new OrderAddressResponse(
                order.Address.Number,
                order.Address.Street,
                order.Address.Neighbourhood,
                order.Address.City,
                order.Address.Country,
                order.Address.Complement,
                order.Address.CEP
            );

            var response = new OrderResponse(
                order.Id,
                order.UserId,
                order.OrderType,
                addressResponse,
                order.PaymentMethod,
                order.TotalPrice,
                order.OrderDate,
                order.Confirmation,
                order.Status,
                order.DiscountId
            );

            return Result<OrderResponse>.Success(response);
        }
    }
}
