namespace WebApplicationHsApp.Models
{
    public class TaskApproveViewModel
    {
        public int TaskApproveId { get; set; }
        public string ApproveComments { get; set; }
        public string ApprovedDate { get; set; }
        public bool? IsApproved { get; set; }
        public string ApprovalLevel { get; set; }
        public string FirstName { get; set; }
        public string DepartmentName { get; set; }
        public int? EmpId { get; set; }
        public int TaskId { get; set; }
    }
}