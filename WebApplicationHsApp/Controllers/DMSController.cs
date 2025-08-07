using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    public class DMSController : Controller
    {
        private MyIntranetAppEntities _context = new MyIntranetAppEntities();
        // GET: DMS
        public ActionResult Index()
        {
            ViewBag.TotalDocuments = _context.tbl_Dms.Count();
            ViewBag.TotalEmployeeDocuments = _context.tbl_Dms.Select(l => l.EmpId).Distinct().Count();
            ViewBag.TotalEmployees = _context.tbl_User.Where(l => l.IsActive == true).Count();
            return View();
        }
        public ActionResult UploadDocument()
        {
            return View();
        }
        public ActionResult GetEmployeesDocumentUploaded()
        {
            var TotalEmployees = _context.tbl_Dms.Select(l => l.EmpId).Distinct().ToList();
            var emplist = (from e in _context.tbl_User
                           join te in TotalEmployees on e.EmpId equals te
                           let count = _context.tbl_Dms.Where(l => l.EmpId == e.EmpId).Count()
                           select new
                           {
                               e.EmpId,
                               e.FirstName,
                               e.LocationName,
                               e.DepartmentName,
                               e.Designation,
                               count
                           }).ToList();
            return Json(emplist, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SaveNewDocument(tbl_Dms model, HttpPostedFileBase[] AnnualHealthCheck, HttpPostedFileBase[] PreEmploymentHealthCheck,
            HttpPostedFileBase[] BackgroundVerification, HttpPostedFileBase[] TrainingRecords, HttpPostedFileBase[] Miscellaneous, HttpPostedFileBase[] ProbationandConfirmation,
            HttpPostedFileBase[] Privileges, HttpPostedFileBase[] PerformanceAppraisals, HttpPostedFileBase[] AppointmentLetter, HttpPostedFileBase[] JobDescription,
            HttpPostedFileBase[] JoiningReport, HttpPostedFileBase[] Resume, HttpPostedFileBase[] Registrations, HttpPostedFileBase[] QualificationCertificates,
            HttpPostedFileBase[] ExperienceLetter, HttpPostedFileBase[] AddressProof, HttpPostedFileBase[] PhotoGraph)
        {
            try
            {

                model.IsActive = true;
                model.Createdby = User.Identity.Name;
                model.Createdon = DateTime.Now;
                model.DisplayToEmp = false;
                model.EmpId = model.EmpId;
                model.IsApproved = true;
                model.IsEmpCreated = false;
                if (model.Remarks == null)
                    model.Remarks = "";
                model.ApprovedBy = User.Identity.Name;
                string locationsave = ("Z:\\" + model.EmpId);
                if (!Directory.Exists(locationsave))
                {
                    Directory.CreateDirectory(locationsave);
                }
                if (AnnualHealthCheck != null)
                {
                    SaveDocumentModelfile(model, AnnualHealthCheck, "AnnualHealthCheck", locationsave);
                }
                if (PreEmploymentHealthCheck != null)
                {
                    SaveDocumentModelfile(model, PreEmploymentHealthCheck, "PreEmploymentHealthCheck", locationsave);
                }
                if (BackgroundVerification != null)
                {
                    SaveDocumentModelfile(model, BackgroundVerification, "BackgroundVerification", locationsave);
                }
                if (TrainingRecords != null)
                {
                    SaveDocumentModelfile(model, TrainingRecords, "TrainingRecords", locationsave);
                }
                if (Miscellaneous != null)
                {
                    SaveDocumentModelfile(model, Miscellaneous, "Miscellaneous", locationsave);
                }
                if (ProbationandConfirmation != null)
                {
                    SaveDocumentModelfile(model, ProbationandConfirmation, "ProbationandConfirmation", locationsave);
                }
                if (Privileges != null)
                {
                    SaveDocumentModelfile(model, Privileges, "Privileges", locationsave);
                }
                if (PerformanceAppraisals != null)
                {
                    SaveDocumentModelfile(model, PerformanceAppraisals, "PerformanceAppraisals", locationsave);
                }
                if (AppointmentLetter != null)
                {
                    SaveDocumentModelfile(model, AppointmentLetter, "AppointmentLetter", locationsave);
                }
                if (JobDescription != null)
                {
                    SaveDocumentModelfile(model, JobDescription, "JobDescription", locationsave);
                }
                if (JoiningReport != null)
                {
                    SaveDocumentModelfile(model, JoiningReport, "JoiningReport", locationsave);
                }
                if (Resume != null)
                {
                    SaveDocumentModelfile(model, Resume, "Resume", locationsave);
                }
                if (Registrations != null)
                {
                    SaveDocumentModelfile(model, Registrations, "Registrations", locationsave);
                }
                if (QualificationCertificates != null)
                {
                    SaveDocumentModelfile(model, QualificationCertificates, "QualificationCertificates", locationsave);
                }
                if (ExperienceLetter != null)
                {
                    SaveDocumentModelfile(model, ExperienceLetter, "ExperienceLetter", locationsave);
                }
                if (AddressProof != null)
                {
                    SaveDocumentModelfile(model, AddressProof, "AddressProof", locationsave);
                }
                if (PhotoGraph != null)
                {
                    SaveDocumentModelfile(model, PhotoGraph, "PhotoGraph", locationsave);
                }
                ViewBag.Message = "Success";
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex, JsonRequestBehavior.AllowGet);
            }
        }
        public void SaveDocumentModelfile(tbl_Dms model, HttpPostedFileBase[] files, string category, string locationsave)
        {
            foreach (HttpPostedFileBase file in files)
            {
                if (file != null && !string.IsNullOrEmpty(file.FileName))
                {
                    string UniqueFileName = file.FileName.Replace(" ", "_");
                    if (!Directory.Exists(locationsave + "\\" + category))
                    {
                        Directory.CreateDirectory(locationsave + "\\" + category);
                    }
                    string PathName = Path.Combine(locationsave + "\\" + category, UniqueFileName);
                    file.SaveAs(PathName);
                    model.DocumentUrl = UniqueFileName;
                    model.DocumentName = file.FileName;
                    model.CategoryId = _context.tbl_DmsCategory.Where(l => l.Name == category).SingleOrDefault().Id;
                    _context.tbl_Dms.Add(model);
                    _context.SaveChanges();
                }
            }
        }
        public ActionResult ChangeNameOfdocument(int id, string name)
        {
            var model = _context.tbl_Dms.Where(l => l.DocumentId == id).SingleOrDefault();
            model.DocumentName = name;
            _context.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public FileStreamResult Download(int id)
        {
            var model = _context.tbl_Dms.Where(l => l.DocumentId == id).SingleOrDefault();
            string category = _context.tbl_DmsCategory.Where(l => l.Id == model.CategoryId).SingleOrDefault().Name;
            string locationsave = ("Z:\\" + model.EmpId);
            locationsave = locationsave + "\\" + category;
            FileStream fs = new FileStream(locationsave + "\\" + model.DocumentUrl, FileMode.Open, FileAccess.Read);
            string filetype = Path.GetExtension(locationsave + "\\" + model.DocumentUrl);
            return File(fs, MimeTypeMap.GetMimeType(filetype));
        }
        public ActionResult Preview(int id)
        {
            return View(id);
        }
        public ActionResult GetEmployeeDocuments(int empid)
        {
            var documents = _context.tbl_Dms.Where(l => l.EmpId == empid).ToList();
            List<DmsViewModel> result = ConvertToModel(documents);

            string realPath;
            realPath = "Z:\\" + empid;

            var c = _context.tbl_DmsCategory.FirstOrDefault();
            var e = _context.tbl_User.Where(l => l.EmpId == empid).SingleOrDefault();

            //IEnumerable<string> fileList = Directory.EnumerateFiles(realPath);
            //foreach (string file in fileList)
            //{
            //    FileInfo d = new FileInfo(file);

            //    DmsViewModel fileModel = new DmsViewModel
            //    {
            //        ApprovedBy = "Admin",
            //        CategoryColor = c.Color,
            //        CategoryId = c.Id,
            //        Createdby = e.CreatedBy,
            //        CategoryName = c.Name,
            //        Createdon = d.CreationTime.ToString("dd/MM/yyyy hh:mm tt"),
            //        DepartmentId = e.DepartmentId.HasValue ? e.DepartmentId.Value : 0,
            //        DepartmentName = e.DepartmentName,
            //        DisplayToEmp = "Yes",
            //        DocumentId = 0,
            //        DocumentName = d.Name,
            //        DocumentUrl = empid + "/" + Path.GetFileName(file),
            //        EmpId = empid,
            //        EmpName = e.FirstName,
            //        IsActive = "Yes",
            //        IsApproved = "Yes",
            //        IsDeleted = "No",
            //        IsEmpCreated = "No",
            //        LocationId = e.LocationId.HasValue ? e.LocationId.Value : 0,
            //        LocationName = e.LocationName,
            //        Remarks = ""
            //    };
            //    result.Add(fileModel);
            //}

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxbyEmployeeDocumentsAll(JQueryDataTableParamModel param)
        {
            var list = _context.tbl_Dms.ToList();
            List<DmsViewModel> tasks = ConvertToModel(list);
            tasks = tasks.OrderByDescending(t => t.DocumentId).ToList();
            IEnumerable<DmsViewModel> filtered;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filtered = tasks
                   .Where(c => c.DocumentId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.EmpName != null && c.EmpName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.EmpId != null && c.EmpId.ToString().Contains(param.sSearch.ToLower())
                               ||
                               c.DocumentName.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.CategoryName != null && c.CategoryName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationName != null && c.LocationName.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.DepartmentName != null && c.DepartmentName.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.IsActive != null && c.IsActive.ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filtered = tasks;
            }
            IEnumerable<DmsViewModel> displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayed
                                           select new[]
                                           {
                                                   Convert.ToString(c.DocumentId),
                                                   c.DepartmentName,
                                                   c.EmpId.ToString(),
                                                   c.EmpName,
                                                   c.DocumentName,
                                                   c.DocumentUrl,
                                                   c.CategoryName,
                                                   c.CategoryColor,
                                                   c.Remarks,
                                                   c.IsApproved,
                                                   c.Createdon,
                                                   c.Createdby,
                                                   c.IsActive,
                                                   Convert.ToString(c.DocumentId)
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filtered.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public List<DmsViewModel> ConvertToModel(List<tbl_Dms> documents)
        {
            var result = (from d in documents
                          join c in _context.tbl_DmsCategory on d.CategoryId equals c.Id
                          join e in _context.tbl_User on d.EmpId equals e.EmpId
                          select new DmsViewModel
                          {
                              ApprovedBy = d.ApprovedBy,
                              CategoryColor = c.Color,
                              CategoryId = c.Id,
                              Createdby = d.Createdby,
                              CategoryName = c.Name,
                              Createdon = d.Createdon.HasValue ? d.Createdon.Value.ToString("dd/MM/yyyy hh:mm tt") : "",
                              DepartmentId = e.DepartmentId.HasValue ? e.DepartmentId.Value : 0,
                              DepartmentName = e.DepartmentName,
                              DisplayToEmp = d.DisplayToEmp.HasValue ? d.DisplayToEmp.Value == true ? "Yes" : "No" : "No",
                              DocumentId = d.DocumentId,
                              DocumentName = d.DocumentName,
                              DocumentUrl = e.EmpId.Value + "/" + d.DocumentUrl,
                              EmpId = e.EmpId.HasValue ? e.EmpId.Value : 0,
                              EmpName = e.FirstName,
                              IsActive = d.IsActive.HasValue ? d.IsActive.Value == true ? "Yes" : "No" : "No",
                              IsApproved = d.IsApproved.HasValue ? d.IsApproved.Value == true ? "Yes" : "No" : "No",
                              IsDeleted = d.IsDeleted.HasValue ? d.IsDeleted.Value == true ? "Yes" : "No" : "No",
                              IsEmpCreated = d.IsEmpCreated.HasValue ? d.IsEmpCreated.Value == true ? "Yes" : "No" : "No",
                              LocationId = e.LocationId.HasValue ? e.LocationId.Value : 0,
                              LocationName = e.LocationName,
                              Remarks = d.Remarks
                          }).ToList();
            return result;
        }
        public ActionResult ManageCategory()
        {
            return View();
        }
        public ActionResult GetAllCategoryDetails(JQueryDataTableParamModel param)
        {
            List<tbl_DmsCategory> query = (from d in _context.tbl_DmsCategory select d).ToList();
            int count = 0;
            IEnumerable<tbl_DmsCategory> mealTypes;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                mealTypes = query
                    .Where(x => x.Name.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || x.Description.ToString().ToUpper().Contains(param.sSearch.ToUpper())

                    || x.IsActive.ToString().ToUpper().Contains(param.sSearch.ToUpper()));
            }
            else
            {
                mealTypes = query;
            }
            IEnumerable<tbl_DmsCategory> meals = mealTypes.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in meals
                                               //where c.IsActive !=false
                                           select new object[] {
                             "0",
                             c.Name.ToString(),
                             c.Description,
                             c.Color,
                             //string.IsNullOrEmpty(c.Document1)?string.Empty:c.Document1,
                             c.IsActive.HasValue?c.IsActive.ToString():"false",
                             c.Id.ToString()
                         };
            count = result.Count();
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = count,
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveCategoryDetails(tbl_DmsCategory mealType)
        {
            string result = string.Empty;
            try
            {
                tbl_DmsCategory meal = null;
                int Id = mealType.Id;


                if (mealType.Id > 0)
                {
                    meal = _context.tbl_DmsCategory.Where(l => l.Id == mealType.Id).SingleOrDefault();

                    meal.IsActive = true;
                    meal.Name = mealType.Name;
                }
                else
                {
                    mealType.CreatedOn = DateTime.Now;
                    mealType.IsActive = true;
                    _context.tbl_DmsCategory.Add(mealType);
                }
                _context.SaveChanges();
                if (mealType.Id > 0 || meal.Id > 0)
                {
                    result = Id > 0 ? "Category successfully updated" : "Category successfully saved";
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCategoryDetails(int Id)
        {
            tbl_DmsCategory model = _context.tbl_DmsCategory.Where(X => X.Id == Id).SingleOrDefault();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAllCategorys()
        {
            List<tbl_DmsCategory> model = _context.tbl_DmsCategory.Where(l => l.IsActive == true).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCategoryStatus(int Id)
        {
            try
            {
                tbl_DmsCategory meal = _context.tbl_DmsCategory.Where(x => x.Id == Id).FirstOrDefault();
                if (meal != null)
                {
                    _context.tbl_DmsCategory.Remove(meal);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkCategoryStatus(string name, int Id)
        {
            bool status = true;
            if (Id > 0)
            {
                status = _context.tbl_DmsCategory.Select(x => x.Name.ToUpper() == name.Trim().ToUpper() && x.Id != Id).FirstOrDefault();
            }
            {
                status = _context.tbl_DmsCategory.Select(x => x.Name.ToUpper() == name.Trim().ToUpper()).FirstOrDefault();
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
    }
}