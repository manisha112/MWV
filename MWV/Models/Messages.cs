using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class Messages
    {
        [Key]
        public int message_id { get; set; }
        public string message_action { get; set; }
        public string message_for_role { get; set; }
        public int? message_for_agentid { get; set; }
        public string message_text { get; set; }
        public string message_subject { get; set; }

        public string recipient1 { get; set; }
        public string recipient2 { get; set; }
        public string cc1 { get; set; }
        public string cc2 { get; set; }
        public string bcc1 { get; set; }
        public string bcc2 { get; set; }

        public DateTime created_on { get; set; }
        public string created_by { get; set; }
        public string attachment1 { get; set; }
        public string attachment2 { get; set; }
        public string status { get; set; }
        public int? viewed { get; set; }
        public string machinehead_aspnetuserid { get; set; }
        public int? order_id { get; set; }

    }
}