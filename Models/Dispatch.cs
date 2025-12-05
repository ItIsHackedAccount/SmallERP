using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Models
{
    public class Dispatch
    {
        public int Id { get; set; }

        public string DispatchNo { get; set; }

        public int OrderId { get; set; }

        public string OrderNo { get; set; }

        public string Customer { get; set; }

        public int CustomerId { get; set; }

        public string  Date { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");

        public string Status { get; set; }
    }
}
