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
    interface IProductionPlanner
    {
        IQueryable PaperMillList();
        IQueryable GetAgentList();
        IQueryable GetCustomerList();
        IQueryable GetSrNo();
        IQueryable GetShades();
        List<Order> NewCreatedOrderList();
        List<TempPendingApproval> GetAllPendingApproval();
        List<TempPendingApproval> ProductionPlanHeaderDetails(int id);
        List<TempJumboDetails> ProductionPlanJmbosDetails(int id);
        List<TempJumboDetails> ProductionPlanLotsDetails(int id);
        List<TempPendingApproval> ConflictCustomer();
        List<TempPendingApproval> GetSearchProductionPlanByEntity(int millId, DateTime fromDate, DateTime toDate, int AgentId, int CustId, int prId, int SrnoListBymillid);
        List<TempPendingApproval> GetProductionPlan();
        List<deckle_approvals> GetDeckleDetails();
        List<TempJumboDetails> DeckleQuickiewDetails();
        List<deckle_approvals> SeeMismatchDetails(int dmId);
        void GetDeckleDetails(int id, string action, string Remark);
        List<Agent> lstemails();

    }
}