using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class ProductionJumbo
    {
        [Key]
       public int pj_id { get; set; }
        public int pr_id { get; set; }
        public int? jumbo_no { get; set; }
        public string bf_code { get; set; }
         public string gsm_code { get; set; }
         public string shade_code { get; set; }
        public decimal? planned_qty { get; set; }
        public DateTime? estimated_start { get; set; }
        public decimal? planned_width { get; set; }
        public decimal? jumbo_width { get; set; }
        public DateTime? actual_start { get; set; }
        public DateTime? actual_end { get; set; }
        
        [NotMapped]
        public decimal TempMasterJumboNo { get; set; }

    }
}