using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MedicalEventsController : ApiController
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public string Get()
        {
            var weekimg = myapp.tbl_Settings.Where(l => l.SettingKey == "MedicalEvents").ToList();
            if (weekimg.Count > 0)
            {
                return weekimg[0].SettingValue;
            }
            return "";
        }
    }
}
