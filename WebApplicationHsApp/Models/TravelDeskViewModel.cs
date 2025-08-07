using System.Collections.Generic;

namespace WebApplicationHsApp.Models
{
    public class TravelDeskViewModel
    {
        public int TravelDeskId { get; set; }
        public int EmpId { get; set; }
        public string EmpName { get; set; }
        public string MailId { get; set; }
        public string Designation { get; set; }
        public string TravelCategory { get; set; }
        public string TravelSubCategory { get; set; }
        public string NameOfConference { get; set; }
        public string EmployeeCategory { get; set; }
        public string DateOfJourney { get; set; }
        public string PlaceFrom { get; set; }
        public string PlaceTo { get; set; }
        public string DateOfReturnJourney { get; set; }
        public string ReturnPlaceFrom { get; set; }
        public string ReturnPlaceTo { get; set; }
        public bool Accommodation { get; set; }
        public string SpecialNote { get; set; }
        public bool IsHODApproved { get; set; }
        public string Status { get; set; }
        public string AdminComments { get; set; }
        public string BookingDeatils { get; set; }
        public string HODName { get; set; }
        public int HODEmpid { get; set; }
        public string HODEMail { get; set; }
        public string CurrentState { get; set; }
        public int? CurrentUser { get; set; }
        public int? approverId { get; set; }
        public string Approverstatus { get; set; }
        public string ApproverLevel { get; set; }

        public string DateOfBirth { get; set; }
        public string Mobile { get; set; }
        public string Detailsoftheparticipation { get; set; }
        public string ModeOfTravel { get; set; }
        public string EventStartDate { get; set; }
        public string EventEndDate { get; set; }
        public string RegistrationNeeded { get; set; }
        public string IfRegistrationNeededDetails { get; set; }
        public string ReimbursementByConference { get; set; }
        public string IfReimbursementByConference { get; set; }
        public string SponsoredBy { get; set; }

        public string InvitedAs { get; set; }
        public string VenuDetails { get; set; }
        public string AccommodationNote { get; set; }
        public string PurposeOfTrip { get; set; }
        public string OnwardTime { get; set; }
        public string ReturnTime { get; set; }
        public string TSMCNO { get; set; }
        public string RequestDate { get; set; }
        public List<TravelDeskDocumentViewModel> Documents { get; set; }
        public List<travelApprovalViewModel> ApprovalList { get; set; }
    }
    public class TravelDeskDocumentViewModel
    {
        public int DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string DocumentUrl { get; set; }
    }
    public class travelApprovalViewModel
    {
        public int TravelDeskApproverId { get; set; }
        public string ApproverStatus { get; set; }
        public string ApproverComments { get; set; }
        public string TypeOfApprover { get; set; }
        public int TravelDeskId { get; set; }
        public int ApproverEmpId { get; set; }
        public string ApproverName { get; set; }
    }
    public static class TravelDeskContansts
    {
        public static string WorkFlow_State_Hod = "WithHod";
        public static string WorkFlow_State_Travel_Admin = "TravelAdmin";
        public static string WorkFlow_State_Travel_User = "TravelUser";
        public static string WorkFlow_State_Requestor = "Requestor";
        public static string WorkFlow_State_Completed = "Completed";
        public static string Travel_Admin = "700018";
        public static string Travel_Admin2 = "1539";
        
        public static string Travel_User = "700018";
    }
}