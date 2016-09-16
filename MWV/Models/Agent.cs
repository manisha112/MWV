using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class Agent
    {
        public Agent()
        {
            this.Customer = new HashSet<Customer>();
            this.Order = new HashSet<Order>();
        }
        [Key]
        public int agent_id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string mobile { get; set; }
        public string landline { get; set; }
        public string address { get; set; }
        public string aspnetusers_id { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string pincode { get; set; }
        public int? credit_days { get; set; }
        public decimal? credit_limit { get; set; }
        [NotMapped]
        public virtual string password { get; set; }
        [NotMapped]
        public virtual string ConfirmPassowrd { get; set; }
        //public virtual ICollection<AspNetUsers> AspNetUsers { get; set; }
        public virtual ICollection<Customer> Customer { get; set; }
        public virtual ICollection<Order> Order { get; set; }
    }
}