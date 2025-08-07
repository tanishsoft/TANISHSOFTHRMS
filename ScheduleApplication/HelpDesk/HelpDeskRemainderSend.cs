using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using ScheduleApplication.Common;
using ScheduleApplication.DataModel;

namespace ScheduleApplication.HelpDesk
{
    public class HelpDeskRemainderSend
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        string username = ConfigurationManager.AppSettings["username"];
        string password = ConfigurationManager.AppSettings["password"];
        string SenderId = ConfigurationManager.AppSettings["SenderId"];
        string SendSMSCheck = ConfigurationManager.AppSettings["SendSMS"];
        public string Sendtickets()
        {
            string jobstatus = "Success";
            string Evironmentcheck = ConfigurationManager.AppSettings["Environment"];
            try
            {
                var takslist = myapp.tbl_Task.Where(l => l.AssignStatus != "New" && l.AssignStatus != "Done" && l.AssignDepartmentName == "IT").ToList();

                DateTime CurrentDate = DateTime.Now;
                string body = "<p>Dear Sir</p><br /><p> The following Tickets are updated more than 4 hours ago.</p><br /><table style='padding:5px;'><tr><td style='border:solid 1px #eee;'>Ticket Id</td><td style='border:solid 1px #eee;'>Created On</td><td style='border:solid 1px #eee;'>Category</td><td style='border:solid 1px #eee;'>Subject</td><td style='border:solid 1px #eee;'>Description</td><td style='border:solid 1px #eee;'>Creator</td><td style='border:solid 1px #eee;'>Status</td><td style='border:solid 1px #eee;'>Assign</td></tr>";
                //Level 1 Tickets
                bool isbody = false;
                var ticekts4hours = takslist.Where(l => CurrentDate.Subtract(l.ModifiedOn.Value).TotalHours >= 4 && CurrentDate.Subtract(l.ModifiedOn.Value).TotalHours <= 8).ToList();
                foreach (var task in ticekts4hours)
                {
                    var ticketlogs = myapp.tbl_TaskRemainderLogs.Where(l => l.TaskId == task.TaskId && l.SentTo == "4hours").ToList();
                    //ticketlogs = ticketlogs.Where(l => l.SentOn.Value.ToString("dd/MM/yyyy") == CurrentDate.ToString("dd/MM/yyyy")).ToList();
                    //ticketlogs = ticketlogs.Where(l => CurrentDate.Subtract(l.SentOn.Value).TotalHours >= 4 && CurrentDate.Subtract(l.SentOn.Value).TotalHours <= 8).ToList();
                    if (ticketlogs.Count == 0)
                    {
                        isbody = true;
                        body += "<tr><td style='border:solid 1px #eee;'>" + task.TaskId + "</td><td style='border:solid 1px #eee;'>" + task.CreatedOn.ToString() + "</td><td style='border:solid 1px #eee;'>" + task.CategoryOfComplaint + "</td><td style='border:solid 1px #eee;'>" + task.Subject + "</td><td style='border:solid 1px #eee;'>" + task.Description + "</td><td style='border:solid 1px #eee;'>" + task.CreatorName + "</td><td style='border:solid 1px #eee;'>" + task.AssignStatus + "</td><td style='border:solid 1px #eee;'>" + task.AssignName + "</td></tr>";
                        string msg = "Id -" + task.TaskId + "" + task.CategoryOfComplaint + " Sub: " + task.Subject + "- Created On: " + task.CreatedOn.ToString() + "";
                        if (task.AssignName != null && task.AssignName != "")
                        {
                            msg = msg + " Assign to " + task.AssignName;
                        }
                        msg = msg + " More than 4 hours";
                        string mobile = "7995007710";
                        string url = "http://smslogin.mobi/spanelv2/api.php?username=" + username + "&password=" + password + "&to=" + mobile + "&from=" + SenderId + "&message=" + msg;

                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        request.MaximumAutomaticRedirections = 4;
                        request.MaximumResponseHeadersLength = 4;
                        request.Credentials = CredentialCache.DefaultCredentials;
                        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        Stream receiveStream = response.GetResponseStream();
                        StreamReader stIn = new StreamReader(receiveStream, Encoding.UTF8);
                        var msg1 = stIn.ReadToEnd();

                        tbl_TaskRemainderLogs model = new tbl_TaskRemainderLogs();
                        model.SentHod = msg1;
                        model.SentTo = "4hours";
                        model.SentOn = DateTime.Now;
                        //model.SentTo = "ahmadali@fernandez.foundation";
                        model.TaskId = (int)task.TaskId;
                        myapp.tbl_TaskRemainderLogs.Add(model);
                        myapp.SaveChanges();
                    }

                }
                body += "</table><br /><p>This is system generated email</p>";

                if (ticekts4hours.Count > 0 && isbody)
                {
                    SendAnEmail cm = new SendAnEmail();
                    MailModel mailmodel = new MailModel();
                    mailmodel.fromemail = "helpdesk@fernandez.foundation";
                    mailmodel.toemail = "ahmadali@fernandez.foundation";
                    mailmodel.ccemail = "elroy@fernandez.foundation,edward.c@fernandez.foundation";

                    mailmodel.subject = "Tickets Updated more than 4 hours ago";
                    mailmodel.body = body;
                    mailmodel.filepath = "";
                    mailmodel.username = "Help Desk";
                    mailmodel.fromname = "Help Desk";
                    cm.SendEmail(mailmodel);
                }

                string bodymanger = "<p>Dear Sir</p><p> The following Tickets are updated more than 8 hours ago.</p><br /><table style='padding:5px;'><tr><td style='border:solid 1px #eee;'>Ticket Id</td><td style='border:solid 1px #eee;'>Created On</td><td style='border:solid 1px #eee;'>Category</td><td style='border:solid 1px #eee;'>Subject</td><td style='border:solid 1px #eee;'>Description</td><td style='border:solid 1px #eee;'>Creator</td><td style='border:solid 1px #eee;'>Status</td><td style='border:solid 1px #eee;'>Assign</td></tr>";
                //Level 1 Tickets
                var ticekts8hours = takslist.Where(l => CurrentDate.Subtract(l.ModifiedOn.Value).TotalHours > 8).ToList();
                bool isbodymanger = false;
                foreach (var task in ticekts8hours)
                {
                    var ticketlogs = myapp.tbl_TaskRemainderLogs.Where(l => l.TaskId == task.TaskId && l.SentTo == "8hours").ToList();
                    //ticketlogs = ticketlogs.Where(l => l.SentOn.Value.ToString("dd/MM/yyyy") == CurrentDate.ToString("dd/MM/yyyy")).ToList();
                    if (ticketlogs.Count == 0)
                    {
                        isbodymanger = true;
                        bodymanger += "<tr><td style='border:solid 1px #eee;'>" + task.TaskId + "</td><td style='border:solid 1px #eee;'>" + task.CreatedOn.ToString() + "</td><td style='border:solid 1px #eee;'>" + task.CategoryOfComplaint + "</td><td style='border:solid 1px #eee;'>" + task.Subject + "</td><td style='border:solid 1px #eee;'>" + task.Description + "</td><td style='border:solid 1px #eee;'>" + task.CreatorName + "</td><td style='border:solid 1px #eee;'>" + task.AssignStatus + "</td><td style='border:solid 1px #eee;'>" + task.AssignName + "</td></tr>";
                        string msg = "Id -" + task.TaskId + "" + task.CategoryOfComplaint + " Sub: " + task.Subject + "- Created On: " + task.CreatedOn.ToString();
                        if (task.AssignName != null && task.AssignName != "")
                        {
                            msg = msg + " Assign to " + task.AssignName;
                        }
                        msg = msg + " More than 8 hours";
                        //string mobile = "9849650444";
                        //string url = "http://smslogin.mobi/spanelv2/api.php?username=" + username + "&password=" + password + "&to=" + mobile + "&from=" + SenderId + "&message=" + msg;

                        //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                        //request.MaximumAutomaticRedirections = 4;
                        //request.MaximumResponseHeadersLength = 4;
                        //request.Credentials = CredentialCache.DefaultCredentials;
                        //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                        //Stream receiveStream = response.GetResponseStream();
                        //StreamReader stIn = new StreamReader(receiveStream, Encoding.UTF8);
                        //var msg1 = stIn.ReadToEnd();

                        tbl_TaskRemainderLogs model = new tbl_TaskRemainderLogs();
                        model.SentHod = "";
                        model.SentTo = "8hours";
                        model.SentOn = DateTime.Now;
                        //model.SentTo = "nandakishore_p@fernandez.foundation";
                        model.TaskId = (int)task.TaskId;
                        myapp.tbl_TaskRemainderLogs.Add(model);
                        myapp.SaveChanges();
                    }
                }
                bodymanger += "</table><br /><p>This is system generated email</p>";
                if (ticekts8hours.Count > 0 && isbodymanger)
                {
                    SendAnEmail cm = new SendAnEmail();
                    MailModel mailmodel = new MailModel();
                    mailmodel.fromemail = "helpdesk@fernandez.foundation";
                    mailmodel.toemail = "nandakishore_p@fernandez.foundation";
                    mailmodel.ccemail = "elroy@fernandez.foundation,ahmadali@fernandez.foundation,edward.c@fernandez.foundation";

                    mailmodel.subject =  "Tickets Updated more than 8 hours ago";
                    mailmodel.body = bodymanger;
                    mailmodel.filepath = "";
                    mailmodel.username = "Help Desk";
                    mailmodel.fromname = "Help Desk";
                    cm.SendEmail(mailmodel);
                }

                //SendAnEmail cm1 = new SendAnEmail();
                //MailModel mailmodel1 = new MailModel();
                //mailmodel1.fromemail = "helpdesk@fernandez.foundation";
                //mailmodel1.toemail = "vamsi@microarctech.com";
                //mailmodel1.ccemail = "vamsi@microarctech.com";

                //mailmodel1.subject = "Job running successfully on " + DateTime.Now;
                //mailmodel1.body = "Job running successfully on " + DateTime.Now;
                //mailmodel1.filepath = "";
                //mailmodel1.username = "Help Desk";
                //mailmodel1.fromname = "Help Desk";
                //cm1.SendEmail(mailmodel1);
            }
            catch (Exception ex)
            {
                jobstatus = "Error " + ex.Message;
                //SendAnEmail cm1 = new SendAnEmail();
                //MailModel mailmodel1 = new MailModel();
                //mailmodel1.fromemail = "helpdesk@fernandez.foundation";
                //mailmodel1.toemail = "vamsi@microarctech.com";
                //mailmodel1.ccemail = "vamsi@microarctech.com";

                //mailmodel1.subject = "Job running Error on " + DateTime.Now;
                //mailmodel1.body = ex.Message;
                //mailmodel1.filepath = "";
                //mailmodel1.username = "Help Desk";
                //mailmodel1.fromname = "Help Desk";
                //cm1.SendEmail(mailmodel1);
            }
            return jobstatus;
        }
    }
}
