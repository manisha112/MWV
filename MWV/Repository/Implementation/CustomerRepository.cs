using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.WebPages.Razor;
using Microsoft.AspNet.Identity;
using MWV.Models;
using MWV.DBContext;
using MWV.Repository.Interfaces;
using System.Reflection;
using System.Text.RegularExpressions;
using MWV.ViewModels;


namespace MWV.Repository.Implementation
{
    public class Cust
    {
        public string label { get; set; }
        public Int64? value { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string address1 { get; set; }
        public string address2 { get; set; }
        public string address3 { get; set; }
        public string city { get; set; }
        public string pincode { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string fax { get; set; }
        public decimal? CreditLimit { get; set; }
        public string agentname { get; set; }
        public int? CreditDays { get; set; }
        public string ValueForCust { get; set; }

    }


    public class CustomerRepository : ICustomer
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        #region GetCustNameByCustId
        // this function returns the customer list filtered by Agent id
        public string GetCustNameByCustId(int custid)
        {

            try
            {
                var Customers = db.Customers.Where(c => c.customer_id == custid).Select(l => l.name).SingleOrDefault();

                return Customers.ToString();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerRepository->GetCustNameByCustId:", Ex);
                return null;
            }

        }
        #endregion

        #region CustListbyAgentId
        // this function returns the customer list filtered by Agent id
        public List<Customer> CustListbyAgentId()
        {
            OrderRepository objOrderRep = new OrderRepository();

            try
            {
                int LoggedInAgent_id = objOrderRep.GetAgentID();

                var Customers = db.Customers.Where(c => c.agent_id == LoggedInAgent_id
                          && c.status == "Approved").OrderBy(x => x.name).ToList();

                return Customers;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerRepository->CustListbyAgentId:", Ex);
                return null;
            }

        }
        #endregion

        #region CreateCustomer
        // this function returns the customer list filtered by Agent id
        public Boolean CreateCustomer(Customer customer)
        {

            try
            {
                customer.status = "Created";
                db.Customers.Add(customer);
                db.SaveChanges();

                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerRepository->CreateCustomer:", Ex);
                return false;
            }

        }
        #endregion

        #region CustSearchResult
        // this function returns the customer list filtered by Agent id and the search string for the autocomplete feature
        public List<Customer> CustSearchResult(int agentid, string searchStr)
        {
            try
            {
                //  int LoggedInAgent_id = GetAgentID();
                var customer_search_list = db.Customers.Where(b => b.agent_id == agentid && b.status == "Approved" && b.name.Contains(searchStr));
                //var cus = customer_search_list.Select(x => new { x.name, x.address1, x.city, x.country }).ToList();
                //List<Customer> c =(List<Customer>)db.Customers.Select(e => new { e.name, e.address1, e.city });
                //  return c;
                return customer_search_list.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerRepository->CustSearchResult:", Ex);
                return null;
            }
        }
        #endregion

        public string GetProductionPlannerName()
        {
            var plannerDetails = (from role in db.AspNetUsers
                                  join netRole in db.AspNetUserRoles on role.Id equals netRole.UserId
                                  join user in db.AspNetRoles on netRole.RoleId equals user.Id
                                  where user.Name == "ProductionPlanner"
                                  select role).SingleOrDefault();

            return plannerDetails.name;

        }


        public string GetEmail()
        {
            var plannerDetails = (from role in db.AspNetUsers
                                  join netRole in db.AspNetUserRoles on role.Id equals netRole.UserId
                                  join user in db.AspNetRoles on netRole.RoleId equals user.Id
                                  where user.Name == "ProductionPlanner"
                                  select role).SingleOrDefault();

            return plannerDetails.Email;
        }

        public string GetAgentmail(string id)
        {
            string Agntemail = (from ag in db.Agents where ag.agent_id.ToString() == id select ag.email).SingleOrDefault();
            return Agntemail;
        }

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

        public List<Order> OrdersSearchResultsByCustomerPO(string allcustPoNo)
        {
            OrderRepository ordRep = new OrderRepository();
            int LoggedInAgentId = ordRep.GetAgentID();

            try
            {
                List<Order> lstOrder = new List<Order>();
                string[] strCheckedValue = Regex.Split(allcustPoNo, ",").ToArray();

                lstOrder = (from d in db.Orders
                            where strCheckedValue.Contains(d.customerpo)
                            // && d.CustomerId == LoggedInCustomerId
                            orderby d.order_date descending
                            select d).ToList<Order>();

                return lstOrder.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Order->OrdersSearchResults:", Ex);
                return null;
            }

        }

        public List<TempReports> AgentReport(int agentId, string status, DateTime fromdt, DateTime todt)
        {
            try
            {
                List<TempReports> orderList = new List<TempReports>();

                if (agentId == 0 && (status == "" || status == null))
                {
                    orderList = AgentReportforAll(fromdt, todt);// get data of all agent with all status type
                }
                else if (agentId != 0 && (status == "" || status == null))
                {
                    orderList = AgentReportByAgentIdWithAllStatus(agentId, fromdt, todt); // get record agent wise  with all status
                }
                else if (agentId != 0 && (status != "" || status != null))
                {
                    orderList = AgentReportByAgentIdWithStatusWise(agentId, status, fromdt, todt);// get record of each agent and status wise
                }
                else if (agentId == 0 && (status != "" || status != null))
                {
                    orderList = AllAgentReportbyEachStatus(status, fromdt, todt);// get record of all agent with each status wise
                }

                return orderList;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Order->OrdersSearchResults:", Ex);
                return null;
            }

        }

        // report for all agent with all status from, start date to end date
        private List<TempReports> AgentReportforAll(DateTime fromdt, DateTime todt)
        {
            try
            {
                var orderList = (from ord in db.Orders
                                 join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                                 join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                                 join pro in db.Products on ordPro.product_code equals pro.product_code
                                 join cust in db.Customers on ord.customer_id equals cust.customer_id
                                 join agent in db.Agents on ord.agent_id equals agent.agent_id
                                 join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                                 where (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                                 select new TempReports
                                 {
                                     customerPO = ord.customerpo,
                                     customerPodt = ord.order_date,
                                     agentPO = ord.order_id,
                                     agentPOdt = ord.order_date,
                                     itemNo = "",
                                     customerName = cust.name,
                                     brandName = "",
                                     machineName = papermill.name,
                                     bf = pro.bf_code,
                                     gsm = pro.gsm_code,
                                     shade_code = ordPro.shade_code,
                                     qty = ordPro.qty,//qty is same as weight
                                     width = ordPro.width,
                                     diameter = ordPro.diameter,
                                     description = pro.description,
                                     basicprice = ordPro.unit_price,
                                     underplanningQty = (decimal?)ordPro.qty - (decimal?)ordPro.qty_scheduled ?? 0,
                                     actualLoadedQty = 0,
                                     actualProducedQty = 0,
                                     truck_no = "",
                                     agentName = agent.name,
                                     dispatchedOnFactory = null,//date of dispatched
                                     Variant = "",
                                     order_product_id = ordPro.order_product_id,
                                     schduledQty = (decimal?)ordPro.qty_scheduled
                                 })
                    .ToList();
                List<TempReports> reportObj = new List<TempReports>();
                foreach (var items in orderList)
                {
                    TempReports obj = new TempReports();
                    var actualPcQty = db.ProductionChild.Where(x => x.actual_end != null
                                && x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty);
                    var actualLoadedQty = db.Truck_dispatch_details.
                                          Where(x => x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty_loaded);

                    var truckNos = from tdd in db.Truck_dispatch_details
                                   join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                                   where tdd.order_product_id == items.order_product_id
                                  && (td.status == "Dispatched" || td.status == "Completed")
                                   select td;
                    string truckList = string.Empty;

                    foreach (var item in truckNos)
                    {
                        DateTime dt = new DateTime();
                        Boolean flag = false;
                        if (item.left_factory_on == null)
                        {
                            flag = false;
                        }
                        else
                        {
                            dt = (DateTime)item.left_factory_on;
                            flag = true;
                        }
                        if (truckList != "")
                        {
                            if (flag == true)
                            {
                                truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                            }
                            else
                            {
                                truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                            }
                        }
                        else
                        {
                            if (flag == true)
                            {
                                truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                            }
                            else
                                if (flag == false)
                                {
                                    truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                                }

                        }
                    }
                    obj.customerPO = items.customerPO;
                    obj.customerPodt = items.customerPodt;
                    obj.agentPO = items.agentPO;
                    obj.agentPOdt = items.agentPOdt;
                    obj.itemNo = "";
                    obj.customerName = items.customerName;
                    obj.brandName = "";
                    obj.machineName = items.machineName;
                    obj.bf = items.bf;
                    obj.gsm = items.gsm;
                    obj.shade_code = items.shade_code;
                    obj.qty = items.qty;//qty is same as weight
                    obj.width = items.width;
                    obj.diameter = items.diameter;
                    obj.description = items.description;
                    obj.underplanningQty = (decimal?)items.underplanningQty ?? 0;
                    obj.dispatchedQty = (decimal?)actualLoadedQty ?? 0;
                    obj.actualProducedQty = (decimal?)actualPcQty ?? 0;
                    obj.plannedQty = ((decimal?)items.schduledQty ?? 0) - ((decimal?)actualPcQty ?? 0);
                    obj.FGqty = ((decimal?)actualPcQty ?? 0) - ((decimal?)actualLoadedQty ?? 0);
                    obj.unit_price = (decimal?)items.unit_price ?? 0;
                    obj.truck_no = truckList;
                    obj.agentName = items.agentName;
                    obj.dispatchedOnFactory = DateTime.MinValue;
                    obj.Variant = "";
                    obj.order_product_id = items.order_product_id;
                    reportObj.Add(obj);
                }
                return reportObj;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Customer->AgentReportforAll:", Ex);
                return null;
            }

        }

        // report by agentid with all status from, start date to end date
        private List<TempReports> AgentReportByAgentIdWithAllStatus(int agentId, DateTime fromdt, DateTime todt)
        {
            try
            {
                var orderList = (from ord in db.Orders
                                 join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                                 join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                                 join pro in db.Products on ordPro.product_code equals pro.product_code
                                 join cust in db.Customers on ord.customer_id equals cust.customer_id
                                 join agent in db.Agents on ord.agent_id equals agent.agent_id
                                 join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id

                                 where (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                                 select new TempReports
                                                {
                                                    customerPO = ord.customerpo,
                                                    customerPodt = ord.order_date,
                                                    agentPO = ord.order_id,
                                                    agentPOdt = ord.order_date,
                                                    itemNo = "",
                                                    customerName = cust.name,
                                                    brandName = "",
                                                    machineName = papermill.name,
                                                    bf = pro.bf_code,
                                                    gsm = pro.gsm_code,
                                                    shade_code = ordPro.shade_code,
                                                    qty = ordPro.qty,//qty is same as weight
                                                    width = ordPro.width,
                                                    diameter = ordPro.diameter,
                                                    description = pro.description,
                                                    basicprice = ordPro.unit_price,
                                                    underplanningQty = (decimal?)ordPro.qty - (decimal?)ordPro.qty_scheduled ?? 0,
                                                    actualLoadedQty = 0,
                                                    actualProducedQty = 0,
                                                    truck_no = "",
                                                    agentName = agent.name,
                                                    dispatchedOnFactory = null,//date of dispatched
                                                    Variant = "",
                                                    order_product_id = ordPro.order_product_id,
                                                    schduledQty = (decimal?)ordPro.qty_scheduled
                                                })
                    .ToList();
                List<TempReports> reportObj = new List<TempReports>();
                foreach (var items in orderList)
                {
                    TempReports obj = new TempReports();
                    var actualPcQty = db.ProductionChild.Where(x => x.actual_end != null
                                && x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty);
                    var actualLoadedQty = db.Truck_dispatch_details.
                                          Where(x => x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty_loaded);

                    var truckNos = from tdd in db.Truck_dispatch_details
                                   join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                                   where tdd.order_product_id == items.order_product_id
                                  && (td.status == "Dispatched" || td.status == "Completed")
                                   select td;
                    string truckList = string.Empty;

                    foreach (var item in truckNos)
                    {
                        DateTime dt = new DateTime();
                        Boolean flag = false;
                        if (item.left_factory_on == null)
                        {
                            flag = false;
                        }
                        else
                        {
                            dt = (DateTime)item.left_factory_on;
                            flag = true;
                        }
                        if (truckList != "")
                        {
                            if (flag == true)
                            {
                                truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                            }
                            else
                            {
                                truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                            }
                        }
                        else
                        {
                            if (flag == true)
                            {
                                truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                            }
                            else
                                if (flag == false)
                                {
                                    truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                                }

                        }
                    }
                    obj.customerPO = items.customerPO;
                    obj.customerPodt = items.customerPodt;
                    obj.agentPO = items.agentPO;
                    obj.agentPOdt = items.agentPOdt;
                    obj.itemNo = "";
                    obj.customerName = items.customerName;
                    obj.brandName = "";
                    obj.machineName = items.machineName;
                    obj.bf = items.bf;
                    obj.gsm = items.gsm;
                    obj.shade_code = items.shade_code;
                    obj.qty = items.qty;//qty is same as weight
                    obj.width = items.width;
                    obj.diameter = items.diameter;
                    obj.description = items.description;
                    obj.underplanningQty = (decimal?)items.underplanningQty ?? 0;
                    obj.dispatchedQty = (decimal?)actualLoadedQty ?? 0;
                    obj.actualProducedQty = (decimal?)actualPcQty ?? 0;
                    obj.plannedQty = ((decimal?)items.schduledQty ?? 0) - ((decimal?)actualPcQty ?? 0);
                    obj.FGqty = ((decimal?)actualPcQty ?? 0) - ((decimal?)actualLoadedQty ?? 0);
                    obj.unit_price = (decimal?)items.basicprice ?? 0;
                    obj.truck_no = truckList;
                    obj.agentName = items.agentName;
                    obj.dispatchedOnFactory = DateTime.MinValue;
                    obj.Variant = "";
                    obj.order_product_id = items.order_product_id;
                    reportObj.Add(obj);
                }
                return reportObj;

            }
            catch (Exception Ex)
            {
                logger.Error("Error in Customer->AgentReportByAgentIdWithAllStatus:", Ex);
                return null;
            }

        }

        // report by agentid with each status from, start date to end date
        private List<TempReports> AgentReportByAgentIdWithStatusWise(int agentId, string status, DateTime fromdt, DateTime todt)
        {
            try
            {
                // string[] selectedStatus = Regex.Split(status, ",").ToArray();

                var orderList = (from ord in db.Orders
                                 join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                                 join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                                 join pro in db.Products on ordPro.product_code equals pro.product_code
                                 join cust in db.Customers on ord.customer_id equals cust.customer_id
                                 join agent in db.Agents on ord.agent_id equals agent.agent_id
                                 join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                                 where ord.agent_id == agentId
                                 && (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                                 select new TempReports
                                 {
                                     customerPO = ord.customerpo,
                                     customerPodt = ord.order_date,
                                     agentPO = ord.order_id,
                                     agentPOdt = ord.order_date,
                                     itemNo = "",
                                     customerName = cust.name,
                                     brandName = "",
                                     machineName = papermill.name,
                                     bf = pro.bf_code,
                                     gsm = pro.gsm_code,
                                     shade_code = ordPro.shade_code,
                                     qty = ordPro.qty,//qty is same as weight
                                     width = ordPro.width,
                                     diameter = ordPro.diameter,
                                     description = pro.description,
                                     basicprice = ordPro.unit_price,
                                     underplanningQty = (decimal?)ordPro.qty - (decimal?)ordPro.qty_scheduled ?? 0,
                                     actualLoadedQty = 0,
                                     actualProducedQty = 0,
                                     truck_no = "",
                                     agentName = agent.name,
                                     dispatchedOnFactory = null,//date of dispatched
                                     Variant = "",
                                     order_product_id = ordPro.order_product_id,
                                     schduledQty = (decimal?)ordPro.qty_scheduled
                                 })
                    //.GroupBy(x => x.order_product_id)
                    // .AsEnumerable()
                    .ToList();
                List<TempReports> reportObj = new List<TempReports>();
                foreach (var items in orderList)
                {
                    TempReports obj = new TempReports();
                    var actualPcQty = db.ProductionChild.Where(x => x.actual_end != null
                                && x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty);
                    var actualLoadedQty = db.Truck_dispatch_details.
                                          Where(x => x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty_loaded);

                    var truckNos = from tdd in db.Truck_dispatch_details
                                   join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                                   where tdd.order_product_id == items.order_product_id
                                  && (td.status == "Dispatched" || td.status == "Completed")
                                   select td;
                    string truckList = string.Empty;

                    foreach (var item in truckNos)
                    {
                        DateTime dt = new DateTime();
                        Boolean flag = false;
                        if (item.left_factory_on == null)
                        {
                            flag = false;
                        }
                        else
                        {
                            dt = (DateTime)item.left_factory_on;
                            flag = true;
                        }
                        if (truckList != "")
                        {
                            if (flag == true)
                            {
                                truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                            }
                            else
                            {
                                truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                            }
                        }
                        else
                        {
                            if (flag == true)
                            {
                                truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                            }
                            else
                                if (flag == false)
                                {
                                    truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                                }

                        }
                    }
                    obj.customerPO = items.customerPO;
                    obj.customerPodt = items.customerPodt;
                    obj.agentPO = items.agentPO;
                    obj.agentPOdt = items.agentPOdt;
                    obj.itemNo = "";
                    obj.customerName = items.customerName;
                    obj.brandName = "";
                    obj.machineName = items.machineName;
                    obj.bf = items.bf;
                    obj.gsm = items.gsm;
                    obj.shade_code = items.shade_code;
                    obj.qty = items.qty;//qty is same as weight
                    obj.width = items.width;
                    obj.diameter = items.diameter;
                    obj.description = items.description;
                    obj.underplanningQty = (decimal?)items.underplanningQty ?? 0;
                    obj.dispatchedQty = (decimal?)actualLoadedQty ?? 0;
                    obj.actualProducedQty = (decimal?)actualPcQty ?? 0;
                    obj.plannedQty = ((decimal?)items.schduledQty ?? 0) - ((decimal?)actualPcQty ?? 0);
                    obj.FGqty = ((decimal?)actualPcQty ?? 0) - ((decimal?)actualLoadedQty ?? 0);
                    obj.unit_price = (decimal?)items.basicprice ?? 0;
                    obj.truck_no = truckList;
                    obj.agentName = items.agentName;
                    obj.dispatchedOnFactory = DateTime.MinValue;
                    obj.Variant = "";
                    obj.order_product_id = items.order_product_id;
                    reportObj.Add(obj);
                }

                // List<dynamic> dynamicList = orderList.Select(x => (dynamic)x).ToList();
                return reportObj;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Order->OrdersSearchResults:", Ex);
                return null;
            }

        }

        // report for all agent with each status from, start date to end date
        private List<TempReports> AllAgentReportbyEachStatus(string status, DateTime fromdt, DateTime todt)
        {
            try
            {

                string[] selectedStatus = Regex.Split(status, ",").ToArray();
                List<TempReports> orderList = (from ord in db.Orders
                                               join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                                               join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                                               join pro in db.Products on ordPro.product_code equals pro.product_code
                                               join cust in db.Customers on ord.customer_id equals cust.customer_id
                                               join agent in db.Agents on ord.agent_id equals agent.agent_id
                                               join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                                               where (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                                               select new TempReports
                                               {
                                                   customerPO = ord.customerpo,
                                                   customerPodt = ord.order_date,
                                                   agentPO = ord.order_id,
                                                   agentPOdt = ord.order_date,
                                                   itemNo = "",
                                                   customerName = cust.name,
                                                   brandName = "",
                                                   machineName = papermill.name,
                                                   bf = pro.bf_code,
                                                   gsm = pro.gsm_code,
                                                   shade_code = ordPro.shade_code,
                                                   qty = ordPro.qty,//qty is same as weight
                                                   width = ordPro.width,
                                                   diameter = ordPro.diameter,
                                                   description = pro.description,
                                                   basicprice = ordPro.unit_price,
                                                   underplanningQty = (decimal?)ordPro.qty - (decimal?)ordPro.qty_scheduled ?? 0,
                                                   actualLoadedQty = 0,
                                                   actualProducedQty = 0,
                                                   truck_no = "",
                                                   agentName = agent.name,
                                                   dispatchedOnFactory = null,//date of dispatched
                                                   Variant = "",
                                                   order_product_id = ordPro.order_product_id,
                                                   schduledQty = (decimal?)ordPro.qty_scheduled
                                               })
                    .ToList();
                List<TempReports> reportObj = new List<TempReports>();
                foreach (var items in orderList)
                {
                    TempReports obj = new TempReports();
                    var actualPcQty = db.ProductionChild.Where(x => x.actual_end != null
                                && x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty);
                    var actualLoadedQty = db.Truck_dispatch_details.
                                          Where(x => x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty_loaded);

                    var truckNos = from tdd in db.Truck_dispatch_details
                                   join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                                   where tdd.order_product_id == items.order_product_id
                                   && (td.status == "Dispatched" || td.status == "Completed")
                                   select td;
                    string truckList = string.Empty;

                    foreach (var item in truckNos)
                    {
                        DateTime dt = new DateTime();
                        Boolean flag = false;
                        if (item.left_factory_on == null)
                        {
                            flag = false;
                        }
                        else
                        {
                            dt = (DateTime)item.left_factory_on;
                            flag = true;
                        }
                        if (truckList != "")
                        {
                            if (flag == true)
                            {
                                truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                            }
                            else
                            {
                                truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                            }
                        }
                        else
                        {
                            if (flag == true)
                            {
                                truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                            }
                            else
                                if (flag == false)
                                {
                                    truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                                }

                        }
                    }
                    obj.customerPO = items.customerPO;
                    obj.customerPodt = items.customerPodt;
                    obj.agentPO = items.agentPO;
                    obj.agentPOdt = items.agentPOdt;
                    obj.itemNo = "";
                    obj.customerName = items.customerName;
                    obj.brandName = "";
                    obj.machineName = items.machineName;
                    obj.bf = items.bf;
                    obj.gsm = items.gsm;
                    obj.shade_code = items.shade_code;
                    obj.qty = items.qty;//qty is same as weight
                    obj.width = items.width;
                    obj.diameter = items.diameter;
                    obj.description = items.description;
                    obj.underplanningQty = (decimal?)items.underplanningQty ?? 0;
                    obj.dispatchedQty = (decimal?)actualLoadedQty ?? 0;
                    obj.actualProducedQty = (decimal?)actualPcQty ?? 0;
                    obj.plannedQty = ((decimal?)items.schduledQty ?? 0) - ((decimal?)actualPcQty ?? 0);
                    obj.FGqty = ((decimal?)actualPcQty ?? 0) - ((decimal?)actualLoadedQty ?? 0);
                    obj.unit_price = (decimal?)items.basicprice ?? 0;
                    obj.truck_no = truckList;
                    obj.agentName = items.agentName;
                    obj.dispatchedOnFactory = DateTime.MinValue;
                    obj.Variant = "";
                    obj.order_product_id = items.order_product_id;
                    reportObj.Add(obj);
                }

                // List<dynamic> dynamicList = orderList.Select(x => (dynamic)x).ToList();
                return reportObj;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Order->OrdersSearchResults:", Ex);
                return null;
            }

        }



        public List<TempReports> BillofDateReport(string type, int agentid, int customerId, int papermillId, DateTime fromdt, DateTime todt)
        {
            try
            {
                List<TempReports> orderList = new List<TempReports>();
                switch (type)
                {
                    case "by-customer":
                        if (customerId == 0 && papermillId == 0)
                        { orderList = BDreportforAllCustomer(fromdt, todt); }
                        else if (customerId != 0 && papermillId == 0)
                        { orderList = BDreportOfEachCustomer(customerId, fromdt, todt); }
                        else if (customerId != 0 && papermillId != 0)
                        { orderList = BDreportOfEachCustomerByMillId(papermillId, customerId, fromdt, todt); }
                        else if (customerId == 0 && papermillId != 0)
                        { orderList = BDreportByPaperMillId(papermillId, fromdt, todt); }
                        break;

                    case "by-agent":
                        if (agentid == 0 && papermillId == 0)
                        { orderList = BDreportOfAllAgent(fromdt, todt); }
                        else if (agentid != 0 && papermillId == 0)
                        { orderList = BDreportOfEachAgent(agentid, fromdt, todt); }
                        else if (agentid != 0 && papermillId != 0)
                        { orderList = BDreportOfEachAgentByPapermill(papermillId, agentid, fromdt, todt); }
                        else if (agentid == 0 && papermillId != 0)
                        { orderList = BDreportByPaperMillId(papermillId, fromdt, todt); }
                        break;

                }
                return orderList;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Order->OrdersSearchResults:", Ex);
                return null;
            }

        }
        private List<TempReports> BDreportforAllCustomer(DateTime? fromdt, DateTime? todt)
        {
            var OrderList = (from ord in db.Orders
                             join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                             join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                             join pro in db.Products on ordPro.product_code equals pro.product_code
                             join cust in db.Customers on ord.customer_id equals cust.customer_id
                             join agent in db.Agents on ord.agent_id equals agent.agent_id
                             join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                             // join tdd in db.Truck_dispatch_details on ordPro.order_id equals tdd.order_id
                             // join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                             where (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                             select new TempReports
                             {
                                 customerPO = ord.customerpo,
                                 agentPO = ord.order_id,
                                 itemNo = "",
                                 agentPOdt = ord.order_date,
                                 customerName = cust.name,
                                 agentName = agent.name,
                                 brandName = "",
                                 machineName = papermill.name,
                                 bf = pro.bf_code,
                                 gsm = pro.gsm_code,
                                 shade_code = ordPro.shade_code,
                                 qty = ordPro.qty,
                                 width = ordPro.width,
                                 diameter = ordPro.diameter,
                                 description = pro.description,
                                 unit_price = ordPro.unit_price ?? 0,// unitprice is same as rate
                                 amount = ordPro.amount,// amount is same as bill amount
                                 requested_delivery_date = ordPro.requested_delivery_date.ToString(),
                                 // actualDeliveryDate = td.left_factory_on,
                                 // truck_no = td.truck_no,
                                 stateName = "",
                                 // order_id = ord.order_id,//deliveryOrderNo is same as order_id
                                 // dispatchedOnFactory = td.left_factory_on,
                                 Variant = "",
                                 order_product_id = ordPro.order_product_id
                                 //excelFileName = "AllCustomer"
                             }).ToList();
            List<TempReports> reportObj = new List<TempReports>();
            foreach (var items in OrderList)
            {
                TempReports obj = new TempReports();

                var truckNos = from tdd in db.Truck_dispatch_details
                               join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                               where tdd.order_product_id == items.order_product_id
                              && (td.status == "Dispatched" || td.status == "Completed")
                               select td;
                string truckList = string.Empty;

                foreach (var item in truckNos)
                {
                    DateTime dt = new DateTime();
                    Boolean flag = false;
                    if (item.left_factory_on == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        dt = (DateTime)item.left_factory_on;
                        flag = true;
                    }
                    if (truckList != "")
                    {
                        if (flag == true)
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                        }
                    }
                    else
                    {
                        if (flag == true)
                        {
                            truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                        }

                    }
                    DateTime rqdt = Convert.ToDateTime(items.requested_delivery_date);
                    DateTime acdt = new DateTime();
                if (truckNos.Count() != 0)
                {
                    // var query = (from d in truckNos select d.left_factory_on).Max();
                    DateTime? acdt1 = (DateTime?)truckNos.Where(p => p != null).Max(p => p.left_factory_on);
                    if (acdt1 != null)
                    {
                        acdt = (DateTime)acdt1;
                    }
                    else
                    { }
                }
                    obj.customerPO = items.customerPO;
                    obj.agentPO = items.agentPO;
                    obj.itemNo = "";
                    obj.customerName = items.customerName;
                    obj.brandOwner = "";
                    obj.bf = items.bf;
                    obj.gsm = items.gsm;
                    obj.shade_code = items.shade_code;
                    obj.qty = items.qty;//qty is same as weight
                    obj.width = items.width;
                    obj.diameter = items.diameter;
                    obj.description = items.description;
                    obj.unit_price = (decimal?)items.unit_price ?? 0;
                    obj.truck_no = truckList;
                    obj.agentName = items.agentName;
                    obj.requested_delivery_date = rqdt.ToShortDateString();
                    obj.Variant = "";
                    obj.order_product_id = items.order_product_id;
                    obj.stateName = items.stateName;
                    obj.machineName = items.machineName;
                    obj.actualDeliveryDate = acdt.ToShortDateString();
                    obj.amount = items.amount;
                    obj.agentPOdt = items.agentPOdt;
                    reportObj.Add(obj);
                }
            }
            return reportObj;

        }
        private List<TempReports> BDreportOfEachCustomer(int customerId, DateTime fromdt, DateTime todt)
        {
            var OrderList = (from ord in db.Orders
                             join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                             join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                             join pro in db.Products on ordPro.product_code equals pro.product_code
                             join cust in db.Customers on ord.customer_id equals cust.customer_id
                             join agent in db.Agents on ord.agent_id equals agent.agent_id
                             join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                             // join tdd in db.Truck_dispatch_details on ordPro.order_id equals tdd.order_id
                             //  join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                             where ord.customer_id == customerId
                             && (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                             select new TempReports
                             {
                                 customerPO = ord.customerpo,
                                 agentPO = ord.order_id,
                                 itemNo = "",
                                 agentPOdt = ord.order_date,
                                 customerName = cust.name,
                                 agentName = agent.name,
                                 brandName = "",
                                 machineName = papermill.name,
                                 bf = pro.bf_code,
                                 gsm = pro.gsm_code,
                                 shade_code = ordPro.shade_code,
                                 qty = ordPro.qty,
                                 width = ordPro.width,
                                 diameter = ordPro.diameter,
                                 description = pro.description,
                                 unit_price = ordPro.unit_price ?? 0,// unitprice is same as rate
                                 amount = ordPro.amount,// amount is same as bill amount
                                 requested_delivery_date = ordPro.requested_delivery_date.ToString(),
                                 // actualDeliveryDate = td.left_factory_on,
                                 // truck_no = td.truck_no,
                                 stateName = "",
                                 // order_id = ord.order_id,//deliveryOrderNo is same as order_id
                                 // dispatchedOnFactory = td.left_factory_on,
                                 Variant = "",
                                 order_product_id = ordPro.order_product_id
                                 //excelFileName = "AllCustomer"
                             }).ToList();
            List<TempReports> reportObj = new List<TempReports>();
            foreach (var items in OrderList)
            {
                TempReports obj = new TempReports();

                var truckNos = from tdd in db.Truck_dispatch_details
                               join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                               where tdd.order_product_id == items.order_product_id
                               && (td.status == "Dispatched" || td.status == "Completed")
                               select td;
                string truckList = string.Empty;

                foreach (var item in truckNos)
                {
                    DateTime dt = new DateTime();
                    Boolean flag = false;
                    if (item.left_factory_on == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        dt = (DateTime)item.left_factory_on;
                        flag = true;
                    }
                    if (truckList != "")
                    {
                        if (flag == true)
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                        }
                    }
                    else
                    {
                        if (flag == true)
                        {
                            truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                        }

                    }
                    DateTime rqdt = Convert.ToDateTime(items.requested_delivery_date);
               DateTime acdt = new DateTime();
                if (truckNos.Count() != 0)
                {
                    // var query = (from d in truckNos select d.left_factory_on).Max();
                    DateTime? acdt1 = (DateTime?)truckNos.Where(p => p != null).Max(p => p.left_factory_on);
                    if (acdt1 != null)
                    {
                        acdt = (DateTime)acdt1;
                    }
                    else
                    { }
                }
                    obj.customerPO = items.customerPO;
                    obj.agentPO = items.agentPO;
                    obj.itemNo = "";
                    obj.customerName = items.customerName;
                    obj.brandOwner = "";
                    obj.bf = items.bf;
                    obj.gsm = items.gsm;
                    obj.shade_code = items.shade_code;
                    obj.qty = items.qty;//qty is same as weight
                    obj.width = items.width;
                    obj.diameter = items.diameter;
                    obj.description = items.description;
                    obj.unit_price = (decimal?)items.unit_price ?? 0;
                    obj.truck_no = truckList;
                    obj.agentName = items.agentName;
                    obj.requested_delivery_date = rqdt.ToShortDateString();
                    obj.Variant = "";
                    obj.order_product_id = items.order_product_id;
                    obj.stateName = items.stateName;
                    obj.machineName = items.machineName;
                    obj.actualDeliveryDate = acdt.ToShortDateString();
                    obj.amount = items.amount;
                    obj.description = items.description;
                    obj.agentPOdt = items.agentPOdt;
                    reportObj.Add(obj);
                }
            }
            return reportObj;

        }
        private List<TempReports> BDreportOfAllAgent(DateTime fromdt, DateTime todt)
        {
            var OrderList = (from ord in db.Orders
                             join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                             join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                             join pro in db.Products on ordPro.product_code equals pro.product_code
                             join cust in db.Customers on ord.customer_id equals cust.customer_id
                             join agent in db.Agents on ord.agent_id equals agent.agent_id
                             join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                             // join tdd in db.Truck_dispatch_details on ordPro.order_id equals tdd.order_id
                             //  join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                             where (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                             select new TempReports
                             {
                                 customerPO = ord.customerpo,
                                 agentPO = ord.order_id,
                                 itemNo = "",
                                 agentPOdt = ord.order_date,
                                 customerName = cust.name,
                                 agentName = agent.name,
                                 brandName = "",
                                 machineName = papermill.name,
                                 bf = pro.bf_code,
                                 gsm = pro.gsm_code,
                                 shade_code = ordPro.shade_code,
                                 qty = ordPro.qty,
                                 width = ordPro.width,
                                 diameter = ordPro.diameter,
                                 description = pro.description,
                                 unit_price = ordPro.unit_price ?? 0,// unitprice is same as rate
                                 amount = ordPro.amount,// amount is same as bill amount
                                 requested_delivery_date = ordPro.requested_delivery_date.ToString(),
                                 // actualDeliveryDate = td.left_factory_on,
                                 // truck_no = td.truck_no,
                                 stateName = "",
                                 // order_id = ord.order_id,//deliveryOrderNo is same as order_id
                                 // dispatchedOnFactory = td.left_factory_on,
                                 Variant = "",
                                 order_product_id = ordPro.order_product_id
                                 //excelFileName = "AllCustomer"
                             }).ToList();
            List<TempReports> reportObj = new List<TempReports>();
            foreach (var items in OrderList)
            {
                TempReports obj = new TempReports();

                var truckNos = (from tdd in db.Truck_dispatch_details
                                join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                                where tdd.order_product_id == items.order_product_id
                                && (td.status == "Dispatched" || td.status == "Completed")
                                select td).ToList();
                string truckList = string.Empty;

                foreach (var item in truckNos)
                {
                    DateTime dt = new DateTime();
                    Boolean flag = false;
                    if (item.left_factory_on == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        dt = (DateTime)item.left_factory_on;
                        flag = true;
                    }
                    if (truckList != "")
                    {
                        if (flag == true)
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                        }
                    }
                    else
                    {
                        if (flag == true)
                        {
                            truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                        }

                    }

                }
                DateTime rqdt = Convert.ToDateTime(items.requested_delivery_date);
                DateTime acdt = new DateTime();
                if (truckNos.Count() != 0)
                {
                    // var query = (from d in truckNos select d.left_factory_on).Max();
                    DateTime? acdt1 = (DateTime?)truckNos.Where(p => p != null).Max(p => p.left_factory_on);
                    if (acdt1 != null)
                    {
                        acdt = (DateTime)acdt1;
                    }
                    else
                    { }
                }
                obj.customerPO = items.customerPO;
                obj.agentPO = items.agentPO;
                obj.itemNo = "";
                obj.customerName = items.customerName;
                obj.brandOwner = "";
                obj.bf = items.bf;
                obj.gsm = items.gsm;
                obj.shade_code = items.shade_code;
                obj.qty = items.qty;//qty is same as weight
                obj.width = items.width;
                obj.diameter = items.diameter;
                obj.description = items.description;
                obj.unit_price = (decimal?)items.unit_price ?? 0;
                obj.truck_no = truckList;
                obj.agentName = items.agentName;
                obj.requested_delivery_date = rqdt.ToShortDateString();
                obj.Variant = "";
                obj.order_product_id = items.order_product_id;
                obj.stateName = items.stateName;
                obj.machineName = items.machineName;
                obj.actualDeliveryDate = acdt.ToShortDateString();
                obj.amount = items.amount;
                obj.description = items.description;
                obj.agentPOdt = items.agentPOdt;
                reportObj.Add(obj);
            }
            return reportObj;
        }
        private List<TempReports> BDreportOfEachAgent(int agentId, DateTime fromdt, DateTime todt)
        {
            var OrderList = (from ord in db.Orders
                             join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                             join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                             join pro in db.Products on ordPro.product_code equals pro.product_code
                             join cust in db.Customers on ord.customer_id equals cust.customer_id
                             join agent in db.Agents on ord.agent_id equals agent.agent_id
                             join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                             // join tdd in db.Truck_dispatch_details on ordPro.order_id equals tdd.order_id
                             //   join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                             where ord.agent_id == agentId
                             && (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                             select new TempReports
                             {
                                 customerPO = ord.customerpo,
                                 agentPO = ord.order_id,
                                 itemNo = "",
                                 agentPOdt = ord.order_date,
                                 customerName = cust.name,
                                 agentName = agent.name,
                                 brandName = "",
                                 machineName = papermill.name,
                                 bf = pro.bf_code,
                                 gsm = pro.gsm_code,
                                 shade_code = ordPro.shade_code,
                                 qty = ordPro.qty,
                                 width = ordPro.width,
                                 diameter = ordPro.diameter,
                                 description = pro.description,
                                 unit_price = ordPro.unit_price ?? 0,// unitprice is same as rate
                                 amount = ordPro.amount,// amount is same as bill amount
                                 requested_delivery_date = ordPro.requested_delivery_date.ToString(),
                                 // actualDeliveryDate = td.left_factory_on,
                                 // truck_no = td.truck_no,
                                 stateName = "",
                                 // order_id = ord.order_id,//deliveryOrderNo is same as order_id
                                 // dispatchedOnFactory = td.left_factory_on,
                                 Variant = "",
                                 order_product_id = ordPro.order_product_id
                                 //excelFileName = "AllCustomer"
                             }).ToList();
            List<TempReports> reportObj = new List<TempReports>();
            foreach (var items in OrderList)
            {
                TempReports obj = new TempReports();

                var truckNos = from tdd in db.Truck_dispatch_details
                               join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                               where tdd.order_product_id == items.order_product_id
                               && (td.status == "Dispatched" || td.status == "Completed")
                               select td;
                string truckList = string.Empty;

                foreach (var item in truckNos)
                {
                    DateTime dt = new DateTime();
                    Boolean flag = false;
                    if (item.left_factory_on == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        dt = (DateTime)item.left_factory_on;
                        flag = true;
                    }
                    if (truckList != "")
                    {
                        if (flag == true)
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                        }
                    }
                    else
                    {
                        if (flag == true)
                        {
                            truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                        }

                    }

                }
                DateTime rqdt = Convert.ToDateTime(items.requested_delivery_date);
                 DateTime acdt = new DateTime();
                if (truckNos.Count() != 0)
                {
                    // var query = (from d in truckNos select d.left_factory_on).Max();
                    DateTime? acdt1 = (DateTime?)truckNos.Where(p => p != null).Max(p => p.left_factory_on);
                    if (acdt1 != null)
                    {
                        acdt = (DateTime)acdt1;
                    }
                    else
                    { }
                }
                obj.customerPO = items.customerPO;
                obj.agentPO = items.agentPO;
                obj.itemNo = "";
                obj.customerName = items.customerName;
                obj.brandOwner = "";
                obj.bf = items.bf;
                obj.gsm = items.gsm;
                obj.shade_code = items.shade_code;
                obj.qty = items.qty;//qty is same as weight
                obj.width = items.width;
                obj.diameter = items.diameter;
                obj.description = items.description;
                obj.unit_price = (decimal?)items.unit_price ?? 0;
                obj.truck_no = truckList;
                obj.agentName = items.agentName;
                obj.requested_delivery_date = rqdt.ToShortDateString();
                obj.Variant = "";
                obj.order_product_id = items.order_product_id;
                obj.stateName = items.stateName;
                obj.machineName = items.machineName;
                obj.actualDeliveryDate = acdt.ToShortDateString();
                obj.amount = items.amount;
                obj.description = items.description;
                obj.agentPOdt = items.agentPOdt;
                reportObj.Add(obj);
            }
            return reportObj;

        }
        private List<TempReports> BDreportByPaperMillId(int papermillId, DateTime fromdt, DateTime todt)
        {
            var OrderList = (from ord in db.Orders
                             join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                             join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                             join pro in db.Products on ordPro.product_code equals pro.product_code
                             join cust in db.Customers on ord.customer_id equals cust.customer_id
                             join agent in db.Agents on ord.agent_id equals agent.agent_id
                             join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                             //  join tdd in db.Truck_dispatch_details on ordPro.order_id equals tdd.order_id
                             //join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id

                             where sch.papermill_id == papermillId
                             && (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                             select new TempReports
                             {
                                 customerPO = ord.customerpo,
                                 agentPO = ord.order_id,
                                 itemNo = "",
                                 agentPOdt = ord.order_date,
                                 customerName = cust.name,
                                 agentName = agent.name,
                                 brandName = "",
                                 machineName = papermill.name,
                                 bf = pro.bf_code,
                                 gsm = pro.gsm_code,
                                 shade_code = ordPro.shade_code,
                                 qty = ordPro.qty,
                                 width = ordPro.width,
                                 diameter = ordPro.diameter,
                                 description = pro.description,
                                 unit_price = ordPro.unit_price ?? 0,// unitprice is same as rate
                                 amount = ordPro.amount,// amount is same as bill amount
                                 requested_delivery_date = ordPro.requested_delivery_date.ToString(),
                                 // actualDeliveryDate = td.left_factory_on,
                                 // truck_no = td.truck_no,
                                 stateName = "",
                                 // order_id = ord.order_id,//deliveryOrderNo is same as order_id
                                 // dispatchedOnFactory = td.left_factory_on,
                                 Variant = "",
                                 order_product_id = ordPro.order_product_id
                                 //excelFileName = "AllCustomer"
                             }).ToList();
            List<TempReports> reportObj = new List<TempReports>();

            foreach (var items in OrderList)
            {
                TempReports obj = new TempReports();

                var truckNos = from tdd in db.Truck_dispatch_details
                               join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                               where tdd.order_product_id == items.order_product_id
                               && (td.status == "Dispatched" || td.status == "Completed")
                               select td;
                string truckList = string.Empty;

                foreach (var item in truckNos)
                {
                    DateTime dt = new DateTime();
                    Boolean flag = false;
                    if (item.left_factory_on == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        dt = (DateTime)item.left_factory_on;
                        flag = true;
                    }
                    if (truckList != "")
                    {
                        if (flag == true)
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                        }
                    }
                    else
                    {
                        if (flag == true)
                        {
                            truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                        }
                    }
                }
               DateTime acdt = new DateTime();
                if (truckNos.Count() != 0)
                {
                    // var query = (from d in truckNos select d.left_factory_on).Max();
                    DateTime? acdt1 = (DateTime?)truckNos.Where(p => p != null).Max(p => p.left_factory_on);
                    if (acdt1 != null)
                    {
                        acdt = (DateTime)acdt1;
                    }
                    else
                    { }
                }
                DateTime rqdt = Convert.ToDateTime(items.requested_delivery_date);
                //DateTime acdt = Convert.ToDateTime(items.actualDeliveryDate);
                obj.customerPO = items.customerPO;
                obj.agentPO = items.agentPO;
                obj.itemNo = "";
                obj.customerName = items.customerName;
                obj.brandOwner = "";
                obj.bf = items.bf;
                obj.gsm = items.gsm;
                obj.shade_code = items.shade_code;
                obj.qty = items.qty;//qty is same as weight
                obj.width = items.width;
                obj.diameter = items.diameter;
                obj.description = items.description;
                obj.unit_price = (decimal?)items.unit_price ?? 0;
                obj.truck_no = truckList;
                obj.agentName = items.agentName;
                obj.requested_delivery_date = rqdt.ToShortDateString();
                obj.Variant = "";
                obj.order_product_id = items.order_product_id;
                obj.stateName = items.stateName;
                obj.machineName = items.machineName;
                obj.actualDeliveryDate = acdt.ToShortDateString();
                obj.amount = items.amount;
                obj.description = items.description;
                obj.agentPOdt = items.agentPOdt;
                reportObj.Add(obj);
            }
            return reportObj;
        }
        private List<TempReports> BDreportOfEachAgentByPapermill(int papermillId, int agentid, DateTime fromdt, DateTime todt)
        {
            var OrderList = (from ord in db.Orders
                             join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                             join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                             join pro in db.Products on ordPro.product_code equals pro.product_code
                             join cust in db.Customers on ord.customer_id equals cust.customer_id
                             join agent in db.Agents on ord.agent_id equals agent.agent_id
                             join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                             //join tdd in db.Truck_dispatch_details on ordPro.order_id equals tdd.order_id
                             // join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                             where sch.papermill_id == papermillId
                             && ord.agent_id == agentid
                             && (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                             select new TempReports
                             {
                                 customerPO = ord.customerpo,
                                 agentPO = ord.order_id,
                                 itemNo = "",
                                 agentPOdt = ord.order_date,
                                 customerName = cust.name,
                                 agentName = agent.name,
                                 brandName = "",
                                 machineName = papermill.name,
                                 bf = pro.bf_code,
                                 gsm = pro.gsm_code,
                                 shade_code = ordPro.shade_code,
                                 qty = ordPro.qty,
                                 width = ordPro.width,
                                 diameter = ordPro.diameter,
                                 description = pro.description,
                                 unit_price = ordPro.unit_price ?? 0,// unitprice is same as rate
                                 amount = ordPro.amount,// amount is same as bill amount
                                 requested_delivery_date = ordPro.requested_delivery_date.ToString(),
                                 // actualDeliveryDate = td.left_factory_on,
                                 // truck_no = td.truck_no,
                                 stateName = "",
                                 // order_id = ord.order_id,//deliveryOrderNo is same as order_id
                                 // dispatchedOnFactory = td.left_factory_on,
                                 Variant = "",
                                 order_product_id = ordPro.order_product_id
                                 //excelFileName = "AllCustomer"
                             }).ToList();
            List<TempReports> reportObj = new List<TempReports>();
            foreach (var items in OrderList)
            {
                TempReports obj = new TempReports();

                var truckNos = from tdd in db.Truck_dispatch_details
                               join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                               where tdd.order_product_id == items.order_product_id
                               && (td.status == "Dispatched" || td.status == "Completed")
                               select td;
                string truckList = string.Empty;

                foreach (var item in truckNos)
                {
                    DateTime dt = new DateTime();
                    Boolean flag = false;
                    if (item.left_factory_on == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        dt = (DateTime)item.left_factory_on;
                        flag = true;
                    }
                    if (truckList != "")
                    {
                        if (flag == true)
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                        }
                    }
                    else
                    {
                        if (flag == true)
                        {
                            truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                        }

                    }

                }
              DateTime acdt = new DateTime();
                if (truckNos.Count() != 0)
                {
                    // var query = (from d in truckNos select d.left_factory_on).Max();
                    DateTime? acdt1 = (DateTime?)truckNos.Where(p => p != null).Max(p => p.left_factory_on);
                    if (acdt1 != null)
                    {
                        acdt = (DateTime)acdt1;
                    }
                    else
                    { }
                }
                DateTime rqdt = Convert.ToDateTime(items.requested_delivery_date);
                //  DateTime acdt = Convert.ToDateTime(items.actualDeliveryDate);
                obj.customerPO = items.customerPO;
                obj.agentPO = items.agentPO;
                obj.itemNo = "";
                obj.customerName = items.customerName;
                obj.brandOwner = "";
                obj.bf = items.bf;
                obj.gsm = items.gsm;
                obj.shade_code = items.shade_code;
                obj.qty = items.qty;//qty is same as weight
                obj.width = items.width;
                obj.diameter = items.diameter;
                obj.description = items.description;
                obj.unit_price = (decimal?)items.unit_price ?? 0;
                obj.truck_no = truckList;
                obj.agentName = items.agentName;
                obj.requested_delivery_date = rqdt.ToShortDateString();
                obj.Variant = "";
                obj.order_product_id = items.order_product_id;
                obj.stateName = items.stateName;
                obj.machineName = items.machineName;
                obj.actualDeliveryDate = acdt.ToShortDateString();
                obj.amount = items.amount;
                obj.description = items.description;
                obj.agentPOdt = items.agentPOdt;
                reportObj.Add(obj);
            }
            return reportObj;
        }
        private List<TempReports> BDreportOfEachCustomerByMillId(int papermillId, int customerId, DateTime fromdt, DateTime todt)
        {
            var OrderList = (from ord in db.Orders
                             join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                             join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                             join pro in db.Products on ordPro.product_code equals pro.product_code
                             join cust in db.Customers on ord.customer_id equals cust.customer_id
                             join agent in db.Agents on ord.agent_id equals agent.agent_id
                             join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                             //   join tdd in db.Truck_dispatch_details on ordPro.order_id equals tdd.order_id
                             // join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                             where sch.papermill_id == papermillId
                             && ord.customer_id == customerId
                             && (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                             select new TempReports
                             {
                                 customerPO = ord.customerpo,
                                 agentPO = ord.order_id,
                                 itemNo = "",
                                 agentPOdt = ord.order_date,
                                 customerName = cust.name,
                                 agentName = agent.name,
                                 brandName = "",
                                 machineName = papermill.name,
                                 bf = pro.bf_code,
                                 gsm = pro.gsm_code,
                                 shade_code = ordPro.shade_code,
                                 qty = ordPro.qty,
                                 width = ordPro.width,
                                 diameter = ordPro.diameter,
                                 description = pro.description,
                                 unit_price = ordPro.unit_price ?? 0,// unitprice is same as rate
                                 amount = ordPro.amount,// amount is same as bill amount
                                 requested_delivery_date = ordPro.requested_delivery_date.ToString(),
                                 //actualDeliveryDate = td.left_factory_on,
                                 // truck_no = td.truck_no,
                                 stateName = "",
                                 // order_id = ord.order_id,//deliveryOrderNo is same as order_id
                                 // dispatchedOnFactory = td.left_factory_on,
                                 Variant = "",
                                 order_product_id = ordPro.order_product_id
                                 //excelFileName = "AllCustomer"
                             }).ToList();
            List<TempReports> reportObj = new List<TempReports>();
            foreach (var items in OrderList)
            {
                TempReports obj = new TempReports();

                var truckNos = from tdd in db.Truck_dispatch_details
                               join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                               where tdd.order_product_id == items.order_product_id
                               && (td.status == "Dispatched" || td.status == "Completed")
                               select td;
                string truckList = string.Empty;

                foreach (var item in truckNos)
                {
                    DateTime dt = new DateTime();
                    Boolean flag = false;
                    if (item.left_factory_on == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        dt = (DateTime)item.left_factory_on;
                        flag = true;
                    }
                    if (truckList != "")
                    {
                        if (flag == true)
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                        }
                    }
                    else
                    {
                        if (flag == true)
                        {
                            truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                        }
                    }
                }
                DateTime rqdt = Convert.ToDateTime(items.requested_delivery_date);
               DateTime acdt = new DateTime();
               if (truckNos.Count() != 0)
               {
                   // var query = (from d in truckNos select d.left_factory_on).Max();
                   DateTime? acdt1 = (DateTime?)truckNos.Where(p => p != null).Max(p => p.left_factory_on);
                   if (acdt1 != null)
                   {
                       acdt = (DateTime)acdt1;
                   }
                   else
                   { }
               }
                //DateTime acdt = (DateTime)(from d in truckNos select d.left_factory_on).Max();
                obj.customerPO = items.customerPO;
                obj.agentPO = items.agentPO;
                obj.itemNo = "";
                obj.customerName = items.customerName;
                obj.brandOwner = "";
                obj.bf = items.bf;
                obj.gsm = items.gsm;
                obj.shade_code = items.shade_code;
                obj.qty = items.qty;//qty is same as weight
                obj.width = items.width;
                obj.diameter = items.diameter;
                obj.description = items.description;
                obj.unit_price = (decimal?)items.unit_price ?? 0;
                obj.truck_no = truckList;
                obj.agentName = items.agentName;
                obj.requested_delivery_date = rqdt.ToShortDateString();
                obj.Variant = "";
                obj.order_product_id = items.order_product_id;
                obj.stateName = items.stateName;
                obj.machineName = items.machineName;
                obj.actualDeliveryDate = acdt.ToShortDateString();
                obj.amount = items.amount;
                obj.description = items.description;
                obj.agentPOdt = items.agentPOdt;
                reportObj.Add(obj);
            }
            return reportObj;
        }
        public List<TempReports> OrderBookReport(string type, int agentId, int customerId, DateTime fromdt, DateTime todt)
        {
            try
            {
                List<TempReports> orderList = new List<TempReports>();
                switch (type)
                {
                    case "by-customer":
                        if (customerId == 0)
                        { orderList = OrderBookReportForAllCustomer(fromdt, todt); }
                        else
                        { orderList = OrderBookReportForEachCustomer(customerId, fromdt, todt); }
                        break;
                    case "by-agent":
                        if (agentId == 0)
                        { orderList = OrderBookReportForAllAgent(fromdt, todt); }
                        else
                        { orderList = OrderBookReportForEachAgent(agentId, fromdt, todt); }
                        break;
                }
                return orderList;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Order->OrdersSearchResults:", Ex);
                return null;
            }

        }
        private List<TempReports> OrderBookReportForAllCustomer(DateTime fromdt, DateTime todt)
        {
            List<TempReports> orderList = (from ord in db.Orders
                                           join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                                           join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                                           join pro in db.Products on ordPro.product_code equals pro.product_code
                                           join cust in db.Customers on ord.customer_id equals cust.customer_id
                                           join agent in db.Agents on ord.agent_id equals agent.agent_id
                                           join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                                           where (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                                           select new TempReports
                                           {
                                               customerPO = ord.customerpo,
                                               agentPO = ord.order_id,
                                               itemNo = "",
                                               customerName = cust.name,
                                               agentName = agent.name,
                                               brandOwner = "",
                                               bf = pro.bf_code,
                                               gsm = pro.gsm_code,
                                               shade_code = ordPro.shade_code,
                                               qty = ordPro.qty,//qty is same as weight
                                               width = ordPro.width,
                                               diameter = ordPro.diameter,
                                               description = pro.description,
                                               requested_delivery_date = ordPro.requested_delivery_date.ToString(),
                                               unit_price = ordPro.unit_price,
                                               machineName = papermill.name,
                                               underplanningQty = (decimal?)ordPro.qty - (decimal?)ordPro.qty_scheduled ?? 0,
                                               actualLoadedQty = 0,
                                               actualProducedQty = 0,
                                               truck_no = "",
                                               agentname = agent.name,
                                               dispatchedOnFactory = null,//date of dispatched
                                               Variant = "",
                                               order_product_id = ordPro.order_product_id,
                                               schduledQty = (decimal?)ordPro.qty_scheduled,
                                               stateName = "",
                                               orderloggingdt = DateTime.MinValue,

                                           })
                       .ToList();
            List<TempReports> reportObj = new List<TempReports>();
            foreach (var items in orderList)
            {
                TempReports obj = new TempReports();
                var actualPcQty = db.ProductionChild.Where(x => x.actual_end != null
                            && x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty);
                var actualLoadedQty = db.Truck_dispatch_details.
                                      Where(x => x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty_loaded);

                var truckNos = from tdd in db.Truck_dispatch_details
                               join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                               where tdd.order_product_id == items.order_product_id
                               && (td.status == "Dispatched" || td.status == "Completed")
                               select td;
                string truckList = string.Empty;

                foreach (var item in truckNos)
                {
                    DateTime dt = new DateTime();
                    Boolean flag = false;
                    if (item.left_factory_on == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        dt = (DateTime)item.left_factory_on;
                        flag = true;
                    }
                    if (truckList != "")
                    {
                        if (flag == true)
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                        }
                    }
                    else
                    {
                        if (flag == true)
                        {
                            truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                            if (flag == false)
                            {
                                truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                            }

                    }
                }
                DateTime rqdt = Convert.ToDateTime(items.requested_delivery_date);
                obj.customerPO = items.customerPO;
                obj.agentPO = items.agentPO;
                obj.itemNo = "";
                obj.customerName = items.customerName;
                obj.brandOwner = "";
                obj.bf = items.bf;
                obj.gsm = items.gsm;
                obj.shade_code = items.shade_code;
                obj.qty = items.qty;//qty is same as weight
                obj.width = items.width;
                obj.diameter = items.diameter;
                obj.description = items.description;
                obj.underplanningQty = (decimal?)items.underplanningQty ?? 0;
                obj.dispatchedQty = (decimal?)actualLoadedQty ?? 0;
                obj.actualProducedQty = (decimal?)actualPcQty ?? 0;
                obj.plannedQty = ((decimal?)items.schduledQty ?? 0) - ((decimal?)actualPcQty ?? 0);
                obj.FGqty = ((decimal?)actualPcQty ?? 0) - ((decimal?)actualLoadedQty ?? 0);
                obj.unit_price = (decimal?)items.unit_price ?? 0;
                obj.truck_no = truckList;
                obj.agentName = items.agentName;
                obj.requested_delivery_date = rqdt.ToShortDateString();
                obj.Variant = "";
                obj.order_product_id = items.order_product_id;
                obj.stateName = items.stateName;
                obj.machineName = items.machineName;
                obj.orderloggingdt = items.orderloggingdt;
                reportObj.Add(obj);


            }
            return reportObj;
        }
        private List<TempReports> OrderBookReportForEachCustomer(int customerId, DateTime fromdt, DateTime todt)
        {
            List<TempReports> orderList = (from ord in db.Orders
                                           join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                                           join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                                           join pro in db.Products on ordPro.product_code equals pro.product_code
                                           join cust in db.Customers on ord.customer_id equals cust.customer_id
                                           join agent in db.Agents on ord.agent_id equals agent.agent_id
                                           join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                                           where ord.customer_id == customerId
                                           && (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                                           select new TempReports
                                           {
                                               customerPO = ord.customerpo,
                                               agentPO = ord.order_id,
                                               itemNo = "",
                                               customerName = cust.name,
                                               agentName = agent.name,
                                               brandOwner = "",
                                               bf = pro.bf_code,
                                               gsm = pro.gsm_code,
                                               shade_code = ordPro.shade_code,
                                               qty = ordPro.qty,//qty is same as weight
                                               width = ordPro.width,
                                               diameter = ordPro.diameter,
                                               description = pro.description,
                                               requested_delivery_date = ordPro.requested_delivery_date.ToString(),
                                               unit_price = ordPro.unit_price,
                                               machineName = papermill.name,
                                               underplanningQty = (decimal?)ordPro.qty - (decimal?)ordPro.qty_scheduled ?? 0,
                                               actualLoadedQty = 0,
                                               actualProducedQty = 0,
                                               truck_no = "",
                                               agentname = agent.name,
                                               dispatchedOnFactory = null,//date of dispatched
                                               Variant = "",
                                               order_product_id = ordPro.order_product_id,
                                               schduledQty = (decimal?)ordPro.qty_scheduled,
                                               stateName = "",
                                               orderloggingdt = DateTime.MinValue,

                                           })
                       .ToList();
            List<TempReports> reportObj = new List<TempReports>();
            foreach (var items in orderList)
            {
                TempReports obj = new TempReports();
                var actualPcQty = db.ProductionChild.Where(x => x.actual_end != null
                            && x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty);
                var actualLoadedQty = db.Truck_dispatch_details.
                                      Where(x => x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty_loaded);

                var truckNos = from tdd in db.Truck_dispatch_details
                               join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                               where tdd.order_product_id == items.order_product_id
                              && (td.status == "Dispatched" || td.status == "Completed")
                               select td;
                string truckList = string.Empty;

                foreach (var item in truckNos)
                {
                    DateTime dt = new DateTime();
                    Boolean flag = false;
                    if (item.left_factory_on == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        dt = (DateTime)item.left_factory_on;
                        flag = true;
                    }
                    if (truckList != "")
                    {
                        if (flag == true)
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                        }
                    }
                    else
                    {
                        if (flag == true)
                        {
                            truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                            if (flag == false)
                            {
                                truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                            }

                    }
                }
                DateTime rqdt = Convert.ToDateTime(items.requested_delivery_date);
                obj.customerPO = items.customerPO;
                obj.agentPO = items.agentPO;
                obj.itemNo = "";
                obj.customerName = items.customerName;
                obj.brandOwner = "";
                obj.bf = items.bf;
                obj.gsm = items.gsm;
                obj.shade_code = items.shade_code;
                obj.qty = items.qty;//qty is same as weight
                obj.width = items.width;
                obj.diameter = items.diameter;
                obj.description = items.description;
                obj.underplanningQty = (decimal?)items.underplanningQty ?? 0;
                obj.dispatchedQty = (decimal?)actualLoadedQty ?? 0;
                obj.actualProducedQty = (decimal?)actualPcQty ?? 0;
                obj.plannedQty = ((decimal?)items.schduledQty ?? 0) - ((decimal?)actualPcQty ?? 0);
                obj.FGqty = ((decimal?)actualPcQty ?? 0) - ((decimal?)actualLoadedQty ?? 0);
                obj.unit_price = (decimal?)items.unit_price ?? 0;
                obj.truck_no = truckList;
                obj.agentName = items.agentName;
                obj.requested_delivery_date = rqdt.ToShortDateString();
                obj.Variant = "";
                obj.order_product_id = items.order_product_id;
                obj.stateName = items.stateName;
                obj.machineName = items.machineName;
                obj.orderloggingdt = items.orderloggingdt;
                reportObj.Add(obj);
            }
            return reportObj;
        }
        private List<TempReports> OrderBookReportForAllAgent(DateTime fromdt, DateTime todt)
        {
            List<TempReports> orderList = (from ord in db.Orders
                                           join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                                           join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                                           join pro in db.Products on ordPro.product_code equals pro.product_code
                                           join cust in db.Customers on ord.customer_id equals cust.customer_id
                                           join agent in db.Agents on ord.agent_id equals agent.agent_id
                                           join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                                           where (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                                           select new TempReports
                                           {
                                               customerPO = ord.customerpo,
                                               agentPO = ord.order_id,
                                               itemNo = "",
                                               customerName = cust.name,
                                               agentName = agent.name,
                                               brandOwner = "",
                                               bf = pro.bf_code,
                                               gsm = pro.gsm_code,
                                               shade_code = ordPro.shade_code,
                                               qty = ordPro.qty,//qty is same as weight
                                               width = ordPro.width,
                                               diameter = ordPro.diameter,
                                               description = pro.description,
                                               requested_delivery_date = ordPro.requested_delivery_date.ToString(),
                                               unit_price = ordPro.unit_price,
                                               machineName = papermill.name,
                                               underplanningQty = (decimal?)ordPro.qty - (decimal?)ordPro.qty_scheduled ?? 0,
                                               actualLoadedQty = 0,
                                               actualProducedQty = 0,
                                               truck_no = "",
                                               agentname = agent.name,
                                               dispatchedOnFactory = null,//date of dispatched
                                               Variant = "",
                                               order_product_id = ordPro.order_product_id,
                                               schduledQty = (decimal?)ordPro.qty_scheduled,
                                               stateName = "",
                                               orderloggingdt = DateTime.MinValue,

                                           })
                       .ToList();
            List<TempReports> reportObj = new List<TempReports>();
            foreach (var items in orderList)
            {
                TempReports obj = new TempReports();
                var actualPcQty = db.ProductionChild.Where(x => x.actual_end != null
                            && x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty);
                var actualLoadedQty = db.Truck_dispatch_details.
                                      Where(x => x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty_loaded);

                var truckNos = from tdd in db.Truck_dispatch_details
                               join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                               where tdd.order_product_id == items.order_product_id
                               && (td.status == "Dispatched" || td.status == "Completed")
                               select td;
                string truckList = string.Empty;

                foreach (var item in truckNos)
                {
                    DateTime dt = new DateTime();
                    Boolean flag = false;
                    if (item.left_factory_on == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        dt = (DateTime)item.left_factory_on;
                        flag = true;
                    }
                    if (truckList != "")
                    {
                        if (flag == true)
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                        }
                    }
                    else
                    {
                        if (flag == true)
                        {
                            truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                            if (flag == false)
                            {
                                truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                            }

                    }
                }
                DateTime rqdt = Convert.ToDateTime(items.requested_delivery_date);
                obj.customerPO = items.customerPO;
                obj.agentPO = items.agentPO;
                obj.itemNo = "";
                obj.customerName = items.customerName;
                obj.brandOwner = "";
                obj.bf = items.bf;
                obj.gsm = items.gsm;
                obj.shade_code = items.shade_code;
                obj.qty = items.qty;//qty is same as weight
                obj.width = items.width;
                obj.diameter = items.diameter;
                obj.description = items.description;
                obj.underplanningQty = (decimal?)items.underplanningQty ?? 0;
                obj.dispatchedQty = (decimal?)actualLoadedQty ?? 0;
                obj.actualProducedQty = (decimal?)actualPcQty ?? 0;
                obj.plannedQty = ((decimal?)items.schduledQty ?? 0) - ((decimal?)actualPcQty ?? 0);
                obj.FGqty = ((decimal?)actualPcQty ?? 0) - ((decimal?)actualLoadedQty ?? 0);
                obj.unit_price = (decimal?)items.unit_price ?? 0;
                obj.truck_no = truckList;
                obj.agentName = items.agentName;
                obj.requested_delivery_date = rqdt.ToShortDateString();
                obj.Variant = "";
                obj.order_product_id = items.order_product_id;
                obj.stateName = items.stateName;
                obj.machineName = items.machineName;
                obj.orderloggingdt = items.orderloggingdt;
                reportObj.Add(obj);


            }
            return reportObj;
        }
        private List<TempReports> OrderBookReportForEachAgent(int agentId, DateTime fromdt, DateTime todt)
        {
            List<TempReports> orderList = (from ord in db.Orders
                                           join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                                           join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                                           join pro in db.Products on ordPro.product_code equals pro.product_code
                                           join cust in db.Customers on ord.customer_id equals cust.customer_id
                                           join agent in db.Agents on ord.agent_id equals agent.agent_id
                                           join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                                           where ord.agent_id == agentId
                                           && (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                                           select new TempReports
                                           {
                                               customerPO = ord.customerpo,
                                               agentPO = ord.order_id,
                                               itemNo = "",
                                               customerName = cust.name,
                                               agentName = agent.name,
                                               brandOwner = "",
                                               bf = pro.bf_code,
                                               gsm = pro.gsm_code,
                                               shade_code = ordPro.shade_code,
                                               qty = ordPro.qty,//qty is same as weight
                                               width = ordPro.width,
                                               diameter = ordPro.diameter,
                                               description = pro.description,
                                               requested_delivery_date = ordPro.requested_delivery_date.ToString(),
                                               unit_price = ordPro.unit_price,
                                               machineName = papermill.name,
                                               underplanningQty = (decimal?)ordPro.qty - (decimal?)ordPro.qty_scheduled ?? 0,
                                               actualLoadedQty = 0,
                                               actualProducedQty = 0,
                                               truck_no = "",
                                               agentname = agent.name,
                                               dispatchedOnFactory = null,//date of dispatched
                                               Variant = "",
                                               order_product_id = ordPro.order_product_id,
                                               schduledQty = (decimal?)ordPro.qty_scheduled,
                                               stateName = "",
                                               orderloggingdt = DateTime.MinValue,

                                           })
                       .ToList();
            List<TempReports> reportObj = new List<TempReports>();
            foreach (var items in orderList)
            {
                TempReports obj = new TempReports();
                var actualPcQty = db.ProductionChild.Where(x => x.actual_end != null
                            && x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty);
                var actualLoadedQty = db.Truck_dispatch_details.
                                      Where(x => x.order_product_id == items.order_product_id).Sum(x => (decimal?)x.qty_loaded);

                var truckNos = from tdd in db.Truck_dispatch_details
                               join td in db.Truck_dispatches on tdd.truck_dispatch_id equals td.truck_dispatch_id
                               where tdd.order_product_id == items.order_product_id
                               && (td.status == "Dispatched" || td.status == "Completed")
                               select td;
                string truckList = string.Empty;

                foreach (var item in truckNos)
                {
                    DateTime dt = new DateTime();
                    Boolean flag = false;
                    if (item.left_factory_on == null)
                    {
                        flag = false;
                    }
                    else
                    {
                        dt = (DateTime)item.left_factory_on;
                        flag = true;
                    }
                    if (truckList != "")
                    {
                        if (flag == true)
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                        {
                            truckList = truckList + ", " + item.truck_no + " (" + "Pending" + ")";
                        }
                    }
                    else
                    {
                        if (flag == true)
                        {
                            truckList = truckList + item.truck_no + " (" + dt.ToShortDateString() + ")";
                        }
                        else
                            if (flag == false)
                            {
                                truckList = truckList + item.truck_no + " (" + "Pending" + ")";
                            }

                    }
                }
                DateTime rqdt = Convert.ToDateTime(items.requested_delivery_date);
                obj.customerPO = items.customerPO;
                obj.agentPO = items.agentPO;
                obj.itemNo = "";
                obj.customerName = items.customerName;
                obj.brandOwner = "";
                obj.bf = items.bf;
                obj.gsm = items.gsm;
                obj.shade_code = items.shade_code;
                obj.qty = items.qty;//qty is same as weight
                obj.width = items.width;
                obj.diameter = items.diameter;
                obj.description = items.description;
                obj.underplanningQty = (decimal?)items.underplanningQty ?? 0;
                obj.dispatchedQty = (decimal?)actualLoadedQty ?? 0;
                obj.actualProducedQty = (decimal?)actualPcQty ?? 0;
                obj.plannedQty = ((decimal?)items.schduledQty ?? 0) - ((decimal?)actualPcQty ?? 0);
                obj.FGqty = ((decimal?)actualPcQty ?? 0) - ((decimal?)actualLoadedQty ?? 0);
                obj.unit_price = (decimal?)items.unit_price ?? 0;
                obj.truck_no = truckList;
                obj.agentName = items.agentName;
                obj.requested_delivery_date = rqdt.ToShortDateString();
                obj.Variant = "";
                obj.order_product_id = items.order_product_id;
                obj.stateName = items.stateName;
                obj.machineName = items.machineName;
                obj.orderloggingdt = items.orderloggingdt;
                reportObj.Add(obj);
            }
            return reportObj;
        }
        public List<dynamic> GetFGReportDetails(DateTime fromdt, DateTime todt, int papermillId)
        {

            if (papermillId == 0)
            {
                var OrderList = (from ord in db.Orders
                                 join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                                 join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                                 join pro in db.Products on ordPro.product_code equals pro.product_code
                                 join tdd in db.Truck_dispatch_details on ordPro.order_product_id equals tdd.order_product_id into xtemp
                                 from runsch in xtemp.DefaultIfEmpty()
                                 join cust in db.Customers on ord.customer_id equals cust.customer_id
                                 join agent in db.Agents on ord.agent_id equals agent.agent_id
                                 join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                                 join td in db.Truck_dispatches on runsch.truck_dispatch_id equals td.truck_dispatch_id
                                 join pc in db.ProductionChild on ordPro.order_product_id equals pc.order_product_id into xtemp1
                                 from prochild in xtemp1.Where(x => x.actual_end != null).DefaultIfEmpty()
                                 join pc in db.ProductionChild on ordPro.order_product_id equals pc.order_product_id
                                 join pj in db.ProductionJumbo on pc.pj_id equals pj.pj_id
                                 where
                               (ordPro.requested_delivery_date >= fromdt && ordPro.requested_delivery_date <= todt)
                                 select new
                                 {
                                     customerPO = ord.customerpo,
                                     itemNo = "",
                                     customerName = cust.name,
                                     agentName = agent.name,
                                     brandOwner = "",
                                     bf = pro.bf_code,
                                     gsm = pro.gsm_code,
                                     shade_code = ordPro.shade_code,
                                     qty = ordPro.qty,//weight is same as qty
                                     width = ordPro.width,
                                     diameter = ordPro.diameter,
                                     requested_delivery_date = ordPro.requested_delivery_date,
                                     description = pro.description,
                                     unit_price = ordPro.unit_price,
                                     machineName = papermill.name,
                                     underplanningStatus = ordPro.qty - ordPro.qty_scheduled,
                                     plannedStatus = ordPro.qty_scheduled - xtemp1.Sum(x => (decimal?)x.qty),//producced qty - sum of finished qty(actual produced by machine head)
                                     FGStatus = xtemp1.Sum(x => (decimal?)x.qty) ?? 0 - xtemp.Sum(x => (decimal?)x.qty_loaded),
                                     dispatched = xtemp.Sum(x => (decimal?)x.qty_loaded) ?? 0,
                                     truck_no = td.truck_no,
                                     rollNO = pc.child_rollno,
                                     Variant = "",
                                     lotno = pc.sequence,

                                     //ageofStock = DateTime.Now - pj.actual_end,
                                     //excelFileName = "All"

                                 }).ToList();

                List<dynamic> dynamicList = OrderList.Select(x => (dynamic)x).ToList();
                return dynamicList;

            }
            else
            {
                var OrderList = (from ord in db.Orders
                                 join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                                 join pro in db.Products on ordPro.product_code equals pro.product_code

                                 join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id

                                 join tdd in db.Truck_dispatch_details on ordPro.order_product_id equals tdd.order_product_id into xtemp
                                 from runsch in xtemp.DefaultIfEmpty()
                                 join cust in db.Customers on ord.customer_id equals cust.customer_id
                                 join agent in db.Agents on ord.agent_id equals agent.agent_id
                                 join papermill in db.Papermills on sch.papermill_id equals papermill.papermill_id
                                 join td in db.Truck_dispatches on runsch.truck_dispatch_id equals td.truck_dispatch_id
                                 join pc in db.ProductionChild on ordPro.order_product_id equals pc.order_product_id into xtemp1
                                 from prochild in xtemp1.Where(x => x.actual_end != null).DefaultIfEmpty()
                                 join pc in db.ProductionChild on ordPro.order_product_id equals pc.order_product_id
                                 join pj in db.ProductionJumbo on pc.pj_id equals pj.pj_id
                                 where
                                 ordPro.requested_delivery_date == fromdt && ordPro.requested_delivery_date == todt
                                 select new
                                 {
                                     customerPO = ord.customerpo,
                                     itemNo = "",
                                     customerName = cust.name,
                                     agentName = agent.name,
                                     brandOwner = "",
                                     bf = pro.bf_code,
                                     gsm = pro.gsm_code,
                                     shade_code = ordPro.shade_code,
                                     qty = ordPro.qty,//weight is same as qty
                                     width = ordPro.width,
                                     diameter = ordPro.diameter,
                                     requested_delivery_date = ordPro.requested_delivery_date,
                                     description = pro.description,
                                     unit_price = ordPro.unit_price,
                                     machineName = papermill.name,
                                     underplanningStatus = ordPro.qty - ordPro.qty_scheduled,
                                     plannedStatus = ordPro.qty_scheduled - xtemp1.Sum(x => (decimal?)x.qty),//producced qty - sum of finished qty(actual produced by machine head)
                                     FGStatus = xtemp1.Sum(x => (decimal?)x.qty) ?? 0 - xtemp.Sum(x => (decimal?)x.qty_loaded),
                                     dispatched = xtemp.Sum(x => (decimal?)x.qty_loaded) ?? 0,
                                     truck_no = td.truck_no,
                                     rollNO = pc.child_rollno,
                                     lotno = pc.sequence,
                                     //ageofStock = DateTime.Now - pj.actual_end,
                                     //excelFileName = papermill.name,

                                 }).ToList();
                List<dynamic> dynamicList = OrderList.Select(x => (dynamic)x).ToList();
                return dynamicList;
            }

        }


    }
}