using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class tbl_bm_ShiftingRequestViewModel
    {
        public int ShiftingRequestId { get; set; }
        public Nullable<int> RequestForLocation { get; set; }
        public Nullable<int> RequestForDepartment { get; set; }
        public Nullable<int> RequestForSubDepartment { get; set; }
        public Nullable<int> RequestEmpId { get; set; }
        public string RequestDoctor { get; set; }
        public string RequestItem { get; set; }
        public string RequestComments { get; set; }
        public string RequestDate { get; set; }
        public Nullable<int> PickupLocation { get; set; }
        public Nullable<int> PickupDepartment { get; set; }
        public string PickupComments { get; set; }
        public string ReceivedMakeModelAssetno { get; set; }
        public string ReceivedDate { get; set; }
        public string ReceivedTime { get; set; }
        public string ConditionOfEquipment { get; set; }
        public string HandOvercomments { get; set; }
        public Nullable<int> HandoverEmp { get; set; }
        public string HandOverSignature { get; set; }
        public string UsedOnPatient { get; set; }
        public string PatientMrNo { get; set; }
        public string UsageStartDate { get; set; }
        public string UsageStartTime { get; set; }
        public string UsageEndDate { get; set; }
        public string UsageEndTime { get; set; }
        public Nullable<int> ReturnEmpId { get; set; }
        public string ReturnConditionOfEquipment { get; set; }
        public string ReturnDate { get; set; }
        public string ReturnTime { get; set; }
        public string ReturnComments { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}