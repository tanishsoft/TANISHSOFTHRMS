using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models.Appointment
{
    public class ManageAppointment
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public List<AppointmentSlotViewModel> GetAppointmentSlots()
        {
            var list = (from n in myapp.tbl_AppointmentSlot
                        where n.IsActive == true
                        select new AppointmentSlotViewModel
                        {
                            AppointmentSlotId = n.AppointmentSlotId,
                            SlotName = n.SlotName
                        }).ToList();
            return list;
        }
        public List<AppointmentSlotViewModel> GetAvailableAppointmentSlots(string DateOfAppointment)
        {
            List<AppointmentSlotViewModel> modellist = new List<AppointmentSlotViewModel>();
            var list = (from n in myapp.tbl_AppointmentSlot
                        where n.IsActive == true
                        select new AppointmentSlotViewModel
                        {
                            AppointmentSlotId = n.AppointmentSlotId,
                            SlotName = n.SlotName
                        }).ToList();
            DateTime AppDate = ProjectConvert.ConverDateStringtoDatetime(DateOfAppointment);
            foreach (var v in list)
            {
                var applist = myapp.tbl_Appointments.Where(l => l.DateOfAppointment == AppDate && l.AppointmentSlotId == v.AppointmentSlotId).ToList();
                if (applist.Count == 0)
                {
                    modellist.Add(v);
                }
            }
            return modellist;
        }
        public List<tbl_Patient> GetPatientDetailsByMobileNumber(string mobile)
        {
            var list = myapp.tbl_Patient.Where(l => l.MobileNumber == mobile).ToList();
            return list;
        }
        public string SaveAppointment(AppointmentViewModel model, string CreatedBy)
        {
            try
            {
                int PatientId = 0;
                var list = myapp.tbl_Patient.Where(l => l.MobileNumber == model.MobileNumber).ToList();
                if (list.Count > 0)
                {
                    PatientId = list[0].PatientId;
                }
                else
                {
                    tbl_Patient pmodel = new tbl_Patient();
                    pmodel.Address = model.Address;
                    pmodel.Age = model.Age;
                    pmodel.CreatedBy = CreatedBy;
                    pmodel.CreatedOn = DateTime.Now;
                    pmodel.Email = model.Email;
                    pmodel.Gender = model.Gender;
                    pmodel.MobileNumber = model.MobileNumber;
                    pmodel.ModifiedBy = CreatedBy;
                    pmodel.ModifiedOn = DateTime.Now;
                    pmodel.Name = model.Name;
                    pmodel.ParentName1 = model.ParentName1;

                    pmodel.Remarks = model.Remarks;
                    pmodel.PatientRegisterType = model.PatientRegisterType;
                    myapp.tbl_Patient.Add(pmodel);
                    myapp.SaveChanges();
                    PatientId = pmodel.PatientId;
                }
                //Check Appointment

                DateTime AppDate = ProjectConvert.ConverDateStringtoDatetime(model.DateOfAppointment);
                var applist = myapp.tbl_Appointments.Where(l => l.DateOfAppointment == AppDate && model.AppointmentSlotId == l.AppointmentSlotId).ToList();
                if (applist.Count > 0)
                {
                    return "Appointment is already booked for this slot Date.";
                }
                else
                {

                    tbl_Appointments appmodel = new tbl_Appointments();
                    appmodel.AppointmentSlotId = model.AppointmentSlotId;
                    appmodel.BookingType = model.PatientRegisterType;
                    appmodel.CreatedBy = CreatedBy;
                    appmodel.CreatedOn = DateTime.Now;
                    appmodel.DateOfAppointment = AppDate;
                    appmodel.IsActive = true;
                    appmodel.ModifiedBy = CreatedBy;
                    appmodel.ModifiedOn = DateTime.Now;
                    appmodel.PatientId = PatientId;
                    appmodel.PatientNotes = model.PatientNotes;
                    appmodel.Remarsk = model.Remarks;
                    appmodel.StartTimeOfAppointmnet = DateTime.Now;
                    appmodel.EndTimeOfAppointment = DateTime.Now;
                    myapp.tbl_Appointments.Add(appmodel);
                    myapp.SaveChanges();
                }


                return "Success";
            }
            catch (Exception ex)
            {
                return "Error - " + ex.Message;
            }
        }
        public List<AppointmentViewModel> GetAllAppointments()
        {
            var listOfAppointmen = (from a in myapp.tbl_Appointments.ToList()
                                    join aslot in myapp.tbl_AppointmentSlot on a.AppointmentSlotId equals aslot.AppointmentSlotId
                                    join p in myapp.tbl_Patient on a.PatientId equals p.PatientId
                                    select new AppointmentViewModel
                                    {
                                        Address = p.Address,
                                        Age = p.Age,
                                        AppointmentSlotId = a.AppointmentSlotId.HasValue ? a.AppointmentSlotId.Value : 0,
                                        AppointmentTime = aslot.SlotName,
                                        Area = p.Area,
                                        DateOfAppointment = ProjectConvert.ConverDateTimeToString(a.DateOfAppointment.Value),
                                        Email = p.Email,
                                        Gender = p.Gender,
                                        MobileNumber = p.MobileNumber,
                                        Name = p.Name,
                                        PatientNotes = a.PatientNotes,
                                        Remarks = a.Remarsk,
                                        ParentName1 = p.ParentName1,
                                        PatientRegisterType = p.PatientRegisterType
                                    }
                                  ).ToList();
            return listOfAppointmen;
        }
        public List<AppointmentViewModel> GetAppointmentsByMobile(string mobilenumber)
        {
            int PatientId = 0;
            var list = myapp.tbl_Patient.Where(l => l.MobileNumber == mobilenumber).ToList();
            if (list.Count > 0)
            {
                PatientId = list[0].PatientId;
            }
            var listOfAppointmen = (from a in myapp.tbl_Appointments.ToList()
                                    join aslot in myapp.tbl_AppointmentSlot on a.AppointmentSlotId equals aslot.AppointmentSlotId
                                    join p in myapp.tbl_Patient on a.PatientId equals p.PatientId
                                    where a.PatientId == PatientId && p.PatientId == PatientId
                                    select new AppointmentViewModel
                                    {
                                        Address = p.Address,
                                        Age = p.Age,
                                        AppointmentSlotId = a.AppointmentSlotId.HasValue ? a.AppointmentSlotId.Value : 0,
                                        AppointmentTime = aslot.SlotName,
                                        Area = p.Area,
                                        DateOfAppointment = ProjectConvert.ConverDateTimeToString(a.DateOfAppointment.Value),
                                        Email = p.Email,
                                        Gender = p.Gender,
                                        MobileNumber = p.MobileNumber,
                                        Name = p.Name,
                                        PatientNotes = a.PatientNotes,
                                        Remarks = a.Remarsk,
                                        ParentName1 = p.ParentName1,
                                        PatientRegisterType = p.PatientRegisterType
                                    }
                                  ).ToList();
            return listOfAppointmen;
        }
    }
}