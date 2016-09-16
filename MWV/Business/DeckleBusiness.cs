using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWV.Repository.Implementation;
using MWV.Repository.Interfaces;
using MWV.Models;
using System.Reflection;
using MWV.Business.Deckle;

namespace MWV.Business
{
    public class DeckleBusiness
    {
        //readonly log4net.ILog logger = log4net.LogManager.GetLogger("RollingLogFileAppender");

        readonly log4net.ILog decklelogger = log4net.LogManager.GetLogger("DeckleFileAppender");

        private IDeckle _Deckle;
        private DateTime DeckleCalcUpto = System.DateTime.Now.AddDays(30).Date;

        //Constructer to initialize Interface class
        public DeckleBusiness(IDeckle _Deckle)
        {
            this._Deckle = _Deckle;
        }
        public string CalculateDeckle()
        {
            decklelogger.InfoFormat("Deckle calculation started at " + System.DateTime.Now.ToString());
            // Deckle needs to be calculated for each of the PaperMill Machine

            // Logic for calculating Deckle
            // - Pull all Products where orderstatus = confirmed and requestdate = currentdate+1
            // - Get the machine details to get Jumbo weight and sizes
            // - Get the Lowest Weight to be produced 

            // Get a list of all Papermills
            //List<Papermill> lstPaperMills = _Deckle.GetPaperMills();

            //Get a list of all Active Schedules
            List<Schedule> lstSchedules = _Deckle.GetActiveSchedules();

            string returnPR_IDs = "";
            //foreach(Papermill papermill in lstPaperMills)
            foreach (Schedule schedule in lstSchedules)
            {

                //We need a new object of Schedule as the last calculation may have extended the start dates of the schedule
                //the Schedule object risks of having outdated information, so everytime we get a new Schedule object which we use in calcualtion

                Schedule UpdatedSchedule = _Deckle.GetScheduleByID(schedule.schedule_id);

                Papermill papermill = _Deckle.GetPaperMill(UpdatedSchedule.papermill_id);

                //Log Papermill data
                LogPaperMill(papermill, UpdatedSchedule);

                //calculate new End date for which orders should be picked
                DeckleCalcUpto = (DateTime)UpdatedSchedule.end_date;

                // Get Listing of all Deckle ready orders/Products for the machine
                List<Order_Products> lstOrderProducts = _Deckle.GetOrders(UpdatedSchedule.schedule_id, "Under Planning", DeckleCalcUpto);

                if (lstOrderProducts.Count > 0) 
                {

                    MakePendingSameAsOrder(lstOrderProducts);  //Make all pending qty same as orderqty

                    LogAllOrderProducts(lstOrderProducts);  // Log all Products found to be used in Deckle Calculation

                    decklelogger.InfoFormat("List sorted by Requested Date. data pulled upto " + DeckleCalcUpto.ToShortDateString());
                    lstOrderProducts = SortbyRequestedDate(lstOrderProducts);  //sort the List 

                    //decklelogger.InfoFormat("List sorted by Higest Weight");
                    //lstOrderProducts = SortbyHighestWeight(lstOrderProducts);  //sort the List 
                    LogAllOrderProducts(lstOrderProducts); // see the sorted list

                    //fill products to be passed to Calculation as itemsAvailable 
                    List<Item> itemsAvailable = new List<Item>();

                    foreach(Order_Products orderproduct in lstOrderProducts)
                    {
                        Item item = new Item();
                        item.BF = orderproduct.deckleBFCode;
                        item.GSM = orderproduct.deckleGSMCode;
                        item.OrderProductNo = orderproduct.order_product_id;
                        item.Shade = orderproduct.shade_code;
                        item.Size = (decimal) orderproduct.SizePending;
                        item.OrderQty = (decimal) orderproduct.qty - (decimal) orderproduct.qty_scheduled;
                        item.PendingQty = item.OrderQty;  //Make pending Qty same as orderqty - used later in Calc
                        item.RequestedDate = orderproduct.RequestedDate;
                        itemsAvailable.Add(item);

                    }

                    Decklecalc CalculateDeckle = new Decklecalc(new DeckleRepository(), itemsAvailable, papermill, UpdatedSchedule);

                    int PR_id = CalculateDeckle.CalculateDeckle();
                    if (PR_id > 0)
                    {
                        if(returnPR_IDs == "")
                            returnPR_IDs = PR_id.ToString();
                        else
                            returnPR_IDs += "," +  PR_id.ToString(); 
                    }
                }

            }
            decklelogger.InfoFormat("Deckle calculation Stopped at " + System.DateTime.Now.ToString());
            decklelogger.InfoFormat("============================================================================");
            return returnPR_IDs;
        }

