using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWV.Repository.Interfaces;
using MWV.Models;
using System.Reflection;
using MWV.DBContext;

namespace MWV.Repository.Implementation
{
    public class StackholderRepository
    {
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private MWVDBContext db = new MWVDBContext();


        public bool saveReports(Reports rptdata)
        {
            try
            {
                db.Reports.Add(rptdata);
                db.SaveChanges();
                return true;
            }
            catch (Exception Ex)
            {
                logger.Error("Error in ReportsRepository->saveReports:", Ex);
                return false;
            }
        }
        //Get Recent Reports By All Reports
        public List<Reports> GetLastRecords(string reportname)
        {
            try
            {
                if (reportname == "")
                {
                    var dt = DateTime.Now.AddDays(-7);
                    List<Reports> lst = (from rp in db.Reports
                                         where rp.created_on > dt
                                         orderby rp.created_on descending
                                         select rp).Take(5).ToList();
                    return lst;
                }
                else
                {

                    var dt = DateTime.Now.AddDays(-7);
                    List<Reports> lst = (from rp in db.Reports
                                         where rp.created_on > dt && rp.report_name == reportname
                                         orderby rp.created_on descending
                                         select rp).Take(5).ToList();
                    return lst;

                }

            }
            catch (Exception Ex)
            {
                logger.Error("Error in ReportsRepository->GetLastRecords:", Ex);
                return null;
            }
        }


    }
}