//-----------------------------------------------------------------------------------------------
// This class is the core Deckle calcualtion logic with some helper functions
// Author     : Ramesh Kumhar
// Change History: (maintaned since 11 Sept' 2015)
//    11 Sep 2015: by RK - Now we have schedules in the system, Planning is done based on Schedules
//                   changed DeckleBusiness to pick orders based on schedules
//                   changed DeckleCalc Constructor to pass Papermill object, instead of individual parameters of the papermill
//                   earlier calculation was running for 30 days from today, changed to run till Schedule End date
//-----------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using MWV.Business;
using MWV.Models;
using MWV.Repository.Interfaces;
using System.Reflection;

namespace MWV.Business.Deckle
{

    [Serializable]
    public class Item
    {
        private Int32 orderproductno;
        public Int32 OrderProductNo
        {
            get { return orderproductno; }
            set { orderproductno = value; }
        }

        private string gsm;
        public string GSM
        {
            get { return gsm; }
            set { gsm = value; }
        }

        private string bf;
        public string BF
        {
            get { return bf; }
            set { bf = value; }
        }

        private string shade;
        public string Shade
        {
            get { return shade; }
            set { shade = value; }
        }

        private decimal size;
        public decimal Size
        {
            get { return size; }
            set { size = value; }
        }

        private decimal orderqty;
        public decimal OrderQty
        {
            get { return orderqty; }
            set { orderqty = value; }
        }

        //Temporary fields used in LINQ, not used for any calculation
        private decimal totalweight;
        public decimal TotalWeight
        {
            get { return totalweight; }
            set { totalweight = value; }
        }

        //Temporary fields used in LINQ, not used for any calculation
        private decimal totalsize;
        public decimal TotalSize
        {
            get { return totalsize; }
            set { totalsize = value; }
        }

        private decimal pendingqty;
        public decimal PendingQty
        {
            get { return pendingqty; }
            set { pendingqty = value; }
        }

        public decimal MasterJumboQty { get; set; }
        public int SidecutID { get; set; }
        public string Type { get; set; }
        public DateTime RequestedDate { get; set; }
        public bool isUpgraded { get; set; }
    }



    [Serializable]
    public class Jumbo
    {
        private string gsm;
        public string GSM
        {
            get { return gsm; }
            set { gsm = value; }
        }

        private string bf;
        public string BF
        {
            get { return bf; }
            set { bf = value; }
        }

        private string shade;
        public string Shade
        {
            get { return shade; }
            set { shade = value; }
        }

        private decimal availablesize;
        public decimal Availablesize
        {
            get { return availablesize; }
            set { availablesize = value; }
        }

        private decimal availableweight;
        public decimal AvailableWeight
        {
            get { return availableweight; }
            set { availableweight = value; }
        }

        private bool isopen;
        public bool IsOpen
        {
            get { return isopen; }
            set { isopen = value; }
        }

        private List<Item> itemsassigned;
        public List<Item> ItemsAssigned
        {
            get { return itemsassigned; }
            set { itemsassigned = value; }
        }

        private int masterjumbono;
        public int MasterJumboNo
        {
            get { return masterjumbono; }
            set { masterjumbono = value; }
        }

        public bool IsFullSize { get; set; }
        public DateTime RequestedDate { get; set; }

        public int SendToProductionID { get; set; }
        public int LowerBFID { get; set; }
    }

    public class JumboSize
    {
        public decimal TotalQty { get; set; }
        public List<Size> Sizes { get; set; } 
    }

    public class Size
    {
        public decimal Qty { get; set; }
        public int JumboNo { get; set; }
    }

    public class SideCut
    {
        public int Id { get; set; }
        public decimal Width { get; set; }
        public string Desc { get; set; }
    }

    public class OrderItem
    {
        public int OrderId { get; set; }
        public decimal Size { get; set; }
        public decimal AdjustedQty { get; set; }
    }
    public class ItemSize
    {
        public decimal TotalSize { get; set; }
        public List<OrderItem> Orders { get; set; }
    }

    public class Decklecalc
    {

        readonly log4net.ILog decklelogger = log4net.LogManager.GetLogger("DeckleFileAppender");
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        //Local variables used in calculation
        private decimal MinJumboWeight;
        private decimal MaxJumboWeight;
        private decimal MaxCuts;
        private decimal MaxWeightPerChild;
        private decimal MinDeckleSize;
        private decimal MaxDeckleSize;
        private List<Item> ItemsAvailable;
        private List<Jumbo> JumboRequired;
        private int MasterJumboNo = 0;
        private int MachineNo = 0;
        private string MachineName = "";
        private List<SideCut> SideCuts;
        private IDeckle _Deckle;
        private decimal MINSideCut;
        private decimal MAXSideCut;
        private int RoundWeight = 4;
        private int RoundSize = 2;
        private decimal DiscardWeight = (decimal) 0.05;
        private List<Item> LowerBFItems;  //Used for matching LowerBF
        private bool PRINTSIZEPAIRS = true;
        private bool PRINTDETAILEDLOGS = true;
        private DateTime ScheduleStart;
        private DateTime ScheduleEnd;
        private int ScheduleID;

        //Constructor 

        public Decklecalc(IDeckle _Deckle, List<Item> itemsAvailable, Papermill papermill, Schedule schedule )
        {
            decklelogger.InfoFormat("In Procedure DeckleCalc.Decklecalc");
            // Transfer variables to local
            this._Deckle = _Deckle;
 
            this.MinJumboWeight = (decimal) papermill.min_weight_jumbo;
            this.MaxJumboWeight = (decimal) papermill.max_weight_jumbo;
            this.MaxCuts = (decimal) papermill.max_cuts;
            this.MaxWeightPerChild = (decimal) papermill.max_weight_child;
            this.MinDeckleSize = (decimal) papermill.deckle_min;
            this.MaxDeckleSize = (decimal) papermill.deckle_max;
            this.MachineNo =  papermill.papermill_id;
            this.MachineName = papermill.name;
            this.ScheduleStart = (DateTime) schedule.start_date;
            this.ScheduleEnd = (DateTime) schedule.end_date;
            this.ScheduleID = schedule.schedule_id;

            //transfer list of Items available to local variables
            this.ItemsAvailable = new List<Item>();
            foreach (Item item in itemsAvailable)
            {
                this.ItemsAvailable.Add(item);
            }
            decklelogger.InfoFormat("Number of Items found to create Deckle....." + itemsAvailable.Count.ToString());

            LogAllItems(ItemsAvailable); 

            //Initialize Jumbo Required Tlist
            this.JumboRequired = new List<Jumbo>();

            //Create the SideCuts -- sorted by Highest first
            this.SideCuts = new List<SideCut>();
            { 
                SideCut sidecut = new SideCut();
                sidecut.Id = 3;
                sidecut.Width = (decimal) 40.64;  // 16"
                sidecut.Desc = "16 Inches SideCut";
                SideCuts.Add(sidecut);
            }
            {             
                SideCut sidecut = new SideCut();
                sidecut.Id = 2;
                sidecut.Width = (decimal) 30.48;  // 12"
                sidecut.Desc = "12 Inches SideCut";
                SideCuts.Add(sidecut);
            }
            {
                SideCut sidecut = new SideCut();
                sidecut.Id = 1;
                sidecut.Width = (decimal) 22.86;  // 9"
                sidecut.Desc = "9 Inches SideCut";
                SideCuts.Add(sidecut);
            }

            this.MINSideCut = (decimal) 22.86;
            this.MAXSideCut = (decimal) 40.64; 
        }

        #region CoreDeckleLogic
        //-----------------------------------------------------------------------------------------------
        // Function   : CalculateDeckle
        // Description: This function Runs the Deckle calculation 
        // Author     : Ramesh Kumhar
        // Usage/Logic: This function is called once per Paper mill 
        //            : It predicts and creates the Jumbos that would b required for fitting all the Available items
        //              It runs the Fitting logic per combination of BF/GSM/Shade
        //              If the Item can fit in any of the existing/Open Jumbos then it will try to fit
        //              If the item cannot fit then it will open a new Jumbo 
        //              Items are Added to the ItemAssigned collection within Each Jumbo
        //-----------------------------------------------------------------------------------------------
 
