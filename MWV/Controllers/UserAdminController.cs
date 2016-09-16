using IdentitySample.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MWV.DBContext;
using MWV.Models;


namespace IdentitySample.Controllers
{

    public class UsersAdminController : Controller
    {
        private MWVDBContext db = new MWVDBContext();
        private ApplicationDbContext dbNew = new ApplicationDbContext();
        public UsersAdminController()
        {
        }
        public UsersAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
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

        private ApplicationRoleManager _roleManager;
        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        //[Authorize(Roles = "SuperAdmin")]
        // //GET: /Users/
        // [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            if (message != null)
            {

                ViewBag.StatusMessage =
                    message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed!"
                    : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set!"
                    : message == ManageMessageId.SetTwoFactorSuccess ? "Your two factor provider has been set!"
                    : message == ManageMessageId.Error ? "An error has occurred!"
                    : message == ManageMessageId.ResetPassUserNotFound ? "Reset Password Failed!"
                    : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed!"
                     : message == ManageMessageId.ChangePasswordFailed ? "Change Password Failed!"
                     : message == ManageMessageId.ResetPasswordSucess ? "Password Reset Success!"
                       : message == ManageMessageId.UserdeletionSucess ? "User Deletion Success!"
                         : message == ManageMessageId.UserDeletionfailed ? "Cannot Delete the User!"
                          : message == ManageMessageId.AdduserSucess ? "User added Sucessfully!"
                               : message == ManageMessageId.EditSuccess ? "Your Changes has been Saved!"

                    : "";
            }

            var users = new List<ApplicationUser>();//User to be skipped i.e agent and customer
            var otherUsers = new List<ApplicationUser>();//Get Other All User
            var role = await RoleManager.Roles.Select(k => k.Name).ToListAsync();

            foreach (var user in UserManager.Users.ToList())
            {
                if (await UserManager.IsInRoleAsync(user.Id, "Agent") || await UserManager.IsInRoleAsync(user.Id, "Customer"))
                {
                    users.Add(user);
                }
                else
                {
                    otherUsers.Add(user);

                }
            }
            return View(otherUsers);
        }



