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
    public class ShadeRepository : IShade
    {

        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        public ShadeRepository()
        {

        }

        public List<Shade> GetShades()
        {
            try
            {
                var lstShades = db.Shades;
                return lstShades.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ShadeRepository->GetShades:", Ex);
                return null;
            }
        }

        public bool CheckDuplicateShadeCode(string ShadeCode)
        {
            bool isDuplicateShadeFound;
            var lstShades = db.Shades.ToList();
            var dupShadeCode = lstShades.Find(x => x.shade_code == ShadeCode);

            if (dupShadeCode != null)//duplicate found
            {
                isDuplicateShadeFound = true;
            }
            else   //no duplicate bf code found. 
            {
                isDuplicateShadeFound = false;
            }

            return isDuplicateShadeFound;
        }



        public bool AddShade(string ShadeCode, string ShadeDescription, string created_by)
        {
            try
            {
                //check whether user is adding dulicate shade code
                bool isDuplicateShadeFound;
                isDuplicateShadeFound = CheckDuplicateShadeCode(ShadeCode);
                if (isDuplicateShadeFound == false)  //no duplicate shade code found , then add unique entry. 
                {
                    Shade objShade = new Shade();
                    objShade.shade_code = ShadeCode;
                    objShade.description = ShadeDescription;
                    objShade.created_by = created_by;
                    objShade.created_on = DateTime.Now;
                    db.Shades.Add(objShade);
                    db.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ShadeRepository->AddShade:", Ex);
                return false;
            }
        }


        public bool EditShade(string ShadeCodeToEdit, string ShadeCode, string ShadeDescription, string modified_by)
        {
            try
            {
                bool isEdited;
                var objShade = db.Shades.Where(c => c.shade_code == ShadeCodeToEdit).FirstOrDefault();
                objShade.description = ShadeDescription;
                objShade.modified_by = modified_by;
                objShade.modified_on = DateTime.Now;
                db.SaveChanges();
                isEdited = true;
                return isEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ShadeRepository->EditShade:", Ex);
                return false;//fail to edit
            }
        }

        public bool DeleteShade(string ShadeCode)
        {
            try
            {
                bool isDeleted;
                var objShade = db.Shades.Where(c => c.shade_code == ShadeCode).FirstOrDefault();
                if (objShade.Product_prices.Count > 0) //product price found so dont allow to delete.
                    isDeleted = false;
                else
                {
                    db.Shades.Remove(objShade);
                    db.SaveChanges();
                    isDeleted = true;
                }
                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ShadeRepository->DeleteShade:", Ex);
                return false;//fail to del
            }
        }

    }
}