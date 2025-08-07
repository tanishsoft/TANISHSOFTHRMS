using ScheduleApplication.AttendanceUpdate;
using ScheduleApplication.Common;
using ScheduleApplication.DataModel;
using ScheduleApplication.HelpDesk;
using ScheduleApplication.LeavesAutoAdd;
using ScheduleApplication.Notifications;
using ScheduleApplication.OracleDBEmployee;
using System;
using System.Configuration;
using System.Linq;
namespace ScheduleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            string Evironmentcheck = ConfigurationManager.AppSettings["Environment"];
            SendAnEmail cm = new SendAnEmail();
            MyIntranetAppEntities myapp = new MyIntranetAppEntities();
            try
            {
               
                string SendTicketsStatus = "", Update15TicketsStatus = "", UpdateAttendance = "", SendTodayBirthday = "", SendWorkAnniversary = "",
                    SendRemainders = "", EmployeesElupdate = "", EmployeesCLSLUpdate = "", HelpDeskAutoClose = "", DoctorsLeaveUpdate = "";

                //string UpdateGetcompany = "", updateGetsubdepts = "", updateGetdesignation = "", updateGetEmployees = "";
                DateTime Date = DateTime.Now;
                DailyRunActivites drv = new DailyRunActivites();
                HelpDeskRemainderSend remsend = new HelpDeskRemainderSend();
                SendTicketsStatus = remsend.Sendtickets();
                UpdateTicketAuto upt = new UpdateTicketAuto();
                Update15TicketsStatus = upt.Updateticketevery15minutes();
                DoctorsPunchUpdate modelupdatedoctorsleave = new DoctorsPunchUpdate();
                //modelupdatedoctorsleave.UpdateDoctorsSwipeRecords();
                if (Date.Hour == 1 || Date.Hour == 6 || Date.Hour == 12 || Date.Hour == 18)
                {
                    //var historylist = myapp.tbl_JobsHistory.Where(l => l.JobName == "UpdateAttendance" && l.Environment == Evironmentcheck).ToList();
                    //historylist = historylist.Where(l => l.JobExecutedDate.Value.Date == Date.Date).ToList();
                    //if (historylist.Count == 0)
                    //{
                    //    UpdateAttendance uat = new UpdateAttendance();
                    //    UpdateAttendance = uat.UpdateAttendanceAndRoaster();
                    //    tbl_JobsHistory model = new tbl_JobsHistory();
                    //    model.JobExcutedHour = 6;
                    //    model.JobExcutedMinute = Date.Minute;
                    //    model.JobExecutedDate = Date;
                    //    model.Environment = Evironmentcheck;
                    //    model.JobName = "UpdateAttendance";
                    //    model.JobStatus = false;
                    //    if (UpdateAttendance == "Success")
                    //        model.JobStatus = true;
                    //    model.Message = UpdateAttendance;
                    //    myapp.tbl_JobsHistory.Add(model);
                    //    myapp.SaveChanges();
                    //}

                    //var historySendTodayBirthdayToUser = myapp.tbl_JobsHistory.Where(l => l.JobName == "SendTodayBirthdayToUser" && l.Environment == Evironmentcheck).ToList();
                    //historySendTodayBirthdayToUser = historySendTodayBirthdayToUser.Where(l => l.JobExecutedDate.Value.Date == Date.Date).ToList();
                    //if (historySendTodayBirthdayToUser.Count == 0)
                    //{
                    //    SendTodayBirthday = drv.SendTodayBirthdayToUser();
                    //    tbl_JobsHistory model = new tbl_JobsHistory();
                    //    model.JobExcutedHour = 6;
                    //    model.JobExcutedMinute = Date.Minute;
                    //    model.JobExecutedDate = Date;
                    //    model.Environment = Evironmentcheck;
                    //    model.JobName = "SendTodayBirthdayToUser";
                    //    model.JobStatus = false;
                    //    if (SendTodayBirthday == "Success")
                    //        model.JobStatus = true;
                    //    model.Message = SendTodayBirthday;
                    //    myapp.tbl_JobsHistory.Add(model);
                    //    myapp.SaveChanges();
                    //}
                    var SendworkanniversarytoUsersjob = myapp.tbl_JobsHistory.Where(l => l.JobName == "SendworkanniversarytoUsers" && l.Environment == Evironmentcheck).ToList();
                    SendworkanniversarytoUsersjob = SendworkanniversarytoUsersjob.Where(l => l.JobExecutedDate.Value.Date == Date.Date).ToList();
                    if (SendworkanniversarytoUsersjob.Count == 0)
                    {
                        SendWorkAnniversary = drv.SendworkanniversarytoUsers();
                        tbl_JobsHistory model = new tbl_JobsHistory();
                        model.JobExcutedHour = 6;
                        model.JobExcutedMinute = Date.Minute;
                        model.JobExecutedDate = Date;
                        model.Environment = Evironmentcheck;
                        model.JobName = "SendworkanniversarytoUsers";
                        model.JobStatus = false;
                        if (SendWorkAnniversary == "Success")
                            model.JobStatus = true;
                        model.Message = SendWorkAnniversary;
                        myapp.tbl_JobsHistory.Add(model);
                        myapp.SaveChanges();
                    }
                }
                if (Date.Hour >= 11)
                {
                    var SendRemainderstoUsersForNextdayjob = myapp.tbl_JobsHistory.Where(l => l.JobName == "SendRemainderstoUsersForNextday" && l.Environment == Evironmentcheck).ToList();
                    SendRemainderstoUsersForNextdayjob = SendRemainderstoUsersForNextdayjob.Where(l => l.JobExecutedDate.Value.Date == Date.Date).ToList();
                    if (SendRemainderstoUsersForNextdayjob.Count == 0)
                    {
                        SendRemainders = drv.SendRemainderstoUsersForNextday();
                        tbl_JobsHistory model = new tbl_JobsHistory();
                        model.JobExcutedHour = 14;
                        model.JobExcutedMinute = Date.Minute;
                        model.JobExecutedDate = Date;
                        model.Environment = Evironmentcheck;
                        model.JobName = "SendRemainderstoUsersForNextday";
                        model.JobStatus = false;
                        if (SendRemainders == "Success")
                            model.JobStatus = true;
                        model.Message = SendRemainders;
                        myapp.tbl_JobsHistory.Add(model);
                        myapp.SaveChanges();
                    }
                }
                if (Date.Hour == 1 || Date.Hour == 6 || Date.Hour == 12 || Date.Hour == 18)
                {

                    //HrManuvallyLeaveUpdate hrm = new HrManuvallyLeaveUpdate();
                    //var CheckEmployeeLeaveYearWisejob = myapp.tbl_JobsHistory.Where(l => l.JobName == "CheckEmployeeLeaveYearWise" && l.JobExcutedHour == Date.Hour && l.Environment == Evironmentcheck).ToList();
                    //CheckEmployeeLeaveYearWisejob = CheckEmployeeLeaveYearWisejob.Where(l => l.JobExecutedDate.Value.Date == Date.Date).ToList();
                    //if (CheckEmployeeLeaveYearWisejob.Count == 0)
                    //{
                    //    EmployeesElupdate = hrm.CheckEmployeeLeaveYearWise();
                    //    tbl_JobsHistory model = new tbl_JobsHistory();
                    //    model.JobExcutedHour = Date.Hour;
                    //    model.JobExcutedMinute = Date.Minute;
                    //    model.JobExecutedDate = Date;
                    //    model.Environment = Evironmentcheck;
                    //    model.JobName = "CheckEmployeeLeaveYearWise";
                    //    model.JobStatus = false;
                    //    if (EmployeesElupdate == "Success")
                    //        model.JobStatus = true;
                    //    model.Message = EmployeesElupdate;
                    //    myapp.tbl_JobsHistory.Add(model);
                    //    myapp.SaveChanges();
                    //}
                    //var UpdateCasuvalAndSickLeaves_Newjob = myapp.tbl_JobsHistory.Where(l => l.JobName == "UpdateCasuvalAndSickLeaves_New" && l.JobExcutedHour == Date.Hour && l.Environment == Evironmentcheck).ToList();
                    //UpdateCasuvalAndSickLeaves_Newjob = UpdateCasuvalAndSickLeaves_Newjob.Where(l => l.JobExecutedDate.Value.Date == Date.Date).ToList();
                    //if (UpdateCasuvalAndSickLeaves_Newjob.Count == 0)
                    //{
                    //    EmployeesCLSLUpdate = hrm.UpdateCasuvalAndSickLeaves_New();
                    //    tbl_JobsHistory model = new tbl_JobsHistory();
                    //    model.JobExcutedHour = Date.Hour;
                    //    model.JobExcutedMinute = Date.Minute;
                    //    model.JobExecutedDate = Date;
                    //    model.Environment = Evironmentcheck;
                    //    model.JobName = "UpdateCasuvalAndSickLeaves_New";
                    //    model.JobStatus = false;
                    //    if (EmployeesCLSLUpdate == "Success")
                    //        model.JobStatus = true;
                    //    model.Message = EmployeesCLSLUpdate;
                    //    myapp.tbl_JobsHistory.Add(model);
                    //    myapp.SaveChanges();
                    //}

                    //var UpdateYearlyDoctorsleavesjob = myapp.tbl_JobsHistory.Where(l => l.JobName == "UpdateYearlyDoctorsleaves" && l.JobExcutedHour == Date.Hour && l.Environment == Evironmentcheck).ToList();
                    //UpdateYearlyDoctorsleavesjob = UpdateYearlyDoctorsleavesjob.Where(l => l.JobExecutedDate.Value.Date == Date.Date).ToList();
                    //if (UpdateYearlyDoctorsleavesjob.Count == 0)
                    //{
                    //    //doctors
                    //    DoctorsLeaveUpdate = hrm.UpdateYearlyDoctorsleaves();
                    //    tbl_JobsHistory model = new tbl_JobsHistory();
                    //    model.JobExcutedHour = Date.Hour;
                    //    model.JobExcutedMinute = Date.Minute;
                    //    model.JobExecutedDate = Date;
                    //    model.Environment = Evironmentcheck;
                    //    model.JobName = "UpdateYearlyDoctorsleaves";
                    //    model.JobStatus = false;
                    //    if (DoctorsLeaveUpdate == "Success")
                    //        model.JobStatus = true;
                    //    model.Message = DoctorsLeaveUpdate;
                    //    myapp.tbl_JobsHistory.Add(model);
                    //    myapp.SaveChanges();
                    //}

                    var HelpDeskautoClosejob = myapp.tbl_JobsHistory.Where(l => l.JobName == "HelpDeskautoClose" && l.JobExcutedHour == Date.Hour && l.Environment == Evironmentcheck).ToList();
                    HelpDeskautoClosejob = HelpDeskautoClosejob.Where(l => l.JobExecutedDate.Value.Date == Date.Date).ToList();
                    if (HelpDeskautoClosejob.Count == 0)
                    {
                        //doctors
                        HelpDeskautoClose helps = new HelpDeskautoClose();
                        HelpDeskAutoClose = helps.UpdateHelpdesk();
                        tbl_JobsHistory model = new tbl_JobsHistory();
                        model.JobExcutedHour = Date.Hour;
                        model.JobExcutedMinute = Date.Minute;
                        model.JobExecutedDate = Date;
                        model.Environment = Evironmentcheck;
                        model.JobName = "HelpDeskautoClose";
                        model.JobStatus = false;
                        if (HelpDeskAutoClose == "Success")
                            model.JobStatus = true;
                        model.Message = HelpDeskAutoClose;
                        myapp.tbl_JobsHistory.Add(model);
                        myapp.SaveChanges();
                    }


                    //var ConnectOraclejob = myapp.tbl_JobsHistory.Where(l => l.JobName == "ConnectOracle"  && l.JobExcutedHour == Date.Hour && l.Environment == Evironmentcheck).ToList();
                    //ConnectOraclejob = ConnectOraclejob.Where(l => l.JobExecutedDate.Value.Date == Date.Date).ToList();
                    //if (ConnectOraclejob.Count == 0)
                    //{
                    //    ConnectOracle ocmodel = new ConnectOracle();
                    //    UpdateGetcompany = ocmodel.Getcompany();
                    //    updateGetsubdepts = ocmodel.Getsubdepts();
                    //    updateGetdesignation = ocmodel.Getdesignation();
                    //    updateGetEmployees = ocmodel.GetEmployees();
                    //    tbl_JobsHistory model = new tbl_JobsHistory();
                    //    model.JobExcutedHour = Date.Hour;
                    //    model.JobExcutedMinute = Date.Minute;
                    //    model.Environment = Evironmentcheck;
                    //    model.JobExecutedDate = Date;
                    //    model.JobName = "ConnectOracle";
                    //    model.JobStatus = false;
                    //    if (updateGetEmployees == "Success")
                    //        model.JobStatus = true;
                    //    model.Message = updateGetEmployees;
                    //    myapp.tbl_JobsHistory.Add(model);
                    //    myapp.SaveChanges();
                    //}


                }
            }
            catch (Exception ex)
            {
                MailModel mailmodel = new MailModel();
                mailmodel.fromemail = "helpdesk@fernandez.foundation";
                mailmodel.toemail = "vamsi@microarctech.com";
                mailmodel.ccemail = "ahmadali@fernandez.foundation";
                //mailmodel.ccemail = "elroy@fernandez.foundation,vamsi@microarctech.com";

                mailmodel.subject = "Error While runing attendance Jobs";
                mailmodel.body = "Hi <br /><p>" + ex + "</p>";
                mailmodel.filepath = "";
                mailmodel.username = "Fernandez hospital jobs";
                mailmodel.fromname = "Fernandez hospital jobs";
                cm.SendEmail(mailmodel);
            }

        }
    }
}
