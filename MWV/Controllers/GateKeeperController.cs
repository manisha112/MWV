using MWV.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWV.Models;
using MWV.Repository.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Net;
using MWV.Repository.Implementation;
using System.Reflection;
using MWV.Business;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace MWV.Controllers
{
    public class GateKeeperController : Controller
    {
        private MWVDBContext db = new MWVDBContext();
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
        //Loading GateKeeper Dashboard
        [HandleModelStateException]
        public ActionResult Index()
        {
            return View("DashBoard");
        }

        //Getting incoming truck
        [HandleModelStateException]
        public PartialViewResult TruckInward()
        {
            try
            {
                Truck_dispatchRepository RepObj = new Truck_dispatchRepository();
                var lstTruckIn = RepObj.GetTruckInward();
                if (!lstTruckIn.Any())
                {
                    ViewBag.NoRecordMsg = "No data available !";
                }
                return PartialView("_TruckInward", lstTruckIn);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Dispatch->VehicleSearchResults:", Ex);
                return null;
            }
        }

        //Getting Arrived truck
        [HandleModelStateException]
        public PartialViewResult TruckOutward()
        {
            try
            {
                Truck_dispatchRepository RepObj = new Truck_dispatchRepository();
                var lstTruckOut = RepObj.GetTruckOutward();
                if (!lstTruckOut.Any())
                {
                    ViewBag.NoRecordMsg = "No data available !";
                }
                return PartialView("_TruckOutward", lstTruckOut);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Dispatch->VehicleSearchResults:", Ex);
                return null;
            }
        }

        //Save truck details when truck arrived on factory gate
        [HandleModelStateException]
        public ActionResult SaveArrivedVehicle(int id)
        {
            try
            {
                if (id == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Truck_dispatchRepository RepObj = new Truck_dispatchRepository();
                if (RepObj.SaveArrivedVehicle(id) == false)
                {
                    ViewBag.NoRecordMsg = "Record is not submitted successfully in our database.";
                }
                else
                {
                    string CurrentUserid = User.Identity.GetUserId();
                    var currentUserName = db.AspNetUsers.Where(p => p.Id == CurrentUserid).Select(x => x.name).SingleOrDefault();
                    Truck_dispatches truck_dispatches = db.Truck_dispatches.Find(id);
                    //AlertforTruckArrived(truck_dispatches.agent_id, currentUserName, truck_dispatches.truck_no);
                    //MsgAndMailforTruckArrived(truck_dispatches.agent_id, truck_dispatches.truck_no, currentUserName, "TruckArrived");
                }
                return View();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Dispatch->VehicleSearchResults:", Ex);
                return null;
            }
        }

        //send alert msg to agent when truck is arrived of this agent by gatekeeper
        private bool AlertforTruckArrived(int? agentid, string currentUserName, string truckNo)
        {
            try
            {
                string alertText = "Truck " + truckNo + "  has arrived at Gate";
                string alertSubject = "Truck " + truckNo + "  has arrived at Gate";

                //msgAlertObj.CreateAlertDetails("TruckArrived", "Agent", agentid, alertText, alertSubject, currentUserName, "");

                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in GateKeeper->AlertforTruckArrived:", Ex);
                return false;
            }
        }

        private bool MsgAndMailforTruckArrived(int? agentid, string truckNo, string CreatedBy, string status)
        {
            try
            {

                var agentMailAddress = db.Agents.Where(p => p.agent_id == agentid).SingleOrDefault();
                msgtxt = "Truck " + truckNo + "  has arrived at Gate";
                msgSubject = "Truck " + truckNo + "  has arrived at Gate";
                msgAction = "TruckArrived";
                recipient1 = ""; //agentMailAddress.email;
                cc1 = "";
                bcc1 = "";
                attachment1 = "";

                msgAlertObj.CreateMessagesDetails(msgAction, "Agent", agentid, msgtxt, msgSubject, "", "", "", CreatedBy, "", status, null, null);
                // msgAlertObj.SendMailWithDetails(msgAction, msgtxt, msgSubject, recipient1, cc1, bcc1, attachment1);
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in GateKeeper->MsgAndMailforTruckArrived:", Ex);
                return false;
            }
        }

        //Save truck details when truck left on factory
        [HandleModelStateException]
        public ActionResult SaveTruckOutward(int id)
        {
            try
            {
                if (id == 0)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Truck_dispatchRepository RepObj = new Truck_dispatchRepository();
                if (RepObj.SaveTruckOutward(id) == false)
                {
                    ViewBag.NoRecordMsg = "Record is not submitted successfully in our database.";
                }
                else
                {
                    string CurrentUserid = User.Identity.GetUserId();
                    var currentUserName = db.AspNetUsers.Where(p => p.Id == CurrentUserid).Select(x => x.name).SingleOrDefault();
                    Truck_dispatches truck_dispatches = db.Truck_dispatches.Find(id);
                    //AlertforTruckLeftOnGate(truck_dispatches.agent_id, currentUserName, truck_dispatches.truck_no);
                    //MsgAndMailforTruckLeftOnGate(truck_dispatches.agent_id, truck_dispatches.truck_no, currentUserName, "TruckDispatched");
                }
                return View();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Dispatch->VehicleSearchResults:", Ex);
                return null;
            }
        }

        //send alert to agent when truck is left on factory

        private bool AlertforTruckLeftOnGate(int? agentid, string currentUserName, string truckNo)
        {
            try
            {
                string alertText = "Truck " + truckNo + "  has left factory gate";
                string alertSubject = "Truck " + truckNo + "  has left factory gate";

                msgAlertObj.CreateAlertDetails("TruckDispatched", "Agent", agentid, alertText, alertSubject, currentUserName, null);
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in GateKeeper->AlertforTruckLeftOnGate:", Ex);
                return false;
            }
        }

        private bool MsgAndMailforTruckLeftOnGate(int? agentid, string truckNo, string CreatedBy, string status)
        {
            try
            {
                var agentMailAddress = db.Agents.Where(p => p.agent_id == agentid).SingleOrDefault();
                msgtxt = "Truck " + truckNo + "  has left factory gate";
                msgSubject = "Truck " + truckNo + "  has left factory gate";
                msgAction = "TruckDispatched";
                recipient1 = ""; //agentMailAddress.email;
                cc1 = "";
                bcc1 = "";
                attachment1 = "";

                msgAlertObj.CreateMessagesDetails(msgAction, "Agent", agentid, msgtxt, msgSubject, recipient1, cc1, bcc1, CreatedBy, status, "", null, null);
                //  msgAlertObj.SendMailWithDetails(msgAction, msgtxt, msgSubject, recipient1, cc1, bcc1, attachment1);

                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in GateKeeper->MsgAndMailforTruckLeftOnGate:", Ex);
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
