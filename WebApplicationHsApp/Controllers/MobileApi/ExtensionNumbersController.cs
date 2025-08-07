using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class ExtensionNumbersController : ApiController
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        [EnableCors(origins: "*", headers: "*", methods: "*")]
        public List<tbl_CugList> Get()
        {
            var listuser = myapp.tbl_CugList.ToList();
            return listuser;
        }
        public List<tbl_locationlist> Get(string id)
        {
            var listuser = myapp.tbl_locationlist.Where(l => l.Location == id).ToList();
            return listuser;
        }

    }
}
