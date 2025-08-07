using Oracle.ManagedDataAccess.Client;
using ScheduleApplication.Common;
using ScheduleApplication.DataModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace ScheduleApplication.OracleDBEmployee
{
    public class ConnectOracle
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        string Evironmentcheck = ConfigurationManager.AppSettings["Environment"];
        string host = "111.93.11.121";
        string port = "1521";
        string user = "intranet";
        string password = "intranet";
        string sid = "neosoft";
        public string GetEmployees()
        {
            string Messagestatus = "Success";

            string oradb = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = " + sid + ")));Password=" + password + ";User ID=" + user;
            OracleConnection con = new OracleConnection(oradb);
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select * from emp_details where retirementflag != 1";
                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //Response.Write("<table border='1'>");  
                    //Response.Write("<tr><th>Name</th><th>Roll No</th></tr>"); 
                    List<emp_details> listemp = new List<emp_details>();

                    while (dr.Read())
                    {
                        emp_details obj = new emp_details();
                        obj.empnm = dr["empnm"] != null ? dr["empnm"].ToString() : "";
                        obj.empcode = dr["empcode"] != null ? dr["empcode"].ToString() : "";
                        obj.sdeptid = dr["sdeptid"] != null ? dr["sdeptid"].ToString() : "";
                        obj.designationid = dr["designationid"] != null ? dr["designationid"].ToString() : "";
                        obj.address = dr["address"] != null ? dr["address"].ToString() : "";
                        obj.area = dr["area"] != null ? dr["area"].ToString() : "";
                        obj.city = dr["city"] != null ? dr["city"].ToString() : "";
                        obj.state = dr["state"] != null ? dr["state"].ToString() : "";
                        obj.pincode = dr["pincode"] != null ? dr["pincode"].ToString() : "";
                        obj.phno = dr["phno"] != null ? dr["phno"].ToString() : "";
                        obj.doj = dr["doj"] != null ? dr["doj"].ToString() : "";
                        obj.dob = dr["dob"] != null ? dr["dob"].ToString() : "";
                        obj.sex = dr["sex"] != null ? dr["sex"].ToString() : "";
                        obj.companynm = dr["companynm"] != null ? dr["companynm"].ToString() : "";
                        obj.companyid = dr["companyid"] != null ? dr["companyid"].ToString() : "";
                        obj.grade = dr["grade"] != null ? dr["grade"].ToString() : "";
                        obj.bldgrp = dr["bldgrp"] != null ? dr["bldgrp"].ToString() : "";
                        obj.emptype = dr["emptype"] != null ? dr["emptype"].ToString() : "";
                        obj.martialstatus = dr["martialstatus"] != null ? dr["martialstatus"].ToString() : "";
                        obj.relativename = dr["relativename"] != null ? dr["relativename"].ToString() : "";
                        obj.emailid = dr["emailid"] != null ? dr["emailid"].ToString() : "";
                        obj.locid = dr["locid"] != null ? dr["locid"].ToString() : "";
                        obj.branch = dr["branch"] != null ? dr["branch"].ToString() : "";
                        obj.emergencyno = dr["emergencyno"] != null ? dr["emergencyno"].ToString() : "";
                        listemp.Add(obj);
                    }

                    SendAnEmail cm1 = new SendAnEmail();
                    MailModel mailmodel1 = new MailModel();
                    mailmodel1.fromemail = "helpdesk@fernandez.foundation";
                    mailmodel1.toemail = "ahmadali@fernandez.foundation";
                    mailmodel1.ccemail = "vamsi@microarctech.com";
                    string bodyofemail = "<div>";

                    bodyofemail += "<p style='font-size: 15px;font-family: cambria;color:teal;'>Dear Sir Please find below employees are added into intranet from oracle.</p>";
                    mailmodel1.subject = "Employees added into intranet";

                    mailmodel1.filepath = "";
                    mailmodel1.username = "New Employees";
                    mailmodel1.fromname = "Fernandez Hospital";

                    foreach (var model in listemp)
                    {
                        try
                        {
                            string customuserid = "FH_" + model.empcode;
                            var userslist = myapp.tbl_User.Where(l => l.CustomUserId == customuserid).ToList();
                            if (userslist.Count == 0)
                            {
                                string Password = "";
                                tbl_User tbluser = new tbl_User();
                                tbluser.FirstName = model.empnm;

                                tbluser.LastName = "";
                                tbluser.CustomUserId = "FH_" + model.empcode;
                                tbluser.DateOfBirth = ConverDateStringtoDatetime(model.dob.Replace(" 00:00:00", "").Replace(" 12:00:00 AM", ""));
                                tbluser.DateOfJoining = ConverDateStringtoDatetime(model.doj.Replace(" 00:00:00", "").Replace(" 12:00:00 AM", ""));
                                tbluser.EmailId = model.emailid;
                                tbluser.PhoneNumber = model.phno;
                                tbluser.Extenstion = model.emergencyno;
                                tbluser.LocationName = model.companynm;
                                tbluser.DepartmentName = model.sdeptid;
                                var locationlist = myapp.tbl_Location.Where(l => l.companyid == model.companyid).ToList();
                                if (locationlist.Count > 0)
                                {
                                    tbluser.LocationId = locationlist[0].LocationId;
                                    tbluser.LocationName = locationlist[0].LocationName;
                                    var deptorcllist = myapp.tbl_Oracle_subdepts.Where(l => l.sdeptid == model.sdeptid).ToList();
                                    if (deptorcllist.Count > 0)
                                    {
                                        string name = deptorcllist[0].subdeptnm.ToLower().Replace(" ", "");
                                        var deplist = myapp.tbl_Department.Where(l => l.LocationId == tbluser.LocationId).ToList();
                                        var checkcount = deplist.Where(l => l.DepartmentName.ToLower().Replace(" ", "") == name).ToList();
                                        if (checkcount.Count > 0)
                                        {
                                            tbluser.DepartmentId = checkcount[0].DepartmentId;
                                            tbluser.DepartmentName = checkcount[0].DepartmentName;
                                        }
                                        else
                                        {
                                            tbl_Department modeldept = new tbl_Department();
                                            modeldept.LocationId = tbluser.LocationId;
                                            modeldept.LocationName = locationlist[0].LocationName;
                                            modeldept.ShowInHelpDesk = false;
                                            modeldept.DepartmentName = deptorcllist[0].subdeptnm;
                                            myapp.tbl_Department.Add(modeldept);
                                            myapp.SaveChanges();
                                            tbluser.DepartmentId = modeldept.DepartmentId;
                                            tbluser.DepartmentName = deptorcllist[0].subdeptnm;
                                        }
                                    }
                                    //tbluser.DepartmentId = model.DepartmentId;
                                }
                                var desiglist = myapp.tbl_MasterEmployeeDesignation.Where(l => l.designationid == model.designationid).ToList();
                                if (desiglist.Count > 0)
                                {
                                    tbluser.Designation = desiglist[0].Designation_Name;
                                    tbluser.DesignationID = desiglist[0].ID;
                                    //tbluser.DepartmentId = model.DepartmentId;
                                }
                                tbluser.Gender = model.sex == "1" ? "Male" : "Female";

                                //tbluser.DesignationID = Convert.ToInt64(model.DesignationID);
                                //tbluser.Designation = model.designationid;
                                //tbluser.PlaceAllocation = model.PlaceAllocation;
                                tbluser.SecurityQuestion = "";
                                tbluser.SecurityAnswner = "";
                                tbluser.IsActive = true;
                                tbluser.CreatedBy = "System";
                                tbluser.CreatedOn = DateTime.Now;

                                tbluser.SubDepartmentId = 0;


                                tbluser.SubDepartmentName = "";
                                //tbluser.UserType = model.emptype;
                                tbluser.UserType = "Employee";

                                tbluser.IsEmployeesReporting = false;


                                tbluser.ChangePassword = true;

                                List<string> roleslist = new List<string>();
                                switch (tbluser.UserType)
                                {
                                    case "Employee":
                                        tbluser.IsEmployee = true;
                                        tbluser.IsOffRollDoctor = false;
                                        tbluser.IsOnRollDoctor = false;
                                        roleslist.Add("Employee");
                                        break;
                                    case "OnrollDoctor":
                                        tbluser.IsEmployee = false;
                                        tbluser.IsOffRollDoctor = false;
                                        tbluser.IsOnRollDoctor = true;
                                        roleslist.Add("Doctor");
                                        break;
                                    case "OffrollDoctor":
                                        tbluser.IsEmployee = false;
                                        tbluser.IsOffRollDoctor = true;
                                        tbluser.IsOnRollDoctor = false;
                                        roleslist.Add("Doctor");
                                        break;
                                }
                                tbluser.EmpId = int.Parse(tbluser.CustomUserId.ToLower().Replace("fh_", ""));
                                myapp.tbl_User.Add(tbluser);
                                myapp.SaveChanges();
                                bodyofemail += " <table><tr><td>Employee Name</td><td>" + tbluser.FirstName + "</td></tr>";
                                bodyofemail += "<tr><td>Employee Id</td><td>" + tbluser.CustomUserId + "</td></tr>";

                                // Add holidays
                                var holidaylist = myapp.tbl_Holiday.Where(h => h.HolidayType == "National").ToList();
                                if (holidaylist.Count > 0)
                                {
                                    foreach (var v in holidaylist)
                                    {
                                        tbl_Roaster rs = new tbl_Roaster();
                                        rs.CreatedBy = "System";
                                        rs.CreatedOn = DateTime.Now;
                                        rs.DepartmentId = tbluser.DepartmentId;
                                        rs.IsActive = true;
                                        rs.LocationId = tbluser.LocationId;
                                        rs.ShiftDate = v.HolidayDate;
                                        rs.ShiftEndTime = "18:30";
                                        rs.ShiftStartTime = "09:00";
                                        rs.ShiftTypeName = "Holiday";
                                        rs.ShiftTypeId = 3;
                                        rs.UserId = tbluser.CustomUserId;
                                        rs.UserName = tbluser.FirstName;
                                        myapp.tbl_Roaster.Add(rs);
                                        myapp.SaveChanges();
                                    }
                                }


                                if (tbluser.UserType == "Employee" || tbluser.UserType == "OnrollDoctor")
                                {
                                    if (tbluser.DateOfJoining.Value.Day < 15 && tbluser.DateOfJoining.Value.Month == DateTime.Now.Month && tbluser.DateOfJoining.Value.Year == DateTime.Now.Year)
                                    {
                                        tbl_ManageLeave tblml = new tbl_ManageLeave();
                                        tblml.AvailableLeave = 1;
                                        tblml.CountOfLeave = 1;
                                        tblml.CreatedBy = "Admin";
                                        tblml.CreatedOn = DateTime.Now;
                                        tblml.DepartmentId = tbluser.DepartmentId;
                                        tblml.DepartmentName = tbluser.DepartmentName;
                                        tblml.ExpireDate = DateTime.Now.AddDays(31);
                                        tblml.IsActive = true;
                                        tblml.LeaveTypeId = 1;
                                        tblml.LeaveTypeName = "Casuval Leave";
                                        tblml.LocationId = tbluser.LocationId;
                                        tblml.LocationName = tbluser.LocationName;
                                        tblml.ModifiedBy = "Admin";
                                        tblml.ModifiedOn = DateTime.Now;
                                        tblml.UserId = tbluser.CustomUserId;
                                        tblml.UserName = tbluser.FirstName + " " + tbluser.LastName;
                                        myapp.tbl_ManageLeave.Add(tblml);
                                        LogLeavesHistory(1, "1", "Casuval Leave Added For the " + tbluser.FirstName + " " + tbluser.LastName + " on " + DateTime.Now.ToString(), tbluser.CustomUserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
                                        tbl_ManageLeave tblml1 = new tbl_ManageLeave();
                                        tblml1.AvailableLeave = 1;
                                        tblml1.CountOfLeave = 1;
                                        tblml1.CreatedBy = "Admin";
                                        tblml1.CreatedOn = DateTime.Now;
                                        tblml1.DepartmentId = tbluser.DepartmentId;
                                        tblml1.DepartmentName = tbluser.DepartmentName;
                                        tblml1.ExpireDate = DateTime.Now.AddDays(31);
                                        tblml1.IsActive = true;
                                        tblml1.LeaveTypeId = 4;
                                        tblml1.LeaveTypeName = "Sick Leave";
                                        tblml1.LocationId = tbluser.LocationId;
                                        tblml1.LocationName = tbluser.LocationName;
                                        tblml1.ModifiedBy = "Admin";
                                        tblml1.ModifiedOn = DateTime.Now;
                                        tblml1.UserId = tbluser.CustomUserId;
                                        tblml1.UserName = tbluser.FirstName + " " + tbluser.LastName;
                                        myapp.tbl_ManageLeave.Add(tblml1);
                                        myapp.SaveChanges();
                                        LogLeavesHistory(1, "4", "Sick Leave Added For the " + tbluser.FirstName + " " + tbluser.LastName + " on " + DateTime.Now.ToString(), tbluser.CustomUserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
                                    }
                                }
                                else
                                {
                                    tbl_ManageLeave tblml = new tbl_ManageLeave();
                                    tblml.AvailableLeave = 33;
                                    tblml.CountOfLeave = 33;
                                    tblml.CreatedBy = "Admin";
                                    tblml.CreatedOn = DateTime.Now;
                                    tblml.DepartmentId = tbluser.DepartmentId;
                                    tblml.DepartmentName = tbluser.DepartmentName;
                                    tblml.ExpireDate = DateTime.Now.AddYears(1);
                                    tblml.IsActive = true;
                                    tblml.LeaveTypeId = 1;
                                    tblml.LeaveTypeName = "Casuval Leave";
                                    tblml.LocationId = tbluser.LocationId;
                                    tblml.LocationName = tbluser.LocationName;
                                    tblml.ModifiedBy = "Admin";
                                    tblml.ModifiedOn = DateTime.Now;
                                    tblml.UserId = tbluser.CustomUserId;
                                    tblml.UserName = tbluser.FirstName + " " + tbluser.LastName;
                                    myapp.tbl_ManageLeave.Add(tblml);
                                    LogLeavesHistory(33, "1", "Casuval Leave Added For the " + tbluser.FirstName + " " + tbluser.LastName + " on " + DateTime.Now.ToString(), tbluser.CustomUserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
                                }
                                var rpt = GetReportingMgr(tbluser.CustomUserId, DateTime.Now, DateTime.Now);

                                if (rpt != null && rpt != "")
                                {
                                    var Managerinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == rpt);
                                    if (Managerinfo.EmailId != null && Managerinfo.EmailId != "")
                                    {
                                        SendAnEmail cm = new SendAnEmail();
                                        MailModel mailmodel = new MailModel();
                                        mailmodel.fromemail = "Leave@hospitals.com";
                                        mailmodel.toemail = Managerinfo.EmailId;
                                        string body = "";
                                        body += "<h2><span style='color: rgb(85, 85, 85); font - family: Calibri; font - size: 15px; '>Dear Manager,</span></h2>";
                                        body += "<p style='box - sizing: border - box; margin: 15px 0px; color: rgb(85, 85, 85); font - family: Calibri; font - size: 15px; '>Welcome aboard our team! A new person Joined in our team please find details below.</p>";
                                        body += "<p style='box - sizing: border - box; margin: 15px 0px; color: rgb(85, 85, 85); font - family: Calibri; font - size: 15px; '>Employee Details are</p>";
                                        body += "<table cellpadding='1' cellspacing='1' style='width:500px;'><tbody>   ";
                                        body += " <tr> <td style='border:solid 1px #eee;'>Employee Id</td><td style='border: solid 1px #eee;'>" + tbluser.CustomUserId + "</td></tr>";
                                        body += " <tr> <td style='border:solid 1px #eee;'>Employee Name</td><td style='border: solid 1px #eee;'>" + tbluser.FirstName + "</td></tr>";
                                        body += " <tr> <td style='border:solid 1px #eee;'>Designation</td><td style='border: solid 1px #eee;'>" + tbluser.Designation + "</td></tr>";
                                        body += " <tr> <td style='border:solid 1px #eee;'>Intranet Login</td><td style='border: solid 1px #eee;'>Please contact Hr</td></tr>";
                                        //body += " <tr> <td style='border:solid 1px #eee;'>Employee Id</td><td style='border: solid 1px #eee;'>{empid}</td></tr>";

                                        body += "<p style='box -sizing: border-box; margin: 15px 0px; color: rgb(85, 85, 85); font-family: Calibri; font-size: 15px;'>Sincerely,</p>";
                                        body += "<h2><span style='color: rgb(85, 85, 85); font-family: Calibri; font-size: 15px;'>HR Team &nbsp;</span></h2>";
                                        mailmodel.subject = "New Employee " + tbluser.FirstName + " " + tbluser.LastName + " joined";
                                        mailmodel.body = body;
                                        mailmodel.filepath = "";
                                        mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                                        mailmodel.fromname = "Employee Joined Notification";
                                        mailmodel.ccemail = "";
                                        cm.SendEmail(mailmodel);
                                    }
                                }
                                bodyofemail += "<tr><td>Status </td><td>Success</td></tr>";
                                bodyofemail += "</table>";
                            }
                        }
                        catch (Exception ex)
                        {
                            bodyofemail += "<tr><td>Status </td><td>Error +" + ex.Message + "</td></tr>";
                            bodyofemail += "</table>";
                        }
                        //Response.Write("</table>");  
                    }
                    mailmodel1.body = bodyofemail;
                    cm1.SendEmail(mailmodel1);
                    //Response.Write("</table>");  
                }
                else
                {
                    //Response.Write("No Data In DataBase");  
                }
            }
            catch (Exception ex)
            {
                Messagestatus = "Error " + ex.Message;
            }            
            con.Close();
            return Messagestatus;
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
        public string Getdesignation()
        {
            string message = "Success";
            string oradb = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = " + sid + ")));Password=" + password + ";User ID=" + user;
            OracleConnection con = new OracleConnection(oradb);
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select * from designation";
                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //Response.Write("<table border='1'>");  
                    //Response.Write("<tr><th>Name</th><th>Roll No</th></tr>"); 
                    List<designation> listemp = new List<designation>();
                    while (dr.Read())
                    {
                        designation obj = new designation();
                        obj.designationid = dr["designationid"] != null ? dr["designationid"].ToString() : "";
                        obj.deptid = dr["deptid"] != null ? dr["deptid"].ToString() : "";
                        obj.designationnm = dr["designationnm"] != null ? dr["designationnm"].ToString() : "";
                        listemp.Add(obj);
                    }
                    foreach (var l in listemp)
                    {
                        string name = l.designationnm.ToLower().Replace(" ", "");
                        var checkexists = myapp.tbl_MasterEmployeeDesignation.Where(d => d.Designation_Name.ToLower().Replace(" ", "") == name).ToList();
                        if (checkexists.Count > 0)
                        {
                            checkexists[0].designationid = l.designationid;
                            myapp.SaveChanges();
                        }
                        else
                        {
                            tbl_MasterEmployeeDesignation model = new tbl_MasterEmployeeDesignation();
                            model.CreatedBy = "Admin";
                            model.CreatedDateTime = DateTime.Now;
                            model.CreatedOn = DateTime.Now.ToString("dd/MM/yyyy");
                            model.designationid = l.designationid;
                            model.Designation_Name = l.designationnm;
                            model.Record_Status = true;
                            myapp.tbl_MasterEmployeeDesignation.Add(model);
                            myapp.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = "Error " + ex.Message;
            }
            con.Close();
            return message;
        }
        public string Getsubdepts()
        {
            string message = "Success";
            string oradb = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = " + sid + ")));Password=" + password + ";User ID=" + user;
            OracleConnection con = new OracleConnection(oradb);
            OracleCommand cmd = new OracleCommand();
            try
            {
                cmd.CommandText = "select * from subdepts";
                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //Response.Write("<table border='1'>");  
                    //Response.Write("<tr><th>Name</th><th>Roll No</th></tr>"); 
                    List<subdepts> listemp = new List<subdepts>();

                    while (dr.Read())
                    {
                        subdepts obj = new subdepts();
                        obj.sdeptid = dr["sdeptid"] != null ? dr["sdeptid"].ToString() : "";
                        obj.deptid = dr["deptid"] != null ? dr["deptid"].ToString() : "";
                        obj.subdeptnm = dr["subdeptnm"] != null ? dr["subdeptnm"].ToString() : "";
                        listemp.Add(obj);
                    }
                    foreach (var l in listemp)
                    {
                        string name = l.subdeptnm.ToLower().Replace(" ", "");
                        var checkexists = myapp.tbl_Oracle_subdepts.Where(d => d.subdeptnm.ToLower().Replace(" ", "") == name).ToList();
                        if (checkexists.Count == 0)
                        {
                            tbl_Oracle_subdepts model = new tbl_Oracle_subdepts();
                            model.sdeptid = l.sdeptid;
                            model.deptid = l.deptid;
                            model.subdeptnm = l.subdeptnm;
                            myapp.tbl_Oracle_subdepts.Add(model);
                            myapp.SaveChanges();
                        }

                    }

                }
            }
            catch (Exception ex)
            {
                message = "Error " + ex.Message;
            }
            con.Close();
            return message;
        }
        public string Getcompany()
        {
            string message = "Success";
            string oradb = "Data Source=(DESCRIPTION =(ADDRESS = (PROTOCOL = TCP)(HOST = " + host + ")(PORT = " + port + "))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = " + sid + ")));Password=" + password + ";User ID=" + user;
            OracleConnection con = new OracleConnection(oradb);
            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "select * from company";
                cmd.Connection = con;
                con.Open();
                OracleDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    //Response.Write("<table border='1'>");  
                    //Response.Write("<tr><th>Name</th><th>Roll No</th></tr>"); 
                    List<company> listemp = new List<company>();

                    while (dr.Read())
                    {
                        company obj = new company();
                        obj.companyid = dr["companyid"] != null ? dr["companyid"].ToString() : "";
                        obj.companynm = dr["companynm"] != null ? dr["companynm"].ToString() : "";
                        obj.locid = dr["locid"] != null ? dr["locid"].ToString() : "";
                        listemp.Add(obj);
                    }

                    foreach (var l in listemp)
                    {
                        string name = l.companynm.ToLower().Replace(" ", "");
                        var checkexists = myapp.tbl_Location.Where(d => d.LocationName.ToLower().Replace(" ", "") == name).ToList();
                        if (checkexists.Count > 0)
                        {
                            checkexists[0].locid = l.locid;
                            checkexists[0].companyid = l.companyid;
                            myapp.SaveChanges();
                        }
                        else
                        {
                            tbl_Location model = new tbl_Location();
                            model.companyid = l.companyid;
                            model.locid = l.locid;
                            model.LocationName = l.companynm;
                            myapp.tbl_Location.Add(model);
                            myapp.SaveChanges();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                message = "Error " + ex.Message;
            }
            finally
            {
                con.Close();
            }
            return message;
        }
        CultureInfo provider = CultureInfo.InvariantCulture;
        public DateTime ConverDateStringtoDatetime(string date)
        {
            return DateTime.ParseExact(date, "dd/MM/yyyy", provider);
        }
        public string GetReportingMgr(string userid, DateTime st, DateTime ed)
        {
            var rptmgrId = "";
            var userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == userid);
            if (userinfo != null)
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
                //}
            }
            return rptmgrId;
        }
    }
}
