using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWV.Models
{
    public class AspNetUsers
    {
        [Key]
        public string Id { get; set; }
        public string Email { get; set; }
       
        public System.Boolean? EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }

        public System.Boolean? PhoneNumber { get; set; }
        public System.Boolean? PhoneNumberConfirmed { get; set; }
        public System.Boolean? TwoFactorEnabled { get; set; }


        public DateTime? LockoutEndDateUtc { get; set; }
        public System.Boolean? LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public string name { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string mobile { get; set; }

        public string landline { get; set; }
        public string address { get; set; }
        public string AssignedMachine { get; set; }
        public string location { get; set; }
        public string contactperson { get; set; }
    }
}