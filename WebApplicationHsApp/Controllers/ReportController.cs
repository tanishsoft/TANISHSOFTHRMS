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
    public class ReportController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult empleaveappreport()
        {
            return View();
        }
        public JsonResult GetTaskCountByUser()
        {
            string user = User.Identity.Name;
            var list = myapp.tbl_Task.Where(t => t.IsActive == true && (t.CreatedBy == user)).GroupBy(n => n.AssignStatus).Select(group => new { Key = group.Key, Value = group.Count() });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeesCountByLocation()
        {
            var list = myapp.tbl_User.Where(t => t.IsActive == true).GroupBy(n => n.LocationName).Select(group => new { Key = group.Key, Value = group.Count() });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeesCountByDepartment(string location)
        {
            var list = myapp.tbl_User.Where(t => t.IsActive == true && t.LocationName == location).GroupBy(n => n.DepartmentName).Select(group => new { Key = group.Key, Value = group.Count() });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTodayLeaveCountByLocation()
        {
            var date = DateTime.Now.Date;
            var list = myapp.tbl_Leave.Where(t => t.IsActive == true && t.LeaveStatus.ToUpper() == "APPROVED"
                && (t.LeaveFromDate.Value <= date && t.LeaveTodate.Value >= date)
                ).ToList();
            var employeslist = GetListOfEmployees("");
            list = (from s in list
                    join emp in employeslist on s.UserId equals emp.CustomUserId
                    select s).ToList();
            var values = list.GroupBy(n => n.LocationName).Select(group => new { Key = group.Key, Value = group.Count() });
            return Json(values, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLeavesCountByDepartment(string location)
        {
            var date = DateTime.Now.Date;
            var list = myapp.tbl_Leave.Where(t => t.IsActive == true && t.LocationName.ToUpper() == location.ToUpper() && t.LeaveStatus.ToUpper() == "APPROVED" && (t.LeaveFromDate.Value <= date && t.LeaveTodate.Value >= date)).ToList();
            var employeslist = GetListOfEmployees("");
            list = (from s in list
                    join emp in employeslist on s.UserId equals emp.CustomUserId
                    select s).ToList();
            var values = list.GroupBy(n => n.DepartmentName).Select(group => new { Key = group.Key, Value = group.Count() });
            return Json(values, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAttritionrate()
        {
            Dictionary<string,Decimal> Value = new Dictionary<string, Decimal>();
            var Date = DateTime.Now.AddMonths(-1);
            var month = Date.Month;
            var year = Date.Year;
            var SixmonthsBack = DateTime.Now.AddMonths(-13);
            var Monthsix = SixmonthsBack.Month;
            var Yearsix = SixmonthsBack.Year;
            var MonthLast = 13;
            DateTime date = DateTime.Now;
           var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            firstDayOfMonth.AddMonths(-1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(-1).AddDays(-1);
            var ActualEmployees = myapp.tbl_User.Where(l => l.DateOfJoining < lastDayOfMonth && l.IsActive == true).Count();
            var AYear = Yearsix;
            for (int i= Monthsix; i<= MonthLast; i++)
            {
               
                if (i > 12)
                {
                    i = 1;
                    MonthLast = month;
                    AYear = year;
                }
                var startDate = new DateTime(AYear,i, 1);
                var EndDate = startDate.AddMonths(1).AddDays(-1);
                var Attritions = myapp.tbl_User.Where(l => l.DateOfLeaving >= startDate && l.DateOfLeaving <= EndDate).Count();
                var NewJoin = myapp.tbl_User.Where(l => l.DateOfJoining >= startDate && l.DateOfJoining <= EndDate && l.IsActive == true).Count();
                Decimal Attritionrate = (((decimal)Attritions * 100) / ((decimal)ActualEmployees + (decimal)NewJoin));
                Attritionrate= Math.Round(Attritionrate, 3);
                Value.Add(startDate.ToString("MMM")+"-"+startDate.ToString("yy"), Attritionrate);
            }
            var List = Value.ToList();
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetheadcountofEmployeefromlastYear()
        {
            Dictionary<string, int> Value = new Dictionary<string, int>();
            var Date = DateTime.Now.AddMonths(-1);
            var month = Date.Month;
            var year = Date.Year;
            var SixmonthsBack = DateTime.Now.AddMonths(-12);
            var Monthsix = SixmonthsBack.Month;
            var Yearsix = SixmonthsBack.Year;
            var MonthLast = 13;
            var AYear = Yearsix;
            for (int i = Monthsix; i <= MonthLast; i++)
            {

                if (i > 12)
                {
                    i = 1;
                    MonthLast = month;
                    AYear = year;
                }
                var startDate = new DateTime(AYear, i, 1);
                var EndDate = startDate.AddMonths(1).AddDays(-1);
                var Employees = myapp.tbl_User.Where(l => l.DateOfJoining <= EndDate && l.IsActive == true).Count();
                Value.Add(EndDate.ToString("MMM") + "-" + EndDate.ToString("yy"), Employees);
            }
            var List = Value.ToList();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeCountbyGender()
        {
            Dictionary<string, int> Value = new Dictionary<string, int>();

            var FemaleCount = myapp.tbl_User.Where(l => l.Gender == "F" && l.IsActive == true).Count();
            Value.Add("Female", FemaleCount);
            var maleCount = myapp.tbl_User.Where(l => l.Gender == "M" && l.IsActive == true).Count();
            Value.Add("Male", maleCount);
            var List = Value.ToList();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeCountbyAge()
        {
            var count = myapp.tbl_User.Where(p => p.DateOfBirth != null).ToList()
   .Select(p => new { UserId = p.UserId, Age = DateTime.Now.Year - p.DateOfBirth.Value.Year })
   .GroupBy(p => p.Age > 65 ? 5 : (int)((p.Age - 16) / 10))
   .Select(g => new { AgeGroup = g.Key == 5 ? " 65 above" : string.Format("{0} - {1}", g.Key * 10 + 16, g.Key * 10 + 25), Count = g.Count() });
            var Result = count.OrderByDescending(x => x.AgeGroup);
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GettomorrowLeaveCountByLocation()
        {
            var date = DateTime.Now.AddDays(1);
            var list = myapp.tbl_Leave.Where(t => t.IsActive == true && t.LeaveStatus.ToUpper() == "APPROVED" && ((t.LeaveFromDate <= date.Date || t.DateofAvailableCompoff.Value <= date.Date) && t.LeaveTodate >= date.Date)).ToList();
            var employeslist = GetListOfEmployees("");
            list = (from s in list
                    join emp in employeslist on s.UserId equals emp.CustomUserId
                    select s).ToList();
            var values = list.GroupBy(n => n.LocationName).Select(group => new { Key = group.Key, Value = group.Count() });

            return Json(values, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GettomorrowLeavesCountByDepartment(string location)
        {
            var date = DateTime.Now.AddDays(1);
            var list = myapp.tbl_Leave.Where(t => t.IsActive == true && t.LocationName.ToUpper() == location.ToUpper() && t.LeaveStatus.ToUpper() == "APPROVED" && (t.LeaveFromDate.Value <= date.Date && t.LeaveTodate.Value >= date.Date)).ToList();
            var employeslist = GetListOfEmployees("");
            list = (from s in list
                    join emp in employeslist on s.UserId equals emp.CustomUserId
                    select s).ToList();
            var values = list.GroupBy(n => n.DepartmentName).Select(group => new { Key = group.Key, Value = group.Count() });
            return Json(values, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxLeavesView(JQueryDataTableParamModel param)
        {

            if (Request.IsAuthenticated)
            {
                var employeslist = GetListOfEmployees("");
                var date = DateTime.Now.Date;
                var tdate = DateTime.Now.AddDays(1).Date;
                var query = myapp.tbl_Leave.Where(t => t.IsActive == true && t.LeaveStatus.ToUpper() == "APPROVED" && ((t.LeaveFromDate.Value <= date.Date && t.LeaveTodate.Value >= date.Date) || (t.LeaveFromDate.Value <= tdate.Date && t.LeaveTodate.Value >= tdate.Date))).ToList();
                query = (from s in query
                         join emp in employeslist on s.UserId equals emp.CustomUserId
                         select s).ToList();
                var query1 =
                   (from c in query
                    join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
                    //join app2 in myapp.tbl_User on c.Level2Approver equals app2.CustomUserId
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
                        //Level2Approver = app2.FirstName + " " + app2.LastName,
                        UserName = c.UserName,
                        DepartmentName = c.DepartmentName,
                        LocationName = c.LocationName,
                        userid = c.UserId,
                        ReasonForLeave = c.ReasonForLeave


                    });

                query1 = query1.OrderByDescending(t => t.LeaveId);

                IEnumerable<LeaveViewModels> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query1
                       .Where(c => c.LeaveTypeName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.Level1Approver != null && c.Level1Approver.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.Level2Approver != null && c.Level2Approver.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.LeaveStatus != null && c.LeaveStatus.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filteredCompanies = query1;
                }
                var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayedCompanies

                             select new[] {
                                               c.LocationName,
                                                c.DepartmentName,
                                               c.UserName ,
                                               c.userid,
                                              c.ReasonForLeave,
                                              c.LeaveFromDate,

                                              c.LeaveTodate
                            };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query1.Count(),
                    iTotalDisplayRecords = filteredCompanies.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }

        public ActionResult EmployeeLeaves()
        {
            return View();
        }
        public ActionResult GetLeavesCountWithEmployees(int LocationId, int DepartmentId, string FromDate, string ToDate)
        {
            var query = myapp.tbl_Leave.Where(q => q.LeaveStatus != "Cancelled" && q.LeaveStatus != "Reject").ToList();
            if (LocationId != null && LocationId != 0)
            {
                query = query.Where(q => q.LocationId == LocationId).ToList();
            }
            if (DepartmentId != null && DepartmentId != 0)
            {
                query = query.Where(q => q.DepartmentId == DepartmentId).ToList();
            }

            if (FromDate != null)
            {

                query = query.Where(q => q.LeaveFromDate >= ProjectConvert.ConverDateStringtoDatetime(FromDate)).ToList();
            }
            if (ToDate != null)
            {
                query = query.Where(q => q.LeaveTodate <= ProjectConvert.ConverDateStringtoDatetime(ToDate)).ToList();
            }
            var data = GetLeaves(query.ToList());
            //var list = query.GroupBy(n => n.UserName + "" + n.UserId).Select(group => new { Key = group.Key, Value = group.Count() });
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExpoertGetLeavesCountWithEmployees(int LocationId, int DepartmentId, string FromDate, string ToDate)
        {
            var query = myapp.tbl_Leave.Where(q => q.LeaveStatus != "Cancelled" && q.LeaveStatus != "Reject").ToList();
            if (LocationId != null && LocationId != 0)
            {
                query = query.Where(q => q.LocationId == LocationId).ToList();
            }
            if (DepartmentId != null && DepartmentId != 0)
            {
                query = query.Where(q => q.DepartmentId == DepartmentId).ToList();
            }

            if (FromDate != null)
            {

                query = query.Where(q => q.LeaveFromDate >= ProjectConvert.ConverDateStringtoDatetime(FromDate)).ToList();
            }
            if (ToDate != null)
            {
                query = query.Where(q => q.LeaveTodate <= ProjectConvert.ConverDateStringtoDatetime(ToDate)).ToList();
            }
            var data = GetLeaves(query.ToList());
            //var list = query.GroupBy(n => n.UserName + "" + n.UserId).Select(group => new { Key = group.Key, Value = group.Count() });
            //return Json(data, JsonRequestBehavior.AllowGet);


            var products = new System.Data.DataTable("ViewPermissionsDataTable");
            // products.Rows.Add("Client  : " + list[0].ClientName + " SOA From : " + FromDate + " To : " + ToDate);
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Department Name", typeof(string));
            products.Columns.Add("Employee Id", typeof(string));
            products.Columns.Add("User Name & Id", typeof(string));
            products.Columns.Add("Casual Leaves", typeof(string));
            products.Columns.Add("Sick Leaves", typeof(string));
            products.Columns.Add("Earned Leaves", typeof(string));
            products.Columns.Add("Comp Off", typeof(string));
            products.Columns.Add("Maternity Leave", typeof(string));
            products.Columns.Add("Paternity Leave", typeof(string));
            products.Columns.Add("Loss of pay", typeof(string));
            products.Columns.Add("Total Leaves", typeof(string));

            int transid = 1;
            foreach (var c in data)
            {
                products.Rows.Add(c.LocationName,
                                 c.DepartmentName,
                                 c.UserId,
                                 c.UserNameWithEmpID,
                                 c.CasualLeaveCount,
                                 c.SickLeaveCount,
                                 c.EarnedleaveCount,
                                   c.CompOffCount,
                                 c.MaternityLeaveCount,
                                 c.PaternityLeaveCount,
                                 c.LossofpayCount,
                                 c.TotalLeaveCount

                );
                transid++;
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
            string filename = FromDate + "_" + ToDate + "_LeavesCount.xls";
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
        public List<EmployeeLeave_ReportData> GetLeaves(List<tbl_Leave> list)
        {
            List<Datavalue> dtlistn = new List<Datavalue>();
            List<EmployeeLeave_ReportData> EmpLeaveReport = new List<EmployeeLeave_ReportData>();
            EmployeeLeave_ReportData NewEmpLeaveData;
            //foreach (var v in list)
            //{
            //    double count = v.LeaveCount.Value;
               
            //    string key = v.UserId;
            //    var EmpLeaveData = EmpLeaveReport.Where(e => e.UserId == v.UserId && e.LeaveTypeId == v.LeaveTypeId).ToList();
            //    if (EmpLeaveData.Count() == 0)
            //    {
            //        NewEmpLeaveData = new EmployeeLeave_ReportData()
            //        {
            //            DepartmentId = Convert.ToInt32(v.DepartmentId),
            //            DepartmentName = v.DepartmentName,
            //            LeaveTypeCount = count,
            //            LeaveTypeId = Convert.ToInt32(v.LeaveTypeId),
            //            LeaveTypeName = v.LeaveTypeName,
            //            LocationId = Convert.ToInt32(v.LocationId),
            //            LocationName = v.LocationName,
            //            TotalLeaveCount = count,
            //            UserId = v.UserId,
            //            UserNameWithEmpID = v.UserName
            //        };
            //        EmpLeaveReport.Add(NewEmpLeaveData);
            //    }
               
            //    //if (dtlistn.Where(t => t.Key == key).Count() == 0)
            //    //{
            //    //    Datavalue dv = new Datavalue();
            //    //    dv.Key = key;
            //    //    dv.Value = count;
            //    //    dtlistn.Add(dv);
            //    //}
            //    //else
            //    //{
            //    //    var li = dtlistn.Where(t => t.Key == key).ToList();
            //    //    li[0].Value = li[0].Value + count;
            //    //}
            //}
            var GroupedData = list.GroupBy(k => new { UserID = k.UserId }).Select(e => new EmployeeLeave_ReportData()
            {
                CasualLeaveCount = Convert.ToDouble(e.Where(l => l.LeaveTypeId == 1).Sum(p =>p.LeaveCount)),
                DepartmentName = e.ElementAtOrDefault(0).DepartmentName,
                EarnedleaveCount = Convert.ToDouble(e.Where(l => l.LeaveTypeId == 5).Sum(p => p.LeaveCount)),
                LocationName = e.ElementAtOrDefault(0).LocationName,
                SickLeaveCount = Convert.ToDouble(e.Where(l => l.LeaveTypeId == 4).Sum(p => p.LeaveCount)),
                CompOffCount = Convert.ToDouble(e.Where(l => l.LeaveTypeId == 6).Sum(p => p.LeaveCount)),
                MaternityLeaveCount = Convert.ToDouble(e.Where(l => l.LeaveTypeId == 7).Sum(p => p.LeaveCount)),
                PaternityLeaveCount = Convert.ToDouble(e.Where(l => l.LeaveTypeId == 8).Sum(p =>p.LeaveCount)),
                LossofpayCount = Convert.ToDouble(e.Where(l => l.LeaveTypeId == 1009).Sum(p => p.LeaveCount)),
                TotalLeaveCount = Convert.ToDouble(e.Select(l => Convert.ToDecimal(l.LeaveCount)).Sum()),
                UserNameWithEmpID = e.ElementAtOrDefault(0).UserName,
                UserId = e.ElementAtOrDefault(0).UserId.ToLower().Replace("fh_", "")
            }).ToList();
            return GroupedData;
        }
        public int GetCountOfleaves(DateTime? sdate, DateTime? edate)
        {
            int count = 0;
            if (sdate.Value.ToShortDateString() == edate.Value.ToShortDateString())
            {
                return 1;
            }
            else
            {
                while (sdate <= edate)
                {
                    count++;
                    sdate = sdate.Value.AddDays(1);
                }
            }
            return count;
        }

        public ActionResult EmployeeLeaveReports_ExportToExcel(string ID)
        {
            var query = myapp.tbl_Permission.ToList();
            var tasks =
              (from c in query
               join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
               where c.Level1Approver == User.Identity.Name && c.Status == "Pending"
               orderby c.CreatedOn descending
               select new PermissionViewModels
               {
                   id = c.PermissionId,
                   PermissionDate = c.PermissionDate.HasValue ? c.PermissionDate.Value.ToString("dd/MM/yyyy") : "",
                   StartDate = c.StartDate.HasValue ? c.StartDate.Value.ToString("HH:mm tt") : "",
                   EndDate = c.EndDate.HasValue ? c.EndDate.Value.ToString("HH:mm tt") : "",
                   Status = c.Status,
                   Reason = c.Reason,
                   Requestapprovename = app1.FirstName + " " + app1.LastName,
                   DepartmentName = c.DepartmentName,
                   LocationName = c.LocationName,
                   UserName = c.UserName

               });

            var products = new System.Data.DataTable("ViewPermissionsDataTable");
            // products.Rows.Add("Client  : " + list[0].ClientName + " SOA From : " + FromDate + " To : " + ToDate);
            products.Columns.Add("User Name", typeof(string));
            products.Columns.Add("Department Name", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("Start Time", typeof(string));
            products.Columns.Add("End Time", typeof(string));
            products.Columns.Add("Approve Name", typeof(string));
            products.Columns.Add("Status", typeof(string));
            products.Columns.Add("Reason", typeof(string));
            int transid = 1;
            foreach (var c in tasks)
            {
                products.Rows.Add(c.UserName,
                                 c.DepartmentName,
                                 c.LocationName,
                                 c.PermissionDate,
                                 c.StartDate,
                                 c.EndDate,
                                 c.Requestapprovename,
                                 c.Status,
                                 c.Reason
                );
                transid++;
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
            //string filename = "ViewPermissions_GridData.xls";
            //Response.AddHeader("content-disposition", "attachment; filename=" + filename);
            //Response.ContentType = "application/ms-excel";

            //Response.Charset = "";
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);

            //grid.RenderControl(htw);

            //Response.Output.Write(sw.ToString());
            //Response.Flush();
            //Response.End();

            return View("MyView");
        }

        public ActionResult DutyRoaster_Reports()
        {
            return View();
        }

        public JsonResult GetEmployeeLeaveListToAdmin()
        {
            List<TempEmployeeLeaveTableData> ReturnData = new List<TempEmployeeLeaveTableData>();
            TempEmployeeLeaveTableData NewleaveRecord;
            var query = myapp.tbl_Leave.Where(q => q.Level1Approved == true || q.Level2Approved == true && q.IsCompOff == false && q.LeaveFromDate != null && q.LeaveTodate != null);
            foreach (var item in query)
            {
                string LeaveTypeName_ShortCut = "";
                switch (item.LeaveTypeId)
                {
                    case 1:
                        LeaveTypeName_ShortCut = "CL";
                        break;
                    case 4:
                        LeaveTypeName_ShortCut = "SL";
                        break;
                    case 5:
                        LeaveTypeName_ShortCut = "EL";
                        break;
                }
                DateTime FromDate = Convert.ToDateTime(item.LeaveFromDate);
                DateTime ToDate = Convert.ToDateTime(item.LeaveTodate);
                while (FromDate <= ToDate)
                {
                    ReturnData.Add(NewleaveRecord = new TempEmployeeLeaveTableData()
                    {
                        EmployeeId = item.UserId,
                        LeaveDate = FromDate.ToShortDateString(),
                        LeaveTypeId = Convert.ToInt32(item.LeaveTypeId),
                        LeaveTypeName = item.LeaveTypeName,
                        LeaveTypeName_ShortCut = LeaveTypeName_ShortCut
                    });
                    FromDate = FromDate.AddDays(1);
                }
            }
            return Json(ReturnData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LeavesApproved()
        {
            string user = User.Identity.Name;
            var list = myapp.tbl_Leave.Where(t => t.IsActive == true && (t.UserId == user)).GroupBy(n => n.LeaveStatus).Select(group => new { Key = group.Key, Value = group.Count() });
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LeavesByStatus()
        {
            string user = User.Identity.Name;
            var list = myapp.tbl_ManageLeave.Where(x => x.UserId != null && x.UserId == user && x.IsActive == true).ToList();
            var data1 = list.GroupBy(x => x.LeaveTypeName)
                .Select(x => new
                {
                    Key = x.Key,
                    Value = x.Sum(y => y.AvailableLeave)
                }).ToList();
            return Json(data1, JsonRequestBehavior.AllowGet);
        }
        public ActionResult HelpDeskReports()
        {
            var list = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).ToList();
            if (list.Count > 0)
            {
                ViewBag.CurrentUser = list[0];
            }
            else { ViewBag.CurrentUser = ""; }
            return View();
        }
        //Get the employees based on the login selection
        public ActionResult GetEmployeesByDepartmentByName()
        {
            var dept = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).Single();
            var listname = (from v in myapp.tbl_User where v.DepartmentName == dept.DepartmentName && v.DepartmentId == dept.DepartmentId && v.LocationId == dept.LocationId select v).ToList();
            return Json(listname, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetTasksbyDeptDate(string AssignId, string Department)
        {

            // var tasks = Data.Tasklist;
            var tasks = (from tck in myapp.tbl_Task

                         select tck).ToList();
            var UserList = (from v in myapp.tbl_User where v.UserId.ToString() == AssignId select v).ToList();
            List<WebApplicationHsApp.Models.Data.ReportData> liReportData = new List<WebApplicationHsApp.Models.Data.ReportData>();
            foreach (var enty in UserList)
            {

                var newdata = (from tk in tasks where enty.UserId.ToString().ToLowerInvariant().Contains(tk.AssignId.ToString().ToLowerInvariant()) select tk).ToList();
                WebApplicationHsApp.Models.Data.ReportData rd = new WebApplicationHsApp.Models.Data.ReportData();
                rd.Name = enty.FirstName;
                int jan = 0, feb = 0, mar = 0, apr = 0, may = 0, june = 0, july = 0, aug = 0, sept = 0, oct = 0, nove = 0, dec = 0;
                DateTime presentdate = DateTime.Now;
                foreach (var tsk in newdata)
                {
                    try
                    {
                        DateTime dt = Convert.ToDateTime(tsk.CreatedOn);
                        if (dt.Year == presentdate.Year)
                        {
                            //  dt.Month
                            switch (dt.Month)
                            {
                                case 1:
                                    jan++;
                                    break;
                                case 2:
                                    feb++;
                                    break;
                                case 3:
                                    mar++;
                                    break;
                                case 4:
                                    apr++;
                                    break;
                                case 5:
                                    may++;
                                    break;
                                case 6:
                                    june++;
                                    break;
                                case 7:
                                    july++;
                                    break;
                                case 8:
                                    aug++;
                                    break;
                                case 9:
                                    sept++;
                                    break;
                                case 10:
                                    oct++;
                                    break;
                                case 11:
                                    nove++;
                                    break;
                                case 12:
                                    dec++;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                List<int> dataint = new List<int>();
                dataint.Add(jan); dataint.Add(feb); dataint.Add(mar); dataint.Add(apr);
                dataint.Add(may); dataint.Add(june); dataint.Add(july); dataint.Add(aug);
                dataint.Add(sept); dataint.Add(oct); dataint.Add(nove); dataint.Add(dec);
                rd.data = dataint.ToArray();
                liReportData.Add(rd);
            }
            return Json(liReportData, JsonRequestBehavior.AllowGet);
        }
        public List<tbl_User> GetListOfEmployees(string typeofview)
        {
            var userslist = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            if (!User.IsInRole("Admin") && !User.IsInRole("HrAdmin"))
            {
                string Currenrid = User.Identity.Name;
                var currentuser = userslist.Where(l => l.CustomUserId == Currenrid).ToList();
                if (User.IsInRole("SubDepartmentManager"))
                {
                    userslist = userslist.Where(l => l.LocationId == currentuser[0].LocationId && l.DepartmentId == currentuser[0].DepartmentId && l.SubDepartmentId == currentuser[0].SubDepartmentId).ToList();
                    return userslist;
                }
                else
                {
                    var reportingmgr = myapp.tbl_ReportingManager.Where(l => l.UserId == Currenrid).ToList();
                    if (currentuser.Count > 0 && currentuser[0].UserType == "HOD")
                    {
                        var query = (from u in userslist
                                     from rm in reportingmgr
                                     where u.LocationId == rm.LocationId && u.DepartmentId == rm.DepartmentId && (u.UserType == "Employee" || u.UserType == "HOD")
                                     select u).ToList();
                        query = query.Distinct().ToList();
                        return query;
                    }
                    else if (currentuser.Count > 0 && currentuser[0].UserType == "HeadofHOD")
                    {
                        var query = (from u in userslist
                                     from rm in reportingmgr
                                     where u.LocationId == rm.LocationId && u.DepartmentId == rm.DepartmentId && (u.UserType == "HOD" || u.UserType == "HeadofHOD")
                                     select u).ToList();
                        query = query.Distinct().ToList();
                        return query;
                    }
                    else if (currentuser.Count > 0 && currentuser[0].UserType == "Head of Manager")
                    {
                        var query = (from u in userslist
                                     from rm in reportingmgr
                                     where (u.LocationId == rm.LocationId && u.DepartmentId == rm.DepartmentId && (u.UserType == "HeadofHOD" || u.UserType == "HOD"))
                                     || u.CustomUserId == rm.UserId
                                     select u).ToList();
                        query = query.Distinct().ToList();
                        return query;
                    }
                    //else if (currentuser.Count > 0 && currentuser[0].IsEmployeesReporting.Value)
                    //{
                    //    var emplist = myapp.tbl_AssignEmployeesToManager.Where(m => m.ManagerEmployeeId == Currenrid).ToList();

                    //    var query = (from us in userslist
                    //                 from emp in emplist
                    //                 where us.CustomUserId.ToLower() == emp.EmployeeId.ToLower()
                    //                 select us).Distinct().ToList();
                    //    return query;
                    //}
                    else
                    {
                        if (reportingmgr.Count == 0)
                        {
                            var currentuser1 = userslist.Where(l => l.CustomUserId == User.Identity.Name).ToList();
                            return currentuser1;
                        }
                        else
                        {
                            var query = (from u in userslist
                                         from rm in reportingmgr
                                         where u.LocationId == rm.LocationId && u.DepartmentId == rm.DepartmentId
                                         select u).ToList();
                            query = query.Distinct().ToList();
                            return query.OrderBy(l => l.FirstName).ToList();
                        }
                    }
                }
            }
            else
            {
                if (User.IsInRole("HrAdmin") && typeofview == "Edit")
                {
                    string Currenrid = User.Identity.Name;
                    var currentuser = userslist.Where(l => l.CustomUserId == Currenrid).ToList();
                    if (User.IsInRole("SubDepartmentManager"))
                    {
                        userslist = userslist.Where(l => l.LocationId == currentuser[0].LocationId && l.DepartmentId == currentuser[0].DepartmentId && l.SubDepartmentId == currentuser[0].SubDepartmentId).ToList();
                        return userslist.OrderBy(l => l.FirstName).ToList();
                    }
                    else
                    {
                        var reportingmgr = myapp.tbl_ReportingManager.Where(l => l.UserId == Currenrid).ToList();
                        if (currentuser.Count > 0 && currentuser[0].UserType == "HOD")
                        {
                            var query = (from u in userslist
                                         from rm in reportingmgr
                                         where u.LocationId == rm.LocationId && u.DepartmentId == rm.DepartmentId && (u.UserType == "Employee" || u.UserType == "HOD")
                                         select u).ToList();
                            query = query.Distinct().ToList();
                            return query.OrderBy(l => l.FirstName).ToList();
                        }
                        else if (currentuser.Count > 0 && currentuser[0].UserType == "HeadofHOD")
                        {
                            var query = (from u in userslist
                                         from rm in reportingmgr
                                         where u.LocationId == rm.LocationId && u.DepartmentId == rm.DepartmentId && (u.UserType == "HOD" || u.UserType == "HeadofHOD")

                                         select u).ToList();
                            query = query.Distinct().ToList();
                            return query.OrderBy(l => l.FirstName).ToList();
                        }
                        else if (currentuser.Count > 0 && currentuser[0].UserType == "Head of Manager")
                        {
                            var query = (from u in userslist
                                         from rm in reportingmgr
                                         where (u.LocationId == rm.LocationId && u.DepartmentId == rm.DepartmentId && (u.UserType == "HeadofHOD" || u.UserType == "HOD"))
                                         || u.CustomUserId == rm.UserId
                                         select u).ToList();
                            query = query.Distinct().ToList();
                            return query;
                        }
                        else
                        {
                            if (reportingmgr.Count == 0)
                            {
                                var currentuser1 = userslist.Where(l => l.CustomUserId == User.Identity.Name).ToList();
                                return currentuser1.OrderBy(l => l.FirstName).ToList();
                            }
                            else
                            {
                                var query = (from u in userslist
                                             from rm in reportingmgr
                                             where u.LocationId == rm.LocationId && u.DepartmentId == rm.DepartmentId
                                             select u).ToList();
                                query = query.Distinct().ToList();
                                return query.OrderBy(l => l.FirstName).ToList();
                            }
                        }
                    }
                }
                return userslist.OrderBy(l => l.FirstName).ToList();
            }
        }

        public ActionResult LeaveReport()
        {
            return View();
        }
        public ActionResult AjaxLeaveReport(JQueryDataTableParamModel param)
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
                if (param.Emp != null && param.Emp != "")
                {
                    query = query.Where(q => q.UserId == param.Emp);
                }
                DateTime frmdate;
                DateTime dtodate;
                var Query2 = query.ToList();

                if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
                {
                     frmdate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                     dtodate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                    query = query.Where(l => ((frmdate >= l.LeaveFromDate && frmdate <= l.LeaveTodate)) || (dtodate >= l.LeaveFromDate && dtodate <= l.LeaveTodate));
                   Query2 = query.ToList();
                    for (int i = 0; i < Query2.Count; i++)
                    {
                        if (Query2[i].LeaveFromDate <= frmdate && Query2[i].LeaveTodate >= dtodate)
                        {
                            Query2[i].LeaveFromDate = frmdate;
                            Query2[i].LeaveTodate = dtodate;
                        }
                        else if (Query2[i].LeaveFromDate <= frmdate && Query2[i].LeaveTodate <= dtodate)
                        {
                            Query2[i].LeaveFromDate = frmdate;
                        }
                        else if (Query2[i].LeaveFromDate >= frmdate && Query2[i].LeaveTodate > dtodate)
                        {
                            Query2[i].LeaveTodate = dtodate;
                        }
                        else if (Query2[i].LeaveFromDate > frmdate && Query2[i].LeaveTodate >= dtodate)
                        {
                            Query2[i].LeaveFromDate = frmdate;
                        }
                    }
                }

               
               
                var query1 =
                   (from c in Query2
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
        public ActionResult ExportToExcelEmployeesLeaveReport(string location, string department, string FromDate, string ToDate, string Employee, string LeaveType)
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
            if (Employee != null && Employee != "")
            {
                query = query.Where(q => q.UserId == Employee);
            }
            if (LeaveType != null && LeaveType != "")
            {
                int LeaveTypeId = int.Parse(LeaveType);
                query = query.Where(q => q.LeaveTypeId == LeaveTypeId);
            }
            DateTime frmdate;
            DateTime dtodate;
            var Query2 = query.ToList();
            if (FromDate != null && FromDate != "" && ToDate != null && ToDate != "")
            {
                 frmdate = ProjectConvert.ConverDateStringtoDatetime(FromDate);
                 dtodate = ProjectConvert.ConverDateStringtoDatetime(ToDate);
                query = query.Where(l => ((frmdate >= l.LeaveFromDate && frmdate <= l.LeaveTodate)) || (dtodate >= l.LeaveFromDate && dtodate <= l.LeaveTodate));
                Query2 = query.ToList();
                for (int i = 0; i < Query2.Count; i++)
                {
                    if (Query2[i].LeaveFromDate <= frmdate && Query2[i].LeaveTodate >= dtodate)
                    {
                        Query2[i].LeaveFromDate = frmdate;
                        Query2[i].LeaveTodate = dtodate;
                    }
                    else if (Query2[i].LeaveFromDate <= frmdate && Query2[i].LeaveTodate <= dtodate)
                    {
                        Query2[i].LeaveFromDate = frmdate;
                    }
                    else if (Query2[i].LeaveFromDate >= frmdate && Query2[i].LeaveTodate > dtodate)
                    {
                        Query2[i].LeaveTodate = dtodate;
                    }
                    else if (Query2[i].LeaveFromDate > frmdate && Query2[i].LeaveTodate >= dtodate)
                    {
                        Query2[i].LeaveFromDate = frmdate;
                    }
                }
            }

          
            var query1 =
               (from c in Query2
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
    }
}