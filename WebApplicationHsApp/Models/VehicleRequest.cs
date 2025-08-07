using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Models
{
    public class VehicleRequest
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        HrDataManage hrdm = new HrDataManage();
        public List<tbl_Settings> GetAdminDetails()
        {
            try
            {
                List<tbl_Settings> arr = new List<tbl_Settings>();

                var Email = myapp.tbl_Settings.Where(X => X.SettingKey == "SecurityEmail").SingleOrDefault();
                var Phone = myapp.tbl_Settings.Where(X => X.SettingKey == "SecurityPhoneNo").SingleOrDefault();
                var TUserId = myapp.tbl_Settings.Where(X => X.SettingKey == "SecurityUserId").SingleOrDefault();
                var AdminUserId = myapp.tbl_Settings.Where(X => X.SettingKey == "AdminUserId").SingleOrDefault();
                var AdminPhone = myapp.tbl_Settings.Where(X => X.SettingKey == "AdminPhoneNO").SingleOrDefault();
                var AdminEmail = myapp.tbl_Settings.Where(X => X.SettingKey == "AdminEmail").SingleOrDefault();

                if (Email != null)
                    arr.Add(Email);
                if (Phone != null)
                    arr.Add(Phone);
                if (TUserId != null)
                    arr.Add(TUserId);
                if (AdminUserId != null)
                    arr.Add(AdminUserId);
                if (AdminPhone != null)
                    arr.Add(AdminPhone);
                if (AdminEmail != null)
                    arr.Add(AdminEmail);

                return arr;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public bool sendEmailforUpdateSatatus(List<tbl_VehicleRequest> cat)
        {
            try
            {
                string Email = (from var in myapp.tbl_Settings
                                where var.SettingKey == "SecurityEmail"
                                select var.SettingValue).FirstOrDefault();
                string TEmail = (from var in myapp.tbl_Settings where var.SettingKey == "SecurityEmail" select var.SettingValue).FirstOrDefault();
                string TMobile = (from var in myapp.tbl_Settings where var.SettingKey == "SecurityPhoneNo" select var.SettingValue).FirstOrDefault();
                string AdminEmail = (from var in myapp.tbl_Settings where var.SettingKey == "AdminEmail" select var.SettingValue).FirstOrDefault();
                string AdminMobile = (from var in myapp.tbl_Settings where var.SettingKey == "AdminPhoneNO" select var.SettingValue).FirstOrDefault();

                string Customuserid = cat[0].UserId;
                var curuser = (from v in myapp.tbl_User where v.CustomUserId == Customuserid select v).SingleOrDefault();
                string HodEmail = string.Empty;
                if (curuser.UserType == "Employee")
                {
                    var ManagerId = hrdm.GetReportingMgr(cat[0].UserId, DateTime.Now, DateTime.Now);
                    HodEmail = (from var in myapp.tbl_User where var.CustomUserId == ManagerId select var.EmailId).FirstOrDefault();
                }
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                EmailTeamplates emailtemp = new EmailTeamplates();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = TEmail;
                mailmodel.ccemail = AdminEmail + "," + cat[0].Email;
                if (HodEmail != string.Empty)
                {
                    mailmodel.ccemail += "," + HodEmail;
                }
                mailmodel.subject = "A Vehicle Booking Request ID No : " + cat[0].VehicleRequestId + " has been " + cat[0].Status;
                string mailbody = "<p style='font-family:verdana'>Dear Sir/Madam,";
                //string customuserid = User.Identity.Name;         
                if (cat[0].Status == "Approved")
                {
                    mailbody += "<p style='font-family:verdana'> Your Vehicle request has updated.</p>";
                    mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                    mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Pick Up Date</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Date + "</td></tr>";
                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Vehicle Type Allocated</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].VehicleTypeAllocated + "</td></tr>";
                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Vehicle Details</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].VehicleDetails + "</td></tr>";
                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Driver Contact Details</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].DriverContactDetails + "</td></tr>";
                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Vehicle Register Type</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].VehicleRegisterType + "</td></tr>";
                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Status</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Status + "</td></tr>";
                    mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>StartTime</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].StartTime + "</td></tr>";
                    //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Phone</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + curuser.PhoneNumber + "</td></tr>";
                    mailbody += "</table>";
                    if (cat[0].Mobile != null && cat[0].Mobile.ToString() != "")
                    {
                        SendSms smodel = new SendSms();
                        smodel.SendSmsToEmployee(cat[0].Mobile.ToString(), "Hi " + curuser.FirstName + ", Your Vehicle request has Approved for " + cat[0].Date.Value.ToString("dd/MM/yyyy"));
                    }
                }
                else if (cat[0].Status == "Rejected")
                {
                    mailbody += "<p style='font-family:verdana'>Your Request has been Rejected</p>";
                    mailbody += "<p style='font-family:verdana'>Reason : " + cat[0].Remarks + "</p>";
                    if (cat[0].Mobile != null && cat[0].Mobile.ToString() != "")
                    {
                        SendSms smodel = new SendSms();
                        smodel.SendSmsToEmployee(cat[0].Mobile.ToString(), "Hi " + curuser.FirstName + ", Your Vehicle request has Rejected for " + cat[0].Date.Value.ToString("dd/MM/yyyy") + " Reason :" + cat[0].Remarks);
                    }
                }
                else if (cat[0].Status == "In progress")
                {
                    mailbody += "<p style='font-family:verdana'>Your Request is InProgess Please Wait For the More Updates</p>";
                    if (cat[0].Mobile != null && cat[0].Mobile.ToString() != "")
                    {
                        SendSms smodel = new SendSms();
                        smodel.SendSmsToEmployee(cat[0].Mobile.ToString(), "Hi " + curuser.FirstName + ", Your Vehicle request has In Progress for " + cat[0].Date.Value.ToString("dd/MM/yyyy"));
                    }
                }
                int idv = cat[0].VehicleRequestId;
                var comments = myapp.tbl_VehicleRequestComment.Where(l => l.VehicleRequestId == idv).OrderByDescending(l => l.Id).ToList();
                if (comments.Count > 0)
                {
                    mailbody += "<p style='font-family:verdana'>" + comments[0].Comment + "</p>";
                }

                mailbody += "<br/><p style='font-family:cambria'>This is an auto generated mail,Don't reply to this.</p>";

                mailmodel.body = mailbody;
                //mailmodel.body = "A New Ticket Assigned to you";
                //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                mailmodel.filepath = "";
                //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                mailmodel.fromname = "Vehicle Request";

                cm.SendEmail(mailmodel);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public VehicleRequestViewModel GetRequestbyId(int Id)
        {
            try
            {
                var Request = myapp.tbl_VehicleRequest.Where(X => X.VehicleRequestId == Id).SingleOrDefault();
                VehicleRequestViewModel viewmode = new VehicleRequestViewModel();
                viewmode.ArriveTime = Request.ArriveTime;
                viewmode.CreatedBy = Request.CreatedBy;
                viewmode.CreatedOn = Request.CreatedOn;
                viewmode.Date = Request.Date.HasValue ? Request.Date.Value.ToString("dd/MM/yyyy") : "";
                viewmode.Email = Request.Email;
                viewmode.FromAddress = Request.FromAddress;
                viewmode.Mobile = Request.Mobile;
                viewmode.ModifiedBy = Request.ModifiedBy;
                viewmode.ModifiedOn = Request.ModifiedOn;
                viewmode.NoofPersons = Request.NoofPersons;
                viewmode.RequestorComments = Request.RequestorComments;
                viewmode.Returndate = Request.Returndate.HasValue ? Request.Returndate.Value.ToString("dd/MM/yyyy") : "";
                viewmode.returnRequired = Request.returnRequired;
                viewmode.Returntime = Request.Returntime;
                viewmode.StartTime = Request.StartTime;
                viewmode.Status = Request.Status;
                viewmode.ToAddress = Request.ToAddress;
                viewmode.VehicleRequestId = Request.VehicleRequestId;
                viewmode.VehicleTypeAllocated = Request.VehicleTypeAllocated;
                viewmode.VehicleRegisterType = Request.VehicleRegisterType;
                viewmode.VehicleDetails = Request.VehicleDetails;
                viewmode.DriverContactDetails = Request.DriverContactDetails;
                viewmode.Remarks = Request.Remarks;
                if (Request.StartTime != null)
                {
                    try
                    {
                        viewmode.startHour = Request.StartTime.Split(new char[] { ' ' })[0];
                        viewmode.startmin = Request.StartTime.Split(new char[] { ' ' }, 3)[1];
                        viewmode.startam = Request.StartTime.Split(new char[] { ' ' })[2];
                    }
                    catch { }
                }
                return viewmode;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool UpdateRequest(VehicleRequestViewModel UpdateRequest)
        {
            var ValStatus = true;
            tbl_VehicleRequest model = new tbl_VehicleRequest();
            model.VehicleRequestId = UpdateRequest.VehicleRequestId;
            model.VehicleTypeAllocated = UpdateRequest.VehicleTypeAllocated;
            model.VehicleDetails = UpdateRequest.VehicleDetails;
            model.DriverContactDetails = UpdateRequest.DriverContactDetails;
            model.VehicleRegisterType = UpdateRequest.VehicleRegisterType;
            model.Status = UpdateRequest.Status;
            model.StartTime = UpdateRequest.StartTime;
            model.Remarks = UpdateRequest.Remarks;
            var cat = myapp.tbl_VehicleRequest.Where(l => l.VehicleRequestId == model.VehicleRequestId).ToList();
            if (cat.Count > 0)
            {
                cat[0].VehicleTypeAllocated = model.VehicleTypeAllocated;
                cat[0].VehicleDetails = model.VehicleDetails;
                cat[0].DriverContactDetails = model.DriverContactDetails;
                cat[0].VehicleRegisterType = model.VehicleRegisterType;
                cat[0].Remarks = model.Remarks != null ? model.Remarks : cat[0].Remarks;
                cat[0].StartTime = model.StartTime;
                cat[0].ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
                if (cat.Count > 0)
                {
                    if (cat[0].Admin_Status == null)
                        cat[0].Status = model.Status;
                    else
                    {
                        ValStatus = false;
                        return ValStatus; ;
                    }
                }
                myapp.SaveChanges();
                if (UpdateRequest.SendEmail)
                {
                    string TEmail = (from var in myapp.tbl_Settings where var.SettingKey == "SecurityEmail" select var.SettingValue).FirstOrDefault();
                    string TMobile = (from var in myapp.tbl_Settings where var.SettingKey == "SecurityPhoneNo" select var.SettingValue).FirstOrDefault();
                    string AdminEmail = (from var in myapp.tbl_Settings where var.SettingKey == "AdminEmail" select var.SettingValue).FirstOrDefault();
                    string AdminMobile = (from var in myapp.tbl_Settings where var.SettingKey == "AdminPhoneNO" select var.SettingValue).FirstOrDefault();
                    string Customuserid = cat[0].UserId;
                    var curuser = (from v in myapp.tbl_User where v.CustomUserId == Customuserid select v).SingleOrDefault();
                    string HodEmail = string.Empty;
                    if (curuser.UserType == "Employee")
                    {
                        var ManagerId = hrdm.GetReportingMgr(cat[0].UserId, DateTime.Now, DateTime.Now);
                        HodEmail = (from var in myapp.tbl_User where var.CustomUserId == ManagerId select var.EmailId).FirstOrDefault();
                    }
                    CustomModel cm = new CustomModel();
                    MailModel mailmodel = new MailModel();
                    EmailTeamplates emailtemp = new EmailTeamplates();
                    mailmodel.fromemail = "Leave@hospitals.com";
                    mailmodel.toemail = cat[0].Email;
                    mailmodel.ccemail = TEmail + "," + AdminEmail;
                    if (HodEmail != string.Empty)
                    {
                        mailmodel.ccemail += "," + HodEmail;
                    }
                    mailmodel.subject = "A Vehicle Booking Request ID No : " + cat[0].VehicleRequestId + " has been " + model.Status;
                    string mailbody = "<p style='font-family:verdana'>HI Team,";
                    //string customuserid = User.Identity.Name;

                    if (cat[0].Status == "Approved")
                    {
                        mailbody += "<p style='font-family:verdana'> Your Vehicle request has updated.</p>";
                        mailbody += "<p style='font-family:verdana'>Please find the below details.</p>";
                        mailbody += "<table style='border:1px solid #a1a2a3;width: 60%;'><tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Call Date</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Date + "</td></tr>";
                        mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Vehicle Type Allocated</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].VehicleTypeAllocated + "</td></tr>";
                        mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Vehicle Details</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].VehicleDetails + "</td></tr>";
                        mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Driver Contact Details</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].DriverContactDetails + "</td></tr>";
                        mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Vehicle Register Type</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].VehicleRegisterType + "</td></tr>";
                        mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Status</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].Status + "</td></tr>";
                        mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>StartTime</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + cat[0].StartTime + "</td></tr>";
                        //mailbody += "<tr><td style='border:1px solid #a1a2a3;font-family:verdana'>Requestor Phone</td><td style='border:1px solid #a1a2a3;font-family:verdana'>" + curuser.PhoneNumber + "</td></tr>";
                        mailbody += "</table>";
                        if (cat[0].Mobile != null && cat[0].Mobile.ToString() != "")
                        {
                            SendSms smodel = new SendSms();
                            smodel.SendSmsToEmployee(cat[0].Mobile.ToString(), "Hi " + curuser.FirstName + ", Your Vehicle request has Approved for " + cat[0].Date.Value.ToString("dd/MM/yyyy"));
                        }
                    }
                    else if (cat[0].Status == "Rejected")
                    {
                        mailbody += "<p style='font-family:verdana'>Your Request has been Rejected due to some reason</p>";
                        if (cat[0].Mobile != null && cat[0].Mobile.ToString() != "")
                        {
                            SendSms smodel = new SendSms();
                            smodel.SendSmsToEmployee(cat[0].Mobile.ToString(), "Hi " + curuser.FirstName + ", Your Vehicle request has Rejected for " + cat[0].Date.Value.ToString("dd/MM/yyyy"));
                        }
                    }
                    else if (cat[0].Status == "In progress")
                    {
                        mailbody += "<p style='font-family:verdana'>Your Request is InProgess Please Wait For the More Updates</p>";
                        if (cat[0].Mobile != null && cat[0].Mobile.ToString() != "")
                        {
                            SendSms smodel = new SendSms();
                            smodel.SendSmsToEmployee(cat[0].Mobile.ToString(), "Hi " + curuser.FirstName + ", Your Vehicle request has In Progress for " + cat[0].Date.Value.ToString("dd/MM/yyyy"));
                        }
                    }
                    else if (cat[0].Status == "VEHICLE NOT AVAILABLE")
                    {
                        mailbody += "<p style='font-family:verdana'>No Vehicle is Available at " + cat[0].Date + "  </p>";
                        if (cat[0].Mobile != null && cat[0].Mobile.ToString() != "")
                        {
                            SendSms smodel = new SendSms();
                            smodel.SendSmsToEmployee(cat[0].Mobile.ToString(), "Hi " + curuser.FirstName + ", No Vehicle is Available for " + cat[0].Date.Value.ToString("dd/MM/yyyy"));
                        }
                    }
                    int idv = cat[0].VehicleRequestId;
                    var comments = myapp.tbl_VehicleRequestComment.Where(l => l.VehicleRequestId == idv).OrderByDescending(l => l.Id).ToList();
                    if (comments.Count > 0)
                    {
                        mailbody += "<p style='font-family:verdana'>" + comments[0].Comment + "</p>";
                    }
                    mailbody += "<br/><p style='font-family:cambria'>This is an auto generated mail,Don't reply to this.</p>";

                    mailmodel.body = mailbody;
                    //mailmodel.body = "A New Ticket Assigned to you";
                    //mailmodel.body = emailtemp.NewTicketTemplate(task.TaskId.ToString(), task.CallDateTime.ToString(), task.CreatorDepartmentName + " " + task.CreatorName, task.CategoryOfComplaint, task.Description, task.AssignLocationName + " " + task.AssignDepartmentName, task.AssignName);
                    mailmodel.filepath = "";
                    //mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                    mailmodel.fromname = "Vehicle Request";
                    //mailmodel.ccemail = "";
                    cm.SendEmail(mailmodel);
                }
            }
            return ValStatus;
        }
    }
}