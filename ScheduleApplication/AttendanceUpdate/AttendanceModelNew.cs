using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleApplication.AttendanceUpdate
{
    public class AttendanceModelNew
    {
        public DateTime att_date { get; set; }
        public string employeeid { get; set; }
        public string in_time { get; set; }
        public string out_time { get; set; }
    }
}
