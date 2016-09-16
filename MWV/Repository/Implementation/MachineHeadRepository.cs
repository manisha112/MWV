using MWV.DBContext;
using MWV.Models;
using MWV.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Microsoft.AspNet.Identity;
using MWV.ViewModels;
using System.Collections;

namespace MWV.Repository.Implementation
{
    public class MachineHeadRepository : IMachineHead
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();
        string MachineHeadId = HttpContext.Current.User.Identity.GetUserId();
        private bool disposed = false;

        public void SubmitMachineOffDetails(DateTime offStartDateTime, DateTime offStopDateTime, string Remark)
        {
            try
            {
                var Asssignedmc = from r in db.AspNetUsers
                                  where r.Id == MachineHeadId
                                  select r;
                var papermillId = (from r in Asssignedmc
                                   join re in db.Papermills on r.AssignedMachine equals re.name
                                   select re.papermill_id).SingleOrDefault();

                var machineStatus = (from r in db.Papermills
                                     where r.papermill_id == papermillId
                                     select r).SingleOrDefault();

                papermill_logs objLogs = new papermill_logs();
                objLogs.estimated_start = offStartDateTime;
                objLogs.stop_datetime = offStopDateTime;
                objLogs.remarks = Remark;
                objLogs.papermill_id = papermillId;
                objLogs.aspnetuser_id = MachineHeadId;
                db.papermill_logs.Add(objLogs);
                machineStatus.status = "OFF";
                db.SaveChanges();
                ProductionRun objProRun = new ProductionRun();
                objProRun.estimated_start = objLogs.stop_datetime;
                objProRun.estimated_end = objLogs.estimated_start;
                objProRun.papermill_id = papermillId;
                objProRun.actual_qty = 0;
                objProRun.estimated_qty = 0;
                db.ProductionRun.Add(objProRun);

                db.SaveChanges();

            }
            catch (Exception Ex)
            {
                logger.Error("Error in MachineHeadRepository->MachineOffDetails:", Ex);
            }
        }
        public void SubmitMachineOnDetails(DateTime onStartDateTime, string Remark)
        {
            try
            {

                var papermillId = PappermillIdByLocation();
                var machineStatus = (from r in db.Papermills
                                     where r.papermill_id == papermillId
                                     select r).SingleOrDefault();
                papermill_logs objLogs = new papermill_logs();
                objLogs.actual_start = onStartDateTime;
                objLogs.remarks = Remark;
                objLogs.papermill_id = papermillId;
                objLogs.aspnetuser_id = MachineHeadId;
                machineStatus.status = "ON";
                db.papermill_logs.Add(objLogs);
                db.SaveChanges();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in MachineHeadRepository->SubmitMachineDeOnDetails:", Ex);
            }
        }
        public int PappermillIdByLocation()
        {
            try
            {
                var Asssignedmc = from r in db.AspNetUsers
                                  where r.Id == MachineHeadId
                                  select r;
                var papermillId = (from r in Asssignedmc
                                   join re in db.Papermills on r.AssignedMachine equals re.name
                                   select re.papermill_id).SingleOrDefault();
                return papermillId;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in MachineHeadRepository->MachineOffDetails:", Ex);
                return 0;
            }

        }
        public List<TempPendingApproval> ProductionPlanHeaderDetails(DateTime dt)
        {
            try
            {
                int papermillId = PappermillIdByLocation();
                DateTime date = dt.AddHours(24);
                List<TempPendingApproval> query = (from r in db.ProductionRun
                                                   join rd in db.ProductionJumbo on r.pr_id equals rd.pr_id
                                                   where ((r.estimated_start >= dt && r.estimated_start < date)
                                                   && (r.papermill_id == papermillId))
                                                   group r by r.pr_id into g
                                                   select new TempPendingApproval
                                                   {
                                                       pappermill_id = g.FirstOrDefault().papermill_id,
                                                       srNo = g.FirstOrDefault().pr_id,
                                                       estimated_start = g.FirstOrDefault().estimated_start,
                                                       EstimatedEndDate = g.FirstOrDefault().estimated_end,
                                                       EstimatedRunTime = g.FirstOrDefault().run_time,
                                                       plannedQty = g.FirstOrDefault().estimated_qty,
                                                       jumbo_no = g.Count(),
                                                       actualStart = g.FirstOrDefault().actual_start,
                                                       actual_end = g.FirstOrDefault().actual_end ,
                                                       SchStart = dt
                                                       
                                                   }).ToList();
                return query;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->ProductionPlanHeaderDetails:", Ex);
                return null;
            }
        }
        public List<TempJumboDetails> ProductionPlanJmbosDetails(DateTime dt)
        {
            try
            {
                DateTime date = dt.AddHours(24);
                var query = (from r in db.ProductionJumbo
                             join rd in db.ProductionRun on r.pr_id equals rd.pr_id
                             where rd.estimated_start >= dt && rd.estimated_start <= date
                             select new TempJumboDetails
                             {
                                 BF = r.bf_code,
                                 GSM = r.gsm_code,
                                 shade = r.shade_code,
                                 jumboNo = r.jumbo_no,
                                 plannedQty = r.planned_qty,
                                 estimated_start = r.estimated_start,
                                 srNo = r.pr_id,
                                 actual_start = r.actual_start,
                                 actual_end = r.actual_end
                             });

                return query.ToList();

            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->ProductionPlanJmbosDetails:", Ex);
                return null;
            }
        }
        public List<TempJumboDetails> ProductionPlanLotsDetails(DateTime dt)
        {
            try
            {
                DateTime date = dt.AddHours(24);
                var query = (from r in db.ProductionChild
                             join rr in db.ProductionJumbo on r.pj_id equals rr.pj_id
                             join rd in db.ProductionRun on rr.pr_id equals rd.pr_id
                             join re in db.Order_products on r.order_product_id equals re.order_product_id into tempJoin
                             from rj in tempJoin.DefaultIfEmpty()
                             join rf in db.Orders on rj.order_id equals rf.order_id into temp2Join
                             from rjj in temp2Join.DefaultIfEmpty()
                             join rg in db.Customers on rjj.customer_id equals rg.customer_id into temp3join
                             from rej in temp3join.DefaultIfEmpty()
                             where rd.estimated_start >= dt && rd.estimated_start <= date

                             select new TempJumboDetails
                             {
                                 Sequence = r.sequence,
                                 Width = r.width,
                                 jumboNo = rr.jumbo_no,
                                 DeckleQty = r.qty,
                                 CustomerName = rej.name,
                                 pjid = rr.pj_id,
                                 srNo = rd.pr_id,
                                 pcid = r.pc_id,
                                 order_product_id = r.order_product_id,
                                 actual_start = r.actual_start,
                                 actual_end = r.actual_end
                             });

                return query.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->ProductionPlanLotsDetails:", Ex);
                return null;
            }
        }
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
    }
}
