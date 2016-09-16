using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;

namespace MWV.Repository.Interfaces
{
    interface IProduct
    {
        List<Product> GetListOfProducts();
        bool AddProduct(string ProductCode, string Gsm_Code, string Bf_Code, string ProductDescription, string created_by);
        bool EditProduct(string ProductCode, string Gsm_Code, string Bf_Code, string ProductDescription, string modified_by);
        bool DeleteProduct(string ProductCode);
    }
}
