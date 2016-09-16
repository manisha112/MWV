using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MWV.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace MWV.Models
{
    public class Customer
    {

        public Customer()
        {
            this.Product_price = new HashSet<Product_prices>();
            this.Order = new HashSet<Order>();
        }


        [Key]
        public int customer_id { get; set; }
        [Required]
        public string name { get; set; }
        public string cust_code { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        [EmailAddress]
        public string email { get; set; }
        public string phone { get; set; }
        public string address1 { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int agent_id { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }

        //[Required]
        [Remote("IsCustomerExist", "Customer", AdditionalFields = "name")]
        public string city { get; set; }

        public string pincode { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string fax { get; set; }
        public string status { get; set; }
        public string aspnetusers_id_approvedby { get; set; }
        public Nullable<System.DateTime> approved_on { get; set; }

        public Nullable<DateTime> created_on { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_on { get; set; }
        public string modified_by { get; set; }
        public string remarks { get; set; }

        public decimal? credit_limit { get; set; }
        public int? credit_days { get; set; }
        public string aspnetuser_id { get; set; }
        public int? login_enabled { get; set; }
        public int? order_enabled { get; set; }
        [NotMapped]
        public virtual string password { get; set; }
        [NotMapped]
        public virtual string ConfirmPassowrd { get; set; }
        public virtual Agent Agent { get; set; }
        public virtual ICollection<Product_prices> Product_price { get; set; }
        public virtual ICollection<Order> Order { get; set; }
    }
}