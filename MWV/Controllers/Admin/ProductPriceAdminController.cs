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
    public class ProductPriceAdminController : Controller
    {

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        // GET: ProductPriceAdmin
        public ActionResult Index(int? page, string msg)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            List<Product_prices> lstProductprices;
            ProductPriceRepository objProductPrice = new ProductPriceRepository();
            lstProductprices = objProductPrice.GetListOfProductsPrice();
            // Sort by  Customer name, product_code
            var lstProductpricesSorted = (from e in lstProductprices
                                          orderby e.Customer.name, e.product_code
                                          select e);

            ViewBag.PageData = lstProductpricesSorted.ToPagedList(pageNumber, pageSize);
            ViewBag.Message = msg; //use this viewbag msg for add/edit/delete msg on index page. bydefault it is null.
            return View("Index");
        }

        // GET: ProductPriceAdmin/Details/5
        public ActionResult Details(int id)
        {
            List<Product_prices> lstProductPrice;
            ProductPriceRepository objProdPrice = new ProductPriceRepository();
            lstProductPrice = objProdPrice.GetListOfProductsPrice();
            var ProdPrice = lstProductPrice.Find(x => x.product_price_id == id);
            decimal UnitPrice = Math.Round(ProdPrice.unit_price, 2);
            ProdPrice.unit_price = UnitPrice;
            return View("Edit", ProdPrice);
        }

        // GET: ProductPriceAdmin/Create
        public ActionResult Create()
        {
            ViewBag.CustList = new SelectList(db.Customers.OrderBy(x => x.name), "customer_id", "name");
            ViewBag.PrductCode = new SelectList(db.Products, "product_code", "product_code");
            ViewBag.ShadeCode = new SelectList(db.Shades, "shade_code", "shade_code");
            return View();
        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public string AddProductPrice()
        {
            int prodAdded = 0;
            try
            {
                bool isAdded;

                string id = User.Identity.GetUserId();
                string created_by;
                created_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();
                int CustomerID = Convert.ToInt32(Request["CustomerID"]);
                string ProductCode = Request["ProductCode"].Trim();
                string ShadeCode = Request["ShadeCode"].Trim();
                decimal UnitPrice = Convert.ToDecimal(Request["UnitPrice"].Trim());
                DateTime StartDate = Convert.ToDateTime(Request["StartDate"]);
                DateTime EndDate = Convert.ToDateTime(Request["EndDate"]);
                EndDate = EndDate.AddHours(23);
                EndDate = EndDate.AddMinutes(59);
                EndDate = EndDate.AddSeconds(59);


                /*check pair CustomerID, ProductCode, ShadeCode exists 
                 if pair not exists then add fresh record.
                 * if pair exists then check date validation
                 * */
                string msgCode = CheckCustProdShadeExists(0, CustomerID, ProductCode, ShadeCode, StartDate, EndDate, true);
                List<string> msgCodeAndData = msgCode.Split('-').ToList<string>();
                prodAdded = Convert.ToInt32(msgCodeAndData[0]);

                if (prodAdded == 3)
                {
                    ProductPriceRepository objProdPrice = new ProductPriceRepository();
                    isAdded = objProdPrice.AddProductPrice(CustomerID, ProductCode, ShadeCode, UnitPrice, StartDate, EndDate, created_by);
                    if (isAdded == true)//successfully added in db.
                        msgCode = "3";
                    else
                        msgCode = "0";
                }
                return msgCode;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductPriceAdminController->AddProduct:", Ex);
                return "0";
            }

        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public string EditProductPrice()
        {
            int prodEdited = 0;
            try
            {
                bool isEdited;
                string id = User.Identity.GetUserId();
                string modified_by;
                modified_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();

                int ProductPriceID = Convert.ToInt32(Request["ProductPriceID"].Trim());
                int CustomerID = Convert.ToInt32(Request["CustomerID"].Trim());
                string ProductCode = Request["ProductCode"].Trim();
                string ShadeCode = Request["ShadeCode"].Trim();
                decimal UnitPrice = Convert.ToDecimal(Request["UnitPrice"].Trim());
                DateTime StartDate = Convert.ToDateTime(Request["StartDate"]);
                DateTime EndDate = Convert.ToDateTime(Request["EndDate"]);
                EndDate = EndDate.AddHours(23);
                EndDate = EndDate.AddMinutes(59);
                EndDate = EndDate.AddSeconds(59);

                string msgCode = CheckCustProdShadeExists(ProductPriceID, CustomerID, ProductCode, ShadeCode, StartDate, EndDate, false);
                List<string> msgCodeAndData = msgCode.Split('-').ToList<string>();
                prodEdited = Convert.ToInt32(msgCodeAndData[0]);

                if (prodEdited == 3)
                {
                    ProductPriceRepository objProdPrice = new ProductPriceRepository();
                    isEdited = objProdPrice.EditProductPrice(ProductPriceID, CustomerID, ProductCode, ShadeCode, UnitPrice, StartDate, EndDate, modified_by);
                    if (isEdited == true)//successfully edited in db. 
                        msgCode = "3";
                    else
                        msgCode = "0";
                }
                return msgCode;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductPriceAdminController->EditProductPrice:", Ex);
                return "0";
            }

        }


        public string CheckCustProdShadeExists(int ProductPriceID, int CustomerID, string ProductCode, string ShadeCode, DateTime StartDate, DateTime EndDate, bool isAddOperation)
        {
            string prodPriceAdd = string.Empty;
            List<Product_prices> lstProductprices;
            ProductPriceRepository objProductPrice = new ProductPriceRepository();
            lstProductprices = objProductPrice.GetListOfProductsPrice();

            List<Product_prices> lstPairExists = lstProductprices.Where(m => m.customer_id == CustomerID
                   && m.product_code == ProductCode && m.shade_code == ShadeCode).ToList();

            if (isAddOperation == true)
            {
                if (lstPairExists.Count > 0)//pair already exists , then check date validation
                {
                    //check if same st date and end date entered
                    foreach (var item in lstPairExists)
                    {
                        if (item.start_date == StartDate && item.end_date == EndDate)
                        {
                            prodPriceAdd = "1"; // same record enterd by user with same date duration.
                            break;
                        }
                        else if (StartDate >= item.start_date && StartDate <= item.end_date)
                        {

                            string OvrLappingStDt = item.start_date.Value.Date.ToString("d");
                            string OvrLappingEndDate = item.end_date.Value.Date.ToString("d");
                            prodPriceAdd = "2" + "-" + OvrLappingStDt + "-" + OvrLappingEndDate; //date overlapping.
                            break;
                        }
                        else
                            prodPriceAdd = "3";//can add.
                    }
                }
                else if (lstPairExists == null || lstPairExists.Count == 0)// no pair exist , add fresh record.
                {
                    prodPriceAdd = "3";
                }
            }
            else  ///for edit
            {
                var objProdPriceToEdit = lstProductprices.Find(m => m.product_price_id == ProductPriceID);
                lstProductprices.Remove(objProdPriceToEdit);

                List<Product_prices> lstOtherPairToCheck = lstProductprices.Where(m => m.customer_id == CustomerID
                     && m.product_code == ProductCode && m.shade_code == ShadeCode).ToList();

                if (lstOtherPairToCheck.Count > 0)//if other pair exists , then check date validation.
                {
                    //check if same st date and end date entered
                    foreach (var item in lstOtherPairToCheck)
                    {
                        if (StartDate >= item.start_date && StartDate <= item.end_date)
                        {
                            string OvrLappingStDt = item.start_date.Value.Date.ToString("d");
                            string OvrLappingEndDate = item.end_date.Value.Date.ToString("d");
                            prodPriceAdd = "2" + "-" + OvrLappingStDt + "-" + OvrLappingEndDate; //date overlapping.
                            break;
                        }
                        else
                            prodPriceAdd = "3";//can edit.
                    }
                }
                else if (lstOtherPairToCheck == null || lstOtherPairToCheck.Count == 0)// no other pair exists , allow to edit same record.
                {
                    prodPriceAdd = "3";
                }
            }
            return prodPriceAdd;
        }

        // POST: ProductPriceAdmin/Create
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

        // GET: ProductPriceAdmin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductPriceAdmin/Edit/5
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

        // GET: ProductPriceAdmin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductPriceAdmin/Delete/5
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
