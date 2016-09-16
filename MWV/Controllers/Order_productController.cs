using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MWV.Models;
using MWV.DBContext;
using MWV.Repository.Implementation;
using Microsoft.AspNet.Identity;
using System.Reflection;


namespace MWV.Controllers
{
    public class Order_productController : Controller
    {
        private MWVDBContext db = new MWVDBContext();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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
        // GET: /Order_product/
        [HandleModelStateException]
        public ActionResult Index()
        {
            var order_products = db.Order_products.Include(o => o.order).Include(o => o.Product).Include(o => o.Shade);
            return View(order_products.ToList());
        }

        // GET: /Order_product/Details/5
        [HandleModelStateException]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_Products order_products = db.Order_products.Find(id);
            if (order_products == null)
            {
                return HttpNotFound();
            }
            return View(order_products);
        }

        // GET: /Order_product/Create
        [HandleModelStateException]
        public ActionResult Create()
        {
            ViewBag.order_id = new SelectList(db.Orders, "order_id", "order_id");
            ViewBag.gsm_list = new SelectList(db.Gsms, "gsm_code", "description");
            ViewBag.bf_list = new SelectList(db.Bfs, "bf_code", "description");
            ViewBag.product_code = new SelectList(db.Products, "product_code", "product_code");
            ViewBag.shade_code = new SelectList(db.Shades, "shade_code", "description");
            return View();
        }


        // POST: /Order_product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "order_id,product_code,shade_code,width,unit_price,qty,amount,status,width_planned,qty_planned,diameter,core,created_on,created_by,modified_on,modified_by")] Order_Products order_products)
        {

            // order product added by the logged in agent, so get the name of the agent and insert in 'created by'
            // get the agent_id from 'Agents' table with this user id

            order_products.created_by = GetAgentName();
            OrderRepository objOrderProdRep = new OrderRepository();

            order_products.product_code = objOrderProdRep.GetProductCode(Request["gsm_list"], Request["bf_list"]);
            order_products.shade_code = Request["shade_code"];
            order_products.width = Convert.ToInt16(Request["width"]);
            decimal price = GetPrice(1, order_products.product_code, Request["shade_code"]);
            //string msg="";
            //if (price==0)
            //{
            //   msg = "Price does not exist in the active range";

            //}
            order_products.unit_price = price;
            order_products.qty = Convert.ToDecimal(Request["qty"]);
            order_products.amount = order_products.unit_price * order_products.qty;
            order_products.status = "Approved";

            order_products.diameter = 121;
            order_products.created_on = DateTime.Now;
            order_products.order_id = Convert.ToInt16(Request.UrlReferrer.Segments[3]);

            if (ModelState.IsValid)
            {

                OrderRepository objAddOrderProd = new OrderRepository();
                objAddOrderProd.AddOrder_product(order_products);

                //  return RedirectToAction("Index");

            }

            ViewBag.order_id = new SelectList(db.Orders, "order_id", "status", order_products.order_id);
            ViewBag.product_code = new SelectList(db.Products, "product_code", "gsm_code", order_products.product_code);
            ViewBag.shade_code = new SelectList(db.Shades, "shade_code", "description", order_products.shade_code);
            // return View(order_products);
            return RedirectToAction("Details", "Order", new { id = order_products.order_id }); // goes back to the edit page of order
        }

