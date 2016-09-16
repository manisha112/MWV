using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class ProductionTimeline
    {
        [Key]
        public int production_timeline_id { get; set; }
        public int papermill_id { get; set; }
        public string bf_code { get; set; }
        public string gsm_code { get; set; }
        public string shade_code { get; set; }
        public int speed { get; set; }
        public decimal? ton_per_hour { get; set; }
        public decimal? time_per_ton { get; set; }
        public Nullable<DateTime> created_on { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_on { get; set; }
        public string modified_by { get; set; }

        public virtual Papermill PaperMill { get; set; }
    }
}