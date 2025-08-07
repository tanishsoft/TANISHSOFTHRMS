using System;
using System.Collections.Generic;

namespace WebApplicationHsApp.Models
{
    public class RelationalClass
    {
        public string LocationName { get; set; }
        public int LocationId { get; set; }
        public string DepartmentName { get; set; }
        public int DepartmentId { get; set; }
        public string MovedDate { get; set; }
        public string UpdatedBy { get; set; }
        public int SubDepartmentId { get; set; }
        public string SudDepartmentName { get; set; }
    }

    public class LeaveData_RelationalClass
    {
        public int LeaveTypeId { get; set; }
        public string LeaveTypeName { get; set; }
        public bool IsFullday { get; set; }
        public bool IsCompOff { get; set; }
        public DateTime DateofAvailableCompoff { get; set; }
        public DateTime LeaveFromDate { get; set; }
        public DateTime LeaveTodate { get; set; }
        public string ReasonForLeave { get; set; }
        public string AddressOnLeave { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string LeaveFromDate_SpecificFormat_2 { get; set; }
        public string LeaveTodate_SpecificFormat_2 { get; set; }

    }

    public class RoasterData_RelationalClass
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime ShiftDate { get; set; }
        public string ShiftDate_SpecificFormat_2 { get; set; }
    }

    public class UserEmailList_RelationalClass
    {
        public int UID { get; set; }
        public string UserID { get; set; }
        public string EmailID { get; set; }
        public string UserName { get; set; }
    }

    public partial class ManageAllocation_RelationalClass
    {
        public int AllocationId { get; set; }
        public string AllocationPlace { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string ContactNo { get; set; }
        public Nullable<System.DateTime> AllocaitonFromDate { get; set; }
        public Nullable<System.DateTime> AllocationTodate { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public string AllocaitonFromDate_StringFormat { get; set; }
        public string AllocationTodate_StringFormat { get; set; }
        public string StartTime_StringFormat { get; set; }
        public string EndTime_StringFormat { get; set; }
        public string AllocationDetails { get; set; }
        public string Comment { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
    }

    public class Rostertemp
    {
        public string UserId { get; set; }
        public int ShiftTypeId { get; set; }
        public string ShiftTypeName { get; set; }
        public string ShiftDate { get; set; }
    }
    public class GraphValues
    {
        public string name { get; set; }
        public List<int> data { get; set; }
    }
}