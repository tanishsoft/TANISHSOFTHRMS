using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace WebApplicationHsApp.Models
{
    public class Datavalue
    {
        public string Key { get; set; }
        public int Value { get; set; }
    }

    public class TempEmployeeLeaveTableData
    {
        public string EmployeeId { get; set; }
        public string LeaveDate { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public string LeaveTypeName_ShortCut { get; set; }
    }

    public class EmployeeLeave_ReportData
    {
        public string UserId { get; set; }
        public string UserNameWithEmpID { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public double LeaveTypeCount { get; set; }
        public double TotalLeaveCount { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public double CasualLeaveCount { get; set; }
        public double SickLeaveCount { get; set; }
        public double EarnedleaveCount { get; set; }
        public double CompOffCount { get; set; }
        public double MaternityLeaveCount { get; set; }
        public double PaternityLeaveCount { get; set; }
        public double LossofpayCount { get; set; }
    }


    public class CustomPage
    {
        [AllowHtml]
        public string PageContent { get; set; }
        public string pagetitle { get; set; }
        public bool IsPageisForall { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string PageMenuId { get; set; }
    }
    public class MessageModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public string Data { get; set; }
    }
    public class CustomModel
    {
        string SendEmailvalue = WebConfigurationManager.AppSettings["SendEmail"];
        public string SendEmail(MailModel model)
        {
            string result = "Sent Success";
            try
            {
                //string credentialUserName = "infonet@fernandez.foundation";               
                //string pwd = "rppcivmwsndstvin";
                //SmtpClient smtp = new SmtpClient
                //{
                //    Host = "smtp.gmail.com",
                //    Port = 587,
                //    EnableSsl = true,
                //    DeliveryMethod = SmtpDeliveryMethod.Network,
                //    Credentials = new System.Net.NetworkCredential(credentialUserName, pwd),
                //    Timeout = 50000,
                //};

                string credentialUserName = "infonet@fernandez.foundation";
                string pwd = "Fernandez@654321";
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.office365.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Credentials = new System.Net.NetworkCredential(credentialUserName, pwd),
                    Timeout = 50000,
                };
                string toemailaddress = model.toemail.Contains(',') ? model.toemail.Split(',')[0] : model.toemail;
                MailMessage mailmessage = new MailMessage(new MailAddress(credentialUserName, model.fromname), new MailAddress(toemailaddress, model.username));
                if (model.toemail.Contains(','))
                {
                    var mailidslist = model.toemail.Split(',');
                    foreach (var mailidto in mailidslist)
                    {
                        if (mailidto != null && mailidto != "" && mailidto != toemailaddress)
                            mailmessage.To.Add(mailidto);
                    }
                }
                mailmessage.Subject = model.subject;
                if (!string.IsNullOrEmpty(model.ccemail))
                {
                    string[] CCId = model.ccemail.Split(',');
                    foreach (string CCEmail in CCId)
                    {
                        if (CCEmail != null && CCEmail != "")
                            mailmessage.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                    }
                }
                //mailmessage.Bcc.Add(new MailAddress("ahmadali@fernandez.foundation"));
                mailmessage.Body = model.body;
                mailmessage.IsBodyHtml = true;
                if (model.linkedResource != null && model.linkedResource.Count > 0)
                {
                    AlternateView view = AlternateView.CreateAlternateViewFromString(mailmessage.Body, null, MediaTypeNames.Text.Html);

                    foreach (var v in model.linkedResource)
                    {
                        view.LinkedResources.Add(v);
                    }
                    mailmessage.AlternateViews.Add(view);
                }
                try
                {
                    if (model.attachments != null && model.attachments.Count > 0)
                    {
                        foreach (var attachmentPath in model.attachments)
                        {
                            if (!string.IsNullOrEmpty(attachmentPath))
                            {
                                var filePath = System.IO.Path.Combine(model.folderpath, attachmentPath);
                                if (System.IO.File.Exists(filePath))
                                {
                                    var attachment = new Attachment(filePath);
                                    mailmessage.Attachments.Add(attachment);
                                }

                            }
                        }
                    }
                }
                catch { }
                //if (SendEmailvalue == "yes")
                smtp.Send(mailmessage);

            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }

    public class MailModel
    {
        public string folderpath { get; set; }
        public string fromemail { get; set; }
        public string toemail { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string filepath { get; set; }
        public string username { get; set; }
        public string fromname { get; set; }
        public string ccemail { get; set; }
        public List<LinkedResource> linkedResource { get; set; }
        public List<string> attachments { get; set; }
    }


}