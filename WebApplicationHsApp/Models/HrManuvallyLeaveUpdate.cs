using System;
using System.Linq;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class HrManuvallyLeaveUpdate
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public bool CheckAddedornot(string userid, int year, string leavetype)
        {
            var leavehistory = myapp.tbl_LeaveUpdateHistory.Where(u => u.UserId == userid && u.LeaveType == leavetype && u.IsYearly == true && u.Year == year).ToList();
            if (leavehistory.Count > 0)
            {
                return false;
            }
            else return true;
        }
        public bool CheckEmployeeLeaveYearWise()
        {
            var list = myapp.tbl_User.Where(u => u.IsActive == true && u.DateOfJoining != null && u.Comments != "offrolls").ToList();
            if (list.Count > 0)
            {
                foreach (var v in list)
                {
                    DateTime DateofJoining = v.DateOfJoining.Value;
                    var days = (DateTime.Now - DateofJoining).TotalDays;

                    if (days > 365)
                    {
                        int year = (int)(days / 365.25);
                        if (CheckAddedornot(v.CustomUserId, year, "5"))
                        {
                            //EL
                            double TotalLeaveBalance = GetNoOfLeaveBalance(v.CustomUserId, v.DateOfJoining.Value);
                            var Allleavestypes = myapp.tbl_ManageLeave.Where(ml => ml.UserId == v.CustomUserId).ToList();
                            var checkcasulatype = Allleavestypes.Where(ml => ml.LeaveTypeId == 5).ToList();
                            if (checkcasulatype.Count > 0)
                            {
                                if (TotalLeaveBalance > 90)
                                {
                                    TotalLeaveBalance = 90;
                                }
                                checkcasulatype[0].AvailableLeave = TotalLeaveBalance + 15;
                                //checkcasulatype[0].CountOfLeave = TotalLeaveBalance + 15;
                                myapp.SaveChanges();
                                LogLeavesHistory(15, "5", "Earned Leave Added For the " + v.FirstName + " " + v.LastName + " on " + DateTime.Now.ToString(), v.CustomUserId, year, DateTime.Now.Month + 1, true, false);
                            }
                            else
                            {
                                tbl_ManageLeave tblml = new tbl_ManageLeave();
                                tblml.AvailableLeave = 15;
                                tblml.CountOfLeave = 15;
                                tblml.CreatedBy = "Admin";
                                tblml.CreatedOn = DateTime.Now;
                                tblml.DepartmentId = v.DepartmentId;
                                tblml.DepartmentName = v.DepartmentName;
                                tblml.ExpireDate = DateTime.Now.AddDays(31);
                                tblml.IsActive = true;
                                tblml.LeaveTypeId = 5;
                                tblml.LeaveTypeName = "Earned Leave";
                                tblml.LocationId = v.LocationId;
                                tblml.LocationName = v.LocationName;
                                tblml.ModifiedBy = "Admin";
                                tblml.ModifiedOn = DateTime.Now;
                                tblml.UserId = v.CustomUserId;
                                tblml.UserName = v.FirstName + " " + v.LastName;
                                myapp.tbl_ManageLeave.Add(tblml);
                                myapp.SaveChanges();
                                LogLeavesHistory(15, "5", "Earned Leave Added For the " + v.FirstName + " " + v.LastName + " on " + DateTime.Now.ToString(), v.CustomUserId, year, DateTime.Now.Month + 1, true, false);
                            }
                        }
                    }
                }
            }
            return true;
        }
        public double GetNoOfLeaveBalance(string customerid, DateTime joining)
        {
            double LeaveBalace = 0;
            var days = (DateTime.Now - joining).TotalDays;
            DateTime Dtprev = joining;
            joining = joining.AddDays(365.25);
            int year = 1;
            while (joining < DateTime.Now)
            {
                double addedleaves = 0;
                var addedleaveslist = myapp.tbl_LeaveUpdateHistory.Where(l => l.UserId == customerid && l.Year == year && l.IsYearly == true && l.LeaveType == "5").ToList();
                if (addedleaveslist.Count > 0)
                {
                    addedleaves = addedleaveslist[0].AddedLeaves.Value;
                }
                DateTime nextyear = joining.AddDays(365.25);
                var cleavestaken = myapp.tbl_Leave.Where(l => l.UserId == customerid && l.LeaveFromDate >= joining && l.LeaveFromDate <= nextyear && l.LeaveTypeId == 5).Sum(l => l.LeaveCount);
                if (cleavestaken.HasValue)
                {
                    if (cleavestaken.Value != 0)
                    {
                        addedleaves = addedleaves - cleavestaken.Value;
                    }
                }
                //if (addedleaves > 8)
                //{
                //    addedleaves = 8;
                //}
                LeaveBalace = LeaveBalace + addedleaves;
                Dtprev = joining;
                joining = joining.AddDays(365.25);
                year++;
            }
            return LeaveBalace;
        }

        public void LogLeavesHistory(int AddedLeaves, string LeaveType, string Remarks, string CustomUserId, int year, int month, bool Isyearly, bool Ismonthly)
        {
            tbl_LeaveUpdateHistory luph = new tbl_LeaveUpdateHistory();
            luph.AddedLeaves = AddedLeaves;
            luph.Created = DateTime.Now;
            luph.LeaveType = LeaveType;
            luph.Remarks = Remarks;
            luph.UserId = CustomUserId;
            luph.Year = year;
            luph.Month = month;
            luph.IsYearly = Isyearly;
            luph.IsMonthly = Ismonthly;
            myapp.tbl_LeaveUpdateHistory.Add(luph);
            myapp.SaveChanges();
        }



        public bool CheckAddedornotCLSL(string userid, int year, int month, string leavetype)
        {
            var leavehistory = myapp.tbl_LeaveUpdateHistory.Where(u => u.UserId == userid && u.IsMonthly == true && u.Year == year && u.Month == month && u.LeaveType == leavetype).ToList();
            if (leavehistory.Count > 0)
            {
                return false;
            }
            else return true;
        }

        public bool UpdateCasuvalAndSickLeaves_New()
        {
            var list = myapp.tbl_User.Where(u => u.IsActive == true && u.DateOfJoining != null && u.Comments != "offrolls").ToList();
            if (list.Count > 0)
            {
                foreach (var v in list)
                {
                    DateTime DateofJoining = v.DateOfJoining.Value;
                    var days = (DateTime.Now - DateofJoining).TotalDays;
                    if (days > 16)
                    {
                        var Allleavestypes = myapp.tbl_ManageLeave.Where(ml => ml.UserId == v.CustomUserId).ToList();
                        //if (DateTime.Now.Day == 1)
                        //{
                        if (CheckAddedornotCLSL(v.CustomUserId, DateTime.Now.Year, DateTime.Now.Month, "1"))
                        {
                            var checkcasulatype = Allleavestypes.Where(ml => ml.LeaveTypeId == 1).ToList();

                            if (checkcasulatype.Count > 0)
                            {
                                if (DateTime.Now.Month == 1)
                                {
                                    checkcasulatype[0].AvailableLeave = 1;
                                }
                                else { checkcasulatype[0].AvailableLeave = checkcasulatype[0].AvailableLeave + 1; }

                                LogLeavesHistory(1, "1", "Casuval Leave Added For the " + v.FirstName + " " + v.LastName + " on " + DateTime.Now.ToString(), v.CustomUserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
                            }
                            else
                            {
                                tbl_ManageLeave tblml = new tbl_ManageLeave();
                                tblml.AvailableLeave = 1;
                                tblml.CountOfLeave = 1;
                                tblml.CreatedBy = "Admin";
                                tblml.CreatedOn = DateTime.Now;
                                tblml.DepartmentId = v.DepartmentId;
                                tblml.DepartmentName = v.DepartmentName;
                                tblml.ExpireDate = DateTime.Now.AddDays(31);
                                tblml.IsActive = true;
                                tblml.LeaveTypeId = 1;
                                tblml.LeaveTypeName = "Casuval Leave";
                                tblml.LocationId = v.LocationId;
                                tblml.LocationName = v.LocationName;
                                tblml.ModifiedBy = "Admin";
                                tblml.ModifiedOn = DateTime.Now;
                                tblml.UserId = v.CustomUserId;
                                tblml.UserName = v.FirstName + " " + v.LastName;
                                myapp.tbl_ManageLeave.Add(tblml);
                                LogLeavesHistory(1, "1", "Casuval Leave Added For the " + v.FirstName + " " + v.LastName + " on " + DateTime.Now.ToString(), v.CustomUserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
                            }
                            myapp.SaveChanges();
                        }
                        if (CheckAddedornotCLSL(v.CustomUserId, DateTime.Now.Year, DateTime.Now.Month, "4"))
                        {
                            var checkSicktype = Allleavestypes.Where(ml => ml.LeaveTypeId == 4).ToList();
                            if (checkSicktype.Count > 0)
                            {
                                if (DateTime.Now.Month == 1)
                                {
                                    if (checkSicktype[0].AvailableLeave > 8)
                                    {
                                        checkSicktype[0].AvailableLeave = 8;
                                    }
                                    checkSicktype[0].AvailableLeave = checkSicktype[0].AvailableLeave + 1;
                                }
                                else
                                {
                                    checkSicktype[0].AvailableLeave = checkSicktype[0].AvailableLeave + 1;
                                }
                                LogLeavesHistory(1, "4", "Sick Leave Added For the " + v.FirstName + " " + v.LastName + " on " + DateTime.Now.ToString(), v.CustomUserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
                            }
                            else
                            {
                                tbl_ManageLeave tblml = new tbl_ManageLeave();
                                tblml.AvailableLeave = 1;
                                tblml.CountOfLeave = 1;
                                tblml.CreatedBy = "Admin";
                                tblml.CreatedOn = DateTime.Now;
                                tblml.DepartmentId = v.DepartmentId;
                                tblml.DepartmentName = v.DepartmentName;
                                tblml.ExpireDate = DateTime.Now.AddDays(31);
                                tblml.IsActive = true;
                                tblml.LeaveTypeId = 4;
                                tblml.LeaveTypeName = "Sick Leave";
                                tblml.LocationId = v.LocationId;
                                tblml.LocationName = v.LocationName;
                                tblml.ModifiedBy = "Admin";
                                tblml.ModifiedOn = DateTime.Now;
                                tblml.UserId = v.CustomUserId;
                                tblml.UserName = v.FirstName + " " + v.LastName;
                                myapp.tbl_ManageLeave.Add(tblml);
                                LogLeavesHistory(1, "4", "Sick Leave Added For the " + v.FirstName + " " + v.LastName + " on " + DateTime.Now.ToString(), v.CustomUserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
                            }
                            myapp.SaveChanges();
                            //  }
                            //}
                        }
                    }
                }
            }
            return true;
        }
    }
}