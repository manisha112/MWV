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
    public class GsmController : Controller
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

        // GET: /Gsm/
        [HandleModelStateException]
        public ActionResult Index()
        {
            return View(db.Gsms.ToList());
        }

        // GET: /Gsm/Details/5
        [HandleModelStateException]
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gsm gsm = db.Gsms.Find(id);
            if (gsm == null)
            {
                return HttpNotFound();
            }
            return View(gsm);
        }

        // GET: /Gsm/Create
        [HandleModelStateException]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Gsm/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "gsm_code,description")] Gsm gsm)
        {
            if (ModelState.IsValid)
            {
                db.Gsms.Add(gsm);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(gsm);
        }

        // GET: /Gsm/Edit/5
        [HandleModelStateException]
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gsm gsm = db.Gsms.Find(id);
            if (gsm == null)
            {
                return HttpNotFound();
            }
            return View(gsm);
        }

        // POST: /Gsm/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "gsm_code,description")] Gsm gsm)
        {
            if (ModelState.IsValid)
            {
                db.Entry(gsm).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(gsm);
        }

        // GET: /Gsm/Delete/5
        [HandleModelStateException]
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Gsm gsm = db.Gsms.Find(id);
            if (gsm == null)
            {
                return HttpNotFound();
            }
            return View(gsm);
        }

        // POST: /Gsm/Delete/5
        [HttpPost, ActionName("Delete")]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Gsm gsm = db.Gsms.Find(id);
            db.Gsms.Remove(gsm);
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
