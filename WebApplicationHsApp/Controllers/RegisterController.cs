using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.DataModelRegister;
using WebApplicationHsApp.Models;
namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class RegisterController : Controller
    {
        private MyIntranetApp_RegisterEntities myappregister = new MyIntranetApp_RegisterEntities();
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Fuel()
        {
            return View();
        }
        public ActionResult Key()
        {
            return View();
        }
        public ActionResult LifeCell()
        {
            return View();
        }
        public ActionResult PatientTransfer()
        {
            return View();
        }
        public ActionResult Daak()
        {
            return View();
        }
        public ActionResult Demo()
        {
            return View();
        }
        public ActionResult LostFound()
        {
            return View();
        }
        public ActionResult PatientDischarge()
        {
            return View();
        }
        public ActionResult BiomedicalWaste()
        {
            return View();
        }
        public ActionResult InWardReturnable()
        {
            return View();
        }
        public ActionResult InWardNonReturnable()
        {
            return View();
        }
        public ActionResult OutWardReturnable()
        {
            return View();
        }
        public ActionResult OutWardNonReturnable()
        {
            return View();
        }
        public ActionResult InternalInWard()
        {
            return View();
        }
        public ActionResult NursingOuting()
        {
            return View();
        }
        public ActionResult GetSecurityDetails(int locationId)
        {
            var details = myapp.tbl_OutSourceUser.Where(l => l.LocationId == locationId && l.DepartmentName.ToLower().Contains("security")).ToList();
            details = details.OrderBy(l => l.FirstName).ToList();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NewFuel()
        {
            return View();
        }
        public ActionResult SaveNewFuel(tbl_Register_Fuel model)
        {
            model.CreatedBy = User.Identity.Name;
            string InSign = model.SecuritySupervisorSign;
            model.SecuritySupervisorSign = "";

            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_Fuel.Add(model);
            myappregister.SaveChanges();
            if (InSign != null && InSign != "")
            {
                try
                {
                    InSign = InSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Fuel_SecuritySupervisorSignature_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.SecuritySupervisorSign = "Register_Fuel_SecuritySupervisorSignature_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegister_Fuel(int id)
        {
            var query = myappregister.tbl_Register_Fuel.Where(l => l.Id == id).SingleOrDefault();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRegister_FuelByVechileNo(string VechileNo)
        {
            var query = myappregister.tbl_Register_Fuel.Where(l => l.VechicleNumber == VechileNo).OrderByDescending(l => l.Id).ToList();
            if (query.Count() > 0)
            {
                return Json(query[0], JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_Fuel(int id)
        {
            var query = myappregister.tbl_Register_Fuel.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_Fuel.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetFuel(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_Fuel select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_Fuel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.VechicleNumber != null && c.VechicleNumber.Contains(param.sSearch.ToLower())
                                 ||
                                c.BillNo != null && c.BillNo.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.DriverName != null && c.DriverName.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.OpeningKM != null && c.OpeningKM.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.ClosingKM != null && c.ClosingKM.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId

                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                              c.DriverName,
                                              c.VechicleNumber,
                                              c.OpeningKM,
                                              c.ClosingKM,
                                              c.TotalKM,
                                              c.BillNo,
                                              c.Amount,
                                              c.FuelFilled,
                                              c.FuelQty,
                                              c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                                              u.FirstName,
                                              c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelFuelRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_Fuel select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Fuel Register");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Vechicle Number", typeof(string));
            products.Columns.Add("Opening KM", typeof(string));
            products.Columns.Add("Closing KM", typeof(string));
            products.Columns.Add("Total KM", typeof(string));
            products.Columns.Add("Bill No", typeof(string));
            products.Columns.Add("Fuel Filled", typeof(string));
            products.Columns.Add("Amount", typeof(string));
            products.Columns.Add("Driver Name", typeof(string));
            products.Columns.Add("Security Supervisor", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    c.VechicleNumber,
                    c.OpeningKM,
                    c.ClosingKM,
                    c.TotalKM,
                    c.BillNo,
                    c.FuelFilled,
                    c.Amount,
                    c.DriverName,
                    c.SecuritySupervisor,
                    c.Remarks,
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=FuelRegister.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportPdfFuelRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_Fuel select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Fuel Register");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Vechicle Number", typeof(string));
            products.Columns.Add("Opening KM", typeof(string));
            products.Columns.Add("Closing KM", typeof(string));
            products.Columns.Add("Total KM", typeof(string));
            products.Columns.Add("Bill No", typeof(string));
            products.Columns.Add("Fuel Filled", typeof(string));
            products.Columns.Add("Amount", typeof(string));
            products.Columns.Add("Driver Name", typeof(string));
            products.Columns.Add("Security Supervisor", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    c.VechicleNumber,
                    c.OpeningKM,
                    c.ClosingKM,
                    c.TotalKM,
                    c.BillNo,
                    c.FuelFilled,
                    c.Amount,
                    c.DriverName,
                    c.SecuritySupervisor,
                    c.Remarks,
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }


            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= FuelRegister.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        //public ActionResult FuelUpdate(int id)
        //{
        //    ViewBag.Id = id;
        //    return View();
        //}
        //public ActionResult SaveFuelUpdate(UserSignoffViewModel model)
        //{
        //    model.signature = model.signature.Replace("data:image/png;base64,", String.Empty);
        //    var bytes = Convert.FromBase64String(model.signature);
        //    string file = Path.Combine(Server.MapPath("~/Documents/"), "ReturnSign_" + model.id + ".png");
        //    if (bytes.Length > 0)
        //    {
        //        using (var stream = new FileStream(file, FileMode.Create))
        //        {
        //            stream.Write(bytes, 0, bytes.Length);
        //            stream.Flush();
        //        }
        //    }
        //    var dbModel = myappregister.tbl_Register_Fuel.Where(m => m.Id == model.id).FirstOrDefault();
        //    dbModel. = "ReturnSign_" + model.id + ".png";
        //    dbModel.ReturnDateTime = DateTime.Now;
        //    dbModel.ModifiedBy = User.Identity.Name;
        //    dbModel.ModifiedOn = DateTime.Now;
        //    myapp.SaveChanges();
        //    return Json("Success", JsonRequestBehavior.AllowGet);
        //}
        public ActionResult NewKey()
        {
            return View();
        }
        public ActionResult SaveNewKey(tbl_Register_Key model)
        {
            model.CreatedBy = User.Identity.Name;
            string InSign = model.TakingSignature;
            string SecuritySign = model.TakingSecuritySignature;
            model.TakingSignature = "";
            model.TakingSecuritySignature = "";
            model.TakingDateTime = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_Key.Add(model);
            myappregister.SaveChanges();
            if (InSign != null && InSign != "")
            {
                try
                {
                    InSign = InSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Key_TakingSignature_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.TakingSignature = "Register_Key_TakingSignature_" + model.Id + ".png";
                }
                catch { }
            }
            if (SecuritySign != null && SecuritySign != "")
            {
                try
                {
                    SecuritySign = SecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(SecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Key_TakingSecuritySignature_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.TakingSecuritySignature = "Register_Key_TakingSecuritySignature_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegister_Key(int id)
        {
            var query = myappregister.tbl_Register_Key.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_Key(int id)
        {
            var query = myappregister.tbl_Register_Key.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_Key.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetKey(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_Key select d).OrderByDescending(l => l.Id).AsQueryable();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).AsQueryable();
            }
            if (param.FormType != null && param.FormType != "" && param.FormType != "All")
            {
                query = query.Where(l => l.ReturnDateTime == null).AsQueryable();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).AsQueryable();
            }
            IEnumerable<tbl_Register_Key> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => SqlFunctions.StringConvert((decimal)c.Id).Contains(param.sSearch.ToLower())
                               ||
                                c.EmployeeNumber != null && SqlFunctions.StringConvert((decimal)c.EmployeeNumber).Contains(param.sSearch.ToLower())
                                 ||
                                c.KeyDepartment != null && c.KeyDepartment.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.KeyIssuerName != null && c.KeyIssuerName.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.KeyNumber != null && c.KeyNumber.ToLower().Contains(param.sSearch.ToLower())

                                ||
                                c.NoOfKeys != null && SqlFunctions.StringConvert((decimal)c.NoOfKeys.Value).Contains(param.sSearch.ToLower())
                               
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength).ToList();
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         let u1 = myapp.tbl_User.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         let u2 = myapp.tbl_OutSourceUser.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                             c.EmployeeNumber!=null? c.EmployeeNumber.Value.ToString():"",
                                             u1!=null? u1.FirstName:(u2!=null?u2.FirstName:""),
                                              c.KeyDepartment,
                                              c.KeyIssuerName,
                                              c.KeyNumber,
                                              c.NoOfKeys!=null?c.NoOfKeys.Value.ToString():"0",
                                              c.TakingDateTime.Value.ToString("dd/MM/yyyy HH:mm"),
                                              u.FirstName,
                                              c.ReturnDateTime!=null?c.ReturnDateTime.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelKeyRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_Key select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Key Department", typeof(string));
            products.Columns.Add("Key Number", typeof(string));
            products.Columns.Add("No Of Keys", typeof(string));
            products.Columns.Add("Key Issuer Name", typeof(string));
            products.Columns.Add("Taking DateTime", typeof(string));
            products.Columns.Add("Taking Remarks", typeof(string));
            products.Columns.Add("Taking Security", typeof(string));

            products.Columns.Add("Return Employee", typeof(string));
            products.Columns.Add("Return DateTime", typeof(string));
            products.Columns.Add("Return Remarks", typeof(string));
            products.Columns.Add("Return Security", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();
                if (c.EmployeeNumber != 0)
                {
                    var u1 = myapp.tbl_User.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault();
                    var u3 = myapp.tbl_OutSourceUser.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault();

                    var u4 = myapp.tbl_User.Where(u2 => u2.EmpId == c.ReturnEmployee).SingleOrDefault();
                    var u5 = myapp.tbl_OutSourceUser.Where(u2 => u2.EmpId == c.ReturnEmployee).SingleOrDefault();

                    products.Rows.Add(
                        c.Id.ToString(),
                        (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                        u1 != null ? (u1.FirstName) : (u3 != null ? (u3.FirstName) : ""),
                        c.KeyDepartment,
                        c.KeyNumber,
                        c.NoOfKeys.HasValue ? c.NoOfKeys.Value : 0,
                        c.KeyIssuerName,
                        c.TakingDateTime.HasValue ? c.TakingDateTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                        c.TakingRemarks,
                        c.TakingSecurity,
                        u4 != null ? (u4.FirstName) : (u5 != null ? (u5.FirstName) : ""),
                         c.ReturnDateTime.HasValue ? c.ReturnDateTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                         c.ReturnRemarks,
                         c.ReturnSecurity,
                        c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                         createdby.Count > 0 ? createdby[0].FirstName : c.CreatedBy,
                        modifiedBy.Count > 0 ? modifiedBy[0].FirstName : c.ModifiedBy,
                       c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                    );
                }
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=KeyRegister.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportPdfKeyRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_Key select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Key Department", typeof(string));
            products.Columns.Add("Key Number", typeof(string));
            products.Columns.Add("No Of Keys", typeof(string));
            products.Columns.Add("Key Issuer Name", typeof(string));
            products.Columns.Add("Taking DateTime", typeof(string));
            products.Columns.Add("Taking Remarks", typeof(string));
            products.Columns.Add("Taking Security", typeof(string));

            products.Columns.Add("Return Employee", typeof(string));
            products.Columns.Add("Return DateTime", typeof(string));
            products.Columns.Add("Return Remarks", typeof(string));
            products.Columns.Add("Return Security", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();
                if (c.EmployeeNumber != 0)
                {
                    var u1 = myapp.tbl_User.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault();
                    var u3 = myapp.tbl_OutSourceUser.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault();

                    var u4 = myapp.tbl_User.Where(u2 => u2.EmpId == c.ReturnEmployee).SingleOrDefault();
                    var u5 = myapp.tbl_OutSourceUser.Where(u2 => u2.EmpId == c.ReturnEmployee).SingleOrDefault();

                    products.Rows.Add(
                        c.Id.ToString(),
                        (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                        u1 != null ? (u1.FirstName) : (u3 != null ? (u3.FirstName) : ""),
                        c.KeyDepartment,
                        c.KeyNumber,
                        c.NoOfKeys.HasValue ? c.NoOfKeys.Value : 0,
                        c.KeyIssuerName,
                        c.TakingDateTime.HasValue ? c.TakingDateTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                        c.TakingRemarks,
                        c.TakingSecurity,
                        u4 != null ? (u4.FirstName) : (u5 != null ? (u5.FirstName) : ""),
                         c.ReturnDateTime.HasValue ? c.ReturnDateTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                         c.ReturnRemarks,
                         c.ReturnSecurity,
                        c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                         createdby.Count > 0 ? createdby[0].FirstName : c.CreatedBy,
                        modifiedBy.Count > 0 ? modifiedBy[0].FirstName : c.ModifiedBy,
                       c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                    );
                }
            }


            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= KeyRegister.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult KeyUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult checkIfKeyupdated(int id)
        {
            var checkifalreadyupdate = myappregister.tbl_Register_Key.Where(l => l.Id == id && l.ReturnDateTime == null).Count();
            if (checkifalreadyupdate > 0)
            {
                return Json("Ok", JsonRequestBehavior.AllowGet);
            }
            return Json("NotOk", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveKeyUpdate(UserSignoffViewModel model)
        {
            if (model.signature != null && model.signature != "")
            {
                model.signature = model.signature.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.signature);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Key_ReturnSign_" + model.id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }
            if (model.signature2 != null && model.signature2 != "")
            {
                model.signature2 = model.signature2.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.signature2);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Key_ReturnSign_Security_" + model.id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }
            var dbModel = myappregister.tbl_Register_Key.Where(m => m.Id == model.id).FirstOrDefault();
            dbModel.ReturnSignature = "Register_Key_ReturnSign_" + model.id + ".png";
            dbModel.ReturnSecuritySign = "Register_Key_ReturnSign_Security_" + model.id + ".png";
            dbModel.ReturnDateTime = DateTime.Now;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;
            dbModel.ReturnRemarks = model.remarks;
            if (model.userempId != null && model.userempId != "")
                dbModel.ReturnEmployee = int.Parse(model.userempId);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewLifeCell()
        {
            return View();
        }

        public ActionResult SaveNewLifeCell(tbl_Register_LifeCell model)
        {
            model.CreatedBy = User.Identity.Name;
            string InSign = model.InSign;
            model.InSign = "";
            model.InDateTime = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_LifeCell.Add(model);
            myappregister.SaveChanges();
            if (InSign != null && InSign != "")
            {
                try
                {
                    InSign = InSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "InSign_Register_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.InSign = "InSign_Register_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegister_LifeCell(int id)
        {
            var query = myappregister.tbl_Register_LifeCell.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_LifeCell(int id)
        {
            var query = myappregister.tbl_Register_LifeCell.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_LifeCell.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetLifeCell(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_LifeCell select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_LifeCell> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.CompanyName != null && c.CompanyName.ToString().Contains(param.sSearch.ToLower())
                                 ||
                                c.ContactNumber != null && c.ContactNumber.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.InRemarks != null && c.InRemarks.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.NameOfPatient != null && c.NameOfPatient.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.ParamedicName != null && c.ParamedicName.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.PatientContactNumber != null && c.PatientContactNumber.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.PatientMRNo != null && c.PatientMRNo.ToLower().Contains(param.sSearch.ToLower())
                                          ||
                                c.RoomNo != null && c.RoomNo.ToLower().Contains(param.sSearch.ToLower())
                                          ||
                                c.PatientMRNo != null && c.PatientMRNo.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId

                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                              c.PatientMRNo,
                                              c.NameOfPatient,
                                              c.RoomNo,
                                              c.PatientContactNumber,
                                              c.CompanyName,
                                              c.ParamedicName,
                                              c.ContactNumber,
                                              c.Security,
                                              c.InRemarks,
                                              c.InDateTime.Value.ToString("dd/MM/yyyy HH:mm"),
                                              u.FirstName,
                                              c.OutDateTime!=null ?c.OutDateTime.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelLifecellRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_LifeCell select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("DailyVisits");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Mr No", typeof(string));
            products.Columns.Add("Patient Name", typeof(string));
            products.Columns.Add("Mobile No", typeof(string));
            products.Columns.Add("Room No", typeof(string));
            products.Columns.Add("Company Name", typeof(string));
            products.Columns.Add("Paramedic Name", typeof(string));
            products.Columns.Add("Contact Number", typeof(string));
            products.Columns.Add("Security", typeof(string));
            products.Columns.Add("In DateTime", typeof(string));
            products.Columns.Add("In Remarks", typeof(string));
            products.Columns.Add("Out DateTime", typeof(string));
            products.Columns.Add("Out Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    c.PatientMRNo,
                    c.NameOfPatient,
                    c.PatientContactNumber,
                    c.RoomNo,
                    c.CompanyName,
                    c.ParamedicName,
                    c.ContactNumber,
                    c.Security,
                    c.InDateTime.HasValue ? c.InDateTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.InRemarks,
                    c.OutDateTime.HasValue ? c.OutDateTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.OutRemarks,
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                    c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                    );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=LifeCellRegister.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPdfLifecellRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_LifeCell select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("DailyVisits");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Mr No", typeof(string));
            products.Columns.Add("Patient Name", typeof(string));
            products.Columns.Add("Mobile No", typeof(string));
            products.Columns.Add("Room No", typeof(string));
            products.Columns.Add("Company Name", typeof(string));
            products.Columns.Add("Paramedic Name", typeof(string));
            products.Columns.Add("Contact Number", typeof(string));
            products.Columns.Add("Security", typeof(string));
            products.Columns.Add("In DateTime", typeof(string));
            products.Columns.Add("In Remarks", typeof(string));
            products.Columns.Add("Out DateTime", typeof(string));
            products.Columns.Add("Out Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    c.PatientMRNo,
                    c.NameOfPatient,
                    c.PatientContactNumber,
                    c.RoomNo,
                    c.CompanyName,
                    c.ParamedicName,
                    c.ContactNumber,
                    c.Security,
                    c.InDateTime.HasValue ? c.InDateTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.InRemarks,
                    c.OutDateTime.HasValue ? c.OutDateTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.OutRemarks,
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                    c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                    );
            }


            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= LifeCell.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult LifeCellUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult checkIfLifeCellupdated(int id)
        {
            var checkifalreadyupdate = myappregister.tbl_Register_LifeCell.Where(l => l.Id == id && l.OutDateTime == null).Count();
            if (checkifalreadyupdate > 0)
            {
                return Json("Ok", JsonRequestBehavior.AllowGet);
            }
            return Json("NotOk", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveLifeCellUpdate(UserSignoffViewModel model)
        {
            if (model.signature != null && model.signature != "")
            {
                model.signature = model.signature.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.signature);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_LifeCell_OutSign_" + model.id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }
            var dbModel = myappregister.tbl_Register_LifeCell.Where(m => m.Id == model.id).FirstOrDefault();
            dbModel.OutSign = "Register_LifeCell_OutSign_" + model.id + ".png";
            dbModel.OutDateTime = DateTime.Now;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;
            dbModel.OutRemarks = model.remarks;
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult NewPatientTransfer()
        {
            return View();
        }


        public ActionResult SaveNewPatientTransfer(tbl_Register_PatientTransfer model)
        {
            model.CreatedBy = User.Identity.Name;
            string InSign = model.NurseInchargeSign;
            string InSign2 = model.SecuritySign;
            string InSign3 = model.SecuritySupervisorSign;
            model.NurseInchargeSign = "";
            model.SecuritySign = "";
            model.SecuritySupervisorSign = "";
            model.OutDateTime = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_PatientTransfer.Add(model);
            myappregister.SaveChanges();
            if (InSign != null && InSign != "")
            {
                try
                {
                    InSign = InSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "NurseInchargeSign_Register_PatientTransfer_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.NurseInchargeSign = "NurseInchargeSign_Register_PatientTransfer_" + model.Id + ".png";
                }
                catch { }
            }

            if (InSign2 != null && InSign2 != "")
            {
                try
                {
                    InSign2 = InSign2.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InSign2);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "SecuritySign_Register_PatientTransfer_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.SecuritySign = "SecuritySign_Register_PatientTransfer_" + model.Id + ".png";
                }
                catch { }
            }

            if (InSign3 != null && InSign3 != "")
            {
                try
                {
                    InSign3 = InSign3.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InSign3);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "SecuritySupervisor_Register_PatientTransfer_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.SecuritySupervisorSign = "SecuritySupervisor_Register_PatientTransfer_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegister_PatientTransfer(int id)
        {
            var query = myappregister.tbl_Register_PatientTransfer.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_PatientTransfer(int id)
        {
            var query = myappregister.tbl_Register_PatientTransfer.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_PatientTransfer.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetPatientTransfer(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_PatientTransfer select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_PatientTransfer> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.PatientMRNo != null && c.PatientMRNo.ToString().Contains(param.sSearch.ToLower())
                                 ||
                                c.NameOfPatient != null && c.NameOfPatient.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.InRemarks != null && c.InRemarks.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.FromLocation != null && c.FromLocation.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.ToLocation != null && c.ToLocation.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.OutRemarks != null && c.OutRemarks.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.InRemarks != null && c.InRemarks.ToLower().Contains(param.sSearch.ToLower())
                                          ||
                                c.Security != null && c.Security.ToLower().Contains(param.sSearch.ToLower())
                                          ||
                                c.SecuritySupervisor != null && c.SecuritySupervisor.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         select new[] {
                                              c.Id.ToString(),
                                              c.PatientMRNo,
                                              c.NameOfPatient,
                                              c.FromLocation,
                                              c.ToLocation,
                                              c.Security,
                                              c.NurseIncharge,
                                              c.OutRemarks,
                                              c.OutDateTime!=null?c.OutDateTime.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.InLocation,
                                              c.InRemarks,
                                             c.InDateTime!=null? c.InDateTime.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              u.FirstName,
                                              c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelPatientTransferRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_PatientTransfer select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("PatientTransfer");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("From Location", typeof(string));
            products.Columns.Add("To Location", typeof(string));
            products.Columns.Add("Mr No", typeof(string));
            products.Columns.Add("Patient Name", typeof(string));
            products.Columns.Add("Nurse Incharge", typeof(string));
            products.Columns.Add("Security", typeof(string));
            products.Columns.Add("Security Supervisor", typeof(string));

            products.Columns.Add("Out Date Time", typeof(string));
            products.Columns.Add("Out Remarks", typeof(string));
            products.Columns.Add("In Datetime", typeof(string));
            products.Columns.Add("In Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    c.FromLocation,
                    c.ToLocation,
                    c.PatientMRNo,
                    c.NameOfPatient,
                    c.NurseIncharge,
                    c.Security,
                    c.SecuritySupervisor,
                     c.OutDateTime.HasValue ? c.OutDateTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.OutRemarks,
                    c.InDateTime.HasValue ? c.InDateTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.InRemarks,
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                    c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }

            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=PatientTransferRegister.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportPdfPatientTransferRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_PatientTransfer select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("PatientTransfer");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("From Location", typeof(string));
            products.Columns.Add("To Location", typeof(string));
            products.Columns.Add("Mr No", typeof(string));
            products.Columns.Add("Patient Name", typeof(string));
            products.Columns.Add("Nurse Incharge", typeof(string));
            products.Columns.Add("Security", typeof(string));
            products.Columns.Add("Security Supervisor", typeof(string));

            products.Columns.Add("Out Date Time", typeof(string));
            products.Columns.Add("Out Remarks", typeof(string));
            products.Columns.Add("In Datetime", typeof(string));
            products.Columns.Add("In Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    c.FromLocation,
                    c.ToLocation,
                    c.PatientMRNo,
                    c.NameOfPatient,
                    c.NurseIncharge,
                    c.Security,
                    c.SecuritySupervisor,
                     c.OutDateTime.HasValue ? c.OutDateTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.OutRemarks,
                    c.InDateTime.HasValue ? c.InDateTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.InRemarks,
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                    c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }


            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= PatientTransferRegister.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult checkIfPatientTransferupdated(int id)
        {
            var checkifalreadyupdate = myappregister.tbl_Register_PatientTransfer.Where(l => l.Id == id && l.InDateTime == null).Count();
            if (checkifalreadyupdate > 0)
            {
                return Json("Ok", JsonRequestBehavior.AllowGet);
            }
            return Json("NotOk", JsonRequestBehavior.AllowGet);
        }
        public ActionResult PatientTransferUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult SavePatientTransferUpdate(UserSignoffViewModel model)
        {
            if (model.signature != null && model.signature != "")
            {
                //model.signature = model.signature.Replace("data:image/png;base64,", String.Empty);
                //var bytes = Convert.FromBase64String(model.signature);
                //string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_LifeCell_OutSign_" + model.id + ".png");
                //if (bytes.Length > 0)
                //{
                //    using (var stream = new FileStream(file, FileMode.Create))
                //    {
                //        stream.Write(bytes, 0, bytes.Length);
                //        stream.Flush();
                //    }
                //}
            }
            var dbModel = myappregister.tbl_Register_PatientTransfer.Where(m => m.Id == model.id).FirstOrDefault();
            //dbModel.OutSign = "Register_LifeCell_OutSign_" + model.id + ".png";
            dbModel.InDateTime = DateTime.Now;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;
            dbModel.InRemarks = model.remarks;
            dbModel.InLocation = model.userempId;
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewDaak()
        {
            return View();
        }
        public ActionResult SaveNewDaak(tbl_Register_Daak model)
        {
            model.CreatedBy = User.Identity.Name;
            string SecuritySign = model.DriverSign;
            model.DriverSign = "";
            model.DateOfRegister = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_Daak.Add(model);
            myappregister.SaveChanges();

            if (SecuritySign != null && SecuritySign != "")
            {
                try
                {
                    SecuritySign = SecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(SecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Daak_DriverSign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.DriverSign = "Register_Daak_DriverSign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegister_Daak(int id)
        {
            var query = myappregister.tbl_Register_Daak.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_Daak(int id)
        {
            var query = myappregister.tbl_Register_Daak.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_Daak.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetDaak(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_Daak select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_Daak> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.CourierName != null && c.CourierName.ToString().Contains(param.sSearch.ToLower())
                                 ||
                                c.Description != null && c.Description.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.Driver != null && c.Driver.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.FromLocation != null && c.FromLocation.ToLower().Contains(param.sSearch.ToLower())

                                ||
                                c.ToLocation != null && c.ToLocation.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.PODNo != null && c.PODNo.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                             //join l in myapp.tbl_Location on Convert.ToInt32(c.FromLocation) equals l.LocationId
                             //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId

                         select new[] {
                                              c.Id.ToString(),
                                              // l.LocationName,
                                             c.FromLocation,
                                             c.ToLocation,
                                              c.Description,
                                              c.PODNo,
                                              c.CourierName,
                                              c.Driver,

                                              c.DateOfRegister.Value.ToString("dd/MM/yyyy HH:mm"),
                                              u.FirstName,
                                              c.ModifiedOn.HasValue?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.HandOverto,
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelDaakRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_Daak select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            var products = new System.Data.DataTable("DaakRegister");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("From Location", typeof(string));
            products.Columns.Add("To Location", typeof(string));
            products.Columns.Add("Description", typeof(string));
            products.Columns.Add("POD No", typeof(string));
            products.Columns.Add("Driver", typeof(string));
            products.Columns.Add("Courier Name", typeof(string));
            products.Columns.Add("HandOver To", typeof(string));
            products.Columns.Add("Date Of Register", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),

                    c.FromLocation,
                    c.ToLocation,
                    c.Description,
                    c.PODNo, c.Driver, c.CourierName, c.HandOverto,
                    c.DateOfRegister.HasValue ? c.DateOfRegister.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.Remarks,
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                    c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }
            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=DaakRegister.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPdfDaakRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_Daak select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            var products = new System.Data.DataTable("DaakRegister");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("From Location", typeof(string));
            products.Columns.Add("To Location", typeof(string));
            products.Columns.Add("Description", typeof(string));
            products.Columns.Add("POD No", typeof(string));
            products.Columns.Add("Driver", typeof(string));
            products.Columns.Add("Courier Name", typeof(string));
            products.Columns.Add("HandOver To", typeof(string));
            products.Columns.Add("Date Of Register", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),

                    c.FromLocation,
                    c.ToLocation,
                    c.Description,
                    c.PODNo, c.Driver, c.CourierName, c.HandOverto,
                    c.DateOfRegister.HasValue ? c.DateOfRegister.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.Remarks,
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                    c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }
            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= DaakRegister.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DaakUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult SaveDaakUpdate(UserSignoffViewModel model)
        {

            if (model.signature2 != null && model.signature2 != "")
            {
                model.signature2 = model.signature2.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.signature2);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Daak_HandOverSign_" + model.id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }
            var dbModel = myappregister.tbl_Register_Daak.Where(m => m.Id == model.id).FirstOrDefault();
            dbModel.HandOverSign = "Register_Daak_HandOverSign_" + model.id + ".png";
            dbModel.HandOverto = model.userempId;
            dbModel.Remarks = dbModel.Remarks + " Hand Over Remarks : " + model.remarks;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewDemo()
        {
            return View();
        }
        public ActionResult SaveNewDemo(tbl_Register_Demo model)
        {
            model.CreatedBy = User.Identity.Name;
            string SecuritySign = model.HandoverToSecuritySign;
            model.HandoverToSecuritySign = "";
            model.HandOverDate = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_Demo.Add(model);
            myappregister.SaveChanges();

            if (SecuritySign != null && SecuritySign != "")
            {
                try
                {
                    SecuritySign = SecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(SecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Demo_HandoverToSecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.HandoverToSecuritySign = "Register_Demo_HandoverToSecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegister_Demo(int id)
        {
            var query = myappregister.tbl_Register_Demo.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_Demo(int id)
        {
            var query = myappregister.tbl_Register_Demo.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_Demo.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetDemo(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_Demo select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_Demo> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.ProductDetails != null && c.ProductDetails.ToString().Contains(param.sSearch.ToLower())
                                 ||
                                c.VendorName != null && c.VendorName.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.DcNo != null && c.DcNo.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.HandoverToSecurity != null && c.HandoverToSecurity.ToLower().Contains(param.sSearch.ToLower())

                                ||
                                c.HandOverToDepartment != null && c.HandOverToDepartment.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId

                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                             c.ProductDetails,
                                             c.VendorName,
                                              c.Qty!=null?c.Qty.Value.ToString():"0",
                                              c.DcNo,
                                              c.HandoverToSecurity,
                                              c.HandOverToDepartment,
                                              c.HandOverToPersonName,
                                              c.HandOverDate.Value.ToString("dd/MM/yyyy HH:mm"),
                                              u.FirstName,
                                              c.ReturnDate.HasValue?c.ReturnDate.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelDemoRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_Demo select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("DemoRegister");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Product Details", typeof(string));
            products.Columns.Add("Vendor Name", typeof(string));
            products.Columns.Add("Qty", typeof(string));
            products.Columns.Add("DcNo", typeof(string));
            products.Columns.Add("Handover To Security", typeof(string));
            products.Columns.Add("Handover To Department", typeof(string));
            products.Columns.Add("Handover To Person", typeof(string));
            products.Columns.Add("Handover To Remarks", typeof(string));
            products.Columns.Add("Handover To Date", typeof(string));
            products.Columns.Add("Return Security", typeof(string));
            products.Columns.Add("Return Person", typeof(string));
            products.Columns.Add("Return Department", typeof(string));
            products.Columns.Add("Return Remarks", typeof(string));
            products.Columns.Add("Return Date", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    c.ProductDetails,
                    c.VendorName,
                    c.Qty != null ? c.Qty.Value : 0,
                    c.DcNo,
                    c.HandoverToSecurity,
                    c.HandOverToDepartment, c.HandOverToPersonName, c.HandOverRemarks,
                    c.HandOverDate.HasValue ? c.HandOverDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.ReturnSecurity, c.ReturnPersonName, c.ReturnDepartment, c.ReturnRemarks,
                    c.ReturnDate.HasValue ? c.ReturnDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                    c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=DemoRegister.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportPdfDemoRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_Demo select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("DemoRegister");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Product Details", typeof(string));
            products.Columns.Add("Vendor Name", typeof(string));
            products.Columns.Add("Qty", typeof(string));
            products.Columns.Add("DcNo", typeof(string));
            products.Columns.Add("Handover To Security", typeof(string));
            products.Columns.Add("Handover To Department", typeof(string));
            products.Columns.Add("Handover To Person", typeof(string));
            products.Columns.Add("Handover To Remarks", typeof(string));
            products.Columns.Add("Handover To Date", typeof(string));
            products.Columns.Add("Return Security", typeof(string));
            products.Columns.Add("Return Person", typeof(string));
            products.Columns.Add("Return Department", typeof(string));
            products.Columns.Add("Return Remarks", typeof(string));
            products.Columns.Add("Return Date", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    c.ProductDetails,
                    c.VendorName,
                    c.Qty != null ? c.Qty.Value : 0,
                    c.DcNo,
                    c.HandoverToSecurity,
                    c.HandOverToDepartment, c.HandOverToPersonName, c.HandOverRemarks,
                    c.HandOverDate.HasValue ? c.HandOverDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.ReturnSecurity, c.ReturnPersonName, c.ReturnDepartment, c.ReturnRemarks,
                    c.ReturnDate.HasValue ? c.ReturnDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                    c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }

            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= DemoRegister.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DemoUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult SaveDemoUpdate(tbl_Register_Demo model)
        {
            if (model.ReturnSecuritySign != null && model.ReturnSecuritySign != "")
            {
                model.ReturnSecuritySign = model.ReturnSecuritySign.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.ReturnSecuritySign);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Demo_ReturnSecuritySign_" + model.Id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
                model.ReturnSecuritySign = "";
            }

            var dbModel = myappregister.tbl_Register_Demo.Where(m => m.Id == model.Id).FirstOrDefault();

            dbModel.ReturnSecuritySign = "Register_Demo_ReturnSecuritySign_" + model.Id + ".png";
            dbModel.ReturnDate = DateTime.Now;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;
            dbModel.ReturnRemarks = model.ReturnRemarks;
            dbModel.ReturnSecurity = model.ReturnSecurity;
            dbModel.ReturnDepartment = model.ReturnDepartment;
            dbModel.ReturnPersonName = model.ReturnPersonName;
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewLostFound()
        {
            return View();
        }
        public ActionResult UpdateHandOverDetails(int id, string HandOverPerson, string HandOverPersonAddress)
        {
            var query = myappregister.tbl_Register_LostFound.Where(l => l.Id == id).SingleOrDefault();
            query.Handoverto = HandOverPerson;
            query.HandoverPersonAddress = HandOverPersonAddress;
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveNewLostFound(tbl_Register_LostFound model)
        {
            model.CreatedBy = User.Identity.Name;
            string LostSecuritySign = model.LostRegisterSecuritySign;
            string FoundSecuritySign = model.FoundRegisterSecuritySign;
            if (model.RequestType == "Lost")
            {
                var d = model.LostApproximateTime.Split('_');
                model.LostDate = ProjectConvert.ConverDateStringtoDatetime(d[0]);
                model.LostApproximateTime = d[1];
            }
            else
            {
                var d = model.FoundApproximateTime.Split('_');
                model.FoundDate = ProjectConvert.ConverDateStringtoDatetime(d[0]);
                model.FoundApproximateTime = d[1];
            }
            model.LostRegisterSecuritySign = "";
            model.FoundRegisterSecuritySign = "";
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_LostFound.Add(model);
            myappregister.SaveChanges();

            if (LostSecuritySign != null && LostSecuritySign != "")
            {
                try
                {
                    LostSecuritySign = LostSecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(LostSecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Lost_SecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.LostRegisterSecuritySign = "Register_Lost_SecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            if (FoundSecuritySign != null && FoundSecuritySign != "")
            {
                try
                {
                    FoundSecuritySign = FoundSecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(FoundSecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Found_SecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.FoundRegisterSecuritySign = "Register_Found_SecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegister_LostFound(int id)
        {
            var query = myappregister.tbl_Register_LostFound.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_LostFound(int id)
        {
            var query = myappregister.tbl_Register_LostFound.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_LostFound.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetLostFound(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_LostFound select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_LostFound> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.Items != null && c.Items.Contains(param.sSearch.ToLower())
                                 ||
                                c.RequestType != null && c.RequestType.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.LostRegisterSecurity != null && c.LostRegisterSecurity.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.FoundRegisterSecurity != null && c.FoundRegisterSecurity.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.FoundPerson != null && c.FoundPerson.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.Lost_or_HandOverPerson != null && c.Lost_or_HandOverPerson.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId

                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                             c.Items,
                                             c.Lost_or_HandOverPerson,
                                              c.LostDate.HasValue?c.LostDate.Value.ToString("dd/MM/yyyy") +" "+c.LostApproximateTime:"",
                                              c.LostRegisterSecurity,
                                              c.FoundPerson,
                                              c.FoundRegisterSecurity,
                                              c.FoundDate.HasValue?c.FoundDate.Value.ToString("dd/MM/yyyy")+" "+c.FoundApproximateTime:"",
                                              c.Handoverto,
                                              u.FirstName,
                                              c.ModifiedOn.HasValue?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelLostFoundRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_LostFound select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Register_LostFound");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Request Type", typeof(string));
            products.Columns.Add("Items", typeof(string));
            products.Columns.Add("Lost_or_HandOverPerson", typeof(string));
            products.Columns.Add("LostRegisterSecurity", typeof(string));
            products.Columns.Add("Lost Remarks", typeof(string));
            products.Columns.Add("Lost Date", typeof(string));
            products.Columns.Add("Lost Approximate Time", typeof(string));

            products.Columns.Add("Found Person", typeof(string));
            products.Columns.Add("Found RegisterSecurity", typeof(string));
            products.Columns.Add("Found Remarks", typeof(string));
            products.Columns.Add("Found Date", typeof(string));
            products.Columns.Add("Found Approximate Time", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    c.RequestType,
                    c.Items,
                    c.Lost_or_HandOverPerson,
                    c.LostRegisterSecurity,
                    c.LostRemarks,
                    c.LostDate.HasValue ? c.LostDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.LostApproximateTime,

                     c.FoundPerson,
                    c.FoundRegisterSecurity,
                    c.FoundRemarks,

                     c.FoundDate.HasValue ? c.FoundDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.FoundApproximateTime,

                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                    c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=Register_LostFound.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPdfLostFoundRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_LostFound select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Register_LostFound");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Request Type", typeof(string));
            products.Columns.Add("Items", typeof(string));
            products.Columns.Add("Lost_or_HandOverPerson", typeof(string));
            products.Columns.Add("LostRegisterSecurity", typeof(string));
            products.Columns.Add("Lost Remarks", typeof(string));
            products.Columns.Add("Lost Date", typeof(string));
            products.Columns.Add("Lost Approximate Time", typeof(string));

            products.Columns.Add("Found Person", typeof(string));
            products.Columns.Add("Found RegisterSecurity", typeof(string));
            products.Columns.Add("Found Remarks", typeof(string));
            products.Columns.Add("Found Date", typeof(string));
            products.Columns.Add("Found Approximate Time", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    c.RequestType,
                    c.Items,
                    c.Lost_or_HandOverPerson,
                    c.LostRegisterSecurity,
                    c.LostRemarks,
                    c.LostDate.HasValue ? c.LostDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.LostApproximateTime,

                     c.FoundPerson,
                    c.FoundRegisterSecurity,
                    c.FoundRemarks,

                     c.FoundDate.HasValue ? c.FoundDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.FoundApproximateTime,

                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                    c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }



            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= LostFoundRegister.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult LostFoundUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult SaveLostFoundUpdate(tbl_Register_LostFound model)
        {

            var dbModel = myappregister.tbl_Register_LostFound.Where(m => m.Id == model.Id).FirstOrDefault();

            string LostSecuritySign = model.LostRegisterSecuritySign;
            string FoundSecuritySign = model.FoundRegisterSecuritySign;
            if (dbModel.RequestType != "Lost")
            {
                var d = model.LostApproximateTime.Split('_');
                dbModel.LostDate = ProjectConvert.ConverDateStringtoDatetime(d[0]);
                dbModel.LostApproximateTime = d[1];
                dbModel.LostIsEmployeeOrOther = model.LostIsEmployeeOrOther;
                dbModel.LostRegisterSecurity = model.LostRegisterSecurity;
                dbModel.LostRemarks = model.LostRemarks;
                dbModel.Lost_or_HandOverPerson = model.Lost_or_HandOverPerson;
                dbModel.LostPersonAddress = model.LostPersonAddress;
            }
            else
            {
                var d = model.FoundApproximateTime.Split('_');
                dbModel.FoundDate = ProjectConvert.ConverDateStringtoDatetime(d[0]);
                dbModel.FoundApproximateTime = d[1];
                dbModel.FoundIsEmployeeOrOther = model.FoundIsEmployeeOrOther;
                dbModel.FoundPerson = model.FoundPerson;
                dbModel.FoundRegisterSecurity = model.FoundRegisterSecurity;
                dbModel.FoundRemarks = model.FoundRemarks;
                dbModel.FoundPersonAddress = model.FoundPersonAddress;
            }

            dbModel.IsActive = true;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;

            myappregister.SaveChanges();

            if (LostSecuritySign != null && LostSecuritySign != "")
            {
                try
                {
                    LostSecuritySign = LostSecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(LostSecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Lost_SecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    dbModel.LostRegisterSecuritySign = "Register_Lost_SecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            if (FoundSecuritySign != null && FoundSecuritySign != "")
            {
                try
                {
                    FoundSecuritySign = FoundSecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(FoundSecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Found_SecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    dbModel.FoundRegisterSecuritySign = "Register_Found_SecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewPatientDischarge()
        {
            return View();
        }

        public ActionResult SaveNewPatientDischarge(tbl_Register_PatientDischarge model)
        {
            model.CreatedBy = User.Identity.Name;
            string SecuritySign = model.SecuritySign;
            model.SecuritySign = "";

            string IpExecutiveSign = model.IpExecutiveSign;
            model.IpExecutiveSign = "";

            model.OutDate = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_PatientDischarge.Add(model);
            myappregister.SaveChanges();

            if (SecuritySign != null && SecuritySign != "")
            {
                try
                {
                    SecuritySign = SecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(SecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_PatientDischarge_SecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.SecuritySign = "Register_PatientDischarge_SecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            if (IpExecutiveSign != null && IpExecutiveSign != "")
            {
                try
                {
                    IpExecutiveSign = IpExecutiveSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(IpExecutiveSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_PatientDischarge_IpExecutiveSign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.IpExecutiveSign = "Register_PatientDischarge_IpExecutiveSign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegister_PatientDischarge(int id)
        {
            var query = myappregister.tbl_Register_PatientDischarge.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_PatientDischarge(int id)
        {
            var query = myappregister.tbl_Register_PatientDischarge.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_PatientDischarge.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetPatientDischarge(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_PatientDischarge select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_PatientDischarge> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.PatientMRNo != null && c.PatientMRNo.Contains(param.sSearch.ToLower())
                                 ||
                                c.NameOfPatient != null && c.NameOfPatient.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.RoomNo != null && c.RoomNo.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.Passes != null && c.Passes.ToLower().Contains(param.sSearch.ToLower())

                                ||
                                c.IpExecutive != null && c.IpExecutive.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId

                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                             c.PatientMRNo,
                                             c.NameOfPatient,
                                              c.RoomNo,
                                              c.Passes,
                                              c.Security,
                                              c.IpExecutive,
                                              c.OutDate.Value.ToString("dd/MM/yyyy HH:mm"),
                                              u.FirstName,
                                              c.ModifiedOn.HasValue?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelPatientDischargeRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_PatientDischarge select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("PatientDischarge");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Mr No", typeof(string));
            products.Columns.Add("Patient Name", typeof(string));
            products.Columns.Add("RoomNo", typeof(string));
            products.Columns.Add("Passes", typeof(string));
            products.Columns.Add("Ip Executive", typeof(string));
            products.Columns.Add("Security", typeof(string));
            products.Columns.Add("OutDate", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    c.PatientMRNo,
                    c.NameOfPatient,
                    c.RoomNo,
                    c.Passes,
                    c.IpExecutive,
                    c.Security,
                    c.OutDate.HasValue ? c.OutDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.Remarks,
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                    c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=PatientDischarge.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public JsonResult ExportPdfPatientDischargeRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_PatientDischarge select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("PatientDischarge");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Mr No", typeof(string));
            products.Columns.Add("Patient Name", typeof(string));
            products.Columns.Add("RoomNo", typeof(string));
            products.Columns.Add("Passes", typeof(string));
            products.Columns.Add("Ip Executive", typeof(string));
            products.Columns.Add("Security", typeof(string));
            products.Columns.Add("OutDate", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    c.PatientMRNo,
                    c.NameOfPatient,
                    c.RoomNo,
                    c.Passes,
                    c.IpExecutive,
                    c.Security,
                    c.OutDate.HasValue ? c.OutDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.Remarks,
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var.FirstName).SingleOrDefault(),
                    c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }

            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= PatientDischarge.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewBiomedicalWaste()
        {
            return View();
        }

        public ActionResult SaveNewBiomedicalWaste(tbl_Register_BioMedicalWaste model)
        {
            model.CreatedBy = User.Identity.Name;
            string SecuritySign = model.SecuritySign;
            model.SecuritySign = "";

            string IpExecutiveSign = model.TakenBySign;
            model.TakenBySign = "";


            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_BioMedicalWaste.Add(model);
            myappregister.SaveChanges();

            //if (SecuritySign != null && SecuritySign != "")
            //{
            //    try
            //    {
            //        SecuritySign = SecuritySign.Replace("data:image/png;base64,", String.Empty);
            //        var bytes = Convert.FromBase64String(SecuritySign);
            //        string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_BioMedicalWaste_SecuritySign_" + model.Id + ".png");
            //        if (bytes.Length > 0)
            //        {
            //            using (var stream = new FileStream(file, FileMode.Create))
            //            {
            //                stream.Write(bytes, 0, bytes.Length);
            //                stream.Flush();
            //            }
            //        }
            //        model.SecuritySign = "Register_BioMedicalWaste_SecuritySign_" + model.Id + ".png";
            //    }
            //    catch { }
            //}
            //if (IpExecutiveSign != null && IpExecutiveSign != "")
            //{
            //    try
            //    {
            //        IpExecutiveSign = IpExecutiveSign.Replace("data:image/png;base64,", String.Empty);
            //        var bytes = Convert.FromBase64String(IpExecutiveSign);
            //        string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_BioMedicalWaste_TakenBySign_" + model.Id + ".png");
            //        if (bytes.Length > 0)
            //        {
            //            using (var stream = new FileStream(file, FileMode.Create))
            //            {
            //                stream.Write(bytes, 0, bytes.Length);
            //                stream.Flush();
            //            }
            //        }
            //        model.TakenBySign = "Register_BioMedicalWaste_TakenBySign_" + model.Id + ".png";
            //    }
            //    catch { }
            //}
            //myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegister_BiomedicalWaste(int id)
        {
            var query = myappregister.tbl_Register_BioMedicalWaste.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_BiomedicalWaste(int id)
        {
            var query = myappregister.tbl_Register_BioMedicalWaste.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_BioMedicalWaste.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetBiomedicalWaste(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_BioMedicalWaste select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_BioMedicalWaste> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.SlipNo != null && c.SlipNo.Contains(param.sSearch.ToLower())
                                 ||
                                c.NameOfHCE != null && c.NameOfHCE.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.MedicareRegistratioNo != null && c.MedicareRegistratioNo.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.TakenBy != null && c.TakenBy.ToLower().Contains(param.sSearch.ToLower())

                                ||
                                c.Security != null && c.Security.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId

                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                             c.SlipNo,
                                             c.NameOfHCE,
                                              c.MedicareRegistratioNo,
                                              c.TakenBy,
                                              //c.Security,
                                              c.YellowNoOfBags.HasValue?c.YellowNoOfBags.Value.ToString():"0",
                                              c.RedNoOfBags.HasValue?c.RedNoOfBags.Value.ToString():"0",
                                              c.BlueNoOfBags.HasValue?c.BlueNoOfBags.Value.ToString():"0",
                                              c.SharpsNoOfBags.HasValue?c.SharpsNoOfBags.Value.ToString():"0",
                                              u.FirstName,
                                              c.CreatedOn.HasValue?c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelBiomedicalWasteRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_BioMedicalWaste select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("BioMedicalWaste");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Slip No", typeof(string));
            products.Columns.Add("Name Of HCE", typeof(string));
            products.Columns.Add("TakenBy", typeof(string));
            products.Columns.Add("Security", typeof(string));
            products.Columns.Add("Yellow NoOf Bags", typeof(string));
            products.Columns.Add("Yellow Weight Kgs", typeof(string));
            products.Columns.Add("Red NoOf Bags", typeof(string));
            products.Columns.Add("Red Weight Kgs", typeof(string));
            products.Columns.Add("Blue NoOf Bags", typeof(string));
            products.Columns.Add("Blue Weight Kgs", typeof(string));
            products.Columns.Add("Sharps NoOf Bags", typeof(string));
            products.Columns.Add("Sharps Weight Kgs", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                   c.SlipNo,
                                             c.NameOfHCE,

                                              c.TakenBy,
                                              c.Security,
                                              c.YellowNoOfBags.HasValue ? c.YellowNoOfBags.Value.ToString() : "0",
                                               c.YellowWeightKgs,
                                              c.RedNoOfBags.HasValue ? c.RedNoOfBags.Value.ToString() : "0",
                                              c.RedWeightKgs,
                                              c.BlueNoOfBags.HasValue ? c.BlueNoOfBags.Value.ToString() : "0",
                                              c.BlueWeightKgs,
                                              c.SharpsNoOfBags.HasValue ? c.SharpsNoOfBags.Value.ToString() : "0",
                                     c.SharpsWeightKgs,
                                     c.Remarks,
                                              c.CreatedOn.HasValue ? c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm") : "",
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault()

                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=BioMedicalWaste.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPdfBiomedicalWasteRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_BioMedicalWaste select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("BioMedicalWaste");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Slip No", typeof(string));
            products.Columns.Add("Name Of HCE", typeof(string));
            products.Columns.Add("TakenBy", typeof(string));
            products.Columns.Add("Security", typeof(string));
            products.Columns.Add("Yellow NoOf Bags", typeof(string));
            products.Columns.Add("Yellow Weight Kgs", typeof(string));
            products.Columns.Add("Red NoOf Bags", typeof(string));
            products.Columns.Add("Red Weight Kgs", typeof(string));
            products.Columns.Add("Blue NoOf Bags", typeof(string));
            products.Columns.Add("Blue Weight Kgs", typeof(string));
            products.Columns.Add("Sharps NoOf Bags", typeof(string));
            products.Columns.Add("Sharps Weight Kgs", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                   c.SlipNo,
                                             c.NameOfHCE,

                                              c.TakenBy,
                                              c.Security,
                                              c.YellowNoOfBags.HasValue ? c.YellowNoOfBags.Value.ToString() : "0",
                                               c.YellowWeightKgs,
                                              c.RedNoOfBags.HasValue ? c.RedNoOfBags.Value.ToString() : "0",
                                              c.RedWeightKgs,
                                              c.BlueNoOfBags.HasValue ? c.BlueNoOfBags.Value.ToString() : "0",
                                              c.BlueWeightKgs,
                                              c.SharpsNoOfBags.HasValue ? c.SharpsNoOfBags.Value.ToString() : "0",
                                     c.SharpsWeightKgs,
                                     c.Remarks,
                                              c.CreatedOn.HasValue ? c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm") : "",
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault()

                );
            }

            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= BioMedicalWaste.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult NewInWardReturnable()
        {
            return View();
        }
        public ActionResult SaveNewInWardReturnable(tbl_Register_InWardReturnable model)
        {
            model.CreatedBy = User.Identity.Name;
            string SecuritySign = model.SecuritySign;
            model.SecuritySign = "";
            var edtTime = model.OutTime;
            model.ExceptedDate = ProjectConvert.ConverDateStringtoDatetime(edtTime);
            model.OutTime = DateTime.Now.ToString("hh:mm tt");
            model.OutDate = DateTime.Now;
            model.RegisterDate = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.CreatedBy = User.Identity.Name;
            model.IsActive = true;
            //model.ModifiedBy = User.Identity.Name;
            //model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_InWardReturnable.Add(model);
            myappregister.SaveChanges();

            if (SecuritySign != null && SecuritySign != "")
            {
                try
                {
                    SecuritySign = SecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(SecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_InWardReturnable_SecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.SecuritySign = "Register_InWardReturnable_SecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegister_InWardReturnable(int id)
        {
            var query = myappregister.tbl_Register_InWardReturnable.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_InWardReturnable(int id)
        {
            var query = myappregister.tbl_Register_InWardReturnable.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_InWardReturnable.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetInWardReturnable(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_InWardReturnable select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_InWardReturnable> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.ProductName != null && c.ProductName.ToString().Contains(param.sSearch.ToLower())
                                 ||
                                c.GatePassNo != null && c.GatePassNo.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.Department != null && c.Department.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.AuthorizeBy != null && c.AuthorizeBy.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.SupplierName != null && c.SupplierName.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.Security != null && c.Security.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId

                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                             c.Department,
                                             c.GatePassNo,
                                             c.SupplierName,
                                             c.ProductName,
                                              c.Qty!=null?c.Qty.Value.ToString():"0",
                                              c.AuthorizeBy,
                                              c.Reason,
                                              c.Security,
                                              c.OutDate.HasValue?c.OutDate.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.ExceptedDate.HasValue?c.ExceptedDate.Value.ToString("dd/MM/yyyy"):"",
                                             // c.Remarks,
                                              u.FirstName,
                                              c.ModifiedOn.HasValue?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelInWardReturnableRegister(string fromDate, string toDate, int locationid = 0)
        {
            var query = (from d in myappregister.tbl_Register_InWardReturnable select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (locationid != null && locationid != 0)
            {
                query = query.Where(x => x.LocationId == locationid).ToList();
            }



            var products = new System.Data.DataTable("InWardReturnable");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("GatePassNo", typeof(string));
            products.Columns.Add("Supplier", typeof(string));
            products.Columns.Add("Product", typeof(string));
            products.Columns.Add("Qty", typeof(string));
            products.Columns.Add("AuthorizeBy", typeof(string));
            products.Columns.Add("Reason", typeof(string));
            products.Columns.Add("Security", typeof(string));
            products.Columns.Add("OutDate", typeof(string));
            products.Columns.Add("ExceptedDate", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                      c.Department,
                                             c.GatePassNo,
                                             c.SupplierName,
                                             c.ProductName,
                                              c.Qty != null ? c.Qty.Value.ToString() : "0",
                                              c.AuthorizeBy,
                                              c.Reason,
                                              c.Security,
                                              c.OutDate.HasValue ? c.OutDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                              c.ExceptedDate.HasValue ? c.ExceptedDate.Value.ToString("dd/MM/yyyy") : "",
                                              c.Remarks,
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),

                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=InWardReturnable.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult InWardReturnableUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult SaveInWardReturnableUpdate(tbl_Register_InWardReturnable model)
        {

            var dbModel = myappregister.tbl_Register_InWardReturnable.Where(m => m.Id == model.Id).FirstOrDefault();

            string InPersonSign = model.InPersonSign;
            string InSecuritySign = model.InSecuritySign;
            model.InPersonSign = "";
            model.InSecuritySign = "";
            dbModel.InSecurity = model.InSecurity;
            dbModel.InPerson = model.InPerson;
            if (dbModel.Remarks == null)
            {
                dbModel.Remarks = model.Remarks;
            }
            else
            {
                dbModel.Remarks = dbModel.Remarks + " " + model.Remarks;
            }

            dbModel.IsActive = true;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;

            myappregister.SaveChanges();

            if (InPersonSign != null && InPersonSign != "")
            {
                try
                {
                    InPersonSign = InPersonSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InPersonSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_InWardReturnable_InPersonSign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    dbModel.InPersonSign = "Register_InWardReturnable_InPersonSign_" + model.Id + ".png";
                }
                catch { }
            }
            if (InSecuritySign != null && InSecuritySign != "")
            {
                try
                {
                    InSecuritySign = InSecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InSecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_InWardReturnable_InSecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    dbModel.InSecuritySign = "Register_InWardReturnable_InSecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult NewInWardNonReturnable()
        {
            return View();
        }
        public ActionResult SaveNewInWardNonReturnable(tbl_Register_InwardNonReturnable model)
        {
            model.CreatedBy = User.Identity.Name;
            string SecuritySign = model.SecuritySign;
            string HandOverSign = model.HandOverToSign;
            model.SecuritySign = "";
            model.HandOverToSign = "";
            //var OutdtTime = model.OutTime.Split('$');
            //model.OutTime = OutdtTime[1];
            //model.OutDate = ProjectConvert.ConverDateStringtoDatetime(OutdtTime[0]);
            model.RegisterDate = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_InwardNonReturnable.Add(model);
            myappregister.SaveChanges();

            if (SecuritySign != null && SecuritySign != "")
            {
                try
                {
                    SecuritySign = SecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(SecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_InWardReturnable_SecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.SecuritySign = "Register_InWardReturnable_SecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            if (HandOverSign != null && HandOverSign != "")
            {
                try
                {
                    SecuritySign = SecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(SecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_InwardNonReturnable_HandOverSign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.SecuritySign = "Register_InwardNonReturnable_HandOverSign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegister_InWardNonReturnable(int id)
        {
            var query = myappregister.tbl_Register_InwardNonReturnable.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_InWardNonReturnable(int id)
        {
            var query = myappregister.tbl_Register_InwardNonReturnable.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_InwardNonReturnable.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetInWardNonReturnable(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_InwardNonReturnable select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_InwardNonReturnable> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.ProductName != null && c.ProductName.ToString().Contains(param.sSearch.ToLower())
                                 // ||
                                 //c.GatePassNo != null && c.GatePassNo.ToLower().Contains(param.sSearch.ToLower())
                                 // ||
                                 //c. != null && c.Department.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.HandOverTo != null && c.HandOverTo.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.SupplierName != null && c.SupplierName.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.Security != null && c.Security.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId

                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,

                                             c.SupplierName,
                                             c.ProductName,
                                              c.Qty!=null?c.Qty.Value.ToString():"0",
                                              c.Amount,
                                              c.InvoiceNo,
                                              c.HandOverTo,
                                              c.Security,
                                              c.Remarks,
                                              u.FirstName,
                                              c.ModifiedOn.HasValue?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelInWardNonReturnableRegister(string fromDate, string toDate, int locationid = 0)
        {
            var query = (from d in myappregister.tbl_Register_InwardNonReturnable select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (locationid != null && locationid != 0)
            {
                query = query.Where(l => l.LocationId == locationid).ToList();
            }

            var products = new System.Data.DataTable("InwardNonReturnable");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Supplier", typeof(string));
            products.Columns.Add("Product", typeof(string));
            products.Columns.Add("Qty", typeof(string));
            products.Columns.Add("Amount", typeof(string));
            products.Columns.Add("InvoiceNo", typeof(string));
            products.Columns.Add("HandOverTo", typeof(string));
            products.Columns.Add("Security", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                     c.SupplierName,
                                             c.ProductName,
                                              c.Qty != null ? c.Qty.Value.ToString() : "0",
                                             c.Amount,
                                              c.InvoiceNo,
                                              c.HandOverTo,
                                              c.Security,
                                              c.Remarks,
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),

                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=InwardNonReturnable.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        public ActionResult NewOutWardReturnable()
        {
            return View();
        }
        public ActionResult SaveNewOutWardReturnable(tbl_Register_OutWardReturnable model)
        {
            model.CreatedBy = User.Identity.Name;
            string SecuritySign = model.SecuritySign;

            model.SecuritySign = "";
            model.ExceptedDate = ProjectConvert.ConverDateStringtoDatetime(model.Status);
            model.Status = "New";
            model.RegisterDate = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            //model.ModifiedBy = User.Identity.Name;
            //model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_OutWardReturnable.Add(model);
            myappregister.SaveChanges();

            if (SecuritySign != null && SecuritySign != "")
            {
                try
                {
                    SecuritySign = SecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(SecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_InWardReturnable_SecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.SecuritySign = "Register_InWardReturnable_SecuritySign_" + model.Id + ".png";
                }
                catch { }
            }

            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegister_OutWardReturnable(int id)
        {
            var query = myappregister.tbl_Register_OutWardReturnable.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_OutWardReturnable(int id)
        {
            var query = myappregister.tbl_Register_OutWardReturnable.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_OutWardReturnable.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetOutWardReturnable(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_OutWardReturnable select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_OutWardReturnable> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.ProductName != null && c.ProductName.ToString().Contains(param.sSearch.ToLower())
                                // ||
                                //c.GatePassNo != null && c.GatePassNo.ToLower().Contains(param.sSearch.ToLower())
                                // ||
                                //c. != null && c.Department.ToLower().Contains(param.sSearch.ToLower())
                                // ||
                                //c.HandOverTo != null && c.HandOverTo.ToLower().Contains(param.sSearch.ToLower())
                                //||
                                //c.SupplierName != null && c.SupplierName.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.Security != null && c.Security.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId

                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                             c.Department,
                                             c.ProductName,
                                              c.Qty!=null?c.Qty.Value.ToString():"0",
                                              c.GatePassNo,
                                              c.SendingTo,
                                              c.AuthorizeBy,
                                              c.Security,
                                              c.Remarks,
                                              u.FirstName,
                                              c.ModifiedOn.HasValue?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelOutWardReturnableRegister(string fromDate, string toDate, int locationid = 0)
        {
            var query = (from d in myappregister.tbl_Register_OutWardReturnable select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (locationid != null && locationid > 0)
            {
                query = query.Where(l => l.LocationId == locationid).ToList();
            }
            var products = new System.Data.DataTable("OutWardReturnable");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("Product", typeof(string));
            products.Columns.Add("Qty", typeof(string));
            products.Columns.Add("Gate pass", typeof(string));
            products.Columns.Add("Sending To", typeof(string));
            products.Columns.Add("Authorize by", typeof(string));
            products.Columns.Add("Security", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                     c.Department,
                                             c.ProductName,
                                              c.Qty != null ? c.Qty.Value.ToString() : "0",
                                              c.GatePassNo,
                                              c.SendingTo,
                                              c.AuthorizeBy,
                                              c.Security,
                                              c.Remarks,
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),

                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }



            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=OutWardReturnable.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult OutWardReturnableUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult SaveOutWardReturnableUpdate(tbl_Register_OutWardReturnable model)
        {

            var dbModel = myappregister.tbl_Register_OutWardReturnable.Where(m => m.Id == model.Id).FirstOrDefault();

            string ReturnPersonSign = model.ReturnPersonSign;
            string ReturnSecuritySign = model.ReturnSecuritySign;
            model.ReturnPersonSign = "";
            model.ReturnSecuritySign = "";
            dbModel.ReturnSecurity = model.ReturnSecurity;
            dbModel.ReturnPerson = model.ReturnPerson;
            if (dbModel.Remarks == null)
            {
                dbModel.Remarks = model.Remarks;
            }
            else
            {
                dbModel.Remarks = dbModel.Remarks + " " + model.Remarks;
            }
            dbModel.DateOfReturn = DateTime.Now;
            dbModel.IsActive = true;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;

            myappregister.SaveChanges();

            if (ReturnPersonSign != null && ReturnPersonSign != "")
            {
                try
                {
                    ReturnPersonSign = ReturnPersonSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(ReturnPersonSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_OutWardReturnable_InReturnPersonSign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    dbModel.ReturnPersonSign = "Register_OutWardReturnable_ReturnPersonSign_" + model.Id + ".png";
                }
                catch { }
            }
            if (ReturnSecuritySign != null && ReturnSecuritySign != "")
            {
                try
                {
                    ReturnSecuritySign = ReturnSecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(ReturnSecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_OutWardReturnable_ReturnSecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    dbModel.ReturnSecuritySign = "Register_OutWardReturnable_ReturnSecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult NewOutWardNonReturnable()
        {
            return View();
        }
        public ActionResult SaveNewOutWardNonReturnable(tbl_Register_OutWardNonReturnable model)
        {
            model.CreatedBy = User.Identity.Name;
            string SecuritySign = model.SecuritySign;

            model.SecuritySign = "";

            var OutdtTime = model.OutTime.Split('$');
            model.OutTime = OutdtTime[1];
            model.OutDate = ProjectConvert.ConverDateStringtoDatetime(OutdtTime[0]);
            model.RegisterDate = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_OutWardNonReturnable.Add(model);
            myappregister.SaveChanges();

            if (SecuritySign != null && SecuritySign != "")
            {
                try
                {
                    SecuritySign = SecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(SecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_InWardReturnable_SecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.SecuritySign = "Register_InWardReturnable_SecuritySign_" + model.Id + ".png";
                }
                catch { }
            }

            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegister_OutWardNonReturnable(int id)
        {
            var query = myappregister.tbl_Register_OutWardNonReturnable.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_OutWardNonReturnable(int id)
        {
            var query = myappregister.tbl_Register_OutWardNonReturnable.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_OutWardNonReturnable.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetOutWardNonReturnable(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_OutWardNonReturnable select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_OutWardNonReturnable> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.ProductName != null && c.ProductName.ToString().Contains(param.sSearch.ToLower())
                                // ||
                                //c.GatePassNo != null && c.GatePassNo.ToLower().Contains(param.sSearch.ToLower())
                                // ||
                                //c. != null && c.Department.ToLower().Contains(param.sSearch.ToLower())
                                // ||
                                //c.HandOverTo != null && c.HandOverTo.ToLower().Contains(param.sSearch.ToLower())
                                //||
                                //c.SupplierName != null && c.SupplierName.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.Security != null && c.Security.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId

                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,

                                             c.Department,
                                             c.ProductName,
                                              c.Qty!=null?c.Qty.Value.ToString():"0",
                                              c.GatePassNo,
                                              c.SendingTo,
                                              c.AuthorizeBy,
                                              c.Security,
                                              c.Remarks,
                                              u.FirstName,
                                              c.ModifiedOn.HasValue?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelOutWardNonReturnableRegister(string fromDate, string toDate, int locationid = 0)
        {
            var query = (from d in myappregister.tbl_Register_OutWardNonReturnable select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (locationid != null && locationid != 0)
            {
                query = query.Where(l => l.LocationId == locationid).ToList();
            }

            var products = new System.Data.DataTable("OutWardNonReturnable");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("Product", typeof(string));
            products.Columns.Add("Qty", typeof(string));
            products.Columns.Add("Gate pass", typeof(string));
            products.Columns.Add("Sending To", typeof(string));
            products.Columns.Add("Authorize by", typeof(string));
            products.Columns.Add("Security", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                     c.Department,
                                             c.ProductName,
                                              c.Qty != null ? c.Qty.Value.ToString() : "0",
                                              c.GatePassNo,
                                              c.SendingTo,
                                              c.AuthorizeBy,
                                              c.Security,
                                              c.Remarks,
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),

                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=OutWardNonReturnable.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }



        public ActionResult NewInternalInward()
        {
            return View();
        }
        public ActionResult SaveNewInternalInward(tbl_Register_InternalInward model)
        {
            model.CreatedBy = User.Identity.Name;
            string SecuritySign = model.SecuritySign;
            string HandOverSign = model.HandOverSign;
            string TakenBySign = model.TakenBySign;
            model.SecuritySign = "";
            model.HandOverSign = "";
            model.TakenBySign = "";
            //var OutdtTime = model.OutTime.Split('$');
            //model.OutTime = OutdtTime[1];
            //model.OutDate = ProjectConvert.ConverDateStringtoDatetime(OutdtTime[0]);
            model.RegisterDate = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_InternalInward.Add(model);
            myappregister.SaveChanges();

            if (SecuritySign != null && SecuritySign != "")
            {
                try
                {
                    SecuritySign = SecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(SecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_InWardReturnable_SecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.SecuritySign = "Register_InWardReturnable_SecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            if (HandOverSign != null && HandOverSign != "")
            {
                try
                {
                    HandOverSign = HandOverSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(HandOverSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_InWardReturnable_SecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.HandOverSign = "Register_InWardReturnable_SecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            if (TakenBySign != null && TakenBySign != "")
            {
                try
                {
                    TakenBySign = TakenBySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(TakenBySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_InWardReturnable_SecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.TakenBySign = "Register_InWardReturnable_SecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegister_InternalInward(int id)
        {
            var query = myappregister.tbl_Register_InternalInward.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_InternalInward(int id)
        {
            var query = myappregister.tbl_Register_InternalInward.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_InternalInward.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetInternalInward(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_InternalInward select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_InternalInward> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.ProductName != null && c.ProductName.ToString().Contains(param.sSearch.ToLower())
                                // ||
                                //c.GatePassNo != null && c.GatePassNo.ToLower().Contains(param.sSearch.ToLower())
                                // ||
                                //c. != null && c.Department.ToLower().Contains(param.sSearch.ToLower())
                                // ||
                                //c.HandOverTo != null && c.HandOverTo.ToLower().Contains(param.sSearch.ToLower())
                                //||
                                //c.SupplierName != null && c.SupplierName.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.Security != null && c.Security.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                             c.Department,
                                             c.ProductName,
                                              c.Qty!=null?c.Qty.Value.ToString():"0",
                                              c.GatePassNo,
                                              c.FromPerson,
                                              c.AuthorizeBy,
                                              c.Security,
                                              c.Remarks,
                                              u.FirstName,
                                              c.ModifiedOn.HasValue?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelInternalInwardRegister(string fromDate, string toDate, int locationid = 0)
        {
            var query = (from d in myappregister.tbl_Register_InternalInward select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (locationid != null && locationid != 0)
            {
                query = query.Where(l => l.LocationId == locationid).ToList();
            }

            var products = new System.Data.DataTable("OutWardNonReturnable");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("Product", typeof(string));
            products.Columns.Add("Qty", typeof(string));
            products.Columns.Add("Gate pass", typeof(string));
            products.Columns.Add("FromPerson", typeof(string));
            products.Columns.Add("Authorize by", typeof(string));
            products.Columns.Add("Security", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                     c.Department,
                                             c.ProductName,
                                              c.Qty != null ? c.Qty.Value.ToString() : "0",
                                              c.GatePassNo,
                                              c.FromPerson,
                                              c.AuthorizeBy,
                                              c.Security,
                                              c.Remarks,
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),

                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
            }



            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=InternalInward.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult VehicleParking()
        {
            return View();
        }
        public ActionResult NewVehicleParking()
        {
            return View();
        }
        public ActionResult SaveNewVehicleParking(tbl_Register_VehicleParking model)
        {
            model.CreatedBy = User.Identity.Name;
            string InSecuritySign = model.InSecuritySign;
            string InDriverSign = model.InDriverSign;
            model.InSecuritySign = "";
            model.InDriverSign = "";
            model.DateOfRegister = DateTime.Now;
            model.InTime = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            //model.ModifiedBy = User.Identity.Name;
            //model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_VehicleParking.Add(model);
            myappregister.SaveChanges();
            if (InSecuritySign != null && InSecuritySign != "")
            {
                try
                {
                    InSecuritySign = InSecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InSecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_VehicleParking_InSecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.InSecuritySign = "Register_VehicleParking_InSecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            if (InDriverSign != null && InDriverSign != "")
            {
                try
                {
                    InDriverSign = InDriverSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InDriverSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_VehicleParking_InDriverSign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.InDriverSign = "Register_VehicleParking_InDriverSign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult checkIfVehicleParkingupdated(int id)
        {
            var checkifalreadyupdate = myappregister.tbl_Register_VehicleParking.Where(l => l.Id == id && l.OutTime == null).Count();
            if (checkifalreadyupdate > 0)
            {
                return Json("Ok", JsonRequestBehavior.AllowGet);
            }
            return Json("NotOk", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRegister_VehicleParking(int id)
        {
            var query = myappregister.tbl_Register_VehicleParking.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_VehicleParking(int id)
        {
            var query = myappregister.tbl_Register_VehicleParking.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_VehicleParking.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetVehicleParking(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_VehicleParking select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_VehicleParking> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.ParkingLocation != null && c.ParkingLocation.ToString().Contains(param.sSearch.ToLower())
                                 ||
                                c.WheelerType != null && c.WheelerType.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.VehicleNameOrNumber != null && c.VehicleNameOrNumber.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.MobileNo != null && c.MobileNo.ToLower().Contains(param.sSearch.ToLower())

                                ||
                                c.InDriver != null && c.InDriver.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         //let u1 = myapp.tbl_User.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         //let u2 = myapp.tbl_OutSourceUser.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                             c.ParkingLocation,
                                             c.WheelerType,
                                              c.VehicleNameOrNumber,
                                              c.MobileNo,
                                              c.InDriver,
                                              //c.DateOfRegister.Value.ToString("dd/MM/yyyy HH:mm"),
                                              c.InTime!=null?c.InTime.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                            c.OutDriver,
                                            c.OutTime!=null?c.OutTime.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                            c.Purpose,
                                              u.FirstName,
                                              c.ModifiedOn!=null?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelVehicleParkingRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_VehicleParking select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("VehicleParking");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Parking Location", typeof(string));
            products.Columns.Add("Wheeler Type", typeof(string));
            products.Columns.Add("Vehicle", typeof(string));
            products.Columns.Add("MobileNo", typeof(string));
            products.Columns.Add("In Driver", typeof(string));
            products.Columns.Add("In Time", typeof(string));
            products.Columns.Add("Out Driver", typeof(string));
            products.Columns.Add("Out Time", typeof(string));

            products.Columns.Add("Purpose", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();

                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                   c.ParkingLocation,
                    c.WheelerType,
                    c.VehicleNameOrNumber,
                    c.MobileNo,
                    c.InDriver,
                    c.InTime.HasValue ? c.InTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.OutDriver,
                     c.OutTime.HasValue ? c.OutTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                     c.Purpose,
                     createdby.Count > 0 ? createdby[0].FirstName : c.CreatedBy,
                    modifiedBy.Count > 0 ? modifiedBy[0].FirstName : c.ModifiedBy,
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=VehicleParking.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPdfVehicleParkingRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_VehicleParking select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("VehicleParking");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Parking Location", typeof(string));
            products.Columns.Add("Wheeler Type", typeof(string));
            products.Columns.Add("Vehicle", typeof(string));
            products.Columns.Add("MobileNo", typeof(string));
            products.Columns.Add("In Driver", typeof(string));
            products.Columns.Add("In Time", typeof(string));
            products.Columns.Add("Out Driver", typeof(string));
            products.Columns.Add("Out Time", typeof(string));

            products.Columns.Add("Purpose", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();

                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                   c.ParkingLocation,
                    c.WheelerType,
                    c.VehicleNameOrNumber,
                    c.MobileNo,
                    c.InDriver,
                    c.InTime.HasValue ? c.InTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.OutDriver,
                     c.OutTime.HasValue ? c.OutTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                     c.Purpose,
                     createdby.Count > 0 ? createdby[0].FirstName : c.CreatedBy,
                    modifiedBy.Count > 0 ? modifiedBy[0].FirstName : c.ModifiedBy,
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }



            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= VehicleParking.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult VehicleParkingUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult SaveVehicleParkingUpdate(tbl_Register_VehicleParking model)
        {
            if (model.OutDriverSign != null && model.OutDriverSign != "")
            {
                model.OutDriverSign = model.OutDriverSign.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.OutDriverSign);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_VehicleParking_OutDriverSign_" + model.Id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }
            if (model.OutSecuritySign != null && model.OutSecuritySign != "")
            {
                model.OutSecuritySign = model.OutSecuritySign.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.OutSecuritySign);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_VehicleParking_OutSecuritySign_" + model.Id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }
            var dbModel = myappregister.tbl_Register_VehicleParking.Where(m => m.Id == model.Id).FirstOrDefault();
            dbModel.OutDriverSign = "Register_VehicleParking_OutDriverSign_" + model.Id + ".png";
            dbModel.OutSecuritySign = "Register_VehicleParking_OutSecuritySign_" + model.Id + ".png";
            dbModel.OutTime = DateTime.Now;
            dbModel.OutSecurity = model.OutSecurity;
            dbModel.OutDriver = model.OutDriver;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;

            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }



        public ActionResult VehicleMovement()
        {
            return View();
        }
        public ActionResult NewVehicleMovement()
        {
            return View();
        }
        public ActionResult SaveNewVehicleMovement(tbl_Register_VehicleMovement model)
        {
            model.CreatedBy = User.Identity.Name;
            string OutSecuritySign = model.OutSecuritySign;
            string InDriverSign = model.DriverSign;
            model.OutSecuritySign = "";
            model.DriverSign = "";
            model.DateOfRegister = DateTime.Now;
            model.OutTime = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            //model.ModifiedBy = User.Identity.Name;
            //model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_VehicleMovement.Add(model);
            myappregister.SaveChanges();
            if (OutSecuritySign != null && OutSecuritySign != "")
            {
                try
                {
                    OutSecuritySign = OutSecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(OutSecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_VehicleMovement_OutSecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.InSecuritySign = "Register_VehicleMovement_OutSecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            if (InDriverSign != null && InDriverSign != "")
            {
                try
                {
                    InDriverSign = InDriverSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InDriverSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_VehicleMovement_DriverSign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.DriverSign = "Register_VehicleMovement_DriverSign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult checkIfVehicleMovementupdated(int id)
        {
            var checkifalreadyupdate = myappregister.tbl_Register_VehicleMovement.Where(l => l.Id == id && l.InTime == null).Count();
            if (checkifalreadyupdate > 0)
            {
                return Json("Ok", JsonRequestBehavior.AllowGet);
            }
            return Json("NotOk", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRegister_VehicleMovement(int id)
        {
            var query = myappregister.tbl_Register_VehicleMovement.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_VehiclMovement(int id)
        {
            var query = myappregister.tbl_Register_VehicleMovement.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_VehicleMovement.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetVehicleMovement(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_VehicleMovement select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_VehicleMovement> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.InReading != null && c.InReading.ToString().Contains(param.sSearch.ToLower())
                                 ||
                                c.WheelerType != null && c.WheelerType.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.VehicleNameOrNumber != null && c.VehicleNameOrNumber.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.OutReading != null && c.OutReading.ToLower().Contains(param.sSearch.ToLower())

                                ||
                                c.Driver != null && c.Driver.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         //let u1 = myapp.tbl_User.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         //let u2 = myapp.tbl_OutSourceUser.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,

                                             c.WheelerType,
                                              c.VehicleNameOrNumber,
                                              c.Driver,
                                               c.OutReading,
                                               c.OutTime!=null?c.OutTime.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                               c.OutSecurity,
                                               c.GoingTo,
                                               c.Purpose,
                                               c.InReading,
                                              c.InTime!=null?c.InTime.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                            c.InSecurity,
                                           c.Remarks,
                                              u.FirstName,
                                              c.ModifiedOn!=null?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelVehicleMovementRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_VehicleMovement select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Wheeler Type", typeof(string));
            products.Columns.Add("Vehicle", typeof(string));
            products.Columns.Add("Driver", typeof(string));
            products.Columns.Add("Out Reading", typeof(string));
            products.Columns.Add("Out Time", typeof(string));
            products.Columns.Add("Out Security", typeof(string));
            products.Columns.Add("GoingTo", typeof(string));
            products.Columns.Add("Purpose", typeof(string));

            products.Columns.Add("In Reading", typeof(string));
            products.Columns.Add("In Time", typeof(string));
            products.Columns.Add("In Security", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();

                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                  c.WheelerType,
                    c.VehicleNameOrNumber,
                    c.Driver,
                    c.OutReading,
                    c.OutTime.HasValue ? c.OutTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.OutSecurity,
                    c.GoingTo,
                   c.Purpose,
                   c.InReading,

                     c.InTime.HasValue ? c.InTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                     c.InSecurity,
                     c.Remarks,

                     createdby.Count > 0 ? createdby[0].FirstName : c.CreatedBy,
                    modifiedBy.Count > 0 ? modifiedBy[0].FirstName : c.ModifiedBy,
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=KeyRegister.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPdfVehicleMovementRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_VehicleMovement select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Wheeler Type", typeof(string));
            products.Columns.Add("Vehicle", typeof(string));
            products.Columns.Add("Driver", typeof(string));
            products.Columns.Add("Out Reading", typeof(string));
            products.Columns.Add("Out Time", typeof(string));
            products.Columns.Add("Out Security", typeof(string));
            products.Columns.Add("GoingTo", typeof(string));
            products.Columns.Add("Purpose", typeof(string));

            products.Columns.Add("In Reading", typeof(string));
            products.Columns.Add("In Time", typeof(string));
            products.Columns.Add("In Security", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();

                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                  c.WheelerType,
                    c.VehicleNameOrNumber,
                    c.Driver,
                    c.OutReading,
                    c.OutTime.HasValue ? c.OutTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.OutSecurity,
                    c.GoingTo,
                   c.Purpose,
                   c.InReading,

                     c.InTime.HasValue ? c.InTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                     c.InSecurity,
                     c.Remarks,

                     createdby.Count > 0 ? createdby[0].FirstName : c.CreatedBy,
                    modifiedBy.Count > 0 ? modifiedBy[0].FirstName : c.ModifiedBy,
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }



            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= VehicleMovement.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult VehicleMovementUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult SaveVehicleMovementUpdate(tbl_Register_VehicleMovement model)
        {
            //if (model.OutDriverSign != null && model.OutDriverSign != "")
            //{
            //    model.OutDriverSign = model.OutDriverSign.Replace("data:image/png;base64,", String.Empty);
            //    var bytes = Convert.FromBase64String(model.OutDriverSign);
            //    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_VehicleParking_OutDriverSign_" + model.Id + ".png");
            //    if (bytes.Length > 0)
            //    {
            //        using (var stream = new FileStream(file, FileMode.Create))
            //        {
            //            stream.Write(bytes, 0, bytes.Length);
            //            stream.Flush();
            //        }
            //    }
            //}
            if (model.InSecuritySign != null && model.InSecuritySign != "")
            {
                model.InSecuritySign = model.InSecuritySign.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.InSecuritySign);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_VehicleMovement_InSecuritySign_" + model.Id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }
            var dbModel = myappregister.tbl_Register_VehicleMovement.Where(m => m.Id == model.Id).FirstOrDefault();
            //dbModel.OutDriverSign = "Register_VehicleParking_OutDriverSign_" + model.Id + ".png";
            dbModel.InSecuritySign = "Register_VehicleMovement_InSecuritySign_" + model.Id + ".png";
            dbModel.InTime = DateTime.Now;
            dbModel.InReading = model.InReading;
            dbModel.InSecurity = model.InSecurity;
            dbModel.TotalKiloMeter = (int.Parse(model.InReading) - int.Parse(dbModel.OutReading)).ToString();
            //dbModel.OutDriver = model.OutDriver;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;

            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllParkingLocations(int locid)
        {
            var query = (from d in myappregister.tbl_ParkingLocation
                             //where d.LocationId == locid
                         select d).OrderBy(l => l.ParkingLocation).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ParkingStatus()
        {
            return View();
        }
        public ActionResult GetReadytoParkVechiles(string parkingloc)
        {
            DateTime dtchekc = DateTime.Now.AddDays(-1).Date;
            var list = myapp.tbl_ValetParking.Where(l => l.ParkingAreaName == parkingloc && l.CreatedOn.Value >= dtchekc).ToList();
            var parkedveckies = myappregister.tbl_Register_VehicleParking.Where(l => l.ParkingLocation == parkingloc && l.CreatedOn.Value >= dtchekc).Select(l => l.VehicleNameOrNumber).ToList();
            list = list.Where(l => !parkedveckies.Contains(l.VehicleNo)).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetParkingStatusLive()
        {
            var plist = (from d in myappregister.tbl_ParkingLocation select d).ToList();
            var list = myappregister.tbl_Register_VehicleParking.Where(l => l.OutTime == null).ToList();
            var twowheel = list.Where(l => l.WheelerType == "2 Wheeler").ToList();
            var fourwheel = list.Where(l => l.WheelerType == "4 Wheeler").ToList();

            var results = (from p in twowheel
                           group p by p.ParkingLocation into g
                           select new { ParkingLocation = g.Key, list = g.ToList() }).ToList();
            var finalresult = (from r in results
                               join p in plist on r.ParkingLocation equals p.ParkingLocation
                               let Avaliable = p.ParkingVolume - r.list.Count()
                               let Type = "2 Wheeler"
                               select new
                               {
                                   r.ParkingLocation,
                                   Avaliable,
                                   Type
                               }).ToList();

            var results2 = (from p in fourwheel
                            group p by p.ParkingLocation into g
                            select new { ParkingLocation = g.Key, list = g.ToList() }).ToList();
            var finalresult2 = (from r in results2
                                join p in plist on r.ParkingLocation equals p.ParkingLocation
                                let Avaliable = p.ParkingVolume - r.list.Count()
                                let Type = "4 Wheeler"
                                select new
                                {
                                    r.ParkingLocation,
                                    Avaliable,
                                    Type
                                }).ToList();
            finalresult.AddRange(finalresult2);
            List<string> allavliable = new List<string>();
            foreach (var p in plist)
            {
                var checkexist = finalresult.Where(l => l.ParkingLocation == p.ParkingLocation).Count();
                if (checkexist == 0)
                {
                    allavliable.Add(p.ParkingLocation);
                }
            }
            if (allavliable.Count > 0)
            {
                var checkav = (from p in plist
                               join a in allavliable on p.ParkingLocation equals a
                               let Avaliable = p.ParkingVolume
                               let Type = p.ParkingType
                               select new
                               {
                                   p.ParkingLocation,
                                   Avaliable,
                                   Type
                               }).ToList();
                finalresult.AddRange(checkav);
            }
            return Json(finalresult, JsonRequestBehavior.AllowGet);
        }


        public ActionResult PatientMovement()
        {
            return View();
        }
        public ActionResult NewPatientMovement()
        {
            return View();
        }
        public ActionResult SaveNewPatientMovement(tbl_Patient_Movement_Register model)
        {
            model.CreatedBy = User.Identity.Name;
            string InSecuritySign = model.InSecuritySign;
            string InPersonSign = model.InPersonSign;
            model.InSecuritySign = "";
            model.InPersonSign = "";
            model.DateOfRegister = DateTime.Now;
            model.InTime = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            //model.ModifiedBy = User.Identity.Name;
            //model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Patient_Movement_Register.Add(model);
            myappregister.SaveChanges();
            if (InSecuritySign != null && InSecuritySign != "")
            {
                try
                {
                    InSecuritySign = InSecuritySign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InSecuritySign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Patient_Movement_InSecuritySign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.InSecuritySign = "Register_Patient_Movement_InSecuritySign_" + model.Id + ".png";
                }
                catch { }
            }
            if (InPersonSign != null && InPersonSign != "")
            {
                try
                {
                    InPersonSign = InPersonSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InPersonSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Patient_Movement_InPersonSign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.InPersonSign = "Register_Patient_Movement_InPersonSign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult checkIfPatientMovementupdated(int id)
        {
            var checkifalreadyupdate = myappregister.tbl_Patient_Movement_Register.Where(l => l.Id == id && l.OutTime == null).Count();
            if (checkifalreadyupdate > 0)
            {
                return Json("Ok", JsonRequestBehavior.AllowGet);
            }
            return Json("NotOk", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRegister_PatientMovement(int id)
        {
            var query = myappregister.tbl_Patient_Movement_Register.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_PatientMovement(int id)
        {
            var query = myappregister.tbl_Patient_Movement_Register.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Patient_Movement_Register.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetPatientMovement(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Patient_Movement_Register select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Patient_Movement_Register> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.PatientMRNo != null && c.PatientMRNo.ToString().Contains(param.sSearch.ToLower())
                                 ||
                                c.NameOfPatient != null && c.NameOfPatient.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.Mobile != null && c.Mobile.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.ConsultantName != null && c.ConsultantName.ToLower().Contains(param.sSearch.ToLower())

                                ||
                                c.InPerson != null && c.InPerson.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         //let u1 = myapp.tbl_User.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         //let u2 = myapp.tbl_OutSourceUser.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                             c.PatientMRNo,
                                              c.NameOfPatient,
                                              c.Mobile,
                                               c.Ward,
                                               c.OutTime!=null?c.OutTime.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                               c.InTime!=null?c.InTime.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                               c.RoomNo,
                                               c.ConsultantName,                                              
                                            //c.InSecurity,
                                           c.Remarks,
                                              u.FirstName,
                                              c.ModifiedOn!=null?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelPatientMovementRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Patient_Movement_Register select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Patient MRNo", typeof(string));
            products.Columns.Add("Name Of Patient", typeof(string));
            products.Columns.Add("Mobile", typeof(string));
            products.Columns.Add("Ward", typeof(string));
            products.Columns.Add("Out Time", typeof(string));
            products.Columns.Add("In Time", typeof(string));
            products.Columns.Add("RoomNo", typeof(string));
            products.Columns.Add("Consultant", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                 c.PatientMRNo,
                    c.NameOfPatient,
                    c.Mobile,
                    c.Ward,

                    c.OutTime.HasValue ? c.OutTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                     c.InTime.HasValue ? c.InTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.RoomNo,
                    c.ConsultantName,

                     c.Remarks,

                     createdby.Count > 0 ? createdby[0].FirstName : c.CreatedBy,
                    modifiedBy.Count > 0 ? modifiedBy[0].FirstName : c.ModifiedBy,
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=KeyRegister.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPdfPatientMovementRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Patient_Movement_Register select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Patient MRNo", typeof(string));
            products.Columns.Add("Name Of Patient", typeof(string));
            products.Columns.Add("Mobile", typeof(string));
            products.Columns.Add("Ward", typeof(string));
            products.Columns.Add("Out Time", typeof(string));
            products.Columns.Add("In Time", typeof(string));
            products.Columns.Add("RoomNo", typeof(string));
            products.Columns.Add("Consultant", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();
                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                 c.PatientMRNo,
                    c.NameOfPatient,
                    c.Mobile,
                    c.Ward,

                    c.OutTime.HasValue ? c.OutTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                     c.InTime.HasValue ? c.InTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.RoomNo,
                    c.ConsultantName,

                     c.Remarks,

                     createdby.Count > 0 ? createdby[0].FirstName : c.CreatedBy,
                    modifiedBy.Count > 0 ? modifiedBy[0].FirstName : c.ModifiedBy,
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }



            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= PatientMovementRegister.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult PatientMovementUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult SavePatientMovementUpdate(tbl_Patient_Movement_Register model)
        {
            if (model.OutSecuritySign != null && model.OutSecuritySign != "")
            {
                model.OutSecuritySign = model.OutSecuritySign.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.OutSecuritySign);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Patient_Movement_OutSecuritySign_" + model.Id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }
            if (model.OutPersonSign != null && model.OutPersonSign != "")
            {
                model.OutPersonSign = model.OutPersonSign.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.OutPersonSign);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Patient_Movement_OutPersonSign_" + model.Id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }
            var dbModel = myappregister.tbl_Patient_Movement_Register.Where(m => m.Id == model.Id).FirstOrDefault();
            //dbModel.OutDriverSign = "Register_VehicleParking_OutDriverSign_" + model.Id + ".png";
            dbModel.OutSecuritySign = "Register_Patient_Movement_OutSecuritySign_" + model.Id + ".png";
            dbModel.OutPersonSign = "Register_Patient_Movement_OutPersonSign_" + model.Id + ".png";
            dbModel.OutTime = DateTime.Now;
            dbModel.OutSecurity = model.OutSecurity;
            dbModel.OutPerson = model.OutPerson;
            dbModel.Ward = model.Ward;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;

            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }



        public ActionResult Posting()
        {
            return View();
        }
        public ActionResult NewPosting()
        {
            return View();
        }
        public ActionResult SaveNewPosting(tbl_Register_Posting model)
        {
            model.CreatedBy = User.Identity.Name;
            string InAuthorizeEmpSign = model.InAuthorizeEmpSign;
            string InTimeEmpSign = model.InTimeEmpSign;
            model.InAuthorizeEmpSign = "";
            model.InTimeEmpSign = "";
            //model.DateOfRegister = DateTime.Now;
            model.InTime = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            //model.ModifiedBy = User.Identity.Name;
            //model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_Posting.Add(model);
            myappregister.SaveChanges();
            if (InAuthorizeEmpSign != null && InAuthorizeEmpSign != "")
            {
                try
                {
                    InAuthorizeEmpSign = InAuthorizeEmpSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InAuthorizeEmpSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Posting_InAuthorizeEmpSign_" + model.PostingId + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.InAuthorizeEmpSign = "Register_Posting_InAuthorizeEmpSign_" + model.PostingId + ".png";
                }
                catch { }
            }
            if (InTimeEmpSign != null && InTimeEmpSign != "")
            {
                try
                {
                    InTimeEmpSign = InTimeEmpSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(InTimeEmpSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Posting_InTimeEmpSign_" + model.PostingId + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.InTimeEmpSign = "Register_Posting_InTimeEmpSign_" + model.PostingId + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult checkIfPostingupdated(int id)
        {
            var checkifalreadyupdate = myappregister.tbl_Register_Posting.Where(l => l.PostingId == id && l.OutTime == null).Count();
            if (checkifalreadyupdate > 0)
            {
                return Json("Ok", JsonRequestBehavior.AllowGet);
            }
            return Json("NotOk", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRegister_Posting(int id)
        {
            var query = myappregister.tbl_Register_Posting.Where(l => l.PostingId == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_Posting(int id)
        {
            var query = myappregister.tbl_Register_Posting.Where(l => l.PostingId == id).SingleOrDefault();
            myappregister.tbl_Register_Posting.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetPosting(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_Posting select d).OrderByDescending(l => l.PostingId).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_Posting> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.PostingId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.PostingType != null && c.PostingType.Contains(param.sSearch.ToLower())
                                 ||
                                c.PostingLocation != null && c.PostingLocation.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.PostingDate != null && c.PostingDate.ToString().Contains(param.sSearch.ToLower())
                                 ||
                                c.EmployeeId != null && c.EmployeeId.ToString().Contains(param.sSearch.ToLower())

                                ||
                                c.InAuthorizeEmp != null && c.InAuthorizeEmp.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         join u1 in myapp.tbl_User on c.EmployeeId equals u1.EmpId
                         //let u1 = myapp.tbl_User.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         //let u2 = myapp.tbl_OutSourceUser.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         select new[] {
                                              c.PostingId.ToString(),
                                               l.LocationName,
                                             c.PostingType,
                                              c.PostingLocation,
                                              c.PostingRank,
                                               c.PostingDate!=null?c.PostingDate.Value.ToString("dd/MM/yyyy"):"",
                                               c.InTime!=null?c.InTime.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                                c.OutTime!=null?c.OutTime.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                               c.InAuthorizeEmp,
                                               c.OutAuthorizeEmp,
                                            u1.FirstName,
                                            c.Shift,
                                           c.Remarks,
                                              u.FirstName,
                                              c.ModifiedOn!=null?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.PostingId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelPostingRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_Posting select d).OrderByDescending(l => l.PostingId).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("PostingType", typeof(string));
            products.Columns.Add("PostingLocation", typeof(string));
            products.Columns.Add("Rank", typeof(string));
            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("In Time", typeof(string));
            products.Columns.Add("Out Time", typeof(string));
            products.Columns.Add("InAuthorizeEmp", typeof(string));
            products.Columns.Add("OutAuthorizeEmp", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Shift", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();
                var u1 = myapp.tbl_User.Where(u2 => u2.EmpId == c.EmployeeId).SingleOrDefault();

                products.Rows.Add(
                    c.PostingId.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                   c.PostingType,
                                              c.PostingLocation,
                                              c.PostingRank,
                                               c.PostingDate != null ? c.PostingDate.Value.ToString("dd/MM/yyyy") : "",
                                               c.InTime != null ? c.InTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                                c.OutTime != null ? c.OutTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                               c.InAuthorizeEmp,
                                               c.OutAuthorizeEmp,
                                            u1.FirstName,
                                            c.Shift,
                                           c.Remarks,
                                              createdby,
                                              c.ModifiedOn != null ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""

                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=PostingRegister.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPdfPostingRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_Posting select d).OrderByDescending(l => l.PostingId).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("PostingType", typeof(string));
            products.Columns.Add("PostingLocation", typeof(string));
            products.Columns.Add("Rank", typeof(string));
            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("In Time", typeof(string));
            products.Columns.Add("Out Time", typeof(string));
            products.Columns.Add("InAuthorizeEmp", typeof(string));
            products.Columns.Add("OutAuthorizeEmp", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Shift", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();
                var u1 = myapp.tbl_User.Where(u2 => u2.EmpId == c.EmployeeId).SingleOrDefault();

                products.Rows.Add(
                    c.PostingId.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                   c.PostingType,
                                              c.PostingLocation,
                                              c.PostingRank,
                                               c.PostingDate != null ? c.PostingDate.Value.ToString("dd/MM/yyyy") : "",
                                               c.InTime != null ? c.InTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                                c.OutTime != null ? c.OutTime.Value.ToString("dd/MM/yyyy HH:mm") : "",
                                               c.InAuthorizeEmp,
                                               c.OutAuthorizeEmp,
                                            u1.FirstName,
                                            c.Shift,
                                           c.Remarks,
                                              createdby,
                                              c.ModifiedOn != null ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""

                );
            }



            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= PostingRegister.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult PostingUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult SavePostingUpdate(tbl_Register_Posting model)
        {
            if (model.OutAuthorizeEmpSign != null && model.OutAuthorizeEmpSign != "")
            {
                model.OutAuthorizeEmpSign = model.OutAuthorizeEmpSign.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.OutAuthorizeEmpSign);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_OutAuthorizeEmpSign_" + model.PostingId + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }
            if (model.OutTimeEmpSign != null && model.OutTimeEmpSign != "")
            {
                model.OutTimeEmpSign = model.OutTimeEmpSign.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.OutTimeEmpSign);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_OutTimeEmpSign_" + model.PostingId + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }
            var dbModel = myappregister.tbl_Register_Posting.Where(m => m.PostingId == model.PostingId).FirstOrDefault();
            //dbModel.OutDriverSign = "Register_VehicleParking_OutDriverSign_" + model.Id + ".png";
            dbModel.OutAuthorizeEmpSign = "Register_OutAuthorizeEmpSign_" + model.PostingId + ".png";
            dbModel.OutTimeEmpSign = "Register_OutTimeEmpSign_" + model.PostingId + ".png";
            dbModel.OutTime = DateTime.Now;
            dbModel.OutAuthorizeEmp = model.OutAuthorizeEmp;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;

            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult Occurance()
        {
            return View();
        }
        public ActionResult NewOccurance()
        {
            return View();
        }
        public ActionResult SaveNewOccurance(tbl_Register_Occurance model)
        {
            model.CreatedBy = User.Identity.Name;
            string EmployeeSign = model.EmployeeSign;

            model.EmployeeSign = "";

            model.DateOfRegister = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_Occurance.Add(model);
            myappregister.SaveChanges();
            if (EmployeeSign != null && EmployeeSign != "")
            {
                try
                {
                    EmployeeSign = EmployeeSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(EmployeeSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Occurance_EmployeeSign_" + model.OccuranceId + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.EmployeeSign = "Register_Occurance_EmployeeSign_" + model.OccuranceId + ".png";
                }
                catch { }
            }

            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRegister_Occurance(int id)
        {
            var query = myappregister.tbl_Register_Occurance.Where(l => l.OccuranceId == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_Occurance(int id)
        {
            var query = myappregister.tbl_Register_Occurance.Where(l => l.OccuranceId == id).SingleOrDefault();
            myappregister.tbl_Register_Occurance.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetOccurance(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_Occurance select d).OrderByDescending(l => l.OccuranceId).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_Occurance> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.OccuranceId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.Occurance != null && c.Occurance.Contains(param.sSearch.ToLower())
                                 ||
                                c.Remarks != null && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.OccuranceTime != null && c.OccuranceTime.Contains(param.sSearch.ToLower())
                                 ||
                                c.EmployeeId != null && c.EmployeeId.ToString().Contains(param.sSearch.ToLower())

                                ||
                                c.Shift != null && c.Shift.ToLower().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         join u1 in myapp.tbl_User on c.EmployeeId equals u1.EmpId
                         //let u1 = myapp.tbl_User.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         //let u2 = myapp.tbl_OutSourceUser.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         select new[] {
                                              c.OccuranceId.ToString(),
                                               l.LocationName,
                                                 u1.FirstName,
                                             c.Occurance,
                                              c.OccuranceTime,
                                              c.Shift,
                                               c.DateOfRegister!=null?c.DateOfRegister.Value.ToString("dd/MM/yyyy"):"",
                                           c.Remarks,
                                              u.FirstName,
                                              c.ModifiedOn!=null?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.OccuranceId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelOccuranceRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_Occurance select d).OrderByDescending(l => l.OccuranceId).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Occurance", typeof(string));
            products.Columns.Add("Occurance Time", typeof(string));
            products.Columns.Add("Shift", typeof(string));
            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();
                var u1 = myapp.tbl_User.Where(u2 => u2.EmpId == c.EmployeeId).SingleOrDefault();


                products.Rows.Add(
                    c.OccuranceId.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    u1 != null ? (u1.FirstName) : "",
                    c.Occurance,
                    c.OccuranceTime,
                    c.Shift,
                    c.DateOfRegister.HasValue ? c.DateOfRegister.Value.ToString("dd/MM/yyyy HH:mm") : "",

                    c.Remarks,
                    createdby,
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=OccuranceRegister.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPdfOccuranceRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_Occurance select d).OrderByDescending(l => l.OccuranceId).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Occurance", typeof(string));
            products.Columns.Add("Occurance Time", typeof(string));
            products.Columns.Add("Shift", typeof(string));
            products.Columns.Add("Date", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();
                var u1 = myapp.tbl_User.Where(u2 => u2.EmpId == c.EmployeeId).SingleOrDefault();


                products.Rows.Add(
                    c.OccuranceId.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    u1 != null ? (u1.FirstName) : "",
                    c.Occurance,
                    c.OccuranceTime,
                    c.Shift,
                    c.DateOfRegister.HasValue ? c.DateOfRegister.Value.ToString("dd/MM/yyyy HH:mm") : "",

                    c.Remarks,
                    createdby,
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }



            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= OccuranceRegister.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GatePassApprove()
        {
            return View();
        }
        public ActionResult SecurityGatePass()
        {
            return View();
        }

        public ActionResult AjaxGatePassApprove(JQueryDataTableParamModel param)
        {

            List<tbl_GatePass> query = (from d in myapp.tbl_GatePass select d).ToList();
            string currentuser = User.Identity.Name;
            var tbluser = myapp.tbl_User.Where(l => l.CustomUserId == currentuser).SingleOrDefault();
            if (tbluser != null)
            {
                string approval = GatePassWorkflow.HodApproval.ToString();
                query = query.Where(l => l.AuthorizedByEmpId == tbluser.EmpId && l.CurrentWorkFlow == approval).ToList();
            }
            query = query.OrderByDescending(m => m.GatePassId).ToList();
            IEnumerable<tbl_GatePass> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.GatePassId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.NameOfTheCompany != null && c.NameOfTheCompany.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_GatePass> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                                           select new object[] {
                                               c.GatePassId.ToString(),
                                             (from V in myapp.tbl_Location where V.LocationId == c.LocationId select V.LocationName).SingleOrDefault(),
            c.ToLocation,
                                               c.Date.Value.ToString("dd/MM/yyyy"),
                                               c.Address,
                                               c.AuthorizedByName +" "+c.AuthorizedByEmpId
                                               ,c.ReceivingCompanyName,c.Comments,c.GatePassType,c.Status,c.CurrentWorkFlow,
                                           u.FirstName
                                             ,c.AuthorizedByDepartment,  c.GatePassId.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxSecurityGatePass(JQueryDataTableParamModel param)
        {

            List<tbl_GatePass> query = (from d in myapp.tbl_GatePass select d).ToList();
            string currentuser = User.Identity.Name;
            var tbluser = myapp.tbl_User.Where(l => l.CustomUserId == currentuser).SingleOrDefault();
            if (tbluser != null)
            {
                if (User.IsInRole("Admin"))
                {

                }
                else
                {
                    string frmLocSecurityOut = GatePassWorkflow.FromLocationSecurityOut.ToString();
                    string frmLocSecurityIn = GatePassWorkflow.FromLocationSecurityIn.ToString();
                    string ToLocSecurityIn = GatePassWorkflow.ToLocationSecurityIn.ToString();
                    string ToLocSecurityOut = GatePassWorkflow.ToLocationSecurityOut.ToString();
                    query = query.Where(l => l.CurrentWorkFlow == frmLocSecurityOut
                    || l.CurrentWorkFlow == frmLocSecurityIn
                    || l.CurrentWorkFlow == ToLocSecurityIn
                    || l.CurrentWorkFlow == ToLocSecurityOut).ToList();
                }
            }
            query = query.OrderByDescending(m => m.GatePassId).ToList();
            IEnumerable<tbl_GatePass> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.GatePassId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.LocationId != null && c.LocationId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.NameOfTheCompany != null && c.NameOfTheCompany.ToString().ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_GatePass> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedCompanies
                                           join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                                           select new object[] {
                                               c.GatePassId.ToString(),
                                             (from V in myapp.tbl_Location where V.LocationId == c.LocationId select V.LocationName).SingleOrDefault(),
            c.ToLocation,
                                               c.Date.Value.ToString("dd/MM/yyyy"),
                                               c.Address,
                                               c.AuthorizedByName +" "+c.AuthorizedByEmpId
                                               ,c.ReceivingCompanyName,c.Comments,c.GatePassType,c.Status ,c.CurrentWorkFlow,
                                           u.FirstName
                                             ,c.AuthorizedByDepartment,

                                              c.GatePassType+"^"+c.Status+"^"+c.GatePassId.ToString()+"^"+c.CurrentWorkFlow
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult MobileDepositing()
        {
            return View();
        }
        public ActionResult NewMobileDepositing()
        {
            return View();
        }

        public ActionResult SaveNewMobileDepositing(tbl_Register_MobileDepositing model)
        {
            model.CreatedBy = User.Identity.Name;
            string SecurityInSign = model.SecurityInSign;

            model.SecurityInSign = "";

            string EmployeeInSign = model.EmployeeInSign;

            model.EmployeeInSign = "";

            model.RegisterDate = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_MobileDepositing.Add(model);
            myappregister.SaveChanges();
            if (SecurityInSign != null && SecurityInSign != "")
            {
                try
                {
                    SecurityInSign = SecurityInSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(SecurityInSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_MobileDepositing_SecurityInSign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.SecurityInSign = "Register_MobileDepositing_SecurityInSign_" + model.Id + ".png";
                }
                catch { }
            }
            if (EmployeeInSign != null && EmployeeInSign != "")
            {
                try
                {
                    EmployeeInSign = EmployeeInSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(EmployeeInSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_MobileDepositing_EmployeeInSign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.EmployeeInSign = "Register_MobileDepositing_EmployeeInSign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRegister_MobileDepositing(int id)
        {
            var query = myappregister.tbl_Register_MobileDepositing.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_MobileDepositing(int id)
        {
            var query = myappregister.tbl_Register_MobileDepositing.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_MobileDepositing.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetMobileDepositing(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_MobileDepositing select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_MobileDepositing> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.EmployeeNumber != null && c.EmployeeNumber.Contains(param.sSearch.ToLower())
                                 ||
                                c.Remarks != null && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.TokenNumber != null && c.TokenNumber.Contains(param.sSearch.ToLower())
                                 ||
                                c.InSecurity != null && c.InSecurity.ToString().Contains(param.sSearch.ToLower())

                                ||
                                c.LocationId != null && c.LocationId.ToString().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.CustomUserId
                         //let u1 = myapp.tbl_User.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         //let u2 = myapp.tbl_OutSourceUser.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                                 u1.FirstName,
                                             c.EmployeeNumber,
                                              c.InSecurity,
                                              c.TokenNumber,
                                               c.RegisterDate!=null?c.RegisterDate.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                           c.Remarks,
                                              u.FirstName,
                                              c.ModifiedOn!=null?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelMobileDepositingRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_MobileDepositing select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Employee Number", typeof(string));
            products.Columns.Add("In Security", typeof(string));
            products.Columns.Add("Token Number", typeof(string));
            products.Columns.Add("Register Date", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();
                var u1 = myapp.tbl_User.Where(u2 => u2.CustomUserId == c.EmployeeNumber).SingleOrDefault();


                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    u1 != null ? (u1.FirstName) : "",
                    c.EmployeeNumber,
                    c.InSecurity,
                    c.TokenNumber,
                    c.RegisterDate.HasValue ? c.RegisterDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.Remarks,
                    createdby,
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=OccuranceRegister.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPDFMobileDepositingRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_MobileDepositing select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Employee Number", typeof(string));
            products.Columns.Add("In Security", typeof(string));
            products.Columns.Add("Token Number", typeof(string));
            products.Columns.Add("Register Date", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();
                var u1 = myapp.tbl_User.Where(u2 => u2.CustomUserId == c.EmployeeNumber).SingleOrDefault();


                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    u1 != null ? (u1.FirstName) : "",
                    c.EmployeeNumber,
                    c.InSecurity,
                    c.TokenNumber,
                    c.RegisterDate.HasValue ? c.RegisterDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.Remarks,
                    createdby,
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }

            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= MobileDepositing.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult MobileDepositingUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult SaveMobileDepositingUpdate(tbl_Register_MobileDepositing model)
        {
            if (model.SecurityOutSign != null && model.SecurityOutSign != "")
            {
                model.SecurityOutSign = model.SecurityOutSign.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.SecurityOutSign);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_SecurityOutSign_" + model.Id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }
            if (model.EmployeeOutSign != null && model.EmployeeOutSign != "")
            {
                model.EmployeeOutSign = model.EmployeeOutSign.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.EmployeeOutSign);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_EmployeeOutSign_" + model.Id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }
            var dbModel = myappregister.tbl_Register_MobileDepositing.Where(m => m.Id == model.Id).FirstOrDefault();
            //dbModel.OutDriverSign = "Register_VehicleParking_OutDriverSign_" + model.Id + ".png";
            dbModel.EmployeeOutSign = "Register_EmployeeOutSign_" + model.Id + ".png";
            dbModel.SecurityOutSign = "Register_SecurityOutSign_" + model.Id + ".png";
            dbModel.ModifiedOn = DateTime.Now;
            dbModel.OutSecurity = model.OutSecurity;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;

            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult VisitorRegister()
        {
            return View();
        }
        public ActionResult PatientVisits()
        {
            return View();
        }
        public ActionResult StaffMovementRegister()
        {
            return View();
        }
        public ActionResult NewVisitorRegister()
        {
            return View();
        }
        public ActionResult NewPatientVisits()
        {
            return View();
        }
        public ActionResult NewStaffMovementRegister()
        {
            return View();
        }

        public ActionResult NursingOutingUpdate(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        //public ActionResult SaveNursingOutingUpdate(tbl_Register_NursingOuting model)
        //{
        //    if (model.InSecuritySign != null && model.InSecuritySign != "")
        //    {
        //        model.InSecuritySign = model.InSecuritySign.Replace("data:image/png;base64,", String.Empty);
        //        var bytes = Convert.FromBase64String(model.InSecuritySign);
        //        string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_NursingOuting_InSecuritySign_" + model.Id + ".png");
        //        if (bytes.Length > 0)
        //        {
        //            using (var stream = new FileStream(file, FileMode.Create))
        //            {
        //                stream.Write(bytes, 0, bytes.Length);
        //                stream.Flush();
        //            }
        //        }
        //    }
        //    if (model.InEmployeeSign != null && model.InEmployeeSign != "")
        //    {
        //        model.InEmployeeSign = model.InEmployeeSign.Replace("data:image/png;base64,", String.Empty);
        //        var bytes = Convert.FromBase64String(model.InEmployeeSign);
        //        string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_NursingOuting_InEmployeeSign_" + model.Id + ".png");
        //        if (bytes.Length > 0)
        //        {
        //            using (var stream = new FileStream(file, FileMode.Create))
        //            {
        //                stream.Write(bytes, 0, bytes.Length);
        //                stream.Flush();
        //            }
        //        }
        //    }
        //    var dbModel = myappregister.tbl_Register_NursingOuting.Where(m => m.Id == model.Id).FirstOrDefault();
        //    //dbModel.OutDriverSign = "Register_VehicleParking_OutDriverSign_" + model.Id + ".png";
        //    dbModel.InEmployeeSign = "Register_NursingOuting_InEmployeeSign_" + model.Id + ".png";
        //    dbModel.InSecuritySign = "Register_NursingOuting_InSecuritySign_" + model.Id + ".png";
        //    dbModel.ModifiedOn = DateTime.Now;
        //    dbModel.InSecurity = model.InSecurity;
        //    dbModel.ModifiedBy = User.Identity.Name;
        //    dbModel.InDate = DateTime.Now;

        //    myappregister.SaveChanges();
        //    return Json("Success", JsonRequestBehavior.AllowGet);
        //}
        public ActionResult NewNursingOuting()
        {
            return View();
        }

        public ActionResult SaveNewNursingOuting(tbl_Register_NursingOuting model)
        {
            model.CreatedBy = User.Identity.Name;
            string SecurityOutSign = model.OutSecuritySign;

            model.OutSecuritySign = "";

            string EmployeeOutSign = model.OutEmployeeSign;

            model.OutEmployeeSign = "";

            model.OutDate = DateTime.Now;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_NursingOuting.Add(model);
            myappregister.SaveChanges();
            if (SecurityOutSign != null && SecurityOutSign != "")
            {
                try
                {
                    SecurityOutSign = SecurityOutSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(SecurityOutSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_NursingOuting_SecurityOutSign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.OutSecuritySign = "Register_NursingOuting_SecurityOutSign_" + model.Id + ".png";
                }
                catch { }
            }
            if (EmployeeOutSign != null && EmployeeOutSign != "")
            {
                try
                {
                    EmployeeOutSign = EmployeeOutSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(EmployeeOutSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_NursingOuting_OutEmployeeSign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.OutEmployeeSign = "Register_NursingOuting_OutEmployeeSign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRegister_NursingOuting(int id)
        {
            var query = myappregister.tbl_Register_NursingOuting.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_NursingOuting(int id)
        {
            var query = myappregister.tbl_Register_NursingOuting.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_NursingOuting.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetNursingOuting(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_NursingOuting select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_NursingOuting> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.EmployeeNo != null && c.EmployeeNo.Contains(param.sSearch.ToLower())
                                 ||
                                c.OutRemarks != null && c.OutRemarks.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.ReasonForGoing != null && c.ReasonForGoing.Contains(param.sSearch.ToLower())
                                 ||
                                c.InSecurity != null && c.InSecurity.ToString().Contains(param.sSearch.ToLower())

                                ||
                                c.LocationId != null && c.LocationId.ToString().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         join u1 in myapp.tbl_User on c.EmployeeNo equals u1.CustomUserId
                         //let u1 = myapp.tbl_User.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         //let u2 = myapp.tbl_OutSourceUser.Where(u2 => u2.EmpId == c.EmployeeNumber).SingleOrDefault()
                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                                 u1.FirstName,
                                             c.EmployeeNo,
                                              c.OutSecurity,
                                            GetReasonGoingOutstr(c),
                                               c.OutDate!=null?c.OutDate.Value.ToString("dd/MM/yyyy HH:mm tt"):"",
                                           c.OutRemarks,
                                           c.InSecurity,
                                           c.InDate!=null?c.InDate.Value.ToString("dd/MM/yyyy HH:mm tt"):"",
                                              u.FirstName,
                                              c.ModifiedOn!=null?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm tt"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public string GetReasonGoingOutstr(tbl_Register_NursingOuting m)
        {
            string str = m.ReasonForGoing;
            if (m.ReasonForGoing == "Leave" || m.ReasonForGoing == "Night Off Permission" || m.ReasonForGoing == "Permission")
            {
                if (m.LeaveFromDate != null)
                {
                    str = str + " - " + m.LeaveFromDate + " To " + m.LeaveToDate;
                }
            }
            else if (m.ReasonForGoing == "Duty")
            {
                str = str + " - " + m.ShiftName;
            }
            return str;
        }

        public ActionResult SaveNursingOutingUpdate(UserSignoffViewModel model)
        {
            if (model.signature != null && model.signature != "")
            {
                model.signature = model.signature.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.signature);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_NursingOuting_EmployeeInSign_" + model.id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }
            if (model.signature2 != null && model.signature2 != "")
            {
                model.signature2 = model.signature2.Replace("data:image/png;base64,", String.Empty);
                var bytes = Convert.FromBase64String(model.signature2);
                string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_NursingOuting_SecurityInSign_" + model.id + ".png");
                if (bytes.Length > 0)
                {
                    using (var stream = new FileStream(file, FileMode.Create))
                    {
                        stream.Write(bytes, 0, bytes.Length);
                        stream.Flush();
                    }
                }
            }

            var dbModel = myappregister.tbl_Register_NursingOuting.Where(m => m.Id == model.id).FirstOrDefault();
            dbModel.InSecuritySign = "Register_NursingOuting_SecurityInSign_" + model.id + ".png";
            dbModel.InEmployeeSign = "Register_NursingOuting_EmployeeInSign_" + model.id + ".png";
            dbModel.InSecurity = model.userempId;
            dbModel.InDate = DateTime.Now;
            dbModel.ModifiedBy = User.Identity.Name;
            dbModel.ModifiedOn = DateTime.Now;

            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelNursingOutingRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_MobileDepositing select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Employee Number", typeof(string));
            products.Columns.Add("In Security", typeof(string));
            products.Columns.Add("Token Number", typeof(string));
            products.Columns.Add("Register Date", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();
                var u1 = myapp.tbl_User.Where(u2 => u2.CustomUserId == c.EmployeeNumber).SingleOrDefault();


                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    u1 != null ? (u1.FirstName) : "",
                    c.EmployeeNumber,
                    c.InSecurity,
                    c.TokenNumber,
                    c.RegisterDate.HasValue ? c.RegisterDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.Remarks,
                    createdby,
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=OccuranceRegister.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPDFNursingOutingRegister(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_MobileDepositing select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Employee Number", typeof(string));
            products.Columns.Add("In Security", typeof(string));
            products.Columns.Add("Token Number", typeof(string));
            products.Columns.Add("Register Date", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();
                var u1 = myapp.tbl_User.Where(u2 => u2.CustomUserId == c.EmployeeNumber).SingleOrDefault();


                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    u1 != null ? (u1.FirstName) : "",
                    c.EmployeeNumber,
                    c.InSecurity,
                    c.TokenNumber,
                    c.RegisterDate.HasValue ? c.RegisterDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.Remarks,
                    createdby,
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }

            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= MobileDepositing.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ManageRounds()
        {
            return View();
        }
        public ActionResult NewRounds()
        {
            return View();
        }

        public ActionResult SaveNewRounds(tbl_Register_Round model)
        {
            model.CreatedBy = User.Identity.Name;
            string AttenderSign = model.AttenderSign;

            model.AttenderSign = "";
            model.CreatedOn = DateTime.Now;
            model.ModifiedBy = User.Identity.Name;
            model.ModifiedOn = DateTime.Now;
            myappregister.tbl_Register_Round.Add(model);
            myappregister.SaveChanges();
            if (AttenderSign != null && AttenderSign != "")
            {
                try
                {
                    AttenderSign = AttenderSign.Replace("data:image/png;base64,", String.Empty);
                    var bytes = Convert.FromBase64String(AttenderSign);
                    string file = Path.Combine(Server.MapPath("~/Documents/"), "Register_Round_AttenderSign_" + model.Id + ".png");
                    if (bytes.Length > 0)
                    {
                        using (var stream = new FileStream(file, FileMode.Create))
                        {
                            stream.Write(bytes, 0, bytes.Length);
                            stream.Flush();
                        }
                    }
                    model.AttenderSign = "Register_Round_AttenderSign_" + model.Id + ".png";
                }
                catch { }
            }
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetRegister_Round(int id)
        {
            var query = myappregister.tbl_Register_Round.Where(l => l.Id == id).SingleOrDefault();

            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteRegister_Round(int id)
        {
            var query = myappregister.tbl_Register_Round.Where(l => l.Id == id).SingleOrDefault();
            myappregister.tbl_Register_Round.Remove(query);
            myappregister.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        public ActionResult AjaxGetRegisterRound(JQueryDataTableParamModel param)
        {
            var query = (from d in myappregister.tbl_Register_Round select d).OrderByDescending(l => l.Id).ToList();

            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Register_Round> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.MrNo != null && c.MrNo.Contains(param.sSearch.ToLower())
                                 ||
                                c.IpNo != null && c.IpNo.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.NameOfPatient != null && c.NameOfPatient.Contains(param.sSearch.ToLower())
                                 ||
                                c.Remarks != null && c.Remarks.Contains(param.sSearch.ToLower())

                                ||
                                c.ModifiedBy != null && c.ModifiedBy.ToString().Contains(param.sSearch.ToLower())
                                ||
                                c.CreatedOn != null && c.CreatedOn.Value.ToString("dd/MM/yyyy").Contains(param.sSearch.ToLower())
                               );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         join l in myapp.tbl_Location on c.LocationId equals l.LocationId
                         //join u1 in myapp.tbl_User on c.EmployeeNumber equals u1.EmpId
                         join u in myapp.tbl_User on c.CreatedBy equals u.CustomUserId
                         select new[] {
                                              c.Id.ToString(),
                                               l.LocationName,
                                             c.IpNo,
                                              c.MrNo,
                                           c.NameOfPatient,
                                               c.CreatedOn!=null?c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm tt"):"",
                                           c.Remarks,
                                           c.ActionTaken,
                                  
                                              u.FirstName,
                                              c.ModifiedOn!=null?c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm tt"):"",
                                              c.Id.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelRegister_Round(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_MobileDepositing select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Employee Number", typeof(string));
            products.Columns.Add("In Security", typeof(string));
            products.Columns.Add("Token Number", typeof(string));
            products.Columns.Add("Register Date", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();
                var u1 = myapp.tbl_User.Where(u2 => u2.CustomUserId == c.EmployeeNumber).SingleOrDefault();


                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    u1 != null ? (u1.FirstName) : "",
                    c.EmployeeNumber,
                    c.InSecurity,
                    c.TokenNumber,
                    c.RegisterDate.HasValue ? c.RegisterDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.Remarks,
                    createdby,
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=OccuranceRegister.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPDFRegister_Round(string fromDate, string toDate)
        {
            var query = (from d in myappregister.tbl_Register_MobileDepositing select d).OrderByDescending(l => l.Id).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }

            var products = new System.Data.DataTable("Key");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("Employee", typeof(string));
            products.Columns.Add("Employee Number", typeof(string));
            products.Columns.Add("In Security", typeof(string));
            products.Columns.Add("Token Number", typeof(string));
            products.Columns.Add("Register Date", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            foreach (var c in query)
            {
                var createdby = (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var).ToList();
                var modifiedBy = (from var in myapp.tbl_User where var.CustomUserId == c.ModifiedBy select var).ToList();
                var u1 = myapp.tbl_User.Where(u2 => u2.CustomUserId == c.EmployeeNumber).SingleOrDefault();


                products.Rows.Add(
                    c.Id.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                    u1 != null ? (u1.FirstName) : "",
                    c.EmployeeNumber,
                    c.InSecurity,
                    c.TokenNumber,
                    c.RegisterDate.HasValue ? c.RegisterDate.Value.ToString("dd/MM/yyyy HH:mm") : "",
                    c.Remarks,
                    createdby,
                   c.ModifiedOn.HasValue ? c.ModifiedOn.Value.ToString("dd/MM/yyyy HH:mm") : ""
                );
            }

            Document document = new Document();
            //PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
            PdfWriter writer = PdfWriter.GetInstance(document, Response.OutputStream);

            document.Open();

            PdfPTable table = new PdfPTable(products.Columns.Count);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell(new Phrase("Products"));
            iTextSharp.text.Font font5 = iTextSharp.text.FontFactory.GetFont(FontFactory.HELVETICA, 4);
            iTextSharp.text.Font myFont = iTextSharp.text.FontFactory.GetFont(FontFactory.TIMES, 5, BaseColor.DARK_GRAY);
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
            Response.AddHeader("content-disposition", "attachment; filename= MobileDepositing.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}