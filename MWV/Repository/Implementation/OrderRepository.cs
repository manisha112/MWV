using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MWV.Models;
using MWV.DBContext;
using MWV.Repository.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.WebPages.Html;
using System.Web.Mvc;
using MWV.ViewModels;
using MWV.Business;

namespace MWV.Repository.Implementation
{
    public class OrderRepository : IOrder
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private MWVDBContext db = new MWVDBContext();
        public OrderRepository()
        {

        }

        #region
        // this function returns one row of the Order_products table
        public tempOrderProducts GetProductDetails(int prodid)
        {
            //var prod_details = db.Order_products.Where(k => k.order_product_id == prodid).SingleOrDefault();

            var prod_details = (from ord in db.Order_products
                                join prods in db.Products on ord.product_code equals prods.product_code
                                where ord.order_product_id == prodid
                                select new tempOrderProducts
                                {
                                    bf_code = prods.bf_code,
                                    gsm_code = prods.gsm_code,
                                    shade_code = ord.shade_code,
                                    amount = ord.amount,
                                    core = ord.core,
                                    created_by = ord.created_by,
                                    diameter = ord.diameter,
                                    order_id = ord.order_id,
                                    order_product_id = ord.order_product_id,
                                    product_code = ord.product_code,
                                    qty = ord.qty,
                                    width = ord.width,
                                    unit_price = ord.unit_price,
                                    requested_delivery_date = ord.requested_delivery_date
                                }).SingleOrDefault();

            return prod_details;
        }
        #endregion


        #region AddOrder
        // this function adds the order to the database
        // <returns the order id>
        public int AddOrder(OrderRepository.tempOrder temporder, List<OrderRepository.tempOrderProducts> lstTempProds)
        {
            try
            {
                // get the properties from temp and add to Order order
                Order order = new Order();
                order.agent_id = temporder.agent_id;
                // get the agent name
                order.created_by = temporder.agentname;
                //DateTime dtCreation = Convert.ToDateTime(DateTime.Now.ToString("MMM-dd-yyyy"));
                DateTime dtCreation = DateTime.Now;

                order.created_on = dtCreation;
                order.customer_id = temporder.customer_id;
                //DateTime reqdeldate = Convert.ToDateTime(temporder.requested_delivery_date.ToString());
                // commented for CR
                // order.requested_delivery_date = temporder.requested_delivery_date; // Convert.ToDateTime(reqdeldate.ToString("MMM-dd-yyyy"));
                order.customerpo = temporder.customerpo;
                order.status = "Created";
                order.order_date = DateTime.Now;

                decimal amount = 0.0m;
                foreach (OrderRepository.tempOrderProducts item in lstTempProds)
                {
                    if (item.status == "A" || item.status == "E")
                    {
                        amount = amount + Convert.ToDecimal(item.amount);
                    }
                }
                order.amount = amount;
                // add to the database, return the order id
                db.Orders.Add(order);
                db.SaveChanges();
                int lastinsertedId = order.order_id;

                // loop through the list to get the products 
                // and add the products to the products table, prepare the order_product object
                foreach (OrderRepository.tempOrderProducts item in lstTempProds)
                {
                    Order_Products orderproducts = new Order_Products();
                    if (item.status == "A" || item.status == "E")
                    {
                        orderproducts.amount = item.amount;
                        orderproducts.core = item.core;
                        orderproducts.diameter = item.diameter;
                        orderproducts.order_id = lastinsertedId;
                        orderproducts.product_code = item.product_code;
                        orderproducts.qty = item.qty;
                        orderproducts.shade_code = item.shade_code;
                        orderproducts.status = "Created";
                        orderproducts.unit_price = item.unit_price;
                        orderproducts.width = item.width;
                        // because there is a check of quantity by this field while arranging transportation 
                        orderproducts.qty_planned_byAgent = 0;
                        // since a condition is being put these fields need to be entered as zero
                        orderproducts.qty_dispatched_byFactory = 0;
                        orderproducts.qty_scheduled = 0;

                        orderproducts.created_on = DateTime.Now;

                        orderproducts.created_by = item.created_by;
                        orderproducts.requested_delivery_date = item.requested_delivery_date;

                        db.Order_products.Add(orderproducts);
                    }
                    db.SaveChanges();
                }
                return lastinsertedId;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->AddOrder:", Ex);
                return 0;
            }
        }
        #endregion

