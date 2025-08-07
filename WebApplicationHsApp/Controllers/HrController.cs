using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;
using WebApplicationHsApp.Models.MobileApi;

namespace WebApplicationHsApp.Controllers
{
    [Authorize(Roles = "Admin,Employee,Doctor")]
    public class HrController : Controller
    {
        public string message = "";
        public double CurrentLeave = 0;

        // GET: Hr
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        private HrDataManage hrdm = new HrDataManage();
        private readonly CultureInfo provider = CultureInfo.InvariantCulture;
        private log4net.ILog logger = log4net.LogManager.GetLogger(typeof(HrController));
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult LeaveManagement()
        {
            DateTime JoiningDate = DateTime.Now;
            List<tbl_User> UserList = myapp.tbl_User.Where(e => e.EmailId == User.Identity.Name).ToList();
            if (UserList.Count > 0)
            {
                JoiningDate = Convert.ToDateTime(UserList[0].DateOfJoining);
            }
            JoiningDate = JoiningDate.AddDays(30);
            ViewBag.EmpJoiningDateAfterOneMonth = JoiningDate.ToString("MM,dd,yyyy");

            return View();
        }
        #region EmployeeDetails
        public ActionResult ViewEmployeeDetails()
        {
            return View();
        }
        public ActionResult Employeedetails()
        {
            return View();
        }

