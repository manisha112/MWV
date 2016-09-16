using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MWV.Models;

namespace MWV.Repository.Interfaces
{
    public interface IDeckle : IDisposable
    {
        //string Papermill;
        
        List<Order_Products> GetOrders(int PapermillId, string Status, DateTime RequestDate); //change status to ENUM class
        Papermill GetPaperMill(int papermillid);
        ProductionTimeline GetProductionTimeline(int Papermill, String BF, String GSM, String Shade);
        ProductionRun GetLastRunEstimateEndTime(int Papermill, int ScheduleID);
        int CreateProductionRun(ProductionRun productionrun, List<ProductionJumbo> ProductionJumboList);
        int CreateProductionRunChilds(List<ProductionChild> ProductionChildList);
        int GetLastJumboNo();
        bool UpdateOrderProductQty(int OrderProductNo, decimal MasterJumboQty);
        string GetLowerDeckleCombination(string BF, string GSM, string Shade);
        int CreateDeckleApprovals(List<deckle_approvals> approvals);
        List<deckle_approvals> GetDeckleApprovals(int MillNo, string BF, string GSM, string Shade, string Action);
        int UpdateDeckleApprovals(int deckleApprovalId);
        string GetLowerBF(string BFcode);
       // bool MarkOrdersAsPlanned();
        List<dynamic> GetDeckleApprovalsCreatedbetween(DateTime startdate, DateTime enddate);
        List<Schedule> GetActiveSchedules();
        List<Schedule> GetActiveSchedulesforPapermill(int papermillid);
        int UpdateScheduleEndDate(int scheduleid, DateTime NewEnddate);
        int UpdateScheduleStartDate(int scheduleid, DateTime NewStartdate);
        int DelayProductionPlansByMinsforSchedule(int Scheduleid, int DelayinMinutes);
        Schedule GetScheduleByID(int Scheduleid);

    }
}
