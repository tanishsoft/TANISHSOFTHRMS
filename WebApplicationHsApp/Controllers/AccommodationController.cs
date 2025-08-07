using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApplicationHsApp.Controllers
{
    public class AccommodationController : Controller
    {
        // GET: Accommodation
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BookingRequests()
        {
            return View();
        }
        public ActionResult ApproveRequests()
        {
            return View();
        }
        public ActionResult CurrentOccupied()
        {
            return View();
        }
        public ActionResult ManageLocations()
        {
            return View();
        }
        public ActionResult ManageFlats()
        {
            return View();
        }
        public ActionResult ManageCheckList()
        {
            return View();
        }
        public ActionResult Reports()
        {
            return View();
        }
    }
}