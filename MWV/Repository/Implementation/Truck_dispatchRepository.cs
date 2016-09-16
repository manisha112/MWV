using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWV.Repository.Interfaces;
using MWV.Models;
using MWV.DBContext;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Text.RegularExpressions;

namespace MWV.Repository.Implementation
{
    public class Truck_dispatchRepository : ITruck_dispatch
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        #region Getkeeper
        public List<Papermill> PappermillLocation()
        {
            try
            {
                string currentUserId = HttpContext.Current.User.Identity.GetUserId();

                var userWorkLocation = from r in db.AspNetUsers
                                       where r.Id == currentUserId
                                       select r;
                var papermillLocation = (from r in userWorkLocation
                                         join re in db.Papermills on r.location equals re.location
                                         select re).ToList();
                return papermillLocation;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_dispatchRepository->GetTruckOutward:", Ex);
                return null;
            }
        }
        public List<Truck_dispatches> GetTruckInward()
        {
            try
            {
                var papermillLocation = PappermillLocation().Select(p => p.location);
                var millLocation = papermillLocation.First().Trim();
                var papermills = db.Papermills.Where(p => p.location == millLocation).ToList();
                int firstPapermillId = papermills[0].papermill_id;
                int secondPappermil = papermills[1].papermill_id;
                var Currrentday = DateTime.Now.ToShortDateString();
                DateTime today = Convert.ToDateTime(Currrentday);
                var tomorrow = today.AddHours(36);

                var lstTruckIn = (from td in db.Truck_dispatches
                                  join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                  join op in db.Order_products on tdd.order_product_id equals op.order_product_id
                                  // join ord in db.Orders on op.order_id equals ord.order_id
                                  join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                  where td.estimated_arrival <= tomorrow
                                     && td.actual_arrival_at_gate == null
                                     && (sch.papermill_id == firstPapermillId || sch.papermill_id == secondPappermil)
                                  select td).ToList();

                var objs = (from c in lstTruckIn
                            orderby c.estimated_arrival descending
                            select c).GroupBy(g => g.truck_dispatch_id).Select(x => x.FirstOrDefault());
                List<Truck_dispatches> query = new List<Truck_dispatches>();

                foreach (var items in objs)
                {
                    Truck_dispatches obj = new Truck_dispatches();
                    obj.estimated_arrival = items.estimated_arrival;
                    obj.actual_arrival_at_gate = items.actual_arrival_at_gate;
                    obj.left_factory_on = items.left_factory_on;
                    obj.loaded_capacity = items.loaded_capacity;
                    obj.status = items.status;
                    obj.truck_dispatch_id = items.truck_dispatch_id;
                    obj.truck_no = items.truck_no;
                    query.Add(obj);
                }
                return query;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_dispatchRepository->GetTruckInward:", Ex);
                return null;
            }
        }
        public List<Truck_dispatches> GetTruckOutward()
        {
            try
            {
                var papermillLocation = PappermillLocation().Select(p => p.location);

                var millLocation = papermillLocation.First().Trim();
                var papermills = db.Papermills.Where(p => p.location == millLocation).ToList();
                int firstPapermillId = papermills[0].papermill_id;
                int secondPappermil = papermills[1].papermill_id;

                var lstTruckIn = (from td in db.Truck_dispatches
                                  join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                  join op in db.Order_products on tdd.order_product_id equals op.order_product_id
                                  // join ord in db.Orders on op.order_id equals ord.order_id
                                  join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                  where td.actual_arrival_at_gate != null
                                     && td.left_factory_on == null
                                     && td.status == "Dispatched"
                                     && (sch.papermill_id == firstPapermillId || sch.papermill_id == secondPappermil)

                                  select td).ToList();
                var objs = (from c in lstTruckIn
                            //orderby c.estimated_arrival descending
                            orderby c.actual_arrival_at_gate descending
                            select c).GroupBy(g => g.truck_dispatch_id).Select(x => x.FirstOrDefault());
                List<Truck_dispatches> query = new List<Truck_dispatches>();

                foreach (var items in objs)
                {
                    Truck_dispatches obj = new Truck_dispatches();
                    obj.estimated_arrival = items.estimated_arrival;
                    obj.actual_arrival_at_gate = items.actual_arrival_at_gate;
                    obj.left_factory_on = items.left_factory_on;
                    obj.loaded_capacity = items.loaded_capacity;
                    obj.status = items.status;
                    obj.truck_dispatch_id = items.truck_dispatch_id;
                    obj.truck_no = items.truck_no;
                    query.Add(obj);
                }

                // var lstTruckOut = db.Truck_dispatches.Where(p => (p.actual_arrival_at_gate != null
                //                                                   && p.left_factory_on == null)).ToList();
                return query;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_dispatchRepository->GetTruckOutward:", Ex);
                return null;
            }
        }
        public Boolean SaveArrivedVehicle(int id)
        {
            try
            {
                string CurrentUserid = System.Web.HttpContext.Current.User.Identity.GetUserId();

                var currentUserName = db.AspNetUsers.Where(p => p.Id == CurrentUserid).Select(x => x.name).SingleOrDefault();
                Truck_dispatches truck_dispatches = db.Truck_dispatches.Find(id);
                truck_dispatches.actual_arrival_at_gate = DateTime.Now;
                truck_dispatches.inward_by = CurrentUserid;
                truck_dispatches.status = "Arrived";
                truck_dispatches.modified_by = currentUserName;
                truck_dispatches.modified_on = DateTime.Now;
                db.SaveChanges();
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_dispatchRepository->SaveArrivedVehicle:", Ex);
                return false;
            }
        }
        public Boolean SaveTruckOutward(int id)
        {
            try
            {
                string CurrentUserid = System.Web.HttpContext.Current.User.Identity.GetUserId();

                var currentUserName = db.AspNetUsers.Where(p => p.Id == CurrentUserid).Select(x => x.name).SingleOrDefault();
                Truck_dispatches truck_dispatches = db.Truck_dispatches.Find(id);
                truck_dispatches.left_factory_on = DateTime.Now;
                truck_dispatches.outward_by = CurrentUserid;
                truck_dispatches.status = "Completed";
                truck_dispatches.modified_by = currentUserName;
                truck_dispatches.modified_on = DateTime.Now;
                db.SaveChanges();
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_dispatchRepository->SaveTruckOutward:", Ex);
                return false;
            }
            //}

        }
        #endregion

