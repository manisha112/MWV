using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWV.Models;
using MWV.DBContext;
using MWV.Repository.Interfaces;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;
using System.Web.WebPages.Html;
using System.Web.Mvc;


namespace MWV.Models
{
    public class TempPendingApproval
    {

        [Key]
        public DateTime? actual_end { get; set; }
        public DateTime? actualStart { get; set; }
        public int order_id { get; set; }
        public int ordCount { get; set; }
        [Display(Name = "Select Customer")]
        public int? customer_id { get; set; }
        public int? agentId { get; set; }
        public string ordStatus { get; set; }
        public string CustStatus { get; set; }
        public decimal? CreditLimit { get; set; }
        public string remarks { get; set; }
        public string AgentName { get; set; }
        public string CustomerName { get; set; }
        public DateTime? CustCreatedOn { get; set; }
        public string Addess1 { get; set; }
        public string Addess2 { get; set; }
        public string Addess3 { get; set; }
        public int? CreditDays { get; set; }
        public string City { get; set; }
        public int? jumbo_no { get; set; }
        public int pappermill_id { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public virtual Agent Agent { get; set; }
        public string papermillName { get; set; }
        public string PinNo { get; set; }
        public DateTime? estimated_start { get; set; }
        public int srNo { get; set; }
        public int pjid { get; set; }
        public string Phone { get; set; }
        public string fax { get; set; }
        public DateTime? EstimatedEndDate { get; set; }
        public int? runTime { get; set; }
        public decimal? EstimatedRunTime { get; set; }
        public decimal? plannedQty { get; set; }
        public decimal? ActualQty { get; set; }
        public string DuplicateCustomerName { get; set; }
        public string DuplicateAgentName { get; set; }
        public int DuplicateCustId { get; set; }
        public virtual Customer Customer { get; set; }

        public int alert_id { get; set; }
        public string alert_action { get; set; }
        public string alert_for_role { get; set; }
        public int alert_for_agentid { get; set; }
        public string alert_text { get; set; }
        public string alert_subject { get; set; }
        public DateTime? created_on { get; set; }
        public string created_by { get; set; }

        public int msgid { get; set; }
        public string onAction { get; set; }
        public string usrrole { get; set; }
        public int msgagntid { get; set; }
        public string msgtext { get; set; }
        public string msgsubject { get; set; }
        public string recipient_first { get; set; }
        public string recipient_sec { get; set; }
        public string cc_first { get; set; }
        public string cc_sec { get; set; }
        public string bcc_first { get; set; }
        public string bcc_sec { get; set; }
        public DateTime? createdon { get; set; }
        public string createdby { get; set; }
        public string attachment1 { get; set; }
        public string attachment2 { get; set; }
        public string msg_status { get; set; }

        public string machHName { get; set; }
        public string macname { get; set; }
        public DateTime? SchStart { get; set; }
        public int? viewed { get; set; }
        public int? schedule_id { get; set; }
        public string name { get; set; }

        public decimal? width { get; set; }
        public string product_code { get; set; }
        public decimal? qty { get; set; }
        public DateTime? requested_delivery_date { get; set; }
        public string shade { get; set; }
        public string bf { get; set; }
        public string gsm { get; set; }
        public int? order_id_feedback { get; set; }
        public decimal? qtyDispatchFacility { get; set; }
        public int? order_product_id {get;set; }
        public decimal? qtyUnderplanning { get; set; }
        public decimal? qtyLoaded { get; set; }
        public decimal? qtySchForLoading { get; set; }
    }
}