        public int CalculateDeckle() 
        {
            decklelogger.InfoFormat("_DeckleCalc.CalculateDeckle_");
            int PR_ID = 0;

            //___TODO___ Check if Solution can exist
            if (CheckIfDeckleCanExist())
            {

            }

            //----------------------------------------------------------------------
            // Create Jumbos ahead so that we dont need to create it everytime.
            //----------------------------------------------------------------------
            //commented by RK on 17Aug to start from empty Jumbo and create as required
            //JumboRequired = ComputeAndCreateJumbos(MaxDeckleSize, MaxJumboWeight);
            JumboRequired = new List<Jumbo>();

            //----------------------------------------------------------------------
            // Do the calculations now, Scan items, match size and find a jumbo and fit it there
            //----------------------------------------------------------------------
            
            // -- Run the outer loop once for each BF, GSM and Shade
            var DistinctBF_GSM_Shade = ItemsAvailable.GroupBy(g => new { g.BF, g.GSM, g.Shade, g.RequestedDate })
                                  .Select(g => new Item { BF = g.Key.BF, GSM = g.Key.GSM, Shade = g.Key.Shade, RequestedDate = g.Key.RequestedDate });
        
            foreach(var distinctcombination in DistinctBF_GSM_Shade)
            {
                decklelogger.InfoFormat("Running calculation for -> {0}/{1}/{2}/{3} Combination", distinctcombination.BF, distinctcombination.GSM, distinctcombination.Shade, distinctcombination.RequestedDate);
                
                bool ItemsWithPendingQtyFound = true;
                
                while (ItemsWithPendingQtyFound == true)  // This loop needs to run until there are items available with pending qty
                {
                    // fetch products matching the above combination sorted by lowest Qty 
                    var BFgsmShadelist = ItemsAvailable.Where(t => t.BF == distinctcombination.BF && t.GSM == distinctcombination.GSM && t.Shade == distinctcombination.Shade && t.RequestedDate == distinctcombination.RequestedDate && t.PendingQty > 0)
                                                    .Select(t => t)
                                                    .OrderBy(t => t.PendingQty);

                    //.OrderBy(t => t.PendingQty)
                    //.OrderByDescending(t => t.PendingQty)
                    //.OrderByDescending(t => t.RequestedDate.ToShortDateString())

                    decklelogger.InfoFormat("");
                    decklelogger.InfoFormat("For Combination -> {0}/{1}/{2}/{3} found : {04} Items to fit", distinctcombination.BF, distinctcombination.GSM, distinctcombination.Shade, distinctcombination.RequestedDate.ToShortDateString(), BFgsmShadelist.Count());
                    
                    decklelogger.InfoFormat(" Running Self Fit..");

                    foreach(var item in BFgsmShadelist)
                    {
                        // Check if the entire orderProduct can fit in one Jumbo 
                        CheckIfthisOrderProductCanFitEntirelyOnOneJumbo(item);
                    }

                    decklelogger.InfoFormat(" After Self Fit..");
                    LogJumbo();
                    LogAllItems(ItemsAvailable);

                    decklelogger.InfoFormat(""); 
                    decklelogger.InfoFormat(" Running Independent items..");

                    // if Items found which still have pending Qty then try to find an appropriate container for them
                    foreach (var item in BFgsmShadelist)
                    {

                        //Find the next available Jumbo this product can fit in
                        int Jumbono = GetNextAvailableJumbo(item.BF, item.GSM, item.Shade, item.PendingQty, item.Size);
                        if (Jumbono != -1) // Found a place
                        {
                            //Found a Jumbo where this item can fit in
                                decklelogger.InfoFormat("Found Jumbo# : " + Jumbono.ToString());
                        
                            //--Add this item to the Jumbo 
                            //make a copy of object as we have to change the qty as per other items
                            //Item Newitem = item;
                        
                            //get the new weight of the current item, as per the diameter/weight of the existing product
                            decimal newweight = Math.Round(ComputeNewWeightofItemAsPerSize(JumboRequired[Jumbono].ItemsAssigned[0].Size, JumboRequired[Jumbono].ItemsAssigned[0].OrderQty, item.Size),RoundWeight);

                            //Add the product to the Itemsavailable collection
                            AssignItemToJumbo(JumboRequired[Jumbono], item, newweight);
                       
                            // Since we could not fit the entire weight of the product on to Jumbo Reel, 
                            // deduct the weight that could be fit from the pending weight from the Product
                            UpdatePendingQtyinItemAvailableList(item, newweight);
                        }
                        else
                        {
                            // Item cannot fit in any of the partial jumbos that we have, so we need a new Jumbo container
                            int EmptyJumbono = GetEmptyJumbo(item.BF, item.GSM, item.Shade, item.PendingQty, item.Size);
                            decimal ItemQty = item.PendingQty;      //item.OrderQty;

                            //Cannot produce any Child which is more than the MaxWeightofChild
                            if (ItemQty > MaxWeightPerChild)
                            {
                                decklelogger.InfoFormat("Item greater than MaxWeightperchild");
                                ItemQty = MaxWeightPerChild;

                                //it is ideal to produce a rounded weight
                                //ItemQty = Math.Truncate(ItemQty);
                            }

                            //It is possible that all Jumbos have Exhausted for the Combination, then create a new Jumbo for the combination
                            if(EmptyJumbono == -1)
                            {
                                decklelogger.InfoFormat("All Jumbos Exhausted : " + EmptyJumbono.ToString());
                                //Create new jumbo
                                EmptyJumbono = CreateNewJumbo(item.BF, item.GSM, item.Shade);
                                decklelogger.InfoFormat("New Jumbo Created : " + EmptyJumbono.ToString());
                            }
                            decklelogger.InfoFormat("Empty Jumbo opened # : " + EmptyJumbono.ToString());

                            //Take only Qty that can fit in the Jumbo, Sometimes the OrderQty is more than Max size the Jumbo can hold
                            if (ItemQty > JumboRequired[EmptyJumbono].AvailableWeight)
                                ItemQty = JumboRequired[EmptyJumbono].AvailableWeight;

                            //Add the product to the Itemsavailable collection
                            AssignItemToJumbo(JumboRequired[EmptyJumbono], item, ItemQty);

                            //Deduct the Qty from pending qty (mostly it will become zero)
                            UpdatePendingQtyinItemAvailableList(item, ItemQty);

                            //--Match Other Sizes and see if there is a possibility of other products to fit on to this Jumbo

                            //create Pairs for possible combinations and return the best pair
                            List<ItemSize> itemSizeList = new List<ItemSize>();
                            int Foundpairitem = CreateSizePairs(item, BFgsmShadelist.ToList(), itemSizeList, ItemQty); 

                            if (Foundpairitem != -1)
                            {
                                //Found a pair, so lets add the products to the Jumbo
                                List<OrderItem> orderItems = itemSizeList[Foundpairitem].Orders;
                                for (int i = 1; i <= orderItems.Count() - 1; i++)   //Skip the first item as it is already added to the Jumbo
                                { 
                                    decimal adjustedqty = Math.Round( orderItems[i].AdjustedQty,RoundWeight);
                                    //Add the product to the Itemsavailable collection
                                    AssignItemToJumbo(JumboRequired[EmptyJumbono], GetItembyOrderProductFromItemsAvailable(orderItems[i].OrderId), adjustedqty);

                                    //Deduct the Qty from pending qty (mostly it will become zero)
                                    UpdatePendingQtyinItemAvailableList(GetItembyOrderProductFromItemsAvailable(orderItems[i].OrderId), adjustedqty);
                                }
                            }


                        }  // end of opennewjumbo 

  
                        //If Qty left is few kilograms then reset it to zero (Qty defined in DiscardWeight variable)
                        ForcefulResetofQtytoZero(item);

                        if(PRINTDETAILEDLOGS)
                        { 
                            LogJumbo();
                            LogAllItems(ItemsAvailable);
                        }
                    }  //end of for loop for each orderproduct with same BF/GSM/Shade combination

                    decklelogger.InfoFormat("JUMBO Log and ITEM log after every BF/GSM/Shade combination");
                    LogJumbo();
                    LogAllItems(ItemsAvailable);

                    if (BFgsmShadelist.Count() == 0)  // if all items are passed through then the list will be empty
                    {
                        ItemsWithPendingQtyFound = false; // exit criteria for the loop
                    }

                } // End of While Loop

                //--------------------------------------------------------------------------
                // We have finished running Deckle for one BF, GSM, Shade and Date combination
                // It is possible that few deckle may not be complete now, we can try taking orders for same BF/GSM/Shade 
                // from next release date, and see if it can fit in here.
                //--------------------------------------------------------------------------

                //build next date to pass to the function
                DateTime nextDate;
                nextDate = GetNextDeliveryDateforBFGSMShade(distinctcombination.BF, distinctcombination.GSM, distinctcombination.Shade, distinctcombination.RequestedDate);
                if(nextDate.ToShortDateString() != "01/01/01")
                    FillJumbosWithNextDateAvailableOrders(distinctcombination.BF, distinctcombination.GSM, distinctcombination.Shade, distinctcombination.RequestedDate ,nextDate);

                //--------------------------------------------------------------------------
                // Processs Lower BF Approvals
                // Process approved for Upgrade to Higher BF, where we seek for matching approvals of Jumbos which can take lower BF orders
                //--------------------------------------------------------------------------
                GetOrdersFromLowerBF(distinctcombination.BF, distinctcombination.GSM, distinctcombination.Shade, distinctcombination.RequestedDate, LowerBFItems);
                if (LowerBFItems != null)
                { 
                    ProcessLowerBFApprovals(distinctcombination.RequestedDate, LowerBFItems);
                }

            } // End of Foreach loop for calculations of creating Jumbos and assigning items to it


            //----------------------------------------------------------------------
            // Jumbos may not reach full QTY/Weight, Merge Jumbos to create a Full Jumbo within MIN and MAX Jumbo Weight
            //----------------------------------------------------------------------

            decklelogger.InfoFormat("______ Running Merge And Create Master Jumbos_________");
            MergeAndCreateMasterJumbos();

            //----------------------------------------------------------------------
            //___TODO___ If CUTOFF time for Production is near and sufficient orders have not come then we need to use Lower BF orders to fill the JUMBOS
            //----------------------------------------------------------------------

            decklelogger.InfoFormat("____Upgrade orders to Higher BF");
            //UpgradeOrdersToHigherBF();

            //----------------------------------------------------------------------
            // Process Sidecuts for pending widths in Jumbo
            //----------------------------------------------------------------------
            ProcessSideCuts();

            //----------------------------------------------------------------------
            // Process Send to Production Approvals
            //----------------------------------------------------------------------
            
            ProcessSendToProductionApprovals();

            //----------------------------------------------------------------------
            //___TODO___ After Processing Jumbos, some Jumbos may not qualify to be included in Master due to short width, We need to exclude them from processing
            //----------------------------------------------------------------------


            decklelogger.InfoFormat("______ RECREATING - Running Merge And Create Master Jumbos_________");

            MergeAndCreateMasterJumbos();
            
            LogMasterJumbo();

            //----------------------------------------------------------------------
            //  If Master JUMBOS are not full and more orders are required to fill it, then Raise an ALERT and send emails
            //----------------------------------------------------------------------

            decklelogger.InfoFormat("______ Raise Alert For Jumbos Not Full_________");
            RaiseAlertForJumbosNotFull();

            //----------------------------------------------------------------------
            // following things are done in this procedure: CreateProductionSchedule
            //     Create a Production Run Schedule
            //     Find timelines for each of the BF/GSM/Shade combination
            //     Add Jumbos to the production Schedule
            //     Add Child Roll to the Jumbo / Production Schedule
            //----------------------------------------------------------------------
            PR_ID = CreateProductionSchedule();
            if (PR_ID > 0)  //if a production schedule was created
            { 
                //----------------------------------------------------------------------
                // Update the ORDERS/PRODUCTS table and mark the Records as Scheduled
                //----------------------------------------------------------------------
                CalculateQtyScheduledFromMasterJumbos();
                UpdateOrdersWithProductionRunSchedules();
                UpdateDBwithLowerBFStatus();
                UpdateDBwithSendToProductionStatus();
                
            }

            return PR_ID; //Return the newly created produtionrun_id so that we can send emails from the parent function
        }



        //-----------------------------------------------------------------------------------------------
        // Function   : UpdatePendingQtyinItemAvailableList
        // Description: This function updates the Pending Qty in the ItemsAvailable List  
        // Author     : Ramesh Kumhar
        // Usage      : Once an Item has been Put into the Jumbo, this function is called to deduct 
        //            : the item balance, so the Pending items can only be used for the rest of the calculation
        //-----------------------------------------------------------------------------------------------
        private void UpdatePendingQtyinItemAvailableList(Item itemToUpdate, decimal QtyToUpdate)
        {
            //___TODO___ This can be converted to a LINQ Search and Update
            foreach (Item item in ItemsAvailable)
            {
                if(item.OrderProductNo == itemToUpdate.OrderProductNo)
                {
                    if (QtyToUpdate == 0)
                        item.PendingQty = 0;
                    else
                        item.PendingQty = Math.Round(item.PendingQty - QtyToUpdate,RoundWeight);
                }
            }
 
        }

        //-----------------------------------------------------------------------------------------------
        // Function   : GetNextAvailableJumbo
        // Description: Gets the Next available Jumbo where this item can fit
        // Author     : Ramesh Kumhar
        // Usage      : Returns the Item number of the Jumbo Collection where this item can fit
        //            
        //-----------------------------------------------------------------------------------------------

        private int GetNextAvailableJumbo(string BF, string GSM, string Shade, decimal QtyToFit, decimal SizetoFit )
        {
            decklelogger.InfoFormat(String.Format("_GetNextAvailableJumbo_ for BF:{0,-10} GSM: {1,-10} Shade: {2,-10} QtytoFit: {3,-10} SizetoFit: {4,-10}", BF, GSM, Shade, QtyToFit, SizetoFit));
            int returnvalue = -1;
            // This procedure is supposed to give the Next available Jumbo number which will be used to fill
            // JumboRequired list will contain multiple BF,GSM,Shade combination jumbos, we need to pick the right one
            for(int i = 0; i <= JumboRequired.Count -1; i++)
            {
                if (JumboRequired[i].IsOpen)  // Goal is to first find Jumbos that are already open and partially used
                { 
                    if(JumboRequired[i].BF == BF && JumboRequired[i].GSM == GSM && JumboRequired[i].Shade == Shade) 
                    {
 
                        decimal newweight = ComputeNewWeightofItemAsPerSize(JumboRequired[i].ItemsAssigned[0].Size, JumboRequired[i].ItemsAssigned[0].OrderQty, SizetoFit);
                        if(ValidateIfItemQualifiesForThisJumbo(i,SizetoFit, newweight, QtyToFit))
                        {
                            returnvalue = i;   //Hurray... We found a place where this item can fit in
                            break;
                        }

                    }
                }
            }

            return returnvalue;
        }

