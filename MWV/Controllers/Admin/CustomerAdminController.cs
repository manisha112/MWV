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
using MWV.ViewModels;
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
    public class CustomerAdminController : Controller
    {

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();
        private ApplicationDbContext dbNew = new ApplicationDbContext();
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
        // GET: CustomerAdmin
        public ActionResult Index(int? page, string msg)
        {
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            List<Customer> lstCustomer;
            CustomerAdminRepository objCust = new CustomerAdminRepository();
            lstCustomer = objCust.GetCustomers();
            //  return View(lstCustomer.OrderBy(x => x.name).ToList());//sort by cust name
            ViewBag.PageData = lstCustomer.OrderBy(x => x.name).ToList().ToPagedList(pageNumber, pageSize);
            ViewBag.Message = msg; //use this viewbag msg for add/edit/delete msg on index page. bydefault it is null.
            return View("Index");
        }

        //Create Customer Get Method
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                CustomerAdminRepository objCust = new CustomerAdminRepository();
                var lstAgent = objCust.GetAgents();
                ViewBag.Agent_list = new SelectList(lstAgent, "agent_id", "name");

                return View("Create");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerAdminController->Create:", Ex);
                return null;
            }

        }

        [HttpPost]
        public ActionResult Create(Customer ObjGetCust)
        {
            try
            {
                AgentAdminRepository objAgentRepo = new AgentAdminRepository();
                CustomerAdminRepository objCust = new CustomerAdminRepository();
                string aspID = objAgentRepo.getAspnetId(ObjGetCust.email);
                ObjGetCust.aspnetuser_id = aspID;
                var result = objCust.CreateCustomer(ObjGetCust);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerAdminController->Create:", Ex);
                return null;
            }

        }



        // GET: CustomerAdmin/Details/5
        public ActionResult Details(int id)
        {
            List<Customer> lstCustomer;
            CustomerAdminRepository objCust = new CustomerAdminRepository();
            lstCustomer = objCust.GetCustomers();
            var cust = lstCustomer.Find(x => x.customer_id == id);
            int agntId = lstCustomer.Where(k => k.customer_id == id).Select(f => f.agent_id).SingleOrDefault();
            ViewBag.agntid = agntId;
            return View("Edit", cust);
        }






        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public ActionResult DeleteDetails(int id)
        {
            List<Customer> lstCustomer;
            CustomerAdminRepository objCust = new CustomerAdminRepository();
            lstCustomer = objCust.GetCustomers();
            var cust = lstCustomer.Find(x => x.customer_id == id);
            return View("Delete", cust);
        }


        // [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool EditCustomer()
        {
            try
            {
                bool isEdited;
                string id = User.Identity.GetUserId();
                string modified_by;
                modified_by = (from a in db.AspNetUsers where a.Id == id select a.name).SingleOrDefault();
                CustomerAdminRepository objCust = new CustomerAdminRepository();


                int CustomerIDToEdit = Convert.ToInt32(Request["CustomerIDToEdit"]);
                string name = Request["name"].Trim();
                string email = Request["email"].Trim();
                string phone = Request["phone"].Trim();
                string address1 = Request["address1"].Trim();
                string address2 = Request["address2"].Trim();
                string address3 = Request["address3"].Trim();
                string city = Request["city"].Trim();
                string pincode = Request["pincode"].Trim();
                string state = Request["state"].Trim();
                string country = Request["country"].Trim();
                string fax = Request["fax"].Trim();
                //string status = Request["status"].Trim();
                //  string remarks = Request["remarks"].Trim();

                string firstname = Request["firstname"].Trim();
                int agent_id = Convert.ToInt32(Request["agent_id"]);
                string lastname = Request["firstname"].Trim();

                isEdited = objCust.EditCustomer(CustomerIDToEdit, name, email, phone, address1, address2, address3, city, pincode, state, country, fax, "", modified_by, firstname, lastname, agent_id);
                return isEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerAdminController->EditCustomer:", Ex);
                return false;
            }
        }


        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public bool DeleteCustomer()
        {
            try
            {
                bool isDeleted;
                int CustomerIDToDel = Convert.ToInt32(Request["CustomerIDToDel"]);
                CustomerAdminRepository objCust = new CustomerAdminRepository();
                isDeleted = objCust.DeleteCustomer(CustomerIDToDel);
                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerAdminController->DeleteCustomer:", Ex);
                return false;
            }

        }



        // GET: CustomerAdmin/Edit/5
        public ActionResult Edit(int id)
        {
            List<Customer> lstCustomer;
            CustomerAdminRepository objCust = new CustomerAdminRepository();
            var lstAgent = objCust.GetAgents();
            ViewBag.Agent_list = new SelectList(lstAgent, "agent_id", "name");
            lstCustomer = objCust.GetCustomers();
            var cust = lstCustomer.Find(x => x.customer_id == id);
            int agntId = lstCustomer.Where(k => k.customer_id == id).Select(f => f.agent_id).SingleOrDefault();
            ViewBag.agntid = agntId;
            return View("Edit", cust);
        }

        // POST: CustomerAdmin/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CustomerAdmin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CustomerAdmin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        //Assingn And Remove roles and login to customer
        [HttpPost]
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> CustomerRoles(RegisterViewModel userViewModel)
        {
            try
            {
                var user = new ApplicationUser { UserName = userViewModel.Email, Email = userViewModel.Email, firstName = userViewModel.firstName, lastName = userViewModel.lastName, mobile = userViewModel.mobile, address = userViewModel.address, location = userViewModel.location, city = userViewModel.city, state = userViewModel.state };

                var CheckIncust = db.Customers.Where(k => k.customer_id.ToString() == user.location && k.login_enabled == 1).SingleOrDefault();
                if (CheckIncust != null)
                {
                    var queryAspnet = db.AspNetUsers.Where(x => x.Id == CheckIncust.aspnetuser_id).SingleOrDefault();
                    if (queryAspnet != null)
                    {
                        queryAspnet.UserName = userViewModel.Email;
                        db.SaveChanges();
                        CheckIncust.login_enabled = 0;
                        db.SaveChanges();
                    }

                }

                else
                {
                    var adminresult = await UserManager.CreateAsync(user, userViewModel.Password);
                    if (adminresult.Succeeded)
                    {


                        var result = await UserManager.AddToRolesAsync(user.Id, "Customer");
                        // also Update a record to the 'Customer' table in db
                        var query = db.Customers.Where(x => x.customer_id.ToString() == user.location).FirstOrDefault();//assume Location As Customer Id
                        if (query != null)
                        {
                            query.aspnetuser_id = user.Id;
                            query.email = user.Email;
                            query.login_enabled = 0;
                            db.SaveChanges();
                        }
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
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerAdminController->CustomerRoles:", Ex);
                return null;
            }
        }
        //Disable Customer Login
        public ActionResult RemoveCustomerLogin()
        {
            try
            {
                int CustomerID = Convert.ToInt32(Request["CustomerIDToEdit"]);
                string aspnetid = (from cm in db.Customers where cm.customer_id == CustomerID select cm.aspnetuser_id).FirstOrDefault();
                var query = db.AspNetUsers.Where(k => k.Id == aspnetid).SingleOrDefault();
                if (query != null)
                {
                    query.UserName = string.Empty;
                    db.SaveChanges();
                }
                //Update status in Customer Table
                var queryStatus = db.Customers.Where(l => l.customer_id == CustomerID).SingleOrDefault();
                if (queryStatus != null)
                {
                    queryStatus.login_enabled = 1;
                    db.SaveChanges();
                }

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerAdminController->RemoveCustomerLogin:", Ex);
                return null;
            }
        }

        //Remove Customer From Roles
        [Authorize(Roles = "SuperAdmin, MWVAdmin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                CustomerAdminRepository objAgentRepo = new CustomerAdminRepository();
                string aspnetID = objAgentRepo.getAspnetIdbyCustID(id);
                if (aspnetID == "" || aspnetID == null)
                {
                    var query = db.Customers.FirstOrDefault(k => k.aspnetuser_id == aspnetID);
                    if (query != null)
                    {
                        db.Customers.Remove(query);
                        db.SaveChanges();
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }

                }
                else
                {
                    if (ModelState.IsValid)
                    {
                        if (aspnetID == null)
                        {
                            return Json(false, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var user = await UserManager.FindByIdAsync(aspnetID);
                            if (user == null)
                            {
                                return Json(false, JsonRequestBehavior.AllowGet);
                            }
                            var query = db.Customers.FirstOrDefault(k => k.aspnetuser_id == aspnetID);
                            if (query != null)
                            {
                                db.Customers.Remove(query);
                                db.SaveChanges();

                            }
                            else
                            {
                                return Json(false, JsonRequestBehavior.AllowGet);
                            }
                            var queryaspnet = db.AspNetUsers.FirstOrDefault(p => p.Id == aspnetID);
                            if (queryaspnet != null)
                            {
                                db.AspNetUsers.Remove(queryaspnet);
                                db.SaveChanges();

                            }

                        }


                        return Json(true, JsonRequestBehavior.AllowGet);
                        //return RedirectToAction("Index", new { Message = ManageMessageId.UserdeletionSucess });

                    }
                    return Json(false, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception)
            {

                return Json(false, JsonRequestBehavior.AllowGet);

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
