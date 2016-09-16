using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using System.Web.WebPages.Razor;
using Microsoft.AspNet.Identity;
using MWV.Models;
using MWV.DBContext;
using MWV.Repository.Interfaces;
using System.Reflection;

namespace MWV.Repository.Implementation
{
    public class ProductPriceRepository : IProductPrice
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        public ProductPriceRepository()
        {

        }

        public List<Product_prices> GetListOfProductsPrice()
        {
            try
            {
                var lstProductPrice = db.Product_prices;
                return lstProductPrice.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductPriceRepository->GetListOfProductPrice:", Ex);
                return null;
            }
        }
        //public List<Gsm> GetGsms()
        //{
        //    try
        //    {
        //        var lstGsms = db.Gsms;
        //        return lstGsms.ToList();
        //    }
        //    catch (Exception Ex)
        //    {
        //        logger.Error("Error in ProductPriceRepository->GetGsms:", Ex);
        //        return null;
        //    }
        //}

        public bool AddProductPrice(int CustomerID, string ProductCode, string ShadeCode, decimal UnitPrice, DateTime StartDate, DateTime EndDate, string created_by)
        {
            try
            {
                Product_prices objProdprices = new Product_prices();
                objProdprices.customer_id = CustomerID;
                objProdprices.product_code = ProductCode;
                objProdprices.shade_code = ShadeCode;
                objProdprices.unit_price = UnitPrice;
                objProdprices.start_date = StartDate;
                objProdprices.end_date = EndDate;
                objProdprices.created_by = created_by;
                objProdprices.created_on = DateTime.Now;

                db.Product_prices.Add(objProdprices);
                db.SaveChanges();
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductPriceRepository->AddProductPrice:", Ex);
                return false;
            }
        }

        public bool EditProductPrice(int ProductPriceID, int CustomerID, string ProductCode, string ShadeCode, decimal UnitPrice, DateTime StartDate, DateTime EndDate, string modified_by)
        {
            try
            {
                bool isEdited;
                var objProdprices = db.Product_prices.Where(c => c.product_price_id == ProductPriceID).FirstOrDefault();
                objProdprices.customer_id = CustomerID;
                objProdprices.product_code = ProductCode;
                objProdprices.shade_code = ShadeCode;
                objProdprices.unit_price = UnitPrice;
                objProdprices.start_date = StartDate;
                objProdprices.end_date = EndDate;
                objProdprices.modified_by = modified_by;
                objProdprices.modified_on = DateTime.Now;

                db.SaveChanges();
                isEdited = true;
                return isEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductPriceRepository->EditProductPrice:", Ex);
                return false;
            }
        }


    }
}