        private bool ValidateIfItemQualifiesForThisJumbo(int Index, decimal SizetoFit, decimal NewWeight, decimal QtytoFit)
        {
            // Check if sizetofit can fit in the available size
            if(SizetoFit <= JumboRequired[Index].Availablesize)
            {
                //Check if New weight to fit will fit the Remaining jumbo weight
                if (NewWeight <= JumboRequired[Index].AvailableWeight)
                { 
                    //Check if the New Calculated weight is more than the Qty on Order... it should not be... 
                    if(NewWeight <= QtytoFit)
                    {
                        //Check if the No of cuts is exceeding for the machine
                        if(JumboRequired[Index].ItemsAssigned.Count +1 <= MaxCuts)
                        { 
                            return true;
                        }
                    }
                }
            }

            return false;


        }

        //-----------------------------------------------------------------------------------------------
        // Function   : GetEmptyJumbo
        // Description: Gets an empty Jumbo to store the item
        // Author     : Ramesh Kumhar
        // Usage      : Checks and returns an empty Jumbo after matching BF/GSM/Shade
        //            
        //-----------------------------------------------------------------------------------------------
        private int GetEmptyJumbo(string BF, string GSM, string Shade, decimal QtyToFit, decimal SizetoFit)
        {
            decklelogger.InfoFormat(String.Format("_GetEmptyJumbo_ for BF:{0,-10} GSM: {1,-10} Shade: {2,-10} QtytoFit: {3,-10} SizetoFit: {4,-10}", BF, GSM, Shade, QtyToFit, SizetoFit));
            int returnvalue = -1;
            // This procedure is supposed to give the Next available Jumbo number which will be used to fill
            // JumboRequired list will contain multiple BF,GSM,Shade combination jumbos, we need to pick the right one
            for (int i = 0; i <= JumboRequired.Count - 1; i++)
            {
                if (JumboRequired[i].BF == BF && JumboRequired[i].GSM == GSM && JumboRequired[i].Shade == Shade && JumboRequired[i].IsOpen == false)
                {

                    // Check if sizetofit can fit in the available size
                    if (SizetoFit <= JumboRequired[i].Availablesize)
                    {
                        returnvalue = i;   
                        break;
                    }

                }
            }

            //TODO: 
            // If there are no Jumbos that are empty and this item cannot fit then create a new JUMBO here 

            return returnvalue;
        }

        private void ForcefulResetofQtytoZero(Item item)
        {
            // if due to some reason the Product qty is 0.01 then we should just mark the product as completed
            if (item.PendingQty > 0 && item.PendingQty <= DiscardWeight)
            {
                decklelogger.InfoFormat("_ForcefulResetofQtytoZero: Forceful Reset of ItemQTY to 0 for OrderProduct {0} with pending qty {1}", item.OrderProductNo, item.PendingQty);
                //Deduct the Qty from pending qty (mostly it will become zero)
                UpdatePendingQtyinItemAvailableList(item, (decimal)0.0);
            }

        }
        private bool CheckIfthisOrderProductCanFitEntirelyOnOneJumbo(Item item)
        {
            //If Qty left is few kilograms then reset it to zero
            ForcefulResetofQtytoZero(item);

            //If the weight of the product is more than the Maxweightperchild then we can make an attempt to check
            //if (item.PendingQty > )
            int quotient = (int) (MaxDeckleSize / item.Size);
            decimal remainder = (decimal) (MaxDeckleSize % item.Size);
            bool success = false;

            // if the no of childs are less than the max cuts then proceed and the sum of all sizes fitting is more than the MinDeckle size
            if (quotient <= MaxCuts && (quotient * item.Size) >= MinDeckleSize) 
            {
                if(remainder <= (MaxDeckleSize - MinDeckleSize)) // if Trim is less than allowed wastage, then proceed
                {
                    decimal ItemQty = 0;
                    int NoofJumbosrequiredtoFit = 1;
                    ItemQty = item.PendingQty / quotient;
                    //If itemqty > maxChildWeight then we need to split into multiple jumbos
                    if (ItemQty > MaxWeightPerChild)
                    {
                        NoofJumbosrequiredtoFit = (int) (ItemQty / MaxWeightPerChild);
                        NoofJumbosrequiredtoFit += (ItemQty % MaxWeightPerChild == 0 ? 0 : 1);
                        //reset itemqty to a new value
                        ItemQty = Math.Round( item.PendingQty / (NoofJumbosrequiredtoFit * quotient), RoundWeight);

                        //bug fixed on 09sept - sum of all childs exceeding MaxJumboWt
                        if((ItemQty * quotient) > MaxJumboWeight)
                        {
                            //increase No of Jumbos required to fit by 1
                            NoofJumbosrequiredtoFit++;
                            //reset itemqty to a new value
                            ItemQty = Math.Round(item.PendingQty / (NoofJumbosrequiredtoFit * quotient), RoundWeight);
                        }
                    }

                    // Loop and Assign multiple items to the Jumbo
                    for (int jumno = 1; jumno <= NoofJumbosrequiredtoFit; jumno++ )
                    {
                        //Get a new Jumbo to fit
                        int EmptyJumbono = GetEmptyJumbo(item.BF, item.GSM, item.Shade, item.PendingQty, item.Size);

                        //It is possible that all Jumbos have Exhausted for the Combination, then create a new Jumbo for the combination
                        if (EmptyJumbono == -1)
                        {
                            decklelogger.InfoFormat("All Jumbos Exhausted (Full Order Product Fit): " + EmptyJumbono.ToString());
                            //Create new jumbo
                            EmptyJumbono = CreateNewJumbo(item.BF, item.GSM, item.Shade);
                            decklelogger.InfoFormat("New Jumbo Created (Full order Product Fit): " + EmptyJumbono.ToString());
                        }
                        decklelogger.InfoFormat("Empty Jumbo opened (Full order Product Fit) # : " + EmptyJumbono.ToString());

                        for (int i = 1; i <= quotient; i++)
                        {
                            //Add the product to the Itemsavailable collection
                            AssignItemToJumbo(JumboRequired[EmptyJumbono], item, ItemQty);

                            //Deduct the Qty from pending qty (mostly it will become zero)
                            UpdatePendingQtyinItemAvailableList(item, ItemQty);
                        }
                    }
                    success = true;
                }
            }
            //If Qty left is few kilograms then reset it to zero
            ForcefulResetofQtytoZero(item);

            if (success)
                return true;
            else
                return false;
        }

        //-----------------------------------------------------------------------------------------------
        // Function   : ComputeAndCreateJumbos
        // Description: Function Computes the weight of combination BF/GSM/Shade and creates number of Jumbos
        // Author     : Ramesh Kumhar
        // Usage      : 
        //            
        //-----------------------------------------------------------------------------------------------
        private List<Jumbo> ComputeAndCreateJumbos(decimal AvailableSize, decimal AvailableWeight)
        {
            decklelogger.InfoFormat("_DeckleCalc.ComputeAndCreateJumbos_");

            //This function does a lookahead of all Items availble on orders and based on GSM, BF and Shade and creates Jumbo rolls

            var ExpectedJumbo = ItemsAvailable.GroupBy(g => new { g.BF, g.GSM, g.Shade })
                                              .Select(g => new Item 
                                                    { 
                                                        BF = g.Key.BF, 
                                                        GSM = g.Key.GSM, 
                                                        Shade = g.Key.Shade,
                                                        TotalWeight = g.Sum(wt => wt.OrderQty),
                                                        TotalSize = g.Sum(wt =>wt.Size)
                                                    });

            this.JumboRequired = new List<Jumbo>();
            // transfer Expected Jumbos to collection 
            foreach(var item in ExpectedJumbo)
            {
                decklelogger.InfoFormat(String.Format("Splitting total orders -> {0,-10} {1,-10} {2,-10} TotalWeight:{3,-10} TotalSize:{4,-10} ",
                         item.BF, item.GSM, item.Shade, item.TotalWeight, item.TotalSize));

                //find out how many jumbos will be required based on Max of Weight or Size
                int size = (int) Math.Ceiling(Decimal.Divide( item.TotalSize, AvailableSize));
                int weight = (int) Math.Ceiling(Decimal.Divide( item.TotalWeight , AvailableWeight));
                int maxnoofJumbos = 0;

                if (size >= weight)
                    maxnoofJumbos = size;
                else
                    maxnoofJumbos = weight;

                for (int i = 1; i <= maxnoofJumbos; i++)
                {
                    Jumbo jumbo = new Jumbo();
                    jumbo.BF = item.BF;
                    jumbo.GSM = item.GSM;
                    jumbo.Shade = item.Shade;
                    jumbo.Availablesize = AvailableSize;
                    jumbo.AvailableWeight = AvailableWeight;
                    jumbo.IsOpen = false;
                    JumboRequired.Add(jumbo);
                    decklelogger.InfoFormat(String.Format("      Split Into Jumbo sizes -> {0,-10} {1,-10} {2,-10} {3,-10} {4,-10} ",
                                 jumbo.BF, jumbo.GSM, jumbo.Shade, jumbo.Availablesize, jumbo.AvailableWeight));

                }
            }
            return JumboRequired;
        }

        private void MergeAndCreateMasterJumbos()
        {
            MasterJumboNo = 0;
            //update all Master jumbo numbers to 0 or assign a number if it is full weight
            foreach (Jumbo jumbo in JumboRequired)
            {
                //added on 15 July
                // if Jumbo size is not full then we cannot include in masterjumbo
                decimal jumbosize = MaxDeckleSize - jumbo.Availablesize;
                if (jumbosize >= MinDeckleSize)
                {
                    jumbo.IsFullSize = true;
                    decimal Jumboweight = MaxJumboWeight - jumbo.AvailableWeight;
                    if (Jumboweight >= MinJumboWeight && Jumboweight <= MaxJumboWeight) //If full Jumbos weight already present
                        jumbo.MasterJumboNo = ++MasterJumboNo;
                    else
                        jumbo.MasterJumboNo = 0;
                }
                else
                { 
                    jumbo.IsFullSize = false;
                    jumbo.MasterJumboNo = 0;
                }
            }

            decklelogger.InfoFormat("After assigning full weight Jumbos");
            LogJumbo();

            // -- Get a list of Distinct Jumbos 
            
            var DistinctJumbosList = JumboRequired.GroupBy(g => new { g.BF, g.GSM, g.Shade, g.RequestedDate})
                                  .Select(g => new Item { BF = g.Key.BF, GSM = g.Key.GSM, Shade = g.Key.Shade, RequestedDate = g.Key.RequestedDate });
            foreach(var distinctjumbo in DistinctJumbosList)
            {

                decklelogger.InfoFormat("Running Loop for Distinct Jumbos :" + distinctjumbo.BF + "/" + distinctjumbo.GSM + "/" + distinctjumbo.Shade + " / " + distinctjumbo.RequestedDate.ToShortDateString());
    
                // fetch Jumbos for the selected combination
                var BFgsmShadeJumbolist = JumboRequired.Where(t => t.BF == distinctjumbo.BF && t.GSM == distinctjumbo.GSM && t.Shade == distinctjumbo.Shade && t.MasterJumboNo == 0 && t.IsFullSize == true && t.RequestedDate == distinctjumbo.RequestedDate)
                                                    //.OrderBy(t => t.RequestedDate);
                                                    .OrderBy(t => t.AvailableWeight); 
                int i = 0;
                foreach (var jumbo in BFgsmShadeJumbolist)
                {
                    decklelogger.InfoFormat("       Jumbo # " + JumboRequired.IndexOf(jumbo).ToString() + " to be paired :" + jumbo.BF + "/" + jumbo.GSM + "/" + jumbo.Shade + " Qty:" + (MaxJumboWeight - jumbo.AvailableWeight));

                    decimal Jumboweight = MaxJumboWeight - jumbo.AvailableWeight;
                    if (jumbo.MasterJumboNo == 0)
                    {
                        createProbabilityAndGetBestPairingSize(JumboRequired.IndexOf(jumbo), Jumboweight, BFgsmShadeJumbolist.ToList());
                    }
                    i++;
                } // end of foreach 

            } // endof distinct foreach

            //Print Final status of Jumbos 
            decklelogger.InfoFormat("......Final status of Jumbos after MasterJumbo creation");
            LogJumbo();
        }

