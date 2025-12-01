using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models
{
    internal class Finance
    {
        public string Income { get; set; } = string.Empty;

        public string Cost { get; set; } = string.Empty;

        public string Profit { get; set; } = string.Empty;

        public string Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
    }
}
