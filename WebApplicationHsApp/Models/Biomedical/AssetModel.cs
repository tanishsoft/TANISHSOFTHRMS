using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class AssetModel
    {
        public int AssetId { get; set; }
        public Nullable<int> AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }
        public string Name { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string ModeOfProcurement { get; set; }
        public Nullable<int> VendorId { get; set; }
        public string VendorName { get; set; }
        public string Vendor { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> Quantity { get; set; }
        public string Warranty { get; set; }
        public Nullable<int> LicenceTrackerId { get; set; }
        public string PurchaseDate { get; set; }
        public string Remarks { get; set; }
        public string RefferenceDocument1 { get; set; }
        public string RefferenceDocument2 { get; set; }
        public string RefferenceDocument3 { get; set; }
        public string Impact { get; set; }
        public string SerialNumber { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string Insurance { get; set; }
        public string RegistrationNo { get; set; }
        public string Model { get; set; }
        public string ServiceSupportDetails { get; set; }
        public Nullable<int> BrandId { get; set; }
        public string PONumber { get; set; }
    }
}