        private void createProbabilityAndGetBestPairingSize(int jumboNo, decimal Qty, List<Jumbo> BFgsmShadeJumbolist)
        {
            List<JumboSize> jumboSizes = new List<JumboSize>();

            //create a probability match
            for(int i = 0; i <= BFgsmShadeJumbolist.Count() - 1; i++)
            {

                if (i != jumboNo && BFgsmShadeJumbolist[i].MasterJumboNo == 0)  //&& BFgsmShadeJumbolist[i].IsFullSize
                {

                    JumboSize jumbosize = new JumboSize();
                    jumboSizes.Add(jumbosize);

                    //List<Size> sizes = jumbosize.Sizes;
                    jumbosize.Sizes = new List<Size>();

                    //Add base size
                    Size size = new Size();
                    size.JumboNo = jumboNo;
                    size.Qty = Qty;
                    jumbosize.Sizes.Add(size);

                    decimal Totweight = Qty;
                    Totweight = SumJumboQty(BFgsmShadeJumbolist, Totweight, jumbosize.Sizes, i++, jumboNo);

                    jumbosize.TotalQty = Totweight;

                }
            } // end of FOR loop

            decklelogger.InfoFormat("               Sets created ");

            decimal BestQty = 0;
            int BestSet = 0;
            int cnt = 0; 
            
            foreach (JumboSize jumbosz in jumboSizes)
            {
                if (PRINTSIZEPAIRS)
                { 
                    decklelogger.InfoFormat("                    Jumbo Combination set [" + jumboSizes.IndexOf(jumbosz).ToString() + "]  Qty: "  +  jumbosz.TotalQty + " in " + jumbosz.Sizes.Count().ToString());
                    foreach (Size sz in jumbosz.Sizes)
                    {
                        decklelogger.InfoFormat("                            Jumbo # = " + sz.JumboNo + "   Qty: " + sz.Qty);
                    }
                }
                //Find the best match in this Jumbo
                if (BestQty < jumbosz.TotalQty)
                { 
                    BestQty = jumbosz.TotalQty;
                    BestSet = cnt;
                }
                 cnt++;

            }
            //Update the best set of Jumbos with masterjumboNo
            if (BestQty >= MinJumboWeight)
            {
                //Increment the Master Jumbo number
                ++MasterJumboNo;
                decklelogger.InfoFormat("        Assigning Master Jumbo # " + MasterJumboNo.ToString() + "    Best Set Match: " + BestSet.ToString() + "   with Qty: " + BestQty.ToString());

                foreach (Size sz in jumboSizes[BestSet].Sizes)
                {
                    JumboRequired[sz.JumboNo].MasterJumboNo = MasterJumboNo;
                    decklelogger.InfoFormat("               assigned Jumbo # " + JumboRequired.IndexOf(JumboRequired[sz.JumboNo]).ToString());
                }

                if(PRINTDETAILEDLOGS)
                    LogJumbo();
            }
            else
            {
                decklelogger.InfoFormat("     No Master Jumbo set could be created");

            }
        }

        protected decimal SumJumboQty(List<Jumbo> BFgsmShadeJumbolist, decimal TotWeight, List<Size> Sizes, int i, int jumbono)
        {
            for (int j = i; j <= BFgsmShadeJumbolist.Count() - 1; j++)
            {
                if (BFgsmShadeJumbolist[j].MasterJumboNo == 0 && JumboRequired.IndexOf(BFgsmShadeJumbolist[j]) != jumbono)  //&& BFgsmShadeJumbolist[j].IsFullSize
                {
                    if (TotWeight + (MaxJumboWeight - BFgsmShadeJumbolist[j].AvailableWeight) < MaxJumboWeight)
                    {
                        if (!CheckIfsizeAlreadyExistsinSizes(Sizes, JumboRequired.IndexOf(BFgsmShadeJumbolist[j])))
                        {
                            Size size = new Size();
                            size.JumboNo = JumboRequired.IndexOf(BFgsmShadeJumbolist[j]);
                            size.Qty = MaxJumboWeight - BFgsmShadeJumbolist[j].AvailableWeight;
                            Sizes.Add(size);

                            TotWeight += (MaxJumboWeight - BFgsmShadeJumbolist[j].AvailableWeight);
                            TotWeight = SumJumboQty(BFgsmShadeJumbolist, TotWeight, Sizes, j++, jumbono);

                        }
                    }
                }
                //break;
            }
            return TotWeight;

        }

        private int CreateSizePairs(Item currItem, List<Item> itemList, List<ItemSize> itemSizeList, decimal QtytoMatch)
        {
            //____TODO____ This function to create Size Pairs has scope of improvement
            // currently it is not considering all the permutations of the various size, Need some logic improvement here

            decklelogger.InfoFormat("Creating SizePairs.. for " + currItem.Size.ToString() + " width");
            //create a new pair
            for (int i = 0; i <= itemList.Count() - 1; i++)
            {

                for (int j = 0; j <= itemList.Count() - 1; j++)
                {
                    ItemSize itemsize = new ItemSize();   //create a new itemsize
                    itemsize.Orders = new List<OrderItem>();  //create a new orders list within itemsize

                    //add the current product to the orders
                    OrderItem orderitem = new OrderItem();
                    orderitem.OrderId = currItem.OrderProductNo;  //[i].OrderProductNo;
                    orderitem.Size = currItem.Size; //itemList[i].Size;
                    orderitem.AdjustedQty = QtytoMatch;    //QTY of the product that we have put on the Jumbo
                    itemsize.Orders.Add(orderitem);

                    //add the current product to the orders
                    //OrderItem orderitem1 = new OrderItem();
                    //orderitem1.OrderId = itemList[i].OrderProductNo;
                    //orderitem1.Size = itemList[i].Size;
                    //orderitem1.AdjustedQty = Math.Round(ComputeNewWeightofItemAsPerSize(currItem.Size, QtytoMatch, itemList[i].Size), 4);
                    //itemsize.Orders.Add(orderitem1);
                    decimal qtyadded = AddItemtoSizePairSet(itemsize.Orders, itemList[i]);
                    decimal sizeadded = currItem.Size;
                    if (qtyadded > 0)
                        sizeadded += itemList[i].Size;

                    decimal setsize = getItemSize(sizeadded, j, itemList, itemsize.Orders);
                    itemsize.TotalSize = setsize;

                    itemSizeList.Add(itemsize);
                }
                
            }

            //Print SizePairs
            if (PRINTSIZEPAIRS)
            { 
                foreach(ItemSize item in itemSizeList)
                {
                    string log; 
                    log = "   Set [" + itemSizeList.IndexOf(item) + "]  Deckle size: " + item.TotalSize.ToString() ;
                    foreach(OrderItem order in item.Orders)
                    {
                        log = log + (" {" + order.OrderId.ToString() + ", " + order.Size.ToString() + ", " + order.AdjustedQty.ToString() + "} ");
                    }
                    decklelogger.InfoFormat("   {0} ", log);

                }
            }
            if (itemSizeList.Count() > 0)
            {
                //Find the best pair that will match, based on size

                //sort the list on Totalsize and take the first one
                var NewItemSizeTopItem = itemSizeList.OrderByDescending(t => t.TotalSize)
                                                        .Select(t => t).First();

                if (NewItemSizeTopItem.TotalSize < MinDeckleSize)
                {
                    //If the best matching size is less than the min deckle size, then we do not have a match
                    // it is better that we match it one by one

                    return -1;
                }
                else
                {
                    decklelogger.InfoFormat("");
                    decklelogger.InfoFormat("Best matching Pair found for this item = Set [" + itemSizeList.IndexOf(NewItemSizeTopItem).ToString() + "]");
                    int index = itemSizeList.IndexOf(NewItemSizeTopItem);
                    string log; 
                    log = "   Set [" + itemSizeList.IndexOf(NewItemSizeTopItem) + "]  Deckle size: " + NewItemSizeTopItem.TotalSize.ToString() ;
                    foreach(OrderItem order in itemSizeList[index].Orders)
                    {
                        log = " {" + order.OrderId.ToString() + ", " + order.Size.ToString() + ", " + order.AdjustedQty.ToString() + "} ";
                    }
                    decklelogger.InfoFormat("   {0} ", log);

                    return itemSizeList.IndexOf(NewItemSizeTopItem);
                }
            }
            else
                return -1;
        }

        // This function returns the Item from the Itemavailable collection by passing the orderProductNo
        private Item GetItembyOrderProductFromItemsAvailable(int orderItemNo)
        {
            Item item = ItemsAvailable.Where(t => t.OrderProductNo == orderItemNo)
                                    .Select(t => t).First();
            return item;
        }


        protected decimal getItemSize(decimal rowSize, int i, List<Item> itemList, List<OrderItem> orderItems)
        {
            for (int j = i; j <= itemList.Count() - 1; j++)
            {
                //Skip the same order Item and take the item only if the qty is still available
                //if (orderItems[0].OrderId != itemList[j].OrderProductNo && orderItems[1].OrderId != itemList[j].OrderProductNo && itemList[j].PendingQty > 0)
                if (!CheckOrderExistinSizePairSet(orderItems,itemList[j]) &&  itemList[j].PendingQty > 0)
                {
                    if ((rowSize + itemList[j].Size) < MaxDeckleSize)
                    {
                        //compute how much of weight of this product would fit on to the Jumbo based on the first item size and Qty
                        decimal newweight = Math.Round(ComputeNewWeightofItemAsPerSize(orderItems[0].Size, orderItems[0].AdjustedQty, itemList[j].Size), RoundWeight);
                        
                        //Check if the new weight is available in pending Qty
                        if(newweight <= itemList[j].PendingQty)
                        { 
                            // this item is qualified to sit next to the original Item 
                            //OrderItem orderitem = new OrderItem();
                            //orderitem.OrderId = itemList[j].OrderProductNo;
                            //orderitem.Size = itemList[j].Size;
                            //orderitem.AdjustedQty = newweight;
                            //orderItems.Add(orderitem);
                            //rowSize += itemList[j].Size;
                            decimal qtyadded = AddItemtoSizePairSet(orderItems,itemList[j]); //returns qty that was added
                            if (qtyadded > 0)
                                rowSize += itemList[j].Size;
                            if (j < itemList.Count() - 1)
                                rowSize = getItemSize(rowSize, ++j, itemList, orderItems);   //Recursive call 

                        }
                    }
                }
            }
            return rowSize;
        }

        private decimal AddItemtoSizePairSet(List<OrderItem> OrderItemsinItemsize, Item item)
        {
            //check using LINQ if the order is already there
            var orders = OrderItemsinItemsize.Where(p => p.OrderId == item.OrderProductNo)
                                            .Select(p => p).FirstOrDefault();

            var sumwidth = OrderItemsinItemsize.Select(p => p.Size).Sum();

            decimal Adjustedqty = Math.Round(ComputeNewWeightofItemAsPerSize(OrderItemsinItemsize[0].Size, OrderItemsinItemsize[0].AdjustedQty, item.Size), RoundWeight);

            if (orders == null && Adjustedqty <= item.PendingQty) // if not there then Add also Check if the new weight is available in pending Qty
            {
                if (sumwidth + item.Size <= MaxDeckleSize) //One final check to see if the sum of sizes are not going above deckle size
                {
                    OrderItem orderitem = new OrderItem();
                    orderitem.OrderId = item.OrderProductNo;  //[i].OrderProductNo;
                    orderitem.Size = item.Size; //itemList[i].Size;
                    orderitem.AdjustedQty = Adjustedqty;
                    OrderItemsinItemsize.Add(orderitem);
                    return orderitem.AdjustedQty;
                }
                else
                    return 0;
            }
            else  // else return 0
                return 0;

        }

