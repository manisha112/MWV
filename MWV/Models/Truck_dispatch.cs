using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MWV.Models
{
    public class Truck_dispatches
    {
        public Truck_dispatches()
        {
            this.Truckdispatchdetails = new HashSet<Truck_dispatch_details>();
        }
        [Key]
        public int truck_dispatch_id { get; set; }

        public int agent_id { get; set; }
        public string truck_no { get; set; }


        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> estimated_arrival { get; set; }
        public Nullable<decimal> estimated_capacity { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> actual_arrival_at_gate { get; set; }

        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> agent_dispatched_on { get; set; }

        public string status { get; set; }
        public Nullable<DateTime> left_factory_on { get; set; }
        public Nullable<decimal> loaded_capacity { get; set; }
        

        [ScaffoldColumn(false)]
        public DateTime created_on { get; set; }
        
        [ScaffoldColumn(false)]
        public string created_by { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? modified_on { get; set; }

        [ScaffoldColumn(false)]
        public string modified_by { get; set; }

        public virtual Agent agentdetails { get; set; }
        public virtual ICollection<Truck_dispatch_details> Truckdispatchdetails { get; set; }

        public string inward_by { get; set; }
        public string outward_by { get; set; }
        public string loaded_by { get; set; }
    }
}