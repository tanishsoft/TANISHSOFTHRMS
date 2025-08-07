using System;
using System.Linq;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class BookConferenceScheduleSms
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public void SendSmstoEmp()
        {
            SendSms sms = new SendSms();
            CustomModel cm = new CustomModel();
            DateTime dt = DateTime.Now;
            dt = dt.AddDays(1).Date;
            MailModel mailmodel = new MailModel();
            var events = myapp.tbl_Booking.Where(e => e.FromDate >= dt && e.ToDate <= dt).ToList();
            foreach (var evnt in events)
            {
                mailmodel = new MailModel();
                mailmodel.fromemail = "it_bookConference@fernandez.foundation";
                mailmodel.subject = "Meeting Date: " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + "   Time : " + evnt.StartTime + " to " + evnt.EndTime;
                string mailbody = "<h4 style='font-family:cambria'>A New Meeting  on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + "   Time : " + evnt.StartTime + " to " + evnt.EndTime + "</h4><br />";
                string footer = "";
                string smstoIT = "", smstoAudio = "", Maintenance = "";
                var cuser = myapp.tbl_User.Where(u => u.CustomUserId == evnt.CreatedBy).Single();
                footer += "<h4 style='font-family:cambria'>Location: " + evnt.LocationName + "-" + evnt.BuildingName + "-" + evnt.FloorName + " Created By : " + cuser.FirstName + " </h4><br />";
                footer += "<br/><h4 style='font-family:cambria'>This is an auto generated mail,Don't reply to this.</h>";
                mailmodel.filepath = "";
                mailmodel.fromname = "A New Meeting  on " + evnt.FromDate.Value.ToString("dd/MM/yyyy");
                mailmodel.ccemail = "";
                string smssub = "", smsfooter = "";
                smssub = "Meeting Date : " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + " Time :" + evnt.StartTime + " to " + evnt.EndTime + ".";
                smsfooter = "Location: " + evnt.LocationName + "-" + evnt.BuildingName + "-" + evnt.FloorName + " Created By : " + cuser.FirstName;
                if (evnt.video != null && evnt.video != "")
                {
                    smstoAudio = "8179003925";
                    string Videomsg = smssub;
                    Videomsg += "Requirements: " + evnt.video + "";
                    Videomsg += smsfooter;
                    sms.SendSmsToEmployee(smstoAudio, Videomsg);
                    mailmodel.toemail = "srinivas@fernandez.foundation";
                    mailbody += "<h4 style='font-family:cambria'>Requirements: " + evnt.video + "</h4><br />";
                    mailmodel.body = mailbody + footer;
                    cm.SendEmail(mailmodel);
                }
                if (evnt.ITReq != null && evnt.ITReq != "")
                {
                    mailmodel.toemail = "it@fernandez.foundation,elroy@fernandez.foundation";
                    mailbody += "<h4 style='font-family:cambria'>Requirements: " + evnt.ITReq + "</h4><br />";
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

                    case 1:
                        if (evnt.ITReq != null && evnt.ITReq != "")
                        {
                            smstoIT = "8008902012,8008883508";
                            string Videomsg = smssub;
                            Videomsg += "Requirements: " + evnt.ITReq + "";
                            Videomsg += smsfooter;
                            sms.SendSmsToEmployee(smstoIT, Videomsg);                           
                        }
                        if (isMaintenancereq)
                        {
                            Maintenance = "8008902011,8008902003";
                            string Videomsg = smssub;
                            Videomsg += "Requirements: ";
                            if (evnt.Chair != null && evnt.Chair != "")
                            {
                                Videomsg += "No Of Chairs:" + evnt.Chair + " ";
                            }
                            if (evnt.Tables != null && evnt.Tables != "")
                            {
                                Videomsg += "No Of Tables:" + evnt.Tables + " ";
                            }
                            if (evnt.Arrangemnts != null && evnt.Arrangemnts != "")
                            {
                                Videomsg += "Arrangemnts:" + evnt.Arrangemnts + " ";
                            }
                            Videomsg += smsfooter;
                            sms.SendSmsToEmployee(Maintenance, Videomsg);
                        }
                        break;
                    case 2:
                        if (evnt.ITReq != null && evnt.ITReq != "")
                        {
                            smstoIT = "8008300040,8008883508";
                            string Videomsg = smssub;
                            Videomsg += "Requirements: " + evnt.ITReq + "";
                            Videomsg += smsfooter;
                            sms.SendSmsToEmployee(smstoIT, Videomsg);
                        }
                        if (isMaintenancereq)
                        {
                            Maintenance = "8008300041,8179004475";
                            string Videomsg = smssub;
                            Videomsg += "Requirements: ";
                            if (evnt.Chair != null && evnt.Chair != "")
                            {
                                Videomsg += "No Of Chairs:" + evnt.Chair + " ";
                            }
                            if (evnt.Tables != null && evnt.Tables != "")
                            {
                                Videomsg += "No Of Tables:" + evnt.Tables + " ";
                            }
                            if (evnt.Arrangemnts != null && evnt.Arrangemnts != "")
                            {
                                Videomsg += "Arrangemnts:" + evnt.Arrangemnts + " ";
                            }
                            Videomsg += smsfooter;
                            sms.SendSmsToEmployee(Maintenance, Videomsg);

                            mailmodel.toemail = "ayyavaru_m@fernandez.foundation";
                            mailbody += "<h4 style='font-family:cambria'>Requirements:</h4><br />";

                            if (evnt.Chair != null && evnt.Chair != "")
                            {
                                mailbody += "<h4 style='font-family:cambria'>No Of Chairs:" + evnt.Chair + "</h4><br /> ";
                            }
                            if (evnt.Tables != null && evnt.Tables != "")
                            {
                                mailbody += "<h4 style='font-family:cambria'>No Of Tables:" + evnt.Tables + "</h4><br /> ";
                            }
                            if (evnt.Arrangemnts != null && evnt.Arrangemnts != "")
                            {
                                mailbody += "<h4 style='font-family:cambria'>Arrangemnts:" + evnt.Arrangemnts + "</h4><br /> ";
                            }
                            mailmodel.body = mailbody + footer;
                            cm.SendEmail(mailmodel);
                        }
                        break;
                    case 3:
                        if (evnt.ITReq != null && evnt.ITReq != "")
                        {
                            smstoIT = "7337353760,8008883508";
                            string Videomsg = smssub;
                            Videomsg += "Requirements: " + evnt.ITReq + "";
                            Videomsg += smsfooter;
                            sms.SendSmsToEmployee(smstoIT, Videomsg);
                        }
                        if (isMaintenancereq)
                        {
                            Maintenance = "8008882005";
                            string Videomsg = smssub;
                            Videomsg += "Requirements: ";
                            if (evnt.Chair != null && evnt.Chair != "")
                            {
                                Videomsg += "No Of Chairs:" + evnt.Chair + " ";
                            }
                            if (evnt.Tables != null && evnt.Tables != "")
                            {
                                Videomsg += "No Of Tables:" + evnt.Tables + " ";
                            }
                            if (evnt.Arrangemnts != null && evnt.Arrangemnts != "")
                            {
                                Videomsg += "Arrangemnts:" + evnt.Arrangemnts + " ";
                            }
                            Videomsg += smsfooter;
                            sms.SendSmsToEmployee(Maintenance, Videomsg);
                        }
                        break;
                    case 4:
                        if (evnt.ITReq != null && evnt.ITReq != "")
                        {
                            smstoIT = "8008300040,8008883508";
                            string Videomsg = smssub;
                            Videomsg += "Requirements: " + evnt.ITReq + "";
                            Videomsg += smsfooter;
                            sms.SendSmsToEmployee(smstoIT, Videomsg);
                        }
                        if (isMaintenancereq)
                        {
                            Maintenance = "8008300041,8179003962";
                            string Videomsg = smssub;
                            Videomsg += "Requirements: ";
                            if (evnt.Chair != null && evnt.Chair != "")
                            {
                                Videomsg += "No Of Chairs:" + evnt.Chair + " ";
                            }
                            if (evnt.Tables != null && evnt.Tables != "")
                            {
                                Videomsg += "No Of Tables:" + evnt.Tables + " ";
                            }
                            if (evnt.Arrangemnts != null && evnt.Arrangemnts != "")
                            {
                                Videomsg += "Arrangemnts:" + evnt.Arrangemnts + " ";
                            }
                            Videomsg += smsfooter;
                            sms.SendSmsToEmployee(Maintenance, Videomsg);
                        }
                        break;
                    case 5:
                        if (evnt.ITReq != null && evnt.ITReq != "")
                        {
                            smstoIT = "7337353760,8008883508";
                            string Videomsg = smssub;
                            Videomsg += "Requirements: " + evnt.ITReq + "";
                            Videomsg += smsfooter;
                            sms.SendSmsToEmployee(smstoIT, Videomsg);
                        }
                        if (isMaintenancereq)
                        {
                            Maintenance = "8008902037,7337353776,8008902040";
                            string Videomsg = smssub;
                            Videomsg += "Requirements: ";
                            if (evnt.Chair != null && evnt.Chair != "")
                            {
                                Videomsg += "No Of Chairs:" + evnt.Chair + " ";
                            }
                            if (evnt.Tables != null && evnt.Tables != "")
                            {
                                Videomsg += "No Of Tables:" + evnt.Tables + " ";
                            }
                            if (evnt.Arrangemnts != null && evnt.Arrangemnts != "")
                            {
                                Videomsg += "Arrangemnts:" + evnt.Arrangemnts + " ";
                            }
                            Videomsg += smsfooter;
                            sms.SendSmsToEmployee(Maintenance, Videomsg);
                            mailmodel.toemail = " maintenance_sh @fernandez.foundation, assis@fernandez.foundation";
                            mailbody += "<h4 style='font-family:cambria'>Requirements:</h4><br />";

                            if (evnt.Chair != null && evnt.Chair != "")
                            {
                                mailbody += "<h4 style='font-family:cambria'>No Of Chairs:" + evnt.Chair + "</h4><br /> ";
                            }
                            if (evnt.Tables != null && evnt.Tables != "")
                            {
                                mailbody += "<h4 style='font-family:cambria'>No Of Tables:" + evnt.Tables + "</h4><br /> ";
                            }
                            if (evnt.Arrangemnts != null && evnt.Arrangemnts != "")
                            {
                                mailbody += "<h4 style='font-family:cambria'>Arrangemnts:" + evnt.Arrangemnts + "</h4><br /> ";
                            }
                            mailmodel.body = mailbody + footer;
                            cm.SendEmail(mailmodel);
                        }
                        break;
                }
            }
        }
    }
}