        private bool CheckOrderExistinSizePairSet(List<OrderItem> OrderItemsinItemsize, Item item)
        {
            //check using LINQ if the order is already there
            var orders = OrderItemsinItemsize.Where(p => p.OrderId == item.OrderProductNo)
                                            .Select(p => p).FirstOrDefault();
            if (orders == null) 
                return false;
            else
                return true;
        }

        private void GetListofPartialJumbosThatCanbeUpgraded()
        {

             // -- Run the outer loop once for each BF, GSM and Shade
            var DistinctJumbosList = JumboRequired.GroupBy(g => new { g.BF, g.GSM, g.Shade })
                                  .Select(g => new Item { BF = g.Key.BF, GSM = g.Key.GSM, Shade = g.Key.Shade });

            foreach (var distinctjumbo in DistinctJumbosList)
            {

                //___TODO___ Find Higher and Lower Deckle Combinations (one size lower) that can be used for Match Pairs
                string NextBF = _Deckle.GetLowerDeckleCombination(distinctjumbo.BF, distinctjumbo.GSM, distinctjumbo.Shade);

                decklelogger.InfoFormat("Next Best BF found [ {0} ] for {1}/{2}/{3}", NextBF, distinctjumbo.BF, distinctjumbo.GSM, distinctjumbo.Shade);
                if (NextBF != "") 
                {
                    // Find deckle that are not complete in Higher BF

                    // fetch Jumbos for the selected combination to find incomplete Jumbos
                    var BFgsmShadeJumbolist = JumboRequired.Where(t => t.BF == distinctjumbo.BF && t.GSM == distinctjumbo.GSM && t.Shade == distinctjumbo.Shade && t.MasterJumboNo == 0 && t.IsFullSize == false)
                                                        .OrderBy(t => t.Availablesize);
                    foreach (var jumbo in BFgsmShadeJumbolist)
                    {
                        //find jumbos that are atleast half the size
                        //if (jumbo.Availablesize )
                            

                    }

                  //___TODO___ Find deckle that are not complete in Lower BF 
                }
            }

        }

        protected bool CheckIfsizeAlreadyExistsinSizes(List<Size> Sizes, int Jumbono)
        {
            foreach (Size sz in Sizes)
            {
                if (sz.JumboNo == Jumbono)
                    return true;
            }
            return false;
        }

        private void RaiseAlertForJumbosNotFull()
        {

            //____TODO____ Send emails of Shortfall Qty to Agents

            decklelogger.InfoFormat(" .... ShortFall QTY Email to be sent to Agents...");
            decklelogger.InfoFormat(String.Format("          {0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10} {6,-10} {7,-10} {8,-10} {9,-10}",
                 "MachineNo", "BF", "GSM", "Shade", "Size", "Qty", "", "", "", ""));

            decklelogger.InfoFormat(String.Format("          {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} ", "----------"));

            List<deckle_approvals> ApprovalList = new List<deckle_approvals>();

            foreach(Jumbo jumbo in JumboRequired)
            {
                if(!jumbo.IsFullSize && jumbo.IsOpen)  //If jumbo is not full and is in open state
                {
                    // ---- For Jumbos that are not full, find the qty required to fill the Jumbos and Print
                    // Take the first item to get the Size and Qty and calculate the shortfall QTY and SIZE
                    if (jumbo.ItemsAssigned.Count() > 0)
                    {
                        decimal size = jumbo.Availablesize;
                        decimal qty = Math.Round((jumbo.Availablesize * jumbo.ItemsAssigned[0].OrderQty) / jumbo.ItemsAssigned[0].Size, RoundWeight);

                        string matchedSize = "";
                        foreach(Item item in jumbo.ItemsAssigned)
                        {
                            if (matchedSize == "")
                                matchedSize += item.Size;
                            else
                                matchedSize += " + " + item.Size;
                        }

                        deckle_approvals approval = new deckle_approvals();
                        approval.request_date = DateTime.Now;
                        approval.bf_code = jumbo.ItemsAssigned[0].BF;
                        approval.gsm_code = jumbo.ItemsAssigned[0].GSM;
                        approval.shade_code = jumbo.ItemsAssigned[0].Shade;
                        approval.papermill_id = MachineNo;
                        approval.matched_sizes = matchedSize;
                        approval.required_size = size;
                        approval.required_weight = qty;
                        ApprovalList.Add(approval);

                        decklelogger.InfoFormat(String.Format("          {0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10} {6,-10} {7,-10} {8,-10} {9,-10}",
                             MachineNo, jumbo.ItemsAssigned[0].BF, jumbo.ItemsAssigned[0].GSM, jumbo.ItemsAssigned[0].Shade, size, qty, "", "", "", ""));
                    }

                }
            }

            if (ApprovalList.Count > 0)
            {
                if (_Deckle.CreateDeckleApprovals(ApprovalList) == 1)
                {
                    decklelogger.InfoFormat("  " + ApprovalList.Count.ToString() +  " Lower BF Mismatches created in Database");
                }

            }
            
        }

        private void GetOrdersFromLowerBF(string currBF, string currGSM, string currShade, DateTime currDelDate, List<Item> LowerBFItems)
        {

            if(LowerBFItems != null)
                LowerBFItems.Clear();

            //Find LowerBF
            string LowerBfCode = _Deckle.GetLowerBF(currBF);
            if (LowerBfCode != "")
            { 
                //fetch orders from ItemsAvailable collection for same delivery date
                List<Item> lowerbf = ItemsAvailable.Where(t => t.BF == LowerBfCode && t.GSM == currGSM && t.Shade == currShade && t.RequestedDate == currDelDate && t.PendingQty > 0)
                                            .OrderBy(t => t.PendingQty)
                                            .Select(t => t).ToList<Item>();
                LowerBFItems = lowerbf;

            }

        }

        private void ProcessLowerBFApprovals(DateTime DeliveryDate, List<Item> LowerBFItems)
        {
            //Scan Jumbos where we are feeling short of orders to complete Jumbo
            //Check if we have approvals for the same to fill them with Lower BF orders

            decklelogger.InfoFormat("");
            decklelogger.InfoFormat("...Processsing Lower BF approvals for Delivery Date " + DeliveryDate.ToShortDateString());
            string usedIDs = "";
            foreach (Jumbo jumbo in JumboRequired)
            {
                decimal jumbosize = MaxDeckleSize - jumbo.Availablesize;
                if (jumbo.IsOpen && jumbosize <= MinDeckleSize && DeliveryDate.ToShortDateString() == jumbo.RequestedDate.ToShortDateString())
                {
                    jumbo.LowerBFID = 0;
                    CheckLowerBFApprovals(jumbo, usedIDs);
                    if (jumbo.LowerBFID != 0)
                    {
                        //Match found
                        usedIDs += jumbo.LowerBFID.ToString() + ",";

                        //Let us see if we can find a Matching size in the Lower BF combination
                        foreach(Item LbItem in LowerBFItems)
                        {
                            if (LbItem.Size == jumbo.Availablesize)
                            {
                                //Yippee.. We got a item that can be upgraded to HigherBF
                                //get the new weight of the current item, as per the diameter/weight of the existing product
                                decimal newweight = Math.Round(ComputeNewWeightofItemAsPerSize(jumbo.ItemsAssigned[0].Size, jumbo.ItemsAssigned[0].OrderQty, LbItem.Size), RoundWeight);

                                //Add the product to the Itemsavailable collection
                                AssignItemToJumbo(jumbo, LbItem, newweight);

                                // deduct the weight that could be fit from the pending weight from the Product
                                UpdatePendingQtyinItemAvailableList(LbItem, newweight);

                                //Mark the item in the Jumbo as Upgraded
                                foreach(Item newlyaddeditem in jumbo.ItemsAssigned)
                                {
                                    if(newlyaddeditem.OrderProductNo == LbItem.OrderProductNo)
                                    {
                                        //this one just got added
                                        newlyaddeditem.isUpgraded = true;
                                        break;
                                    }
                                }
                                decklelogger.InfoFormat("      Item Added with order#" + LbItem.OrderProductNo.ToString() + "  with Size: " + LbItem.Size.ToString() + "  Qty: " + newweight.ToString());

                            }

                        }
                    }

                }
            }

           


        }


        private void CheckLowerBFApprovals(Jumbo jumbo, string UsedIds)
        {

            //get a list of all LowerBF Approved Combination sizes
            List<deckle_approvals> ApprovalList = new List<deckle_approvals>();
            ApprovalList = _Deckle.GetDeckleApprovals(MachineNo, jumbo.BF, jumbo.GSM, jumbo.Shade, "Send To Lower Bf");
            foreach (deckle_approvals approval in ApprovalList)
            {
                if (UsedIds.IndexOf(approval.dm_id.ToString() + ",", 0) == -1) //if the ID is already used in earlier Jumbo the skip
                {
                    // if Size and Qty Matches then mark the Jumbo as Full
                    if (approval.required_size == jumbo.Availablesize && jumbo.LowerBFID == 0)  //&& approval.required_weight == jumbo.AvailableWeight // weight removed as this is available weight remaining on Jumbo, we wanted for individual item
                    {
                        // Allocate Approval to Jumbo 
                        jumbo.IsFullSize = true;
                        jumbo.LowerBFID = approval.dm_id;
                        decklelogger.InfoFormat("         Jumbo # " + JumboRequired.IndexOf(jumbo).ToString() + " with pending width " + jumbo.Availablesize.ToString() + "  matched with DMID [" + jumbo.LowerBFID.ToString() + "] " + " for 'Lower BF Match'  status ");
                        break;
                    }
                }
            }

        }

        private void UpdateDBwithLowerBFStatus()
        {
            decklelogger.InfoFormat("...Marking 'Lower BF Used' status as used in DB");
            foreach (Jumbo jumbo in JumboRequired)
            {
                if (jumbo.MasterJumboNo != 0 && jumbo.LowerBFID != 0)
                {
                    //Mark the Approval as utilized
                    int returnval = _Deckle.UpdateDeckleApprovals(jumbo.LowerBFID);
                    if (returnval == 1)
                        decklelogger.InfoFormat(" Jumbo # " + JumboRequired.IndexOf(jumbo).ToString() + " with pending width " + jumbo.Availablesize.ToString() + " , QTY " + jumbo.AvailableWeight.ToString() + "  matched with [" + jumbo.LowerBFID.ToString() + "] " + " was utilized as 'Use with Lower BF' ");
                }
            }
        }


        private void ProcessSendToProductionApprovals()
        {
            //Scan the "Send to Production" Approvals list and mark the jumbos as complete which matches 

            decklelogger.InfoFormat("...Processsing SEND_TO_PRODUCTION approvals");
            string usedIDs = "";
            foreach(Jumbo jumbo in JumboRequired)
            {
                if (jumbo.IsOpen && !jumbo.IsFullSize)
                {
                    jumbo.SendToProductionID = 0;
                    CheckSendToProductionPlans(jumbo, usedIDs);
                    if (jumbo.SendToProductionID != 0)
                    {
                        usedIDs += jumbo.SendToProductionID.ToString() + ","; 

                    }

                }
            }

        }

