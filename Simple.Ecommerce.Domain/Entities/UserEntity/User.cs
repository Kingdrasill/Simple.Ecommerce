using Simple.Ecommerce.Domain.Entities.LoginEntity;
using Simple.Ecommerce.Domain.Entities.OrderEntity;
using Simple.Ecommerce.Domain.Entities.ReviewEntity;
using Simple.Ecommerce.Domain.Entities.UserAddressEntity;
using Simple.Ecommerce.Domain.Entities.UserCardEntity;
using Simple.Ecommerce.Domain.EntityDeletionEvents;
using Simple.Ecommerce.Domain;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.PhotoObject;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Simple.Ecommerce.Domain.Entities.UserEntity
{
    public class User : BaseEntity
    {
        public string Name {  get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string Password { get; private set; }
        public Photo? Photo { get; private set; } = null;
        [IgnoreDataMember, NotMapped]
        public ICollection<UserAddress> UserAddresses { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<UserCard> UserCards { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<Review> Reviews { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<Order> Orders { get; private set; }
        [IgnoreDataMember, NotMapped]
        public ICollection<Login> Logins { get; private set; }

        public User() 
        {
            UserAddresses = new HashSet<UserAddress>();
            UserCards = new HashSet<UserCard>();
            Reviews = new HashSet<Review>();
            Orders = new HashSet<Order>();
            Logins = new HashSet<Login>();
        }

        private User(int id, string name, string email, string phoneNumber, string password, Photo? photo = null)
        {
            Id = id;
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Password = password;
            Photo = photo;

            UserAddresses = new HashSet<UserAddress>();
            UserCards = new HashSet<UserCard>();
            Reviews = new HashSet<Review>();
            Orders = new HashSet<Order>();
            Logins = new HashSet<Login>();
        }

        public Result<User> Create(int id, string name, string email, string phoneNumber, string password, Photo? photo = null)
        {
            return new UserValidator().Validate(new User(id, name, email, phoneNumber, password, photo));
        }

        public void AddOrUpdatePhoto(Photo? photo) => Photo = photo;

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new UserDeletedEvent(Id));
        }
    }
}
