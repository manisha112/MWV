using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWV.DBContext;
using MWV.Models;
using MWV.Repository.Implementation;
using System.Text.RegularExpressions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Reflection;
using MWV.Business;

namespace MWV.Controllers
{
    public class DispatchController : Controller
    {

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();
        public Truck_dispatchRepository objTd = new Truck_dispatchRepository();
        ProductionPlannerRepository ProObj = new ProductionPlannerRepository();
        private MessageAndAlertsBusiness msgAlertObj = new MessageAndAlertsBusiness();
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
        [HandleModelStateException]
        public ActionResult Index()
        {
            ViewBag.lstAgent = new SelectList(ProObj.GetAgentList(), "agent_id", "name");
            ViewBag.lstProductCode = new SelectList(ProObj.GetProductCodeList(), "product_code", "product_code");
            return View("DashBoard");
        }
        [HandleModelStateException]
        public PartialViewResult VehicleSearchResults()
        {
            Truck_dispatchRepository tdRep = new Truck_dispatchRepository();
            try
            {
                string searchByString = Request["searchByString"];
                string searchString = Request["searchString"];
                DateTime fromDate = new DateTime();
                DateTime toDate = new DateTime();

                if (searchByString == "product-code")
                {
                    fromDate = Convert.ToDateTime(Request["fromDate"]);
                    DateTime todate = Convert.ToDateTime(Request["toDate"]);
                    toDate = todate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    var lstSearchResultByProdCode = tdRep.SearchVehiclesByProdCode(searchByString, searchString, fromDate, toDate);
                    if (!lstSearchResultByProdCode.Any())
                    {
                        ViewBag.NoRecordMsg = "No data available !";
                    }
                    ViewBag.searchresult = lstSearchResultByProdCode;
                    return PartialView("_VehicleSearchResults", lstSearchResultByProdCode);
                }
                else if (searchByString == "vehicle-no")
                {

                    var lstSearchResult = tdRep.SearchVehiclesByAgentIdAndTruckNum(searchByString, searchString, fromDate, toDate);
                    if (!lstSearchResult.Any())
                    {
                        ViewBag.NoRecordMsg = "No data available !";
                    }
                    ViewBag.searchresult = lstSearchResult;
                    //ViewBag.dayText = "Search";         
                    ViewBag.noFilter = "Hidden vehicles filter";
                    return PartialView("_VehicleSearchResults", lstSearchResult);
                }
                else if (searchByString == "agent-name")
                {
                    fromDate = Convert.ToDateTime(Request["fromDate"]);
                    DateTime todate = Convert.ToDateTime(Request["toDate"]);
                    toDate = todate.AddHours(23).AddMinutes(59).AddSeconds(59);
                    var lstSearchResult = tdRep.SearchVehiclesByAgentIdAndTruckNum(searchByString, searchString, fromDate, toDate);
                    if (!lstSearchResult.Any())
                    {
                        ViewBag.NoRecordMsg = "No data available !";
                    }
                    ViewBag.searchresult = lstSearchResult;
                    return PartialView("_VehicleSearchResults", lstSearchResult);
                }
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Dispatch->VehicleSearchResults:", Ex);
                return null;
            }
            return null;
        }
        [HandleModelStateException]
        public PartialViewResult GetFilterVehicleDetails()
        {
            Truck_dispatchRepository tdRep = new Truck_dispatchRepository();
            try
            {
                DateTime fromDate = new DateTime();
                DateTime toDate = new DateTime();
                string searchByString = Request["searchByString"];
                string searchString = Request["searchString"];
                string filterVehicles = Request["filterVehicles"];
                if (searchByString == "product-code")
                {
                    string ProductCode = Request["ProductCode"];
                    searchString = ProductCode;
                    fromDate = Convert.ToDateTime(Request["fromDate"]);
                    toDate = Convert.ToDateTime(Request["toDate"]);
                    var lstSearchResultByProdCode = tdRep.SearchVehiclesByProductcodeWithStatus(searchByString, searchString, filterVehicles, fromDate, toDate);
                    if (!lstSearchResultByProdCode.Any())
                    {
                        ViewBag.NoRecordMsg = "No data available !";
                    }
                    ViewBag.searchresult = lstSearchResultByProdCode;
                    return PartialView("_VehicleSearchResults", lstSearchResultByProdCode);
                }
                else if (searchByString == "vehicle-no")
                {

                    var lstSearchResult = tdRep.SearchVehiclesByAgentIdAndTruckNum(searchByString, searchString, fromDate, toDate);
                    if (!lstSearchResult.Any())
                    {
                        ViewBag.NoRecordMsg = "No data available !";
                    }
                    ViewBag.searchresult = lstSearchResult;
                    return PartialView("_VehicleSearchResults", lstSearchResult);
                }
                else if (searchByString == "agent-name")
                {
                    fromDate = Convert.ToDateTime(Request["fromDate"]);
                    toDate = Convert.ToDateTime(Request["toDate"]);
                    var lstSearchResult = tdRep.SearchVehiclesByAgentAndVehicleStatus(searchByString, searchString, filterVehicles, fromDate, toDate);
                    if (!lstSearchResult.Any())
                    {
                        ViewBag.NoRecordMsg = "No data available !";
                    }
                    ViewBag.searchresult = lstSearchResult;
                    return PartialView("_VehicleSearchResults", lstSearchResult);
                }
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Dispatch->GetFilterVehicleDetails:", Ex);
                return null;
            }
            return null;
        }
        [HandleModelStateException]
        public PartialViewResult VehicleSearchResultsByTruckStatus()
        {
            try
            {
                string searchByString = Request["searchByString"];
                string searchString = Request["searchString"];

                var lstSearchResult = new List<Truck_dispatches>();
                string productCode = string.Empty;
                string vehicleNo = string.Empty;
                DateTime fromDate = Convert.ToDateTime(Request["fromDate"]);
                DateTime toDate = Convert.ToDateTime(Request["toDate"]);

                Truck_dispatchRepository tdRep = new Truck_dispatchRepository();
                if (searchByString == "agent-name")
                {
                    string agentId = Request["AgentId"];
                    lstSearchResult = tdRep.SearchVehicles(agentId, fromDate, toDate);

                }
                else
                    if (searchByString == "product-code")
                    {
                        productCode = Request["ProductCode"];
                        lstSearchResult = tdRep.SearchVehicles(productCode, fromDate, toDate);
                    }
                //else
                //    if (searchByString == "vehicle-no")
                //    {
                //vehicleNo =  Request["ProductCode"];
                //    }


                // var lstSearchResult = tdRep.SearchVehicles(searchByString, fromDate, toDate);
                ViewBag.searchresult = lstSearchResult;
                if (!lstSearchResult.Any())
                {
                    ViewBag.NoRecordMsg = "No data available !";
                }
                return PartialView("_VehicleSearchResults", lstSearchResult);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Dispatch->VehicleSearchResultsByTruckStatus:", Ex);
                return null;
            }
        }
        [HandleModelStateException]
        public PartialViewResult TodaysVehiclesDDLbyStatus()
        {
            try
            {
                string DaySelectedValue = Request["DaySelectedValue"];
                string selectedTextValue = Request["selectedTextValue"];
                if (DaySelectedValue == "Today")
                {
                    ViewBag.dayText = "Today";
                }
                else
                    if (DaySelectedValue == "Tomorrow")
                    {
                        ViewBag.dayText = "Tomorrow";
                    }
                    else
                        if (DaySelectedValue == "NextDay")
                        {
                            ViewBag.dayText = "NextDay";
                        }
                if (selectedTextValue == "Search")
                {
                    if (Request["searchByString"] == "vehicle-no")
                    {
                        ViewBag.flag = "vehicle-no";
                    }
                    ViewBag.dayText = "Search";
                }
                return PartialView("_TodayFilterVechile");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Dispatch->GetFilterVehicleDetails:", Ex);
                return null;
            }
        }
        [HandleModelStateException]
        public PartialViewResult VehicleScheduleDetailsToday()
        {
            try
            {
                DateTime dt = Convert.ToDateTime(Request["VehicleScheduleDate"]);
                string selectedValue = Request["SelectedValue"];
                var lstVehicles = objTd.SearchVehicleByDaysAndVehicleStatus(dt, selectedValue);
                if (!lstVehicles.Any())
                {
                    ViewBag.NoRecordMsg = "No data available !";
                }
                ViewBag.VehiclesToday = lstVehicles;

                return PartialView("_VehicleScheduleDetails");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Dispatch->VehicleScheduleDetailsToday:", Ex);
                return null;
            }
        }
        [HandleModelStateException]
        public PartialViewResult VehicleScheduleDetailsByTomorrow()
        {
            try
            {
                DateTime dt = Convert.ToDateTime(Request["VehicleScheduleDate"]);
                string selectedValue = Request["SelectedValue"];
                var lstVehicles = objTd.SearchVehicleByDaysAndVehicleStatus(dt, selectedValue);
                if (!lstVehicles.Any())
                {
                    ViewBag.NoRecordMsg = "No data available !";
                }
                ViewBag.VehiclesToday = lstVehicles;

                return PartialView("_VehicleScheduleDetails");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Dispatch->VehicleScheduleDetailsByTomorrow:", Ex);
                return null;
            }
        }
        [HandleModelStateException]
        public PartialViewResult VehicleScheduleDetailsByNextDay()
        {
            try
            {
                DateTime dt = Convert.ToDateTime(Request["VehicleScheduleDate"]);
                string selectedValue = Request["SelectedValue"];
                var lstVehicles = objTd.SearchVehicleByDaysAndVehicleStatus(dt, selectedValue);
                if (!lstVehicles.Any())
                {
                    ViewBag.NoRecordMsg = "No data available !";
                }
                ViewBag.VehiclesToday = lstVehicles;

                return PartialView("_VehicleScheduleDetails");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Dispatch->VehicleScheduleDetailsByNextDay:", Ex);
                return null;
            }
        }
        [HandleModelStateException]
        public PartialViewResult SearchVehicleSToday()
        {
            try
            {
                // MWVDBContext db = new MWVDBContext();
                DateTime dt = Convert.ToDateTime(Request["VehicleScheduleDate"]);
                string selectedValue = Request["SelectedValue"];
                var lstVehicles = objTd.SearchVehicleByDaysAndVehicleStatus(dt, selectedValue);
                if (!lstVehicles.Any())
                {
                    ViewBag.NoRecordMsg = "No data available !";
                }
                ViewBag.VehiclesToday = lstVehicles;

                return PartialView("_VehiclesFiltter");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Dispatch->SearchVehicleSToday:", Ex);
                return null;
            }
        }
        [HandleModelStateException]
        public PartialViewResult VehicleDispatchDetails()
        {
            try
            {
                int tdId = Convert.ToInt16(Request["tdId"]);
                var Vehicle = db.Truck_dispatches.Where(k => k.truck_dispatch_id == tdId).ToList();
                var rollDetails = (from td in db.Truck_dispatches
                                   join tdd in db.Truck_dispatch_details on td.truck_dispatch_id equals tdd.truck_dispatch_id
                                   join pc in db.ProductionChild on tdd.order_product_id equals pc.order_product_id
                                   where pc.actual_end != null && pc.actual_loaded_on == null && td.truck_dispatch_id == tdId
                                   select pc).ToList();
                ViewBag.rollDetails = rollDetails;
                ViewBag.Vehicle = Vehicle;
                int cargocount = 0;
                foreach (var item in Vehicle)
                {
                    cargocount = item.Truckdispatchdetails.Count;
                }

                ViewBag.cargocount = cargocount;

                return PartialView("_VehicleDispatchDetails", Vehicle);
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Transportation->VehicleDispatchDetails:", Ex);
                return null;
            }
        }
        [HttpPost]
        [HandleModelStateException]
        public void DispatchComplete(string allVals)
        {
            OrderRepository orObj = new OrderRepository();
            StockRepository stockObj = new StockRepository();

            try
            {
                string CurrentUserid = User.Identity.GetUserId();
                var currentUserName = db.AspNetUsers.Where(p => p.Id == CurrentUserid).Select(x => x.name).SingleOrDefault();

                string[] strCheckedValue = Regex.Split(allVals, ",").Skip(1).ToArray();  //all reels no which are marking as dispathced
                int truckdispatchid = Convert.ToInt16(allVals.Split(',')[0]);  //arr[0] is the truck dispatched id
                var truckDispatch = db.Truck_dispatches.Where(c => c.truck_dispatch_id == truckdispatchid).FirstOrDefault();
            
                objTd.SaveReelsAsDispatched(allVals); //reel mark as loaded on truck
                objTd.SaveOrdeProductQty(allVals);



                var truckDispatchDetails = db.Truck_dispatch_details.Where(c => c.truck_dispatch_id == truckdispatchid).ToList();

                foreach (var tdd_id in truckDispatchDetails)
                {
                    int orderProdId = Convert.ToInt16(tdd_id.order_product_id);

                    var tdd_details = db.Truck_dispatch_details
                                      .Where(c => c.truck_dispatch_details_id == tdd_id.truck_dispatch_details_id
                                       && c.order_product_id == orderProdId).FirstOrDefault();


                    //tdd_details.qty_loaded = tdd_details.qty;
                    db.SaveChanges();


                    UpdateOrderProductQty(orderProdId, tdd_details.qty_loaded);

                    var orderDetails = (from op in db.Order_products
                                        join ord in db.Orders on op.order_id equals ord.order_id
                                        join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                        where op.order_product_id == orderProdId
                                        select new { op, sch, ord }).SingleOrDefault();

                    //This function is used for when all orderproduct is in same status then orderstatus should be changed as ordeproduct status
                    orObj.MatchedOrderStatusWithOrdPro(0, orderProdId);

                    //This function is supposed to Add the Stock being dispatched
                    stockObj.StockDispatched(DateTime.Now, (int)orderDetails.sch.papermill_id, (int)orderDetails.ord.agent_id, (int)orderDetails.ord.customer_id, orderDetails.op.product_code, orderDetails.op.shade_code, (decimal)orderDetails.op.qty);
                }

                truckDispatch.status = "Dispatched";
                db.SaveChanges();

                //AlertforTruckLoaded(truckDispatch.agent_id, currentUserName, truckDispatch.truck_no);
                //MsgAndMailforTruckLoaded(truckDispatch.agent_id, truckDispatch.truck_no, currentUserName, "TruckDispatched");
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Dispatch->GetFilterVehicleDetails:", Ex);
            }
        }



