using System;
using System.Collections.Generic;

namespace WebApplicationHsApp.Models
{
    public class EventDelegateViewModel
    {
        public int DelegateId { get; set; }
        public string Name { get; set; }
        public string EmailId { get; set; }
        public string PhoneNumber { get; set; }
        public string Gender { get; set; }
        public string Address { get; set; }
        public string Qualification { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public string Document1 { get; set; }
        public string Document2 { get; set; }
        public string Document3 { get; set; }
        public string Document4 { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Title { get; set; }
        public string RegistrationNo { get; set; }
        public Nullable<int> EventId { get; set; }
        public string EventName { get; set; }
        public string BookingFromDate { get; set; }
        public string BookingToDate { get; set; }
        public Nullable<bool> Workshop { get; set; }
        public Nullable<bool> Conference { get; set; }
        public string UTRTransaction { get; set; }
        public string PaymentType { get; set; }
        public string BankName { get; set; }
        public string CCdetails { get; set; }
        public string TotalAmount { get; set; }
        public string Discount { get; set; }
        public string FinalBlanceTopay { get; set; }
        public List<EventManageDelegateMealTypeViewModel> EventDelegateMealTypeModel { get; set; }
        public List<EventManageDelegateMethodViewModel> EventDelegateMethodModel { get; set; }
        public string CompanyName { get; set; }
    }

    public partial class EventManageDelegateMealTypeViewModel
    {
        public int EventId { get; set; }
        public int MealTypeId { get; set; }
        public int DelegateId { get; set; }
        public string EventDate { get; set; }
        public Nullable<int> Qty { get; set; }
        public string Comments { get; set; }
    }
    public partial class EventManageDelegateMealTypeHistoryModel
    {
        public int EventDelegateMealTypeHistoryId { get; set; }
        public Nullable<int> EventId { get; set; }
        public Nullable<int> MealTypeId { get; set; }
        public Nullable<int> DelegateId { get; set; }
        public string EventDate { get; set; }
        public Nullable<int> Qty { get; set; }
        public string Comments { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class EventDelegateExcelModel
    {
        public string Title { get; set; }
        public string Delegate { get; set; }
        public string Organization { get; set; }
        public string Address { get; set; }
        public string Desig { get; set; }
        public string CellNumber { get; set; }
        public string EmailID { get; set; }
        public string Amount { get; set; }
        public string ReceiptNo { get; set; }
        public string Modeofpayment { get; set; }
        public string UTRTransactionNo { get; set; }
        public string City { get; set; }
        public string TotalDays { get; set; }
        public string Workshop { get; set; }
        public string Conference { get; set; }
        public string RegNo { get; set; }
    }
}