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
    public class PaperMillRepository : IPaperMill
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        public PaperMillRepository()
        {

        }

        public List<Papermill> GetListOfPaperMill()
        {
            try
            {
                var lstPapermill = db.Papermills;
                return lstPapermill.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in PaperMillRepository->GetListOfPaperMill:", Ex);
                return null;
            }
        }


        public bool AddPaperMill(decimal capacity, decimal min_width, decimal max_width, string location, decimal deckle_min, decimal deckle_max, int max_cuts, decimal min_diameter, decimal max_diameter, decimal max_weight_child, decimal min_weight_jumbo, decimal max_weight_jumbo, string name, string address, string created_by)
        {
            try
            {
                Papermill objPaperMill = new Papermill();
                objPaperMill.capacity = capacity;
                objPaperMill.min_width = min_width;
                objPaperMill.max_width = max_width;
                objPaperMill.location = location;
                objPaperMill.deckle_min = deckle_min;
                objPaperMill.deckle_max = deckle_max;
                objPaperMill.max_cuts = max_cuts;
                objPaperMill.min_diameter = min_diameter;
                objPaperMill.max_diameter = max_diameter;
                objPaperMill.max_weight_child = max_weight_child;
                objPaperMill.min_weight_jumbo = min_weight_jumbo;
                objPaperMill.max_weight_jumbo = max_weight_jumbo;
                objPaperMill.name = name;
                objPaperMill.address = address;
               // objPaperMill.status = status;
                objPaperMill.created_by = created_by;
                objPaperMill.created_on = DateTime.Now;

                db.Papermills.Add(objPaperMill);
                db.SaveChanges();
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in PaperMillRepository->AddPaperMill:", Ex);
                return false;
            }
        }

        public bool EditPaperMill(int PaperMillIDToEdit, decimal Capacity, decimal min_width, decimal max_width, string location, decimal deckle_min,
                          decimal deckle_max, int max_cuts, decimal min_diameter, decimal max_diameter, decimal max_weight_child,
                          decimal min_weight_jumbo, decimal max_weight_jumbo, string name, string address,  string modified_by)
        {
            try
            {
                bool isEdited;
                var objPaperMill = db.Papermills.Where(c => c.papermill_id == PaperMillIDToEdit).FirstOrDefault();
                objPaperMill.capacity = Capacity;
                objPaperMill.min_width = min_width;
                objPaperMill.max_width = max_width;
                objPaperMill.location = location;
                objPaperMill.deckle_min = deckle_min;
                objPaperMill.deckle_max = deckle_max;
                objPaperMill.max_cuts = max_cuts;
                objPaperMill.min_diameter = min_diameter;
                objPaperMill.max_diameter = max_diameter;
                objPaperMill.max_weight_child = max_weight_child;
                objPaperMill.min_weight_jumbo = min_weight_jumbo;
                objPaperMill.max_weight_jumbo = max_weight_jumbo;
                objPaperMill.name = name;
                objPaperMill.address = address;
               // objPaperMill.status = status;
                objPaperMill.modified_by = modified_by;
                objPaperMill.modified_on = DateTime.Now;

                db.SaveChanges();
                isEdited = true;
                return isEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in PaperMillRepository->EditPaperMill:", Ex);
                return false;
            }
        }

        public bool DeletePaperMill(int PaperMillIDToDel)
        {
            try
            {
                bool isDeleted;
                var objPaperMill = db.Papermills.Where(c => c.papermill_id == PaperMillIDToDel).FirstOrDefault();
                db.Papermills.Remove(objPaperMill);
                db.SaveChanges();// if gives error then it's constranit violation, return false. 
                isDeleted = true;
                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in PaperMillRepository->DeletePaperMill:", Ex);
                return false;//fail to del
            }
        }



    }
}