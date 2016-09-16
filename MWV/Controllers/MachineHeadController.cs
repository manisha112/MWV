
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWV.Models;
using MWV.DBContext;
using Microsoft.AspNet.Identity;
using MWV.Repository.Implementation;
using System.Reflection;

namespace MWV.Controllers
{
    public class MachineHeadController : Controller
    {

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();
        MachineHeadRepository machineObj = new MachineHeadRepository();
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

        DateTime offStopDateTime;
        string Remark = string.Empty;
        string offStopTimeHourAmPm = string.Empty;
        string offStartTimeHourAmPm = string.Empty;
        string[] offStopTime;

        MachineHeadRepository MachineObj = new MachineHeadRepository();
        // GET: /MachineHead/
         [HandleModelStateException]
        public ActionResult Index()
        {
            try
            {
                string id = User.Identity.GetUserId();
                var Asssignedmc = from r in db.AspNetUsers
                                  where r.Id == id
                                  select r;
                var papermillId = (from r in Asssignedmc
                                   join re in db.Papermills on r.AssignedMachine equals re.name
                                   select re.papermill_id).SingleOrDefault();

                var machineStatus = (from r in db.Papermills
                                     where r.papermill_id == papermillId
                                     select r.status).SingleOrDefault();

                ViewBag.status = machineStatus;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in MachineHeadController->Index:", Ex);
                return null;
            }
            return View("DashBoard");
        }

        //Stop machine by machine head 
         [HandleModelStateException]
        public ActionResult SubmitMachineOffDetails()
        {
            try
            {
                Remark = Request["SelectedRemark"];
                offStopDateTime = Convert.ToDateTime(Request["offStopDateTime"]);
                offStopTimeHourAmPm = Request["offStopTimeHourAmPm"];
                offStopTime = Request["offStopDateTime"].Split(':');
                if (offStopTimeHourAmPm == "am")
                {
                    offStopDateTime = Convert.ToDateTime(Request["offStopDateTime"]);
                }
                else
                {
                    offStopDateTime = offStopDateTime.AddHours(Convert.ToDouble(offStopTime[1]) + 12);
                }

                DateTime offStartDateTime = Convert.ToDateTime(Request["offStartDateTime"]);
                string offStartTimeHourAmPm = Request["offStartTimeHourAmPm"];
                string[] offStartTime = Request["offStartDateTime"].Split(':');

                if (offStartTimeHourAmPm == "am")
                {
                    offStartDateTime = Convert.ToDateTime(Request["offStartDateTime"]); ;
                }
                else
                {
                    offStartDateTime = offStartDateTime.AddHours(Convert.ToDouble(offStartTime[1]) + 12);
                }
                machineObj.SubmitMachineOffDetails(offStartDateTime, offStopDateTime, Remark);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in MachineHeadController->SubmitMachineOffDetails:", Ex);
                return null;
            }
            return View();
        }

        //Start machine by machine head 
         [HandleModelStateException]
        public ActionResult SubmitMachineOnDetails()
        {
            try
            {
                Remark = Request["SelectedRemark"];
                DateTime onStartDateTime = Convert.ToDateTime(Request["onStartDateTime"]);
                string onStartTimeHourAmPm = Request["onStartTimeHourAmPm"];
                string[] onStartTime = Request["onStartDateTime"].Split(':');
                if (onStartTimeHourAmPm == "am")
                {
                    onStartDateTime = Convert.ToDateTime(Request["onStartDateTime"]);
                }
                else
                {
                    onStartDateTime = onStartDateTime.AddHours(Convert.ToDouble(onStartTime[1]) + 12);
                }
                machineObj.SubmitMachineOnDetails(onStartDateTime, Remark);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in MachineHeadController->SubmitMachineOnDetails:", Ex);
                return null;
            }
            return View();
        }
         [HandleModelStateException]
        public ActionResult GetProPlanDetailsBydates()
        {
            try
            {
                DateTime dt = Convert.ToDateTime(Request["selectedDateTime"]);
                var ProPlanDetails = MachineObj.ProductionPlanHeaderDetails(dt);
                var JumbosDetails = MachineObj.ProductionPlanJmbosDetails(dt);

                if (!ProPlanDetails.Any())
                {
                    ViewBag.NoRecordMsg = "No data available !";
                }

                ViewBag.ProPlanDetails = ProPlanDetails;
                ViewBag.JumbosDetails = JumbosDetails;
                ViewBag.NoOfJumboes = JumbosDetails;
                ViewBag.Lots = MachineObj.ProductionPlanLotsDetails(dt);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in MachineHeadController->GetProPlanDetailsBydates:", Ex);
                return null;
            }
            return PartialView("_SeeProPlanDetails");
        }