        // GET: /Users/Details/5
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);

            ViewBag.RoleNames = await UserManager.GetRolesAsync(user.Id);

            return View(user);
        }

        //
        // GET: /Users/Create
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> Create()
        {
            //get Location Distinic
            var lstLocation = (from pm in db.Papermills
                               group pm by new { pm.location } into grp
                               select grp.FirstOrDefault()).ToList();
            // show the list to select the papermill
            var lstPapermills = db.Papermills.ToList();
            ViewBag.Papermills_list = new SelectList(lstPapermills, "name", "name");
            // show the list to select the location
            ViewBag.Location_list = new SelectList(lstLocation, "location", "location");

            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        //
        // POST: /Users/Create
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            try
            {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email, firstName = userViewModel.firstName, lastName = userViewModel.lastName, mobile = userViewModel.mobile, landline = userViewModel.landline, address = userViewModel.address, AssignedMachine = userViewModel.AssignedMachine, location = userViewModel.location, city = userViewModel.city, state = userViewModel.state, contactperson = userViewModel.contactperson, name = userViewModel.name };
                if (selectedRoles[0] == "MachineHead")
                {
                    // save the assignment of papermill to the table 'AspNetUsers' table
                    userViewModel.AssignedMachine = Request["papermill_list"];
                    user.AssignedMachine = Request["papermill_list"];
                }
                if (selectedRoles[0] == "GateKeeper" || selectedRoles[0] == "Dispatch")
                {
                    // save the assignment of papermill to the table 'AspNetUsers' table
                    userViewModel.location = Request["Location_list"];
                    user.location = Request["Location_list"];
                }
                var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);

                //Add User to the selected Roles 
                if (adminresult.Succeeded)
                {
                    if (selectedRoles != null) // any one role is selected
                    {
                        var result = await UserManager.AddToRolesAsync(user.Id, selectedRoles);

                        //if (selectedRoles[0] == "Agent")
                        //{
                        //    // also add a record to the 'Agent' table in db

                        //    Agent objAgent = new Agent();
                        //    objAgent.name = user.name;
                        //    objAgent.email = userViewModel.Email;
                        //    objAgent.Mobile = userViewModel.mobile;
                        //    objAgent.landline = userViewModel.landline;
                        //    objAgent.address = userViewModel.address;
                        //    objAgent.aspnetusers_id = user.Id;
                        //    db.Agents.Add(objAgent);
                        //    db.SaveChanges();
                        //}

                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("", result.Errors.First());
                            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
                            return View();
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", adminresult.Errors.First());
                    ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");


                    return View();

                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AdduserSucess });
            }
            var lstPapermills = db.Papermills.ToList();
            ViewBag.Papermills_list = new SelectList(lstPapermills, "name", "name");
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");

            // show the list to select the location
            ViewBag.Location_list = new SelectList(lstPapermills, "location", "location");
            return View();

            }
            catch (Exception Ex)
            {
                return View();
            }

        }

        //
        // GET: /Users/Edit/1
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // show the list to select the papermill
            var lstPapermills = (from pm in db.Papermills
                                 group pm by new { pm.location } into grp
                                 select grp.FirstOrDefault()).ToList();
            var Papermills = db.Papermills.ToList();
            ViewBag.Papermills_list = new SelectList(Papermills, "name", "name");//, "name", "name"
            // show the list to select the location
            ViewBag.Location_list = new SelectList(lstPapermills, "location", "location");//, "location", "location"
            var query = (from p in db.AspNetUsers where p.Id.ToString() == id select new { p.location }).ToList();
            foreach (var loc in query)
            {
                string locat = loc.location;
                ViewBag.MillLoc = locat;
            }
            string ConPerson = String.Empty;
            var querypapermil = (from pm in db.AspNetUsers where pm.Id.ToString() == id select new { pm.AssignedMachine, pm.contactperson }).ToList();
            foreach (var am in querypapermil)
            {
                string macname = am.AssignedMachine;
                ViewBag.SelectMill = macname;
                ConPerson = am.contactperson;
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                firstName = user.firstName,
                lastName = user.lastName,
                name = user.name,
                address = user.address,
                landline = user.landline,
                location = user.location,
                mobile = user.mobile,
                Email = user.Email,
                city = user.city,
                state = user.state,
                contactperson = user.contactperson,


                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
        }

        //
        // POST: /Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id,name,papermill_list,firstName,lastName,mobile,landline,address,city,state,contactperson")] EditUserViewModel editUser, params string[] SelectedRole)
        {
            try
            {
            int agentid;


            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }
                //Get papermills
                var lstPapermills = (from pm in db.Papermills
                                     group pm by new { pm.location } into grp
                                     select grp.FirstOrDefault()).ToList();
                // Bind papermill Name
                var Papermills = db.Papermills.ToList();
                ViewBag.Papermills_list = new SelectList(Papermills, "name", "name");//, "name", "name"
                // show the list to select the location
                ViewBag.Location_list = new SelectList(lstPapermills, "location", "location");//, "location", "location"
                var query = (from p in db.AspNetUsers where p.Id.ToString() == editUser.Id select new { p.location }).ToList();
                foreach (var loc in query)
                {
                    string locat = loc.location;
                    ViewBag.MillLoc = locat;
                }
                string ConPerson = String.Empty;
                var querypapermil = (from pm in db.AspNetUsers where pm.Id.ToString() == editUser.Id select new { pm.AssignedMachine, pm.contactperson }).ToList();
                foreach (var am in querypapermil)
                {
                    string macname = am.AssignedMachine;
                    ViewBag.SelectMill = macname;
                    ConPerson = am.contactperson;
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


                SelectedRole = SelectedRole ?? new string[] { };
                var result = await UserManager.AddToRolesAsync(user.Id, SelectedRole.Except(userRoles).ToArray<string>());

                if (SelectedRole[0] == "MachineHead")
                {
                    // save the assignment of papermill to the table 'AspNetUsers' table
                    user.AssignedMachine = Request["papermill_list"];
                }
                if (SelectedRole[0] == "GateKeeper" || SelectedRole[0] == "Dispatch")
                {
                    // save the assignment of papermill to the table 'AspNetUsers' table
                    user.location = Request["Location_list"];
                }
                if (SelectedRole[0] == "Agent")
                {
                    // also add a record to the 'Agent' table in db
                    // MWVDBContext db = new MWVDBContext();
                    //agentid = (from ag in db.Agents join aspnet in db.AspNetUsers on ag.aspnetusers_id equals aspnet.Id where aspnet.Id == user.Id select ag.agent_id).SingleOrDefault();
                    //var query = db.Agents.FirstOrDefault(x => x.agent_id == agentid);
                    //if (query != null)
                    //{
                    //    query.name = user.name;
                    //    query.email = user.Email;
                    //    query.Mobile = user.mobile;
                    //    query.landline = user.landline;
                    //    query.address = user.address;
                    //    query.aspnetusers_id = user.Id;
                    //    db.SaveChanges();


                    //}

                }


                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View(new EditUserViewModel()
            {
                Id = user.Id,
                firstName = user.firstName,
                lastName = user.lastName,
                name = user.name,
                address = user.address,
                landline = user.landline,
                location = user.location,
                mobile = user.mobile,
                Email = user.Email,
                city = user.city,
                state = user.state,
                contactperson = user.contactperson,


                RolesList = RoleManager.Roles.ToList().Select(x => new SelectListItem()
                {
                    Selected = userRoles.Contains(x.Name),
                    Text = x.Name,
                    Value = x.Name
                })
            });
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(SelectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }

                return RedirectToAction("Index", new { Message = ManageMessageId.EditSuccess });
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
        }
            catch (Exception Ex)
            {
                return View();

            }

        }







        //
        // GET: /Users/Delete/5
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (id == null)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                    var user = await UserManager.FindByIdAsync(id);
                    if (user == null)
                    {
                        return HttpNotFound();
                    }
                    var query = db.Agents.FirstOrDefault(k => k.aspnetusers_id == id);
                    if (query != null)
                    {
                        db.Agents.Remove(query);
                        db.SaveChanges();

                    }
                    var queryaspnet = db.AspNetUsers.FirstOrDefault(p => p.Id == id);
                    if (queryaspnet != null)
                    {
                        db.AspNetUsers.Remove(queryaspnet);
                        db.SaveChanges();

                    }

                    // var result = await UserManager.DeleteAsync(user);
                    //if (!result.Succeeded)
                    //{
                    //    ModelState.AddModelError("", result.Errors.First());
                    //    return View();
                    //}
                    return RedirectToAction("Index", new { Message = ManageMessageId.UserdeletionSucess });

                }
                return View();
            }
            catch (Exception)
            {

                return RedirectToAction("Index", new { Message = ManageMessageId.UserDeletionfailed });

            }
        }


        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var chkOldpass = await UserManager.FindAsync(User.Identity.Name, model.OldPassword);
            if (chkOldpass != null)
            {
                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        //await SignInAsync(user, isPersistent: false);
                    }
                    return RedirectToAction("AdminHome", "Home", new { Message = ManageMessageId.ChangePasswordSuccess });
                }

                ModelState.AddModelError("", result.Errors.First());
                //return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordFailed });
            }
            else
            {
                ViewBag.IncorrectPass = "Incorrect Old Password!";
                return View();
            }
            return View(model);
        }


        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            ResetPasswordViewModel rs = new ResetPasswordViewModel();
            if (code != null)
            {

                var query = (from ap in db.AspNetUsers where ap.Id == code select new { ap.Email, ap.name, ap.Id }).ToList();
                foreach (var rese in query)
                {
                    rs.Email = rese.Email;
                    rs.name = rese.name;
                    rs.id = rese.Id;
                }
            }
            return code == null ? View("Error") : View(rs);
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var code = await UserManager.GeneratePasswordResetTokenAsync(model.id);
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("Index", new { Message = ManageMessageId.ResetPassUserNotFound });
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.ResetPasswordSucess });
            }
            ModelState.AddModelError("", result.Errors.First());
            //AddErrors(result);
            return View(model);
        }


        public enum ManageMessageId
        {
            AddPhoneSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error,
            ChangePasswordFailed,
            ResetPassUserNotFound,
            ResetPasswordSucess,
            UserdeletionSucess,
            UserDeletionfailed,
            AdduserSucess,
            EditSuccess

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
