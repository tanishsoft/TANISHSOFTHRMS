using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class ManageHelpDesk
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public string AddNewTask(TaskSaveModel model, string currentUser, string serverpath, HttpPostedFileBase[] Upload, HttpPostedFileBase[] DailyCashcollction, HttpPostedFileBase[] loginuserwisecollections, HttpPostedFileBase[] NASReport, HttpPostedFileBase[] OthersDocument)
        {
            tbl_Task task = new tbl_Task
            {
                AssertEquipId = model.AssertEquipId,
                TaskId = model.TaskId,
                CreatorLocationId = model.CreatorLocationId,
                CreatorLocationName = model.CreatorLocationName,
                CreatorDepartmentId = model.CreatorDepartmentId,
                CreatorDepartmentName = model.CreatorDepartmentName,
                CreatorId = model.CreatorId,
                CreatorName = model.CreatorName,
                CreatorPlace = model.CreatorPlace,
                AssertEquipName = model.AssertEquipName,
                CategoryOfComplaint = model.CategoryOfComplaint,
                Description = model.Description,
                AssignLocationId = model.AssignLocationId,
                AssignLocationName = model.AssignLocationName,
                AssignDepartmentId = model.AssignDepartmentId,
                AssignDepartmentName = model.AssignDepartmentName,
                AssignId = model.AssignId,
                AssignName = model.AssignName,
                WorkDoneRemarks = model.WorkDoneRemarks,
                AssignStatus = model.AssignStatus,
                CreatorStatus = model.CreatorStatus,
                LatestComment = model.LatestComment,
                TaskDoneByUserId = model.TaskDoneByUserId,
                TaskDoneByName = model.TaskDoneByName,
                IsActive = model.IsActive,
                ExtensionNo = model.ExtensionNo,
                EmailId = model.EmailId,
                Others = model.Others,
                Subject = model.Subject,
                IsVendorTicket = model.IsVendorTicket,
                CapexPrepareDate = model.CapexPrepareDate != null && model.CapexPrepareDate != "" ? ProjectConvert.ConverDateStringtoDatetime(model.CapexPrepareDate) : DateTime.Now,
                CapexApproveDate = model.CapexApproveDate != null && model.CapexApproveDate != "" ? ProjectConvert.ConverDateStringtoDatetime(model.CapexApproveDate) : DateTime.Now,
                TaskType = model.TaskType,
                Priority = model.Priority,
                DocumentReceived = model.DocumentReceived
            };
            task.CallDateTime = DateTime.Now;
            task.CallStartDateTime = DateTime.Now;
            task.CallEndDateTime = DateTime.Now;
            task.CreatedBy = currentUser;
            task.ModifiedBy = currentUser;
            task.CreatedOn = DateTime.Now;
            task.ModifiedOn = DateTime.Now;
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel
            {
                fromemail = "it_helpdesk@fernandez.foundation"
            };
            string mobilenumber = "";
            switch (task.AssignLocationId)
            {
                case 1:
                case 13:
                    switch (task.AssignDepartmentName.ToLower())
                    {
                        case "brand & communication":
                            mailmodel.toemail = "divya.m@fernandez.foundation";
                            mailmodel.ccemail = "ashish.bala@fernandez.foundation,academics@fernandez.foundation,divya.m@fernandez.foundation,ashrafali_s@fernandez.foundation";
                            mobilenumber = "8374001919";
                            if (task.CategoryOfComplaint == "Patient feedback video" || task.CategoryOfComplaint == "Photo & Video requirement")
                            {
                                mailmodel.toemail = "av@fernandez.foundation";
                                mailmodel.ccemail = "srinivas@fernandez.foundation,ashish.bala@fernandez.foundation, divya.m@fernandez.foundation,shivashankar.t@fernandez.foundation,ashrafali_s@fernandez.foundation";
                            }
                            break;
                        case "academics":
                            mailmodel.toemail = "academics@fernandez.foundation";
                            mailmodel.ccemail = "divya.m@fernandez.foundation";
                            mobilenumber = "8008552503,8374001919";
                            break;
                        case "information technology":
                        case "it":
                            mailmodel.toemail = "it_helpdesk@fernandez.foundation";
                            mobilenumber = "8008902012";
                            break;
                        case "biomedical":
                            mobilenumber = "8008902027";
                            break;
                        case "purchase":
                            mailmodel.toemail = "sitaramaiah_t@fernandez.foundation";
                            mailmodel.ccemail = "purchase@fernandez.foundation,veerababu_k@fernandez.foundation,capex@fernandez.foundation";
                            mobilenumber = "";
                            break;
                        case "maintenance":

                            switch (task.CategoryOfComplaint)
                            {
                                case "Electrician":
                                    mobilenumber = "8008902003";
                                    break;
                                case "AC_operator":
                                    mobilenumber = "8008902001";
                                    break;
                                case "Plumber":
                                    mobilenumber = "8008902004";
                                    break;
                                case "Gas_Plant_Operator":
                                    mobilenumber = "8008902003";
                                    break;
                            }
                            break;
                    }
                    break;
                case 2:
                    switch (task.AssignDepartmentName.ToLower())
                    {
                        case "academics":
                            mailmodel.toemail = "academics@fernandez.foundation";
                            mailmodel.ccemail = "divya.m@fernandez.foundation";
                            mobilenumber = "8008552503,8374001919";
                            break;
                        case "information technology":
                        case "it":
                            mailmodel.toemail = "it_helpdesk@fernandez.foundation";
                            mobilenumber = "8008300040";
                            break;
                        case "purchase":
                            mailmodel.toemail = "sitaramaiah_t@fernandez.foundation";
                            mailmodel.ccemail = "purchase@fernandez.foundation,veerababu_k@fernandez.foundation,capex@fernandez.foundation";
                            mobilenumber = "";
                            break;
                        case "biomedical":
                            mobilenumber = "8008902034";
                            break;

                        case "maintenance":
                            mailmodel.toemail = "ayyavaru_m@fernandez.foundation";
                            switch (task.CategoryOfComplaint)
                            {
                                case "Electrician":
                                    mobilenumber = "8179004475";
                                    break;
                                case "AC_operator":
                                    mobilenumber = "8179004476";
                                    break;
                                case "Plumber":
                                    mobilenumber = "8790165435";
                                    break;
                                case "Gas_Plant_Operator":
                                    mobilenumber = "8008504731";
                                    break;
                                    //case "Carpenter":
                                    //    mobilenumber = "8008902004";
                                    //    break;
                            }

                            break;

                    }
                    break;
                case 3:
                    switch (task.AssignDepartmentName.ToLower())
                    {
                        case "academics":
                            mailmodel.toemail = "academics@fernandez.foundation";
                            mailmodel.ccemail = "divya.m@fernandez.foundation";
                            mobilenumber = "8008552503,8374001919";
                            break;
                        case "information technology":
                        case "it":
                            mailmodel.toemail = "it_helpdesk@fernandez.foundation";
                            mobilenumber = "7337353760";
                            break;
                        case "biomedical":
                            mobilenumber = "8008902034";
                            break;
                        case "purchase":
                            mailmodel.toemail = "sitaramaiah_t@fernandez.foundation";
                            mailmodel.ccemail = "purchase@fernandez.foundation,veerababu_k@fernandez.foundation,capex@fernandez.foundation";
                            mobilenumber = "";
                            break;
                        case "maintenance":
                            switch (task.CategoryOfComplaint)
                            {
                                case "Electrician":
                                    mobilenumber = "8179004475";
                                    break;
                                case "AC_operator":
                                    mobilenumber = "8179004476";
                                    break;
                                case "Plumber":
                                    mobilenumber = "8790165435";
                                    break;
                                case "Gas_Plant_Operator":
                                    mobilenumber = "8008504731";
                                    break;
                            }

                            break;
                    }
                    break;
                case 4:
                    switch (task.AssignDepartmentName.ToLower())
                    {
                        case "academics":
                            mailmodel.toemail = "academics@fernandez.foundation";
                            mailmodel.ccemail = "divya.m@fernandez.foundation";
                            mobilenumber = "8008552503,8374001919";
                            break;
                        case "information technology":
                        case "it":
                            mailmodel.toemail = "it_helpdesk@fernandez.foundation";
                            mobilenumber = "8008300040";
                            break;
                        case "biomedical":
                            mobilenumber = "8008300046";
                            break;
                        case "purchase":
                            mailmodel.toemail = "sitaramaiah_t@fernandez.foundation";
                            mailmodel.ccemail = "purchase@fernandez.foundation,veerababu_k@fernandez.foundation,capex@fernandez.foundation";
                            mobilenumber = "";
                            break;
                        case "maintenance":
                            switch (task.CategoryOfComplaint)
                            {
                                case "Electrician":
                                    mobilenumber = "8179003962";
                                    break;
                                case "AC_operator":
                                    mobilenumber = "8179004476";
                                    break;
                                case "Plumber":
                                    mobilenumber = "8790165435";
                                    break;
                            }

                            break;


                    }
                    break;
                case 5:
                    switch (task.AssignDepartmentName.ToLower())
                    {
                        case "brand identity & communication":
                            mailmodel.toemail = "divya.m@fernandez.foundation";
                            mailmodel.ccemail = "ashish.bala@fernandez.foundation,academics@fernandez.foundation,divya.m@fernandez.foundation,ashrafali_s@fernandez.foundation";
                            mobilenumber = "8374001919";
                            if (task.CategoryOfComplaint == "Patient feedback video" || task.CategoryOfComplaint == "Photo & Video requirement")
                            {
                                mailmodel.toemail = "av@fernandez.foundation";
                                mailmodel.ccemail = "srinivas@fernandez.foundation,ashish.bala@fernandez.foundation, divya.m@fernandez.foundation,shivashankar.t@fernandez.foundation,ashrafali_s@fernandez.foundation";
                            }
                            break;
                        case "academics":
                            mailmodel.toemail = "academics@fernandez.foundation";
                            mailmodel.ccemail = "divya.m@fernandez.foundation";
                            mobilenumber = "8008552503,8374001919";
                            break;
                        case "information technology":
                        case "it":
                            mailmodel.toemail = "it_helpdesk@fernandez.foundation";
                            mobilenumber = "7337353760";
                            break;
                        case "biomedical":
                            mobilenumber = "7337320892";
                            break;
                        case "purchase":
                            mailmodel.toemail = "sitaramaiah_t@fernandez.foundation";
                            mailmodel.ccemail = "purchase@fernandez.foundation,veerababu_k@fernandez.foundation,capex@fernandez.foundation";
                            mobilenumber = "";
                            break;
                        case "maintenance":
                            mailmodel.toemail = "anthony_p@fernandez.foundation";
                            mailmodel.ccemail = "assis@fernandez.foundation";
                            mobilenumber = "8008500591";
                            switch (task.CategoryOfComplaint)
                            {
                                case "Electrician":
                                    mobilenumber = "7337353776";
                                    break;
                                case "AC_operator":
                                    mobilenumber = "7337353781";
                                    break;
                                case "Plumber":
                                    mobilenumber = "7337353778";
                                    break;
                                case "Gas_Plant_Operator":
                                    mobilenumber = "7337353779";
                                    break;
                            }

                            break;
                    }

                    break;
                case 9:
                    switch (task.AssignDepartmentName.ToLower())
                    {
                        case "purchase":
                            mailmodel.toemail = "sitaramaiah_t@fernandez.foundation";
                            mailmodel.ccemail = "purchase@fernandez.foundation,veerababu_k@fernandez.foundation,capex@fernandez.foundation";
                            mobilenumber = "";
                            break;
                        case "maintenance":
                            mailmodel.toemail = "anthony_p@fernandez.foundation";
                            mailmodel.ccemail = "assis@fernandez.foundation";

                            switch (task.CategoryOfComplaint)
                            {
                                case "Electrician":
                                    mobilenumber = "9704995665";
                                    break;
                                case "AC_operator":
                                    mobilenumber = "9704995665";
                                    break;
                                case "Plumber":
                                    mobilenumber = "9704995665";
                                    break;

                            }

                            break;
                    }
                    break;
                default:
                    break;

            }
            task.ModifiedOn = DateTime.Now;
            myapp.tbl_Task.Add(task);
            myapp.SaveChanges();
            List<HttpPostedFileBase> fileslist = new List<HttpPostedFileBase>();

            if (Upload != null)
            {
                fileslist.Add(Upload[0]);
            }

            if (DailyCashcollction != null)
            {
                fileslist.Add(DailyCashcollction[0]);
            }

            if (loginuserwisecollections != null)
            {
                fileslist.Add(loginuserwisecollections[0]);
            }

            if (NASReport != null)
            {
                fileslist.Add(NASReport[0]);
            }
            if (OthersDocument != null)
            {
                fileslist.Add(OthersDocument[0]);
            }
            foreach (HttpPostedFileBase file in fileslist)
            {
                if (file != null)
                {
                    //HttpPostedFileBase file = Others[0];
                    string fname = file.FileName;

                    string fileName = Path.GetFileName(file.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    string path = Path.Combine(serverpath, guidid + fileName);
                    file.SaveAs(path);
                    tbl_TaskDoument tsk = new tbl_TaskDoument
                    {
                        CreatedBy = currentUser,
                        CreatedOn = DateTime.Now,
                        DocumentName = fileName,
                        DocumentPath = guidid + fileName,
                        IsPrivate = true,
                        TaskId = task.TaskId
                    };
                    myapp.tbl_TaskDoument.Add(tsk);
                    myapp.SaveChanges();
                }
            }
            if (task.Subject != null && task.Subject != "")
            {
                //  mailmodel.subject = "Ticket " + task.TaskId + " " + task.Subject.Substring(0, 50) + " - " + task.CreatorLocationName + " " + task.CreatorDepartmentName;
                mailmodel.subject = "Ticket " + task.TaskId + " - " + task.CreatorLocationName + " " + task.CreatorDepartmentName + " " + task.CreatorName + " " + task.CategoryOfComplaint + " " + task.ExtensionNo;
            }

            if (mailmodel.toemail != null && mailmodel.toemail != "")
            {
                string mailbody = "";
                mailbody += "<p style='font-family:verdana;font-size:14px;'>Ticket Details are: </p><table style='border:solid 1px #eee;'>";
                mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>Task Id</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>" + task.TaskId + "</td></tr>";
                mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>Creator</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>" + task.CreatorLocationName + " " + task.CreatorDepartmentName + " " + task.CreatorName + "</td></tr>";
                mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>Assign Location</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>" + task.AssignLocationName + "</td></tr>";
                mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>Assign Department</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>" + task.AssignDepartmentName + "</td></tr>";
                mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>Category Of Complaint</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>" + task.CategoryOfComplaint + "</td></tr>";
                //if (task.CapexApproveDate != null)
                //{
                //    mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>Capex Approve Date</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>" + task.CapexApproveDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                //}
                //if (task.CapexPrepareDate != null)
                //{
                //    mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>Capex Prepare Date</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>" + task.CapexPrepareDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                //}
                mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>Subject</td><td  style='`border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>" + task.Subject + "</td></tr>";
                mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>Description</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>" + task.Description + "</td></tr>";
                mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>Extension No</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>" + task.ExtensionNo + "</td></tr>";
                mailbody += "<tr><td style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>Email Id</td><td  style='border-bottom: 1px solid #ededed; border-right: 1px solid #ededed;color:#171f23de;'>" + task.EmailId + "</td></tr>";
                mailbody += "</table>";

                string body = "";
                using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("~/EmailTemplates/Biomedical_Asset.html")))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("[[Heading]]", "A New Ticket From " + task.CreatorLocationName + "");
                body = body.Replace("[[table]]", mailbody);
                mailmodel.body = body;
                mailmodel.filepath = "";
                mailmodel.fromname = "New Ticket Created";
                if (mailmodel.ccemail == null || mailmodel.ccemail == "")
                {
                    mailmodel.ccemail = "";
                }

                //cm.SendEmail(mailmodel);
                Thread email = new Thread(delegate ()
                 {
                     cm.SendEmail(mailmodel);
                 });

                email.IsBackground = true;
                email.Start();
            }
            if (mobilenumber != null && mobilenumber != "")
            {
                SendSms sms = new SendSms();
                string messagetosendsms = "Dear Team, You have a new ticket ID NO " + task.TaskId + " is created by " + task.CreatorName + " " + task.CreatorDepartmentName + " under the category " + task.CategoryOfComplaint + ". Please login to Infonet to view the details. Tanishsoft Hrms";
                sms.SendSmsToEmployee(mobilenumber, messagetosendsms);
            }
            //Automate settings
            if (task.Subject != null && task.Subject != "")
            {
                List<tbl_TaskAutoAssignSetting> listofauto = myapp.tbl_TaskAutoAssignSetting.Where(l => l.CreatorLocationId == task.CreatorLocationId && l.CreatorDepartmentId == task.CreatorDepartmentId && l.LocationId == task.AssignLocationId && l.Departmentid == task.AssignDepartmentId).ToList();
                if (listofauto.Count > 0)
                {
                    foreach (tbl_TaskAutoAssignSetting la in listofauto)
                    {
                        if (task.Subject.ToLower().Contains(la.Subject.ToLower()))
                        {
                            List<tbl_Task> tasks = myapp.tbl_Task.Where(l => l.TaskId == task.TaskId).ToList();
                            List<tbl_User> user = myapp.tbl_User.Where(l => l.CustomUserId == la.AssignToUserId).ToList();
                            if (tasks.Count > 0 && user.Count > 0)
                            {
                                tasks[0].AssignId = user[0].UserId;
                                tasks[0].AssignName = user[0].FirstName;
                                tasks[0].CallStartDateTime = DateTime.Now.AddMinutes(2);
                                tasks[0].ModifiedOn = DateTime.Now.AddMinutes(2);
                                tasks[0].AssignStatus = "In Progress";
                                tasks[0].CreatorStatus = "In Progress";
                                tasks[0].Others = "";
                                tbl_TaskComment tsc = new tbl_TaskComment
                                {
                                    Comment = "Task Pick Up By - " + user[0].FirstName,
                                    TaskId = task.TaskId,
                                    CommentedBy = currentUser,
                                    CommentDate = DateTime.Now.AddMinutes(2)
                                };
                                myapp.tbl_TaskComment.Add(tsc);
                                myapp.SaveChanges();
                                if (la.SendEmail.Value)
                                {
                                    cm = new CustomModel();
                                    mailmodel = new MailModel();
                                    EmailTeamplates emailtemp = new EmailTeamplates();
                                    mailmodel.fromemail = "helpdesk@hospitals.com";
                                    mailmodel.toemail = user[0].EmailId;
                                    mailmodel.subject = "A Ticket " + tasks[0].TaskId + " Assigned to you ";
                                    string mailbody = "<p style='font-family:verdana'>Dear " + user[0].FirstName + ",";
                                    mailbody += "<p style='font-family:verdana'>New Task has assigned a task to you. Please click on this <a target='_blank' href='https://infonet.fernandezhospital.com/'>link</a>  to see the task.Do not forget to update the task status after completion.</p>";
                                    mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                                    mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Call Date</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + tasks[0].CallDateTime + "</td></tr>";
                                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Creator Department</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + tasks[0].CreatorDepartmentName + "</td></tr>";
                                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Creator Name</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + tasks[0].CreatorName + "</td></tr>";
                                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Category Of Complaint</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + tasks[0].CategoryOfComplaint + "</td></tr>";
                                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Subject</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + tasks[0].Subject + "</td></tr>";
                                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Description</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + tasks[0].Description + "</td></tr>";
                                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Extension No</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + tasks[0].AssertEquipId + "</td></tr>";
                                    mailbody += "</table>";
                                    mailbody += "<br/><p style='font-family:cambria'>This is an auto generated mail,Don't reply to this.</p>";
                                    mailmodel.body = mailbody;
                                    mailmodel.filepath = "";
                                    mailmodel.fromname = "Help Desk";
                                    mailmodel.ccemail = "";
                                    cm.SendEmail(mailmodel);
                                }
                                if (la.SendSMS.Value)
                                {
                                    mailmodel.subject = "Ticket " + task.TaskId + " - " + task.Subject + " by " + task.CreatorLocationName + " " + task.CreatorDepartmentName + " " + task.CreatorName + " " + task.CategoryOfComplaint;
                                    if (user[0].PhoneNumber != null && user[0].PhoneNumber != "")
                                    {
                                        SendSms sms = new SendSms();
                                        sms.SendSmsToEmployee(user[0].PhoneNumber, mailmodel.subject);
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return "You have succesfully created a task.";
        }
    }
}