        #region AddOrder
        // this function adds the order to the database
        // <returns the order id>
        public int UpdateOrder(OrderRepository.tempOrder temporder, List<OrderRepository.tempOrderProducts> lstTempProds)
        {
            try
            {
                // OrderRepository order = new OrderRepository();
                Order objOrder = db.Orders.Find(temporder.order_id);

                // get the properties from temp and add to Order order
                //Order order = new Order();
                objOrder.agent_id = temporder.agent_id;
                // get the agent name
                objOrder.created_by = temporder.agentname;
                DateTime dtCreation = DateTime.Now;

                objOrder.created_on = dtCreation;
                objOrder.customer_id = temporder.customer_id;
                objOrder.requested_delivery_date = temporder.requested_delivery_date;
                objOrder.status = "Created";
                objOrder.order_date = DateTime.Now;

                // add to the database, return the order id
                db.Orders.Add(objOrder);
                db.SaveChanges();
                int lastinsertedId = objOrder.order_id;

                // loop through the list to get the products 
                // and add the products to the products table, prepare the order_product object
                foreach (OrderRepository.tempOrderProducts item in lstTempProds)
                {
                    Order_Products orderproducts = new Order_Products();
                    if (orderproducts.status == "A")
                    {
                        orderproducts.amount = item.amount;
                        orderproducts.core = item.core;
                        orderproducts.diameter = item.diameter;
                        orderproducts.order_id = lastinsertedId;
                        orderproducts.product_code = item.product_code;
                        orderproducts.qty = item.qty;
                        orderproducts.shade_code = item.shade_code;
                        orderproducts.status = "Created";
                        orderproducts.unit_price = item.unit_price;
                        orderproducts.width = item.width;
                        // because there is a check of quantity by this field while arranging transportation 
                        orderproducts.qty_planned_byAgent = 0;
                        // since a condition is being put these fields need to be entered as zero
                        orderproducts.qty_dispatched_byFactory = 0;
                        orderproducts.qty_scheduled = 0;

                        orderproducts.created_on = DateTime.Now;

                        orderproducts.created_by = item.created_by;

                        db.Order_products.Add(orderproducts);
                    }
                    db.SaveChanges();
                }
                return lastinsertedId;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->UpdateOrder:", Ex);
                return 0;
            }
        }
        #endregion

        #region OrdersSearchResults
        public List<Order> OrdersSearchResults(string SelectedStatusValue, DateTime fromDate, DateTime todate)
        {
            DateTime toDate = todate.AddHours(23).AddMinutes(59).AddSeconds(59);

            // since the server has american format of date
            // convert the ddmmyyyy to mmddyyyy
            //DateTime fromDate = Convert.ToDateTime(fromDate1.ToString("MMM dd yyyy")); // Convert.ToDateTime(fromDate1.ToString("MM dd yyyy"));
            //DateTime toDate =Convert.ToDateTime(toDate1.ToString("MMM dd yyyy")); //Convert.ToDateTime(toDate1.ToString("MM dd yyyy"));

            OrderRepository ordRep = new OrderRepository();
            int LoggedInAgentId = ordRep.GetAgentID();

            using (db = new MWVDBContext())
            {
                try
                {
                    List<Order> lstOrder = new List<Order>();

                    switch (SelectedStatusValue)
                    {
                        case "All-Orders":

                            lstOrder = (from d in db.Orders
                                        where (d.order_date >= fromDate) && (d.order_date <= toDate) && d.agent_id == LoggedInAgentId
                                        orderby d.order_date descending
                                        select d).ToList<Order>();
                            break;

                        case "Under-Planning":
                            SelectedStatusValue = SelectedStatusValue.Replace('-', ' ');
                            lstOrder = (from d in db.Orders
                                        where (d.status == SelectedStatusValue && (d.order_date >= fromDate) && (d.order_date <= toDate) && d.agent_id == LoggedInAgentId)
                                        orderby d.order_date descending
                                        select d).ToList<Order>(); ;
                            break;

                        case "Dispatched":

                            lstOrder = (from d in db.Orders
                                        where (d.status == SelectedStatusValue && (d.order_date >= fromDate) && (d.order_date <= toDate) && d.agent_id == LoggedInAgentId)
                                        orderby d.order_date descending
                                        select d).ToList<Order>();
                            break;

                        case "Planned":

                            lstOrder = (from d in db.Orders
                                        where (d.status == SelectedStatusValue && (d.order_date >= fromDate) && (d.order_date <= toDate) && d.agent_id == LoggedInAgentId)
                                        orderby d.order_date descending
                                        select d).ToList<Order>();
                            break;

                        case "In-Warehouse":
                            SelectedStatusValue = SelectedStatusValue.Replace('-', ' ');
                            lstOrder = (from d in db.Orders
                                        where (d.status == SelectedStatusValue && (d.order_date >= fromDate) && (d.order_date <= toDate) && d.agent_id == LoggedInAgentId)
                                        orderby d.order_date descending
                                        select d).ToList<Order>();
                            break;
                    }

                    var lstOrderbyAgent = lstOrder.ToList();
                    //return lstOrder;
                    return lstOrderbyAgent;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Order->OrdersSearchResults:", Ex);
                    return null;
                }
            }
        }
        #endregion

