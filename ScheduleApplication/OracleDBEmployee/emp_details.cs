namespace ScheduleApplication.OracleDBEmployee
{
    public class emp_details
    {

        public string empnm { get; set; }
        public string empcode { get; set; }
        public string sdeptid { get; set; }//masters
        public string designationid { get; set; } //masters
        public string address { get; set; }
        public string area { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string pincode { get; set; }
        public string phno { get; set; }
        public string doj { get; set; }
        public string dob { get; set; }
        public string sex { get; set; }
        public string companynm { get; set; }
        public string companyid { get; set; }//masters
        public string grade { get; set; }
        public string bldgrp { get; set; }//masters
        public string emptype { get; set; }//masters
        public string martialstatus { get; set; }
        public string relativename { get; set; }
        public string emailid { get; set; }
        public string locid { get; set; }
        public string branch { get; set; }
        public string emergencyno { get; set; }

    }

    public class designation
    {
        public string designationid { get; set; }
        public string deptid { get; set; }
        public string designationnm { get; set; }
       
    }
    public class subdepts
    {
        public string sdeptid { get; set; }
        public string deptid { get; set; }
        public string subdeptnm { get; set; }
    }
    public class company
    {
        public string companyid { get; set; }
        public string companynm { get; set; }
        public string locid { get; set; }
      
    }
}
