using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class TransactionViewModel
    {
        public int TransactionId { get; set; }
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
        public Nullable<int> SalesEmpId { get; set; }
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
        public List<tbl_cm_TransactionItem> TransactionItems { get; set; }
    }
}