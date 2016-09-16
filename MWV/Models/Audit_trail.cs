using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class Audit_trail
    {
        [Key]
        public int audit_trail_id { get; set; }
        public string object_name { get; set; }
        public string action { get; set; }
        public string aspnet_users_id { get; set; }
        public DateTime updated_on { get; set; }
        public string role { get; set; }
    }
}