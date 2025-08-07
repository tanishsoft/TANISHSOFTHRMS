using System;

namespace WebApplicationHsApp.Models
{
    public class CourseViewModel
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public string Description { get; set; }
        public string CourseImg { get; set; }
        public string DateOfCourse { get; set; }
        public Nullable<int> Rating { get; set; }
        public Nullable<bool> IsGlobal { get; set; }
        public Nullable<int> LocationId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
    }
    public class CourceTraineeViewModel
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeSign { get; set; }
        public string DateOfCourse { get; set; }
        public string Department { get; set; }
        public string TraineerName { get; set; }
        public string Mode { get; set; }
        public string Remarks { get; set; }
    }
}