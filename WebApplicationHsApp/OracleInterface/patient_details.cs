namespace WebApplicationHsApp.OracleInterface
{
    public class patient_details
    {
        public string regdt { get; set; }
        public string regtm { get; set; }
        public string mrno { get; set; }
        public string ipno { get; set; }
        public string name { get; set; }
        public string AGE { get; set; }
        public string PHNO { get; set; }
        public string BEDNO { get; set; }
        public string ROOMTYPES { get; set; }
    }
    public class Patient_Billing
    {
        public string patid { get; set; }
        public string ipno { get; set; }
        public string name { get; set; }
        public string admissiondate { get; set; }
        public string dischargedt { get; set; }
        public string totalamount { get; set; }
        public string paidamount { get; set; }
        public string discount { get; set; }
        public string balance { get; set; }
        public string postdisc { get; set; }
        public string NETAMOUNT { get; set; }
    }
}