        //private List<Order_Products> FindProducttoFitDeckle(List<Order_Products> lstOrderProducts)
        //{
        //    foreach (Order_Products orderproduct in lstOrderProducts)
        //    {
        //    return lstOrderProducts;
        //}

        private List<Order_Products> SortbyLowestWeight(List<Order_Products> lstOrderProducts)
        {
            List<Order_Products> sortedProducts = lstOrderProducts.OrderBy(o => o.QtyPending).ToList();
            return sortedProducts;
        }

        private List<Order_Products> SortbyHighestWeight(List<Order_Products> lstOrderProducts)
        {
            List<Order_Products> sortedProducts = lstOrderProducts.OrderByDescending(o => o.QtyPending).ToList();
            return sortedProducts;
        }
        private List<Order_Products> SortbyRequestedDate(List<Order_Products> lstOrderProducts)
        {
            List<Order_Products> sortedProducts = lstOrderProducts.OrderBy(o => o.RequestedDate).ToList();
            return sortedProducts;
        }
        
        private List<Order_Products> MakePendingSameAsOrder(List<Order_Products> lstOrderProducts)
        {
            
            lstOrderProducts.Select(c => { c.QtyPending = (decimal) c.qty; return c; }).ToList();
            lstOrderProducts.Select(c => { c.SizePending = (decimal)c.width; return c; }).ToList();
            
            return lstOrderProducts;
        }

        //public ProductionTimeline GetProductionTimeline(int Papermill, String BF, String GSM, String Shade)
        //{
        //    return _Deckle.GetProductionTimeline(Papermill, BF, GSM, Shade);
        //}

        private void LogPaperMill(Papermill lstPaperMill, Schedule Schedule)  //
        {
            decklelogger.InfoFormat("....Running Calculation for Schedule: " + Schedule.shade_code + " on Papermill " + lstPaperMill.name + " START: " + Schedule.start_date.ToString() + " END: " + Schedule.end_date.ToString()  + "..................");
            decklelogger.InfoFormat("PaperMill ID    : " + lstPaperMill.papermill_id + "         \t MAX CUTS: " + lstPaperMill.max_cuts.ToString());
            decklelogger.InfoFormat("PaperMill Name  :" + lstPaperMill.name + "," + lstPaperMill.location);
            decklelogger.InfoFormat("Capacity        :" + lstPaperMill.capacity.ToString() + "         \t Max Child Weight: " + lstPaperMill.max_weight_child.ToString());
           // decklelogger.InfoFormat("Jumbo  Width MIN: " + lstPaperMill.min_width.ToString() + " \t MAX: " + lstPaperMill.max_width.ToString() );
            decklelogger.InfoFormat("Deckle Width MIN: " + lstPaperMill.deckle_min.ToString() + " \t MAX: " + lstPaperMill.deckle_max.ToString());
            decklelogger.InfoFormat("Diameter  MIN   : " + lstPaperMill.min_diameter.ToString() + " \t MAX: " + lstPaperMill.max_diameter.ToString());
            decklelogger.InfoFormat("Jumbo weight MIN: " + lstPaperMill.min_weight_jumbo.ToString() + " \t MAX: " + lstPaperMill.max_weight_jumbo.ToString());
            decklelogger.InfoFormat("--------------");

        }
        private void LogAllOrderProducts(List<Order_Products> lstOrderProducts)
        {
            decklelogger.InfoFormat("Number of Orders found ....." + lstOrderProducts.Count.ToString());
            decklelogger.InfoFormat(String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10} {6,-10} {7,-10} {8,-10} {9,-10} {10,-10} {11,-10}",
                                             "Ord_ID", "OrdPrdID", "Cust_ID", "BF", "GSM", "Shade", "Width", "Qty", "Diameter", "QtyPending", "SizePending", "Req Date"));
            decklelogger.InfoFormat(String.Format("{0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-11}",
                                             "----------"));
            foreach (Order_Products orderproduct in lstOrderProducts)
            {
                decklelogger.InfoFormat(String.Format("{0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10} {6,-10} {7,-10} {8,-10} {9,-10} {10,-10} {11,-10}",
                                             orderproduct.order_id,
                                             orderproduct.order_product_id,
                                             orderproduct.deckleCustomerId,
                                             orderproduct.deckleBFCode,
                                             orderproduct.deckleGSMCode,
                                             orderproduct.shade_code,
                                             orderproduct.width,
                                             orderproduct.qty,
                                             orderproduct.diameter,
                                             orderproduct.QtyPending,
                                             orderproduct.SizePending,
                                             orderproduct.RequestedDate.ToShortDateString()
                                             ));

            }
            decklelogger.InfoFormat(" ");

        }
    }
}