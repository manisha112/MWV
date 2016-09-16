using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;

namespace MWV.Repository.Interfaces
{
    interface IOrder_product
    {
        int AddOrder_product(Order_Products order_products);
        List<Order_Products> GetOrderProducts(int orderid);
        List<Bf> GetBfs();
        List<Gsm> GetGsms();
    }
}
