namespace WebApplicationHsApp.Models
{
    public class EmpScheduleViewModel
    {
        public int ScheduleId { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string DateOfSchedule { get; set; }
        public int SlotDuration { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public string IsDayBlocked { get; set; }
        public string IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
    }

    public class EmpScheduleTimeViewModel
    {
        public int ScheduleId { get; set; }
        public string StratTime { get; set; }
        public string EndTime { get; set; }
    }
}