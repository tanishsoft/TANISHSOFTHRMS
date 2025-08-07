using ScheduleApplication.DataModel;
using System;
using System.Linq;
using ScheduleApplication.Common;
using System.Configuration;
using System.Collections.Generic;
using System.IO;

namespace ScheduleApplication.Notifications
{
    public class DailyRunActivites
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();

        public string SendTodayBirthdayToUser()
        {
            //string Evironmentcheck = ConfigurationManager.AppSettings["Environment"];
            string jobstatus = "Success";
            try
            {
                DateTime dt = DateTime.Now.Date;
                var listuser = myapp.tbl_User.Where(t => t.IsActive == true && t.DateOfBirth != null && t.DateOfBirth.Value.Month == dt.Month && t.DateOfBirth.Value.Day == dt.Day).ToList();
                foreach (var v in listuser)
                {
                    string cusemail = v.CustomUserId.ToLower() + "@fernandez.foundation";
                    if (v.EmailId != null && v.EmailId.ToLower() != cusemail)
                    {
                        SendAnEmail cm = new SendAnEmail();
                        MailModel mailmodel = new MailModel();
                        mailmodel.fromemail = "helpdesk@fernandez.foundation";
                        mailmodel.toemail = v.EmailId;
                        mailmodel.ccemail = "ahmadali@fernandez.foundation";
                        //mailmodel.ccemail = "elroy@fernandez.foundation,vamsi@microarctech.com";
                        string body = "<div style='height: 544px; background: url(&quot;http://111.93.11.121:16/Images/002.jpg &quot;) center center no-repeat;' id ='borthdaybackgroundimg'>";
                        body += "<div  style='padding:245px;'>";
                        body += "<p style='font-size: 25px;font-style: italic;font-family: cambria;color:teal;'>Dear " + v.FirstName + "</p><p style='font-size: 25px;font-style: italic;font-family: cambria;color:teal;'> Wishing you an extraordinary birthday and an important year. From every one of us.</p><br /></div></div>";
                        mailmodel.subject = "Fernandez Hospital wishes you a Happy Birthday";
                        mailmodel.body = body;
                        mailmodel.filepath = "";
                        mailmodel.username = v.FirstName;
                        mailmodel.fromname = "Fernandez Hospital";
                        cm.SendEmail(mailmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                jobstatus = "Error " + ex.Message;
            }
            return jobstatus;
        }

        public string SendworkanniversarytoUsers()
        {
            string jobstatus = "Success";
            try
            {
                //string Evironmentcheck = ConfigurationManager.AppSettings["Environment"];
                DateTime dt = DateTime.Now.Date;
                var listuser = myapp.tbl_User.Where(t => t.IsActive == true && t.SendRemainder == true && t.DateOfJoining != null).ToList();
                List<tbl_User> users = new List<tbl_User>();
                foreach (var l in listuser)
                {
                    DateTime joindate = l.DateOfJoining.Value;
                    if (l.DateOfJoining.Value.Month == dt.Month && l.DateOfJoining.Value.Day == dt.Day)
                    {
                        users.Add(l);
                    }
                    else if (joindate.AddMonths(1).Month == dt.Month && l.DateOfJoining.Value.Day == dt.Day && dt.Year == joindate.AddMonths(1).Year)
                    {
                        users.Add(l);
                    }
                    else if (joindate.AddMonths(6).Month == dt.Month && l.DateOfJoining.Value.Day == dt.Day && dt.Year == joindate.AddMonths(6).Year)
                    {
                        users.Add(l);
                    }
                }


                //var listuser = myapp.tbl_User.Where(t => t.IsActive == true && t.DateOfJoining != null && t.DateOfJoining.Value.Month == dt.Month && t.DateOfJoining.Value.Day == dt.Day).ToList();
                foreach (var v in users)
                {
                    DateTime joindate = v.DateOfJoining.Value;
                    bool IsSixmonth = false, IsOneMonth = false, IsYear = false;
                    if (v.DateOfJoining.Value.Month == dt.Month && v.DateOfJoining.Value.Day == dt.Day)
                    {
                        IsYear = true;
                    }
                    else if (joindate.AddMonths(1).Month == dt.Month && v.DateOfJoining.Value.Day == dt.Day && dt.Year == joindate.AddMonths(1).Year)
                    {
                        IsOneMonth = true;
                    }
                    else if (joindate.AddMonths(6).Month == dt.Month && v.DateOfJoining.Value.Day == dt.Day && dt.Year == joindate.AddMonths(6).Year)
                    {
                        IsSixmonth = true;
                    }
                    string cusemail = v.CustomUserId.ToLower() + "@fernandez.foundation";
                    if (v.EmailId != null && v.EmailId.ToLower() != cusemail)
                    {
                        SendAnEmail cm = new SendAnEmail();
                        MailModel mailmodel = new MailModel();
                        mailmodel.fromemail = "helpdesk@fernandez.foundation";
                        mailmodel.toemail = v.EmailId;
                        mailmodel.ccemail = "ahmadali@fernandez.foundation";

                        string body = "";
                        string hodbody = "";
                        //var buildDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                        //var filePath = buildDir + @"\Biomedical_Asset.html";
                        //using (StreamReader reader = new StreamReader(filePath))
                        //{
                        //    body = reader.ReadToEnd();
                        //    hodbody = reader.ReadToEnd();
                        ////}
                        //body = body.Replace("[[Heading]]", "Congratulations on your service anniversary today! ");
                        //hodbody = hodbody.Replace("[[Heading]]", "Congratulations on service anniversary today! ");

                        if (IsOneMonth)
                        {
                            hodbody = "Dear Nursing Superintendent,<br />" + v.FirstName + " has completed one month today with the Fernandez Foundation. You are requested to complete one month of nursing privilege in discussion with the employee.";
                            //hodbody = hodbody.Replace("[[table]]", tblinner);
                            body = "Dear " + v.FirstName + ",<br />Congratulations!! You have completed one month today with the Fernandez Foundation. Please connect with your HOD for a one month nursing privileges discussion.";
                            //body = body.Replace("[[table]]", tblinner);
                        }
                        if (IsSixmonth)
                        {
                            hodbody = "Dear Nursing Superintendent,<br />" + v.FirstName + " has completed six months today with the Fernandez Foundation. You are requested to complete six months of nursing privilege in discussion with the employee.";
                            //hodbody = hodbody.Replace("[[table]]", tblinner);
                            body = "Dear " + v.FirstName + ",<br />Congratulations!! You have completed six months today with the Fernandez Foundation. Please connect with your HOD for a six months nursing privileges discussion.";
                            //body = body.Replace("[[table]]", tblinner);
                        }
                        if (IsYear)
                        {
                            hodbody = "Dear Nursing Superintendent,<br />" + v.FirstName + " has completed one year today with the Fernandez Foundation. You are requested to complete one year of nursing privilege in discussion with the employee.";
                            //hodbody = hodbody.Replace("[[table]]", tblinner);
                            body = "Dear " + v.FirstName + ",<br />Congratulations!! You have completed one year today with the Fernandez Foundation. Please connect with your HOD for a one year nursing privileges discussion.";
                            //body = body.Replace("[[table]]", tblinner);
                        }


                        //string body = "<div style='height: 544px; background-color:#eee;' id ='borthdaybackgroundimg'>";
                        //body += "<div  style='padding:100px;'>";
                        //body += "<p style='font-size: 25px;font-style: italic;font-family: cambria;color:teal;'>Dear " + v.FirstName + "</p><p style='font-size: 25px;font-style: italic;font-family: cambria;color:teal;'>Congratulations on your service anniversary today! “We are so proud to have you as part of our work family. We hope that you keep up the good work for many years to come!</p><br />";
                        mailmodel.subject = "Congratulations on your service anniversary today! ";
                        mailmodel.body = body;
                        mailmodel.filepath = "";
                        mailmodel.username = v.FirstName;
                        mailmodel.fromname = "Fernandez Hospital";
                        cm.SendEmail(mailmodel);
                        if (v.ReportingManagerId != null && v.ReportingManagerId > 0)
                        {
                            var userdet = myapp.tbl_User.Where(l => l.EmpId == v.ReportingManagerId).SingleOrDefault();
                            if (userdet != null && userdet.EmailId != null && userdet.EmailId != "")
                            {
                                mailmodel.toemail = userdet.EmailId;
                                mailmodel.body = hodbody;
                                cm.SendEmail(mailmodel);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                jobstatus = "Error " + ex.Message;
            }
            return jobstatus;
        }

        public string SendRemainderstoUsersForNextday()
        {
            string jobstatus = "Success";
            try
            {
                //string Evironmentcheck = ConfigurationManager.AppSettings["Environment"];
                DateTime dt = DateTime.Now.Date;
                dt = dt.AddDays(1);
                var bookdaylist = myapp.tbl_Booking.Where(e => e.FromDate >= dt && e.ToDate <= dt && e.IsActive == true).ToList();
                foreach (var evnt in bookdaylist)
                {
                    SendAnEmail cm = new SendAnEmail();
                    MailModel mailmodel = new MailModel();
                    var cuser = myapp.tbl_User.Where(l => l.CustomUserId == evnt.CreatedBy).FirstOrDefault();
                    if (cuser != null)
                    {
                        mailmodel.fromemail = "it_bookConference@fernandez.foundation";
                        mailmodel.subject = "Meeting Date: " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + "   Time : " + evnt.StartTime + " to " + evnt.EndTime;
                        string mailbody = "<p style='font-family:verdana;font-size:14px;'>A New Meeting  on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + "   Time : " + evnt.StartTime + " to " + evnt.EndTime + "</p>";
                        string footer = "";

                        string smssub = "", smsfooter = "";
                        smssub = "Meeting Date : " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + " Time :" + evnt.StartTime + " to " + evnt.EndTime + ".";
                        smsfooter = "Location: " + evnt.LocationName + " - " + evnt.BuildingName + " - " + evnt.FloorName + " Created By : " + cuser.FirstName;
                        footer += "<p  style='font-family:verdana;font-size:14px;'>Location: " + evnt.LocationName + " - " + evnt.BuildingName + " - " + evnt.FloorName + " Created By : " + cuser.FirstName + " </p>";
                        footer += "<br/><p  style='font-family:verdana;font-size:12px;'>This is an auto generated mail,Don't reply to this.</p>";
                        mailmodel.filepath = "";
                        mailmodel.fromname = cuser.FirstName + "  Event " + evnt.Titles + " on " + evnt.FromDate.Value.ToString("dd/MM/yyyy");
                        mailmodel.ccemail = "";
                        if (evnt.video != null && evnt.video != "")
                        {

                            mailmodel.toemail = "srinivas@fernandez.foundation";
                            mailbody += "<p style='font-family:verdana;font-size:14px;'>Event Title : " + evnt.Titles + "</p>";
                            mailbody += "<p style='font-family:verdana;font-size:14px;'>Video Requirements are : " + evnt.video + "</p>";
                            mailmodel.body = mailbody + footer;
                            cm.SendEmail(mailmodel);
                        }
                        if (evnt.ITReq != null && evnt.ITReq != "")
                        {
                            mailbody = "<p style='font-family:verdana;font-size:14px;'>A New Meeting  on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + "   Time : " + evnt.StartTime + " to " + evnt.EndTime + "</p>";
                            mailmodel.toemail = "it@fernandez.foundation";
                            mailmodel.ccemail = "elroy@fernandez.foundation";
                            mailbody += "<p style='font-family:verdana;font-size:14px;'>Event Title : " + evnt.Titles + "</p>";
                            mailbody += "<p style='font-family:verdana;font-size:14px;'>IT Requirements are :  " + evnt.ITReq + "</p>";
                            mailmodel.body = mailbody + footer;
                            cm.SendEmail(mailmodel);
                        }
                        bool isMaintenancereq = false;
                        if ((evnt.Chair != null && evnt.Chair != "") || (evnt.Tables != null && evnt.Tables != "") || (evnt.Arrangemnts != null && evnt.Arrangemnts != ""))
                        {
                            isMaintenancereq = true;
                        }

                        switch (evnt.LocationId)
                        {


                            case 2:

                                if (isMaintenancereq)
                                {
                                    mailmodel.toemail = "ayyavaru_m@fernandez.foundation";
                                    mailbody = "<p style='font-family:verdana;font-size:14px;'>A New Meeting  on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + "   Time : " + evnt.StartTime + " to " + evnt.EndTime + "</p>";
                                    mailbody += "<p style='font-family:verdana;font-size:14px;'>Event Title : " + evnt.Titles + "</p>";
                                    mailbody += "<p style='font-family:verdana;font-size:14px;'>Requirements are </p>";

                                    if (evnt.Chair != null && evnt.Chair != "")
                                    {
                                        mailbody += "<p style='font-family:verdana;font-size:14px;'>No Of Chairs : " + evnt.Chair + "</p> ";
                                    }
                                    if (evnt.Tables != null && evnt.Tables != "")
                                    {
                                        mailbody += "<p style='font-family:verdana;font-size:14px;'>No Of Tables : " + evnt.Tables + "</p>";
                                    }
                                    if (evnt.Arrangemnts != null && evnt.Arrangemnts != "")
                                    {
                                        mailbody += "<p style='font-family:verdana;font-size:14px;'>Arrangemnts : " + evnt.Arrangemnts + "</p> ";
                                    }
                                    mailmodel.body = mailbody + footer;
                                    cm.SendEmail(mailmodel);
                                }
                                break;

                            case 5:

                                if (isMaintenancereq)
                                {

                                    mailmodel.toemail = "maintenance_sh@fernandez.foundation";
                                    mailmodel.ccemail = "assis@fernandez.foundation";
                                    mailbody = "<p style='font-family:verdana;font-size:14px;'>A New Meeting  on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + "   Time : " + evnt.StartTime + " to " + evnt.EndTime + "</p>";
                                    mailbody += "<p style='font-family:verdana;font-size:14px;'>Event Title : " + evnt.Titles + "</p>";
                                    mailbody += "<p style='font-family:verdana;font-size:14px;'>Requirements are </p>";

                                    if (evnt.Chair != null && evnt.Chair != "")
                                    {
                                        mailbody += "<p style='font-family:verdana;font-size:14px;'>No Of Chairs : " + evnt.Chair + "</p> ";
                                    }
                                    if (evnt.Tables != null && evnt.Tables != "")
                                    {
                                        mailbody += "<p style='font-family:verdana;font-size:14px;'>No Of Tables : " + evnt.Tables + "</p>";
                                    }
                                    if (evnt.Arrangemnts != null && evnt.Arrangemnts != "")
                                    {
                                        mailbody += "<p style='font-family:verdana;font-size:14px;'>Arrangemnts : " + evnt.Arrangemnts + "</p> ";
                                    }
                                    mailmodel.body = mailbody + footer;
                                    cm.SendEmail(mailmodel);
                                }
                                break;
                        }

                        //academics mail send option
                        mailbody = "";
                        mailmodel.toemail = "academics@fernandez.foundation";
                        mailmodel.ccemail = "abhishek_sengupta@fernandez.foundation";
                        mailbody += "<p style='font-family:verdana;font-size:14px;'>Hello,</p>";
                        mailbody += "<p style='font-family:verdana;font-size:14px;'>The conference room at " + evnt.LocationName + "-" + evnt.BuildingName + "-" + evnt.FloorName + " has been booked by " + cuser.FirstName + " for " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + " Time is " + evnt.StartTime + " to " + evnt.EndTime + ".</p> ";
                        mailbody += "<p style='font-family:verdana;font-size:14px;'>Event Title : " + evnt.Titles + "</p>";
                        mailbody += "<p style='font-family:verdana;font-size:14px;'>Services required </p> ";


                        if (evnt.Chair != null && evnt.Chair != "")
                        {
                            mailbody += "<p style='font-family:verdana;font-size:14px;'>No Of Chairs : " + evnt.Chair + "</p> ";
                        }
                        if (evnt.Tables != null && evnt.Tables != "")
                        {
                            mailbody += "<p style='font-family:verdana;font-size:14px;'>No Of Tables : " + evnt.Tables + "</p>";
                        }
                        if (evnt.Arrangemnts != null && evnt.Arrangemnts != "")
                        {
                            mailbody += "<p style='font-family:verdana;font-size:14px;'>Arrangemnts : " + evnt.Arrangemnts + "</p> ";
                        }
                        mailbody += "<p style='font-family:verdana;font-size:14px;'>IT Requirements are : " + evnt.ITReq + "</p> ";
                        mailmodel.body = mailbody + footer;
                        cm.SendEmail(mailmodel);
                    }
                }
            }
            catch (Exception ex)
            {
                jobstatus = "Error " + ex.Message;
            }
            return jobstatus;
        }
    }
}
