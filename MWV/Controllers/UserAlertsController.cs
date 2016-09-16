using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWV.Models;
using MWV.DBContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using IdentitySample.Models;
using PagedList;
using PagedList.Mvc;
using MWV.Repository.Implementation;
using System.IO;
using System.Text;
using System.Reflection;


namespace MWV.Controllers
{
    public class UserAlertsController : Controller
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
        // GET: /UserAlerts/

        //Show All alerts to user
        [HandleModelStateException]
        public ActionResult ShowAleartMsg(int? page)
        {
            try
            {
                string alphabet = string.Empty;
                int pageSize = 100;
                int pageNumber = (page ?? 1);
                string id = User.Identity.GetUserId();
                if (id != null || id == "")
                {
                    var RoleNames = UserManager.GetRoles(id);
                    string rolename = RoleNames[0];
                    ViewBag.Rollname = rolename;
                    AlertsMessagesRepository obj = new AlertsMessagesRepository();
                    var result = obj.ShowAlerts(rolename, id);
                    if (result.Count == 0)
                    {
                        ViewBag.Errormsg = "No data available !";
                        ViewBag.alertLst = result.ToPagedList(pageNumber, pageSize);
                    }
                    else
                    {
                        ViewBag.alertLst = result.ToPagedList(pageNumber, pageSize);
                    }
                    return PartialView("_UserAlerts");
                }
                return null;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in UseralertsController->ShowAleartMsg", Ex);
                return null;

            }

        }
        //Delete alerts 
        [HandleModelStateException]
        public ActionResult DeleteItems()
        {
            try
            {
                int selectedCustomerid = Convert.ToInt16(Request["selectedCustomerid"]);
                AlertsMessagesRepository obj = new AlertsMessagesRepository();
                obj.deleteAlerts(selectedCustomerid);
                return View();

            }
            catch (Exception Ex)
            {
                logger.Error("Error in UseralertsController->DeleteItems", Ex);
                return null;
            }

        }
        //Show Message To User
        [HandleModelStateException]
        public ActionResult ShowUsermsgs(int? page)
        {
            try
            {
                string alphabet = string.Empty;
                int pageSize = 100;
                int pageNumber = (page ?? 1);
                string id = User.Identity.GetUserId();
                if (id != null || id == "")
                {
                    var RoleNames = UserManager.GetRoles(id);
                    string rolename = RoleNames[0];
                    ViewBag.Rollname = rolename;
                    AlertsMessagesRepository obj = new AlertsMessagesRepository();
                    var result = obj.showMessages(rolename, id);
                    if (result.Count == 0)
                    {
                        ViewBag.Errormsg = "No data available !";
                        ViewBag.MsgLst = result.ToPagedList(pageNumber, pageSize);
                    }
                    else
                    {
                        ViewBag.MsgLst = result.ToPagedList(pageNumber, pageSize);
                    }
                    return PartialView("_UserMessages");
                }
                return null;

            }
            catch (Exception Ex)
            {
                logger.Error("Error in UseralertsController->ShowUsermsgs", Ex);
                return null;
            }

        }
        //delete user messages
        [HandleModelStateException]
        public ActionResult deleteMessages()
        {
            try
            {
                int selectedCustomerid = Convert.ToInt16(Request["selectedCustomerid"]);
                AlertsMessagesRepository obj = new AlertsMessagesRepository();
                obj.deleteMessages(selectedCustomerid);
                return View();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in UseralertsController->deleteMessages", Ex);
                return null;
            }

        }
        //Download Messages order pdf file 
        [HandleModelStateException]
        public ActionResult DownloadFiles(string custId, string type)
        {
            try
            {
                string attachment;
                if (type == "1")
                {
                    attachment = (from ms in db.Messages where ms.message_id.ToString() == custId select ms.attachment1).SingleOrDefault();
                    var pathFile = Path.Combine(Server.MapPath("~/"), attachment);
                    string subStr = attachment.Substring(9);
                    return File(pathFile, "application/pdf", subStr);
                }
                else
                {
                    attachment = (from ms in db.Messages where ms.message_id.ToString() == custId select ms.attachment2).SingleOrDefault();
                    var pathFile = Path.Combine(Server.MapPath("~/"), attachment);
                    string subStr = attachment.Substring(9);
                    return File(pathFile, "application/pdf", subStr);

                }
            }
            catch (Exception Ex)
            {
                logger.Error("Error in UseralertsController->DownloadFiles", Ex);
                return null;
            }



        }
        //Changed message status to unread to read
        [HandleModelStateException]
        public ActionResult UpdateToReadMsg()
        {
            try
            {
                Int16 id = Convert.ToInt16(Request["msgid"]);
                AlertsMessagesRepository objUpdate = new AlertsMessagesRepository();
                objUpdate.UpdateReadStatusMsg(id);
                return View();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in UseralertsController->UpdateToReadMsg", Ex);
                return null;
            }


        }
        //Changed alerts status unread to read
        [HandleModelStateException]
        public ActionResult UpdateToReadAlerts()
        {
            try
            {
                Int16 id = Convert.ToInt16(Request["msgid"]);
                AlertsMessagesRepository objUpdate = new AlertsMessagesRepository();
                objUpdate.UpdateReadStatusAlert(id);
                return View();

            }
            catch (Exception Ex)
            {
                logger.Error("Error in UseralertsController->UpdateToReadAlerts", Ex);
                return null;
            }

        }
        //retrive image for new messgae 
        [HandleModelStateException]
        public ActionResult SetAlertIntervalsAlerts()
        {
            try
            {
                Int64 Count = 0; string imgpath = string.Empty;
                OrderRepository objOrderRep = new OrderRepository();

                string id = User.Identity.GetUserId();
                if (id != null || id == "")
                {
                    var RoleNames = UserManager.GetRoles(id);
                    string rollname = RoleNames[0];
                    int LoggedInAgent_id = objOrderRep.GetAgentID();

                    if (LoggedInAgent_id != 0)
                    {
                        Count = (from a in db.Alerts where a.alert_for_role == "Agent" && a.alert_for_agentid == LoggedInAgent_id && a.viewed == 1 select new { }).Count();
                        if (Count > 0)
                        { imgpath = "/images/alarm-on.png"; }
                        else { imgpath = "/images/alarm-off.png"; }
                    }
                    else if (id != null)
                    {
                        if (rollname == "MachineHead")
                        {
                            Count = (from a in db.Alerts where a.alert_for_role == "MachineHead" && a.viewed == 1 && a.machinehead_aspnetuserid == id select new { }).Count();
                            if (Count > 0) { imgpath = "/images/alarm-on.png"; }
                            else { imgpath = "/images/alarm-off.png"; }
                        }
                        else
                        {
                            Count = (from a in db.Alerts where a.alert_for_role == rollname && a.viewed == 1 select new { }).Count();
                            if (Count > 0) { imgpath = "/images/alarm-on.png"; }
                            else { imgpath = "/images/alarm-off.png"; }
                        }
                    }
                    return Json(imgpath, JsonRequestBehavior.AllowGet);
                }
                return null;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in UseralertsController->SetAlertIntervalsAlerts", Ex);
                return null;

            }



        }
        //retrive image for new alerts
        [HandleModelStateException]
        public ActionResult SetAlertIntervalsMessages()
        {
            try
            {
                Int64 Count = 0; string imgpath = string.Empty;
                OrderRepository objOrderRep = new OrderRepository();
                int LoggedInAgent_id = objOrderRep.GetAgentID();
                string id = User.Identity.GetUserId();
                if (id != null || id == "")
                {
                    var RoleNames = UserManager.GetRoles(id);
                    string rollname = RoleNames[0];
                    if (LoggedInAgent_id != 0)
                    {
                        Count = (from a in db.Messages where a.message_for_role == "Agent" && a.message_for_agentid == LoggedInAgent_id && a.viewed == 1 select new { }).Count();
                        if (Count > 0)
                        {
                            imgpath = "/images/mail-on.png";
                        }
                        else
                        {
                            imgpath = "/images/mail-off.png";
                        }
                    }
                    else if (id != null)
                    {
                        if (rollname == "MachineHead")
                        {
                            Count = (from a in db.Messages where a.message_for_role == "MachineHead" && a.viewed == 1 && a.machinehead_aspnetuserid == id select new { }).Count();
                            if (Count > 0)
                            {
                                imgpath = "/images/mail-on.png";
                            }
                            else
                            {
                                imgpath = "/images/mail-off.png";
                            }
                        }
                        else
                        {
                            Count = (from a in db.Messages where a.message_for_role == rollname && a.viewed == 1 select new { }).Count();
                            if (Count > 0)
                            {
                                imgpath = "/images/mail-on.png";
                            }
                            else
                            {
                                imgpath = "/images/mail-off.png";
                            }

                        }

                    }
                    return Json(imgpath, JsonRequestBehavior.AllowGet);
                }

                return null;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in UseralertsController->SetAlertIntervalsMessages", Ex);
                return null;

            }
        }
        //Save user feedback in order table
        [HandleModelStateException]
        public ActionResult SaveFeedback()
        {
            try
            {
                OrderRepository objOrderRep = new OrderRepository();
                AlertsMessagesRepository objam = new AlertsMessagesRepository();
                ProductionPlannerRepository objpr = new ProductionPlannerRepository();
                string id = User.Identity.GetUserId();
                if (id != null || id == "")
                {
                    var RoleNames = UserManager.GetRoles(id);
                    int LoggedInAgent_id = objOrderRep.GetAgentID();
                    string rollname = RoleNames[0];
                    int ordid = Convert.ToInt16(Request["orderid"]);
                    string feedback = Request["feedback"];
                    int ratings = Convert.ToInt16(Request["ratingcount"]);
                    DateTime dtCurrent = DateTime.Now;
                    if (rollname == "Agent")
                    {
                        string agentname = objOrderRep.GetAgentName();
                        objam.savefeedbacktodb(feedback, ratings, dtCurrent, agentname, ordid);
                    }
                    else
                    {
                        string ratedby = (from asp in db.AspNetUsers where asp.Id == id.ToString() select asp.name).SingleOrDefault();
                        objam.savefeedbacktodb(feedback, ratings, dtCurrent, ratedby, ordid);
                    }
                    return View();
                }
                return null;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in UseralertsController->SaveFeedback", Ex);
                return null;
            }
        }


    }
}
