using System;
using System.Linq;
using ScheduleApplication.DataModel;

namespace ScheduleApplication.HelpDesk
{
    public class UpdateTicketAuto
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public string Updateticketevery15minutes()
        {
            string jobrunstauts = "Success";
            try
            {
                DateTime Dt = DateTime.Now.AddDays(-1);
                var ticekts = myapp.tbl_Task.Where(l => l.AssignStatus == "New" && l.CallDateTime >= Dt && l.TaskType == null && l.AssignDepartmentName == "IT").ToList();
                foreach (var v in ticekts)
                {
                    if (v.CallDateTime.Value.TimeOfDay.Hours > 07 && v.CallDateTime.Value.TimeOfDay.Hours < 21)
                    {
                        v.AssignId = 8182;
                        v.AssignName = "MOHD AHMAD ALI";
                        v.CallStartDateTime = DateTime.Now;
                        v.ModifiedOn = DateTime.Now;
                        v.AssignStatus = "In Progress";
                        v.CreatorStatus = "In Progress";
                        myapp.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                jobrunstauts = "Error" + ex.Message;
            }
            return jobrunstauts;
        }
    }
}
