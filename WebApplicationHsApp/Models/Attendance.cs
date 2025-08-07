using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class Attendance
    {
        public string EmployeeId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string InTime { get; set; }
        public string OutTime { get; set; }
        public string LateHours { get; set; }
        public string ExtraHoust { get; set; }
        public bool IsSingleSwipe { get; set; }
        public string ShiftName { get; set; }
        public string LeaveType { get; set; }
        public string Remarks { get; set; }

    }
    public class AttendanceSwipeRoasterReport
    {
        public string EmployeeName { get; set; }
        public string EmployeeId { get; set; }
        public string TotalPresent { get; set; }
        public string TotalAbsent { get; set; }
        public string TotalLeaves { get; set; }
        public string TotalLop { get; set; }
        public string Total { get; set; }
        public List<AttendanceDayWise> AttendanceDayWise { get; set; }
    }
    public class AttendanceDayWise
    {
        public string Shift { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }
        public string LateIn { get; set; }
        public string EarlyIn { get; set; }
        public string EarlyOut { get; set; }
        public string LateOut { get; set; }
        public string OTHrs { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
    }
}