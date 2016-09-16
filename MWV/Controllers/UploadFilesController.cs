using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MWV.DBContext;
using System.Collections;
using MWV.Repository;
using MWV.Repository.Implementation;
using MWV.Models;
using System.Data;
using OfficeOpenXml;
using System.Reflection;



namespace MWV.Controllers
{
    public class UploadFilesController : Controller
    {
        private MWVDBContext db = new MWVDBContext();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //private MWVDBContext db = new MWVDBContext();

        //
        // GET: /UploadFiles/


        public ActionResult Index()
        {

            return View();
        }
        public ActionResult UploadOrders()
        {

            return View("_UploadOrders");
        }
        public class GetDataCsvFiles
        {
            public string journalTemlNm;
            public string JrnlBatNo;
            public string postingDate;
            public string reelNo;
            public string Text;
            public string itemcat;
            public string productGrp;
            public decimal? qty;
            public string variantCode;
            public decimal? size;
            public decimal? lenth;
            public string RealBarcodeNo;
            public string macOprate;
            public string atualStart;
        }

        public class OrdersCSV
        {
            public int Order_id;
            public DateTime Order_date;
            public string AgentName;
            public string CustomerName;
            public string BF_code;
            public string Gsm_code;
            public string Shade_code;
            public decimal width;
            public decimal Qty;
            public decimal Diameter;
            public Int16 Core;
            public DateTime Requested_delivery_date;
            public decimal price;
            public decimal totalAmount;
            public string NotUpdatesReason;
            public string Pono { get; set; }
        }

        [HttpPost]
        public ActionResult GetFile(HttpPostedFileBase file)
        {

            List<GetDataCsvFiles> gdList = new List<GetDataCsvFiles>();
            List<GetDataCsvFiles> gdUnsucessList = new List<GetDataCsvFiles>();
            var filename = ""; string ext = "";
            if (file != null)
            {
                filename = Path.GetFileName(file.FileName);
                ext = Path.GetExtension(filename);
            }
            else
            {
                ViewBag.errorMsg = "Please Select *CSV File!";
                return View("Index");

            }

            if (file != null && file.ContentLength > 0 && ext == ".csv")
            {
                Int64 ProdoctJumboID, rowsUpdated = 0, NotUpdated = 0;
                int i = 0;
                if (!System.IO.Directory.Exists(Server.MapPath("~/MWV/Upload/navision/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/MWV/Upload/navision/"));
                }
                file.SaveAs(Server.MapPath("~/MWV/Upload/navision/" + filename));
                var path = Path.Combine(Server.MapPath("~/MWV/Upload/navision/"), filename);
                //file.SaveAs(path);
                const char fieldSeparator = ',';
                string filePath = path;
                using (StreamReader SR = new StreamReader(filePath))    // the way to go
                {
                    GetDataCsvFiles gd = new GetDataCsvFiles();
                    while (!SR.EndOfStream) //best way to do it
                    {
                        i++;
                        var CSValues = SR.ReadLine().Split(fieldSeparator);
                        if (CSValues.Length > 2 && i > 2)
                        {
                            #region Get Values From Csv File

                            gd.journalTemlNm = CSValues[0];
                            gd.JrnlBatNo = CSValues[1];
                            gd.postingDate = CSValues[2];
                            gd.reelNo = CSValues[3];
                            gd.Text = "" + CSValues[4];
                            gd.itemcat = CSValues[5];
                            gd.productGrp = CSValues[6];
                            gd.qty = Convert.ToDecimal(CSValues[7]);
                            gd.qty = gd.qty / 1000;
                            gd.variantCode = CSValues[8];
                            gd.size = Convert.ToDecimal(CSValues[9]);
                            gd.lenth = Convert.ToDecimal(CSValues[10]);
                            gd.RealBarcodeNo = CSValues[11];
                            gd.macOprate = CSValues[12];
                            gd.atualStart = CSValues[13];
                            #endregion

                            //Query to Production Table
                            var query = (from j in db.ProductionJumbo where j.jumbo_no.ToString() == gd.Text select new { j.pj_id }).ToList();
                            //totalRecords = query.Count();
                            foreach (var Pjid in query)
                            {
                                ProdoctJumboID = Pjid.pj_id;
                                var queryUpdate = db.ProductionChild.FirstOrDefault(x => x.pj_id == ProdoctJumboID && x.width == gd.size && x.qty == gd.qty && x.external_startdate == null);
                                if (queryUpdate != null)
                                {
                                    gdList.Add(new GetDataCsvFiles() { journalTemlNm = gd.journalTemlNm, itemcat = gd.itemcat, atualStart = gd.atualStart, JrnlBatNo = gd.JrnlBatNo, lenth = gd.lenth, macOprate = gd.macOprate, postingDate = gd.postingDate, productGrp = gd.productGrp, qty = gd.qty, RealBarcodeNo = gd.RealBarcodeNo, reelNo = gd.reelNo, size = gd.size, Text = gd.Text, variantCode = gd.variantCode });
                                    ViewBag.UpdatedList = gdList;

                                    rowsUpdated++;
                                    queryUpdate.actual_start = Convert.ToDateTime(gd.atualStart);
                                    queryUpdate.external_startdate = Convert.ToDateTime(gd.atualStart);
                                    queryUpdate.child_rollno = gd.RealBarcodeNo;
                                    db.SaveChanges();
                                }
                                else
                                {

                                    gdUnsucessList.Add(new GetDataCsvFiles() { journalTemlNm = gd.journalTemlNm, itemcat = gd.itemcat, atualStart = gd.atualStart, JrnlBatNo = gd.JrnlBatNo, lenth = gd.lenth, macOprate = gd.macOprate, postingDate = gd.postingDate, productGrp = gd.productGrp, qty = gd.qty, RealBarcodeNo = gd.RealBarcodeNo, reelNo = gd.reelNo, size = gd.size, Text = gd.Text, variantCode = gd.variantCode });
                                    ViewBag.unpdatedList = gdUnsucessList;
                                }
                            }

                        }
                    }


                    if (i == 4)
                    {
                        i = i - 3;
                        NotUpdated = 0;
                        ViewBag.NoUpdate = "Not Updated =" + NotUpdated;
                        ViewBag.TotalRecords = "Total Records =" + i;
                        ViewBag.RowsUpdated = "Total Updated =" + rowsUpdated;


                    }
                    else
                    {
                        i = i - 2;
                        NotUpdated = i - rowsUpdated;
                        ViewBag.NoUpdate = "Not Updated =" + NotUpdated;
                        ViewBag.TotalRecords = "Total Records =" + i;
                        ViewBag.RowsUpdated = "Total Updated =" + rowsUpdated;

                    }

                }
            }
            else
            {
                ViewBag.errorMsg = "Please Upload *CSV File Format!";
            }

            return View("Index");
        }

