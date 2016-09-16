using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class Shade
    {
        public Shade()
        {
            this.Product_prices = new HashSet<Product_prices>();
            this.order_products = new HashSet<Order_Products>();
        }
        [Key]
        public string shade_code { get; set; }
        public string description { get; set; }
        public Nullable<DateTime> created_on { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_on { get; set; }
        public string modified_by { get; set; }

        public virtual ICollection<Product_prices> Product_prices { get; set; }
        public virtual ICollection<Order_Products> order_products { get; set; }
    }
}