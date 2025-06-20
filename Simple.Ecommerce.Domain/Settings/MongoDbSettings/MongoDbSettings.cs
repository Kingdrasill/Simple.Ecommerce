namespace Simple.Ecommerce.Domain.Settings.MongoDbSettings
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string OrderSummaryCollection { get; set; }
        public string StockCollection { get; set; }
        public string UserHistoryCollection { get; set; }
    }
}
