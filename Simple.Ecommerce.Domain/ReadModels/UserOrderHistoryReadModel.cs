using Simple.Ecommerce.Domain.ValueObjects.UserOrderObject;

namespace Simple.Ecommerce.Domain.ReadModels
{
    public class UserOrderHistoryReadModel : BaseReadModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public List<UserOrderEntry> Orders { get; set; }
    }
}
