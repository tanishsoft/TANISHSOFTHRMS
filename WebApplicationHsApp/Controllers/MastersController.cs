using System.Web.Mvc;
using System;
using System.Collections.Generic;
using WebApplicationHsApp.DataModel;
using System.Linq;
using WebApplicationHsApp.Models;
using System.Web;
using System.IO;

namespace WebApplicationHsApp.Controllers
{
    public class MastersController : Controller
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Administration/Masters
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Category()
        {
            return View();
        }
        public ActionResult SubCategory()
        {
            return View();
        }
        public ActionResult ManageDocuments()
        {
            return View();
        }

        public JsonResult SaveAdminCategory(tbl_AdminCategory category, HttpPostedFileBase Upload)
        {
            if (Upload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(Upload.FileName);
                string extension = Path.GetExtension(Upload.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;
                category.UserHelpdocument = fileName;
                Upload.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));

            }
            category.CreatedBy = User.Identity.Name;
            category.CreatedOn = DateTime.Now;
            //category.DepartmentId = Convert.ToInt32((from var in myapp.tbl_User where var.CustomUserId == category.CreatedBy select var.DepartmentId).FirstOrDefault());
            //category.LocationId = Convert.ToInt32((from var in myapp.tbl_User where var.CustomUserId == category.CreatedBy select var.LocationId).FirstOrDefault());
            category.IsActive = true;
            myapp.tbl_AdminCategory.Add(category);
            myapp.SaveChanges();
            return Json("Added Successfully", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllAdminCategoriesbyDepartmentId(int LocId,string formtype)
        {
            var query = myapp.tbl_AdminCategory.Where(l => l.DepartmentId == LocId&& l.FormType==formtype).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllAdminsubCategoriesbycategoryId(int CategoryId)
        {
            var query = myapp.tbl_AdminSubCategory.Where(l => l.CategoryId == CategoryId).ToList();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAdminCategorybyid(int CategoryId)
        {
            var query = myapp.tbl_AdminCategory.Where(l => l.CategoryId == CategoryId).SingleOrDefault();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetAdminCategory(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_AdminCategory select d).ToList();
            IEnumerable<tbl_AdminCategory> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.CategoryId.ToString().ToLower().Contains(param.sSearch.ToLower())
                   ||
                                (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName) != null && (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              
                                (from var in myapp.tbl_Department where var.DepartmentId == c.DepartmentId select var.DepartmentName) != null && (from var in myapp.tbl_Department where var.DepartmentId == c.DepartmentId select var.DepartmentName).ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Description != null && c.Description.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                                              c.CategoryId.ToString(),
                                               (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName),
                                                (from var in myapp.tbl_Department where var.DepartmentId == c.DepartmentId select var.DepartmentName),
                                              c.Name,
                                              c.Description,
                                              c.FormType,
                                              c.UserHelpdocument,
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.CategoryId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UpdateAdminCategory(tbl_AdminCategory model)
        {
            var cat = myapp.tbl_AdminCategory.Where(l => l.CategoryId == model.CategoryId).ToList();
            if (cat.Count > 0)
            {
                cat[0].FormType = model.FormType;
                cat[0].LocationId = model.LocationId;
                cat[0].DepartmentId = model.DepartmentId;
                cat[0].Name = model.Name;
                cat[0].Description = model.Description;
                cat[0].IsActive = model.IsActive;
                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAdminCategory(int id)
        {
            var cat = myapp.tbl_AdminCategory.Where(l => l.CategoryId == id).ToList();
            if (cat.Count > 0)
            {
                myapp.tbl_AdminCategory
.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetAdminsubCategory(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_AdminSubCategory select d).ToList();
            IEnumerable<tbl_AdminSubCategory> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.CategoryId.ToString().ToLower().Contains(param.sSearch.ToLower())
                   ||
                                (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName) != null && (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                (from var in myapp.tbl_AdminCategory where var.CategoryId == c.CategoryId select var.Name) != null && (from var in myapp.tbl_AdminCategory where var.CategoryId == c.CategoryId select var.Name).ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                (from var in myapp.tbl_Department where var.DepartmentId == c.DepartmentId select var.DepartmentName) != null && (from var in myapp.tbl_Department where var.DepartmentId == c.DepartmentId select var.DepartmentName).ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.Name != null && c.Name.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Description != null && c.Description.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                             c.SubCategoryId.ToString(),
                                               (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName),
                                               (from var in myapp.tbl_AdminCategory where var.CategoryId == c.CategoryId select var.Name),
                                                (from var in myapp.tbl_Department where var.DepartmentId == c.DepartmentId select var.DepartmentName),
                                              c.Name,
                                              c.Description,
                                              c.UserHelpDocuement,
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.SubCategoryId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAdminsubCategorybyid(int subCategoryId)
        {
            var query = myapp.tbl_AdminSubCategory.Where(l => l.SubCategoryId == subCategoryId).SingleOrDefault();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult saveSubcategory(tbl_AdminSubCategory category, HttpPostedFileBase Upload)
        {
            if (Upload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(Upload.FileName);
                string extension = Path.GetExtension(Upload.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;
                category.UserHelpDocuement = fileName;
                Upload.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));

            }
            category.CreatedBy = User.Identity.Name;
            category.CreatedOn = DateTime.Now;
            //category.DepartmentId = Convert.ToInt32((from var in myapp.tbl_User where var.CustomUserId == User.Identity.Name select var.DepartmentId));
            //category.LocationId = Convert.ToInt32((from var in myapp.tbl_User where var.CustomUserId == User.Identity.Name select var.LocationId));
            category.DepartmentId = Convert.ToInt32((from var in myapp.tbl_User where var.CustomUserId == category.CreatedBy select var.DepartmentId).FirstOrDefault());
            category.LocationId = Convert.ToInt32((from var in myapp.tbl_User where var.CustomUserId == category.CreatedBy select var.LocationId).FirstOrDefault());
            category.IsActive = true;
            myapp.tbl_AdminSubCategory.Add(category);
            myapp.SaveChanges();
            return Json("Added Successfully", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateSubcategory(tbl_AdminSubCategory category, HttpPostedFileBase Upload)
        {
            var cat = myapp.tbl_AdminSubCategory.Where(l => l.SubCategoryId == category.SubCategoryId).ToList();
            if (cat.Count > 0)
            {
                if (Upload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(Upload.FileName);
                    string extension = Path.GetExtension(Upload.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    if (cat[0].UserHelpDocuement != null)
                    {
                        var filePath = Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), cat[0].UserHelpDocuement);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    cat[0].UserHelpDocuement = fileName;
                    Upload.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));

                }
               // cat[0].LocationId =category.LocationId;
                cat[0].CategoryId =category.CategoryId;
               // cat[0].DepartmentId = category.DepartmentId;
                cat[0].Description = category.Description;
                cat[0].Name = category.Name;
                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
                cat[0].IsActive = true;
                myapp.SaveChanges();
            }
           
            return Json("Updated successfully", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAdminsubCategory(int id)
        {
            var cat = myapp.tbl_AdminSubCategory.Where(l => l.SubCategoryId == id).ToList();
            if (cat.Count > 0)
            {
                if (cat[0].UserHelpDocuement != null)
                {
                    var filePath = Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), cat[0].UserHelpDocuement);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                myapp.tbl_AdminSubCategory.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetAdminDocument(JQueryDataTableParamModel param)
        {

            var query = (from d in myapp.tbl_AdminDocument select d).ToList();
            IEnumerable<tbl_AdminDocument> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.CategoryId.ToString().ToLower().Contains(param.sSearch.ToLower())
                   ||
                                (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName) != null && (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName).ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                (from var in myapp.tbl_AdminCategory where var.CategoryId == c.CategoryId select var.Name) != null && (from var in myapp.tbl_AdminCategory where var.CategoryId == c.CategoryId select var.Name).ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                (from var in myapp.tbl_AdminSubCategory where var.SubCategoryId == c.SubCategoryId select var.Name) != null && (from var in myapp.tbl_AdminSubCategory where var.SubCategoryId == c.SubCategoryId select var.Name).ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.DocumentType != null && c.DocumentType.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.DocumentTitle != null && c.DocumentTitle.ToString().ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            var displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            var result = from c in displayedCompanies
                         select new object[] {
                             c.DocuementId.ToString(),
                                               (from var in myapp.tbl_Location where var.LocationId == c.LocationId select var.LocationName),
                                               (from var in myapp.tbl_AdminCategory where var.CategoryId == c.CategoryId select var.Name),
                                                 (from var in myapp.tbl_AdminSubCategory where var.SubCategoryId == c.SubCategoryId select var.Name),
                                             c.DocumentType,   c.DocumentTitle,
                                            
                                              c.DocumentDescription,
                                              c.IsActive.HasValue?c.IsActive.ToString():"false",
                                              c.DocuementId.ToString()};
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAdminDocumentbyid(int DocId)
        {
            var query = myapp.tbl_AdminDocument.Where(l => l.DocuementId == DocId).SingleOrDefault();
            return Json(query, JsonRequestBehavior.AllowGet);
        }
        public JsonResult saveAdminDocument(tbl_AdminDocument Doc, HttpPostedFileBase Upload)
        {
            var subcat = new tbl_AdminSubCategory(); var cat = new tbl_AdminCategory();
            if (Doc.SubCategoryId != 0 )
            {
                subcat = myapp.tbl_AdminSubCategory.Where(l => l.SubCategoryId == Doc.SubCategoryId).SingleOrDefault();
                Doc.LocationId = subcat.LocationId;
                Doc.DepartmentId = subcat.DepartmentId;
                Doc.CategoryId = subcat.CategoryId;
            }
            if (Doc.CategoryId != 0)
            {
                cat = myapp.tbl_AdminCategory.Where(l => l.CategoryId == Doc.CategoryId).SingleOrDefault();
                Doc.LocationId = cat.LocationId;
                Doc.DepartmentId = cat.DepartmentId;
            }

            if (Upload != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(Upload.FileName);
                string extension = Path.GetExtension(Upload.FileName);
                string guidid = Guid.NewGuid().ToString();
                fileName = fileName + guidid + extension;
                Doc.DocumentDescription = fileName;
                Upload.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));

            }
           
            
            Doc.CreatedBy = User.Identity.Name;
            Doc.CreatedOn = DateTime.Now;
            Doc.IsActive = true;
            myapp.tbl_AdminDocument.Add(Doc);
            myapp.SaveChanges();
            return Json("Added Successfully", JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateAdminDocument(tbl_AdminDocument Doc, HttpPostedFileBase Upload)
        {
            var cat = myapp.tbl_AdminDocument.Where(l => l.DocuementId == Doc.DocuementId).ToList();
            if (cat.Count > 0)
            {
                if (Upload != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(Upload.FileName);
                    string extension = Path.GetExtension(Upload.FileName);
                    string guidid = Guid.NewGuid().ToString();
                    fileName = fileName + guidid + extension;
                    if (cat[0].DocumentDescription != null)
                    {
                        var filePath = Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), cat[0].DocumentDescription);
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                    cat[0].DocumentDescription = fileName;
                    Upload.SaveAs(Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), fileName));

                }
                cat[0].LocationId = Doc.LocationId;
                cat[0].CategoryId = Doc.CategoryId;
                cat[0].DocumentType = Doc.DocumentType;
                cat[0].DocumentTitle = Doc.DocumentTitle;
                cat[0].SubCategoryId = Doc.SubCategoryId;
                cat[0].ModifiedBy = User.Identity.Name;
                cat[0].ModifiedOn = DateTime.Now;
                cat[0].IsActive = true;
                myapp.SaveChanges();
            }

            return Json("Updated successfully", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteAdminDocument(int id)
        {
            var cat = myapp.tbl_AdminDocument.Where(l => l.DocuementId == id).ToList();
            if (cat.Count > 0)
            {
                if (cat[0].DocumentDescription != null)
                {
                    var filePath = Path.Combine(Server.MapPath("~/Documents/AdministrationDocuments/"), cat[0].DocumentDescription);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                myapp.tbl_AdminDocument.Remove(cat[0]);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetDocbysubcatogery(int subId)
        {
            var files = from file in myapp.tbl_AdminDocument
                        where file.SubCategoryId == subId
                        select new { Did = file.DocuementId, url = file.DocumentDescription, Type=file.DocumentType,Title=file.DocumentTitle };
            return Json(files, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetDocbycatogery(int catId)
        {
            var files = from file in myapp.tbl_AdminDocument
                        where file.CategoryId == catId
                        select new { Did = file.DocuementId, url = file.DocumentDescription, Type = file.DocumentType, Title = file.DocumentTitle };
            return Json(files, JsonRequestBehavior.AllowGet);
        }

    }
}