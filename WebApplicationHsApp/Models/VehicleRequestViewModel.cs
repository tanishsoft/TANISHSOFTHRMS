using System;

namespace WebApplicationHsApp.Models
{
    public class VehicleRequestViewModel
    {
        public int VehicleRequestId { get; set; }
        public string VehicleTypeRequested { get; set; }
        public string VehicleTypeAllocated { get; set; }
        public string UserId { get; set; }
        public string Date { get; set; }
        public string FromAddress { get; set; }
        public string ToAddress { get; set; }
        public string ArriveTime { get; set; }
        public string StartTime { get; set; }
        public string RequestorComments { get; set; }
        public string Remarks { get; set; }
        public string VehicleDetails { get; set; }
        public string DriverContactDetails { get; set; }
        public string Status { get; set; }
        public string VehicleRegisterType { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string startHour { get; set; }
        public string startmin { get; set; }
        public string startam { get; set; }
        public bool SendEmail { get; set; }
        public Nullable<long> Mobile { get; set; }
        public string Email { get; set; }
        public Nullable<long> NoofPersons { get; set; }
        public string returnRequired { get; set; }
        public string Returndate { get; set; }
        public string Returntime { get; set; }
    }
}
