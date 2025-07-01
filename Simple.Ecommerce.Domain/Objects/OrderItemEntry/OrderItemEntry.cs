namespace Simple.Ecommerce.Domain.Objects
{
    public class OrderItemEntry
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}
