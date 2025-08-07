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
using WebApplicationHsApp.Models;
using WebApplicationHsApp.OracleInterface;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class ConcessionFormController : Controller
    {
        private readonly MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: ConcessionForm
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NewConcession(int id = 0)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult PrintIPD(int id)
        {
            var cpxmodelfull = GetConcessionForPrint(id);
            return View(cpxmodelfull);
        }
        public ActionResult PrintOPD(int id)
        {
            var cpxmodelfull = GetConcessionForPrint(id);
            return View(cpxmodelfull);
        }
        public ActionResult AjaxGetConcessions(JQueryDataTableParamModel param)
        {
            List<tbl_Concession> squery = myapp.tbl_Concession.ToList();
            if (param.locationid != null && param.locationid > 0)
            {
                squery = squery.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.category != null && param.category != "")
            {
                squery = squery.Where(l => l.Category == param.category).ToList();
            }
            if (param.SubCategory != null && param.SubCategory != "")
            {
                squery = squery.Where(l => l.SubCategory == param.SubCategory).ToList();
            }
            if (param.Emp != null && param.Emp != "")
            {
                squery = squery.Where(l => l.ConcessionAdvised == param.Emp).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                DateTime dtfrmdate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                DateTime dttodate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                if (dtfrmdate == dttodate)
                {
                    dttodate = dttodate.AddDays(1);
                }
                squery = (from t in squery
                          where t.CreatedOn.Value >= dtfrmdate && t.CreatedOn.Value <= dttodate
                          select t).ToList();
            }
            List<ConcessionViewModel> query = (from m in squery
                                               join user in myapp.tbl_User on m.CreatedBy equals user.CustomUserId
                                               join l in myapp.tbl_Location on m.LocationId equals l.LocationId
                                               select new ConcessionViewModel
                                               {
                                                   ConcessionId = m.ConcessionId,
                                                   Category = m.Category + " " + m.SubCategory,
                                                   ConcessionAdvised = m.ConcessionAdvised,
                                                   ConcessionType = m.ConcessionType,
                                                   Consultation = m.Consultation,
                                                   Investigations = m.Investigations,
                                                   IPNumber = m.IPNumber,
                                                   Justification = m.Justification,
                                                   Package = m.Package,
                                                   PaidByPatient = m.PaidByPatient,
                                                   PatientName = m.PatientName,
                                                   ProcedureName = m.ProcedureName,
                                                   Remarks = m.Remarks,
                                                   Scan = m.Scan,
                                                   Service = m.Service,
                                                   TotalBillAmount = m.TotalBillAmount,
                                                   TotalConcessionAmount = m.TotalConcessionAmount,
                                                   DateOfSubmit = ProjectConvert.ConverDateTimeToString(m.DateOfSubmit.Value),
                                                   BillNo = m.BillNo,
                                                   Status = m.Status,
                                                   ApproverStatus = m.ApproverStatus,
                                                   ApproverComments = m.ApproverComments,
                                                   DocumentName = m.Document,
                                                   Approver = m.ModifiedBy,
                                                   LocationId = m.LocationId.HasValue ? m.LocationId.Value : 0,
                                                   LocationName = l.LocationName
                                               }).ToList();
            query = query.OrderByDescending(l => l.ConcessionId).ToList();

            IEnumerable<ConcessionViewModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Category.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.ConcessionAdvised != null && c.ConcessionAdvised.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Consultation != null && c.Consultation.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Investigations != null && c.Investigations.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.IPNumber != null && c.IPNumber.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Justification != null && c.Justification.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.PatientName != null && c.PatientName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.BillNo != null && c.BillNo.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.LocationName != null && c.LocationName.ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<ConcessionViewModel> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           join a in myapp.tbl_User on c.Approver equals a.CustomUserId
                                           select new[] {
                                              c.ConcessionId.ToString(),
                                              c.LocationName,
                                              c.IPNumber,
                                              c.PatientName,
                                              c.DateOfSubmit,
                                              c.Category,
                                              c.ConcessionAdvised,
                                              c.Justification,
                                              c.BillNo,
                                              c.TotalBillAmount,
                                              c.TotalConcessionAmount,
                                              c.ConcessionType,
                                              a.FirstName+" "+a.LastName,
                                              c.ApproverStatus,
                                              c.DocumentName,
                                              c.ConcessionId.ToString(),
                                              };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Concessions_ExportToExcel(string FromDate, string ToDate, int LocationId = 0, string Category = "", string SubCategory = "", string ConcessionAdvised = "")
        {
            List<tbl_Concession> squery = myapp.tbl_Concession.ToList();
            if (LocationId != null && LocationId != 0)
            {
                squery = squery.Where(l => l.LocationId == LocationId).ToList();
            }
            if (FromDate != null && FromDate != "" && ToDate != null && ToDate != "")
            {
                DateTime dtfrmdate = ProjectConvert.ConverDateStringtoDatetime(FromDate);
                DateTime dttodate = ProjectConvert.ConverDateStringtoDatetime(ToDate);
                if (dtfrmdate == dttodate)
                {
                    dttodate = dttodate.AddDays(1);
                }
                squery = (from t in squery
                          where t.CreatedOn.Value >= dtfrmdate && t.CreatedOn.Value <= dttodate
                          select t).ToList();
            }
            if (Category != null && Category != "")
            {
                squery = squery.Where(l => l.Category == Category).ToList();
            }
            if (SubCategory != null && SubCategory != "")
            {
                squery = squery.Where(l => l.SubCategory == SubCategory).ToList();
            }
            if (ConcessionAdvised != null && ConcessionAdvised != "")
            {
                squery = squery.Where(l => l.ConcessionAdvised == ConcessionAdvised).ToList();
            }
            List<ConcessionViewModel> query = (from m in squery
                                               join user in myapp.tbl_User on m.CreatedBy equals user.CustomUserId
                                               join a in myapp.tbl_User on m.ModifiedBy equals a.CustomUserId
                                               join l in myapp.tbl_Location on m.LocationId equals l.LocationId
                                               select new ConcessionViewModel
                                               {
                                                   ConcessionId = m.ConcessionId,
                                                   Category = m.Category,
                                                   SubCategory = m.SubCategory,
                                                   ConcessionAdvised = m.ConcessionAdvised,
                                                   ConcessionType = m.ConcessionType,
                                                   Consultation = m.Consultation,
                                                   Investigations = m.Investigations,
                                                   IPNumber = m.IPNumber,
                                                   Justification = m.Justification,
                                                   Package = m.Package,
                                                   PaidByPatient = m.PaidByPatient,
                                                   PatientName = m.PatientName,
                                                   ProcedureName = m.ProcedureName,
                                                   Remarks = m.Remarks,
                                                   Scan = m.Scan,
                                                   Service = m.Service,
                                                   TotalBillAmount = m.TotalBillAmount,
                                                   TotalConcessionAmount = m.TotalConcessionAmount,
                                                   DateOfSubmit = ProjectConvert.ConverDateTimeToString(m.DateOfSubmit.Value),
                                                   BillNo = m.BillNo,
                                                   Status = m.Status,
                                                   ApproverStatus = m.ApproverStatus,
                                                   ApproverComments = m.ApproverComments,
                                                   DocumentName = m.Document,
                                                   Approver = a.FirstName + " " + a.LastName,
                                                   LocationName = l.LocationName
                                               }).ToList();
            //query = query.OrderByDescending(l => l.ConcessionId).ToList();


            System.Data.DataTable products = new System.Data.DataTable("MyTasksDataTable");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("MR/IP No", typeof(string));
            products.Columns.Add("Patient Name", typeof(string));
            products.Columns.Add("Date Of Submit", typeof(string));
            products.Columns.Add("Category", typeof(string));
            products.Columns.Add("Sub Category", typeof(string));
            products.Columns.Add("Concession Advised", typeof(string));
            products.Columns.Add("Justification", typeof(string));
            products.Columns.Add("Bill No", typeof(string));
            products.Columns.Add("Total BillAmount", typeof(string));
            products.Columns.Add("Total Concession Amount", typeof(string));
            products.Columns.Add("Concession Type", typeof(string));
            products.Columns.Add("Approver", typeof(string));
            products.Columns.Add("Status", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(c.ConcessionId,
                    c.LocationName,
                    c.IPNumber,
                     c.PatientName,
                    c.DateOfSubmit,
                    c.Category,
                    c.SubCategory,
                    c.ConcessionAdvised,
                    c.Justification,
                    c.BillNo,
                    c.TotalBillAmount,
                    c.TotalConcessionAmount,
                    c.ConcessionType,
                   c.Approver,
                   c.Status
                );
                //}

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
            string filename = "Concessions_Data.xls";
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
        public ActionResult SaveConcession(ConcessionViewModel model, HttpPostedFileBase[] Upload)
        {
            tbl_Concession dbmodel = new tbl_Concession();
            if (model.ConcessionId > 0)
            {
                dbmodel = myapp.tbl_Concession.Where(l => l.ConcessionId == model.ConcessionId).SingleOrDefault();
                dbmodel.DateOfAdmission = model.DateOfAdmission;
                dbmodel.DateOfDischarge = model.DateOfDischarge;
                dbmodel.Pharmacy = model.Pharmacy;
                dbmodel.NST = model.NST;
                dbmodel.ApproverComments = "";
                dbmodel.ModifiedBy = User.Identity.Name;
                dbmodel.ModifiedOn = DateTime.Now;
                dbmodel.Category = model.Category;
                dbmodel.ConcessionAdvised = model.ConcessionAdvised;
                dbmodel.ConcessionType = model.ConcessionType;
                dbmodel.Consultation = model.Consultation;
                dbmodel.Investigations = model.Investigations;
                dbmodel.IPNumber = model.IPNumber;
                dbmodel.Justification = model.Justification;
                dbmodel.Package = model.Package;
                dbmodel.PaidByPatient = model.PaidByPatient;
                dbmodel.PatientName = model.PatientName;
                dbmodel.ProcedureName = model.ProcedureName;
                dbmodel.Remarks = model.Remarks;
                dbmodel.Scan = model.Scan;
                dbmodel.Service = model.Service;
                dbmodel.TotalBillAmount = model.TotalBillAmount;
                dbmodel.TotalConcessionAmount = model.TotalConcessionAmount;

                dbmodel.BillNo = model.BillNo;
                dbmodel.LocationId = model.LocationId;
                dbmodel.SubCategory = model.SubCategory;
                dbmodel.TotalConcessionPercentage = model.TotalConcessionPercentage;
                if (model.Validity != null && model.Validity != "")
                {
                    dbmodel.Validity = ProjectConvert.ConverDateStringtoDatetime(model.Validity);
                }
                myapp.SaveChanges();
            }
            else
            {
                dbmodel = new tbl_Concession
                {
                    DateOfAdmission = model.DateOfAdmission,
                    DateOfDischarge = model.DateOfDischarge,
                    Pharmacy = model.Pharmacy,
                    NST = model.NST,
                    ApproverComments = "",
                    ApproverStatus = "Waiting",
                    CreatedBy = User.Identity.Name,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    ModifiedBy = User.Identity.Name,
                    ModifiedOn = DateTime.Now,
                    Status = "Approval Waiting",
                    Category = model.Category,
                    ConcessionAdvised = model.ConcessionAdvised,
                    ConcessionType = model.ConcessionType,
                    Consultation = model.Consultation,
                    Investigations = model.Investigations,
                    IPNumber = model.IPNumber,
                    Justification = model.Justification,
                    Package = model.Package,
                    PaidByPatient = model.PaidByPatient,
                    PatientName = model.PatientName,
                    ProcedureName = model.ProcedureName,
                    Remarks = model.Remarks,
                    Scan = model.Scan,
                    Service = model.Service,
                    TotalBillAmount = model.TotalBillAmount,
                    TotalConcessionAmount = model.TotalConcessionAmount,
                    DateOfSubmit = DateTime.Now,
                    BillNo = model.BillNo,
                    LocationId = model.LocationId,
                    SubCategory = model.SubCategory,
                    TotalConcessionPercentage = model.TotalConcessionPercentage,

                };
                if (model.Validity != null && model.Validity != "")
                {
                    dbmodel.Validity = ProjectConvert.ConverDateStringtoDatetime(model.Validity);
                }
                myapp.tbl_Concession.Add(dbmodel);
                myapp.SaveChanges();
            }
            if (Upload != null && Upload.Length > 0)
            {
                string serverpath = Server.MapPath("~/ExcelUplodes/");
                foreach (HttpPostedFileBase file in Upload)
                {
                    if (file != null)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string guidid = "Concession_" + dbmodel.ConcessionId + "_doc_" + fileName;
                        string path = Path.Combine(serverpath, guidid);
                        file.SaveAs(path);
                        dbmodel.Document = guidid;
                        myapp.SaveChanges();
                    }
                }
            }
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel
            {
                fromemail = "it_helpdesk@fernandez.foundation"
            };
            mailmodel.toemail = "";
            mailmodel.subject = "New Concession " + dbmodel.ConcessionId + " - " + dbmodel.IPNumber + " " + dbmodel.PatientName + " " + dbmodel.TotalConcessionAmount;
            if (mailmodel.toemail != null && mailmodel.toemail != "")
            {
                mailmodel.body = GetEmailBody(dbmodel, false, "");
                mailmodel.filepath = "";
                mailmodel.fromname = "Concession Request Created";
                if (mailmodel.ccemail == null || mailmodel.ccemail == "")
                {
                    mailmodel.ccemail = "";
                }
                if (model.SendemailMd == "true")
                {
                    mailmodel.ccemail = mailmodel.ccemail + ",ilaiah.dongari@fernandez.foundation";
                }
                if (model.SendEmailTo != null && model.SendEmailTo != "")
                {
                    mailmodel.ccemail = mailmodel.ccemail + "," + model.SendEmailTo;
                }
                switch (dbmodel.LocationId)
                {
                    case 1:
                        mailmodel.ccemail = mailmodel.ccemail + ",drashraf@fernandez.foundation,manga_k@fernandez.foundation";
                        break;
                    case 2:
                        //mailmodel.ccemail = mailmodel.ccemail + ",joyce_j@fernandez.foundation,narsaraju.bollepally@fernandez.foundation";
                        mailmodel.ccemail = mailmodel.ccemail + ",joyce_j@fernandez.foundation,billing_hg@fernandez.foundation";
                        break;
                    //case 4:
                    //    mailmodel.ccemail = mailmodel.ccemail + ",";
                    //break;
                    case 5:
                        //mailmodel.ccemail = mailmodel.ccemail + ", swathi.b@fernandez.foundation,narsaraju.bollepally@fernandez.foundation";
                        mailmodel.ccemail = mailmodel.ccemail + ", joyce_j@fernandez.foundation,billing_sh@fernandez.foundation,drshamim.shaikh@fernandez.foundation";
                        break;
                    case 9:
                        //mailmodel.ccemail = mailmodel.ccemail + ",narsaraju.bollepally@fernandez.foundation";
                        mailmodel.ccemail = mailmodel.ccemail;
                        break;
                }
                mailmodel.ccemail = mailmodel.ccemail + ",chiefmedicaldirector@fernandez.foundation";

                cm.SendEmail(mailmodel);
                mailmodel.ccemail = "";
                //Send email to approvers
                switch (dbmodel.LocationId)
                {
                    case 1:
                        mailmodel.body = GetEmailBody(dbmodel, true, "1409");
                        mailmodel.toemail = "drashraf@fernandez.foundation";
                        break;
                    case 2:
                    case 5:
                        mailmodel.body = GetEmailBody(dbmodel, true, "4053");
                        mailmodel.toemail = "joyce_j@fernandez.foundation";
                        break;
                    case 4:
                        mailmodel.body = GetEmailBody(dbmodel, true, "2106");
                        mailmodel.toemail = "drrajshree.manukumar@fernandez.foundation";
                        break;
                    //case 5:
                    //    mailmodel.body = GetEmailBody(dbmodel, true, "5531");
                    //    mailmodel.toemail = "swathi.b@fernandez.foundation";
                    //    break;
                    case 9:
                        mailmodel.body = GetEmailBody(dbmodel, true, "5274");
                        mailmodel.toemail = "satyadurga_a@fernandez.foundation";
                        break;
                }
                //mailmodel.toemail = "ahmadali@fernandez.foundation";
                //mailmodel.ccemail = "ahmadali@fernandez.foundation,narsaraju.bollepally@fernandez.foundation";
                //if (mailmodel.ccemail != null && mailmodel.ccemail != "")
                //{
                //    mailmodel.ccemail += "," + model.SendEmailTo;
                //}
                cm.SendEmail(mailmodel);
                //mailmodel.body = GetEmailBody(dbmodel, true, "900074");
                //mailmodel.toemail = "drtara@fernandez.foundation";
                //cm.SendEmail(mailmodel);

            }
            return Json("Successfully Added", JsonRequestBehavior.AllowGet);
        }
        public string GetEmailBody(tbl_Concession dbmodel, bool isapproval, string approverempid)
        {
            string mailbody = "<p style='font-family:verdana;font-size:13px;'>Dear Sir, Please find below are the Concession details :</p>";
            mailbody += "<table style='width:60%;margin:auto;border:solid 1px #a1a2a3;'>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Concession Id</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.ConcessionId + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Date Of Submit</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.DateOfSubmit.Value.ToString("dd/MM/yyyy") + "</td></tr>";
            if (dbmodel.Validity != null)
            {
                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Validity</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.Validity.Value.ToString("dd/MM/yyyy") + "</td></tr>";
            }
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>IP Number</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.IPNumber + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Patient Name</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.PatientName + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Category</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.Category + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Sub Category</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.SubCategory + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Concession Advised</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.ConcessionAdvised + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Justification</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.Justification + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Total Bill Amount</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.TotalBillAmount + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Total Concession Amount</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.TotalConcessionAmount + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Total Concession Percentage</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.TotalConcessionPercentage + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Concession Type</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.ConcessionType + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Bill No</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.BillNo + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Approver Status</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.ApproverStatus + "</td></tr>";
            if (isapproval)
            {
                mailbody += "<tr><td style='border-bottom:solid 1px #ddd;font-size:12px;color:green;text-align:center;font-family:verdana;' colspan='2'><a href='https://infonet.fernandezhospital.com/ConcessionForm/ApproveFromEmail?id=" + dbmodel.ConcessionId + "&comments=Ok&user=" + approverempid + "'>Click here to Approve</a></td></tr>";
                mailbody += "<tr><td style='border-bottom:solid 1px #ddd;font-size:12px;color:red;text-align:center;font-family:verdana;' colspan='2'><a href='https://infonet.fernandezhospital.com/ConcessionForm/Reject?id=" + dbmodel.ConcessionId + "&comments=Not-Ok&user=" + approverempid + "'>Click here to Reject</a></td></tr>";
            }
            mailbody += "</table><br /><p style='font-family:verdana;font-size:12px;'>This is an auto generated mail,Don't reply to this.</p>";
            return mailbody;
        }
        public ActionResult Approve(int id, string comments, string user, string ConcessionAdvised, string Justification, string TotalBillAmount, string TotalConcessionAmount
            , string Package, string Service, string Consultation, string Scan, string Investigations, string ProcedureName)
        {
            var dbmodel = myapp.tbl_Concession.Where(l => l.ConcessionId == id).SingleOrDefault();
            if (ConcessionAdvised != null && ConcessionAdvised != "")
            {
                dbmodel.ConcessionAdvised = ConcessionAdvised;
            }
            if (Justification != null && Justification != "")
            {
                dbmodel.Justification = Justification;
            }
            if (TotalBillAmount != null && TotalBillAmount != "")
            {
                dbmodel.TotalBillAmount = TotalBillAmount;
            }
            if (TotalConcessionAmount != null && TotalConcessionAmount != "")
            {
                dbmodel.TotalConcessionAmount = TotalConcessionAmount;
            }
            if (Package != null && Package != "")
            {
                dbmodel.Package = Package;
            }
            if (Service != null && Service != "")
            {
                dbmodel.Service = Service;
            }
            if (Consultation != null && Consultation != "")
            {
                dbmodel.Consultation = Consultation;
            }
            if (Scan != null && Scan != "")
            {
                dbmodel.Scan = Scan;
            }
            if (Investigations != null && Investigations != "")
            {
                dbmodel.Investigations = Investigations;
            }
            if (ProcedureName != null && ProcedureName != "")
            {
                dbmodel.ProcedureName = ProcedureName;
            }
            dbmodel.ApproverComments = comments;
            dbmodel.ApproverStatus = "Approved";
            dbmodel.Status = "Approved";
            dbmodel.ModifiedOn = DateTime.Now;
            dbmodel.ModifiedBy = user;
            myapp.SaveChanges();
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel
            {
                fromemail = "it_helpdesk@fernandez.foundation"
            };
            mailmodel.toemail = "";
            mailmodel.subject = "The Concession Request " + dbmodel.ConcessionId + " - " + dbmodel.IPNumber + " " + dbmodel.PatientName + " " + dbmodel.TotalConcessionAmount + " has approved";
            if (mailmodel.toemail != null && mailmodel.toemail != "")
            {
                mailmodel.body = GetEmailBody(dbmodel, false, "");
                mailmodel.filepath = "";
                mailmodel.fromname = "Concession Request Approved";
                if (mailmodel.ccemail == null || mailmodel.ccemail == "")
                {
                    //mailmodel.ccemail = "chiefmedicaldirector@fernandez.foundation,narsaraju.bollepally@fernandez.foundation";
                    mailmodel.ccemail = "chiefmedicaldirector@fernandez.foundation";
                }
                switch (dbmodel.LocationId)
                {
                    case 1:
                        mailmodel.ccemail = mailmodel.ccemail + ",drashraf@fernandez.foundation,manga_k@fernandez.foundation";
                        break;
                    case 2:
                        mailmodel.ccemail = mailmodel.ccemail + ",joyce_j@fernandez.foundation,billing_hg@fernandez.foundation";
                        break;
                    //case 4:
                    //    mailmodel.ccemail = mailmodel.ccemail + ",";
                    //break;
                    case 5:
                        mailmodel.ccemail = mailmodel.ccemail + ", joyce_j@fernandez.foundation,billing_sh@fernandez.foundation,drshamim.shaikh@fernandez.foundation";
                        break;
                    case 4:
                        mailmodel.ccemail = mailmodel.ccemail + ",drrajshree.manukumar@fernandez.foundation";
                        break;
                    case 9:
                        mailmodel.ccemail = mailmodel.ccemail + ",satyadurga_a@fernandez.foundation";
                        break;
                }
                //mailmodel.toemail = "ahmadali@fernandez.foundation";
                //mailmodel.ccemail = "ahmadali@fernandez.foundation";
                cm.SendEmail(mailmodel);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ApproveFromEmail(int id, string comments, string user)
        {
            var dbmodel = myapp.tbl_Concession.Where(l => l.ConcessionId == id).SingleOrDefault();

            dbmodel.ApproverComments = comments;
            dbmodel.ApproverStatus = "Approved";
            dbmodel.Status = "Approved";
            dbmodel.ModifiedOn = DateTime.Now;
            dbmodel.ModifiedBy = user;
            myapp.SaveChanges();
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel
            {
                fromemail = "it_helpdesk@fernandez.foundation"
            };
            mailmodel.toemail = "";
            mailmodel.subject = "The Concession Request " + dbmodel.ConcessionId + " - " + dbmodel.IPNumber + " " + dbmodel.PatientName + " " + dbmodel.TotalConcessionAmount + " has approved";
            if (mailmodel.toemail != null && mailmodel.toemail != "")
            {
                mailmodel.body = GetEmailBody(dbmodel, false, "");
                mailmodel.filepath = "";
                mailmodel.fromname = "Concession Request has Approved";
                if (mailmodel.ccemail == null || mailmodel.ccemail == "")
                {
                    //mailmodel.ccemail = "chiefmedicaldirector@fernandez.foundation,narsaraju.bollepally@fernandez.foundation";
                    mailmodel.ccemail = "chiefmedicaldirector@fernandez.foundation";
                }
                switch (dbmodel.LocationId)
                {
                    case 1:
                        mailmodel.ccemail = mailmodel.ccemail + ",drashraf@fernandez.foundation,manga_k@fernandez.foundation";
                        break;
                    case 2:
                        mailmodel.ccemail = mailmodel.ccemail + ",joyce_j@fernandez.foundation,billing_hg@fernandez.foundation";
                        break;
                    //case 4:
                    //    mailmodel.ccemail = mailmodel.ccemail + ",";
                    //break;
                    case 5:
                        mailmodel.ccemail = mailmodel.ccemail + ", joyce_j@fernandez.foundation,billing_sh@fernandez.foundation,drshamim.shaikh@fernandez.foundation";
                        break;
                    case 4:
                        mailmodel.ccemail = mailmodel.ccemail + ",drrajshree.manukumar@fernandez.foundation";
                        break;
                    case 9:
                        mailmodel.ccemail = mailmodel.ccemail + ",satyadurga_a@fernandez.foundation";
                        break;
                }
                //mailmodel.toemail = "ahmadali@fernandez.foundation";
                //mailmodel.ccemail = "ahmadali@fernandez.foundation";
                cm.SendEmail(mailmodel);
            }
            return Json("Successfully approved the request", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Reject(int id, string comments, string user)
        {
            var dbmodel = myapp.tbl_Concession.Where(l => l.ConcessionId == id).SingleOrDefault();
            dbmodel.ApproverComments = comments;
            dbmodel.ApproverStatus = "Rejected";
            dbmodel.Status = "Rejected";
            dbmodel.ModifiedOn = DateTime.Now;
            dbmodel.ModifiedBy = user;
            myapp.SaveChanges();
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel
            {
                fromemail = "it_helpdesk@fernandez.foundation"
            };
            mailmodel.toemail = "";
            mailmodel.subject = "The Concession Request " + dbmodel.ConcessionId + " - " + dbmodel.IPNumber + " " + dbmodel.PatientName + " " + dbmodel.TotalConcessionAmount + " has Rejected";
            if (mailmodel.toemail != null && mailmodel.toemail != "")
            {
                mailmodel.body = GetEmailBody(dbmodel, false, "");
                mailmodel.filepath = "";
                mailmodel.fromname = "Concession Request has Rejected";
                if (mailmodel.ccemail == null || mailmodel.ccemail == "")
                {
                    mailmodel.ccemail = "chiefmedicaldirector@fernandez.foundation,ilaiah.dongari@fernandez.foundation";
                }
                switch (dbmodel.LocationId)
                {
                    case 1:
                        mailmodel.ccemail = mailmodel.ccemail + ",drashraf@fernandez.foundation,manga_k@fernandez.foundation";
                        break;
                    case 2:
                        mailmodel.ccemail = mailmodel.ccemail + ",joyce_j@fernandez.foundation";
                        break;
                    //case 4:
                    //    mailmodel.ccemail = mailmodel.ccemail + ",";
                    //break;
                    case 5:
                        mailmodel.ccemail = mailmodel.ccemail + ", joyce_j@fernandez.foundation,drshamim.shaikh@fernandez.foundation";
                        break;
                    case 4:
                        mailmodel.ccemail = mailmodel.ccemail + ",drrajshree.manukumar@fernandez.foundation";
                        break;
                    case 9:
                        mailmodel.ccemail = mailmodel.ccemail + ",satyadurga_a@fernandez.foundation";
                        break;
                }
                //mailmodel.toemail = "ahmadali@fernandez.foundation";
                //mailmodel.ccemail = "ahmadali@fernandez.foundation";
                cm.SendEmail(mailmodel);
            }
            return Json("The request has Rejected", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetConcession(int id)
        {
            var model = myapp.tbl_Concession.Where(l => l.ConcessionId == id).SingleOrDefault();
            ConcessionViewModel dbmodel = new ConcessionViewModel
            {
                Status = model.Status,
                Category = model.Category,
                ConcessionAdvised = model.ConcessionAdvised,
                ConcessionType = model.ConcessionType,
                Consultation = model.Consultation,
                Investigations = model.Investigations,
                IPNumber = model.IPNumber,
                Justification = model.Justification,
                Package = model.Package,
                PaidByPatient = model.PaidByPatient,
                PatientName = model.PatientName,
                ProcedureName = model.ProcedureName,
                Remarks = model.Remarks,
                Scan = model.Scan,
                Service = model.Service,
                TotalBillAmount = model.TotalBillAmount,
                TotalConcessionAmount = model.TotalConcessionAmount,
                DateOfSubmit = model.DateOfSubmit != null ? ProjectConvert.ConverDateTimeToString(model.DateOfSubmit.Value) : "",
                BillNo = model.BillNo,
                LocationId = model.LocationId.Value,
                SubCategory = model.SubCategory,
                TotalConcessionPercentage = model.TotalConcessionPercentage,
                Validity = model.Validity != null ? ProjectConvert.ConverDateTimeToString(model.Validity.Value) : "",
                DocumentName = model.Document,
                Pharmacy = model.Pharmacy,
                NST = model.NST,
                DateOfAdmission = model.DateOfAdmission,
                DateOfDischarge = model.DateOfDischarge,
                ConcessionId = model.ConcessionId
            };
            return Json(dbmodel, JsonRequestBehavior.AllowGet);
        }
        public ConcessionViewModel GetConcessionForPrint(int id)
        {
            var model = myapp.tbl_Concession.Where(l => l.ConcessionId == id).SingleOrDefault();
            ConcessionViewModel dbmodel = new ConcessionViewModel
            {
                Status = model.Status,
                Category = model.Category,
                ConcessionAdvised = model.ConcessionAdvised,
                ConcessionType = model.ConcessionType,
                Consultation = model.Consultation,
                Investigations = model.Investigations,
                IPNumber = model.IPNumber,
                Justification = model.Justification,
                Package = model.Package,
                PaidByPatient = model.PaidByPatient,
                PatientName = model.PatientName,
                ProcedureName = model.ProcedureName,
                Remarks = model.Remarks,
                Scan = model.Scan,
                Service = model.Service,
                TotalBillAmount = model.TotalBillAmount,
                TotalConcessionAmount = model.TotalConcessionAmount,
                DateOfSubmit = model.DateOfSubmit != null ? ProjectConvert.ConverDateTimeToString(model.DateOfSubmit.Value) : "",
                BillNo = model.BillNo,
                LocationId = model.LocationId.Value,
                SubCategory = model.SubCategory,
                TotalConcessionPercentage = model.TotalConcessionPercentage,
                Validity = model.Validity != null ? ProjectConvert.ConverDateTimeToString(model.Validity.Value) : "",
                DocumentName = model.Document,
                Pharmacy = model.Pharmacy,
                NST = model.NST,
                DateOfAdmission = model.DateOfAdmission,
                DateOfDischarge = model.DateOfDischarge,
                ConcessionId = model.ConcessionId,
                Approver = model.ModifiedBy,
                ApproverComments = model.ApproverComments,
                ApproverStatus = model.ApproverStatus
            };
            var appuser = myapp.tbl_User.Where(l => l.CustomUserId == model.ModifiedBy).ToList();
            if (appuser.Count > 0)
            {
                dbmodel.Approver = appuser[0].FirstName + " " + appuser[0].LastName;
            }
            return dbmodel;
        }

        public ActionResult GetBillingForMrNoOrIpNo(string mrno)
        {
            var details = myapp.tbl_Patient.Where(l => l.MRNo == mrno || l.IPNo == mrno).ToList();
            return Json(details, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetBillingforipno(string ipno)
        {
            ConnectOracle connect = new ConnectOracle();
            var model = connect.GetBillingDetails(ipno);
            try
            {
                model.admissiondate = ProjectConvert.ConverDateStringtoDatetime(model.admissiondate, "MM/dd/yyyy hh:mm:ss tt").ToString("dd/MM/yyyy");
            }
            catch
            {
                try
                {
                    model.admissiondate = ProjectConvert.ConverDateStringtoDatetime(model.admissiondate, "M/dd/yyyy hh:mm:ss tt").ToString("dd/MM/yyyy");
                }
                catch { }
            }
            try
            {
                model.dischargedt = ProjectConvert.ConverDateStringtoDatetime(model.dischargedt, "MM/dd/yyyy hh:mm:ss tt").ToString("dd/MM/yyyy");
            }
            catch
            {
                try
                {
                    model.dischargedt = ProjectConvert.ConverDateStringtoDatetime(model.dischargedt, "M/dd/yyyy hh:mm:ss tt").ToString("dd/MM/yyyy");
                }
                catch { }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}