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
    //public class Bf
    //{
    //    public string bf_code { get; set; }
    //    public string description { get; set; }
    //}

    public class BfRepository : IBf
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private MWVDBContext db = new MWVDBContext();

        public BfRepository()
        {

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
                logger.Error("Error in BRepository->GetBfs:", Ex);
                return null;
            }
        }

        public bool CheckDuplicateBFCode(string BFCode)
        {
            bool isDuplicateBFFound;
            var lstBfs = db.Bfs.ToList();
            var dupBFCode = lstBfs.Find(x => x.bf_code == BFCode);

            if (dupBFCode != null)//duplicate found
            {
                isDuplicateBFFound = true;
            }
            else   //no duplicate bf code found. 
            {
                isDuplicateBFFound = false;
            }

            return isDuplicateBFFound;
        }

        public bool AddBF(string BFCode, string BFDescription, string created_by)
        {
            try
            {
                //check whether user is adding dulicate BF code
                bool isDuplicateBFFound;
                isDuplicateBFFound = CheckDuplicateBFCode(BFCode);
                if (isDuplicateBFFound == false)  //no duplicate bf code found , then add unique entry. 
                {
                    Bf objBF = new Bf();
                    objBF.bf_code = BFCode;
                    objBF.description = BFDescription;
                    objBF.created_by = created_by;
                    objBF.created_on = DateTime.Now;
                    db.Bfs.Add(objBF);
                    db.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in BFRepository->AddBF:", Ex);
                return false;
            }
        }

        public bool EditBF(string BFCodeToEdit, string BFCode, string BFDescription, string modified_by)
        {
            try
            {
                bool isEdited;
                var objBF = db.Bfs.Where(c => c.bf_code == BFCodeToEdit).FirstOrDefault();
                objBF.description = BFDescription;
                objBF.modified_by = modified_by;
                objBF.modified_on = DateTime.Now;
                db.SaveChanges();
                isEdited = true;
                return isEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in BFRepository->EditBF:", Ex);
                return false;//fail to edit
            }
        }

        public bool DeleteBF(string BFCode)
        {
            try
            {
                bool isDeleted;
                var objBF = db.Bfs.Where(c => c.bf_code == BFCode).FirstOrDefault();
                if (objBF.Products.Count > 0) //product found so dont allow to delete.
                    isDeleted = false;
                else
                {
                    db.Bfs.Remove(objBF);
                    db.SaveChanges();
                    isDeleted = true;
                }
                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in BFRepository->DeleteBF:", Ex);
                return false;//fail to del
            }
        }

    }
}