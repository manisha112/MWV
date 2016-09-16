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
    public class ProductAdminController : Controller
    {

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();
        // GET: ProductAdmin
        public ActionResult Index(string msg)
        {
            List<Product> lstProduct;
            ProductRepository objProduct = new ProductRepository();
            lstProduct = objProduct.GetListOfProducts();
            ViewBag.Message = msg; //use this viewbag msg for add/edit/delete msg on index page. bydefault it is null.
            return View(lstProduct.ToList());
        }

        // GET: ProductAdmin/Details/5
        public ActionResult Details(string id)
        {
            List<Product> lstProduct;
            ProductRepository objProd = new ProductRepository();
            lstProduct = objProd.GetListOfProducts();
            var ProdCode = lstProduct.Find(x => x.product_code == Convert.ToString(id));
            ViewBag.gsm_list = new SelectList(db.Gsms, "gsm_code", "description");
            ViewBag.bf_list = new SelectList(db.Bfs, "bf_code", "description");
            return View("Edit", ProdCode);
        }

        // GET: ProductAdmin/Create
        public ActionResult Create()
        {
            ViewBag.gsm_list = new SelectList(db.Gsms, "gsm_code", "description");
            ViewBag.bf_list = new SelectList(db.Bfs, "bf_code", "description");
            return View();
        }

        // POST: ProductAdmin/Create
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

        // GET: ProductAdmin/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProductAdmin/Edit/5
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

        // GET: ProductAdmin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductAdmin/Delete/5
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

        public int CheckDuplicateProductCodeAndGsmBF(string ProductCode, string GSMCode, string BFCode, bool isAddOperation)
        {
            int DuplicateCode = 0;
            List<Product> lstProduct;
            ProductRepository objProduct = new ProductRepository();
            lstProduct = objProduct.GetListOfProducts().ToList();

            if (isAddOperation == true)
            {
                var dupProdCode = lstProduct.Find(x => x.product_code.Trim() == Convert.ToString(ProductCode));
                var dupGSMBF = lstProduct.Find(c => c.gsm_code.Trim() == GSMCode && c.bf_code.Trim() == BFCode);

                if (dupProdCode != null)//check duplicate prod while adding.
                {
                    DuplicateCode = 1;//duplicate ProductCode found.
                }
                else if (dupGSMBF != null)//check duplicate GSM BF while adding.
                {
                    DuplicateCode = 2;
                }
                else
                    DuplicateCode = 3; // prod can be added  
            }
            else
            {
                var RemoveObj = lstProduct.Find(x => x.product_code.Trim() == Convert.ToString(ProductCode));
                lstProduct.Remove(RemoveObj);
                var dupGSMBF = lstProduct.Find(c => c.gsm_code.Trim() == GSMCode && c.bf_code.Trim() == BFCode);
                if (dupGSMBF != null)//check duplicate GSM BF while editing.
                {
                    DuplicateCode = 2;
                }
                else
                    DuplicateCode = 3; // prod can be edited.  
            }
            return DuplicateCode;
        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public int AddProduct()
        {
            int prodAdded = 0;
            try
            {
                bool isAdded;
                string id = User.Identity.GetUserId();
                string created_by;
                created_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();
                string ProductCode = Request["ProductCode"].Trim();
                string GSMCode = Request["GSMCode"].Trim();
                string BFCode = Request["BFCode"].Trim();
                string ProductDescription = Request["ProductDescription"].Trim();

                prodAdded = CheckDuplicateProductCodeAndGsmBF(ProductCode, GSMCode, BFCode, true);

                if (prodAdded == 3)
                {
                    ProductRepository objProd = new ProductRepository();
                    isAdded = objProd.AddProduct(ProductCode, GSMCode, BFCode, ProductDescription, created_by);
                    if (isAdded == true)
                        prodAdded = 3;
                    else
                        prodAdded = 0;
                }
                return prodAdded;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductAdminController->AddProduct:", Ex);
                return prodAdded = 0;
            }

        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public int EditProduct()
        {
            int prodEdited = 0;
            try
            {
                bool isEdited;
                string id = User.Identity.GetUserId();
                string modified_by;
                modified_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();
                string ProductCode = Request["ProductCode"].Trim();
                string GSMCode = Request["GSMCode"].Trim();
                string BFCode = Request["BFCode"].Trim();
                string ProductDescription = Request["ProductDescription"].Trim();

                prodEdited = CheckDuplicateProductCodeAndGsmBF(ProductCode, GSMCode, BFCode, false);

                if (prodEdited == 3)
                {
                    ProductRepository objProd = new ProductRepository();
                    isEdited = objProd.EditProduct(ProductCode, GSMCode, BFCode, ProductDescription, modified_by);
                    if (isEdited == true)
                        prodEdited = 3;
                    else
                        prodEdited = 0;
                }
                return prodEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductAdminController->AddProduct:", Ex);
                return prodEdited = 0;
            }
        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public ActionResult DeleteDetails(string id)
        {
            List<Product> lstProduct;
            ProductRepository objProduct = new ProductRepository();
            lstProduct = objProduct.GetListOfProducts();
            var ProductCode = lstProduct.Find(x => x.product_code == Convert.ToString(id));
            return View("Delete", ProductCode);
        }

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool DeleteProduct()
        {
            try
            {
                bool isDeleted;
                string ProductCode = Request["ProductCode"];
                // string BFDescription = Request["BFDescription"].Trim();
                ProductRepository objProd = new ProductRepository();
                isDeleted = objProd.DeleteProduct(ProductCode);
                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductAdminController->DeleteProduct:", Ex);
                return false;
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
