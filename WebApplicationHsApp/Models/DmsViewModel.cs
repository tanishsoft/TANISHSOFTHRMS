using System;

namespace WebApplicationHsApp.Models
{
    public class DmsViewModel
    {
        public int DocumentId { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public string DocumentName { get; set; }
        public string DocumentUrl { get; set; }
        public string IsEmpCreated { get; set; }
        public string IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public string Remarks { get; set; }
        public string IsActive { get; set; }
        public string IsDeleted { get; set; }
        public string Createdon { get; set; }
        public string Createdby { get; set; }
        public string DisplayToEmp { get; set; }
    }
}