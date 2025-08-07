using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;
using WebApplicationHsApp.Service;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private static readonly TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        private readonly TaskService _taskService = new TaskService();
        [AllowAnonymous]
        public ActionResult Index()
        {
            if (Request.IsAuthenticated && User.Identity.Name != null && User.Identity.Name != "")
            {
                List<tbl_User> userfirsttime = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).ToList();
                if (userfirsttime.Count > 0 && userfirsttime[0].ChangePassword != null && userfirsttime[0].ChangePassword.Value)
                {
                    return RedirectToAction("ChangePassword", "Manage");
                }
            }
            List<tbl_Settings> notification = myapp.tbl_Settings.Where(l => l.SettingKey == "HomepagePopup").ToList();
            if (notification.Count > 0)
            {
                ViewBag.Nofication = notification[0].SettingValue;
            }
            List<tbl_Settings> weekimg = myapp.tbl_Settings.Where(l => l.SettingKey == "WeekImage").ToList();
            if (weekimg.Count > 0)
            {
                ViewBag.WeekImage = weekimg[0].SettingValue;
            }
            List<tbl_HomePageVideos> model = myapp.tbl_HomePageVideos.Where(l => l.IsActive == true).OrderByDescending(l => l.HomePageVideosId).Take(1).ToList();
            if (model.Count > 0)
            {
                ViewBag.VideoUrl = model[0].VideoUrl;
                ViewBag.VideoId = model[0].HomePageVideosId;
                ViewBag.VideoDescription = model[0].Description;
                ViewBag.VideoName = model[0].Name;
                ViewBag.VideoDate = model[0].CreatedOn.Value.ToString("dd MMM yyyy");
            }
            else
            {

                ViewBag.VideoUrl = "";
                ViewBag.VideoId = "";
                ViewBag.VideoDescription = "";
                ViewBag.VideoName = "";
                ViewBag.VideoDate = "";
            }
            return View();
        }
        [AllowAnonymous]
        public ActionResult StraightFromTheHeartDrEvita()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
        [Authorize(Roles = "Admin,Employee,OutSource")]
        public async Task<ActionResult> MyTask()
        {
            if (User.IsInRole("OutSource"))
            {
                List<tbl_OutSourceUser> list = await myapp.tbl_OutSourceUser.Where(u => u.CustomUserId == User.Identity.Name).ToListAsync();
                if (list.Count > 0)
                {
                    ViewBag.CurrentUser = list[0];
                }
                else { ViewBag.CurrentUser = ""; }
            }
            else
            {
                List<tbl_User> list = await myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).ToListAsync();
                if (list.Count > 0)
                {
                    ViewBag.CurrentUser = list[0];
                }
                else { ViewBag.CurrentUser = ""; }
            }
            if (User.IsInRole("Admin") || User.IsInRole("DepartmentManager"))
            {
                //ViewBag.New = await myapp.tbl_Task.Where(l => l.AssignStatus == "New").CountAsync();
                //ViewBag.inprogess = await myapp.tbl_Task.Where(l => l.AssignStatus == "In Progress").CountAsync();
                //ViewBag.Reopen = await myapp.tbl_Task.Where(l => l.AssignStatus == "Re Open").CountAsync();
                //ViewBag.Completed = await myapp.tbl_Task.Where(l => l.AssignStatus == "Done").CountAsync();
            }
            return View();
        }
        public ActionResult MyTaskView(int id)
        {
            return View();
        }
        public ActionResult MyTaskSettingsToAutomate()
        {
            return View();
        }
        //[Authorize(Roles = "Admin,Employee,OutSource")]
        public ActionResult HelpDeskReport()
        {
            List<tbl_Task> list = myapp.tbl_Task.Where(l => l.AssignDepartmentName == "Information Technology" || l.AssignDepartmentName == "IT").ToList();
            HelpDeskReportViewModel Hd = new HelpDeskReportViewModel
            {
                TotalIssues = list.Count(),
                TotalIssuesClosed = list.Where(l => l.CreatorStatus == "Done").Count(),
                NoOfShivamIssues = list.Where(l => l.CategoryOfComplaint == "Shivam").Count(),
                NoOfTicketsOpen = list.Where(l => l.AssignStatus != "Done").Count(),
                NoOfHardwareandOtherIssues = list.Where(l => l.CategoryOfComplaint != "Shivam").Count(),
                NoOfIssuesclosedWithin24Hours = (from l in list
                                                 where (l.CallStartDateTime.Value.Subtract(l.CallEndDateTime.Value).TotalHours <= 24)
                                                 select l).Count(),
                NoOfIssuesclosedAfter24Hours = (from l in list
                                                where (l.CallStartDateTime.Value.Subtract(l.CallEndDateTime.Value).TotalHours > 24)
                                                select l).Count()
            };
            return View(Hd);
        }
        public JsonResult GettheCountReport(string fromdate, string todate)
        {
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(fromdate);
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(todate);
            double totaldays = dtto.Subtract(dtfrom).TotalDays;
            List<tbl_Task> list = myapp.tbl_Task.Where(l => (l.AssignDepartmentName == "Information Technology" || l.AssignDepartmentName == "IT") && l.CallDateTime >= dtfrom && l.CallDateTime <= dtto).ToList();
            double avgtktsperday = list.Count / totaldays;//Average Tickets raised per Day
            double avgtktsperdayclosed = list.Where(l => l.CreatorStatus == "Done").ToList().Count / totaldays;//Average Tickets raised per Day

            double totalminutes = 0;
            foreach (tbl_Task l in list)
            {
                totalminutes += ((l.CallStartDateTime - l.CallDateTime).Value.TotalMinutes);
            }

            string AverageTicketsraisedperDay = avgtktsperday.ToString("0.0");
            string AverageTicketsClosedperDay = avgtktsperdayclosed.ToString("0.0");
            string AverageResponseTime = (totalminutes / totaldays).ToString("0.0");

            Dictionary<string, string> dict = new Dictionary<string, string>
            {
                { "Average Tickets Raised Per Day", AverageTicketsraisedperDay },
                { "Average Tickets Closed Per Day", AverageTicketsClosedperDay },
                { "Average Response Time", AverageResponseTime }
            };
            int NoOfIssuesclosedWithin24Hours = (from l in list
                                                 where (l.CallStartDateTime.Value.Subtract(l.CallEndDateTime.Value).TotalHours <= 24)
                                                 select l).Count();
            int NoOfIssuesclosedAfter24Hours = (from l in list
                                                where (l.CallStartDateTime.Value.Subtract(l.CallEndDateTime.Value).TotalHours > 24)
                                                select l).Count();
            List<int> department = myapp.tbl_Department.Where(l => l.DepartmentName == "Information Technology" || l.DepartmentName == "IT").Select(l => l.DepartmentId).ToList();
            List<string> catlist = (from dc in myapp.tbl_DepartmentVsCategory
                                    join dep in department on dc.DepartmentId equals dep
                                    join c in myapp.tbl_Category on dc.CategoryId equals c.CategoryId
                                    select c.Name).Distinct().ToList();
            foreach (string v in catlist)
            {
                double countofissues = list.Where(l => l.CategoryOfComplaint == v).Count() / totaldays;
                dict.Add("Average Tickets raised " + v + " per Day", countofissues.ToString("0.0"));
            }

            dict.Add("No of Issues closed with in 24 hours", NoOfIssuesclosedWithin24Hours.ToString("0.0"));
            dict.Add("No of issues closed after 24 hours", NoOfIssuesclosedAfter24Hours.ToString("0.0"));
            dict.Add("No Of Tickets Open", list.Where(l => l.AssignStatus != "Done").Count().ToString("0.0"));


            return Json(dict, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult Employees()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult AjaxGetEmployees(JQueryDataTableParamModel param)
        {
            var query = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
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
                                ||
                              c.Extenstion != null && c.Extenstion.ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_User> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[] {
                                               c.CustomUserId,
                                              c.LocationName,
                                              c.DepartmentName,
                                              c.FirstName+" "+c.LastName,
                                              //c.CustomUserId,
                                              c.EmailId,
                                              c.PlaceAllocation,
                                              c.PhoneNumber,
                                              c.Extenstion,
                                              c.DepartmentName,
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
        [AllowAnonymous]
        public ActionResult EmployeeSearch()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult GetEmployeeSearch(string searchTerm)
        {
            var query = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            query = query
                    .Where(c => c.CustomUserId.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                                 c.LocationName != null && c.LocationName.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                               c.FirstName != null && c.FirstName.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                               c.LastName != null && c.LastName.ToLower().Contains(searchTerm.ToLower())
                                 ||
                               c.EmailId != null && c.EmailId.ToLower().Contains(searchTerm.ToLower())
                               ||
                               c.PlaceAllocation != null && c.PlaceAllocation.ToLower().Contains(searchTerm.ToLower())

                               ).ToList();
            var resulst = (from q in query
                           select new
                           {
                               id = q.UserId,
                               text = q.FirstName + " " + q.LastName + " - " + q.Designation + " - " + q.DepartmentName
                           }).ToList();
            return Json(resulst, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult GetCustomIdEmployeeSearch(string searchTerm)
        {
            var query = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            query = query
                    .Where(c => c.CustomUserId.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                                 c.LocationName != null && c.LocationName.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                               c.FirstName != null && c.FirstName.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                               c.LastName != null && c.LastName.ToLower().Contains(searchTerm.ToLower())

                                 ||
                               c.EmailId != null && c.EmailId.ToLower().Contains(searchTerm.ToLower())
                               ||
                               c.PlaceAllocation != null && c.PlaceAllocation.ToLower().Contains(searchTerm.ToLower())

                               ).ToList();
            var resulst = (from q in query
                           select new
                           {
                               id = q.CustomUserId,
                               text = q.FirstName + " " + q.LastName + " - " + q.Designation + " - " + q.DepartmentName
                           }).ToList();
            return Json(resulst, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult GetEmployeeById(int Userid)
        {
            var query = myapp.tbl_User.Where(l => l.IsActive == true && l.UserId == Userid).SingleOrDefault();
            var model = new tbl_User()
            {
                ChangePassword = query.ChangePassword,
                Comments = query.Comments,
                CreatedBy = query.CreatedBy,
                CreatedOn = query.CreatedOn,
                CugNumber = query.CugNumber,
                CustomUserId = query.CustomUserId,
                DateOfBirth = query.DateOfBirth,
                DateOfJoining = query.DateOfJoining,
                DateOfLeaving = query.DateOfLeaving,
                DepartmentId = query.DepartmentId,
                DepartmentId1 = query.DepartmentId1,
                DepartmentId2 = query.DepartmentId2,
                DepartmentName = query.DepartmentName,
                DepartmentName1 = query.DepartmentName1,
                DepartmentName2 = query.DepartmentName2,
                Designation = query.Designation,
                DesignationID = query.DesignationID,
                DigitalSignature = query.DigitalSignature,
                EmailId = query.EmailId,
                EmpId = query.EmpId,
                EmployeePhoto = query.EmployeePhoto,
                Extenstion = query.Extenstion,
                FirstName = query.FirstName,
                Gender = query.Gender,
                IsActive = query.IsActive,
                IsAppLogin = query.IsAppLogin,
                IsEmployee = query.IsEmployee,
                IsEmployeesReporting = query.IsEmployeesReporting,
                IsOffRollDoctor = query.IsOffRollDoctor,
                IsOnRollDoctor = query.IsOnRollDoctor,
                LastName = query.LastName,
                LocationId = query.LocationId,
                LocationName = query.LocationName,
                PhoneNumber = query.PhoneNumber,
                PlaceAllocation = query.PlaceAllocation,
                SecurityAnswner = query.SecurityAnswner,
                SecurityQuestion = query.SecurityQuestion,
                SubDepartmentId = query.SubDepartmentId,
                SubDepartmentName = query.SubDepartmentName,
                UserId = query.UserId,
                UserType = query.UserType

            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult GetEmployeeByEmpId(int empId)
        {
            var query = myapp.tbl_User.Where(l => l.IsActive == true && l.EmpId == empId).SingleOrDefault();
            var model = new tbl_User()
            {
                ChangePassword = query.ChangePassword,
                Comments = query.Comments,
                CreatedBy = query.CreatedBy,
                CreatedOn = query.CreatedOn,
                CugNumber = query.CugNumber,
                CustomUserId = query.CustomUserId,
                DateOfBirth = query.DateOfBirth,
                DateOfJoining = query.DateOfJoining,
                DateOfLeaving = query.DateOfLeaving,
                DepartmentId = query.DepartmentId,
                DepartmentId1 = query.DepartmentId1,
                DepartmentId2 = query.DepartmentId2,
                DepartmentName = query.DepartmentName,
                DepartmentName1 = query.DepartmentName1,
                DepartmentName2 = query.DepartmentName2,
                Designation = query.Designation,
                DesignationID = query.DesignationID,
                DigitalSignature = query.DigitalSignature,
                EmailId = query.EmailId,
                EmpId = query.EmpId,
                EmployeePhoto = query.EmployeePhoto,
                Extenstion = query.Extenstion,
                FirstName = query.FirstName,
                Gender = query.Gender,
                IsActive = query.IsActive,
                IsAppLogin = query.IsAppLogin,
                IsEmployee = query.IsEmployee,
                IsEmployeesReporting = query.IsEmployeesReporting,
                IsOffRollDoctor = query.IsOffRollDoctor,
                IsOnRollDoctor = query.IsOnRollDoctor,
                LastName = query.LastName,
                LocationId = query.LocationId,
                LocationName = query.LocationName,
                PhoneNumber = query.PhoneNumber,
                PlaceAllocation = query.PlaceAllocation,
                SecurityAnswner = query.SecurityAnswner,
                SecurityQuestion = query.SecurityQuestion,
                SubDepartmentId = query.SubDepartmentId,
                SubDepartmentName = query.SubDepartmentName,
                UserId = query.UserId,
                UserType = query.UserType

            };
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMyTaskComments(int taskid)
        {
            IOrderedQueryable<tbl_TaskComment> list = myapp.tbl_TaskComment.Where(t => t.TaskId == taskid).OrderByDescending(t => t.TaskCommentId);
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DoneTask(int id)
        {
            string message = "Success";
            List<tbl_Task> list = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            string Designation = "";
            if (!User.IsInRole("OutSource"))
            {
                Designation = (from k in myapp.tbl_User where k.CustomUserId == User.Identity.Name select k).SingleOrDefault().Designation;
            }

            List<tbl_Task> listoftask = (from v in myapp.tbl_Task where v.IsActive == true && v.CallDateTime != null select v).ToList();
            if (list.Count > 0)
            {
                if (list[0].CreatedBy == User.Identity.Name)
                {

                    list[0].CreatorStatus = "Done";
                    list[0].ModifiedOn = DateAndTime.Now;
                    tbl_TaskComment tsc = new tbl_TaskComment
                    {
                        Comment = "Task Done by - " + list[0].AssignName,
                        TaskId = id,
                        CommentedBy = User.Identity.Name,
                        CommentDate = DateTime.Now
                    };
                    myapp.tbl_TaskComment.Add(tsc);
                    myapp.SaveChanges();
                }

                else if (listoftask.Count > 0)
                {

                    if (Designation != null && Designation == "Manager" && Designation != "")
                    {
                        tbl_Task singlelist = myapp.tbl_Task.Where(t => t.TaskId == id).SingleOrDefault();
                        DateTime calldatetime = singlelist.CallDateTime.Value;
                        double days = (DateTime.Now - calldatetime).TotalDays;
                        if (days >= 7)
                        {
                            list[0].CreatorStatus = "Done";
                            list[0].AssignStatus = "Done";
                            list[0].ModifiedOn = DateAndTime.Now;
                            tbl_TaskComment tsc = new tbl_TaskComment
                            {
                                Comment = "Task Done by - " + list[0].AssignName,
                                TaskId = id,
                                CommentedBy = User.Identity.Name,
                                CommentDate = DateTime.Now
                            };
                            myapp.tbl_TaskComment.Add(tsc);
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
        public ActionResult GetDiaryEvents(string start, string end)
        {
            List<tbl_Event> events = myapp.tbl_Event.ToList();
            if (Request.IsAuthenticated && User.Identity.Name != null)
            {
                events = events.Where(u => u.CreatedBy == User.Identity.Name).ToList();
            }
            //var eventslist = myapp.tbl_Event.Where(u => u.CreatedBy == User.Identity.Name).ToList();

            var eventList = from e in events
                            select new
                            {
                                id = e.EventId,
                                title = e.EventTitle,
                                start = e.EventDate + " " + e.EventTime,
                                end = e.EventDuration,
                                color = "",
                                allDay = false,
                                //resources = Data.Entitylist.Single(en => en.CHlId == e.chlid.ToString()).Department
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult GetHolidays()
        {

            string query = "Select * from tbl_Holiday ORDER BY HolidayDate";
            List<tbl_Holiday> list = myapp.tbl_Holiday.SqlQuery(query).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBookingEvents(string start, string end)
        {
            List<tbl_Booking> events = myapp.tbl_Booking.ToList();
            if (Request.IsAuthenticated && User.Identity.Name != null)
            {
                events = events.Where(u => u.CreatedBy == User.Identity.Name).ToList();
            }
            //var eventslist = myapp.tbl_Event.Where(u => u.CreatedBy == User.Identity.Name).ToList();

            var eventList = from e in events
                            select new
                            {
                                id = e.BookingId,
                                title = e.EventDescription,
                                //start = e.EventDate.Value.ToShortDateString() + " " + e.EventTime,
                                start = e.ToDate + "" + e.StartTime,
                                end = e.EndTime,
                                color = "",
                                allDay = false
                                //resources = Data.Entitylist.Single(en => en.CHlId == e.chlid.ToString()).Department
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteTask(int id)
        {
            string message = "Success";
            List<tbl_Task> list = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            if (list.Count > 0)
            {
                if (list[0].CreatedBy == User.Identity.Name)
                {
                    myapp.tbl_Task.Remove(list[0]);
                    myapp.SaveChanges();
                }
                else
                {
                    message = "Please as creator to delete the task";
                }
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }


        public ActionResult SaveMyTaskComment(int taskid, string comment)
        {
            tbl_TaskComment tsc = new tbl_TaskComment
            {
                Comment = comment,
                TaskId = taskid,
                CommentedBy = User.Identity.Name,
                CommentDate = DateTime.Now
            };
            myapp.tbl_TaskComment.Add(tsc);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveNewNotification(tbl_Notification tbl)
        {
            tbl.CreatedOn = DateTime.Now;
            tbl.CreatedBy = User.Identity.Name;
            tbl.NotificationFrom = User.Identity.Name;
            tbl.IsActive = true;
            myapp.tbl_Notification.Add(tbl);
            myapp.SaveChanges();

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxNotification(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                string user = User.Identity.Name;
                //var dept = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).Single();
                IQueryable<tbl_Notification> tasks = myapp.tbl_Notification.Where(t => t.IsActive == true && (t.NotificationFrom == user || t.NotificationTo == user));
                tasks = tasks.OrderByDescending(t => t.NotificationDate);
                //var dat = DateTime.Now.ToLocalTime();
                IEnumerable<tbl_Notification> filteredCompanies;
                //Check whether the companies should be filtered by keyword
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = tasks
                       .Where(c => c.NotificationId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.NotificationDate != null && c.NotificationDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.NotificationFrom != null && c.NotificationFrom.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.NotificationTo != null && c.NotificationTo.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.Notification != null && c.Notification.ToLower().Contains(param.sSearch.ToLower())

                                  );
                }
                else
                {
                    filteredCompanies = tasks;
                }
                IEnumerable<tbl_Notification> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies
                                                   // let restime = GetResposetime(c.CreateDate, c.Starttime)
                                                   //let date = dat
                                               select new[] { Convert.ToString(c.NotificationId),
                                 c.Notification,
                                 c.NotificationFrom,
                                 c.NotificationTo,
                                 c.NotificationDate.Value.ToShortDateString(),
                                 c.CreatedOn.Value.ToShortDateString(),
                                 Convert.ToString(c.NotificationId) };
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

        public ActionResult GetMyPendingtaskLatest()
        {
            if (Request.IsAuthenticated)
            {
                if (User.IsInRole("OutSource"))
                {
                    tbl_OutSourceUser dept = myapp.tbl_OutSourceUser.Where(u => u.CustomUserId == User.Identity.Name).Single();
                    List<tbl_Task> list = (from v in myapp.tbl_Task where v.IsActive != null select v).ToList();
                    list = list.Where(l => l.AssignId == dept.UserId && l.AssignStatus != "Done" && l.AssignStatus != "Done").ToList();
                    return Json(list, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    tbl_User dept = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).Single();
                    List<tbl_Task> list = (from v in myapp.tbl_Task where v.IsActive != null select v).ToList();
                    list = list.Where(l => l.AssignId == dept.UserId && l.AssignStatus != "Done" && l.AssignStatus != "Done").ToList();
                    return Json(list, JsonRequestBehavior.AllowGet);
                }


            }
            else { return RedirectToAction("Login", "Account"); }
        }
        public async Task<ActionResult> AjaxUserviewnew(JQueryDataTableParamModel param)
        {
            if (!Request.IsAuthenticated)
                return RedirectToAction("Login", "Account");

            string userId = User.Identity.Name;
            string role = GetUserRole(); // You can customize how to resolve the current role

            var (tasks, totalCount) = await _taskService.GetFilteredTasksAsync(param, userId, role);

            var result = tasks.Select(c => new[]
            {
            Convert.ToString(c.TaskId),
            c.CallDateTime?.ToString("dd/MM/yyyy HH:mm"),
            Responsetime(c.CallDateTime ?? DateTime.Now, c.CallStartDateTime ?? c.CallDateTime ?? DateTime.Now),
            c.CreatorName,
            c.AssignLocationName,
            c.ExtensionNo,
            $"{c.CategoryOfComplaint} {(c.DocumentReceived == true ? " - Yes -" : "")}",
            c.Subject,
            c.AssignName,
            CalculateAge(c.CallStartDateTime ?? DateTime.Now, c.CallEndDateTime ?? DateTime.Now),
            c.AssignStatus,
            c.CreatorStatus,
            Convert.ToString(c.TaskId)
        });

            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = totalCount,
                iTotalDisplayRecords = totalCount,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        private string GetUserRole()
        {
            // Simplified - you can expand this if multiple roles can be assigned
            if (User.IsInRole("Admin")) return "Admin";
            if (User.IsInRole("OutSource")) return "OutSource";
            if (User.IsInRole("LocationManager")) return "LocationManager";
            if (User.IsInRole("DepartmentManager")) return "DepartmentManager";
            return "User";
        }
        public ActionResult Getlistofemployeess(string dept)
        {

            string empid = User.Identity.Name;
            List<tbl_User> list = (from l in myapp.tbl_User where l.CustomUserId == empid select l).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyTasks_ExportToExcel(string FromDate, string ToDate, int locationid, int departmentid, string status, string emp, string category)
        {
            // Retrieve tasks with initial filter
            IQueryable<tbl_Task> tasksQuery = myapp.tbl_Task.Where(t => t.IsActive == true);

            if (User.IsInRole("OutSource"))
            {
                tbl_OutSourceUser dept = myapp.tbl_OutSourceUser.Single(u => u.CustomUserId == User.Identity.Name);

                if (User.IsInRole("Admin"))
                {
                    ViewBag.EntityType = "admin";
                    ViewBag.Department = dept.DepartmentName;
                }
                else if (User.IsInRole("LocationManager"))
                {
                    tasksQuery = tasksQuery.Where(t => t.CreatorLocationId == dept.LocationId
                        || t.CreatorLocationId == t.AssignLocationId
                        || t.AssignDepartmentId == dept.LocationId
                        || t.AssignDepartmentName == dept.DepartmentName.ToString());
                }
                else if (User.IsInRole("DepartmentManager") || new[] { "IT", "Information Technology", "Biomedical", "Maintenance", "Academics", "Purchase", "Finance & Accounts" }.Contains(dept.DepartmentName))
                {
                    switch (dept.DepartmentName)
                    {
                        case "Information Technology":
                        case "IT":
                            tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Information Technology" || t.AssignDepartmentName == "IT");
                            break;
                        case "Biomedical":
                            tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Biomedical");
                            break;
                        case "Maintenance":
                            tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Maintenance");
                            break;
                        case "Academics":
                            tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Academics");
                            break;
                        case "Purchase":
                            tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Purchase");
                            break;
                        case "Finance & Accounts":
                            tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Finance & Accounts");
                            break;
                        default:
                            tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentId == dept.DepartmentId);
                            break;
                    }
                }
                else
                {
                    tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignId == dept.UserId);
                }

                // Additional filters
                if (locationid != null && locationid != 0)
                {
                    tasksQuery = tasksQuery.Where(q => q.CreatorLocationId == locationid);
                }
                if (departmentid != null && departmentid != 0)
                {
                    tasksQuery = tasksQuery.Where(q => q.CreatorDepartmentId == departmentid);
                }
                if (Information.IsNumeric(emp) && Convert.ToInt32(emp) > 0)
                {
                    tasksQuery = tasksQuery.Where(q => q.AssignId == Convert.ToInt32(emp));
                }
                if (!string.IsNullOrEmpty(status))
                {
                    if (status == "Pending")
                    {
                        status = "In Progress";
                    }
                    tasksQuery = tasksQuery.Where(q => q.AssignStatus == status);
                }
            }
            else
            {
                tbl_User dept = myapp.tbl_User.Single(u => u.CustomUserId == User.Identity.Name);

                if (User.IsInRole("Admin"))
                {
                    ViewBag.EntityType = "admin";
                    ViewBag.Department = dept.DepartmentName;
                }
                else if (User.IsInRole("LocationManager"))
                {
                    tasksQuery = tasksQuery.Where(t => t.CreatorLocationId == dept.LocationId
                        || t.CreatorLocationId == t.AssignLocationId
                        || t.AssignDepartmentId == dept.LocationId
                        || t.AssignDepartmentName == dept.DepartmentName.ToString());
                }
                else if (User.IsInRole("DepartmentManager") || new[] { "IT", "Information Technology", "Biomedical", "Maintenance", "Academics", "Purchase" }.Contains(dept.DepartmentName))
                {
                    switch (dept.DepartmentName)
                    {
                        case "Information Technology":
                        case "IT":
                            tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "IT" || t.AssignDepartmentName == "Information Technology");
                            break;
                        case "Biomedical":
                            tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Biomedical");
                            break;
                        case "Maintenance":
                            tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Maintenance");
                            break;
                        case "Academics":
                            tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Academics");
                            break;
                        case "Purchase":
                            tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Purchase");
                            break;
                        default:
                            tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentId == dept.DepartmentId);
                            break;
                    }
                }
                else
                {
                    tasksQuery = tasksQuery.Where(t => t.CreatorId == dept.UserId || t.AssignId == dept.UserId);
                }

                // Additional filters
                if (locationid != null && locationid != 0)
                {
                    tasksQuery = tasksQuery.Where(q => q.CreatorLocationId == locationid);
                }
                if (departmentid != null && departmentid != 0)
                {
                    tasksQuery = tasksQuery.Where(q => q.CreatorDepartmentId == departmentid);
                }
                if (Information.IsNumeric(emp) && Convert.ToInt32(emp) > 0)
                {
                    tasksQuery = tasksQuery.Where(q => q.AssignId == Convert.ToInt32(emp));
                }
                if (!string.IsNullOrEmpty(status))
                {
                    if (status == "Pending")
                    {
                        status = "In Progress";
                    }
                    tasksQuery = tasksQuery.Where(q => q.AssignStatus == status);
                }
            }
            //IQueryable<tbl_TaskComment> queryallComments = (from c in myapp.tbl_TaskComment select c);
            // Common filters
            if (!string.IsNullOrEmpty(category))
            {
                tasksQuery = tasksQuery.Where(t => t.CategoryOfComplaint == category);
            }
            if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
            {
                DateTime dtfrmdate = ProjectConvert.ConverDateStringtoDatetime(FromDate);
                DateTime dttodate = ProjectConvert.ConverDateStringtoDatetime(ToDate).AddDays(1);

                tasksQuery = tasksQuery.Where(t => t.CallDateTime.Value >= dtfrmdate && t.CallDateTime.Value < dttodate);
                //queryallComments= queryallComments.Where(t => t.CommentDate!=null && ( t.CommentDate.Value >= dtfrmdate && t.CommentDate.Value < dttodate));
            }
            DateTime dt = DateTime.Now.AddYears(-1);
            //queryallComments = queryallComments.Where(t => t.CommentDate != null && t.CommentDate.Value >= dt);
            List<tbl_Task> tasks = tasksQuery.OrderByDescending(t => t.ModifiedOn).ToList();
            var taskIds = tasks.Select(t => t.TaskId).ToList();

            // Then use these TaskIds in the query to filter tbl_TaskComment
            List<tbl_TaskComment> allComments = myapp.tbl_TaskComment
                .Where(c => taskIds.Contains(c.TaskId.Value))
                .ToList();
            List<tbl_Location> locationslist = myapp.tbl_Location.ToList();
            System.Data.DataTable products = new System.Data.DataTable("MyTasksDataTable");
            // products.Rows.Add("Client  : " + list[0].ClientName + " SOA From : " + FromDate + " To : " + ToDate);
            products.Columns.Add("ID", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Issue Log in date", typeof(string));
            products.Columns.Add("Issue Log in time", typeof(string));

            products.Columns.Add("User", typeof(string));
            products.Columns.Add("Category", typeof(string));
            products.Columns.Add("Subject", typeof(string));
            //products.Columns.Add("Equip", typeof(string));
            products.Columns.Add("Description", typeof(string));
            products.Columns.Add("IT Engineer Response time(h:m:s)", typeof(string));
            products.Columns.Add("Pikcup IT support Engineer", typeof(string));
            products.Columns.Add("Assined Location", typeof(string));
            products.Columns.Add("Assined IT support Engineer(s)", typeof(string));
            products.Columns.Add("Status", typeof(string));
            products.Columns.Add("Extension", typeof(string));
            products.Columns.Add("Issue done Engineer", typeof(string));

            products.Columns.Add("Tota time take for issue complted(d:h:m)", typeof(string));
            products.Columns.Add("Work Done Remarks", typeof(string));
            products.Columns.Add("Assing vendor Comments", typeof(string));
            //products.Columns.Add("Creator Status", typeof(string));

            //products.Columns.Add("Issue Closed(d:h:m)", typeof(string));

            foreach (tbl_Task c in tasks)
            {

                string totlatimetaken = CalculateAge(c.CallStartDateTime.Value, c.CallEndDateTime.Value);

                //if (totlatimetaken != null && totlatimetaken != "")
                //{
                List<tbl_TaskComment> comments = allComments.Where(t => t.TaskId == c.TaskId).ToList();
                comments = comments.OrderBy(cm => cm.CommentDate).ToList();
                string Taskdoneby = "";
                long taskdonebyid = 0;
                long taskpickupbyid = 0;
                List<tbl_TaskComment> Taskdonebycomments = comments.Where(l => l.Comment.Contains("Task Done by ")).OrderByDescending(cm => cm.CommentDate).ToList();
                if (Taskdonebycomments.Count > 0)
                {
                    Taskdoneby = Taskdonebycomments[0].Comment.Replace("Task Done by ", "");
                    taskdonebyid = Taskdonebycomments[0].TaskCommentId;
                    Taskdoneby = Taskdoneby.Replace("-", "");
                }
                string firstpickupby = "";
                List<tbl_TaskComment> pickupbycomments = comments.Where(l => l.Comment.Contains("Task Pick Up By")).OrderByDescending(cm => cm.CommentDate).ToList();
                if (pickupbycomments.Count > 0)
                {
                    firstpickupby = pickupbycomments[0].Comment.Replace("Task Pick Up By", "");
                    taskpickupbyid = pickupbycomments[0].TaskCommentId;
                }
                string strcmmt = "";
                string assignto = "";

                foreach (tbl_TaskComment v in comments)
                {
                    if (v.TaskCommentId != taskdonebyid && v.TaskCommentId != taskpickupbyid)
                    {
                        assignto += v.Comment.Replace("Task Pick Up By ", "").Replace("Task Done by ", "").Replace("Task Assigned To ", "").Replace("Task Re Open", "").Replace("by", "");
                    }
                    if (!v.Comment.Contains("Task Assigned To") && !v.Comment.Contains("Task Pick Up By") && !v.Comment.Contains("Task Re Open") && !v.Comment.Contains("Task Done by"))
                    {
                        strcmmt += v.Comment + " on " + v.CommentDate.Value.ToString("dd/MM/yyyy") + " " + v.CommentDate.Value.ToShortTimeString() + "  --- *** ---  ";
                    }
                }

                if (string.IsNullOrEmpty(assignto))
                {
                    if (!string.IsNullOrEmpty(firstpickupby.Replace("-", "")))
                    {
                        assignto = firstpickupby.Replace("-", "");
                    }
                }
                if (string.IsNullOrEmpty(firstpickupby))
                {
                    firstpickupby = assignto;
                }

                products.Rows.Add(Convert.ToString(c.TaskId),
                    c.CreatorLocationName,
                                 c.CallDateTime.Value.ToString("dd/MM/yyyy"),
                                 c.CallDateTime.Value.ToShortTimeString(),
                                   c.CreatorDepartmentName + " " + c.CreatorName,
                                      c.CategoryOfComplaint,
                                      c.Subject,
                                   c.Description,
                               Responsetime(c.CallDateTime.Value, c.CallStartDateTime.Value),//ResponseTime                      

                              firstpickupby.Replace("-", ""),
                              c.AssignLocationName,
                                RemoveDuplicateWords(assignto), //Assined IT support Engineer(s)
                                                                //c.AssertEquipId + " " + c.AssertEquipName,
                                   c.AssignStatus, c.ExtensionNo,
                                  Taskdoneby,//Issue done Engineer
                                   totlatimetaken,//Total Time Taken   
                                 c.WorkDoneRemarks,


                           strcmmt//Vendor Comments

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
        public string RemoveDuplicateWords(string input)
        {
            // Split the string by spaces
            string[] words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Use a HashSet to keep track of unique words
            HashSet<string> uniqueWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // Create a list to store the result without duplicates
            List<string> resultWords = new List<string>();

            foreach (var word in words)
            {
                if (uniqueWords.Add(word)) // Only add if the word is not already present
                {
                    resultWords.Add(word);
                }
            }

            // Join the result back into a string
            return string.Join(" ", resultWords);
        }
        //if task is wrongly created by using this will assign to currect department

        //public JsonResult Assigntasktoemployee(int id, int locid, string locname, int deptid, string deptname, int empid, string empname)
        public JsonResult Assigntasktoemployee(int id, int locid, string locname, int deptid, string deptname)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            if (tasks.Count > 0)
            {
                tasks[0].AssignLocationId = locid;
                tasks[0].AssignLocationName = locname;
                tasks[0].AssignDepartmentId = deptid;
                tasks[0].AssignDepartmentName = deptname;
                tasks[0].Others = "NewAssign";
                tasks[0].ModifiedOn = DateAndTime.Now;
                myapp.SaveChanges();
                //if (deptname == "Academics")
                //{
                //    CustomModel cm = new CustomModel();
                //    MailModel mailmodel = new MailModel();
                //    string mailbody = "";
                //    mailmodel.fromemail = "Helpdesk@fernandez.foundation";
                //    mailmodel.subject = "A task(" + id + ") to you ";
                //    mailmodel.filepath = "";
                //    mailmodel.fromname = "Help Desk ";
                //    mailmodel.ccemail = "";
                //    mailmodel.toemail = "academics@fernandez.foundation";
                //    mailbody += "<h4 style='font-family:cambria'>Hello,</h4><br />";
                //    mailmodel.body = mailbody;
                //    cm.SendEmail(mailmodel);
                //}
            }
            return Json("The task is successfully assign to <b style='color:blue;font-family:italic;'>" + deptname + "</b>", JsonRequestBehavior.AllowGet);
        }
        //Assign the task to the employee
        public JsonResult Assigntasktoemployeeesbydepartment(int id, int empid, string empname)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            tbl_User assign = (from v in myapp.tbl_User where v.UserId == empid select v).SingleOrDefault();
            if (tasks.Count > 0)
            {
                tbl_User dept = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).Single();
                if (empname != null && empname != "")
                {
                    //tasks[0].CallStartDateTime = DateTime.Now;
                    tasks[0].AssignName = empname;
                    tasks[0].AssignId = empid;
                    tasks[0].Others = "NewAssign";
                    tbl_TaskComment tsc = new tbl_TaskComment
                    {
                        Comment = "Task Assigned To - " + empname + " by - " + dept.FirstName,
                        TaskId = id,
                        CommentedBy = User.Identity.Name,
                        CommentDate = DateTime.Now
                    };
                    myapp.tbl_TaskComment.Add(tsc);
                    myapp.SaveChanges();
                }
                if (assign.EmailId != null)
                {
                    CustomModel cm = new CustomModel();
                    MailModel mailmodel = new MailModel();
                    EmailTeamplates emailtemp = new EmailTeamplates();
                    mailmodel.fromemail = "Leave@hospitals.com";
                    mailmodel.toemail = assign.EmailId;
                    mailmodel.subject = "A Ticket " + tasks[0].TaskId + " Assigned to you ";
                    string mailbody = "<p style='font-family:verdana'>Dear " + assign.FirstName + ",";
                    string customuserid = User.Identity.Name;
                    tbl_User curuser = (from v in myapp.tbl_User where v.CustomUserId == customuserid select v).SingleOrDefault();
                    mailbody += "<p style='font-family:verdana'>" + curuser.FirstName + " has assigned a task to you. Please click on this <a target='_blank' href='https://infonet.fernandezhospital.com/'>link</a>  to see the task.Do not forget to update the task status after completion.</p>";
                    mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                    mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Call Date</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + tasks[0].CallDateTime + "</td></tr>";
                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Creator Department</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + tasks[0].CreatorDepartmentName + "</td></tr>";
                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Creator Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + tasks[0].CreatorName + "</td></tr>";
                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Category Of Complaint</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + tasks[0].CategoryOfComplaint + "</td></tr>";
                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Subject</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + tasks[0].Subject + "</td></tr>";
                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Description</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + tasks[0].Description + "</td></tr>";
                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Extension No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + tasks[0].AssertEquipId + "</td></tr>";
                    mailbody += "</table>";
                    mailbody += "<br/><p style='font-family:cambria'>This is an auto generated mail,Don't reply to this.</p>";
                    mailmodel.body = mailbody;
                    //mailmodel.body = "A New Ticket Assigned to you";
                    //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                    mailmodel.filepath = "";
                    //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                    mailmodel.fromname = "Help Desk";
                    mailmodel.ccemail = "";
                    cm.SendEmail(mailmodel);
                }

            }
            return Json("The task is successfully assigned to &nbsp;<b style='color:green;font-family:italic;'>" + assign.FirstName + "</b>", JsonRequestBehavior.AllowGet);
        }
        public ActionResult RejectTask(int id)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            string name = User.Identity.Name;
            tbl_User ulist = (from v in myapp.tbl_User where v.CustomUserId == name select v).SingleOrDefault();
            if (tasks.Count > 0)
            {
                //tasks[0].CallStartDateTime = DateTime.Now;
                tasks[0].AssignStatus = "Rejected";
                tasks[0].CreatorStatus = "Rejected";
                tasks[0].WorkDoneRemarks = "Task Rejected by" + ulist.FirstName + "";
                tasks[0].ModifiedOn = DateAndTime.Now;
                myapp.SaveChanges();
            }
            return Json("You have successfully reject the task", JsonRequestBehavior.AllowGet);
        }
        public JsonResult StartTask(int id)
        {
            string msg = "Success";
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            if (User.IsInRole("OutSource"))
            {
                List<tbl_OutSourceUser> user = myapp.tbl_OutSourceUser.Where(u => u.CustomUserId == User.Identity.Name).ToList();
                if (tasks.Count > 0 && user.Count > 0)
                {
                    //tasks[0].AssignLocationId = user[0].LocationId;
                    //tasks[0].AssignLocationName = user[0].LocationName;
                    tasks[0].AssignDepartmentId = user[0].DepartmentId;
                    tasks[0].AssignDepartmentName = user[0].DepartmentName;
                    if (tasks[0].AssignId == null || tasks[0].AssignId == 0 || tasks[0].Others == "NewAssign")
                    {
                        tasks[0].AssignId = user[0].UserId;
                        tasks[0].AssignName = user[0].FirstName;
                        tasks[0].CallStartDateTime = DateTime.Now;
                        tasks[0].ModifiedOn = DateAndTime.Now;
                        tasks[0].AssignStatus = "In Progress";
                        tasks[0].CreatorStatus = "In Progress";
                        tasks[0].Others = "";
                        //myapp.SaveChanges();

                        tbl_TaskComment tsc = new tbl_TaskComment
                        {
                            Comment = "Task Pick Up By - " + user[0].FirstName,
                            TaskId = id,
                            CommentedBy = User.Identity.Name,
                            CommentDate = DateTime.Now
                        };
                        myapp.tbl_TaskComment.Add(tsc);
                        myapp.SaveChanges();
                    }

                    else
                    {
                        msg = "The Ticked already picked by some one";
                    }
                }
            }
            else
            {
                List<tbl_User> user = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).ToList();
                if (tasks.Count > 0 && user.Count > 0)
                {
                    //tasks[0].AssignLocationId = user[0].LocationId;
                    //tasks[0].AssignLocationName = user[0].LocationName;
                    tasks[0].AssignDepartmentId = user[0].DepartmentId;
                    tasks[0].AssignDepartmentName = user[0].DepartmentName;
                    if (tasks[0].AssignStatus != "Done" && (tasks[0].AssignId == null || tasks[0].AssignId == 0 || tasks[0].Others == "NewAssign"))
                    {
                        tasks[0].AssignId = user[0].UserId;
                        tasks[0].AssignName = user[0].FirstName;
                        tasks[0].CallStartDateTime = DateTime.Now;
                        tasks[0].ModifiedOn = DateAndTime.Now;
                        tasks[0].AssignStatus = "In Progress";
                        tasks[0].CreatorStatus = "In Progress";
                        tasks[0].Others = "";
                        //myapp.SaveChanges();

                        tbl_TaskComment tsc = new tbl_TaskComment
                        {
                            Comment = "Task Pick Up By - " + user[0].FirstName,
                            TaskId = id,
                            CommentedBy = User.Identity.Name,
                            CommentDate = DateTime.Now
                        };
                        myapp.tbl_TaskComment.Add(tsc);
                        myapp.SaveChanges();
                    }

                    else
                    {
                        msg = "The Ticked already picked by some one";
                    }
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReOpenTask(int id)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            // var list = (from v in myapp.tbl_Task where v.AssignStatus != null && v.AssignStatus == "In Progress" select v).SingleOrDefault();

            if (tasks.Count > 0)
            {
                // tasks[0].CallStartDateTime = DateTime.Now;
                tasks[0].AssignStatus = "Re Open";
                tasks[0].CreatorStatus = "Re Open";
                tasks[0].ModifiedOn = DateAndTime.Now;
                myapp.SaveChanges();

                tbl_TaskComment tsc = new tbl_TaskComment
                {
                    Comment = "Task Re Open - " + tasks[0].AssignName,
                    TaskId = id,
                    CommentedBy = User.Identity.Name,
                    CommentDate = DateTime.Now
                };
                myapp.tbl_TaskComment.Add(tsc);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public string GetTimeDifference(DateTime st, DateTime ed)
        {
            TimeSpan t = ed.Subtract(st);
            //return ts.TotalMinutes.ToString("0.00");
            string answer = "";

            answer = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Days, t.Hours, t.Minutes);

            if (st == ed)
            {
                return "0";
            }

            return answer;
        }

        public string Responsetime(DateTime st, DateTime ed)
        {
            if (ed > st)
            {
                string startTime = "7:15 AM";
                string endTime = "8:45 PM";
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
                //return span.TotalMinutes.ToString("0.00");
                string answer = "";
                if (t.TotalMinutes > 1)
                {
                    answer = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Days, t.Hours, t.Minutes);
                }
                else
                {
                    answer = string.Format("{0:D2}:{1:D2}:{2:D2}", 0, 0, 7);
                }

                return answer;
            }
            else
            {

                return string.Format("{0:D2}:{1:D2}:{2:D2}", 0, 0, 9);
            }
        }
        public JsonResult EndTask(int id)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
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
                    tasks[0].ModifiedOn = DateAndTime.Now;
                }
                myapp.SaveChanges();
                tbl_TaskComment tsc = new tbl_TaskComment
                {
                    Comment = "Task Done by " + tasks[0].AssignName,
                    TaskId = id,
                    CommentedBy = User.Identity.Name,
                    CommentDate = DateTime.Now
                };
                myapp.tbl_TaskComment.Add(tsc);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult WorkDoneRemarks(int id, string remarks)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            if (tasks.Count > 0)
            {
                string currentuserid = User.Identity.Name;
                tbl_User list = (from v in myapp.tbl_User where v.CustomUserId == currentuserid select v).SingleOrDefault();
                if (list != null)
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
                        tasks[0].ModifiedOn = DateAndTime.Now;
                        myapp.SaveChanges();
                        tbl_TaskComment tsc = new tbl_TaskComment
                        {
                            Comment = "Task Done by " + tasks[0].AssignName,
                            TaskId = id,
                            CommentedBy = User.Identity.Name,
                            CommentDate = DateTime.Now
                        };
                        myapp.tbl_TaskComment.Add(tsc);
                        myapp.SaveChanges();
                    }
                    else
                    {

                        return Json("The Request already assign to Some other person", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult AssigntoVendor(int id, string remarks)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            if (tasks.Count > 0)
            {
                tasks[0].WorkDoneRemarks = remarks;
                tasks[0].CallEndDateTime = DateTime.Now;
                tasks[0].AssignStatus = "Pending from Vendor";
                tasks[0].CreatorStatus = "In Progress";
                tasks[0].ModifiedOn = DateAndTime.Now;
                tasks[0].IsVendorTicket = true;
                myapp.SaveChanges();
                tbl_TaskComment tsc = new tbl_TaskComment
                {
                    Comment = "Pending from Vendor " + remarks,
                    TaskId = id,
                    CommentedBy = User.Identity.Name,
                    CommentDate = DateTime.Now
                };
                myapp.tbl_TaskComment.Add(tsc);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public string AddNewTask(TaskSaveModel model, HttpPostedFileBase[] Upload, HttpPostedFileBase[] DailyCashcollction, HttpPostedFileBase[] loginuserwisecollections, HttpPostedFileBase[] NASReport, HttpPostedFileBase[] OthersDocument)
        {
            ManageHelpDesk helpDesk = new ManageHelpDesk();
            string path = Server.MapPath("~/ExcelUplodes/");
            string message = helpDesk.AddNewTask(model, User.Identity.Name, path, Upload, DailyCashcollction, loginuserwisecollections, NASReport, OthersDocument);
            return message;
        }


        public ActionResult NewTask()
        {
            return View();
        }
        public ActionResult EditTask(int id)
        {
            tbl_Task model = myapp.tbl_Task.Where(t => t.TaskId == id).Single();
            return View(model);
        }
        public JsonResult UpdateDescription(int id, string Description, string Category)
        {

            tbl_Task model = myapp.tbl_Task.Where(t => t.TaskId == id).Single();

            List<tbl_User> listuser = myapp.tbl_User.Where(t => t.CustomUserId == User.Identity.Name).ToList();
            if (model.CreatedBy == User.Identity.Name || model.AssignDepartmentName == listuser[0].DepartmentName)
            {
                if (Category != null && Category != "0" && Category != "")
                    model.CategoryOfComplaint = Category;
                model.Description = Description;
                model.ModifiedOn = DateAndTime.Now;
                myapp.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("you are not authorized to edit", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ViewTask(int id)
        {
            tbl_Task model = myapp.tbl_Task.Where(t => t.TaskId == id).Single();
            return View(model);
        }
        public JsonResult GetTask(int id)
        {


            tbl_Task model = myapp.tbl_Task.Where(t => t.TaskId == id).Single();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult Calender()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetTimeTracks()
        {
            string login = User.Identity.Name;
            List<SystemLog> list = (from s in myapp.SystemLogs where s.LoginId == User.Identity.Name select s).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RefreshTimeLog()
        {
            string msg = "refresh success";
            if (Session["InTime"] != null)
            {
                string logdurarion = Session["InTime"].ToString();
                DateTime t1 = Convert.ToDateTime(Convert.ToDateTime(logdurarion).ToShortTimeString());
                DateTime date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                DateTime t2 = Convert.ToDateTime(date.ToShortTimeString());
                TimeSpan ts = t2.Subtract(t1);
                logdurarion = ts.ToString();
                Session["LogDuration"] = logdurarion;
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult RequestForUserId()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult BookRoom()
        {
            return View();
        }
        public ActionResult ManageNotification()
        {
            return View();
        }
        public ActionResult ManageAllocation()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult ManageVisitor()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult FeedBack()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ExtensionNumbers()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ExtensionNumbers(string id)
        {

            try
            {
                List<tbl_locationlist> locList = myapp.tbl_locationlist.Where(t => t.Location == id).ToList();
                ViewBag.locList = locList;
                return Json(locList);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [AllowAnonymous]
        public ActionResult AjaxGetExtensionNumbersWithoutFloor(JQueryDataTableParamModel param)
        {
            List<tbl_locationlist> extentions = myapp.tbl_locationlist.ToList();
            List<tbl_Location> locationlist = myapp.tbl_Location.ToList();
            extentions = (from l in extentions
                          join loc in locationlist on l.Location equals loc.LocationId.ToString()
                          select new tbl_locationlist
                          {
                              Extension = l.Extension,
                              Floor = l.Floor,
                              Location = loc.LocationName,
                              Id = l.Id,
                              Name = l.Name
                          }).ToList();
            extentions = extentions.OrderBy(l => l.Name).ToList();
            IEnumerable<tbl_locationlist> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = extentions
                   .Where(c =>
                                c.Location != null && c.Location.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Floor != null && c.Floor.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Extension != null && c.Extension.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.Name != null && c.Name.ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = extentions;
            }
            IEnumerable<tbl_locationlist> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[] {
                                              c.Name,
                                              c.Extension,
                                              c.Location
                                              };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = extentions.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }
        [AllowAnonymous]
        public ActionResult AjaxGetExtensionNumbers(JQueryDataTableParamModel param)
        {
            List<tbl_locationlist> extentions = myapp.tbl_locationlist.ToList();
            List<tbl_Location> locationlist = myapp.tbl_Location.ToList();
            extentions = (from l in extentions
                          join loc in locationlist on l.Location equals loc.LocationId.ToString()
                          select new tbl_locationlist
                          {
                              Extension = l.Extension,
                              Floor = l.Floor,
                              Location = loc.LocationName,
                              Id = l.Id,
                              Name = l.Name
                          }).ToList();
            IEnumerable<tbl_locationlist> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = extentions
                   .Where(c =>
                                c.Location != null && c.Location.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Floor != null && c.Floor.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Extension != null && c.Extension.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.Name != null && c.Name.ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = extentions;
            }
            IEnumerable<tbl_locationlist> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[] {c.Location,
                                              c.Name,
                                              c.Extension,
                                              c.Floor
                                              };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = extentions.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }
        [AllowAnonymous]
        public ActionResult DoctorsList()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetSettings(string key)
        {
            List<tbl_Settings> list = myapp.tbl_Settings.Where(t => t.IsActive == true && t.SettingKey == key).ToList();
            if (list.Count > 0)
            {
                return Json(list[0].SettingValue, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("No Data", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult SendtestEmail(string key)
        {
            try
            {
                CustomModel cm = new CustomModel();
                MailModel model = new MailModel
                {
                    fromemail = "IT@gmail.com",
                    toemail = "srinivas@fernandez.foundation",

                    subject = "Intranet survey",
                    body = "<html>"
                };
                model.body += "<head>";
                model.body += "<title>Social Media Campaign - Mailer 2_2019 (1)</title>";
                model.body += "<meta http-equiv='Content-Type' content='text/html; charset=iso-8859-1'>";
                model.body += "</head>";
                model.body += "<body bgcolor='#FFFFFF' leftmargin='0' topmargin='0' marginwidth='0' marginheight='0'>";
                model.body += "<table id='Table_01' width='753' height='913' border='0' cellpadding='0' cellspacing='0'>";
                model.body += "<tr>";
                model.body += "<td colspan='10'>";

                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/index_01.jpg' width='753' height='317' alt=''></td>";
                model.body += "</tr>";
                model.body += "<tr>";
                model.body += "<td colspan='5'>";
                model.body += "<a href='https://www.facebook.com/fernandezstorkhome/photos/a.553185821519807/1189282911243425/?type=3&theater'>";
                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/Social-Media-Campaign-1.jpg' width='383' height='360' border='0' alt=''></a></td>";
                model.body += "<td colspan='4'>";
                model.body += "<a href='https://www.facebook.com/fernandezstorkhome/photos/a.553185821519807/1189749071196809/?type=3&theater'>";
                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/Social-Media-Campaign-2.jpg' width='324' height='360' border='0' alt=''></a></td>";
                model.body += "<td rowspan='4'>";

                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/index_04.jpg' width='46' height='596' alt=''></td>";
                model.body += "</tr>";
                model.body += "<tr>";
                model.body += "<td colspan='9'>";

                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/index_05.jpg' width='707' height='160' alt=''></td>";
                model.body += "</tr>";
                model.body += "<tr>";
                model.body += "<td rowspan='2'>";

                model.body += "		<img src='http://111.93.11.121:16/FH_Communication/images/index_06.jpg' width='271' height='76' alt=''></td>";
                model.body += "	<td>";
                model.body += "	<a href='https://www.facebook.com/fernandezstorkhome'>";
                model.body += "	<img src='http://111.93.11.121:16/FH_Communication/images/facebook.jpg' width='42' height='51' border='0' alt=''></a></td>";
                model.body += "<td rowspan='2'>";
                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/index_08.jpg' width='14' height='76' alt=''></td>";
                model.body += "<td>";
                model.body += "	<a href='https://twitter.com/FHStorkHome'>";
                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/twitter.jpg' width='42' height='51' border='0' alt=''></a></td>";
                model.body += "<td rowspan='2'>";
                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/index_10.jpg' width='14' height='76' alt=''></td>";
                model.body += "<td>";
                model.body += "<a href='https://www.instagram.com/stork.home/' target='_blank'>";
                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/INSTAGRAM.jpg' width='43' height='51' border='0' alt=''></a></td>";
                model.body += "<td rowspan='2'>";
                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/index_12.jpg' width='14' height='76' alt=''></td>";
                model.body += "<td>";
                model.body += "	<a href='https://www.linkedin.com/company/fernandez-hospital-pvt-ltd/'>";
                model.body += "	<img src='http://111.93.11.121:16/FH_Communication/images/Linkdin.gif' width='41' height='51' border='0' alt=''></a></td>";
                model.body += "<td rowspan='2'>";
                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/index_14.jpg' width='226' height='76' alt=''></td>";
                model.body += "</tr>";
                model.body += "<tr>";
                model.body += "<td>";
                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/index_15.jpg' width='42' height='25' alt=''></td>";
                model.body += "<td>";
                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/index_16.jpg' width='42' height='25' alt=''></td>";
                model.body += "<td>";
                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/index_17.jpg' width='43' height='25' alt=''></td>";
                model.body += "<td>";
                model.body += "<img src='http://111.93.11.121:16/FH_Communication/images/index_18.jpg' width='41' height='25' alt=''></td>";
                model.body += "</tr>";
                model.body += "</table>";

                model.body += "</body>";
                model.body += "</html>";
                //model.body += "<p><a href='http://111.93.11.121:18/Service/Survey?id=2'><img src='http://111.93.11.121:18/Images/IntranetSurvey%20Mailer_2019-min1.jpg' alt='click here' style='width:850px;' /></a>";
                //model.body+= "<p>Thanks</p>";
                model.filepath = "";
                model.username = "User";
                model.fromname = "Intranet survey";
                model.ccemail = "ahmadali@fernandez.foundation";
                cm.SendEmail(model);
                return Json("No Data", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [AllowAnonymous]
        public ActionResult SendMDPostThoughEmail(string emailids)
        {
            try
            {
                CustomModel cm = new CustomModel();
                MailModel model = new MailModel
                {
                    fromemail = "IT@gmail.com",
                    toemail = "vamsirm26@gmail.com",
                    subject = "From The Desk Of Dr. Evita Fernandez",
                    body = "<html><body>"
                };
                if (emailids != null && emailids != "")
                {

                }
                model.body += "<p><img style='border-radius: 82px; display: block; margin-left: auto; margin-right: auto;' src='http://111.93.11.121:18/images/director.png' alt='' width='157' height='157' /></p>";
                model.body += "<p style='text-align: center;'><strong><span style='color: #993300;'>Dr Evita Fernandez- Speaks</span></strong></p>";
                model.body += "<p style='text-align: left;'>&nbsp;</p>";
                tbl_Settings list = myapp.tbl_Settings.Where(t => t.IsActive == true && t.SettingKey == "MdMessage").SingleOrDefault();
                if (list != null)
                {
                    model.body += list.SettingValue;
                }
                model.body += "</body></html>";

                model.filepath = "";
                model.username = "User";
                model.fromname = "From The Desk Of Dr. Evita Fernandez";
                model.ccemail = emailids;
                cm.SendEmail(model);

                tbl_Share model2 = new tbl_Share
                {
                    PostId = list.SettingsId,
                    SharedOn = DateTime.Now,
                    SharedBy = User.Identity.Name,
                    IsActive = true
                };
                myapp.tbl_Share.Add(model2);
                myapp.SaveChanges();
                return Json("No Data", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [AllowAnonymous]
        public ActionResult SendTheVideoEmail(string key, string emailids)
        {
            try
            {
                CustomModel cm = new CustomModel();
                MailModel model = new MailModel
                {
                    fromemail = "ahmadali@fernandez.foundation",
                    toemail = "ahmadali@fernandez.foundation",
                    ccemail = emailids,
                    subject = "THE FERNANDEZ FOUNDATION PRAYER SERVICE 2020",
                    body = "<html><body>"
                };
                if (emailids != null && emailids != "")
                {

                }
                //model.body += "<p><img style='border-radius: 82px; display: block; margin-left: auto; margin-right: auto;' src='http://111.93.11.121:18/images/director.png' alt='' width='157' height='157' /></p>";
                model.body += "<p style='text-align: center;'><strong><span style='color: #993300;'> Video - THE FERNANDEZ FOUNDATION PRAYER SERVICE 2020</span></strong></p>";
                model.body += "<p style='text-align: left;'>&nbsp;</p>";
                tbl_Settings list = myapp.tbl_Settings.Where(t => t.SettingKey == key).SingleOrDefault();
                if (list == null)
                {
                    list = new tbl_Settings
                    {
                        IsActive = true,
                        SettingKey = key,
                        SettingValue = "0"
                    };
                    myapp.tbl_Settings.Add(list);
                    myapp.SaveChanges();
                }
                if (list != null)
                {
                    model.body += "<p style='text-align:center;'><a href='http://111.93.11.121:16/Home/MoreVideos'>Click here to View</a></p>";
                }
                model.body += "</body></html>";

                model.filepath = "";
                model.username = "User";
                model.fromname = "Fernandez Foundation - Intranet";
                model.ccemail = emailids;
                var resultcheck = cm.SendEmail(model);

                tbl_Share model2 = new tbl_Share
                {
                    PostId = list.SettingsId,
                    SharedOn = DateTime.Now,
                    SharedBy = User.Identity.Name,
                    IsActive = true
                };
                myapp.tbl_Share.Add(model2);
                myapp.SaveChanges();
                return Json(resultcheck, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetTodayBirthdays()
        {
            DateTime dt = DateTime.Now.Date;
            //List<tbl_User> listuser = new List<tbl_User>();

            List<tbl_User> listuser = myapp.tbl_User.Where(t => t.IsActive == true && t.DateOfBirth != null && t.DateOfBirth.Value.Month == dt.Month && t.DateOfBirth.Value.Day == dt.Day).ToList();
            //foreach (var v in list)
            //{
            //    if (v.DateOfBirth.Value.Month == dt.Month && v.DateOfBirth.Value.Day == dt.Day)
            //    {
            //        listuser.Add(v);
            //    }
            //}
            //list = list.Where(t=>t.DateOfBirth.Value.Month == dt.Month && t.DateOfBirth.Value.Day == dt.Day).ToList();
            if (listuser.Count > 0)
            {
                return Json(listuser, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("No Birth Days", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetTodayJoines()
        {
            DateTime Dt = DateTime.Now.AddDays(-8);
            List<tbl_User> list = myapp.tbl_User.Where(t => t.IsActive == true && t.DateOfJoining != null && t.DateOfJoining >= Dt).ToList();
            if (list.Count > 0)
            {
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("No Joines", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult AjaxGetVisitor(JQueryDataTableParamModel param)
        {

            if (Request.IsAuthenticated)
            {

                List<tbl_Visitor> query = myapp.tbl_Visitor.Where(a => a.IsActive == true).OrderByDescending(a => a.VisitorId).ToList();

                IEnumerable<tbl_Visitor> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.Mobile != null && c.Mobile.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.ComingFrom != null && c.ComingFrom.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.LocationName != null && c.LocationName.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filteredCompanies = query;
                }
                IEnumerable<tbl_Visitor> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies

                                               select new[] {
                                              c.Name,
                                              c.Mobile,
                                              c.ComingFrom,
                                              c.Purpose,
                                              c.ToMeet,
                                              c.LocationName,
                                              c.Comment,
                              c.VisitorId.ToString()};
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
        public JsonResult SaveVisitor(tbl_Visitor tbll)
        {
            tbll.CreatedBy = User.Identity.Name;
            tbll.CreatedOn = DateTime.Now;
            tbll.IsActive = true;
            myapp.tbl_Visitor.Add(tbll);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteVisitor(int id)
        {
            tbl_Visitor tb = myapp.tbl_Visitor.Where(a => a.VisitorId == id).SingleOrDefault();
            tb.IsActive = false;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult Page(int id)
        {
            ViewBag.PageId = id;
            List<tbl_Page> tb = myapp.tbl_Page.Where(a => a.PageId == id).ToList();
            if (tb.Count > 0)
            {
                ViewBag.PageName = tb[0].PageName;
                ViewBag.HtmlStr = tb[0].PageContent;
                return View();
            }
            else
            {
                ViewBag.PageName = "No Page Found";
                ViewBag.HtmlStr = "No Content Found";
                return View();
            }
        }


        public ActionResult AjaxGetAllocation(JQueryDataTableParamModel param)
        {

            if (Request.IsAuthenticated)
            {

                List<tbl_ManageAllocation> query = myapp.tbl_ManageAllocation.OrderByDescending(a => a.AllocationId).ToList();

                IEnumerable<tbl_ManageAllocation> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.AllocationPlace.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.ContactNo != null && c.ContactNo.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.EmployeeName != null && c.EmployeeName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filteredCompanies = query;
                }
                IEnumerable<tbl_ManageAllocation> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies

                                               select new[] {
                                c.AllocationPlace,
                                c.LocationName,
                                c.DepartmentName,
                                c.EmployeeName,
                                c.ContactNo,
                                c.AllocaitonFromDate.Value.ToShortDateString(),
                                c.AllocationTodate.Value.ToShortDateString(),
                              c.StartTime!=null? c.StartTime.Value.ToShortTimeString():"",
                               c.EndTime!=null? c.EndTime.Value.ToShortTimeString():"",
                              c. AllocationId.ToString(),
                             c.AllocationPlace,
                             c.Comment
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

        public async Task<JsonResult> GetManageAllocationListToAdmin()
        {
            List<tbl_ManageAllocation> query = await myapp.tbl_ManageAllocation.OrderByDescending(a => a.AllocationId).ToListAsync();
            List<ManageAllocation_RelationalClass> ReturnData = query.Select(e => new ManageAllocation_RelationalClass()
            {
                AllocationId = e.AllocationId,
                AllocaitonFromDate = Convert.ToDateTime(e.AllocaitonFromDate),
                AllocaitonFromDate_StringFormat = ((e.AllocaitonFromDate != null) ? Convert.ToDateTime(e.AllocaitonFromDate).ToString("DD/MM/yyyy") : ""),
                AllocationDetails = e.AllocationDetails,
                AllocationPlace = e.AllocationPlace,
                AllocationTodate = Convert.ToDateTime(e.AllocationTodate),
                AllocationTodate_StringFormat = ((e.AllocationTodate != null) ? Convert.ToDateTime(e.AllocationTodate).ToString("DD/MM/yyyy") : ""),
                Comment = e.Comment,
                ContactNo = e.ContactNo,
                CreatedBy = e.CreatedBy,
                CreatedOn = Convert.ToDateTime(e.CreatedOn),
                DepartmentId = Convert.ToInt32(e.DepartmentId),
                DepartmentName = e.DepartmentName,
                EmployeeId = e.EmployeeId,
                EmployeeName = e.EmployeeName,
                EndTime = Convert.ToDateTime(e.EndTime),
                EndTime_StringFormat = "",
                LocationId = Convert.ToInt32(e.LocationId),
                LocationName = e.LocationName,
                StartTime = Convert.ToDateTime(e.StartTime),
                StartTime_StringFormat = "",
            }).ToList();
            return Json(ReturnData, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SaveAllocation(tbl_ManageAllocation tbll)
        {
            tbll.CreatedBy = User.Identity.Name;
            tbll.CreatedOn = DateTime.Now;
            if (tbll.AllocationId != 0)
            {
                tbl_ManageAllocation tb = myapp.tbl_ManageAllocation.Where(a => a.AllocationId == tbll.AllocationId).SingleOrDefault();
                tb.AllocationPlace = tbll.AllocationPlace;

                tb.LocationId = tbll.LocationId;
                tb.LocationName = tbll.LocationName;
                tb.DepartmentId = tbll.DepartmentId;
                tb.DepartmentName = tbll.DepartmentName;
                tb.EmployeeId = tbll.EmployeeId;
                tb.EmployeeName = tbll.EmployeeName;
                tb.ContactNo = tbll.ContactNo;
                tb.AllocaitonFromDate = tbll.AllocaitonFromDate;
                tb.AllocationTodate = tbll.AllocationTodate;
                tb.StartTime = tbll.StartTime;
                tb.EndTime = tbll.EndTime;
                tb.AllocationDetails = tbll.AllocationDetails;
                tb.Comment = tbll.Comment;

            }
            else
            {
                myapp.tbl_ManageAllocation.Add(tbll);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAllocation(int id)
        {
            tbl_ManageAllocation tb = myapp.tbl_ManageAllocation.Where(a => a.AllocationId == id).SingleOrDefault();
            myapp.tbl_ManageAllocation.Remove(tb);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult MyProfile()
        {
            if (TempData["UpdateMsg"] != null)
            {
                ViewBag.UpdatedMsg = TempData["UpdateMsg"];
            }
            tbl_User user = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).Single();
            return View(user);
        }
        public ActionResult Tasks()
        {
            return View();
        }
        public ActionResult AjaxbyUserview(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                tbl_User dept = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).Single();

                List<tbl_Task> tasks = (from v in myapp.tbl_Task where v.IsActive == true && v.CreatedBy == dept.CustomUserId select v).ToList();
                //var tasks = myapp.tbl_Task.Where(t => t.IsActive == true).ToList();
                List<tbl_Task> list = (from v in myapp.tbl_Task where v.IsActive != null select v).ToList();


                tasks = tasks.OrderByDescending(t => t.TaskId).ToList();
                //var dat = DateTime.Now.ToLocalTime();
                IEnumerable<tbl_Task> filteredCompanies;
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
                IEnumerable<tbl_Task> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies
                                                   // let restime = GetResposetime(c.CreateDate, c.Starttime)
                                                   //let date = dat
                                               select new[] { Convert.ToString(c.TaskId),
                                 c.CallDateTime.Value.ToString(),
                                 GetTimeDifference(c.CallDateTime.Value,c.CallStartDateTime.Value),//ResponseTime                                
                                 c.CreatorDepartmentName +" "+c.CreatorName,
                                 c.CreatorLocationName,
                                 c.ExtensionNo /*+" "+c.AssertEquipName*/,
                                 c.AssertEquipId,
                                 c.AssertEquipName,
                                 c.Description,
                                 /*c.AssignDepartmentName+" "+*/
                                 c.AssignName,
                                 c.WorkDoneRemarks,
                                 GetTimeDifference(c.CallStartDateTime.Value,c.CallEndDateTime.Value),//Total Time Taken                               
                                 c.AssignStatus,
                                 c.CreatorStatus,
                                 Convert.ToString(c.TaskId) };
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
        public JsonResult GetuserFname()
        {
            string username = User.Identity.Name;
            if (User.IsInRole("OutSource"))
            {

                tbl_OutSourceUser list = (from l in myapp.tbl_OutSourceUser where l.CustomUserId == username select l).SingleOrDefault();
                if (list != null)
                {
                    return Json(list.FirstName, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Test Admin", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {


                tbl_User list = (from l in myapp.tbl_User where l.CustomUserId == username select l).SingleOrDefault();
                if (list != null)
                {
                    return Json(list.FirstName, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Test Admin", JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult PolicyProtect()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult CUGNumbers()
        {
            return View(myapp.tbl_CugList.ToList());
        }
        public ActionResult ExportExcel_CugNumbers()
        {
            List<tbl_CugList> list = myapp.tbl_CugList.ToList();
            System.Data.DataTable products = new System.Data.DataTable("EmployeesLogin");
            products.Columns.Add("S.No", typeof(string));
            products.Columns.Add("Name", typeof(string));
            products.Columns.Add("Extension", typeof(string));
            int i = 1;
            foreach (tbl_CugList c in list)
            {
                products.Rows.Add(i,
                                 c.Name,
                                 c.Extension
                );
                i = i + 1;
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
            string filename = "CugNumbers.xls";
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
        public JsonResult ViewDescription(int id)
        {
            tbl_Task model = myapp.tbl_Task.Where(t => t.TaskId == id).SingleOrDefault();
            return Json(model.Description, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDocumentTaskStatus(int id, bool status)
        {
            tbl_Task model = myapp.tbl_Task.Where(t => t.TaskId == id).SingleOrDefault();
            model.DocumentReceived = status;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult WorkDonwstatus(int id)
        {
            tbl_Task model = myapp.tbl_Task.Where(t => t.TaskId == id).SingleOrDefault();
            return Json(model.WorkDoneRemarks, JsonRequestBehavior.AllowGet);
        }


        public JsonResult WorkDoneRemarksComment(int id, string Remarks)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            if (tasks.Count > 0)
            {
                tasks[0].WorkDoneRemarks = Remarks;
                tasks[0].ModifiedOn = DateAndTime.Now;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
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
                            tbl_TaskDoument tsk = new tbl_TaskDoument
                            {
                                CreatedBy = User.Identity.Name,
                                CreatedOn = DateTime.Now,
                                DocumentName = fileName,
                                DocumentPath = guidid + fileName,
                                IsPrivate = checkpublic == "true" ? true : false,
                                TaskId = int.Parse(Id)
                            };
                            myapp.tbl_TaskDoument.Add(tsk);
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
            List<tbl_TaskDoument> list = myapp.tbl_TaskDoument.Where(l => l.TaskId == id).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveTaskAutoassignSettings(tbl_TaskAutoAssignSetting model)
        {
            List<tbl_TaskAutoAssignSetting> list = myapp.tbl_TaskAutoAssignSetting.Where(l => l.LocationId == model.LocationId && l.Departmentid == model.Departmentid
            && l.CreatorDepartmentId == model.CreatorDepartmentId && l.CreatorLocationId == model.CreatorLocationId
            && l.AssignToUserId == model.AssignToUserId && l.Subject == model.Subject).ToList();
            if (list.Count == 0)
            {
                model.IsActive = true;
                myapp.tbl_TaskAutoAssignSetting.Add(model);
                myapp.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("The Subject for this user is allready exists", JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteTaskAuto(int id)
        {
            List<tbl_TaskAutoAssignSetting> modellist = myapp.tbl_TaskAutoAssignSetting.Where(l => l.Id == id).ToList();
            if (modellist.Count > 0)
            {
                myapp.tbl_TaskAutoAssignSetting.Remove(modellist[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxbyTaskAutoassignSettings(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                List<TaskAutoAssignViewModel> tasks = (from m in myapp.tbl_TaskAutoAssignSetting
                                                       join l in myapp.tbl_Location on m.LocationId equals l.LocationId
                                                       join d in myapp.tbl_Department on m.Departmentid equals d.DepartmentId
                                                       join lc in myapp.tbl_Location on m.CreatorLocationId equals lc.LocationId
                                                       join dc in myapp.tbl_Department on m.CreatorDepartmentId equals dc.DepartmentId
                                                       join u in myapp.tbl_User on m.AssignToUserId equals u.CustomUserId
                                                       select new TaskAutoAssignViewModel
                                                       {
                                                           AssignToUserid = m.AssignToUserId,
                                                           AssignToUserName = u.FirstName,
                                                           DepartmentId = m.Departmentid.Value,
                                                           DepartmentName = d.DepartmentName,
                                                           Id = m.Id,
                                                           LocationId = m.LocationId.Value,
                                                           LocationName = l.LocationName,
                                                           NotifyEmail = m.SendEmail.Value,
                                                           NotifySMS = m.SendSMS.Value,
                                                           Subject = m.Subject,
                                                           CreatorDepartmentId = m.CreatorDepartmentId.Value,
                                                           CreatorLocationId = m.CreatorLocationId.Value,
                                                           CreatorDepartmentName = dc.DepartmentName,
                                                           CreatorLocationName = lc.LocationName
                                                       }).ToList();
                tasks = tasks.OrderByDescending(t => t.Id).ToList();
                IEnumerable<TaskAutoAssignViewModel> filteredCompanies;
                //Check whether the companies should be filtered by keyword
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = tasks
                       .Where(c => c.Id.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.AssignToUserName != null && c.AssignToUserName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.Subject != null && c.Subject.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.LocationName != null && c.LocationName.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())

                                  );
                }
                else
                {
                    filteredCompanies = tasks;
                }
                IEnumerable<TaskAutoAssignViewModel> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies

                                               select new[] { Convert.ToString(c.Id),
                                 c.CreatorLocationName,
                                 c.CreatorDepartmentName,
                                  c.LocationName,
                                 c.DepartmentName,
                                 c.AssignToUserName,
                                 c.Subject,
                                 c.NotifySMS.ToString(),
                                 c.NotifyEmail.ToString(),
                                 Convert.ToString(c.Id) };
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
        #region DashBoard
        public ActionResult DashBoard()
        {
            ViewBag.New = myapp.tbl_Task.Where(l => l.AssignStatus == "New").Count();
            ViewBag.inprogess = myapp.tbl_Task.Where(l => l.AssignStatus == "In Progress").Count();
            ViewBag.Reopen = myapp.tbl_Task.Where(l => l.AssignStatus == "Re Open").Count();
            ViewBag.Completed = myapp.tbl_Task.Where(l => l.AssignStatus == "Done").Count();
            return View();
        }


        public ActionResult GetCountOfTaskBasedonLocation()
        {

            var tasksbyLocation = (from o in myapp.tbl_Task
                                   group o by o.CreatorLocationName into g
                                   orderby g.Count() descending
                                   select new
                                   {
                                       Location = g.Key,
                                       CountDetails = g.Count()
                                   }).ToList();

            return Json(new { result = tasksbyLocation }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCountOfTaskBasedonDepartment()
        {

            var tasksbyLocation = (from o in myapp.tbl_Task
                                   group o by o.CreatorDepartmentName into g
                                   orderby g.Count() descending
                                   select new
                                   {
                                       Department = g.Key,
                                       CountDetails = g.Count()
                                   }).ToList();

            return Json(new { result = tasksbyLocation }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCountOfVehicleBooking()
        {

            var VehicleBooking = (from o in myapp.tbl_VehicleRequest
                                  group o by o.NoofPersons into g
                                  orderby g.Count() descending
                                  select new
                                  {
                                      NoofPersons = g.Key,
                                      CountDetails = g.Count()
                                  }).ToList();

            return Json(new { result = VehicleBooking }, JsonRequestBehavior.AllowGet);
        }
        #endregion
        [AllowAnonymous]
        public ActionResult MdMessageMore()
        {
            return View();
        }

        public JsonResult SaveLikes(string key)
        {
            tbl_Settings list = myapp.tbl_Settings.Where(t => t.IsActive == true && t.SettingKey == key).SingleOrDefault();
            if (list == null)
            {
                list = new tbl_Settings
                {
                    IsActive = true,
                    SettingKey = key,
                    SettingValue = "0"
                };
                myapp.tbl_Settings.Add(list);
                myapp.SaveChanges();
            }
            string username = User.Identity.Name;
            //check likes
            var likes = myapp.tbl_Likes.Where(l => l.PostId == list.SettingsId && l.LikedBy == username).Count();
            if (likes == 0)
            {
                tbl_Likes model = new tbl_Likes
                {
                    PostId = list.SettingsId,
                    LikedOn = DateTime.Now,
                    LikedBy = username,
                    IsActive = true,
                    CommentId = 0,
                    SubCommentId = 0
                };
                myapp.tbl_Likes.Add(model);
                myapp.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("You already liked this Post.", JsonRequestBehavior.AllowGet);
            }

        }
        [AllowAnonymous]
        public JsonResult SaveComment(string key, string Message, string commentedby = "")
        {
            tbl_Settings list = myapp.tbl_Settings.Where(t => t.IsActive == true && t.SettingKey == key).FirstOrDefault();
            if (list == null)
            {
                list = new tbl_Settings
                {
                    IsActive = true,
                    SettingKey = key,
                    SettingValue = "0"
                };
                myapp.tbl_Settings.Add(list);
                myapp.SaveChanges();
            }
            tbl_Comment model = new tbl_Comment
            {
                PostId = list.SettingsId,
                CommentedBy = (commentedby != null && commentedby != "") ? commentedby : User.Identity.Name,
                CommentedOn = DateTime.Now,
                IsActive = true,
                Message = Message,
                TotalLikes = 0
            };
            myapp.tbl_Comment.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult GetTotalLikes(string key)
        {
            tbl_Settings list = myapp.tbl_Settings.Where(t => t.IsActive == true && t.SettingKey == key).FirstOrDefault();
            if (list != null)
            {
                int postid = list.SettingsId;
                int Count = myapp.tbl_Likes.Where(l => l.PostId == postid).Count();
                return Json(Count, JsonRequestBehavior.AllowGet);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult GetTotalShare(string key)
        {
            tbl_Settings list = myapp.tbl_Settings.Where(t => t.IsActive == true && t.SettingKey == key).FirstOrDefault();
            if (list != null)
            {
                int postid = list.SettingsId;
                int Count = myapp.tbl_Share.Where(l => l.PostId == postid).Count();
                return Json(Count, JsonRequestBehavior.AllowGet);
            }
            return Json("0", JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult GetCommentsbyPostId(string key)
        {
            tbl_Settings list = myapp.tbl_Settings.Where(t => t.IsActive == true && t.SettingKey == key).FirstOrDefault();
            if (list != null)
            {
                int modelid = list.SettingsId;
                List<tbl_Comment> model = myapp.tbl_Comment.Where(l => l.PostId == modelid).OrderByDescending(l => l.CommentId).ToList();
                //List<tbl_User> listofuser = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
                var updateModel = (from m in model
                                   let u = myapp.tbl_User.Where(l => l.CustomUserId == m.CommentedBy).SingleOrDefault()

                                   select new
                                   {
                                       CommentId = m.CommentId,
                                       CommentedBy = u != null ? (u.FirstName + " " + u.LastName) : m.CommentedBy,
                                       CommentedOn = m.CommentedOn.Value.ToString("dd-MMM hh:mm tt"),
                                       Message = m.Message,
                                       TotalLikes = m.TotalLikes,
                                       SubComment = myapp.tbl_SubComment.Where(l => l.CommentId == m.CommentId && l.PostId == m.PostId).ToList()
                                   }).ToList();
                return Json(updateModel, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    CommentId = "1",
                    CommentedBy = "0",
                    CommentedOn = "",
                    Message = "",
                    TotalLikes = "0",
                    SubComment = ""
                }, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult DeleteCommentId(int id)
        {
            tbl_Comment comment = myapp.tbl_Comment.Where(l => l.CommentId == id).SingleOrDefault();
            myapp.tbl_Comment.Remove(comment);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveEditMessage(int id, string Message)
        {
            tbl_Comment comment = myapp.tbl_Comment.Where(l => l.CommentId == id).SingleOrDefault();
            comment.Message = Message;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveMDReplayOnMessage(int id, string Message)
        {
            tbl_Comment comment = myapp.tbl_Comment.Where(l => l.CommentId == id).SingleOrDefault();
            //comment.Message = Message;
            //myapp.SaveChanges();
            string username = User.Identity.Name;
            tbl_User listofuser = myapp.tbl_User.Where(l => l.CustomUserId == username).SingleOrDefault();
            tbl_SubComment subcomment = new tbl_SubComment
            {
                CommentedBy = listofuser.FirstName + " " + listofuser.LastName,
                CommentedOn = DateAndTime.Now,
                CommentId = comment.CommentId,
                IsActive = true,
                Message = Message,
                PostId = comment.PostId,
                TotalLikes = 0

            };
            myapp.tbl_SubComment.Add(subcomment);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult MoreVideos()
        {
            List<tbl_HomePageVideos> model = myapp.tbl_HomePageVideos.Where(l => l.IsActive == true).OrderByDescending(l => l.HomePageVideosId).ToList();
            model = model.Skip(1).ToList();
            return View(model);
        }
        public ActionResult ManageHomePageVideos()
        {
            return View();
        }
        public ActionResult UpdateDocumentStatusTicket(int ticketid, bool status)
        {
            var ticket = myapp.tbl_Task.Where(l => l.TaskId == ticketid).SingleOrDefault();
            if (ticket != null)
            {
                ticket.DocumentReceived = status;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendemailtoNewEmployee(string EmployeeName, string Mobile, string Email, string dateofjoining, string Notes, string empid)
        {
            string status = "Success";
            try
            {

                CustomModel cm = new CustomModel();
                MailModel model = new MailModel
                {
                    fromemail = "IT@fernandez.foundation",
                    toemail = "ahmadali@fernandez.foundation",
                    subject = "Buddy Program " + EmployeeName,
                    body = "<html><body>"
                };
                model.body += "<p>Dear " + EmployeeName + " welcome to Fernandez Foundation!";
                model.body += "<p>New employee details are below : </p>";
                model.body += "<p><table style='width:60%;margin:auto;border:solid 1px #eee;'>";
                model.body += "<tr><td style='border:solid 1px #eee;font-size:14px;'>Employee Name</td><td  style='font-family:verdana;border:solid 1px #eee;font-size:14px;'>" + EmployeeName + "</td></tr>";
                model.body += "<tr><td style='border:solid 1px #eee;font-size:14px;'>Mobile</td><td  style='font-family:verdana;border:solid 1px #eee;font-size:14px;'>" + Email + "</td></tr>";
                model.body += "<tr><td style='border:solid 1px #eee;font-size:14px;'>Email</td><td  style='font-family:verdana;border:solid 1px #eee;font-size:14px;'>" + Mobile + "</td></tr>";
                model.body += "<tr><td style='border:solid 1px #eee;font-size:14px;'>Notes</td><td  style='font-family:verdana;border:solid 1px #eee;font-size:14px;'>" + Notes + "</td></tr>";
                model.body += "</table>";
                int intempid = int.Parse(empid);
                var user = myapp.tbl_User.Where(l => l.UserId == intempid).SingleOrDefault();

                string mobilenumber = "";
                if (user.PhoneNumber != null && user.PhoneNumber != "")
                {
                    mobilenumber = user.PhoneNumber;
                }
                if (user.CugNumber != null && user.CugNumber != "")
                {
                    if (mobilenumber == "")
                    {
                        mobilenumber = user.CugNumber;
                    }
                    else
                    {
                        mobilenumber = mobilenumber + "," + user.CugNumber;
                    }
                }

                string smstonewemploayee = "Dear " + EmployeeName + " welcome to Fernandez Foundation. " + user.FirstName + " will be your HR buddy from the department of " + user.DepartmentName + ". You can reach your buddy on " + mobilenumber + " ";
                smstonewemploayee += " Thanks and Regards";
                smstonewemploayee += " HR @ Fernandez Foundation";

                string Smstohrbuddy = "Dear " + user.FirstName + ", " + EmployeeName + " has joined in your department on " + dateofjoining + ". You can connect with the new joinee on " + Mobile + ". ";
                Smstohrbuddy += "Your support and help will ensure their initial days are more productive. ";
                Smstohrbuddy += " Thanks and Regards ";
                Smstohrbuddy += " HR @ Fernandez Foundation";

                SendSms sms = new SendSms();
                sms.SendSmsToEmployee(Mobile, smstonewemploayee);
                sms.SendSmsToEmployee(mobilenumber, Smstohrbuddy);

                model.body += "<p>" + user.FirstName + " will be your HR buddy from the department of " + user.DepartmentName + ".</p>";
                model.body += "<p><table style='width:60%;margin:auto;border:solid 1px #eee;'>";
                model.body += "<tr><td style='border:solid 1px #eee;font-size:14px;'>Employee Name</td><td  style='font-family:verdana;border:solid 1px #eee;font-size:14px;'>" + user.FirstName + "</td></tr>";
                model.body += "<tr><td style='border:solid 1px #eee;font-size:14px;'>Mobile</td><td  style='font-family:verdana;border:solid 1px #eee;font-size:14px;'>" + user.EmailId + "</td></tr>";
                model.body += "<tr><td style='border:solid 1px #eee;font-size:14px;'>Email</td><td  style='font-family:verdana;border:solid 1px #eee;font-size:14px;'>" + user.PhoneNumber + "</td></tr>";
                model.body += "<tr><td style='border:solid 1px #eee;font-size:14px;'>Department</td><td  style='font-family:verdana;border:solid 1px #eee;font-size:14px;'>" + user.DepartmentName + "</td></tr>";
                model.body += "<tr><td style='border:solid 1px #eee;font-size:14px;'>Location</td><td  style='font-family:verdana;border:solid 1px #eee;font-size:14px;'>" + user.LocationName + "</td></tr>";
                model.body += "<tr><td style='border:solid 1px #eee;font-size:14px;'>Cug Number</td><td  style='font-family:verdana;border:solid 1px #eee;font-size:14px;'>" + user.CugNumber + "</td></tr>";
                model.body += "<tr><td style='border:solid 1px #eee;font-size:14px;'>Extenstion</td><td  style='font-family:verdana;border:solid 1px #eee;font-size:14px;'>" + user.Extenstion + "</td></tr>";
                model.body += "</table>";
                model.toemail = user.EmailId;
                model.body += "</body></html>";
                model.filepath = "";
                model.username = "User";
                model.fromname = "Buddy Program";
                model.ccemail = Email;
                if (model.toemail != null && model.toemail != "" && model.ccemail != null && model.ccemail != "")
                {
                    status = cm.SendEmail(model);
                }
                else
                {
                    status = " Invalid Email address please validate ";
                }
            }
            catch (Exception exc)
            {
                status = exc.Message;
            }

            return Json(status, JsonRequestBehavior.AllowGet);

        }
        public ActionResult BuddyProgram()
        {
            return View();
        }
        [AllowAnonymous]
        public JsonResult TestSMS()
        {
            try
            {
                SendSms sms = new SendSms();
                if (sms != null)
                {
                    var msgres = sms.SendSmsToEmployee("9642875432", "Dear Rahul Dutta, you have a Capex request pending for approval, please login to infonet to approve the capex request.");

                    return Json("1" + msgres, JsonRequestBehavior.AllowGet);
                }
                return Json("Object is null", JsonRequestBehavior.AllowGet);
            }
            catch (Exception exc)
            {
                return Json("2" + exc.Message, JsonRequestBehavior.AllowGet);
            }

        }
    }
}