using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWV.Repository.Interfaces;
using MWV.DBContext;
using MWV.Models;
using System.Reflection;

namespace MWV.Repository.Implementation
{
    public class AlertsMessagesRepository : IAlertsMessages
    {
        private MWVDBContext db = new MWVDBContext();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public List<TempPendingApproval> ShowAlerts(string role, string id)
        {
            try
            {
                int? LoggedInAgent_id = 0;

                if (role == "Agent")
                {
                    LoggedInAgent_id = (from a in db.Agents where a.aspnetusers_id == id select a.agent_id).SingleOrDefault();
                    List<TempPendingApproval> lst = (from al in db.Alerts
                                                     where al.alert_for_role == role
                                                         && al.alert_for_agentid == LoggedInAgent_id
                                                     orderby al.created_on descending
                                                     select new TempPendingApproval
                                                     {
                                                         alert_id = al.alert_id,
                                                         alert_action = al.alert_action,
                                                         alert_for_role = al.alert_for_role,
                                                         alert_text = al.alert_text,
                                                         alert_subject = al.alert_subject,
                                                         created_on = al.created_on,
                                                         created_by = al.created_by,
                                                         viewed = al.viewed
                                                     }).ToList();
                    return lst;
                }
                else if (role == "MachineHead")
                {

                    List<TempPendingApproval> lst = (from al in db.Alerts where al.alert_for_role == "MachineHead" && al.machinehead_aspnetuserid == id orderby al.created_on descending select new TempPendingApproval { alert_id = al.alert_id, alert_action = al.alert_action, alert_for_role = al.alert_for_role, alert_text = al.alert_text, alert_subject = al.alert_subject, created_on = al.created_on, created_by = al.created_by, viewed = al.viewed }).ToList();
                    return lst;

                }



                else
                {
                    List<TempPendingApproval> lst = (from al in db.Alerts where al.alert_for_role == role orderby al.created_on descending select new TempPendingApproval { alert_id = al.alert_id, alert_action = al.alert_action, alert_for_role = al.alert_for_role, alert_text = al.alert_text, alert_subject = al.alert_subject, created_on = al.created_on, created_by = al.created_by, viewed = al.viewed }).ToList();
                    return lst;
                }
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AlertMessagesRepository->ShowAlerts:", Ex);
                return null;
            }


        }

        public bool deleteAlerts(int id)
        {
            try
            {
                var DeleteId = db.Alerts.FirstOrDefault(k => k.alert_id == id);
                if (DeleteId != null)
                {
                    db.Alerts.Remove(DeleteId);
                    db.SaveChanges();

                }

                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AlertMessagesRepository->deleteAlerts:", Ex);
                return false;
            }

        }

        public bool CreateAlerts(string alertAction, string alertForRole, int? alertForAgentId, string text, string alertSubject, string createdBy, string machineHeadID)
        {
            try
            {
                Alerts altObj = new Alerts();

                altObj.alert_action = alertAction;
                altObj.alert_for_role = alertForRole;
                altObj.alert_for_agentid = alertForAgentId;
                altObj.alert_text = text;
                altObj.alert_subject = alertSubject;
                altObj.created_on = DateTime.Now;
                altObj.created_by = createdBy;
                altObj.viewed = 1;
                altObj.machinehead_aspnetuserid = machineHeadID;
                db.Alerts.Add(altObj);
                db.SaveChanges();
                return true;

            }
            catch (Exception Ex)
            {
                logger.Error("Error in AlertMessagesRepository->CreateAlerts:", Ex);
                return false;
            }
        }

        //public bool CreateMessages(string messageAction, string messageforRole, int? messageforAgentid, string messageText,
        //      string messageSubject, string recipient1, string recipient2, string cc1,string cc2, string bcc1, string bcc2, string createdOn,
        //      string createdBy, string attachment1, string attachment2, string status)
        public bool CreateMessages(Messages MessagesObj)
        {
            try
            {
                Messages msgObj = new Messages();

                msgObj.message_action = MessagesObj.message_action;
                msgObj.message_for_role = MessagesObj.message_for_role;
                msgObj.message_for_agentid = MessagesObj.message_for_agentid;
                msgObj.message_text = MessagesObj.message_text;
                msgObj.message_subject = MessagesObj.message_subject;
                msgObj.recipient1 = MessagesObj.recipient1;
                msgObj.cc1 = MessagesObj.cc1;
                msgObj.bcc1 = MessagesObj.bcc1;
                msgObj.attachment1 = MessagesObj.attachment1;
                msgObj.status = MessagesObj.status;
                msgObj.created_on = DateTime.Now;
                msgObj.viewed = 1;
                msgObj.created_by = MessagesObj.created_by;
                msgObj.machinehead_aspnetuserid = MessagesObj.machinehead_aspnetuserid;
                msgObj.order_id=MessagesObj.order_id;
                db.Messages.Add(msgObj);
                db.SaveChanges();
                return true;

            }
            catch (Exception Ex)
            {
                logger.Error("Error in AlertMessagesRepository->CreateMessages:", Ex);
                return false;
            }
        }

