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
    public class GsmRepository : IGsm
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        public GsmRepository()
        {

        }

        public List<Gsm> GetGSMs()
        {
            try
            {
                var lstGSMs = db.Gsms;
                return lstGSMs.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in GsmRepository->GetGSMs:", Ex);
                return null;
            }
        }

        public bool CheckDuplicateGsmCode(string GsmCode)
        {
            bool isDuplicateGsmFound;
            var lstGSMs = db.Gsms.ToList();
            var dupCode = lstGSMs.Find(x => x.gsm_code.Trim() == GsmCode);

            if (dupCode != null)//duplicate found
            {
                isDuplicateGsmFound = true;
            }
            else   //no duplicate gsm code found. 
            {
                isDuplicateGsmFound = false;
            }

            return isDuplicateGsmFound;
        }

        public bool AddGsm(string GsmCode, string GsmDescription, string created_by)
        {
            try
            {
                //check whether user is adding dulicate gsm code
                bool isDuplicateFound;
                isDuplicateFound = CheckDuplicateGsmCode(GsmCode);
                if (isDuplicateFound == false)  //no duplicate gsm code found , then add unique entry. 
                {
                    Gsm objGsm = new Gsm();
                    objGsm.gsm_code = GsmCode;
                    objGsm.description = GsmDescription;
                    objGsm.created_by = created_by;
                    objGsm.created_on = DateTime.Now;
                    db.Gsms.Add(objGsm);
                    db.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in GsmRepository->AddGsm:", Ex);
                return false;
            }
        }

        public bool EditGsm(string GsmCodeToEdit, string GsmCode, string GsmDescription, string modified_by)
        {
            try
            {
                bool isEdited;
                var objGsm = db.Gsms.Where(c => c.gsm_code == GsmCodeToEdit).FirstOrDefault();
                objGsm.description = GsmDescription;
                objGsm.modified_by = modified_by;
                objGsm.modified_on = DateTime.Now;
                db.SaveChanges();
                isEdited = true;
                return isEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in GsmRepository->EditGsm:", Ex);
                return false;//fail to edit
            }
        }

        public bool DeleteGsm(string GsmCode)
        {
            try
            {
                bool isDeleted;
                var objGsm = db.Gsms.Where(c => c.gsm_code == GsmCode).FirstOrDefault();
                if (objGsm.Products.Count > 0) //product found so dont allow to delete.
                    isDeleted = false;
                else
                {
                    db.Gsms.Remove(objGsm);
                    db.SaveChanges();
                    isDeleted = true;
                }
                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in GsmRepository->DeleteGsm:", Ex);
                return false;//fail to del
            }
        }

    }
}