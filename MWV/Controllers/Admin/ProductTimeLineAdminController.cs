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
    public class ProductTimeLineAdminController : Controller
    {

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        // GET: ProductTimeLineAdmin
        public ActionResult Index(int? page, string msg)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            List<ProductionTimeline> lstProductTimeline;
            ProductTimeLineRepository objProductTimeline = new ProductTimeLineRepository();
            lstProductTimeline = objProductTimeline.GetListOfProductTimeLines();
            // Sort by  papermill, bf, gsm, shade
            var lstProductTimelineSorted = (from e in lstProductTimeline
                                            orderby e.PaperMill.name, e.bf_code, e.gsm_code, e.shade_code
                                            select e);

            ViewBag.PageData = lstProductTimelineSorted.ToPagedList(pageNumber, pageSize);
            ViewBag.Message = msg; //use this viewbag msg for add/edit/delete msg on index page. bydefault it is null.
            return View("Index");
        }

        // GET: ProductTimeLineAdmin/Details/5
        public ActionResult Details(int id)
        {
            List<ProductionTimeline> lstProductTL;
            ProductTimeLineRepository objProdTL = new ProductTimeLineRepository();
            lstProductTL = objProdTL.GetListOfProductTimeLines();
            var ProdCode = lstProductTL.Find(x => x.production_timeline_id == id);
            return View("Edit", ProdCode);
        }

        // GET: ProductTimeLineAdmin/Create
        public ActionResult Create()
        {
            ViewBag.papermill_list = new SelectList(db.Papermills, "papermill_id", "name");
            ViewBag.bf_list = new SelectList(db.Bfs, "bf_code", "description");
            ViewBag.gsm_list = new SelectList(db.Gsms, "gsm_code", "description");
            ViewBag.shadecode_list = new SelectList(db.Shades, "shade_code", "shade_code");
            return View();
        }


        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public int AddProductTimeLine()
        {
            int prodTLAdded = 0;
            try
            {
                bool isAdded;

                string id = User.Identity.GetUserId();
                string created_by;
                created_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();
                int PaperMillID = Convert.ToInt32(Request["PaperMillID"]);
                string BFCode = Request["BFCode"].Trim();
                string GSMCode = Request["GSMCode"].Trim();
                string ShadeCode = Request["ShadeCode"].Trim();
                int Speed = Convert.ToInt32(Request["speed"]);
                decimal TonPrHr = Convert.ToDecimal(Request["TonPrHr"].Trim());
                decimal TimePrTon = Convert.ToDecimal(Request["TimePrTon"].Trim());


                /*check pair PaperMillID, BFCode, GSMCode,ShadeCode exists 
                 if pair not exists then add fresh record.
                 * */
                prodTLAdded = CheckPaperMillBfGsmShadeExists(PaperMillID, BFCode, GSMCode, ShadeCode, true);

                if (prodTLAdded == 3)
                {
                    ProductTimeLineRepository objProdTL = new ProductTimeLineRepository();
                    isAdded = objProdTL.AddProductTimeLine(PaperMillID, BFCode, GSMCode, ShadeCode, Speed, TonPrHr, TimePrTon, created_by);
                    if (isAdded == true)
                        prodTLAdded = 3;
                    else
                        prodTLAdded = 0;
                }
                return prodTLAdded;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductTimeLineAdminController->AddProductTimeLine:", Ex);
                return prodTLAdded = 0;
            }

        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool EditProductTimeLine()
        {
            try
            {
                bool isEdited;

                string id = User.Identity.GetUserId();
                string modified_by;
                modified_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();
                int ProductTLIDToEdit = Convert.ToInt32(Request["ProductTLIDToEdit"]);
                int Speed = Convert.ToInt32(Request["speed"]);
                decimal TonPrHr = Convert.ToDecimal(Request["TonPrHr"].Trim());
                decimal TimePrTon = Convert.ToDecimal(Request["TimePrTon"].Trim());

                ProductTimeLineRepository objProdTL = new ProductTimeLineRepository();
                isEdited = objProdTL.EditProductTimeLine(ProductTLIDToEdit, Speed, TonPrHr, TimePrTon, modified_by);
                return isEdited;

            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductTimeLineAdminController->EditProductTimeLine:", Ex);
                return false;
            }

        }


        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public ActionResult DeleteDetails(int id)
        {
            List<ProductionTimeline> lstProductTL;
            ProductTimeLineRepository objProductTL = new ProductTimeLineRepository();
            lstProductTL = objProductTL.GetListOfProductTimeLines();
            var ProductTL = lstProductTL.Find(x => x.production_timeline_id == id);
            return View("Delete", ProductTL);
        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool DeleteProductTimeLine()
        {
            try
            {
                bool isDeleted;
                int ProductTLIDToDel = Convert.ToInt32(Request["ProductTLIDToDel"]);
                int PaperMillID = Convert.ToInt32(Request["PaperMillID"]);
                string BFCode = Request["BFCode"].Trim();
                string GSMCode = Request["GSMCode"].Trim();
                string ShadeCode = Request["ShadeCode"].Trim();
                ProductTimeLineRepository objProdTL = new ProductTimeLineRepository();
                isDeleted = objProdTL.DeleteProductTimeLine(ProductTLIDToDel, PaperMillID, BFCode, GSMCode, ShadeCode);
                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductAdminController->DeleteProduct:", Ex);
                return false;
            }


            return false;
        }



        public int CheckPaperMillBfGsmShadeExists(int PaperMillID, string BFCode, string GSMCode, string ShadeCode, bool isAddOperation)
        {
            int ProdTLAdd = 0;
            List<ProductionTimeline> lstProductTL;
            ProductTimeLineRepository objProductTL = new ProductTimeLineRepository();

            lstProductTL = objProductTL.GetListOfProductTimeLines();

            List<ProductionTimeline> lstPairExists = lstProductTL.Where(m => m.papermill_id == PaperMillID
                   && m.bf_code == BFCode && m.gsm_code == GSMCode && m.shade_code == ShadeCode).ToList();

            if (isAddOperation == true)
            {
                if (lstPairExists.Count > 0)//pair already exists.
                {
                    ProdTLAdd = 2;
                }
                else if (lstPairExists == null || lstPairExists.Count == 0)// no pair exist , add fresh record.
                {
                    ProdTLAdd = 3;
                }
            }
            //else  ///for delete
            //{
            //    List<deckle_approvals> lstDeckApp;
            //    ProductTimeLineRepository objProductTL1 = new ProductTimeLineRepository();
            //    lstDeckApp = objProductTL1.GetListOfDeckleApprovals();

            //    List<deckle_approvals> lstDeckPairExists = lstDeckApp.Where(m => m.papermill_id == PaperMillID
            //           && m.bf_code == BFCode && m.gsm_code == GSMCode && m.shade_code == ShadeCode).ToList();


            //    if (lstDeckPairExists.Count > 0)//if  pair exists in deckle_approvals , dont allow for deletion
            //    {
            //        ProdTLAdd = 2;
            //    }
            //    else if (lstDeckPairExists == null || lstDeckPairExists.Count == 0)// if no  pair exists in deckle_approvals, allow to delete.
            //    {
            //        prodPriceAdd = 3;
            //    }
            //}
            return ProdTLAdd;
        }

        // POST: ProductTimeLineAdmin/Create
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

        // GET: ProductTimeLineAdmin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductTimeLineAdmin/Edit/5
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

        // GET: ProductTimeLineAdmin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductTimeLineAdmin/Delete/5
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
