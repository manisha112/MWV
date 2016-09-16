using MWV.DBContext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWV.Models
{
    public class TruckTransportation
    {

        public IEnumerable<Papermill> locations { get; set; }

        [DisplayName("Location")]
        public int papermill_id { get; set; }

        [DisplayName("Vehicle Capacity")]
        public string vehicleCapacity { get; set; }

        [DisplayName("Vehicle Number")]
        public int vehicleNumber { get; set; }

        [DisplayName("Start")]
        public DateTime agent_dispatched_on { get; set; }

        [DisplayName("Expected Arrival")]
        public DateTime estimated_arrival { get; set; }

        // Details to enter on add Cargo
        public IEnumerable<Order> order_id { get; set; }

        [DisplayName("Select PurchaseOrder")]
        public int agent_id { get; set; }

        [Remote("GetAllValues", "TruckTransportation", AdditionalFields = "order_id,product_code")]
        [DisplayName("Enter Quantity")]
        public int qty { get; set; }

        public IEnumerable<Order_Products> product_code { get; set; }

        [DisplayName("Select Product")]
        [Required, Range(1, Int32.MaxValue)]
        public int order_product_id { get; set; }

        public List<Order_Products> Order_Products { get; set; }

       
    }
}