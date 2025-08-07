using System;

namespace WebApplicationHsApp.Models
{
    public class CompOffLeaveViewModel
    {
        public long id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmailId { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string CompOffDate { get; set; }
        public Nullable<System.DateTime> CompOffDateTime { get; set; }
        public string RequestReason { get; set; }
        public Nullable<bool> Record_Status { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public Nullable<System.DateTime> CreatedDateTime { get; set; }
        public string CreatedDate_CustomFormat { get; set; }
        public string Request_Status { get; set; }
        public Nullable<bool> IsApproved_Admin { get; set; }
        public Nullable<bool> IsApproved_Manager { get; set; }
        public Nullable<bool> IsApproved_Manager_2 { get; set; }
        public Nullable<bool> IsApproved_Manager_3 { get; set; }
        public Nullable<bool> IsApproved_Manager_4 { get; set; }
        public string ExpiryDate { get; set; }
        public Nullable<System.DateTime> ExpiryDateTime { get; set; }
        public string Leave_Status { get; set; }
        public string CompOffLeave_Approver_1 { get; set; }
        public string CompOffLeave_Approver_2 { get; set; }
        public Nullable<int> ShiftTypeId { get; set; }
        public Nullable<bool> IsLeaveTaken { get; set; }
        public Nullable<double> LeavesTakenCount { get; set; }
    }
}