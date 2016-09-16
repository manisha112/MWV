using MWV.DBContext;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MWV.Models
{
    public class TransportationDetails
    {
        MWVDBContext db = new MWVDBContext();

        public IEnumerable<Papermill> locations { get; set; }
        [DisplayName("Location")]
        public int papermill_id { get; set; }

        [DisplayName("Vehicle Capacity")]
        public string  vehicleCapacity { get; set; }

        [DisplayName("Vehicle Number")]
        public int vehicleNumber { get; set; }

        [DisplayName("Start")]
        public DateTime actual_arrival_at_gate { get; set; }
        
        [DisplayName("Expected Arrival")]
        public DateTime estimated_arrival { get; set; }

        // Details to enter on add Cargo
        public IEnumerable<Order> order_id { get; set; }
        [DisplayName("Select PurchaseOrder")]
        public int agent_id { get; set; }

        [DisplayName("Enter Quantity")]
        public int qty { get; set; }

        public IEnumerable<Order_Products> product_code { get; set; }
        [DisplayName("Select Product")]
        public int order_product_id { get; set; }

        

        public void FetchTransportationDetails()
        {
             locations = db.Papermills.AsEnumerable<Papermill>();
        }
        public void FetchPurchaseOrders()
        {
            order_id = db.Orders.AsEnumerable<Order>();
        }
        public void FetchOrderProductDetails()
        {
            product_code = db.Order_products.AsEnumerable<Order_Products>();
        }
    }
}