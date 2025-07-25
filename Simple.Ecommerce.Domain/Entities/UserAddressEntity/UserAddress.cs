using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.AddressObject;

namespace Simple.Ecommerce.Domain.Entities.UserAddressEntity
{
    public class UserAddress : BaseEntity
    {
        public int UserId { get; private set; }
        public User User { get; private set; } = null!;
        public Address Address { get; private set; }

        public UserAddress() { }

        private UserAddress(int id, int userId, Address address) 
        {
            Id = id;
            UserId = userId;
            Address = address;
        }

        public Result<UserAddress> Create(int id, int userId, Address address)
        {
            return new UserAddressValidator().Validate(new UserAddress(id, userId, address));
        }

        public Result<UserAddress> Validate()
        {
            return new UserAddressValidator().Validate(this);
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new UserAddressDeletedEvent(Id));
        }
    }
}
