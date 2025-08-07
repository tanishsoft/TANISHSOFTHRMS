using System.Linq;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Controllers
{
    public class ChatController : Controller
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Chat
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetChats(string currentuserid, string tagretempid)
        {
            //string currentuserid = User.Identity.Name;
            //var room = myapp.tbl_Chat_Rooms.Where(l => l.EmpId == tagretempid && l.AdminId == currentuserid).SingleOrDefault();
            //if (room == null)
            //{
            //    var objroom = new tbl_Chat_Rooms
            //    {
            //        AdminId = currentuserid,
            //        EmpId = tagretempid,
            //        CreatedBy = currentuserid,
            //        CreatedDate = DateTime.Now,
            //        IsActive = true,
            //        Name = "New Chat Room _ " + tagretempid,
            //        UpdatedBy = currentuserid,
            //        UpdatedDate = DateTime.Now
            //    };
            //    myapp.tbl_Chat_Rooms.Add(objroom);
            //    myapp.SaveChanges();
            //    room = objroom;
            //}
            //var messages = myapp.tbl_Chat_Messages.Where(l => (l.ToRoomId == room.Id)).ToList();
            //var users = myapp.tbl_User.Where(l => l.IsActive == true).ToList();
            //var result = (from m in messages
            //              join u in users on m.FromUserId equals u.CustomUserId
            //              let date = m.Timestamp.ToString("dd/MM/yyyy hh:mm")
            //              let FromUser = u.FirstName
            //              select new
            //              {
            //                  m.Content,
            //                  m.CreatedDate,
            //                  date,
            //                  FromUser
            //              }).ToList();
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public ActionResult GetEmployeeSearch(string searchTerm)
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
            var resulst = (from q in query
                           select new
                           {
                               id = q.EmpId,
                               text = q.FirstName + "_" + q.EmpId
                           }).ToList();
            return Json(resulst, JsonRequestBehavior.AllowGet);
        }

    }
}