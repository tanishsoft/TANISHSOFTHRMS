using System;
using System.Linq;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;
using WebApplicationHsApp.OracleInterface;

namespace WebApplicationHsApp.Controllers
{
    [AllowAnonymous]
    public class TaskScheduleController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: TaskSchedule
        public ActionResult Index()
        {
            return View();
        }
        public string Updateticketevery15minutes()
        {
            string jobrunstauts = "Success";
            try
            {
                DateTime Dt = DateTime.Now.AddDays(-2);
                var ticekts = myapp.tbl_Task.Where(l => l.AssignStatus == "New" && l.CallDateTime >= Dt && (l.AssignDepartmentName.ToLower() == "it" || l.AssignDepartmentName.ToLower() == "information technology")).ToList();
                foreach (var v in ticekts)
                {
                    TimeSpan ts = DateTime.Now - v.CallDateTime.Value;

                    if (ts.TotalMinutes > 15)
                    {
                        v.AssignId = 8182;
                        v.AssignName = "MOHD AHMAD ALI";
                        v.CallStartDateTime = DateTime.Now;
                        v.ModifiedOn = DateTime.Now;
                        v.AssignStatus = "In Progress";
                        v.CreatorStatus = "In Progress";
                        myapp.SaveChanges();
                        tbl_User curuser = (from urs in myapp.tbl_User where urs.UserId == 8182 select urs).SingleOrDefault();
                        if (curuser.EmailId != null)
                        {
                            CustomModel cm = new CustomModel();
                            MailModel mailmodel = new MailModel();
                            EmailTeamplates emailtemp = new EmailTeamplates();
                            mailmodel.fromemail = "Leave@hospitals.com";
                            mailmodel.toemail = curuser.EmailId;
                            mailmodel.subject = "A Ticket " + v.TaskId + " Assigned to you ";
                            string mailbody = "<p style='font-family:verdana'>Dear " + curuser.FirstName + ",";
                            string customuserid = User.Identity.Name;

                            mailbody += "<p style='font-family:verdana'>The System has assigned a task to you. Please click on this <a target='_blank' href='https://infonet.fernandezhospital.com/'>link</a>  to see the task.Do not forget to update the task status after completion.</p>";
                            mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                            mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Call Date</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + v.CallDateTime + "</td></tr>";
                            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Creator Department</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + v.CreatorDepartmentName + "</td></tr>";
                            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Creator Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + v.CreatorName + "</td></tr>";
                            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Category Of Complaint</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + v.CategoryOfComplaint + "</td></tr>";
                            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Subject</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + v.Subject + "</td></tr>";
                            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Description</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + v.Description + "</td></tr>";
                            mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Extension No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + v.AssertEquipId + "</td></tr>";
                            mailbody += "</table>";
                            mailbody += "<br/><p style='font-family:cambria'>This is an auto generated mail,Don't reply to this.</p>";
                            mailmodel.body = mailbody;
                            //mailmodel.body = "A New Ticket Assigned to you";
                            //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                            mailmodel.filepath = "";
                            //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                            mailmodel.fromname = "Help Desk";
                            mailmodel.ccemail = "vamsirm26@gmail.com";
                            cm.SendEmail(mailmodel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                jobrunstauts = "Error" + ex.Message;
            }
            return jobrunstauts;
        }

        public string UpdatePatientInfo15minutes()
        {
            ConnectOracle modelcon = new ConnectOracle();
            string jobrunstauts = "Success";
            try
            {
                jobrunstauts = modelcon.UpdatePatientInfo();
            }
            catch (Exception ex)
            {
                jobrunstauts = "Error" + ex.Message;
            }
            return jobrunstauts;
        }
    }
}