using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc.Html;
using System.Web.WebPages.Html;

namespace MWV.Models
{
    public class Order_Products
    {
        [Key]
        public int order_product_id { get; set; }
        public int order_id { get; set; }
        public string product_code { get; set; }

        [Display(Name = "Shade")]
        public string shade_code { get; set; }
        public Nullable<decimal> width { get; set; }
        public Nullable<decimal> unit_price { get; set; }

        [Display(Name = "Quantity")]
        public Nullable<decimal> qty { get; set; }

        [Display(Name = "Amount")]
        public Nullable<decimal> amount { get; set; }
        public string status { get; set; }

        public Nullable<decimal> width_planned { get; set; }
        public Nullable<decimal> qty_planned_byAgent { get; set; }
        public Nullable<decimal> diameter { get; set; }
        public Nullable<int> core { get; set; }


        public Nullable<DateTime> created_on { get; set; }
        public string created_by { get; set; }
        public Nullable<DateTime> modified_on { get; set; }
        public string modified_by { get; set; }
        public Nullable<decimal> qty_scheduled { get; set; }
        public Nullable<decimal> qty_dispatched_byFactory { get; set; }

        public DateTime? requested_delivery_date { get; set; }
        public int? schedule_id { get; set; }

        // Navigation Properties
        public virtual Order order { get; set; }
        public virtual Product Product { get; set; }
        public virtual Shade Shade { get; set; }

        // by RK Temp Fields for Deckle calculations.. Not part of Model
        [NotMapped]
        public decimal QtyPending { get; set; }
        [NotMapped]
        public decimal SizePending { get; set; }
        [NotMapped]
        public DateTime RequestedDate { get; set; }
        [NotMapped]
        public int deckleCustomerId { get; set; }
        [NotMapped]
        public string deckleBFCode { get; set; }
        [NotMapped]
        public string deckleGSMCode { get; set; }
        [NotMapped]
        public string description { get; set; }

        [NotMapped]
        public decimal? dispatched { get; set; }

        [NotMapped]
        public string truck_no { get; set; }

        [NotMapped]
        public int agentPO { get; set; }
        [NotMapped]
        public DateTime? agentPOdt { get; set; }

        [NotMapped]
        public string brandName { get; set; }

        [NotMapped]
        public string customerPO { get; set; }


        [NotMapped]
        public string bf { get; set; }
        [NotMapped]
        public string gsm { get; set; }
        [NotMapped]
        public string customerName { get; set; }
        [NotMapped]
        public decimal? underplanningStatus { get; set; }
        [NotMapped]
        public decimal? plannedStatus { get; set; }
        [NotMapped]
        public decimal? FGStatus { get; set; }

        [NotMapped]
        public DateTime? customerPodt { get; set; }
        [NotMapped]
        public string itemNo { get; set; }
        [NotMapped]
        public decimal? basicprice { get; set; }

        [NotMapped]
        public string agentName { get; set; }

        [NotMapped]
        public DateTime? actualDeliveryDate { get; set; }
        [NotMapped]
        public DateTime? dispatchedOnFactory { get; set; }

        [NotMapped]
        public string machineName { get; set; }
        

                [NotMapped]
        public string Variant { get; set; }
    }
}