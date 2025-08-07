using System;
using System.Collections.Generic;

namespace WebApplicationHsApp.Models.Appointment
{
    public class PatientBillViewModel
    {
        public int BillId { get; set; }
        public Nullable<int> PatientId { get; set; }
        public string PatientName { get; set; }
        public Nullable<double> TotalPrice { get; set; }
        public string DiscountType { get; set; }
        public string ModeOfPayment { get; set; }
        public string CardNumber { get; set; }
        public string NameOfTheCard { get; set; }
        public Nullable<double> DiscountValue { get; set; }
        public Nullable<double> TaxAmount { get; set; }
        public Nullable<double> FinalPrice { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public List<PatientBillItemViewModel> PatientBillItems { get; set; }
    }
    public class PatientBillItemViewModel
    {
        public int BillItemId { get; set; }
        public Nullable<int> BillId { get; set; }
        public Nullable<int> PatientId { get; set; }
        public string PatientName { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public Nullable<double> ServicePrice { get; set; }
        public Nullable<bool> IsCredit { get; set; }
        public Nullable<bool> IsDebit { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
    public class ServiceList
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsCredit { get; set; }
        public bool IsDebit { get; set; }
        public double Price { get; set; }
        public List<ServiceSubList> SubItems { get; set; }
    }
    public class ServiceSubList
    {
        public string Name { get; set; }
        public double Price { get; set; }
    }
    public partial class PatientViewModel
    {
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string ParentName1 { get; set; }
        public string ParentName2 { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Area { get; set; }
        public string MobileNumber { get; set; }
        public string PatientRegisterType { get; set; }
        public string CreatedBy { get; set; }        
        public string ModifiedBy { get; set; }        
        public string Remarks { get; set; }
    }
}