        #region OrdersSearchResultsByOrderId
        public List<Order> OrdersSearchResultsByOrderId(string SelectedStatusValue, int poNumber)
        {
            OrderRepository ordRep = new OrderRepository();
            int LoggedInAgentId = ordRep.GetAgentID();

            try
            {
                List<Order> lstOrder = new List<Order>();


                lstOrder = (from d in db.Orders
                            where (d.order_id == poNumber) && d.agent_id == LoggedInAgentId
                            orderby d.order_date descending
                            select d).Take(10).ToList<Order>();

                return lstOrder.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Order->OrdersSearchResults:", Ex);
                return null;
            }

        }
        #endregion

        public decimal GetProductsAvailableQty(decimal? prodid)
        {
            var list = db.Order_products.Where(p => p.order_product_id == prodid).SingleOrDefault();
            var qtyScheduled = list.qty_scheduled;

            TempJumboDetails ttdObj = new TempJumboDetails();

            var sumOfqty = from tdd in db.Truck_dispatch_details
                           where //tdd.order_id == items.order_id &&
                           tdd.order_product_id == prodid
                           group tdd by 1 into g
                           select new
                           {
                               SumTotal = g.Sum(x => x.qty)
                           };
            var pendingQty = sumOfqty.SingleOrDefault();
            decimal? availableQty = 0;
            if (pendingQty == null)
            {
                availableQty = qtyScheduled - 0;
            }
            else
            {
                availableQty = qtyScheduled - pendingQty.SumTotal;// scheduled qty of particular product - sum of qty  
            }
            //var qtyPlannedByAgent = items.qty_planned_byAgent;

            if (availableQty > 0)
            {
                decimal availableqty = availableQty.Value;
                return availableqty;
            }

            //var qtyPlannedByAgent = list.qty_planned_byAgent;
            //var availableQty = qtyScheduled - qtyPlannedByAgent;

            return 0;
        }

        #region DeleteOrder
        // this function adds the order to the database
        // <returns the order id>
        public void DeleteOrder(int orderid)
        {
            Order order = db.Orders.Find(orderid);

            // Query the database for the rows to be deleted. 
            var deleteOrderDetails =
                from orderproducts in db.Order_products
                where orderproducts.order_id == orderid
                select orderproducts;
            var deleteOrderDispatchDetails = from odd in db.Truck_dispatch_details
                                             where odd.order_id == orderid
                                             select odd;
            if (deleteOrderDispatchDetails.ToList().Count != 0)
            {
                foreach (Truck_dispatch_details tdd in deleteOrderDispatchDetails)
                {
                    db.Truck_dispatch_details.Remove(tdd);
                }
            }
            if (deleteOrderDetails.ToList().Count != 0)
            {
                foreach (var prod in deleteOrderDetails)
                {
                    db.Order_products.Remove(prod);
                }
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                // Provide for exceptions.
            }

            db.Orders.Remove(order);
            db.SaveChanges();
        }
        #endregion

