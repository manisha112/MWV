using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using MWV.Models;
using MWV.DBContext;
using MWV.Repository.Implementation;
using Microsoft.AspNet.Identity;
using PagedList;
using PagedList.Mvc;

namespace MWV.Controllers
{
    public class Truck_dispatchController : Controller
    {
        private MWVDBContext db = new MWVDBContext();
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

        // GET: /Truck_dispatch/
        [HandleModelStateException]
        public ActionResult Index()
        {
            return View(db.Truck_dispatches.ToList());
        }
        [HandleModelStateException]
        public void RemoveVehicle(int id)
        {
            Truck_dispatchRepository tdsObj = new Truck_dispatchRepository();
            tdsObj.DeleteVehicles(id);
        }

        // GET: /Truck_dispatch/Details/5
        [HandleModelStateException]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Truck_dispatches truck_dispatches = db.Truck_dispatches.Find(id);

            if (truck_dispatches == null)
            {
                return HttpNotFound();
            }
            return View(truck_dispatches);
        }


        // GET: /Truck_dispatch/Create
        [HandleModelStateException]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Truck_dispatch/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "truck_dispatch_id,agent_id,truck_no,estimated_arrival,estimated_capacity,actual_arrival_at_gate,agent_dispatched_on,status,left_factory_on,loaded_capacity,created_on,created_by,modified_on,modified_by")] Truck_dispatches truck_dispatches)
        {
            if (ModelState.IsValid)
            {
                db.Truck_dispatches.Add(truck_dispatches);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(truck_dispatches);
        }

        // GET: /Truck_dispatch/Edit/5
        [HandleModelStateException]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Truck_dispatches truck_dispatches = db.Truck_dispatches.Find(id);
            if (truck_dispatches == null)
            {
                return HttpNotFound();
            }
            return View(truck_dispatches);
        }

        // POST: /Truck_dispatch/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "truck_dispatch_id,agent_id,truck_no,estimated_arrival,estimated_capacity,actual_arrival_at_gate,agent_dispatched_on,status,left_factory_on,loaded_capacity,created_on,created_by,modified_on,modified_by")] Truck_dispatches truck_dispatches)
        {
            if (ModelState.IsValid)
            {
                db.Entry(truck_dispatches).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(truck_dispatches);
        }

        // GET: /Truck_dispatch/Delete/5
        [HandleModelStateException]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Truck_dispatches truck_dispatches = db.Truck_dispatches.Find(id);
            if (truck_dispatches == null)
            {
                return HttpNotFound();
            }
            return View(truck_dispatches);
        }

        // POST: /Truck_dispatch/Delete/5
        [HandleModelStateException]
        [HttpPost, ActionName("Delete")]

        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Truck_dispatches truck_dispatches = db.Truck_dispatches.Find(id);
            db.Truck_dispatches.Remove(truck_dispatches);
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
        /// <summary>
        /// Functions created by Bhoo Software Pvt. Ltd. developers
        /// </summary>
        [HandleModelStateException]
        public PartialViewResult searchResultTransportation(int? page)
        {
            DateTime fromdt = new DateTime();
            DateTime todt = new DateTime();
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            string selectedVehicleValue = "";
            if (Session["fromDate"] == null && Session["toDate"] == null && Session["selectVehicleTypeddl"] == null)
            {
                Session["fromDate"] = Convert.ToDateTime(Request["FromDateTime"]);
                Session["toDate"] = Convert.ToDateTime(Request["ToDateTime"]);
                string strVehicleType = Request["selectVehicleTypeddl"];
                Session["selectVehicleTypeddl"] = strVehicleType;
                fromdt = Convert.ToDateTime(Session["fromDate"]);
                todt = Convert.ToDateTime(Session["toDate"]);
                selectedVehicleValue = Session["selectVehicleTypeddl"].ToString();
            }
            else
            {
                fromdt = Convert.ToDateTime(Session["fromDate"]);
                todt = Convert.ToDateTime(Session["toDate"]);
                selectedVehicleValue = Session["selectVehicleTypeddl"].ToString();
            }

            List<Truck_dispatches> TruckDetailsList = new List<Truck_dispatches>();
            Truck_dispatchRepository TruckDisObj = new Truck_dispatchRepository();

            if (selectedVehicleValue == "Vehicle-Number")
            {
                string vehicleNumber = Request["vehicleNumber"];
                TruckDetailsList = TruckDisObj.searchResultTransportation(selectedVehicleValue, vehicleNumber);
            }
            else
            {
                TruckDetailsList = TruckDisObj.searchResultTransportation(selectedVehicleValue, fromdt, todt);
            }

            if (!TruckDetailsList.Any())
            {
                ViewBag.noTruckCroMsg = "No data available !";
            }
            ViewBag.TruckDetailsList = TruckDetailsList.ToPagedList(pageNumber, pageSize);
            ViewBag.Pagesize = TruckDetailsList.Count;
            return PartialView("_searchResultTransportation");
        }

        public void ClearSessionOnVehiclesSearch()
        {
            Session["fromDate"] = null;
            Session["toDate"] = null;
            Session["selectVehicleTypeddl"] = null;
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [HandleModelStateException]
        public JsonResult GetProductsByOrderId(int? id)
        {

            var prodList = (from Order_Products op in db.Order_products
                            join Order in db.Orders on op.order_id equals Order.order_id
                            where op.order_id == id
                            select op).ToList();
            var result = (from s in prodList
                          select new
                          {
                              id = s.order_product_id,
                              name = s.product_code
                          }).ToList();

            string message = string.Format("Products of selected order are: ");

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HandleModelStateException]
        public PartialViewResult TruckInward()
        {
            var lstTruckIn = db.Truck_dispatches.Where(p => p.estimated_arrival.Value.Day == DateTime.Now.Day
                                                        && p.estimated_arrival.Value.Month == DateTime.Now.Month
                                                        && p.estimated_arrival.Value.Year == DateTime.Now.Year
            ).ToList();
            return PartialView("_TruckInward", lstTruckIn);
        }
        [HandleModelStateException]
        public ActionResult EditTruckInward(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Truck_dispatches truck_dispatches = db.Truck_dispatches.Find(id);
            if (truck_dispatches == null)
            {
                return HttpNotFound();
            }

            truck_dispatches.actual_arrival_at_gate = DateTime.Now;
            return View("EditTruckInward", truck_dispatches);
        }

        [HttpPost]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult EditTruckInward([Bind(Include = "truck_dispatch_id,agent_id,truck_no,estimated_arrival,estimated_capacity,actual_arrival_at_gate,agent_dispatched_on,status,left_factory_on,loaded_capacity,created_on,created_by,modified_on,modified_by")] Truck_dispatches truck_dispatches)
        {
            Truck_dispatches td = db.Truck_dispatches.Find(truck_dispatches.truck_dispatch_id);
            //var td = db.Truck_dispatches.Single(p=> p.truck_dispatch_id ==truck_dispatches.truck_dispatch_id);

            //var td = db.Truck_dispatches.Where(p=> p.truck_dispatch_id == truck_dispatches.truck_dispatch_id);
            //td.ToList()[0].actual_arrival_at_gate = Convert.ToDateTime(Request["actual_arrival_at_gate"]);

            td.actual_arrival_at_gate = Convert.ToDateTime(Request["actual_arrival_at_gate"]);

            db.SaveChanges();

            return View("TruckInward", new List<Truck_dispatches> { td });
        }
        [HandleModelStateException]
        public int GetAgentID()
        {
            string id = User.Identity.GetUserId();
            // get the agent_id from 'Agents' table with this user id
            int Agentid = (from a in db.Agents where a.aspnetusers_id == id select a.agent_id).SingleOrDefault();
            return Agentid;
        }
        [HandleModelStateException]
        public PartialViewResult TruckOutward()
        {
            //  var lstTruckOut = (from d in db.Truck_dispatches select d).ToList();

            var lstTruckOut = db.Truck_dispatches.Where(p => p.estimated_arrival.Value.Day == DateTime.Now.Day
                                                                    && p.estimated_arrival.Value.Month == DateTime.Now.Month
                                                                    && p.estimated_arrival.Value.Year == DateTime.Now.Year
                                                                    && p.left_factory_on == null
            ).ToList();
            return PartialView("_TruckOutward", lstTruckOut);
        }
        [HandleModelStateException]
        public ActionResult EditTruckOutward(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Truck_dispatches truck_dispatches = db.Truck_dispatches.Find(id);
            if (truck_dispatches == null)
            {
                return HttpNotFound();
            }

            truck_dispatches.left_factory_on = DateTime.Now;
            return View("EditTruckOutward", truck_dispatches);
        }

        [HttpPost]
        [HandleModelStateException]
        [ValidateAntiForgeryToken]
        public ActionResult EditTruckOutward([Bind(Include = "truck_dispatch_id,agent_id,truck_no,estimated_arrival,estimated_capacity,actual_arrival_at_gate,agent_dispatched_on,status,left_factory_on,loaded_capacity,created_on,created_by,modified_on,modified_by")] Truck_dispatches truck_dispatches)
        {
            Truck_dispatches td = db.Truck_dispatches.Find(truck_dispatches.truck_dispatch_id);
            //var td = db.Truck_dispatches.Single(p=> p.truck_dispatch_id ==truck_dispatches.truck_dispatch_id);

            //var td = db.Truck_dispatches.Where(p=> p.truck_dispatch_id == truck_dispatches.truck_dispatch_id);
            //td.ToList()[0].actual_arrival_at_gate = Convert.ToDateTime(Request["actual_arrival_at_gate"]);

            td.left_factory_on = Convert.ToDateTime(Request["left_factory_on"]);

            db.SaveChanges();

            return View("TruckOutward", new List<Truck_dispatches> { td });
        }
        [HandleModelStateException]
        public string GetCustNamebyAgentIdAndOrderId(int orderid)
        {
            OrderRepository objOrderRep = new OrderRepository();
            int LoggedinAgentId = objOrderRep.GetAgentID();
            // get the customer id from Orders table with this order_id and agent_id
            var cust = db.Orders.Where(j => j.agent_id == LoggedinAgentId && j.order_id == orderid).SingleOrDefault();
            int? custid = cust.customer_id;
            var CustRecord = db.Customers.Where(p => p.customer_id == custid).Single();
            return CustRecord.name;


        }

        public void DestroyTransportationSession()
        {
            // destroy the temp session objects
            Session["tempDispatch"] = null;
            Session["tempCargo"] = null;
        }

        #region addTempDispatchinSession
        // Orders Panel --> Order Search --> 'See Detail' of Order --> 
        // Arrange Transportation button --> 'Add Cargo' button click
        // makes a temporary record of transportation in the session 
        [HandleModelStateException]
        public ActionResult addTempDispatchinSession()
        {
            // first destroy the exiting sessionobjects 
            Truck_dispatchRepository.tempTruckDispatch objTruckDispatch = new Truck_dispatchRepository.tempTruckDispatch();
            OrderRepository objOrderRep = new OrderRepository();
            // objTruckDispatch.locations =  db.Papermills;

            objTruckDispatch.agent_id = objOrderRep.GetAgentID();
            objTruckDispatch.created_by = objOrderRep.GetAgentName();
            objTruckDispatch.location_id = Convert.ToInt16(Request["loc_id"]);
            objTruckDispatch.status = "A";
            objTruckDispatch.location = objOrderRep.GetPapermillLocation(objTruckDispatch.location_id);

            // store location in session to get the orders by location and agent id in the add cargo panel
            Session["loc_id"] = Convert.ToInt16(Request["loc_id"]);

            objTruckDispatch.truck_no = Request["vehicle_num"];
            objTruckDispatch.estimated_capacity = Convert.ToDecimal(Request["vehicle_capacity"]);

            DateTime agent_disp_on_date = Convert.ToDateTime(Request["agent_disp_on"]);
            DateTime agent_disp_on_date_withHours;
            string[] agent_disp_on_time = Request["agent_disp_on_time"].Split(':');

            string agent_disp_on_time_ampm = Request["agent_disp_on_time_ampm"];
            if (Request["agent_disp_on_time_ampm"].Contains("am"))
            {
                agent_disp_on_date_withHours = agent_disp_on_date.AddHours(Convert.ToDouble(agent_disp_on_time[0]));
            }
            else
                agent_disp_on_date_withHours = agent_disp_on_date.AddHours(Convert.ToDouble(agent_disp_on_time[0]) + 12);

            objTruckDispatch.agent_dispatched_on = agent_disp_on_date_withHours;

            DateTime estimated_arr_date = Convert.ToDateTime(Request["estimated_arr_date"]);
            DateTime estimated_arr_date_withHours;
            string[] estimated_arr_date_time = Request["estimated_arr_date_time"].Split(':');

            string estimated_arr_date_time_ampm = Request["estimated_arr_date_time_ampm"];
            if (Request["estimated_arr_date_time_ampm"].Contains("am"))
            {
                estimated_arr_date_withHours = estimated_arr_date.AddHours(Convert.ToDouble(estimated_arr_date_time[0]));
            }
            else
                estimated_arr_date_withHours = estimated_arr_date.AddHours(Convert.ToDouble(estimated_arr_date_time[0]) + 12);

            objTruckDispatch.estimated_arrival = estimated_arr_date_withHours;
            objTruckDispatch.created_on = DateTime.Now;

            Session["tempDispatch"] = objTruckDispatch;
            return null;
        }
        #endregion

        /// <summary>
        /// Recent Transportation --> Vehicle (See details) --> Edit Vehicle --> 
        /// Add Cargo link --> Add Cargo button
        /// </summary>
        /// <returns></returns>
        #region addTempCargo
        [HandleModelStateException]
        public PartialViewResult addTempCargoinSession()
        {
            Truck_dispatchRepository.tempTruckDispatchDetails objCargo = new Truck_dispatchRepository.tempTruckDispatchDetails();
            List<Truck_dispatchRepository.tempTruckDispatchDetails> lstCargo;
            int sequenceNumber = 0;

            if (Session["tempCargo"] != null)
            {
                lstCargo = Session["tempCargo"] as List<Truck_dispatchRepository.tempTruckDispatchDetails>;
                sequenceNumber = lstCargo.Last().sequenceNumber + 1;
            }
            else
            {
                lstCargo = new List<Truck_dispatchRepository.tempTruckDispatchDetails>();
                sequenceNumber = 1;
            }
            // preparing the cargo
            OrderRepository objOrderRep = new OrderRepository();

            int loc_id = Convert.ToInt16(Session["loc_id"]);

            objCargo.order_id = Convert.ToInt16(Request["order_id"]); // coming from the drop down of orders
            // with this order id set the customer name in session
            int selectedOrderId = objCargo.order_id;
            string custname = GetCustNamebyAgentIdAndOrderId(selectedOrderId);
            Session["selectedOrderCustName"] = custname;
            int prodid = Convert.ToInt16(Request["prod_id"]);
            TempData["p_id"] = prodid;
            objCargo.order_product_id = prodid; //coming from the drop down of order products
            objCargo.product_code = Request["prod_code"];
            objCargo.qty = Convert.ToDecimal(Request["qty"]); // coming from the textbox
            //objCargo.truck_dispatch_id = ;//will remain null at the moment
            // query the Order_product table and get the width, bf, gsm and shade_co
            //var prod = db.Order_products.Find(prodid);
            var prod = objOrderRep.GetProductDetails(prodid);
            objCargo.width = prod.width;
            objCargo.bf_code = prod.bf_code;
            objCargo.gsm_code = prod.gsm_code;
            objCargo.shade_code = prod.shade_code;
            objCargo.status = "A";
            objCargo.cust_name = custname;
            objCargo.sequenceNumber = sequenceNumber;

            lstCargo.Add(objCargo);

            Session["tempCargo"] = lstCargo;
            sequenceNumber = sequenceNumber + 1;


            // show only the cargos which do not have 'D' status
            // show cargo where status!="D"
            var currentList = lstCargo.Where(k => k.status != "D").ToList();
            Session["filterList"] = currentList;
            if (!currentList.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }

            return PartialView("_currentCargoList", currentList);

            //if (!lstCargo.Any())
            //{
            //    ViewBag.NoRecordMsg = "No record found in our database.";
            //}

            //return PartialView("_currentCargoList", lstCargo);
        }
        #endregion
        [HandleModelStateException]
        public int GetpmIdByTruckDispatchId(int tddid)
        {
            var truck_dispatches = (from r in db.Truck_dispatches
                                    join td in db.Truck_dispatch_details on r.truck_dispatch_id equals td.truck_dispatch_id
                                    //join or in db.Orders on td.order_id equals or.order_id
                                    // join pm in db.Papermills on or.papermill_id equals pm.papermill_id
                                    join or in db.Order_products on td.order_product_id equals or.order_product_id
                                    join sch in db.Schedule on or.schedule_id equals sch.schedule_id
                                    join pm in db.Papermills on sch.papermill_id equals pm.papermill_id
                                    where r.truck_dispatch_id == tddid
                                    select new TempTruckDetails
                                    {
                                        truck_dispatch_id = r.truck_dispatch_id,
                                        truck_no = r.truck_no,
                                        pmlocationid = pm.papermill_id,
                                        location = pm.location,
                                        address = pm.address,
                                        created_on = r.created_on,
                                        estimated_capacity = r.estimated_capacity,
                                        agent_dispatched_on = r.agent_dispatched_on,
                                        estimated_arrival = r.estimated_arrival,
                                        actual_arrival_at_gate = r.actual_arrival_at_gate,
                                        left_factory_on = r.left_factory_on,
                                        papermill_id = pm.papermill_id
                                    })
                                            .ToList();
            OrderRepository objOrder = new OrderRepository();
            // get the papermill list
            var Papermills = objOrder.GetAllPapermills();
            //string selectedpmid = truck_dispatches.Select(p=>p.papermill_id).ToString();
            foreach (var item in truck_dispatches)
            {
                ViewBag.papermill_list = new SelectList(Papermills, "papermill_id", "location", item.papermill_id);
            }

            var objs = (from c in truck_dispatches
                        select c).GroupBy(g => g.truck_dispatch_id).Select(x => x.FirstOrDefault());

            //   .Distinct();
            //    .GroupBy(t => new { t.srNo, t.papermillName, t.estimated_start });
            //.OrderByDescending(x => x.srNo);

            List<TempTruckDetails> query = new List<TempTruckDetails>();
            foreach (var items in objs)
            {
                TempTruckDetails obj = new TempTruckDetails();
                obj.truck_dispatch_id = items.truck_dispatch_id;
                obj.truck_no = items.truck_no;
                obj.pmlocationid = items.papermill_id;
                obj.location = items.location;
                obj.address = items.address;
                obj.created_on = items.created_on;
                obj.estimated_capacity = items.estimated_capacity;
                obj.agent_dispatched_on = items.agent_dispatched_on;
                obj.estimated_arrival = items.estimated_arrival;
                obj.actual_arrival_at_gate = items.actual_arrival_at_gate;
                obj.left_factory_on = items.left_factory_on;
                obj.papermill_id = items.papermill_id;
                TempData["pmid"] = obj.papermill_id;
                query.Add(obj);
            }

            // here put the location id also in the object, get the location id by the truck dispatch id that we pass from ajax
            int pmid = (int)TempData["pmid"];

            return pmid;
        }
        #region GetDispatchAndCargoInSession
        // makes a temporary record of transportation in the session 
        // when I am in the edit mode of See Vehicle detail
        [HandleModelStateException]
        public PartialViewResult GetDispatchAndCargoInSession()
        {
            // first empty the sessions
            Session["tempDispatch"] = "";
            Session["tempCargo"] = "";

            int truck_dispatch_id = Convert.ToInt16(Request["truck_dispatch_id"]);
            Truck_dispatchRepository objtddRep = new Truck_dispatchRepository();
            Truck_dispatches td = objtddRep.GetDispatchBytdid(truck_dispatch_id);
            // here put the location id also in the object, get the location id by the truck dispatch id that we pass from ajax
            int pmid = GetpmIdByTruckDispatchId(truck_dispatch_id);
            // convert the Truck_dispatches type object into the 'Truck_dispatchRepository.tempTruckDispatch' type object
            Truck_dispatchRepository.tempTruckDispatch temptd = new Truck_dispatchRepository.tempTruckDispatch();
            temptd.actual_arrival_at_gate = td.actual_arrival_at_gate;
            temptd.agent_dispatched_on = td.agent_dispatched_on;
            temptd.created_by = td.created_by;
            temptd.created_on = td.created_on;
            temptd.estimated_arrival = td.estimated_arrival;
            temptd.estimated_capacity = td.estimated_capacity;
            temptd.left_factory_on = td.left_factory_on;
            temptd.truck_dispatch_id = td.truck_dispatch_id;
            temptd.truck_no = td.truck_no;

            temptd.location_id = pmid;
            Session["tempDispatch"] = temptd;

            List<Truck_dispatch_details> lsttdd = td.Truckdispatchdetails.ToList();
            List<Truck_dispatchRepository.tempTruckDispatchDetails> lstCargo = new List<Truck_dispatchRepository.tempTruckDispatchDetails>();
            int sequenceNumber = 1;
            foreach (var item in lsttdd)
            {
                Truck_dispatchRepository.tempTruckDispatchDetails objlstCargo = new Truck_dispatchRepository.tempTruckDispatchDetails();

                objlstCargo.sequenceNumber = sequenceNumber;
                objlstCargo.order_id = item.order_id;
                int prodid = item.order_product_id;
                objlstCargo.order_product_id = prodid;
                objlstCargo.product_code = item.Order_products.product_code;
                objlstCargo.cust_name = item.Order.Customer.name;
                objlstCargo.qty = item.qty;
                objlstCargo.truck_dispatch_id = item.truck_dispatch_id;
                objlstCargo.truck_dispatch_details_id = item.truck_dispatch_details_id;

                // query the Order_products table to get the reqd values
                OrderRepository objOrderRep = new OrderRepository();
                //Order_Products proddetail = objOrderRep.GetProductDetails(prodid);
                var proddetail = objOrderRep.GetProductDetails(prodid);
                objlstCargo.bf_code = proddetail.bf_code;
                objlstCargo.gsm_code = proddetail.gsm_code;
                objlstCargo.shade_code = proddetail.shade_code;
                objlstCargo.width = proddetail.width;


                lstCargo.Add(objlstCargo);
                sequenceNumber++;
            }
            Session["tempCargo"] = lstCargo;
            Session["filterList"] = lstCargo;
            return PartialView("_currentCargoList", lstCargo);
        }
        #endregion
        [HandleModelStateException]
        public PartialViewResult addTempCargoinExistingDispatchAndCargoinSession()
        {
            Truck_dispatchRepository.tempTruckDispatchDetails objCargo = new Truck_dispatchRepository.tempTruckDispatchDetails();
            List<Truck_dispatchRepository.tempTruckDispatchDetails> lstCargo;
            int sequenceNumber = 0;
            if (Session["tempCargo"] != null)
            {
                lstCargo = Session["tempCargo"] as List<Truck_dispatchRepository.tempTruckDispatchDetails>;
                sequenceNumber = lstCargo.Count + 1;
            }
            else
            {
                lstCargo = new List<Truck_dispatchRepository.tempTruckDispatchDetails>();
                sequenceNumber = 1;
            }
            // preparing the cargo
            OrderRepository objOrderRep = new OrderRepository();

            int loc_id = Convert.ToInt16(Session["loc_id"]);

            objCargo.order_id = Convert.ToInt16(Request["order_id"]); // coming from the drop down of orders
            // with this order id set the customer name in session
            int selectedOrderId = objCargo.order_id;
            string custname = GetCustNamebyAgentIdAndOrderId(selectedOrderId);
            Session["selectedOrderCustName"] = custname;
            int prodid = Convert.ToInt16(Request["prod_id"]);
            // var prod = db.Order_products.Find(prodid);

            var prod = objOrderRep.GetProductDetails(prodid);
            objCargo.order_product_id = prodid; //coming from the drop down of order products
            objCargo.product_code = Request["prod_code"];
            objCargo.qty = Convert.ToDecimal(Request["qty"]); // coming from the textbox
            //objCargo.truck_dispatch_id = ;//will remain null at the moment
            //get the customer name 
            // objCargo.status = "A";
            // query the Order_product table and get the width, bf, gsm and shade_code

            objCargo.width = prod.width;
            objCargo.bf_code = prod.bf_code;
            objCargo.gsm_code = prod.gsm_code;
            objCargo.shade_code = prod.shade_code;

            objCargo.cust_name = custname;
            objCargo.sequenceNumber = sequenceNumber;

            lstCargo.Add(objCargo);

            Session["tempCargo"] = lstCargo;
            sequenceNumber = sequenceNumber + 1;
            ViewBag.lstCargo = lstCargo;
            //return PartialView("_td_currentCargoList", lstCargo);
            return PartialView("_TransportationDetails", lstCargo);
        }

        #region OrderSearchaddTempCargoinSession
        /// Orders Panel --> Order Search --> See Detail of Order --> Arrange Transportation button
        /// --> Add Cargo blue button
        [HandleModelStateException]
        public PartialViewResult OrderSearchaddTempCargoinSession()
        {
            Truck_dispatchRepository.tempTruckDispatchDetails objCargo = new Truck_dispatchRepository.tempTruckDispatchDetails();
            List<Truck_dispatchRepository.tempTruckDispatchDetails> lstCargo;
            int sequenceNumber = 0;
            if (Session["tempCargo"] != null)
            {
                lstCargo = Session["tempCargo"] as List<Truck_dispatchRepository.tempTruckDispatchDetails>;
                sequenceNumber = lstCargo.Last().sequenceNumber + 1;
            }
            else
            {
                lstCargo = new List<Truck_dispatchRepository.tempTruckDispatchDetails>();
                sequenceNumber = 1;
            }
            // preparing the cargo
            OrderRepository objOrderRep = new OrderRepository();

            int loc_id = Convert.ToInt16(Session["loc_id"]);
            //objCargo.truck_dispatch_id = ;//will remain null 
            objCargo.order_id = Convert.ToInt16(Request["order_id"]); // coming from the drop down of orders
            // with this order id set the customer name in session
            int selectedOrderId = objCargo.order_id;
            //get the customer name
            string custname = GetCustNamebyAgentIdAndOrderId(selectedOrderId);
            Session["selectedOrderCustName"] = custname;
            objCargo.cust_name = custname;
            int prodid = Convert.ToInt16(Request["prod_id"]); //coming from the drop down of order products
            objCargo.order_product_id = prodid;
            string prodcode = Request["prod_code"];
            objCargo.product_code = prodcode;
            objCargo.qty = Convert.ToDecimal(Request["qty"]); // coming from the textbox

            //product details to display
            // query the Order_products table to get the reqd values
            //Order_Products proddetail = objOrderRep.GetProductDetails(prodid);
            var proddetail = objOrderRep.GetProductDetails(prodid);
            objCargo.bf_code = proddetail.bf_code;
            objCargo.gsm_code = proddetail.gsm_code;
            objCargo.shade_code = proddetail.shade_code;
            objCargo.width = proddetail.width;

            objCargo.status = "A";

            objCargo.sequenceNumber = sequenceNumber;

            lstCargo.Add(objCargo);

            Session["tempCargo"] = lstCargo;
            sequenceNumber = sequenceNumber + 1;

            var currentList = lstCargo.Where(k => k.status != "D").ToList();
            Session["filterList"] = currentList;
            if (!currentList.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            return PartialView("_OrderSearchCurrentCargoList", currentList);

            //return PartialView("_OrderSearchReviewTransportation", lsttempCargo);
            //return PartialView("_OrderSearchCurrentCargoList", lstCargo);
            //return PartialView("_currentCargoList", lstCargo);
        }
        #endregion

        #region confirmTransportation
        /// <summary>
        /// Coming from -
        /// 1) Recent Transportation --> Vehicle (See details) --> Edit Vehicle --> 
        /// Add Cargos(if any) --> Submit Transportation --> ReviewEditTransportation() --> Confirm Transportation
        /// 2) Orders Panel --> Order Search --> See Detail of Order --> Arrange Transportation button
        /// --> Add Cargo --> Submit Transportation --> Confirm Submit Transportation
        /// </summary>
        /// <returns></returns>
        [HandleModelStateException]
        public void confirmTransportation()
        {
            // on the confirmation save to the database
            Truck_dispatchRepository objTruckRep = new Truck_dispatchRepository();
            // take the truck dispatch and the cargo details from the session and pass it to the AddDispatch method

            Truck_dispatchRepository.tempTruckDispatch objtempTruckDispatch = new Truck_dispatchRepository.tempTruckDispatch();
            objtempTruckDispatch = Session["tempDispatch"] as Truck_dispatchRepository.tempTruckDispatch;

            // get the list of cargo items from the session
            List<Truck_dispatchRepository.tempTruckDispatchDetails> lsttempCargo = new List<Truck_dispatchRepository.tempTruckDispatchDetails>();
            lsttempCargo = Session["tempCargo"] as List<Truck_dispatchRepository.tempTruckDispatchDetails>;
            int lastid;
            //if ((objtempTruckDispatch.truck_dispatch_id != null || objtempTruckDispatch.truck_dispatch_id != 0) && objtempTruckDispatch.status != "A")
            //if (objtempTruckDispatch.truck_dispatch_id != 0 && objtempTruckDispatch.status != "A")
            //{
            //    lastid = objTruckRep.UpdateDispatch(objtempTruckDispatch, lsttempCargo);
            //}
            if (objtempTruckDispatch.truck_dispatch_id != 0) //coming from update so update in database //&& objtempTruckDispatch.status != "A"
            {
                lastid = objTruckRep.UpdateDispatch(objtempTruckDispatch, lsttempCargo);
            }
            else
            {
                lastid = objTruckRep.AddDispatch(objtempTruckDispatch, lsttempCargo);
            }
            if (lastid != null)
            {
                // destroy the temp session objects
                Session["tempDispatch"] = null;
                Session["tempCargo"] = null;
            }

        }
        #endregion
        [HandleModelStateException]
        public PartialViewResult OrderSearchReviewEditTransportation()
        {
            // Step: 1 - Get the exisitng dispatch record from the session
            Truck_dispatchRepository.tempTruckDispatch objTruckDispatch = new Truck_dispatchRepository.tempTruckDispatch();
            objTruckDispatch = Session["tempDispatch"] as Truck_dispatchRepository.tempTruckDispatch;
            // Step: 2 - get the details from the posted data

            OrderRepository objOrderRep = new OrderRepository();
            // objTruckDispatch.locations =  db.Papermills;

            objTruckDispatch.agent_id = objOrderRep.GetAgentID();
            objTruckDispatch.created_by = objOrderRep.GetAgentName();
            objTruckDispatch.location_id = Convert.ToInt16(Request["loc_id"]);

            objTruckDispatch.location = objOrderRep.GetPapermillLocation(objTruckDispatch.location_id);

            // store location in session to get the orders by location and agent id in the add cargo panel
            Session["loc_id"] = Convert.ToInt16(Request["loc_id"]);

            objTruckDispatch.truck_no = Request["vehicle_num"];
            objTruckDispatch.estimated_capacity = Convert.ToDecimal(Request["vehicle_capacity"]);

            DateTime agent_disp_on_date = Convert.ToDateTime(Request["agent_disp_on"]);
            DateTime agent_disp_on_date_withHours;
            string[] agent_disp_on_time = Request["agent_disp_on_time"].Split(':');

            string agent_disp_on_time_ampm = Request["agent_disp_on_time_ampm"];
            if (Request["agent_disp_on_time_ampm"].Contains("am"))
            {
                agent_disp_on_date_withHours = agent_disp_on_date.AddHours(Convert.ToDouble(agent_disp_on_time[0]));
            }
            else
                agent_disp_on_date_withHours = agent_disp_on_date.AddHours(Convert.ToDouble(agent_disp_on_time[0]) + 12);

            objTruckDispatch.agent_dispatched_on = agent_disp_on_date_withHours;

            DateTime estimated_arr_date = Convert.ToDateTime(Request["estimated_arr_date"]);
            DateTime estimated_arr_date_withHours;
            string[] estimated_arr_date_time = Request["estimated_arr_date_time"].Split(':');

            string estimated_arr_date_time_ampm = Request["estimated_arr_date_time_ampm"];
            if (Request["estimated_arr_date_time_ampm"].Contains("am"))
            {
                estimated_arr_date_withHours = estimated_arr_date.AddHours(Convert.ToDouble(estimated_arr_date_time[0]));
            }
            else
                estimated_arr_date_withHours = estimated_arr_date.AddHours(Convert.ToDouble(estimated_arr_date_time[0]) + 12);

            objTruckDispatch.estimated_arrival = estimated_arr_date_withHours;
            objTruckDispatch.created_on = DateTime.Now;

            // Step 3: - put back into session
            Session["tempDispatch"] = objTruckDispatch;

            // set the ViewBag so that the header details are displayed using the ViewBag
            ViewBag.truck_no = objTruckDispatch.truck_no;
            ViewBag.location_id = objTruckDispatch.location_id; // this is papermill id
            ViewBag.location = objTruckDispatch.location;
            ViewBag.address = objTruckDispatch.address;
            ViewBag.created_on = objTruckDispatch.created_on;
            ViewBag.estimated_capacity = objTruckDispatch.estimated_capacity;
            ViewBag.agent_dispatched_on = objTruckDispatch.agent_dispatched_on;
            ViewBag.estimated_arrival = objTruckDispatch.estimated_arrival;
            ViewBag.actual_arrival_at_gate = objTruckDispatch.actual_arrival_at_gate;
            ViewBag.left_factory_on = objTruckDispatch.left_factory_on;

            // get the list of cargo items from the session
            List<Truck_dispatchRepository.tempTruckDispatchDetails> lsttempCargo = new List<Truck_dispatchRepository.tempTruckDispatchDetails>();
            lsttempCargo = Session["tempCargo"] as List<Truck_dispatchRepository.tempTruckDispatchDetails>;

            // show only the cargos which do not have 'D' status
            // show cargo where status!="D"
            var currentList = lsttempCargo.Where(k => k.status != "D").ToList();
            int countCargosMarkedAsD = currentList.Where(k => k.status == "D").Count();
            //if (!currentList.Any())
            //{
            //    ViewBag.NoRecordMsg = "No record found in our database.";
            //}
            decimal? loadedQty = 0;
            foreach (var itmes in lsttempCargo)
            {
                if (itmes.status == "A")
                {
                    loadedQty = loadedQty + itmes.qty;
                }
            }

            if (objTruckDispatch.estimated_capacity < loadedQty)
            {
                ViewBag.Qtymsg = "Truck capacity is less than the total cargo being loaded on it";
            }
            if (!currentList.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            if (countCargosMarkedAsD == currentList.Count)
            {
                ViewBag.NoCargo = "No cargos are there in Vehicle. Please add some cargos";
            }
            //return PartialView("_OrderSearchReviewTransportation", lsttempCargo);
            return PartialView("_OrderSearchReviewTransportation", currentList);
        }

        /// <summary>
        /// Recent Transportation --> Vehicle (See details) --> Edit Vehicle --> 
        /// Add Cargos(if any) --> Submit Transportation --> ReviewEditTransportation()
        /// </summary>
        /// <returns>partial view: "_reviewTransportation" </returns>
        [HandleModelStateException]
        public PartialViewResult ReviewEditTransportation()
        {

            Truck_dispatchRepository.tempTruckDispatch objTruckDispatch = new Truck_dispatchRepository.tempTruckDispatch();
            if (Session["tempDispatch"] != null) // User is coming from edit Vehicle
            {
                // Step: 1 - Get the exisitng dispatch record from the session

                objTruckDispatch = Session["tempDispatch"] as Truck_dispatchRepository.tempTruckDispatch;

            }
            else // User is coming from 'Arrange New Transportation' form, on 'Submit Transportation', create the 'Session["tempDispatch"]' session
            {
                Session["tempDispatch"] = objTruckDispatch;
                objTruckDispatch.location_id = Convert.ToInt16(Request["loc_id"]);
            }
            // Step: 2 - get the details from the posted data

            OrderRepository objOrderRep = new OrderRepository();
            // objTruckDispatch.locations =  db.Papermills;

            objTruckDispatch.agent_id = objOrderRep.GetAgentID();
            objTruckDispatch.created_by = objOrderRep.GetAgentName();

            objTruckDispatch.location = objOrderRep.GetPapermillLocation(objTruckDispatch.location_id);

            // store location in session to get the orders by location and agent id in the add cargo panel
            //   Session["loc_id"] = Convert.ToInt16(Request["loc_id"]);

            objTruckDispatch.truck_no = Request["vehicle_num"];
            objTruckDispatch.estimated_capacity = Convert.ToDecimal(Request["vehicle_capacity"]);
            objTruckDispatch.status = "A";

            DateTime agent_disp_on_date = Convert.ToDateTime(Request["agent_disp_on"]); //Convert.ToDateTime("2015-07-17 08:00:00.000"); //
            DateTime agent_disp_on_date_withHours;
            string[] agent_disp_on_time = Request["agent_disp_on_time"].Split(':');

            string agent_disp_on_time_ampm = Request["agent_disp_on_time_ampm"];
            if (Request["agent_disp_on_time_ampm"].Contains("am"))
            {
                agent_disp_on_date_withHours = agent_disp_on_date.AddHours(Convert.ToDouble(agent_disp_on_time[0]));
            }
            else
                agent_disp_on_date_withHours = agent_disp_on_date.AddHours(Convert.ToDouble(agent_disp_on_time[0]) + 12);

            objTruckDispatch.agent_dispatched_on = agent_disp_on_date_withHours;

            DateTime estimated_arr_date = Convert.ToDateTime(Request["estimated_arr_date"]); //Convert.ToDateTime("2015-07-18 08:00:00.000"); //
            DateTime estimated_arr_date_withHours;
            string[] estimated_arr_date_time = Request["estimated_arr_date_time"].Split(':');

            string estimated_arr_date_time_ampm = Request["estimated_arr_date_time_ampm"];
            if (Request["estimated_arr_date_time_ampm"].Contains("am"))
            {
                estimated_arr_date_withHours = estimated_arr_date.AddHours(Convert.ToDouble(estimated_arr_date_time[0]));
            }
            else
                estimated_arr_date_withHours = estimated_arr_date.AddHours(Convert.ToDouble(estimated_arr_date_time[0]) + 12);

            objTruckDispatch.estimated_arrival = estimated_arr_date_withHours;
            objTruckDispatch.created_on = DateTime.Now;

            // Step 3: - put back into session
            Session["tempDispatch"] = objTruckDispatch;



            // set the ViewBag so that the header details are displayed using the ViewBag
            ViewBag.truck_no = objTruckDispatch.truck_no;
            ViewBag.location_id = objTruckDispatch.location_id; // this is papermill id
            ViewBag.location = objTruckDispatch.location;
            ViewBag.address = objTruckDispatch.address;
            ViewBag.created_on = objTruckDispatch.created_on;
            ViewBag.estimated_capacity = objTruckDispatch.estimated_capacity;
            ViewBag.agent_dispatched_on = objTruckDispatch.agent_dispatched_on;
            ViewBag.estimated_arrival = objTruckDispatch.estimated_arrival;
            ViewBag.actual_arrival_at_gate = objTruckDispatch.actual_arrival_at_gate;
            ViewBag.left_factory_on = objTruckDispatch.left_factory_on;

            // get the list of cargo items from the session
            List<Truck_dispatchRepository.tempTruckDispatchDetails> lsttempCargo = new List<Truck_dispatchRepository.tempTruckDispatchDetails>();
            lsttempCargo = Session["tempCargo"] as List<Truck_dispatchRepository.tempTruckDispatchDetails>;

            // show only the cargos which do not have 'D' status
            // show cargo where status!="D"
            var currentList = lsttempCargo.Where(k => k.status != "D").ToList();
            int countCargosMarkedAsD = currentList.Where(k => k.status == "D").Count();
            decimal? loadedQty = 0;
            foreach (var itmes in lsttempCargo)
            {
                if (itmes.status == "A")
                {
                    loadedQty = loadedQty + itmes.qty;
                }
            }

            if (objTruckDispatch.estimated_capacity < loadedQty)
            {
                ViewBag.Qtymsg = "Truck capacity is less than the total cargo being loaded on it";
            }
            if (!currentList.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            if (countCargosMarkedAsD == currentList.Count)
            {
                ViewBag.NoCargo = "No cargos are there in Vehicle. Please add some cargos";
            }
            return PartialView("_reviewTransportation", currentList);
            //return PartialView("_reviewTransportation", lsttempCargo);

        }

        //this below method was used when on when 'edit' was not considered on 'submit  transportation', instead now 'ReviewEditTransportation' method is used
        [HandleModelStateException]
        public PartialViewResult GetReviewTransportation()
        {
            Truck_dispatchRepository.tempTruckDispatch objTransportationDetails = new Truck_dispatchRepository.tempTruckDispatch();
            objTransportationDetails = Session["tempDispatch"] as Truck_dispatchRepository.tempTruckDispatch;

            ViewBag.truck_no = objTransportationDetails.truck_no;
            ViewBag.location_id = objTransportationDetails.location_id; // this is papermill id
            ViewBag.location = objTransportationDetails.location;
            ViewBag.address = objTransportationDetails.address;
            ViewBag.created_on = objTransportationDetails.created_on;
            ViewBag.estimated_capacity = objTransportationDetails.estimated_capacity;
            ViewBag.agent_dispatched_on = objTransportationDetails.agent_dispatched_on;
            ViewBag.estimated_arrival = objTransportationDetails.estimated_arrival;
            ViewBag.actual_arrival_at_gate = objTransportationDetails.actual_arrival_at_gate;
            ViewBag.left_factory_on = objTransportationDetails.left_factory_on;

            // get the list of cargo items from the session
            List<Truck_dispatchRepository.tempTruckDispatchDetails> lsttempCargo = new List<Truck_dispatchRepository.tempTruckDispatchDetails>();
            lsttempCargo = Session["tempCargo"] as List<Truck_dispatchRepository.tempTruckDispatchDetails>;

            return PartialView("_reviewTransportation", lsttempCargo);
        }

        /// <summary>
        /// Coming from -
        /// 1) Recent Transportation --> Vehicle (See details) --> Edit Vehicle --> 
        /// Add Cargos(if any) --> Submit Transportation --> ReviewEditTransportation() --> Confirm Transportation
        /// 2) Orders Panel --> Order Search --> See Detail of Order --> Arrange Transportation button
        /// --> Add Cargo --> Submit Transportation --> Confirm Submit Transportation
        /// </summary>
        [HandleModelStateException]
        public PartialViewResult confirmRemoveCargo(int id)
        {
            List<Truck_dispatchRepository.tempTruckDispatchDetails> lstTempCargo = new List<Truck_dispatchRepository.tempTruckDispatchDetails>();
            lstTempCargo = Session["tempCargo"] as List<Truck_dispatchRepository.tempTruckDispatchDetails>; //GetSessionItem<OrderRepository.tempOrderProducts>("tempOrderProds");

            // query the list to get the item at this id
            var cargo = lstTempCargo.Where(p => p.sequenceNumber == id).SingleOrDefault();
            // update the status = 'D' of this record
            if (cargo != null) //found a record with this sequence id
            {
                cargo.status = "D";
            }
            Session["tempCargo"] = lstTempCargo;

            // do not remove from the list to show in the current products list, just leave the cargo with statud 'D'
            // lstTempCargo.Remove(cargo);

            //loadProductFormLists();
            // show only the ones whose status!="D"
            var currentList = lstTempCargo.Where(k => k.status != "D").ToList();
            int countCargosMarkedAsD = currentList.Where(k => k.status == "D").Count();
            Session["filterList"] = currentList;
            //if(countCargosMarkedAsD == currentList.Count)
            //{
            //    ViewBag.NoCargo = 0;
            //}

            if (!currentList.Any() || (countCargosMarkedAsD == currentList.Count))
            {
                ViewBag.NoRecordMsg = "No data available !";
                // Session["filterList"] = null;
            }
            // return PartialView("_currentCargoList", lstTempCargo);
            return PartialView("_currentCargoList", currentList);
        }
        [HandleModelStateException]
        public PartialViewResult GetArrangeNewForm()
        {

            OrderRepository objOrder = new OrderRepository();
            // get the papermill list
            var Papermills = objOrder.GetPapermills();
            ViewBag.papermill_list = new SelectList(Papermills, "papermill_id", "location");


            return PartialView("_ArrangeNewTransportation");
        }
        [HandleModelStateException]
        public PartialViewResult GetDispatchinSession(int? id)
        {
            // first empty the sessions
            Session["tempDispatch"] = "";
            Session["tempCargo"] = "";

            //string vehicle_num = Request["vehicle_num"];
            Truck_dispatchRepository objtddRep = new Truck_dispatchRepository();
            Truck_dispatches tdispatch = objtddRep.GetDispatchBytdid(id);

            Session["tempDispatch"] = tdispatch;
            List<Truck_dispatch_details> lsttdd = tdispatch.Truckdispatchdetails.ToList();
            List<Truck_dispatchRepository.tempTruckDispatchDetails> lstCargo = new List<Truck_dispatchRepository.tempTruckDispatchDetails>();
            Truck_dispatchRepository.tempTruckDispatchDetails objlstCargo = new Truck_dispatchRepository.tempTruckDispatchDetails();
            foreach (var item in lsttdd)
            {
                objlstCargo.order_id = item.order_id;
                objlstCargo.order_product_id = item.order_product_id;
                objlstCargo.product_code = item.Order_products.product_code;
                objlstCargo.cust_name = item.Order.Customer.name;
                objlstCargo.qty = item.qty;
                objlstCargo.truck_dispatch_id = item.truck_dispatch_id;
                objlstCargo.truck_dispatch_details_id = item.truck_dispatch_details_id;
                lstCargo.Add(objlstCargo);
            }
            Session["tempCargo"] = lstCargo;
            ViewBag.ExistingCargoList = lstCargo;

            return PartialView("_currentCargoList", lstCargo);
        }
        [HandleModelStateException]
        public PartialViewResult GetVehicleForEdit(int? id)
        {
            OrderRepository objOrder = new OrderRepository();

            var truck_dispatches = (from r in db.Truck_dispatches
                                    join td in db.Truck_dispatch_details on r.truck_dispatch_id equals td.truck_dispatch_id
                                    //join or in db.Orders on td.order_id equals or.order_id
                                    join op in db.Order_products on td.order_product_id equals op.order_product_id
                                    join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                    join pm in db.Papermills on sch.papermill_id equals pm.papermill_id

                                    where r.truck_dispatch_id == id
                                    select new TempTruckDetails
                                    {
                                        truck_dispatch_id = r.truck_dispatch_id,
                                        truck_no = r.truck_no,
                                        pmlocationid = pm.papermill_id,
                                        location = pm.location,
                                        address = pm.address,
                                        created_on = r.created_on,
                                        estimated_capacity = r.estimated_capacity,
                                        agent_dispatched_on = r.agent_dispatched_on,
                                        estimated_arrival = r.estimated_arrival,
                                        actual_arrival_at_gate = r.actual_arrival_at_gate,
                                        left_factory_on = r.left_factory_on,
                                        papermill_id = pm.papermill_id
                                    })
                                    .ToList();

            // get the papermill list
            var Papermills = objOrder.GetAllPapermills();
            //string selectedpmid = truck_dispatches.Select(p=>p.papermill_id).ToString();
            foreach (var item in truck_dispatches)
            {
                ViewBag.papermill_list = new SelectList(Papermills, "papermill_id", "location", item.papermill_id);
            }

            var objs = (from c in truck_dispatches
                        select c).GroupBy(g => g.truck_dispatch_id).Select(x => x.FirstOrDefault());

            //   .Distinct();
            //    .GroupBy(t => new { t.srNo, t.papermillName, t.estimated_start });
            //.OrderByDescending(x => x.srNo);

            List<TempTruckDetails> query = new List<TempTruckDetails>();
            foreach (var items in objs)
            {
                TempTruckDetails obj = new TempTruckDetails();
                obj.truck_dispatch_id = items.truck_dispatch_id;
                obj.truck_no = items.truck_no;
                obj.pmlocationid = items.papermill_id;
                obj.location = items.location;
                obj.address = items.address;
                obj.created_on = items.created_on;
                obj.estimated_capacity = items.estimated_capacity;
                obj.agent_dispatched_on = items.agent_dispatched_on;
                obj.estimated_arrival = items.estimated_arrival;
                obj.actual_arrival_at_gate = items.actual_arrival_at_gate;
                obj.left_factory_on = items.left_factory_on;
                obj.papermill_id = items.papermill_id;
                query.Add(obj);
            }


            // var truckdetails = truck_dispatches.GroupBy(t => new { t.truck_dispatch_id });
            ViewBag.VehicleDetails = query;
            return PartialView("_ArrangeNewTransportation");
        }
        [HandleModelStateException]
        public PartialViewResult GetVehicleOnQueueForLocation(int? page)
        {
            if (page == null)
            {
                Session["locationId"] = null;
                Session["locationId"] = Convert.ToInt16(Request["locationId"]);
            }

            int pageSize = 5;
            int pageNumber = (page ?? 1);
            int id = Convert.ToInt16(Session["locationId"]);
            Truck_dispatchRepository tdrObj = new Truck_dispatchRepository();
            var vehicleList = tdrObj.GetVehicleOnQueueForLocation(id);
            ViewBag.vehicleList = vehicleList.ToPagedList(pageNumber, pageSize);
            ViewBag.Pagesize = vehicleList.Count();
            OrderRepository objOrderRep = new OrderRepository();
            ViewBag.LoggedInAgentId = objOrderRep.GetAgentID();
            if (!vehicleList.Any())
            {
                ViewBag.NoRecordMsg = "No data available !";
            }
            return PartialView("_VehiclesInQueueByLocation");
        }
    }
}
