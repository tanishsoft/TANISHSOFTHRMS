using ScheduleApplication.DataModel;
using System;
using System.Linq;

namespace ScheduleApplication.AttendanceUpdate
{
    public class UpdateAttendance
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        aeonhrmEntities attenapp = new aeonhrmEntities();
        public string UpdateAttendanceAndRoaster()
        {
            string jobrunstatus = "Success";
            DateTime dt = DateTime.Now;
            dt = dt.AddDays(-20);
            try
            {
                var mastershifttypes = myapp.tbl_ShiftType.ToList();
                var persmissionslist = myapp.tbl_Permission.ToList();
                var attenlist = attenapp.Database.SqlQuery<AttendanceModelNew>("SELECT DATEADD(SECOND,CONVERT(int,SUBSTRING(pch_time1,5,2)),DATEADD(MINUTE,CONVERT(int,SUBSTRING(pch_time1,3,2)),DATEADD(hour,CONVERT(int,SUBSTRING(pch_time1,1,2)),pch_date))) as att_date,[indexno] as employeeid FROM [dbo].[DataTable] where Pch_Date>'20200221'  order by pch_date").ToList();
                attenlist = attenlist.OrderBy(l => l.att_date).ToList();
                foreach (var a in attenlist)
                {
                    DateTime attdate = Convert.ToDateTime(a.att_date.ToString("yyyy-MM-dd"));
                    var tatten = myapp.tbl_att_master.Where(l => l.att_date == attdate && l.employeeid == a.employeeid).ToList();
                               
                    double totalminutes = 0;                   
                    if (tatten.Count > 0)
                    {
                        //tatten[0].att_date = a.att_date.Date;
                        //tatten[0].employeeid = a.employeeid;
                        //tatten[0].in_time = a.att_date.ToString("hh:mm tt");
                        //tatten[0].lastupdatedon = DateTime.Now;
                        tatten[0].out_time = a.att_date.ToString("HH:mm");
                        tatten[0].roasterlatein = Convert.ToDecimal(totalminutes);
                        myapp.SaveChanges();
                    }
                    else
                    {
                        tbl_att_master taten = new tbl_att_master();
                        taten.att_date = attdate;
                        taten.employeeid = a.employeeid;
                        taten.in_time = a.att_date.ToString("HH:mm");
                        taten.lastupdatedon = DateTime.Now;
                        //taten.out_time = a.att_date.ToString("hh:mm tt");
                        taten.roasterlatein = Convert.ToDecimal(totalminutes);
                        myapp.tbl_att_master.Add(taten);
                        myapp.SaveChanges();
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
