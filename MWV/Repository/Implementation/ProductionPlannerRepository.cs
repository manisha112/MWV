using System;
using System.Collections.Generic;
using System.Linq;
using MWV.Models;
using MWV.DBContext;
using MWV.Repository.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Microsoft.AspNet.Identity;
using System.Web.WebPages.Html;
using System.Web.Mvc;
using System.Reflection;
using System.Dynamic;
using System.Data;
using IdentitySample.Models;
using System.Collections;
using MWV.ViewModels;
using System.Data.Entity.Core.Objects;

namespace MWV.Repository.Implementation
{
    public class ProductionPlannerRepository : IProductionPlanner
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();

        public IQueryable PaperMillList()
        {
            try
            {
                Papermill PaperMillList = new Papermill();

                var lstPapermills = db.Papermills
                                      .OrderByDescending(l => l.papermill_id)
                                       .Select(x => new { x.name, x.papermill_id });
                return lstPapermills;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->PaperMillList:", Ex);
                return null;
            }
        }
        public IQueryable GetAgentList()
        {
            try
            {
                var query = db.Agents
                                   .OrderByDescending(l => l.name)
                                   .Select(x => new { x.name, x.agent_id });
                return query;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetAgentList:", Ex);
                return null;
            }
        }
        public IQueryable GetCustomerList()
        {
            try
            {
                var query = db.Customers
                                   .OrderByDescending(l => l.name)
                                   .Where(l => l.status == "Approved")
                                   .Select(x => new { x.name, x.customer_id });
                return query;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetCustomerList:", Ex);
                return null;
            }
        }
        public IQueryable GetSrNo()
        {
            try
            {
                var query = db.ProductionRun
                                   .OrderByDescending(l => l.pr_id)
                                   .Where(r => r.run_time != null || r.run_time != 0)
                                   .Select(x => new { x.pr_id });
                return query;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetSrNo:", Ex);
                return null;
            }
        }
        public IQueryable GetProductCodeList()
        {
            try
            {
                var query = db.Products
                                   .OrderByDescending(l => l.product_code)
                                   .Select(x => new { x.product_code });
                return query;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetProductCodeList:", Ex);
                return null;
            }
        }
        public List<Order> NewCreatedOrderList()
        {
            try
            {
                var OrdersList = (from r in db.Orders
                                  where r.status == "Created"
                                  orderby r.order_id descending
                                  select r);
                return OrdersList.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->NewCreatedOrderList:", Ex);
                return null;
            }
        }
        public List<TempPendingApproval> GetAllPendingApproval()
        {
            try
            {
                //this query uses left outer join, it joins newly created customers with approved customers on name + City to match
                var ConflictCustAndPendingCustList = (from dc in db.Customers
                                                      join da in db.Agents on dc.agent_id equals da.agent_id
                                                      join cust in db.Customers on new { f1 = dc.name.ToUpper(), f2 = dc.city.ToUpper() } equals new { f1 = cust.name.ToUpper(), f2 = cust.city.ToUpper() } into matching
                                                      from subcust in matching.Where(p => p.status == "Approved").DefaultIfEmpty()
                                                      // join ord in db.Orders on subcust.customer_id equals ord.customer_id into tempJoin
                                                      //from rj in tempJoin.DefaultIfEmpty()
                                                      where dc.status == "Created"
                                                      select new TempPendingApproval
                                                      {
                                                          CustomerName = dc.name,
                                                          AgentName = da.name,
                                                          agentId = da.agent_id,
                                                          customer_id = dc.customer_id,
                                                          //ordStatus = rj.status,
                                                          City = dc.city,
                                                          Country = dc.country,
                                                          State = dc.state,
                                                          Addess1 = dc.address1,
                                                          Addess2 = dc.address2,
                                                          Addess3 = dc.address3,
                                                          Phone = dc.phone,
                                                          fax = dc.fax,
                                                          PinNo = dc.pincode,
                                                          CustCreatedOn = dc.created_on,
                                                          DuplicateCustomerName = (subcust == null ? string.Empty : subcust.name),
                                                          DuplicateAgentName = (subcust == null ? string.Empty : subcust.Agent.name),
                                                          DuplicateCustId = (subcust == null ? 0 : subcust.customer_id),
                                                          ordCount = 0
                                                      });


                List<TempPendingApproval> query = new List<TempPendingApproval>();
                foreach (var items in ConflictCustAndPendingCustList.ToList())
                {
                    int ordCount = 0;
                    TempPendingApproval tdaObj = new TempPendingApproval();
                    if (items.DuplicateCustId != null && items.DuplicateCustId != 0)
                    {
                        int dupCustId = items.DuplicateCustId;
                        var ordCountList = (from ord in db.Orders
                                            where ord.customer_id == dupCustId && ord.status != "Completed"
                                            select ord);
                        ordCount = ordCountList.Count();
                    }
                    tdaObj.CustomerName = items.CustomerName;
                    tdaObj.AgentName = items.AgentName;
                    tdaObj.agentId = items.agentId;
                    tdaObj.customer_id = items.customer_id;
                    // tdaObj.//ordStatus = rj.status,
                    tdaObj.City = items.City;
                    tdaObj.Country = items.Country;
                    tdaObj.State = items.State;
                    tdaObj.Addess1 = items.Addess1;
                    tdaObj.Addess2 = items.Addess2;
                    tdaObj.Addess3 = items.Addess3;
                    tdaObj.Phone = items.Phone;
                    tdaObj.fax = items.fax;
                    tdaObj.PinNo = items.PinNo;
                    tdaObj.CustCreatedOn = items.CustCreatedOn;
                    tdaObj.DuplicateCustomerName = items.DuplicateCustomerName;
                    tdaObj.DuplicateAgentName = items.DuplicateAgentName;
                    tdaObj.DuplicateCustId = items.DuplicateCustId;
                    tdaObj.ordCount = ordCount;
                    query.Add(tdaObj);
                }

                return query;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetAllPendingApproval:", Ex);
                return null;
            }
        }
        public List<TempPendingApproval> ProductionPlanHeaderDetails(int id)
        {
            try
            {
                var query = (from r in db.ProductionChild
                             join rr in db.ProductionJumbo on r.pj_id equals rr.pj_id
                             join rd in db.ProductionRun on rr.pr_id equals rd.pr_id
                             join dc in db.Papermills on rd.papermill_id equals dc.papermill_id
                             where rr.pr_id == id && (rd.run_time != null || rd.run_time != 0)
                             select new TempPendingApproval
                             {
                                 srNo = rd.pr_id,
                                 estimated_start = rd.estimated_start,
                                 EstimatedEndDate = rd.estimated_end,
                                 EstimatedRunTime = rd.run_time,
                                 plannedQty = rd.estimated_qty,
                                 ActualQty = rd.actual_qty,
                                 papermillName = dc.name
                             });
                return query.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->ProductionPlanHeaderDetails:", Ex);
                return null;
            }
        }
        public List<TempJumboDetails> ProductionPlanJmbosDetails(int id)
        {
            try
            {
                var query = (from r in db.ProductionJumbo
                             where r.pr_id == id
                             select new TempJumboDetails
                             {
                                 BF = r.bf_code,
                                 GSM = r.gsm_code,
                                 shade = r.shade_code,
                                 jumboNo = r.jumbo_no,
                                 plannedQty = r.planned_qty,

                             });

                return query.ToList();

            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->ProductionPlanJmbosDetails:", Ex);
                return null;
            }
        }
        public List<TempJumboDetails> ProductionPlanLotsDetails(int id)
        {
            try
            {
                //var query = (from r in db.ProductionChild
                //             join rr in db.ProductionJumbo on r.pj_id equals rr.pj_id
                //             join rd in db.ProductionRun on rr.pr_id equals rd.pr_id
                //             join re in db.Order_products on r.order_product_id equals re.order_product_id
                //             join rf in db.Orders on re.order_id equals rf.order_id
                //             join rg in db.Customers on rf.customer_id equals rg.customer_id

                var query = (from r in db.ProductionChild
                             join rr in db.ProductionJumbo on r.pj_id equals rr.pj_id
                             join rd in db.ProductionRun on rr.pr_id equals rd.pr_id
                             join re in db.Order_products on r.order_product_id equals re.order_product_id into tempJoin
                             from rj in tempJoin.DefaultIfEmpty()
                             join rf in db.Orders on rj.order_id equals rf.order_id into temp2Join
                             from rjj in temp2Join.DefaultIfEmpty()
                             join rg in db.Customers on rjj.customer_id equals rg.customer_id into temp3join
                             from rej in temp3join.DefaultIfEmpty()
                             where rr.pr_id == id
                             select new TempJumboDetails
                             {
                                 marking = r.marking,
                                 Sequence = r.sequence,
                                 Width = r.width,
                                 jumboNo = rr.jumbo_no,
                                 DeckleQty = r.qty,
                                 CustomerName = rej.name,
                                 pjid = rr.pj_id,
                                 srNo = rd.pr_id,
                                 pcid = r.pc_id,
                                 order_product_id = r.order_product_id,
                             });

                return query.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->ProductionPlanLotsDetails:", Ex);
                return null;
            }
        }
        public List<TempPendingApproval> ConflictCustomer()
        {
            try
            {

                var results = (from a in db.Customers
                               join bp in db.Orders on a.customer_id equals bp.customer_id
                               join da in db.Agents on bp.agent_id equals da.agent_id
                               select new TempPendingApproval
                                   {
                                       CustomerName = a.name,
                                       AgentName = da.name,
                                       customer_id = a.customer_id,
                                       ordStatus = bp.status,
                                       CustStatus = a.status,
                                       City = a.city,
                                       Country = a.country,
                                       State = a.state,
                                       Addess1 = a.address1,
                                       Addess2 = a.address2,
                                       Addess3 = a.address3,
                                       Phone = a.phone,
                                       fax = a.fax,
                                       PinNo = a.pincode
                                   })
       .AsEnumerable()
       .GroupBy(t => new { t.City, t.CustomerName })
       .Where(g => g.Count() > 1).ToList();
                var list = results.SelectMany(d => d)
                 .Where(x => x.CustStatus == "Created")
                 .ToList();
                return list;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->ConflictCustomer:", Ex);
                return null;
            }
        }
        public List<TempPendingApproval> GetSearchProductionPlanByEntity(int millId, DateTime fromDate, DateTime toDate, int AgentId, int CustId, int prId, int SrnoListBymillid)
        {
            try
            {
                if (AgentId != 0 && millId != 0)
                {
                    var ProductionPlanByAgent = (from r in db.ProductionRun
                                                 //join op in db.Order_products on r
                                                 join sch in db.Schedule on r.schedule_id equals sch.schedule_id
                                                 join rf in db.Orders on sch.papermill_id equals rf.papermill_id
                                                 join dc in db.Papermills on sch.papermill_id equals dc.papermill_id
                                                 join de in db.Agents on rf.agent_id equals de.agent_id
                                                 where dc.papermill_id == millId
                                                 && (r.estimated_start >= fromDate
                                                 && r.estimated_start <= toDate)
                                                 && de.agent_id == AgentId
                                                 && (r.run_time > 0)
                                                 select new TempPendingApproval
                                                 {
                                                     estimated_start = r.estimated_start,
                                                     papermillName = dc.name,
                                                     srNo = r.pr_id,
                                                 });
                    var objs = (from c in ProductionPlanByAgent
                                orderby c.srNo
                                select c).GroupBy(g => g.srNo).Select(x => x.FirstOrDefault());
                    List<TempPendingApproval> query = new List<TempPendingApproval>();
                    foreach (var items in objs)
                    {
                        TempPendingApproval obj = new TempPendingApproval();
                        obj.srNo = items.srNo;
                        obj.papermillName = items.papermillName;
                        obj.estimated_start = items.estimated_start;
                        query.Add(obj);
                    }
                    return query;

                }
                else if (CustId != 0 && millId != 0)
                {
                    var ProductionPlanByCust = (from r in db.ProductionRun
                                                join op in db.Order_products on r.schedule_id equals op.schedule_id
                                                join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                                join rf in db.Orders on op.order_id equals rf.order_id
                                                join dc in db.Papermills on sch.papermill_id equals dc.papermill_id
                                                where sch.papermill_id == millId
                                                    && (r.estimated_start >= fromDate
                                                    && r.estimated_start <= toDate)
                                                    && rf.customer_id == CustId
                                                    && (r.run_time > 0)
                                                select new TempPendingApproval
                                                {
                                                    estimated_start = r.estimated_start,
                                                    papermillName = dc.name,
                                                    customer_id = rf.customer_id,
                                                    srNo = r.pr_id
                                                });
                    var objs = (from c in ProductionPlanByCust
                                orderby c.srNo
                                select c).GroupBy(g => g.srNo).Select(x => x.FirstOrDefault());

                    List<TempPendingApproval> query = new List<TempPendingApproval>();
                    foreach (var items in objs)
                    {
                        TempPendingApproval obj = new TempPendingApproval();
                        obj.srNo = items.srNo;
                        obj.papermillName = items.papermillName;
                        obj.estimated_start = items.estimated_start;
                        query.Add(obj);
                    }

                    return query;
                }
                else if (SrnoListBymillid != 0 && millId != 0)
                {
                    var ProductionPlanBySrid = (from d in db.ProductionRun
                                                join dc in db.Papermills on d.papermill_id equals dc.papermill_id
                                                where dc.papermill_id == millId
                                                 && d.pr_id == SrnoListBymillid
                                                 && (d.run_time > 0)
                                                select new TempPendingApproval
                                                {
                                                    estimated_start = d.estimated_start,
                                                    papermillName = dc.name,
                                                    srNo = d.pr_id
                                                }).OrderByDescending(x => x.srNo);
                    return ProductionPlanBySrid.ToList();
                }
                else if (AgentId != 0 && millId == 0)
                {
                    var ProductionPlanByAgent = (from r in db.ProductionRun
                                                 join d in db.ProductionJumbo on r.pr_id equals d.pr_id
                                                 join pc in db.ProductionChild on d.pj_id equals pc.pj_id
                                                 join op in db.Order_products on pc.order_product_id equals op.order_product_id
                                                 join o in db.Orders on op.order_id equals o.order_id

                                                 join sch in db.Schedule on r.schedule_id equals sch.schedule_id

                                                 join pm in db.Papermills on sch.papermill_id equals pm.papermill_id
                                                 where r.estimated_start >= fromDate
                                                 && r.estimated_start <= toDate
                                                 && o.agent_id == AgentId
                                                 && (r.run_time > 0)
                                                 select new
                                                 {
                                                     estimated_start = r.estimated_start,
                                                     papermillName = pm.name,
                                                     srNo = r.pr_id,
                                                 }).ToList();

                    var objs = (from c in ProductionPlanByAgent
                                orderby c.srNo
                                select c).GroupBy(g => g.srNo).Select(x => x.FirstOrDefault());
                    List<TempPendingApproval> query = new List<TempPendingApproval>();
                    foreach (var items in objs)
                    {
                        TempPendingApproval obj = new TempPendingApproval();
                        obj.srNo = items.srNo;
                        obj.papermillName = items.papermillName;
                        obj.estimated_start = items.estimated_start;
                        query.Add(obj);
                    }
                    return query;

                }
                else if (CustId != 0 && millId == 0)
                {
                    var ProductionPlanByCust = (from r in db.ProductionRun
                                                join op in db.Order_products on r.schedule_id equals op.schedule_id
                                                join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                                                join rf in db.Orders on op.order_id equals rf.order_id
                                                join dc in db.Papermills on sch.papermill_id equals dc.papermill_id
                                                where r.estimated_start >= fromDate
                                                && r.estimated_start <= toDate
                                                && rf.customer_id == CustId
                                                && (r.run_time > 0)
                                                select new TempPendingApproval
                                                {
                                                    estimated_start = r.estimated_start,
                                                    papermillName = dc.name,
                                                    customer_id = rf.customer_id,
                                                    srNo = r.pr_id
                                                });
                    var objs = (from c in ProductionPlanByCust
                                orderby c.srNo
                                select c).GroupBy(g => g.srNo).Select(x => x.FirstOrDefault());

                    List<TempPendingApproval> query = new List<TempPendingApproval>();
                    foreach (var items in objs)
                    {
                        TempPendingApproval obj = new TempPendingApproval();
                        obj.srNo = items.srNo;
                        obj.papermillName = items.papermillName;
                        obj.estimated_start = items.estimated_start;
                        query.Add(obj);
                    }
                    return ProductionPlanByCust.ToList();
                }
                else if (prId != 0 && millId == 0)
                {
                    var ProductionPlanBySrid = (from r in db.ProductionRun
                                                join dc in db.Papermills on r.papermill_id equals dc.papermill_id
                                                where r.pr_id == prId
                                                && (r.run_time > 0)
                                                select new TempPendingApproval
                                                {
                                                    estimated_start = r.estimated_start,
                                                    papermillName = dc.name,
                                                    srNo = r.pr_id
                                                }).OrderByDescending(x => x.srNo);
                    return ProductionPlanBySrid.ToList();
                }

                return null;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetSearchProductionPlanByEntity:", Ex);
                return null;
            }
        }
        public List<TempPendingApproval> GetProductionPlan()
        {
            try
            {

                var dt = DateTime.Now.AddDays(-7);
                var ProPlan = (from d in db.ProductionRun
                               join dc in db.Papermills on d.papermill_id equals dc.papermill_id
                               where d.estimated_start > dt && (d.run_time > 0)
                               orderby d.estimated_start descending
                               select new TempPendingApproval
     {
         estimated_start = d.estimated_start,
         papermillName = dc.name,
         srNo = d.pr_id

     }).Take(2);
                return ProPlan.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetProductionPlan:", Ex);
                return null;
            }
        }
        public List<deckle_approvals> GetDeckleDetails()
        {
            try
            {
                var DeckleDetails = from r in db.deckle_approvals
                                    where r.action == null
                                    select r;
                return DeckleDetails.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetDeckleDetails:", Ex);
                return null;
            }
        }
        public List<TempJumboDetails> DeckleQuickiewDetails()
        {
            try
            {
                var dt = DateTime.Now.AddDays(-7);
                var DeckleDetails = from r in db.deckle_approvals
                                    where r.action == null
                                    select r;
                var deckle = (from r in DeckleDetails
                              join re in db.Papermills on r.papermill_id equals re.papermill_id
                              where r.request_date > dt
                              select new TempJumboDetails
                              {
                                  papermillname = re.name,
                                  RequestedDate = r.request_date,
                                  BF = r.bf_code,
                                  GSM = r.gsm_code,
                                  dm_id = r.dm_id,
                                  shade = r.shade_code,
                              }).Take(2);
                return deckle.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->DeckleQuickiewDetails:", Ex);
                return null;
            }
        }
        public List<deckle_approvals> SeeMismatchDetails(int dmId)
        {
            try
            {
                var DeckleDetails = GetDeckleDetails().Where(x => x.dm_id == dmId);
                return DeckleDetails.ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->SeeMismatchDetails:", Ex);
                return null;
            }
        }
        public void GetDeckleDetails(int id, string action, string Remark)
        {
            string currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId();
            try
            {
                var deckle = db.deckle_approvals.Where(c => c.dm_id == id).FirstOrDefault();
                deckle.approver_aspnetuserid = currentUserId;
                deckle.approved_on = DateTime.Now;
                deckle.action = action;
                deckle.remarks = Remark;
                db.SaveChanges();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetDeckleDetails:", Ex);

            }

        }
        //Get Machine Head Details

