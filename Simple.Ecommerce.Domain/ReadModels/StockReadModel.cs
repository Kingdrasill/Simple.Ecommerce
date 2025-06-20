namespace Simple.Ecommerce.Domain.ReadModels
{
    public class StockReadModel : BaseReadModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int QuantityAvailable { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
