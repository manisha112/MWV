using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWV.Repository.Interfaces;
using MWV.DBContext;
using System.Reflection;
using MWV.Models;

namespace MWV.Repository.Implementation
{
    public class FinanceHeadRepository : IFinanceHead
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        #region ApprovedCustList
        // this function returns the Approved customer list
        public List<Customer> CustListApproved()
        {
            try
            {
                var Customers = db.Customers.Where(c => c.status == "Approved").ToList();
                return Customers;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Finance Repository->CustListbyAgentId:", Ex);
                return null;
            }

        }

        #endregion

        #region Get AlphaWise
        public IQueryable GetAlphawiseName()
        {
            try
            {
                var custs = (from p in db.Customers
                             let first = p.name.Substring(0, 1)
                             orderby first
                             select first.ToUpper()).Distinct();
                return custs;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Finance Repository->GetAlphawiseName:", Ex);
                return null;
            }


        }

        #endregion

        #region Update Customer Record
        public bool UpdateOrderProductQty(int CreditDays, decimal CreditLimit, int CustomerID)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    var query = db.Customers.FirstOrDefault(x => x.customer_id == CustomerID);
                    query.credit_days = CreditDays;
                    query.credit_limit = CreditLimit;
                    db.SaveChanges();
                    return true;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Finance Repository->UpdateCustomer:", Ex);
                    return false;
                }
            }

        }
        #endregion

        #region get list of Customer on DDL Changed
        public List<TempPendingApproval> GetListCustomer(int id)
        {
            try
            {
                var query = (from aspntusr in db.AspNetUsers join agnt in db.Agents on aspntusr.Id equals agnt.aspnetusers_id join ct in db.Customers on agnt.agent_id equals ct.agent_id where ct.customer_id == id select new TempPendingApproval { customer_id = ct.customer_id, CustomerName = ct.name, Addess1 = ct.address1, Addess2 = ct.address2, Addess3 = ct.address3, AgentName = agnt.name, CreditLimit = ct.credit_limit, CreditDays = ct.credit_days }).ToList();
                return query;
            }
            catch (Exception Ex)
            {

                logger.Error("Error in Finance Repository->GetListCustomer:", Ex);
                return null;
            }




        }

        #endregion

        #region Get List Alpha Wise

        public List<TempPendingApproval> GetListAlphawise(string alpha)
        {
            try
            {
                var query = (from cust in db.Customers
                             join agent in db.Agents on cust.agent_id equals agent.agent_id
                             where cust.name.Substring(0, 1) == alpha && cust.status == "Approved"
                             select new TempPendingApproval
                             {
                                 CreditLimit = cust.credit_limit,
                                 CreditDays = cust.credit_days,
                                 customer_id = cust.customer_id,
                                 CustomerName = cust.name,
                                 Addess1 = cust.address1,
                                 Addess2 = cust.address2,
                                 Addess3 = cust.address3,
                                 AgentName = agent.name,
                                 City = cust.city,
                                 Phone = cust.phone,
                                 fax = cust.fax,
                                 State = cust.state,
                                 Country = cust.country
                             }).ToList();

                return query;
            }
            catch (Exception Ex)
            {

                logger.Error("Error in Finance Repository->GetListAlphawise:", Ex);
                return null;
            }

        }
        #endregion

        #region Search Customer
        public List<Cust> SearchCustomer(string searchStr)
        {
            try
            {
                List<Cust> lst = (from aspntusr in db.AspNetUsers join agnt in db.Agents on aspntusr.Id equals agnt.aspnetusers_id join c in db.Customers on agnt.agent_id equals c.agent_id where c.status == "Approved" && c.name.Contains(searchStr) select new Cust { label = c.name, value = c.customer_id, address1 = c.address1, city = c.city, state = c.state, pincode = c.pincode, country = c.country, phone = c.phone, fax = c.fax, CreditLimit = c.credit_limit, CreditDays = c.credit_days, agentname = agnt.name }).ToList();
                return lst;
            }
            catch (Exception Ex)
            {

                logger.Error("Error in Finance Repository->SearchCustomer:", Ex);
                return null;
            }
        }
        #endregion

        #region Dispose
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}