        public List<TempPendingApproval> GetMachineheadDetails(int Id)
        {
            try
            {
                var query = (from pr in db.ProductionRun join pm in db.Papermills on pr.papermill_id equals pm.papermill_id where pr.pr_id == Id select new { pm.name, pr.estimated_start }).SingleOrDefault();
                var query1 = (from asp in db.AspNetUsers where asp.AssignedMachine == query.name select new TempPendingApproval { machHName = asp.name, macname = asp.AssignedMachine, Addess1 = asp.Email, estimated_start = query.estimated_start, papermillName = query.name, CustomerName = asp.Id }).ToList();
                return query1;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetMachineheadDetails:", Ex);
                return null;
            }

        }
        //Get Papermill Name
        public string GetPapermillName(int Id)
        {
            try
            {
                string Name = (from pr in db.ProductionRun join pm in db.Papermills on pr.papermill_id equals pm.papermill_id where pr.pr_id == Id select pm.name).SingleOrDefault();
                return Name;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetPapermillName:", Ex);
                return null;
            }

        }
        //Update Path in Production run Table of Pdf file
        public bool UpdatePathForPdfPP(string filepath, int id)
        {
            try
            {
                var query = db.ProductionRun.FirstOrDefault(k => k.pr_id == id);
                if (query != null)
                {
                    query.pdf_file = filepath;
                    db.SaveChanges();


                }
                return true;
            }
            catch (Exception Ex)
            {

                logger.Error("Error in ProductionPlannerRepository->UpdatePathForPdfPP:", Ex);
                return false;
            }

        }
        //Get File Path for Email Attachment
        public string GetFilePath(int id)
        {
            try
            {
                string pathname = string.Empty;
                pathname = (from pr in db.ProductionRun where pr.pr_id == id select pr.pdf_file).SingleOrDefault();
                return pathname;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetFilePath:", Ex);
                return null;
            }

        }
        //Get List Of Agent Email For Shortfall Qty
        public List<Agent> lstemails()
        {
            try
            {
                var query = db.Agents.ToList();
                return query;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->lstemails:", Ex);
                return null;
            }
        }
        //Get Mail Details for Order Approvel
        public List<string> GetPapermillDetails(int millid, int ord)
        {
            try
            {
                List<string> lststring = new List<string>();
                string machname = (from m in db.Papermills where m.papermill_id == millid select m.name).SingleOrDefault();
                string location = (from m in db.Papermills where m.papermill_id == millid select m.location).SingleOrDefault();
                string custname = (from c in db.Customers join o in db.Orders on c.customer_id equals o.customer_id where o.order_id == ord select c.name).SingleOrDefault();
                lststring.Add(machname);
                lststring.Add(location);
                lststring.Add(custname);
                return lststring;

            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetPapermillDetails:", Ex);
                return null;
            }
        }

