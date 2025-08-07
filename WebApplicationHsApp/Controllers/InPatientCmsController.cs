using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class InPatientCmsController : Controller
    {
        // GET: InPatientCms
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public ActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> DashBoard()
        {
            InpatientDashBoardViewModel viewModel = new InpatientDashBoardViewModel();
            var query = await (from p in myapp.tbl_cmp_OrderRequestByPatient
                               join m in myapp.tbl_MealType on p.MealtypeId equals m.MealTypeId
                               join o in myapp.tbl_cmp_OrderRequest on p.OrderRequestId equals o.OrderRequestId
                               select new
                               {
                                   MealTypeName = m.Name,
                                   RequestType = o.RequestType,
                                   IsDellvered = p.IsDelivered
                               }).ToListAsync();
            viewModel.BreakFastNotDelivered = query.Where(m => m.IsDellvered == false && m.MealTypeName == "BreakFast").Count();
            viewModel.BreakFastTotalDelivered = query.Where(m => m.IsDellvered == true && m.MealTypeName == "BreakFast").Count();
            viewModel.BreakFastOrderedtoKitchen = query.Where(m => m.RequestType == "To Kitchen" && m.MealTypeName == "BreakFast").Count();
            viewModel.BreakFastOrderedtoCafeteria = query.Where(m => m.RequestType == "To Cafeteria" && m.MealTypeName == "BreakFast").Count();

            viewModel.LunchNotDelivered = query.Where(m => m.IsDellvered == false && m.MealTypeName == "Lunch").Count();
            viewModel.LunchTotalDelivered = query.Where(m => m.IsDellvered == true && m.MealTypeName == "Lunch").Count();
            viewModel.LunchOrderedtoKitchen = query.Where(m => m.RequestType == "To Kitchen" && m.MealTypeName == "Lunch").Count();
            viewModel.LunchOrderedtoCafeteria = query.Where(m => m.RequestType == "To Cafeteria" && m.MealTypeName == "Lunch").Count();

            viewModel.DinnerNotDelivered = query.Where(m => m.IsDellvered == false && m.MealTypeName == "Dinner").Count();
            viewModel.DinnerTotalDelivered = query.Where(m => m.IsDellvered == true && m.MealTypeName == "Dinner").Count();
            viewModel.DinnerOrderedtoKitchen = query.Where(m => m.RequestType == "To Kitchen" && m.MealTypeName == "Dinner").Count();
            viewModel.DinnerOrderedtoCafeteria = query.Where(m => m.RequestType == "To Cafeteria" && m.MealTypeName == "Dinner").Count();
            return View(viewModel);
        }
        public ActionResult ManageDietPlans()
        {
            return View();
        }
        public ActionResult ManageOrders()
        {
            return View();
        }
        public ActionResult CreateOrderRequestAttendant()
        {
            var mealtype = myapp.tbl_cm_MealType.ToList();
            ViewBag.MealType = 0;
            DateTime dt = DateTime.Now;
            if (TimeBetween("04:00", "09:00"))
            {
                var mt = mealtype.Where(l => l.MealTypeName == "Lunch").SingleOrDefault();
                if (mt != null)
                    ViewBag.MealTypeId = mt.MealTypeId;
                ViewBag.RequestType = "To Kitchen";

            }
            else if (TimeBetween("10:00", "11:30"))
            {
                var mt = mealtype.Where(l => l.MealTypeName == "Snacks").SingleOrDefault();
                if (mt != null)
                    ViewBag.MealTypeId = mt.MealTypeId;
                ViewBag.RequestType = "To Kitchen";

            }
            else if (TimeBetween("12:00", "13:30"))
            {
                var mt = mealtype.Where(l => l.MealTypeName == "Dinner").SingleOrDefault();
                if (mt != null)
                    ViewBag.MealTypeId = mt.MealTypeId;
                ViewBag.RequestType = "To Kitchen";

            }
            else if (TimeBetween("15:00", "17:00"))
            {
                var mt = mealtype.Where(l => l.MealTypeName == "Breakfast").SingleOrDefault();
                if (mt != null)
                    ViewBag.MealTypeId = mt.MealTypeId;

                ViewBag.RequestType = "To Kitchen";
            }
            else
            {
                ViewBag.RequestType = "To Cafeteria";
            }


            return View();
        }
        public ActionResult CreateOrderRequest()
        {
            var mealtype = myapp.tbl_cm_MealType.ToList();
            ViewBag.MealType = 0;
            DateTime dt = DateTime.Now;
            if (TimeBetween("04:00", "09:00"))
            {
                var mt = mealtype.Where(l => l.MealTypeName == "Lunch").SingleOrDefault();
                if (mt != null)
                    ViewBag.MealTypeId = mt.MealTypeId;
                ViewBag.RequestType = "To Kitchen";

            }
            else if (TimeBetween("10:00", "11:30"))
            {
                var mt = mealtype.Where(l => l.MealTypeName == "Snacks").SingleOrDefault();
                if (mt != null)
                    ViewBag.MealTypeId = mt.MealTypeId;
                ViewBag.RequestType = "To Kitchen";

            }
            else if (TimeBetween("12:00", "13:30"))
            {
                var mt = mealtype.Where(l => l.MealTypeName == "Dinner").SingleOrDefault();
                if (mt != null)
                    ViewBag.MealTypeId = mt.MealTypeId;
                ViewBag.RequestType = "To Kitchen";

            }
            else if (TimeBetween("15:00", "17:00"))
            {
                var mt = mealtype.Where(l => l.MealTypeName == "Breakfast").SingleOrDefault();
                if (mt != null)
                    ViewBag.MealTypeId = mt.MealTypeId;

                ViewBag.RequestType = "To Kitchen";
            }
            else
            {
                ViewBag.RequestType = "To Cafeteria";
            }


            return View();
        }
        private bool TimeBetween(string startime, string endtime)
        {
            // convert datetime to a TimeSpan
            TimeSpan now = DateTime.Now.TimeOfDay;
            TimeSpan start = TimeSpan.Parse(startime);
            TimeSpan end = TimeSpan.Parse(endtime);
            // see if start comes before end
            if (start < end)
                return start <= now && now <= end;
            // start is after end, so do the inverse comparison
            return !(end < now && now < start);
        }
        public ActionResult CreateOrderRequest2()
        {
            ViewBag.MealType = 0;
            ViewBag.RequestType = "To Cafeteria";
            return View();
        }
        public ActionResult EditOrderRequest(int id = 0, string status = "")
        {
            ViewBag.id = id;
            ViewBag.status = status;
            return View();
        }
        public ActionResult EditOrderRequestAttendendent(int id = 0, string status = "")
        {
            ViewBag.id = id;
            ViewBag.status = status;
            return View();
        }
        public ActionResult ManageDiets()
        {
            return View();
        }
        public ActionResult AssignDietPlans()
        {
            return View();
        }
        public ActionResult KitchenManager()
        {
            return View();
        }
        public ActionResult ManageDietitianOrders()
        {
            return View();
        }
        public ActionResult ManageCafeteriaOrders()
        {
            return View();
        }
        public ActionResult ManageSupervisorOrders()
        {
            return View();
        }
        public ActionResult FoodCalculation()
        {
            return View();
        }
        public ActionResult AjaxGetCMSPDiet(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cmp_Diet select d).OrderByDescending(l => l.DietId).ToList();
            IEnumerable<tbl_cmp_Diet> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.DietId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.DietName != null && c.DietName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                                              c.DietId.ToString(),
                                              c.DietName,
                                              c.DietType,
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.DietId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save_UpdateCMSPDiet(tbl_cmp_Diet model)
        {
            tbl_cmp_Diet _diet = new tbl_cmp_Diet();
            if (model.DietId != 0)
            {
                _diet = myapp.tbl_cmp_Diet.Where(m => m.DietId == model.DietId).FirstOrDefault();
                _diet.ModifiedBy = User.Identity.Name;
                _diet.ModifiedOn = DateTime.Now;
            }
            _diet.DietName = model.DietName;
            _diet.DietType = model.DietType;
            _diet.IsActive = model.IsActive;
            if (model.DietId == 0)
            {
                _diet.CreatedBy = User.Identity.Name;
                _diet.CreatedOn = DateTime.Now;
                myapp.tbl_cmp_Diet.Add(_diet);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCMPDiet(int id)
        {
            var cat = myapp.tbl_cmp_Diet.Where(l => l.DietId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_cmp_Diet.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDiets()
        {
            var query = myapp.tbl_cmp_Diet.Where(m => m.IsActive == true).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMSP_Location()
        {
            var query = myapp.tbl_Patient.Where(m => m.Bedallocated == true).GroupBy(n => new { n.LocationId, n.LocationName }).Select(g => new { g.Key.LocationId, g.Key.LocationName }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMSP_FloorByLocation(string id)
        {
            var query = myapp.tbl_Patient.Where(m => m.Bedallocated == true && m.LocationId == id).GroupBy(n => new { n.FloorId, n.FloorNo }).Select(g => new { g.Key.FloorId, g.Key.FloorNo }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMSP_LocationLiqud()
        {
            var query = myapp.tbl_Patient.Where(m => m.Bedallocated == true && m.Remarks != "Normal").GroupBy(n => new { n.LocationId, n.LocationName }).Select(g => new { g.Key.LocationId, g.Key.LocationName }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMSP_FloorByLocationLiqud(string id)
        {
            var query = myapp.tbl_Patient.Where(m => m.Bedallocated == true && m.Remarks != "Normal" && m.LocationId == id).GroupBy(n => new { n.FloorId, n.FloorNo }).Select(g => new { g.Key.FloorId, g.Key.FloorNo }).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMSP_OrderPlacebyId(int id)
        {
            var query = myapp.tbl_cmp_OrderRequest.Where(m => m.OrderRequestId == id).SingleOrDefault();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMSP_OrderPlacebyIdAttendent(int id)
        {
            var query = myapp.tbl_cmp_OrderRequestAttendendent.Where(m => m.OrderRequestId == id).SingleOrDefault();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMSPatientsbyLocandFloor(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_Patient where d.Bedallocated == true select d).ToList();
            if (param.FormType != null && param.FormType != "")
            {
                query = query.Where(l => l.Remarks == param.FormType).ToList();
            }

            if (param.floor != null && param.floor != "")
            {
                query = query.Where(m => m.FloorNo == param.floor).ToList();
            }
            else
            {
                query = new List<tbl_Patient>();
            }
            var orders = (from it in myapp.tbl_cmp_OrderRequest
                          join t in myapp.tbl_cmp_OrderRequestByPatient on it.OrderRequestId equals t.OrderRequestId
                          where it.MealTypeId == param.MealType
                          select new InpatientViewModel
                          {
                              PatientId = t.PatientId,
                              DateOfOrder = it.DateOfOrder
                          }).ToList();
            if (param.MealType != null && param.MealType != 0)
            {
                if (param.MealType == 1)
                {
                    if (param.fromdate != null && param.fromdate != "")
                    {
                        DateTime dtorder = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                        var patients = orders.Where(m => m.DateOfOrder.Value.ToString("dd/MM/yyyy") == dtorder.ToString("dd/MM/yyyy")).Select(m => m.PatientId).ToList();
                        if (patients.Count > 0 && query.Count > 0)
                        {
                            query = query.Where(p => !patients.Contains(p.PatientId)).ToList();
                        }
                    }
                    else
                    {
                        var patients = orders.Where(m => m.DateOfOrder.Value.ToString("dd/MM/yyyy") == DateTime.Now.AddDays(1).ToString("dd/MM/yyyy")).Select(m => m.PatientId).ToList();
                        if (patients.Count > 0 && query.Count > 0)
                        {
                            query = query.Where(p => !patients.Contains(p.PatientId)).ToList();
                        }
                    }
                }
                else
                {
                    if (param.fromdate != null && param.fromdate != "")
                    {
                        DateTime dtorder = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                        var patients = orders.Where(m => m.DateOfOrder.Value.ToString("dd/MM/yyyy") == dtorder.ToString("dd/MM/yyyy")).Select(m => m.PatientId).ToList();
                        if (patients.Count > 0 && query.Count > 0)
                        {
                            query = query.Where(p => !patients.Contains(p.PatientId)).ToList();
                        }
                    }
                    else
                    {
                        var patients = orders.Where(m => m.DateOfOrder.Value.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy")).Select(m => m.PatientId).ToList();
                        if (patients.Count > 0 && query.Count > 0)
                        {
                            query = query.Where(p => !patients.Contains(p.PatientId)).ToList();
                        }
                    }
                }
            }
            query = query.OrderBy(l => l.BedNo).ToList();
            IEnumerable<tbl_Patient> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PatientId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.RoomNo != null && c.RoomNo.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                             c.PatientId.ToString(),
                             c.Name,
                             c.MRNo,
                             c.RoomNo+" "+c.BedNo,
                             c.MobileNumber,
                             c.PatientId.ToString(),
                             c.PatientId.ToString()
                              //c.PatientId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetCMSPatientsbyLocandFloorAttendendent(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_Patient where d.Bedallocated == true select d).ToList();
            if (param.FormType != null && param.FormType != "")
            {
                query = query.Where(l => l.Remarks == param.FormType).ToList();
            }

            if (param.floor != null && param.floor != "")
            {
                query = query.Where(m => m.FloorNo == param.floor).ToList();
            }
            else
            {
                query = new List<tbl_Patient>();
            }
            var orders = (from it in myapp.tbl_cmp_OrderRequestAttendendent
                          join t in myapp.tbl_cmp_OrderRequestAttendendentByPatient on it.OrderRequestId equals t.OrderRequestId
                          where it.MealTypeId == param.MealType
                          select new InpatientViewModel
                          {
                              PatientId = t.PatientId,
                              DateOfOrder = it.DateOfOrder
                          }).ToList();
            if (param.MealType != null && param.MealType != 0)
            {
                if (param.MealType == 1)
                {
                    if (param.fromdate != null && param.fromdate != "")
                    {
                        DateTime dtorder = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                        var patients = orders.Where(m => m.DateOfOrder.Value.ToString("dd/MM/yyyy") == dtorder.ToString("dd/MM/yyyy")).Select(m => m.PatientId).ToList();
                        if (patients.Count > 0 && query.Count > 0)
                        {
                            query = query.Where(p => !patients.Contains(p.PatientId)).ToList();
                        }
                    }
                    else
                    {
                        var patients = orders.Where(m => m.DateOfOrder.Value.ToString("dd/MM/yyyy") == DateTime.Now.AddDays(1).ToString("dd/MM/yyyy")).Select(m => m.PatientId).ToList();
                        if (patients.Count > 0 && query.Count > 0)
                        {
                            query = query.Where(p => !patients.Contains(p.PatientId)).ToList();
                        }
                    }
                }
                else
                {
                    if (param.fromdate != null && param.fromdate != "")
                    {
                        DateTime dtorder = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                        var patients = orders.Where(m => m.DateOfOrder.Value.ToString("dd/MM/yyyy") == dtorder.ToString("dd/MM/yyyy")).Select(m => m.PatientId).ToList();
                        if (patients.Count > 0 && query.Count > 0)
                        {
                            query = query.Where(p => !patients.Contains(p.PatientId)).ToList();
                        }
                    }
                    else
                    {
                        var patients = orders.Where(m => m.DateOfOrder.Value.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy")).Select(m => m.PatientId).ToList();
                        if (patients.Count > 0 && query.Count > 0)
                        {
                            query = query.Where(p => !patients.Contains(p.PatientId)).ToList();
                        }
                    }
                }
            }
            query = query.OrderBy(l => l.BedNo).ToList();
            IEnumerable<tbl_Patient> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PatientId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.RoomNo != null && c.RoomNo.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                             c.PatientId.ToString(),
                             c.Name,
                             c.MRNo,
                             c.RoomNo+" "+c.BedNo,
                             c.MobileNumber,
                             c.PatientId.ToString(),
                             c.PatientId.ToString()
                              //c.PatientId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveOrderRequestPatientsListAttendendent(List<tbl_cmp_OrderRequestAttendendentByPatient> model)
        {
            for (int i = 0; i < model.Count; i++)
            {
                //var checkcount = myapp.tbl_cmp_OrderRequestByPatient.Where(l => l.MealtypeId == model[i].MealtypeId
                //  && l.PatientId == model[i].PatientId && l.AssignBedNo == model[i].AssignBedNo
                //  && l.AssignRoomNo == model[i].AssignBedNo).Count();
                //if (checkcount == 0)
                //{
                model[i].CreatedBy = User.Identity.Name;
                model[i].CreatedOn = DateTime.Now;
                model[i].IsDelivered = false;
                myapp.tbl_cmp_OrderRequestAttendendentByPatient.Add(model[i]);
                myapp.SaveChanges();
                //}
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveOrderRequestPatientsList(List<tbl_cmp_OrderRequestByPatient> model)
        {
            for (int i = 0; i < model.Count; i++)
            {
                //var checkcount = myapp.tbl_cmp_OrderRequestByPatient.Where(l => l.MealtypeId == model[i].MealtypeId
                //  && l.PatientId == model[i].PatientId && l.AssignBedNo == model[i].AssignBedNo
                //  && l.AssignRoomNo == model[i].AssignBedNo).Count();
                //if (checkcount == 0)
                //{
                model[i].CreatedBy = User.Identity.Name;
                model[i].CreatedOn = DateTime.Now;
                model[i].IsDelivered = false;
                myapp.tbl_cmp_OrderRequestByPatient.Add(model[i]);
                myapp.SaveChanges();
                //}
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveCMSPOrderRequestAttendendent(tbl_cmp_OrderRequestAttendendent model)
        {
            try
            {
                var id = User.Identity.Name;
                var time = DateTime.Now.TimeOfDay;
                //if (time > new TimeSpan(15, 00, 00) && time < new TimeSpan(19, 00, 00))
                //{
                //    model.DateOfOrder = DateTime.Now.AddDays(1);
                //}
                //else
                //{
                //    model.DateOfOrder = DateTime.Now;
                //}
                if (model.ModifiedBy != null && model.ModifiedBy != "")
                {
                    model.DateOfOrder = ProjectConvert.ConverDateStringtoDatetime(model.ModifiedBy);
                }
                model.RequestUserId = (from V in myapp.tbl_User where V.CustomUserId == id select V.UserId).SingleOrDefault();
                model.CreatedBy = User.Identity.Name;
                model.CreatedOn = DateTime.Now;
                if ((DateTime.Now.Hour >= 4 && DateTime.Now.Hour < 10) || DateTime.Now.Hour == 11 || DateTime.Now.Hour == 13 || (DateTime.Now.Hour >= 15 && DateTime.Now.Hour < 19))
                {
                    model.RequestType = "To Kitchen";
                }
                else
                {
                    model.RequestType = "To Cafeteria";
                }

                if (model.RequestType == "To Kitchen")
                    model.CurrentStatus = InPatientWorkflow.InDietitian.ToString();
                else
                    model.CurrentStatus = InPatientWorkflow.InCafeteria.ToString();
                List<string> employees = new List<string>();
                employees.Add("5427"); employees.Add("4415"); employees.Add("2195"); employees.Add("2094"); employees.Add("2134");
                employees.Add("1834"); employees.Add("900336"); employees.Add("3809"); employees.Add("6944");
                if (employees.Contains(User.Identity.Name))
                {
                    //model.DateOfOrder = DateTime.Now;
                    model.RequestType = "To Kitchen";
                    model.CurrentStatus = InPatientWorkflow.InKitchen.ToString();
                    //if (time > new TimeSpan(16, 00, 00))
                    //{
                    //    model.DateOfOrder = DateTime.Now.AddDays(1);
                    //}
                }
                myapp.tbl_cmp_OrderRequestAttendendent.Add(model);
                myapp.SaveChanges();

                return Json(model.OrderRequestId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult SaveCMSPOrderRequest(tbl_cmp_OrderRequest model)
        {
            try
            {
                var id = User.Identity.Name;
                var time = DateTime.Now.TimeOfDay;
                //if (time > new TimeSpan(15, 00, 00) && time < new TimeSpan(19, 00, 00))
                //{
                //    model.DateOfOrder = DateTime.Now.AddDays(1);
                //}
                //else
                //{
                //    model.DateOfOrder = DateTime.Now;
                //}
                model.DateOfOrder = DateTime.Now;
                if (model.ModifiedBy != null && model.ModifiedBy != "")
                {
                    model.DateOfOrder = ProjectConvert.ConverDateStringtoDatetime(model.ModifiedBy);
                }
                model.RequestUserId = (from V in myapp.tbl_User where V.CustomUserId == id select V.UserId).SingleOrDefault();
                model.CreatedBy = User.Identity.Name;
                model.CreatedOn = DateTime.Now;
                if ((DateTime.Now.Hour >= 4 && DateTime.Now.Hour < 10) || DateTime.Now.Hour == 11 || DateTime.Now.Hour == 13 || (DateTime.Now.Hour >= 15 && DateTime.Now.Hour < 19))
                {
                    model.RequestType = "To Kitchen";
                }
                else
                {
                    model.RequestType = "To Cafeteria";
                }

                if (model.RequestType == "To Kitchen")
                    model.CurrentStatus = InPatientWorkflow.InDietitian.ToString();
                else
                    model.CurrentStatus = InPatientWorkflow.InCafeteria.ToString();
                List<string> employees = new List<string>();
                employees.Add("5427"); employees.Add("4415"); employees.Add("2195"); employees.Add("2094"); employees.Add("2134");
                employees.Add("1834"); employees.Add("900336"); employees.Add("3809"); employees.Add("6944");
                if (employees.Contains(User.Identity.Name))
                {
                    //model.DateOfOrder = DateTime.Now;
                    model.RequestType = "To Kitchen";
                    model.CurrentStatus = InPatientWorkflow.InKitchen.ToString();
                    //if (time > new TimeSpan(16, 00, 00))
                    //{
                    //    model.DateOfOrder = DateTime.Now.AddDays(1);
                    //}
                }
                myapp.tbl_cmp_OrderRequest.Add(model);
                myapp.SaveChanges();

                return Json(model.OrderRequestId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        public ActionResult UpdateOrderRequestPatientsList(List<tbl_cmp_OrderRequestByPatient> model)
        {
            for (int i = 0; i < model.Count; i++)
            {
                var id = model[i].PatientDietId;
                var dbModel = myapp.tbl_cmp_OrderRequestByPatient.Where(m => m.PatientDietId == id).SingleOrDefault();
                dbModel.ModifiedBy = User.Identity.Name;
                dbModel.ModifiedOn = DateTime.Now;
                dbModel.MealtypeId = model[i].MealtypeId;
                dbModel.DietId = model[i].DietId;
                dbModel.Notes = model[i].Notes;
                dbModel.OrderRequestId = model[i].OrderRequestId;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateSupervisorOrderRequestPatientsList(List<tbl_cmp_OrderRequestByPatient> model)
        {
            for (int i = 0; i < model.Count; i++)
            {
                var id = model[i].PatientDietId;
                var dbModel = myapp.tbl_cmp_OrderRequestByPatient.Where(m => m.PatientDietId == id).SingleOrDefault();
                dbModel.ModifiedBy = User.Identity.Name;
                dbModel.ModifiedOn = DateTime.Now;
                dbModel.MealtypeId = model[i].MealtypeId;
                dbModel.DietId = model[i].DietId;
                dbModel.Notes = model[i].Notes;
                dbModel.DeliverDietId = model[i].DeliverDietId;
                dbModel.DeliverRoomNo = model[i].DeliverRoomNo;
                dbModel.ReasonToNotDeliver = model[i].ReasonToNotDeliver;
                dbModel.IsDelivered = model[i].IsDelivered;
                dbModel.OrderRequestId = model[i].OrderRequestId;
                myapp.SaveChanges();
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UpdateOrderRequestPatientsListAttendendent(List<tbl_cmp_OrderRequestAttendendentByPatient> model)
        {
            for (int i = 0; i < model.Count; i++)
            {
                var id = model[i].PatientDietId;
                var dbModel = myapp.tbl_cmp_OrderRequestAttendendentByPatient.Where(m => m.PatientDietId == id).SingleOrDefault();
                dbModel.ModifiedBy = User.Identity.Name;
                dbModel.ModifiedOn = DateTime.Now;
                dbModel.MealtypeId = model[i].MealtypeId;
                dbModel.DietId = model[i].DietId;
                dbModel.Notes = model[i].Notes;
                dbModel.OrderRequestId = model[i].OrderRequestId;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult UpdateSupervisorOrderRequestPatientsListAttendendent(List<tbl_cmp_OrderRequestAttendendentByPatient> model)
        {
            for (int i = 0; i < model.Count; i++)
            {
                var id = model[i].PatientDietId;
                var dbModel = myapp.tbl_cmp_OrderRequestAttendendentByPatient.Where(m => m.PatientDietId == id).SingleOrDefault();
                dbModel.ModifiedBy = User.Identity.Name;
                dbModel.ModifiedOn = DateTime.Now;
                dbModel.MealtypeId = model[i].MealtypeId;
                dbModel.DietId = model[i].DietId;
                dbModel.Notes = model[i].Notes;
                dbModel.DeliverDietId = model[i].DeliverDietId;
                dbModel.DeliverRoomNo = model[i].DeliverRoomNo;
                dbModel.ReasonToNotDeliver = model[i].ReasonToNotDeliver;
                dbModel.IsDelivered = model[i].IsDelivered;
                dbModel.OrderRequestId = model[i].OrderRequestId;
                myapp.SaveChanges();
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateCMSPOrderRequestAttendendent(tbl_cmp_OrderRequestAttendendent model)
        {
            try
            {
                var orderid = model.OrderRequestId;
                var dbModel = myapp.tbl_cmp_OrderRequestAttendendent.Where(m => m.OrderRequestId == orderid).SingleOrDefault();
                switch (model.ModifiedBy)
                {
                    case "InDietitian":
                        dbModel.DietitianEmpId = int.Parse(User.Identity.Name);
                        dbModel.DietitianActionDate = DateTime.Now;
                        break;
                    case "InKitchen":
                        dbModel.KitchenEmpId = int.Parse(User.Identity.Name);
                        dbModel.KitchenActionDate = DateTime.Now;
                        break;
                    case "InCafeteria":
                        dbModel.CafeteriaEmpId = int.Parse(User.Identity.Name);
                        dbModel.CafeteriaActionDate = DateTime.Now;
                        break;
                    case "InSupervisor":
                        dbModel.SupervisorEmpId = int.Parse(User.Identity.Name);
                        dbModel.SupervisorActionDate = DateTime.Now;
                        break;
                }
                var id = User.Identity.Name;
                dbModel.RequestUserId = (from V in myapp.tbl_User where V.CustomUserId == id select V.UserId).SingleOrDefault();
                dbModel.ModifiedBy = User.Identity.Name;
                dbModel.ModifiedOn = DateTime.Now;
                dbModel.MealTypeId = model.MealTypeId;
                dbModel.RequestNotes = model.RequestNotes;
                dbModel.CurrentStatus = model.CurrentStatus;
                myapp.SaveChanges();
                return Json(model.OrderRequestId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult UpdateCMSPOrderRequest(tbl_cmp_OrderRequest model)
        {
            try
            {
                var orderid = model.OrderRequestId;
                var dbModel = myapp.tbl_cmp_OrderRequest.Where(m => m.OrderRequestId == orderid).SingleOrDefault();
                switch (model.ModifiedBy)
                {
                    case "InDietitian":
                        dbModel.DietitianEmpId = int.Parse(User.Identity.Name);
                        dbModel.DietitianActionDate = DateTime.Now;
                        break;
                    case "InKitchen":
                        dbModel.KitchenEmpId = int.Parse(User.Identity.Name);
                        dbModel.KitchenActionDate = DateTime.Now;
                        break;
                    case "InCafeteria":
                        dbModel.CafeteriaEmpId = int.Parse(User.Identity.Name);
                        dbModel.CafeteriaActionDate = DateTime.Now;
                        break;
                    case "InSupervisor":
                        dbModel.SupervisorEmpId = int.Parse(User.Identity.Name);
                        dbModel.SupervisorActionDate = DateTime.Now;
                        break;
                }
                var id = User.Identity.Name;
                dbModel.RequestUserId = (from V in myapp.tbl_User where V.CustomUserId == id select V.UserId).SingleOrDefault();
                dbModel.ModifiedBy = User.Identity.Name;
                dbModel.ModifiedOn = DateTime.Now;
                dbModel.MealTypeId = model.MealTypeId;
                dbModel.RequestNotes = model.RequestNotes;
                dbModel.CurrentStatus = model.CurrentStatus;
                myapp.SaveChanges();
                return Json(model.OrderRequestId, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult AjaxGetCMSPatientsbyOrderId(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cmp_OrderRequestByPatient select d).ToList();

            if (param.OrderRequestId != null && param.OrderRequestId != 0)
            {
                query = query.Where(m => m.OrderRequestId == param.OrderRequestId).ToList();
            }
            else
            {
                query = new List<tbl_cmp_OrderRequestByPatient>();
            }
            IEnumerable<tbl_cmp_OrderRequestByPatient> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PatientId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.PatientDietId != null && c.PatientDietId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.DietId != null && c.DietId.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join p in myapp.tbl_Patient on c.PatientId equals p.PatientId
                         select new[] {
                             c.PatientDietId.ToString(),
                                              c.PatientId.ToString(),
                                            p.Name,
                                            p.MRNo,
                             p.RoomNo +" "+p.BedNo,

                                                    p.MobileNumber,

                          c.PatientDietId.ToString() + "-"+c.DietId, c.PatientDietId.ToString()+ "-"+c.Notes};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMSPatientsbyOrderIdAttendendent(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cmp_OrderRequestAttendendentByPatient select d).ToList();

            if (param.OrderRequestId != null && param.OrderRequestId != 0)
            {
                query = query.Where(m => m.OrderRequestId == param.OrderRequestId).ToList();
            }
            else
            {
                query = new List<tbl_cmp_OrderRequestAttendendentByPatient>();
            }
            IEnumerable<tbl_cmp_OrderRequestAttendendentByPatient> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PatientId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.PatientDietId != null && c.PatientDietId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.DietId != null && c.DietId.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join p in myapp.tbl_Patient on c.PatientId equals p.PatientId
                         select new[] {
                             c.PatientDietId.ToString(),
                                              c.PatientId.ToString(),
                                            p.Name,
                                            p.MRNo,
                             p.RoomNo +" "+p.BedNo,

                                                    p.MobileNumber,

                          c.PatientDietId.ToString() + "-"+c.DietId, c.PatientDietId.ToString()+ "-"+c.Notes};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetSupervisorCMSPatientsbyOrderId(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cmp_OrderRequestByPatient select d).ToList();

            if (param.OrderRequestId != null && param.OrderRequestId != 0)
            {
                query = query.Where(m => m.OrderRequestId == param.OrderRequestId).ToList();
            }
            else
            {
                query = new List<tbl_cmp_OrderRequestByPatient>();
            }
            IEnumerable<tbl_cmp_OrderRequestByPatient> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PatientId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.PatientDietId != null && c.PatientDietId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.DietId != null && c.DietId.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join p in myapp.tbl_Patient on c.PatientId equals p.PatientId
                         select new[] {
                             c.PatientDietId.ToString(),
                                              c.PatientId.ToString(),
                                            p.Name,
                                            p.MRNo,
                            p.RoomNo+" "+p.BedNo,
                              c.PatientDietId.ToString() + "-"+(c.DeliverRoomNo!=null?c.DeliverRoomNo: p.RoomNo+" "+p.BedNo),
                                                    p.MobileNumber,

                          c.PatientDietId.ToString() + "-"+c.DietId,
                             c.PatientDietId.ToString()+ "-"+c.Notes,
                             c.PatientDietId.ToString()+ "-"+(c.IsDelivered!=null?(c.IsDelivered==true?"Yes":"No"):"Yes"),
                         c.PatientDietId.ToString()+ "-"+(c.ReasonToNotDeliver!=null?c.ReasonToNotDeliver:" "),
                         c.PatientDietId.ToString()+ "-"+(c.DeliverDietId!=null?c.DeliverDietId:c.DietId),
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AjaxGetSupervisorCMSPatientsbyOrderIdAttendendent(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cmp_OrderRequestAttendendentByPatient select d).ToList();

            if (param.OrderRequestId != null && param.OrderRequestId != 0)
            {
                query = query.Where(m => m.OrderRequestId == param.OrderRequestId).ToList();
            }
            else
            {
                query = new List<tbl_cmp_OrderRequestAttendendentByPatient>();
            }
            IEnumerable<tbl_cmp_OrderRequestAttendendentByPatient> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PatientId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.PatientDietId != null && c.PatientDietId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.DietId != null && c.DietId.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join p in myapp.tbl_Patient on c.PatientId equals p.PatientId
                         select new[] {
                             c.PatientDietId.ToString(),
                                              c.PatientId.ToString(),
                                            p.Name,
                                            p.MRNo,
                            p.RoomNo+" "+p.BedNo,
                              c.PatientDietId.ToString() + "-"+(c.DeliverRoomNo!=null?c.DeliverRoomNo: p.RoomNo+" "+p.BedNo),
                                                    p.MobileNumber,

                          c.PatientDietId.ToString() + "-"+c.DietId,
                             c.PatientDietId.ToString()+ "-"+c.Notes,
                             c.PatientDietId.ToString()+ "-"+(c.IsDelivered!=null?(c.IsDelivered==true?"Yes":"No"):"Yes"),
                         c.PatientDietId.ToString()+ "-"+(c.ReasonToNotDeliver!=null?c.ReasonToNotDeliver:" "),
                         c.PatientDietId.ToString()+ "-"+(c.DeliverDietId!=null?c.DeliverDietId:c.DietId),
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMpOrderRequest(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cmp_OrderRequest  select d).OrderByDescending(l => l.OrderRequestId).ToList();


            IEnumerable<tbl_cmp_OrderRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.OrderRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.MealTypeId != null && c.MealTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join m in myapp.tbl_cm_MealType on c.MealTypeId equals m.MealTypeId
                         join u in myapp.tbl_User on c.RequestUserId equals u.UserId
                         select new[] {
                             c.OrderRequestId.ToString(),
                             c.DateOfOrder.Value.ToString("dd/MM/yyyy"),
                             c.LocationId,
                             c.FloorId,
                             m.MealTypeName,
                             u.FirstName + " "+u.LastName,
                             c.RequestNotes,c.CurrentStatus,c.OrderRequestId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetCMpOrderRequestAttendendent(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cmp_OrderRequestAttendendent select d).OrderByDescending(l => l.OrderRequestId).ToList();


            IEnumerable<tbl_cmp_OrderRequestAttendendent> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.OrderRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.MealTypeId != null && c.MealTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join m in myapp.tbl_cm_MealType on c.MealTypeId equals m.MealTypeId
                         join u in myapp.tbl_User on c.RequestUserId equals u.UserId
                         select new[] {
                             c.OrderRequestId.ToString(),
                             c.DateOfOrder.Value.ToString("dd/MM/yyyy"),
                             c.LocationId,
                             c.FloorId,
                             m.MealTypeName,
                             u.FirstName + " "+u.LastName,
                             c.RequestNotes,c.CurrentStatus,c.OrderRequestId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMpCafeteriaOrderRequest(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cmp_OrderRequest where d.CurrentStatus == InPatientWorkflow.InCafeteria.ToString() select d).OrderByDescending(l => l.OrderRequestId).ToList();


            IEnumerable<tbl_cmp_OrderRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.OrderRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.MealTypeId != null && c.MealTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join m in myapp.tbl_cm_MealType on c.MealTypeId equals m.MealTypeId
                         join u in myapp.tbl_User on c.RequestUserId equals u.UserId
                         select new[] {
                             c.OrderRequestId.ToString(),
                             c.DateOfOrder.Value.ToString("dd/MM/yyyy"),
                             c.LocationId,
                             c.FloorId,
                         m.MealTypeName,
                             u.FirstName + " "+u.LastName,
                             c.RequestNotes,c.CurrentStatus,c.OrderRequestId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMpDietOrderRequest(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_cmp_OrderRequest select d).OrderByDescending(l => l.OrderRequestId).ToList();
            if (param.locationname != null && param.locationname != "" && param.locationname != "Select")
            {
                query = query.Where(m => m.LocationId == param.locationname).ToList();
            }
            if (param.MealType != null && param.MealType != 0)
            {
                query = query.Where(m => m.MealTypeId == param.MealType).ToList();
            }
            if (param.floor != null && param.floor != "" && param.floor != "Select")
            {
                query = query.Where(m => m.FloorId == param.floor).ToList();
            }
            IEnumerable<tbl_cmp_OrderRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.OrderRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.MealTypeId != null && c.MealTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join m in myapp.tbl_cm_MealType on c.MealTypeId equals m.MealTypeId
                         join u in myapp.tbl_User on c.RequestUserId equals u.UserId
                         select new[] {
                             c.OrderRequestId.ToString(),
                             c.DateOfOrder.Value.ToString("dd/MM/yyyy"),
                             c.LocationId,
                             c.FloorId,
                             m.MealTypeName,
                             u.FirstName + " "+u.LastName
                             ,c.RequestNotes,c.CurrentStatus,c.OrderRequestId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMpDietOrderRequestAttendant(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_cmp_OrderRequestAttendendent select d).OrderByDescending(l => l.OrderRequestId).ToList();
            if (param.locationname != null && param.locationname != "" && param.locationname != "Select")
            {
                query = query.Where(m => m.LocationId == param.locationname).ToList();
            }
            if (param.MealType != null && param.MealType != 0)
            {
                query = query.Where(m => m.MealTypeId == param.MealType).ToList();
            }
            if (param.floor != null && param.floor != "" && param.floor != "Select")
            {
                query = query.Where(m => m.FloorId == param.floor).ToList();
            }
            IEnumerable<tbl_cmp_OrderRequestAttendendent> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.OrderRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.MealTypeId != null && c.MealTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join m in myapp.tbl_cm_MealType on c.MealTypeId equals m.MealTypeId
                         join u in myapp.tbl_User on c.RequestUserId equals u.UserId
                         select new[] {
                             c.OrderRequestId.ToString(),
                             c.DateOfOrder.Value.ToString("dd/MM/yyyy"),
                             c.LocationId,
                             c.FloorId,
                             m.MealTypeName,
                             u.FirstName + " "+u.LastName
                             ,c.RequestNotes,c.CurrentStatus,c.OrderRequestId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LoadKitchenOrders(int mealTypeId)
        {
            DateTime dt = DateTime.Now.Date;
            var suborders = (from d in myapp.tbl_cmp_OrderRequest
                             join it in myapp.tbl_cmp_OrderRequestByPatient on d.OrderRequestId equals it.OrderRequestId
                             join p in myapp.tbl_cmp_Diet on it.DietId equals p.DietId
                             where d.CurrentStatus == "InKitchen"
                             select new
                             {
                                 d.LocationId,
                                 d.DateOfOrder,
                                 it.PatientId,
                                 p.DietName,
                                 it.DeliverBedNo,
                                 it.Notes,
                                 d.MealTypeId
                             }).ToList();
            if (mealTypeId != 0)
            {
                suborders = suborders.Where(l => l.MealTypeId == mealTypeId).ToList();
            }
            suborders = suborders.Where(l => l.DateOfOrder.Value.Date == dt).ToList();
            var locations = suborders.Select(l => l.LocationId).Distinct().ToList();
            List<KitchenOrdersViewModel> result = new List<KitchenOrdersViewModel>();
            foreach (var l in locations)
            {
                var dietnames = suborders.Where(s => s.LocationId == l).Select(d => d.DietName).Distinct().ToList();
                foreach (var d in dietnames)
                {
                    var dietlist = suborders.Where(s => s.LocationId == l && s.DietName == d).ToList();
                    KitchenOrdersViewModel m = new KitchenOrdersViewModel();
                    m.DietName = d;
                    m.Location = l;
                    m.Count = dietlist.Count();
                    m.Customizations = string.Join(",", dietlist.Where(c => c.Notes != null && c.Notes != "").Select(c => c.Notes));
                    result.Add(m);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SendKitchenOrdersToSupervisor(int mealTypeId)
        {
            var suborders = (from d in myapp.tbl_cmp_OrderRequest
                             join it in myapp.tbl_cmp_OrderRequestByPatient on d.OrderRequestId equals it.OrderRequestId
                             join p in myapp.tbl_cmp_Diet on it.DietId equals p.DietId
                             where d.MealTypeId == mealTypeId && d.CurrentStatus == InPatientWorkflow.InKitchen.ToString()
                             select new
                             {
                                 d.DateOfOrder,
                                 it.PatientId,
                                 p.DietName,
                                 it.DeliverBedNo,
                                 d.OrderRequestId
                             }).ToList();
            suborders = suborders.Where(l => l.DateOfOrder.Value.Date == DateTime.Now.Date).ToList();
            foreach (var order in suborders)
            {
                var dbModel = myapp.tbl_cmp_OrderRequest.Where(m => m.OrderRequestId == order.OrderRequestId).SingleOrDefault();
                var id = User.Identity.Name;
                dbModel.RequestUserId = (from V in myapp.tbl_User where V.CustomUserId == id select V.UserId).SingleOrDefault();
                dbModel.ModifiedBy = User.Identity.Name;
                dbModel.ModifiedOn = DateTime.Now;
                dbModel.MealTypeId = mealTypeId;
                dbModel.RequestNotes = "OK";
                dbModel.CurrentStatus = "InSupervisor";
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);

        }
        public ActionResult AjaxGetCMpInKitchenOrderRequest(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cmp_OrderRequest where d.CurrentStatus == InPatientWorkflow.InKitchen.ToString() select d).OrderByDescending(l => l.OrderRequestId).ToList();

            if (param.floor != null && param.floor != "")
            {
                query = query.Where(m => m.FloorId == param.floor).ToList();
            }
            if (param.MealType != null && param.MealType != 0)
            {
                query = query.Where(m => m.MealTypeId == param.MealType).ToList();
            }
            if (param.locationname != null && param.locationname != "")
            {
                query = query.Where(m => m.LocationId == param.locationname).ToList();
            }
            IEnumerable<tbl_cmp_OrderRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.OrderRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.MealTypeId != null && c.MealTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join v in myapp.tbl_cm_MealType on c.MealTypeId equals v.MealTypeId
                         join u in myapp.tbl_User on c.RequestUserId equals u.UserId
                         select new[] {
                             c.OrderRequestId.ToString(),
                             c.DateOfOrder.Value.ToString("dd/MM/yyyy"),
                             c.LocationId,
                             c.FloorId,
                            v.MealTypeName,
                              u.FirstName + " "+u.LastName,
                             c.RequestNotes,
                             c.CurrentStatus,
                             c.OrderRequestId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetCMpSupervisorOrderRequest(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cmp_OrderRequest where d.CurrentStatus == InPatientWorkflow.InSupervisor.ToString() select d).OrderByDescending(l => l.OrderRequestId).ToList();
            if (param.locationname != null && param.locationname != "" && param.locationname != "Select")
            {
                query = query.Where(m => m.LocationId == param.locationname).ToList();
            }
            if (param.MealType != null && param.MealType != 0)
            {
                query = query.Where(m => m.MealTypeId == param.MealType).ToList();
            }
            if (param.floor != null && param.floor != "" && param.floor != "Select")
            {
                query = query.Where(m => m.FloorId == param.floor).ToList();
            }

            IEnumerable<tbl_cmp_OrderRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.OrderRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.MealTypeId != null && c.MealTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join m in myapp.tbl_cm_MealType on c.MealTypeId equals m.MealTypeId
                         join u in myapp.tbl_User on c.RequestUserId equals u.UserId
                         select new[] {
                             c.OrderRequestId.ToString(),
                             c.DateOfOrder.Value.ToString("dd/MM/yyyy"),
                             c.LocationId,
                             c.FloorId,
                         m.MealTypeName,
                             u.FirstName + " "+u.LastName,c.RequestNotes,c.CurrentStatus,c.OrderRequestId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AjaxGetCMSDashBoardData(JQueryDataTableParamModel param)
        {
            var date = DateTime.Now.ToString("dd/MM/yyyy");
            var query = (from p in myapp.tbl_cmp_OrderRequestByPatient
                         join o in myapp.tbl_cmp_OrderRequest on p.OrderRequestId equals o.OrderRequestId
                         join pa in myapp.tbl_Patient on p.PatientId equals pa.PatientId
                         join d in myapp.tbl_cmp_Diet on p.DietId equals d.DietId
                         select new DashboardTodayViewModel
                         {
                             MealTypeId = o.MealTypeId,
                             Location = o.LocationId,
                             Floor = o.FloorId,
                             PatientId = p.PatientId,
                             IsDellvered = p.IsDelivered,
                             AssignRoomNo = pa.RoomNo + " " + pa.BedNo,
                             MrNo = pa.MRNo,
                             Name = pa.Name,
                             Mobile = pa.MobileNumber,
                             DietName = d.DietName,
                             RequestType = o.RequestType,
                             CreatedOn = o.DateOfOrder

                         }).ToList();
            query = query.Where(m => m.CreatedOn.Value.ToString("dd/MM/yyyy") == date).ToList();
            if (param.floor != null && param.floor != "")
            {
                query = query.Where(m => m.Floor == param.floor).ToList();
            }
            if (param.MealType != null && param.MealType != 0)
            {
                query = query.Where(m => m.MealTypeId == param.MealType).ToList();
            }
            if (param.locationname != null && param.locationname != "")
            {
                query = query.Where(m => m.Location == param.locationname).ToList();
            }

            IEnumerable<DashboardTodayViewModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PatientId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.DietName != null && c.DietName.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                            c.PatientId.ToString(),
                            c.Name,
                            c.MrNo,
                            c.AssignRoomNo,
                            c.Mobile,
                            c.DietName,
                            c.IsDellvered.ToString(),
                            c.RequestType

                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetOrderCountbyDietPlan()
        {
            var pl = from r in myapp.tbl_cmp_OrderRequestByPatient
                     join d in myapp.tbl_cmp_Diet on r.DietId equals d.DietId

                     orderby d.DietName
                     group d by d.DietName into grp
                     select new { key = grp.Key, cnt = grp.Count() };

            return Json(pl, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetCMSNursing(JQueryDataTableParamModel param)
        {
            var date = DateTime.Now.ToString("dd/MM/yyyy");
            var query = (from p in myapp.tbl_cmp_OrderRequestByPatient
                         join o in myapp.tbl_cmp_OrderRequest on p.OrderRequestId equals o.OrderRequestId
                         join pa in myapp.tbl_Patient on p.PatientId equals pa.PatientId
                         join d in myapp.tbl_cmp_Diet on p.DietId equals d.DietId
                         join u in myapp.tbl_User on p.CreatedBy equals u.CustomUserId
                         select new DashboardTodayViewModel
                         {
                             OrderId = o.OrderRequestId,
                             MealTypeId = o.MealTypeId,
                             Location = o.LocationId,
                             Floor = o.FloorId,
                             PatientId = p.PatientId,
                             IsDellvered = p.IsDelivered,
                             AssignRoomNo = pa.RoomNo,
                             MrNo = pa.MRNo,
                             Name = pa.Name,
                             RoomNo = pa.RoomNo + " " + pa.BedNo,
                             Mobile = pa.MobileNumber,
                             DietName = d.DietName,
                             Notes = p.Notes,
                             RequestType = o.RequestType,
                             CreatedOn = o.DateOfOrder,
                             User = u.FirstName,
                             AttendantMeal = p.AttendantMeal
                         }).ToList();
            //  query = query.Where(m => m.CreatedOn.Value.ToString("dd/MM/yyyy") == date).ToList();
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                DateTime todate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                if (fromdate == todate)
                {
                    todate = todate.AddDays(1);
                }
                query = query.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date <= todate).ToList();
            }
            if (param.FormType != null && param.FormType != "" && param.FormType != "Select" && param.FormType == "Attender")
            {
                query = query.Where(m => m.AttendantMeal == "Yes").ToList();
            }
            if (param.floor != null && param.floor != "" && param.floor != "Select")
            {
                query = query.Where(m => m.Floor == param.floor).ToList();
            }
            if (param.MealType != null && param.MealType != 0)
            {
                query = query.Where(m => m.MealTypeId == param.MealType).ToList();
            }
            if (param.locationname != null && param.locationname != "" && param.locationname != "Select")
            {
                query = query.Where(m => m.Location == param.locationname).ToList();
            }

            IEnumerable<DashboardTodayViewModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PatientId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.DietName != null && c.DietName.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {

                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                             c.OrderId.ToString(),
                             myapp.tbl_MealType.Where(m=>m.MealTypeId==c.MealTypeId).Select(n=>n.Name).FirstOrDefault(),
                            c.MrNo.ToString(),
                            c.Name,
                            c.RoomNo,
                            c.DietName,
                            c.Notes,
                            c.User,
                            c.CreatedOn.Value.ToString("dd/MM/yyyy"),
                              c.CreatedOn.Value.ToString("h:mm tt"),
                            c.RequestType

                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult AjaxGetCMSDietitian(JQueryDataTableParamModel param)
        {
            var date = DateTime.Now.ToString("dd/MM/yyyy");
            var query = (from p in myapp.tbl_cmp_OrderRequestByPatient
                         join o in myapp.tbl_cmp_OrderRequest on p.OrderRequestId equals o.OrderRequestId
                         join pa in myapp.tbl_Patient on p.PatientId equals pa.PatientId
                         join d in myapp.tbl_cmp_Diet on p.DietId equals d.DietId
                         //join u in myapp.tbl_User on o.DietitianEmpId equals u.EmpId
                         select new DashboardTodayViewModel
                         {
                             OrderId = o.OrderRequestId,
                             MealTypeId = o.MealTypeId,
                             Location = o.LocationId,
                             Floor = o.FloorId,
                             PatientId = p.PatientId,
                             IsDellvered = p.IsDelivered,
                             AssignRoomNo = pa.RoomNo,
                             MrNo = pa.MRNo,
                             Name = pa.Name,
                             RoomNo = pa.RoomNo + " " + pa.BedNo,
                             Mobile = pa.MobileNumber,
                             DietName = d.DietName,
                             Notes = p.Notes,
                             RequestType = o.RequestType,
                             CreatedOn = o.DateOfOrder,
                             DietCreatedOn = o.DietitianActionDate,
                             User = (o.DietitianEmpId != null && o.DietitianEmpId > 0) ? myapp.tbl_User.Where(l => l.EmpId == o.DietitianEmpId).FirstOrDefault().FirstName : "",
                             AttendantMeal = p.AttendantMeal
                         }).ToList();
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                DateTime todate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                if (fromdate == todate)
                {
                    todate = todate.AddDays(1);
                }
                query = query.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date <= todate).ToList();
            }
            //query = query.Where(m => m.CreatedOn.Value.ToString("dd/MM/yyyy") == date).ToList();
            if (param.floor != null && param.floor != "" && param.floor != "Select")
            {
                query = query.Where(m => m.Floor == param.floor).ToList();
            }
            if (param.FormType != null && param.FormType != "" && param.FormType != "Select" && param.FormType == "Attender")
            {
                query = query.Where(m => m.AttendantMeal == "Yes").ToList();
            }
            if (param.MealType != null && param.MealType != 0)
            {
                query = query.Where(m => m.MealTypeId == param.MealType).ToList();
            }
            if (param.locationname != null && param.locationname != "" && param.locationname != "Select")
            {
                query = query.Where(m => m.Location == param.locationname).ToList();
            }

            IEnumerable<DashboardTodayViewModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PatientId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.DietName != null && c.DietName.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {

                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join mt in myapp.tbl_MealType on c.MealTypeId equals mt.MealTypeId
                         select new[] {
                             c.OrderId.ToString(),
                           mt.Name,
                            c.MrNo.ToString(),
                            c.Name,
                            c.RoomNo,
                            c.DietName,
                            c.Notes,
                            c.User,
                            c.CreatedOn.Value.ToString("dd/MM/yyyy"),
                              c.CreatedOn.Value.ToString("h:mm tt"),
                            c.RequestType,
                            c.DietCreatedOn!=null? c.DietCreatedOn.Value.ToString("dd/MM/yyyy"):"",
                           c.DietCreatedOn!=null? c.DietCreatedOn.Value.ToString("h:mm tt"):""

                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetCMSAttendendent(JQueryDataTableParamModel param)
        {
            var date = DateTime.Now.ToString("dd/MM/yyyy");
            var query = (from p in myapp.tbl_cmp_OrderRequestAttendendentByPatient
                         join o in myapp.tbl_cmp_OrderRequestAttendendent on p.OrderRequestId equals o.OrderRequestId
                         join pa in myapp.tbl_Patient on p.PatientId equals pa.PatientId
                         join d in myapp.tbl_cmp_Diet on p.DietId equals d.DietId
                         join u in myapp.tbl_User on p.CreatedBy equals u.CustomUserId
                         select new DashboardTodayViewModel
                         {
                             OrderId = o.OrderRequestId,
                             MealTypeId = o.MealTypeId,
                             Location = o.LocationId,
                             Floor = o.FloorId,
                             PatientId = p.PatientId,
                             IsDellvered = p.IsDelivered,
                             AssignRoomNo = pa.RoomNo,
                             MrNo = pa.MRNo,
                             Name = pa.Name,
                             RoomNo = pa.RoomNo + " " + pa.BedNo,
                             Mobile = pa.MobileNumber,
                             DietName = d.DietName,
                             Notes = p.Notes,
                             RequestType = o.RequestType,
                             CreatedOn = o.DateOfOrder,
                             DietCreatedOn = o.DietitianActionDate,
                             User = u.FirstName,
                             AttendantMeal = p.AttendantMeal
                         }).ToList();
            query = query.OrderByDescending(l => l.OrderId).ToList();
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                DateTime todate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                if (fromdate == todate)
                {
                    todate = todate.AddDays(1);
                }
                query = query.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date <= todate).ToList();
            }
            //query = query.Where(m => m.CreatedOn.Value.ToString("dd/MM/yyyy") == date).ToList();
            if (param.floor != null && param.floor != "" && param.floor != "Select")
            {
                query = query.Where(m => m.Floor == param.floor).ToList();
            }
            if (param.FormType != null && param.FormType != "" && param.FormType != "Select" && param.FormType == "Attender")
            {
                query = query.Where(m => m.AttendantMeal == "Yes").ToList();
            }
            if (param.MealType != null && param.MealType != 0)
            {
                query = query.Where(m => m.MealTypeId == param.MealType).ToList();
            }
            if (param.locationname != null && param.locationname != "" && param.locationname != "Select")
            {
                query = query.Where(m => m.Location == param.locationname).ToList();
            }

            IEnumerable<DashboardTodayViewModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PatientId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.DietName != null && c.DietName.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {

                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join mt in myapp.tbl_MealType on c.MealTypeId equals mt.MealTypeId
                         select new[] {
                             c.OrderId.ToString(),
                           mt.Name,
                            c.MrNo.ToString(),
                            c.Name,
                            c.RoomNo,
                            //c.DietName,
                            //c.Notes,
                            c.User,
                            c.CreatedOn.Value.ToString("dd/MM/yyyy"),
                              c.CreatedOn.Value.ToString("h:mm tt"),
                            c.RequestType
                             //c.DietCreatedOn.Value.ToString("dd/MM/yyyy"),
                             // c.DietCreatedOn.Value.ToString("h:mm tt")

                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportExcelNursing(int MealType = 0, string Location = null, string floor = null, string fromDate = null, string toDate = null, string attendermeal = "")
        {
            var Result = (from p in myapp.tbl_cmp_OrderRequestByPatient
                          join o in myapp.tbl_cmp_OrderRequest on p.OrderRequestId equals o.OrderRequestId
                          join pa in myapp.tbl_Patient on p.PatientId equals pa.PatientId
                          join d in myapp.tbl_cmp_Diet on p.DietId equals d.DietId
                          join u in myapp.tbl_User on p.CreatedBy equals u.CustomUserId
                          select new DashboardTodayViewModel
                          {
                              OrderId = o.OrderRequestId,
                              MealTypeId = o.MealTypeId,
                              MrNo = pa.MRNo,
                              Name = pa.Name,
                              RoomNo = pa.RoomNo + " " + pa.BedNo,
                              DietName = d.DietName,
                              Notes = p.Notes != null ? p.Notes.Replace("&", " and ") : "",
                              User = u.FirstName,
                              Floor = o.FloorId,
                              PatientId = p.PatientId,
                              IsDellvered = p.IsDelivered,
                              AssignRoomNo = pa.RoomNo,
                              Mobile = pa.MobileNumber,
                              Location = o.LocationId,

                              RequestType = o.RequestType,
                              CreatedOn = o.CreatedOn,
                              AttendantMeal = p.AttendantMeal
                          }).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                DateTime todate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                if (fromdate == todate)
                {
                    todate = todate.AddDays(1);
                    Result = Result.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date < todate).ToList();

                }
                else
                {
                    Result = Result.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date <= todate).ToList();
                }
            }
            if (attendermeal != null && attendermeal != "" && attendermeal != "Select" && attendermeal == "Attender")
            {
                Result = Result.Where(m => m.AttendantMeal == "Yes").ToList();
            }
            if (floor != null && floor != "")
            {
                Result = Result.Where(m => m.Floor == floor).ToList();
            }
            if (MealType != null && MealType != 0)
            {
                Result = Result.Where(m => m.MealTypeId == MealType).ToList();
            }
            if (Location != null && Location != "")
            {
                Result = Result.Where(m => m.Location == Location).ToList();
            }
            var products = new System.Data.DataTable("Nursing");
            products.Columns.Add("S.No", typeof(string));
            products.Columns.Add("Meal Type", typeof(string));
            products.Columns.Add("Mr No ", typeof(string));
            products.Columns.Add("P. Name", typeof(string));
            products.Columns.Add("Room NO", typeof(string));
            products.Columns.Add("Diet Plan", typeof(string));
            products.Columns.Add("Customization Note", typeof(string));
            products.Columns.Add("User ID", typeof(string));
            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("Time", typeof(string));
            products.Columns.Add("Request Type", typeof(string));
            foreach (var item in Result)
            {
                products.Rows.Add(
               item.OrderId.ToString(),
                  (from var in myapp.tbl_MealType where var.MealTypeId == item.MealTypeId select var.Name).SingleOrDefault(),
item.MrNo, item.Name, item.RoomNo, item.DietName, item.Notes, item.User, item.CreatedOn.Value.ToString("dd/MM/yyyy"),
                              item.CreatedOn.Value.ToString("h:mm tt"), item.RequestType
                );
            }

            GridView grid = new GridView
            {
                DataSource = products
            };
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=Nursing.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelDietitian(int MealType = 0, string Location = null, string floor = null, string fromDate = null, string toDate = null, string attendermeal = "")
        {
            var Result = (from p in myapp.tbl_cmp_OrderRequestByPatient
                          join o in myapp.tbl_cmp_OrderRequest on p.OrderRequestId equals o.OrderRequestId
                          join pa in myapp.tbl_Patient on p.PatientId equals pa.PatientId
                          join d in myapp.tbl_cmp_Diet on p.DietId equals d.DietId
                          join u in myapp.tbl_User on o.DietitianEmpId equals u.EmpId
                          select new DashboardTodayViewModel
                          {
                              OrderId = o.OrderRequestId,
                              MealTypeId = o.MealTypeId,
                              MrNo = pa.MRNo,
                              Name = pa.Name,
                              RoomNo = pa.RoomNo + " " + pa.BedNo,
                              DietName = d.DietName,
                              Notes = p.Notes,
                              User = u.FirstName,
                              Floor = o.FloorId,
                              PatientId = p.PatientId,
                              IsDellvered = p.IsDelivered,
                              AssignRoomNo = pa.RoomNo,
                              Mobile = pa.MobileNumber,
                              Location = o.LocationId,
                              DietCreatedOn = o.DietitianActionDate,
                              RequestType = o.RequestType,
                              CreatedOn = o.CreatedOn,
                              AttendantMeal = p.AttendantMeal
                          }).ToList();

            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                DateTime todate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                if (fromdate == todate)
                {
                    todate = todate.AddDays(1);
                    Result = Result.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date < todate).ToList();
                }
                else
                {
                    Result = Result.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date <= todate).ToList();
                }
            }
            if (attendermeal != null && attendermeal != "" && attendermeal != "Select" && attendermeal == "Attender")
            {
                Result = Result.Where(m => m.AttendantMeal == "Yes").ToList();
            }
            if (floor != null && floor != "")
            {
                Result = Result.Where(m => m.Floor == floor).ToList();
            }
            if (MealType != null && MealType != 0)
            {
                Result = Result.Where(m => m.MealTypeId == MealType).ToList();
            }
            if (Location != null && Location != "")
            {
                Result = Result.Where(m => m.Location == Location).ToList();
            }
            Result = Result.OrderBy(l => l.RoomNo).ToList();
            var products = new System.Data.DataTable("Dietitian");
            products.Columns.Add("S.No", typeof(string));
            products.Columns.Add("Meal Type", typeof(string));
            products.Columns.Add("Mr No ", typeof(string));
            products.Columns.Add("P. Name", typeof(string));
            products.Columns.Add("Room NO", typeof(string));
            products.Columns.Add("Diet Plan", typeof(string));
            products.Columns.Add("Customization Note", typeof(string));
            products.Columns.Add("User ID", typeof(string));
            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("Time", typeof(string));
            products.Columns.Add("Request Type", typeof(string));
            products.Columns.Add("Dietitian Save Date", typeof(string));
            products.Columns.Add("Dietitian Save Time", typeof(string));
            foreach (var item in Result)
            {
                products.Rows.Add(
               item.OrderId.ToString(),
                  (from var in myapp.tbl_MealType where var.MealTypeId == item.MealTypeId select var.Name).SingleOrDefault(),
item.MrNo, item.Name, item.RoomNo, item.DietName, item.Notes, item.User, item.CreatedOn.Value.ToString("dd/MM/yyyy"),
                              item.CreatedOn.Value.ToString("h:mm tt"), item.RequestType,
                              item.DietCreatedOn.Value.ToString("dd/MM/yyyy"),
                              item.DietCreatedOn.Value.ToString("h:mm tt")
                );
            }

            GridView grid = new GridView
            {
                DataSource = products
            };
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=Dietitian.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportExcelAttendendent(int MealType = 0, string Location = null, string floor = null, string fromDate = null, string toDate = null, string attendermeal = "")
        {
            var Result = (from p in myapp.tbl_cmp_OrderRequestAttendendentByPatient
                          join o in myapp.tbl_cmp_OrderRequestAttendendent on p.OrderRequestId equals o.OrderRequestId
                          join pa in myapp.tbl_Patient on p.PatientId equals pa.PatientId
                          join d in myapp.tbl_cmp_Diet on p.DietId equals d.DietId
                          join u in myapp.tbl_User on o.DietitianEmpId equals u.EmpId
                          select new DashboardTodayViewModel
                          {
                              OrderId = o.OrderRequestId,
                              MealTypeId = o.MealTypeId,
                              MrNo = pa.MRNo,
                              Name = pa.Name,
                              RoomNo = pa.RoomNo + " " + pa.BedNo,
                              DietName = d.DietName,
                              Notes = p.Notes != null ? p.Notes.Replace("&", " and ") : "",
                              User = u.FirstName,
                              Floor = o.FloorId,
                              PatientId = p.PatientId,
                              IsDellvered = p.IsDelivered,
                              AssignRoomNo = pa.RoomNo,
                              Mobile = pa.MobileNumber,
                              Location = o.LocationId,
                              DietCreatedOn = o.DietitianActionDate,
                              RequestType = o.RequestType,
                              CreatedOn = o.CreatedOn,
                              AttendantMeal = p.AttendantMeal
                          }).ToList();

            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                DateTime todate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                if (fromdate == todate)
                {
                    todate = todate.AddDays(1);
                    Result = Result.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date < todate).ToList();
                }
                else
                {
                    Result = Result.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date <= todate).ToList();
                }
            }
            if (attendermeal != null && attendermeal != "" && attendermeal != "Select" && attendermeal == "Attender")
            {
                Result = Result.Where(m => m.AttendantMeal == "Yes").ToList();
            }
            if (floor != null && floor != "")
            {
                Result = Result.Where(m => m.Floor == floor).ToList();
            }
            if (MealType != null && MealType != 0)
            {
                Result = Result.Where(m => m.MealTypeId == MealType).ToList();
            }
            if (Location != null && Location != "")
            {
                Result = Result.Where(m => m.Location == Location).ToList();
            }
            Result = Result.OrderBy(l => l.RoomNo).ToList();
            var products = new System.Data.DataTable("Dietitian");
            products.Columns.Add("S.No", typeof(string));
            products.Columns.Add("Meal Type", typeof(string));
            products.Columns.Add("Mr No ", typeof(string));
            products.Columns.Add("P. Name", typeof(string));
            products.Columns.Add("Room NO", typeof(string));
            //products.Columns.Add("Diet Plan", typeof(string));
            //products.Columns.Add("Customization Note", typeof(string));
            products.Columns.Add("User ID", typeof(string));
            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("Time", typeof(string));
            products.Columns.Add("Request Type", typeof(string));
            products.Columns.Add("Dietitian Save Date", typeof(string));
            products.Columns.Add("Dietitian Save Time", typeof(string));
            foreach (var item in Result)
            {
                products.Rows.Add(
               item.OrderId.ToString(),
                  (from var in myapp.tbl_MealType where var.MealTypeId == item.MealTypeId select var.Name).SingleOrDefault(),
item.MrNo, item.Name, item.RoomNo,
//item.DietName, item.Notes,
item.User, item.CreatedOn.Value.ToString("dd/MM/yyyy"),
                              item.CreatedOn.Value.ToString("h:mm tt"), item.RequestType,
                              item.DietCreatedOn.Value.ToString("dd/MM/yyyy"),
                              item.DietCreatedOn.Value.ToString("h:mm tt")
                );
            }

            GridView grid = new GridView
            {
                DataSource = products
            };
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=Dietitian.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public FileStreamResult ExportPDFNursing(int MealType = 0, string Location = null, string floor = null, string fromDate = null, string toDate = null, string attendermeal = "")
        {
            var Result = (from p in myapp.tbl_cmp_OrderRequestByPatient
                          join o in myapp.tbl_cmp_OrderRequest on p.OrderRequestId equals o.OrderRequestId
                          join pa in myapp.tbl_Patient on p.PatientId equals pa.PatientId
                          join d in myapp.tbl_cmp_Diet on p.DietId equals d.DietId
                          join u in myapp.tbl_User on p.CreatedBy equals u.CustomUserId
                          select new DashboardTodayViewModel
                          {
                              OrderId = o.OrderRequestId,
                              MealTypeId = o.MealTypeId,
                              MrNo = pa.MRNo,
                              Name = pa.Name,
                              RoomNo = pa.RoomNo + " " + pa.BedNo,
                              DietName = d.DietName,
                              Notes = p.Notes!=null? p.Notes.Replace("&"," and "):"",
                              User = u.FirstName,
                              Floor = o.FloorId,
                              PatientId = p.PatientId,
                              IsDellvered = p.IsDelivered,
                              AssignRoomNo = pa.RoomNo,
                              Mobile = pa.MobileNumber,
                              Location = o.LocationId,
                              BedNo = pa.BedNo,
                              RequestType = o.RequestType,
                              CreatedOn = o.DateOfOrder,
                              AttendantMeal = p.AttendantMeal,
                          }).ToList();

            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                DateTime todate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                if (fromdate == todate)
                {
                    todate = todate.AddDays(1);
                    Result = Result.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date < todate).ToList();
                }
                else
                {
                    Result = Result.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date <= todate).ToList();
                }
            }
            if (floor != null && floor != "")
            {
                Result = Result.Where(m => m.Floor == floor).ToList();
            }
            if (attendermeal != null && attendermeal != "" && attendermeal != "Select" && attendermeal == "Attender")
            {
                Result = Result.Where(m => m.AttendantMeal == "Yes").ToList();
            }
            if (MealType != null && MealType != 0)
            {
                Result = Result.Where(m => m.MealTypeId == MealType).ToList();
            }
            if (Location != null && Location != "")
            {
                Result = Result.Where(m => m.Location == Location).ToList();
            }
            Result = Result.OrderBy(l => parseintvalue(l.BedNo)).ToList();
            var listfloors = Result.Select(l => l.Floor).Distinct().ToList();
            var mealtypes = myapp.tbl_MealType.ToList();


            int pageNumber = 1;
            //Create document
            MemoryStream workStream = new MemoryStream();
            DateTime dTime = DateTime.Now;
            //file name to be created
            string strPDFFileName = string.Format("SamplePdf" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            //Create PDF Table with 5 columns
            //PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            PdfWriter writer = PdfWriter.GetInstance(doc, workStream);
            //HTMLWorker htmlWorker = new HTMLWorker(doc);
            //StringBuilder Sb = new StringBuilder();
            int page = 0;
            doc.Open();
            StringBuilder Sb = new StringBuilder();
            Sb.Append("<html><head><style>body {font-family: sans-serif !important;font-size:15px;}.table td{ padding:8px;} .table {    width: 100%;  margin-bottom: 1rem;  color: #212529;  vertical-align: top;  border-color: #dee2e6; font-size:12px; } .table > :not(caption) > * > * {  padding: 0.5rem 0.5rem;  background-color: var(--bs-table-bg);  border-bottom-width: 1px;  box-shadow: inset 0 0 0 9999px var(--bs-table-accent-bg);}.table-bordered > :not(caption) > * {  border-width: 1px 0;}.table-bordered > :not(caption) > * > * {  border-width: 0 1px;}</style></head><body></body></html><p style='text-align:center;font-size:16px'>Tanishsoft Hrms</p>");
            Sb.Append("<p style='text-align:center;'>4-1-1230, Bogulkunta, Near Abids, Hyderabad, Telangana 500001,Phone: 040 4022 2300 Ph: 40222300</p>");
            Sb.Append("<p style='text-align:center;'>From Date : " + fromDate + "  Meal type : " + (from var in mealtypes where var.MealTypeId == MealType select var.Name).SingleOrDefault() + "</p>");
            if (attendermeal != null && attendermeal != "" && attendermeal != "Select" && attendermeal == "Attender")
            {
                Sb.Append("<p style='text-align:center;'>Attendant meal for the Location : " + Location + "</p> <hr />");
                Sb.Append("<table style='width:100%;  border-collapse: collapse;' class='table'><thead><tr><td  style='border: solid 1px #ddd;width:5%;'>S.No</td><td  style='border: solid 1px #ddd;width:13%;'>Mr No</td><td  style='border: solid 1px #ddd;width:13%;'>Bed No</td><td  style='border: solid 1px #ddd;width:31%;'>Name</td><td  style='border: solid 1px #ddd;width:38%;'>Notes</td></tr></thead><tbody>");
                int serialno = 1;
                foreach (var locf in listfloors)
                {
                    //StringBuilder Sb2 = new StringBuilder();
                    var subresult = Result.Where(l => l.Floor == locf).ToList();
                    Sb.Append("<tr><td colspan='5'  style='border: solid 1px #ddd'>" + locf + "</td></tr>");
                    foreach (var item in subresult)
                    {
                        Sb.Append("<tr><td style='border: solid 1px #ddd'>" + serialno + "</td><td  style='border: solid 1px #ddd'>" + item.MrNo + "</td><td  style='border: solid 1px #ddd'>" + item.BedNo + "</td><td  style='border: solid 1px #ddd'>" + item.Name + "</td><td  style='border: solid 1px #ddd'>&nbsp;</td></tr>");
                        serialno++;
                    }
                }
            }
            else
            {
                Sb.Append("<p style='text-align:center;'>Location : " + Location + "</p> <hr />");
                Sb.Append("<table style='width:100%;  border-collapse: collapse;' class='table'><thead><tr><td  style='border: solid 1px #ddd;width:5%;'>S.No</td><td  style='border: solid 1px #ddd;width:10%;'>Mr No</td><td  style='border: solid 1px #ddd;width:10%;'>Bed No</td><td  style='border: solid 1px #ddd;width:27%;'>Name</td><td  style='border: solid 1px #ddd;width:10%;'>Diet Type</td><td  style='border: solid 1px #ddd;width:38%;'>Notes</td></tr></thead><tbody>");
                int serialno = 1;
                foreach (var locf in listfloors)
                {
                    //StringBuilder Sb2 = new StringBuilder();
                    var subresult = Result.Where(l => l.Floor == locf).ToList();
                    Sb.Append("<tr><td colspan='6'  style='border: solid 1px #ddd'>" + locf + "</td></tr>");
                    foreach (var item in subresult)
                    {
                        Sb.Append("<tr><td style='border: solid 1px #ddd'>" + serialno + "</td><td  style='border: solid 1px #ddd'>" + item.MrNo + "</td><td  style='border: solid 1px #ddd'>" + item.BedNo + "</td><td  style='border: solid 1px #ddd'>" + item.Name + "</td><td  style='border: solid 1px #ddd'>" + item.DietName + "</td><td  style='border: solid 1px #ddd'>" + item.Notes + "</td></tr>");
                        serialno++;
                    }
                }
            }
            Sb.Append("</tbody></table>");
            StringReader sr = new StringReader(Sb.ToString());
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);
            doc.Close();
            //PdfReader pdfReader = new PdfReader(workStream.ToArray());
            //PdfStamper _scratchDoc = new PdfStamper(pdfReader, workStream);
            byte[] pdf = workStream.ToArray();
            HttpResponseMessage result = new HttpResponseMessage
            {
                Content = new ByteArrayContent(pdf)
            };
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
            {
                FileName = Location + "_Menuitem.pdf"
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            //log.Info("End - ExportToPdf - " + User.Identity.Name + " " + DateTime.Now);
            //return result;
            Response.AddHeader("Content-Disposition", "inline; filename=" + Location + "_Menuitem.pdf");
            MemoryStream outputStream = new MemoryStream();
            outputStream.Write(pdf, 0, pdf.Length);
            outputStream.Position = 0;
            return File(outputStream, "application/pdf");
        }
        public int parseintvalue(string a)
        {

            string b = string.Empty;
            int val2 = 0;

            for (int i = 0; i < a.Length; i++)
            {
                if (Char.IsDigit(a[i]))
                    b += a[i];
            }

            if (b.Length > 0)
                val2 = int.Parse(b);
            return val2;
        }
        public FileStreamResult ExportPDFDietitian(int MealType = 0, string Location = null, string floor = null, string fromDate = null, string toDate = null, string attendermeal = "")
        {
            var Result = (from p in myapp.tbl_cmp_OrderRequestByPatient
                          join o in myapp.tbl_cmp_OrderRequest on p.OrderRequestId equals o.OrderRequestId
                          join pa in myapp.tbl_Patient on p.PatientId equals pa.PatientId
                          join d in myapp.tbl_cmp_Diet on p.DietId equals d.DietId
                          join u in myapp.tbl_User on o.DietitianEmpId equals u.EmpId
                          select new DashboardTodayViewModel
                          {
                              OrderId = o.OrderRequestId,
                              MealTypeId = o.MealTypeId,
                              MrNo = pa.MRNo,
                              Name = pa.Name,
                              RoomNo = pa.RoomNo + " " + pa.BedNo,
                              DietName = d.DietName,
                              Notes = p.Notes != null ? p.Notes.Replace("&", " and ") : "",
                              User = u.FirstName,
                              Floor = o.FloorId,
                              PatientId = p.PatientId,
                              IsDellvered = p.IsDelivered,
                              AssignRoomNo = pa.RoomNo,
                              Mobile = pa.MobileNumber,
                              Location = o.LocationId,
                              DietCreatedOn = o.DietitianActionDate,
                              RequestType = o.RequestType,
                              CreatedOn = o.DateOfOrder,
                              BedNo = pa.BedNo,
                              AttendantMeal = p.AttendantMeal
                          }).ToList();

            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                DateTime todate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                if (fromdate == todate)
                {
                    todate = todate.AddDays(1);
                    Result = Result.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date < todate).ToList();
                }
                else
                {
                    Result = Result.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date <= todate).ToList();
                }
            }
            if (floor != null && floor != "")
            {
                Result = Result.Where(m => m.Floor == floor).ToList();
            }
            if (attendermeal != null && attendermeal != "" && attendermeal != "Select" && attendermeal == "Attender")
            {
                Result = Result.Where(m => m.AttendantMeal == "Yes").ToList();
            }
            if (MealType != null && MealType != 0)
            {
                Result = Result.Where(m => m.MealTypeId == MealType).ToList();
            }
            if (Location != null && Location != "")
            {
                Result = Result.Where(m => m.Location == Location).ToList();
            }
            Result = Result.OrderBy(l => parseintvalue(l.BedNo)).ToList();
            var listfloors = Result.Select(l => l.Floor).Distinct().ToList();
            var mealtypes = myapp.tbl_MealType.ToList();

            int pageNumber = 1;
            //Create document
            MemoryStream workStream = new MemoryStream();
            DateTime dTime = DateTime.Now;
            //file name to be created
            string strPDFFileName = string.Format("SamplePdf" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            //Create PDF Table with 5 columns
            //PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            PdfWriter writer = PdfWriter.GetInstance(doc, workStream);
            //HTMLWorker htmlWorker = new HTMLWorker(doc);
            //StringBuilder Sb = new StringBuilder();
            int page = 0;
            doc.Open();
            StringBuilder Sb = new StringBuilder();
            Sb.Append("<html><head><style>body {font-family: sans-serif !important;font-size:15px;}.table td{ padding:8px;} .table {    width: 100%;  margin-bottom: 1rem;  color: #212529;  vertical-align: top;  border-color: #dee2e6; font-size:12px; } .table > :not(caption) > * > * {  padding: 0.5rem 0.5rem;  background-color: var(--bs-table-bg);  border-bottom-width: 1px;  box-shadow: inset 0 0 0 9999px var(--bs-table-accent-bg);}.table-bordered > :not(caption) > * {  border-width: 1px 0;}.table-bordered > :not(caption) > * > * {  border-width: 0 1px;}</style></head><body></body></html><p style='text-align:center;font-size:16px'>Tanishsoft Hrms</p>");
            Sb.Append("<p style='text-align:center;'>4-1-1230, Bogulkunta, Near Abids, Hyderabad, Telangana 500001,Phone: 040 4022 2300 Ph: 40222300</p>");
            Sb.Append("<p style='text-align:center;'>From Date : " + fromDate + "  Meal type : " + (from var in mealtypes where var.MealTypeId == MealType select var.Name).SingleOrDefault() + "</p>");

            if (attendermeal != null && attendermeal != "" && attendermeal != "Select" && attendermeal == "Attender")
            {
                Sb.Append("<p style='text-align:center;'>Attendant meal for the Location : " + Location + "</p> <hr />");
                Sb.Append("<table style='width:100%;  border-collapse: collapse;' class='table'><thead><tr><td  style='border: solid 1px #ddd;width:5%;'>S.No</td><td  style='border: solid 1px #ddd;width:13%;'>Mr No</td><td  style='border: solid 1px #ddd;width:13%;'>Bed No</td><td  style='border: solid 1px #ddd;width:31%;'>Name</td><td  style='border: solid 1px #ddd;width:38%;'>Notes</td></tr></thead><tbody>");
                int serialno = 1;
                foreach (var locf in listfloors)
                {
                    //StringBuilder Sb2 = new StringBuilder();
                    var subresult = Result.Where(l => l.Floor == locf).ToList();
                    Sb.Append("<tr><td colspan='5'  style='border: solid 1px #ddd'> " + locf + "</td></tr>");
                    foreach (var item in subresult)
                    {
                        Sb.Append("<tr><td style='border: solid 1px #ddd'>" + serialno + "</td><td  style='border: solid 1px #ddd'>" + item.MrNo + "</td><td  style='border: solid 1px #ddd'>" + item.BedNo + "</td><td  style='border: solid 1px #ddd'>" + item.Name + "</td><td  style='border: solid 1px #ddd'>&nbsp;</td></tr>");
                        serialno++;
                    }
                }
            }
            else
            {
                Sb.Append("<p style='text-align:center;'>Location : " + Location + "</p> <hr />");
                Sb.Append("<table style='width:100%;  border-collapse: collapse;' class='table'><thead><tr><td  style='border: solid 1px #ddd;width:5%;'>S.No</td><td  style='border: solid 1px #ddd;width:10%;'>Mr No</td><td  style='border: solid 1px #ddd;width:10%;'>Bed No</td><td  style='border: solid 1px #ddd;width:27%;'>Name</td><td  style='border: solid 1px #ddd;width:10%;'>Diet Type</td><td  style='border: solid 1px #ddd;width:38%;'>Notes</td></tr></thead><tbody>");
                int serialno = 1;
                foreach (var locf in listfloors)
                {
                    //StringBuilder Sb2 = new StringBuilder();
                    var subresult = Result.Where(l => l.Floor == locf).ToList();
                    Sb.Append("<tr><td colspan='6'  style='border: solid 1px #ddd'>" + locf + "</td></tr>");
                    foreach (var item in subresult)
                    {
                        Sb.Append("<tr><td style='border: solid 1px #ddd'>" + serialno + "</td><td  style='border: solid 1px #ddd'>" + item.MrNo + "</td><td  style='border: solid 1px #ddd'>" + item.BedNo + "</td><td  style='border: solid 1px #ddd'>" + item.Name + "</td><td  style='border: solid 1px #ddd'>" + item.DietName + "</td><td  style='border: solid 1px #ddd'>" + item.Notes + "</td></tr>");
                        serialno++;
                    }
                }
            }
            Sb.Append("</tbody></table>");
            StringReader sr = new StringReader(Sb.ToString());
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);
            doc.Close();
            //PdfReader pdfReader = new PdfReader(workStream.ToArray());
            //PdfStamper _scratchDoc = new PdfStamper(pdfReader, workStream);
            byte[] pdf = workStream.ToArray();
            //HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
            HttpResponseMessage result = new HttpResponseMessage
            {
                Content = new ByteArrayContent(pdf)
            };
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
            {
                FileName = " Menuitem.pdf"
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            //log.Info("End - ExportToPdf - " + User.Identity.Name + " " + DateTime.Now);
            //return result;
            Response.AddHeader("Content-Disposition", "inline; filename=Menuitem.pdf");
            MemoryStream outputStream = new MemoryStream();
            outputStream.Write(pdf, 0, pdf.Length);
            outputStream.Position = 0;
            return File(outputStream, "application/pdf");
        }

        public FileStreamResult ExportPDFAttendendent(int MealType = 0, string Location = null, string floor = null, string fromDate = null, string toDate = null, string attendermeal = "")
        {
            var Result = (from p in myapp.tbl_cmp_OrderRequestAttendendentByPatient
                          join o in myapp.tbl_cmp_OrderRequestAttendendent on p.OrderRequestId equals o.OrderRequestId
                          join pa in myapp.tbl_Patient on p.PatientId equals pa.PatientId
                          join d in myapp.tbl_cmp_Diet on p.DietId equals d.DietId
                          join u in myapp.tbl_User on o.DietitianEmpId equals u.EmpId
                          select new DashboardTodayViewModel
                          {
                              OrderId = o.OrderRequestId,
                              MealTypeId = o.MealTypeId,
                              MrNo = pa.MRNo,
                              Name = pa.Name,
                              RoomNo = pa.RoomNo + " " + pa.BedNo,
                              DietName = d.DietName,
                              Notes = p.Notes != null ? p.Notes.Replace("&", " and ") : "",
                              User = u.FirstName,
                              Floor = o.FloorId,
                              PatientId = p.PatientId,
                              IsDellvered = p.IsDelivered,
                              AssignRoomNo = pa.RoomNo,
                              Mobile = pa.MobileNumber,
                              Location = o.LocationId,
                              DietCreatedOn = o.DietitianActionDate,
                              RequestType = o.RequestType,
                              CreatedOn = o.DateOfOrder,
                              BedNo = pa.BedNo,
                              AttendantMeal = p.AttendantMeal
                          }).ToList();

            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                DateTime todate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                if (fromdate == todate)
                {
                    todate = todate.AddDays(1);
                    Result = Result.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date < todate).ToList();
                }
                else
                {
                    Result = Result.Where(l => l.CreatedOn.Value.Date >= fromdate && l.CreatedOn.Value.Date <= todate).ToList();
                }
            }
            if (floor != null && floor != "")
            {
                Result = Result.Where(m => m.Floor == floor).ToList();
            }
            if (attendermeal != null && attendermeal != "" && attendermeal != "Select" && attendermeal == "Attender")
            {
                Result = Result.Where(m => m.AttendantMeal == "Yes").ToList();
            }
            if (MealType != null && MealType != 0)
            {
                Result = Result.Where(m => m.MealTypeId == MealType).ToList();
            }
            if (Location != null && Location != "")
            {
                Result = Result.Where(m => m.Location == Location).ToList();
            }
            Result = Result.OrderBy(l => parseintvalue(l.BedNo)).ToList();
            var listfloors = Result.Select(l => l.Floor).Distinct().ToList();
            var mealtypes = myapp.tbl_MealType.ToList();

            int pageNumber = 1;
            //Create document
            MemoryStream workStream = new MemoryStream();
            DateTime dTime = DateTime.Now;
            //file name to be created
            string strPDFFileName = string.Format("SamplePdf" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
            //Create PDF Table with 5 columns
            //PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            PdfWriter writer = PdfWriter.GetInstance(doc, workStream);
            //HTMLWorker htmlWorker = new HTMLWorker(doc);
            //StringBuilder Sb = new StringBuilder();
            int page = 0;
            doc.Open();
            StringBuilder Sb = new StringBuilder();
            Sb.Append("<html><head><style>body {font-family: sans-serif !important;font-size:15px;}.table td{ padding:8px;} .table {    width: 100%;  margin-bottom: 1rem;  color: #212529;  vertical-align: top;  border-color: #dee2e6; font-size:12px; } .table > :not(caption) > * > * {  padding: 0.5rem 0.5rem;  background-color: var(--bs-table-bg);  border-bottom-width: 1px;  box-shadow: inset 0 0 0 9999px var(--bs-table-accent-bg);}.table-bordered > :not(caption) > * {  border-width: 1px 0;}.table-bordered > :not(caption) > * > * {  border-width: 0 1px;}</style></head><body></body></html><p style='text-align:center;font-size:16px'>Tanishsoft Hrms</p>");
            Sb.Append("<p style='text-align:center;'>4-1-1230, Bogulkunta, Near Abids, Hyderabad, Telangana 500001,Phone: 040 4022 2300 Ph: 40222300</p>");
            Sb.Append("<p style='text-align:center;'>From Date : " + fromDate + "  Meal type : " + (from var in mealtypes where var.MealTypeId == MealType select var.Name).SingleOrDefault() + "</p>");


            Sb.Append("<p style='text-align:center;'>Attendant meal for the Location : " + Location + "</p> <hr />");
            Sb.Append("<table style='width:100%;  border-collapse: collapse;' class='table'><thead><tr><td  style='border: solid 1px #ddd;width:5%;'>S.No</td><td  style='border: solid 1px #ddd;width:13%;'>Mr No</td><td  style='border: solid 1px #ddd;width:13%;'>Bed No</td><td  style='border: solid 1px #ddd;width:31%;'>Name</td><td  style='border: solid 1px #ddd;width:38%;'>Notes</td></tr></thead><tbody>");
            int serialno = 1;
            foreach (var locf in listfloors)
            {
                //StringBuilder Sb2 = new StringBuilder();
                var subresult = Result.Where(l => l.Floor == locf).ToList();
                Sb.Append("<tr><td colspan='5'  style='border: solid 1px #ddd'> " + locf + "</td></tr>");
                foreach (var item in subresult)
                {
                    Sb.Append("<tr><td style='border: solid 1px #ddd'>" + serialno + "</td><td  style='border: solid 1px #ddd'>" + item.MrNo + "</td><td  style='border: solid 1px #ddd'>" + item.BedNo + "</td><td  style='border: solid 1px #ddd'>" + item.Name + "</td><td  style='border: solid 1px #ddd'>&nbsp;</td></tr>");
                    serialno++;
                }
            }

            Sb.Append("</tbody></table>");
            StringReader sr = new StringReader(Sb.ToString());
            XMLWorkerHelper.GetInstance().ParseXHtml(writer, doc, sr);
            doc.Close();
            //PdfReader pdfReader = new PdfReader(workStream.ToArray());
            //PdfStamper _scratchDoc = new PdfStamper(pdfReader, workStream);
            byte[] pdf = workStream.ToArray();
            //HttpResponseMessage result = Request.CreateResponse(HttpStatusCode.OK);
            HttpResponseMessage result = new HttpResponseMessage
            {
                Content = new ByteArrayContent(pdf)
            };
            result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
            {
                FileName = " Menuitem.pdf"
            };
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            //log.Info("End - ExportToPdf - " + User.Identity.Name + " " + DateTime.Now);
            //return result;
            Response.AddHeader("Content-Disposition", "inline; filename=Menuitem.pdf");
            MemoryStream outputStream = new MemoryStream();
            outputStream.Write(pdf, 0, pdf.Length);
            outputStream.Position = 0;
            return File(outputStream, "application/pdf");
        }

        public ActionResult SendOrdersToSupervisorExportExcel(int mealTypeId)
        {
            DateTime dt = DateTime.Now.Date;
            var suborders = (from d in myapp.tbl_cmp_OrderRequest
                             join it in myapp.tbl_cmp_OrderRequestByPatient on d.OrderRequestId equals it.OrderRequestId
                             join p in myapp.tbl_cmp_Diet on it.DietId equals p.DietId
                             where d.CurrentStatus == "InKitchen"
                             select new
                             {
                                 d.LocationId,
                                 d.DateOfOrder,
                                 it.PatientId,
                                 p.DietName,
                                 it.DeliverBedNo,
                                 it.Notes,
                                 d.MealTypeId
                             }).ToList();
            if (mealTypeId != 0)
            {
                suborders = suborders.Where(l => l.MealTypeId == mealTypeId).ToList();
            }
            suborders = suborders.Where(l => l.DateOfOrder.Value.Date == dt).ToList();
            //suborders = suborders.Where(l => l.DateOfOrder.Value.Date == DateTime.Now.Date).ToList();
            var locations = suborders.Select(l => l.LocationId).Distinct().ToList();
            List<KitchenOrdersViewModel> result = new List<KitchenOrdersViewModel>();
            foreach (var l in locations)
            {
                var dietnames = suborders.Where(s => s.LocationId == l).Select(d => d.DietName).Distinct().ToList();
                foreach (var d in dietnames)
                {
                    var dietlist = suborders.Where(s => s.LocationId == l && s.DietName == d).ToList();
                    KitchenOrdersViewModel m = new KitchenOrdersViewModel();
                    m.DietName = d;
                    m.Location = l;
                    m.Count = dietlist.Count();
                    m.Customizations = string.Join(",", dietlist.Where(c => c.Notes != null && c.Notes != "").Select(c => c.Notes));
                    result.Add(m);
                }
            }
            var products = new System.Data.DataTable("SendOrdersToSupervisor");
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Diet variety", typeof(string));
            products.Columns.Add("Count", typeof(string));
            products.Columns.Add("Customizations", typeof(string));

            foreach (var item in result)
            {
                products.Rows.Add(
             item.Location,
             item.DietName,
             item.Count,
             item.Customizations
                );
            }

            GridView grid = new GridView
            {
                DataSource = products
            };
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=SendOrdersToSupervisor.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}