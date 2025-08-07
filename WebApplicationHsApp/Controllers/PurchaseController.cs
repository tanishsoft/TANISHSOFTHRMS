using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class PurchaseController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Purchase
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Purchase()
        {
            return View();
        }
        public ActionResult AjaxGetPurchase(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_Purchase select d).ToList();
            IEnumerable<tbl_Purchase> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PurchaseId.ToString().ToLower().Contains(param.sSearch.ToLower())
                   ||
                    c.PurchaseNumber.ToString() != null && c.PurchaseNumber.ToString().ToLower().Contains(param.sSearch.ToLower())
                    ||
                                (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName) != null && (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                (from var in myapp.tbl_User where var.CustomUserId == c.Employee select var.FirstName+""+var.LastName) != null && (from var in myapp.tbl_User where var.CustomUserId == c.Employee select var.FirstName + "" + var.LastName).ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                (from var in myapp.tbl_Department where var.DepartmentId == c.DepartmentId select var.DepartmentName) != null && (from var in myapp.tbl_Department where var.DepartmentId == c.DepartmentId select var.DepartmentName).ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                (from var in myapp.tbl_Vendor where var.VendorId == c.VendorId select var.Name) != null && (from var in myapp.tbl_Vendor where var.VendorId == c.VendorId select var.Name).ToString().ToLower().Contains(param.sSearch.ToLower())
                              
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                             c.PurchaseId.ToString(),
                              c.PurchaseNumber.ToString(),
                                               (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName),
                                                (from var in myapp.tbl_Department where var.DepartmentId == c.DepartmentId select var.DepartmentName),
                                                (from var in myapp.tbl_User where var.CustomUserId == c.Employee select var.FirstName+""+var.LastName),
                                                 (from var in myapp.tbl_Vendor where var.VendorId == c.VendorId select var.Name),
                                              c.Title,
                                              c.Description,
                                              c.Duration,
                                                c.PurchaseDate.HasValue?(c.PurchaseDate.Value.ToString("dd/MM/yyyy")):"",
                                                c.StartDate.HasValue?(c.StartDate.Value.ToString("dd/MM/yyyy")):"",
                                              c.TotalQty,
                                              c.QuotationNumber,
                                                c.QuotationAmount,
                                              c.PurchaseAmout,
                                              c.TotalTaxAmount,
                                              c.Discount,
                                                c.TotalAmount,
                                              c.Status,
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                               c.PurchaseId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SavePurchase(purchasemodel model)
        {
            tbl_Purchase purchase = new tbl_Purchase
            {
                LocationId = model.LocationId,
                DepartmentId = model.DepartmentId,
                PurchaseNumber = model.PurchaseNumber,
                VendorId = model.VendorId,
                Duration = model.Duration,
                Employee= (from var in myapp.tbl_User where var.EmpId == Convert.ToInt32(model.Employee) select var.CustomUserId).ToString(),
                TotalAmount=model.TotalAmount,
                TotalQty=model.TotalQty,
                TotalTaxAmount=model.TotalTaxAmount,
                Discount=model.Discount,
                QuotationAmount=model.QuotationAmount,
                QuotationNumber=model.QuotationNumber,
                PurchaseAmout=model.PurchaseAmout,
               
                IsActive = true,
                Title = model.Title,

                PurchaseDate = ProjectConvert.ConverDateStringtoDatetime(model.PurchaseDate),
                StartDate = ProjectConvert.ConverDateStringtoDatetime(model.StartDate),
               
                Remarks = model.Remarks,
                Status = model.Status,
                Description = model.Description,
                CreatedBy = User.Identity.Name,
                CreatedOn = DateTime.Now
            };
         
            myapp.tbl_Purchase.Add(purchase);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult Gettbl_Purchasebyid(int PurchaseId)
        {
            tbl_Purchase query = myapp.tbl_Purchase.Where(l => l.PurchaseId== PurchaseId).SingleOrDefault();
            string purchaseDate = query.PurchaseDate.Value.ToString("MM-dd-yyyy");
            string StartDate = query.StartDate.Value.ToString("MM-dd-yyyy");
            string EmpId = (from var in myapp.tbl_User where var.CustomUserId ==query.Employee select var.EmpId).ToString();
            var obj = new { query, purchaseDate, StartDate };
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult purchasechangestatus(tbl_Purchase model)
        {
            List<tbl_Purchase> cat = myapp.tbl_Purchase.Where(l => l.PurchaseId == model.PurchaseId).ToList();
            if (cat != null)
            {
                cat[0].Status = model.Status;

                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult UpdatePurchase(purchasemodel model)
        {
            List<tbl_Purchase> cat = myapp.tbl_Purchase.Where(l => l.PurchaseId == model.PurchaseId).ToList();
            if (cat != null)
            {
                cat[0]. LocationId = model.LocationId;
                cat[0].DepartmentId = model.DepartmentId;
                cat[0].PurchaseNumber = model.PurchaseNumber;
                cat[0].VendorId = model.VendorId;
                cat[0].Duration = model.Duration;
                cat[0].Employee = (from var in myapp.tbl_User where var.EmpId == Convert.ToInt32(model.Employee) select var.CustomUserId).ToString();
                cat[0].TotalAmount = model.TotalAmount;
                cat[0].TotalQty = model.TotalQty;
                cat[0].TotalTaxAmount = model.TotalTaxAmount;
                cat[0].Discount = model.Discount;
                cat[0].QuotationAmount = model.QuotationAmount;
                cat[0].QuotationNumber = model.QuotationNumber;
                cat[0].PurchaseAmout = model.PurchaseAmout;
                cat[0].IsActive = true;
                cat[0].Title = model.Title;

                cat[0].PurchaseDate = ProjectConvert.ConverDateStringtoDatetime(model.PurchaseDate);
                cat[0].StartDate = ProjectConvert.ConverDateStringtoDatetime(model.StartDate);

                cat[0].Remarks = model.Remarks;
                cat[0].Status = model.Status;
                cat[0].Description = model.Description;
                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
            }
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletePurchase(int id)
        {
            var cat = myapp.tbl_Purchase.Where(l => l.PurchaseId == id).ToList();
                myapp.tbl_Purchase.Remove(cat[0]);
                myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult Savepurchasewithtracker(tbl_AdministrationTrackerPurchase model)
        {
            model.IsActive = true;
            myapp.tbl_AdministrationTrackerPurchase.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveTaskwithtracker(tbl_AdministrationTrackerTask model)
        {
            model.IsActive = true;
            myapp.tbl_AdministrationTrackerTask.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetpurchaDetailsbyTrackerid(int TrackerId)
        {
            var query = from tp in myapp.tbl_AdministrationTrackerPurchase
                        join sa in myapp.tbl_Purchase on tp.PurchaseId equals sa.PurchaseId
                        join l in myapp.tbl_Location on sa.LocationId equals l.LocationId
                        join e in myapp.tbl_User on sa.Employee equals e.CustomUserId
                        join d in myapp.tbl_Department on sa.DepartmentId equals d.DepartmentId
                        join v in myapp.tbl_Vendor on sa.VendorId equals v.VendorId
                        where tp.TrackerId == TrackerId
                        select new  {
                            PurchaseId = sa.PurchaseId+"",
                            PurchaseNumber= sa.PurchaseNumber,
                            QuotationAmount = sa.QuotationAmount,
                            QuotationNumber = sa.QuotationNumber,
                            Remarks = sa.Remarks,
                            StartDate = sa.StartDate.Value,
                            PurchaseDate = sa.PurchaseDate.Value,
                            DepartmentId = d.DepartmentName,
                            LocationId =l.LocationName,
                            Employee = e.FirstName+" "+e.LastName,
                            Title=sa.Title,
                            Duration=sa.Duration,
                            Discount=sa.Discount,
                            TotalQty=sa.TotalQty,
                            TotalAmount=sa.TotalAmount,
                            TotalTaxAmount= sa.TotalTaxAmount,
                           VendorId= v.Name,
                            PurchaseAmout=sa.PurchaseAmout,
                           Status =sa.Status
                        };
            var Result = query.ToList();
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
    }
}