using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using Microsoft.AspNet.Identity.Owin;
using WebApplicationHsApp.Models;
using System.Threading.Tasks;
using System.Data.Entity;
using WebApplicationHsApp.OracleInterface;

namespace WebApplicationHsApp.Controllers
{
    [Authorize(Roles = "Admin,HrAdmin,Vendor")]
    public class HrAdminController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();

        private ApplicationUserManager _userManager;

        public HrAdminController()
        {
        }

        public HrAdminController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        // GET: HrAdmin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LeaveTypes()
        {
            return View();
        }
        public JsonResult GetLeaveTypes()
        {
            var list = myapp.tbl_LeaveType.ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AddLeaveType(tbl_LeaveType tbll)
        {
            tbll.CreatedBy = User.Identity.Name;
            tbll.CreatedOn = DateTime.Now;
            myapp.tbl_LeaveType.Add(tbll);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteLeaveType(int id)
        {
            var list = myapp.tbl_LeaveType.Where(l => l.LeaveTypeId == id).ToList();
            if (list.Count > 0)
            {
                myapp.tbl_LeaveType.Remove(list[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult LeaveTypeVsRole()
        {
            return View();
        }
        public JsonResult SaveLeaveTypeVsRole(LeaveTypeVsRoleViewModel model)
        {
            return Json("Sccuess", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteLeaveTypeVsRole(int id)
        {
            return Json("Sccuess", JsonRequestBehavior.AllowGet);
        }
        public ActionResult EmployeeLeaves()
        {
            if (TempData["UploadImportMsg"] != null)
            {
                ViewBag.UploadImportMsg = TempData["UploadImportMsg"];
            }
            return View();
        }
        public JsonResult GetEmployeeLeaves()
        {
            var list = myapp.tbl_ManageLeave.ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetEmployeeLeaveCount(JQueryDataTableParamModel param)
        {
            List<LeavesCountView> Leavscount = new List<LeavesCountView>();

            var query = myapp.tbl_ManageLeave.ToList();
            var newquery = query.GroupBy(u => u.UserId).ToList();
            foreach (var v in newquery)
            {
                LeavesCountView lv = new LeavesCountView();
                lv.UserId = v.Key;
                foreach (var vs in v.ToList())
                {
                    lv.UserName = vs.UserName;
                    lv.LocationId = vs.LocationId.HasValue ? vs.LocationId.Value : 0;
                    lv.DepartmentId = vs.DepartmentId.HasValue ? vs.DepartmentId.Value : 0;
                    lv.LocationName = vs.LocationName;
                    lv.DepartmentName = vs.DepartmentName;
                    if (vs.LeaveTypeId == 1)
                    {
                        lv.CasuvalAvailableLeave = vs.AvailableLeave;

                    }
                    if (vs.LeaveTypeId == 4)
                    {
                        lv.SickAvailableLeave = vs.AvailableLeave;
                    }
                    if (vs.LeaveTypeId == 5)
                    {
                        lv.EarnedAvailableLeave = vs.AvailableLeave;
                    }
                    if (vs.LeaveTypeId == 6)
                    {
                        lv.CompoffBalance = vs.AvailableLeave;
                    }
                    lv.IsActive = vs.IsActive;
                }
                Leavscount.Add(lv);
            }

            if (param.locationid != null && param.locationid != 0)
            {
                Leavscount = Leavscount.Where(q => q.LocationId == param.locationid).ToList();
            }
            if (param.departmentid != null && param.departmentid != 0)
            {
                Leavscount = Leavscount.Where(q => q.DepartmentId == param.departmentid).ToList();
            }
            Leavscount = (from q in Leavscount
                          orderby int.Parse(q.UserId.ToLower().Replace("fh_", ""))
                          select q).ToList();
            IEnumerable<LeavesCountView> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = Leavscount
                   .Where(c => c.UserId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.UserName != null && c.UserName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())
                              //||
                              //c.LeaveTypeName != null && c.LeaveTypeName.ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = Leavscount;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies

                         select new[] {
                          c.UserId.ToLower().Replace("fh_",""),
                            c.LocationName,
                            c.DepartmentName,
                             c.UserName,
                             c.CasuvalAvailableLeave.HasValue ? c.CasuvalAvailableLeave.Value.ToString():"0",
                             c.SickAvailableLeave.HasValue? c.SickAvailableLeave.Value.ToString():"0",
                            c.EarnedAvailableLeave.HasValue? c.EarnedAvailableLeave.Value.ToString():"0",
                             c.CompoffBalance.HasValue? c.CompoffBalance.Value.ToString():"0",
                            c.IsActive.ToString(),
                             c.UserId
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = Leavscount.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeLeavesByid(string empid)
        {
            List<LeavesCountView> Leavscount = new List<LeavesCountView>();
            var query = myapp.tbl_ManageLeave.Where(u => u.UserId == empid).ToList();
            var newquery = query.GroupBy(u => u.UserId).ToList();
            foreach (var v in newquery)
            {
                LeavesCountView lv = new LeavesCountView();
                lv.UserId = v.Key;
                foreach (var vs in v.ToList())
                {
                    lv.UserName = vs.UserName;
                    lv.LocationId = vs.LocationId.Value;
                    lv.DepartmentId = vs.DepartmentId.Value;
                    lv.LocationName = vs.LocationName;
                    lv.DepartmentName = vs.DepartmentName;
                    if (vs.LeaveTypeId == 1)
                    {
                        lv.CasuvalAvailableLeave = vs.AvailableLeave;

                    }
                    if (vs.LeaveTypeId == 4)
                    {
                        lv.SickAvailableLeave = vs.AvailableLeave;
                    }
                    if (vs.LeaveTypeId == 5)
                    {
                        lv.EarnedAvailableLeave = vs.AvailableLeave;
                    }
                    lv.IsActive = vs.IsActive;
                }
                Leavscount.Add(lv);
            }
            if (Leavscount.Count == 1)
            {
                return Json(Leavscount[0], JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(null, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult UpdateEmployeeleaves(string empid, double casuval, double sick, double earned)
        {
            var query = myapp.tbl_ManageLeave.Where(u => u.UserId == empid).ToList();
            if (query.Count > 0)
            {
                foreach (var v in query)
                {
                    if (v.LeaveTypeId == 1)
                    {
                        v.AvailableLeave = casuval;

                    }
                    if (v.LeaveTypeId == 4)
                    {
                        v.AvailableLeave = sick;
                    }
                    if (v.LeaveTypeId == 5)
                    {
                        v.AvailableLeave = earned;
                    }
                }
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult AddEmployeeLeave(tbl_ManageLeave tbll)
        {
            var list = myapp.tbl_User.Where(u => u.IsActive == true && u.DateOfJoining != null && u.CustomUserId == tbll.UserId).ToList();
            DateTime DateofJoining = list[0].DateOfJoining.Value;
            var days = (DateTime.Now - DateofJoining).TotalDays;
            if (days > 15)
            {
                var checkdtaa = myapp.tbl_ManageLeave.Where(l => l.UserId == tbll.UserId && l.LeaveTypeId == tbll.LeaveTypeId).ToList();
                if (checkdtaa.Count == 0)
                {
                    int year = (int)(days / 365.25);
                    if (tbll.LeaveTypeId == 5)
                    {
                        if (tbll.AvailableLeave.HasValue && tbll.AvailableLeave.Value > 0 && tbll.AvailableLeave <= tbll.CountOfLeave)
                        {
                            double TotalLeaves = tbll.CountOfLeave.Value;
                            double Thisyearaddedleave = tbll.AvailableLeave.Value;

                            if (list.Count > 0)
                            {
                                tbl_LeaveUpdateHistory luph = new tbl_LeaveUpdateHistory();
                                if (year == 1)
                                {
                                    AddLeavestoHistory(tbll.CountOfLeave.Value, "5", "EL added", tbll.UserId, 1, DateTime.Now.Month, true, false);
                                }
                                else
                                {
                                    var checkprevious = year - 1;
                                    if (checkprevious > 1)
                                    {
                                        var checkcurrentleaves = tbll.CountOfLeave.Value - tbll.AvailableLeave.Value;
                                        var eachyearleaves = checkcurrentleaves / checkprevious;
                                        for (var i = 1; i < year; i++)
                                        {
                                            AddLeavestoHistory(eachyearleaves, "5", "EL added", tbll.UserId, i, DateTime.Now.Month, true, false);
                                        }
                                        AddLeavestoHistory(tbll.AvailableLeave.Value, "5", "EL added", tbll.UserId, year, DateTime.Now.Month, true, false);
                                    }
                                    else
                                    {
                                        var alvlleaves = tbll.CountOfLeave.Value - tbll.AvailableLeave.Value;
                                        AddLeavestoHistory(alvlleaves, "5", "EL added", tbll.UserId, year - 1, DateTime.Now.Month, true, false);
                                        AddLeavestoHistory(tbll.AvailableLeave.Value, "5", "EL added", tbll.UserId, year, DateTime.Now.Month, true, false);
                                    }
                                }
                            }
                        }
                    }
                    else if (tbll.LeaveTypeId == 1 || tbll.LeaveTypeId == 4)
                    {
                        AddLeavestoHistory(tbll.CountOfLeave.Value, tbll.LeaveTypeId.ToString(), tbll.LeaveTypeName, tbll.UserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
                    }
                    tbll.AvailableLeave = tbll.CountOfLeave;
                    tbll.CreatedBy = User.Identity.Name;
                    tbll.CreatedOn = DateTime.Now;
                    myapp.tbl_ManageLeave.Add(tbll);
                    myapp.SaveChanges();
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Leaves need to be add after 15 days of employee joining", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json("Please Check Leave Already added", JsonRequestBehavior.AllowGet);
            }

        }
        public int MonthDifference(DateTime lValue, DateTime rValue)
        {
            return (lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year);
        }
        public void AddLeavestoHistory(double count, string type, string remarks, string userid, int year, int month, bool isyearly, bool ismonthly)
        {
            tbl_LeaveUpdateHistory luph = new tbl_LeaveUpdateHistory();
            luph.AddedLeaves = count;
            luph.Created = DateTime.Now;
            luph.LeaveType = type;
            luph.Remarks = remarks;
            luph.UserId = userid;
            luph.Year = year;
            luph.Month = month;
            luph.IsYearly = isyearly;
            luph.IsMonthly = ismonthly;
            myapp.tbl_LeaveUpdateHistory.Add(luph);
            myapp.SaveChanges();
        }
        public JsonResult DeleteEmployeeLeave(int id)
        {
            var list = myapp.tbl_ManageLeave.Where(l => l.UserLeaveId == id).ToList();
            if (list.Count > 0)
            {
                myapp.tbl_ManageLeave.Remove(list[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult LeaveApplications()
        {
            return View();
        }
        public ActionResult AjaxMyApprovedView(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                var query = myapp.tbl_Leave.Where(q => q.LeaveStatus == "Approved" || q.LeaveStatus == "Pending");

                if (param.locationid != null && param.locationid != 0)
                {
                    query = query.Where(q => q.LocationId == param.locationid);
                }
                if (param.departmentid != null && param.departmentid != 0)
                {
                    if (param.locationid != null && param.locationid > 0)
                    {
                        query = query.Where(q => q.DepartmentId == param.departmentid);
                    }
                    else
                    {
                        var departmentid = myapp.tbl_DepartmentVsCommonDepartment.Where(d => d.DepartmentId == param.departmentid).Select(n => n.CommonDepartmentId).ToList();
                        query = (from q in query where departmentid.Contains(q.DepartmentId) select q);
                    }
                }
                if (param.LeaveTypeid != null && param.LeaveTypeid != 0)
                {
                    query = query.Where(q => q.LeaveTypeId == param.LeaveTypeid);
                }
                if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
                {
                    DateTime frmdate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                    DateTime dtodate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                    query = query.Where(l => (frmdate <= l.LeaveFromDate && dtodate >= l.LeaveFromDate) || (frmdate <= l.LeaveTodate && dtodate >= l.LeaveTodate));

                }

                if (param.Emp != null && param.Emp != "")
                {
                    query = query.Where(q => q.UserId == param.Emp);
                }
                var query1 =
                   (from c in query.ToList()
                    join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
                    join app2 in myapp.tbl_User on c.Level2Approver equals app2.CustomUserId
                    //where c.LeaveStatus == "Pending" && (c.Level1Approver == User.Identity.Name || c.Level2Approver == User.Identity.Name)
                    select new LeaveViewModels
                    {
                        LeaveId = c.LeaveId.ToString(),
                        LeaveTypeName = c.LeaveTypeName,
                        IsFullday = c.IsFullday.ToString(),
                        IsCompOff = c.IsCompOff.ToString(),
                        LeaveFromDate = c.LeaveFromDate.Value.ToString("dd/MM/yyyy"),
                        LeaveTodate = c.LeaveTodate.Value.ToString("dd/MM/yyyy"),
                        LeaveStatus = c.LeaveStatus,
                        Level1Approver = app1.FirstName + " " + app1.LastName,
                        Level2Approver = app2.FirstName + " " + app2.LastName,
                        UserName = c.UserName,
                        DepartmentName = c.DepartmentName,
                        LocationName = c.LocationName,
                        DateofAvailableCompoff = (Convert.ToBoolean(c.IsCompOff) ? (c.DateofAvailableCompoff.HasValue ? c.DateofAvailableCompoff.Value.ToString("dd/MM/yyyy") : "") : c.LeaveFromDate.Value.ToString("dd/MM/yyyy")),
                        AddressOnLeave = c.AddressOnLeave,
                        ReasonForLeave = c.ReasonForLeave,
                        TotalLeaves = c.LeaveCount.HasValue ? c.LeaveCount.Value : 0,
                        LeaveCreatedOn = c.CreatedOn.Value.ToString("dd/MM/yyyy")

                    });

                var results = query1.OrderByDescending(t => t.LeaveId).ToList();

                IEnumerable<LeaveViewModels> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = results
                       .Where(c => c.LeaveTypeName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.Level1Approver != null && c.Level1Approver.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||

                                  c.LeaveStatus != null && c.LeaveStatus.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.LocationName != null && c.LocationName.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.UserName != null && c.UserName.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.userid != null && c.userid.ToLower().Contains(param.sSearch.ToLower())

                                  );
                }
                else
                {
                    filteredCompanies = results;
                }
                var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayedCompanies
                             select new[] {
                                 c.LocationName,
                                 c.DepartmentName,
                                  c.UserName,
                                  c.LeaveTypeName,
                                  c.IsFullday.ToString(),
                                  c.IsCompOff.ToString(),
                                 (Convert.ToBoolean(c.IsCompOff)?c.DateofAvailableCompoff:c.LeaveFromDate),
                                 c.LeaveTodate,
                                 c.TotalLeaves.ToString(),
                                 c.LeaveCreatedOn,
                                 c.LeaveStatus,
                              c.LeaveId.ToString()};
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = results.Count(),
                    iTotalDisplayRecords = filteredCompanies.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }
        public ActionResult ViewPermissions()
        {
            return View();
        }
        public ActionResult ManageShiftTypes()
        {
            return View();
        }
        public JsonResult GetShiftTypes()
        {
            var list = myapp.tbl_ShiftType.ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AddShiftType(tbl_ShiftType tbll)
        {
            tbll.CreatedBy = User.Identity.Name;
            tbll.CreatedOn = DateTime.Now;
            myapp.tbl_ShiftType.Add(tbll);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteShiftType(int id)
        {
            var list = myapp.tbl_ShiftType.Where(l => l.ShiftTypeId == id).ToList();
            if (list.Count > 0)
            {
                myapp.tbl_ShiftType.Remove(list[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageHolidays()
        {
            return View();
        }
        public JsonResult GetHolidays()
        {
            var list = myapp.tbl_Holiday.OrderBy(a => a.HolidayDate).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AddHoliday(tbl_Holiday tbll)
        {
            tbll.CreatedBy = User.Identity.Name;
            tbll.CreatedOn = DateTime.Now;
            myapp.tbl_Holiday.Add(tbll);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteHoliday(int id)
        {
            var list = myapp.tbl_Holiday.Where(l => l.HolidayId == id).ToList();
            if (list.Count > 0)
            {
                myapp.tbl_Holiday.Remove(list[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ViewDutyRoster()
        {
            return View();
        }
        public ActionResult Reports()
        {
            return View();
        }
        public ActionResult TimeTrack()
        {
            return View();
        }
        public ActionResult ManageEmployees()
        {
            //var list = myapp.tbl_User.ToList();
            return View();
        }
        public ActionResult VendorManageEmployees()
        {
            string vendoremail = User.Identity.Name;
            List<tbl_User> usrlist = new List<tbl_User>();
            var vendordetails = myapp.tbl_Vendor.Where(l => l.Email == vendoremail).SingleOrDefault();
            if (vendordetails != null)
            {
                usrlist = myapp.tbl_User.Where(l => l.VendorId == vendordetails.VendorId).ToList();
            }
            return View(usrlist);
        }
        public JsonResult ExportExcelEmployees()
        {
            var query = myapp.tbl_User.Where(a => a.IsActive == true).ToList();

            var products = new System.Data.DataTable("Users");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Employee Number", typeof(string));
            products.Columns.Add("EmailId", typeof(string));
            products.Columns.Add("Phone Number", typeof(string));
            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("Sub Department", typeof(string));
            products.Columns.Add("Designation", typeof(string));
            products.Columns.Add("Reporting Manager", typeof(string));
            products.Columns.Add("FoodType", typeof(string));
            products.Columns.Add("Gender", typeof(string));
            products.Columns.Add("AadharCard", typeof(string));
            products.Columns.Add("PanCard", typeof(string));
            products.Columns.Add("Qualification", typeof(string));
            products.Columns.Add("CollageName", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.UserId,
                  c.LocationName,
                                              c.FirstName + " " + c.LastName + "(" + (c.IsAppLogin == true ? "Y" : "N") + ")",
                                              c.CustomUserId.ToLower().Replace("fh_", "") + " " + (c.IsOffRollDoctor == true ? "OffRole" : "") + " " + (c.IsOnRollDoctor == true ? "OnRole" : ""),
                                              c.EmailId,
                                              //c.PlaceAllocation,
                                              c.PhoneNumber,
                                              //c.Extenstion,
                                              c.DepartmentName,
                                              (c.SubDepartmentName != null && c.SubDepartmentName != "NULL") ? c.SubDepartmentName : " ",
                                              c.Designation,
                                              c.ReportingManagerId>0?(from V in myapp.tbl_User where V.EmpId == c.ReportingManagerId select V.FirstName + " " + V.EmpId).SingleOrDefault():"",
                                                 c.FoodType,
                                                 c.Gender,
                                                 c.aadharCard,
                                                 c.PanCard,
                                                 c.Qualification,
                                                 c.CollegeName

                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=AllEmployees.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetEmployees(JQueryDataTableParamModel param)
        {

            if (Request.IsAuthenticated)
            {
                var query = myapp.tbl_User.Where(a => a.IsActive == true).ToList();
                if (param.locationid != null && param.locationid != 0)
                {
                    query = query.Where(q => q.LocationId == param.locationid).ToList();
                }
                if (param.departmentid != null && param.departmentid != 0)
                {
                    if (param.locationid != null && param.locationid > 0)
                    {
                        query = query.Where(q => q.DepartmentId == param.departmentid).ToList();
                    }
                    else
                    {
                        var departmentid = myapp.tbl_DepartmentVsCommonDepartment.Where(d => d.DepartmentId == param.departmentid).Select(n => n.CommonDepartmentId).ToList();
                        query = (from q in query where departmentid.Contains(q.DepartmentId) select q).ToList();
                    }
                }
                query = (from q in query
                         orderby int.Parse(q.CustomUserId.ToLower().Replace("fh_", ""))
                         select q).ToList();
                IEnumerable<tbl_User> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.CustomUserId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.FirstName != null && c.FirstName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.LastName != null && c.LastName.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.LastName != null && c.LastName.ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                  c.EmailId != null && c.EmailId.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.PlaceAllocation != null && c.PlaceAllocation.ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                  c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())
                                  );
                }
                else
                {
                    filteredCompanies = query;
                }
                var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayedCompanies
                             select new[] {/*c.UserId.ToString(),*/
                                              c.LocationName,
                                              c.FirstName+" "+c.LastName +"("+(c.IsAppLogin==true?"Y":"N")+")",
                                              c.CustomUserId.ToLower().Replace("fh_","") + " "+ (c.IsOffRollDoctor==true? "OffRole":"")+" "+(c.IsOnRollDoctor==true?"OnRole":""),
                                              c.EmailId,
                                              //c.PlaceAllocation,
                                              c.PhoneNumber,
                                              //c.Extenstion,
                                              c.DepartmentName,
                                              (c.SubDepartmentName!=null && c.SubDepartmentName!="NULL")?c.SubDepartmentName:" ",
                                              c.Designation,
                                                 (from V in myapp.tbl_User where V.EmpId == c.ReportingManagerId select V.FirstName+" "+V.EmpId).SingleOrDefault(),
                                                 c.FoodType,
                                              c.UserId.ToString()};
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query.Count(),
                    iTotalDisplayRecords = filteredCompanies.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }
        public ActionResult ManageGroupEmployee()
        {
            return View();
        }
        public ActionResult AddEmployee()
        {
            //int NextUserID = 1;
            //var List = myapp.tbl_User.Select(e => e.UserId).ToList();
            //if (List.Count > 0)
            //{
            //    NextUserID = List.Max();
            //    NextUserID++;
            //}
            //ViewBag.NextUserID = NextUserID;

            if (TempData["UploadImportMsg"] != null)
            {
                ViewBag.UploadImportMsg = TempData["UploadImportMsg"];
            }
            return View();
        }
        public JsonResult GetDepartmentReporingManager(int locationid, int departmentid, string type)
        {
            if (type == "Employee")
            {
                var list = myapp.tbl_ReportingManager.Where(l => l.LocationId == locationid && l.DepartmentId == departmentid && l.IsHod == true).ToList();
                if (list.Count > 0)
                {
                    return Json(list[0], JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("No Data", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var list = myapp.tbl_ReportingManager.Where(l => l.LocationId == locationid && l.DepartmentId == departmentid && l.IsHodOfHod == true).ToList();
                if (list.Count > 0)
                {
                    return Json(list[0], JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("No Data", JsonRequestBehavior.AllowGet);
                }
            }
        }
        public JsonResult GetSubDepartmentReporingManager(int locationid, int departmentid, int subdepartmentid, string type)
        {
            if (type == "Employee")
            {
                var list = myapp.tbl_ReportingManager.Where(l => l.LocationId == locationid && l.DepartmentId == departmentid && l.SubDepartmentId == subdepartmentid && l.IsHod == true).ToList();
                if (list.Count > 0)
                {
                    return Json(list[0], JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("No Data", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var list = myapp.tbl_ReportingManager.Where(l => l.LocationId == locationid && l.DepartmentId == departmentid && l.SubDepartmentId == subdepartmentid && l.IsHodOfHod == true).ToList();
                if (list.Count > 0)
                {
                    return Json(list[0], JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("No Data", JsonRequestBehavior.AllowGet);
                }
            }
        }
        public JsonResult ValidateDepartment(string data, string type)
        {
            bool Validate = true;
            string message = "";
            JavaScriptSerializer js = new JavaScriptSerializer();
            if (data != null && data != "")
            {
                var list = js.Deserialize<List<tbl_ReportingManager>>(data);
                if (list.Count > 0)
                {
                    foreach (var v in list)
                    {
                        Validate = ValidateReportingManager(v, type, ref message);
                    }
                }
            }
            MessageModel mm = new MessageModel();
            mm.IsSuccess = Validate;
            mm.Message = message;

            return Json(mm, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageReportingManagers()
        {
            return View();
        }

        [HttpPost]
        public ActionResult EditLeave(int id)
        {
            var leaves = myapp.tbl_Leave.Where(l => l.LeaveId == id).ToList();
            var model =
                    (from c in leaves
                     join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
                     join app2 in myapp.tbl_User on c.Level2Approver equals app2.CustomUserId

                     select new LeaveViewModels
                     {
                         LeaveId = c.LeaveId.ToString(),
                         LeaveTypeName = c.LeaveTypeName,
                         IsFullday = c.IsFullday.ToString(),
                         IsCompOff = c.IsCompOff.ToString(),
                         LeaveFromDate = c.LeaveFromDate.Value.ToString("dd/MM/yyyy"),
                         LeaveTodate = c.LeaveTodate.Value.ToString("dd/MM/yyyy"),
                         LeaveStatus = c.LeaveStatus,
                         Level1Approver = app1.FirstName + " " + app1.LastName,
                         Level2Approver = app2.FirstName + " " + app2.LastName,
                         ReasonForLeave = c.ReasonForLeave,
                         AddressOnLeave = c.AddressOnLeave,
                         DateofAvailableCompoff = (c.DateofAvailableCompoff.HasValue ? c.DateofAvailableCompoff.Value.ToString("dd/MM/yyyy") : ""),
                         Level1ApproveComment = c.Level1ApproveComment,
                         Level2ApproveComment = c.Level2ApproveComment,
                         Level1Approved = c.Level1Approved.ToString(),
                         Level2Approved = c.Level2Approved.ToString()
                     }).FirstOrDefault();

            return PartialView("_EditLeave", model);
        }


        public JsonResult GetEditLeaveDetailsByID(int id)
        {
            var List = (from c in myapp.tbl_Leave
                        join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
                        join app2 in myapp.tbl_User on c.Level2Approver equals app2.CustomUserId
                        where c.LeaveId == id
                        select c).ToList();
            var model = (from c in List
                         select new LeaveViewModels
                         {
                             LeaveId = c.LeaveId.ToString(),
                             LeaveTypeId = Convert.ToInt32(c.LeaveTypeId),
                             LeaveTypeName = c.LeaveTypeName,
                             IsFullday = c.IsFullday.ToString(),
                             IsCompOff = c.IsCompOff.ToString(),
                             LeaveFromDate = c.LeaveFromDate.Value.ToString("dd/MM/yyyy"),
                             LeaveTodate = c.LeaveTodate.Value.ToString("dd/MM/yyyy"),
                             LeaveStatus = c.LeaveStatus,
                             ReasonForLeave = c.ReasonForLeave,
                             AddressOnLeave = c.AddressOnLeave,
                             DateofAvailableCompoff = (c.DateofAvailableCompoff.HasValue ? c.DateofAvailableCompoff.Value.ToString("dd/MM/yyyy") : ""),
                             Level1ApproveComment = c.Level1ApproveComment,
                             Level2ApproveComment = c.Level2ApproveComment,
                             Level1Approved = c.Level1Approved.ToString(),
                             Level2Approved = c.Level2Approved.ToString(),
                             LeaveFromDate_CustomFormat_1 = (!(Convert.ToBoolean(c.IsCompOff)) ? Convert.ToDateTime(c.LeaveFromDate).ToString("mm/dd/yyyy") : ""),
                             LeaveToDate_CustomFormat_1 = (!(Convert.ToBoolean(c.IsCompOff)) ? Convert.ToDateTime(c.LeaveTodate).ToString("mm/dd/yyyy") : ""),
                             DateofAvailableCompoff_CustomFormat_1 = ((Convert.ToBoolean(c.IsCompOff)) ? Convert.ToDateTime(c.DateofAvailableCompoff).ToString("mm/dd/yyyy") : "")
                         }).FirstOrDefault();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HrUpdateLeaveManagement(tbl_Leave task)
        {
            var tasks = myapp.tbl_Leave.Where(t => t.LeaveId == task.LeaveId).ToList();
            if (tasks.Count > 0)
            {
                tasks[0].LeaveTypeId = task.LeaveTypeId;
                tasks[0].LeaveTypeName = task.LeaveTypeName;
                tasks[0].IsCompOff = task.IsCompOff;
                tasks[0].IsFullday = task.IsFullday;
                tasks[0].DateofAvailableCompoff = task.DateofAvailableCompoff;
                tasks[0].LeaveFromDate = task.LeaveFromDate;
                tasks[0].LeaveTodate = task.LeaveTodate;
                tasks[0].ReasonForLeave = task.ReasonForLeave;
                tasks[0].AddressOnLeave = task.AddressOnLeave;
                tasks[0].ModifiedBy = User.Identity.Name;
                tasks[0].ModifiedOn = DateTime.Now;

                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxReportingManagers(JQueryDataTableParamModel param)
        {

            if (Request.IsAuthenticated)
            {
                var query = myapp.tbl_ReportingManager.OrderBy(f => f.UserId).ToList();
                IEnumerable<tbl_ReportingManager> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.ReportingManagerId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||

                                  c.UserName != null && c.UserName.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.SubDepartmentName != null && c.SubDepartmentName.ToLower().Contains(param.sSearch.ToLower())
                                    ||

                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())
                                  );
                }
                else
                {
                    filteredCompanies = query;
                }
                var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayedCompanies
                             select new[] {
                                              c.LocationName,
                                              c.DepartmentName,
                                              c.SubDepartmentName,
                                              c.UserName,
                                              c.IsHod.HasValue?c.IsHod.Value.ToString():"",
                                              c.IsHodOfHod.HasValue?c.IsHodOfHod.Value.ToString():"",
                                              c.IsManagerOfHod.HasValue?c.IsManagerOfHod.Value.ToString():"",
                                              c.ReportingManagerId.ToString()};
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query.Count(),
                    iTotalDisplayRecords = filteredCompanies.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }


        [HttpPost]
        public JsonResult DeleteReportingManagers(int id)
        {
            var tasks = myapp.tbl_ReportingManager.Where(t => t.ReportingManagerId == id).ToList();
            if (tasks.Count > 0)
            {
                myapp.tbl_ReportingManager.Remove(tasks[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SaveReportingManager(tbl_ReportingManager task)
        {
            bool Validate = true;
            string message = "";
            string Employetype = "";
            if (task.IsHod.Value)
            {
                Employetype = "HOD";
            }
            else if (task.IsHodOfHod.Value)
            {
                Employetype = "HeadofHOD";
            }
            else if (task.IsManagerOfHod.Value)
            {
                Employetype = "ManagerOfHod";
            }

            Validate = ValidateReportingManager(task, Employetype, ref message);
            if (Validate)
            {

                task.IsActive = true;
                myapp.tbl_ReportingManager.Add(task);
                myapp.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message, JsonRequestBehavior.AllowGet);
            }
        }

        public bool ValidateReportingManager(tbl_ReportingManager v, string type, ref string message)
        {
            bool Validate = true;
            if (v.SubDepartmentId != 0 && v.SubDepartmentName != "")
            {
                if (type == "HeadofHOD")
                {
                    var Checklist = myapp.tbl_ReportingManager.Where(l => l.LocationId == v.LocationId && l.DepartmentId == v.DepartmentId && l.SubDepartmentId == v.SubDepartmentId && l.IsHodOfHod == true).ToList();
                    if (Checklist.Count > 0)
                    {
                        Validate = false;
                        message = "The " + v.LocationName + " - " + v.DepartmentName + " - " + v.SubDepartmentName + " already assign some one";
                    }
                }
                else if (type == "HOD")
                {
                    var Checklist = myapp.tbl_ReportingManager.Where(l => l.LocationId == v.LocationId && l.DepartmentId == v.DepartmentId && l.SubDepartmentId == v.SubDepartmentId && l.IsHod == true).ToList();
                    if (Checklist.Count > 0)
                    {
                        Validate = false;
                        message = "The " + v.LocationName + " - " + v.DepartmentName + " - " + v.SubDepartmentName + " already assign some one";
                    }
                }
                else if (type == "ManagerOfHod")
                {
                    var Checklist = myapp.tbl_ReportingManager.Where(l => l.LocationId == v.LocationId && l.DepartmentId == v.DepartmentId && l.SubDepartmentId == v.SubDepartmentId && l.IsManagerOfHod == true).ToList();
                    if (Checklist.Count > 0)
                    {
                        Validate = false;
                        message = "The " + v.LocationName + " - " + v.DepartmentName + " - " + v.SubDepartmentName + " already assign some one";
                    }
                }
            }
            else
            {
                if (type == "ManagerOfHod")
                {
                    var Checklist2 = myapp.tbl_ReportingManager.Where(l => l.LocationId == v.LocationId && l.DepartmentId == v.DepartmentId && l.IsManagerOfHod == true).ToList();
                    if (Checklist2.Count > 0)
                    {
                        Validate = false;
                        message = "The " + v.LocationName + " - " + v.DepartmentName + " - " + v.SubDepartmentName + " already assign some one";
                    }
                }
                else if (type == "HeadofHOD")
                {
                    var Checklist2 = myapp.tbl_ReportingManager.Where(l => l.LocationId == v.LocationId && l.DepartmentId == v.DepartmentId && l.IsHodOfHod == true).ToList();
                    if (Checklist2.Count > 0)
                    {
                        Validate = false;
                        message = "The " + v.LocationName + " - " + v.DepartmentName + " - " + v.SubDepartmentName + " already assign some one";
                    }
                }
                else
                {
                    var Checklist2 = myapp.tbl_ReportingManager.Where(l => l.LocationId == v.LocationId && l.DepartmentId == v.DepartmentId && l.IsHod == true).ToList();
                    if (Checklist2.Count > 0)
                    {
                        Validate = false;
                        message = "The " + v.LocationName + " - " + v.DepartmentName + " - " + v.SubDepartmentName + " already assign some one";
                    }
                }
            }
            return Validate;
        }
        public ActionResult ExportToExcelEmployeesLeaveApplications(string location, string department, string FromDate, string ToDate, string Employee, string LeaveType)
        {

            var query = myapp.tbl_Leave.Where(q => q.LeaveStatus == "Approved" || q.LeaveStatus == "Pending");
            if (location != null && location != "")
            {
                int LocationId = int.Parse(location);
                query = query.Where(q => q.LocationId == LocationId);
            }
            if (department != null && department != "")
            {
                int departmentId = int.Parse(department);
                query = query.Where(q => q.DepartmentId == departmentId);
            }
            if (FromDate != null && FromDate != "" && ToDate != null && ToDate != "")
            {
                DateTime frmdate = ProjectConvert.ConverDateStringtoDatetime(FromDate);
                DateTime dtodate = ProjectConvert.ConverDateStringtoDatetime(ToDate);

                query = query.Where(l => (frmdate <= l.LeaveFromDate && dtodate >= l.LeaveFromDate) || (frmdate <= l.LeaveTodate && dtodate >= l.LeaveTodate));

            }

            if (Employee != null && Employee != "")
            {
                query = query.Where(q => q.UserId == Employee);
            }
            if (LeaveType != null && LeaveType != "")
            {
                int LeaveTypeId = int.Parse(LeaveType);
                query = query.Where(q => q.LeaveTypeId == LeaveTypeId);
            }
            var query1 =
               (from c in query.ToList()
                join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId

                select new LeaveViewModels
                {
                    userid = c.UserId,
                    LeaveId = c.LeaveId.ToString(),
                    LeaveTypeName = c.LeaveTypeName,
                    IsFullday = c.IsFullday.ToString(),
                    IsCompOff = c.IsCompOff.ToString(),
                    LeaveFromDate = c.LeaveFromDate.Value.ToString("dd/MM/yyyy"),
                    LeaveTodate = c.LeaveTodate.Value.ToString("dd/MM/yyyy"),
                    LeaveStatus = c.LeaveStatus,
                    Level1Approver = app1.FirstName + " " + app1.LastName,
                    UserName = c.UserName,
                    DepartmentName = c.DepartmentName,
                    LocationName = c.LocationName,
                    DateofAvailableCompoff = (c.DateofAvailableCompoff.HasValue ? c.DateofAvailableCompoff.Value.ToString("dd/MM/yyyy") : ""),
                    ReasonForLeave = c.ReasonForLeave,
                    AddressOnLeave = c.AddressOnLeave,
                    TotalLeaves = c.LeaveCount.HasValue ? c.LeaveCount.Value : 0,

                }).ToList();

            query1 = query1.OrderByDescending(t => t.LeaveId).ToList();


            var products = new System.Data.DataTable("EmployeesLeaveApplications");
            // products.Rows.Add("Client  : " + list[0].ClientName + " SOA From : " + FromDate + " To : " + ToDate);
            //  products.Columns.Add("UserId", typeof(string));
            products.Columns.Add("LeaveId", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("User Id", typeof(string));
            products.Columns.Add("UserName", typeof(string));
            products.Columns.Add("Leave Type", typeof(string));
            products.Columns.Add("From Date", typeof(string));
            products.Columns.Add("To Date", typeof(string));
            products.Columns.Add("NoOf Leaves", typeof(string));
            products.Columns.Add("ReasonForLeave", typeof(string));
            products.Columns.Add("AddressOnLeave", typeof(string));
            products.Columns.Add("Status", typeof(string));
            products.Columns.Add("Approver", typeof(string));

            foreach (var c in query1)
            {
                products.Rows.Add(c.LeaveId.ToString(),
                                 c.LocationName,
                                 c.DepartmentName,
                                 c.userid,
                                 c.UserName,
                                 c.LeaveTypeName,
                                 c.LeaveFromDate,
                                 c.LeaveTodate,
                                 c.TotalLeaves.ToString(),
                                 c.ReasonForLeave,
                                 c.AddressOnLeave,
                                 c.LeaveStatus,
                                 c.Level1Approver
                );

            }


            var grid = new GridView();
            grid.GridLines = GridLines.Both;
            grid.BorderStyle = BorderStyle.Solid;

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


        public ActionResult ExportToExcelEmployeesLeaveTypeData()
        {

            List<LeavesCountView> Leavscount = new List<LeavesCountView>();

            var query = myapp.tbl_ManageLeave.ToList();
            var newquery = query.GroupBy(u => u.UserId).ToList();
            foreach (var v in newquery)
            {
                LeavesCountView lv = new LeavesCountView();
                lv.UserId = v.Key;
                foreach (var vs in v.ToList())
                {
                    lv.UserName = vs.UserName;
                    lv.LocationId = vs.LocationId.HasValue ? vs.LocationId.Value : 0;
                    lv.DepartmentId = vs.DepartmentId.HasValue ? vs.DepartmentId.Value : 0;
                    lv.LocationName = vs.LocationName;
                    lv.DepartmentName = vs.DepartmentName;
                    if (vs.LeaveTypeId == 1)
                    {
                        lv.CasuvalAvailableLeave = vs.AvailableLeave;

                    }
                    if (vs.LeaveTypeId == 4)
                    {
                        lv.SickAvailableLeave = vs.AvailableLeave;
                    }
                    if (vs.LeaveTypeId == 5)
                    {
                        lv.EarnedAvailableLeave = vs.AvailableLeave;
                    }
                    if (vs.LeaveTypeId == 6)
                    {
                        lv.CompoffBalance = vs.AvailableLeave;
                    }
                    lv.IsActive = vs.IsActive;
                }
                Leavscount.Add(lv);
            }


            Leavscount = (from q in Leavscount
                          orderby int.Parse(q.UserId.ToLower().Replace("fh_", ""))
                          select q).ToList();
            var products = new System.Data.DataTable("EmployeesLeaveApplications");
            // products.Rows.Add("Client  : " + list[0].ClientName + " SOA From : " + FromDate + " To : " + ToDate);
            //  products.Columns.Add("User Id", typeof(string));
            //products.Columns.Add("User Leave Id", typeof(string));
            products.Columns.Add("User Id", typeof(string));
            products.Columns.Add("Location Name", typeof(string));
            products.Columns.Add("Department Name", typeof(string));
            products.Columns.Add("User Name", typeof(string));
            products.Columns.Add("Casuval Leave", typeof(string));
            products.Columns.Add("Sick Leave", typeof(string));
            products.Columns.Add("Earned Leave", typeof(string));
            products.Columns.Add("Comp Off", typeof(string));
            //products.Columns.Add("IsActive", typeof(string));
            foreach (var c in Leavscount)
            {
                products.Rows.Add(
                                 //c.UserLeaveId.ToString(),
                                 c.UserId,
                                 c.LocationName,
                                 c.DepartmentName,
                                 c.UserName,
                                 c.CasuvalAvailableLeave,
                                 c.SickAvailableLeave,
                                 c.EarnedAvailableLeave,
                                 c.CompoffBalance

                //Convert.ToBoolean(c.IsActive).ToString()
                );

            }


            var grid = new GridView();
            grid.GridLines = GridLines.Both;
            grid.BorderStyle = BorderStyle.Solid;

            grid.HeaderStyle.BackColor = System.Drawing.Color.Teal;
            grid.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grid.DataSource = products;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            string filename = "EmployeesLeaveTypeData.xls";
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

        public ActionResult ExportToExcelallcompoffsAdmin()
        {
            var query = myapp.tbl_RequestCompOffLeave.ToList();
            var products = new System.Data.DataTable("Compoff");
            // products.Rows.Add("Client  : " + list[0].ClientName + " SOA From : " + FromDate + " To : " + ToDate);
            //  products.Columns.Add("User Id", typeof(string));
            //products.Columns.Add("User Leave Id", typeof(string));
            products.Columns.Add("User Id", typeof(string));
            products.Columns.Add("Location Name", typeof(string));
            products.Columns.Add("Department Name", typeof(string));
            products.Columns.Add("User Name", typeof(string));
            products.Columns.Add("CompOff Date", typeof(string));
            products.Columns.Add("Reason", typeof(string));
            products.Columns.Add("Status", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            //products.Columns.Add("IsActive", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                                 //c.UserLeaveId.ToString(),
                                 c.UserId.ToLower().Replace("fh_", ""),
                                 c.LocationName,
                                 c.DepartmentName,
                                 c.UserName,
                                 c.CompOffDateTime.HasValue ? c.CompOffDateTime.Value.ToString("dd/MM/yyyy") : "",
                                 c.RequestReason,
                                 c.Leave_Status,
                                 c.CreatedDateTime.Value.ToString("dd/MM/yyyy")

                //Convert.ToBoolean(c.IsActive).ToString()
                );

            }


            var grid = new GridView();
            grid.GridLines = GridLines.Both;
            grid.BorderStyle = BorderStyle.Solid;

            grid.HeaderStyle.BackColor = System.Drawing.Color.Teal;
            grid.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grid.DataSource = products;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            string filename = "EmployeesCompoffrequest.xls";
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


        public async Task<JsonResult> GetAllUserEmailListToAdmin_HRAdmin()
        {
            GlobalDataFunctions _GlobalFun = new GlobalDataFunctions();
            List<UserEmailList_RelationalClass> ReturnData = await _GlobalFun.GetAllUserEmailListToAdmin_GlobalFunDA("");
            return Json(ReturnData, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetAllUserListToAdmin_HRAdmin()
        {
            List<tbl_User> ReturnData = new List<tbl_User>();
            if (Request.IsAuthenticated)
            {
                ReturnData = await myapp.tbl_User.ToListAsync();
                await Task.Delay(0);
            }
            return Json(ReturnData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeactivateEmployee(int UserId)
        {
            var userlist = myapp.tbl_User.Where(e => e.UserId == UserId).ToList();
            if (userlist.Count > 0)
            {
                userlist[0].IsActive = false;
                myapp.SaveChanges();
                return Json("Successfully DeActivated", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Error Occured While DeActivating User", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult EditUserDataByUserID_HRAdmin(string Data)
        {
            string ReturnData = "Error Occured While Updating User Data";
            if (Data != null)
            {
                JavaScriptSerializer JS = new JavaScriptSerializer();
                UserViewModel NewUserData = JS.Deserialize<UserViewModel>(Data);
                List<tbl_User> UserListData = myapp.tbl_User.Where(e => e.UserId == NewUserData.UserId).ToList();
                if (UserListData.Count() > 0)
                {
                    UserListData[0].FirstName = NewUserData.FirstName;
                    UserListData[0].EmailId = NewUserData.EmailId;
                    UserListData[0].PhoneNumber = NewUserData.PhoneNumber;
                    UserListData[0].Gender = NewUserData.Gender;
                    if (NewUserData.DateOfBirth != null && NewUserData.DateOfBirth != "")
                    {
                        try
                        {
                            UserListData[0].DateOfBirth = ProjectConvert.ConverDateStringtoDatetime(NewUserData.DateOfBirth);
                        }
                        catch { }
                    }
                    if (NewUserData.DateOfJoining != null && NewUserData.DateOfJoining != "")
                    {
                        try
                        {
                            UserListData[0].DateOfJoining = ProjectConvert.ConverDateStringtoDatetime(NewUserData.DateOfJoining);
                        }
                        catch { }
                    }

                    UserListData[0].DesignationID = NewUserData.DesignationID;
                    UserListData[0].Designation = NewUserData.Designation;
                    UserListData[0].UserType = NewUserData.UserType;
                    UserListData[0].aadharCard = NewUserData.AdhaarCard;
                    UserListData[0].PanCard = NewUserData.PanCard;
                    UserListData[0].ReportingManagerId = NewUserData.ReportingManagerId;
                    UserListData[0].IsOnRollDoctor = NewUserData.IsOnRollDoctor;
                    UserListData[0].IsOffRollDoctor = NewUserData.IsOffRollDoctor;
                    UserListData[0].Qualification = NewUserData.Qualification;
                    UserListData[0].CollegeName = NewUserData.CollageName;
                    myapp.SaveChangesAsync();
                    string CustomUserId = UserListData[0].CustomUserId;
                    try
                    {
                        var user = UserManager.Users.Single(s => s.UserName == CustomUserId);
                        bool isupdate = false;
                        if (NewUserData.PhoneNumber != null && NewUserData.PhoneNumber != "")
                        {
                            user.PhoneNumber = NewUserData.PhoneNumber;
                            user.PhoneNumberConfirmed = true;
                            isupdate = true;
                        }
                        if (NewUserData.EmailId != null && NewUserData.EmailId != "")
                        {
                            user.Email = NewUserData.EmailId;
                            user.EmailConfirmed = true;
                            isupdate = true;
                        }
                        if (isupdate)
                        {
                            UserManager.UpdateAsync(user);
                        }
                    }
                    catch
                    {

                    }
                    ReturnData = "Successfully Updated";
                }
            }
            return Json(ReturnData, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportToExcelEmployeesWhoHaveLogin()
        {
            var listuser = myapp.tbl_User.ToList();
            var aspuser = myapp.AspNetUsers.ToList();
            var list = (from m in aspuser
                        from p in listuser
                        where m.UserName == p.CustomUserId
                        select p).Distinct().ToList();

            var listnonActive = listuser.Except(list).ToList();
            var products = new System.Data.DataTable("EmployeesLogin");

            products.Columns.Add("Location Name", typeof(string));
            products.Columns.Add("Department Name", typeof(string));
            products.Columns.Add("Employee Id", typeof(string));
            products.Columns.Add("Employee Name", typeof(string));
            products.Columns.Add("Is Login", typeof(string));

            foreach (var c in listnonActive)
            {
                products.Rows.Add(c.LocationName,
                                 c.DepartmentName,
                                 c.CustomUserId.Replace("Fh_", ""),
                                 c.FirstName,
                                 "No"
                );
            }
            foreach (var c in list)
            {
                products.Rows.Add(c.LocationName,
                                 c.DepartmentName,
                                 c.CustomUserId.Replace("Fh_", ""),
                                 c.FirstName,
                                 "Yes"
                );
            }

            var grid = new GridView();
            grid.GridLines = GridLines.Both;
            grid.BorderStyle = BorderStyle.Solid;

            grid.HeaderStyle.BackColor = System.Drawing.Color.Teal;
            grid.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grid.DataSource = products;
            grid.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            string filename = "EmployeesLogin.xls";
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
        public JsonResult GetEmployeeHistory(int id)
        {
            var l = (from m in myapp.tbl_User_History
                     where m.UserId == id
                     select m).ToList();

            var list = (from m in l
                        select new RelationalClass
                        {
                            DepartmentId = m.DepartmentId.Value,
                            DepartmentName = m.DepartmentName,
                            LocationId = m.LocationId.Value,
                            LocationName = m.LocationName,
                            MovedDate = m.CreatedOn.Value.ToString("dd/MM/yyyy"),
                            UpdatedBy = m.CreatedBy

                        }).ToList();
            var subs = (from m in myapp.tbl_User
                        where m.UserId == id
                        select m).ToList();
            var sublist = (from m in subs
                           select new RelationalClass
                           {
                               DepartmentId = m.DepartmentId.Value,
                               DepartmentName = m.DepartmentName,
                               LocationId = m.LocationId.Value,
                               LocationName = m.LocationName,
                               MovedDate = m.CreatedOn.Value.ToString("dd/MM/yyyy"),
                               UpdatedBy = m.CreatedBy

                           }).AsEnumerable();
            list.AddRange(sublist);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRemarksOfEmployee(int id)
        {
            var l = (from m in myapp.tbl_User
                     where m.UserId == id
                     select m).ToList();
            return Json(l[0], JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRemarksOfEmployee(int id, string Remarks)
        {
            var l = (from m in myapp.tbl_User
                     where m.UserId == id
                     select m).ToList();
            l[0].Comments = Remarks;
            if (Remarks.ToLower() == "hold")
            {
                l[0].IsActive = false;
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateFoodTypeOfEmployee(int id, string FoodType)
        {
            var l = (from m in myapp.tbl_User
                     where m.UserId == id
                     select m).ToList();
            l[0].FoodType = FoodType;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult HrViewCompOff()
        {
            return View();
        }
        public ActionResult HrViewCompOffEncash()
        {
            return View();
        }
        public JsonResult UpdateCompOffEncash(int reqid, string remarks, bool Approved, string Date, string status)
        {
            var cmp = myapp.tbl_CompOffEncash.Where(l => l.CompOffEncashId == reqid).SingleOrDefault();
            if (cmp != null)
            {
                cmp.ModifiedBy = User.Identity.Name;
                cmp.IsApproved = false;
                cmp.HRApproved = Approved;
                cmp.Status = status;
                cmp.ModifiedOn = DateTime.Now;
                if (Date != null && Date != "")
                    cmp.HrEnchashedDate = ProjectConvert.ConverDateStringtoDatetime(Date);
                cmp.HrRemarks = remarks;
                //myapp.tbl_CompOffEncash.Add(cmp);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetHrCompOffEncashes(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                var userslist = myapp.tbl_User.ToList();
                var list = (from v in myapp.tbl_CompOffEncash select v).ToList();
                var query = (from l in list
                             join usr in userslist on l.ReportingTo equals usr.CustomUserId
                             join usr2 in userslist on l.UserId equals usr2.CustomUserId
                             select new CompOffEncashViewModel
                             {
                                 CompOffEncashId = l.CompOffEncashId,
                                 DepartmentId = usr2.DepartmentId,
                                 DepartmentName = usr2.DepartmentName,
                                 HRApproved = l.HRApproved.HasValue ? l.HRApproved.Value.ToString() : "",
                                 HrEnchashedDate = l.HrEnchashedDate.HasValue ? l.HrEnchashedDate.Value.ToString("dd/MM/yyyy") : "",
                                 HrRemarks = l.HrRemarks,
                                 IsApproved = l.IsApproved.HasValue ? l.IsApproved.Value.ToString() : "",
                                 LocationId = usr2.LocationId,
                                 LocationName = usr2.LocationName,
                                 ModifiedBy = l.ModifiedBy,
                                 ModifiedOn = l.ModifiedOn.HasValue ? l.ModifiedOn.Value.ToString("dd/MM/yyyy") : "",
                                 NoOfDays = l.NoOfDays,
                                 Remarks = l.Remarks,
                                 ReportingTo = l.ReportingTo,
                                 ReportingToName = usr.FirstName,
                                 Status = l.Status,
                                 SubmitedOn = l.SubmitedOn.HasValue ? l.SubmitedOn.Value.ToString("dd/MM/yyyy") : "",
                                 UserId = l.UserId,
                                 UserName = usr2.FirstName
                             }).ToList();

                IEnumerable<CompOffEncashViewModel> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.CompOffEncashId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||

                                  c.UserName != null && c.UserName.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.ReportingToName != null && c.ReportingToName.ToLower().Contains(param.sSearch.ToLower())
                                    ||

                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())
                                  );
                }
                else
                {
                    filteredCompanies = query;
                }
                var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayedCompanies
                             select new[] {
                                              c.LocationName,
                                              c.DepartmentName,
                                              c.UserId.ToLower().Replace("fh_",""),
                                              c.UserName,
                                              c.SubmitedOn,
                                              c.NoOfDays.ToString(),
                                              c.Remarks,
                                               c.ReportingToName,
                                              c.IsApproved,
                                              c.HRApproved,
                                              c.HrRemarks,
                                              c.HrEnchashedDate,
                                              c.Status,
                                              c.CompOffEncashId.ToString()
                             };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query.Count(),
                    iTotalDisplayRecords = filteredCompanies.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }

        public JsonResult ValidateTheUserIsReportingManagerOrNot(int empid)
        {
            var list = myapp.tbl_User.Where(l => l.UserId == empid).ToList();
            if (list.Count > 0)
            {
                string userid = list[0].CustomUserId;
                var list2 = myapp.tbl_ReportingManager.Where(l => l.UserId == userid).ToList();
                if (list2.Count > 0)
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ViewReportingEmployees()
        {
            return View();
        }
        public ActionResult AjaxGetReprotingEmployees(JQueryDataTableParamModel param)
        {

            if (Request.IsAuthenticated)
            {
                var query = GetListOfEmployees("Edit", param.Emp);

                IEnumerable<tbl_User> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.CustomUserId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.FirstName != null && c.FirstName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.LastName != null && c.LastName.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.LastName != null && c.LastName.ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                  c.EmailId != null && c.EmailId.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.PlaceAllocation != null && c.PlaceAllocation.ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                  c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())
                                  );
                }
                else
                {
                    filteredCompanies = query;
                }
                var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayedCompanies
                             select new[] {/*c.UserId.ToString(),*/
                                              c.LocationName,
                                              c.FirstName+" "+c.LastName,
                                              c.CustomUserId.ToLower().Replace("fh_",""),
                                              c.EmailId,
                                              //c.PlaceAllocation,
                                              c.PhoneNumber,
                                              //c.Extenstion,
                                              c.DepartmentName,
                                              c.SubDepartmentName,
                                              c.Designation
                                             };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query.Count(),
                    iTotalDisplayRecords = filteredCompanies.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }

        public List<tbl_User> GetListOfEmployees(string typeofview, string userid)
        {
            var userslist = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            List<tbl_User> userslistnew = new List<tbl_User>();
            string Currenrid = userid;
            var currentuser = userslist.Where(l => l.CustomUserId == Currenrid).ToList();

            var reportingmgr = myapp.tbl_ReportingManager.Where(l => l.UserId == Currenrid).ToList();

            foreach (var v in reportingmgr)
            {
                if (v.IsHod.Value)
                {
                    var query = (from u in userslist
                                 where u.LocationId == v.LocationId &&
                                 u.DepartmentId == v.DepartmentId
                                 && u.UserType.ToLower() == "employee"
                                 select u).ToList();
                    var listq = query.AsEnumerable();
                    userslistnew.AddRange(listq);
                }
                else if (v.IsHodOfHod.Value)
                {
                    var query = (from u in userslist
                                 where u.LocationId == v.LocationId &&
                                 u.DepartmentId == v.DepartmentId
                                 && u.UserType.ToLower() == "hod"
                                 select u).ToList();
                    var listq = query.AsEnumerable();
                    userslistnew.AddRange(listq);

                }
                else if (v.IsManagerOfHod.Value)
                {
                    var query = (from u in userslist
                                 where u.LocationId == v.LocationId &&
                                 u.DepartmentId == v.DepartmentId
                                 && u.UserType.ToLower() == "headofhod"
                                 select u).ToList();
                    var listq = query.AsEnumerable();
                    userslistnew.AddRange(listq);

                }

            }

            if (currentuser.Count > 0 && currentuser[0].UserType == "HOD")
            {
                var query = (from u in userslist
                                 //from rm in reportingmgr
                             where u.LocationId == currentuser[0].LocationId &&
                             u.DepartmentId == currentuser[0].DepartmentId &&
                             (u.UserType == "HOD" || u.UserType == "Employee")
                             select u).ToList();

                userslistnew.AddRange(query);

            }
            else if (currentuser.Count > 0 && currentuser[0].UserType.ToLower().Replace(" ", "") == "headofhod")
            {
                var query = (from u in userslist
                                 //from rm in reportingmgr
                             where u.LocationId == currentuser[0].LocationId &&
                             u.DepartmentId == currentuser[0].DepartmentId &&
                             (u.UserType == "HOD" || u.UserType == "HeadofHOD")
                             select u).ToList();
                userslistnew.AddRange(query);

            }
            else if (currentuser.Count > 0 && currentuser[0].UserType.ToLower().Replace(" ", "") == "headofmanager")
            {
                var query = (from u in userslist
                                 //from rm in reportingmgr
                             where (u.LocationId == currentuser[0].LocationId && u.DepartmentId == currentuser[0].DepartmentId && (u.UserType == "HeadofHOD"))

                             select u).ToList();
                userslistnew.AddRange(query);

            }
            //else if (currentuser.Count > 0 && currentuser[0].IsEmployeesReporting.Value)
            //{
            //    var emplist = myapp.tbl_AssignEmployeesToManager.Where(m => m.ManagerEmployeeId == Currenrid).ToList();

            //    var query = (from us in userslist
            //                 from emp in emplist
            //                 where us.CustomUserId.ToLower() == emp.EmployeeId.ToLower()
            //                 select us).ToList();
            //    userslistnew.AddRange(query);
            //}
            else
            {
                if (reportingmgr.Count == 0)
                {
                    userslistnew = userslist.Where(l => l.CustomUserId == userid).ToList();

                }
                else
                {
                    var query = (from u in userslist
                                     //from rm in reportingmgr
                                 where u.LocationId == currentuser[0].LocationId && u.DepartmentId == currentuser[0].DepartmentId
                                 select u).ToList();
                    //userslist = query.Distinct().ToList();
                    userslistnew.AddRange(query);
                }
            }

            userslistnew = userslistnew.Distinct().ToList();
            userslistnew = (from q in userslistnew
                            orderby int.Parse(q.CustomUserId.ToLower().Replace("fh_", ""))
                            select q).ToList();
            return userslistnew;
        }
        public ActionResult AjaxGetEmployeesHold(JQueryDataTableParamModel param)
        {

            if (Request.IsAuthenticated)
            {
                var query = myapp.tbl_User.Where(a => a.Comments != null && a.Comments.ToLower() == "hold").ToList();

                query = (from q in query
                         orderby int.Parse(q.CustomUserId.ToLower().Replace("fh_", ""))
                         select q).ToList();
                IEnumerable<tbl_User> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.CustomUserId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.FirstName != null && c.FirstName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.LastName != null && c.LastName.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.LastName != null && c.LastName.ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                  c.EmailId != null && c.EmailId.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.PlaceAllocation != null && c.PlaceAllocation.ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                  c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())
                                  );
                }
                else
                {
                    filteredCompanies = query;
                }
                var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayedCompanies
                             select new[] {c.UserId.ToString(),
                                              c.LocationName,
                                              c.FirstName+" "+c.LastName,
                                              c.CustomUserId.ToLower().Replace("fh_",""),
                                              c.EmailId,
                                              //c.PlaceAllocation,
                                              c.PhoneNumber,
                                              //c.Extenstion,
                                              c.DepartmentName,

                                              c.Designation,
                                                (from V in myapp.tbl_User where V.EmpId == c.ReportingManagerId select V.FirstName+" "+V.EmpId).SingleOrDefault()
                                             };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query.Count(),
                    iTotalDisplayRecords = filteredCompanies.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }

        public JsonResult UpdateApploginstatus(int id, bool status)
        {
            var user = myapp.tbl_User.Where(l => l.UserId == id).ToList();
            if (user.Count > 0)
            {
                user[0].IsAppLogin = status;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateServiceAnniversarystatus(int id, bool status)
        {
            var user = myapp.tbl_User.Where(l => l.UserId == id).ToList();
            if (user.Count > 0)
            {
                user[0].SendRemainder = status;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeInfoById(int id)
        {

            var user = myapp.tbl_User.Where(l => l.UserId == id).ToList();
            List<UserViewModel> nuser = new List<UserViewModel>();
            try
            {
                nuser = (from u in user
                         select new UserViewModel
                         {
                             UserId = u.UserId,
                             FirstName = u.FirstName,
                             LastName = u.LastName,
                             CustomUserId = u.CustomUserId,
                             DateOfBirth = u.DateOfBirth.HasValue ? u.DateOfBirth.Value.ToString("dd/MM/yyyy") : "",
                             DateOfJoining = u.DateOfJoining.HasValue ? u.DateOfJoining.Value.ToString("dd/MM/yyyy") : "",
                             EmailId = u.EmailId,
                             PhoneNumber = u.PhoneNumber,
                             Gender = u.Gender,
                             Extenstion = u.Extenstion,
                             LocationId = u.LocationId,
                             LocationName = u.LocationName,
                             DepartmentId = u.DepartmentId,
                             DepartmentName = u.DepartmentName,
                             Designation = u.Designation,
                             PlaceAllocation = u.PlaceAllocation,
                             UserType = u.UserType,
                             SecurityQuestion = u.SecurityQuestion,
                             SecurityAnswner = u.SecurityAnswner,
                             Comments = u.Comments,
                             IsActive = u.IsActive,
                             CreatedOn = u.CreatedOn.HasValue ? u.CreatedOn.Value.ToString("dd/MM/yyyy") : "",
                             CreatedBy = u.CreatedBy,
                             SubDepartmentId = u.SubDepartmentId,
                             SubDepartmentName = u.SubDepartmentName,
                             DateOfLeaving = u.DateOfLeaving.HasValue ? u.DateOfLeaving.Value.ToString("dd/MM/yyyy") : "",
                             DesignationID = u.DesignationID,
                             ChangePassword = u.ChangePassword,
                             IsEmployeesReporting = u.IsEmployeesReporting,
                             EmpId = u.EmpId,
                             IsAppLogin = u.IsAppLogin,
                             IsEmployee = u.IsEmployee.HasValue ? u.IsEmployee.Value : false,
                             IsOffRollDoctor = u.IsOffRollDoctor.HasValue ? u.IsOffRollDoctor.Value : false,
                             IsOnRollDoctor = u.IsOnRollDoctor.HasValue ? u.IsOnRollDoctor.Value : false,
                             photo = u.EmployeePhoto,
                             AdhaarCard = u.aadharCard,
                             PanCard = u.PanCard,
                             Qualification=u.Qualification,
                             CollageName=u.CollegeName,
                             ReportingManagerId = u.ReportingManagerId.HasValue ? u.ReportingManagerId.Value : 0,
                             strtbl_ReportingManager = u.ReportingManagerId.HasValue ? (from V in myapp.tbl_User where V.EmpId == u.ReportingManagerId select V.FirstName + " " + V.EmpId).SingleOrDefault() : ""

                         }).ToList();
            }
            catch (Exception ex)
            {

            }
            return Json(nuser[0], JsonRequestBehavior.AllowGet);
        }

        public ActionResult UploadImage(int id, HttpPostedFileBase ImageFile)
        {
            List<tbl_User> UserListData = myapp.tbl_User.Where(e => e.UserId == id).ToList();
            string FileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);

            //To Get File Extension  
            string FileExtension = Path.GetExtension(ImageFile.FileName);

            //Add Current Date To Attached File Name  
            FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

            //Get Upload path from Web.Config file AppSettings.  
            string UploadPath = Server.MapPath("~/Documents/Images/");

            //Its Create complete path to store in server.  
            var ImagePath = UploadPath + FileName;

            //To copy and save file into server.  
            ImageFile.SaveAs(ImagePath);

            UserListData[0].EmployeePhoto = FileName;
            myapp.SaveChanges();
            return Json("Successfully Updated", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExporttoExcelRoster(string location, string department, string FromDate, string ToDate, string Employee)
        {
            int Locationid = int.Parse(location);
            int Departmentid = int.Parse(department);
            DateTime Dtfromdate = ProjectConvert.ConverDateStringtoDatetime(FromDate);
            DateTime Dttodate = ProjectConvert.ConverDateStringtoDatetime(ToDate);
            var employees = myapp.tbl_User.Where(l => l.LocationId == Locationid && l.DepartmentId == Departmentid && l.IsActive == true).ToList();
            if (Employee != null && Employee != "" && Employee != "0")
            {
                employees = employees.Where(l => l.CustomUserId == Employee).ToList();
            }
            var shifts = myapp.uspGetRoster(User.Identity.Name, Dtfromdate, Dttodate, Locationid, Departmentid).ToList();
            var newlist = (from l in shifts
                           select new
                           {
                               EmployeeId = l.EmployeeId,
                               Fromdate = l.Fromdate.Value.ToString("dd/MM/yyyy"),
                               ShiftId = l.ShiftId,
                               ShiftTypeName = l.ShiftTypeName,
                               IsHoliday = l.IsHoliday.Value,
                               //CheckLeave = CheckIsLeaveApplied(l.UserId, l.ShiftDate.Value.Date),
                               LeaveType = l.LeaveType
                           }).ToList();

            List<string> DateString = new List<string>();
            DateTime dtst = Dtfromdate;

            while (dtst <= Dttodate)
            {
                DateString.Add(dtst.ToString("dd/MM/yyyy"));
                dtst = dtst.AddDays(1);
            }
            var products = new System.Data.DataTable("ExporttoExcelRoster");
            // products.Rows.Add("Client  : " + list[0].ClientName + " SOA From : " + FromDate + " To : " + ToDate);
            //  products.Columns.Add("UserId", typeof(string));
            products.Columns.Add("Name", typeof(string));
            products.Columns.Add("Employee Id", typeof(string));
            foreach (var va in DateString)
            {
                products.Columns.Add(va, typeof(string));
            }
            foreach (var c in employees)
            {
                List<string> paramslist = new List<string>();
                paramslist.Add(c.FirstName);
                paramslist.Add(c.CustomUserId.Replace("FH_", ""));
                foreach (var va in DateString)
                {
                    var res = newlist.Where(l => l.Fromdate == va && l.EmployeeId == c.CustomUserId).ToList();
                    if (res.Count > 0)
                    {
                        if (res[0].LeaveType != null && res[0].LeaveType != "")
                        {
                            paramslist.Add(res[0].ShiftTypeName + " " + res[0].LeaveType + " ");
                        }
                        else
                            paramslist.Add(res[0].ShiftTypeName);
                    }
                    else
                    {
                        paramslist.Add("");
                    }
                }
                products.Rows.Add(paramslist.ToArray());
            }
            var grid = new GridView();
            grid.GridLines = GridLines.Both;
            grid.BorderStyle = BorderStyle.Solid;
            grid.HeaderStyle.BackColor = System.Drawing.Color.Teal;
            grid.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            string filename = "ExporttoExcelRoster.xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + filename);
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            GridViewRow HeaderRow = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            TableCell HeaderCell2 = new TableCell();
            DateTime DtNow = DateTime.Now;
            HeaderCell2.Text = "Duty Roster From - " + FromDate + " To " + ToDate;
            HeaderCell2.ColumnSpan = 15;
            HeaderRow.Cells.Add(HeaderCell2);
            if (grid.Controls.Count > 0)
                grid.Controls[0].Controls.AddAt(0, HeaderRow);
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");
        }
        public JsonResult AjaxGetEmployeesInOracle()
        {
            ConnectOracle conor = new ConnectOracle();
            //conor.Getcompany();
            //conor.Getsubdepts();
            //conor.Getdesignation();

            var query = conor.GetEmployees();

            return Json(query, JsonRequestBehavior.AllowGet);
            //IEnumerable<emp_details> filteredCompanies;
            //if (!string.IsNullOrEmpty(param.sSearch))
            //{
            //    filteredCompanies = query
            //       .Where(c => c.empcode.ToString().ToLower().Contains(param.sSearch.ToLower())
            //                   ||
            //                    c.empnm != null && c.empnm.ToString().ToLower().Contains(param.sSearch.ToLower())
            //                   ||
            //                  c.emailid != null && c.emailid.ToString().ToLower().Contains(param.sSearch.ToLower())

            //                  );
            //}
            //else
            //{
            //    filteredCompanies = query;
            //}
            //var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            //var result = from c in displayedCompanies
            //             select new[] {c.empcode,
            //                                  c.empnm,
            //                                  c.companynm,
            //                                  c.sdeptid,
            //                                  c.designationid,                                             
            //                                  c.dob,                                              
            //                                  c.doj,
            //                                  c.emailid,
            //                                  c.empcode
            //                                 };
            //return Json(new
            //{
            //    sEcho = param.sEcho,
            //    iTotalRecords = query.Count(),
            //    iTotalDisplayRecords = filteredCompanies.Count(),
            //    aaData = result
            //}, JsonRequestBehavior.AllowGet);

        }
        public JsonResult GeneratePin(int userId)
        {
            var SecurityAnswner = string.Empty;
            Random rand = new Random();
            var random = rand.Next(999999, 10000000);
            SecurityAnswner = random.ToString();
            var user = myapp.tbl_User.Where(l => l.UserId == userId).ToList();
            if (user.Count > 0)
            {
                var checkUser = myapp.tbl_User.Where(l => l.SecurityAnswner == SecurityAnswner).FirstOrDefault();
                if (checkUser == null)
                {
                    random = random + rand.Next(0, 100);
                    SecurityAnswner = random.ToString();
                }
                user[0].SecurityQuestion = "PIN";
                user[0].SecurityAnswner = SecurityAnswner;
                myapp.SaveChanges();
            }
            return Json(SecurityAnswner, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintCard(int id)
        {
            var user = myapp.tbl_User.Where(l => l.UserId == id).SingleOrDefault();
            return View(user);
        }
    }
}