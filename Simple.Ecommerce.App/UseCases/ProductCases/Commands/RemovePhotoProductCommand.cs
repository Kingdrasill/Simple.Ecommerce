﻿using ImageFile.Library.Core.Enums;
using ImageFile.Library.Core.Services;
using Simple.Ecommerce.App.Interfaces.Commands.ProductCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.Patterns.UoW;
using Simple.Ecommerce.Domain.Entities.ProductPhotoEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.ProductCases.Commands
{
    public class RemovePhotoProductCommand : IRemovePhotoProductCommand
    {
        private readonly IProductPhotoRepository _productPhotoRepository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly IImageManager _imageManager;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public RemovePhotoProductCommand(
            IProductPhotoRepository productPhotoRepository, 
            ISaverTransectioner unityOfWork,
            IImageManager imageManager,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _productPhotoRepository = productPhotoRepository;
            _saverOrTransectioner = unityOfWork;
            _imageManager = imageManager;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<bool>> Execute(int productPhotoId)
        {
            var getProductPhoto = await _productPhotoRepository.Get(productPhotoId);
            if (getProductPhoto.IsFailure)
            {
                return Result<bool>.Failure(getProductPhoto.Errors!);
            }

            var productPhoto = getProductPhoto.GetValue();

            var deleteImage = await _imageManager.DeleteImageAsync(productPhoto.Photo.FileName);
            if (deleteImage.Result != ImageOperationResult.Success)
            {
                return Result<bool>.Failure(new List<Error> { new($"ImageFileSystem.{deleteImage.Result}", deleteImage.ErrorMessage!) });
            }

            var deleteResult = await _productPhotoRepository.Delete(productPhotoId);
            if (deleteResult.IsSuccess)
            {
                var commit = await _saverOrTransectioner.SaveChanges();
                if (commit.IsFailure)
                    return commit;

                if (_useCache.Use)
                    _cacheHandler.SetItemStale<ProductPhoto>();
            }

            return deleteResult;
        }
    }
}
