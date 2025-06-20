namespace Simple.Ecommerce.Domain.Interfaces.BaseEntity
{
    public interface IBaseEntity
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public void MarkAsDeleted(bool raiseEvent = true);
    }
}
