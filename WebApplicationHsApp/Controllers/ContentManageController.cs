using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;
using WebApplicationHsApp.Models.CommonModels;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class ContentManageController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: ContentManage
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        public ActionResult HRMessage()
        {
            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "HRMessage").ToList();
            ViewBag.message = "";
            if (tb.Count > 0)
                return View(tb[0]);
            else
                return View(new tbl_Settings());
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult HRMessage(tbl_Settings tbll)
        {

            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "HRMessage").ToList();
            if (tb.Count > 0)
            {
                tb[0].SettingValue = tbll.SettingValue;
                tb[0].IsActive = true;
                myapp.SaveChanges();
                ViewBag.message = "Success";
            }
            else
            {
                tbl_Settings tbs = new tbl_Settings();
                tbs.IsActive = true;
                tbs.SettingKey = "HRMessage";
                tbs.SettingValue = tbll.SettingValue;
                myapp.tbl_Settings.Add(tbs);
                myapp.SaveChanges();
                ViewBag.message = "Success";
            }
            return View();
        }
        [Authorize]
        public ActionResult HRNotification()
        {
            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "HrNotification").ToList();
            ViewBag.message = "";
            if (tb.Count > 0)
                return View(tb[0]);
            else
                return View(new tbl_Settings());
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult HRNotification(tbl_Settings tbll)
        {

            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "HrNotification").ToList();
            if (tb.Count > 0)
            {
                tb[0].SettingValue = tbll.SettingValue;
                tb[0].IsActive = true;
                myapp.SaveChanges();
                ViewBag.message = "Success";
            }
            else
            {
                tbl_Settings tbs = new tbl_Settings();
                tbs.IsActive = true;
                tbs.SettingKey = "HrNotification";
                tbs.SettingValue = tbll.SettingValue;
                myapp.tbl_Settings.Add(tbs);
                myapp.SaveChanges();
                ViewBag.message = "Success";
            }
            return View();
        }
        [Authorize]
        public ActionResult HrOpenings()
        {
            ViewBag.message = "";
            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "HrOpening").ToList();
            if (tb.Count > 0)
                return View(tb[0]);
            else
                return View(new tbl_Settings());
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult HrOpenings(tbl_Settings tbll)
        {
            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "HrOpening").ToList();
            if (tb.Count > 0)
            {
                ViewBag.message = "Success";
                tb[0].SettingValue = tbll.SettingValue;
                tb[0].IsActive = true;
                myapp.SaveChanges();
            }
            else
            {
                ViewBag.message = "Success";
                tbl_Settings tbs = new tbl_Settings();
                tbs.IsActive = true;
                tbs.SettingKey = "HrOpening";
                tbs.SettingValue = tbll.SettingValue;
                myapp.tbl_Settings.Add(tbs);
                myapp.SaveChanges();
            }
            return View();
        }
        [Authorize]
        public ActionResult MdMessage()
        {
            ViewBag.message = "";
            var tb = myapp.tbl_Settings.Where(a => a.IsActive == true && a.SettingKey == "MdMessage").ToList();
            if (tb.Count > 0)
                return View(tb[0]);
            else
                return View(new tbl_Settings());
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult MdMessage(tbl_Settings tbll)
        {
            var tb = myapp.tbl_Settings.Where(a => a.IsActive == true && a.SettingKey == "MdMessage").ToList();
            if (tb.Count > 0)
            {
                ViewBag.message = "Success";
                tb[0].SettingValue = tbll.SettingValue;
                tb[0].IsActive = true;
                myapp.SaveChanges();
            }
            else
            {
                ViewBag.message = "Success";
                tbl_Settings tbs = new tbl_Settings();
                tbs.IsActive = true;
                tbs.SettingKey = "MdMessage";
                tbs.SettingValue = tbll.SettingValue;
                myapp.tbl_Settings.Add(tbs);
                myapp.SaveChanges();
            }
            return View();
        }
        public ActionResult Events()
        {
            ViewBag.message = "";
            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "Events").ToList();
            if (tb.Count > 0)
                return View(tb[0]);
            else
                return View(new tbl_Settings());
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Events(tbl_Settings tbll)
        {
            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "Events").ToList();
            if (tb.Count > 0)
            {
                ViewBag.message = "Success";
                tb[0].SettingValue = tbll.SettingValue;
                tb[0].IsActive = true;
                myapp.SaveChanges();
            }
            else
            {
                ViewBag.message = "Success";
                tbl_Settings tbs = new tbl_Settings();
                tbs.IsActive = true;
                tbs.SettingKey = "Events";
                tbs.SettingValue = tbll.SettingValue;
                myapp.tbl_Settings.Add(tbs);
                myapp.SaveChanges();
            }
            return View();
        }

        public string SaveResponseOnMdMessageReplay(int ParentId, string Message)
        {
            MdMessagesViewModel modelmd = new MdMessagesViewModel();
            modelmd.SaveResponseOnMdMessageReplay(ParentId, Message, User.Identity.Name);
            return "Thank you! your message has been successfully sent.";
        }
        public string SaveResponseOnMdMessage(string Mobile, string Message)
        {
            MdMessagesViewModel modelmd = new MdMessagesViewModel();
            modelmd.SaveResponseOnMdMessage(Mobile, Message, User.Identity.Name);
            return "Thank you! your message has been successfully sent.";
        }
        public JsonResult GetSettingsResponses(string Key)
        {
            MdMessagesViewModel modelmd = new MdMessagesViewModel();
            var model = modelmd.GetSettingsResponses(Key);

            return Json(model, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetSettingsResponsesUser(string Key)
        {
            MdMessagesViewModel modelmd = new MdMessagesViewModel();
            var model = modelmd.GetSettingsResponsesUser(Key, User.Identity.Name);

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MDMessageReplay()
        {
            return View();
        }
        public ActionResult MDMessages()
        {
            return View();
        }
        [Authorize]
        public ActionResult NewMdMessage()
        {
            return View(new tbl_Settings());
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult NewMdMessage(tbl_Settings tbll)
        {
            var tb = myapp.tbl_Settings.Where(a => a.IsActive == true && a.SettingKey == "MdMessage").ToList();
            if (tb.Count > 0)
            {
                ViewBag.message = "Success";
                tb[0].SettingValue = tbll.SettingValue;
                tb[0].IsActive = false;
                myapp.SaveChanges();
            }

            ViewBag.message = "Success";
            tbl_Settings tbs = new tbl_Settings();
            tbs.IsActive = true;
            tbs.SettingKey = "MdMessage";
            tbs.SettingValue = tbll.SettingValue;
            myapp.tbl_Settings.Add(tbs);
            myapp.SaveChanges();
            return View();
        }
        [Authorize]
        public ActionResult HomeEventsAll()
        {
            ViewBag.message = "";
            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "HomeEventsAll").ToList();
            if (tb.Count > 0)
                return View(tb[0]);
            else
                return View(new tbl_Settings());
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult HomeEventsAll(tbl_Settings tbll)
        {
            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "HomeEventsAll").ToList();
            if (tb.Count > 0)
            {
                ViewBag.message = "Success";
                tb[0].SettingValue = tbll.SettingValue;
                tb[0].IsActive = true;
                myapp.SaveChanges();
            }
            else
            {
                ViewBag.message = "Success";
                tbl_Settings tbs = new tbl_Settings();
                tbs.IsActive = true;
                tbs.SettingKey = "HomeEventsAll";
                tbs.SettingValue = tbll.SettingValue;
                myapp.tbl_Settings.Add(tbs);
                myapp.SaveChanges();
            }
            return View();
        }

        [Authorize]
        public ActionResult TodayTomorrowEventsAll()
        {
            ViewBag.message = "";
            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "TodayTomorrowEventsAll").ToList();
            if (tb.Count > 0)
                return View(tb[0]);
            else
                return View(new tbl_Settings());
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult TodayTomorrowEventsAll(tbl_Settings tbll)
        {
            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "TodayTomorrowEventsAll").ToList();
            if (tb.Count > 0)
            {
                ViewBag.message = "Success";
                tb[0].SettingValue = tbll.SettingValue;
                tb[0].IsActive = true;
                myapp.SaveChanges();
            }
            else
            {
                ViewBag.message = "Success";
                tbl_Settings tbs = new tbl_Settings();
                tbs.IsActive = true;
                tbs.SettingKey = "TodayTomorrowEventsAll";
                tbs.SettingValue = tbll.SettingValue;
                myapp.tbl_Settings.Add(tbs);
                myapp.SaveChanges();
            }
            return View();
        }

        [Authorize]
        public ActionResult ResultsEventsAll()
        {
            ViewBag.message = "";
            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "ResultsEventsAll").ToList();
            if (tb.Count > 0)
                return View(tb[0]);
            else
                return View(new tbl_Settings());
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ResultsEventsAll(tbl_Settings tbll)
        {
            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "ResultsEventsAll").ToList();
            if (tb.Count > 0)
            {
                ViewBag.message = "Success";
                tb[0].SettingValue = tbll.SettingValue;
                tb[0].IsActive = true;
                myapp.SaveChanges();
            }
            else
            {
                ViewBag.message = "Success";
                tbl_Settings tbs = new tbl_Settings();
                tbs.IsActive = true;
                tbs.SettingKey = "ResultsEventsAll";
                tbs.SettingValue = tbll.SettingValue;
                myapp.tbl_Settings.Add(tbs);
                myapp.SaveChanges();
            }
            return View();
        }
    }
}