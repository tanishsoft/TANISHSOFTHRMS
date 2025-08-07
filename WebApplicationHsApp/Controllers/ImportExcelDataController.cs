using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;
using Microsoft.VisualBasic;
using System.IO;
using System.Data.OleDb;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Collections.Generic;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class ImportExcelDataController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        HrDataManage hrdm = new HrDataManage();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ImportExcelDataController));
        public ImportExcelDataController()
        {
        }

        public ImportExcelDataController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        [HttpPost]
        public ActionResult AddNewEmployeeLeaveCount_ImportExcel(HttpPostedFileBase ExcelFileData)
        {
            string ReturnMsg = "";
            if (ExcelFileData != null)
            {
                try
                {
                    string DateNow = DateTime.Now.ToString("dd/MM/yyyy");
                    string NewGUID = Convert.ToString(Guid.NewGuid());
                    string CurrentDateTime = DateTime.Now.ToString("dd/MM/yyyy/hh/mm/ss/tt", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                    CurrentDateTime = (CurrentDateTime.Contains("/")) ? CurrentDateTime.Replace("/", "_") : CurrentDateTime;
                    string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + "_" + CurrentDateTime + "." + Path.GetExtension(ExcelFileData.FileName);
                    var PathName = Path.Combine(Server.MapPath("~/ExcelUplodes/"), UniqueFileName);
                    ExcelFileData.SaveAs(PathName);

                    string myexceldataquery = "select LocationId,DepartmentId,UserId,LeaveTypeId,CountOfLeave,AvailableLeave from [sheet1$]";
                    //string excelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathName + ";Extended Properties=Excel 12.0;";
                    string sexcelconnectionstring = "";
                    if (PathName.Contains(".xlsx"))
                    {
                        sexcelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathName + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
                    }
                    else
                    {
                        sexcelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathName + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
                    }
                    OleDbConnection oledbconn = new OleDbConnection(sexcelconnectionstring);
                    OleDbCommand oledbcmd = new OleDbCommand(myexceldataquery, oledbconn);
                    oledbconn.Open();
                    OleDbDataReader DR = oledbcmd.ExecuteReader();

                    var LocationsList = myapp.tbl_Location.ToList();
                    var DepartmentList = myapp.tbl_Department.ToList();
                    var UserList = myapp.tbl_User.ToList();
                    var LeaveTypeList = myapp.tbl_LeaveType.ToList();
                    while (DR.Read())
                    {
                        if (DR["LocationId"] != null && DR["DepartmentId"] != null && DR["UserId"] != null && DR["LeaveTypeId"] != null && DR["CountOfLeave"] != null && DR["AvailableLeave"] != null)
                        {
                            int LocationId = Convert.ToInt32(DR["LocationId"]);
                            int DepartmentId = Convert.ToInt32(DR["DepartmentId"]);
                            string UserId = Convert.ToString(DR["UserId"]);
                            int LeaveTypeId = Convert.ToInt32(DR["LeaveTypeId"]);
                            string CountOfLeave = Convert.ToString(DR["CountOfLeave"]);
                            string AvailableLeave = Convert.ToString(DR["AvailableLeave"]);
                            tbl_ManageLeave LeaveTypeData = new tbl_ManageLeave()
                            {
                                LocationId = LocationId,
                                LocationName = LocationsList.Where(e => e.LocationId == LocationId).Select(k => k.LocationName).SingleOrDefault(),
                                DepartmentId = DepartmentId,
                                DepartmentName = DepartmentList.Where(e => e.DepartmentId == DepartmentId).Select(k => k.DepartmentName).SingleOrDefault(),
                                UserId = UserId,
                                UserName = UserList.Where(e => e.CustomUserId == UserId).Select(k => k.FirstName + " " + k.LastName).SingleOrDefault(),
                                LeaveTypeId = LeaveTypeId,
                                LeaveTypeName = LeaveTypeList.Where(e => e.LeaveTypeId == LeaveTypeId).Select(k => k.LeaveName).SingleOrDefault(),
                                CountOfLeave = Information.IsNumeric(CountOfLeave) ? Double.Parse(CountOfLeave) : 0,
                                AvailableLeave = Information.IsNumeric(AvailableLeave) ? Double.Parse(AvailableLeave) : 0,
                                IsActive = true
                            };
                            string AddedStatus = AddEmployeeLeave_ImportLeaveTypeExcel(LeaveTypeData);
                        }
                    }
                    oledbconn.Close();
                    ReturnMsg = "Employee Leave Count Successfully Uploaded";
                    if (System.IO.File.Exists(PathName))
                    {
                        System.IO.File.Delete(PathName);
                    }
                }
                catch (Exception EX)
                {
                    logger.Info(EX);
                    ReturnMsg = "Error Occured. Please Try Again Later !";
                }
            }
            TempData["UploadImportMsg"] = ReturnMsg;
            return RedirectToAction("EmployeeLeaves", "HrAdmin");
        }

        public string AddEmployeeLeave_ImportLeaveTypeExcel(tbl_ManageLeave tbll)
        {
            var list = myapp.tbl_User.Where(u => u.IsActive == true && u.DateOfJoining != null && u.CustomUserId == tbll.UserId).ToList();
            DateTime DateofJoining = list[0].DateOfJoining.Value;
            var days = (DateTime.Now - DateofJoining).TotalDays;
            int year = (int)(days / 365.25);
            if (tbll.LeaveTypeId == 5)
            {
                if (tbll.AvailableLeave.HasValue && tbll.AvailableLeave.Value > 0 && tbll.AvailableLeave <= tbll.CountOfLeave)
                {
                    double TotalLeaves = tbll.CountOfLeave.Value;
                    double Thisyearaddedleave = tbll.AvailableLeave.Value;

                    if (list.Count > 0)
                    {
                        tbl_LeaveUpdateHistory luph = new tbl_LeaveUpdateHistory();
                        //if (year == 1)
                        //{
                        AddLeavestoHistory_ImportExcel(tbll.CountOfLeave.Value, "5", "EL added", tbll.UserId, year, DateTime.Now.Month, true, false);
                        //}
                        //else
                        //{
                        //    var checkprevious = year - 1;
                        //    if (checkprevious > 1)
                        //    {
                        //        var checkcurrentleaves = tbll.CountOfLeave.Value - tbll.AvailableLeave.Value;
                        //        var eachyearleaves = checkcurrentleaves / checkprevious;
                        //        for (var i = 1; i < year; i++)
                        //        {
                        //            AddLeavestoHistory_ImportExcel(eachyearleaves, "5", "EL added", tbll.UserId, i, DateTime.Now.Month, true, false);
                        //        }
                        //        AddLeavestoHistory_ImportExcel(tbll.AvailableLeave.Value, "5", "EL added", tbll.UserId, year, DateTime.Now.Month, true, false);
                        //    }
                        //    else
                        //    {
                        //        var alvlleaves = tbll.CountOfLeave.Value - tbll.AvailableLeave.Value;
                        //        AddLeavestoHistory_ImportExcel(alvlleaves, "5", "EL added", tbll.UserId, year - 1, DateTime.Now.Month, true, false);
                        //        AddLeavestoHistory_ImportExcel(tbll.AvailableLeave.Value, "5", "EL added", tbll.UserId, year, DateTime.Now.Month, true, false);
                        //    }
                        //}
                    }
                }
            }
            else if (tbll.LeaveTypeId == 1 || tbll.LeaveTypeId == 4)
            {
                AddLeavestoHistory_ImportExcel(tbll.CountOfLeave.Value, tbll.LeaveTypeId.ToString(), tbll.LeaveTypeName, tbll.UserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
            }
            tbll.AvailableLeave = tbll.CountOfLeave;
            tbll.CreatedBy = User.Identity.Name;
            tbll.CreatedOn = DateTime.Now;
            myapp.tbl_ManageLeave.Add(tbll);
            myapp.SaveChanges();
            return "Success";
        }
        public void AddLeavestoHistory_ImportExcel(double count, string type, string remarks, string userid, int year, int month, bool isyearly, bool ismonthly)
        {
            tbl_LeaveUpdateHistory luph = new tbl_LeaveUpdateHistory();
            luph.AddedLeaves = count;
            luph.Created = DateTime.Now;
            luph.LeaveType = type;
            luph.Remarks = remarks;
            luph.UserId = userid;
            luph.Year = year;
            luph.Month = month;
            luph.IsYearly = isyearly;
            luph.IsMonthly = ismonthly;
            myapp.tbl_LeaveUpdateHistory.Add(luph);
            myapp.SaveChanges();
        }


        public async Task<ActionResult> AddNewEmployeeData_ImportExcel(HttpPostedFileBase ExcelFileData)
        {
            string ReturnMsg = "";
            if (ExcelFileData != null)
            {
                try
                {
                    string DateNow = DateTime.Now.ToString("dd/MM/yyyy");
                    string NewGUID = Convert.ToString(Guid.NewGuid());
                    string CurrentDateTime = DateTime.Now.ToString("dd/MM/yyyy/hh/mm/ss/tt", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                    CurrentDateTime = (CurrentDateTime.Contains("/")) ? CurrentDateTime.Replace("/", "_") : CurrentDateTime;
                    string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + "_" + CurrentDateTime + "." + Path.GetExtension(ExcelFileData.FileName);
                    var PathName = Path.Combine(Server.MapPath("~/ExcelUplodes/"), UniqueFileName);
                    ExcelFileData.SaveAs(PathName);

                    string myexceldataquery = "select EmpId,Name,Email,Location,Department,SubDepartment,Mobile,DateOfJoining,DateOfBirth,Gender,IsLoginRequired,EmployeeType,Designation,CL,SL,EL,ReportingManager,FatherSpouseName,AadhaarNumber,PanNumber,MaritalStatus from [sheet1$]";
                    //string excelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathName + ";Extended Properties=Excel 12.0;";
                    string sexcelconnectionstring = "";
                    if (PathName.Contains(".xlsx"))
                    {
                        sexcelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathName + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
                    }
                    else
                    {
                        sexcelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathName + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
                    }
                    OleDbConnection oledbconn = new OleDbConnection(sexcelconnectionstring);
                    OleDbCommand oledbcmd = new OleDbCommand(myexceldataquery, oledbconn);
                    oledbconn.Open();
                    OleDbDataReader DR = oledbcmd.ExecuteReader();
                    var LocationsList = myapp.tbl_Location.ToList();
                    var DepartmentList = myapp.tbl_Department.ToList();
                    var SubDeptList = myapp.tbl_subdepartment.ToList();
                    var DesigntionList = myapp.tbl_MasterEmployeeDesignation.ToList();
                    while (DR.Read())
                    {
                        string empname = Convert.ToString(DR["Name"]);
                        string empid = Convert.ToString(DR["EmpId"]);
                        if (empname != null && empname != "" && empid != null && empid != "")
                        {
                            string LocationName = Convert.ToString(DR["Location"]);
                            string DepartmentName = Convert.ToString(DR["Department"]);
                            string SubDepartmentName = Convert.ToString(DR["SubDepartment"]);
                            string DesignationName = Convert.ToString(DR["Designation"]);
                            string IsLoginReq = Convert.ToString(DR["IsLoginRequired"]);
                            int LocationID = 0, DepartmentID = 0, SubDepartmentID = 0; long DesignationID = 0;
                            tbl_User tbluser = new tbl_User();
                            if (LocationName != null && LocationName != "")
                            {
                                LocationID = int.Parse(LocationName);
                            }
                            if (DepartmentName != null && DepartmentName != "")
                            {
                                DepartmentID = int.Parse(DepartmentName);
                            }
                            if (SubDepartmentName != null && SubDepartmentName != "")
                            {
                                SubDepartmentID = int.Parse(SubDepartmentName);
                            }
                            var TempsLocationList = LocationsList.Where(e => e.LocationId == LocationID).ToList();
                            if (TempsLocationList.Count > 0)
                            {
                                tbluser.LocationId = TempsLocationList[0].LocationId;
                                tbluser.LocationName = TempsLocationList[0].LocationName;
                            }
                            var TempDeptList = DepartmentList.Where(e => e.DepartmentId == DepartmentID).ToList();
                            if (TempDeptList.Count > 0)
                            {
                                tbluser.DepartmentId = TempDeptList[0].DepartmentId;
                                tbluser.DepartmentName = TempDeptList[0].DepartmentName;
                            }

                            var TempSubDeptList = SubDeptList.Where(e => e.SubDepartmentId == SubDepartmentID).ToList();
                            if (TempSubDeptList.Count > 0)
                            {
                                tbluser.SubDepartmentId = TempSubDeptList[0].SubDepartmentId;
                                tbluser.SubDepartmentName = TempSubDeptList[0].Name;
                            }
                            if (DesignationName != null && DesignationName != "")
                            {
                                DesignationName = DesignationName.Trim();
                                var TempDesignationList = DesigntionList.Where(e => e.Designation_Name == DesignationName).ToList();
                                if (TempDesignationList.Count > 0)
                                {
                                    tbluser.DesignationID = TempDesignationList[0].ID;
                                    tbluser.Designation = TempDesignationList[0].Designation_Name;
                                }
                            }

                            tbluser.FirstName = Convert.ToString(DR["Name"]);
                            tbluser.CustomUserId = Convert.ToString(DR["EmpId"]);
                            if (DR["DateOfBirth"] != null && DR["DateOfBirth"] != "")
                                tbluser.DateOfBirth = ProjectConvert.ConverDateStringtoDatetimeMobile(Convert.ToString(DR["DateOfBirth"]));
                            if (DR["DateOfJoining"] != null)
                                tbluser.DateOfJoining = ProjectConvert.ConverDateStringtoDatetimeMobile(Convert.ToString(DR["DateOfJoining"]));
                            tbluser.EmailId = Convert.ToString(DR["Email"]);
                            tbluser.PersonalEmail = Convert.ToString(DR["Email"]);
                            tbluser.PhoneNumber = Convert.ToString(DR["Mobile"]);
                            tbluser.Gender = Convert.ToString(DR["Gender"]);
                            tbluser.UserType = Convert.ToString(DR["EmployeeType"]);
                            tbluser.IsActive = true;
                            tbluser.CreatedBy = User.Identity.Name;
                            tbluser.CreatedOn = DateTime.Now;
                            if (IsLoginReq == "Yes")
                            {
                                tbluser.ChangePassword = true;
                            }
                            tbluser.aadharCard = Convert.ToString(DR["AadhaarNumber"]);
                            tbluser.PanCard = Convert.ToString(DR["PanNumber"]);
                            tbluser.FatherSpouseName = Convert.ToString(DR["FatherSpouseName"]);
                            tbluser.MaritalStatus = Convert.ToString(DR["MaritalStatus"]);
                            tbluser.ReportingManagerId = Convert.ToInt32(DR["ReportingManager"]);

                            //ReportingManager,FatherSpouseName,MaritalStatus
                            var ccheckuserexitst = myapp.tbl_User.Where(t => t.CustomUserId == tbluser.CustomUserId).ToList();
                            if (ccheckuserexitst.Count == 0)
                            {
                                try
                                {
                                    tbluser.EmpId = int.Parse(tbluser.CustomUserId);
                                    myapp.tbl_User.Add(tbluser);
                                    myapp.SaveChanges();
                                    string Password = "";
                                    if (IsLoginReq == "Yes")
                                    {
                                        Password = "Fh@123";
                                        string EmailId = tbluser.EmailId;
                                        string UserId = tbluser.CustomUserId;

                                        if (EmailId == null || EmailId == "")
                                        {
                                            EmailId = UserId + "@fernandez.foundation";
                                        }
                                        var user = new ApplicationUser { UserName = UserId, Email = EmailId };
                                        var result = await UserManager.CreateAsync(user, Password);
                                        logger.Info(result.Succeeded);
                                        if (result.Errors.Count() > 0)
                                        {
                                            foreach (var v in result.Errors)
                                            {
                                                logger.Info(v);
                                            }
                                        }
                                        if (result.Succeeded)
                                        {
                                            if (tbluser.UserType == "Head of Head" || tbluser.UserType == "HOD")
                                            {
                                                List<string> roleslist = new List<string>();
                                                roleslist.Add("Employee");
                                                roleslist.Add("DepartmentManager");
                                                UserManager.AddToRoles(user.Id, roleslist.ToArray());
                                            }
                                            else
                                            {
                                                UserManager.AddToRole(user.Id, "Employee");
                                            }
                                        }
                                    }
                                    //var rpt = hrdm.GetReportingMgr(tbluser.CustomUserId, DateTime.Now, DateTime.Now);
                                    //if (rpt != null && rpt != "")
                                    //{
                                    //    var Managerinfo = myapp.tbl_User.Where(a => a.CustomUserId == rpt).ToList();
                                    //    if (Managerinfo.Count > 0)
                                    //    {
                                    //        if (Managerinfo[0].EmailId != null && Managerinfo[0].EmailId != "")
                                    //        {
                                    //            CustomModel cm = new CustomModel();
                                    //            MailModel mailmodel = new MailModel();
                                    //            EmailTeamplates emailtemp = new EmailTeamplates();
                                    //            mailmodel.fromemail = "Leave@hospitals.com";
                                    //            mailmodel.toemail = Managerinfo[0].EmailId;
                                    //            mailmodel.subject = "New Employee " + tbluser.FirstName + " " + tbluser.LastName + " joined";
                                    //            mailmodel.body = emailtemp.NewEmployeeBodyTemplate(tbluser.FirstName + " " + tbluser.LastName, tbluser.CustomUserId.Replace("FH_", ""), tbluser.CustomUserId, Password, Managerinfo[0].FirstName + " " + Managerinfo[0].LastName, tbluser.Designation);
                                    //            mailmodel.filepath = "";
                                    //            mailmodel.username = Managerinfo[0].FirstName + " " + Managerinfo[0].LastName;
                                    //            mailmodel.fromname = "Employee Joined Notification";
                                    //            mailmodel.ccemail = "";
                                    //            cm.SendEmail(mailmodel);
                                    //        }
                                    //    }
                                    //}
                                    if (tbluser.UserType == "Head of Head" || tbluser.UserType == "HOD")
                                    {
                                        var itm = new tbl_ReportingManager();
                                        itm.LocationId = tbluser.LocationId;
                                        itm.LocationName = tbluser.LocationName;
                                        itm.DepartmentId = tbluser.DepartmentId;
                                        itm.DepartmentName = tbluser.DepartmentName;
                                        itm.SubDepartmentId = tbluser.SubDepartmentId;
                                        itm.SubDepartmentName = tbluser.SubDepartmentName;
                                        itm.UserId = tbluser.CustomUserId;
                                        itm.UserName = tbluser.FirstName;
                                        if (tbluser.UserType == "Head of Head")
                                        {
                                            itm.IsHodOfHod = true;
                                            itm.IsHod = false;
                                        }
                                        else
                                        {
                                            itm.IsHod = true;
                                            itm.IsHodOfHod = false;
                                        }
                                        itm.IsActive = true;
                                        myapp.tbl_ReportingManager.Add(itm);
                                        myapp.SaveChanges();
                                    }
                                    //if (DR["CL"] != null && DR["CL"] != "")
                                    //{
                                    //    tbl_ManageLeave tblcl = new tbl_ManageLeave();
                                    //    tblcl.LocationName = tbluser.LocationName;
                                    //    tblcl.LocationId = tbluser.LocationId;
                                    //    tblcl.DepartmentName = tbluser.DepartmentName;
                                    //    tblcl.DepartmentId = tbluser.DepartmentId;
                                    //    tblcl.UserName = tbluser.FirstName + " " + tbluser.LastName;
                                    //    tblcl.UserId = tbluser.CustomUserId;
                                    //    tblcl.LeaveTypeName = "Casuval";
                                    //    tblcl.LeaveTypeId = 1;
                                    //    if (DR["CL"] != null && DR["CL"] != "")
                                    //        tblcl.CountOfLeave = Convert.ToDouble(DR["CL"]);
                                    //    else
                                    //        tblcl.CountOfLeave = 0;
                                    //    //ExpireDate: $("#ExpireDate").val(),
                                    //    if (DR["CL"] != null && DR["CL"] != "")
                                    //        tblcl.AvailableLeave = Convert.ToDouble(DR["CL"]);
                                    //    else
                                    //        tblcl.AvailableLeave = 0;
                                    //    tblcl.IsActive = true;
                                    //    AddEmployeeLeave_ImportLeaveTypeExcel(tblcl);
                                    //}
                                    //if (DR["SL"] != null && DR["SL"] != "")
                                    //{
                                    //    tbl_ManageLeave tblcl = new tbl_ManageLeave();
                                    //    tblcl.LocationName = tbluser.LocationName;
                                    //    tblcl.LocationId = tbluser.LocationId;
                                    //    tblcl.DepartmentName = tbluser.DepartmentName;
                                    //    tblcl.DepartmentId = tbluser.DepartmentId;
                                    //    tblcl.UserName = tbluser.FirstName + " " + tbluser.LastName;
                                    //    tblcl.UserId = tbluser.CustomUserId;
                                    //    tblcl.LeaveTypeName = "Sick Leave";
                                    //    tblcl.LeaveTypeId = 4;
                                    //    if (DR["SL"] != null && DR["SL"] != "")
                                    //        tblcl.CountOfLeave = Convert.ToDouble(DR["SL"]);
                                    //    else
                                    //        tblcl.CountOfLeave = 0;
                                    //    //ExpireDate: $("#ExpireDate").val(),
                                    //    if (DR["SL"] != null && DR["SL"] != "")
                                    //        tblcl.AvailableLeave = Convert.ToDouble(DR["SL"]);
                                    //    else
                                    //        tblcl.AvailableLeave = 0;
                                    //    tblcl.IsActive = true;
                                    //    AddEmployeeLeave_ImportLeaveTypeExcel(tblcl);
                                    //}
                                    //if (DR["EL"] != null && DR["EL"] != "")
                                    //{
                                    //    tbl_ManageLeave tblcl = new tbl_ManageLeave();
                                    //    tblcl.LocationName = tbluser.LocationName;
                                    //    tblcl.LocationId = tbluser.LocationId;
                                    //    tblcl.DepartmentName = tbluser.DepartmentName;
                                    //    tblcl.DepartmentId = tbluser.DepartmentId;
                                    //    tblcl.UserName = tbluser.FirstName + " " + tbluser.LastName;
                                    //    tblcl.UserId = tbluser.CustomUserId;
                                    //    tblcl.LeaveTypeName = "Earned";
                                    //    tblcl.LeaveTypeId = 5;
                                    //    if (DR["EL"] != null && DR["EL"] != "")
                                    //        tblcl.CountOfLeave = Convert.ToDouble(DR["EL"]);
                                    //    else
                                    //        tblcl.CountOfLeave = 0;
                                    //    //ExpireDate: $("#ExpireDate").val(),
                                    //    tblcl.AvailableLeave = tblcl.CountOfLeave;
                                    //    tblcl.IsActive = true;
                                    //    AddEmployeeLeave_ImportLeaveTypeExcel(tblcl);
                                    //}
                                }
                                catch
                                {

                                }
                            }
                        }
                    }
                    oledbconn.Close();
                    ReturnMsg = "Successfully Uploaded";
                    if (System.IO.File.Exists(PathName))
                    {
                        System.IO.File.Delete(PathName);
                    }
                }
                catch (Exception EX)
                {
                    logger.Info(EX);
                    ReturnMsg = "Error Occured. Please Try Again Later !";
                }
            }
            TempData["UploadImportMsg"] = ReturnMsg;
            return RedirectToAction("EmployeeLeaves", "HrAdmin");
        }

        public async Task<ActionResult> AddNewEmployeeData_ImportExcel_Latest(HttpPostedFileBase ExcelFileData)
        {
            string ReturnMsg = "";
            if (ExcelFileData != null)
            {
                try
                {
                    string DateNow = DateTime.Now.ToString("dd/MM/yyyy");
                    string NewGUID = Convert.ToString(Guid.NewGuid());
                    string CurrentDateTime = DateTime.Now.ToString("dd/MM/yyyy/hh/mm/ss/tt", System.Globalization.DateTimeFormatInfo.InvariantInfo);
                    CurrentDateTime = (CurrentDateTime.Contains("/")) ? CurrentDateTime.Replace("/", "_") : CurrentDateTime;
                    string UniqueFileName = NewGUID.Replace("-", "_").ToUpper() + "_" + CurrentDateTime + "." + Path.GetExtension(ExcelFileData.FileName);
                    var PathName = Path.Combine(Server.MapPath("~/ExcelUplodes/"), UniqueFileName);
                    ExcelFileData.SaveAs(PathName);

                    string myexceldataquery = "select Id,Name,CL,SL,EL from [sheet1$]";
                    //string excelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathName + ";Extended Properties=Excel 12.0;";
                    string sexcelconnectionstring = "";
                    if (PathName.Contains(".xlsx"))
                    {
                        sexcelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathName + ";Extended Properties=\"Excel 12.0 Xml;HDR=YES;IMEX=1\";";
                    }
                    else
                    {
                        sexcelconnectionstring = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + PathName + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=1\";";
                    }
                    OleDbConnection oledbconn = new OleDbConnection(sexcelconnectionstring);
                    OleDbCommand oledbcmd = new OleDbCommand(myexceldataquery, oledbconn);
                    oledbconn.Open();
                    OleDbDataReader DR = oledbcmd.ExecuteReader();
                    while (DR.Read())
                    {
                        string empname = Convert.ToString(DR["Name"]);
                        string empid = Convert.ToString(DR["Id"]);
                        if (empname != null && empname != "" && empid != null && empid != "")
                        {
                            string userid = empid;
                            var tbluser = myapp.tbl_User.Where(us => us.CustomUserId == userid).SingleOrDefault();
                            if (tbluser != null)
                            {
                                if (DR["CL"] != null && DR["CL"] != "")
                                {
                                    tbl_ManageLeave tblcl = new tbl_ManageLeave();
                                    tblcl.LocationName = tbluser.LocationName;
                                    tblcl.LocationId = tbluser.LocationId;
                                    tblcl.DepartmentName = tbluser.DepartmentName;
                                    tblcl.DepartmentId = tbluser.DepartmentId;
                                    tblcl.UserName = tbluser.FirstName + " " + tbluser.LastName;
                                    tblcl.UserId = tbluser.CustomUserId;
                                    tblcl.LeaveTypeName = "Casuval";
                                    tblcl.LeaveTypeId = 1;
                                    if (DR["CL"] != null && DR["CL"] != "")
                                        tblcl.CountOfLeave = Convert.ToDouble(DR["CL"]);
                                    else
                                        tblcl.CountOfLeave = 0;
                                    //ExpireDate: $("#ExpireDate").val(),
                                    if (DR["CL"] != null && DR["CL"] != "")
                                        tblcl.AvailableLeave = Convert.ToDouble(DR["CL"]);
                                    else
                                        tblcl.AvailableLeave = 0;
                                    tblcl.IsActive = true;
                                    AddEmployeeLeave_ImportLeaveTypeExcel(tblcl);
                                }
                                if (DR["SL"] != null && DR["SL"] != "")
                                {
                                    tbl_ManageLeave tblcl = new tbl_ManageLeave();
                                    tblcl.LocationName = tbluser.LocationName;
                                    tblcl.LocationId = tbluser.LocationId;
                                    tblcl.DepartmentName = tbluser.DepartmentName;
                                    tblcl.DepartmentId = tbluser.DepartmentId;
                                    tblcl.UserName = tbluser.FirstName + " " + tbluser.LastName;
                                    tblcl.UserId = tbluser.CustomUserId;
                                    tblcl.LeaveTypeName = "Sick Leave";
                                    tblcl.LeaveTypeId = 4;
                                    if (DR["SL"] != null && DR["SL"] != "")
                                        tblcl.CountOfLeave = Convert.ToDouble(DR["SL"]);
                                    else
                                        tblcl.CountOfLeave = 0;
                                    //ExpireDate: $("#ExpireDate").val(),
                                    if (DR["SL"] != null && DR["SL"] != "")
                                        tblcl.AvailableLeave = Convert.ToDouble(DR["SL"]);
                                    else
                                        tblcl.AvailableLeave = 0;
                                    tblcl.IsActive = true;
                                    AddEmployeeLeave_ImportLeaveTypeExcel(tblcl);
                                }
                                if (DR["EL"] != null && DR["EL"] != "")
                                {
                                    tbl_ManageLeave tblcl = new tbl_ManageLeave();
                                    tblcl.LocationName = tbluser.LocationName;
                                    tblcl.LocationId = tbluser.LocationId;
                                    tblcl.DepartmentName = tbluser.DepartmentName;
                                    tblcl.DepartmentId = tbluser.DepartmentId;
                                    tblcl.UserName = tbluser.FirstName + " " + tbluser.LastName;
                                    tblcl.UserId = tbluser.CustomUserId;
                                    tblcl.LeaveTypeName = "Earned";
                                    tblcl.LeaveTypeId = 5;
                                    if (DR["EL"] != null && DR["EL"] != "")
                                        tblcl.CountOfLeave = Convert.ToDouble(DR["EL"]);
                                    else
                                        tblcl.CountOfLeave = 0;
                                    //ExpireDate: $("#ExpireDate").val(),
                                    tblcl.AvailableLeave = tblcl.CountOfLeave;
                                    tblcl.IsActive = true;
                                    AddEmployeeLeave_ImportLeaveTypeExcel(tblcl);
                                }
                            }
                        }
                    }
                    oledbconn.Close();
                    ReturnMsg = "Successfully Uploaded";
                    if (System.IO.File.Exists(PathName))
                    {
                        System.IO.File.Delete(PathName);
                    }
                }
                catch (Exception EX)
                {
                    logger.Info(EX);
                    ReturnMsg = "Error Occured. Please Try Again Later !";
                }
            }
            TempData["UploadImportMsg"] = ReturnMsg;
            return RedirectToAction("EmployeeLeaves", "HrAdmin");
        }
    }
}