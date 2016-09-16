using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MWV.Repository.Implementation;
using MWV.Repository.Interfaces;

namespace MWV.Controllers
{
    public class UserMailerController : Controller
    {
        //
        // GET: /UserMailer/
        public ActionResult Index()
        {
            IUserMailer mailer = new UserMailer();
            //mailer.SendMail().Send(); 
            return View();
        }
	}
}