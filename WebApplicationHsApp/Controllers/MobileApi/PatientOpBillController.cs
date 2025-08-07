using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.UI;
using WebApplicationHsApp.Models.Appointment;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    //[Authorize]
    [RoutePrefix("api/PatientOpBill")]
    public class PatientOpBillController : ApiController
    {
        ManagePatientBills ManageApp = new ManagePatientBills();
        [HttpGet]
        [Route("GetAllPatientBills")]
        public List<PatientBillViewModel> GetAllPatientBills()
        {
            return ManageApp.GetAllPatientBills();
        }
        [HttpGet]
        [Route("GetAllPatients")]
        public List<PatientViewModel> GetAllPatients()
        {
            return ManageApp.GetAllPatients();
        }
        [HttpGet]
        [Route("GetPatientBillById")]
        public PatientBillViewModel GetPatientBillById(int id)
        {
            return ManageApp.GetPatientBillById(id);
        }
        [HttpPost]
        [Route("SavePatientBill")]
        public string SavePatientBill(PatientBillViewModel model)
        {
            return ManageApp.SavePatientBill(model, User.Identity.Name);
        }
        [HttpGet]
        [Route("GetAllServiceList")]
        public List<ServiceList> GetAllServiceList()
        {
            return ManageApp.GetAllServiceList();
        }
        [HttpGet]
        [Route("ExportBillPDF")]
        public HttpResponseMessage ExportBillPDF(int Billno)
        {
            HttpResponseMessage result = new HttpResponseMessage();
            var billp=ManageApp.GetPatientBillById(Billno);
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    StringBuilder sb = new StringBuilder();
                    
                    //Generate Invoice (Bill) Header.
                    sb.Append("<table width='100%' cellspacing='0' cellpadding='2'>");
                    sb.Append("<tr><td align='center' style='background-color: #18B5F0' colspan = '2'><b>Order Sheet</b></td></tr>");
                    sb.Append("<tr><td colspan = '2'></td></tr>");
                    sb.Append("<tr><td><b>Order No: </b>");
                    sb.Append(billp.BillId.ToString());
                    sb.Append("</td><td align = 'right'><b>Date: </b>");
                    sb.Append(DateTime.Now);
                    sb.Append(" </td></tr>");
                    sb.Append("<tr><td colspan = '2'><b>Name: </b>");
                    sb.Append("Evolve Childrens Clinic");
                    sb.Append("</td></tr>");
                    sb.Append("</table>");
                    sb.Append("<br />");

                    //Generate Invoice (Bill) Items Grid.
                    sb.Append("<table border = '1'>");
                    sb.Append("<tr>");
                    sb.Append("<th style = 'background-color: #D20B0C;color:#ffffff'>");
                    sb.Append("Name");
                    sb.Append("</th>");
                    sb.Append("<th style = 'background-color: #D20B0C;color:#ffffff'>");
                    sb.Append("Price");
                    sb.Append("</th>");
                    sb.Append("</tr>");
                    foreach (var row in billp.PatientBillItems)
                    {
                        sb.Append("<tr>");
                       
                            sb.Append("<td>");
                            sb.Append(row.ServiceName +" "+row.ServiceDescription);
                        
                        sb.Append("</td>");
                        sb.Append("<td>");
                        sb.Append(row.ServicePrice);

                        sb.Append("</td>");
                        sb.Append("</tr>");
                    }
                    sb.Append("<tr>");
                    //sb.Append(dt.Columns.Count - 1);
                    sb.Append("<td>Total</td>");
                    sb.Append("<td>");
                    sb.Append(billp.TotalPrice);
                    sb.Append("</td>");
                    sb.Append("</tr></table>");

                    //Export HTML String as PDF.
                    StringReader sr = new StringReader(sb.ToString());
                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                    HTMLWorker htmlparser = new HTMLWorker(pdfDoc);
                    MemoryStream workStream = new MemoryStream();
                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, workStream);
                    pdfDoc.Open();
                    htmlparser.Parse(sr);
                    pdfDoc.Close();
                    //Response.ContentType = "application/pdf";
                    //Response.AddHeader("content-disposition", "attachment;filename=Invoice_" + orderNo + ".pdf");
                    //Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    //Response.Write(pdfDoc);
                    //Response.End();
                    byte[] pdf = workStream.ToArray();
                    result = Request.CreateResponse(HttpStatusCode.OK);
                    result.Content = new ByteArrayContent(pdf);
                    result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
                    result.Content.Headers.ContentDisposition.FileName = "IngredientsForeCastReport Pdf.pdf";
                    result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
                }
            }
            return result;
        }
    }
}
