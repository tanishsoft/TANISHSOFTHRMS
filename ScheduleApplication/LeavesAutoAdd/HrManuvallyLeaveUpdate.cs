using System;
using System.Linq;
using ScheduleApplication.Common;
using ScheduleApplication.DataModel;
using System.Configuration;

namespace ScheduleApplication.LeavesAutoAdd
{
    public class HrManuvallyLeaveUpdate
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        string Evironmentcheck = ConfigurationManager.AppSettings["Environment"];
        //employees
        public bool CheckAddedornot(string userid, int year, string leavetype)
        {
            var leavehistory = myapp.tbl_LeaveUpdateHistory.Where(u => u.UserId == userid && u.LeaveType == leavetype && u.IsYearly == true && u.Year == year).ToList();
            if (leavehistory.Count > 0)
            {
                return false;
            }
            else return true;
        }
        public string CheckEmployeeLeaveYearWise()
        {
            string jobrunstatus = "Success";

            try
            {
                bool isbody = false;
                string htmlforleaves = "<html><body><p>Dear Sir,</p><p>Leaves are updated please find the below details.</p><table style='width: 100%; border:solid 1px #eee;'>";
                htmlforleaves += "<thead>";
                htmlforleaves += "<tr style='background-color: teal;color:black;'>";
                htmlforleaves += "<td style='padding:5px'>Name</td>";
                htmlforleaves += "<td style='padding:5px'>Employe Id</td>";
                htmlforleaves += "<td style='padding:5px'>Leave Type</td>";
                htmlforleaves += "<td style='padding:5px'>Added Leaves</td>";
                htmlforleaves += "<td style='padding:5px'>Date Time</td>";
                htmlforleaves += "</tr>";
                htmlforleaves += "</thead><tbody>";
                var list = myapp.tbl_User.Where(u => u.IsActive == true && u.DateOfJoining != null && (u.IsEmployee == true || u.IsOnRollDoctor == true)).ToList();
                list = list.Where(l => l.Comments != "offrole").ToList();
                if (list.Count > 0)
                {
                    foreach (var v in list)
                    {
                        //user check
                        //var query = myapp.Database.SqlQuery<int>("select count(au.UserName) as Total from AspNetUsers au inner join AspNetUserRoles aur on aur.UserId=au.Id inner join AspNetRoles r on r.Id=aur.RoleId where r.Name='Employee' and au.UserName=@userName", new SqlParameter("userName", v.CustomUserId)).SingleOrDefault();
                        //if (query > 0)
                        //{
                        DateTime DateofJoining = v.DateOfJoining.Value;
                        var days = (DateTime.Now - DateofJoining).TotalDays;

                        if (days > 365)
                        {
                            int year = (int)(days / 365.25);
                            if (CheckAddedornot(v.CustomUserId, year, "5"))
                            {
                                //EL
                                //double TotalLeaveBalance = GetNoOfLeaveBalance(v.CustomUserId, v.DateOfJoining.Value);
                                var Allleavestypes = myapp.tbl_ManageLeave.Where(ml => ml.UserId == v.CustomUserId).ToList();
                                var checkcasulatype = Allleavestypes.Where(ml => ml.LeaveTypeId == 5).ToList();
                                if (checkcasulatype.Count > 0)
                                {
                                    if (checkcasulatype[0].AvailableLeave > 90)
                                    {
                                        checkcasulatype[0].AvailableLeave = 90;
                                    }
                                    checkcasulatype[0].AvailableLeave = checkcasulatype[0].AvailableLeave + 15;
                                    //checkcasulatype[0].CountOfLeave = TotalLeaveBalance + 15;


                                    htmlforleaves += "<tr style='background-color: orange;'>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.FirstName + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.CustomUserId + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>Earned Leave</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + 15 + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + DateTime.Now + "</td>";
                                    htmlforleaves += "</tr>";
                                    isbody = true;

                                    myapp.SaveChanges();
                                    LogLeavesHistory(15, "5", "Earned Leave Added For the " + v.FirstName + " " + v.LastName + " on " + DateTime.Now.ToString(), v.CustomUserId, year, DateTime.Now.Month, true, false);
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
                                    htmlforleaves += "<tr style='background-color: orange;'>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.FirstName + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.CustomUserId + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>Earned Leave</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + 15 + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + DateTime.Now + "</td>";
                                    htmlforleaves += "</tr>";
                                    isbody = true;
                                    LogLeavesHistory(15, "5", "Earned Leave Added For the " + v.FirstName + " " + v.LastName + " on " + DateTime.Now.ToString(), v.CustomUserId, year, DateTime.Now.Month, true, false);
                                }
                            }
                            //}
                        }
                    }
                    htmlforleaves += "</tbody></table></body></html>";
                    SendAnEmail cm = new SendAnEmail();
                    MailModel mailmodel = new MailModel();
                    mailmodel.fromemail = "Leave@hospitals.com";
                    mailmodel.toemail = "ahmadali@fernandez.foundation";
                    mailmodel.ccemail = "vamsi@microarctech.com";
                    mailmodel.subject = "Leaves Are Updated  Earned Leaves - " + DateTime.Now;
                    mailmodel.body = htmlforleaves;
                    mailmodel.filepath = "";
                    mailmodel.username = "Fernandez Hospital Leave Management";
                    mailmodel.fromname = "Leave Application";
                    if (isbody)
                        cm.SendEmail(mailmodel);
                }
            }
            catch (Exception ex)
            {
                jobrunstatus = "Error " + ex.Message;
            }
            return jobrunstatus;
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

        public void LogLeavesHistory(double AddedLeaves, string LeaveType, string Remarks, string CustomUserId, int year, int month, bool Isyearly, bool Ismonthly)
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

        public string UpdateCasuvalAndSickLeaves_New()
        {
            string jobrunstatus = "";
            try
            {
                string htmlforleaves = "<html><body><p>Dear Sir,</p><br /><p>Leaves are updated please find the below details.</p><table style='width: 100%; border:solid 1px #eee;'>";
                htmlforleaves += "<thead>";
                htmlforleaves += "<tr style='background-color: orange;'>";
                htmlforleaves += "<td style='padding:5px'>Name</td>";
                htmlforleaves += "<td style='padding:5px'>Employe Id</td>";
                htmlforleaves += "<td style='padding:5px'>Leave Type</td>";
                htmlforleaves += "<td style='padding:5px'>Added Leaves</td>";
                htmlforleaves += "<td style='padding:5px'>Date Time</td>";
                htmlforleaves += "</tr>";
                htmlforleaves += "</thead><tbody>";
                bool isbody = false;
                var list = myapp.tbl_User.Where(u => u.IsActive == true && u.DateOfJoining != null && (u.IsEmployee == true || u.IsOnRollDoctor == true)).ToList();
                if (list.Count > 0)
                {
                    foreach (var v in list)
                    {
                        //var query = myapp.Database.SqlQuery<int>("select count(au.UserName) as Total from AspNetUsers au inner join AspNetUserRoles aur on aur.UserId=au.Id inner join AspNetRoles r on r.Id=aur.RoleId where r.Name='Employee' and au.UserName=@userName", new SqlParameter("userName", v.CustomUserId)).SingleOrDefault();
                        //if (query > 0)
                        //{
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

                                    htmlforleaves += "<tr style='background-color: orange;'>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.FirstName + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.CustomUserId + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>Casuval Leave</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + 1 + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + DateTime.Now + "</td>";
                                    htmlforleaves += "</tr>";

                                    isbody = true;
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
                                    htmlforleaves += "<tr style='background-color: orange;'>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.FirstName + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.CustomUserId + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>Casuval Leave</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + 1 + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + DateTime.Now + "</td>";
                                    htmlforleaves += "</tr>";
                                    isbody = true;
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
                                    htmlforleaves += "<tr style='background-color: orange;'>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.FirstName + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.CustomUserId + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>Sick Leave</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + 1 + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + DateTime.Now + "</td>";
                                    htmlforleaves += "</tr>"; isbody = true;
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
                                    htmlforleaves += "<tr style='background-color: orange;'>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.FirstName + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.CustomUserId + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>Sick Leave</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + 1 + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + DateTime.Now + "</td>";
                                    htmlforleaves += "</tr>"; isbody = true;
                                    LogLeavesHistory(1, "4", "Sick Leave Added For the " + v.FirstName + " " + v.LastName + " on " + DateTime.Now.ToString(), v.CustomUserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
                                }
                                myapp.SaveChanges();
                                //  }
                                //}
                            }
                            //}
                        }
                    }
                    htmlforleaves += "</tbody></table></body></html>";
                    SendAnEmail cm = new SendAnEmail();
                    MailModel mailmodel = new MailModel();

                    if (isbody)
                    {
                        mailmodel.fromemail = "Leave@hospitals.com";
                        mailmodel.toemail = "ahmadali@fernandez.foundation";
                        mailmodel.ccemail = "vamsi@microarctech.com";

                        mailmodel.subject =  "Leaves Are Updated Casuval And Sick - " + DateTime.Now;
                        mailmodel.body = htmlforleaves;
                        mailmodel.filepath = "";
                        mailmodel.username = "Fernandez Hospital Leave Management";
                        mailmodel.fromname = "Leave Application";
                        cm.SendEmail(mailmodel);
                    }

                }
            }
            catch (Exception ex)
            {
                jobrunstatus = "Error " + ex.Message;
            }
            return jobrunstatus;
        }

        //docotors

        public string UpdateYearlyDoctorsleaves()
        {
            string jobrunstatus = "Success";
            try
            {
                bool isbody = false;
                string htmlforleaves = "<html><body><p>Dear Sir,</p><p>Leaves are updated please find the below details.</p><table style='width: 100%; border:solid 1px #eee;'>";
                htmlforleaves += "<thead>";
                htmlforleaves += "<tr style='background-color: teal;color:black;'>";
                htmlforleaves += "<td style='padding:5px'>Name</td>";
                htmlforleaves += "<td style='padding:5px'>Employe Id</td>";
                htmlforleaves += "<td style='padding:5px'>Leave Type</td>";
                htmlforleaves += "<td style='padding:5px'>Added Leaves</td>";
                htmlforleaves += "<td style='padding:5px'>Date Time</td>";
                htmlforleaves += "</tr>";
                htmlforleaves += "</thead><tbody>";
                var list = myapp.tbl_User.Where(u => u.IsActive == true && u.DateOfJoining != null && u.IsOffRollDoctor == true).ToList();
                if (list.Count > 0)
                {
                    foreach (var v in list)
                    {
                        //user check
                        //var query = myapp.Database.SqlQuery<int>("select count(au.UserName) as Total from AspNetUsers au inner join AspNetUserRoles aur on aur.UserId=au.Id inner join AspNetRoles r on r.Id=aur.RoleId where r.Name='Doctor' and au.UserName=@userName", new SqlParameter("userName", v.CustomUserId)).SingleOrDefault();
                        //if (query > 0)
                        //{
                        DateTime DateofJoining = v.DateOfJoining.Value;
                        var days = (DateTime.Now - DateofJoining).TotalDays;
                        int year = (int)(days / 365.25);
                        if (year > 5)
                        {
                            if (CheckAddedornot(v.CustomUserId, year, "4"))
                            {
                                //EL
                                var Allleavestypes = myapp.tbl_ManageLeave.Where(ml => ml.UserId == v.CustomUserId).ToList();
                                var checkcasulatype = Allleavestypes.Where(ml => ml.LeaveTypeId == 4).ToList();
                                if (checkcasulatype.Count > 0)
                                {

                                    checkcasulatype[0].AvailableLeave = 12;
                                    checkcasulatype[0].CountOfLeave = 12;
                                    htmlforleaves += "<tr style='background-color: orange;'>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.FirstName + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.CustomUserId + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>Sick Leave</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + 12 + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + DateTime.Now + "</td>";
                                    htmlforleaves += "</tr>";
                                    isbody = true;

                                    myapp.SaveChanges();
                                    LogLeavesHistory(12, "4", "Sick Leave Added For the " + v.FirstName + " " + v.LastName + " on " + DateTime.Now.ToString(), v.CustomUserId, year, DateTime.Now.Month, true, false);
                                }
                                else
                                {
                                    tbl_ManageLeave tblml = new tbl_ManageLeave();
                                    tblml.AvailableLeave = 12;
                                    tblml.CountOfLeave = 12;
                                    tblml.CreatedBy = "Admin";
                                    tblml.CreatedOn = DateTime.Now;
                                    tblml.DepartmentId = v.DepartmentId;
                                    tblml.DepartmentName = v.DepartmentName;
                                    tblml.ExpireDate = DateTime.Now.AddYears(1);
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
                                    myapp.SaveChanges();
                                    htmlforleaves += "<tr style='background-color: orange;'>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.FirstName + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + v.CustomUserId + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>Sick Leave</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + 12 + "</td>";
                                    htmlforleaves += "<td style='padding:5px'>" + DateTime.Now + "</td>";
                                    htmlforleaves += "</tr>";
                                    isbody = true;
                                    LogLeavesHistory(12, "4", "Scik Leave Added For the " + v.FirstName + " " + v.LastName + " on " + DateTime.Now.ToString(), v.CustomUserId, year, DateTime.Now.Month, true, false);
                                }
                            }
                        }
                        if (CheckAddedornotCLSL(v.CustomUserId, DateTime.Now.Year, DateTime.Now.Month, "1"))
                        {
                            //EL
                            var Allleavestypes = myapp.tbl_ManageLeave.Where(ml => ml.UserId == v.CustomUserId).ToList();
                            var checkcasulatype = Allleavestypes.Where(ml => ml.LeaveTypeId == 1).ToList();
                            if (checkcasulatype.Count > 0)
                            {
                                if (DateTime.Now.Month == 1)
                                {
                                    checkcasulatype[0].AvailableLeave = 2.75;
                                }
                                else
                                {
                                    checkcasulatype[0].AvailableLeave = checkcasulatype[0].AvailableLeave + 2.75;
                                }
                                //checkcasulatype[0].CountOfLeave = 33;
                                htmlforleaves += "<tr style='background-color: orange;'>";
                                htmlforleaves += "<td style='padding:5px'>" + v.FirstName + "</td>";
                                htmlforleaves += "<td style='padding:5px'>" + v.CustomUserId + "</td>";
                                htmlforleaves += "<td style='padding:5px'>Casual Leave</td>";
                                htmlforleaves += "<td style='padding:5px'>" + 2.75 + "</td>";
                                htmlforleaves += "<td style='padding:5px'>" + DateTime.Now + "</td>";
                                htmlforleaves += "</tr>";
                                isbody = true;

                                myapp.SaveChanges();
                                LogLeavesHistory(2.75, "1", "Casual Leave Added For the " + v.FirstName + " " + v.LastName + " on " + DateTime.Now.ToString(), v.CustomUserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
                            }
                            else
                            {
                                tbl_ManageLeave tblml = new tbl_ManageLeave();
                                tblml.AvailableLeave = 2.75;
                                tblml.CountOfLeave = 2.75;
                                tblml.CreatedBy = "Admin";
                                tblml.CreatedOn = DateTime.Now;
                                tblml.DepartmentId = v.DepartmentId;
                                tblml.DepartmentName = v.DepartmentName;
                                tblml.ExpireDate = DateTime.Now.AddYears(1);
                                tblml.IsActive = true;
                                tblml.LeaveTypeId = 1;
                                tblml.LeaveTypeName = "Casual Leave";
                                tblml.LocationId = v.LocationId;
                                tblml.LocationName = v.LocationName;
                                tblml.ModifiedBy = "Admin";
                                tblml.ModifiedOn = DateTime.Now;
                                tblml.UserId = v.CustomUserId;
                                tblml.UserName = v.FirstName + " " + v.LastName;
                                myapp.tbl_ManageLeave.Add(tblml);
                                myapp.SaveChanges();
                                htmlforleaves += "<tr style='background-color: orange;'>";
                                htmlforleaves += "<td style='padding:5px'>" + v.FirstName + "</td>";
                                htmlforleaves += "<td style='padding:5px'>" + v.CustomUserId + "</td>";
                                htmlforleaves += "<td style='padding:5px'>Casual Leave</td>";
                                htmlforleaves += "<td style='padding:5px'>" + 2.75 + "</td>";
                                htmlforleaves += "<td style='padding:5px'>" + DateTime.Now + "</td>";
                                htmlforleaves += "</tr>";
                                isbody = true;
                                LogLeavesHistory(2.75, "1", "Casual Leave Added For the " + v.FirstName + " " + v.LastName + " on " + DateTime.Now.ToString(), v.CustomUserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
                            }
                        }
                        //}
                    }
                }
                htmlforleaves += "</tbody></table></body></html>";
                SendAnEmail cm = new SendAnEmail();
                MailModel mailmodel = new MailModel();

                if (isbody)
                {
                    mailmodel.fromemail = "Leave@hospitals.com";
                    mailmodel.toemail = "ahmadali@fernandez.foundation";
                    mailmodel.ccemail = "vamsi@microarctech.com";
                    mailmodel.subject = "Doctors Leaves Are Updated  Earned Leaves - " + DateTime.Now;
                    mailmodel.body = htmlforleaves;
                    mailmodel.filepath = "";
                    mailmodel.username = "Fernandez Hospital Leave Management";
                    mailmodel.fromname = "Leave Application";
                    cm.SendEmail(mailmodel);
                }
            }
            catch (Exception ex)
            {
                jobrunstatus = "Error " + ex.Message;
            }
            return jobrunstatus;
        }
    }
}
