using System;

namespace WebApplicationHsApp.Models
{
    public class LeavesCountView
    {
        public Nullable<int> UserLeaveId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string LocationName { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<int> LocationId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public Nullable<double> CasuvalAvailableLeave { get; set; }
        public Nullable<double> SickAvailableLeave { get; set; }
        public Nullable<double> EarnedAvailableLeave { get; set; }
        public Nullable<double> CompoffBalance { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    }
}