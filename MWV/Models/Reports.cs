using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MWV.Models
{
    public class Reports
    {
        [Key]
        public int report_id { get; set; }
        public string created_by { get; set; }
        public DateTime? created_on { get; set; }
        public string report_name { get; set; }
        public string report_criteria { get; set; }
        public string file_name { get; set; }

    }
}