        private bool AlertforTruckLoaded(int? agentid, string currentUserName, string truckNo)
        {
            try
            {
                string alertText = "Truck " + truckNo + " has been loaded and Dispatched";
                string alertSubject = "Truck " + truckNo + " has been loaded and Dispatched";

                msgAlertObj.CreateAlertDetails("TruckDispatched", "Agent", agentid, alertText, alertSubject, currentUserName, "");
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in GateKeeper->AlertforTruckLoaded:", Ex);
                return false;
            }
        }

        private bool MsgAndMailforTruckLoaded(int? agentid, string truckNo, string CreatedBy, string status)
        {
            try
            {

                var agentMailAddress = db.Agents.Where(p => p.agent_id == agentid).SingleOrDefault();
                string msgtxt = "Truck " + truckNo + " has been loaded and Dispatched";
                string msgSubject = "Truck " + truckNo + " has been loaded and Dispatched";
                string msgAction = "TruckDispatched";
                string recipient1 = ""; //agentMailAddress.email;
                string cc1 = "";
                string bcc1 = "";
                string attachment1 = "";
                // msgAlertObj.CreateMessagesDetails(msgAction, "Agent", agentid, msgtxt, msgSubject, recipient1, cc1, bcc1, CreatedBy, status,"",null,null);
                msgAlertObj.CreateMessagesDetails(msgAction, "Agent", agentid, msgtxt, msgSubject, "", "", "", CreatedBy, "", status, null, null);
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Dispatch->MsgAndMailforTruckLoaded:", Ex);
                return false;
            }
        }

