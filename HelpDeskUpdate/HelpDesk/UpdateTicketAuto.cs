using System;
using System.Linq;
using HelpDeskUpdate.DbModel;

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
                DateTime Dt = DateTime.Now.AddDays(-2);
                var ticekts = myapp.tbl_Task.Where(l => l.AssignStatus == "New" && l.CallDateTime >= Dt && (l.AssignDepartmentName.ToLower() == "it" || l.AssignDepartmentName.ToLower() == "information technology")).ToList();
                foreach (var v in ticekts)
                {
                    TimeSpan ts = DateTime.Now - v.CallDateTime.Value;

                    if (ts.TotalMinutes > 15)
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
