using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models.MobileApi;

namespace WebApplicationHsApp.Models
{
    public class HrDataManage
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        JavaScriptSerializer js = new JavaScriptSerializer();
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(HrDataManage));
        public bool CheckTheCasuvalLeaveBeforeDasy(DateTime LeaveFromDate, DateTime LeaveToDate, string Username)
        {
            bool Validleave = true;
            if (LeaveToDate.Date < DateTime.Now.Date)
            {
                TimeSpan difference = DateTime.Now.Date - LeaveToDate.Date;
                var dayscountval = difference.TotalDays;
                if (dayscountval > 2)
                {
                    List<tbl_Roaster> RoasterData = myapp.tbl_Roaster.Where(e => e.UserId == Username && e.ShiftDate != null).ToList();
                    if (RoasterData.Count > 0)
                    {
                        var list = RoasterData.Where(l => l.ShiftDate >= LeaveToDate.Date && l.ShiftDate <= DateTime.Now.Date).ToList();
                        var checksiftleaveorweekoff = list.Where(l => l.ShiftTypeId == 3 || l.ShiftTypeId == 4).Count();
                        dayscountval = dayscountval - checksiftleaveorweekoff;
                        if (dayscountval > 2)
                        {
                            Validleave = false;
                        }

                    }
                }
            }
            else if (LeaveFromDate.Date < DateTime.Now.Date)
            {
                TimeSpan difference = DateTime.Now.Date - LeaveFromDate.Date;
                var dayscountval = difference.TotalDays;
                if (dayscountval > 2)
                {
                    List<tbl_Roaster> RoasterData = myapp.tbl_Roaster.Where(e => e.UserId == Username && e.ShiftDate != null).ToList();
                    if (RoasterData.Count > 0)
                    {
                        var list = RoasterData.Where(l => l.ShiftDate >= LeaveFromDate.Date && l.ShiftDate <= DateTime.Now.Date).ToList();
                        var checksiftleaveorweekoff = list.Where(l => l.ShiftTypeId == 3 || l.ShiftTypeId == 4).Count();
                        dayscountval = dayscountval - checksiftleaveorweekoff;
                        if (dayscountval > 2)
                        {
                            Validleave = false;
                        }

                    }
                }
            }
            return Validleave;
        }
        public string GetReportingMgr(string userid, DateTime st, DateTime ed)
        {
            var rptmgrId = "";
            var userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == userid);
            if (userinfo != null)
            {
                int reportingmangerid = userinfo.ReportingManagerId.HasValue ? userinfo.ReportingManagerId.Value : 0;
                if (reportingmangerid > 0)
                {
                    return reportingmangerid.ToString();
                }
                var listofmanager = myapp.tbl_AssignEmployeesToManager.Where(t => t.EmployeeId == userid).ToList();
                if (listofmanager.Count > 0)
                {
                    rptmgrId = listofmanager[0].ManagerEmployeeId;
                }
                else
                {
                    var locId = userinfo.LocationId;
                    var deptId = userinfo.DepartmentId;
                    var SupDep = userinfo.SubDepartmentId;
                    if (userinfo.UserType == "HOD")
                    {
                        var rptmgr = myapp.tbl_ReportingManager.FirstOrDefault(a => a.LocationId == locId && a.DepartmentId == deptId && a.SubDepartmentId == SupDep && a.IsHodOfHod == true);
                        if (rptmgr == null)
                        {
                            var rptmgrobj = myapp.tbl_ReportingManager.FirstOrDefault(a => a.LocationId == locId && a.DepartmentId == deptId && a.IsHodOfHod == true);
                            if (rptmgrobj != null)
                            {
                                rptmgrId = rptmgrobj.UserId;
                                if (rptmgrobj.Is_OnLeave != null)
                                {
                                    bool rptmgtisonleave = rptmgrobj.Is_OnLeave.HasValue ? rptmgrobj.Is_OnLeave.Value : false;
                                    if (rptmgtisonleave)
                                    {
                                        if (rptmgrobj.FromDate != null && rptmgrobj.ToDate != null && st >= rptmgrobj.FromDate && st <= rptmgrobj.ToDate && ed >= rptmgrobj.FromDate && ed <= rptmgrobj.ToDate)
                                        {
                                            rptmgrId = rptmgrobj.Emp_UserId;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            rptmgrId = rptmgr.UserId;
                            if (rptmgr.Is_OnLeave != null)
                            {
                                bool rptmgtisonleave = rptmgr.Is_OnLeave.HasValue ? rptmgr.Is_OnLeave.Value : false;
                                if (rptmgtisonleave)
                                {
                                    if (rptmgr.FromDate != null && rptmgr.ToDate != null && st >= rptmgr.FromDate && st <= rptmgr.ToDate && ed >= rptmgr.FromDate && ed <= rptmgr.ToDate)
                                    {
                                        rptmgrId = rptmgr.Emp_UserId;
                                    }
                                }
                            }
                        }
                    }
                    else if (userinfo.UserType == "HeadofHOD")
                    {
                        var rptmgr = myapp.tbl_ReportingManager.FirstOrDefault(a => a.LocationId == locId && a.DepartmentId == deptId && a.SubDepartmentId == SupDep && a.IsManagerOfHod == true);
                        if (rptmgr == null)
                        {
                            var rptmgrobj = myapp.tbl_ReportingManager.FirstOrDefault(a => a.LocationId == locId && a.DepartmentId == deptId && a.IsManagerOfHod == true);
                            if (rptmgrobj != null)
                            {
                                rptmgrId = rptmgrobj.UserId;
                                if (rptmgrobj.Is_OnLeave != null)
                                {
                                    bool rptmgtisonleave = rptmgrobj.Is_OnLeave.HasValue ? rptmgrobj.Is_OnLeave.Value : false;
                                    if (rptmgtisonleave)
                                    {
                                        if (rptmgrobj.FromDate != null && rptmgrobj.ToDate != null && st >= rptmgrobj.FromDate && st <= rptmgrobj.ToDate && ed >= rptmgrobj.FromDate && ed <= rptmgrobj.ToDate)
                                        {
                                            rptmgrId = rptmgrobj.Emp_UserId;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            rptmgrId = rptmgr.UserId;
                            if (rptmgr.Is_OnLeave != null)
                            {
                                bool rptmgtisonleave = rptmgr.Is_OnLeave.HasValue ? rptmgr.Is_OnLeave.Value : false;
                                if (rptmgtisonleave)
                                {
                                    if (rptmgr.FromDate != null && rptmgr.ToDate != null && st >= rptmgr.FromDate && st <= rptmgr.ToDate && ed >= rptmgr.FromDate && ed <= rptmgr.ToDate)
                                    {
                                        rptmgrId = rptmgr.Emp_UserId;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        //Check if assign developer existsi

                        //var listofmanager = myapp.tbl_AssignEmployeesToManager.Where(t => t.EmployeeId == userid).ToList();
                        //if (listofmanager.Count > 0)
                        //{
                        //    rptmgrId = listofmanager[0].ManagerEmployeeId;
                        //}
                        //else
                        //{
                        var rptmgr = myapp.tbl_ReportingManager.FirstOrDefault(a => a.LocationId == locId && a.DepartmentId == deptId && a.SubDepartmentId == SupDep && a.IsHod == true);
                        if (rptmgr == null)
                        {
                            var rptmgrobj = myapp.tbl_ReportingManager.FirstOrDefault(a => a.LocationId == locId && a.DepartmentId == deptId && a.IsHod == true);
                            if (rptmgrobj != null)
                            {
                                rptmgrId = rptmgrobj.UserId;
                                if (rptmgrobj.Is_OnLeave != null)
                                {
                                    bool rptmgtisonleave = rptmgrobj.Is_OnLeave.HasValue ? rptmgrobj.Is_OnLeave.Value : false;
                                    if (rptmgtisonleave)
                                    {
                                        if (rptmgrobj.FromDate != null && rptmgrobj.ToDate != null && st >= rptmgrobj.FromDate && st <= rptmgrobj.ToDate && ed >= rptmgrobj.FromDate && ed <= rptmgrobj.ToDate)
                                        {
                                            rptmgrId = rptmgrobj.Emp_UserId;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            rptmgrId = rptmgr.UserId;
                            bool rptmgtisonleave = rptmgr.Is_OnLeave.HasValue ? rptmgr.Is_OnLeave.Value : false;
                            if (rptmgtisonleave)
                            {
                                if (rptmgr.FromDate != null && rptmgr.ToDate != null && st >= rptmgr.FromDate && st <= rptmgr.ToDate && ed >= rptmgr.FromDate && ed <= rptmgr.ToDate)
                                {
                                    rptmgrId = rptmgr.Emp_UserId;
                                }
                            }
                        }

                    }
                }

                //}
            }
            return rptmgrId;
        }
        public bool CheckApplyLeaveWithPreviousHalfAppliedLeaveData_HRController(DateTime LeaveFromDate, DateTime LeaveToDate, string username, int leavetypeid, string session)
        {

            bool ReturnStatus = true;
            try
            {

                var PrevLeaveList = myapp.tbl_Leave.Where(e => e.UserId == username && e.IsCompOff == false && e.LeaveStatus != "Cancelled" && e.LeaveStatus != "Reject" && e.LeaveTypeId != leavetypeid && e.LeaveSessionDay != session).ToList();
                //Check Before day
                DateTime TempAppliedLeaveFrom = LeaveFromDate.AddDays(-1);
                var result = PrevLeaveList.Where(l => TempAppliedLeaveFrom >= l.LeaveFromDate && TempAppliedLeaveFrom <= l.LeaveTodate).ToList();
                if (result.Count > 0)
                {
                    var result1 = result.Where(l => TempAppliedLeaveFrom >= l.LeaveFromDate && TempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                    if (result1.Count > 0)
                    {
                        if (session != null && session == "2nd Half")
                        {
                            ReturnStatus = true;
                        }
                        else
                        {
                            ReturnStatus = false;
                        }
                    }
                    var result11 = result.Where(l => TempAppliedLeaveFrom >= l.LeaveFromDate && TempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay == "2nd Half").ToList();
                    if (result11.Count > 0)
                    {
                        ReturnStatus = false;
                    }
                }
                //Next day
                DateTime NTempAppliedLeaveFrom = LeaveFromDate.AddDays(1);
                var resultn = PrevLeaveList.Where(l => NTempAppliedLeaveFrom >= l.LeaveFromDate && NTempAppliedLeaveFrom <= l.LeaveTodate).ToList();
                if (resultn.Count > 0)
                {
                    var resultn1 = resultn.Where(l => TempAppliedLeaveFrom >= l.LeaveFromDate && TempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                    if (resultn1.Count > 0)
                    {
                        if (session != null && session == "1st Half")
                        {
                            ReturnStatus = true;
                        }
                        else
                        {
                            ReturnStatus = false;
                        }
                    }
                    var resultn11 = resultn.Where(l => TempAppliedLeaveFrom >= l.LeaveFromDate && TempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay == "1st Half").ToList();
                    if (resultn11.Count > 0)
                    {
                        ReturnStatus = false;
                    }
                }

                DateTime TempAppliedLeaveto = LeaveToDate.AddDays(-1);
                var result2 = PrevLeaveList.Where(l => TempAppliedLeaveto >= l.LeaveFromDate && TempAppliedLeaveto <= l.LeaveTodate).ToList();
                if (result2.Count > 0)
                {
                    //ReturnStatus = false;
                    var result21 = result2.Where(l => TempAppliedLeaveFrom >= l.LeaveFromDate && TempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                    if (result21.Count > 0)
                    {
                        if (session != null && session == "2nd Half")
                        {
                            ReturnStatus = true;
                        }
                        else
                        {
                            ReturnStatus = false;
                        }
                    }
                    var result211 = result2.Where(l => TempAppliedLeaveFrom >= l.LeaveFromDate && TempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay == "2nd Half").ToList();
                    if (result211.Count > 0)
                    {
                        ReturnStatus = false;
                    }
                }
                //Next day
                DateTime NTempAppliedLeaveto = LeaveToDate.AddDays(1);
                var resultn2 = PrevLeaveList.Where(l => NTempAppliedLeaveto >= l.LeaveFromDate && NTempAppliedLeaveto <= l.LeaveTodate).ToList();
                if (resultn2.Count > 0)
                {
                    //ReturnStatus = false;
                    var resultn21 = resultn2.Where(l => TempAppliedLeaveFrom >= l.LeaveFromDate && TempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                    if (resultn21.Count > 0)
                    {
                        if (session != null && session == "1st Half")
                        {
                            ReturnStatus = true;
                        }
                        else
                        {
                            ReturnStatus = false;
                        }
                    }
                    var resultn211 = resultn2.Where(l => TempAppliedLeaveFrom >= l.LeaveFromDate && TempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay == "1st Half").ToList();
                    if (resultn211.Count > 0)
                    {
                        ReturnStatus = false;
                    }
                }
                //Check Holidays and then before days
                if (ReturnStatus)
                {
                    DateTime CheckDutyroasterfrom = LeaveFromDate.AddDays(-3);
                    DateTime CheckDutyroasterto = LeaveToDate.AddDays(3);
                    var listofdutyrster = myapp.tbl_Roaster.Where(l => l.UserId == username && l.ShiftDate != null && l.ShiftDate > CheckDutyroasterfrom && l.ShiftDate <= CheckDutyroasterto && (l.ShiftTypeId == 3 || l.ShiftTypeId == 4)).ToList();

                    //check 1day previous
                    DateTime checkfrmonedayp = LeaveFromDate.AddDays(-1);
                    var checkcount = listofdutyrster.Where(l => l.ShiftDate == checkfrmonedayp).ToList();
                    if (checkcount.Count > 0)
                    {
                        checkfrmonedayp = checkfrmonedayp.AddDays(-1);
                        var checkdudtyroasterlist = PrevLeaveList.Where(l => checkfrmonedayp >= l.LeaveFromDate && checkfrmonedayp <= l.LeaveTodate).Count();
                        if (checkdudtyroasterlist > 0)
                        {
                            //ReturnStatus = false;

                            var checkdudtyroasterlist1 = PrevLeaveList.Where(l => checkfrmonedayp >= l.LeaveFromDate && checkfrmonedayp <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                            if (checkdudtyroasterlist1.Count > 0)
                            {
                                if (session != null && session == "2nd Half")
                                {
                                    ReturnStatus = true;
                                }
                                else
                                {
                                    ReturnStatus = false;
                                }
                            }
                            var checkdudtyroasterlist11 = PrevLeaveList.Where(l => checkfrmonedayp >= l.LeaveFromDate && checkfrmonedayp <= l.LeaveTodate && l.LeaveSessionDay == "1st Half").ToList();
                            if (checkdudtyroasterlist11.Count > 0)
                            {
                                if (session != null && session == "2nd Half")
                                {
                                    ReturnStatus = true;
                                }
                                else
                                {
                                    ReturnStatus = false;
                                }
                            }
                        }
                    }
                    //check 2day previous
                    DateTime checkfrmtwodayp = LeaveFromDate.AddDays(-2);
                    var checkcount1 = listofdutyrster.Where(l => l.ShiftDate == checkfrmtwodayp).ToList();
                    if (checkcount1.Count > 0)
                    {
                        checkfrmtwodayp = checkfrmtwodayp.AddDays(-1);
                        var checkdudtyroasterlist = PrevLeaveList.Where(l => checkfrmtwodayp >= l.LeaveFromDate && checkfrmtwodayp <= l.LeaveTodate).Count();
                        if (checkdudtyroasterlist > 0)
                        {
                            //ReturnStatus = false;
                            var checkdudtyroasterlist1 = PrevLeaveList.Where(l => checkfrmtwodayp >= l.LeaveFromDate && checkfrmtwodayp <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                            if (checkdudtyroasterlist1.Count > 0)
                            {
                                if (session != null && session == "2nd Half")
                                {
                                    ReturnStatus = true;
                                }
                                else
                                {
                                    ReturnStatus = false;
                                }
                            }
                            var checkdudtyroasterlist11 = PrevLeaveList.Where(l => checkfrmtwodayp >= l.LeaveFromDate && checkfrmtwodayp <= l.LeaveTodate && l.LeaveSessionDay == "1st Half").ToList();
                            if (checkdudtyroasterlist11.Count > 0)
                            {
                                if (session != null && session == "2nd Half")
                                {
                                    ReturnStatus = true;
                                }
                                else
                                {
                                    ReturnStatus = false;
                                }
                            }
                        }
                    }

                    //check 1day Next
                    DateTime checkfrmonedayN = LeaveFromDate.AddDays(1);
                    var checkcountN = listofdutyrster.Where(l => l.ShiftDate == checkfrmonedayN).ToList();
                    if (checkcountN.Count > 0)
                    {
                        checkfrmonedayN = checkfrmonedayN.AddDays(1);
                        var checkdudtyroasterlist = PrevLeaveList.Where(l => checkfrmonedayN >= l.LeaveFromDate && checkfrmonedayN <= l.LeaveTodate).Count();
                        if (checkdudtyroasterlist > 0)
                        {
                            //ReturnStatus = false;
                            var checkdudtyroasterlist1 = PrevLeaveList.Where(l => checkfrmonedayN >= l.LeaveFromDate && checkfrmonedayN <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                            if (checkdudtyroasterlist1.Count > 0)
                            {
                                if (session != null && session == "2nd Half")
                                {
                                    ReturnStatus = true;
                                }
                                else
                                {
                                    ReturnStatus = false;
                                }
                            }
                            var checkdudtyroasterlist11 = PrevLeaveList.Where(l => checkfrmonedayN >= l.LeaveFromDate && checkfrmonedayN <= l.LeaveTodate && l.LeaveSessionDay == "1st Half").ToList();
                            if (checkdudtyroasterlist11.Count > 0)
                            {
                                if (session != null && session == "2nd Half")
                                {
                                    ReturnStatus = true;
                                }
                                else
                                {
                                    ReturnStatus = false;
                                }
                            }
                        }
                    }
                    //check 2day Next
                    DateTime checkfrmtwodayn = LeaveFromDate.AddDays(2);
                    var checkcount12 = listofdutyrster.Where(l => l.ShiftDate == checkfrmtwodayn).ToList();
                    if (checkcount12.Count > 0)
                    {
                        checkfrmtwodayn = checkfrmtwodayn.AddDays(1);
                        var checkdudtyroasterlist = PrevLeaveList.Where(l => checkfrmtwodayn >= l.LeaveFromDate && checkfrmtwodayn <= l.LeaveTodate).Count();
                        if (checkdudtyroasterlist > 0)
                        {
                            //ReturnStatus = false;
                            var checkdudtyroasterlist1 = PrevLeaveList.Where(l => checkfrmtwodayn >= l.LeaveFromDate && checkfrmtwodayn <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                            if (checkdudtyroasterlist1.Count > 0)
                            {
                                if (session != null && session == "2nd Half")
                                {
                                    ReturnStatus = true;
                                }
                                else
                                {
                                    ReturnStatus = false;
                                }
                            }
                            var checkdudtyroasterlist11 = PrevLeaveList.Where(l => checkfrmtwodayn >= l.LeaveFromDate && checkfrmtwodayn <= l.LeaveTodate && l.LeaveSessionDay == "1st Half").ToList();
                            if (checkdudtyroasterlist11.Count > 0)
                            {
                                if (session != null && session == "2nd Half")
                                {
                                    ReturnStatus = true;
                                }
                                else
                                {
                                    ReturnStatus = false;
                                }
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ReturnStatus = false;
            }
            return ReturnStatus;
        }
        public bool CheckApplyLeaveWithPreviousAppliedLeaveData_HRControllerCompoff(DateTime LeaveFromDate, DateTime LeaveToDate, string username, int leavetypeid)
        {

            bool ReturnStatus = true;
            try
            {

                var PrevLeaveList = myapp.tbl_Leave.Where(e => e.UserId == username && e.LeaveTypeId != 1009 && e.IsCompOff == false && e.LeaveStatus != "Cancelled" && e.LeaveStatus != "Reject" && e.LeaveTypeId == leavetypeid).ToList();
                //Check Before day
                DateTime TempAppliedLeaveFrom = LeaveFromDate.AddDays(-1);

                var result1 = PrevLeaveList.Where(l => TempAppliedLeaveFrom >= l.LeaveFromDate && TempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                if (result1.Count > 0)
                {
                    ReturnStatus = false;
                }

                var result = PrevLeaveList.Where(l => TempAppliedLeaveFrom >= l.LeaveFromDate && TempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay != "1st Half").Count();
                if (result > 0)
                {
                    ReturnStatus = false;
                }
                //Next day
                DateTime NTempAppliedLeaveFrom = LeaveFromDate.AddDays(1);
                var resultn = PrevLeaveList.Where(l => NTempAppliedLeaveFrom >= l.LeaveFromDate && NTempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay != "2nd Half").Count();
                if (resultn > 0)
                {
                    ReturnStatus = false;
                }
                var resultn1 = PrevLeaveList.Where(l => NTempAppliedLeaveFrom >= l.LeaveFromDate && NTempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                if (resultn1.Count > 0)
                {
                    ReturnStatus = false;
                }
                DateTime TempAppliedLeaveto = LeaveToDate.AddDays(-1);
                var result2 = PrevLeaveList.Where(l => TempAppliedLeaveto >= l.LeaveFromDate && TempAppliedLeaveto <= l.LeaveTodate && l.LeaveSessionDay != "1st Half").Count();
                if (result2 > 0)
                {
                    ReturnStatus = false;
                }
                var result21 = PrevLeaveList.Where(l => TempAppliedLeaveto >= l.LeaveFromDate && TempAppliedLeaveto <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                if (result21.Count > 0)
                {
                    ReturnStatus = false;
                }
                //Next day
                DateTime NTempAppliedLeaveto = LeaveToDate.AddDays(1);
                var resultn2 = PrevLeaveList.Where(l => NTempAppliedLeaveto >= l.LeaveFromDate && NTempAppliedLeaveto <= l.LeaveTodate && l.LeaveSessionDay != "2nd Half").Count();
                if (resultn2 > 0)
                {
                    ReturnStatus = false;
                }
                var resultn21 = PrevLeaveList.Where(l => NTempAppliedLeaveto >= l.LeaveFromDate && NTempAppliedLeaveto <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                if (resultn21.Count > 0)
                {
                    ReturnStatus = false;
                }
                //Check Holidays and then before days
                if (ReturnStatus)
                {
                    DateTime CheckDutyroasterfrom = LeaveFromDate.AddDays(-3);
                    DateTime CheckDutyroasterto = LeaveToDate.AddDays(3);
                    var listofdutyrster = myapp.tbl_Roaster.Where(l => l.UserId == username && l.ShiftDate != null && l.ShiftDate > CheckDutyroasterfrom && l.ShiftDate <= CheckDutyroasterto && (l.ShiftTypeId == 3 || l.ShiftTypeId == 4)).ToList();

                    //check 1day previous
                    DateTime checkfrmonedayp = LeaveFromDate.AddDays(-1);
                    var checkcount = listofdutyrster.Where(l => l.ShiftDate == checkfrmonedayp).ToList();
                    if (checkcount.Count > 0)
                    {
                        checkfrmonedayp = checkfrmonedayp.AddDays(-1);
                        var checkdudtyroasterlist = PrevLeaveList.Where(l => checkfrmonedayp >= l.LeaveFromDate && checkfrmonedayp <= l.LeaveTodate && l.LeaveSessionDay != "1st Half").Count();
                        if (checkdudtyroasterlist > 0)
                        {
                            ReturnStatus = false;
                        }
                    }
                    //check 2day previous
                    DateTime checkfrmtwodayp = LeaveFromDate.AddDays(-2);
                    var checkcount1 = listofdutyrster.Where(l => l.ShiftDate == checkfrmtwodayp).ToList();
                    if (checkcount1.Count > 0)
                    {
                        checkfrmtwodayp = checkfrmtwodayp.AddDays(-1);
                        var checkdudtyroasterlist = PrevLeaveList.Where(l => checkfrmtwodayp >= l.LeaveFromDate && checkfrmtwodayp <= l.LeaveTodate && l.LeaveSessionDay != "1st Half").Count();
                        if (checkdudtyroasterlist > 0)
                        {
                            ReturnStatus = false;
                        }
                    }

                    //check 1day Next
                    DateTime checkfrmonedayN = LeaveFromDate.AddDays(1);
                    var checkcountN = listofdutyrster.Where(l => l.ShiftDate == checkfrmonedayN).ToList();
                    if (checkcountN.Count > 0)
                    {
                        checkfrmonedayN = checkfrmonedayN.AddDays(1);
                        var checkdudtyroasterlist = PrevLeaveList.Where(l => checkfrmonedayN >= l.LeaveFromDate && checkfrmonedayN <= l.LeaveTodate && l.LeaveSessionDay != "2nd Half").Count();
                        if (checkdudtyroasterlist > 0)
                        {
                            ReturnStatus = false;
                        }
                    }
                    //check 2day Next
                    DateTime checkfrmtwodayn = LeaveFromDate.AddDays(2);
                    var checkcount12 = listofdutyrster.Where(l => l.ShiftDate == checkfrmtwodayn).ToList();
                    if (checkcount12.Count > 0)
                    {
                        checkfrmtwodayn = checkfrmtwodayn.AddDays(1);
                        var checkdudtyroasterlist = PrevLeaveList.Where(l => checkfrmtwodayn >= l.LeaveFromDate && checkfrmtwodayn <= l.LeaveTodate && l.LeaveSessionDay != "2nd Half").Count();
                        if (checkdudtyroasterlist > 0)
                        {
                            ReturnStatus = false;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ReturnStatus = false;
            }
            return ReturnStatus;
        }
        public bool CheckApplyLeaveWithPreviousAppliedLeaveData_HRController(DateTime LeaveFromDate, DateTime LeaveToDate, string username, int leavetypeid)
        {

            bool ReturnStatus = true;
            try
            {
                List<tbl_Leave> PrevLeaveList = new List<tbl_Leave>();

                switch (leavetypeid)
                {
                    case 1:
                        PrevLeaveList = myapp.tbl_Leave.Where(e => e.UserId == username && e.LeaveTypeId != 1009 && e.IsCompOff == false && e.LeaveStatus != "Cancelled" && e.LeaveStatus != "Reject" && e.LeaveTypeId != leavetypeid).ToList();
                        break;
                    case 4:
                    case 5:
                    case 8:
                    case 7:
                    case 1009:
                        PrevLeaveList = myapp.tbl_Leave.Where(e => e.UserId == username && e.LeaveStatus != "Cancelled" && e.LeaveStatus != "Reject" && e.LeaveTypeId != leavetypeid && e.LeaveTypeId != 6 && e.LeaveTypeId != 1009).ToList();
                        break;
                }

                //Check Before day
                DateTime TempAppliedLeaveFrom = LeaveFromDate.AddDays(-1);

                var result1 = PrevLeaveList.Where(l => TempAppliedLeaveFrom >= l.LeaveFromDate && TempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                if (result1.Count > 0)
                {
                    ReturnStatus = false;
                }

                var result = PrevLeaveList.Where(l => TempAppliedLeaveFrom >= l.LeaveFromDate && TempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay != "1st Half").Count();
                if (result > 0)
                {
                    ReturnStatus = false;
                }
                //Next day
                DateTime NTempAppliedLeaveFrom = LeaveFromDate.AddDays(1);
                var resultn = PrevLeaveList.Where(l => NTempAppliedLeaveFrom >= l.LeaveFromDate && NTempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay != "2nd Half").Count();
                if (resultn > 0)
                {
                    ReturnStatus = false;
                }
                var resultn1 = PrevLeaveList.Where(l => NTempAppliedLeaveFrom >= l.LeaveFromDate && NTempAppliedLeaveFrom <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                if (resultn1.Count > 0)
                {
                    ReturnStatus = false;
                }
                DateTime TempAppliedLeaveto = LeaveToDate.AddDays(-1);
                var result2 = PrevLeaveList.Where(l => TempAppliedLeaveto >= l.LeaveFromDate && TempAppliedLeaveto <= l.LeaveTodate && l.LeaveSessionDay != "1st Half").Count();
                if (result2 > 0)
                {
                    ReturnStatus = false;
                }
                var result21 = PrevLeaveList.Where(l => TempAppliedLeaveto >= l.LeaveFromDate && TempAppliedLeaveto <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                if (result21.Count > 0)
                {
                    ReturnStatus = false;
                }
                //Next day
                DateTime NTempAppliedLeaveto = LeaveToDate.AddDays(1);
                var resultn2 = PrevLeaveList.Where(l => NTempAppliedLeaveto >= l.LeaveFromDate && NTempAppliedLeaveto <= l.LeaveTodate && l.LeaveSessionDay != "2nd Half").Count();
                if (resultn2 > 0)
                {
                    ReturnStatus = false;
                }
                var resultn21 = PrevLeaveList.Where(l => NTempAppliedLeaveto >= l.LeaveFromDate && NTempAppliedLeaveto <= l.LeaveTodate && l.LeaveSessionDay == null).ToList();
                if (resultn21.Count > 0)
                {
                    ReturnStatus = false;
                }
                //Check Holidays and then before days
                if (ReturnStatus)
                {
                    DateTime CheckDutyroasterfrom = LeaveFromDate.AddDays(-2);
                    DateTime CheckDutyroasterto = LeaveToDate.AddDays(2);
                    var listofdutyrster = myapp.tbl_Roaster.Where(l => l.UserId == username && l.ShiftDate != null && l.ShiftDate > CheckDutyroasterfrom && l.ShiftDate <= CheckDutyroasterto && (l.ShiftTypeId == 3 || l.ShiftTypeId == 4)).ToList();

                    //check 1day previous
                    DateTime checkfrmonedayp = LeaveFromDate.AddDays(-1);
                    var checkcount = listofdutyrster.Where(l => l.ShiftDate == checkfrmonedayp).ToList();
                    if (checkcount.Count > 0)
                    {
                        checkfrmonedayp = checkfrmonedayp.AddDays(-1);
                        var checkdudtyroasterlist = PrevLeaveList.Where(l => checkfrmonedayp >= l.LeaveFromDate && checkfrmonedayp <= l.LeaveTodate && l.LeaveSessionDay != "1st Half").Count();
                        if (checkdudtyroasterlist > 0)
                        {
                            ReturnStatus = false;
                        }
                    }
                    //check 2day previous
                    //DateTime checkfrmtwodayp = LeaveFromDate.AddDays(-2);
                    //var checkcount1 = listofdutyrster.Where(l => l.ShiftDate == checkfrmtwodayp).ToList();
                    //if (checkcount1.Count > 0)
                    //{
                    //    checkfrmtwodayp = checkfrmtwodayp.AddDays(-1);
                    //    var checkdudtyroasterlist = PrevLeaveList.Where(l => checkfrmtwodayp >= l.LeaveFromDate && checkfrmtwodayp <= l.LeaveTodate && l.LeaveSessionDay != "1st Half").Count();
                    //    if (checkdudtyroasterlist > 0)
                    //    {
                    //        ReturnStatus = false;
                    //    }
                    //}

                    //check 1day Next
                    DateTime checkfrmonedayN = LeaveFromDate.AddDays(1);
                    var checkcountN = listofdutyrster.Where(l => l.ShiftDate == checkfrmonedayN).ToList();
                    if (checkcountN.Count > 0)
                    {
                        checkfrmonedayN = checkfrmonedayN.AddDays(1);
                        var checkdudtyroasterlist = PrevLeaveList.Where(l => checkfrmonedayN >= l.LeaveFromDate && checkfrmonedayN <= l.LeaveTodate && l.LeaveSessionDay != "2nd Half").Count();
                        if (checkdudtyroasterlist > 0)
                        {
                            ReturnStatus = false;
                        }
                    }
                    //check 2day Next
                    //DateTime checkfrmtwodayn = LeaveFromDate.AddDays(2);
                    //var checkcount12 = listofdutyrster.Where(l => l.ShiftDate == checkfrmtwodayn).ToList();
                    //if (checkcount12.Count > 0)
                    //{
                    //    checkfrmtwodayn = checkfrmtwodayn.AddDays(1);
                    //    var checkdudtyroasterlist = PrevLeaveList.Where(l => checkfrmtwodayn >= l.LeaveFromDate && checkfrmtwodayn <= l.LeaveTodate && l.LeaveSessionDay != "2nd Half").Count();
                    //    if (checkdudtyroasterlist > 0)
                    //    {
                    //        ReturnStatus = false;
                    //    }
                    //}

                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                ReturnStatus = false;
            }
            return ReturnStatus;
        }
        public LeaveViewModels ViewLeave(int id)
        {
            var leaves = myapp.tbl_Leave.Where(l => l.LeaveId == id).ToList();
            var model =
                    (from c in leaves
                     join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
                     join app2 in myapp.tbl_User on c.Level2Approver equals app2.CustomUserId
                     //where c.LeaveId == id
                     select new LeaveViewModels
                     {
                         LeaveId = c.LeaveId.ToString(),
                         LeaveTypeName = c.LeaveTypeName,
                         //IsFullday = c.IsFullday.ToString(),
                         //IsCompOff = c.IsCompOff.ToString(),
                         LeaveFromDate = c.LeaveFromDate.Value.ToString("dd/MM/yyyy"),
                         LeaveTodate = c.LeaveTodate.Value.ToString("dd/MM/yyyy"),

                         //leavefdate = c.LeaveFromDate.HasValue ? ProjectConvert.ConverDateTimeToString(c.LeaveFromDate.Value) : "",
                         // LeaveTdate = c.LeaveFromDate.HasValue ? ProjectConvert.ConverDateTimeToString(c.LeaveTodate.Value) : "",
                         LeaveStatus = c.LeaveStatus,
                         Level1Approver = app1.FirstName + " " + app1.LastName,
                         //Level2Approver = app2.FirstName + " " + app2.LastName,
                         ReasonForLeave = c.ReasonForLeave,
                         AddressOnLeave = c.AddressOnLeave,
                         //DateofAvailableCompoff = c.DateofAvailableCompoff,
                         // Level1ApproveComment = c.Level1ApproveComment,
                         // Level2ApproveComment = c.Level2ApproveComment,
                         // Level1Approved = c.Level1Approved.ToString(),
                         // Level2Approved = c.Level2Approved.ToString()
                     }).FirstOrDefault();
            return model;
        }

        public string SaveLeavePermission(string date, string starttime, string endtime, string reason, string approval, string currentuser)
        {
            string msg = "Success";
            try
            {
                tbl_Permission ps = new tbl_Permission();
                ps.PermissionDate = Convert.ToDateTime(date);
                DateTime dt = Convert.ToDateTime(date + " " + starttime);
                ps.StartDate = dt;
                DateTime edt = Convert.ToDateTime(date + " " + endtime);
                if (dt < edt)
                {
                    ps.EndDate = edt;
                    ps.Reason = reason;
                    ps.CreatedBy = currentuser;
                    ps.CreatedOn = DateTime.Now;
                    TimeSpan diff = edt - dt;
                    double hours = diff.TotalHours;
                    if (hours > 0)
                    {
                        if (hours <= 2)
                        {                //validate this month hours
                            DateTime now = DateTime.Now;
                            var monthstartDate = new DateTime(ps.PermissionDate.Value.Year, ps.PermissionDate.Value.Month, 1);
                            var monthendDate = monthstartDate.AddMonths(1).AddDays(-1);
                            // check persmmiion 

                            var list = myapp.tbl_Permission.Where(u => u.UserId == currentuser && u.Status != "Cancelled" &&
                            u.PermissionDate == ps.PermissionDate &&
                            (u.StartDate >= ps.StartDate || ps.StartDate < u.EndDate)
                            && ps.EndDate <= u.EndDate).ToList();
                            if (list.Count == 0)
                            {
                                var listofper = myapp.tbl_Permission.Where(u => u.UserId == currentuser && u.PermissionDate >= monthstartDate && u.Status != "Cancelled" && u.PermissionDate <= monthendDate).ToList();
                                double totalhours1 = 0;
                                foreach (var v in listofper)
                                {
                                    TimeSpan diff1 = v.EndDate.Value - v.StartDate.Value;
                                    totalhours1 = totalhours1 + diff1.TotalHours;
                                }
                                totalhours1 = totalhours1 + hours;
                                if (totalhours1 <= 2)
                                {
                                    var st = GetReportingMgr(currentuser, DateTime.Now, DateTime.Now);

                                    if (!string.IsNullOrEmpty(st))
                                    {
                                        ps.Level1Approver = st;
                                        var userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == currentuser);
                                        if (userinfo != null)
                                        {
                                            ps.UserId = userinfo.CustomUserId;
                                            ps.UserName = userinfo.FirstName + " " + userinfo.LastName;
                                            ps.LocationId = userinfo.LocationId;
                                            ps.LocationName = userinfo.LocationName;
                                            ps.DepartmentId = userinfo.DepartmentId;
                                            ps.DepartmentName = userinfo.DepartmentName;
                                            ps.Status = "Pending";
                                        }
                                        myapp.tbl_Permission.Add(ps);
                                        myapp.SaveChanges();
                                        NotificationSendModel notify = new NotificationSendModel();
                                        var devideidstosend = (
                                                               from d in myapp.tbl_DeviceInfo
                                                               where st == d.UserId
                                                               select new NotificationUserModel
                                                               {
                                                                   DeviceId = d.DeviceId,
                                                                   Userid = d.UserId
                                                               }).Distinct().ToList();
                                        var response = notify.SendNotificationToSome("ApprovePermissionPage", "Permission Approve Request from " + ps.UserName, ps.UserId, "Approve Permission", devideidstosend);
                                    }
                                    else
                                    {
                                        msg = "The reporting manager was not set please check";
                                    }
                                }
                                else
                                {
                                    msg = "Maximum hours should be 2 hours per month";
                                }
                            }
                            else
                            {
                                msg = "Permission applying on same time again";
                            }
                        }
                        else
                        {
                            msg = "Maximum hours should be 2 hours per month";
                        }
                    }
                    else
                    {
                        msg = "Minimum hours should be 0.5";
                    }
                }
                else
                {
                    msg = "Please check the end time should be grater than start time";
                }
            }
            catch (Exception ex)
            {
                msg = "Error please try again " + ex.Message;
            }
            return msg;
        }
        public PermissionViewModels ViewPermision(int id)
        {
            var permission = myapp.tbl_Permission.Where(l => l.PermissionId == id).ToList();
            var model =
                   (from c in permission
                    join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId

                    where c.PermissionId == id
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
                        Comments = c.Level1ApproveComment,
                        UserName = c.UserName
                    }).FirstOrDefault();
            return model;
        }

        public bool IsHOlidayornotCheck(DateTime dt)
        {
            var holidaylst = myapp.tbl_Holiday.Where(l => l.IsActive == true).ToList();
            var countlist = holidaylst.Where(l => l.HolidayDate.Value.Date == dt.Date).ToList();
            if (countlist.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public string SaveEmployeesShiftdata(string data, string fromdate, string todate, string currentuser)
        {
            string msg = "Success";
            //List<tbl_Roaster> listdata = js.Deserialize<List<tbl_Roaster>>(data);
            //foreach (var es in listdata)
            //{
            //    myapp.uspupdateshift(es.ShiftTypeId, es.ShiftDate, es.UserId, currentuser);
            //}
            List<tbl_Roaster> listdata = js.Deserialize<List<tbl_Roaster>>(data);
            //CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(fromdate);
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(todate);
            var eshiftslist = (from e in myapp.tbl_Roaster
                               where e.ShiftDate.Value >= dtfrom && e.ShiftDate.Value <= dtto && e.ShiftDate.Value >= dtfrom && e.ShiftDate <= dtto
                               select e).ToList();
            foreach (var es in listdata)
            {
                var uplist = (from l in eshiftslist
                              where l.UserId == es.UserId && l.ShiftDate.Value.ToShortDateString() == es.ShiftDate.Value.ToShortDateString()
                              select l).ToList();
                if (uplist.Count > 0)
                {
                    if (uplist[0].ShiftTypeId != 3)
                    {
                        uplist[0].UserId = es.UserId;
                        uplist[0].ShiftTypeId = es.ShiftTypeId;
                        uplist[0].ShiftDate = es.ShiftDate;
                        uplist[0].ShiftTypeName = es.ShiftTypeName;
                        //if (IsHOlidayornotCheck(es.ShiftDate.Value))
                        //{
                        //    if (es.ShiftTypeName != "Holiday")
                        //    {
                        uplist[0].ShiftTypeName = es.ShiftTypeName;
                        //    }
                        //}
                        myapp.SaveChanges();
                    }
                    else
                    {
                        if (!IsHOlidayornotCheck(es.ShiftDate.Value))
                        {
                            uplist[0].UserId = es.UserId;
                            uplist[0].ShiftTypeId = es.ShiftTypeId;
                            uplist[0].ShiftDate = es.ShiftDate;
                            uplist[0].ShiftTypeName = es.ShiftTypeName;
                            myapp.SaveChanges();
                        }
                    }
                }
                else
                {
                    if (es.ShiftTypeId == 3)
                    {
                        var countescheck = myapp.tbl_Roaster.Where(l => l.UserId == es.UserId && l.ShiftTypeId == 3 && l.ShiftDate.Value.Year == DateTime.Now.Year).Count();

                        if (countescheck <= 8)
                        {
                            var tb = new tbl_Roaster();
                            tb.UserId = es.UserId;
                            tb.ShiftTypeId = es.ShiftTypeId;
                            tb.ShiftDate = es.ShiftDate;
                            tb.CreatedBy = currentuser;
                            var userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == es.UserId);
                            tb.CreatedOn = DateTime.Now;
                            tb.DepartmentId = userinfo.DepartmentId;
                            tb.LocationId = userinfo.LocationId;
                            tb.UserName = userinfo.FirstName;
                            tb.IsActive = true;
                            var shift = myapp.tbl_ShiftType.FirstOrDefault(a => a.ShiftTypeId == es.ShiftTypeId);
                            tb.ShiftTypeName = shift.ShiftTypeName;
                            tb.ShiftStartTime = shift.ShiftStartTime;
                            tb.ShiftEndTime = shift.ShiftEndTime;
                            myapp.tbl_Roaster.Add(tb);
                            myapp.SaveChanges();
                        }
                        else
                        {
                            msg = "Please check your duty roster Leave Shift type will not allow more than 4 in a year";
                        }
                    }
                    else
                    {
                        var tb = new tbl_Roaster();
                        tb.UserId = es.UserId;
                        tb.ShiftTypeId = es.ShiftTypeId;
                        tb.ShiftDate = es.ShiftDate;
                        tb.CreatedBy = currentuser;
                        var userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == es.UserId);
                        tb.CreatedOn = DateTime.Now;
                        tb.DepartmentId = userinfo.DepartmentId;
                        tb.LocationId = userinfo.LocationId;
                        tb.UserName = userinfo.FirstName;
                        tb.IsActive = true;
                        var shift = myapp.tbl_ShiftType.FirstOrDefault(a => a.ShiftTypeId == es.ShiftTypeId);
                        tb.ShiftTypeName = shift.ShiftTypeName;
                        tb.ShiftStartTime = shift.ShiftStartTime;
                        tb.ShiftEndTime = shift.ShiftEndTime;
                        myapp.tbl_Roaster.Add(tb);
                        myapp.SaveChanges();
                    }
                }
            }
            return msg;
        }
        public string SaveTransferEmployee(tbl_User user, string currentuser)
        {
            var data = myapp.tbl_User.FirstOrDefault(a => a.UserId == user.UserId);
            if (data != null)
            {
                tbl_User_History objhis = new tbl_User_History();
                objhis.UserId = data.UserId;
                objhis.FirstName = data.FirstName;
                objhis.LastName = data.LastName;
                objhis.CustomUserId = data.CustomUserId;
                objhis.DateOfJoining = data.DateOfJoining;
                objhis.EmailId = data.EmailId;
                objhis.PhoneNumber = data.PhoneNumber;
                objhis.Gender = data.Gender;
                objhis.Extenstion = data.Extenstion;
                objhis.LocationId = data.LocationId;
                objhis.LocationName = data.LocationName;
                objhis.DepartmentId = data.DepartmentId;
                objhis.DepartmentName = data.DepartmentName;
                objhis.DepartmentId1 = data.DepartmentId1;
                objhis.DepartmentName1 = data.DepartmentName1;
                objhis.DepartmentId2 = data.DepartmentId2;
                objhis.DepartmentName2 = data.DepartmentName2;
                objhis.Designation = data.Designation;
                objhis.PlaceAllocation = data.PlaceAllocation;
                objhis.UserType = data.UserType;
                objhis.SecurityQuestion = data.SecurityQuestion;
                objhis.SecurityAnswner = data.SecurityAnswner;
                objhis.Comments = data.Comments;
                objhis.IsActive = data.IsActive;
                objhis.CreatedOn = DateTime.Now;
                objhis.CreatedBy = currentuser;
                myapp.tbl_User_History.Add(objhis);
                myapp.SaveChanges();
                data.LocationId = user.LocationId;
                data.LocationName = user.LocationName;
                data.DepartmentId = user.DepartmentId;
                data.DepartmentName = user.DepartmentName;
                data.ReportingManagerId = user.ReportingManagerId;
                data.Designation = user.Designation;
                data.DesignationID = user.DesignationID;
                //if ((data.SubDepartmentId != null && data.SubDepartmentId != 0) || (user.SubDepartmentId != null && user.SubDepartmentId != 0))
                //{
                if (user.SubDepartmentId == null)
                {
                    data.SubDepartmentId = 0;
                    data.SubDepartmentName = "";
                }
                else
                {
                    data.SubDepartmentId = user.SubDepartmentId;
                    data.SubDepartmentName = user.SubDepartmentName;
                }
                //}
                myapp.SaveChanges();

                var leaves = myapp.tbl_Leave.Where(l => l.UserId == data.CustomUserId).ToList();
                foreach (var v in leaves)
                {
                    v.LocationId = user.LocationId;
                    v.LocationName = user.LocationName;
                    v.DepartmentId = user.DepartmentId;
                    v.DepartmentName = user.DepartmentName;
                    myapp.SaveChanges();
                }
            }
            return "Sucess";

        }
        public List<RoasterData_RelationalClass> GetRoasterSpecificData(string username)
        {
            List<tbl_Roaster> RoasterData = myapp.tbl_Roaster.Where(e => e.UserId == username).ToList();
            List<RoasterData_RelationalClass> ReturnData = RoasterData.Select(e => new RoasterData_RelationalClass { ShiftDate = Convert.ToDateTime(e.ShiftDate), ShiftDate_SpecificFormat_2 = Convert.ToDateTime(e.ShiftDate).ToString("MM,dd,yyyy"), UserId = e.UserId, UserName = e.UserName }).ToList();
            return ReturnData;
        }
        public List<LeaveData_RelationalClass> GetSpecificLeaveDataFields(string username)
        {
            List<tbl_Leave> LeaveList = myapp.tbl_Leave.Where(e => e.UserId == username).ToList();
            List<LeaveData_RelationalClass> ReturnData = LeaveList.Select(e => new LeaveData_RelationalClass
            {
                AddressOnLeave = e.AddressOnLeave,
                DateofAvailableCompoff = Convert.ToDateTime(e.DateofAvailableCompoff),
                IsCompOff = Convert.ToBoolean(e.IsCompOff),
                IsFullday = Convert.ToBoolean(e.IsFullday),
                LeaveFromDate = Convert.ToDateTime(e.LeaveFromDate),
                LeaveFromDate_SpecificFormat_2 = e.LeaveFromDate.HasValue ? ProjectConvert.ConverDateTimeToString(e.LeaveFromDate.Value) : "",
                LeaveTodate = Convert.ToDateTime(e.LeaveTodate),
                LeaveTodate_SpecificFormat_2 = e.LeaveTodate.HasValue ? ProjectConvert.ConverDateTimeToString(e.LeaveTodate.Value) : "",
                LeaveTypeId = Convert.ToInt32(e.LeaveTypeId),
                LeaveTypeName = e.LeaveTypeName,
                ReasonForLeave = e.ReasonForLeave,
                UserId = e.UserId,
                UserName = e.UserName
            }).ToList();
            return ReturnData;
        }
        public List<string> GetShiftTimetoddl(string start, string end, string date)
        {
            List<string> timelist = new List<string>();
            if (start == null || start == "")
            {
                start = "06:00 AM";
            }
            if (end == null || end == "")
            {
                end = "10:00 PM";
            }
            DateTime stdate = Convert.ToDateTime(date + " " + start);
            DateTime enddate = Convert.ToDateTime(date + " " + end);
            var newdate = stdate;
            if (newdate <= enddate)
            {
                while (newdate <= enddate)
                {
                    timelist.Add(newdate.ToShortTimeString());
                    newdate = newdate.AddMinutes(30);
                }
            }
            else
            {
                enddate = enddate.AddDays(1);
                while (newdate <= enddate)
                {
                    timelist.Add(newdate.ToShortTimeString());
                    newdate = newdate.AddMinutes(30);
                }
            }

            return timelist;
        }
    }
}