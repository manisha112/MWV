using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace MWV.Models
{
    public class Alerts
    {
        public Alerts()
        { }
        [Key]
        public int alert_id { get; set; }
        public string alert_action { get; set; }
        public string alert_for_role { get; set; }
        public int? alert_for_agentid { get; set; }
        public string alert_text { get; set; }
        public string alert_subject { get; set; }
        public DateTime? created_on { get; set; }
        public string created_by { get; set; }
        public int? viewed { get; set; }
        public string machinehead_aspnetuserid { get; set; }
    }
}