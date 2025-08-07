using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class tbl_CugListController : Controller
    {
        private MyIntranetAppEntities db = new MyIntranetAppEntities();

        // GET: tbl_CugList
        public ActionResult Index()
        {
            return View(db.tbl_CugList.ToList());
        }

        // GET: tbl_CugList/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_CugList tbl_CugList = db.tbl_CugList.Find(id);
            if (tbl_CugList == null)
            {
                return HttpNotFound();
            }
            return View(tbl_CugList);
        }

        // GET: tbl_CugList/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tbl_CugList/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Extension,IsActive")] tbl_CugList tbl_CugList)
        {
            if (ModelState.IsValid)
            {
                db.tbl_CugList.Add(tbl_CugList);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tbl_CugList);
        }

        // GET: tbl_CugList/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_CugList tbl_CugList = db.tbl_CugList.Find(id);
            if (tbl_CugList == null)
            {
                return HttpNotFound();
            }
            return View(tbl_CugList);
        }

        // POST: tbl_CugList/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Extension,IsActive")] tbl_CugList tbl_CugList)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_CugList).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_CugList);
        }

        // GET: tbl_CugList/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_CugList tbl_CugList = db.tbl_CugList.Find(id);
            if (tbl_CugList == null)
            {
                return HttpNotFound();
            }
            return View(tbl_CugList);
        }

        // POST: tbl_CugList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_CugList tbl_CugList = db.tbl_CugList.Find(id);
            db.tbl_CugList.Remove(tbl_CugList);
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
    }
}
