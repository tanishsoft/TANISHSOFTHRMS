using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class VendorMealRequestModel
    {
        public int VendorStockRequestId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string VendorName { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> TotalQtyRequested { get; set; }
        public Nullable<int> TotalQtyReceived { get; set; }
        public Nullable<int> TotalQtyReturn { get; set; }
        public Nullable<int> TotalQtyDamage { get; set; }
        public string TotalNotes { get; set; }
        public Nullable<int> RequestedEmpId { get; set; }
        public string RequestedOn { get; set; }
        public Nullable<int> ReceiverEmpId { get; set; }
        public string ReceivedOnDate { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedOn { get; set; }
    }
}