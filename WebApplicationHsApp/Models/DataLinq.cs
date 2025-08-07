using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Script.Serialization;
using System.Data.OleDb;
using WebApplicationHsApp.Controllers;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    internal class DataLinq
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public string updateEmpshift(int RoasterId, int ShiftId, string UserId, string Fromdate, string ShiftType)
        {

            var es = (from e in myapp.tbl_Roaster where e.RoasterId == RoasterId select e).FirstOrDefault();
            es.UserId = UserId;
            es.ShiftTypeId = ShiftId;
            es.ShiftDate = Convert.ToDateTime(Fromdate);            
            es.ShiftTypeName = ShiftType;          
            myapp.SaveChanges();
            return "Success";
        }
        public List<tbl_ManageLeave> GetListofemphrsurvey()
        {
            var serlis = (from s in myapp.tbl_ManageLeave select s).ToList();
            return serlis;
        }
        public List<tbl_User> GetEntityList()
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            var list = (from e in myapp.tbl_User

                        select e).ToList();
           
            return list;
        }
        public List<tbl_Task> GetMyTasks(string AssignId)
        {
           
              var  tickts = (from tck in myapp.tbl_Task
                          where tck.TaskDoneByUserId== AssignId 
                          select tck).OrderByDescending(t => t.CreatedOn).ToList();
           
            return tickts;
        }
    }
}