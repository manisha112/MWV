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
using System.Web.WebPages.Html;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using IdentitySample.Models;
using System.Web;
using System.IO;
using System.Text;
using MWV.Repository;

namespace MWV.Controllers.Admin
{
    public class AgentAdminController : Controller
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();
        private AgentAdminRepository objAgentRepo = new AgentAdminRepository();
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
        //
        // GET: /AgentAdmin/
        public ActionResult Index(int? page, string msg)
        {


            int pageSize = 5;
            int pageNumber = (page ?? 1);
            var query = db.Agents.OrderBy(k => k.name).ToList();
            ViewBag.AgentLst = query.ToList().ToPagedList(pageNumber, pageSize);
            ViewBag.ErrorMsg = msg;
            return View("Index", query);
        }
        //Agent Create Get 
        public ActionResult Create()
        {
            return PartialView("_Create");

        }

        //Agent Create Post Method Save Data in Agent Table
        [HttpPost]
        public ActionResult Create(Agent objAgent)
        {
            try
            {
                var result = (dynamic)0;
                string aspID = objAgentRepo.getAspnetId(objAgent.email);
                if (aspID != "")
                {
                    objAgent.aspnetusers_id = aspID;
                    result = objAgentRepo.SaveAgent(objAgent);
                }



                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentAdmin->Create->Post Method:", Ex);
                return null;
            }

        }

        [HttpGet]
        public PartialViewResult Edit(int id)
        {
            try
            {

                return PartialView("_Edit", objAgentRepo.lstAgent(id));
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentAdmin->Edit->GET Method:", Ex);
                return null;
            }
        }

        [HttpPost]
        public ActionResult Edit(Agent objagnt)
        {
            try
            {

                bool status = objAgentRepo.EditAgent(objagnt);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentAdmin->Edit->Post Method:", Ex);
                return null;
            }
        }

