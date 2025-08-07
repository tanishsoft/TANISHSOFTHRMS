using System;

namespace WebApplicationHsApp.Models
{
    public class UserAssetViewModel
    {
        public int UserAssetId { get; set; }
        public string UserId { get; set; }
        public Nullable<int> AssetId { get; set; }
        public string DateOfAssign { get; set; }
        public Nullable<System.DateTime> DTDateOfAssign { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> AllocationRequestId { get; set; }
        public string AllocationStatus { get; set; }
        public string AssetSerialNumber { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> DeAllocationRequestId { get; set; }
        public string DeAllocateDate { get; set; }
        public Nullable<System.DateTime> DTDeAllocateDate { get; set; }
        public string DeAllocateComments { get; set; }
        public string DeAllocateStatus { get; set; }
        public string AssetName { get; set; }
        public string Location { get; set; }
        public string Department { get; set; }
        public string Designation { get; set; }
        public string UserName { get; set; }
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public AssetModel AssetModel { get; set; }
        public Nullable<int> BuildingId { get; set; }
        public Nullable<int> FloorId { get; set; }
        public Nullable<int> RoomId { get; set; }
        public string BuildingName { get; set; }
        public string FloorName { get; set; }
        public string RoomName { get; set; }
        public string AssetAllocationType { get; set; }
    }
}