        private void CheckSendToProductionPlans(Jumbo jumbo, string UsedIds)
        {

            //get a list of all Send to production approvals 
            List<deckle_approvals> ApprovalList = new List<deckle_approvals>();
            ApprovalList = _Deckle.GetDeckleApprovals(MachineNo, jumbo.BF, jumbo.GSM, jumbo.Shade, "Send To Production");
            foreach(deckle_approvals approval in ApprovalList)
            {
                if (UsedIds.IndexOf(approval.dm_id.ToString() + ",", 0) == -1) //if the ID is already used in earlier Jumbo the skip
                { 
                    // if Size and Qty Matches then mark the Jumbo as Full
                    if (approval.required_size == jumbo.Availablesize && jumbo.SendToProductionID == 0)  //&& approval.required_weight == jumbo.AvailableWeight // weight removed as this is available weight remaining on Jumbo, we wanted for individual item
                    {
                        //Mark the Jumbo as Full & mark it as being marked from Send to productin
                        jumbo.IsFullSize = true;
                        jumbo.SendToProductionID = approval.dm_id;
                        decklelogger.InfoFormat(" Jumbo # " + JumboRequired.IndexOf(jumbo).ToString() + " with pending width " + jumbo.Availablesize.ToString() + " , QTY " + jumbo.AvailableWeight.ToString() + "  matched with [" + jumbo.SendToProductionID.ToString() + "] " + " for 'Sent to Production'  status ");
                        break;
                    }
                }
            }

        }

        private void UpdateDBwithSendToProductionStatus()
        {
            decklelogger.InfoFormat("...Marking 'SEND_TO_PRODUCTION' status as used in DB");
            foreach(Jumbo jumbo in JumboRequired)
            {
                if(jumbo.MasterJumboNo !=0 && jumbo.SendToProductionID != 0)
                {
                    //Mark the Approval as utilized
                    int returnval = _Deckle.UpdateDeckleApprovals(jumbo.SendToProductionID);
                    if (returnval == 1)
                        decklelogger.InfoFormat(" Jumbo # " + JumboRequired.IndexOf(jumbo).ToString() + " with pending width " + jumbo.Availablesize.ToString() + " , QTY " + jumbo.AvailableWeight.ToString() + "  matched with [" + jumbo.SendToProductionID.ToString() + "] " + " was utilized as 'Sent to Production' ");
                }
            }
        }

        private void CalculateQtyScheduledFromMasterJumbos()
        {
            // find out qty that have been consumed in MasterJumbos from Jumbo

            foreach (Jumbo jumbo in JumboRequired)
            {
                if (jumbo.MasterJumboNo > 0)
                {
                    foreach (Item item in jumbo.ItemsAssigned)
                    {
                        UpdateMasterJumboQtyinItemAvailableList(item.OrderProductNo, item.OrderQty);
                    }
                }
            }
            decklelogger.InfoFormat("Items Available Position after summing up Qty from Master Jumbos .. to be Put in Schedule");
            LogAllItems(ItemsAvailable);

        }

        private int CreateProductionSchedule()
        {
            decklelogger.InfoFormat("");
            decklelogger.InfoFormat("--Creating Production Schedule--");

            List<ProductionJumbo> ProductionJumboList = new List<ProductionJumbo>();
            List<ProductionChild> ProductionChildList = new List<ProductionChild>();
            bool IsError = false;
            DateTime PlanEstimatedStart = DateTime.MinValue;
            DateTime PlanEstimatedEnd = DateTime.MinValue;
            DateTime JumboEstimatedStart = DateTime.MinValue;
            decimal totalqty = 0;
            int totaltime = 0;
            int newProdRunId = 0;

            //Get Last Estimated end time from ProductionRun table for the paper machine
            ProductionRun prdRun = new ProductionRun();
            prdRun = _Deckle.GetLastRunEstimateEndTime(MachineNo, ScheduleID);
            if (prdRun == null)
            {
                //it is possible that the schedule may be starting in a future date, if yes then use the future date to start the schedule
                if (ScheduleStart > DateTime.Now)
                {
                    decklelogger.InfoFormat("No Records found in Production table for Schedule [" + ScheduleID.ToString() + "].. creating Plan with start date as " + ScheduleStart.ToString());
                    PlanEstimatedStart = ScheduleStart;
                    PlanEstimatedEnd = ScheduleStart;
                }
                else
                { 
                    //Schedule date is earlier than today's date, but there are no earlier production plans
                    // Since Prod Plans cannot start in past, we change the Plan start to start now
                    decklelogger.InfoFormat("No Records found in Production table for Schedule [" + ScheduleID.ToString() + "].. creating Plan with current date");
                    PlanEstimatedStart = DateTime.Now;
                    PlanEstimatedEnd = DateTime.Now;
                }
            }
            else
            {
                //if the last PR plan end date was earlier than today, reset the plan start date to now
                if(prdRun.estimated_end < DateTime.Now)
                {
                    PlanEstimatedStart = DateTime.Now;
                    PlanEstimatedEnd = DateTime.Now;
                    decklelogger.InfoFormat("older Plans found for Schedule[" + ScheduleID.ToString() + "]...with old End Date...new Estimated schedule Start : " + PlanEstimatedStart.ToString());
                }
                else
                { 
                    //There are Prod plans available for future
                    PlanEstimatedStart = (DateTime)prdRun.estimated_end;
                    PlanEstimatedEnd = (DateTime)prdRun.estimated_end;
                    decklelogger.InfoFormat("Future plans exists for Schedule [" + ScheduleID.ToString() + "] new plan - Estimated schedule Start : " + PlanEstimatedStart.ToString());
                }
            }

            // Get last Jumbo number
            int LastJumbonumber = _Deckle.GetLastJumboNo();
            //____TODO___ Handle Resetting of Jumbo Numbers at the end of the every year

            // We already have the Master jumbo number, run the loop from 1 to Masterjumbono

            for (int localMastJumboNo = 1; localMastJumboNo <= MasterJumboNo; localMastJumboNo++)
            {
                decimal ProductionPerTonTime = 0;
                JumboEstimatedStart = PlanEstimatedEnd;
                //Get a list of Jumbos from the JumboRequired with matching MasterJumboNo
                var JumboProdSch = JumboRequired.Where(t => t.MasterJumboNo == localMastJumboNo)
                                                .OrderBy(t => t.MasterJumboNo).ToList();

                //Find the time taken to Produce per ton of Jumbo
                ProductionTimeline prdtimeline = new ProductionTimeline();
                prdtimeline = _Deckle.GetProductionTimeline(MachineNo, JumboProdSch[0].BF, JumboProdSch[0].GSM, JumboProdSch[0].Shade);

                // if no timeline found then log an error
                if (prdtimeline == null)
                {
                    decklelogger.ErrorFormat("---------------Missing Record in ProductionTimeline Table -------------------------");
                    decklelogger.ErrorFormat("ERROR: Production timeline record does not exist for combination:");
                    decklelogger.ErrorFormat("Machine# " + MachineNo.ToString() + " BF: " + JumboProdSch[0].BF + " GSM: " + JumboProdSch[0].GSM + " Shade: " + JumboProdSch[0].Shade);
                    logger.ErrorFormat("ERROR: Production timeline record does not exist for combination:");
                    logger.ErrorFormat("Machine# " + MachineNo.ToString() + " BF: " + JumboProdSch[0].BF + " GSM: " + JumboProdSch[0].GSM + " Shade: " + JumboProdSch[0].Shade);
                    IsError = true;
                }
                else
                {
                    ProductionPerTonTime = (decimal) prdtimeline.time_per_ton; //This is time taken to produce per ton (in Minutes)
                    decklelogger.InfoFormat("TimeLine -> Machine# " + MachineNo.ToString() + "/" + JumboProdSch[0].BF + "/" + JumboProdSch[0].GSM + "/" + JumboProdSch[0].Shade + " -> Time taken to produce 1 ton:" + ProductionPerTonTime.ToString());
                }


                if (ProductionPerTonTime > 0 )
                {
                    decimal masterJumboQty = 0;
                    int masterjumbotime = 0;
                    decimal masterJumboSize = 0;

                    foreach (Jumbo jumbo in JumboProdSch) 
                    {
                        //Add all the Qty of the individual jumbo's in the MasterJumbo
                        masterJumboQty += MaxJumboWeight - jumbo.AvailableWeight;
                        // get the maxsize from the list of jumbos that we have, that is the width we should produce for the master Jumbo.
                        if (masterJumboSize < (MaxDeckleSize - jumbo.Availablesize))
                            masterJumboSize = (MaxDeckleSize - jumbo.Availablesize);

                    }
                    //calculate total time required to produce the Master Jumbo (totqty x per ton time)
                    masterjumbotime = (int) (masterJumboQty * ProductionPerTonTime);

                    //compute the end time based on the start time
                    PlanEstimatedEnd = JumboEstimatedStart.AddMinutes(masterjumbotime);

                    decklelogger.InfoFormat("   Master Jumbo Production time: " + masterjumbotime.ToString() + " minutes,   Scheduled Est End: " + PlanEstimatedEnd.ToString());

                    //---Create ProductionJumbo record (one record per Master Jumbo)
                    ProductionJumbo newPrdJumbo = new ProductionJumbo();
                    newPrdJumbo.jumbo_no = ++LastJumbonumber;
                    newPrdJumbo.bf_code = JumboProdSch[0].BF;
                    newPrdJumbo.gsm_code = JumboProdSch[0].GSM;
                    newPrdJumbo.shade_code = JumboProdSch[0].Shade;
                    newPrdJumbo.planned_qty = masterJumboQty;
                    newPrdJumbo.estimated_start = JumboEstimatedStart;
                    newPrdJumbo.planned_width = masterJumboSize;
                    newPrdJumbo.jumbo_width = masterJumboSize + 0; //Add 2 CM on each size for trimming
                    newPrdJumbo.TempMasterJumboNo = localMastJumboNo;  //Assign the masterJumno Number so that we can use it to get Items
                    ProductionJumboList.Add(newPrdJumbo);

                    totalqty += masterJumboQty;
                    totaltime += masterjumbotime;

                }
            }

            if (!IsError && ProductionJumboList.Count() > 0)
            {
                PlanEstimatedEnd = PlanEstimatedStart.AddMinutes(totaltime);
                //Create the ProductionRun  and add the data to the database
                ProductionRun newprdrun = new ProductionRun();
                newprdrun.papermill_id = MachineNo;
                newprdrun.estimated_start = PlanEstimatedStart;
                newprdrun.estimated_end = PlanEstimatedStart.AddMinutes(totaltime);
                newprdrun.run_time = totaltime;
                newprdrun.estimated_qty = totalqty;
                newprdrun.schedule_id = ScheduleID;   //Assign ScheduleID to the Production Plan -- for tracking
                newProdRunId = _Deckle.CreateProductionRun(newprdrun, ProductionJumboList);  // get the last added Run schedule number

                if (newProdRunId > 0)
                {
                    // insert records in ProductionChild table

                    int i = 1;

                    foreach (ProductionJumbo prdjumbo in ProductionJumboList)
                    {
                        //find the 
                        foreach (Jumbo jumbo in JumboRequired)
                        {
                            if (prdjumbo.TempMasterJumboNo == jumbo.MasterJumboNo)
                            {
                                List<Item> Items = jumbo.ItemsAssigned;
                                foreach (Item item in Items)
                                {
                                    //--Create records in ProductionChild table
                                    ProductionChild Productionchild = new ProductionChild();
                                    Productionchild.pj_id = prdjumbo.pj_id;
                                    if (item.Type == "I")
                                    {
                                        Productionchild.order_product_id = item.OrderProductNo;
                                        //Markings - If Jumbo is of different BF and Item is of different BF then Mark the Roll 
                                        if (jumbo.BF != item.BF)
                                            Productionchild.marking = item.BF.ToString() + " BF";
                                    }
                                    else
                                    {
                                        //Mark child reel as SideCut
                                        Productionchild.marking = "12BF (Sidecut)";
                                        Productionchild.sidecut_id = item.SidecutID;
                                    }

                                    Productionchild.width = item.Size;
                                    Productionchild.qty = item.OrderQty;
                                    Productionchild.sequence = i;
                                    ProductionChildList.Add(Productionchild);
                                }
                                i++;    //Increment Sequence no after all items in one jumbo no are added
                            }
                        }
                    }
                    int returnval = _Deckle.CreateProductionRunChilds(ProductionChildList);  // Create ProductionRun Child records

                    if (returnval > 0)
                    {
                         
                        LogProductionSchedule(newprdrun, ProductionJumboList, ProductionChildList);
                    }
                    else
                        IsError = true;

                    //added on 12 Sept 15
                    //Time to check If the PROD Plan just created is Surpassing Schedule end date 
                    if (PlanEstimatedEnd > ScheduleEnd)
                    {
                        //This Production Plan has surpassed the Schedule End Datetime, We should modify the Subsequent Production Plan's start date

                        //Get all the Active Production Plans
                        List<Schedule> lstSchedules = _Deckle.GetActiveSchedulesforPapermill(MachineNo);

                        bool schExtended = false;  //variable to track the schedule was extended, 
                        DateTime newEndDate = DateTime.MinValue;
                        foreach (Schedule schedule in lstSchedules)
                        {
                            //if this is the Next schedule from the current one, and this is where the overlap has happened then extend the start date of this schedule
                            if (schExtended)  //Run this code for all Plans
                            {
                                //if current Prod Plan end date is greater than the next schedule start date
                                if (newEndDate >= schedule.start_date)
                                {
                                    decklelogger.InfoFormat("Extending Sibbling Schedule[" + schedule.schedule_id.ToString() + "] with new Start date: " + newEndDate.ToString());

                                    //Extend the Schedule Start date
                                    int tmp = _Deckle.UpdateScheduleStartDate(schedule.schedule_id, newEndDate);
                                    //if there are prodplans created for the schedule then move the prod plan dates by the difference in minutes
                                    if (schedule.TotalRuntime > 0)
                                    {
                                        //find difference in minutes (end time of our current plan - next prod schedule start date)
                                        TimeSpan ts = newEndDate.Subtract((DateTime)schedule.start_date);  //Schedule.start_date will still have the old start date of the plan as the recordset is not yet refreshed
                                        int MinutesDelay = (int)ts.TotalMinutes;
                                        decklelogger.InfoFormat("  Extending Production Plans under Schedule [" + schedule.schedule_id.ToString() + "] by : " + MinutesDelay.ToString() + " Minutes");
                                        //Move All Prod Plans attached to this schedule
                                        int tmpdelay = _Deckle.DelayProductionPlansByMinsforSchedule(schedule.schedule_id, MinutesDelay);
                                        
                                        //Check if we need to extend the End Date of the Schedule, 
                                        DateTime tmpend = (DateTime) schedule.start_date;
                                        tmpend = tmpend.AddMinutes(MinutesDelay);
                                        if (schedule.end_date < tmpend)  // If schedule end date is less than temp end, then we need to extend end date of schedule
                                        {
                                            int t = _Deckle.UpdateScheduleEndDate(schedule.schedule_id, tmpend);
                                            newEndDate = (DateTime)tmpend;
                                        }
                                        else
                                            newEndDate = (DateTime)schedule.end_date;
                                    }
                                    else
                                        newEndDate = (DateTime)schedule.end_date;
                                }
                                else
                                    newEndDate = (DateTime) schedule.end_date;  
                            }

                            //if this is the current schedule then extend the end date of this schedule
                            if(schedule.schedule_id == ScheduleID)
                            {
                                decklelogger.InfoFormat("Extending Schedule[" + schedule.schedule_id.ToString() + "] with new end date: " + PlanEstimatedEnd.ToString());
                                if (_Deckle.UpdateScheduleEndDate(schedule.schedule_id, PlanEstimatedEnd) > 0)
                                { 
                                    schExtended = true;
                                    newEndDate = PlanEstimatedEnd;
                                }
                            }

                        }  //end of for

                    }

                }
                else
                    IsError = true;
            }
            else
                decklelogger.InfoFormat("No Production Plan/Schedule Created ");

            if (IsError)
                return 0;
            else
                return newProdRunId;
        }