        public IEnumerable<tempOrderProducts> GetProductsByOrderId(int? orderid)
        {
            using (db = new MWVDBContext())
            {
                try
                {

                    //   var ProductList = db.Order_products.Where(p => p.order_id == orderid).ToList();

                    var ProductList = (from ord in db.Order_products
                                       join prods in db.Products on ord.product_code equals prods.product_code
                                       where ord.order_id == orderid
                                       select new tempOrderProducts
                                        {
                                            bf_code = prods.bf_code,
                                            gsm_code = prods.gsm_code,
                                            shade_code = ord.shade_code,
                                            amount = ord.amount,
                                            core = ord.core,
                                            created_by = ord.created_by,
                                            diameter = ord.diameter,
                                            order_id = ord.order_id,
                                            order_product_id = ord.order_product_id,
                                            product_code = ord.product_code,
                                            qty = ord.qty,
                                            width = ord.width,
                                            unit_price = ord.unit_price,
                                            requested_delivery_date = ord.requested_delivery_date
                                        }).ToList();
                    return ProductList;

                }
                catch (Exception Ex)
                {
                    logger.Error("Error in OrderRepository->GetOrders:", Ex);
                    return null;
                }
            }

        }

        #region AddOrder_product
        // this function adds the order product to the database
        // <returns the order product id>
        public int AddOrder_product(Order_Products orderproduct)
        {

            try
            {
                // add to the database
                // return the order id
                db.Order_products.Add(orderproduct);
                db.SaveChanges();
                // query the db again to get the id of the last inserted record
                //var LastInsertedId = db.Orders.Where(p=> p.order_date == DateTime.Now.Date).SingleOrDefault();
                int lastinsertedId = orderproduct.order_product_id;
                // and return that

                return lastinsertedId;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Order->AddOrder:", Ex);
                return 0;
            }

        }
        #endregion

        public int GetAgentID()
        {
            try
            {
                string id = HttpContext.Current.User.Identity.GetUserId();
                // get the agent_id from 'Agents' table with this user id
                //var Agentid = db.Agents.Where(b => b.aspnetusers_id == id).Single();
                //// (from a in db.Agents where a.aspnetusers_id == id select a.agent_id).SingleOrDefault();
                //return Agentid.agent_id;
                int Agentid = (from a in db.Agents where a.aspnetusers_id == id select a.agent_id).SingleOrDefault();
                return Agentid;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->GetAgentID:", Ex);
                throw Ex.InnerException;
                // return 0;
            }

        }

        public string GetAgentName()
        {
            string id = HttpContext.Current.User.Identity.GetUserId();
            // get the agent_name from 'Agents' table with this user id
            string Agentname = (from a in db.Agents where a.aspnetusers_id == id select a.name).SingleOrDefault();
            return Agentname;
        }

        public List<Order> GetAllOrdersbyLocation(string location)
        {
            int LoggedAgentId = GetAgentID();

            // get all papermills in this location
            var papermills = db.Papermills.Where(p => p.location == location).ToList();
            //int pm_id1 = papermills[0].papermill_id;
            //int pm_id2 = papermills[1].papermill_id;

            var orders = db.Orders.Where(p => p.agent_id == LoggedAgentId);

            return orders.ToList();
        }
        public List<Papermill> GetPapermills()
        {
            try
            {
                var lstPapermills = db.Papermills
                                    .OrderByDescending(l => l.papermill_id)
                    //.AsEnumerable()
                                     .GroupBy(x => x.location)
                                     .Select(x => x.FirstOrDefault());
                // to do - difference in this query and getting distinct values???

                return lstPapermills.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->GetPapermills:", Ex);
                return null;
            }
        }
        public List<Order> GetAllOrdersbyAgentandLocation(int loc_id)
        {
            int LoggedAgentId = GetAgentID();

            // query the papermill table with this location id and get the location column
            string papermill_location = GetPapermillLocation(loc_id).Trim();
            // get all papermills in this location
            var papermills = db.Papermills.Where(p => p.location == papermill_location).ToList();
            int pm_id1 = papermills[0].papermill_id;
            int pm_id2 = papermills[1].papermill_id;

            //var orders = db.Orders.Where(p => p.agent_id == LoggedAgentId && (p.papermill_id == pm_id1 || p.papermill_id == pm_id2));
            var orders = (from ord in db.Orders
                          join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                          join pro in db.Products on ordPro.product_code equals pro.product_code
                          join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                          where ord.agent_id == LoggedAgentId &&
                          (sch.papermill_id == pm_id1 || sch.papermill_id == pm_id2)
                          select ord).ToList();

            return orders.ToList();
        }

        public List<Order_Products> GetOrderProducts(int orderid)
        {
            try
            {
                // get a list of all orders 
                // join with order table and select orders where orderid matches request
                orderid = 3;
                var ProductList = db.Order_products.Where(p => p.order_id == orderid).ToList<Order_Products>();

                return ProductList;

            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->GetOrders:", Ex);
                return null;
            }

        }