        public string GetOrderAgentName(int? OrdId)
        {
            try
            {
                string orderdAgentName = db.Agents.Where(c => c.agent_id == OrdId).Select(c => c.name).FirstOrDefault();
                return orderdAgentName;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetOrderAgentName:", Ex);
                return null;

            }
        }

        public List<TempJumboDetails> ProductionPlanJmbosDetailsPdf(int id)
        {
            try
            {
                var query = (from r in db.ProductionJumbo
                             where r.pr_id == id
                             select new TempJumboDetails
                             {
                                 BF = r.bf_code,
                                 GSM = r.gsm_code,
                                 shade = r.shade_code,
                                 jumboNo = r.jumbo_no,
                                 plannedQty = r.planned_qty,
                                 estimated_start = r.estimated_start,

                             });

                return query.ToList();

            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->ProductionPlanJmbosDetails:", Ex);
                return null;
            }
        }
        //For Getting Shades
        public IQueryable GetShades()
        {
            try
            {
                var shadecode = db.Schedule.Where(x => x.status == "Active").Select(d => d.shade_code).Distinct();
                return shadecode;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetShades:", Ex);
                return null;
            }
        }

        public List<TempPendingApproval> GetPapermillNamesonShade(string shadecode)
        {
            try
            {

                List<TempPendingApproval> query = (from sh in db.Schedule join pm in db.Papermills on sh.papermill_id equals pm.papermill_id where sh.shade_code == shadecode select new TempPendingApproval { schedule_id = sh.schedule_id, name = pm.name }).ToList();
                return query;
            }
            catch (Exception Ex)
            {

                logger.Error("Error in ProductionPlannerRepository->GetShades:", Ex);
                return null;
            }



        }

