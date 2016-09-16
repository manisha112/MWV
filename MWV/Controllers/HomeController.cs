using System.Web.Mvc;
using MWV.Business;
using System;
using System.Text.RegularExpressions;
using MWV.Repository.Implementation;
using MWV.DBContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MWV.Models;
using IdentitySample.Models;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;
using MWV.ViewModels;
using MWV.Repository.Interfaces;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MWV.Controllers;
using System.IO;
using System.Reflection;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System.Drawing;
using OfficeOpenXml.Style;
using System.Web.Hosting;

namespace IdentitySample.Controllers
{
    //[Authorize(Roles="SuperAdmin, mwvAdmin, Agent")]
    public class HomeController : Controller
    {
        private MWVDBContext db = new MWVDBContext();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

        public ActionResult Index()
        {
            string id = System.Web.HttpContext.Current.User.Identity.GetUserId();

            //added by RK
            //if for any reason Id is empty the do not fire the LINQ

            Session["uname"] = "";
            if (id != null)
            {
                var LoggedInUserName = db.AspNetUsers.Where(p => p.Id == id).Select(x => x.firstname).SingleOrDefault();
                Session["uname"] = LoggedInUserName;
            }
            //string id1 = User.Identity.GetUserId();
            //if (id != null)
            //{
            //    var RoleNames = UserManager.GetRoles(id);
            //    return RedirectToAction("Index", RoleNames[0]);

            //}
            //else
            //{
            //    return RedirectToAction("Login", "Account");
            //}


            if (User.IsInRole("GateKeeper"))
            {
                return RedirectToAction("Index", "GateKeeper");
            }
            else
                if (User.IsInRole("Agent"))
                {
                    return RedirectToAction("Index", "Agent");
                }
                else
                    if (User.IsInRole("MachineHead"))
                    {
                        return RedirectToAction("Index", "MachineHead");
                    }
                    else
                        if (User.IsInRole("ProductionPlanner"))
                        {
                            return RedirectToAction("Index", "ProductionPlanner");
                        }
                        else
                            if (User.IsInRole("FinanceHead"))
                            {
                                return RedirectToAction("Index", "FinanceHead");
                            }
                            else
                                if (User.IsInRole("Dispatch"))
                                {
                                    return RedirectToAction("Index", "Dispatch");
                                }
                                else
                                    if (User.IsInRole("MWVAdmin"))
                                    {
                                        ViewBag.MWVAdmin = "MWVAdmin";
                                        return View("Index");
                                        //return RedirectToAction("Index", "Index");
                                    }
                                    else
                                        if (User.IsInRole("Customer"))
                                        {
                                            return RedirectToAction("Index", "Customer");
                                        }
                                        else
                                            if (User.IsInRole("SuperAdmin"))
                                            {
                                                return RedirectToAction("Index", "SuperAdmin");
                                            }
                                            else
                                                if (User.IsInRole("ExecutiveStakeHolder"))
                                                {
                                                    return RedirectToAction("Index", "Stakeholder");
                                                }
            return RedirectToAction("Login", "Account");

        }

