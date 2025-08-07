using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class DoctorController : Controller
    {
        // GET: Doctor
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DoctorRegistration()
        {
            return View();
        }
        public ActionResult AjaxGetDoctorRegistration(JQueryDataTableParamModel param)
        {
           var Doctorlist = myapp.tbl_Doctor_RegistrationFrom.Where(l => l.DR_IsActive == true).ToList();
            var query = (from d in myapp.tbl_Doctor_RegistrationFrom

                         select d).ToList();
            IEnumerable<tbl_Doctor_RegistrationFrom> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.DR_Id.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                              (c.DR_FirstName+' '+c.DR_LastName) != null && (c.DR_FirstName + ' ' + c.DR_LastName).ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.DR_Qualification != null && c.DR_Qualification.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.DR_specialization != null && c.DR_specialization.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         select new object[] {
                            c.DR_Id,
                             (c.DR_FirstName+' '+c.DR_LastName),
                             c.DR_mobileNo,
                             c.DR_Email,
                             c.DR_DateOfBirth.ToString(),
                             c.DR_Gender,
                             c.DR_Qualification,
                             c.DR_specialization,
                             c.DR_Id
                           };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        static CultureInfo provider = CultureInfo.InvariantCulture;
        public static DateTime ConverDateStringtoDatetime(string date)
        {
            return DateTime.ParseExact(date, "dd-MM-yyyy", provider);
        }
        public JsonResult SaveDOctorRegistration(tbl_Doctor_RegistrationFromViewModel DR)
        {
            tbl_Doctor_RegistrationFrom doctor = new tbl_Doctor_RegistrationFrom();
            doctor.DR_FirstName = DR.DR_FirstName;
            doctor.DR_LastName = DR.DR_LastName;
            doctor.DR_BloodGroup = DR.DR_BloodGroup;
            doctor.DR_Email = DR.DR_Email;
            doctor.DR_Experience = DR.DR_Experience;
            doctor.DR_Gender = DR.DR_Gender;
            doctor.DR_mobileNo = DR.DR_mobileNo;
            doctor.DR_Nationality = DR.DR_Nationality;
            doctor.DR_Qualification = DR.DR_Qualification;
            doctor.DR_RegistrationNumber = DR.DR_RegistrationNumber;
            doctor.DR_RegistrationState = DR.DR_RegistrationState;
            doctor.DR_residence_city = DR.DR_residence_city;
            doctor.DR_residence_country = DR.DR_residence_country;
            doctor.DR_residence_fax = DR.DR_residence_fax;
            doctor.DR_residence_FlotNo = DR.DR_residence_FlotNo;
            doctor.DR_residence_locality = DR.DR_residence_locality;
            doctor.DR_residence_office = DR.DR_residence_office;
            doctor.DR_residence_PhoneNo = DR.DR_residence_PhoneNo;
            doctor.DR_residence_pincode = DR.DR_residence_pincode;
            doctor.DR_residence_RoadName = DR.DR_residence_RoadName;
            doctor.DR_residence_State = DR.DR_residence_State;
            doctor.DR_specialization = DR.DR_specialization;
            doctor.DR_Title = DR.DR_Title;
            if (DR.DR_DateOfBirth != null)
                doctor.DR_DateOfBirth =ConverDateStringtoDatetime(DR.DR_DateOfBirth);

            doctor.DR_CreatedOn = DateTime.Now;
            doctor.DR_IsActive = true;
            myapp.tbl_Doctor_RegistrationFrom.Add(doctor);
            myapp.SaveChanges();
            var result = " New Request Successfully Added";
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDoctorById(int DRId)
        {
            var Request = myapp.tbl_Doctor_RegistrationFrom.Where(X => X.DR_Id== DRId).SingleOrDefault();
            tbl_Doctor_RegistrationFromViewModel viewmode = new tbl_Doctor_RegistrationFromViewModel();
            viewmode.DR_Id = Request.DR_Id;
            viewmode.DR_Title = Request.DR_Title;
            viewmode.DR_FirstName = Request.DR_FirstName;
            viewmode.DR_LastName = Request.DR_LastName;
            viewmode.DR_BloodGroup = Request.DR_BloodGroup;
            viewmode.DR_DateOfBirth = Request.DR_DateOfBirth.Value.ToString("dd/MM/yyyy");
            viewmode.DR_Email = Request.DR_Email;
            viewmode.DR_Experience = Request.DR_Experience;
            viewmode.DR_Gender = Request.DR_Gender;
            viewmode.DR_mobileNo = Request.DR_mobileNo;
            viewmode.DR_Nationality = Request.DR_Nationality;
            viewmode.DR_Qualification = Request.DR_Qualification;
            viewmode.DR_RegistrationNumber = Request.DR_RegistrationNumber;
            viewmode.DR_RegistrationState = Request.DR_RegistrationState;
            viewmode.DR_residence_city = Request.DR_residence_city;
            viewmode.DR_residence_country = Request.DR_residence_country;
            viewmode.DR_residence_fax = Request.DR_residence_fax;
            viewmode.DR_residence_FlotNo = Request.DR_residence_FlotNo;
            viewmode.DR_residence_locality = Request.DR_residence_locality;
            viewmode.DR_residence_office = Request.DR_residence_office;
            viewmode.DR_residence_PhoneNo = Request.DR_residence_PhoneNo;
            viewmode.DR_residence_pincode = Request.DR_residence_pincode;
            viewmode.DR_residence_RoadName = Request.DR_residence_RoadName;
            viewmode.DR_residence_State = Request.DR_residence_State;
            viewmode.DR_specialization = Request.DR_specialization;
            return Json(viewmode, JsonRequestBehavior.AllowGet);
        }


        public JsonResult UpdateDoctorRegistration(tbl_Doctor_RegistrationFromViewModel DR1)
        {
            var status = false;
            tbl_Doctor_RegistrationFrom model = new tbl_Doctor_RegistrationFrom();
            
            model.DR_Title = DR1.DR_Title;
            model.DR_FirstName = DR1.DR_FirstName;
            model.DR_LastName = DR1.DR_LastName;
            model.DR_BloodGroup = DR1.DR_BloodGroup;
            model.DR_DateOfBirth= ConverDateStringtoDatetime(DR1.DR_DateOfBirth);
            model.DR_Email = DR1.DR_Email;
            model.DR_Experience = DR1.DR_Experience;
            model.DR_Gender = DR1.DR_Gender;
            model.DR_mobileNo = DR1.DR_mobileNo;
            model.DR_Nationality = DR1.DR_Nationality;
            model.DR_Qualification = DR1.DR_Qualification;
            model.DR_RegistrationNumber = DR1.DR_RegistrationNumber;
            model.DR_RegistrationState = DR1.DR_RegistrationState;
            model.DR_residence_city = DR1.DR_residence_city;
            model.DR_residence_country = DR1.DR_residence_country;
            model.DR_residence_fax = DR1.DR_residence_fax;
            model.DR_residence_FlotNo = DR1.DR_residence_FlotNo;
            model.DR_residence_locality = DR1.DR_residence_locality;
            model.DR_residence_office = DR1.DR_residence_office;
            model.DR_residence_PhoneNo = DR1.DR_residence_PhoneNo;
            model.DR_residence_pincode = DR1.DR_residence_pincode;
            model.DR_residence_RoadName = DR1.DR_residence_RoadName;
            model.DR_residence_State = DR1.DR_residence_State;
            model.DR_specialization = DR1.DR_specialization;
            var cat = myapp.tbl_Doctor_RegistrationFrom.Where(l => l.DR_Id == DR1.DR_Id).ToList();
            if (cat.Count > 0)
            {
                cat[0].DR_Title = model.DR_Title;
                cat[0].DR_FirstName = model.DR_FirstName;
                cat[0].DR_LastName = model.DR_LastName;
                cat[0].DR_BloodGroup = model.DR_BloodGroup;
                cat[0].DR_DateOfBirth = model.DR_DateOfBirth;
                cat[0].DR_Email = model.DR_Email;
                cat[0].DR_Experience = model.DR_Experience;
                cat[0].DR_Gender = model.DR_Gender;
                cat[0].DR_mobileNo = model.DR_mobileNo;
                cat[0].DR_Nationality = model.DR_Nationality;
                cat[0].DR_Qualification = model.DR_Qualification;
                cat[0].DR_RegistrationNumber = model.DR_RegistrationNumber;
                cat[0].DR_RegistrationState = model.DR_RegistrationState;
                cat[0].DR_residence_city = model.DR_residence_city;
                cat[0].DR_residence_country = model.DR_residence_country;
                cat[0].DR_residence_fax = model.DR_residence_fax;
                cat[0].DR_residence_FlotNo = model.DR_residence_FlotNo;
                cat[0].DR_residence_locality = model.DR_residence_locality;
                cat[0].DR_residence_office = model.DR_residence_office;
                cat[0].DR_residence_PhoneNo = model.DR_residence_PhoneNo;
                cat[0].DR_residence_pincode = model.DR_residence_pincode;
                cat[0].DR_residence_RoadName = model.DR_residence_RoadName;
                cat[0].DR_residence_State = model.DR_residence_State;
                cat[0].DR_specialization = model.DR_specialization;
                cat[0].DR_ModifiedOn = DateTime.Now;
                cat[0].DR_IsActive = true;
                myapp.SaveChanges();
                status = true;
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteDR(int DRSId)
        {
            var v = myapp.tbl_Doctor_RegistrationFrom.Where(a => a.DR_Id == DRSId).FirstOrDefault();
            if (v != null)
            {
                myapp.tbl_Doctor_RegistrationFrom.Remove(v);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}