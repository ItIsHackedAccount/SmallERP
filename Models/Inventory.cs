using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models
{
    public class Inventory
    {
        public int Id { get; set; }
        public string SKU { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string Amount { get; set; } = string.Empty;
    }
}