        public void UpdateOrderProductQty(int orderProdId, decimal? qtyLoaded)
        {
            try
            {

                //Get the row from DB
                Order_Products OrdPrd = (from Order_Products in db.Order_products
                                         where Order_Products.order_product_id == orderProdId
                                         select Order_Products).First();

                if (OrdPrd != null)
                {
                    OrdPrd.qty_dispatched_byFactory = OrdPrd.qty_dispatched_byFactory + qtyLoaded;
                    if (OrdPrd.qty == OrdPrd.qty_dispatched_byFactory)
                    {
                        OrdPrd.status = "Dispatched";   // mark orderproduct as Dispatched if all of the QTY is planned

                    }
                    db.Entry(OrdPrd).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();


                    Order order = (from Order in db.Orders
                                   where Order.order_id == OrdPrd.order_id
                                   select Order).First();
                    if (order != null)
                    {
                        var orderprod = (from Order_Products in db.Order_products
                                         where Order_Products.order_id == order.order_id
                                         && (Order_Products.status == "Under Planning"
                                         || Order_Products.status == "Planned" || Order_Products.status == "In Warehouse")
                                         select Order_Products).ToList<Order_Products>();

                        //if there are still order products with under planning, planned  status then we skip marking it as planned
                        if (orderprod == null)
                        {
                            order.status = "Dispatched";
                            db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                        }

                        //Update Orders and mark them as deckled

                    }

                }
            }
            catch (Exception Ex)
            {
                logger.Error("Error in Deckle->UpdateOrderProductQty:", Ex);
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
