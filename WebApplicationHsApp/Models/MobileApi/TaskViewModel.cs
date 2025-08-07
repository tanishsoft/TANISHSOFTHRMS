using System;

namespace WebApplicationHsApp.Models
{
    public class TaskViewModel
    {
        public long TaskId { get; set; }
        public string CallDateTime { get; set; }
        public string CallStartDateTime { get; set; }
        public Nullable<int> CreatorLocationId { get; set; }
        public string CreatorLocationName { get; set; }
        public Nullable<int> CreatorDepartmentId { get; set; }
        public string CreatorDepartmentName { get; set; }
        public Nullable<int> CreatorId { get; set; }
        public string CreatorName { get; set; }
        public string CreatorPlace { get; set; }
        public string AssertEquipId { get; set; }
        public string AssertEquipName { get; set; }
        public string CategoryOfComplaint { get; set; }
        public string Description { get; set; }
        public Nullable<int> AssignLocationId { get; set; }
        public string AssignLocationName { get; set; }
        public Nullable<int> AssignDepartmentId { get; set; }
        public string AssignDepartmentName { get; set; }
        public Nullable<int> AssignId { get; set; }
        public string AssignName { get; set; }
        public string WorkDoneRemarks { get; set; }
        public string CallEndDateTime { get; set; }
        public string AssignStatus { get; set; }
        public string CreatorStatus { get; set; }
        public string LatestComment { get; set; }
        public string TaskDoneByUserId { get; set; }
        public string TaskDoneByName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string ExtensionNo { get; set; }
        public string EmailId { get; set; }
        public string Others { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string Subject { get; set; }
        public Nullable<bool> IsVendorTicket { get; set; }
        public string CapexPrepareDate { get; set; }
        public string CapexApproveDate { get; set; }
        public string TaskType { get; set; }
        public string ResponseTime { get; set; }
        public string Age { get; set; }
    }
}