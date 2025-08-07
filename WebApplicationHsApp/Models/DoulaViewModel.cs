using System;

namespace WebApplicationHsApp.Models
{
    public class DoulaViewModel
    {
        public int DoulaId { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string MrNumber { get; set; }
        public string PatientName { get; set; }
        public string Mobile { get; set; }
        public string EmailId { get; set; }
        public string IpNo { get; set; }
        public string Date { get; set; }
        public string SpecificDoula { get; set; }
        public string PaymentStatus { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public Nullable<bool> IsActive { get; set; }     
        public string Column1 { get; set; }
        public string Column2 { get; set; }
        public string Column3 { get; set; }
    }
}