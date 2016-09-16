using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;


namespace MWV.Models
{
     public enum enumOrderStatus:   int
        {
         
         Created =1,    
            Approved=2, 
            Scheduled=3,
            Loaded=4,   
            PartialLoaded=5, 
            Dispatch=6,  
            Completed=7 
        }
     public  enum enumCustomerStatus
        {
         
            Submitted=1,  // once the agent creates the customer
            Active=2,     // after the production manager approves the custoomer
            Inactive=3,   // no longer used by the system
            Rejected =4   // after the production manager rejects the custoomer
        }
   
}