using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class papermill_logs
    {
        [Key]
        public int pm_log_id { get; set; }
        public int? papermill_id { get; set; }
        public DateTime? stop_datetime { get; set; }
        public DateTime? estimated_start { get; set; }
        public string aspnetuser_id { get; set; }
        public DateTime? actual_start { get; set; }      
        public string remarks { get; set; }

    }
}