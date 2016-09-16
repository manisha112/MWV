using MWV.Models;
using MWV.Repository.Implementation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MWV.Controllers
{
    public class StakeholderController : Controller
    {
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
        // GET: /Stakeholder/
        [HandleModelStateException]
        public ActionResult Index()
        {
            StockRepository stockobj = new StockRepository();
            ProductionPlannerRepository ProObj = new ProductionPlannerRepository();

            ViewBag.lstPapermills = new SelectList(ProObj.PaperMillList(), "papermill_id", "name");

            List<Agent> AgentList = stockobj.GetAgentList();
            ViewBag.AgentList = new SelectList(AgentList, "agent_id", "name");
            return View("Dashboard");
        }
        [HandleModelStateException]
        public PartialViewResult GetReportResult()
        {
            string name = TempData["filename"].ToString();
            string[] filename = name.Split('.').ToArray();
            ViewBag.name = filename[0];
            ViewBag.finalPath = TempData["finalPath"];
            ViewBag.filepath = TempData["filename"];
            ViewBag.noRecords = TempData["NoRecords"];
            ViewBag.CreatedOn = DateTime.Now.ToString("dd MMM yyyy hh:mm tt");
            return PartialView("_ReportsDownload");
        }
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
                    cst.ValueForCust = cust.label;
                    custNew.Add(cst);
                }
                return Json(custNew.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in StackholderController->GetSearchResult", Ex);
                return null;
            }


        }

    


        //For Showing 7 Day Latest Report
        [HandleModelStateException]
        public PartialViewResult GetlastRecords()
        {
            try
            {
                StackholderRepository ObjReports = new StackholderRepository();
                string rptname = "";
                var data = ObjReports.GetLastRecords(rptname);
                ViewBag.recordsData = data;
                ViewBag.recordsDataCount = data.Count();
                //ViewBag.filePath = data.Select(x => x.file_name);
                return PartialView("_ShowRecentReports");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in StackholderController->GetlastRecords", Ex);
                return null;
            }
        }

        //For Showing Report by Selected Dropdown
        [HandleModelStateException]
        public PartialViewResult GetRecentRecordsByReport(string reportName)
        {
            try
            {
                StackholderRepository ObjReports = new StackholderRepository();
                var data = ObjReports.GetLastRecords(reportName);
                ViewBag.recordsData = data;
                ViewBag.recordsDataCount = data.Count();
                return PartialView("_ShowRecentReports");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in StackholderController->GetlastRecords", Ex);
                return null;
            }
        }
    }
}