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
    public class CoreRepository : ICore
    {

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private MWVDBContext db = new MWVDBContext();

        public CoreRepository()
        {

        }

        public List<Core> GetCore()
        {
            try
            {
                var lstCore = db.Cores;
                return lstCore.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CoreRepository->GetCore:", Ex);
                return null;
            }
        }


        public bool CheckDuplicateCoreCode(string CoreCode)
        {
            bool isDuplicateCoreFound;
            var lstCore = db.Cores.ToList();
            var dupCoreCode = lstCore.Find(x => x.core_code == CoreCode);

            if (dupCoreCode != null)//duplicate found
            {
                isDuplicateCoreFound = true;
            }
            else   //no duplicate core code found. 
            {
                isDuplicateCoreFound = false;
            }

            return isDuplicateCoreFound;
        }

        public bool AddCore(string CoreCode, string CoreDescription, string created_by)
        {
            try
            {
                //check whether user is adding dulicate core code
                bool isDuplicateCoreFound;
                isDuplicateCoreFound = CheckDuplicateCoreCode(CoreCode);
                if (isDuplicateCoreFound == false)  //no duplicate core code found , then add unique entry. 
                {
                    Core objCore = new Core();
                    objCore.core_code = CoreCode;
                    objCore.description = CoreDescription;
                    objCore.created_by = created_by;
                    objCore.created_on = DateTime.Now;
                    db.Cores.Add(objCore);
                    db.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CoreRepository->AddCore:", Ex);
                return false;
            }
        }


        public bool EditCore(string CoreCodeToEdit, string CoreCode, string CoreDescription, string modified_by)
        {
            try
            {
                bool isEdited;
                var objCore = db.Cores.Where(c => c.core_code == CoreCodeToEdit).FirstOrDefault();
                objCore.description = CoreDescription;
                objCore.modified_by = modified_by;
                objCore.modified_on = DateTime.Now;
                db.SaveChanges();
                isEdited = true;
                return isEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CoreRepository->EditCore:", Ex);
                return false;//fail to edit
            }
        }


        public bool DeleteCore(string CoreCode)
        {
            try
            {
                bool isDeleted;
                var objCore = db.Cores.Where(c => c.core_code == CoreCode).FirstOrDefault();
                db.Cores.Remove(objCore);
                db.SaveChanges();
                isDeleted = true;
                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CoreRepository->DeleteCore:", Ex);
                return false;//fail to del
            }
        }


    }
}