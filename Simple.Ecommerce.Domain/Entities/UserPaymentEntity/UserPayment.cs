using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.PaymentInformationObject;

namespace Simple.Ecommerce.Domain.Entities.UserPaymentEntity
{
    public class UserPayment : BaseEntity
    {
        public int UserId { get; private set; }
        public User User { get; private set; } = null!;
        public PaymentInformation PaymentInformation { get; private set; }

        public UserPayment() { }

        private UserPayment(int id, int userId, PaymentInformation paymentInformation)
        {
            Id = id;
            UserId = userId;
            PaymentInformation = paymentInformation;
        }

        public Result<UserPayment> Create(int id, int userId, PaymentInformation paymentInformation)
        {
            return new UserPaymentValidator().Validate(new UserPayment(id, userId, paymentInformation));
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new UserPaymentDeletedEvent(Id));
            
        }
    }
}
