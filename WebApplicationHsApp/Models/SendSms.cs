using System;
using System.Net;

namespace WebApplicationHsApp.Models
{
    public class SendSms
    {
        public string SendSmsToEmployee(string mobile, string msg)
        {
            //string username = ConfigurationManager.AppSettings["username"];
            //string password = ConfigurationManager.AppSettings["password"];
            //string SenderId = ConfigurationManager.AppSettings["SenderId"];
            //string SendSMSCheck = ConfigurationManager.AppSettings["SendSMS"];
            try
            {
                //if (SendSMSCheck == "yes")
                //{
                //string url = "http://api.textlocal.in/send/?username=" + username + "&password=" + password + "&numbers=" + mobile + "&sender=" + SenderId + "&message=" + msg;
                //string msg1 = "";
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //request.MaximumAutomaticRedirections = 4;
                //request.MaximumResponseHeadersLength = 4;
                //request.Credentials = CredentialCache.DefaultCredentials;
                //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //Stream receiveStream = response.GetResponseStream();
                //StreamReader stIn = new StreamReader(receiveStream, Encoding.UTF8);
                //msg1 = stIn.ReadToEnd();
                //return msg1;

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                String message = System.Web.HttpUtility.UrlEncode(msg);
                using (var wb = new System.Net.WebClient())
                {
                    byte[] response = wb.UploadValues("https://api.textlocal.in/send/", new System.Collections.Specialized.NameValueCollection()
                    {
                    {"apikey" , "jsslpRtdX4Y-GCDRzrr4e8kadWL16tiIyrIlU36RfG"},
                    {"numbers" , mobile},
                    {"message" , message},
                    {"sender" , "FERNAN"}
                    });
                    string result = System.Text.Encoding.UTF8.GetString(response);
                    return result;
                }

                //}
                //else
                //{
                //    return "Success";
                //}

            }
            catch (Exception exc)
            {
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel
                {
                    fromemail = "Leave@hospitals.com",
                    toemail = "vamsirm26@gmail.com",
                    subject = "Error on SMS",
                    body = exc.Message + exc.InnerException.ToString(),
                    filepath = "",
                    fromname = "Error on SMS",
                    ccemail = "edward.c@fernandez.foundation"
                };
                cm.SendEmail(mailmodel);
                return "error";
            }
        }
    }
}