        public List<Bf> GetBfs()
        {

            try
            {
                var lstBfs = db.Bfs;

                //var bfs = db.Products; //.Select(p => p.bf_code.Distinct())
                //var ss = db.Products.Select(p => p.bf_code.Distinct());



                return lstBfs.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->GetBfs:", Ex);
                return null;
            }

        }

        public List<Gsm> GetGsms()
        {

            try
            {
                var lstGsms = db.Gsms;
                return lstGsms.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->GetGsms:", Ex);
                return null;
            }

        }
        public List<Core> GetCores()
        {
            var lstCores = db.Cores;
            return lstCores.ToList();
        }
        public List<Shade> GetShades()
        {

            try
            {
                var lstShades = db.Shades;

                return lstShades.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->GetShades:", Ex);
                return null;
            }

        }

        public string GetPapermillLocation(int papermillid)
        {
            var strPapermill = db.Papermills
                .Where(p => p.papermill_id == papermillid).SingleOrDefault();
            return strPapermill.location;
        }



        public List<Papermill> GetAllPapermills()
        {
            try
            {
                var lstPapermills = db.Papermills
                                    .OrderByDescending(l => l.papermill_id);
                //.AsEnumerable()
                //.GroupBy(x => x.location)
                //.Select(x => x.FirstOrDefault());
                // to do - difference in this query and getting distinct values???

                return lstPapermills.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->GetPapermills:", Ex);
                return null;
            }
        }

        public string GetProductCode(string bf_code, string gsm_code)
        {
            List<Product> product = db.Products.Where(p => p.gsm_code == gsm_code && p.bf_code == bf_code).ToList();
            return product[0].product_code;
        }
        //Added by manisha
        public decimal GetProductCodebyCustId(int custid, string bf_code, string gsm_code, string shade_code)
        {
            List<Product_prices> price = new List<Product_prices>();

            if (bf_code != "" && gsm_code != "")
            {
                List<Product> product = db.Products.Where(p => p.gsm_code == gsm_code && p.bf_code == bf_code).ToList();
                var productCode = product[0].product_code;
                price = db.Product_prices.Where(p => p.product_code == productCode
                   && (DateTime.Now > p.start_date && DateTime.Now < p.end_date)
                   && p.customer_id == custid && p.shade_code == shade_code).ToList();
            }

            if (price.Count != 0)
            {
                //  return price[0].unit_price;
                return Math.Round(price[0].unit_price, 2);
            }
            else
                return 0;
        }

        public string GetProductDesc(string bf_code, string gsm_code)
        {
            List<Product> product = db.Products.Where(p => p.gsm_code == gsm_code && p.bf_code == bf_code).ToList();
            return product[0].description;
        }

        public decimal GetPrice(int custid, string product_code, string shade_code)
        {
            // get the unit price from 'Product_prices table for a particular range
            List<Product_prices> price = db.Product_prices.Where(p => p.product_code == product_code && (DateTime.Now > p.start_date && DateTime.Now < p.end_date) && p.customer_id == custid && p.shade_code == shade_code).ToList();
            if (price.Count != 0)
            {
                return price[0].unit_price;
            }
            else
                return 0;
        }

