using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class purchasemodel
    {
        public int PurchaseId { get; set; }
        public string PurchaseNumber { get; set; }
        public string PurchaseDate { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Duration { get; set; }
        public string StartDate { get; set; }
        public Nullable<int> LocationId { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string Employee { get; set; }
        public Nullable<int> TotalQty { get; set; }
        public string QuotationNumber { get; set; }
        public Nullable<decimal> QuotationAmount { get; set; }
        public Nullable<decimal> PurchaseAmout { get; set; }
        public Nullable<decimal> TotalTaxAmount { get; set; }
        public Nullable<decimal> Discount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string Status { get; set; }
        public Nullable<int> VendorId { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}