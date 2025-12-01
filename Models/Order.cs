
using System;

namespace ERP.Models
{
    public class Order
    {
        public string OrderId { get; set; } = Guid.NewGuid().ToString("N");
        public string Customer { get; set; } = string.Empty;
        public decimal Total { get; set; }
    }
}