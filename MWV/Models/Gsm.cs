using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class Gsm
    {
        public Gsm()
        {
            this.Products = new HashSet<Product>();
        }
        [Key]
        public string gsm_code { get; set; }
        public string description { get; set; }
        public Nullable<DateTime> created_on { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_on { get; set; }
        public string modified_by { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}