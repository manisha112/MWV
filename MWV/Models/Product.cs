using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class Product
    {
        public Product()
        {
            this.Product_price = new HashSet<Product_prices>();
            this.Order_product = new HashSet<Order_Products>();
            this.Product_papermill = new HashSet<Product_papermill>();
        }
        [Key]
        public string product_code { get; set; }
        public string gsm_code { get; set; }
        public string bf_code { get; set; }
        public string description { get; set; }
        public Nullable<DateTime> created_on { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_on { get; set; }
        public string modified_by { get; set; }

        public virtual Bf Bf { get; set; }
        public virtual Gsm Gsm { get; set; }

        public virtual ICollection<Product_prices> Product_price { get; set; }
        public virtual ICollection<Order_Products> Order_product { get; set; }
        public virtual ICollection<Product_papermill> Product_papermill { get; set; }
    }
}