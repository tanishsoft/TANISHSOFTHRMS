using System;
using System.Linq;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class HelpDeskautoClose
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public void UpdateHelpdesk()
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
    }
}