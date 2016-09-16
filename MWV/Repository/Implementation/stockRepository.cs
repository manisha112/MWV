using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWV.Models;
using MWV.DBContext;
using MWV.Repository.Interfaces;
using System.Reflection;
using MWV.ViewModels;

namespace MWV.Repository.Implementation
{
    public class StockRepository : IStock
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private MWVDBContext db ;

        public StockRepository()
        {

        }

        //This function is supposed to Add the Stock being produced  
        public int StockProduced(DateTime currDate, int PapermillId, int Agent_id, int Customer_id, string Product_code, string Shade_code, decimal StockProduced)
        {
            return StockEntry(currDate.Date, PapermillId, Agent_id, Customer_id, Product_code, Shade_code, StockProduced, 0);
        }

        //This function is supposed to Add the Stock being dispatched
        public int StockDispatched(DateTime currDate, int PapermillId, int Agent_id, int Customer_id, string Product_code, string Shade_code, decimal StockDispatched)
        {
            return StockEntry(currDate.Date, PapermillId, Agent_id, Customer_id, Product_code, Shade_code, 0, StockDispatched);
        }

        //This function is supposed to Add the Stock being produced to the Inventory, or dispatched
        private int StockEntry(DateTime currDate, int PapermillId, int Agent_id, int Customer_id, string Product_code, string Shade_code, decimal StockProduced, decimal StockDispatched)
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    // Check if the stock record already exist for the given date, update the stock
                    var stock = (from Stk in db.Stock
                                 where Stk.stock_date == currDate
                                     && Stk.papermill_id == PapermillId
                                     && Stk.agent_id == Agent_id
                                     && Stk.customer_id == Customer_id
                                     && Stk.product_code == Product_code
                                     && Stk.shade_code == Shade_code
                                 select Stk).FirstOrDefault();
                    if (stock != null)
                    {
                        // means stock exist for supplied date
                        if (StockProduced > 0)
                            stock.stock_produced = stock.stock_produced + StockProduced;   //Add to current stock
                        if (StockDispatched > 0)
                            stock.stock_shipped = stock.stock_shipped + StockDispatched;

                        db.Entry(stock).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        // If no Stock Exist for given date, get the last Stock entry, and create a new stocks entry
                        var oldstock = (from Stk in db.Stock
                                        where Stk.stock_date < currDate
                                            && Stk.papermill_id == PapermillId
                                            && Stk.agent_id == Agent_id
                                            && Stk.customer_id == Customer_id
                                            && Stk.product_code == Product_code
                                            && Stk.shade_code == Shade_code
                                        orderby Stk.stock_date descending
                                        select Stk).FirstOrDefault();

                        decimal NewopeningStock = 0;
                        if (oldstock != null)   //Old record found, compute the new opening stock
                        {
                            //calculate opening stock
                            NewopeningStock = (decimal)oldstock.opening_stock + (decimal)oldstock.stock_produced - (decimal)oldstock.stock_shipped;
                        }

                        //Create a new record 
                        Stock Newstock = new Stock();
                        Newstock.stock_date = currDate.Date;
                        Newstock.papermill_id = PapermillId;
                        Newstock.agent_id = Agent_id;
                        Newstock.customer_id = Customer_id;
                        Newstock.product_code = Product_code;
                        Newstock.shade_code = Shade_code;
                        Newstock.opening_stock = NewopeningStock;
                        Newstock.stock_produced = 0;
                        Newstock.stock_shipped = 0;
                        if (StockProduced > 0)
                            Newstock.stock_produced = StockProduced;
                        if (StockDispatched > 0)
                            Newstock.stock_shipped = StockDispatched;
                        Newstock.stock_shipped = 0;
                        db.Stock.Add(Newstock);
                        db.SaveChanges();

                    }
                    return 1;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in Stock->StockEntry:", Ex);
                    return 0;
                }

            }
        }

        public List<Agent> GetAgentList()
        {
            using (db = new MWVDBContext())
            {
                try
                {
                    var query = (from agent in db.Agents
                                 select agent).ToList();
                    return query;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in StockRepository->GetAgentList:", Ex);
                    return null;
                }
            }
        }

        public List<ExportStock> GetDailyStock(int Agent_id, DateTime currDate)
        {
            currDate = currDate.Date;  //get only date if someone accidently passes datetime
            using (db = new MWVDBContext())
            {
                try
                {
                    var stock = (from Stk in db.Stock
                                 join cust in db.Customers on Stk.customer_id equals cust.customer_id
                                 join prod in db.Products on Stk.product_code equals prod.product_code
                                 join pm in db.Papermills on Stk.papermill_id equals pm.papermill_id
                                 where Stk.stock_date == currDate
                                     && Stk.agent_id == Agent_id
                                 select new ExportStock
                                 {
                                     Customer = cust.name,
                                     PaperMill = pm.name,
                                     BF = prod.bf_code,
                                     GSM = prod.gsm_code,
                                     Shade = Stk.shade_code,
                                     OpeningStock = (decimal) Stk.opening_stock,
                                     ClosingStock = (decimal)Stk.opening_stock + (decimal)Stk.stock_produced - (decimal) Stk.stock_shipped
                                 }).ToList();
                    return stock;
                }
                catch (Exception Ex)
                {
                    logger.Error("Error in StockRepository->StockEntry:", Ex);
                    return null;
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