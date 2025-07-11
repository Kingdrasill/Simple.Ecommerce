﻿using Simple.Ecommerce.App.Interfaces.Commands.DiscountCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.Patterns.UoW;
using Simple.Ecommerce.Domain.Entities.DiscountTierEntity;
using Simple.Ecommerce.Domain.Objects;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.DiscountCases.Commands
{
    public class DeleteDiscountTierDiscountCommand : IDeleteDiscountTierDiscountCommand
    {
        private readonly IDiscountTierRepository _repository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public DeleteDiscountTierDiscountCommand(
            IDiscountTierRepository repository, 
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

        public async Task<Result<bool>> Execute(int id)
        {
            var deleteResult = await _repository.Delete(id);
            if (deleteResult.IsSuccess)
            {
                var commit = await _saverOrTransectioner.SaveChanges();
                if (commit.IsFailure)
                    return commit;

                if (_useCache.Use)
                    _cacheHandler.SetItemStale<DiscountTier>();
            }

            return deleteResult;
        }
    }
}
