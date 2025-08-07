using System;

namespace WebApplicationHsApp.Models
{
    public class PmViewModel
    {
        public int PreventiveMaintenanceId { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<int> SubDepartmentId { get; set; }
        public string SubDepartmentName { get; set; }
        public string Month { get; set; }
        public string Equipment { get; set; }
        public string AssetNumber { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string Manufacture { get; set; }
        public string AssignTo { get; set; }
        public string WorkDoneDescription { get; set; }
        public string SpareReplaced { get; set; }
        public string JobDoneby { get; set; }
        public string CallStatus { get; set; }
        public string Document1 { get; set; }
        public string Document2 { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public string NextDueDate { get; set; }
        public string AdminComments { get; set; }
        public string type { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string AssignToName { get; set; }
        public Nullable<int> PreventiveCheckListId { get; set; }
        public string PreventiveCheckList { get; set; }
        public int UserEmpId { get; set; }
    }
}