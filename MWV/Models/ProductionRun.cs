using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class ProductionRun
    {
        [Key]
        public int pr_id { get; set; }
        public int papermill_id { get; set; }
        public DateTime? estimated_start { get; set; }
        public DateTime? estimated_end { get; set; }
        public int?  run_time{ get; set; }
        public DateTime? actual_start { get; set; }
        public DateTime? actual_end { get; set; }
        public int? actual_runtime { get; set; }
        public decimal? estimated_qty { get; set; }
        public decimal? actual_qty { get; set; }
        public string pdf_file { get; set; }
        public int? schedule_id { get; set; }
    }
}