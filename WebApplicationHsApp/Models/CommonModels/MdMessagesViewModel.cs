using System;
using System.Collections.Generic;
using System.Linq;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models.CommonModels
{
    public class MdMessagesViewModel
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public string SaveResponseOnMdMessageReplay(int ParentId, string Message, string CurrentUser)
        {
            var tb = myapp.tbl_SettingsResponse.Where(a => a.Id == ParentId).ToList();
            if (tb.Count > 0)
            {
                tbl_SettingsResponse model = new tbl_SettingsResponse();
                model.CreatedBy = CurrentUser;
                model.CreatedOn = DateTime.Now;
                model.IsActive = true;
                model.IsRead = true;
                model.Message = Message;
                model.ModifiedBy = CurrentUser;
                model.ModifiedOn = DateTime.Now;
                model.PhoneNo = "";
                model.ResponseParentId = ParentId;
                model.SettingsId = tb[0].SettingsId;
                model.SettingsKey = tb[0].SettingsKey;
                model.SettingsValue = tb[0].SettingsValue;
                myapp.tbl_SettingsResponse.Add(model);
                myapp.SaveChanges();
                string usercreatedby = tb[0].CreatedBy;
                //var user = myapp.tbl_User.Where(l => l.CustomUserId == usercreatedby).FirstOrDefault();
                //if (user != null && user.EmailId != null)
                //{
                //    CustomModel cm = new CustomModel();
                //    MailModel mailmodel = new MailModel();
                //    mailmodel.fromemail = "Leave@hospitals.com";
                //    mailmodel.toemail = user.EmailId;
                //    mailmodel.subject = "Replays on MD Message";
                //    string bodyhtml = " <p>Dear " + user.FirstName + " " + user.LastName + ",</p>";
                //    bodyhtml += "<p> MD Madam  commented on your Message, Please find the comments are below.</p>";
                //    bodyhtml += "<p>" + model.Message + "</p>";
                //    bodyhtml += "<p>&nbsp;</p>";
                //    bodyhtml += "<p>Thanks &amp; Regards</p>";
                //    bodyhtml += "<p>Notification Team</p>";
                //    bodyhtml += "<p>Note: This is system generated email please dont reply.</p>";
                //    mailmodel.body = bodyhtml;
                //    mailmodel.filepath = "";
                //    mailmodel.username = user.FirstName + " " + user.LastName;
                //    mailmodel.fromname = "FeedBack";
                //    mailmodel.ccemail = "evita@fernandez.foundation";
                //    cm.SendEmail(mailmodel);
                //}
            }

            return "Thank you! your message has been successfully sent.";
        }
        public string SaveResponseOnMdMessage(string Mobile, string Message, string CurrentUser)
        {
            var tb = myapp.tbl_Settings.Where(a => a.SettingKey == "MdMessage").ToList();
            if (tb.Count > 0)
            {
                tbl_SettingsResponse model = new tbl_SettingsResponse();
                model.CreatedBy = CurrentUser;
                model.CreatedOn = DateTime.Now;
                model.IsActive = true;
                model.IsRead = true;
                model.Message = Message;
                model.ModifiedBy = CurrentUser;
                model.ModifiedOn = DateTime.Now;
                model.PhoneNo = Mobile;
                model.SettingsId = tb[0].SettingsId;
                model.SettingsKey = tb[0].SettingKey;
                model.SettingsValue = tb[0].SettingValue.Length > 2000 ? tb[0].SettingValue.Substring(0, 2000) : tb[0].SettingValue;
                myapp.tbl_SettingsResponse.Add(model);
                myapp.SaveChanges();
                //var user = myapp.tbl_User.Where(l => l.CustomUserId == CurrentUser).FirstOrDefault();
                //if (user != null)
                //{
                //    CustomModel cm = new CustomModel();
                //    MailModel mailmodel = new MailModel();
                //    mailmodel.fromemail = "Leave@hospitals.com";
                //    mailmodel.toemail = "evita@fernandez.foundation";
                //    mailmodel.subject = "Replays on MD Message";
                //    string bodyhtml = " <p>Dear Madam,</p>";
                //    bodyhtml += "<p>" + user.FirstName + " " + user.LastName + " (" + user.DepartmentName + ") " + model.PhoneNo + "  commented on your Message, Please find the comments are below.</p>";
                //    bodyhtml += "<p>" + model.Message + "</p>";
                //    bodyhtml += "<p>&nbsp;</p>";
                //    bodyhtml += "<p>Thanks &amp; Regards</p>";
                //    bodyhtml += "<p>Notification Team</p>";
                //    bodyhtml += "<p>Note: This is system generated email please dont reply.</p>";
                //    mailmodel.body = bodyhtml;
                //    mailmodel.filepath = "";
                //    mailmodel.username = user.FirstName + " " + user.LastName;
                //    mailmodel.fromname = "FeedBack";
                //    if (user.EmailId != null && user.EmailId != "")
                //        mailmodel.ccemail = user.EmailId;
                //    cm.SendEmail(mailmodel);
                //}
            }

            return "Thank you! your message has been successfully sent.";
        }
        public List<SettingsResponseViewModel> GetSettingsResponses(string Key)
        {
            var results = myapp.tbl_SettingsResponse.Where(l => l.SettingsKey == Key).OrderByDescending(l => l.CreatedOn).ToList();
            List<SettingsResponseViewModel> model = new List<SettingsResponseViewModel>();
            var keysettings = results.Select(l => l.SettingsId.Value).Distinct().ToList();
            foreach (var v in keysettings)
            {
                SettingsResponseViewModel m = new SettingsResponseViewModel();
                m.SettingsId = v;
                var allresults = results.OrderByDescending(l => l.CreatedOn).Where(l => l.SettingsId.Value == v && (l.ResponseParentId == null || l.ResponseParentId == 0)).ToList();
                m.SettingsValue = allresults.FirstOrDefault().SettingsValue;
                m.Comments = new List<SettingsResponseAll>();
                foreach (var all in allresults)
                {
                    SettingsResponseAll modelcomment = new SettingsResponseAll();
                    modelcomment.Id = all.Id;
                    modelcomment.Message = all.Message;
                    modelcomment.MobileNo = all.PhoneNo;
                    modelcomment.CreatedOn = all.CreatedOn.Value.ToString("dd/MM/yyyy");
                    var listuser = myapp.tbl_User.Where(l => l.CustomUserId == all.CreatedBy).ToList();
                    modelcomment.CreatedBy = listuser.Count > 0 ? listuser.FirstOrDefault().FirstName + " (" + listuser.FirstOrDefault().DepartmentName + ")" : "";
                    modelcomment.SubComments = new List<SettingsResponseAll>();
                    var results2 = results.OrderByDescending(l => l.CreatedOn).Where(l => l.ResponseParentId == all.Id).ToList();
                    foreach (var all2 in results2)
                    {
                        SettingsResponseAll modelcomment2 = new SettingsResponseAll();
                        modelcomment2.Id = all2.Id;
                        modelcomment2.Message = all2.Message;
                        modelcomment2.CreatedOn = all2.CreatedOn.Value.ToString("dd/MM/yyyy");
                        var listuser2 = myapp.tbl_User.Where(l => l.CustomUserId == all2.CreatedBy).ToList();
                        modelcomment2.CreatedBy = listuser2.Count > 0 ? listuser2.FirstOrDefault().FirstName : "";
                        modelcomment.SubComments.Add(modelcomment2);
                    }
                    m.Comments.Add(modelcomment);
                }
                model.Add(m);
            }
            return model;
        }


        public List<SettingsResponseViewModel> GetSettingsResponsesUser(string Key, string currentuser)
        {
            var results = myapp.tbl_SettingsResponse.Where(l => l.SettingsKey == Key).OrderByDescending(l => l.CreatedOn).ToList();
            List<SettingsResponseViewModel> model = new List<SettingsResponseViewModel>();
            var keysettings = results.Select(l => l.SettingsId.Value).Distinct().ToList();
            foreach (var v in keysettings)
            {
                SettingsResponseViewModel m = new SettingsResponseViewModel();
                m.SettingsId = v;
                var allresults = results.OrderByDescending(l => l.CreatedOn).Where(l => l.SettingsId.Value == v && (l.ResponseParentId == null || l.ResponseParentId == 0)
                 && l.CreatedBy == currentuser).ToList();
                if (allresults.Count > 0)
                {
                    m.SettingsValue = allresults.FirstOrDefault().SettingsValue;
                    m.Comments = new List<SettingsResponseAll>();
                    foreach (var all in allresults)
                    {
                        SettingsResponseAll modelcomment = new SettingsResponseAll();
                        modelcomment.Id = all.Id;
                        modelcomment.Message = all.Message;
                        modelcomment.MobileNo = all.PhoneNo;
                        modelcomment.CreatedOn = all.CreatedOn.Value.ToString("dd/MM/yyyy");
                        var listuser = myapp.tbl_User.Where(l => l.CustomUserId == all.CreatedBy).ToList();
                        modelcomment.CreatedBy = listuser.Count > 0 ? listuser.FirstOrDefault().FirstName + " (" + listuser.FirstOrDefault().DepartmentName + ")" : "";
                        modelcomment.SubComments = new List<SettingsResponseAll>();
                        var results2 = results.OrderByDescending(l => l.CreatedOn).Where(l => l.ResponseParentId == all.Id).ToList();
                        foreach (var all2 in results2)
                        {
                            SettingsResponseAll modelcomment2 = new SettingsResponseAll();
                            modelcomment2.Id = all2.Id;
                            modelcomment2.Message = all2.Message;
                            modelcomment2.CreatedOn = all2.CreatedOn.Value.ToString("dd/MM/yyyy");
                            var listuser2 = myapp.tbl_User.Where(l => l.CustomUserId == all2.CreatedBy).ToList();
                            modelcomment2.CreatedBy = listuser2.Count > 0 ? listuser2.FirstOrDefault().FirstName : "";
                            modelcomment.SubComments.Add(modelcomment2);
                        }
                        m.Comments.Add(modelcomment);
                    }
                    model.Add(m);
                }
            }
            return model;
        }
    }
}