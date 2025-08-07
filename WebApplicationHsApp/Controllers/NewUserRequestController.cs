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
    public class NewUserRequestController : Controller
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        private HrDataManage hrdm = new HrDataManage();
        // GET: NewUserRequest
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MyRequests()
        {
            return View();
        }
        public ActionResult MyApproval()
        {
            return View();
        }
        public ActionResult RequestSupport()
        {
            return View();
        }
        public ActionResult AjaxGetMyRequests(JQueryDataTableParamModel param)
        {
            string Createdby = User.Identity.Name;
            List<tbl_NewUserAccessRequest> query = myapp.tbl_NewUserAccessRequest.ToList();

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(l => l.CreatedBy == Createdby).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn != null && x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn != null && x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            List<tbl_Location> locations = myapp.tbl_Location.ToList();
            for (int i = 0; i < query.Count; i++)
            {
                var Approveloc = "";
                var Approve = query[i].ApprovedLocation != null ? query[i].ApprovedLocation.Split(',') : null;
                if (Approve != null)
                {
                    for (int j = 0; j < Approve.Length; j++)
                    {
                        if (Approve[j] != "")
                        {
                            var Loc = Convert.ToInt32(Approve[j]);
                            if (Loc != 0)
                            {
                                if (Approveloc == "")
                                {
                                    Approveloc = locations.Where(l => l.LocationId == Loc).SingleOrDefault().LocationName;
                                }
                                else
                                {
                                    Approveloc = Approveloc + "," + locations.Where(l => l.LocationId == Loc).SingleOrDefault().LocationName;
                                }
                            }
                        }
                    }
                    query[i].ApprovedLocation = Approveloc;
                }

            }
            IEnumerable<tbl_NewUserAccessRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.UserRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                   ||
                    c.Name != null && c.Name.ToLower().Contains(param.sSearch.ToLower())
                    ||
                    c.EmailId != null && c.EmailId.ToLower().Contains(param.sSearch.ToLower())
                    ||
                  c.Mobile != null && c.Mobile.ToLower().Contains(param.sSearch.ToLower())
                   ||
                   c.EmpNo != null && c.EmpNo.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_NewUserAccessRequest> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                               //join l in locations on c.ApprovedLocation equals l.LocationId
                                           select new object[] {
                             c.UserRequestId.ToString(),
                              c.EmpNo,
                                              c.Name,
                                              c.EmailId,
                                              c.Mobile,
                                                c.Designation,
                                                 c.SubDepartment,
                                                c.Remarks,
                                                c.IsHodApproved.HasValue?c.IsHodApproved.Value.ToString():"No",
                                                c.HodApproveComments,

                                            c.ApprovedLocation,
                                                c.IsSupportTeamCompleted,
                                                c.UsernameProvided,
                                                c.PasswordProvided,
                                                c.SupportTeamComments,
                                                c.CreatedOn!=null?c.CreatedOn.Value.ToString("dd/MM/yyyy"):"",
                                               c.UserRequestId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelNewuser(string fromDate, string toDate, int locationid = 0)
        {
            string Createdby = User.Identity.Name;
            List<tbl_NewUserAccessRequest> query = myapp.tbl_NewUserAccessRequest.ToList();

            if (!User.IsInRole("Admin"))
            {
                query = query.Where(l => l.CreatedBy == Createdby).ToList();
            }
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn != null && x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn != null && x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            var products = new System.Data.DataTable("UserAccessRequest");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("EmpNo", typeof(string));
            products.Columns.Add("Name", typeof(string));
            products.Columns.Add("EmailId", typeof(string));
            products.Columns.Add("Mobile", typeof(string));
            products.Columns.Add("Designation", typeof(string));
            products.Columns.Add("Sub Department", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("IsHodApproved", typeof(string));
            products.Columns.Add("Comments", typeof(string));

            products.Columns.Add("Approve Location", typeof(string));
            products.Columns.Add("Is Support Approved", typeof(string));
            products.Columns.Add("User Name", typeof(string));
            products.Columns.Add("Password", typeof(string));
            products.Columns.Add("Support Comments", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();

                products.Rows.Add(
                   c.UserRequestId.ToString(),
                              c.EmpNo,
                                              c.Name,
                                              c.EmailId,
                                              c.Mobile,
                                                c.Designation,
                                                 c.SubDepartment,
                                                c.Remarks,
                                                c.IsHodApproved.HasValue ? c.IsHodApproved.Value.ToString() : "No",
                                                c.HodApproveComments,

                                            c.ApprovedLocation,
                                                c.IsSupportTeamCompleted,
                                                c.UsernameProvided,
                                                c.PasswordProvided,
                                                c.SupportTeamComments,
                                                c.CreatedOn != null ? c.CreatedOn.Value.ToString("dd/MM/yyyy") : ""
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=UserAccessRequest.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetRequestsSupport(JQueryDataTableParamModel param)
        {
            //List<tbl_Location> locations = myapp.tbl_Location.ToList();
            //var userslist = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            List<tbl_NewUserAccessRequest> query = myapp.tbl_NewUserAccessRequest.Where(l => l.IsHodApproved == true).OrderByDescending(l => l.CreatedOn).ToList();
            if (param.status != null && param.status == "Yes")
            {
                query = query.Where(l => l.IsSupportTeamCompleted == true).ToList();
            }
            else if (param.status != null && param.status == "No")
            {
                query = query.Where(l => l.IsSupportTeamCompleted == false).ToList();
            }
            else
            {
                query = query.Where(l => l.IsSupportTeamCompleted == false).ToList();
            }
            IEnumerable<tbl_NewUserAccessRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.UserRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                   ||
                    c.Name != null && c.Name.ToLower().Contains(param.sSearch.ToLower())
                    ||
                    c.EmailId != null && c.EmailId.ToLower().Contains(param.sSearch.ToLower())
                    ||
                  c.Mobile != null && c.Mobile.ToLower().Contains(param.sSearch.ToLower())
                   ||
                   c.EmpNo != null && c.EmpNo.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_NewUserAccessRequest> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                               //join l in locations on c.ApprovedLocation.Contains(l.LocationId)

                                           select new object[] {
                             c.UserRequestId.ToString(),
                              c.EmpNo,
                                              c.Name,
                                              c.EmailId,
                                              c.Mobile,
                                                c.Designation,
                                                c.SubDepartment,
                                                c.Remarks,
                                                c.IsHodApproved.HasValue?c.IsHodApproved.Value.ToString():"No",
                                                c.HodApproveComments,
                                               "",//l.LocationName,
                                               c.ModifiedBy,
                                               c.ModifiedOn.Value.ToString("dd/MM/yyyy"),
                                               c.UserRequestId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetRequestsPendingforapproval(JQueryDataTableParamModel param)
        {
            string UserId = User.Identity.Name;
            List<tbl_NewUserAccessRequest> query = myapp.tbl_NewUserAccessRequest.Where(l => l.HodApprovalId == UserId && l.IsHodApproved == false).ToList();
            IEnumerable<tbl_NewUserAccessRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.UserRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                   ||
                    c.Name != null && c.Name.ToLower().Contains(param.sSearch.ToLower())
                    ||
                    c.EmailId != null && c.EmailId.ToLower().Contains(param.sSearch.ToLower())
                    ||
                  c.Mobile != null && c.Mobile.ToLower().Contains(param.sSearch.ToLower())
                   ||
                   c.EmpNo != null && c.EmpNo.ToLower().Contains(param.sSearch.ToLower())
                    );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_NewUserAccessRequest> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           select new object[] {
                             c.UserRequestId.ToString(),
                              c.EmpNo,
                                              c.Name,
                                              c.EmailId,
                                              c.Mobile,
                                                c.Designation,
                                                 c.SubDepartment,
                                                c.Remarks,
                                                c.IsHodApproved.HasValue?c.IsHodApproved.Value.ToString():"No",
                                                c.HodApproveComments,
                                               c.UserRequestId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSubdepartmentList()
        {
            List<tbl_NewUserRequestSubdepartment> list = myapp.tbl_NewUserRequestSubdepartment.Where(l => l.IsActive == true).OrderBy(l => l.Name).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveNewUserRequest(tbl_NewUserAccessRequest model)
        {
            string rpt = "";
            int employeeid = int.Parse(model.EmpNo.Replace("fh_", ""));
            tbl_User employe = myapp.tbl_User.Where(l => l.EmpId == employeeid).SingleOrDefault();
            model.ApprovedLocation = "0";
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            model.EmpDepartmentId = employe.DepartmentId;
            model.EmpLocationId = employe.LocationId;
            rpt = employe.ReportingManagerId.HasValue ? employe.ReportingManagerId.Value.ToString() : "0";
            model.HodApprovalId = rpt;
            model.HodApproveComments = "";
            model.IsActive = true;
            model.IsHodApproved = false;
            model.IsSupportTeamCompleted = false;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            model.SupportTeamComments = "";
            myapp.tbl_NewUserAccessRequest.Add(model);
            myapp.SaveChanges();

            tbl_User hodporfile = myapp.tbl_User.Where(l => l.CustomUserId == rpt).SingleOrDefault();
            if (hodporfile != null)
            {
                // Send email to Manager
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = hodporfile.EmailId;
                mailmodel.ccemail = "ahmadali@fernandez.foundation";
                //if (hodporfile.EmailId != string.Empty)
                //{
                //    mailmodel.ccemail += "," + HodEmail;
                //}
                mailmodel.subject = "A New User Access Request " + model.UserRequestId + "";
                string mailbody = "<p style='font-family:verdana'>Dear Sir,";
                mailbody += "<p style='font-family:verdana'>" + employe.FirstName + " has requested for new user access . Please click on this <a target='_blank' href='https://infonet.fernandezhospital.com/'>link</a>  to see the request.Do not forget to update the request status after completion.</p>";
                mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Emp No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + model.EmpNo + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Emp Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + model.Name + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Designation </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + model.Designation + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Email Id</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + model.EmailId + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Mobile</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + model.Mobile + "</td></tr>";
                //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Arrive Time</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.ArriveTime + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Sub Department</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + model.SubDepartment + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Remarks </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + model.Remarks + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Same Rights like </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + model.SameRightsLikeempId + "</td></tr>";
                //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Contact No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.Mobile + "</td></tr>";
                mailbody += "</table>";
                mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
                mailmodel.body = mailbody;
                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "New User Request access";

                cm.SendEmail(mailmodel);
            }
            //if (TMobile != null && TMobile != "")
            //{
            //    if (AdminMobile != null && AdminMobile != "")
            //    {
            //        TMobile = TMobile + "," + AdminMobile;
            //    }
            //    SendSms smodel = new SendSms();
            //    smodel.SendSmsToEmployee(TMobile, "HI Team Vehicle Booking Request from " + curuser.FirstName + ". Id :" + mode.VehicleRequestId);
            //}
            return Json("New User request Successfully created", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFullRequest(int id)
        {
            var result = myapp.tbl_NewUserAccessRequest.Where(l => l.UserRequestId == id).SingleOrDefault();
            List<tbl_Location> locations = myapp.tbl_Location.ToList();

            var Approveloc = "";
            var Approve = result.ApprovedLocation != null ? result.ApprovedLocation.Split(',') : null;
            if (Approve != null)
            {
                for (int j = 0; j < Approve.Length; j++)
                {
                    if (Approve[j] != "")
                    {
                        var Loc = Convert.ToInt32(Approve[j]);
                        if (Loc != 0)
                        {
                            if (Approveloc == "")
                            {
                                Approveloc = locations.Where(l => l.LocationId == Loc).SingleOrDefault().LocationName;
                            }
                            else
                            {
                                Approveloc = Approveloc + "," + locations.Where(l => l.LocationId == Loc).SingleOrDefault().LocationName;
                            }
                        }
                    }
                }
                result.ApprovedLocation = Approveloc;
            }
            var username = myapp.tbl_User.Where(l => l.CustomUserId == result.EmpNo).SingleOrDefault();
            var hoddetails = myapp.tbl_User.Where(l => l.CustomUserId == result.HodApprovalId).SingleOrDefault();
            NewUserAccessRequestViewModel model = new NewUserAccessRequestViewModel();
            model.ApprovedLocation = result.ApprovedLocation;
            model.CreatedBy = result.CreatedBy;
            model.CreatedOn = result.CreatedOn;
            model.Department = username.DepartmentName;
            model.Designation = result.Designation;
            model.EmailId = result.EmailId;
            model.EmpDepartmentId = result.EmpDepartmentId;
            model.EmpLocationId = result.EmpLocationId;
            model.EmpNo = result.EmpNo;
            model.HodApprovalId = result.HodApprovalId;
            if (hoddetails != null)
                model.HodApprovalName = hoddetails.FirstName;
            model.HodApproveComments = result.HodApproveComments;
            model.IsActive = result.IsActive;
            model.IsHodApproved = result.IsHodApproved;
            model.IsSupportTeamCompleted = result.IsSupportTeamCompleted;
            model.Mobile = result.Mobile;
            model.ModifiedBy = result.ModifiedBy;
            model.ModifiedOn = result.ModifiedOn;
            model.Name = result.Name;
            model.PasswordProvided = result.PasswordProvided;
            model.Remarks = result.Remarks;
            model.RequestFor = result.RequestFor;
            model.RequestIs = result.RequestIs;
            model.SameRightsLikeempId = result.SameRightsLikeempId;
            model.SubDepartment = result.SubDepartment;
            model.SupportTeamComments = result.SupportTeamComments;
            model.UsernameProvided = result.UsernameProvided;
            model.UserRequestId = result.UserRequestId;

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEmployeeDetails(string empid, string reqtype)
        {
            int employeeid = 0;
            if (reqtype == "MySelf")
            {
                employeeid = int.Parse(User.Identity.Name.ToLower().Replace("fh_", ""));
            }
            else
            {
                employeeid = empid != "" ? int.Parse(empid.ToLower().Replace("fh_", "")) : 0;
                int employeeid2 = int.Parse(User.Identity.Name.ToLower().Replace("fh_", ""));
                if (employeeid2 == employeeid)
                {
                    return Json("", JsonRequestBehavior.AllowGet);
                }
            }
            tbl_User employee = myapp.tbl_User.Where(l => l.EmpId == employeeid).SingleOrDefault();
            var employeeDetails = new
            {
                employee.aadharCard,
                employee.PanCard,
                employee.CollegeName,
                employee.Comments,
                employee.CreatedBy,
                CreatedOn = employee.CreatedOn.HasValue ? employee.CreatedOn.Value.ToString("yyyy-MM-dd") : "",
                employee.CugNumber,
                employee.CustomUserId,
                DateOfBirth = employee.DateOfBirth.HasValue ? employee.DateOfBirth.Value.ToString("yyyy-MM-dd") : "",
                DateOfJoining = employee.DateOfJoining.HasValue ? employee.DateOfJoining.Value.ToString("yyyy-MM-dd") : "",
                employee.DateOfLeaving,
                employee.DepartmentId,
                employee.DepartmentId1,
                employee.DepartmentId2,
                employee.DepartmentName,
                employee.DepartmentName1,
                employee.DepartmentName2,
                employee.Designation,
                employee.DesignationID,
                employee.DigitalSignature,
                employee.EmailId,
                employee.EmpId,
                employee.EmployeePhoto,
                employee.Extenstion,
                employee.FatherSpouseName,
                employee.FirstName,
                employee.FoodType,
                employee.Gender,
                employee.IsActive,
                employee.IsAppLogin,
                employee.IsEmployee,
                employee.IsEmployeesReporting,
                employee.IsOffRollDoctor,
                employee.IsOnRollDoctor,
                employee.IsUserOnline,
                employee.LastName,
                employee.LocationId,
                employee.LocationName,
                employee.MaritalStatus,
                employee.PersonalEmail,
                employee.PhoneNumber,
                employee.PlaceAllocation,
                employee.Qualification,
                employee.ReportingManagerId,
                employee.SecurityAnswner,
                employee.SecurityQuestion,
                employee.SendRemainder,
                employee.SubDepartmentId,
                employee.SubDepartmentName,
                employee.UserId,
                employee.UserOnlineStatus,
                employee.UserType,
                employee.VendorId



            };
            return Json(employeeDetails, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ApproveNewUserRequest(int id, string ApproveLocation, string comments)
        {
            tbl_NewUserAccessRequest request = myapp.tbl_NewUserAccessRequest.Where(l => l.UserRequestId == id).SingleOrDefault();
            if (request != null)
            {
                request.IsHodApproved = true;
                request.ApprovedLocation = ApproveLocation;
                request.HodApproveComments = comments;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SupportApproveNewUserRequest(int id, string comments, string username, string password)
        {
            tbl_NewUserAccessRequest request = myapp.tbl_NewUserAccessRequest.Where(l => l.UserRequestId == id).SingleOrDefault();
            if (request != null)
            {
                request.ModifiedBy = User.Identity.Name;
                request.ModifiedOn = DateTime.Now;
                request.IsSupportTeamCompleted = true;
                request.SupportTeamComments = comments;
                request.UsernameProvided = username;
                request.PasswordProvided = password;
                myapp.SaveChanges();

                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = "Itsupport@fernandez.foundation";
                mailmodel.ccemail = "ahmadali@fernandez.foundation";
                if (request.EmailId != string.Empty)
                {
                    mailmodel.ccemail += "," + request.EmailId;
                }
                mailmodel.subject = "A New User Access Request " + request.UserRequestId + " has updated";
                string mailbody = "<p style='font-family:verdana'>Dear " + request.Name + ",";
                mailbody += "<p style='font-family:verdana'>you have has requested for new user access has updated. Please click on this <a target='_blank' href='https://infonet.fernandezhospital.com/'>link</a>  to see the request.Do not forget to update the request status after completion.</p>";
                mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Emp No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.EmpNo + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Emp Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.Name + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Designation </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.Designation + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Email Id</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.EmailId + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Mobile</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.Mobile + "</td></tr>";
                //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Arrive Time</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.ArriveTime + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Sub Department</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.SubDepartment + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Remarks </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.Remarks + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>HOD Comments </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.HodApproveComments + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>User Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.UsernameProvided + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Password </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.PasswordProvided + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Support Team Comments </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.SupportTeamComments + "</td></tr>";
                mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Same Rights like </td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + request.SameRightsLikeempId + "</td></tr>";
                //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Contact No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + mode.Mobile + "</td></tr>";
                mailbody += "</table>";
                mailbody += "<br/><p style='font-family:cambria'>This mail is auto generated. Please do not reply.</p>";
                mailmodel.body = mailbody;
                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "New User Request access";
                cm.SendEmail(mailmodel);
                if (request.Mobile != null && request.Mobile != "")
                {
                    SendSms smodel = new SendSms();
                    smodel.SendSmsToEmployee(request.Mobile, "Hi " + request.Name + " your new user request access updated username:" + request.UsernameProvided + ", password: " + request.PasswordProvided + ", Comments: " + request.SupportTeamComments);
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}