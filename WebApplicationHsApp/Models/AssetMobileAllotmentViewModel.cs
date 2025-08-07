using System;

namespace WebApplicationHsApp.Models
{
    public class AssetMobileAllotmentViewModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public int? AssetTypeId { get; set; }
        public int? BrandID { get; set; }
        public int? ModelID { get; set; }
        public string SerialNumber { get; set; }
        public string BatterySerialNo { get; set; }
        public string DOP { get; set; }
        public string IMEINO { get; set; }
        public string AssetLabel { get; set; }
        public string Accessories { get; set; }
        public string MonthlyPlan { get; set; }
        public string SIMNO { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<int> AssigntoEmpId { get; set; }
        public string AssignDate { get; set; }
        public string AssignComments { get; set; }
        public Nullable<int> IssuedByEmpId { get; set; }
        public string AssignEmpAchknowledged { get; set; }
        public string AssetStatus { get; set; }
        public Nullable<int> ReturnReceivedBy { get; set; }
        public string ReturnReceivedOn { get; set; }
        public string ReturnReceivedComments { get; set; }
        public Nullable<int> CellPhoneRequestId { get; set; }
    }
}