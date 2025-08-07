using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    public class DoulaController : Controller
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Doula
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult SpecificDoula()
        {
            return View();
        }
        public ActionResult AjaxGetDoula(JQueryDataTableParamModel param)
        {
            var query = (from d in myapp.tbl_Doula select d).OrderByDescending(l => l.DoulaId).ToList();
            if (param.FormType != null && param.FormType != "")
            {
                query = query.Where(l => l.SpecificDoula == param.FormType).ToList();
            }
            if (param.locationid != null && param.locationid > 0)
            {
                query = query.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                query = query.Where(x => x.Date.Value.Date >= FromDate.Date).Where(x => x.Date.Value.Date <= ToDate.Date).ToList();
            }
            IEnumerable<tbl_Doula> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.DoulaId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.SpecificDoula != null && c.SpecificDoula.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.PatientName != null && c.PatientName.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.Mobile != null && c.Mobile.ToLower().Contains(param.sSearch.ToLower())
                                 ||
                                c.MrNumber != null && c.MrNumber.ToLower().Contains(param.sSearch.ToLower())

                                ||
                                c.Remarks != null && c.Remarks.ToLower().Contains(param.sSearch.ToLower())
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
                         join u in myapp.tbl_User on c.UpdatedBy equals u.CustomUserId
                         select new[] {
                                              c.DoulaId.ToString(),

                                              l.LocationName,
                                              c.Date.HasValue?c.Date.Value.ToString("dd/MM/yyyy"):"",

                                              c.MrNumber,
                                              c.PatientName,
                                              c.Mobile,
                                              c.SpecificDoula,
                                              c.PaymentStatus,
                                              c.UpdatedBy,
                                              c.UpdatedOn.HasValue?c.UpdatedOn.Value.ToString("dd/MM/yyyy"):"",
                                              c.DoulaId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelDoula(int locationid, string fromDate, string toDate)
        {
            var query = (from d in myapp.tbl_Doula select d).OrderBy(l => l.DoulaId).ToList();
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                query = query.Where(x => x.Date.Value.Date >= FromDate.Date).Where(x => x.Date.Value.Date <= ToDate.Date).ToList();
            }
            if (locationid != null && locationid != 0)
            {
                query = query.Where(l => l.LocationId == locationid).ToList();
            }
            var products = new System.Data.DataTable("Doula");
            products.Columns.Add("Sl No", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("EDD Date", typeof(string));
            products.Columns.Add("Mr No", typeof(string));
            products.Columns.Add("Patient Name", typeof(string));
            products.Columns.Add("Mobile No", typeof(string));
            products.Columns.Add("Specific Doula", typeof(string));
            products.Columns.Add("Payment Status", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            int rownumber = 1;
            foreach (var c in query)
            {
                products.Rows.Add(
                    rownumber.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                      c.Date.HasValue ? c.Date.Value.ToString("dd/MM/yyyy") : "",
                    c.MrNumber,
                    c.PatientName,
                    c.Mobile,
                    c.SpecificDoula,
                    c.PaymentStatus,
                    c.Remarks,
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.UpdatedBy select var.FirstName).SingleOrDefault(),
                    c.UpdatedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
                rownumber++;

            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=DoulaSlots.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportPdfDoula(int locationid, string fromDate, string toDate)
        {
            var query = (from d in myapp.tbl_Doula select d).OrderBy(l => l.DoulaId).ToList();
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
            var products = new System.Data.DataTable("Doula");
            products.Columns.Add("Sl No", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("EDD Date", typeof(string));
            products.Columns.Add("Mr No", typeof(string));
            products.Columns.Add("Patient Name", typeof(string));
            products.Columns.Add("Mobile No", typeof(string));
            products.Columns.Add("Specific Doula", typeof(string));
            products.Columns.Add("Payment Status", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Created On", typeof(string));
            products.Columns.Add("Created By", typeof(string));
            products.Columns.Add("Modified By", typeof(string));
            products.Columns.Add("Modified On", typeof(string));
            int rownumber = 1;
            foreach (var c in query)
            {
                products.Rows.Add(
                  rownumber.ToString(),
                    (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).SingleOrDefault(),
                      c.Date.HasValue ? c.Date.Value.ToString("dd/MM/yyyy") : "",
                    c.MrNumber,
                    c.PatientName,
                    c.Mobile,
                    c.SpecificDoula,
                    c.PaymentStatus,
                    c.Remarks,
                    c.CreatedOn.Value.ToString("dd/MM/yyyy HH:mm"),
                     (from var in myapp.tbl_User where var.CustomUserId == c.CreatedBy select var.FirstName).SingleOrDefault(),
                     (from var in myapp.tbl_User where var.CustomUserId == c.UpdatedBy select var.FirstName).SingleOrDefault(),
                    c.UpdatedOn.Value.ToString("dd/MM/yyyy HH:mm")
                );
                rownumber++;
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
            Response.AddHeader("content-disposition", "attachment; filename= DoulaSlots.pdf");
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveDoual(DoulaViewModel model)
        {
            try
            {
                DateTime dt = DateTime.Now;
                if (model.Date != null && model.Date != "")
                    dt = ProjectConvert.ConverDateStringtoDatetime(model.Date, "dd/MM/yyyy");

                var listcount = myapp.tbl_Doula.Where(l => l.Date.Value.Year == dt.Year && l.Date.Value.Month == dt.Month).Count();
                if (listcount < 12)
                {
                    tbl_Doula dbModel = new tbl_Doula();
                    dbModel.EmailId = model.EmailId;
                    dbModel.IpNo = model.IpNo;
                    dbModel.LocationId = model.LocationId;
                    dbModel.Mobile = model.Mobile;
                    dbModel.MrNumber = model.MrNumber;
                    dbModel.PatientName = model.PatientName;
                    dbModel.PaymentStatus = model.PaymentStatus;
                    dbModel.Remarks = model.Remarks;
                    dbModel.SpecificDoula = model.SpecificDoula;
                    dbModel.Status = model.Status;
                    dbModel.Date = ProjectConvert.ConverDateStringtoDatetime(model.Date, "dd/MM/yyyy");
                    dbModel.UpdatedOn = DateTime.Now;
                    dbModel.UpdatedBy = User.Identity.Name;
                    dbModel.CreatedBy = User.Identity.Name;
                    dbModel.CreatedOn = DateTime.Now;
                    myapp.tbl_Doula.Add(dbModel);
                    myapp.SaveChanges();
                }
                else
                {
                    return Json("Maximum 12 slots can be allowed per month.", JsonRequestBehavior.AllowGet);
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error " + ex.Message, JsonRequestBehavior.AllowGet);
            }

        }

        public ActionResult DeleteDoual(int id)
        {
            var details = myapp.tbl_Doula.Where(l => l.DoulaId == id).SingleOrDefault();
            if (details != null)
            {
                myapp.tbl_Doula.Remove(details);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveSpecificDoula(string name)
        {
            var details = myapp.tbl_Doula_Specific.Where(l => l.SpecificDoula == name).SingleOrDefault();
            if (details == null)
            {
                tbl_Doula_Specific m = new tbl_Doula_Specific();
                m.SpecificDoula = name;
                m.CreatedBy = User.Identity.Name;
                m.CreatedOn = DateTime.Now;
                m.IsActive = true;
                myapp.tbl_Doula_Specific.Add(m);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetSpecificDoulas()
        {
            var list = (from m in myapp.tbl_Doula_Specific select m).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteSpecificDoulas(int id)
        {
            var details = myapp.tbl_Doula_Specific.Where(l => l.Id == id).SingleOrDefault();
            if (details != null)
            {
                myapp.tbl_Doula_Specific.Remove(details);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}