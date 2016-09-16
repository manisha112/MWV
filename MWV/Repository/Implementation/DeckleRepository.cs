using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWV.Repository.Interfaces;
using System.Reflection;
using MWV.Models;
using MWV.DBContext;
using System.Data.SqlClient;
using System.Collections;

namespace MWV.Repository.Implementation
{
    public class DeckleRepository : IDeckle
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private MWVDBContext db;

        public DeckleRepository()
        {

        }

        #region GetOrders
        /// <summary>
        /// This Function helps to get a list of Products in Open Orders for the Deckle Matching
        /// </summary>
        /// <param name="PapermillId"></param>
        /// <param name="Status"></param>
        /// <param name="RequestDate"></param>
        /// <returns>Strongly typed Product List</returns>
        public List<Order_Products> GetOrders(int Scheduleid, string Status, DateTime RequestDate) //change status to ENUM class
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    // get a list of all orders with the status mentioned in request
                    // join with order table and select orders where papermillID matches request
                    //  and RequestDate >= requestDate in request

                    var ProductList = (from Order_Products in db.Order_products
                                       //join Order in db.Orders on Order_Products.order_id equals Order.order_id
                                       where Order_Products.status == Status && Order_Products.requested_delivery_date <= RequestDate
                                       && Order_Products.schedule_id == Scheduleid
                                           //&& Order.papermill_id == PapermillId
                                       && Order_Products.qty > Order_Products.qty_scheduled
                                       //orderby Order_product.Bf descending, Order_product.Gsm descending, Order_product.shade_code
                                       select Order_Products).ToList<Order_Products>();
                    //select new {RequestedDate = Order.requested_delivery_date};

