using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class QuickViewModel
    {
        public List<Customer> CustomerDetails { get; set; }
        public List<Order> OrderDetails { get; set; }
        public List<Truck_dispatches> Truck_dispatches { get; set; }
        public List<Truck_dispatch_details> Truck_dispatch_details { get; set; }
        public List<Order_Products> Order_Products { get; set; }
        public List<Papermill> papermill { get; set; }

    }

    public class TempTruckDetails
    {
        public int papermill_id { get; set; }
        public int truck_dispatch_id { get; set; }
        public string truck_no { get; set; }
        public string location { get; set; }
        public int pmlocationid { get; set; }
        public string address { get; set; }
        public DateTime created_on { get; set; }
        public decimal? estimated_capacity { get; set; }
        public DateTime? agent_dispatched_on { get; set; }
        public DateTime? estimated_arrival { get; set; }
        public DateTime? actual_arrival_at_gate { get; set; }
        public DateTime? left_factory_on { get; set; }

        // for use on truck dispatch repository
        public string agentname { get; set; }
        public int agent_id { get; set; }

        public string status { get; set; }
        public int? millId { get; set; }
        public Nullable<decimal> loaded_capacity { get; set; }

        public virtual Agent agentdetails { get; set; }
        public virtual ICollection<Truck_dispatch_details> Truckdispatchdetails { get; set; }

        public string inward_by { get; set; }
        public string outward_by { get; set; }
        public string loaded_by { get; set; }
    }
}