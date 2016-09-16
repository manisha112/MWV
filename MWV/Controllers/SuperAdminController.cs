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
    public class SuperAdminController : Controller
    {
        private MWVDBContext db = new MWVDBContext();

        public SuperAdminController()
        {
        }

        public SuperAdminController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
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
        // GET: /Users/
        // [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> Index()
        {
            return View(await UserManager.Users.ToListAsync());
        }

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
            // show the list to select the papermill
            var lstPapermills = db.Papermills.ToList();
            ViewBag.Papermills_list = new SelectList(lstPapermills, "name", "name");
            // show the list to select the location
            ViewBag.Location_list = new SelectList(lstPapermills, "location", "location");

            //Get the list of Roles
            ViewBag.RoleId = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> Create(RegisterViewModel userViewModel, params string[] selectedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email, firstName = userViewModel.firstName, lastName = userViewModel.lastName, mobile = userViewModel.mobile, landline = userViewModel.landline, address = userViewModel.address, AssignedMachine = userViewModel.AssignedMachine, location = userViewModel.location };
                if (selectedRoles[0] == "MachineHead")
                {
                    // save the assignment of papermill to the table 'AspNetUsers' table
                    userViewModel.AssignedMachine = Request["papermill_list"];
                    user.AssignedMachine = Request["papermill_list"];
                }
                if (selectedRoles[0] == "GateKeeper")
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

                        if (selectedRoles[0] == "Agent")
                        {
                            // also add a record to the 'Agent' table in db

                            Agent objAgent = new Agent();
                            objAgent.name = userViewModel.firstName + " " + userViewModel.lastName;
                            objAgent.email = userViewModel.Email;
                            objAgent.mobile = userViewModel.mobile;
                            objAgent.landline = userViewModel.landline;
                            objAgent.address = userViewModel.address;
                            objAgent.aspnetusers_id = user.Id;
                            db.Agents.Add(objAgent);
                            db.SaveChanges();
                        }

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
                return RedirectToAction("Index");
            }
            ViewBag.RoleId = new SelectList(RoleManager.Roles, "Name", "Name");
            return View();
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
            var lstPapermills = db.Papermills.ToList();
            ViewBag.Papermills_list = new SelectList(lstPapermills, "name", "name");
            // show the list to select the location
            ViewBag.Location_list = new SelectList(lstPapermills, "location", "location");


            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userRoles = await UserManager.GetRolesAsync(user.Id);

            return View(new EditUserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
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
        public async Task<ActionResult> Edit([Bind(Include = "Email,Id,papermill_list")] EditUserViewModel editUser, params string[] selectedRole)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByIdAsync(editUser.Id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                user.UserName = editUser.Email;
                user.Email = editUser.Email;
                if (selectedRole[0] == "MachineHead")
                {
                    // save the assignment of papermill to the table 'AspNetUsers' table
                    user.AssignedMachine = Request["papermill_list"];
                }
                if (selectedRole[0] == "GateKeeper")
                {
                    // save the assignment of papermill to the table 'AspNetUsers' table
                    user.location = Request["Location_list"];
                }


                var userRoles = await UserManager.GetRolesAsync(user.Id);

                selectedRole = selectedRole ?? new string[] { };

                var result = await UserManager.AddToRolesAsync(user.Id, selectedRole.Except(userRoles).ToArray<string>());
                if (selectedRole[0] == "Agent")
                {
                    // also add a record to the 'Agent' table in db
                    MWVDBContext db = new MWVDBContext();
                    Agent objAgent = new Agent();
                    objAgent.name = user.firstName + " " + user.lastName;
                    objAgent.email = user.Email;
                    objAgent.mobile = user.mobile;
                    objAgent.landline = user.landline;
                    objAgent.address = user.address;
                    objAgent.aspnetusers_id = user.Id;
                    db.Agents.Add(objAgent);
                    db.SaveChanges();
                }

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                result = await UserManager.RemoveFromRolesAsync(user.Id, userRoles.Except(selectedRole).ToArray<string>());

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Something failed.");
            return View();
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
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }
                return RedirectToAction("Index");
            }
            return View();
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