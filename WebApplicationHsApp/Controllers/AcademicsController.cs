using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class AcademicsController : Controller
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Academics
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DoplerHeart()
        {
            return View();
        }
        public ActionResult AcademicRota()
        {
            List<tbl_Settings> notification = myapp.tbl_Settings.Where(l => l.SettingKey == "AcademicRota").ToList();
            if (notification.Count > 0)
            {
                ViewBag.Nofication = notification[0].SettingValue;
            }
            return View();
        }
        [Authorize]
        public ActionResult ManageVideos()
        {
            return View();
        }
        [Authorize]
        public ActionResult ManageHomePageVideos()
        {
            return View();
        }
        [Authorize]
        public ActionResult Videos()
        {
            List<tbl_AcademicsVideos> model = myapp.tbl_AcademicsVideos.Where(l => l.IsActive == true && l.VideoLocation == "Academics").ToList();
            return View(model);
        }
        public ActionResult AjaxGetVideos(JQueryDataTableParamModel param)
        {

            List<tbl_AcademicsVideos> query = myapp.tbl_AcademicsVideos.OrderByDescending(l => l.AcademicsVideoId).ToList();
            IEnumerable<tbl_AcademicsVideos> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.AcademicsVideoId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CategoryName != null && c.CategoryName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Description != null && c.Description.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_AcademicsVideos> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[] {
                                              c.AcademicsVideoId.ToString(),
                                              c.Name,
                                              c.CategoryName,
                                              c.Description,
                                               c.VideoUrl,
                                                c.VideoType,
                                                c.VideoLocation,
                                                c.TotalNoOfViews.HasValue?c.TotalNoOfViews.ToString():"0",
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.AcademicsVideoId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveNewVideo(tbl_AcademicsVideos model, HttpPostedFileBase ExcelFileData)
        {
            try
            {
                string NewGUID = Guid.NewGuid().ToString();

                string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + Path.GetExtension(ExcelFileData.FileName);
                string PathName = Path.Combine(Server.MapPath("~/Videos/"), UniqueFileName);
                ExcelFileData.SaveAs(PathName);
                model.VideoUrl = UniqueFileName;
                model.VideoType = "mp4";
                model.IsActive = true;
                model.CreatedBy = User.Identity.Name;
                model.CreatedOn = DateTime.Now;
                model.ModfiedBy = User.Identity.Name;
                model.ModifedOn = DateTime.Now;
                model.TotalNoOfViews = 0;
                myapp.tbl_AcademicsVideos.Add(model);
                myapp.SaveChanges();
                ViewBag.Message = "Success";
                return RedirectToAction("ManageVideos");
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetVideoDetails(int id)
        {
            tbl_AcademicsVideos list = myapp.tbl_AcademicsVideos.Where(l => l.AcademicsVideoId == id).SingleOrDefault();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult SaveNewVideo(tbl_AcademicsVideos model)
        //{
        //    model.IsActive = true;
        //    model.CreatedBy = User.Identity.Name;
        //    model.CreatedOn = DateTime.Now;
        //    model.ModfiedBy = User.Identity.Name;
        //    model.ModifedOn = DateTime.Now;
        //    model.TotalNoOfViews = 0;
        //    myapp.tbl_AcademicsVideos.Add(model);
        //    myapp.SaveChanges();
        //    return Json("Success", JsonRequestBehavior.AllowGet);
        //}

        public ActionResult UpdateVideoDetials(int AcademicsVideoId, string Name, string Description, string CategoryName)
        {
            tbl_AcademicsVideos list = myapp.tbl_AcademicsVideos.Where(l => l.AcademicsVideoId == AcademicsVideoId).SingleOrDefault();
            list.Name = Name;
            list.Description = Description;
            list.CategoryName = CategoryName;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteVideo(int id)
        {
            tbl_AcademicsVideos list = myapp.tbl_AcademicsVideos.Where(l => l.AcademicsVideoId == id).SingleOrDefault();
            if (list != null)
            {
                int accid = list.AcademicsVideoId;
                myapp.tbl_AcademicsVideoHistory.RemoveRange(myapp.tbl_AcademicsVideoHistory.Where(l => l.AcademicsVideoId == accid));
                myapp.SaveChanges();
                string PathName = Path.Combine(Server.MapPath("~/Videos/"), list.VideoUrl);
                if (System.IO.File.Exists(PathName))
                {
                    System.IO.File.Delete(PathName);
                }
                list.IsActive = false;
                myapp.tbl_AcademicsVideos.Remove(list);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult MidwifeTraining()
        {
            List<tbl_AcademicsVideos> model = myapp.tbl_AcademicsVideos.Where(l => l.IsActive == true && l.VideoLocation == "Midwife_Training").ToList();
            return View(model);
        }

        public ActionResult UpdateViews(int id)
        {
            string currentuser = User.Identity.Name;
            DateTime Date = DateTime.Now.Date;
            bool updatecount = false;
            List<tbl_AcademicsVideoHistory> modelhistory = myapp.tbl_AcademicsVideoHistory.Where(l => l.AcademicsVideoId == id && l.UserId == currentuser && l.DateOfLastViwed == Date).ToList();
            if (modelhistory.Count > 0)
            {
                foreach (tbl_AcademicsVideoHistory v in modelhistory)
                {
                    if (v.DateOfViwed.Value.Hour != DateTime.Now.Hour && v.DateOfViwed.Value.Hour != (DateTime.Now.Hour - 1))
                    {
                        updatecount = true;
                    }
                }
            }
            else
            {
                updatecount = true;
            }
            if (updatecount)
            {
                tbl_AcademicsVideos model = myapp.tbl_AcademicsVideos.Where(l => l.AcademicsVideoId == id).SingleOrDefault();
                if (model != null)
                {
                    model.TotalNoOfViews = model.TotalNoOfViews + 1;
                    myapp.SaveChanges();
                }
                tbl_AcademicsVideoHistory modelhis = new tbl_AcademicsVideoHistory
                {
                    AcademicsVideoId = id,
                    Comments = "",
                    DateOfViwed = DateTime.Now,
                    DateOfLastViwed = DateTime.Now.Date,
                    TotalNoOfViews = 1,
                    UserId = currentuser
                };
                myapp.tbl_AcademicsVideoHistory.Add(modelhis);
                myapp.SaveChanges();
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult MDTrackonVideos()
        {
            return View();
        }
        public ActionResult UsersViewdHistory()
        {

            List<VideosViewedHistroyViewModel> list = myapp.Database.SqlQuery<VideosViewedHistroyViewModel>(" select v.AcademicsVideoId, v.Name, v.CategoryName,convert(varchar(150),vs.DateOfViwed) as DateOfViwed,vs.TotalNoOfViews,vs.UserId,usr.FirstName+' '+usr.LastName as UserName from tbl_AcademicsVideos v   inner join tbl_AcademicsVideoHistory vs on vs.AcademicsVideoId = v.AcademicsVideoId   inner join tbl_User usr on usr.CustomUserId = vs.UserId   where v.VideoLocation = 'Midwife_Training'").ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxUsersViewdHistory(JQueryDataTableParamModel param)
        {

            List<VideosViewedHistroyViewModel> list = myapp.Database.SqlQuery<VideosViewedHistroyViewModel>(" select v.AcademicsVideoId, v.Name, v.CategoryName,convert(varchar(150),vs.DateOfViwed) as DateOfViwed,vs.TotalNoOfViews,vs.UserId,usr.FirstName+' '+usr.LastName as UserName from tbl_AcademicsVideos v   inner join tbl_AcademicsVideoHistory vs on vs.AcademicsVideoId = v.AcademicsVideoId   inner join tbl_User usr on usr.CustomUserId = vs.UserId   where v.VideoLocation = 'Midwife_Training' order by vs.DateOfViwed desc ").ToList();
            IEnumerable<VideosViewedHistroyViewModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = list
                   .Where(c => c.AcademicsVideoId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CategoryName != null && c.CategoryName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.UserName != null && c.UserName.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = list;
            }
            IEnumerable<VideosViewedHistroyViewModel> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[] {
                                              //c.AcademicsVideoId.ToString(),
                                               c.DateOfViwed,
                                              c.Name,
                                              c.CategoryName,
                                               c.UserId,
                                              c.UserName,
                                               c.TotalNoOfViews.ToString()

                                                };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = list.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportExcelVideoViews()
        {
            List<VideosViewedHistroyViewModel> list = myapp.Database.SqlQuery<VideosViewedHistroyViewModel>(" select v.AcademicsVideoId, v.Name, v.CategoryName,convert(varchar(150),vs.DateOfViwed) as DateOfViwed,vs.TotalNoOfViews,vs.UserId,usr.FirstName+' '+usr.LastName as UserName from tbl_AcademicsVideos v   inner join tbl_AcademicsVideoHistory vs on vs.AcademicsVideoId = v.AcademicsVideoId   inner join tbl_User usr on usr.CustomUserId = vs.UserId   where v.VideoLocation = 'Midwife_Training' order by vs.DateOfViwed desc ").ToList();
            System.Data.DataTable products = new System.Data.DataTable("EmployeesLeaveApplications");
            // products.Rows.Add("Client  : " + list[0].ClientName + " SOA From : " + FromDate + " To : " + ToDate);
            //  products.Columns.Add("UserId", typeof(string));
            products.Columns.Add("Date Of Viewed", typeof(string));
            products.Columns.Add("Name", typeof(string));
            products.Columns.Add("Category", typeof(string));
            products.Columns.Add("User Id", typeof(string));
            products.Columns.Add("User  Name", typeof(string));
            products.Columns.Add("Total NoOf Views", typeof(string));
            foreach (VideosViewedHistroyViewModel c in list)
            {
                products.Rows.Add(c.DateOfViwed,
                                 c.Name,
                                 c.CategoryName,
                                 c.UserId,
                                 c.UserName,
                                 c.TotalNoOfViews

                );

            }


            GridView grid = new GridView
            {
                GridLines = GridLines.Both,
                BorderStyle = BorderStyle.Solid
            };

            grid.HeaderStyle.BackColor = System.Drawing.Color.Teal;
            grid.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grid.DataSource = products;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            string filename = "EmployeesLeaveApplications.xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + filename);
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }
        [AllowAnonymous]
        public ActionResult OurMission()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult NewsFrom()
        {
            List<tbl_Protocol> list = myapp.tbl_Protocol.Where(l => l.PageName == "Events").OrderByDescending(l => l.CreatedOn).ToList();
            return View(list);
        }
        [AllowAnonymous]
        public ActionResult NewsFromImages(string path)
        {
            string pathserver = Server.MapPath("~/Images/December/" + path);
            DirectoryInfo di = new DirectoryInfo(pathserver);
            var Images = di.GetFiles("*.jpg").Select(l => l.Name).ToArray();
            Array.Sort(Images, new AlphanumComparatorFast());
            return View(Images.ToList());
        }


        public ActionResult AjaxGetHomePageVideos(JQueryDataTableParamModel param)
        {
            List<tbl_HomePageVideos> query = myapp.tbl_HomePageVideos.OrderByDescending(l => l.HomePageVideosId).ToList();
            IEnumerable<tbl_HomePageVideos> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.HomePageVideosId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())

                               ||
                              c.Description != null && c.Description.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_HomePageVideos> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[] {
                                              c.HomePageVideosId.ToString(),
                                              c.Name,
                                              c.Description,
                                               c.VideoUrl,
                                                c.VideoType,
                                               // c.VideoLocation,
                                                c.TotalNoOfViews.HasValue?c.TotalNoOfViews.ToString():"0",
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.HomePageVideosId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveNewHomePageVideo(tbl_HomePageVideos model, HttpPostedFileBase ExcelFileData)
        {
            try
            {
                string NewGUID = Guid.NewGuid().ToString();

                string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + Path.GetExtension(ExcelFileData.FileName);
                string PathName = Path.Combine(Server.MapPath("~/Videos/"), UniqueFileName);
                ExcelFileData.SaveAs(PathName);
                model.VideoUrl = UniqueFileName;
                model.VideoType = "mp4";
                model.IsActive = true;
                model.CreatedBy = User.Identity.Name;
                model.CreatedOn = DateTime.Now;
                model.TotalNoOfViews = 0;
                myapp.tbl_HomePageVideos.Add(model);
                myapp.SaveChanges();
                ViewBag.Message = "Success";
                return RedirectToAction("ManageHomePageVideos");
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }

        }
        public ActionResult GetHomePageVideoDetails(int id)
        {
            tbl_HomePageVideos list = myapp.tbl_HomePageVideos.Where(l => l.HomePageVideosId == id).SingleOrDefault();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UpdateHomePageVideoDetials(int AcademicsVideoId, string Name, string Description)
        {
            tbl_HomePageVideos list = myapp.tbl_HomePageVideos.Where(l => l.HomePageVideosId == AcademicsVideoId).SingleOrDefault();
            list.Name = Name;
            list.Description = Description;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteHomePageVideo(int id)
        {
            tbl_HomePageVideos list = myapp.tbl_HomePageVideos.Where(l => l.HomePageVideosId == id).SingleOrDefault();
            if (list != null)
            {
                int accid = list.HomePageVideosId;
                list.IsActive = false;
                myapp.tbl_HomePageVideos.Remove(list);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadAcademicRota()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadAcademicRota(HttpPostedFileBase ExcelFileData)
        {
            string NewGUID = Guid.NewGuid().ToString();

            string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + Path.GetExtension(ExcelFileData.FileName);
            string PathName = Path.Combine(Server.MapPath("~/ExcelUplodes/"), UniqueFileName);
            ExcelFileData.SaveAs(PathName);

            List<tbl_Settings> tb = myapp.tbl_Settings.Where(a => a.SettingKey == "AcademicRota").ToList();
            if (tb.Count > 0)
            {
                string PathName1 = Path.Combine(Server.MapPath("~/ExcelUplodes/"), tb[0].SettingValue);
                if ((System.IO.File.Exists(PathName1)))
                {
                    System.IO.File.Delete(PathName1);
                }
                tb[0].SettingValue = UniqueFileName;
                tb[0].IsActive = true;
                myapp.SaveChanges();
            }
            else
            {
                tbl_Settings tbs = new tbl_Settings
                {
                    IsActive = true,
                    SettingKey = "AcademicRota",
                    SettingValue = UniqueFileName
                };
                myapp.tbl_Settings.Add(tbs);
                myapp.SaveChanges();
            }
            ViewBag.Message = "Success";
            return View();
        }
        public ActionResult ManageGallery()
        {
            return View();
        }
        public ActionResult NewGallery()
        {
            return View();
        }
        public ActionResult AjaxGetMainGallery(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_Img_Gallery select d).OrderByDescending(l => l.GalleryId).ToList();

            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Img_Gallery> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.GalleryId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.GalleryName != null && c.GalleryName.Contains(param.sSearch.ToLower())
                                 ||
                                c.GalleryDescription != null && c.GalleryDescription.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                 c.DateOfGallery != null && c.DateOfGallery.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
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
                                              c.GalleryId.ToString(),
                                               c.GalleryName,
                                             c.GalleryDescription,

                                              c.DateOfGallery.Value.ToString("dd/MM/yyyy HH:mm"),
                                              u.FirstName,
                                              c.CreatedOn!=null?c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.GalleryId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewGallery(string type)
        {
            var list = (from e in myapp.tbl_Img_Gallery select e).ToList();
            if (type == null)
            {
                list = list.Where(l => l.GalleryDescription == "").ToList();
            }
            else
            {
                if (type == "Happenings_at_Fernandez")
                {
                    list = list.Where(l => l.GalleryDescription == type || l.GalleryDescription == "").ToList();
                }
                else
                {
                    list = list.Where(l => l.GalleryDescription == type).ToList();
                }
            }
            var yearlist = list.Select(l => l.DateOfGallery.Value.Year).Distinct().ToList();
            List<ListMainEventModel> models = new List<ListMainEventModel>();
            foreach (var y in yearlist)
            {
                var submodel = list.Where(l => l.DateOfGallery.Value.Year == y).ToList();
                submodel = submodel.OrderByDescending(l => l.DateOfGallery).ToList();
                ListMainEventModel m = new ListMainEventModel
                {
                    Year = y
                };
                m.MainEventModels = (from n in submodel
                                     let llistimg = myapp.tbl_Img_GalleryImages.Where(n1 => n1.GalleryId == n.GalleryId).ToList()
                                     select new MainEventModel
                                     {
                                         Description = n.GalleryDescription,

                                         EventId = n.GalleryId,
                                         EventTitle = n.GalleryName,


                                         StartDateTime = (n.DateOfGallery.Value).ToString("dd MMM"),
                                         MainEventModelImages = (from l in llistimg
                                                                 select new MainEventModelImages
                                                                 {
                                                                     ImageName = l.ImageName,
                                                                     ImagePath = l.ImageLocation,
                                                                     isMainBanner = l.IsMainBanner.HasValue ? l.IsMainBanner.Value : false
                                                                 }).ToList()
                                     }).ToList();
                models.Add(m);
            }
            models = models.OrderByDescending(l => l.Year).ToList();
            return View(models);
        }

        public ActionResult GratitudeGallery(string type)
        {
            var list = (from e in myapp.tbl_Img_Gallery select e).ToList();
            if (type == null)
            {
                list = list.Where(l => l.GalleryDescription == "").ToList();
            }
            else
            {
                if (type == "Happenings_at_Fernandez")
                {
                    list = list.Where(l => l.GalleryDescription == type || l.GalleryDescription == "").ToList();
                }
                else
                {
                    list = list.Where(l => l.GalleryDescription == type).ToList();
                }
            }
            var yearlist = list.Select(l => l.DateOfGallery.Value.Year).Distinct().ToList();
            List<ListMainEventModel> models = new List<ListMainEventModel>();
            foreach (var y in yearlist)
            {
                var submodel = list.Where(l => l.DateOfGallery.Value.Year == y).ToList();
                submodel = submodel.OrderByDescending(l => l.DateOfGallery).ToList();
                ListMainEventModel m = new ListMainEventModel
                {
                    Year = y
                };
                m.MainEventModels = (from n in submodel
                                     let llistimg = myapp.tbl_Img_GalleryImages.Where(n1 => n1.GalleryId == n.GalleryId).ToList()
                                     select new MainEventModel
                                     {
                                         Description = n.GalleryDescription,

                                         EventId = n.GalleryId,
                                         EventTitle = n.GalleryName,


                                         StartDateTime = (n.DateOfGallery.Value).ToString("dd MMM"),
                                         MainEventModelImages = (from l in llistimg
                                                                 select new MainEventModelImages
                                                                 {
                                                                     ImageName = l.ImageName,
                                                                     ImagePath = l.ImageLocation,
                                                                     isMainBanner = l.IsMainBanner.HasValue ? l.IsMainBanner.Value : false
                                                                 }).ToList()
                                     }).ToList();
                models.Add(m);
            }
            models = models.OrderByDescending(l => l.Year).ToList();
            return View(models);
        }
        public ActionResult GetGalleryImages(int id)
        {
            var sublist = myapp.tbl_Img_GalleryImages.Where(a => a.GalleryId == id).ToList();
            return Json(sublist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteEventGallery(int id)
        {
            var list = myapp.tbl_Img_Gallery.Where(l => l.GalleryId == id).ToList();
            foreach (var l in list)
            {
                var sublist = myapp.tbl_Img_GalleryImages.Where(a => a.GalleryId == l.GalleryId).ToList();
                foreach (var s in sublist)
                {
                    string PathName = Path.Combine(Server.MapPath("~/Documents/Images/"), s.ImageLocation);
                    if (System.IO.File.Exists(PathName))
                    {
                        System.IO.File.Delete(PathName);
                    }
                    myapp.tbl_Img_GalleryImages.Remove(s);
                    myapp.SaveChanges();
                }
                myapp.tbl_Img_Gallery.Remove(l);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEventGalleryDetails(int id)
        {
            var list = myapp.tbl_Img_Gallery.Where(l => l.GalleryId == id).SingleOrDefault();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateEventTtitle(int id, string title)
        {
            var list = myapp.tbl_Img_Gallery.Where(l => l.GalleryId == id).SingleOrDefault();
            if (list != null)
            {
                list.GalleryName = title;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UploadEventGallery(string GalleryName, string DateOfGallery, string PageType, HttpPostedFileBase Thumbnail, HttpPostedFileBase MainBanner, HttpPostedFileBase[] Upload)
        {
            tbl_Img_Gallery model = new tbl_Img_Gallery();
            model.IsActive = true;
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            model.GalleryName = GalleryName;
            model.GalleryDescription = PageType;
            model.DateOfGallery = ProjectConvert.ConverDateStringtoDatetime(DateOfGallery);
            model.IsActive = true;
            myapp.tbl_Img_Gallery.Add(model);
            myapp.SaveChanges();

            if (Thumbnail != null)
            {
                string NewGUID = Guid.NewGuid().ToString();
                string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + Path.GetExtension(Thumbnail.FileName);
                string PathName = Path.Combine(Server.MapPath("~/Documents/Images/"), UniqueFileName);
                Thumbnail.SaveAs(PathName);
                tbl_Img_GalleryImages submodel = new tbl_Img_GalleryImages();
                submodel.IsActive = true;
                submodel.CreatedBy = User.Identity.Name;
                submodel.CreatedOn = DateTime.Now;
                submodel.GalleryId = model.GalleryId;
                submodel.ImageName = "Thumbnail";
                submodel.ImageLocation = UniqueFileName;
                submodel.IsMainBanner = true;
                myapp.tbl_Img_GalleryImages.Add(submodel);
                myapp.SaveChanges();
            }
            if (MainBanner != null)
            {
                string NewGUID = Guid.NewGuid().ToString();
                string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + Path.GetExtension(MainBanner.FileName);
                string PathName = Path.Combine(Server.MapPath("~/Documents/Images/"), UniqueFileName);
                MainBanner.SaveAs(PathName);
                tbl_Img_GalleryImages submodel = new tbl_Img_GalleryImages();
                submodel.IsActive = true;
                submodel.CreatedBy = User.Identity.Name;
                submodel.CreatedOn = DateTime.Now;
                submodel.GalleryId = model.GalleryId;
                submodel.ImageName = MainBanner.FileName;
                submodel.ImageLocation = UniqueFileName;
                submodel.IsMainBanner = true;
                myapp.tbl_Img_GalleryImages.Add(submodel);
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
                        tbl_Img_GalleryImages submodel = new tbl_Img_GalleryImages();
                        submodel.IsActive = true;
                        submodel.CreatedBy = User.Identity.Name;
                        submodel.CreatedOn = DateTime.Now;
                        submodel.GalleryId = model.GalleryId;
                        submodel.ImageName = MainBanner.FileName;
                        submodel.ImageLocation = UniqueFileName;
                        submodel.IsMainBanner = false;
                        myapp.tbl_Img_GalleryImages.Add(submodel);
                        myapp.SaveChanges();
                    }
                }
            }
            ViewBag.Message = "Success";

            return RedirectToAction("ManageGallery");
        }

        [AllowAnonymous]
        public ActionResult MicrosoftTutorial() {
            return View();
        }
    }
}