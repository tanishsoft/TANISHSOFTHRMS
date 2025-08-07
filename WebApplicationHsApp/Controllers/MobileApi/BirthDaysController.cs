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
    public class BirthDaysController : ApiController
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public List<BirthDaysModel> Get()
        {
            DateTime dt = DateTime.Now.Date;
            var listuser = myapp.tbl_User.Where(t => t.IsActive == true && t.DateOfBirth != null && t.DateOfBirth.Value.Month == dt.Month && t.DateOfBirth.Value.Day == dt.Day).ToList();
            var model = (from l in listuser
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
