using System;
using System.Collections.Generic;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class Data
    {
        public static DateTime GetDate(string date)
        {
            try
            {
                return DateTime.ParseExact(date, "dd/MM/yyyy", null);
            }
            catch
            {
                return DateTime.Now;
            }

        }
        public static string updateEmployeeshifts(int RoasterId, int ShiftTypeId, string UserId, DateTime Fromdate, string ShiftType)
        {
            DataLinq dl = new DataLinq();
            return dl.updateEmpshift(RoasterId, ShiftTypeId, UserId, Fromdate.ToShortDateString(), ShiftType);
        }
        public class ReportData
        {
            public string Name { get; set; }
            public int[] data { get; set; }
            public string Status { get; set; }
        }
        private static List<tbl_Task> _Tasklist = null;
        public static List<tbl_Task> Tasklist
        {
            get
            {
                if (_Tasklist == null || _Tasklist.Count == 0)
                {
                    _Tasklist = GetMyTasks("");
                    return _Tasklist;
                }
                else
                    return _Tasklist;
            }
            set
            {
                _Tasklist = value;
            }
        }
        public static List<tbl_Task> GetMyTasks(string AssignId)
        {


            DataLinq dl = new DataLinq();
            return dl.GetMyTasks(AssignId);
        }
       
    }
}