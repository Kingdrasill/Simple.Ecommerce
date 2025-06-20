using Simple.Ecommerce.Domain.Interfaces.DeleteEvent;

namespace Simple.Ecommerce.Domain.Events.DeletedEvent
{
    public class ProductDiscountDeletedEvent : IDeleteEvent
    {
        public int ProductDiscountId { get; }
        public DateTime OccuredOn { get; } = DateTime.UtcNow;

        public ProductDiscountDeletedEvent(int productDiscountId)
        {
            ProductDiscountId = productDiscountId;
        }
    }
}
