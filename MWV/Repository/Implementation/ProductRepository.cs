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
    public class ProductRepository : IProduct
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        public ProductRepository()
        {

        }

        public List<Product> GetListOfProducts()
        {
            try
            {
                var lstProduct = db.Products;
                return lstProduct.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductRepository->GetListOfProducts:", Ex);
                return null;
            }
        }

        public List<Bf> GetBfs()
        {
            try
            {
                var lstBfs = db.Bfs;
                return lstBfs.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductRepository->GetBfs:", Ex);
                return null;
            }

        }

        public List<Gsm> GetGsms()
        {
            try
            {
                var lstGsms = db.Gsms;
                return lstGsms.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductRepository->GetGsms:", Ex);
                return null;
            }

        }

        public bool AddProduct(string ProductCode, string GSMCode, string BFCode, string ProductDescription, string created_by)
        {
            try
            {
                Product objProd = new Product();
                objProd.product_code = ProductCode;
                objProd.gsm_code = GSMCode;
                objProd.bf_code = BFCode;
                objProd.description = ProductDescription;
                objProd.created_by = created_by;
                objProd.created_on = DateTime.Now;
                db.Products.Add(objProd);
                db.SaveChanges();
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductRepository->AddProduct:", Ex);
                return false;
            }
        }

        public bool EditProduct(string ProductCode, string GSMCode, string BFCode, string ProductDescription, string modified_by)
        {
            try
            {
                bool isEdited;
                var objProd = db.Products.Where(c => c.product_code == ProductCode).FirstOrDefault();
                objProd.gsm_code = GSMCode;
                objProd.bf_code = BFCode;
                objProd.description = ProductDescription;
                objProd.modified_by = modified_by;
                objProd.modified_on = DateTime.Now;
                db.SaveChanges();
                isEdited = true;
                return isEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductRepository->EditProduct:", Ex);
                return false;//fail to edit
            }
        }

        public bool DeleteProduct(string ProductCode)
        {
            try
            {
                bool isDeleted;
                var objProd = db.Products.Where(c => c.product_code == ProductCode).FirstOrDefault();
                //if (objProd.Product_price.Count > 0) //product price found so dont allow to delete.
                //    isDeleted = false;
                // else
                // {
                //     db.Products.Remove(objProd);
                //     db.SaveChanges();
                //     isDeleted = true;
                // }

                db.Products.Remove(objProd);
                db.SaveChanges();// if gives error then it's constranit violation, return false. 
                isDeleted = true;
                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductRepository->DeleteProduct:", Ex);
                return false;//fail to del
            }
        }

    }
}