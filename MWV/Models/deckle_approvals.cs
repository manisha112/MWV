using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWV.Models
{
    public class deckle_approvals
    {
        [Key]
        public int dm_id { get; set; }
        public DateTime? request_date { get; set; }
        public string bf_code { get; set; }
        public string gsm_code { get; set; }
        public string shade_code { get; set; }
        public int? papermill_id { get; set; }
        public string matched_sizes { get; set; }
        public decimal? required_size { get; set; }
        public decimal? required_weight { get; set; }
        public string approver_aspnetuserid { get; set; }
        public DateTime? approved_on { get; set; }
        public string action { get; set; }
        public string remarks { get; set; }
        public int? used  { get; set; }
	    public DateTime? used_on  { get; set; }
    }
}