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
    public class TaskController : Controller
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Administration/Task/AdminTracker
        public ActionResult Index()
        {
            DashboardModel Model = new DashboardModel
            {
                TotalLicense = myapp.tbl_AdministrationTracker.Where(l => l.FormType == "License").Count(),
                TotalLegalCases = myapp.tbl_AdministrationTracker.Where(l => l.FormType == "Legal Case").Count(),
                TotalLeaseDates = myapp.tbl_AdministrationTracker.Where(l => l.FormType == "Lease Date").Count(),
                TotalAgreements = myapp.tbl_AdministrationTracker.Where(l => l.FormType == "Agreement").Count()
            };
            return View(Model);
        }
        public ActionResult AdminTracker()
        {
            return View();
        }
        public ActionResult AdminTrackerMaster()
        {
            return View();
        }
        public ActionResult HelpDocuments()
        {
            return View();
        }
        public ActionResult ManageTask(int id)
        {
            ViewBag.id = id;
            return View();
        }
        public JsonResult GetVendors()
        {
            List<tbl_Vendor> query = myapp.tbl_Vendor.ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportExcelAdminTracker(string rfromdate, string rtodate, string efromdate, string etodate, string sfromdate, string stodate, int locationid, int departmentid, string status, string renewal, string FormType)
        {
            List<tbl_AdministrationTracker> query = FilterAdminTracker(rfromdate, rtodate, efromdate, etodate, sfromdate, stodate, locationid.ToString(), departmentid.ToString(), 0, 0, status, renewal, FormType);
            var locations = myapp.tbl_Location.ToList();
            var departments = myapp.tbl_CommonDepartment.ToList();
            var modelquery = (from q in query
                                  //join l in locations on q.LocationId equals Convert.ToString(l.LocationId)
                              join d in departments on q.DepartmentId equals Convert.ToString(d.CommonDepartmentId)
                              select new
                              {
                                  TrackerId = q.TrackerId,
                                  LocationId = q.LocationId,
                                  LocationName = string.Join(",", myapp.tbl_Location.Where(l => q.LocationId.Contains(l.LocationId.ToString())).Select(l => l.LocationName).ToList()),
                                  DepartmentId = q.DepartmentId,
                                  DepartmentName = d.Name,
                                  FormType = q.FormType,
                                  AgreementSubType = q.AgreementSubType,
                                  Title = q.Title,
                                  LicenseDepartmentName_Gov = q.LicenseDepartmentName_Gov,
                                  Description = q.Description,
                                  StartDate = q.StartDate.HasValue ? q.StartDate.Value.ToString("dd/MM/yyyy") : "",
                                  EndDate = q.EndDate.HasValue ? q.EndDate.Value.ToString("dd/MM/yyyy") : "",
                                  NextRenewalDate = q.NextRenewalDate.HasValue ? q.NextRenewalDate.Value.ToString("dd/MM/yyyy") : "",
                                  Status = q.Status,
                                  PrimaryUserId = q.PrimaryUserId,
                                  SecondaryUserid = q.SecondaryUserid,
                                  Remainder1NoOfDays = q.Remainder1NoOfDays,
                                  Remainder1NoOfDaysNotes = q.Remainder1NoOfDaysNotes,
                                  Remarks = q.Remarks
                              }).ToList();
            GridView grid = new GridView
            {
                GridLines = GridLines.Both,
                BorderStyle = BorderStyle.Solid
            };

            grid.HeaderStyle.BackColor = System.Drawing.Color.Teal;
            grid.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grid.DataSource = modelquery;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            string filename = "AdminTracker.xls";


            Response.AddHeader("content-disposition", "attachment; filename=" + filename);
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View();
        }
        public List<tbl_AdministrationTracker> FilterAdminTracker(string rfromdate, string rtodate, string efromdate, string etodate, string sfromdate, string stodate, string locationid, string departmentid, int Categoryid, int SubCategoryid, string status, string renewal, string FormType, string Expired = "")
        {
            List<tbl_AdministrationTracker> query = (from d in myapp.tbl_AdministrationTracker select d).ToList();
            if (locationid != "0" && departmentid != "0")
            {

                query = query.Where(l => l.LocationId.Contains(locationid) && l.DepartmentId == departmentid).ToList();
            }
            if (Categoryid != 0 && SubCategoryid == 0)
            {

                query = query.Where(l => l.CategoryId == Categoryid).ToList();
            }
            if (FormType != null && FormType != "")
            {

                query = query.Where(l => l.FormType == FormType).ToList();
            }
            if (locationid != "0" && departmentid == "0")
            {
                query = query.Where(l => l.LocationId.Contains(locationid.ToString())).ToList();
            }
            foreach (var v in query)
            {
                if (v.EndDate < DateTime.Now && v.Status!= "Not using")
                {
                    v.Status = "Expired";
                }
            }
            if (status != "" && status != null && status != "")
            {

                query = query.Where(l => l.Status == status).ToList();
            }
            if (renewal != "" && renewal != null)
            {
                DateTime date = DateTime.Now;

                switch (renewal)
                {
                    case "7 Days":
                        date = date.AddDays(7);
                        break;
                    case "15 Days":
                        date = date.AddDays(15);
                        break;
                    case "30 Days":
                        date = date.AddDays(30);
                        break;
                    case "60 Days":
                        date = date.AddDays(60);
                        break;
                    case "90 Days":
                        date = date.AddDays(90);
                        break;
                    default:
                        break;
                }

                query = query.Where(l => l.NextRenewalDate <= date).ToList();

            }
            if (Expired != "" && Expired != null)
            {
                DateTime date = DateTime.Now;

                switch (Expired)
                {
                    case "7 Days":
                        date = date.AddDays(7);
                        break;
                    case "15 Days":
                        date = date.AddDays(15);
                        break;
                    case "30 Days":
                        date = date.AddDays(30);
                        break;
                    case "60 Days":
                        date = date.AddDays(60);
                        break;
                    case "90 Days":
                        date = date.AddDays(90);
                        break;
                    case "120 Days":
                        date = date.AddDays(120);
                        break;
                    case "150 Days":
                        date = date.AddDays(150);
                        break;
                    default:
                        break;
                }

                query = query.Where(l => l.EndDate <= date).ToList();

            }

            if (sfromdate != "" && sfromdate != null && stodate != null && stodate != "")
            {
                DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(sfromdate);
                DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(stodate);

                query = query.Where(l => l.StartDate >= dtfrom && l.StartDate <= dtto).ToList();
            }
            if (efromdate != "" && efromdate != null && etodate != null && etodate != "")
            {
                DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(efromdate);
                DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(etodate);

                query = query.Where(l => l.EndDate >= dtfrom && l.EndDate <= dtto).ToList();
            }
            if (rfromdate != "" && rfromdate != null && rtodate != null && rtodate != "")
            {
                DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(rfromdate);
                DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(rtodate);

                query = query.Where(l => l.NextRenewalDate >= dtfrom && l.NextRenewalDate <= dtto).ToList();
            }
            query = query.OrderByDescending(l => l.TrackerId).ToList();
            return query;
        }

        public ActionResult GetRenewalEvents(string start, string end, int LocationId = 0)
        {
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(start, "yyyy-MM-dd");
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(end, "yyyy-MM-dd");
            var events = myapp.tbl_AdministrationTracker.Where(e => (e.NextRenewalDate >= dtfrom && e.NextRenewalDate <= dtto)).ToList();
            if (LocationId > 0)
            {
                events = events.Where(l => l.LocationId.Contains(LocationId.ToString())).ToList();
            }
            var eventList = from e in events
                                //let dts = DateTime.Parse(e.EndDate)
                                //let dte = DateTime.Parse(e.EndTime)
                            select new
                            {
                                id = e.TrackerId,
                                title = e.Title + ' ' + e.Description,
                                //  start = e.EventDate.Value.ToShortDateString() + " " + e.EventTime,
                                start = e.NextRenewalDate.Value.ToString("yyyy-MM-dd") + "T" + "09:00",
                                end = e.NextRenewalDate.Value.ToString("yyyy-MM-dd") + "T" + "14:00",
                                //color = e.EventColor,
                                allDay = false
                                //resources = Data.Entitylist.Single(en => en.CHlId == e.chlid.ToString()).Department
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetExpiredEvents(string start, string end, int LocationId = 0)
        {
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(start, "yyyy-MM-dd");
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(end, "yyyy-MM-dd");
            var events = myapp.tbl_AdministrationTracker.Where(e => (e.EndDate >= dtfrom && e.EndDate <= dtto)).ToList();
            if (LocationId > 0)
            {
                events = events.Where(l => l.LocationId.Contains(LocationId.ToString())).ToList();
            }
            var eventList = from e in events
                                //let dts = DateTime.Parse(e.EndDate)
                                //let dte = DateTime.Parse(e.EndTime)
                            select new
                            {
                                id = e.TrackerId,
                                title = e.FormType + " : " + e.Title + " " + e.Description,
                                //  start = e.EventDate.Value.ToShortDateString() + " " + e.EventTime,
                                start = e.EndDate.Value.ToString("yyyy-MM-dd") + "T" + "09:00",
                                end = e.EndDate.Value.ToString("yyyy-MM-dd") + "T" + "14:00",
                                //color = e.EventColor,
                                allDay = false
                                //resources = Data.Entitylist.Single(en => en.CHlId == e.chlid.ToString()).Department
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetAdminTracker(JQueryDataTableParamModel param)
        {

            List<tbl_AdministrationTracker> query = FilterAdminTracker(param.rfromdate, param.rtodate, param.efromdate, param.etodate, param.sfromdate, param.stodate, param.locationid.ToString(), param.departmentid.ToString(), param.categoryid, param.SubCategoryid, param.status, param.renewal, param.FormType, param.Expired);
            IEnumerable<tbl_AdministrationTracker> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.CategoryId.ToString().ToLower().Contains(param.sSearch.ToLower())
                   ||
                                (from var in myapp.tbl_Location where c.LocationId.Contains(var.LocationId.ToString()) select var.LocationName) != null && (from var in myapp.tbl_Location where var.LocationId.ToString() == c.LocationId select var.LocationName).ToString().ToLower().Contains(param.sSearch.ToLower())
                               // ||
                               // (from var in myapp.tbl_AdminCategory where var.CategoryId == c.CategoryId select var.Name) != null && (from var in myapp.tbl_AdminCategory where var.CategoryId == c.CategoryId select var.Name).ToString().ToLower().Contains(param.sSearch.ToLower())
                               //||
                               // (from var in myapp.tbl_AdminSubCategory where var.SubCategoryId == c.SubCategoryId select var.Name) != null && (from var in myapp.tbl_AdminSubCategory where var.SubCategoryId == c.SubCategoryId select var.Name).ToString().ToLower().Contains(param.sSearch.ToLower())

                               ||
                               (from var in myapp.tbl_CommonDepartment where var.CommonDepartmentId.ToString() == c.DepartmentId select var.Name) != null && (from var in myapp.tbl_CommonDepartment where var.CommonDepartmentId.ToString() == c.DepartmentId select var.Name).ToString().ToLower().Contains(param.sSearch.ToLower())

                                ||
                                c.Title != null && c.Title.ToString().ToLower().Contains(param.sSearch.ToLower())

                               ||
                              c.Description != null && c.Description.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_AdministrationTracker> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           join d in myapp.tbl_CommonDepartment on c.DepartmentId equals d.CommonDepartmentId.ToString()
                                           //  join s in myapp.tbl_AdminSubCategory on c.SubCategoryId equals s.SubCategoryId 
                                           //join l in myapp.tbl_Location on c.LocationId equals l.LocationId.ToString()
                                           //join ca in myapp.tbl_AdminCategory on c.CategoryId equals ca.CategoryId

                                           select new object[] {
                            c.TrackerId.ToString(),
                            string.Join(",", myapp.tbl_Location.Where(l=>c.LocationId.Contains(l.LocationId.ToString())).Select(l=>l.LocationName).ToList()) +" "+d.Name,
                            //ca.Name,
                           // +" " +""+sJ.Name,
                                              c.Title,
                                              c.FormType =="License"? c.FormType+" "+c.LicenseDepartmentName_Gov: c.FormType=="Agreement"?c.FormType+" " +c.AgreementSubType:c.FormType,
                                             // c.Description,
                                              c.StartDate.HasValue?(c.StartDate.Value.ToString("dd/MM/yyyy")):"",
                                              c.EndDate.HasValue?(c.EndDate.Value.ToString("dd/MM/yyyy")):"",
                                             c.NextRenewalDate.HasValue?(c.NextRenewalDate.Value.ToString("dd/MM/yyyy")):"",
                                              c.Status,
                                              c.TrackerId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AdminsavetrackerDocuments(tbl_AdministrationTrackerDocument Doc, HttpPostedFileBase Upload)
        {
            if (Upload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(Upload.FileName);
                string extension = Path.GetExtension(Upload.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;
                Doc.DocumentUrl = fileName;
                Upload.SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));

            }
            Doc.CreatedBy = User.Identity.Name;
            Doc.CreatedOn = DateTime.Now;
            myapp.tbl_AdministrationTrackerDocument.Add(Doc);
            myapp.SaveChanges();
            return Json("Added Successfully", JsonRequestBehavior.AllowGet);
        }
        public JsonResult AdminsavetrackerNotes(tbl_AdministrationTrackerNotes model)
        {
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            myapp.tbl_AdministrationTrackerNotes.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveAdminTracker(AdminTrackerModel model, HttpPostedFileBase Upload, HttpPostedFileBase UserHelpDocument, HttpPostedFileBase[] OtherDocuments)
        {

            if (Upload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(Upload.FileName);
                string extension = Path.GetExtension(Upload.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;
                model.Remarks = fileName;
                Upload.SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));

            }

            tbl_AdministrationTracker tracker = new tbl_AdministrationTracker
            {
                LocationId = model.LocationId,
                DepartmentId = model.DepartmentId,
                CategoryId = 0,
                SubCategoryId = 0,
                PrimaryUserId = model.PrimaryUserId,
                SecondaryUserid = model.SecondaryUserid,
                IsActive = true,
                Title = model.Title,
                FormType = model.FormType,
                StartDate = ProjectConvert.ConverDateStringtoDatetime(model.StartDate),
                EndDate = ProjectConvert.ConverDateStringtoDatetime(model.EndDate),
                NextRenewalDate = ProjectConvert.ConverDateStringtoDatetime(model.NextRenewalDate),
                Remainder1NoOfDays = model.Remainder1NoOfDays,
                Remainder1NoOfDaysNotes = model.Remainder1NoOfDaysNotes,
                Remainder2NoOfDays = model.Remainder2NoOfDays,
                Remainder2NoOfDaysNotes = model.Remainder2NoOfDaysNotes,
                Remainder3NoOfDays = model.Remainder3NoOfDays,
                Remainder3NoOfDaysNotes = model.Remainder3NoOfDaysNotes,
                Remarks = model.Remarks,
                Status = model.Status,
                Description = model.Description,
                CreatedBy = User.Identity.Name,
                CreatedOn = DateTime.Now,
                AgreementSubType = model.AgreementSubType,
                LicenseDepartmentName_Gov = model.LicenseDepartmentName_Gov,
                ModifiedBy = User.Identity.Name,
                ModifiedOn = DateTime.Now
            };
            tracker.PrimaryUserId = model.PrimaryUserId;
            tracker.SecondaryUserid = model.SecondaryUserid;
            myapp.tbl_AdministrationTracker.Add(tracker);
            myapp.SaveChanges();
            if (UserHelpDocument != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(UserHelpDocument.FileName);
                string extension = Path.GetExtension(UserHelpDocument.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;

                UserHelpDocument.SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));
                tbl_AdministrationTrackerDocument modeldec = new tbl_AdministrationTrackerDocument();
                modeldec.DocumentTitle = "User Help Document";
                modeldec.DocumentUrl = fileName;
                modeldec.TrackerId = tracker.TrackerId;
                modeldec.CreatedBy = User.Identity.Name;
                modeldec.CreatedOn = DateTime.Now;
                modeldec.ModifiedBy = User.Identity.Name;
                modeldec.ModifiedOn = DateTime.Now;
                myapp.tbl_AdministrationTrackerDocument.Add(modeldec);
                myapp.SaveChanges();
            }
            if (OtherDocuments != null && OtherDocuments.Count() > 0)
            {
                foreach (var doc in OtherDocuments)
                {
                    string fileName = Path.GetFileNameWithoutExtension(doc.FileName);
                    string extension = Path.GetExtension(doc.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;

                    doc.SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));

                    tbl_AdministrationTrackerDocument modeldec = new tbl_AdministrationTrackerDocument();
                    modeldec.DocumentTitle = doc.FileName;
                    modeldec.DocumentUrl = fileName;
                    modeldec.TrackerId = tracker.TrackerId;
                    modeldec.CreatedBy = User.Identity.Name;
                    modeldec.CreatedOn = DateTime.Now;
                    modeldec.ModifiedBy = User.Identity.Name;
                    modeldec.ModifiedOn = DateTime.Now;
                    myapp.tbl_AdministrationTrackerDocument.Add(modeldec);
                    myapp.SaveChanges();
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAdminTrackerbyid(int TrackerId)
        {
            tbl_AdministrationTracker query = myapp.tbl_AdministrationTracker.Where(l => l.TrackerId == TrackerId).SingleOrDefault();
            string startDate = query.StartDate.HasValue ? query.StartDate.Value.ToString("dd/MM/yyyy") : "";
            string EndDate = query.EndDate.HasValue ? query.EndDate.Value.ToString("dd/MM/yyyy") : "";
            string Renewaldate = query.NextRenewalDate.HasValue ? query.NextRenewalDate.Value.ToString("dd/MM/yyyy") : "";
            var listofdocuments = myapp.tbl_AdministrationTrackerDocument.Where(l => l.TrackerId == TrackerId).ToList();
            var obj = new { query, startDate, EndDate, Renewaldate, listofdocuments };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdateAdminTracker(AdminTrackerModel model)
        {
            List<tbl_AdministrationTracker> cat = myapp.tbl_AdministrationTracker.Where(l => l.TrackerId == model.TrackerId).ToList();
            if (cat != null)
            {
                //cat[0].IsActive = true;
                cat[0].Title = model.Title;
                cat[0].FormType = model.FormType;
                cat[0].StartDate = ProjectConvert.ConverDateStringtoDatetime(model.StartDate);
                cat[0].EndDate = ProjectConvert.ConverDateStringtoDatetime(model.EndDate);
                cat[0].NextRenewalDate = ProjectConvert.ConverDateStringtoDatetime(model.NextRenewalDate);
                cat[0].Remainder1NoOfDays = model.Remainder1NoOfDays;
                cat[0].Remainder1NoOfDaysNotes = model.Remainder1NoOfDaysNotes;
                //cat[0].Remainder2NoOfDays = model.Remainder2NoOfDays;
                //cat[0].Remainder2NoOfDaysNotes = model.Remainder2NoOfDaysNotes;
                //cat[0].Remainder3NoOfDays = model.Remainder3NoOfDays;
                //cat[0].Remainder3NoOfDaysNotes = model.Remainder3NoOfDaysNotes;
                //cat[0].Remarks = model.Remarks;
                cat[0].Description = model.Description;
                cat[0].Status = model.Status;
                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateVendortoadmintracker(tbl_AdministrationTracker model)
        {
            List<tbl_AdministrationTracker> cat = myapp.tbl_AdministrationTracker.Where(l => l.TrackerId == model.TrackerId).ToList();
            cat[0].VendorId = model.VendorId;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAdminTask(int Id)
        {
            var tb = myapp.tbl_AdministrationTracker.Where(a => a.TrackerId == Id).SingleOrDefault();
            myapp.tbl_AdministrationTracker.Remove(tb);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult AdminTrackerhangestatus(tbl_AdministrationTracker model)
        {
            List<tbl_AdministrationTracker> cat = myapp.tbl_AdministrationTracker.Where(l => l.TrackerId == model.TrackerId).ToList();
            if (cat != null)
            {
                cat[0].Status = model.Status;
                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetDocbyTrackId(int trackId)
        {
            var files = from file in myapp.tbl_AdministrationTrackerDocument
                        where file.TrackerId == trackId
                        select new { Did = file.TrackerDocumentId, url = file.DocumentUrl, title = file.DocumentTitle };
            return Json(files, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetNotesbytrackid(int trackId)
        {
            List<tbl_AdministrationTrackerNotes> list = (from notes in myapp.tbl_AdministrationTrackerNotes
                                                         where notes.TrackerId == trackId
                                                         select notes).ToList();
            var files = (from notes in list
                         select new { id = notes.TrackerHistoryId, notes.Remarks, CreatedDate = notes.CreatedOn.Value.ToString("dd/MM/yyyy"), CreatedBy = notes.CreatedBy }).ToList();
            return Json(files, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetDocbySubcategoryid(int Subid)
        {
            var UserHelpfiles = from file in myapp.tbl_AdminSubCategory
                                where file.SubCategoryId == Subid
                                select new { Did = file.SubCategoryId, url = file.UserHelpDocuement, title = "user Help document", type = "" };
            var Files = from file in myapp.tbl_AdminDocument
                        where file.SubCategoryId == Subid
                        select new { Did = file.DocuementId, url = file.DocumentDescription, title = file.DocumentTitle, type = file.DocumentType };
            var result = Enumerable.Concat(UserHelpfiles.AsEnumerable(), Files.AsEnumerable());
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetDocbycategoryid(int Subid)
        {
            var UserHelpfiles = from file in myapp.tbl_AdminCategory
                                where file.CategoryId == Subid
                                select new { Did = file.CategoryId, url = file.UserHelpdocument, title = "user Help document", type = "" };
            var Files = from file in myapp.tbl_AdminDocument
                        where file.CategoryId == Subid
                        select new { Did = file.DocuementId, url = file.DocumentDescription, title = file.DocumentTitle, type = file.DocumentType };
            var result = Enumerable.Concat(UserHelpfiles.AsEnumerable(), Files.AsEnumerable());
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public JsonResult SendRemaindersToUser()
        {
            List<tbl_AdministrationTracker> Tracker = myapp.tbl_AdministrationTracker.Where(l => l.IsActive == true && l.Status != "Not using").ToList();
            var locations = myapp.tbl_Location.ToList();
            var department = myapp.tbl_CommonDepartment.ToList();
            string message = "Success";
            for (int i = 0; i < Tracker.Count; i++)
            {
                try
                {
                    string jobname = "Administration_" + Tracker[i].TrackerId;
                    DateTime dt = DateTime.Now.Date;
                    var list = myapp.tbl_JobsHistory.Where(t1 => t1.JobName == jobname).ToList();
                    list = list.Where(t1 => t1.JobExecutedDate.Value.Date == dt).ToList();
                    if (list.Count == 0)
                    {
                        tbl_JobsHistory tbl = new tbl_JobsHistory
                        {
                            Environment = "PROD",
                            JobExcutedHour = DateTime.Now.Hour,
                            JobExcutedMinute = DateTime.Now.Minute,
                            JobExecutedDate = DateTime.Now,
                            JobName = jobname,
                            JobStatus = true,
                            Message = "Success"
                        };
                        myapp.tbl_JobsHistory.Add(tbl);
                        myapp.SaveChanges();
                        string subject = string.Join(",", locations.Where(l => Tracker[i].LocationId.Contains(Convert.ToString(l.LocationId))).Select(l => l.LocationName).ToList());
                        subject += "-" + department.Where(l => Convert.ToString(l.CommonDepartmentId) == Tracker[0].DepartmentId).Select(l => l.Name).FirstOrDefault();
                        subject += "-" + Tracker[i].Title;
                        CustomModel cm = new CustomModel();
                        MailModel mailmodel = new MailModel();
                        EmailTeamplates emailtemp = new EmailTeamplates();
                        mailmodel.fromemail = "Leave@hospitals.com";
                        // mailmodel.toemail = (from var in myapp.tbl_User where var.CustomUserId==Tracker[i].PrimaryUserId select var.EmailId).ToString();
                        mailmodel.toemail = "vasavi_m@fernandez.foundation";
                        mailmodel.ccemail = "ahmadali@fernandez.foundation";
                        mailmodel.subject = "Reminder for " + subject + "";
                        string mailbody = "<p style='font-family:verdana'>HI Team,";
                        //mailbody += " Tracker is going to expire/expired on " + Tracker[i].EndDate;
                        //mailbody += "  Remainder:" + Tracker[i].Remainder1NoOfDaysNotes;
                        mailbody += "<p style='font-family:verdana'>" + Tracker[i].Title + " Tracker is going to expire/expired on " + Tracker[i].EndDate + ".</p>";
                        mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                        mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>FormType</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Tracker[i].FormType + "</td></tr>";
                        mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Title</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Tracker[i].Title + "</td></tr>";
                        mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Description</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Tracker[i].Description + "</td></tr>";
                        mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Start Date </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Tracker[i].StartDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                        mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>End Date</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Tracker[i].EndDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                        mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Next Renewal Date</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Tracker[i].NextRenewalDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                        //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Category</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Tracker[i].CategoryId + "</td></tr>";
                        mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Status</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Tracker[i].Status + "</td></tr>";
                        if (Tracker[i].FormType == "License")
                        {
                            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Gov Department</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Tracker[i].LicenseDepartmentName_Gov + "</td></tr>";
                        }
                        if (Tracker[i].FormType == "Agreement")
                        {
                            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Agreement SubTypet</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Tracker[i].AgreementSubType + "</td></tr>";
                        }
                        mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Tracker Id</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Tracker[i].TrackerId + "</td></tr>";
                        //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>vendor</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Tracker[i].VendorId + "</td></tr>";
                        // mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Contact No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.Mobile + "</td></tr>";
                        mailbody += "</table>";
                        mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
                        mailmodel.body = mailbody;
                        mailmodel.fromname = "Reminder for Tracker";
                        DateTime RemainderDate = Tracker[i].EndDate.Value;
                        if (Tracker[i].Remainder1NoOfDays != null)
                        {
                            RemainderDate = RemainderDate.AddDays(-Tracker[i].Remainder1NoOfDays.Value);
                        }

                        if (Tracker[i].EndDate <= DateTime.Now)
                        {
                            mailmodel.ccemail = mailmodel.ccemail + ",chandrika.p@fernandez.foundation,meena.s@fernandez.foundation";
                        }
                        if (Tracker[i].EndDate <= DateTime.Now || RemainderDate <= DateTime.Now)
                        {
                            cm.SendEmail(mailmodel);
                        }
                        //if (Tracker[i].Remainder1NoOfDays != null && Tracker[i].EndDate.Value.ToString("dd/MM/yyyy") == DateTime.Now.AddDays(Convert.ToDouble(Tracker[i].Remainder1NoOfDays)).ToString("dd/MM/yyyy"))
                        //{
                        //    Remaindermessage = Tracker[i].Remainder1NoOfDaysNotes;
                        //    cm.SendEmail(mailmodel);
                        //}
                        //else if (Tracker[i].Remainder2NoOfDays != null && Tracker[i].EndDate.Value.ToString("dd/MM/yyyy") == DateTime.Now.AddDays(Convert.ToDouble(Tracker[i].Remainder2NoOfDays)).ToString("dd/MM/yyyy"))
                        //{
                        //    Remaindermessage = Tracker[i].Remainder2NoOfDaysNotes;
                        //    cm.SendEmail(mailmodel);
                        //}
                        //else if (Tracker[i].Remainder3NoOfDays != null && Tracker[i].EndDate.Value.ToString("dd/MM/yyyy") == DateTime.Now.AddDays(Convert.ToDouble(Tracker[i].Remainder3NoOfDays)).ToString("dd/MM/yyyy"))
                        //{
                        //    Remaindermessage = Tracker[i].Remainder3NoOfDaysNotes;
                        //    cm.SendEmail(mailmodel);
                        //}
                    }
                }
                catch (Exception ex)
                {
                    message = message + ex.Message;
                }
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetUserEmailidsOfRequest(int requestId)
        {
            string emailids = "";
            var request = myapp.tbl_AdministrationTracker.Where(l => l.TrackerId == requestId).SingleOrDefault();
            if (request != null)
            {
                if (request.PrimaryUserId != null && request.PrimaryUserId != "")
                {
                    emailids = myapp.tbl_User.Where(l => l.CustomUserId == request.PrimaryUserId).SingleOrDefault().EmailId;
                    if (emailids != null && emailids != "")
                    {
                        emailids = emailids + ",";
                    }
                }
                if (request.SecondaryUserid != null && request.SecondaryUserid != "")
                {
                    emailids += myapp.tbl_User.Where(l => l.CustomUserId == request.SecondaryUserid).SingleOrDefault().EmailId;
                }
            }
            return Json(emailids, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendEmailToOwner(int requestId, string RequestOwner, string MailIds, string comments)
        {
            var request = myapp.tbl_AdministrationTracker.Where(l => l.TrackerId == requestId).SingleOrDefault();
            if (request != null)
            {
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Trackerrequest@hospitals.com";
                // mailmodel.toemail = (from var in myapp.tbl_User where var.CustomUserId==request.PrimaryUserId select var.EmailId).ToString();
                mailmodel.toemail = RequestOwner.Contains(",") ? RequestOwner.Split(',')[0] : RequestOwner;
                mailmodel.ccemail = (RequestOwner.Contains(",") ? RequestOwner.Split(',')[1] + "," : "") + MailIds;
                mailmodel.subject = "Request update for Tracker " + request.TrackerId + "";
                string mailbody = "<p style='font-family:verdana'>HI Team,";
                //mailbody += "Tracker is expired on " + request.EndDate;
                //mailbody += "Remainder:" + request.Remainder1NoOfDaysNotes;
                //mailbody += "<p style='font-family:verdana'>" + request.Title + " Tracker is expired on " + request.EndDate + ".</p>";
                mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>FormType</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.FormType + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Description</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.Description + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Start Date </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.StartDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'> End Date</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.EndDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'> Next Renewal Date</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.NextRenewalDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Category</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.CategoryId + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Status</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.Status + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Comments</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + comments + "</td></tr>";
                // mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Contact No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.Mobile + "</td></tr>";
                mailbody += "</table>";
                mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
                mailmodel.body = mailbody;
                mailmodel.fromname = "Reminder for Tracker";
                cm.SendEmail(mailmodel);
            }
            return Json("Email Sent Successfully", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateTrackerRequest(int requestId, string startdate, string todate, string nextrenewaldate, string status, string comments)
        {
            var request = myapp.tbl_AdministrationTracker.Where(l => l.TrackerId == requestId).SingleOrDefault();
            if (request != null)
            {
                request.StartDate = ProjectConvert.ConverDateStringtoDatetime(startdate);
                request.EndDate = ProjectConvert.ConverDateStringtoDatetime(todate);
                request.NextRenewalDate = ProjectConvert.ConverDateStringtoDatetime(nextrenewaldate);
                request.Status = status;
                request.Remarks = comments;
                myapp.SaveChanges();
                tbl_AdministrationTrackerNotes notes = new tbl_AdministrationTrackerNotes();
                notes.CreatedBy = User.Identity.Name;
                notes.CreatedOn = DateTime.Now;
                notes.HistoryType = "Renewal";
                notes.ModifiedBy = User.Identity.Name;
                notes.ModifiedOn = DateTime.Now;
                notes.Remarks = "Renewal Comments - " + comments;
                notes.TrackerId = requestId;
                myapp.tbl_AdministrationTrackerNotes.Add(notes);
                myapp.SaveChanges();
            }

            return Json("Request Updated successfully", JsonRequestBehavior.AllowGet);
        }
    }
}