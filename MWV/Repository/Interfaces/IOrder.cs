using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;
using MWV.Repository.Implementation;


namespace MWV.Repository.Interfaces
{
    interface IOrder
    {
        int AddOrder(OrderRepository.tempOrder temporder, List<OrderRepository.tempOrderProducts> lstTempProds);
        List<Order> OrdersSearchResults(string SelectedStatusValue, DateTime fromDate, DateTime toDate);
        
    }
}
