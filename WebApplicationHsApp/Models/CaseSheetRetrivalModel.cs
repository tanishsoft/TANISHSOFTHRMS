using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class CaseSheetRetrivalModel
    {
        public int CaseSheetRetrivalId { get; set; }
        public string EmpNo { get; set; }
        public string EmpName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string DataRequestedBy { get; set; }
        public string DataRequestedDate { get; set; }
        public string PurposeOfTheData { get; set; }
        public string SpecificData { get; set; }
        public string SpecificDataValue { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public string StatusRemarks { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Description { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string SheetType { get; set; }
    }
}