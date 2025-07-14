using Simple.Ecommerce.App.Interfaces.Commands.CategoryCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
using Simple.Ecommerce.Contracts.CategoryContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.CategoryEntity;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.CategoryCases.Commands
{
    public class UpdateCategoryCommand : IUpdateCategoryCommand
    {
        private readonly ICategoryRepository _repository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public UpdateCategoryCommand(
            ICategoryRepository repository,
            ISaverTransectioner unityOfWork,
            UseCache useCache,
            ICacheHandler cacheHandler
        )
        {
            _repository = repository;
            _saverOrTransectioner = unityOfWork;
            _cacheHandler = cacheHandler;
            _useCache = useCache;
        }

        public async Task<Result<CategoryResponse>> Execute(CategoryRequest request)
        {
            var getCategory = await _repository.Get(request.Id);
            if (getCategory.IsFailure)
            {
                return Result<CategoryResponse>.Failure(getCategory.Errors!);
            }

            var instance = new Category().Create(
                request.Id,
                request.Name
            );
            if (instance.IsFailure)
            {
                return Result<CategoryResponse>.Failure(instance.Errors!);
            }

            var updateResult = await _repository.Update(instance.GetValue());
            if (updateResult.IsFailure)
            {
                return Result<CategoryResponse>.Failure(updateResult.Errors!);
            }

            var commit = await _saverOrTransectioner.SaveChanges();
            if (commit.IsFailure)
            {
                return Result<CategoryResponse>.Failure(commit.Errors!);
            }

            var category = updateResult.GetValue();

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
