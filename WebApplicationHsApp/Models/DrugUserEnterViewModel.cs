using System;
using System.Collections.Generic;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class DrugUserEnterViewModel
    {
      
        public Nullable<int> DrugId { get; set; }       
        public Nullable<decimal> StockQty { get; set; }
        public string StockDate { get; set; }

    }
    public class ModeltoBindDataDrug
    {
       public List<DrugUserEnterViewModel> model { get; set; }
        public List<tbl_Drug> drugs { get; set; }
      public  List<string> Dates { get; set; }
    }
    public class DrugUserEntryHistoryModel
    {
        public int DrugNICUReportId { get; set; }
        public Nullable<int> DrugId { get; set; }
        public string DrugName { get; set; }
        public Nullable<decimal> StockQty { get; set; }
        public Nullable<bool> IsCredit { get; set; }
        public Nullable<bool> IsDebit { get; set; }
        public string PatientId { get; set; }
        public string PatientName { get; set; }
        public string StockGivenPerson { get; set; }
        public string Remarks { get; set; }
        public string StockDate { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string TypeOfStock { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string MrNumber { get; set; }
        public string RoomNo { get; set; }
        public string DrugExpiryDate { get; set; }
    }
}