using System;
using System.Collections.Generic;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class GatePassViewModel
    {
        public int GatePassId { get; set; }
        public int LocationId { get; set; }
        public string ToLocation { get; set; }
        public string ToLocationCmpCPName { get; set; }
        public string ToLocationCmpCPMobile { get; set; }
        public string ToLocationCmpCPEmail { get; set; }
        public string CurrentWorkFlow { get; set; }
        public string NameOfTheCompany { get; set; }
        public string Date { get; set; }
        public string ExpectedDateOfReturn { get; set; }
        public string Address { get; set; }
        public string AuthorizedByName { get; set; }
        public int AuthorizedByEmpId { get; set; }
        public string AuthorizedByDesignation { get; set; }
        public string AuthorizedByDepartment { get; set; }
        public string ReceivingCompanyName { get; set; }
        public string Comments { get; set; }
        public string GatePassType { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public List<tbl_GatePassAsset> assets { get; set; }
        public int ToDepartmentId { get; set; }
    }
}