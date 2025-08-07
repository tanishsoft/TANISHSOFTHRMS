using System;

namespace WebApplicationHsApp.Models
{
    public class TaskSLAUserModel
    {
        public int TaskSLAUserId { get; set; }
        public Nullable<int> TaskLocationId { get; set; }
        public string TaskLocationName { get; set; }

        public Nullable<int> TaskDepartmentId { get; set; }
        public string TaskDepartmentName { get; set; }
        public Nullable<int> SLAUserId { get; set; }
        public string SLAUserName { get; set; }
        public string SLAUserEmail { get; set; }
        public string SLAUserMobile { get; set; }
        public Nullable<int> TaskLevel { get; set; }
        public string Description { get; set; }
        public Nullable<int> DurationInMinutes { get; set; }
        public string TypeOfRequest { get; set; }
        public string DepartmentEmail { get; set; }
        public string DepartmentMobile { get; set; }
    }
}