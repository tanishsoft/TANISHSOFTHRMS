namespace WebApplicationHsApp.Models.Biomedical
{
    public class AssetTransferModel
    {
        public int AssetTransferId { get; set; }
        public int AssetId { get; set; }
        public string AssetModel { get; set; }
        public string AssetNo { get; set; }
        public int FromLocationId { get; set; }
        public int FromDepartmentId { get; set; }
        public int FromSubDepartmentId { get; set; }

        public string FromLocation { get; set; }
        public string FromDepartment { get; set; }
        public string FromSubDepartment { get; set; }


        public int ToLocationId { get; set; }
        public int ToDepartmentId { get; set; }
        public int ToSubDepartmentId { get; set; }

        public string ToLocation { get; set; }
        public string ToDepartment { get; set; }
        public string ToSubDepartment { get; set; }

        public string Document { get; set; }
        public string ReasonForTranfer { get; set; }
        public bool IsActive { get; set; }
        public string CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
    }
}