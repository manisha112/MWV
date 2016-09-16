using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class Truck_dispatch_details
    {
        [Key]
        public int truck_dispatch_details_id { get; set; }
        public int truck_dispatch_id { get; set; }
        public int order_id { get; set; }
        public int order_product_id { get; set; }
        public decimal? qty_loaded { get; set; }
        public Nullable<decimal> qty { get; set; }

        public virtual Order_Products Order_products { get; set; }
        public virtual Order Order { get; set; }
        public virtual Truck_dispatches Truck_dispatches { get; set; }
    }
}