namespace WebApplicationHsApp.Models
{
    public class TaskAutoAssignViewModel
    {
        public int Id { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }

        public int CreatorLocationId { get; set; }
        public string CreatorLocationName { get; set; }
        public int CreatorDepartmentId { get; set; }
        public string CreatorDepartmentName { get; set; }


        public string AssignToUserid { get; set; }
        public string AssignToUserName { get; set; }
        public string Subject { get; set; }
        public bool NotifySMS { get; set; }
        public bool NotifyEmail { get; set; }
    }
}