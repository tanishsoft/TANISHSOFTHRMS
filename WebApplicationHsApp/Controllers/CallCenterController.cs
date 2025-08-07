using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;
using WebApplicationHsApp.Service;

namespace WebApplicationHsApp.Controllers
{
    public class CallCenterController : Controller
    {
        private readonly MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        private readonly TaskService _taskService =new TaskService();

      
        // GET: CallCenter
        public async Task<ActionResult> Index()
        {
            List<tbl_User> list = await myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).ToListAsync();
            if (list.Count > 0)
            {
                ViewBag.CurrentUser = list[0];
            }
            else { ViewBag.CurrentUser = ""; }
            ViewBag.TotalRequests = await myapp.tbl_TaskCallcenter.Where(t => t.IsActive == true && t.TaskType != null && t.TaskType == "Requests").CountAsync();
            ViewBag.TotalComplaints = await myapp.tbl_TaskCallcenter.Where(t => t.IsActive == true && t.TaskType != null && t.TaskType == "Complaints").CountAsync();
            ViewBag.TotalClosed = await myapp.tbl_TaskCallcenter.Where(t => t.IsActive == true && t.TaskType != null && t.AssignStatus == "Done").CountAsync();
            ViewBag.TotalInProgress = await myapp.tbl_TaskCallcenter.Where(t => t.IsActive == true && t.TaskType != null && t.AssignStatus != "Done").CountAsync();
            return View();
        }
        //public async Task<ActionResult> GetDepartmentByLocation(int id)
        //{
        //    List<tbl_TaskSLAUser> list = await myapp.tbl_TaskSLAUser.Where(d => d.TaskLocationId == id).ToListAsync();
        //    var model = (from l in list
        //                 select new
        //                 {
        //                     DepartmentId = l.TaskDepartmentId,
        //                     DepartmentName = l.CustomDepartmentName
        //                 }).Distinct().ToList();
        //    return Json(model, JsonRequestBehavior.AllowGet);
        //}
        public async Task<ActionResult> GetEmployeesByDepartmenet(int locationid, int departmentid)
        {
            List<tbl_TaskSLAUser> list = await myapp.tbl_TaskSLAUser.Where(d => d.TaskLocationId == locationid && d.TaskDepartmentId == departmentid).ToListAsync();
            List<tbl_User> users = await myapp.tbl_User.Where(l => l.IsActive == true).ToListAsync();
            var model = (from u in list
                         join u1 in users on u.SLAUserId equals u1.EmpId
                         select new
                         {
                             u.SLAUserId,
                             u1.FirstName,
                             u.EmpEmail,
                             u.EmpMobile

                         }).Distinct().ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageTaskSLAUsers()
        {
            return View();
        }
        public ActionResult ManageDepartment()
        {
            return View();
        }
        public ActionResult CallCenterReport()
        {
            return View();
        }
        public ActionResult GetSpecializations()
        {
            var list = myapp.tbl_Doctor.Where(l => l.IsActive == true && l.ShowInHelpdesk == true).Select(l => l.Specialization).Distinct().ToList();
            list = list.OrderBy(l => l).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDoctorsBySpecializations(string name)
        {
            var list = myapp.tbl_Doctor.Where(l => l.IsActive == true && l.ShowInHelpdesk == true && l.Specialization == name).ToList();
            list = list.OrderBy(l => l.DoctorName).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        private List<callCenterReportViewModel> GetcallCenterReport(int locationId, string fromDate, string toDate, int departmentid = 0)
        {
            try
            {
                var startDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var endDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                var list = new List<callCenterReportViewModel>();
                var callCenter = myapp.tbl_TaskCallcenter.Where(l => l.CreatedOn >= startDate && l.CreatedOn <= endDate).AsQueryable();
                //callCenter = callCenter.Where(l => l.CreatedOn >= startDate && l.CreatedOn <= endDate).ToList();
                if (locationId != 0)
                {
                    callCenter = callCenter.Where(l => l.AssignLocationId == locationId).AsQueryable();
                }
                if (departmentid != null && departmentid != 0)
                {
                    callCenter = callCenter.Where(l => l.AssignDepartmentId == departmentid).AsQueryable();
                }
                var values = callCenter.ToList();
                var groupedcallCenter = values.GroupBy(u => u.CallDateTime.Value.Date)
                                          .Select(grp => new { Date = grp.Key, callCenter = grp.ToList() })
                                          .ToList();
                foreach (var dt in groupedcallCenter)
                {
                    var loclist = dt.callCenter.Select(l => l.AssignLocationId).Distinct().ToList();
                    foreach (var loc in loclist)
                    {
                        var depttickets = dt.callCenter.Where(l => l.AssignLocationId == loc).Select(l => l.AssignDepartmentId).Distinct().ToList();
                        foreach (var dept in depttickets)
                        {
                            var tickets = dt.callCenter.Where(l => l.AssignLocationId == loc && l.AssignDepartmentId == dept).ToList();
                            //var locid= Convert.ToInt32(loc.LocationId);
                            var data = new callCenterReportViewModel();
                            data.date = dt.Date;
                            data.totalTickets = tickets.Count();
                            data.locationId = loc.Value;
                            data.locationName = myapp.tbl_Location.Where(n => n.LocationId == loc).Select(b => b.LocationName).FirstOrDefault();
                            data.departmentId = dept.HasValue ? dept.Value : 0;
                            data.departmentName = tickets[0].AssignDepartmentName;
                            data.activeTicket = tickets.Where(n => n.AssignStatus != "Done").Count();
                            //var days = DateTime.Now.AddMinutes(-300);

                            var completedtickets = tickets.Where(b => b.AssignStatus == "Done").ToList();
                            var ComplaintsclosedwithinTat = completedtickets.Where(b => b.TaskType == "Complaints" && (b.CallEndDateTime - b.CallDateTime).Value.TotalHours < 4).Count();
                            var nonComplaintsclosedwithinTat = completedtickets.Where(b => b.TaskType != "Complaints" && (b.CallEndDateTime - b.CallDateTime).Value.TotalHours < 6).Count();
                            data.closedWithInTAT = ComplaintsclosedwithinTat + nonComplaintsclosedwithinTat;
                            data.closedOutOfTAT = tickets.Count() - data.closedWithInTAT;

                            //data.closedWithInTAT = tickets.Where(b => b.AssignStatus == "Done" && b.ModifiedOn < b.CallDateTime.Value.AddMinutes(300)).Count();
                            //data.closedOutOfTAT = tickets.Where(b => b.AssignStatus == "Done" && b.ModifiedOn > b.CallDateTime.Value.AddMinutes(300)).Count();

                            data.secondLevelPending = tickets.Where(v => v.AssignStatus != "Done" && v.CreatorStatus != "Done").Count();
                            data.thirdLevelPending = tickets.Where(v => v.AssignStatus != "Done" && v.CreatorStatus != "Done").Count();
                            list.Add(data);
                        }
                    }
                }
                return list;
            }
            catch (Exception exc)
            {
                return new List<callCenterReportViewModel>();
            }
        }
        public JsonResult ExportExcelCallCenter(string fromDate, string toDate, int locationId = 0, int departmentid = 0)
        {

            List<callCenterReportViewModel> query = GetcallCenterReport(locationId, fromDate, toDate, departmentid);

            var products = new System.Data.DataTable("Call Center Report");

            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("Total tickets", typeof(string));
            products.Columns.Add("Active tickets", typeof(string));
            products.Columns.Add("Closed within TAT", typeof(string));
            products.Columns.Add("Out of TAT", typeof(string));
            products.Columns.Add("2nd Level pending", typeof(string));
            products.Columns.Add("3nrd Level pending", typeof(string));

            foreach (var item in query)
            {
                products.Rows.Add(
              item.date.ToString("dd/MM/yyyy"),
              item.locationName,
              item.departmentName,
              item.totalTickets,
              item.activeTicket,
              item.closedWithInTAT,
              item.closedOutOfTAT,
              item.secondLevelPending,
              item.thirdLevelPending

                );
            }

            products.Rows.Add("", "Total Tickets ", query.Sum(l => l.totalTickets));
            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=CallCenterReport.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> AjaxGetCallCenterReport(JQueryDataTableParamModel param)
        {
            //List<tbl_TaskSLAUser> squery = await myapp.tbl_TaskSLAUser.ToListAsync();
            List<callCenterReportViewModel> query = GetcallCenterReport(param.locationid, param.fromdate, param.todate, param.departmentid);
            IEnumerable<callCenterReportViewModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.locationName.ToString().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<callCenterReportViewModel> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[] {
                                           c.date.ToString("dd/MM/yyyy") , c.locationName, c.departmentName,

                                               c.totalTickets.ToString(),c.activeTicket.ToString(),
                                           c.closedWithInTAT.ToString(),c.closedOutOfTAT.ToString(),c.secondLevelPending.ToString(),c.thirdLevelPending.ToString()
                                              };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveDepartment(tbl_CallCenterDepartment tbll)
        {
            if (tbll.DepartmentId != 0)
            {
                var tb = myapp.tbl_CallCenterDepartment.Where(a => a.DepartmentId == tbll.DepartmentId).SingleOrDefault();
                tb.DepartmentName = tbll.DepartmentName;
                tb.LocationId = tbll.LocationId;
                tb.LocationName = tbll.LocationName;
            }
            else
            {
                myapp.tbl_CallCenterDepartment.Add(tbll);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteDepartment(int id)
        {
            var tb = myapp.tbl_CallCenterDepartment.Where(a => a.DepartmentId == id).SingleOrDefault();
            myapp.tbl_CallCenterDepartment.Remove(tb);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetDepartmentView()
        {
            var result = from c in myapp.tbl_CallCenterDepartment.ToList()
                         select new[] {
                             c.DepartmentId.ToString(),
                             c.DepartmentName,
                             c.LocationName,
                             c.DepartmentId.ToString(),
                             c.LocationId.ToString()
                             };
            return Json(new
            {
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveTaskSLAUser(tbl_TaskSLAUser model)
        {
            model.IsActive = true;
            myapp.tbl_TaskSLAUser.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteTaskSLAUser(int id)
        {
            tbl_TaskSLAUser submodel = myapp.tbl_TaskSLAUser.Where(l => l.TaskSLAUserId == id).SingleOrDefault();
            myapp.tbl_TaskSLAUser.Remove(submodel);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDepartmentByLocation(int id)
        {
            var list = myapp.tbl_CallCenterDepartment.Where(d => d.LocationId == id).OrderBy(d => d.DepartmentName).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //public ActionResult GetDepartmentName(int locationid, int departmentid)
        //{
        //    List<tbl_TaskSLAUser> dept = myapp.tbl_TaskSLAUser.Where(l => l.TaskLocationId == locationid && l.TaskDepartmentId == departmentid && l.CustomDepartmentName != null).ToList();
        //    if (dept.Count > 0)
        //    {
        //        return Json(dept[0].CustomDepartmentName, JsonRequestBehavior.AllowGet);
        //    }
        //    return Json("", JsonRequestBehavior.AllowGet);
        //}
        public async Task<ActionResult> AjaxGetTaskSLAUsers(JQueryDataTableParamModel param)
        {
            //List<tbl_TaskSLAUser> squery = await myapp.tbl_TaskSLAUser.ToListAsync();
            List<TaskSLAUserModel> query = await (from m in myapp.tbl_TaskSLAUser
                                                  join loc in myapp.tbl_Location on m.TaskLocationId equals loc.LocationId
                                                  join dept in myapp.tbl_CallCenterDepartment on m.TaskDepartmentId equals dept.DepartmentId
                                                  join user in myapp.tbl_User on m.SLAUserId equals user.EmpId
                                                  select new TaskSLAUserModel
                                                  {
                                                      Description = m.Description,
                                                      DurationInMinutes = m.DurationInMinutes,
                                                      SLAUserId = m.SLAUserId,
                                                      SLAUserName = user.FirstName,
                                                      TaskDepartmentId = m.TaskDepartmentId,
                                                      TaskDepartmentName = dept.DepartmentName,
                                                      TaskLevel = m.TaskLevel,
                                                      TaskLocationId = m.TaskLocationId,
                                                      TaskLocationName = loc.LocationName,
                                                      TaskSLAUserId = m.TaskSLAUserId,
                                                      //SLAUserEmail = user.EmailId,
                                                      //SLAUserMobile = user.PhoneNumber,
                                                      TypeOfRequest = m.TypeOfRequest,
                                                      SLAUserEmail = m.EmpEmail,
                                                      SLAUserMobile = m.EmpMobile
                                                  }).ToListAsync();

            IEnumerable<TaskSLAUserModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.SLAUserId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.TaskLocationName != null && c.TaskLocationName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.TaskDepartmentName != null && c.TaskDepartmentName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.SLAUserName != null && c.SLAUserName.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.Description != null && c.Description.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.TypeOfRequest != null && c.TypeOfRequest.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.DurationInMinutes != null && c.DurationInMinutes.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<TaskSLAUserModel> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[] {
                                              c.TaskSLAUserId.ToString(),
                                              c.TaskLocationName,
                                              c.TaskDepartmentName,
                                              //c.DepartmentEmail,
                                              c.SLAUserId.ToString(),
                                              c.SLAUserName,
                                              c.SLAUserEmail,
                                              c.SLAUserMobile,
                                              c.TypeOfRequest,
                                              c.TaskLevel.ToString(),
                                              c.DurationInMinutes.ToString(),
                                              //c.Description,
                                              c.TaskSLAUserId.ToString()
                                              };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> AjaxUserviewnew(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {

                List<tbl_TaskCallcenter> tasks = await myapp.tbl_TaskCallcenter.Where(t => t.IsActive == true && t.TaskType != null).ToListAsync();
                bool issearch = false;
                if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
                {
                    issearch = true;
                    DateTime dtfrmdate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                    DateTime dttodate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                    tasks = (from t in tasks
                             where t.CallDateTime.Value.Date >= dtfrmdate.Date && t.CallDateTime.Value.Date <= dttodate.Date
                             select t).ToList();
                }

                {
                    tbl_User dept = await myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).SingleAsync();
                    if (User.IsInRole("Admin"))
                    {
                        ViewBag.EntityType = "admin";
                        ViewBag.Department = dept.DepartmentName;
                    }
                    else
                    {
                        var slausers = await myapp.tbl_TaskSLAUser.Where(l => l.SLAUserId == dept.EmpId).ToListAsync();
                        var tasks1 = tasks.Where(t => t.CreatorDepartmentId == dept.DepartmentId || t.AssignDepartmentId == dept.DepartmentId).ToList();
                        tasks = (from t in tasks
                                 join s in slausers on t.AssignLocationId equals s.TaskLocationId
                                 where t.AssignDepartmentId == s.TaskDepartmentId || t.CreatorId == dept.UserId
                                 select t).ToList();
                        tasks.AddRange(tasks1);
                        tasks = tasks.Distinct().ToList();
                        // tasks = tasks.Where(t => t.AssignDepartmentName == dept.DepartmentId.ToString() && t.CreatorLocationId == t.AssignLocationId).ToList();//the tasks will load basedon the department
                    }
                    if (param.locationid != null && param.locationid != 0)
                    {
                        issearch = true;
                        tasks = tasks.Where(q => q.CreatorLocationId == param.locationid).ToList();
                    }
                    if (param.departmentid != null && param.departmentid != 0)
                    {
                        issearch = true;
                        tasks = tasks.Where(q => q.CreatorDepartmentId == param.departmentid).ToList();
                    }
                    if (param.Emp != null && param.Emp != "" && param.Emp != "0")
                    {
                        issearch = true;
                        tasks = tasks.Where(q => q.AssignId == Convert.ToInt32(param.Emp)).ToList();
                    }
                    if (!string.IsNullOrEmpty(param.status))
                    {
                        issearch = true;
                        if (param.status == "Pending")
                        {
                            param.status = "In Progress";
                        }
                        tasks = tasks.Where(q => q.AssignStatus == param.status).ToList();
                    }
                    if (param.FormType != null && param.FormType != "")
                    {
                        tasks = tasks.Where(l => l.Priority == param.FormType).ToList();
                    }
                    if (param.Expired != null && param.Expired != "")
                    {
                        //if (param.Expired == "Yes")
                        //{
                        //    tasks = tasks.Where(l => l.DocumentReceived == true).ToList();
                        //}
                        //else
                        //{
                        //    tasks = tasks.Where(l => l.DocumentReceived == false).ToList();
                        //}
                        tasks = tasks.Where(l => l.TaskType == param.Expired).ToList();
                    }
                    if (param.sSearch != null && param.sSearch != "")
                    {
                    }
                    else
                    {
                        if (!issearch)
                        {
                            tasks = tasks.Where(q => !(q.AssignStatus.ToLower() == "done" && q.CreatorStatus.ToLower() == "done")).ToList();
                        }
                    }
                }
                if (param.category != null && param.category != "")
                {
                    tasks = tasks.Where(t => t.CategoryOfComplaint == param.category).ToList();
                }

                //if (param.status == null || param.status == "")
                //{
                //    tasks = tasks.Where(t => !(t.AssignStatus == "New" && t.CreatorStatus == "New")).ToList();
                //}

                tasks = tasks.OrderByDescending(t => t.ModifiedOn).ToList();
                //var dat = DateTime.Now.ToLocalTime();
                IEnumerable<tbl_TaskCallcenter> filteredCompanies;
                //Check whether the companies should be filtered by keyword
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = tasks
                       .Where(c => c.TaskId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.CallDateTime != null && c.CallDateTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.CreatorDepartmentName != null && c.CreatorDepartmentName.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.CreatorName != null && c.CreatorName.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.CreatorLocationName != null && c.CreatorLocationName.ToLower().Contains(param.sSearch.ToLower())
                                   //  ||
                                   //c.OtherData != null && c.OtherData.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                 c.ExtensionNo != null && c.ExtensionNo.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                 c.AssertEquipId != null && c.AssertEquipId.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                 c.AssertEquipName != null && c.AssertEquipName.ToLower().Contains(param.sSearch.ToLower())

                                   // ||
                                   //c.EquipName != null && c.EquipName.ToLower().Contains(param.sSearch.ToLower())
                                   // ||
                                   // c.Complaint != null && c.Complaint.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.Description != null && c.Description.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                   c.AssignDepartmentName != null && c.AssignDepartmentName.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.AssignName != null && c.AssignName.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                   c.WorkDoneRemarks != null && c.WorkDoneRemarks.ToLower().Contains(param.sSearch.ToLower())
                                   // ||
                                   //c.EquipName != null && c.EquipName.ToLower().Contains(param.sSearch.ToLower())
                                   // ||
                                   //c.Timetaken != null && c.Timetaken.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.CreatorStatus != null && c.CreatorStatus.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.CallStartDateTime != null && c.CallStartDateTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.CallEndDateTime != null && c.CallEndDateTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  // ||
                                  //dat !=null && dat.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  );
                }
                else
                {
                    filteredCompanies = tasks;
                }
                IEnumerable<tbl_TaskCallcenter> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies
                                               select new[] {
                                                   Convert.ToString(c.TaskId),
                                                   c.CallDateTime.Value.ToString("dd/MM/yyyy HH:mm"),
                                                   Responsetime(c.CallDateTime.Value,c.CallStartDateTime.Value),//ResponseTime 
                                                   c.CreatorName,
                                                   c.CreatorLocationName,
                                                   c.AssertEquipId,
                                                   c.CategoryOfComplaint  +" "+(c.DocumentReceived==true?" - Yes - ":""),
                                                   c.Subject,
                                                   c.AssignLocationName,
                                                   c.AssignName,
                                                   c.WorkDoneRemarks,
                                                   CalculateAge(c.CallStartDateTime.Value,c.CallEndDateTime.Value),
                                                    c.CallEndDateTime.HasValue ? c.CallEndDateTime.Value.ToString("dd/MM/yyyy HH:mm tt") : "",
                                                  c.AssignStatus +"_"+CalculateAge(c.CallDateTime.Value,c.CallEndDateTime.Value,c.AssignStatus),
                                                   Convert.ToString(c.TaskId)
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
        public async Task<ActionResult> AjaxUserviewnew1(JQueryDataTableParamModel param)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");

         
            var userId = User.Identity.Name;

            var dept = await myapp.tbl_User.SingleAsync(u => u.CustomUserId == User.Identity.Name);
            var isAdmin = User.IsInRole("Admin");

            var tasks = await _taskService.GetFilteredTasksAsync(
                param,
                userId: dept.CustomUserId,
                dbUserId: dept.UserId,         // actual DB primary key
                empId: dept.EmpId.Value,
                departmentId: dept.DepartmentId.Value,
                isAdmin: isAdmin
            );

            var result = tasks.Select(c => new[]
            {
    c.TaskId.ToString(),
    c.CallDateTime?.ToString("dd/MM/yyyy HH:mm"),
    Responsetime(c.CallDateTime.Value, c.CallStartDateTime ?? c.CallDateTime.Value),
    c.CreatorName,
    c.CreatorLocationName,
    c.AssertEquipId,
    $"{c.CategoryOfComplaint} {(c.DocumentReceived == true ? "- Yes -" : "")}",
    c.Subject,
    c.AssignLocationName,
    c.AssignName,
    c.WorkDoneRemarks,
    CalculateAge(c.CallStartDateTime ?? DateTime.Now, c.CallEndDateTime ?? DateTime.Now),
    c.CallEndDateTime?.ToString("dd/MM/yyyy HH:mm tt") ?? "",
    $"{c.AssignStatus}_{CalculateAge(c.CallDateTime.Value, c.CallEndDateTime ?? DateTime.Now, c.AssignStatus)}",
    c.TaskId.ToString()
});

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = param.TotalCount,
                iTotalDisplayRecords = param.TotalCount,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public string CalculateAge(DateTime st, DateTime ed, string status)
        {
            if (status == "Done")
            {
                return "0";
            }
            else
            {
                ed = DateTime.Now;
                if (ed > st)
                {
                    TimeSpan t = (ed - st);
                    return t.TotalHours.ToString("0.0");
                }
                else
                {
                    return "";
                }
            }
        }
        public JsonResult WorkDoneRemarksComment(int id, string Remarks)
        {
            List<tbl_TaskCallcenter> tasks = myapp.tbl_TaskCallcenter.Where(t => t.TaskId == id).ToList();
            if (tasks.Count > 0)
            {
                if (tasks[0].AssignId != null && tasks[0].AssignId != 0)
                {
                    tasks[0].WorkDoneRemarks = Remarks;
                    tasks[0].ModifiedOn = DateTime.Now;
                    myapp.SaveChanges();
                }
                else
                {
                    return Json("Please assign task first", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ViewDescription(int id)
        {
            tbl_TaskCallcenter model = myapp.tbl_TaskCallcenter.Where(t => t.TaskId == id).SingleOrDefault();
            return Json(model.Description, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDocumentTaskStatus(int id, bool status)
        {
            tbl_TaskCallcenter model = myapp.tbl_TaskCallcenter.Where(t => t.TaskId == id).SingleOrDefault();
            model.DocumentReceived = status;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult WorkDonwstatus(int id)
        {
            tbl_TaskCallcenter model = myapp.tbl_TaskCallcenter.Where(t => t.TaskId == id).SingleOrDefault();
            return Json(model.WorkDoneRemarks, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDescription(int id, string Description, string Category)
        {
            tbl_TaskCallcenter model = myapp.tbl_TaskCallcenter.Where(t => t.TaskId == id).Single();
            List<tbl_User> listuser = myapp.tbl_User.Where(t => t.CustomUserId == User.Identity.Name).ToList();
            if (model.CreatedBy == User.Identity.Name || model.AssignDepartmentName == listuser[0].DepartmentName)
            {
                if (Category != null && Category != "0" && Category != "")
                    model.CategoryOfComplaint = Category;
                model.Description = Description;
                model.ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("you are not authorized to edit", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult WorkDoneRemarks(int id, string remarks)
        {
            List<tbl_TaskCallcenter> tasks = myapp.tbl_TaskCallcenter.Where(t => t.TaskId == id).ToList();
            if (tasks.Count > 0)
            {
                string currentuserid = User.Identity.Name;
                tbl_User list = (from v in myapp.tbl_User where v.CustomUserId == currentuserid select v).SingleOrDefault();
                if (list != null)
                {
                    if (tasks[0].AssignId == null || tasks[0].AssignId == 0)
                    {
                        return Json("Please assign task first", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (list.UserId == tasks[0].AssignId || User.IsInRole("DepartmentManager"))
                        {
                            tasks[0].WorkDoneRemarks = remarks;
                            if (tasks[0].IsVendorTicket == null || tasks[0].IsVendorTicket == false)
                            {
                                tasks[0].CallEndDateTime = DateTime.Now;
                            }

                            tasks[0].AssignStatus = "Done";
                            tasks[0].CreatorStatus = "Pending from user";
                            tasks[0].ModifiedOn = DateTime.Now;
                            myapp.SaveChanges();
                        }
                        else
                        {
                            return Json("The Request already assign to Some other person", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTask(int id)
        {
            tbl_TaskCallcenter model = myapp.tbl_TaskCallcenter.Where(t => t.TaskId == id).Single();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EndTask(int id)
        {
            List<tbl_TaskCallcenter> tasks = myapp.tbl_TaskCallcenter.Where(t => t.TaskId == id).ToList();
            tbl_User list = (from v in myapp.tbl_User where v.CustomUserId == User.Identity.Name select v).SingleOrDefault();
            if (tasks.Count > 0)
            {
                if (tasks[0].IsVendorTicket == null || tasks[0].IsVendorTicket == false)
                {
                    tasks[0].CallEndDateTime = DateTime.Now;
                }

                tasks[0].AssignStatus = "Done";
                tasks[0].CreatorStatus = "Pending from user";
                if (list.CustomUserId != null)
                {
                    tasks[0].TaskDoneByUserId = User.Identity.Name;
                    tasks[0].TaskDoneByName = list.FirstName;
                    tasks[0].ModifiedOn = DateTime.Now;
                }
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DoneTask(int id)
        {
            string message = "Success";
            List<tbl_TaskCallcenter> list = myapp.tbl_TaskCallcenter.Where(t => t.TaskId == id).ToList();
            string Designation = "";


            List<tbl_TaskCallcenter> listoftask = (from v in myapp.tbl_TaskCallcenter where v.IsActive == true && v.CallDateTime != null select v).ToList();
            if (list.Count > 0)
            {
                if (list[0].CreatedBy == User.Identity.Name)
                {

                    list[0].CreatorStatus = "Done";
                    list[0].ModifiedOn = DateTime.Now;
                    myapp.SaveChanges();
                }

                else if (listoftask.Count > 0)
                {

                    if (Designation != null && Designation == "Manager" && Designation != "")
                    {
                        tbl_TaskCallcenter singlelist = myapp.tbl_TaskCallcenter.Where(t => t.TaskId == id).SingleOrDefault();
                        DateTime calldatetime = singlelist.CallDateTime.Value;
                        double days = (DateTime.Now - calldatetime).TotalDays;
                        if (days >= 7)
                        {
                            list[0].CreatorStatus = "Done";
                            list[0].AssignStatus = "Done";
                            list[0].ModifiedOn = DateTime.Now;
                            myapp.SaveChanges();
                        }
                        else
                        {
                            return Json("Please as creator to close the task", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else { return Json("You don't have rights to do this job", JsonRequestBehavior.AllowGet); }


                }
                else
                {
                    return Json("Please as creator to close the task", JsonRequestBehavior.AllowGet);
                }
                // myapp.SaveChanges();
            }


            return Json(message, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public string AddNewTask(TaskSaveModel model)
        {
            tbl_TaskCallcenter task = new tbl_TaskCallcenter
            {
                AssertEquipId = model.AssertEquipId,
                TaskId = model.TaskId,
                CreatorLocationId = model.CreatorLocationId,
                CreatorLocationName = model.CreatorLocationName,
                CreatorDepartmentId = model.CreatorDepartmentId,
                CreatorDepartmentName = model.CreatorDepartmentName,
                CreatorId = model.CreatorId,
                CreatorName = model.CreatorName,
                CreatorPlace = model.CreatorPlace,
                AssertEquipName = model.AssertEquipName,
                CategoryOfComplaint = model.CategoryOfComplaint,
                Description = model.Description,
                AssignLocationId = model.AssignLocationId,
                AssignLocationName = model.AssignLocationName,
                AssignDepartmentId = model.AssignDepartmentId,
                AssignDepartmentName = model.AssignDepartmentName,
                AssignId = model.AssignId,
                AssignName = model.AssignName,
                WorkDoneRemarks = model.WorkDoneRemarks,
                AssignStatus = model.AssignStatus,
                CreatorStatus = model.CreatorStatus,
                LatestComment = model.LatestComment,
                TaskDoneByUserId = model.TaskDoneByUserId,
                TaskDoneByName = model.TaskDoneByName,
                IsActive = model.IsActive,
                ExtensionNo = model.ExtensionNo,
                EmailId = model.EmailId,
                Others = model.Others,
                Subject = model.Subject,
                IsVendorTicket = model.IsVendorTicket,
                CapexPrepareDate = DateTime.Now,
                CapexApproveDate = DateTime.Now,
                TaskType = model.TaskType,
                Priority = model.Priority,
                DocumentReceived = model.DocumentReceived,
                Doctor = model.Doctor,
                Specialization = model.Specialization
            };
            task.CallDateTime = DateTime.Now;
            task.CallStartDateTime = DateTime.Now;
            task.CallEndDateTime = DateTime.Now;
            task.CreatedBy = User.Identity.Name;
            task.ModifiedBy = User.Identity.Name;
            task.CreatedOn = DateTime.Now;
            task.ModifiedOn = DateTime.Now;
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel
            {
                fromemail = "it_helpdesk@fernandez.foundation"
            };
            task.ModifiedOn = DateTime.Now;
            myapp.tbl_TaskCallcenter.Add(task);
            myapp.SaveChanges();
            List<tbl_TaskSLAUser> list = myapp.tbl_TaskSLAUser.Where(l => l.TypeOfRequest == task.TaskType && l.TaskLocationId == task.AssignLocationId && l.TaskDepartmentId == task.AssignDepartmentId && l.IsActive == true && l.TaskLevel == 1).ToList();

            var emailslist = (from u in list
                              select new
                              {
                                  u.SLAUserId,

                                  u.EmpEmail,
                                  u.EmpMobile

                              }).Distinct().ToList();
            var subemailslist = emailslist.Where(l => l.EmpEmail != null && l.EmpEmail != "").ToList();
            //var subemailslist1 = emailslist.Where(l => l.DepartmentEmail != null && l.DepartmentEmail != "" && l.TaskLevel == 1).Select(l => l.DepartmentEmail).ToList();
            if (task.Subject != null && task.Subject != "")
            {
                //  mailmodel.subject = "Ticket " + task.TaskId + " " + task.Subject.Substring(0, 50) + " - " + task.CreatorLocationName + " " + task.CreatorDepartmentName;
                mailmodel.subject = "Ticket " + task.TaskId + " - " + task.CreatorLocationName + " " + task.CreatorDepartmentName + " " + task.CreatorName + ". Given " + task.TaskType + " " + task.CategoryOfComplaint;
            }
            string mobilenumber = "";
            if (subemailslist != null && subemailslist.Count > 0)
            {
                string body = "";
                string mailbody = GetEmailBody(task, new List<tbl_TaskSLAUser>());
                using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_Asset.html")))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("[[Heading]]", "The Ticket " + task.TaskId + " was created for " + task.CreatorDepartmentName + "");
                body = body.Replace("[[table]]", mailbody);
                mailmodel.body = body;

                //mailmodel.body = "<p style='font-size:15px;'>Dear,</p>";
                //mailmodel.body += "<p style='font-size:15px;'>The Ticket " + task.TaskId + " was created for " + task.CreatorDepartmentName + "</p>";
                //mailmodel.body += GetEmailBody(task, new List<tbl_TaskSLAUser>());
                mailmodel.filepath = "";
                mailmodel.fromname = "New Ticket Created";
                mailmodel.ccemail = "";
                int i = 0;
                foreach (var detail in subemailslist)
                {
                    if (detail.EmpMobile != null && detail.EmpMobile != "")
                        mobilenumber += detail.EmpMobile + ",";
                    if (i == 0)
                    {
                        mailmodel.toemail = detail.EmpEmail;
                    }
                    else
                    {
                        mailmodel.ccemail += detail.EmpEmail + ",";
                    }
                    i++;
                }
                if (subemailslist.Count > 1)
                {
                    mailmodel.ccemail = mailmodel.ccemail.TrimEnd(',');
                    mobilenumber = mobilenumber.TrimEnd(',');
                }
                //if (task.AssignDepartmentName.Contains("Information"))
                //{
                //    mailmodel.toemail = "it@fernandez.foundation";
                //    mailmodel.ccemail = "";
                //cm.SendEmail(mailmodel);
                Thread email = new Thread(delegate ()
                {
                    cm.SendEmail(mailmodel);
                });

                email.IsBackground = true;
                email.Start();
                //}
                //if (subemailslist1.Count > 0)
                //{
                //    foreach (var ccemail in subemailslist1)
                //    {
                //        if (mailmodel.ccemail != null && mailmodel.ccemail != "")
                //        {
                //            mailmodel.ccemail = mailmodel.ccemail + "," + ccemail;
                //        }
                //        else
                //        {
                //            mailmodel.ccemail = ccemail;
                //        }
                //    }
                //}

            }
            if (mobilenumber != null && mobilenumber != "")
            {
                SendSms sms = new SendSms();
                //sms.SendSmsToEmployee(mobilenumber, mailmodel.subject + " " + task.Subject);
            }

            return "You have succesfully created a task.";
        }
        public string GetEmailBodyBulk(List<tbl_TaskCallcenter> tasks, tbl_TaskSLAUser userlist)
        {

            string mailbody = "";
            mailbody += "<p style='font-size:12px;'>Ticket Details are: </p><table style='width:103%;margin:auto;border:solid 1px #eee;'>";
            mailbody += "<tr><th style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Type</th><th style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Priority</th><th style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Task Id</th><th style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Created On</th><th style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Creator</th>";
            mailbody += "<th style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Assign Location</th><th style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Assign Department</th><th style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Assign To</th><th style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Category Of Complaint</th>";
            mailbody += "<th style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Subject</th><th style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>MR/Phone Number</th><th style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Description</th><th style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Email Id</th></tr>";
            foreach (var task in tasks)
            {
                mailbody += "<tr><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.TaskType + "</td>";
                mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.Priority + "</td>";
                mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.TaskId + "</td>";
                mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + (task.CreatedOn.HasValue ? task.CreatedOn.Value.ToString("dd/MM/yyyy hh:mm") : "") + "</td>";
                mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.CreatorLocationName + " " + task.CreatorDepartmentName + " " + task.CreatorName + "</td>";
                mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.AssignLocationName + "</td>";
                mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.AssignDepartmentName + "</td>";
                if (task.AssignName != null && task.AssignName != "")
                {
                    mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.AssignName + "</td>";
                }
                else
                {
                    mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>&nbsp;</td>";

                }
                mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.CategoryOfComplaint + "</td>";
                mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.Subject + "</td>";
                mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.AssertEquipId + "</td>";

                mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.Description + "</td>";
                //mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.ExtensionNo + "</td>";
                mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.EmailId + "</td>";
                //foreach (var list in userlist)
                //{

                //mailbody += "<td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + userlist.Description + "</td></tr>";
                //}

            }
            mailbody += "</table><br />";
            mailbody += "<p style='font-size:12px;'>This is an auto generated mail,Don't reply to this.</p>";
            return mailbody;
        }
        public string GetEmailBody(tbl_TaskCallcenter task, List<tbl_TaskSLAUser> userlist)
        {
            string mailbody = "";
            mailbody += "<p style='font-size:12px;'>Ticket Details are: </p><table style='width:60%;margin:auto;border:solid 1px #eee;'>";
            mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Type</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.TaskType + "</td></tr>";
            mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Priority</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.Priority + "</td></tr>";
            mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Task Id</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.TaskId + "</td></tr>";
            mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Creator</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.CreatorLocationName + " " + task.CreatorDepartmentName + " " + task.CreatorName + "</td></tr>";
            mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Assign Location</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.AssignLocationName + "</td></tr>";
            mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Assign Department</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.AssignDepartmentName + "</td></tr>";
            if (task.AssignName != null && task.AssignName != "")
            {
                mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Assign To</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.AssignName + "</td></tr>";
            }
            mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Category Of Complaint</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.CategoryOfComplaint + "</td></tr>";
            mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Subject</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.Subject + "</td></tr>";
            mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>MR/Phone Number</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.AssertEquipId + "</td></tr>";

            mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Description</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.Description + "</td></tr>";
            mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Extension No</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.ExtensionNo + "</td></tr>";
            mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>Email Id</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.EmailId + "</td></tr>";
            foreach (var list in userlist)
            {
                string Leveldescription = "";
                if (list.TaskLevel == 1)
                {
                    Leveldescription = "First Level User : ";
                }
                else if (list.TaskLevel == 2)
                {
                    Leveldescription = "Second Level User : ";
                }
                else if (list.TaskLevel == 3)
                {
                    Leveldescription = "Third Level User : ";
                }
                else if (list.TaskLevel == 4)
                {
                    Leveldescription = "Fourth Level User : ";
                }
                else if (list.TaskLevel == 5)
                {
                    Leveldescription = "Fifth Level User : ";
                }
                else if (list.TaskLevel == 6)
                {
                    Leveldescription = "Sixth Level User : ";
                }
                mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + Leveldescription + "</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + list.Description + "</td></tr>";
            }
            //if (userlist.Count == 0)
            //{
            //    string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
            //    mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'><a href='" + baseUrl + "/Home/StartTask?id=" + task.TaskId + "' target='_blank'>Pick Up</a></td><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;'>" + task.EmailId + "</td></tr>";
            //}
            mailbody += "</table>";
            return mailbody;
        }
        public double GetMinutesDifference(DateTime calldate)
        {
           
            TimeSpan slaStart = new TimeSpan(8, 0, 0); // 8:00 AM
            TimeSpan slaEnd = new TimeSpan(20, 0, 0);  // 8:00 PM
            DateTime now = DateTime.Now;

            
            double slaMinutes = 0;

            if (calldate.TimeOfDay < slaStart)
            {
               
                calldate = calldate.Date + slaStart;
            }
            else if (calldate.TimeOfDay >= slaEnd)
            {
                
                calldate = calldate.Date.AddDays(1) + slaStart;
            }

            while (calldate < now)
            {
              
                DateTime endOfSlaWindow = calldate.Date + slaEnd;

                if (now <= endOfSlaWindow)
                {
                  
                    slaMinutes += (now - calldate).TotalMinutes;
                    break;
                }
                else
                {
                    slaMinutes += (endOfSlaWindow - calldate).TotalMinutes;
                    calldate = calldate.AddDays(1).Date + slaStart;
                }
            }
            return slaMinutes;
        }

        public string SendemailtoNextlevel()
        {
            DateTime dtnow = DateTime.Now;
            DateTime checkdate = ProjectConvert.ConverDateStringtoDatetime("01/06/2021");
            if (dtnow.Hour > 7 && dtnow.Hour < 17)
            {
                List<tbl_TaskSLAUser> taskslausers = myapp.tbl_TaskSLAUser.ToList();
                //List<tbl_TaskSLAUser> allusers = new List<tbl_TaskSLAUser>();
                List<tbl_User> users = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
                List<tbl_TaskCallcenter> tasklist = myapp.tbl_TaskCallcenter.Where(l => l.AssignStatus != "Done" && l.TaskType != null && l.CallDateTime > checkdate).ToList();

                var assignlocation = tasklist.Select(l => l.AssignLocationId).Distinct().ToList();

                taskslausers = taskslausers.Where(l => l.TaskLevel > 1 && assignlocation.Contains(l.TaskLocationId)).ToList();
                foreach (var usr in taskslausers)
                {
                    if ((usr.TaskLocationId == 4 || usr.TaskLocationId == 9) && dtnow.Hour > 16 && dtnow.Hour < 20)
                    {

                    }
                    else
                    {
                        var list = tasklist.Where(l => l.AssignLocationId == usr.TaskLocationId && l.AssignDepartmentId == usr.TaskDepartmentId).ToList();
                        int SLAMinutes = 360;
                        //if (task.TaskType == "Complaints")
                        //SLAMinutes = "240";
                        bool sendemail1 = false;
                        if (usr.TaskLevel == 2)
                        {
                            list = (from l in list
                                    let minutes = GetMinutesDifference(l.CallDateTime.Value)
                                    where minutes > SLAMinutes && minutes < 420
                                    select l).ToList();
                            sendemail1 = true;

                        }
                        else if (usr.TaskLevel == 3)
                        {
                            SLAMinutes = 420;
                            list = (from l in list
                                    let minutes = GetMinutesDifference(l.CallDateTime.Value)
                                    where minutes > SLAMinutes
                                    select l).ToList();
                            sendemail1 = true;
                        }
                        if (sendemail1 && list.Count > 0)
                        {
                            DateTime dt = DateTime.Now.Date;
                            string JobName = "HelpDeskSLA_" + usr.SLAUserId;
                            var checkcount = myapp.tbl_JobsHistory.Where(l => l.JobName == JobName && l.JobExecutedDate.Value == dt).Count();
                            if (checkcount == 0)
                            {
                                CustomModel cm = new CustomModel();
                                MailModel mailmodel = new MailModel
                                {
                                    fromemail = "it_helpdesk@fernandez.foundation"
                                };
                                mailmodel.filepath = "";
                                mailmodel.fromname = "HelpDesk SLA Reminder";
                                mailmodel.ccemail = "";
                                mailmodel.subject = "Call Center Helpdesk Reminder";
                                var userdetails = users.Where(l => l.EmpId == usr.SLAUserId).SingleOrDefault();
                                if (userdetails != null)
                                {
                                    mailmodel.toemail = userdetails.EmailId;
                                    //mailmodel.toemail = "phanisrinivas111@gmail.com";
                                    string body = "";
                                    using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/BulkCallCenter_Template.html")))
                                    {
                                        body = reader.ReadToEnd();
                                    }
                                    body = body.Replace("[[employee_name]]", userdetails.FirstName);
                                    body = body.Replace("[[callCenterTable]]", GetEmailBodyBulk(list, usr));
                                    mailmodel.body = body;
                                    mailmodel.ccemail = "it_helpdesk@fernandez.foundation";
                                    if (usr.EmpEmail != null && usr.EmpEmail != "")
                                    {
                                        mailmodel.ccemail = mailmodel.ccemail + "," + usr.EmpEmail;
                                    }
                                   
                                    cm.SendEmail(mailmodel);
                                    tbl_JobsHistory tbl = new tbl_JobsHistory
                                    {
                                        Environment = "PROD",
                                        JobExcutedHour = DateTime.Now.Hour,
                                        JobExcutedMinute = DateTime.Now.Minute,
                                        JobExecutedDate = DateTime.Now.Date,
                                        JobName = JobName,
                                        JobStatus = true,
                                        Message = "Success"
                                    };
                                    myapp.tbl_JobsHistory.Add(tbl);
                                    myapp.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            return "You have succesfully created a task.";
        }
        public string Responsetime(DateTime st, DateTime ed)
        {
            if (ed > st)
            {
                string startTime = "08:00 AM";
                string endTime = "04:00 PM";
                TimeSpan checktime1 = DateTime.Parse(startTime).TimeOfDay;
                TimeSpan checktime2 = DateTime.Parse(endTime).TimeOfDay;

                TimeSpan time = st.TimeOfDay;
                if (time >= checktime1 && time <= checktime2)
                {

                }
                else
                {
                    time = checktime1;
                }
                TimeSpan time2 = ed.TimeOfDay;

                TimeSpan t = (ed - st);

                string answer = "";

                answer = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                if (st == ed)
                {
                    return "0";
                }

                return answer;
            }
            else
            {
                return "0";
            }
        }
        public string CalculateAge(DateTime st, DateTime ed)
        {
            if (ed > st)
            {
                TimeSpan t = (ed - st);
                string answer = "";
                if (t.TotalMinutes > 1)
                {
                    answer = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Days, t.Hours, t.Minutes);
                }
                return answer;
            }
            else
            {
                return "";
            }
        }
        public async Task<JsonResult> GetCategoryByDepartmentId(int id, string type = "")
        {
            var list = await myapp.tbl_DepartmentVsCategory.Where(l => l.DepartmentId == id && l.IsActive == true).ToListAsync();
            var model = await (from c in myapp.tbl_Category
                               join l in list on c.CategoryId equals l.CategoryId
                               select c).OrderBy(l => l.Name).ToListAsync();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetCpxCountByCpxRelatedTo()
        {
            var valuesgroup = await (from cpx in myapp.tbl_TaskCallcenter
                                     where cpx.TaskType != null
                                     group cpx by cpx.AssignLocationName into cpxGroup
                                     select new
                                     {
                                         Key = cpxGroup.Key,
                                         Count = cpxGroup.Count(),
                                     }).ToListAsync();
            return Json(valuesgroup, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetCpxCountByRequestForTheDepartment()
        {
            var commondept = await myapp.tbl_CommonDepartment.ToListAsync();
            var valuesgroup = await (from cpx in myapp.tbl_TaskCallcenter
                                     where cpx.TaskType != null
                                     group cpx by cpx.AssignDepartmentName into cpxGroup
                                     select new
                                     {
                                         Key = cpxGroup.Key,
                                         Count = cpxGroup.Count(),
                                     }).ToListAsync();
            return Json(valuesgroup, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> GetCpxCountByProjectTitle()
        {
            var valuesgroup = await (from cpx in myapp.tbl_TaskCallcenter
                                     where cpx.TaskType != null
                                     group cpx by cpx.CategoryOfComplaint into cpxGroup
                                     select new
                                     {
                                         Key = cpxGroup.Key,
                                         Count = cpxGroup.Count(),
                                     }).ToListAsync();
            return Json(valuesgroup, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReOpenTask(int id)
        {
            List<tbl_TaskCallcenter> tasks = myapp.tbl_TaskCallcenter.Where(t => t.TaskId == id).ToList();
            // var list = (from v in myapp.tbl_Task where v.AssignStatus != null && v.AssignStatus == "In Progress" select v).SingleOrDefault();

            if (tasks.Count > 0)
            {
                // tasks[0].CallStartDateTime = DateTime.Now;
                tasks[0].AssignStatus = "Re Open";
                tasks[0].CreatorStatus = "Re Open";
                tasks[0].ModifiedOn = DateTime.Now;
                myapp.SaveChanges();

                //tbl_TaskComment tsc = new tbl_TaskComment
                //{
                //    Comment = "Task Re Open - " + tasks[0].AssignName,
                //    TaskId = id,
                //    CommentedBy = User.Identity.Name,
                //    CommentDate = DateTime.Now
                //};
                //myapp.tbl_tas.Add(tsc);
                //myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult StartTask(int id)
        {
            string msg = "Success";
            List<tbl_TaskCallcenter> tasks = myapp.tbl_TaskCallcenter.Where(t => t.TaskId == id).ToList();
            List<tbl_User> user = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).ToList();
            if (tasks.Count > 0 && user.Count > 0)
            {

                if (tasks[0].AssignStatus != "Done" && (tasks[0].AssignId == null || tasks[0].AssignId == 0 || tasks[0].Others == "NewAssign"))
                {
                    tasks[0].AssignId = user[0].UserId;
                    tasks[0].AssignName = user[0].FirstName;
                    tasks[0].CallStartDateTime = DateTime.Now;
                    tasks[0].ModifiedOn = DateTime.Now;
                    tasks[0].AssignStatus = "In Progress";
                    tasks[0].CreatorStatus = "In Progress";
                    tasks[0].Others = "";
                    myapp.SaveChanges();
                }

                else
                {
                    msg = "The Ticked already picked by some one";
                }

            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> MyTasks_ExportToExcel(string FromDate, string ToDate, int locationid, int departmentid, string status, string emp, string category)
        {
            List<tbl_TaskCallcenter> tasks = await myapp.tbl_TaskCallcenter.Where(t => t.IsActive == true && t.TaskType != null).ToListAsync();

            if (category != null && category != "")
            {
                tasks = tasks.Where(t => t.CategoryOfComplaint == category).ToList();
            }
            if (FromDate != null && FromDate != "" && ToDate != null && ToDate != "")
            {
                DateTime dtfrmdate = ProjectConvert.ConverDateStringtoDatetime(FromDate);
                DateTime dttodate = ProjectConvert.ConverDateStringtoDatetime(ToDate);
                if (dtfrmdate == dttodate)
                {
                    dttodate = dttodate.AddDays(1);
                }
                tasks = (from t in tasks
                         where t.CallDateTime.Value >= dtfrmdate && t.CallDateTime.Value <= dttodate
                         select t).ToList();
            }
            tasks = tasks.OrderByDescending(t => t.ModifiedOn).ToList();
            List<tbl_Location> locationslist = myapp.tbl_Location.ToList();
            System.Data.DataTable products = new System.Data.DataTable("MyTasksDataTable");
            // products.Rows.Add("Client  : " + list[0].ClientName + " SOA From : " + FromDate + " To : " + ToDate);
            products.Columns.Add("ID", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Assign Location - Deaprtment", typeof(string));
            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("Ticket raised time", typeof(string));
            products.Columns.Add("Type", typeof(string));
            products.Columns.Add("User", typeof(string));
            products.Columns.Add("Category", typeof(string));
            products.Columns.Add("MR / Phone number", typeof(string));
            products.Columns.Add("Subject", typeof(string));
            products.Columns.Add("Priority", typeof(string));

            //products.Columns.Add("Equip", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("IT Engineer Response time(h:m:s)", typeof(string));

            products.Columns.Add("Picked up by", typeof(string));
            //products.Columns.Add("Assined IT support Engineer(s)", typeof(string));
            products.Columns.Add("Status", typeof(string));
            //products.Columns.Add("Closed By", typeof(string));

            products.Columns.Add("Close Time(d:h:m)", typeof(string));
            products.Columns.Add("Closure Date", typeof(string));
            products.Columns.Add("Closure remarks", typeof(string));
            products.Columns.Add("Closed at Level1", typeof(string));
            products.Columns.Add("Closed at Level2", typeof(string));
            products.Columns.Add("SLA", typeof(string));
            products.Columns.Add("Total Hours", typeof(string));
            //products.Columns.Add("Assing vendor Comments", typeof(string));
            //products.Columns.Add("Creator Status", typeof(string));

            //products.Columns.Add("Issue Closed(d:h:m)", typeof(string));

            foreach (tbl_TaskCallcenter c in tasks)
            {
                TimeSpan timesp = (c.CallEndDateTime.Value - c.CallStartDateTime.Value);
                string totlatimetaken = CalculateAge(c.CallStartDateTime.Value, c.CallEndDateTime.Value);
                //if (totlatimetaken != null && totlatimetaken != "")
                //{
                //List<tbl_TaskComment> comments = myapp.tbl_TaskComment.Where(t => t.TaskId == c.TaskId).ToList();
                //comments = comments.OrderBy(cm => cm.CommentDate).ToList();
                //string Taskdoneby = "";
                //long taskdonebyid = 0;
                //long taskpickupbyid = 0;
                //List<tbl_TaskComment> Taskdonebycomments = comments.Where(l => l.Comment.Contains("Task Done by ")).OrderByDescending(cm => cm.CommentDate).ToList();
                //if (Taskdonebycomments.Count > 0)
                //{
                //    Taskdoneby = Taskdonebycomments[0].Comment.Replace("Task Done by ", "");
                //    taskdonebyid = Taskdonebycomments[0].TaskCommentId;
                //}
                //string firstpickupby = "";
                //List<tbl_TaskComment> pickupbycomments = comments.Where(l => l.Comment.Contains("Task Pick Up By")).OrderByDescending(cm => cm.CommentDate).ToList();
                //if (pickupbycomments.Count > 0)
                //{
                //    firstpickupby = pickupbycomments[0].Comment.Replace("Task Pick Up By", "");
                //    taskpickupbyid = pickupbycomments[0].TaskCommentId;
                //}
                //string strcmmt = "";
                //}

                string closedat1 = "No";
                string closedat2 = "No";
                if (c.TaskType == "Complaints" && c.AssignStatus == "Done")
                {
                    if (timesp.TotalMinutes < 120)
                    {
                        closedat1 = "Yes";
                    }
                    else if (timesp.TotalMinutes < 240)
                    {
                        closedat2 = "Yes";
                    }
                }
                else if (c.AssignStatus == "Done")
                {
                    if (timesp.TotalMinutes < 240)
                    {
                        closedat1 = "Yes";
                    }
                    else if (timesp.TotalMinutes < 500)
                    {
                        closedat2 = "Yes";
                    }
                }
                if (c.AssignStatus != "Done")
                {
                    closedat1 = "No";
                    closedat2 = "No";
                }
                string SLA = "";
                if (c.TaskType == "Complaints" && timesp.TotalHours > 4)
                {
                    SLA = "SLA Missed";
                }
                else if (timesp.TotalHours > 6)
                {
                    SLA = "SLA Missed";
                }
                products.Rows.Add(Convert.ToString(c.TaskId),
                    c.CreatorLocationName,
                     c.AssignLocationName + " " + c.AssignDepartmentName,
                    c.CallDateTime.Value.ToString("dd/MM/yyyy"),
                    c.CallDateTime.Value.ToShortTimeString(),
                    c.TaskType,
                    c.CreatorDepartmentName + " " + c.CreatorName,
                    c.CategoryOfComplaint,
                    c.AssertEquipId,
                    c.Subject,
                    c.Priority,
                    c.Description,
                    Responsetime(c.CallDateTime.Value, c.CallStartDateTime.Value),//ResponseTime                   
                    c.AssignName,
                    //assignto, 
                    c.AssignStatus,
                    //Taskdoneby,
                    totlatimetaken,
                   c.CallEndDateTime.HasValue ? c.CallEndDateTime.Value.ToString("dd/MM/yyyy HH:mm tt") : "",
                    c.WorkDoneRemarks,
                    closedat1,
                    closedat2,
                    SLA,
                    timesp.TotalHours.ToString("0")
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
            string filename = "MyTasks_GridData.xls";
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