        //For Upload of Orders CSV
        public PartialViewResult UploadOrdersCSV()
        {
            HttpPostedFileBase file = this.Request.Files[0];
            List<OrdersCSV> OrderList = new List<OrdersCSV>();
            List<OrdersCSV> SucessList = new List<OrdersCSV>();
            List<OrdersCSV> FailedList = new List<OrdersCSV>();
            OrderRepository objorder = new OrderRepository();
            List<Order_Products> objOp = new List<Order_Products>();
            Order_Products ObjOrderProd = new Order_Products();

            var filename = ""; string ext = "";
            if (file != null)
            {
                filename = Path.GetFileName(file.FileName);
                ext = Path.GetExtension(filename);
            }
            else
            {
                ViewBag.errorMsg = "Please Select *xlsx File!";
                return PartialView("_UploadOrders");

            }
            if (file != null && file.ContentLength > 0 && ext == ".xlsx")
            {
                Int64 ProdoctJumboID, rowsUpdated = 0, NotUpdated = 0;
                int i = 0; int Totalcount = 0;
                if (!System.IO.Directory.Exists(Server.MapPath("~/MWV/Upload/Orders/")))
                {
                    Directory.CreateDirectory(Server.MapPath("~/MWV/Upload/Orders/"));
                }
                file.SaveAs(Server.MapPath("~/MWV/Upload/Orders/" + filename));
                var path = Path.Combine(Server.MapPath("~/MWV/Upload/Orders/"), filename);

                // Get the file we are going to process
                var existingFile = new FileInfo(path);
                // Open and read the XlSX file.
                using (var package = new ExcelPackage(existingFile))
                {
                    // Get the work book in the file
                    ExcelWorkbook workBook = package.Workbook;
                    if (workBook != null)
                    {
                        if (workBook.Worksheets.Count > 0)
                        {
                            // Get the first worksheet
                            foreach (var currentWorksheet in workBook.Worksheets)
                            {
                                int currRow = currentWorksheet.Dimension.End.Row;
                                for (int p = currentWorksheet.Dimension.Start.Column; p <= currRow; p++)
                                {

                                    // read some data
                                    // object col1Header = currentWorksheet.Cells[0, 1].Value;
                                    if (p != 1)
                                    {
                                        OrdersCSV ord = new OrdersCSV();
                                        ord.Order_id = Convert.ToInt16(currentWorksheet.Cells[p, 1].Text);
                                        ord.Order_date = Convert.ToDateTime(currentWorksheet.Cells[p, 2].Text);
                                        ord.AgentName = Convert.ToString(currentWorksheet.Cells[p, 3].Text);
                                        ord.CustomerName = Convert.ToString(currentWorksheet.Cells[p, 4].Text);
                                        ord.BF_code = "" + Convert.ToString(currentWorksheet.Cells[p, 5].Text);
                                        ord.Gsm_code = Convert.ToString(currentWorksheet.Cells[p, 6].Text);
                                        ord.Shade_code = Convert.ToString(currentWorksheet.Cells[p, 7].Text);
                                        ord.width = Convert.ToDecimal(currentWorksheet.Cells[p, 8].Text);
                                        ord.Qty = Convert.ToDecimal(currentWorksheet.Cells[p, 9].Text);
                                        ord.price = Convert.ToDecimal(currentWorksheet.Cells[p, 10].Text);
                                        ord.Diameter = Convert.ToDecimal(currentWorksheet.Cells[p, 11].Text);
                                        ord.Core = Convert.ToInt16(currentWorksheet.Cells[p, 12].Text);
                                        ord.Requested_delivery_date = Convert.ToDateTime(currentWorksheet.Cells[p, 13].Text);
                                        ord.Pono = Convert.ToString(currentWorksheet.Cells[p, 14].Text);

                                        OrderList.Add(ord);
                                        Totalcount++;

                                    }
                                }
                            }
                        }

                    }
                }
                int orderID = 0; decimal totalamt = 0;
                //We have the complete orders loaded in list now,
                //get unique order numbers from the list so that we can commit them as one batch in DB
                var UniqueOrders = OrderList.GroupBy(g => new { g.Order_id })
                                .Select(t => new OrdersCSV { Order_id = t.Key.Order_id });

                foreach (var unqitem in UniqueOrders)
                {
                    totalamt = 0; orderID = 0;
                    //Get all products within an order from the uploaded CSV
                    var SingleOrders = OrderList.Where(t => t.Order_id == unqitem.Order_id)
                                        .Select(t => t);


                    bool firstitem = true;
                    int Agent_ID = 0; int Cust_ID = 0; int chkCount = 0;
                    foreach (var prod in SingleOrders)
                    {
                        if (firstitem == true)
                        {

                            //Lets create the Header Entry for Order
                            // Get agentid from order repo
                            Agent_ID = objorder.GetAgentbyName(prod.AgentName);
                            Cust_ID = objorder.GetCustomerbyName(prod.CustomerName, Agent_ID);
                            //if agentid is null/0 then this is an error 
                            //if customerid is null/o then error
                            if (Agent_ID != 0 && Cust_ID != 0)
                            {
                                //Create order object and insert in DB, return orderID
                                Order objOrderdata = new Order();
                                objOrderdata.order_date = prod.Order_date;
                                objOrderdata.agent_id = Agent_ID;
                                objOrderdata.customer_id = Cust_ID;
                                objOrderdata.customerpo = prod.Pono;
                                objOrderdata.status = "Created";
                                orderID = objorder.AddOrderfromCSV(objOrderdata);
                            }
                            firstitem = false;
                        }
                        //create a OrderProucts list
                        // Create the individual orderprduct record objOp ObjOrderProd
                        string product_code = objorder.GetProductCodeCSV(prod.BF_code, prod.Gsm_code);
                        //decimal unit_Price = objorder.GetUnitPrice(Cust_ID, product_code);
                        if (product_code != "" && orderID != 0)
                        {
                            //List<decimal> TotalAmt =new List<decimal>();
                            //TotalAmt.Add(+prod.price);

                            totalamt = totalamt + prod.Qty * prod.price;
                            ObjOrderProd.order_id = orderID;
                            ObjOrderProd.product_code = product_code;
                            ObjOrderProd.shade_code = prod.Shade_code;
                            ObjOrderProd.width = prod.width;
                            ObjOrderProd.unit_price = prod.price;
                            ObjOrderProd.qty = prod.Qty;
                            //objorder.Amount=prod.am
                            ObjOrderProd.status = "Created";
                            ObjOrderProd.diameter = prod.Diameter;
                            ObjOrderProd.amount = prod.Qty * prod.price;
                            ObjOrderProd.core = prod.Core;
                            ObjOrderProd.created_on = DateTime.Now;
                            ObjOrderProd.unit_price = prod.price;
                            ObjOrderProd.width_planned = 0;
                            ObjOrderProd.qty_dispatched_byFactory = 0;
                            ObjOrderProd.qty_scheduled = 0;
                            ObjOrderProd.qty_planned_byAgent = 0;
                            ObjOrderProd.diameter = 0;
                            ObjOrderProd.core = 0;
                            ObjOrderProd.requested_delivery_date = prod.Requested_delivery_date;
                            objOp.Add(ObjOrderProd);

                            //Commit the List
                            //If order product gave error, then put this in error and delete the order record
                            int Objorder_Id = objorder.AddOrderProductsfromCSV(ObjOrderProd);
                            if (Objorder_Id != 0)
                            {
                                SucessList.Add(new OrdersCSV() { AgentName = prod.AgentName, BF_code = prod.BF_code, Core = prod.Core, CustomerName = prod.CustomerName, Diameter = prod.Diameter, Gsm_code = prod.Gsm_code, Order_date = prod.Order_date, Order_id = prod.Order_id, Qty = prod.Qty, price = prod.price, Requested_delivery_date = prod.Requested_delivery_date, Shade_code = prod.Shade_code, width = prod.width,Pono=prod.Pono });
                            }
                            else
                            {
                                if (Agent_ID == 0 || Cust_ID == 0)
                                {
                                    FailedList.Add(new OrdersCSV() { AgentName = prod.AgentName, BF_code = prod.BF_code, Core = prod.Core, CustomerName = prod.CustomerName, Diameter = prod.Diameter, Gsm_code = prod.Gsm_code, Order_date = prod.Order_date, Order_id = prod.Order_id, Qty = prod.Qty, price = prod.price, Requested_delivery_date = prod.Requested_delivery_date, Shade_code = prod.Shade_code, width = prod.width,Pono=prod.Pono,NotUpdatesReason = "Incorrect Agent Name or Customer name" });
                                }
                                else if (product_code == "")
                                {
                                    FailedList.Add(new OrdersCSV() { AgentName = prod.AgentName, BF_code = prod.BF_code, Core = prod.Core, CustomerName = prod.CustomerName, Diameter = prod.Diameter, Gsm_code = prod.Gsm_code, Order_date = prod.Order_date, Order_id = prod.Order_id, Qty = prod.Qty, price = prod.price, Requested_delivery_date = prod.Requested_delivery_date, Shade_code = prod.Shade_code, width = prod.width,Pono=prod.Pono,NotUpdatesReason = "Product Code Not Found" });
                                }
                            }
                        }
                        else
                        {
                            if (Agent_ID == 0 || Cust_ID == 0)
                            {
                                FailedList.Add(new OrdersCSV() { AgentName = prod.AgentName, BF_code = prod.BF_code, Core = prod.Core, CustomerName = prod.CustomerName, Diameter = prod.Diameter, Gsm_code = prod.Gsm_code, Order_date = prod.Order_date, Order_id = prod.Order_id, Qty = prod.Qty, price = prod.price, Requested_delivery_date = prod.Requested_delivery_date, Shade_code = prod.Shade_code, width = prod.width,Pono=prod.Pono,NotUpdatesReason = "Incorrect Agent Name or Customer name" });
                            }
                            else if (product_code == "")
                            {
                                FailedList.Add(new OrdersCSV() { AgentName = prod.AgentName, BF_code = prod.BF_code, Core = prod.Core, CustomerName = prod.CustomerName, Diameter = prod.Diameter, Gsm_code = prod.Gsm_code, Order_date = prod.Order_date, Order_id = prod.Order_id, Qty = prod.Qty, price = prod.price, Requested_delivery_date = prod.Requested_delivery_date, Shade_code = prod.Shade_code, width = prod.width,Pono=prod.Pono,NotUpdatesReason = "Product Code Not Found" });
                            }
                        }
                    }
                    //Display Sucess list and Error list of order products
                    //Update total Amount in Order table
                    bool AmtStatus = objorder.Updatetotalamt(totalamt, orderID);

                    //Check order has order product if no delete order
                    if (orderID != 0)
                    {
                        CreateUploadPdf(orderID);
                        int status = objorder.CheckOrderInTable(orderID);
                        if (status == 1)
                        {
                            SucessList = null;
                        }
                    }

                } //end for - Indivudual order

                ViewBag.Sucessorders = SucessList;
                ViewBag.UnsucessOrder = FailedList;
                ViewBag.SucessCount = "Total Records Success =" + SucessList.Count();
                ViewBag.UnsucessCount = "Total Records Failed =" + FailedList.Count();

                int count = OrderList.Count;
                ViewBag.TotalRecords = "Total Orders =" + count;

            }
            else
            {
                ViewBag.errorMsg = "Please Upload *xlsx File Format!";
            }

            return PartialView("_UploadOrders");
        }

