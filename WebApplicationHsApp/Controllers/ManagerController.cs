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
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Manager
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MyEmployees()
        {
            return View();
        }
        public JsonResult GetMyEmployeelist()
        {
            var list = GetListOfEmployees("Edit");
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public List<tbl_User> GetListOfEmployees(string typeofview)
        {
            List<tbl_User> userslistnew = new List<tbl_User>();
            var userslist = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            if (!User.IsInRole("Admin") && !User.IsInRole("HrAdmin"))
            {
                string Currenrid = User.Identity.Name;
                var currentuser = userslist.Where(l => l.CustomUserId == Currenrid).ToList();
                if (User.IsInRole("SubDepartmentManager"))
                {
                    userslistnew = userslist.Where(l => l.LocationId == currentuser[0].LocationId && l.DepartmentId == currentuser[0].DepartmentId && l.SubDepartmentId == currentuser[0].SubDepartmentId).ToList();

                }
                else
                {
                    var reportingmgr = myapp.tbl_ReportingManager.Where(l => l.UserId == Currenrid).ToList();

                    foreach (var v in reportingmgr)
                    {
                        if (v.IsHod.Value)
                        {
                            var query = (from u in userslist
                                         where u.LocationId == v.LocationId &&
                                         u.DepartmentId == v.DepartmentId
                                         && (u.UserType==null || u.UserType.ToLower() == "employee")
                                         select u).ToList();

                            userslistnew.AddRange(query);
                        }
                        else if (v.IsHodOfHod.Value)
                        {
                            var query = (from u in userslist
                                         where u.LocationId == v.LocationId &&
                                         u.DepartmentId == v.DepartmentId
                                         && u.UserType.ToLower() == "hod"
                                         select u).ToList();

                            userslistnew.AddRange(query);

                        }
                        else if (v.IsManagerOfHod.Value)
                        {
                            var query = (from u in userslist
                                         where u.LocationId == v.LocationId &&
                                         u.DepartmentId == v.DepartmentId
                                         && u.UserType.ToLower() == "headofhod"
                                         select u).ToList();

                            userslistnew.AddRange(query);

                        }

                    }

                    if (currentuser.Count > 0 && currentuser[0].UserType == "HOD")
                    {
                        var query = (from u in userslist
                                         //from rm in reportingmgr
                                     where u.LocationId == currentuser[0].LocationId &&
                                     u.DepartmentId == currentuser[0].DepartmentId
                                     && (u.UserType == "HOD" || u.UserType == "Employee")
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
                                     where (u.LocationId == currentuser[0].LocationId
                                     && u.DepartmentId == currentuser[0].DepartmentId && (u.UserType == "HeadofHOD"))

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
                            userslistnew = userslist.Where(l => l.CustomUserId == User.Identity.Name).ToList();

                        }
                        else
                        {
                            var query = (from u in userslist
                                             //from rm in reportingmgr
                                         where u.LocationId == currentuser[0].LocationId && u.DepartmentId == currentuser[0].DepartmentId
                                         select u).ToList();
                            userslistnew.AddRange(query);

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
                        userslistnew = userslist.Where(l => l.LocationId == currentuser[0].LocationId && l.DepartmentId == currentuser[0].DepartmentId && l.SubDepartmentId == currentuser[0].SubDepartmentId).ToList();

                    }
                    else
                    {
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

                                userslistnew.AddRange(query);
                            }
                            else if (v.IsHodOfHod.Value)
                            {
                                var query = (from u in userslist
                                             where u.LocationId == v.LocationId &&
                                             u.DepartmentId == v.DepartmentId
                                             && u.UserType.ToLower() == "hod"
                                             select u).ToList();

                                userslistnew.AddRange(query);

                            }
                            else if (v.IsManagerOfHod.Value)
                            {
                                var query = (from u in userslist
                                             where u.LocationId == v.LocationId &&
                                             u.DepartmentId == v.DepartmentId
                                             && u.UserType.ToLower() == "headofhod"
                                             select u).ToList();

                                userslistnew.AddRange(query);

                            }

                        }

                        if (currentuser.Count > 0 && currentuser[0].UserType == "HOD")
                        {
                            var query = (from u in userslist
                                             //from rm in reportingmgr
                                         where u.LocationId == currentuser[0].LocationId
                                         && u.DepartmentId == currentuser[0].DepartmentId && (u.UserType == "HOD" || u.UserType == "Employee")
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
                                         where (u.LocationId == currentuser[0].LocationId &&
                                         u.DepartmentId == currentuser[0].DepartmentId && (u.UserType == "HeadofHOD"))

                                         select u).ToList();
                            userslistnew.AddRange(query);

                        }
                        //else if (currentuser.Count > 0 && currentuser[0].IsEmployeesReporting.Value)
                        //{
                        //    var emplist = myapp.tbl_AssignEmployeesToManager.Where(m => m.ManagerEmployeeId == Currenrid).ToList();

                        //    var query = (from us in userslist
                        //                 from emp in emplist
                        //                 where us.CustomUserId.ToLower() == emp.EmployeeId.ToLower()
                        //                 select us).Distinct().ToList();
                        //    userslistnew.AddRange(query);

                        //}
                        else
                        {
                            if (reportingmgr.Count == 0)
                            {
                                userslistnew = userslist.Where(l => l.CustomUserId == User.Identity.Name).ToList();

                            }
                            else
                            {
                                var query = (from u in userslist
                                                 //from rm in reportingmgr
                                             where u.LocationId == currentuser[0].LocationId && u.DepartmentId == currentuser[0].DepartmentId
                                             select u).ToList();
                                userslistnew.AddRange(query);

                            }
                        }
                    }
                }

            }
            userslistnew = userslistnew.Distinct().ToList();
            userslistnew = (from q in userslistnew
                            orderby int.Parse(q.CustomUserId.ToLower().Replace("fh_", ""))
                            select q).ToList();
            return userslistnew;
        }
        public ActionResult AjaxGetEmployees(JQueryDataTableParamModel param)
        {
            var query = GetListOfEmployees("Edit");

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
                         select new[] {
                                              c.LocationName,
                                              c.DepartmentName,
                                              c.FirstName+" "+c.LastName,
                                              c.CustomUserId.ToLower().Replace("fh_",""),
                                              c.EmailId,
                                              //c.PlaceAllocation,
                                              c.PhoneNumber,
                                              c.Extenstion,
                                              //c.DepartmentName,
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
        public ActionResult MyEmployeeLeaves()
        {
            return View();
        }
        public ActionResult AjaxMyApprovedView(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                //var reportingmgr = myapp.tbl_ReportingManager.Where(l => l.UserId == User.Identity.Name).ToList();
                //if (reportingmgr.Count > 0)
                //{
                var queryEmployees = GetListOfEmployees("Edit");
                var leaveslit = myapp.tbl_Leave.ToList();
                var query = (from q in leaveslit
                             join rm in queryEmployees
                             on q.UserId equals rm.CustomUserId
                             //where q.DepartmentId == rm.DepartmentId
                             select q);
                if (param.LeaveTypeid != null && param.LeaveTypeid != 0)
                {
                    query = query.Where(q => q.LeaveTypeId == param.LeaveTypeid);
                }
                if (param.fromdate != null && param.fromdate != "")
                {
                    DateTime dt = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                    query = query.Where(q => q.LeaveFromDate >= dt);
                }
                if (param.todate != null && param.todate != "")
                {
                    DateTime dt = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                    query = query.Where(q => q.LeaveTodate <= dt);
                }
                if (param.Emp != null && param.Emp != "")
                {
                    query = query.Where(q => q.UserId == param.Emp);
                }
                var query1 =
                   (from c in query.ToList()
                    join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
                    //join app2 in myapp.tbl_User on c.Level2Approver equals app2.CustomUserId
                    //where c.LeaveStatus == "Pending" && (c.Level1Approver == User.Identity.Name || c.Level2Approver == User.Identity.Name)
                    select new LeaveViewModels
                    {
                        LeaveId = c.LeaveId.ToString(),
                        LeaveTypeName = c.LeaveTypeName,
                        IsFullday = c.IsFullday.ToString(),
                        //IsCompOff = c.IsCompOff.ToString(),
                        LeaveFromDate = c.LeaveFromDate.Value.ToString("dd/MM/yyyy"),
                        LeaveTodate = c.LeaveTodate.Value.ToString("dd/MM/yyyy"),
                        LeaveStatus = c.LeaveStatus,
                        Level1Approver = app1.FirstName + " " + app1.LastName,
                        //Level2Approver = app2.FirstName + " " + app2.LastName,
                        UserName = c.UserName,
                        DepartmentName = c.DepartmentName,
                        LocationName = c.LocationName,
                        DateofAvailableCompoff = c.DateofAvailableCompoff.HasValue ? c.DateofAvailableCompoff.Value.ToString("dd/MM/yyyy") : "",
                        AddressOnLeave = c.AddressOnLeave,
                        ReasonForLeave = c.ReasonForLeave,
                        TotalLeaves = c.LeaveCount.HasValue ? c.LeaveCount.Value : 0,
                        LeaveSessionDay = c.LeaveSessionDay,
                        LeaveCreatedOn = c.CreatedOn.Value.ToString("dd/MM/yyyy")
                    });
                query1 = query1.Distinct();
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
                                               c.LeaveTypeName,
                                               c.IsFullday.ToString(),
                                               c.LeaveSessionDay,
                                              // c.IsCompOff.ToString(),
                                               (Convert.ToBoolean(c.IsCompOff)?c.DateofAvailableCompoff:c.LeaveFromDate),
                                               c.LeaveTodate,
                                               c.AddressOnLeave,
                                               c.ReasonForLeave,
                                               c.TotalLeaves.ToString() +" " +(c.WeeklyOffDay!=null ? "("+ c.WeeklyOffDay +" as Weekly Off)":""),
                                               c.LeaveStatus,
                                               c.LeaveCreatedOn,
                              c.LeaveId.ToString()};
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query1.Count(),
                    iTotalDisplayRecords = filteredCompanies.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    List<LeaveViewModels> list = new List<LeaveViewModels>();
                //    return Json(new
                //    {
                //        sEcho = param.sEcho,
                //        iTotalRecords = 0,
                //        iTotalDisplayRecords = 0,
                //        aaData = list
                //    }, JsonRequestBehavior.AllowGet);
                //}
            }
            else { return RedirectToAction("Login", "Account"); }
        }

        public ActionResult MyEmployeeLeaves_ExportToExcel(string Emp, string fromdate, string todate, int LeaveTypeid)
        {

            //var reportingmgr = myapp.tbl_ReportingManager.Where(l => l.UserId == User.Identity.Name).ToList();
            var queryEmployees = GetListOfEmployees("Edit");
            var leaveslit = myapp.tbl_Leave.ToList();
            var query = (from q in leaveslit
                         join rm in queryEmployees
                         on q.UserId equals rm.CustomUserId
                         //where q.DepartmentId == rm.DepartmentId
                         select q);
            if (LeaveTypeid != null && LeaveTypeid != 0)
            {
                query = query.Where(q => q.LeaveTypeId == LeaveTypeid);
            }
            if (fromdate != null && fromdate != "")
            {
                DateTime dt = ProjectConvert.ConverDateStringtoDatetime(fromdate);
                query = query.Where(q => q.LeaveFromDate >= dt);
            }
            if (todate != null && todate != "")
            {
                DateTime dt = ProjectConvert.ConverDateStringtoDatetime(todate);
                query = query.Where(q => q.LeaveTodate <= dt);
            }
            if (Emp != null && Emp != "")
            {
                query = query.Where(q => q.UserId == Emp);
            }
            var query1 =
               (from c in query.ToList()
                join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
                //join app2 in myapp.tbl_User on c.Level2Approver equals app2.CustomUserId
                //where c.LeaveStatus == "Pending" && (c.Level1Approver == User.Identity.Name || c.Level2Approver == User.Identity.Name)
                select new LeaveViewModels
                {
                    LeaveId = c.LeaveId.ToString(),
                    LeaveTypeName = c.LeaveTypeName,
                    IsFullday = c.IsFullday.ToString(),
                    //IsCompOff = c.IsCompOff.ToString(),
                    LeaveFromDate = c.LeaveFromDate.Value.ToString("dd/MM/yyyy"),
                    LeaveTodate = c.LeaveTodate.Value.ToString("dd/MM/yyyy"),
                    LeaveStatus = c.LeaveStatus,
                    Level1Approver = app1.FirstName + " " + app1.LastName,
                    //Level2Approver = app2.FirstName + " " + app2.LastName,
                    UserName = c.UserName,
                    DepartmentName = c.DepartmentName,
                    LocationName = c.LocationName,
                    DateofAvailableCompoff = c.DateofAvailableCompoff.HasValue ? c.DateofAvailableCompoff.Value.ToString("dd/MM/yyyy") : "",
                    AddressOnLeave = c.AddressOnLeave,
                    ReasonForLeave = c.ReasonForLeave,
                    TotalLeaves = c.LeaveCount.HasValue ? c.LeaveCount.Value : 0,
                    LeaveSessionDay = c.LeaveSessionDay,
                    LeaveCreatedOn = c.CreatedOn.Value.ToString("dd/MM/yyyy")
                });
            query1 = query1.Distinct();
            query1 = query1.OrderByDescending(t => t.LeaveId);
            var products = new System.Data.DataTable("ViewPermissionsDataTable");
            // products.Rows.Add("Client  : " + list[0].ClientName + " SOA From : " + FromDate + " To : " + ToDate);
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("UserName", typeof(string));
            products.Columns.Add("Leave Type", typeof(string));
            products.Columns.Add("Is Full Day", typeof(string));
            products.Columns.Add("Session Day", typeof(string));
            products.Columns.Add("From Date", typeof(string));
            products.Columns.Add("To Date", typeof(string));
            products.Columns.Add("Address On Leave", typeof(string));
            products.Columns.Add("Reason", typeof(string));

            products.Columns.Add("Total Leaves", typeof(string));
            products.Columns.Add("Leave Status", typeof(string));
            products.Columns.Add("Leave Created On", typeof(string));
            int transid = 1;
            foreach (var c in query1)
            {
                products.Rows.Add(c.LocationName,
                                 c.DepartmentName,
                                  c.UserName,
                                               c.LeaveTypeName,
                                               c.IsFullday.ToString(),
                                               c.LeaveSessionDay,
                                               // c.IsCompOff.ToString(),
                                               (Convert.ToBoolean(c.IsCompOff) ? c.DateofAvailableCompoff : c.LeaveFromDate),
                                               c.LeaveTodate,
                                               c.AddressOnLeave,
                                               c.ReasonForLeave,
                                               c.TotalLeaves.ToString() + " " + (c.WeeklyOffDay != null ? "(" + c.WeeklyOffDay + " as Weekly Off)" : ""),
                                               c.LeaveStatus,
                                               c.LeaveCreatedOn
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
            string filename = "MyEmployeeLeaves_GridData.xls";
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

        public ActionResult MyEmployeePermissions()
        {
            return View();
        }
        public JsonResult GetEditLeaveDetailsByID(int id)
        {
            var List = (from c in myapp.tbl_Leave
                        join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
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
                             DateofAvailableCompoff = c.DateofAvailableCompoff.HasValue ? c.DateofAvailableCompoff.Value.ToString("dd/MM/yyyy") : "",
                             Level1ApproveComment = c.Level1ApproveComment,
                             Level2ApproveComment = c.Level2ApproveComment,
                             Level1Approved = c.Level1Approved.ToString(),

                             LeaveFromDate_CustomFormat_1 = (!(Convert.ToBoolean(c.IsCompOff)) ? Convert.ToDateTime(c.LeaveFromDate).ToString("MM/dd/yyyy") : ""),
                             LeaveToDate_CustomFormat_1 = (!(Convert.ToBoolean(c.IsCompOff)) ? Convert.ToDateTime(c.LeaveTodate).ToString("MM/dd/yyyy") : ""),
                             DateofAvailableCompoff_CustomFormat_1 = ((Convert.ToBoolean(c.IsCompOff)) ? Convert.ToDateTime(c.DateofAvailableCompoff).ToString("MM/dd/yyyy") : "")
                         }).FirstOrDefault();

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetApprovePermission(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                //var reportingmgr = myapp.tbl_ReportingManager.Where(l => l.UserId == User.Identity.Name).ToList();
                var queryEmployees = GetListOfEmployees("Edit");
                var leaveslit = myapp.tbl_Permission.ToList();
                var query = (from q in leaveslit
                             from m in queryEmployees
                             where q.UserId == m.CustomUserId
                             select q);

                if (param.locationid != null && param.locationid != 0)
                {
                    query = query.Where(q => q.LocationId == param.locationid).ToList();
                }
                if (param.departmentid != null && param.departmentid != 0)
                {
                    query = query.Where(q => q.DepartmentId == param.departmentid).ToList();
                }
                if (param.Emp != null && param.Emp != "")
                {
                    query = query.Where(q => q.UserId == param.Emp).ToList();
                }
                if (!String.IsNullOrEmpty(param.status))
                {
                    query = query.Where(q => q.Status == param.status).ToList();
                }

                var tasks =
                  (from c in query.ToList()
                   join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
                   where c.Level1Approver == User.Identity.Name && app1.IsActive == true

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

                IEnumerable<PermissionViewModels> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = tasks
                       .Where(c => c.id.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.PermissionDate != null && c.PermissionDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                   c.StartDate != null && c.StartDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                   c.EndDate != null && c.EndDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                   c.Status != null && c.Status.ToString().ToLower().Contains(param.sSearch.ToLower())
                                     ||
                                   c.Requestapprovename != null && c.Requestapprovename.ToString().ToLower().Contains(param.sSearch.ToLower())
                               );
                }
                else
                {
                    filteredCompanies = tasks;
                }
                var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayedCompanies
                             select new[] {
                                 c.UserName,
                                 c.DepartmentName,
                                 c.LocationName,
                                 c.PermissionDate,
                                 c.StartDate,
                                 c.EndDate,
                                 c.Requestapprovename,
                                 c.Status,
                                 c.Reason,
                                 Convert.ToString(c.id)
                                    };

                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = tasks.Count(),
                    iTotalDisplayRecords = filteredCompanies.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }


        public ActionResult ViewPermissions_ExportToExcel(string ClientId, string FromDate, string ToDate)
        {

            //var reportingmgr = myapp.tbl_ReportingManager.Where(l => l.UserId == User.Identity.Name).ToList();
            var queryEmployees = GetListOfEmployees("Edit");
            var leaveslit = myapp.tbl_Permission.ToList();
            var query = (from q in leaveslit
                         from m in queryEmployees
                         where q.UserId == m.CustomUserId
                         select q).ToList();
            var tasks =
              (from c in query
               join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
               where c.Level1Approver == User.Identity.Name && app1.IsActive == true
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
            string filename = "ViewPermissions_GridData.xls";
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
        public double GetCompoffLeaveBalancetotal(string username)
        {
            double balance = 0;
            var CompOffReqList = myapp.tbl_ManageLeave.Where(l => l.UserId == username && l.LeaveTypeId == 6).ToList();
            if (CompOffReqList.Count > 0)
            {
                balance = CompOffReqList[0].AvailableLeave.Value;
            }


            return balance;
        }
        public ActionResult GetAjaxsLeaveBalances(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                List<LeavesCountView> Leavscount = new List<LeavesCountView>();

                var query = myapp.tbl_ManageLeave.ToList();
                var queryEmployees = GetListOfEmployees("Edit");
                query = (from q in query
                         join qu in queryEmployees on q.UserId equals qu.CustomUserId
                         select q).ToList();
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
                          c.UserId.ToString(),
                            c.LocationName,
                            c.DepartmentName,
                             c.UserName,
                             c.CasuvalAvailableLeave.HasValue ? c.CasuvalAvailableLeave.Value.ToString():"0",
                             c.SickAvailableLeave.HasValue? c.SickAvailableLeave.Value.ToString():"0",
                            c.EarnedAvailableLeave.HasValue? c.EarnedAvailableLeave.Value.ToString():"0",
                            c.CompoffBalance.ToString(),
                            c.IsActive.ToString()};
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

        public ActionResult ViewLeaveBalances()
        {
            return View();
        }
        public ActionResult ExportToExcelEmployeesLeaveTypeData()
        {

            List<LeavesCountView> Leavscount = new List<LeavesCountView>();

            var query = myapp.tbl_ManageLeave.ToList();
            var reportingmgr = myapp.tbl_ReportingManager.Where(l => l.UserId == User.Identity.Name).ToList();
            if (reportingmgr.Count > 0)
            {
                query = query.Where(q => q.LocationId == reportingmgr[0].LocationId && q.DepartmentId == reportingmgr[0].DepartmentId).ToList();
            }
            var newquery = query.GroupBy(u => u.UserId).ToList();
            foreach (var v in newquery)
            {
                LeavesCountView lv = new LeavesCountView();
                lv.UserId = v.Key;
                foreach (var vs in v.ToList())
                {
                    lv.UserId = vs.UserId;
                    lv.UserName = vs.UserName;
                    lv.LocationId = vs.LocationId;
                    lv.DepartmentId = vs.DepartmentId;
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

            var products = new System.Data.DataTable("EmployeesLeaveApplications");
            // products.Rows.Add("Client  : " + list[0].ClientName + " SOA From : " + FromDate + " To : " + ToDate);
            products.Columns.Add("User Id", typeof(string));
            products.Columns.Add("Location Name", typeof(string));
            products.Columns.Add("Department Name", typeof(string));
            products.Columns.Add("User Name", typeof(string));
            products.Columns.Add("Casuval Available", typeof(string));
            products.Columns.Add("Sick Available", typeof(string));
            products.Columns.Add("Earned Available", typeof(string));

            foreach (var c in Leavscount)
            {
                products.Rows.Add(c.UserId.ToLower().Replace("fh_", ""),
                                 c.LocationName,
                                 c.DepartmentName,
                                 c.UserName,
                                 c.CasuvalAvailableLeave,
                                 c.SickAvailableLeave.ToString(),
                                 c.EarnedAvailableLeave.ToString()


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

        public ActionResult EmployeeCompOffRequests()
        {
            return View();
        }
        public ActionResult AjaxAllRequestCompOffView(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {

                var query = myapp.tbl_RequestCompOffLeave.ToList();
                var queryEmployees = GetListOfEmployees("Edit");
                query = (from q in query
                         from m in queryEmployees
                         where q.UserId == m.CustomUserId
                         select q).ToList();
                IEnumerable<tbl_RequestCompOffLeave> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.CompOffDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.UserName != null && c.UserName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   // ||
                                   //c.Level2Approver != null && c.Level2Approver.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filteredCompanies = query;
                }
                var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayedCompanies

                             select new[] {
                                 c.UserId,
                                 c.UserName,
                                               c.DepartmentName,
                                               c.LocationName,
                                                 c.CompOffDateTime.Value.ToString("dd/MM/yyyy"),
                                               c.RequestReason,
                                               c.Leave_Status,
                                               c.CreatedDateTime.Value.ToString("dd/MM/yyyy")
                              //c.ID.ToString()
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
    }
}