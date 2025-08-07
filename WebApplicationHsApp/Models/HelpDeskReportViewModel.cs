namespace WebApplicationHsApp.Models
{
    public class HelpDeskReportViewModel
    {
        public int TotalIssues { get; set; }
        public int TotalIssuesClosed { get; set; }
        public int NoOfShivamIssues { get; set; }
        public int NoOfHardwareandOtherIssues { get; set; }
        public int NoOfIssuesclosedWithin24Hours { get; set; }
        public int NoOfIssuesclosedAfter24Hours { get; set; }
        public int NoOfTicketsOpen { get; set; }
        public int NewProcessImplementations { get; set; }
        public double AverageTicketsPerday { get; set; }
        public double AverageTicketsClosedPerday { get; set; }
        public string AverageResponseTime { get; set; }
    }
}