        public void SaveProductionRunActualTime(int id)
        {
            try
            {

                var ProPlanDetails = (from pr in db.ProductionRun
                                      where pr.pr_id == id // id is the plan no.
                                      select pr).SingleOrDefault();

                ProPlanDetails.actual_start = DateTime.Now;
                db.SaveChanges();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in MachineHeadController->SaveProductionRunActualTime:", Ex);
            }
        }

        public void SaveProductiionPlanEndTime(int id)
        {
            try
            {

                var ProPlanDetails = (from pr in db.ProductionRun
                                      where pr.pr_id == id // id is the plan no.
                                      select pr).SingleOrDefault();

                ProPlanDetails.actual_end = DateTime.Now;
                db.SaveChanges();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in MachineHeadController->SaveProductiionPlanEndTime:", Ex);
            }
        }

        public void SaveProductionJumboActualTime(int id)
        {
            try
            {
                int planNo = Convert.ToInt16(Request["planNo"]);
                var jumboDetails = (from pj in db.ProductionJumbo
                                      where pj.jumbo_no == id
                                      && pj.pr_id == planNo // id is the plan no.
                                      select pj).SingleOrDefault();

                jumboDetails.actual_end = DateTime.Now;
                db.SaveChanges();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in MachineHeadController->SaveProductionJumboActualTime:", Ex);
            }
        }

        public void SaveProductionJumboStart(int id)
        {
            try
            {
                int planNo = Convert.ToInt16(Request["planNo"]);
                var jumboDetails = (from pr in db.ProductionJumbo
                                      where pr.jumbo_no == id && pr.pr_id == planNo// id is the plan no.
                                      select pr).SingleOrDefault();

                jumboDetails.actual_start = DateTime.Now;
                db.SaveChanges();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in MachineHeadController->SaveProductionJumboStart:", Ex);
            }
        }

        public void SaveProductionChildActualTime(int id)
        {
            try
            {
                OrderRepository orObj = new OrderRepository();
                int planNo = Convert.ToInt16(Request["planNo"]);
                var ProPlanDetails = (from pr in db.ProductionRun
                                      join pj in db.ProductionJumbo on pr.pr_id equals pj.pr_id
                                      join pc in db.ProductionChild on pj.pj_id equals pc.pj_id
                                      join op in db.Order_products on pc.order_product_id equals op.order_product_id
                                      join ord in db.Orders on op.order_id equals ord.order_id
                                      where pr.pr_id == planNo && pc.sequence == id // id is the sequence no.
                                      select new { pc, pr, pj, ord, op }).ToList();

                foreach (var items in ProPlanDetails)
                {
                    DateTime dt = DateTime.Now;

                    StockRepository stockpObj = new StockRepository();

                    //This function is supposed to Add the Stock being produced  
                    stockpObj.StockProduced(dt, items.pr.papermill_id, (int)items.ord.agent_id, (int)items.ord.customer_id, items.op.product_code, items.pj.shade_code, (decimal)items.pc.qty);
                    items.pc.actual_end = DateTime.Now;
                    //items.op.status = "In Warehouse";
                    db.SaveChanges();

                    if (items.op.status == "Planned")
                    {
                        orObj.ChecOrderQty(items.op.order_product_id);
                    }
                    orObj.MatchedOrderStatusWithOrdPro(0, items.op.order_product_id);

                }

            }
            catch (Exception Ex)
            {
                logger.Error("Error in MachineHeadController->SaveProductionChildActualTime:", Ex);
            }
        }


        public void SaveProductionLotStart(int id)
        {
            try
            {
                int planNo = Convert.ToInt16(Request["planNo"]);
                var lotDetails = (from pr in db.ProductionJumbo
                                 join pc in db.ProductionChild on pr.pj_id equals pc.pj_id
                                 where pr.pr_id == planNo
                                 && pc.sequence == id // id is the sequence no.
                                 select pc).ToList();

                foreach (var items in lotDetails)
                {
                    DateTime dt = DateTime.Now;

                    items.actual_start = DateTime.Now;
                    db.SaveChanges();
                }
            }
            catch (Exception Ex)
            {
                logger.Error("Error in MachineHeadController->SaveProductionLotStart:", Ex);
            }
        }

        public void SaveProductionPlanOff(int id) // if production plan is on by mistake then we can off plan, if no checkbox is selected.
        {
            try
            {

                var ProPlanDetails = (from pr in db.ProductionRun
                                      where pr.pr_id == id // id is the plan no.
                                      select pr).ToList();
                foreach (var items in ProPlanDetails)
                {
                    items.actual_start = null; // if plan is going to off then actual start date will be null
                    db.SaveChanges();
                }

            }
            catch (Exception Ex)
            {
                logger.Error("Error in MachineHeadController->SaveProductionPlanOff:", Ex);
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
