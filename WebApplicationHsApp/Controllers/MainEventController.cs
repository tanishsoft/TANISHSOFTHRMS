using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    public class MainEventController : Controller
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: MainEvent
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ViewGallery()
        {
            var list = (from e in myapp.tbl_MainEvent select e).ToList();
            var yearlist = list.Select(l => l.StartDateTime.Value.Year).Distinct().ToList();
            List<ListMainEventModel> models = new List<ListMainEventModel>();
            foreach (var y in yearlist)
            {
                var submodel = list.Where(l => l.StartDateTime.Value.Year == y).ToList();
                ListMainEventModel m = new ListMainEventModel
                {
                    Year = y
                };
                m.MainEventModels = (from n in submodel
                                     let llistimg = myapp.tbl_MainEventGallery.Where(n1 => n1.EventId == n.EventId).ToList()
                                     select new MainEventModel
                                     {
                                         Description = n.Description,
                                         Duration = n.Duration,
                                         EventId = n.EventId,
                                         EventTitle = n.EventTitle,
                                         Location = n.Location,
                                         OtherDetails = n.OtherDetails,
                                         StartDateTime = ProjectConvert.ConverDateTimeToString(n.StartDateTime.Value),
                                         MainEventModelImages = (from l in llistimg
                                                                 select new MainEventModelImages
                                                                 {
                                                                     ImageName = l.ImageName,
                                                                     ImagePath = l.ImagePath,
                                                                     isMainBanner = l.IsMainBanner.HasValue ? l.IsMainBanner.Value : false
                                                                 }).ToList()
                                     }).ToList();
                models.Add(m);
            }
            models = models.OrderByDescending(l => l.Year).ToList();
            return View(models);
        }

        //public ActionResult GetAllEvents()
        //{

        //}
        public ActionResult NewEvent()
        {
            return View();
        }
        public ActionResult EventResult(int eventId)
        {
            ViewBag.EventId = eventId;
            var eventcheck = myapp.tbl_MainEvent.Where(l => l.EventId == eventId).SingleOrDefault();
            ViewBag.EventTitle = eventcheck.EventTitle;
            return View();
        }
        [HttpPost]
        public ActionResult UploadEventResult(int eventId, string ResultTitle, string ResultDescription, HttpPostedFileBase MainBanner)
        {
            string ImagePath = "";
            if (MainBanner != null)
            {
                string NewGUID = Guid.NewGuid().ToString();
                string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + Path.GetExtension(MainBanner.FileName);
                string PathName = Path.Combine(Server.MapPath("~/Documents/Images/"), UniqueFileName);
                MainBanner.SaveAs(PathName);
                ImagePath = UniqueFileName;
            }
            tbl_MainEventResult model = new tbl_MainEventResult();
            model.IsActive = true;
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            model.EventId = eventId;
            model.ResultDescription = ResultDescription;
            model.ResultBanner = ImagePath;
            model.ResultTitle = ResultTitle;
            myapp.tbl_MainEventResult.Add(model);
            myapp.SaveChanges();
            return RedirectToAction("EventResult", new { eventId = eventId });
        }
        public ActionResult EventGallery(int eventId)
        {
            ViewBag.EventId = eventId;
            var eventcheck = myapp.tbl_MainEvent.Where(l => l.EventId == eventId).SingleOrDefault();
            ViewBag.EventTitle = eventcheck.EventTitle;
            return View();
        }

        [HttpPost]
        public ActionResult UploadEventGallery(int eventId, HttpPostedFileBase MainBanner, HttpPostedFileBase[] Upload)
        {
            if (MainBanner != null)
            {
                string NewGUID = Guid.NewGuid().ToString();
                string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + Path.GetExtension(MainBanner.FileName);
                string PathName = Path.Combine(Server.MapPath("~/Documents/Images/"), UniqueFileName);
                MainBanner.SaveAs(PathName);
                tbl_MainEventGallery model = new tbl_MainEventGallery();
                model.IsActive = true;
                model.CreatedBy = User.Identity.Name;
                model.CreatedOn = DateTime.Now;
                model.EventId = eventId;
                model.ImageName = MainBanner.FileName;
                model.ImagePath = UniqueFileName;
                model.IsMainBanner = true;
                myapp.tbl_MainEventGallery.Add(model);
                myapp.SaveChanges();
            }
            if (Upload != null)
            {
                foreach (var file in Upload)
                {
                    if (file != null)
                    {
                        string NewGUID = Guid.NewGuid().ToString();
                        string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + Path.GetExtension(file.FileName);
                        string PathName = Path.Combine(Server.MapPath("~/Documents/Images/"), UniqueFileName);
                        file.SaveAs(PathName);
                        tbl_MainEventGallery model = new tbl_MainEventGallery();
                        model.IsActive = true;
                        model.CreatedBy = User.Identity.Name;
                        model.CreatedOn = DateTime.Now;
                        model.EventId = eventId;
                        model.ImageName = file.FileName;
                        model.ImagePath = UniqueFileName;
                        model.IsMainBanner = false;
                        myapp.tbl_MainEventGallery.Add(model);
                        myapp.SaveChanges();
                    }
                }
            }
            ViewBag.Message = "Success";

            return RedirectToAction("Index");
        }
        public ActionResult AjaxGetMainEvent(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_MainEvent select d).OrderByDescending(l => l.EventId).ToList();

            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_MainEvent> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.EventId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.EventTitle != null && c.EventTitle.Contains(param.sSearch.ToLower())
                                 ||
                                c.Description != null && c.Description.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.OtherDetails != null && c.OtherDetails.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.Location != null && c.Location.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                 c.StartDateTime != null && c.StartDateTime.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         select new[] {
                                              c.EventId.ToString(),
                                               c.EventTitle,
                                             c.Description,
                                            c.OtherDetails,
                                              c.Location,
                                              c.StartDateTime.Value.ToString("dd/MM/yyyy HH:mm"),
                                              u.FirstName,
                                              c.CreatedOn!=null?c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.EventId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveNewEvent(MainEventModel model)
        {
            tbl_MainEvent dbModel = new tbl_MainEvent()
            {
                Description = model.Description,
                Duration = model.Duration,
                EventTitle = model.EventTitle,
                Location = model.Location,
                OtherDetails = model.OtherDetails,
                StartDateTime = ProjectConvert.ConverDateStringtoDatetime(model.StartDateTime)
            };

            dbModel.CreatedBy = User.Identity.Name;
            dbModel.CreatedOn = DateTime.Now;
            dbModel.IsActive = true;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;
            myapp.tbl_MainEvent.Add(dbModel);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetLatestEvents()
        {
            var events = myapp.tbl_MainEvent.Where(l => l.IsActive == true).OrderByDescending(l => l.StartDateTime).Take(4).ToList();

            var mevents= (from n in events
                          let llistimg = myapp.tbl_MainEventGallery.Where(n1 => n1.EventId == n.EventId).ToList()
                          select new MainEventModel
                          {
                              Description = n.Description,
                              Duration = n.Duration,
                              EventId = n.EventId,
                              EventTitle = n.EventTitle,
                              Location = n.Location,
                              OtherDetails = n.OtherDetails,
                              StartDateTime = ProjectConvert.ConverDateTimeToString(n.StartDateTime.Value),
                              MainEventModelImages = (from l in llistimg
                                                      select new MainEventModelImages
                                                      {
                                                          ImageName = l.ImageName,
                                                          ImagePath = l.ImagePath,
                                                          isMainBanner = l.IsMainBanner.HasValue ? l.IsMainBanner.Value : false
                                                      }).ToList()
                          }).ToList();
            return Json(mevents, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetToDayandTomorrowEvents()
        {
            DateTime dttoday = DateTime.Now.Date;
            DateTime dtTomorrow = DateTime.Now.AddDays(1).Date;
            var mainevent = myapp.tbl_MainEvent.Where(l => l.IsActive == true).ToList();
            var events = mainevent.Where(l => l.StartDateTime.Value.Date == dttoday || l.StartDateTime.Value.Date == dtTomorrow).Take(4).ToList();


            var mevents = (from n in events
                           let llistimg = myapp.tbl_MainEventGallery.Where(n1 => n1.EventId == n.EventId).ToList()
                           select new MainEventModel
                           {
                               Description = n.Description,
                               Duration = n.Duration,
                               EventId = n.EventId,
                               EventTitle = n.EventTitle,
                               Location = n.Location,
                               OtherDetails = n.OtherDetails,
                               StartDateTime = ProjectConvert.ConverDateTimeToString(n.StartDateTime.Value),
                               MainEventModelImages = (from l in llistimg
                                                       select new MainEventModelImages
                                                       {
                                                           ImageName = l.ImageName,
                                                           ImagePath = l.ImagePath,
                                                           isMainBanner = l.IsMainBanner.HasValue ? l.IsMainBanner.Value : false
                                                       }).ToList()
                           }).ToList();
            return Json(mevents, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEventResults()
        {
            var events = myapp.tbl_MainEventResult.Where(l => l.IsActive == true).Take(6).ToList();
            var mevents = (from e in events
                           join eg in myapp.tbl_MainEvent on e.EventId equals eg.EventId
                           select new
                           {
                               e.EventId,
                               eg.EventTitle,
                               eg.Duration,
                               eg.Location,
                               eg.StartDateTime.Value,
                               eg.OtherDetails,
                               e.ResultDescription,
                               e.ResultBanner,
                               e.ResultTitle
                           }).ToList();
            return Json(mevents, JsonRequestBehavior.AllowGet);
        }
    }
}