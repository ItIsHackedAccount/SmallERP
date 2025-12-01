using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models
{
    public class Sales
    {
        public int Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public string Customer { get; set; } = "User";

        public string Amount { get; set; } = string.Empty;

        public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        public int Inventory_ID { get; set; }
    }
}