        // GET: /Order_product/Edit/5
        [HandleModelStateException]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_Products order_products = db.Order_products.Find(id);
            if (order_products == null)
            {
                return HttpNotFound();
            }
            ViewBag.order_id = new SelectList(db.Orders, "order_id", "order_id", order_products.order_id);
            ViewBag.product_code = new SelectList(db.Products, "product_code", "product_code", order_products.product_code);
            ViewBag.shade_code = new SelectList(db.Shades, "shade_code", "description", order_products.shade_code);
            return View(order_products);
        }

        // POST: /Order_product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "order_product_id,order_id,product_code,shade_code,width,unit_price,qty,amount,status,width_planned,qty_planned,diameter,core,created_on,created_by,modified_on,modified_by")] Order_Products order_products)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order_products).State = EntityState.Modified;
                db.SaveChanges();
                //return RedirectToAction("Index");
            }
            ViewBag.order_id = new SelectList(db.Orders, "order_id", "status", order_products.order_id);
            ViewBag.product_code = new SelectList(db.Products, "product_code", "gsm_code", order_products.product_code);
            ViewBag.shade_code = new SelectList(db.Shades, "shade_code", "description", order_products.shade_code);

            return RedirectToAction("Edit", "Order", new { id = Request["order_id"] }); // goes back to the edit page of order
        }

        // GET: /Order_product/Delete/5
        [HandleModelStateException]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_Products order_products = db.Order_products.Find(id);
            if (order_products == null)
            {
                return HttpNotFound();
            }
            return View(order_products);
        }

        // POST: /Order_product/Delete/5
        [HttpPost, ActionName("Delete")]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order_Products order_products = db.Order_products.Find(id);
            // first delete from truck_dispatch_detail if it exists there 
            var deleteOrderDispatch = db.Truck_dispatch_details.Where(p => p.order_product_id == id);
            if (deleteOrderDispatch.ToList().Count != 0)
            {
                foreach (Truck_dispatch_details td in deleteOrderDispatch)
                {
                    db.Truck_dispatch_details.Remove(td);
                }
            }
            db.Order_products.Remove(order_products);
            db.SaveChanges();
            //return RedirectToAction("Index");
            return RedirectToAction("Edit", "Order", new { id = order_products.order_id }); // goes back to the edit page of order
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // functions written by Rajni

        public string GetAgentName()
        {
            string id = User.Identity.GetUserId();
            var ag = (from a in db.Agents where a.aspnetusers_id == id select a.agent_id).SingleOrDefault();
            var agName = db.Agents.Where(p => p.agent_id == ag).Select(m => m.name).SingleOrDefault();
            return agName;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [HandleModelStateException]
        public JsonResult GetGsms()
        {
            try
            {
                string bf = Request.Url.Segments[3];
                var lstGsms = db.Products.Where(k => k.bf_code == bf).ToList();

                var result = (from s in lstGsms
                              select new
                              {
                                  id = s.gsm_code,
                                  name = s.gsm_code
                              }).ToList();

                string message = string.Format("Gsms of selected bf are: ");

                return Json(result, JsonRequestBehavior.AllowGet);

            }
            catch (Exception Ex)
            {
                logger.Error("Error in OrderProducctcontroller->GetGsms:", Ex);
                return null;
            }

        }

        public string GetProductCode(string bf_code, string gsm_code)
        {
            OrderRepository objOrderProdRep = new OrderRepository();
            return objOrderProdRep.GetProductCode(bf_code, gsm_code);
        }
        public decimal GetProductCodebyCustId()
        {
            int custid = Convert.ToInt16(Request["CustId"]);
            string bf_code = Request["selectedBf"];
            string gsm_code = Request["selectedGsm"];
            string shade_code = Request["selectShade"];
            OrderRepository objOrderProdRep = new OrderRepository();
            return objOrderProdRep.GetProductCodebyCustId(custid, bf_code, gsm_code, shade_code);
        }

        public decimal GetPrice(int customer_id, string product_code, string shade_code)
        {
            OrderRepository objOrderProdRep = new OrderRepository();
            return objOrderProdRep.GetPrice(customer_id, product_code, shade_code);
        }


        // GET: /Order_product/ProductionPlanner/ViewDetails/5
        [HandleModelStateException]
        public ActionResult ViewDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order_Products order_products = db.Order_products.Find(id);
            if (order_products == null)
            {
                return HttpNotFound();
            }
            return View("ProductionPlanner/ViewDetails", order_products);
        }
        // functions written by Rajni




        //public ActionResult AllProdsForLatestOrder()
        //{
        //    // get the lastest order created 
        //    var objOrder = db.Orders.OrderByDescending(m => m.order_id).Take(1).ToList();

        //    Order order = db.Orders.Find(objOrder[0].order_id);
        //    // pass this order id to the order repository
        //    Order_productRepository objOrderProdsRep = new Order_productRepository();
        //    //List<Order_Products>
        //    var orderProds = objOrderProdsRep.GetOrderProducts(objOrder[0].order_id);

        //    if (orderProds.Count == 0) { Response.Write("The list is empty"); return HttpNotFound(); }
        //    else
        //    {
        //        ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name", order.agent_id);
        //        ViewBag.customer_id = new SelectList(db.Customers, "customer_id", "name", order.customer_id);
        //        return View("ProdsLastestOrderView", orderProds);
        //        // return View(orderProds); // view waas not found
        //    }

        //    //ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name");
        //    //ViewBag.customer_id = new SelectList(db.Customers, "customer_id", "name");
        //    //return View();
        //}

        // GET: /Order_product/ //List<Order_Products>

        //public ActionResult ShowOrderProducts(int OrderId)
        //{
        //    //var order_products = db.Order_products.Include(o => o.order).Include(o => o.Product).Include(o => o.Shade).Where(p=> p.order_id==OrderId);
        //    var order_products = (from c in db.Order_products
        //                          where c.order_id == OrderId
        //                          select c).ToList<Order_Products>();
        //    //db.Order_products.Where(p => p.order_id == OrderId).Select( p => new { p.product_code, p.shade_code, p.unit_price } ).ToList<Order_Products>();
        //    // return order_products;
        //    return View(order_products);
        //    //return PartialView("Partial3", order_products.ToList<Order_Products>);
        //}
    }
}
