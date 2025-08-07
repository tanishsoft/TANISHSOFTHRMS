using System;

namespace WebApplicationHsApp.Models
{
    public class CpxApproverModel
    {
        public string ApprovalLevel { get; set; }
        public string Name { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string ApproveComments { get; set; }
    }
}