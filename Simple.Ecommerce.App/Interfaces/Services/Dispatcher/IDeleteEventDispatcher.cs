using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.App.Interfaces.Services.Dispatcher
{
    public interface IDeleteEventDispatcher
    {
        Task Dispatch(IEnumerable<IDeleteEvent> domainEvents);
    }
}