        private void UpdateOrdersWithProductionRunSchedules()
        {
            decklelogger.InfoFormat("");
            decklelogger.InfoFormat("---- Updating Order Products with production Run Schedules---- ");

            //Loop thru the ItemsAvailable collection, MasterjumboQty field is already updated with the qty of orders proocesed in this RUN
            foreach (Item item in ItemsAvailable)
            {
                if (!_Deckle.UpdateOrderProductQty(item.OrderProductNo, item.MasterJumboQty))
                {
                    decklelogger.InfoFormat("  ERROR: Order Product " + item.OrderProductNo.ToString() + " could not be updated");
                }
                else
                {
                    decklelogger.InfoFormat("      Order Product # " + item.OrderProductNo.ToString() + " updated with qty " + item.MasterJumboQty.ToString());
                }
            }
 
        }

        private void ProcessSideCuts()
        {
            decklelogger.InfoFormat("");
            decklelogger.InfoFormat("---- Processing SideCuts-----");

            foreach(Jumbo jumbo in JumboRequired)   //do this for all Jumbos
            {
                if (jumbo.IsOpen)  // sidecut cannot be calculated if the Jumbo is empty
                {
                    if (jumbo.Availablesize >= MINSideCut && jumbo.Availablesize <= MAXSideCut) //go in only if there is a possible match scenario
                    {
                        if(jumbo.ItemsAssigned.Count < MaxCuts) //if we have already reached maxcuts then we cannot add more
                        {
                            //Find a cut size that will fit
                            foreach(SideCut sidecut in SideCuts)
                            {
                                if(sidecut.Width < jumbo.Availablesize)
                                {
                                    //Calculate new qty based on item sizes (mostly first item) that is already present in Jumbo

                                    decimal newweight = Math.Round(ComputeNewWeightofItemAsPerSize(jumbo.ItemsAssigned[0].Size, jumbo.ItemsAssigned[0].OrderQty, sidecut.Width), RoundWeight);

                                    decklelogger.InfoFormat("     Found a Jumbo to fit Sidecut, Jumbo# " + JumboRequired.IndexOf(jumbo).ToString() + "  SideCut : " + sidecut.Desc + " Weight: " + newweight.ToString());
                                    //the width of the sidecut will fit the Jumbo, but the weight may become more than the max Allowed Weight, If yes we cannot add
                                    if (jumbo.AvailableWeight - newweight > 0)
                                    {
                                        AssignSideCutToJumbo(jumbo, sidecut, newweight);
                                        decklelogger.InfoFormat("        Added");
                                    }
                                    else
                                        decklelogger.InfoFormat("        Could not add - Adding Sidecut will exceed Jumbo weight, Sidecut weight: " + newweight.ToString() + " Jumbo Avl Weight: " + jumbo.AvailableWeight.ToString());

                                }
                            }
                        }
                    }
                }
            }
            decklelogger.InfoFormat("");
            decklelogger.InfoFormat(".. Jumbo Dump after Sidecut processing");

            LogJumbo();
        }

        private void AssignSideCutToJumbo(Jumbo myJumbo, SideCut sideCut, decimal QtytoUpdate)
        {
            List<Item> Items = myJumbo.ItemsAssigned;
            if (Items == null)
            {
                myJumbo.ItemsAssigned = new List<Item>();
            }

            Item item = new Item();
            item.BF = myJumbo.BF;
            item.GSM = myJumbo.GSM;
            item.Type = "S";   // Mark it as sidecut
            item.OrderProductNo = 0; // Order Product # will be zero as this is a sidecut
            item.SidecutID = sideCut.Id;
            item.OrderQty = QtytoUpdate;
            item.PendingQty = 0; //Sidecuts do not have pending qty
            item.Shade = myJumbo.Shade;
            item.Size = sideCut.Width;
            item.TotalSize = 0;
            item.TotalWeight = 0;
            myJumbo.ItemsAssigned.Add(item);

            int indexof = myJumbo.ItemsAssigned.IndexOf(item);  //get the Indexof the newly added item
            myJumbo.ItemsAssigned[indexof].OrderQty = QtytoUpdate;  //update the Qty of the Item to the Qty that would get produced
            myJumbo.Availablesize = Math.Round(myJumbo.Availablesize - item.Size, RoundSize);
            //myJumbo.AvailableWeight = Math.Round(myJumbo.AvailableWeight - myItemtoAssign.OrderQty, 4); // bug found on 10 June
            myJumbo.AvailableWeight = Math.Round(myJumbo.AvailableWeight - QtytoUpdate, RoundWeight);
            myJumbo.IsOpen = true;
        }

        //This Function finds and fills pending jumbos with next day orders if they are available
        private void FillJumbosWithNextDateAvailableOrders(string currBF, string currGSM, string currShade, DateTime currDate, DateTime NextDate)
        {

            decklelogger.InfoFormat("");
            decklelogger.InfoFormat(" in Func _FillJumbosWithNextDateAvailableOrders.. Trying to find next day orders for {0}/{1}/{2}/{3} for Jumbos that are not full", currBF, currGSM, currShade, NextDate.ToShortDateString());

            //Find jumbos that are not fill width
            foreach(Jumbo jumbo in JumboRequired)
            {
                //match given size and check for those jumbos only
                if (jumbo.BF == currBF && jumbo.GSM == currGSM && jumbo.Shade == currShade && jumbo.RequestedDate == currDate)
                {
                    // if jumbo has reached MINDeckle size then there is no point, else give a try to find if something can fit here
                    if ((MaxDeckleSize - jumbo.Availablesize) < MinDeckleSize)
                    {
                        //we found a jumbo which is qualified to fit orders from next day
                        decklelogger.InfoFormat("  Fetching next day orders to fill jumbo [{0}] with Pending size: {1}, Qty: {2} ", JumboRequired.IndexOf(jumbo), jumbo.Availablesize, jumbo.AvailableWeight );

                        // Find best orders that can fit into the Jumbo, get next day orders sorted by size desc
                        var BFgsmShadelist = ItemsAvailable.Where(t => t.BF == currBF && t.GSM == currGSM && t.Shade == currShade && t.RequestedDate == NextDate && t.PendingQty > 0)
                                .Select(t => t)
                                .OrderByDescending(t => t.Size);
                        

                        Item curritem = new Item();   //build a fake item to pass it to the createsizepairs functin
                        curritem.Size = MaxDeckleSize - jumbo.Availablesize;
                        curritem.OrderProductNo = 0;

                        //-- create size pairs to get the best possible match

                        List<ItemSize> itemSizeList = new List<ItemSize>();
                        int Foundpairitem = CreateSizePairs(curritem, BFgsmShadelist.ToList(), itemSizeList, MaxJumboWeight - jumbo.AvailableWeight);
                        if (Foundpairitem != -1)
                        {

                            //Found a pair, so lets add the products to the Jumbo
                            List<OrderItem> orderItems = itemSizeList[Foundpairitem].Orders;
                            for (int i = 1; i <= orderItems.Count() - 1; i++)   //Skip the first item as it is already added to the Jumbo
                            {
                                decimal adjustedqty = Math.Round(orderItems[i].AdjustedQty, RoundWeight);
                                //Add the product to the Itemsavailable collection
                                AssignItemToJumbo(jumbo, GetItembyOrderProductFromItemsAvailable(orderItems[i].OrderId), adjustedqty);

                                //Deduct the Qty from pending qty (mostly it will become zero)
                                UpdatePendingQtyinItemAvailableList(GetItembyOrderProductFromItemsAvailable(orderItems[i].OrderId), adjustedqty);
                            }
                        }

                    }

                }


            }  // end of for loop

            


        }

        #endregion CoreDeckleLogic



