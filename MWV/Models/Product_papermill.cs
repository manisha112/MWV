using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class Product_papermill
    {
        [Key]
        public int product_papermill_id { get; set; }
        public string product_code { get; set; }
        public int papermill_id { get; set; }
        public Nullable<int> time_prepare_rawmaterial { get; set; }
        public Nullable<int> time_prepare_jumbo { get; set; }
        public Nullable<int> time_move_jumbo_cutter { get; set; }
        public Nullable<int> time_to_cut_jumbo { get; set; }
        public Nullable<int> time_to_weigh_label_child { get; set; }
        public Nullable<int> time_to_move_dispatch { get; set; }

        public virtual Papermill Papermill { get; set; }
        public virtual Product Product { get; set; }
    }
}