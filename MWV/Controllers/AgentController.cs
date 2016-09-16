using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MWV.Models;
using MWV.DBContext;
using MWV.Repository.Implementation;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Reflection;
using Microsoft.AspNet.Identity.Owin;
using IdentitySample.Models;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using OfficeOpenXml;
using IdentitySample.Controllers;

namespace MWV.Controllers
{
    public class AgentController : Controller
    {
        private MWVDBContext db = new MWVDBContext();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        OrderRepository objOrder = new OrderRepository();
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
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: /Agent/
        [HandleModelStateException]
        public ActionResult Index()
        {
            try
            {
                ProductionPlannerRepository ProObj = new ProductionPlannerRepository();
                ViewBag.lstPapermills = new SelectList(ProObj.PaperMillList(), "papermill_id", "name");

                // get the agent name and put it in the ViewBag
                string agentname = objOrder.GetAgentName();
                ViewBag.agentname = agentname;

                var Bfs = objOrder.GetBfs();
                ViewBag.Bf_list = new SelectList(Bfs, "bf_code", "description");

                // Get the gsm list
                var Gsms = objOrder.GetGsms();
                ViewBag.Gsm_list = new SelectList(Gsms, "gsm_code", "description");

                // Get the bf list
                var Shades = objOrder.GetShades();
                ViewBag.Shade_list = new SelectList(Shades, "shade_code", "description");

                //make the list of core and save in ViewBag
                var lstCores = objOrder.GetCores();
                ViewBag.Core_list = new SelectList(lstCores, "core_code", "description");

                // get the papermill list
                var Papermills = objOrder.GetPapermills();
                ViewBag.papermill_list = new SelectList(Papermills, "papermill_id", "location");

                // For Customer Tab

                ////for displayig the List for 'Select Customer' from drop down
                CustomerRepository objCust = new CustomerRepository();
                var Customers = objCust.CustListbyAgentId();

                // get the count of customers
                Session["TotalCustomers"] = Customers.Count();
                ViewBag.customer_list = new SelectList(Customers, "customer_id", "name");

                var custs = (from p in Customers
                             let first = p.name.Substring(0, 1)
                             orderby first
                             select first.ToUpper()).Distinct();

                // for displaying the list for 'selectCustomerAlphabetically'
                //var lstCustomer= objCust.GetCustomersAlphabetically();
                ViewBag.custlist_alphabetically = new SelectList(custs, "", "");
                Session["fromDuplicateBtn"] = 0;
                return View("DashBoard");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMsg = "error in Agent controller--> Index Action Method" + ex.InnerException;
                return View("Error");
            }

        }


        [HandleModelStateException]
        public ActionResult ScheduleDetails(int id)
        {
            ProductionPlannerRepository ProObj = new ProductionPlannerRepository();
            var scheduleDetails = (from sch in db.Schedule
                                   where sch.papermill_id == id && sch.status == "Active"
                                   orderby sch.start_date ascending
                                   select sch).ToList();
            ViewBag.scheduleDetails = scheduleDetails;
            if (!scheduleDetails.Any())
            {
                ViewBag.norecordMsg = "No schedule !";
            }
            return PartialView("_Schedule");
        }
        [HandleModelStateException]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agent agent = db.Agents.Find(id);
            if (agent == null)
            {
                return HttpNotFound();
            }
            return View(agent);
        }

        public Boolean Creates()
        {
            CustomerRepository obj = new CustomerRepository();
            obj.AgentReport(1, "all", Convert.ToDateTime(2015 - 07 - 06), DateTime.Now);
            return true;
        }
        //GET: /Agent/Create

        // POST: /Agent/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "agent_id,name,email,Mobile,landline,address,aspnetusers_id")] Agent agent)
        {
            if (ModelState.IsValid)
            {
                db.Agents.Add(agent);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(agent);
        }

        // GET: /Agent/Edit/5
        [HandleModelStateException]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agent agent = db.Agents.Find(id);
            if (agent == null)
            {
                return HttpNotFound();
            }
            return View(agent);
        }

        // POST: /Agent/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "agent_id,name,email,Mobile,landline,address,aspnetusers_id")] Agent agent)
        {
            if (ModelState.IsValid)
            {
                db.Entry(agent).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(agent);
        }

        // GET: /Agent/Delete/5
        [HandleModelStateException]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agent agent = db.Agents.Find(id);
            if (agent == null)
            {
                return HttpNotFound();
            }
            return View(agent);
        }

