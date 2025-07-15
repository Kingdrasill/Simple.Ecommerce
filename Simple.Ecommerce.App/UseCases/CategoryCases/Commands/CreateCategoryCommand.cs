using Simple.Ecommerce.App.Interfaces.Commands.CategoryCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.Contracts.CategoryContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CategoryEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.CategoryCases.Commands
{
    public class CreateCategoryCommand : ICreateCategoryCommand
    {
        private readonly ICategoryRepository _repository;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public CreateCategoryCommand(
            ICategoryRepository repository,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<CategoryResponse>> Execute(CategoryRequest request)
        {
            var getCategory = await _repository.Get(request.Id);
            if (getCategory.IsSuccess)
            {
                return Result<CategoryResponse>.Failure(new List<Error> { new Error("CreateCategoryCommand.AlreadyExists", "A categoria já existe!") });
            }

            var instance = new Category().Create(
                request.Id,
                request.Name
            );
            if (instance.IsFailure)
            {
                return Result<CategoryResponse>.Failure(instance.Errors!);
            }

            var createResult = await _repository.Create(instance.GetValue());
            if (createResult.IsFailure)
            {
                return Result<CategoryResponse>.Failure(createResult.Errors!);
            }
            var category = createResult.GetValue();

            if (_useCache.Use)
                _cacheHandler.SetItemStale<Category>();

            var response = new CategoryResponse(
                category.Id,
                category.Name
            );

            return Result<CategoryResponse>.Success(response);
        }
    }
}
