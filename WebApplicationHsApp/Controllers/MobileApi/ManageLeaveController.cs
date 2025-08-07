using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;
using WebApplicationHsApp.Models.MobileApi;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    [RoutePrefix("api/ManageLeave")]
    public class ManageLeaveController : ApiController
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public string message = "";
        public double CurrentLeave = 0;
        HrDataManage hrdm = new HrDataManage();
        [HttpGet]
        [Route("GetMyLeavesCount")]
        public List<tbl_ManageLeave> GetMyLeavesCount()
        {
            var list = myapp.tbl_ManageLeave.Where(u => u.UserId == User.Identity.Name && u.LeaveTypeId < 7).ToList();
            return list;
        }
        [HttpGet]
        [Route("GetLeaveTypes")]
        public List<tbl_LeaveType> GetLeaveTypes()
        {
            var list = myapp.tbl_LeaveType.Where(l => l.IsActive == true).ToList();
            if (User.IsInRole("HrAdmin") || User.IsInRole("Admin"))
            {
                return list;
            }
            else
            {
                list = list.Where(l => l.LeaveTypeId != 7 && l.LeaveTypeId != 8).ToList();
                var userl = myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).ToList();
                if (userl.Count > 0)
                {
                    if (userl[0].IsOffRollDoctor != null && userl[0].IsOffRollDoctor.Value)
                    {
                        list = list.Where(l => l.LeaveTypeId != 5).ToList();
                    }
                }
                return list;
            }
        }
        [HttpGet]
        [Route("GetAllMyLeavesCount")]
        public ApproverLeaveCount GetAllMyLeavesCount()
        {
            double balance = 0;
            balance = GetCompoffLeaveBalancetotal(User.Identity.Name);
            ApproverLeaveCount ap = new ApproverLeaveCount();

            var query1 = (from c in myapp.tbl_Leave
                          where c.LeaveStatus == "Pending" && (c.Level1Approver == User.Identity.Name || c.Level2Approver == User.Identity.Name)
                          select c).Count();
            ap.Leaves = query1;

            var tasks = (from c in myapp.tbl_Permission where c.Level1Approver == User.Identity.Name && c.Status == "Pending" select c).Count();
            ap.Permissions = tasks;
            var tasks2 = (from c in myapp.tbl_RequestCompOffLeave where c.CompOffLeave_Approver_1 == User.Identity.Name && c.Leave_Status == "Pending" select c).Count();
            ap.CompOff = tasks2;
            ap.CompOffBalance = balance;
            return ap;
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
        [HttpGet]
        [Route("GetLeavesWaitingForMyApproval")]
        public List<LeaveViewModels> GetLeavesWaitingForMyApproval()
        {
            var query1 =
                  (from c in myapp.tbl_Leave.OrderByDescending(l => l.CreatedOn).ToList()
                   join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
                   //join app2 in myapp.tbl_User on c.Level2Approver equals app2.CustomUserId
                   where c.LeaveStatus == "Pending" && (c.Level1Approver == User.Identity.Name || c.Level2Approver == User.Identity.Name)
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
                       AddressOnLeave = c.AddressOnLeave,
                       ReasonForLeave = c.ReasonForLeave,
                       TotalLeaves = c.LeaveCount.HasValue ? c.LeaveCount.Value : 0,
                       LeaveSessionDay = c.LeaveSessionDay,
                       LeaveCreatedOn = c.CreatedOn.Value.ToString("dd/MM/yyyy")
                   }).ToList();

            query1 = query1.ToList();
            return query1;
        }

        private void GetLeavesCountByValidateShiftTypes(DateTime dtstart, DateTime dtenddate, string userid, bool Isfullday, bool IsCompOff, string WeeklyOffDay = "")
        {
            if (IsCompOff)
            {
                CurrentLeave = 1;
                if (!Isfullday)
                    CurrentLeave = 0.5;
            }
            else
            {
                TimeSpan difference = dtenddate - dtstart;
                CurrentLeave = difference.TotalDays + 1;
                if (User.IsInRole("Doctor"))
                {
                    int dayofweek = 0;
                    switch (WeeklyOffDay)
                    {
                        case "Sunday":
                            dayofweek = 0;
                            break;
                        case "Monday":
                            dayofweek = 1;
                            break;
                        case "Tuesday":
                            dayofweek = 2;
                            break;
                        case "Wednesday":
                            dayofweek = 3;
                            break;
                        case "Thursday":
                            dayofweek = 4;
                            break;
                        case "Friday":
                            dayofweek = 5;
                            break;
                        case "Saturday":
                            dayofweek = 6;
                            break;
                    }
                    DateTime ndtstart = dtstart;
                    DateTime ndtenddate = dtenddate;
                    int totalweekoffs = 0;
                    while (ndtstart <= ndtenddate)
                    {
                        if ((int)ndtstart.DayOfWeek == dayofweek)
                        {
                            totalweekoffs = totalweekoffs + 1;
                        }
                        ndtstart = ndtstart.AddDays(1);
                    }
                    CurrentLeave = CurrentLeave - totalweekoffs;
                }
                else
                {
                    List<tbl_Roaster> RoasterData = myapp.tbl_Roaster.Where(e => e.UserId == userid && e.ShiftDate != null).ToList();
                    if (RoasterData.Count > 0)
                    {
                        var list = RoasterData.Where(l => l.ShiftDate >= dtstart && l.ShiftDate <= dtenddate).ToList();
                        var checksiftleaveorweekoff = list.Where(l => l.ShiftTypeId == 3 || l.ShiftTypeId == 4).Count();
                        CurrentLeave = CurrentLeave - checksiftleaveorweekoff;
                    }

                }
                if (CurrentLeave > 0 && !Isfullday)
                {
                    if (CurrentLeave == 1)
                    {
                        CurrentLeave = 0.5;
                    }
                }
            }

        }
        private bool CheckTheLeaveBalances(string Userid, int leavetypeid)
        {
            bool validatebalce = true;
            if (leavetypeid == 6 || leavetypeid == 1009 || leavetypeid == 7)
            {
                return validatebalce;
            }
            else
            {
                var leavelist = myapp.tbl_ManageLeave.Where(t => t.UserId == Userid && t.LeaveTypeId == leavetypeid).ToList();
                if (leavelist.Count > 0)
                {
                    if (leavelist[0].AvailableLeave != null && leavelist[0].AvailableLeave.Value != 0)
                    {
                        if (leavelist[0].AvailableLeave.Value >= CurrentLeave)
                        {
                            validatebalce = true;
                        }
                        else
                        {
                            validatebalce = false;
                        }
                    }
                    else
                    {
                        validatebalce = false;
                    }
                }
                else
                {
                    validatebalce = false;
                }
                if (!validatebalce)
                {
                    message = "your leave balance is low please contact your HR";
                }
            }
            return validatebalce;
        }

        private bool validateLeave(tbl_Leave tblleave)
        {
            bool ReturnStatus = true;
            message = "";
            //check leave in holiday
            bool IsCampOffLeave = Convert.ToBoolean(tblleave.IsCompOff);
            if (tblleave.LeaveTypeId == 1 || tblleave.LeaveTypeId == 4)
            {
                if (tblleave.LeaveFromDate.Value.Date.Year == DateTime.Now.Year)
                {
                    ReturnStatus = true;
                }
                else
                {
                    ReturnStatus = false;
                    message = "Casuval Sick Leaves need to be take this year only";
                }
            }
            if (!(CurrentLeave > 0) && !IsCampOffLeave)
            {
                ReturnStatus = false;
                message = "Please check the shif your trying to apply on weekoff or holiday";
            }

            if (tblleave.LeaveFromDate <= tblleave.LeaveTodate)
            {
                ReturnStatus = true;
            }
            else
            {
                ReturnStatus = false;
                message = "From Date should be grater than to date";
            }

            //1)      Employee shall be eligible to avail for any type of leave only after completion of 1 month from the date of joining.
            DateTime EmployeeDateOfJoing = DateTime.Now;
            var doj = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == tblleave.UserId);
            if (doj != null)
            {
                EmployeeDateOfJoing = Convert.ToDateTime(doj.DateOfJoining);
                if ((DateTime.Now - EmployeeDateOfJoing).TotalDays < 30 && tblleave.LeaveTypeId != 6)
                {
                    message = "Employee shall be eligible to avail for any type of leave only after completion of 1 month from the date of joining";
                    ReturnStatus = false;
                }
            }
            else
            {
                message = "Employee shall be eligible to avail for any type of leave only after completion of 1 month from the date of joining";
                ReturnStatus = false;
            }
            // Check Last month

            if (DateTime.UtcNow.Month == 12 && tblleave.LeaveTypeId == 1)
            {
                if (tblleave.LeaveFromDate.Value.Year != DateTime.UtcNow.Year)
                {
                    message = "you can't apply " + tblleave.LeaveFromDate.Value.Year + " leaves before jan";
                    ReturnStatus = false;
                }

            }

            DateTime EmployeeCalendarYearDate = EmployeeDateOfJoing.AddYears(1);
            /**Checking Comp-Off Data**/
            if (ReturnStatus && IsCampOffLeave)
            {
                if (Convert.ToInt32(tblleave.LeaveTypeId) == 6)
                {
                    if (CurrentLeave > 1)
                    {
                        message = "Comp-Off Should Not Apply More Than 1 Day";
                        ReturnStatus = false;
                    }

                    if (ReturnStatus)
                    {
                        var leavebanalcecmp = GetCompoffLeaveBalancetotal(User.Identity.Name);
                        if (leavebanalcecmp < CurrentLeave)
                        {
                            message = "Please Check Your Comp-Off Leave Balance Is Low";
                            ReturnStatus = false;
                        }
                    }
                }
                else
                {
                    message = "Comp-Off Should Apply With Casual Leave";
                    ReturnStatus = false;
                }
            }
            /*Checking CompOff Request Approved List*/
            if (IsCampOffLeave && ReturnStatus)
            {
                string UserId = User.Identity.Name;
                var CompOffReqList = myapp.tbl_RequestCompOffLeave.Where(e => e.UserId == UserId && (e.Leave_Status == "Approved" || e.IsApproved_Manager == true)).ToList();
                //
                if (CompOffReqList.Count > 0)
                {
                    double balance = GetCompoffLeaveBalancetotal(User.Identity.Name);

                    if (balance == 0 || balance == 0.0)
                    {
                        ReturnStatus = false;
                    }
                }
                else
                {
                    ReturnStatus = false;
                }
                if (!ReturnStatus)
                {
                    message = "You Don't Have Any Comp Off Leaves To Apply For Leave";
                }
            }
            /*Checking Roaster Data Added For This Employee or not*/
            if (ReturnStatus)
            {

                if (!User.IsInRole("Doctor"))
                {
                    List<tbl_Roaster> RoasterData = myapp.tbl_Roaster.Where(e => e.UserId == tblleave.UserId && e.ShiftDate != null && e.ShiftDate.Value >= tblleave.LeaveFromDate.Value && e.ShiftDate.Value <= tblleave.LeaveTodate).ToList();
                    if (RoasterData.Count > 0)
                    {
                        var checkleave = RoasterData.Where(r => r.ShiftTypeId == 3).ToList();
                        if (checkleave.Count > 0 && CurrentLeave == 0)
                        {
                            ReturnStatus = false;
                            message = "Please check the date you are trying to apply on Leave";
                        }

                    }
                    else
                    {
                        ReturnStatus = false;
                        message = "Shift Data Is Not Added For This Employee";
                    }
                }
            }
            if (ReturnStatus)
            {
                string strleaveuserid = tblleave.UserId;
                // check if already appliyed leave
                if (tblleave.IsFullday.Value)
                {
                    var checkaaplcount = myapp.tbl_Leave.Where(l => l.UserId == strleaveuserid &&
                        l.LeaveFromDate.Value >= tblleave.LeaveFromDate.Value && tblleave.LeaveFromDate.Value >= l.LeaveTodate.Value &&
                        l.LeaveStatus != "Cancelled" && l.LeaveStatus != "Reject").ToList();
                    if (checkaaplcount.Count > 0)
                    {
                        message = "you have already applied leave on these dates";
                        ReturnStatus = false;
                    }
                }
                else
                {
                    var checkaaplcount = myapp.tbl_Leave.Where(l => l.UserId == strleaveuserid &&
                        l.LeaveFromDate.Value >= tblleave.LeaveFromDate.Value &&
                        l.LeaveFromDate.Value <= tblleave.LeaveTodate.Value &&
                         l.LeaveTodate.Value >= tblleave.LeaveFromDate.Value &&
                         l.LeaveTodate.Value <= tblleave.LeaveTodate.Value &&
                        l.IsFullday.Value == true &&
                        l.LeaveStatus != "Cancelled" &&
                        l.LeaveStatus != "Reject" && l.LeaveTypeId != tblleave.LeaveTypeId).ToList();
                    if (checkaaplcount.Count > 0)
                    {
                        if (tblleave.LeaveSessionDay != null && tblleave.LeaveSessionDay == "1st Half")
                        {
                        }
                        else if (tblleave.LeaveSessionDay != null && tblleave.LeaveSessionDay == "2nd Half")
                        {
                            var valco = checkaaplcount.Where(l => l.LeaveTypeId != 6).ToList();
                            if (valco.Count > 0)
                            {
                                message = "you have already applied leave on these dates";
                                ReturnStatus = false;
                            }
                        }
                        else
                        {
                            message = "you have already applied leave on these dates";
                            ReturnStatus = false;
                        }
                    }
                }
                if (ReturnStatus && IsCampOffLeave)
                {
                    //Check Campoff with Sick Leave
                    ReturnStatus = hrdm.CheckApplyLeaveWithPreviousAppliedLeaveData_HRControllerCompoff(Convert.ToDateTime(tblleave.LeaveFromDate), Convert.ToDateTime(tblleave.LeaveTodate), tblleave.UserId, 4);
                    if (!ReturnStatus)
                    {
                        message = "you are trying to apply comp off leave with sick leave will not allow";
                    }
                }
                switch (tblleave.LeaveTypeId)
                {
                    case 1:
                        //Casuval Validation 1 : Casuval Leaves should not be availed more that three days
                        //if (CurrentLeave > 3)
                        //{
                        //    message = " Casuval Leaves should not be availed more that three days";
                        //    ReturnStatus = false;
                        //}
                        //4)      Casual leave should be applied for sanction at least one day in advance.
                        //if (!hrdm.CheckTheCasuvalLeaveBeforeDasy(tblleave.LeaveFromDate.Value, tblleave.LeaveTodate.Value, tblleave.UserId))
                        //{
                        //   message = "Casual leave should be applied for sanction at least one day in advance";
                        //    ReturnStatus = false;
                        //}
                        //else
                        //{
                        //}
                        /*Checking With Previous Leaves*/
                        if (!IsCampOffLeave && ReturnStatus)
                        {
                            if (CurrentLeave == 0.5 && tblleave.LeaveSessionDay == "1st Half" || tblleave.LeaveSessionDay == "2nd Half")
                            {
                                ReturnStatus = hrdm.CheckApplyLeaveWithPreviousHalfAppliedLeaveData_HRController(Convert.ToDateTime(tblleave.LeaveFromDate), Convert.ToDateTime(tblleave.LeaveTodate), tblleave.UserId, 1, tblleave.LeaveSessionDay);
                                if (!ReturnStatus)
                                {
                                    message = "you have already applied leave on these dates before/after.";
                                }
                            }
                            else
                            {
                                ReturnStatus = hrdm.CheckApplyLeaveWithPreviousAppliedLeaveData_HRController(Convert.ToDateTime(tblleave.LeaveFromDate), Convert.ToDateTime(tblleave.LeaveTodate), tblleave.UserId, 1);
                                if (!ReturnStatus)
                                {
                                    message = "you have already applied leave on these dates before/after.";
                                }
                            }
                        }
                        break;
                    case 4:
                        //Sick Leave Validation 1 : Sick Leaves should not be availed more that 12 days
                        var count1 = CurrentLeave;
                        if (count1 > 12)
                        {
                            message = " Sick Leaves should not be availed more that 12 days";
                            ReturnStatus = false;
                        }
                        else
                        {
                        }
                        /*Checking With Previous Leaves*/
                        if (ReturnStatus)
                        {
                            if (CurrentLeave == 0.5 && tblleave.LeaveSessionDay == "1st Half" || tblleave.LeaveSessionDay == "2nd Half")
                            {
                                ReturnStatus = hrdm.CheckApplyLeaveWithPreviousHalfAppliedLeaveData_HRController(Convert.ToDateTime(tblleave.LeaveFromDate), Convert.ToDateTime(tblleave.LeaveTodate), tblleave.UserId, 4, tblleave.LeaveSessionDay);
                                if (!ReturnStatus)
                                {
                                    message = "you have already applied leave on these dates before/after.";
                                }
                            }

                            else
                            {
                                ReturnStatus = hrdm.CheckApplyLeaveWithPreviousAppliedLeaveData_HRController(Convert.ToDateTime(tblleave.LeaveFromDate), Convert.ToDateTime(tblleave.LeaveTodate), tblleave.UserId, 4);
                                if (!ReturnStatus)
                                {
                                    message = "you have already applied leave on these dates before/after.";
                                }
                            }

                        }
                        break;
                    case 5:
                        //Earned Leave Validation 1 : Earned Leaves should  be minimum of 4 days
                        //var count2 = CurrentLeave;
                        //if (count2 < 4)
                        //{
                        //    message = "Earned Leaves should  be minimum of 4 days";
                        //    ReturnStatus = false;
                        //}
                        //else
                        //{
                        //}
                        if (ReturnStatus)
                        {
                            var PrevLeaveList = myapp.tbl_Leave.Where(e => e.UserId == tblleave.UserId).ToList();
                            PrevLeaveList = PrevLeaveList.Where(e => Convert.ToInt32(e.LeaveTypeId) == 5 && EmployeeDateOfJoing >= e.CreatedOn && EmployeeCalendarYearDate <= e.CreatedOn).ToList();
                            if (PrevLeaveList.Count > 3)
                            {
                                message = "Earned Leaves Should Not entitled fo more than three spells in a calendar year.";
                                ReturnStatus = false;
                            }
                        }
                        /*Checking With Previous Leaves*/
                        if (!IsCampOffLeave && ReturnStatus)
                        {
                            //ReturnStatus = hrdm.CheckApplyLeaveWithPreviousAppliedLeaveData_HRController(Convert.ToDateTime(tblleave.LeaveFromDate), Convert.ToDateTime(tblleave.LeaveTodate), tblleave.UserId, 5);
                            //if (!ReturnStatus)
                            //{
                            //    message = "you have already applied leave on these dates before/after.";
                            //}
                            if (CurrentLeave == 0.5 && tblleave.LeaveSessionDay == "1st Half" || tblleave.LeaveSessionDay == "2nd Half")
                            {
                                ReturnStatus = hrdm.CheckApplyLeaveWithPreviousHalfAppliedLeaveData_HRController(Convert.ToDateTime(tblleave.LeaveFromDate), Convert.ToDateTime(tblleave.LeaveTodate), tblleave.UserId, 5, tblleave.LeaveSessionDay);
                                if (!ReturnStatus)
                                {
                                    message = "you have already applied leave on these dates before/after.";
                                }
                            }

                            else
                            {
                                ReturnStatus = hrdm.CheckApplyLeaveWithPreviousAppliedLeaveData_HRController(Convert.ToDateTime(tblleave.LeaveFromDate), Convert.ToDateTime(tblleave.LeaveTodate), tblleave.UserId, 5);
                                if (!ReturnStatus)
                                {
                                    message = "you have already applied leave on these dates before/after.";
                                }
                            }
                        }
                        break;
                }
            }
            return ReturnStatus;
        }
        private bool CheckEmployeeIsonLeave(string userid, DateTime Fromdate, DateTime Todate)
        {
            var list = myapp.tbl_Leave.Where(l => l.UserId == userid &&
                        l.LeaveFromDate.Value >= Fromdate &&
                        l.LeaveTodate.Value <= Todate).ToList();
            if (list.Count > 0)
            {
                return false;
            }
            return true;
        }
        [HttpPost]
        [Route("CreateNewLeave")]
        public string SaveNewLeave(LeaveApply model)
        {
            ConvertLeaveObj clo = new ConvertLeaveObj();
            tbl_Leave tbll = clo.ConvertMobile(model);
            if (tbll.LeaveFromDate != null && tbll.LeaveTodate != null)
            {
                message = "";
                tbll.UserId = User.Identity.Name;

                GetLeavesCountByValidateShiftTypes(tbll.LeaveFromDate.Value, tbll.LeaveTodate.Value, tbll.UserId, tbll.IsFullday.Value, tbll.IsCompOff.Value, tbll.WeeklyOffDay);

                if (CurrentLeave > 0)
                {
                    if ((validateLeave(tbll) && CheckTheLeaveBalances(tbll.UserId, tbll.LeaveTypeId.Value)) || tbll.LeaveTypeId.Value == 1009)
                    {
                        tbll.CreatedBy = User.Identity.Name;
                        tbll.CreatedOn = DateTime.Now;
                        string UserIdofEmployeeIfManagerLeave = tbll.Level1Approver;
                        tbll.Level1Approver = "";
                        bool isrepotmanager = false;
                        bool issaveleave = true;
                        //tbll.LeaveStatus
                        var rpt = hrdm.GetReportingMgr(User.Identity.Name, tbll.LeaveFromDate.Value, tbll.LeaveTodate.Value);
                        var userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == User.Identity.Name);

                        if (userinfo != null)
                        {

                            tbll.UserName = userinfo.FirstName + " " + userinfo.LastName;
                            tbll.LocationId = userinfo.LocationId;
                            tbll.LocationName = userinfo.LocationName;
                            tbll.DepartmentId = userinfo.DepartmentId;
                            tbll.DepartmentName = userinfo.DepartmentName;
                            tbll.LeaveStatus = "Pending";
                            if (tbll.UserId.ToLowerInvariant() == "fh_1")
                            {
                                tbll.LeaveStatus = "Approved";
                                rpt = "fh_1";
                                tbll.Level1Approved = true;
                                tbll.Level2Approved = true;
                            }
                            tbll.IsActive = true;
                            tbll.LeaveCount = CurrentLeave;
                            if (!string.IsNullOrEmpty(rpt))
                            {
                                tbll.Level1Approver = rpt;
                                tbll.Level2Approver = rpt;
                            }
                            else
                            {
                                issaveleave = false;
                                message = "Reporting Manager Not avaliable";
                            }
                            //Send Mail
                        }
                        else
                        {
                            issaveleave = false;
                            message = "Please contact with HR you are not allowed to apply leave";
                        }
                        if (issaveleave)
                        {

                            if (User.IsInRole("DepartmentManager"))
                            {
                                if (UserIdofEmployeeIfManagerLeave != null && UserIdofEmployeeIfManagerLeave != "")
                                {
                                    isrepotmanager = true;
                                    // Check altrnate employee is on leave 
                                    if (CheckEmployeeIsonLeave(UserIdofEmployeeIfManagerLeave, tbll.LeaveFromDate.Value, tbll.LeaveTodate.Value))
                                    {
                                        var data = myapp.tbl_ReportingManager.Where(a => a.UserId == tbll.UserId).ToList();
                                        if (data.Count() > 0)
                                        {
                                            foreach (var dts in data)
                                            {
                                                dts.Emp_UserId = UserIdofEmployeeIfManagerLeave;
                                                dts.Is_OnLeave = true;
                                                dts.FromDate = tbll.LeaveFromDate.Value;
                                                dts.ToDate = tbll.LeaveTodate.Value;
                                            }
                                            myapp.SaveChanges();

                                            // add role 

                                            var checkrole = myapp.AspNetUsers.Where(l => l.UserName == UserIdofEmployeeIfManagerLeave).ToList();
                                            if (checkrole.Count > 0)
                                            {

                                            }
                                        }
                                    }
                                    else
                                    {
                                        issaveleave = false;
                                        message = "Please check the Reliever on leave";
                                    }
                                }
                            }
                            if (issaveleave)
                            {
                                //if (tbll.IsCompOff.Value)
                                //{
                                //    var cmplist = myapp.tbl_RequestCompOffLeave.Where(t => t.IsLeaveTaken == false && t.UserId == tbll.UserId && t.Leave_Status == "Approved").OrderBy(t => t.CompOffDateTime).ToList();
                                //    foreach (var v in cmplist)
                                //    {
                                //        DateTime dtval = v.CompOffDateTime.Value.AddDays(90);
                                //        if (dtval > DateTime.Now)
                                //        {
                                //            if (tbll.IsFullday.Value)
                                //            {
                                //                if (!v.IsApproved_Manager_4.Value)
                                //                {
                                //                    tbll.DateOfCompOffLeave = v.CompOffDateTime;
                                //                    v.IsLeaveTaken = true;
                                //                    myapp.SaveChanges();
                                //                    break;
                                //                }
                                //            }
                                //            else
                                //            {
                                //                if (v.LeavesTakenCount > 0)
                                //                {
                                //                    tbll.DateOfCompOffLeave = v.CompOffDateTime;
                                //                    v.IsLeaveTaken = true;
                                //                    v.LeavesTakenCount = 1;
                                //                    myapp.SaveChanges();
                                //                    break;
                                //                }
                                //                else
                                //                {
                                //                    tbll.DateOfCompOffLeave = v.CompOffDateTime;
                                //                    v.LeavesTakenCount = 0.5;
                                //                    if (v.IsApproved_Manager_4.Value)
                                //                    {
                                //                        v.IsLeaveTaken = true;
                                //                    }
                                //                    myapp.SaveChanges();
                                //                    break;
                                //                }
                                //            }
                                //        }
                                //    }
                                //}
                                try
                                {
                                    tbll.LeaveTypeName = myapp.tbl_LeaveType.Where(l => l.LeaveTypeId == tbll.LeaveTypeId).SingleOrDefault().LeaveName;
                                }
                                catch { }
                                myapp.tbl_Leave.Add(tbll);
                                myapp.SaveChanges();

                                var leavelist = myapp.tbl_ManageLeave.Where(t => t.UserId == tbll.UserId && t.LeaveTypeId == tbll.LeaveTypeId).ToList();

                                if (leavelist.Count > 0)
                                {
                                    if (!tbll.IsFullday.Value)
                                    {
                                        if (leavelist[0].AvailableLeave != null && leavelist[0].AvailableLeave.Value != 0)
                                        {
                                            leavelist[0].AvailableLeave = leavelist[0].AvailableLeave - 0.5;
                                        }
                                        else
                                        {
                                            leavelist[0].AvailableLeave = leavelist[0].CountOfLeave - 0.5;
                                        }
                                    }
                                    else
                                    {
                                        if (leavelist[0].AvailableLeave != null && leavelist[0].AvailableLeave.Value != 0)
                                        {
                                            leavelist[0].AvailableLeave = leavelist[0].AvailableLeave - CurrentLeave;
                                        }
                                        else
                                        {
                                            leavelist[0].AvailableLeave = leavelist[0].CountOfLeave - CurrentLeave;
                                        }
                                    }
                                    myapp.SaveChanges();
                                }

                                if (rpt != null && rpt != "")
                                {
                                    var cc = string.Empty;
                                    //var tblrptmng = myapp.tbl_ReportingManager.Where(a => a.Emp_UserId == rpt).ToList();
                                    //if (tblrptmng != null && tblrptmng.Count > 0)
                                    //{
                                    //    foreach (var vrmp in tblrptmng)
                                    //    {
                                    //        if (vrmp.Is_OnLeave == true)
                                    //        {
                                    //            string rptuseridtest = vrmp.UserId;
                                    //            var LeaveManagerinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == rptuseridtest);
                                    //            cc = LeaveManagerinfo.EmailId;

                                    //        }
                                    //    }

                                    //}

                                    var Managerinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == rpt);
                                    string body = string.Empty;
                                    string emailidofemp = "";
                                    if (isrepotmanager)
                                    {
                                        var listofuser = myapp.tbl_User.Where(a => a.CustomUserId == UserIdofEmployeeIfManagerLeave).ToList();

                                        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/HodRequestForLeave.html")))
                                        {
                                            body = reader.ReadToEnd();
                                        }
                                        body = body.Replace("{Name}", tbll.UserName);
                                        if (tbll.LeaveTypeId == 6)
                                        {
                                            body = body.Replace("{Days}", "1");
                                        }
                                        else
                                        {
                                            body = body.Replace("{Days}", CurrentLeave.ToString());
                                        }
                                        body = body.Replace("{fromdate}", (tbll.LeaveFromDate != null) ? Convert.ToDateTime(tbll.LeaveFromDate).ToString("dd/MM/yyyy") : "");
                                        body = body.Replace("{todate}", (tbll.LeaveTodate != null) ? Convert.ToDateTime(tbll.LeaveTodate).ToString("dd/MM/yyyy") : "");
                                        body = body.Replace("{reason}", tbll.ReasonForLeave);
                                        body = body.Replace("{Designation}", userinfo.Designation);
                                        body = body.Replace("{HrLeaveId}", tbll.LeaveId.ToString());
                                        if (listofuser.Count > 0)
                                        {
                                            emailidofemp = listofuser[0].EmailId;
                                            body = body.Replace("{subemployee}", listofuser[0].FirstName + " " + listofuser[0].LastName);
                                        }
                                    }
                                    else
                                    {
                                        using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/RequestForLeave.html")))
                                        {
                                            body = reader.ReadToEnd();
                                        }
                                        body = body.Replace("{Name}", tbll.UserName);
                                        if (tbll.LeaveTypeId == 6)
                                        {
                                            body = body.Replace("{Days}", "1");
                                        }
                                        else
                                        {
                                            body = body.Replace("{Days}", CurrentLeave.ToString());
                                        }
                                        body = body.Replace("{fromdate}", (tbll.LeaveFromDate != null) ? Convert.ToDateTime(tbll.LeaveFromDate).ToString("dd/MM/yyyy") : "");
                                        body = body.Replace("{todate}", (tbll.LeaveTodate != null) ? Convert.ToDateTime(tbll.LeaveTodate).ToString("dd/MM/yyyy") : "");
                                        body = body.Replace("{reason}", tbll.ReasonForLeave);
                                        body = body.Replace("{Designation}", userinfo.Designation);
                                        body = body.Replace("{HrLeaveId}", tbll.LeaveId.ToString());
                                    }
                                    string Subject = "";
                                    if (tbll.LeaveTypeId == 6)
                                    {
                                        Subject = tbll.UserName + " is on leave " + Convert.ToDateTime(tbll.LeaveTodate).ToString("dd/MM/yyyy");
                                    }
                                    else
                                    {
                                        Subject = tbll.UserName + " is on leave from " + Convert.ToDateTime(tbll.LeaveFromDate).ToString("dd/MM/yyyy") + " to " + Convert.ToDateTime(tbll.LeaveTodate).ToString("dd/MM/yyyy");
                                    }

                                    CustomModel cm = new CustomModel();
                                    MailModel mailmodel = new MailModel();
                                    mailmodel.fromemail = "Leave@hospitals.com";
                                    mailmodel.toemail = Managerinfo.EmailId;
                                    if (emailidofemp != null && emailidofemp != "")
                                    {
                                        mailmodel.toemail = mailmodel.toemail + "," + emailidofemp;
                                    }
                                    mailmodel.subject = Subject;
                                    mailmodel.body = body;
                                    mailmodel.filepath = "";
                                    mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                                    mailmodel.fromname = "Leave Application";
                                    mailmodel.ccemail = cc;
                                    cm.SendEmail(mailmodel);
                                    NotificationSendModel notify = new NotificationSendModel();
                                    var devideidstosend = (
                                                           from d in myapp.tbl_DeviceInfo
                                                           where Managerinfo.CustomUserId == d.UserId
                                                           select new NotificationUserModel
                                                           {
                                                               DeviceId = d.DeviceId,
                                                               Userid = d.UserId
                                                           }).Distinct().ToList();
                                    var response = notify.SendNotificationToSome("ApproveLeavesPage", Subject, User.Identity.Name, "Approve Leave", devideidstosend);
                                    message = "Leave applied successfully";
                                }
                            }
                        }
                    }
                }
                else
                {
                    message = "You are trying to apply leave on holiday or weekoff";
                }
            }
            else
            {
                message = "Invalid Date";
            }
            return message;
        }
        [HttpGet]
        [Route("GetMyAppliedLeaves")]
        public List<LeaveViewModels> GetMyAppliedLeaves()
        {
            var list = myapp.tbl_Leave.Where(l => l.UserId == User.Identity.Name).OrderByDescending(l => l.CreatedOn).ToList();
            var query = (from c in list
                         join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
                         //join app2 in myapp.tbl_User on c.Level2Approver equals app2.CustomUserId

                         select new LeaveViewModels
                         {
                             LeaveId = c.LeaveId.ToString(),
                             LeaveTypeName = c.LeaveTypeName,
                             IsFullday = c.IsFullday.Value.ToString(),
                             IsCompOff = c.IsCompOff.ToString(),
                             LeaveFromDate = c.LeaveFromDate.Value.ToString("dd/MM/yyyy"),
                             LeaveTodate = c.LeaveTodate.Value.ToString("dd/MM/yyyy"),
                             LeaveStatus = c.LeaveStatus,
                             Level1Approver = app1.FirstName + " " + app1.LastName,
                             LeaveSessionDay = c.LeaveSessionDay,
                             LeaveCreatedOn = c.CreatedOn.Value.ToString("dd/MM/yyyy"),
                             TotalLeaves = c.LeaveCount.HasValue ? c.LeaveCount.Value : 0,
                             ReasonForLeave = c.ReasonForLeave
                             //Level2Approver = app2.FirstName + " " + app2.LastName
                         }).ToList();
            query = query.ToList();
            return query;
        }

        [HttpPost]
        [Route("CreateNewPermission")]
        public string SaveNewPermission(PermissionModel model)
        {
            var msg = hrdm.SaveLeavePermission(model.date, model.starttime, model.endtime, model.reason, model.approval, User.Identity.Name);
            return msg;
        }
        [HttpGet]
        [Route("GetMyAppliedPermissions")]
        public List<PermissionViewModels> GetMyAppliedPermissions()
        {
            var permissions = myapp.tbl_Permission.Where(l => l.UserId == User.Identity.Name).OrderByDescending(l => l.CreatedOn).ToList();
            var tasks =
                    (from c in permissions
                     join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
                     where c.UserId == User.Identity.Name && app1.IsActive == true
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
                         CreatedOn = c.CreatedOn.Value.ToString("dd/MM/yyyy")

                     }).ToList();
            return tasks;
        }

        [HttpGet]
        [Route("GetMyShiftsForCompOff")]
        public List<ShiftTypeViewModel> GetMyShiftsForCompOff()
        {
            int User_DeptID = 0, User_SubDeptID = 0;
            List<tbl_User> dept = myapp.tbl_User.Where(e => e.CustomUserId == User.Identity.Name).ToList();
            if (dept.Count > 0)
            {
                User_DeptID = Convert.ToInt32(dept[0].DepartmentId);
                User_SubDeptID = Convert.ToInt32(dept[0].SubDepartmentId);
            }
            var DeptVsShiptType = myapp.tbl_DepartmentShifts.Where(e => e.DepartmentId == User_DeptID).ToList();
            List<int> IDList = DeptVsShiptType.Select(e => Convert.ToInt32(e.ShiftTypeId)).Distinct().ToList();
            var list = (from o in myapp.tbl_ShiftType
                        where IDList.Contains(o.ShiftTypeId)
                        select new ShiftTypeViewModel
                        {
                            id = o.ShiftTypeId,
                            Name = o.ShiftTypeName,
                            starttime = o.ShiftStartTime,
                            endtime = o.ShiftEndTime
                        }).ToList();
            list = list.Where(l => l.id != null).ToList();
            return list;
        }

        [HttpPost]
        [Route("CreateNewCompOffLeave")]
        public string CreateNewCompOffLeave(CompOffTypeModel cmpty)
        {
            tbl_RequestCompOffLeave ReqData = new tbl_RequestCompOffLeave();
            ReqData.CompOffDate = cmpty.CompOffDate;
            ReqData.RequestReason = cmpty.RequestReason;
            ReqData.ShiftTypeId = cmpty.ShiftTypeId;
            //ReqData.DayWorkedType = cmpty.DayWorkedType;

            message = "";
            try
            {

                string UserId = User.Identity.Name;
                DateTime CompOffDateTime = ProjectConvert.ConverDateStringtoDatetime(cmpty.CompOffDate);
                var shift = (from v in myapp.tbl_Roaster where v.UserId == UserId select v).ToList();
                var RoasterData = myapp.tbl_Roaster.Where(e => e.UserId == UserId && e.ShiftDate != null && e.ShiftDate == CompOffDateTime).ToList();


                if (RoasterData != null && RoasterData.Count > 0)
                {
                    var countofdate = RoasterData.Where(l => l.ShiftTypeId == 3 || l.ShiftTypeId == 4).ToList();
                    if (countofdate.Count() > 0)
                    {

                        var CompOffLeaveRequestList = myapp.tbl_RequestCompOffLeave.Where(e => e.UserId == UserId && e.Leave_Status != "Rejected" && e.Leave_Status != "Canceled").ToList();
                        if (ReqData != null)
                        {

                            if (CompOffLeaveRequestList.Where(e => e.CompOffDate == ReqData.CompOffDate).Count() == 0)
                            {
                                var rpt = hrdm.GetReportingMgr(User.Identity.Name, CompOffDateTime, CompOffDateTime);

                                var userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == User.Identity.Name);
                                if (CompOffDateTime > userinfo.DateOfJoining)
                                {
                                    if (userinfo != null)
                                    {
                                        ReqData.UserName = userinfo.FirstName + " " + userinfo.LastName;
                                        ReqData.LocationId = userinfo.LocationId;
                                        ReqData.LocationName = userinfo.LocationName;
                                        ReqData.DepartmentId = userinfo.DepartmentId;
                                        ReqData.DepartmentName = userinfo.DepartmentName;
                                        if (!string.IsNullOrEmpty(rpt))
                                        {
                                            ReqData.CompOffLeave_Approver_1 = rpt;
                                            ReqData.CompOffLeave_Approver_2 = rpt;
                                        }
                                        ReqData.CompOffDateTime = CompOffDateTime;
                                        ReqData.CreatedBy = UserId;
                                        ReqData.CreatedDate = DateTime.Now.ToString("dd/MM/yyyy");
                                        ReqData.CreatedDateTime = DateTime.Now;
                                        ReqData.ExpiryDate = DateTime.Now.AddDays(30).ToString("dd/MM/yyyy");
                                        ReqData.ExpiryDateTime = DateTime.Now.AddDays(30);
                                        ReqData.IsApproved_Admin = false;
                                        ReqData.IsApproved_Manager = false;
                                        ReqData.IsApproved_Manager_2 = false;
                                        ReqData.IsApproved_Manager_3 = false;
                                        if (cmpty.DayWorkedType != null && cmpty.DayWorkedType == "Half Day")
                                        {
                                            ReqData.IsApproved_Manager_4 = true;
                                        }
                                        else
                                        {
                                            ReqData.IsApproved_Manager_4 = false;
                                        }
                                        ReqData.Leave_Status = "Pending";
                                        ReqData.Record_Status = true;
                                        ReqData.UserEmailId = userinfo.EmailId;
                                        ReqData.UserId = userinfo.CustomUserId;
                                        ReqData.IsLeaveTaken = false;
                                        ReqData.LeavesTakenCount = 0;
                                        myapp.tbl_RequestCompOffLeave.Add(ReqData);
                                        myapp.SaveChanges();
                                        countofdate[0].ShiftTypeId = ReqData.ShiftTypeId;
                                        if (ReqData.RequestReason != null && ReqData.RequestReason != "")
                                        {
                                            try
                                            {
                                                countofdate[0].ShiftTypeName = myapp.tbl_ShiftType.Where(l => l.ShiftTypeId == ReqData.ShiftTypeId).SingleOrDefault().ShiftTypeName;
                                            }
                                            catch { }
                                        }
                                        myapp.SaveChanges();
                                        message = "Success";
                                        NotificationSendModel notify = new NotificationSendModel();
                                        var devideidstosend = (
                                                               from d in myapp.tbl_DeviceInfo
                                                               where rpt == d.UserId
                                                               select new NotificationUserModel
                                                               {
                                                                   DeviceId = d.DeviceId,
                                                                   Userid = d.UserId
                                                               }).Distinct().ToList();
                                        var response = notify.SendNotificationToSome("ApproveCompOffPage", "CompOff  Approve Request from " + ReqData.UserName, userinfo.CustomUserId, "Approve CompOff", devideidstosend);
                                    }
                                }
                                else
                                {
                                    message = "A CompOff Leave Should apply after joining date.";
                                }
                            }
                            else
                            {
                                message = "A CompOff Leave Is Already Applied On This Date.";
                            }
                        }
                    }
                    else
                    {
                        message = "CompOff must be apply on either week off or holidays";
                    }
                }
                else
                {
                    message = "Please check duty roster not available for this day";
                }
            }
            catch (Exception EX)
            {
                message = EX.Message;
            }
            return message;
        }

        [HttpGet]
        [Route("GetMyRequestCompOff")]
        public List<tbl_RequestCompOffLeave> GetMyRequestCompOff()
        {
            string UserId = User.Identity.Name;
            var query = myapp.tbl_RequestCompOffLeave.Where(e => e.UserId == UserId).OrderByDescending(l => l.CreatedDateTime).ToList();
            return query;
        }
        [HttpGet]
        [Route("GetPermissionsWaitingForMyApproval")]
        public List<PermissionViewModels> GetPermissionsWaitingForMyApproval()
        {
            var query = myapp.tbl_Permission.OrderByDescending(l => l.CreatedOn).ToList();
            var tasks =
              (from c in query
               join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
               where c.Level1Approver == User.Identity.Name && c.Status == "Pending"
               && app1.IsActive == true
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
                   UserName = c.UserName,
                   CreatedOn = c.CreatedOn.Value.ToString("dd/MM/yyyy")

               });
            return tasks.ToList();
        }


        [HttpGet]
        [Route("GetCompOffRequestsForMyApproval")]
        public List<tbl_RequestCompOffLeave> GetCompOffRequestsForMyApproval()
        {
            string UserId = User.Identity.Name;
            var query = myapp.tbl_RequestCompOffLeave.Where(e => e.Leave_Status == "Pending" && (e.CompOffLeave_Approver_1 == UserId || e.CompOffLeave_Approver_2 == UserId)).OrderByDescending(l => l.CreatedDateTime).ToList();
            query = query.OrderBy(l => l.Leave_Status).ToList();
            return query;
        }

        [HttpPost]
        [Route("ApproveLeave")]
        public string ApproveLeave(int LeaveId)
        {
            message = "Success";
            var tasks = myapp.tbl_Leave.Where(t => t.LeaveId == LeaveId).ToList();
            if (tasks.Count > 0)
            {
                tasks[0].LeaveStatus = "Approved";
                if (tasks[0].Level1Approver == User.Identity.Name && tasks[0].Level1Approved == null)
                {
                    tasks[0].Level1Approved = true;
                }
                else if (tasks[0].Level2Approver == User.Identity.Name && tasks[0].Level2Approved == null)
                {
                    tasks[0].Level2Approved = true;
                }
                myapp.SaveChanges();
            }
            return message;
        }
        [HttpPost]
        [Route("RejectLeave")]
        public string RejectLeave(int LeaveId, string comments)
        {
            message = "Success";
            var tasks = myapp.tbl_Leave.Where(t => t.LeaveId == LeaveId).ToList();
            if (tasks.Count > 0)
            {
                if (tasks[0].LeaveStatus != "Cancelled" && tasks[0].LeaveStatus != "Reject")
                {
                    tasks[0].LeaveStatus = "Reject";
                    if (tasks[0].Level1Approver == User.Identity.Name && tasks[0].Level1Approved == null)
                    {
                        tasks[0].Level1Approved = false;
                        tasks[0].Level1ApproveComment = comments;
                    }
                    else if (tasks[0].Level2Approver == User.Identity.Name && tasks[0].Level2Approved == null)
                    {
                        tasks[0].Level2Approved = false;
                        tasks[0].Level2ApproveComment = comments;
                    }
                    string cususerid = tasks[0].UserId;
                    int Lavetypeid = tasks[0].LeaveTypeId.Value;
                    var list = myapp.tbl_ManageLeave.Where(v => v.UserId == cususerid && v.LeaveTypeId == Lavetypeid).ToList();
                    if (list.Count > 0)
                    {
                        if (!tasks[0].IsFullday.Value)
                        {
                            list[0].AvailableLeave = list[0].AvailableLeave + 0.5;
                        }
                        else
                        {
                            if (tasks[0].LeaveCount == null || tasks[0].LeaveCount == 0)
                            {
                                tasks[0].LeaveCount = 1;
                            }
                            list[0].AvailableLeave = list[0].AvailableLeave + tasks[0].LeaveCount;
                            myapp.SaveChanges();
                        }

                    }
                    tasks[0].LeaveCount = 0;
                    myapp.SaveChanges();
                    //if (tasks[0].LeaveTypeId == 6)
                    //{
                    //    if (tasks[0].DateofAvailableCompoff != null)
                    //    {
                    //        DateTime Dt = tasks[0].DateofAvailableCompoff.Value.Date;
                    //        string useridcurrr = tasks[0].UserId;
                    //        var cmprequest = myapp.tbl_RequestCompOffLeave.Where(l => l.UserId == useridcurrr).ToList();
                    //        cmprequest = cmprequest.Where(l => l.CompOffDateTime.Value.Date == Dt.Date).ToList();
                    //        if (cmprequest.Count > 0)
                    //        {
                    //            cmprequest[0].IsLeaveTaken = false;
                    //            myapp.SaveChanges();
                    //        }
                    //    }
                    //}
                    var listmanger = myapp.tbl_ReportingManager.Where(l => l.UserId == cususerid).ToList();
                    if (listmanger.Count > 0)
                    {
                        try
                        {
                            foreach (var v2 in listmanger)
                            {
                                var validate = v2.Is_OnLeave.HasValue ? v2.Is_OnLeave.Value : false;
                                if (validate)
                                {
                                    if (v2.FromDate == tasks[0].LeaveFromDate && v2.ToDate == tasks[0].LeaveTodate)
                                    {
                                        v2.Is_OnLeave = false;
                                        v2.FromDate = null;
                                        v2.ToDate = null;
                                        myapp.SaveChanges();
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            return message;
        }

        [HttpPost]
        [Route("CancelLeave")]
        public string CancelLeave(int LeaveId)
        {
            message = "Success";
            var tasks = myapp.tbl_Leave.Where(t => t.LeaveId == LeaveId).ToList();
            if (tasks.Count > 0)
            {
                if (tasks[0].LeaveStatus != "Cancelled" && tasks[0].LeaveStatus != "Reject" && tasks[0].LeaveStatus != "Approved")
                {
                    tasks[0].LeaveStatus = "Cancelled";
                    string cusUserId = tasks[0].UserId;
                    int Leavetypeid = tasks[0].LeaveTypeId.Value;
                    var list = myapp.tbl_ManageLeave.Where(v => v.UserId == cusUserId && v.LeaveTypeId == Leavetypeid).ToList();
                    if (list.Count > 0)
                    {
                        if (!tasks[0].IsFullday.Value)
                        {
                            list[0].AvailableLeave = list[0].AvailableLeave + 0.5;
                        }
                        else
                        {
                            if (tasks[0].LeaveCount == null || tasks[0].LeaveCount == 0)
                            {
                                tasks[0].LeaveCount = 1;
                            }
                            list[0].AvailableLeave = list[0].AvailableLeave + tasks[0].LeaveCount;
                            myapp.SaveChanges();
                        }
                    }
                    tasks[0].LeaveCount = 0;
                    myapp.SaveChanges();
                    //if (tasks[0].LeaveTypeId == 6)
                    //{
                    //    if (tasks[0].DateofAvailableCompoff != null)
                    //    {
                    //        DateTime Dt = tasks[0].DateofAvailableCompoff.Value.Date;
                    //        string useridcurrr = tasks[0].UserId;
                    //        var cmprequest = myapp.tbl_RequestCompOffLeave.Where(l => l.UserId == useridcurrr).ToList();
                    //        cmprequest = cmprequest.Where(l => l.CompOffDateTime.Value.Date == Dt.Date).ToList();
                    //        if (cmprequest.Count > 0)
                    //        {
                    //            cmprequest[0].IsLeaveTaken = false;
                    //            myapp.SaveChanges();
                    //        }
                    //    }
                    //}
                    var listmanger = myapp.tbl_ReportingManager.Where(l => l.UserId == cusUserId).ToList();
                    if (listmanger.Count > 0)
                    {
                        try
                        {
                            foreach (var v2 in listmanger)
                            {
                                var validate = v2.Is_OnLeave.HasValue ? v2.Is_OnLeave.Value : false;
                                if (validate)
                                {
                                    if (v2.FromDate == tasks[0].LeaveFromDate && v2.ToDate == tasks[0].LeaveTodate)
                                    {
                                        v2.Is_OnLeave = false;
                                        v2.FromDate = null;
                                        v2.ToDate = null;
                                        myapp.SaveChanges();
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    return message = "After Leave Approved or Reject will not allow to cancel";
                }

            }
            return message;
        }

        [HttpPost]
        [Route("CancelCompOffrequest")]
        public string CancelCompOffrequest(int CompOffRequestid)
        {
            var tasks = myapp.tbl_RequestCompOffLeave.Where(t => t.ID == CompOffRequestid).ToList();
            if (tasks.Count > 0)
            {
                string statusbefore = tasks[0].Leave_Status;
                tasks[0].Leave_Status = "Canceled";
                myapp.SaveChanges();
                string userid = tasks[0].UserId;
                var date = tasks[0].CompOffDateTime.Value.Date;
                var roster = myapp.tbl_Roaster.Where(l => l.UserId == userid && l.ShiftDate.Value == date).ToList();
                if (roster.Count > 0)
                {
                    roster[0].ShiftTypeId = 3;
                    roster[0].ShiftTypeName = "Holiday";
                    myapp.SaveChanges();
                }

                if (statusbefore == "Approved")
                {
                    var cmplist = myapp.tbl_ManageLeave.Where(cl => cl.UserId == userid && cl.LeaveTypeId == 6).ToList();
                    if (cmplist.Count > 0)
                    {
                        cmplist[0].AvailableLeave = cmplist[0].AvailableLeave - 1;
                        myapp.SaveChanges();
                    }
                }
            }
            return "Success";
        }
        [HttpPost]
        [Route("AcceptOrRejectCompOffRequest")]
        public string AcceptOrRejectCompOffRequest(int CompOffRequestid, string Status)
        {
            message = "Success";
            string UserId = User.Identity.Name;
            List<tbl_RequestCompOffLeave> ReturnData = myapp.tbl_RequestCompOffLeave.Where(e => e.CompOffLeave_Approver_1 == UserId || e.CompOffLeave_Approver_2 == UserId).ToList();
            if (!String.IsNullOrEmpty(Status) && Information.IsNumeric(CompOffRequestid))
            {
                ReturnData = ReturnData.Where(e => e.ID == CompOffRequestid).ToList();
                if (ReturnData.Count > 0)
                {
                    ReturnData[0].Leave_Status = Status;
                    ReturnData[0].IsApproved_Manager = (Status == "Approved") ? true : false;
                    myapp.SaveChanges();
                    message = "CompOff Leave " + Status + " Successfully";
                    if (Status == "Approved" && ReturnData[0].ShiftTypeId != null && ReturnData[0].ShiftTypeId > 0)
                    {
                        string userid2 = ReturnData[0].UserId;
                        var userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == userid2);
                        var cmplist = myapp.tbl_ManageLeave.Where(cl => cl.UserId == userinfo.CustomUserId && cl.LeaveTypeId == 6).ToList();
                        if (cmplist.Count > 0)
                        {
                            cmplist[0].AvailableLeave = cmplist[0].AvailableLeave + 1;
                            myapp.SaveChanges();
                        }
                        else
                        {
                            tbl_ManageLeave model = new tbl_ManageLeave();
                            model.AvailableLeave = 1;
                            model.CountOfLeave = 1;
                            model.CreatedBy = User.Identity.Name;
                            model.CreatedOn = DateAndTime.Now;
                            model.LeaveTypeId = 6;
                            model.LeaveTypeName = "Comp Off";
                            model.LocationId = userinfo.LocationId;
                            model.LocationName = userinfo.LocationName;
                            model.UserId = userinfo.CustomUserId;
                            model.UserName = userinfo.FirstName;
                            model.ModifiedBy = User.Identity.Name;
                            model.ModifiedOn = DateAndTime.Now;
                            model.IsActive = true;
                            model.DepartmentId = userinfo.DepartmentId;
                            model.DepartmentName = userinfo.DepartmentName;
                            myapp.tbl_ManageLeave.Add(model);
                            myapp.SaveChanges();
                        }
                    }
                    else
                    {
                        string userid = ReturnData[0].UserId;
                        DateTime Dt = ReturnData[0].CompOffDateTime.Value;

                        var uplist = (from l in myapp.tbl_Roaster
                                      where l.UserId == userid && l.ShiftDate.Value == Dt
                                      select l).ToList();
                        if (uplist.Count > 0)
                        {
                            uplist[0].ShiftTypeId = 3;

                            uplist[0].ShiftTypeName = "Holiday";
                            myapp.SaveChanges();
                        }
                    }
                }
            }

            return message;
        }


        [HttpPost]
        [Route("ApprovePermision")]
        public string ApprovePermision(int PermissionId)
        {
            message = "Success";
            var tasks = myapp.tbl_Permission.Where(t => t.PermissionId == PermissionId).ToList();
            if (tasks.Count > 0)
            {
                tasks[0].Status = "Approved";
                if (tasks[0].Level1Approver == User.Identity.Name && tasks[0].Level1Approved == null)
                {
                    tasks[0].Level1Approved = true;
                }

                myapp.SaveChanges();
            }
            return message;
        }
        [HttpPost]
        [Route("RejectPermision")]
        public string RejectPermision(int PermissionId, string comments)
        {
            var tasks = myapp.tbl_Permission.Where(t => t.PermissionId == PermissionId).ToList();
            if (tasks.Count > 0)
            {
                tasks[0].Status = "Reject";
                if (tasks[0].Level1Approver == User.Identity.Name && tasks[0].Level1Approved == null)
                {
                    tasks[0].Level1Approved = false;
                    tasks[0].Level1ApproveComment = comments;
                }

                myapp.SaveChanges();
            }
            return "Success";
        }
        [HttpPost]
        [Route("CancelPermissions")]
        public string CancelPermissions(int PermissionId)
        {
            var tasks = myapp.tbl_Permission.Where(t => t.PermissionId == PermissionId).ToList();
            string msg = "Permission Cancelled Successfully";
            if (tasks.Count > 0)
            {
                //if (tasks[0].Level1Approved == null || tasks[0].Level1Approved == false)
                //{
                tasks[0].Status = "Cancelled";
                myapp.SaveChanges();
                //}
                //else
                //{
                //    msg = "Permission will not Cancelled after approve";
                //}
            }
            return msg;
        }


        [HttpPost]
        [Route("SaveLeaveFromManager")]
        public string HrSaveLeaveManagement(tbl_Leave tbll)
        {
            if (tbll.LeaveFromDate != null && tbll.LeaveTodate != null)
            {
                message = "Success";
                //tbll.UserId = User.Identity.Name;

                GetLeavesCountByValidateShiftTypes(tbll.LeaveFromDate.Value, tbll.LeaveTodate.Value, tbll.UserId, tbll.IsFullday.Value, tbll.IsCompOff.Value);

                if (CurrentLeave > 0)
                {
                    if (validateLeave(tbll) && CheckTheLeaveBalances(tbll.UserId, tbll.LeaveTypeId.Value))
                    {
                        tbll.CreatedBy = User.Identity.Name;
                        tbll.CreatedOn = DateTime.Now;
                        //tbll.LeaveStatus
                        //var rpt = hrdm.GetReportingMgr(User.Identity.Name);
                        var userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == tbll.UserId);
                        if (userinfo != null)
                        {

                            tbll.UserName = userinfo.FirstName + " " + userinfo.LastName;
                            tbll.LocationId = userinfo.LocationId;
                            tbll.LocationName = userinfo.LocationName;
                            tbll.DepartmentId = userinfo.DepartmentId;
                            tbll.DepartmentName = userinfo.DepartmentName;
                            tbll.LeaveStatus = "Approved";
                            tbll.IsActive = true;
                            tbll.LeaveCount = CurrentLeave;


                            if (!string.IsNullOrEmpty(User.Identity.Name))
                            {
                                tbll.Level1Approver = User.Identity.Name;
                                tbll.Level1Approved = true;
                                tbll.Level2Approved = true;
                                tbll.Level2Approver = User.Identity.Name;
                            }
                            //Send Mail
                        }
                        if (tbll.IsCompOff.Value)
                        {
                            var cmplist = myapp.tbl_RequestCompOffLeave.Where(t => t.IsLeaveTaken == false && t.UserId == tbll.UserId && t.Leave_Status == "Approved").OrderBy(t => t.CompOffDateTime).ToList();
                            foreach (var v in cmplist)
                            {
                                DateTime dtval = v.CompOffDateTime.Value.AddDays(90);
                                if (dtval > DateTime.Now)
                                {
                                    if (tbll.IsFullday.Value)
                                    {
                                        if (!v.IsApproved_Manager_4.Value)
                                        {
                                            tbll.DateOfCompOffLeave = v.CompOffDateTime;
                                            v.IsLeaveTaken = true;
                                            myapp.SaveChanges();
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        if (v.LeavesTakenCount > 0)
                                        {
                                            tbll.DateOfCompOffLeave = v.CompOffDateTime;
                                            v.IsLeaveTaken = true;
                                            v.LeavesTakenCount = 1;
                                            myapp.SaveChanges();
                                            break;
                                        }
                                        else
                                        {
                                            tbll.DateOfCompOffLeave = v.CompOffDateTime;
                                            v.LeavesTakenCount = 0.5;
                                            if (v.IsApproved_Manager_4.Value)
                                            {
                                                v.IsLeaveTaken = true;
                                            }
                                            myapp.SaveChanges();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        try
                        {
                            tbll.LeaveTypeName = myapp.tbl_LeaveType.Where(l => l.LeaveTypeId == tbll.LeaveTypeId).SingleOrDefault().LeaveName;
                        }
                        catch { }
                        myapp.tbl_Leave.Add(tbll);
                        myapp.SaveChanges();
                        var leavelist = myapp.tbl_ManageLeave.Where(t => t.UserId == tbll.UserId && t.LeaveTypeId == tbll.LeaveTypeId).ToList();

                        if (leavelist.Count > 0)
                        {
                            if (!tbll.IsFullday.Value)
                            {
                                if (leavelist[0].AvailableLeave != null && leavelist[0].AvailableLeave.Value != 0)
                                {
                                    leavelist[0].AvailableLeave = leavelist[0].AvailableLeave - 0.5;
                                }
                                else
                                {
                                    leavelist[0].AvailableLeave = leavelist[0].CountOfLeave - 0.5;
                                }
                            }
                            else
                            {
                                if (leavelist[0].AvailableLeave != null && leavelist[0].AvailableLeave.Value != 0)
                                {
                                    leavelist[0].AvailableLeave = leavelist[0].AvailableLeave - CurrentLeave;
                                }
                                else
                                {
                                    leavelist[0].AvailableLeave = leavelist[0].CountOfLeave - CurrentLeave;
                                }
                            }
                        }
                        myapp.SaveChanges();

                        message = "Leave successfully applied";
                    }

                }
                else
                {
                    message = "Please check the date";
                }

            }
            else
            {
                message = "Invalid Date";
            }
            return message;
        }

        [HttpPost]
        [Route("SaveCompOffRequestFromManager")]
        public string RequestNewCompOffLeaveByManager(CompOffTypeModel cmpty)
        {

            tbl_RequestCompOffLeave ReqData = new tbl_RequestCompOffLeave();
            ReqData.CompOffDate = cmpty.CompOffDate;
            ReqData.RequestReason = cmpty.RequestReason;
            ReqData.ShiftTypeId = cmpty.ShiftTypeId;
            ReqData.UserId = cmpty.UserId;
            message = "";
            try
            {

                string UserId = cmpty.UserId;
                DateTime CompOffDateTime = ProjectConvert.ConverDateStringtoDatetime(cmpty.CompOffDate);
                var shift = (from v in myapp.tbl_Roaster where v.UserId == UserId select v).ToList();
                var RoasterData = myapp.tbl_Roaster.Where(e => e.UserId == UserId && e.ShiftDate != null && e.ShiftDate == CompOffDateTime).ToList();


                if (RoasterData != null && RoasterData.Count > 0)
                {
                    var countofdate = RoasterData.Where(l => l.ShiftTypeId.Value == 3 || l.ShiftTypeId.Value == 4).ToList();
                    if (countofdate.Count() > 0)
                    {

                        var CompOffLeaveRequestList = myapp.tbl_RequestCompOffLeave.Where(e => e.UserId == UserId && e.Leave_Status != "Rejected" && e.Leave_Status != "Canceled").ToList();
                        if (ReqData != null)
                        {

                            if (CompOffLeaveRequestList.Where(e => e.CompOffDate == ReqData.CompOffDate).Count() == 0)
                            {
                                var rpt = hrdm.GetReportingMgr(UserId, CompOffDateTime, CompOffDateTime);

                                var userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == UserId);
                                if (CompOffDateTime > userinfo.DateOfJoining)
                                {
                                    if (userinfo != null)
                                    {
                                        ReqData.UserName = userinfo.FirstName + " " + userinfo.LastName;
                                        ReqData.LocationId = userinfo.LocationId;
                                        ReqData.LocationName = userinfo.LocationName;
                                        ReqData.DepartmentId = userinfo.DepartmentId;
                                        ReqData.DepartmentName = userinfo.DepartmentName;
                                        if (!string.IsNullOrEmpty(rpt))
                                        {
                                            ReqData.CompOffLeave_Approver_1 = rpt;
                                            ReqData.CompOffLeave_Approver_2 = rpt;
                                        }
                                        ReqData.CompOffDateTime = CompOffDateTime;
                                        ReqData.CreatedBy = UserId;
                                        ReqData.CreatedDate = DateTime.Now.ToString("dd/MM/yyyy");
                                        ReqData.CreatedDateTime = DateTime.Now;
                                        ReqData.ExpiryDate = DateTime.Now.AddDays(30).ToString("dd/MM/yyyy");
                                        ReqData.ExpiryDateTime = DateTime.Now.AddDays(30);
                                        ReqData.IsApproved_Admin = false;
                                        ReqData.IsApproved_Manager = true;
                                        ReqData.IsApproved_Manager_2 = true;
                                        ReqData.IsApproved_Manager_3 = true;
                                        if (cmpty.DayWorkedType != null && cmpty.DayWorkedType == "Half Day")
                                        {
                                            ReqData.IsApproved_Manager_4 = true;
                                        }
                                        else
                                        {
                                            ReqData.IsApproved_Manager_4 = false;
                                        }
                                        ReqData.IsLeaveTaken = false;
                                        ReqData.Leave_Status = "Approved";
                                        ReqData.Record_Status = true;
                                        ReqData.LeavesTakenCount = 0;
                                        ReqData.UserEmailId = userinfo.EmailId;
                                        ReqData.UserId = userinfo.CustomUserId;
                                        myapp.tbl_RequestCompOffLeave.Add(ReqData);
                                        myapp.SaveChanges();
                                        countofdate[0].ShiftTypeId = ReqData.ShiftTypeId;

                                        if (ReqData.RequestReason != null && ReqData.RequestReason != "")
                                        {
                                            countofdate[0].ShiftTypeName = myapp.tbl_ShiftType.Where(l => l.ShiftTypeId == ReqData.ShiftTypeId).SingleOrDefault().ShiftTypeName;
                                        }
                                        myapp.SaveChanges();
                                        message = "Success";
                                    }
                                }
                                else
                                {
                                    message = "A CompOff Leave Should apply after joining date.";
                                }
                            }
                            else
                            {
                                message = "A CompOff Leave Is Already Applied On This Date.";
                            }
                        }
                    }
                    else
                    {
                        message = "CompOff must be apply on either week off or holidays";
                    }
                }
                else
                {
                    message = "Please check duty roster not available for this day";
                }
            }
            catch (Exception EX)
            {
                message = EX.Message;
            }
            return message;
        }
        [HttpGet]
        [Route("CheckCurrentUserIsManager")]
        public bool CheckCurrentUserIsManager()
        {
            if (User.IsInRole("DepartmentManager"))
            {
                return true;
            }
            else
                return false;
        }
    }
}
