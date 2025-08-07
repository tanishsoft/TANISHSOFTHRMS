using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    [RoutePrefix("api/HelpDesk")]
    public class HelpDeskController : ApiController
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        [HttpGet]
        [Route("GetTaskDescription")]
        public string ViewDescription(int id)
        {
            tbl_Task model = myapp.tbl_Task.Where(t => t.TaskId == id).SingleOrDefault();
            return model.Description;
        }

        [HttpGet]
        [Route("GetTaskWorkDoneRemarks")]
        public string WorkDonestatus(int id)
        {
            tbl_Task model = myapp.tbl_Task.Where(t => t.TaskId == id).SingleOrDefault();
            return model.WorkDoneRemarks;
        }

        [HttpGet]
        [Route("UpdateTaskWorkDoneRemarks")]
        public string WorkDoneRemarksComment(int id, string Remarks)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            if (tasks.Count > 0)
            {
                tasks[0].WorkDoneRemarks = Remarks;
                tasks[0].ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
            }
            return "Success";
        }
        [HttpGet]
        [Route("GetCloseTask")]
        public string EndTask(int id)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            tbl_User list = (from v in myapp.tbl_User where v.CustomUserId == User.Identity.Name select v).SingleOrDefault();
            if (tasks.Count > 0)
            {
                if (tasks[0].IsVendorTicket == null || tasks[0].IsVendorTicket == false)
                {
                    tasks[0].CallEndDateTime = DateTime.Now;
                }

                tasks[0].AssignStatus = "Done";
                tasks[0].CreatorStatus = "Pending from user";
                if (list.CustomUserId != null)
                {
                    tasks[0].TaskDoneByUserId = User.Identity.Name;
                    tasks[0].TaskDoneByName = list.FirstName;
                    tasks[0].ModifiedOn = DateTime.Now;
                }
                myapp.SaveChanges();
                tbl_TaskComment tsc = new tbl_TaskComment
                {
                    Comment = "Task Done by " + tasks[0].AssignName,
                    TaskId = id,
                    CommentedBy = User.Identity.Name,
                    CommentDate = DateTime.Now
                };
                myapp.tbl_TaskComment.Add(tsc);
                myapp.SaveChanges();
            }
            return "Success";
        }
        [HttpGet]
        [Route("SaveWorkDoneComments")]
        public string WorkDoneRemarks(int id, string remarks)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            if (tasks.Count > 0)
            {
                string currentuserid = User.Identity.Name;
                tbl_User list = (from v in myapp.tbl_User where v.CustomUserId == currentuserid select v).SingleOrDefault();
                if (list != null)
                {
                    if (list.UserId == tasks[0].AssignId)
                    {
                        tasks[0].WorkDoneRemarks = remarks;
                        if (tasks[0].IsVendorTicket == null || tasks[0].IsVendorTicket == false)
                        {
                            tasks[0].CallEndDateTime = DateTime.Now;
                        }

                        tasks[0].AssignStatus = "Done";
                        tasks[0].CreatorStatus = "Pending from user";
                        tasks[0].ModifiedOn = DateTime.Now;
                        myapp.SaveChanges();
                        tbl_TaskComment tsc = new tbl_TaskComment
                        {
                            Comment = "Task Done by " + tasks[0].AssignName,
                            TaskId = id,
                            CommentedBy = User.Identity.Name,
                            CommentDate = DateTime.Now
                        };
                        myapp.tbl_TaskComment.Add(tsc);
                        myapp.SaveChanges();
                    }
                    else
                    {

                        return "The Request already assign to Some other person";
                    }
                }
            }
            return "Success";
        }
        [HttpGet]
        [Route("AssigntoVendor")]
        public string AssigntoVendor(int id, string remarks)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            if (tasks.Count > 0)
            {
                tasks[0].WorkDoneRemarks = remarks;
                tasks[0].CallEndDateTime = DateTime.Now;
                tasks[0].AssignStatus = "Pending from Vendor";
                tasks[0].CreatorStatus = "In Progress";
                tasks[0].ModifiedOn = DateTime.Now;
                tasks[0].IsVendorTicket = true;
                myapp.SaveChanges();
                tbl_TaskComment tsc = new tbl_TaskComment
                {
                    Comment = "Pending from Vendor " + remarks,
                    TaskId = id,
                    CommentedBy = User.Identity.Name,
                    CommentDate = DateTime.Now
                };
                myapp.tbl_TaskComment.Add(tsc);
                myapp.SaveChanges();
            }
            return "Success";
        }
        [HttpGet]
        [Route("GetTaskComments")]
        public List<tbl_TaskComment> GetMyTaskComments(int taskid)
        {
            List<tbl_TaskComment> list = myapp.tbl_TaskComment.Where(t => t.TaskId == taskid).OrderByDescending(t => t.TaskCommentId).ToList();
            return list;
        }
        [HttpGet]
        [Route("CloseTask")]
        public string DoneTask(int id)
        {
            string message = "Success";
            List<tbl_Task> list = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            string Designation = "";


            List<tbl_Task> listoftask = (from v in myapp.tbl_Task where v.IsActive == true && v.CallDateTime != null select v).ToList();
            if (list.Count > 0)
            {
                if (list[0].CreatedBy == User.Identity.Name)
                {

                    list[0].CreatorStatus = "Done";
                    list[0].ModifiedOn = DateTime.Now;
                    tbl_TaskComment tsc = new tbl_TaskComment
                    {
                        Comment = "Task Done by - " + list[0].AssignName,
                        TaskId = id,
                        CommentedBy = User.Identity.Name,
                        CommentDate = DateTime.Now
                    };
                    myapp.tbl_TaskComment.Add(tsc);
                    myapp.SaveChanges();
                }

                else if (listoftask.Count > 0)
                {

                    if (Designation != null && Designation == "Manager" && Designation != "")
                    {
                        tbl_Task singlelist = myapp.tbl_Task.Where(t => t.TaskId == id).SingleOrDefault();
                        DateTime calldatetime = singlelist.CallDateTime.Value;
                        double days = (DateTime.Now - calldatetime).TotalDays;
                        if (days >= 7)
                        {
                            list[0].CreatorStatus = "Done";
                            list[0].AssignStatus = "Done";
                            list[0].ModifiedOn = DateTime.Now;
                            tbl_TaskComment tsc = new tbl_TaskComment
                            {
                                Comment = "Task Done by - " + list[0].AssignName,
                                TaskId = id,
                                CommentedBy = User.Identity.Name,
                                CommentDate = DateTime.Now
                            };
                            myapp.tbl_TaskComment.Add(tsc);
                            myapp.SaveChanges();
                        }
                        else
                        {
                            return "Please as creator to close the task";
                        }
                    }
                    else { return "You don't have rights to do this job"; }


                }
                else
                {
                    return "Please as creator to close the task";
                }
                // myapp.SaveChanges();
            }


            return message;
        }
        [HttpGet]
        [Route("SaveTaskComment")]
        public string SaveMyTaskComment(int taskid, string comment)
        {
            tbl_TaskComment tsc = new tbl_TaskComment
            {
                Comment = comment,
                TaskId = taskid,
                CommentedBy = User.Identity.Name,
                CommentDate = DateTime.Now
            };
            myapp.tbl_TaskComment.Add(tsc);
            myapp.SaveChanges();
            return "Success";
        }
        [HttpGet]
        [Route("PickupTask")]
        public string StartTask(int id)
        {
            string msg = "Success";
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            if (User.IsInRole("OutSource"))
            {
                List<tbl_OutSourceUser> user = myapp.tbl_OutSourceUser.Where(u => u.CustomUserId == User.Identity.Name).ToList();
                if (tasks.Count > 0 && user.Count > 0)
                {
                    tasks[0].AssignLocationId = user[0].LocationId;
                    tasks[0].AssignLocationName = user[0].LocationName;
                    tasks[0].AssignDepartmentId = user[0].DepartmentId;
                    tasks[0].AssignDepartmentName = user[0].DepartmentName;
                    if (tasks[0].AssignId == null || tasks[0].AssignId == 0 || tasks[0].Others == "NewAssign")
                    {
                        tasks[0].AssignId = user[0].UserId;
                        tasks[0].AssignName = user[0].FirstName;
                        tasks[0].CallStartDateTime = DateTime.Now;
                        tasks[0].ModifiedOn = DateTime.Now;
                        tasks[0].AssignStatus = "In Progress";
                        tasks[0].CreatorStatus = "In Progress";
                        tasks[0].Others = "";
                        //myapp.SaveChanges();

                        tbl_TaskComment tsc = new tbl_TaskComment
                        {
                            Comment = "Task Pick Up By - " + user[0].FirstName,
                            TaskId = id,
                            CommentedBy = User.Identity.Name,
                            CommentDate = DateTime.Now
                        };
                        myapp.tbl_TaskComment.Add(tsc);
                        myapp.SaveChanges();
                    }

                    else
                    {
                        msg = "The Ticked already picked by some one";
                    }
                }
            }
            else
            {
                List<tbl_User> user = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).ToList();
                if (tasks.Count > 0 && user.Count > 0)
                {
                    tasks[0].AssignLocationId = user[0].LocationId;
                    tasks[0].AssignLocationName = user[0].LocationName;
                    tasks[0].AssignDepartmentId = user[0].DepartmentId;
                    tasks[0].AssignDepartmentName = user[0].DepartmentName;
                    if (tasks[0].AssignStatus != "Done" && (tasks[0].AssignId == null || tasks[0].AssignId == 0 || tasks[0].Others == "NewAssign"))
                    {
                        tasks[0].AssignId = user[0].UserId;
                        tasks[0].AssignName = user[0].FirstName;
                        tasks[0].CallStartDateTime = DateTime.Now;
                        tasks[0].ModifiedOn = DateTime.Now;
                        tasks[0].AssignStatus = "In Progress";
                        tasks[0].CreatorStatus = "In Progress";
                        tasks[0].Others = "";
                        //myapp.SaveChanges();

                        tbl_TaskComment tsc = new tbl_TaskComment
                        {
                            Comment = "Task Pick Up By - " + user[0].FirstName,
                            TaskId = id,
                            CommentedBy = User.Identity.Name,
                            CommentDate = DateTime.Now
                        };
                        myapp.tbl_TaskComment.Add(tsc);
                        myapp.SaveChanges();
                    }

                    else
                    {
                        msg = "The Ticked already picked by some one";
                    }
                }
            }
            return msg;
        }
        [HttpGet]
        [Route("ReOpenTask")]
        public string ReOpenTask(int id)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            // var list = (from v in myapp.tbl_Task where v.AssignStatus != null && v.AssignStatus == "In Progress" select v).SingleOrDefault();

            if (tasks.Count > 0)
            {
                // tasks[0].CallStartDateTime = DateTime.Now;
                tasks[0].AssignStatus = "Re Open";
                tasks[0].CreatorStatus = "Re Open";
                tasks[0].ModifiedOn = DateTime.Now;
                myapp.SaveChanges();

                tbl_TaskComment tsc = new tbl_TaskComment
                {
                    Comment = "Task Re Open - " + tasks[0].AssignName,
                    TaskId = id,
                    CommentedBy = User.Identity.Name,
                    CommentDate = DateTime.Now
                };
                myapp.tbl_TaskComment.Add(tsc);
                myapp.SaveChanges();
            }
            return "Success";
        }
        [HttpGet]
        [Route("AssignTaskToDepartment")]
        public string Assigntasktoemployee(int id, int locid, string locname, int deptid, string deptname)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            if (tasks.Count > 0)
            {
                tasks[0].AssignLocationId = locid;
                tasks[0].AssignLocationName = locname;
                tasks[0].AssignDepartmentId = deptid;
                tasks[0].AssignDepartmentName = deptname;
                tasks[0].Others = "NewAssign";
                tasks[0].ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
            }
            return "The task is successfully assign to <b style='color:blue;font-family:italic;'>" + deptname + "</b>";
        }
        [HttpGet]
        [Route("AssignTaskToEmployee")]
        public string Assigntasktoemployeeesbydepartment(int id, int empid)
        {
            List<tbl_Task> tasks = myapp.tbl_Task.Where(t => t.TaskId == id).ToList();
            string customuserid = User.Identity.Name;
            tbl_User assign = (from v in myapp.tbl_User where v.UserId == empid select v).SingleOrDefault();
            if (tasks.Count > 0)
            {
                tbl_User dept = myapp.tbl_User.Where(u => u.CustomUserId == customuserid).SingleOrDefault();
                if (assign.FirstName != null && assign.FirstName != "")
                {
                    //tasks[0].CallStartDateTime = DateTime.Now;
                    tasks[0].AssignName = assign.FirstName;
                    tasks[0].AssignId = empid;
                    tasks[0].Others = "NewAssign";
                    tbl_TaskComment tsc = new tbl_TaskComment
                    {
                        Comment = "Task Assigned To - " + assign.FirstName + " by - " + (dept != null ? dept.FirstName : "User"),
                        TaskId = id,
                        CommentedBy = User.Identity.Name,
                        CommentDate = DateTime.Now
                    };
                    myapp.tbl_TaskComment.Add(tsc);
                    myapp.SaveChanges();
                }
                if (assign.EmailId != null)
                {
                    CustomModel cm = new CustomModel();
                    MailModel mailmodel = new MailModel();
                    EmailTeamplates emailtemp = new EmailTeamplates();
                    mailmodel.fromemail = "Leave@hospitals.com";
                    mailmodel.toemail = assign.EmailId;
                    mailmodel.subject = "A Ticket " + id + " Assigned to you ";
                    string mailbody = "<p style='font-family:verdana'>Dear " + assign.FirstName + ",";
                    mailbody += "<p style='font-family:verdana'>" + (dept != null ? dept.FirstName : "User") + " has assigned a task to you. Please click on this <a target='_blank' href='https://infonet.fernandezhospital.com/'>link</a>  to see the task.Do not forget to update the task status after completion.</p>";
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
                    //mailmodel.body = "A New Ticket Assigned to you";
                    //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                    mailmodel.filepath = "";
                    //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                    mailmodel.fromname = "Help Desk";
                    mailmodel.ccemail = "";
                    cm.SendEmail(mailmodel);
                }

            }
            return "The task is successfully assigned to &nbsp;<b style='color:green;font-family:italic;'>" + assign.FirstName + "</b>";
        }
        [HttpGet]
        [Route("GetTaskDocuments")]
        public List<tbl_TaskDoument> GetListOfFiles(int id)
        {
            List<tbl_TaskDoument> list = myapp.tbl_TaskDoument.Where(l => l.TaskId == id).ToList();
            return list;
        }
        [HttpGet]
        [Route("GetMyTasks")]
        public List<TaskViewModel> GetMyTasks(int page = 1, int pageSize = 15)
        {
            var skipAmount = pageSize * (page - 1);
            var tasks = myapp.tbl_Task.Where(t => t.IsActive == true).AsQueryable();
            tbl_User dept = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).Single();
            if (User.IsInRole("LocationManager"))
            {
                tasks = tasks.Where(t => t.CreatorLocationId == dept.LocationId
                || t.CreatorLocationId == t.AssignLocationId
                || t.AssignDepartmentId == dept.LocationId
                || t.AssignDepartmentName == dept.DepartmentName.ToString()).AsQueryable();
            }
            else if (User.IsInRole("DepartmentManager") || dept.DepartmentName == "Information Technology" || dept.DepartmentName == "Biomedical" || dept.DepartmentName == "Maintenance" || dept.DepartmentName == "Academics" || dept.DepartmentName == "Purchase" || dept.DepartmentName == "Finance & Accounts")
            {
                switch (dept.DepartmentName)
                {
                    case "Information Technology":
                        tasks = tasks.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Information Technology").AsQueryable();
                        break;
                    case "Biomedical":
                        tasks = tasks.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Biomedical").AsQueryable();
                        break;
                    case "Maintenance":
                        tasks = tasks.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Maintenance").AsQueryable();
                        break;
                    case "Academics":
                        tasks = tasks.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Academics").AsQueryable();
                        break;
                    case "Purchase":
                        tasks = tasks.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Purchase").AsQueryable();
                        break;
                    case "Finance & Accounts":
                        tasks = tasks.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentName == "Finance & Accounts").AsQueryable();
                        break;
                    default:
                        tasks = tasks.Where(t => t.CreatorId == dept.UserId || t.AssignDepartmentId == dept.DepartmentId).AsQueryable();
                        break;

                }

            }
            //filter by others
            else
            {
                tasks = tasks.Where(t => t.CreatorId == dept.UserId || t.AssignId == dept.UserId).AsQueryable();
            }

            tasks = tasks.OrderByDescending(t => t.ModifiedOn).AsQueryable();
            var list = tasks.Skip(skipAmount).Take(pageSize).ToList();
            List<TaskViewModel> result = (from c in list
                                              // let restime = GetResposetime(c.CreateDate, c.Starttime)
                                              //let date = dat
                                          select new TaskViewModel
                                          {
                                              TaskId = c.TaskId,

                                              CallDateTime = c.CallDateTime.Value.ToString("dd/MM/yyyy HH:mm"),
                                              ResponseTime = Responsetime(c.CallDateTime.Value, c.CallStartDateTime.Value),//ResponseTime                                
                                              CreatorName = c.CreatorName,
                                              CreatorLocationName = c.CreatorLocationName,
                                              ExtensionNo = c.ExtensionNo /*+" "+c.AssertEquipName*/,
                                              CategoryOfComplaint = c.CategoryOfComplaint,
                                              //c.AssertEquipId,
                                              //c.AssertEquipName,
                                              Subject = c.Subject,
                                              /*c.AssignDepartmentName+" "+*/
                                              AssignName = c.AssignName,
                                              //c.WorkDoneRemarks,
                                              Age = CalculateAge(c.CallStartDateTime.Value, c.CallEndDateTime.Value),//Total Time Taken                               
                                          }).ToList();
            result = result
            //.OrderByPropertyOrField(orderBy, ascending)
            .Skip(skipAmount)
            .Take(pageSize).ToList();
            return result;
        }
        [HttpGet]
        [Route("GetTask")]
        public tbl_Task GetTask(int id)
        {
            tbl_Task model = myapp.tbl_Task.Where(t => t.TaskId == id).Single();
            return model;
        }
        [HttpGet]
        [Route("UpdateDescription")]
        public string UpdateDescription(int id, string Description, string Category)
        {

            tbl_Task model = myapp.tbl_Task.Where(t => t.TaskId == id).Single();
            List<tbl_User> listuser = myapp.tbl_User.Where(t => t.CustomUserId == User.Identity.Name).ToList();
            if (model.CreatedBy == User.Identity.Name || model.AssignDepartmentName == listuser[0].DepartmentName)
            {
                //model.CategoryOfComplaint = Category;
                model.Description = Description;
                model.ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
                return "Success";
            }
            else
            {
                return "you are not authorized to edit";
            }
        }
        [HttpPost]
        [Route("UploadFile")]
        public System.Threading.Tasks.Task<HttpResponseMessage> UploadFileAsync()
        {
            List<string> savefilepath = new List<string>();
            string root = HttpContext.Current.Server.MapPath("~/ExcelUplodes");
            var provider = new MultipartFormDataStreamProvider(root);
            string newfilename = "";
            string name = "";
            var task = Request.Content.ReadAsMultipartAsync(provider).
                ContinueWith<HttpResponseMessage>(t =>
                {
                    if (t.IsCanceled || t.IsFaulted)
                    {
                        Request.CreateErrorResponse(HttpStatusCode.InternalServerError, t.Exception);
                    }
                    foreach (MultipartFileData item in provider.FileData)
                    {
                        try
                        {
                            name = item.Headers.ContentDisposition.FileName.Replace("\"", "");
                            newfilename = Guid.NewGuid() + Path.GetExtension(name);
                            File.Copy(item.LocalFileName, Path.Combine(root, newfilename));

                        }
                        catch (Exception)
                        {
                            throw;
                        }
                    }
                    foreach (var key in provider.FormData.AllKeys)
                    {
                        foreach (var val in provider.FormData.GetValues(key))
                        {
                            if (key == "id")
                            {
                                tbl_TaskDoument tsk = new tbl_TaskDoument
                                {
                                    CreatedBy = User.Identity.Name,
                                    CreatedOn = DateTime.Now,
                                    DocumentName = name,
                                    DocumentPath = newfilename,
                                    TaskId = int.Parse(val)
                                };
                                myapp.tbl_TaskDoument.Add(tsk);
                                myapp.SaveChanges();
                            }
                        }
                    }
                    return Request.CreateResponse(HttpStatusCode.Created, savefilepath);
                });
            return task;
        }
        private string Responsetime(DateTime st, DateTime ed)
        {
            if (ed > st)
            {
                string startTime = "7:15 AM";
                string endTime = "8:45 PM";
                TimeSpan checktime1 = DateTime.Parse(startTime).TimeOfDay;
                TimeSpan checktime2 = DateTime.Parse(endTime).TimeOfDay;

                TimeSpan time = st.TimeOfDay;
                if (time >= checktime1 && time <= checktime2)
                {

                }
                else
                {
                    time = checktime1;
                }
                TimeSpan time2 = ed.TimeOfDay;

                TimeSpan t = (ed - st);

                string answer = "";

                answer = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
                if (st == ed)
                {
                    return "0";
                }

                return answer;
            }
            else
            {
                return "0";
            }
        }
        private string CalculateAge(DateTime st, DateTime ed)
        {
            if (ed > st)
            {
                TimeSpan t = (ed - st);
                //return span.TotalMinutes.ToString("0.00");
                string answer = "";
                if (t.TotalMinutes > 1)
                {
                    answer = string.Format("{0:D2}:{1:D2}:{2:D2}", t.Days, t.Hours, t.Minutes);
                }

                return answer;
            }
            else
            {
                return "";
            }
        }
        [HttpPost]
        [Route("AddNewTask")]
        public string AddNewTask(TaskSaveModel model)
        {
            string currentuser = User.Identity.Name;
            var userdetails = myapp.tbl_User.Where(l => l.CustomUserId == currentuser).SingleOrDefault();
            model.CreatorDepartmentId = userdetails.DepartmentId;
            model.CreatorDepartmentName = userdetails.DepartmentName;
            model.CreatorLocationId = userdetails.LocationId;
            model.CreatorLocationName = userdetails.LocationName;
            model.CreatorId = userdetails.UserId;
            model.CreatorStatus = "New";
            model.AssignStatus = "New";
            model.IsActive = true;
            model.CreatorName = userdetails.FirstName;
            ManageHelpDesk helpDesk = new ManageHelpDesk();
            string message = helpDesk.AddNewTask(model, model.CurrentUser, "", null, null, null, null, null);
            return message;
        }
    }
}
