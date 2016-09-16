using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWV.Models
{
    public class Bf
    {
        public Bf()
        {
            this.Products = new HashSet<Product>();
        }
        [Key]
        [Remote("GetProductCode", "Order_product", AdditionalFields = "gsm_code")]
        public string bf_code { get; set; }
        public string description { get; set; }
        public Nullable<DateTime> created_on { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_on { get; set; }
        public string modified_by { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}