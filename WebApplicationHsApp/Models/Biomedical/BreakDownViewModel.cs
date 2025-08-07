using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp
{
    public class BreakDownViewModel
    {
        public int BreakDownId { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public Nullable<int> SubDepartmentId { get; set; }
        public string SubDepartmentName { get; set; }
        public Nullable<int> AssetId { get; set; }
        public string AssetNumber { get; set; }
        public string EquipmentName { get; set; }
        public string ProblemReported { get; set; }
        public Nullable<int> UserEmpId { get; set; }
        public string UserEmpName { get; set; }
        public string UserStatus { get; set; }
        public string UserSignature { get; set; }
        public string UserComments { get; set; }
        public string ProblemObserved { get; set; }
        public string FaultyAccessoryOrSpare { get; set; }
        public string SN_FaultyAccessoryOrSpare { get; set; }
        public string WorkDone { get; set; }
        public string AccessoryOrSpareReplaced { get; set; }
        public string SN_AccessoryOrSpareReplaced { get; set; }
        public string CallStatus { get; set; }
        public string Document1 { get; set; }
        public string Document2 { get; set; }
        public string StartDate { get; set; }
        public string StartTime { get; set; }
        public string EndDate { get; set; }
        public string EndTime { get; set; }
        public string AdminComments { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public int AssignTo { get; set; }
    }
}