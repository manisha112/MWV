using MWV.DBContext;
using IdentitySample;
using MWV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MWV.Repository.Implementation;
using Microsoft.AspNet.Identity;
using System.Dynamic;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Collections;
using MWV.ViewModels;
using PagedList;
using PagedList.Mvc;
using MWV.Business;
using System.Reflection;
using System.IO;


namespace MWV.Controllers
{

    public class ProductionPlannerController : Controller
    {
        private MWVDBContext db = new MWVDBContext();
        ProductionPlannerRepository ProObj = new ProductionPlannerRepository();
        private MessageAndAlertsBusiness msgAlertObj = new MessageAndAlertsBusiness();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        string msgtxt = string.Empty;
        string msgSubject = string.Empty;
        string msgAction = string.Empty;
        string recipient1 = string.Empty;
        string cc1 = string.Empty;
        string bcc1 = string.Empty;
        string attachment1 = string.Empty;
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

        //loading production planner dashboard 
        [HandleModelStateException]
        public ActionResult Index()
        {
            OrderRepository ordRep = new OrderRepository();
            string id = User.Identity.GetUserId();
            ViewBag.lstPapermills = new SelectList(ProObj.PaperMillList(), "papermill_id", "name");
            ViewBag.lstAgent = new SelectList(ProObj.GetAgentList(), "agent_id", "name");
            ViewBag.lstCustomer = new SelectList(ProObj.GetCustomerList(), "customer_id", "name");
            ViewBag.lstofSrNo = new SelectList(ProObj.GetSrNo(), "pr_id", "pr_id");
            return View("DashBoard");
        }

        //New orders region
        #region

        /* get orders which is in created status ,so that  planner can 
          see the created status orders and the he/she can approved orders. */
        [HttpGet]
        [HandleModelStateException]
        public PartialViewResult GetNewOrderList()
        {
            try
            {
                if (!ProObj.NewCreatedOrderList().Any())
                {
                    ViewBag.NoRecordMsg = "No data available !";
                }
                ViewBag.lstShadeCode = new SelectList(ProObj.GetShades(), "shade_code");
                //  ViewBag.lstPapermills = new SelectList(ProObj.PaperMillList(), "papermill_id", "name");
                ViewBag.OrderList = ProObj.NewCreatedOrderList();
                return PartialView("_NewOrderList");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Production Planner->GetNewOrderList", Ex);
                return null;
            }

        }

        // save bulk of orders action to papermill
        [HttpPost]
        [HandleModelStateException]
        public ActionResult SubmitOrderToMill(string allVals)
        {
            try
            {
                OrderRepository orObj = new OrderRepository();
                ProductionPlannerRepository ObjPr = new ProductionPlannerRepository();
                string id = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var LoggedInUserName = db.AspNetUsers.Where(p => p.Id == id).Select(x => x.name).SingleOrDefault();

                /* in allvalus we take the millid and orders id's, and save this orders to papermill
                in allvalus first indexof we get the papermillid and and remaning are orders id's */
                var CheckedValue = allVals.Split(',');

                string[] strCheckedValue = Regex.Split(allVals, ",").ToArray();
                var SchID = Convert.ToInt16(allVals.Split(',')[0]);
                string[] strCheckedValuepp = Regex.Split(allVals, ",").Skip(1).ToArray();


                foreach (var ordPro in strCheckedValuepp)
                {
                    int ordProductId = Convert.ToInt16(ordPro);
                    var ord_prodStatus = db.Order_products.Where(c => c.order_product_id == ordProductId).SingleOrDefault();

                    ord_prodStatus.status = "Under Planning"; // orderproducts updating in underplanning
                    ord_prodStatus.schedule_id = SchID;
                    db.SaveChanges();

                    orObj.MatchedOrderStatusWithOrdPro(0, ordProductId);
                }


                //int ord = Convert.ToInt16(ordId);
                //var AssOrder = db.Orders.Where(c => c.order_id == ord).FirstOrDefault();
                //string orderdAgentName = ObjPr.GetOrderAgentName(AssOrder.agent_id);


                /* when order is send to pappermill by planner then status of this order and
                   product of this order(orderProduct) both will be update as underPlanning status*/

                //AssOrder.status = "Under Planning"; // order updating in underplanning
                //AssOrder.approved_by = LoggedInUserName;
                //AssOrder.papermill_id = millid;
                //AssOrder.modified_by = LoggedInUserName;
                //AssOrder.modified_on = DateTime.Now;

                //db.SaveChanges();
                //int millid=3;
                //var lstDetails = ObjPr.GetPapermillDetails(millid, ord);
                //MailoperationPapermill(orderdAgentName.ToString(), ord, lstDetails[2].ToString(), lstDetails[0].ToString(), lstDetails[1].ToString(), AssOrder.agent_id);
                return RedirectToAction("_NewOrderList");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Production Planner->SubmitOrderToMill", Ex);
                return null;
            }


        }




