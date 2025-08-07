using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class EmailTeamplates
    {
        public string LeaveBodyTemplate(string Name, string Days, string fromdate, string todate, string reason, string Designation)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplates/RequestForLeave.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{Name}", Name);
            body = body.Replace("{Days}", Days);
            body = body.Replace("{fromdate}", fromdate);
            body = body.Replace("{todate}", todate);
            body = body.Replace("{reason}", reason);
            body = body.Replace("{Designation}", Designation);
            return body;
        }
        public string NewEmployeeBodyTemplate(string EmployeeName, string empid, string intranetloginid, string intranetpassword, string ReportingManagerName, string Designation)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplates/NewEmployee.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{EmployeeName}", EmployeeName);
            body = body.Replace("{empid}", empid);
            body = body.Replace("{intranetloginid}", intranetloginid);
            body = body.Replace("{intranetpassword}", intranetpassword);
            body = body.Replace("{ReportingManagerName}", ReportingManagerName);
            body = body.Replace("{Designation}", Designation);
            return body;
        }
        public string NewTicketTemplate(string TaskId, string Call_Date, string Creator, string CategoryOfComplaint, string Description, string AssignLocation, string AssignTo)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~/EmailTemplates/NewTask.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{TaskId}", TaskId.ToString());
            body = body.Replace("{Call_Date}", Call_Date);
            body = body.Replace("{Creator}", Creator);
            body = body.Replace("{CategoryOfComplaint}", CategoryOfComplaint);
            body = body.Replace("{Description}", Description);
            body = body.Replace("{AssignLocation}", AssignLocation);
            body = body.Replace("{AssignTo}", AssignTo);
            return body;
        }
    }
}