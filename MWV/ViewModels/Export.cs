using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MWV.Models;
using MWV.DBContext;
using MWV.Repository.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace MWV.ViewModels
{
    public class ExportStock
    {
        public string Customer { get; set; }
        public string PaperMill { get; set; }
        public string BF { get; set; }
        public string GSM { get; set; }
        public string Shade { get; set; }
        public decimal OpeningStock { get; set; }
        public decimal ClosingStock { get; set; }

    }
}