        public List<TempJumboDetails> GetBfgsmShade(string SchName)
        {
            try
            {

                //List<TempPendingApproval> lst = (from op in db.Order_products join o in db.Orders on op.order_id equals o.order_id where op.shade_code == SchName  select new TempPendingApproval {bf=o.bf ,order_id = op.order_id, width = op.width, product_code = op.product_code, qty = op.qty, requested_delivery_date = op.requested_delivery_date, shade = op.shade_code }).ToList<TempPendingApproval>();
                //return lst;
                int? value = null;
                List<TempJumboDetails> orderProductList = (from r in db.Order_products
                                                           join re in db.Products on r.product_code equals re.product_code
                                                           where (r.shade_code == SchName) && (r.schedule_id == null ? r.schedule_id.Equals(value) : r.schedule_id == value)
                                                           && r.status == "Created"
                                                           select new TempJumboDetails
                                                           {
                                                               order_product_id = r.order_product_id,
                                                               order_id = r.order_id,
                                                               BF = re.bf_code,
                                                               GSM = re.gsm_code,
                                                               shade = r.shade_code,
                                                               qty = r.qty,
                                                               width = r.width,
                                                               RequestedDate = r.requested_delivery_date
                                                           }).ToList<TempJumboDetails>();
                return orderProductList;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetBfgsmShade:", Ex);
                return null;
            }

        }

