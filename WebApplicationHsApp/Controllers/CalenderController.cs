using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class CalenderController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Calender
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BookRoom()
        {
            var cuser = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).Single();
            ViewBag.CurrentUser = cuser;
            return View();
        }
        public JsonResult SaveEvent(tbl_Event evnt)
        {
            var cuser = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).Single();
            evnt.EventOwnerName = cuser.FirstName + " " + cuser.LastName;
            evnt.CreatedBy = User.Identity.Name;
            evnt.CreatedOn = DateTime.Now;
            evnt.ModifiedBy = User.Identity.Name;
            evnt.ModifiedOn = DateTime.Now;
            myapp.tbl_Event.Add(evnt);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDiaryEvents(string start, string end)
        {
            var events = myapp.tbl_Event.ToList();
            if (Request.IsAuthenticated)
            {
                events = events.Where(u => u.CreatedBy == User.Identity.Name).ToList();
            }
            //var eventslist = myapp.tbl_Event.Where(u => u.CreatedBy == User.Identity.Name).ToList();

            var eventList = from e in events
                            select new
                            {
                                id = e.EventId,
                                title = e.EventTitle,
                                start = e.EventDate + " " + e.EventTime,
                                end = e.EventDuration,
                                color = "",
                                allDay = false,
                                //resources = Data.Entitylist.Single(en => en.CHlId == e.chlid.ToString()).Department
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
        public bool ValidateBookingExistsornot(DateTime Dt, List<tbl_Booking> bookdaylist, string StartTime, string EndTime)
        {
            bool isnewbooking = true;
            foreach (var bdl in bookdaylist)
            {
                var FromDate = Convert.ToDateTime(Dt.ToShortDateString() + " " + StartTime);
                var ToDate = Convert.ToDateTime(Dt.ToShortDateString() + " " + EndTime);
                var CFromDate = Convert.ToDateTime(bdl.FromDate.Value.ToShortDateString() + " " + bdl.StartTime);
                var CToDate = Convert.ToDateTime(bdl.FromDate.Value.ToShortDateString() + " " + bdl.EndTime);
                if ((FromDate < CToDate) && (CFromDate < ToDate))
                {
                    isnewbooking = false;
                }
                //if ((ToDate < CFromDate) && (ToDate > CToDate) )
                //{
                //    isnewbooking = false;
                //}
            }
            return isnewbooking;
        }
        public JsonResult SaveBooking(BookingViewModel model)
        {
            tbl_Booking evnt = new tbl_Booking();
            evnt.Titles = model.Titles;
            string sch = model.Repeatsch;
            evnt.FromDate = ProjectConvert.ConverDateStringtoDatetime(model.FromDate);
            evnt.ToDate = ProjectConvert.ConverDateStringtoDatetime(model.FromDate);
            evnt.StartTime = model.StartTime;
            evnt.EndTime = model.EndTime;
            evnt.EventDescription = model.EventDescription;
            evnt.LocationId = model.LocationId;
            evnt.LocationName = model.LocationName;
            evnt.Eventinvitesgroupid = model.Eventinvitesgroupid;
            evnt.EventUnattend = model.EventUnattend;
            evnt.Eventdocument = model.Eventdocument;
            evnt.RoomId = model.RoomId;
            evnt.RoomName = model.RoomName;
            evnt.RoomExtension = model.RoomExtension;
            evnt.FloorId = model.FloorId;
            evnt.FloorName = model.FloorName;
            evnt.BuildingId = model.BuildingId;
            evnt.BuildingName = model.BuildingName;
            evnt.Chair = model.Chair;
            evnt.Tables = model.Tables;
            evnt.ITReq = model.ITReq;
            evnt.video = model.video;
            evnt.Arrangemnts = model.Arrangemnts;
            evnt.DepartLocation = model.DepartLocation;
            evnt.NameEmpl = model.NameEmpl;
            evnt.NameDepartment = model.NameDepartment;
            evnt.UGC_Extension = model.UGC_Extension;
            evnt.EmailID = model.EmailID;
            evnt.EventColor = model.EventColor;

            var cuser = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).Single();
            var EvtFromDate = Convert.ToDateTime(evnt.FromDate.Value.ToShortDateString() + " " + evnt.StartTime);
            var EvtToDate = Convert.ToDateTime(evnt.FromDate.Value.ToShortDateString() + " " + evnt.EndTime);
            if (EvtFromDate < EvtToDate)
            {
                var bookdaylist = myapp.tbl_Booking.Where(e => e.LocationId == evnt.LocationId && e.BuildingId == evnt.BuildingId && e.FloorId == evnt.FloorId && e.FromDate == evnt.FromDate).ToList();
                if (bookdaylist.Count > 0)
                {

                    if (model.RepeatChecked && model.RepeatEndDate != null && model.RepeatEndDate != "")
                    {
                        if (model.Repeatsch == "Daily")
                        {
                            DateTime RepeatEnddate = ProjectConvert.ConverDateStringtoDatetime(model.RepeatEndDate);
                            DateTime StartDate = evnt.FromDate.Value;
                            int dayofweek = 8;
                            if (model.Weeklyleave != null && model.Weeklyleave != "")
                            {
                                dayofweek = int.Parse(model.Weeklyleave);
                            }
                            while (StartDate <= RepeatEnddate)
                            {
                                if ((int)StartDate.DayOfWeek != dayofweek)
                                {
                                    if (ValidateBookingExistsornot(StartDate, bookdaylist, evnt.StartTime, evnt.EndTime))
                                    {
                                        evnt.FromDate = StartDate;
                                        evnt.ToDate = StartDate;
                                        evnt.DepartLocation = cuser.LocationName;
                                        evnt.NameEmpl = cuser.FirstName + " " + cuser.LastName;
                                        evnt.NameDepartment = cuser.DepartmentName;
                                        evnt.IsActive = cuser.IsActive;
                                        evnt.CreatedBy = User.Identity.Name;
                                        evnt.CreatedOn = DateTime.Now;
                                        evnt.ModifiedBy = User.Identity.Name;
                                        evnt.ModifiedOn = DateTime.Now;
                                        myapp.tbl_Booking.Add(evnt);
                                        myapp.SaveChanges();
                                    }
                                    else
                                    {
                                        return Json("Another Meeting is booked at that time in same location please try another location ", JsonRequestBehavior.AllowGet);
                                    }
                                }
                                StartDate = StartDate.AddDays(1);
                            }
                        }
                        else if (model.Repeatsch == "Weekly")
                        {
                            DateTime RepeatEnddate = ProjectConvert.ConverDateStringtoDatetime(model.RepeatEndDate);
                            DateTime StartDate = evnt.FromDate.Value;
                            int dayofweek = 8;
                            if (model.Weeklyleave != null && model.Weeklyleave != "")
                            {
                                dayofweek = int.Parse(model.Weeklyleave);
                            }
                            while (StartDate <= RepeatEnddate)
                            {
                                if ((int)StartDate.DayOfWeek != dayofweek)
                                {
                                    if (ValidateBookingExistsornot(StartDate, bookdaylist, evnt.StartTime, evnt.EndTime))
                                    {
                                        evnt.FromDate = StartDate;
                                        evnt.ToDate = StartDate;
                                        evnt.DepartLocation = cuser.LocationName;
                                        evnt.NameEmpl = cuser.FirstName + " " + cuser.LastName;
                                        evnt.NameDepartment = cuser.DepartmentName;
                                        evnt.IsActive = cuser.IsActive;
                                        evnt.CreatedBy = User.Identity.Name;
                                        evnt.CreatedOn = DateTime.Now;
                                        evnt.ModifiedBy = User.Identity.Name;
                                        evnt.ModifiedOn = DateTime.Now;
                                        myapp.tbl_Booking.Add(evnt);
                                        myapp.SaveChanges();
                                    }
                                    else
                                    {
                                        return Json("Another Meeting is booked at that time in same location please try another location ", JsonRequestBehavior.AllowGet);
                                    }
                                }
                                StartDate = StartDate.AddDays(7);
                            }
                        }
                        else if (model.Repeatsch == "Monthly")
                        {
                            DateTime RepeatEnddate = ProjectConvert.ConverDateStringtoDatetime(model.RepeatEndDate);
                            DateTime StartDate = evnt.FromDate.Value;
                            int dayofweek = 8;
                            if (model.Weeklyleave != null && model.Weeklyleave != "")
                            {
                                dayofweek = int.Parse(model.Weeklyleave);
                            }
                            while (StartDate <= RepeatEnddate)
                            {
                                if ((int)StartDate.DayOfWeek != dayofweek)
                                {
                                    if (ValidateBookingExistsornot(StartDate, bookdaylist, evnt.StartTime, evnt.EndTime))
                                    {
                                        evnt.FromDate = StartDate;
                                        evnt.ToDate = StartDate;
                                        evnt.DepartLocation = cuser.LocationName;
                                        evnt.NameEmpl = cuser.FirstName + " " + cuser.LastName;
                                        evnt.NameDepartment = cuser.DepartmentName;
                                        evnt.IsActive = cuser.IsActive;
                                        evnt.CreatedBy = User.Identity.Name;
                                        evnt.CreatedOn = DateTime.Now;
                                        evnt.ModifiedBy = User.Identity.Name;
                                        evnt.ModifiedOn = DateTime.Now;
                                        myapp.tbl_Booking.Add(evnt);
                                        myapp.SaveChanges();
                                    }
                                    else
                                    {
                                        return Json("Another Meeting is booked at that time in same location please try another location ", JsonRequestBehavior.AllowGet);
                                    }
                                }
                                StartDate = StartDate.AddMonths(1);
                            }
                        }
                    }
                    else
                    {
                        if (ValidateBookingExistsornot(evnt.FromDate.Value, bookdaylist, evnt.StartTime, evnt.EndTime))
                        {
                            evnt.DepartLocation = cuser.LocationName;
                            evnt.NameEmpl = cuser.FirstName + " " + cuser.LastName;
                            evnt.NameDepartment = cuser.DepartmentName;
                            evnt.IsActive = cuser.IsActive;
                            evnt.CreatedBy = User.Identity.Name;
                            evnt.CreatedOn = DateTime.Now;
                            evnt.ModifiedBy = User.Identity.Name;
                            evnt.ModifiedOn = DateTime.Now;
                            myapp.tbl_Booking.Add(evnt);
                            myapp.SaveChanges();
                        }
                        else
                        {
                            return Json("Another Meeting is booked at that time in same location please try another location ", JsonRequestBehavior.AllowGet);
                        }
                    }

                }
                else
                {
                    if (model.RepeatChecked && model.RepeatEndDate != null && model.RepeatEndDate != "")
                    {
                        if (model.Repeatsch == "Daily")
                        {
                            DateTime RepeatEnddate = ProjectConvert.ConverDateStringtoDatetime(model.RepeatEndDate);
                            DateTime StartDate = evnt.FromDate.Value;
                            int dayofweek = 8;
                            if (model.Weeklyleave != null && model.Weeklyleave != "")
                            {
                                dayofweek = int.Parse(model.Weeklyleave);
                            }
                            while (StartDate <= RepeatEnddate)
                            {
                                if ((int)StartDate.DayOfWeek != dayofweek)
                                {
                                    evnt.FromDate = StartDate;
                                    evnt.ToDate = StartDate;
                                    evnt.DepartLocation = cuser.LocationName;
                                    evnt.NameEmpl = cuser.FirstName + " " + cuser.LastName;
                                    evnt.NameDepartment = cuser.DepartmentName;
                                    evnt.IsActive = cuser.IsActive;
                                    evnt.CreatedBy = User.Identity.Name;
                                    evnt.CreatedOn = DateTime.Now;
                                    evnt.ModifiedBy = User.Identity.Name;
                                    evnt.ModifiedOn = DateTime.Now;
                                    myapp.tbl_Booking.Add(evnt);
                                    myapp.SaveChanges();
                                }
                                StartDate = StartDate.AddDays(1);
                            }
                        }
                        else if (model.Repeatsch == "Weekly")
                        {
                            DateTime RepeatEnddate = ProjectConvert.ConverDateStringtoDatetime(model.RepeatEndDate);
                            DateTime StartDate = evnt.FromDate.Value;
                            int dayofweek = 8;
                            if (model.Weeklyleave != null && model.Weeklyleave != "")
                            {
                                dayofweek = int.Parse(model.Weeklyleave);
                            }
                            while (StartDate <= RepeatEnddate)
                            {
                                if ((int)StartDate.DayOfWeek != dayofweek)
                                {
                                    evnt.FromDate = StartDate;
                                    evnt.ToDate = StartDate;
                                    evnt.DepartLocation = cuser.LocationName;
                                    evnt.NameEmpl = cuser.FirstName + " " + cuser.LastName;
                                    evnt.NameDepartment = cuser.DepartmentName;
                                    evnt.IsActive = cuser.IsActive;
                                    evnt.CreatedBy = User.Identity.Name;
                                    evnt.CreatedOn = DateTime.Now;
                                    evnt.ModifiedBy = User.Identity.Name;
                                    evnt.ModifiedOn = DateTime.Now;
                                    myapp.tbl_Booking.Add(evnt);
                                    myapp.SaveChanges();
                                }
                                StartDate = StartDate.AddDays(7);
                            }
                        }
                        else if (model.Repeatsch == "Monthly")
                        {

                            DateTime RepeatEnddate = ProjectConvert.ConverDateStringtoDatetime(model.RepeatEndDate);
                            DateTime StartDate = evnt.FromDate.Value;
                            int dayofweek = 8;
                            if (model.Weeklyleave != null && model.Weeklyleave != "")
                            {
                                dayofweek = int.Parse(model.Weeklyleave);
                            }
                            while (StartDate <= RepeatEnddate)
                            {
                                if ((int)StartDate.DayOfWeek != dayofweek)
                                {
                                    evnt.FromDate = StartDate;
                                    evnt.ToDate = StartDate;
                                    evnt.DepartLocation = cuser.LocationName;
                                    evnt.NameEmpl = cuser.FirstName + " " + cuser.LastName;
                                    evnt.NameDepartment = cuser.DepartmentName;
                                    evnt.IsActive = cuser.IsActive;
                                    evnt.CreatedBy = User.Identity.Name;
                                    evnt.CreatedOn = DateTime.Now;
                                    evnt.ModifiedBy = User.Identity.Name;
                                    evnt.ModifiedOn = DateTime.Now;
                                    myapp.tbl_Booking.Add(evnt);
                                    myapp.SaveChanges();
                                }
                                StartDate = StartDate.AddMonths(1);
                            }
                        }
                    }
                    else
                    {
                        evnt.DepartLocation = cuser.LocationName;
                        evnt.NameEmpl = cuser.FirstName + " " + cuser.LastName;
                        evnt.NameDepartment = cuser.DepartmentName;
                        evnt.IsActive = cuser.IsActive;
                        evnt.CreatedBy = User.Identity.Name;
                        evnt.CreatedOn = DateTime.Now;
                        evnt.ModifiedBy = User.Identity.Name;
                        evnt.ModifiedOn = DateTime.Now;
                        myapp.tbl_Booking.Add(evnt);
                        myapp.SaveChanges();
                    }
                }
            }
            else
            {
                return Json("The End Time Should be grater than start time ", JsonRequestBehavior.AllowGet);
            }
            //if (evnt.FromDate.Value.Date == DateTime.Now.Date)
            //{

            SendSms sms = new SendSms();
            CustomModel cm = new CustomModel();

            MailModel mailmodel = new MailModel();

            mailmodel.fromemail = "it_bookConference@fernandez.foundation";
            mailmodel.subject = cuser.FirstName + " Event " + evnt.Titles + " on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + "   Time : " + evnt.StartTime + " to " + evnt.EndTime;
            string mailbody = "<p style='font-family:verdana;font-size:14px;'>A New Meeting  on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + "   Time : " + evnt.StartTime + " to " + evnt.EndTime + "</p>";
            string footer = "";
            string smstoIT = "", smstoAudio = "", Maintenance = "";
            string smssub = "", smsfooter = "";
            smssub = cuser.FirstName + " Event " + evnt.Titles + " on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + " Time :" + evnt.StartTime + " to " + evnt.EndTime + ".";
            smsfooter = "Location: " + evnt.LocationName + " - " + evnt.BuildingName + " - " + evnt.FloorName + " Created By : " + cuser.FirstName;
            footer += "<p  style='font-family:verdana;font-size:14px;'>Location: " + evnt.LocationName + " - " + evnt.BuildingName + " - " + evnt.FloorName + " Created By : " + cuser.FirstName + " </p>";
            footer += "<br/><p  style='font-family:verdana;font-size:12px;'>This is an auto generated mail,Don't reply to this.</p>";
            mailmodel.filepath = "";
            mailmodel.fromname = cuser.FirstName + "  Event " + evnt.Titles + " on " + evnt.FromDate.Value.ToString("dd/MM/yyyy");
            mailmodel.ccemail = "";

            if (evnt.video != null && evnt.video != "")
            {
                smstoAudio = "8179003925";
                string Videomsg = smssub;
                Videomsg += "Requirements: " + evnt.video + "";
                Videomsg += smsfooter;
                sms.SendSmsToEmployee(smstoAudio, Videomsg);
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
                mailmodel.ccemail = "elroy@fernandez.foundation, edward.c@fernandez.foundation";
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
                smstoIT = "7995007710, 8008802214, 8008883508";
                string Videomsg = cuser.FirstName + " Event " + evnt.Titles + " on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + " Time is " + evnt.StartTime + " to " + evnt.EndTime + ".";
                Videomsg += "Requirements: " + evnt.ITReq + "";
                Videomsg += "Location: " + evnt.LocationName + "-" + evnt.BuildingName + "-" + evnt.FloorName + " Created By : " + cuser.FirstName;
                sms.SendSmsToEmployee(smstoIT, Videomsg);
            }

            if ((evnt.Chair != null && evnt.Chair != "") || (evnt.Tables != null && evnt.Tables != "") || (evnt.Arrangemnts != null && evnt.Arrangemnts != ""))
            {
                mailmodel.ccemail = "";
                switch (evnt.LocationId)
                {
                    case 1:
                        Maintenance = "8008908011";
                        mailmodel.toemail = "ramesh.v@fernandez.foundation";
                        break;
                    case 2:
                    case 3:
                    case 4:
                        Maintenance = "8008300041";
                        mailmodel.toemail = "ayyavaru_m@fernandez.foundation";
                        break;
                    case 5:
                        Maintenance = "8008500591,8008902040";
                        mailmodel.toemail = "anthony_p@fernandez.foundation";
                        mailmodel.ccemail = "assis@fernandez.foundation";
                        break;
                    case 12:
                        mailmodel.toemail = "academics@fernandez.foundation";
                        if (evnt.FloorId == 23 && evnt.BuildingId == 18)
                        {
                            mailmodel.ccemail = "midwifery@fernandez.foundation,principal.fsn@fernandez.foundation,admin.fsn@fernandez.foundation";
                        }
                        break;
                }

                string Videomsg = cuser.FirstName + " Event " + evnt.Titles + " on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + " Time is " + evnt.StartTime + " to " + evnt.EndTime + ".";

                Videomsg += "Requirements: ";
                if (evnt.Chair != null && evnt.Chair != "")
                {
                    Videomsg += "No Of Chairs: " + evnt.Chair + " ";
                }
                if (evnt.Tables != null && evnt.Tables != "")
                {
                    Videomsg += "No Of Tables: " + evnt.Tables + " ";
                }
                if (evnt.Arrangemnts != null && evnt.Arrangemnts != "")
                {
                    Videomsg += "Arrangemnts: " + evnt.Arrangemnts + " ";
                }
                Videomsg += "Location: " + evnt.LocationName + " - " + evnt.BuildingName + " - " + evnt.FloorName + " Created By : " + cuser.FirstName;
                sms.SendSmsToEmployee(Maintenance, Videomsg);


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

            //academics mail send option
            mailbody = "";
            mailmodel.toemail = "academics@fernandez.foundation";
            mailmodel.ccemail = "abhishek_sengupta@fernandez.foundation";

            // cm.SendEmail(mailmodel);
            //}
            //sms implementation
            return Json("Success", JsonRequestBehavior.AllowGet);

        }
        public JsonResult GetEventsByParams(string date, string todate, int LocationId = 0, int BuildingId = 0)
        {
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(date, "dd/MM/yyyy");
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(todate, "dd/MM/yyyy");
            var events = myapp.tbl_Booking.Where(e => e.FromDate >= dtfrom && e.ToDate <= dtto).ToList();
            if (LocationId > 0)
            {
                events = events.Where(l => l.LocationId == LocationId).ToList();
            }
            if (BuildingId > 0)
            {
                events = events.Where(l => l.BuildingId == BuildingId).ToList();
            }
            var eventslist = (from e in events
                              join app1 in myapp.tbl_User on e.CreatedBy equals app1.CustomUserId
                              select new
                              {
                                  LocationName = e.LocationName,
                                  BuildingName = e.BuildingName,
                                  FloorName = e.FloorName,
                                  FromDate = e.FromDate.Value.ToString("dd/MM/yyyy"),
                                  StartTime = e.StartTime,
                                  EndTime = e.EndTime,
                                  Titles = e.Titles,
                                  CreatedBy = app1.FirstName
                              }
                            ).ToList();
            return Json(eventslist, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetBookingEvents(string start, string end, int LocationId = 0,int BuildingId=0)
        {
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(start, "yyyy-MM-dd");
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(end, "yyyy-MM-dd");
            var events = myapp.tbl_Booking.Where(e => e.FromDate >= dtfrom && e.ToDate <= dtto).ToList();
            if (LocationId > 0)
            {
                events = events.Where(l => l.LocationId == LocationId).ToList();
            }
            if (BuildingId > 0)
            {
                events = events.Where(l => l.BuildingId == BuildingId).ToList();
            }
            var eventList = from e in events
                            let dts = DateTime.Parse(e.StartTime)
                            let dte = DateTime.Parse(e.EndTime)
                            select new
                            { 
                                id = e.BookingId,
                                title = e.EventDescription,
                                //  start = e.EventDate.Value.ToShortDateString() + " " + e.EventTime,
                                start = e.FromDate.Value.ToString("yyyy-MM-dd") + "T" + dts.ToString("HH:mm"),
                                end = e.ToDate.Value.ToString("yyyy-MM-dd") + "T" + dte.ToString("HH:mm"),
                                color = e.EventColor,
                                allDay = false
                                //resources = Data.Entitylist.Single(en => en.CHlId == e.chlid.ToString()).Department
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetFullEvent(int EventId)
        {
            var eventlist = myapp.tbl_Event.Where(e => e.EventId == EventId).ToList();
            if (eventlist.Count > 0)
            {
                return Json(eventlist[0], JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NA", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetFullBookingEvent(int EventId)
        {
            var eventlist = myapp.tbl_Booking.Where(e => e.BookingId == EventId).ToList();
            if (eventlist.Count > 0)
            {
                var ev = eventlist[0];
                BookCalenderModel bcm = new BookCalenderModel();
                bcm.Arrangemnts = ev.Arrangemnts;
                bcm.BookingId = ev.BookingId;
                bcm.BuildingId = ev.BuildingId;
                bcm.BuildingName = ev.BuildingName;
                bcm.Chair = ev.Chair;
                bcm.CreatedBy = ev.CreatedBy;
                bcm.CreatedOn = ev.CreatedOn;
                bcm.DepartLocation = ev.DepartLocation;
                bcm.EmailID = ev.EmailID;
                bcm.EndTime = ev.EndTime;
                bcm.EventDescription = ev.EventDescription;
                bcm.Eventdocument = ev.Eventdocument;
                bcm.Eventinvitesgroupid = ev.Eventinvitesgroupid;
                bcm.EventUnattend = ev.EventUnattend;
                bcm.FloorId = ev.FloorId;
                bcm.FloorName = ev.FloorName;
                bcm.FromDate = ev.FromDate.Value.ToString("dd/MM/yyyy");
                bcm.IsActive = ev.IsActive;
                bcm.ITReq = ev.ITReq;
                bcm.LocationId = ev.LocationId;
                bcm.LocationName = ev.LocationName;
                bcm.ModifiedBy = ev.ModifiedBy;
                bcm.ModifiedOn = ev.ModifiedOn;
                bcm.NameDepartment = ev.NameDepartment;
                bcm.NameEmpl = ev.NameEmpl;
                bcm.RoomExtension = ev.RoomExtension;
                bcm.RoomId = ev.RoomId;
                bcm.RoomName = ev.RoomName;
                bcm.StartTime = ev.StartTime;
                bcm.Tables = ev.Tables;
                bcm.Titles = ev.Titles;
                bcm.ToDate = ev.ToDate.Value.ToString("dd/MM/yyyy");
                bcm.UGC_Extension = ev.UGC_Extension;
                bcm.video = ev.video;

                return Json(bcm, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NA", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult UpdateBooking(tbl_Booking evnt)
        {
            var eventlist = myapp.tbl_Booking.Where(e => e.BookingId == evnt.BookingId).ToList();
            if (eventlist.Count > 0)
            {
                eventlist[0].Titles = evnt.Titles;
                eventlist[0].StartTime = evnt.StartTime;
                eventlist[0].EndTime = evnt.EndTime;
                eventlist[0].EventDescription = evnt.EventDescription;
                eventlist[0].Chair = evnt.Chair;
                eventlist[0].Tables = evnt.Tables;
                eventlist[0].ITReq = evnt.ITReq;
                eventlist[0].video = evnt.video;
                eventlist[0].UGC_Extension = evnt.UGC_Extension;
                eventlist[0].EmailID = evnt.EmailID;
                myapp.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("NA", JsonRequestBehavior.AllowGet);
            }
        }
        public bool VerifyUserAbletoEdit(int id)
        {
            var eventlist = myapp.tbl_Booking.Where(e => e.BookingId == id).ToList();
            if (eventlist.Count > 0)
            {
                if (eventlist[0].CreatedBy == User.Identity.Name)
                {
                    DateTime Dtrcheck = Convert.ToDateTime(eventlist[0].FromDate.Value.ToShortDateString() + " " + eventlist[0].StartTime);
                    if (Dtrcheck > DateTime.Now)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else return false;
            }
            else return false;
        }
        public ActionResult CancelRoomBooking(int id)
        {
            var cuser = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).Single();

            var eventlist = myapp.tbl_Booking.Where(e => e.BookingId == id).ToList();
            if (eventlist.Count > 0)
            {
                if ((eventlist[0].CreatedBy == User.Identity.Name || cuser.DepartmentName == "Academics") || User.IsInRole("Admin"))
                {
                    var evnt = eventlist[0];
                    string mobile = eventlist[0].UGC_Extension;
                    myapp.tbl_Booking.Remove(eventlist[0]);
                    myapp.SaveChanges();
                  
                    SendSms sms = new SendSms();
                    CustomModel cm = new CustomModel();

                    MailModel mailmodel = new MailModel();

                    mailmodel.fromemail = "it_bookConference@fernandez.foundation";
                    mailmodel.subject ="Event is Cancelled : "+ cuser.FirstName + " Event " + evnt.Titles + " on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + "   Time : " + evnt.StartTime + " to " + evnt.EndTime;
                    string mailbody = "<p style='font-family:verdana;font-size:14px;'>Meeting  on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + "   Time : " + evnt.StartTime + " to " + evnt.EndTime + " has Cancelled </p>";
                    string footer = "";
                 
                    string smssub = "", smsfooter = "";
                    smssub = cuser.FirstName + " Event " + evnt.Titles + " on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + " Time :" + evnt.StartTime + " to " + evnt.EndTime + ".";
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
                        mailbody = "<p style='font-family:verdana;font-size:14px;'> Meeting  on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + "   Time : " + evnt.StartTime + " to " + evnt.EndTime + " has Cancelled </p>";
                        mailmodel.toemail = "it@fernandez.foundation";
                        mailmodel.ccemail = "elroy@fernandez.foundation, edward.c@fernandez.foundation";
                        mailbody += "<p style='font-family:verdana;font-size:14px;'>Hello,</p>";
                        mailbody += "<p style='font-family:verdana;font-size:14px;'>The conference room at " + evnt.LocationName + "-" + evnt.BuildingName + "-" + evnt.FloorName + " has been booked by " + cuser.FirstName + " for " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + " Time is " + evnt.StartTime + " to " + evnt.EndTime + "  has Cancelled .</p> ";
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

                    if ((evnt.Chair != null && evnt.Chair != "") || (evnt.Tables != null && evnt.Tables != "") || (evnt.Arrangemnts != null && evnt.Arrangemnts != ""))
                    {
                        mailmodel.ccemail = "";
                        switch (evnt.LocationId)
                        {
                            case 1:
                               
                                mailmodel.toemail = "ramesh.v@fernandez.foundation";
                                break;
                            case 2:
                            case 3:
                            case 4:
                              
                                mailmodel.toemail = "ayyavaru_m@fernandez.foundation";
                                break;
                            case 5:
                              
                                mailmodel.toemail = "anthony_p@fernandez.foundation";
                                mailmodel.ccemail = "assis@fernandez.foundation";
                                break;
                            case 12:
                                mailmodel.toemail = "academics@fernandez.foundation";
                                if (evnt.FloorId == 23 && evnt.BuildingId == 18)
                                {
                                    mailmodel.ccemail = "midwifery@fernandez.foundation,principal.fsn@fernandez.foundation,admin.fsn@fernandez.foundation";
                                }
                                break;
                        }

                 


                        mailbody = "<p style='font-family:verdana;font-size:14px;'> Meeting  on " + evnt.FromDate.Value.ToString("dd/MM/yyyy") + "   Time : " + evnt.StartTime + " to " + evnt.EndTime + "has Cancelled</p>";
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



                    if (eventlist[0].CreatedBy != User.Identity.Name && cuser.DepartmentName == "Academics" && mobile != null && mobile != "")
                    {
                         sms = new SendSms();
                        string msg = "Meeting Date : " + eventlist[0].FromDate.Value.ToString("dd/MM/yyyy") + " Time :" + eventlist[0].StartTime + " to " + eventlist[0].EndTime + ".";
                        msg += "is deleted by  " + cuser.FirstName + "";
                        sms.SendSmsToEmployee(mobile, msg);
                    }
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(" You Dont Have Access To Cancel Please Contact Creator", JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json("NA", JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetByRoom()
        {
            var list = (from a in myapp.tbl_Room
                        where a.IsActive == true
                        select new
                        {
                            id = a.RoomId,
                            name = a.RoomName
                        }).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
    }
}