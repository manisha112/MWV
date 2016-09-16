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

namespace MWV.ViewModels
{
    public class TempJumboDetails
    {
        public DateTime? actual_end { get; set; }
        public DateTime? actual_start { get; set; }
        public string combination { get; set; }
        public int? jumboNo { get; set; }
        public int pjid { get; set; }
        public int? order_product_id { get; set; }
        public int? pcid { get; set; }
        public int srNo { get; set; }
        public string BF { get; set; }
        public string GSM { get; set; }
        public string shade { get; set; }
        public decimal? qty { get; set; }
        public decimal? width { get; set; }
        public string widthforddl { get; set; }
        public string fax { get; set; }
        public decimal? Width { get; set; }
        public int? Sequence { get; set; }
        public decimal? DeckleQty { get; set; }
        public string CustomerName { get; set; }
        public decimal? plannedQty { get; set; }
        public DateTime? estimated_start { get; set; }
        public DateTime? RequestedDate { get; set; }
        public string papermillname { get; set; }
        public int? dm_id { get; set; }
        public string product_code { get; set; }
        public int? order_id { get; set; }
        public string custpo { get; set; }
        public string marking { get; set; }
    }
}