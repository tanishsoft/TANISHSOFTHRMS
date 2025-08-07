using System;

namespace WebApplicationHsApp.Models
{
    public class TaskApproverMasterViewModel
    {
        public int ApproveListId { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Operator { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<int> ApproverLevel { get; set; }
    }
}