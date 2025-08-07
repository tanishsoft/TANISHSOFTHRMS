using System;

namespace WebApplicationHsApp.Models
{
    public class callCenterReportViewModel
    {
        public DateTime date { get; set; }
        public string locationName { get; set; }
        public int locationId  {get; set; }
        public string departmentName { get; set; }
        public int departmentId { get; set; }
        public string specialization { get; set; }
        public string doctor { get; set; }
        public int activeTicket { get; set; }
        public int totalTickets { get; set; }

        public int closedWithInTAT { get; set; }
        public int closedOutOfTAT { get; set; }

        public int secondLevelPending { get; set; }
        public int thirdLevelPending { get; set; }

    }
}