        private static readonly CultureInfo pr = CultureInfo.InvariantCulture;
        public static DateTime ConverDateStringtoDatetime(string date)
        {
            return DateTime.ParseExact(date, "dd-MM-yyyy", pr);
        }
        public JsonResult SaveEmployeeDetails(EmployeeDetailsViewModal Details)
        {
            List<tbl_EmployeeDetails> cat = myapp.tbl_EmployeeDetails.Where(l => l.EmpCode == Details.EmpCode || l.EmpId == Details.EmpId).ToList();
            if (cat.Count > 0)
            {
                cat[0].EmpCode = Details.EmpCode;
                cat[0].EmpName = Details.EmpName;
                cat[0].EmpLocationId = Details.EmpLocationId;
                cat[0].EmpDepartmentId = Details.EmpDepartmentId;
                cat[0].Gender = Details.Gender;
                cat[0].EmpStatus = Details.EmpStatus;
                if (Details.EmpDOB != null)
                {
                    cat[0].EmpDOB = ConverDateStringtoDatetime(Details.EmpDOB);
                }

                cat[0].EmpAge = Details.EmpAge;
                cat[0].EmpSpouseName = Details.EmpSpouseName;
                cat[0].EmpSpouseRelation = Details.EmpSpouseRelation;
                cat[0].EmpSpouseGender = Details.EmpSpouseGender;
                cat[0].EmpSpouseAge = Details.EmpSpouseAge;
                if (Details.EmpSpouseDOB != null)
                {
                    cat[0].EmpSpouseDOB = ConverDateStringtoDatetime(Details.EmpSpouseDOB);
                }

                cat[0].EmpDependantName = Details.EmpDependantName;
                cat[0].EmpDependantRelation = Details.EmpDependantRelation;
                cat[0].EmpDependantGender = Details.EmpDependantGender;
                cat[0].EmpDependantAge = Details.EmpDependantAge;
                if (Details.EmpDependantDOB != null)
                {
                    cat[0].EmpDependantDOB = ConverDateStringtoDatetime(Details.EmpDependantDOB);
                }

                cat[0].EmpChild1Age = Details.EmpChild1Age;
                cat[0].EmpChild1Relation = Details.EmpChild1Relation;
                cat[0].EmpChild1Name = Details.EmpChild1Name;
                cat[0].EmpChild1Gender = Details.EmpChild1Gender;
                if (Details.EmpChild1DOB != null)
                {
                    cat[0].EmpChild1DOB = ConverDateStringtoDatetime(Details.EmpChild1DOB);
                }

                cat[0].EmpChild2Age = Details.EmpChild2Age;
                cat[0].EmpChild2Name = Details.EmpChild2Name;
                cat[0].EmpChild2Relation = Details.EmpChild2Relation;
                cat[0].EmpChild2Gender = Details.EmpChild2Gender;
                if (Details.EmpChild2DOB != null)
                {
                    cat[0].EmpChild2DOB = ConverDateStringtoDatetime(Details.EmpChild2DOB);
                }

                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
                cat[0].EmpRemarks = Details.EmpRemarks;
                myapp.SaveChanges();
            }
            else
            {
                tbl_EmployeeDetails Emp = new tbl_EmployeeDetails
                {
                    EmpCode = Details.EmpCode,
                    EmpName = Details.EmpName,
                    EmpLocationId = Details.EmpLocationId,
                    EmpDepartmentId = Details.EmpDepartmentId,
                    Gender = Details.Gender,
                    EmpStatus = Details.EmpStatus
                };
                if (Details.EmpDOB != null)
                {
                    Emp.EmpDOB = ConverDateStringtoDatetime(Details.EmpDOB);
                }

                Emp.EmpAge = Details.EmpAge;
                Emp.EmpSpouseName = Details.EmpSpouseName;
                Emp.EmpSpouseRelation = Details.EmpSpouseRelation;
                Emp.EmpSpouseGender = Details.EmpSpouseGender;
                Emp.EmpSpouseAge = Details.EmpSpouseAge;
                if (Details.EmpSpouseDOB != null)
                {
                    Emp.EmpSpouseDOB = ConverDateStringtoDatetime(Details.EmpSpouseDOB);
                }

                Emp.EmpDependantName = Details.EmpDependantName;
                Emp.EmpDependantRelation = Details.EmpDependantRelation;
                Emp.EmpDependantGender = Details.EmpDependantGender;
                Emp.EmpDependantAge = Details.EmpDependantAge;
                if (Details.EmpDependantDOB != null)
                {
                    Emp.EmpDependantDOB = ConverDateStringtoDatetime(Details.EmpDependantDOB);
                }

                Emp.EmpChild1Age = Details.EmpChild1Age;
                Emp.EmpChild1Name = Details.EmpChild1Name;
                Emp.EmpChild1Relation = Details.EmpChild1Relation;
                Emp.EmpChild1Gender = Details.EmpChild1Gender;
                if (Details.EmpChild1DOB != null)
                {
                    Emp.EmpChild1DOB = ConverDateStringtoDatetime(Details.EmpChild1DOB);
                }

                Emp.EmpChild2Age = Details.EmpChild2Age;
                Emp.EmpChild2Name = Details.EmpChild2Name;
                Emp.EmpChild2Relation = Details.EmpChild2Relation;
                Emp.EmpChild2Gender = Details.EmpChild2Gender;
                if (Details.EmpChild2DOB != null)
                {
                    Emp.EmpChild2DOB = ConverDateStringtoDatetime(Details.EmpChild2DOB);
                }

                Emp.CreatedBy = User.Identity.Name;
                Emp.CreatedOn = DateTime.Now;
                Emp.EmpRemarks = Details.EmpRemarks;
                myapp.tbl_EmployeeDetails.Add(Emp);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEmployeeDetails(int EmpId)
        {
            List<tbl_EmployeeDetails> cat = new List<tbl_EmployeeDetails>();
            if (EmpId != -1)
            {
                cat = myapp.tbl_EmployeeDetails.Where(l => l.EmpId == EmpId).ToList();
            }
            else
            {
                string userid = Convert.ToString(User.Identity.Name.ToLower().Replace("fh_", ""));
                cat = myapp.tbl_EmployeeDetails.Where(l => l.EmpCode == userid).ToList();
            }

            if (cat.Count > 0)
            {
                EmployeeDetailsViewModal Emp = new EmployeeDetailsViewModal
                {
                    EmpCode = cat[0].EmpCode,
                    EmpName = cat[0].EmpName,
                    EmpLocationId = cat[0].EmpLocationId,
                    EmpDepartmentId = cat[0].EmpDepartmentId,
                    Gender = cat[0].Gender,
                    EmpStatus = cat[0].EmpStatus
                };
                if (cat[0].EmpDOB != null)
                {
                    Emp.EmpDOB = cat[0].EmpDOB.Value.ToString("dd-MM-yyyy");
                }

                Emp.EmpAge = cat[0].EmpAge;
                Emp.EmpSpouseName = cat[0].EmpSpouseName;
                Emp.EmpSpouseGender = cat[0].EmpSpouseGender;
                Emp.EmpSpouseRelation = cat[0].EmpSpouseRelation;
                Emp.EmpSpouseAge = cat[0].EmpSpouseAge;
                if (cat[0].EmpSpouseDOB != null)
                {
                    Emp.EmpSpouseDOB = cat[0].EmpSpouseDOB.Value.ToString("dd-MM-yyyy");
                }

                Emp.EmpDependantName = cat[0].EmpDependantName;
                Emp.EmpDependantRelation = cat[0].EmpDependantRelation;
                Emp.EmpDependantGender = cat[0].EmpDependantGender;
                Emp.EmpDependantAge = cat[0].EmpDependantAge;
                if (cat[0].EmpDependantDOB != null)
                {
                    Emp.EmpDependantDOB = cat[0].EmpDependantDOB.Value.ToString("dd-MM-yyyy");
                }

                Emp.EmpChild1Age = cat[0].EmpChild1Age;
                Emp.EmpChild1Name = cat[0].EmpChild1Name;
                Emp.EmpChild1Gender = cat[0].EmpChild1Gender;
                Emp.EmpChild1Relation = cat[0].EmpChild1Relation;
                if (cat[0].EmpChild1DOB != null)
                {
                    Emp.EmpChild1DOB = cat[0].EmpChild1DOB.Value.ToString("dd-MM-yyyy");
                }

                Emp.EmpChild2Age = cat[0].EmpChild2Age;
                Emp.EmpChild2Name = cat[0].EmpChild2Name;
                Emp.EmpChild2Gender = cat[0].EmpChild2Gender;
                Emp.EmpChild2Relation = cat[0].EmpChild2Relation;
                if (cat[0].EmpChild2DOB != null)
                {
                    Emp.EmpChild2DOB = cat[0].EmpChild2DOB.Value.ToString("dd-MM-yyyy");
                }

                Emp.EmpRemarks = cat[0].EmpRemarks;
                return Json(Emp, JsonRequestBehavior.AllowGet);
            }
            else
            {
                tbl_User u = myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).SingleOrDefault();
                UserViewModel emp = new UserViewModel
                {
                    CustomUserId = Convert.ToString(User.Identity.Name.ToLower().Replace("fh_", "")),
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    LocationName = u.LocationName,
                    DepartmentName = u.DepartmentName,
                    DateOfBirth = u.DateOfBirth.Value.ToString("dd-MM-yyyy"),
                    Gender = u.Gender
                };
                return Json(emp, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AjaxGetEmpDetails(JQueryDataTableParamModel param)

        {

            List<tbl_EmployeeDetails> query = (from d in myapp.tbl_EmployeeDetails
                                               select d).ToList();
            IEnumerable<tbl_EmployeeDetails> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                    .Where(c => c.EmpId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.EmpCode != null && c.EmpCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.EmpName != null && c.EmpName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.EmpLocationId != null && c.EmpLocationId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.EmpDepartmentId != null && c.EmpDepartmentId.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }

            IEnumerable<tbl_EmployeeDetails> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            IEnumerable<object[]> result = from c in displayedCompanies
                                           select new object[] {
                            c.EmpId,c.EmpCode,c.EmpName,c.EmpLocationId,c.EmpDepartmentId,
                              c.EmpDOB.HasValue? c.EmpDOB.Value.ToString("dd/MM/yyyy"):"",
                             c.Gender==null?c.Gender:"",c.EmpAge,
                            c.EmpSpouseName,c.EmpSpouseGender==null?c.EmpSpouseGender:"" ,
                             c.EmpSpouseDOB.HasValue? c.EmpSpouseDOB.Value.ToString("dd/MM/yyyy"):"",
                             c.EmpSpouseAge,
                            c.EmpChild1Name,c.EmpChild1Gender==null?c.EmpChild1Gender:"",
                            c.EmpChild1DOB.HasValue? c.EmpChild1DOB.Value.ToString("dd/MM/yyyy"):"",
                             c.EmpChild1Age,
                            c.EmpChild2Name
                           //,c.EmpChild2Gender,c.EmpChild2DOB.Value.ToString("dd/mm/yyyy")
                           ,c.EmpChild2Age
                           ,c.EmpRemarks
                           ,c.EmpId.ToString()
                           };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelEmpDetails()
        {
            List<tbl_EmployeeDetails> Result = myapp.tbl_EmployeeDetails.ToList();

            List<EmployeeDetailsViewModal> M = new List<EmployeeDetailsViewModal>();

            DataTable products = new System.Data.DataTable("Insurance");

            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Employee Code", typeof(string));
            products.Columns.Add("Employee Name", typeof(string));
            products.Columns.Add("Employee Department", typeof(string));

            products.Columns.Add("Employee Location", typeof(string));
            products.Columns.Add("Employee Data of Birth", typeof(string));
            products.Columns.Add("Employee Age", typeof(string));
            products.Columns.Add("Employee Gender", typeof(string));
            products.Columns.Add("Employee Status", typeof(string));
            products.Columns.Add("Spouse Name", typeof(string));
            products.Columns.Add("Spouse Gender", typeof(string));
            products.Columns.Add("Spouse DOB", typeof(string));
            products.Columns.Add("Spouse Age", typeof(string));
            products.Columns.Add("Spouse Relation", typeof(string));
            products.Columns.Add("Dependant Name", typeof(string));
            products.Columns.Add("Dependant Gender", typeof(string));
            products.Columns.Add("Dependant DOB", typeof(string));
            products.Columns.Add("Dependant Age", typeof(string));
            products.Columns.Add("Dependant Relation", typeof(string));
            products.Columns.Add("Child 1 Name", typeof(string));
            products.Columns.Add("Child 1 Gender", typeof(string));
            products.Columns.Add("Child 1 DOB", typeof(string));
            products.Columns.Add("Child 1 Age", typeof(string));
            products.Columns.Add("Child 2 Name", typeof(string));
            products.Columns.Add("Child 2 Gender", typeof(string));
            products.Columns.Add("Child 2 DOB", typeof(string));
            products.Columns.Add("Child 2 Age", typeof(string));
            products.Columns.Add("Remarks", typeof(string));



            foreach (tbl_EmployeeDetails item in Result)
            {
                products.Rows.Add(
               item.EmpId.ToString(),
                item.EmpCode,
                 item.EmpName,
                 item.EmpDepartmentId,
               item.EmpLocationId,
             item.EmpDOB.HasValue ? (item.EmpDOB.Value.ToString("dd/MM/yyyy")) : "",
             item.EmpAge.ToString(),
               (item.Gender == null ? " " : item.Gender),
             item.EmpStatus,
              item.EmpSpouseName,
                 (item.EmpSpouseGender == null ? " " : item.EmpSpouseGender),
               item.EmpSpouseDOB.HasValue ? (item.EmpSpouseDOB.Value.ToString("dd/MM/yyyy")) : "",
               item.EmpSpouseAge,
               item.EmpSpouseRelation,
                item.EmpDependantName,
                 (item.EmpDependantGender == null ? " " : item.EmpDependantGender),
                item.EmpDependantDOB.HasValue ? (item.EmpDependantDOB.Value.ToString("dd/MM/yyyy")) : "",
              item.EmpDependantAge,
                 item.EmpDependantRelation,
                item.EmpChild1Age,
                item.EmpChild1Name,
                (item.EmpChild1Gender == null ? " " : item.EmpChild1Gender),
                item.EmpChild1DOB.HasValue ? (item.EmpChild1DOB.Value.ToString("dd/MM/yyyy")) : "",
                item.EmpChild2Name,
                (item.EmpChild2Gender == null ? " " : item.EmpChild2Gender),
                   item.EmpChild2DOB.HasValue ? (item.EmpChild2DOB.Value.ToString("dd/MM/yyyy")) : "",
                  item.EmpChild2Age.ToString(),
                 item.EmpRemarks

                );
            }
            GridView grid = new GridView
            {
                DataSource = products
            };
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=EmployeeDetails.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        //public JsonResult UpdateRemarks(tbl_EmployeeDetails Emp)
        //{
        //    var cat = myapp.tbl_EmployeeDetails.Where(l => l.EmpId == Emp.EmpId).ToList();
        //    if (cat.Count > 0)
        //    {
        //        cat[0].EmpRemarks = Emp.EmpRemarks;
        //        myapp.SaveChanges();
        //    }
        //    return Json("Success", JsonRequestBehavior.AllowGet);
        //}
        #endregion
        public void GetLeavesCountByValidateShiftTypes(DateTime dtstart, DateTime dtenddate, string userid, bool Isfullday, bool IsCompOff, string WeeklyOffDay = "")
        {
            if (IsCompOff)
            {
                CurrentLeave = 1;
                if (!Isfullday)
                {
                    CurrentLeave = 0.5;
                }
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
                        List<tbl_Roaster> list = RoasterData.Where(l => l.ShiftDate >= dtstart && l.ShiftDate <= dtenddate).ToList();
                        int checksiftleaveorweekoff = list.Where(l => l.ShiftTypeId == 3 || l.ShiftTypeId == 4).Count();
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
        public bool CheckTheLeaveBalances(string Userid, int leavetypeid)
        {
            bool validatebalce = true;
            if (leavetypeid == 6 || leavetypeid == 1009 || leavetypeid == 7)
            {
                return validatebalce;
            }
            else
            {
                List<tbl_ManageLeave> leavelist = myapp.tbl_ManageLeave.Where(t => t.UserId == Userid && t.LeaveTypeId == leavetypeid).ToList();
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


        [HttpPost]
        public JsonResult RequestNewCompOffLeave(CompOffTypeModel cmpty)
        {

            tbl_RequestCompOffLeave ReqData = new tbl_RequestCompOffLeave
            {
                CompOffDate = cmpty.CompOffDate,
                RequestReason = cmpty.RequestReason,
                ShiftTypeId = cmpty.ShiftTypeId
            };
            //ReqData.DayWorkedType = cmpty.DayWorkedType;

            message = "";
            try
            {

                string UserId = User.Identity.Name;
                DateTime CompOffDateTime = ProjectConvert.ConverDateStringtoDatetime(cmpty.CompOffDate);
                if (User.IsInRole("Doctor"))
                {
                    List<tbl_RequestCompOffLeave> CompOffLeaveRequestList = myapp.tbl_RequestCompOffLeave.Where(e => e.UserId == UserId && e.Leave_Status != "Rejected" && e.Leave_Status != "Canceled").ToList();
                    if (CompOffLeaveRequestList.Where(e => e.CompOffDate == ReqData.CompOffDate).Count() == 0)
                    {
                        string rpt = hrdm.GetReportingMgr(User.Identity.Name, CompOffDateTime, CompOffDateTime);
                        tbl_User userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == User.Identity.Name);
                        if (userinfo != null)
                        {
                            if (CompOffDateTime > userinfo.DateOfJoining)
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
                                //countofdate[0].ShiftTypeId = ReqData.ShiftTypeId;
                                //if (ReqData.RequestReason != null && ReqData.RequestReason != "")
                                //    countofdate[0].ShiftTypeName = ReqData.RequestReason.Split(':')[1];
                                //myapp.SaveChanges();
                                message = "Success";
                            }
                            else
                            {
                                message = "A CompOff Leave Should apply after joining date.";
                            }
                        }
                    }
                    else
                    {
                        message = "A CompOff Leave Is Already Applied On This Date.";
                    }
                }
                else
                {
                    List<tbl_Roaster> shift = (from v in myapp.tbl_Roaster where v.UserId == UserId select v).ToList();
                    List<tbl_Roaster> RoasterData = myapp.tbl_Roaster.Where(e => e.UserId == UserId && e.ShiftDate != null && e.ShiftDate == CompOffDateTime).ToList();
                    if (RoasterData != null && RoasterData.Count > 0)
                    {
                        List<tbl_Roaster> countofdate = RoasterData.Where(l => l.ShiftTypeId == 3 || l.ShiftTypeId == 4).ToList();
                        if (countofdate.Count() > 0)
                        {
                            List<tbl_RequestCompOffLeave> CompOffLeaveRequestList = myapp.tbl_RequestCompOffLeave.Where(e => e.UserId == UserId && e.Leave_Status != "Rejected" && e.Leave_Status != "Canceled").ToList();
                            if (CompOffLeaveRequestList.Where(e => e.CompOffDate == ReqData.CompOffDate).Count() == 0)
                            {
                                string rpt = hrdm.GetReportingMgr(User.Identity.Name, CompOffDateTime, CompOffDateTime);
                                tbl_User userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == User.Identity.Name);
                                if (userinfo != null)
                                {
                                    if (CompOffDateTime > userinfo.DateOfJoining)
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
                                            countofdate[0].ShiftTypeName = ReqData.RequestReason.Split(':')[1];
                                        }

                                        myapp.SaveChanges();
                                        message = "Success";
                                    }
                                    else
                                    {
                                        message = "A CompOff Leave Should apply after joining date.";
                                    }
                                }
                            }
                            else
                            {
                                message = "A CompOff Leave Is Already Applied On This Date.";
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
            }
            catch (Exception EX)
            {
                message = EX.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RequestNewCompOffLeaveByManager(CompOffTypeModel cmpty)
        {

            tbl_RequestCompOffLeave ReqData = new tbl_RequestCompOffLeave
            {
                CompOffDate = cmpty.CompOffDate,
                RequestReason = cmpty.RequestReason,
                ShiftTypeId = cmpty.ShiftTypeId,
                UserId = cmpty.UserId
            };
            message = "";
            try
            {

                string UserId = cmpty.UserId;
                DateTime CompOffDateTime = ProjectConvert.ConverDateStringtoDatetime(cmpty.CompOffDate);
                List<tbl_Roaster> shift = (from v in myapp.tbl_Roaster where v.UserId == UserId select v).ToList();
                List<tbl_Roaster> RoasterData = myapp.tbl_Roaster.Where(e => e.UserId == UserId && e.ShiftDate != null && e.ShiftDate == CompOffDateTime).ToList();


                if (RoasterData != null && RoasterData.Count > 0)
                {
                    List<tbl_Roaster> countofdate = RoasterData.Where(l => l.ShiftTypeId.Value == 3 || l.ShiftTypeId.Value == 4).ToList();
                    if (countofdate.Count() > 0)
                    {

                        List<tbl_RequestCompOffLeave> CompOffLeaveRequestList = myapp.tbl_RequestCompOffLeave.Where(e => e.UserId == UserId && e.Leave_Status != "Rejected" && e.Leave_Status != "Canceled").ToList();
                        if (ReqData != null)
                        {

                            if (CompOffLeaveRequestList.Where(e => e.CompOffDate == ReqData.CompOffDate).Count() == 0)
                            {
                                string rpt = hrdm.GetReportingMgr(UserId, CompOffDateTime, CompOffDateTime);

                                tbl_User userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == UserId);
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
                                            countofdate[0].ShiftTypeName = ReqData.RequestReason.Split(':')[1];
                                        }

                                        myapp.SaveChanges();
                                        message = "Success";
                                        List<tbl_ManageLeave> cmplist = myapp.tbl_ManageLeave.Where(cl => cl.UserId == userinfo.CustomUserId && cl.LeaveTypeId == 6).ToList();
                                        if (cmplist.Count > 0)
                                        {
                                            cmplist[0].AvailableLeave = cmplist[0].AvailableLeave + 1;
                                            myapp.SaveChanges();
                                        }
                                        else
                                        {
                                            tbl_ManageLeave model = new tbl_ManageLeave
                                            {
                                                AvailableLeave = 1,
                                                CountOfLeave = 1,
                                                CreatedBy = User.Identity.Name,
                                                CreatedOn = DateAndTime.Now,
                                                LeaveTypeId = 6,
                                                LeaveTypeName = "Comp Off",
                                                LocationId = userinfo.LocationId,
                                                LocationName = userinfo.LocationName,
                                                UserId = userinfo.CustomUserId,
                                                UserName = userinfo.FirstName,
                                                ModifiedBy = User.Identity.Name,
                                                ModifiedOn = DateAndTime.Now,
                                                IsActive = true,
                                                DepartmentId = userinfo.DepartmentId,
                                                DepartmentName = userinfo.DepartmentName
                                            };
                                            myapp.tbl_ManageLeave.Add(model);
                                            myapp.SaveChanges();
                                        }
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
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRequestedCompOffListToUserByStatus(string Status)
        {
            List<tbl_RequestCompOffLeave> ReturnData = new List<tbl_RequestCompOffLeave>();
            try
            {
                string UserId = User.Identity.Name;
                ReturnData = myapp.tbl_RequestCompOffLeave.Where(e => e.UserId == UserId).ToList();
                //if (!String.IsNullOrEmpty(Status))
                //{
                //    ReturnData = ReturnData.Where(e => e.Leave_Status == Status).ToList();
                //}
            }
            catch (Exception)
            {
            }
            return Json(ReturnData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetRequestedCompOffListToManagerToApproveOrRejectByStatus(string Status)
        {
            List<tbl_RequestCompOffLeave> ReturnData = new List<tbl_RequestCompOffLeave>();
            try
            {
                string UserId = User.Identity.Name;
                ReturnData = myapp.tbl_RequestCompOffLeave.Where(e => e.CompOffLeave_Approver_1 == UserId || e.CompOffLeave_Approver_2 == UserId).ToList();
                if (!string.IsNullOrEmpty(Status))
                {
                    ReturnData = ReturnData.Where(e => e.Leave_Status == Status).ToList();


                }
            }
            catch (Exception)
            {
            }
            return Json(ReturnData, JsonRequestBehavior.AllowGet);
        }


        public JsonResult AcceptOrRejectRequestedCompOffListByStatus(long ID, string Status)
        {
            message = "";
            try
            {
                string UserId = User.Identity.Name;
                List<tbl_RequestCompOffLeave> ReturnData = myapp.tbl_RequestCompOffLeave.Where(e => e.CompOffLeave_Approver_1 == UserId || e.CompOffLeave_Approver_2 == UserId).ToList();
                if (!string.IsNullOrEmpty(Status) && Information.IsNumeric(ID))
                {
                    ReturnData = ReturnData.Where(e => e.ID == ID).ToList();
                    if (ReturnData.Count > 0)
                    {
                        string userid = ReturnData[0].UserId;
                        ReturnData[0].Leave_Status = Status;
                        ReturnData[0].IsApproved_Manager = (Status == "Approved") ? true : false;
                        myapp.SaveChanges();
                        tbl_User userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == userid);
                        message = "CompOff Leave " + Status + " Successfully";
                        if (Status == "Approved" && ReturnData[0].ShiftTypeId != null && ReturnData[0].ShiftTypeId > 0)
                        {
                            List<tbl_ManageLeave> cmplist = myapp.tbl_ManageLeave.Where(cl => cl.UserId == userinfo.CustomUserId && cl.LeaveTypeId == 6).ToList();
                            if (cmplist.Count > 0)
                            {
                                cmplist[0].AvailableLeave = cmplist[0].AvailableLeave + 1;
                                myapp.SaveChanges();
                            }
                            else
                            {
                                tbl_ManageLeave model = new tbl_ManageLeave
                                {
                                    AvailableLeave = 1,
                                    CountOfLeave = 1,
                                    CreatedBy = User.Identity.Name,
                                    CreatedOn = DateAndTime.Now,
                                    LeaveTypeId = 6,
                                    LeaveTypeName = "Comp Off",
                                    LocationId = userinfo.LocationId,
                                    LocationName = userinfo.LocationName,
                                    UserId = userinfo.CustomUserId,
                                    UserName = userinfo.FirstName,
                                    ModifiedBy = User.Identity.Name,
                                    ModifiedOn = DateAndTime.Now,
                                    IsActive = true,
                                    DepartmentId = userinfo.DepartmentId,
                                    DepartmentName = userinfo.DepartmentName
                                };
                                myapp.tbl_ManageLeave.Add(model);
                                myapp.SaveChanges();
                            }
                        }
                        else
                        {

                            DateTime Dt = ReturnData[0].CompOffDateTime.Value;

                            List<tbl_Roaster> uplist = (from l in myapp.tbl_Roaster
                                                        where l.UserId == userid && l.ShiftDate.Value == Dt
                                                        select l).ToList();
                            if (uplist.Count > 0)
                            {
                                uplist[0].ShiftTypeId = 3;

                                uplist[0].ShiftTypeName = "Holiday";
                                myapp.SaveChanges();
                            }
                            //var cmplist = myapp.tbl_ManageLeave.Where(cl => cl.UserId == userinfo.CustomUserId && cl.LeaveTypeId == 6).ToList();
                            //if (cmplist.Count > 0)
                            //{
                            //    cmplist[0].AvailableLeave = cmplist[0].AvailableLeave - 1;
                            //    myapp.SaveChanges();
                            //}

                        }
                    }
                }
            }
            catch (Exception EX)
            {
                message = EX.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCountOfMMedicalHarLeave(LeaveApply model)
        {
            ConvertLeaveObj clo = new ConvertLeaveObj();
            tbl_Leave tbll = clo.Convert(model);
            GetLeavesCountByValidateShiftTypes(tbll.LeaveFromDate.Value, tbll.LeaveTodate.Value, tbll.UserId, tbll.IsFullday.Value, tbll.IsCompOff.Value);
            return Json(CurrentLeave, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckAnyDepartmentPersonOnLeave(LeaveApply model)
        {
            ConvertLeaveObj clo = new ConvertLeaveObj();
            tbl_Leave tbll = clo.Convert(model);
            tbll.UserId = User.Identity.Name;
            tbl_User userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == User.Identity.Name);
            if (userinfo != null)
            {
                tbll.UserName = userinfo.FirstName + " " + userinfo.LastName;
                tbll.LocationId = userinfo.LocationId;
                tbll.LocationName = userinfo.LocationName;
                tbll.DepartmentId = userinfo.DepartmentId;
                tbll.DepartmentName = userinfo.DepartmentName;
            }

            List<tbl_Leave> listofleaves = myapp.tbl_Leave.Where(l => l.LocationId == tbll.LocationId && l.DepartmentId == tbll.DepartmentId).ToList();
            var employees = (from l in listofleaves
                             where l.LeaveFromDate >= tbll.LeaveFromDate && l.LeaveTodate <= tbll.LeaveTodate
                             && l.LeaveTodate >= tbll.LeaveFromDate && l.LeaveFromDate <= tbll.LeaveTodate
                             select new
                             {
                                 Name = l.UserName,
                                 UserId = l.UserId,
                                 LeaveFromDate = l.LeaveFromDate.Value.ToString("dd/MM/yyyy"),
                                 LeaveTodate = l.LeaveTodate.Value.ToString("dd/MM/yyyy"),
                                 Status = l.LeaveStatus,
                                 Reason = l.ReasonForLeave
                             }).ToList();

            if (employees.Count == 0)
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(employees, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult LeaveManagement(LeaveApply model)
        {
            ConvertLeaveObj clo = new ConvertLeaveObj();
            tbl_Leave tbll = clo.Convert(model);
            if (tbll.LeaveFromDate != null && tbll.LeaveTodate != null)
            {
                message = "";
                tbll.UserId = User.Identity.Name;
                tbll.WeeklyOffDay = model.WeeklyOffDay;
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
                        string rpt = hrdm.GetReportingMgr(User.Identity.Name, tbll.LeaveFromDate.Value, tbll.LeaveTodate.Value);
                        tbl_User userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == User.Identity.Name);

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
                                        List<tbl_ReportingManager> data = myapp.tbl_ReportingManager.Where(a => a.UserId == tbll.UserId).ToList();
                                        if (data.Count() > 0)
                                        {
                                            foreach (tbl_ReportingManager dts in data)
                                            {
                                                dts.Emp_UserId = UserIdofEmployeeIfManagerLeave;
                                                dts.Is_OnLeave = true;
                                                dts.FromDate = tbll.LeaveFromDate.Value;
                                                dts.ToDate = tbll.LeaveTodate.Value;
                                            }
                                            myapp.SaveChanges();

                                            // add role 

                                            List<AspNetUser> checkrole = myapp.AspNetUsers.Where(l => l.UserName == UserIdofEmployeeIfManagerLeave).ToList();
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
                                myapp.tbl_Leave.Add(tbll);
                                myapp.SaveChanges();

                                List<tbl_ManageLeave> leavelist = myapp.tbl_ManageLeave.Where(t => t.UserId == tbll.UserId && t.LeaveTypeId == tbll.LeaveTypeId).ToList();

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
                                        LogLeavesHistory(-0.5, tbll.LeaveTypeId.ToString(), "Debit", tbll.UserId, DateTime.Now.Year, DateTime.Now.Month, false, false);
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
                                        LogLeavesHistory(-CurrentLeave, tbll.LeaveTypeId.ToString(), "Debit", tbll.UserId, DateTime.Now.Year, DateTime.Now.Month, false, false);
                                    }
                                    myapp.SaveChanges();
                                }

                                if (rpt != null && rpt != "")
                                {
                                    string cc = string.Empty;
                                    tbl_User Managerinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == rpt);
                                    string body = string.Empty;
                                    string emailidofemp = "";
                                    if (isrepotmanager)
                                    {
                                        List<tbl_User> listofuser = myapp.tbl_User.Where(a => a.CustomUserId == UserIdofEmployeeIfManagerLeave).ToList();

                                        using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/HodRequestForLeave.html")))
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
                                        using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/RequestForLeave.html")))
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
                                    MailModel mailmodel = new MailModel
                                    {
                                        fromemail = "Leave@hospitals.com",
                                        toemail = Managerinfo.EmailId
                                    };
                                    if (emailidofemp != null && emailidofemp != "")
                                    {
                                        mailmodel.toemail = mailmodel.toemail + "," + emailidofemp;
                                    }
                                    mailmodel.subject = Subject;
                                    if (User.Identity.Name.ToLower() == "fh_1")
                                    {
                                        string bodyhtml = "<p>This is to communicate and inform</p>";
                                        bodyhtml += "<p>I(Dr Evita Fernandez) will be on leave " + model.LeaveFromDate + " to " + model.LeaveTodate + ".</p>";
                                        bodyhtml += "<p>&nbsp;</p>";
                                        bodyhtml += "<p>Thanking you.</p>";
                                        bodyhtml += "<p>&nbsp;</p>";
                                        bodyhtml += "<p>Regards,</p>";
                                        bodyhtml += "<p>Dr Evita Fernandez.</p>";
                                        bodyhtml += "<p>Managing Director.</p>";
                                        body = bodyhtml;
                                        cc = "drpramod@fernandez.foundation,gowri_ch@fernandez.foundation";
                                    }
                                    mailmodel.body = body;
                                    mailmodel.filepath = "";
                                    mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                                    mailmodel.fromname = "Leave Application";
                                    mailmodel.ccemail = cc;
                                    cm.SendEmail(mailmodel);
                                    NotificationSendModel notify = new NotificationSendModel();
                                    List<NotificationUserModel> devideidstosend = (
                                                           from d in myapp.tbl_DeviceInfo
                                                           where Managerinfo.CustomUserId == d.UserId
                                                           select new NotificationUserModel
                                                           {
                                                               DeviceId = d.DeviceId,
                                                               Userid = d.UserId
                                                           }).Distinct().ToList();
                                    string response = notify.SendNotificationToSome("ApproveLeavesPage", Subject, User.Identity.Name, "Approve Leave", devideidstosend);
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
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public bool CheckEmployeeIsonLeave(string userid, DateTime Fromdate, DateTime Todate)
        {
            List<tbl_Leave> list = myapp.tbl_Leave.Where(l => l.UserId == userid && l.LeaveStatus == "Pending" && l.LeaveStatus == "Approved" &&
                        l.LeaveFromDate.Value >= Fromdate &&
                        l.LeaveTodate.Value <= Todate).ToList();
            if (list.Count > 0)
            {
                return false;
            }
            return true;
        }
        public JsonResult GetComoffLeavesAvailable()
        {
            double balance = 0;
            balance = GetCompoffLeaveBalancetotal(User.Identity.Name);
            return Json(balance, JsonRequestBehavior.AllowGet);
        }
        public double GetCompoffLeaveBalancetotal(string username)
        {
            double balance = 0;
            List<tbl_ManageLeave> CompOffReqList = myapp.tbl_ManageLeave.Where(l => l.UserId == username && l.LeaveTypeId == 6).ToList();
            if (CompOffReqList.Count > 0)
            {
                balance = CompOffReqList[0].AvailableLeave.Value;
            }


            return balance;
        }
        public void LogLeavesHistory(double AddedLeaves, string LeaveType, string Remarks, string CustomUserId, int year, int month, bool Isyearly, bool Ismonthly)
        {
            tbl_LeaveUpdateHistory luph = new tbl_LeaveUpdateHistory
            {
                AddedLeaves = AddedLeaves,
                Created = DateTime.Now,
                LeaveType = LeaveType,
                Remarks = Remarks,
                UserId = CustomUserId,
                Year = year,
                Month = month,
                IsYearly = Isyearly,
                IsMonthly = Ismonthly
            };
            myapp.tbl_LeaveUpdateHistory.Add(luph);
            myapp.SaveChanges();
        }
        public bool validateLeave(tbl_Leave tblleave)
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
            tbl_User doj = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == tblleave.UserId);
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
                        double leavebanalcecmp = GetCompoffLeaveBalancetotal(tblleave.UserId);
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
                List<tbl_RequestCompOffLeave> CompOffReqList = myapp.tbl_RequestCompOffLeave.Where(e => e.UserId == UserId && (e.Leave_Status == "Approved" || e.IsApproved_Manager == true)).ToList();
                //
                if (CompOffReqList.Count > 0)
                {
                    double balance = GetCompoffLeaveBalancetotal(tblleave.UserId);

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
                logger.Info("Shift UserId --" + tblleave.UserId);
                logger.Info("Shift From Date --" + tblleave.LeaveFromDate.Value);
                logger.Info("Shift To Date --" + tblleave.LeaveTodate.Value);
                if (!User.IsInRole("Doctor"))
                {
                    //Check Duty Roaster for all the days.

                    DateTime shiftsdate = tblleave.LeaveFromDate.Value;
                    DateTime shiftedate = tblleave.LeaveTodate.Value;

                    while (shiftsdate < shiftedate)
                    {

                        List<tbl_Roaster> RoasterData = myapp.tbl_Roaster.Where(e => e.UserId == tblleave.UserId && e.ShiftDate != null && e.ShiftDate.Value == shiftsdate).ToList();
                        if (RoasterData.Count > 0)
                        {
                            List<tbl_Roaster> checkleave = RoasterData.Where(r => r.ShiftTypeId == 3).ToList();
                            if (checkleave.Count > 0 && CurrentLeave == 0)
                            {
                                ReturnStatus = false;
                                message = "Please check the date you are trying to apply on Leave";
                            }
                            logger.Info("Shift Dates --" + RoasterData[0].ShiftDate.Value.ToString("dd/MM/yyyy"));
                        }
                        else
                        {
                            ReturnStatus = false;
                            message = "Shift Data Is Not Added For This Employee";
                            break;
                        }
                        shiftsdate = shiftsdate.AddDays(1);
                    }


                }
            }
            if (ReturnStatus)
            {
                string strleaveuserid = tblleave.UserId;
                // check if already appliyed leave
                if (tblleave.IsFullday.Value)
                {
                    List<tbl_Leave> checkaaplcount = myapp.tbl_Leave.Where(l => l.UserId == strleaveuserid &&
                        l.LeaveStatus != "Cancelled" && l.LeaveStatus != "Reject" &&
              ((tblleave.LeaveFromDate.Value >= l.LeaveFromDate.Value && tblleave.LeaveFromDate.Value <= l.LeaveTodate.Value)
                       || (tblleave.LeaveTodate.Value >= l.LeaveFromDate.Value && tblleave.LeaveTodate.Value <= l.LeaveTodate.Value))
                   ).ToList();


                    if (checkaaplcount.Count > 0)
                    {
                        message = "you have already applied leave on these dates";
                        ReturnStatus = false;
                    }
                }
                else
                {
                    List<tbl_Leave> checkaaplcount = myapp.tbl_Leave.Where(l => l.UserId == strleaveuserid &&
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
                            List<tbl_Leave> valco = checkaaplcount.Where(l => l.LeaveTypeId != 6).ToList();
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
                        double count1 = CurrentLeave;
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
                            List<tbl_Leave> PrevLeaveList = myapp.tbl_Leave.Where(e => e.UserId == tblleave.UserId).ToList();
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



        public double GetDateDifference(DateTime start, DateTime end)
        {
            TimeSpan difference = end - start;
            return difference.TotalDays + 1;
        }
        public ActionResult EmployeePolicy()
        {
            return View();
        }
        public ActionResult ViewDutyRoster()
        {
            return View();
        }
        public ActionResult EditDutyRoster()
        {
            return View();
        }
        public ActionResult MyTimeSheet()
        {
            return View();
        }
        public ActionResult MyTimeTrack()
        {
            return View();
        }
        public ActionResult MyLeaves()
        {
            return View();
        }
        public string GetLeaveType(bool? isfullday)
        {
            if (isfullday.HasValue && isfullday.Value)
            {
                return "Full Day";
            }
            else if (isfullday.HasValue && !isfullday.Value)
            {
                return "Half Day";
            }
            else
            {
                return "";
            }
        }
        public ActionResult EditLeave(int id)
        {

            tbl_Leave leve = (from v in myapp.tbl_Leave where v.LeaveId == id select v).Single();
            return Json(leve, JsonRequestBehavior.AllowGet);

        }
        public ActionResult GetAllDepartmentsandemployees(string dept)
        {

            List<tbl_User> list = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);

        }
        public ActionResult AjaxMyLeavesView(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                List<tbl_Leave> leaves = myapp.tbl_Leave.Where(l => l.UserId == User.Identity.Name).ToList();
                List<LeaveViewModels> query = (from c in leaves
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
                                                   //Level2Approver = app2.FirstName + " " + app2.LastName
                                               }).ToList();
                query = query.OrderByDescending(t => t.LeaveId).ToList();

                IEnumerable<LeaveViewModels> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.LeaveTypeName != null && c.LeaveTypeName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.Level1Approver != null && c.Level1Approver.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   // ||
                                   //c.Level2Approver != null && c.Level2Approver.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.LeaveStatus != null && c.LeaveStatus.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filteredCompanies = query;
                }
                IEnumerable<LeaveViewModels> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies

                                               select new[] {
                                               c.LeaveTypeName,
                                               c.IsFullday,
                                               c.LeaveSessionDay,
                                                 c.LeaveFromDate,
                                               c.LeaveTodate,
                                               c.TotalLeaves.ToString(),
                                               //c.LeaveFromDate.HasValue?   c.LeaveFromDate.Value.ToShortDateString():"",
                                               //c.LeaveTodate.HasValue?     c.LeaveTodate.Value.ToShortDateString():"",
                                               c.Level1Approver ,
                                               //c.Level2Approver,
                                                c.LeaveStatus,
                                                c.LeaveCreatedOn,
                              c.LeaveId.ToString()};
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
        public ActionResult AjaxAllRequestCompOffView(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {

                List<tbl_RequestCompOffLeave> query = myapp.tbl_RequestCompOffLeave.ToList();
                if (param.locationid != null && param.locationid > 0)
                {
                    query = query.Where(l => l.LocationId == param.locationid).ToList();
                }
                if (param.departmentid != null && param.departmentid > 0)
                {
                    query = query.Where(l => l.DepartmentId == param.departmentid).ToList();
                }
                if (param.Emp != null && param.Emp != "")
                {
                    query = query.Where(l => l.UserId == param.Emp).ToList();
                }
                if (param.fromdate != null && param.fromdate != "")
                {
                    DateTime dt = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                    query = query.Where(q => q.CompOffDateTime >= dt).ToList();
                }
                if (param.todate != null && param.todate != "")
                {
                    DateTime dt = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                    query = query.Where(q => q.CompOffDateTime <= dt).ToList();
                }
                IEnumerable<tbl_RequestCompOffLeave> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.CompOffDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.UserName != null && c.UserName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   // ||
                                   //c.Level2Approver != null && c.Level2Approver.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filteredCompanies = query;
                }
                IEnumerable<tbl_RequestCompOffLeave> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies

                                               select new[] {
                                 c.UserId,
                                 c.UserName,
                                               c.DepartmentName,
                                               c.LocationName,
                                                 c.CompOffDateTime.Value.ToString("dd/MM/yyyy"),
                                               c.RequestReason,
                                               c.Leave_Status,
                                               c.CreatedDateTime.Value.ToString("dd/MM/yyyy")
                              //c.ID.ToString()
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
        public ActionResult AjaxMyRequestCompOffView(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                string UserId = User.Identity.Name;
                List<tbl_RequestCompOffLeave> query = myapp.tbl_RequestCompOffLeave.Where(e => e.UserId == UserId).OrderByDescending(l => l.CreatedDateTime).ToList();

                IEnumerable<tbl_RequestCompOffLeave> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.CompOffDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.UserName != null && c.UserName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   // ||
                                   //c.Level2Approver != null && c.Level2Approver.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filteredCompanies = query;
                }
                IEnumerable<tbl_RequestCompOffLeave> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies

                                               select new[] {
                                 c.UserName,
                                               c.DepartmentName,
                                               c.LocationName,
                                                 c.CompOffDateTime.Value.ToString("dd/MM/yyyy"),
                                               c.RequestReason,
                                               c.Leave_Status,
                                               c.CreatedDateTime.Value.ToString("dd/MM/yyyy"),
                              c.ID.ToString()
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
        public ActionResult AjaxMyRequestCompOffViewApproved(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                string UserId = User.Identity.Name;
                List<tbl_RequestCompOffLeave> query = myapp.tbl_RequestCompOffLeave.Where(e => e.UserId == UserId && e.Leave_Status == "Approved").ToList();

                IEnumerable<tbl_RequestCompOffLeave> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.CompOffDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.UserName != null && c.UserName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   // ||
                                   //c.Level2Approver != null && c.Level2Approver.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filteredCompanies = query;
                }
                IEnumerable<tbl_RequestCompOffLeave> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies

                                               select new[] {
                                 c.UserName,
                                 c.DepartmentName,
                                 c.LocationName,
                                 c.CompOffDateTime.Value.ToString("dd/MM/yyyy"),
                                 c.RequestReason,
                                 c.Leave_Status,
                                 c.CreatedDateTime.Value.ToString("dd/MM/yyyy")
                              //c.ID.ToString()
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
        public ActionResult AjaxMyPendingRequestCompOff(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                string UserId = User.Identity.Name;
                List<tbl_RequestCompOffLeave> query = myapp.tbl_RequestCompOffLeave.Where(e => e.Leave_Status != "Approved" && (e.CompOffLeave_Approver_1 == UserId || e.CompOffLeave_Approver_2 == UserId)).ToList();
                query = query.OrderByDescending(l => l.ID).ToList();
                IEnumerable<tbl_RequestCompOffLeave> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.CompOffDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.UserName != null && c.UserName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   // ||
                                   //c.Level2Approver != null && c.Level2Approver.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filteredCompanies = query;
                }
                IEnumerable<tbl_RequestCompOffLeave> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies

                                               select new[] {
                                 c.UserName,
                                               c.DepartmentName,
                                               c.LocationName,
                                                 c.CompOffDateTime.Value.ToString("dd/MM/yyyy"),
                                               c.RequestReason,
                                               c.Leave_Status,
                                               c.CreatedDateTime.Value.ToString("dd/MM/yyyy"),
                              c.ID.ToString()
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
        public JsonResult BulkCancelLeave(string ids, string comments)
        {
            try
            {
                if (ids != null && ids != "")
                {
                    string[] leaveid = ids.Split(',');
                    foreach (string v in leaveid)
                    {
                        if (v != null && v != "")
                        {
                            int id = int.Parse(v);
                            List<tbl_Leave> tasks = myapp.tbl_Leave.Where(t => t.LeaveId == id).ToList();
                            if (tasks.Count > 0)
                            {
                                if (tasks[0].LeaveStatus != "Cancelled" && tasks[0].LeaveStatus != "Reject" && tasks[0].LeaveStatus != "Approved")
                                {
                                    tasks[0].LeaveStatus = "Cancelled";
                                    string cusUserId = tasks[0].UserId;
                                    int Leavetypeid = tasks[0].LeaveTypeId.Value;
                                    List<tbl_ManageLeave> list = myapp.tbl_ManageLeave.Where(v1 => v1.UserId == cusUserId && v1.LeaveTypeId == Leavetypeid).ToList();
                                    if (list.Count > 0)
                                    {
                                        if (!tasks[0].IsFullday.Value)
                                        {
                                            list[0].AvailableLeave = list[0].AvailableLeave + 0.5;
                                        }
                                        else
                                        {
                                            list[0].AvailableLeave = list[0].AvailableLeave + tasks[0].LeaveCount;
                                        }
                                    }
                                    tasks[0].LeaveCount = 0;
                                    myapp.SaveChanges();
                                    if (tasks[0].LeaveTypeId == 6)
                                    {
                                        if (tasks[0].DateofAvailableCompoff != null)
                                        {
                                            DateTime Dt = tasks[0].DateofAvailableCompoff.Value.Date;
                                            string useridcurrr = tasks[0].UserId;
                                            List<tbl_ManageLeave> cmplist = myapp.tbl_ManageLeave.Where(cl => cl.UserId == useridcurrr && cl.LeaveTypeId == 6).ToList();
                                            if (cmplist.Count > 0)
                                            {
                                                cmplist[0].AvailableLeave = cmplist[0].AvailableLeave - 1;
                                                myapp.SaveChanges();
                                            }
                                        }
                                    }
                                    List<tbl_ReportingManager> listmanger = myapp.tbl_ReportingManager.Where(l => l.UserId == cusUserId).ToList();
                                    if (listmanger.Count > 0)
                                    {
                                        try
                                        {
                                            foreach (tbl_ReportingManager v1 in listmanger)
                                            {
                                                bool validate = v1.Is_OnLeave.HasValue ? v1.Is_OnLeave.Value : false;
                                                if (validate)
                                                {
                                                    if (v1.FromDate == tasks[0].LeaveFromDate && v1.ToDate == tasks[0].LeaveTodate)
                                                    {
                                                        v1.Is_OnLeave = false;
                                                        v1.FromDate = null;
                                                        v1.ToDate = null;
                                                        myapp.SaveChanges();
                                                    }
                                                }
                                            }
                                        }
                                        catch { }
                                    }
                                }

                            }
                        }
                    }
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult CancelLeave(int id)
        {
            List<tbl_Leave> tasks = myapp.tbl_Leave.Where(t => t.LeaveId == id).ToList();
            if (tasks.Count > 0)
            {
                if (tasks[0].LeaveStatus != "Cancelled" && tasks[0].LeaveStatus != "Reject" && tasks[0].LeaveStatus != "Approved")
                {
                    tasks[0].LeaveStatus = "Cancelled";
                    string cusUserId = tasks[0].UserId;
                    int Leavetypeid = tasks[0].LeaveTypeId.Value;
                    List<tbl_ManageLeave> list = myapp.tbl_ManageLeave.Where(v => v.UserId == cusUserId && v.LeaveTypeId == Leavetypeid).ToList();
                    if (list.Count > 0)
                    {
                        if (!tasks[0].IsFullday.Value)
                        {
                            list[0].AvailableLeave = list[0].AvailableLeave + 0.5;
                        }
                        else
                        {
                            list[0].AvailableLeave = list[0].AvailableLeave + tasks[0].LeaveCount;
                        }
                    }

                    tasks[0].LeaveCount = 0;
                    myapp.SaveChanges();


                    List<tbl_ReportingManager> listmanger = myapp.tbl_ReportingManager.Where(l => l.UserId == cusUserId).ToList();
                    if (listmanger.Count > 0)
                    {
                        try
                        {
                            foreach (tbl_ReportingManager v in listmanger)
                            {
                                bool validate = v.Is_OnLeave.HasValue ? v.Is_OnLeave.Value : false;
                                if (validate)
                                {
                                    if (v.FromDate == tasks[0].LeaveFromDate && v.ToDate == tasks[0].LeaveTodate)
                                    {
                                        v.Is_OnLeave = false;
                                        v.FromDate = null;
                                        v.ToDate = null;
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
                    return Json("After Leave Approved or Reject will not allow to cancel", JsonRequestBehavior.AllowGet);
                }

            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ViewLeave(int id)
        {
            LeaveViewModels model = hrdm.ViewLeave(id);
            return PartialView("_ViewLeave", model);
        }
        public JsonResult CancelCompOffrequest(int id)
        {
            List<tbl_RequestCompOffLeave> tasks = myapp.tbl_RequestCompOffLeave.Where(t => t.ID == id).ToList();
            if (tasks.Count > 0)
            {
                string statusbefore = tasks[0].Leave_Status;
                tasks[0].Leave_Status = "Canceled";
                myapp.SaveChanges();
                string userid = tasks[0].UserId;
                DateTime date = tasks[0].CompOffDateTime.Value.Date;
                List<tbl_Roaster> roster = myapp.tbl_Roaster.Where(l => l.UserId == userid && l.ShiftDate.Value == date).ToList();
                if (roster.Count > 0)
                {
                    roster[0].ShiftTypeId = 3;
                    roster[0].ShiftTypeName = "Holiday";
                    myapp.SaveChanges();
                }

                if (statusbefore == "Approved")
                {
                    List<tbl_ManageLeave> cmplist = myapp.tbl_ManageLeave.Where(cl => cl.UserId == userid && cl.LeaveTypeId == 6).ToList();
                    if (cmplist.Count > 0)
                    {
                        cmplist[0].AvailableLeave = cmplist[0].AvailableLeave - 1;
                        myapp.SaveChanges();
                    }
                }

            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApproveLeaveCount()
        {
            int query1 = (from c in myapp.tbl_Leave
                          where c.LeaveStatus == "Pending" && (c.Level1Approver == User.Identity.Name || c.Level2Approver == User.Identity.Name)
                          select c).Count();
            return Json(query1, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApprovePermissionsCount()
        {
            int tasks = (from c in myapp.tbl_Permission where c.Level1Approver == User.Identity.Name && c.Status == "Pending" select c).Count();
            return Json(tasks, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApproveRequestCompOffLeave()
        {
            int tasks = (from c in myapp.tbl_RequestCompOffLeave where c.CompOffLeave_Approver_1 == User.Identity.Name && c.Leave_Status == "Pending" select c).Count();
            return Json(tasks, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxMyApprovedView(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                List<tbl_Leave> leaves = myapp.tbl_Leave.Where(c => c.LeaveStatus == "Pending" && (c.Level1Approver == User.Identity.Name || c.Level2Approver == User.Identity.Name)).ToList();
                IEnumerable<LeaveViewModels> query1 =
                   (from c in leaves
                    join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
                    //join app2 in myapp.tbl_User on c.Level2Approver equals app2.CustomUserId

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
                        LeaveCreatedOn = c.CreatedOn.Value.ToString("dd/MM/yyyy"),
                        WeeklyOffDay = c.WeeklyOffDay
                    });

                query1 = query1.OrderByDescending(t => t.LeaveId);

                IEnumerable<LeaveViewModels> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query1
                       .Where(c => c.LeaveTypeName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.Level1Approver != null && c.Level1Approver.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.Level2Approver != null && c.Level2Approver.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.LeaveStatus != null && c.LeaveStatus.ToLower().Contains(param.sSearch.ToLower()));
                }
                else
                {
                    filteredCompanies = query1;
                }
                IEnumerable<LeaveViewModels> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies

                                               select new[] {c.LeaveId.ToString(),
                                               c.LeaveTypeName,
                                               c.IsFullday,
                                              c.LeaveSessionDay,
                                               c.LeaveFromDate,
                                               c.LeaveTodate,
                                               c.TotalLeaves.ToString() +" " +(c.WeeklyOffDay!=null ? "("+ c.WeeklyOffDay +" as Weekly Off)":""),
                                              // c.LeaveFromDate.HasValue? c.LeaveFromDate.Value.ToShortDateString():"",
                                             //  c.LeaveTodate.HasValue? c.LeaveTodate.Value.ToShortDateString():"",                                             
                                               c.UserName ,
                                               c.DepartmentName,
                                               c.LeaveStatus,
                                               c.LeaveCreatedOn,
                              c.LeaveId.ToString()};
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query1.Count(),
                    iTotalDisplayRecords = filteredCompanies.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }
        public JsonResult BulkApproveLave(string ids)
        {
            try
            {
                if (ids != null && ids != "")
                {
                    string[] leaveid = ids.Split(',');
                    foreach (string v in leaveid)
                    {
                        if (v != null && v != "")
                        {
                            int id = int.Parse(v);
                            List<tbl_Leave> tasks = myapp.tbl_Leave.Where(t => t.LeaveId == id).ToList();
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
                        }
                    }
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult ApproveLeave(int id)
        {
            ViewBag.Message = "Success";
            List<tbl_Leave> tasks = myapp.tbl_Leave.Where(t => t.LeaveId == id).ToList();
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
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public int GetCountOfleaves(DateTime? sdate, DateTime? edate)
        {
            int count = 0;
            if (sdate.Value.ToShortDateString() == edate.Value.ToShortDateString())
            {
                return 1;
            }
            else
            {
                while (sdate <= edate)
                {
                    count++;
                    sdate = sdate.Value.AddDays(1);
                }
            }
            return count;
        }
        public JsonResult BulkRejectLeave(string ids, string comments)
        {
            try
            {
                if (ids != null && ids != "")
                {
                    string[] leaveid = ids.Split(',');
                    foreach (string v in leaveid)
                    {
                        if (v != null && v != "")
                        {
                            int id = int.Parse(v);
                            List<tbl_Leave> tasks = myapp.tbl_Leave.Where(t => t.LeaveId == id).ToList();
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
                                    List<tbl_ManageLeave> list = myapp.tbl_ManageLeave.Where(v1 => v1.UserId == cususerid && v1.LeaveTypeId == Lavetypeid).ToList();
                                    if (list.Count > 0)
                                    {
                                        if (!tasks[0].IsFullday.Value)
                                        {
                                            list[0].AvailableLeave = list[0].AvailableLeave + 0.5;
                                            LogLeavesHistory(0.5, Lavetypeid.ToString(), "Credit", cususerid, DateTime.Now.Year, DateTime.Now.Month, false, false);
                                        }
                                        else
                                        {
                                            if (tasks[0].LeaveCount == null || tasks[0].LeaveCount == 0)
                                            {
                                                tasks[0].LeaveCount = 1;
                                            }
                                            list[0].AvailableLeave = list[0].AvailableLeave + tasks[0].LeaveCount;
                                            myapp.SaveChanges();
                                            LogLeavesHistory(tasks[0].LeaveCount.Value, Lavetypeid.ToString(), "Credit", cususerid, DateTime.Now.Year, DateTime.Now.Month, false, false);
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
                                    //        //var cmprequest = myapp.tbl_RequestCompOffLeave.Where(l => l.UserId == useridcurrr).ToList();
                                    //        //cmprequest = cmprequest.Where(l => l.CompOffDateTime.Value.Date == Dt.Date).ToList();
                                    //        //if (cmprequest.Count > 0)
                                    //        //{
                                    //        //    cmprequest[0].IsLeaveTaken = false;
                                    //        //    myapp.SaveChanges();
                                    //        //}
                                    //        var cmplist = myapp.tbl_ManageLeave.Where(cl => cl.UserId == useridcurrr && cl.LeaveTypeId == 6).ToList();
                                    //        if (cmplist.Count > 0)
                                    //        {
                                    //            cmplist[0].AvailableLeave = cmplist[0].AvailableLeave - 1;
                                    //            LogLeavesHistory(-1, Lavetypeid.ToString(), "Debit", cususerid, DateTime.Now.Year, DateTime.Now.Month, false, false);
                                    //            myapp.SaveChanges();
                                    //        }
                                    //    }
                                    //}
                                    List<tbl_ReportingManager> listmanger = myapp.tbl_ReportingManager.Where(l => l.UserId == cususerid).ToList();
                                    if (listmanger.Count > 0)
                                    {
                                        try
                                        {
                                            foreach (tbl_ReportingManager v2 in listmanger)
                                            {
                                                bool validate = v2.Is_OnLeave.HasValue ? v2.Is_OnLeave.Value : false;
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
                        }
                    }
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult RejectLeave(int id, string comments)
        {
            List<tbl_Leave> tasks = myapp.tbl_Leave.Where(t => t.LeaveId == id).ToList();
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
                    List<tbl_ManageLeave> list = myapp.tbl_ManageLeave.Where(v => v.UserId == cususerid && v.LeaveTypeId == Lavetypeid).ToList();
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
                    //        //var cmprequest = myapp.tbl_RequestCompOffLeave.Where(l => l.UserId == useridcurrr).ToList();
                    //        //cmprequest = cmprequest.Where(l => l.CompOffDateTime.Value.Date == Dt.Date).ToList();
                    //        //if (cmprequest.Count > 0)
                    //        //{
                    //        //    cmprequest[0].IsLeaveTaken = false;
                    //        //    myapp.SaveChanges();
                    //        //}
                    //        var cmplist = myapp.tbl_ManageLeave.Where(cl => cl.UserId == useridcurrr && cl.LeaveTypeId == 6).ToList();
                    //        if (cmplist.Count > 0)
                    //        {
                    //            cmplist[0].AvailableLeave = cmplist[0].AvailableLeave - 1;
                    //            myapp.SaveChanges();
                    //        }
                    //    }
                    //}
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult RejectLeave(int id)
        {
            List<tbl_Leave> tasks = myapp.tbl_Leave.Where(t => t.LeaveId == id).ToList();
            if (tasks.Count > 0)
            {
                if (tasks[0].LeaveStatus != "Cancelled" && tasks[0].LeaveStatus != "Reject")
                {
                    tasks[0].LeaveStatus = "Reject";
                    if (tasks[0].Level1Approver == User.Identity.Name && tasks[0].Level1Approved == null)
                    {
                        tasks[0].Level1Approved = false;
                        tasks[0].Level1ApproveComment = "";
                    }
                    else if (tasks[0].Level2Approver == User.Identity.Name && tasks[0].Level2Approved == null)
                    {
                        tasks[0].Level2Approved = false;
                        tasks[0].Level2ApproveComment = "";
                    }
                    string cususerid = tasks[0].UserId;
                    int Lavetypeid = tasks[0].LeaveTypeId.Value;
                    List<tbl_ManageLeave> list = myapp.tbl_ManageLeave.Where(v => v.UserId == cususerid && v.LeaveTypeId == Lavetypeid).ToList();
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

                    List<tbl_ReportingManager> listmanger = myapp.tbl_ReportingManager.Where(l => l.UserId == cususerid).ToList();
                    if (listmanger.Count > 0)
                    {
                        try
                        {
                            foreach (tbl_ReportingManager v in listmanger)
                            {
                                bool validate = v.Is_OnLeave.HasValue ? v.Is_OnLeave.Value : false;
                                if (validate)
                                {
                                    if (v.FromDate == tasks[0].LeaveFromDate && v.ToDate == tasks[0].LeaveTodate)
                                    {
                                        v.Is_OnLeave = false;
                                        v.FromDate = null;
                                        v.ToDate = null;
                                        myapp.SaveChanges();
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveLeavePermission(string date, string starttime, string endtime, string reason, string approval)
        {
            if (Request.IsAuthenticated)
            {
                string msg = hrdm.SaveLeavePermission(date, starttime, endtime, reason, approval, User.Identity.Name);
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }

        public ActionResult AjaxGetMyLeavePermission(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {
                List<tbl_Permission> permissions = myapp.tbl_Permission.Where(l => l.UserId == User.Identity.Name).ToList();

                List<PermissionViewModels> tasks =
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

                IEnumerable<PermissionViewModels> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = tasks
                       .Where(c => c.id.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.PermissionDate != null && c.PermissionDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                   c.StartDate != null && c.StartDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                   c.EndDate != null && c.EndDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                   c.Status != null && c.Status.ToString().ToLower().Contains(param.sSearch.ToLower())
                                     ||
                                   c.Requestapprovename != null && c.Requestapprovename.ToString().ToLower().Contains(param.sSearch.ToLower())


                               );
                }
                else
                {
                    filteredCompanies = tasks;
                }
                IEnumerable<PermissionViewModels> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies
                                               select new[] {
                                 c.PermissionDate,
                                 c.StartDate,
                                 c.EndDate,
                                 c.Requestapprovename,
                                 c.Status,
                                 c.Reason,
                                 c.CreatedOn,
                                 Convert.ToString(c.id)
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
        public ActionResult AjaxGetApprovePermission(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {

                List<tbl_Permission> query = myapp.tbl_Permission.ToList();
                if (param.locationid != null && param.locationid != 0)
                {
                    query = query.Where(q => q.LocationId == param.locationid).ToList();
                }
                if (param.departmentid != null && param.departmentid != 0)
                {
                    query = query.Where(q => q.DepartmentId == param.departmentid).ToList();
                }
                if (param.Emp != null && param.Emp != "")
                {
                    query = query.Where(q => q.UserId == param.Emp).ToList();
                }
                if (!string.IsNullOrEmpty(param.status))
                {
                    query = query.Where(q => q.Status == param.status).ToList();
                }

                IEnumerable<PermissionViewModels> tasks =
                  (from c in query.ToList()
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

                IEnumerable<PermissionViewModels> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = tasks
                       .Where(c => c.id.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.PermissionDate != null && c.PermissionDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                   c.StartDate != null && c.StartDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                   c.EndDate != null && c.EndDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                   c.Status != null && c.Status.ToString().ToLower().Contains(param.sSearch.ToLower())
                                     ||
                                   c.Requestapprovename != null && c.Requestapprovename.ToString().ToLower().Contains(param.sSearch.ToLower())
                               );
                }
                else
                {
                    filteredCompanies = tasks;
                }
                IEnumerable<PermissionViewModels> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies
                                               select new[] {
                                 c.UserName,
                                 c.DepartmentName,
                                 c.LocationName,
                                 c.PermissionDate,
                                 c.StartDate,
                                 c.EndDate,
                                 c.Requestapprovename,
                                 c.Status,
                                 c.Reason,
                                 c.CreatedOn,
                                 Convert.ToString(c.id)
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
        public ActionResult AjaxGetAllPermission(JQueryDataTableParamModel param)
        {
            if (Request.IsAuthenticated)
            {

                List<tbl_Permission> query = myapp.tbl_Permission.OrderByDescending(p => p.PermissionId).ToList();
                if (param.locationid != null && param.locationid != 0)
                {
                    query = query.Where(q => q.LocationId == param.locationid).ToList();
                }
                if (param.departmentid != null && param.departmentid != 0)
                {
                    if (param.locationid != null && param.locationid > 0)
                    {
                        query = query.Where(q => q.DepartmentId == param.departmentid).ToList();
                    }
                    else
                    {
                        List<int?> departmentid = myapp.tbl_DepartmentVsCommonDepartment.Where(d => d.DepartmentId == param.departmentid).Select(n => n.CommonDepartmentId).ToList();
                        query = (from q in query where departmentid.Contains(q.DepartmentId) select q).ToList();
                    }
                }
                if (param.Emp != null && param.Emp != "")
                {
                    query = query.Where(q => q.UserId == param.Emp).ToList();
                }
                if (!string.IsNullOrEmpty(param.status))
                {
                    query = query.Where(q => q.Status == param.status).ToList();
                }

                List<PermissionViewModels> tasks =
                  (from c in query.ToList()
                   join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
                   where app1.IsActive == true
                   //where c.Level1Approver == User.Identity.Name && c.Status == "Pending"
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
                       UserId = c.UserId

                   }).ToList();

                IEnumerable<PermissionViewModels> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = tasks
                       .Where(c => c.id.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.PermissionDate != null && c.PermissionDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                   c.StartDate != null && c.StartDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                   c.EndDate != null && c.EndDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                   c.Status != null && c.Status.ToString().ToLower().Contains(param.sSearch.ToLower())

                                    ||
                                   c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                   c.DepartmentName != null && c.DepartmentName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                     ||
                                   c.UserName != null && c.UserName.ToString().ToLower().Contains(param.sSearch.ToLower())

                                     ||
                                   c.Requestapprovename != null && c.Requestapprovename.ToString().ToLower().Contains(param.sSearch.ToLower())
                               );
                }
                else
                {
                    filteredCompanies = tasks;
                }
                IEnumerable<PermissionViewModels> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies
                                               select new[] {
                                 c.UserId.ToLower().Replace("fh_",""),
                                 c.UserName,
                                 c.DepartmentName,
                                 c.LocationName,
                                 c.PermissionDate,
                                 c.StartDate,
                                 c.EndDate,
                                 c.Requestapprovename,
                                 c.Status,
                                 c.Reason,
                                 Convert.ToString(c.id)
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

        public ActionResult ViewPermissions_ExportToExcel(string ClientId, string FromDate, string ToDate)
        {

            List<tbl_Permission> query = myapp.tbl_Permission.ToList();
            IEnumerable<PermissionViewModels> tasks =
              (from c in query
               join app1 in myapp.tbl_User on c.Level1Approver equals app1.CustomUserId
               where app1.IsActive == true
               //  where c.Level1Approver == User.Identity.Name && c.Status == "Pending"
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
                   UserName = c.UserName

               });

            DataTable products = new System.Data.DataTable("ViewPermissionsDataTable");
            // products.Rows.Add("Client  : " + list[0].ClientName + " SOA From : " + FromDate + " To : " + ToDate);
            products.Columns.Add("User Name", typeof(string));
            products.Columns.Add("Department Name", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("Start Time", typeof(string));
            products.Columns.Add("End Time", typeof(string));
            products.Columns.Add("Approve Name", typeof(string));
            products.Columns.Add("Status", typeof(string));
            products.Columns.Add("Reason", typeof(string));
            int transid = 1;
            foreach (PermissionViewModels c in tasks)
            {
                products.Rows.Add(c.UserName,
                                 c.DepartmentName,
                                 c.LocationName,
                                 c.PermissionDate,
                                 c.StartDate,
                                 c.EndDate,
                                 c.Requestapprovename,
                                 c.Status,
                                 c.Reason
                );
                transid++;
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
            string filename = "ViewPermissions_GridData.xls";
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



        [HttpPost]
        public JsonResult DeletePermissions(int id)
        {
            List<tbl_Permission> tasks = myapp.tbl_Permission.Where(t => t.PermissionId == id).ToList();
            string msg = "Permission Cancelled Successfully";
            if (tasks.Count > 0)
            {
                if (tasks[0].Level1Approved == null || tasks[0].Level1Approved == false)
                {
                    tasks[0].Status = "Cancelled";
                    myapp.SaveChanges();
                }
                else
                {
                    msg = "Permission will not Cancelled after approve";
                }
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult ApprovePermision(int id)
        {

            List<tbl_Permission> tasks = myapp.tbl_Permission.Where(t => t.PermissionId == id).ToList();
            if (tasks.Count > 0)
            {
                tasks[0].Status = "Approved";
                if (tasks[0].Level1Approver == User.Identity.Name && tasks[0].Level1Approved == null)
                {
                    tasks[0].Level1Approved = true;
                }

                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult RejectPermision(int id, string comments)
        {
            List<tbl_Permission> tasks = myapp.tbl_Permission.Where(t => t.PermissionId == id).ToList();
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
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ViewPermision(int id)
        {
            PermissionViewModels model = hrdm.ViewPermision(id);
            return PartialView("_ViewPermission", model);
        }

        [HttpPost]
        public ActionResult GetemployeeshiftsJson(string fromdate, string todate, int locationid = 0, int deptid = 0, string userid = "0")
        {
            try
            {
                if (Request.IsAuthenticated)
                {

                    DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(fromdate);
                    DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(todate);
                    List<uspGetRoster_Result> shifts = myapp.uspGetRoster(User.Identity.Name, dtfrom, dtto, locationid, deptid).ToList();
                    if (userid != null && userid != "" && userid != "0")
                    {
                        shifts = shifts.Where(l => l.EmployeeId == userid).ToList();
                    }
                    var newlist = (from l in shifts
                                   select new
                                   {
                                       EmployeeId = l.EmployeeId,
                                       Fromdate = l.Fromdate.Value.ToShortDateString(),
                                       ShiftId = l.ShiftId,
                                       ShiftTypeName = (l.CompRequest != null && l.CompRequest != "" && l.CompRequest.Contains(":") && l.CompRequest.Split(':')[1].Trim() != l.ShiftTypeName.Trim()) ? l.CompRequest.Split(':')[1] + "/" + l.ShiftTypeName.Replace("Holiday", "H").Replace("WeekOff", "WO") : l.ShiftTypeName.Replace("Holiday", "H").Replace("WeekOff", "WO"),
                                       IsHoliday = l.IsHoliday.Value,
                                       //CheckLeave = CheckIsLeaveApplied(l.UserId, l.ShiftDate.Value.Date),
                                       LeaveType = l.LeaveType,
                                       CompRequest = l.CompRequest
                                   }).ToList();
                    return Json(newlist, JsonRequestBehavior.AllowGet);

                }
                else { return RedirectToAction("Login", "Account"); }
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }
        public string CheckIsLeaveAppliedGetType(string userid, DateTime dt)
        {
            IQueryable<tbl_Leave> list = myapp.tbl_Leave.Where(u => u.UserId == userid && dt >= u.LeaveFromDate && u.LeaveTodate >= dt && u.LeaveStatus != "Cancelled" && u.LeaveStatus != "Reject");
            if (list.Count() > 0)
            {
                List<tbl_Leave> leavelist = list.Take(1).ToList();
                string typel = "";
                switch (leavelist[0].LeaveTypeId.Value)
                {
                    case 1:
                        typel = "CL";
                        break;
                    case 4:
                        typel = "SL";
                        break;
                    case 5:
                        typel = "EL";
                        break;
                    case 6:
                        typel = "CO";
                        break;
                    case 7:
                        typel = "ML";
                        break;
                    case 8:
                        typel = "PL";
                        break;
                    case 1009:
                        typel = "LOP";
                        break;
                }
                return typel;
            }
            else
            {
                return "";
            }
        }
        public bool CheckIsLeaveApplied(string userid, DateTime dt)
        {
            IQueryable<tbl_Leave> list = myapp.tbl_Leave.Where(u => u.UserId == userid && dt >= u.LeaveFromDate && u.LeaveTodate >= dt && u.LeaveStatus != "Cancelled" && u.LeaveStatus != "Reject");
            if (list.Count() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Validate is Holida 
        private readonly DateTime dtholidayfrom = ProjectConvert.ConverDateStringtoDatetime("26/01/2017");
        private readonly DateTime dtholidayfrom1 = ProjectConvert.ConverDateStringtoDatetime("01/05/2017");
        private readonly DateTime dtholidayfrom2 = ProjectConvert.ConverDateStringtoDatetime("02/06/2017");
        private readonly DateTime dtholidayfrom3 = ProjectConvert.ConverDateStringtoDatetime("15/08/2017");
        private readonly DateTime dtholidayfrom4 = ProjectConvert.ConverDateStringtoDatetime("02/10/2017");
        private readonly DateTime dtholidayfroma = ProjectConvert.ConverDateStringtoDatetime("26/01/2018");
        private readonly DateTime dtholidayfroma1 = ProjectConvert.ConverDateStringtoDatetime("01/05/2018");
        private readonly DateTime dtholidayfroma2 = ProjectConvert.ConverDateStringtoDatetime("02/06/2018");
        private readonly DateTime dtholidayfroma3 = ProjectConvert.ConverDateStringtoDatetime("15/08/2018");
        private readonly DateTime dtholidayfroma4 = ProjectConvert.ConverDateStringtoDatetime("02/10/2018");
        public bool IsHOlidayornotCheck(DateTime dt)
        {

            if (dt == dtholidayfrom || dt == dtholidayfrom1 || dt == dtholidayfrom2 || dt == dtholidayfrom3 || dt == dtholidayfrom4
                || dt == dtholidayfroma || dt == dtholidayfroma1 || dt == dtholidayfroma2 || dt == dtholidayfroma3 || dt == dtholidayfroma4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        [HttpPost]
        public ActionResult GetDeptemployeeshiftsJson(string fromdate, string todate, int locationid = 0, int deptid = 0, string userid = "0")
        {

            if (Request.IsAuthenticated)
            {
                DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(fromdate);
                DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(todate);
                List<tbl_Roaster> eshiftslist = myapp.tbl_Roaster.Where(s => s.IsActive == true && s.UserId == User.Identity.Name).ToList();
                if (locationid != 0)
                {
                    eshiftslist = eshiftslist.Where(s => s.LocationId == locationid).ToList();
                }
                if (deptid != 0)
                {
                    eshiftslist = eshiftslist.Where(s => s.DepartmentId == deptid).ToList();
                }
                if (userid != null && userid != "" && userid != "0")
                {
                    eshiftslist = eshiftslist.Where(s => s.UserId == userid).ToList();
                }
                List<tbl_Roaster> list = eshiftslist.ToList();
                List<tbl_Roaster> shifts = (from s in list
                                            where Convert.ToDateTime(s.ShiftDate) >= dtfrom && Convert.ToDateTime(s.ShiftDate) <= dtto
                                            select s).ToList();
                var newlist = (from l in shifts
                               select new
                               {
                                   EmployeeId = l.UserId,
                                   Fromdate = l.ShiftDate.Value.ToShortDateString(),
                                   ShiftId = l.ShiftTypeId,
                                   ShiftTypeName = l.ShiftTypeName

                               }).ToList();
                return Json(newlist, JsonRequestBehavior.AllowGet);

            }
            else { return RedirectToAction("Login", "Account"); }
        }
        public ActionResult GetShiftType(string id)
        {
            if (Request.IsAuthenticated)
            {
                int User_DeptID = 0, User_SubDeptID = 0;
                List<tbl_User> dept = myapp.tbl_User.Where(e => e.CustomUserId == User.Identity.Name).ToList();
                if (dept.Count > 0)
                {
                    User_DeptID = Convert.ToInt32(dept[0].DepartmentId);
                    User_SubDeptID = Convert.ToInt32(dept[0].SubDepartmentId);
                }
                List<tbl_DepartmentShifts> DeptVsShiptType = myapp.tbl_DepartmentShifts.Where(e => e.DepartmentId == User_DeptID).ToList();
                //if (User_SubDeptID > 0)
                //{
                //    DeptVsShiptType = DeptVsShiptType.Where(e => e.SubDeptID == User_SubDeptID).ToList();
                //}
                List<int> IDList = DeptVsShiptType.Select(e => Convert.ToInt32(e.ShiftTypeId)).Distinct().ToList();
                var list = (from o in myapp.tbl_ShiftType
                            where IDList.Contains(o.ShiftTypeId)
                            select new
                            {
                                id = o.ShiftTypeId,
                                Name = o.ShiftTypeName,
                                starttime = o.ShiftStartTime,
                                endtime = o.ShiftEndTime
                            }).ToList();
                list = list.Where(l => l.id != null).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }
        public List<tbl_User> GetListOfEmployees(string typeofview)
        {
            List<tbl_User> userslistnew = new List<tbl_User>();
            List<tbl_User> userslist = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            if (!User.IsInRole("Admin") && !User.IsInRole("HrAdmin"))
            {
                string Currenrid = User.Identity.Name;
                List<tbl_User> currentuser = userslist.Where(l => l.CustomUserId == Currenrid).ToList();
                if (User.IsInRole("SubDepartmentManager"))
                {
                    userslistnew = userslist.Where(l => l.LocationId == currentuser[0].LocationId && l.DepartmentId == currentuser[0].DepartmentId && l.SubDepartmentId == currentuser[0].SubDepartmentId).ToList();

                }
                else
                {
                    List<tbl_ReportingManager> reportingmgr = myapp.tbl_ReportingManager.Where(l => l.UserId == Currenrid).ToList();

                    foreach (tbl_ReportingManager v in reportingmgr)
                    {
                        if (v.IsHod.Value)
                        {
                            List<tbl_User> query = (from u in userslist
                                                    where u.LocationId == v.LocationId &&
                                                    u.DepartmentId == v.DepartmentId
                                                    && u.UserType.ToLower() == "employee"
                                                    select u).ToList();

                            userslistnew.AddRange(query);
                        }
                        else if (v.IsHodOfHod.Value)
                        {
                            List<tbl_User> query = (from u in userslist
                                                    where u.LocationId == v.LocationId &&
                                                    u.DepartmentId == v.DepartmentId
                                                    && u.UserType.ToLower() == "hod"
                                                    select u).ToList();

                            userslistnew.AddRange(query);

                        }
                        else if (v.IsManagerOfHod.Value)
                        {
                            List<tbl_User> query = (from u in userslist
                                                    where u.LocationId == v.LocationId &&
                                                    u.DepartmentId == v.DepartmentId
                                                    && u.UserType.ToLower() == "headofhod"
                                                    select u).ToList();

                            userslistnew.AddRange(query);

                        }

                    }

                    if (currentuser.Count > 0 && currentuser[0].UserType == "HOD")
                    {
                        List<tbl_User> query = (from u in userslist
                                                    //from rm in reportingmgr
                                                where u.LocationId == currentuser[0].LocationId &&
                                                u.DepartmentId == currentuser[0].DepartmentId
                                                && (u.UserType == "HOD" || u.UserType == "Employee")
                                                select u).ToList();
                        userslistnew.AddRange(query);
                    }
                    else if (currentuser.Count > 0 && currentuser[0].UserType.ToLower().Replace(" ", "") == "headofhod")
                    {
                        List<tbl_User> query = (from u in userslist
                                                    //from rm in reportingmgr
                                                where u.LocationId == currentuser[0].LocationId &&
                                                u.DepartmentId == currentuser[0].DepartmentId &&
                                                (u.UserType == "HOD" || u.UserType == "HeadofHOD")
                                                select u).ToList();
                        userslistnew.AddRange(query);

                    }
                    else if (currentuser.Count > 0 && currentuser[0].UserType.ToLower().Replace(" ", "") == "headofmanager")
                    {
                        List<tbl_User> query = (from u in userslist
                                                    //from rm in reportingmgr
                                                where (u.LocationId == currentuser[0].LocationId
                                                && u.DepartmentId == currentuser[0].DepartmentId && (u.UserType == "HeadofHOD"))

                                                select u).ToList();
                        userslistnew.AddRange(query);
                    }
                    //else if (currentuser.Count > 0 && currentuser[0].IsEmployeesReporting != null && currentuser[0].IsEmployeesReporting.Value)
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
                            List<tbl_User> query = (from u in userslist
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
                    List<tbl_User> currentuser = userslist.Where(l => l.CustomUserId == Currenrid).ToList();
                    if (User.IsInRole("SubDepartmentManager"))
                    {
                        userslistnew = userslist.Where(l => l.LocationId == currentuser[0].LocationId && l.DepartmentId == currentuser[0].DepartmentId && l.SubDepartmentId == currentuser[0].SubDepartmentId).ToList();

                    }
                    else
                    {
                        List<tbl_ReportingManager> reportingmgr = myapp.tbl_ReportingManager.Where(l => l.UserId == Currenrid).ToList();


                        foreach (tbl_ReportingManager v in reportingmgr)
                        {
                            if (v.IsHod.Value)
                            {
                                List<tbl_User> query = (from u in userslist
                                                        where u.LocationId == v.LocationId &&
                                                        u.DepartmentId == v.DepartmentId
                                                        && u.UserType.ToLower() == "employee"
                                                        select u).ToList();

                                userslistnew.AddRange(query);
                            }
                            else if (v.IsHodOfHod.Value)
                            {
                                List<tbl_User> query = (from u in userslist
                                                        where u.LocationId == v.LocationId &&
                                                        u.DepartmentId == v.DepartmentId
                                                        && u.UserType.ToLower() == "hod"
                                                        select u).ToList();

                                userslistnew.AddRange(query);

                            }
                            else if (v.IsManagerOfHod.Value)
                            {
                                List<tbl_User> query = (from u in userslist
                                                        where u.LocationId == v.LocationId &&
                                                        u.DepartmentId == v.DepartmentId
                                                        && u.UserType.ToLower() == "headofhod"
                                                        select u).ToList();

                                userslistnew.AddRange(query);

                            }

                        }

                        if (currentuser.Count > 0 && currentuser[0].UserType == "HOD")
                        {
                            List<tbl_User> query = (from u in userslist
                                                        //from rm in reportingmgr
                                                    where u.LocationId == currentuser[0].LocationId
                                                    && u.DepartmentId == currentuser[0].DepartmentId && (u.UserType == "HOD" || u.UserType == "Employee")
                                                    select u).ToList();
                            userslistnew.AddRange(query);

                        }
                        else if (currentuser.Count > 0 && currentuser[0].UserType.ToLower().Replace(" ", "") == "headofhod")
                        {
                            List<tbl_User> query = (from u in userslist
                                                        //from rm in reportingmgr
                                                    where u.LocationId == currentuser[0].LocationId &&
                                                    u.DepartmentId == currentuser[0].DepartmentId &&
                                                    (u.UserType == "HOD" || u.UserType == "HeadofHOD")
                                                    select u).ToList();
                            userslistnew.AddRange(query);

                        }
                        else if (currentuser.Count > 0 && currentuser[0].UserType.ToLower().Replace(" ", "") == "headofmanager")
                        {
                            List<tbl_User> query = (from u in userslist
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
                                List<tbl_User> query = (from u in userslist
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
                    userslistnew = userslist;
                }

            }
            userslistnew = userslistnew.Distinct().ToList();
            userslistnew = (from q in userslistnew
                            orderby int.Parse(q.CustomUserId.ToLower().Replace("fh_", ""))
                            select q).ToList();
            return userslistnew;
        }
        public JsonResult Getmydeptemps(string typeofview)
        {
            List<tbl_User> list = GetListOfEmployees(typeofview);

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveEmployeesShiftdata(string data, string fromdate, string todate)
        {
            if (Request.IsAuthenticated)
            {
                string msg = hrdm.SaveEmployeesShiftdata(data, fromdate, todate, User.Identity.Name);
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }

        public ActionResult GetMyLeavesCount()
        {
            IQueryable<tbl_ManageLeave> list = myapp.tbl_ManageLeave.Where(u => u.UserId == User.Identity.Name && u.LeaveTypeId < 7);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult InactiveEmployees()
        {
            return View();
        }
        public ActionResult AjaxGetEmployees(JQueryDataTableParamModel param)
        {

            if (Request.IsAuthenticated)
            {

                List<tbl_User> query = myapp.tbl_User.Where(a => a.IsActive == false).OrderBy(l => l.CustomUserId).ToList();

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
                                  );
                }
                else
                {
                    filteredCompanies = query;
                }
                IEnumerable<tbl_User> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies

                                               select new[] {c.CustomUserId,
                                              c.LocationName,
                                              c.FirstName+" "+c.LastName,
                                              c.CustomUserId,
                                              c.EmailId,
                                              c.PlaceAllocation,
                                              c.PhoneNumber,
                                              c.Extenstion,
                                              c.DepartmentName,
                                              c.Designation,
                                              c.ReportingManagerId.HasValue?c.ReportingManagerId.Value.ToString():"",
                                              c.CustomUserId.ToString()};
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

        public ActionResult SaveTransferEmployee(tbl_User user)
        {
            if (Request.IsAuthenticated)
            {
                string msg = hrdm.SaveTransferEmployee(user, User.Identity.Name);
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            else { return RedirectToAction("Login", "Account"); }
        }

        public JsonResult HrSaveLeaveManagement(LeaveApply model)
        {
            ConvertLeaveObj clo = new ConvertLeaveObj();
            tbl_Leave tbll = clo.Convert(model);
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
                        tbl_User userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == tbll.UserId);
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

                        myapp.tbl_Leave.Add(tbll);
                        myapp.SaveChanges();
                        List<tbl_ManageLeave> leavelist = myapp.tbl_ManageLeave.Where(t => t.UserId == tbll.UserId && t.LeaveTypeId == tbll.LeaveTypeId).ToList();

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
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteLeave(int id)
        {
            List<tbl_Leave> tasks = myapp.tbl_Leave.Where(t => t.LeaveId == id).ToList();
            if (tasks.Count > 0)
            {
                tasks[0].IsActive = false;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        public JsonResult LeaveOtherLeave(tbl_Leave tbll)
        {
            message = "";
            if (!tbll.IsCompOff.Value)
            {
                GetLeavesCountByValidateShiftTypes(tbll.LeaveFromDate.Value, tbll.LeaveTodate.Value, tbll.UserId, tbll.IsFullday.Value, tbll.IsCompOff.Value);
            }
            if (validateLeave(tbll) && CheckTheLeaveBalances(tbll.UserId, tbll.LeaveTypeId.Value))
            {
                tbll.CreatedBy = User.Identity.Name;
                tbll.CreatedOn = DateTime.Now;
                tbl_User userinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == tbll.UserId);
                if (userinfo != null)
                {
                    tbll.UserId = User.Identity.Name;
                    tbll.UserName = userinfo.FirstName + " " + userinfo.LastName;
                    tbll.LocationId = userinfo.LocationId;
                    tbll.LocationName = userinfo.LocationName;
                    tbll.DepartmentId = userinfo.DepartmentId;
                    tbll.DepartmentName = userinfo.DepartmentName;
                    tbll.LeaveStatus = "Pending";
                    tbll.IsActive = true;
                    tbll.Level1Approver = User.Identity.Name;
                    tbll.Level2Approver = User.Identity.Name;
                }
                myapp.tbl_Leave.Add(tbll);
                myapp.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LeaveReportingMgr()
        {
            return View();
        }
        [HttpPost]
        public JsonResult LeaveReportingMgr(tbl_ReportingManager tbl)
        {
            List<tbl_ReportingManager> data = myapp.tbl_ReportingManager.Where(a => a.UserId == User.Identity.Name).ToList();
            if (data.Count() > 0)
            {
                if (tbl.Is_OnLeave == true)
                {
                    data[0].Is_OnLeave = true;
                    data[0].Emp_UserId = tbl.Emp_UserId;
                }
                else
                {
                    data[0].Is_OnLeave = false;
                    data[0].Emp_UserId = null;
                }
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSpecificLeaveDataFieldsToAdmin_HRController()
        {
            List<LeaveData_RelationalClass> ReturnData = hrdm.GetSpecificLeaveDataFields(User.Identity.Name);
            return Json(ReturnData, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSpecificRoasterFieldsDataToAdmin_HRController()
        {
            List<RoasterData_RelationalClass> ReturnData = hrdm.GetRoasterSpecificData(User.Identity.Name);
            return Json(ReturnData, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public JsonResult CheckEmployeeLeaveYearWise()
        {
            logger.Info("Update Fired" + DateAndTime.Now.ToString());
            HrManuvallyLeaveUpdate hrml = new HrManuvallyLeaveUpdate();
            hrml.CheckEmployeeLeaveYearWise();
            hrml.UpdateCasuvalAndSickLeaves_New();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManagersReturnReporting()
        {
            return View();
        }

        public ActionResult AjaxRetunReportingManagers(JQueryDataTableParamModel param)
        {

            if (Request.IsAuthenticated)
            {
                tbl_User list = (from v in myapp.tbl_User where v.CustomUserId == User.Identity.Name select v).SingleOrDefault();
                List<tbl_ReportingManager> query = myapp.tbl_ReportingManager.Where(t => t.UserId == list.CustomUserId).ToList();
                IEnumerable<tbl_ReportingManager> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.ReportingManagerId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||

                                  c.UserName != null && c.UserName.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.SubDepartmentName != null && c.SubDepartmentName.ToLower().Contains(param.sSearch.ToLower())
                                    ||

                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())
                                  );
                }
                else
                {
                    filteredCompanies = query;
                }
                IEnumerable<tbl_ReportingManager> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies
                                               select new[] {
                                              c.LocationName,
                                              c.DepartmentName,
                                              c.SubDepartmentName,
                                              c.UserName,
                                              c.IsHod.HasValue?c.IsHod.Value.ToString():"",
                                              c.IsHodOfHod.HasValue?c.IsHodOfHod.Value.ToString():"",
                                              c.IsManagerOfHod.HasValue?c.IsManagerOfHod.Value.ToString():"",
                                              c.ReportingManagerId.ToString()};
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
        //Re-assigning the reporting manager to the department
        public ActionResult UpdateOfReportmenager(int ReportingManagerId)
        {
            tbl_User getlist = (from k in myapp.tbl_User where k.CustomUserId == User.Identity.Name select k).SingleOrDefault();
            List<tbl_ReportingManager> list = (from v in myapp.tbl_ReportingManager where v.UserId == getlist.CustomUserId select v).ToList();
            tbl_ReportingManager getmangerlist = (from v in myapp.tbl_ReportingManager where v.ReportingManagerId == ReportingManagerId select v).SingleOrDefault();
            if (list.Count > 0)
            {
                //if (ReportingManagerId == getmangerlist.ReportingManagerId)
                // {
                list[0].Is_OnLeave = false;
                // list[0].Emp_UserId = null;
                //list[0].IsActive = false;
                // }
            }
            else
            {
                list[0].Is_OnLeave = true;

            }
            //tbl_ReportingManager rept = new tbl_ReportingManager();
            //rept.Is_OnLeave = false;
            //rept.Emp_UserId = "NULL";
            myapp.SaveChanges();
            return Json("You have successfully re-assigned as a reporting manager to your department", JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveEmployeesShiftdataBulk(string Fromdate, string Todate, int ShiftTypeId, string ShiftTypeName, string UserIds, int SelectType)
        {
            List<Rostertemp> listtr = new List<Rostertemp>();
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(Fromdate);
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(Todate);
            string[] userlist = UserIds.Split(',');
            while (dtfrom <= dtto)
            {
                foreach (string usr in userlist)
                {
                    if (usr != null && usr != "")
                    {
                        if (!IsHOlidayornotCheck(dtfrom))
                        {
                            Rostertemp tr = new Rostertemp
                            {
                                UserId = usr
                            };
                            if ((int)dtfrom.DayOfWeek == SelectType)
                            {
                                tr.ShiftTypeId = 4;
                                tr.ShiftTypeName = "WeekOff";
                            }
                            else
                            {
                                tr.ShiftTypeId = ShiftTypeId;
                                tr.ShiftTypeName = ShiftTypeName;
                            }
                            tr.ShiftDate = dtfrom.ToString("MM/dd/yyyy");
                            listtr.Add(tr);
                        }
                    }
                }
                dtfrom = dtfrom.AddDays(1);
            }
            JavaScriptSerializer js = new JavaScriptSerializer();
            string data = js.Serialize(listtr);
            string msg = hrdm.SaveEmployeesShiftdata(data, Fromdate, Todate, User.Identity.Name);
            return Json(msg, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportToExcelViewDutyRoster(string fromdate, string todate)
        {
            List<tbl_User> employeslist = GetListOfEmployees("");
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(fromdate);
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(todate);
            IQueryable<tbl_Roaster> eshiftslist = myapp.tbl_Roaster.Where(s => s.IsActive == true);
            List<tbl_Roaster> list = eshiftslist.ToList();
            List<tbl_Roaster> shifts = (from s in list
                                        where Convert.ToDateTime(s.ShiftDate) >= dtfrom && Convert.ToDateTime(s.ShiftDate) <= dtto
                                        select s).ToList();
            shifts = (from s in shifts
                      join emp in employeslist on s.UserId equals emp.CustomUserId
                      select s).ToList();
            var newlist = (from l in shifts
                           select new
                           {
                               EmployeeId = l.UserId,
                               EmployeeName = l.UserName,
                               Fromdate = l.ShiftDate.Value.ToShortDateString(),
                               ShiftId = l.ShiftTypeId,
                               ShiftTypeName = l.ShiftTypeName,
                               IsHoliday = IsHOlidayornotCheck(l.ShiftDate.Value),
                               LeaveType = CheckIsLeaveAppliedGetType(l.UserId, l.ShiftDate.Value.Date)
                           }).ToList();
            DataTable products = new System.Data.DataTable("EmployeesShiftRoster");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("EmployeeName", typeof(string));
            DateTime newdtfrom = dtfrom;
            DateTime newdtto = dtto;
            int countofvaluse = 2;
            while (newdtfrom < newdtto)
            {
                products.Columns.Add(newdtfrom.ToString("dd/MM/yyyy"), typeof(string));
                countofvaluse = countofvaluse + 1;
                newdtfrom = newdtfrom.AddDays(1);
            }
            foreach (tbl_User emps in employeslist)
            {
                object[] array1 = new object[countofvaluse];
                array1[0] = emps.CustomUserId;
                array1[1] = emps.FirstName;
                DateTime newdtfrom1 = dtfrom;
                DateTime newdtto1 = dtto;
                int acount = 2;
                while (newdtfrom1 < newdtto1)
                {
                    var listcheck = newlist.Where(l => l.EmployeeId == emps.CustomUserId && l.Fromdate == newdtfrom1.ToShortDateString()).ToList();
                    if (listcheck.Count > 0)
                    {
                        if (listcheck[0].IsHoliday)
                        {
                            array1[acount] = "Holiday";
                        }
                        else
                        {
                            if (listcheck[0].LeaveType != null && listcheck[0].LeaveType != "")
                            {
                                array1[acount] = listcheck[0].ShiftTypeName + " - " + listcheck[0].LeaveType;
                            }
                            else
                            {
                                array1[acount] = listcheck[0].ShiftTypeName;
                            }
                        }
                    }
                    else
                    {
                        array1[acount] = "";
                    }
                    newdtfrom1 = newdtfrom1.AddDays(1);
                    acount = acount + 1;
                }
                products.Rows.Add(array1);
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
            string filename = "Shift.xls";
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

        public JsonResult GetShiftOfTheDay(string Date)
        {
            HrDataManage hrdm = new HrDataManage();
            DateTime dt = ProjectConvert.ConverDateStringtoDatetime(Date);
            List<tbl_Roaster> roaster = myapp.tbl_Roaster.Where(l => l.UserId == User.Identity.Name && l.ShiftDate.Value == dt).ToList();
            if (roaster.Count > 0)
            {
                int shifttypeid = roaster[0].ShiftTypeId.Value;
                List<tbl_ShiftType> shifttype = myapp.tbl_ShiftType.Where(l => l.ShiftTypeId == shifttypeid).ToList();
                List<string> data = hrdm.GetShiftTimetoddl(shifttype[0].ShiftStartTime, shifttype[0].ShiftEndTime, dt.ToShortDateString());
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<string> data = hrdm.GetShiftTimetoddl("", "", dt.ToShortDateString());
                return Json(data, JsonRequestBehavior.AllowGet);
            }
        }
        protected override JsonResult Json(object data, string contentType, System.Text.Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new JsonResult()
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                MaxJsonLength = int.MaxValue // Use this value to set your maximum size for all of your Requests
            };
        }
        public JsonResult SaveCompOffEncash(string remarks, int count)
        {
            HrDataManage hrdm = new HrDataManage();
            string reportingto = hrdm.GetReportingMgr(User.Identity.Name, DateTime.Today, DateTime.Today);
            List<tbl_User> user = myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).ToList();
            double checkcount = GetCompoffLeaveBalancetotal(User.Identity.Name);
            checkcount = checkcount - count;
            if (checkcount >= 0 && count > 0)
            {
                if (reportingto != null && reportingto != "" && user.Count > 0)
                {
                    tbl_CompOffEncash cmp = new tbl_CompOffEncash
                    {
                        UserId = User.Identity.Name,
                        NoOfDays = count,
                        IsApproved = false,
                        HRApproved = false,
                        ReportingTo = reportingto,
                        Status = "New",
                        SubmitedOn = DateTime.Now,
                        LocationId = user[0].LocationId,
                        DepartmentId = user[0].DepartmentId,
                        Remarks = remarks
                    };
                    myapp.tbl_CompOffEncash.Add(cmp);
                    myapp.SaveChanges();
                }
            }
            else
            {
                return Json("Insufficient Balance Please Check", JsonRequestBehavior.AllowGet);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetMyCompOffEncashes(JQueryDataTableParamModel param)
        {

            if (Request.IsAuthenticated)
            {
                List<tbl_User> userslist = myapp.tbl_User.Where(c => c.IsActive == true).ToList();
                tbl_User currentuser = userslist.Where(l => l.CustomUserId == User.Identity.Name).SingleOrDefault();
                List<tbl_CompOffEncash> list = (from v in myapp.tbl_CompOffEncash where v.UserId == User.Identity.Name select v).ToList();
                List<CompOffEncashViewModel> query = (from l in list
                                                      join usr in userslist on l.ReportingTo equals usr.CustomUserId
                                                      select new CompOffEncashViewModel
                                                      {
                                                          CompOffEncashId = l.CompOffEncashId,
                                                          DepartmentId = currentuser.DepartmentId,
                                                          DepartmentName = currentuser.DepartmentName,
                                                          HRApproved = l.HRApproved.HasValue ? l.HRApproved.Value.ToString() : "",
                                                          HrEnchashedDate = l.HrEnchashedDate.HasValue ? l.HrEnchashedDate.Value.ToString("dd/MM/yyyy") : "",
                                                          HrRemarks = l.HrRemarks,
                                                          IsApproved = l.IsApproved.HasValue ? l.IsApproved.Value.ToString() : "",
                                                          LocationId = currentuser.LocationId,
                                                          LocationName = currentuser.LocationName,
                                                          ModifiedBy = l.ModifiedBy,
                                                          ModifiedOn = l.ModifiedOn.HasValue ? l.ModifiedOn.Value.ToString("dd/MM/yyyy") : "",
                                                          NoOfDays = l.NoOfDays,
                                                          Remarks = l.Remarks,
                                                          ReportingTo = l.ReportingTo,
                                                          ReportingToName = usr.FirstName,
                                                          Status = l.Status,
                                                          SubmitedOn = l.SubmitedOn.HasValue ? l.SubmitedOn.Value.ToString("dd/MM/yyyy") : "",
                                                          UserId = l.UserId,
                                                          UserName = currentuser.FirstName
                                                      }).ToList();

                IEnumerable<CompOffEncashViewModel> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.CompOffEncashId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||

                                  c.UserName != null && c.UserName.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.ReportingToName != null && c.ReportingToName.ToLower().Contains(param.sSearch.ToLower())
                                    ||

                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())
                                  );
                }
                else
                {
                    filteredCompanies = query;
                }
                IEnumerable<CompOffEncashViewModel> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<string[]> result = from c in displayedCompanies
                                               select new[] {

                                              c.UserId.ToLower().Replace("fh_",""),
                                              c.UserName,
                                              c.SubmitedOn,
                                              c.NoOfDays.ToString(),
                                              c.Remarks,
                                               c.ReportingToName,
                                              c.IsApproved,
                                              c.HRApproved,
                                              c.HrRemarks,
                                              c.HrEnchashedDate,
                                              c.Status,
                                              c.CompOffEncashId.ToString()


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

        public JsonResult UpdateCompOffrequest()
        {
            List<tbl_User> users = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            foreach (tbl_User usr in users)
            {
                List<tbl_Leave> leaves = myapp.tbl_Leave.Where(l => l.LeaveTypeId == 6 && l.UserId == usr.CustomUserId && l.LeaveStatus != "Cancelled" && l.LeaveStatus != "Reject").OrderBy(l => l.LeaveFromDate).ToList();
                List<tbl_RequestCompOffLeave> CompOffReqList = myapp.tbl_RequestCompOffLeave.Where(e => e.IsApproved_Manager_4 == false && e.UserId == usr.CustomUserId && e.Leave_Status == "Approved").OrderBy(cl => cl.CompOffDateTime).ToList();
                foreach (tbl_RequestCompOffLeave cmp in CompOffReqList)
                {
                    DateTime dtvalidcomp = cmp.CompOffDateTime.Value.AddDays(90);
                    int leavescheck = leaves.Where(l => l.LeaveFromDate < dtvalidcomp).Count();
                    if (leavescheck > 0 || dtvalidcomp > DateTime.Now)
                    {
                        cmp.IsApproved_Manager_4 = true;
                        myapp.SaveChanges();
                    }
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCompOffBalance()
        {
            List<tbl_User> users = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            foreach (tbl_User usr in users)
            {
                int leaves = myapp.tbl_Leave.Where(l => l.LeaveTypeId == 6 && l.UserId == usr.CustomUserId && (l.LeaveStatus == "Approved" || l.LeaveStatus == "Pending")).Count();

                leaves = 0;

                int CompOffReqList = myapp.tbl_RequestCompOffLeave.Where(e => e.IsApproved_Manager_4 == true && e.UserId == usr.CustomUserId && e.Leave_Status != "Rejected" && e.Leave_Status != "Canceled").Count();
                int totla = CompOffReqList - leaves;
                //if (totla < 0)
                //{
                //    totla = 0;
                //}
                List<tbl_ManageLeave> listcheck = myapp.tbl_ManageLeave.Where(l => l.LeaveTypeId == 6 && l.UserId == usr.CustomUserId).ToList();
                if (listcheck.Count > 0)
                {
                    listcheck[0].AvailableLeave = totla;
                    listcheck[0].CountOfLeave = totla;
                    myapp.SaveChanges();
                }
                else
                {
                    tbl_ManageLeave model = new tbl_ManageLeave
                    {
                        AvailableLeave = totla,
                        CountOfLeave = totla,
                        CreatedBy = User.Identity.Name,
                        CreatedOn = DateAndTime.Now,
                        LeaveTypeId = 6,
                        LeaveTypeName = "Comp Off",
                        LocationId = usr.LocationId,
                        LocationName = usr.LocationName,
                        UserId = usr.CustomUserId,
                        UserName = usr.FirstName,
                        ModifiedBy = User.Identity.Name,
                        ModifiedOn = DateAndTime.Now,
                        IsActive = true,
                        DepartmentId = usr.DepartmentId,
                        DepartmentName = usr.DepartmentName
                    };
                    myapp.tbl_ManageLeave.Add(model);
                    myapp.SaveChanges();
                }
            }


            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Roaster(HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);
                string conString = string.Empty;
                string conString03 = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = { 0 }; Extended Properties = 'Excel 8.0;HDR=YES";
                string conString07 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = conString03;
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = conString07;
                        break;
                }

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }
                tbl_Roaster Roaster = new tbl_Roaster();
                //Insert records to database table.
                foreach (DataRow row in dt.Rows)
                {
                    string user = row["EmployeeId"].ToString();
                    string ShiftTypeName = row["Shift"].ToString();
                    DateTime Date = Convert.ToDateTime(row["Date"]);
                    List<tbl_Roaster> cat = myapp.tbl_Roaster.Where(l => l.UserId == user && l.ShiftDate == Date).ToList();
                    List<tbl_ShiftType> shiftdetails = (from var in myapp.tbl_ShiftType where var.ShiftTypeName == ShiftTypeName select var).ToList();
                    List<tbl_User> shiftuser = (from var in myapp.tbl_User where var.CustomUserId == user select var).ToList();
                    if (shiftdetails.Count > 0)
                    {
                        if (cat.Count > 0)
                        {
                            cat[0].UserId = shiftuser[0].CustomUserId;
                            cat[0].ShiftTypeName = ShiftTypeName;
                            cat[0].ShiftDate = Date;
                            cat[0].ShiftTypeId = shiftdetails[0].ShiftTypeId;
                            cat[0].ShiftEndTime = shiftdetails[0].ShiftEndTime;
                            cat[0].ShiftStartTime = shiftdetails[0].ShiftStartTime;
                            cat[0].UserName = shiftuser[0].FirstName;
                            cat[0].LocationId = shiftuser[0].LocationId;
                            cat[0].DepartmentId = shiftuser[0].DepartmentId;
                            cat[0].IsActive = true;
                            myapp.SaveChanges();
                        }
                        else
                        {
                            Roaster.UserId = shiftuser[0].CustomUserId;
                            Roaster.ShiftTypeName = row["Shift"].ToString();
                            Roaster.ShiftDate = Date;
                            Roaster.ShiftTypeId = shiftdetails[0].ShiftTypeId;
                            Roaster.ShiftEndTime = shiftdetails[0].ShiftEndTime;
                            Roaster.ShiftStartTime = shiftdetails[0].ShiftStartTime;
                            Roaster.UserName = shiftuser[0].FirstName;
                            Roaster.LocationId = shiftuser[0].LocationId;
                            Roaster.DepartmentId = shiftuser[0].DepartmentId;
                            Roaster.IsActive = true;
                            Roaster.CreatedOn = DateTime.Now;
                            myapp.tbl_Roaster.Add(Roaster);
                            myapp.SaveChanges();
                        }
                    }
                }


            }
            return Redirect("/Hr/EditDutyRoster");
        }

        public ActionResult HRNotification()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Vacin()
        {
            return View();
        }
    }
}
