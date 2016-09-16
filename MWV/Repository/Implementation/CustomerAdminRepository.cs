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
    public class CustomerAdminRepository
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        public CustomerAdminRepository()
        {

        }

        public List<Customer> GetCustomers()
        {
            try
            {
                var lstCustomers = db.Customers;
                return lstCustomers.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerAdminRepository->GetCustomers:", Ex);
                return null;
            }
        }


        public bool EditCustomer(int CustomerIDToEdit, string name, string email, string phone, string address1, string address2, string address3, string city, string pincode, string state, string country, string fax, string remarks, string modified_by, string firstname, string lastname, Int32 agntID)
        {
            try
            {
                bool isEdited;
                var objCust = db.Customers.Where(c => c.customer_id == CustomerIDToEdit).FirstOrDefault();
                objCust.name = name;
                if (email == "")
                    objCust.email = null;
                else
                    objCust.email = email;

                objCust.phone = phone;
                objCust.address1 = address1;
                objCust.address2 = address2;
                objCust.address3 = address3;
                objCust.city = city;
                objCust.pincode = pincode;
                objCust.state = state;
                objCust.country = country;
                objCust.fax = fax;
                objCust.remarks = remarks;
                objCust.modified_by = modified_by;
                objCust.modified_on = DateTime.Now;
                objCust.firstname = firstname;
                objCust.lastname = lastname;
                objCust.agent_id = agntID;
                db.SaveChanges();
                isEdited = true;
                return isEdited;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerAdminRepository->EditCustomer:", Ex);
                return false;//fail to edit
            }
        }


        public bool DeleteCustomer(int CustomerIDToDel)
        {
            try
            {
                bool isDeleted;
                var objCust = db.Customers.Where(c => c.customer_id == CustomerIDToDel).FirstOrDefault();
                db.Customers.Remove(objCust);
                db.SaveChanges();
                isDeleted = true;
                return isDeleted;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerAdminRepository->DeleteCustomer:", Ex);
                return false;//fail to del
            }
        }

        //Create New Customer
        public bool CreateCustomer(Customer ObjCust)
        {
            try
            {
                string createdby = db.AspNetUsers.Where(k => k.Id == ObjCust.aspnetuser_id).Select(o => o.name).SingleOrDefault();
                ObjCust.created_on = DateTime.Now;
                ObjCust.status = "Created";
                ObjCust.created_by = createdby;
                ObjCust.login_enabled = 0;
                db.Customers.Add(ObjCust);
                db.SaveChanges();
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerAdminRepository->CreateCustomer:", Ex);
                return false;//fail to del
            }
        }


        // Get All Agent 
        public List<Agent> GetAgents()
        {
            try
            {
                var query = db.Agents;
                return query.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in CustomerAdminRepository->CreateCustomer:", Ex);
                return null;
            }
        }
        public string getAspnetIdbyCustID(int id)
        {
            try
            {
                string queryGetAspnetId = db.Customers.Where(k => k.customer_id == id).Select(k => k.aspnetuser_id).SingleOrDefault();
                return queryGetAspnetId;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentAdminRepository->getAspnetId", Ex);
                return "";
            }
        }
    }

}
