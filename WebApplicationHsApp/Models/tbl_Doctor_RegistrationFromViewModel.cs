using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class tbl_Doctor_RegistrationFromViewModel
    {
        public long DR_Id { get; set; }
        public string DR_Title { get; set; }
        public string DR_FirstName { get; set; }
        public string DR_LastName { get; set; }
        public string DR_DateOfBirth { get; set; }
        public string DR_Gender { get; set; }
        public string DR_BloodGroup { get; set; }
        public string DR_Nationality { get; set; }
        public string DR_Languagespoken { get; set; }
        public string DR_mobileNo { get; set; }
        public string DR_Email { get; set; }
        public string DR_ImageUrl { get; set; }
        public string DR_Qualification { get; set; }
        public string DR_RegistrationNumber { get; set; }
        public string DR_RegistrationState { get; set; }
        public string DR_Experience { get; set; }
        public string DR_specialization { get; set; }
        public string DR_residence_FlotNo { get; set; }
        public string DR_residence_RoadName { get; set; }
        public string DR_residence_locality { get; set; }
        public string DR_residence_city { get; set; }
        public string DR_residence_State { get; set; }
        public string DR_residence_country { get; set; }
        public string DR_residence_PhoneNo { get; set; }
        public string DR_residence_office { get; set; }
        public string DR_residence_fax { get; set; }
        public string DR_residence_pincode { get; set; }
        public Nullable<bool> DR_IsActive { get; set; }
        public string DR_CreatedBy { get; set; }
        public Nullable<System.DateTime> DR_CreatedOn { get; set; }
        public string DR_ModifiedBy { get; set; }
        public Nullable<System.DateTime> DR_ModifiedOn { get; set; }
    }
}