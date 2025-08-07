using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class ReveicedStockViewModel
    {
        public int ReceivedStockId { get; set; }
        public Nullable<int> StoreId { get; set; }
        public string StockRecivedOn { get; set; }
        public Nullable<int> StockRecivedEmpId { get; set; }
        public string StockRecivedEmpName { get; set; }
        public string StockRecivedNotes { get; set; }
        public Nullable<int> TotalItems { get; set; }
        public Nullable<int> TotalItemsAccepted { get; set; }
        public Nullable<int> TotalItemsReturn { get; set; }
        public Nullable<int> TotalItemsDamage { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}