        //Sourcing Report
        public Array GetSourcingData(DateTime fromdate, DateTime todate)
        {
            try
            {
                List<TempPendingApproval> lst = null;
                //var lst = (from pc in db.ProductionChild
                //           join pj in db.ProductionJumbo on pc.pj_id equals pj.pj_id
                //           let dt = pj.estimated_start
                //           join op in db.Order_products on pc.order_product_id equals op.order_product_id
                //           join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                //           join pm in db.Papermills on sch.papermill_id equals pm.papermill_id
                //           join pr in db.Products on op.product_code equals pr.product_code
                //           where (pj.estimated_start >= fromdate) && (pj.estimated_start <= todate)
                //           group new { pc, pj, op, pr, pm } by EntityFunctions.TruncateTime(pj.estimated_start) into g
                //           orderby g.FirstOrDefault().pj.estimated_start
                //           select new
                //           {
                //               estimated_start = g.Key,
                //               qty = g.Sum(k => k.pc.qty),
                //               gsm = g.FirstOrDefault().pr.gsm_code,
                //               bf = g.FirstOrDefault().pr.bf_code,
                //               shade = g.FirstOrDefault().op.shade_code,
                //               papermillName = g.FirstOrDefault().pm.name
                //           }).ToList();
                //{
                //    requested_delivery_date = g.Where(h => h.estimated_start == h.estimated_start).Select(t => t.estimated_start).FirstOrDefault(),
                //    qty = g.Where(c => c.estimated_start == c.estimated_start).Sum(c => c.qty),
                //    machineName = g.Where(h => h.papermillName == h.papermillName).Select(t => t.papermillName).FirstOrDefault(),
                //    shade_code = g.Where(h => h.shade == h.shade).Select(t => t.shade).FirstOrDefault(),
                //    bf = g.Where(h => h.bf == h.bf).Select(t => t.bf).FirstOrDefault(),
                //    gsm = g.Where(h => h.gsm == h.gsm).Select(t => t.gsm).FirstOrDefault(),

                //}).AsEnumerable();
                var lst1 = (from pc in db.ProductionChild
                            join pj in db.ProductionJumbo on pc.pj_id equals pj.pj_id
                            let dt = EntityFunctions.TruncateTime(pj.estimated_start)
                            join op in db.Order_products on pc.order_product_id equals op.order_product_id
                            join sch in db.Schedule on op.schedule_id equals sch.schedule_id
                            join pm in db.Papermills on sch.papermill_id equals pm.papermill_id
                            join po in db.Products on op.product_code equals po.product_code
                            where (pj.estimated_start >= fromdate) && (pj.estimated_start <= todate)
                            group new { pc, pj, op, po, pm } by new { dt, op.shade_code, pm.name, po.gsm_code, po.bf_code } into g
                            orderby g.FirstOrDefault().pm.name, g.FirstOrDefault().po.bf_code, g.FirstOrDefault().po.gsm_code, g.FirstOrDefault().op.shade_code
                            select new
                            {
                                estimated_start = g.FirstOrDefault().pj.estimated_start,
                                papermillName = g.FirstOrDefault().pm.name,
                                gsm = g.FirstOrDefault().po.gsm_code,
                                bf = g.FirstOrDefault().po.bf_code,
                                shade = g.FirstOrDefault().op.shade_code,
                                qty = g.Sum(k => k.pc.qty),


                            }).ToList();





                string[,] objArray = new string[1, 1];

                int TotRow = 1;
                int TotCol = 0;
                TimeSpan diff = todate - fromdate;
                int NoofDays = diff.Days + 1; //increased by 1 to accomdate both days

                TotCol = 4 + NoofDays; //columns resized to Number of days the crosstab report is required
                objArray = (string[,])ResizeArray(objArray, TotRow, TotCol);

                //create the Header Now in Index 0 with the column names
                objArray[0, 0] = "Papermill Name";
                objArray[0, 1] = "BF";
                objArray[0, 2] = "GSM";
                objArray[0, 3] = "Shade";
                for (int i = 4; i <= TotCol - 1; i++)
                {
                    //Initialize header to Dates from From Date to End Date
                    objArray[0, i] = fromdate.AddDays(i - 4).ToShortDateString();
                }

                //Run Loop for each item in the list
                foreach (var item in lst1)
                {
                    int FindRow, FindCol;
                    FindRow = FindRowInArray(objArray, item.papermillName, item.bf, item.gsm, item.shade);
                    if (FindRow == -1) // Did not find an existing row 
                    {
                        //Add the combination to the array
                        objArray = (string[,])AddRowtoArray(objArray, item.papermillName, item.bf, item.gsm, item.shade);
                        FindRow = objArray.GetUpperBound(0); //get the last index that was what was added
                    }

                    FindCol = FindDateinArray(objArray, Convert.ToDateTime(item.estimated_start));
                    if (FindCol == -1)
                    {
                        //We are in soup, handle softerror here
                    }
                    else
                    {
                        //Update Qty in Array at the relevant Index
                        objArray[FindRow, FindCol] = item.qty.ToString();
                    }

                }

                return objArray;


            }
            catch (Exception Ex)
            {

                logger.Error("Error in ProductionPlannerRepository->GetSourcingData:", Ex);
                return null;
            }
        }

