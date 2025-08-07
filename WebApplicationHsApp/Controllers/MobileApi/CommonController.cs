using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    //[Authorize]
    [RoutePrefix("api/Common")]
    public class CommonController : ApiController
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        [HttpGet]
        [Route("GetEmployeesBySearch")]
        [AllowAnonymous]
        public List<tbl_User> GetEmployeeSearch(string searchTerm)
        {
            var query = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            query = query
                    .Where(c => c.CustomUserId.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                                 c.LocationName != null && c.LocationName.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                               c.FirstName != null && c.FirstName.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                               c.LastName != null && c.LastName.ToLower().Contains(searchTerm.ToLower())
                               ||
                               c.LastName != null && c.LastName.ToLower().Contains(searchTerm.ToLower())
                                 ||
                               c.EmailId != null && c.EmailId.ToLower().Contains(searchTerm.ToLower())
                               ||
                               c.PlaceAllocation != null && c.PlaceAllocation.ToLower().Contains(searchTerm.ToLower())
                                 ||
                               c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(searchTerm.ToLower())
                               ||
                               c.DepartmentName != null && c.DepartmentName.ToLower().Contains(searchTerm.ToLower())
                                 ||
                               c.Extenstion != null && c.Extenstion.ToLower().Contains(searchTerm.ToLower())
                                ||
                               c.Designation != null && c.Designation.ToLower().Contains(searchTerm.ToLower())
                               ).ToList();
            return query;
        }
        [HttpGet]
        [Route("GetLocations")]
        [AllowAnonymous]
        public List<tbl_Location> GetLocations()
        {
            var list = myapp.tbl_Location.ToList();
            return list;
        }
        [HttpGet]
        [Route("GetEmployees")]
        [AllowAnonymous]
        public List<UserViewModelNew> GetEmployees()
        {
            var list = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            List<UserViewModelNew> model = (from l in list
                                            select new UserViewModelNew
                                            {
                                                CugNumber = l.CugNumber,
                                                CustomUserId = l.CustomUserId,
                                                DateOfBirth = l.DateOfBirth,
                                                DateOfJoining = l.DateOfJoining,
                                                DepartmentId = l.DepartmentId,
                                                DepartmentName = l.DepartmentName,
                                                Designation = l.Designation,
                                                EmailId = l.EmailId,
                                                EmpId = l.EmpId,
                                                Extenstion = l.Extenstion,
                                                FirstName = l.FirstName,
                                                LastName = l.LastName,
                                                LocationId = l.LocationId,
                                                LocationName = l.LocationName,
                                                PhoneNumber = l.PhoneNumber,
                                                ReportingManagerId = l.ReportingManagerId,
                                                SubDepartmentId = l.SubDepartmentId,
                                                SubDepartmentName = l.SubDepartmentName,
                                                UserId = l.UserId
                                            }).ToList();
            return model;
        }
        [HttpGet]
        [Route("GetDepartmentByLocation")]
        [AllowAnonymous]
        public List<tbl_Department> GetDepartmentByLocation(int id)
        {
            var list = myapp.tbl_Department.Where(d => d.LocationId == id).OrderBy(d => d.DepartmentName).ToList();
            return list;
        }
        [HttpGet]
        [Route("GetEmployeesByDepartment")]
        [AllowAnonymous]
        public List<tbl_User> GetEmployeesByDepartment(int id)
        {
            var list = myapp.tbl_User.Where(d => d.DepartmentId == id && d.IsActive == true).OrderBy(d => d.FirstName).ToList();
            return list;
        }
        [HttpGet]
        [Route("GetCategoryByDepartment")]
        [AllowAnonymous]
        public List<tbl_Category> GetCategoryByDepartmentId(int id)
        {
            var list = myapp.tbl_DepartmentVsCategory.Where(l => l.DepartmentId == id).ToList();
            var model = (from c in myapp.tbl_Category.ToList()
                         join l in list on c.CategoryId equals l.CategoryId
                         select c).ToList();
            model = model.OrderBy(l => l.Name).ToList();
            return model;
        }
    }
}
