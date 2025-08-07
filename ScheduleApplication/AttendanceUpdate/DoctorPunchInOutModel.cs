using System;

namespace ScheduleApplication.AttendanceUpdate
{
    public class DoctorPunchInOutModel
    {
        public int ResponseId { get; set; }
        public string strUserName { get; set; }
        public string strPassword { get; set; }
        public string strInstance { get; set; }
        public string strDate { get; set; }
        public string authenticationStatus { get; set; }
        public string dateFormatStatus { get; set; }
        public string strEmpId { get; set; }
        public string strEmpCode { get; set; }
        public string strPunchDate { get; set; }
        public string strPunchTime { get; set; }
        public string strPunchMode { get; set; }
        public string strPunchLatitude { get; set; }
        public string strPunchLongitude { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    }
}
