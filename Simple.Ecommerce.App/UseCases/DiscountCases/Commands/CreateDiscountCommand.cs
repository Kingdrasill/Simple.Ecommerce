using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.DiscountContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.DiscountEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class CreateDiscountCommand : ICreateDiscountCommand
    {
        private readonly IDiscountRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CreateDiscountCommand(
            IDiscountRepository repository,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<DiscountCompleteDTO>> Execute(DiscountRequest request)
        {
            var getDiscount = await _repository.Get(request.Id);
            if (getDiscount.IsSuccess)
            {
                return Result<DiscountCompleteDTO>.Failure(new List<Error> { new("CreateDiscountCommand.AlreadyExists", "O desconto já existe!") });
            }

            var instace = new Discount().Create(
                request.Id,
                request.Name,
                request.DiscountType,
                request.DiscountScope,
                request.DiscountValueType,
                request.Value,
                request.ValidFrom,
                request.ValidTo,
                request.IsActive
            );
            if (instace.IsFailure)
            {
                return Result<DiscountCompleteDTO>.Failure(instace.Errors!);
            }

            var createResult = await _repository.Create(instace.GetValue());
            if (createResult.IsFailure)
            {
                return Result<DiscountCompleteDTO>.Failure(createResult.Errors!);
            }
            var discount = createResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Discount>();

            var response = new DiscountCompleteDTO(
                discount.Id, 
                discount.Name,
                discount.DiscountType,
                discount.DiscountScope,
                discount.DiscountValueType,
                discount.Value,
                discount.ValidFrom,
                discount.ValidTo,
                discount.IsActive,
                null,
                null,
                null
            );

            return Result<DiscountCompleteDTO>.Success(response);
        }
    }
}
