using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class tbl_UserAssetViewModel
    {
        public long UserAsset_id { get; set; }
        public string User_id { get; set; }
        public Nullable<long> Asset_id { get; set; }
        public string Date_Assign { get; set; }
        public string UserAsset_Remarks { get; set; }
        public Nullable<bool> UserAsset_IsActive { get; set; }
        public Nullable<System.DateTime> UserAsset_CreatedOn { get; set; }
        public string UserAsset_CreatedBy { get; set; }
        public string UserAsset_ModifiedBy { get; set; }
        public Nullable<System.DateTime> UserAsset_ModifiedOn { get; set; }
        public Nullable<long> Parent_AssetId { get; set; }
        public string DeallocateDate { get; set; }
        public string DeallocateComments { get; set; }
    }
}