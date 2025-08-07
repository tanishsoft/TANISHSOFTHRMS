
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
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
using WebApplicationHsApp.Models.Biomedical;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class BiomedicalController : Controller
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Biomedical
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> Dashboard()
        {
            ViewBag.AssetCount = await myapp.tbl_bm_Asset.CountAsync();
            return View();
        }
        public ActionResult CreateDailyRounds(int id = 0)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult ManageDailyRounds()
        {
            ViewBag.BioMedicalTeam = "No";
            var userdet = myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).SingleOrDefault();
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.EmpId == userdet.UserId).Count();
            if (inhcarge > 0)
            {
                ViewBag.BioMedicalTeam = "Yes";
            }
            return View();
        }
        public ActionResult CreateBreakDown(int id = 0)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult ManageBreakDown()
        {
            ViewBag.BioMedicalTeam = "No";
            var userdet = myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).SingleOrDefault();
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.EmpId == userdet.UserId).Count();
            if (inhcarge > 0)
            {
                ViewBag.BioMedicalTeam = "Yes";
            }
            return View();
        }
        public ActionResult CreatePM(int id = 0)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult ManagePM()
        {
            ViewBag.BioMedicalTeam = "No";
            var userdet = myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).SingleOrDefault();
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.EmpId == userdet.UserId).Count();
            if (inhcarge > 0)
            {
                ViewBag.BioMedicalTeam = "Yes";
            }
            return View();
        }
        public ActionResult BookPM()
        {
            return View();
        }
        public ActionResult CreateAsset(int id = 0)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult ManageAsset()
        {
            ViewBag.BioMedicalTeam = "No";
            var userdet = myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).SingleOrDefault();
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.EmpId == userdet.UserId).Count();
            if (inhcarge > 0)
            {
                ViewBag.BioMedicalTeam = "Yes";
            }
            return View();
        }
        public ActionResult CreateShiftingRequest(int id = 0)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult ManageShiftingRequest()
        {
            ViewBag.BioMedicalTeam = "No";
            var userdet = myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).SingleOrDefault();
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.EmpId == userdet.UserId).Count();
            if (inhcarge > 0)
            {
                ViewBag.BioMedicalTeam = "Yes";
            }
            return View();
        }
        public ActionResult CreateCheckList(int id = 0)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult ManageCheckList()
        {
            ViewBag.BioMedicalTeam = "No";
            var userdet = myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).SingleOrDefault();
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.EmpId == userdet.UserId).Count();
            if (inhcarge > 0)
            {
                ViewBag.BioMedicalTeam = "Yes";
            }
            return View();
        }
        public async Task<ActionResult> Save_Update_DailyRounds(DailyRoundsViewModel model, List<HttpPostedFileBase> UploadDocument)
        {
            try
            {
                tbl_bm_DailyRound _DailyRound = new tbl_bm_DailyRound();
                if (model.DailyRoundId != 0)
                {
                    _DailyRound = await myapp.tbl_bm_DailyRound.Where(m => m.DailyRoundId == model.DailyRoundId).FirstOrDefaultAsync();
                    _DailyRound.ModifiedBy = User.Identity.Name;
                    _DailyRound.ModifiedOn = DateTime.Now;
                }
                _DailyRound.AdminComments = model.AdminComments;
                _DailyRound.CallStatus = model.CallStatus;
                _DailyRound.DepartmentId = model.DepartmentId;
                if (model.EndDate != "" && model.EndDate != null)
                {
                    _DailyRound.EndDate = ProjectConvert.ConverDateStringtoDatetime(model.EndDate);
                    _DailyRound.EndTime = model.EndTime;
                }
                _DailyRound.IsActive = true;
                _DailyRound.LocationId = model.LocationId;
                _DailyRound.StartDate = ProjectConvert.ConverDateStringtoDatetime(model.StartDate);
                _DailyRound.StartTime = model.StartTime;
                _DailyRound.SubDepartmentId = model.SubDepartmentId;
                _DailyRound.WorkDone = model.WorkDone;
                _DailyRound.AdminComments = model.AdminComments;
                if (model.DailyRoundId == 0)
                {
                    _DailyRound.CreatedBy = User.Identity.Name;
                    _DailyRound.CreatedOn = DateTime.Now;
                    myapp.tbl_bm_DailyRound.Add(_DailyRound);
                    await myapp.SaveChangesAsync();
                    SendEmailDaliyRounds(_DailyRound);
                }
                else
                {
                    await myapp.SaveChangesAsync();
                }
                if (UploadDocument != null && UploadDocument.Count > 0)
                {
                    for (int i = 0; i < UploadDocument.Count; i++)
                    {
                        string fileNames = Path.GetFileNameWithoutExtension(UploadDocument[i].FileName);
                        string fileName = fileNames;
                        string extension = Path.GetExtension(UploadDocument[i].FileName);
                        string guidid = Guid.NewGuid().ToString();
                        fileName = fileName + guidid + extension;
                        UploadDocument[i].SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));
                        tbl_bm_Doument document = new tbl_bm_Doument();
                        document.CreatedBy = User.Identity.Name;
                        document.CreatedOn = DateTime.Now;
                        document.DocumentPath = fileName;
                        document.DocumentName = fileNames;
                        document.EntityId = _DailyRound.DailyRoundId;
                        document.EntityType = "Daily Rounds";
                        document.IsPrivate = false;
                        myapp.tbl_bm_Doument.Add(document);
                        await myapp.SaveChangesAsync();
                    }
                }

            }

            catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public void SendEmailDaliyRounds(tbl_bm_DailyRound model)
        {
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.LocationId && l.EmpType == "Incharge").SingleOrDefault();
            if (inhcarge != null)
            {
                var inchargedetails = myapp.tbl_User.Where(l => l.UserId == inhcarge.EmpId).SingleOrDefault();
                var allEngineer = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.LocationId && l.EmpType == "Engineer").Select(l => l.EmpId).ToList();
                var allEngineeretails = myapp.tbl_User.Where(l => allEngineer.Contains(l.UserId)).Select(l => l.EmailId).ToList();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = inchargedetails.EmailId;
                mailmodel.ccemail = string.Join(",", allEngineeretails);
                //mailmodel.toemail = "phanisrinivas111@gmail.com";
                //mailmodel.ccemail = "phanisrinivas111@gmail.com";
                mailmodel.subject = "New Daily Round is Created Daily Round Id : " + model.DailyRoundId + "";
                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_DailyRound.html")))
                {
                    body = reader.ReadToEnd();
                }
                var userdet = myapp.tbl_User.Where(l => l.CustomUserId == model.CreatedBy).SingleOrDefault();
                body = body.Replace("[[employee_name]]", userdet.FirstName);
                body = body.Replace("[[Location]]", (from v in myapp.tbl_Location where v.LocationId == model.LocationId select v.LocationName).SingleOrDefault());
                body = body.Replace("[[Department]]", (from v in myapp.tbl_Department where v.DepartmentId == model.DepartmentId select v.DepartmentName).SingleOrDefault());
                body = body.Replace("[[Subdepartment]]", (from v in myapp.tbl_subdepartment where v.SubDepartmentId == model.SubDepartmentId select v.Name).SingleOrDefault());
                body = body.Replace("[[WorkDone]]", model.WorkDone);
                body = body.Replace("[[CallStatus]]", model.CallStatus);
                if (model.StartDate != null)
                {
                    body = body.Replace("[[StartDate]]", model.StartDate.Value.ToString("dd/MM/yyyy") + " " + model.StartTime);
                }
                else
                {
                    body = body.Replace("[[StartDate]]", "");
                }
                if (model.EndDate != null)
                {
                    body = body.Replace("[[EndDate]]", model.EndDate.Value.ToString("dd/MM/yyyy") + " " + model.EndTime);
                }
                else
                {
                    body = body.Replace("[[EndDate]]", "");
                }
                body = body.Replace("[[VerifiedBy]]", "");
                body = body.Replace("[[Signature]]", "");
                mailmodel.body = body;
                mailmodel.filepath = "";
                mailmodel.fromname = "BioMedical - Helpdesk";
                cm.SendEmail(mailmodel);
            }
        }
        public async Task<ActionResult> GetDailyRounds(int id)
        {
            var dbModel = await myapp.tbl_bm_DailyRound.Where(m => m.DailyRoundId == id).FirstOrDefaultAsync();
            DailyRoundsViewModel model = new DailyRoundsViewModel();
            model.DailyRoundId = id;
            model.AdminComments = dbModel.AdminComments != null ? dbModel.AdminComments : "";
            model.CallStatus = dbModel.CallStatus;
            model.DepartmentId = dbModel.DepartmentId;
            //model.WorkDone = dbModel.WorkDone;
            if (dbModel.DepartmentId != null && dbModel.DepartmentId != 0)
            {
                model.DepartmentName = await (from var in myapp.tbl_Department where var.DepartmentId == model.DepartmentId select var.DepartmentName).SingleOrDefaultAsync();
            }
            if (dbModel.EndDate.HasValue)
                model.EndDate = dbModel.EndDate.Value.ToString("dd-MM-yyyy");
            else
                model.EndDate = "";
            model.StartDate = dbModel.StartDate.Value.ToString("dd-MM-yyyy");
            model.EndTime = dbModel.EndTime;
            model.LocationId = dbModel.LocationId;
            if (dbModel.LocationId != null && dbModel.LocationId != 0)
            {
                model.LocationName = await (from var in myapp.tbl_Location where var.LocationId == dbModel.LocationId select var.LocationName).SingleOrDefaultAsync();
            }
            model.StartTime = dbModel.StartTime;
            model.SubDepartmentId = dbModel.SubDepartmentId;
            if (dbModel.SubDepartmentId != null && dbModel.SubDepartmentId != 0)
            {
                model.SubDepartmentName = await (from var in myapp.tbl_subdepartment where var.SubDepartmentId == dbModel.SubDepartmentId select var.Name).SingleOrDefaultAsync();
            }
            model.WorkDone = dbModel.WorkDone != null ? dbModel.WorkDone : "";
            model.UserEmpId = dbModel.UserEmpId != null ? dbModel.UserEmpId : 0;
            if (dbModel.UserEmpId != null && dbModel.UserEmpId != 0)
            {
                var UserEmpId = dbModel.UserEmpId;
                model.UserEmpName = await (from var in myapp.tbl_User where var.UserId == UserEmpId select var.FirstName + " " + var.LastName).SingleOrDefaultAsync();
            }
            else
            {
                model.UserEmpName = "";
            }
            model.UserSignature = dbModel.UserSignature != null ? dbModel.UserSignature : "";
            model.UserStatus = dbModel.UserStatus != null ? dbModel.UserStatus : "";
            model.Document1 = dbModel.Document1 != null ? dbModel.Document1 : "";
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AjaxGetDailyRounds(JQueryDataTableParamModel param)
        {

            List<tbl_bm_DailyRound> query = await (from d in myapp.tbl_bm_DailyRound select d).ToListAsync();
            var userdet = myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).SingleOrDefault();
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.EmpId == userdet.UserId).Count();
            if (inhcarge > 0)
            {
            }
            else
            {
                query = query.Where(l => l.LocationId == userdet.LocationId).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (param.category != null && param.category != "")
            {
                query = query.Where(m => m.CallStatus.ToLower() == param.category.ToLower()).ToList();
            }
            if (param.locationid != null && param.locationid != 0)
            {
                query = query.Where(m => m.LocationId == param.locationid).ToList();
            }
            if (param.departmentid != null && param.departmentid != 0)
            {
                query = query.Where(m => m.DepartmentId == param.departmentid).ToList();
            }
            if (param.subdepartmentid != null && param.subdepartmentid != 0)
            {
                query = query.Where(m => m.SubDepartmentId == param.subdepartmentid).ToList();
            }
            if (param.Emp != null && param.Emp != "")
            {
                var empId = Convert.ToInt32(param.Emp);
                var id = await (from u in myapp.tbl_User where u.UserId == empId select u.CustomUserId).SingleOrDefaultAsync();
                query = query.Where(m => m.CreatedBy == id).ToList();
            }
            query = query.OrderByDescending(m => m.DailyRoundId).ToList();
            IEnumerable<tbl_bm_DailyRound> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.DailyRoundId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.CallStatus != null && c.CallStatus.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.DepartmentId != null && c.DepartmentId.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_bm_DailyRound> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                                           join d in myapp.tbl_Department on c.DepartmentId equals d.DepartmentId
                                           join sd in myapp.tbl_subdepartment on c.SubDepartmentId equals sd.SubDepartmentId
                                           join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                                           select new object[] {
                                             c.CreatedOn.Value.ToString("ddMMyyyy")+"_"+c.DailyRoundId.ToString(),
                                              l.LocationName,
                                              d.DepartmentName +"-"+sd.Name,
                                              u.FirstName,
                                              c.CallStatus,
                                              c.StartDate.HasValue?c.StartDate.Value.ToString("dd/MM/yyyy")+" "+c.StartTime:"",
                                              c.EndDate.HasValue?c.EndDate.Value.ToString("dd/MM/yyyy")+" "+c.EndTime:"",
                                              //c.WorkDone,
                                              //c.Document1!=null?c.Document1:"",
                                              c.UserStatus!=null && c.UserStatus=="Done"? "Yes - "+myapp.tbl_User.Where(su=>su.UserId==c.UserEmpId).SingleOrDefault().FirstName:"No",
                                              //c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.DailyRoundId.ToString()+"-"+c.CallStatus+"-"+c.UserStatus
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> Save_Update_BreakDown(BreakDownViewModel model)
        {
            try
            {
                tbl_bm_BreakDown _BreakDown = new tbl_bm_BreakDown();
                if (model.BreakDownId != 0)
                {
                    _BreakDown = await myapp.tbl_bm_BreakDown.Where(m => m.BreakDownId == model.BreakDownId).FirstOrDefaultAsync();
                    _BreakDown.ModifiedBy = User.Identity.Name;
                    _BreakDown.ModifiedOn = DateTime.Now;
                }
                _BreakDown.CallStatus = "pending";
                _BreakDown.AssetNumber = model.AssetNumber;
                _BreakDown.EquipmentName = model.EquipmentName.Trim();
                _BreakDown.ProblemReported = model.ProblemReported;
                _BreakDown.DepartmentId = model.DepartmentId;
                _BreakDown.LocationId = model.LocationId;

                _BreakDown.IsActive = true;
                _BreakDown.LocationId = model.LocationId;
                _BreakDown.SubDepartmentId = model.SubDepartmentId;

                if (model.BreakDownId == 0)
                {
                    _BreakDown.CreatedBy = User.Identity.Name;
                    _BreakDown.CreatedOn = DateTime.Now;
                    myapp.tbl_bm_BreakDown.Add(_BreakDown);
                    await myapp.SaveChangesAsync();
                    SendEmailBreakDown(_BreakDown);
                }
                else
                {
                    await myapp.SaveChangesAsync();
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        public void SendEmailBreakDown(tbl_bm_BreakDown model)
        {
            var teamdetails = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.LocationId && (l.EmpType == "Incharge" || l.EmpType == "Engineer")).ToList();
            if (teamdetails != null && teamdetails.Count > 0)
            {
                try
                {
                    string mobilenumber = String.Join(",", myapp.tbl_bm_TeamSetup.Where(l => l.EmpMobile != null && l.EmpMobile != "" && l.LocationId == model.LocationId && (l.EmpType == "Incharge" || l.EmpType == "Engineer")).Select(l => l.EmpMobile).Distinct().ToList());
                    SendSms sms = new SendSms();
                    if (mobilenumber != null && mobilenumber != "")
                    {
                        string message = "Dear Biomedical Team, You have a new breakdown request ID NO " + model.BreakDownId + " pending for service, please login to Infonet to view the details. Tanishsoft Hrms";
                        sms.SendSmsToEmployee(mobilenumber, message);
                    }
                }
                catch { }
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                var userdet = myapp.tbl_User.Where(l => l.CustomUserId == model.CreatedBy).SingleOrDefault();
                mailmodel.fromemail = "BreakDown@fernandez.foundation";
                mailmodel.toemail = userdet.EmailId;
                mailmodel.ccemail = string.Join(",", teamdetails.Select(e => e.EmpEmail).ToList());
                mailmodel.subject = "New Break down is Created. Id is " + model.BreakDownId + "";
                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_BreakDown.html")))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("[[employee_name]]", userdet.FirstName);
                body = body.Replace("[[Location]]", (from v in myapp.tbl_Location where v.LocationId == model.LocationId select v.LocationName).SingleOrDefault());
                body = body.Replace("[[Department]]", (from v in myapp.tbl_Department where v.DepartmentId == model.DepartmentId select v.DepartmentName).SingleOrDefault());
                body = body.Replace("[[Subdepartment]]", (from v in myapp.tbl_subdepartment where v.SubDepartmentId == model.SubDepartmentId select v.Name).SingleOrDefault());
                body = body.Replace("[[AssetNumber]]", model.AssetNumber);
                body = body.Replace("[[EquipmentName]]", model.EquipmentName);
                if (model.WorkDone != null && model.WorkDone != "")
                    body = body.Replace("[[WorkDone]]", model.WorkDone);
                else
                    body = body.Replace("[[WorkDone]]", "");
                body = body.Replace("[[CallStatus]]", model.CallStatus);
                body = body.Replace("[[ProblemReported]]", model.ProblemReported);
                if (model.StartDate != null)
                {
                    body = body.Replace("[[StartDate]]", model.StartDate.Value.ToString("dd/MM/yyyy") + " " + model.StartTime);
                }
                else
                {
                    body = body.Replace("[[StartDate]]", "");
                }
                if (model.EndDate != null)
                {
                    body = body.Replace("[[EndDate]]", model.EndDate.Value.ToString("dd/MM/yyyy") + " " + model.EndTime);
                }
                else
                {
                    body = body.Replace("[[EndDate]]", "");
                }
                body = body.Replace("[[VerifiedBy]]", "");
                body = body.Replace("[[Signature]]", "");
                body = body.Replace("[[ProblemObserved]]", "");
                body = body.Replace("[[FaultyAccessoryOrSpare]]", "");
                body = body.Replace("[[SN_FaultyAccessoryOrSpare]]", "");
                body = body.Replace("[[AccessoryOrSpareReplaced]]", "");
                body = body.Replace("[[SN_AccessoryOrSpareReplaced]]", "");
                mailmodel.body = body;
                mailmodel.filepath = "";
                mailmodel.fromname = "BioMedical - Helpdesk";
                cm.SendEmail(mailmodel);
            }
        }
        public JsonResult Save_Update_NeedInfoBreakDown(BreakDownViewModel model, HttpPostedFileBase UploadDocument)
        {
            tbl_bm_BreakDown bd = new tbl_bm_BreakDown();
            if (model.BreakDownId != 0)
            {
                bd = myapp.tbl_bm_BreakDown.Where(m => m.BreakDownId == model.BreakDownId).FirstOrDefault();
                bd.ModifiedBy = User.Identity.Name;
                bd.ModifiedOn = DateTime.Now;
            }
            bd.AdminComments = model.AdminComments;
            bd.CallStatus = model.CallStatus;
            bd.SN_AccessoryOrSpareReplaced = model.SN_AccessoryOrSpareReplaced;
            bd.FaultyAccessoryOrSpare = model.FaultyAccessoryOrSpare;
            bd.AccessoryOrSpareReplaced = model.AccessoryOrSpareReplaced;
            if (model.EndDate != null && model.EndDate != "")
            {
                bd.EndDate = ProjectConvert.ConverDateStringtoDatetime(model.EndDate);
                bd.EndTime = model.EndTime;
            }

            bd.IsActive = true;
            bd.SN_FaultyAccessoryOrSpare = model.SN_FaultyAccessoryOrSpare;
            bd.StartDate = ProjectConvert.ConverDateStringtoDatetime(model.StartDate);
            bd.StartTime = model.StartTime;
            bd.ProblemObserved = model.ProblemObserved;
            bd.WorkDone = model.WorkDone;
            bd.AdminComments = model.AdminComments;
            if (UploadDocument != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(UploadDocument.FileName);
                string extension = Path.GetExtension(UploadDocument.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;
                bd.Document1 = fileName;
                UploadDocument.SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));

            }

            try
            {
                myapp.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBreakDown(int id)
        {
            var dbModel = myapp.tbl_bm_BreakDown.Where(m => m.BreakDownId == id).FirstOrDefault();
            BreakDownViewModel model = new BreakDownViewModel();
            model.BreakDownId = id;
            model.AssetId = dbModel.AssetId;
            model.UserEmpId = dbModel.UserEmpId;
            if (dbModel.UserEmpId != null && dbModel.UserEmpId != 0)
            {
                var UserEmpId = dbModel.UserEmpId;
                model.UserEmpName = (from var in myapp.tbl_User where var.UserId == UserEmpId select var.FirstName + " " + var.LastName).SingleOrDefault();
            }
            else
            {
                model.UserEmpName = "";
            }
            model.UserComments = dbModel.UserComments;
            model.UserSignature = dbModel.UserSignature != null ? dbModel.UserSignature : "";
            model.ProblemReported = dbModel.ProblemReported != null ? dbModel.ProblemReported : "";
            model.UserStatus = dbModel.UserStatus != null ? dbModel.UserStatus : "";
            model.AssetNumber = dbModel.AssetNumber != null ? dbModel.AssetNumber : "";
            model.EquipmentName = dbModel.EquipmentName != null ? dbModel.EquipmentName : "";
            model.DepartmentId = dbModel.DepartmentId;
            if (dbModel.DepartmentId != null && dbModel.DepartmentId != 0)
            {
                model.DepartmentName = (from var in myapp.tbl_Department where var.DepartmentId == model.DepartmentId select var.DepartmentName).SingleOrDefault();
            }
            model.LocationId = dbModel.LocationId;
            if (dbModel.LocationId != null && dbModel.DepartmentId != 0)
            {
                model.LocationName = (from var in myapp.tbl_Location where var.LocationId == model.LocationId select var.LocationName).SingleOrDefault();
            }
            model.SubDepartmentId = dbModel.SubDepartmentId;
            if (dbModel.SubDepartmentId != null && dbModel.SubDepartmentId != 0)
            {
                model.SubDepartmentName = (from var in myapp.tbl_subdepartment where var.SubDepartmentId == model.SubDepartmentId select var.Name).SingleOrDefault();
            }
            model.ProblemObserved = dbModel.ProblemObserved != null ? dbModel.ProblemObserved : "";
            model.FaultyAccessoryOrSpare = dbModel.FaultyAccessoryOrSpare != null ? dbModel.FaultyAccessoryOrSpare : "";
            model.SN_FaultyAccessoryOrSpare = dbModel.SN_FaultyAccessoryOrSpare != null ? dbModel.SN_FaultyAccessoryOrSpare : "";
            model.WorkDone = dbModel.WorkDone != null ? dbModel.WorkDone : "";
            model.AccessoryOrSpareReplaced = dbModel.AccessoryOrSpareReplaced != null ? dbModel.AccessoryOrSpareReplaced : "";
            model.SN_AccessoryOrSpareReplaced = dbModel.SN_AccessoryOrSpareReplaced != null ? dbModel.SN_AccessoryOrSpareReplaced : "";

            model.StartTime = dbModel.StartTime;
            if (dbModel.EndDate.HasValue)
                model.EndDate = dbModel.EndDate.Value.ToString("dd-MM-yyyy");
            else
                model.EndDate = "";
            if (dbModel.StartDate.HasValue)
                model.StartDate = dbModel.StartDate.Value.ToString("dd-MM-yyyy");
            model.EndTime = dbModel.EndTime;
            model.CallStatus = dbModel.CallStatus;
            model.Document1 = dbModel.Document1;
            model.AssignTo = dbModel.AssignTo.HasValue ? dbModel.AssignTo.Value : 0;
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetBreakDown(JQueryDataTableParamModel param)
        {
            List<tbl_bm_BreakDown> query = (from d in myapp.tbl_bm_BreakDown select d).ToList();
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (param.category != null && param.category != "")
            {
                query = query.Where(m => m.CallStatus.ToLower() == param.category.ToLower()).ToList();
            }
            int useridentityname = int.Parse(User.Identity.Name);
            var empid = (from u in myapp.tbl_User where u.EmpId == useridentityname select u.UserId).SingleOrDefault();
            var temusers = myapp.tbl_bm_TeamSetup.Where(l => l.EmpId == empid).ToList();
            if (!User.IsInRole("Admin"))
            {
                if (temusers.Count > 0)
                {
                    var roles = temusers.Select(l => l.EmpType).Distinct().ToList();
                    if (roles.Contains("Engineer"))
                    {
                        query = query.Where(m => (m.AssignTo == useridentityname || m.AssignTo == null || m.AssignTo == 0)).ToList();
                    }
                }
                else
                {
                    query = query.Where(l => l.CreatedBy == User.Identity.Name).ToList();
                }
            }
            if (param.Emp != null && param.Emp != "")
            {
                var empId = Convert.ToInt32(param.Emp);
                var id = (from u in myapp.tbl_User where u.UserId == empId select u.CustomUserId).SingleOrDefault();
                query = query.Where(m => m.CreatedBy == id).ToList();
            }
            if (param.locationid != null && param.locationid != 0)
            {
                query = query.Where(m => m.LocationId == param.locationid).ToList();
            }
            if (param.departmentid != null && param.departmentid != 0)
            {
                query = query.Where(m => m.DepartmentId == param.departmentid).ToList();
            }
            if (param.subdepartmentid != null && param.subdepartmentid != 0)
            {
                query = query.Where(m => m.SubDepartmentId == param.subdepartmentid).ToList();
            }
            if (param.equipment != null && param.equipment != "")
            {
                query = query.Where(m => m.EquipmentName.Trim() == param.equipment.Trim()).ToList();
            }
            if (param.AssetTypeId != null && param.AssetTypeId != 0)//AssetId
            {
                var id = Convert.ToString(param.AssetTypeId);
                query = query.Where(m => m.AssetNumber == id).ToList();
            }
            query = query.OrderByDescending(m => m.BreakDownId).ToList();
            IEnumerable<tbl_bm_BreakDown> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.BreakDownId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.AssetNumber != null && c.AssetNumber.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())
                              ||
                               c.DepartmentId != null && c.DepartmentId.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_bm_BreakDown> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           join loc in myapp.tbl_Location on c.LocationId equals loc.LocationId
                                           join dept in myapp.tbl_Department on c.DepartmentId equals dept.DepartmentId
                                           join sdept in myapp.tbl_subdepartment on c.SubDepartmentId equals sdept.SubDepartmentId
                                           select new object[] {
                                               c.CreatedOn.Value.ToString("ddMMyyyy")+"_"+c.BreakDownId.ToString(),
                                               loc.LocationName,
                                               dept.DepartmentName+" - "+sdept.Name,
                                               c.AssetNumber,
                                               c.EquipmentName,
                                               c.CreatedOn.HasValue?c.CreatedOn.Value.ToString("dd/MM/yyyy hh:mm tt") :"",
                                               c.StartDate.HasValue?c.StartDate.Value.ToString("dd/MM/yyyy")+" "+c.StartTime:"",
                                               c.EndDate.HasValue?c.EndDate.Value.ToString("dd/MM/yyyy")+" "+c.EndTime:"",
                                               c.AssignTo.HasValue?myapp.tbl_User.Where(su=>su.EmpId==c.AssignTo).SingleOrDefault().FirstName:"",
                                               c.CallStatus,
                                               c.UserStatus!=null && c.UserStatus=="Done"? "Yes - "+myapp.tbl_User.Where(su=>su.UserId==c.UserEmpId).SingleOrDefault().FirstName:"No",
                                              c.BreakDownId.ToString()+"-"+c.CallStatus+"-"+c.UserStatus+"-"+( c.StartDate.HasValue?"YES":"NO")
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> PickuptheBreakdown(int id)
        {
            tbl_bm_BreakDown query = await myapp.tbl_bm_BreakDown.Where(l => l.BreakDownId == id).SingleOrDefaultAsync();
            query.AssignTo = int.Parse(User.Identity.Name);
            await myapp.SaveChangesAsync();
            SendEmailAssinBreakDown(query);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> RejectTheBreakdownRequest(int id)
        {
            tbl_bm_BreakDown query = await myapp.tbl_bm_BreakDown.Where(l => l.BreakDownId == id).SingleOrDefaultAsync();
            query.CallStatus = "Rejected";
            await myapp.SaveChangesAsync();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> AssigntoBreakdown(int id, int empid)
        {
            tbl_bm_BreakDown query = await myapp.tbl_bm_BreakDown.Where(l => l.BreakDownId == id).SingleOrDefaultAsync();
            query.AssignTo = empid;
            await myapp.SaveChangesAsync();
            SendEmailAssinBreakDown(query);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public void SendEmailAssinBreakDown(tbl_bm_BreakDown model)
        {
            var user = myapp.tbl_User.Where(m => m.EmpId == model.AssignTo).FirstOrDefault();
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel();
            EmailTeamplates emailtemp = new EmailTeamplates();
            mailmodel.fromemail = "Leave@hospitals.com";
            var teamdetails1 = myapp.tbl_bm_TeamSetup.Where(l => l.EmpId == user.UserId).SingleOrDefault();
            var teamdetails = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.LocationId && l.EmpType == "Incharge").ToList();
            if (teamdetails != null && teamdetails.Count > 0)
            {
                mailmodel.ccemail = string.Join(",", teamdetails.Select(l => l.EmpEmail).ToList());
            }
            if (teamdetails1 != null)
            {
                try
                {
                    string mobilenumber = String.Join(",", myapp.tbl_bm_TeamSetup.Where(l => l.EmpMobile != null && l.EmpMobile != "" && l.LocationId == model.LocationId && (l.EmpType == "Incharge" || l.EmpType == "Engineer")).Select(l => l.EmpMobile).Distinct().ToList());
                    SendSms sms = new SendSms();
                    if (mobilenumber != null && mobilenumber != "")
                    {
                        string message = "Dear Biomedical Engineer " + user.FirstName + ", New Break down request ID NO " + model.BreakDownId + " is Assing to you, please login to Infonet to view the details. Tanishsoft Hrms";
                        sms.SendSmsToEmployee(mobilenumber, message);
                    }
                }
                catch { }
                mailmodel.toemail = teamdetails1.EmpEmail;
                mailmodel.subject = "Break down is Assing to you Id " + model.BreakDownId + "";
                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_BreakDown.html")))
                {
                    body = reader.ReadToEnd();
                }
                var userdet = myapp.tbl_User.Where(l => l.CustomUserId == model.CreatedBy).SingleOrDefault();
                body = body.Replace("[[employee_name]]", userdet.FirstName);
                body = body.Replace("[[Location]]", (from v in myapp.tbl_Location where v.LocationId == model.LocationId select v.LocationName).SingleOrDefault());
                body = body.Replace("[[Department]]", (from v in myapp.tbl_Department where v.DepartmentId == model.DepartmentId select v.DepartmentName).SingleOrDefault());
                body = body.Replace("[[Subdepartment]]", (from v in myapp.tbl_subdepartment where v.SubDepartmentId == model.SubDepartmentId select v.Name).SingleOrDefault());
                body = body.Replace("[[AssetNumber]]", model.AssetNumber);
                body = body.Replace("[[EquipmentName]]", model.EquipmentName);
                if (model.WorkDone != null && model.WorkDone != "")
                    body = body.Replace("[[WorkDone]]", model.WorkDone);
                body = body.Replace("[[CallStatus]]", model.CallStatus);
                if (model.StartDate != null)
                {
                    body = body.Replace("[[StartDate]]", model.StartDate.Value.ToString("dd/MM/yyyy") + " " + model.StartTime);
                }
                else
                {
                    body = body.Replace("[[StartDate]]", "");
                }
                if (model.EndDate != null)
                {
                    body = body.Replace("[[EndDate]]", model.EndDate.Value.ToString("dd/MM/yyyy") + " " + model.EndTime);
                }
                else
                {
                    body = body.Replace("[[EndDate]]", "");
                }
                body = body.Replace("[[VerifiedBy]]", "");
                body = body.Replace("[[Signature]]", "");
                body = body.Replace("[[ProblemObserved]]", "");
                body = body.Replace("[[FaultyAccessoryOrSpare]]", "");
                body = body.Replace("[[SN_FaultyAccessoryOrSpare]]", "");
                body = body.Replace("[[AccessoryOrSpareReplaced]]", "");
                body = body.Replace("[[SN_AccessoryOrSpareReplaced]]", "");
                mailmodel.body = body;
                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "BioMedical - Helpdesk";
                cm.SendEmail(mailmodel);
            }
        }
        public ActionResult AjaxGetCheckList(JQueryDataTableParamModel param)
        {

            List<tbl_bm_CheckList> query = (from d in myapp.tbl_bm_CheckList select d).ToList();
            query = query.OrderByDescending(m => m.PreventiveCheckListId).ToList();
            IEnumerable<tbl_bm_CheckList> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PreventiveCheckListId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.CheckListName != null && c.CheckListName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Description != null && c.Description.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_bm_CheckList> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           select new object[] {
                                               c.PreventiveCheckListId.ToString(),

                                              c.CheckListName,
                                              c.Description,
                                              //c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.PreventiveCheckListId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save_UpdateCheckList(tbl_bm_CheckList model)
        {
            try
            {
                tbl_bm_CheckList dbmodel = new tbl_bm_CheckList();
                if (model.PreventiveCheckListId != 0)
                {
                    dbmodel = myapp.tbl_bm_CheckList.Where(n => n.PreventiveCheckListId == model.PreventiveCheckListId).FirstOrDefault();
                    dbmodel.ModifiedBy = User.Identity.Name;
                    dbmodel.ModifiedOn = DateTime.Now;
                }
                dbmodel.CheckListName = model.CheckListName;
                dbmodel.Description = model.Description;
                dbmodel.IsActive = true;
                if (model.PreventiveCheckListId == 0)
                {
                    dbmodel.CreatedBy = User.Identity.Name;
                    dbmodel.CreatedOn = DateTime.Now;
                    myapp.tbl_bm_CheckList.Add(dbmodel);
                }

                myapp.SaveChanges();
                if (model.PreventiveCheckListId == 0)
                    return Json(dbmodel.PreventiveCheckListId, JsonRequestBehavior.AllowGet);
                else
                    return Json(model.PreventiveCheckListId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Save_UpdateCheckListItems(List<tbl_bm_CheckListItem> model)

        {
            var id = model[0].PreventiveCheckListId;
            var obj = myapp.tbl_bm_CheckListItem.Where(m => m.PreventiveCheckListId == id).ToList();
            myapp.tbl_bm_CheckListItem.RemoveRange(obj);
            myapp.SaveChanges();
            for (int i = 0; i < model.Count; i++)
            {
                model[i].IsActive = true;
            }
            myapp.tbl_bm_CheckListItem.AddRange(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCheckListAndItems(int id)
        {
            var checkList = myapp.tbl_bm_CheckList.Where(m => m.PreventiveCheckListId == id).FirstOrDefault();
            var items = myapp.tbl_bm_CheckListItem.Where(m => m.PreventiveCheckListId == id).ToList();
            CheckListandItemsViewModel viewModel = new CheckListandItemsViewModel();
            viewModel.checkList = checkList;
            viewModel.checkListItems = items;

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCheckLists()
        {
            var checkList = myapp.tbl_bm_CheckList.ToList();

            return Json(checkList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save_Update_PM_toassign(string assetids, string empid)
        {
            tbl_bm_PreventiveMaintenance _pm = new tbl_bm_PreventiveMaintenance();
            List<tbl_bm_PreventiveMaintenance> _lstpm = new List<tbl_bm_PreventiveMaintenance>();
            int currentyear = DateTime.Now.Year;
            string[] lstassets = assetids.Split(',');
            string message = "";
            foreach (var assetid in lstassets)
            {
                if (assetid != null && assetid != "")
                {
                    int inassetid = int.Parse(assetid);
                    var asset = myapp.tbl_bm_Asset.Where(m => m.AssetId == inassetid).SingleOrDefault();

                    if (asset != null)
                    {
                        string month = asset.PMDue.Value.ToString("MMM");
                        var verify = myapp.tbl_bm_PreventiveMaintenance.Where(l => l.AssetNumber == asset.AssetNo
                        && l.LocationId == asset.LocationId && l.DepartmentId == asset.DepartmentId && l.Month == month && l.CreatedOn.Value.Year == currentyear
                        && l.SubDepartmentId == asset.SubDepartmentId && l.Equipment == asset.Equipment.Trim()
                        ).SingleOrDefault();
                        if (verify == null)
                        {
                            _pm = new tbl_bm_PreventiveMaintenance();
                            _pm.CallStatus = "pending";
                            _pm.AssetNumber = asset.AssetNo;
                            _pm.Equipment = asset.Equipment.Trim();
                            _pm.AssignTo = empid;
                            _pm.DepartmentId = asset.DepartmentId;
                            _pm.LocationId = asset.LocationId;
                            _pm.Month = asset.PMDue.Value.ToString("MMM");
                            _pm.IsActive = true;
                            _pm.SubDepartmentId = asset.SubDepartmentId;
                            _pm.CreatedBy = User.Identity.Name;
                            _pm.CreatedOn = DateTime.Now;
                            myapp.tbl_bm_PreventiveMaintenance.Add(_pm);
                            myapp.SaveChanges();
                            _lstpm.Add(_pm);
                        }
                        else
                        {

                            message = message + " " + asset.AssetNo + ", ";
                        }
                    }
                }
            }

            if (message != "")
            {
                message = message + " the assets are already assigned.";
            }
            else
            {
                message = "Success";
                SendEmailPMBulk(empid, _lstpm);
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save_Update_PM(PmViewModel model)
        {
            try
            {
                tbl_bm_PreventiveMaintenance _pm = new tbl_bm_PreventiveMaintenance();
                if (model.PreventiveMaintenanceId != 0)
                {
                    _pm = myapp.tbl_bm_PreventiveMaintenance.Where(m => m.PreventiveMaintenanceId == model.PreventiveMaintenanceId).FirstOrDefault();
                    _pm.ModifiedBy = User.Identity.Name;
                    _pm.ModifiedOn = DateTime.Now;
                }
                _pm.CallStatus = "pending";
                _pm.AssetNumber = model.AssetNumber;
                _pm.Equipment = model.Equipment.Trim();
                _pm.AssignTo = model.AssignTo;
                _pm.DepartmentId = model.DepartmentId;
                _pm.LocationId = model.LocationId;
                _pm.Month = model.Month;
                _pm.IsActive = true;
                _pm.SubDepartmentId = model.SubDepartmentId;

                if (model.PreventiveMaintenanceId == 0)
                {
                    _pm.CreatedBy = User.Identity.Name;
                    _pm.CreatedOn = DateTime.Now;
                    myapp.tbl_bm_PreventiveMaintenance.Add(_pm);
                    myapp.SaveChanges();
                    SendEmailPM(_pm);
                }
                else
                {
                    myapp.SaveChanges();
                    if (model.CallStatus == "Completed")
                    {
                        var asset = myapp.tbl_bm_Asset.Where(m => m.AssetNo == _pm.AssetNumber).FirstOrDefault();
                        if (asset != null)
                        {
                            asset.PMDue = _pm.EndDate.Value.AddMonths(6);
                            myapp.SaveChanges();
                        }
                    }
                }
                //if (model.type == "Calender")
                //{
                //    var asset = myapp.tbl_bm_Asset.Where(m => m.AssetNo == model.AssetNumber && m.Equipment == model.Equipment).FirstOrDefault();
                //    if (asset != null)
                //    {
                //        asset.PMDue = DateTime.Now.AddMonths(6);
                //        myapp.SaveChanges();
                //    }
                //}
                return Json("Success", JsonRequestBehavior.AllowGet);
            }

            catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }

        }
        public void SendEmailPMBulk(string AssignTo, List<tbl_bm_PreventiveMaintenance> model)
        {
            var id = Convert.ToInt32(AssignTo);
            var AssigntoDetails = (from var in myapp.tbl_bm_TeamSetup where var.EmpId == id select var).SingleOrDefault();

            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel();
            EmailTeamplates emailtemp = new EmailTeamplates();
            mailmodel.fromemail = "Leave@hospitals.com";
            mailmodel.toemail = AssigntoDetails.EmpEmail;
            mailmodel.ccemail = "";
            mailmodel.subject = "New Preventive Maintenance is assigned";
            string body = "";
            using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_PM.html")))
            {
                body = reader.ReadToEnd();
            }
            var table = "";
            string message = "Dear Biomedical Engineer " + AssigntoDetails.EmpName + ", New PM request ID NO";
            foreach (var m in model)
            {
                var userdet = myapp.tbl_User.Where(l => l.CustomUserId == m.CreatedBy).SingleOrDefault();
                message = message + m.PreventiveMaintenanceId + ",";
                table += "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + userdet.FirstName + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == m.LocationId select var.LocationName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.LocationId == m.DepartmentId select var.DepartmentName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == m.SubDepartmentId select var.Name).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Equipment:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.Equipment + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Asset Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.AssetNumber + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Month:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.Month + "</td></tr>";

                table += "</tbody></table>";
            }
            message = message.TrimEnd(',');
            message = message + " is Assing to you, please login to Infonet to view the details. Tanishsoft Hrms";
            try
            {
                string mobilenumber = AssigntoDetails.EmpMobile;
                SendSms sms = new SendSms();
                if (mobilenumber != null && mobilenumber != "")
                {
                    sms.SendSmsToEmployee(mobilenumber, message);
                }
            }
            catch { }
            body = body.Replace("[[table]]", table);
            mailmodel.body = body;
            //mailmodel.body = "A New Ticket Assigned to you";
            //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
            mailmodel.filepath = "";
            //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
            mailmodel.fromname = "BioMedical - Helpdesk";
            cm.SendEmail(mailmodel);
        }
        public void SendEmailPM(tbl_bm_PreventiveMaintenance model)
        {
            var id = Convert.ToInt32(model.AssignTo);
            var AssigntoDetails = (from var in myapp.tbl_bm_TeamSetup where var.EmpId == id select var).SingleOrDefault();

            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel();
            EmailTeamplates emailtemp = new EmailTeamplates();
            mailmodel.fromemail = "PreventiveMaintenance@Ferendez.foundation";
            mailmodel.toemail = AssigntoDetails.EmpEmail;
            mailmodel.ccemail = "";
            mailmodel.subject = "New Preventive Maintenance is assigned to you the Id is " + model.PreventiveMaintenanceId + "";
            string body = "";
            using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_PM.html")))
            {
                body = reader.ReadToEnd();
            }
            var userdet = myapp.tbl_User.Where(l => l.CustomUserId == model.CreatedBy).SingleOrDefault();
            var table = "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
            table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + userdet.FirstName + "</td></tr>";
            table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == model.LocationId select var.LocationName).SingleOrDefault() + "</td></tr>";
            table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.LocationId == model.DepartmentId select var.DepartmentName).SingleOrDefault() + "</td></tr>";
            table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == model.SubDepartmentId select var.Name).SingleOrDefault() + "</td></tr>";
            table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Equipment:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Equipment + "</td></tr>";
            table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Asset Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.AssetNumber + "</td></tr>";
            table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Month:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Month + "</td></tr>";
            table += "</tbody></table>";

            body = body.Replace("[[table]]", table);
            mailmodel.body = body;
            //mailmodel.body = "A New Ticket Assigned to you";
            //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
            mailmodel.filepath = "";
            mailmodel.fromname = "BioMedical - Helpdesk";
            cm.SendEmail(mailmodel);
        }
        public JsonResult Save_Update_PmWithChecklist(PmViewModel model, HttpPostedFileBase UploadDocument)
        {
            tbl_bm_PreventiveMaintenance pm = new tbl_bm_PreventiveMaintenance();
            if (model.PreventiveMaintenanceId != 0)
            {
                pm = myapp.tbl_bm_PreventiveMaintenance.Where(m => m.PreventiveMaintenanceId == model.PreventiveMaintenanceId).FirstOrDefault();
                pm.ModifiedBy = User.Identity.Name;
                pm.ModifiedOn = DateTime.Now;
            }
            pm.AdminComments = model.AdminComments;
            pm.CallStatus = model.CallStatus;

            if (model.EndDate != "" && model.EndDate != null)
            {
                pm.EndDate = ProjectConvert.ConverDateStringtoDatetime(model.EndDate);
                pm.EndTime = model.EndTime;
            }

            pm.IsActive = true;
            pm.SpareReplaced = model.SpareReplaced;
            pm.StartDate = ProjectConvert.ConverDateStringtoDatetime(model.StartDate);
            pm.StartTime = model.StartTime;
            pm.JobDoneby = model.JobDoneby;
            pm.PreventiveCheckListId = model.PreventiveCheckListId;
            pm.WorkDoneDescription = model.WorkDoneDescription;
            pm.AdminComments = model.AdminComments;
            if (UploadDocument != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(UploadDocument.FileName);
                string extension = Path.GetExtension(UploadDocument.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;
                pm.Document1 = fileName;
                UploadDocument.SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));
            }
            try
            {
                myapp.SaveChanges();
                //
                if (pm.CallStatus == "Completed")
                {
                    var asset = myapp.tbl_bm_Asset.Where(m => m.AssetNo == pm.AssetNumber).FirstOrDefault();
                    if (asset != null)
                    {
                        asset.PMDone = pm.EndDate;
                        asset.PMDue = pm.EndDate.Value.AddMonths(6);
                        myapp.SaveChanges();
                    }
                    try
                    {
                        var list = myapp.tbl_bm_TeamSetup.Where(l => l.EmpMobile != null && l.EmpMobile != "" && l.LocationId == model.LocationId && (l.EmpType == "Incharge")).ToList();
                        foreach (var l in list)
                        {
                            string mobilenumber = l.EmpMobile;
                            SendSms sms = new SendSms();
                            if (mobilenumber != null && mobilenumber != "")
                            {
                                string message = "Dear Incharge " + l.EmpName + ", PM request ID NO " + model.PreventiveMaintenanceId + " has completed, and Incharge sign off need to be done, please login to Infonet to view the details. Tanishsoft Hrms";
                                sms.SendSmsToEmployee(mobilenumber, message);
                            }
                        }
                    }
                    catch { }
                    SendEmailpmUserSignOff(pm, "Completed");
                }
            }
            catch (Exception ex)
            {
            }
            return Json(model.PreventiveMaintenanceId, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save_UpdatePMCheckListItems(List<tbl_bm_PreventiveMaintenance_CheckList> model)

        {
            var id = model[0].PreventiveMaintenanceId;
            var obj = myapp.tbl_bm_PreventiveMaintenance_CheckList.Where(m => m.PreventiveMaintenanceId == id).ToList();
            myapp.tbl_bm_PreventiveMaintenance_CheckList.RemoveRange(obj);
            myapp.SaveChanges();
            for (int i = 0; i < model.Count; i++)
            {
                model[i].IsActive = true;
            }
            myapp.tbl_bm_PreventiveMaintenance_CheckList.AddRange(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetPm(int id)
        {
            var dbModel = myapp.tbl_bm_PreventiveMaintenance.Where(m => m.PreventiveMaintenanceId == id).FirstOrDefault();
            PmViewModel model = new PmViewModel();
            model.PreventiveMaintenanceId = id;
            model.AssignTo = dbModel.AssignTo;

            if (dbModel.AssignTo != null && dbModel.AssignTo != "")
            {
                var AssignToId = Convert.ToInt32(dbModel.AssignTo);

                model.AssignToName = (from V in myapp.tbl_User where V.UserId == AssignToId select V.FirstName + " " + V.LastName).SingleOrDefault();
            }
            if (dbModel.LocationId != null && dbModel.LocationId != 0)
            {
                model.LocationName = (from var in myapp.tbl_Location where var.LocationId == dbModel.LocationId select var.LocationName).SingleOrDefault();
            }
            if (dbModel.SubDepartmentId != null && dbModel.SubDepartmentId != 0)
            {
                model.SubDepartmentName = (from var in myapp.tbl_subdepartment where var.SubDepartmentId == dbModel.SubDepartmentId select var.Name).SingleOrDefault();
            }
            else
            {
                model.SubDepartmentName = "";
            }
            if (dbModel.DepartmentId != null && dbModel.DepartmentId != 0)
            {
                model.DepartmentName = (from var in myapp.tbl_Department where var.DepartmentId == dbModel.DepartmentId select var.DepartmentName).SingleOrDefault();
            }
            else
            {
                model.DepartmentName = "";
            }
            model.Month = dbModel.Month != null ? dbModel.Month : "";
            model.AssetNumber = dbModel.AssetNumber != null ? dbModel.AssetNumber : "";
            if (model.AssetNumber != null && model.AssetNumber != "")
            {
                var asmodel = myapp.tbl_bm_Asset.Where(l => l.AssetNo == model.AssetNumber).OrderByDescending(l => l.PMDue).FirstOrDefault();
                if (asmodel != null)
                {
                    model.NextDueDate = asmodel.PMDue.HasValue ? asmodel.PMDue.Value.ToString("dd/MM/yyyy") : "";
                    model.SerialNumber = asmodel.SerialNo;
                    model.Model = asmodel.Model;
                    model.Manufacture = asmodel.Manufacture;
                }
            }
            model.Equipment = dbModel.Equipment != null ? dbModel.Equipment : "";
            model.JobDoneby = dbModel.JobDoneby != null ? dbModel.JobDoneby : "";
            model.SpareReplaced = dbModel.SpareReplaced != null ? dbModel.SpareReplaced : "";
            model.AdminComments = dbModel.AdminComments != null ? dbModel.AdminComments : "";
            model.LocationId = dbModel.LocationId;
            model.DepartmentId = dbModel.DepartmentId;
            model.SubDepartmentId = dbModel.SubDepartmentId;
            model.StartTime = dbModel.StartTime;
            model.WorkDoneDescription = dbModel.WorkDoneDescription != null ? dbModel.WorkDoneDescription : "";
            if (dbModel.EndDate.HasValue)
                model.EndDate = dbModel.EndDate.Value.ToString("dd-MM-yyyy");
            else
                model.EndDate = "";
            if (dbModel.StartDate.HasValue)
                model.StartDate = dbModel.StartDate.Value.ToString("dd-MM-yyyy");
            else
                model.StartDate = "";
            model.EndTime = dbModel.EndTime;
            model.CallStatus = dbModel.CallStatus != null ? dbModel.CallStatus : "";
            model.Document1 = dbModel.Document1;
            model.PreventiveCheckListId = dbModel.PreventiveCheckListId;
            if (dbModel.PreventiveCheckListId != null && dbModel.PreventiveCheckListId != 0)
            {
                model.PreventiveCheckList = myapp.tbl_bm_CheckList.Where(c => c.PreventiveCheckListId == dbModel.PreventiveCheckListId).Select(v => v.CheckListName).FirstOrDefault();
            }
            model.UserEmpId = dbModel.UserEmpId.HasValue ? dbModel.UserEmpId.Value : 0;
            if (model.UserEmpId > 0)
            {
                if (myapp.tbl_User.Where(l => l.UserId == model.UserEmpId).SingleOrDefault() != null)
                    model.UserEmpId = int.Parse(myapp.tbl_User.Where(l => l.UserId == model.UserEmpId).SingleOrDefault().CustomUserId);
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetPM(JQueryDataTableParamModel param)
        {

            List<tbl_bm_PreventiveMaintenance> query = (from d in myapp.tbl_bm_PreventiveMaintenance select d).ToList();
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                //query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
                query = query.Where(x => x.StartDate != null && x.EndDate != null && x.StartDate.Value.Date >= FromDate.Date && x.EndDate.Value.Date <= ToDate.Date).ToList();
            }
            if (param.category != null && param.category != "")
            {
                query = query.Where(m => m.CallStatus.ToLower() == param.category.ToLower()).ToList();
            }
            int useridentityname = int.Parse(User.Identity.Name);
            var empid = (from u in myapp.tbl_User where u.EmpId == useridentityname select u).SingleOrDefault();
            var temusers = myapp.tbl_bm_TeamSetup.Where(l => l.EmpId == empid.UserId).ToList();
            if (temusers.Count > 0)
            {
                var roles = temusers.Select(l => l.EmpType).Distinct().ToList();
                if (roles.Contains("Engineer"))
                {
                    string sempid = empid.UserId.ToString();
                    query = query.Where(m => m.AssignTo == sempid).ToList();
                }
            }
            else
            {
                query = query.Where(l => l.LocationId == empid.LocationId).ToList();
                //query = query.Where(l => l.CreatedBy == User.Identity.Name).ToList();
            }
            if (param.Emp != null && param.Emp != "")
            {
                //var empId = Convert.ToInt32(param.Emp);
                //var id = (from u in myapp.tbl_User where u.UserId == empId select u.CustomUserId).SingleOrDefault();
                query = query.Where(m => m.AssignTo == param.Emp).ToList();
            }
            if (param.locationid != null && param.locationid != 0)
            {
                query = query.Where(m => m.LocationId == param.locationid).ToList();
            }
            if (param.departmentid != null && param.departmentid != 0)
            {
                query = query.Where(m => m.DepartmentId == param.departmentid).ToList();
            }
            if (param.subdepartmentid != null && param.subdepartmentid != 0)
            {
                query = query.Where(m => m.SubDepartmentId == param.subdepartmentid).ToList();
            }
            if (param.equipment != null && param.equipment != "")
            {
                query = query.Where(m => m.Equipment.Trim() == param.equipment.Trim()).ToList();
            }
            if (param.AssetTypeId != null && param.AssetTypeId != 0)//AssetId
            {
                var id = Convert.ToString(param.AssetTypeId);
                query = query.Where(m => m.AssetNumber == id).ToList();
            }
            if (param.FormType != null && param.FormType != "")
            {
                if (param.FormType == "Yes")
                {
                    query = query.Where(m => m.UserStatus == "Done").ToList();
                }
                else
                {
                    query = query.Where(m => m.UserStatus != "Done").ToList();
                }
            }
            query = query.OrderByDescending(m => m.PreventiveMaintenanceId).ToList();
            IEnumerable<tbl_bm_PreventiveMaintenance> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PreventiveMaintenanceId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.AssetNumber != null && c.AssetNumber.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())


                               ||
                               c.DepartmentId != null && c.DepartmentId.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_bm_PreventiveMaintenance> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                                           join d in myapp.tbl_Department on c.DepartmentId equals d.DepartmentId
                                           join sd in myapp.tbl_subdepartment on c.SubDepartmentId equals sd.SubDepartmentId
                                           join u in myapp.tbl_User on Convert.ToInt32(c.AssignTo) equals u.UserId
                                           select new object[] {
                                             c.Month+"_"+ c.PreventiveMaintenanceId.ToString(),
                                             l.LocationName,
                                             d.DepartmentName,
                                               sd.Name,
                                              c.AssetNumber,
                                              c.Equipment,
                                              c.Month,
                                             u.FirstName + " " + u.LastName + " - " + u.Designation,
                                             c.Document1,
                                             c.CallStatus,
                                              //c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              //c.CallStatus,
                                              //c.UserSignature,
                                               c.UserStatus!=null && c.UserStatus=="Done"? "Yes - "+myapp.tbl_User.Where(su=>su.UserId==c.UserEmpId).SingleOrDefault().FirstName:"No",
                                              c.PreventiveMaintenanceId.ToString()+"-"+c.CallStatus+"-"+c.UserStatus
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeletePreventiveMaintenance(int id)
        {
            int empid = int.Parse(User.Identity.Name);
            var userid = myapp.tbl_User.Where(l => l.EmpId == empid).SingleOrDefault().UserId;
            var usrlist = myapp.tbl_bm_TeamSetup.Where(l => l.EmpId == userid && (l.EmpType == "Hod" || l.EmpType == "Incharge")).ToList();
            if (usrlist.Count > 0)
            {
                var query = myapp.tbl_bm_PreventiveMaintenance.Where(l => l.PreventiveMaintenanceId == id).SingleOrDefault();
                myapp.tbl_bm_PreventiveMaintenance.Remove(query);
                myapp.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Access Denied", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetPmCheckListItems(int id)
        {
            var dbModel = myapp.tbl_bm_PreventiveMaintenance_CheckList.Where(m => m.PreventiveMaintenanceId == id).ToList();

            return Json(dbModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetAssetCreatePM(JQueryDataTableParamModel param)
        {
            List<tbl_bm_Asset> query = (from d in myapp.tbl_bm_Asset where d.IsActive == true select d).ToList();
            if (param.locationid != null && param.locationid != 0)
            {
                query = query.Where(m => m.LocationId == param.locationid).ToList();
            }
            if (param.departmentid != null && param.departmentid != 0)
            {
                query = query.Where(m => m.DepartmentId == param.departmentid).ToList();
            }
            if (param.subdepartmentid != null && param.subdepartmentid != 0)
            {
                query = query.Where(m => m.SubDepartmentId == param.subdepartmentid).ToList();
            }
            if (param.equipment != null && param.equipment != "")
            {
                query = query.Where(m => m.Equipment.Trim() == param.equipment.Trim()).ToList();
            }
            if (param.Year != null && param.Year != 0)
            {
                query = query.Where(m => m.PMDue != null && m.PMDue.Value.Year == param.Year).ToList();
            }
            if (param.PmDue != null && param.PmDue != 0)
            {
                query = query.Where(m => m.PMDue != null && m.PMDue.Value.Month == param.PmDue).ToList();
            }


            query = query.OrderByDescending(m => m.AssetId).ToList();
            IEnumerable<tbl_bm_Asset> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.AssetId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Equipment != null && c.Equipment.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.AssetNo != null && c.AssetNo.ToString().ToLower().Contains(param.sSearch.ToLower())


                               ||
                               c.DepartmentId != null && c.DepartmentId.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_bm_Asset> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                                           join d in myapp.tbl_Department on c.DepartmentId equals d.DepartmentId
                                           join sd in myapp.tbl_subdepartment on c.SubDepartmentId equals sd.SubDepartmentId
                                           select new object[] {
                                              c.AssetId.ToString(),
                                            l.LocationName,
d.DepartmentName,
sd.Name,
                                              c.Equipment,
                                              c.SerialNo,
                                              c.AssetNo,
                                              c.PMDone.HasValue?c.PMDone.Value.ToString("dd/MM/yyyy"):"",
                                                   c.PMDue.HasValue?c.PMDue.Value.ToString("dd/MM/yyyy"):"",

                                              //c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.AssetId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetAsset(JQueryDataTableParamModel param)
        {

            List<tbl_bm_Asset> query = (from d in myapp.tbl_bm_Asset select d).ToList();
            query = query.Where(m => m.AssetStatus != "InActive").ToList();
            if (param.locationid != null && param.locationid != 0)
            {
                query = query.Where(m => m.LocationId == param.locationid).ToList();
            }
            if (param.departmentid != null && param.departmentid != 0)
            {
                query = query.Where(m => m.DepartmentId == param.departmentid).ToList();
            }
            if (param.subdepartmentid != null && param.subdepartmentid != 0)
            {
                query = query.Where(m => m.SubDepartmentId == param.subdepartmentid).ToList();
            }
            if (param.equipment != null && param.equipment != "")
            {
                query = query.Where(m => m.Equipment.Trim() == param.equipment.Trim()).ToList();
            }
            if (param.PmDue != null && param.PmDue != 0)
            {
                query = query.Where(m => m.PMDue != null && m.PMDue.Value.Month == param.PmDue).ToList();
            }

            query = query.OrderByDescending(m => m.AssetId).ToList();
            IEnumerable<tbl_bm_Asset> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.AssetId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Equipment != null && c.Equipment.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.AssetNo != null && c.AssetNo.ToString().ToLower().Contains(param.sSearch.ToLower())


                               ||
                               c.DepartmentId != null && c.DepartmentId.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_bm_Asset> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                                           join d in myapp.tbl_Department on c.DepartmentId equals d.DepartmentId
                                           join sd in myapp.tbl_subdepartment on c.SubDepartmentId equals sd.SubDepartmentId
                                           select new object[] {
                                              c.AssetId.ToString(),
                                             l.LocationName,
d.DepartmentName,
sd.Name,
                                              c.Equipment,
                                              c.Manufacture,
                                              c.Model,
                                               c.SerialNo,
                                              c.AssetNo,c.DateOfInstallation,c.PONumber,c.EquipmentCost,c.VendorDetails,
                                              c.PMDone.HasValue?c.PMDone.Value.ToString("dd/MM/yyyy"):"",
                                                   c.PMDue.HasValue?c.PMDue.Value.ToString("dd/MM/yyyy"):"",
                                                    c.CALLDone.HasValue?c.CALLDone.Value.ToString("dd/MM/yyyy"):"",
                                                   c.CALLDue.HasValue?c.CALLDue.Value.ToString("dd/MM/yyyy"):"",
                                                    c.AssetService,
                                                   // c.WarrantyStartDate.HasValue?c.WarrantyStartDate.Value.ToString("dd/MM/yyyy"):"",
                                                   //c.WarrantyEndDate.HasValue?c.WarrantyEndDate.Value.ToString("dd/MM/yyyy"):"",
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.AssetId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelAssets(int locationId = 0, int departmentId = 0, int subdepartmentId = 0, string Equipment = "", string month = "")
        {

            List<tbl_bm_Asset> query = (from d in myapp.tbl_bm_Asset select d).ToList();

            if (locationId != null && locationId != 0)
            {
                query = query.Where(m => m.LocationId == locationId).ToList();
            }
            if (departmentId != null && departmentId != 0)
            {
                query = query.Where(m => m.DepartmentId == departmentId).ToList();
            }
            if (subdepartmentId != null && subdepartmentId != 0)
            {
                query = query.Where(m => m.SubDepartmentId == subdepartmentId).ToList();
            }
            if (Equipment != null && Equipment != "")
            {
                query = query.Where(m => m.Equipment == Equipment).ToList();
            }
            if (month != null && month != "")
            {
                int monthpmdue = int.Parse(month);
                query = query.Where(m => m.PMDue.Value.Month == monthpmdue).ToList();
            }
            var products = new System.Data.DataTable("Daliy Rounds");
            products.Columns.Add("AssetId", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("Sub Department", typeof(string));
            products.Columns.Add("Equipment", typeof(string));
            products.Columns.Add("Manufacture", typeof(string));
            products.Columns.Add("Model", typeof(string));
            products.Columns.Add("Serial No", typeof(string));
            products.Columns.Add("Asset No", typeof(string));
            products.Columns.Add("Date Of Installation", typeof(string));
            products.Columns.Add("PM Done", typeof(string));
            products.Columns.Add("PM Due", typeof(string));
            products.Columns.Add("CALL Done", typeof(string));
            products.Columns.Add("CALL Due", typeof(string));
            products.Columns.Add("Service Provider Contact", typeof(string));
            products.Columns.Add("Comments", typeof(string));
            products.Columns.Add("Status", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("CreatedBy", typeof(string));

            foreach (var item in query)
            {
                products.Rows.Add(
               item.AssetId.ToString(),
                  (from var in myapp.tbl_Location where var.LocationId == item.LocationId select var.LocationName).SingleOrDefault(),
(from var in myapp.tbl_Department where var.DepartmentId == item.DepartmentId select var.DepartmentName).SingleOrDefault(),
(from var in myapp.tbl_subdepartment where var.SubDepartmentId == item.SubDepartmentId select var.Name).SingleOrDefault(),
item.Equipment, item.Manufacture, item.Model, item.SerialNo, item.AssetNo, item.DateOfInstallation,
(item.PMDone.HasValue ? item.PMDone.Value.ToString("dd/MM/yyyy") : ""),
(item.PMDue.HasValue ? item.PMDue.Value.ToString("dd/MM/yyyy") : ""),
(item.CALLDone.HasValue ? item.CALLDone.Value.ToString("dd/MM/yyyy") : ""),
(item.CALLDue.HasValue ? item.CALLDue.Value.ToString("dd/MM/yyyy") : ""),
item.ServiceProviderContact, item.Comments, (item.AssetStatus == null ? "Active" : item.AssetStatus),
(item.CreatedOn.HasValue ? item.CreatedOn.Value.ToString("dd/MM/yyyy") : ""), item.CreatedBy
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=Assets.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveUpdateAsset(AssetbmViewModel taskmodel)
        {
            var type = "NEW";
            tbl_bm_Asset _pm = new tbl_bm_Asset();
            int inassetid = int.Parse(taskmodel.AssetId);
            if (inassetid != 0)
            {
                _pm = myapp.tbl_bm_Asset.Where(m => m.AssetId == inassetid).FirstOrDefault();
                _pm.ModifiedBy = User.Identity.Name;
                _pm.ModifiedOn = DateTime.Now;
                type = "UPDATE";
            }
            _pm.AssetNo = taskmodel.AssetNo;
            _pm.Equipment = taskmodel.Equipment;
            if (taskmodel.PMDone != "" && taskmodel.PMDone != null)
            {
                _pm.PMDone = ProjectConvert.ConverDateStringtoDatetime(taskmodel.PMDone);
            }
            if (taskmodel.PMDue != "" && taskmodel.PMDue != null)
            {
                _pm.PMDue = ProjectConvert.ConverDateStringtoDatetime(taskmodel.PMDue);
            }
            if (taskmodel.WarrantyStartDate != "" && taskmodel.WarrantyStartDate != null)
            {
                _pm.WarrantyStartDate = ProjectConvert.ConverDateStringtoDatetime(taskmodel.WarrantyStartDate);
            }
            if (taskmodel.WarrantyEndDate != "" && taskmodel.WarrantyEndDate != null)
            {
                _pm.WarrantyEndDate = ProjectConvert.ConverDateStringtoDatetime(taskmodel.WarrantyEndDate);
            }
            if (taskmodel.CALLDue != "" && taskmodel.CALLDue != null)
            {
                _pm.CALLDue = ProjectConvert.ConverDateStringtoDatetime(taskmodel.CALLDue);
            }
            if (taskmodel.CALLDone != "" && taskmodel.CALLDone != null)
            {
                _pm.CALLDone = ProjectConvert.ConverDateStringtoDatetime(taskmodel.CALLDone);
            }
            if (taskmodel.CalibrationDue != "" && taskmodel.CalibrationDue != null)
            {
                _pm.CalibrationDue = ProjectConvert.ConverDateStringtoDatetime(taskmodel.CalibrationDue);
            }
            _pm.DepartmentId = int.Parse(taskmodel.DepartmentId);
            _pm.LocationId = int.Parse(taskmodel.LocationId);
            _pm.Comments = taskmodel.Comments;
            _pm.IsActive = true;
            _pm.SubDepartmentId = int.Parse(taskmodel.SubDepartmentId);
            _pm.DateOfInstallation = taskmodel.DateOfInstallation;
            _pm.Manufacture = taskmodel.Manufacture;
            _pm.Model = taskmodel.Model;
            _pm.SerialNo = taskmodel.SerialNo;
            _pm.ServiceProviderContact = taskmodel.ServiceProviderContact;
            _pm.AssetService = taskmodel.AssetService;
            _pm.PONumber = taskmodel.PONumber;
            _pm.VendorDetails = taskmodel.VendorDetails;
            _pm.EquipmentCost = taskmodel.EquipmentCost;
            try
            {
                if (int.Parse(taskmodel.AssetId) == 0)
                {
                    _pm.CreatedBy = User.Identity.Name;
                    _pm.CreatedOn = DateTime.Now;
                    myapp.tbl_bm_Asset.Add(_pm);
                    myapp.SaveChanges();
                    SendEmailNewAsset(_pm, type);
                    try
                    {
                        string LocationName = myapp.tbl_Location.Where(l => l.LocationId == _pm.LocationId).SingleOrDefault().LocationName;
                        string mobilenumber = String.Join(",", myapp.tbl_bm_TeamSetup.Where(l => l.EmpMobile != null && l.EmpMobile != "" && l.LocationId == _pm.LocationId).Select(l => l.EmpMobile).Distinct().ToList());
                        SendSms sms = new SendSms();
                        if (mobilenumber != null && mobilenumber != "")
                        {
                            string message = "Dear Biomedical Team, New Asset is added to location " + LocationName + ", please login to Infonet to view the details. Tanishsoft Hrms";
                            sms.SendSmsToEmployee(mobilenumber, message);
                        }
                    }
                    catch { }
                }
                else
                {
                    myapp.SaveChanges();
                }
            }
            catch (Exception ex)
            {

            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ActiveInactiveAsset(int id)
        {
            var model = myapp.tbl_bm_Asset.Where(m => m.AssetId == id).FirstOrDefault();

            if (model.IsActive == true)
            {
                model.IsActive = false;
            }
            else
            {
                model.IsActive = true;
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAsset(int id)
        {
            var model = myapp.tbl_bm_Asset.Where(m => m.AssetId == id).FirstOrDefault();
            AssetbmViewModel _pm = new AssetbmViewModel();
            _pm.AssetNo = model.AssetNo;
            _pm.Equipment = model.Equipment;
            _pm.DepartmentId = model.DepartmentId.HasValue ? model.DepartmentId.Value.ToString() : "0";
            _pm.LocationId = model.LocationId.HasValue ? model.LocationId.Value.ToString() : "0";
            _pm.Comments = model.Comments;
            _pm.IsActive = "true";
            _pm.SubDepartmentId = model.SubDepartmentId.HasValue ? model.SubDepartmentId.Value.ToString() : "0";
            if (model.DateOfInstallation.Contains("/"))
            {
                _pm.DateOfInstallation = model.DateOfInstallation.Replace("/", "-");
            }
            else
            {
                _pm.DateOfInstallation = model.DateOfInstallation;
            }
            _pm.Manufacture = model.Manufacture;
            _pm.Model = model.Model;
            _pm.SerialNo = model.SerialNo;
            _pm.EquipmentCost = model.EquipmentCost;
            _pm.VendorDetails = model.VendorDetails;
            _pm.PONumber = model.PONumber;
            _pm.ServiceProviderContact = model.ServiceProviderContact;
            if (model.PMDone.HasValue)
                _pm.PMDone = model.PMDone.Value.ToString("dd-MM-yyyy");
            if (model.CALLDue.HasValue)
                _pm.CALLDue = model.CALLDue.Value.ToString("dd-MM-yyyy");
            if (model.CALLDone.HasValue)
                _pm.CALLDone = model.CALLDone.Value.ToString("dd-MM-yyyy");
            if (model.PMDue.HasValue)
                _pm.PMDue = model.PMDue.Value.ToString("dd-MM-yyyy");
            if (model.CalibrationDue.HasValue)
                _pm.CalibrationDue = model.CalibrationDue.Value.ToString("dd-MM-yyyy");
            if (model.WarrantyStartDate.HasValue)
                _pm.WarrantyStartDate = model.WarrantyStartDate.Value.ToString("dd-MM-yyyy");
            if (model.WarrantyEndDate.HasValue)
                _pm.WarrantyEndDate = model.WarrantyEndDate.Value.ToString("dd-MM-yyyy");
            _pm.AssetService = model.AssetService;
            return Json(_pm, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAssetByAssetNo(string id)
        {
            var model = myapp.tbl_bm_Asset.Where(m => m.AssetNo == id).FirstOrDefault();
            var equipment = model.Equipment;
            return Json(equipment, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAssetByEquipment(string id)
        {
            var model = myapp.tbl_bm_Asset.Where(m => m.Equipment == id).FirstOrDefault();
            var assetNo = model.AssetNo;
            return Json(assetNo, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDepartmentByLocation(int id, int month = 0)
        {
            var list = (from d in myapp.tbl_Department
                        join ad in myapp.tbl_bm_Asset on d.DepartmentId equals ad.DepartmentId
                        where d.LocationId == id
                        select new
                        {
                            ad.DepartmentId,
                            d.DepartmentName,
                            ad.PMDue
                        }).Distinct().OrderBy(l => l.DepartmentName).ToList();
            if (month != 0)
            {
                list = list.Where(n => n.PMDue != null && n.PMDue.Value.Month == month).ToList();

            }
            var sublist = (from l in list
                           select new
                           {
                               l.DepartmentId,
                               l.DepartmentName
                           }).Distinct().ToList();
            return Json(sublist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSubDepartmentByDepartment(int id, int month = 0)
        {
            var list = (from d in myapp.tbl_subdepartment
                        join ad in myapp.tbl_bm_Asset on d.SubDepartmentId equals ad.SubDepartmentId
                        where d.DepartmentId == id
                        select new
                        {
                            ad.SubDepartmentId,
                            d.Name,
                            ad.PMDue
                        }).Distinct().OrderBy(l => l.Name).ToList();
            if (month != 0)
            {
                list = list.Where(n => n.PMDue != null && n.PMDue.Value.Month == month).ToList();

            }
            var sublist = (from l in list
                           select new
                           {
                               l.SubDepartmentId,
                               l.Name
                           }).Distinct().ToList();
            return Json(sublist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEquipmentByDepartments(int locationid, int departmentid = 0, int subdepartmentId = 0, int month = 0)
        {
            List<tbl_bm_Asset> list = myapp.tbl_bm_Asset.Where(m => m.LocationId == locationid).ToList();
            if (departmentid != null && departmentid > 0)
            {
                list = list.Where(m => m.DepartmentId == departmentid).ToList();
            }
            if (subdepartmentId != null && subdepartmentId > 0)
            {
                list = list.Where(m => m.SubDepartmentId == subdepartmentId).ToList();
            }
            if (month != 0)
                list = list.Where(b => b.PMDue != null && b.PMDue.Value.Month == month).ToList();

            var model = list.OrderBy(l => l.Equipment).Select(n => n.Equipment).Distinct().ToList();


            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEquipmentBySubDepartments(int locationid, int departmentid, int subdepartmentId)
        {
            List<tbl_bm_Asset> list = myapp.tbl_bm_Asset.Where(m => m.SubDepartmentId == subdepartmentId && m.DepartmentId == departmentid && m.LocationId == locationid).ToList();


            var model = list.OrderBy(l => l.Equipment).Select(n => n.Equipment).Distinct().ToList();


            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CompletedDailyRounds(string endDate, string endTime, int id)
        {
            var dbModel = myapp.tbl_bm_DailyRound.Where(m => m.DailyRoundId == id).FirstOrDefault();
            dbModel.CallStatus = "Completed";
            dbModel.EndDate = ProjectConvert.ConverDateStringtoDatetime(endDate);
            dbModel.EndTime = endTime;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveUsersignDailyRound(UserSignoffViewModel model)
        {
            model.signature = model.signature.Replace("data:image/png;base64,", String.Empty);
            var bytes = Convert.FromBase64String(model.signature);
            string file = Path.Combine(Server.MapPath("~/Documents/"), "Sign_" + model.id + ".png");

            //Debug.WriteLine(file);
            if (bytes.Length > 0)
            {
                using (var stream = new FileStream(file, FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
            }
            var dbModel = myapp.tbl_bm_DailyRound.Where(m => m.DailyRoundId == model.id).FirstOrDefault();
            dbModel.UserSignature = "Sign_" + model.id + ".png";
            dbModel.UserEmpId = int.Parse(model.userempId);
            dbModel.UserStatus = "Done";
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;
            myapp.SaveChanges();



            //if (dbModel.CallStatus == "Completed" && dbModel.UserStatus == "Done")
            //{
            SendEmailDRUserSignOff(dbModel);
            //}
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveUsersignBreakDown(UserSignoffViewModel model)
        {
            model.signature = model.signature.Replace("data:image/png;base64,", String.Empty);
            var bytes = Convert.FromBase64String(model.signature);
            string file = Path.Combine(Server.MapPath("~/Documents/"), "Sign_" + model.id + ".png");

            //Debug.WriteLine(file);
            if (bytes.Length > 0)
            {
                using (var stream = new FileStream(file, FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush();
                }
            }
            var dbModel = myapp.tbl_bm_BreakDown.Where(m => m.BreakDownId == model.id).FirstOrDefault();
            dbModel.UserSignature = "Sign_" + model.id + ".png";
            dbModel.UserEmpId = int.Parse(model.userempId);
            dbModel.UserStatus = "Done";
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;
            myapp.SaveChanges();
            if (dbModel.CallStatus == "Completed" && dbModel.UserStatus == "Done")
            {
                SendEmailbdUserSignOff(dbModel);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveUsersignPM(UserSignoffViewModel model)
        {
            //model.signature = model.signature.Replace("data:image/png;base64,", String.Empty);
            //var bytes = Convert.FromBase64String(model.signature);
            //string file = Path.Combine(Server.MapPath("~/Documents/"), "Sign_" + model.id + ".png");

            ////Debug.WriteLine(file);
            //if (bytes.Length > 0)
            //{
            //    using (var stream = new FileStream(file, FileMode.Create))
            //    {
            //        stream.Write(bytes, 0, bytes.Length);
            //        stream.Flush();
            //    }
            //}
            var dbModel = myapp.tbl_bm_PreventiveMaintenance.Where(m => m.PreventiveMaintenanceId == model.id).FirstOrDefault();
            dbModel.UserSignature = model.signature;
            dbModel.UserEmpId = int.Parse(model.userempId);
            dbModel.UserStatus = "Done";
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;
            myapp.SaveChanges();
            if (dbModel.CallStatus == "Completed" && dbModel.UserStatus == "Done")
            {
                //var asset = myapp.tbl_bm_Asset.Where(m => m.AssetNo == dbModel.AssetNumber).FirstOrDefault();
                //if (asset != null)
                //{
                //    //asset.PMDue = dbModel.StartDate.Value.AddMonths(6);
                //    asset.PMDue = asset.PMDue != null ? asset.PMDue.Value.AddMonths(6) : dbModel.StartDate.Value.AddMonths(6);
                //    myapp.SaveChanges();
                //}
                SendEmailpmUserSignOff(dbModel);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult CompletedBreakDown(string endDate, string endTime, int id)
        {
            var dbModel = myapp.tbl_bm_BreakDown.Where(m => m.BreakDownId == id).FirstOrDefault();
            dbModel.CallStatus = "Completed";
            dbModel.EndDate = ProjectConvert.ConverDateStringtoDatetime(endDate);
            dbModel.EndTime = endTime;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult CompletedPM(string endDate, string endTime, int id)
        {
            var dbModel = myapp.tbl_bm_PreventiveMaintenance.Where(m => m.PreventiveMaintenanceId == id).FirstOrDefault();
            dbModel.CallStatus = "Completed";
            dbModel.EndDate = ProjectConvert.ConverDateStringtoDatetime(endDate);
            dbModel.EndTime = endTime;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;
            myapp.SaveChanges();
            var asset = myapp.tbl_bm_Asset.Where(m => m.AssetNo == dbModel.AssetNumber).FirstOrDefault();
            if (asset != null)
            {
                asset.PMDone = dbModel.EndDate;
                asset.PMDue = dbModel.EndDate.Value.AddMonths(6);
                myapp.SaveChanges();
            }
            SendEmailpmUserSignOff(dbModel, "Completed");
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelDailyRounds(string fromDate, string toDate, string callStatus = null, int id = 0, int locationId = 0, int departmentId = 0, int subdepartmentId = 0)
        {

            List<tbl_bm_DailyRound> query = (from d in myapp.tbl_bm_DailyRound select d).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (callStatus != null && callStatus != "")
            {
                query = query.Where(m => m.CallStatus.ToLower() == callStatus.ToLower()).ToList();
            }
            if (id != null && id != 0)
            {
                var uid = (from u in myapp.tbl_User where u.UserId == id select u.CustomUserId).SingleOrDefault();
                query = query.Where(m => m.CreatedBy == uid).ToList();
            }
            if (locationId != null && locationId != 0)
            {
                query = query.Where(m => m.LocationId == locationId).ToList();
            }
            if (departmentId != null && departmentId != 0)
            {
                query = query.Where(m => m.DepartmentId == departmentId).ToList();
            }
            if (subdepartmentId != null && subdepartmentId != 0)
            {
                query = query.Where(m => m.SubDepartmentId == subdepartmentId).ToList();
            }
            var products = new System.Data.DataTable("Daliy Rounds");
            products.Columns.Add("Daily Rounds Id", typeof(string));
            products.Columns.Add("Location Id", typeof(string));
            products.Columns.Add("Department Id ", typeof(string));
            products.Columns.Add("Sub Department Id ", typeof(string));
            products.Columns.Add("WorkDone", typeof(string));
            products.Columns.Add("CallStatus", typeof(string));
            products.Columns.Add("Start Date and Time", typeof(string));
            products.Columns.Add("End Date and Time", typeof(string));
            products.Columns.Add("Admin Comments", typeof(string));
            products.Columns.Add("UserEmpId", typeof(string));

            foreach (var item in query)
            {
                products.Rows.Add(
               item.DailyRoundId.ToString(),
                  (from var in myapp.tbl_Location where var.LocationId == item.LocationId select var.LocationName).SingleOrDefault(),
(from var in myapp.tbl_Department where var.DepartmentId == item.DepartmentId select var.DepartmentName).SingleOrDefault(),
(from var in myapp.tbl_subdepartment where var.SubDepartmentId == item.SubDepartmentId select var.Name).SingleOrDefault(),
                item.WorkDone,
                 item.CallStatus,
               item.StartDate.HasValue ? (item.StartDate.Value.ToString("dd/MM/yyyy") + " " + item.StartTime) : "",
                item.EndDate.HasValue ? (item.EndDate.Value.ToString("dd/MM/yyyy") + " " + item.EndTime) : "",
           item.AdminComments,
           (from var in myapp.tbl_User where var.UserId == item.UserEmpId select var.FirstName).SingleOrDefault()

                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=DailyRounds.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportExcelPM(string fromDate, string toDate, string callStatus = null, int id = 0, int locationId = 0, int departmentId = 0, int subdepartmentId = 0, string equipment = null, string assetId = null)
        {

            List<tbl_bm_PreventiveMaintenance> query = (from d in myapp.tbl_bm_PreventiveMaintenance select d).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (callStatus != null && callStatus != "")
            {
                query = query.Where(m => m.CallStatus.ToLower() == callStatus.ToLower()).ToList();
            }
            if (id != null && id != 0)
            {
                var uid = (from u in myapp.tbl_User where u.UserId == id select u.CustomUserId).SingleOrDefault();
                query = query.Where(m => m.CreatedBy == uid).ToList();
            }
            if (locationId != null && locationId != 0)
            {
                query = query.Where(m => m.LocationId == locationId).ToList();
            }
            if (departmentId != null && departmentId != 0)
            {
                query = query.Where(m => m.DepartmentId == departmentId).ToList();
            }
            if (subdepartmentId != null && subdepartmentId != 0)
            {
                query = query.Where(m => m.SubDepartmentId == subdepartmentId).ToList();
            }
            if (equipment != null && equipment != "")
            {
                query = query.Where(m => m.Equipment.Trim() == equipment.Trim()).ToList();
            }
            if (assetId != null && assetId != "")
            {
                query = query.Where(m => m.AssetNumber == assetId).ToList();
            }
            var products = new System.Data.DataTable("PM");
            products.Columns.Add("Preventive Maintenance Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("Sub Department", typeof(string));
            products.Columns.Add("Month", typeof(string));
            products.Columns.Add("Year", typeof(string));
            products.Columns.Add("Equipment", typeof(string));
            products.Columns.Add("Asset Number", typeof(string));
            //products.Columns.Add("AssignTo", typeof(string));
            products.Columns.Add("Work Done Description", typeof(string));
            products.Columns.Add("Spare Replaced", typeof(string));
            products.Columns.Add("Job Done by", typeof(string));
            products.Columns.Add("CallStatus", typeof(string));
            products.Columns.Add("Start Date", typeof(string));
            products.Columns.Add("Start Time", typeof(string));
            products.Columns.Add("End Date", typeof(string));
            products.Columns.Add("End Time", typeof(string));
            products.Columns.Add("Admin Comments", typeof(string));
            foreach (var item in query)
            {
                products.Rows.Add(
               item.PreventiveMaintenanceId.ToString(),
                  (from var in myapp.tbl_Location where var.LocationId == item.LocationId select var.LocationName).SingleOrDefault(),
(from var in myapp.tbl_Department where var.DepartmentId == item.DepartmentId select var.DepartmentName).SingleOrDefault(),
(from var in myapp.tbl_subdepartment where var.SubDepartmentId == item.SubDepartmentId select var.Name).SingleOrDefault(),
                item.Month, item.StartDate.HasValue ? (item.StartDate.Value.ToString("yyyy")) : "",
                item.Equipment, item.AssetNumber, item.WorkDoneDescription,
                   item.SpareReplaced, item.JobDoneby,
                 item.CallStatus,
               item.StartDate.HasValue ? (item.StartDate.Value.ToString("dd/MM/yyyy")) : "",
                item.StartDate.HasValue ? (item.StartTime) : "",
                item.EndDate.HasValue ? (item.EndDate.Value.ToString("dd/MM/yyyy")) : "",
                item.EndDate.HasValue ? (item.EndTime) : "",
                item.AdminComments


                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=PM.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelBreakDown(string fromDate, string toDate, string callStatus = null, int id = 0, int locationId = 0, int departmentId = 0, int subdepartmentId = 0, string equipment = null, string assetId = null)
        {

            List<tbl_bm_BreakDown> query = (from d in myapp.tbl_bm_BreakDown select d).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (callStatus != null && callStatus != "")
            {
                query = query.Where(m => m.CallStatus.ToLower() == callStatus.ToLower()).ToList();
            }
            if (id != null && id != 0)
            {
                var uid = (from u in myapp.tbl_User where u.UserId == id select u.CustomUserId).SingleOrDefault();
                query = query.Where(m => m.CreatedBy == uid).ToList();
            }
            if (locationId != null && locationId != 0)
            {
                query = query.Where(m => m.LocationId == locationId).ToList();
            }
            if (departmentId != null && departmentId != 0)
            {
                query = query.Where(m => m.DepartmentId == departmentId).ToList();
            }
            if (subdepartmentId != null && subdepartmentId != 0)
            {
                query = query.Where(m => m.SubDepartmentId == subdepartmentId).ToList();
            }
            if (equipment != null && equipment != "")
            {
                query = query.Where(m => m.EquipmentName.Trim() == equipment.Trim()).ToList();
            }
            if (assetId != null && assetId != "")
            {
                query = query.Where(m => m.AssetNumber == assetId).ToList();
            }
            var products = new System.Data.DataTable("BreakDown");
            products.Columns.Add("BreakDown Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("Sub Department", typeof(string));
            //products.Columns.Add("Asset Id", typeof(string));
            products.Columns.Add("Asset Number", typeof(string));
            products.Columns.Add("Equipment Name", typeof(string));
            products.Columns.Add("Manufacture", typeof(string));
            products.Columns.Add("Model", typeof(string));
            products.Columns.Add("SerialNo", typeof(string));
            products.Columns.Add("AssignTo", typeof(string));
            products.Columns.Add("Problem Reported", typeof(string));

            products.Columns.Add("Problem Observed", typeof(string));
            products.Columns.Add("Faulty Accessory Or Spare", typeof(string));
            products.Columns.Add("SN_Faulty Accessory Or Spare", typeof(string));
            products.Columns.Add("WorkDone", typeof(string));
            products.Columns.Add("Accessory Or SpareReplaced", typeof(string));
            products.Columns.Add("SN_Accessory Or SpareReplaced", typeof(string));
            products.Columns.Add("CallStatus", typeof(string));
            products.Columns.Add("Call Received Date", typeof(string));
            products.Columns.Add("Call Received Time", typeof(string));
            products.Columns.Add("User Emp Id", typeof(string));
            products.Columns.Add("User Status", typeof(string));
            products.Columns.Add("User Comments", typeof(string));
            products.Columns.Add("Start Date", typeof(string));
            products.Columns.Add("StartTime", typeof(string));
            products.Columns.Add("End Date", typeof(string));
            products.Columns.Add("End Time", typeof(string));
            products.Columns.Add("Response Time(M)", typeof(string));
            products.Columns.Add("Down Time(M)", typeof(string));
            products.Columns.Add("Admin Comments", typeof(string));
            foreach (var item in query)
            {
                var assestdetails = myapp.tbl_bm_Asset.Where(l => l.AssetNo == item.AssetNumber).FirstOrDefault();
                //if (assestdetails != null)
                //{
                string StartDatestr = (item.StartDate.HasValue ? (item.StartDate.Value.ToString("dd/MM/yyyy")) : "");
                string EndDatestr = (item.EndDate.HasValue ? (item.EndDate.Value.ToString("dd/MM/yyyy")) : "");

                string ResponseTime = "";
                string DownTime = "";
                if (StartDatestr != null && StartDatestr != "" && item.StartTime != null && item.StartTime != "")
                {
                    try
                    {
                        if (StartDatestr.Contains("-"))
                        {
                            DateTime stdate = DateTime.ParseExact(StartDatestr + " " + item.StartTime, "dd-MM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
                            ResponseTime = (stdate - item.CreatedOn).Value.TotalMinutes.ToString("0");
                            if (ResponseTime.Contains('-'))
                            {
                                ResponseTime = "0";
                            }
                        }
                        else
                        {
                            DateTime stdate = DateTime.ParseExact(StartDatestr + " " + item.StartTime, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                            ResponseTime = (stdate - item.CreatedOn).Value.TotalMinutes.ToString("0");
                            if (ResponseTime.Contains('-'))
                            {
                                ResponseTime = "0";
                            }
                        }
                    }
                    catch { }
                }
                if (EndDatestr != null && EndDatestr != "" && item.EndTime != null && item.EndTime != "")
                {
                    try
                    {
                        if (EndDatestr.Contains("-"))
                        {
                            DateTime enddate = DateTime.ParseExact(EndDatestr + " " + item.EndTime, "dd-MM-yyyy hh:mm tt", CultureInfo.InvariantCulture);
                            DownTime = (enddate - item.CreatedOn).Value.TotalMinutes.ToString("0");
                            if (DownTime.Contains('-'))
                            {
                                DownTime = "0";
                            }

                        }
                        else
                        {
                            DateTime enddate = DateTime.ParseExact(EndDatestr + " " + item.EndTime, "dd/MM/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                            DownTime = (enddate - item.CreatedOn).Value.TotalMinutes.ToString("0");
                            if (DownTime.Contains('-'))
                            {
                                DownTime = "0";
                            }
                        }
                    }
                    catch { }
                }
                string Assigntoname = "";
                if (item.AssignTo > 0)
                {
                    var userdetails = myapp.tbl_User.Where(l => l.EmpId == item.AssignTo).SingleOrDefault();
                    if (userdetails != null)
                    {
                        Assigntoname = userdetails.FirstName;
                    }
                }
                products.Rows.Add(
               item.BreakDownId.ToString(),
               (from var in myapp.tbl_Location where var.LocationId == item.LocationId select var.LocationName).SingleOrDefault(),
               (from var in myapp.tbl_Department where var.DepartmentId == item.DepartmentId select var.DepartmentName).SingleOrDefault(),
               (from var in myapp.tbl_subdepartment where var.SubDepartmentId == item.SubDepartmentId select var.Name).SingleOrDefault(),
               item.AssetNumber,
               item.EquipmentName,
               assestdetails != null ? assestdetails.Manufacture : "",
                assestdetails != null ? assestdetails.Model : "",
               assestdetails != null ? assestdetails.SerialNo : "",
               Assigntoname,
               item.ProblemReported, item.ProblemObserved,
               item.FaultyAccessoryOrSpare, item.SN_FaultyAccessoryOrSpare, item.WorkDone, item.AccessoryOrSpareReplaced,
               item.SN_AccessoryOrSpareReplaced,
               item.CallStatus,
                  "'" + (item.CreatedOn.HasValue ? item.CreatedOn.Value.ToString("dd/MM/yyyy") : ""),
                     "'" + (item.CreatedOn.HasValue ? item.CreatedOn.Value.ToString("hh:mm tt") : ""), item.UserEmpId, item.UserStatus, item.UserComments,
               "'" + (item.StartDate.HasValue ? (item.StartDate.Value.ToString("dd/MM/yyyy")) : ""),
                "'" + (item.StartDate.HasValue ? (item.StartTime) : ""),
                "'" + (item.EndDate.HasValue ? (item.EndDate.Value.ToString("dd/MM/yyyy")) : ""),
                "'" + (item.EndDate.HasValue ? (item.EndTime) : ""),
                ResponseTime,
               DownTime,
                item.AdminComments
                );
                //}
            }
            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=BreakDown.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult GetAssertAutoComplete(string id, int locationId, int departmentId, int subDepartmentId)
        {
            var query = myapp.tbl_bm_Asset.Where(l => l.IsActive == true && l.LocationId == locationId && l.DepartmentId == departmentId && l.SubDepartmentId == subDepartmentId).ToList();
            if (id != "" && id != "null" && id != null)
            {
                query = query.Where(n => n.Equipment.ToLower().Trim() == id.ToLower()).ToList();
            }

            var resulst = (from q in query
                           select new
                           {
                               id = q.AssetNo,
                               text = q.Equipment + " " + q.AssetNo
                           }).Distinct().ToList();
            return Json(resulst, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult GetEquipmentNameAutoComplete(string searchTerm)
        {
            var query = myapp.tbl_bm_Asset.Where(l => l.IsActive == true).ToList();
            //if (id != "" && id != "null" && id != null)
            //{
            //    query = query.Where(n => n.AssetNo == id).ToList();
            //}
            query = query
                    .Where(c => c.Equipment.ToLower().Contains(searchTerm.ToLower())).ToList();

            var resulst = (from q in query
                           select new
                           {
                               id = q.Equipment,
                               text = q.Equipment
                           }).Distinct().ToList();
            return Json(resulst, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public async Task<ActionResult> GetEmployeeSearch(string searchTerm, int id, string table)
        {

            //if(table== "DailyRounds")
            //{
            //    var dR = myapp.tbl_bm_DailyRound.Where(m => m.DailyRoundId == id).FirstOrDefault();
            //    locationId = dR.LocationId.Value;
            //    departmentId = dR.DepartmentId.Value;
            //}
            //else if (table == "BreakDown")
            //{
            //    var dR = myapp.tbl_bm_BreakDown.Where(m => m.BreakDownId == id).FirstOrDefault();
            //    locationId = dR.LocationId.Value;
            //    departmentId = dR.DepartmentId.Value;
            //}
            //else if (table == "PM")
            //{
            //    var dR = myapp.tbl_bm_PreventiveMaintenance.Where(m => m.PreventiveMaintenanceId == id).FirstOrDefault();
            //    locationId = dR.LocationId.Value;
            //    departmentId = dR.DepartmentId.Value;
            //}
            //var query = myapp.tbl_User.Where(l => l.IsActive == true && l.LocationId==locationId&& l.DepartmentId==departmentId).ToList();

            var query = await myapp.tbl_User
                     .Where(c => c.IsActive == true && (c.CustomUserId.ToLower().Contains(searchTerm.ToLower())
                                 ||
                                  c.LocationName != null && c.LocationName.ToLower().Contains(searchTerm.ToLower())
                                 ||
                                c.FirstName != null && c.FirstName.ToLower().Contains(searchTerm.ToLower())
                                 ||
                                c.LastName != null && c.LastName.ToLower().Contains(searchTerm.ToLower())
                                ||
                                c.LastName != null && c.LastName.ToLower().Contains(searchTerm.ToLower())
                                  ||
                                c.EmailId != null && c.EmailId.ToLower().Contains(searchTerm.ToLower())
                                ||
                                c.PlaceAllocation != null && c.PlaceAllocation.ToLower().Contains(searchTerm.ToLower())
                                  ||
                                c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(searchTerm.ToLower())
                                ||
                                c.DepartmentName != null && c.DepartmentName.ToLower().Contains(searchTerm.ToLower())
                                  ||
                                c.Extenstion != null && c.Extenstion.ToLower().Contains(searchTerm.ToLower())
                                 ||
                                c.Designation != null && c.Designation.ToLower().Contains(searchTerm.ToLower())
                                )).ToListAsync();
            var resulst = (from q in query
                           select new
                           {
                               id = q.UserId,
                               text = q.FirstName + " " + q.LastName + " - " + q.Designation + " - " + q.DepartmentName
                           }).ToList();
            return Json(resulst, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmpIdByUserId(int UserId)
        {
            var user = myapp.tbl_User.Where(l => l.UserId == UserId).SingleOrDefault().EmpId;
            return Json(user, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetheadcountofDailyRoundByEmployees()
        {
            var pl = from r in myapp.tbl_bm_DailyRound
                     join d in myapp.tbl_User on r.CreatedBy equals d.CustomUserId
                     orderby d.FirstName + " " + d.LastName
                     group d by d.FirstName + " " + d.LastName into grp
                     select new { key = grp.Key, cnt = grp.Count() };
            return Json(pl, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetheadcountofBreakDownByEmployees()
        {
            var pl = from r in myapp.tbl_bm_BreakDown
                     join d in myapp.tbl_User on r.AssignTo equals d.EmpId
                     orderby d.FirstName + " " + d.LastName
                     group d by d.FirstName + " " + d.LastName into grp
                     select new { key = grp.Key, cnt = grp.Count() };
            return Json(pl, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetheadcountofBreakDownByEquipment()
        {
            var pl = from r in myapp.tbl_bm_BreakDown
                     join d in myapp.tbl_bm_Asset on r.EquipmentName equals d.Equipment
                     orderby d.Equipment
                     group d by d.Equipment into grp
                     select new { key = grp.Key, cnt = grp.Count() };
            return Json(pl, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetheadcountofPMByEmployee()
        {
            var pmlist = (from p in myapp.tbl_bm_PreventiveMaintenance where p.AssignTo != null && p.AssignTo != "null" && p.AssignTo != "" select p).ToList();
            var usrlist = (from u in myapp.tbl_User select u).ToList();
            var pl = from r in pmlist
                     join d in usrlist on Convert.ToInt32(r.AssignTo) equals d.UserId
                     orderby d.FirstName + " " + d.LastName
                     group d by d.FirstName + " " + d.LastName into grp
                     select new { key = grp.Key, cnt = grp.Count() };
            return Json(pl, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetheadcountofPMByMonth()
        {
            var pl = from r in myapp.tbl_bm_PreventiveMaintenance
                     orderby r.Month
                     group r by r.Month into grp
                     select new { key = grp.Key, cnt = grp.Count() };
            return Json(pl, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetPMSCalender(string start, string end, int LocationId = 0, int DepartmentId = 0, int SubDepartmentId = 0, string Equipment = null)
        {
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(start, "yyyy-MM-dd");
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(end, "yyyy-MM-dd");
            var pm = await (from d in myapp.tbl_bm_Asset
                            where d.PMDue >= dtfrom && d.PMDue <= dtto
                            select d).ToListAsync();

            if (LocationId != 0)
            {
                pm = pm.Where(m => m.LocationId == LocationId).ToList();
            }
            if (DepartmentId != 0)
            {
                pm = pm.Where(m => m.DepartmentId == DepartmentId).ToList();
            }
            if (SubDepartmentId != 0)
            {
                pm = pm.Where(m => m.SubDepartmentId == SubDepartmentId).ToList();
            }
            if (Equipment != "" && Equipment != null)
            {
                pm = pm.Where(m => m.Equipment.Trim() == Equipment.Trim()).ToList();
            }
            var eventList = from e in pm
                            select new
                            {
                                id = e.AssetId,
                                title = e.Equipment + "<br />  " + e.AssetNo,
                                //  start = e.EventDate.Value.ToShortDateString() + " " + e.EventTime,
                                start = e.PMDue.Value.ToString("yyyy-MM-dd"),
                                end = e.PMDue.Value.ToString("yyyy-MM-dd"),
                                color = "#FFA900",
                                allDay = true
                                //resources = Data.Entitylist.Single(en => en.CHlId == e.chlid.ToString()).Department
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetPMAssetDetails(int id)
        {
            //var pm = (from r in myapp.tbl_bm_PreventiveMaintenance
            //          join d in myapp.tbl_bm_Asset on r.AssetNumber equals d.AssetNo
            //          where r.PreventiveMaintenanceId == id
            //          select d).FirstOrDefault();
            var result = await myapp.tbl_bm_Asset.Where(l => l.AssetId == id).SingleOrDefaultAsync();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            if (Request.Form["id"] != null && Request.Form["id"] != "")
            {
                string Id = Request.Form["id"].ToString();
                string checkpublic = Request.Form["public"].ToString();
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
                            tbl_bm_AssetDoument tsk = new tbl_bm_AssetDoument
                            {
                                CreatedBy = User.Identity.Name,
                                CreatedOn = DateTime.Now,
                                DocumentName = fileName,
                                DocumentPath = guidid + fileName,
                                IsPrivate = checkpublic == "true" ? true : false,
                                AssetId = int.Parse(Id)
                            };
                            myapp.tbl_bm_AssetDoument.Add(tsk);
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

        public JsonResult GetListOfFiles(int id)
        {
            List<tbl_bm_AssetDoument> list = myapp.tbl_bm_AssetDoument.Where(l => l.AssetId == id).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DisposeAsset()
        {
            if (Request.Form["id"] != null && Request.Form["id"] != "")
            {
                int Id = Convert.ToInt32(Request.Form["id"].ToString());
                string reason = Request.Form["ReasonForRemoval"].ToString();
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
                            string path = Path.Combine(Server.MapPath("~/Documents/"), guidid + fileName);
                            file.SaveAs(path);
                            var asset = myapp.tbl_bm_Asset.Where(m => m.AssetId == Id).FirstOrDefault();
                            asset.ReasonForRemoval = reason;
                            asset.DisposalDocument = guidid + fileName;
                            asset.ModifiedBy = User.Identity.Name;
                            asset.ModifiedOn = DateTime.Now;
                            asset.AssetStatus = "InActive";
                            myapp.SaveChanges();
                            SendEmailDisposalAsset(asset);
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

        public JsonResult SaveExcel(int LocationId)
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    var locationId = LocationId;
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
                        var newName = fname.Split('.');
                        fname = newName[0] + "." + newName[1];
                        var uploadRootFolderInput = AppDomain.CurrentDomain.BaseDirectory + "\\ExcelUploads";
                        Directory.CreateDirectory(uploadRootFolderInput);
                        var directoryFullPathInput = uploadRootFolderInput;
                        fname = Path.Combine(directoryFullPathInput, fname);
                        file.SaveAs(fname);
                        string xlsFile = fname;
                        string g = "Assettag";
                        string myexceldataquery = String.Format("select * from [{0}$]", g);
                        //string excelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathName + ";Extended Properties=Excel 12.0;";
                        string sexcelconnectionstring = "";
                        if (xlsFile.Contains(".xlsx"))
                        {
                            sexcelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + xlsFile + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
                        }
                        else
                        {
                            sexcelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + xlsFile + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
                        }
                        OleDbConnection oledbconn = new OleDbConnection(sexcelconnectionstring);
                        OleDbCommand oledbcmd = new OleDbCommand(myexceldataquery, oledbconn);
                        oledbconn.Open();
                        OleDbDataReader DR = oledbcmd.ExecuteReader();
                        var deptmain = myapp.tbl_Department.Where(l => l.LocationId == LocationId && l.DepartmentName == "Nursing").SingleOrDefault();
                        while (DR.Read())
                        {
                            try
                            {
                                var departId = 0;
                                var deptName = DR["LOCATION"].ToString().Trim();
                                var dept = myapp.tbl_subdepartment.Where(m => m.LocationId == LocationId && m.DepartmentId == deptmain.DepartmentId &&
                                m.Name.Trim() == deptName).SingleOrDefault();
                                if (dept == null)
                                {
                                    tbl_subdepartment department = new tbl_subdepartment();
                                    department.Name = deptName;
                                    department.DepartmentName = "Nursing";
                                    department.LocationId = locationId;
                                    department.DepartmentId = deptmain.DepartmentId;
                                    myapp.tbl_subdepartment.Add(department);
                                    myapp.SaveChanges();
                                    departId = department.SubDepartmentId;
                                }
                                else
                                {
                                    departId = dept.SubDepartmentId;
                                }
                                tbl_bm_Asset asset = new tbl_bm_Asset();
                                asset.LocationId = LocationId;
                                asset.DepartmentId = deptmain.DepartmentId;
                                asset.SubDepartmentId = departId;
                                if (DR["MANUFACTURE"] != null && DR["MANUFACTURE"] != "" && DR["MANUFACTURE"] != "N/A")
                                    asset.Manufacture = DR["MANUFACTURE"].ToString().Trim();
                                if (DR["EQUIPMENT"] != null && DR["EQUIPMENT"] != "" && DR["EQUIPMENT"] != "N/A")
                                    asset.Equipment = DR["EQUIPMENT"].ToString().Trim();
                                if (DR["MODEL"] != null && DR["MODEL"] != "" && DR["MODEL"] != "N/A")
                                    asset.Model = DR["MODEL"].ToString().Trim();
                                if (DR["SERIALNO"] != null && DR["SERIALNO"] != "" && DR["SERIALNO"] != "N/A")
                                    asset.SerialNo = DR["SERIALNO"].ToString().Trim();
                                if (DR["ASSETNO"] != null && DR["ASSETNO"] != "" && DR["ASSETNO"] != "N/A")
                                    asset.AssetNo = DR["ASSETNO"].ToString().Trim();
                                //if (DR["DOI"] != null && DR["DOI"] != "" && DR["DOI"] != "N/A")
                                //    asset.DateOfInstallation = DR["DOI"].ToString().Trim();
                                if (DR["PMDONE"] != null && DR["PMDONE"] != "" && DR["PMDONE"] != "N/A" && DR["PMDONE"].ToString().Trim() != "")
                                    asset.PMDone = Convert.ToDateTime(DR["PMDONE"].ToString().Trim());
                                if (DR["PMDUE"] != null && DR["PMDUE"] != "" && DR["PMDUE"] != "N/A" && DR["PMDUE"].ToString().Trim() != "")
                                    asset.PMDue = Convert.ToDateTime(DR["PMDUE"].ToString().Trim());
                                if (DR["CALDONE"] != null && DR["CALDONE"] != "" && DR["CALDONE"] != "N/A" && DR["CALDONE"] != string.Empty && DR["CALDONE"].ToString().Trim() != "")
                                    asset.CALLDone = Convert.ToDateTime(DR["CALDONE"].ToString().Trim());
                                if (DR["CALDUE"] != null && DR["CALDUE"] != "" && DR["CALDUE"] != "N/A" && DR["CALDUE"].ToString().Trim() != "")
                                    asset.CALLDue = Convert.ToDateTime(DR["CALDUE"].ToString().Trim());
                                if (DR["DOI"] != null && DR["DOI"] != "" && DR["DOI"] != "N/A")
                                    asset.DateOfInstallation = DR["DOI"].ToString().Trim();
                                if (DR["SERVICEPROVIDERCONTACT"] != null && DR["SERVICEPROVIDERCONTACT"] != "" && DR["SERVICEPROVIDERCONTACT"] != "N/A")
                                    asset.ServiceProviderContact = DR["SERVICEPROVIDERCONTACT"].ToString().Trim();

                                myapp.tbl_bm_Asset.Add(asset);
                                myapp.SaveChanges();
                            }
                            catch (Exception ex)
                            {
                                continue;
                            }
                        }

                        oledbconn.Close();
                        if (System.IO.File.Exists(fname))
                        {
                            System.IO.File.Delete(fname);
                        }

                    }

                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {

                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult SaveAssetTransfer(tbl_bm_AssetTransfer Assetmodel, HttpPostedFileBase document)
        {
            if (document != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(document.FileName);
                string extension = Path.GetExtension(document.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;
                Assetmodel.Document = fileName;
                document.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));
            }
            var asset = myapp.tbl_bm_Asset.Where(m => m.AssetId == Assetmodel.AssetId).FirstOrDefault();
            Assetmodel.FromLocationId = asset.LocationId;
            Assetmodel.FromDepartmentId = asset.DepartmentId;
            Assetmodel.FromSubDepartmentId = asset.SubDepartmentId;
            Assetmodel.CreatedBy = User.Identity.Name;
            Assetmodel.IsActive = true;
            Assetmodel.CreatedOn = DateTime.Now;
            myapp.tbl_bm_AssetTransfer.Add(Assetmodel);
            myapp.SaveChanges();
            SendEmailTransferAsset(Assetmodel);
            asset.LocationId = Assetmodel.ToLocationId;
            asset.DepartmentId = Assetmodel.ToDepartmentId;
            asset.SubDepartmentId = Assetmodel.ToSubDepartmentId;
            asset.ModifiedBy = User.Identity.Name;
            asset.ModifiedOn = DateTime.Now;
            myapp.SaveChanges();

            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetListOfFilesforBM(int id)
        {
            List<tbl_bm_Doument> list = myapp.tbl_bm_Doument.Where(l => l.EntityId == id).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetListOfFilesforDailyRounds()
        {
            if (Request.Form["id"] != null && Request.Form["id"] != "")
            {
                string Id = Request.Form["id"].ToString();
                string checkpublic = Request.Form["public"].ToString();
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
                            string path = Path.Combine(Server.MapPath("~/Documents/"), guidid + fileName);
                            file.SaveAs(path);
                            tbl_bm_Doument tsk = new tbl_bm_Doument
                            {
                                CreatedBy = User.Identity.Name,
                                CreatedOn = DateTime.Now,
                                DocumentName = fileName,
                                DocumentPath = guidid + fileName,
                                IsPrivate = checkpublic == "true" ? true : false,
                                EntityId = int.Parse(Id),
                                EntityType = "Daily Rounds"
                            };
                            myapp.tbl_bm_Doument.Add(tsk);
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

        public void SendEmailbdUserSignOff(tbl_bm_BreakDown model)
        {
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.LocationId && l.EmpType == "Incharge").ToList();
            if (inhcarge != null && inhcarge.Count > 0)
            {
                try
                {
                    string mobilenumber = String.Join(",", myapp.tbl_bm_TeamSetup.Where(l => l.EmpMobile != null && l.EmpMobile != "" && l.LocationId == model.LocationId && (l.EmpType == "Incharge" || l.EmpType == "Engineer")).Select(l => l.EmpMobile).Distinct().ToList());
                    SendSms sms = new SendSms();
                    if (mobilenumber != null && mobilenumber != "")
                    {
                        string message = "Dear Biomedical Team, Breakdown request ID NO " + model.BreakDownId + " has completed, and user sign off taken, please login to Infonet to view the details. Tanishsoft Hrms";
                        sms.SendSmsToEmployee(mobilenumber, message);
                    }
                }
                catch { }
                var userdet = myapp.tbl_User.Where(l => l.CustomUserId == model.CreatedBy).SingleOrDefault();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = userdet.EmailId;
                mailmodel.ccemail = string.Join(",", inhcarge.Select(l => l.EmpEmail).ToList());
                mailmodel.subject = "Break down Completed And Sign Off Break down Id " + model.BreakDownId + "";
                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_BreakDown.html")))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("[[employee_name]]", userdet.FirstName);
                body = body.Replace("[[Location]]", (from v in myapp.tbl_Location where v.LocationId == model.LocationId select v.LocationName).SingleOrDefault());
                body = body.Replace("[[Department]]", (from v in myapp.tbl_Department where v.DepartmentId == model.DepartmentId select v.DepartmentName).SingleOrDefault());
                body = body.Replace("[[Subdepartment]]", (from v in myapp.tbl_subdepartment where v.SubDepartmentId == model.SubDepartmentId select v.Name).SingleOrDefault());
                body = body.Replace("[[AssetNumber]]", model.AssetNumber);
                body = body.Replace("[[EquipmentName]]", model.EquipmentName);
                if (model.WorkDone != null && model.WorkDone != "")
                    body = body.Replace("[[WorkDone]]", model.WorkDone);
                body = body.Replace("[[CallStatus]]", model.CallStatus);
                if (model.StartDate != null)
                {
                    body = body.Replace("[[StartDate]]", model.StartDate.Value.ToString("dd/MM/yyyy") + " " + model.StartTime);
                }
                else
                {
                    body = body.Replace("[[StartDate]]", "");
                }
                if (model.EndDate != null)
                {
                    body = body.Replace("[[EndDate]]", model.EndDate.Value.ToString("dd/MM/yyyy") + " " + model.EndTime);
                }
                else
                {
                    body = body.Replace("[[EndDate]]", "");
                }
                body = body.Replace("[[VerifiedBy]]", (from v in myapp.tbl_User where v.UserId == model.UserEmpId select v.FirstName).SingleOrDefault());
                string completepath = Request.Url.AbsoluteUri + "~/Documents/" + model.UserSignature;
                body = body.Replace("[[Signature]]", completepath);
                body = body.Replace("[[ProblemObserved]]", model.ProblemObserved);
                body = body.Replace("[[FaultyAccessoryOrSpare]]", model.FaultyAccessoryOrSpare);
                body = body.Replace("[[SN_FaultyAccessoryOrSpare]]", model.SN_FaultyAccessoryOrSpare);
                body = body.Replace("[[AccessoryOrSpareReplaced]]", model.SN_AccessoryOrSpareReplaced);
                body = body.Replace("[[SN_AccessoryOrSpareReplaced]]", model.SN_AccessoryOrSpareReplaced);
                mailmodel.body = body;
                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "BioMedical - Helpdesk";
                cm.SendEmail(mailmodel);
            }
        }


        public void SendEmailpmUserSignOff(tbl_bm_PreventiveMaintenance model, string type = null)
        {
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.LocationId && l.EmpType == "Incharge").ToList();
            if (inhcarge != null && inhcarge.Count > 0)
            {

                var id = Convert.ToInt32(model.AssignTo);
                var AssigntoDetails = (from t in myapp.tbl_bm_TeamSetup where t.EmpId == id select t).SingleOrDefault();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = AssigntoDetails.EmpEmail;
                mailmodel.ccemail = string.Join(",", inhcarge.Select(l => l.EmpEmail).ToList());
                if (type == null)
                    mailmodel.subject = "Preventive Maintenance is Completed and User Sign Off Preventive Maintenance Id " + model.PreventiveMaintenanceId + "";
                else
                    mailmodel.subject = "Preventive Maintenance is Completed Preventive Maintenance Id " + model.PreventiveMaintenanceId + "";

                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_PM.html")))
                {
                    body = reader.ReadToEnd();
                }
                var userdet = myapp.tbl_User.Where(l => l.CustomUserId == model.CreatedBy).SingleOrDefault();
                var table = "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + userdet.FirstName + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == model.LocationId select var.LocationName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.LocationId == model.DepartmentId select var.DepartmentName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == model.SubDepartmentId select var.Name).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Equipment:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Equipment + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Asset Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.AssetNumber + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Month:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Month + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Job Done by:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.JobDoneby + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Spare Replaced:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.SpareReplaced + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Start Date & Time:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.StartDate != null ? (model.StartDate.Value.ToString("dd/MM/yyyy") + "  " + model.StartTime) : "" + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>End Date & Time:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.EndDate != null ? (model.EndDate.Value.ToString("dd/MM/yyyy") + "  " + model.EndTime) : "" + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Work Done:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.WorkDoneDescription + "</td></tr>";
                if (type == null)
                {
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sign Off User:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_User where var.UserId == model.UserEmpId select var.FirstName).SingleOrDefault() + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Image:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'><a  target='_blank' href='/Images/signatures/" + model.UserSignature + ".jpg'>View</a></td></tr>";
                }
                table += "</tbody></table>";
                body = body.Replace("[[table]]", table);
                mailmodel.body = body;

                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "BioMedical - Helpdesk";
                cm.SendEmail(mailmodel);
            }
        }

        public void SendEmailDRUserSignOff(tbl_bm_DailyRound model)
        {
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.LocationId && l.EmpType == "Incharge").ToList();
            if (inhcarge != null && inhcarge.Count > 0)
            {
                try
                {
                    string mobilenumber = String.Join(",", myapp.tbl_bm_TeamSetup.Where(l => l.EmpMobile != null && l.EmpMobile != "" && l.LocationId == model.LocationId && (l.EmpType == "Incharge" || l.EmpType == "Engineer")).Select(l => l.EmpMobile).Distinct().ToList());
                    SendSms sms = new SendSms();
                    if (mobilenumber != null && mobilenumber != "")
                    {
                        string message = "Dear Biomedical Team, Daily rounds ID NO " + model.DailyRoundId + " has completed, and user sign off taken. Please login to Infonet to view the details. Tanishsoft Hrms";
                        sms.SendSmsToEmployee(mobilenumber, message);
                    }
                }
                catch { }
                //var inchargedetails = myapp.tbl_User.Where(l => l.UserId == inhcarge.EmpId).SingleOrDefault();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Biomedical@fernandez.foundation";
                var userdet = myapp.tbl_User.Where(l => l.CustomUserId == model.CreatedBy).SingleOrDefault();
                mailmodel.toemail = userdet.EmailId;
                mailmodel.subject = "Daily Rounds Completed And Sign Off Daily Rounds Id: " + model.DailyRoundId + "";
                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_DailyRound.html")))
                {
                    body = reader.ReadToEnd();
                }

                mailmodel.ccemail = string.Join(",", inhcarge.Select(l => l.EmpEmail).ToList());
                body = body.Replace("[[employee_name]]", userdet.FirstName);
                body = body.Replace("[[Location]]", (from v in myapp.tbl_Location where v.LocationId == model.LocationId select v.LocationName).SingleOrDefault());
                body = body.Replace("[[Department]]", (from v in myapp.tbl_Department where v.DepartmentId == model.DepartmentId select v.DepartmentName).SingleOrDefault());
                body = body.Replace("[[Subdepartment]]", (from v in myapp.tbl_subdepartment where v.SubDepartmentId == model.SubDepartmentId select v.Name).SingleOrDefault());
                body = body.Replace("[[WorkDone]]", model.WorkDone);
                body = body.Replace("[[CallStatus]]", model.CallStatus);
                if (model.StartDate != null)
                {
                    body = body.Replace("[[StartDate]]", model.StartDate.Value.ToString("dd/MM/yyyy") + " " + model.StartTime);
                }
                else
                {
                    body = body.Replace("[[StartDate]]", "");
                }
                if (model.EndDate != null)
                {
                    body = body.Replace("[[EndDate]]", model.EndDate.Value.ToString("dd/MM/yyyy") + " " + model.EndTime);
                }
                else
                {
                    body = body.Replace("[[EndDate]]", "");
                }
                body = body.Replace("[[VerifiedBy]]", (from v in myapp.tbl_User where v.UserId == model.UserEmpId select v.FirstName).SingleOrDefault());
                string completepath = Request.Url.AbsoluteUri + "~/Documents/" + model.UserSignature;
                body = body.Replace("[[Signature]]", completepath);
                mailmodel.body = body;
                mailmodel.filepath = "";
                mailmodel.fromname = "BioMedical - Helpdesk";
                cm.SendEmail(mailmodel);
            }
        }

        public ActionResult ManageStaff()
        {
            return View();
        }
        public ActionResult SaveTeamSetup(tbl_bm_TeamSetup model)
        {
            tbl_bm_TeamSetup model1 = new tbl_bm_TeamSetup();
            if (model.Id != 0)
            {
                model1 = myapp.tbl_bm_TeamSetup.Where(n => n.Id == model.Id).FirstOrDefault();
                model1.LocationId = model.LocationId;
                model1.EmpType = model.EmpType;
                model1.EmpId = model.EmpId;
                model1.EmpEmail = model.EmpEmail;
                model1.EmpMobile = model.EmpMobile;
                model1.EmpName = model.EmpName;
            }
            else
            {
                model.CreatedBy = User.Identity.Name;
                model.CreatedOn = DateTime.Now;
                myapp.tbl_bm_TeamSetup.Add(model);
            }


            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteTeamSetup(int id)
        {
            var model = myapp.tbl_bm_TeamSetup.Where(l => l.Id == id).SingleOrDefault();
            myapp.tbl_bm_TeamSetup.Remove(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult Get_TeamSetup(int id)
        {
            var viewmode = myapp.tbl_bm_TeamSetup.Where(m => m.Id == id).FirstOrDefault();
            var EmpId = (from V in myapp.tbl_User where V.UserId == viewmode.EmpId select V.FirstName + " " + V.LastName + " - " + V.Designation + " - " + V.DepartmentName).SingleOrDefault();
            object obj = new { model = viewmode, EmpId = EmpId };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetTeamSetup(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_bm_TeamSetup select d).ToList();
            IEnumerable<tbl_bm_TeamSetup> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.EmpId != null && c.EmpId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join loc in myapp.tbl_Location on c.LocationId equals loc.LocationId
                         join u in myapp.tbl_User on c.EmpId equals u.UserId
                         select new[] {
                                              c.Id.ToString(),
                                              loc.LocationName,
                                              c.EmpType,
                                               u.CustomUserId,
                                              c.EmpName,
                                              c.EmpMobile,
                                              c.EmpEmail,
                         c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEmployeeDetails(int userid)
        {
            var list = myapp.tbl_User.Where(u => u.UserId == userid).SingleOrDefault();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public void SendEmailNewAsset(tbl_bm_Asset model, string type = null)
        {
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.LocationId && l.EmpType == "Incharge").ToList();
            if (inhcarge != null && inhcarge.Count > 0)
            {
                var hod = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.LocationId && l.EmpType == "Hod").ToList();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                var userdet = myapp.tbl_User.Where(l => l.CustomUserId == model.CreatedBy).SingleOrDefault();
                var userdet1 = myapp.tbl_User.Where(l => l.CustomUserId == model.ModifiedBy).SingleOrDefault();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = inhcarge[0].EmpEmail;
                mailmodel.ccemail = string.Join(",", hod.Select(l => l.EmpEmail).ToList());
                if (type == "NEW")
                {
                    mailmodel.subject = "New Asset is created Asset Id " + model.AssetId + "";
                }
                else
                {
                    mailmodel.subject = "Asset is Updated AssetId " + model.AssetId + "";
                }

                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_Asset.html")))
                {
                    body = reader.ReadToEnd();
                }
                if (type == "NEW")
                    body = body.Replace("[[Heading]]", "Biomedical- New Asset Details");
                else
                    body = body.Replace("[[Heading]]", "Biomedical- Updated Asset Details");

                var table = "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + userdet.FirstName + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == model.LocationId select var.LocationName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.DepartmentId == model.DepartmentId select var.DepartmentName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == model.SubDepartmentId select var.Name).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Equipment:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Equipment + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>manufacture Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Manufacture + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Model</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Model + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Serial Number</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.SerialNo + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Asset No</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.AssetNo + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Service Provider Contact</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ServiceProviderContact + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Comments</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Comments + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Date of Install</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.DateOfInstallation + "</td></tr>";
                table += "</tbody></table>";
                body = body.Replace("[[table]]", table);
                mailmodel.body = body;

                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "Biomedical-Asset";
                cm.SendEmail(mailmodel);
            }
        }

        public void SendEmailTransferAsset(tbl_bm_AssetTransfer Tmodel)
        {
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == Tmodel.ToLocationId && l.EmpType == "Incharge").ToList();
            if (inhcarge != null && inhcarge.Count > 0)
            {
                //var inchargedetails = myapp.tbl_User.Where(l => l.UserId == inhcarge.EmpId).SingleOrDefault();
                var Hod = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == Tmodel.ToLocationId && l.EmpType == "Hod").ToList();
                var model = myapp.tbl_bm_Asset.Where(m => m.AssetId == Tmodel.AssetId).FirstOrDefault();

                try
                {
                    string LocationName = myapp.tbl_Location.Where(l => l.LocationId == Tmodel.ToLocationId).SingleOrDefault().LocationName;
                    string fLocationName = myapp.tbl_Location.Where(l => l.LocationId == Tmodel.FromLocationId).SingleOrDefault().LocationName;
                    string mobilenumber = String.Join(",", myapp.tbl_bm_TeamSetup.Where(l => l.EmpMobile != null && l.EmpMobile != "" && l.LocationId == Tmodel.ToLocationId).Select(l => l.EmpMobile).Distinct().ToList());
                    SendSms sms = new SendSms();
                    if (mobilenumber != null && mobilenumber != "")
                    {
                        string message = "Dear Biomedical Team, New Asset is Transferred from " + fLocationName + " to " + LocationName + ", please login to Infonet to view the details. Tanishsoft Hrms";
                        sms.SendSmsToEmployee(mobilenumber, message);
                    }
                }
                catch { }

                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                var userdet = myapp.tbl_User.Where(l => l.CustomUserId == Tmodel.CreatedBy).SingleOrDefault();
                mailmodel.toemail = inhcarge[0].EmpEmail;
                mailmodel.ccemail = string.Join(",", Hod.Select(l => l.EmpEmail).ToList());

                mailmodel.subject = "New Asset is created Asset Id " + model.AssetId + "";


                mailmodel.subject = "Asset Transfered " + model.AssetId + "";

                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_Asset.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("[[Heading]]", "Biomedical-Asset Transfer Details");
                var table = "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Transfer Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + userdet.FirstName + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>From Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == model.LocationId select var.LocationName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>To Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == Tmodel.ToLocationId select var.LocationName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>From Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.DepartmentId == model.DepartmentId select var.DepartmentName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>To Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.DepartmentId == Tmodel.ToDepartmentId select var.DepartmentName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>From Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == model.SubDepartmentId select var.Name).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>To Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == Tmodel.ToSubDepartmentId select var.Name).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Equipment:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Equipment + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>manufacture Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Manufacture + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Model</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Model + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Serial Number</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.SerialNo + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Asset No</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.AssetNo + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Date of Install</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.DateOfInstallation + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Service Provider Contact</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ServiceProviderContact + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Comments</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Comments + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>ReasonForTranfer</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + Tmodel.ReasonForTranfer + "</td></tr>";
                table += "</tbody></table>";
                body = body.Replace("[[table]]", table);
                mailmodel.body = body;

                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "Biomedical-Asset";
                cm.SendEmail(mailmodel);
            }
        }

        public void SendEmailDisposalAsset(tbl_bm_Asset model)
        {
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.LocationId && l.EmpType == "Incharge").ToList();
            if (inhcarge != null && inhcarge.Count > 0)
            {

                var Hod = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.LocationId && l.EmpType == "Hod").ToList();
                try
                {
                    string LocationName = myapp.tbl_Location.Where(l => l.LocationId == model.LocationId).SingleOrDefault().LocationName;
                    string mobilenumber = String.Join(",", myapp.tbl_bm_TeamSetup.Where(l => l.EmpMobile != null && l.EmpMobile != "" && l.LocationId == model.LocationId).Select(l => l.EmpMobile).Distinct().ToList());
                    SendSms sms = new SendSms();
                    if (mobilenumber != null && mobilenumber != "")
                    {
                        string message = "Dear Biomedical Team, An Asset is disposed from location " + LocationName + ", please login to Infonet to view the details. Tanishsoft Hrms";
                        sms.SendSmsToEmployee(mobilenumber, message);
                    }
                }
                catch { }
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                var userdet = myapp.tbl_User.Where(l => l.CustomUserId == model.ModifiedBy).SingleOrDefault();
                mailmodel.toemail = inhcarge[0].EmpEmail;
                mailmodel.ccemail = string.Join(",", Hod.Select(l => l.EmpEmail).ToList());
                mailmodel.subject = "Asset is Disposed " + model.AssetId + "";

                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_Asset.html")))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("[[Heading]]", "Biomedical-  Disposal Asset Details");

                var table = "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + userdet.FirstName + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == model.LocationId select var.LocationName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.DepartmentId == model.DepartmentId select var.DepartmentName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == model.SubDepartmentId select var.Name).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Equipment:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Equipment + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>manufacture Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Manufacture + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Model</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Model + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Serial Number</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.SerialNo + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Asset No</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.AssetNo + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Date of Install</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.DateOfInstallation + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Service Provider Contact</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ServiceProviderContact + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Comments</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Comments + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>ReasonForRemoval</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ReasonForRemoval + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>AssetStatus</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.AssetStatus + "</td></tr>";
                table += "</tbody></table>";
                body = body.Replace("[[table]]", table);
                mailmodel.body = body;

                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "BioMedical- Asset details";
                cm.SendEmail(mailmodel);
            }
        }
        public void SendEmailForPendingBreakDown()
        {
            var model = myapp.tbl_bm_BreakDown.Where(n => n.CallStatus == "pending").ToList();
            var inhcarge = "";// myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == m.LocationId && l.EmpType == "Incharge").SingleOrDefault();
            if (inhcarge != null)
            {
                //var inchargedetails = myapp.tbl_User.Where(l => l.UserId == inhcarge.EmpId).SingleOrDefault();
                //var allEngineer = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == m.LocationId && l.EmpType == "Engineer").Select(l => l.EmpId).ToList();
                //var allEngineeretails = myapp.tbl_User.Where(l => allEngineer.Contains(l.UserId)).Select(l => l.EmailId).ToList();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = "phanisrinivas111@gmail.com";
                //mailmodel.toemail = inchargedetails.EmailId;
                //mailmodel.ccemail = string.Join(",", allEngineeretails);
                mailmodel.subject = "Daily Reminder for Pending Break Downs ";
                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/BreakDown_Remainder.html")))
                {
                    body = reader.ReadToEnd();
                }
                var table = "";
                foreach (var m in model)
                {
                    var userdet = myapp.tbl_User.Where(l => l.CustomUserId == m.CreatedBy).SingleOrDefault();
                    table += "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + userdet.FirstName + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == m.LocationId select var.LocationName).SingleOrDefault() + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.LocationId == m.DepartmentId select var.DepartmentName).SingleOrDefault() + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == m.SubDepartmentId select var.Name).SingleOrDefault() + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Equipment:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.EquipmentName + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Asset Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.AssetNumber + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Work Done:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.WorkDone + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>CallStatus:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.CallStatus + "</td></tr>";
                    if (m.StartDate != null)
                    {
                        table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Start Date & Time:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.StartDate.Value.ToString("dd/MM/yyyy") + " " + m.StartTime + "</td></tr>";
                    }
                    else
                    {
                        table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Start Date & Time:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>&nbsp</td></tr>";

                    }
                    if (m.EndDate != null)
                    {
                        table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>End Date & Time:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.EndDate.Value.ToString("dd/MM/yyyy") + " " + m.EndDate + "</td></tr>";

                    }
                    else
                    {
                        table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Start Date & Time:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>&nbsp</td></tr>";

                    }
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Problem Observed:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.ProblemObserved + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>FaultyAccessoryOrSpare:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.FaultyAccessoryOrSpare + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>SN_FaultyAccessoryOrSpare:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.SN_FaultyAccessoryOrSpare + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>AccessoryOrSpareReplaced:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.AccessoryOrSpareReplaced + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>SN_AccessoryOrSpareReplaced:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.SN_AccessoryOrSpareReplaced + "</td></tr>";

                    table += "</tbody></table>";
                }

                body = body.Replace("[[table]]", table);

                mailmodel.body = body;
                mailmodel.filepath = "";
                mailmodel.fromname = "BioMedical - Helpdesk";
                cm.SendEmail(mailmodel);
            }
        }
        public void SendEmailForPendingPM()
        {
            var model = myapp.tbl_bm_PreventiveMaintenance.Where(m => m.CallStatus == "pending").ToList();

            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model[0].LocationId && l.EmpType == "Incharge").SingleOrDefault();
            if (inhcarge != null)
            {
                var inchargedetails = myapp.tbl_User.Where(l => l.UserId == inhcarge.EmpId).SingleOrDefault();
                var allEngineer = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model[0].LocationId && l.EmpType == "Engineer").Select(l => l.EmpId).ToList();
                var allEngineeretails = myapp.tbl_User.Where(l => allEngineer.Contains(l.UserId)).Select(l => l.EmailId).ToList();

                //var id = Convert.ToInt32(AssignTo);
                //var AssigntoDetails = (from var in myapp.tbl_User where var.UserId == id select var).SingleOrDefault();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = inchargedetails.EmailId;
                //mailmodel.toemail = AssigntoDetails.EmailId;
                mailmodel.ccemail = string.Join(",", allEngineeretails);
                mailmodel.subject = "New Preventive Maintenance is assigned";
                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_PM.html")))
                {
                    body = reader.ReadToEnd();
                }
                var table = "";
                foreach (var m in model)
                {
                    var userdet = myapp.tbl_User.Where(l => l.CustomUserId == m.CreatedBy).SingleOrDefault();
                    table += "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + userdet.FirstName + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == m.LocationId select var.LocationName).SingleOrDefault() + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.LocationId == m.DepartmentId select var.DepartmentName).SingleOrDefault() + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == m.SubDepartmentId select var.Name).SingleOrDefault() + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Equipment:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.Equipment + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Asset Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.AssetNumber + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Month:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.Month + "</td></tr>";

                    table += "</tbody></table>";
                }
                body = body.Replace("[[table]]", table);
                mailmodel.body = body;
                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "BioMedical - Helpdesk";
                cm.SendEmail(mailmodel);
            }
        }
        public void PMSendEmailNotCompleated()
        {
            var model = myapp.tbl_bm_PreventiveMaintenance.Where(m => m.CallStatus == "pending").ToList();
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model[0].LocationId && l.EmpType == "Incharge").SingleOrDefault();
            if (inhcarge != null)
            {
                var inchargedetails = myapp.tbl_User.Where(l => l.UserId == inhcarge.EmpId).SingleOrDefault();
                var allEngineer = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model[0].LocationId && l.EmpType == "Engineer").Select(l => l.EmpId).ToList();
                var allEngineeretails = myapp.tbl_User.Where(l => allEngineer.Contains(l.UserId)).Select(l => l.EmailId).ToList();

                //var id = Convert.ToInt32(AssignTo);
                //var AssigntoDetails = (from var in myapp.tbl_User where var.UserId == id select var).SingleOrDefault();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = inchargedetails.EmailId;
                //mailmodel.toemail = AssigntoDetails.EmailId;
                mailmodel.ccemail = string.Join(",", allEngineeretails);
                mailmodel.subject = "PM Not Compleated For Perticular Month";
                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_PM.html")))
                {
                    body = reader.ReadToEnd();
                }
                var table = "";
                foreach (var m in model)
                {
                    var userdet = myapp.tbl_User.Where(l => l.CustomUserId == m.CreatedBy).SingleOrDefault();
                    table += "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + userdet.FirstName + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == m.LocationId select var.LocationName).SingleOrDefault() + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.LocationId == m.DepartmentId select var.DepartmentName).SingleOrDefault() + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == m.SubDepartmentId select var.Name).SingleOrDefault() + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Equipment:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.Equipment + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Asset Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.AssetNumber + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Month:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.Month + "</td></tr>";

                    table += "</tbody></table>";
                }
                body = body.Replace("[[table]]", table);
                mailmodel.body = body;
                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "BioMedical - Helpdesk";
                cm.SendEmail(mailmodel);
            }
        }
        public ActionResult DailyRoundUserSignOff(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult BreakdownUserSignOff(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult PMUserSignOff(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public JsonResult BulkSave_Update_PM_CallDue(int locationId, int deptId, int subdeptId, string calldue, string calldone, string equipment = null)
        {
            var list = myapp.tbl_bm_Asset.Where(m => m.LocationId == locationId && m.DepartmentId == deptId && m.SubDepartmentId == subdeptId).ToList();
            if (equipment != null)
            {
                list = list.Where(n => n.Equipment == equipment).ToList();
            }
            foreach (var item in list)
            {
                item.CALLDone = ProjectConvert.ConverDateStringtoDatetime(calldone);
                item.CALLDue = ProjectConvert.ConverDateStringtoDatetime(calldue);
                item.ModifiedBy = User.Identity.Name;
                item.ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
            }
            return Json("Successfully updated", JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckSpareforlastthreeMonths(string spare)
        {
            var result = "success";
            var date = DateTime.Now.AddMonths(-3);
            var list = myapp.tbl_bm_BreakDown.Where(p => p.CreatedOn >= date && date <= p.CreatedOn && p.SN_AccessoryOrSpareReplaced == spare).ToList();
            if (list.Count != 0)
            {
                result = "fail";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void SendEmailForPMDUEReminder()
        {
            var date = DateTime.Now.AddMonths(-1);
            var model = myapp.tbl_bm_PreventiveMaintenance.Where(m => m.CallStatus == "pending").ToList();
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model[0].LocationId && l.EmpType == "Incharge").SingleOrDefault();
            if (inhcarge != null)
            {
                var inchargedetails = myapp.tbl_User.Where(l => l.UserId == inhcarge.EmpId).SingleOrDefault();
                var allEngineer = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model[0].LocationId && l.EmpType == "Engineer").Select(l => l.EmpId).ToList();
                var allEngineeretails = myapp.tbl_User.Where(l => allEngineer.Contains(l.UserId)).Select(l => l.EmailId).ToList();

                //var id = Convert.ToInt32(AssignTo);
                //var AssigntoDetails = (from var in myapp.tbl_User where var.UserId == id select var).SingleOrDefault();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = inchargedetails.EmailId;
                //mailmodel.toemail = AssigntoDetails.EmailId;
                mailmodel.ccemail = string.Join(",", allEngineeretails);
                mailmodel.subject = "New Preventive Maintenance is assigned";
                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_PM.html")))
                {
                    body = reader.ReadToEnd();
                }
                var table = "";
                foreach (var m in model)
                {
                    var userdet = myapp.tbl_User.Where(l => l.CustomUserId == m.CreatedBy).SingleOrDefault();
                    table += "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + userdet.FirstName + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == m.LocationId select var.LocationName).SingleOrDefault() + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.LocationId == m.DepartmentId select var.DepartmentName).SingleOrDefault() + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == m.SubDepartmentId select var.Name).SingleOrDefault() + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Equipment:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.Equipment + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Asset Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.AssetNumber + "</td></tr>";
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Month:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + m.Month + "</td></tr>";

                    table += "</tbody></table>";
                }
                body = body.Replace("[[table]]", table);
                mailmodel.body = body;
                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "BioMedical - Reminder";
                cm.SendEmail(mailmodel);
            }
        }
        public async Task<ActionResult> Save_Update_ShifitingRequest(tbl_bm_ShiftingRequest model)
        {
            try
            {
                tbl_bm_ShiftingRequest _BreakDown = new tbl_bm_ShiftingRequest();
                if (model.ShiftingRequestId != 0)
                {
                    _BreakDown = await myapp.tbl_bm_ShiftingRequest.Where(m => m.ShiftingRequestId == model.ShiftingRequestId).FirstOrDefaultAsync();
                    _BreakDown.ModifiedBy = User.Identity.Name;
                    _BreakDown.ModifiedOn = DateTime.Now;
                }
                _BreakDown.RequestForDepartment = model.RequestForDepartment;
                _BreakDown.RequestForLocation = model.RequestForLocation;
                _BreakDown.RequestDoctor = model.RequestDoctor.Trim();
                _BreakDown.RequestItem = model.RequestItem;
                _BreakDown.RequestComments = model.RequestComments;
                _BreakDown.RequestEmpId = Convert.ToInt32(User.Identity.Name);
                _BreakDown.RequestDate = DateTime.Now;

                if (model.ShiftingRequestId == 0)
                {
                    _BreakDown.CreatedBy = User.Identity.Name;
                    _BreakDown.CreatedOn = DateTime.Now;
                    myapp.tbl_bm_ShiftingRequest.Add(_BreakDown);
                    await myapp.SaveChangesAsync();
                    SendEmailNewShiftRequest(_BreakDown, "NEW");
                }
                else
                {
                    await myapp.SaveChangesAsync();
                    SendEmailNewShiftRequest(_BreakDown, "UPDATE");
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetShifitingRequest(int id)
        {
            var dbModel = myapp.tbl_bm_ShiftingRequest.Where(m => m.ShiftingRequestId == id).FirstOrDefault();
            tbl_bm_ShiftingRequestViewModel model = new tbl_bm_ShiftingRequestViewModel();
            model.ConditionOfEquipment = dbModel.ConditionOfEquipment;
            model.HandOvercomments = dbModel.HandOvercomments;
            model.ReceivedTime = dbModel.ReceivedTime;
            model.HandoverEmp = dbModel.HandoverEmp;
            model.HandOverSignature = dbModel.HandOverSignature;
            model.PatientMrNo = dbModel.PatientMrNo;
            model.PickupComments = dbModel.PickupComments;
            model.PickupDepartment = dbModel.PickupDepartment;
            model.PickupLocation = dbModel.PickupLocation;
            if (dbModel.ReceivedDate != null)
                model.ReceivedDate = ProjectConvert.ConverDateTimeToString(dbModel.ReceivedDate.Value);
            model.ReceivedMakeModelAssetno = dbModel.ReceivedMakeModelAssetno;
            model.ReceivedTime = dbModel.ReceivedTime;
            model.Remarks = dbModel.Remarks;
            model.RequestComments = dbModel.RequestComments;
            if (dbModel.RequestDate != null)
                model.RequestDate = ProjectConvert.ConverDateTimeToString(dbModel.RequestDate.Value);
            model.RequestDoctor = dbModel.RequestDoctor;
            model.RequestEmpId = dbModel.RequestEmpId;
            model.RequestForDepartment = dbModel.RequestForDepartment;
            model.RequestForLocation = dbModel.RequestForLocation;
            model.RequestForSubDepartment = dbModel.RequestForSubDepartment;
            model.RequestItem = dbModel.RequestItem;
            model.ReturnComments = dbModel.ReturnComments;
            model.ReturnConditionOfEquipment = dbModel.ReturnConditionOfEquipment;
            if (dbModel.ReturnDate != null)
                model.ReturnDate = ProjectConvert.ConverDateTimeToString(dbModel.ReturnDate.Value);
            model.ReturnEmpId = dbModel.ReturnEmpId;
            model.ReturnTime = dbModel.ReturnTime;
            model.ShiftingRequestId = id;
            model.Status = dbModel.Status;
            if (dbModel.UsageEndDate != null)
                model.UsageEndDate = ProjectConvert.ConverDateTimeToString(dbModel.UsageEndDate.Value);
            model.UsageEndTime = dbModel.UsageEndTime;
            if (dbModel.UsageStartDate != null)
                model.UsageStartDate = ProjectConvert.ConverDateTimeToString(dbModel.UsageStartDate.Value);
            model.UsageStartTime = dbModel.UsageStartTime;
            model.UsedOnPatient = dbModel.UsedOnPatient;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> AjaxGetShiftingRequest(JQueryDataTableParamModel param)
        {

            List<tbl_bm_ShiftingRequest> query = await (from d in myapp.tbl_bm_ShiftingRequest select d).ToListAsync();
            var userdet = myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).SingleOrDefault();
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.EmpId == userdet.UserId).Count();
            if (inhcarge > 0)
            {
            }
            else
            {
                query = query.Where(l => l.RequestForLocation == userdet.LocationId).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (param.locationid != null && param.locationid != 0)
            {
                query = query.Where(m => m.RequestForLocation == param.locationid).ToList();
            }
            if (param.departmentid != null && param.departmentid != 0)
            {
                query = query.Where(m => m.RequestForDepartment == param.departmentid).ToList();
            }
            if (param.subdepartmentid != null && param.subdepartmentid != 0)
            {
                query = query.Where(m => m.RequestForSubDepartment == param.subdepartmentid).ToList();
            }
            if (param.Emp != null && param.Emp != "")
            {
                var empId = Convert.ToInt32(param.Emp);
                var id = await (from u in myapp.tbl_User where u.UserId == empId select u.EmpId).SingleOrDefaultAsync();
                query = query.Where(m => m.RequestEmpId == id).ToList();
            }
            query = query.OrderByDescending(m => m.ShiftingRequestId).ToList();
            IEnumerable<tbl_bm_ShiftingRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.ShiftingRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.RequestDoctor != null && c.RequestDoctor.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.RequestItem != null && c.RequestItem.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.RequestForLocation != null && c.RequestForLocation.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_bm_ShiftingRequest> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           join l in myapp.tbl_Location on c.RequestForLocation equals l.LocationId
                                           join d in myapp.tbl_Department on c.RequestForDepartment equals d.DepartmentId
                                           join u in myapp.tbl_User on c.RequestEmpId equals u.EmpId
                                           select new object[] {
                                             c.CreatedOn.Value.ToString("ddMMyyyy")+"_"+c.ShiftingRequestId,
                                              l.LocationName,
                                              d.DepartmentName ,
                                              c.RequestDoctor,

                                              c.RequestItem,
                                              c.RequestDate.HasValue?c.RequestDate.Value.ToString("dd/MM/yyyy"):"",
                                              u.FirstName,
                                              c.ShiftingRequestId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }


        public async Task<ActionResult> Save_Update_RetunUpdate(tbl_bm_ShiftingRequestViewModel model)
        {
            try
            {
                tbl_bm_ShiftingRequest _BreakDown = new tbl_bm_ShiftingRequest();
                if (model.ShiftingRequestId != 0)
                {
                    _BreakDown = await myapp.tbl_bm_ShiftingRequest.Where(m => m.ShiftingRequestId == model.ShiftingRequestId).FirstOrDefaultAsync();
                    _BreakDown.ModifiedBy = User.Identity.Name;
                    _BreakDown.ModifiedOn = DateTime.Now;
                }
                _BreakDown.ReturnConditionOfEquipment = model.ConditionOfEquipment;
                _BreakDown.ReturnDate = ProjectConvert.ConverDateStringtoDatetime(model.ReturnDate);
                _BreakDown.ReturnTime = model.ReturnTime;
                _BreakDown.Status = model.Status;
                _BreakDown.ReturnEmpId = Convert.ToInt32(User.Identity.Name);


                await myapp.SaveChangesAsync();
                SendEmailReturn(_BreakDown.ShiftingRequestId);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> Save_Update_UsageDetailsByUser(tbl_bm_ShiftingRequestViewModel model)
        {
            try
            {
                tbl_bm_ShiftingRequest _BreakDown = new tbl_bm_ShiftingRequest();
                if (model.ShiftingRequestId != 0)
                {
                    _BreakDown = await myapp.tbl_bm_ShiftingRequest.Where(m => m.ShiftingRequestId == model.ShiftingRequestId).FirstOrDefaultAsync();
                    _BreakDown.ModifiedBy = User.Identity.Name;
                    _BreakDown.ModifiedOn = DateTime.Now;
                }
                _BreakDown.UsedOnPatient = model.UsedOnPatient;
                _BreakDown.UsageStartDate = ProjectConvert.ConverDateStringtoDatetime(model.UsageStartDate);
                _BreakDown.UsageEndDate = ProjectConvert.ConverDateStringtoDatetime(model.UsageEndDate);
                _BreakDown.UsageEndTime = model.UsageEndTime;
                _BreakDown.UsageStartTime = model.UsageStartTime;
                _BreakDown.PatientMrNo = model.PatientMrNo;


                await myapp.SaveChangesAsync();

                SendEmailUsageUser(_BreakDown.ShiftingRequestId);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }


        public async Task<ActionResult> Save_Update_PickUpUpdate(tbl_bm_ShiftingRequestViewModel model)
        {
            try
            {
                tbl_bm_ShiftingRequest _BreakDown = new tbl_bm_ShiftingRequest();
                if (model.ShiftingRequestId != 0)
                {
                    _BreakDown = await myapp.tbl_bm_ShiftingRequest.Where(m => m.ShiftingRequestId == model.ShiftingRequestId).FirstOrDefaultAsync();
                    _BreakDown.ModifiedBy = User.Identity.Name;
                    _BreakDown.ModifiedOn = DateTime.Now;
                }
                _BreakDown.PickupLocation = model.PickupLocation;
                _BreakDown.ReceivedDate = ProjectConvert.ConverDateStringtoDatetime(model.ReceivedDate);
                _BreakDown.ReceivedMakeModelAssetno = model.ReceivedMakeModelAssetno;
                _BreakDown.ConditionOfEquipment = model.ConditionOfEquipment;
                _BreakDown.HandOvercomments = model.HandOvercomments;
                _BreakDown.ReceivedTime = model.ReceivedTime;
                _BreakDown.HandoverEmp = Convert.ToInt32(User.Identity.Name);


                await myapp.SaveChangesAsync();
                SendEmailPickUpUpdate(_BreakDown.ShiftingRequestId);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error", JsonRequestBehavior.AllowGet);
            }
        }
        public void SendEmailNewShiftRequest(tbl_bm_ShiftingRequest model, string type = null)
        {
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.RequestForLocation && l.EmpType == "Incharge").ToList();
            if (inhcarge != null && inhcarge.Count > 0)
            {
                var hod = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.RequestForLocation && l.EmpType == "Hod").ToList();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                var userdet = myapp.tbl_User.Where(l => l.EmpId == model.RequestEmpId).SingleOrDefault();
                var userdet1 = myapp.tbl_User.Where(l => l.EmpId == model.RequestEmpId).SingleOrDefault();
                mailmodel.fromemail = "Leave@hospitals.com";
                //mailmodel.toemail = inhcarge[0].EmpEmail;
                //mailmodel.ccemail = string.Join(",", hod.Select(l => l.EmpEmail).ToList());
                mailmodel.toemail = "phanisrinivas111@gmail.com";
                if (type == "NEW")
                {
                    mailmodel.subject = "New Shift Request is created  Id " + model.ShiftingRequestId + "";
                }
                else
                {
                    mailmodel.subject = "Shift Request is Updated Id " + model.ShiftingRequestId + "";
                }

                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/BioMedical_NewShiftRequest.html")))
                {
                    body = reader.ReadToEnd();
                }
                if (type == "NEW")
                    body = body.Replace("[[Heading]]", "Biomedical- New Shift Request Details");
                else
                    body = body.Replace("[[Heading]]", "Biomedical- Shift Request Details");

                var table = "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + userdet.FirstName + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == model.RequestForLocation select var.LocationName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.DepartmentId == model.RequestForDepartment select var.DepartmentName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == model.RequestForSubDepartment select var.Name).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Request Item:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestItem + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Request Doctor:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestDoctor + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Request Date</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Comments</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestComments + "</td></tr>";

                table += "</tbody></table>";
                body = body.Replace("[[table]]", table);
                mailmodel.body = body;

                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "Biomedical-Shift Request";
                cm.SendEmail(mailmodel);
            }
        }


        public void SendEmailPickUpUpdate(int id)
        {
            var model = myapp.tbl_bm_ShiftingRequest.Where(m => m.ShiftingRequestId == id).FirstOrDefault();
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.PickupLocation && l.EmpType == "Incharge").ToList();
            if (inhcarge != null && inhcarge.Count > 0)
            {
                var hod = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.PickupLocation && l.EmpType == "Hod").ToList();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                var userdet = myapp.tbl_User.Where(l => l.CustomUserId == model.CreatedBy).SingleOrDefault();
                mailmodel.fromemail = "Leave@hospitals.com";
                //mailmodel.toemail = inhcarge[0].EmpEmail;
                //mailmodel.ccemail = string.Join(",", hod.Select(l => l.EmpEmail).ToList());
                mailmodel.toemail = "phanisrinivas111@gmail.com";

                mailmodel.subject = "Pick up Details for Shift Request Id " + model.ShiftingRequestId + "";


                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/BioMedical_NewShiftRequest.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("[[Heading]]", "Biomedical- Pick Up Details");


                var table = "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + userdet.FirstName + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == model.RequestForLocation select var.LocationName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.DepartmentId == model.RequestForDepartment select var.DepartmentName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == model.RequestForSubDepartment select var.Name).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Request Item:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestItem + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Request Doctor:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestDoctor + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Request Date</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Comments</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestComments + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == model.RequestForLocation select var.LocationName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Received Make Model:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ReceivedMakeModelAssetno + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>ReceivedDate:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ReceivedDate.Value.ToString("dd/MM/yyyy") + "" + model.ReceivedTime + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Condition Of Equipment</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ConditionOfEquipment + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Hand Over</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.HandOvercomments + "</td></tr>";

                table += "</tbody></table>";
                body = body.Replace("[[table]]", table);
                mailmodel.body = body;

                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "Biomedical-Shift Request";
                cm.SendEmail(mailmodel);
            }
        }
        public void SendEmailUsageUser(int id)
        {
            var model = myapp.tbl_bm_ShiftingRequest.Where(m => m.ShiftingRequestId == id).FirstOrDefault();
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.PickupLocation && l.EmpType == "Incharge").ToList();
            if (inhcarge != null && inhcarge.Count > 0)
            {
                var hod = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.PickupLocation && l.EmpType == "Hod").ToList();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                var userdet = myapp.tbl_User.Where(l => l.CustomUserId == model.CreatedBy).SingleOrDefault();
                mailmodel.fromemail = "Leave@hospitals.com";
                //mailmodel.toemail = inhcarge[0].EmpEmail;
                //mailmodel.ccemail = string.Join(",", hod.Select(l => l.EmpEmail).ToList());
                mailmodel.toemail = "phanisrinivas111@gmail.com";

                mailmodel.subject = "Pick up Details for Shift Request Id " + model.ShiftingRequestId + "";


                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/BioMedical_NewShiftRequest.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("[[Heading]]", "Biomedical- Pick Up Details");


                var table = "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + userdet.FirstName + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == model.RequestForLocation select var.LocationName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.DepartmentId == model.RequestForDepartment select var.DepartmentName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == model.RequestForSubDepartment select var.Name).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Request Item:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestItem + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Request Doctor:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestDoctor + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Request Date</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Comments</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestComments + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Patient :</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.UsedOnPatient + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Patient Mr No:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.PatientMrNo + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Usage Start Date & Time:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.UsageStartDate.Value.ToString("dd/MM/yyyy") + "" + model.UsageStartTime + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Usage Start Date & Time:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.UsageEndDate.Value.ToString("dd/MM/yyyy") + "" + model.UsageEndTime + "</td></tr>";

                table += "</tbody></table>";
                body = body.Replace("[[table]]", table);
                mailmodel.body = body;

                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "Biomedical-Shift Request";
                cm.SendEmail(mailmodel);
            }
        }


        public void SendEmailReturn(int id)
        {
            var model = myapp.tbl_bm_ShiftingRequest.Where(m => m.ShiftingRequestId == id).FirstOrDefault();
            var inhcarge = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.PickupLocation && l.EmpType == "Incharge").ToList();
            if (inhcarge != null && inhcarge.Count > 0)
            {
                var hod = myapp.tbl_bm_TeamSetup.Where(l => l.LocationId == model.PickupLocation && l.EmpType == "Hod").ToList();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                var userdet = myapp.tbl_User.Where(l => l.CustomUserId == model.CreatedBy).SingleOrDefault();
                mailmodel.fromemail = "Leave@hospitals.com";
                //mailmodel.toemail = inhcarge[0].EmpEmail;
                //  mailmodel.ccemail = string.Join(",", hod.Select(l => l.EmpEmail).ToList());
                mailmodel.toemail = "phanisrinivas111@gmail.com";
                mailmodel.subject = "Pick up Details for Shift Request Id " + model.ShiftingRequestId + "";


                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/BioMedical_NewShiftRequest.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("[[Heading]]", "Biomedical- Pick Up Details");


                var table = "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + userdet.FirstName + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == model.RequestForLocation select var.LocationName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Department where var.DepartmentId == model.RequestForDepartment select var.DepartmentName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sub Department Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_subdepartment where var.SubDepartmentId == model.RequestForSubDepartment select var.Name).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Request Item:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestItem + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Request Doctor:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestDoctor + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Request Date</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Comments</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RequestComments + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Condition Of Equipment:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ReturnConditionOfEquipment + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Status:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Status + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Return Start Date & Time:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ReturnDate.Value.ToString("dd/MM/yyyy") + "" + model.ReturnTime + "</td></tr>";

                table += "</tbody></table>";
                body = body.Replace("[[table]]", table);
                mailmodel.body = body;

                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "Biomedical-Shift Request";
                cm.SendEmail(mailmodel);
            }
        }
        public JsonResult ExportExcelShiftingRequest(string fromDate, string toDate, int id = 0, int locationId = 0, int departmentId = 0, int subdepartmentId = 0)
        {

            List<tbl_bm_ShiftingRequest> query = (from d in myapp.tbl_bm_ShiftingRequest select d).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (id != null && id != 0)
            {
                var uid = (from u in myapp.tbl_User where u.UserId == id select u.EmpId).SingleOrDefault();
                query = query.Where(m => m.RequestEmpId == uid).ToList();
            }
            if (locationId != null && locationId != 0)
            {
                query = query.Where(m => m.RequestForLocation == locationId).ToList();
            }
            if (departmentId != null && departmentId != 0)
            {
                query = query.Where(m => m.RequestForDepartment == departmentId).ToList();
            }
            var products = new System.Data.DataTable("Shifiting Request");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location Id", typeof(string));
            products.Columns.Add("Department Id ", typeof(string));
            products.Columns.Add("Request Doctor", typeof(string));
            products.Columns.Add("Request Item", typeof(string));
            products.Columns.Add("Request Date", typeof(string));
            products.Columns.Add("Request Comments", typeof(string));
            products.Columns.Add("Request User", typeof(string));
            products.Columns.Add("Admin Comments", typeof(string));
            products.Columns.Add("UserEmpId", typeof(string));

            products.Columns.Add("Pickup Location", typeof(string));
            products.Columns.Add("Received Make Model Assetno", typeof(string));
            products.Columns.Add("Received Date", typeof(string));
            products.Columns.Add("Request Item", typeof(string));
            products.Columns.Add("Request Date & Time", typeof(string));
            products.Columns.Add("Condition Of Equipment", typeof(string));
            products.Columns.Add("Hand Over Emp", typeof(string));
            products.Columns.Add("Hand Over comments", typeof(string));
            products.Columns.Add("Used On Patient", typeof(string));

            products.Columns.Add("Patient Mr No", typeof(string));
            products.Columns.Add("Usage Start Date & Time", typeof(string));
            products.Columns.Add("Usage End Date & Time", typeof(string));
            products.Columns.Add("Return Emp", typeof(string));
            products.Columns.Add("Return Date & Time", typeof(string));
            products.Columns.Add("Return Condition Of Equipment", typeof(string));

            foreach (var item in query)
            {
                products.Rows.Add(
               item.ShiftingRequestId.ToString(),
                  (from var in myapp.tbl_Location where var.LocationId == item.RequestForLocation select var.LocationName).SingleOrDefault(),
(from var in myapp.tbl_Department where var.DepartmentId == item.RequestForDepartment select var.DepartmentName).SingleOrDefault(),
                item.RequestDoctor,
                 item.RequestItem,
               item.RequestDate.HasValue ? (item.RequestDate.Value.ToString("dd/MM/yyyy")) : "",
           item.RequestComments,
           (from var in myapp.tbl_User where var.UserId == item.RequestEmpId select var.FirstName).SingleOrDefault(),
             (from var in myapp.tbl_Location where var.LocationId == item.PickupLocation select var.LocationName).SingleOrDefault(),
             item.ReceivedMakeModelAssetno, item.ReceivedDate.Value.ToString("dd/MM/yyyy") + " " + item.ReceivedTime,
             item.ConditionOfEquipment, (from var in myapp.tbl_User where var.UserId == item.HandoverEmp select var.FirstName).SingleOrDefault(),
             item.HandOvercomments, item.UsedOnPatient, item.PatientMrNo, item.UsageStartDate.Value.ToString("dd/MM/yyyy") + " " + item.UsageStartTime,
             item.UsageEndDate.Value.ToString("dd/MM/yyyy") + " " + item.UsageEndTime, (from var in myapp.tbl_User where var.UserId == item.ReturnEmpId select var.FirstName).SingleOrDefault(),
             item.ReturnDate.Value.ToString("dd/MM/yyyy") + " " + item.ReturnTime, item.ReturnConditionOfEquipment
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=ShiftingRequest.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AssetTransferHistory()
        {
            return View();
        }
        public ActionResult AjaxGetAssetTransferHistory(JQueryDataTableParamModel param)
        {
            List<AssetTransferModel> assetTransfers = new List<AssetTransferModel>();
            string con = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection db = new SqlConnection(con))
            {
                string query1 = "select assttrn.AssetTransferId, assttrn.AssetId, assttrn.FromLocationId, assttrn.FromDepartmentId, assttrn.FromSubDepartmentId, assttrn.ToLocationId, assttrn.ToDepartmentId, assttrn.ToSubDepartmentId, assttrn.Document, assttrn.ReasonForTranfer, bast.Model as AssetModel, bast.AssetNo, bast.Equipment, loc.LocationName as FromLocation, loc1.LocationName as ToLocation, dept.DepartmentName as FromDepartment, dept1.DepartmentName as ToDepartment, subdept.Name as FromSubDepartment, subdept1.Name as ToSubDepartment, u.FirstName as CreatedBy, FORMAT(assttrn.CreatedOn, 'dd/MM/yyyy hh:mm tt') AS CreatedOn from [dbo].[tbl_bm_AssetTransfer] assttrn inner join [dbo].[tbl_bm_Asset] bast on assttrn.AssetId=bast.AssetId inner join tbl_Location loc on loc.LocationId=assttrn.FromLocationId inner join tbl_Location loc1 on loc1.LocationId=assttrn.ToLocationId inner join tbl_Department dept on dept.DepartmentId=assttrn.FromDepartmentId inner join tbl_Department dept1 on dept1.DepartmentId=assttrn.ToDepartmentId inner join tbl_User u on u.CustomUserId=assttrn.CreatedBy left join tbl_subdepartment subdept on subdept.SubDepartmentId=assttrn.FromSubDepartmentId and subdept.DepartmentId=assttrn.FromDepartmentId left join tbl_subdepartment subdept1 on subdept1.SubDepartmentId=assttrn.ToSubDepartmentId and subdept1.DepartmentId=assttrn.ToDepartmentId order by assttrn.CreatedOn desc";

                assetTransfers = db.Query<AssetTransferModel>(query1).ToList();
            }

            IEnumerable<AssetTransferModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = assetTransfers
                   .Where(c => c.AssetTransferId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.AssetNo != null && c.AssetNo.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.FromLocation != null && c.FromLocation.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.ToLocation != null && c.ToLocation.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                              c.AssetModel != null && c.AssetModel.ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = assetTransfers;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies

                         select new[] {
                                              c.AssetTransferId.ToString(),
                                              c.FromLocation,
                                              c.FromDepartment+" "+c.FromSubDepartment,
                                              c.ToLocation,
                                              c.ToDepartment+" "+c.ToSubDepartment,
                                              c.AssetNo,
                                              c.AssetModel,
                                              c.CreatedBy,
                                              c.ReasonForTranfer,
                                              c.Document,
                                              c.CreatedOn};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = assetTransfers.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
    }
}
