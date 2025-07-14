using Simple.Ecommerce.App.Interfaces.Commands.OrderCommands;
using Simple.Ecommerce.App.Interfaces.Data;
using Simple.Ecommerce.App.Interfaces.Services.Cache;
using Simple.Ecommerce.App.Interfaces.Services.UnityOfWork;
using Simple.Ecommerce.App.Services.OrderProcessing.Handlers;
using Simple.Ecommerce.Contracts.OrderContracts;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Errors.BaseError;
using Simple.Ecommerce.Domain.Exceptions.ResultException;
using Simple.Ecommerce.Domain.OrderProcessing.Commands;
using Simple.Ecommerce.Domain.Settings.UseCacheSettings;

namespace Simple.Ecommerce.App.UseCases.OrderCases.Commands
{
    public class ConfirmOrderCommand : IConfirmOrderCommand
    {
        private readonly ProcessConfirmedOrderCommandHandler _processConfirmedOrderCommandHandler;
        private readonly IOrderRepository _repository;
        private readonly ISaverTransectioner _saverOrTransectioner;
        private readonly UseCache _useCache;
        private readonly ICacheHandler _cacheHandler;

        public ConfirmOrderCommand(
            ProcessConfirmedOrderCommandHandler processConfirmedOrderCommandHandler,
            IOrderRepository repository, 
            ISaverTransectioner unityOfWork,
            UseCache useCache, 
            ICacheHandler cacheHandler
        )
        {
            _processConfirmedOrderCommandHandler = processConfirmedOrderCommandHandler;
            _repository = repository;
            _saverOrTransectioner = unityOfWork;
            _useCache = useCache;
            _cacheHandler = cacheHandler;
        }

        public async Task<Result<OrderCompleteDTO>> Execute(int id)
        {
            var processCommand = new ProcessConfirmedOrderCommand(id);
            var processedResult = await _processConfirmedOrderCommandHandler.Handle(processCommand);
            if (processedResult.IsFailure)
            {
                return Result<OrderCompleteDTO>.Failure(processedResult.Errors!);
            }

            await _saverOrTransectioner.BeginTransaction();
            try
            {
                var order = processedResult.GetValue();
                order.UpdateStatus("Pending Payment");

                var updateResult = await _repository.Update(order);
                if (updateResult.IsFailure)
                {
                    throw new ResultException(updateResult.Errors!);
                }

                var getCompleteOrder = await _repository.GetCompleteOrder(id);
                if (getCompleteOrder.IsFailure)
                {
                    throw new ResultException(getCompleteOrder.Errors!);
                }

                await _saverOrTransectioner.Commit();
                if (_useCache.Use)
                    _cacheHandler.SetItemStale<Order>();

                return Result<OrderCompleteDTO>.Success(getCompleteOrder.GetValue());
            }
            catch (ResultException rex)
            {
                await _saverOrTransectioner.Rollback();
                return Result<OrderCompleteDTO>.Failure(rex.Errors!);
            }
            catch (Exception ex)
            {
                await _saverOrTransectioner.Rollback();
                return Result<OrderCompleteDTO>.Failure(new List<Error>{ new("ConfirmOrderCommand.Unknown", ex.Message) });
            }
        }
    }
}
