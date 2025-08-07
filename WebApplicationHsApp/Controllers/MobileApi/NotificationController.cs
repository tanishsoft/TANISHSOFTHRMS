using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models.MobileApi;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    [RoutePrefix("api/Notification")]
    public class NotificationController : ApiController
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        [HttpGet]
        [Route("GetCurrentUserNotificationRoles")]
        public List<string> GetCurrentUserNotificationRoles()
        {
            List<string> roles = new List<string>();
            if (User.IsInRole("Admin") || User.Identity.Name.ToLower() == "1")
            {
                roles.Add("All");
                roles.Add("Unit 1");
                roles.Add("Unit 2");
                roles.Add("Unit 3");
                roles.Add("Unit 4");
                roles.Add("Unit 5");
                roles.Add("All Employees");
                roles.Add("All Doctors");
                roles.Add("All My Reportings");
            }
            else if (User.IsInRole("LocationManager"))
            {
                roles.Add("All My Location Employees");
                roles.Add("All My Department Employees");
                roles.Add("All My Reportings");
            }
            else if (User.IsInRole("DepartmentManager"))
            {
                roles.Add("All My Department Employees");
                roles.Add("All My Reportings");
            }
            else if (User.IsInRole("SubDepartmentManager"))
            {
                roles.Add("All My Department Employees");
                roles.Add("All My Reportings");
            }
            else if (User.IsInRole("Employee"))
            {
                roles.Add("All My Department Employees");
                roles.Add("All My Reportings");
            }
            return roles;
        }
        [HttpPost]
        [Route("SendNotificationTo")]
        public string SendNotificationTo(NotificationViewModel model)
        {
            NotificationSendModel notify = new NotificationSendModel();
            var currentuser = myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).ToList();
            string response = "Please re login and try";
            var deviceidlist = myapp.tbl_DeviceInfo.ToList();
            if (currentuser.Count > 0)
            {
                switch (model.ToRole)
                {
                    case "All":
                        response = notify.SendNotificationToAll(model.Message, currentuser[0].FirstName + " " + currentuser[0].LastName);
                        break;
                    case "Unit 1":
                        var unit1emps = myapp.tbl_User.Where(l => l.LocationId == 1 && l.IsActive == true).ToList();
                        var devideidstosend = (from u in unit1emps
                                               from d in deviceidlist
                                               where u.CustomUserId == d.UserId
                                               select new NotificationUserModel
                                               {
                                                   DeviceId = d.DeviceId,
                                                   Userid = d.UserId
                                               }).Distinct().ToList();
                        response = notify.SendNotificationToSome("NotificationsPage",model.Message, User.Identity.Name, currentuser[0].FirstName + " " + currentuser[0].LastName, devideidstosend);

                        break;
                    case "Unit 2":
                        var unit2emps = myapp.tbl_User.Where(l => l.LocationId == 2 && l.IsActive == true).ToList();
                        var devideidstosend2 = (from u in unit2emps
                                                from d in deviceidlist
                                                where u.CustomUserId == d.UserId
                                                select new NotificationUserModel
                                                {
                                                    DeviceId = d.DeviceId,
                                                    Userid = d.UserId
                                                }).Distinct().ToList();
                        response = notify.SendNotificationToSome("NotificationsPage", model.Message, User.Identity.Name, currentuser[0].FirstName + " " + currentuser[0].LastName, devideidstosend2);

                        break;
                    case "Unit 3":
                        var unit3emps = myapp.tbl_User.Where(l => l.LocationId == 3 && l.IsActive == true).ToList();
                        var devideidstosend3 = (from u in unit3emps
                                                from d in deviceidlist
                                                where u.CustomUserId == d.UserId
                                                select new NotificationUserModel
                                                {
                                                    DeviceId = d.DeviceId,
                                                    Userid = d.UserId
                                                }).Distinct().ToList();
                        response = notify.SendNotificationToSome("NotificationsPage", model.Message, User.Identity.Name, currentuser[0].FirstName + " " + currentuser[0].LastName, devideidstosend3);
                        break;
                    case "Unit 4":
                        var unit4emps = myapp.tbl_User.Where(l => l.LocationId == 4 && l.IsActive == true).ToList();
                        var devideidstosend4 = (from u in unit4emps
                                                from d in deviceidlist
                                                where u.CustomUserId == d.UserId
                                                select new NotificationUserModel
                                                {
                                                    DeviceId = d.DeviceId,
                                                    Userid = d.UserId
                                                }).Distinct().ToList();
                        response = notify.SendNotificationToSome("NotificationsPage", model.Message, User.Identity.Name, currentuser[0].FirstName + " " + currentuser[0].LastName, devideidstosend4);
                        break;
                    case "Unit 5":
                        var unit5emps = myapp.tbl_User.Where(l => l.LocationId == 5 && l.IsActive == true).ToList();
                        var devideidstosend5 = (from u in unit5emps
                                                from d in deviceidlist
                                                where u.CustomUserId == d.UserId
                                                select new NotificationUserModel
                                                {
                                                    DeviceId = d.DeviceId,
                                                    Userid = d.UserId
                                                }).Distinct().ToList();
                        response = notify.SendNotificationToSome("NotificationsPage", model.Message, User.Identity.Name, currentuser[0].FirstName + " " + currentuser[0].LastName, devideidstosend5);
                        break;
                    case "All Employees":
                        string querytoexecute = "select  dev.UserId,dev.DeviceId from [dbo].[AspNetUserRoles]  apusrole  inner join AspNetUsers apus on apus.Id = apusrole.UserId inner join tbl_User tusr on tusr.CustomUserId = apus.UserName inner join tbl_DeviceInfo dev on dev.UserId = apus.UserName where apusrole.RoleId = 2 and tusr.IsActive = 1";
                        var listofemployees = myapp.Database.SqlQuery<NotificationUserModel>(querytoexecute).ToList();

                        response = notify.SendNotificationToSome("NotificationsPage", model.Message, User.Identity.Name, currentuser[0].FirstName + " " + currentuser[0].LastName, listofemployees);
                        break;
                    case "All Doctors":
                        string querytoexecute1 = "select  dev.UserId,dev.DeviceId from [dbo].[AspNetUserRoles]  apusrole  inner join AspNetUsers apus on apus.Id = apusrole.UserId inner join tbl_User tusr on tusr.CustomUserId = apus.UserName inner join tbl_DeviceInfo dev on dev.UserId = apus.UserName where apusrole.RoleId = 3 and tusr.IsActive = 1";
                        var listofdoctors1 = myapp.Database.SqlQuery<NotificationUserModel>(querytoexecute1).ToList();

                        response = notify.SendNotificationToSome("NotificationsPage", model.Message, User.Identity.Name, currentuser[0].FirstName + " " + currentuser[0].LastName, listofdoctors1);
                        break;
                    case "All My Reportings":
                        var reportingemployees = GetListOfEmployees("Edit");
                        var devideidstosendrept = (from u in reportingemployees
                                                   from d in deviceidlist
                                                   where u.CustomUserId == d.UserId
                                                   select new NotificationUserModel
                                                   {
                                                       DeviceId = d.DeviceId,
                                                       Userid = d.UserId
                                                   }).Distinct().ToList();
                        response = notify.SendNotificationToSome("NotificationsPage", model.Message, User.Identity.Name, currentuser[0].FirstName + " " + currentuser[0].LastName, devideidstosendrept);

                        break;
                    case "All My Location Employees":
                        int currentlocation = currentuser[0].LocationId.Value;
                        var unitcemps = myapp.tbl_User.Where(l => l.LocationId == currentlocation && l.IsActive == true).ToList();
                        var devideidstosendc = (from u in unitcemps
                                                from d in deviceidlist
                                                where u.CustomUserId == d.UserId
                                                select new NotificationUserModel
                                                {
                                                    DeviceId = d.DeviceId,
                                                    Userid = d.UserId
                                                }).Distinct().ToList();
                        response = notify.SendNotificationToSome("NotificationsPage", model.Message, User.Identity.Name, currentuser[0].FirstName + " " + currentuser[0].LastName, devideidstosendc);
                        break;
                    case "All My Department Employees":
                        int currentdepartment = currentuser[0].DepartmentId.Value;
                        var unitcempd = myapp.tbl_User.Where(l => l.DepartmentId == currentdepartment && l.IsActive == true).ToList();
                        var devideidstosendd = (from u in unitcempd
                                                from d in deviceidlist
                                                where u.CustomUserId == d.UserId
                                                select new NotificationUserModel
                                                {
                                                    DeviceId = d.DeviceId,
                                                    Userid = d.UserId
                                                }).Distinct().ToList();
                        response = notify.SendNotificationToSome("NotificationsPage", model.Message, User.Identity.Name, currentuser[0].FirstName + " " + currentuser[0].LastName, devideidstosendd);
                        break;
                }
            }
            return response;
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
        [HttpGet]
        [Route("GetAllMyNotifications")]
        public List<NotificationMobileModel> GetAllMyNotifications()
        {
            var list = myapp.tbl_NotificationLog.Where(l => l.ToUserId == User.Identity.Name).ToList();
            var list2 = (from l in list
                         select new NotificationMobileModel
                         {
                             ClickAction = l.ClickAction,
                             FromUserId = l.FromUserId,
                             Id = l.Id,
                             IsRead = l.IsRead.HasValue ? l.IsRead.Value : false,
                             Message = l.Message,
                             Params = l.Params,
                             SentOn = l.SentOn.HasValue ? l.SentOn.Value.ToString("dd/MM/yyyy") : "",
                             SentResponse = l.SentResponse,
                             ToUserId = l.ToUserId
                         }).ToList();
            return list2;
        }
        [HttpGet]
        [Route("GetMyUnreadNotifications")]
        public List<NotificationMobileModel> GetMyUnreadNotifications()
        {
            var list = myapp.tbl_NotificationLog.Where(l => l.ToUserId == User.Identity.Name && l.IsRead == false).ToList();
            var list2 = (from l in list
                         select new NotificationMobileModel
                         {
                             ClickAction = l.ClickAction,
                             FromUserId = l.FromUserId,
                             Id = l.Id,
                             IsRead = l.IsRead.HasValue ? l.IsRead.Value : false,
                             Message = l.Message,
                             Params = l.Params,
                             SentOn = l.SentOn.HasValue ? l.SentOn.Value.ToString("dd/MM/yyyy") : "",
                             SentResponse = l.SentResponse,
                             ToUserId = l.ToUserId
                         }).ToList();
            return list2;
        }
        [HttpGet]
        [Route("GetMySentNotifications")]
        public List<NotificationMobileModel> GetMySentNotifications()
        {
            var list = myapp.tbl_NotificationLog.Where(l => l.FromUserId == User.Identity.Name).ToList();
            var list2 = (from l in list
                         select new NotificationMobileModel
                         {
                             ClickAction = l.ClickAction,
                             FromUserId = l.FromUserId,
                             Id = l.Id,
                             IsRead = l.IsRead.HasValue ? l.IsRead.Value : false,
                             Message = l.Message,
                             Params = l.Params,
                             SentOn = l.SentOn.HasValue ? l.SentOn.Value.ToString("dd/MM/yyyy") : "",
                             SentResponse = l.SentResponse,
                             ToUserId = l.ToUserId
                         }).ToList();
            return list2;
        }
    }
}
