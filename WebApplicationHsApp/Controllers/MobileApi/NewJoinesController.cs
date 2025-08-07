using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models.MobileApi;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class NewJoinesController : ApiController
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public List<BirthDaysModel> Get()
        {
            DateTime dt = DateTime.Now.Date;
            dt = dt.AddDays(-7);
            var list = myapp.tbl_User.Where(t => t.DateOfJoining != null && t.DateOfJoining.Value >= dt).ToList();

            var model = (from l in list
                         select new BirthDaysModel()
                         {
                             Department = l.DepartmentName,
                             Designation = l.Designation,
                             Location = l.LocationName,
                             Name = l.FirstName + " " + l.LastName,
                             UserId = l.CustomUserId
                         }).ToList();

            return model;
        }
    }
}
