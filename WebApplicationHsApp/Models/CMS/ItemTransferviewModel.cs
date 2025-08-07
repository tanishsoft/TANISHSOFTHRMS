using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class ItemTransferviewModel
    {
        public int ItemTransferId { get; set; }
        public Nullable<int> FromStoreId { get; set; }
        public Nullable<int> ToStoreId { get; set; }
        public String FromStoreName { get; set; }
        public string  ToStoreName { get; set; }
        public string ModeOfTransfer { get; set; }
        public Nullable<int> SenderEmpId { get; set; }
        public string SenderEmpName { get; set; }
        public Nullable<int> ReceiverEmpId { get; set; }
        public string ReceiverEmpName { get; set; }
        public string SenderAttachment { get; set; }
        public string ReceiverAttachment { get; set; }
        public string SenderNotes { get; set; }
        public string ReceiverNotes { get; set; }
        public string SentOnDate { get; set; }
        public string SentOnTime { get; set; }
        public string ReceivedOnDate { get; set; }
        public string ReceivedOnTime { get; set; }
        public Nullable<int> RequestedEmpId { get; set; }
        public string RequestedEmpName { get; set; }
        public string RequestedOn { get; set; }
        public string RequestorNotes { get; set; }
        public Nullable<bool> IsCanteen { get; set; }
        public Nullable<int> FromLocationId { get; set; }
        public Nullable<int> ToLocationId { get; set; }
        public string FromLocationName { get; set; }
        public string ToLocationName { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Savetype { get; set; }
    }
}