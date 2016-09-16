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
using Microsoft.AspNet.Identity;
using MWV.Repository.Implementation;
using PagedList;
using PagedList.Mvc;
using MWV.Business;
using System.Reflection;

using MWV.Repository.Interfaces;
using MWV.Controllers;
using System.Text.RegularExpressions;




namespace MWV.Controllers
{
    public class CustomerController : Controller
    {
        private MWVDBContext db = new MWVDBContext();
        private MessageAndAlertsBusiness msgAlertObj = new MessageAndAlertsBusiness();
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

        // GET: /Customer/
        [HandleModelStateException]
        public ActionResult Index()
        {
            return View("DashBoard");

        }
        [HandleModelStateException]
        public ActionResult OrdersSearchResultsByCustomerPO(string allVals, int? page)
        {
            CustomerRepository custrepObj = new CustomerRepository();
            if (page == null)
            {
                Session["AllcustomerPO"] = null;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            string allcustPO = string.Empty;
            if (Session["AllcustomerPO"] == null)
            {
                Session["AllcustomerPO"] = allVals;
                allcustPO = allVals;
            }
            else
            {
                allcustPO = Session["AllcustomerPO"].ToString();
            }
            var ordersList = custrepObj.OrdersSearchResultsByCustomerPO(allcustPO);
            ViewBag.ordersList = ordersList.ToPagedList(pageNumber, pageSize);
            if (!ordersList.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            ViewBag.Pagesize = ordersList.Count();
            return PartialView("_SearchCustomerOrders");

        }
        [HandleModelStateException]
        public PartialViewResult CreateCustomer([Bind(Exclude = "agent_id", Include = "name,email,phone,address1,address2,address3,city,pincode,state,country,fax")] Customer customer)
        {
            try
            {
                List<Customer> lst = new List<Customer>();
                lst.Add(customer);
                string id = User.Identity.GetUserId();
                int LoggedInAgent_id; string agentName;
                LoggedInAgent_id = (from a in db.Agents where a.aspnetusers_id == id select a.agent_id).SingleOrDefault();
                agentName = (from a in db.Agents where a.aspnetusers_id == id select a.name).SingleOrDefault();
                // Get the agent_id from 'Agents' table with this user id.
                if (id != null)
                {

                    ViewBag.Agent_id = LoggedInAgent_id;
                    var Customers = db.Customers.Where(c => c.agent_id == LoggedInAgent_id).FirstOrDefault();

                    customer.agent_id = LoggedInAgent_id;
                    customer.created_by = agentName;
                    customer.created_on = DateTime.Now;

                }
                CustomerRepository objCR = new CustomerRepository();
                bool statusOfCust = objCR.CreateCustomer(customer);
                if (statusOfCust == true)
                {


                    ViewBag.message1 = "Thank you for your submission ";
                    ViewBag.message2 = "Your submission will be reviewed shortly, and you will be notified if it is approved.";
                    Boolean Statusmail = Mailoperation(customer, agentName, LoggedInAgent_id, lst, id);
                    return PartialView("~/Views/Customer/_CustomerCreationSuccess.cshtml", customer);
                }
                else
                {
                    ViewBag.message1 = "";
                    ViewBag.message2 = "Customer could not be added to database";
                    return PartialView("~/Views/Customer/_CustomerCreationSuccess.cshtml", customer);
                }

            }

            catch (Exception ex)
            {

                logger.Error("Error in CustomerController->Create", ex);
                return null;
            }


        }
        //Mail to Production Planner for Order Placed And Set Alerts and Messages to production Planner
        private Boolean Mailoperation(Customer customer, string agentName, int LoggedInAgent_id, List<Customer> lst, string id)
        {
            try
            {
                EmailController emailFunc = new EmailController();
                CustomerRepository objCR = new CustomerRepository();
                string Plannername = objCR.GetProductionPlannerName();
                ViewBag.PlannerName = Plannername;
                ViewBag.CustName = customer.name;
                ViewBag.City = customer.city;
                ViewBag.CreatedBy = customer.created_by;
                ViewBag.agntname = agentName;
                ViewBag.CreatedOn = customer.created_on.Value.ToShortDateString();
                string Msgsub = "New Customer " + customer.name + ", " + customer.city + " added by " + agentName;
                string pdfOutput = emailFunc.GeneratePdfOutput(this.ControllerContext, lst, "NewCustomerCreated");
                UserMailer objusm = new UserMailer();

                string emailtext = objCR.GetEmail();
                string ccmail = objCR.GetAgentmail(LoggedInAgent_id.ToString());
                bool status = objusm.sendMails(emailtext, "", Msgsub, ccmail, "", "", "", pdfOutput, "", "");
                if (status == false)
                {
                    msgAlertObj.CreateMessagesDetails("NewCustomerCreated", "ProductionPlanner", LoggedInAgent_id, pdfOutput, Msgsub, null, null, null, customer.created_by, null, "Failed", null, null);
                }
                else
                {
                    msgAlertObj.CreateMessagesDetails("NewCustomerCreated", "ProductionPlanner", LoggedInAgent_id, pdfOutput, Msgsub, null, null, null, customer.created_by, null, "Deliverd", null, null);
                }
                AlertforNewCustomerCreated(customer.agent_id, customer.name, customer.city, agentName);//alert to planner when new customer is created.
                Dispose(true);
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CreateCustomer->Mailoperation:", Ex);
                return false;
            }


        }
        //Create alert For Production Planner
        private bool AlertforNewCustomerCreated(int agntid, string customerName, string customerCity, string agentName)
        {
            try
            {
                string alertText = agentName + " has created a new customer " + customerName + ", " + customerCity + " and is waiting for Approval";
                string alertSubject = "New customer " + customerName + ", " + customerCity + " added by " + agentName;
                //alert msg to planner when agent creates a new customer
                msgAlertObj.CreateAlertDetails("NewCustomerCreated", "ProductionPlanner", agntid, alertText, alertSubject, agentName, null);
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerController->AlertforNewCustomerCreated:", Ex);
                return false;
            }
        }


        // GET: /Customer/Details/5
        [HandleModelStateException]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }
        [HandleModelStateException]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        [HttpPost]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "customer_id,name,email,phone,address1,agent_id,address2,address3,city,pincode,state,country,fax,status,aspnetusers_id_approvedby,approved_on,created_on,created_by,modified_on,modified_by")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: /Customer/Delete/5
        [HandleModelStateException]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: /Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
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
        public ActionResult IsCustomerExist(string name, string city)
        {
            Customer customer = new Customer();
            string id = User.Identity.GetUserId();
            int LoggedInAgent_id;

            // Get the agent_id from 'Agents' table with this user id.

            LoggedInAgent_id = (from a in db.Agents where a.aspnetusers_id == id select a.agent_id).SingleOrDefault();

            ViewBag.Agent_id = LoggedInAgent_id;
            var Customers = db.Customers.Where(c => c.agent_id == LoggedInAgent_id).FirstOrDefault();

            //check whether name and city for this customer is already available with agent.
            var cust = db.Customers.FirstOrDefault(c => c.name.Equals(name, StringComparison.OrdinalIgnoreCase)
            && c.city.Equals(city, StringComparison.OrdinalIgnoreCase)
            );

            //If not available then create this customer.
            if (cust == null)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            // To check if agent is different for the customer entered.
            else if (cust.agent_id == Customers.agent_id)
            {
                string message = string.Format("Customer already exists and is attached to you with {0} status", Customers.status);
                return Json(message, JsonRequestBehavior.AllowGet);
            }

            else
            {
                string message = string.Format("Customer exists and is attached to another agent. Please contact MWV to transfer the customer to you");
                return Json(message, JsonRequestBehavior.AllowGet);
            }
        }
        [HandleModelStateException]
        public string GetCustomerNameByCustomerId(int id)
        {
            var customer = db.Customers.Where(p => p.customer_id == id).Single();
            Session["custname"] = customer.name;
            return customer.name;
        }
        // this function 
        [HandleModelStateException]
        public PartialViewResult GetCustomerbyAgentId(int id) // here the selected customer id is also needed so that only the selected customer will be shown
        {
            int selectedCustomerid = id;
            OrderRepository objOrderRep = new OrderRepository();
            int LoggedinAgentId = objOrderRep.GetAgentID();
            var lstCustomers = db.Customers.Where(p => p.agent_id == LoggedinAgentId && p.customer_id == selectedCustomerid);
            //foreach(var item in lstCustomers){
            //    //ViewBag.custname = item.name;
            //    Session["custname"] = item.name;
            //}
            return PartialView("_customerList", lstCustomers);
        }

