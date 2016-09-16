using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class Stock
    {
        [Key]
        public int stock_id { get; set; }
        public DateTime? stock_date { get; set; }
        public int papermill_id { get; set; }
        public int agent_id { get; set; }
        public int customer_id { get; set; }
        public string product_code { get; set; }
        public string shade_code { get; set; }
        public Nullable<decimal> opening_stock { get; set; }
        public Nullable<decimal> stock_produced { get; set; }
        public Nullable<decimal> stock_shipped { get; set; }
    }
}