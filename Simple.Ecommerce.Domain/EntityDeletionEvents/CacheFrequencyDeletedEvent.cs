using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.EntityDeletionEvents
{
    public class CacheFrequencyDeletedEvent : IDeleteEvent
    {
        public int CacheFrequencyId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public CacheFrequencyDeletedEvent(int cacheFrequencyId)
        {
            CacheFrequencyId = cacheFrequencyId;
        }
    }
}
