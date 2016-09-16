using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class Audit_trail_detail
    {
           [Key]
        public int audit_trail_details_id { get; set; }
        public int audit_trail_id { get; set; }
        public string field_name { get; set; }
        public string old_value { get; set; }
        public string new_value { get; set; }
    }
}