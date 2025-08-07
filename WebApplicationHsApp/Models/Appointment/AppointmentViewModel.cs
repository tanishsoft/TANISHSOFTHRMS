namespace WebApplicationHsApp.Models.Appointment
{
    public class AppointmentViewModel
    {
        public string Name { get; set; }
        public string Age { get; set; }
        public string Gender { get; set; }
        public string ParentName1 { get; set; }
        public string ParentName2 { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Area { get; set; }
        public string MobileNumber { get; set; }
        public string PatientRegisterType { get; set; }
        public int AppointmentSlotId { get; set; }
        public string DateOfAppointment { get; set; }//DD/MM//YYYY
        public string AppointmentTime { get; set; }
        public string PatientNotes { get; set; }
        public string Remarks { get; set; }
    }
    public class AppointmentSlotViewModel
    {
        public int AppointmentSlotId { get; set; }
        public string SlotName { get; set; }
    }
}