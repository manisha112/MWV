using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using PagedList;
using PagedList.Mvc;
using MWV.Repository.Implementation;
using MWV.DBContext;
using MWV.Models;


namespace MWV.Controllers
{
    public class QuickViewController : Controller
    {
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

        // GET: /QuickView/
        [HandleModelStateException]
        public ActionResult Index()
        {
            QuickViewModel quickView = new QuickViewModel();
            var dt = DateTime.Now.AddDays(-7); //Show only 7 days Records from current Date

            var customers = (from r in db.Customers
                             where r.approved_on > dt
                             select r);

            var order = (from r in db.Orders
                         where r.order_date > dt
                         select r);

            var truck_dispatches = (from r in db.Truck_dispatches
                                    where r.estimated_arrival > dt
                                    select r);

            quickView.CustomerDetails = customers.ToList();
            quickView.OrderDetails = order.ToList();
            quickView.Truck_dispatches = truck_dispatches.ToList();
            return View(quickView);
        }
        [HandleModelStateException]
        public PartialViewResult QuickViewDetails()
        {
            OrderRepository ordRepo = new OrderRepository();
            int AgentId = ordRepo.GetAgentID();

            QuickViewModel quickView = new QuickViewModel();
            var dt = DateTime.Now.AddDays(-7); //Show only 7 days Records from current Date

            // get three recent 7 days orders for show the this ordres in quickview details
            var order = (from r in db.Orders
                         where (r.order_date > dt && r.agent_id == AgentId)
                         orderby r.order_date descending
                         select r).Take(3);

            // get three recent 7 days vehicles record for display vehicles in recent quickview details
            var truck_dispatches = (from r in db.Truck_dispatches
                                    where (r.estimated_arrival > dt && r.agent_id == AgentId)
                                    orderby r.modified_on descending
                                    //orderby r.estimated_arrival descending
                                    select r).Take(3);

            quickView.OrderDetails = order.ToList();
            quickView.Truck_dispatches = truck_dispatches.ToList();
            return PartialView("_QuickView", quickView);
        }
        [HandleModelStateException]
        public ActionResult seeAllRecentOrders(int? page)
        {
            var dt = DateTime.Now.AddDays(-7); //Show only 7 days Records from current Date
            OrderRepository ordRepo = new OrderRepository();
            int AgentId = ordRepo.GetAgentID();

            //get all 7 days orders from current Date
            var orders = (from r in db.Orders
                          where (r.order_date > dt && r.agent_id == AgentId)
                          orderby r.order_date descending
                          select r);

            int pageSize = 5;
            int pageNumber = (page ?? 1); // used for pagging
            if (orders.ToList().Count == 0)
            {
                ViewBag.NoRecordMsg = "No data available !";
            }

            ViewBag.OrderDetails = orders.ToList().ToPagedList(pageNumber, pageSize);// apply pagging for this orders list
            ViewBag.Pagesize = orders.ToList().Count();
            return PartialView("_AllRecentOrders");
        }
        [HandleModelStateException]
        public ActionResult seeAllRecentTransportation(int? page)
        {
            OrderRepository ordRepo = new OrderRepository();
            int AgentId = ordRepo.GetAgentID();

            int pageSize = 5;
            int pageNumber = (page ?? 1);// used for pagging
            var dt = DateTime.Now.AddDays(-7); //Show only 7 days Records from current Date

            //get all 7 days vehicles from current Date
            var truck_dispatches = (from r in db.Truck_dispatches
                                    where (r.estimated_arrival > dt && r.agent_id == AgentId)
                                    orderby r.estimated_arrival descending
                                    select r);

            if (truck_dispatches.ToList().Count == 0)
            {
                ViewBag.NoRecordMsg = "No data available !";
            }

            ViewBag.Truck_dispatches = truck_dispatches.ToList().ToPagedList(pageNumber, pageSize);
            ViewBag.Pagesize = truck_dispatches.ToList().Count();
            return PartialView("_AllRecentTransportation");
        }
        [HandleModelStateException]
        public ActionResult seeTransportationDetails(int id)
        {
            QuickViewModel quickView = new QuickViewModel();
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //get details of particular vehicle by vehicle id , vehicle id is truck_dispatch_id.
            var truck_dispatches = (from r in db.Truck_dispatches
                                    join td in db.Truck_dispatch_details on r.truck_dispatch_id equals td.truck_dispatch_id
                                    //join or in db.Orders on td.order_id equals or.order_id

                                    join op in db.Order_products on td.order_product_id equals op.order_product_id
                                    join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                    join pm in db.Papermills on sch.papermill_id equals pm.papermill_id
                                    where r.truck_dispatch_id == id
                                    select new TempTruckDetails
                                    {
                                        truck_dispatch_id = r.truck_dispatch_id,
                                        truck_no = r.truck_no,
                                        pmlocationid = pm.papermill_id,
                                        location = pm.location,
                                        address = pm.address,
                                        created_on = r.created_on,
                                        estimated_capacity = r.estimated_capacity,
                                        agent_dispatched_on = r.agent_dispatched_on,
                                        estimated_arrival = r.estimated_arrival,
                                        actual_arrival_at_gate = r.actual_arrival_at_gate,
                                        left_factory_on = r.left_factory_on,
                                        papermill_id = pm.papermill_id
                                    }).ToList();


            var truck_dispatches_detail = (from r in db.Truck_dispatch_details
                                           where r.truck_dispatch_id == id
                                           select r);
            if (truck_dispatches == null || truck_dispatches_detail == null)
            {
                return HttpNotFound();
            }
            ViewBag.Truck_dispatches = truck_dispatches.Take(1);
            quickView.Truck_dispatch_details = truck_dispatches_detail.ToList();
            if (!truck_dispatches_detail.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            return PartialView("_TransportationDetails", quickView);
        }
        [HandleModelStateException]
        public ActionResult GetOrderDetailsMaster(int id)
        {
            QuickViewModel quickView = new QuickViewModel();
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orderDetails = (from r in db.Orders
                                where r.order_id == id
                                //&& r.is_deckled == true
                                select r).ToList();

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

            ViewBag.selectedpm = selectedpm;
            quickView.OrderDetails = orderDetails;
            return PartialView("_OrderDetailsMaster", quickView);
        }
        [HandleModelStateException]
        public ActionResult GetOrderDetailsChild(int id)
        {
            QuickViewModel quickView = new QuickViewModel();
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orderProductsDetails = (from r in db.Order_products
                                        where r.order_id == id
                                        select r);

            //get total amount of order by sum of all orderproduct of paricular id
            var totaLAmount = (from r in db.Order_products
                               where r.order_id == id
                               select r.amount).Sum();

            List<TempPendingApproval> lst = (from or in db.Order_products
                                             join orderid in db.Orders on or.order_id equals orderid.order_id
                                             join pc in db.ProductionChild on or.order_product_id equals pc.order_product_id
                                             join pj in db.ProductionJumbo on pc.pj_id equals pj.pj_id
                                             where (orderid.order_id == id)
                                             group new { pc, pj } by new { pc.order_product_id, pj.estimated_start } into g
                                             select new TempPendingApproval { ActualQty = g.Sum(k => k.pc.qty), estimated_start = g.FirstOrDefault().pj.estimated_start, order_id = (int)g.FirstOrDefault().pc.order_product_id }).ToList();
            if (lst.Count > 0) { ViewBag.EstQtySch = lst; } else { ViewBag.EstQtySch = null; }



            //For Qty Dispatch Facility To Date
            List<TempPendingApproval> qtyDispatchFacility = (from tdd in db.Truck_dispatch_details
                                                             where tdd.order_id == id
                                                             group tdd by tdd.order_product_id into g
                                                             select new TempPendingApproval
                                                             {
                                                                 qtyDispatchFacility = g.Sum(k => k.qty_loaded),
                                                                 order_product_id = g.FirstOrDefault().order_product_id,
                                                             }
                                              ).ToList();
            if (qtyDispatchFacility.Count > 0) { ViewBag.qtyDispatchFacility = qtyDispatchFacility; } else { ViewBag.qtyDispatchFacility = null; }
            //For Qty Dispatced To Date

            List<TempPendingApproval> qtyschforlod = (from op in db.Order_products
                                                      join pc in db.ProductionChild
                                                      on op.order_product_id equals pc.order_product_id
                                                      where op.order_id == id && pc.actual_end != null
                                                      group new { pc, op } by op.order_product_id into p
                                                      select new TempPendingApproval
                                                      {
                                                          // qtyUnderplanning = p.Sum(l => (decimal?)l.op.qty) - p.Sum(k => (decimal?)k.op.qty_scheduled),
                                                          plannedQty = p.Sum(j => j.pc.qty),
                                                          order_product_id = p.FirstOrDefault().op.order_product_id,///Actual Planned qty


                                                      }).ToList();
            if (qtyschforlod.Count > 0) { ViewBag.qtyforsch = qtyschforlod; } else { ViewBag.qtyforsch = null; }

            //Schudle For Loading
            List<TempPendingApproval> qtyForSchudle = (from tdd in db.Truck_dispatch_details
                                                       where tdd.order_id == id
                                                       group tdd by tdd.order_product_id into k
                                                       select new TempPendingApproval
                                                       {
                                                           qty = k.FirstOrDefault().qty,
                                                           qtyLoaded = k.FirstOrDefault().qty_loaded,
                                                           order_product_id = k.FirstOrDefault().order_product_id,

                                                       }

                                                         )
                                                         .ToList();

            if (qtyForSchudle.Count > 0) { ViewBag.qtyForSchudle = qtyForSchudle; } else { ViewBag.qtyForSchudle = null; }


            ViewBag.totaLAmount = totaLAmount;
            quickView.Order_Products = orderProductsDetails.ToList();
            return PartialView("_OrderDetailsChild", quickView);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}