using System;

namespace WebApplicationHsApp.Models
{
    public class UserViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CustomUserId { get; set; }
        public string DateOfBirth { get; set; }
        public string DateOfJoining { get; set; }
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Extenstion { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<int> DepartmentId1 { get; set; }
        public string DepartmentName1 { get; set; }
        public Nullable<int> DepartmentId2 { get; set; }
        public string DepartmentName2 { get; set; }
        public string Designation { get; set; }
        public string PlaceAllocation { get; set; }
        public string UserType { get; set; }
        public string SecurityQuestion { get; set; }
        public string SecurityAnswner { get; set; }
        public string Comments { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<int> SubDepartmentId { get; set; }
        public string SubDepartmentName { get; set; }
        public string DateOfLeaving { get; set; }
        public Nullable<long> DesignationID { get; set; }
        public Nullable<bool> ChangePassword { get; set; }
        public Nullable<bool> IsEmployeesReporting { get; set; }
        public Nullable<int> EmpId { get; set; }
        public Nullable<bool> IsAppLogin { get; set; }
        public string photo { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsOffRollDoctor { get; set; }
        public bool IsOnRollDoctor { get; set; }
        public string UserOnlineStatus { get; set; }
        public string CurrentRoom { get; set; }
        public string Avatar { get; set; }
        public int ReportingManagerId { get; set; }
        public string strtbl_ReportingManager { get; set; }
        public string AdhaarCard { get; set; }
        public string PanCard { get; set; }
        public string UserRole { get; set; }
        public string Qualification { get; set; }
        public string CollageName { get; set; }
    }
}