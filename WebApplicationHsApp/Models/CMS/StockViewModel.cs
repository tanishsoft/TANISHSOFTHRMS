using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class StockViewModel
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int TotalReceived { get; set; }
        public int TotalSales { get; set; }
        public int Balance { get; set; }
    }
    public class InpatientViewModel
    {
        public int? PatientId { get; set; }
        public DateTime? DateOfOrder { get; set; }
       
    }
}