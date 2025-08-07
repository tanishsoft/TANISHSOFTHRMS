namespace WebApplicationHsApp.Models
{
    public class ConcessionViewModel
    {
        public int ConcessionId { get; set; }
        public int LocationId { get; set; }
        public string DateOfSubmit { get; set; }
        public string IPNumber { get; set; }
        public string PatientName { get; set; }
        public string Category { get; set; }
        public string ConcessionAdvised { get; set; }
        public string Justification { get; set; }
        public string TotalBillAmount { get; set; }
        public string TotalConcessionAmount { get; set; }
        public string ConcessionType { get; set; }
        public string Package { get; set; }
        public string PaidByPatient { get; set; }
        public string Service { get; set; }
        public string Consultation { get; set; }
        public string Scan { get; set; }
        public string Investigations { get; set; }
        public string ProcedureName { get; set; }
        public string Status { get; set; }
        public string ApproverStatus { get; set; }
        public string Approver { get; set; }
        public string ApproverComments { get; set; }
        public string Remarks { get; set; }
        public string BillNo { get; set; }
        public string SubCategory { get; set; }
        public string Validity { get; set; }
        public string SendemailMd { get; set; }
        public string TotalConcessionPercentage { get; set; }
        public string DocumentName { get; set; }
        public string DateOfAdmission { get; set; }
        public string DateOfDischarge { get; set; }
        public string Pharmacy { get; set; }
        public string NST { get; set; }
        public string SendEmailTo { get; set; }
       
        public string LocationName { get; set; }
    }
}