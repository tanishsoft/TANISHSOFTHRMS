using System;

namespace WebApplicationHsApp.Models
{
    public class BookCalenderModel
    {
        public long BookingId { get; set; }
        public string Titles { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string EventDescription { get; set; }
        public Nullable<int> LocationId { get; set; }
        public string LocationName { get; set; }
        public Nullable<int> Eventinvitesgroupid { get; set; }
        public string EventUnattend { get; set; }
        public string Eventdocument { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public string RoomExtension { get; set; }
        public Nullable<int> FloorId { get; set; }
        public string FloorName { get; set; }
        public int BuildingId { get; set; }
        public string BuildingName { get; set; }
        public string Chair { get; set; }
        public string Tables { get; set; }
        public string ITReq { get; set; }
        public string video { get; set; }
        public string Arrangemnts { get; set; }
        public string DepartLocation { get; set; }
        public string NameEmpl { get; set; }
        public string NameDepartment { get; set; }
        public string UGC_Extension { get; set; }
        public string EmailID { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
    }
}