        //Send Mail To Agent For order has been Approved From Production Planner
        private bool MailoperationPapermill(string agentName, int orderNo, string custName, string machincename, string locationname, int? agid)
        {
            try
            {
                List<Order> lst = new List<Order>();
                string id = User.Identity.GetUserId();
                CustomerRepository objCR = new CustomerRepository();
                EmailController emailFunc = new EmailController();
                MessageAndAlertsBusiness objmail = new MessageAndAlertsBusiness();
                ViewBag.agntName = agentName;
                ViewBag.orderNo = orderNo;
                ViewBag.customername = custName;
                ViewBag.machincename = machincename;
                ViewBag.locationname = locationname;
                string Msgsub = "Order #" + orderNo + " has been Approved for " + machincename;
                string pdfOutput = emailFunc.GeneratePdfOutput(this.ControllerContext, lst, "OrderApproved");
                UserMailer objusm = new UserMailer();
                string emailtext = objCR.GetAgentmail(agid.ToString());
                bool statusodmail = objusm.sendMails(emailtext, "", Msgsub, "", "", "", "", pdfOutput, "", "");
                if (statusodmail == false)
                {
                    objmail.CreateMessagesDetails("OrderApproved", "Agent", agid, pdfOutput, Msgsub, null, null, null, agentName, attachment1, "Failed", null, null);
                }
                else
                {
                    objmail.CreateMessagesDetails("OrderApproved", "Agent", agid, pdfOutput, Msgsub, null, null, null, agentName, attachment1, "Deliverd", null, null);
                }
                objmail.CreateAlertDetails("OrderApproved", "Agent", agid, "Order no " + orderNo + " Placed", Msgsub, agentName, null);
                return true;

            }
            catch (Exception Ex)
            {
                logger.Error("Error in Production Planner->MailoperationPapermill:", Ex);
                return false;

            }

        }


        // get header details of new created order
        [HttpGet]
        [HandleModelStateException]
        public ActionResult GetNewOrdersSeeDetailsMst(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var orderDetils = (from r in db.Orders
                               where r.order_id == id
                               select r).SingleOrDefault();
            var AgentName = (from r in db.Agents
                             where r.agent_id == orderDetils.agent_id
                             select r.name);
            ViewBag.AgentName = AgentName.FirstOrDefault();
            ViewBag.lstPapermills = new SelectList(ProObj.PaperMillList(), "papermill_id", "name");
            return PartialView("_NewOrdersSeeDetailsMst", orderDetils);
        }

        //get subdetails of new created orders
        [HttpGet]
        [HandleModelStateException]
        public ActionResult GetNewOrdersSeeDetailsChild(int id)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }

            var orderDetails = (from r in db.Orders
                                where r.order_id == id
                                select r).SingleOrDefault();

            var AgentName = (from r in db.Agents
                             where r.agent_id == orderDetails.agent_id
                             select r.name);


            var CustomerName = (from r in db.Customers
                                where r.customer_id == orderDetails.customer_id
                                select r.name);

            //getting orders product list by order id
            List<TempJumboDetails> orderProductList = (from r in db.Order_products
                                                       join re in db.Products on r.product_code equals re.product_code
                                                       join or in db.Orders on r.order_id equals or.order_id
                                                       where r.order_id == id
                                                       select new TempJumboDetails
                                                       {
                                                           BF = re.bf_code,
                                                           GSM = re.gsm_code,
                                                           shade = r.shade_code,
                                                           qty = r.qty,
                                                           width = r.width,
                                                           RequestedDate = r.requested_delivery_date,
                                                           custpo = or.customerpo
                                                       }).ToList<TempJumboDetails>();

