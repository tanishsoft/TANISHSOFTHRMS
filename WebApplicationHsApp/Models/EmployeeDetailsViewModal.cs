using System;

namespace WebApplicationHsApp.Models
{
    public class EmployeeDetailsViewModal
    {
        public long EmpId { get; set; }
        public string EmpCode { get; set; }
        public string EmpName { get; set; }
        public string EmpDepartmentId { get; set; }
        public string EmpLocationId { get; set; }
        public string EmpDOB { get; set; }
        public Nullable<int> EmpAge { get; set; }
        public string Gender { get; set; }
        public string EmpStatus { get; set; }
        public string EmpSpouseName { get; set; }
        public string EmpSpouseGender { get; set; }
        public string EmpSpouseDOB { get; set; }
        public Nullable<int> EmpSpouseAge { get; set; }
        public string EmpSpouseRelation { get; set; }
        public string EmpDependantRelation { get; set; }
        public string EmpDependantName { get; set; }
        public string EmpDependantGender { get; set; }
        public Nullable<long> EmpDependantAge { get; set; }
        public string EmpDependantDOB { get; set; }
        public string EmpChild1Name { get; set; }
        public string EmpChild1Gender { get; set; }
        public string EmpChild1DOB { get; set; }
        public Nullable<int> EmpChild1Age { get; set; }
        public string EmpChild2Name { get; set; }
        public string EmpChild2Gender { get; set; }
        public string EmpChild2DOB { get; set; }
        public Nullable<int> EmpChild2Age { get; set; }
        public string EmpRemarks { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string EmpChild1Relation { get; set; }
        public string EmpChild2Relation { get; set; }
       
      

    }
}