        //Add Agent in Aspnet Table For Create Login
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> AgentRoles(RegisterViewModel userViewModel)
        {
            try
            {
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email, firstName = userViewModel.firstName, lastName = userViewModel.lastName, name = userViewModel.name, mobile = userViewModel.mobile, address = userViewModel.address, location = userViewModel.location, city = userViewModel.city, state = userViewModel.state };


                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);
                if (adminresult.Succeeded)
                {
                    var result = await UserManager.AddToRolesAsync(user.Id, "Agent");
                    if (!result.Succeeded)
                    {

                        string Errormsg = result.Errors.First();
                        return Json(Errormsg, JsonRequestBehavior.AllowGet);
                    }


                }
                else
                {
                    string Errormsg = "";
                    if (adminresult.Errors.Count() > 1)
                    {
                        Errormsg = adminresult.Errors.ElementAt(1);
                    }
                    else
                    {
                        Errormsg = adminresult.Errors.ElementAt(0);
                    }

                    return Json(Errormsg.ToString(), JsonRequestBehavior.AllowGet);
                }

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerAdminController->CustomerRoles:", Ex);
                return null;
            }
        }

        //Edit Agent 
        [HttpPost]

        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> EditAgentLogin([Bind(Include = "Email,Id,name,papermill_list,firstName,lastName,mobile,landline,address,city,state,contactperson")] EditUserViewModel editUser, params string[] selectedRoles)
        {
            //selectedRoles[0] = "Agent";

            string aspnetID = objAgentRepo.getAspnetIdbyagentID(Convert.ToInt16(editUser.Id));
            var user = await UserManager.FindByIdAsync(aspnetID);
            if (user == null)
            {
                return HttpNotFound();
            }
            user.UserName = editUser.Email;
            user.firstName = editUser.firstName;
            user.lastName = editUser.lastName;
            user.name = editUser.name;
            user.Email = editUser.Email;
            user.mobile = editUser.mobile;
            user.landline = editUser.landline;
            user.address = editUser.address;
            user.city = editUser.city;
            user.state = editUser.state;
            user.location = editUser.location;
            user.contactperson = editUser.contactperson;
            var userRoles = await UserManager.GetRolesAsync(user.Id);
            selectedRoles = selectedRoles ?? new string[] { "Agent" };
            var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles.Except(userRoles).ToArray<string>());
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", result.Errors.First());
                string Errormsg = "";
                if (result.Errors.Count() > 1)
                {
                    Errormsg = result.Errors.ElementAt(1);
                }
                else
                {
                    Errormsg = result.Errors.ElementAt(0);
                    if (Errormsg == "Name user4@gmail.com is already taken.")
                    {
                        Errormsg = "Email user4@gmail.com is already taken.";
                    }
                }

                return Json(Errormsg.ToString(), JsonRequestBehavior.AllowGet);
            }
            result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRoles).ToArray<string>());
            if (!result.Succeeded)
            {
                string Errormsg = "";
                if (result.Errors.Count() > 1)
                {
                    Errormsg = result.Errors.ElementAt(1);
                }
                else
                {
                    Errormsg = result.Errors.ElementAt(0);
                    if (Errormsg == "Name user4@gmail.com is already taken.")
                    {
                        Errormsg = "Email user4@gmail.com is already taken.";
                    }
                }

                return Json(Errormsg.ToString(), JsonRequestBehavior.AllowGet);
            }

            // return RedirectToAction("Index", new { Message = ManageMessageId.EditSuccess });

            return Json(true, JsonRequestBehavior.AllowGet);
        }



        [HttpGet]
        public PartialViewResult Delete(int id)
        {
            try
            {

                return PartialView("_Delete", objAgentRepo.lstAgent(id));
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentAdmin->Delete->Get Method:", Ex);
                return null;
            }
        }

        //For Delete agent From Aspnet Table
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {

                string aspnetID = objAgentRepo.getAspnetIdbyagentID(id);
                if (ModelState.IsValid)
                {
                    if (aspnetID == null)
                    {
                        return Json("User Not Found!", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        var user = await UserManager.FindByIdAsync(aspnetID);
                        if (user == null)
                        {
                            return Json("User Not Found!", JsonRequestBehavior.AllowGet);
                        }
                        var query = db.Agents.FirstOrDefault(k => k.aspnetusers_id == aspnetID);
                        if (query != null)
                        {
                            db.Agents.Remove(query);
                            db.SaveChanges();

                        }
                        var queryaspnet = db.AspNetUsers.FirstOrDefault(p => p.Id == aspnetID);
                        if (queryaspnet != null)
                        {
                            db.AspNetUsers.Remove(queryaspnet);
                            db.SaveChanges();

                        }


                        return Json("User Deletion Success!", JsonRequestBehavior.AllowGet);
                        //return RedirectToAction("Index", new { Message = ManageMessageId.UserdeletionSucess });
                    }
                }
                return View();
            }
            catch (Exception)
            {

                return Json("User Deletion Failed!", JsonRequestBehavior.AllowGet);

            }
        }

        //Reset Password
        [HttpGet]
        public PartialViewResult resetPassword(int id)
        {
            try
            {
                ResetPasswordViewModel rs = new ResetPasswordViewModel();
                string aspnetID = objAgentRepo.getAspnetIdbyagentID(id);
                if (aspnetID != null)
                {

                    var query = (from ap in db.AspNetUsers where ap.Id == aspnetID select new { ap.Email, ap.name, ap.Id }).ToList();
                    foreach (var rese in query)
                    {
                        rs.Email = rese.Email;
                        rs.name = rese.name;
                        rs.id = rese.Id;
                    }
                }


                return PartialView("_ResetPassword", rs);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentAdmin->resetPassword->Get Method:", Ex);
                return null;
            }

        }

        [HttpPost]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var code = "";
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                int agntid = Convert.ToInt16(model.id);
                string aspnetID = objAgentRepo.getAspnetIdbyagentID(agntid);
                code = await UserManager.GeneratePasswordResetTokenAsync(aspnetID);
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return Json("User Not Found!", JsonRequestBehavior.AllowGet);
                }
                var result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
                if (result.Succeeded)
                {
                    return Json("Password Reset Success!", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ModelState.AddModelError("", result.Errors.First());
                    string Errormsg = "";
                    if (result.Errors.Count() > 1)
                    {
                        Errormsg = result.Errors.ElementAt(1);
                    }
                    else
                    {
                        Errormsg = result.Errors.ElementAt(0);
                    }
                    return Json(Errormsg.ToString(), JsonRequestBehavior.AllowGet);

                }

            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentAdmin->ResetPassword->POST Method:", Ex);
                return null;
            }

        }
    }

}