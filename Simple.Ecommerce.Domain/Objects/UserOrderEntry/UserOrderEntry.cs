﻿namespace Simple.Ecommerce.Domain.Objects
{
    public class UserOrderEntry
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
