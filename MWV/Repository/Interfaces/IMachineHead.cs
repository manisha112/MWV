using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MWV.Models;
using System.Web;
using MWV.Repository.Implementation;
using System.Collections;
using MWV.ViewModels;

namespace MWV.Repository.Interfaces
{
    interface IMachineHead
    {
        void SubmitMachineOffDetails(DateTime offStartDateTime, DateTime offStopDateTime, string Remark);
        void SubmitMachineOnDetails(DateTime onStartDateTime, string Remark);  
        List<TempPendingApproval> ProductionPlanHeaderDetails(DateTime dt);
        List<TempJumboDetails> ProductionPlanJmbosDetails(DateTime dt);
        List<TempJumboDetails> ProductionPlanLotsDetails(DateTime dt);      
    }
}