        public List<Order> GetCurrentCustOrders(int agentid, int custid, DateTime fromDate, DateTime toDate)
        {

            var lstOrders = db.Orders.Where(t => t.customer_id == custid && t.agent_id == agentid && t.order_date >= fromDate && t.order_date <= toDate);
            return lstOrders.ToList();
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

        public class tempPapermillInfoView
        {
            public int papermill_id { get; set; }
            public string location { get; set; }
            public string address { get; set; }
        }

        // temp order class to hold the order created by the customer on the client side
        public class tempOrder
        {
            [Key]
            public int order_id { get; set; }

            public Agent Agent { get; set; } // for getting the name of the agent to create the order in the database

            //public Nullable<DateTime> order_date { get; set; }

            public string customerpo { get; set; }
            public Nullable<int> agent_id { get; set; }


            public Nullable<int> customer_id { get; set; }
            public string status { get; set; }
            public string agentname { get; set; }
            public string customername { get; set; }


            public Nullable<DateTime> requested_delivery_date { get; set; }

            // list of order products
            public IList<tempOrderProducts> lstProducts { get; set; }
            public Papermill papermills { get; set; }
        }

        public class tempOrderProducts
        {
            [Key]
            public int order_product_id { get; set; }
            public int order_id { get; set; }
            public int flagProduct { get; set; }
            // to show in the edit product page
            public string bf_code { get; set; }
            public string gsm_code { get; set; }

            public string product_code { get; set; }
            public string description { get; set; }

            public string shade_code { get; set; }
            public Nullable<decimal> width { get; set; }
            public Nullable<decimal> unit_price { get; set; }
            public DateTime? requested_delivery_date { get; set; }

            public Nullable<decimal> qty { get; set; }


            public Nullable<decimal> amount { get; set; }
            public string status { get; set; }
            public string created_by { get; set; }

            public Nullable<decimal> diameter { get; set; }
            public int? core { get; set; }

            public int sequenceNumber { get; set; }
            public decimal? widthInInch { get; set; }
        }

        public List<Order> OrderPdf(int id)
        {
            var orderDetails = (from o in db.Orders where o.order_id == id select o);

            //var orderProductsDetails = (from r in db.Order_products
            //                            where r.order_id == id
            //                            select r);

            //var totaLAmount = (from r in db.Order_products
            //                   where r.order_id == id
            //                   select r.amount).Sum();

            return orderDetails.ToList();

        }

        public List<Order_Products> OrderProductsPdf(int id)
        {
            var orderProductsDetails = (from r in db.Order_products
                                        where r.order_id == id
                                        select r);
            return orderProductsDetails.ToList();

        }

        public decimal Amount(int id)
        {
            var totaLAmount = (from r in db.Order_products
                               where r.order_id == id
                               select r.amount).Sum();
            return Convert.ToDecimal(totaLAmount);

        }

        public int AddOrderfromCSV(Order order)
        {
            try
            {
                Order objOrder = new Order();
                objOrder.order_date = order.order_date;
                objOrder.agent_id = order.agent_id;
                objOrder.customer_id = order.customer_id;
                objOrder.status = order.status;
                objOrder.requested_delivery_date = order.requested_delivery_date;
                objOrder.remarks = order.remarks;
                objOrder.amount = order.amount;
                objOrder.papermill_id = order.papermill_id;
                objOrder.created_on = DateTime.Now;
                objOrder.created_by = order.created_by;
                objOrder.modified_on = order.modified_on;
                objOrder.modified_by = order.modified_by;
                objOrder.customerpo = order.customerpo;

                db.Orders.Add(objOrder);
                db.SaveChanges();
                int ordId = objOrder.order_id;

                return ordId;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->AddOrderfromCSV:", Ex);
                return 0;
            }
        }

        public int AddOrderProductsfromCSV(Order_Products orderproducts)
        {
            int ordId = 0;
            List<int> objOrderid = new List<int>();
            try
            {
                db.Order_products.Add(orderproducts);
                ordId = db.SaveChanges();
                return ordId;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->AddOrderProductsfromCSV:", Ex);
                objOrderid.Add(0);
                return 0;
            }

        }

        public int GetCustomerbyName(string CustomerName, int AgentID)
        {
            try
            {
                int CustomerID = (from a in db.Customers where a.name == CustomerName && a.status == "Approved" && a.agent_id == AgentID select a.customer_id).FirstOrDefault();
                return CustomerID;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->GetCustomerbyName:", Ex);
                return 0;
            }
        }
        public int GetAgentbyName(string AgentName)
        {
            try
            {
                int AgentID = (from a in db.Agents where a.name == AgentName select a.agent_id).SingleOrDefault();
                return AgentID;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->GetAgentbyName:", Ex);
                return 0;
            }
        }

        public string GetProductCodeCSV(string bf, string gsm)
        {
            try
            {
                string prod_Code = (from pr in db.Products where pr.bf_code == bf && pr.gsm_code == gsm select pr.product_code).SingleOrDefault();
                return prod_Code;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->GetOroductCode:", Ex);
                return null;
            }

        }

        public decimal GetUnitPrice(int cust_Id, string ProductCode)
        {
            try
            {
                decimal unit_price = (from pp in db.Product_prices where pp.customer_id == cust_Id && pp.product_code == ProductCode select pp.unit_price).SingleOrDefault();
                return unit_price;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->GetUnitPrice:", Ex);
                return 0;
            }
        }

        public bool RollbackOrder(int orderId)
        {
            try
            {
                var deleteOrder = db.Order_products.Where(k => k.order_id == orderId).SingleOrDefault();
                db.Order_products.Remove(deleteOrder);
                db.SaveChanges();

                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->RollbackOrder:", Ex);
                return false;
            }
        }
        public int CheckOrderInTable(int ordID)
        {
            int flag = 0;
            try
            {

                int query = (from or in db.Orders join op in db.Order_products on or.order_id equals op.order_id where op.order_id == ordID select new { op.order_id }).Count();
                if (query == 0)
                {
                    var DeleteObj = db.Orders.Where(j => j.order_id == ordID).SingleOrDefault();
                    db.Orders.Remove(DeleteObj);
                    db.SaveChanges();
                    flag = 1;
                }

                return flag;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->CheckOrderInTable:", Ex);
                return flag;
            }

        }



        public bool Updatetotalamt(decimal Totalamt, int ordid)
        {
            try
            {
                var query = db.Orders.Where(p => p.order_id == ordid).FirstOrDefault();
                if (query != null)
                {
                    query.amount = Totalamt;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->Updatetotalamt:", Ex);
                return false;
            }
        }

        public string GetAgentNamePdf(int id)
        {

            // get the agent_name from 'Agents' table with this user id
            string Agentname = (from a in db.Agents join o in db.Orders on a.agent_id equals o.agent_id where o.order_id == id select a.name).SingleOrDefault();
            return Agentname;
        }

        public Boolean MatchedOrderStatusWithOrdPro(int orderid, int ordProId)
        {
            if (orderid != 0)
            {
                var orderList = from ord in db.Orders
                                where ord.order_id == orderid
                                select ord;
                foreach (var items in orderList)
                {

                    var proList = (from op in db.Order_products
                                   where op.order_id == items.order_id
                                   && op.status == items.status
                                   select op).ToList();
                    if (proList.Count() == 0)
                    {
                        var orderProductStatus = (from op in db.Order_products
                                                  where op.order_id == items.order_id
                                                  select op).ToList();
                        items.status = orderProductStatus.Select(x => x.status).FirstOrDefault();
                        db.SaveChanges();
                    }
                    return true;
                }


            }
            else
            {
                var proList = (from op in db.Order_products
                               where op.order_product_id == ordProId
                               select op).SingleOrDefault();
                var orderProList = from ord in db.Orders
                                   join op in db.Order_products on ord.order_id equals op.order_id
                                   where ord.order_id == proList.order_id
                                     && ord.status == op.status
                                   select op;

                if (orderProList.Count() == 0)
                {
                    var orderProductStatus = (from ord in db.Orders
                                              where ord.order_id == proList.order_id
                                              select ord).SingleOrDefault();

                    var orderProstatus = (from op in db.Order_products
                                          where op.order_product_id == ordProId
                                          select op.status).SingleOrDefault();

                    orderProductStatus.status = orderProstatus;
                    db.SaveChanges();
                    if (orderProductStatus.status == "Dispatched")
                    {
                        MessageforOrderDispathed(orderProductStatus.order_id);
                }
                }
                return true;
            }
            return false;
        }

        public Boolean ChecOrderQty(int orderProId)
        {

            var sumofQty = (from pc in db.ProductionChild
                            where pc.order_product_id == orderProId
                            && pc.actual_end != null
                            select pc.qty).Sum();
            //return Convert.ToDecimal(totaLAmount);


            var orderedQty = (from op in db.Order_products
                              where op.order_product_id == orderProId
                              select op).SingleOrDefault();
            if ((decimal)sumofQty == orderedQty.qty)
            {
                orderedQty.status = "In warehouse";
                db.SaveChanges();

            }

            return false;
        }

        public bool MessageforOrderDispathed(int ordId)
        {
            try
            {
                MessageAndAlertsBusiness objmail = new MessageAndAlertsBusiness();
                int? agntid = db.Orders.Where(k => k.order_id == ordId).Select(f => f.agent_id).SingleOrDefault();
                objmail.CreateMessagesDetails("OrderCompleted", "Agent", agntid, "", "", "", "", "", "", "", "", null, ordId);
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderRepository->MessageforOrderDispathed:", Ex);
                return false;
            }
        }
    }
}
