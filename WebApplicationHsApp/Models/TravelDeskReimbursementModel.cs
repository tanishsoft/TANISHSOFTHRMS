using System.Collections.Generic;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class TravelDeskReimbursementModel
    {
        public int ReimbursementId { get; set; }
        public string EmpId { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string ConferenceName { get; set; }
        public string Venue { get; set; }
        public string ConferenceDate { get; set; }
        public string IsTravelExpense { get; set; }
        public decimal TravelExpense { get; set; }
        public string IsAccommodation { get; set; }
        public decimal Accommodation { get; set; }
        public string IsMiscellaneous { get; set; }
        public decimal MiscellaneousAmount { get; set; }
        public string IsRegistrationFee { get; set; }
        public decimal RegistrationFeeAmount { get; set; }
        public int TravelRequestId { get; set; }
        public int HodUserId { get; set; }
        public string HodUserName { get; set; }
        public string HodEmailId { get; set; }
        public string HodName { get; set; }
        public string HodAction { get; set; }
        public string HodRemarks { get; set; }
        public int CurrentLevel { get; set; }
        public string AccountDeptAction { get; set; }
        public string AccountDeptRemarks { get; set; }
        public int AccountDeptUserId { get; set; }
        public bool IsActive { get; set; }
        public string Status { get; set; }
        public string PaidThrough { get; set; }
        public decimal TotalAmount { get; set; }
        public string Notes { get; set; }
        public string TravellerDetails { get; set; }
        public List<tbl_TravelDeskReimbursementDocument> documents { get; set; }
    }
}