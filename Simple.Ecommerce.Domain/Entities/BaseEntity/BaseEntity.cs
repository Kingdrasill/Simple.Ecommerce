using Simple.Ecommerce.Domain.Interfaces.BaseEntity;
using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;
using System.ComponentModel.DataAnnotations;

namespace Simple.Ecommerce.Domain.Entities
{
    public abstract class BaseEntity : IBaseEntity
    {
        [KeyAttribute]
        public int Id { get; set; }
        public bool Deleted { get; set; } = false;

        private List<IDeleteEvent> _domainEvents = new();
        public IReadOnlyCollection<IDeleteEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDeleteEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents() => _domainEvents.Clear();

        public abstract void MarkAsDeleted(bool raiseEvent = true);
    }
}
