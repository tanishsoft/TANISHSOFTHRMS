using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;
using WebApplicationHsApp.DataModel;
using System.Threading.Tasks;
using System.Data.Entity;

namespace WebApplicationHsApp.Models
{
    public class GlobalDataFunctions
    {

        private JavaScriptSerializer JS;
        private MyIntranetAppEntities myapp;
        private TimeZoneInfo timeZoneInfo;
        private DateTime ToDayDate_DateTimeFormat_1;
        private string ToDayDate_StringFormat_1;
        private string CurrentClassName_ClassLevel;
        private MethodBase CurrentMethodBase_ClassLevel;
        private string UserFriendlyErrorMsg;
        private HttpSessionState Session;
        log4net.ILog logger = log4net.LogManager.GetLogger(typeof(GlobalDataFunctions));
        public GlobalDataFunctions()
        {
            myapp = new MyIntranetAppEntities();
            JS = new JavaScriptSerializer();
            timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            ToDayDate_DateTimeFormat_1 = TimeZoneInfo.ConvertTime(DateTime.Now, timeZoneInfo);
            ToDayDate_StringFormat_1 = ToDayDate_DateTimeFormat_1.ToString("dd/MM/yyyy");
            CurrentClassName_ClassLevel = MethodBase.GetCurrentMethod().DeclaringType.Name;
            Session = HttpContext.Current.Session;
        }
        public string ConvertAnyFormatToSpecificDateFormat(string Dt)
        {
            string ReturnData = ToDayDate_StringFormat_1;
            if (!String.IsNullOrEmpty(Dt))
            {
                try
                {
                    DateTime date;
                    if (DateTime.TryParseExact(Dt, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) == false)
                    {
                        var success = DateTime.TryParseExact(Dt, new[] { "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy", "M/d/yyyy", "dd-MM-yyyy", "d-MM-yyyy", "dd-M-yyyy", "d-M-yyyy", "MM-dd-yyyy", "M-dd-yyyy", "MM-d-yyyy", "M-d-yyyy", "yyyy-MM-dd", "yyyy-M-dd", "yyyy-MM-d", "yyyy-M-d", "yyyy-dd-MM", "yyyy-d-MM", "yyyy-dd-M", "yyyy-d-M" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                        ReturnData = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        date = DateTime.ParseExact(Dt, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                        ReturnData = date.ToString("MM/dd/yyyy");
                    }
                }
                catch (Exception EX)
                {
                    logger.Info(EX);
                }
            }
            return ReturnData;
        }

        public DateTime ConvertStringDateFormatToDateTimeFormat(string Dt)
        {
            DateTime ReturnData = ToDayDate_DateTimeFormat_1;
            if (!String.IsNullOrEmpty(Dt))
            {
                try
                {
                    DateTime date;
                    if (DateTime.TryParseExact(Dt, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) == false)
                    {
                        var success = DateTime.TryParseExact(Dt, new[] { "dd/MM/yyyy", "d/MM/yyyy", "dd/M/yyyy", "d/M/yyyy", "MM/dd/yyyy", "M/dd/yyyy", "MM/d/yyyy", "M/d/yyyy", "dd-MM-yyyy", "d-MM-yyyy", "dd-M-yyyy", "d-M-yyyy", "MM-dd-yyyy", "M-dd-yyyy", "MM-d-yyyy", "M-d-yyyy", "yyyy-MM-dd", "yyyy-M-dd", "yyyy-MM-d", "yyyy-M-d", "yyyy-dd-MM", "yyyy-d-MM", "yyyy-dd-M", "yyyy-d-M" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out date);
                        string SpecificDt = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        ReturnData = DateTime.ParseExact(SpecificDt, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        ReturnData = DateTime.ParseExact(Dt, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    }
                }
                catch (Exception EX)
                {
                    logger.Info(EX);
                }
            }
            return ReturnData;
        }


        public async Task<List<UserEmailList_RelationalClass>> GetAllUserEmailListToAdmin_GlobalFunDA(string Data)
        {
            List<UserEmailList_RelationalClass> ReturnData = new List<UserEmailList_RelationalClass>();
            try
            {
                List<tbl_User> UserList = await myapp.tbl_User.ToListAsync();
                ReturnData = UserList.Where(e => !String.IsNullOrEmpty(e.EmailId)).Select(k => new UserEmailList_RelationalClass { UID = k.UserId, EmailID = (k.EmailId).ToLower(), UserID = k.CustomUserId, UserName = k.FirstName }).ToList();
            }
            catch (Exception EX)
            {
                logger.Info(EX);
            }
            return ReturnData;
        }

    }
}