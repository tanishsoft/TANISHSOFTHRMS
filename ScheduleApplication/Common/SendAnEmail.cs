using System;
using System.Net.Mail;

namespace ScheduleApplication.Common
{
    public class MailModel
    {
        public string fromemail { get; set; }
        public string toemail { get; set; }
        public string subject { get; set; }
        public string body { get; set; }
        public string filepath { get; set; }
        public string username { get; set; }
        public string fromname { get; set; }
        public string ccemail { get; set; }

    }

    public class SendAnEmail
    {

        public string SendEmail(MailModel model)
        {
            string result = "Sent Success";
            try
            {
                //var credentialUserName = "leavemanagement@fernandez.foundation";
                //var pwd = "Fernandez@123";
                //if (model.fromemail.Contains("helpdesk"))
                //{
                //    credentialUserName = "fernandezhopstialhelpdesk@gmail.com";
                //    pwd = "Fmh@it@rt01";
                //}
                //string sentFrom = "infonet@fernandez.foundation";
                //string pwd = "Infonet@123";

                //SmtpClient smtp = new SmtpClient
                //{
                //    Host = "smtp.gmail.com",
                //    Port = 587,
                //    EnableSsl = true,
                //    DeliveryMethod = SmtpDeliveryMethod.Network,
                //    Credentials = new System.Net.NetworkCredential(sentFrom, pwd),
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
                MailMessage mailmessage = new MailMessage(new MailAddress(credentialUserName, model.fromname), new MailAddress(model.toemail, model.username));
                mailmessage.Subject = model.subject;
                if (!string.IsNullOrEmpty(model.ccemail))
                {
                    string[] CCId = model.ccemail.Split(',');
                    foreach (string CCEmail in CCId)
                    {
                        mailmessage.CC.Add(new MailAddress(CCEmail)); //Adding Multiple CC email Id
                    }
                }
                mailmessage.Body = model.body;
                mailmessage.IsBodyHtml = true;

                smtp.Send(mailmessage);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }
    }
}
