using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class CounselingController : Controller
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Counseling
        public ActionResult Index()
        {
            List<tbl_User> list = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).ToList();
            if (list.Count > 0)
            {
                ViewBag.CurrentUser = list[0];
            }
            else { ViewBag.CurrentUser = new tbl_User(); }
            return View();
        }
        public ActionResult MyRequests()
        {
            return View();
        }
        public ActionResult EmpSchedule()
        {
            return View();
        }
        public ActionResult MyCounselings()
        {
            return View();
        }
        public ActionResult GetDoctors()
        {
            var result = myapp.tbl_User.Where(l => l.IsOffRollDoctor == true || l.IsOnRollDoctor == true).ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetDoctorSchedule(int doctorid, string Date)
        {
            DateTime Scdt = ProjectConvert.ConverDateStringtoDatetime(Date);
            var empscheduls = myapp.tbl_EmpSchedule.Where(l => l.EmpId == doctorid && l.DateOfSchedule != null).ToList();
            var result = empscheduls.Where(l => l.DateOfSchedule.Value.ToString("dd/MM/yyyy") == Scdt.ToString("dd/MM/yyyy")).ToList();
            var details = myapp.tbl_EmpCounseling.Where(l => l.IsActive==true && l.DoctorEmpId == doctorid).ToList();
            details = details.Where(l => l.DateOfSchedule.Value.ToString("dd/MM/yyyy") == Scdt.ToString("dd/MM/yyyy")).ToList();
            List<EmpScheduleTimeViewModel> models = new List<EmpScheduleTimeViewModel>();
            foreach (var res in result)
            {
                DateTime dt = DateTime.Parse(res.FromTime);
                DateTime dtto = DateTime.Parse(res.ToTime);
                while (dt <= dtto)
                {
                    EmpScheduleTimeViewModel model = new EmpScheduleTimeViewModel();
                    model.ScheduleId = res.ScheduleId;
                    model.StratTime = dt.ToString("HH:mm");
                    dt = dt.AddMinutes(res.SlotDuration.Value);
                    model.EndTime = dt.ToString("HH:mm");
                    var check = details.Where(l => l.SlotTime == model.StratTime + " - " + model.EndTime).Count();
                    if (check == 0)
                        models.Add(model);
                }
            }
            return Json(models, JsonRequestBehavior.AllowGet);
        }
        public ActionResult saveEmpSchedule(List<EmpScheduleViewModel> Emp, List<tbl_EmpScheduleBreak> Empbreak)
        {
            for (int i = 0; i < Emp.Count; i++)
            {
                tbl_EmpSchedule sch = new tbl_EmpSchedule();
                sch.EmpId = Emp[i].EmpId;
                sch.FromTime = Emp[i].FromTime;
                sch.IsActive = true;
                sch.ToTime = Emp[i].ToTime;
                sch.SlotDuration = Emp[i].SlotDuration;
                sch.DateOfSchedule = ProjectConvert.ConverDateStringtoDatetime(Emp[i].DateOfSchedule);
                sch.CreatedBy = User.Identity.Name;
                sch.CreatedOn = DateTime.Now;
                myapp.tbl_EmpSchedule.Add(sch);
                myapp.SaveChanges();
                //for(int j = 0; j < Empbreak.Count; j++)
                //{
                //    tbl_EmpScheduleBreak brk = new tbl_EmpScheduleBreak();
                //    brk.EmpId = Empbreak[i].EmpId;
                //    brk.FromTime = Empbreak[i].FromTime;
                //    brk.IsActive = Empbreak[i].IsActive;
                //    brk.ToTime = Empbreak[i].ToTime;
                //    brk.ScheduleId = sch.ScheduleId;
                //    myapp.tbl_EmpScheduleBreak.Add(brk);
                //    myapp.SaveChanges();
                //}
            }

            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult saveCounseling(EmployeeCounselingmodel Emp, List<tbl_EmpCounselingFamily> Fam)
        {
            tbl_EmpCounseling Con = new tbl_EmpCounseling();
            Con.DateOfSchedule = ProjectConvert.ConverDateStringtoDatetime(Emp.DateOfSchedule);
            Con.DoctorEmpId = Emp.DoctorEmpId;
            Con.EmpComments = Emp.EmpComments;
            Con.ScheduleId = Emp.ScheduleId;
            Con.EmpId = Emp.EmpId;
            Con.SlotTime = Emp.SlotTime;
            Con.OtherColumn = Emp.EmpMobile;
            Con.IsActive = true;
            Con.CreatedBy = User.Identity.Name;
            Con.CreatedOn = DateTime.Now;
            Con.CounselingType = Fam == null ? "Self" : "Family";
            myapp.tbl_EmpCounseling.Add(Con);
            myapp.SaveChanges();
            if (Fam != null)
            {
                for (int j = 0; j < Fam.Count; j++)
                {
                    tbl_EmpCounselingFamily brk = new tbl_EmpCounselingFamily();
                    brk.EmpId = Fam[j].EmpId;
                    brk.CounselingId = Con.CounselingId;
                    brk.IsActive = Fam[j].IsActive;
                    brk.Name = Fam[j].Name;
                    brk.Age = Fam[j].Age;
                    brk.Relation = Fam[j].Relation;
                    brk.CreatedBy = User.Identity.Name;
                    brk.CreatedOn = DateTime.Now;
                    myapp.tbl_EmpCounselingFamily.Add(brk);
                    myapp.SaveChanges();
                }
            }
            List<tbl_User> Userdetails = myapp.tbl_User.Where(u => u.EmpId == Emp.EmpId).ToList();
            List<tbl_User> doctorDetails = myapp.tbl_User.Where(u => u.EmpId == Emp.DoctorEmpId).ToList();
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel();
            EmailTeamplates emailtemp = new EmailTeamplates();
            mailmodel.fromemail = "Leave@hospitals.com";
            mailmodel.toemail = Userdetails[0].EmailId;

            //mailmodel.subject = "Thank you for requesting for counselling session.";
            string mailbody = "<p style='font-family:verdana'>Dear " + Userdetails[0].FirstName + ",";
            //mailbody += "<p>Thank you for requesting for counselling session  on Date : " + Emp.DateOfSchedule + " At Time: " + Emp.SlotTime + ".</p>";
            //mailbody += "<p>The respective doctor " + doctorDetails[0].FirstName + " " + doctorDetails[0].LastName + " will get in touch with you at the time of the slot you have chosen. In consideration of the time pressures on our doctors, you are requested to be on time for your counselling session.</p>";
            //mailmodel.body = mailbody;
            //mailmodel.fromname = "Counselling Session";
            //cm.SendEmail(mailmodel);


            if (doctorDetails[0].EmailId != null && doctorDetails[0].EmailId != "")
            {
                mailmodel.toemail = doctorDetails[0].EmailId;
                mailmodel.subject = Userdetails[0].FirstName + " has booked a counselling session.";
                mailbody = "<p style='font-family:verdana'>Dear Doctor,";
                mailbody += "<p>" + Userdetails[0].FirstName + " has booked a counselling session.</p>";
                mailbody += "<p>Dear Doctor, " + Userdetails[0].FirstName + " has booked a counselling session with you on " + Emp.DateOfSchedule + " " + Emp.SlotTime + ".please make your self available during the selected slot for the counselling . If any change please inform the Employee on " + Emp.EmpMobile + ".Thank you.</p>";
                mailmodel.body = mailbody;
                mailmodel.fromname = "Counselling Session";
                cm.SendEmail(mailmodel);
            }


            if (Emp.EmpMobile != null && Emp.EmpMobile != "")
            {
                var TMobile = Emp.EmpMobile;
                SendSms smodel = new SendSms();
                smodel.SendSmsToEmployee(TMobile, "Thank you for requesting for counselling session on Date : " + Emp.DateOfSchedule + " At Time: " + Emp.SlotTime + ". The respective doctor " + doctorDetails[0].FirstName + " " + doctorDetails[0].LastName + " will get in touch with you at the time of the slot you have chosen. In consideration of the time pressures on our doctors, you are requested to be on time for your counselling session");
            }
            if (doctorDetails[0].PhoneNumber != null && doctorDetails[0].PhoneNumber != "")
            {
                var TMobile = doctorDetails[0].PhoneNumber;
                SendSms smodel1 = new SendSms();
                smodel1.SendSmsToEmployee(TMobile, "Dear Doctor, " + Userdetails[0].FirstName + " has booked a counselling session  with you on " + Emp.DateOfSchedule + " " + Emp.SlotTime + ". please make your self available during the selected slot for the counselling . If any change please inform the Employee on " + Emp.EmpMobile + ".Thank you");
            }
            return Json("Thank you for requesting for counselling session.", JsonRequestBehavior.AllowGet);
        }

        public ActionResult AjaxGetMyCounselingdetails(JQueryDataTableParamModel param)
        {
            var activeusers = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            //List<tbl_User> list = activeusers.Where(u => u.CustomUserId == User.Identity.Name).ToList();
            var EMpid = int.Parse(User.Identity.Name);
            var query = myapp.tbl_EmpCounseling.Where(l=>l.IsActive==true).ToList();
            if (!User.IsInRole("Admin"))
            {
                query = query.Where(d => d.EmpId == EMpid || d.CreatedBy == User.Identity.Name).OrderByDescending(x => x.CounselingId).ToList();
            }

            IEnumerable<tbl_EmpCounseling> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.CounselingId.ToString().ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.DoctorEmpId != null && c.DoctorEmpId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.SlotTime != null && c.SlotTime.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.DateOfSchedule != null && c.DateOfSchedule.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }

            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);

            var result = from c in displayedCompanies
                         join d in activeusers on c.DoctorEmpId equals d.EmpId
                         join d1 in activeusers on c.EmpId equals d1.EmpId
                         select new object[] {
                             c.CounselingId.ToString(),
                             d1.LocationName,
                             d1.FirstName,
                             c.OtherColumn,
                             c.CounselingType,
                             d.FirstName,
                             c.DateOfSchedule.Value.ToString("dd/MM/yyyy"),
                             c.SlotTime,
                             c.EmpComments,
                             c.Observation,
                             c.CounselingId.ToString()
                           };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetmySchedule(string start, string end, int LocationId = 0)
        {
            DateTime dtfrom = ProjectConvert.ConverDateStringtoDatetime(start, "yyyy-MM-dd");
            DateTime dtto = ProjectConvert.ConverDateStringtoDatetime(end, "yyyy-MM-dd");
            var users = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            int empid = int.Parse(User.Identity.Name);
            var events = myapp.tbl_EmpCounseling.Where(e => e.IsActive == true && e.DoctorEmpId == empid && e.DateOfSchedule >= dtfrom && e.DateOfSchedule <= dtto).ToList();
            var myschedules = myapp.tbl_EmpSchedule.Where(l => l.EmpId == empid).ToList();
            var eventList = from e in events
                            join sc in myschedules on e.ScheduleId equals sc.ScheduleId
                            join usr in users on e.EmpId equals usr.EmpId
                            let dts = DateTime.Parse(e.SlotTime.Split('-')[0].Trim())
                            //let dte = DateTime.Parse(sc.ToTime)
                            select new
                            {
                                id = e.CounselingId,
                                title = usr.FirstName,
                                //  start = e.EventDate.Value.ToShortDateString() + " " + e.EventTime,
                                start = e.DateOfSchedule.Value.ToString("yyyy-MM-dd") + "T" + dts.ToString("HH:mm"),
                                end = e.DateOfSchedule.Value.ToString("yyyy-MM-dd") + "T" + dts.AddMinutes(sc.SlotDuration.Value).ToString("HH:mm"),
                                color = "teal",
                                allDay = false
                                //resources = Data.Entitylist.Single(en => en.CHlId == e.chlid.ToString()).Department
                            };
            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetFullBookingDetails(int id)
        {
            var details = myapp.tbl_EmpCounseling.Where(l => l.CounselingId == id).SingleOrDefault();
            EmployeeCounselingmodel model = new EmployeeCounselingmodel();
            model.CounselingFee = 0;
            model.CounselingId = details.CounselingId;
            model.CounselingType = details.CounselingType;
            model.CreatedBy = details.CreatedBy;
            model.CreatedOn = details.CreatedOn.Value.ToString("dd/MM/yyyy");
            model.DateOfSchedule = details.DateOfSchedule.Value.ToString("dd/MM/yyyy");
            model.DoctorEmpId = details.DoctorEmpId;
            model.DoctorEmpName = myapp.tbl_User.Where(l => l.EmpId == details.DoctorEmpId).SingleOrDefault().FirstName;
            model.EmpId = details.EmpId;
            var emp = myapp.tbl_User.Where(l => l.EmpId == details.EmpId).SingleOrDefault();
            model.EmpMobile = details.OtherColumn;
            model.EmpName = emp.FirstName;
            model.Observation = details.Observation;
            model.OtherColumn = details.OtherColumn;
            model.OtherColumn1 = details.OtherColumn1;
            model.ScheduleId = details.ScheduleId;
            model.SlotTime = details.SlotTime;
            model.families = myapp.tbl_EmpCounselingFamily.Where(l => l.CounselingId == details.CounselingId).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveCounselingObservation(CounselingObservationViewModel model)
        {
            var details = myapp.tbl_EmpCounseling.Where(l => l.CounselingId == model.CounselingId).SingleOrDefault();
            details.Observation = model.Observation;
            myapp.SaveChanges();
            foreach (var m in model.family)
            {
                var fde = myapp.tbl_EmpCounselingFamily.Where(l => l.Id == m.Id).SingleOrDefault();
                fde.Observation = m.Observation;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult CancelRequest(int id)
        {
            var details = myapp.tbl_EmpCounseling.Where(l => l.CounselingId == id).SingleOrDefault();
            details.IsActive = false;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}