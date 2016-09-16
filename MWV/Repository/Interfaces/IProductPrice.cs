using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;

namespace MWV.Repository.Interfaces
{
    interface IProductPrice
    {

        List<Product_prices> GetListOfProductsPrice();
        bool AddProductPrice(int CustomerID, string ProductCode, string ShadeCode, decimal UnitPrice, DateTime StartDate, DateTime EndDate, string created_by);
        bool EditProductPrice(int ProductPriceID, int CustomerID, string ProductCode, string ShadeCode, decimal UnitPrice, DateTime StartDate, DateTime EndDate, string modified_by);
    }
}
