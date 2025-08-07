using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.DataModelRegister;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    [Authorize(Roles = "Admin,HrAdmin,Vendor")]
    public class AdminController : Controller
    {
        private MyIntranetApp_RegisterEntities myappregister = new MyIntranetApp_RegisterEntities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public AdminController()
        {
        }

        public AdminController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: Admin
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ManageLocation()
        {
            return View();
        }
        public ActionResult ManageDesignation()
        {
            return View();
        }

        public JsonResult SaveNewDesignation(tbl_MasterEmployeeDesignation NewData)
        {
            try
            {
                NewData.CreatedBy = User.Identity.Name;
                NewData.CreatedOn = DateTime.Now.ToString("MM/dd/yyyy");
                NewData.CreatedDateTime = DateTime.Now;
                NewData.Record_Status = true;
                myapp.tbl_MasterEmployeeDesignation.Add(NewData);
                myapp.SaveChanges();
            }
            catch (Exception)
            {
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDesignationListToAdmin()
        {
            var List = myapp.tbl_MasterEmployeeDesignation.Where(e => e.Record_Status == true).ToList();
            return Json(List, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveLocation(tbl_Location tbll)
        {
            if (tbll.LocationId != 0)
            {
                var tb = myapp.tbl_Location.Where(a => a.LocationId == tbll.LocationId).SingleOrDefault();
                tb.LocationName = tbll.LocationName;
                tb.ShowinCPX = tbll.ShowinCPX;
                tb.ShowasSubLocation = tbll.ShowasSubLocation;
                tb.ParentLocationId = tbll.ParentLocationId;
            }
            else
            {
                myapp.tbl_Location.Add(tbll);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetLocationView()
        {

            var result = (from c in myapp.tbl_Location
                          select c).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetLocationViewJson()
        {
            var result = from c in myapp.tbl_Location.ToList()
                         select new[] {
                             c.LocationId.ToString(),
                             c.LocationName,
                             c.ShowinCPX.HasValue?c.ShowinCPX.Value.ToString():"",
                             c.ShowasSubLocation.HasValue?c.ShowasSubLocation.Value.ToString():"",
                             c.ParentLocationId.HasValue?c.ParentLocationId.Value.ToString():"",
                             c.LocationId.ToString()

                             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageDepartment()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SaveDepartment(tbl_Department tbll)
        {
            if (tbll.DepartmentId != 0)
            {
                var tb = myapp.tbl_Department.Where(a => a.DepartmentId == tbll.DepartmentId).SingleOrDefault();
                tb.DepartmentName = tbll.DepartmentName;
                tb.LocationId = tbll.LocationId;
                tb.LocationName = tbll.LocationName;
                tb.ShowInHelpDesk = tbll.ShowInHelpDesk;
            }
            else
            {
                myapp.tbl_Department.Add(tbll);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteDepartment(int id)
        {
            var tb = myapp.tbl_Department.Where(a => a.DepartmentId == id).SingleOrDefault();
            myapp.tbl_Department.Remove(tb);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetDepartmentView()
        {
            var result = from c in myapp.tbl_Department.ToList()
                         select new[] {
                             c.DepartmentId.ToString(),
                             c.DepartmentName,
                             c.LocationName,
                             c.ShowInHelpDesk.HasValue?c.ShowInHelpDesk.Value.ToString():"False",
                             c.DepartmentId.ToString(),
                             c.LocationId.ToString()
                             };
            return Json(new
            {
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageBuilding()
        {
            return View();
        }
        [HttpPost]
        public JsonResult SaveBuilding(tbl_Building tbll)
        {
            tbll.IsActive = true;
            if (tbll.BuildingId != 0)
            {
                var tb = myapp.tbl_Building.Where(a => a.BuildingId == tbll.BuildingId).SingleOrDefault();
                tb.BuildingName = tbll.BuildingName;
                tb.LocationId = tbll.LocationId;
                tb.LocationName = tbll.LocationName;
                tb.IsActive = true;
            }
            else
            {
                myapp.tbl_Building.Add(tbll);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteBuilding(int id)
        {
            var tb = myapp.tbl_Building.Where(a => a.BuildingId == id).SingleOrDefault();
            myapp.tbl_Building.Remove(tb);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetBuildingView()
        {
            var result = from c in myapp.tbl_Building.ToList()
                         select new[] {
                             c.BuildingName,
                             c.LocationName,
                             c.BuildingId.ToString()            ,
                             c.LocationId.ToString()

                             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageFloor()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetFloor()
        {
            var s = (from pd in myapp.tbl_Floor
                     select new
                     {

                         pd.FloorId,
                         pd.FloorName,
                         pd.LocationId,
                         pd.BuildingId,
                         pd.LocationName,
                         pd.BuildingName
                     }).ToList();
            return Json(s, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetByLocation(int Locationid)
        {
            var list = (from a in myapp.tbl_Building
                        where a.LocationId == Locationid && a.IsActive == true
                        select a).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveFloor(tbl_Floor tbll)
        {

            if (tbll.FloorId != 0)
            {
                var tb = myapp.tbl_Floor.Where(a => a.FloorId == tbll.FloorId).SingleOrDefault();
                tb.BuildingName = tbll.BuildingName;
                tb.LocationId = tbll.LocationId;
                tb.LocationName = tbll.LocationName;
                tb.BuildingId = tbll.BuildingId;
                tb.FloorName = tbll.FloorName;
                tb.IsActive = true;
            }
            else
            {
                tbll.IsActive = true;
                myapp.tbl_Floor.Add(tbll);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteFloor(int id)
        {
            var tb = myapp.tbl_Floor.Where(a => a.FloorId == id).SingleOrDefault();
            myapp.tbl_Floor.Remove(tb);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageRoom()
        {
            return View();
        }

        public JsonResult DeleteRoom(int id)
        {
            var tb = myapp.tbl_Room.Where(a => a.RoomId == id).SingleOrDefault();
            myapp.tbl_Room.Remove(tb);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetByFloor(int id)
        {
            var list = (from a in myapp.tbl_Floor
                        where a.BuildingId == id && a.IsActive == true
                        select a).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategoryById(int id)
        {
            var list = (from a in myapp.tbl_Category
                        where a.CategoryId == id && a.IsActive == true
                        select a).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ManageRoomView()
        {
            var list = myapp.tbl_Room.Where(a => a.IsActive == true).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveRoom(tbl_Room tbll)
        {
            tbll.IsActive = true;
            if (tbll.RoomId != 0)
            {
                var tb = myapp.tbl_Room.Where(a => a.RoomId == tbll.RoomId).SingleOrDefault();
                if (tb != null)
                {
                    tb.BuildingName = tbll.BuildingName;
                    tb.LocationId = tbll.LocationId;
                    tb.LocationName = tbll.LocationName;
                    tb.RoomName = tbll.RoomName;
                    tb.RoomExtension = tbll.RoomExtension;
                    tb.BuildingId = tbll.BuildingId;
                    tb.IsActive = true;
                    tb.FloorId = tbll.FloorId;
                    tb.FloorName = tbll.FloorName;
                }
            }
            else
            {
                myapp.tbl_Room.Add(tbll);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        #region ManageGroup
        public ActionResult ManagePlace()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveSpace(tbl_Place tbll)
        {
            var rom = myapp.tbl_Room.Where(a => a.RoomId == tbll.RoomId).SingleOrDefault();
            if (rom != null)
            {
                tbll.RoomExtension = rom.RoomExtension;
            }
            if (tbll.PlaceId != 0)
            {
                var tb = myapp.tbl_Place.Where(a => a.PlaceId == tbll.PlaceId).SingleOrDefault();
                if (tb != null)
                {
                    tb.PlaceNo = tbll.PlaceNo;
                    tb.PlaceExtension = tbll.PlaceExtension;
                    tb.RoomId = tbll.RoomId;
                    tb.RoomName = tbll.RoomName;
                    tb.RoomExtension = tbll.RoomExtension;
                    tb.FloorId = tbll.FloorId;
                    tb.FloorName = tbll.FloorName;
                    tb.BuildingId = tbll.BuildingId;
                    tb.BuildingName = tbll.BuildingName;
                    tb.LocationId = tbll.LocationId;
                    tb.LocationName = tbll.LocationName;
                    tb.IsActive = true;
                }
            }
            else
            {
                tbll.IsActive = true;
                myapp.tbl_Place.Add(tbll);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SpaceDelete(int id)
        {
            var tb = myapp.tbl_Place.Where(a => a.PlaceId == id).SingleOrDefault();
            myapp.tbl_Place.Remove(tb);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult ManageSpaceGet()
        {
            var result = myapp.tbl_Place.ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetByRoom(int id)
        {
            var list = (from a in myapp.tbl_Room
                        where a.FloorId == id && a.IsActive == true
                        select a).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ManageGroup
        public ActionResult ManageGroup()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SaveGroup(tbl_Group tbll)
        {
            if (tbll.GroupId != 0)
            {
                var tb = myapp.tbl_Group.Where(a => a.GroupId == tbll.GroupId).SingleOrDefault();
                tb.GroupName = tbll.GroupName;
                tb.GroupLocationId = tbll.GroupLocationId;
                tb.GroupLocationName = tbll.GroupLocationName;
                tb.IsActive = true;
            }
            else
            {
                tbll.IsActive = true;
                myapp.tbl_Group.Add(tbll);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteGroup(int id)
        {
            var tb = myapp.tbl_Group.Where(a => a.GroupId == id).SingleOrDefault();
            myapp.tbl_Group.Remove(tb);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetGroupView()
        {
            var result = from c in myapp.tbl_Group.ToList()
                         select new[] {
                             c.GroupName,
                             c.GroupLocationName,
                             c.GroupId.ToString()            ,
                             c.GroupLocationId.ToString()

                             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region ManagePage
        public ActionResult ManagePage()
        {
            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult ManagePage(CustomPage model)
        {
            try
            {
                tbl_Page page = new tbl_Page();
                page.CreatedBy = User.Identity.Name;
                page.CreatedOn = DateTime.Now;
                page.DepartmentId = model.DepartmentId;
                page.DepartmentName = model.DepartmentName;
                page.ForAll = model.IsPageisForall;
                page.IsActive = true;
                page.MenuId = model.PageMenuId;
                page.LocationId = model.LocationId;
                page.LocationName = model.LocationName;
                page.PageContent = model.PageContent;
                page.PageName = model.pagetitle;
                myapp.tbl_Page.Add(page);
                myapp.SaveChanges();
                ViewBag.SuccessMsg = "Success";
                return View();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }

        }
        public ActionResult AjaxGetPages(JQueryDataTableParamModel param)
        {
            var query = myapp.tbl_Page;
            IEnumerable<tbl_Page> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PageId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.PageName != null && c.PageName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.MenuId != null && c.MenuId.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies

                         select new[] {c.PageId.ToString(),
                                              c.LocationName,
                                              c.DepartmentName,
                                              c.PageName,

                                              c.MenuId,

                                              c.PageId.ToString()
                                              };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }
        public ActionResult DeletePage(int id)
        {
            var query = myapp.tbl_Page.Where(i => i.PageId == id).ToList();
            if (query.Count > 0)
            {
                myapp.tbl_Page.Remove(query[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region ManageSettings
        public ActionResult ManageSettings()
        {
            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SaveSettings(tbl_Settings tbll)
        {
            if (tbll.SettingsId != 0)
            {
                var tb = myapp.tbl_Settings.Where(a => a.SettingsId == tbll.SettingsId).SingleOrDefault();
                tb.SettingKey = tbll.SettingKey;
                tb.SettingValue = tbll.SettingValue;
                tb.IsActive = true;
            }
            else
            {
                tbll.IsActive = true;
                myapp.tbl_Settings.Add(tbll);
            }
            myapp.SaveChanges();
            return RedirectToAction("ManageSettings");
        }

        public JsonResult DeleteSettings(int id)
        {
            var tb = myapp.tbl_Settings.Where(a => a.SettingsId == id).SingleOrDefault();
            myapp.tbl_Settings.Remove(tb);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetSettingsView()
        {
            var result = from c in myapp.tbl_Settings.Where(a => a.IsActive == true).ToList()
                         let SettingValue = (c.SettingValue != null && c.SettingValue.Length > 10) ? c.SettingValue.Substring(0, 10) : c.SettingValue
                         select new[] {c.SettingKey,
                             SettingValue,
                             c.SettingsId.ToString()

                             };
            return Json(new { aaData = result }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        public ActionResult ManageSubDepartment()
        {
            return View();
        }
        public ActionResult AjaxSubDepartment(JQueryDataTableParamModel param)
        {
            var query = myapp.tbl_subdepartment;
            IEnumerable<tbl_subdepartment> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.SubDepartmentId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.DepartmentName != null && c.DepartmentName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Name != null && c.Name.ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies

                         select new[] {c.SubDepartmentId.ToString(),
                                              c.LocationName,
                                              c.DepartmentName,
                                              c.Name,
                                              c.SubDepartmentId.ToString()
                                              };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SaveSubDepartment(tbl_subdepartment tbl)
        {
            myapp.tbl_subdepartment.Add(tbl);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteSubDepartment(int id)
        {
            var query = myapp.tbl_subdepartment.Where(i => i.SubDepartmentId == id).ToList();
            if (query.Count > 0)
            {
                myapp.tbl_subdepartment.Remove(query[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSubDepartmentsByDepartment(int id)
        {
            var query = myapp.tbl_subdepartment.Where(i => i.DepartmentId == id).ToList();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllSubDepatmentListToAdmin()
        {
            List<tbl_subdepartment> ReturnData = myapp.tbl_subdepartment.ToList();
            return Json(ReturnData, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageDepartmentVsShiftType()
        {
            return View();
        }
        public ActionResult AjaxDepartmentShiftType(JQueryDataTableParamModel param)
        {
            var query = myapp.tbl_DepartmentShifts.ToList();
            if (param.locationid != null && param.locationid != 0)
            {
                query = query.Where(q => q.LocationId == param.locationid).ToList();
            }
            if (param.departmentid != null && param.departmentid != 0)
            {
                query = query.Where(q => q.DepartmentId == param.departmentid).ToList();
            }
            IEnumerable<tbl_DepartmentShifts> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.DepartmentShiftId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.DepartmentName != null && c.DepartmentName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.ShiftTypeName != null && c.ShiftTypeName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.ShiftStartTime != null && c.ShiftStartTime.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.ShiftEndTime != null && c.ShiftEndTime.ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies

                         select new[] {c.DepartmentShiftId.ToString(),
                                              c.LocationName,
                                              c.DepartmentName,
                                              c.ShiftTypeName,
                                              c.ShiftStartTime,
                                              c.ShiftEndTime,
                                              c.DepartmentShiftId.ToString()
                                              };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult SaveDepartmentShiftType(tbl_DepartmentShifts tbl)
        {
            myapp.tbl_DepartmentShifts.Add(tbl);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteDepartmentShiftType(int id)
        {
            var query = myapp.tbl_DepartmentShifts.Where(i => i.DepartmentShiftId == id).ToList();
            if (query.Count > 0)
            {
                myapp.tbl_DepartmentShifts.Remove(query[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddOutSource()
        {
            return View();
        }
        public ActionResult ManageOutSource()
        {
            return View();
        }
        public JsonResult ExportExceloutSource()
        {
            var query = myapp.tbl_OutSourceUser.Where(a => a.IsActive == true && a.CustomUserId!=null).ToList();

            var products = new System.Data.DataTable("OutSource");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Employee Number", typeof(string));
            products.Columns.Add("EmailId", typeof(string));
            products.Columns.Add("Phone Number", typeof(string));
            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("Sub Department", typeof(string));
            products.Columns.Add("Designation", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.UserId,
                  c.LocationName,
                                              c.FirstName + " " + c.LastName,
                                              c.CustomUserId.Replace("out_", ""),
                                              c.EmailId,
                                              //c.PlaceAllocation,
                                              c.PhoneNumber,
                                              //c.Extenstion,
                                              c.DepartmentName,
                                              c.SubDepartmentName,
                                              c.Designation
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=OutSourceUsers.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult AjaxGetOutSources(JQueryDataTableParamModel param)
        {

            if (Request.IsAuthenticated)
            {
                var query = myapp.tbl_OutSourceUser.Where(a => a.IsActive == true).ToList();


                IEnumerable<tbl_OutSourceUser> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.CustomUserId != null && c.CustomUserId.Contains(param.sSearch.ToLower())
                                   ||
                                    c.LocationName != null && c.LocationName.ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.FirstName != null && c.FirstName.ToLower().Contains(param.sSearch.ToLower())
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
                var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayedCompanies
                             select new[] {/*c.UserId.ToString(),*/
                                              c.LocationName,
                                              c.FirstName+" "+c.LastName,
                                              c.CustomUserId.Replace("out_",""),
                                              c.EmailId,
                                              //c.PlaceAllocation,
                                              c.PhoneNumber,
                                              //c.Extenstion,
                                              c.DepartmentName,
                                              c.SubDepartmentName,
                                              c.Designation,
                                              c.EmpId.HasValue?c.EmpId.Value.ToString():"",
                                              c.UserId.ToString()};
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

        public ActionResult ManageDrug()
        {
            return View();
        }
        public JsonResult SaveDrug(tbl_Drug model)
        {
            myapp.tbl_Drug.Add(model);
            myapp.SaveChanges();
            return Json("New Drug Created Successfully", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteDrug(int id)
        {
            var model = myapp.tbl_Drug.Where(l => l.DrugId == id).ToList();
            if (model.Count > 0)
            {
                myapp.tbl_Drug.Remove(model[0]);
                myapp.SaveChanges();
            }
            return Json("Drug Deleted Successfully", JsonRequestBehavior.AllowGet);
        }
        public JsonResult updateDrug(tbl_Drug model)
        {
            var model2 = myapp.tbl_Drug.Where(l => l.DrugId == model.DrugId).ToList();
            if (model2.Count > 0)
            {
                model2[0].DepartmentId = model.DepartmentId;
                model2[0].DrugDescription = model.DrugDescription;
                model2[0].DrugExpiryDate = model.DrugExpiryDate;
                model2[0].DrugName = model.DrugName;
                model2[0].FloorId = model.FloorId;
                model2[0].IsActive = model.IsActive;
                model2[0].LocationId = model.LocationId;
                model2[0].StockTotal = model.StockTotal;
                model2[0].SubDepartmentId = model.SubDepartmentId;
                myapp.SaveChanges();
            }
            return Json("Drug Deleted Successfully", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetDrugs(JQueryDataTableParamModel param)
        {

            if (Request.IsAuthenticated)
            {
                var query = (from d in myapp.tbl_Drug
                             join l in myapp.tbl_Location on d.LocationId equals l.LocationId
                             join dep in myapp.tbl_Department on d.DepartmentId equals dep.DepartmentId
                             join f in myapp.tbl_Floor on d.FloorId equals f.FloorId
                             select new DrugViewModel
                             {
                                 DepartmentId = d.DepartmentId,
                                 DrugDescription = d.DrugDescription,
                                 DrugExpiryDate = d.DrugExpiryDate,
                                 DrugName = d.DrugName,
                                 FloorId = d.FloorId,
                                 IsActive = d.IsActive,
                                 LocationId = d.LocationId,
                                 StockTotal = d.StockTotal,
                                 SubDepartmentId = d.SubDepartmentId,
                                 DrugId = d.DrugId,
                                 Location = l.LocationName,
                                 DepartmentName = dep.DepartmentName,
                                 Floor = f.FloorName
                             }).ToList();


                IEnumerable<DrugViewModel> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.DrugName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.Location != null && c.Location.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.DepartmentName != null && c.DepartmentName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.StockTotal != null && c.StockTotal.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.DrugDescription != null && c.DrugDescription.ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                  c.DrugExpiryDate != null && c.DrugExpiryDate.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.Floor != null && c.Floor.ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())
                                  );
                }
                else
                {
                    filteredCompanies = query;
                }
                var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayedCompanies
                             select new[] {/*c.UserId.ToString(),*/
                                              c.Location,
                                              c.DepartmentName,
                                              c.Floor,
                                              c.DrugId.ToString(),
                                              //c.PlaceAllocation,
                                              c.DrugName,
                                              //c.Extenstion,
                                              c.StockTotal.ToString(),
                                              c.DrugExpiryDate.HasValue?c.DrugExpiryDate.ToString():"",

                                              c.DrugId.ToString()};
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

        public ActionResult AjaxGetFloorDepartment(JQueryDataTableParamModel param)
        {

            if (Request.IsAuthenticated)
            {
                var query = (from d in myapp.tbl_FloorDepartment
                             join l in myapp.tbl_Location on d.LocationId equals l.LocationId
                             join dep in myapp.tbl_Department on d.DepartmentId equals dep.DepartmentId
                             join b in myapp.tbl_Building on d.BuildingId equals b.BuildingId
                             join f in myapp.tbl_Floor on d.FloorId equals f.FloorId
                             select new FloorDepartmentViewModel
                             {
                                 DepartmentId = d.DepartmentId.Value,
                                 BuildingId = d.BuildingId.Value,
                                 BuildingName = b.BuildingName,
                                 FloorName = f.FloorName,
                                 FloorId = f.FloorId,
                                 IsActive = d.IsActive.Value,
                                 LocationId = d.LocationId.Value,
                                 Id = d.Id,
                                 LocationName = l.LocationName,
                                 DepartmentName = dep.DepartmentName
                             }).ToList();


                IEnumerable<FloorDepartmentViewModel> filteredCompanies;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    filteredCompanies = query
                       .Where(c => c.DepartmentId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                    c.BuildingId != null && c.BuildingId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.DepartmentName != null && c.DepartmentName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                   ||
                                  c.BuildingName != null && c.BuildingName.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.FloorName != null && c.FloorName.ToLower().Contains(param.sSearch.ToLower())
                                    ||
                                  c.FloorId != null && c.FloorId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                                  c.LocationName != null && c.LocationName.ToLower().Contains(param.sSearch.ToLower())
                                  );
                }
                else
                {
                    filteredCompanies = query;
                }
                var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                var result = from c in displayedCompanies
                             select new[] {
                                              c.LocationName,
                                              c.DepartmentName,
                                              c.BuildingName,
                                              c.FloorName,
                                              c.Id.ToString()};
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

        public JsonResult SaveFloorDepartment(tbl_FloorDepartment model)
        {
            model.IsActive = true;
            myapp.tbl_FloorDepartment.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteFloorDepartment(int id)
        {
            var model = myapp.tbl_FloorDepartment.Where(l => l.Id == id).ToList();
            myapp.tbl_FloorDepartment.Remove(model[0]);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetFloorsByDepartmentId(int id)
        {
            var model = myapp.tbl_FloorDepartment.Where(l => l.DepartmentId == id).ToList();
            var list = (from m in model
                        join f in myapp.tbl_Floor on m.FloorId equals f.FloorId
                        select f).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageCategory()
        {
            return View();
        }
        public ActionResult ManageCategoryDepartment()
        {
            return View();
        }
        public JsonResult SaveNewCategory(tbl_Category model)
        {
            myapp.tbl_Category.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCategories()
        {
            var query = (from d in myapp.tbl_Category select d).OrderBy(l => l.Name).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCategory(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_Category select d).ToList();
            IEnumerable<tbl_Category> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.CategoryId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Description != null && c.Description.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                                              c.CategoryId.ToString(),
                                              c.Name,
                                              c.Description,
                                              c.SLAHours,
                                              c.Priority,
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.CategoryId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateCategory(tbl_Category model)
        {
            var cat = myapp.tbl_Category.Where(l => l.CategoryId == model.CategoryId).ToList();
            if (cat.Count > 0)
            {
                cat[0].Name = model.Name;
                cat[0].Description = model.Description;
                cat[0].SLAHours = model.SLAHours;
                cat[0].Priority = model.Priority;
                cat[0].IsActive = model.IsActive;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCategory(int id)
        {
            var cat = myapp.tbl_Category.Where(l => l.CategoryId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_Category.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveNewDepartmentCategory(tbl_DepartmentVsCategory model)
        {
            myapp.tbl_DepartmentVsCategory.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetDepartmentCategory(JQueryDataTableParamModel param)
        {
            var model = (from d in myapp.tbl_DepartmentVsCategory
                         join l in myapp.tbl_Location on d.LocationId equals l.LocationId
                         join dep in myapp.tbl_Department on d.DepartmentId equals dep.DepartmentId
                         join cat in myapp.tbl_Category on d.CategoryId equals cat.CategoryId
                         select new DepartmentVsCategoryViewModel
                         {
                             Id = d.Id,
                             CategoryId = d.CategoryId.Value,
                             DepartmentId = d.DepartmentId.Value,
                             LocationId = d.LocationId.Value,
                             IsActive = d.IsActive.Value,
                             LocationName = l.LocationName,
                             DepartmentName = dep.DepartmentName,
                             Name = cat.Name
                         }).ToList();
            IEnumerable<DepartmentVsCategoryViewModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = model
                   .Where(c => c.Id.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.DepartmentName != null && c.DepartmentName.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = model;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                                              c.Id.ToString(),
                                              c.LocationName,
                                              c.DepartmentName,
                                              c.Name,
                                              c.IsActive.ToString(),
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = model.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateDepartmentCategory(tbl_DepartmentVsCategory model)
        {
            var cat = myapp.tbl_DepartmentVsCategory.Where(l => l.Id == model.Id).ToList();
            if (cat.Count > 0)
            {
                cat[0].CategoryId = model.CategoryId;
                cat[0].DepartmentId = model.DepartmentId;
                cat[0].LocationId = model.LocationId;
                cat[0].IsActive = model.IsActive;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteDepartmentCategory(int id)
        {
            var cat = myapp.tbl_DepartmentVsCategory.Where(l => l.Id == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_DepartmentVsCategory.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        #region vendor
        public ActionResult ManageVendor()
        {
            return View();
        }
        public ActionResult GetVendorlist()
        {
            var query = (from d in myapp.tbl_Vendor select d).ToList();
            query = query.OrderBy(l => l.Name).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetVendor(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_Vendor select d).ToList();
            query = query.OrderByDescending(l => l.VendorId).ToList();
            IEnumerable<tbl_Vendor> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.VendorId.ToString().ToLower().Contains(param.sSearch.ToLower())
                   ||
  c.Name != null && c.Name.ToLower().Contains(param.sSearch.ToLower())
  ||
                                c.VendorType != null && c.VendorType.ToLower().Contains(param.sSearch.ToLower())
                               ||

                                c.PersonalMobile != null && c.PersonalMobile.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Description != null && c.Description.ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                             c.VendorId.ToString(),
                             c.Name,
                               c.VendorType,
                             c.Description,
                             c.Address,
                             c.PersonalMobile,
                             c.OfficeNumber,
                            c.Email, c.GSTNumber,
                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                               c.VendorId.ToString()
                                             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> SaveVendor(tbl_Vendor model)
        {
            if (model.CreatedBy != null && model.CreatedBy != "")
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.CreatedBy);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Vendor");
                }
            }
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            myapp.tbl_Vendor.Add(model);
            await myapp.SaveChangesAsync();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetVendorbyid(int VendorId)
        {
            var query = myapp.tbl_Vendor.Where(l => l.VendorId == VendorId).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetVendors()
        {
            var query = myapp.tbl_Vendor.ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateVendor(tbl_Vendor model)
        {
            var cat = myapp.tbl_Vendor.Where(l => l.VendorId == model.VendorId).ToList();
            if (cat.Count > 0)
            {
                cat[0].VendorType = model.VendorType;
                cat[0].Address = model.Address;
                cat[0].PersonalMobile = model.PersonalMobile;
                cat[0].OfficeNumber = model.OfficeNumber;
                cat[0].Email = model.Email;
                cat[0].Name = model.Name;
                cat[0].Description = model.Description;
                cat[0].IsActive = model.IsActive;
                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeactiveVendor(int id)
        {
            var tb = myapp.tbl_Vendor.Where(a => a.VendorId == id).SingleOrDefault();
            if (tb.IsActive == true)
            {
                tb.IsActive = false;
            }
            else
            {
                tb.IsActive = true;
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteVendor(int Id)
        {
            var tb = myapp.tbl_Vendor.Where(a => a.VendorId == Id).SingleOrDefault();
            myapp.tbl_Vendor.Remove(tb);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        #endregion


        public ActionResult ManageParkingLocation()
        {
            return View();
        }

        public JsonResult SaveNewParkingLocation(tbl_ParkingLocation model)
        {
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_ParkingLocation.Add(model);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllParkingLocations()
        {
            var query = (from d in myappregister.tbl_ParkingLocation select d).OrderBy(l => l.ParkingLocation).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetParkingLocation(JQueryDataTableParamModel param)
        {

            var query = (from d in myappregister.tbl_ParkingLocation select d).ToList();
            IEnumerable<tbl_ParkingLocation> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.ParkingLocation != null && c.ParkingLocation.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.ParkingType != null && c.ParkingType.ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         select new[] {
                                              c.Id.ToString(),
                                              l.LocationName,
                                              c.ParkingType,
                                              c.ParkingLocation,
                                              c.ParkingVolume.HasValue?c.ParkingVolume.ToString():"0",
                                              c.CreatedBy,
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetParkingLocationById(int id)
        {
            var list = (from a in myappregister.tbl_ParkingLocation
                        where a.Id == id && a.IsActive == true
                        select a).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateParkingLocation(tbl_ParkingLocation model)
        {
            var cat = myappregister.tbl_ParkingLocation.Where(l => l.Id == model.Id).ToList();
            if (cat.Count > 0)
            {
                cat[0].ParkingLocation = model.ParkingLocation;
                cat[0].ParkingType = model.ParkingType;
                cat[0].ParkingVolume = model.ParkingVolume;
                cat[0].IsActive = model.IsActive;
                myappregister.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteParkingLocation(int id)
        {
            var cat = myappregister.tbl_ParkingLocation.Where(l => l.Id == id).ToList();
            if (cat.Count > 0)
            {
                myappregister.tbl_ParkingLocation.Remove(cat[0]);
                myappregister.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageDropdownValues()
        {
            return View();
        }


        public JsonResult SaveNewDropdownValues(tbl_DropdownValues model)
        {

            model.IsActive = true;
            myapp.tbl_DropdownValues.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllDropdownValues()
        {
            var query = (from d in myapp.tbl_DropdownValues select d).OrderBy(l => l.Id).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetDropdownValues(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_DropdownValues select d).ToList();
            IEnumerable<tbl_DropdownValues> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Name != null && c.Name.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Description != null && c.Description.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.DropdownType != null && c.DropdownType.ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                                              c.Id.ToString(),
                                              c.DropdownType,
                                              c.Name,
                                              c.Description,
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDropdownValueById(int id)
        {
            var list = (from a in myapp.tbl_DropdownValues
                        where a.Id == id && a.IsActive == true
                        select a).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateDropdownValues(tbl_DropdownValues model)
        {
            var cat = myapp.tbl_DropdownValues.Where(l => l.Id == model.Id).ToList();
            if (cat.Count > 0)
            {
                cat[0].Description = model.Description;
                cat[0].DropdownType = model.DropdownType;
                cat[0].Name = model.Name;
                cat[0].IsActive = model.IsActive;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteDropdownValues(int id)
        {
            var cat = myapp.tbl_DropdownValues.Where(l => l.Id == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_DropdownValues.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}