using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MWV.Models
{
    public class ShowOrderViewModel
    {
        public List<Order> OrderDetails {get;set;}
        public List<Order_Products> OrderProductDetails {get;set;}
        
    }
}