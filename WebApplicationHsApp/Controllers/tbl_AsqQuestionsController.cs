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
    public class tbl_AsqQuestionsController : Controller
    {
        private MyIntranetAppEntities db = new MyIntranetAppEntities();

        // GET: tbl_AsqQuestions
        public ActionResult Index()
        {
            return View(db.tbl_AsqQuestions.ToList());
        }

        // GET: tbl_AsqQuestions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_AsqQuestions tbl_AsqQuestions = db.tbl_AsqQuestions.Find(id);
            if (tbl_AsqQuestions == null)
            {
                return HttpNotFound();
            }
            return View(tbl_AsqQuestions);
        }

        // GET: tbl_AsqQuestions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: tbl_AsqQuestions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AsqQuestionId,FillingMonth,FillingPageType,FillingPageTypeOrder,Question,QuestionImage,QuestionOptions,OrderNumber,IsActive")] tbl_AsqQuestions tbl_AsqQuestions)
        {
            if (ModelState.IsValid)
            {
                db.tbl_AsqQuestions.Add(tbl_AsqQuestions);
                db.SaveChanges();
                //return RedirectToAction("Index");
            }

            return View(tbl_AsqQuestions);
        }

        // GET: tbl_AsqQuestions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_AsqQuestions tbl_AsqQuestions = db.tbl_AsqQuestions.Find(id);
            if (tbl_AsqQuestions == null)
            {
                return HttpNotFound();
            }
            return View(tbl_AsqQuestions);
        }

        // POST: tbl_AsqQuestions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AsqQuestionId,FillingMonth,FillingPageType,FillingPageTypeOrder,Question,QuestionImage,QuestionOptions,OrderNumber,IsActive")] tbl_AsqQuestions tbl_AsqQuestions)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tbl_AsqQuestions).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tbl_AsqQuestions);
        }

        // GET: tbl_AsqQuestions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tbl_AsqQuestions tbl_AsqQuestions = db.tbl_AsqQuestions.Find(id);
            if (tbl_AsqQuestions == null)
            {
                return HttpNotFound();
            }
            return View(tbl_AsqQuestions);
        }

        // POST: tbl_AsqQuestions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tbl_AsqQuestions tbl_AsqQuestions = db.tbl_AsqQuestions.Find(id);
            db.tbl_AsqQuestions.Remove(tbl_AsqQuestions);
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
