using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel
    {
        public string Returl { get; set; }
    }

    public class SendCodeViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel
    {
        [Required]
        public string Provider { get; set; }
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }
        [Display(Name = "Remember this browser?")]
        public bool RememberBrowser { get; set; }
        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class LoginViewModel
    {
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "User Id")]
        public string UserId { get; set; }
        [Required]
        //[DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        //[DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
    public class RegisterEmployeeViewModel
    {
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "User Name")]
        public string FirstName { get; set; }
        //[Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "User Id")]
        public string UserId { get; set; }
        [Required]
        [Display(Name = "Employee Id")]
        public string EmployeeId { get; set; }
        [Required]
        [Display(Name = "Date Of Joining")]
        public string DateOfJoining { get; set; }
        [Required]
        [Display(Name = "Date Of Birth")]
        public string DateOfBirth { get; set; }
        [Required]
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        [Required]
        [Display(Name = "Location")]
        public string ddlLocation { get; set; }
        [Required]
        [Display(Name = "Department")]
        public string ddlDepartment { get; set; }
        [Display(Name = "Sub Department")]
        public string ddlSubDepartment { get; set; }
        public int LocationId { get; set; }
        public int DepartmentId { get; set; }
        public string LocationName { get; set; }
        public string DepartmentName { get; set; }
        public int SubDepartmnetId { get; set; }
        public string SubDepartmentName { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [Required]
        [Display(Name = "Designation")]
        public string DesignationID { get; set; }
        [Display(Name = "Work Location")]
        public string PlaceAllocation { get; set; }
        [Display(Name = "User Role")]
        public string UserRole { get; set; }
        [Display(Name = "Security Quetion")]
        public string SecurityQuetion { get; set; }
        [Display(Name = "Security Answer")]
        public string SecurityAnswer { get; set; }
        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }
        [Display(Name = "Extension Number")]
        public string ExtensionNumber { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        //[DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public bool IsLogin { get; set; }
        public string UserType { get; set; }
        public int ReportingManagerId { get; set; }
        public string ddlVendor { get; set; }
        public string strtbl_ReportingManager { get; set; }
        public List<tbl_ReportingManager> tbl_ReportingManager { get; set; }
        public string AdhaarCard { get; set; }
        public string PanCard { get; set; }
        //To change label title value  
        public string ImagePath { get; set; }
        public int EmpId { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public HttpPostedFileBase ImageFile { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required]
        //[EmailAddress]
        [Display(Name = "UserId")]
        public string UserId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        //[DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        //[DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        //[EmailAddress]
        [Display(Name = "User Id")]
        public string UserId { get; set; }
        [Display(Name = "User EMail Id")]
        public string Email { get; set; }
        [Display(Name = " User Phone Number")]
        public string Phone { get; set; }
    }
    public class UserHaveRoleAccessViewModel
    {
        public string CustomUserId { get; set; }
        public string Name { get; set; }
        public string LocationName { get; set; }
        public string DepartmentName { get; set; }
    }
}