                    //store the customer_id, gsmcode and bfcode in temporary variables as the virtual objects wont be available later
                    if (ProductList != null)
                    {
                        foreach (Order_Products orderproduct in ProductList)
                        {
                            orderproduct.deckleCustomerId = (int)orderproduct.order.customer_id;
                            orderproduct.RequestedDate = (DateTime)orderproduct.requested_delivery_date;
                            orderproduct.deckleBFCode = orderproduct.Product.bf_code;
                            orderproduct.deckleGSMCode = orderproduct.Product.gsm_code;
                        }
                    }
                    return ProductList;

                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->GetOrders:", Ex);
                    return null;
                }
            }
        }
        #endregion

        public Papermill GetPaperMill(int papermillid)
        {
            using (db = new MWVDBContext())
            {
                try
                {

                    var papermill = (from Papermill in db.Papermills
                                     where Papermill.papermill_id == papermillid
                                     select Papermill).FirstOrDefault();

                    return papermill;

                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->GetPaperMill:", Ex);
                    return null;
                }
            }
        }

        public List<Schedule> GetActiveSchedules()
        {
            using (db = new MWVDBContext())
            {
                try
                {

                    var Schedules = (from Schedule in db.Schedule
                                     where Schedule.status == "Active"
                                     select Schedule).ToList<Schedule>();

                    return Schedules;

                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->GetActiveSchedules:", Ex);
                    return null;
                }
            }
        }

        public Schedule GetScheduleByID(int Scheduleid)
        {
            using (db = new MWVDBContext())
            {
                try
                {

                    var schedule = (from Schedule in db.Schedule
                                    where Schedule.schedule_id == Scheduleid
                                    select Schedule).FirstOrDefault();

                    return schedule;

                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->GeScheduleByID:", Ex);
                    return null;
                }
            }
        }

        public List<Schedule> GetActiveSchedulesforPapermill(int papermillid)
        {
            using (db = new MWVDBContext())
            {
                try
                {

                    var Schedules = (from sch in db.Schedule
                                     join pr in db.ProductionRun on sch.schedule_id equals pr.schedule_id into xtemp
                                     from runsch in xtemp.DefaultIfEmpty()
                                     where sch.status == "Active" && sch.papermill_id == papermillid
                                     select new
                                     {
                                         schedule_id = sch.schedule_id,
                                         papermill_id = sch.papermill_id,
                                         shade_code = sch.shade_code,
                                         start_date = sch.start_date,
                                         end_date = sch.end_date,
                                         TotalRuntime = xtemp.Sum(x => (int?)x.run_time) ?? 0
                                     })
                        //.GroupBy(x => x.schedule_id)
                                                .AsEnumerable()
                                                .Select(x => new Schedule
                                                    {
                                                        schedule_id = x.schedule_id,
                                                        papermill_id = x.papermill_id,
                                                        shade_code = x.shade_code,
                                                        start_date = x.start_date,
                                                        end_date = x.end_date,
                                                        TotalRuntime = x.TotalRuntime
                                                    })
                                                   .OrderBy(x => x.start_date)
                                                   .GroupBy(x => x.schedule_id)
                                                   .Select(x => x.FirstOrDefault());
                    //.ToList();
                    //var objs = (from sch in Schedules
                    //            orderby sch.schedule_id
                    //            select sch).GroupBy(g => g.schedule_id).Select(x => x.FirstOrDefault());

                    List<Schedule> SchduleList = new List<Schedule>();
                    foreach (var items in Schedules)
                    {
                        Schedule obj = new Schedule();
                        obj.schedule_id = items.schedule_id;
                        obj.papermill_id = items.papermill_id;
                        obj.shade_code = items.shade_code;
                        obj.start_date = items.start_date;
                        obj.end_date = items.end_date;
                        obj.TotalRuntime = items.TotalRuntime;
                        SchduleList.Add(obj);
                    }
                    //for (int i = 0; i < Schedules.Count; i++)
                    //{
                    //    var query = Schedules[0];
                    //}
                    return SchduleList;
                    //var xSchedules = (from sch in db.Schedule
                    //                            join pr in db.ProductionRun on sch.schedule_id equals pr.schedule_id into xtemp
                    //                            from runsch in xtemp.DefaultIfEmpty()
                    //                            where sch.status == "Active" && sch.papermill_id == papermillid
                    //                            select new
                    //                            {
                    //                                schedule_id = sch.schedule_id,
                    //                                papermill_id = sch.papermill_id,
                    //                                shade_code = sch.shade_code,
                    //                                start_date = sch.start_date,
                    //                                end_date = sch.end_date,
                    //                                TotalRuntime = xtemp.Sum(x => (int?)x.run_time) ?? 0
                    //                            })
                    //                            .GroupBy(x => x.schedule_id)
                    //                            .AsEnumerable()
                    //                            .ToList();

                    //List<Schedule>  Schedules = (from sch in xSchedules)
                    //                 .Select(x => new Schedule
                    //                 {
                    //                    schedule_id = x.schedule_id,
                    //                    papermill_id = x.papermill_id,
                    //                    shade_code = x.shade_code,
                    //                    start_date = x.start_date,
                    //                    end_date = x.end_date,
                    //                    TotalRuntime = x.TotalRuntime
                    //                 })
                    //                 .OrderBy(x=> x.start_date)
                    //                 .ToList();



                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->GetActiveSchedules:", Ex);
                    return null;
                }
            }
        }


        public ProductionTimeline GetProductionTimeline(int Papermill, String BF, String GSM, String Shade)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    ProductionTimeline productionTimeline = (from ProductionTimeline in db.ProductionTimeline
                                                             where ProductionTimeline.bf_code == BF && ProductionTimeline.gsm_code == GSM &&
                                                                   ProductionTimeline.shade_code == Shade && ProductionTimeline.papermill_id == Papermill
                                                             select ProductionTimeline).FirstOrDefault();
                    return productionTimeline;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->GetProductionTimeline:", Ex);
                    return null;
                }

            }

        }

        public ProductionRun GetLastRunEstimateEndTime(int Papermill, int ScheduleID)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    ProductionRun productionrun = (from ProductionRun in db.ProductionRun
                                                   where ProductionRun.schedule_id == ScheduleID
                                                   orderby ProductionRun.estimated_end descending
                                                   select ProductionRun).FirstOrDefault();
                    return productionrun;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->GetLastRunEstimateEndTime:", Ex);
                    return null;
                }
            }
        }

        public string GetLowerDeckleCombination(string BF, string GSM, string Shade)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    var combo = (from Product in db.Products
                                 join Product_prices in db.Product_prices on Product.product_code equals Product_prices.product_code
                                 where Product.gsm_code == GSM && Product_prices.shade_code == Shade
                                 orderby Product.bf_code descending
                                 select Product).ToList<Product>();
                    //find the BF and take the next record

                    bool found = false;
                    string nextBF = "";
                    foreach (Product prd in combo)
                    {
                        if (found && nextBF == "")
                        {
                            nextBF = prd.bf_code;
                        }
                        if (prd.bf_code == BF)
                            found = true;
                    }

                    if (nextBF == "")
                        return "";
                    else
                        return nextBF;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->GetLowerDeckleCombination:", Ex);
                    return null;
                }
            }
        }

        public int CreateProductionRun(ProductionRun productionrun, List<ProductionJumbo> ProductionJumboList)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    // add to the database and return the ProductionrunID
                    db.ProductionRun.Add(productionrun);
                    db.SaveChanges();
                    int lastinsertedId = productionrun.pr_id;

                    foreach (ProductionJumbo prdjumbo in ProductionJumboList)
                    {
                        prdjumbo.pr_id = lastinsertedId;
                        db.ProductionJumbo.Add(prdjumbo);
                        db.SaveChanges();
                    }
                    // and return that

                    return lastinsertedId;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->CreateProductionRun:", Ex);
                    return 0;
                }

            }
        }

        public int CreateProductionRunChilds(List<ProductionChild> ProductionChildList)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    foreach (ProductionChild prdchild in ProductionChildList)
                    {
                        db.ProductionChild.Add(prdchild);
                        db.SaveChanges();
                        prdchild.child_rollno = "Dummy" + prdchild.pc_id.ToString();
                        db.Entry(prdchild).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                    return 1;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->CreateProductionRunChilds:", Ex);
                    return 0;
                }

            }
        }

        public bool UpdateOrderProductQty(int OrderProductNo, decimal MasterJumboQty)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    //Get the row from DB
                    Order_Products OrdPrd = (from Order_Products in db.Order_products
                                             where Order_Products.order_product_id == OrderProductNo
                                             select Order_Products).First();

                    if (OrdPrd != null)
                    {
                        OrdPrd.qty_scheduled = OrdPrd.qty_scheduled + MasterJumboQty;
                        if (OrdPrd.qty_scheduled == OrdPrd.qty)
                        {
                            OrdPrd.status = "Planned";   // mark orderproduct as Planned if all of the QTY is planned
                        }
                        db.Entry(OrdPrd).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        //Update Orders and mark them as deckled

                        Order order = (from Order in db.Orders
                                       where Order.order_id == OrdPrd.order_id
                                       select Order).First();
                        if (order != null)
                        {
                            if (order.is_deckled != true)
                            {
                                order.is_deckled = true;
                                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                            }

                            // Check if all Products have been marked as Planned
                            // see if there are any order products with Under planning status,
                            // if yes then we cannot mark the order as Planned

                            var orderprod = (from Order_Products in db.Order_products
                                             where Order_Products.order_id == order.order_id && Order_Products.status == "Under Planning"
                                             select Order_Products).ToList<Order_Products>();

                            //if there are still order products with under planning status then we skip marking it as planned
                            if (orderprod == null)
                            {
                                order.status = "Planned";
                                db.Entry(order).State = System.Data.Entity.EntityState.Modified;
                            }
                            db.SaveChanges();

                        }

                        return true;
                    }
                    return false;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->UpdateOrderProductQty:", Ex);
                    return false;
                }

            }
        }

        //--------------------------------
        // THis function has not been tested
        // ---------code merged in  UpdateOrderProductQty function above
        //Mark Orders as Planned when all OrderProducts have been marked as Planned
        //public bool MarkOrdersAsPlanned()
        //{
        //    using (db = new MWVDBContext())
        //    {
        //        List<int> Ords = new List<int> { 22, 44, 66 };
        //        try
        //        {
        //            //Get a list of all orders for which the order products were updated
        //            var orders = (from Order_Products in db.Order_products
        //                          join Order in db.Orders on Order_Products.order_id equals Order.order_id
        //                          where Ords.Any(o => o == Order_Products.order_product_id)
        //                          select Order).ToList<Order>().Distinct<Order>();

        //            //loop through each order and see if there are any order prdoducts with Under planning status,
        //            // if yes then we cannot mark the order as Planned
        //            foreach( Order ord in orders)
        //            {
        //                var orderprod = (from Order_Products in db.Order_products
        //                                 where Order_Products.order_id == ord.order_id && Order_Products.status == "Under Planning"
        //                                 select Order_Products).ToList<Order_Products>() ;
        //                //if there are still order products with under planning status then we skip marking it as planned
        //                if (orderprod == null)
        //                {
        //                    ord.status = "Planned";
        //                    db.Entry(ord).State = System.Data.Entity.EntityState.Modified;
        //                    db.SaveChanges();
        //                }

        //            }
        //            return true;
        //        }

        //        catch (Exception Ex)
        //        {
        //            logger.Error("Error in Deckle->MarkOrdersAsPlanned:", Ex);
        //            return false;
        //        }
        //    } 
        //}


        public int GetLastJumboNo()
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    var productionjumbo = db.ProductionJumbo.Max(x => x.jumbo_no);
                    if (productionjumbo.Value == null) //if there is no data in the DB
                        return 0;
                    else
                        return productionjumbo.Value;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->GetLastJumboNo:", Ex);
                    return 0;
                }
            }
        }

        public int CreateDeckleApprovals(List<deckle_approvals> approvals)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    foreach (deckle_approvals approval in approvals)
                    {
                        db.deckle_approvals.Add(approval);
                        db.SaveChanges();
                    }

                    return 1;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->CreateDeckleApprovals:", Ex);
                    return 0;
                }

            }
        }

        public List<deckle_approvals> GetDeckleApprovals(int MillNo, string BF, string GSM, string Shade, string Action)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    // Valid Actions "Send To Lower Bf" & "Send To Production"
                    var approvals = (from deckle_approvals in db.deckle_approvals
                                     where deckle_approvals.papermill_id == MillNo && deckle_approvals.bf_code == BF
                                           && deckle_approvals.gsm_code == GSM && deckle_approvals.shade_code == Shade
                                           && deckle_approvals.action == Action
                                           && deckle_approvals.used == null
                                     orderby deckle_approvals.required_size descending
                                     select deckle_approvals).ToList<deckle_approvals>();

                    return approvals;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->GetDeckleApprovals:", Ex);
                    return null;
                }

            }
        }
        public List<dynamic> GetDeckleApprovalsCreatedbetween(DateTime startdate, DateTime enddate)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    var approvals = (from da in db.deckle_approvals
                                     where da.request_date > startdate && da.request_date < enddate
                                     group da by new { da.bf_code, da.gsm_code, da.shade_code,da.required_size } into h
                                     //orderby h.FirstOrDefault().papermill_id
                                     select new
                                     {
                                         bf_code = h.FirstOrDefault().bf_code,
                                         gsm_code = h.FirstOrDefault().gsm_code,
                                         shade_code = h.FirstOrDefault().shade_code,
                                         required_size = h.FirstOrDefault().required_size,
                                         required_weight = h.Sum(k => k.required_weight)
                                     }).ToList();

                    List<dynamic> dynamicList = approvals.Select(x => (dynamic)x).ToList();
                    return dynamicList;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->GetDeckleApprovalsCreatedbetween:", Ex);
                    return null;
                }

            }
        }

        public int UpdateDeckleApprovals(int deckleApprovalId)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    deckle_approvals approval = (from deckle_approvals in db.deckle_approvals
                                                 where deckle_approvals.dm_id == deckleApprovalId
                                                 select deckle_approvals).First();
                    if (approval != null)
                    {
                        approval.used = 1;
                        approval.used_on = DateTime.Now;
                        db.SaveChanges();
                        return 1;
                    }
                    else
                        return 0;

                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->UpdateDeckleApprovals:", Ex);
                    return 0;
                }

            }
        }

        public int UpdateScheduleEndDate(int scheduleid, DateTime NewEnddate)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    Schedule sch = (from Sch in db.Schedule
                                    where Sch.schedule_id == scheduleid
                                    select Sch).First();
                    if (sch != null)
                    {
                        sch.end_date = NewEnddate;
                        db.SaveChanges();
                        return 1;
                    }
                    else
                        return 0;

                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->UpdateScheduleEndDate:", Ex);
                    return 0;
                }

            }
        }

        public int DelayProductionPlansByMinsforSchedule(int Scheduleid, int DelayinMinutes)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    //Get all Production Plans belonging to a schedule

                    var prodplan = (from Prodplan in db.ProductionRun
                                    where Prodplan.schedule_id == Scheduleid
                                    select Prodplan).ToList();
                    if (prodplan != null)
                    {
                        foreach (ProductionRun pr in prodplan)
                        {
                            DateTime newstart = (DateTime)pr.estimated_start;
                            DateTime newend = (DateTime)pr.estimated_end;
                            pr.estimated_start = newstart.AddMinutes(DelayinMinutes);
                            pr.estimated_end = newend.AddMinutes(DelayinMinutes);
                            db.Entry(pr).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();

                            //Get all Production Jumboes for the Production run 
                            var prodjumboes = (from Prodjumbo in db.ProductionJumbo
                                               where Prodjumbo.pr_id == pr.pr_id
                                               select Prodjumbo).ToList();
                            if (prodjumboes != null)
                            {
                                foreach (ProductionJumbo pj in prodjumboes)
                                {
                                    DateTime newstartpj = (DateTime)pj.estimated_start;
                                    pj.estimated_start = newstartpj.AddMinutes(DelayinMinutes);
                                    db.Entry(pj).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();

                                }

                            }
                        }

                        return 1;
                    }
                    else
                        return 0;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->DelayProductionPlansByMinsforSchedule:", Ex);
                    return 0;
                }

            }

        }


        public int UpdateScheduleStartDate(int scheduleid, DateTime NewStartdate)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    Schedule sch = (from Sch in db.Schedule
                                    where Sch.schedule_id == scheduleid
                                    select Sch).First();
                    if (sch != null)
                    {
                        sch.start_date = NewStartdate;
                        db.SaveChanges();
                        return 1;
                    }
                    else
                        return 0;

                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->UpdateScheduleStartDate:", Ex);
                    return 0;
                }

            }
        }

        public string GetLowerBF(string BFcode)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    var Bfs = (from Bf in db.Bfs
                               orderby Bf.bf_code descending
                               select Bf).ToList<Bf>();

                    bool found = false;
                    string returnval = "";
                    foreach (var BF in Bfs)
                    {
                        if (found)
                        {
                            returnval = BF.bf_code;
                            break;
                        }

                        if (BF.bf_code == BFcode)
                            found = true;
                    }
                    if (returnval == "")
                        return "";
                    else
                        return returnval;

                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Deckle->GetLowerBF:", Ex);
                    return "";
                }

            }

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