using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.DataModelRegister;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    public class LearningController : Controller
    {
        private MyIntranetApp_RegisterEntities myappregister = new MyIntranetApp_RegisterEntities();
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Learning
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ManageCourse()
        {
            return View();
        }
        public ActionResult ManageAllCourseTrainee(int courceId)
        {
            ViewBag.Id = courceId;
            return View();
        }
        [AllowAnonymous]
        public ActionResult GetTrainingsSearch(string searchTerm)
        {
            var query = myappregister.tbl_Lms_Course.Where(l => l.IsActive == true).ToList();
            query = query
                    .Where(c => c.CourseId.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                                 c.CourseCode != null && c.CourseCode.ToLower().Contains(searchTerm.ToLower())
                                ||
                               c.CourseName != null && c.CourseName.ToLower().Contains(searchTerm.ToLower())
                                ||
                               c.Description != null && c.Description.ToLower().Contains(searchTerm.ToLower())

                                 ||
                               c.DateOfCourse != null && c.DateOfCourse.Value.ToString("dd/MM/yyyy").ToLower().Contains(searchTerm.ToLower())
                               ||
                               c.Remarks != null && c.Remarks.ToLower().Contains(searchTerm.ToLower())
                               ).ToList();
            var resulst = (from q in query
                           select new
                           {
                               id = q.CourseId,
                               text = q.CourseCode + " " + q.CourseName
                           }).ToList();
            return Json(resulst, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult GetCourcesSearch(string dept, string searchTerm)
        {
            //var query = myappregister.tbl_Lms_Course.Where(l => l.IsActive == true).ToList();

            var PathName = Path.Combine(Server.MapPath("~/Documents/"), "Department_Wise_Training_Calendar_2021.xlsx");

            string myexceldataquery = "select * from [" + dept + "$]";
            //string excelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathName + ";Extended Properties=Excel 12.0;";
            string sexcelconnectionstring = "";
            if (PathName.Contains(".xlsx"))
            {
                sexcelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathName + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
            }
            else
            {
                sexcelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathName + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
            }
            OleDbConnection oledbconn = new OleDbConnection(sexcelconnectionstring);
            OleDbCommand oledbcmd = new OleDbCommand(myexceldataquery, oledbconn);
            OleDbDataAdapter da = new OleDbDataAdapter(oledbcmd);
            oledbconn.Open();
            DataSet ds = new DataSet();
            da.Fill(ds);
            //return Json(ds.Tables[0], JsonRequestBehavior.AllowGet);
            oledbconn.Close();
            oledbconn.Dispose();

            //var list = JsonConvert.SerializeObject(ds.Tables[0], Formatting.None, new JsonSerializerSettings()
            //{
            //    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            //});
            var myData = ds.Tables[0].AsEnumerable().Select(r => new
            {
                V = r.ItemArray[1]
            }).ToList();

            myData = myData
                .Where(c => c.V.ToString().ToLower().Contains(searchTerm.ToLower())).ToList();
            var resulst = (from q in myData
                           select new
                           {
                               id = q.V,
                               text = q.V
                           }).ToList();
            return Json(resulst, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NewCourse(int id = 0)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult SaveNewCourse(CourseViewModel model)
        {
            if (model.CourseId > 0)
            {
                var dbmodel = myappregister.tbl_Lms_Course.Where(l => l.CourseId == model.CourseId).SingleOrDefault();
                dbmodel.CourseCode = model.CourseCode;
                dbmodel.CourseName = model.CourseName;
                dbmodel.Description = model.Description;
                dbmodel.IsGlobal = model.IsGlobal;
                dbmodel.LocationId = model.LocationId;
                dbmodel.Rating = model.Rating;
                dbmodel.Remarks = model.Remarks;
                dbmodel.ModifiedBy = User.Identity.Name;
                dbmodel.ModifiedOn = DateTime.Now;
                dbmodel.Status = model.Status;
                dbmodel.DateOfCourse = ProjectConvert.ConverDateStringtoDatetime(model.DateOfCourse);
                myappregister.SaveChanges();
            }
            else
            {
                tbl_Lms_Course dbmodel = new tbl_Lms_Course();
                dbmodel.DateOfCourse = ProjectConvert.ConverDateStringtoDatetime(model.DateOfCourse);
                dbmodel.CourseCode = model.CourseCode;
                dbmodel.CourseName = model.CourseName;
                dbmodel.Description = model.Description;
                dbmodel.IsGlobal = model.IsGlobal;
                dbmodel.LocationId = model.LocationId;
                dbmodel.Rating = model.Rating;
                dbmodel.Remarks = model.Remarks;
                dbmodel.CreatedBy = User.Identity.Name;
                dbmodel.CreatedOn = DateTime.Now;
                dbmodel.IsActive = true;
                dbmodel.Status = model.Status;
                dbmodel.ModifiedBy = User.Identity.Name;
                dbmodel.ModifiedOn = DateTime.Now;
                myappregister.tbl_Lms_Course.Add(dbmodel);
                myappregister.SaveChanges();
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveNewCourceEmployee(tbl_Lms_CourceTrainee model)
        {

            model.CreatedBy = User.Identity.Name;
            string TraineeSign = model.TraineeSign;
            model.TraineeSign = "";
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            model.AttendedOn = DateTime.Now;
            myappregister.tbl_Lms_CourceTrainee.Add(model);
            myappregister.SaveChanges();
            if (TraineeSign != null && TraineeSign != "")
            {
                try
                {
                    TraineeSign = TraineeSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(TraineeSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_CourceTrainee_Signature_" + model.CourseId + "_" + model.TraineeEmpId + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.TraineeSign = "Register_CourceTrainee_Signature_" + model.CourseId + "_" + model.TraineeEmpId + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetCourceTraineeDetailsById(string CourseName)
        {
            var query = myappregister.tbl_Lms_CourceTrainee.Where(l => l.CourseName == CourseName).ToList();
            var model = (from q in query
                             //join c in myappregister.tbl_Lms_Course on q.CourseId equals c.CourseId
                         join u in myapp.tbl_User on q.TraineeEmpId equals u.EmpId
                         select new CourceTraineeViewModel
                         {
                             CourseCode = "",
                             CourseId = 0,
                             CourseName = q.CourseName,
                             DateOfCourse = q.AttendedOn.HasValue ? q.AttendedOn.Value.ToString("dd/MM/yyyy") : "",
                             EmployeeId = u.EmpId.Value,
                             EmployeeName = u.FirstName,
                             EmployeeSign = q.TraineeSign,
                             Department = q.Department
                         }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRegister_Course(int id)
        {
            var query = myappregister.tbl_Lms_Course.Where(l => l.CourseId == id).SingleOrDefault();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_Course(int id)
        {
            var query = myappregister.tbl_Lms_Course.Where(l => l.CourseId == id).SingleOrDefault();
            myappregister.tbl_Lms_Course.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCourse(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Lms_Course select d).OrderByDescending(l => l.CourseId).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Lms_Course> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.CourseId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.CourseCode != null && c.CourseCode.ToString().Contains(param.sSearch.ToLower())
                                 ||
                                c.CourseName != null && c.CourseName.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.Description != null && c.Description.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.Remarks != null && c.Remarks.ToLower().Contains(param.sSearch.ToLower())

                                ||
                                c.Rating != null && c.Rating.Value.ToString().ToLower().Contains(param.sSearch.ToLower())
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
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId

                         select new[] {
                                              c.CourseId.ToString(),
                                               l.LocationName,
                                               c.Status,
                                            c.CourseCode,
                                            c.CourseName,
                                              c.Description,
                                              c.DateOfCourse!=null?c.DateOfCourse.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Remarks,
                                              //c.Rating!=null?c.Rating.Value.ToString():"0",
                                              u.FirstName,
                                              c.ModifiedOn!=null?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.CourseId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportAllAttendees(int CourceId = 0)
        {
            var list = (from l in myappregister.tbl_Lms_CourceTrainee where l.CourseId == CourceId select l).ToList();

            var query = (from q in list
                         join c in myappregister.tbl_Lms_Course on q.CourseId equals c.CourseId
                         join u in myapp.tbl_User on q.TraineeEmpId equals u.EmpId
                         select new CourceTraineeViewModel
                         {
                             CourseCode = "",
                             CourseId = 0,
                             CourseName = q.CourseName,
                             DateOfCourse = q.AttendedOn.HasValue ? q.AttendedOn.Value.ToString("dd/MM/yyyy HH:mm tt") : "",
                             EmployeeId = u.EmpId.Value,
                             EmployeeName = u.FirstName,
                             EmployeeSign = q.TraineeSign,
                             TraineerName = c.Description,
                             Mode = q.Mode,
                             Remarks = q.Remarks
                         }).ToList();
            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Trainer", typeof(string));
            products.Columns.Add("Topic", typeof(string));
            products.Columns.Add("Employee Number", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Attended On", typeof(string));
            products.Columns.Add("Mode", typeof(string));
          
            products.Columns.Add("Remarks", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                     c.TraineerName,
                                            c.CourseName,
                                              c.EmployeeId.ToString(),
                                              c.EmployeeName,
                                              c.DateOfCourse != null ? c.DateOfCourse : "",
                                              c.Mode,
                                              c.Remarks

                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=OccuranceRegister.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);

        }
        public ActionResult AjaxGetCourseVsTrainee(JQueryDataTableParamModel param)
        {
            var list = (from l in myappregister.tbl_Lms_CourceTrainee select l).ToList();
            if (param.locationid != null && param.locationid > 0)
            {
                list = list.Where(l => l.CourseId == param.locationid).ToList();
            }
            var query = (from q in list
                         join c in myappregister.tbl_Lms_Course on q.CourseId equals c.CourseId
                         join u in myapp.tbl_User on q.TraineeEmpId equals u.EmpId
                         select new CourceTraineeViewModel
                         {
                             CourseCode = "",
                             CourseId = 0,
                             CourseName = q.CourseName,
                             DateOfCourse = q.AttendedOn.HasValue ? q.AttendedOn.Value.ToString("dd/MM/yyyy HH:mm tt") : "",
                             EmployeeId = u.EmpId.Value,
                             EmployeeName = u.FirstName,
                             EmployeeSign = q.TraineeSign,
                             TraineerName = c.Description,

                         }).ToList();


            //if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            //{
            //    var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
            //    var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
            //    query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            //}
            IEnumerable<CourceTraineeViewModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.CourseId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.CourseCode != null && c.CourseCode.ToString().Contains(param.sSearch.ToLower())
                                 ||
                                c.CourseName != null && c.CourseName.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.EmployeeName != null && c.EmployeeName.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.EmployeeId != null && c.EmployeeId.ToString().Contains(param.sSearch.ToLower())

                                ||
                                c.DateOfCourse != null && c.DateOfCourse.Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                                            c.TraineerName,
                                            c.CourseName,
                                              c.EmployeeId.ToString(),
                                              c.EmployeeName,
                                              c.DateOfCourse!=null?c.DateOfCourse:"",
                                              c.EmployeeSign};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
    }
}