using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class AdminTrackerModel
    {
        public int TrackerId { get; set; }
        public string Title { get; set; }
        public string FormType { get; set; }
        public string LocationId { get; set; }
        public string DepartmentId { get; set; }
        public string LocationName { get; set; }
        public string DepartmentName { get; set; }
        public string Description { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string NextRenewalDate { get; set; }
        public Nullable<int> Remainder1NoOfDays { get; set; }
        public string Remainder1NoOfDaysNotes { get; set; }
        public Nullable<int> Remainder2NoOfDays { get; set; }
        public string Remainder2NoOfDaysNotes { get; set; }
        public Nullable<int> Remainder3NoOfDays { get; set; }
        public string Remainder3NoOfDaysNotes { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
     
        public string PrimaryUserId { get; set; }
        public string SecondaryUserid { get; set; }
        public string LicenseDepartmentName_Gov { get; set; }
        public string AgreementSubType { get; set; }
    }
}