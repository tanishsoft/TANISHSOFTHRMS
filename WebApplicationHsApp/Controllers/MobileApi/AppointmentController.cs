using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models.Appointment;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    //[Authorize]
    [RoutePrefix("api/Appointment")]
    public class AppointmentController : ApiController
    {
        ManageAppointment ManageApp = new ManageAppointment();
        [HttpGet]
        [Route("GetAppointmentSlots")]
        public List<AppointmentSlotViewModel> GetAppointmentSlots()
        {
            return ManageApp.GetAppointmentSlots();
        }
        [HttpGet]
        [Route("GetAvailableAppointmentSlots")]
        public List<AppointmentSlotViewModel> GetAvailableAppointmentSlots(string DateOfAppointment)
        {
            return ManageApp.GetAvailableAppointmentSlots(DateOfAppointment);
        }

        [HttpGet]
        [Route("GetPatientDetailsByMobileNumber")]
        public tbl_Patient GetPatientDetailsByMobileNumber(string Mobile)
        {
            var list = ManageApp.GetPatientDetailsByMobileNumber(Mobile);
            if (list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        [HttpGet]
        [Route("GetAllAppointments")]
        public List<AppointmentViewModel> GetAllAppointments()
        {
            return ManageApp.GetAllAppointments();
        }

        [HttpGet]
        [Route("GetAppointmentsByMobile")]
        public List<AppointmentViewModel> GetAppointmentsByMobile(string Mobile)
        {
            return ManageApp.GetAppointmentsByMobile(Mobile);
        }
        [HttpPost]
        [Route("SaveAppointment")]
        public string SaveAppointment(AppointmentViewModel model)
        {
            return ManageApp.SaveAppointment(model, User.Identity.Name);
        }
       
    }
}