        public Boolean deckleMethod()
        {
            DeckleRepository drObj = new DeckleRepository();
            drObj.GetActiveSchedulesforPapermill(1);
            return true;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetAllOrdersbyAgentandLocation(int id)
        {
            OrderRepository objOrder = new OrderRepository();
            string papermill_location = objOrder.GetPapermillLocation(id).Trim();
            var papermills = db.Papermills.Where(p => p.location == papermill_location).ToList();
            int pm_id1 = papermills[0].papermill_id;
            int pm_id2 = papermills[1].papermill_id;
            int LoggedAgentId = objOrder.GetAgentID();
            Truck_dispatchRepository tdObj = new Truck_dispatchRepository();

            var lstOrders = (from Order_Products op in db.Order_products
                             join ord in db.Orders on op.order_id equals ord.order_id
                             join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                             where ord.agent_id == LoggedAgentId
                              && (sch.papermill_id == pm_id1 || sch.papermill_id == pm_id2)
                             //&& ord.is_deckled == true
                             select new
                             {
                                 product_code = op.product_code,
                                 qty_planned_byAgent = op.qty_planned_byAgent,
                                 qty_scheduled = op.qty_scheduled,
                                 order_id = op.order_id,
                                 customerName = ord.Customer.name,
                                 order_product_id = op.order_product_id
                             }).ToList();

            List<TempJumboDetails> list = new List<TempJumboDetails>();
            foreach (var items in lstOrders)
            {
                TempJumboDetails ttdObj = new TempJumboDetails();
                var qtyScheduled = items.qty_scheduled; // scheduled qty of particular product
                var sumOfqty = from tdd in db.Truck_dispatch_details
                               where //tdd.order_id == items.order_id &&
                               tdd.order_product_id == items.order_product_id
                               group tdd by 1 into g
                               select new
                               {
                                   SumTotal = g.Sum(x => x.qty)
                               };
                var pendingQty = sumOfqty.SingleOrDefault();
                decimal? availableQty = 0;
                if (pendingQty == null)
                {
                    availableQty = qtyScheduled - 0;
                }
                else
                {
                    availableQty = qtyScheduled - pendingQty.SumTotal;// scheduled qty of particular product - sum of qty  
                }

                if (availableQty > 0)
                {
                    ttdObj.product_code = items.product_code;
                    ttdObj.order_product_id = items.order_product_id;
                    ttdObj.order_id = items.order_id;
                    ttdObj.CustomerName = items.customerName;
                    list.Add(ttdObj);

                }
            }

            Session["filterList"] = null;
            Session["tempDispatch"] = null;
            Session["tempCargo"] = null;
            var objs = (from c in list
                        orderby c.order_id
                        select c).GroupBy(g => g.order_id).Select(x => x.FirstOrDefault());

            var result = (from s in objs
                          select new
                          {
                              id = s.order_id,
                              name = s.CustomerName
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult searchCustomer(int agentid, string searchStr)
        {
            //searchStr = "cu";
            CustomerRepository objCust = new CustomerRepository();
            var result = objCust.CustSearchResult(agentid, searchStr);

            List<Cust> custNew = new List<Cust>();
            foreach (Customer cust in result)
            {
                Cust cst = new Cust();
                //cst.customer_id = cust.customer_id;
                //cst.name = cust.name;
                cst.address1 = cust.address1;
                cst.city = cust.city;
                custNew.Add(cst);
            }

            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(custNew);
            string[] digits = Regex.Split(jsonString, @"\[");
            digits = Regex.Split(digits[1].ToString(), @"\]");
            jsonString = "[" + " {\"customers\" : " + " [ " + digits[0] + " ] " + "}" + "]";
            return Content(jsonString, "application/json");
            //Response.Write(jsonString + " "); // this prints without the escape characters on the screen, 
            // whereas, in the jsonString when it prints on the browser it shows backslashes and it is not 
            // a valid json, when I check it on jsonlint.com

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult AdminHome(ManageMessageId? message)
        {
            if (message != null)
            {
                ViewBag.StatusMessage =
                    message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed!"
                                        : "";
            }

            return View("DashBoard");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Excluded agent_id as ModelState.IsValid was returning false always[Reason: agent_id is hidden field and it cannot be null].
        // Gets initialized inside and database is updated accordingy.
        public ActionResult Create([Bind(Exclude = "agent_id", Include = "customer_id,name,email,phone,address1,address2,address3,city,pincode,state,country,fax,status,aspnetusers_id_approvedby,approved_on,created_on,created_by,modified_on,modified_by")] Customer customer)
        {
            string id = User.Identity.GetUserId();
            int LoggedInAgent_id;

            // Get the agent_id from 'Agents' table with this user id.
            if (id != null)
            {
                LoggedInAgent_id = (from a in db.Agents where a.aspnetusers_id == id select a.agent_id).SingleOrDefault();

                ViewBag.Agent_id = LoggedInAgent_id;
                var Customers = db.Customers.Where(c => c.agent_id == LoggedInAgent_id).FirstOrDefault();

                customer.agent_id = Customers.agent_id;
            }
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                //return RedirectToAction("DashBoard");
                return View("Index");

            }

            return View("DashBoard");
        }

        [HttpGet]
        public JsonResult GetCustList()
        {
            var custList = db.Customers.Select(p => new { customer_id = p.customer_id, name = p.name });
            return Json(custList, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult GetProductsByOrderId(int? id)
        {

            List<Truck_dispatchRepository.tempTruckDispatchDetails> ordPrdList = Session["filterList"] as List<Truck_dispatchRepository.tempTruckDispatchDetails>;

            List<TempJumboDetails> list = new List<TempJumboDetails>();
            var prodList = (from Order_Products op in db.Order_products
                            join Order in db.Orders on op.order_id equals Order.order_id
                            where op.order_id == id
                            //&& op.order_product_id != ordPrdList
                            select new
                            {
                                product_code = op.product_code,
                                qty_planned_byAgent = op.qty_planned_byAgent,
                                qty_scheduled = op.qty_scheduled,
                                order_id = op.order_id,
                                order_product_id = op.order_product_id,
                                width = op.width.ToString(),
                                combination = op.Product.bf_code + " BF " + op.Product.Gsm.gsm_code + " GSM " + op.shade_code //+ op.product_code +","+ op.shade_code,
                                //combination = String.Format("{0:0.00}", op.width)  + " cm x " + op.Product.bf_code + "BF " + op.Product.Gsm.gsm_code + "GSM " + op.shade_code //+ op.product_code +","+ op.shade_code,

                            }).ToList();
            foreach (var items in prodList)
            {
                TempJumboDetails ttdObj = new TempJumboDetails();
                var qtyScheduled = items.qty_scheduled; // scheduled qty of particular product
                var sumOfqty = from tdd in db.Truck_dispatch_details
                               where tdd.order_product_id == items.order_product_id
                               group tdd by 1 into g
                               select new
                               {
                                   SumTotal = g.Sum(x => x.qty)
                               };
                var pendingQty = sumOfqty.SingleOrDefault();
                decimal? availableQty = 0;
                if (pendingQty == null)
                {
                    availableQty = qtyScheduled - 0;
                }
                else
                {
                    availableQty = qtyScheduled - pendingQty.SumTotal;// scheduled qty of particular product - sum of qty  
                }
                if (availableQty > 0)
                {
                    ttdObj.product_code = items.product_code;
                    ttdObj.order_product_id = items.order_product_id;
                    ttdObj.combination = items.combination;
                    ttdObj.widthforddl = items.width;
                    list.Add(ttdObj);

                }
            }
            //var count = 0;
            if (ordPrdList != null)
            {
                foreach (var itm in ordPrdList)
                {
                    var filterList = list.Where(p => p.order_product_id != itm.order_product_id).ToList();
                    list = filterList;
                }
            }
            //  after discussion replace the above query with below
            // (from Order_Products op in db.Order_products
            // join Order in db.Orders on op.order_id equals Order.order_id
            // where op.order_id == id && op.qty_planned_byAgent < op.qty
            // && (op.status == "Planned" || op.status == "In Warehouse")
            // || (op.status == "Under Planning" && (op.qty_scheduled - op.qty_planned_byAgent > 0))
            // select op).ToList();

            var result = (from s in list
                          //  where s.order_product_id 
                          select new
                          {
                              id = s.order_product_id,
                              width = s.widthforddl,
                              name = s.combination
                          }).ToList();

            string message = string.Format("Products of selected order are: ");

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        //[HttpGet]
        //public ActionResult DashBoard()

        //{
        //    return View();
        //}

        public void stock()
        {
            //This method is for testing only...Remove later
            StockRepository stockrepo = new StockRepository();
            DateTime dt = DateTime.Now.Date;
            int result1 = stockrepo.StockProduced(dt, 4, 1, 29, "Bf18-Gsm170", "NATURAL", 22);
            int result2 = stockrepo.StockDispatched(dt, 4, 1, 29, "24BF-135gsm", "NATURAL", 12);

        }
        public void SendStockReport()
        {
            //This method Runs the Stock report for each agent and sends them an email

            //Get a list of all Agents with their email Addresses
            StockRepository stockobj = new StockRepository();
            UserMailer mailobj = new UserMailer();
            DateTime currDate = DateTime.Now;
            List<Agent> AgentList = stockobj.GetAgentList();
            //Loop through all agents and get the CSV fields 
            if (AgentList != null)
            {
                foreach (Agent agent in AgentList)
                {
                    List<ExportStock> ExportList = stockobj.GetDailyStock(agent.agent_id, currDate);
                    var count = ExportList.Count();
                    if (count != 0)
                    {
                        using (ExcelPackage Excel = new ExcelPackage())
                        {

                            //Create a sheet
                            Excel.Workbook.Worksheets.Add("Stocks");
                            ExcelWorksheet ws = Excel.Workbook.Worksheets[1];
                            ws.Name = "Stocks Report"; //Setting Sheet's name
                            ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                            ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                            //Add Headers
                            AddHeadertoExcel(ws, 1, 1, "Customer");
                            AddHeadertoExcel(ws, 2, 1, "PaperMill");
                            AddHeadertoExcel(ws, 3, 1, "BF");
                            AddHeadertoExcel(ws, 4, 1, "GSM");
                            AddHeadertoExcel(ws, 5, 1, "Shade");
                            AddHeadertoExcel(ws, 6, 1, "OpeningStock");
                            AddHeadertoExcel(ws, 7, 1, "ClosingStock");

                            //Dump the entire Object to Excel row
                            int row = 2;
                            foreach (ExportStock stock in ExportList) //Creating Headings
                            {
                                AddStringColtoExcel(ws, 1, row, stock.Customer);
                                AddStringColtoExcel(ws, 2, row, stock.PaperMill);
                                AddStringColtoExcel(ws, 3, row, stock.BF);
                                AddStringColtoExcel(ws, 4, row, stock.GSM);
                                AddStringColtoExcel(ws, 5, row, stock.Shade);
                                AddintColtoExcel(ws, 6, row, stock.OpeningStock);
                                AddintColtoExcel(ws, 7, row, stock.ClosingStock);
                                row++;
                            }

                            //Create the File locally
                            Byte[] bin = Excel.GetAsByteArray();
                            string folder = currDate.ToString("MMM-yyyy");
                            bool exists = System.IO.Directory.Exists(Server.MapPath("~/MWV/CSV/" + folder));
                            if (!exists)
                            {
                                System.IO.Directory.CreateDirectory(Server.MapPath("~/MWV/CSV/" + folder));
                            }
                            string finalPath = "/MWV/CSV/" + folder;
                            string fullPath = Server.MapPath(finalPath + "/" + agent.name.Replace(" ", "-") + "_StockReport-" + currDate.Year.ToString() + currDate.Month.ToString() + currDate.Day.ToString() + ".xlsx");
                            string attachment1 = Server.MapPath(finalPath + "/" + agent.name.Replace(" ", "-") + "_StockReport-" + currDate.Year.ToString() + currDate.Month.ToString() + currDate.Day.ToString() + ".xlsx");
                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                            System.IO.File.WriteAllBytes(fullPath, bin);

                            //SendEmail Code Here
                            string mailSub = "Stock Report For -" + currDate;
                            string msgbody = "Dear " + agent.name + ",<br /> " + "Stock Report For Date " + currDate + "<br />" + "Attach Excel File Is the Details,<br/> " + "<br /> Reagards,<br/>" + "Support Team,<br/>" + "support@westrock.com.";

                            mailobj.sendMails(agent.email, "", mailSub, "", "", "", "", msgbody, fullPath, "");

                        }
                    }
                }
            }


        }

        private void AddHeadertoExcel(ExcelWorksheet ws, int col, int row, string Value)
        {
            var cell = ws.Cells[row, col];
            var fill = cell.Style.Fill;
            fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            fill.BackgroundColor.SetColor(Color.Gray);  //Setting the background color of header cells to Gray
            var border = cell.Style.Border;
            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
            cell.Value = Value; //Setting Value in cell

        }

        public void AddStringColtoExcel(ExcelWorksheet ws, int col, int row, string Value)
        {
            var cell = ws.Cells[row, col];
            var border = cell.Style.Border;
            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
            cell.Value = Value; //Setting Value in cell

        }
        private void AddintColtoExcel(ExcelWorksheet ws, int col, int row, decimal Value)
        {
            var cell = ws.Cells[row, col];
            var border = cell.Style.Border;
            border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style = ExcelBorderStyle.Thin;
            cell.Value = Value; //Setting Value in cell

        }
        public void Deckle()
        {
            DateTime startDate = DateTime.Now;
            DeckleBusiness objDeckle = new DeckleBusiness(new DeckleRepository());
            string PR_ids = objDeckle.CalculateDeckle();
            DateTime endDate = DateTime.Now;

            //----Raise Alert for Production Plan creation
            // convert to Array string to get all PR_ids
            string[] PRs;
            PRs = PR_ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pr in PRs)
            {

                ProductionPdfs(Convert.ToInt32(pr));
            }

            //Raise Message for Production Plan creation and send emails to Machine Head and Production Planner 

            // ---- send Shortfall Email
            //we know the start and end time taken to calulate the Deckle, pull all records from the table between this request date and send email
            DeckleRepository repoObj = new DeckleRepository();
            var ShortfallList = repoObj.GetDeckleApprovalsCreatedbetween(startDate, endDate);


            EmailforShortFall(ShortfallList);

            //foreach (deckle_approvals approval in ShortfallList)
            //{


            //}

        }
        //For Production planner Pdf and send mail to Machine Head and Production Planner
        public bool ProductionPdfs(int id)
        {
            try
            {
                string mhName = string.Empty; string macName = string.Empty; string Email = string.Empty; DateTime? eststart = null; string papermillname = string.Empty; string machineheadid = string.Empty;
                ProductionPlannerRepository ProObj = new ProductionPlannerRepository();
                CustomerRepository objCs = new CustomerRepository();
                UserMailer Objum = new UserMailer();
                var ProPlanDetails = ProObj.ProductionPlanHeaderDetails(id);
                var JumbosDetails = ProObj.ProductionPlanJmbosDetailsPdf(id);
                var LotsDetails = ProObj.ProductionPlanLotsDetails(id);
                ViewBag.estStartTime = ProPlanDetails.Select(x => x.estimated_start).First();
                ViewBag.papermillName = ProPlanDetails.Select(x => x.papermillName).First();
                ViewBag.JumbosDetails = JumbosDetails;
                ViewBag.LotsDetails = LotsDetails;
                byte[] pdfOutput = ControllerContext.GeneratePdf(JumbosDetails, "ProductionPlannerPdf");
                string CurDate = DateTime.Today.ToString("MMM-yyyy");
                bool exists = System.IO.Directory.Exists(Server.MapPath("~/MWV/PDF/" + CurDate));
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath("~/MWV/PDF/" + CurDate));
                }
                var mhDetails = ProObj.GetMachineheadDetails(id);
                foreach (var i in mhDetails)
                {
                    mhName = i.machHName;
                    macName = i.macname;
                    Email = i.Addess1;
                    eststart = i.estimated_start;
                    papermillname = i.papermillName;
                    machineheadid = i.CustomerName;

                }
                string finalPath = "/MWV/PDF/" + CurDate;
                papermillname = papermillname.Replace(" ", "_");
                string fullPath = Server.MapPath(finalPath + "/" + papermillname + "_Plan_" + id.ToString() + ".pdf");
                string attachment1 = "/MWV/PDF/" + CurDate + "/" + papermillname + "_Plan_" + id.ToString() + ".pdf";
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                System.IO.File.WriteAllBytes(fullPath, pdfOutput);
                bool updatePath = ProObj.UpdatePathForPdfPP(attachment1, id);
                string Planneremail = objCs.GetEmail();
                MailoperationPP(macName, mhName, Email, eststart, fullPath, id, Planneremail, papermillname, mhDetails, machineheadid);
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CreateCustomer->ProductionPdfs:", Ex);
                return false;
            }

        }
        //Mail To Production Planner and Machine Head
        private void MailoperationPP(string macineName, string machinceHeadName, string email, DateTime? eststart, string filePath, int id, string Planneremail, string papermillname, List<TempPendingApproval> lst, string MachId)
        {
            try
            {
                string cuUser = User.Identity.GetUserId();
                ProductionPlannerRepository objEMail = new ProductionPlannerRepository();
                CustomerRepository objCR = new CustomerRepository();
                EmailController emailFunc = new EmailController();
                MessageAndAlertsBusiness objmail = new MessageAndAlertsBusiness();
                UserMailer objusm = new UserMailer();
                ViewBag.MachHname = machinceHeadName;
                ViewBag.Machname = macineName;
                ViewBag.PlanNo = id;
                ViewBag.SchStartDate = eststart;
                string getFilePath = objEMail.GetFilePath(id);
                string PathOffile = Path.Combine(Server.MapPath(getFilePath));
                string msgSub = "Production Plan #" + id + " Created For " + papermillname;
                //  DateTime dt = DateTime.ParseExact(eststart.ToString(), "dd MMM yyyy", null);
                string dateAlert = eststart.Value.ToShortDateString();

                string alertText = "Production Plan #" + id + " has been created for " + papermillname + " , Scheduled Start :" + dateAlert;
                string pdfOutput = emailFunc.GeneratePdfOutput(this.ControllerContext, lst, "PapermillPlanCreated");

                bool statusofmail = objusm.sendMails(email, "", msgSub, Planneremail, "", "", "", pdfOutput, PathOffile, "");

                if (statusofmail == false)
                {
                    objmail.CreateMessagesDetails("PlanCreated", "MachineHead", null, pdfOutput, msgSub, "", Planneremail, "", machinceHeadName, getFilePath, "Failed", MachId, null);
                    objmail.CreateMessagesDetails("PlanCreated", "ProductionPlanner", null, pdfOutput, msgSub, "", Planneremail, "", machinceHeadName, getFilePath, "Failed", null, null);
                }
                else
                {
                    objmail.CreateMessagesDetails("PlanCreated", "MachineHead", null, pdfOutput, msgSub, "", Planneremail, "", machinceHeadName, getFilePath, "Deliverd", MachId, null);
                    objmail.CreateMessagesDetails("PlanCreated", "ProductionPlanner", null, pdfOutput, msgSub, "", Planneremail, "", machinceHeadName, getFilePath, "Deliverd", null, null);
                }

                objmail.CreateAlertDetails("PlanCreated", "MachineHead", null, alertText, msgSub, machinceHeadName, MachId);
                objmail.CreateAlertDetails("PlanCreated", "ProductionPlanner", null, alertText, msgSub, machinceHeadName, null);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CreateCustomer->MailoperationPP:", Ex);

            }


        }

        //For ShortFall Email to All Agent in System And production Planner
        private bool EmailforShortFall(List<dynamic> lst)
        {
            try
            {
                if (lst.Count != 0)
                {
                    EmailController emailFunc = new EmailController();
                    ProductionPlannerRepository objMail = new ProductionPlannerRepository();
                    MessageAndAlertsBusiness objEmail = new MessageAndAlertsBusiness();
                    UserMailer objusm = new UserMailer();
                    CustomerRepository objCR = new CustomerRepository();
                    List<deckle_approvals> objDklst = new List<deckle_approvals>();
                    deckle_approvals objDk = new deckle_approvals();
                    foreach (var items in lst)
                    {
                        objDk.bf_code = items.bf_code;
                        objDk.gsm_code = items.gsm_code;
                        objDk.shade_code = items.shade_code;
                        objDk.required_size = items.required_size;
                        objDk.required_weight = items.required_weight;
                        objDklst.Add(new deckle_approvals() { bf_code = objDk.bf_code, gsm_code = objDk.gsm_code, shade_code = objDk.shade_code, required_size = objDk.required_size, required_weight = objDk.required_weight });

                    }
                    ViewBag.DeckleData = objDklst;
                    string pdfOutput = emailFunc.GeneratePdfOutput(this.ControllerContext, lst, "DeckleShortfallMail");
                    string MsgSub = "Production Shortfall";
                    var emailLst = objMail.lstemails();
                    foreach (Agent items in emailLst)
                    {
                        bool statusofmail = objusm.sendMails(items.email, "", MsgSub, "", "", "", "", pdfOutput, "", "");
                        if (statusofmail == false)
                        {
                            objEmail.CreateMessagesDetails("Shortfall", "Agent", items.agent_id, pdfOutput, MsgSub, "", "", "", "", "", "Failed", null, null);
                        }
                        else
                        {
                            objEmail.CreateMessagesDetails("Shortfall", "Agent", items.agent_id, pdfOutput, MsgSub, "", "", "", "", "", "Deliverd", null, null);
                        }
                    }
                    if (emailLst.Count >= 1)
                    {
                        string planneremail = objCR.GetEmail();
                        bool statusofmail = objusm.sendMails(planneremail, "", MsgSub, "", "", "", "", pdfOutput, "", "");
                        if (statusofmail == false)
                        {
                            objEmail.CreateMessagesDetails("Shortfall", "ProductionPlanner", null, pdfOutput, MsgSub, "", "", "", "", "", "Failed", null, null);
                        }
                        else
                        {
                            objEmail.CreateMessagesDetails("Shortfall", "ProductionPlanner", null, pdfOutput, MsgSub, "", "", "", "", "", "Deliverd", null, null);
                        }

                    }
                    return true;
                }
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CreateCustomer->EmailforShortFall:", Ex);
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

        public enum ManageMessageId
        {

            ChangePasswordSuccess

        }

        public ActionResult GetAgentReport(string allVals)
        {
            Reports ObjReports = new Reports();
            StackholderRepository objReortsdata = new StackholderRepository();
            DateTime fromdt = Convert.ToDateTime(Request["fromdt"]);  //DateTime.Now.AddDays(-200);
            DateTime todt = Convert.ToDateTime(Request["todt"]); // DateTime.Now.AddDays(100);

            string PathtoFile = string.Empty;
            int agentId = 0;
            //int itemcount = 0;
            if (Request["agentId"] != "")
            {
                agentId = Convert.ToInt16(Request["agentId"]);
            }
            CustomerRepository obj = new CustomerRepository();
            DateTime currDate = DateTime.Now;
            var agentReportdetails = obj.AgentReport(agentId, allVals, fromdt, todt);
            string[] arrValues = allVals.Split(',');
            string filename = string.Empty;
            string finalPath = string.Empty;

            if (agentReportdetails.Count != 0)
            {
                foreach (var schobj in agentReportdetails)
                {

                    using (ExcelPackage Excel = new ExcelPackage())
                    {

                        int col = 1;
                        //Create a sheet
                        Excel.Workbook.Worksheets.Add("AgentReport");
                        ExcelWorksheet ws = Excel.Workbook.Worksheets[1];
                        ws.Name = "Agent Report"; //Setting Sheet's name
                        ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                        ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                        HomeController homeobj = new HomeController();
                        //Add Headers
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Customer-PO");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Customer-PO Date");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Agent-PO");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Agent-PO Date");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Item Number");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "CustomerName");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Brand Name");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Machine- PM");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "GSM");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "BF");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Variant");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Shade");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Weight (in MT)");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Width (in cm)");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Diameter (in cm)");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Product Description");

                        foreach (var item in arrValues)
                        {
                            if (item == "Under Planning")
                            {

                                homeobj.AddHeadertoExcel(ws, col++, 1, "Under Planning");

                            }
                            else if (item == "Planned")
                            {

                                homeobj.AddHeadertoExcel(ws, col++, 1, "Planned");
                            }
                            else if (item == "In Warehouse")
                            {
                                homeobj.AddHeadertoExcel(ws, col++, 1, "FG");

                            }
                            else if (item == "Dispatched")
                            {
                                homeobj.AddHeadertoExcel(ws, col++, 1, "Dispatched");
                            }
                        }

                        //homeobj.AddHeadertoExcel(ws, col++, 1, "FG");
                        //homeobj.AddHeadertoExcel(ws, col++, 1, "Dispatched");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Basic Price");
                        //homeobj.AddHeadertoExcel(ws, col++, 1, "Date of dispatch");
                        homeobj.AddHeadertoExcel(ws, col++, 1, "Vehicle Number");

                        //Dump the entire Object to E 21,xcel row
                        int row = 2;
                        int count = 1;
                        int previousordid = 0;
                        foreach (var ord_pro in agentReportdetails) //Creating Headings
                        {

                            // itemcount = itemcount + 1;
                            int printcol = 1;
                            if (ord_pro.customerPO == null || ord_pro.customerPO == "")
                            { homeobj.AddStringColtoExcel(ws, printcol++, row, ""); }
                            else { homeobj.AddStringColtoExcel(ws, printcol++, row, ord_pro.customerPO); }
                            DateTime dt = (DateTime)ord_pro.customerPodt;
                            homeobj.AddStringColtoExcel(ws, printcol++, row, dt.ToShortDateString());
                            homeobj.AddStringColtoExcel(ws, printcol++, row, ord_pro.agentPO.ToString());

                            DateTime agentPOdt = (DateTime)ord_pro.agentPOdt;
                            homeobj.AddStringColtoExcel(ws, printcol++, row, agentPOdt.ToShortDateString());
                            if (ord_pro.agentPO == previousordid)
                            {
                                previousordid = ord_pro.agentPO;
                                homeobj.AddStringColtoExcel(ws, printcol++, row, count.ToString());
                                count++;
                            }
                            else
                            {
                                count = 1;
                                previousordid = ord_pro.agentPO;
                                homeobj.AddStringColtoExcel(ws, printcol++, row, count.ToString());
                                count++;
                            }
                            homeobj.AddStringColtoExcel(ws, printcol++, row, ord_pro.customerName);
                            if (ord_pro.description == null || ord_pro.description == "")
                            { homeobj.AddStringColtoExcel(ws, printcol++, row, ""); }
                            else { homeobj.AddStringColtoExcel(ws, printcol++, row, ord_pro.brandName); }
                            homeobj.AddStringColtoExcel(ws, printcol++, row, ord_pro.machineName);
                            homeobj.AddStringColtoExcel(ws, printcol++, row, ord_pro.gsm);
                            homeobj.AddStringColtoExcel(ws, printcol++, row, ord_pro.bf.ToString());
                            if (ord_pro.Variant == "" || ord_pro.Variant == null)
                            { homeobj.AddStringColtoExcel(ws, printcol++, row, ""); }
                            else { homeobj.AddStringColtoExcel(ws, printcol++, row, ord_pro.Variant.ToString()); }

                            homeobj.AddStringColtoExcel(ws, printcol++, row, ord_pro.shade_code.ToString());
                            homeobj.AddintColtoExcel(ws, printcol++, row, (decimal)ord_pro.qty);
                            homeobj.AddintColtoExcel(ws, printcol++, row, (decimal)ord_pro.width);
                            homeobj.AddintColtoExcel(ws, printcol++, row, (decimal)ord_pro.diameter);
                            if (ord_pro.description == "" || ord_pro.description == null)
                            { homeobj.AddStringColtoExcel(ws, printcol++, row, ""); }
                            else { homeobj.AddStringColtoExcel(ws, printcol++, row, ord_pro.description.ToString()); }
                            //homeobj.AddStringColtoExcel(ws, 17, row, ord_pro.underplanningStatus.ToString());
                            //homeobj.AddStringColtoExcel(ws, 18, row, ord_pro.plannedStatus.ToString());
                            //homeobj.AddStringColtoExcel(ws, 19, row, ord_pro.FGStatus.ToString());
                            //homeobj.AddStringColtoExcel(ws, 20, row, ord_pro.dispatched.ToString());
                            foreach (var item in arrValues)
                            {
                                if (item == "Under Planning")
                                {
                                    homeobj.AddintColtoExcel(ws, printcol++, row, (decimal)ord_pro.underplanningQty);
                                }
                                else if (item == "Planned")
                                {
                                    homeobj.AddintColtoExcel(ws, printcol++, row, (decimal)ord_pro.plannedQty);
                                }
                                else if (item == "In Warehouse")
                                {
                                    homeobj.AddintColtoExcel(ws, printcol++, row, (decimal)ord_pro.FGqty);
                                }
                                else if (item == "Dispatched")
                                {
                                    homeobj.AddintColtoExcel(ws, printcol++, row, (decimal)ord_pro.dispatchedQty);
                                }
                            }

                            homeobj.AddintColtoExcel(ws, printcol++, row, (decimal)ord_pro.unit_price);
                            //if (ord_pro.dispatchedOnFactory == null)
                            //{ homeobj.AddStringColtoExcel(ws, printcol++, row, "Pending"); }
                            //else { homeobj.AddStringColtoExcel(ws, printcol++, row, ord_pro.dispatchedOnFactory.ToString()); }
                            if (ord_pro.truck_no == "")
                            {
                                homeobj.AddStringColtoExcel(ws, printcol++, row, "");
                            }
                            else { homeobj.AddStringColtoExcel(ws, printcol++, row, ord_pro.truck_no.ToString()); }

                            row++;
                        }

                        // Create the File locally
                        Byte[] bin = Excel.GetAsByteArray();
                        string folder = currDate.ToString("MMM-yyyy");
                        bool exists = System.IO.Directory.Exists(Server.MapPath("~/MWV/CSV/" + folder));
                        if (!exists)
                        {
                            System.IO.Directory.CreateDirectory(Server.MapPath("~/MWV/CSV/" + folder));
                        }
                        finalPath = "/MWV/CSV/" + folder;
                        //string fullPath = Server.MapPath(finalPath + "/" + "OrderBookReport-" + schobj.excelFileName.Replace(" ", "-") + "-" + fromdt.Year.ToString() + "-" + fromdt.Month.ToString() + "-" + fromdt.Day.ToString() + "-" + "To" + "-" + todt.Year.ToString() + "-" + todt.Month.ToString() + "-" + todt.Day.ToString() + ".xlsx");
                        string fullPath = Server.MapPath(finalPath + "/" + "AgentReport.xlsx");

                        filename = "AgentReport" + ".xlsx";
                        PathtoFile = finalPath + filename;
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        System.IO.File.WriteAllBytes(fullPath, bin);
                    }
                }
                //Save Into Reports Table
                ObjReports.created_on = DateTime.Now;
                ObjReports.file_name = PathtoFile;
                if (agentId != 0)
                {
                    var agentName = (from agent in db.Agents where agent.agent_id == agentId select agent.name).SingleOrDefault();
                    ObjReports.report_criteria = agentName + ", " + fromdt + ", " + todt;
                }
                else
                { ObjReports.report_criteria = "All" + ", " + fromdt + ", " + todt; }

                ObjReports.report_name = "AgentReport";
                objReortsdata.saveReports(ObjReports);
            }
            else
            {
                TempData["NoRecords"] = "No Records Found!";
            }
            TempData["filename"] = filename;
            TempData["finalPath"] = finalPath;
            return RedirectToAction("GetReportResult", "Stakeholder");
        }

        public ActionResult OrderBookReport()
        {
            Reports ObjReports = new Reports();
            StackholderRepository objReortsdata = new StackholderRepository();
            DateTime fromdt = Convert.ToDateTime(Request["fromdt"]);  //DateTime.Now.AddDays(-200);
            DateTime todt = Convert.ToDateTime(Request["todt"]); // DateTime.Now.AddDays(100);
            var status = Request["status"];
            var searchtype = Request["customerOrOrderType"];
            int agentId = 0;
            string Pathtofile = string.Empty;
            int customerId = 0;
            if (Request["agentId"] != "")
            {
                agentId = Convert.ToInt16(Request["agentId"]);
            }
            if (Request["customerId"] != "")
            {
                customerId = Convert.ToInt16(Request["customerId"]);
            }
            CustomerRepository obj = new CustomerRepository();
            DateTime currDate = DateTime.Now;
            var agentReportdetails = obj.OrderBookReport(searchtype, agentId, customerId, fromdt, todt);
            var Count = agentReportdetails.Count;
            string filename = string.Empty;
            string finalPath = string.Empty;
            if (Count != 0)
            {
                foreach (var schobj in agentReportdetails)
                {
                    using (ExcelPackage Excel = new ExcelPackage())
                    {

                        //Create a sheet
                        Excel.Workbook.Worksheets.Add("AgentReport");
                        ExcelWorksheet ws = Excel.Workbook.Worksheets[1];
                        ws.Name = "OrderBook Report"; //Setting Sheet's name
                        ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                        ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                        HomeController homeobj = new HomeController();
                        //Add Headers
                        homeobj.AddHeadertoExcel(ws, 1, 1, "Agent-PO");
                        homeobj.AddHeadertoExcel(ws, 2, 1, "Customer-PO");
                        homeobj.AddHeadertoExcel(ws, 3, 1, "Item Number");
                        homeobj.AddHeadertoExcel(ws, 4, 1, "Agent Name");
                        homeobj.AddHeadertoExcel(ws, 5, 1, "Customer Name");
                        homeobj.AddHeadertoExcel(ws, 6, 1, "Brand Owner");
                        homeobj.AddHeadertoExcel(ws, 7, 1, "GSM");
                        homeobj.AddHeadertoExcel(ws, 8, 1, "BF");
                        homeobj.AddHeadertoExcel(ws, 9, 1, "Shade");
                        homeobj.AddHeadertoExcel(ws, 10, 1, "Variant");
                        homeobj.AddHeadertoExcel(ws, 11, 1, "Weight (in MT)");
                        homeobj.AddHeadertoExcel(ws, 12, 1, "Width (in cm)");
                        homeobj.AddHeadertoExcel(ws, 13, 1, "Diameter");
                        homeobj.AddHeadertoExcel(ws, 14, 1, "Order Logging Date");
                        homeobj.AddHeadertoExcel(ws, 15, 1, "Requested delivery date");
                        homeobj.AddHeadertoExcel(ws, 16, 1, "Product Description");
                        homeobj.AddHeadertoExcel(ws, 17, 1, "Basic Price");
                        homeobj.AddHeadertoExcel(ws, 18, 1, "Machine-PM");
                        homeobj.AddHeadertoExcel(ws, 19, 1, "Under Planning");
                        homeobj.AddHeadertoExcel(ws, 20, 1, "Planned");
                        homeobj.AddHeadertoExcel(ws, 21, 1, "FG");
                        homeobj.AddHeadertoExcel(ws, 22, 1, "Dispatched");
                        //Dump the entire Object to E 21,xcel row
                        int row = 2;
                        int count = 1;
                        int previousordid = 0;
                        foreach (var ord_pro in agentReportdetails) //Creating Headings
                        {
                            homeobj.AddStringColtoExcel(ws, 1, row, ord_pro.agentPO.ToString());
                            homeobj.AddStringColtoExcel(ws, 2, row, ord_pro.customerPO);

                            if (ord_pro.agentPO == previousordid)
                            {
                                previousordid = ord_pro.agentPO;
                                homeobj.AddStringColtoExcel(ws, 3, row, count.ToString());
                                count++;
                            }
                            else
                            {
                                count = 1;
                                previousordid = ord_pro.agentPO;
                                homeobj.AddStringColtoExcel(ws, 3, row, count.ToString());
                                count++;
                            }

                            //homeobj.AddStringColtoExcel(ws, 3, row, (row - 1).ToString());
                            homeobj.AddStringColtoExcel(ws, 4, row, ord_pro.agentName);
                            homeobj.AddStringColtoExcel(ws, 5, row, ord_pro.customerName);
                            if (ord_pro.brandOwner == null || ord_pro.brandOwner == "")
                            { homeobj.AddStringColtoExcel(ws, 6, row, ""); }
                            else { homeobj.AddStringColtoExcel(ws, 6, row, ord_pro.brandOwner); }
                            //homeobj.AddStringColtoExcel(ws, 6, row, ord_pro.brandOwner);
                            homeobj.AddStringColtoExcel(ws, 7, row, ord_pro.gsm);
                            homeobj.AddStringColtoExcel(ws, 8, row, ord_pro.bf.ToString());
                            homeobj.AddStringColtoExcel(ws, 9, row, ord_pro.shade_code.ToString());

                            if (ord_pro.Variant == null || ord_pro.Variant == "")
                            { homeobj.AddStringColtoExcel(ws, 10, row, ""); }
                            else { homeobj.AddStringColtoExcel(ws, 10, row, ord_pro.Variant); }

                            homeobj.AddintColtoExcel(ws, 11, row, (decimal)ord_pro.qty);
                            homeobj.AddintColtoExcel(ws, 12, row, (decimal)ord_pro.width);
                            homeobj.AddintColtoExcel(ws, 13, row, (decimal)ord_pro.diameter);

                            if (ord_pro.orderloggingdt == null || ord_pro.orderloggingdt == DateTime.MinValue)
                            { homeobj.AddStringColtoExcel(ws, 14, row, ""); }
                            else
                            {
                                DateTime dt = (DateTime)ord_pro.orderloggingdt;
                                homeobj.AddStringColtoExcel(ws, 14, row, dt.ToShortDateString());
                            }

                            if (ord_pro.requested_delivery_date == null || ord_pro.requested_delivery_date == DateTime.MinValue.ToShortDateString())
                            { homeobj.AddStringColtoExcel(ws, 15, row, ""); }
                            else { homeobj.AddStringColtoExcel(ws, 15, row, ord_pro.requested_delivery_date); }

                            if (ord_pro.description == null || ord_pro.description == "")
                            { homeobj.AddStringColtoExcel(ws, 16, row, ""); }
                            else { homeobj.AddStringColtoExcel(ws, 16, row, ord_pro.description.ToString()); }

                            homeobj.AddintColtoExcel(ws, 17, row, (decimal)ord_pro.unit_price);
                            homeobj.AddStringColtoExcel(ws, 18, row, ord_pro.machineName.ToString());
                            homeobj.AddintColtoExcel(ws, 19, row, (decimal)ord_pro.underplanningQty);
                            homeobj.AddintColtoExcel(ws, 20, row, (decimal)ord_pro.plannedQty);
                            homeobj.AddintColtoExcel(ws, 21, row, (decimal)ord_pro.FGqty);
                            homeobj.AddintColtoExcel(ws, 22, row, (decimal)ord_pro.dispatchedQty);
                            row++;
                        }


                        // Create the File locally
                        Byte[] bin = Excel.GetAsByteArray();
                        string folder = currDate.ToString("MMM-yyyy");
                        bool exists = System.IO.Directory.Exists(Server.MapPath("~/MWV/CSV/" + folder));
                        if (!exists)
                        {
                            System.IO.Directory.CreateDirectory(Server.MapPath("~/MWV/CSV/" + folder));
                        }
                        finalPath = "/MWV/CSV/" + folder;
                        //string fullPath = Server.MapPath(finalPath + "/" + "OrderBookReport-" + schobj.excelFileName.Replace(" ", "-") + "-" + fromdt.Year.ToString() + "-" + fromdt.Month.ToString() + "-" + fromdt.Day.ToString() + "-" + "To" + "-" + todt.Year.ToString() + "-" + todt.Month.ToString() + "-" + todt.Day.ToString() + ".xlsx");
                        string fullPath = Server.MapPath(finalPath + "/" + "OrderBookReport.xlsx");

                        filename = "OrderBookReport.xlsx";
                        Pathtofile = finalPath + "/" + "OrderBookReport.xlsx";
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        System.IO.File.WriteAllBytes(fullPath, bin);

                    }
                }
                var agentName = (dynamic)null;
                var customerName = (dynamic)null;
                switch (searchtype)
                {
                    case "by-customer":

                        customerName = (from cust in db.Customers
                                        where cust.customer_id == customerId
                                        select cust.name).SingleOrDefault();

                        break;
                    case "by-agent":
                        agentName = (from agent in db.Agents
                                     where agent.agent_id == agentId
                                     select agent.name).SingleOrDefault();
                        break;
                }

                //Save Into Reports Table
                ObjReports.created_on = DateTime.Now;
                ObjReports.file_name = Pathtofile;
                if (searchtype == "by-customer")
                { ObjReports.report_criteria = searchtype + ", " + customerName + ", " + fromdt + ", " + todt; }
                else if (searchtype == "by-agent")
                { ObjReports.report_criteria = searchtype + ", " + agentName + ", " + fromdt + ", " + todt; }
                else
                { ObjReports.report_criteria = searchtype + ", " + "All" + ", " + fromdt + ", " + todt; }

                ObjReports.report_name = "OrderBookReport";
                objReortsdata.saveReports(ObjReports);
            }
            else
            {
                TempData["NoRecords"] = "No Records Found!";
            }

            TempData["filename"] = filename;
            TempData["finalPath"] = finalPath;
            return RedirectToAction("GetReportResult", "Stakeholder", filename);
        }

        public ActionResult BDReport()
        {
            Reports ObjReports = new Reports();
            StackholderRepository objReortsdata = new StackholderRepository();
            DateTime fromdt = Convert.ToDateTime(Request["fromdt"]);  //DateTime.Now.AddDays(-200);
            DateTime todt = Convert.ToDateTime(Request["todt"]); // DateTime.Now.AddDays(100);
            int agentId = 0;
            int customerId = 0;
            int papermillId = 0;
            string PathTofile = string.Empty;
            if (Request["customerId"] != "")
            {
                customerId = Convert.ToInt16(Request["customerId"]);
            }
            if (Request["agentId"] != "")
            {
                agentId = Convert.ToInt16(Request["agentId"]);
            }
            if (Request["papermillId"] != "")
            {
                papermillId = Convert.ToInt16(Request["papermillId"]);
            }
            CustomerRepository obj = new CustomerRepository();
            string filename = string.Empty;
            string finalPath = string.Empty;
            var customerOragentType = Request["customerOrOrderType"];
            var agentReportdetails = obj.BillofDateReport(customerOragentType, agentId, customerId, papermillId, fromdt, todt);
            var Count = agentReportdetails.Count();
            if (Count != 0)
            {
                foreach (var schobj in agentReportdetails)
                {
                    using (ExcelPackage Excel = new ExcelPackage())
                    {

                        //Create a sheet
                        Excel.Workbook.Worksheets.Add("AgentReport");
                        ExcelWorksheet ws = Excel.Workbook.Worksheets[1];
                        ws.Name = "BD Report"; //Setting Sheet's name
                        ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                        ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                        HomeController homeobj = new HomeController();
                        //Add Headers
                        homeobj.AddHeadertoExcel(ws, 1, 1, "Agent-PO");
                        homeobj.AddHeadertoExcel(ws, 2, 1, "PO Number");
                        homeobj.AddHeadertoExcel(ws, 3, 1, "Item Number");
                        homeobj.AddHeadertoExcel(ws, 4, 1, "Agent-PO Date");
                        homeobj.AddHeadertoExcel(ws, 5, 1, "AgentName");
                        homeobj.AddHeadertoExcel(ws, 6, 1, "Brand Name");
                        homeobj.AddHeadertoExcel(ws, 7, 1, "Machine- PM");
                        homeobj.AddHeadertoExcel(ws, 8, 1, "GSM");
                        homeobj.AddHeadertoExcel(ws, 9, 1, "BF");
                        homeobj.AddHeadertoExcel(ws, 10, 1, "Shade");
                        homeobj.AddHeadertoExcel(ws, 11, 1, "Variant");
                        homeobj.AddHeadertoExcel(ws, 12, 1, "Product Description");
                        homeobj.AddHeadertoExcel(ws, 13, 1, "Diameter");
                        homeobj.AddHeadertoExcel(ws, 14, 1, "Width (in cm)");
                        homeobj.AddHeadertoExcel(ws, 15, 1, "Rate");
                        homeobj.AddHeadertoExcel(ws, 16, 1, "Bill Amt.");
                        homeobj.AddHeadertoExcel(ws, 17, 1, "Weight (in MT)");
                        homeobj.AddHeadertoExcel(ws, 18, 1, "Requested delivery date");
                        homeobj.AddHeadertoExcel(ws, 19, 1, "Actual delivery date");
                        homeobj.AddHeadertoExcel(ws, 20, 1, "Vehicle Number");
                        homeobj.AddHeadertoExcel(ws, 21, 1, "delivery Order No");
                        homeobj.AddHeadertoExcel(ws, 22, 1, "State Name");
                        //homeobj.AddHeadertoExcel(ws, 23, 1, "Date of dispatch");

                        //Dump the entire Object to E 21,xcel row
                        int row = 2;
                        int count = 1;
                        int previousordid = 0;
                        foreach (var ord_pro in agentReportdetails) //Creating Headings
                        {
                            homeobj.AddStringColtoExcel(ws, 1, row, ord_pro.agentPO.ToString());
                            homeobj.AddStringColtoExcel(ws, 2, row, ord_pro.agentPO.ToString());//po no. same as agent po
                            if (ord_pro.agentPO == previousordid)
                            {
                                previousordid = ord_pro.agentPO;
                                homeobj.AddStringColtoExcel(ws, 3, row, count.ToString());
                                count++;
                            }
                            else
                            {
                                count = 1;
                                previousordid = ord_pro.agentPO;
                                homeobj.AddStringColtoExcel(ws, 3, row, count.ToString());
                                count++;
                            }

                            //homeobj.AddStringColtoExcel(ws, 3, row, (row - 1).ToString());
                            if (ord_pro.agentPOdt != null)
                            {
                                DateTime dt = (DateTime)ord_pro.agentPOdt;
                                homeobj.AddStringColtoExcel(ws, 4, row, dt.ToShortDateString());
                            }
                            else { homeobj.AddStringColtoExcel(ws, 4, row, ""); }
                            homeobj.AddStringColtoExcel(ws, 5, row, ord_pro.agentName);

                            if (ord_pro.brandName == "" || ord_pro.brandName == null)
                            { homeobj.AddStringColtoExcel(ws, 6, row, ""); }
                            else { homeobj.AddStringColtoExcel(ws, 6, row, ord_pro.brandName.ToString()); }

                            homeobj.AddStringColtoExcel(ws, 7, row, ord_pro.machineName);
                            homeobj.AddStringColtoExcel(ws, 8, row, ord_pro.gsm);
                            homeobj.AddStringColtoExcel(ws, 9, row, ord_pro.bf.ToString());
                            homeobj.AddStringColtoExcel(ws, 10, row, ord_pro.shade_code.ToString());
                            //homeobj.AddHeadertoExcel(ws, 11, 1, "Variant");

                            if (ord_pro.Variant == "" || ord_pro.Variant == null)
                            { homeobj.AddStringColtoExcel(ws, 11, row, ""); }
                            else { homeobj.AddStringColtoExcel(ws, 11, row, ord_pro.Variant.ToString()); }

                            if (ord_pro.description != "" || ord_pro.description != null)
                            { homeobj.AddStringColtoExcel(ws, 12, row, ord_pro.description.ToString()); }
                            else { homeobj.AddStringColtoExcel(ws, 12, row, ""); }

                            homeobj.AddintColtoExcel(ws, 13, row, (decimal)ord_pro.diameter);
                            homeobj.AddintColtoExcel(ws, 14, row, (decimal)ord_pro.width);
                            homeobj.AddintColtoExcel(ws, 15, row, (decimal)ord_pro.qty);
                            homeobj.AddintColtoExcel(ws, 16, row, (decimal)ord_pro.amount);

                            homeobj.AddintColtoExcel(ws, 17, row, (decimal)ord_pro.qty);
                            if (ord_pro.requested_delivery_date != null)
                            { homeobj.AddStringColtoExcel(ws, 18, row, ord_pro.requested_delivery_date.ToString()); }
                            else if (ord_pro.requested_delivery_date == null || ord_pro.requested_delivery_date == DateTime.MinValue.ToShortDateString())
                            { homeobj.AddStringColtoExcel(ws, 18, row, ""); }

                            if (ord_pro.actualDeliveryDate == null || ord_pro.actualDeliveryDate == DateTime.MinValue.ToShortDateString())
                            { homeobj.AddStringColtoExcel(ws, 19, row, ""); }
                            else { homeobj.AddStringColtoExcel(ws, 19, row, ord_pro.actualDeliveryDate.ToString()); }


                            if (ord_pro.truck_no == "")
                            {
                                homeobj.AddStringColtoExcel(ws, 20, row, "");
                            }
                            else { homeobj.AddStringColtoExcel(ws, 20, row, ord_pro.truck_no.ToString()); }

                            //homeobj.AddStringColtoExcel(ws, 20, row, ord_pro.truck_no);
                            homeobj.AddStringColtoExcel(ws, 21, row, ord_pro.agentPO.ToString());

                            if (ord_pro.stateName == null || ord_pro.stateName == "")
                            { homeobj.AddStringColtoExcel(ws, 22, row, ""); }
                            else { homeobj.AddStringColtoExcel(ws, 22, row, ord_pro.stateName.ToString()); }

                            //if (ord_pro.dispatchedOnFactory != null)
                            //{ homeobj.AddStringColtoExcel(ws, 23, row, ord_pro.dispatchedOnFactory.ToString()); }
                            //else { homeobj.AddStringColtoExcel(ws, 23, row, "pending"); }

                            row++;
                        }

                        // Create the File locally
                        DateTime currDate = DateTime.Now;
                        Byte[] bin = Excel.GetAsByteArray();
                        string folder = currDate.ToString("MMM-yyyy");
                        bool exists = System.IO.Directory.Exists(Server.MapPath("~/MWV/CSV/" + folder));
                        if (!exists)
                        {
                            System.IO.Directory.CreateDirectory(Server.MapPath("~/MWV/CSV/" + folder));
                        }
                        finalPath = "/MWV/CSV/" + folder;
                        //string fullPath = Server.MapPath(finalPath + "/" + "BDreport-" + schobj.excelFileName.Replace(" ", "-") + "-" + fromdt.Year.ToString() + "-" + fromdt.Month.ToString() + "-" + fromdt.Day.ToString() + "-" + "To" + "-" + todt.Year.ToString() + "-" + todt.Month.ToString() + "-" + todt.Day.ToString() + ".xlsx");
                        string fullPath = Server.MapPath(finalPath + "/" + "BDreport.xlsx");
                        PathTofile = finalPath + "/" + "BDreport.xlsx";
                        filename = "BDreport.xlsx";
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        System.IO.File.WriteAllBytes(fullPath, bin);

                    }
                }
                //Save Into Reports Table

                var agentName = (dynamic)null;
                var customerName = (dynamic)null;
                switch (customerOragentType)
                {
                    case "by-customer":

                        customerName = (from cust in db.Customers
                                        where cust.customer_id == customerId
                                        select cust.name).SingleOrDefault();

                        break;
                    case "by-agent":
                        agentName = (from agent in db.Agents
                                     where agent.agent_id == agentId
                                     select agent.name).SingleOrDefault();
                        break;
                }

                //Save Into Reports Table
                ObjReports.created_on = DateTime.Now;
                ObjReports.file_name = PathTofile;
                if (customerOragentType == "by-customer")
                { ObjReports.report_criteria = customerOragentType + ", " + customerName + ", " + fromdt + ", " + todt; }
                else if (customerOragentType == "by-agent")
                { ObjReports.report_criteria = customerOragentType + ", " + agentName + ", " + fromdt + ", " + todt; }
                else
                { ObjReports.report_criteria = customerOragentType + ", " + "All" + ", " + fromdt + ", " + todt; }

                ObjReports.report_name = "BdReport";
                objReortsdata.saveReports(ObjReports);
            }
            else
            {
                TempData["NoRecords"] = "No Records Found!";
            }


            TempData["filename"] = filename;
            TempData["finalPath"] = finalPath;
            return RedirectToAction("GetReportResult", "Stakeholder", filename);
        }

        public ActionResult FGReport()
        {
            Reports ObjReports = new Reports();
            StackholderRepository objReortsdata = new StackholderRepository();
            DateTime fromdt = DateTime.Now.AddDays(-100);
            DateTime todt = DateTime.Now;
            CustomerRepository obj = new CustomerRepository();
            DateTime currDate = DateTime.Now;
            string pathTofile = string.Empty;
            string finalPath = string.Empty;
            string filename = string.Empty;

            var fgDetails = obj.GetFGReportDetails(fromdt, todt, 3);
            var count = fgDetails.Count;

            if (count != 0)
            {
                foreach (var schobj in fgDetails)
                {
                    using (ExcelPackage Excel = new ExcelPackage())
                    {

                        //Create a sheet
                        Excel.Workbook.Worksheets.Add("AgentReport");
                        ExcelWorksheet ws = Excel.Workbook.Worksheets[1];
                        ws.Name = "OrderBook Report"; //Setting Sheet's name
                        ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                        ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                        HomeController homeobj = new HomeController();
                        //Add Headers
                        //homeobj.AddHeadertoExcel(ws, 1, 1, "Agent-PO");
                        homeobj.AddHeadertoExcel(ws, 1, 1, "Customer-PO");
                        homeobj.AddHeadertoExcel(ws, 2, 1, "Item Number");
                        homeobj.AddHeadertoExcel(ws, 3, 1, "Agent Name");
                        homeobj.AddHeadertoExcel(ws, 4, 1, "Customer Name");
                        homeobj.AddHeadertoExcel(ws, 5, 1, "Brand Owner");
                        homeobj.AddHeadertoExcel(ws, 6, 1, "Machine-PM");
                        homeobj.AddHeadertoExcel(ws, 7, 1, "GSM");
                        homeobj.AddHeadertoExcel(ws, 8, 1, "BF");
                        homeobj.AddHeadertoExcel(ws, 9, 1, "Shade");
                        homeobj.AddHeadertoExcel(ws, 10, 1, "Variant");
                        homeobj.AddHeadertoExcel(ws, 11, 1, "Weight (in MT)");
                        homeobj.AddHeadertoExcel(ws, 12, 1, "Roll No.");
                        homeobj.AddHeadertoExcel(ws, 13, 1, "Lot No.");
                        homeobj.AddHeadertoExcel(ws, 14, 1, "Width (in cm)");
                        homeobj.AddHeadertoExcel(ws, 15, 1, "Diameter");

                        //homeobj.AddHeadertoExcel(ws, 16, 1, "Age of stock");
                        //Dump the entire Object to E 21,xcel row
                        int row = 2;
                        foreach (var ord_pro in fgDetails) //Creating Headings
                        {
                            //homeobj.AddStringColtoExcel(ws, 1, row, ord_pro.agentPO.ToString());
                            homeobj.AddStringColtoExcel(ws, 1, row, ord_pro.customerPO);
                            homeobj.AddStringColtoExcel(ws, 2, row, (row - 1).ToString());
                            homeobj.AddStringColtoExcel(ws, 3, row, ord_pro.agentName);
                            homeobj.AddStringColtoExcel(ws, 4, row, ord_pro.customerName);
                            //homeobj.AddStringColtoExcel(ws,5, row, ord_pro.brandName);
                            homeobj.AddStringColtoExcel(ws, 6, row, ord_pro.machineName);

                            homeobj.AddStringColtoExcel(ws, 7, row, ord_pro.gsm);
                            homeobj.AddStringColtoExcel(ws, 8, row, ord_pro.bf.ToString());
                            homeobj.AddStringColtoExcel(ws, 9, row, ord_pro.shade_code.ToString());
                            //homeobj.AddStringColtoExcel(ws, 10, row, ord_pro.Variant.ToString());
                            homeobj.AddStringColtoExcel(ws, 11, row, ord_pro.qty.ToString());
                            homeobj.AddStringColtoExcel(ws, 12, row, ord_pro.rollNO.ToString());
                            homeobj.AddStringColtoExcel(ws, 13, row, ord_pro.lotno.ToString());
                            homeobj.AddStringColtoExcel(ws, 14, row, ord_pro.width.ToString());
                            homeobj.AddStringColtoExcel(ws, 15, row, ord_pro.diameter.ToString());
                            row++;
                        }

                        // Create the File locally
                        Byte[] bin = Excel.GetAsByteArray();
                        string folder = currDate.ToString("MMM-yyyy");
                        bool exists = System.IO.Directory.Exists(Server.MapPath("~/MWV/CSV/" + folder));
                        if (!exists)
                        {
                            System.IO.Directory.CreateDirectory(Server.MapPath("~/MWV/CSV/" + folder));
                        }
                        finalPath = "/MWV/CSV/" + folder;
                        //string fullPath = Server.MapPath(finalPath + "/" + "OrderBook_" + schobj.excelFileName.Replace(" ", "_") + fromdt.Year.ToString() + "-" + fromdt.Month.ToString() + fromdt.Day.ToString() + "To" + todt.Year.ToString() + "-" + todt.Month.ToString() + todt.Day.ToString() + ".xlsx");
                        // string fullPath = Server.MapPath(finalPath + "/" + "FGReport_" + schobj.excelFileName.Replace(" ", "-") + "-" + fromdt.Year.ToString() + "-" + fromdt.Month.ToString() + "-" + fromdt.Day.ToString() + "-" + "To" + "-" + todt.Year.ToString() + "-" + todt.Month.ToString() + "-" + todt.Day.ToString() + ".xlsx");
                        // pathTofile = finalPath + "FGReport_" + schobj.excelFileName.Replace(" ", "-") + "-" + fromdt.Year.ToString() + "-" + fromdt.Month.ToString() + "-" + fromdt.Day.ToString() + "-" + "To" + "-" + todt.Year.ToString() + "-" + todt.Month.ToString() + "-" + todt.Day.ToString() + ".xlsx";
                        //string attachment1 = Server.MapPath(finalPath + "/" + schobj.excelFileName.Replace(" ", "-") + fromdt.Year.ToString() + "-" + fromdt.Month.ToString() + fromdt.Day.ToString() + "To" + todt.Year.ToString() + "-" + todt.Month.ToString() + todt.Day.ToString() + ".xlsx");

                        string fullPath = Server.MapPath(finalPath + "/" + "FGReport.xlsx");
                        filename = "FgReport.xlsx";
                        pathTofile = finalPath + "/" + filename;

                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        System.IO.File.WriteAllBytes(fullPath, bin);

                    }

                }
                //Save Into Reports Table

                ObjReports.created_on = DateTime.Now;
                ObjReports.file_name = pathTofile;
                ObjReports.report_criteria = fromdt + "," + todt;
                ObjReports.report_name = "FGReport";
                objReortsdata.saveReports(ObjReports);
            }
            else
            {
                TempData["NoRecords"] = "No Records Found!";
            }

            TempData["filename"] = filename;
            TempData["finalPath"] = finalPath;
            return RedirectToAction("GetReportResult", "Stakeholder");
        }

        public ActionResult Download(string filename)
        {

            // string fullPath = Server.MapPath("~" + filename);
            string filenameExtension = Path.GetFileName(filename);
            Response.AddHeader("Content-Disposition", "attachment; filename=" + filenameExtension);
            return File(filename, "application/excel");

        }

        public ActionResult SourcingReportGetData()
        {
            try
            {
                Reports ObjReports = new Reports();
                StackholderRepository objReortsdata = new StackholderRepository();
                DateTime fromdate = Convert.ToDateTime(Request["fromdt"]);
                DateTime todate = Convert.ToDateTime(Request["todt"]);
                string filename = string.Empty;
                string finalPath = string.Empty;
                string fullPath = string.Empty;
                string PathtoFile = string.Empty;
                DateTime currDate = DateTime.Now;
                ProductionPlannerRepository objpr = new ProductionPlannerRepository();
                string[,] lstdata = (string[,])objpr.GetSourcingData(fromdate, todate);

                if (lstdata != null)
                {
                    using (ExcelPackage Excel = new ExcelPackage())
                    {

                        //Create a sheet
                        Excel.Workbook.Worksheets.Add("Sourcing Report");
                        ExcelWorksheet ws = Excel.Workbook.Worksheets[1];
                        ws.Name = "Sourcing Report"; //Setting Sheet's name
                        ws.Cells.Style.Font.Size = 11; //Default font size for whole sheet
                        ws.Cells.Style.Font.Name = "Calibri"; //Default Font name for whole sheet

                        //Add Headers to the Excel sheet from the array index, we already have the header names in index 0 (Row 0)

                        for (int column = 0; column <= lstdata.GetUpperBound(1); column++)
                        {
                            AddHeadertoExcel(ws, column + 1, 1, lstdata[0, column]);
                        }

                        //Add Table Row to the Excel sheet, different formatting  from header row
                        for (int Row = 1; Row <= lstdata.GetUpperBound(0); Row++)
                        {
                            for (int column = 0; column <= lstdata.GetUpperBound(1); column++)
                            {
                                AddStringColtoExcel(ws, column + 1, Row + 1, lstdata[Row, column]);
                            }
                        }

                        // Create the File locally
                        Byte[] bin = Excel.GetAsByteArray();
                        string folder = currDate.ToString("MMM-yyyy");
                        bool exists = System.IO.Directory.Exists(Server.MapPath("~/MWV/CSV/" + folder));
                        if (!exists)
                        {
                            System.IO.Directory.CreateDirectory(Server.MapPath("~/MWV/CSV/" + folder));
                        }
                        finalPath = "/MWV/CSV/" + folder;
                        // string fullPath = Server.MapPath(finalPath + "/" + "Sourcing Report-" + currDate.Year.ToString() + currDate.Month.ToString() + currDate.Day.ToString() + ".xlsx");
                        //string fullPath = Server.MapPath(finalPath + "/" + "SourcingReport" + "-" + fromdate.Year.ToString() + "-" + fromdate.Month.ToString() + "-" + fromdate.Day.ToString() + "-" + "To" + "-" + todate.Year.ToString() + "-" + todate.Month.ToString() + "-" + todate.Day.ToString() + ".xlsx");
                        //string attachment1 = Server.MapPath(finalPath + "/" + "sourcingReport-" + currDate.Year.ToString() + currDate.Month.ToString() + currDate.Day.ToString() + ".xlsx");

                        fullPath = Server.MapPath(finalPath + "/" + "SourcingReport.xlsx");
                        string attachment1 = Server.MapPath(finalPath + "/" + "SourcingReport.xlsx");
                        filename = "SourcingReport.xlsx";
                        PathtoFile = finalPath + "/" + filename;
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                        System.IO.File.WriteAllBytes(fullPath, bin);

                    }
                    //Save Into Reports Table
                    ObjReports.created_on = DateTime.Now;
                    ObjReports.file_name = PathtoFile;
                    ObjReports.report_criteria = fromdate + ", " + todate;
                    ObjReports.report_name = "SorcingReport";
                    objReortsdata.saveReports(ObjReports);
                    MailReportTostackholder(fromdate.ToString("dd MMM yyyy"), todate.ToString("dd MMM yyyy"), fullPath);
                }
                else
                {
                    TempData["NoRecords"] = "No Records Found!";
                }

                TempData["filename"] = filename;
                TempData["finalPath"] = finalPath;
                return RedirectToAction("GetReportResult", "Stakeholder");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in HomeController->SourcingReportGetData:", Ex);
                return null;
            }
        }

        //Send Mail to Stackholder
        public bool MailReportTostackholder(string Fromdate, string todate, string filepath)
        {
            try
            {
                MessageAndAlertsBusiness objMessage = new MessageAndAlertsBusiness();
                UserMailer objUser = new UserMailer();
                ProductionPlannerRepository objor = new ProductionPlannerRepository();
                string id = User.Identity.GetUserId();
                string StackHolderMail = objor.getStackholderEmail(id);
                string subject = "Sourcing Report - " + Fromdate + " - " + todate;
                string msgbody = "Dear Stackholder,<br/> Sourcing Report For Date " + Fromdate + "-" + todate + "<br />" + "Attach Excel File Is Details,<br /> " + "<br /> Reagards,<br/>" + "Support Team,<br/>" + "support@westrock.com.";
                var status = objUser.sendMails(StackHolderMail, "", subject, "", "", "", "", msgbody, filepath, "");
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in HomeController->SourcingReportGetData:", Ex);
                return false;
            }
        }

        public void CreateSchudle()
        {
            ProductionPlannerRepository objpr = new ProductionPlannerRepository();
            string status = "Active";
            var lst = objpr.GetSchudle(status);
            bool result = objpr.CreateSchudle(new Schedule() { papermill_id = 3, shade_code = "NATURAL", start_date = DateTime.Now, end_date = DateTime.Now.AddDays(5), created_on = DateTime.Now, created_by = "ProductionPlanner", status = "Active" });
            bool EditResult = objpr.EditSchudle(new Schedule() { schedule_id = 3, start_date = DateTime.Now, end_date = DateTime.Now.AddDays(5), modified_on = DateTime.Now, modified_by = "ProductionPlanner", status = "Active" });
            bool DeleteSch = objpr.DeleteSchudle(3);

        }
    }
}
