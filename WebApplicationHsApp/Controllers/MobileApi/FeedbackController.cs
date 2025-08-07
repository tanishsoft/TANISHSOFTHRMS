using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;
using WebApplicationHsApp.Models.MobileApi;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    [RoutePrefix("api/Feedback")]
    public class FeedbackController : ApiController
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        [HttpPost]
        [Route("SendFeedBack")]
        public string SendFeedBack(NotificationViewModel model)
        {
            var currentuser = myapp.tbl_User.Where(l => l.CustomUserId == User.Identity.Name).ToList();
            string message = "";
            if (currentuser.Count > 0)
            {
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel();
                mailmodel.fromemail = "Leave@hospitals.com";
                mailmodel.toemail = "ahmadali@fernandez.foundation";
                mailmodel.subject = "New Feed Back From Mobile App";
                mailmodel.body = "<b>"+model.Message+"</b><br /><br />";
                mailmodel.filepath = "";
                mailmodel.username = currentuser[0].FirstName + " " + currentuser[0].LastName;
                mailmodel.fromname = "FeedBack";
                mailmodel.ccemail = "vamsirm26@gmail.com";
                message = cm.SendEmail(mailmodel);
            }
            return message;
        }
    }
}
