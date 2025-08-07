using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;
using WebApplicationHsApp.OracleInterface;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class CMSController : Controller
    {
        // GET: CMS
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EmpFoodDashboard()
        {
            return View();
        }
        public ActionResult HrAdminEmpFood()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            var id = User.Identity.Name;
            CMSEmployeeDashBoard employeeDashBoard = new CMSEmployeeDashBoard();
            employeeDashBoard.myOrders = (from it in myapp.tbl_cm_Transaction
                                          join t in myapp.tbl_cm_TransactionItem on it.TransactionId equals t.TransactionId
                                          join i in myapp.tbl_cm_Item on t.ItemId equals i.ItemId
                                          where it.CreatedBy == id
                                          select new OrdersDashBoardViewModel
                                          {
                                              Id = t.Id,
                                              Item = i.ItemName,
                                              Date = it.CreatedOn.Value,
                                              Qty = t.Qty.Value,
                                              Comments = it.SalesEmpNotes
                                          }).Take(5).ToList();
            employeeDashBoard.myPayments = (from it in myapp.tbl_cm_Transaction
                                            where it.CreatedBy == id
                                            select new PaymentsDashBoardViewModel
                                            {
                                                Id = it.TransactionId,
                                                Type = it.ModeOfPayment,
                                                Date = it.CreatedOn.Value,
                                                Amount = it.FinalPrice,
                                                Comments = it.SalesEmpNotes
                                            }).Take(5).ToList();
            employeeDashBoard.TotalAmount = myapp.tbl_cm_Transaction.Where(m => m.CreatedBy == id).Take(5).Sum(m => m.FinalPrice);
            return PartialView(employeeDashBoard);
        }
        public ActionResult CanteenDashboard()
        {
            var id = User.Identity.Name;
            CMSCanteenDashBoard employeeDashBoard = new CMSCanteenDashBoard();
            employeeDashBoard.myOrders = (from it in myapp.tbl_cm_Transaction
                                          join t in myapp.tbl_cm_TransactionItem on it.TransactionId equals t.TransactionId
                                          join i in myapp.tbl_cm_Item on t.ItemId equals i.ItemId
                                          where it.CreatedBy == id
                                          select new OrdersDashBoardViewModel
                                          {
                                              Id = t.Id,
                                              Item = i.ItemName,
                                              Date = it.CreatedOn.Value,
                                              Qty = t.Qty.Value,
                                              Comments = it.SalesEmpNotes
                                          }).Take(10).ToList();
            employeeDashBoard.myPayments = (from it in myapp.tbl_cm_Transaction
                                            where it.CreatedBy == id
                                            select new PaymentsDashBoardViewModel
                                            {
                                                Id = it.TransactionId,
                                                Type = it.ModeOfPayment,
                                                Date = it.CreatedOn.Value,
                                                Amount = it.FinalPrice,
                                                Comments = it.SalesEmpNotes
                                            }).Take(10).ToList();
            employeeDashBoard.TotalAmount = myapp.tbl_cm_Transaction.Where(m => m.CreatedBy == id).Take(5).Sum(m => m.FinalPrice);
            employeeDashBoard.TotalPayment = myapp.tbl_cm_Transaction.Where(m => m.CreatedBy == id).Sum(m => m.FinalPrice);
            employeeDashBoard.NonCooked = (from it in myapp.tbl_cm_Transaction
                                           join t in myapp.tbl_cm_TransactionItem on it.TransactionId equals t.TransactionId
                                           join i in myapp.tbl_cm_Item on t.ItemId equals i.ItemId
                                           where i.ItemType == "Recipe"
                                           select it).Count();
            employeeDashBoard.Cooked = (from it in myapp.tbl_cm_Transaction
                                        join t in myapp.tbl_cm_TransactionItem on it.TransactionId equals t.TransactionId
                                        join i in myapp.tbl_cm_Item on t.ItemId equals i.ItemId
                                        where i.ItemType != "Recipe"
                                        select it).Count();
            MealTpyeDashBoardViewModel breakfast = new MealTpyeDashBoardViewModel();
            MealTpyeDashBoardViewModel lunch = new MealTpyeDashBoardViewModel();
            MealTpyeDashBoardViewModel dinner = new MealTpyeDashBoardViewModel();
            employeeDashBoard.mealType.Add(breakfast);
            employeeDashBoard.mealType.Add(lunch);
            employeeDashBoard.mealType.Add(dinner);

            return PartialView(employeeDashBoard);
        }
        public ActionResult AdminDashboard()
        {
            var id = User.Identity.Name;
            CMSAdminDashBoard employeeDashBoard = new CMSAdminDashBoard();
            employeeDashBoard.myOrders = (from it in myapp.tbl_cm_Transaction
                                          join t in myapp.tbl_cm_TransactionItem on it.TransactionId equals t.TransactionId
                                          join i in myapp.tbl_cm_Item on t.ItemId equals i.ItemId
                                          select new OrdersDashBoardViewModel
                                          {
                                              Id = t.Id,
                                              Item = i.ItemName,
                                              Date = it.CreatedOn.Value,
                                              Qty = t.Qty.Value,
                                              Comments = it.SalesEmpNotes
                                          }).Take(10).ToList();
            employeeDashBoard.myPayments = (from it in myapp.tbl_cm_Transaction
                                            select new PaymentsDashBoardViewModel
                                            {
                                                Id = it.TransactionId,
                                                Type = it.ModeOfPayment,
                                                Date = it.CreatedOn.Value,
                                                Amount = it.FinalPrice,
                                                Comments = it.SalesEmpNotes
                                            }).Take(10).ToList();
            employeeDashBoard.TotalAmount = myapp.tbl_cm_Transaction.Take(10).Sum(m => m.FinalPrice);
            employeeDashBoard.TotalPayment = myapp.tbl_cm_Transaction.Where(m => m.CreatedBy == id).Sum(m => m.FinalPrice);
            employeeDashBoard.NonCooked = (from it in myapp.tbl_cm_Transaction
                                           join t in myapp.tbl_cm_TransactionItem on it.TransactionId equals t.TransactionId
                                           join i in myapp.tbl_cm_Item on t.ItemId equals i.ItemId
                                           where i.ItemType == "Recipe"
                                           select it).Count();
            employeeDashBoard.Cooked = (from it in myapp.tbl_cm_Transaction
                                        join t in myapp.tbl_cm_TransactionItem on it.TransactionId equals t.TransactionId
                                        join i in myapp.tbl_cm_Item on t.ItemId equals i.ItemId
                                        where i.ItemType != "Recipe"
                                        select it).Count();

            MealTpyeDashBoardViewModel breakfast = new MealTpyeDashBoardViewModel();
            MealTpyeDashBoardViewModel lunch = new MealTpyeDashBoardViewModel();
            MealTpyeDashBoardViewModel dinner = new MealTpyeDashBoardViewModel();
            employeeDashBoard.mealType.Add(breakfast);
            employeeDashBoard.mealType.Add(lunch);
            employeeDashBoard.mealType.Add(dinner);
            return PartialView(employeeDashBoard);
        }
        public ActionResult ManageCMCateen()
        {
            return PartialView();
        }
        public ActionResult CaptureSalesByCanteen()
        {
            var user = User.Identity.Name;
            int userId = ((from V in myapp.tbl_User where V.CustomUserId == user select V.UserId).SingleOrDefault());
            var empStore = myapp.tbl_cm_StoreEmp.Where(m => m.EmpId == userId).SingleOrDefault();
            ViewBag.CanteenId = empStore != null ? empStore.StoreId : 0;
            return View();
        }
        public ActionResult GeneratePin()
        {
            var id = User.Identity.Name;
            ViewBag.id = (from V in myapp.tbl_User where V.CustomUserId == id select V.UserId).SingleOrDefault();
            ViewBag.Pin = (from V in myapp.tbl_User where V.CustomUserId == id select V.SecurityAnswner).SingleOrDefault();
            return PartialView();
        }
        public ActionResult ManageCMCategory()
        {
            return PartialView();
        }
        public ActionResult ManageCMMealType()
        {
            return PartialView();
        }
        public ActionResult ManageCMUnitType()
        {
            return PartialView();
        }
        public ActionResult ManageCMStore()
        {
            return PartialView();
        }
        public ActionResult ManageCMItem()
        {
            return PartialView();
        }
        public ActionResult CreateCMItem(int id = 0)
        {
            ViewBag.id = id;

            return PartialView();
        }
        public ActionResult ManageCMStock()
        {
            return PartialView();
        }
        public ActionResult bill(int id)
        {
            var data = GetPrintDetails(id);
            return View(data);
        }
        public ActionResult CreateCMStock(int id = 0)
        {
            ViewBag.id = id;
            return PartialView();
        }
        public ActionResult CancelCaptureSalesByCanteen(int id = 0)
        {
            //var user = User.Identity.Name;
            //int userId = ((from V in myapp.tbl_User where V.CustomUserId == user select V.UserId).SingleOrDefault());
            //var empStore = myapp.tbl_cm_StoreEmp.Where(m => m.EmpId == userId).SingleOrDefault();
            ViewBag.CanteenId = 0;
            ViewBag.id = id;
            return PartialView();
        }
        public ActionResult ManageCMSItemTransfer()
        {
            var loginId = User.Identity.Name;
            ViewBag.Userid = (from V in myapp.tbl_User where V.CustomUserId == loginId select V.UserId).SingleOrDefault();
            ViewBag.UserName = (from V in myapp.tbl_User where V.CustomUserId == loginId select V.FirstName + " " + V.LastName + " " + V.Designation).SingleOrDefault();

            return PartialView();
        }
        public ActionResult CreateCMItemTransfer(int id = 0)
        {
            var loginId = User.Identity.Name;
            ViewBag.Userid = (from V in myapp.tbl_User where V.CustomUserId == loginId select V.UserId).SingleOrDefault();
            ViewBag.UserName = (from V in myapp.tbl_User where V.CustomUserId == loginId select V.FirstName + " " + V.LastName + " " + V.Designation).SingleOrDefault();

            ViewBag.id = id;
            return PartialView();
        }

        public ActionResult ManageDistributeItems()
        {
            var loginId = User.Identity.Name;
            ViewBag.Userid = (from V in myapp.tbl_User where V.CustomUserId == loginId select V.UserId).SingleOrDefault();
            ViewBag.UserName = (from V in myapp.tbl_User where V.CustomUserId == loginId select V.FirstName + " " + V.LastName + " " + V.Designation).SingleOrDefault();

            return PartialView();
        }
        public ActionResult CreateDistributeItems(int id = 0)
        {
            var loginId = User.Identity.Name;
            ViewBag.Userid = (from V in myapp.tbl_User where V.CustomUserId == loginId select V.UserId).SingleOrDefault();
            ViewBag.UserName = (from V in myapp.tbl_User where V.CustomUserId == loginId select V.FirstName + " " + V.LastName + " " + V.Designation).SingleOrDefault();

            ViewBag.id = id;
            return PartialView();
        }
        public ActionResult ManageCMSBill()
        {
            return PartialView();
        }

        public ActionResult CreateCMSBill(int id = 0)
        {
            ViewBag.id = id;
            return PartialView();
        }
        public ActionResult ManagePlaceRequest()
        {
            return PartialView();
        }

        public ActionResult CreatePlaceRequest(int id = 0)
        {
            ViewBag.id = id;
            return PartialView();
        }
        public ActionResult ManageReceived()
        {
            return PartialView();
        }
        public ActionResult CreateReceive(int id = 0)
        {
            ViewBag.id = id;
            return PartialView();
        }
        public ActionResult CreateSales(int id = 0)
        {
            ViewBag.id = id;
            return PartialView();
        }
        public ActionResult ManageSales()
        {
            return PartialView();
        }
        public ActionResult ManageCanteenEmployees()
        {
            return PartialView();
        }
        public ActionResult ManageMySales()
        {
            return PartialView();
        }
        public ActionResult ManagePayments()
        {
            return PartialView();
        }
        public ActionResult ManageCredit()
        {
            return PartialView();
        }
        public ActionResult ManageMyCredit()
        {
            return PartialView();
        }
        public ActionResult ItemsSalesReport()
        {
            return PartialView();
        }
        public ActionResult ManageMyPayments()
        {
            return PartialView();
        }
        public ActionResult CreateFoodManage(int id = 0)
        {
            ViewBag.id = id;
            return PartialView();
        }
        public ActionResult EmpCreateFood()
        {
            return PartialView();
        }
        public ActionResult ManageFood()
        {
            return PartialView();
        }
        public ActionResult CurrentStock()
        {
            return PartialView();
        }
        public ActionResult MealUsedReport()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult GetItemSearch(string searchTerm)
        {
            var query = myapp.tbl_cm_Item.Where(l => l.IsActive == true).ToList();
            query = query
                    .Where(c => c.ItemId.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                                 c.ItemName != null && c.ItemName.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                               c.ItemCode != null && c.ItemCode.ToString().ToLower().Contains(searchTerm.ToLower())

                               ).ToList();
            //var user = myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).SingleOrDefault();
            //var store = myapp.tbl_cm_StoreEmp.Where(l => l.EmpId == user.UserId).SingleOrDefault();
            var resulst = (from q in query
                           select new CMSItemViewModel
                           {
                               ItemId = q.ItemId,
                               ItemName = q.ItemName,
                               AvailableStock = "0"
                           }).ToList();
            //if (store != null)
            //{
            //    for (int i = 0; i < resulst.Count; i++)
            //    {
            //        var id = resulst[i].ItemId;
            //        List<StockViewModel> finalmodel = new List<StockViewModel>();
            //        int totalsales = (from it in myapp.tbl_cm_Transaction
            //                          join t in myapp.tbl_cm_TransactionItem on it.TransactionId equals t.TransactionId
            //                          where it.CanteenId == store.StoreId && t.ItemId == id && t.Qty != null
            //                          select t.Qty.Value).Sum();
            //        var received = (from it in myapp.tbl_cm_ItemTransfer
            //                        join t in myapp.tbl_cm_ItemTransferItem on it.ItemTransferId equals t.ItemTransferId
            //                        where it.ToStoreId == store.StoreId && t.ItemId == id && t.QtyRecived != null
            //                        select t.QtyRecived.Value).ToList();


            //        var received2 = (from it in myapp.tbl_cm_ReceivedStock
            //                         join t in myapp.tbl_cm_ReceivedStockItem on it.ReceivedStockId equals t.ReceivedStockId
            //                         where it.StoreId == store.StoreId && t.ItemId == id && t.QtyRecived != null
            //                         select t.QtyRecived.Value).ToList();
            //        decimal totalreceived = received.Sum() + received2.Sum();
            //        resulst[i].AvailableStock = (totalreceived - totalsales).ToString();
            //    }

            //}
            //resulst = resulst.Where(m => m.AvailableStock != "0").ToList();
            var finalresult = (from r in resulst
                               select new
                               {
                                   id = r.ItemId,
                                   text = r.ItemName
                               }).ToList();
            return Json(finalresult, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetItemSearchbyCanteenId(string searchTerm, int id)
        {
            var query = (from a in myapp.tbl_cm_TransactionItem
                         join b in myapp.tbl_cm_Item on a.ItemId equals b.ItemId
                         join d in myapp.tbl_cm_Transaction on a.TransactionId equals d.TransactionId
                         where d.CanteenId == id
                         select new
                         {
                             ItemId = a.ItemId,
                             ItemName = b.ItemName,
                             ItemCode = b.ItemCode
                         }).DistinctBy(c => c.ItemId).ToList();
            query = query
                    .Where(c => c.ItemId.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                                 c.ItemName != null && c.ItemName.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                               c.ItemCode != null && c.ItemCode.ToString().ToLower().Contains(searchTerm.ToLower())

                               ).ToList();

            var finalresult = (from r in query
                               select new
                               {
                                   id = r.ItemId,
                                   text = r.ItemName
                               }).ToList();
            return Json(finalresult, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMSCategory(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cm_Category select d).ToList();
            IEnumerable<tbl_cm_Category> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.CategoryId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.CategoryName != null && c.CategoryName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.CategoryDescription != null && c.CategoryDescription.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                                              c.CategoryId.ToString(),
                                              c.CategoryName,
                                              c.CategoryDescription,
                                              //c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.CategoryId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save_UpdateCMSCategory(tbl_cm_Category model)
        {
            tbl_cm_Category _Category = new tbl_cm_Category();
            if (model.CategoryId != 0)
            {
                _Category = myapp.tbl_cm_Category.Where(m => m.CategoryId == model.CategoryId).FirstOrDefault();
                _Category.ModifiedBy = User.Identity.Name;
                _Category.ModifiedOn = DateTime.Now;
            }
            _Category.CategoryCode = model.CategoryCode;
            _Category.CategoryDescription = model.CategoryDescription;
            _Category.CategoryName = model.CategoryName;
            _Category.IsActive = true;
            if (model.CategoryId == 0)
            {
                _Category.CreatedBy = User.Identity.Name;
                _Category.CreatedOn = DateTime.Now;
                myapp.tbl_cm_Category.Add(_Category);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCMCategory(int id)
        {
            var cat = myapp.tbl_cm_Category.Where(l => l.CategoryId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_cm_Category.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategory()
        {
            var query = myapp.tbl_cm_Category.ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCanteen()
        {
            var query = myapp.tbl_cm_Canteen.ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStore()
        {
            var query = myapp.tbl_cm_Store.ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMSMealType(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cm_MealType select d).ToList();
            IEnumerable<tbl_cm_MealType> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.MealTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.MealTypeName != null && c.MealTypeName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.MealTypeDescription != null && c.MealTypeDescription.ToString().ToLower().Contains(param.sSearch.ToLower())


                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                                              c.MealTypeId.ToString(),

                                              c.MealTypeName,
                                              c.MealTypeDescription,
                                              //c.IsActive.ToString(),
                                              c.MealTypeId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save_UpdateCMSMealType(tbl_cm_MealType model)
        {
            tbl_cm_MealType _MealType = new tbl_cm_MealType();
            if (model.MealTypeId != 0)
            {
                _MealType = myapp.tbl_cm_MealType.Where(m => m.MealTypeId == model.MealTypeId).FirstOrDefault();
                _MealType.ModifiedBy = User.Identity.Name;
                _MealType.ModifiedOn = DateTime.Now;
            }
            _MealType.MealTypeDescription = model.MealTypeDescription;
            _MealType.MealTypeName = model.MealTypeName;
            _MealType.IsActive = true;
            if (model.MealTypeId == 0)
            {
                _MealType.CreatedBy = User.Identity.Name;
                _MealType.CreatedOn = DateTime.Now;
                _MealType.ModifiedOn = DateTime.Now;
                myapp.tbl_cm_MealType.Add(_MealType);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCMMealType(int id)
        {
            var cat = myapp.tbl_cm_MealType.Where(l => l.MealTypeId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_cm_MealType.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMealType()
        {
            var query = myapp.tbl_cm_MealType.ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMSUnitType(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cm_UnitType select d).ToList();
            IEnumerable<tbl_cm_UnitType> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.UnitTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.UnitTypeName != null && c.UnitTypeName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.UnitTypeDescription != null && c.UnitTypeDescription.ToString().ToLower().Contains(param.sSearch.ToLower())
                              //    ||
                              //c.UnitTypeCode != null && c.UnitTypeCode.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                                              c.UnitTypeId.ToString(),
                                              //c.UnitTypeCode,
                                              c.UnitTypeName,
                                              c.UnitTypeDescription,
                                              //c.IsActive.ToString(),
                                              c.UnitTypeId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save_UpdateCMSUnitType(tbl_cm_UnitType model)
        {
            tbl_cm_UnitType _UnitType = new tbl_cm_UnitType();
            if (model.UnitTypeId != 0)
            {
                _UnitType = myapp.tbl_cm_UnitType.Where(m => m.UnitTypeId == model.UnitTypeId).FirstOrDefault();
                _UnitType.ModifiedBy = User.Identity.Name;
                _UnitType.ModifiedOn = DateTime.Now;
            }
            _UnitType.UnitTypeCode = model.UnitTypeCode;
            _UnitType.UnitTypeDescription = model.UnitTypeDescription;
            _UnitType.UnitTypeName = model.UnitTypeName;
            _UnitType.IsActive = true;
            if (model.UnitTypeId == 0)
            {
                _UnitType.CreatedBy = User.Identity.Name;
                _UnitType.CreatedOn = DateTime.Now;
                _UnitType.ModifiedOn = DateTime.Now;
                myapp.tbl_cm_UnitType.Add(_UnitType);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCMUnitType(int id)
        {
            var cat = myapp.tbl_cm_UnitType.Where(l => l.UnitTypeId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_cm_UnitType.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUnitType()
        {
            var query = myapp.tbl_cm_UnitType.ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMSCanteen(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cm_Canteen select d).ToList();
            IEnumerable<tbl_cm_Canteen> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.CanteenId.ToString().ToLower().Contains(param.sSearch.ToLower())

                                  ||
                              c.CanteenName != null && c.CanteenName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.CanteenLocation != null && c.CanteenLocation.ToString().ToLower().Contains(param.sSearch.ToLower())

                                  ||
                              c.CanteenInchargeEmpId != null && c.CanteenInchargeEmpId.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                                              c.CanteenId.ToString(),

                                              c.CanteenName,

                                                (from V in myapp.tbl_Location where V.LocationId == c.CanteenLocation select V.LocationName).SingleOrDefault(),

                                              (from V in myapp.tbl_User where V.UserId == c.CanteenInchargeEmpId select V.FirstName+" "+V.LastName).SingleOrDefault(),

                                              (from V in myapp.tbl_User where V.UserId == c.CanteenManagerEmpId select V.FirstName+" "+V.LastName).SingleOrDefault(),
                                              c.IsActive.ToString(),
                                              c.CanteenId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save_UpdateCMSCanteen(tbl_cm_Canteen model)
        {
            tbl_cm_Canteen canteen = new tbl_cm_Canteen();
            if (model.CanteenId != 0)
            {
                canteen = myapp.tbl_cm_Canteen.Where(m => m.CanteenId == model.CanteenId).FirstOrDefault();
                canteen.ModifiedBy = User.Identity.Name;
                canteen.ModifiedOn = DateTime.Now;
            }
            canteen.CanteenName = model.CanteenName;
            canteen.CanteenLocation = model.CanteenLocation;
            canteen.CanteenInchargeEmpId = model.CanteenInchargeEmpId;
            canteen.CanteenManagerEmpId = model.CanteenManagerEmpId;
            canteen.IsActive = true;
            if (model.CanteenId == 0)
            {
                canteen.CreatedBy = User.Identity.Name;
                canteen.CreatedOn = DateTime.Now;
                canteen.ModifiedOn = DateTime.Now;
                myapp.tbl_cm_Canteen.Add(canteen);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_Canteen_ById(int id)
        {
            var viewmode = myapp.tbl_cm_Canteen.Where(m => m.CanteenId == id).FirstOrDefault();
            var inChargeEmp = (from V in myapp.tbl_User where V.UserId == viewmode.CanteenInchargeEmpId select V.FirstName + " " + V.LastName + " - " + V.Designation + " - " + V.DepartmentName).SingleOrDefault();
            var managerEmp = (from V in myapp.tbl_User where V.UserId == viewmode.CanteenManagerEmpId select V.FirstName + " " + V.LastName + " - " + V.Designation + " - " + V.DepartmentName).SingleOrDefault();
            object obj = new { model = viewmode, inChargeEmp = inChargeEmp, managerEmp = managerEmp };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_Canteen()
        {
            var viewmode = myapp.tbl_cm_Canteen.ToList();
            return Json(viewmode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCMCanteen(int id)
        {
            var cat = myapp.tbl_cm_Canteen.Where(l => l.CanteenId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_cm_Canteen.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetCMS_Items()
        {
            var viewmode = myapp.tbl_cm_Item.Where(l => l.IsActive == true).ToList();
            return Json(viewmode, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMSStore(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cm_Store select d).ToList();
            IEnumerable<tbl_cm_Store> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.StoreId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||

                              c.StoreName != null && c.StoreName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.StoreLocation != null && c.StoreLocation.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                                              c.StoreId.ToString(),
                                              c.StoreName,
                                              c.TypeOf,
                                           (from V in myapp.tbl_Location where V.LocationId == c.StoreLocation select V.LocationName).SingleOrDefault(),
                                          c.IsPrimary.ToString(),c.Iskitchen.ToString(),
                                          (from V in myapp.tbl_User where V.UserId == c.StoreInchargeEmpId select V.FirstName+" "+V.LastName).SingleOrDefault(),

                                              (from V in myapp.tbl_User where V.UserId == c.StoreManagerEmpId select V.FirstName+" "+V.LastName).SingleOrDefault(),
                                              //c.IsActive.ToString(),
                                              c.StoreId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save_UpdateCMSStore(tbl_cm_Store model)
        {
            tbl_cm_Store store = new tbl_cm_Store();
            if (model.StoreId != 0)
            {
                store = myapp.tbl_cm_Store.Where(m => m.StoreId == model.StoreId).FirstOrDefault();
                store.ModifiedBy = User.Identity.Name;
                store.ModifiedOn = DateTime.Now;
            }

            store.StoreName = model.StoreName;
            store.StoreLocation = model.StoreLocation;

            store.StoreInchargeEmpId = model.StoreInchargeEmpId;

            store.StoreManagerEmpId = model.StoreManagerEmpId;
            store.Iskitchen = model.Iskitchen;
            store.IsPrimary = model.IsPrimary;
            store.TypeOf = model.TypeOf;
            store.IsActive = true;
            if (model.StoreId == 0)
            {
                store.CreatedBy = User.Identity.Name;
                store.CreatedOn = DateTime.Now;
                store.ModifiedOn = DateTime.Now;
                myapp.tbl_cm_Store.Add(store);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_Store_ById(int id)
        {
            var viewmode = myapp.tbl_cm_Store.Where(m => m.StoreId == id).FirstOrDefault();
            var inChargeEmp = (from V in myapp.tbl_User where V.UserId == viewmode.StoreInchargeEmpId select V.FirstName + " " + V.LastName + " - " + V.Designation + " - " + V.DepartmentName).SingleOrDefault();
            var managerEmp = (from V in myapp.tbl_User where V.UserId == viewmode.StoreManagerEmpId select V.FirstName + " " + V.LastName + " - " + V.Designation + " - " + V.DepartmentName).SingleOrDefault();
            object obj = new { model = viewmode, inChargeEmp = inChargeEmp, managerEmp = managerEmp };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_Payment_ById(int id)
        {
            var viewmode = myapp.tbl_cm_Payment.Where(m => m.PaymentId == id).FirstOrDefault();
            var PaidEmp = (from V in myapp.tbl_User where V.UserId == viewmode.PaidByEmpId select V.FirstName + " " + V.LastName + " - " + V.Designation + " - " + V.DepartmentName).SingleOrDefault();
            var ReceivedEmp = (from V in myapp.tbl_User where V.UserId == viewmode.ReceivedByEmpId select V.FirstName + " " + V.LastName + " - " + V.Designation + " - " + V.DepartmentName).SingleOrDefault();
            object obj = new { model = viewmode, PaidEmp = PaidEmp, ReceivedEmp = ReceivedEmp };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_Store()
        {
            var viewmode = myapp.tbl_cm_Store.ToList();
            return Json(viewmode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCMStore(int id)
        {
            var cat = myapp.tbl_cm_Store.Where(l => l.StoreId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_cm_Store.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMSItem(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cm_Item select d).ToList();
            IEnumerable<tbl_cm_Item> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.ItemId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.ItemName != null && c.ItemName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.ItemDescription != null && c.ItemDescription.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                              c.ItemCode != null && c.ItemCode.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.ItemType != null && c.ItemType.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                                              c.ItemId.ToString(),
                                              c.ItemCode,
                                              c.ItemName,
                                              c.ItemDescription,
                             (from V in myapp.tbl_cm_UnitType where V.UnitTypeId == c.UnitTypeId select V.UnitTypeName).SingleOrDefault(),
                             c.ItemType,c.TotalCost,c.TotalCostStaff,c.PackagingCost,
                                              c.IsActive.ToString(),
                                              c.ItemId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save_UpdateItem(CMSItemViewModel model, HttpPostedFileBase UploadDocument)
        {
            try
            {
                tbl_cm_Item dbmodel = new tbl_cm_Item();
                if (model.ItemId != 0)
                {
                    dbmodel.ModifiedBy = User.Identity.Name;
                    dbmodel.ModifiedOn = DateTime.Now;
                    var id = model.ItemId;
                    dbmodel = myapp.tbl_cm_Item.Where(i => i.ItemId == id).FirstOrDefault();
                }
                dbmodel.ItemCode = model.ItemCode;
                dbmodel.ItemDescription = model.ItemDescription;

                dbmodel.ItemName = model.ItemName;
                dbmodel.ItemType = model.ItemType;
                dbmodel.IsActive = true;
                dbmodel.UnitTypeId = model.UnitTypeId;
                dbmodel.TotalCost = model.TotalCost;
                dbmodel.TotalCostStaff = model.TotalCostStaff;
                dbmodel.PackagingCost = model.PackagingCost;
                //if (UploadDocument != null)
                //{
                //    string fileName = Path.GetFileNameWithoutExtension(UploadDocument.FileName);
                //    string extension = Path.GetExtension(UploadDocument.FileName);
                //    string guidid = Guid.NewGuid().ToString();
                //    fileName = fileName + guidid + extension;
                //    UploadDocument.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));
                //    dbmodel.ItemImage = fileName;
                //}
                if (model.ItemId == 0)
                {
                    dbmodel.CreatedBy = User.Identity.Name;
                    dbmodel.CreatedOn = DateTime.Now;
                    myapp.tbl_cm_Item.Add(dbmodel);
                }

                myapp.SaveChanges();
                Save_UpDateItemCategory(model.Category, dbmodel.ItemId);
                Save_UpDateItemMealType(model.MealType, dbmodel.ItemId);


                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public void Save_UpDateItemCategory(string category, int id)
        {
            var cat = myapp.tbl_cm_ItemCategory.Where(m => m.ItemId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_cm_ItemCategory.RemoveRange(cat);
                myapp.SaveChanges();
            }
            if (!string.IsNullOrEmpty(category))
            {
                var categoryList = category.Split(',');
                for (int i = 0; i < categoryList.Length; i++)
                {
                    tbl_cm_ItemCategory _ItemCategory = new tbl_cm_ItemCategory();
                    _ItemCategory.CategoryId = Convert.ToInt32(categoryList[i]);
                    _ItemCategory.ItemId = id;
                    _ItemCategory.IsActive = true;
                    myapp.tbl_cm_ItemCategory.Add(_ItemCategory);
                    myapp.SaveChanges();
                }

            }
        }
        public void Save_UpDateItemMealType(string mealType, int id)
        {
            var cat = myapp.tbl_cm_ItemMealType.Where(m => m.ItemId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_cm_ItemMealType.RemoveRange(cat);
                myapp.SaveChanges();
            }
            if (!string.IsNullOrEmpty(mealType))
            {
                var categoryList = mealType.Split(',');
                for (int i = 0; i < categoryList.Length; i++)
                {
                    tbl_cm_ItemMealType _ItemCategory = new tbl_cm_ItemMealType();
                    _ItemCategory.MealTypeId = Convert.ToInt32(categoryList[i]);
                    _ItemCategory.ItemId = id;
                    _ItemCategory.IsActive = true;
                    myapp.tbl_cm_ItemMealType.Add(_ItemCategory);
                    myapp.SaveChanges();
                }

            }
        }
        public async Task<JsonResult> GetCMS_Item(int id)
        {
            var model = await myapp.tbl_cm_Item.Where(m => m.ItemId == id).FirstOrDefaultAsync();
            CMSItemViewModel dbmodel = new CMSItemViewModel();
            //var user = await myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).SingleOrDefaultAsync();
            //var store = await myapp.tbl_cm_StoreEmp.Where(l => l.EmpId == user.UserId).SingleOrDefaultAsync();
            //if (store != null)
            //{
            //    List<StockViewModel> finalmodel = new List<StockViewModel>();
            //    int totalsales = await (from it in myapp.tbl_cm_Transaction
            //                            join t in myapp.tbl_cm_TransactionItem on it.TransactionId equals t.TransactionId
            //                            where it.CanteenId == store.StoreId && t.ItemId == id && t.Qty != null
            //                            select t.Qty.Value).SumAsync();
            //    var received = await (from it in myapp.tbl_cm_ItemTransfer
            //                          join t in myapp.tbl_cm_ItemTransferItem on it.ItemTransferId equals t.ItemTransferId
            //                          where it.ToStoreId == store.StoreId && t.ItemId == id && t.QtyRecived != null
            //                          select t.QtyRecived.Value).ToListAsync();


            //    var received2 = await (from it in myapp.tbl_cm_ReceivedStock
            //                            join t in myapp.tbl_cm_ReceivedStockItem on it.ReceivedStockId equals t.ReceivedStockId
            //                            where it.StoreId == store.StoreId && t.ItemId == id && t.QtyRecived != null
            //                            select t.QtyRecived.Value).ToListAsync();
            //   decimal totalreceived = received.Sum() +  received2.Sum();
            //    dbmodel.AvailableStock = (totalreceived - totalsales).ToString();
            //}
            //else
            //{
            dbmodel.AvailableStock = "100";
            //}
            dbmodel.ItemCode = model.ItemCode;
            dbmodel.ItemDescription = model.ItemDescription;
            dbmodel.ItemName = model.ItemName;
            dbmodel.ItemType = model.ItemType;
            dbmodel.UnitTypeId = model.UnitTypeId;
            dbmodel.ItemId = model.ItemId;
            dbmodel.TotalCost = model.TotalCost;
            dbmodel.PackagingCost = model.PackagingCost;
            dbmodel.TotalCostStaff = model.TotalCostStaff.HasValue ? model.TotalCostStaff.Value : 0;
            var category = await myapp.tbl_cm_ItemCategory.Where(m => m.ItemId == dbmodel.ItemId).ToListAsync();
            if (category.Count > 0)
            {
                for (int t = 0; t < category.Count; t++)
                {
                    if (dbmodel.Category == null)
                    {
                        dbmodel.Category = category[t].CategoryId.ToString();
                    }
                    else
                    {
                        dbmodel.Category += "," + category[t].CategoryId.ToString();
                    }
                }
            }
            var mealType = await myapp.tbl_cm_ItemMealType.Where(m => m.ItemId == dbmodel.ItemId).ToListAsync();
            if (mealType.Count > 0)
            {
                for (int t = 0; t < mealType.Count; t++)
                {
                    if (dbmodel.MealType == null)
                    {
                        dbmodel.MealType = mealType[t].MealTypeId.ToString();
                    }
                    else
                    {
                        dbmodel.MealType += "," + mealType[t].MealTypeId.ToString();
                    }
                }
            }
            return Json(dbmodel, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteCMItem(int id)
        {
            var cat = myapp.tbl_cm_Item.Where(l => l.ItemId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_cm_Item.Remove(cat[0]);
                myapp.SaveChanges();
                var category = myapp.tbl_cm_ItemCategory.Where(m => m.ItemId == id).ToList();
                if (category.Count > 0)
                {
                    myapp.tbl_cm_ItemCategory.RemoveRange(category);
                    myapp.SaveChanges();
                }
                var MealType = myapp.tbl_cm_ItemMealType.Where(m => m.ItemId == id).ToList();
                if (MealType.Count > 0)
                {
                    myapp.tbl_cm_ItemMealType.RemoveRange(MealType);
                    myapp.SaveChanges();
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult InActiveCMItem(int id)
        {
            var cat = myapp.tbl_cm_Item.Where(l => l.ItemId == id).ToList();
            if (cat.Count > 0)
            {
                cat[0].IsActive = false;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ActiveCMItem(int id)
        {
            var cat = myapp.tbl_cm_Item.Where(l => l.ItemId == id).ToList();
            if (cat.Count > 0)
            {
                cat[0].IsActive = true;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMSStock(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cm_ReceivedStock select d).ToList();
            IEnumerable<tbl_cm_ReceivedStock> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.ReceivedStockId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.StoreId.ToString() != null && c.StoreId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.TotalItems != null && c.TotalItems.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                              c.TotalItemsAccepted != null && c.TotalItemsAccepted.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.TotalItemsReturn != null && c.TotalItemsReturn.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.TotalItemsDamage != null && c.TotalItemsDamage.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                              c.StockRecivedNotes != null && c.StockRecivedNotes.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                                              c.ReceivedStockId.ToString(),
                                                                       (from V in myapp.tbl_cm_Store where V.StoreId == c.StoreId select V.StoreName).SingleOrDefault(),
                                               c.StockRecivedOn.HasValue?(c.StockRecivedOn.Value.ToString("dd/MM/yyyy")):"",
                                                                         (from V in myapp.tbl_User where V.UserId == c.StockRecivedEmpId select V.FirstName+" "+V.LastName).SingleOrDefault(),
                                               c.StockRecivedNotes,
                                              c.TotalItems,
                                              c.TotalItemsAccepted.ToString(),
                                               c.TotalItemsReturn.ToString(),
                                              c.TotalItemsDamage.ToString(),
                                              //c.IsActive.ToString(),
                                              c.ReceivedStockId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save_UpdateCMSStock(ReveicedStockViewModel model)
        {
            tbl_cm_ReceivedStock stock = new tbl_cm_ReceivedStock();
            if (model.ReceivedStockId != 0)
            {
                stock = myapp.tbl_cm_ReceivedStock.Where(m => m.ReceivedStockId == model.ReceivedStockId).FirstOrDefault();
                stock.ModifiedBy = User.Identity.Name;
                stock.ModifiedOn = DateTime.Now;
            }
            stock.StoreId = model.StoreId;
            stock.StockRecivedNotes = model.StockRecivedNotes;
            stock.TotalItems = model.TotalItems;
            stock.TotalItemsAccepted = model.TotalItemsAccepted;
            stock.TotalItemsReturn = model.TotalItemsReturn;
            stock.TotalItemsDamage = model.TotalItemsDamage;
            stock.StockRecivedEmpId = model.StockRecivedEmpId;
            stock.StockRecivedOn = ProjectConvert.ConverDateStringtoDatetime(model.StockRecivedOn);
            stock.IsActive = true;
            if (model.ReceivedStockId == 0)
            {
                stock.CreatedBy = User.Identity.Name;
                stock.CreatedOn = DateTime.Now;
                stock.ModifiedOn = DateTime.Now;
                myapp.tbl_cm_ReceivedStock.Add(stock);
            }
            myapp.SaveChanges();
            if (model.ReceivedStockId == 0)
                return Json(stock.ReceivedStockId, JsonRequestBehavior.AllowGet);
            else
                return Json(model.ReceivedStockId, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_Stock_ById(int id)
        {
            var model = myapp.tbl_cm_ReceivedStock.Where(m => m.ReceivedStockId == id).FirstOrDefault();
            ReveicedStockViewModel stock = new ReveicedStockViewModel();
            stock.StoreId = model.StoreId;
            stock.StockRecivedNotes = model.StockRecivedNotes;
            stock.TotalItems = model.TotalItems;
            stock.TotalItemsAccepted = model.TotalItemsAccepted;
            stock.TotalItemsReturn = model.TotalItemsReturn;
            stock.TotalItemsDamage = model.TotalItemsDamage;
            stock.StockRecivedEmpId = model.StockRecivedEmpId;
            stock.StockRecivedEmpName = (from V in myapp.tbl_User where V.UserId == stock.StockRecivedEmpId select V.FirstName + " " + V.LastName + " - " + V.Designation + " - " + V.DepartmentName).SingleOrDefault();

            stock.StockRecivedOn = model.StockRecivedOn.HasValue ? model.StockRecivedOn.Value.ToString("dd/MM/yyyy") : "[N/A]";
            return Json(stock, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_Stock()
        {
            var viewmode = myapp.tbl_cm_ReceivedStock.ToList();
            return Json(viewmode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCMStock(int id)
        {
            var cat = myapp.tbl_cm_ReceivedStock.Where(l => l.ReceivedStockId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_cm_ReceivedStock.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetCMSManageDistributeItems(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cm_ItemTransfer select d).ToList();
            IEnumerable<tbl_cm_ItemTransfer> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.ItemTransferId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.ToStoreId != null && c.ToStoreId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.FromStoreId != null && c.FromStoreId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                              c.SenderEmpId != null && c.SenderEmpId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.ModeOfTransfer != null && c.ModeOfTransfer.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                                              c.ItemTransferId.ToString(),
                              (from V in myapp.tbl_cm_Store where V.StoreId == c.FromStoreId select V.StoreName).SingleOrDefault(),
                             (from V in myapp.tbl_cm_Store where V.StoreId == c.ToStoreId select V.StoreName).SingleOrDefault(),
                                              c.ModeOfTransfer,
                             (from V in myapp.tbl_User where V.UserId == c.SenderEmpId select V.FirstName+" "+V.LastName).SingleOrDefault(),
                             (from V in myapp.tbl_User where V.UserId == c.ReceiverEmpId select V.FirstName+" "+V.LastName).SingleOrDefault(),
                                              //c.SenderNotes,
                                              //c.ReceiverNotes,
                                              c.SentOnDate+" "+c.SentOnTime,
                                                c.ReceivedOnDate+" "+c.ReceivedOnTime,
                                              //c.IsCanteen.ToString(),
                             (from V in myapp.tbl_Location where V.LocationId == c.FromLocationId select V.LocationName).SingleOrDefault(),
                             (from V in myapp.tbl_Location where V.LocationId == c.ToLocationId select V.LocationName).SingleOrDefault(),
                                         //c.IsActive.ToString(),
                                              c.ItemTransferId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMSItemTransfer(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cm_ItemTransfer select d).ToList();
            IEnumerable<tbl_cm_ItemTransfer> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.ItemTransferId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.ToStoreId != null && c.ToStoreId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.FromStoreId != null && c.FromStoreId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                              c.RequestedEmpId != null && c.RequestedEmpId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.ModeOfTransfer != null && c.ModeOfTransfer.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                                              c.ItemTransferId.ToString(),
                              (from V in myapp.tbl_cm_Store where V.StoreId == c.FromStoreId select V.StoreName).SingleOrDefault(),
                             (from V in myapp.tbl_cm_Store where V.StoreId == c.ToStoreId select V.StoreName).SingleOrDefault(),
                                              //c.ModeOfTransfer,
                             (from V in myapp.tbl_User where V.UserId == c.SenderEmpId select V.FirstName+" "+V.LastName).SingleOrDefault(),
                             (from V in myapp.tbl_User where V.UserId == c.ReceiverEmpId select V.FirstName+" "+V.LastName).SingleOrDefault(),
                                              //c.SenderNotes,
                                              //c.ReceiverNotes,
                                              c.SentOnDate+" "+c.SentOnTime,
                                                c.ReceivedOnDate+" "+c.ReceivedOnTime,
                             (from V in myapp.tbl_User where V.UserId == c.RequestedEmpId select V.FirstName+" "+V.LastName).SingleOrDefault(),
                                    c.RequestedOn.HasValue?(c.RequestedOn.Value.ToString("dd/MM/yyyy")):"",
                             //                 c.IsCanteen.ToString(),
                             //(from V in myapp.tbl_Location where V.LocationId == c.FromLocationId select V.LocationName).SingleOrDefault(),
                             //(from V in myapp.tbl_Location where V.LocationId == c.ToLocationId select V.LocationName).SingleOrDefault(),
                             //              c.IsCanteen.ToString(),   c.IsActive.ToString(),
                                              c.ItemTransferId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save_UpdateDistrubItems(ItemTransferviewModel model, HttpPostedFileBase SenderAttachment, HttpPostedFileBase ReceiverAttachment)
        {
            try
            {
                tbl_cm_ItemTransfer dbmodel = new tbl_cm_ItemTransfer();
                if (model.ItemTransferId != 0)
                {
                    dbmodel.ModifiedBy = User.Identity.Name;
                    dbmodel.ModifiedOn = DateTime.Now;
                    var id = model.ItemTransferId;
                    dbmodel = myapp.tbl_cm_ItemTransfer.Where(m => m.ItemTransferId == id).SingleOrDefault();
                }
                dbmodel.FromLocationId = (from V in myapp.tbl_cm_Store where V.StoreId == model.FromStoreId select V.StoreLocation).SingleOrDefault();
                dbmodel.FromStoreId = model.FromStoreId;
                dbmodel.IsCanteen = model.IsCanteen;
                dbmodel.ModeOfTransfer = "Van";
                dbmodel.RequestedEmpId = model.RequestedEmpId;
                if (model.RequestedOn != null)
                    dbmodel.RequestedOn = ProjectConvert.ConverDateStringtoDatetime(model.RequestedOn);
                dbmodel.RequestorNotes = model.RequestorNotes;
                dbmodel.ToLocationId = (from V in myapp.tbl_cm_Store where V.StoreId == model.ToLocationId select V.StoreLocation).SingleOrDefault();
                dbmodel.ToStoreId = model.ToStoreId;
                dbmodel.SenderEmpId = model.SenderEmpId;
                dbmodel.SenderNotes = model.SenderNotes;
                dbmodel.SentOnDate = model.SentOnDate;

                dbmodel.SentOnTime = model.SentOnTime;
                dbmodel.ReceivedOnDate = model.ReceivedOnDate;
                dbmodel.ReceivedOnTime = model.ReceivedOnTime;
                dbmodel.ReceiverEmpId = model.ReceiverEmpId;

                dbmodel.ReceiverNotes = model.ReceiverNotes;
                dbmodel.IsActive = true;
                if (ReceiverAttachment != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ReceiverAttachment.FileName);
                    string extension = Path.GetExtension(ReceiverAttachment.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    ReceiverAttachment.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));
                    dbmodel.ReceiverAttachment = fileName;
                }
                if (SenderAttachment != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(SenderAttachment.FileName);
                    string extension = Path.GetExtension(SenderAttachment.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    SenderAttachment.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));
                    dbmodel.SenderAttachment = fileName;
                }
                if (model.ItemTransferId == 0)
                {
                    dbmodel.CreatedBy = User.Identity.Name;
                    dbmodel.CreatedOn = DateTime.Now;
                    myapp.tbl_cm_ItemTransfer.Add(dbmodel);
                }

                myapp.SaveChanges();
                if (model.ItemTransferId == 0)
                    return Json(dbmodel.ItemTransferId, JsonRequestBehavior.AllowGet);
                else
                    return Json(model.ItemTransferId, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult SaveItemTransfer(ItemTransferviewModel model)
        {
            try
            {
                tbl_cm_ItemTransfer dbmodel = new tbl_cm_ItemTransfer();
                dbmodel.FromLocationId = (from V in myapp.tbl_cm_Store where V.StoreId == model.FromStoreId select V.StoreLocation).SingleOrDefault();
                dbmodel.FromStoreId = model.FromStoreId;
                dbmodel.IsCanteen = model.IsCanteen;
                dbmodel.ModeOfTransfer = "Van";
                dbmodel.RequestedEmpId = model.RequestedEmpId;
                if (model.RequestedOn != null)
                    dbmodel.RequestedOn = ProjectConvert.ConverDateStringtoDatetime(model.RequestedOn);
                dbmodel.RequestorNotes = model.RequestorNotes;
                dbmodel.ToLocationId = (from V in myapp.tbl_cm_Store where V.StoreId == model.ToStoreId select V.StoreLocation).SingleOrDefault();
                dbmodel.ToStoreId = model.ToStoreId;
                dbmodel.IsActive = true;
                dbmodel.CreatedBy = User.Identity.Name;
                dbmodel.CreatedOn = DateTime.Now;
                myapp.tbl_cm_ItemTransfer.Add(dbmodel);
                myapp.SaveChanges();
                if (model.ItemTransferId == 0)
                    return Json(dbmodel.ItemTransferId, JsonRequestBehavior.AllowGet);
                else
                    return Json(model.ItemTransferId, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public void SendEmailItemTransfer(tbl_cm_ItemTransfer model)
        {
            string body = string.Empty;
            var id = model.ItemTransferId;
            using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/ItemTransferTemplate.html")))
            {
                body = reader.ReadToEnd();
            }

            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel
            {
                fromemail = "Leave@hospitals.com",
                toemail = "phanisrinivas111@gmail.com",
                //  subject = Subject,
                body = body,
                filepath = "",
                fromname = "",
                ccemail = ""
            };
            cm.SendEmail(mailmodel);
        }
        public ActionResult UpdateItemTransfer(ItemTransferviewModel model)
        {
            try
            {
                var id = model.ItemTransferId;
                tbl_cm_ItemTransfer dbmodel = myapp.tbl_cm_ItemTransfer.Where(m => m.ItemTransferId == id).SingleOrDefault();
                dbmodel.ModifiedBy = User.Identity.Name;
                dbmodel.ModifiedOn = DateTime.Now;
                dbmodel.FromLocationId = model.FromLocationId;
                dbmodel.FromStoreId = model.FromStoreId;
                dbmodel.IsCanteen = model.IsCanteen;
                dbmodel.ModeOfTransfer = model.ModeOfTransfer;
                dbmodel.RequestedEmpId = model.RequestedEmpId;
                if (model.RequestedOn != null)
                    dbmodel.RequestedOn = ProjectConvert.ConverDateStringtoDatetime(model.RequestedOn);
                dbmodel.RequestorNotes = model.RequestorNotes;
                dbmodel.ToLocationId = model.ToLocationId;
                dbmodel.ToStoreId = model.ToStoreId;
                dbmodel.IsActive = true;
                myapp.SaveChanges();
                if (model.ItemTransferId == 0)
                    return Json(dbmodel.ItemTransferId, JsonRequestBehavior.AllowGet);
                else
                    return Json(model.ItemTransferId, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult SenderUpdate(ItemTransferviewModel model, HttpPostedFileBase SenderAttachment)
        {
            try
            {
                var id = model.ItemTransferId;
                tbl_cm_ItemTransfer dbmodel = myapp.tbl_cm_ItemTransfer.Where(m => m.ItemTransferId == id).SingleOrDefault();
                dbmodel.ModifiedBy = User.Identity.Name;
                dbmodel.ModifiedOn = DateTime.Now;
                dbmodel.SenderEmpId = model.SenderEmpId;
                dbmodel.SenderNotes = model.SenderNotes;
                dbmodel.SentOnDate = model.SentOnDate;

                dbmodel.SentOnTime = model.SentOnTime;
                dbmodel.IsActive = true;
                if (SenderAttachment != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(SenderAttachment.FileName);
                    string extension = Path.GetExtension(SenderAttachment.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    SenderAttachment.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));
                    dbmodel.SenderAttachment = fileName;
                }
                myapp.SaveChanges();
                if (model.ItemTransferId == 0)
                    return Json(dbmodel.ItemTransferId, JsonRequestBehavior.AllowGet);
                else
                    return Json(model.ItemTransferId, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult ReceiverUpdate(ItemTransferviewModel model, HttpPostedFileBase ReceiverAttachment)
        {
            try
            {
                var id = model.ItemTransferId;
                tbl_cm_ItemTransfer dbmodel = myapp.tbl_cm_ItemTransfer.Where(m => m.ItemTransferId == id).SingleOrDefault();
                dbmodel.ModifiedBy = User.Identity.Name;
                dbmodel.ModifiedOn = DateTime.Now;
                dbmodel.ReceivedOnDate = model.ReceivedOnDate;
                dbmodel.ReceivedOnTime = model.ReceivedOnTime;
                dbmodel.ReceiverEmpId = model.ReceiverEmpId;

                dbmodel.ReceiverNotes = model.ReceiverNotes;
                dbmodel.IsActive = true;
                if (ReceiverAttachment != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(ReceiverAttachment.FileName);
                    string extension = Path.GetExtension(ReceiverAttachment.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    ReceiverAttachment.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));
                    dbmodel.ReceiverAttachment = fileName;
                }
                myapp.SaveChanges();
                if (model.ItemTransferId == 0)
                    return Json(dbmodel.ItemTransferId, JsonRequestBehavior.AllowGet);
                else
                    return Json(model.ItemTransferId, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetCMS_ItemTransfer_ById(int id)
        {
            var viewmode = myapp.tbl_cm_ItemTransfer.Where(m => m.ItemTransferId == id).FirstOrDefault();
            ItemTransferviewModel model = new ItemTransferviewModel();
            model.FromLocationId = viewmode.FromLocationId;
            model.FromLocationName = (from V in myapp.tbl_Location where V.LocationId == model.FromLocationId select V.LocationName).SingleOrDefault();

            model.ToLocationId = viewmode.ToLocationId;
            model.ToLocationName = (from V in myapp.tbl_Location where V.LocationId == model.ToLocationId select V.LocationName).SingleOrDefault();

            model.ToStoreId = viewmode.ToStoreId;
            model.ToStoreName = (from V in myapp.tbl_cm_Store where V.StoreId == model.ToStoreId select V.StoreName).SingleOrDefault();

            model.FromStoreId = viewmode.FromStoreId;
            model.FromStoreName = (from V in myapp.tbl_cm_Store where V.StoreId == model.FromStoreId select V.StoreName).SingleOrDefault();

            model.IsCanteen = viewmode.IsCanteen;
            model.ItemTransferId = viewmode.ItemTransferId;
            model.ModeOfTransfer = viewmode.ModeOfTransfer;
            model.ReceivedOnDate = viewmode.ReceivedOnDate;
            model.ReceivedOnTime = viewmode.ReceivedOnTime;
            model.ReceiverAttachment = viewmode.ReceiverAttachment;
            model.ReceiverEmpId = viewmode.ReceiverEmpId;
            model.ReceiverEmpName = (from V in myapp.tbl_User where V.UserId == model.ReceiverEmpId select V.FirstName + " " + V.LastName + " - " + V.Designation + " - " + V.DepartmentName).SingleOrDefault();
            model.ReceiverNotes = viewmode.ReceiverNotes;
            model.RequestedEmpId = viewmode.RequestedEmpId;
            model.RequestedEmpName = (from V in myapp.tbl_User where V.UserId == model.RequestedEmpId select V.FirstName + " " + V.LastName + " - " + V.Designation + " - " + V.DepartmentName).SingleOrDefault();
            model.RequestedOn = viewmode.RequestedOn.HasValue ? viewmode.RequestedOn.Value.ToString("dd/MM/yyyy") : "";
            model.RequestorNotes = viewmode.RequestorNotes;
            model.SenderEmpId = viewmode.SenderEmpId;
            model.SenderEmpName = (from V in myapp.tbl_User where V.UserId == model.SenderEmpId select V.FirstName + " " + V.LastName + " - " + V.Designation + " - " + V.DepartmentName).SingleOrDefault();
            model.SentOnDate = viewmode.SentOnDate;
            model.SentOnTime = viewmode.SentOnTime;
            model.SenderAttachment = viewmode.SenderAttachment;
            model.SenderNotes = viewmode.SenderNotes;

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCMItemTransfer(int id)
        {
            var cat = myapp.tbl_cm_ItemTransfer.Where(l => l.ItemTransferId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_cm_ItemTransfer.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save_UpdateItemTransferList(List<tbl_cm_ItemTransferItem> model)

        {
            var id = model[0].ItemTransferId;
            var obj = myapp.tbl_cm_ItemTransferItem.Where(m => m.ItemTransferId == id).ToList();
            myapp.tbl_cm_ItemTransferItem.RemoveRange(obj);
            myapp.SaveChanges();
            for (int i = 0; i < model.Count; i++)
            {
                model[i].IsActive = true;
            }
            myapp.tbl_cm_ItemTransferItem.AddRange(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save_UpdateReceivedStockList(List<tbl_cm_ReceivedStockItem> model)
        {
            var id = model[0].ReceivedStockId;
            var obj = myapp.tbl_cm_ReceivedStockItem.Where(m => m.ReceivedStockId == id).ToList();
            myapp.tbl_cm_ReceivedStockItem.RemoveRange(obj);
            myapp.SaveChanges();
            for (int i = 0; i < model.Count; i++)
            {
                model[i].IsActive = true;
            }
            myapp.tbl_cm_ReceivedStockItem.AddRange(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save_UpdateBillList(List<tbl_cm_TransactionItem> model)
        {

            var tran = model[0].TransactionId;
            var obj = myapp.tbl_cm_TransactionItem.Where(m => m.TransactionId == tran).ToList();
            myapp.tbl_cm_TransactionItem.RemoveRange(obj);
            myapp.SaveChanges();
            for (int i = 0; i < model.Count; i++)
            {
                model[i].IsActive = true;
            }
            myapp.tbl_cm_TransactionItem.AddRange(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        public ActionResult AjaxGetCMSBill(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cm_Transaction select d).ToList();
            IEnumerable<tbl_cm_Transaction> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.TransactionId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.TransactionCustomerType != null && c.TransactionCustomerType.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.ModeOfPayment != null && c.ModeOfPayment.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                              c.DiscountType != null && c.DiscountType.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.EmpId != null && c.EmpId.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                                              c.TransactionId.ToString(),c.TransactionCustomerType,
                             (from V in myapp.tbl_User where V.UserId == c.EmpId select V.FirstName+" "+V.LastName).SingleOrDefault(),
                             (from V in myapp.tbl_User where V.UserId == c.SalesEmpId select V.FirstName+" "+V.LastName).SingleOrDefault(),
                                              c.DiscountType,
                                              c.ModeOfPayment,
                                              c.DiscountValue,
                                                c.TaxAmount,c.TotalPrice,c.FinalPrice,c.RefundAmount,c.IsFreeMeal.ToString(),
                             (from V in myapp.tbl_cm_Canteen where V.CanteenId == c.CanteenId select V.CanteenName).SingleOrDefault(),
                    c.SalesEmpNotes,   c.BillAddress,c.CardNumber,c.NameOfTheCard,   c.IsActive.ToString(),
                                              c.TransactionId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save_UpdateBIll(tbl_cm_Transaction model)
        {
            try
            {
                tbl_cm_Transaction dbmodel = new tbl_cm_Transaction();
                if (model.TransactionId != 0)
                {
                    dbmodel.ModifiedBy = User.Identity.Name;
                    dbmodel.ModifiedOn = DateTime.Now;
                }
                dbmodel.EmpEmail = model.EmpEmail;
                dbmodel.EmpId = model.EmpId;
                dbmodel.BillAddress = model.BillAddress;
                dbmodel.CanteenId = model.CanteenId;
                dbmodel.CardNumber = model.CardNumber;
                dbmodel.DiscountType = model.DiscountType;
                dbmodel.DiscountValue = model.DiscountValue;

                dbmodel.EmpMobile = model.EmpMobile;
                dbmodel.EmpName = model.EmpName;
                dbmodel.FinalPrice = model.FinalPrice;
                dbmodel.IsActive = true;
                dbmodel.IsFreeMeal = model.IsFreeMeal;
                dbmodel.ModeOfPayment = model.ModeOfPayment;
                dbmodel.NameOfTheCard = model.NameOfTheCard;

                dbmodel.RefundAmount = model.RefundAmount;
                dbmodel.SalesEmpId = model.SalesEmpId;
                dbmodel.SalesEmpNotes = model.SalesEmpNotes;
                dbmodel.TaxAmount = model.TaxAmount;
                dbmodel.TotalPrice = model.TotalPrice;
                dbmodel.TransactionCustomerType = model.TransactionCustomerType;

                if (model.TransactionId == 0)
                {
                    dbmodel.CreatedBy = User.Identity.Name;
                    dbmodel.CreatedOn = DateTime.Now;
                    myapp.tbl_cm_Transaction.Add(dbmodel);
                }

                myapp.SaveChanges();
                if (model.TransactionId == 0)
                    return Json(dbmodel.TransactionId, JsonRequestBehavior.AllowGet);
                else
                    return Json(model.TransactionId, JsonRequestBehavior.AllowGet);



            }
            catch (Exception ex)
            {
                return Json("Error" + ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult GetCMS_Bill_ById(int id)
        {
            var viewmode = myapp.tbl_cm_Transaction.Where(m => m.TransactionId == id).FirstOrDefault();
            return Json(viewmode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCMBill(int id)
        {
            var cat = myapp.tbl_cm_Transaction.Where(l => l.TransactionId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_cm_Transaction.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_BillItems_BytranId(int id)
        {
            var viewmode = myapp.tbl_cm_TransactionItem.Where(m => m.TransactionId == id).ToList();
            return Json(viewmode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_TransferItems_BytraId(int id)
        {
            var viewmode = myapp.tbl_cm_ItemTransferItem.Where(m => m.ItemTransferId == id).ToList();
            return Json(viewmode, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_stockItems_ByResId(int id)
        {
            var viewmode = myapp.tbl_cm_ReceivedStockItem.Where(m => m.ReceivedStockId == id).ToList();
            return Json(viewmode, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetVendorMealRequest(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cm_VendorMealRequest select d).ToList();
            IEnumerable<tbl_cm_VendorMealRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.VendorStockRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.VendorId != null && c.VendorId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                              c.RequestedOn != null && c.RequestedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                                              c.VendorStockRequestId.ToString(),
                                              (from V in myapp.tbl_Vendor where V.VendorId == c.VendorId select V.Name).SingleOrDefault(),
                                              (from V in myapp.tbl_Location where V.LocationId == c.LocationId select V.LocationName).SingleOrDefault(),
                                              c.RequestedOn.HasValue?c.RequestedOn.Value.ToString("dd/MM/yyyy"):"",
                                            c.TotalNotes,
                                              c.IsActive.ToString(),
                                              c.VendorStockRequestId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_VendorMealType()
        {
            var view = myapp.tbl_cm_VendorMealRequest.ToList();
            view = view.Where(m => m.RequestedOn.Value.ToString("dd/MM/yyyy") == DateTime.Now.ToString("dd/MM/yyyy")).ToList();
            var list = new List<VendorMealRequestModel>();
            for (int i = 0; i < view.Count; i++)
            {
                var viewmode = new VendorMealRequestModel();
                viewmode.VendorStockRequestId = view[i].VendorStockRequestId;
                var id = view[i].VendorId;
                viewmode.VendorName = (from V in myapp.tbl_Vendor where V.VendorId == id select V.Name).SingleOrDefault();
                id = view[i].LocationId;
                viewmode.LocationName = (from V in myapp.tbl_Location where V.LocationId == id select V.LocationName).SingleOrDefault();
                list.Add(viewmode);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save_UpdateCMSVendorMealRequest(VendorMealRequestModel model)
        {
            tbl_cm_VendorMealRequest stock = new tbl_cm_VendorMealRequest();
            if (model.VendorStockRequestId != 0)
            {
                stock = myapp.tbl_cm_VendorMealRequest.Where(m => m.VendorStockRequestId == model.VendorStockRequestId).FirstOrDefault();
                stock.ModifiedBy = User.Identity.Name;
                stock.ModifiedOn = DateTime.Now;
            }
            stock.VendorId = model.VendorId;
            stock.LocationId = model.LocationId;
            stock.RequestedOn = ProjectConvert.ConverDateStringtoDatetime(model.RequestedOn);
            stock.TotalNotes = model.TotalNotes;

            stock.IsActive = true;
            if (model.VendorStockRequestId == 0)
            {
                stock.CreatedBy = User.Identity.Name;
                stock.CreatedOn = DateTime.Now;
                stock.ModifiedOn = DateTime.Now;
                myapp.tbl_cm_VendorMealRequest.Add(stock);
            }
            myapp.SaveChanges();
            if (model.VendorStockRequestId == 0)
                return Json(stock.VendorStockRequestId, JsonRequestBehavior.AllowGet);
            else
                return Json(model.VendorStockRequestId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save_UpdateMealRequestMealTypeList(List<tbl_cm_VendorMealRequestMealType> model)
        {

            for (int i = 0; i < model.Count; i++)
            {
                tbl_cm_VendorMealRequestMealType meal = new tbl_cm_VendorMealRequestMealType();
                if (model[i].VendorStockRequestMealId != 0)
                {
                    var id = model[i].VendorStockRequestMealId;
                    meal = myapp.tbl_cm_VendorMealRequestMealType.Where(m => m.VendorStockRequestMealId == id).FirstOrDefault();
                }
                meal.MealTypeId = model[i].MealTypeId;
                meal.QtyRequested = model[i].QtyRequested;
                meal.VendorStockRequestId = model[i].VendorStockRequestId;
                if (model[i].VendorStockRequestMealId == 0)
                {
                    myapp.tbl_cm_VendorMealRequestMealType.Add(meal);
                }
                myapp.SaveChanges();

            }
            sendVendorRequestEmail(model);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_VendorMealRequest_ById(int id)
        {
            var model = myapp.tbl_cm_VendorMealRequest.Where(m => m.VendorStockRequestId == id).FirstOrDefault();
            VendorMealRequestModel stock = new VendorMealRequestModel();
            stock.VendorId = model.VendorId;
            stock.LocationId = model.LocationId;
            stock.VendorName = (from V in myapp.tbl_Vendor where V.VendorId == model.VendorId select V.Name).SingleOrDefault();
            stock.LocationName = (from V in myapp.tbl_Location where V.LocationId == model.LocationId select V.LocationName).SingleOrDefault();
            stock.TotalNotes = model.TotalNotes;
            stock.RequestedOn = model.RequestedOn.HasValue ? model.RequestedOn.Value.ToString("dd/MM/yyyy") : "[N/A]";
            return Json(stock, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_VendorMealTypeMeal_ByResId(int id)
        {
            var viewmode = myapp.tbl_cm_VendorMealRequestMealType.Where(m => m.VendorStockRequestId == id).ToList();
            return Json(viewmode, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetVendorMealReceived(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cm_VendorMealRequest where d.ReceivedOnDate != null select d).ToList();
            IEnumerable<tbl_cm_VendorMealRequest> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.VendorStockRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.VendorId != null && c.VendorId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                  ||
                              c.ReceivedOnDate != null && c.ReceivedOnDate.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                                              c.VendorStockRequestId.ToString(),
                                              (from V in myapp.tbl_Vendor where V.VendorId == c.VendorId select V.Name).SingleOrDefault(),
                                              (from V in myapp.tbl_Location where V.LocationId == c.LocationId select V.LocationName).SingleOrDefault(),
                                              c.ReceivedOnDate.HasValue?c.ReceivedOnDate.Value.ToString("dd/MM/yyyy"):"",
                                            c.TotalNotes,
                                              c.IsActive.ToString(),
                                              c.VendorStockRequestId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save_UpdateCMSMealRequestReceived(VendorMealRequestModel model)
        {
            tbl_cm_VendorMealRequest stock = new tbl_cm_VendorMealRequest();
            if (model.VendorStockRequestId != 0)
            {
                stock = myapp.tbl_cm_VendorMealRequest.Where(m => m.VendorStockRequestId == model.VendorStockRequestId).FirstOrDefault();
                stock.ModifiedBy = User.Identity.Name;
                stock.ModifiedOn = DateTime.Now;
            }
            else
            {
                stock = myapp.tbl_cm_VendorMealRequest.Where(m => m.VendorId == model.VendorId && m.LocationId == model.LocationId).Take(1).FirstOrDefault();
            }
            if (stock != null)
            {
                stock.VendorId = model.VendorId;
                stock.LocationId = model.LocationId;
                stock.ReceivedOnDate = ProjectConvert.ConverDateStringtoDatetime(model.ReceivedOnDate);
                stock.TotalNotes = model.TotalNotes;
                myapp.SaveChanges();
                return Json(stock.VendorStockRequestId, JsonRequestBehavior.AllowGet);
            }

            return Json(0, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Save_UpdateMealReceivedList(List<tbl_cm_VendorMealRequestMealType> model)
        {

            for (int i = 0; i < model.Count; i++)
            {
                tbl_cm_VendorMealRequestMealType meal = new tbl_cm_VendorMealRequestMealType();
                if (model[i].VendorStockRequestMealId != 0)
                {
                    var id = model[i].VendorStockRequestMealId;
                    meal = myapp.tbl_cm_VendorMealRequestMealType.Where(m => m.VendorStockRequestMealId == id).FirstOrDefault();
                }
                if (meal != null)
                {
                    meal.QtyReceived = model[i].QtyReceived;
                    meal.QtyDamage = model[i].QtyDamage;
                    meal.QtyReturn = model[i].QtyReturn;
                    meal.Notes = model[i].Notes;

                }
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMSVendorMealAll(JQueryDataTableParamModel param)
        {
            DateTime dttoday = DateTime.Now.Date;
            var query = (from d in myapp.tbl_cm_VendorMeal select d).OrderByDescending(l => l.VendorStockMealId).ToList();

            IEnumerable<tbl_cm_VendorMeal> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.VendorStockMealId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.MealTypeId != null && c.MealTypeId.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.EmpId != null && c.EmpId.ToString().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join u in myapp.tbl_User on c.EmpId equals u.EmpId
                         join u2 in myapp.tbl_User on c.CreatedBy equals u2.CustomUserId
                         join mt in myapp.tbl_cm_MealType on c.MealTypeId equals mt.MealTypeId
                         let Name = u.FirstName + " " + u.LastName
                         select new object[] {
                                              c.VendorStockMealId.ToString(),
                                             Name,
                                             mt.MealTypeName,
                                              c.Qty.ToString(),
                                            c.Notes,
                                              c.IsActive.ToString(),
                                              c.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                              u2.FirstName+" "+u2.LastName,
                                             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMSVendorMeal(JQueryDataTableParamModel param)
        {
            DateTime dttoday = DateTime.Now.Date;
            var query = (from d in myapp.tbl_cm_VendorMeal where d.CreatedOn == dttoday select d).ToList();
            IEnumerable<tbl_cm_VendorMeal> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.VendorStockRequestId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.MealTypeId != null && c.MealTypeId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.EmpId != null && c.EmpId.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join u in myapp.tbl_User on c.EmpId equals u.EmpId
                         join mt in myapp.tbl_cm_MealType on c.MealTypeId equals mt.MealTypeId
                         let Name = u.FirstName + " " + u.LastName
                         select new object[] {
                                              c.VendorStockMealId.ToString(),
                                             Name,
                                             mt.MealTypeName,
                                              c.Qty.ToString(),
                                            c.Notes,
                                              c.IsActive.ToString(),
                                              c.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                              c.CreatedBy,
                                             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveVendorMeal(tbl_cm_VendorMeal model)
        {
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            model.ModifiedOn = DateTime.Now;
            myapp.tbl_cm_VendorMeal.Add(model);
            myapp.SaveChanges();
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveEmpVendorMeal(tbl_cm_VendorMeal model)
        {
            var user = myapp.tbl_User.Where(u => u.EmpId == model.EmpId && u.IsActive == true).FirstOrDefault();
            if (user != null)
            {
                model.CreatedBy = User.Identity.Name;
                model.CreatedOn = DateTime.Now.Date;
                model.ModifiedOn = DateTime.Now;
                model.EmpId = model.EmpId;
                var time = DateTime.Now.TimeOfDay;
                if (time > new TimeSpan(06, 00, 00) && time < new TimeSpan(10, 00, 00))
                {
                    model.MealTypeId = myapp.tbl_MealType.Where(m => m.Name == "Break Fast").Select(n => n.MealTypeId).FirstOrDefault();
                }
                else if (time > new TimeSpan(11, 00, 00) && time < new TimeSpan(16, 00, 00))
                {
                    model.MealTypeId = myapp.tbl_MealType.Where(m => m.Name == "Lunch").Select(n => n.MealTypeId).FirstOrDefault();

                }
                else if (time > new TimeSpan(17, 00, 00) && time < new TimeSpan(22, 00, 00))
                {
                    model.MealTypeId = myapp.tbl_MealType.Where(m => m.Name == "Dinner").Select(n => n.MealTypeId).FirstOrDefault();
                }

                var checkdetails = myapp.tbl_cm_VendorMeal.Where(l => l.CreatedOn == model.CreatedOn && l.EmpId == model.EmpId && l.MealTypeId == model.MealTypeId).Count();
                if (checkdetails == 0)
                {
                    myapp.tbl_cm_VendorMeal.Add(model);
                    myapp.SaveChanges();
                    return Json("Successfully Saved", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Employee already taken food", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Invalid User Please try again", JsonRequestBehavior.AllowGet);
        }
        public void sendVendorRequestEmail(List<tbl_cm_VendorMealRequestMealType> model)
        {
            string body = string.Empty;
            var id = model[0].VendorStockRequestId;
            tbl_cm_VendorMealRequest req = myapp.tbl_cm_VendorMealRequest.Where(l => l.VendorStockRequestId == id).SingleOrDefault();
            tbl_Vendor vendor = myapp.tbl_Vendor.Where(v => v.VendorId == req.VendorId).SingleOrDefault();
            tbl_Location location = myapp.tbl_Location.Where(v => v.LocationId == req.LocationId).SingleOrDefault();
            using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/VendorPlaceRequestTemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            var mealTable = "<table style='border:1px solid #eee;width: 60%;'>";
            mealTable = "<thead><th style='border:1px solid #eee;font-family:Segoe UI;'>Meal Type</th><th>Qty</th></thead>";
            for (int i = 0; i < model.Count; i++)
            {
                var mealId = model[i].MealTypeId;
                var meal = (from V in myapp.tbl_MealType where V.MealTypeId == mealId select V.Name).SingleOrDefault();

                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>" + meal + "</td>";
                mealTable += "<td style='border:1px solid #eee;font-family:Segoe UI'>" + model[i].QtyRequested + "</td></tr>";
            }

            mealTable += "</table>";
            body = body.Replace("{Location}", location.LocationName);
            body = body.Replace("{VendorName}", vendor.Name);
            body = body.Replace("{RequestedOn}", (req.RequestedOn != null) ? Convert.ToDateTime(req.RequestedOn).ToString("dd/MM/yyyy") : "");
            body = body.Replace("{Notes}", req.TotalNotes);
            body = body.Replace("{table}", mealTable);
            string Subject = "";
            Subject = "A Meal request ID " + req.VendorStockRequestId + "";
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel
            {
                fromemail = "Leave@hospitals.com",
                toemail = "phanisrinivas111@gmail.com",
                subject = Subject,
                body = body,
                filepath = "",
                fromname = "",
                ccemail = ""
            };
            cm.SendEmail(mailmodel);
        }
        public ActionResult AjaxGetMealReport(JQueryDataTableParamModel param)
        {

            var query = GetReportData();
            IEnumerable<ReportMealsUsedModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.LocationName != null && c.LocationName.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                          "",
                          "",
                                            "",
                                            "",
                                            "",
                                            "",
                                            "",
                                            "",
                                            ""
                                             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        private List<ReportMealsUsedModel> GetReportData()
        {
            var list = new List<ReportMealsUsedModel>();
            var vendorMealRequest = myapp.tbl_cm_VendorMealRequest.ToList();
            var vendorMealRequestMeal = myapp.tbl_cm_VendorMealRequestMealType.ToList();
            var vendorMeal = myapp.tbl_cm_VendorMeal.ToList();
            return list;
        }

        // Payments
        public ActionResult SavePayment(tbl_cm_Payment model)
        {
            model.PaymentDate = DateTime.Now;
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myapp.tbl_cm_Payment.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetMyPayment(JQueryDataTableParamModel param)
        {
            string empid = User.Identity.Name;
            var id = (from u in myapp.tbl_User where u.CustomUserId == empid select u.UserId).SingleOrDefault();
            var query = (from d in myapp.tbl_cm_Payment where d.PaidByEmpId == id select d).ToList();
            IEnumerable<tbl_cm_Payment> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PaymentId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.CanteenId != null && c.CanteenId.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.PaidByEmpId != null && c.PaidByEmpId.ToString().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join u in myapp.tbl_cm_Canteen on c.CanteenId equals u.CanteenId
                         join us1 in myapp.tbl_User on c.PaidByEmpId equals us1.UserId
                         join us2 in myapp.tbl_User on c.ReceivedByEmpId equals us2.UserId
                         select new object[] {
                                              c.PaymentId.ToString(),
                             c.PaymentDate.Value.ToString("dd/MM/yyyy"),
                                            u.CanteenName,
                                            c.TotalAmount.ToString(),
                                            c.ReceivedAmount.ToString(),
                                            us1.EmpId,
                                            us1.FirstName,
                                             us2.EmpId,
                                            us2.FirstName,
                                            c.Remarks
                                             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetPayment(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_cm_Payment select d).ToList();
            IEnumerable<tbl_cm_Payment> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PaymentId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.CanteenId != null && c.CanteenId.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.PaidByEmpId != null && c.PaidByEmpId.ToString().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join u in myapp.tbl_cm_Store on c.CanteenId equals u.StoreId
                         join us1 in myapp.tbl_User on c.PaidByEmpId equals us1.UserId
                         join us2 in myapp.tbl_User on c.ReceivedByEmpId equals us2.UserId
                         select new object[] {
                                              c.PaymentId.ToString(),
                             c.PaymentDate.Value.ToString("dd/MM/yyyy"),
                                            u.StoreName,
                                            c.TotalAmount.ToString(),
                                            c.ReceivedAmount.ToString(),
                                            us1.EmpId,
                                            us1.FirstName,
                                             us2.EmpId,
                                            us2.FirstName,
                                            c.Remarks,
                                              c.PaymentId.ToString()
                                             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetMySales(JQueryDataTableParamModel param)
        {
            string empid = User.Identity.Name;
            var userId = (from V in myapp.tbl_User where V.CustomUserId == empid select V.UserId).SingleOrDefault();

            var query = (from d in myapp.tbl_cm_Transaction select d).ToList();
            query = query.Where(m => m.SalesEmpId == userId).ToList();
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (param.ModeOfPayment != null && param.ModeOfPayment != "")
            {
                query = query.Where(m => m.ModeOfPayment == param.ModeOfPayment).ToList();
            }
            if (param.FormType != null && param.FormType != "")//SaleType
            {
                query = query.Where(m => m.SaleType == param.FormType).ToList();
            }
            query = query.OrderByDescending(l => l.TransactionId).ToList();
            IEnumerable<tbl_cm_Transaction> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.TransactionId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.CanteenId != null && c.CanteenId.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.SalesEmpId != null && c.SalesEmpId.ToString().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join u in myapp.tbl_cm_Store on c.CanteenId equals u.StoreId
                         join us1 in myapp.tbl_User on c.SalesEmpId equals us1.UserId
                         select new object[] {
                                              c.TransactionId.ToString(),
                             c.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                   u.StoreName,
                                            c.SaleType,
                                            c.TotalPrice.ToString(),
                                            c.DiscountValue.ToString(),
                                            c.FinalPrice.ToString(),
                                            c.TotalPaidAmount.ToString(),
                                            c.EmpName,
                                            c.EmpMobile,
                                            us1.EmpId,
                                            us1.FirstName,
                                          c.ModeOfPayment,
                                              c.TransactionId.ToString()
                                             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetSales(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cm_Transaction select d).ToList();
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (param.ModeOfPayment != null && param.ModeOfPayment != "")
            {
                query = query.Where(m => m.ModeOfPayment == param.ModeOfPayment).ToList();
            }
            if (param.FormType != null && param.FormType != "")//SaleType
            {
                query = query.Where(m => m.SaleType == param.FormType).ToList();
            }
            if (param.Canteen != 0)
            {
                query = query.Where(m => m.CanteenId == param.Canteen).ToList();
            }
            if (param.Emp != null && param.Emp != "")//SaleType
            {
                var id = Convert.ToInt32(param.Emp);
                query = query.Where(m => m.SalesEmpId == id).ToList();
            }
            query = query.OrderByDescending(l => l.TransactionId).ToList();
            IEnumerable<tbl_cm_Transaction> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.TransactionId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.CanteenId != null && c.CanteenId.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.SalesEmpId != null && c.SalesEmpId.ToString().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join u in myapp.tbl_cm_Store on c.CanteenId equals u.StoreId
                         join us1 in myapp.tbl_User on c.SalesEmpId equals us1.UserId
                         select new object[] {
                                              c.TransactionId.ToString(),
                             c.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                            u.StoreName,
                                            c.SaleType,
                                            c.TotalPrice.ToString(),
                                            c.DiscountValue.ToString(),
                                            c.FinalPrice.ToString(),
                                            c.TotalPaidAmount.ToString(),
                                            c.EmpName,
                                            c.EmpMobile,
                                            us1.EmpId,
                                            us1.FirstName,
                                          c.ModeOfPayment,
                                              c.TransactionId.ToString()
                                             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetMyCreditSales(JQueryDataTableParamModel param)
        {
            string empid = User.Identity.Name;
            var userId = (from V in myapp.tbl_User where V.CustomUserId == empid select V.UserId).SingleOrDefault();
            var query = (from d in myapp.tbl_cm_Transaction where d.ModeOfPayment == "Credit" select d).ToList();
            query = query.Where(m => m.SalesEmpId == userId).ToList();
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            query = query.OrderByDescending(l => l.TransactionId).ToList();
            //if (param.ModeOfPayment != null && param.ModeOfPayment != "")
            //{
            //    query = query.Where(m => m.ModeOfPayment == param.ModeOfPayment).ToList();
            //}
            //if (param.FormType != null && param.FormType != "")//SaleType
            //{
            //    query = query.Where(m => m.SaleType == param.FormType).ToList();
            //}
            //if (param.Canteen != 0)
            //{
            //    query = query.Where(m => m.CanteenId == param.Canteen).ToList();
            //}
            //if (param.Emp != null && param.Emp != "")//SaleType
            //{
            //    var id = Convert.ToInt32(param.Emp);
            //    query = query.Where(m => m.SalesEmpId == id).ToList();
            //}
            IEnumerable<tbl_cm_Transaction> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.TransactionId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.CanteenId != null && c.CanteenId.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.SalesEmpId != null && c.SalesEmpId.ToString().Contains(param.sSearch.ToLower())
                              ||
                              c.EmpName != null && c.EmpName.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.EmpMobile != null && c.EmpMobile.ToString().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join u in myapp.tbl_cm_Store on c.CanteenId equals u.StoreId
                         join us1 in myapp.tbl_User on c.SalesEmpId equals us1.UserId
                         select new object[] {
                                              c.TransactionId.ToString(),
                             c.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                            u.StoreName,
                                            c.SaleType,
                                            c.TotalPrice.ToString(),
                                            c.DiscountValue.ToString(),
                                            c.FinalPrice.ToString(),
                                            c.TotalPaidAmount.ToString(),
                                            c.PatientId,
                                            c.InPatientdRoomNo,
                                            c.EmpName,
                                            c.EmpMobile,
                                            us1.EmpId,
                                            us1.FirstName,
                                          c.ModeOfPayment,
                                              c.TransactionId.ToString()
                                             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCreditSales(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_cm_Transaction where d.ModeOfPayment == "Credit" select d).ToList();
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            query = query.OrderByDescending(l => l.TransactionId).ToList();
            //if (param.ModeOfPayment != null && param.ModeOfPayment != "")
            //{
            //    query = query.Where(m => m.ModeOfPayment == param.ModeOfPayment).ToList();
            //}
            //if (param.FormType != null && param.FormType != "")//SaleType
            //{
            //    query = query.Where(m => m.SaleType == param.FormType).ToList();
            //}
            if (param.Canteen != 0)
            {
                query = query.Where(m => m.CanteenId == param.Canteen).ToList();
            }
            if (param.Emp != null && param.Emp != "")//SaleType
            {
                var id = Convert.ToInt32(param.Emp);
                query = query.Where(m => m.SalesEmpId == id).ToList();
            }
            IEnumerable<tbl_cm_Transaction> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.TransactionId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.CanteenId != null && c.CanteenId.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.SalesEmpId != null && c.SalesEmpId.ToString().Contains(param.sSearch.ToLower())
                              ||
                              c.EmpName != null && c.EmpName.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.EmpMobile != null && c.EmpMobile.ToString().Contains(param.sSearch.ToLower())
                              ||
                              c.PatientId != null && c.PatientId.ToString().Contains(param.sSearch.ToLower())
                              ||
                              c.InPatientdRoomNo != null && c.InPatientdRoomNo.ToString().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join u in myapp.tbl_cm_Store on c.CanteenId equals u.StoreId
                         join us1 in myapp.tbl_User on c.SalesEmpId equals us1.UserId
                         select new object[] {
                                              c.TransactionId.ToString(),
                             c.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                            u.StoreName,
                                            c.SaleType,
                                            c.TotalPrice.ToString(),
                                            //c.DiscountValue.ToString(),
                                            //c.FinalPrice.ToString(),
                                            c.TotalPaidAmount.ToString(),
                                            c.PatientId,
                                            c.InPatientdRoomNo,
                                            c.EmpName,
                                            c.EmpMobile,
                                            us1.EmpId,
                                            us1.FirstName,
                                          c.ModeOfPayment,
                                              c.TransactionId.ToString()
                                             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save_UpdateCMSTransaction(tbl_cm_Transaction model)
        {
            tbl_cm_ReceivedStock stock = new tbl_cm_ReceivedStock();

            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            model.ModifiedOn = DateTime.Now;
            model.DiscountType = "Amount";
            model.IsFreeMeal = false;
            if (model.EmpId != 0 && model.SaleType == "Emp")
            {
                model.EmpName = (from V in myapp.tbl_User where V.UserId == model.EmpId select V.FirstName + " " + V.LastName).SingleOrDefault();
                model.EmpMobile = (from V in myapp.tbl_User where V.UserId == model.EmpId select V.PhoneNumber).SingleOrDefault();
                model.EmpEmail = (from V in myapp.tbl_User where V.UserId == model.EmpId select V.EmailId).SingleOrDefault();
            }
            var userId = User.Identity.Name;
            model.SalesEmpId = (from V in myapp.tbl_User where V.CustomUserId == userId select V.UserId).SingleOrDefault();
            myapp.tbl_cm_Transaction.Add(model);
            myapp.SaveChanges();
            try
            {
                string mobilenumber = model.EmpMobile;
                if (model.SaleType == "InPatient")
                {
                    var p = myapp.tbl_Patient.Where(l => l.MRNo == model.PatientId).FirstOrDefault();
                    if (p != null)
                    {
                        mobilenumber = p.MobileNumber;
                    }
                }
                SendSms sms = new SendSms();
                if (mobilenumber != null && mobilenumber != "" && model.ModeOfPayment != "Credit")
                {
                    string message = "Hello! Thank you for ordering from Fernz Cafe. We have received your payment of Rs " + model.TotalPaidAmount + ". Enjoy your healthy meal!";
                    sms.SendSmsToEmployee(mobilenumber, message);
                }
            }
            catch { }
            return Json(model.TransactionId, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCMSTransaction(tbl_cm_Transaction data)
        {
            var model = myapp.tbl_cm_Transaction.Where(m => m.TransactionId == data.TransactionId).FirstOrDefault();
            model.ModeOfPayment = data.ModeOfPayment;
            model.SaleType = data.SaleType;
            model.TransactionCustomerType = data.TransactionCustomerType;
            model.TotalPaidAmount = data.TotalPaidAmount;
            model.FinalPrice = data.FinalPrice;
            model.TotalPrice = data.TotalPrice;
            model.TotalPaidAmount = data.TotalPaidAmount;
            model.DiscountValue = data.DiscountValue;
            model.ModifiedBy = User.Identity.Name;
            model.InPatientdRoomNo = data.InPatientdRoomNo;
            model.PatientId = data.PatientId;
            model.EmpMobile = data.EmpMobile;
            model.EmpName = data.EmpName;
            model.ModifiedOn = DateTime.Now;
            model.DiscountType = "Amount";
            model.IsFreeMeal = false;
            model.NameOfTheCard = data.NameOfTheCard;
            model.CardNumber = data.CardNumber;
            if (model.EmpId != 0 && model.SaleType == "Emp")
            {
                model.EmpName = (from V in myapp.tbl_User where V.UserId == data.EmpId select V.FirstName + " " + V.LastName).SingleOrDefault();
                model.EmpMobile = (from V in myapp.tbl_User where V.UserId == data.EmpId select V.PhoneNumber).SingleOrDefault();
                model.EmpEmail = (from V in myapp.tbl_User where V.UserId == data.EmpId select V.EmailId).SingleOrDefault();
            }
            var userId = User.Identity.Name;
            model.SalesEmpId = (from V in myapp.tbl_User where V.CustomUserId == userId select V.UserId).SingleOrDefault();
            myapp.SaveChanges();
            return Json(model.TransactionId, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult Save_UpdateTransactionItemList(List<tbl_cm_TransactionItem> model)
        {
            for (int i = 0; i < model.Count; i++)
            {
                model[i].IsActive = true;
            }
            myapp.tbl_cm_TransactionItem.AddRange(model);
            myapp.SaveChanges();
            //  GetPrintDetails(model[0].TransactionId.Value);
            var result = new FilePathResult("~/Documents/Bill.html", "text/html");
            return result;
        }
        [HttpPost]
        public ActionResult UpdateTransactionItemList(List<tbl_cm_TransactionItem> model)
        {
            var transactionId = model[0].TransactionId;
            var items = myapp.tbl_cm_TransactionItem.Where(m => m.TransactionId == transactionId).ToList();
            myapp.tbl_cm_TransactionItem.RemoveRange(items);
            myapp.SaveChanges();
            for (int i = 0; i < model.Count; i++)
            {
                model[i].IsActive = true;
            }
            myapp.tbl_cm_TransactionItem.AddRange(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetCMSCurrentStock(JQueryDataTableParamModel param)
        {
            int StoreId = 0;
            if (param.categoryid != null && param.categoryid > 0)
            {
                StoreId = param.categoryid;
            }

            var modelitemtrsn = myapp.tbl_cm_ItemTransfer.Where(l => l.ToStoreId == StoreId).ToList();
            var modelitemrec = myapp.tbl_cm_ReceivedStock.Where(l => l.StoreId == StoreId).ToList();
            var modelitemsale = myapp.tbl_cm_Transaction.Where(l => l.CanteenId == StoreId).ToList();
            List<StockViewModel> finalmodel = new List<StockViewModel>();
            finalmodel.AddRange((from it in modelitemsale
                                 join t in myapp.tbl_cm_TransactionItem on it.TransactionId equals t.TransactionId
                                 join itm in myapp.tbl_cm_Item on t.ItemId equals itm.ItemId
                                 select new StockViewModel
                                 {
                                     ItemId = itm.ItemId,
                                     ItemName = itm.ItemName,
                                     TotalSales = t.Qty.HasValue ? t.Qty.Value : 0,
                                     Balance = 0,
                                     TotalReceived = 0
                                 }).ToList());

            finalmodel.AddRange((from it in modelitemtrsn
                                 join t in myapp.tbl_cm_ItemTransferItem on it.ItemTransferId equals t.ItemTransferId
                                 join itm in myapp.tbl_cm_Item on t.ItemId equals itm.ItemId
                                 select new StockViewModel
                                 {
                                     ItemId = itm.ItemId,
                                     ItemName = itm.ItemName,
                                     TotalSales = 0,
                                     Balance = 0,
                                     TotalReceived = Convert.ToInt32(t.QtySend.HasValue ? t.QtySend.Value : 0)
                                 }).ToList());
            finalmodel.AddRange((from it in modelitemrec
                                 join t in myapp.tbl_cm_ReceivedStockItem on it.ReceivedStockId equals t.ReceivedStockId
                                 join itm in myapp.tbl_cm_Item on t.ItemId equals itm.ItemId
                                 select new StockViewModel
                                 {
                                     ItemId = itm.ItemId,
                                     ItemName = itm.ItemName,
                                     TotalSales = 0,
                                     Balance = 0,
                                     TotalReceived = Convert.ToInt32(t.QtyRecived.HasValue ? t.QtyRecived.Value : 0)
                                 }).ToList());
            var tmp = from x in finalmodel
                      group x by x.ItemId;
            finalmodel = (from f in tmp
                          select new StockViewModel
                          {
                              ItemId = f.Key,
                              TotalReceived = f.Sum(x => x.TotalReceived),
                              TotalSales = f.Sum(x => x.TotalSales),
                              Balance = f.Sum(x => x.Balance),
                              ItemName = f.FirstOrDefault().ItemName
                          }).ToList();
            IEnumerable<StockViewModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = finalmodel
                   .Where(c => c.ItemId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.ItemName != null && c.ItemName.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = finalmodel;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                                              c.ItemId.ToString(),
                                              c.ItemName,
                                              c.TotalReceived.ToString(),
                                              c.TotalSales.ToString(),
                                              (c.TotalReceived-c.TotalSales).ToString()
                           };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = finalmodel.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelSales(string FromDate, string ToDate, string role)
        {
            DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(FromDate);
            DateTime todate = ProjectConvert.ConverDateStringtoDatetime(ToDate);
            List<tbl_cm_Store> canteenList = myapp.tbl_cm_Store.Where(m => m.TypeOf == "Canteen").ToList();
            List<tbl_User> userList = myapp.tbl_User.ToList();
            List<tbl_cm_Transaction> query = myapp.tbl_cm_Transaction.ToList();
            query = query.Where(p => p.CreatedOn.Value.Date >= fromdate.Date && p.CreatedOn.Value.Date <= todate.Date).ToList();

            if (role == "User")
            {
                var userId = User.Identity.Name;
                var salesEmpId = (from V in myapp.tbl_User where V.CustomUserId == userId select V.UserId).SingleOrDefault();
                query = query.Where(m => m.SalesEmpId == salesEmpId).ToList();
            }
            var Result = from c in query
                         join u in canteenList on c.CanteenId equals u.StoreId
                         join e in userList on c.SalesEmpId equals e.UserId
                         select new
                         {
                             c,
                             u.StoreName,
                             e.FirstName,
                             e.LastName,
                             e.Designation,
                         };
            System.Data.DataTable products = new System.Data.DataTable("Sales");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Customer Type", typeof(string));
            products.Columns.Add("Employee Name", typeof(string));
            products.Columns.Add("Total Price", typeof(string));
            products.Columns.Add("Mode Of Payment", typeof(string));
            products.Columns.Add("Card Number", typeof(string));
            products.Columns.Add("Name Of The Card", typeof(string));
            products.Columns.Add("Discount", typeof(string));
            products.Columns.Add("Final Price", typeof(string));
            products.Columns.Add("Outer Name", typeof(string));
            products.Columns.Add("Outer Phone No", typeof(string));

            products.Columns.Add("Canteen", typeof(string));
            products.Columns.Add("Sales Employee", typeof(string));
            products.Columns.Add("Total Paid Amount", typeof(string));
            products.Columns.Add("Patient Id", typeof(string));
            products.Columns.Add("Patient Room No", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            foreach (var item in Result)
            {
                products.Rows.Add(
               item.c.TransactionId.ToString(),
                item.c.TransactionCustomerType,
              item.c.EmpName,
                 item.c.TotalPrice != null ? item.c.TotalPrice.ToString() : "0",
            item.c.ModeOfPayment,
            item.c.CardNumber,
             item.c.NameOfTheCard,

item.c.DiscountValue != null ? item.c.DiscountValue.ToString() : "0",
item.c.FinalPrice != null ? item.c.FinalPrice.ToString() : "0",
item.c.EmpName,
item.c.EmpMobile,
item.StoreName,
item.FirstName + " " + item.LastName + " " + item.Designation,
item.c.TotalPaidAmount != null ? item.c.TotalPaidAmount.ToString() : "0",
item.c.PatientId,
item.c.InPatientdRoomNo,
  item.c.CreatedOn.HasValue ? (item.c.CreatedOn.Value.ToString("dd/MM/yyyy")) : ""
                );
            }


            GridView grid = new GridView
            {
                DataSource = products
            };
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=Sales.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportPDFSales(string FromDate, string ToDate, string role, string mode, int Canteen = 0, int emp = 0)
        {
            DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(FromDate);
            DateTime todate = ProjectConvert.ConverDateStringtoDatetime(ToDate);
            List<tbl_cm_Store> canteenList = myapp.tbl_cm_Store.Where(m => m.TypeOf == "Canteen").ToList();
            List<tbl_User> userList = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            List<tbl_cm_Transaction> query = myapp.tbl_cm_Transaction.ToList();
            if (role == "User")
            {
                var userId = User.Identity.Name;
                var salesEmpId = (from V in userList where V.CustomUserId == userId select V.UserId).SingleOrDefault();
                query = query.Where(m => m.SalesEmpId == salesEmpId).ToList();
            }
            if (mode == "Credit")
            {

                query = query.Where(m => m.ModeOfPayment == "Credit").ToList();
            }
            if (Canteen != 0)
            {
                query = query.Where(m => m.CanteenId == Canteen).ToList();
            }
            if (emp != 0)
            {
                query = query.Where(m => m.SalesEmpId == emp).ToList();
            }
            query = query.Where(p => p.CreatedOn.Value.Date >= fromdate.Date && p.CreatedOn.Value.Date <= todate.Date).ToList();
            var Result = from c in query
                         join u in canteenList on c.CanteenId equals u.StoreId
                         join e in userList on c.SalesEmpId equals e.UserId
                         select new
                         {
                             c,
                             u.StoreName,
                             e.FirstName,
                             e.LastName,
                             e.Designation,
                         };
            System.Data.DataTable products = new System.Data.DataTable("Sales");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Customer Type", typeof(string));
            products.Columns.Add("Employee Name", typeof(string));
            products.Columns.Add("Total Price", typeof(string));
            products.Columns.Add("Mode Of Payment", typeof(string));
            products.Columns.Add("Card Number", typeof(string));
            products.Columns.Add("Name Of The Card", typeof(string));
            products.Columns.Add("Discount", typeof(string));
            products.Columns.Add("Final Price", typeof(string));
            products.Columns.Add("Outer Name", typeof(string));
            products.Columns.Add("Outer Phone No", typeof(string));

            products.Columns.Add("Canteen", typeof(string));
            products.Columns.Add("Sales Employee", typeof(string));
            products.Columns.Add("Total Paid Amount", typeof(string));
            products.Columns.Add("Patient Id", typeof(string));
            products.Columns.Add("Patient Room No", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            foreach (var item in Result)
            {
                products.Rows.Add(
               item.c.TransactionId.ToString(),
                item.c.TransactionCustomerType,
              item.c.EmpName,
                 item.c.TotalPrice != null ? item.c.TotalPrice.ToString() : "0",
            item.c.ModeOfPayment,
            item.c.CardNumber,
             item.c.NameOfTheCard,

item.c.DiscountValue != null ? item.c.DiscountValue.ToString() : "0",
item.c.FinalPrice != null ? item.c.FinalPrice.ToString() : "0",
item.c.EmpName,
item.c.EmpMobile,
item.StoreName,
item.FirstName + " " + item.LastName + " " + item.Designation,
item.c.TotalPaidAmount != null ? item.c.TotalPaidAmount.ToString() : "0",
item.c.PatientId,
item.c.InPatientdRoomNo,
  item.c.CreatedOn.HasValue ? (item.c.CreatedOn.Value.ToString("dd/MM/yyyy")) : ""
                );
            }
            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();
            string text = "Sales";
            Paragraph paragraph = new Paragraph();
            paragraph.SpacingBefore = 10;
            paragraph.SpacingAfter = 10;
            paragraph.Alignment = Element.ALIGN_CENTER;
            paragraph.Font = FontFactory.GetFont(FontFactory.HELVETICA, 12f, BaseColor.RED);
            paragraph.Add(text);
            document.Add(paragraph);
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES_BOLD, 8, BaseColor.BLUE);

            PdfPTable table2 = new PdfPTable(2);
            PdfPRow row2 = null;

            //table2.DefaultCell.Border = 1;
            //table2.HorizontalAlignment = 2;

            table2.SpacingBefore = 2;
            table2.WidthPercentage = 40;
            PdfPCell cell2 = new PdfPCell(new Phrase("Total"));

            table2.AddCell(new Phrase("Payment Type", myFont));
            table2.AddCell(new Phrase("Total Amount " + (Result.Select(t => t.c.TotalPaidAmount ?? 0).Sum()).ToString(), myFont));
            table2.AddCell(new Phrase("Cash", font5));
            table2.AddCell(new Phrase((Result.Where(m => m.c.ModeOfPayment == "Cash").Select(t => t.c.TotalPaidAmount ?? 0).Sum()).ToString(), font5));

            table2.AddCell(new Phrase("Card", font5));
            table2.AddCell(new Phrase((Result.Where(m => m.c.ModeOfPayment == "Card").Select(t => t.c.TotalPaidAmount ?? 0).Sum()).ToString(), font5));

            table2.AddCell(new Phrase("UPI", font5));
            table2.AddCell(new Phrase((Result.Where(m => m.c.ModeOfPayment == "UPI").Select(t => t.c.TotalPaidAmount ?? 0).Sum()).ToString(), font5));

            table2.AddCell(new Phrase("Criedt", font5));
            table2.AddCell(new Phrase((Result.Where(m => m.c.ModeOfPayment == "Credit").Select(t => t.c.TotalPaidAmount ?? 0).Sum()).ToString(), font5));

            PdfPTable table3 = new PdfPTable(2);
            PdfPRow row3 = null;

            //table2.DefaultCell.Border = 1;
            //table2.HorizontalAlignment = 2;

            table3.SpacingBefore = 2;
            table3.WidthPercentage = 40;
            PdfPCell cell3 = new PdfPCell(new Phrase("Total"));

            table3.AddCell(new Phrase("Sales Type ", myFont));
            table3.AddCell(new Phrase("Total Amount " + (Result.Select(t => t.c.TotalPaidAmount ?? 0).Sum()).ToString(), myFont));
            table3.AddCell(new Phrase("Staff", font5));
            table3.AddCell(new Phrase((Result.Where(m => m.c.SaleType == "Emp").Select(t => t.c.TotalPaidAmount ?? 0).Sum()).ToString(), font5));

            table3.AddCell(new Phrase("Patient", font5));
            table3.AddCell(new Phrase((Result.Where(m => m.c.SaleType == "InPatient").Select(t => t.c.TotalPaidAmount ?? 0).Sum()).ToString(), font5));

            table3.AddCell(new Phrase("Outer", font5));
            table3.AddCell(new Phrase((Result.Where(m => m.c.SaleType == "Outer").Select(t => t.c.TotalPaidAmount ?? 0).Sum()).ToString(), font5));




            PdfPTable outer = new PdfPTable(2);

            outer.AddCell(table2);

            outer.AddCell(table3);

            document.Add(outer);
            PdfPTable table = new PdfPTable(products.Columns.Count);
            PdfPRow row = null;

            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));

            cell.Colspan = products.Columns.Count;

            foreach (DataColumn c in products.Columns)
            {
                table.AddCell(new Phrase(c.ColumnName, myFont));
            }
            int i = products.Columns.Count;
            foreach (DataRow r in products.Rows)
            {
                if (products.Rows.Count > 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        table.AddCell(new Phrase(r[j].ToString(), font5));
                    }
                }
            }
            document.Add(table);
            document.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename= Sales.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateStockfromserver(string ponumber, string locid = "")
        {
            OracleInterface.ConnectOracle oracle = new OracleInterface.ConnectOracle();
            var modal = oracle.UpdateStock(ponumber);
            return Json(modal, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult GetInPatientdSearch(string searchTerm)
        {
            var query = myapp.tbl_Patient.ToList();
            query = query
                    .Where(c => c.MRNo != null && c.MRNo.ToLower().Contains(searchTerm.ToLower())
                                ||
                                 c.Name != null && c.Name.ToLower().Contains(searchTerm.ToLower())
                                ||
                               c.IPNo != null && c.IPNo.ToLower().Contains(searchTerm.ToLower())

                               ).ToList();
            var resulst = (from q in query
                           select new
                           {
                               id = q.MRNo,
                               text = q.MRNo + " " + q.Name
                           }).ToList();
            return Json(resulst, JsonRequestBehavior.AllowGet);
        }


        #region CanteenEmployee
        public ActionResult AjaxGetCMSCanteenEmployee(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_cm_StoreEmp select d).ToList();
            IEnumerable<tbl_cm_StoreEmp> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.StoreEmpId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.EmpId != null && c.EmpId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.StoreId != null && c.StoreId.ToString().ToLower().Contains(param.sSearch.ToLower()));
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new[] {
                                              c.StoreEmpId.ToString(),
                                                          (from V in myapp.tbl_User where V.UserId == c.EmpId select V.FirstName + " " + V.LastName).SingleOrDefault(),
                                                                                                        (from V in myapp.tbl_cm_Store where V.StoreId == c.StoreId select V.StoreName).SingleOrDefault(),
                                              //c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.StoreEmpId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Save_UpdateCMSCanteenEmployee(tbl_cm_StoreEmp model)
        {
            tbl_cm_StoreEmp _Category = new tbl_cm_StoreEmp();
            if (model.StoreEmpId != 0)
            {
                var id = model.StoreEmpId;
                _Category = myapp.tbl_cm_StoreEmp.Where(m => m.StoreEmpId == id).FirstOrDefault();
                _Category.ModifiedBy = User.Identity.Name;
                _Category.ModifiedOn = DateTime.Now;
            }
            _Category.EmpId = model.EmpId;
            _Category.StoreId = model.StoreId;
            _Category.IsActive = true;
            if (model.StoreEmpId == 0)
            {
                _Category.CreatedBy = User.Identity.Name;
                _Category.CreatedOn = DateTime.Now;
                myapp.tbl_cm_StoreEmp.Add(_Category);
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCMCanteenEmployee(int id)
        {
            var cat = myapp.tbl_cm_StoreEmp.Where(l => l.StoreEmpId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_cm_StoreEmp.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCanteenEmployee(int id)
        {
            var query = myapp.tbl_cm_StoreEmp.Where(m => m.StoreEmpId == id).FirstOrDefault();
            var Emp = (from V in myapp.tbl_User where V.UserId == query.EmpId select V.FirstName + " " + V.LastName + " - " + V.Designation + " - " + V.DepartmentName).SingleOrDefault();
            object obj = new { model = query, Emp = Emp };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_PatitentNamebyMRno(string id)
        {
            var query = myapp.tbl_Patient.Where(m => m.MRNo == id).FirstOrDefault();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetTotalAmount(string role, string fromDate = null, string toDate = null, int Canteen = 0, int emp = 0)
        {

            List<tbl_cm_Transaction> query = myapp.tbl_cm_Transaction.ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                DateTime todate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                //if (fromdate == todate)
                //{
                //    todate = todate.AddDays(1);
                //}
                query = query.Where(p => p.CreatedOn.Value.Date >= fromdate.Date && p.CreatedOn.Value.Date <= todate.Date).ToList();

            }
            if (role == "User")
            {
                var userId = User.Identity.Name;
                var salesEmpId = (from V in myapp.tbl_User where V.CustomUserId == userId select V.UserId).SingleOrDefault();
                query = query.Where(m => m.SalesEmpId == salesEmpId).ToList();
            }
            else
            {
                if (Canteen != 0)
                {
                    query = query.Where(m => m.CanteenId == Canteen).ToList();
                }
                if (emp != 0)
                {
                    query = query.Where(m => m.SalesEmpId == emp).ToList();
                }
            }

            var totalAmount = query.Select(t => t.TotalPaidAmount ?? 0).Sum();
            var cashAmount = query.Where(m => m.ModeOfPayment == "Cash").Select(t => t.TotalPaidAmount ?? 0).Sum();
            var CardAmount = query.Where(m => m.ModeOfPayment == "Card").Select(t => t.TotalPaidAmount ?? 0).Sum();
            var uPIAmount = query.Where(m => m.ModeOfPayment == "UPI").Select(t => t.TotalPaidAmount ?? 0).Sum();
            var CreditAmount = query.Where(m => m.ModeOfPayment == "Credit").Select(t => t.TotalPaidAmount ?? 0).Sum();
            var StaffAmount = query.Where(m => m.SaleType == "Emp").Select(t => t.TotalPaidAmount ?? 0).Sum();
            var outerAmount = query.Where(m => m.SaleType == "Outer").Select(t => t.TotalPaidAmount ?? 0).Sum();
            var InpatientAmount = query.Where(m => m.SaleType == "InPatient").Select(t => t.TotalPaidAmount ?? 0).Sum();
            var result = new { totalAmount = totalAmount, cashAmount = cashAmount, CardAmount = CardAmount, uPIAmount = uPIAmount, CreditAmount = CreditAmount, StaffAmount = StaffAmount, outerAmount = outerAmount, InpatientAmount = InpatientAmount };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCaptureSales(int id)
        {
            var items = myapp.tbl_cm_TransactionItem.Where(m => m.TransactionId == id).ToList();
            var Emp = "";
            var patient = "";
            var query = myapp.tbl_cm_Transaction.Where(m => m.TransactionId == id).FirstOrDefault();
            if (query != null)
            {
                if (query.EmpId != null && query.EmpId != 0)
                {
                    Emp = (from V in myapp.tbl_User where V.UserId == query.EmpId select V.FirstName + " " + V.LastName + " - " + V.Designation + " - " + V.DepartmentName).SingleOrDefault();
                }

                if (query.PatientId != null && query.PatientId != "")
                {
                    patient = (from V in myapp.tbl_Patient where V.MRNo == query.PatientId select V.MRNo + " " + V.Name).FirstOrDefault();

                }
            }
            object obj = new { transactions = query, Emp = Emp, items = items, patient = patient };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCMS_StoreByEmpId()
        {
            var user = User.Identity.Name;
            int userId = ((from V in myapp.tbl_User where V.CustomUserId == user select V.UserId).SingleOrDefault());
            var empStore = myapp.tbl_cm_StoreEmp.Where(m => m.EmpId == userId).SingleOrDefault();
            var canteenId = empStore != null ? empStore.StoreId : 0;
            var CanteenName = "";
            if (canteenId != null || canteenId != 0)
            {
                CanteenName = (from V in myapp.tbl_cm_Store where V.StoreId == canteenId select V.StoreName).SingleOrDefault();

            }
            object obj = new { canteenId = canteenId, CanteenName = CanteenName };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        #endregion
        public JsonResult GetItemsbySalesId(int id)
        {
            var displayedCompanies = myapp.tbl_cm_TransactionItem.Where(l => l.TransactionId == id).ToList();

            var result = from c in displayedCompanies
                         select new[] {
                                              c.Id.ToString(),
                                                          (from V in myapp.tbl_cm_Item where V.ItemId == c.ItemId select V.ItemName).SingleOrDefault(),
                                                          c.Qty.ToString(),
                                                          c.FinalAmount.ToString()
                               };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetItemsforSalesReport(int id, string fromDate, string toDate, int canteenid)
        {
            var details = (from tranitm in myapp.tbl_cm_TransactionItem
                           join trn in myapp.tbl_cm_Transaction on tranitm.TransactionId equals trn.TransactionId
                           join it in myapp.tbl_cm_Item on tranitm.ItemId equals it.ItemId
                           join ts in myapp.tbl_cm_Store on trn.CanteenId equals ts.StoreId
                           join tus in myapp.tbl_User on trn.CreatedBy equals tus.CustomUserId
                           let UserName = tus.FirstName + " " + tus.LastName
                           select new
                           {
                               trn.SalesEmpId,
                               UserName,
                               ts.StoreName,
                               ts.StoreId,
                               it.ItemName,
                               tranitm.Qty,
                               tranitm.FinalAmount,
                               trn.CreatedOn,
                               it.ItemId
                           }).ToList();
            if (canteenid != null && canteenid > 0)
            {
                details = details.Where(l => l.StoreId == canteenid).ToList();
            }
            if (id != null && id > 0)
            {
                details = details.Where(l => l.ItemId == id).ToList();
            }
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                DateTime todate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                if (fromdate == todate)
                {
                    todate = todate.AddDays(1);
                }
                details = details.Where(l => l.CreatedOn >= fromdate && l.CreatedOn <= todate).ToList();
            }

            var result = (from t in details
                          group t by new { t.StoreId, t.ItemName, t.StoreName, t.UserName, t.SalesEmpId, t.ItemId }
                        into grp
                          select new
                          {
                              grp.Key.StoreId,
                              grp.Key.ItemName,
                              grp.Key.StoreName,
                              grp.Key.UserName,
                              grp.Key.SalesEmpId,
                              grp.Key.ItemId,
                              Qty = grp.Sum(t => t.Qty),
                              FinalAmount = grp.Sum(t => t.FinalAmount)
                          }).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteSales(int id)
        {
            var subdetails = myapp.tbl_cm_TransactionItem.Where(l => l.TransactionId == id).ToList();
            myapp.tbl_cm_TransactionItem.RemoveRange(subdetails);
            myapp.SaveChanges();
            var details = myapp.tbl_cm_Transaction.Where(l => l.TransactionId == id).ToList();
            myapp.tbl_cm_Transaction.RemoveRange(details);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPDFItemsforSalesReport(string fromDate, string toDate, int canteenid)
        {
            var details = (from tranitm in myapp.tbl_cm_TransactionItem
                           join trn in myapp.tbl_cm_Transaction on tranitm.TransactionId equals trn.TransactionId
                           join it in myapp.tbl_cm_Item on tranitm.ItemId equals it.ItemId
                           join ts in myapp.tbl_cm_Store on trn.CanteenId equals ts.StoreId
                           join tus in myapp.tbl_User on trn.CreatedBy equals tus.CustomUserId
                           let UserName = tus.FirstName + " " + tus.LastName
                           select new
                           {
                               trn.SalesEmpId,
                               UserName,
                               ts.StoreName,
                               ts.StoreId,
                               it.ItemName,
                               tranitm.Qty,
                               tranitm.FinalAmount,
                               trn.CreatedOn
                           }).ToList();
            if (canteenid != null && canteenid > 0)
            {
                details = details.Where(l => l.StoreId == canteenid).ToList();
            }
            Document document = new Document();
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);
            document.Open();
            string text = "Sales";
            text = "Sales - From Date :" + fromDate + "  To Date :" + toDate;
            Paragraph paragraph1 = new Paragraph();
            paragraph1.SpacingBefore = 10;
            paragraph1.SpacingAfter = 10;
            paragraph1.Alignment = Element.ALIGN_CENTER;
            paragraph1.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 12f, BaseColor.DARK_GRAY);
            paragraph1.Add(text);
            document.Add(paragraph1);
            PdfPTable outer = new PdfPTable(2);
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                DateTime fromdate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                DateTime todate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                if (fromdate == todate)
                {
                    todate = todate.AddDays(1);
                }
                details = details.Where(l => l.CreatedOn >= fromdate && l.CreatedOn <= todate).ToList();

                for (DateTime counter = fromdate; counter <= todate; counter = counter.AddDays(1))
                {

                    var result = (from t in details
                                  where t.CreatedOn.Value.ToString("dd/MM/yyyy") == counter.ToString("dd/MM/yyyy")
                                  group t by new { t.StoreId, t.ItemName, t.StoreName, t.UserName, t.SalesEmpId }
                                into grp
                                  select new
                                  {
                                      grp.Key.StoreId,
                                      grp.Key.ItemName,
                                      grp.Key.StoreName,
                                      grp.Key.UserName,
                                      grp.Key.SalesEmpId,
                                      Qty = grp.Sum(t => t.Qty),
                                      FinalAmount = grp.Sum(t => t.FinalAmount)
                                  }).ToList();
                    if (result.Count != 0)
                    {
                        System.Data.DataTable products = new System.Data.DataTable("itemsalesreport");
                        products.Columns.Add("Store", typeof(string));
                        products.Columns.Add("User Name", typeof(string));
                        products.Columns.Add("Item", typeof(string));
                        products.Columns.Add("Qty", typeof(string));
                        products.Columns.Add("Rate", typeof(string));
                        foreach (var item in result)
                        {
                            products.Rows.Add(
                                item.StoreName,
                                item.UserName,
                                item.ItemName, item.Qty, item.FinalAmount
                            );
                        }

                        //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                        //  
                        text = "Sales no :" + counter.ToString("MMMM,dd,yyyy");
                        Paragraph paragraph2 = new Paragraph();
                        paragraph2.SpacingBefore = 10;
                        paragraph2.SpacingAfter = 10;
                        paragraph2.Alignment = Element.ALIGN_CENTER;
                        paragraph2.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 12f, BaseColor.DARK_GRAY);
                        paragraph2.Add(text);
                        document.Add(paragraph2);

                        PdfPTable table = new PdfPTable(products.Columns.Count);
                        table.WidthPercentage = 100;
                        PdfPCell cell = new PdfPCell(new Phrase("Products"));
                        iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);
                        iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES_BOLD, 8, BaseColor.BLUE);
                        cell.Colspan = products.Columns.Count;

                        foreach (DataColumn c in products.Columns)
                        {
                            table.AddCell(new Phrase(c.ColumnName, myFont));
                        }
                        int i = products.Columns.Count;
                        foreach (DataRow r in products.Rows)
                        {
                            if (products.Rows.Count > 0)
                            {
                                for (int j = 0; j < i; j++)
                                {
                                    table.AddCell(new Phrase(r[j].ToString(), font5));
                                }
                            }
                        }


                        document.Add(table);

                    }

                }

                document.Close();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment; filename= itemsalesreport.pdf");
                Response.End();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult EmployeeOrder()
        {
            return View();
        }
        public ActionResult MyOrder()
        {
            return View();
        }
        public ActionResult Save_Update_MyOrder(Cm_EmployeeOrderViewModel model)
        {
            string userid = User.Identity.Name;
            var user = myapp.tbl_User.Where(l => l.CustomUserId == userid).SingleOrDefault();
            tbl_cm_EmpOrders dbmodel = new tbl_cm_EmpOrders();
            if (model.OrderId != 0)
            {
                dbmodel = myapp.tbl_cm_EmpOrders.Where(n => n.OrderId == model.OrderId).FirstOrDefault();
                dbmodel.ModifiedBy = User.Identity.Name;
                dbmodel.ModifiedOn = DateTime.Now;
            }
            dbmodel.EmpId = user.UserId;
            dbmodel.ItemId1 = model.ItemId1;
            dbmodel.ItemId2 = model.ItemId2;
            dbmodel.ItemId3 = model.ItemId3;
            dbmodel.ItemId4 = model.ItemId4;
            dbmodel.ItemId5 = model.ItemId5;
            dbmodel.ItemQty1 = model.ItemQty1;
            dbmodel.ItemQty2 = model.ItemQty2;
            dbmodel.ItemQty3 = model.ItemQty3;
            dbmodel.ItemQty4 = model.ItemQty4;
            dbmodel.ItemQty5 = model.ItemQty5;
            dbmodel.Remarks = model.Remarks;
            dbmodel.OrderBy = int.Parse(User.Identity.Name);
            dbmodel.OrderDate = ProjectConvert.ConverDateStringtoDatetime(model.OrderDate);
            dbmodel.LocationId = model.LocationId;
            dbmodel.PaymentDetails = model.PaymentDetails;
            dbmodel.PaymentType = model.PaymentType;
            dbmodel.CugNumber = model.CugNumber;
            dbmodel.OrderStatus = "New";
            if (model.OrderId == 0)
            {
                dbmodel.CreatedBy = User.Identity.Name;
                dbmodel.CreatedOn = DateTime.Now;
                myapp.tbl_cm_EmpOrders.Add(dbmodel);
            }

            if (dbmodel.PaymentType == "HOD_Approval")
            {
                dbmodel.OrderStatus = "Waiting for Approval";
                dbmodel.ApproverId = user.ReportingManagerId;
                myapp.SaveChanges();
                if (user.ReportingManagerId > 0)
                {

                    SentEmailEmployeeOrders(dbmodel, true, user.ReportingManagerId.Value);
                }
            }
            else
            {
                myapp.SaveChanges();
                if (model.OrderId == 0)
                {
                    SentEmailEmployeeOrders(dbmodel);
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Save_Update_EmployeeOrder(Cm_EmployeeOrderViewModel model)
        {
            tbl_cm_EmpOrders dbmodel = new tbl_cm_EmpOrders();
            if (model.OrderId != 0)
            {
                dbmodel = myapp.tbl_cm_EmpOrders.Where(n => n.OrderId == model.OrderId).FirstOrDefault();
                dbmodel.ModifiedBy = User.Identity.Name;
                dbmodel.ModifiedOn = DateTime.Now;
            }
            dbmodel.EmpId = model.EmpId;
            dbmodel.ItemId1 = model.ItemId1;
            dbmodel.ItemId2 = model.ItemId2;
            dbmodel.ItemId3 = model.ItemId3;
            dbmodel.ItemId4 = model.ItemId4;
            dbmodel.ItemId5 = model.ItemId5;
            dbmodel.ItemQty1 = model.ItemQty1;
            dbmodel.ItemQty2 = model.ItemQty2;
            dbmodel.ItemQty3 = model.ItemQty3;
            dbmodel.ItemQty4 = model.ItemQty4;
            dbmodel.ItemQty5 = model.ItemQty5;
            dbmodel.Remarks = model.Remarks;
            dbmodel.OrderBy = int.Parse(User.Identity.Name);
            dbmodel.OrderDate = ProjectConvert.ConverDateStringtoDatetime(model.OrderDate);
            dbmodel.LocationId = model.LocationId;
            dbmodel.PaymentDetails = model.PaymentDetails;
            dbmodel.PaymentType = model.PaymentType;
            dbmodel.CugNumber = model.CugNumber;
            dbmodel.OrderStatus = "New";
            if (model.OrderId == 0)
            {
                dbmodel.CreatedBy = User.Identity.Name;
                dbmodel.CreatedOn = DateTime.Now;
                myapp.tbl_cm_EmpOrders.Add(dbmodel);
            }
            myapp.SaveChanges();
            if (model.OrderId == 0)
            {
                SentEmailEmployeeOrders(dbmodel);
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public void SentEmailEmployeeOrders(tbl_cm_EmpOrders model, bool IsHod = false, int hodid = 0)
        {
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel();
            EmailTeamplates emailtemp = new EmailTeamplates();
            mailmodel.fromemail = "Leave@hospitals.com";
            mailmodel.toemail = "logistics.kitchen@fernandez.foundation";
            mailmodel.ccemail = "";
            mailmodel.subject = "Employee Order " + model.OrderId + "";

            if (IsHod)
            {
                var hoddetails = myapp.tbl_User.Where(l => l.EmpId == hodid).SingleOrDefault();
                mailmodel.toemail = hoddetails.EmailId;
                mailmodel.ccemail = "logistics.kitchen@fernandez.foundation";
                mailmodel.subject = "Request for Approval on Employee Order " + model.OrderId + "";
            }

            string body = "";
            using (StreamReader reader = new StreamReader(Server.MapPath("~/EmailTemplates/Biomedical_Asset.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("[[Heading]]", mailmodel.subject);

            var table = "<table cellpadding='0' cellspacing='0' style='width: 100%; border: 1px solid #ededed'><tbody>";
            table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Location Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_Location where var.LocationId == model.LocationId select var.LocationName).SingleOrDefault() + "</td></tr>";
            table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Employee Name:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + (from var in myapp.tbl_User where var.UserId == model.EmpId select var.FirstName + " " + var.LastName).SingleOrDefault() + "</td></tr>";

            if (model.ItemId1 != null)
            {
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Item : " + (from var in myapp.tbl_cm_Item where var.ItemId == model.ItemId1 select var.ItemName).SingleOrDefault() + "</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>Qty : " + model.ItemQty1 + "</td></tr>";
            }
            if (model.ItemId2 != null)
            {
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Item : " + (from var in myapp.tbl_cm_Item where var.ItemId == model.ItemId2 select var.ItemName).SingleOrDefault() + "</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>Qty : " + model.ItemQty2 + "</td></tr>";
            }
            if (model.ItemId3 != null)
            {
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Item : " + (from var in myapp.tbl_cm_Item where var.ItemId == model.ItemId3 select var.ItemName).SingleOrDefault() + "</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>Qty : " + model.ItemQty3 + "</td></tr>";
            }
            if (model.ItemId4 != null)
            {
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Item : " + (from var in myapp.tbl_cm_Item where var.ItemId == model.ItemId4 select var.ItemName).SingleOrDefault() + "</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>Qty : " + model.ItemQty4 + "</td></tr>";
            }
            if (model.ItemId5 != null)
            {
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Item : " + (from var in myapp.tbl_cm_Item where var.ItemId == model.ItemId5 select var.ItemName).SingleOrDefault() + "</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>Qty : " + model.ItemQty5 + "</td></tr>";
            }


            table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Order Date:</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.OrderDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
            table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de'>Remarks</td><td style='padding: 10px; border-bottom: 1px solid #ededed; color: rgba(23,31,35,.87);'>" + model.Remarks + "</td></tr>";


            if (IsHod)
            {
                table += "<tr><td style='padding: 10px; border-bottom: 1px solid #ededed; border-right: 1px solid #ededed; width: 35%; font-weight:500; color:#171f23de' colspan='2'>Please login into infonet and approve the order.</td></tr>";

            }

            table += "</tbody></table>";
            body = body.Replace("[[table]]", table);
            mailmodel.body = body;
            //mailmodel.body = "A New Ticket Assigned to you";
            //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
            mailmodel.filepath = "";
            //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
            mailmodel.fromname = "Order";
            cm.SendEmail(mailmodel);
        }

        public ActionResult AjaxGetHRAdminOrders(JQueryDataTableParamModel param)
        {
            List<tbl_cm_EmpOrders> query = new List<tbl_cm_EmpOrders>();
            int empid = int.Parse(User.Identity.Name);

            query = (from d in myapp.tbl_cm_EmpOrders select d).ToList();

            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            query = query.OrderByDescending(l => l.OrderId).ToList();
            IEnumerable<tbl_cm_EmpOrders> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.OrderId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.EmpId != null && c.EmpId.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join u in myapp.tbl_Location on c.LocationId equals u.LocationId
                         join us1 in myapp.tbl_User on c.EmpId equals us1.UserId
                         join it1 in myapp.tbl_cm_Item on c.ItemId1.Value equals it1.ItemId
                         where c.ItemId1 != null
                         select new object[] {
                                              c.OrderId.ToString(),
                             c.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                            u.LocationName,
                                            us1.DepartmentName,
                                            us1.FirstName,
                                            it1.ItemName,
                                            c.ItemQty1.HasValue?c.ItemQty1.Value.ToString():"0",
                                            c.Remarks,c.OrderStatus,
                                              c.OrderId.ToString()
                                             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetEmployeeOrders(JQueryDataTableParamModel param)
        {
            List<tbl_cm_EmpOrders> query = new List<tbl_cm_EmpOrders>();
            int empid = int.Parse(User.Identity.Name);
            if (User.IsInRole("CanteenAdminTeam") || User.IsInRole("Admin"))
            {
                query = (from d in myapp.tbl_cm_EmpOrders select d).ToList();
            }
            else
            {
                query = (from d in myapp.tbl_cm_EmpOrders
                         where (d.CreatedBy == User.Identity.Name || d.EmpId == empid || d.ApproverId == empid)
                         select d).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            query = query.OrderByDescending(l => l.OrderId).ToList();
            IEnumerable<tbl_cm_EmpOrders> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.OrderId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.EmpId != null && c.EmpId.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationId != null && c.LocationId.ToString().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join u in myapp.tbl_Location on c.LocationId equals u.LocationId
                         join us1 in myapp.tbl_User on c.EmpId equals us1.UserId
                         join it1 in myapp.tbl_cm_Item on c.ItemId1.Value equals it1.ItemId
                         where c.ItemId1 != null
                         select new object[] {
                                              c.OrderId.ToString(),
                             c.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                            u.LocationName,
                                            us1.DepartmentName,
                                            us1.FirstName,
                                            it1.ItemName,
                                            c.ItemQty1.HasValue?c.ItemQty1.Value.ToString():"0",
                                            c.Remarks,c.OrderStatus,
                                              c.OrderId.ToString()
                                             };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEmployeeOrders_ById(int id)
        {
            Cm_EmployeeOrderViewModel dbmodel = new Cm_EmployeeOrderViewModel();
            var model = myapp.tbl_cm_EmpOrders.Where(n => n.OrderId == id).FirstOrDefault();
            dbmodel.EmpId = model.EmpId;
            dbmodel.EmpName = (from var in myapp.tbl_User where var.UserId == model.EmpId select var.FirstName + " " + var.LastName).SingleOrDefault();
            dbmodel.ItemId1 = model.ItemId1;
            if (model.ItemId1 != null)
                dbmodel.Item1Name = (from var in myapp.tbl_cm_Item where var.ItemId == model.ItemId1 select var.ItemName).SingleOrDefault();
            if (model.ItemId1 != null)
                dbmodel.Item2Name = (from var in myapp.tbl_cm_Item where var.ItemId == model.ItemId2 select var.ItemName).SingleOrDefault();
            if (model.ItemId3 != null)
                dbmodel.Item3Name = (from var in myapp.tbl_cm_Item where var.ItemId == model.ItemId3 select var.ItemName).SingleOrDefault();
            if (model.ItemId4 != null)
                dbmodel.Item4Name = (from var in myapp.tbl_cm_Item where var.ItemId == model.ItemId4 select var.ItemName).SingleOrDefault();
            if (model.ItemId5 != null)
                dbmodel.Item5Name = (from var in myapp.tbl_cm_Item where var.ItemId == model.ItemId5 select var.ItemName).SingleOrDefault();

            dbmodel.ItemId2 = model.ItemId2;
            dbmodel.ItemId3 = model.ItemId3;
            dbmodel.ItemId4 = model.ItemId4;
            dbmodel.ItemId5 = model.ItemId5;
            dbmodel.ItemQty1 = model.ItemQty1;
            dbmodel.ItemQty2 = model.ItemQty2;
            dbmodel.ItemQty3 = model.ItemQty3;
            dbmodel.ItemQty4 = model.ItemQty4;
            dbmodel.ItemQty5 = model.ItemQty5;
            dbmodel.Remarks = model.Remarks;
            dbmodel.OrderBy = model.OrderBy;
            dbmodel.OrderDate = model.OrderBy.Value.ToString("dd-MM-yyyy");
            dbmodel.LocationId = model.LocationId;
            dbmodel.CugNumber = model.CugNumber;
            dbmodel.PaymentType = model.PaymentType;
            dbmodel.PaymentDetails = model.PaymentDetails;
            return Json(dbmodel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDetailedQtyforItems(int id)
        {
            var details = (from tranitm in myapp.tbl_cm_TransactionItem
                           join trn in myapp.tbl_cm_Transaction on tranitm.TransactionId equals trn.TransactionId
                           join it in myapp.tbl_cm_Item on tranitm.ItemId equals it.ItemId
                           where tranitm.ItemId == id
                           select new
                           {
                               it.ItemName,
                               tranitm.Qty,
                               it.ItemId,
                               trn.CreatedOn
                           }).ToList();
            //var results = (from t in details

            //               select new object[]
            //               {
            //                   t.ItemName,
            //                   t.Qty.ToString(),
            //                   t.ItemId.ToString(),
            //                   t.CreatedOn.Value.ToString("dd/MM/yyyy")
            //               }).ToList();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPDFCurrentStock(int id)
        {

            int StoreId = 0;
            if (id != null && id > 0)
            {
                StoreId = id;
            }
            var store = myapp.tbl_cm_Store.Where(m => m.StoreId == id).FirstOrDefault();
            var modelitemtrsn = myapp.tbl_cm_ItemTransfer.Where(l => l.ToStoreId == StoreId).ToList();
            var modelitemrec = myapp.tbl_cm_ReceivedStock.Where(l => l.StoreId == StoreId).ToList();
            var modelitemsale = myapp.tbl_cm_Transaction.Where(l => l.CanteenId == StoreId).ToList();
            List<StockViewModel> finalmodel = new List<StockViewModel>();
            finalmodel.AddRange((from it in modelitemsale
                                 join t in myapp.tbl_cm_TransactionItem on it.TransactionId equals t.TransactionId
                                 join itm in myapp.tbl_cm_Item on t.ItemId equals itm.ItemId
                                 select new StockViewModel
                                 {
                                     ItemId = itm.ItemId,
                                     ItemName = itm.ItemName,
                                     TotalSales = t.Qty.HasValue ? t.Qty.Value : 0,
                                     Balance = 0,
                                     TotalReceived = 0
                                 }).ToList());

            finalmodel.AddRange((from it in modelitemtrsn
                                 join t in myapp.tbl_cm_ItemTransferItem on it.ItemTransferId equals t.ItemTransferId
                                 join itm in myapp.tbl_cm_Item on t.ItemId equals itm.ItemId
                                 select new StockViewModel
                                 {
                                     ItemId = itm.ItemId,
                                     ItemName = itm.ItemName,
                                     TotalSales = 0,
                                     Balance = 0,
                                     TotalReceived = Convert.ToInt32(t.QtySend.HasValue ? t.QtySend.Value : 0)
                                 }).ToList());
            finalmodel.AddRange((from it in modelitemrec
                                 join t in myapp.tbl_cm_ReceivedStockItem on it.ReceivedStockId equals t.ReceivedStockId
                                 join itm in myapp.tbl_cm_Item on t.ItemId equals itm.ItemId
                                 select new StockViewModel
                                 {
                                     ItemId = itm.ItemId,
                                     ItemName = itm.ItemName,
                                     TotalSales = 0,
                                     Balance = 0,
                                     TotalReceived = Convert.ToInt32(t.QtyRecived.HasValue ? t.QtyRecived.Value : 0)
                                 }).ToList());
            var tmp = from x in finalmodel
                      group x by x.ItemId;
            finalmodel = (from f in tmp
                          select new StockViewModel
                          {
                              ItemId = f.Key,
                              TotalReceived = f.Sum(x => x.TotalReceived),
                              TotalSales = f.Sum(x => x.TotalSales),
                              Balance = f.Sum(x => x.Balance),
                              ItemName = f.FirstOrDefault().ItemName
                          }).ToList();


            System.Data.DataTable products = new System.Data.DataTable("itemsalesreport");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Name", typeof(string));
            products.Columns.Add("Total Received", typeof(string));
            products.Columns.Add("Total Sales", typeof(string));
            products.Columns.Add("Balance", typeof(string));
            foreach (var item in finalmodel)
            {
                products.Rows.Add(
                    item.ItemId,
                    item.ItemName,
                    item.TotalReceived, item.TotalSales, (item.TotalReceived - item.TotalSales)
                );
            }
            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();
            string text = "Current Stock";
            Paragraph paragraph = new Paragraph();
            paragraph.SpacingBefore = 10;
            paragraph.SpacingAfter = 10;
            paragraph.Alignment = Element.ALIGN_CENTER;
            paragraph.Font = FontFactory.GetFont(FontFactory.HELVETICA, 20f, BaseColor.RED);
            paragraph.Add(text);
            document.Add(paragraph);
            text = "Canteen :" + store.StoreName;
            Paragraph paragraph1 = new Paragraph();
            paragraph1.SpacingBefore = 10;
            paragraph1.SpacingAfter = 10;
            paragraph1.Alignment = Element.ALIGN_RIGHT;
            paragraph1.Font = FontFactory.GetFont(FontFactory.TIMES_BOLD, 10f, BaseColor.BLUE);
            paragraph1.Add(text);
            document.Add(paragraph1);

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 5);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES_BOLD, 8, BaseColor.DARK_GRAY);
            cell.Colspan = products.Columns.Count;

            foreach (DataColumn c in products.Columns)
            {
                table.AddCell(new Phrase(c.ColumnName, myFont));
            }
            int i = products.Columns.Count;
            foreach (DataRow r in products.Rows)
            {
                if (products.Rows.Count > 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        table.AddCell(new Phrase(r[j].ToString(), font5));
                    }
                }
            }
            document.Add(table);
            document.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename= CurrentStock.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportExcelCurrentStock(int id)
        {

            int StoreId = 0;
            if (id != null && id > 0)
            {
                StoreId = id;
            }
            var store = myapp.tbl_cm_Store.Where(m => m.StoreId == id).FirstOrDefault();
            var modelitemtrsn = myapp.tbl_cm_ItemTransfer.Where(l => l.ToStoreId == StoreId).ToList();
            var modelitemrec = myapp.tbl_cm_ReceivedStock.Where(l => l.StoreId == StoreId).ToList();
            var modelitemsale = myapp.tbl_cm_Transaction.Where(l => l.CanteenId == StoreId).ToList();
            List<StockViewModel> finalmodel = new List<StockViewModel>();
            finalmodel.AddRange((from it in modelitemsale
                                 join t in myapp.tbl_cm_TransactionItem on it.TransactionId equals t.TransactionId
                                 join itm in myapp.tbl_cm_Item on t.ItemId equals itm.ItemId
                                 select new StockViewModel
                                 {
                                     ItemId = itm.ItemId,
                                     ItemName = itm.ItemName,
                                     TotalSales = t.Qty.HasValue ? t.Qty.Value : 0,
                                     Balance = 0,
                                     TotalReceived = 0
                                 }).ToList());

            finalmodel.AddRange((from it in modelitemtrsn
                                 join t in myapp.tbl_cm_ItemTransferItem on it.ItemTransferId equals t.ItemTransferId
                                 join itm in myapp.tbl_cm_Item on t.ItemId equals itm.ItemId
                                 select new StockViewModel
                                 {
                                     ItemId = itm.ItemId,
                                     ItemName = itm.ItemName,
                                     TotalSales = 0,
                                     Balance = 0,
                                     TotalReceived = Convert.ToInt32(t.QtySend.HasValue ? t.QtySend.Value : 0)
                                 }).ToList());
            finalmodel.AddRange((from it in modelitemrec
                                 join t in myapp.tbl_cm_ReceivedStockItem on it.ReceivedStockId equals t.ReceivedStockId
                                 join itm in myapp.tbl_cm_Item on t.ItemId equals itm.ItemId
                                 select new StockViewModel
                                 {
                                     ItemId = itm.ItemId,
                                     ItemName = itm.ItemName,
                                     TotalSales = 0,
                                     Balance = 0,
                                     TotalReceived = Convert.ToInt32(t.QtyRecived.HasValue ? t.QtyRecived.Value : 0)
                                 }).ToList());
            var tmp = from x in finalmodel
                      group x by x.ItemId;
            finalmodel = (from f in tmp
                          select new StockViewModel
                          {
                              ItemId = f.Key,
                              TotalReceived = f.Sum(x => x.TotalReceived),
                              TotalSales = f.Sum(x => x.TotalSales),
                              Balance = f.Sum(x => x.Balance),
                              ItemName = f.FirstOrDefault().ItemName
                          }).ToList();
            System.Data.DataTable products = new System.Data.DataTable("itemsalesreport");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Name", typeof(string));
            products.Columns.Add("Total Received", typeof(string));
            products.Columns.Add("Total Sales", typeof(string));
            products.Columns.Add("Balance", typeof(string));
            foreach (var item in finalmodel)
            {
                products.Rows.Add(
                    item.ItemId,
                    item.ItemName,
                    item.TotalReceived, item.TotalSales, (item.TotalReceived - item.TotalSales)
                );
            }


            GridView grid = new GridView
            {
                DataSource = products
            };
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=Sales.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult Cancel_ById(int id)
        {
            var model = myapp.tbl_cm_EmpOrders.Where(m => m.OrderId == id).ToList();
            if (model != null && model.Count > 0)
            {
                model[0].OrderStatus = "Cancelled by User";
                model[0].IsActive = false;
                model[0].ModifiedBy = User.Identity.Name;
                model[0].ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateOrderStatus_ById(int id, string status)
        {
            var model = myapp.tbl_cm_EmpOrders.Where(m => m.OrderId == id).ToList();
            if (model != null && model.Count > 0)
            {
                model[0].IsActive = true;
                model[0].OrderStatus = status;
                model[0].ModifiedBy = User.Identity.Name;
                model[0].ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ApproveOrder_ById(int id, string status, string comments)
        {
            var model = myapp.tbl_cm_EmpOrders.Where(m => m.OrderId == id).ToList();
            if (model != null && model.Count > 0)
            {
                int approverid = Convert.ToInt32(User.Identity.Name);
                if (approverid == model[0].ApproverId || User.IsInRole("Admin"))
                {
                    model[0].IsActive = true;
                    model[0].ApproverStatus = status;
                    model[0].ApproverId = Convert.ToInt32(User.Identity.Name);
                    model[0].ApproverComments = comments;
                    model[0].ModifiedBy = User.Identity.Name;
                    model[0].ModifiedOn = DateTime.Now;
                    myapp.SaveChanges();
                }
                else
                {
                    return Json("you dont have access to approve", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult CheckEmployeeForFreeMeal(string id)
        {
            var result = "success";
            tbl_User user = new tbl_User();
            user = myapp.tbl_User.Where(m => m.CustomUserId == id).FirstOrDefault();
            if (user == null)
            {
                int userId = Convert.ToInt32(id);
                user = myapp.tbl_User.Where(m => m.UserId == userId).FirstOrDefault();
                if (user == null)
                    return Json(null, JsonRequestBehavior.AllowGet);
            }

            return Json(user, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveItemspurchase(int StoreId, List<Items_Purchase> model)
        {
            tbl_cm_ReceivedStock m = new tbl_cm_ReceivedStock();
            string ponumber = model.Count > 0 ? model[0].pono : "";
            var checkdata = myapp.tbl_cm_ReceivedStock.Where(l => l.StockRecivedNotes == ponumber).ToList();
            if (checkdata.Count == 0)
            {
                m.CreatedBy = User.Identity.Name;
                m.CreatedOn = DateTime.Now;
                m.IsActive = true;
                m.ModifiedBy = User.Identity.Name;
                m.ModifiedOn = DateTime.Now;
                m.StockRecivedEmpId = int.Parse(User.Identity.Name);
                m.StockRecivedNotes = model[0].pono;
                m.StockRecivedOn = DateTime.Now;
                m.StoreId = StoreId;
                m.TotalItems = 0;
                m.TotalItemsAccepted = 0;
                m.TotalItemsDamage = 0;
                m.TotalItemsReturn = 0;
                myapp.tbl_cm_ReceivedStock.Add(m);
                myapp.SaveChanges();
            }
            else
            {
                m = checkdata[0];
            }

            foreach (var obj in model)
            {
                tbl_cm_ReceivedStockItem mt = new tbl_cm_ReceivedStockItem();
                mt.ReceivedStockId = m.ReceivedStockId;
                mt.QtyRecived = int.Parse(obj.qty);
                mt.QtyDamage = 0;
                mt.QtyRequested = 0;
                mt.QtyReturn = 0;
                mt.QtySend = 0;
                mt.Notes = obj.purchdt + obj.itemname;
                mt.IsActive = true;
                var itemcheck = myapp.tbl_cm_Item.Where(l => l.ItemName == obj.itemname).SingleOrDefault();
                if (itemcheck != null)
                {
                    mt.ItemId = itemcheck.ItemId;
                }
                else
                {
                    tbl_cm_Item item = new tbl_cm_Item();
                    item.CostPerServe = 0;
                    item.CreatedBy = User.Identity.Name;
                    item.CreatedOn = DateTime.Now;
                    item.DeliveryCost = 0;
                    item.HeatingInstructions = "";
                    item.IngredientTotalCost = 0;
                    item.IsActive = true;
                    item.IsRecipe = true;
                    item.ItemCode = "";
                    item.ItemDescription = obj.itemname;
                    item.ItemName = obj.itemname;
                    item.ItemType = "Recipe";
                    item.KitchenNotes = "";
                    item.LabourCost = 0;
                    item.ModifiedBy = User.Identity.Name;
                    item.ModifiedOn = DateTime.Now;
                    item.Notes = "";
                    item.OtherCost = 0;
                    item.Packaging = "";
                    item.PackagingCost = 0;
                    item.PackagingInstructions = "";
                    item.PreparationMethod = "";
                    item.Serves = 1;
                    item.ServingInstructions = "";
                    item.TotalCalories = 1;
                    item.TotalCost = Convert.ToDecimal(obj.rate);
                    item.TotalCostStaff = Convert.ToDecimal(obj.rate);
                    item.UnitTypeId = 1;
                    myapp.tbl_cm_Item.Add(item);
                    myapp.SaveChanges();
                    mt.ItemId = item.ItemId;
                }

                var checkdatareciveditems = myapp.tbl_cm_ReceivedStockItem.Where(l1 => l1.ReceivedStockId == m.ReceivedStockId && l1.ItemId == mt.ItemId).Count();
                if (checkdatareciveditems == 0)
                {
                    myapp.tbl_cm_ReceivedStockItem.Add(mt);
                    myapp.SaveChanges();
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public tbl_cm_TransactionViewModel GetPrintDetails(int id)
        {
            var dbModel = myapp.tbl_cm_Transaction.Where(m => m.TransactionId == id).FirstOrDefault();
            var dbModelItem = myapp.tbl_cm_TransactionItem.Where(m => m.TransactionId == id).ToList();
            tbl_cm_TransactionViewModel model = new tbl_cm_TransactionViewModel();

            model.BillAddress = dbModel.BillAddress;
            model.CanteenId = dbModel.CanteenId;
            model.CanteenName = myapp.tbl_cm_Store.Where(m => m.StoreId == model.CanteenId).Select(n => n.StoreName).FirstOrDefault();
            model.CardNumber = dbModel.CardNumber;
            model.CreatedBy = dbModel.CreatedBy;
            model.CreatedOn = dbModel.CreatedOn;
            model.DiscountType = dbModel.DiscountType;
            model.DiscountValue = dbModel.DiscountValue;
            model.EmpEmail = dbModel.EmpEmail;
            model.EmpId = dbModel.EmpId;
            model.EmpMobile = dbModel.EmpMobile;
            model.EmpName = dbModel.EmpName;
            model.FinalPrice = dbModel.FinalPrice;
            model.InPatientdRoomNo = dbModel.InPatientdRoomNo;
            model.IsActive = dbModel.IsActive;
            model.ModifiedBy = dbModel.ModifiedBy;
            model.ModifiedOn = dbModel.ModifiedOn;
            model.IsFreeMeal = dbModel.IsFreeMeal;
            model.ModeOfPayment = dbModel.ModeOfPayment;
            model.NameOfTheCard = dbModel.NameOfTheCard;
            model.PatientId = dbModel.PatientId;
            model.RefundAmount = dbModel.RefundAmount;
            model.SalesEmpId = dbModel.SalesEmpId;
            model.SalesEmpNotes = dbModel.SalesEmpNotes;
            model.SaleType = dbModel.SaleType;
            model.FinalPrice = dbModel.FinalPrice;
            model.TotalPrice = dbModel.TotalPrice;
            model.TransactionId = dbModel.TransactionId;
            List<tbl_cm_TransactionItemViewModel> listobj = new List<tbl_cm_TransactionItemViewModel>();
            foreach (var item in dbModelItem)
            {
                tbl_cm_TransactionItemViewModel itemmodel = new tbl_cm_TransactionItemViewModel();
                itemmodel.Amount = item.Amount;
                itemmodel.Discount = item.Discount;
                itemmodel.FinalAmount = item.FinalAmount;
                itemmodel.Id = item.Id;
                itemmodel.ItemId = item.ItemId;
                itemmodel.ItemName = myapp.tbl_cm_Item.Where(m => m.ItemId == item.ItemId).Select(n => n.ItemName).FirstOrDefault();
                itemmodel.Notes = item.Notes;
                itemmodel.Qty = item.Qty;
                itemmodel.Tax = item.Tax;
                itemmodel.TransactionId = item.TransactionId;
                listobj.Add(itemmodel);
            }
            model.tbl_Cm_TransactionItems = listobj;
            return model;
        }

        public List<EmployeeFoodDashboardCount> GetEmployeeFoodDataBind(string fromdate, string todate)
        {
            DateTime FromDate = DateTime.Now.Date;
            DateTime ToDate = DateTime.Now.Date;
            var userslit = myapp.tbl_User.ToList();
            var mealtypesmaster = myapp.tbl_MealType.ToList();
            if (fromdate != null && fromdate != "")
            {
                FromDate = ProjectConvert.ConverDateStringtoDatetime(fromdate);
            }
            if (todate != null && todate != "")
            {
                ToDate = ProjectConvert.ConverDateStringtoDatetime(todate);
            }
            var list = myapp.tbl_cm_VendorMeal.Where(l => l.IsActive == true && l.CreatedOn >= FromDate && l.CreatedOn <= ToDate).ToList();

            var datelist = list.Select(l => l.CreatedOn).Distinct().ToList();
            List<EmployeeFoodDashboardCount> listdata = new List<EmployeeFoodDashboardCount>();
            foreach (var dt in datelist)
            {
                var list2 = list.Where(l => l.CreatedOn == dt).ToList();
                var mealtypes = list2.Select(l => l.MealTypeId).Distinct().ToList();
                EmployeeFoodDashboardCount efd = new EmployeeFoodDashboardCount();
                efd.Date = dt.Value.ToString("dd/MM/yyyy");
                foreach (var mt in mealtypes)
                {
                    var list3 = list2.Where(l => l.MealTypeId == mt).ToList();
                    string MealTypeName = "";
                    if (mt > 0)
                    {
                        MealTypeName = mealtypesmaster.Where(l => l.MealTypeId == mt).SingleOrDefault().Name;
                    }
                    if (MealTypeName == "Lunch")
                    {
                        efd.LEntitledCount = (from l in list3
                                              join u in userslit on l.EmpId equals u.EmpId
                                              where u.FoodType == "Entitled"
                                              select l).Count();
                        efd.LUnEntitledCount = (from l in list3
                                                join u in userslit on l.EmpId equals u.EmpId
                                                where u.FoodType != "Entitled"
                                                select l).Count();
                    }
                    else if (MealTypeName == "Dinner")
                    {
                        efd.DEntitledCount = (from l in list3
                                              join u in userslit on l.EmpId equals u.EmpId
                                              where u.FoodType == "Entitled"
                                              select l).Count();
                        efd.DUnEntitledCount = (from l in list3
                                                join u in userslit on l.EmpId equals u.EmpId
                                                where u.FoodType != "Entitled"
                                                select l).Count();
                    }
                    else
                    {
                        efd.BFEntitledCount = (from l in list3
                                               join u in userslit on l.EmpId equals u.EmpId
                                               where u.FoodType == "Entitled"
                                               select l).Count();
                        efd.BFUnEntitledCount = (from l in list3
                                                 join u in userslit on l.EmpId equals u.EmpId
                                                 where u.FoodType != "Entitled"
                                                 select l).Count();
                    }
                }
                listdata.Add(efd);
            }
            return listdata;
        }
        public JsonResult GetCountOfEmployeeFood(string fromdate, string todate)
        {
            var listdata = GetEmployeeFoodDataBind(fromdate, todate);
            return Json(listdata, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportToCountOfEmployeeFood(string fromdate, string todate)
        {
            var listdata = GetEmployeeFoodDataBind(fromdate, todate);
            DataTable products = new DataTable("Sales");
            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("Break Fast Total", typeof(string));
            products.Columns.Add("Break Fast Entitled", typeof(string));
            products.Columns.Add("Break Fast Un Entitled", typeof(string));
            products.Columns.Add("Lunch Total", typeof(string));
            products.Columns.Add("Lunch Entitled", typeof(string));
            products.Columns.Add("Lunch Un Entitled", typeof(string));
            products.Columns.Add("Dinner Total", typeof(string));
            products.Columns.Add("Dinner Entitled", typeof(string));
            products.Columns.Add("Dinner Un Entitled", typeof(string));
            foreach (var item in listdata)
            {
                products.Rows.Add(
                    item.Date,
                    item.BFEntitledCount + item.BFUnEntitledCount,
                    item.BFEntitledCount,
                    item.BFUnEntitledCount,
                    item.LEntitledCount + item.LUnEntitledCount,
                    item.LEntitledCount,
                    item.LUnEntitledCount,
                    item.DEntitledCount + item.DUnEntitledCount,
                    item.DEntitledCount,
                    item.DUnEntitledCount
                );
            }
            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();
            string text = "Sales From " + fromdate + " To " + todate;
            Paragraph paragraph = new Paragraph();
            paragraph.SpacingBefore = 10;
            paragraph.SpacingAfter = 10;
            paragraph.Alignment = Element.ALIGN_CENTER;
            paragraph.Font = FontFactory.GetFont(FontFactory.HELVETICA, 12f, BaseColor.RED);
            paragraph.Add(text);
            document.Add(paragraph);
            Font font5 = FontFactory.GetFont(FontFactory.HELVETICA, 5);
            Font myFont = FontFactory.GetFont(FontFactory.TIMES_BOLD, 8, BaseColor.BLUE);

            PdfPTable table2 = new PdfPTable(2);
            table2.SpacingBefore = 2;
            table2.WidthPercentage = 33;
            PdfPCell cell2 = new PdfPCell(new Phrase("BreakFast"));

            table2.AddCell(new Phrase("BreakFast", myFont));
            table2.AddCell(new Phrase(""));
            table2.AddCell(new Phrase("Total Sale", font5));
            table2.AddCell(new Phrase((listdata.Sum(l => l.BFEntitledCount) + listdata.Sum(l => l.BFUnEntitledCount)).ToString(), font5));

            table2.AddCell(new Phrase("Entitled", font5));
            table2.AddCell(new Phrase((listdata.Sum(l => l.BFEntitledCount)).ToString(), font5));

            table2.AddCell(new Phrase("Un Entitled", font5));
            table2.AddCell(new Phrase((listdata.Sum(l => l.BFUnEntitledCount)).ToString(), font5));


            PdfPTable table3 = new PdfPTable(2);
            table3.SpacingBefore = 2;
            table3.WidthPercentage = 33;
            PdfPCell cell3 = new PdfPCell(new Phrase("Lunch"));

            table3.AddCell(new Phrase("Lunch", myFont));
            table3.AddCell(new Phrase(""));
            table3.AddCell(new Phrase("Total Sale", font5));
            table3.AddCell(new Phrase((listdata.Sum(l => l.LEntitledCount) + listdata.Sum(l => l.LUnEntitledCount)).ToString(), font5));

            table3.AddCell(new Phrase("Entitled", font5));
            table3.AddCell(new Phrase((listdata.Sum(l => l.LEntitledCount)).ToString(), font5));

            table3.AddCell(new Phrase("Un Entitled", font5));
            table3.AddCell(new Phrase((listdata.Sum(l => l.LUnEntitledCount)).ToString(), font5));


            PdfPTable table4 = new PdfPTable(2);
            table4.SpacingBefore = 2;
            table4.WidthPercentage = 33;
            PdfPCell cell4 = new PdfPCell(new Phrase("Dinner"));

            table4.AddCell(new Phrase("Dinner", myFont));
            table4.AddCell(new Phrase(""));
            table4.AddCell(new Phrase("Total Sale", font5));
            table4.AddCell(new Phrase((listdata.Sum(l => l.DEntitledCount) + listdata.Sum(l => l.DUnEntitledCount)).ToString(), font5));

            table4.AddCell(new Phrase("Entitled", font5));
            table4.AddCell(new Phrase((listdata.Sum(l => l.DEntitledCount)).ToString(), font5));

            table4.AddCell(new Phrase("Un Entitled", font5));
            table4.AddCell(new Phrase((listdata.Sum(l => l.DUnEntitledCount)).ToString(), font5));

            PdfPTable outer = new PdfPTable(3);
            outer.AddCell(table2);
            outer.AddCell(table3);
            outer.AddCell(table4);
            document.Add(outer);
            PdfPTable table = new PdfPTable(products.Columns.Count);
            PdfPRow row = null;

            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));

            cell.Colspan = products.Columns.Count;

            foreach (DataColumn c in products.Columns)
            {
                table.AddCell(new Phrase(c.ColumnName, myFont));
            }
            int i = products.Columns.Count;
            foreach (DataRow r in products.Rows)
            {
                if (products.Rows.Count > 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        table.AddCell(new Phrase(r[j].ToString(), font5));
                    }
                }
            }
            document.Add(table);
            document.Close();
            Response.ContentType = "application/pdf";
            Response.AddHeader("content-disposition", "attachment; filename= MealReport.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}