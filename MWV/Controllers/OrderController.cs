using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MWV.Models;
using MWV.DBContext;
using Microsoft.AspNet.Identity;
using MWV.Repository.Implementation;
using System.Web.Routing;
using System.Globalization;
using System.Collections.Generic;
using PagedList;
using PagedList.Mvc;
using MWV.ViewModels;
using MWV.Business;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Security.Permissions;



namespace MWV.Controllers
{

    public class OrderController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();
        [HandleModelStateException]
        protected override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            // your code here
            if (!Request.IsAuthenticated)
            {
                this.ModelState.AddModelError("440", "Session Timeout");
                throw new ModelStateException(this.ModelState);

            }
        }
        // GET: /Order/
        [Authorize(Roles = "SuperAdmin, MWVAdmin, Agent")]
        [HandleModelStateException]
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Agent).Include(o => o.Customer);
            //(from d in db.Orders where d.status == "Approved" select d).FirstOrDefault();
            return View(orders);
        }
        [HandleModelStateException]
        public ActionResult OrderSearch()
        {
            return View();
        }

        // GET: /Order/Details/5
        [HandleModelStateException]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }


        ////[HttpPost]
        [HandleModelStateException]
        public PartialViewResult OrdersSearchResults(int? page)
        {
            try
            {
                DateTime fromdt = new DateTime();
                DateTime todt = new DateTime();
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                //DateTime fromDate = Convert.ToDateTime(Request["FromDateTime"]);
                //DateTime todate = Convert.ToDateTime(Request["ToDateTime"]);
                //string SelectedValue = Request["SelectedOrderType"];
                string SelectedStatusValue = "";
                //using time as "23:59:59" instead of 00:00:00
                // DateTime toDate = Convert.ToDateTime(Request["ToDateTime"]).AddHours(23).AddMinutes(59).AddSeconds(59);
                if (Session["fromDate"] == null && Session["toDate"] == null && Session["SelectedStatusValue"] == null)
                {
                    Session["fromDate"] = Convert.ToDateTime(Request["FromDateTime"]);
                    Session["toDate"] = Convert.ToDateTime(Request["ToDateTime"]);
                    Session["SelectedStatusValue"] = Request["SelectedOrderType"];
                    fromdt = Convert.ToDateTime(Session["fromDate"]);
                    todt = Convert.ToDateTime(Session["toDate"]);
                    SelectedStatusValue = Session["SelectedStatusValue"].ToString();
                }
                else
                {
                    fromdt = Convert.ToDateTime(Session["fromDate"]);
                    todt = Convert.ToDateTime(Session["toDate"]);
                    SelectedStatusValue = Session["SelectedStatusValue"].ToString();
                }

                List<Order> lstOrder;
                OrderRepository ordObj = new OrderRepository();
                if (SelectedStatusValue == "Purchase-Order-Number")
                {
                    int poNumber = Convert.ToInt16(Request["poNumber"]);
                    lstOrder = ordObj.OrdersSearchResultsByOrderId(SelectedStatusValue, poNumber);

                }
                else
                {
                    fromdt = Convert.ToDateTime(Session["fromDate"]);
                    todt = Convert.ToDateTime(Session["toDate"]);
                    lstOrder = ordObj.OrdersSearchResults(SelectedStatusValue, fromdt, todt);

                }

                if (!lstOrder.Any())
                {
                    ViewBag.noRecordMsg = "No data available !";
                }
                ViewBag.lstOrder = lstOrder.ToPagedList(pageNumber, pageSize);
                ViewBag.Pagesize = lstOrder.Count;

            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderController->OrdersSearchResults:", Ex);
                return null;//fail to edit
            }


            return PartialView("_OrdersSearchResults");
        }
        [HandleModelStateException]
        public void ClearSessionOnOrdersSearch()
        {
            Session["fromDate"] = null;
            Session["toDate"] = null;
            Session["SelectedStatusValue"] = null;
        }
        [HandleModelStateException]
        public decimal GetProductsAvailableQty(decimal id)
        {
            OrderRepository objOrd = new OrderRepository();
            // var list = db.Order_products.Where(p => p.order_product_id == id).SingleOrDefault();
            //  Session["ordPrdId"] = null;
            Session["ordPrdId"] = id;
            //int usedId = Convert.ToInt16(Session["ordPrdId"]);
            // int[] argusedId = usedId.ToString().Cast<int>().ToArray();
            decimal availableqty = objOrd.GetProductsAvailableQty(id);
            return availableqty;
        }
        [HandleModelStateException]
        public PartialViewResult GetOrderDetails(int id)
        {

            var ordSearch = (from Order ord in db.Orders

                             join op in db.Order_products on ord.order_id equals op.order_id
                             join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                             join Papermill pm in db.Papermills on sch.papermill_id equals pm.papermill_id
                             where ord.order_id == id
                             select new OrderRepository.tempPapermillInfoView
                             {
                                 papermill_id = pm.papermill_id,
                                 location = pm.location,
                                 address = pm.address
                             }
                             ).Distinct();
            int selectedpm = ordSearch.Select(p => p.papermill_id).SingleOrDefault();
            TempData["selectedPapermillId"] = selectedpm;
            return PartialView("_OrderDetailsArrNewTransportation", ordSearch.ToList());
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [HandleModelStateException]
        public JsonResult GetAllOrdersbyLocation()
        {
            Session["filterList"] = null;
            OrderRepository objOrder = new OrderRepository();

            int pmid = Convert.ToInt16(TempData["selectedPapermillId"]);
            var lstOrders = objOrder.GetAllOrdersbyAgentandLocation(pmid);

            //var lstGsms = db.Products.Where(k => k.bf_code == bf).ToList();
            //if (lstOrders.Count != 0)
            //{
            var result = (from s in lstOrders
                          select new
                          {
                              id = s.order_id,
                              name = s.order_id
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        [HandleModelStateException]
        public JsonResult GetAllOrdersbyLocationId(int id)
        {
            Session["filterList"] = null;
            OrderRepository objOrder = new OrderRepository();

            int pmid = id;
            var lstOrders = objOrder.GetAllOrdersbyAgentandLocation(pmid);

            //var lstGsms = db.Products.Where(k => k.bf_code == bf).ToList();
            //if (lstOrders.Count != 0)
            //{
            var result = (from s in lstOrders
                          select new
                          {
                              id = s.order_id,
                              name = s.order_id
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        //[AcceptVerbs(HttpVerbs.Get)]
        //public JsonResult GetAllOrdersbyLocationName(string locationName)
        //{
        //    //string locationName = Request["pmlocation"];
        //    OrderRepository objOrder = new OrderRepository();


        //    var lstOrders = objOrder.GetAllOrdersbyLocationName(locationName);

        //    //var lstGsms = db.Products.Where(k => k.bf_code == bf).ToList();
        //    //if (lstOrders.Count != 0)
        //    //{
        //    var result = (from s in lstOrders
        //                  select new
        //                  {
        //                      id = s.order_id,
        //                      name = s.order_id
        //                  }).ToList();
        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}
        // GET: /Order/Create
        [HandleModelStateException]
        public ActionResult Create()
        {
            ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name");
            //  instead the logged in user is the agent, so display his id in front of the label of Agent ID

            string id = User.Identity.GetUserId();
            int LoggedInAgent_id;

            // get the agent_id from 'Agents' table with this user id

            LoggedInAgent_id = (from a in db.Agents where a.aspnetusers_id == id select a.agent_id).SingleOrDefault();

            ViewBag.Agent_id = LoggedInAgent_id;
            var Customers = db.Customers.Where(c => c.agent_id == LoggedInAgent_id && c.status == "Approved").ToList();
            //from c in db.Customers where c.agent_id == LoggedInAgent_id select c.name ;

            // ViewBag.customer_id = Customers[0].customer_id;
            ViewBag.customer_list = new SelectList(Customers, "customer_id", "name");
            return View();
        }

        //#region

        //        // POST: /Order/Create
        //        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //        [HttpPost]
        //        [ValidateAntiForgeryToken]
        //        [Authorize(Roles = "Agent")]
        //        // sStatus = "Created"     
        //        public ActionResult Create([Bind(Include = "order_date,agent_id,customer_id,status,requested_delivery_date,remarks,amount,papermill_id")] Order order)
        //        {
        //            order.agent_id = GetAgentID();
        //            order.customer_id = Convert.ToInt16(Request["customer_list"]);
        //            order.requested_delivery_date = Convert.ToDateTime(Request["requested_delivery_date"]);

        //            order.order_date = DateTime.Now;
        //            order.status = "Created";


        //            if (ModelState.IsValid)
        //            {
        //                // pass the order object to the repository class OrderRepository.cs 's AddOrder() method with the 'order' object

        //                OrderRepository objOrder = new OrderRepository();
        //                int OrderID = objOrder.AddOrder(order);
        //                // return RedirectToAction("Edit");

        //                ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name", order.agent_id);
        //                ViewBag.customer_id = new SelectList(db.Customers, "customer_id", "name", order.customer_id);

        //                return RedirectToAction("Details", new RouteValueDictionary(
        //                    new { controller = "Order", action = "Details", Id = OrderID }));
        //            }
        //            else
        //                return View(order);
        //        }

        //#endregion



        // GET: /Order/Edit/5
        [HandleModelStateException]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // get the associate order products also with this order
            //Order order = db.Orders.Find(id);
            Order order = db.Orders.Include(i => i.OrderProducts).Where(j => j.order_id == id).Single();

            ViewBag.prodlist = new SelectList(order.OrderProducts, "order_product_id", "product_code");

            // PopulateRelatedOrderProducts(order); not needed

            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name", order.agent_id);
            ViewBag.customer_id = new SelectList(db.Customers, "customer_id", "name", order.customer_id);
            ViewBag.LastInsertedId = id;
            return View(order);
            // instead show the order  products list i.e.redirect to the index action method of order_products

            //return RedirectToAction("Index", new RouteValueDictionary(
            //    new { controller = "Order_product", action = "Index"}));
        }

        // POST: /Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "order_id,order_date,agent_id,customer_id,status,requested_delivery_date,remarks,amount")] Order order)
        {
            var ord = db.Orders.Find(order.order_id);
            ord.requested_delivery_date = Convert.ToDateTime(Request["requested_delivery_date"]);

            if (ModelState.IsValid)
            {
                //db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                // return RedirectToAction("Index");
            }
            ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name", order.agent_id);
            ViewBag.customer_id = new SelectList(db.Customers, "customer_id", "name", order.customer_id);
            //return View(ord);
            return RedirectToAction("Index");
        }

        // GET: /Order/Delete/5
        [HandleModelStateException]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: /Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderRepository or = new OrderRepository();
            or.DeleteOrder(id);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        //public ActionResult AddProducts() { }

        //// GET: /Order_product/
        //public ActionResult ShowOrderProducts(int OrderId)
        //{
        //   //var order_products = db.Order_products.Include(o => o.order).Include(o => o.Product).Include(o => o.Shade).Where(p=> p.order_id==OrderId);
        //    var order_products = db.Order_products.Where(p => p.order_id == OrderId);
        //  //return View(order_products.ToList());
        //    return PartialView("Partial3", order_products);
        //}

        //public ActionResult ShowAllOrderProducts()
        //{
        //    var order_products = db.Order_products.Include(o => o.order).Include(o => o.Product).Include(o => o.Shade);
        //    // return View(order_products.ToList());
        //    return PartialView("Partial3", order_products);
        //}

        [HandleModelStateException]
        public ActionResult GenerateDuplicateOrder(int? id)
        {
            try
            {

                int? orderid;
                if (id == null)
                {
                    orderid = (int)Session["LastInsertedOrder_id"];
                }
                else
                {
                    orderid = id;
                }
                // get the latest order created 
                var Order = db.Orders.Find(orderid);//db.Orders.OrderByDescending(m => m.order_id).Take(1).ToList();
                OrderRepository.tempOrder objTempOrder = new OrderRepository.tempOrder();
                objTempOrder.agent_id = Order.agent_id;
                objTempOrder.Agent = Order.Agent;
                objTempOrder.agentname = Order.Agent.name;
                objTempOrder.customer_id = Order.customer_id;
                objTempOrder.customername = Order.Customer.name;
                objTempOrder.customerpo = Order.customerpo;
                objTempOrder.order_id = Order.order_id;
                objTempOrder.papermills = Order.papermill;
                objTempOrder.requested_delivery_date = Order.requested_delivery_date;
                objTempOrder.status = "A";
                // objTempOrder.widthInInch = (Order.width) * Convert.ToDecimal(2.54);

                OrderRepository objOrderRep = new OrderRepository();
                var lstOrderProds = objOrderRep.GetProductsByOrderId(orderid);
                // copy each field one by one to convert this into the type 'OrderRepository.tempOrderProducts'

                List<OrderRepository.tempOrderProducts> lstTempProds = new List<OrderRepository.tempOrderProducts>();
                // get the bf and gsm code from the product code
                //string[] prodCode; 
                int seqNumber = 1;
                // store the header and details in the session without the Ids....
                foreach (var item in lstOrderProds)
                {
                    OrderRepository.tempOrderProducts objTempOrderProd = new OrderRepository.tempOrderProducts();
                    objTempOrderProd.bf_code = item.bf_code;
                    objTempOrderProd.gsm_code = item.gsm_code;
                    objTempOrderProd.amount = item.amount;
                    objTempOrderProd.core = item.core;
                    objTempOrderProd.created_by = item.created_by;
                    objTempOrderProd.diameter = item.diameter;
                    objTempOrderProd.order_id = item.order_id;
                    objTempOrderProd.order_product_id = item.order_product_id;
                    objTempOrderProd.product_code = item.product_code;
                    objTempOrderProd.qty = item.qty;
                    objTempOrderProd.shade_code = item.shade_code;
                    objTempOrderProd.status = "A"; //item.status; 
                    // otherwise the status remains as 'Created' and these products do not get added to the db
                    //  as I am checking the statusas "A" while adding
                    objTempOrderProd.width = item.width;

                    decimal unitprice = Convert.ToDecimal(item.unit_price);
                    //objTempOrderProd.unit_price = item.unit_price;
                    objTempOrderProd.unit_price = Math.Round(unitprice, 2);

                    objTempOrderProd.requested_delivery_date = item.requested_delivery_date;
                    //item.qty_dispatched_byFactory = objTempOrderProd.qty
                    //item.qty_planned_byAgent = objTempOrderProd.
                    objTempOrderProd.order_id = 0;
                    objTempOrderProd.order_product_id = 0;
                    objTempOrderProd.sequenceNumber = seqNumber;
                    objTempOrderProd.widthInInch = (item.width) * Convert.ToDecimal(2.54);
                    lstTempProds.Add(objTempOrderProd);
                    seqNumber = seqNumber + 1;
                }

                Session["tempOrder"] = objTempOrder; //Order;
                Session["tempOrderProds"] = lstTempProds;
                // loading the lists for the edit form
                loadProductFormLists();
                //setting a flag for displaying the customer drop down
                Session["fromDuplicateBtn"] = 1;
                // get the products total
                ViewBag.grandtotal = GetProductsTotal(lstTempProds);
                ViewBag.lstTempProds = lstTempProds;
                return PartialView("_currentProductsList");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderController->GenerateDuplicateOrder:", Ex);
                return null;//fail to edit
            }
        }

        // GET: /Order/Create
        [HandleModelStateException]
        public ActionResult CreateDuplicateOrder(int? Orderid)
        {

            //get the details of this order id
            if (Orderid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // get the associate order products also with this order
            //Order order = db.Orders.Find(id);
            Order order = db.Orders.Include(i => i.OrderProducts).Where(j => j.order_id == Orderid).Single();

            ViewBag.prodlist = new SelectList(order.OrderProducts, "order_product_id", "product_code");

            // PopulateRelatedOrderProducts(order); not needed

            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name", order.agent_id);
            // ViewBag.customer_id = new SelectList(db.Customers, "customer_id", "name", order.customer_id);
            ViewBag.LastInsertedId = Orderid;

            // ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name");
            //  instead the logged in user is the agent, so display his id in front of the label of Agent ID

            string id = User.Identity.GetUserId();
            int LoggedInAgent_id;

            // get the agent_id from 'Agents' table with this user id

            LoggedInAgent_id = (from a in db.Agents where a.aspnetusers_id == id select a.agent_id).SingleOrDefault();

            ViewBag.Agent_id = LoggedInAgent_id;
            var Customers = db.Customers.Where(c => c.agent_id == LoggedInAgent_id && c.status == "Approved").ToList();
            //from c in db.Customers where c.agent_id == LoggedInAgent_id select c.name ;

            // ViewBag.customer_id = Customers[0].customer_id;
            ViewBag.customer_list = new SelectList(Customers, "customer_id", "name");
            // return View();

            return View(order);
        }

        //#region CreateDuplicateOrder
        ////CreateDuplicateOrder
        //// POST: /Order/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Agent")]
        //// sStatus = "Created"     
        //public ActionResult CreateDuplicateOrder([Bind(Include = "order_date,agent_id,customer_id,status,requested_delivery_date,remarks,amount,papermill_id")] Order order)
        //{
        //    order.agent_id = GetAgentID();
        //    order.customer_id = Convert.ToInt16(Request["customer_list"]);
        //    order.requested_delivery_date = Convert.ToDateTime(Request["requested_delivery_date"]);

        //    order.order_date = DateTime.Now;
        //    order.status = "Created";


        //    if (ModelState.IsValid)
        //    {
        //        // pass the order object to the repository class OrderRepository.cs 's AddOrder() method with the 'order' object

        //        OrderRepository objOrder = new OrderRepository();
        //        int OrderID = objOrder.AddOrder(order);
        //        // return RedirectToAction("Edit");

        //        ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name", order.agent_id);
        //        ViewBag.customer_id = new SelectList(db.Customers, "customer_id", "name", order.customer_id);

        //        return RedirectToAction("Details", new RouteValueDictionary(
        //            new { controller = "Order", action = "Details", Id = OrderID }));
        //    }
        //    else
        //        return View(order);
        //}
        //#endregion
        [HandleModelStateException]
        public ActionResult GetAllCreatedOrders()
        {
            var lstOrders = db.Orders.Where(b => b.status == "Created").ToList();
            return View("ProductionPlanner/GetAllCreatedOrders", lstOrders);
        }
        [HttpPost]
        [HandleModelStateException]
        public ActionResult EditOrderToApprove(Order order)
        {
            var ordDetails = (from ord in db.Orders
                              join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                              join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id
                              where ord.order_id == order.order_id
                              select new { ord, ordPro, sch }).SingleOrDefault();
            //db.Orders.Find(order.order_id);
            ordDetails.ord.status = Request["lstStatus"];
            ordDetails.sch.papermill_id = Convert.ToInt16(Request["papermill_id"]);

            if (ModelState.IsValid)
            {
                // db.Entry(order).State = EntityState.Modified; 
                // this above line is not needed as only some column of one record is being updated
                // instead just db.SaveChanges() updates the record
                db.SaveChanges();
            }
            ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name", order.agent_id);
            ViewBag.customer_id = new SelectList(db.Customers, "customer_id", "name", order.customer_id);

            return RedirectToAction("GetAllCreatedOrders");
        }


        [HttpGet]
        [HandleModelStateException]
        public ActionResult EditOrderToApprove(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // get the associate order products also with this order
            //Order order = db.Orders.Find(id);
            Order order = db.Orders.Include(i => i.OrderProducts).Where(j => j.order_id == id).Single();

            ViewBag.prodlist = new SelectList(order.OrderProducts, "order_product_id", "product_code");

            // PopulateRelatedOrderProducts(order); not needed

            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name", order.agent_id);
            ViewBag.customer_id = new SelectList(db.Customers, "customer_id", "name", order.customer_id);
            //ViewBag.papermill_id = new SelectList(db.Papermills, "papermill_id", "name", order.papermill_id);
            ViewBag.lstPapermills = db.Papermills.ToList();
            //ViewBag.LastInsertedId = id;

            return View("ProductionPlanner/EditOrderToApprove", order);
        }

        //GetAllOrdersForLocation
        [AcceptVerbs(HttpVerbs.Get)]
        [HandleModelStateException]
        public JsonResult GetAllOrdersForLocation(int id)
        {
            OrderRepository objOrder = new OrderRepository();
            var truck_dispatches = (from r in db.Truck_dispatches
                                    join td in db.Truck_dispatch_details on r.truck_dispatch_id equals td.truck_dispatch_id
                                    // join or in db.Orders on td.order_id equals or.order_id
                                    join op in db.Order_products on td.order_product_id equals op.order_product_id
                                    join sch in db.Schedule on op.schedule_id equals sch.schedule_id

                                    join pm in db.Papermills on sch.papermill_id equals pm.papermill_id

                                    where r.truck_dispatch_id == id
                                    select new TempTruckDetails
                                    {
                                        truck_dispatch_id = r.truck_dispatch_id,
                                        truck_no = r.truck_no,
                                        pmlocationid = sch.papermill_id,
                                        location = pm.location,
                                        address = pm.address,
                                        created_on = r.created_on,
                                        estimated_capacity = r.estimated_capacity,
                                        agent_dispatched_on = r.agent_dispatched_on,
                                        estimated_arrival = r.estimated_arrival,
                                        actual_arrival_at_gate = r.actual_arrival_at_gate,
                                        left_factory_on = r.left_factory_on,
                                        papermill_id = pm.papermill_id
                                    })
                                    .ToList();

            // get the papermill list
            var Papermills = objOrder.GetAllPapermills();
            //string selectedpmid = truck_dispatches.Select(p=>p.papermill_id).ToString();
            foreach (var item in truck_dispatches)
            {
                ViewBag.papermill_list = new SelectList(Papermills, "papermill_id", "location", item.papermill_id);
            }

            var objs = (from c in truck_dispatches
                        select c).GroupBy(g => g.truck_dispatch_id).Select(x => x.FirstOrDefault());

            //   .Distinct();
            //    .GroupBy(t => new { t.srNo, t.papermillName, t.estimated_start });
            //.OrderByDescending(x => x.srNo);

            List<TempTruckDetails> query = new List<TempTruckDetails>();
            foreach (var items in objs)
            {
                TempTruckDetails obj = new TempTruckDetails();
                obj.truck_dispatch_id = items.truck_dispatch_id;
                obj.truck_no = items.truck_no;
                obj.pmlocationid = items.papermill_id;
                obj.location = items.location;
                obj.address = items.address;
                obj.created_on = items.created_on;
                obj.estimated_capacity = items.estimated_capacity;
                obj.agent_dispatched_on = items.agent_dispatched_on;
                obj.estimated_arrival = items.estimated_arrival;
                obj.actual_arrival_at_gate = items.actual_arrival_at_gate;
                obj.left_factory_on = items.left_factory_on;
                obj.papermill_id = items.papermill_id;
                TempData["pmid"] = obj.papermill_id;
                query.Add(obj);
            }
            //OrderRepository objOrder = new OrderRepository();
            Session["locationid"] = null;
            int locationid = (int)TempData["pmid"];//get it from the truck_dispatch_id
            Session["locationid"] = locationid;
            //var lstOrders = objOrder.GetAllOrdersbyAgentandLocation(locationid);

            //var lstGsms = db.Products.Where(k => k.bf_code == bf).ToList();
            //if (lstOrders.Count != 0)
            //{
            //OrderRepository objOrder = new OrderRepository();
            //  var lstOrders = objOrder.GetAllOrdersbyAgentandLocation(id);
            string papermill_location = objOrder.GetPapermillLocation(locationid).Trim();
            var papermills = db.Papermills.Where(p => p.location == papermill_location).ToList();
            int pm_id1 = papermills[0].papermill_id;
            int pm_id2 = papermills[1].papermill_id;
            int LoggedAgentId = objOrder.GetAgentID();
            Truck_dispatchRepository tdObj = new Truck_dispatchRepository();

            // var lstOrders = db.Orders.Where(p => p.agent_id == LoggedAgentId && (p.papermill_id == firstPapermillId || p.papermill_id == secondPappermil));

            var listOrders = (from Order_Products op in db.Order_products
                              join ord in db.Orders on op.order_id equals ord.order_id

                              join ordPro in db.Order_products on ord.order_id equals ordPro.order_id
                              join sch in db.Schedule on ordPro.schedule_id equals sch.schedule_id

                              where// op.order_id == id
                                  // &&
                              ord.agent_id == LoggedAgentId
                               && (sch.papermill_id == pm_id1 || sch.papermill_id == pm_id2)
                              select new
                              {
                                  product_code = op.product_code,
                                  qty_planned_byAgent = op.qty_planned_byAgent,
                                  qty_scheduled = op.qty_scheduled,
                                  order_id = op.order_id,
                                  customerName = ord.Customer.name,
                                  order_product_id = op.order_product_id
                              }).ToList();


            List<TempJumboDetails> list = new List<TempJumboDetails>();


            foreach (var items in listOrders)
            {
                TempJumboDetails ttdObj = new TempJumboDetails();
                var qtyScheduled = items.qty_scheduled; // scheduled qty of particular product
                var sumOfqty = from tdd in db.Truck_dispatch_details
                               where tdd.order_product_id == items.order_product_id
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
                if (availableQty > 0)
                {
                    ttdObj.product_code = items.product_code;
                    ttdObj.order_product_id = items.order_product_id;
                    ttdObj.order_id = items.order_id;
                    ttdObj.CustomerName = items.customerName;
                    list.Add(ttdObj);
                }
            }

            //foreach (var items in listOrders)
            //{
            //    TempJumboDetails ttdObj = new TempJumboDetails();
            //    var qtyScheduled = items.qty_scheduled;
            //    var qtyPlannedByAgent = items.qty_planned_byAgent;
            //    var availableQty = qtyScheduled - qtyPlannedByAgent;
            //    if (availableQty > 0)
            //    {
            //        ttdObj.product_code = items.product_code;
            //        ttdObj.order_product_id = items.order_product_id;
            //        ttdObj.order_id = items.order_id;
            //        ttdObj.CustomerName = items.customerName;
            //        list.Add(ttdObj);
            //    }
            //}

            var objList = (from c in list
                           orderby c.order_id
                           select c).GroupBy(g => g.order_id).Select(x => x.FirstOrDefault());

            var result = (from s in objList
                          select new
                          {
                              id = s.order_id,
                              name = s.CustomerName
                          }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);

        }
        [HandleModelStateException]
        public void loadProductFormLists()
        {
            //List<OrderRepository.tempOrderProducts> lstTempProds = new List<OrderRepository.tempOrderProducts>();
            //lstTempProds = Session["tempOrderProds"] as List<OrderRepository.tempOrderProducts>; //GetSessionItem<OrderRepository.tempOrderProducts>("tempOrderProds");
            //// query the list to get the item at this id
            //var prod = lstTempProds.Where(p => p.sequenceNumber == id).SingleOrDefault();

            //we need to fill the drop downs of bf and gsm on the partial view so,
            // Get the bf list
            OrderRepository objOrder = new OrderRepository();
            var Bfs = objOrder.GetBfs();
            ViewBag.Bf_list = new SelectList(Bfs, "bf_code", "description");

            // Get the gsm list
            var Gsms = objOrder.GetGsms();
            ViewBag.Gsm_list = new SelectList(Gsms, "gsm_code", "description");

            // Get the bf list
            var Shades = objOrder.GetShades();
            ViewBag.Shade_list = new SelectList(Shades, "shade_code", "description");

            var lstCores = objOrder.GetCores();
            ViewBag.Core_list = new SelectList(lstCores, "core_code", "description");
        }
        [HandleModelStateException]
        public PartialViewResult OrderReview()
        {
            // get the order details from the sesion
            OrderRepository.tempOrder objTempOrder = new OrderRepository.tempOrder();
            objTempOrder = Session["tempOrder"] as OrderRepository.tempOrder;
            // we are getting req_del_date with form post to this method
            // so, overwrite the req_del_date in the session
            // objTempOrder.requested_delivery_date = Convert.ToDateTime(Request["requested_delivery_date"]);

            objTempOrder.customerpo = Request["customerpo"];
            //var customer = (from Customer in db.Customers
            //                join Order in db.Orders on Customer.customer_id equals objTempOrder.customer_id
            //                select Customer).ToList<Customer>();

            //  and  put in ViewBag the session
            Session["custname"] = objTempOrder.customername;
            //ViewBag.req_del_date = objTempOrder.requested_delivery_date;
            ViewBag.customerpo = objTempOrder.customerpo;
            // now put in session because the dashboard contains this div
            //Session["req_del_date"] = objTempOrder.requested_delivery_date;
            Session["customerpo"] = objTempOrder.customerpo;

            List<OrderRepository.tempOrderProducts> lstTempProds = new List<OrderRepository.tempOrderProducts>();
            lstTempProds = Session["tempOrderProds"] as List<OrderRepository.tempOrderProducts>; //GetSessionItem<OrderRepository.tempOrderProducts>("tempOrderProds");

            bool isUnitPriceZero = false;
            foreach (var items in lstTempProds)
            {
                if (items.amount == 0) { isUnitPriceZero = true; }
            }
            string message;
            if (isUnitPriceZero == true)
            {
                // one or more items have zero unit price
                message = "This order has products with zero unit price, so the order can not be added to the db";
            }

            else message = "";

            ViewBag.message = message;

            var currentList = lstTempProds.Where(k => k.status != "D").ToList();
            if (!currentList.Any())
            {
                // ViewBag.NoRecordMsg = "No record found in our database.";
                message = "0";
                ViewBag.NoRecordMsg = 0;
            }
            decimal gtotal;
            //= (decimal)lstTempProds.Sum(item => item.amount);

            ViewBag.grandtotal = GetProductsTotal(currentList);
            // instead put in session as we want it on the dashboard
            // Session["grandtotal"] = gtotal;
            ViewBag.currentList = currentList;
            return PartialView("_OrderReview", currentList);
            // return PartialView("_OrderReview", lstTempProds);
        }

        [HandleModelStateException]
        public PartialViewResult OrderConfirmation()
        {
            // save order and its products which are in Session["tempOrder"] and  Session["tempOrderProds"] 
            // Step 1: Get the tempOrder from session
            OrderRepository objOrderRep = new OrderRepository();
            OrderRepository.tempOrder objTempOrder = new OrderRepository.tempOrder();

            objTempOrder = Session["tempOrder"] as OrderRepository.tempOrder;
            ViewBag.custname = objTempOrder.customername;
            // Step 2: Get the tempProducts from session
            List<OrderRepository.tempOrderProducts> lstTempProds = new List<OrderRepository.tempOrderProducts>();
            if (Session["tempOrderProds"] != null) // session has products
            {
                lstTempProds = Session["tempOrderProds"] as List<OrderRepository.tempOrderProducts>;
            }

            int lastid;
            ///////////////////////////
            //if ((objTempOrder.order_id != null || objTempOrder.order_id != 0) && objTempOrder.status != "A")
            //{
            //    lastid = objOrderRep.UpdateOrder(objTempOrder, lstTempProds);
            //}
            //else
            //{
            lastid = objOrderRep.AddOrder(objTempOrder, lstTempProds);
            // objOrderRep.CreatePdf(objTempOrder,lstTempProds);
            //int LastInsertedOrder_id = objOrderRep.AddOrder(objTempOrder, lstTempProds);
            //}
            List<OrderRepository.tempOrderProducts> currentList = new List<OrderRepository.tempOrderProducts>();

            if (lastid != null)
            {
                currentList = lstTempProds.Where(k => k.status != "D").ToList();
                if (!currentList.Any())
                {
                    ViewBag.NoRecordMsg = "No data available !";
                }
                ViewBag.LastInsertedOrder_id = lastid;
                Session["LastInsertedOrder_id"] = lastid;
                //(decimal)lstTempProds.Sum(item => item.amount);
                ViewBag.grandtotal = GetProductsTotal(currentList);
                //ViewBag.req_del_date = objTempOrder.requested_delivery_date;
                ViewBag.customerpo = objTempOrder.customerpo;

                // destroy the temp session objects
                Session["tempOrder"] = null;
                Session["tempOrderProds"] = null;

                //Added By Sagar for Pdf Create
                Order objorder = new Order();
                QuickViewModel objquickview = new QuickViewModel();
                OrderRepository ores = new OrderRepository();
                objquickview.Order_Products = ores.OrderProductsPdf(lastid).ToList();
                objquickview.OrderDetails = ores.OrderPdf(lastid).ToList();
                var totaLAmount = ores.Amount(lastid);
                List<Order_Products> lst = new List<Order_Products>();
                Order_Products op = new Order_Products();
                foreach (var ord in objquickview.OrderDetails)
                {
                    ViewBag.AgentName = ord.Agent.name;
                    ViewBag.Customername = ord.Customer.name;
                    ViewBag.PurchaseordNo = ord.order_id;
                    lastid = ord.order_id;
                    ViewBag.orderdate = ord.order_date;
                    ViewBag.CustPo = ord.customerpo;
                    ViewBag.totalamt = totaLAmount;
                    foreach (var order_prod in objquickview.Order_Products)
                    {

                        op.requested_delivery_date = order_prod.requested_delivery_date;
                        op.product_code = order_prod.product_code;
                        op.shade_code = order_prod.shade_code;
                        op.width = order_prod.width;
                        op.qty = order_prod.qty;
                        op.amount = order_prod.amount;
                        lst.Add(new Order_Products() { requested_delivery_date = op.requested_delivery_date, product_code = op.product_code, shade_code = op.shade_code, width = op.width, qty = op.qty, amount = op.amount });

                    }

                }
                string agntname = objOrderRep.GetAgentName();
                agntname = agntname.Replace(" ", "_");
                byte[] pdfOutput = ControllerContext.GeneratePdf(lst, "CreateOrderPdf");
                string CurDate = DateTime.Today.ToString("MMM-yyyy", new CultureInfo("en-US"));
                bool exists = System.IO.Directory.Exists(Server.MapPath("~/MWV/PDF/" + CurDate));
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath("~/MWV/PDF/" + CurDate));
                }
                string finalPath = "/MWV/PDF/" + CurDate;
                string fullPath = Server.MapPath(finalPath + "/" + agntname + "_PO_" + lastid.ToString() + ".pdf");
                string attachment1 = "/MWV/PDF/" + CurDate + "/" + agntname + "_PO_" + lastid.ToString() + ".pdf";
                var updateAttachment = db.Orders.FirstOrDefault(k => k.order_id == lastid);
                if (updateAttachment != null)
                {
                    updateAttachment.pdf_file = attachment1;
                    db.SaveChanges();

                }
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                System.IO.File.WriteAllBytes(fullPath, pdfOutput);
                Mailoperation(objOrderRep.GetAgentName(), lastid, objTempOrder.customername, lst, fullPath, objOrderRep.GetAgentID());
            }
            else
            {
                ViewBag.error = "Order could not be saved to the database";
            }
            ViewBag.lstTempProds = currentList;
            // return PartialView("_OrderConfirmation", lstTempProds);
            return PartialView("_OrderConfirmation", currentList);

        }


        private bool Mailoperation(string agentName, int orderNo, string custName, List<Order_Products> lst, string filePath, int agntId)
        {
            try
            {

                CustomerRepository objCR = new CustomerRepository();
                EmailController emailFunc = new EmailController();
                MessageAndAlertsBusiness objmail = new MessageAndAlertsBusiness();
                UserMailer objusm = new UserMailer();
                ViewBag.agntName = agentName;
                ViewBag.orderNo = orderNo;
                ViewBag.customername = custName;
                string attchment = (from o in db.Orders where o.order_id == orderNo select o.pdf_file).SingleOrDefault();
                string PathOffile = Path.Combine(Server.MapPath(attchment));
                string Msgsub = "New order #" + orderNo + " placed";
                string pdfOutput = emailFunc.GeneratePdfOutput(this.ControllerContext, lst, "OrderPlaced");
                string id = User.Identity.GetUserId();
                int LoggedInAgent_id = (from a in db.Agents where a.aspnetusers_id == id select a.agent_id).SingleOrDefault();
                string emailcc = objCR.GetEmail();
                string emailto = objCR.GetAgentmail(LoggedInAgent_id.ToString());
                bool statusofmail = objusm.sendMails(emailto, "", Msgsub, emailcc, "", "", "", pdfOutput, PathOffile, "");
                if (statusofmail == false)
                {
                    objmail.CreateMessagesDetails("OrderPlaced", "Agent", agntId, pdfOutput, Msgsub, null, null, null, agentName, attchment, "Failed", null, null);
                }
                else
                {
                    objmail.CreateMessagesDetails("OrderPlaced", "Agent", agntId, pdfOutput, Msgsub, null, null, null, agentName, attchment, "Deliverd", null, null);

                }
                // objmail.CreateAlertDetails("OrderPlaced", "ProductionPlanner", agntId, "Order no " + orderNo + " Placed", Msgsub, agentName, null);
                return true;

            }
            catch (Exception Ex)
            {
                logger.Error("Error in CreateCustomer->MsgAndMailforCustomerCreated:", Ex);
                return false;

            }


        }

        //  public PartialViewResult OrderConfirmation()
        //  {
        //      // save order and its products which are in Session["tempOrder"] and  Session["tempOrderProds"] 
        //      // Step 1: Get the tempOrder from session
        //      OrderRepository objOrder = new OrderRepository();
        //      OrderRepository.tempOrder objTempOrder = new OrderRepository.tempOrder();

        //      objTempOrder = Session["tempOrder"] as OrderRepository.tempOrder;

        //      // Step 2: Get the tempProducts from session
        //      List<OrderRepository.tempOrderProducts> lstTempProds = new List<OrderRepository.tempOrderProducts>();
        //      if (Session["tempOrderProds"] != null) // session has products
        //      {
        //          lstTempProds = Session["tempOrderProds"] as List<OrderRepository.tempOrderProducts>;
        //      }

        //      if (ViewBag.message !="") 
        //      {
        //       int LastInsertedOrder_id = objOrder.AddOrder(objTempOrder, lstTempProds);
        //       ViewBag.LastInsertedOrder_id = LastInsertedOrder_id;
        //      decimal gtotal = (decimal)lstTempProds.Sum(item => item.amount);
        //          ViewBag.grandtotal = Math.Round(gtotal, 2);
        //          ViewBag.req_del_date = objTempOrder.requested_delivery_date;
        //          // destroy the temp session objects
        //          Session["tempOrder"] = null;
        //          Session["tempOrderProds"] = null;

        //          return PartialView("_OrderConfirmation", lstTempProds);
        //      }
        //      else
        //      {
        //          // return the partial view with a message
        //          return PartialView("_OrderConfirmation", ViewBag.message);
        //      }
        //}

        public void DestroyOrderSession()
        {
            // destroy the temp session objects
            Session["tempOrder"] = null;
            Session["tempOrderProds"] = null;
            Session["grandtotal"] = null;
        }
        [HandleModelStateException]
        public void addOrderinSession()
        {
            OrderRepository objOrderRep = new OrderRepository();
            OrderRepository.tempOrder objtempOrder = new OrderRepository.tempOrder();
            objtempOrder.order_id = 0;
            objtempOrder.agent_id = objOrderRep.GetAgentID();
            int custid = Convert.ToInt16(Request["customer_id"]);
            objtempOrder.customer_id = custid;
            // commented for CR
            // objtempOrder.requested_delivery_date = Convert.ToDateTime(Request["requested_delivery_date"]);

            objtempOrder.customerpo = Request["customerpo"];
            CustomerRepository custRep = new CustomerRepository();
            objtempOrder.customername = custRep.GetCustNameByCustId(custid);

            objtempOrder.agentname = objOrderRep.GetAgentName();
            // commented for CR
            // Session["req_del_date"] = objtempOrder.requested_delivery_date; // to display on the review page

            Session["tempOrder"] = objtempOrder;
            // return Json(Session["tempOrder"], JsonRequestBehavior.AllowGet);  //"order has been created";
            // return PartialView("");
        }
        [HandleModelStateException]
        public void editOrderinSession()
        {
            // Step 1: Get the order which is already in session
            OrderRepository.tempOrder objtempOrder = Session["tempOrder"] as OrderRepository.tempOrder;
            // Step 2: Set the new values entered in the form (in this case it is just the requested delivery date)
            // objtempOrder.requested_delivery_date = ;
            // Step 3: Save the object again in the same session key
            Session["tempOrder"] = objtempOrder;
        }
        [HandleModelStateException]
        public decimal GetProductsTotal(List<OrderRepository.tempOrderProducts> currentList)
        {
            decimal gtotal = (decimal)currentList.Sum(item => item.amount);
            return Math.Round(gtotal, 2);
        }
        [HandleModelStateException]
        public PartialViewResult addProductToOrderinSession(int id)
        {
            int sequenceNum = 0;
            List<OrderRepository.tempOrderProducts> lstTempProds;
            OrderRepository.tempOrderProducts objtempProduct = new OrderRepository.tempOrderProducts();
            // if there are products already in the list in session, 
            // then get them in the list and add more to it
            if (Session["tempOrderProds"] != null)
            {
                lstTempProds = Session["tempOrderProds"] as List<OrderRepository.tempOrderProducts>; //GetSessionItem<OrderRepository.tempOrderProducts>("tempOrderProds");
                if (id > 0)//when user wants to copy product.
                {
                    //Get details of product which  from list
                    //On the basis of SeqNum of product which user want to copy, get details and fill new product obj.
                    var objProductToCopy = lstTempProds.Where(p => p.sequenceNumber == id).SingleOrDefault();

                    objtempProduct.sequenceNumber = lstTempProds.Last().sequenceNumber + 1;
                    objtempProduct.product_code = objProductToCopy.product_code;
                    objtempProduct.shade_code = objProductToCopy.shade_code;
                    objtempProduct.description = objProductToCopy.description;
                    objtempProduct.bf_code = objProductToCopy.bf_code;
                    objtempProduct.gsm_code = objProductToCopy.gsm_code;
                    objtempProduct.width = objProductToCopy.width;
                    //decimal unitprice = objProductToCopy.GetPrice(custid, prodCode, shadecode);
                    objtempProduct.unit_price = objProductToCopy.unit_price;
                    objtempProduct.qty = objProductToCopy.qty;
                    objtempProduct.amount = objProductToCopy.amount;
                    objtempProduct.diameter = objProductToCopy.diameter;
                    objtempProduct.core = objProductToCopy.core;
                    objtempProduct.status = "A"; // indicating that this record is 'Added'
                    objtempProduct.created_by = objProductToCopy.created_by;
                    objtempProduct.requested_delivery_date = objProductToCopy.requested_delivery_date;
                    objtempProduct.unit_price = objProductToCopy.unit_price;
                    objtempProduct.widthInInch = (objProductToCopy.width) * Convert.ToDecimal(2.54);

                }
                sequenceNum = lstTempProds.Last().sequenceNumber + 1;
            }
            // else add the product to the list and pass the list to the partial view
            else
            {
                lstTempProds = new List<OrderRepository.tempOrderProducts>();
                sequenceNum = 1;
            }

            int custid = Convert.ToInt16(Request["hCustomer_id"]);
            // preparing the product
            if (id == 0)//when user wants to add new product.
            {
                string shadecode = Request["selectShade"];
                DateTime reqdt = Convert.ToDateTime(Request["requested_delivery_date"]);

                OrderRepository objOrderProdRep = new OrderRepository(); // for GetProductCode() and GetProductDesc()
                string selectedBf = Request["selectBf"];
                string selectedGsm = Request["selectGsm"];
                string prodCode = objOrderProdRep.GetProductCode(selectedBf, selectedGsm);
                string prodDesc = objOrderProdRep.GetProductDesc(selectedBf, selectedGsm);

                objtempProduct.bf_code = selectedBf;
                objtempProduct.gsm_code = selectedGsm;

                objtempProduct.sequenceNumber = sequenceNum;
                objtempProduct.product_code = prodCode;
                objtempProduct.shade_code = shadecode;
                objtempProduct.description = prodDesc;
                objtempProduct.width = Convert.ToDecimal(Request["selectWidth"]);
                decimal unitprice = objOrderProdRep.GetPrice(custid, prodCode, shadecode);
                objtempProduct.unit_price = Math.Round(unitprice, 2);

                objtempProduct.qty = Math.Round(Convert.ToDecimal(Request["inputQuantityMt"]), 4);
                objtempProduct.amount = Convert.ToDecimal(Request["unit_price"]) * objtempProduct.qty;
                objtempProduct.diameter = Convert.ToDecimal(Request["inputDiaCm"]);
                objtempProduct.core = Convert.ToInt16(Request["selectCore"]);
                objtempProduct.status = "A"; // indicating that this record is 'Added'
                objtempProduct.created_by = User.Identity.Name;
                objtempProduct.requested_delivery_date = Convert.ToDateTime(Request["requested_delivery_date"]);
                objtempProduct.unit_price = Convert.ToDecimal(Request["unit_price"]);
                objtempProduct.widthInInch = Convert.ToDecimal(Request["selectWidth"]) * Convert.ToDecimal(2.54);
            }
            lstTempProds.Add(objtempProduct);
            sequenceNum = sequenceNum + 1;
            Session["tempOrderProds"] = lstTempProds;
            // we need to fill the drop downs of bf and gsm on the partial view so //
            loadProductFormLists();
            var currentList = lstTempProds.Where(k => k.status != "D").ToList();
            // save total of products into the viewbag // 
            ViewBag.grandtotal = GetProductsTotal(currentList);
            if (!currentList.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            ViewBag.lstTempProds = currentList;
            return PartialView("_currentProductsList", currentList);
            // return PartialView("_currentProductsList", lstTempProds);
        }
        [HandleModelStateException]
        public Boolean CheckShadeScheduleTime()
        {
            var shadeCode = Request["shadeCode"];
            DateTime requDeliveryDate = Convert.ToDateTime(Request["requested_delivery_date"]);
            var schduleList = (from sch in db.Schedule
                               where sch.shade_code == shadeCode
                                    && (sch.start_date <= requDeliveryDate
                                    && sch.end_date >= requDeliveryDate)
                               select sch).ToList();
            if (schduleList.Count() == 0)
            {
                return false;
            }
            return true;
        }

        [HandleModelStateException]
        public decimal GetTotalFromSession()
        {
            decimal gtotal;
            List<OrderRepository.tempOrderProducts> lstTempProds = new List<OrderRepository.tempOrderProducts>();
            lstTempProds = Session["tempOrderProds"] as List<OrderRepository.tempOrderProducts>; //GetSessionItem<OrderRepository.tempOrderProducts>("tempOrderProds");
            //get the total of all products which are not marked as 'D'
            var currentList = lstTempProds.Where(k => k.status != "D").ToList();

            //if (lstTempProds.Count!=0)

            if (currentList.Any())
            {
                //gtotal =  //(decimal)Session["grandtotal"];
                gtotal = (decimal)currentList.Sum(item => item.amount);

            }
            else
            {
                gtotal = 0.0M;
                // gtotal = (decimal)Session["grandtotal"];
            }
            Session["grandtotal"] = gtotal;

            return Math.Round(gtotal, 2);

        }

        // NOT BEING USED - displays the prefilled form to edit the product details
        //public PartialViewResult showEditProductinSession(int id) //GET
        //{
        //    List<OrderRepository.tempOrderProducts> lstTempProds = new List<OrderRepository.tempOrderProducts>();
        //    lstTempProds = Session["tempOrderProds"] as List<OrderRepository.tempOrderProducts>; //GetSessionItem<OrderRepository.tempOrderProducts>("tempOrderProds");
        //    // query the list to get the item at this id
        //    var prod = lstTempProds.Where(p => p.sequenceNumber == id).SingleOrDefault();

        //    //we need to fill the drop downs of bf and gsm on the partial view so,
        //    // Get the bf list
        //    OrderRepository objOrder = new OrderRepository();
        //    var Bfs = objOrder.GetBfs();
        //    ViewBag.Bf_list = new SelectList(Bfs, "bf_code", "description", prod.bf_code);

        //    // Get the gsm list
        //    var Gsms = objOrder.GetGsms();
        //    ViewBag.Gsm_list = new SelectList(Gsms, "gsm_code", "description", prod.gsm_code);

        //    // Get the bf list
        //    var Shades = objOrder.GetShades();
        //    ViewBag.Shade_list = new SelectList(Shades, "shade_code", "description", prod.shade_code);

        //    // Get the product code
        //    // string prod_code = objProd.GetProductCode();

        //    //make the list of core and save in ViewBag
        //    Dictionary<int, string> dictCores = new Dictionary<int, string>();
        //    dictCores.Add(3, "3 inches");
        //    dictCores.Add(4, "4 inches");
        //    ViewBag.Core_list = new SelectList(dictCores, "key", "value", prod.core);

        //    // modify the properties which are edited and update the status = 'E' of this record

        //    // show the addOrderinSession product panel with the prefilled values
        //    // preparing to show the product


        //    return PartialView("_EditProduct", prod);
        //}
        [HandleModelStateException]
        public PartialViewResult confirmEditProductinSession()// POST
        {
            try
            {
                List<OrderRepository.tempOrderProducts> lstTempProds = new List<OrderRepository.tempOrderProducts>();
                lstTempProds = Session["tempOrderProds"] as List<OrderRepository.tempOrderProducts>; //GetSessionItem<OrderRepository.tempOrderProducts>("tempOrderProds");
                int sequenceNumber = Convert.ToInt16(Request["sequenceNumber"]);
                //// query the list to get the item at this sequence number
                OrderRepository.tempOrderProducts prod = lstTempProds.Where(p => p.sequenceNumber == sequenceNumber).SingleOrDefault();
                if (prod != null) // found a list record with this sequence number
                {
                    OrderRepository objOrderProdRep = new OrderRepository(); // for GetProductCode() and GetProductDesc()

                    string selectedBf = Request["selectBf"];
                    string selectedGsm = Request["selectGsm"];

                    int custid = Convert.ToInt16(Request["hCustomer_id"]);

                    string prodCode = objOrderProdRep.GetProductCode(selectedBf, selectedGsm);
                    string prodDesc = objOrderProdRep.GetProductDesc(selectedBf, selectedGsm);
                    string shadecode = Request["selectShade"];
                    prod.bf_code = selectedBf;
                    prod.gsm_code = selectedGsm;

                    // prod.sequenceNumber = Convert.ToInt16(Request["sequenceNum"]);
                    prod.product_code = prodCode;
                    prod.shade_code = shadecode;
                    prod.description = prodDesc;
                    prod.width = Convert.ToDecimal(Request["selectWidth"]);
                    decimal unitprice = objOrderProdRep.GetPrice(custid, prodCode, shadecode);
                    prod.unit_price = Math.Round(unitprice, 2);

                    prod.qty = Math.Round(Convert.ToDecimal(Request["inputQuantityMt"]), 4);
                    prod.amount = Convert.ToDecimal(Request["priceText"]) * prod.qty;
                    prod.diameter = Convert.ToDecimal(Request["inputDiaCm"]);
                    prod.core = Convert.ToInt16(Request["selectCore"]);
                    prod.status = "E"; // indicating that this record is 'Edited'
                    prod.created_by = User.Identity.Name;
                    prod.requested_delivery_date = Convert.ToDateTime(Request["requested_delivery_date"]);
                    prod.unit_price = Convert.ToDecimal(Request["priceText"]);
                    prod.widthInInch = Convert.ToDecimal(Request["selectWidth"]) * Convert.ToDecimal(2.54);
                }
                // modify the properties which are edited and update the status = 'E' of this record having this sequence number

                // update the list with the edited product

                Session["tempOrderProds"] = lstTempProds;

                ////////////////////////////////////////
                var currentList = lstTempProds.Where(k => k.status != "D").ToList();

                // calculate the total of products // 

                ViewBag.grandtotal = ViewBag.grandtotal = GetProductsTotal(currentList);

                if (!currentList.Any())
                {
                    ViewBag.NoRecordMsg = "No data available !";
                }
                ////////////////////////////////////////

                loadProductFormLists();

                // return PartialView("_currentProductsList", lstTempProds);
                ViewBag.lstTempProds = currentList;
                return PartialView("_currentProductsList");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderController->confirmEditProductinSession:", Ex);
                return null;
            }
        }
        [HandleModelStateException]
        public PartialViewResult confirmRemoveProduct(int id)
        {
            List<OrderRepository.tempOrderProducts> lstTempProds = new List<OrderRepository.tempOrderProducts>();
            lstTempProds = Session["tempOrderProds"] as List<OrderRepository.tempOrderProducts>; //GetSessionItem<OrderRepository.tempOrderProducts>("tempOrderProds");

            // query the list to get the item at this id
            var prod = lstTempProds.Where(p => p.sequenceNumber == id).SingleOrDefault();
            // update the status = 'D' of this record
            if (prod != null) //found a record with this sequence id
            {
                prod.status = "D";
            }
            // do not remove from the list but just do not show in the current products list
            // lstTempProds.Remove(prod);

            Session["tempOrderProds"] = lstTempProds;

            ////////////////////////////////////////
            var currentList = lstTempProds.Where(k => k.status != "D").ToList();

            //// calculate the total of products // 
            decimal gtotal = 0;

            if (!currentList.Any())
            {
                gtotal = 0;
                ViewBag.grandtotal = 0;
                Session["grandtotal"] = 0;
                ViewBag.noRecordMessage = "No Products added. Please click on 'Add Products' to add a product.";
                //return PartialView("_currentProductsList");
            }
            else
            {
                // update the total in the session
                ViewBag.grandtotal = "";
                ViewBag.grandtotal = GetProductsTotal(currentList);
                //Session["grandtotal"] = Math.Round(gtotal, 2);
            }

            loadProductFormLists();
            ViewBag.lstTempProds = currentList;
            //return PartialView("_currentProductsList", lstTempProds);
            return PartialView("_currentProductsList", currentList);
        }


    }


}
