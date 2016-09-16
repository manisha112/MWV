using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class Schedule
    {
        [Key]
        public int schedule_id { get; set; }
        public int papermill_id { get; set; }
        public DateTime? start_date { get; set; }
        public DateTime? end_date { get; set; }
        public string shade_code { get; set; }
        public string status { get; set; }
        public Nullable<DateTime> created_on { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_on { get; set; }
        public string modified_by { get; set; }

        // Navigation Properties
        public virtual Papermill papermills { get; set; }

        [NotMapped]
        public decimal TotalRuntime { get; set; }
    }
}