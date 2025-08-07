using System;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class LeaveApply
    {
        public long LeaveId { get; set; }
        public Nullable<int> LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public Nullable<bool> IsFullday { get; set; }
        public Nullable<bool> IsCompOff { get; set; }
        public string DateofAvailableCompoff { get; set; }
        public string LeaveFromDate { get; set; }
        public string LeaveTodate { get; set; }
        public string ReasonForLeave { get; set; }
        public string AddressOnLeave { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Level1Approver { get; set; }
        public Nullable<bool> Level1Approved { get; set; }
        public string Level1ApproveComment { get; set; }
        public string Level2Approver { get; set; }
        public Nullable<bool> Level2Approved { get; set; }
        public string Level2ApproveComment { get; set; }
        public string Level3Approver { get; set; }
        public Nullable<bool> Level3Approved { get; set; }
        public string Level3ApproveComment { get; set; }
        public string LeaveStatus { get; set; }
        public string LeaveSessionDay { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<double> LeaveCount { get; set; }
        public string WeeklyOffDay { get; set; }

    }

    public class ConvertLeaveObj
    {
        public tbl_Leave Convert(LeaveApply obj)
        {
            tbl_Leave m = new tbl_Leave();
            m.LeaveId = obj.LeaveId;
            m.LeaveTypeId = obj.LeaveTypeId;
            m.LeaveTypeName = obj.LeaveTypeName;
            m.IsFullday = obj.IsFullday;
            m.IsCompOff = obj.IsCompOff;
            m.DateofAvailableCompoff = ProjectConvert.ConverDateStringtoDatetime(obj.DateofAvailableCompoff);
            m.LeaveFromDate = ProjectConvert.ConverDateStringtoDatetime(obj.LeaveFromDate);
            m.LeaveTodate = ProjectConvert.ConverDateStringtoDatetime(obj.LeaveTodate);
            m.ReasonForLeave = obj.ReasonForLeave;
            m.AddressOnLeave = obj.AddressOnLeave;
            m.UserId = obj.UserId;
            m.UserName = obj.UserName;
            m.LocationId = obj.LocationId;
            m.LocationName = obj.LocationName;
            m.DepartmentId = obj.DepartmentId;
            m.DepartmentName = obj.DepartmentName;
            m.Level1Approver = obj.Level1Approver;
            m.Level1Approved = obj.Level1Approved;
            m.Level1ApproveComment = obj.Level1ApproveComment;
            m.Level2Approver = obj.Level2Approver;
            m.Level2Approved = obj.Level2Approved;
            m.Level2ApproveComment = obj.Level2ApproveComment;
            m.Level3Approver = obj.Level3Approver;
            m.Level3Approved = obj.Level3Approved;
            m.Level3ApproveComment = obj.Level3ApproveComment;
            m.LeaveStatus = obj.LeaveStatus;
            m.LeaveSessionDay = obj.LeaveSessionDay;
            m.Remarks = obj.Remarks;
            return m;
        }
        public tbl_Leave ConvertMobile(LeaveApply obj)
        {
            tbl_Leave m = new tbl_Leave();
            m.LeaveId = obj.LeaveId;
            m.LeaveTypeId = obj.LeaveTypeId;
            m.LeaveTypeName = obj.LeaveTypeName;
            m.IsFullday = obj.IsFullday;
            m.IsCompOff = obj.IsCompOff;
            m.DateofAvailableCompoff = ProjectConvert.ConverDateStringtoDatetimeMobile(obj.DateofAvailableCompoff);
            m.LeaveFromDate = ProjectConvert.ConverDateStringtoDatetimeMobile(obj.LeaveFromDate);
            m.LeaveTodate = ProjectConvert.ConverDateStringtoDatetimeMobile(obj.LeaveTodate);
            m.ReasonForLeave = obj.ReasonForLeave;
            m.AddressOnLeave = obj.AddressOnLeave;
            m.UserId = obj.UserId;
            m.UserName = obj.UserName;
            m.LocationId = obj.LocationId;
            m.LocationName = obj.LocationName;
            m.DepartmentId = obj.DepartmentId;
            m.DepartmentName = obj.DepartmentName;
            m.Level1Approver = obj.Level1Approver;
            m.Level1Approved = obj.Level1Approved;
            m.Level1ApproveComment = obj.Level1ApproveComment;
            m.Level2Approver = obj.Level2Approver;
            m.Level2Approved = obj.Level2Approved;
            m.Level2ApproveComment = obj.Level2ApproveComment;
            m.Level3Approver = obj.Level3Approver;
            m.Level3Approved = obj.Level3Approved;
            m.Level3ApproveComment = obj.Level3ApproveComment;
            m.LeaveStatus = obj.LeaveStatus;
            m.LeaveSessionDay = obj.LeaveSessionDay;
            m.Remarks = obj.Remarks;
            return m;
        }
    }
}