        private static Array ResizeArray(Array arr, int newrow, int newcol)
        {

            var temp = Array.CreateInstance(arr.GetType().GetElementType(), newrow, newcol);
            int length = arr.Length <= temp.Length ? arr.Length : temp.Length;
            Array.ConstrainedCopy(arr, 0, temp, 0, length);
            return temp;
        }

        private int FindRowInArray(Array arr, string PM, string BF, string GSM, string Shade)
        {
            for (int row = arr.GetLowerBound(0); row <= arr.GetUpperBound(0); row++)  //Bound 0 = Row, Bound 1 = Column
            {
                if (arr.GetValue(row, 0).ToString() == PM && arr.GetValue(row, 1).ToString() == BF && arr.GetValue(row, 2).ToString() == GSM && arr.GetValue(row, 3).ToString() == Shade)
                {
                    //Found a match
                    return row;
                }
            }
            //If we got here means, we didnt get a match,
            return -1;

        }

        private Array AddRowtoArray(Array arr, string PM, string BF, string GSM, string Shade)
        {
            //create a new Row in the Array with the Combination and return the index
            //get the Max row index and add 1 to it
            int NewRow = arr.GetUpperBound(0) + 1; //will return the Max Row, Add 1 for index starting from zero
            int NewCol = arr.GetUpperBound(1) + 1;   // will return the Max column
            arr = (string[,])ResizeArray(arr, NewRow + 1, NewCol);  //Resize the array
            //Assign values to the row
            arr.SetValue(PM, NewRow, 0);
            arr.SetValue(BF, NewRow, 1);
            arr.SetValue(GSM, NewRow, 2);
            arr.SetValue(Shade, NewRow, 3);
            return arr;
        }
        private int FindDateinArray(Array arr, DateTime date)
        {
            for (int col = 4; col <= arr.GetUpperBound(1); col++)  //start from 4 index as that is where the dates start
            {
                if (arr.GetValue(0, col).ToString() == date.ToShortDateString())
                {
                    //Found the Matching Date
                    return col;
                }

            }
            return -1; // This situation should not arise, as the date should always be found, this is worst case scenario
        }

