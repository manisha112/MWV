
using MWV.DBContext;
using System.Net;
using System.Web.WebPages.Razor;
using Microsoft.AspNet.Identity;
using MWV.Models;
using MWV.Repository.Interfaces;
using System.Reflection;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;

namespace MWV.Repository.Implementation
{

    public class UserMailer
    {
        private MWVDBContext db = new MWVDBContext();
        readonly log4net.ILog logger = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

       public bool sendMails(string recipient1, string recipient2, string subject, string cc1, string cc2, string bcc1, string bcc2, string msgtext, string attachment1, string attachment2)
        {
            try
            {
                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress(recipient1));
                if (string.IsNullOrEmpty(recipient2) == false)
                {
                    message.To.Add(new MailAddress(recipient2));
                }
                else if (string.IsNullOrEmpty(cc1) == false)
                {
                    message.CC.Add(cc1);
                }
                else if (string.IsNullOrEmpty(cc2) == false)
                {
                    message.CC.Add(cc2);
                }
                else if (string.IsNullOrEmpty(bcc1) == false)
                {
                    message.Bcc.Add(bcc1);

                }
                else if (string.IsNullOrEmpty(bcc2) == false)
                {
                    message.Bcc.Add(bcc2);

                }
                message.Subject = subject;
                message.Body = msgtext;
                if (attachment1 == null || attachment1 == "")
                { }
                else
                {
                    System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType();
                    contentType.MediaType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                    Attachment objat = new Attachment(attachment1, MediaTypeNames.Application.Pdf);
                    ContentDisposition disposition = objat.ContentDisposition;
                    message.Attachments.Add(objat);

                }
                if (attachment2 == null || attachment2 == "")
                { }
                else
                {
                    System.Net.Mime.ContentType contentType = new System.Net.Mime.ContentType();
                    contentType.MediaType = System.Net.Mime.MediaTypeNames.Application.Pdf;
                    Attachment objat2 = new Attachment(attachment2, MediaTypeNames.Application.Pdf);
                    ContentDisposition disposition = objat2.ContentDisposition;
                    message.Attachments.Add(objat2);

                }

                message.IsBodyHtml = true;
                SmtpClient client = new SmtpClient();
                client.Send(message);
                client.Dispose();
                return true;
            }
            catch (System.Exception ex)
            {
                logger.Error("Error in UserMailerRepository->sendMails: ", ex);
                return false;
            }

        }


    }
}