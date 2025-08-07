using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class ReportMealsUsedModel
    {
        public int Id { get; set; }
        public string LocationName { get; set; }
        public string Date { get; set; }
        public string BreakFastDelivered { get; set; }
        public string BreakFastUsed { get; set; }
        public string LunchDelivered { get; set; }
        public string LunchUsed { get; set; }
        public string DinnerDelivered { get; set; }
        public string DinnerUsed { get; set; }

    }
}