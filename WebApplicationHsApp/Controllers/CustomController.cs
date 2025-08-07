using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
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
    public class CustomController : Controller
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Custom
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GatePass(int id = 0)
        {

            var user = myapp.tbl_User.Where(m => m.CustomUserId == User.Identity.Name).FirstOrDefault();
            ViewBag.id = id;
            ViewBag.Name = user.FirstName + " " + user.LastName;
            ViewBag.EmpNo = user.UserId;
            ViewBag.Location = user.LocationId;
            ViewBag.Designation = user.Designation;
            ViewBag.Department = user.DepartmentName;
            return View();
        }

        public ActionResult CellPhoneRequest(int id = 0)
        {
            var user = myapp.tbl_User.Where(m => m.CustomUserId == User.Identity.Name).FirstOrDefault();
            ViewBag.id = id;
            ViewBag.Name = user.FirstName + " " + user.LastName;
            ViewBag.EmpNo = user.UserId;
            ViewBag.Designation = user.Designation;
            ViewBag.Department = user.DepartmentName;
            ViewBag.Location = user.LocationId;
            return View();
        }

        public ActionResult ManageGatePass()
        {
            return View();
        }
        public ActionResult ManageCellPhoneRequest()
        {
            return View();
        }
        public ActionResult ApprovalCellPhoneRequest()
        {
            return View();
        }
        public ActionResult CellPhoneRequestApproverMaster()
        {
            return View();
        }
        public ActionResult PrintMobilePhonePolicy(int id)
        {
            PrintMobileViewModel printmobileviewmodel = new PrintMobileViewModel();
            MobileAsset result = (from s in myapp.tbl_CellPhoneRequest
                                  join a in myapp.tbl_Asset_MobileAllotment on s.CellPhoneRequestId equals a.CellPhoneRequestId
                                  where s.CellPhoneRequestId == id
                                  select new MobileAsset
                                  { cellPhoneRequest = s, mobileAllotment = a }).FirstOrDefault();
            if (result != null)
            {
                var crestedUser = myapp.tbl_User.Where(m => m.CustomUserId == result.cellPhoneRequest.CreatedBy).FirstOrDefault();
                printmobileviewmodel.Accessories = result.mobileAllotment.Accessories;
                if (result.mobileAllotment.BrandId != null)
                    printmobileviewmodel.BrandName = myapp.tbl_AssetBrand.Where(m => m.AssetBrandId == result.mobileAllotment.BrandId).Select(n => n.Name).FirstOrDefault();
                printmobileviewmodel.AssetLabel = result.mobileAllotment.AssetLabel;
                printmobileviewmodel.AssetStatus = result.mobileAllotment.AssetStatus;
                printmobileviewmodel.AssignComments = result.mobileAllotment.AssignComments;
                printmobileviewmodel.AssignDate = result.mobileAllotment.AssignDate.HasValue ? result.mobileAllotment.AssignDate.Value.ToString("dd/MM/yyyy") : "";
                printmobileviewmodel.AssigntoEmpId = result.mobileAllotment.AssigntoEmpId;
                printmobileviewmodel.AssigntoEmpNmae = myapp.tbl_User.Where(m => m.EmpId == result.mobileAllotment.AssigntoEmpId).Select(n => n.FirstName + " " + n.LastName).FirstOrDefault();
                printmobileviewmodel.BatterySerialNo = result.mobileAllotment.BatterySerialNo;
                printmobileviewmodel.BrandId = result.mobileAllotment.BrandId;
                printmobileviewmodel.CellPhoneRequestId = result.mobileAllotment.CellPhoneRequestId;
                printmobileviewmodel.Department = result.cellPhoneRequest.Department;
                printmobileviewmodel.Description = result.mobileAllotment.Description;
                printmobileviewmodel.Designation = result.cellPhoneRequest.Designation;
                printmobileviewmodel.DOP = result.mobileAllotment.DOP.HasValue ? result.mobileAllotment.DOP.Value.ToString("dd/MM/yyyy") : "";
                printmobileviewmodel.EmpNo = result.cellPhoneRequest.EmpNo;
                printmobileviewmodel.Id = result.mobileAllotment.Id;
                printmobileviewmodel.IMEINO = result.mobileAllotment.IMEINO;
                if (result.mobileAllotment.IssuedByEmpId != null)
                    printmobileviewmodel.IssuedByEmpName = myapp.tbl_User.Where(m => m.EmpId == result.mobileAllotment.IssuedByEmpId).Select(n => n.FirstName + " " + n.LastName).FirstOrDefault();
                printmobileviewmodel.LocationId = result.cellPhoneRequest.LocationId;
                printmobileviewmodel.MobileNumber = result.mobileAllotment.MobileNumber;
                printmobileviewmodel.ModelId = result.mobileAllotment.ModelId;
                printmobileviewmodel.ModelName = myapp.tbl_AssetModel.Where(m => m.AssetModelId == result.mobileAllotment.ModelId).Select(n => n.Name).FirstOrDefault();
                printmobileviewmodel.MonthlyPlan = result.mobileAllotment.MonthlyPlan;
                printmobileviewmodel.Name = result.cellPhoneRequest.Name;
                printmobileviewmodel.Purpose = result.cellPhoneRequest.Purpose;
                printmobileviewmodel.RequestType = result.cellPhoneRequest.RequestType;
                printmobileviewmodel.ReturnReceivedBy = result.mobileAllotment.ReturnReceivedBy;
                printmobileviewmodel.ReturnReceivedComments = result.mobileAllotment.ReturnReceivedComments;
                if (result.mobileAllotment.ReturnReceivedBy != null)
                    printmobileviewmodel.ReturnReceivedName = myapp.tbl_User.Where(m => m.EmpId == result.mobileAllotment.ReturnReceivedBy).Select(n => n.FirstName + " " + n.LastName).FirstOrDefault();
                printmobileviewmodel.ReturnReceivedOn = result.mobileAllotment.ReturnReceivedOn.HasValue ? result.mobileAllotment.ReturnReceivedOn.Value.ToString("dd/MM/yyyy") : "";
                printmobileviewmodel.SerialNumber = result.mobileAllotment.SerialNumber;
                printmobileviewmodel.SIMNO = result.mobileAllotment.SIMNO;
                printmobileviewmodel.Status = result.cellPhoneRequest.Status;
                printmobileviewmodel.Subject = result.mobileAllotment.Subject;
                printmobileviewmodel.HODName = result.cellPhoneRequest.IsHODApproved == true ? myapp.tbl_User.Where(m => m.EmpId == crestedUser.ReportingManagerId).Select(n => n.FirstName + " " + n.LastName).FirstOrDefault() : "";
                printmobileviewmodel.HODApprovedOn = result.cellPhoneRequest.HODApprovedOn.HasValue ? result.cellPhoneRequest.HODApprovedOn.Value.ToString("dd/MM/yyyy") : "";
                var adminId = myapp.tbl_CellPhoneRequestApprover.Where(n => n.LocationId == result.cellPhoneRequest.LocationId).Select(n => n.ApproverId).FirstOrDefault();
                printmobileviewmodel.AdminName = result.cellPhoneRequest.IsAdministratorApproved == true ? myapp.tbl_User.Where(m => m.EmpId == adminId).Select(n => n.FirstName + " " + n.LastName).FirstOrDefault() : "";
                printmobileviewmodel.AdminApprovedOn = result.cellPhoneRequest.AdminApprovedOn.HasValue ? result.cellPhoneRequest.AdminApprovedOn.Value.ToString("dd/MM/yyyy") : "";
                printmobileviewmodel.ITUpdatedBy = result.cellPhoneRequest.IsAdministratorApproved == true ? myapp.tbl_User.Where(m => m.EmpId == result.cellPhoneRequest.ITUpdatedBy).Select(n => n.FirstName + " " + n.LastName).FirstOrDefault() : "";
                printmobileviewmodel.ITUpdatedOn = result.cellPhoneRequest.ITUpdatedOn.HasValue ? result.cellPhoneRequest.ITUpdatedOn.Value.ToString("dd/MM/yyyy") : "";
                printmobileviewmodel.accpected = result.cellPhoneRequest.Status == "Completed" ? crestedUser.FirstName + " " + crestedUser.LastName : "";
            }
            return View(printmobileviewmodel);
        }
        public JsonResult GetCellPhoneApproverById(int id)
        {
            var viewmode = myapp.tbl_CellPhoneRequestApprover.Where(m => m.Id == id).FirstOrDefault();
            var inChargeEmp = (from V in myapp.tbl_User where V.UserId == viewmode.ApproverId select V.FirstName + " " + V.LastName + " - " + V.Designation + " - " + V.DepartmentName).SingleOrDefault();
            object obj = new { model = viewmode, inChargeEmp = inChargeEmp };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save_UpdateCellPhoneApprover(tbl_CellPhoneRequestApprover model)
        {
            tbl_CellPhoneRequestApprover _Category = new tbl_CellPhoneRequestApprover();
            if (model.Id != 0)
            {
                var id = model.Id;
                _Category = myapp.tbl_CellPhoneRequestApprover.Where(m => m.Id == id).FirstOrDefault();
            }
            _Category.LocationId = model.LocationId;

            var tbluser = myapp.tbl_User.Where(l => l.UserId == model.ApproverId).SingleOrDefault();
            if (tbluser != null)
            {
                _Category.ApproverId = tbluser.EmpId;
            }
            if (model.Id == 0)
            {
                _Category.CreatedBy = User.Identity.Name;
                _Category.CreatedOn = DateTime.Now;
                myapp.tbl_CellPhoneRequestApprover.Add(_Category);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCellPhoneApprover(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_CellPhoneRequestApprover select d).ToList();
            IEnumerable<tbl_CellPhoneRequestApprover> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                              c.ApproverId != null && c.ApproverId.ToString().ToLower().Contains(param.sSearch.ToLower())
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

                                                (from V in myapp.tbl_Location where V.LocationId == c.LocationId select V.LocationName).SingleOrDefault(),

                                              (from V in myapp.tbl_User where V.EmpId == c.ApproverId select V.FirstName+" "+V.LastName).SingleOrDefault(),
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCellPhoneApprover(int id)
        {
            var cat = myapp.tbl_CellPhoneRequestApprover.Where(l => l.Id == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_CellPhoneRequestApprover.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCellPhoneRequest(JQueryDataTableParamModel param)
        {
            int currentemp = int.Parse(User.Identity.Name);
            var tbluser = myapp.tbl_User.Where(l => l.EmpId == currentemp).SingleOrDefault();
            List<tbl_CellPhoneRequest> query = (from d in myapp.tbl_CellPhoneRequest select d).Where(m => m.IsActive == true).ToList();
            if (tbluser.DepartmentName == "Information Technology" || tbluser.DepartmentName == "IT")
            {
                query = query.ToList();
            }
            else
            {
                query = query.Where(l => l.CreatedBy == User.Identity.Name || l.CurrentApprover == User.Identity.Name).ToList();
            }
            query = query.OrderByDescending(m => m.CellPhoneRequestId).ToList();
            if (param.locationid != null && param.locationid != 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.departmentid != null && param.departmentid != 0)
            {
                var department = myapp.tbl_Department.Where(m => m.DepartmentId == param.departmentid).Select(n => n.DepartmentName).FirstOrDefault();
                query = query.Where(l => l.Department == department).ToList();
            }

            if (param.Emp != null && param.Emp != "")
            {
                var id = Convert.ToInt32(param.Emp);
                query = query.Where(l => l.EmpNo == id).ToList();
            }
            if (param.status != null && param.status != "")
            {
                query = query.Where(l => l.Status == param.status).ToList();
            }
            if (param.typeofitem != null && param.typeofitem != "")
            {
                query = query.Where(l => l.RequestType == param.typeofitem).ToList();
            }
            IEnumerable<tbl_CellPhoneRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.CellPhoneRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.EmpNo != null && c.EmpNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.Department != null && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_CellPhoneRequest> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies

                                           select new object[] {
                                              c.CellPhoneRequestId.ToString(),
                                              c.CreatedOn.HasValue?c.CreatedOn.Value.ToString("dd/MM/yyyy"):"",
                                              c.EmpNo.ToString(),c.Name,c.Designation,c.Department
                                             ,c.Purpose,c.RequestType, c.NewOrRepair,
                                               c.CurrentApprover!="0"?myapp.tbl_User.Where(l=>l.CustomUserId==c.CurrentApprover).SingleOrDefault().FirstName:"",

                                               c.Status,
                                            c.CellPhoneRequestId.ToString()+"-"+((c.Status=="IT Updated" || c.Status=="Completed")?"yes":"No")
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetCellPhoneRequestforApproval(JQueryDataTableParamModel param)
        {
            List<tbl_CellPhoneRequest> query = (from d in myapp.tbl_CellPhoneRequest select d).Where(m => m.IsActive == true).ToList();
            string empid = User.Identity.Name;
            query = query.Where(l => l.CurrentApprover == empid).OrderByDescending(m => m.CellPhoneRequestId).ToList();
            //query = query.Where(m => m.Status != "").ToList();
            IEnumerable<tbl_CellPhoneRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.CellPhoneRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.EmpNo != null && c.EmpNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.Department != null && c.Department.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_CellPhoneRequest> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies

                                           select new object[] {
                                              c.CellPhoneRequestId.ToString(),
                                              c.EmpNo.ToString(),c.Name,c.Designation,c.Department
                                             ,c.Purpose,c.CurrentApprover,c.Status,
                                            c.CellPhoneRequestId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save_Update_CellPhoneRequest(tbl_CellPhoneRequest model)
        {
            tbl_CellPhoneRequest request = new tbl_CellPhoneRequest();
            if (model.CellPhoneRequestId != 0)
            {
                request = myapp.tbl_CellPhoneRequest.Where(m => m.CellPhoneRequestId == model.CellPhoneRequestId).FirstOrDefault();
                request.ModifiedBy = User.Identity.Name;
                request.ModifiedOn = DateTime.Now;
            }
            request.LocationId = model.LocationId;
            request.Status = "Pending for HOD Approval";
            var tbluser = myapp.tbl_User.Where(l => l.UserId == model.EmpNo).SingleOrDefault();
            if (tbluser != null)
            {
                request.EmpNo = tbluser.EmpId;


                int currentApprover = tbluser.ReportingManagerId.HasValue ? tbluser.ReportingManagerId.Value : 0;
                request.CurrentApprover = currentApprover.ToString();

            }
            request.Name = model.Name;
            request.IsHODApproved = false;
            request.IsAdministratorApproved = false;
            request.IsActive = true;
            request.Department = model.Department;
            request.Designation = model.Designation;
            request.Purpose = model.Purpose;
            request.RequestType = model.RequestType;
            request.NewOrRepair = model.NewOrRepair;
            if (model.CellPhoneRequestId == 0)
            {
                request.CreatedBy = User.Identity.Name;
                request.CreatedOn = DateTime.Now;
                myapp.tbl_CellPhoneRequest.Add(request);
            }
            try
            {
                myapp.SaveChanges();
            }
            catch (Exception ex)
            {
            }
            SendEmailtoHOD(request);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public void SendEmailtoHOD(tbl_CellPhoneRequest model)
        {
            string body = string.Empty;
            var mealTable = "<table style='border:1px solid #eee;width: 60%;'>";
            mealTable += "<tr><td style='border:1px solid #eee;'>Request Id</td><td style='border:1px solid #eee;'>" + model.CellPhoneRequestId + "</td></tr>";
            mealTable += "<tr><td style='border:1px solid #eee;'>Employee Name</td><td style='border:1px solid #eee;'>" + model.Name + "</td></tr>";
            mealTable += "<tr><td style='border:1px solid #eee;'>Designation</td><td style='border:1px solid #eee;'>" + model.Designation + "</td></tr>";
            mealTable += "<tr><td style='border:1px solid #eee;'>Emp Id</td><td style='border:1px solid #eee;'>" + model.EmpNo + "</td></tr>";
            mealTable += "<tr><td style='border:1px solid #eee;'>Department</td><td style='border:1px solid #eee;'>" + model.Department + "</td></tr>";
            mealTable += "<tr><td style='border:1px solid #eee;'>Purpose</td><td style='border:1px solid #eee;'>" + model.Purpose + "</td></tr>";

            string Subject = "";
            Subject = "CUG Request from " + model.Name + "";
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel
            {
                fromemail = "Leave@hospitals.com",
                toemail = "",
                subject = Subject,
                body = mealTable,
                filepath = "",
                fromname = "",
                ccemail = "ahmadali@fernandez.foundation,IT@fernandez.foundation"
            };
            if (model.CurrentApprover != null && model.CurrentApprover != "")
            {
                var id = int.Parse(model.CurrentApprover);
                mealTable += "<tr><td style='border:1px solid #eee;'>Approve</td><td style='border:1px solid #eee;'><a href='https://infonet.fernandezhospital.com/Custom/Approval_Reject_CellPhoneRequest?id=" + model.CellPhoneRequestId + "&status=Approval&comments=ok&type=" + model.RequestType + "&userid=" + id + "'>Click here</a></td></tr>";
                mealTable += "<tr><td style='border:1px solid #eee;'>Reject</td><td style='border:1px solid #eee;'><a href='https://infonet.fernandezhospital.com/Custom/Approval_Reject_CellPhoneRequest?id=" + model.CellPhoneRequestId + "&status=Reject&comments=ok&type=" + model.RequestType + "&userid=" + id + "'>Click here</a></td></tr>";
                mealTable += "</table>";
                var HodEmailId = myapp.tbl_User.Where(m => m.EmpId == id).FirstOrDefault();
                if (HodEmailId != null)
                {
                    mailmodel.toemail = HodEmailId.EmailId;
                    body = body + "Dear " + HodEmailId.FirstName + " , <br/> Please find below are the CUG Request details waiting for your approval. ";
                    body = body + mealTable;
                    mailmodel.body = body;
                    cm.SendEmail(mailmodel);
                }
            }
            else
            {
                mealTable += "</table>";
                mailmodel.toemail = "ahmadali@fernandez.foundation";
                mailmodel.ccemail = "IT@fernandez.foundation";
                body = body + "Dear Team, <br/> Please find below are the CUG Request details waiting for your approval. ";
                body = body + mealTable;
                mailmodel.body = body;
                cm.SendEmail(mailmodel);
            }
        }
        [AllowAnonymous]
        public JsonResult Approval_Reject_CellPhoneRequest(int id, string status, string comments, string type, string userid = "")
        {
            tbl_CellPhoneRequest request = new tbl_CellPhoneRequest();
            request = myapp.tbl_CellPhoneRequest.Where(m => m.CellPhoneRequestId == id).FirstOrDefault();
            string currentuser = User.Identity.Name;
            if (userid != null && userid != "")
            {
                currentuser = userid;
            }
            if (request.CurrentApprover == currentuser)
            {
                request.ModifiedBy = currentuser;
                request.ModifiedOn = DateTime.Now;
                request.ApprovedRequestType = type;
                if (request.IsAdministratorApproved == false && request.IsHODApproved == false)
                {
                    request.Status = "Pending for Admin Approval";
                    int approverid = int.Parse(currentuser);
                    var approve = myapp.tbl_CellPhoneRequestApprover.Where(l => l.LocationId == request.LocationId && l.ApproverId != approverid).ToList();

                    request.IsHODApproved = status == "Approval" ? true : false;
                    request.HODComments = comments;
                    request.HODApprovedOn = DateTime.Now;
                    if (approve.Count > 0)
                    {
                        request.CurrentApprover = approve[0].ApproverId.HasValue ? approve[0].ApproverId.Value.ToString() : "";
                    }
                    else
                    {
                        request.Status = "Admin Approved";
                        request.CurrentApprover = "0";
                        request.IsAdministratorApproved = status == "Approval" ? true : false;
                        request.AdministratorComments = comments;
                        request.AdminApprovedOn = DateTime.Now;
                    }
                }
                else if (request.IsAdministratorApproved == false && request.IsHODApproved == true)
                {
                    request.Status = "Admin Approved";
                    request.CurrentApprover = "0";
                    request.IsAdministratorApproved = status == "Approval" ? true : false;
                    request.AdministratorComments = comments;
                    request.AdminApprovedOn = DateTime.Now;
                }
                try
                {
                    myapp.SaveChanges();
                }
                catch (Exception ex)
                {
                }
                if (request.IsAdministratorApproved == false)
                    SendEmailtoHOD(request);
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("You are not authorized to approve this request", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetCellPhoneRequest(int id)
        {
            var dbModel = myapp.tbl_CellPhoneRequest.Where(m => m.CellPhoneRequestId == id && m.IsActive == true).FirstOrDefault();
            if (dbModel == null)
            {
                return Json("Enter Valid Request ID", JsonRequestBehavior.AllowGet);
            }
            return Json(dbModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCellPhoneRequestforView(int id)
        {
            var dbModel = myapp.tbl_CellPhoneRequest.Where(m => m.CellPhoneRequestId == id && m.IsActive == true).FirstOrDefault();
            var asset = myapp.tbl_Asset_MobileAllotment.Where(m => m.CellPhoneRequestId == id).ToList();
            if (dbModel == null)
            {
                return Json("Enter Valid Request ID", JsonRequestBehavior.AllowGet);
            }
            var obj = new MobileAsset { cellPhoneRequest = dbModel, mobileAllotmentList = asset };

            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUserDetails(int id)
        {
            var dbModel = myapp.tbl_User.Where(m => m.UserId == id).FirstOrDefault();
            if (dbModel == null)
            {
                return Json("Enter Valid Request ID", JsonRequestBehavior.AllowGet);
            }
            return Json(dbModel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetUserDetailsByEmpId(int id)
        {
            var dbModel = myapp.tbl_User.Where(m => m.EmpId == id).FirstOrDefault();
            if (dbModel == null)
            {
                var dbModel2 = myapp.tbl_OutSourceUser.Where(m => m.EmpId == id).FirstOrDefault();
                if (dbModel2 == null)
                {
                    return Json("Enter Valid Request ID", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(dbModel2, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(dbModel, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult AcceptCellPhoneRequest(int id)
        {
            var cat = myapp.tbl_CellPhoneRequest.Where(l => l.CellPhoneRequestId == id).ToList();
            if (cat.Count > 0)
            {
                int empno = int.Parse(User.Identity.Name);
                if (cat[0].EmpNo == empno)
                {
                    cat[0].Status = "Completed";
                    myapp.SaveChanges();
                }
                else
                {
                    return Json("You dont have access to accept the request", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCellPhoneRequest(int id)
        {
            var cat = myapp.tbl_CellPhoneRequest.Where(l => l.CellPhoneRequestId == id).ToList();
            if (cat.Count > 0)
            {
                int empno = int.Parse(User.Identity.Name);
                if (cat[0].EmpNo == empno)
                {
                    cat[0].IsActive = false;
                    myapp.tbl_CellPhoneRequest.Add(cat[0]);
                    myapp.SaveChanges();
                }
                else
                {
                    return Json("You dont have access to delete the request", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetGatePass(JQueryDataTableParamModel param)
        {

            List<tbl_GatePass> query = (from d in myapp.tbl_GatePass select d).ToList();
            string currentuser = User.Identity.Name;
            string intolocdept = GatePassWorkflow.ToLocationDepartmentIn.ToString();
            var tbluser = myapp.tbl_User.Where(l => l.CustomUserId == currentuser).SingleOrDefault();
            if (tbluser != null)
            {
                query = query.Where(l => (l.CreatedBy == currentuser || l.AuthorizedByDepartment == tbluser.DepartmentName)
                || (l.ToDepartmentId == tbluser.DepartmentId && l.CurrentWorkFlow == intolocdept)).ToList();
            }
            query = query.OrderByDescending(m => m.GatePassId).ToList();
            IEnumerable<tbl_GatePass> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.GatePassId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.NameOfTheCompany != null && c.NameOfTheCompany.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_GatePass> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                                           select new object[] {
                                               c.GatePassId.ToString(),
                                             (from V in myapp.tbl_Location where V.LocationId == c.LocationId select V.LocationName).SingleOrDefault(),
            c.ToLocation,
                                               c.Date.Value.ToString("dd/MM/yyyy"),
                                               c.Address,
                                               c.AuthorizedByName +" "+c.AuthorizedByEmpId
                                               ,c.ReceivingCompanyName,c.Comments,c.GatePassType,c.Status,c.CurrentWorkFlow,
                                           u.FirstName
                                             ,c.AuthorizedByDepartment,
                                            c.GatePassType+"^"+c.Status+"^"+c.GatePassId.ToString()+"^"+c.CurrentWorkFlow
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Save_Update_GatePass(GatePassViewModel model)
        {
            tbl_GatePass request = new tbl_GatePass();
            if (model.GatePassId != 0)
            {
                request = myapp.tbl_GatePass.Where(m => m.GatePassId == model.GatePassId).FirstOrDefault();
                request.ModifiedBy = User.Identity.Name;
                request.ModifiedOn = DateTime.Now;
            }
            request.ToDepartmentId = model.ToDepartmentId;
            request.ToLocation = model.ToLocation;
            request.ToLocationCmpCPEmail = model.ToLocationCmpCPEmail;
            request.ToLocationCmpCPMobile = model.ToLocationCmpCPMobile;
            request.ToLocationCmpCPName = model.ToLocationCmpCPName;
            request.CurrentWorkFlow = GatePassWorkflow.HodApproval.ToString();
            request.Status = "New";
            request.GatePassType = model.GatePassType;
            request.Address = model.Address;
            request.AuthorizedByDepartment = model.AuthorizedByDepartment;
            request.IsActive = true;
            request.AuthorizedByDesignation = model.AuthorizedByDesignation;
            request.AuthorizedByEmpId = model.AuthorizedByEmpId;
            request.AuthorizedByName = model.AuthorizedByName;
            request.LocationId = model.LocationId;
            request.NameOfTheCompany = model.NameOfTheCompany;
            request.ReceivingCompanyName = model.ReceivingCompanyName;
            request.Comments = model.Comments;
            request.Date = DateTime.Now;
            if (model.ExpectedDateOfReturn != null && model.ExpectedDateOfReturn != "")
            {
                request.ExpectedDateOfReturn = ProjectConvert.ConverDateStringtoDatetime(model.ExpectedDateOfReturn);
            }
            request.ApproverComments = "Pending for Approval";
            request.ReceivingCompanyName = model.ReceivingCompanyName;
            var tbluser = myapp.tbl_User.Where(l => l.UserId == request.AuthorizedByEmpId).SingleOrDefault();
            if (tbluser != null)
            {
                request.AuthorizedByEmpId = tbluser.EmpId;
            }
            if (model.GatePassId == 0)
            {
                request.CreatedBy = User.Identity.Name;
                request.CreatedOn = DateTime.Now;
                myapp.tbl_GatePass.Add(request);
            }
            try
            {
                myapp.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            return Json(request.GatePassId, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save_UpdateGatePassAsset(List<tbl_GatePassAsset> model)

        {
            var id = model[0].GatePassId;
            var obj = myapp.tbl_GatePassAsset.Where(m => m.GatePassId == id).ToList();
            myapp.tbl_GatePassAsset.RemoveRange(obj);
            myapp.SaveChanges();
            for (int i = 0; i < model.Count; i++)
            {
                model[i].IsActive = true;
            }
            myapp.tbl_GatePassAsset.AddRange(model);
            myapp.SaveChanges();
            SendEmailGatPass(id);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public void SendEmailGatPass(int? id)
        {
            var gatePass = myapp.tbl_GatePass.Where(l => l.GatePassId == id).FirstOrDefault();
            var gatePassAsset = myapp.tbl_GatePassAsset.Where(n => n.GatePassId == id).ToList();
            var loginUserId = User.Identity.Name;
            var gatepassAuthId = myapp.tbl_User.Where(n => n.EmpId == gatePass.AuthorizedByEmpId).FirstOrDefault();
            if (gatepassAuthId.CustomUserId != loginUserId)
            {

                string Subject = "Gate Pass Details";
                string body = "";
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/BioMedical_NewShiftRequest.html")))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("[[Heading]]", "Gate Pass Details");
                var div = "<div><label>Gate Pass Details</label>";

                var table = "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Id:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == gatePass.LocationId select var.LocationName).SingleOrDefault() + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Name of the Company:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + gatePass.NameOfTheCompany + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Date:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + gatePass.Date.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Receiving Company Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + gatePass.ReceivingCompanyName + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>GatePass Type:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + gatePass.GatePassType + "</td></tr>";
                if (gatePass.GatePassType == "Return")
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Excepted Date:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + gatePass.ExpectedDateOfReturn.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Address:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + gatePass.Address + "</td></tr>";
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Comments:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + gatePass.Comments + "</td></tr>";



                table += "</tbody></table>";
                div += table;
                div += "</div><div><label>Asset Details</label>";
                table = "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tr><th>Asset Label</th><th>Quantity</th><th>Reason</th><th>Description</th></tr><tbody>";
                foreach (var item in gatePassAsset)
                {
                    table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + item.AssetLabel + "</td></tr>";
                    table += "<td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + item.Quantity + "</td>";
                    table += "<td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + item.Reason + "</td>";
                    table += "<td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + item.Description + "</td></tr>";

                }



                table += "</tbody></table>";
                div += table;
                div += "</div>";
                body = body.Replace("[[table]]", div);
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel
                {
                    fromemail = "Leave@hospitals.com",
                    toemail = gatepassAuthId.EmailId,
                    subject = Subject,
                    body = body,
                    filepath = "",
                    fromname = "",
                    ccemail = "ahmadali@fernandez.foundation"
                };
                cm.SendEmail(mailmodel);
            }
        }
        public JsonResult Get_GatePass_ById(int id)
        {
            var model = myapp.tbl_GatePass.Where(m => m.GatePassId == id).FirstOrDefault();
            GatePassViewModel request = new GatePassViewModel();
            request.GatePassType = model.GatePassType;
            request.Address = model.Address;
            request.AuthorizedByDepartment = model.AuthorizedByDepartment;

            request.IsActive = true;
            request.AuthorizedByDesignation = model.AuthorizedByDesignation;
            request.AuthorizedByEmpId = model.AuthorizedByEmpId.HasValue ? model.AuthorizedByEmpId.Value : 0;
            request.AuthorizedByName = model.AuthorizedByName;
            request.LocationId = model.LocationId.HasValue ? model.LocationId.Value : 0;
            request.NameOfTheCompany = model.NameOfTheCompany;
            request.ReceivingCompanyName = model.ReceivingCompanyName;
            request.Comments = model.Comments;
            request.Date = model.Date.Value.ToString("dd-MM-yyyy");
            request.ExpectedDateOfReturn = model.ExpectedDateOfReturn.Value.ToString("dd-MM-yyyy");
            request.ReceivingCompanyName = model.ReceivingCompanyName;
            request.assets = myapp.tbl_GatePassAsset.Where(m => m.GatePassId == id).ToList();
            return Json(request, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Get_GatePassAsset_ByGatePassId(int id)
        {
            var viewmode = myapp.tbl_GatePassAsset.Where(m => m.GatePassId == id).ToList();
            return Json(viewmode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLocations()
        {
            var viewmode = myapp.tbl_locationlist.ToList();
            return Json(viewmode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGatePassDetails(int id)
        {
            var gpass = myapp.tbl_GatePass.Where(l => l.GatePassId == id).SingleOrDefault();
            var assetsdetails = myapp.tbl_GatePassAsset.Where(l => l.GatePassId == id).ToList();
            var model = new GatePassViewModel
            {
                Address = gpass.Address,
                AuthorizedByDepartment = gpass.AuthorizedByDepartment,
                AuthorizedByDesignation = gpass.AuthorizedByDesignation,
                AuthorizedByEmpId = gpass.AuthorizedByEmpId.HasValue ? gpass.AuthorizedByEmpId.Value : 0,
                AuthorizedByName = gpass.AuthorizedByName,
                Comments = gpass.Comments,
                CreatedBy = gpass.CreatedBy,
                CreatedOn = gpass.CreatedOn.HasValue ? gpass.CreatedOn.Value : DateTime.Now,
                CurrentWorkFlow = gpass.CurrentWorkFlow,
                Date = gpass.Date.HasValue ? ProjectConvert.ConverDateTimeToString(gpass.Date.Value) : "",
                ExpectedDateOfReturn = gpass.ExpectedDateOfReturn.HasValue ? ProjectConvert.ConverDateTimeToString(gpass.ExpectedDateOfReturn.Value) : "",
                GatePassId = gpass.GatePassId,
                GatePassType = gpass.GatePassType,
                IsActive = gpass.IsActive.HasValue ? gpass.IsActive.Value : false,
                LocationId = gpass.LocationId.HasValue ? gpass.LocationId.Value : 0,
                ModifiedBy = gpass.ModifiedBy,
                ModifiedOn = gpass.ModifiedOn.HasValue ? gpass.ModifiedOn.Value : DateTime.Now,
                NameOfTheCompany = gpass.NameOfTheCompany,
                ReceivingCompanyName = gpass.ReceivingCompanyName,
                Status = gpass.Status,
                ToDepartmentId = gpass.ToDepartmentId.HasValue ? gpass.ToDepartmentId.Value : 0,
                ToLocation = gpass.ToLocation,
                ToLocationCmpCPEmail = gpass.ToLocationCmpCPEmail,
                ToLocationCmpCPMobile = gpass.ToLocationCmpCPMobile,
                ToLocationCmpCPName = gpass.ToLocationCmpCPName,
                assets = assetsdetails
            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteGatePass(int id)
        {
            var cat = myapp.tbl_GatePass.Where(l => l.GatePassId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_GatePass.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ApproveRejectGatePass(int id, string status, string comments)
        {
            var cat = myapp.tbl_GatePass.Where(l => l.GatePassId == id).ToList();
            if (cat.Count > 0)
            {
                string workflow = cat[0].CurrentWorkFlow;
                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
                cat[0].ApprovedByEmpId = Convert.ToInt32(User.Identity.Name);
                cat[0].ApproverStatus = status;
                if (status == "Approve")
                {
                    cat[0].Status = "Approved";
                    cat[0].CurrentWorkFlow = GatePassWorkflow.FromLocationSecurityOut.ToString();
                }
                else
                {
                    cat[0].Status = "Rejected";
                }
                cat[0].ApproverComments = comments;
                myapp.SaveChanges();
                tbl_GatePassComment gc = new tbl_GatePassComment
                {

                    CommentedBy = User.Identity.Name,
                    CommentedOn = DateTime.Now,
                    Comments = comments,
                    CurrentWorkFlow = workflow,
                    GatePassId = id,
                    Status = status
                };
                myapp.tbl_GatePassComment.Add(gc);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateGatePassDispatch(int id)
        {
            var cat = myapp.tbl_GatePass.Where(l => l.GatePassId == id).ToList();
            if (cat.Count > 0)
            {
                string tolocdept = GatePassWorkflow.ToLocationDepartmentIn.ToString();
                if (cat[0].CurrentWorkFlow == tolocdept)
                {
                    cat[0].CurrentWorkFlow = GatePassWorkflow.ToLocationSecurityOut.ToString();
                    myapp.SaveChanges();

                    tbl_GatePassComment gc = new tbl_GatePassComment
                    {

                        CommentedBy = User.Identity.Name,
                        CommentedOn = DateTime.Now,
                        Comments = "Dispatch From Department",
                        CurrentWorkFlow = tolocdept,
                        GatePassId = id,
                        Status = "Dispatch From Department"
                    };
                    myapp.tbl_GatePassComment.Add(gc);
                    myapp.SaveChanges();
                }
                else
                {
                    return Json("Fail", JsonRequestBehavior.AllowGet);
                }

            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateGatePassFinalConfirmation(int id, string comments)
        {
            var cat = myapp.tbl_GatePass.Where(l => l.GatePassId == id).ToList();
            if (cat.Count > 0)
            {
                if (cat[0].GatePassType == "Return")
                {
                    if (cat[0].CurrentWorkFlow == GatePassWorkflow.FromLocationDepartmentIn.ToString())
                    {
                        cat[0].CurrentWorkFlow = GatePassWorkflow.FromLocationDepartmentIn.ToString();
                        cat[0].Status = "Completed";
                        myapp.SaveChanges();
                        tbl_GatePassComment gc = new tbl_GatePassComment
                        {
                            CommentedBy = User.Identity.Name,
                            CommentedOn = DateTime.Now,
                            Comments = comments,
                            CurrentWorkFlow = GatePassWorkflow.FromLocationDepartmentIn.ToString(),
                            GatePassId = id,
                            Status = "Completed"
                        };
                        myapp.tbl_GatePassComment.Add(gc);
                        myapp.SaveChanges();
                    }
                    else
                    {

                        return Json("This action can n't perfrom now. Only Once Item return back to department", JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    if (cat[0].CurrentWorkFlow == GatePassWorkflow.ToLocationDepartmentIn.ToString())
                    {
                        cat[0].CurrentWorkFlow = GatePassWorkflow.ToLocationDepartmentIn.ToString();
                        cat[0].Status = "Completed";
                        myapp.SaveChanges();
                        tbl_GatePassComment gc = new tbl_GatePassComment
                        {
                            CommentedBy = User.Identity.Name,
                            CommentedOn = DateTime.Now,
                            Comments = comments,
                            CurrentWorkFlow = GatePassWorkflow.ToLocationDepartmentIn.ToString(),
                            GatePassId = id,
                            Status = "Completed"
                        };
                        myapp.tbl_GatePassComment.Add(gc);
                        myapp.SaveChanges();
                    }
                    else
                    {

                        return Json("This action can n't perfrom now. Only Once Item return back to department", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetGatePassComments(int id)
        {
            var list = (from g in myapp.tbl_GatePassComment where g.GatePassId == id select g).ToList();
            var model = (from l in list
                         join u in myapp.tbl_User on l.CommentedBy equals u.CustomUserId
                         let CommentedOn = l.CommentedOn.HasValue ? l.CommentedOn.Value.ToString("dd/MM/yyyy") : ""
                         let Name = u.FirstName
                         select new
                         {
                             l.GatePassId,
                             l.Id,
                             l.Status,
                             l.CurrentWorkFlow,
                             CommentedOn,
                             Name,
                             l.Comments
                         }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SecurityAcceptGatePass(int id, string status, string comments)
        {
            var cat = myapp.tbl_GatePass.Where(l => l.GatePassId == id).ToList();
            if (cat.Count > 0)
            {
                tbl_GatePassComment gc = new tbl_GatePassComment
                {

                    CommentedBy = User.Identity.Name,
                    CommentedOn = DateTime.Now,
                    Comments = comments,
                    CurrentWorkFlow = cat[0].CurrentWorkFlow,
                    GatePassId = id,
                    Status = status
                };
                myapp.tbl_GatePassComment.Add(gc);
                myapp.SaveChanges();
                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
                cat[0].SecurityAcceptedId = Convert.ToInt32(User.Identity.Name);
                cat[0].Status = status;
                if (status == "Accepted")
                {
                    switch (cat[0].CurrentWorkFlow)
                    {

                        case nameof(GatePassWorkflow.FromLocationSecurityOut):
                            cat[0].CurrentWorkFlow = GatePassWorkflow.ToLocationSecurityIn.ToString();

                            if (cat[0].ToLocation == "Other" && cat[0].GatePassType != "Return")
                            {
                                cat[0].Status = "Completed";
                            }
                            break;
                        case nameof(GatePassWorkflow.ToLocationSecurityIn):
                            cat[0].CurrentWorkFlow = GatePassWorkflow.ToLocationDepartmentIn.ToString();
                            break;
                        case nameof(GatePassWorkflow.ToLocationSecurityOut):
                            cat[0].CurrentWorkFlow = GatePassWorkflow.FromLocationSecurityIn.ToString();
                            break;
                        case nameof(GatePassWorkflow.FromLocationSecurityIn):
                            cat[0].CurrentWorkFlow = GatePassWorkflow.FromLocationDepartmentIn.ToString();
                            break;
                    }
                }
                cat[0].SecurityComments = comments;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateGatePassStatus(int id, string status)
        {
            var cat = myapp.tbl_GatePass.Where(l => l.GatePassId == id).ToList();
            if (cat.Count > 0)
            {
                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;

                cat[0].Status = status;


                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult Assgin_Mobile_CellPhoneRequest(int id, string[] mobileId, string comments)
        {
            var list = mobileId[0].Split(',').ToList();
            var cellrequest = myapp.tbl_CellPhoneRequest.Where(n => n.CellPhoneRequestId == id).FirstOrDefault();
            foreach (var item in list)
            {
                var ids = Convert.ToInt32(item);
                var cat = myapp.tbl_Asset_MobileAllotment.Where(l => l.Id == ids).ToList();
                if (cat.Count > 0)
                {
                    cat[0].ModifiedBy = User.Identity.Name;
                    cat[0].ModifiedOn = DateTime.Now;
                    cat[0].AssignDate = DateTime.Now;
                    cat[0].AssigntoEmpId = cellrequest.EmpNo;
                    cat[0].AssignComments = comments;
                    cat[0].CellPhoneRequestId = id;
                    cat[0].AssetStatus = "InUse";
                    cellrequest.Status = "IT Updated";
                    cellrequest.ITUpdatedBy = Convert.ToInt32(User.Identity.Name);
                    //cellrequest.ITUpdatedOn = DateTime.Now;
                    myapp.SaveChanges();
                }
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageValetParking()
        {
            return View();
        }
        public ActionResult ManageDailyVisiting()
        {
            return View();
        }
        public ActionResult NewDailyVisiting()
        {
            return View();
        }
        public ActionResult SaveValetParking(tbl_ValetParking model)
        {
            model.CreatedBy = User.Identity.Name;
            //var user = myapp.tbl_User.Where(l => l.CustomUserId == model.CreatedBy).SingleOrDefault();
            //if (user != null)
            //{
            //    model.LocationId = user.LocationId;
            //}
            model.InTime = DateTime.Now.ToString("hh:mm tt");
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            //model.ModifiedOn = DateTime.Now;
            myapp.tbl_ValetParking.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateValetParking(tbl_ValetParking model)
        {
            var query = myapp.tbl_ValetParking.Where(l => l.ValetParkingId == model.ValetParkingId).SingleOrDefault();
            if (query != null)
            {
                query.ModifiedOn = DateTime.Now;
                query.OutTime = DateTime.Now.ToString("hh:mm tt");
                query.OutDriverName = model.OutDriverName;
                query.CheckoutRemarks = model.CheckoutRemarks;
                query.IsActive = true;
                query.ModifiedBy = User.Identity.Name;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetValetParking(int id)
        {
            var query = myapp.tbl_ValetParking.Where(l => l.ValetParkingId == id).SingleOrDefault();
            query.InTime = query.CreatedOn.Value.ToString("dd/MM/yyyy") + " " + query.InTime;
            query.VehicleNo = query.VehicleNo.ToUpper();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateDriverValetParking(int id, string indriver = "", string outdriver = "")
        {
            var query = myapp.tbl_ValetParking.Where(l => l.ValetParkingId == id).SingleOrDefault();
            if (indriver != null && indriver != "")
            {
                query.InDriverName = indriver;
            }
            if (outdriver != null && outdriver != "")
            {
                query.OutDriverName = outdriver;

            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteValetParking(int id)
        {
            var query = myapp.tbl_ValetParking.Where(l => l.ValetParkingId == id).SingleOrDefault();
            myapp.tbl_ValetParking.Remove(query);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult checkIfPatientVisitupdated(int id)
        {
            var checkifalreadyupdate = myapp.tbl_DailyVisiting.Where(l => l.DailyVisitingId == id && l.OutTime == null).Count();
            if (checkifalreadyupdate > 0)
            {
                return Json("Ok", JsonRequestBehavior.AllowGet);
            }
            return Json("NotOk", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetValetParking(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_ValetParking select d).OrderByDescending(l => l.ValetParkingId).AsQueryable();
            //if (!User.IsInRole("Admin"))
            //{
            //    string CreatedBy = User.Identity.Name;
            //    var user = myapp.tbl_User.Where(l => l.CustomUserId == CreatedBy).SingleOrDefault();
            //    if (user != null)
            //    {
            //        query = query.Where(l => l.LocationId == user.LocationId).ToList();
            //    }
            //}
            if (param.FormType != null && param.FormType != "" && param.FormType != "All")
            {
                query = query.Where(l => l.OutTime == null).AsQueryable();
            }
            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).AsQueryable();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).AsQueryable();
            }
            IEnumerable<tbl_ValetParking> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => SqlFunctions.StringConvert((decimal)c.ValetParkingId).Contains(param.sSearch.ToLower())
                               ||
                                c.VehicleNo != null && c.VehicleNo.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.InTime != null && c.InTime.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.OutTime != null && c.OutTime.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.InDriverName != null && c.InDriverName.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.OutDriverName != null && c.OutDriverName.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.Remarks != null && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.TokenNumber != null && c.TokenNumber.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.MobileNumber != null && c.MobileNumber.ToLower().Contains(param.sSearch.ToLower())

                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId

                         select new[] {
                                              c.ValetParkingId.ToString(),
                                              l.LocationName,
                                              c.VehicleNo.ToUpper(),
                                               c.CreatedOn.Value.ToString("dd/MM/yyyy")+" "+c.InTime,
                                              c.InDriverName,
                                              c.TokenNumber,
                                              c.MobileNumber,
                                              c.Remarks,
                                             c.OutTime!=null && c.OutTime!=""?( c.ModifiedOn.Value.ToString("dd/MM/yyyy")+" "+c.OutTime):"",
                                              c.OutDriverName,
                                              c.CheckoutRemarks,
                                              u.FirstName,
                                              c.OutTime!=null && c.OutTime!=""?(from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault():"",

                                              c.ValetParkingId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelValetParking(string fromDate, string toDate)
        {
            var query = (from d in myapp.tbl_ValetParking select d).OrderByDescending(l => l.ValetParkingId).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Valet Parking");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Vehicle No", typeof(string));
            products.Columns.Add("In Time", typeof(string));
            products.Columns.Add("IN Driver", typeof(string));
            products.Columns.Add("Token", typeof(string));
            products.Columns.Add("Mobile", typeof(string));
            products.Columns.Add("Out Time", typeof(string));
            products.Columns.Add("Out Driver", typeof(string));
            products.Columns.Add("In Remarks", typeof(string));
            products.Columns.Add("Out Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.ValetParkingId.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    c.VehicleNo,
                    "'" + c.CreatedOn.Value.ToString("dd/MM/yyyy") + " " + c.InTime,
                    c.InDriverName,
                    c.TokenNumber,
                    c.MobileNumber,
                   c.OutTime != null && c.OutTime != "" ? ("'" + c.ModifiedOn.Value.ToString("dd/MM/yyyy") + " " + c.OutTime) : "",
                   c.OutTime != null && c.OutTime != "" ? c.OutDriverName : "",
                    c.Remarks,
                    c.CheckoutRemarks,
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                    c.OutTime != null && c.OutTime != "" ? (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault() : ""
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=ValetParking.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        // Daily Visit Track..
        public ActionResult SaveDailyVisiting(tbl_DailyVisiting model)
        {
            model.CreatedBy = User.Identity.Name;
            string InSign = model.InSign;
            string SecuritySignature = model.SecuritySignature;
            model.InSign = "";
            model.SecuritySignature = "";
            if (model.MrNo == null)
            {
                model.MrNo = "";
            }
            if (model.VisitingType == "Staff")
            {
                model.OutTime = DateTime.Now.ToString("hh:mm tt");
            }
            else
            {
                model.InTime = DateTime.Now.ToString("hh:mm tt");
            }
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myapp.tbl_DailyVisiting.Add(model);
            myapp.SaveChanges();
            if (InSign != null && InSign != "")
            {
                try
                {
                    InSign = InSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InSign);
                    if (model.VisitingType == "Staff")
                    {
                        string file = Path.Combine(Server.MapPath("~/Documents/"), "OutSign_" + model.DailyVisitingId + ".png");
                        if (bytes.Length > 0)
                        {
                            using (var stream = new FileStream(file, FileMode.Create))
                            {
                                stream.Write(bytes, 0, bytes.Length);
                                stream.Flush();
                            }
                        }
                        model.OutSign = "OutSign_" + model.DailyVisitingId + ".png";
                    }
                    else
                    {
                        string file = Path.Combine(Server.MapPath("~/Documents/"), "InSign_" + model.DailyVisitingId + ".png");
                        if (bytes.Length > 0)
                        {
                            using (var stream = new FileStream(file, FileMode.Create))
                            {
                                stream.Write(bytes, 0, bytes.Length);
                                stream.Flush();
                            }
                        }
                        model.InSign = "InSign_" + model.DailyVisitingId + ".png";
                    }

                }
                catch { }
            }
            if (SecuritySignature != null && SecuritySignature != "")
            {
                try
                {
                    SecuritySignature = SecuritySignature.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(SecuritySignature);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "SecuritySign_" + model.DailyVisitingId + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.SecuritySignature = "SecuritySign_" + model.DailyVisitingId + ".png";
                }
                catch { }
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateDailyVisiting(tbl_DailyVisiting model)
        {
            var query = myapp.tbl_DailyVisiting.Where(l => l.DailyVisitingId == model.DailyVisitingId).SingleOrDefault();
            if (query != null)
            {
                query.PatientName = model.PatientName;
                query.MobileNo = model.MobileNo;
                query.Remarks = model.Remarks;
                query.IsActive = true;
                query.ModifiedBy = User.Identity.Name;
                query.ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult OutUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult SaveOutUpdateDailyVisit(UserSignoffViewModel model)
        {
            var dbModel = myapp.tbl_DailyVisiting.Where(m => m.DailyVisitingId == model.id).FirstOrDefault();
            model.signature = model.signature.Replace("data:image/png;base64,", String.Empty);
            var bytes = Convert.FromBase64String(model.signature);
            if (dbModel.VisitingType == "Staff")
            {
                string file = Path.Combine(Server.MapPath("~/Documents/"), "InSign_" + model.id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
                dbModel.InSign = "InSign_" + model.id + ".png";
                dbModel.InTime = DateTime.Now.ToString("hh:mm tt");
            }
            else
            {
                string file = Path.Combine(Server.MapPath("~/Documents/"), "OutSign_" + model.id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
                dbModel.OutSign = "OutSign_" + model.id + ".png";
                dbModel.OutTime = DateTime.Now.ToString("hh:mm tt");
            }
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDailyVisiting(int id)
        {
            var query = myapp.tbl_DailyVisiting.Where(l => l.DailyVisitingId == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteDailyVisiting(int id)
        {
            var query = myapp.tbl_DailyVisiting.Where(l => l.DailyVisitingId == id).SingleOrDefault();
            myapp.tbl_DailyVisiting.Remove(query);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetDailyVisiting(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_DailyVisiting select d).OrderByDescending(l => l.DailyVisitingId).ToList();
            //if (!User.IsInRole("Admin"))
            //{
            //    string CreatedBy = User.Identity.Name;
            //    var user = myapp.tbl_User.Where(l => l.CustomUserId == CreatedBy).SingleOrDefault();
            //    if (user != null)
            //    {
            //        query = query.Where(l => l.LocationId == user.LocationId).ToList();
            //    }
            //}
            if (param.FormType != null && param.FormType != "")
            {
                query = query.Where(l => l.VisitingType == param.FormType).ToList();
            }
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
            IEnumerable<tbl_DailyVisiting> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.DailyVisitingId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.PatientType != null && c.PatientType.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.PatientName != null && c.PatientName.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.MobileNo != null && c.MobileNo.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.MrNo != null && c.MrNo.ToLower().Contains(param.sSearch.ToLower())

                                ||
                                c.Remarks != null && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
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
                         join u in myapp.tbl_User on c.ModifiedBy equals u.CustomUserId
                         select new[] {
                                              c.DailyVisitingId.ToString(),
                                              c.VisitingType,
                                              l.LocationName,
                                              c.MrNo,
                                              c.PatientName,
                                              c.MobileNo,
                                              c.PatientType,
                                           c.VisitingType=="Staff"?( c.OutTime!=null && c.OutTime!=""?( c.CreatedOn.Value.ToString("dd/MM/yyyy")+" "+ c.OutTime):""):
                                           (c.InTime!=null && c.InTime!=""?( c.CreatedOn.Value.ToString("dd/MM/yyyy")+" "+ c.InTime):""),
                                              u.FirstName,
                                                  c.VisitingType=="Staff"?( c.InTime!=null && c.InTime!=""?( c.ModifiedOn.Value.ToString("dd/MM/yyyy")+" "+ c.InTime):""):
                                           (c.OutTime!=null && c.OutTime!=""?( c.ModifiedOn.Value.ToString("dd/MM/yyyy")+" "+ c.OutTime):""),
                                              c.DailyVisitingId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelDailyVisiting(string fromDate, string toDate, string type)
        {
            var query = (from d in myapp.tbl_DailyVisiting select d).OrderByDescending(l => l.DailyVisitingId).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (type != null && type != "")
            {
                query = query.Where(l => l.VisitingType == type).ToList();
            }
            var products = new System.Data.DataTable("DailyVisits");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Visiting Type", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Mr No", typeof(string));
            products.Columns.Add("Patient Name", typeof(string));
            products.Columns.Add("Mobile No", typeof(string));
            products.Columns.Add("Patient Type", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.DailyVisitingId.ToString(),
                    c.VisitingType,
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    c.MrNo,
                    c.PatientName,
                    c.MobileNo,
                    c.PatientType,
                    c.Remarks,
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                    c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=DailyVisits.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPdfDailyVisiting(string fromDate, string toDate, string type)
        {
            var query = (from d in myapp.tbl_DailyVisiting select d).OrderByDescending(l => l.DailyVisitingId).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (type != null && type != "")
            {
                query = query.Where(l => l.VisitingType == type).ToList();
            }
            var products = new System.Data.DataTable("DailyVisits");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Visiting Type", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Mr No", typeof(string));
            products.Columns.Add("Patient Name", typeof(string));
            products.Columns.Add("Mobile No", typeof(string));
            products.Columns.Add("Patient Type", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.DailyVisitingId.ToString(),
                    c.VisitingType,
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    c.MrNo,
                    c.PatientName,
                    c.MobileNo,
                    c.PatientType,
                    c.Remarks,
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                    c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }


            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
            cell.Colspan = products.Columns.Count;

            foreach (DataColumn c in products.Columns)
            {
                table.AddCell(new Phrase(c.ColumnName, myFont));
            }
            int i = products.Columns.Count;
            foreach (DataRow r in products.Rows)
            {
                if (products.Rows.Count > 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        table.AddCell(new Phrase(r[j].ToString(), font5));
                    }
                }
            }
            document.Add(table);
            document.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename= " + type + "DailyVisits.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageNursingDocuments()
        {
            return View();
        }
        public ActionResult NewNursingDocument(int id = 0)
        {
            ViewBag.id = id;
            return View();
        }
        public ActionResult AjaxGetNursingDocuments(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_NursingDetails select d).OrderByDescending(l => l.NursingDetailsId).ToList();


            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_NursingDetails> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.NursingDetailsId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.NameOfStudent != null && c.NameOfStudent.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.FatherName != null && c.FatherName.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.HallTicketNo != null && c.HallTicketNo.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(param.sSearch.ToLower())

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
                                              c.NursingDetailsId.ToString(),
                                              c.NameOfStudent,
                                              c.FatherName,
                                               c.HallTicketNo,
                                              c.RegistrationNo,
                                              c.Qualification,
                                              c.DateOfJoining,
                                              c.DateOfRelieving,
                                              u.FirstName,
                                              c.NursingDetailsId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AdminNursingDocuments(tbl_NursingDetailsDocument Doc, HttpPostedFileBase Upload)
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
            myapp.tbl_NursingDetailsDocument.Add(Doc);
            myapp.SaveChanges();
            return Json("Added Successfully", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetNursingDocumentbyId(int id)
        {
            var files = from file in myapp.tbl_NursingDetailsDocument
                        where file.NursingDetailsId == id
                        select new { Did = file.Id, url = file.DocumentUrl, title = file.DocumentName, type = file.DocumentType };
            return Json(files, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetNursingDetailsbyId(int id)
        {
            var files = myapp.tbl_NursingDetails.Where(l => l.NursingDetailsId == id).SingleOrDefault();
            return Json(files, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveNursingDocuments(tbl_NursingDetails model, HttpPostedFileBase Markmemo1file, HttpPostedFileBase Markmemo2file,
            HttpPostedFileBase Markmemo3file, HttpPostedFileBase OrginalCertificatefile, HttpPostedFileBase StateRegistrationCertificatefile,
            HttpPostedFileBase Transcriptfile, HttpPostedFileBase CourseCompletionfile, HttpPostedFileBase TransferCertificatefile)
        {
            if (model.NursingDetailsId == 0)
            {
                model.CreatedBy = User.Identity.Name;
                model.CreatedOn = DateTime.Now;
                model.IsActive = true;
                myapp.tbl_NursingDetails.Add(model);
                myapp.SaveChanges();
            }
            else
            {
                var dbmodel = myapp.tbl_NursingDetails.Where(l => l.NursingDetailsId == model.NursingDetailsId).SingleOrDefault();
                if (dbmodel != null)
                {
                    dbmodel.Course = model.Course;
                    dbmodel.NameOfStudent = model.NameOfStudent;
                    dbmodel.FatherName = model.FatherName;
                    dbmodel.DateOfBirth = model.DateOfBirth;
                    dbmodel.Religion = model.Religion;
                    dbmodel.IndentificationMarks = model.IndentificationMarks;
                    dbmodel.Qualification = model.Qualification;
                    dbmodel.AdmissionNo = model.AdmissionNo;
                    dbmodel.RegistrationNo = model.RegistrationNo;
                    dbmodel.HallTicketNo = model.HallTicketNo;
                    dbmodel.DateOfJoining = model.DateOfJoining;
                    dbmodel.DateOfRelieving = model.DateOfRelieving;
                    dbmodel.ResultsParticulars_PTS_date = model.ResultsParticulars_PTS_date;
                    dbmodel.ResultsParticulars_PTS_remarks = model.ResultsParticulars_PTS_remarks;
                    dbmodel.ResultsParticulars_1styear_date = model.ResultsParticulars_1styear_date;
                    dbmodel.ResultsParticulars_1styear_remarks = model.ResultsParticulars_1styear_remarks;

                    dbmodel.ResultsParticulars_2ndyear_date = model.ResultsParticulars_2ndyear_date;
                    dbmodel.ResultsParticulars_2ndyear_remarks = model.ResultsParticulars_2ndyear_remarks;
                    dbmodel.ResultsParticulars_3rdyear_date = model.ResultsParticulars_3rdyear_date;
                    dbmodel.ResultsParticulars_3rdyear_remarks = model.ResultsParticulars_3rdyear_remarks;
                    dbmodel.ResultsParticulars_Internship_date = model.ResultsParticulars_Internship_date;
                    dbmodel.ResultsParticulars_Internship_remarks = model.ResultsParticulars_Internship_remarks;
                    myapp.SaveChanges();

                }

            }
            if (Markmemo1file != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(Markmemo1file.FileName);
                string extension = Path.GetExtension(Markmemo1file.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;
                //model.Remarks = fileName;
                Markmemo1file.SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));
                tbl_NursingDetailsDocument modeldec = new tbl_NursingDetailsDocument();
                modeldec.DocumentName = "Mark Memo1";
                modeldec.DocumentUrl = fileName;
                modeldec.NursingDetailsId = model.NursingDetailsId;
                modeldec.CreatedBy = User.Identity.Name;
                modeldec.CreatedOn = DateTime.Now;

                myapp.tbl_NursingDetailsDocument.Add(modeldec);
                myapp.SaveChanges();
            }

            if (Markmemo2file != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(Markmemo2file.FileName);
                string extension = Path.GetExtension(Markmemo2file.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;

                Markmemo2file.SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));
                tbl_NursingDetailsDocument modeldec = new tbl_NursingDetailsDocument();
                modeldec.DocumentName = "Mark Memo2";
                modeldec.DocumentUrl = fileName;
                modeldec.NursingDetailsId = model.NursingDetailsId;
                modeldec.CreatedBy = User.Identity.Name;
                modeldec.CreatedOn = DateTime.Now;

                myapp.tbl_NursingDetailsDocument.Add(modeldec);
                myapp.SaveChanges();
            }

            if (Markmemo3file != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(Markmemo3file.FileName);
                string extension = Path.GetExtension(Markmemo3file.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;

                Markmemo3file.SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));
                tbl_NursingDetailsDocument modeldec = new tbl_NursingDetailsDocument();
                modeldec.DocumentName = "Mark Memo3";
                modeldec.DocumentUrl = fileName;
                modeldec.NursingDetailsId = model.NursingDetailsId;
                modeldec.CreatedBy = User.Identity.Name;
                modeldec.CreatedOn = DateTime.Now;

                myapp.tbl_NursingDetailsDocument.Add(modeldec);
                myapp.SaveChanges();
            }
            if (OrginalCertificatefile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(OrginalCertificatefile.FileName);
                string extension = Path.GetExtension(OrginalCertificatefile.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;

                OrginalCertificatefile.SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));
                tbl_NursingDetailsDocument modeldec = new tbl_NursingDetailsDocument();
                modeldec.DocumentName = "Orginal Certificate";
                modeldec.DocumentUrl = fileName;
                modeldec.NursingDetailsId = model.NursingDetailsId;
                modeldec.CreatedBy = User.Identity.Name;
                modeldec.CreatedOn = DateTime.Now;

                myapp.tbl_NursingDetailsDocument.Add(modeldec);
                myapp.SaveChanges();
            }

            if (StateRegistrationCertificatefile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(StateRegistrationCertificatefile.FileName);
                string extension = Path.GetExtension(StateRegistrationCertificatefile.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;

                StateRegistrationCertificatefile.SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));
                tbl_NursingDetailsDocument modeldec = new tbl_NursingDetailsDocument();
                modeldec.DocumentName = "State Registration Certificate";
                modeldec.DocumentUrl = fileName;
                modeldec.NursingDetailsId = model.NursingDetailsId;
                modeldec.CreatedBy = User.Identity.Name;
                modeldec.CreatedOn = DateTime.Now;
                myapp.tbl_NursingDetailsDocument.Add(modeldec);
                myapp.SaveChanges();
            }

            if (Transcriptfile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(Transcriptfile.FileName);
                string extension = Path.GetExtension(Transcriptfile.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;

                Transcriptfile.SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));
                tbl_NursingDetailsDocument modeldec = new tbl_NursingDetailsDocument();
                modeldec.DocumentName = "Transcript";
                modeldec.DocumentUrl = fileName;
                modeldec.NursingDetailsId = model.NursingDetailsId;
                modeldec.CreatedBy = User.Identity.Name;
                modeldec.CreatedOn = DateTime.Now;
                myapp.tbl_NursingDetailsDocument.Add(modeldec);
                myapp.SaveChanges();
            }

            if (CourseCompletionfile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(CourseCompletionfile.FileName);
                string extension = Path.GetExtension(CourseCompletionfile.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;

                CourseCompletionfile.SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));
                tbl_NursingDetailsDocument modeldec = new tbl_NursingDetailsDocument();
                modeldec.DocumentName = "Course Completion";
                modeldec.DocumentUrl = fileName;
                modeldec.NursingDetailsId = model.NursingDetailsId;
                modeldec.CreatedBy = User.Identity.Name;
                modeldec.CreatedOn = DateTime.Now;
                myapp.tbl_NursingDetailsDocument.Add(modeldec);
                myapp.SaveChanges();
            }

            if (TransferCertificatefile != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(TransferCertificatefile.FileName);
                string extension = Path.GetExtension(TransferCertificatefile.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;

                TransferCertificatefile.SaveAs(Path.Combine(Server.MapPath("~/Documents/"), fileName));
                tbl_NursingDetailsDocument modeldec = new tbl_NursingDetailsDocument();
                modeldec.DocumentName = "Transfer Certificate";
                modeldec.DocumentUrl = fileName;
                modeldec.NursingDetailsId = model.NursingDetailsId;
                modeldec.CreatedBy = User.Identity.Name;
                modeldec.CreatedOn = DateTime.Now;
                myapp.tbl_NursingDetailsDocument.Add(modeldec);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}