using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class ProductionChild
    {
        [Key]
        public int pc_id { get; set; }
        public int? pj_id { get; set; }
        public int? order_product_id { get; set; }
        public decimal? width { get; set; }
        public decimal? qty { get; set; }
        public string child_rollno { get; set; }
        public int? sequence { get; set; }
        public string truck_no { get; set; }
        public decimal? actual_loaded_qty { get; set; }
        public DateTime? actual_loaded_on { get; set; }
        public int? sidecut_id { get; set; }
        public DateTime? actual_start { get; set; }
        public DateTime? actual_end { get; set; }
        public DateTime? external_startdate { get; set; }
        public DateTime? external_enddate { get; set; }
        public string marking { get; set; }
        public int? truck_dispatch_id { get; set; }
        [NotMapped]
        public decimal TempJumboIndex { get; set; }
        
    }
}