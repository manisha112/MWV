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
    public class PaperMillAdminController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();
        // GET: PaperMillAdmin
        public ActionResult Index(string msg)
        {
            List<Papermill> lstPapermill;
            PaperMillRepository objPapermill = new PaperMillRepository();
            lstPapermill = objPapermill.GetListOfPaperMill();
            ViewBag.Message = msg; //use this viewbag msg for add/edit/delete msg on index page. bydefault it is null.
            return View(lstPapermill.ToList().OrderBy(x => x.name));
        }

        // GET: PaperMillAdmin/Details/5
        public ActionResult Details(int id)
        {
            List<Papermill> lstPapermill;
            PaperMillRepository objPapermill = new PaperMillRepository();
            lstPapermill = objPapermill.GetListOfPaperMill();
            var Papermill = lstPapermill.Find(x => x.papermill_id == id);
            return View("Edit", Papermill);
        }

        public bool CheckDuplicateName(int PaperMillIDToEdit, string location, string name, bool isAddOperation)
        {

            bool isDupFound = false;
            List<Papermill> lstPapermill;
            PaperMillRepository objPapermill = new PaperMillRepository();
            lstPapermill = objPapermill.GetListOfPaperMill();

            if (isAddOperation == true)
            {
                var dupName = lstPapermill.Find(c => c.location.Trim() == location && c.name.Trim() == name);
                if (dupName != null)//check duplicate location + name  pair while adding.
                {
                    isDupFound = true;
                }
                else
                {
                    isDupFound = false; // can add papermill details
                }
            }
            else //for edit check pair location + name
            {
                var RemoveObj = lstPapermill.Find(x => x.papermill_id == PaperMillIDToEdit);
                lstPapermill.Remove(RemoveObj);
                var dupName = lstPapermill.Find(c => c.location.Trim() == location && c.name.Trim() == name);
                if (dupName != null)//check duplicate location + name  pair while editing.
                {
                    isDupFound = true;
                }
                else
                    isDupFound = false; // can edit papermill details 
            }

            return isDupFound;
        }



        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool AddPaperMill()
        {
            bool isPaperMillAdd = false;
            bool isDupFound = false;
            try
            {

                string id = User.Identity.GetUserId();
                string created_by;
                created_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();

                decimal capacity = Convert.ToDecimal(Request["capacity"]);
                decimal min_width = Convert.ToDecimal(Request["min_width"]);
                decimal max_width = Convert.ToDecimal(Request["max_width"]);
                string location = Request["location"].Trim();
                decimal deckle_min = Convert.ToDecimal(Request["deckle_min"]);
                decimal deckle_max = Convert.ToDecimal(Request["deckle_max"]);
                int max_cuts = Convert.ToInt32(Request["max_cuts"]);
                decimal min_diameter = Convert.ToDecimal(Request["min_diameter"]);
                decimal max_diameter = Convert.ToDecimal(Request["max_diameter"]);
                decimal max_weight_child = Convert.ToDecimal(Request["max_weight_child"]);
                decimal min_weight_jumbo = Convert.ToDecimal(Request["min_weight_jumbo"]);
                decimal max_weight_jumbo = Convert.ToDecimal(Request["max_weight_jumbo"]);
                string name = Request["name"].Trim();
                string address = Request["address"].Trim();

                isDupFound = CheckDuplicateName(0, location, name, true);

                if (isDupFound == false)//can add
                {
                    PaperMillRepository objPapermill = new PaperMillRepository();
                    isPaperMillAdd = objPapermill.AddPaperMill(capacity, min_width, max_width, location, deckle_min, deckle_max, max_cuts, min_diameter, max_diameter, max_weight_child, min_weight_jumbo, max_weight_jumbo, name, address, created_by);
                    if (isPaperMillAdd == true)
                        isPaperMillAdd = true;
                    else
                        isPaperMillAdd = false;
                }
                else
                    isPaperMillAdd = false;

                return isPaperMillAdd;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in PaperMillAdminController->PaperMillAdd:", Ex);
                return false;
            }

        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool EditPaperMill()
        {
            bool isEdited = false;
            bool isDupFound = false;
            try
            {

                string id = User.Identity.GetUserId();
                string modified_by;
                modified_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();
                int PaperMillIDToEdit = Convert.ToInt32(Request["PaperMillIDToEdit"]);
                decimal capacity = Convert.ToDecimal(Request["capacity"]);
                decimal min_width = Convert.ToDecimal(Request["min_width"]);
                decimal max_width = Convert.ToDecimal(Request["max_width"]);
                string location = Request["location"].Trim();
                decimal deckle_min = Convert.ToDecimal(Request["deckle_min"]);
                decimal deckle_max = Convert.ToDecimal(Request["deckle_max"]);
                int max_cuts = Convert.ToInt32(Request["max_cuts"]);
                decimal min_diameter = Convert.ToDecimal(Request["min_diameter"]);
                decimal max_diameter = Convert.ToDecimal(Request["max_diameter"]);
                decimal max_weight_child = Convert.ToDecimal(Request["max_weight_child"]);
                decimal min_weight_jumbo = Convert.ToDecimal(Request["min_weight_jumbo"]);
                decimal max_weight_jumbo = Convert.ToDecimal(Request["max_weight_jumbo"]);
                string name = Request["name"].Trim();
                string address = Request["address"].Trim();

                isDupFound = CheckDuplicateName(PaperMillIDToEdit, location, name, false);

                if (isDupFound == false)//can edit
                {
                    PaperMillRepository objPapermill = new PaperMillRepository();
                    isEdited = objPapermill.EditPaperMill(PaperMillIDToEdit, capacity, min_width, max_width, location, deckle_min, deckle_max, max_cuts, min_diameter, max_diameter, max_weight_child, min_weight_jumbo, max_weight_jumbo, name, address, modified_by);
                    if (isEdited == true)
                        isEdited = true;
                    else
                        isEdited = false;
                }
                else
                    isEdited = false;

                return isEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in PaperMillAdminController->EditPaperMill:", Ex);
                return false;
            }
        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public ActionResult DeleteDetails(int id)
        {
            List<Papermill> lstPapermill;
            PaperMillRepository objPapermill = new PaperMillRepository();
            lstPapermill = objPapermill.GetListOfPaperMill();
            var Papermill = lstPapermill.Find(x => x.papermill_id == id);
            return View("Delete", Papermill);
        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool DeletePaperMill()
        {
            try
            {
                bool isDeleted;
                int PaperMillIDToDel = Convert.ToInt32(Request["PaperMillIDToDel"]);
                PaperMillRepository objPapermill = new PaperMillRepository();
                isDeleted = objPapermill.DeletePaperMill(PaperMillIDToDel);
                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductAdminController->DeleteProduct:", Ex);
                return false;
            }

        }


        // GET: PaperMillAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PaperMillAdmin/Create
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

        // GET: PaperMillAdmin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PaperMillAdmin/Edit/5
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

        // GET: PaperMillAdmin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PaperMillAdmin/Delete/5
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
