using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class RefundRequestViewModel
    {
        public int RefundRequestId { get; set; }
        public int LocationId { get; set; }
        public string Justification { get; set; }
        public string ModeUsed { get; set; }
        public string ModeUsedDetails
        { get; set; }
        public string PatientName { get; set; }
        public string PatientIpNo
        { get; set; }
        public string PatientMrNo { get; set; }
        public string PatientMobile
        { get; set; }
        public string RefundAmount { get; set; }
        public string RefundBillNo
        { get; set; }
        public string AccountHolderName { get; set; }
        public string IFSC
        { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string Branch { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public bool IsActive { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }

    }
}