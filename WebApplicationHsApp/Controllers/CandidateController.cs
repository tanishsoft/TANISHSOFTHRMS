using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    public class CandidateController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Candidate
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Candidate()
        {
            return View();
        }
        public ActionResult AjaxGetCandidateDetails(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_Candidates select d).ToList();
            IEnumerable<tbl_Candidates> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Candidate_id.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                (c.FirstName+' '+c.LastName) != null && (c.FirstName + ' ' + c.LastName).ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Email != null && c.Email.ToString().ToLower().Contains(param.sSearch.ToLower())

                              ||
                               c.PhoneNumber != null && c.PhoneNumber.ToString().ToLower().Contains(param.sSearch.ToLower())
                             
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {


                                              c.Candidate_id.ToString(),
                                             (c.FirstName+' '+c.LastName),
                                              c.Email,
                                              c.PhoneNumber,
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.Candidate_id.ToString()
                         };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveNewCandidate(tbl_Candidates model)
        {
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn= DateTime.Now;
            model.IsActive = true;
            myapp.tbl_Candidates.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCandidateDeatilsById(int CandidateId)
        {
            var model = myapp.tbl_Candidates.Where(X => X.Candidate_id == CandidateId).SingleOrDefault();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCandidate(tbl_Candidates model)
        {
            var cat = myapp.tbl_Candidates.Where(l => l.Candidate_id == model.Candidate_id).ToList();
            if (cat.Count > 0)
            {
                cat[0].FirstName = model.FirstName;
                cat[0].LastName = model.LastName;
                cat[0].Email = model.Email;
                cat[0].PhoneNumber = model.PhoneNumber;
                cat[0].IsActive = true;
                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
            }

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteCandidate(int CandidateId)
        {
            var v = myapp.tbl_Candidates.Where(a => a.Candidate_id == CandidateId).FirstOrDefault();
            if (v != null)
            {
                myapp.tbl_Candidates.Remove(v);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}