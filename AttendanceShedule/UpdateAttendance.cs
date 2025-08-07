using AttendanceSchedule.CurrentDataModel;
using AttendanceSchedule.DataModel;
using System;
using System.Linq;

namespace AttendanceSchedule
{
    public class UpdateAttendance
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        private aeonhrmEntities attenapp = new aeonhrmEntities();
        public string UpdateAttendanceAndRoaster()
        {
            string jobrunstatus = "Success";
            DateTime dt = DateTime.Now;
            dt = dt.AddDays(-5);
            try
            {
                System.Collections.Generic.List<tbl_ShiftType> mastershifttypes = myapp.tbl_ShiftType.ToList();
                DateTime Date = DateTime.Now.AddDays(-10);
                System.Collections.Generic.List<tbl_Permission> persmissionslist = myapp.tbl_Permission.Where(l => l.PermissionDate >= Date).ToList();
                System.Collections.Generic.List<DataTable> attenlist = attenapp.DataTables.Where(l => l.Pch_Date >= dt).ToList();
                foreach (DataTable a in attenlist)
                {
                    try
                    {
                        string employeeid = a.indexno.Replace(" ", "");
                        string fhemployeeid = "FH_" + int.Parse(employeeid).ToString();
                        System.Collections.Generic.List<tbl_att_master> tatten = myapp.tbl_att_master.Where(l => l.att_date == a.Pch_Date && l.employeeid == employeeid).ToList();
                        System.Collections.Generic.List<tbl_Roaster> shifttypes = myapp.tbl_Roaster.Where(l => l.ShiftDate == a.Pch_Date && l.UserId == fhemployeeid).ToList();
                        string Pch_Time = a.Pch_Time;
                        
                         Pch_Time = Pch_Time.Insert(2, ":");
                        DateTime intimeOrOutime = Convert.ToDateTime(Pch_Time);

                        string in_time = "", out_time = "";
                        if (tatten.Count > 0)
                        {
                            in_time = tatten[0].in_time;
                            out_time = tatten[0].out_time;
                        }
                        if (intimeOrOutime.Hour <= 13)
                        {
                            in_time = Pch_Time;
                        }
                        else
                        {
                            out_time = Pch_Time;
                        }
                        //calculate late in 
                        double totalminutes = 0;
                        double Latein = 0, LateOut = 0;
                        if (in_time != null && in_time.Replace(" ", "") != "" && out_time != null && out_time.Replace(" ", "") != "")
                        {
                            if (shifttypes.Count > 0)
                            {
                                System.Collections.Generic.List<tbl_Permission> permission = persmissionslist.Where(l => l.PermissionDate == a.Pch_Date && l.UserId == fhemployeeid).ToList();
                                if (permission.Count == 0)
                                {
                                    int shiftid = shifttypes[0].ShiftTypeId.Value;
                                    string shiftstarttime = mastershifttypes.Where(l => l.ShiftTypeId == shiftid).SingleOrDefault().ShiftStartTime;
                                    DateTime intime = Convert.ToDateTime(in_time);
                                    DateTime shifttime = Convert.ToDateTime(shiftstarttime);
                                    totalminutes = intime.Subtract(shifttime).TotalMinutes;
                                    string ShiftEndTime = mastershifttypes.Where(l => l.ShiftTypeId == shiftid).SingleOrDefault().ShiftEndTime;
                                    Latein = intime.Subtract(shifttime).TotalMinutes;
                                    DateTime outtime = Convert.ToDateTime(out_time);
                                    DateTime shiftendtime = Convert.ToDateTime(ShiftEndTime);
                                    LateOut = outtime.Subtract(shiftendtime).TotalMinutes;
                                }
                                else
                                {
                                    bool validpermission = false;
                                    DateTime intime = Convert.ToDateTime(in_time);
                                    foreach (tbl_Permission per in permission)
                                    {
                                        DateTime perintime = Convert.ToDateTime(per.StartDate.Value.ToString("hh:mm"));
                                        DateTime perouttime = Convert.ToDateTime(per.EndDate.Value.ToString("hh:mm"));
                                        if (perintime <= intime && perouttime >= intime)
                                        {
                                            validpermission = true;
                                        }
                                    }
                                    if (!validpermission)
                                    {
                                        DateTime permissiontime = Convert.ToDateTime(in_time);
                                        int shiftid = shifttypes[0].ShiftTypeId.Value;
                                        string shiftstarttime = mastershifttypes.Where(l => l.ShiftTypeId == shiftid).SingleOrDefault().ShiftStartTime;
                                        DateTime shifttime = Convert.ToDateTime(shiftstarttime);
                                        totalminutes = intime.Subtract(shifttime).TotalMinutes;
                                        string ShiftEndTime = mastershifttypes.Where(l => l.ShiftTypeId == shiftid).SingleOrDefault().ShiftEndTime;
                                        Latein = intime.Subtract(shifttime).TotalMinutes;
                                        DateTime outtime = Convert.ToDateTime(out_time);
                                        DateTime shiftendtime = Convert.ToDateTime(ShiftEndTime);
                                        LateOut = outtime.Subtract(shiftendtime).TotalMinutes;
                                    }
                                }
                            }
                        }

                        if (tatten.Count > 0)
                        {
                            tatten[0].att_date = a.Pch_Date;
                            tatten[0].auth_earlyout = "";
                            tatten[0].auth_latein = "";
                            tatten[0].auth_ot = "";
                            tatten[0].cmp_cod = "";
                            tatten[0].co_ot = "";
                            tatten[0].earlyin = "";
                            tatten[0].earlyout = "";
                            tatten[0].empcod = "";
                            tatten[0].employeeid = employeeid;
                            tatten[0].extrahours = "";
                            tatten[0].holiday = "";
                            tatten[0].in_time = in_time;
                            tatten[0].in_time1 = "";
                            tatten[0].lastupdatedon = DateTime.Now;
                            tatten[0].latein = Latein.ToString();
                            tatten[0].lateout = LateOut.ToString();
                            tatten[0].loc_cod = "";
                            tatten[0].out_time = out_time;
                            tatten[0].out_time1 = "";
                            tatten[0].punchdate = a.recorddatetime;
                            tatten[0].remarks = a.remarks;
                            tatten[0].roasterlatein = Convert.ToDecimal(totalminutes);
                            tatten[0].shft_cod = "";
                            tatten[0].shft_type = "";
                            tatten[0].site_cod = "";
                            tatten[0].status = "";
                            tatten[0].transact = "";
                            myapp.SaveChanges();
                        }
                        else
                        {

                            tbl_att_master taten = new tbl_att_master
                            {
                                att_date = a.Pch_Date,
                                auth_earlyout = "",
                                auth_latein = "",
                                auth_ot = "",
                                cmp_cod = "",
                                co_ot = "",
                                earlyin = "",
                                earlyout = "",
                                empcod = "",
                                employeeid = employeeid,
                                extrahours = "",
                                holiday = "",
                                in_time = in_time.Replace(" ", ""),
                                in_time1 = "",
                                lastupdatedon = DateTime.Now,
                                latein = Latein.ToString(),
                                lateout = LateOut.ToString(),
                                loc_cod = "",
                                out_time = out_time.Replace(" ", ""),
                                out_time1 = "",
                                punchdate = a.recorddatetime,
                                remarks = a.remarks,
                                roasterlatein = Convert.ToDecimal(totalminutes),
                                shft_cod = "",
                                shft_type = "",
                                site_cod = "",
                                status = "",
                                transact = ""
                            };
                            myapp.tbl_att_master.Add(taten);
                            myapp.SaveChanges();
                        }
                    }
                    catch { }

                }

                //SendAnEmail cm = new SendAnEmail();
                //MailModel mailmodel = new MailModel();
                //mailmodel.fromemail = "helpdesk@fernandez.foundation";
                //mailmodel.toemail = "vamsi@microarctech.com";
                ////mailmodel.ccemail = "elroy@fernandez.foundation,vamsi@microarctech.com";

                //mailmodel.subject = "Successfully runing attendance Job";
                //mailmodel.body = "Hi <br /><p>Job running successfully</p>";
                //mailmodel.filepath = "";
                //mailmodel.username = "Help Desk";
                //mailmodel.fromname = "Help Desk";
                //cm.SendEmail(mailmodel);
            }
            catch (Exception ex)
            {
                //SendAnEmail cm = new SendAnEmail();
                //MailModel mailmodel = new MailModel();
                //mailmodel.fromemail = "helpdesk@fernandez.foundation";
                //mailmodel.toemail = "vamsi@microarctech.com";
                ////mailmodel.ccemail = "elroy@fernandez.foundation,vamsi@microarctech.com";

                //mailmodel.subject = "Error While runing attendance Job";
                //mailmodel.body = "Hi <br /><p>" + ex + "</p>";
                //mailmodel.filepath = "";
                //mailmodel.username = "Help Desk";
                //mailmodel.fromname = "Help Desk";
                //cm.SendEmail(mailmodel);
                jobrunstatus = "Error " + ex.Message;
            }
            return jobrunstatus;
        }
    }
}
