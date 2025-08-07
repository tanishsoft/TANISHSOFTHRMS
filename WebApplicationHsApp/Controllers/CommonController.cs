using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    //[Authorize]
    public class CommonController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Common
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetLocations()
        {
            var list = myapp.tbl_Location.ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDepartmentByLocation(int id)
        {
            var list = myapp.tbl_Department.Where(d => d.LocationId == id).OrderBy(d => d.DepartmentName).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSubDepartmentByDepartment(int id)
        {
            var list = myapp.tbl_subdepartment.Where(d => d.DepartmentId == id).OrderBy(d => d.Name).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEmployeesByDepartment(int id)
        {
            var list = myapp.tbl_User.Where(d => d.DepartmentId == id && d.IsActive == true).OrderBy(d => d.FirstName).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFloorsByDepartmentId(int id)
        {
            var model = myapp.tbl_FloorDepartment.Where(l => l.DepartmentId == id).ToList();
            var list = (from m in model
                        join f in myapp.tbl_Floor on m.FloorId equals f.FloorId
                        select f).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        //Get the employees based on the login selection
        public JsonResult GetEmployeesByDepartmentname()
        {
            //if (User.IsInRole("OutSource"))
            //{
            //    var dept = myapp.tbl_OutSourceUser.Where(u => u.CustomUserId == User.Identity.Name).Single();
            //    var listname = (from v in myapp.tbl_OutSourceUser where v.IsActive == true && v.DepartmentName == dept.DepartmentName select v).ToList();
            //    return Json(listname, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            var dept = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).ToList();
            string departmentname = dept[0].DepartmentName;
            var listname = (from v in myapp.tbl_User
                            where v.IsActive == true && v.DepartmentName == departmentname
                            select new
                            {
                                v.CustomUserId,
                                v.FirstName,
                                v.LastName,
                                v.EmailId,
                                v.EmpId,
                                v.UserId
                            }
                            ).ToList();
            return Json(listname, JsonRequestBehavior.AllowGet);
            //}

        }
        public ActionResult GetEmployeesByUserLocation()
        {
            if (User.IsInRole("OutSource"))
            {
                var cuser = myapp.tbl_OutSourceUser.Where(u => u.CustomUserId == User.Identity.Name).Single();
                var list = myapp.tbl_OutSourceUser.Where(d => d.LocationId == cuser.LocationId && d.IsActive == true).ToList();
                return Json(list, JsonRequestBehavior.AllowGet);
            }
            else
            {

                var cuser = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).Single();
                var list = myapp.tbl_User.Where(d => d.LocationId == cuser.LocationId && d.IsActive == true).ToList();

                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetVendorlist()
        {
            var query = (from d in myapp.tbl_Vendor select d).ToList();
            query = query.OrderBy(l => l.Name).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDesignationListToAdmin()
        {
            var List = myapp.tbl_MasterEmployeeDesignation.Where(e => e.Record_Status == true).OrderBy(l => l.Designation_Name).ToList();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetMenuPages()
        {
            var list = (from p in myapp.tbl_Page
                        where p.IsActive == true
                        select new
                        {
                            p.PageId,
                            p.PageName,
                            p.MenuId
                        }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetByLocation(int Locationid)
        {
            var list = (from a in myapp.tbl_Building
                        where a.LocationId == Locationid && a.IsActive == true
                        select a).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetByFloor(int id)
        {
            var list = (from a in myapp.tbl_Floor
                        where a.BuildingId == id && a.IsActive == true
                        select a).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetByRoom(int id)
        {
            var list = (from a in myapp.tbl_Room
                        where a.FloorId == id && a.IsActive == true
                        select a).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetShiftTypes()
        {
            var list = (from a in myapp.tbl_ShiftType where a.IsActive == true select a).OrderBy(d => d.ShiftTypeName).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployeesByReportingmgr()
        {
            var list = new List<tbl_User>();
            var rptmgr = myapp.tbl_ReportingManager.FirstOrDefault(a => a.UserId == User.Identity.Name);
            if (rptmgr != null)
            {
                var loc = rptmgr.LocationId;
                var dep = rptmgr.DepartmentId;
                var sub = rptmgr.SubDepartmentId;
                if (sub != null)
                {
                    list = myapp.tbl_User.Where(d => d.LocationId == loc && d.DepartmentId == dep && d.SubDepartmentId == sub && d.IsActive == true).ToList();
                }
                else
                {
                    list = myapp.tbl_User.Where(d => d.LocationId == loc && d.DepartmentId == dep && d.IsActive == true).OrderBy(d => d.FirstName).ToList();
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEmployeesByVendor(string VendorId)
        {
            var list = new List<tbl_OutSourceUser>();
            if (VendorId != null && VendorId != "")
            {

                list = myapp.tbl_OutSourceUser.Where(d => d.VendorId == VendorId && d.IsActive == true).ToList();
            }
            else {
                string vendoremail = User.Identity.Name;
                var rptmgr = myapp.tbl_Vendor.FirstOrDefault(a => a.Email == vendoremail);
                if (rptmgr != null)
                {
                    string vendorid = rptmgr.VendorId.ToString();
                    list = myapp.tbl_OutSourceUser.Where(d => d.VendorId == vendorid && d.IsActive == true).ToList();
                }
            }
            
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetAllVendors()
        {
            var list = myapp.tbl_Vendor.Where(a => a.Email != null && a.IsActive == true).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetLeaveTypes()
        {
            var list = myapp.tbl_LeaveType.ToList();
            if (User.IsInRole("HrAdmin") || User.IsInRole("Admin"))
            {
                return Json(list, JsonRequestBehavior.AllowGet);
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
                return Json(list, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetCommonDepartments()
        {
            var list = myapp.tbl_CommonDepartment.OrderBy(l => l.Name).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCommonDropdownValues(string dtype)
        {
            var list = myapp.tbl_DropdownValues.Where(l => l.DropdownType == dtype).OrderBy(l => l.Name).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategoryByDepartmentId(int id, string type = "")
        {
            var list = myapp.tbl_DepartmentVsCategory.Where(l => l.DepartmentId == id && l.IsActive == true).ToList();
            var model = (from c in myapp.tbl_Category.ToList()
                         join l in list on c.CategoryId equals l.CategoryId
                         select c).ToList();
            if (type != null && type != "")
            {
                model = model.Where(l => l.Description == type).ToList();
            }
            model = model.OrderBy(l => l.Name).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDrugsEntryData(int Location = 0, int Department = 0, int Floor = 0)
        {
            ModeltoBindDataDrug Mdd = new ModeltoBindDataDrug();
            var list = myapp.tbl_Drug.Where(l => l.LocationId == Location && l.DepartmentId == Department && l.FloorId == Floor).ToList();
            if (list.Count > 0)
            {
                Mdd.drugs = list;
                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                var saveddrugsinfo = myapp.tbl_DrugNicuReport.Where(l => l.StockDate >= startDate && l.StockDate <= endDate).ToList();
                var sdate = startDate;
                var edate = endDate;
                List<DrugUserEnterViewModel> model = new List<DrugUserEnterViewModel>();
                List<string> Dates = new List<string>();
                while (sdate <= edate)
                {
                    foreach (var v in list)
                    {
                        DrugUserEnterViewModel m = new DrugUserEnterViewModel();
                        m.DrugId = v.DrugId;
                        m.StockDate = sdate.ToString("dd/MM");
                        //if (sdate < now)
                        //{
                        //m.StockQty = v.StockTotal;
                        m.StockQty = 0;
                        var checkhis = saveddrugsinfo.Where(l => l.DrugId == v.DrugId && l.StockDate == sdate).ToList();
                        if (checkhis.Count > 0)
                        {
                            //decimal actuvalbalance = v.StockTotal.HasValue ? v.StockTotal.Value : 0;
                            decimal actuvalbalance = 0;
                            decimal TotalDebit = checkhis.Where(l => l.IsDebit == true).Sum(l => l.StockQty).Value;
                            decimal TotalCredit = checkhis.Where(l => l.IsCredit == true).Sum(l => l.StockQty).Value;
                            actuvalbalance = actuvalbalance + TotalCredit - TotalDebit;
                            m.StockQty = actuvalbalance;
                        }
                        //}
                        //else
                        //{

                        //}
                        model.Add(m);
                    }
                    Dates.Add(sdate.ToString("dd/MM"));
                    sdate = sdate.AddDays(1);
                }
                Mdd.model = model;
                Mdd.Dates = Dates;
            }
            return PartialView(Mdd);
        }


        public JsonResult SaveDrugInfo(DrugUserEntryHistoryModel model)
        {
            string message = "";
            tbl_DrugNicuReport obj = new tbl_DrugNicuReport();
            obj.CreatedBy = User.Identity.Name;
            obj.CreatedOn = DateTime.Now;
            obj.DrugId = model.DrugId;
            obj.IsCredit = model.IsCredit;
            obj.IsDebit = model.IsDebit;
            obj.ModifiedBy = User.Identity.Name;
            obj.ModifiedOn = DateTime.Now;
            obj.PatientId = model.PatientId;
            obj.PatientName = model.PatientName;
            obj.Remarks = model.Remarks;

            obj.StockDate = ProjectConvert.ConverDateStringtoDatetime(model.StockDate + "/" + DateTime.Now.Year);

            obj.StockQty = model.StockQty;
            obj.UserId = User.Identity.Name;
            obj.MrNumber = model.MrNumber;
            obj.RoomNo = model.RoomNo;

            if (model.IsDebit.Value)
            {
                var checkhis = myapp.tbl_DrugNicuReport.Where(l => l.DrugId == model.DrugId && l.StockDate == obj.StockDate).ToList();
                if (checkhis.Count > 0)
                {

                    var aaty = checkhis.Where(l => l.IsCredit == true).Sum(l => l.StockQty).Value;
                    var daty = checkhis.Where(l => l.IsDebit == true).Sum(l => l.StockQty).Value;
                    var balance = aaty - daty;
                    if (balance >= model.StockQty)
                    {
                        myapp.tbl_DrugNicuReport.Add(obj);
                        myapp.SaveChanges();
                        message = "Success";
                    }
                    else
                    {
                        message = "Please check balance is low";
                    }
                }

            }
            if (model.IsCredit.Value)
            {
                var dmodel = myapp.tbl_Drug.Where(l => l.DrugId == model.DrugId).ToList();
                if (dmodel.Count > 0)
                {

                    if (model.StockQty <= dmodel[0].StockTotal)
                    {
                        if (model.DrugExpiryDate != null && model.DrugExpiryDate != "")
                        {
                            dmodel[0].DrugExpiryDate = ProjectConvert.ConverDateStringtoDatetime(model.DrugExpiryDate);
                            myapp.SaveChanges();
                            myapp.tbl_DrugNicuReport.Add(obj);
                            myapp.SaveChanges();
                            message = "Success";
                        }
                    }
                    else
                    {
                        message = "Please check your adding more than actual stock";
                    }
                }
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCurrentMonthDrugHistory(int DrugId, string date)
        {
            DateTime dt = ProjectConvert.ConverDateStringtoDatetime(date + "/" + DateTime.Now.Year);

            var report = myapp.tbl_DrugNicuReport.Where(l => l.DrugId == DrugId && l.StockDate == dt).ToList();
            var list = (from m in report
                        join u in myapp.tbl_User on m.CreatedBy equals u.CustomUserId
                        join d in myapp.tbl_Drug on m.DrugId equals d.DrugId

                        select new DrugUserEntryHistoryModel
                        {
                            DrugName = d.DrugName,
                            CreatedBy = m.CreatedBy,
                            DrugId = m.DrugId,
                            DrugNICUReportId = m.DrugNICUReportId,
                            CreatedOn = m.CreatedOn.Value.ToString("dd/MM/yyyy"),
                            PatientId = m.PatientId,
                            PatientName = m.PatientName,
                            UserName = u.FirstName,
                            UserId = u.CustomUserId,
                            Remarks = m.Remarks,
                            StockQty = m.StockQty,
                            TypeOfStock = m.IsCredit == true ? "Add" : "Remove",
                            DrugExpiryDate = d.DrugExpiryDate.Value.ToString("dd/MM/yyyy"),
                            MrNumber = m.MrNumber,
                            RoomNo = m.RoomNo

                        }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetDrugReportAddViewModel(JQueryDataTableParamModel param)
        {
            var users = myapp.tbl_User.ToList();
            var drugs = myapp.tbl_Drug.ToList();
            var floorlist = myapp.tbl_Floor.ToList();
            var loclist = myapp.tbl_Location.ToList();
            var resultsmodel = myapp.tbl_DrugNicuReport.Where(l => l.IsCredit == true).ToList();
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(param.todate);
            resultsmodel = resultsmodel.Where(q => q.StockDate >= dtfrom && q.StockDate <= dtto).ToList();
            List<DrugReportAddViewModel> model = new List<DrugReportAddViewModel>();
            while (dtfrom <= dtto)
            {
                var list = resultsmodel.Where(l => l.StockDate.Value.Date == dtfrom.Date).ToList();
                if (list.Count > 0)
                {
                    int count = 0;
                    decimal LastQtyadded = 0;
                    foreach (var l in list)
                    {
                        try
                        {
                            DrugReportAddViewModel m = new DrugReportAddViewModel();
                            if (count == 0)
                            {
                                m.BeforeAddQty = 0;
                            }
                            else
                            {
                                m.BeforeAddQty = LastQtyadded;
                            }
                            var cdrug = drugs.Where(u => u.DrugId == l.DrugId).SingleOrDefault();
                            m.AddQty = l.StockQty.Value;
                            m.Date = l.StockDate.Value.ToString("dd/MM/yyyy");
                            m.DrugName = cdrug.DrugName;
                            m.ExpireDate = cdrug.DrugExpiryDate.Value.ToString("dd/MM/yyyy");
                            m.Time = l.CreatedOn.Value.ToString("hh:mm");
                            m.Remarks = l.Remarks;
                            m.TotalQty = m.AddQty + m.BeforeAddQty;
                            m.Username = users.Where(u => u.CustomUserId == l.CreatedBy).SingleOrDefault().FirstName;
                            m.Floor = floorlist.Where(f => f.FloorId == cdrug.FloorId).SingleOrDefault().FloorName;
                            m.Location = loclist.Where(loc => loc.LocationId == cdrug.LocationId).SingleOrDefault().LocationName;
                            model.Add(m);
                            LastQtyadded = m.TotalQty;
                        }
                        catch
                        { }
                        count++;
                    }
                }
                dtfrom = dtfrom.AddDays(1);
            }



            IEnumerable<DrugReportAddViewModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = model
                   .Where(c => c.DrugName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Remarks != null && c.Remarks.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Username != null && c.Username.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Location != null && c.Location.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.Floor != null && c.Floor.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.Date != null && c.Date.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.TotalQty != null && c.TotalQty.ToString().Contains(param.sSearch.ToLower())
                                ||
                              c.AddQty != null && c.AddQty.ToString().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = model;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                                              c.Date,
                                              c.Time,
                                              c.DrugName,
                                              c.BeforeAddQty.ToString(),

                                              c.AddQty.ToString(),

                                              c.TotalQty.ToString(),
                                              c.ExpireDate,
                                              c.Username,
                                              c.Location,
                         c.Floor,
                         c.Remarks
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = model.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult AjaxGetDrugReportRemoveViewModel(JQueryDataTableParamModel param)
        {
            var users = myapp.tbl_User.ToList();
            var drugs = myapp.tbl_Drug.ToList();
            var floorlist = myapp.tbl_Floor.ToList();
            var loclist = myapp.tbl_Location.ToList();
            var aquerydrug = myapp.tbl_DrugNicuReport.ToList();
            var resultsmodel = aquerydrug.Where(l => l.IsDebit == true).ToList();
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(param.todate);
            resultsmodel = resultsmodel.Where(q => q.StockDate >= dtfrom && q.StockDate <= dtto).ToList();
            List<DrugReportRemoveViewModel> model = new List<DrugReportRemoveViewModel>();
            while (dtfrom <= dtto)
            {
                var list = resultsmodel.Where(l => l.StockDate.Value.Date == dtfrom.Date).ToList();
                if (list.Count > 0)
                {
                    int count = 0;
                    decimal LastQtyadded = 0;
                    foreach (var l in list)
                    {
                        try
                        {
                            DrugReportRemoveViewModel m = new DrugReportRemoveViewModel();
                            if (count == 0)
                            {
                                m.BeforeGivetopatientqty = aquerydrug.Where(d => d.StockDate.Value.Date == dtfrom.Date && d.IsCredit == true).Sum(d => d.StockQty).Value;
                            }
                            else
                            {
                                m.BeforeGivetopatientqty = LastQtyadded;
                            }
                            var cdrug = drugs.Where(u => u.DrugId == l.DrugId).SingleOrDefault();
                            m.HowmanygiventoPatient = l.StockQty.Value;
                            m.Date = l.StockDate.Value.ToString("dd/MM/yyyy");
                            m.DrugName = cdrug.DrugName;
                            m.IpNumber = l.PatientId;
                            m.MrNumber = l.MrNumber;
                            m.PatientName = l.PatientName;
                            m.RoomNumber = l.RoomNo;
                            m.ExpireDate = cdrug.DrugExpiryDate.Value.ToString("dd/MM/yyyy");
                            m.Time = l.CreatedOn.Value.ToString("hh:mm");
                            m.Remarks = l.Remarks;
                            m.NowbalanceQty = m.BeforeGivetopatientqty - m.HowmanygiventoPatient;
                            m.Username = users.Where(u => u.CustomUserId == l.CreatedBy).SingleOrDefault().FirstName;
                            m.Floor = floorlist.Where(f => f.FloorId == cdrug.FloorId).SingleOrDefault().FloorName;
                            m.Location = loclist.Where(loc => loc.LocationId == cdrug.LocationId).SingleOrDefault().LocationName;
                            model.Add(m);
                            LastQtyadded = m.NowbalanceQty;
                        }
                        catch
                        { }
                        count++;
                    }
                }
                dtfrom = dtfrom.AddDays(1);
            }



            IEnumerable<DrugReportRemoveViewModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = model
                   .Where(c => c.DrugName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Remarks != null && c.Remarks.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Username != null && c.Username.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Location != null && c.Location.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.IpNumber != null && c.IpNumber.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.MrNumber != null && c.MrNumber.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.PatientName != null && c.PatientName.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.RoomNumber != null && c.RoomNumber.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.Floor != null && c.Floor.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.Date != null && c.Date.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.NowbalanceQty != null && c.NowbalanceQty.ToString().Contains(param.sSearch.ToLower())
                                ||
                              c.HowmanygiventoPatient != null && c.HowmanygiventoPatient.ToString().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = model;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                                              c.Date,
                                              c.Time,
                                              c.IpNumber,
                                              c.MrNumber,
                                              c.PatientName,
                                              c.RoomNumber,
                                              c.DrugName,
                                              c.BeforeGivetopatientqty.ToString(),

                                              c.HowmanygiventoPatient.ToString(),

                                              c.NowbalanceQty.ToString(),

                                              c.Username,
                                              c.Location,
                         c.Floor,
                         c.Remarks};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = model.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ExportExcel_DrugReportAddViewModel(string fromdate, string todate)
        {
            var users = myapp.tbl_User.ToList();
            var drugs = myapp.tbl_Drug.ToList();
            var floorlist = myapp.tbl_Floor.ToList();
            var loclist = myapp.tbl_Location.ToList();
            var resultsmodel = myapp.tbl_DrugNicuReport.Where(l => l.IsCredit == true).ToList();
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(fromdate);
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(todate);
            resultsmodel = resultsmodel.Where(q => q.StockDate >= dtfrom && q.StockDate <= dtto).ToList();
            List<DrugReportAddViewModel> model = new List<DrugReportAddViewModel>();
            while (dtfrom <= dtto)
            {
                var list = resultsmodel.Where(l => l.StockDate.Value.Date == dtfrom.Date).ToList();
                if (list.Count > 0)
                {
                    int count = 0;
                    decimal LastQtyadded = 0;
                    foreach (var l in list)
                    {
                        try
                        {
                            DrugReportAddViewModel m = new DrugReportAddViewModel();
                            if (count == 0)
                            {
                                m.BeforeAddQty = 0;
                            }
                            else
                            {
                                m.BeforeAddQty = LastQtyadded;
                            }
                            var cdrug = drugs.Where(u => u.DrugId == l.DrugId).SingleOrDefault();
                            m.AddQty = l.StockQty.Value;
                            m.Date = l.StockDate.Value.ToString("dd/MM/yyyy");
                            m.DrugName = cdrug.DrugName;
                            m.ExpireDate = cdrug.DrugExpiryDate.Value.ToString("dd/MM/yyyy");
                            m.Time = l.CreatedOn.Value.ToString("hh:mm");
                            m.Remarks = l.Remarks;
                            m.TotalQty = m.AddQty + m.BeforeAddQty;
                            m.Username = users.Where(u => u.CustomUserId == l.CreatedBy).SingleOrDefault().FirstName;
                            m.Floor = floorlist.Where(f => f.FloorId == cdrug.FloorId).SingleOrDefault().FloorName;
                            m.Location = loclist.Where(loc => loc.LocationId == cdrug.LocationId).SingleOrDefault().LocationName;
                            model.Add(m);
                            LastQtyadded = m.TotalQty;
                        }
                        catch
                        { }
                        count++;
                    }
                }
                dtfrom = dtfrom.AddDays(1);
            }
            var products = new System.Data.DataTable("DrugReportAddViewModel");
            products.Columns.Add("S.No", typeof(string));
            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("Time", typeof(string));
            products.Columns.Add("Drug Name", typeof(string));
            products.Columns.Add("Before Add Qty", typeof(string));
            products.Columns.Add("Add Qty", typeof(string));
            products.Columns.Add("Total Qty", typeof(string));
            products.Columns.Add("Expire Date", typeof(string));
            products.Columns.Add("Username", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Floor", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            int i = 1;
            foreach (var c in model)
            {
                products.Rows.Add(i,
                                 c.Date,
                                              c.Time,
                                              c.DrugName,
                                              c.BeforeAddQty.ToString(),

                                              c.AddQty.ToString(),

                                              c.TotalQty.ToString(),
                                              c.ExpireDate,
                                              c.Username,
                                              c.Location,
                         c.Floor,
                         c.Remarks
                );
                i = i + 1;
            }

            var grid = new GridView();
            grid.GridLines = GridLines.Both;
            grid.BorderStyle = BorderStyle.Solid;
            grid.HeaderStyle.BackColor = System.Drawing.Color.Teal;
            grid.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            string filename = "DrugReportAddViewModel.xls";
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

        public ActionResult ExportExcel_DrugReportRemoveViewModel(string fromdate, string todate)
        {
            var users = myapp.tbl_User.ToList();
            var drugs = myapp.tbl_Drug.ToList();
            var floorlist = myapp.tbl_Floor.ToList();
            var loclist = myapp.tbl_Location.ToList();
            var aquerydrug = myapp.tbl_DrugNicuReport.ToList();
            var resultsmodel = aquerydrug.Where(l => l.IsDebit == true).ToList();
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(fromdate);
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(todate);
            resultsmodel = resultsmodel.Where(q => q.StockDate >= dtfrom && q.StockDate <= dtto).ToList();
            List<DrugReportRemoveViewModel> model = new List<DrugReportRemoveViewModel>();
            while (dtfrom <= dtto)
            {
                var list = resultsmodel.Where(l => l.StockDate.Value.Date == dtfrom.Date).ToList();
                if (list.Count > 0)
                {
                    int count = 0;
                    decimal LastQtyadded = 0;
                    foreach (var l in list)
                    {
                        try
                        {
                            DrugReportRemoveViewModel m = new DrugReportRemoveViewModel();
                            if (count == 0)
                            {
                                m.BeforeGivetopatientqty = aquerydrug.Where(d => d.StockDate.Value.Date == dtfrom.Date && d.IsCredit == true).Sum(d => d.StockQty).Value;
                            }
                            else
                            {
                                m.BeforeGivetopatientqty = LastQtyadded;
                            }
                            var cdrug = drugs.Where(u => u.DrugId == l.DrugId).SingleOrDefault();
                            m.HowmanygiventoPatient = l.StockQty.Value;
                            m.Date = l.StockDate.Value.ToString("dd/MM/yyyy");
                            m.DrugName = cdrug.DrugName;
                            m.IpNumber = l.PatientId;
                            m.MrNumber = l.MrNumber;
                            m.PatientName = l.PatientName;
                            m.RoomNumber = l.RoomNo;
                            m.ExpireDate = cdrug.DrugExpiryDate.Value.ToString("dd/MM/yyyy");
                            m.Time = l.CreatedOn.Value.ToString("hh:mm");
                            m.Remarks = l.Remarks;
                            m.NowbalanceQty = m.BeforeGivetopatientqty - m.HowmanygiventoPatient;
                            m.Username = users.Where(u => u.CustomUserId == l.CreatedBy).SingleOrDefault().FirstName;
                            m.Floor = floorlist.Where(f => f.FloorId == cdrug.FloorId).SingleOrDefault().FloorName;
                            m.Location = loclist.Where(loc => loc.LocationId == cdrug.LocationId).SingleOrDefault().LocationName;
                            model.Add(m);
                            LastQtyadded = m.NowbalanceQty;
                        }
                        catch
                        { }
                        count++;
                    }
                }
                dtfrom = dtfrom.AddDays(1);
            }
            var products = new System.Data.DataTable("DrugReportRemoveViewModel");
            products.Columns.Add("S.No", typeof(string));
            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("Time", typeof(string));
            products.Columns.Add("IpNumber", typeof(string));
            products.Columns.Add("MrNumber", typeof(string));
            products.Columns.Add("Patient Name", typeof(string));

            products.Columns.Add("Room Number", typeof(string));
            products.Columns.Add("Drug Name", typeof(string));

            products.Columns.Add("Before Qty", typeof(string));
            products.Columns.Add("Given Qty", typeof(string));
            products.Columns.Add("Balance Qty", typeof(string));
            products.Columns.Add("Username", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Floor", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            int i = 1;
            foreach (var c in model)
            {
                products.Rows.Add(i,
                                   c.Date,
                                              c.Time,
                                              c.IpNumber,
                                              c.MrNumber,
                                              c.PatientName,
                                              c.RoomNumber,
                                              c.DrugName,
                                              c.BeforeGivetopatientqty.ToString(),

                                              c.HowmanygiventoPatient.ToString(),

                                              c.NowbalanceQty.ToString(),

                                              c.Username,
                                              c.Location,
                         c.Floor,
                         c.Remarks
                );
                i = i + 1;
            }

            var grid = new GridView();
            grid.GridLines = GridLines.Both;
            grid.BorderStyle = BorderStyle.Solid;
            grid.HeaderStyle.BackColor = System.Drawing.Color.Teal;
            grid.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            string filename = "DrugReportRemoveViewModel.xls";
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
        public JsonResult GetAllRoles()
        {
            var query = myapp.Database.SqlQuery<RoleViewModel>("select Id,Name from AspNetRoles").ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
    }
}