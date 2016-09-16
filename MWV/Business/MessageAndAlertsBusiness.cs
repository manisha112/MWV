using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MWV.Repository.Implementation;
using MWV.Repository.Interfaces;
using MWV.Models;

namespace MWV.Business
{
    public class MessageAndAlertsBusiness
    {

        public AlertsMessagesRepository alertMsgRepObj = new AlertsMessagesRepository();
        public bool CreateAlertDetails(string alertAction, string alertForRole, int? alertForAgentId, string text, string alertSubject, string createdBy,string machineHeadID )
        {
            alertMsgRepObj.CreateAlerts(alertAction, alertForRole, alertForAgentId, text, alertSubject, createdBy,machineHeadID);
            return true;
        }

        //public bool CreateMessagesDetails(Messages MessagesObj)

        public bool CreateMessagesDetails(string messageAction, string messageforRole, int? messageforAgentid, string messageText,
            string messageSubject, string recipient1, string cc1, string bcc1,
            string createdBy, string attachment1, string status,string machinehead_aspnetuserid,int? order_id)
        {
            Messages msgObj = new Messages();

            msgObj.message_action = messageAction;
            msgObj.message_for_role = messageforRole;
            msgObj.message_for_agentid = messageforAgentid;
            msgObj.message_text = messageText;
            msgObj.message_subject = messageSubject;
            msgObj.recipient1 = recipient1;
            msgObj.cc1 = cc1;
            msgObj.bcc1 = bcc1;
            msgObj.attachment1 = attachment1;
            msgObj.status = status;
            msgObj.created_on = DateTime.Now;
            msgObj.created_by = createdBy;
            msgObj.machinehead_aspnetuserid=machinehead_aspnetuserid;
            msgObj.order_id=order_id;
            alertMsgRepObj.CreateMessages(msgObj);
            return true;
        }

        
    }
}