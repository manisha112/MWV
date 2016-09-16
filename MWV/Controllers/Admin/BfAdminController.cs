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




namespace MWV.Controllers
{
    public class BfAdminController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        // GET: BfAdmin
        
        public ActionResult Index(string msg)
        {
            List<Bf> lstBf;
            BfRepository objBF = new BfRepository();
            lstBf = objBF.GetBfs();
            ViewBag.Message = msg; //use this viewbag msg for add/edit/delete msg on index page. bydefault it is null.
            return PartialView(lstBf.OrderBy(x => x.bf_code).ToList());//sort by bf_code
        }

        // GET: BfAdmin/Details/5
        public ActionResult Details(string id)
        {
            List<Bf> lstBf;
            BfRepository objBF = new BfRepository();
            lstBf = objBF.GetBfs();
            var BFCode = lstBf.Find(x => x.bf_code == id);
            return PartialView("Edit", BFCode);
        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public ActionResult DeleteDetails(string id)
        {
            List<Bf> lstBf;
            BfRepository objBF = new BfRepository();
            lstBf = objBF.GetBfs();
            var BFCode = lstBf.Find(x => x.bf_code == Convert.ToString(id));
            return PartialView("Delete", BFCode);
        }

        // GET: BfAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: BfAdmin/Create
        // [HttpPost]
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool AddBF()
        {
            try
            {
                bool isBFAdded;
                string id = User.Identity.GetUserId();
                string created_by;
                created_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();
                string BFCode = Request["BFCode"].Trim();
                string BFDescription = Request["BFDescription"].Trim();
                BfRepository objBF = new BfRepository();
                isBFAdded = objBF.AddBF(BFCode, BFDescription, created_by);
                if (isBFAdded == true)
                {
                    return true;
                    // ActionType = 1;
                }

                else
                    return false;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in BFAdminController->AddBF:", Ex);
                return false;
            }
        }
        // GET: BfAdmin/Edit/5
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool EditBF()
        {
            try
            {
                bool isBFEdited;
                string id = User.Identity.GetUserId();
                string modified_by;
                modified_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();
                string BFCodeToEdit = Request["BFCodeToEdit"];
                string BFCode = Request["BFCode"];
                string BFDescription = Request["BFDescription"].Trim();
                BfRepository objBF = new BfRepository();
                isBFEdited = objBF.EditBF(BFCodeToEdit, BFCode, BFDescription, modified_by);
                return isBFEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in BFAdminController->EditBF:", Ex);
                return false;
            }
        }

        // POST: BfAdmin/Edit/5
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

        // GET: BfAdmin/Delete/5
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool DeleteBF()
        {
            try
            {
                bool isBFDeleted;
                string BFCode = Request["BFCode"];
                // string BFDescription = Request["BFDescription"].Trim();
                BfRepository objBF = new BfRepository();
                isBFDeleted = objBF.DeleteBF(BFCode);
                return isBFDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in BFAdminController->DeleteBF:", Ex);
                return false;
            }

        }

        // POST: BfAdmin/Delete/5
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
