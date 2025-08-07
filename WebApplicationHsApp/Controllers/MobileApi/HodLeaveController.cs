using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    [RoutePrefix("api/HodLeave")]
    public class HodLeaveController : ApiController
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public string message = "";
        public double CurrentLeave = 0;
        HrDataManage hrdm = new HrDataManage();
        [HttpGet]
        [Route("GetListOfMyEmployees")]
        public List<tbl_User> GetListOfMyEmployees()
        {
            return GetListOfEmployees("Edit");
        }
      
        //Manager Option
        private List<tbl_User> GetListOfEmployees(string typeofview)
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
        [Route("GetMyEmployeesLeaveBalances")]
        public List<LeavesCountView> GetMyEmployeesLeaveBalances()
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
                    lv.CompoffBalance = GetCompoffLeaveBalancetotal(vs.UserId);
                    lv.IsActive = vs.IsActive;
                }
                Leavscount.Add(lv);
            }
            return Leavscount;
        }
        [HttpGet]
        [Route("GetMyEmployeesLeaveApplications")]
        public List<LeaveViewModels> GetMyEmployeesLeaveApplications()
        {
            var queryEmployees = GetListOfEmployees("Edit");
            var leaveslit = myapp.tbl_Leave.OrderByDescending(l => l.LeaveFromDate).ToList();
            var query = (from q in leaveslit
                         join rm in queryEmployees
                         on q.UserId equals rm.CustomUserId
                         //where q.DepartmentId == rm.DepartmentId
                         select new LeaveViewModels
                         {
                             AddressOnLeave = q.AddressOnLeave,
                             DateofAvailableCompoff = q.DateofAvailableCompoff.HasValue ? q.DateofAvailableCompoff.Value.ToString("dd/MM/yyyy") : "",
                             DepartmentName = q.DepartmentName,
                             IsCompOff = q.IsCompOff.HasValue ? q.IsCompOff.Value.ToString() : "false",
                             IsFullday = q.IsFullday.HasValue ? q.IsFullday.Value.ToString() : "false",
                             LeaveCreatedOn = q.CreatedOn.HasValue ? q.CreatedOn.Value.ToString("dd/MM/yyyy") : "",
                             LeaveFromDate = q.LeaveFromDate.HasValue ? q.LeaveFromDate.Value.ToString("dd/MM/yyyy") : "",
                             LeaveTodate = q.LeaveTodate.HasValue ? q.LeaveTodate.Value.ToString("dd/MM/yyyy") : "",
                             LeaveSessionDay = q.LeaveSessionDay,
                             LeaveStatus = q.LeaveStatus,
                             LeaveTypeName = q.LeaveTypeName,
                             UserName = q.UserName,
                             userid = q.UserId,
                             TotalLeaves = q.LeaveCount.HasValue ? q.LeaveCount.Value : 0,
                             ReasonForLeave = q.ReasonForLeave,
                             LocationName = q.LocationName,
                             Level1Approver = q.Level1Approver,
                             Level2Approver = q.Level2Approver,
                             LeaveId = q.LeaveId.ToString(),
                             id = q.LeaveId.ToString()
                         }).ToList();

            return query;
        }

        [HttpGet]
        [Route("GetMyEmployeesPermissions")]
        public List<PermissionViewModels> GetMyEmployeesPermissions()
        {
            var queryEmployees = GetListOfEmployees("Edit");
            var leaveslit = myapp.tbl_Permission.OrderByDescending(l => l.PermissionDate).ToList();
            var query = (from q in leaveslit
                         from m in queryEmployees
                         where q.UserId == m.CustomUserId
                         select new PermissionViewModels
                         {
                             Comments = q.Remarks,
                             CreatedOn = q.CreatedOn.Value.ToString("dd/MM/yyyy"),
                             DepartmentName = q.DepartmentName,
                             EndDate = q.EndDate.HasValue ? q.EndDate.Value.ToString("hh:mm tt") : "",
                             id = q.PermissionId,
                             LocationName = q.LocationName,
                             PermissionDate = q.PermissionDate.HasValue ? q.PermissionDate.Value.ToString("dd/MM/yyyy") : "",
                             Reason = q.Reason,
                             Requestapprovename = q.Level1Approver,
                             StartDate = q.StartDate.HasValue ? q.StartDate.Value.ToString("hh:mm tt") : "",
                             UserId = q.UserId,
                             Status = q.Status,
                             UserName = q.UserName
                         }).ToList();

            return query;
        }
        [HttpGet]
        [Route("GetMyEmployeesCompOffRequests")]
        public List<CompOffLeaveViewModel> GetMyEmployeesCompOffRequests()
        {
            var query = myapp.tbl_RequestCompOffLeave.OrderByDescending(l => l.CompOffDateTime).ToList();
            var queryEmployees = GetListOfEmployees("Edit");
            query = (from q in query
                     from m in queryEmployees
                     where q.UserId == m.CustomUserId
                     select q).ToList();
            var query2 = (from q in query
                          select new CompOffLeaveViewModel
                          {
                              CompOffDate = q.CompOffDate,
                              CompOffDateTime = q.CompOffDateTime,
                              CompOffLeave_Approver_1 = q.CompOffLeave_Approver_1,
                              CompOffLeave_Approver_2 = q.CompOffLeave_Approver_2,
                              CreatedBy = q.CreatedBy,
                              CreatedDate = q.CreatedDate,
                              CreatedDateTime = q.CreatedDateTime,
                              CreatedDate_CustomFormat = q.CreatedDate_CustomFormat,
                              DepartmentId = q.DepartmentId,
                              DepartmentName = q.DepartmentName,
                              ExpiryDate = q.ExpiryDate,
                              ExpiryDateTime = q.ExpiryDateTime,
                              id = q.ID,
                              IsApproved_Admin = q.IsApproved_Admin,
                              IsApproved_Manager = q.IsApproved_Manager,
                              IsApproved_Manager_2 = q.IsApproved_Manager_2,
                              IsApproved_Manager_3 = q.IsApproved_Manager_3,
                              IsApproved_Manager_4 = q.IsApproved_Manager_4,
                              IsLeaveTaken = q.IsLeaveTaken,
                              LeavesTakenCount = q.LeavesTakenCount,
                              Leave_Status = q.Leave_Status,
                              LocationId = q.LocationId,
                              LocationName = q.LocationName,
                              Record_Status = q.Record_Status,
                              RequestReason = q.RequestReason,
                              Request_Status = q.Request_Status,
                              ShiftTypeId = q.ShiftTypeId,
                              UserEmailId = q.UserEmailId,
                              UserId = q.UserId,
                              UserName = q.UserName
                          }).ToList();
            return query2;
        }
        private double GetCompoffLeaveBalancetotal(string username)
        {
            double balance = 0;
            var CompOffReqList = myapp.tbl_RequestCompOffLeave.Where(l => l.UserId == username && l.IsLeaveTaken == false && l.Leave_Status == "Approved").ToList();
            foreach (var vdd in CompOffReqList)
            {
                DateTime dtval = vdd.CompOffDateTime.Value.AddDays(90);
                if (dtval > DateTime.Now)
                {
                    if (vdd.IsApproved_Manager_4.Value)
                    {
                        balance = balance + 0.5;
                    }
                    else
                    {
                        if (vdd.LeavesTakenCount > 0)
                        {
                            balance = balance + 0.5;
                        }
                        else
                        {
                            balance = balance + 1;
                        }
                    }
                }
            }
            return balance;
        }
    }
}
