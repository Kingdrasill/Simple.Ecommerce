using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.Domain.Entities.FrequencyEntity
{
    public class CacheFrequency : BaseEntity
    {
        public string Entity { get; private set; }
        public int Frequency { get; private set; }
        public int? HoursToLive { get; set; }
        public bool Expirable { get; private set; }
        public bool KeepCached { get; private set; }

        public CacheFrequency() { }

        private CacheFrequency(int id, string entity, int frequency, int? hoursToLive, bool expirable = true, bool keepCached = false)
        {
            Id = id;
            Entity = entity;
            Frequency = frequency;
            HoursToLive = hoursToLive;
            Expirable = expirable;
            KeepCached = keepCached;
        }

        public Result<CacheFrequency> Create(int id, string entity, int frequency, int? hoursToLive, bool expirable = true, bool keepCached = false) 
        { 
            return new CacheFrequencyValidator().Validate(new CacheFrequency(id, entity, frequency, hoursToLive, expirable, keepCached));
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {

            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new CacheFrequencyDeletedEvent(Id));
        }
    }
}