        // this function returns the customer details to the sequence 'Customers' tab --> Select Customer by dropdown 
        [HandleModelStateException]
        public PartialViewResult GetCustomer(int id)
        {
            int selectedCustomerid = id;
            OrderRepository objOrderRep = new OrderRepository();
            int LoggedinAgentId = objOrderRep.GetAgentID();
            var lstCustomers = db.Customers.Where(p => p.agent_id == LoggedinAgentId && p.customer_id == selectedCustomerid);

            return PartialView("_CustDetailsWithOrderSearch", lstCustomers.ToList<Customer>());
        }
        // this funcction returns the orders related to the customer selected in the sequence 'Customers' tab --> Select Customer by dropdown --> 'View' blue button
        [HandleModelStateException]
        public PartialViewResult GetCurrentCustOrders(int? page)
        {
            if (page == null)
            {
                Session["selectedCustomerid"] = null;
                Session["fromDate"] = null;
                Session["toDate"] = null;
            }
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            int selectedCustomerid = 0;
            // get the fromDate and toDate from the posted data
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();

            if (Session["selectedCustomerid"] == null && Session["fromDate"] == null && Session["toDate"] == null)
            {

                Session["fromDate"] = Convert.ToDateTime(Request["fromDate"]);
                Session["toDate"] = Convert.ToDateTime(Request["toDate"]);
                fromDate = Convert.ToDateTime(Session["fromDate"]);
                toDate = Convert.ToDateTime(Session["toDate"]);
                Session["selectedCustomerid"] = Convert.ToInt16(Request["selectedCustomerid"]);
                selectedCustomerid = Convert.ToInt16(Session["selectedCustomerid"]);
            }
            else
            {
                fromDate = Convert.ToDateTime(Session["fromDate"]);
                toDate = Convert.ToDateTime(Session["toDate"]);
                selectedCustomerid = Convert.ToInt16(Session["selectedCustomerid"]);
            }
            OrderRepository objOrderRep = new OrderRepository();
            int LoggedinAgentId = objOrderRep.GetAgentID();
            var lstOrders = objOrderRep.GetCurrentCustOrders(LoggedinAgentId, selectedCustomerid, fromDate, toDate);
            ViewBag.lstOrders = lstOrders.ToList().ToPagedList(pageNumber, pageSize);
            ViewBag.Pagecount = lstOrders.Count;
            if (!lstOrders.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            return PartialView("_CurrentCustOrders");
        }
        [HandleModelStateException]
        public PartialViewResult GetCustomersbyAlphabet(int? page)
        {
            int pageSize = 4;
            int pageNumber = (page ?? 1);
            // get the posted alphabet from the ajax
            //modified by manisha
            string alphabet = string.Empty;
            if (Session["selectedAlphabet"] == null)
            {
                Session["selectedAlphabet"] = Request["selectedAlphabet"];
                alphabet = Session["selectedAlphabet"].ToString();
            }
            else
            {
                alphabet = Session["selectedAlphabet"].ToString();
            }
            OrderRepository objOrderRep = new OrderRepository();
            int LoggedinAgentId = objOrderRep.GetAgentID();
            var custs = db.Customers.Where(p => p.name.Substring(0, 1) == alphabet && p.agent_id == LoggedinAgentId && p.status == "Approved");
            ViewBag.CustomerList = custs.ToList().ToPagedList(pageNumber, pageSize);
            ViewBag.Pagecount = custs.ToList().Count();
            return PartialView("_SearchCustomersAlphabetically", custs);
        }

        public void clearAlphabetcustomerSession()
        {
            Session["selectedAlphabet"] = null;
        }

        [HandleModelStateException]
        public ActionResult GetOrderDetailsMaster(int id)
        {
            QuickViewModel orderDetail = new QuickViewModel();
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orderDetails = (from r in db.Orders
                                where r.order_id == id
                                //&& r.is_deckled == true
                                select r).ToList();
            orderDetail.OrderDetails = orderDetails;
            return PartialView("_OrderDetailsMaster", orderDetail);
        }
        [HandleModelStateException]
        public ActionResult GetOrderDetailsChild(int id)
        {
            QuickViewModel orderDetail = new QuickViewModel();
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var orderProductsDetails = (from r in db.Order_products
                                        where r.order_id == id
                                        select r);

            //get total amount of order by sum of all orderproduct of paricular id
            var totaLAmount = (from r in db.Order_products
                               where r.order_id == id
                               select r.amount).Sum();

            List<TempPendingApproval> lst = (from or in db.Order_products
                                             join orderid in db.Orders on or.order_id equals orderid.order_id
                                             join pc in db.ProductionChild on or.order_product_id equals pc.order_product_id
                                             join pj in db.ProductionJumbo on pc.pj_id equals pj.pj_id
                                             where (orderid.order_id == id)
                                             group new { pc, pj } by new { pc.order_product_id, pj.estimated_start } into g
                                             select new TempPendingApproval { ActualQty = g.Sum(k => k.pc.qty), estimated_start = g.FirstOrDefault().pj.estimated_start, order_id = (int)g.FirstOrDefault().pc.order_product_id }).ToList();
            if (lst.Count > 0) { ViewBag.EstQtySch = lst; } else { ViewBag.EstQtySch = null; }

            List<TempPendingApproval> qtyschforlod = (from ord in db.Orders
                                                      join ordp in db.Order_products on ord.order_id equals ordp.order_id
                                                      join tdd in db.Truck_dispatch_details on ordp.order_product_id equals tdd.order_product_id
                                                      where (tdd.order_id == id)
                                                      group new { tdd } by new { tdd.order_product_id } into o
                                                      select new TempPendingApproval { plannedQty = o.Sum(l => l.tdd.qty_loaded), ActualQty = o.Sum(j => j.tdd.qty), order_id = (int)o.FirstOrDefault().tdd.order_product_id }).ToList();
            if (qtyschforlod.Count > 0) { ViewBag.qtyforsch = qtyschforlod; } else { qtyschforlod = null; }


            ViewBag.totaLAmount = totaLAmount;
            orderDetail.Order_Products = orderProductsDetails.ToList();
            return PartialView("_OrderDetailsChild", orderDetail);
        }


        


    }

}
