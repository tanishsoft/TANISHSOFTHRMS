using Ionic.Zip;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class HospitalController : Controller
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Hospital
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LeavePolicy()
        {
            return View();
        }
        public ActionResult GrievancePolicy()
        {
            return View();
        }
        public ActionResult MedicalReimbursementPolicy()
        {
            return View();
        }
        public ActionResult RecruitmentPolicy()
        {
            return View();
        }
        public ActionResult SexualHarassmentPolicy()
        {
            return View();
        }
        public ActionResult EmployeeHandBook()
        {
            return View();
        }
        public ActionResult WorkFromHome()
        {
            return View();
        }

        public ActionResult NewsLetters()
        {
            //var weekimg = myapp.tbl_Settings.Where(l => l.SettingKey == "GeneralEvents").ToList();
            //if (weekimg.Count > 0)
            //{
            //    ViewBag.GeneralEvents = weekimg[0].SettingValue;
            //}
            //var weekimg2 = myapp.tbl_Settings.Where(l => l.SettingKey == "MedicalEvents").ToList();
            //if (weekimg2.Count > 0)
            //{
            //    ViewBag.MedicalEvents = weekimg2[0].SettingValue;
            //}
            List<tbl_Protocol> list = myapp.tbl_Protocol.Where(l => l.PageName == "Events").ToList();
            return View(list);


        }

        public ActionResult TrainingPolicy()
        {
            return View();
        }
        public ActionResult InductionPolicy()
        {
            return View();
        }
        public ActionResult DisciplinaryPolicy()
        {
            return View();
        }
        public ActionResult ReferralBonusPolicy()
        {
            return View();
        }
        public ActionResult RecognitionAndReward()
        {
            return View();
        }
        public ActionResult AnnualHealthCheckup()
        {
            return View();
        }
        public ActionResult NewJoiners()
        {
            return View();
        }

        public ActionResult UploadWeekImage()
        {
            return View();
        }
        public ActionResult CaseSheetRetrival()
        {
            return View();
        }
        public ActionResult ViewCaseSheetRetrival()
        {
            ViewBag.Total = myapp.tbl_CaseSheetRetrival.Count();
            ViewBag.Approved = myapp.tbl_CaseSheetRetrival.Where(l => l.Status == "Approved").Count();
            ViewBag.Rejected = myapp.tbl_CaseSheetRetrival.Where(l => l.Status == "Rejected").Count();
            ViewBag.onhold = myapp.tbl_CaseSheetRetrival.Where(l => l.Status == "On Hold").Count();
            return View();
        }
        public ActionResult ManageSheetRetrival()
        {
            ViewBag.Total = myapp.tbl_CaseSheetRetrival.Count();
            ViewBag.Approved = myapp.tbl_CaseSheetRetrival.Where(l => l.Status == "Approved").Count();
            ViewBag.Rejected = myapp.tbl_CaseSheetRetrival.Where(l => l.Status == "Rejected").Count();
            ViewBag.onhold = myapp.tbl_CaseSheetRetrival.Where(l => l.Status == "On Hold").Count();
            return View();
        }
        public ActionResult CaseSheetAdminSettings()
        {
            return View();
        }

        // FMU Data
        public ActionResult FMUDataRetrival()
        {
            return View();
        }
        public ActionResult ViewFMUDataRetrival()
        {
            ViewBag.Total = myapp.tbl_FMUDataRetrieval.Count();
            ViewBag.Approved = myapp.tbl_FMUDataRetrieval.Where(l => l.Status == "Approved").Count();
            ViewBag.Rejected = myapp.tbl_FMUDataRetrieval.Where(l => l.Status == "Rejected").Count();
            ViewBag.onhold = myapp.tbl_FMUDataRetrieval.Where(l => l.Status == "On Hold").Count();
            return View();
        }
        public ActionResult ManageFMUDataRetrival()
        {
            ViewBag.Total = myapp.tbl_FMUDataRetrieval.Count();
            ViewBag.Approved = myapp.tbl_FMUDataRetrieval.Where(l => l.Status == "Approved").Count();
            ViewBag.Rejected = myapp.tbl_FMUDataRetrieval.Where(l => l.Status == "Rejected").Count();
            ViewBag.onhold = myapp.tbl_FMUDataRetrieval.Where(l => l.Status == "On Hold").Count();
            return View();
        }
        public ActionResult FMUDataAdminSettings()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadWeekImage(HttpPostedFileBase ExcelFileData)
        {

            string NewGUID = Guid.NewGuid().ToString();

            string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + Path.GetExtension(ExcelFileData.FileName);
            string PathName = Path.Combine(Server.MapPath("~/ExcelUplodes/"), UniqueFileName);
            ExcelFileData.SaveAs(PathName);

            List<tbl_Settings> tb = myapp.tbl_Settings.Where(a => a.SettingKey == "WeekImage").ToList();
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
                    SettingKey = "WeekImage",
                    SettingValue = UniqueFileName
                };
                myapp.tbl_Settings.Add(tbs);
                myapp.SaveChanges();
            }
            ViewBag.Message = "Success";
            return View();
        }
        public JsonResult RemoveSettingsimages(string type)
        {
            List<tbl_Settings> tb = myapp.tbl_Settings.Where(a => a.SettingKey == type).ToList();
            if (tb.Count > 0)
            {
                string PathName1 = Path.Combine(Server.MapPath("~/ExcelUplodes/"), tb[0].SettingValue);
                if ((System.IO.File.Exists(PathName1)))
                {
                    System.IO.File.Delete(PathName1);
                }
                myapp.tbl_Settings.Remove(tb[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UploadModalpopup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadModalpopup(HttpPostedFileBase ExcelFileData)
        {
            string NewGUID = Guid.NewGuid().ToString();

            string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + Path.GetExtension(ExcelFileData.FileName);
            string PathName = Path.Combine(Server.MapPath("~/ExcelUplodes/"), UniqueFileName);
            ExcelFileData.SaveAs(PathName);

            List<tbl_Settings> tb = myapp.tbl_Settings.Where(a => a.SettingKey == "HomepagePopup").ToList();
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
                    SettingKey = "HomepagePopup",
                    SettingValue = UniqueFileName
                };
                myapp.tbl_Settings.Add(tbs);
                myapp.SaveChanges();
            }
            ViewBag.Message = "Success";
            return View();
        }

        public ActionResult UploadMedicalEvents()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadMedicalEvents(HttpPostedFileBase ExcelFileData)
        {
            string NewGUID = Guid.NewGuid().ToString();

            string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + Path.GetExtension(ExcelFileData.FileName);
            string PathName = Path.Combine(Server.MapPath("~/ExcelUplodes/"), UniqueFileName);
            ExcelFileData.SaveAs(PathName);

            List<tbl_Settings> tb = myapp.tbl_Settings.Where(a => a.SettingKey == "MedicalEvents").ToList();
            if (tb.Count > 0)
            {
                tb[0].SettingValue = UniqueFileName;
                tb[0].IsActive = true;
                myapp.SaveChanges();
            }
            else
            {
                tbl_Settings tbs = new tbl_Settings
                {
                    IsActive = true,
                    SettingKey = "MedicalEvents",
                    SettingValue = UniqueFileName
                };
                myapp.tbl_Settings.Add(tbs);
                myapp.SaveChanges();
            }
            ViewBag.Message = "Success";
            return View();
        }

        public ActionResult UploadGeneralEvents()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadGeneralEvents(HttpPostedFileBase ExcelFileData)
        {
            string NewGUID = Guid.NewGuid().ToString();

            string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + Path.GetExtension(ExcelFileData.FileName);
            string PathName = Path.Combine(Server.MapPath("~/ExcelUplodes/"), UniqueFileName);
            ExcelFileData.SaveAs(PathName);

            List<tbl_Settings> tb = myapp.tbl_Settings.Where(a => a.SettingKey == "GeneralEvents").ToList();
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
                    SettingKey = "GeneralEvents",
                    SettingValue = UniqueFileName
                };
                myapp.tbl_Settings.Add(tbs);
                myapp.SaveChanges();
            }
            ViewBag.Message = "Success";
            return View();
        }

        public ActionResult PerformanceAppraisal()
        {
            return View();
        }
        public ActionResult ProtocolView(string name, string PageName)
        {
            ViewBag.ProtocolName = name;
            List<tbl_Protocol> list = myapp.tbl_Protocol.Where(l => l.Category == name && l.PageName == PageName).OrderBy(l => l.Name).ToList();

            return View(list);
        }
        public ActionResult ManageProtocaol()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ManageProtocaol(tbl_Protocol model, HttpPostedFileBase ExcelFileData)
        {
            string NewGUID = Guid.NewGuid().ToString();

            string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + Path.GetExtension(ExcelFileData.FileName);
            string PathName = Path.Combine(Server.MapPath("~/ExcelUplodes/"), UniqueFileName);
            ExcelFileData.SaveAs(PathName);
            model.CreatedOn = DateTime.Now;
            model.CreatedBy = "admin";
            model.IsActive = true;
            model.ModifiedBy = "admin";
            model.ModifiedOn = DateTime.Now;
            model.FilePath = UniqueFileName;

            myapp.tbl_Protocol.Add(model);
            myapp.SaveChanges();

            ViewBag.Message = "Success";
            return Redirect("/Hospital/ManageProtocaol");
        }
        public JsonResult GetProtocols()
        {
            List<tbl_Protocol> list = myapp.tbl_Protocol.ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteProtocol(int id)
        {
            List<tbl_Protocol> list = myapp.tbl_Protocol.Where(l => l.Id == id).ToList();
            if (list.Count > 0)
            {
                string PathName = Path.Combine(Server.MapPath("~/ExcelUplodes/"), list[0].FilePath);
                if ((System.IO.File.Exists(PathName)))
                {
                    System.IO.File.Delete(PathName);
                }
                myapp.tbl_Protocol.Remove(list[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageNicuDrug()
        {
            return View();
        }
        public ActionResult Holidays()
        {
            return View();
        }
        public ActionResult IntranetSurvey()
        {
            return View();
        }

        private static readonly CultureInfo pr = CultureInfo.InvariantCulture;
        public static DateTime ConverDateStringtoDatetime(string date)
        {
            return DateTime.ParseExact(date, "dd-MM-yyyy", pr);
        }
        public JsonResult SaveFMUData(CaseSheetRetrivalModel Case)
        {
            tbl_FMUDataRetrieval Sheet = new tbl_FMUDataRetrieval
            {
                Comments = Case.Comments,
                DataRequestedBy = Case.DataRequestedBy,
                DataRequestedDate = ConverDateStringtoDatetime(Case.DataRequestedDate),
                DepartmentId = Case.DepartmentId,
                Email = Case.Email,
                EmpName = Case.EmpName,
                Description = Case.Description,
                SheetType = Case.SheetType,
                FromDate = ConverDateStringtoDatetime(Case.FromDate),
                ToDate = ConverDateStringtoDatetime(Case.ToDate)
            };
            if (Case.EmpNo != null)
            {
                Sheet.EmpNo = Case.EmpNo;
            }

            else
            {
                //Case.EmpNo = "fh_" + Case.EmpNo;
                Sheet.EmpNo = Case.EmpNo;
            }

            Sheet.Mobile = Case.Mobile;
            Sheet.PurposeOfTheData = Case.PurposeOfTheData;
            Sheet.SpecificData = Case.SpecificData;
            Sheet.SpecificDataValue = Case.SpecificDataValue;
            Sheet.CreatedOn = DateTime.Now;
            Sheet.CreatedBy = User.Identity.Name;
            myapp.tbl_FMUDataRetrieval.Add(Sheet);
            myapp.SaveChanges();
            sendFMUDataEmail(Sheet);
            tbl_Department Department = myapp.tbl_Department.Where(l => l.DepartmentId == Sheet.DepartmentId).SingleOrDefault();
            string AdminEmail = (from var in myapp.tbl_Settings where var.SettingKey == "FMUDataAdminEmail" select var.SettingValue).FirstOrDefault();
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel();
            EmailTeamplates emailtemp = new EmailTeamplates();
            mailmodel.fromemail = "Leave@hospitals.com";
            mailmodel.toemail = Sheet.Email;
            if (AdminEmail != null)
            {
                mailmodel.ccemail = AdminEmail;
            }

            mailmodel.subject = "A FMU Data " + Sheet.FMUDataRetrievalId + "";
            string mailbody = "<p style='font-family:verdana'>HI Team,";
            mailbody += "<p style='font-family:verdana'>" + Sheet.EmpName + " has submitted Case Sheet . Please click on this <a target='_blank' href='https://infonet.fernandezhospital.com/'>link</a>  to see the FUM Data.Do not forget to update the FUM Data status after completion.</p>";
            mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
            mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.EmpNo + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.EmpName + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee Department </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Department.DepartmentName + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'> Email</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.Email + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'> Mobile</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.Mobile + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requested by</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.DataRequestedBy + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requested Date</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.DataRequestedDate + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Purpose of data </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.PurposeOfTheData + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Specific Data</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.SpecificData + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Specific Data value</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.SpecificDataValue + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Comments</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.Comments + "</td></tr>";



            mailbody += "</table>";
            mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
            mailmodel.body = mailbody;
            //mailmodel.body = "A New Ticket Assigned to you";
            //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
            mailmodel.filepath = "";
            //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
            mailmodel.fromname = "FMU Data";

            cm.SendEmail(mailmodel);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public void sendFMUDataEmail(tbl_FMUDataRetrieval sheet)
        {
            string body = string.Empty;
            tbl_Department Department = myapp.tbl_Department.Where(l => l.DepartmentId == sheet.DepartmentId).SingleOrDefault();
            using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/FMUDataEmailTemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{EmpName}", sheet.EmpName);
            body = body.Replace("{EmpNo}", sheet.EmpNo);
            body = body.Replace("{DepartmentName}", Department.DepartmentName);
            body = body.Replace("{DataRequestedBy}", sheet.DataRequestedBy);
            body = body.Replace("{DataRequestedDate}", (sheet.DataRequestedDate != null) ? Convert.ToDateTime(sheet.DataRequestedDate).ToString("dd/MM/yyyy") : "");
            body = body.Replace("{PurposeOfTheData}", sheet.PurposeOfTheData);
            body = body.Replace("{SpecificData}", sheet.SpecificData);
            body = body.Replace("{SpecificDataValue}", sheet.SpecificDataValue);
            body = body.Replace("{Comments}", sheet.Comments);
            body = body.Replace("{Id}", sheet.FMUDataRetrievalId.ToString());
            string Subject = "";
            Subject = "A FMU Data " + sheet.FMUDataRetrievalId + "";
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel
            {
                fromemail = "Leave@hospitals.com",
                toemail = "drgeeta@fernandez.foundation",
                subject = Subject,
                body = body,
                filepath = "",
                fromname = "FMU Data",
                ccemail = ""
            };
            cm.SendEmail(mailmodel);
        }
        public JsonResult ApproveFMUData(int id)
        {
            ViewBag.Message = "Success";
            List<tbl_FMUDataRetrieval> sheet = myapp.tbl_FMUDataRetrieval.Where(s => s.FMUDataRetrievalId == id).ToList();
            if (sheet.Count > 0)
            {
                sheet[0].ModifiedBy = User.Identity.Name;
                sheet[0].ModifiedOn = DateTime.Now;
                sheet[0].ApprovedBy = User.Identity.Name;
                sheet[0].ApprovedOn = DateTime.Now;
                sheet[0].Status = "Approved";
            }
            myapp.SaveChanges();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult RejectFMUData(int id)
        {
            ViewBag.Message = "Success";
            List<tbl_FMUDataRetrieval> sheet = myapp.tbl_FMUDataRetrieval.Where(s => s.FMUDataRetrievalId == id).ToList();
            if (sheet.Count > 0)
            {
                sheet[0].ApprovedBy = User.Identity.Name;
                sheet[0].ApprovedOn = DateTime.Now;
                sheet[0].ModifiedBy = User.Identity.Name;
                sheet[0].ModifiedOn = DateTime.Now;
                sheet[0].Status = "Rejected";
            }
            myapp.SaveChanges();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetFMUData(JQueryDataTableParamModel param)

        {
            List<tbl_Department> Departlist = myapp.tbl_Department.ToList();
            List<tbl_FMUDataRetrieval> query = myapp.tbl_FMUDataRetrieval.OrderByDescending(x => x.FMUDataRetrievalId).ToList();

            IEnumerable<tbl_FMUDataRetrieval> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c =>
                    c.FMUDataRetrievalId.ToString().ToLower().Contains(param.sSearch.ToLower())
                    ||
                    c.EmpNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.EmpName != null && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.PurposeOfTheData != null && c.PurposeOfTheData.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.SpecificData != null && c.SpecificData.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }

            IEnumerable<tbl_FMUDataRetrieval> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            IEnumerable<object[]> result = from c in displayedCompanies
                                           join u in Departlist on c.DepartmentId equals u.DepartmentId

                                           select new object[] {
                             c.FMUDataRetrievalId.ToString(),
                             c.SheetType,
                             c.EmpNo,
                             c.EmpName,
                            u.DepartmentName
,
                             c.Email,
                             c.Mobile,
                             c.DataRequestedBy,
                            c.DataRequestedDate.HasValue?(c.DataRequestedDate.Value.ToString("dd/MM/yyyy")):"",
                            c.PurposeOfTheData,
                            c.SpecificData,
                            c.SpecificDataValue,
                            c.Description,
                            c.FromDate.HasValue?(c.FromDate.Value.ToString("dd/MM/yyyy")):"",
                             c.ToDate.HasValue?(c.ToDate.Value.ToString("dd/MM/yyyy")):"",
                            c.Comments,
                             c.Status,
                             c.ApprovedBy,
                             c.FMUDataRetrievalId
                           };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateStatusFMUData(int Id, string Status, string Remarks)
        {
            bool ValStatus = true;
            List<tbl_FMUDataRetrieval> cat = myapp.tbl_FMUDataRetrieval.Where(l => l.FMUDataRetrievalId == Id).ToList();
            if (cat.Count > 0)
            {
                cat[0].Status = Status;
                cat[0].StatusRemarks = Remarks;

                myapp.SaveChanges();
                string customuserid = User.Identity.Name;
                int deptid = cat[0].DepartmentId.HasValue ? cat[0].DepartmentId.Value : 1;
                tbl_Department Department = myapp.tbl_Department.Where(l => l.DepartmentId == deptid).SingleOrDefault();
                string AdminEmail = (from var in myapp.tbl_Settings where var.SettingKey == "CaseSheetAdminEmail" select var.SettingValue).FirstOrDefault();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = cat[0].Email;
                if (AdminEmail != null)
                {
                    mailmodel.ccemail = AdminEmail;
                }

                mailmodel.subject = "FMU " + cat[0].FMUDataRetrievalId + " has been " + cat[0].Status + "";
                string mailbody = "<p style='font-family:verdana'>HI Team,";
                mailbody += "<p style='font-family:verdana'>" + cat[0].EmpName + " your FMU status has updated .</p>";
                mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].EmpNo + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].EmpName + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee Department </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Department.DepartmentName + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Email</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Email + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Mobile</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Mobile + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requested by</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].DataRequestedBy + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requested Date</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].DataRequestedDate + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Purpose of data </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].PurposeOfTheData + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Specific Data</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].SpecificData + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Specific Data value</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].SpecificDataValue + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Comments</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Comments + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Status</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Status + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Remarks</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].StatusRemarks + "</td></tr>";

                mailbody += "</table>";
                mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
                mailmodel.body = mailbody;
                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "FMU Data";

                cm.SendEmail(mailmodel);
            }
            else
            {
                ValStatus = false;
            }

            return Json(ValStatus, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelFMUData(string FromDate, string ToDate)
        {
            DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(FromDate);
            DateTime todate = ProjectConvert.ConverDateStringtoDatetime(ToDate);
            List<tbl_Department> Departlist = myapp.tbl_Department.ToList();
            List<tbl_FMUDataRetrieval> query = myapp.tbl_FMUDataRetrieval.Where(p => p.DataRequestedDate >= fromdate && p.DataRequestedDate <= todate).ToList();

            var Result = from c in query
                         join u in Departlist on c.DepartmentId equals u.DepartmentId
                         select new
                         {
                             c,
                             u.DepartmentName
                         };
            System.Data.DataTable products = new System.Data.DataTable("Case Sheet");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Employee No", typeof(string));
            products.Columns.Add("Employee Name", typeof(string));
            products.Columns.Add("Department name", typeof(string));
            products.Columns.Add("E mail", typeof(string));
            products.Columns.Add("Mobile", typeof(string));
            products.Columns.Add("Data Requested by", typeof(string));
            products.Columns.Add("Data Required Date", typeof(string));
            products.Columns.Add("purpose of data", typeof(string));
            products.Columns.Add("Specific Data", typeof(string));
            products.Columns.Add("SpecificDataValue", typeof(string));

            products.Columns.Add("Comments", typeof(string));
            products.Columns.Add("Status", typeof(string));
            products.Columns.Add("StatusRemarks", typeof(string));
            foreach (var item in Result)
            {
                products.Rows.Add(
               item.c.FMUDataRetrievalId.ToString(),
                item.c.EmpNo,
                 item.c.EmpName,
                 item.DepartmentName,
            item.c.Email,
            item.c.Mobile.ToString(),
             item.c.DataRequestedBy,
             item.c.DataRequestedDate.HasValue ? (item.c.DataRequestedDate.Value.ToString("dd/MM/yyyy")) : "",
item.c.PurposeOfTheData,
item.c.SpecificData,
item.c.SpecificDataValue,
item.c.Comments,
item.c.Status,
item.c.StatusRemarks

                );
            }


            GridView grid = new GridView
            {
                DataSource = products
            };
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=FMUDataRetrival.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveCaseSheet(CaseSheetRetrivalModel Case)
        {
            tbl_CaseSheetRetrival Sheet = new tbl_CaseSheetRetrival
            {
                Comments = Case.Comments,
                DataRequestedBy = Case.DataRequestedBy,
                DataRequestedDate = ConverDateStringtoDatetime(Case.DataRequestedDate),
                DepartmentId = Case.DepartmentId,
                Email = Case.Email,
                EmpName = Case.EmpName,
                Description = Case.Description,
                SheetType = Case.SheetType,
                FromDate = ConverDateStringtoDatetime(Case.FromDate),
                ToDate = ConverDateStringtoDatetime(Case.ToDate)
            };
            if (Case.EmpNo != null)
            {
                Sheet.EmpNo = Case.EmpNo;
            }

            else
            {
                //Case.EmpNo = "fh_" + Case.EmpNo;
                Sheet.EmpNo = Case.EmpNo;
            }

            Sheet.Mobile = Case.Mobile;
            Sheet.PurposeOfTheData = Case.PurposeOfTheData;
            Sheet.SpecificData = Case.SpecificData;
            Sheet.SpecificDataValue = Case.SpecificDataValue;
            Sheet.CreatedOn = DateTime.Now;
            Sheet.CreatedBy = User.Identity.Name;
            myapp.tbl_CaseSheetRetrival.Add(Sheet);
            myapp.SaveChanges();
            //sendCasesheetEmail(Sheet);
            tbl_Department Department = myapp.tbl_Department.Where(l => l.DepartmentId == Sheet.DepartmentId).SingleOrDefault();
            string AdminEmail = (from var in myapp.tbl_Settings where var.SettingKey == "CaseSheetAdminEmail" select var.SettingValue).FirstOrDefault();
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel();
            EmailTeamplates emailtemp = new EmailTeamplates();
            mailmodel.fromemail = "Leave@hospitals.com";
            mailmodel.toemail = Sheet.Email;
            if (AdminEmail != null)
            {
                mailmodel.ccemail = AdminEmail;
            }

            mailmodel.subject = "A Case Sheet " + Sheet.CaseSheetRetrivalId + "";
            string mailbody = "<p style='font-family:verdana'>HI Team,";
            mailbody += "<p style='font-family:verdana'>" + Sheet.EmpName + " has submitted Case Sheet . Please click on this <a target='_blank' href='https://infonet.fernandezhospital.com/'>link</a>  to see the Case Sheet.Do not forget to update the Case Sheet status after completion.</p>";
            mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
            mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.EmpNo + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.EmpName + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee Department </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Department.DepartmentName + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'> Email</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.Email + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'> Mobile</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.Mobile + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requested by</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.DataRequestedBy + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requested Date</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.DataRequestedDate + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Purpose of data </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.PurposeOfTheData + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Specific Data</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.SpecificData + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Specific Data value</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.SpecificDataValue + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Comments</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Sheet.Comments + "</td></tr>";



            mailbody += "</table>";
            mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
            mailmodel.body = mailbody;
            //mailmodel.body = "A New Ticket Assigned to you";
            //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
            mailmodel.filepath = "";
            //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
            mailmodel.fromname = "Case Sheet";

            cm.SendEmail(mailmodel);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public void sendCasesheetEmail(tbl_CaseSheetRetrival sheet)
        {
            string body = string.Empty;
            tbl_Department Department = myapp.tbl_Department.Where(l => l.DepartmentId == sheet.DepartmentId).SingleOrDefault();
            using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/CaseSheetEmailTemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{EmpName}", sheet.EmpName);
            body = body.Replace("{EmpNo}", sheet.EmpNo);
            body = body.Replace("{DepartmentName}", Department.DepartmentName);
            body = body.Replace("{DataRequestedBy}", sheet.DataRequestedBy);
            body = body.Replace("{DataRequestedDate}", (sheet.DataRequestedDate != null) ? Convert.ToDateTime(sheet.DataRequestedDate).ToString("dd/MM/yyyy") : "");
            body = body.Replace("{PurposeOfTheData}", sheet.PurposeOfTheData);
            body = body.Replace("{SpecificData}", sheet.SpecificData);
            body = body.Replace("{SpecificDataValue}", sheet.SpecificDataValue);
            body = body.Replace("{Comments}", sheet.Comments);
            body = body.Replace("{Id}", sheet.CaseSheetRetrivalId.ToString());
            string Subject = "";
            Subject = "A Case Sheet " + sheet.CaseSheetRetrivalId + "";
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel
            {
                fromemail = "Leave@hospitals.com",
                toemail = "drtara@fernandez.foundation",
                subject = Subject,
                body = body,
                filepath = "",
                fromname = "Case Sheet",
                ccemail = ""
            };
            cm.SendEmail(mailmodel);
        }
        public JsonResult ApproveCaseSheet(int id)
        {
            ViewBag.Message = "Success";
            List<tbl_CaseSheetRetrival> sheet = myapp.tbl_CaseSheetRetrival.Where(s => s.CaseSheetRetrivalId == id).ToList();
            if (sheet.Count > 0)
            {
                sheet[0].ModifiedBy = User.Identity.Name;
                sheet[0].ModifiedOn = DateTime.Now;
                sheet[0].ApprovedBy = User.Identity.Name;
                sheet[0].ApprovedOn = DateTime.Now;
                sheet[0].Status = "Approved";
            }
            myapp.SaveChanges();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult RejectCaseSheet(int id)
        {
            ViewBag.Message = "Success";
            List<tbl_CaseSheetRetrival> sheet = myapp.tbl_CaseSheetRetrival.Where(s => s.CaseSheetRetrivalId == id).ToList();
            if (sheet.Count > 0)
            {
                sheet[0].ApprovedBy = User.Identity.Name;
                sheet[0].ApprovedOn = DateTime.Now;
                sheet[0].ModifiedBy = User.Identity.Name;
                sheet[0].ModifiedOn = DateTime.Now;
                sheet[0].Status = "Rejected";
            }
            myapp.SaveChanges();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCaseSheet(JQueryDataTableParamModel param)

        {
            List<tbl_Department> Departlist = myapp.tbl_Department.ToList();
            List<tbl_CaseSheetRetrival> query = myapp.tbl_CaseSheetRetrival.OrderByDescending(x => x.CaseSheetRetrivalId).ToList();

            IEnumerable<tbl_CaseSheetRetrival> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c =>
                    c.CaseSheetRetrivalId.ToString().ToLower().Contains(param.sSearch.ToLower())
                    ||
                    c.EmpNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.EmpName != null && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.PurposeOfTheData != null && c.PurposeOfTheData.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.SpecificData != null && c.SpecificData.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }

            IEnumerable<tbl_CaseSheetRetrival> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            IEnumerable<object[]> result = from c in displayedCompanies
                                           join u in Departlist on c.DepartmentId equals u.DepartmentId

                                           select new object[] {
                             c.CaseSheetRetrivalId.ToString(),
                             c.SheetType,
                             c.EmpNo,
                             c.EmpName,
                            u.DepartmentName
,
                             c.Email,
                             c.Mobile,
                             c.DataRequestedBy,
                            c.DataRequestedDate.HasValue?(c.DataRequestedDate.Value.ToString("dd/MM/yyyy")):"",
                            c.PurposeOfTheData,
                            c.SpecificData,
                            c.SpecificDataValue,
                            c.Description,
                            c.FromDate.HasValue?(c.FromDate.Value.ToString("dd/MM/yyyy")):"",
                             c.ToDate.HasValue?(c.ToDate.Value.ToString("dd/MM/yyyy")):"",
                            c.Comments,
                             c.Status,
                             c.ApprovedBy,
                             c.CaseSheetRetrivalId
                           };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateStatus(int Id, string Status, string Remarks)
        {
            bool ValStatus = true;
            List<tbl_CaseSheetRetrival> cat = myapp.tbl_CaseSheetRetrival.Where(l => l.CaseSheetRetrivalId == Id).ToList();
            if (cat.Count > 0)
            {
                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
                cat[0].ApprovedBy = User.Identity.Name;
                cat[0].ApprovedOn = DateTime.Now;
                cat[0].Status = Status;
                cat[0].StatusRemarks = Remarks;

                myapp.SaveChanges();
                string customuserid = User.Identity.Name;
                int deptid = cat[0].DepartmentId.HasValue ? cat[0].DepartmentId.Value : 1;
                tbl_Department Department = myapp.tbl_Department.Where(l => l.DepartmentId == deptid).SingleOrDefault();
                string AdminEmail = (from var in myapp.tbl_Settings where var.SettingKey == "CaseSheetAdminEmail" select var.SettingValue).FirstOrDefault();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = cat[0].Email;
                if (AdminEmail != null)
                {
                    mailmodel.ccemail = AdminEmail;
                }

                mailmodel.subject = "Case Sheet " + cat[0].CaseSheetRetrivalId + " has been " + cat[0].Status + "";
                string mailbody = "<p style='font-family:verdana'>HI Team,";
                mailbody += "<p style='font-family:verdana'>" + cat[0].EmpName + " your Case Sheet status has updated .</p>";
                mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].EmpNo + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].EmpName + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee Department </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Department.DepartmentName + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Email</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Email + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Mobile</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Mobile + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requested by</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].DataRequestedBy + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requested Date</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].DataRequestedDate + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Purpose of data </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].PurposeOfTheData + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Specific Data</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].SpecificData + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Specific Data value</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].SpecificDataValue + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Comments</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Comments + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Status</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Status + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Remarks</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].StatusRemarks + "</td></tr>";

                mailbody += "</table>";
                mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
                mailmodel.body = mailbody;
                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "Case Sheet";

                cm.SendEmail(mailmodel);
            }
            else
            {
                ValStatus = false;
            }

            return Json(ValStatus, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelCaseSheet(string FromDate, string ToDate)
        {
            DateTime fromdate = Convert.ToDateTime(FromDate);
            DateTime todate = Convert.ToDateTime(ToDate);
            List<tbl_Department> Departlist = myapp.tbl_Department.ToList();
            List<tbl_CaseSheetRetrival> query = myapp.tbl_CaseSheetRetrival.Where(p => p.DataRequestedDate >= fromdate && p.DataRequestedDate <= todate).ToList();

            var Result = from c in query
                         join u in Departlist on c.DepartmentId equals u.DepartmentId
                         select new
                         {
                             c,
                             u.DepartmentName
                         };
            System.Data.DataTable products = new System.Data.DataTable("Case Sheet");
            products.Columns.Add("CaseSheetRetrivalId", typeof(string));
            products.Columns.Add("Employee No", typeof(string));
            products.Columns.Add("Employee Name", typeof(string));
            products.Columns.Add("Department name", typeof(string));
            products.Columns.Add("E mail", typeof(string));
            products.Columns.Add("Mobile", typeof(string));
            products.Columns.Add("Data Requested by", typeof(string));
            products.Columns.Add("Data Required Date", typeof(string));
            products.Columns.Add("purpose of data", typeof(string));
            products.Columns.Add("Specific Data", typeof(string));
            products.Columns.Add("SpecificDataValue", typeof(string));

            products.Columns.Add("Comments", typeof(string));
            products.Columns.Add("Status", typeof(string));
            products.Columns.Add("StatusRemarks", typeof(string));
            foreach (var item in Result)
            {
                products.Rows.Add(
               item.c.CaseSheetRetrivalId.ToString(),
                item.c.EmpNo,
                 item.c.EmpName,
                 item.DepartmentName,
            item.c.Email,
            item.c.Mobile.ToString(),
             item.c.DataRequestedBy,
             item.c.DataRequestedDate.HasValue ? (item.c.DataRequestedDate.Value.ToString("dd/MM/yyyy")) : "",
item.c.PurposeOfTheData,
item.c.SpecificData,
item.c.SpecificDataValue,
item.c.Comments,
item.c.Status,
item.c.StatusRemarks

                );
            }


            GridView grid = new GridView
            {
                DataSource = products
            };
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=CaseSheetRetrival.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateAdminsettings(IEnumerable<tbl_Settings> datav)
        {

            foreach (tbl_Settings item in datav)
            {
                List<tbl_Settings> cat = myapp.tbl_Settings.Where(l => l.SettingKey == item.SettingKey).ToList();
                if (cat.Count > 0)
                {
                    cat[0].SettingKey = item.SettingKey;
                    cat[0].SettingValue = item.SettingValue;
                    cat[0].IsActive = true;
                    myapp.SaveChanges();
                }
                else
                {
                    tbl_Settings settings = new tbl_Settings
                    {
                        SettingKey = item.SettingKey,
                        SettingValue = item.SettingValue,
                        IsActive = true
                    };
                    myapp.tbl_Settings.Add(settings);
                    myapp.SaveChanges();
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult GetSettings(string Name)
        {
            List<tbl_Settings> list = myapp.tbl_Settings.Where(X => X.SettingKey.Contains(Name)).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAdminSettingsDetails()
        {
            tbl_Settings AdminEmail = myapp.tbl_Settings.Where(X => X.SettingKey == "CaseSheetAdminEmail").SingleOrDefault();

            return Json(AdminEmail, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAdminSettingsDetailsFMUData()
        {
            tbl_Settings AdminEmail = myapp.tbl_Settings.Where(X => X.SettingKey == "FMUDataAdminEmail").SingleOrDefault();

            return Json(AdminEmail, JsonRequestBehavior.AllowGet);
        }
        public ActionResult OurBrand()
        {
            return View();
        }
        public ActionResult OurBrandView()
        {
            return View();
        }
        public ActionResult WorkPlaceViolence()
        {
            return View();
        }
        public ActionResult BrandMerchandise()
        {
            return View();
        }
        public ActionResult BrandTemplates()
        {
            return View();
        }
        //[AllowAnonymous]
        public ActionResult TrendingOnSocialMedia()
        {
            string pathserver = Server.MapPath("~/Documents/Images/Trending_on_social_media/");
            List<string> folders = new List<string>();
            var Images1 = Directory.GetDirectories(pathserver);
            foreach (var dir in Images1)
            {
                folders.Add(dir.Replace(pathserver, ""));
            }
            return View(folders);
        }
        public ActionResult FullTrendingOnSocialMedia(string date)
        {
            ViewBag.Date = date;
            return View();
        }
        #region EmrEntry

        public ActionResult AjaxGetEmrEntry(JQueryDataTableParamModel param)
        {
            List<tbl_EmrEntry> query = myapp.tbl_EmrEntry.OrderByDescending(l => l.EmrEntryId).ToList();
            if (param.locationid != null && param.locationid != 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.DateofError != null && x.DateofError.Value.Date >= FromDate.Date).Where(x => x.DateofError != null && x.DateofError.Value.Date <= ToDate.Date).ToList();
            }
            if (!User.IsInRole("Admin"))
            {
                query = query.Where(l => l.UserId == User.Identity.Name).ToList();
            }
            if (param.Emp != null && param.Emp != "")
            {
                query = query.Where(l => l.UserId == param.Emp).ToList();
            }

            IEnumerable<tbl_EmrEntry> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.EmrEntryId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.UserId.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_EmrEntry> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies

                                           select new object[] {
                            c.EmrEntryId,
                             (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).FirstOrDefault(),
                            (from var in myapp.tbl_User where var.CustomUserId == c.UserId select var.FirstName).FirstOrDefault(),
                            c.MrNo_PatientName, c.PatientName,c.DoctorName,
                            c.Mobile,
                            c.DateofError.HasValue ? c.DateofError.Value.ToString("dd-MM-yyyy") : "",
                             c.DateOfChange.HasValue ? c.DateOfChange.Value.ToString("dd-MM-yyyy") : "",
                            c.ReasonForChange,
                            c.RequestedCorrectionDetails,
                            c.FormName,
                             c.Suggestions,
                           c.IsActive.HasValue?c.IsActive.ToString():"false",


                            c.EmrEntryId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelEmrEntry(string fromDate, string toDate, int locationid = 0)
        {
            List<tbl_EmrEntry> query = myapp.tbl_EmrEntry.OrderByDescending(l => l.EmrEntryId).ToList();
            if (locationid != null && locationid != 0)
            {
                query = query.Where(l => l.LocationId == locationid).ToList();
            }
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.DateofError != null && x.DateofError.Value.Date >= FromDate.Date).Where(x => x.DateofError != null && x.DateofError.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("VehicleParking");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("User", typeof(string));
            products.Columns.Add("MrNo", typeof(string));
            products.Columns.Add("PatientName", typeof(string));
            products.Columns.Add("DoctorName", typeof(string));
            products.Columns.Add("Mobile", typeof(string));
            products.Columns.Add("DateofError", typeof(string));
            products.Columns.Add("DateOfChange", typeof(string));
            products.Columns.Add("ReasonForChange", typeof(string));

            products.Columns.Add("RequestedCorrectionDetails", typeof(string));
            products.Columns.Add("FormName", typeof(string));
            products.Columns.Add("Suggestions", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();

                products.Rows.Add(
                    c.EmrEntryId,
                             (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).FirstOrDefault(),
                            (from var in myapp.tbl_User where var.CustomUserId == c.UserId select var.FirstName).FirstOrDefault(),
                            c.MrNo_PatientName, c.PatientName, c.DoctorName,
                            c.Mobile,
                            c.DateofError.HasValue ? c.DateofError.Value.ToString("dd-MM-yyyy") : "",
                             c.DateOfChange.HasValue ? c.DateOfChange.Value.ToString("dd-MM-yyyy") : "",
                            c.ReasonForChange,
                            c.RequestedCorrectionDetails,
                            c.FormName,
                             c.Suggestions,


                     createdby.Count > 0 ? createdby[0].FirstName : c.CreatedBy,

                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=EmrEntry.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveEmrEntry(EmrEntryModel Model)
        {
            if (Model.EmrEntryId == 0)
            {
                tbl_EmrEntry Entry = new tbl_EmrEntry
                {
                    DateOfChange = ProjectConvert.ConverDateStringtoDatetime(Model.DateOfChange),
                    DateofError = ProjectConvert.ConverDateStringtoDatetime(Model.DateofError),
                    FormName = Model.FormName,
                    LocationId = Model.LocationId,
                    Mobile = Model.Mobile,
                    MrNo_PatientName = Model.MrNo,
                    ReasonForChange = Model.ReasonForChange,
                    RequestedCorrectionDetails = Model.RequestedCorrectionDetails,
                    Suggestions = Model.Suggestions,
                    UserId = Model.UserId,
                    IsActive = true,
                    PatientName = Model.PatientName,
                    DoctorName = Model.DoctorName,
                    CreatedBy = User.Identity.Name,
                    CreatedOn = DateTime.Now,
                    ModifedBy = User.Identity.Name,
                    ModifiedOn = DateTime.Now
                };
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                string mailbody = "<p style='font-family:verdana'>HI Team,";
                mailbody += "<p style='font-family:verdana'> The " + Entry.UserId + " Emr Entry is created.</p>";
                mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Entry.UserId + "</td></tr>";
                //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].EmpName + "</td></tr>";
                //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee Department </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Department.DepartmentName + "</td></tr>";
                //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Email</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Email + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Mobile</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.Mobile + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>MrNo</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.MrNo + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Patient Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.PatientName + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Doctor Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.DoctorName + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Date Of Error</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.DateofError + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Date Of Change </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.DateOfChange + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Reason For Change</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.ReasonForChange + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>RequestedCorrectionDetails</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.RequestedCorrectionDetails + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Form Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.FormName + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Suggestion to avoid</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.Suggestions + "</td></tr>";
                //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Remarks</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].StatusRemarks + "</td></tr>";

                mailbody += "</table>";
                mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
                mailmodel.body = mailbody;
                List<tbl_Settings> list = myapp.tbl_Settings.Where(X => X.SettingKey.Contains(Model.LocationId + "EmrEntryApprover")).ToList();
                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                mailmodel.subject = "Emr Entry";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "EMR Entry";
                mailmodel.ccemail = "";
                string locid = Model.LocationId.ToString();
                foreach (tbl_Settings v in list)
                {
                    if (v.SettingValue != null && v.SettingValue != "")
                    {
                        if (v.SettingKey == locid + "EmrEntryApprover1")
                        {
                            Entry.ApprovalId1 = v.SettingValue;
                            Entry.IsApproved1 = false;
                        }
                        if (v.SettingKey == locid + "EmrEntryApprover2")
                        {
                            Entry.ApprovalId2 = v.SettingValue;
                            Entry.IsApproved2 = false;
                        }
                        if (v.SettingKey == locid + "EmrEntryApprover3")
                        {
                            Entry.ApprovalId3 = v.SettingValue;
                            Entry.IsApproved3 = false;
                        }
                        if (v.SettingKey == locid + "EmrEntryApprover4")
                        {
                            Entry.ApprovalId4 = v.SettingValue;
                            Entry.IsApproved4 = false;
                        }
                        if (v.SettingKey == locid + "EmrEntryApprover5")
                        {
                            Entry.ApprovalId5 = v.SettingValue;
                            Entry.IsApproved5 = false;
                        }
                        if (v.SettingKey == locid + "EmrEntryApprover6")
                        {
                            Entry.ApprovalId6 = v.SettingValue;
                            Entry.IsApproved6 = false;
                        }

                        if (v.SettingKey == locid + "EmrEntryApproverEmailtoSend1")
                        {
                            mailmodel.ccemail += v.SettingValue + ",";

                        }
                        if (v.SettingKey == locid + "EmrEntryApproverEmailtoSend2")
                        {
                            mailmodel.ccemail += v.SettingValue + ",";
                        }
                        if (v.SettingKey == locid + "EmrEntryApproverEmailtoSend3")
                        {
                            mailmodel.ccemail += v.SettingValue + ",";
                        }
                        if (v.SettingKey == locid + "EmrEntryApproverEmailtoSend4")
                        {
                            mailmodel.ccemail += v.SettingValue + ",";
                        }
                    }
                }
                myapp.tbl_EmrEntry.Add(Entry);
                myapp.SaveChanges();
                mailmodel.toemail = "edward.c@fernandez.foundation";
                mailmodel.username = "EMR Entry";
                mailmodel.fromemail = "EmrEntry@fernandez.foundation";
                cm.SendEmail(mailmodel);
            }
            else
            {
                List<tbl_EmrEntry> Entry = myapp.tbl_EmrEntry.Where(l => l.EmrEntryId == Model.EmrEntryId).ToList();
                Entry[0].DateOfChange = ProjectConvert.ConverDateStringtoDatetime(Model.DateOfChange);
                Entry[0].DateofError = ProjectConvert.ConverDateStringtoDatetime(Model.DateofError);
                Entry[0].FormName = Model.FormName;
                Entry[0].PatientName = Model.PatientName;
                Entry[0].DoctorName = Model.DoctorName;
                //Entry[0].LocationId = Model.LocationId;
                Entry[0].Mobile = Model.Mobile;
                Entry[0].MrNo_PatientName = Model.MrNo;
                Entry[0].ReasonForChange = Model.ReasonForChange;
                Entry[0].RequestedCorrectionDetails = Model.RequestedCorrectionDetails;
                Entry[0].Suggestions = Model.Suggestions;
                //Entry[0].UserId = Model.UserId;
                myapp.SaveChanges();
            }
            return Json("Added Successfully", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetWaitingForApprovalEmrEntry(JQueryDataTableParamModel param)
        {
            string UserId = User.Identity.Name;
            UserId = UserId.ToLower().Replace("fh_", "");
            List<tbl_EmrEntry> query = myapp.tbl_EmrEntry.Where(l => l.ApprovalId1 == UserId
            || l.ApprovalId2 == UserId
            || l.ApprovalId3 == UserId
            || l.ApprovalId4 == UserId
            || l.ApprovalId5 == UserId
            || l.ApprovalId6 == UserId
            ).OrderByDescending(l => l.EmrEntryId).ToList();
            if (param.locationid != null && param.locationid != 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.Emp != null && param.Emp != "")
            {
                query = query.Where(l => l.UserId == param.Emp).ToList();
            }
            IEnumerable<tbl_EmrEntry> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.EmrEntryId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.UserId.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_EmrEntry> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies

                                           select new object[] {
                            c.EmrEntryId,
                             (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).FirstOrDefault(),
                            (from var in myapp.tbl_User where var.CustomUserId == c.UserId select var.FirstName).FirstOrDefault(),
                            c.MrNo_PatientName, c.PatientName,c.DoctorName,
                            c.Mobile,
                            c.DateofError.HasValue ? c.DateofError.Value.ToString("dd-MM-yyyy") : "",
                             c.DateOfChange.HasValue ? c.DateOfChange.Value.ToString("dd-MM-yyyy") : "",
                            c.ReasonForChange,
                            c.RequestedCorrectionDetails,
                            c.FormName,
                             c.Suggestions,
                           c.IsActive.HasValue?c.IsActive.ToString():"false",


                            c.EmrEntryId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        [Authorize]
        public ActionResult EmrEntry()
        {
            ViewBag.User = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).SingleOrDefault();
            return View();
        }
        [Authorize]
        public ActionResult ApproveEmrEntry()
        {
            return View();
        }
        public JsonResult JsonApproveEmrEntry(int id)
        {
            tbl_EmrEntry Model = myapp.tbl_EmrEntry.Where(X => X.EmrEntryId == id).SingleOrDefault();
            if (Model.ApprovalId1 == User.Identity.Name)
            {
                Model.IsApproved1 = true;
            }
            myapp.SaveChanges();
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel();
            string mailbody = "<p style='font-family:verdana'>HI Team,";
            mailbody += "<p style='font-family:verdana'> The " + User.Identity.Name + " has Approved the Emr Entr.</p>";
            mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
            mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.CreatedBy + "</td></tr>";
            //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].EmpName + "</td></tr>";
            //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee Department </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Department.DepartmentName + "</td></tr>";
            //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Email</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Email + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Mobile</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.Mobile + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>MrNo</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.MrNo_PatientName + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Patient Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.PatientName + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Doctor Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.DoctorName + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Date Of Error</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.DateofError + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Date Of Change </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.DateOfChange + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Reason For Change</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.ReasonForChange + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>RequestedCorrectionDetails</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.RequestedCorrectionDetails + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Form Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.FormName + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Suggestion to avoid</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Model.Suggestions + "</td></tr>";
            //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Remarks</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].StatusRemarks + "</td></tr>";

            mailbody += "</table>";
            mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
            mailmodel.body = mailbody;
            List<tbl_Settings> list = myapp.tbl_Settings.Where(X => X.SettingKey.Contains(Model.LocationId + "EmrEntryApprover")).ToList();

            foreach (tbl_Settings v in list)
            {
                if (v.SettingValue != null && v.SettingValue != "")
                {
                    if (v.SettingKey == Model.LocationId + "EmrEntryApproverEmailtoSend1")
                    {
                        mailmodel.ccemail += v.SettingValue + ",";

                    }
                    if (v.SettingKey == Model.LocationId + "EmrEntryApproverEmailtoSend2")
                    {
                        mailmodel.ccemail += v.SettingValue + ",";
                    }
                    if (v.SettingKey == Model.LocationId + "EmrEntryApproverEmailtoSend3")
                    {
                        mailmodel.ccemail += v.SettingValue + ",";
                    }
                    if (v.SettingKey == Model.LocationId + "EmrEntryApproverEmailtoSend4")
                    {
                        mailmodel.ccemail += v.SettingValue + ",";
                    }
                }
            }
            mailmodel.filepath = "";
            mailmodel.subject = "Emr Entry";
            //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
            mailmodel.fromname = "EMR Entry";
            //mailmodel.ccemail = "";

            mailmodel.toemail = "edward.c@fernandez.foundation";
            mailmodel.username = "EMR Entry";
            mailmodel.fromemail = "EmrEntry@fernandez.foundation";
            cm.SendEmail(mailmodel);
            return Json("Success");
        }
        public JsonResult GetEmrEntrybyId(int EmrEntryId)
        {
            tbl_EmrEntry query = myapp.tbl_EmrEntry.Where(X => X.EmrEntryId == EmrEntryId).SingleOrDefault();
            EmrEntryViewModel model = new EmrEntryViewModel()
            {
                Approval1Comments = query.Approval1Comments,
                Approval2Comments = query.Approval2Comments,
                Approval3Comments = query.Approval3Comments,
                Approval4Comments = query.Approval4Comments,
                Approval5Comments = query.Approval5Comments,
                Approval6Comments = query.Approval6Comments,
                Approval7Comments = query.Approval7Comments,
                ApprovalId1 = query.ApprovalId1,
                ApprovalId2 = query.ApprovalId2,
                ApprovalId3 = query.ApprovalId3,
                ApprovalId4 = query.ApprovalId4,
                ApprovalId5 = query.ApprovalId5,
                ApprovalId6 = query.ApprovalId6,
                ApprovalId7 = query.ApprovalId7,
                CreatedBy = query.CreatedBy,
                CreatedOn = query.CreatedOn.HasValue ? ProjectConvert.ConverDateTimeToString(query.CreatedOn.Value) : "",
                DateOfChange = query.DateOfChange.HasValue ? ProjectConvert.ConverDateTimeToString(query.DateOfChange.Value) : "",
                DateofError = query.DateofError.HasValue ? ProjectConvert.ConverDateTimeToString(query.DateofError.Value) : "",
                EmrEntryId = query.EmrEntryId,
                FormName = query.FormName,
                IsActive = query.IsActive,
                IsApproved1 = query.IsApproved1,
                IsApproved2 = query.IsApproved2,
                IsApproved3 = query.IsApproved3,
                IsApproved4 = query.IsApproved4,
                IsApproved5 = query.IsApproved5,
                IsApproved6 = query.IsApproved6,
                IsApproved7 = query.IsApproved7,
                LocationId = query.LocationId,
                Mobile = query.Mobile,
                ModifedBy = query.ModifedBy,
                ModifiedOn = query.ModifiedOn.HasValue ? ProjectConvert.ConverDateTimeToString(query.ModifiedOn.Value) : "",
                MrNo_PatientName = query.MrNo_PatientName,
                ReasonForChange = query.ReasonForChange,
                RequestedCorrectionDetails = query.RequestedCorrectionDetails,
                Suggestions = query.Suggestions,
                UserId = query.UserId
            };


            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteEmrEntry(int EmrEntryId)
        {
            tbl_EmrEntry v = myapp.tbl_EmrEntry.Where(a => a.EmrEntryId == EmrEntryId).FirstOrDefault();
            if (v != null)
            {
                myapp.tbl_EmrEntry.Remove(v);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        #endregion
        public JsonResult SendBrandedMerchandise(string details)
        {
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel();
            EmailTeamplates emailtemp = new EmailTeamplates();
            mailmodel.fromemail = "Leave@hospitals.com";
            mailmodel.toemail = "vamsirm26@gmail.com";
            mailmodel.ccemail = "abhishek_sengupta@fernandez.foundation";
            mailmodel.subject = "Branded Merchandise Request from " + details.Split(',')[1] + " on " + details.Split(',')[2];//Emp id
            string mailbody = "<p style='font-family:verdana'>HI Team,";
            mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
            mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Employee Id</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + details.Split(',')[0] + "</td></tr>";//Dept
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'> Employee Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + details.Split(',')[1] + "</td></tr>";//Emp name
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Brand Product </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + details.Split(',')[2] + "</td></tr>";//Emp id
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Required quantity</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + details.Split(',')[3] + "</td></tr>";//Required quntity
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Comments</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + details.Split(',')[4] + "</td></tr>";//Comments
            mailbody += "</table>";
            mailmodel.body = mailbody;
            mailmodel.fromname = "Branded Merchandise";
            cm.SendEmail(mailmodel);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult HRTrainingCalendar()
        {
            var excelSheets = GetTrainingDepartments();
            return View(excelSheets);
        }
        public ActionResult GetTrainingDepartmentsToBind()
        {
            var excelSheets = GetTrainingDepartments();
            return Json(excelSheets, JsonRequestBehavior.AllowGet);
        }
        public List<string> GetTrainingDepartments()
        {
            var PathName = Path.Combine(Server.MapPath("~/Documents/"), "Department_Wise_Training_Calendar_2021.xlsx");
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
            //OleDbCommand oledbcmd = new OleDbCommand(myexceldataquery, oledbconn);
            oledbconn.Open();
            DataTable dt = oledbconn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

            if (dt == null)
            {
                return null;
            }

            List<string> excelSheets = new List<string>();


            // Add the sheet name to the string array.
            foreach (DataRow row in dt.Rows)
            {
                string name = row["TABLE_NAME"].ToString().Replace("$", "").Replace("'", "");
                if (!name.Contains("_xlnm"))
                {
                    excelSheets.Add(name);
                }
            }
            oledbconn.Close();
            oledbconn.Dispose();
            dt.Dispose();
            return excelSheets;
        }

        public ActionResult GetTrainingCalenderSheetData(string sheetname)
        {

            var PathName = Path.Combine(Server.MapPath("~/Documents/"), "Department_Wise_Training_Calendar_2021.xlsx");

            string myexceldataquery = "select * from [" + sheetname + "$]";
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

            var list = JsonConvert.SerializeObject(ds.Tables[0], Formatting.None, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            });
            return Content(list, "application/json");
        }
        //[AllowAnonymous]
        public ActionResult FHNDA()
        {
            return View();
        }
        public ActionResult PrintFHNDA(string name, string designation, string department, bool accept)
        {
            ViewBag.name = name;
            ViewBag.designation = designation;
            ViewBag.department = department;
            ViewBag.accept = accept;
            return View();
        }
        public ActionResult SaveNDA(string name, string designation, string department, bool accept)
        {
            tbl_FhNda model = new tbl_FhNda();
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            model.Name = name;
            model.Designation = designation;
            model.Departmnet = department;
            model.IsAccepted = accept;
            myapp.tbl_FhNda.Add(model);
            myapp.SaveChanges();
            SendSms sms = new SendSms();
            sms.SendSmsToEmployee(designation, "Dear " + name + " your acceptance of  NDA agreement has been submitted successfully. Thankyou. Tanishsoft Hrms.");
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageFHNDA()
        {
            return View();
        }
        public ActionResult AjaxGetFHNDA(JQueryDataTableParamModel param)
        {
            var query = myapp.tbl_FhNda.ToList();

            IEnumerable<tbl_FhNda> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c =>
                    c.Id.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.Departmnet != null && c.Departmnet.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Designation != null && c.Designation.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.CreatedOn != null && c.CreatedOn.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies

                         select new object[] {
                             c.Id.ToString(),

                             c.Name,
                             c.Departmnet,
                             c.Designation,
                             c.CreatedOn.Value.ToString("dd/MM/yyyy")
                           };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        [AllowAnonymous]
        public ActionResult ConsentForm()
        {
            return View();
        }
        public ActionResult SaveConsentForm(tbl_ConsentForm model)
        {
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myapp.tbl_ConsentForm.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageConsentForm()
        {
            return View();
        }
        public ActionResult AjaxGetConsentForm(JQueryDataTableParamModel param)
        {
            var query = myapp.tbl_ConsentForm.ToList();

            IEnumerable<tbl_ConsentForm> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c =>
                    c.Id.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.Address != null && c.Address.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.S_Or_W_Off != null && c.S_Or_W_Off.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.CreatedOn != null && c.CreatedOn.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies

                         select new object[] {
                             c.Id.ToString(),

                             c.Name,
                             c.S_Or_W_Off,
                             c.Address,
                             c.CustomDate,
                             c.ContactNumber,
                             c.CreatedOn.Value.ToString("dd/MM/yyyy")
                           };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Covid19Training()
        {
            return View();
        }
        public ActionResult PdfPreview(int id)
        {
            var model = myapp.tbl_Protocol.Where(l => l.Id == id).SingleOrDefault();
            return View(model);
        }
        public ActionResult CLSBLSTrainingVideo()
        {
            return View();
        }
        public ActionResult DoctorsPrivilges()
        {
            return View();
        }
        public ActionResult WhistleBlowerPolicy()
        {
            return View();
        }
        public ActionResult ProbationsAndConfirmations()
        {
            return View();
        }
        public ActionResult TransferProcess()
        {
            return View();
        }
        public ActionResult MaternityLeaveProcess()
        {
            return View();
        }
        public ActionResult OccupationalHealthHazard()
        {
            return View();
        }
        public ActionResult PIPProcess()
        {
            return View();
        }
        public ActionResult GenerateSignature()
        {
            return View();
        }
        public async Task<ActionResult> UploadExcel_ForSignature(HttpPostedFileBase ExcelFileData)
        {
            string ReturnMsg = "";
            if (ExcelFileData != null)
            {
                try
                {
                    string DateNow = DateTime.Now.ToString("dd/MM/yyyy");
                    string NewGUID = Convert.ToString(Guid.NewGuid());
                    string CurrentDateTime = DateTime.Now.ToString("dd/MM/yyyy/hh/mm/ss/tt", DateTimeFormatInfo.InvariantInfo);
                    CurrentDateTime = (CurrentDateTime.Contains("/")) ? CurrentDateTime.Replace("/", "_") : CurrentDateTime;
                    string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + "_" + CurrentDateTime + "." + Path.GetExtension(ExcelFileData.FileName);
                    var PathName = Path.Combine(Server.MapPath("~/ExcelUplodes/"), UniqueFileName);
                    ExcelFileData.SaveAs(PathName);
                    string myexceldataquery = "select EmpId,Name,Designation,Contactnumber,Extension,Emailid,Location,Department from [sheet1$]";
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
                    oledbconn.Open();
                    OleDbDataReader DR = oledbcmd.ExecuteReader();
                    System.IO.DirectoryInfo di = new DirectoryInfo(Server.MapPath("~/Documents/EmpSignatureTemplate/"));

                    foreach (FileInfo file in di.GetFiles())
                    {
                        file.Delete();
                    }
                    while (DR.Read())
                    {
                        string empname = Convert.ToString(DR["Name"]);
                        string empid = Convert.ToString(DR["EmpId"]);
                        string Designation = Convert.ToString(DR["Designation"]);
                        string Extension = Convert.ToString(DR["Extension"]);
                        string Department = Convert.ToString(DR["Department"]);
                        string Contactnumber = Convert.ToString(DR["Contactnumber"]);
                        string Emailid = Convert.ToString(DR["Emailid"]);
                        string Location = Convert.ToString(DR["Location"]);
                        if (empname != null && empname != "" && empid != null && empid != "")
                        {
                            string userid = empid;
                            string body = "";
                            using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/EmailSignatureTemplate.html")))
                            {
                                body = reader.ReadToEnd();
                            }
                            body = body.Replace("{{NAME}}", empname);
                            body = body.Replace("{{Designation}}", Designation);
                            body = body.Replace("{{Department}}", Department);
                            body = body.Replace("{{Location}}", Location);
                            body = body.Replace("{{Extension}}", Extension);
                            body = body.Replace("{{Email}}", Emailid);
                            body = body.Replace("{{Contactnumber}}", Contactnumber);
                            var template = Path.Combine(Server.MapPath("~/Documents/EmpSignatureTemplate/"), empid + ".html");
                            System.IO.File.WriteAllText(template, body);
                        }
                    }
                    oledbconn.Close();
                    string[] filePaths = Directory.GetFiles(Server.MapPath("~/Documents/EmpSignatureTemplate/"));
                    ReturnMsg = "Successfully Uploaded";
                    if (System.IO.File.Exists(PathName))
                    {
                        System.IO.File.Delete(PathName);
                    }
                    using (ZipFile zip = new ZipFile())
                    {
                        zip.AlternateEncodingUsage = ZipOption.AsNecessary;
                        zip.AddDirectoryByName("Files");
                        foreach (var file in filePaths)
                        {

                            zip.AddFile(file, "Files");

                        }
                        string zipName = String.Format("EmployeeSignatures_{0}.zip", DateTime.Now.ToString("yyyy-MMM-dd-HHmmss"));
                        using (MemoryStream memoryStream = new MemoryStream())
                        {
                            zip.Save(memoryStream);
                            return File(memoryStream.ToArray(), "application/zip", zipName);
                        }
                    }

                }
                catch (Exception EX)
                {

                    ReturnMsg = "Error Occured. Please Try Again Later !";
                }
            }
            TempData["UploadImportMsg"] = ReturnMsg;
            return RedirectToAction("EmployeeLeaves", "HrAdmin");
        }
        [AllowAnonymous]
        public ActionResult FFYearlyStats()
        {
            return View();
        }
        public ActionResult AttendanceManagementPolicy()
        {
            return View();
        }
        public ActionResult OvertimePolicy()
        {
            return View();
        }
        public ActionResult UniformsDressCode()
        {
            return View();
        }
        public ActionResult EmployeeWellnessProgram()
        {
            return View();
        }
        public ActionResult LearningandDevelopment()
        {
            return View();
        }
        public ActionResult OutstationTravelPolicy()
        {
            return View();
        }
        public ActionResult AccommodationAllotment()
        {
            return View();
        }
        public ActionResult Gratitudes(int id)
        {
            var sublist = myapp.tbl_Img_GalleryImages.Where(a => a.GalleryId == id).ToList();
            return View(sublist);
        }
       
      
        public ActionResult LocalConveyance()
        {
            return View();
        }
        public ActionResult ClinicalConsultantsOffDays() {
            return View();
        }
        public ActionResult SuperannuationAndPostRetirement()
        {
            return View();
        }
    }
}