using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class EmployeeProfileController : Controller
    {
        // GET: EmployeeProfile
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult EmployeeProfile(int UserId)
        {
            var user= myapp.tbl_User.Where(l => l.UserId == UserId).SingleOrDefault();
            ViewBag.userid = UserId;
            ViewBag.cumuserid = user.CustomUserId;
            return View(user);
        }
        public JsonResult GetNotes(int Userid)
        {
            var Notes = myapp.tbl_usernotes.Where(l => l.Userid == Userid).ToList();
            return Json(Notes, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveNotes(tbl_usernotes Notes)
        {
            var list = myapp.tbl_usernotes.Where(l => l.Userid == Notes.Userid).ToList();
        
                tbl_usernotes userNotes = new tbl_usernotes();
                userNotes.Userid = Notes.Userid;
                userNotes.UserCustomId = Notes.UserCustomId;
                userNotes.Comments = Notes.Comments;
                userNotes.Createdby = User.Identity.Name;
                userNotes.Createdon = DateTime.Now;
            userNotes.Commentshead = Notes.Commentshead;
                myapp.tbl_usernotes.Add(userNotes);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}