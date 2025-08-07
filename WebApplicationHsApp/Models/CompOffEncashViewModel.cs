using System;

namespace WebApplicationHsApp.Models
{
    public class CompOffEncashViewModel
    {
        public int CompOffEncashId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<int> NoOfDays { get; set; }
        public string SubmitedOn { get; set; }
        public string ReportingTo { get; set; }
        public string ReportingToName { get; set; }
        public string IsApproved { get; set; }
        public string HRApproved { get; set; }
        public string Status { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Remarks { get; set; }
        public string HrRemarks { get; set; }
        public string HrEnchashedDate { get; set; }
    }
}