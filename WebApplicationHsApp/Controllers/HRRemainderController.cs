using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class HRRemainderController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: HRRemainder
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            DateTime dttoday = DateTime.Now;
            ViewBag.TotalRemainders = myapp.tbl_Hr_Remainder.Where(l => l.IsActive == true).Count();
            ViewBag.TotalRemaindersActive = myapp.tbl_Hr_Remainder.Where(l => l.IsActive == true && l.NextRenewalDate >= dttoday).Count();
            ViewBag.TotalRemaindersExipred = myapp.tbl_Hr_Remainder.Where(l => l.IsActive == true && l.NextRenewalDate < dttoday).Count();
            ViewBag.TotalRemaindersSent = myapp.tbl_Hr_RemainderLog.Count();
            return View();
        }
        public ActionResult ManageRemainderType()
        {
            return View();
        }
        public JsonResult GetRemaindersCountByType()
        {
            var list = (from r in myapp.tbl_Hr_Remainder
                        join rt in myapp.tbl_Hr_RemainderType on r.RemainderTypeId equals rt.RemainderTypeId
                        group rt by rt.RemainderType into g
                        select new
                        {
                            Key = g.Key,
                            Value = g.Count()
                        }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRemaindersCountByStatus()
        {
            var list = (from r in myapp.tbl_Hr_Remainder
                        group r by r.Status into g
                        select new
                        {
                            Key = g.Key,
                            Value = g.Count()
                        }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageLogs()
        {
            return View();
        }
        public JsonResult SaveRemainder(HrRemainderViewModel model)
        {
            tbl_Hr_Remainder dbmodel = new tbl_Hr_Remainder();
            //dbmodel.CollageName = model.CollageName;
            dbmodel.CreatedBy = User.Identity.Name;
            dbmodel.CreatedOn = DateTime.Now;
            dbmodel.IsCommon = model.IsCommon;
            if (model.EmpId != 0)
            {
                var user = myapp.tbl_User.Where(l => l.UserId == model.EmpId).SingleOrDefault();
                dbmodel.EmpId = user.EmpId;
                dbmodel.FirstRegistrationCouncil = model.FirstRegistrationCouncil;
                dbmodel.NursingCouncil = model.NursingCouncil;
                dbmodel.RegistrationNumber = model.RegistrationNumber;
            }
            else
            {
                dbmodel.EmpId = 0;
            }
            dbmodel.IsActive = true;
            dbmodel.LastRenewalDate = ProjectConvert.ConverDateStringtoDatetime(model.LastRenewalDate);
            dbmodel.ModifiedBy = User.Identity.Name;
            dbmodel.ModifiedOn = DateTime.Now;
            dbmodel.NextRenewalDate = ProjectConvert.ConverDateStringtoDatetime(model.NextRenewalDate);

            dbmodel.RemainderTypeId = model.RemainderTypeId;
            dbmodel.Remarks = model.Remarks;
            dbmodel.Status = model.Status;
            dbmodel.Title = model.Title;
            myapp.tbl_Hr_Remainder.Add(dbmodel);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetRemainder(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_Hr_Remainder select d).OrderByDescending(l => l.RemainderId).ToList();
            IEnumerable<tbl_Hr_Remainder> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.RemainderTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Title != null && c.Title.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join r in myapp.tbl_Hr_RemainderType on c.RemainderTypeId equals r.RemainderTypeId
                         //join u in myapp.tbl_User on c.EmpId equals u.EmpId
                         let user = myapp.tbl_User.Where(l => l.EmpId == c.EmpId).FirstOrDefault()
                         //let FirstName = (c.EmpId == null || c.EmpId == 0) ? "" : (myapp.tbl_User.Where(l => l.EmpId == c.EmpId).SingleOrDefault() != null ? myapp.tbl_User.Where(l => l.EmpId == c.EmpId).SingleOrDefault().FirstName : "")
                         //let CollegeName = (c.EmpId == null || c.EmpId == 0) ? "" : (myapp.tbl_User.Where(l => l.EmpId == c.EmpId).SingleOrDefault() != null ? myapp.tbl_User.Where(l => l.EmpId == c.EmpId).SingleOrDefault().CollegeName : "")
                         select new[] {
                                              c.RemainderId.ToString(),
                                              //c.Title,
                                             r.RemainderType,
                                              user!=null?user.FirstName:c.Title,
                                               user!=null?user.CollegeName:"",
                                               user!=null && user.DateOfJoining!=null?user.DateOfJoining.Value.ToString("dd/MM/yyyy"):"",
                                             

                                              c.NursingCouncil,
                                              c.FirstRegistrationCouncil,
                                              c.RegistrationNumber,
                                              c.LastRenewalDate.HasValue?c.LastRenewalDate.Value.ToString("dd/MM/yyyy"):"",
                                              c.NextRenewalDate.HasValue?c.NextRenewalDate.Value.ToString("dd/MM/yyyy"):"",
                                              c.Status,
                                              c.Remarks,
                                              c.IsActive == true?"Yes":"No",
                                              c.RemainderId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult UploadFilesForRemainder()
        {
            if (Request.Form["id"] != null && Request.Form["id"] != "")
            {
                string Id = Request.Form["id"].ToString();
                string Notes = Request.Form["Notes"].ToString();
                string RenewalDate = Request.Form["RenewalDate"].ToString();
                string NextRenewalDate = Request.Form["NextRenewalDate"].ToString();
                if (Request.Files.Count > 0)
                {
                    try
                    {
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            string fname;
                            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                            }
                            else
                            {
                                fname = file.FileName;
                            }
                            string fileName = Path.GetFileName(file.FileName);
                            string guidid = Guid.NewGuid().ToString();
                            string path = Path.Combine(Server.MapPath("~/ExcelUplodes/"), guidid + fileName);
                            file.SaveAs(path);
                            tbl_Hr_RemainderAction tsk = new tbl_Hr_RemainderAction
                            {
                                CreatedBy = User.Identity.Name,
                                CreatedOn = DateTime.Now,
                                DocumentUrl = guidid + fileName,
                                Notes = Notes,
                                RenewalDate = ProjectConvert.ConverDateStringtoDatetime(RenewalDate),
                                RemainderId = int.Parse(Id)
                            };
                            myapp.tbl_Hr_RemainderAction.Add(tsk);
                            myapp.SaveChanges();
                            var remainder = myapp.tbl_Hr_Remainder.Where(l => l.RemainderId == tsk.RemainderId).SingleOrDefault();
                            remainder.LastRenewalDate = tsk.RenewalDate;
                            remainder.NextRenewalDate = ProjectConvert.ConverDateStringtoDatetime(NextRenewalDate);
                            myapp.SaveChanges();
                        }
                        // Returns message that successfully uploaded  
                        return Json("File Uploaded Successfully!");
                    }
                    catch (Exception ex)
                    {
                        return Json("Error occurred. Error details: " + ex.Message);
                    }
                }
                else
                {
                    return Json("No files selected.");
                }
            }
            else
            {
                return Json("Please select Id");
            }
        }


        [HttpPost]
        public ActionResult UploadFilesForRemainderImportExcel()
        {

            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    for (int i = 0; i < files.Count; i++)
                    {
                        HttpPostedFileBase file = files[i];
                        string fname;
                        if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                        {
                            string[] testfiles = file.FileName.Split(new char[] { '\\' });
                            fname = testfiles[testfiles.Length - 1];
                        }
                        else
                        {
                            fname = file.FileName;
                        }
                        string fileName = Path.GetFileName(file.FileName);
                        string guidid = Guid.NewGuid().ToString();
                        string path = Path.Combine(Server.MapPath("~/ExcelUplodes/"), guidid + fileName);
                        file.SaveAs(path);

                        string myexceldataquery = "select Title,RemainderType,EmployeeId,CollageName,NursingCouncil,FirstRegistrationCouncil,RegistrationNumber,LastRenewalDate,NextRenewalDate,Status,Notes from [sheet1$]";
                        //string excelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathName + ";Extended Properties=Excel 12.0;";
                        string sexcelconnectionstring = "";
                        if (path.Contains(".xlsx"))
                        {
                            sexcelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
                        }
                        else
                        {
                            sexcelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
                        }
                        OleDbConnection oledbconn = new OleDbConnection(sexcelconnectionstring);
                        OleDbCommand oledbcmd = new OleDbCommand(myexceldataquery, oledbconn);
                        oledbconn.Open();
                        OleDbDataReader DR = oledbcmd.ExecuteReader();
                        var remaindertypes = myapp.tbl_Hr_RemainderType.ToList();
                        while (DR.Read())
                        {
                            string Title = Convert.ToString(DR["Title"]);
                            int remaindertypeid = 0;
                            string RemainderType = Convert.ToString(DR["RemainderType"]);
                            string EmployeeId = Convert.ToString(DR["EmployeeId"]);
                            string CollageName = Convert.ToString(DR["CollageName"]);
                            string NursingCouncil = Convert.ToString(DR["NursingCouncil"]);
                            string FirstRegistrationCouncil = Convert.ToString(DR["FirstRegistrationCouncil"]);
                            string RegistrationNumber = Convert.ToString(DR["RegistrationNumber"]);
                            DateTime LastRenewalDate = Convert.ToDateTime(DR["LastRenewalDate"]);
                            DateTime NextRenewalDate = Convert.ToDateTime(DR["NextRenewalDate"]);
                            string Status = Convert.ToString(DR["Status"]);
                            string Notes = Convert.ToString(DR["Notes"]);
                            if (RemainderType != null && RemainderType != "")
                            {
                                var rtype = remaindertypes.Where(l => l.RemainderType == RemainderType).SingleOrDefault();
                                if (rtype != null)
                                {
                                    remaindertypeid = rtype.RemainderTypeId;
                                }
                            }
                            tbl_Hr_Remainder r = new tbl_Hr_Remainder()
                            {
                                //CollageName = CollageName,
                                Status = Status,
                                Remarks = Notes,
                                CreatedBy = User.Identity.Name,
                                CreatedOn = DateTime.Now,
                                EmpId = int.Parse(EmployeeId),
                                FirstRegistrationCouncil = FirstRegistrationCouncil,
                                IsActive = true,
                                LastRenewalDate = LastRenewalDate,
                                ModifiedBy = User.Identity.Name,
                                ModifiedOn = DateTime.Now,
                                NextRenewalDate = NextRenewalDate,
                                NursingCouncil = NursingCouncil,
                                RegistrationNumber = RegistrationNumber,
                                RemainderTypeId = remaindertypeid,
                                RemainderId = 0,
                                Title = Title
                            };
                            myapp.tbl_Hr_Remainder.Add(r);
                            myapp.SaveChanges();
                        }
                        oledbconn.Close();
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }
                    // Returns message that successfully uploaded  
                    return Json("File Uploaded Successfully!");
                }
                catch (Exception ex)
                {
                    return Json("Error occurred. Error details: " + ex.Message);
                }
            }
            else
            {
                return Json("No files selected.");
            }

        }

        public JsonResult GetListOfFiles(int id)
        {
            List<tbl_Hr_RemainderAction> list = myapp.tbl_Hr_RemainderAction.Where(l => l.RemainderId == id).ToList();
            var model = (from l in list
                         let RenewalDate = l.RenewalDate.HasValue ? l.RenewalDate.Value.ToString("dd/MM/yyyy") : ""
                         select new
                         {
                             l.Id,
                             l.RemainderId,
                             l.Notes,
                             l.DocumentUrl,
                             RenewalDate
                         }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateStatus(int id, string status)
        {
            var model = myapp.tbl_Hr_Remainder.Where(l => l.RemainderId == id).SingleOrDefault();
            model.Status = status;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult SendRemainderEmail()
        {
            var remainders = myapp.tbl_Hr_Remainder.ToList();
            var rtype = myapp.tbl_Hr_RemainderType.ToList();
            var aremainder = (from r in remainders
                              join rt in rtype on r.RemainderTypeId equals rt.RemainderTypeId
                              //join u in myapp.tbl_User on r.EmpId equals u.EmpId                             
                              where r.IsActive == true
                              let u = (r.EmpId != null && r.EmpId != 0) ? myapp.tbl_User.Where(l => l.EmpId == r.EmpId).FirstOrDefault() : new tbl_User()
                              let FirstName = (r.EmpId != null && r.EmpId != 0 && u!=null) ? u.FirstName : ""
                              let LocationName = (r.EmpId != null && r.EmpId != 0 && u != null) ? u.LocationName : ""
                              let DepartmentName = (r.EmpId != null && r.EmpId != 0 && u != null) ? u.DepartmentName : ""
                              let Designation = (r.EmpId != null && r.EmpId != 0 && u != null) ? u.Designation : ""
                              let Qualification = (r.EmpId != null && r.EmpId != 0 && u != null) ? u.Qualification : ""
                              let CollegeName = (r.EmpId != null && r.EmpId != 0 && u != null) ? u.CollegeName : ""
                              let ReportingManagerId = (r.EmpId != null && r.EmpId != 0 && u != null) ? u.ReportingManagerId : 0
                              let EmailId = (r.EmpId != null && r.EmpId != 0 && u != null) ? u.EmailId : ""
                              let IsActive = (r.EmpId != null && r.EmpId != 0 && u != null) ? u.IsActive : false
                              let DateOfJoining = (r.EmpId != null && r.EmpId != 0 && u != null) ? u.DateOfJoining : DateTime.MinValue
                              select new
                              {
                                  r.RemainderId,
                                  r.EmpId,
                                  FirstName,
                                  LocationName,
                                  DepartmentName,
                                  Designation,
                                  Qualification,
                                  CollegeName,
                                  ReportingManagerId,
                                  EmailId,
                                  r.FirstRegistrationCouncil,
                                  r.LastRenewalDate,
                                  r.NextRenewalDate,
                                  r.NursingCouncil,
                                  r.RegistrationNumber,
                                  r.RemainderTypeId,
                                  r.Remarks,
                                  r.Title,
                                  r.Status,
                                  rt.RemainderNoOfDaysNotes,
                                  rt.RemainderRepeatType,
                                  rt.RemainderType,
                                  rt.RemainderSentToEmail,
                                  rt.RemainderNoOfDays,
                                  rt.RemainderSentToEmployee,
                                  rt.RemainderSentToHOD,
                                  rt.RemainderNoOfDays1,
                                  rt.RemainderSentToEmployee1,
                                  rt.RemainderSentToHOD1,
                                  rt.RemainderNoOfDays2,
                                  rt.RemainderSentToEmployee2,
                                  rt.RemainderSentToHOD2,
                                  rt.RemainderNoOfDays3,
                                  rt.RemainderSentToEmployee3,
                                  rt.RemainderSentToHOD3,
                                  rt.RemainderSentType,
                                  IsActive,
                                  DateOfJoining
                              }).ToList();
            foreach (var r in aremainder)
            {
                try
                {
                    if (r.NextRenewalDate != null && r.RemainderNoOfDays != null && r.IsActive == true)
                    {
                        bool RemainderSentToEmployee = false, RemainderSentToHOD = false, CheckforRemainder = false;
                        var datedifferent = r.NextRenewalDate.Value - DateTime.Now;
                        int totaldays = int.Parse(datedifferent.TotalDays.ToString("0"));
                        if (totaldays > 0)
                        {
                            if (r.RemainderType == "Privileging")
                            {
                                datedifferent = DateTime.Now - r.DateOfJoining.Value;
                                totaldays = int.Parse(datedifferent.TotalDays.ToString("0"));
                                if (DateTime.Now.Day == r.DateOfJoining.Value.Day && DateTime.Now.Month == r.DateOfJoining.Value.Month)
                                {
                                    CheckforRemainder = true;
                                }
                                else if (totaldays == 30)
                                {
                                    CheckforRemainder = true;
                                }
                                else if (totaldays == 180)
                                {
                                    CheckforRemainder = true;
                                }
                            }
                            else
                            {
                                if (totaldays == r.RemainderNoOfDays.Value)
                                {
                                    RemainderSentToEmployee = r.RemainderSentToEmployee.HasValue ? r.RemainderSentToEmployee.Value : false;
                                    RemainderSentToHOD = r.RemainderSentToHOD.HasValue ? r.RemainderSentToHOD.Value : false;
                                    CheckforRemainder = true;
                                }
                                else if (totaldays == r.RemainderNoOfDays1.Value)
                                {
                                    RemainderSentToEmployee = r.RemainderSentToEmployee1.HasValue ? r.RemainderSentToEmployee1.Value : false;
                                    RemainderSentToHOD = r.RemainderSentToHOD1.HasValue ? r.RemainderSentToHOD1.Value : false;
                                    CheckforRemainder = true;
                                }
                                else if (totaldays == r.RemainderNoOfDays2.Value)
                                {
                                    RemainderSentToEmployee = r.RemainderSentToEmployee2.HasValue ? r.RemainderSentToEmployee2.Value : false;
                                    RemainderSentToHOD = r.RemainderSentToHOD2.HasValue ? r.RemainderSentToHOD2.Value : false;
                                    CheckforRemainder = true;
                                }
                                else if (totaldays == r.RemainderNoOfDays3.Value)
                                {
                                    RemainderSentToEmployee = r.RemainderSentToEmployee3.HasValue ? r.RemainderSentToEmployee3.Value : false;
                                    RemainderSentToHOD = r.RemainderSentToHOD3.HasValue ? r.RemainderSentToHOD3.Value : false;
                                    CheckforRemainder = true;
                                }
                                else
                                {
                                    CheckforRemainder = false;
                                }
                            }

                        }
                        if (CheckforRemainder)
                        {
                            CustomModel cm = new CustomModel();
                            MailModel mailmodel = new MailModel();
                            EmailTeamplates emailtemp = new EmailTeamplates();
                            mailmodel.fromemail = "Reminders@hospitals.com";
                            if (RemainderSentToEmployee == true)
                            {
                                mailmodel.toemail = r.EmailId;
                            }

                            if (r.RemainderSentToEmail != null && r.RemainderSentToEmail != "")
                            {
                                if (mailmodel.ccemail != null && mailmodel.ccemail != "")
                                {
                                    mailmodel.ccemail = mailmodel.ccemail + "," + r.RemainderSentToEmail;
                                    if (mailmodel.toemail == null || mailmodel.toemail == "")
                                    {
                                        mailmodel.toemail = r.RemainderSentToEmail.Split(',')[0];
                                    }
                                }
                                else
                                {
                                    if (mailmodel.toemail == null || mailmodel.toemail == "")
                                    {
                                        mailmodel.toemail = r.RemainderSentToEmail.Split(',')[0];
                                    }
                                    mailmodel.ccemail = r.RemainderSentToEmail;
                                }
                            }
                            mailmodel.subject = "Remainder on  " + r.Title + "";

                            mailmodel.filepath = "";
                            mailmodel.fromname = "Reminder from HR Fernandez";
                            tbl_Hr_RemainderLog mlog = new tbl_Hr_RemainderLog();
                            mlog.RemainderId = r.RemainderId;
                            mlog.SentRemainderOn = DateTime.Now.Date;
                            string body = "";
                            string hodbody = "";
                            using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_Asset.html")))
                            {
                                body = reader.ReadToEnd();
                                hodbody = reader.ReadToEnd();
                            }
                            body = body.Replace("[[Heading]]", "Reminder on " + r.Title);
                            hodbody = hodbody.Replace("[[Heading]]", "Reminder on " + r.Title);
                            if (r.RemainderType == "Privileging")
                            {
                                if (totaldays == 30)
                                {
                                    string tblinner = "Dear Nursing Superintendent,<br />" + r.FirstName + " has completed one month today with the Fernandez Foundation. You are requested to complete one month of nursing privilege in discussion with the employee.";
                                    hodbody = hodbody.Replace("[[table]]", tblinner);
                                    tblinner = "Dear " + r.FirstName + ",<br />Congratulations!! You have completed one month today with the Fernandez Foundation. Please connect with your HOD for a one month nursing privileges discussion.";
                                    body = body.Replace("[[table]]", tblinner);
                                }
                                else if (totaldays == 180)
                                {
                                    string tblinner = "Dear Nursing Superintendent,<br />" + r.FirstName + " has completed six months today with the Fernandez Foundation. You are requested to complete six months of nursing privilege in discussion with the employee.";
                                    hodbody = hodbody.Replace("[[table]]", tblinner);
                                    tblinner = "Dear " + r.FirstName + ",<br />Congratulations!! You have completed six months today with the Fernandez Foundation. Please connect with your HOD for a six months nursing privileges discussion.";
                                    body = body.Replace("[[table]]", tblinner);
                                }
                                else
                                {
                                    int years = totaldays / 360;
                                    string tblinner = "Dear Nursing Superintendent,<br />" + r.FirstName + " has completed one year today with the Fernandez Foundation. You are requested to complete " + years + " year(s) of nursing privilege in discussion with the employee.";
                                    hodbody = hodbody.Replace("[[table]]", tblinner);
                                    tblinner = "Dear " + r.FirstName + ",<br />Congratulations!! You have completed "+ years + " year(s) today with the Fernandez Foundation. Please connect with your HOD for a one year nursing privileges discussion.";
                                    body = body.Replace("[[table]]", tblinner);
                                }
                            }
                            else
                            {
                                var table = "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Title:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + r.Title + "</td></tr>";
                                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Reminder Type:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + r.RemainderType + "</td></tr>";
                                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Notes:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + r.RemainderNoOfDaysNotes + "</td></tr>";
                                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Renewal Date:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + r.NextRenewalDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + r.FirstName + "</td></tr>";
                                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location & Dept:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + r.LocationName + " " + r.DepartmentName + "</td></tr>";
                                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Status</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + r.Status + "</td></tr>";
                                table += "</tbody></table>";
                                body = body.Replace("[[table]]", table);

                            }
                            mailmodel.body = body;
                            //check if already sent today
                            var checkstatus = myapp.tbl_Hr_RemainderLog.Where(l => l.RemainderId == r.RemainderId && l.SentRemainderOn.Value == mlog.SentRemainderOn).Count();

                            //switch (r.RemainderRepeatType)
                            //{
                            //    case "Daily":
                            if (checkstatus == 0)
                            {
                                //mailmodel.toemail = "vamsirm26@gmail.com";
                                //mailmodel.ccemail = "vamsirm26@gmail.com";
                                var result = cm.SendEmail(mailmodel);
                                if (result.Contains("Success"))
                                {
                                    mlog.SentRemainderOn = DateTime.Now.Date;
                                    mlog.RemainderSentCC = mailmodel.ccemail;
                                    mlog.RemainderBody = Regex.Replace(mailmodel.body, @"<(.|\n)*?>", "").Replace("&nbsp;", "").Trim();
                                    mlog.RemainderSentTo = mailmodel.toemail;
                                    mlog.RemainderSubject = mailmodel.subject;
                                    myapp.tbl_Hr_RemainderLog.Add(mlog);
                                    myapp.SaveChanges();

                                    if (RemainderSentToHOD == true)
                                    {
                                        if (r.ReportingManagerId != null && r.ReportingManagerId > 0)
                                        {
                                            var userdet = myapp.tbl_User.Where(l => l.EmpId == r.ReportingManagerId).SingleOrDefault();
                                            if (userdet != null && userdet.EmailId != null && userdet.EmailId != "")
                                            {
                                                mailmodel.toemail = userdet.EmailId;
                                                mailmodel.body = hodbody;
                                                cm.SendEmail(mailmodel);
                                            }
                                        }
                                    }
                                }
                            }
                            //        break;
                            //    case "WeeklyOnce":
                            //        if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
                            //        {
                            //            var result2 = cm.SendEmail(mailmodel);
                            //            if (result2.Contains("Success"))
                            //            {
                            //                mlog.SentRemainderOn = DateTime.Now;
                            //                mlog.RemainderSentCC = mailmodel.ccemail;
                            //                mlog.RemainderBody = Regex.Replace(mailmodel.body, @"<(.|\n)*?>", "").Replace("&nbsp;", "").Trim();
                            //                mlog.RemainderSentTo = mailmodel.toemail;
                            //                mlog.RemainderSubject = mailmodel.subject;
                            //                myapp.tbl_Hr_RemainderLog.Add(mlog);
                            //                myapp.SaveChanges();
                            //            }
                            //        }
                            //        break;
                            //    case "TwoWeeksOnce":
                            //        if (DateTime.Now.DayOfWeek == DayOfWeek.Monday && (DateTime.Now.Day < 8 || DateTime.Now.Day > 24))
                            //        {
                            //            var result3 = cm.SendEmail(mailmodel);
                            //            if (result3.Contains("Success"))
                            //            {
                            //                mlog.SentRemainderOn = DateTime.Now;
                            //                mlog.RemainderSentCC = mailmodel.ccemail;
                            //                mlog.RemainderBody = Regex.Replace(mailmodel.body, @"<(.|\n)*?>", "").Replace("&nbsp;", "").Trim();
                            //                mlog.RemainderSentTo = mailmodel.toemail;
                            //                mlog.RemainderSubject = mailmodel.subject;
                            //                myapp.tbl_Hr_RemainderLog.Add(mlog);
                            //                myapp.SaveChanges();
                            //            }
                            //        }
                            //        break;
                            //    case "Monthly":
                            //        if (DateTime.Now.Day == 1)
                            //        {
                            //            var result4 = cm.SendEmail(mailmodel);
                            //            if (result4.Contains("Success"))
                            //            {
                            //                mlog.SentRemainderOn = DateTime.Now;
                            //                mlog.RemainderSentCC = mailmodel.ccemail;
                            //                mlog.RemainderBody = Regex.Replace(mailmodel.body, @"<(.|\n)*?>", "").Replace("&nbsp;", "").Trim();
                            //                mlog.RemainderSentTo = mailmodel.toemail;
                            //                mlog.RemainderSubject = mailmodel.subject;
                            //                myapp.tbl_Hr_RemainderLog.Add(mlog);
                            //                myapp.SaveChanges();
                            //            }
                            //        }
                            //        break;
                            //}
                        }

                    }
                }
                catch (Exception exc)
                {

                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveRemainderType(tbl_Hr_RemainderType model)
        {
            if (model.RemainderTypeId > 0)
            {
                var dbmodel = (from d in myapp.tbl_Hr_RemainderType where d.RemainderTypeId == model.RemainderTypeId select d).SingleOrDefault();
                dbmodel.RemainderNoOfDays = model.RemainderNoOfDays;
                dbmodel.RemainderNoOfDays1 = model.RemainderNoOfDays1;
                dbmodel.RemainderNoOfDays2 = model.RemainderNoOfDays2;
                dbmodel.RemainderNoOfDays3 = model.RemainderNoOfDays3;
                dbmodel.RemainderNoOfDays4 = model.RemainderNoOfDays4;
                dbmodel.RemainderNoOfDaysNotes = model.RemainderNoOfDaysNotes;
                dbmodel.RemainderRepeatType = model.RemainderRepeatType;
                dbmodel.RemainderSentToEmail = model.RemainderSentToEmail;
                dbmodel.RemainderSentToEmployee = model.RemainderSentToEmployee;
                dbmodel.RemainderSentToEmployee1 = model.RemainderSentToEmployee1;
                dbmodel.RemainderSentToEmployee2 = model.RemainderSentToEmployee2;
                dbmodel.RemainderSentToEmployee3 = model.RemainderSentToEmployee3;
                dbmodel.RemainderSentToEmployee4 = model.RemainderSentToEmployee4;
                dbmodel.RemainderSentToHOD = model.RemainderSentToHOD;
                dbmodel.RemainderSentToHOD1 = model.RemainderSentToHOD1;
                dbmodel.RemainderSentToHOD2 = model.RemainderSentToHOD2;
                dbmodel.RemainderSentToHOD3 = model.RemainderSentToHOD3;
                dbmodel.RemainderSentToHOD4 = model.RemainderSentToHOD4;
                dbmodel.RemainderSentType = model.RemainderSentType;
                dbmodel.RemainderType = model.RemainderType;
                dbmodel.Remarks = model.Remarks;
                dbmodel.Status = model.Status;
                myapp.SaveChanges();
            }
            else
            {
                model.CreatedBy = User.Identity.Name;
                model.CreatedOn = DateTime.Now;
                model.ModifiedBy = User.Identity.Name;
                model.ModifiedOn = DateTime.Now;
                myapp.tbl_Hr_RemainderType.Add(model);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRemainderTypeDetails(int id)
        {
            var query = (from d in myapp.tbl_Hr_RemainderType where d.RemainderTypeId == id select d).SingleOrDefault();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRemainderTypeMaster()
        {
            var query = (from d in myapp.tbl_Hr_RemainderType where d.IsActive == true select d).OrderBy(l => l.RemainderType).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetRemainderType(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_Hr_RemainderType select d).OrderByDescending(l => l.RemainderTypeId).ToList();
            IEnumerable<tbl_Hr_RemainderType> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.RemainderTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.RemainderType != null && c.RemainderType.ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                                              c.RemainderTypeId.ToString(),
                                              c.RemainderType,
                                              c.RemainderNoOfDays.ToString(),
                                              c.RemainderRepeatType,
                                              c.RemainderSentType,
                                              c.RemainderNoOfDaysNotes,
                                              c.RemainderSentToEmployee == true?"Yes":"No",
                                              c.RemainderSentToHOD == true?"Yes":"No",
                                              c.RemainderSentToEmail,
                                              c.IsActive == true?"Yes":"No",
                                              c.RemainderTypeId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetHRRemainders(JQueryDataTableParamModel param)
        {

            List<tbl_Hr_Remainder> query = FilterAdminTracker(param.rfromdate, param.rtodate, param.efromdate, param.etodate, param.sfromdate, param.stodate, param.status, param.renewal, param.Expired);
            IEnumerable<tbl_Hr_Remainder> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.RemainderId.ToString().ToLower().Contains(param.sSearch.ToLower())

                                ||
                                c.Title != null && c.Title.ToString().ToLower().Contains(param.sSearch.ToLower())

                               ||
                              c.RegistrationNumber != null && c.RegistrationNumber.ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_Hr_Remainder> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           join d in myapp.tbl_Hr_RemainderType on c.RemainderTypeId equals d.RemainderTypeId
                                           join u in myapp.tbl_User on c.EmpId equals u.EmpId
                                           select new object[] {
                            c.RemainderId.ToString(),
                          u.FirstName +" "+u.LocationName+" "+u.DepartmentName,
                            //ca.Name,
                           // +" " +""+sJ.Name,
                                              c.Title,
                                             d.RemainderType,
                                             // c.Description,
                                              c.LastRenewalDate.HasValue?(c.LastRenewalDate.Value.ToString("dd/MM/yyyy")):"",
                                              c.NextRenewalDate.HasValue?(c.NextRenewalDate.Value.ToString("dd/MM/yyyy")):"",
                                             c.NextRenewalDate.HasValue?(c.NextRenewalDate.Value.ToString("dd/MM/yyyy")):"",
                                              c.Status,
                                              c.RemainderId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public List<tbl_Hr_Remainder> FilterAdminTracker(string rfromdate, string rtodate, string efromdate, string etodate, string sfromdate, string stodate, string status, string renewal, string Expired = "")
        {
            List<tbl_Hr_Remainder> query = (from d in myapp.tbl_Hr_Remainder select d).ToList();


            foreach (var v in query)
            {
                if (v.NextRenewalDate < DateTime.Now && v.Status != "Not using")
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

                query = query.Where(l => l.NextRenewalDate <= date).ToList();

            }

            if (sfromdate != "" && sfromdate != null && stodate != null && stodate != "")
            {
                DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(sfromdate);
                DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(stodate);

                query = query.Where(l => l.LastRenewalDate >= dtfrom && l.LastRenewalDate <= dtto).ToList();
            }
            if (efromdate != "" && efromdate != null && etodate != null && etodate != "")
            {
                DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(efromdate);
                DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(etodate);

                query = query.Where(l => l.NextRenewalDate >= dtfrom && l.NextRenewalDate <= dtto).ToList();
            }
            if (rfromdate != "" && rfromdate != null && rtodate != null && rtodate != "")
            {
                DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(rfromdate);
                DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(rtodate);

                query = query.Where(l => l.NextRenewalDate >= dtfrom && l.NextRenewalDate <= dtto).ToList();
            }
            query = query.OrderByDescending(l => l.RemainderId).ToList();
            return query;
        }

    }
}