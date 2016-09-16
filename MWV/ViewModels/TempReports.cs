using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using MWV.Models;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MWV.ViewModels
{
    public class TempReports
    {
        public string stateName { get; set; }
        public DateTime? orderloggingdt { get; set; }

        public decimal? amount { get; set; }
        public string brandOwner { get; set; }
        public string agentname { get; set; }
        public string description { get; set; }
        public string requested_delivery_date { get; set; }
        public decimal? dispatched { get; set; }
        public string truck_no { get; set; }
        public int agentPO { get; set; }
        public DateTime? agentPOdt { get; set; }
        public string brandName { get; set; }
        public string customerPO { get; set; }
        public string bf { get; set; }
        public string gsm { get; set; }
        public string customerName { get; set; }
        public DateTime? customerPodt { get; set; }
        public string itemNo { get; set; }
        public decimal? basicprice { get; set; }
        public string agentName { get; set; }
        public string actualDeliveryDate { get; set; }
        public DateTime? dispatchedOnFactory { get; set; }
        public string machineName { get; set; }
        public int order_product_id { get; set; }
        public string shade_code { get; set; }
        public decimal? qty { get; set; }
        public decimal? width { get; set; }
        public string Variant { get; set; }
        public decimal? diameter { get; set; }
        public decimal? underplanningQty { get; set; }
        public decimal? plannedQty { get; set; }
        public decimal? FGqty { get; set; }
        public decimal? dispatchedQty { get; set; }
        public decimal? unit_price { get; set; }
        public decimal? actualProducedQty { get; set; }
        public decimal? actualLoadedQty { get; set; }
        public decimal? schduledQty { get; set; }

    }
}
