using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Enums.Discount;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Commands
{
    public class ChangeDiscountOrderCommand : IChangeDiscountOrderCommand
    {
        private readonly IOrderRepository _repository;
        private readonly IDiscountRepository _discountRepository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ChangeDiscountOrderCommand(
            IOrderRepository repository, 
            IDiscountRepository discountRepository,
            ISaverTransectioner unityOfWork,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _discountRepository = discountRepository;
            _saverOrTransectioner = unityOfWork;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(OrderDiscountRequest request)
        {
            var getOrder = await _repository.Get(request.OrderId);
            if (getOrder.IsFailure)
            {
                return Result<bool>.Failure(getOrder.Errors!);
            }

            var getDiscount = request.DiscountId is null ? null : await _discountRepository.Get(request.DiscountId.Value);
            if (getDiscount is not null)
            {
                if (getDiscount.IsFailure)
                    return Result<bool>.Failure(getDiscount.Errors!);

                List<Error> errors = new();

                if (getDiscount.GetValue().DiscountScope != DiscountScope.Order)
                    errors.Add(new("CreateOrderCommand.InvalidDiscountScope", "O desconto não é aplicável a pedidos!"));
                if (getDiscount.GetValue().DiscountType is DiscountType.Tiered or DiscountType.BuyOneGetOne or DiscountType.Bundle)
                    errors.Add(new("CreateOrderCommand.InvalidDiscountType", "O tipo de desconto aplicado não é aplicável a pedidos!"));
                if (getDiscount.GetValue().ValidFrom > DateTime.UtcNow)
                    errors.Add(new("CreateOrderCommand.DiscountNotValidYet", "O desconto ainda não está válido!"));
                if (getDiscount.GetValue().ValidTo < DateTime.UtcNow)
                    errors.Add(new("CreateOrderCommand.DiscountExpired", "O desconto já expirou!"));
                if (!getDiscount.GetValue().IsActive)
                    errors.Add(new("CreateOrderCommand.InactiveDiscount", "O desconto não está ativo!"));

                if (errors.Count != 0)
                {
                    return Result<bool>.Failure(errors);
                }
            }

            var order = getOrder.GetValue();
            order.UpdateDiscountId(request.DiscountId);

            var updateResult = await _repository.Update(order);
            if (updateResult.IsFailure)
            {
                return Result<bool>.Failure(updateResult.Errors!);
            }

            var commit = await _saverOrTransectioner.SaveChanges();
            if (commit.IsFailure)
            {
                return Result<bool>.Failure(commit.Errors!);
            }

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Order>();

            return Result<bool>.Success(true);
        }
    }
}
