using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class HelpController : Controller
    {
        // GET: Help
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult UserLeave()
        {
            return View();
        }

        public ActionResult Helpdesk()
        {
            return View();
        }

      
       
        public ActionResult Hod()
        {
            return View();
        }

        public ActionResult HrAdmin()
        {
            return View();
        }


        public ActionResult Admin()
        {
            return View();
        }

      
    }
}