        #region Dispatch
        public List<Truck_dispatches> SearchVehicles(string selectVichleText, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var papermillLocation = PappermillLocation().Select(p => p.location);

                var millLocation = papermillLocation.First().Trim();
                var papermills = db.Papermills.Where(p => p.location == millLocation).ToList();
                int firstPapermillId = papermills[0].papermill_id;
                int secondPappermil = papermills[1].papermill_id;

                List<Truck_dispatches> TruckDetailsList = new List<Truck_dispatches>();
                switch (selectVichleText)
                {
                    case "all-vehicles ":

                        TruckDetailsList = (from td in db.Truck_dispatches
                                            join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                            join op in db.Order_products on tdd.order_product_id equals op.order_product_id
                                            //join ord in db.Orders on tdd.order_id equals ord.order_id
                                            join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                            where (td.estimated_arrival >= fromDate) && (td.estimated_arrival <= toDate)
                                                && (sch.papermill_id == firstPapermillId || sch.papermill_id == secondPappermil)
                                            orderby td.estimated_arrival descending
                                            select td).ToList<Truck_dispatches>();

                        //(from d in db.Truck_dispatches
                        // where (d.estimated_arrival > fromDate) && (d.estimated_arrival < toDate)
                        // orderby d.estimated_arrival descending
                        // select d).ToList<Truck_dispatches>();
                        break;

                    case "show-arrived":
                        TruckDetailsList = (from td in db.Truck_dispatches
                                            join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                            join op in db.Order_products on tdd.order_product_id equals op.order_product_id
                                            // join ord in db.Orders on tdd.order_id equals ord.order_id
                                            join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                            where (td.actual_arrival_at_gate >= fromDate) && (td.actual_arrival_at_gate <= toDate)
                                                && (sch.papermill_id == firstPapermillId || sch.papermill_id == secondPappermil)
                                            orderby td.estimated_arrival descending
                                            select td).ToList<Truck_dispatches>();

                        //TruckDetailsList = (from d in db.Truck_dispatches
                        //                    where
                        //                        //(d.estimated_arrival > fromDate) && (d.estimated_arrival < toDate)
                        //                        // && 
                        //                    (d.actual_arrival_at_gate >= fromDate) && (d.actual_arrival_at_gate < toDate)
                        //                    orderby d.estimated_arrival descending
                        //                    select d).ToList<Truck_dispatches>();
                        break;

                    case "show-departed":

                        TruckDetailsList = (from td in db.Truck_dispatches
                                            join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                            join op in db.Order_products on tdd.order_product_id equals op.order_product_id
                                            //  join ord in db.Orders on tdd.order_id equals ord.order_id
                                            join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                            where (td.left_factory_on >= fromDate) && (td.left_factory_on <= toDate)
                                                && (sch.papermill_id == firstPapermillId || sch.papermill_id == secondPappermil)
                                            orderby td.estimated_arrival descending
                                            select td).ToList<Truck_dispatches>();



                        //TruckDetailsList = (from d in db.Truck_dispatches
                        //                    where (d.left_factory_on > fromDate) && (d.left_factory_on < toDate)
                        //                    // && (d.status == "")
                        //                    orderby d.estimated_arrival descending
                        //                    select d).ToList<Truck_dispatches>();
                        break;

                    case "show-in-queue":
                        TruckDetailsList = (from td in db.Truck_dispatches
                                            join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                            join op in db.Order_products on tdd.order_product_id equals op.order_product_id
                                            //join ord in db.Orders on tdd.order_id equals ord.order_id
                                            join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                            where (td.estimated_arrival >= fromDate) && (td.estimated_arrival <= toDate)
                                                && (td.actual_arrival_at_gate == null)
                                                && (sch.papermill_id == firstPapermillId || sch.papermill_id == secondPappermil)
                                            orderby td.estimated_arrival descending
                                            select td).ToList<Truck_dispatches>();

                        //TruckDetailsList = (from d in db.Truck_dispatches
                        //                    where (d.estimated_arrival > fromDate) && (d.estimated_arrival < toDate)
                        //                    && (d.actual_arrival_at_gate == null)
                        //                    //&& d.status == "Created"
                        //                    orderby d.estimated_arrival descending
                        //                    select d).ToList<Truck_dispatches>();
                        break;
                }
                return TruckDetailsList;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_DispatchRepostitory->SearchVehicles:", Ex);
                return null;
            }
        }
        public List<TempTruckDetails> SearchVehiclesByProdCode(string searchByString, string searchString, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var papermillLocation = PappermillLocation().Select(p => p.location);

                var millLocation = papermillLocation.First().Trim();
                var papermills = db.Papermills.Where(p => p.location == millLocation).ToList();
                int firstPapermillId = papermills[0].papermill_id;
                int secondPappermil = papermills[1].papermill_id;

                List<TempTruckDetails> VehiclesByProdCode = new List<TempTruckDetails>();
                VehiclesByProdCode = (from td in db.Truck_dispatches
                                      join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                      join op in db.Order_products on tdd.order_product_id equals op.order_product_id

                                      // join ord in db.Orders on op.order_id equals ord.order_id
                                      join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                      where (td.estimated_arrival >= fromDate && td.estimated_arrival <= toDate)
                                       && op.product_code == searchString
                                       && (sch.papermill_id == firstPapermillId || sch.papermill_id == secondPappermil)
                                      select new TempTruckDetails
                                      {
                                          agentdetails = td.agentdetails,
                                          truck_no = td.truck_no,
                                          agentname = td.agentdetails.name,
                                          estimated_arrival = td.estimated_arrival,
                                          actual_arrival_at_gate = td.actual_arrival_at_gate,
                                          left_factory_on = td.left_factory_on,
                                          truck_dispatch_id = td.truck_dispatch_id,
                                          status = td.status,
                                          millId = sch.papermill_id
                                      }).ToList<TempTruckDetails>();

                var objs = (from c in VehiclesByProdCode
                            orderby c.estimated_arrival ascending
                            select c).GroupBy(g => g.truck_dispatch_id).Select(x => x.FirstOrDefault());
                List<TempTruckDetails> query = new List<TempTruckDetails>();

                foreach (var items in objs)
                {
                    TempTruckDetails obj = new TempTruckDetails();
                    obj.truck_dispatch_id = items.truck_dispatch_id;
                    obj.truck_no = items.truck_no;
                    obj.agentdetails = items.agentdetails;
                    obj.agentname = items.agentname;
                    obj.estimated_arrival = items.estimated_arrival;
                    obj.actual_arrival_at_gate = items.actual_arrival_at_gate;
                    obj.left_factory_on = items.left_factory_on;
                    obj.truck_dispatch_id = items.truck_dispatch_id;
                    obj.status = items.status;
                    query.Add(obj);
                }
                return query;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_DispatchRepostitory->SearchVehiclesByProdCode:", Ex);
                return null;
            }
        }
        public List<TempTruckDetails> SearchVehiclesByAgentIdAndTruckNum(string searchByString, string searchString, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var papermillLocation = PappermillLocation().Select(p => p.location);
                var millLocation = papermillLocation.First().Trim();
                var papermills = db.Papermills.Where(p => p.location == millLocation).ToList();
                int firstPapermillId = papermills[0].papermill_id;
                int secondPappermil = papermills[1].papermill_id;
                List<TempTruckDetails> Vehicles = new List<TempTruckDetails>();
                switch (searchByString)
                {
                    case "agent-name":
                        int agentId = Convert.ToInt16(searchString);
                        Vehicles = (from td in db.Truck_dispatches
                                    join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                    join op in db.Order_products on tdd.order_product_id equals op.order_product_id
                                    //join ord in db.Orders on op.order_id equals ord.order_id
                                    join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                    where (td.estimated_arrival >= fromDate && td.estimated_arrival <= toDate)
                                        //td.actual_arrival_at_gate != null
                                        && td.agent_id == agentId
                                        && (sch.papermill_id == firstPapermillId || sch.papermill_id == secondPappermil)
                                    select new TempTruckDetails
                                    {
                                        agentdetails = td.agentdetails,
                                        truck_no = td.truck_no,
                                        agentname = td.agentdetails.name,
                                        estimated_arrival = td.estimated_arrival,
                                        actual_arrival_at_gate = td.actual_arrival_at_gate,
                                        left_factory_on = td.left_factory_on,
                                        truck_dispatch_id = td.truck_dispatch_id,
                                        status = td.status
                                    }).ToList<TempTruckDetails>();


                        //Vehicles = db.Truck_dispatches.Where(p => p.agentdetails.agent_id == agentId
                        //    && (p.estimated_arrival > fromDate && p.estimated_arrival < toDate)).ToList<Truck_dispatches>();
                        break;

                    case "vehicle-no":
                        Vehicles = (from td in db.Truck_dispatches
                                    join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                    join op in db.Order_products on tdd.order_product_id equals op.order_product_id
                                    // join ord in db.Orders on tdd.order_id equals ord.order_id
                                    join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                    where td.truck_no == searchString
                                    && (sch.papermill_id == firstPapermillId || sch.papermill_id == secondPappermil)
                                    select new TempTruckDetails
                                    {
                                        agentdetails = td.agentdetails,
                                        truck_no = td.truck_no,
                                        agentname = td.agentdetails.name,
                                        estimated_arrival = td.estimated_arrival,
                                        actual_arrival_at_gate = td.actual_arrival_at_gate,
                                        left_factory_on = td.left_factory_on,
                                        truck_dispatch_id = td.truck_dispatch_id,
                                        status = td.status
                                    }).ToList<TempTruckDetails>();

                        //Vehicles = db.Truck_dispatches.Where(p => p.truck_no == searchString).ToList<Truck_dispatches>();

                        break;
                }
                var objs = (from c in Vehicles
                            orderby c.truck_dispatch_id
                            select c).GroupBy(g => g.truck_dispatch_id).Select(x => x.FirstOrDefault());
                List<TempTruckDetails> query = new List<TempTruckDetails>();

                foreach (var items in objs)
                {
                    TempTruckDetails obj = new TempTruckDetails();
                    obj.truck_dispatch_id = items.truck_dispatch_id;
                    obj.truck_no = items.truck_no;
                    obj.agentdetails = items.agentdetails;
                    obj.agentname = items.agentname;
                    obj.estimated_arrival = items.estimated_arrival;
                    obj.actual_arrival_at_gate = items.actual_arrival_at_gate;
                    obj.left_factory_on = items.left_factory_on;
                    obj.truck_dispatch_id = items.truck_dispatch_id;
                    obj.status = items.status;
                    query.Add(obj);
                }
                return query;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_DispatchRepostitory->SearchVehiclesByAgentIdAndTruckNum:", Ex);
                return null;
            }
        }
        public List<TempTruckDetails> SearchVehiclesByAgentAndVehicleStatus(string searchByString, string searchString, string filterString, DateTime fromDate, DateTime toDate)
        {
            try
            {
                List<TempTruckDetails> Vehicles = new List<TempTruckDetails>();
                var papermillLocation = PappermillLocation().Select(p => p.location);
                var millLocation = papermillLocation.First().Trim();
                var papermills = db.Papermills.Where(p => p.location == millLocation).ToList();
                int firstPapermillId = papermills[0].papermill_id;
                int secondPappermil = papermills[1].papermill_id;

                var VehiclesResult = (from td in db.Truck_dispatches
                                      join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                      join op in db.Order_products on tdd.order_product_id equals op.order_product_id
                                      //  join ord in db.Orders on op.order_id equals ord.order_id
                                      join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                      where
                                          //td.actual_arrival_at_gate != null
                                          // && td.agent_id == agentId
                                          //  &&
                                      (sch.papermill_id == firstPapermillId || sch.papermill_id == secondPappermil)
                                      select new TempTruckDetails
                                      {
                                          agentdetails = td.agentdetails,
                                          truck_no = td.truck_no,
                                          agentname = td.agentdetails.name,
                                          estimated_arrival = td.estimated_arrival,
                                          actual_arrival_at_gate = td.actual_arrival_at_gate,
                                          left_factory_on = td.left_factory_on,
                                          truck_dispatch_id = td.truck_dispatch_id,
                                          status = td.status
                                      }).ToList<TempTruckDetails>();



                int agentId = Convert.ToInt16(searchString);
                switch (filterString)
                {
                    case "show-arrived":
                        Vehicles = VehiclesResult.Where(p => p.agentdetails.agent_id == agentId
                           && (p.status == "Arrived")
                           && (p.estimated_arrival >= fromDate && p.estimated_arrival <= toDate)).ToList<TempTruckDetails>();
                        break;
                    case "all-vehicles":
                        Vehicles = VehiclesResult.Where(p => p.agentdetails.agent_id == agentId
                           && (p.estimated_arrival >= fromDate && p.estimated_arrival <= toDate)).ToList<TempTruckDetails>();
                        break;
                    case "show-departed":
                        Vehicles = VehiclesResult.Where(p => p.agentdetails.agent_id == agentId
                             && (p.status == "Dispatched" || p.status == "Completed")
                           && (p.estimated_arrival >= fromDate && p.estimated_arrival <= toDate)).ToList<TempTruckDetails>();
                        break;
                    case "show-in-queue":
                        Vehicles = VehiclesResult.Where(p => p.agentdetails.agent_id == agentId
                             && (p.status == "Created")
                           && (p.estimated_arrival >= fromDate && p.estimated_arrival <= toDate)).ToList<TempTruckDetails>();
                        break;

                }
                var objs = (from c in Vehicles
                            orderby c.truck_dispatch_id
                            select c).GroupBy(g => g.truck_dispatch_id).Select(x => x.FirstOrDefault());
                List<TempTruckDetails> query = new List<TempTruckDetails>();

                foreach (var items in objs)
                {
                    TempTruckDetails obj = new TempTruckDetails();
                    obj.truck_dispatch_id = items.truck_dispatch_id;
                    obj.truck_no = items.truck_no;
                    obj.agentdetails = items.agentdetails;
                    obj.agentname = items.agentname;
                    obj.estimated_arrival = items.estimated_arrival;
                    obj.actual_arrival_at_gate = items.actual_arrival_at_gate;
                    obj.left_factory_on = items.left_factory_on;
                    obj.truck_dispatch_id = items.truck_dispatch_id;
                    obj.status = items.status;
                    query.Add(obj);
                }
                return query;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_DispatchRepostitory->SearchVehiclesByAgentAndVehicleStatus:", Ex);
                return null;
            }
        }
        public List<TempTruckDetails> SearchVehiclesByProductcodeWithStatus(string searchByString, string searchString, string filterString, DateTime fromDate, DateTime toDate)
        {
            try
            {
                List<TempTruckDetails> Vehicles = new List<TempTruckDetails>();

                switch (filterString)
                {
                    case "show-arrived":
                        var VehiclesSearch = SearchVehiclesByProdCode(searchByString, searchString, fromDate, toDate);
                        Vehicles = VehiclesSearch.Where(p => p.status == "Arrived").ToList<TempTruckDetails>();
                        break;
                    case "all-vehicles":
                        var allVehiclesSearch = SearchVehiclesByProdCode(searchByString, searchString, fromDate, toDate);
                        Vehicles = allVehiclesSearch.ToList<TempTruckDetails>();
                        break;
                    case "show-departed":
                        var departedVehiclesSearch = SearchVehiclesByProdCode(searchByString, searchString, fromDate, toDate);
                        Vehicles = departedVehiclesSearch.Where(p => (p.status == "Dispatched") || (p.status == "Completed")).ToList<TempTruckDetails>();
                        break;
                    case "show-in-queue":

                        var queueVehiclesSearch = SearchVehiclesByProdCode(searchByString, searchString, fromDate, toDate);
                        Vehicles = queueVehiclesSearch.Where(p => p.status == "Created").ToList<TempTruckDetails>();
                        break;

                }
                return Vehicles;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_DispatchRepostitory->SearchVehiclesByProductcodeWithStatus:", Ex);
                return null;
            }
        }
        public List<Truck_dispatches> SearchVehicleByDaysAndVehicleStatus(DateTime dt, string selectVichleText)
        {
            try
            {
                List<Truck_dispatches> TruckDetailsList = new List<Truck_dispatches>();
                DateTime date = Convert.ToDateTime(dt.AddHours(24));
                var papermillLocation = PappermillLocation().Select(p => p.location);
                var millLocation = papermillLocation.First().Trim();
                var papermills = db.Papermills.Where(p => p.location == millLocation).ToList();
                int firstPapermillId = papermills[0].papermill_id;
                int secondPappermil = papermills[1].papermill_id;

                if (selectVichleText == "Today" || selectVichleText == "Tomorrow"
                   || selectVichleText == "NextDay" || selectVichleText == "all-vehicles")
                {
                    TruckDetailsList = (from td in db.Truck_dispatches
                                        join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                        // join ord in db.Orders on tdd.order_product_id equals ord.o
                                        join op in db.Order_products on tdd.order_product_id equals op.order_product_id
                                        join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                        where (td.estimated_arrival >= dt && td.estimated_arrival < date)
                                            //|| (td.left_factory_on > dt) && (td.left_factory_on < date)
                                            && (sch.papermill_id == firstPapermillId || sch.papermill_id == secondPappermil)
                                        orderby td.estimated_arrival descending
                                        select td).ToList<Truck_dispatches>();

                    //(from td in db.Truck_dispatches
                    // where (td.estimated_arrival >= dt && td.estimated_arrival < date)
                    // //|| (td.left_factory_on > dt) && (td.left_factory_on < date)
                    // select td).ToList<Truck_dispatches>();

                }
                else if (selectVichleText == "show-departed")
                {
                    TruckDetailsList = (from td in db.Truck_dispatches
                                        join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                        join ord in db.Order_products on tdd.order_product_id equals ord.order_product_id
                                        join sch in db.Schedule on ord.schedule_id equals sch.schedule_id
                                        where (td.estimated_arrival >= dt && td.estimated_arrival < date)
                                            // || (td.left_factory_on > dt) && (td.left_factory_on < date)
                                            && (td.status == "Dispatched" || td.status == "Completed")
                                        orderby td.estimated_arrival descending
                                        select td).ToList<Truck_dispatches>();

                    //TruckDetailsList = (from d in db.Truck_dispatches
                    //                 where (d.left_factory_on > dt) && (d.left_factory_on < date)
                    //                  && (d.status == "Completed")
                    //                 orderby d.estimated_arrival descending
                    //                 select d).ToList<Truck_dispatches>();
                }
                else if (selectVichleText == "show-arrived")
                {
                    TruckDetailsList = (from td in db.Truck_dispatches
                                        join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                        //join ord in db.Orders on tdd.order_id equals ord.order_id
                                        join op in db.Order_products on tdd.order_product_id equals op.order_product_id
                                        join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                        where (td.estimated_arrival >= dt && td.estimated_arrival < date)
                                            //|| (td.left_factory_on > dt) && (td.left_factory_on < date)
                                            // && (td.actual_arrival_at_gate != null)
                                             && td.status == "Arrived"
                                             && (sch.papermill_id == firstPapermillId || sch.papermill_id == secondPappermil)
                                        orderby td.estimated_arrival descending
                                        select td).ToList<Truck_dispatches>();

                    //TruckDetailsList = (from d in db.Truck_dispatches
                    //                    where (d.estimated_arrival > dt) && (d.estimated_arrival < date)
                    //                    && (d.actual_arrival_at_gate != null) && d.status == "Arrived"
                    //                    orderby d.estimated_arrival descending
                    //                    select d).ToList<Truck_dispatches>();

                }
                else if (selectVichleText == "show-in-queue")
                {
                    TruckDetailsList = (from td in db.Truck_dispatches
                                        join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                        // join ord in db.Orders on tdd.order_id equals ord.order_id
                                        join op in db.Order_products on tdd.order_product_id equals op.order_product_id
                                        join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                        where (td.estimated_arrival >= dt && td.estimated_arrival < date)
                                            //|| (td.left_factory_on > dt) && (td.left_factory_on < date)
                                             && td.status == "Created"
                                             && (sch.papermill_id == firstPapermillId || sch.papermill_id == secondPappermil)
                                        orderby td.estimated_arrival descending
                                        select td).ToList<Truck_dispatches>();

                    //TruckDetailsList = (from td in db.Truck_dispatches
                    //                    where td.estimated_arrival >= dt && td.estimated_arrival < date
                    //                    && td.status == "Created"
                    //                    select td).ToList<Truck_dispatches>();
                }
                var objs = (from c in TruckDetailsList
                            orderby c.truck_dispatch_id
                            select c).GroupBy(g => g.truck_dispatch_id).Select(x => x.FirstOrDefault());
                List<Truck_dispatches> query = new List<Truck_dispatches>();

                foreach (var items in objs)
                {
                    Truck_dispatches obj = new Truck_dispatches();
                    obj.truck_dispatch_id = items.truck_dispatch_id;
                    obj.truck_no = items.truck_no;
                    obj.agentdetails = items.agentdetails;
                    obj.estimated_arrival = items.estimated_arrival;
                    obj.actual_arrival_at_gate = items.actual_arrival_at_gate;
                    obj.left_factory_on = items.left_factory_on;
                    obj.truck_dispatch_id = items.truck_dispatch_id;
                    obj.status = items.status;
                    query.Add(obj);
                }
                return query;
                // return TruckDetailsList;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_DispatchRepostitory->SearchVehicleByDaysAndVehicleStatus:", Ex);
                return null;
            }
        }
        public bool SaveReelsAsDispatched(string allvalues)
        {
            try
            {
                int truckdispatchid = Convert.ToInt16(allvalues.Split(',')[0]);

                var tdDetails = (from td in db.Truck_dispatches
                                 where td.truck_dispatch_id == truckdispatchid
                                 select td).FirstOrDefault();


                //get all reels no which are going to mark as loaded on truck
                string[] checkedReels = Regex.Split(allvalues, ",").Skip(1).ToArray();

                foreach (var reelno in checkedReels)
                {
                    var reelsDetails = (from pc in db.ProductionChild
                                        where pc.child_rollno == reelno
                                        select pc).FirstOrDefault();

                    reelsDetails.truck_no = tdDetails.truck_no;
                    reelsDetails.actual_loaded_qty = reelsDetails.qty;
                    reelsDetails.actual_loaded_on = DateTime.Now;
                    reelsDetails.truck_dispatch_id = tdDetails.truck_dispatch_id;

                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_DispatchRepostitory->SaveReelsAsDispatched:", Ex);
                return false;
            }
        }

        public bool SaveOrdeProductQty(string allvalues)
        {
            try
            {
                int truckdispatchid = Convert.ToInt16(allvalues.Split(',')[0]);

                //reel numbers which is mark as loaded on truck
                string[] reelNos = Regex.Split(allvalues, ",").Skip(1).ToArray();

                foreach (var reelno in reelNos)
                {
                    var reelsDetails = (from pc in db.ProductionChild
                                        where pc.child_rollno == reelno
                                        select pc).FirstOrDefault();

                    var tddDtails = (from tdd in db.Truck_dispatch_details
                                     where tdd.order_product_id == reelsDetails.order_product_id
                                            && tdd.truck_dispatch_id == truckdispatchid
                                     select tdd).FirstOrDefault();

                    //reel qty is storing in loaded qty
                    tddDtails.qty_loaded += reelsDetails.qty;
                    db.SaveChanges();
                }

                //get sum of loaded qty in truck
                var tddLoadedQty = db.Truck_dispatch_details.Where(c => c.truck_dispatch_id == truckdispatchid).Sum(x => x.qty_loaded);

                //get details of truck
                var tdDetails = (from td in db.Truck_dispatches
                                 where td.truck_dispatch_id == truckdispatchid
                                 select td).FirstOrDefault();

                tdDetails.loaded_capacity = tddLoadedQty;
                db.SaveChanges();
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_DispatchRepostitory->SaveReelsAsDispatched:", Ex);
                return false;
            }
        }

        #endregion

        public List<Truck_dispatches> searchResultTransportation(string selectVichleValue, DateTime fromDate, DateTime todate)
        {
            try
            {
                OrderRepository ordRep = new OrderRepository();
                int LoggedInAgentId = ordRep.GetAgentID();

                DateTime toDate = todate.AddHours(23).AddMinutes(59).AddSeconds(59);
                List<Truck_dispatches> TruckDetailsList = new List<Truck_dispatches>();

                switch (selectVichleValue)
                {
                    case "All-Vehicles":

                        TruckDetailsList = (from d in db.Truck_dispatches
                                            where (d.estimated_arrival >= fromDate) && (d.estimated_arrival <= toDate)
                                              && d.agent_id == LoggedInAgentId
                                            orderby d.estimated_arrival descending
                                            select d).ToList<Truck_dispatches>();
                        break;

                    case "Scheduled-Vehicles":
                        TruckDetailsList = (from d in db.Truck_dispatches
                                            where (d.estimated_arrival >= fromDate)
                                              && (d.estimated_arrival <= toDate)
                                              && (d.actual_arrival_at_gate == null)
                                              && d.agent_id == LoggedInAgentId
                                            orderby d.estimated_arrival descending
                                            select d).ToList<Truck_dispatches>();
                        break;

                    case "Departed-Vehicles":

                        TruckDetailsList = (from d in db.Truck_dispatches
                                            where (d.left_factory_on >= fromDate)
                                              && (d.left_factory_on <= toDate)
                                              && d.agent_id == LoggedInAgentId
                                            orderby d.estimated_arrival descending
                                            select d).ToList<Truck_dispatches>();
                        break;
                }
                return TruckDetailsList;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_DispatchRepostitory->searchResultTransportation:", Ex);
                return null;
            }
        }

        public Truck_dispatches GetDispatch(string truckno)
        {
            var dispatch = db.Truck_dispatches.Where(d => d.truck_no == truckno).First();
            return dispatch;
        }

        public Truck_dispatches GetDispatchBytdid(int? truckdispatchid)
        {
            var dispatch = db.Truck_dispatches.Where(d => d.truck_dispatch_id == truckdispatchid).SingleOrDefault();
            return dispatch;
        }

        public int AddDispatch(Truck_dispatchRepository.tempTruckDispatch tempTruckdispatch, List<Truck_dispatchRepository.tempTruckDispatchDetails> lstTempCargo)
        {
            OrderRepository ordRep = new OrderRepository();


            Truck_dispatches objTruckDisp = new Truck_dispatches();
            objTruckDisp.agent_id = tempTruckdispatch.agent_id;
            objTruckDisp.truck_no = tempTruckdispatch.truck_no;
            objTruckDisp.estimated_capacity = tempTruckdispatch.estimated_capacity;
            objTruckDisp.agent_dispatched_on = tempTruckdispatch.agent_dispatched_on;
            objTruckDisp.estimated_arrival = tempTruckdispatch.estimated_arrival;
            objTruckDisp.created_by = tempTruckdispatch.created_by;
            objTruckDisp.created_on = DateTime.Now;
            objTruckDisp.modified_on = DateTime.Now;
            objTruckDisp.modified_by = ordRep.GetAgentName();
            objTruckDisp.status = "Created";
            //objTruckDisp.modified_on = null;

            db.Truck_dispatches.Add(objTruckDisp);
            db.SaveChanges();
            int LastInsertedId = objTruckDisp.truck_dispatch_id;



            // iterate the temp list for the cargo items to be shipped
            foreach (Truck_dispatchRepository.tempTruckDispatchDetails cargoItem in lstTempCargo)
            {
                Truck_dispatch_details objTruck_dispatch_details = new Truck_dispatch_details();
                if (cargoItem.status == "A")
                {
                    objTruck_dispatch_details.truck_dispatch_id = LastInsertedId;
                    objTruck_dispatch_details.order_id = cargoItem.order_id;
                    objTruck_dispatch_details.order_product_id = cargoItem.order_product_id;
                    objTruck_dispatch_details.qty = cargoItem.qty;
                    // update the 'Order_products' table with qty
                    // call it after the demo
                    //Update_qty_planned(cargoItem.order_product_id, cargoItem);
                    db.Truck_dispatch_details.Add(objTruck_dispatch_details);
                    db.SaveChanges();


                    //the qty entered by agent to added to truck that is added to the 'qty_planned_by_agent' field
                    //var ordDetailsList = db.Order_products.Where(p => p.order_id == cargoItem.order_id).SingleOrDefault();
                    ////Order_Products opObj = new Order_Products();
                    //ordDetailsList.qty_planned_byAgent = objTruck_dispatch_details.qty + ordDetailsList.qty_planned_byAgent;                 
                    //db.SaveChanges();

                }
            }
            return LastInsertedId;
        }

        public int UpdateDispatch(Truck_dispatchRepository.tempTruckDispatch tempTruckdispatch, List<Truck_dispatchRepository.tempTruckDispatchDetails> lstTempCargo)
        {
            OrderRepository order = new OrderRepository();
            Truck_dispatches objTruckDisp = db.Truck_dispatches.Find(tempTruckdispatch.truck_dispatch_id); //new Truck_dispatches();
            objTruckDisp.agent_id = tempTruckdispatch.agent_id;
            objTruckDisp.truck_no = tempTruckdispatch.truck_no;
            objTruckDisp.estimated_capacity = tempTruckdispatch.estimated_capacity;
            objTruckDisp.agent_dispatched_on = tempTruckdispatch.agent_dispatched_on;
            objTruckDisp.estimated_arrival = tempTruckdispatch.estimated_arrival;
            objTruckDisp.created_by = tempTruckdispatch.created_by;
            //objTruckDisp.created_on = DateTime.Now;
            // objTruckDisp.status = "Created";
            objTruckDisp.modified_on = DateTime.Now;
            objTruckDisp.modified_by = order.GetAgentName();

            db.SaveChanges();
            //int LastInsertedId = objTruckDisp.truck_dispatch_id;

            // iterate the temp list for the cargo items to be shipped
            foreach (Truck_dispatchRepository.tempTruckDispatchDetails cargoItem in lstTempCargo)
            {
                Truck_dispatch_details objTruck_dispatch_details = new Truck_dispatch_details(); //db.Truck_dispatch_details.Find(tempTruckdispatch.truck_dispatch_id); //new Truck_dispatch_details();
                //objTruck_dispatch_details.truck_dispatch_id = LastInsertedId;
                if (cargoItem.status == "A")
                {
                    objTruck_dispatch_details.truck_dispatch_id = tempTruckdispatch.truck_dispatch_id;
                    objTruck_dispatch_details.order_id = cargoItem.order_id;
                    objTruck_dispatch_details.order_product_id = cargoItem.order_product_id;
                    objTruck_dispatch_details.qty = cargoItem.qty;
                    db.Truck_dispatch_details.Add(objTruck_dispatch_details);
                    db.SaveChanges();
                    // update the 'Order_products' table with qty
                    // call it after the demo
                    //Update_qty_planned(cargoItem.order_product_id, cargoItem);
                    //int ordId = cargoItem.order_id;
                    //var ordDetailsList = db.Order_products.Where(p => p.order_id == ordId).SingleOrDefault();
                    ////Order_Products opObj = new Order_Products();
                    //ordDetailsList.qty_planned_byAgent = objTruck_dispatch_details.qty + ordDetailsList.qty_planned_byAgent;
                    //db.SaveChanges();



                }
                if (cargoItem.status == "D" && cargoItem.truck_dispatch_details_id != 0) //remove the record from the database
                {
                    // find the record
                    Truck_dispatch_details tdd = db.Truck_dispatch_details.Find(cargoItem.truck_dispatch_details_id);
                    db.Truck_dispatch_details.Remove(tdd);
                    db.SaveChanges();
                }
                if (cargoItem.status == "E")
                {
                    // get the row from the db with the 
                    // truck_dispatch_details_id =cargoItem.truck_dispatch_details_id 
                    // and update the record
                }

            }
            return 1; // the update was successfull
        }

        public void Update_qty_planned(int order_product_id, Truck_dispatchRepository.tempTruckDispatchDetails cargoItem)
        {

            // the qty agent has planned to ship is the tempCargo's qty field, 
            // so update the order_products table's 'qty_planned_by_agent' column with this value
            // so that we know how much is remaining

            // find the order_product record with order_id for which 
            var orderProd = db.Order_products.Find(order_product_id);

            orderProd.qty_planned_byAgent = cargoItem.qty;
            db.SaveChanges();
        }

        public void Update_qty_dispatched(int order_product_id, decimal tddqty)
        {
            // the qty dispatcher has loaded on the truck needs to be updated in the Order_products table
            // column 'qty_dispatched_byFactory', so that we know how much has been shipped

            // find the order_product record with order_id for which 
            var orderProd = db.Order_products.Find(order_product_id);

            //Order_Products objOrderProds = new Order_Products();
            orderProd.qty_dispatched_byFactory = orderProd.qty_dispatched_byFactory + tddqty;
            db.SaveChanges();
        }


        public List<Truck_dispatches> searchResultTransportation(string selectedVehicleValue, string vehicleNumber)
        {
            try
            {
                List<Truck_dispatches> TruckDetailsList = new List<Truck_dispatches>();

                TruckDetailsList = (from d in db.Truck_dispatches
                                    where (d.truck_no == vehicleNumber)
                                    orderby d.truck_no descending
                                    select d).ToList<Truck_dispatches>();

                return TruckDetailsList;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_DispatchRepostitory->searchResultTransportation:", Ex);
                return null;
            }
        }

        public void DeleteVehicles(int tdId)
        {
            try
            {

                var TruckDetailsList = (from d in db.Truck_dispatch_details
                                        where (d.truck_dispatch_id == tdId)
                                        orderby d.truck_dispatch_id descending
                                        select d).ToList();
                var TruckDispatchMstList = (from d in db.Truck_dispatches
                                            where (d.truck_dispatch_id == tdId) && (d.actual_arrival_at_gate == null)
                                            orderby d.truck_dispatch_id descending
                                            select d);

                //var ordPrdId = TruckDetailsList.Select(p => p.order_product_id).SingleOrDefault();

                foreach (var items in TruckDetailsList)
                {
                    var productList = (from op in db.Order_products
                                       where op.order_id == items.order_id
                                       && op.order_product_id == items.order_product_id
                                       select op).SingleOrDefault();

                    //Order_Products opObj = new Order_Products();
                    productList.qty_planned_byAgent = productList.qty_planned_byAgent - items.qty;
                    db.SaveChanges();

                }

                if (TruckDispatchMstList.Any())
                {
                    db.Truck_dispatch_details.RemoveRange(TruckDetailsList);
                    db.Truck_dispatches.RemoveRange(TruckDispatchMstList);
                    db.SaveChanges();

                }
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck_DispatchRepostitory->DeleteVehicles:", Ex);

            }

        }

        public List<Truck_dispatches> GetVehicleOnQueueForLocation(int locationId)
        {
            try
            {
                var fromdt = DateTime.Now.Date;
                var todt = Convert.ToDateTime(fromdt.AddHours(36));
                OrderRepository objOrder = new OrderRepository();
                //  var lstOrders = objOrder.GetAllOrdersbyAgentandLocation(id);
                string papermill_location = objOrder.GetPapermillLocation(locationId).Trim();
                var papermills = db.Papermills.Where(p => p.location == papermill_location).ToList();
                int pm_id1 = papermills[0].papermill_id;
                int pm_id2 = papermills[1].papermill_id;
                // int LoggedAgentId = objOrder.GetAgentID();

                var TruckDetailsList = (from td in db.Truck_dispatches
                                        join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                        // join ord in db.Orders on tdd.order_id equals ord.order_id
                                        join opp in db.Order_products on tdd.order_product_id equals opp.order_product_id
                                        join sch in db.Schedule on opp.schedule_id equals sch.schedule_id
                                        where ///(td.estimated_arrival >= fromdt)
                                            // &&
                                                 td.actual_arrival_at_gate == null
                                                 && (td.estimated_arrival <= todt)
                                                 && (sch.papermill_id == pm_id1 || sch.papermill_id == pm_id2)
                                        // && ord.agent_id == LoggedAgentId
                                        //orderby td.estimated_arrival descending
                                        select td).Distinct().OrderByDescending(x => x.estimated_arrival).ToList<Truck_dispatches>();
                return TruckDetailsList;

            }
            catch (Exception Ex)
            {
                logger.Error("Error in Truck-dispatchReprository->GetVehicleOnQueueForLocation:", Ex);
                return null;
            }

        }

        public class tempTruckDispatch
        {
            [Key]
            public int truck_dispatch_id { get; set; }

            public IEnumerable<Papermill> locations { get; set; }
            public int location_id { get; set; } // this is papermill_id
            public string location { get; set; }
            public string address { get; set; }
            public int agent_id { get; set; }
            public string truck_no { get; set; }

            public Nullable<decimal> estimated_capacity { get; set; }
            public DateTime? agent_dispatched_on { get; set; }

            public Nullable<DateTime> estimated_arrival { get; set; }
            public Nullable<DateTime> actual_arrival_at_gate { get; set; }
            public Nullable<DateTime> left_factory_on { get; set; }

            public string status { get; set; }
            public Nullable<DateTime> created_on { get; set; }
            public string created_by { get; set; }

            public virtual ICollection<Truck_dispatch_details> Truckdispatchdetails { get; set; }

        }
        public class tempTruckDispatchDetails
        {
            [Key]
            public int truck_dispatch_details_id { get; set; }
            public int sequenceNumber { get; set; }
            public int order_id { get; set; }

            public int truck_dispatch_id { get; set; }

            public int order_product_id { get; set; }
            public string product_code { get; set; }

            public Nullable<decimal> qty { get; set; }

            public string status { get; set; }

            public string cust_name { get; set; }

            // to display in _OrderSearchCurrentCargoList.cshtml, these fields are needed
            public decimal? width { get; set; }
            public string bf_code { get; set; }
            public string gsm_code { get; set; }
            public string shade_code { get; set; }

        }

        #region Dispose
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}