            ViewBag.ordDate = orderDetails.order_date;
            ViewBag.AgentName = AgentName.FirstOrDefault();

            ViewBag.CustomerName = CustomerName.FirstOrDefault();
            ViewBag.ProductList = orderProductList;
            return PartialView("_NewOrdersSeeDetailsChild");
        }

        //single order send to papermill
        //[HttpPost]
        //public ActionResult SingleOrderSubmitToMill(string allVals)
        //{
        //    ProductionPlannerRepository ObjPr = new ProductionPlannerRepository();

        //    /* in allvalus we take the millid and orders id's, and save this orders to papermill
        //    in allvalus first indexof we get the papermillid and and remaning are orders id's */
        //    var Millid = Convert.ToInt16(allVals.Split(',')[0]);
        //    var ordId = Convert.ToInt16(allVals.Split(',')[1]);

        //    string id = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //    var LoggedInUserName = db.AspNetUsers.Where(p => p.Id == id).Select(x => x.name).SingleOrDefault();
        //    var AssOrder = db.Orders.Where(c => c.order_id == ordId).FirstOrDefault();
        //    var ord_prodStatus = db.Order_products.Where(c => c.order_id == ordId).ToList();
        //    foreach (var ordproId in ord_prodStatus)
        //    {
        //        ordproId.status = "Under Planning";// orderproducts updating in underplanning
        //    }
        //    AssOrder.status = "Under Planning";// order updating in underplanning
        //    AssOrder.approved_by = LoggedInUserName;
        //    AssOrder.papermill_id = Millid;
        //    db.SaveChanges();

        //    //send alert msg to agent when order is send to pappermill (underplanning)
        //    string orderdAgentName = ObjPr.GetOrderAgentName(AssOrder.agent_id);
        //    var lstDetails = ObjPr.GetPapermillDetails(Millid, ordId);
        //    MailoperationPapermill(orderdAgentName.ToString(), ordId, lstDetails[2].ToString(), lstDetails[0].ToString(), lstDetails[1].ToString(), AssOrder.agent_id);
        //    return RedirectToAction("_NewOrdersSeeDetailsChild");
        //}

        //Get Papermill On Shade Change
        [HandleModelStateException]
        public ActionResult GetPapermillNameonShade()
        {
            try
            {
                string papermillName = Request["papermillname"];
                ProductionPlannerRepository objpr = new ProductionPlannerRepository();
                //var lstPapermillname=objpr.GetPapermillNamesonShade(papermillName);
                var query = (from sh in db.Schedule
                             join pm in db.Papermills on sh.papermill_id equals pm.papermill_id
                             where sh.shade_code == papermillName
                             select new { schedule_id = sh.schedule_id, name = pm.name, papermillid = pm.papermill_id }).ToList();

                return Json(query, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Production Planner->GetPapermillNameonShade", Ex);
                return null;
            }

        }

        //get Bf Gsm,Shade,qty,
        [HandleModelStateException]
        public PartialViewResult GetbfgsmShade()
        {
            try
            {

                string schname = Request["Shadetext"];
                ProductionPlannerRepository objPr = new ProductionPlannerRepository();
                var items = objPr.GetBfgsmShade(schname);
                // var lst =(from sh in db.Schedule where sh.shade_code == "NATURAL" && sh.schedule_id == null select new);
                ViewBag.Curitems = items.ToList();


                return PartialView("_NewShadeApproveOrders");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Production Planner->GetbfgsmShade", Ex);
                return null;
            }
        }
        #endregion

        //Mismatch region
        #region
        //Get deckle mismatches with pagging
        [HttpGet]
        [HandleModelStateException]
        public PartialViewResult Mismatch(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var DeckleDetails = ProObj.GetDeckleDetails();
            if (!DeckleDetails.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            ViewBag.DeckleDetails = DeckleDetails.ToPagedList(pageNumber, pageSize);
            return PartialView("_Mismatch");
        }

        //Deckle mismatch sendToLowerBf and sendToProduction
        [HandleModelStateException]
        public ActionResult SubmitDeckleToLowerBfORproduction(int Id)
        {
            string SelectedAction = "";
            string Remark = Request["ActionRemark"];
            if (Request["sendToLowerBf"] == "sendToLowerBf")
            {
                SelectedAction = "Send To Lower Bf";
            }
            if (Request["sendToProduction"] == "sendToProduction")
            {
                SelectedAction = "Send To Production";
            }
            ProObj.GetDeckleDetails(Id, SelectedAction, Remark);
            return View();
        }

        [HandleModelStateException]
        public PartialViewResult SeeMismatchDetails(int id)
        {
            ViewBag.QuickiewSeeDetails = ProObj.SeeMismatchDetails(id);
            return PartialView("_SeeMismatchDetails");
        }
        #endregion

        //PendingApproval region
        #region
        //Getting customer list that approval is pending
        [HandleModelStateException]
        public PartialViewResult GetPendingApproval(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var PendingApprovals = ProObj.GetAllPendingApproval(); //get pending approval customer list
            if (!PendingApprovals.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            ViewBag.CustPendingApproval = PendingApprovals.ToPagedList(pageNumber, pageSize);
            ViewBag.Pagecount = PendingApprovals.Count;
            return PartialView("_PendingApprovals");
        }

        // See details for pending approval
        [HandleModelStateException]
        public PartialViewResult SeeApprovalsDetails(int id)
        {
            var ApprovalPendingCust = ProObj.GetAllPendingApproval().Where(x => x.customer_id == id).ToList();
            ViewBag.ApprovalPendingCust = ApprovalPendingCust;
            return PartialView("_seeApprovalsDetails");
        }

        //submit action either Customer apprroved or rejected
        [HandleModelStateException]
        public ContentResult SubmitCustomerAction()
        {
            UserMailer objusm = new UserMailer();
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            string plannerName = (from a in db.AspNetUsers where a.Id == currentUserId select a.name).SingleOrDefault();

            string customerAction = Request["customerAction"];
            int custid = Convert.ToInt16(Request["custid"]);
            string remark = Request["remark"];
            int duplicateCustId = 0;
            if (Request["DuplicateCustId"] != "")
            {
                duplicateCustId = Convert.ToInt16(Request["DuplicateCustId"]);
            }
            if (customerAction == "approve")
            {
                customerAction = "Approved";
            }
            else
            {
                customerAction = "Rejected";
            }

            var Query = (from d in db.Customers
                         join ag in db.Agents on d.agent_id equals ag.agent_id
                         where d.customer_id == custid
                         select d).FirstOrDefault();
            Query.status = customerAction;
            Query.aspnetusers_id_approvedby = currentUserId;
            Query.approved_on = DateTime.Now;
            Query.remarks = remark;

            // when city and customer name is same then we get the data as duplicate customer otherwise duplicate cutomer details will be null
            if (duplicateCustId != 0)
            {
                var duplicateCust = (from d in db.Customers
                                     where d.customer_id == duplicateCustId
                                     select d).FirstOrDefault();
                if (customerAction == "Approved")
                {
                    duplicateCust.status = "Rejected";
                }
                else
                {
                    duplicateCust.status = "Approved";
                }

                db.SaveChanges();
                AlertforCustomerApprovedAndDeny(duplicateCust.status, duplicateCust.name, duplicateCust.city, plannerName, duplicateCust.agent_id);
                Mailoperation(Query.Agent.name, Query.name, Query.city, Query.Agent.agent_id, Query.created_by, Query.remarks, customerAction);
            }

            db.SaveChanges();
            AlertforCustomerApprovedAndDeny(Query.status, Query.name, Query.city, plannerName, Query.agent_id);
            Mailoperation(Query.Agent.name, Query.name, Query.city, Query.Agent.agent_id, Query.created_by, Query.remarks, customerAction);

            return Content("Successfully submitted the action.");
        }

        //Create Alert For Customer
        private bool AlertforCustomerApprovedAndDeny(string customerStatus, string customerName, string customerCity, string CreatedBy, int agentId)
        {
            try
            {
                if (customerStatus == "Approved")// if customer is approved by planner then send msg to agent which is created customer
                {
                    string alertText = "Your Customer " + customerName + ", " + customerCity + " has been approved";
                    string alertSubject = "Customer Approved " + customerName + ", " + customerCity;
                    msgAlertObj.CreateAlertDetails("CustomerApproved", "Agent", agentId, alertText, alertSubject, CreatedBy, null);
                }
                else
                    if (customerStatus == "Rejected")// if customer is Rejected by planner then send msg to agent which is created customer
                    {
                        string alertText = "Your Customer " + customerName + ", " + customerCity + " has been Rejected";
                        string alertSubject = "Customer Rejected  " + customerName + ", " + customerCity;

                        msgAlertObj.CreateAlertDetails("CustomerRejected", "Agent", agentId, alertText, alertSubject, CreatedBy, null);
                    }

                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlanner->AlertforCustomerApprovedAndDeny:", Ex);
                return false;
            }
        }
        //Send Mail And Message To Customer With Status
        private bool Mailoperation(string agentName, string custName, string city, int agentid, string CreatedBy, string remark, string status)
        {
            try
            {
                string id = User.Identity.GetUserId();
                CustomerRepository objCR = new CustomerRepository();
                List<Customer> lstobj = new List<Customer>();
                string subject = "Customer Rejected " + custName + "," + city;
                string ApSub = "Customer Approved " + custName + "," + city;
                msgAction = "CustomerRejected";
                EmailController emailFunc = new EmailController();
                ViewBag.CustName = custName;
                ViewBag.City = city;
                ViewBag.agntname = agentName;
                ViewBag.remark = remark;
                string pdfOutput;
                UserMailer objusm = new UserMailer();
                if (status == "Rejected")
                {
                    msgAction = "CustomerRejected";
                    pdfOutput = emailFunc.GeneratePdfOutput(this.ControllerContext, lstobj, "CustomerRejected");
                    string emailto = objCR.GetAgentmail(agentid.ToString());
                    string emailcc = objCR.GetEmail();

                    bool statusofmailUn = objusm.sendMails(emailto, "", subject, emailcc, "", "", "", pdfOutput, "", "");
                    if (statusofmailUn == false)
                    {
                        msgAlertObj.CreateMessagesDetails(msgAction, "Agent", agentid, pdfOutput, subject, recipient1, cc1, bcc1, CreatedBy, attachment1, "Failed", null, null);
                    }
                    else
                    {
                        msgAlertObj.CreateMessagesDetails(msgAction, "Agent", agentid, pdfOutput, subject, recipient1, cc1, bcc1, CreatedBy, attachment1, "Deliverd", null, null);

                    }

                }
                else
                {
                    msgAction = "CustomerApproved";
                    pdfOutput = emailFunc.GeneratePdfOutput(this.ControllerContext, lstobj, "CustomerApproved");
                    string emailto = objCR.GetAgentmail(agentid.ToString());
                    string emailcc = objCR.GetEmail();
                    bool statusofmailApp = objusm.sendMails(emailto, "", ApSub, emailcc, "", "", "", pdfOutput, "", "");
                    if (statusofmailApp == false)
                    {
                        msgAlertObj.CreateMessagesDetails(msgAction, "Agent", agentid, pdfOutput, ApSub, recipient1, cc1, bcc1, CreatedBy, attachment1, "Failed", null, null);
                    }
                    else
                    {
                        msgAlertObj.CreateMessagesDetails(msgAction, "Agent", agentid, pdfOutput, ApSub, recipient1, cc1, bcc1, CreatedBy, attachment1, "Deliverd", null, null);

                    }
                }
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionplannerController->Mailoperation:", Ex);
                return false;
            }

        }

        #endregion

        //Production Plan region
        #region
        //Production plan with jumboes details
        [HandleModelStateException]
        public PartialViewResult ProductionPlanHeaderDetails(int id)
        {
            var ProPlanDetails = ProObj.ProductionPlanHeaderDetails(id);
            var JumbosDetails = ProObj.ProductionPlanJmbosDetails(id);
            var LotsDetails = ProObj.ProductionPlanLotsDetails(id);
            int num = LotsDetails.Count();
            if (!ProPlanDetails.Any() || !JumbosDetails.Any() || !LotsDetails.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            else
            {
                ViewBag.estStartTime = ProPlanDetails.Select(x => x.estimated_start).First();
                ViewBag.estEndTime = ProPlanDetails.Select(x => x.EstimatedEndDate).First();
                ViewBag.EstimatedRunTime = ProPlanDetails.Select(x => x.EstimatedRunTime).First();
                //ViewBag.estimatedPlannedQty = ProPlanDetails.Select(x => x.plannedQty).First();
                //var estQty = ProPlanDetails.Select(x => x.plannedQty).First();
                decimal? estQty = ProPlanDetails.Select(x => x.plannedQty).First();
                double dbl1 = (double)estQty;
                ViewBag.estimatedPlannedQty = Math.Round(dbl1, 2);
                ViewBag.papermillName = ProPlanDetails.Select(x => x.papermillName).First();
                ViewBag.SrNo = ProPlanDetails.Select(x => x.srNo).First();
                ViewBag.jumboesNo = JumbosDetails.Count();
                ViewBag.JumbosDetails = JumbosDetails;
                ViewBag.LotsDetails = LotsDetails;
            }
            return PartialView("_ProductionPlanDetails");
        }

        //production plan accoring to searchByType
        [HandleModelStateException]
        public PartialViewResult GetSearchProductionPlan(int? page)
        {
            int MillId = 0;
            DateTime fromdt = new DateTime();
            DateTime todt = new DateTime();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            if (Request["SelectedSrno"] != "" || Request["SrnoBymillid"] != "")
            {
                if (Request["FromDateTime"] != "DD-MM-YYYY" && Request["ToDateTime"] != "DD-MM-YYYY")
                {
                    fromdt = Convert.ToDateTime(Request["FromDateTime"]);
                    DateTime todate = Convert.ToDateTime(Request["ToDateTime"]);
                    todt = todate.AddHours(23).AddMinutes(59).AddSeconds(59);
                }
            }
            else
            {
                fromdt = Convert.ToDateTime(Request["FromDateTime"]);
                todt = Convert.ToDateTime(Request["ToDateTime"]);
            }
            int SelectedAgentId = 0;
            int SelectedSrnoId = 0;
            int SelectedCustId = 0;
            int SrnoBymillid = 0;

            if (Request["SelectedMillId"] == "")
            {
                MillId = 0;
            }
            else
            {
                MillId = Convert.ToInt32(Request["SelectedMillId"]);
            }

            if (Request["SelectedAgent"] != "")
            {
                SelectedAgentId = Convert.ToInt16(Request["SelectedAgent"]);
            }
            else if (Request["SelectedSrno"] != "")
            {
                SelectedSrnoId = Convert.ToInt16(Request["SelectedSrno"]);

            }
            else if (Request["SelectedCustomer"] != "")
            {
                SelectedCustId = Convert.ToInt16(Request["SelectedCustomer"]);

            }
            else if (Request["SrnoBymillid"] != "")
            {
                SrnoBymillid = Convert.ToInt16(Request["SrnoBymillid"]);
            }

            if (Session["fromDate"] == null && Session["toDate"] == null)
            {
                Session["fromDate"] = fromdt;
                Session["toDate"] = todt;
            }
            else
            {
                fromdt = Convert.ToDateTime(Session["fromDate"]);
                todt = Convert.ToDateTime(Session["toDate"]);
            }

            if (Session["SelectedAgent"] == null)
            {
                Session["SelectedAgent"] = SelectedAgentId;
            }
            else
            {
                SelectedAgentId = Convert.ToInt32(Session["SelectedAgent"]);
            }

            if (Session["SelectedCustomer"] == null)
            {
                Session["SelectedCustomer"] = SelectedCustId;
            }
            else
            {
                SelectedCustId = Convert.ToInt32(Session["SelectedCustomer"]);
            }
            if (Session["SrnoBymillid"] == null)
            {
                Session["SrnoBymillid"] = SrnoBymillid;
            }
            else
            {
                SrnoBymillid = Convert.ToInt16(Session["SrnoBymillid"]);
            }

            if (Session["SelectedMillId"] == null)
            {
                Session["SelectedMillId"] = MillId;
            }
            else
            {
                MillId = Convert.ToInt32(Session["SelectedMillId"]);
            }

            if (Session["SelectedSrno"] == null)
            {
                Session["SelectedSrno"] = SelectedSrnoId;
            }
            else
            {
                SelectedSrnoId = Convert.ToInt32(Session["SelectedSrno"]);
            }

            var ProductionPlanByEntity = ProObj.GetSearchProductionPlanByEntity(MillId, fromdt, todt, SelectedAgentId, SelectedCustId, SelectedSrnoId, SrnoBymillid).OrderByDescending(p => p.srNo);
            //  ProductionPlanByEntity = ProductionPlanByEntity.OrderByDescending( p => p.srNo);
            if (!ProductionPlanByEntity.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            //var ProductionPlan = ProductionPlanByEntity.GroupBy(x => x.srNo).ToList();
            ViewBag.GetSearchProductionPlan = ProductionPlanByEntity.ToPagedList(pageNumber, pageSize);
            ViewBag.PagesizeSearchplan = ProductionPlanByEntity.Count();
            return PartialView("_SearchProductionPlan");
        }

        //Recent See All production plan for quickview
        [HandleModelStateException]
        public PartialViewResult SeeAllRecentProductionPlans(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var orderDetail = (from d in db.ProductionRun
                               join dc in db.Papermills on d.papermill_id equals dc.papermill_id
                               where (d.run_time > 0)
                               select new TempPendingApproval
                              {
                                  estimated_start = d.estimated_start,
                                  papermillName = dc.name,
                                  srNo = d.pr_id

                              }).OrderByDescending(x => x.srNo).ToList<TempPendingApproval>();
            // return orderDetail;
            //}

            ViewBag.GetSearchProductionPlan = orderDetail.ToPagedList(pageNumber, pageSize);
            ViewBag.PagesizeAllRecent = orderDetail.Count;
            return PartialView("_SearchProductionPlan");
        }

        //Get Production plan from date to todate
        [HandleModelStateException]
        public PartialViewResult GetAllProPlanFromtoDate(int? page)
        {
            DateTime fromdt = new DateTime();
            DateTime todt = new DateTime();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            DateTime fromDate = Convert.ToDateTime(Request["FromDateTime"]);
            DateTime todate = Convert.ToDateTime(Request["ToDateTime"]);

            //using time as "23:59:59" instead of 00:00:00
            DateTime toDate = todate.AddHours(23).AddMinutes(59).AddSeconds(59);

            if (Session["fromDate"] == null && Session["toDate"] == null)
            {
                Session["fromDate"] = fromDate;
                Session["toDate"] = toDate;
                fromdt = Convert.ToDateTime(Session["fromDate"]);
                todt = Convert.ToDateTime(Session["toDate"]);
            }
            else
            {
                fromdt = Convert.ToDateTime(Session["fromDate"]);
                todt = Convert.ToDateTime(Session["toDate"]);
            }
            var ProPlanDetail = (from d in db.ProductionRun
                                 join dc in db.Papermills on d.papermill_id equals dc.papermill_id
                                 where (d.estimated_start >= fromdt && d.estimated_start <= todt)
                                 && (d.run_time > 0)
                                 select new TempPendingApproval
                                 {
                                     estimated_start = d.estimated_start,
                                     papermillName = dc.name,
                                     srNo = d.pr_id,
                                     runTime = d.run_time


                                 }).OrderByDescending(x => x.srNo).ToList<TempPendingApproval>();

            if (!ProPlanDetail.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            ViewBag.GetSearchProductionPlan = ProPlanDetail.ToPagedList(pageNumber, pageSize);
            ViewBag.PagesizeFromDate = ProPlanDetail.Count;
            return PartialView("_SearchProductionPlan");
        }

        //Get Production palnner by pappermill
        [HandleModelStateException]
        public PartialViewResult GetProPlanByMill(int? page)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            DateTime fromdt = new DateTime();
            DateTime todt = new DateTime();
            DateTime todate = new DateTime();
            int millid = 0;

            if (Session["fromDate"] == null && Session["toDate"] == null && Session["SelectedMillId"] == null)
            {
                fromdt = Convert.ToDateTime(Request["FromDateTime"]);
                todate = Convert.ToDateTime(Request["ToDateTime"]);
                todt = todate.AddHours(23).AddMinutes(59).AddSeconds(59);
                millid = Convert.ToInt16(Request["SelectedMillId"]);

                Session["fromDate"] = fromdt;
                Session["toDate"] = todt;
                Session["SelectedMillId"] = millid;

            }
            else
            {
                fromdt = Convert.ToDateTime(Session["fromDate"]);
                todt = Convert.ToDateTime(Session["toDate"]);
                millid = Convert.ToInt16(Session["SelectedMillId"]);
            }

            var ProPlanDetail = (from d in db.ProductionRun
                                 join dc in db.Papermills on d.papermill_id equals dc.papermill_id
                                 where (d.estimated_start >= fromdt && d.estimated_start <= todt)
                                 && (d.papermill_id == millid)
                                 && (d.run_time > 0)
                                 select new TempPendingApproval
                                 {
                                     estimated_start = d.estimated_start,
                                     papermillName = dc.name,
                                     srNo = d.pr_id

                                 }).OrderByDescending(x => x.srNo).ToList<TempPendingApproval>();

            if (!ProPlanDetail.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            ViewBag.GetSearchProductionPlan = ProPlanDetail.ToPagedList(pageNumber, pageSize);
            ViewBag.PagesizeMill = ProPlanDetail.Count();
            return PartialView("_SearchProductionPlan");
        }

        //Get production pla serial no. by papermillid
        [AcceptVerbs(HttpVerbs.Get)]
        [HandleModelStateException]
        public JsonResult GetSrNoByMillId()
        {
            int millid = Convert.ToInt16(Request["searchBy"]);


            var query = (from r in db.ProductionRun
                         join re in db.Papermills on r.papermill_id equals re.papermill_id
                         where re.papermill_id == millid
                         && (r.run_time > 0)
                         select new
                         {
                             srno = r.pr_id
                         }).OrderByDescending(x => x.srno);

            return Json(query, JsonRequestBehavior.AllowGet);
        }

        #endregion

        // 7 days quickview region
        #region
        //RecentMismatch for 7 days 
        [HandleModelStateException]
        public PartialViewResult RecentMismatch()
        {
            var DeckleDetails = ProObj.DeckleQuickiewDetails();
            if (!DeckleDetails.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            ViewBag.QuickiewSeeDetails = DeckleDetails;
            return PartialView("_QuickViewMismatch");
        }

        //Recent quickview 7 days pending approval
        [HandleModelStateException]
        public PartialViewResult RecentPendingApproval()
        {
            var ProPlanDetails = ProObj.GetAllPendingApproval().
                                   Where(x => x.CustCreatedOn > DateTime.Now.AddDays(-7))
                                  .OrderByDescending(x => x.estimated_start).Take(2);
            if (!ProPlanDetails.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            ViewBag.SeePendingApproval = ProPlanDetails;
            return PartialView("_QuickViewPendingApproval");
        }

        //Recent  quickview 7 days production plan
        [HandleModelStateException]
        public PartialViewResult RecentProductionPlan()
        {
            var ProductionPlans = ProObj.GetProductionPlan();
            if (!ProductionPlans.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            ViewBag.ProductionPlanlist = ProductionPlans;
            return PartialView("_ProductionPlanlQuickViewDetails");
        }
        #endregion

        //Common methods
        #region
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void clearSession()
        {
            Session["fromDate"] = null;
            Session["toDate"] = null;
            Session["SelectedAgent"] = null;
            Session["SelectedCustomer"] = null;
            Session["searchPlansSrNo"] = null;
            Session["SelectedSrno"] = null;
            Session["SelectedMillId"] = null;
            Session["SrnoBymillid"] = null;
        }
        #endregion

        #region
        //For PP Pdf Downloads
        [HandleModelStateException]
        public ActionResult GetPdffileForPP(string orderid)
        {
            try
            {
                ProductionPlannerRepository objrep = new ProductionPlannerRepository();
                string agentname = objrep.GetPapermillName(Convert.ToInt16(orderid));
                agentname = agentname.Replace(" ", "_");
                string filename = agentname + "_Plan_" + orderid;
                string fullName = "";
                DirectoryInfo hdDirectoryInWhichToSearch = new DirectoryInfo(Server.MapPath("~/MWV/PDF/"));
                FileInfo[] filesInDir = hdDirectoryInWhichToSearch.GetFiles("*" + filename + "*.pdf*", SearchOption.AllDirectories);

                foreach (FileInfo foundFile in filesInDir)
                {
                    fullName = foundFile.FullName;

                }
                Response.AddHeader("Content-Disposition", "attachment; filename=" + filename + ".pdf");
                return File(fullName, Response.ContentType);


            }
            catch (Exception Ex)
            {

                logger.Error("Error in CreateCustomer->ProductionPdfs:", Ex);
            }
            return null;
        }
        #endregion

        
    }
}
