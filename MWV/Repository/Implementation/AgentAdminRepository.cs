using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWV.Repository.Interfaces;
using MWV.Models;
using MWV.DBContext;
using System.Reflection;
namespace MWV.Repository
{
    public class AgentAdminRepository
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();
        public bool SaveAgent(Agent Agnt)
        {
            try
            {
                db.Agents.Add(Agnt);
                db.SaveChanges();

                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentAdminRepository->SaveAgent", Ex);
                return false;
            }
        }

        public List<Agent> lstAgent(int id)
        {
            var lstAgent = db.Agents.Where(k => k.agent_id == id);
            return lstAgent.ToList();

        }

        public bool EditAgent(Agent ObjAgnt)
        {
            try
            {
                var query = db.Agents.Where(k => k.agent_id == ObjAgnt.agent_id).SingleOrDefault();
                if (query != null)
                {
                    query.name = ObjAgnt.name;
                    query.email = ObjAgnt.email;
                    query.mobile = ObjAgnt.mobile;
                    query.landline = ObjAgnt.landline;
                    query.address = ObjAgnt.address;
                    query.credit_limit = ObjAgnt.credit_limit;
                    query.credit_days = ObjAgnt.credit_days;
                    query.address2 = ObjAgnt.address2;
                    query.address3 = ObjAgnt.address3;
                    query.city = ObjAgnt.city;
                    query.state = ObjAgnt.state;
                    query.pincode = ObjAgnt.pincode;
                    query.firstname = ObjAgnt.firstname;
                    query.lastname = ObjAgnt.lastname;

                    db.SaveChanges();


                }
                //string aspnetID = db.Agents.Where(k => k.agent_id == ObjAgnt.agent_id).Select(k => k.aspnetusers_id).SingleOrDefault();
                //var updateQuery = db.AspNetUsers.Where(l => l.Id == aspnetID).SingleOrDefault();
                //if (updateQuery != null)
                //{
                //    updateQuery.Email = ObjAgnt.email;
                //    updateQuery.firstname = ObjAgnt.firstname;
                //    updateQuery.lastname = ObjAgnt.lastname;
                //    db.SaveChanges();
                //    return true;
                //}



                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentAdminRepository->EditAgent", Ex);
                return false;
            }
        }

        public string getAspnetId(string email)
        {
            try
            {
                string queryGetAspnetId = db.AspNetUsers.Where(k => k.Email == email).Select(k => k.Id).SingleOrDefault();
                return queryGetAspnetId;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentAdminRepository->getAspnetId", Ex);
                return "";
            }
        }

        public string getAspnetIdbyagentID(int id)
        {
            try
            {
                string queryGetAspnetId = db.Agents.Where(k => k.agent_id == id).Select(k => k.aspnetusers_id).SingleOrDefault();
                return queryGetAspnetId;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentAdminRepository->getAspnetId", Ex);
                return "";
            }
        }

        //Check Email Id Exists in Aspnet
        public bool chkAspnetEmail(string id, string email)
        {
            try
            {
                var query = db.AspNetUsers.Where(p => p.Id == id).Select(l => l.Email == email).Count();
                if (query > 0)
                {
                    return false;

                }
                else
                {
                    return true;

                }

                
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AgentAdminRepository->getAspnetId", Ex);
                return false;
            }
        }

    }
}