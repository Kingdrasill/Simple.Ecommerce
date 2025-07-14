using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
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
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CreateDiscountCommand(
            IDiscountRepository repository,
            ISaverTransectioner unityOfWork,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _saverOrTransectioner = unityOfWork;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<DiscountDTO>> Execute(DiscountRequest request)
        {
            var getDiscount = await _repository.Get(request.Id);
            if (getDiscount.IsSuccess)
            {
                return Result<DiscountDTO>.Failure(new List<Error> { new("CreateDiscountCommand.AlreadyExists", "O desconto já existe!") });
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
                return Result<DiscountDTO>.Failure(instace.Errors!);
            }

            var createResult = await _repository.Create(instace.GetValue());
            if (createResult.IsFailure)
            {
                return Result<DiscountDTO>.Failure(createResult.Errors!);
            }

            var commit = await _saverOrTransectioner.SaveChanges();
            if (commit.IsFailure)
            {
                return Result<DiscountDTO>.Failure(commit.Errors!);
            }

            var discount = createResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Discount>();

            var response = new DiscountDTO(
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

            return Result<DiscountDTO>.Success(response);
        }
    }
}
