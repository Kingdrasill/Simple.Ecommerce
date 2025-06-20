using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.Events.DeletedEvent
{
    public class UserAddressDeletedEvent : IDeleteEvent
    {
        public int UserAddressId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public UserAddressDeletedEvent(int userAddressId)
        {
            UserAddressId = userAddressId;
        }
    }
}