        #region Dispose
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


        public List<TempPendingApproval> showMessages(string role, string id)
        {
            try
            {
                int LoggedInAgent_id;
                if (role == "Agent")
                {
                    LoggedInAgent_id = (from a in db.Agents where a.aspnetusers_id == id select a.agent_id).SingleOrDefault();
                    List<TempPendingApproval> lst = (from al in db.Messages where al.message_for_role == role && al.message_for_agentid == LoggedInAgent_id orderby al.created_on descending select new TempPendingApproval { alert_id = al.message_id, alert_action = al.message_action, alert_for_role = al.message_for_role, alert_text = al.message_text, alert_subject = al.message_subject, created_on = al.created_on, created_by = al.created_by, attachment1 = al.attachment1, attachment2 = al.attachment2, viewed = al.viewed, order_id_feedback = al.order_id }).ToList();
                    return lst;
                }
                else if (role == "MachineHead")
                {
                    List<TempPendingApproval> lst = (from al in db.Messages where al.message_for_role == "MachineHead" && al.machinehead_aspnetuserid == id orderby al.created_on descending select new TempPendingApproval { alert_id = al.message_id, alert_action = al.message_action, alert_for_role = al.message_for_role, alert_text = al.message_text, alert_subject = al.message_subject, created_on = al.created_on, created_by = al.created_by, attachment1 = al.attachment1, attachment2 = al.attachment2, viewed = al.viewed, order_id_feedback = al.order_id }).ToList();
                    return lst;
                }
                else
                {
                    List<TempPendingApproval> lst = (from al in db.Messages where al.message_for_role == role orderby al.created_on descending select new TempPendingApproval { alert_id = al.message_id, alert_action = al.message_action, alert_for_role = al.message_for_role, alert_text = al.message_text, alert_subject = al.message_subject, created_on = al.created_on, created_by = al.created_by, attachment1 = al.attachment1, attachment2 = al.attachment2, viewed = al.viewed, order_id_feedback = al.order_id }).ToList();
                    return lst;
                }
            }
            catch (Exception Ex)
            {

                logger.Error("Error in AlertMessagesRepository->showMessages:", Ex);
                return null;

            }


        }

        public bool deleteMessages(int id)
        {
            try
            {
                var DeleteId = db.Messages.FirstOrDefault(k => k.message_id == id);
                if (DeleteId != null)
                {
                    db.Messages.Remove(DeleteId);
                    db.SaveChanges();

                }

                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AlertMessagesRepository->deleteMessages:", Ex);
                return false;
            }

        }



        public bool UpdateReadStatusMsg(int msgid)
        {
            try
            {
                var query = db.Messages.FirstOrDefault(p => p.message_id == msgid);
                if (query != null)
                {
                    query.viewed = 0;
                    db.SaveChanges();


                }
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AlertMessagesRepository->UpdateReadStatusMsg:", Ex);
                return false;
            }
        }

        public bool UpdateReadStatusAlert(int alertId)
        {
            try
            {
                var query = db.Alerts.FirstOrDefault(p => p.alert_id == alertId);
                if (query != null)
                {
                    query.viewed = 0;
                    db.SaveChanges();


                }
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in AlertMessagesRepository->UpdateReadStatusAlert:", Ex);
                return false;
            }
        }

        public bool savefeedbacktodb(string feedback, int ratings, DateTime createdon, string createdby, int orderid)
        {
            try
            {
                var query = db.Orders.FirstOrDefault(k => k.order_id == orderid);
                if (query != null)
                {
                    query.rating = ratings;
                    query.rating_remarks = feedback;
                    query.rated_on = createdon;
                    query.rated_by = createdby;

                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception Ex)
            {

                logger.Error("Error in AlertMessagesRepository->UpdateReadStatusAlert:", Ex);
                return false;
            }
        }

    }
}