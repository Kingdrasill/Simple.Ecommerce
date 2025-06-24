using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.CardInformationObject;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.Domain.Entities.UserCardEntity
{
    public class UserCard : BaseEntity
    {
        public int UserId { get; private set; }
        public User User { get; private set; } = null!;
        public CardInformation CardInformation { get; private set; }

        public UserCard() { }

        private UserCard(int id, int userId, CardInformation cardInformation)
        {
            Id = id;
            UserId = userId;
            CardInformation = cardInformation;
        }

        public Result<UserCard> Create(int id, int userId, CardInformation cardInformation)
        {
            return new UserCardValidator().Validate(new UserCard(id, userId, cardInformation));
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new UserCardDeletedEvent(Id));
            
        }
    }
}
