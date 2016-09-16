using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWV.Repository.Implementation;
using MWV.DBContext;
using MWV.Models;
using PagedList;
using PagedList.Mvc;
using System.Text.RegularExpressions;
using System.Reflection;
namespace MWV.Controllers
{
    public class FinanceHeadController : Controller
    {
        private MWVDBContext db = new MWVDBContext();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
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
        //
        // GET: /FinanceHead/ Home
        [HandleModelStateException]
        public ActionResult Index()
        {
            try
            {
                FinanceHeadRepository ObjFinance = new FinanceHeadRepository();
                var Customers = ObjFinance.CustListApproved();
                ViewBag.customer_list = new SelectList(Customers, "customer_id", "name");
                var custs = ObjFinance.GetAlphawiseName();
                // for displaying the list for 'selectCustomerAlphabetically'
                ViewBag.custlist_alphabetically = new SelectList(custs, "", "");
                return View("DashBoard");
            }
            catch (Exception ex)
            {

                ViewBag.ErrorMsg = "error in Finance controller--> Index Action Method" + ex.InnerException;
                return View("Error");
            }

        }
        //Get Selected Customer
        [HandleModelStateException]
        public PartialViewResult GetFinanceCustomer(int id)
        {
            try
            {
                int selectedCustomerid = id;
                FinanceHeadRepository GetCustomer = new FinanceHeadRepository();
                var result = GetCustomer.GetListCustomer(id);
                ViewBag.GetFinance = result;
                foreach (var i in result)
                {
                    ViewBag.CustName = i.CustomerName;
                    ViewBag.Address1 = i.Addess1;
                    ViewBag.Address2 = i.Addess2;
                    ViewBag.Address3 = i.Addess3;
                    ViewBag.AgentName = i.AgentName;
                    ViewBag.CreditLimit = i.CreditLimit;
                    ViewBag.CreditDay = i.CreditDays;
                    ViewBag.fax = i.fax;
                    ViewBag.pincode = i.PinNo;
                    ViewBag.city = i.City;
                    ViewBag.state = i.State;
                    ViewBag.country = i.Country;
                    ViewBag.phone = i.Phone;

                }
                return PartialView("_FinanceCustomer");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in FinanceHead->GetFinanceCustomer:", Ex);
                return null;
            }

        }
        //Update Customer Details
        [HandleModelStateException]
        public ActionResult UpdateCustomerDetails()
        {
            try
            {
                int selectedCustomerid = Convert.ToInt16(Request["selectedCustomerid"]);
                int CreditDays = Convert.ToInt16(Request["CreditDays"]);
                decimal CreditAmt = Convert.ToDecimal(Request["CreditAmt"]);
                FinanceHeadRepository ObjFinance = new FinanceHeadRepository();
                ObjFinance.UpdateOrderProductQty(CreditDays, CreditAmt, selectedCustomerid);
                return View();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in FinanceHead->UpdateCustomerDetails:", Ex);
                return null;
            }

        }
        //Get Customer List By Selected Alphbet
        [HandleModelStateException]
        public PartialViewResult GetCustomersbyAlphabet(int? page)
        {
            try
            {
                string alphabet = string.Empty;
                int pageSize = 5;
                int pageNumber = (page ?? 1);
                if (page == null)
                {
                    Session["alphabet"] = Request["selectedAlphabet"];
                }
                alphabet = Session["alphabet"].ToString();
                // get the posted alphabet from the ajax

                FinanceHeadRepository ObjGetAlpha = new FinanceHeadRepository();
                var AlphaWise = ObjGetAlpha.GetListAlphawise(alphabet);
                var pagecount = AlphaWise.Count;
                ViewBag.CustmoerList = AlphaWise.ToPagedList(pageNumber, pageSize);
                ViewBag.PageSizeForPagi = pagecount;
                return PartialView("_GetCustomerByAlpha");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in FinanceHead->GetCustomersbyAlphabet:", Ex);
                return null;
            }

        }

        //Get Selected Customer Details Selected In Alphabet 
        [HandleModelStateException]
        public PartialViewResult GetCustomersbyTab()
        {
            // get the posted alphabet from the ajax
            int id = Convert.ToInt16(Request["selectedCustomerid"]);
            FinanceHeadRepository GetCustomer = new FinanceHeadRepository();
            var result = GetCustomer.GetListCustomer(id);
            foreach (var i in result)
            {
                ViewBag.CustId = i.customer_id;
                ViewBag.CustName = i.CustomerName;
                ViewBag.Address1 = i.Addess1;
                ViewBag.Address2 = i.Addess2;
                ViewBag.Address3 = i.Addess3;
                ViewBag.AgentName = i.AgentName;
                ViewBag.CreditLimit = i.CreditLimit;
                ViewData["CreditDays"] = i.CreditDays;
                ViewBag.city = i.City;
                ViewBag.state = i.State;
                ViewBag.country = i.Country;
                ViewBag.phone = i.Phone;
                ViewBag.fax = i.fax;
                ViewBag.pincode = i.PinNo;

            }
            return PartialView("_GetListSelected");
        }
        //For Search Customer by any Letter
        [HandleModelStateException]
        public JsonResult GetSearchResult(string query)
        {
            try
            {
                FinanceHeadRepository Obj = new FinanceHeadRepository();
                var result = Obj.SearchCustomer(query);
                List<Cust> custNew = new List<Cust>();
                foreach (Cust cust in result)
                {
                    Cust cst = new Cust();
                    cst.label = cust.label;
                    cst.value = cust.value;
                    cst.address1 = cust.address1;
                    cst.address2 = cust.address2;
                    cst.address3 = cust.address3;
                    cst.agentname = cust.agentname;
                    cst.phone = cust.phone;
                    cst.fax = cust.fax;
                    cst.email = cust.email;
                    cst.city = cust.city;
                    cst.state = cust.state;
                    cst.pincode = cust.pincode;
                    cst.country = cust.country;
                    cst.CreditDays = cust.CreditDays;
                    cst.CreditLimit = cust.CreditLimit;
                    custNew.Add(cst);
                }
                return Json(custNew.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in FinanceHead->GetSearchResult:", Ex);
                return null;
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