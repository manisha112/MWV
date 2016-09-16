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
    public class ShadeAdminController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        // GET: ShadeAdmin
        public ActionResult Index(string msg)
        {
            List<Shade> lstShade;
            ShadeRepository objShade = new ShadeRepository();
            lstShade = objShade.GetShades();
            ViewBag.Message = msg; //use this viewbag msg for add/edit/delete msg on index page. bydefault it is null.
            return View(lstShade.OrderBy(x => x.shade_code).ToList());//sort by shade_code
        }

        // GET: ShadeAdmin/Details/5
        public ActionResult Details(string id)
        {
            List<Shade> lstShade;
            ShadeRepository objShade = new ShadeRepository();
            lstShade = objShade.GetShades();
            var ShadeCode = lstShade.Find(x => x.shade_code == id);
            return View("Edit", ShadeCode);
        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public ActionResult DeleteDetails(string id)
        {
            List<Shade> lstShade;
            ShadeRepository objShade = new ShadeRepository();
            lstShade = objShade.GetShades();
            var ShadeCode = lstShade.Find(x => x.shade_code == Convert.ToString(id));
            return View("Delete", ShadeCode);
        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool AddShade()
        {
            try
            {
                bool isAdded;
                string id = User.Identity.GetUserId();
                string created_by;
                created_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();
                string ShadeCode = Request["ShadeCode"].Trim();
                string ShadeDescription = Request["ShadeDescription"].Trim();
                ShadeRepository objShade = new ShadeRepository();
                isAdded = objShade.AddShade(ShadeCode, ShadeDescription, created_by);
                if (isAdded == true)
                {
                    return true;
                    // ActionType = 1;
                }

                else
                    return false;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ShadeAdminController->AddShade:", Ex);
                return false;
            }
        }


        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool EditShade()
        {
            try
            {
                bool isEdited;
                string id = User.Identity.GetUserId();
                string modified_by;
                modified_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();
                string ShadeCodeToEdit = Request["ShadeCodeToEdit"];
                string ShadeCode = Request["ShadeCode"];
                string ShadeDescription = Request["ShadeDescription"].Trim();
                ShadeRepository objShade = new ShadeRepository();
                isEdited = objShade.EditShade(ShadeCodeToEdit, ShadeCode, ShadeDescription, modified_by);
                return isEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ShadeAdminController->EditShade:", Ex);
                return false;
            }
        }


        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool DeleteShade()
        {
            try
            {
                bool isDeleted;
                string ShadeCode = Request["ShadeCode"];
                ShadeRepository objShade = new ShadeRepository();
                isDeleted = objShade.DeleteShade(ShadeCode);
                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ShadeAdminController->DeleteShade:", Ex);
                return false;
            }

        }



        // GET: ShadeAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShadeAdmin/Create
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

        // GET: ShadeAdmin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ShadeAdmin/Edit/5
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

        // GET: ShadeAdmin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ShadeAdmin/Delete/5
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
