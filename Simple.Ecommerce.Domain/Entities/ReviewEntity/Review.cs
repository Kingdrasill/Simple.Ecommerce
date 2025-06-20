using Simple.Ecommerce.Domain.Entities.ProductEntity;
using Simple.Ecommerce.Domain.Entities.UserEntity;
using Simple.Ecommerce.Domain.Events.DeletedEvent;
using Simple.Ecommerce.Domain.Validation.Validators;
using Simple.Ecommerce.Domain.ValueObjects.ResultObject;

namespace Simple.Ecommerce.Domain.Entities.ReviewEntity
{
    public class Review : BaseEntity
    {
        public int ProductId { get; private set; }
        public Product Product { get; private set; } = null!;
        public int UserId { get; private set; }
        public User User { get; private set; } = null!;
        public int Score { get; private set; }
        public string? Comment { get; private set; }

        public Review() { }

        private Review(int id, int productId, int userId, int score, string? comment) 
        {
            Id = id;
            ProductId = productId;
            UserId = userId;
            Score = score;
            Comment = comment;
        }

        public Result<Review> Create(int id, int productId, int userId, int score, string? comment)
        {
            return new ReviewValidator().Validate(new Review(id, productId, userId, score, comment));
        }

        public override void MarkAsDeleted(bool raiseEvent = true)
        {
            if (Deleted)
                return;

            Deleted = true;

            if (raiseEvent)
                AddDomainEvent(new ReviewDeletedEvent(Id));
        }
    }
}
