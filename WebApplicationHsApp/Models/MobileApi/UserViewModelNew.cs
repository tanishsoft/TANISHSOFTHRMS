using System;

namespace WebApplicationHsApp.Models
{
    public class UserViewModelNew
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomUserId { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<System.DateTime> DateOfJoining { get; set; }
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }       
        public string Extenstion { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string Designation { get; set; }
        public Nullable<int> SubDepartmentId { get; set; }
        public string SubDepartmentName { get; set; }       
        public Nullable<int> EmpId { get; set; }
        public string CugNumber { get; set; }       
        public Nullable<int> ReportingManagerId { get; set; }
    }
   
}