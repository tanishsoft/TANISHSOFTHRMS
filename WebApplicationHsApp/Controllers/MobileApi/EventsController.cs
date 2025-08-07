using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class EventsController : ApiController
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        [HttpGet]
        [Route("Get")]
        public string Get()
        {
            var weekimg = myapp.tbl_Settings.Where(l => l.SettingKey == "GeneralEvents").ToList();
            if (weekimg.Count > 0)
            {
                return weekimg[0].SettingValue;
            }
            return "";
        }
        [HttpGet]
        [Route("GetEvents")]
        public List<tbl_Protocol> GetEvents()
        {
            var list = myapp.tbl_Protocol.Where(l => l.PageName == "Events").ToList();
            return list;
        }

    }
}
