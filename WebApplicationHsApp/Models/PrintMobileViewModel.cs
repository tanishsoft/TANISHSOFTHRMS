using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class PrintMobileViewModel
    {

        public int Id { get; set; }
        public string Subject { get; set; }
        public string Description { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string BrandName { get; set; }
        public Nullable<int> ModelId { get; set; }
        public string ModelName { get; set; }
        public string SerialNumber { get; set; }
        public string BatterySerialNo { get; set; }
        public string DOP { get; set; }
        public string IMEINO { get; set; }
        public string AssetLabel { get; set; }
        public string Accessories { get; set; }
        public string MonthlyPlan { get; set; }
        public string SIMNO { get; set; }
        public string MobileNumber { get; set; }
        public Nullable<int> AssigntoEmpId { get; set; }
        public string AssigntoEmpNmae { get; set; }
        public string AssignDate { get; set; }
        public string AssignComments { get; set; }
        public Nullable<int> IssuedByEmpId { get; set; }
        public string IssuedByEmpName { get; set; }
        public string AssignEmpAchknowledged { get; set; }
        public string AssetStatus { get; set; }
        public Nullable<int> ReturnReceivedBy { get; set; }
        public string ReturnReceivedName { get; set; }
        public string ReturnReceivedOn { get; set; }
        public string ReturnReceivedComments { get; set; }
        public Nullable<bool> CanReUse { get; set; }     
        public Nullable<int> CellPhoneRequestId { get; set; }   
        public string Name { get; set; }
        public string Designation { get; set; }
        public Nullable<int> EmpNo { get; set; }
        public string Department { get; set; }
        public string Purpose { get; set; }
        public Nullable<bool> IsHODApproved { get; set; }
        public string HODComments { get; set; }
        public Nullable<bool> IsAdministratorApproved { get; set; }
        public string AdministratorComments { get; set; }
        public string CurrentApprover { get; set; }
        public string Status { get; set; }     
        public Nullable<int> LocationId { get; set; }
        public string RequestType { get; set; }
        public string ApprovedRequestType { get; set; }
        public string HODName { get; set; }
        public string AdminName { get; set; }
        public string HODApprovedOn { get; set; }
        public string accpected { get; set; }
        public string AdminApprovedOn { get; set; }
        public string ITUpdatedBy { get; set; }

        public string ITUpdatedOn { get; set; }
    }
    public class MobileAsset
    {
        public tbl_Asset_MobileAllotment mobileAllotment { get; set; }
        public List<tbl_Asset_MobileAllotment> mobileAllotmentList { get; set; }
        public tbl_CellPhoneRequest cellPhoneRequest { get; set ;}
    }
}