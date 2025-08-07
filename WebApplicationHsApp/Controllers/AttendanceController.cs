using System;
using System.Collections.Generic;
using System.Data;
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
    public class AttendanceController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Attendance
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Manager()
        {
            return View();
        }
        public ActionResult HR()
        {
            return View();
        }
        public ActionResult VendorEmployee()
        {
            return View();
        }
        public JsonResult GetDashboardCount()
        {

            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetMySwipesTotal(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                DateTime date = DateTime.Now;
                //var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                //var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                int userid = int.Parse(User.Identity.Name.ToLower().Replace("fh_", ""));
                var query1 = myapp.uspGetEmployeesSwep(date.AddDays(-45), date, 0, 0, userid, 0, 0, 0, "").OrderByDescending(t => t.att_date).ToList();


                IEnumerable<uspGetEmployeesSwep_Result> filtered;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filtered = query1
                       .Where(c => c.att_date.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.earlyin != null && c.earlyin.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.in_time != null && c.in_time.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.out_time != null && c.out_time.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filtered = query1;
                }
                var displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayed

                             select new[] {c.employeeid,
                                 c.FirstName,
                                               c.att_date.Value.ToString("dd/MM/yyyy"),
                                               c.in_time,
                                              c.out_time,
                                               c.latein,
                                               c.lateout,
                                               c.earlyin,
                                               c.earlyout,
                                               c.status};
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query1.Count(),
                    iTotalDisplayRecords = filtered.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }

        public ActionResult AjaxGetManagerSwipesTotal(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                DateTime date = DateTime.Now;
                //var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                //var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                int userid = int.Parse(User.Identity.Name.ToLower().Replace("fh_", ""));
                List<uspGetEmployeesSwep_Result> query1 = new List<uspGetEmployeesSwep_Result>();
                //var query1 = myapp.uspGetEmployeesSwep(date.AddYears(-50), date, 0, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
                if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
                {
                    DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                    DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                    query1 = myapp.uspGetEmployeesSwep(dtfrom, dtto, 0, 0, 0, 1, 0, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
                }
                else
                {
                    query1 = myapp.uspGetEmployeesSwep(date.AddDays(-45), date, 0, 0, 0, 1, 0, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
                }
                if (param.Emp != null && param.Emp != "")
                {
                    try
                    {
                        //int empid = int.Parse(param.Emp.ToLower().Replace("fh_", ""));
                        query1 = query1.Where(l => l.employeeid == param.Emp).ToList();
                    }
                    catch { }
                }
                IEnumerable<uspGetEmployeesSwep_Result> filtered;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filtered = query1
                       .Where(c => c.att_date.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.earlyin != null && c.earlyin.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.in_time != null && c.in_time.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.out_time != null && c.out_time.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filtered = query1;
                }
                var displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayed

                             select new[] {c.employeeid.ToString(),
                                  c.LocationName,
                                 c.DepartmentName,
                                  c.FirstName,
                                               c.att_date.Value.ToString("dd/MM/yyyy"),
                                               c.in_time,
                                              c.out_time,
                                               c.latein,
                                               c.lateout,
                                               c.earlyin,
                                               c.earlyout,
                                               c.status};
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query1.Count(),
                    iTotalDisplayRecords = filtered.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }

        public ActionResult AjaxGetVendorEmployeeSwipesTotal(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                DateTime date = DateTime.Now;
                //var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                //var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                string userid = User.Identity.Name;
                var usr = myapp.tbl_Vendor.Where(l => l.Email == userid).SingleOrDefault();
                if (User.IsInRole("Admin") || User.IsInRole("HrAdmin"))
                {
                    if (param.FormType != null && param.FormType != "")
                    {
                        int vid = int.Parse(param.FormType);
                        usr= myapp.tbl_Vendor.Where(d => d.VendorId == vid && d.IsActive == true).SingleOrDefault();
                    }
                }
                List<uspGetEmployeesSwep_Result> query1 = new List<uspGetEmployeesSwep_Result>();
                if (usr != null)
                {
                    //var query1 = myapp.uspGetEmployeesSwep(date.AddYears(-50), date, 0, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
                    if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
                    {
                        DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                        DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                        query1 = myapp.uspGetEmployeesSwep(dtfrom, dtto, 0, 0, 0, 1, 0, usr.VendorId, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
                    }
                    else
                    {
                        query1 = myapp.uspGetEmployeesSwep(date.AddDays(-45), date, 0, 0, 0, 1, 0, usr.VendorId, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
                    }
                }
                if (param.FormType != null && param.FormType != "")
                {
                    var vemps = myapp.tbl_OutSourceUser.Where(d => d.VendorId == param.FormType && d.IsActive == true).ToList();
                    query1 = (from q in query1
                              join e in vemps on q.employeeid equals e.CustomUserId
                              select q).ToList();
                }
                if (param.Emp != null && param.Emp != "")
                {
                    try
                    {

                        query1 = query1.Where(l => l.employeeid == param.Emp).ToList();
                    }
                    catch { }
                }
                IEnumerable<uspGetEmployeesSwep_Result> filtered;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filtered = query1
                       .Where(c => c.att_date.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.earlyin != null && c.earlyin.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.in_time != null && c.in_time.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.out_time != null && c.out_time.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filtered = query1;
                }
                var displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayed

                             select new[] {
                                 c.employeeid,
                                  c.LocationName,
                                 c.DepartmentName,
                                  c.FirstName,
                                               c.att_date.Value.ToString("dd/MM/yyyy"),
                                               c.in_time,
                                              c.out_time,
                                               c.latein,
                                               c.lateout,
                                               c.earlyin,
                                               c.earlyout,
                                               c.status};
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query1.Count(),
                    iTotalDisplayRecords = filtered.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }
        public ActionResult ExportToExcelEmployeeAttendance(string fromdate, string todate, int locationid, int departmentid, string employeeid)
        {

            DateTime date = DateTime.Now;

            int userid = int.Parse(User.Identity.Name.ToLower().Replace("fh_", ""));
            List<uspGetEmployeesSwep_Result> query1 = new List<uspGetEmployeesSwep_Result>();
            if (locationid != 0 && departmentid != 0 && fromdate != null && fromdate != "" && todate != null && todate != "")
            {
                DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(fromdate);
                DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(todate);

                query1 = myapp.uspGetEmployeesSwep(dtfrom, dtto, locationid, departmentid, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
            }
            else if (locationid != 0 && fromdate != null && fromdate != "" && todate != null && todate != "")
            {
                DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(fromdate);
                DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(todate);
                query1 = myapp.uspGetEmployeesSwep(dtfrom, dtto, locationid, 0, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();

            }
            else if (locationid != 0 && departmentid != 0)
            {

                query1 = myapp.uspGetEmployeesSwep(date.AddDays(-45), date, locationid, departmentid, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
            }
            else if (fromdate != null && fromdate != "" && todate != null && todate != "")
            {
                DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(fromdate);
                DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(todate);
                query1 = myapp.uspGetEmployeesSwep(dtfrom, dtto, 0, 0, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
            }
            else
            {
                query1 = myapp.uspGetEmployeesSwep(date.AddDays(-45), date, 0, 0, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
            }

            if (employeeid != null && employeeid != "")
            {

                query1 = query1.Where(l => l.employeeid == employeeid).ToList();
            }
            var products = new System.Data.DataTable("AccountBalance");

            products.Columns.Add("Id", typeof(string));

            products.Columns.Add("Name", typeof(string));
            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("Shift Type", typeof(string));
            products.Columns.Add("In Time", typeof(string));
            products.Columns.Add("Out Time", typeof(string));
            products.Columns.Add("Late In", typeof(string));
            products.Columns.Add("Late Out", typeof(string));
            products.Columns.Add("Early In", typeof(string));
            products.Columns.Add("Early Out", typeof(string));
            //products.Columns.Add("Status", typeof(string));
            foreach (var c in query1)
            {

                products.Rows.Add(
                   c.employeeid.ToString(),
                    c.FirstName,

                                 c.DepartmentName,
                                 c.LocationName,
                                               c.att_date.Value.ToString("dd/MM/yyyy"),
                                               c.ShiftTypeName,
                                               c.in_time,
                                              c.out_time,
                                                c.latein,
                                               c.lateout,
                                               c.earlyin,
                                               c.earlyout
                //c.status
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
            string filename = "EmployeeAttendance.xls";


            Response.AddHeader("content-disposition", "attachment; filename=" + filename);
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View();
        }

        public ActionResult AjaxGetHrManagerSwipesTotal(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                DateTime date = DateTime.Now;
                //var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                //var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                int userid = int.Parse(User.Identity.Name.ToLower().Replace("fh_", ""));
                List<uspGetEmployeesSwep_Result> query1 = new List<uspGetEmployeesSwep_Result>();
                if (param.locationid != null && param.locationid != 0 && param.departmentid != null && param.departmentid != 0 && param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
                {
                    DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                    DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(param.todate);

                    query1 = myapp.uspGetEmployeesSwep(dtfrom, dtto, param.locationid, param.departmentid, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
                }
                else if (param.locationid != null && param.locationid != 0 && param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
                {
                    DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                    DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                    query1 = myapp.uspGetEmployeesSwep(dtfrom, dtto, param.locationid, 0, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();

                }
                else if (param.locationid != null && param.locationid != 0 && param.departmentid != null && param.departmentid != 0)
                {

                    query1 = myapp.uspGetEmployeesSwep(date.AddDays(-45), date, param.locationid, param.departmentid, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
                }
                else if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
                {
                    DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                    DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                    query1 = myapp.uspGetEmployeesSwep(dtfrom, dtto, 0, 0, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
                }
                else
                {
                    query1 = myapp.uspGetEmployeesSwep(date.AddDays(-45), date, 0, 0, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
                }
                if (param.Emp != null && param.Emp != "")
                {
                    try
                    {

                        query1 = query1.Where(l => l.employeeid == param.Emp).ToList();
                    }
                    catch { }
                }

                IEnumerable<uspGetEmployeesSwep_Result> filtered;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filtered = query1
                       .Where(c => c.att_date.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.earlyin != null && c.earlyin.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.in_time != null && c.in_time.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.employeeid != null && c.employeeid.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.FirstName != null && c.FirstName.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.out_time != null && c.out_time.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filtered = query1;
                }
                var displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayed

                             select new[] {c.employeeid.ToUpper(),
                                 c.FirstName,
                                 c.DepartmentName,

                                   c.LocationName,
                                               c.att_date.Value.ToString("dd/MM/yyyy"),
                                               c.ShiftTypeName,
                                               c.in_time,
                                              c.out_time,
                                               c.latein,
                                               c.lateout,
                                               c.earlyin,
                                               c.earlyout
                                              };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query1.Count(),
                    iTotalDisplayRecords = filtered.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }
        public ActionResult ExportToExcelEmployeeAttendanceLate(string fromdate, string todate, int locationid, int departmentid)
        {

            DateTime date = DateTime.Now;

            int userid = int.Parse(User.Identity.Name.ToLower().Replace("fh_", ""));
            List<uspGetEmployeesSwep_Result> query1 = new List<uspGetEmployeesSwep_Result>();
            if (fromdate != null && fromdate != "" && todate != null && todate != "")
            {
                DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(fromdate);
                DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(todate);
                int locationid1 = 0, departmentid1 = 0;
                if (locationid != null)
                {
                    locationid1 = locationid;
                }
                if (departmentid1 != null)
                {
                    departmentid1 = departmentid;
                }
                query1 = myapp.uspGetEmployeesSwep(dtfrom, dtto, locationid1, departmentid1, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).Where(l => l.latein != null).ToList();
                query1 = query1.Where(l => Convert.ToInt32(l.latein) > 10).ToList();
            }
            else
            {
                DateTime dtfrom = DateTime.Now.AddMonths(-2);
                DateTime dtto = DateTime.Now;
                int locationid1 = 0, departmentid1 = 0;
                if (locationid != null)
                {
                    locationid1 = locationid;
                }
                if (departmentid != null)
                {
                    departmentid1 = departmentid;
                }
                query1 = myapp.uspGetEmployeesSwep(dtfrom, dtto, locationid1, departmentid1, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).Where(l => l.latein != null).ToList();
                query1 = query1.Where(l => Convert.ToInt32(l.latein) > 10).ToList();
            }
            List<EmployeeSwipesLeaves> list2 = new List<EmployeeSwipesLeaves>();
            var employeeslist = query1.Select(l => l.employeeid).Distinct().ToList();
            foreach (var q in employeeslist)
            {
                var querydetails = query1.Where(l => l.employeeid == q).ToList();
                EmployeeSwipesLeaves empl = new EmployeeSwipesLeaves();

                empl.att_date = querydetails[0].att_date;
                empl.cmp_code = querydetails[0].cmp_code;
                empl.DepartmentName = querydetails[0].DepartmentName;
                empl.earlyin = querydetails[0].earlyin;
                empl.earlyout = querydetails[0].earlyout;
                empl.employeeid = int.Parse(querydetails[0].employeeid);
                empl.extrahours = querydetails[0].extrahours;
                empl.FirstName = querydetails[0].FirstName;
                empl.in_time = querydetails[0].in_time;
                empl.latein = Convert.ToDecimal(querydetails[0].latein);
                empl.lateout = querydetails[0].lateout;
                empl.LocationName = querydetails[0].LocationName;
                empl.out_time = querydetails[0].out_time;
                empl.shft_cod = querydetails[0].shft_cod;
                empl.status = querydetails[0].status;
                empl.TotalDaysLate = querydetails.Count;
                list2.Add(empl);
            }
            list2 = list2.Where(l => l.TotalDaysLate >= 3).ToList();

            foreach (var l in list2)
            {
                l.LeavestoBededuct = Convert.ToInt32(l.TotalDaysLate / 3);
                if (l.LeavestoBededuct >= 1)
                    l.LeavestoBededuct = l.LeavestoBededuct / 2;
            }
            var products = new System.Data.DataTable("AccountBalance");

            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Name", typeof(string));
            products.Columns.Add("Department", typeof(string));

            products.Columns.Add("Location", typeof(string));
            //products.Columns.Add("Date", typeof(string));
            //products.Columns.Add("In Time", typeof(string));
            //products.Columns.Add("Out Time", typeof(string));
            //products.Columns.Add("Late In", typeof(string));
            //products.Columns.Add("Late Out", typeof(string));
            products.Columns.Add("Late Days", typeof(string));
            products.Columns.Add("Total Leaves (Lop)", typeof(string));
            //products.Columns.Add("Status", typeof(string));
            foreach (var c in list2)
            {
                products.Rows.Add(
                   c.employeeid.ToString(),
                     c.FirstName,

                                 c.DepartmentName,
                                c.LocationName,
                                               // c.att_date.Value.ToString("dd/MM/yyyy"),
                                               // c.in_time,
                                               //c.out_time,
                                               // c.latein,
                                               // c.lateout,
                                               c.TotalDaysLate.ToString(),
                                               c.LeavestoBededuct
                //c.status
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
            string filename = "EmployeeAttendance.xls";


            Response.AddHeader("content-disposition", "attachment; filename=" + filename);
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View();
        }
        public ActionResult AjaxsLoadAlltheSwipesLate(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                List<uspGetEmployeesSwep_Result> query1 = new List<uspGetEmployeesSwep_Result>();
                if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
                {
                    DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                    DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                    int locationid = 0, departmentid = 0;
                    if (param.locationid != null)
                    {
                        locationid = param.locationid;
                    }
                    if (param.departmentid != null)
                    {
                        departmentid = param.departmentid;
                    }
                    query1 = myapp.uspGetEmployeesSwep(dtfrom, dtto, locationid, departmentid, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).Where(l => l.latein != null).ToList();
                    query1 = query1.Where(l => Convert.ToInt32(l.latein) > 10).ToList();
                }
                List<EmployeeSwipesLeaves> list2 = new List<EmployeeSwipesLeaves>();
                var employeeslist = query1.Select(l => l.employeeid).Distinct().ToList();
                foreach (var q in employeeslist)
                {
                    var querydetails = query1.Where(l => l.employeeid == q).ToList();
                    EmployeeSwipesLeaves empl = new EmployeeSwipesLeaves();

                    empl.att_date = querydetails[0].att_date;
                    empl.cmp_code = querydetails[0].cmp_code;
                    empl.DepartmentName = querydetails[0].DepartmentName;
                    empl.earlyin = querydetails[0].earlyin;
                    empl.earlyout = querydetails[0].earlyout;
                    empl.employeeid = int.Parse(querydetails[0].employeeid);
                    empl.extrahours = querydetails[0].extrahours;
                    empl.FirstName = querydetails[0].FirstName;
                    empl.in_time = querydetails[0].in_time;
                    empl.latein = Convert.ToDecimal(querydetails[0].latein);
                    empl.lateout = querydetails[0].lateout;
                    empl.LocationName = querydetails[0].LocationName;
                    empl.out_time = querydetails[0].out_time;
                    empl.shft_cod = querydetails[0].shft_cod;
                    empl.status = querydetails[0].status;
                    empl.TotalDaysLate = querydetails.Count;
                    list2.Add(empl);
                }
                list2 = list2.Where(l => l.TotalDaysLate >= 3).ToList();
                foreach (var l in list2)
                {
                    l.LeavestoBededuct = Convert.ToInt32(l.TotalDaysLate / 3);
                    if (l.LeavestoBededuct >= 1)
                        l.LeavestoBededuct = l.LeavestoBededuct / 2;
                }

                IEnumerable<EmployeeSwipesLeaves> filtered;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filtered = list2
                       .Where(c => c.att_date.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.earlyin != null && c.earlyin.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.in_time != null && c.in_time.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.out_time != null && c.out_time.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filtered = list2;
                }
                var displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayed

                             select new[] {c.employeeid.ToString(),
                                c.FirstName,
                                 c.DepartmentName,

                                   c.LocationName,
                                              // c.att_date.Value.ToString("dd/MM/yyyy"),
                                              // c.in_time,
                                              //c.out_time,
                                              // c.latein.HasValue?c.latein.ToString():"",
                                              // c.lateout,
                                               c.TotalDaysLate.ToString(),
                                               c.LeavestoBededuct.ToString(),
                                               //c.earlyin,
                                               //c.earlyout,
                                               //c.status
                             };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = list2.Count(),
                    iTotalDisplayRecords = filtered.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }
        //Manager Dashboard

        public ActionResult AjaxGetManagerTodaySwipesTotal(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                DateTime date = DateTime.Now;
                //var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                //var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                int userid = int.Parse(User.Identity.Name.ToLower().Replace("fh_", ""));
                var query1 = myapp.uspGetEmployeesSwep(date, date, 0, 0, 0, 1, 0, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
                query1 = query1.Where(l => l.latein != null).ToList();

                IEnumerable<uspGetEmployeesSwep_Result> filtered;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filtered = query1
                       .Where(c => c.att_date.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.earlyin != null && c.earlyin.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.in_time != null && c.in_time.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.out_time != null && c.out_time.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filtered = query1;
                }
                var displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayed

                             select new[] {c.FirstName,
                                               c.att_date.Value.ToString("dd/MM/yyyy"),
                                               c.in_time,
                                              c.out_time,
                                               c.latein
                             };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query1.Count(),
                    iTotalDisplayRecords = filtered.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }
        public ActionResult AjaxGetManagerYestradaySwipesTotal(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                DateTime date = DateTime.Now;
                //var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                //var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                int userid = int.Parse(User.Identity.Name.ToLower().Replace("fh_", ""));
                var query1 = myapp.uspGetEmployeesSwep(date.AddDays(-1), date, 0, 0, 0, 1, 0, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();

                query1 = query1.Where(l => l.latein != null).ToList();
                IEnumerable<uspGetEmployeesSwep_Result> filtered;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filtered = query1
                       .Where(c => c.att_date.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.earlyin != null && c.earlyin.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.in_time != null && c.in_time.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.out_time != null && c.out_time.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filtered = query1;
                }
                var displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayed

                             select new[] {
                                               c.FirstName,
                                               c.att_date.Value.ToString("dd/MM/yyyy"),
                                               c.in_time,
                                              c.out_time,
                                               c.latein
                             };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query1.Count(),
                    iTotalDisplayRecords = filtered.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }

        //Get Current Month Late Commers

        public JsonResult GetCurrentMonthLateComersByManager()
        {
            DateTime date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var query1 = myapp.uspGetEmployeesSwep(firstDayOfMonth, lastDayOfMonth, 0, 0, 0, 1, 0, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
            query1 = query1.Where(l => l.latein != null).ToList();
            var names = (from q in query1
                         select q.employeeid).Distinct().ToList();
            List<GraphValues> vallist = new List<GraphValues>();

            foreach (var rs in names)
            {
                var fadate = firstDayOfMonth;
                if (rs != null)
                {
                    GraphValues gv = new GraphValues();
                    gv.name = query1.Where(q => q.employeeid == rs).ToList()[0].FirstName;
                    List<int> gvcount = new List<int>();
                    while (fadate <= date)
                    {
                        var scount = query1.Where(q => q.att_date.Value.ToShortDateString() == fadate.ToShortDateString() && q.employeeid == rs).ToList();
                        if (scount.Count > 0)
                        {
                            scount[0].latein = scount[0].latein;

                            if (scount[0].latein != null)
                            {
                                gvcount.Add(int.Parse(scount[0].latein));
                            }
                            else gvcount.Add(0);
                        }
                        else
                        {
                            gvcount.Add(0);
                        }
                        fadate = fadate.AddDays(1);
                    }
                    gv.data = gvcount;
                    vallist.Add(gv);
                }
            }
            return Json(vallist, JsonRequestBehavior.AllowGet);
        }
        public int GetMinutesBystring(string minutes)
        {
            if (minutes.Contains(":"))
            {
                var mins = minutes.Split(':');
                var totallate = int.Parse(mins[1]) + (60 * int.Parse(mins[0]));
                return totallate;
            }
            return 0;
        }

        //Hr Dashboard

        public ActionResult AjaxGetHrManagerTodaySwipesTotal(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                DateTime date = DateTime.Now;
                //var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
                //var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                var query1 = myapp.uspGetEmployeesSwep(date, date, 0, 0, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
                query1 = query1.Where(l => l.latein != null).ToList();
                IEnumerable<uspGetEmployeesSwep_Result> filtered;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filtered = query1
                       .Where(c => c.att_date.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.earlyin != null && c.earlyin.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.in_time != null && c.in_time.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.out_time != null && c.out_time.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filtered = query1;
                }
                var displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayed

                             select new[] {c.employeeid.ToString(),
                                 c.LocationName,
                                 c.DepartmentName,
                                  c.FirstName,
                                               c.att_date.Value.ToString("dd/MM/yyyy"),
                                               c.in_time,
                                              c.out_time,
                                               c.latein
                                              };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query1.Count(),
                    iTotalDisplayRecords = filtered.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }
        public ActionResult AjaxGetHrManagerYestradaySwipesTotal(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                DateTime date = DateTime.Now;

                var query1 = myapp.uspGetEmployeesSwep(date.AddDays(-1), date, 0, 0, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
                query1 = query1.Where(l => l.latein != null).ToList();
                IEnumerable<uspGetEmployeesSwep_Result> filtered;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filtered = query1
                       .Where(c => c.att_date.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.earlyin != null && c.earlyin.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.in_time != null && c.in_time.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.out_time != null && c.out_time.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filtered = query1;
                }
                var displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayed

                             select new[] {c.employeeid.ToString(),
                                 c.LocationName,
                                 c.DepartmentName,
                                  c.FirstName,
                                               c.att_date.Value.ToString("dd/MM/yyyy"),
                                               c.in_time,
                                              c.out_time,
                                              c.latein
                                               };
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query1.Count(),
                    iTotalDisplayRecords = filtered.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }

        public JsonResult GetCurrentMonthLateComersByHy(int locationId, int Departmentid)
        {
            DateTime date = DateTime.Now;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var query1 = myapp.uspGetEmployeesSwep(firstDayOfMonth, lastDayOfMonth, locationId, Departmentid, 0, 0, 1, 0, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
            query1 = query1.Where(l => l.latein != null).ToList();
            var names = (from q in query1
                         select q.employeeid).Distinct().ToList();
            List<GraphValues> vallist = new List<GraphValues>();

            foreach (var rs in names)
            {
                var fadate = firstDayOfMonth;
                if (rs != null)
                {
                    GraphValues gv = new GraphValues();
                    gv.name = query1.Where(q => q.employeeid == rs).ToList()[0].FirstName;
                    List<int> gvcount = new List<int>();
                    while (fadate <= date)
                    {
                        var scount = query1.Where(q => q.att_date.Value.ToShortDateString() == fadate.ToShortDateString() && q.employeeid == rs).ToList();
                        if (scount.Count > 0)
                        {
                            scount[0].latein = scount[0].latein;

                            if (scount[0].latein != null)
                            {
                                gvcount.Add(int.Parse(scount[0].latein));
                            }
                            else gvcount.Add(0);
                        }
                        else
                        {
                            gvcount.Add(0);
                        }
                        fadate = fadate.AddDays(1);
                    }
                    gv.data = gvcount;
                    vallist.Add(gv);
                }
            }
            return Json(vallist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSwipeswithRoaster(string Fromdate, string Todate, int locationId = 0, int Departmentid = 0, int employeeid = 0)
        {
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(Fromdate);
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(Todate);
            var shifts = myapp.uspGetEmployeesSwepWithRoster(dtfrom, dtto, locationId, Departmentid, employeeid).ToList();
            List<AttendanceSwipeRoasterReport> reportmodel = new List<AttendanceSwipeRoasterReport>();
            var distintemployeeid = shifts.Select(l => l.CustomUserId).Distinct().ToList();

            foreach (var l in distintemployeeid)
            {
                var listshits = shifts.Where(cu => cu.CustomUserId == l).ToList();
                AttendanceSwipeRoasterReport model = new AttendanceSwipeRoasterReport();
                model.EmployeeId = l;
                model.EmployeeName = listshits.FirstOrDefault().FirstName;
                model.Total = listshits.Count.ToString();
                model.AttendanceDayWise = new List<AttendanceDayWise>();
                var countlop = listshits.Where(lp => lp.roasterlatein > 10).Count();
                model.TotalLop = countlop >= 1 ? (countlop / 2).ToString() : countlop.ToString();
                model.TotalLeaves = listshits.Where(ls => ls.LeaveType != null && ls.LeaveType != "").Count().ToString();
                model.TotalAbsent = "0";
                DateTime Dtstart = dtfrom;
                while (Dtstart <= dtto)
                {
                    var tshifts = listshits.Where(c => c.ShiftDate.Value.Date == Dtstart.Date).ToList();
                    AttendanceDayWise amodel = new AttendanceDayWise();
                    amodel.Date = Dtstart.ToString("dd-MM");
                    if (tshifts.Count > 0)
                    {
                        amodel.EarlyIn = tshifts[0].earlyin;
                        amodel.EarlyOut = tshifts[0].earlyout;
                        amodel.LateIn = tshifts[0].roasterlatein.HasValue ? (tshifts[0].roasterlatein.Value > 0 ? tshifts[0].roasterlatein.Value.ToString() : "0") : "";
                        amodel.LateOut = tshifts[0].lateout;
                        amodel.OTHrs = "";
                        amodel.Shift = tshifts[0].ShiftTypeName;
                        amodel.Status = "PR";
                        if (tshifts[0].in_time == null && tshifts[0].out_time == null && tshifts[0].ShiftTypeName == "WO")
                        {
                            amodel.Status = "-";
                        }
                        amodel.TimeIn = tshifts[0].in_time;
                        amodel.TimeOut = tshifts[0].out_time;
                        if (tshifts[0].in_time == null && tshifts[0].out_time == null && tshifts[0].ShiftTypeName != "WO")
                        {
                            amodel.Status = "AB";
                        }
                        if (tshifts[0].LeaveType != null && tshifts[0].LeaveType != "")
                        {
                            amodel.Status = tshifts[0].LeaveType;
                        }
                        amodel.Shift = (tshifts[0].CompOffReq != null && tshifts[0].CompOffReq != "" && tshifts[0].CompOffReq.Contains(":") && tshifts[0].CompOffReq.Split(':')[1].Trim() != tshifts[0].ShiftTypeName.Trim()) ? tshifts[0].CompOffReq.Split(':')[1] + "/" + tshifts[0].ShiftTypeName.Replace("Holiday", "H").Replace("WeekOff", "WO") : tshifts[0].ShiftTypeName.Replace("Holiday", "H").Replace("WeekOff", "WO");
                    }
                    model.AttendanceDayWise.Add(amodel);
                    Dtstart = Dtstart.AddDays(1);
                }
                model.TotalPresent = model.AttendanceDayWise.Where(la => la.Status == "PR").Count().ToString();
                var totalAbsent = (listshits.Count - model.AttendanceDayWise.Where(la => la.Status == "PR").Count());
                totalAbsent = (totalAbsent - model.AttendanceDayWise.Where(la => la.Status == "-").Count());
                model.TotalAbsent = totalAbsent.ToString();
                reportmodel.Add(model);
            }

            //return Json(reportmodel, JsonRequestBehavior.AllowGet);
            DataTable dt = new DataTable("Grid");
            string MainHtmltable = " <table  style='border-collapse: collapse;border: 1px solid #ccc;font-size: 12px;'>";


            var distintemployeeid1 = shifts.Select(l => l.CustomUserId).Distinct().ToList();
            foreach (var l in distintemployeeid1)
            {
                List<string> str = new List<string>();

                var listofemp = reportmodel.Where(rm => rm.EmployeeId == l).ToList();
                //reportmodel
                var la = listofemp.SingleOrDefault();
                if (la != null)
                {
                    MainHtmltable = MainHtmltable + "<tr>";
                    MainHtmltable = MainHtmltable + "<th style='background-color: teal;color:white;border: 1px solid #ccc;width:150px;'>Employee</th>";
                    MainHtmltable = MainHtmltable + "<th style='background-color: teal;color:white;border: 1px solid #ccc;width:150px;'>-</th>";

                    DateTime Dtstart1 = dtfrom;
                    while (Dtstart1 <= dtto)
                    {
                        MainHtmltable = MainHtmltable + "<th style='background-color: teal;color:white;border: 1px solid #ccc;width:100px;'>" + Dtstart1.ToString("dd-MM") + "</th>";

                        Dtstart1 = Dtstart1.AddDays(1);
                    }
                    MainHtmltable = MainHtmltable + "</tr>";



                    MainHtmltable = MainHtmltable + "<tr>";
                    MainHtmltable = MainHtmltable + "<td style='width:120px;border: 1px solid #ccc;width:150px;'>";

                    MainHtmltable = MainHtmltable + "<span style='color:teal;'> " + la.EmployeeName + "</span><br>";
                    MainHtmltable = MainHtmltable + "Emp Id: " + la.EmployeeId.Replace("FH_", "") + "<br>";
                    MainHtmltable = MainHtmltable + "Total Days: " + la.Total + "<br>";
                    MainHtmltable = MainHtmltable + "Total Absent: " + la.TotalAbsent + "<br>";
                    MainHtmltable = MainHtmltable + "Total Leaves: " + la.TotalLeaves + "<br>";
                    MainHtmltable = MainHtmltable + "Total Lop: " + la.TotalLop + "<br>";
                    MainHtmltable = MainHtmltable + "Total Present: " + la.TotalPresent + "<br>";
                    MainHtmltable = MainHtmltable + "</td>";


                    MainHtmltable = MainHtmltable + "<td style='width:120px;border: 1px solid #ccc;width:150px;'>";
                    MainHtmltable = MainHtmltable + " Shift : <br>";

                    MainHtmltable = MainHtmltable + "<span style='color:red;'>Status :</span><br> ";
                    MainHtmltable = MainHtmltable + "In Time: <br>";
                    MainHtmltable = MainHtmltable + "Out Time:<br> ";
                    MainHtmltable = MainHtmltable + "<span style='color:red;'>Late In:</span> <br>";
                    MainHtmltable = MainHtmltable + "Late Out:<br> ";
                    MainHtmltable = MainHtmltable + "</td>";

                    DateTime Dtstart2 = dtfrom;
                    while (Dtstart2 <= dtto)
                    {

                        var listcheck = (from m in listofemp
                                         from lis in m.AttendanceDayWise
                                         where lis.Date == Dtstart2.ToString("dd-MM")
                                         select lis).ToList();

                        if (listcheck.Count > 0)
                        {

                            MainHtmltable = MainHtmltable + "<td style='width:120px;border: 1px solid #ccc;text-align:left;width:100px;'>";
                            MainHtmltable = MainHtmltable + " ";

                            MainHtmltable = MainHtmltable + listcheck[0].Shift + "<br>";
                            MainHtmltable = MainHtmltable + "<span style='color:red;'>" + listcheck[0].Status + "</span><br>";
                            MainHtmltable = MainHtmltable + listcheck[0].TimeIn + "<br>";
                            MainHtmltable = MainHtmltable + listcheck[0].TimeOut + "<br>";
                            MainHtmltable = MainHtmltable + "<span style='color:red;'>" + listcheck[0].LateIn + "</span><br>";
                            MainHtmltable = MainHtmltable + listcheck[0].LateOut + "<br>";
                            MainHtmltable = MainHtmltable + "</td>";
                        }

                        Dtstart2 = Dtstart2.AddDays(1);
                    }
                    MainHtmltable = MainHtmltable + "</tr>";
                }
                //foreach (var amodel in l.AttendanceDayWise)
                //{
                //    str.Add(amodel.Date);
                //    strEarlyIn.Add(amodel.EarlyIn);
                //    strEarlyOut.Add(amodel.EarlyOut);
                //    strLateIn.Add(amodel.LateIn);
                //    strLateOut.Add(amodel.LateOut);
                //    strShift.Add(amodel.Shift);
                //    strStatus.Add(amodel.Status);
                //    strTimeIn.Add(amodel.TimeIn);
                //    strTimeOut.Add(amodel.TimeOut);                  

                //}

            }


            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=MyExcelFile.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";

            Response.Output.Write(MainHtmltable);
            Response.Flush();
            Response.End();
            return View("MyView");

        }

        public ActionResult GetSwipeswithRoasterForVendor(string Fromdate, string Todate, int locationId = 0, int Departmentid = 0, int employeeid = 0,string FormType="")
        {
            string userid = User.Identity.Name;
            var usr = myapp.tbl_Vendor.Where(l => l.Email == userid).SingleOrDefault();
            if (User.IsInRole("Admin") || User.IsInRole("HrAdmin"))
            {
                if (FormType != null && FormType != "")
                {
                    int vid = int.Parse(FormType);
                    usr = myapp.tbl_Vendor.Where(d => d.VendorId == vid && d.IsActive == true).SingleOrDefault();
                }
            }
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(Fromdate);
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(Todate);
            //var shifts = myapp.uspGetEmployeesSwepWithRoster(dtfrom, dtto, locationId, Departmentid, employeeid).ToList();
            var shifts = myapp.uspGetEmployeesSwep(dtfrom, dtto, 0, 0, 0, 1, 0, usr.VendorId, User.Identity.Name).OrderByDescending(t => t.att_date).ToList();
            List<AttendanceSwipeRoasterReport> reportmodel = new List<AttendanceSwipeRoasterReport>();
            var distintemployeeid = shifts.Select(l => l.employeeid).Distinct().ToList();

            foreach (var l in distintemployeeid)
            {
                var listshits = shifts.Where(cu => cu.employeeid == l).ToList();
                AttendanceSwipeRoasterReport model = new AttendanceSwipeRoasterReport();
                model.EmployeeId = l;
                model.EmployeeName = listshits.FirstOrDefault().FirstName;
                model.Total = listshits.Count.ToString();
                model.AttendanceDayWise = new List<AttendanceDayWise>();
                var countlop = 0;
                model.TotalLop = countlop >= 1 ? (countlop / 2).ToString() : countlop.ToString();
                model.TotalLeaves = "0";
                model.TotalAbsent = "0";
                DateTime Dtstart = dtfrom;
                while (Dtstart <= dtto)
                {
                    var tshifts = listshits.Where(c => c.att_date.Value.Date == Dtstart.Date).ToList();
                    AttendanceDayWise amodel = new AttendanceDayWise();
                    amodel.Date = Dtstart.ToString("dd-MM");
                    if (tshifts.Count > 0)
                    {
                        amodel.EarlyIn = tshifts[0].earlyin;
                        amodel.EarlyOut = tshifts[0].earlyout;
                        amodel.LateIn = "";
                        amodel.LateOut = tshifts[0].lateout;
                        amodel.OTHrs = "";
                        amodel.Shift = tshifts[0].ShiftTypeName;
                        amodel.Status = "PR";
                        if (tshifts[0].in_time == null && tshifts[0].out_time == null && tshifts[0].ShiftTypeName == "WO")
                        {
                            amodel.Status = "-";
                        }
                        amodel.TimeIn = tshifts[0].in_time;
                        amodel.TimeOut = tshifts[0].out_time;
                        if (tshifts[0].in_time == null && tshifts[0].out_time == null && tshifts[0].ShiftTypeName != "WO")
                        {
                            amodel.Status = "AB";
                        }
                        //if (tshifts[0].LeaveType != null && tshifts[0].LeaveType != "")
                        //{
                        //    amodel.Status = tshifts[0].LeaveType;
                        //}
                        amodel.Shift = "";
                    }
                    model.AttendanceDayWise.Add(amodel);
                    Dtstart = Dtstart.AddDays(1);
                }
                model.TotalPresent = model.AttendanceDayWise.Where(la => la.Status == "PR").Count().ToString();
                var totalAbsent = (listshits.Count - model.AttendanceDayWise.Where(la => la.Status == "PR").Count());
                totalAbsent = (totalAbsent - model.AttendanceDayWise.Where(la => la.Status == "-").Count());
                model.TotalAbsent = totalAbsent.ToString();
                reportmodel.Add(model);
            }

            //return Json(reportmodel, JsonRequestBehavior.AllowGet);
            DataTable dt = new DataTable("Grid");
            string MainHtmltable = " <table  style='border-collapse: collapse;border: 1px solid #ccc;font-size: 12px;'>";


            var distintemployeeid1 = shifts.Select(l => l.employeeid).Distinct().ToList();
            foreach (var l in distintemployeeid1)
            {
                List<string> str = new List<string>();

                var listofemp = reportmodel.Where(rm => rm.EmployeeId == l).ToList();
                //reportmodel
                var la = listofemp.SingleOrDefault();
                if (la != null)
                {
                    MainHtmltable = MainHtmltable + "<tr>";
                    MainHtmltable = MainHtmltable + "<th style='background-color: teal;color:white;border: 1px solid #ccc;width:150px;'>Employee</th>";
                    MainHtmltable = MainHtmltable + "<th style='background-color: teal;color:white;border: 1px solid #ccc;width:150px;'>-</th>";

                    DateTime Dtstart1 = dtfrom;
                    while (Dtstart1 <= dtto)
                    {
                        MainHtmltable = MainHtmltable + "<th style='background-color: teal;color:white;border: 1px solid #ccc;width:100px;'>" + Dtstart1.ToString("dd-MM") + "</th>";

                        Dtstart1 = Dtstart1.AddDays(1);
                    }
                    MainHtmltable = MainHtmltable + "</tr>";



                    MainHtmltable = MainHtmltable + "<tr>";
                    MainHtmltable = MainHtmltable + "<td style='width:120px;border: 1px solid #ccc;width:150px;'>";

                    MainHtmltable = MainHtmltable + "<span style='color:teal;'> " + la.EmployeeName + "</span><br>";
                    MainHtmltable = MainHtmltable + "Emp Id: " + la.EmployeeId.Replace("FH_", "") + "<br>";
                    MainHtmltable = MainHtmltable + "Total Days: " + la.Total + "<br>";
                    MainHtmltable = MainHtmltable + "Total Absent: " + la.TotalAbsent + "<br>";
                    MainHtmltable = MainHtmltable + "Total Leaves: " + la.TotalLeaves + "<br>";
                    MainHtmltable = MainHtmltable + "Total Lop: " + la.TotalLop + "<br>";
                    MainHtmltable = MainHtmltable + "Total Present: " + la.TotalPresent + "<br>";
                    MainHtmltable = MainHtmltable + "</td>";


                    MainHtmltable = MainHtmltable + "<td style='width:120px;border: 1px solid #ccc;width:150px;'>";
                    MainHtmltable = MainHtmltable + " Shift : <br>";

                    MainHtmltable = MainHtmltable + "<span style='color:red;'>Status :</span><br> ";
                    MainHtmltable = MainHtmltable + "In Time: <br>";
                    MainHtmltable = MainHtmltable + "Out Time:<br> ";
                    MainHtmltable = MainHtmltable + "<span style='color:red;'>Late In:</span> <br>";
                    MainHtmltable = MainHtmltable + "Late Out:<br> ";
                    MainHtmltable = MainHtmltable + "</td>";

                    DateTime Dtstart2 = dtfrom;
                    while (Dtstart2 <= dtto)
                    {

                        var listcheck = (from m in listofemp
                                         from lis in m.AttendanceDayWise
                                         where lis.Date == Dtstart2.ToString("dd-MM")
                                         select lis).ToList();

                        if (listcheck.Count > 0)
                        {

                            MainHtmltable = MainHtmltable + "<td style='width:120px;border: 1px solid #ccc;text-align:left;width:100px;'>";
                            MainHtmltable = MainHtmltable + " ";

                            MainHtmltable = MainHtmltable + listcheck[0].Shift + "<br>";
                            MainHtmltable = MainHtmltable + "<span style='color:red;'>" + listcheck[0].Status + "</span><br>";
                            MainHtmltable = MainHtmltable + listcheck[0].TimeIn + "<br>";
                            MainHtmltable = MainHtmltable + listcheck[0].TimeOut + "<br>";
                            MainHtmltable = MainHtmltable + "<span style='color:red;'>" + listcheck[0].LateIn + "</span><br>";
                            MainHtmltable = MainHtmltable + listcheck[0].LateOut + "<br>";
                            MainHtmltable = MainHtmltable + "</td>";
                        }

                        Dtstart2 = Dtstart2.AddDays(1);
                    }
                    MainHtmltable = MainHtmltable + "</tr>";
                }
                //foreach (var amodel in l.AttendanceDayWise)
                //{
                //    str.Add(amodel.Date);
                //    strEarlyIn.Add(amodel.EarlyIn);
                //    strEarlyOut.Add(amodel.EarlyOut);
                //    strLateIn.Add(amodel.LateIn);
                //    strLateOut.Add(amodel.LateOut);
                //    strShift.Add(amodel.Shift);
                //    strStatus.Add(amodel.Status);
                //    strTimeIn.Add(amodel.TimeIn);
                //    strTimeOut.Add(amodel.TimeOut);                  

                //}

            }


            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=MyExcelFile.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";

            Response.Output.Write(MainHtmltable);
            Response.Flush();
            Response.End();
            return View("MyView");

        }
    }
}