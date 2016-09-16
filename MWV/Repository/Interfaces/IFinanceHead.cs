using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;

namespace MWV.Repository.Interfaces
{
    interface IFinanceHead
    {
        List<Customer> CustListApproved();
        IQueryable GetAlphawiseName();
        bool UpdateOrderProductQty(int CreditDays, decimal CreditLimit, int CustomerID);
        List<TempPendingApproval> GetListCustomer(int id);
        List<TempPendingApproval> GetListAlphawise(string alpha);
         
    }
}
