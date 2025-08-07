using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class Cm_EmployeeOrderViewModel
    {
        public int OrderId { get; set; }
        public Nullable<int> LocationId { get; set; }
        public Nullable<int> EmpId { get; set; }
        public string EmpName { get; set; }
        public string OrderDate { get; set; }
        public Nullable<int> OrderBy { get; set; }
        public string Item1Name { get; set; }
        public string Item2Name { get; set; }
        public string Item3Name { get; set; }
        public string Item4Name { get; set; }
        public string Item5Name { get; set; }
        public Nullable<int> ItemId1 { get; set; }
        public Nullable<int> ItemQty1 { get; set; }
        public Nullable<int> ItemId2 { get; set; }
        public Nullable<int> ItemQty2 { get; set; }
        public Nullable<int> ItemId3 { get; set; }
        public Nullable<int> ItemQty3 { get; set; }
        public Nullable<int> ItemId4 { get; set; }
        public Nullable<int> ItemQty4 { get; set; }
        public Nullable<int> ItemId5 { get; set; }
        public Nullable<int> ItemQty5 { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Remarks { get; set; }
        public string CugNumber { get; set; }
        public string PaymentType { get; set; }
        public string PaymentDetails { get; set; }
        public string OrderType { get; set; }
        public Nullable<int> ApproverId { get; set; }
        public string ApproverStatus { get; set; }
        public string ApproverComments { get; set; }
        public string OrderStatus { get; set; }
    }
}