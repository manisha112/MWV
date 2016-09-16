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
    public class ProductTimeLineRepository : IProductTimeLine
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        public ProductTimeLineRepository()
        {

        }

        public List<deckle_approvals> GetListOfDeckleApprovals()
        {
            try
            {
                var lstDeckleAppr = db.deckle_approvals;
                return lstDeckleAppr.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductTimeLineRepository->GetListOfDeckleApprovals:", Ex);
                return null;
            }
        }

        public List<ProductionTimeline> GetListOfProductTimeLines()
        {
            try
            {
                var lstProductTimeLine = db.ProductionTimeline;
                return lstProductTimeLine.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductTimeLineRepository->GetListOfProductTimeLine:", Ex);
                return null;
            }
        }

        public bool AddProductTimeLine(int PaperMillID, string BFCode, string GSMCode, string ShadeCode, int Speed, decimal TonPerHr, decimal TimePerTon, string created_by)
        {
            try
            {
                ProductionTimeline objProdTL = new ProductionTimeline();
                objProdTL.papermill_id = PaperMillID;
                objProdTL.bf_code = BFCode;
                objProdTL.gsm_code = GSMCode;
                objProdTL.shade_code = ShadeCode;
                objProdTL.speed = Speed;
                objProdTL.ton_per_hour = TonPerHr;
                objProdTL.time_per_ton = TimePerTon;
                objProdTL.created_by = created_by;
                objProdTL.created_on = DateTime.Now;

                db.ProductionTimeline.Add(objProdTL);
                db.SaveChanges();
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductTimeLineRepository->AddProductTimeLine:", Ex);
                return false;
            }
        }

        public bool EditProductTimeLine(int ProductTLIDToEdit, int Speed, decimal TonPerHr, decimal TimePerTon, string modified_by)
        {
            try
            {
                bool isEdited;
                var objProdTL = db.ProductionTimeline.Where(c => c.production_timeline_id == ProductTLIDToEdit).FirstOrDefault();

                objProdTL.speed = Speed;
                objProdTL.ton_per_hour = TonPerHr;
                objProdTL.time_per_ton = TimePerTon;
                objProdTL.modified_by = modified_by;
                objProdTL.modified_on = DateTime.Now;

                db.SaveChanges();
                isEdited = true;
                return isEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductTimeLineRepository->EditProductTimeLine:", Ex);
                return false;
            }
        }

        public bool DeleteProductTimeLine(int ProductTLIDToDel, int PaperMillID, string BFCode, string GSMCode, string ShadeCode)
        {
            try
            {
                bool isDeleted = false;

                List<deckle_approvals> lstDeckApp;
                lstDeckApp = GetListOfDeckleApprovals();
                List<deckle_approvals> lstDeckPairExists = lstDeckApp.Where(m => m.papermill_id == PaperMillID
                       && m.bf_code == BFCode && m.gsm_code == GSMCode && m.shade_code == ShadeCode).ToList();
                if (lstDeckPairExists.Count > 0)//if  pair exists in deckle_approvals , dont allow for deletion
                {
                    isDeleted = false;
                }
                else if (lstDeckPairExists == null || lstDeckPairExists.Count == 0)// if no  pair exists in deckle_approvals, allow to delete.
                {
                    var objProdTL = db.ProductionTimeline.Where(c => c.production_timeline_id == ProductTLIDToDel).FirstOrDefault();

                    db.ProductionTimeline.Remove(objProdTL);
                    db.SaveChanges();
                    isDeleted = true;
                }

                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductTimeLineRepository->DeleteProductTimeLine:", Ex);
                return false;//fail to del
            }
        }

    }
}