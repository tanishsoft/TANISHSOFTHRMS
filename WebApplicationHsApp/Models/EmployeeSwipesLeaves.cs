using System;

namespace WebApplicationHsApp.Models
{
    public class EmployeeSwipesLeaves
    {
        public Nullable<int> employeeid { get; set; }
        public string shft_cod { get; set; }
        public string cmp_code { get; set; }
        public Nullable<System.DateTime> att_date { get; set; }
        public string in_time { get; set; }
        public string out_time { get; set; }
        public string extrahours { get; set; }
        public string earlyin { get; set; }
        public string earlyout { get; set; }
        public Nullable<decimal> latein { get; set; }
        public int TotalDaysLate { get; set; }
        public decimal LeavestoBededuct { get; set; }
        public string lateout { get; set; }
        public string status { get; set; }
        public string LocationName { get; set; }
        public string DepartmentName { get; set; }
        public string FirstName { get; set; }
    }
}