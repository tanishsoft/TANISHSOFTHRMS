using System;

namespace WebApplicationHsApp.Models
{
    public class HrRemainderViewModel
    {
        public int RemainderId { get; set; }
        public string Title { get; set; }
        public int RemainderTypeId { get; set; }
        public int EmpId { get; set; }
        public string CollageName { get; set; }
        public string NursingCouncil { get; set; }
        public string FirstRegistrationCouncil { get; set; }
        public string RegistrationNumber { get; set; }
        public string LastRenewalDate { get; set; }
        public string NextRenewalDate { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public bool IsCommon { get; set; }
    }
}