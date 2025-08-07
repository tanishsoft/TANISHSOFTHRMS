using System;
using System.Linq;
using HelpDeskUpdate.DbModel;

namespace ScheduleApplication.HelpDesk
{
    public class HelpDeskautoClose
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public string UpdateHelpdesk()
        {
            string jobrunstatus = "Success";
            try
            {
                var list = myapp.tbl_Task.Where(t => t.AssignStatus == "Done" && t.CreatorStatus != "Done" && t.ModifiedOn != null).ToList();
                if (list.Count > 0)
                {
                    foreach (var v in list)
                    {
                        TimeSpan diff = DateTime.Now - v.ModifiedOn.Value;
                        double hours = diff.TotalHours;
                        if (hours > 48)
                        {
                            v.CreatorStatus = "Done";
                            myapp.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                jobrunstatus = "Error " + ex.Message;
            }
            return jobrunstatus;
        }
    }
}