        // POST: /Agent/Delete/5
        [HttpPost, ActionName("Delete")]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Agent agent = db.Agents.Find(id);
            db.Agents.Remove(agent);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HandleModelStateException]
        public string GetCustomerNamebyOrderId(int id)
        {
            int orderid = id;
            // step 1: get order from 'Orders' table with this orderid
            var order = db.Orders.Where(p => p.order_id == orderid).First();
            // get customerid from order record
            int? customerid = order.customer_id;

            // step 2: get customer name from 'Customers' table
            var customer = db.Customers.Where(k => k.customer_id == customerid).First();
            string custname = customer.name;
            return custname;
        }

        //Added By Sagar For AutoSearch Customer 05-08-2015
        [HandleModelStateException]
        public JsonResult GetSearchResult(string query)
        {
            try
            {
                OrderRepository objOrder = new OrderRepository();
                int LoggedInAgent_id = objOrder.GetAgentID();
                CustomerRepository objCust = new CustomerRepository();
                var result = objCust.CustSearchResult(LoggedInAgent_id, query);
                List<Cust> custNew = new List<Cust>();
                foreach (Customer cust in result)
                {
                    Cust cst = new Cust();
                    cst.label = cust.name;
                    cst.value = cust.customer_id;
                    cst.address1 = cust.address1;
                    cst.address2 = cust.address2;
                    cst.address3 = cust.address3;
                    //cst.agentName = cust;
                    cst.phone = cust.phone;
                    cst.fax = cust.fax;
                    cst.email = cust.email;
                    cst.city = cust.city;
                    cst.state = cust.state;
                    cst.pincode = cust.pincode;
                    cst.country = cust.country;
                    cst.CreditDays = cust.credit_days;
                    cst.CreditLimit = cust.credit_limit;
                    custNew.Add(cst);
                }
                return Json(custNew.ToList(), JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentController->GetSearchResult:", Ex);
                return null;

            }


        }
        // For Download order Pdf
        [HandleModelStateException]
        public ActionResult GetPdffile(string orderid)
        {

            try
            {
                string fullName = "";
                OrderRepository objrep = new OrderRepository();
                string agentname = objrep.GetAgentName();
                string filename = string.Empty;
                if (agentname == null)
                {
                    filename = "_PO_" + orderid;
                }
                else
                {
                    agentname = agentname.Replace(" ", "_");
                    filename = agentname + "_PO_" + orderid;

                }
                DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(Server.MapPath("~/MWV/PDF/"));
                FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + filename + "*.pdf*", SearchOption.AllDirectories);
                foreach (FileInfo foundFile in filesInDir)
                {
                    fullName = foundFile.FullName;
                }
                string filenamePdf = Path.GetFileNameWithoutExtension(fullName);
                Response.AddHeader("Content-Disposition", "attachment; filename=" + filenamePdf + ".pdf");
                return File(fullName, Response.ContentType);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentController->GetPdffile:", Ex);
                return null;

            }


        }

        //For Binding All Years Dropdown by current year and next 1 year
        public ActionResult GetCurrentyear()
        {

            var CurDate = DateTime.Now.Year;
            var Nextyrs = DateTime.Now.Year + 1;
            List<string> lst = new List<string>();
            lst.Clear();
            lst.Add(CurDate.ToString());
            lst.Add(Nextyrs.ToString());

            return Json(lst, JsonRequestBehavior.AllowGet);
        }
        [HandleModelStateException]
        public PartialViewResult ShowChangePassword()
        {
            return PartialView("_AgentChangePassword");
        }
        [HttpPost]
        [HandleModelStateException]
        public async Task<PartialViewResult> ShowChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("~/Views/Agent/_AgentChangePassword.cshtml", model);
                }
                var chkOldpass = await UserManager.FindAsync(User.Identity.Name, model.OldPassword);
                if (chkOldpass != null)
                {
                    var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                    if (result != null)
                    {
                        var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                        if (user != null)
                        {
                            //await SignInAsync(user, isPersistent: false);
                        }
                        //return RedirectToAction("AdminHome", "Home", new { Message = ManageMessageId.ChangePasswordSuccess });
                    }

                    ModelState.AddModelError("", result.Errors.First());
                    //return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordFailed });
                }
                else
                {
                    ViewBag.IncorrectPass = "Incorrect Old Password!";
                    //ModelState.AddModelError("", chkOldpass.First());
                    return PartialView("_AgentChangePassword", model);
                }
                // return View(model);
                return PartialView("_AgentChangePassword", model);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentController->ChangedAgentPassword:", Ex);
                return PartialView("~/Views/Agent/_AgentChangePassword.cshtml", model);
            }

        }



        public ActionResult FgReportGetData()
        {
            try
            {

                //var query=(from or in db.Orders join op in db.Order_products on or.order_id equals op.order_id
                //           join ag in db.Agents )

                return View();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentController->FgReportGetData:", Ex);
                return null;
            }
        }

     
        [HandleModelStateException]
        public PartialViewResult GetAgentList()
        {
            StockRepository stockobj = new StockRepository();
            List<Agent> AgentList = stockobj.GetAgentList();
            ViewBag.AgentList = new SelectList(AgentList, "agent_id", "name");
            //ViewBag.AgentList = AgentList;
            return PartialView("_Reports");
        }
        public PartialViewResult GetReportResult(string filename)
        {
            ViewBag.finalPath = TempData["finalPath"];
            ViewBag.filepath = TempData["filename"];
            return PartialView("_ReportsDownload");
        }
    }
}
