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
    public class CoreAdminController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();
        
        // GET: CoreAdmin
        public ActionResult Index(string msg)
        {
            List<Core> lstCore;
            CoreRepository objCore = new CoreRepository();
            lstCore = objCore.GetCore();
            ViewBag.Message = msg; //use this viewbag msg for add/edit/delete msg on index page. bydefault it is null.
            return View(lstCore.OrderBy(x => x.core_code).ToList());//sort by lstCore
        }

        // GET: CoreAdmin/Details/5
        public ActionResult Details(string id)
        {
            List<Core> lstCore;
            CoreRepository objCore = new CoreRepository();
            lstCore = objCore.GetCore();
            var CoreCode = lstCore.Find(x => x.core_code == id);
            return View("Edit", CoreCode);
        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public ActionResult DeleteDetails(string id)
        {
            List<Core> lstCore;
            CoreRepository objCore = new CoreRepository();
            lstCore = objCore.GetCore();
            var CoreCode = lstCore.Find(x => x.core_code == id);
            return View("Delete", CoreCode);
        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool AddCore()
        {
            try
            {
                bool isCoreAdded;
                string id = User.Identity.GetUserId();
                string created_by;
                created_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();
                string CoreCode = Request["CoreCode"].Trim();
                string CoreDescription = Request["CoreDescription"].Trim();
                CoreRepository objCore = new CoreRepository();
                isCoreAdded = objCore.AddCore(CoreCode, CoreDescription, created_by);
                if (isCoreAdded == true)
                {
                    return true;
                    // ActionType = 1;
                }

                else
                    return false;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CoreAdminController->AddCore:", Ex);
                return false;
            }
        }


        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool EditCore()
        {
            try
            {
                bool isEdited;
                string id = User.Identity.GetUserId();
                string modified_by;
                modified_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();
                string CoreCodeToEdit = Request["CoreCodeToEdit"];
                string CoreCode = Request["CoreCode"];
                string CoreDescription = Request["CoreDescription"].Trim();
                CoreRepository objCore = new CoreRepository();
                isEdited = objCore.EditCore(CoreCodeToEdit, CoreCode, CoreDescription, modified_by);
                return isEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CoreAdminController->EditCore:", Ex);
                return false;
            }
        }


        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool DeleteCore()
        {
            try
            {
                bool isDeleted;
                string CoreCode = Request["CoreCode"];
                CoreRepository objCore = new CoreRepository();
                isDeleted = objCore.DeleteCore(CoreCode);
                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CoreAdminController->DeleteCore:", Ex);
                return false;
            }
        }

        // GET: CoreAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CoreAdmin/Create
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

        // GET: CoreAdmin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CoreAdmin/Edit/5
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

        // GET: CoreAdmin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CoreAdmin/Delete/5
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
