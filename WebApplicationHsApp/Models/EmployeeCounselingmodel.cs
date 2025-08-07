using System;
using System.Collections.Generic;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class EmployeeCounselingmodel
    {
        public int CounselingId { get; set; }
        public Nullable<int> EmpId { get; set; }
        public string EmpName { get; set; }
        public string EmpComments { get; set; }
        public string EmpMobile { get; set; }
        public string CounselingType { get; set; }
        public Nullable<int> DoctorEmpId { get; set; }
        public string DoctorEmpName { get; set; }
        public string DateOfSchedule { get; set; }
        public string SlotTime { get; set; }
        public Nullable<int> ScheduleId { get; set; }
        public string Observation { get; set; }
        public Nullable<decimal> CounselingFee { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string OtherColumn { get; set; }
        public string OtherColumn1 { get; set; }
        public List<tbl_EmpCounselingFamily> families { get; set; }
    }

    public class CounselingObservationViewModel
    {
        public int CounselingId { get; set; }
        public string Observation { get; set; }
        public List<CounselingObservationFamillyViewModel> family { get; set; }
    }
    public class CounselingObservationFamillyViewModel
    {
        public int Id { get; set; }
        public string Observation { get; set; }

    }

}