        //For Uploaded Order Create PDF
        public bool CreateUploadPdf(int lastid)
        {
            try
            {
                Order objorder = new Order();
                QuickViewModel objquickview = new QuickViewModel();
                OrderRepository ores = new OrderRepository();
                objquickview.Order_Products = ores.OrderProductsPdf(lastid).ToList();
                objquickview.OrderDetails = ores.OrderPdf(lastid).ToList();
                var totaLAmount = ores.Amount(lastid);
                List<Order_Products> lst = new List<Order_Products>();
                Order_Products op = new Order_Products();
                foreach (var ord in objquickview.OrderDetails)
                {
                    ViewBag.AgentName = ord.Agent.name;
                    ViewBag.Customername = ord.Customer.name;
                    ViewBag.PurchaseordNo = ord.order_id;
                    lastid = ord.order_id;
                    ViewBag.orderdate = ord.order_date;
                    ViewBag.CustPo = ord.customerpo;
                    ViewBag.totalamt = totaLAmount;
                    foreach (var order_prod in objquickview.Order_Products)
                    {

                        op.requested_delivery_date = order_prod.requested_delivery_date;
                        op.product_code = order_prod.product_code;
                        op.shade_code = order_prod.shade_code;
                        op.width = order_prod.width;
                        op.qty = order_prod.qty;
                        op.amount = order_prod.amount;
                        lst.Add(new Order_Products() { requested_delivery_date = op.requested_delivery_date, product_code = op.product_code, shade_code = op.shade_code, width = op.width, qty = op.qty, amount = op.amount });

                    }

                }
                MvcRazorToPdf objPdf = new MvcRazorToPdf();
                string agntname = ores.GetAgentNamePdf(lastid);
                agntname = agntname.Replace(" ", "_");
                byte[] pdfOutput = objPdf.GeneratePdfOutput(this.ControllerContext, lst, "CreateOrderPdf");
                string CurDate = DateTime.Today.ToString("MMM-yyyy");
                bool exists = System.IO.Directory.Exists(Server.MapPath("~/MWV/PDF/" + CurDate));
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(Server.MapPath("~/MWV/PDF/" + CurDate));
                }
                string finalPath = "/MWV/PDF/" + CurDate;
                string fullPath = Server.MapPath(finalPath + "/" + agntname + "_PO_" + lastid.ToString() + ".pdf");
                string attachment1 = "/MWV/PDF/" + CurDate + "/" + agntname + "_PO_" + lastid.ToString() + ".pdf";
                var updateAttachment = db.Orders.FirstOrDefault(k => k.order_id == lastid);
                if (updateAttachment != null)
                {
                    updateAttachment.pdf_file = attachment1;
                    db.SaveChanges();

                }
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                System.IO.File.WriteAllBytes(fullPath, pdfOutput);
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in UploadfileController->CreateUploadPdf:", Ex);
                return false;
            }

        }


    }
}