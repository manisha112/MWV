using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWV.Models;

namespace MWV.Repository.Interfaces
{
   interface IAlertsMessages
    {
        List<TempPendingApproval> ShowAlerts(string role,string id);
        bool deleteAlerts(int id);
        bool CreateAlerts(string alertAction, string alertForRole, int? alertForAgentId, string text, string alertSubject, string createdBy,string machineHeadID);
        List<TempPendingApproval> showMessages(string role, string id);
        bool deleteMessages(int id);
        bool CreateMessages(Messages MessagesObj);
        bool UpdateReadStatusMsg(int msgid);
        bool UpdateReadStatusAlert(int alertId);
    }
}