        public string getStackholderEmail(string id)
        {
            try
            {
                string query = db.AspNetUsers.Where(k => k.Id == id).Select(k => k.Email).SingleOrDefault();
                return query;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->getStackholderEmail:", Ex);
                return null;
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


        // Get All Schudle Status wise
        public List<Schedule> GetSchudle(string status)
        {
            try
            {
                return db.Schedule.Where(k => k.status == status).ToList();
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->GetSchudle:", Ex);
                return null;
            }

        }
        //Create Schudle
        public bool CreateSchudle(Schedule lstCreate)
        {
            try
            {
                db.Schedule.Add(lstCreate);
                db.SaveChanges();
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->CreateSchudle:", Ex);
                return false;
            }
        }

        //Edit Schudle
        public bool EditSchudle(Schedule lstEdit)
        {
            try
            {
                var query = db.Schedule.Where(k => k.schedule_id == lstEdit.schedule_id).SingleOrDefault();
                if (query != null)
                {
                    query.papermill_id=lstEdit.papermill_id;
                    query.shade_code=lstEdit.shade_code;
                    query.start_date=lstEdit.start_date;
                    query.end_date=lstEdit.end_date;
                    query.created_on=lstEdit.created_on;
                    query.created_by="ProductionPlanner";
                    query.modified_on=DateTime.Now;
                    query.modified_by="";
                    query.status=lstEdit.status;
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->EditSchudle:", Ex);
                return false;
            }
        }

        //Delete Schudle
        public bool DeleteSchudle(int schID)
        {
            try
            {
                var query = db.Schedule.Where(k => k.schedule_id == schID).SingleOrDefault();
                if (query != null)
                {
                    db.Schedule.Remove(query);
                    db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ProductionPlannerRepository->DeleteSchudle:", Ex);
                return false;
            }
        }

    }
}