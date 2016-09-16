using System;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
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
using System.Web.WebPages.Html;


namespace MWV.Controllers.Admin
{
    public class GsmAdminController : Controller
    {

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        // GET: GsmAdmin
        public ActionResult Index(string msg)
        {
            List<Gsm> lstGsm;
            GsmRepository objGsm = new GsmRepository();
            lstGsm = objGsm.GetGSMs();
            ViewBag.Message = msg; //use this viewbag msg for add/edit/delete msg on index page. bydefault it is null.
            return PartialView(lstGsm.OrderBy(x => x.gsm_code).ToList());//sort by gsm_code
        }

        // GET: GsmAdmin/Details/5
        public ActionResult Details(string id)
        {
            List<Gsm> lstGsm;
            GsmRepository objGsm = new GsmRepository();
            lstGsm = objGsm.GetGSMs();
            var Gsm = lstGsm.Find(x => x.gsm_code == Convert.ToString(id));
            return View("Edit", Gsm);
        }


        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public ActionResult DeleteDetails(string id)
        {
            List<Gsm> lstGsm;
            GsmRepository objGsm = new GsmRepository();
            lstGsm = objGsm.GetGSMs();
            var Gsm = lstGsm.Find(x => x.gsm_code == Convert.ToString(id));
            return PartialView("Delete", Gsm);
        }


        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool AddGsm()
        {
            try
            {
                bool isAdded;
                string id = User.Identity.GetUserId();
                string created_by;
                created_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();

                string GsmCode = Request["GsmCode"].Trim();
                string GsmDescription = Request["GsmDescription"].Trim();
                GsmRepository objGsm = new GsmRepository();
                isAdded = objGsm.AddGsm(GsmCode, GsmDescription, created_by);
                if (isAdded == true)
                    return true;
                else
                    return false;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in GsmAdminController->AddGsm:", Ex);
                return false;
            }
        }


        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool EditGsm()
        {
            try
            {
                bool isEdited;
                string id = User.Identity.GetUserId();
                string modified_by;
                modified_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();
                string GsmCodeToEdit = Request["GsmCodeToEdit"];
                string GsmCode = Request["GsmCode"];
                string GsmDescription = Request["GsmDescription"].Trim();
                GsmRepository objGsm = new GsmRepository();
                isEdited = objGsm.EditGsm(GsmCodeToEdit, GsmCode, GsmDescription, modified_by);
                return isEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in GsmAdminController->EditGsm:", Ex);
                return false;
            }
        }


        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool DeleteGsm()
        {
            try
            {
                bool isDeleted;
                string GsmCode = Request["GsmCode"];
                GsmRepository objGsm = new GsmRepository();
                isDeleted = objGsm.DeleteGsm(GsmCode);
                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in GsmAdminController->DeleteGsm:", Ex);
                return false;
            }

        }


        // GET: GsmAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GsmAdmin/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: GsmAdmin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: GsmAdmin/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: GsmAdmin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: GsmAdmin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
