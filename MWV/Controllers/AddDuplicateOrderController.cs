using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MWV.Models;
using MWV.DBContext;
using MWV.Repository.Implementation;


namespace MWV.Controllers
{
    public class AddDuplicateOrderController : Controller
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
        // GET: /AddDuplicateOrder/
        [HandleModelStateException]
        public ActionResult Index()
        {
            var orders = db.Orders.Include(o => o.Agent).Include(o => o.Customer);
            return View(orders.ToList());
        }

        // GET: /AddDuplicateOrder/Details/5
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

        // GET: /AddDuplicateOrder/Create
        [HandleModelStateException]
        public ActionResult Create()
        {
            // get the lastest order created 

            var objOrder = db.Orders.OrderByDescending(m => m.order_id).Take(1).ToList();
            //objOrder[0].order_id

            Order order = db.Orders.Find(objOrder[0].order_id);
            //Order order = db.Orders.Find(3);

            ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name", order.agent_id);
            ViewBag.customer_id = new SelectList(db.Customers, "customer_id", "name", order.customer_id);
            return View(order);


            //ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name");
            //ViewBag.customer_id = new SelectList(db.Customers, "customer_id", "name");
            //return View();
        }

        // POST: /AddDuplicateOrder/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "order_date,agent_id,customer_id,status,requested_delivery_date,remarks,amount,papermill_id,created_on,created_by,modified_on,modified_by")] Order order)
        {



            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name", order.agent_id);
            ViewBag.customer_id = new SelectList(db.Customers, "customer_id", "name", order.customer_id);
            return View(order);
        }

        // GET: /AddDuplicateOrder/Edit/5
        [HandleModelStateException]
        public ActionResult Edit(int? id)
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
            ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name", order.agent_id);
            ViewBag.customer_id = new SelectList(db.Customers, "customer_id", "name", order.customer_id);
            return View(order);
        }

        // POST: /AddDuplicateOrder/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "order_id,order_date,agent_id,customer_id,status,requested_delivery_date,remarks,amount,papermill_id,created_on,created_by,modified_on,modified_by")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.agent_id = new SelectList(db.Agents, "agent_id", "name", order.agent_id);
            ViewBag.customer_id = new SelectList(db.Customers, "customer_id", "name", order.customer_id);
            return View(order);
        }

        // GET: /AddDuplicateOrder/Delete/5
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

        // POST: /AddDuplicateOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
            db.SaveChanges();
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
    }
}
