using System.Collections.Generic;

namespace WebApplicationHsApp.Models
{
    public class CpxViewModel
    {
        public int TaskCpxId { get; set; }
        public string CreatorLocationId { get; set; }
        public string CreatorLocationName { get; set; }
        public string CreatorDepartmentId { get; set; }
        public string CreatorDepartmentName { get; set; }
        public string CreatorId { get; set; }
        public string CreatorName { get; set; }
        public string CreatorEmail { get; set; }
        public string ProjectTitle { get; set; }
        public string Item { get; set; }
        public string Qty { get; set; }
        public string Model { get; set; }
        public string MainFeatures { get; set; }
        public decimal EstimatedCost { get; set; }
        public string Priority { get; set; }
        public int RequestForDepartmentId { get; set; }
        public string RequestForDepartment { get; set; }
        public string Budgeted { get; set; }
        public string ItemDetails { get; set; }
        public string IsNewItem { get; set; }
        public string CpxStatus { get; set; }
        public string CpxRelatedTo { get; set; }
        public string MyApprovalStatus { get; set; }
        public string CreatedDate { get; set; }
        public string WorkDoneComments { get; set; }
        public string CurrentApproverLevel { get; set; }
        public string CurrentApprover { get; set; }
        public List<CpxApprovalViewModel> ApprovalList { get; set; }
        public string SupportCompany { get; set; }
        public string ReasonForReplacement { get; set; }
        public string OldProductModel { get; set; }
        public string AMCLastRenewalDate { get; set; }
        public string AMCNextRenewalDate { get; set; }
        public string CPXType { get; set; }
        public string OldProductCost { get; set; }
        public string CpxRelatedToOther { get; set; }
        public string SubProjectTitle { get; set; }
    }
    public class CpxApprovalViewModel
    {
        public int ApproveEmpId { get; set; }
        public string ApproveEmpName { get; set; }
        public string ApproveEmail { get; set; }
        public string IsApproved { get; set; }
        public string ApproveComments { get; set; }
        public string ApprovedDate { get; set; }
        public string ApprovalLevel { get; set; }
    }
}