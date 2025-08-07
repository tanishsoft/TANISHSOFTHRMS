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
    public class TravelDeskController : Controller
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        private HrDataManage hrdm = new HrDataManage();
        // GET: TravelDesk
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NewRequest()
        {
            List<tbl_User> list = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).ToList();
            if (list.Count > 0)
            {
                ViewBag.CurrentUser = list[0];
            }
            else { ViewBag.CurrentUser = new tbl_User(); }

            string rpt = hrdm.GetReportingMgr(User.Identity.Name, DateTime.Now, DateTime.Now);
            if (rpt != null && rpt != "")
            {
                tbl_User user = myapp.tbl_User.Where(u => u.CustomUserId == rpt).SingleOrDefault();
                if (user != null)
                {
                    ViewBag.HodName = user.FirstName;
                    ViewBag.HodEmailId = user.EmailId;
                    ViewBag.HodEmpId = user.EmpId;
                }
                else
                {
                    ViewBag.HodName = "";
                    ViewBag.HodEmailId = "";
                    ViewBag.HodEmpId = "";
                }
            }
            else
            {
                ViewBag.HodName = "";
                ViewBag.HodEmailId = "";
                ViewBag.HodEmpId = "";
            }
            return View();
        }
        public ActionResult MyRequests()
        {
            return View();
        }
        public ActionResult PostTravelRequest()
        {
            List<tbl_User> list = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).ToList();
            if (list.Count > 0)
            {
                ViewBag.CurrentUser = list[0];
            }
            else { ViewBag.CurrentUser = new tbl_User(); }

            string rpt = hrdm.GetReportingMgr(User.Identity.Name, DateTime.Now, DateTime.Now);
            if (rpt != null && rpt != "")
            {
                tbl_User user = myapp.tbl_User.Where(u => u.CustomUserId == rpt).SingleOrDefault();
                if (user != null)
                {
                    ViewBag.HodName = user.FirstName;
                    ViewBag.HodEmailId = user.EmailId;
                    ViewBag.HodEmpId = user.EmpId;
                }
                else
                {
                    ViewBag.HodName = "";
                    ViewBag.HodEmailId = "";
                    ViewBag.HodEmpId = "";
                }
            }
            else
            {
                ViewBag.HodName = "";
                ViewBag.HodEmailId = "";
                ViewBag.HodEmpId = "";
            }
            return View();
        }
        public ActionResult AjaxbyMyRequests(JQueryDataTableParamModel param)
        {
            int CurrentUser = int.Parse(User.Identity.Name);
            var tasks = myapp.tbl_TravelDesk.Where(l => l.EmpId == CurrentUser).ToList();

            tasks = tasks.OrderByDescending(t => t.TravelDeskId).ToList();
            IEnumerable<tbl_TravelDesk> filteredCompanies;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = tasks
                   .Where(c => c.TravelDeskId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.EmpId != null && c.EmpId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.EmpName != null && c.EmpName.Contains(param.sSearch.ToLower())
                               ||
                              c.EmployeeCategory != null && c.EmployeeCategory.Contains(param.sSearch.ToLower())
                               ||
                              c.NameOfConference != null && c.NameOfConference.Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = tasks;
            }
            IEnumerable<tbl_TravelDesk> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[]
                                           {
                                                   Convert.ToString(c.TravelDeskId),
                                                   c.EmpId.ToString(),
                                                   c.EmpName,
                                                   c.TravelCategory,
                                                   c.DateOfJourney.Value.ToString("dd/MM/yyyy"),
                                                   c.DateOfReturnJourney.Value.ToString("dd/MM/yyyy"),
                                                   c.NameOfConference,
                                                   c.Mobile,
                                                   c.Status,
                                                   c.CurrentState!=TravelDeskContansts.WorkFlow_State_Requestor?Convert.ToString(c.TravelDeskId): TravelDeskContansts.WorkFlow_State_Requestor+"-"+Convert.ToString(c.TravelDeskId)
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult CancelRequest(int id, string comments)
        {
            var request = myapp.tbl_TravelDesk.Where(l => l.TravelDeskId == id).SingleOrDefault();
            if (request != null)
            {
                request.Status = "Cancelled";
                //request.AdminComments = comments;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ApproveRequests()
        {
            ViewBag.IsTravelDeskAdmin = (User.Identity.Name == TravelDeskContansts.Travel_Admin || User.Identity.Name == TravelDeskContansts.Travel_Admin2) ? "Yes" : "No";
            return View();
        }
        public ActionResult AjaxbyApproveRequests(JQueryDataTableParamModel param)
        {
            //string CurrentUser = User.Identity.Name;
            var tasks = myapp.tbl_TravelDesk.Where(l => l.CreatedBy == User.Identity.Name).ToList();

            tasks = tasks.OrderByDescending(t => t.TravelDeskId).ToList();
            IEnumerable<tbl_TravelDesk> filteredCompanies;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = tasks
                   .Where(c => c.TravelDeskId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.EmpId != null && c.EmpId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.EmpName != null && c.EmpName.Contains(param.sSearch.ToLower())
                               ||
                              c.EmployeeCategory != null && c.EmployeeCategory.Contains(param.sSearch.ToLower())
                               ||
                              c.NameOfConference != null && c.NameOfConference.Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = tasks;
            }
            IEnumerable<tbl_TravelDesk> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[]
                                           {
                                                   Convert.ToString(c.TravelDeskId),
                                                   c.EmpId.ToString(),
                                                   c.EmpName,
                                                   c.EmployeeCategory,
                                                   c.DateOfJourney.Value.ToString("dd/MM/yyyy"),
                                                   c.DateOfReturnJourney.Value.ToString("dd/MM/yyyy"),
                                                   c.NameOfConference,
                                                   c.TravelCategory,
                                                   Convert.ToString(c.TravelDeskId)
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewAllRequests()
        {
            return View();
        }
        public ActionResult SaveTravelDesk(TravelDeskViewModel model, HttpPostedFileBase UploadDocument, HttpPostedFileBase TSMCcertificate, HttpPostedFileBase documnetAadharcard)
        {
            try
            {
                tbl_TravelDesk dbmodel = new tbl_TravelDesk();

                dbmodel.Accommodation = model.Accommodation;

                dbmodel.CreatedBy = User.Identity.Name;
                dbmodel.CreatedOn = DateTime.Now;
                dbmodel.RegistrationNeeded = model.RegistrationNeeded;
                dbmodel.EventEndDate = ProjectConvert.ConverDateStringtoDatetime(model.EventEndDate);
                dbmodel.ModeOfTravel = model.ModeOfTravel;
                dbmodel.Detailsoftheparticipation = model.Detailsoftheparticipation;
                dbmodel.EventStartDate = ProjectConvert.ConverDateStringtoDatetime(model.EventStartDate);
                dbmodel.IfRegistrationNeededDetails = model.IfRegistrationNeededDetails;
                dbmodel.ReimbursementByConference = model.ReimbursementByConference;
                dbmodel.IfReimbursementByConference = model.IfReimbursementByConference;
                dbmodel.InvitedAs = model.InvitedAs;
                dbmodel.AccommodationNote = model.AccommodationNote;
                dbmodel.VenuDetails = model.VenuDetails;
                dbmodel.SponsoredBy = model.SponsoredBy;
                dbmodel.DateOfBirth = model.DateOfBirth;
                dbmodel.Mobile = model.Mobile;
                dbmodel.DateOfJourney = ProjectConvert.ConverDateStringtoDatetime(model.DateOfJourney);
                dbmodel.DateOfReturnJourney = ProjectConvert.ConverDateStringtoDatetime(model.DateOfReturnJourney);
                dbmodel.Designation = model.Designation;
                dbmodel.EmpId = model.EmpId;
                dbmodel.EmployeeCategory = model.EmployeeCategory;
                dbmodel.EmpName = model.EmpName;
                dbmodel.IsActive = true;

                dbmodel.MailId = model.MailId;
                dbmodel.ModifiedBy = User.Identity.Name;
                dbmodel.ModifiedOn = DateTime.Now;
                dbmodel.NameOfConference = model.NameOfConference;
                dbmodel.PlaceFrom = model.PlaceFrom;
                dbmodel.PlaceTo = model.PlaceTo;
                dbmodel.ReturnPlaceFrom = model.ReturnPlaceFrom;
                dbmodel.ReturnPlaceTo = model.ReturnPlaceTo;
                dbmodel.SpecialNote = model.SpecialNote;
                dbmodel.Status = "New";
                dbmodel.TravelCategory = model.TravelCategory;
                dbmodel.TravelSubCategory = model.TravelSubCategory;
                dbmodel.CurrentState = TravelDeskContansts.WorkFlow_State_Hod;
                dbmodel.CurrentUser = model.HODEmpid;
                dbmodel.PurposeOfTrip = model.PurposeOfTrip;
                dbmodel.OnwardTime = model.OnwardTime;
                dbmodel.ReturnTime = model.ReturnTime;
                dbmodel.TSMCNO = model.TSMCNO;
                myapp.tbl_TravelDesk.Add(dbmodel);
                myapp.SaveChanges();


                if (UploadDocument != null)
                {
                    tbl_TravelDeskDocument doc = new tbl_TravelDeskDocument();
                    string fileName = Path.GetFileNameWithoutExtension(UploadDocument.FileName);
                    string extension = Path.GetExtension(UploadDocument.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    doc.Attachment = fileName;
                    UploadDocument.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));
                    doc.Name = "Invitation";
                    doc.IsActive = true;
                    doc.TravelDeskId = dbmodel.TravelDeskId;
                    myapp.tbl_TravelDeskDocument.Add(doc);
                    myapp.SaveChanges();
                }
                if (TSMCcertificate != null)
                {
                    tbl_TravelDeskDocument doc = new tbl_TravelDeskDocument();
                    string fileName = Path.GetFileNameWithoutExtension(TSMCcertificate.FileName);
                    string extension = Path.GetExtension(TSMCcertificate.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    doc.Attachment = fileName;
                    TSMCcertificate.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));
                    doc.Name = "TSMCcertificate";
                    doc.IsActive = true;
                    doc.TravelDeskId = dbmodel.TravelDeskId;
                    myapp.tbl_TravelDeskDocument.Add(doc);
                    myapp.SaveChanges();
                }
                if (documnetAadharcard != null)
                {
                    tbl_TravelDeskDocument doc = new tbl_TravelDeskDocument();
                    string fileName = Path.GetFileNameWithoutExtension(documnetAadharcard.FileName);
                    string extension = Path.GetExtension(documnetAadharcard.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    doc.Attachment = fileName;
                    documnetAadharcard.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));
                    doc.Name = "Aadhar card";
                    doc.IsActive = true;
                    doc.TravelDeskId = dbmodel.TravelDeskId;
                    myapp.tbl_TravelDeskDocument.Add(doc);
                    myapp.SaveChanges();
                }
                tbl_TravelDeskApprover HOD = new tbl_TravelDeskApprover();
                HOD.ApproverEmpId = model.HODEmpid == 1 ? 700018 : model.HODEmpid;
                HOD.TypeOfApprover = TravelDeskContansts.WorkFlow_State_Hod;
                HOD.TravelDeskId = dbmodel.TravelDeskId;
                HOD.IsActive = true;
                HOD.ApproverStatus = "Pending";
                HOD.CreatedBy = User.Identity.Name;
                HOD.CreatedOn = DateTime.Now;
                myapp.tbl_TravelDeskApprover.Add(HOD);
                myapp.SaveChanges();
                tbl_TravelDeskApprover HOD1 = new tbl_TravelDeskApprover();
                HOD1.ApproverEmpId = int.Parse(TravelDeskContansts.Travel_Admin);
                HOD1.TypeOfApprover = TravelDeskContansts.WorkFlow_State_Travel_Admin;
                HOD1.TravelDeskId = dbmodel.TravelDeskId;
                HOD1.IsActive = true;
                HOD1.ApproverStatus = "Pending";
                HOD1.CreatedBy = User.Identity.Name;
                HOD1.CreatedOn = DateTime.Now;
                myapp.tbl_TravelDeskApprover.Add(HOD1);
                myapp.SaveChanges();
                string status = SendNewTravelDeskDetailstoHOD(model, dbmodel.TravelDeskId, HOD.TravelDeskApproverId);
                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public string SendNewTravelDeskDetailstoHOD(TravelDeskViewModel model, int travelDeskId, int approvalrequestId)
        {
            try
            {
                var AdminEmail = (from var in myapp.tbl_User where var.CustomUserId == TravelDeskContansts.Travel_Admin select var.EmailId).FirstOrDefault();
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "infonet@fernandez.foundation";
                mailmodel.toemail = model.HODEMail== "evita@fernandez.foundation" ? "nitya.amarnath@fhfoundation.co.in" : model.HODEMail;

                mailmodel.subject = "A New Travel Desk Request raised by " + model.EmpName + "";
                string beforemailapproval = "";
                string mailbody = "<p style='font-family:verdana'>HI Team,";
                mailbody += "<p style='font-family:verdana'>" + model.EmpName + " has requested Travel Desk. Please click on this <a target='_blank' href='https://infonet.fernandezhospital.com/'>link</a>  to see the request.Do not forget to update the request status after completion.</p>";
                mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                mailbody += "<table style='border:1px solid #a1a2a3;width: 100%;'><tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Emp ID</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.EmpId + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Name as per Aadhar</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.EmpName + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Mail ID</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.MailId + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Date Of Birth</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.DateOfBirth + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Mobile</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Mobile + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Category</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.EmployeeCategory + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Purpose of trip</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.PurposeOfTrip + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Mode Of Travel</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ModeOfTravel + "</td></tr>";
                //mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sponsored By</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.SponsoredBy + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Event Start date </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.EventStartDate + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Event End Date</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.EventEndDate + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Venue details</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.VenuDetails + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Name Of the " + model.PurposeOfTrip + "</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.NameOfConference + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Details of the participation/representation </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Detailsoftheparticipation + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Invited as</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.InvitedAs + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Registration Needed</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RegistrationNeeded + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>If Yes: </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.IfRegistrationNeededDetails + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>TSMC No</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.TSMCNO + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Onward From </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.PlaceFrom + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Onward To </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.PlaceTo + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Onward Date </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.DateOfJourney + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Onward Time</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.OnwardTime + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Return From </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ReturnPlaceFrom + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Return To </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ReturnPlaceTo + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Return Date </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.DateOfReturnJourney + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Return Time</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ReturnTime + "</td></tr>";

                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Accommodation</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (model.Accommodation == true ? "Yes" : "No") + "</td></tr>";

                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Accommodation Note </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.AccommodationNote + "</td></tr>";
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Reimbursement By Conference/ Workshop Organisers</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ReimbursementByConference + "</td></tr>";

                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>If Reimbursement</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.IfReimbursementByConference + "</td></tr>";

                List<tbl_TravelDeskApprover> approver = myapp.tbl_TravelDeskApprover.Where(l => l.TravelDeskId == travelDeskId).ToList();

                var model2 = (from m in approver
                              join u in myapp.tbl_User on m.ApproverEmpId equals u.EmpId
                              select new
                              {
                                  m.TravelDeskApproverId,
                                  m.TypeOfApprover,
                                  u.FirstName,
                                  m.ApproverStatus,
                                  m.ApproverComments
                              }).ToList();
                model2 = model2.OrderBy(l => l.TravelDeskApproverId).ToList();
                foreach (var user in model2)
                {
                    mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Approver " + user.TypeOfApprover + "</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + user.FirstName + "</td></tr>";
                }

                beforemailapproval = mailbody;

                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana' colspan='2'> <a  Target='_blank' href='https://infonet.fernandezhospital.com/TravelDesk/ApproverStatusRequest?id=" + approvalrequestId + "&comments=ok&status=Approved&createdby=" + model.HODEmpid + "' style='color:green;font-weight:600;' >Approve</a>&nbsp;&nbsp;&nbsp;<a Target='_blank' href='https://infonet.fernandezhospital.com/TravelDesk/ApproverStatusRequest?id=" + approvalrequestId + "&comments=ok&status=Rejected&createdby=" + model.HODEmpid + "'  style='color:red;font-weight:600;' >Reject</a></td></tr>";

                mailbody += "</table>";
                beforemailapproval += "</table>";

                string hodbody = "", userbody = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/TravelDeskTemplate.html")))
                {
                    hodbody = reader.ReadToEnd();
                }
                hodbody = hodbody.Replace("[[ApproverName]]", model.HODName);
                hodbody = hodbody.Replace("[[TravelUser]]", model.EmpName);
                hodbody = hodbody.Replace("[[TravelLocation]]", model.PlaceFrom);
                hodbody = hodbody.Replace("[[DateOfTravel]]", model.DateOfJourney);
                hodbody = hodbody.Replace("[[ReasonForTravel]]", model.PurposeOfTrip);
                hodbody = hodbody.Replace("[[OnWardJourneyType]]", model.ModeOfTravel);
                hodbody = hodbody.Replace("[[OnWardDate]]", model.DateOfJourney);
                hodbody = hodbody.Replace("[[OnWardTime]]", model.OnwardTime);
                hodbody = hodbody.Replace("[[OnWardFromLocation]]", model.PlaceFrom);
                hodbody = hodbody.Replace("[[OnWardToLocation]]", model.PlaceTo);

                hodbody = hodbody.Replace("[[ReturnDate]]", model.DateOfReturnJourney);
                hodbody = hodbody.Replace("[[ReturnTime]]", model.ReturnTime);
                hodbody = hodbody.Replace("[[ReturnFromLocation]]", model.ReturnPlaceFrom);
                hodbody = hodbody.Replace("[[ReturnToLocation]]", model.ReturnPlaceTo);
                hodbody = hodbody.Replace("[[Accommodation]]", (model.Accommodation == true ? "Yes" : "No"));

                hodbody = hodbody.Replace("[[EmpId]]", model.EmpId.ToString());
                hodbody = hodbody.Replace("[[EmpName]]", model.EmpName);
                hodbody = hodbody.Replace("[[MailId]]", model.MailId);
                hodbody = hodbody.Replace("[[DateOfBirth]]", model.DateOfBirth);
                hodbody = hodbody.Replace("[[Mobile]]", model.Mobile);
                hodbody = hodbody.Replace("[[EmployeeCategory]]", model.EmployeeCategory);
                hodbody = hodbody.Replace("[[PurposeOfTrip]]", model.PurposeOfTrip);
                hodbody = hodbody.Replace("[[ModeOfTravel]]", model.ModeOfTravel);
                hodbody = hodbody.Replace("[[EventStartDate]]", model.EventStartDate);
                hodbody = hodbody.Replace("[[EventEndDate]]", model.EventEndDate);
                hodbody = hodbody.Replace("[[VenuDetails]]", model.VenuDetails);
                hodbody = hodbody.Replace("[[NameOfConference]]", model.NameOfConference);
                hodbody = hodbody.Replace("[[Detailsoftheparticipation]]", model.Detailsoftheparticipation);
                hodbody = hodbody.Replace("[[InvitedAs]]", model.InvitedAs);
                hodbody = hodbody.Replace("[[RegistrationNeeded]]", model.RegistrationNeeded);
                hodbody = hodbody.Replace("[[IfRegistrationNeededDetails]]", model.IfRegistrationNeededDetails);
                hodbody = hodbody.Replace("[[TSMCNO]]", model.TSMCNO);
                hodbody = hodbody.Replace("[[PlaceFrom]]", model.PlaceFrom);
                hodbody = hodbody.Replace("[[PlaceTo]]", model.PlaceTo);
                hodbody = hodbody.Replace("[[DateOfJourney]]", model.DateOfJourney);
                hodbody = hodbody.Replace("[[OnwardTime]]", model.OnwardTime);
                hodbody = hodbody.Replace("[[ReturnPlaceFrom]]", model.ReturnPlaceFrom);
                hodbody = hodbody.Replace("[[ReturnPlaceTo]]", model.ReturnPlaceTo);
                hodbody = hodbody.Replace("[[DateOfReturnJourney]]", model.DateOfReturnJourney);
                hodbody = hodbody.Replace("[[ReturnTime]]", model.ReturnTime);
                hodbody = hodbody.Replace("[[Accommodation]]", (model.Accommodation == true ? "Yes" : "No"));
                hodbody = hodbody.Replace("[[AccommodationNote]]", model.AccommodationNote);
                hodbody = hodbody.Replace("[[ReimbursementByConference]]", model.ReimbursementByConference);
                hodbody = hodbody.Replace("[[IfReimbursementByConference]]", model.IfReimbursementByConference);
                hodbody = hodbody.Replace("[[TravelUser]]", model.EmpName);
                hodbody = hodbody.Replace("[[ReasonForTravel]]", model.PurposeOfTrip);
                hodbody = hodbody.Replace("[[OnWardDate]]", model.DateOfJourney);
                hodbody = hodbody.Replace("[[OnWardTime]]", model.OnwardTime);
                hodbody = hodbody.Replace("[[OnWardFromLocation]]", model.PlaceFrom);
                hodbody = hodbody.Replace("[[OnWardToLocation]]", model.PlaceTo);
                hodbody = hodbody.Replace("[[ReturnDate]]", model.DateOfReturnJourney);
                hodbody = hodbody.Replace("[[ReturnTime]]", model.ReturnTime);
                hodbody = hodbody.Replace("[[ReturnFromLocation]]", model.ReturnPlaceFrom);
                hodbody = hodbody.Replace("[[ReturnToLocation]]", model.ReturnPlaceTo);


                string approveurl = "https://infonet.fernandezhospital.com/TravelDesk/ApproverStatusRequest?id=" + approvalrequestId + "&comments=ok&status=Approved&createdby=" + model.HODEmpid;
                string rejecturl = "https://infonet.fernandezhospital.com/TravelDesk/ApproverStatusRequest?id=" + approvalrequestId + "&comments=ok&status=Rejected&createdby=" + model.HODEmpid;

                hodbody = hodbody.Replace("[[ApproveUrl]]", approveurl);
                hodbody = hodbody.Replace("[[RejectUrl]]", rejecturl);

                mailmodel.body = hodbody;
                mailmodel.filepath = "";
                mailmodel.fromname = "Travel Desk Request";

                string result = cm.SendEmail(mailmodel);

                // User boyd
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/BioMedical_Asset.html")))
                {
                    userbody = reader.ReadToEnd();
                }
                userbody = userbody.Replace("[[Heading]]", "Travel Desk Request");
                userbody = userbody.Replace("[[table]]", beforemailapproval);

                mailmodel.body = userbody;
                mailmodel.toemail = model.MailId;
                mailmodel.ccemail = "traveldesk@fernandez.foundation";
                if (result == "Sent Success")
                {
                    result = cm.SendEmail(mailmodel);
                }
                return result;
            }
            catch (Exception exc)
            {
                return exc.Message;
            }


        }
        public ActionResult ManageApprovers()
        {
            return View();
        }
        public ActionResult SaveApproverMaster(tbl_TravelDeskApprover model)
        {
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            myapp.tbl_TravelDeskApprover.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteApproverMaster(int id)
        {
            var model = myapp.tbl_TravelDeskApprover.Where(l => l.TravelDeskApproverId == id).SingleOrDefault();
            myapp.tbl_TravelDeskApprover.Remove(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxbyTaskApproverMaster(JQueryDataTableParamModel param)
        {
            var listofapprovers = myapp.tbl_TravelDeskApprover.ToList();
            List<tbl_TravelDeskApprover> tasks = (from m in listofapprovers

                                                      //join d in myapp.tbl_Department on m.Departmentid equals d.DepartmentId
                                                      //join lc in myapp.tbl_Location on m.CreatorLocationId equals lc.LocationId
                                                      //join dc in myapp.tbl_Department on m.CreatorDepartmentId equals dc.DepartmentId
                                                  join u in myapp.tbl_User on m.ApproverEmpId equals u.EmpId
                                                  select new tbl_TravelDeskApprover
                                                  {
                                                      CreatedBy = m.CreatedBy,
                                                      CreatedOn = m.CreatedOn,
                                                      IsActive = m.IsActive
                                                  }).ToList();
            tasks = tasks.OrderByDescending(t => t.TravelDeskApproverId).ToList();
            IEnumerable<tbl_TravelDeskApprover> filteredCompanies;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = tasks
                   .Where(c => c.TravelDeskApproverId.ToString().ToLower().Contains(param.sSearch.ToLower())
                              // ||
                              //  c.UserName != null && c.UserName.ToString().ToLower().Contains(param.sSearch.ToLower())
                              // ||
                              //c.Amount != null && c.Amount.ToString().Contains(param.sSearch.ToLower())
                              // ||
                              //c.LocationName != null && c.LocationName.ToLower().Contains(param.sSearch.ToLower())
                              // ||
                              //c.Operator != null && c.Operator.ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = tasks;
            }
            IEnumerable<tbl_TravelDeskApprover> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[]
                                           {
                                               Convert.ToString(c.TravelDeskApproverId),
                                               //    c.LocationName,
                                               //    c.UserName,
                                               //    c.Operator,
                                               //    c.Amount.ToString(),
                                               //    c.DepartmentId.ToString(),
                                               //    c.ApproverLevel.ToString(),
                                               //    Convert.ToString(c.ApproveListId)
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult getEmployeeTravelRequests(int empid)
        {
            var models = myapp.tbl_TravelDesk.Where(m => m.EmpId == empid).OrderByDescending(l => l.TravelDeskId).Take(5).ToList();
            List<TravelDeskViewModel> lists = new List<TravelDeskViewModel>();
            foreach (var model in models)
            {
                lists.Add(getTravelDeskViewModel(model));
            }
            return Json(lists, JsonRequestBehavior.AllowGet);
        }
        public TravelDeskViewModel getTravelDeskViewModel(tbl_TravelDesk model)
        {
            TravelDeskViewModel dbmodel = new TravelDeskViewModel();
            dbmodel.TravelDeskId = model.TravelDeskId;
            dbmodel.Accommodation = true;
            dbmodel.DateOfJourney = model.DateOfJourney.HasValue ? model.DateOfJourney.Value.ToString("dd/MM/yyyy") : "";
            dbmodel.DateOfReturnJourney = model.DateOfReturnJourney.HasValue ? model.DateOfReturnJourney.Value.ToString("dd/MM/yyyy") : "";
            dbmodel.Designation = model.Designation;
            dbmodel.EmpId = model.EmpId.Value;
            dbmodel.EmployeeCategory = model.EmployeeCategory;
            dbmodel.EmpName = model.EmpName;
            dbmodel.MailId = model.MailId;
            dbmodel.NameOfConference = model.NameOfConference;
            dbmodel.PlaceFrom = model.PlaceFrom;
            dbmodel.PlaceTo = model.PlaceTo;
            dbmodel.ReturnPlaceFrom = model.ReturnPlaceFrom;
            dbmodel.ReturnPlaceTo = model.ReturnPlaceTo;
            dbmodel.SpecialNote = model.SpecialNote;
            dbmodel.Status = model.Status;
            dbmodel.TravelCategory = model.TravelCategory;
            dbmodel.TravelSubCategory = model.TravelSubCategory;
            dbmodel.CurrentUser = model.CurrentUser;
            dbmodel.CurrentState = model.CurrentState;
            dbmodel.DateOfBirth = model.DateOfBirth;
            dbmodel.Mobile = model.Mobile;
            dbmodel.Detailsoftheparticipation = model.Detailsoftheparticipation;
            dbmodel.ModeOfTravel = model.ModeOfTravel;
            dbmodel.EventStartDate = model.EventStartDate.HasValue ? model.EventStartDate.Value.ToString("dd/MM/yyyy") : "";
            dbmodel.EventEndDate = model.EventEndDate.HasValue ? model.EventEndDate.Value.ToString("dd/MM/yyyy") : "";

            dbmodel.RegistrationNeeded = model.RegistrationNeeded;
            dbmodel.IfRegistrationNeededDetails = model.IfRegistrationNeededDetails;
            dbmodel.ReimbursementByConference = model.ReimbursementByConference;
            dbmodel.IfReimbursementByConference = model.IfReimbursementByConference;
            dbmodel.SponsoredBy = model.SponsoredBy;
            dbmodel.InvitedAs = model.InvitedAs;
            dbmodel.AccommodationNote = model.AccommodationNote;
            dbmodel.VenuDetails = model.VenuDetails;
            dbmodel.TSMCNO = model.TSMCNO;
            dbmodel.PurposeOfTrip = model.PurposeOfTrip;
            dbmodel.OnwardTime = model.OnwardTime;
            dbmodel.ReturnTime = model.ReturnTime;

            dbmodel.Documents = myapp.tbl_TravelDeskDocument.Where(m => m.TravelDeskId == model.TravelDeskId).Select(d => new TravelDeskDocumentViewModel()
            {
                DocumentId = d.TravelDeskDocumentId,
                DocumentName = d.Name,
                DocumentUrl = d.Attachment
            }).ToList();
            dbmodel.ApprovalList = new List<travelApprovalViewModel>();
            dbmodel.ApprovalList = (from a in myapp.tbl_TravelDeskApprover
                                    join u in myapp.tbl_User on a.ApproverEmpId equals u.EmpId
                                    where a.TravelDeskId == model.TravelDeskId
                                    select new travelApprovalViewModel()
                                    {
                                        TravelDeskId = model.TravelDeskId,
                                        ApproverComments = a.ApproverComments,
                                        ApproverEmpId = a.ApproverEmpId.HasValue ? a.ApproverEmpId.Value : 0,
                                        ApproverName = u.FirstName,
                                        ApproverStatus = a.ApproverStatus,
                                        TypeOfApprover = a.TypeOfApprover,
                                        TravelDeskApproverId = a.TravelDeskApproverId
                                    }).ToList();
            return dbmodel;
        }
        public ActionResult GetTravelDeskDetails(int? id, string Approver = "no")
        {

            if (Approver == "yes")
            {
                id = myapp.tbl_TravelDeskApprover.Where(l => l.TravelDeskApproverId == id).Select(m => m.TravelDeskId).FirstOrDefault();
            }
            var model = myapp.tbl_TravelDesk.Where(m => m.TravelDeskId == id).SingleOrDefault();

            var dbmodel = getTravelDeskViewModel(model);
            return Json(dbmodel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxbyTravelDeskDetailsToApprove(JQueryDataTableParamModel param)
        {
            List<tbl_TravelDesk> filtercpx = myapp.tbl_TravelDesk.ToList();
            int CurrentUser = int.Parse(User.Identity.Name);
            int Travel_Admin = int.Parse(TravelDeskContansts.Travel_Admin);
            bool isTraveldeskAdmin = User.Identity.Name == TravelDeskContansts.Travel_Admin || User.Identity.Name == TravelDeskContansts.Travel_Admin2;

            // Now include records where the current user is an approver OR if admin
            var listofApprove = myapp.tbl_TravelDeskApprover
                .Where(l => l.ApproverEmpId == CurrentUser || (isTraveldeskAdmin && l.ApproverEmpId== Travel_Admin))
                .ToList();

            List<TravelDeskViewModel> tasks = (from m in filtercpx
                                               join app in listofApprove on m.TravelDeskId equals app.TravelDeskId
                                               where app.ApproverEmpId == CurrentUser || (isTraveldeskAdmin && app.ApproverEmpId == Travel_Admin)
                                               select new TravelDeskViewModel
                                               {
                                                   TravelDeskId = m.TravelDeskId,
                                                   EmpId = m.EmpId.Value,
                                                   EmpName = m.EmpName,
                                                   Designation = m.Designation,
                                                   TravelCategory = m.TravelCategory,
                                                   TravelSubCategory = m.TravelSubCategory,
                                                   NameOfConference = m.NameOfConference,
                                                   EmployeeCategory = m.EmployeeCategory,
                                                   DateOfJourney = m.DateOfJourney.Value.ToString("dd/MM/yyyy"),
                                                   DateOfReturnJourney = m.DateOfReturnJourney.Value.ToString("dd/MM/yyyy"),
                                                   PlaceFrom = m.PlaceFrom,
                                                   PlaceTo = m.PlaceTo,
                                                   Status = m.Status,
                                                   ReturnPlaceFrom = m.ReturnPlaceFrom,
                                                   ReturnPlaceTo = m.ReturnPlaceTo,
                                                   approverId = app.TravelDeskApproverId,
                                                   Approverstatus = app.ApproverStatus,
                                                   CurrentState = m.CurrentState,
                                                   ApproverLevel = app.TypeOfApprover
                                               }).Distinct().ToList();
            if (param.status != null && param.status != "" && param.status.ToLower() != "all")
            {
                tasks = tasks.Where(l => l.Status == param.status).ToList();
            }
            tasks = tasks.OrderByDescending(t => t.TravelDeskId).ToList();
            IEnumerable<TravelDeskViewModel> filtered;
            ////Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filtered = tasks
                   .Where(c => c.TravelDeskId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.TravelCategory != null && c.TravelCategory.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.DateOfJourney.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.EmpName != null && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.EmpId != null && c.EmpId.ToString().Contains(param.sSearch.ToLower())
                              ||
                              c.EmployeeCategory != null && c.EmployeeCategory.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.Status != null && c.Status.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.Approverstatus != null && c.Approverstatus.ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filtered = tasks;
            }
            IEnumerable<TravelDeskViewModel> displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayed
                                           select new[]
                                           {
                                               Convert.ToString(c.TravelDeskId),
                                               c.EmpId.ToString(),
                                               c.EmpName,
                                               c.NameOfConference,
                                               c.EmployeeCategory,
                                               c.TravelCategory,
                                               //c.TravelSubCategory,
                                               c.DateOfJourney,
                                               c.DateOfReturnJourney,
                                               //c.PlaceFrom,
                                               //c.PlaceTo,
                                               //c.ReturnPlaceFrom,
                                               //c.ReturnPlaceTo,
                                               c.Status,
                                               c.CurrentState,
                                               (c.Approverstatus=="Pending" && c.CurrentState==c.ApproverLevel)? "Pending-"+Convert.ToString(c.approverId): Convert.ToString(c.TravelDeskId)
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filtered.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TravelDeskDetails_ExportToExcel(string FromDate, string ToDate)
        {
            var listofTD = (from m in myapp.tbl_TravelDesk select m).OrderByDescending(l => l.TravelDeskId).ToList();
            var listofApprove = (from a in myapp.tbl_TravelDeskApprover select a).ToList();
            int CurrentUser = int.Parse(User.Identity.Name);

            if (FromDate != null && FromDate != "" && ToDate != null && ToDate != "")
            {
                DateTime dtfrmdate = ProjectConvert.ConverDateStringtoDatetime(FromDate);
                DateTime dttodate = ProjectConvert.ConverDateStringtoDatetime(ToDate);
                if (dtfrmdate == dttodate)
                {
                    dttodate = dttodate.AddDays(1);
                }
                listofTD = (from t in listofTD
                            where t.CreatedOn.Value >= dtfrmdate && t.CreatedOn.Value <= dttodate
                            select t).ToList();
            }

            List<TravelDeskViewModel> tasks = (from m in listofTD
                                               select new TravelDeskViewModel
                                               {
                                                   TravelDeskId = m.TravelDeskId,
                                                   EmpId = m.EmpId.Value,
                                                   EmpName = m.EmpName,
                                                   Designation = m.Designation,
                                                   TravelCategory = m.TravelCategory,
                                                   TravelSubCategory = m.TravelSubCategory,
                                                   NameOfConference = m.NameOfConference,
                                                   EmployeeCategory = m.EmployeeCategory,
                                                   DateOfJourney = m.DateOfJourney.Value.ToString("dd/MM/yyyy"),
                                                   DateOfReturnJourney = m.DateOfReturnJourney.Value.ToString("dd/MM/yyyy"),
                                                   PlaceFrom = m.PlaceFrom,
                                                   PlaceTo = m.PlaceTo,
                                                   Status = m.Status,
                                                   ReturnPlaceFrom = m.ReturnPlaceFrom,
                                                   ReturnPlaceTo = m.ReturnPlaceTo,
                                                   Accommodation = m.Accommodation.HasValue ? m.Accommodation.Value : false,
                                                   AccommodationNote = m.AccommodationNote,
                                                   DateOfBirth = m.DateOfBirth,
                                                   EventStartDate = m.EventStartDate.HasValue ? m.EventStartDate.Value.ToString("dd/MM/yyyy") : "",
                                                   EventEndDate = m.EventEndDate.HasValue ? m.EventEndDate.Value.ToString("dd/MM/yyyy") : "",
                                                   IfRegistrationNeededDetails = m.IfRegistrationNeededDetails,
                                                   InvitedAs = m.InvitedAs,
                                                   ModeOfTravel = m.ModeOfTravel,
                                                   Mobile = m.Mobile,
                                                   MailId = m.MailId,
                                                   IfReimbursementByConference = m.IfReimbursementByConference,
                                                   PurposeOfTrip = m.PurposeOfTrip,
                                                   RegistrationNeeded = m.RegistrationNeeded,
                                                   ReimbursementByConference = m.ReimbursementByConference,
                                                   Detailsoftheparticipation = m.Detailsoftheparticipation,
                                                   OnwardTime = m.OnwardTime,
                                                   ReturnTime = m.ReturnTime,
                                                   SpecialNote = m.SpecialNote,
                                                   SponsoredBy = m.SponsoredBy,
                                                   TSMCNO = m.TSMCNO,
                                                   VenuDetails = m.VenuDetails,
                                                   RequestDate = m.CreatedOn.HasValue ? m.CreatedOn.Value.ToString("dd/MM/yyyy") : ""
                                               }).ToList();
            System.Data.DataTable products = new System.Data.DataTable("MyTasksDataTable");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("EmpId", typeof(string));
            products.Columns.Add("EmpName", typeof(string));
            products.Columns.Add("DateOfBirth", typeof(string));
            products.Columns.Add("Designation", typeof(string));
            products.Columns.Add("EmployeeCategory", typeof(string));
            products.Columns.Add("RequestDate", typeof(string));
            products.Columns.Add("Booking date", typeof(string));
            products.Columns.Add("PurposeOfTrip", typeof(string));
            products.Columns.Add("NameOfConference", typeof(string));
            products.Columns.Add("Venue details", typeof(string));
            products.Columns.Add("InvitedAs", typeof(string));
            products.Columns.Add("Detailsoftheparticipation", typeof(string));
            products.Columns.Add("EventStartDate", typeof(string));
            products.Columns.Add("EventEndDate", typeof(string));

            products.Columns.Add("DateOfJourney", typeof(string));
            products.Columns.Add("OnwardTime", typeof(string));
            products.Columns.Add("PlaceFrom", typeof(string));
            products.Columns.Add("PlaceTo", typeof(string));
            products.Columns.Add("DateOfReturnJourney", typeof(string));
            products.Columns.Add("ReturnTime", typeof(string));
            products.Columns.Add("ReturnPlaceFrom", typeof(string));
            products.Columns.Add("ReturnPlaceTo", typeof(string));
            products.Columns.Add("RegistrationNeeded", typeof(string));
            products.Columns.Add("IfRegistrationNeededDetails", typeof(string));
            products.Columns.Add("TSMCNO", typeof(string));
            products.Columns.Add("Accommodation", typeof(string));
            products.Columns.Add("AccommodationNote", typeof(string));
            products.Columns.Add("Mobile", typeof(string));
            products.Columns.Add("MailId", typeof(string));
            products.Columns.Add("ModeOfTravel", typeof(string));
            products.Columns.Add("SponsoredBy", typeof(string));
            products.Columns.Add("ReimbursementByConference", typeof(string));
            products.Columns.Add("IfReimbursementByConference", typeof(string));
            products.Columns.Add("Status", typeof(string));

            products.Columns.Add("Travel Cost", typeof(string));
            products.Columns.Add("Accommodation Cost", typeof(string));
            products.Columns.Add("Registration Cost", typeof(string));
            products.Columns.Add("Miscellaneous Cost", typeof(string));
            products.Columns.Add("Total Amount", typeof(string));

            products.Columns.Add("TravelCategory", typeof(string));
            products.Columns.Add("TravelSubCategory", typeof(string));
            products.Columns.Add("SpecialNote", typeof(string));


            var list = myapp.tbl_TravelDeskTravel.ToList();

            foreach (var c in tasks)
            {
                decimal TravelCost = 0; decimal AccommodationCost = 0; decimal RegistrationCost = 0; decimal MiscellaneousCost = 0;
                var traveldetails = list.Where(l => l.TravelDeskId == c.TravelDeskId).ToList();
                TravelCost = traveldetails.Where(l => l.CostOfTravel != null && l.TypeOfBooking == "Travel").Sum(l => l.CostOfTravel.Value);
                AccommodationCost = traveldetails.Where(l => l.CostOfTravel != null && l.TypeOfBooking == "Accomedation").Sum(l => l.CostOfTravel.Value);
                RegistrationCost = traveldetails.Where(l => l.CostOfTravel != null && l.TypeOfBooking == "Registration Cost").Sum(l => l.CostOfTravel.Value);
                MiscellaneousCost = traveldetails.Where(l => l.CostOfTravel != null && l.TypeOfBooking == "Miscellaneous Cost").Sum(l => l.CostOfTravel.Value);
                products.Rows.Add(c.TravelDeskId,
                    c.EmpId,
                    c.EmpName,
                      c.DateOfBirth,
                     c.Designation,
                        c.EmployeeCategory,
                        c.RequestDate,
                        c.DateOfJourney,
                        c.PurposeOfTrip,
                          c.NameOfConference,
                          c.VenuDetails,
                            c.InvitedAs,
                            c.Detailsoftheparticipation,

                               c.EventStartDate,
                  c.EventEndDate,
                  c.DateOfJourney,
                   c.OnwardTime,
                        c.PlaceFrom,
                    c.PlaceTo,
                     c.DateOfReturnJourney,
                      c.ReturnTime,
                        c.ReturnPlaceFrom,
                   c.ReturnPlaceTo,
                      c.RegistrationNeeded,
                        c.IfRegistrationNeededDetails,
                         c.TSMCNO,
                             c.Accommodation == true ? "Yes" : "No",
                 c.AccommodationNote,
                  c.Mobile,
                  c.MailId,

                  c.ModeOfTravel,
                  c.SponsoredBy,
                  c.ReimbursementByConference,
                  c.IfReimbursementByConference,
                  c.Status,

                TravelCost, AccommodationCost, RegistrationCost, MiscellaneousCost, (TravelCost + AccommodationCost + RegistrationCost + MiscellaneousCost),
                c.TravelCategory,
                    c.TravelSubCategory,
                c.SpecialNote
                );
                //}

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
            string filename = "TravelDeskDetails_Data.xls";
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
        [Authorize]
        public ActionResult AjaxbyTravelDeskDetailsToaddtraveldek(JQueryDataTableParamModel param)
        {
            var listofTD = (from m in myapp.tbl_TravelDesk select m).ToList();
            var listofApprove = (from a in myapp.tbl_TravelDeskApprover select a).ToList();
            int CurrentUser = int.Parse(User.Identity.Name);

            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                DateTime dtfrmdate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                DateTime dttodate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                if (dtfrmdate == dttodate)
                {
                    dttodate = dttodate.AddDays(1);
                }
                listofTD = (from t in listofTD
                            where t.CreatedOn.Value >= dtfrmdate && t.CreatedOn.Value <= dttodate
                            select t).ToList();
            }

            List<TravelDeskViewModel> tasks = (from m in listofTD
                                               select new TravelDeskViewModel
                                               {
                                                   TravelDeskId = m.TravelDeskId,
                                                   EmpId = m.EmpId.Value,
                                                   EmpName = m.EmpName,
                                                   Designation = m.Designation,
                                                   TravelCategory = m.TravelCategory,
                                                   TravelSubCategory = m.TravelSubCategory,
                                                   NameOfConference = m.NameOfConference,
                                                   EmployeeCategory = m.EmployeeCategory,
                                                   DateOfJourney = m.DateOfJourney.Value.ToString("dd/MM/yyyy"),
                                                   DateOfReturnJourney = m.DateOfReturnJourney.Value.ToString("dd/MM/yyyy"),
                                                   PlaceFrom = m.PlaceFrom,
                                                   PlaceTo = m.PlaceTo,
                                                   Status = m.Status,
                                                   ReturnPlaceFrom = m.ReturnPlaceFrom,
                                                   ReturnPlaceTo = m.ReturnPlaceTo,
                                               }).ToList();

            if (param.status != null && param.status != "" && param.status.ToLower() != "all")
            {
                tasks = tasks.Where(l => l.Status == param.status).ToList();
            }
            tasks = tasks.OrderByDescending(t => t.TravelDeskId).ToList();
            IEnumerable<TravelDeskViewModel> filtered;
            ////Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filtered = tasks
                   .Where(c => c.TravelDeskId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.TravelCategory != null && c.TravelCategory.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.DateOfJourney.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.EmpName != null && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.EmpId != null && c.EmpId.ToString().Contains(param.sSearch.ToLower())
                              ||
                              c.EmployeeCategory != null && c.EmployeeCategory.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.Status != null && c.Status.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.Approverstatus != null && c.Approverstatus.ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filtered = tasks;
            }
            IEnumerable<TravelDeskViewModel> displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayed
                                           select new[]
                                           {
                                               Convert.ToString(c.TravelDeskId),
                                               c.EmpId.ToString(),
                                               c.EmpName,
                                               c.NameOfConference,
                                               c.EmployeeCategory,
                                               c.TravelCategory+" "+(!string.IsNullOrEmpty(c.TravelSubCategory)?c.TravelSubCategory:""),

                                               c.DateOfJourney,
                                               c.DateOfReturnJourney,
                                               c.PlaceFrom,
                                               c.PlaceTo,
                                               c.ReturnPlaceFrom,
                                               c.ReturnPlaceTo,
                                               c.Status,
                                               c.TravelDeskId.ToString()

                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filtered.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApproversList(int id)
        {
            List<tbl_TravelDeskApprover> approver = myapp.tbl_TravelDeskApprover.Where(l => l.TravelDeskId == id).ToList();

            var model = (from m in approver
                         join u in myapp.tbl_User on m.ApproverEmpId equals u.EmpId
                         select new
                         {
                             m.TravelDeskApproverId,
                             m.TypeOfApprover,
                             u.FirstName,
                             m.ApproverStatus,
                             m.ApproverComments
                         }).ToList();
            model = model.OrderBy(l => l.TravelDeskApproverId).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult ApproverStatusRequest(int id, string comments, string status, string createdby = "", string Sponsoredby = "")
        {
            var approve = myapp.tbl_TravelDeskApprover.Where(l => l.TravelDeskApproverId == id).ToList();
            if (approve.Count != 0)
            {
                approve[0].ApproverStatus = status;
                approve[0].ApproverComments = comments;
                approve[0].CreatedBy = createdby != "" ? createdby : User.Identity.Name;
                approve[0].CreatedOn = DateTime.Now;
                myapp.SaveChanges();
                int tdid = approve[0].TravelDeskId.Value;
                string value1 = TravelDeskContansts.WorkFlow_State_Hod;
                switch (approve[0].TypeOfApprover)
                {
                    case "WithHod":
                        var td = myapp.tbl_TravelDesk.Where(m => m.TravelDeskId == tdid).ToList();
                        if (td.Count != 0)
                        {
                            if (status == "Approved")
                            {
                                td[0].Status = "In Progress";
                                td[0].CurrentState = TravelDeskContansts.WorkFlow_State_Travel_Admin;
                                myapp.SaveChanges();
                            }
                            else
                            {
                                td[0].Status = status;
                                myapp.SaveChanges();
                            }
                            sendTravelDeskapprovalEmail(tdid, approve[0], "WithHod");
                        }
                        break;
                    case "TravelAdmin":
                        var td1 = myapp.tbl_TravelDesk.Where(m => m.TravelDeskId == tdid).ToList();
                        if (td1.Count != 0)
                        {
                            if (status == "Approved")
                            {
                                td1[0].Status = "In Progress";
                            }
                            else
                            {
                                td1[0].Status = status;
                            }
                            td1[0].CurrentState = Sponsoredby;
                            td1[0].CurrentState = TravelDeskContansts.WorkFlow_State_Travel_User;
                            myapp.SaveChanges();
                            sendTravelDeskapprovalEmail(tdid, approve[0], "TravelAdmin");
                        }
                        break;
                    case "TravelUser":
                        var td2 = myapp.tbl_TravelDesk.Where(m => m.TravelDeskId == tdid).ToList();
                        if (td2.Count != 0)
                        {
                            if (status == "Approved")
                            {
                                td2[0].Status = "Completed";
                            }
                            else
                            {
                                td2[0].Status = status;
                            }
                            td2[0].CurrentState = TravelDeskContansts.WorkFlow_State_Requestor;
                            myapp.SaveChanges();
                        }
                        break;
                    default:
                        var td3 = myapp.tbl_TravelDesk.Where(m => m.TravelDeskId == tdid).ToList();
                        if (td3.Count != 0)
                        {
                            td3[0].Status = status;
                            myapp.SaveChanges();
                        }
                        break;
                }

            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public string sendTravelDeskapprovalEmail(int id, tbl_TravelDeskApprover approve, string type)
        {
            var toemail = string.Empty;
            var model = myapp.tbl_TravelDesk.Where(m => m.TravelDeskId == id).FirstOrDefault();
            var empId = Convert.ToString(model.EmpId);
            var empemail = (from var in myapp.tbl_User where var.CustomUserId == empId select var.EmailId).FirstOrDefault();
            //if withHod =>send email to travel Admin
            //if travelAdmin=>send email to travel User
            toemail = type == "WithHod" ? (from var in myapp.tbl_User where var.CustomUserId == TravelDeskContansts.Travel_Admin select var.EmailId).FirstOrDefault() : (from var in myapp.tbl_User where var.CustomUserId == TravelDeskContansts.Travel_User select var.EmailId).FirstOrDefault();
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel();
            EmailTeamplates emailtemp = new EmailTeamplates();
            mailmodel.fromemail = "Leave@hospitals.com";
            mailmodel.toemail = toemail;
            mailmodel.ccemail = empemail;
            mailmodel.ccemail += ",traveldesk@fernandez.foundation";
            mailmodel.subject = type == "WithHod" ? "Travel Desk Request of " + model.EmpName + " has been " + approve.ApproverStatus + " by HOD" : "Travel Desk Request of " + model.EmpName + " has been " + approve.ApproverStatus + " by Travel Admin";
            string mailbody = "<p style='font-family:verdana'>HI Team,";
            mailbody += "<p style='font-family:verdana'>" + model.EmpName + " has requested Travel Desk. Please click on this <a target='_blank' href='https://infonet.fernandezhospital.com/'>link</a>  to see the request.Do not forget to update the request status after completion.</p>";
            mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
            mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Emp ID</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.EmpId + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Name as per Aadhar</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.EmpName + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Mail ID</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.MailId + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Date Of Birth</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.DateOfBirth + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Mobile</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Mobile + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Category</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.EmployeeCategory + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Purpose of trip</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.PurposeOfTrip + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Mode Of Travel</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ModeOfTravel + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Sponsored By</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.SponsoredBy + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Event Start date </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (model.EventStartDate.HasValue ? model.EventStartDate.Value.ToString("dd/MM/yyyy") : "") + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Event End date</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (model.EventEndDate.HasValue ? model.EventEndDate.Value.ToString("dd/MM/yyyy") : "") + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Venue details</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.VenuDetails + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Name Of the " + model.PurposeOfTrip + "</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.NameOfConference + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Details of the participation/representation </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Detailsoftheparticipation + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Invited as</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.InvitedAs + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Registration Needed</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.RegistrationNeeded + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>If Yes: </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.IfRegistrationNeededDetails + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>TSMC No</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.TSMCNO + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Onward From </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.PlaceFrom + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Onward To </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.PlaceTo + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Onward Date </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (model.DateOfJourney.HasValue ? model.DateOfJourney.Value.ToString("dd/MM/yyyy") : "") + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Onward Time</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.OnwardTime + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Return From </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ReturnPlaceFrom + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Return To </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ReturnPlaceTo + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Return Date </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (model.DateOfReturnJourney.HasValue ? model.DateOfReturnJourney.Value.ToString("dd/MM/yyyy") : "") + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Return Time</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ReturnTime + "</td></tr>";

            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Accommodation</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (model.Accommodation == true ? "Yes" : "No") + "</td></tr>";

            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Accommodation Note </td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.AccommodationNote + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Reimbursement By Conference/ Workshop Organisers</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.ReimbursementByConference + "</td></tr>";

            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>If Reimbursement</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.IfReimbursementByConference + "</td></tr>";

            List<tbl_TravelDeskApprover> approver = myapp.tbl_TravelDeskApprover.Where(l => l.TravelDeskId == id).ToList();

            var model2 = (from m in approver
                          join u in myapp.tbl_User on m.ApproverEmpId equals u.EmpId
                          select new
                          {
                              m.TravelDeskApproverId,
                              m.TypeOfApprover,
                              u.FirstName,
                              m.ApproverStatus,
                              m.ApproverComments
                          }).ToList();
            model2 = model2.OrderBy(l => l.TravelDeskApproverId).ToList();
            foreach (var user in model2)
            {
                mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Approver " + user.TypeOfApprover + "</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + user.FirstName + "" + user.ApproverStatus + "</td></tr>";
            }

            mailbody += "</table>";


            string body = "";
            using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/BioMedical_Asset.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("[[Heading]]", "Travel desk request " + approve.ApproverStatus + " by HOD");
            body = body.Replace("[[table]]", mailbody);
            mailmodel.body = body;
            mailmodel.filepath = "";
            //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
            mailmodel.fromname = "Travel Desk Approval";

            cm.SendEmail(mailmodel);
            return "success";
        }
        [HttpPost]
        public ActionResult savetraveldesktravel(List<tbl_TravelDeskTravel> Travel)
        {
            if (Travel != null)
            {
                for (int i = 0; i < Travel.Count; i++)
                {
                    Travel[i].CreatedBy = User.Identity.Name;
                    Travel[i].CreatedOn = DateTime.Now;
                    myapp.tbl_TravelDeskTravel.Add(Travel[i]);
                    myapp.SaveChanges();
                }
                var TraveldeskId = Travel[0].TravelDeskId;
                var TD = myapp.tbl_TravelDesk.Where(m => m.TravelDeskId == TraveldeskId).ToList();
                if (TD != null)
                {
                    TD[0].Status = TravelDeskContansts.WorkFlow_State_Completed;
                    TD[0].CurrentState = TravelDeskContansts.WorkFlow_State_Completed;
                    myapp.SaveChanges();
                    sendtraveldeskDetailstouserEmail(TD[0]);
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public string sendtraveldeskDetailstouserEmail(tbl_TravelDesk travelDesk)
        {
            var empId = Convert.ToString(travelDesk.EmpId);
            var empemail = (from var in myapp.tbl_User where var.CustomUserId == empId select var.EmailId).FirstOrDefault();
            var tdid = travelDesk.TravelDeskId;
            var travel = myapp.tbl_TravelDeskTravel.Where(m => m.TravelDeskId == tdid).ToList();
            //if withHod =>send email to travel Admin
            //if travelAdmin=>send email to travel User
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel();
            EmailTeamplates emailtemp = new EmailTeamplates();
            mailmodel.fromemail = "traveldesk@fernandez.foundation";
            mailmodel.toemail = empemail;
            mailmodel.ccemail = "traveldesk@fernandez.foundation";
            mailmodel.subject = "Travel Desk Request " + travelDesk.TravelDeskId + " please check travel desk Travel Details";
            string mailbody = "<p style='font-family:verdana'>HI Team,";
            mailbody += "<p style='font-family:verdana'>Please find the status details.</p>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Current Status</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + travelDesk.Status + "</td></tr>";
            mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Current Travel Desk state</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + travelDesk.CurrentState + "</td></tr>";
            mailbody += "</table>";
            mailbody += "<p style='font-family:verdana'>Please find the Travel Desk " + travelDesk.TravelDeskId + " Travel Deatils.</p>";
            mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><th>Mode of travel<th><th>Travel Details<th><th>Cost of Travel<th><th>Type of booking<th></tr>";
            for (int i = 0; i < travel.Count; i++)
            {
                mailbody += "<tr><td>" + travel[i].ModeOfTravel + "</td><td>" + travel[i].TravelDetails + "</td><td>" + travel[i].CostOfTravel + "</td><td>" + travel[i].TypeOfBooking + "</td></tr>";
            }

            mailbody += "</table>";
            mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
            mailmodel.body = mailbody;
            mailmodel.filepath = "";
            //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
            mailmodel.fromname = "Travel Desk Travel Details";

            cm.SendEmail(mailmodel);
            return "success";
        }
        public ActionResult GetTravelDetailsbytraveldeskid(int id)
        {
            var modal = myapp.tbl_TravelDeskTravel.Where(m => m.TravelDeskId == id).ToList();
            return Json(modal, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult savetraveldesktravelUser(int Travelid, int Accomedationid, string TravelComments, string AccomedationComments)
        {
            List<tbl_TravelDeskTravel> travel = new List<tbl_TravelDeskTravel>();
            List<tbl_TravelDeskTravel> Accomedation = new List<tbl_TravelDeskTravel>();
            int TravelDeskId = 0;
            if (Travelid != 0)
            {
                travel = myapp.tbl_TravelDeskTravel.Where(m => m.TravelDeskBookingDetailsId == Travelid).ToList();
                if (travel.Count != 0)
                {
                    TravelDeskId = travel[0].TravelDeskId.Value;
                    travel[0].UserComments = TravelComments;
                    travel[0].IsUserSelected = true;
                    travel[0].CreatedBy = User.Identity.Name;
                    travel[0].CreatedOn = DateTime.Now;
                    myapp.SaveChanges();
                }
            }
            if (Accomedationid != 0)
            {
                Accomedation = myapp.tbl_TravelDeskTravel.Where(m => m.TravelDeskBookingDetailsId == Accomedationid).ToList();
                if (Accomedation.Count != 0)
                {
                    TravelDeskId = Accomedation[0].TravelDeskId.Value;
                    Accomedation[0].UserComments = AccomedationComments;
                    Accomedation[0].IsUserSelected = true;
                    Accomedation[0].CreatedBy = User.Identity.Name;
                    Accomedation[0].CreatedOn = DateTime.Now;
                    myapp.SaveChanges();
                }
            }
            //var approvers = myapp.tbl_TravelDeskApprover.Where(l => l.TravelDeskId == TravelDeskId).ToList();
            //foreach (var approver in approvers)
            //{
            //    if (approver.TypeOfApprover == TravelDeskContansts.WorkFlow_State_Hod)
            //    {
            //        var travelreq = myapp.tbl_TravelDesk.Where(l => l.TravelDeskId == TravelDeskId).SingleOrDefault();
            //        travelreq.CurrentUser = approver.ApproverEmpId;
            //        travelreq.CurrentState = TravelDeskContansts.WorkFlow_State_Hod;
            //        travelreq.Status = TravelDeskContansts.WorkFlow_State_Hod;
            //        myapp.SaveChanges();
            //    }
            //    approver.ApproverStatus = "Pending";
            //    myapp.SaveChanges();
            //    var traveldesk = myapp.tbl_TravelDesk.Where(n => n.TravelDeskId == TravelDeskId).FirstOrDefault();
            //    var empId = Convert.ToString(traveldesk.EmpId);
            //    var empemail = (from var in myapp.tbl_User where var.CustomUserId == empId select var.EmailId).FirstOrDefault();
            //    string rpt = hrdm.GetReportingMgr(User.Identity.Name, DateTime.Now, DateTime.Now);
            //    tbl_User hod = null;
            //    if (rpt != null && rpt != "")
            //    {
            //        hod = myapp.tbl_User.Where(u => u.CustomUserId == rpt).SingleOrDefault();
            //    }

            string email = (from var in myapp.tbl_User where var.CustomUserId == TravelDeskContansts.Travel_User select var.EmailId).FirstOrDefault();
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel();
            EmailTeamplates emailtemp = new EmailTeamplates();
            mailmodel.fromemail = "traveldesk@fernandez.foundation";
            mailmodel.toemail = email;
            mailmodel.ccemail = "traveldesk@fernandez.foundation";
            mailmodel.subject = "Travel Desk Travel Request " + TravelDeskId;
            string mailbody = "<p style='font-family:verdana'>HI Team,";
            //mailbody += "<p style='font-family:verdana'> please check travel desk Travel Details</p>";
            //mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Current Status</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + traveldesk.Status + "</td></tr>";
            //mailbody += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Current Travel Desk state</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + traveldesk.CurrentState + "</td></tr>";
            //mailbody += "</table>";
            mailbody += "<p style='font-family:verdana'>Please find the Travel Desk " + TravelDeskId + " User Choose Travel Deatils.</p>";
            mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><th>Mode of travel</th><th>Travel Details</th><th>Cost of Travel</th><th>Type of booking</th><th>Comments</th></tr>";
            mailbody += "<tr><td>" + travel[0].ModeOfTravel + "</td><td>" + travel[0].TravelDetails + "</td><td>" + travel[0].CostOfTravel + "</td><td>" + travel[0].TypeOfBooking + "</td><td>" + travel[0].UserComments + "</td></tr>";
            mailbody += "<tr><td>" + Accomedation[0].ModeOfTravel + "</td><td>" + Accomedation[0].TravelDetails + "</td><td>" + Accomedation[0].CostOfTravel + "</td><td>" + Accomedation[0].TypeOfBooking + "</td><td>" + Accomedation[0].UserComments + "</td></tr>";
            mailbody += "</table>";
            mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
            mailmodel.body = mailbody;
            mailmodel.filepath = "";
            mailmodel.fromname = "Travel Desk Travel Details";

            cm.SendEmail(mailmodel);
            //}
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveReimbursementForm(TravelDeskReimbursementModel model, HttpPostedFileBase travelExpenseattachment
            , HttpPostedFileBase accommodationattachment, HttpPostedFileBase miscellaneousattachment
            , HttpPostedFileBase registrationFeeattachment)
        {
            string CurrentUser = User.Identity.Name;
            var user = myapp.tbl_User.Where(l => l.CustomUserId == model.EmpId).FirstOrDefault();
            var approver = myapp.tbl_User.Where(l => l.EmpId == user.ReportingManagerId).FirstOrDefault();
            tbl_TravelDeskReimbursementform dbmodel = new tbl_TravelDeskReimbursementform()
            {

                Accommodation = model.Accommodation,
                AccountDeptAction = "",
                AccountDeptRemarks = "",
                AccountDeptUserId = 0,
                ConferenceDate = ProjectConvert.ConverDateStringtoDatetime(model.ConferenceDate, "dd/MM/yyyy"),
                ConferenceName = model.ConferenceName,
                CreatedBy = User.Identity.Name,
                CreatedOn = DateTime.Now,
                CurrentLevel = 0,
                Designation = model.Designation,
                HodAction = "",
                HodRemarks = "",
                HodUserId = approver != null ? approver.UserId : 0,
                IsAccommodation = model.IsAccommodation == "true" ? true : false,
                IsActive = true,
                IsMiscellaneous = model.IsMiscellaneous == "true" ? true : false,
                IsRegistrationFee = model.IsRegistrationFee == "true" ? true : false,
                IsTravelExpense = model.IsTravelExpense == "true" ? true : false,
                MiscellaneousAmount = model.MiscellaneousAmount,
                Name = model.Name,
                RegistrationFeeAmount = model.RegistrationFeeAmount,
                ReimbursementId = 0,
                Status = "Pending For Approval",
                TravelExpense = model.TravelExpense,
                TravelRequestId = model.TravelRequestId,
                UpdatedBy = User.Identity.Name,
                UpdatedOn = DateTime.Now,
                UserId = user.UserId.ToString(),
                Venue = model.Venue,
                PaidThrough = model.PaidThrough,
                TotalAmount = model.TotalAmount,
                Notes = model.Notes,
                TravellerDetails = model.TravellerDetails
            };
            myapp.tbl_TravelDeskReimbursementform.Add(dbmodel);
            myapp.SaveChanges();

            if (travelExpenseattachment != null)
            {
                tbl_TravelDeskReimbursementDocument doc = new tbl_TravelDeskReimbursementDocument();
                string fileName = Path.GetFileNameWithoutExtension(travelExpenseattachment.FileName);
                string extension = Path.GetExtension(travelExpenseattachment.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;
                doc.DocumentUrl = fileName;
                travelExpenseattachment.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));
                doc.DocumentName = Path.GetFileNameWithoutExtension(travelExpenseattachment.FileName);
                doc.IsActive = true;
                doc.ReimbursementType = "TravelExpense";
                doc.ReimbursementId = dbmodel.ReimbursementId;
                myapp.tbl_TravelDeskReimbursementDocument.Add(doc);
                myapp.SaveChanges();
            }
            if (accommodationattachment != null)
            {
                tbl_TravelDeskReimbursementDocument doc = new tbl_TravelDeskReimbursementDocument();
                string fileName = Path.GetFileNameWithoutExtension(accommodationattachment.FileName);
                string extension = Path.GetExtension(accommodationattachment.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;
                doc.DocumentUrl = fileName;
                accommodationattachment.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));
                doc.DocumentName = Path.GetFileNameWithoutExtension(accommodationattachment.FileName);
                doc.IsActive = true;
                doc.ReimbursementType = "Accommodation";
                doc.ReimbursementId = dbmodel.ReimbursementId;
                myapp.tbl_TravelDeskReimbursementDocument.Add(doc);
                myapp.SaveChanges();
            }
            if (miscellaneousattachment != null)
            {
                tbl_TravelDeskReimbursementDocument doc = new tbl_TravelDeskReimbursementDocument();
                string fileName = Path.GetFileNameWithoutExtension(miscellaneousattachment.FileName);
                string extension = Path.GetExtension(miscellaneousattachment.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;
                doc.DocumentUrl = fileName;
                miscellaneousattachment.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));
                doc.DocumentName = Path.GetFileNameWithoutExtension(miscellaneousattachment.FileName);
                doc.IsActive = true;
                doc.ReimbursementType = "Miscellaneous";
                doc.ReimbursementId = dbmodel.ReimbursementId;
                myapp.tbl_TravelDeskReimbursementDocument.Add(doc);
                myapp.SaveChanges();
            }
            if (registrationFeeattachment != null)
            {
                tbl_TravelDeskReimbursementDocument doc = new tbl_TravelDeskReimbursementDocument();
                string fileName = Path.GetFileNameWithoutExtension(registrationFeeattachment.FileName);
                string extension = Path.GetExtension(registrationFeeattachment.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;
                doc.DocumentUrl = fileName;
                registrationFeeattachment.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));
                doc.DocumentName = Path.GetFileNameWithoutExtension(registrationFeeattachment.FileName);
                doc.IsActive = true;
                doc.ReimbursementType = "RegistrationFee";
                doc.ReimbursementId = dbmodel.ReimbursementId;
                myapp.tbl_TravelDeskReimbursementDocument.Add(doc);
                myapp.SaveChanges();
            }
            if (model.HodEmailId != null && model.HodEmailId != "")
            {
                string hodbody = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/TravelDeskReimbursementTemplate.html")))
                {
                    hodbody = reader.ReadToEnd();
                }
                hodbody = hodbody.Replace("[[ApproverName]]", model.HodName);
                hodbody = hodbody.Replace("[[TravelUser]]", user.FirstName + " " + model.Designation);
                hodbody = hodbody.Replace("[[TotalAmount]]", model.TotalAmount.ToString());
                hodbody = hodbody.Replace("[[Sponsoredby]]", model.PaidThrough);
                hodbody = hodbody.Replace("[[ConferenceName]]", model.ConferenceName);
                hodbody = hodbody.Replace("[[Venue]]", model.Venue);
                hodbody = hodbody.Replace("[[ConferenceDate]]", model.ConferenceDate);


                hodbody = hodbody.Replace("[[Accommodation]]", model.Accommodation.ToString());
                hodbody = hodbody.Replace("[[RegistrationFee]]", model.RegistrationFeeAmount.ToString());
                hodbody = hodbody.Replace("[[Miscellaneous]]", model.MiscellaneousAmount.ToString());
                hodbody = hodbody.Replace("[[TravelExpense]]", model.TravelExpense.ToString());

                hodbody = hodbody.Replace("[[Notes]]", model.Notes);
                hodbody = hodbody.Replace("[[TravellerDetails]]", model.TravellerDetails);

                string approveurl = "https://infonet.fernandezhospital.com/TravelDesk/ApproverReimbursementRequest?id=" + dbmodel.ReimbursementId + "&comments=ok&status=Approved&createdby=" + (approver != null ? approver.CustomUserId : "");
                string rejecturl = "https://infonet.fernandezhospital.com/TravelDesk/ApproverReimbursementRequest?id=" + dbmodel.ReimbursementId + "&comments=ok&status=Rejected&createdby=" + (approver != null ? approver.CustomUserId : "");

                hodbody = hodbody.Replace("[[ApproveUrl]]", approveurl);
                hodbody = hodbody.Replace("[[RejectUrl]]", rejecturl);
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = model.HodEmailId;

                mailmodel.subject = "A New Reimbursement Request from " + user.FirstName + "";
                mailmodel.body = hodbody;
                mailmodel.filepath = "";
                mailmodel.fromname = "Reimbursement Request";
                mailmodel.attachments = new List<string>();
                var documents = myapp.tbl_TravelDeskReimbursementDocument.Where(l => l.ReimbursementId == dbmodel.ReimbursementId).ToList();
                foreach (var document in documents)
                {
                    mailmodel.attachments.Add(document.DocumentUrl);
                }
                mailmodel.folderpath = Server.MapPath("~/Documents/AdministrationDocuments");
                cm.SendEmail(mailmodel);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult CancelReimbursementRequest(int id, string comments)
        {
            var request = myapp.tbl_TravelDeskReimbursementform.Where(l => l.ReimbursementId == id).SingleOrDefault();
            if (request != null)
            {
                request.Status = "Cancelled";
                //request.AdminComments = comments;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetReimbursementRequestDetails(int id)
        {
            var request = myapp.tbl_TravelDeskReimbursementform.Where(l => l.ReimbursementId == id).SingleOrDefault();
            TravelDeskReimbursementModel model = new TravelDeskReimbursementModel()
            {
                Accommodation = request.Accommodation.HasValue ? request.Accommodation.Value : 0,
                AccountDeptAction = request.AccountDeptAction,
                ConferenceDate = request.ConferenceDate.HasValue ? request.ConferenceDate.Value.ToString("dd/MM/yyyy") : "",
                ConferenceName = request.ConferenceName,
                AccountDeptRemarks = request.AccountDeptRemarks,
                Designation = request.Designation,
                HodAction = request.HodAction,
                HodRemarks = request.HodRemarks,
                HodUserId = request.HodUserId.HasValue ? request.HodUserId.Value : 0,
                AccountDeptUserId = request.AccountDeptUserId.HasValue ? request.AccountDeptUserId.Value : 0,
                HodUserName = "",
                IsMiscellaneous = request.IsMiscellaneous == true ? "true" : "false",
                IsAccommodation = request.IsAccommodation == true ? "true" : "false",
                IsRegistrationFee = request.IsRegistrationFee == true ? "true" : "false",
                IsTravelExpense = request.IsTravelExpense == true ? "true" : "false",
                MiscellaneousAmount = request.MiscellaneousAmount.HasValue ? request.MiscellaneousAmount.Value : 0,
                RegistrationFeeAmount = request.RegistrationFeeAmount.HasValue ? request.RegistrationFeeAmount.Value : 0,
                TotalAmount = request.TotalAmount.HasValue ? request.TotalAmount.Value : 0,
                TravelExpense = request.TravelExpense.HasValue ? request.TravelExpense.Value : 0,
                PaidThrough = request.PaidThrough,
                IsActive = request.IsActive.HasValue ? request.IsActive.Value : false,
                Status = request.Status,
                Name = request.Name,
                ReimbursementId = request.ReimbursementId,
                TravelRequestId = request.TravelRequestId.HasValue ? request.TravelRequestId.Value : 0,
                UserId = request.UserId,
                Venue = request.Venue,
                CurrentLevel = request.CurrentLevel.HasValue ? request.CurrentLevel.Value : 0,
                Notes = request.Notes,
                TravellerDetails = request.TravellerDetails,
                documents = myapp.tbl_TravelDeskReimbursementDocument.Where(l => l.ReimbursementId == id).ToList()
            };
            if (model.HodUserId > 0)
            {
                model.HodName = myapp.tbl_User.Where(u => u.UserId == model.HodUserId).SingleOrDefault().FirstName;
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult ApproverReimbursementRequest(int id, string comments, string status, string createdby = "", string Sponsoredby = "")
        {
            var travel = myapp.tbl_TravelDeskReimbursementform.Where(l => l.ReimbursementId == id).ToList();
            if (travel.Count != 0)
            {
                travel[0].HodAction = status;
                travel[0].HodRemarks = comments;
                travel[0].UpdatedBy = createdby != "" ? createdby : User.Identity.Name;
                travel[0].UpdatedOn = DateTime.Now;
                travel[0].CurrentLevel = 1;
                if (User.Identity.Name == TravelDeskContansts.Travel_Admin || User.Identity.Name == TravelDeskContansts.Travel_Admin2)
                {
                    travel[0].CurrentLevel = 2;
                    travel[0].PaidThrough = Sponsoredby;
                }
                travel[0].Status = status;
                myapp.SaveChanges();
                if (status != "Rejected")
                {
                    CustomModel cm = new CustomModel();
                    MailModel mailmodel = new MailModel();
                    EmailTeamplates emailtemp = new EmailTeamplates();
                    mailmodel.fromemail = "traveldesk@fernandez.foundation";
                    if (travel[0].CurrentLevel == 2)
                    {
                        mailmodel.toemail = "accounts@fernandez.foundation";
                        mailmodel.ccemail = "traveldesk@fernandez.foundation";
                    }
                    else
                    {
                        mailmodel.toemail = "traveldesk@fernandez.foundation";
                    }

                    mailmodel.subject = " Reimbursement request of " + travel[0].Name;
                    string mailbody = "<p style='font-family:verdana'>HI Team,";
                    mailbody += "<p style='font-family:verdana'>Please find the Reimbursement request of " + travel[0].Name + " has approved by HOD. Details are below.</p>";
                    mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><th>Label</th><th>Details</th></tr>";
                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Emp Id</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + travel[0].UserId + "</td></tr>";
                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Emp Name</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + travel[0].Name + "</td></tr>";
                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Designation</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + travel[0].Designation + "</td></tr>";

                    if (travel[0].HodUserId > 0)
                    {
                        int hoduserId = travel[0].HodUserId.Value;
                        string HodName = myapp.tbl_User.Where(u => u.UserId == hoduserId).SingleOrDefault().FirstName;
                        mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Hod Name</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + HodName + "</td></tr>";
                    }

                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Hod Remarks</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + travel[0].HodRemarks + "</td></tr>";
                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Conference Name</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + travel[0].ConferenceName + "</td></tr>";
                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Venue</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + travel[0].Venue + "</td></tr>";
                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Conference Date</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + (travel[0].ConferenceDate.HasValue ? travel[0].ConferenceDate.Value.ToString("dd/MM/yyyy") : "") + "</td></tr>";
                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Total Amount</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + travel[0].TotalAmount + "</td></tr>";
                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Sponsoredby</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + travel[0].PaidThrough + "</td></tr>";
                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Accommodation</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + travel[0].Accommodation + "</td></tr>";
                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Registration Fee</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + travel[0].RegistrationFeeAmount + "</td></tr>";
                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Miscellaneous</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + travel[0].MiscellaneousAmount + "</td></tr>";
                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Travel Expense</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + travel[0].TravelExpense + "</td></tr>";

                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Notes</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + travel[0].Notes + "</td></tr>";
                    mailbody += "<tr><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left; font-weight: bold;'>Traveller Details</td><td style='border: 1px solid #a1a2a3; padding: 8px; text-align: left;'>" + travel[0].TravellerDetails + "</td></tr>";
                    mailbody += "</table>";
                    mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";

                    mailmodel.body = mailbody;
                    mailmodel.filepath = "";
                    mailmodel.fromname = "Reimbursement request";
                    mailmodel.attachments = new List<string>();
                    var documents = myapp.tbl_TravelDeskReimbursementDocument.Where(l => l.ReimbursementId == id).ToList();
                    foreach (var document in documents)
                    {
                        mailmodel.attachments.Add(document.DocumentUrl);
                    }
                    mailmodel.folderpath = Server.MapPath("~/Documents/AdministrationDocuments");
                    cm.SendEmail(mailmodel);
                }

            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxMyReimbursementRequests(JQueryDataTableParamModel param)
        {
            string CurrentUser = User.Identity.Name;
            var tasks = myapp.tbl_TravelDeskReimbursementform.Where(l => l.CreatedBy == CurrentUser).ToList();

            tasks = tasks.OrderByDescending(t => t.ReimbursementId).ToList();
            IEnumerable<tbl_TravelDeskReimbursementform> filteredCompanies;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = tasks
                   .Where(c => c.ReimbursementId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.CreatedBy != null && c.CreatedBy.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.ConferenceName != null && c.ConferenceName.Contains(param.sSearch.ToLower())
                               ||
                              c.Venue != null && c.Venue.Contains(param.sSearch.ToLower())
                               ||
                              c.PaidThrough != null && c.PaidThrough.Contains(param.sSearch.ToLower())
                               ||
                              c.Status != null && c.Status.Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = tasks;
            }
            IEnumerable<tbl_TravelDeskReimbursementform> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[]
                                           {
                                                   Convert.ToString(c.ReimbursementId),
                                                   c.CreatedBy.ToString(),
                                                   c.Name,
                                                   c.ConferenceName,
                                                   c.Venue,
                                                   c.ConferenceDate.Value.ToString("dd/MM/yyyy"),
                                                   c.TotalAmount.HasValue?c.TotalAmount.Value.ToString("0.00"):"",
                                                   c.PaidThrough,
                                                   c.Status,
                                                   Convert.ToString(c.ReimbursementId)
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxMyReimbursementRequestsAll(JQueryDataTableParamModel param)
        {
            string CurrentUser = User.Identity.Name;
            var tasks = (from m in myapp.tbl_TravelDeskReimbursementform select m).ToList();
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                DateTime dtfrmdate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                DateTime dttodate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                if (dtfrmdate == dttodate)
                {
                    dttodate = dttodate.AddDays(1);
                }
                tasks = (from t in tasks
                         where t.CreatedOn.Value >= dtfrmdate && t.CreatedOn.Value <= dttodate
                         select t).ToList();
            }
            tasks = tasks.OrderByDescending(t => t.ReimbursementId).ToList();
            IEnumerable<tbl_TravelDeskReimbursementform> filteredCompanies;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = tasks
                   .Where(c => c.ReimbursementId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.CreatedBy != null && c.CreatedBy.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.ConferenceName != null && c.ConferenceName.Contains(param.sSearch.ToLower())
                               ||
                              c.Venue != null && c.Venue.Contains(param.sSearch.ToLower())
                               ||
                              c.PaidThrough != null && c.PaidThrough.Contains(param.sSearch.ToLower())
                               ||
                              c.Status != null && c.Status.Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = tasks;
            }
            IEnumerable<tbl_TravelDeskReimbursementform> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           let requiredApproval = c.CurrentLevel == 1 && (User.Identity.Name == TravelDeskContansts.Travel_Admin || User.Identity.Name == TravelDeskContansts.Travel_Admin2) ? "Y" : "N"
                                           select new[]
                                           {
                                                   Convert.ToString(c.ReimbursementId),
                                                   c.CreatedBy.ToString(),
                                                   c.Name,
                                                   c.ConferenceName,
                                                   c.Venue,
                                                   c.ConferenceDate.Value.ToString("dd/MM/yyyy"),
                                                   c.TotalAmount.HasValue?c.TotalAmount.Value.ToString("0.00"):"",
                                                   c.PaidThrough,
                                                   c.Status,
                                                  requiredApproval+"_"+ Convert.ToString(c.ReimbursementId)
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult TravelReimbursementDetails_ExportToExcel(string FromDate, string ToDate)
        {
            var tasks = (from m in myapp.tbl_TravelDeskReimbursementform select m).OrderByDescending(m => m.ReimbursementId).ToList();
            if (FromDate != null && FromDate != "" && ToDate != null && ToDate != "")
            {
                DateTime dtfrmdate = ProjectConvert.ConverDateStringtoDatetime(FromDate);
                DateTime dttodate = ProjectConvert.ConverDateStringtoDatetime(ToDate);
                if (dtfrmdate == dttodate)
                {
                    dttodate = dttodate.AddDays(1);
                }
                tasks = (from t in tasks
                         where t.CreatedOn.Value >= dtfrmdate && t.CreatedOn.Value <= dttodate
                         select t).ToList();
            }
            System.Data.DataTable products = new System.Data.DataTable("MyTasksDataTable");
            products.Columns.Add("Id", typeof(string));

            products.Columns.Add("EmpName", typeof(string));
            products.Columns.Add("Designation", typeof(string));
            products.Columns.Add("ConferenceName", typeof(string));
            products.Columns.Add("ConferenceDate", typeof(string));
            products.Columns.Add("Venue", typeof(string));
            products.Columns.Add("TotalAmount", typeof(string));
            products.Columns.Add("Sponsoredby", typeof(string));
            products.Columns.Add("Accommodation", typeof(string));
            products.Columns.Add("TravelExpense", typeof(string));
            products.Columns.Add("RegistrationFeeAmount", typeof(string));
            products.Columns.Add("MiscellaneousAmount", typeof(string));
            products.Columns.Add("Status", typeof(string));
            foreach (var c in tasks)
            {
                products.Rows.Add(c.ReimbursementId,
                    c.Name,

                     c.Designation,
                    c.ConferenceName,
                    c.ConferenceDate.HasValue ? c.ConferenceDate.Value.ToString("dd/MM/yyyy") : "",
                    c.Venue,
                    c.TotalAmount.ToString(),
                    c.PaidThrough,
                    c.Accommodation,
                    c.TravelExpense,
                    c.RegistrationFeeAmount,
                    c.MiscellaneousAmount,
                   c.Status
                );
                //}

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
            string filename = "ReimbursementDetails_Data.xls";
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
        public ActionResult AjaxApproveReimbursementRequests(JQueryDataTableParamModel param)
        {
            string CurrentUser = User.Identity.Name;
            var approver = myapp.tbl_User.Where(l => l.CustomUserId == CurrentUser).FirstOrDefault();
            var tasks = myapp.tbl_TravelDeskReimbursementform.Where(l => l.CurrentLevel < 2 &&
            (l.HodUserId == approver.UserId || CurrentUser == TravelDeskContansts.Travel_Admin
            || CurrentUser == TravelDeskContansts.Travel_Admin2)).ToList();
            if (CurrentUser != TravelDeskContansts.Travel_Admin && CurrentUser != TravelDeskContansts.Travel_Admin2)
            {
                tasks = tasks.Where(l => l.CurrentLevel == 0).ToList();
            }
            tasks = tasks.OrderByDescending(t => t.ReimbursementId).ToList();
            IEnumerable<tbl_TravelDeskReimbursementform> filteredCompanies;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = tasks
                   .Where(c => c.ReimbursementId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.CreatedBy != null && c.CreatedBy.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.ConferenceName != null && c.ConferenceName.Contains(param.sSearch.ToLower())
                               ||
                              c.Venue != null && c.Venue.Contains(param.sSearch.ToLower())
                               ||
                              c.PaidThrough != null && c.PaidThrough.Contains(param.sSearch.ToLower())
                               ||
                              c.Status != null && c.Status.Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = tasks;
            }
            IEnumerable<tbl_TravelDeskReimbursementform> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[]
                                           {
                                                   Convert.ToString(c.ReimbursementId),
                                                   c.CreatedBy.ToString(),
                                                   c.Name,
                                                   c.ConferenceName,
                                                   c.Venue,
                                                   c.ConferenceDate.Value.ToString("dd/MM/yyyy"),
                                                   c.TotalAmount.HasValue?c.TotalAmount.Value.ToString("0.00"):"",
                                                   c.PaidThrough,
                                                   c.Status,
                                                   Convert.ToString(c.ReimbursementId)
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
    }
}