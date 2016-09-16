using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MWV.Models;
using System.Web.WebPages.Html;

namespace MWV.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderProducts = new HashSet<Order_Products>();
        }
       

        [Key]
        public int order_id { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = false)]
        public Nullable<DateTime> order_date { get; set; }

        [Display(Name="Agent")]
        public Nullable<int> agent_id { get; set; }

        [Display(Name = "Select Customer")]
        public Nullable<int> customer_id { get; set; }

        public string status { get; set; }

        [Display(Name="Requested Delivery Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> requested_delivery_date { get; set; }


        public string remarks { get; set; }
        public Nullable<decimal> amount { get; set; }
        public Nullable<int> papermill_id { get; set; }

        public Nullable<DateTime> created_on { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_on { get; set; }
        public string modified_by { get; set; }

        // added to the database on 18 June 2015
        public string customerpo { get; set; }
        public string comments { get; set; }
        public string approved_by { get; set; }

        //added on 11 Aug
        public bool? is_deckled { get; set; }
        //added on 28 aug
        public string pdf_file { get; set; }
        //added on 11-09 sept
        public int? rating { get; set; }
        public string rating_remarks { get; set; }
        public DateTime? rated_on { get; set; }
        public string rated_by { get; set; }
        // Navigation Properties
        public virtual Agent Agent { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Papermill papermill { get; set; }

        public virtual ICollection<Order_Products> OrderProducts { get; set; }
        public virtual ICollection<Truck_dispatch_details> Truck_dispatch_details { get; set; }

       ////SearchOrder 
       // public List<SelectListItem> SearchOrder { get; set; }
     

    }
}