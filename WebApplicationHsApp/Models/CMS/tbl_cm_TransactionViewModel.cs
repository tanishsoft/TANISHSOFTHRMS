using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class tbl_cm_TransactionViewModel
    {
        public int TransactionId { get; set; }
        public string TransactionName { get; set; }
        public string TransactionCustomerType { get; set; }
        public Nullable<int> EmpId { get; set; }
        public Nullable<double> TotalPrice { get; set; }
        public string DiscountType { get; set; }
        public string ModeOfPayment { get; set; }
        public string CardNumber { get; set; }
        public string NameOfTheCard { get; set; }
        public Nullable<double> DiscountValue { get; set; }
        public Nullable<double> TaxAmount { get; set; }
        public Nullable<double> FinalPrice { get; set; }
        public Nullable<double> RefundAmount { get; set; }
        public string EmpName { get; set; }
        public string EmpMobile { get; set; }
        public string EmpEmail { get; set; }
        public Nullable<bool> IsFreeMeal { get; set; }
        public Nullable<int> CanteenId { get; set; }
        public string CanteenName { get; set; }
        public Nullable<int> SalesEmpId { get; set; }
        public string SaleName { get; set; }
        public string SalesEmpNotes { get; set; }
        public string BillAddress { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<double> TotalPaidAmount { get; set; }
        public string SaleType { get; set; }
        public string PatientId { get; set; }
        public string InPatientdRoomNo { get; set; }
        public List<tbl_cm_TransactionItemViewModel> tbl_Cm_TransactionItems { get; set; }
    }
    public class tbl_cm_TransactionItemViewModel
    {
        public int Id { get; set; }
        public Nullable<int> TransactionId { get; set; }
        public Nullable<int> ItemId { get; set; }
        public string ItemName { get; set; }
        public Nullable<int> Qty { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<decimal> Tax { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> FinalAmount { get; set; }
        public string Notes { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }

}