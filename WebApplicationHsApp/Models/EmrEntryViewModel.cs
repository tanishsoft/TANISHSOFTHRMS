using System;

namespace WebApplicationHsApp.Models
{
    public class EmrEntryViewModel
    {
        public int EmrEntryId { get; set; }
        public string UserId { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string Mobile { get; set; }
        public string MrNo_PatientName { get; set; }
        public string DateofError { get; set; }
        public string DateOfChange { get; set; }
        public string ReasonForChange { get; set; }
        public string RequestedCorrectionDetails { get; set; }
        public string FormName { get; set; }
        public string Suggestions { get; set; }
        public string ApprovalId1 { get; set; }
        public Nullable<bool> IsApproved1 { get; set; }
        public string Approval1Comments { get; set; }
        public string ApprovalId2 { get; set; }
        public Nullable<bool> IsApproved2 { get; set; }
        public string Approval2Comments { get; set; }
        public string ApprovalId3 { get; set; }
        public Nullable<bool> IsApproved3 { get; set; }
        public string Approval3Comments { get; set; }
        public string ApprovalId4 { get; set; }
        public Nullable<bool> IsApproved4 { get; set; }
        public string Approval4Comments { get; set; }
        public string ApprovalId5 { get; set; }
        public Nullable<bool> IsApproved5 { get; set; }
        public string Approval5Comments { get; set; }
        public string ApprovalId6 { get; set; }
        public Nullable<bool> IsApproved6 { get; set; }
        public string Approval6Comments { get; set; }
        public string ApprovalId7 { get; set; }
        public Nullable<bool> IsApproved7 { get; set; }
        public string Approval7Comments { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifedBy { get; set; }
    }
}