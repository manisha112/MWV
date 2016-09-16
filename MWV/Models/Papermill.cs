using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class Papermill
    {
        public Papermill()
        {
            this.Product_papermill = new HashSet<Product_papermill>();
        }
        [Key]
        public int papermill_id { get; set; }
        public Nullable<decimal> capacity { get; set; }
        public Nullable<decimal> min_width { get; set; }
        public Nullable<decimal> max_width { get; set; }
        public string location { get; set; }
        public Nullable<decimal> deckle_min { get; set; }
        public Nullable<decimal> deckle_max { get; set; }
        public int? max_cuts { get; set; }
        public Nullable<decimal> min_diameter { get; set; }
        public Nullable<decimal> max_diameter { get; set; }
        public Nullable<decimal> max_weight_child { get; set; }
        public Nullable<decimal> min_weight_jumbo { get; set; }
        public Nullable<decimal> max_weight_jumbo { get; set; }
        public string name { get; set; }

        public Nullable<DateTime> created_on { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_on { get; set; }
        public string modified_by { get; set; }

        public string address { get; set; }
        public string status { get; set; }

        public virtual ICollection<Product_papermill> Product_papermill { get; set; }
        
    }
}