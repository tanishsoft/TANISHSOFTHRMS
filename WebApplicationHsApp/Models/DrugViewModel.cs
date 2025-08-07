using System;

namespace WebApplicationHsApp.Models
{
    public class DrugViewModel
    {
        public int DrugId { get; set; }
        public string DrugName { get; set; }
        public Nullable<decimal> StockTotal { get; set; }
        public string DrugDescription { get; set; }
        public Nullable<System.DateTime> DrugExpiryDate { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<int> SubDepartmentId { get; set; }
        public string SubDepartmentName { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string Location { get; set; }
        public Nullable<int> FloorId { get; set; }
        public string Floor { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }

    //Reports
    public class DrugReportAddViewModel
    {
        public int SNo { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string DrugName { get; set; }
        public decimal BeforeAddQty { get; set; }
        public decimal AddQty { get; set; }
        public decimal TotalQty { get; set; }
        public string ExpireDate { get; set; }
        public string Username { get; set; }
        public string Location { get; set; }
        public string Floor { get; set; }
        public string Remarks { get; set; }
    }

    //Reports
    public class DrugReportRemoveViewModel
    {
        public int SNo { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string IpNumber { get; set; }
        public string MrNumber { get; set; }
        public string PatientName { get; set; }
        public string RoomNumber { get; set; }
        public string DrugName { get; set; }
        public decimal BeforeGivetopatientqty { get; set; }
        public decimal HowmanygiventoPatient { get; set; }
        public decimal NowbalanceQty { get; set; }
        public string ExpireDate { get; set; }
        public string Username { get; set; }
        public string Location { get; set; }
        public string Floor { get; set; }
        public string Remarks { get; set; }
    }
}