using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebApplicationHsApp.Models;
using WebApplicationHsApp.DataModel;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using Microsoft.VisualBasic;
using System.Data.Entity;
using System.IO;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private static TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        HrDataManage hrdm = new HrDataManage();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        CustomModel cm = new CustomModel();
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //if (!model.UserId.ToLower().Contains("fh_") && model.UserId != "123456")
            //{
            //    model.UserId = "fh_" + model.UserId;
            //}
            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true

            var user = myapp.tbl_User.Where(l => l.IsActive == true && model.UserId == l.CustomUserId).SingleOrDefault();
            if (user != null)
            {
                var result = await SignInManager.PasswordSignInAsync(model.UserId, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:

                        AppUserDetails.SetCookie(model.UserId + AppCookieKey.LocationId, "1");
                        UpdateSystemLogs(model.UserId);
                        return RedirectToLocal(returnUrl);
                    case SignInStatus.LockedOut:
                        return View("Lockout");
                    case SignInStatus.RequiresVerification:
                        return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                    case SignInStatus.Failure:
                    default:
                        ModelState.AddModelError("", "Invalid login attempt.");
                        return View(model);
                }
            }
            else {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }
        }
        public bool UpdateSystemLogs(string loginid)
        {

            //var lgs = (from e in myapp.SystemLogs where e.LoginId == loginid select e).ToList();


            //if (lgs.Count == 0)
            //{
            //    SystemLog slObj = new SystemLog();
            //    slObj.LoginId = loginid;
            //    slObj.LoginType = "Employee";
            //    slObj.LogStatus = "Active";
            //    slObj.MLOStatus = "Active";
            //    slObj.OutTime = "-NA-";
            //    //slObj.InTime = DateTime.Now.ToShortTimeString();
            //    DateTime date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            //    slObj.InTime = date.ToShortTimeString();
            //    slObj.CreatedOn = (ConvertDateFormat(date.ToString())).ToString();
            //    slObj.CreatedBy = loginid;
            //    slObj.SysLogExtra1 = loginid;
            //    DateTime t1 = Convert.ToDateTime(Convert.ToDateTime(slObj.InTime).ToShortTimeString());
            //    DateTime t2 = Convert.ToDateTime(date.ToShortTimeString());
            //    TimeSpan ts = t2.Subtract(t1);

            //    slObj.DayWiseLogDuration = ts.ToString();
            //    Session["InTime"] = slObj.InTime;
            //    Session["LogDuration"] = slObj.DayWiseLogDuration;
            //    myapp.SystemLogs.Add(slObj);
            //    myapp.SaveChanges();

            //}
            //else
            //{
            //    //DateTime today = DateTime.Today;
            //    DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            //    DateTime Actualdate = ConvertDateFormat(today.ToString());
            //    var yesterday = Actualdate.AddDays(-1).ToString();
            //    var lg = (from l in lgs where l.CreatedOn == yesterday && l.MLOStatus == "Active" && l.LogStatus == "Active" select l).ToList();
            //    if (lg.Count > 0)
            //    {
            //        DateTime date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            //        lg[0].LogStatus = "InActive";

            //        SystemLog slObj = new SystemLog();
            //        slObj.LoginId = loginid;
            //        slObj.LoginType = loginid;
            //        slObj.LogStatus = "Active";
            //        slObj.MLOStatus = "Active";
            //        slObj.OutTime = "-NA-";
            //        slObj.SysLogExtra1 = loginid;
            //        // slObj.InTime = DateTime.Now.ToShortTimeString(); commented by pradeep on 20/08/2015  
            //        slObj.InTime = date.ToShortTimeString();
            //        slObj.CreatedOn = (ConvertDateFormat(date.ToString())).ToString();
            //        slObj.CreatedBy = loginid;
            //        DateTime t1 = Convert.ToDateTime(Convert.ToDateTime(slObj.InTime).ToShortTimeString());
            //        DateTime t2 = Convert.ToDateTime(date.ToShortTimeString());
            //        TimeSpan ts = t2.Subtract(t1);
            //        slObj.DayWiseLogDuration = ts.ToString();
            //        Session["InTime"] = slObj.InTime;
            //        Session["LogDuration"] = slObj.DayWiseLogDuration;
            //        myapp.SystemLogs.Add(slObj);
            //        myapp.SaveChanges();


            //    }
            //    else
            //    {

            //        var day = Actualdate.ToString();
            //        lg = (from l in lgs where l.CreatedOn == day select l).ToList();
            //        if (lg.Count > 0)
            //        {
            //            DateTime date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            //            DateTime t1 = Convert.ToDateTime(Convert.ToDateTime(lg[0].InTime).ToShortTimeString());
            //            DateTime t2 = Convert.ToDateTime(date.ToShortTimeString());
            //            TimeSpan ts = t2.Subtract(t1);
            //            lg[0].DayWiseLogDuration = ts.ToString();
            //            lg[0].LogStatus = "Active";
            //            lg[0].MLOStatus = "Active";
            //            lg[0].SysLogExtra1 = loginid;
            //            Session["InTime"] = lg[0].InTime;
            //            Session["LogDuration"] = lg[0].DayWiseLogDuration;
            //            myapp.SaveChanges();

            //        }
            //        else
            //        {
            //            SystemLog slObj = new SystemLog();
            //            slObj.LoginId = loginid;
            //            slObj.LoginType = "Employee";
            //            slObj.LogStatus = "Active";
            //            slObj.MLOStatus = "Active";
            //            slObj.OutTime = "-NA-";
            //            slObj.SysLogExtra1 = "";
            //            // slObj.InTime = DateTime.Now.ToShortTimeString(); commented by pradeep on 20/08/2015
            //            DateTime date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
            //            slObj.InTime = date.ToShortTimeString();
            //            slObj.CreatedOn = (ConvertDateFormat(date.ToString())).ToString();
            //            slObj.CreatedBy = loginid;
            //            DateTime t1 = Convert.ToDateTime(Convert.ToDateTime(slObj.InTime).ToShortTimeString());
            //            DateTime t2 = Convert.ToDateTime(date.ToShortTimeString());
            //            TimeSpan ts = t2.Subtract(t1);
            //            slObj.DayWiseLogDuration = ts.ToString();
            //            Session["InTime"] = slObj.InTime;
            //            Session["LogDuration"] = slObj.DayWiseLogDuration;
            //            myapp.SystemLogs.Add(slObj);
            //            myapp.SaveChanges();
            //        }
            //    }
            //}
            return true;
        }
        public DateTime ConvertDateFormat(string dat)
        {
            DateTime ReturnData = DateTime.Now;
            var Dt = dat.Split(' ')[0];
            DateTime date;
            if (DateTime.TryParseExact(Dt, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) == true)
            {
                ReturnData = DateTime.ParseExact(Dt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            else
            {
                var success = DateTime.TryParseExact(Dt, new[] { "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy", "M/d/yyyy", "dd-MM-yyyy", "d-MM-yyyy", "dd-M-yyyy", "d-M-yyyy", "MM-dd-yyyy", "M-dd-yyyy", "MM-d-yyyy", "M-d-yyyy", "yyyy-MM-dd", "yyyy-M-dd", "yyyy-MM-d", "yyyy-M-d", "yyyy-dd-MM", "yyyy-d-MM", "yyyy-dd-M", "yyyy-d-M" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                string SpecificDt = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                ReturnData = DateTime.ParseExact(SpecificDt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            return ReturnData;
        }
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Admin");
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/EmployeeRegister
        [AllowAnonymous]
        public ActionResult EmployeeRegister()
        {
            return View();
        }
        public JsonResult AssignRoleToemployee(string empid, string role)
        {
            string message = "Role Assigned successfully";
            try
            {
                int userid = int.Parse(empid);
                var item = myapp.tbl_User.Where(u => u.UserId == userid).ToList();
                if (item.Count > 0)
                {
                    if (item[0].CustomUserId != null && item[0].CustomUserId != "")
                    {
                        var data = UserManager.FindByName(item[0].CustomUserId);
                        if (data != null)
                        {
                            UserManager.AddToRole(data.Id, role);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteRoleToemployee(string empid, string role)
        {
            string message = "Role Unassigned successfully";
            try
            {
                int userid = int.Parse(empid);
                var item = myapp.tbl_User.Where(u => u.UserId == userid).ToList();
                if (item.Count > 0)
                {
                    if (item[0].CustomUserId != null && item[0].CustomUserId != "")
                    {
                        var data = UserManager.FindByName(item[0].CustomUserId);
                        if (data != null)
                        {
                            UserManager.RemoveFromRole(data.Id, role);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetEmployeeRoles(string empid)
        {
            int userid = int.Parse(empid);
            var item = myapp.tbl_User.Where(u => u.UserId == userid).ToList();
            if (item.Count > 0)
            {
                if (item[0].CustomUserId != null && item[0].CustomUserId != "")
                {
                    var data = UserManager.GetRoles(item[0].CustomUserId);
                    return Json(data, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("", JsonRequestBehavior.AllowGet);

        }
        public JsonResult DeleteEmployee(int empid, string DateOfLeaving, string Rmanager)
        {
            string message = "Success";
            try
            {
                var list = myapp.tbl_User.Where(l => l.UserId == empid).ToList();
                foreach (var item in list)
                {
                    if (item.CustomUserId != null && item.CustomUserId != "")
                    {
                        string CustomUserID = item.CustomUserId;
                        var AspNetUsers = myapp.AspNetUsers.Where(e => e.UserName == CustomUserID).ToList();
                        if (AspNetUsers.Count() > 0)
                        {
                            myapp.AspNetUsers.Remove(AspNetUsers[0]);
                            myapp.SaveChanges();
                        }

                        if (Rmanager != null && Rmanager != "")
                        {
                            //if (!Rmanager.ToLower().Contains("fh_"))
                            //{
                            //    Rmanager = "FH_" + Rmanager;
                            //}
                            var list2 = myapp.tbl_ReportingManager.Where(l => l.UserId == item.CustomUserId).ToList();
                            foreach (var l in list2)
                            {
                                l.UserId = Rmanager;
                                var list3 = myapp.tbl_User.Where(c => c.CustomUserId == Rmanager).ToList();
                                if (list3.Count > 0)
                                {
                                    l.UserName = list3[0].FirstName;
                                }
                                myapp.SaveChanges();
                            }

                        }
                    }
                    item.IsActive = false;
                    item.DateOfLeaving = ProjectConvert.ConverDateStringtoDatetime(DateOfLeaving);
                    myapp.SaveChanges();


                }



            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteOutsource(int empid, DateTime DateOfLeaving)
        {
            string message = "Success";
            try
            {
                var list = myapp.tbl_OutSourceUser.Where(l => l.UserId == empid).ToList();
                foreach (var item in list)
                {
                    if (item.CustomUserId != null && item.CustomUserId != "")
                    {
                        var data = UserManager.FindByName(item.CustomUserId);
                        if (data != null)
                        {
                            UserManager.SetLockoutEnabled(item.CustomUserId, true);
                        }
                    }
                    item.IsActive = false;
                    item.DateOfLeaving = DateOfLeaving;
                    myapp.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        //
        // POST: /Account/EmployeeRegister
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EmployeeRegister(RegisterEmployeeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserId, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    UserManager.AddToRole(user.Id, "Employee");
                    tbl_User tbluser = new tbl_User();
                    tbluser.FirstName = model.FirstName;
                    tbluser.LastName = model.LastName;
                    tbluser.CustomUserId = model.UserId;
                    tbluser.DateOfBirth = ProjectConvert.ConverDateStringtoDatetime(model.DateOfBirth);
                    tbluser.DateOfJoining = ProjectConvert.ConverDateStringtoDatetime(model.DateOfJoining);
                    tbluser.EmailId = model.Email;
                    tbluser.PhoneNumber = model.MobileNumber;
                    tbluser.Extenstion = model.ExtensionNumber;
                    tbluser.LocationName = model.LocationName;
                    tbluser.DepartmentName = model.DepartmentName;
                    tbluser.LocationId = model.LocationId;
                    tbluser.DepartmentId = model.DepartmentId;
                    tbluser.Designation = model.Designation;
                    tbluser.PlaceAllocation = model.PlaceAllocation;
                    tbluser.SecurityQuestion = model.SecurityQuetion;
                    tbluser.SecurityAnswner = model.SecurityAnswer;
                    tbluser.IsActive = true;
                    tbluser.CreatedBy = "Admin";
                    tbluser.CreatedOn = DateTime.Now;
                    myapp.tbl_User.Add(tbluser);
                    myapp.SaveChanges();
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }


        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            try
            {
                if (model.UserId != null && model.Email != null && model.Phone != null)
                {
                    var list = myapp.tbl_User.Where(e => e.CustomUserId == model.UserId).ToList();
                    if (list[0].EmailId == model.Email && list[0].PhoneNumber == model.Phone)
                    {
                        ViewBag.x = await UpdatePassword(model.UserId);
                        return View("ForgotPassword");
                    }

                }
                if (model.UserId != null && model.Email == null && model.Phone == null)
                {
                    ViewBag.x = await UpdatePassword(model.UserId);
                    return View("ForgotPassword");
                }
                ViewBag.UserValid = "InValidUser";
                // Don't reveal that the user does not exist or is not confirmed
                return View("ForgotPasswordConfirmation");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.UserId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }
        public async Task<ActionResult> UpdatePasswordFromAdminForVendor(int id, string Password)
        {
            string email = "";

            var useridlist = myapp.tbl_Vendor.Where(l => l.VendorId == id).ToList();
            if (useridlist.Count > 0)
            {
                email = useridlist[0].Email;
            }

            var user = await UserManager.FindByNameAsync(email);
            if (user == null)
            {
                var user2 = new ApplicationUser { UserName = email, Email = email };
                var result2 = await UserManager.CreateAsync(user2, Password);
                if (result2.Succeeded)
                {
                    UserManager.AddToRole(user2.Id, "Vendor");
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, code, Password);
            if (result.Succeeded)
            {
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Please try again password reset was failed", JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<ActionResult> UpdatePasswordFromAdmin(string UserId, string Password)
        {
            if (!UserId.ToLower().Contains("fh"))
            {
                int iuserid = int.Parse(UserId);
                var useridlist = myapp.tbl_User.Where(l => l.UserId == iuserid).ToList();
                if (useridlist.Count > 0)
                {
                    UserId = useridlist[0].CustomUserId;
                }
            }

            var user = await UserManager.FindByNameAsync(UserId);
            if (user == null)
            {
                return Json("In Valid User", JsonRequestBehavior.AllowGet);
            }
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, code, Password);
            if (result.Succeeded)
            {
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Please try again password reset was failed", JsonRequestBehavior.AllowGet);
            }
        }

        public async Task<ActionResult> UpdatePasswordFromAdmin1()
        {
            string results = "";

            var useridlist = myapp.tbl_User.Where(l => l.IsOffRollDoctor == true || l.IsOnRollDoctor == true).ToList();

            foreach (var user in useridlist)
            {
                var user1 = await UserManager.FindByNameAsync(user.CustomUserId);
                if (user1 == null)
                {
                    if (user.EmailId == null || user.EmailId == "")
                    {
                        user.EmailId = user.CustomUserId + "@fernandez.foundation";
                    }
                    var user3 = new ApplicationUser { UserName = user.CustomUserId, Email = user.EmailId };
                    var result = await UserManager.CreateAsync(user3, "fh@123");
                    if (result.Succeeded)
                    {
                        UserManager.AddToRole(user3.Id, "Doctor");
                        results = user.CustomUserId + "Success";
                    }
                    else
                    {
                        results = user.CustomUserId + "Error";
                    }
                }
                else
                {
                }
            }
            return Json("Please try again password reset was failed", JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> UpdatePasswordFromAdminOutSource(string UserId, string Password)
        {
            if (!UserId.ToLower().Contains("out"))
            {
                int iuserid = int.Parse(UserId);
                var useridlist = myapp.tbl_OutSourceUser.Where(l => l.UserId == iuserid).ToList();
                if (useridlist.Count > 0)
                {
                    UserId = useridlist[0].CustomUserId;
                }
            }

            var user = await UserManager.FindByNameAsync(UserId);
            if (user == null)
            {
                return Json("In Valid User", JsonRequestBehavior.AllowGet);
            }
            string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
            var result = await UserManager.ResetPasswordAsync(user.Id, code, Password);
            if (result.Succeeded)
            {
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("Please try again password reset was failed", JsonRequestBehavior.AllowGet);
            }
        }
        //Reset All employees passwords
        public async Task<ActionResult> ResetAllPasswords(ResetPasswordViewModel model)
        {
            var list = (from v in myapp.tbl_User where v.IsActive == true select v).ToList();
            var userslist = (from v in myapp.AspNetUsers where v.UserName != null select v).ToList();
            //var user = await UserManager.FindByNameAsync(model.UserId);
            if (list.Count > 0)
            {
                foreach (var v in userslist)
                {
                    if (v.UserName != null)
                    {
                        string code = await UserManager.GeneratePasswordResetTokenAsync(v.Id);
                        var result = await UserManager.ResetPasswordAsync(v.Id, code, model.Password);
                        if (result.Succeeded)
                        {
                            //return Json("Success", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            // return Json("Please try again password reset was failed", JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        //    return Json("Unable to do this process",JsonRequestBehavior.AllowGet);

                    }

                }
            }
            else { return Json("Please check", JsonRequestBehavior.AllowGet); }
            return View();
        }
        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //UpdateSystemLogLogoff();
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }
        public bool UpdateSystemLogLogoff()
        {
            string LoginId = User.Identity.Name;
            var lg = (from e in myapp.SystemLogs where e.LoginId == LoginId select e).ToList();
            if (lg.Count > 0)
            {
                DateTime today = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                DateTime Actualdate = ConvertDateFormat(today.ToString());
                var day = Actualdate.ToString();
                lg = (from l in lg where l.CreatedOn == day select l).ToList();
                if (lg.Count > 0)
                {
                    DateTime date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE);
                    DateTime t1 = Convert.ToDateTime(Convert.ToDateTime(lg[0].InTime).ToShortTimeString());
                    DateTime t2 = Convert.ToDateTime(date.ToShortTimeString());
                    TimeSpan ts = t2.Subtract(t1);
                    lg[0].DayWiseLogDuration = ts.ToString();
                    lg[0].OutTime = date.ToShortTimeString();
                    lg[0].MLOStatus = "InActive";
                    //lg[0].OutTime = DateTime.Now.ToShortTimeString(); commented by pradeep on 20/08/2015
                    lg[0].LogStatus = "InActive";
                    myapp.SaveChanges();
                }
            }
            return true;
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

        public JsonResult PermanentDeleteEmployee(string empid)
        {
            string message = "Success";
            try
            {
                var data = UserManager.FindByName(empid);
                if (data != null)
                    UserManager.Delete(data);
                var list = myapp.tbl_User.Where(l => l.CustomUserId == empid).ToList();
                if (list.Count() > 0)
                {
                    myapp.tbl_User.Remove(list[0]);
                    myapp.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);

        }

        public JsonResult ActiveEmployee(string empid)
        {
            string message = "Success";
            try
            {
                var data = UserManager.FindByName(empid);
                //  var su = UserManager.Delete(data);
                var list = myapp.tbl_User.Where(l => l.CustomUserId == empid).ToList();
                foreach (var item in list)
                {
                    item.IsActive = true;
                    myapp.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        public JsonResult CheckUserIdExitsOrNot(string UserId)
        {
            var list = myapp.tbl_User.Where(u => u.CustomUserId == UserId).ToList();
            if (list.Count > 0)
            {
                return Json(true, JsonRequestBehavior.AllowGet);

            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> AddEmployee(RegisterEmployeeViewModel model)
        {
            string Password = "", FileName = "";
            tbl_User tbluser = new tbl_User();
            if (model.ImageFile != null)
            {
                FileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                //To Get File Extension  
                string FileExtension = Path.GetExtension(model.ImageFile.FileName);
                //Add Current Date To Attached File Name  
                FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;
                //Get Upload path from Web.Config file AppSettings.  
                string UploadPath = Server.MapPath("~/Documents/Images/");
                //Its Create complete path to store in server.  
                model.ImagePath = UploadPath + FileName;
                //To copy and save file into server.  
                model.ImageFile.SaveAs(model.ImagePath);
            }
            if (model.ddlVendor != null && model.ddlVendor != "")
            {
                tbluser.VendorId = int.Parse(model.ddlVendor);
            }
            model.UserId = model.UserId.Trim();
            tbluser.EmployeePhoto = FileName;
            tbluser.aadharCard = model.AdhaarCard;
            tbluser.PanCard = model.PanCard;
            tbluser.FirstName = model.FirstName;
            model.LastName = "";
            tbluser.LastName = "";
            tbluser.CustomUserId = model.UserId.Trim();
            tbluser.DateOfBirth = ProjectConvert.ConverDateStringtoDatetime(model.DateOfBirth);
            tbluser.DateOfJoining = ProjectConvert.ConverDateStringtoDatetime(model.DateOfJoining);
            tbluser.EmailId = model.Email;
            tbluser.PhoneNumber = model.MobileNumber;
            tbluser.Extenstion = model.ExtensionNumber;
            tbluser.LocationName = model.LocationName;
            tbluser.DepartmentName = model.DepartmentName;
            tbluser.LocationId = model.LocationId;
            tbluser.Gender = model.Gender;
            tbluser.DepartmentId = model.DepartmentId;
            tbluser.DesignationID = Convert.ToInt64(model.DesignationID);
            tbluser.Designation = model.Designation;
            //tbluser.PlaceAllocation = model.PlaceAllocation;
            tbluser.SecurityQuestion = model.SecurityQuetion;
            tbluser.SecurityAnswner = model.SecurityAnswer;
            tbluser.IsActive = true;
            tbluser.CreatedBy = User.Identity.Name;
            tbluser.CreatedOn = DateTime.Now;
            if (model.SubDepartmnetId == null)
            {
                model.SubDepartmnetId = 0;
            }
            tbluser.SubDepartmentId = model.SubDepartmnetId;
            tbluser.SubDepartmentName = model.SubDepartmentName;
            tbluser.ReportingManagerId = model.ReportingManagerId;
            tbluser.UserType = model.UserType;
            tbluser.IsEmployeesReporting = false;
            tbluser.EmpId = int.Parse(model.UserId);
            var userslist = myapp.tbl_User.Where(l => l.CustomUserId == model.UserId).ToList();
            if (userslist.Count == 0)
            {
                if (model.IsLogin)
                {
                    tbluser.ChangePassword = true;
                }
                List<string> roleslist = new List<string>();
                switch (model.UserRole)
                {
                    case "Employee":
                        tbluser.IsEmployee = true;
                        tbluser.IsOffRollDoctor = false;
                        tbluser.IsOnRollDoctor = true;
                        roleslist.Add(model.UserRole);
                        break;
                    //case "OnrollDoctor":
                    //    tbluser.IsEmployee = false;
                    //    tbluser.IsOffRollDoctor = false;
                    //    tbluser.IsOnRollDoctor = true;
                    //    roleslist.Add("Doctor");
                    //    break;
                    case "Offroll":
                        tbluser.IsEmployee = false;
                        tbluser.IsOffRollDoctor = true;
                        tbluser.IsOnRollDoctor = false;
                        roleslist.Add("Doctor");
                        break;
                    case "Student":
                        tbluser.IsEmployee = false;
                        tbluser.IsOffRollDoctor = false;
                        tbluser.IsOnRollDoctor = false;
                        roleslist.Add("Student");
                        break;
                    case "Trainee":
                        tbluser.IsEmployee = false;
                        tbluser.IsOffRollDoctor = false;
                        tbluser.IsOnRollDoctor = false;
                        roleslist.Add("Trainee");
                        break;
                    case "OutSource":
                        tbluser.IsEmployee = false;
                        tbluser.IsOffRollDoctor = false;
                        tbluser.IsOnRollDoctor = false;
                        roleslist.Add("OutSource");
                        break;
                }
                tbluser.EmpId = int.Parse(tbluser.CustomUserId.ToLower().Replace("fh_", ""));
                myapp.tbl_User.Add(tbluser);
                try
                {
                    myapp.SaveChanges();
                }
                catch (Exception ex)
                {

                }

                // Add holidays
                var holidaylist = myapp.tbl_Holiday.Where(h => h.HolidayType == "National").ToList();
                if (holidaylist.Count > 0)
                {
                    foreach (var v in holidaylist)
                    {
                        tbl_Roaster rs = new tbl_Roaster();
                        rs.CreatedBy = User.Identity.Name;
                        rs.CreatedOn = DateAndTime.Now;
                        rs.DepartmentId = tbluser.DepartmentId;
                        rs.IsActive = true;
                        rs.LocationId = tbluser.LocationId;
                        rs.ShiftDate = v.HolidayDate;
                        rs.ShiftEndTime = "18:30";
                        rs.ShiftStartTime = "09:00";
                        rs.ShiftTypeName = "Holiday";
                        rs.ShiftTypeId = 3;
                        rs.UserId = tbluser.CustomUserId;
                        rs.UserName = tbluser.FirstName;
                        myapp.tbl_Roaster.Add(rs);
                        myapp.SaveChanges();
                    }
                }

                //if (model.IsLogin)
                {
                    Password = "fh" + Convert.ToString(Guid.NewGuid()).Replace("-", "").Substring(0, 6);
                    if (model.Email == null || model.Email == "")
                    {
                        model.Email = model.UserId + "@fernandez.foundation";
                    }
                    var user = new ApplicationUser { UserName = model.UserId, Email = model.Email };
                    var result = await UserManager.CreateAsync(user, Password);
                    if (result.Succeeded)
                    {
                        if (roleslist.Count() == 1)
                        {
                            if (model.UserRole == "Offroll")
                            {
                                UserManager.AddToRole(user.Id, "Doctor");
                            }
                            else
                            {
                                UserManager.AddToRole(user.Id, model.UserRole);
                            }
                        }
                        else
                        {
                            UserManager.AddToRoles(user.Id, roleslist.ToArray());
                        }
                    }
                }
                //JavaScriptSerializer js = new JavaScriptSerializer();
                //if (model.strtbl_ReportingManager != null && model.strtbl_ReportingManager != "")
                //{
                //    model.tbl_ReportingManager = js.Deserialize<List<tbl_ReportingManager>>(model.strtbl_ReportingManager);
                //}
                //if (model.tbl_ReportingManager.Any())
                //{
                //    foreach (var item in model.tbl_ReportingManager)
                //    {
                //        var itm = new tbl_ReportingManager();
                //        itm.LocationId = item.LocationId;
                //        itm.LocationName = item.LocationName;
                //        itm.DepartmentId = item.DepartmentId;
                //        itm.DepartmentName = item.DepartmentName;
                //        itm.SubDepartmentId = item.SubDepartmentId;
                //        itm.SubDepartmentName = item.SubDepartmentName;
                //        itm.UserId = tbluser.CustomUserId;
                //        itm.UserName = tbluser.FirstName + " " + tbluser.LastName;
                //        itm.IsHod = item.IsHod;
                //        itm.IsHodOfHod = item.IsHodOfHod;
                //        itm.IsActive = true;
                //        myapp.tbl_ReportingManager.Add(itm);
                //        myapp.SaveChanges();
                //    }
                //}
                if (model.UserRole == "Employee" || model.UserRole == "OnrollDoctor")
                {
                    if (tbluser.DateOfJoining.Value.Day < 15 && tbluser.DateOfJoining.Value.Month == DateTime.Now.Month && tbluser.DateOfJoining.Value.Year == DateTime.Now.Year)
                    {
                        tbl_ManageLeave tblml = new tbl_ManageLeave();
                        tblml.AvailableLeave = 1;
                        tblml.CountOfLeave = 1;
                        tblml.CreatedBy = "Admin";
                        tblml.CreatedOn = DateTime.Now;
                        tblml.DepartmentId = model.DepartmentId;
                        tblml.DepartmentName = model.DepartmentName;
                        tblml.ExpireDate = DateTime.Now.AddDays(31);
                        tblml.IsActive = true;
                        tblml.LeaveTypeId = 1;
                        tblml.LeaveTypeName = "Casuval Leave";
                        tblml.LocationId = model.LocationId;
                        tblml.LocationName = model.LocationName;
                        tblml.ModifiedBy = "Admin";
                        tblml.ModifiedOn = DateTime.Now;
                        tblml.UserId = model.UserId;
                        tblml.UserName = model.FirstName + " " + model.LastName;
                        myapp.tbl_ManageLeave.Add(tblml);
                        LogLeavesHistory(1, "1", "Casuval Leave Added For the " + model.FirstName + " " + model.LastName + " on " + DateTime.Now.ToString(), model.UserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
                        tbl_ManageLeave tblml1 = new tbl_ManageLeave();
                        tblml1.AvailableLeave = 1;
                        tblml1.CountOfLeave = 1;
                        tblml1.CreatedBy = "Admin";
                        tblml1.CreatedOn = DateTime.Now;
                        tblml1.DepartmentId = model.DepartmentId;
                        tblml1.DepartmentName = model.DepartmentName;
                        tblml1.ExpireDate = DateTime.Now.AddDays(31);
                        tblml1.IsActive = true;
                        tblml1.LeaveTypeId = 4;
                        tblml1.LeaveTypeName = "Sick Leave";
                        tblml1.LocationId = model.LocationId;
                        tblml1.LocationName = model.LocationName;
                        tblml1.ModifiedBy = "Admin";
                        tblml1.ModifiedOn = DateTime.Now;
                        tblml1.UserId = model.UserId;
                        tblml1.UserName = model.FirstName + " " + model.LastName;
                        myapp.tbl_ManageLeave.Add(tblml1);
                        myapp.SaveChanges();
                        LogLeavesHistory(1, "4", "Sick Leave Added For the " + model.FirstName + " " + model.LastName + " on " + DateTime.Now.ToString(), model.UserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
                    }
                }
                else
                {
                    tbl_ManageLeave tblml = new tbl_ManageLeave();
                    tblml.AvailableLeave = 33;
                    tblml.CountOfLeave = 33;
                    tblml.CreatedBy = "Admin";
                    tblml.CreatedOn = DateTime.Now;
                    tblml.DepartmentId = model.DepartmentId;
                    tblml.DepartmentName = model.DepartmentName;
                    tblml.ExpireDate = DateTime.Now.AddYears(1);
                    tblml.IsActive = true;
                    tblml.LeaveTypeId = 1;
                    tblml.LeaveTypeName = "Casuval Leave";
                    tblml.LocationId = model.LocationId;
                    tblml.LocationName = model.LocationName;
                    tblml.ModifiedBy = "Admin";
                    tblml.ModifiedOn = DateTime.Now;
                    tblml.UserId = model.UserId;
                    tblml.UserName = model.FirstName + " " + model.LastName;
                    myapp.tbl_ManageLeave.Add(tblml);
                    LogLeavesHistory(33, "1", "Casuval Leave Added For the " + model.FirstName + " " + model.LastName + " on " + DateTime.Now.ToString(), model.UserId, DateTime.Now.Year, DateTime.Now.Month, false, true);
                }
                var rpt = model.ReportingManagerId.ToString();

                if (rpt != null && rpt != "")
                {
                    var Managerinfo = myapp.tbl_User.FirstOrDefault(a => a.CustomUserId == rpt);
                    if (Managerinfo != null && Managerinfo.EmailId != null && Managerinfo.EmailId != "")
                    {
                        CustomModel cm = new CustomModel();
                        MailModel mailmodel = new MailModel();
                        EmailTeamplates emailtemp = new EmailTeamplates();
                        mailmodel.fromemail = "Leave@hospitals.com";
                        mailmodel.toemail = Managerinfo.EmailId;
                        mailmodel.subject = "New Employee " + tbluser.FirstName + " " + tbluser.LastName + " joined";
                        mailmodel.body = emailtemp.NewEmployeeBodyTemplate(tbluser.FirstName + " " + tbluser.LastName, tbluser.CustomUserId.Replace("FH_", ""), tbluser.CustomUserId, Password, Managerinfo.FirstName + " " + Managerinfo.LastName, tbluser.Designation);
                        mailmodel.filepath = "";
                        mailmodel.username = Managerinfo.FirstName + " " + Managerinfo.LastName;
                        mailmodel.fromname = "Employee Joined Notification";
                        mailmodel.ccemail = tbluser.EmailId;
                        cm.SendEmail(mailmodel);
                    }
                }
                if (User.IsInRole("Vendor"))
                {
                    return RedirectToAction("VendorManageEmployees", "HrAdmin");
                }
                else
                {
                    return RedirectToAction("ManageEmployees", "HrAdmin");
                }

            }
            else
            {
                ModelState.AddModelError("error", "User Id Already exists");
                return View();
            }

        }

        [HttpPost]
        public async Task<ActionResult> AddOutSourceUser(RegisterEmployeeViewModel model)
        {
            string Password = "";
            tbl_OutSourceUser tbluser = new tbl_OutSourceUser();
            tbluser.FirstName = model.FirstName;
            model.LastName = "";
            tbluser.LastName = "";
            tbluser.CustomUserId = model.UserId;
            tbluser.DateOfBirth = ProjectConvert.ConverDateStringtoDatetime(model.DateOfBirth);
            tbluser.DateOfJoining = ProjectConvert.ConverDateStringtoDatetime(model.DateOfJoining);
            tbluser.EmailId = model.Email;
            tbluser.PhoneNumber = model.MobileNumber;
            tbluser.Extenstion = model.ExtensionNumber;
            tbluser.LocationName = model.LocationName;
            tbluser.DepartmentName = model.DepartmentName;
            tbluser.LocationId = model.LocationId;
            tbluser.Gender = model.Gender;
            tbluser.DepartmentId = model.DepartmentId;
            tbluser.DesignationID = Convert.ToInt64(model.DesignationID);
            tbluser.Designation = model.Designation;
            //tbluser.PlaceAllocation = model.PlaceAllocation;
            tbluser.SecurityQuestion = model.SecurityQuetion;
            tbluser.SecurityAnswner = model.SecurityAnswer;
            tbluser.IsActive = true;
            tbluser.CreatedBy = User.Identity.Name;
            tbluser.CreatedOn = DateTime.Now;
            tbluser.SubDepartmentId = model.SubDepartmnetId;
            tbluser.SubDepartmentName = model.SubDepartmentName;
            tbluser.UserType = model.UserType;
            tbluser.Address1 = model.Address1;
            tbluser.Address2 = model.Address2;
            tbluser.City = model.City;
            tbluser.State = model.State;
            tbluser.Pincode = model.Pincode;
            tbluser.Country = "India";
            tbluser.VendorId = model.ddlVendor;
            if (model.IsLogin)
            {
                tbluser.ChangePassword = true;
            }
            tbluser.EmpId = model.EmpId;
            myapp.tbl_OutSourceUser.Add(tbluser);
            myapp.SaveChanges();

            if (model.IsLogin)
            {
                Password = "fh" + Convert.ToString(Guid.NewGuid()).Replace("-", "").Substring(0, 6);
                if (model.Email == null || model.Email == "")
                {
                    model.Email = model.UserId + "@fernandez.foundation";
                }
                var user = new ApplicationUser { UserName = model.UserId, Email = model.Email };
                var result = await UserManager.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    List<string> roleslist = new List<string>();
                    roleslist.Add(model.UserRole);
                    UserManager.AddToRole(user.Id, model.UserRole);
                }
            }


            return RedirectToAction("ManageOutSource", "Admin");
        }
        public void LogLeavesHistory(int AddedLeaves, string LeaveType, string Remarks, string CustomUserId, int year, int month, bool Isyearly, bool Ismonthly)
        {
            tbl_LeaveUpdateHistory luph = new tbl_LeaveUpdateHistory();
            luph.AddedLeaves = AddedLeaves;
            luph.Created = DateTime.Now;
            luph.LeaveType = LeaveType;
            luph.Remarks = Remarks;
            luph.UserId = CustomUserId;
            luph.Year = year;
            luph.Month = month;
            luph.IsYearly = Isyearly;
            luph.IsMonthly = Ismonthly;
            myapp.tbl_LeaveUpdateHistory.Add(luph);
            myapp.SaveChanges();
        }
        public async Task<JsonResult> CreateLoginForUserByUserID(int UserID)
        {
            string ReturnMsg = "Error Occured While Creating User Login";
            if (Information.IsNumeric(UserID))
            {
                List<tbl_User> UserList = await myapp.tbl_User.Where(e => e.UserId == UserID).ToListAsync();
                if (UserList.Count() > 0)
                {
                    string CustomUserID = UserList[0].CustomUserId;
                    var AspNetUsers = await myapp.AspNetUsers.Where(e => e.UserName == CustomUserID).ToListAsync();
                    if (AspNetUsers.Count() == 0)
                    {
                        UserList[0].ChangePassword = true;
                        string Password = Convert.ToString(Guid.NewGuid()).Replace("-", "").Substring(0, 6);
                        if (UserList[0].EmailId == null || UserList[0].EmailId == "")
                        {
                            UserList[0].EmailId = CustomUserID + "@fernandez.foundation";
                        }
                        var user = new ApplicationUser { UserName = UserList[0].CustomUserId, Email = UserList[0].EmailId };
                        var result = await UserManager.CreateAsync(user, Password);
                        if (result.Succeeded)
                        {
                            if (UserList[0].IsEmployee.Value)
                            {
                                UserManager.AddToRole(user.Id, "Employee");
                            }
                            else
                            {
                                UserManager.AddToRole(user.Id, "Doctor");
                            }
                            ReturnMsg = "User Login Created Success";
                            await myapp.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        ReturnMsg = "Login Is Already Created For This User";
                    }
                }
            }
            return Json(ReturnMsg, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> CreateLoginForOutsourceByUserID(int UserID)
        {
            string ReturnMsg = "Error Occured While Creating User Login";
            if (Information.IsNumeric(UserID))
            {
                List<tbl_OutSourceUser> UserList = await myapp.tbl_OutSourceUser.Where(e => e.UserId == UserID).ToListAsync();
                if (UserList.Count() > 0)
                {
                    string CustomUserID = UserList[0].CustomUserId;
                    var AspNetUsers = await myapp.AspNetUsers.Where(e => e.UserName == CustomUserID).ToListAsync();
                    if (AspNetUsers.Count() == 0)
                    {
                        UserList[0].ChangePassword = true;
                        string Password = Convert.ToString(Guid.NewGuid()).Replace("-", "").Substring(0, 6);
                        if (UserList[0].EmailId == null || UserList[0].EmailId == "")
                        {
                            UserList[0].EmailId = CustomUserID + "@fernandez.foundation";
                        }
                        var user = new ApplicationUser { UserName = UserList[0].CustomUserId, Email = UserList[0].EmailId };
                        var result = await UserManager.CreateAsync(user, Password);
                        if (result.Succeeded)
                        {
                            UserManager.AddToRole(user.Id, "Employee");
                            ReturnMsg = "User Login Created Success";
                            await myapp.SaveChangesAsync();
                        }
                    }
                    else
                    {
                        ReturnMsg = "Login Is Already Created For This User";
                    }
                }
            }
            return Json(ReturnMsg, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> RemoveLoginForUserByUserID(int UserID)
        {
            string ReturnMsg = "Error Occured While Remove User Login";
            if (Information.IsNumeric(UserID))
            {
                List<tbl_User> UserList = await myapp.tbl_User.Where(e => e.UserId == UserID).ToListAsync();
                if (UserList.Count() > 0)
                {
                    string CustomUserID = UserList[0].CustomUserId;
                    var AspNetUsers = await myapp.AspNetUsers.Where(e => e.UserName == CustomUserID).ToListAsync();
                    if (AspNetUsers.Count() > 0)
                    {
                        myapp.AspNetUsers.Remove(AspNetUsers[0]);
                        myapp.SaveChanges();
                        ReturnMsg = "User Login Disbaled Successfully";
                    }
                    else
                    {
                        ReturnMsg = "No Login For This User";
                    }
                }
            }
            return Json(ReturnMsg, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> RemoveLoginForOutSourceUserByUserID(int UserID)
        {
            string ReturnMsg = "Error Occured While Remove User Login";
            if (Information.IsNumeric(UserID))
            {
                List<tbl_OutSourceUser> UserList = await myapp.tbl_OutSourceUser.Where(e => e.UserId == UserID).ToListAsync();
                if (UserList.Count() > 0)
                {
                    string CustomUserID = UserList[0].CustomUserId;
                    var AspNetUsers = await myapp.AspNetUsers.Where(e => e.UserName == CustomUserID).ToListAsync();
                    if (AspNetUsers.Count() > 0)
                    {
                        myapp.AspNetUsers.Remove(AspNetUsers[0]);
                        myapp.SaveChanges();
                        ReturnMsg = "User Login Disbaled Successfully";
                    }
                    else
                    {
                        ReturnMsg = "No Login For This User";
                    }
                }
            }
            return Json(ReturnMsg, JsonRequestBehavior.AllowGet);
        }
        public string GetReportingMgr(string userid)
        {
            string rptmgrId = hrdm.GetReportingMgr(userid, DateTime.Now, DateTime.Now);
            return rptmgrId;
        }
        [AllowAnonymous]
        public ActionResult LeaveApproval(int id)
        {
            var model = new LeaveViewModels();
            var tbll = myapp.tbl_Leave.FirstOrDefault(t => t.LeaveId == id);
            if (tbll != null)
            {
                model.LeaveId = tbll.LeaveId.ToString();
                model.LeaveStatus = tbll.LeaveStatus;
            }
            return View(model);
        }
        [HttpPost]
        [AllowAnonymous]
        public JsonResult LeaveApprovalSave(LeaveViewModels task)
        {
            var id = Convert.ToInt16(task.LeaveId);
            var tbll = myapp.tbl_Leave.FirstOrDefault(t => t.LeaveId == id);
            if (tbll != null)
            {
                tbll.Level1ApproveComment = task.Level1ApproveComment;
                if (task.Status == 0)
                {
                    tbll.Level1Approved = true;
                    tbll.Level2Approved = true;
                    tbll.LeaveStatus = "Approved";
                }
                else if (task.Status == 1)
                {
                    tbll.Level1Approved = false;
                    tbll.Level2Approved = false;
                    tbll.LeaveStatus = "Rejected";

                }
                else
                {
                    tbll.Level1Approved = true;
                    tbll.Level2Approved = true;
                    tbll.LeaveStatus = "Approved";
                }
                myapp.SaveChanges();
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }


        public ActionResult UpdateUserProfile(tbl_User UserNewData)
        {
            string ReturnMsg = "Error Occured While Updating Profile Data";
            if (UserNewData != null)
            {
                var UsersList = myapp.tbl_User.Where(e => e.UserId == UserNewData.UserId).ToList();
                if (UsersList.Count > 0)
                {
                    string UserID = UsersList[0].CustomUserId;
                    var LoginUserList = myapp.AspNetUsers.Where(e => e.UserName == UserID).ToList();
                    UsersList[0].FirstName = UserNewData.FirstName;
                    UsersList[0].PhoneNumber = UserNewData.PhoneNumber;
                    UsersList[0].EmailId = UserNewData.EmailId;
                    UsersList[0].Gender = UserNewData.Gender;
                    if (LoginUserList.Count > 0)
                    {
                        LoginUserList[0].Email = UserNewData.EmailId;
                        LoginUserList[0].PhoneNumber = UserNewData.PhoneNumber;
                    }
                    myapp.SaveChanges();
                    ReturnMsg = "Profile Updated Successfully";
                }
            }
            TempData["UpdateMsg"] = ReturnMsg;
            return RedirectToAction("MyProfile", "Home");
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<string> UpdatePassword(string UserId)
        {
            try
            {
                string Password = "fh@abc" + UserId.ToLower().Replace("fh_", "");
                var user = await UserManager.FindByNameAsync(UserId);
                if (user == null)
                {
                    return "In Valid User";
                }
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var result = await UserManager.ResetPasswordAsync(user.Id, code, Password);
                if (result.Succeeded)
                {
                    var list = myapp.tbl_User.Where(e => e.CustomUserId == UserId).ToList();
                    if (list.Count > 0 && list[0].EmailId != null && list[0].EmailId != "")
                    {
                        list[0].ChangePassword = true;
                        myapp.SaveChanges();
                        MailModel mailmodel = new MailModel();
                        mailmodel.fromemail = "helpdesk@fernandez.foundation";
                        mailmodel.toemail = list[0].EmailId;
                        //mailmodel.ccemail = "ahmadali@fernandez.foundation";


                        mailmodel.subject = "Thank you for contacting us your password";
                        mailmodel.body = "Hi <br /><p>Thank you for contacting us your password is : " + Password + "</p>";
                        mailmodel.filepath = "";
                        mailmodel.username = "Tanishsoft Hrms jobs";
                        mailmodel.fromname = "Tanishsoft Hrms jobs";
                        cm.SendEmail(mailmodel);
                        SendSms sms = new SendSms();
                        if (list[0].PhoneNumber != null && list[0].PhoneNumber != "")
                        {
                            sms.SendSmsToEmployee(list[0].PhoneNumber, "Hi, your intranet password is : " + Password);
                        }
                    }
                    return "Thank you for contacting us your password sent to your registered email and mobile";
                }
                else
                {
                    return "Please try again password reset was failed";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public ActionResult GetUsersBasedonRole(string role)
        {
            List<UserHaveRoleAccessViewModel> list = myapp.Database.SqlQuery<UserHaveRoleAccessViewModel>("select usr.CustomUserId,usr.FirstName+' '+usr.LastName as Name, loc.LocationName, dept.DepartmentName from AspNetRoles ar  inner join AspNetUserRoles aru on aru.RoleId = ar.Id   inner join  AspNetUsers ausr on ausr.Id = aru.UserId  inner join tbl_User usr on usr.CustomUserId = ausr.UserName  inner join tbl_Location loc on loc.LocationId = usr.LocationId  inner join tbl_Department dept on dept.DepartmentId = usr.DepartmentId   where ar.Name = '" + role + "'").ToList();

            return Json(list, JsonRequestBehavior.AllowGet);
        }

    }
}


