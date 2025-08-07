using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class VehicleController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        HrDataManage hrdm = new HrDataManage();
        VehicleRequest vr = new VehicleRequest();
        // GET: Vehicle
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NewVehicleRequest()
        {
            string customerid = User.Identity.Name;
            var model = myapp.tbl_User.Where(u => u.CustomUserId == customerid).SingleOrDefault();
            if (model != null)
            {
                ViewBag.Email = model.EmailId;
                ViewBag.Mobile = model.PhoneNumber;
            }
            return View();
        }
        public ActionResult ViewVehicleRequest()
        {
            ViewBag.New = myapp.tbl_VehicleRequest.Count(l=>l.Status=="New");
            ViewBag.inprogess = myapp.tbl_VehicleRequest.Count(l => l.Status == "Pending For Approval" || l.Status== "HOD is Approved" || l.Status== "Approved");
            ViewBag.Cancel = myapp.tbl_VehicleRequest.Count(l => l.Status == "Cancel" || l.Status == "HOD is Rejected");
            ViewBag.Completed = myapp.tbl_VehicleRequest.Count(l => l.Status == "Closed");
            return View();
        }
        public ActionResult myvechiclerequests()
        {
            return View();
        }
        public ActionResult ApproverRequests()
        {
            return View();
        }
        public ActionResult AdminApproval()
        {
            return View();
        }
        //AdminSettings
        [HttpGet]
        public ActionResult AdminSetting()
        {
            return View();
        }
        public JsonResult UpdateAdminsettings(IEnumerable<tbl_Settings> datav)
        {

            foreach (var item in datav)
            {
                var cat = myapp.tbl_Settings.Where(l => l.SettingKey == item.SettingKey).ToList();
                if (cat.Count > 0)
                {
                    cat[0].SettingKey = item.SettingKey;
                    cat[0].SettingValue = item.SettingValue;
                    cat[0].IsActive = true;
                    myapp.SaveChanges();
                }
                else
                {
                    tbl_Settings settings = new tbl_Settings();
                    settings.SettingKey = item.SettingKey;
                    settings.SettingValue = item.SettingValue;
                    settings.IsActive = true;
                    myapp.tbl_Settings.Add(settings);
                    myapp.SaveChanges();
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAdminSettingsDetails()
        {
            List<tbl_Settings> arr = vr.GetAdminDetails();

            return Json(arr, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveRequest(VehicleRequestViewModel Request)
        {
            tbl_VehicleRequest mode = new tbl_VehicleRequest();
            mode.VehicleTypeRequested = Request.VehicleTypeRequested;
            mode.ToAddress = Request.ToAddress;
            mode.FromAddress = Request.FromAddress;
            mode.UserId = User.Identity.Name;
            if (Request.Date != null)
                mode.Date = ProjectConvert.ConverDateStringtoDatetime(Request.Date);
            mode.ArriveTime = Request.ArriveTime;
            mode.RequestorComments = Request.RequestorComments;
            mode.Email = Request.Email;
            mode.Mobile = Request.Mobile;
            mode.CreatedBy = User.Identity.Name;
            mode.CreatedOn = DateTime.Now;
            mode.NoofPersons = Request.NoofPersons;
            mode.returnRequired = Request.returnRequired;
            if (Request.Returndate != null)
                mode.Returndate = ProjectConvert.ConverDateStringtoDatetime(Request.Returndate);
            mode.Returntime = Request.Returntime;
            mode.ApproverId = 0;
            mode.Status = "New";
            myapp.tbl_VehicleRequest.Add(mode);
            myapp.SaveChanges();
            string customuserid = User.Identity.Name;
            var curuser = (from v in myapp.tbl_User where v.CustomUserId == customuserid select v).SingleOrDefault();
            string TEmail = (from var in myapp.tbl_Settings where var.SettingKey == "SecurityEmail" select var.SettingValue).FirstOrDefault();
            string TMobile = (from var in myapp.tbl_Settings where var.SettingKey == "SecurityPhoneNo" select var.SettingValue).FirstOrDefault();
            string AdminEmail = (from var in myapp.tbl_Settings where var.SettingKey == "AdminEmail" select var.SettingValue).FirstOrDefault();
            string AdminMobile = (from var in myapp.tbl_Settings where var.SettingKey == "AdminPhoneNO" select var.SettingValue).FirstOrDefault();
            string HodEmail = string.Empty;
            //if (curuser.UserType == "Employee")
            //{
            //var ManagerId = hrdm.GetReportingMgr(customuserid, DateTime.Now, DateTime.Now);
            var ManagerId = curuser.ReportingManagerId;
            if (ManagerId != null && ManagerId.Value > 0)
            {
                string strmanagerid = ManagerId.Value.ToString();
                HodEmail = (from var in myapp.tbl_User where var.CustomUserId == strmanagerid select var.EmailId).FirstOrDefault();
                mode.ApproverId = ManagerId;
                mode.Status = "Pending For Approval";
                myapp.SaveChanges();
            }
            //}
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel();
            EmailTeamplates emailtemp = new EmailTeamplates();
            mailmodel.fromemail = "Leave@hospitals.com";
            mailmodel.toemail = HodEmail;
            //mailmodel.ccemail = AdminEmail;
            //if (HodEmail != string.Empty)
            //{
            //    mailmodel.ccemail += "," + HodEmail;
            //}
            mailmodel.subject = "A Vehicle Booking Request " + mode.VehicleRequestId + "";
            string mailbody = "<p style='font-family:verdana'>Hello Sir,";
            if (ManagerId != null && ManagerId.Value > 0)
            {
                mailmodel.toemail = HodEmail;
                mailbody += "<p style='font-family:verdana'>" + curuser.FirstName + " has requested Vehicle. Please view the request and approve. Click  <a target='_blank' href='https://infonet.fernandezhospital.com/'>here</a>  to see the request.</p>";
            }
            else
            {

                mailmodel.toemail = TEmail;
                mailmodel.ccemail = AdminEmail;
                mailbody += "<p style='font-family:verdana'>" + curuser.FirstName + " has requested Vehicle. Please click on this <a target='_blank' href='https://infonet.fernandezhospital.com/'>link</a>  to see the request.Do not forget to update the request status after completion.</p>";
            }
            mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
            mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Pick Up Date and Time</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Request.Date + " " + Request.ArriveTime + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Return Date and Time</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + Request.Returndate + " " + Request.Returntime + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Type of Vehicle </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.VehicleTypeRequested + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'> Pick Up Location & Address</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.FromAddress + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'> Drop Location & Address</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.ToAddress + "</td></tr>";
            //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Arrive Time</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.ArriveTime + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Purpose of requisition</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.RequestorComments + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Name </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + curuser.FirstName + "</td></tr>";
            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Contact No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.Mobile + "</td></tr>";
            mailbody += "</table>";
            mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
            mailmodel.body = mailbody;
            //mailmodel.body = "A New Ticket Assigned to you";
            //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
            mailmodel.filepath = "";
            //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
            mailmodel.fromname = "Vehicle Request";

            cm.SendEmail(mailmodel);
            if (TMobile != null && TMobile != "")
            {
                if (AdminMobile != null && AdminMobile != "")
                {
                    TMobile = TMobile + "," + AdminMobile;
                }
                SendSms smodel = new SendSms();
                smodel.SendSmsToEmployee(TMobile, "HI Team Vehicle Booking Request from " + curuser.FirstName + ". Id :" + mode.VehicleRequestId);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ApproveVehicleRequest(int id, string remarks)
        {
            var mode = myapp.tbl_VehicleRequest.Where(l => l.VehicleRequestId == id).SingleOrDefault();
            if (mode != null)
            {
                mode.Status = "HOD is Approved";
                mode.IsApproved = true;
                mode.ApprovedOn = DateTime.Now;
                mode.ApproverRemarks = remarks;
                myapp.SaveChanges();
                string customuserid = mode.CreatedBy;
                var curuser = (from v in myapp.tbl_User where v.CustomUserId == customuserid select v).SingleOrDefault();
                string TEmail = (from var in myapp.tbl_Settings where var.SettingKey == "SecurityEmail" select var.SettingValue).FirstOrDefault();
                string TMobile = (from var in myapp.tbl_Settings where var.SettingKey == "SecurityPhoneNo" select var.SettingValue).FirstOrDefault();
                string AdminEmail = (from var in myapp.tbl_Settings where var.SettingKey == "AdminEmail" select var.SettingValue).FirstOrDefault();
                string AdminMobile = (from var in myapp.tbl_Settings where var.SettingKey == "AdminPhoneNO" select var.SettingValue).FirstOrDefault();
                string HodEmail = string.Empty;
                //if (curuser.UserType == "Employee")
                //{
                //var ManagerId = hrdm.GetReportingMgr(customuserid, DateTime.Now, DateTime.Now);
                var ManagerId = curuser.ReportingManagerId;

                //}
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = HodEmail;
                //mailmodel.ccemail = AdminEmail;

                mailmodel.subject = "A Vehicle Booking Request " + mode.VehicleRequestId + "";
                string mailbody = "<p style='font-family:verdana'>Hello Sir,";
                mailmodel.toemail = TEmail;
                mailmodel.ccemail = AdminEmail;
                if (curuser.EmailId != null && curuser.EmailId != string.Empty)
                {
                    mailmodel.ccemail += "," + curuser.EmailId;
                }
                mailbody += "<p style='font-family:verdana'>" + curuser.FirstName + " has requested Vehicle and It has approved by Manager. Please click on this <a target='_blank' href='https://infonet.fernandezhospital.com/'>link</a>  to see the request.Do not forget to update the request status after completion.</p>";

                mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Pick Up Date and Time</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.Date + " " + mode.ArriveTime + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Return Date and Time</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.Returndate + " " + mode.Returntime + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Type of Vehicle </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.VehicleTypeRequested + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'> Pick Up Location & Address</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.FromAddress + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'> Drop Location & Address</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.ToAddress + "</td></tr>";
                //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Arrive Time</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.ArriveTime + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Purpose of requisition</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.RequestorComments + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Name </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + curuser.FirstName + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Contact No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.Mobile + "</td></tr>";
                mailbody += "</table>";
                mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
                mailmodel.body = mailbody;
                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "Vehicle Request";

                cm.SendEmail(mailmodel);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult RejectVehicleRequest(int id, string remarks)
        {
            var mode = myapp.tbl_VehicleRequest.Where(l => l.VehicleRequestId == id).SingleOrDefault();
            if (mode != null)
            {
                mode.Status = "HOD is Rejected";
                mode.IsRejected = true;
                mode.ApprovedOn = DateTime.Now;
                mode.ApproverRemarks = remarks;
                myapp.SaveChanges();

                string customuserid = mode.CreatedBy;
                var curuser = (from v in myapp.tbl_User where v.CustomUserId == customuserid select v).SingleOrDefault();

                string HodEmail = string.Empty;
                //if (curuser.UserType == "Employee")
                //{
                //var ManagerId = hrdm.GetReportingMgr(customuserid, DateTime.Now, DateTime.Now);
                var ManagerId = curuser.ReportingManagerId;

                //}
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = HodEmail;
                //mailmodel.ccemail = AdminEmail;

                mailmodel.subject = "A Vehicle Booking Request " + mode.VehicleRequestId + "";
                string mailbody = "<p style='font-family:verdana'>Dear " + curuser.FirstName + ",";
                mailmodel.toemail = curuser.EmailId;

                mailbody += "<p style='font-family:verdana'>The Vehicle request has rejected by Manager .</p>";

                mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Pick Up Date and Time</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.Date + " " + mode.ArriveTime + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Return Date and Time</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.Returndate + " " + mode.Returntime + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Type of Vehicle </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.VehicleTypeRequested + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'> Pick Up Location & Address</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.FromAddress + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'> Drop Location & Address</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.ToAddress + "</td></tr>";
                //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Arrive Time</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.ArriveTime + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Purpose of requisition</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.RequestorComments + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Name </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + curuser.FirstName + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Contact No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.Mobile + "</td></tr>";
                mailbody += "</table>";
                mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
                mailmodel.body = mailbody;
                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "Vehicle Request";

                cm.SendEmail(mailmodel);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetAdminRequest(JQueryDataTableParamModel param)
        {
            var userlist = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            var query = myapp.tbl_VehicleRequest.OrderByDescending(x => x.VehicleRequestId).ToList();
            IEnumerable<tbl_VehicleRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c =>
                    c.VehicleRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                    || c.ToAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.FromAddress != null && c.FromAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.VehicleTypeRequested != null && c.VehicleTypeRequested.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Date != null && c.Date.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         join u in userlist on c.UserId equals u.CustomUserId
                         select new object[] {
                             c.VehicleRequestId.ToString(),
                            u.CustomUserId,
                             u.FirstName+"("+u.Designation+")",

                             u.LocationName+" "+u.DepartmentName,

                             c.VehicleTypeRequested,
                             c.FromAddress,
                             c.ToAddress,
                             c.Date.HasValue?(c.Date.Value.ToString("dd/MM/yyyy")+" "+c.ArriveTime):"",
                             c.NoofPersons,
                             c.returnRequired,
                             c.Returndate.HasValue?(c.Returndate.Value.ToString("dd/MM/yyyy")+" "+c.Returntime):"",
                             c.Email,
                             c.Mobile,
                             c.Status,
                             //c.Admin_Status,
                             c.VehicleRequestId
                           };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetVehicleRequestsForApproval(JQueryDataTableParamModel param)
        {
            //var userlist = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            int currentempid = int.Parse(User.Identity.Name);
            var query = myapp.tbl_VehicleRequest.Where(d => d.ApproverId == currentempid && d.ApprovedOn == null).OrderByDescending(x => x.VehicleRequestId).ToList();
            IEnumerable<tbl_VehicleRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.ToAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.FromAddress != null && c.FromAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.VehicleRegisterType != null && c.VehicleRegisterType.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Date != null && c.Date.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         //join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         let user = myapp.tbl_User.Where(l => l.CustomUserId == c.CreatedBy).SingleOrDefault()
                         select new object[] {
                             c.VehicleRequestId,
                           user!=null? user.FirstName:"",
                            user!=null?user.DepartmentName:"",
                            user!=null?user.Designation:"",
                             c.VehicleTypeRequested,
                             c.FromAddress,
                             c.ToAddress,
                             c.Date.Value.ToString("dd/MM/yyyy")+" "+c.ArriveTime,
                             c.ArriveTime,
                               c.returnRequired,
                                c.Returndate.HasValue?(c.Returndate.Value.ToString("dd/MM/yyyy")+" "+c.Returntime):"",
                             c.Admin_Status,
                             c.Status,
                             c.VehicleDetails,
                             c.VehicleRequestId
                           };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetMyVehicleRequest(JQueryDataTableParamModel param)
        {
            //var userlist = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            var query = myapp.tbl_VehicleRequest.Where(d => d.UserId == User.Identity.Name).OrderByDescending(x => x.VehicleRequestId).ToList();
            IEnumerable<tbl_VehicleRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.ToAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.FromAddress != null && c.FromAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.VehicleRegisterType != null && c.VehicleRegisterType.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Date != null && c.Date.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new object[] {
                             c.VehicleRequestId,
                             //u.FirstName,
                             //u.DepartmentName,
                             //u.Designation,
                             //u.PhoneNumber,
                             //u.Extenstion,
                             c.VehicleTypeRequested,
                             c.FromAddress,
                             c.ToAddress,
                             c.Date.Value.ToString("dd/MM/yyyy")+" "+c.ArriveTime,
                             c.ArriveTime,
                               c.returnRequired,
                                c.Returndate.HasValue?(c.Returndate.Value.ToString("dd/MM/yyyy")+" "+c.Returntime):"",
                             c.Admin_Status,
                             c.Status,
                             c.VehicleDetails,
                             c.VehicleRequestId
                           };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetVehicleRequest(JQueryDataTableParamModel param)
        {
            var userlist = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            var query = myapp.tbl_VehicleRequest.OrderByDescending(x => x.VehicleRequestId).ToList();
            // var query = myapp.tbl_VehicleRequest.Where(d =>  d.Admin_Status == "APPROVED").ToList();
            IEnumerable<tbl_VehicleRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c =>
                    c.VehicleRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                    ||
                    c.ToAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.FromAddress != null && c.FromAddress.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.VehicleRegisterType != null && c.VehicleRegisterType.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Date != null && c.Date.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         join u in userlist on c.UserId equals u.CustomUserId
                         select new object[] {
                             c.VehicleRequestId.ToString(),
                             c.UserId,
                             u.FirstName+ "("+u.Designation+")",
                             u.LocationName+"-"+u.DepartmentName,
                             c.Mobile,
                             c.VehicleTypeRequested,
                             c.FromAddress,
                             c.ToAddress,
                             c.Date.Value.ToString("dd/MM/yyyy")+" "+c.ArriveTime,
                            c.NoofPersons,
                            c.returnRequired,
                             c.Returndate.HasValue?(c.Returndate.Value.ToString("dd/MM/yyyy")+" "+c.Returntime):"",
                             c.Status,
                             c.VehicleDetails,
                             c.Email,
                             c.VehicleRequestId
                           };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateVehicleRequest(VehicleRequestViewModel UpdateRequest)
        {
            var Status = vr.UpdateRequest(UpdateRequest);
            return Json(Status, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRequestVehicleById(int RequestId)
        {
            VehicleRequestViewModel viewmode = vr.GetRequestbyId(RequestId);
            return Json(viewmode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelVehicleRequest(string FromDate, string ToDate)
        {
            DateTime fromdate = Convert.ToDateTime(FromDate);
            DateTime todate = Convert.ToDateTime(ToDate);
            var Result = myapp.tbl_VehicleRequest.Where(p => p.Date >= fromdate && p.Date <= todate).ToList();

            var products = new System.Data.DataTable("VehicleRequest");
            products.Columns.Add("Request Id", typeof(string));
            products.Columns.Add("User Id", typeof(string));
            products.Columns.Add("Request Type", typeof(string));
            products.Columns.Add("No of Person", typeof(string));
            products.Columns.Add("Pick up Date & Time", typeof(string));
            products.Columns.Add("Pick Up Location", typeof(string));
            products.Columns.Add("Drop Location", typeof(string));
            products.Columns.Add("Return Required", typeof(string));
            products.Columns.Add("Return Date & Time", typeof(string));
            products.Columns.Add("Mobile Number", typeof(string));
            products.Columns.Add("Email", typeof(string));

            products.Columns.Add("Purpose of requisition", typeof(string));
            products.Columns.Add("Transport Status", typeof(string));
            products.Columns.Add("Admin Status", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            foreach (var item in Result)
            {
                products.Rows.Add(
               item.VehicleRequestId.ToString(),
                item.UserId,
                 item.VehicleTypeRequested,
                 item.NoofPersons.ToString(),
             item.Date.Value.ToString("dd/MM/yyyy") + " " + item.ArriveTime,
            item.FromAddress,
            item.ToAddress,
             item.returnRequired,
             item.Returndate.HasValue ? (item.Returndate.Value.ToString("dd/MM/yyyy") + " " + item.Returntime) : "",
item.Mobile.ToString(),
item.Email,
item.RequestorComments,
item.Status,
item.Admin_Status,
item.Remarks

                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=VehicleRequest.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateStatus(int Id, string Status, string Remarks)
        {
            var ValStatus = true;
            var cat = myapp.tbl_VehicleRequest.Where(l => l.VehicleRequestId == Id).ToList();
            if (cat.Count > 0)
            {
                if (cat[0].Admin_Status == null)
                {
                    tbl_VehicleRequestComment c = new tbl_VehicleRequestComment();
                    c.VehicleRequestId = Id;
                    c.Comment = Remarks != null ? Remarks : cat[0].Remarks;
                    c.CommentType = Status;
                    c.CommentOn = DateTime.Now;
                    c.CommentBy = User.Identity.Name;
                    myapp.tbl_VehicleRequestComment.Add(c);
                    myapp.SaveChanges();
                    cat[0].Status = Status;
                    cat[0].Remarks = Remarks != null ? Remarks : cat[0].Remarks;
                }
                else
                {
                    ValStatus = false;
                    return Json(ValStatus, JsonRequestBehavior.AllowGet);
                }

            }
            myapp.SaveChanges();
            ValStatus = vr.sendEmailforUpdateSatatus(cat);
            return Json(ValStatus, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetVehicleComments(int id)
        {
            var commentlist = myapp.tbl_VehicleRequestComment.Where(l => l.VehicleRequestId == id).ToList();
            var vmodel = myapp.tbl_VehicleRequest.Where(l => l.VehicleRequestId == id).SingleOrDefault();
            var model = (from c in commentlist
                         let CommentOn = c.CommentOn.HasValue ? c.CommentOn.Value.ToString("dd/MM/yyyy hh:mm tt") : ""
                         select new
                         {
                             c.Comment,
                             c.CommentBy,
                             CommentOn,
                             c.CommentType,
                             vmodel.VehicleDetails
                         }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult addComment(int id, string comment)
        {
            var cat = myapp.tbl_VehicleRequest.Where(l => l.VehicleRequestId == id).ToList();
            tbl_VehicleRequestComment c = new tbl_VehicleRequestComment();
            c.VehicleRequestId = id;
            c.Comment = comment;
            c.CommentType = "General";
            c.CommentOn = DateTime.Now;
            c.CommentBy = User.Identity.Name;
            myapp.tbl_VehicleRequestComment.Add(c);
            myapp.SaveChanges();
            vr.sendEmailforUpdateSatatus(cat);
            return Json("Success", JsonRequestBehavior.AllowGet);

        }
        public JsonResult CancelRequestByUser(int Id, string Status)
        {
            var ValStatus = true;
            var cat = myapp.tbl_VehicleRequest.Where(l => l.VehicleRequestId == Id).ToList();
            if (cat.Count > 0)
            {
                if (cat[0].Admin_Status == "In progress" || cat[0].Status == "In progress" || cat[0].Admin_Status == null || cat[0].Status == null)
                {
                    cat[0].Status = Status;
                    tbl_VehicleRequestComment c = new tbl_VehicleRequestComment();
                    c.VehicleRequestId = Id;
                    c.Comment = "User has Cancelled the Request";
                    c.CommentType = Status;
                    c.CommentOn = DateTime.Now;
                    c.CommentBy = User.Identity.Name;
                    myapp.tbl_VehicleRequestComment.Add(c);
                    myapp.SaveChanges();
                }
                else
                {
                    ValStatus = false;
                    return Json(ValStatus, JsonRequestBehavior.AllowGet);
                }

            }
            myapp.SaveChanges();
            string Email = (from var in myapp.tbl_Settings
                            where var.SettingKey == "SecurityEmail"
                            select var.SettingValue).FirstOrDefault();
            string TEmail = (from var in myapp.tbl_Settings where var.SettingKey == "SecurityEmail" select var.SettingValue).FirstOrDefault();
            string TMobile = (from var in myapp.tbl_Settings where var.SettingKey == "SecurityPhoneNo" select var.SettingValue).FirstOrDefault();
            string AdminEmail = (from var in myapp.tbl_Settings where var.SettingKey == "AdminEmail" select var.SettingValue).FirstOrDefault();
            string AdminMobile = (from var in myapp.tbl_Settings where var.SettingKey == "AdminPhoneNO" select var.SettingValue).FirstOrDefault();

            string Customuserid = cat[0].UserId;
            var curuser = (from v in myapp.tbl_User where v.CustomUserId == Customuserid select v).SingleOrDefault();
            //string HodEmail = string.Empty;
            //if (curuser.UserType == "Employee")
            //{
            //    var ManagerId = hrdm.GetReportingMgr(cat[0].UserId, DateTime.Now, DateTime.Now);
            //    HodEmail = (from var in myapp.tbl_User where var.CustomUserId == ManagerId select var.EmailId).FirstOrDefault();
            //}
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel();
            EmailTeamplates emailtemp = new EmailTeamplates();
            mailmodel.fromemail = "Leave@hospitals.com";
            mailmodel.toemail = TEmail;
            mailmodel.ccemail = AdminEmail;
            //if (HodEmail != string.Empty)
            //{
            //    mailmodel.ccemail += "," + HodEmail;
            //}
            mailmodel.subject = "A Vehicle Booking Request ID No : " + cat[0].VehicleRequestId + " has been " + cat[0].Status;
            string mailbody = "<p style='font-family:verdana'>Dear Sir/Madam,";
            mailbody += "<p style='font-family:verdana'> Request is Cancelled by " + Customuserid + "</p>";
            mailbody += "<br/><p style='font-family:cambria'>This is an auto generated mail,Don't reply to this.</p>";

            mailmodel.body = mailbody;
            //mailmodel.body = "A New Ticket Assigned to you";
            //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
            mailmodel.filepath = "";
            //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
            mailmodel.fromname = "Vehicle Request";

            cm.SendEmail(mailmodel);
            return Json(ValStatus, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateAdminStatus(int Id, string Status, string Remarks)
        {
            var ValStatus = true;
            var cat = myapp.tbl_VehicleRequest.Where(l => l.VehicleRequestId == Id).ToList();
            if (cat.Count > 0)
            {
                cat[0].Admin_Status = Status;
                cat[0].Status = Status;
                cat[0].Remarks = Remarks != null ? Remarks : cat[0].Remarks;
                tbl_VehicleRequestComment c = new tbl_VehicleRequestComment();
                c.VehicleRequestId = Id;
                c.Comment = Remarks != null ? Remarks : cat[0].Remarks;
                c.CommentType = Status;
                c.CommentOn = DateTime.Now;
                c.CommentBy = User.Identity.Name;
                myapp.tbl_VehicleRequestComment.Add(c);
                myapp.SaveChanges();
            }
            else
            {
                ValStatus = false;
                return Json(ValStatus, JsonRequestBehavior.AllowGet);
            }
            myapp.SaveChanges();
            ValStatus = vr.sendEmailforUpdateSatatus(cat);
            //string customuserid = cat[0].UserId;
            //var curuser = (from v in myapp.tbl_User where v.CustomUserId == customuserid select v).SingleOrDefault();
            //string TEmail = (from var in myapp.tbl_Settings where var.SettingKey == "SecurityEmail" select var.SettingValue).FirstOrDefault();
            //string TMobile = (from var in myapp.tbl_Settings where var.SettingKey == "SecurityPhoneNo" select var.SettingValue).FirstOrDefault();
            //string AdminEmail = (from var in myapp.tbl_Settings where var.SettingKey == "AdminEmail" select var.SettingValue).FirstOrDefault();
            //string AdminMobile = (from var in myapp.tbl_Settings where var.SettingKey == "AdminPhoneNO" select var.SettingValue).FirstOrDefault();
            //string HodEmail = string.Empty;
            //if (curuser.UserType == "Employee")
            //{
            //    var ManagerId = hrdm.GetReportingMgr(cat[0].UserId, DateTime.Now, DateTime.Now);
            //    HodEmail = (from var in myapp.tbl_User where var.CustomUserId == ManagerId select var.EmailId).FirstOrDefault();
            //}
            //CustomModel cm = new CustomModel();
            //MailModel mailmodel = new MailModel();
            //EmailTeamplates emailtemp = new EmailTeamplates();
            //mailmodel.fromemail = "Leave@hospitals.com";
            //mailmodel.toemail = TEmail;
            //mailmodel.ccemail =  AdminEmail + "," + cat[0].Email;
            //if (HodEmail != string.Empty)
            //{
            //    mailmodel.ccemail += "," + HodEmail;
            //}
            //string mailbody = "<p style='font-family:verdana'>HI Team,";
            //mailmodel.subject = "A Vehicle Booking Request " + cat[0].VehicleRequestId + " has " + cat[0].Status;
            //if (Status == "APPROVED")
            //{
            //    mailbody += "<p style='font-family:verdana'> Your Vehicle request has updated.</p>";
            //    mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
            //    mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Pick Up Date</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Date + "</td></tr>";
            //    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Type Of Vehicle</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].VehicleTypeRequested + "</td></tr>";
            //    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Pick Up Address</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].FromAddress + "</td></tr>";
            //    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Drop Address</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].ToAddress + "</td></tr>";
            //    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Arrive Time</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].ArriveTime + "</td></tr>";
            //    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Comments</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].RequestorComments + "</td></tr>";
            //    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + curuser.FirstName + "</td></tr>";
            //    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Phone</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Mobile + "</td></tr>";
            //    mailbody += "</table>";
            //    if (cat[0].Mobile != null && cat[0].Mobile.ToString() != "")
            //    {
            //        SendSms smodel = new SendSms();
            //        smodel.SendSmsToEmployee(cat[0].Mobile.ToString(), "Hi " + curuser.FirstName + ", Your Vehicle request has Approved for " + cat[0].Date.Value.ToString("dd/MM/yyyy"));
            //    }
            //}
            //if (Status == "REJECTED")
            //{
            //    mailbody += "<p style='font-family:verdana'>Request has been Rejected by ADMIN.</p>";
            //    mailbody += "<br/><p style='font-family:cambria'>This is an auto generated mail,Don't reply to this.</p>";
            //    if (cat[0].Mobile != null && cat[0].Mobile.ToString() != "")
            //    {
            //        SendSms smodel = new SendSms();
            //        smodel.SendSmsToEmployee(cat[0].Mobile.ToString(), "Hi " + curuser.FirstName + ", Your Vehicle request has Rejected for " + cat[0].Date.Value.ToString("dd/MM/yyyy"));
            //    }
            //}
            //if (Status == "INPROGESS")
            //{
            //    mailbody += "<p style='font-family:verdana'>Request is INPROGESS Please wait for the Admin Approval.</p>";
            //    mailbody += "<br/><p style='font-family:cambria'>This is an auto generated mail,Don't reply to this.</p>";
            //    if (cat[0].Mobile != null && cat[0].Mobile.ToString() != "")
            //    {
            //        SendSms smodel = new SendSms();
            //        smodel.SendSmsToEmployee(cat[0].Mobile.ToString(), "Hi " + curuser.FirstName + ", Your Vehicle request is INPROGESS for " + cat[0].Date.Value.ToString("dd/MM/yyyy"));
            //    }

            //}
            //mailmodel.body = mailbody;
            ////mailmodel.body = "A New Ticket Assigned to you";
            ////mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
            //mailmodel.filepath = "";
            ////mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
            //mailmodel.fromname = "Vehicle Request";

            //cm.SendEmail(mailmodel);
            return Json(ValStatus, JsonRequestBehavior.AllowGet);
        }
    }
}
