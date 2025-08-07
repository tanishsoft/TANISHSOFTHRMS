using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class tbl_LocationlistController : Controller
    {
        private MyIntranetAppEntities db = new MyIntranetAppEntities();
        // GET: tbl_Locationlist
        public ActionResult Index()
        {
            return View(db.tbl_locationlist.ToList());
        }

        public ActionResult Create()
        {

            ViewBag.LocationList = new SelectList(db.tbl_Location, "LocationId", "LocationName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Extension,Location,Floor")]tbl_locationlist tbl_location)
        {
            if (ModelState.IsValid)
            {
                db.tbl_locationlist.Add(tbl_location);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_location);
        }



        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_locationlist list = db.tbl_locationlist.Find(id);
            if (list == null)
            {
                return HttpNotFound();
            }
            ViewBag.LocationList = new SelectList(db.tbl_Location, "LocationId", "LocationName");
            return View(list);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Extension,Location,Floor")]tbl_locationlist tbl_location)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_location).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_location);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_locationlist list = db.tbl_locationlist.Find(id);
            if (list == null)
            {
                return HttpNotFound();
            }
            return View(list);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            tbl_locationlist list = db.tbl_locationlist.Find(id);
            db.tbl_locationlist.Remove(list); ;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public ActionResult AjaxGetExtensionList(JQueryDataTableParamModel param)
        {

            var query = db.tbl_locationlist.ToList();
            var loclist = db.tbl_Location.ToList();

            if (param.locationname != null && param.locationname != "")
            {
                query = query.Where(q => q.Location == param.locationname).ToList();
            }

            IEnumerable<tbl_locationlist> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Location.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Extension != null && c.Extension.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Floor != null && c.Floor.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Name != null && c.Name.ToLower().Contains(param.sSearch.ToLower())
                              //||
                              //c.LeaveTypeName != null && c.LeaveTypeName.ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies join u in loclist on Convert.ToInt32(c.Location) equals u.LocationId

                         select new object[] {
                          c.Name,
                            c.Extension,
                           u.LocationName,
                             c.Floor,
                             c.Id.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

    }
}