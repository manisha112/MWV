using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;

namespace MWV.Repository.Interfaces
{
    interface ICustomerAdmin
    {
        List<Customer> GetCustomers();
        bool EditCustomer(int CustomerIDToEdit, string name, string email, string phone, string address1, string address2, string address3, string city, string pincode, string state, string country, string fax, string remarks, string modified_by);

        bool DeleteCustomer(int CustomerIDToDel);
    }
}
