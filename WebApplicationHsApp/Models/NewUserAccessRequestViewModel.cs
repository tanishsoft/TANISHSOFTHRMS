using System;

namespace WebApplicationHsApp.Models
{
    public class NewUserAccessRequestViewModel
    {
        public int UserRequestId { get; set; }
        public string RequestFor { get; set; }
        public string RequestIs { get; set; }
        public string EmpNo { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string EmailId { get; set; }
        public string Mobile { get; set; }
        public string SubDepartment { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> EmpLocationId { get; set; }
        public Nullable<int> EmpDepartmentId { get; set; }
        public string HodApprovalId { get; set; }
        public string HodApprovalName { get; set; }
        public Nullable<bool> IsHodApproved { get; set; }
        public string HodApproveComments { get; set; }
        public string ApprovedLocation { get; set; }
        public string SupportTeamComments { get; set; }
        public Nullable<bool> IsSupportTeamCompleted { get; set; }
        public string UsernameProvided { get; set; }
        public string PasswordProvided { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string SameRightsLikeempId { get; set; }
    }
}