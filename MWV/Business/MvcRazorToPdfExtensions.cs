using System;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MWV.Controllers;

namespace MWV.Business
{
    public static class MvcRazorToPdfExtensions
    {
        public static byte[] GeneratePdf(this ControllerContext context, object model=null, string viewName=null,
            Action<PdfWriter, Document> configureSettings=null)
        {
            return new  MvcRazorToPdf().GeneratePdfOutput(context, model, viewName, configureSettings);
        }
    }
}