        #region HelperFunctions
        //-----------------------------------------------------------------------------------------------
        // Function   : AssignItemToJumbo
        // Description: Assigns/Adds an Item to Jumbo
        // Author     : Ramesh Kumhar
        // Usage      : 
        //            
        //-----------------------------------------------------------------------------------------------
        private void AssignItemToJumbo(Jumbo myJumbo, Item myItemtoAssign, decimal QtytoUpdate)
        {
            List<Item> Items = myJumbo.ItemsAssigned;
            if (Items == null)
            {
                myJumbo.ItemsAssigned = new List<Item>();
                myJumbo.RequestedDate = myItemtoAssign.RequestedDate;   //the first date in the item is the jumbo del date
            }
            //myItemtoAssign.OrderQty = QtytoUpdate;

            // Make a copy of the item, as changing anything in the itemassigned changes the Source item as well
            Item item = new Item();
            item.BF = myItemtoAssign.BF;
            item.GSM = myItemtoAssign.GSM;
            item.OrderProductNo = myItemtoAssign.OrderProductNo;
            item.OrderQty = myItemtoAssign.OrderQty;
            item.PendingQty = myItemtoAssign.PendingQty;
            item.Shade = myItemtoAssign.Shade;
            item.Size = myItemtoAssign.Size;
            item.TotalSize = myItemtoAssign.TotalSize;
            item.TotalWeight = myItemtoAssign.TotalWeight;
            item.SidecutID = 0;   // this is a real Order Product, so sidecut will be zero
            item.Type = "I";  //Mark it as Item
            item.RequestedDate = myItemtoAssign.RequestedDate;
            myJumbo.ItemsAssigned.Add(item);

            int indexof = myJumbo.ItemsAssigned.IndexOf(item);  //get the Indexof the newly added item
            myJumbo.ItemsAssigned[indexof].OrderQty = QtytoUpdate;  //update the Qty of the Item to the Qty that would get produced
            myJumbo.Availablesize = Math.Round(myJumbo.Availablesize - item.Size, RoundSize);
            //myJumbo.AvailableWeight = Math.Round(myJumbo.AvailableWeight - myItemtoAssign.OrderQty, 4); // bug found on 10 June
            myJumbo.AvailableWeight = Math.Round(myJumbo.AvailableWeight - QtytoUpdate, RoundWeight); 
            myJumbo.IsOpen = true;
        }

        private decimal ComputeNewWeightofItemAsPerSize(decimal CompareSize, decimal CompareWeight, decimal NewSize)
        {
            return (decimal)(NewSize * CompareWeight) / CompareSize;
        }

        private int CreateNewJumbo(string BF, string GSM, string Shade)
        {
            Jumbo jumbo = new Jumbo();
            jumbo.BF = BF;
            jumbo.GSM = GSM;
            jumbo.Shade = Shade;
            jumbo.Availablesize = MaxDeckleSize;
            jumbo.AvailableWeight = MaxJumboWeight;
            jumbo.IsOpen = false;
            JumboRequired.Add(jumbo);
            int newJumbono = JumboRequired.IndexOf(jumbo);
            return newJumbono;
        }

        //-----------------------------------------------------------------------------------------------
        // Function   : CheckIfDeckleCanExist
        // Description: This function is supposed to Check if creating Deckle is possible, otherwise return and do not waste time
        // Author     : Ramesh Kumhar
        // Usage      : 
        //            
        //-----------------------------------------------------------------------------------------------
        private bool CheckIfDeckleCanExist()
        {
            decklelogger.InfoFormat("In Procedure DeckleCalc.CheckIfDeckleCanExist");

            // This function does a quick check if Deckle can be calculated 
            //ToDo: Put below validations
            //   1: Enough orders to calculate atleast one deckle
            //   2. 
            return true;
        }

        private void UpdateMasterJumboQtyinItemAvailableList(int OrderProductNo, decimal QtyToUpdate)
        {
            //____TODO___ This can be converted to a LINQ to search and update
            foreach (Item item in ItemsAvailable)
            {
                if (item.OrderProductNo == OrderProductNo)
                {
                    item.MasterJumboQty = item.MasterJumboQty + QtyToUpdate;
                }
            }

        }

        private DateTime GetNextDeliveryDateforBFGSMShade(string BF, string GSM, string Shade, DateTime currdatetime)
        {
            var Nextdate = ItemsAvailable.Where(w => w.BF == BF && w.GSM == GSM && w.Shade == Shade)
                                         .OrderBy(g => g.RequestedDate )
                                         .GroupBy(g => new { g.RequestedDate })
                                         .Select(g => new Item { RequestedDate = g.Key.RequestedDate })
                                         .SkipWhile(g => g.RequestedDate <= currdatetime)
                                         .FirstOrDefault();

            DateTime returnDate = Convert.ToDateTime("01/01/01");
            if(Nextdate != null)
            {
                returnDate = Nextdate.RequestedDate;
            }
            return returnDate;

        }

 
        #endregion HelperFunctions

        #region LogFunctions
        private void LogAllItems(List<Item> itemsAvailable)
        {
            decklelogger.InfoFormat("Items Available Dump.....");

            decklelogger.InfoFormat(String.Format("     {0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10} {6,-10} {7,-10} {8,-10} ", 
                               "OrdPrdID", "BF", "GSM", "Shade", "Width", "OrderQty", "PendingQty", "MasterJumboQty", "Req Date"));

            decklelogger.InfoFormat(String.Format("     {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10}",
                                             "----------"));


            foreach (Item item in itemsAvailable)
            {
                decklelogger.InfoFormat(String.Format("     {0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10} {6,-10} {7,-10} {8,-10}",
                                             item.OrderProductNo, item.BF, item.GSM, item.Shade, item.Size, item.OrderQty, item.PendingQty, item.MasterJumboQty, item.RequestedDate.ToShortDateString()));

            }
            decklelogger.InfoFormat(" ");

        }

        public void LogJumbo()
        {

            decklelogger.InfoFormat("");
            decklelogger.InfoFormat("Jumbo Dump.....");

            int i = 0;
            foreach (Jumbo jumbo in JumboRequired)
            {
                decklelogger.InfoFormat(String.Format("     [Jumbo# {0,-3}]  BF:{1,-10} GSM:{2,-10} Shade:{3,-10} AvlSize:{4,-10} AvlWt:{5,-10} Open:{6,-10}   UsedSize:{7,-10} UsedWt:{8,-10} MastJumbo:{9,-10} IsFull:{10,-10} ReqDT:{11,-10}",
                  i, jumbo.BF, jumbo.GSM, jumbo.Shade, jumbo.Availablesize, jumbo.AvailableWeight, jumbo.IsOpen, (MaxDeckleSize - jumbo.Availablesize), (MaxJumboWeight - jumbo.AvailableWeight), jumbo.MasterJumboNo, jumbo.IsFullSize, jumbo.RequestedDate.ToShortDateString()));

                List<Item> Items = jumbo.ItemsAssigned;
                if (Items != null)
                {
                    int j = 0;
                    foreach (Item item in Items)
                    {
                        decklelogger.InfoFormat(String.Format("          [Item# {0,-3}]  Type:{1,-10} OPID:{2,-10} BF:{3,-10} GSM:{4,-10} Shade:{5,-10} Size:{6,-10} Qty:{7,-10} SideCutID:{8,-10} ReqDT:{9,-10} {10,-10} ",
                                 j, item.Type, item.OrderProductNo, item.BF, item.GSM, item.Shade, item.Size, item.OrderQty, item.SidecutID, item.RequestedDate.ToShortDateString(), "", ""));

                        j++;
                    }
                    decklelogger.InfoFormat("");

                }
                i++;
            }
            decklelogger.InfoFormat("");
            decklelogger.InfoFormat("");

        }

        private void LogMasterJumbo()
        {

            decklelogger.InfoFormat(""); 
            decklelogger.InfoFormat("----Master Jumbo Dump---");

            for (int localMastJumboNo = 1; localMastJumboNo <= MasterJumboNo; localMastJumboNo++)
            {
                var JumboProdSch = JumboRequired.Where(t => t.MasterJumboNo == localMastJumboNo)
                                                .OrderBy(t => t.MasterJumboNo).ToList();
                decklelogger.InfoFormat("Master Roll # " + localMastJumboNo.ToString()); 

                foreach (Jumbo jumbo in JumboProdSch)
                {
                    decklelogger.InfoFormat("    Deckle: " + (MaxDeckleSize - jumbo.Availablesize).ToString() + "  Qty: " + (MaxJumboWeight - jumbo.AvailableWeight).ToString() + " Jumbo#: " + JumboRequired.IndexOf(jumbo) + " ReqDT: " + jumbo.RequestedDate.ToShortDateString());

                    int j = 1;
                    foreach (Item item in jumbo.ItemsAssigned)
                    {
                        decklelogger.InfoFormat(String.Format("               [Item  {0,-3}]  Type:{1,-10} OPID:{2,-10} BF:{3,-10} GSM:{4,-10} Shade:{5,-10} Size:{6,-10} Qty:{7,-10} SideCutID:{8,-10} ReqDt{9,-10} {10,-10} ",
                                 j, item.Type, item.OrderProductNo, item.BF, item.GSM, item.Shade, item.Size, item.OrderQty, item.SidecutID, item.RequestedDate.ToShortDateString(), "", ""));
                        j++;
                    }
                }


            }
        }


        private void LogProductionSchedule(ProductionRun newprdrun, List<ProductionJumbo> ProductionJumboList, List<ProductionChild> ProductionChildList)
        {
            decklelogger.InfoFormat("");
            decklelogger.InfoFormat("---Production Schedule created with below data-----");
            decklelogger.InfoFormat("ProductionRun Record: RunID: " + newprdrun.pr_id.ToString() );
            decklelogger.InfoFormat("    PapermillID:" + newprdrun.papermill_id.ToString() + "  Est Start: " + newprdrun.estimated_start.ToString() +
                "  Est End: " + newprdrun.estimated_end.ToString() + "  RunTime: " + newprdrun.run_time.ToString() +
                "  Est Qty: " + newprdrun.estimated_qty.ToString());
            decklelogger.InfoFormat("ProductionJumbo Record: ");
            decklelogger.InfoFormat(String.Format("     {0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10} {6,-10} {7,-10} {8,-10} {9,-10}",
                                             "PJ_ID", "PR_ID", "Jumbo#", "BF", "GSM", "Shade", "Pl Width", "Pl Qty", "JumboWidth", "Est. Start"));
            decklelogger.InfoFormat(String.Format("     {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10}",
                                             "----------"));
            foreach (ProductionJumbo prdjumbo in ProductionJumboList)
            {
                decklelogger.InfoFormat(String.Format("     {0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10} {6,-10} {7,-10} {8,-10} {9,-10}",
                    prdjumbo.pj_id, prdjumbo.pr_id, prdjumbo.jumbo_no, prdjumbo.bf_code, prdjumbo.gsm_code, prdjumbo.shade_code, prdjumbo.planned_width,
                    prdjumbo.planned_qty, prdjumbo.jumbo_width, prdjumbo.estimated_start));
            }
 

            decklelogger.InfoFormat("ProductionChild Record: ");
            decklelogger.InfoFormat(String.Format("          {0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10} {6,-10} ",
                                             "PC_ID", "PJ_ID", "OrdPrdId", "SideCutId", "Width", "QTY", "SEQ"));
            decklelogger.InfoFormat(String.Format("          {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} {0,-10} ",
                                             "----------"));
            foreach (ProductionChild prdchild in ProductionChildList)
            {
                decklelogger.InfoFormat(String.Format("          {0,-10} {1,-10} {2,-10} {3,-10} {4,-10} {5,-10}  {6,-10}",
                    prdchild.pc_id, prdchild.pj_id, prdchild.order_product_id, prdchild.sidecut_id, prdchild.width, prdchild.qty, prdchild.sequence));
            }
        }
        #endregion LogFunctions


    }


}