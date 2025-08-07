using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class CpxController : Controller
    {
        private MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: Cpx
        public ActionResult Index()
        {
            ViewBag.TotalCpxs = myapp.tbl_TaskCpx.Count();
            ViewBag.TotalApproved = myapp.tbl_TaskCpx.Where(l => l.CpxStatus == "Approved").Count();
            ViewBag.TotalClosed = myapp.tbl_TaskCpx.Where(l => l.CpxStatus == "Closed").Count();
            ViewBag.TotalInProgress = myapp.tbl_TaskCpx.Where(l => l.CpxStatus == "Pending" || l.CpxStatus == "In Progress" || l.CpxStatus == "Pending at Requestor" || l.CpxStatus == "Purchase - In Progress").Count();

            ViewBag.TotalEstimatedCost = myapp.tbl_TaskCpx.Where(l => l.CpxStatus != "Rejected").Sum(l => l.EstimatedCost);
            ViewBag.TotalPriorityVital = myapp.tbl_TaskCpx.Where(l => l.CpxStatus != "Rejected" && l.Prioriy == "Vital").Count();
            ViewBag.TotalPriorityEssential = myapp.tbl_TaskCpx.Where(l => l.CpxStatus != "Rejected" && l.Prioriy == "Essential").Count();
            ViewBag.TotalPriorityDesirable = myapp.tbl_TaskCpx.Where(l => l.CpxStatus != "Rejected" && l.Prioriy == "Desirable").Count();

            return View();
        }
        public ActionResult GetCpxCountByCpxRelatedTo()
        {
            var valuesgroup = (from cpx in myapp.tbl_TaskCpx
                               where cpx.CpxStatus != "Rejected"
                               group cpx by cpx.CpxRelatedTo into cpxGroup
                               select new
                               {
                                   Key = cpxGroup.Key,
                                   Count = cpxGroup.Count(),
                               }).ToList();
            return Json(valuesgroup, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCpxCountByRequestForTheDepartment()
        {
            var commondept = myapp.tbl_CommonDepartment.ToList();
            var valuesgroup = (from cpx in myapp.tbl_TaskCpx
                               where cpx.CpxStatus != "Rejected"
                               group cpx by cpx.RequestForDepartment into cpxGroup
                               select new
                               {
                                   Key = cpxGroup.Key,
                                   Count = cpxGroup.Count(),
                               }).ToList();
            var valuesgroup1 = (from c in valuesgroup
                                join dep in commondept on c.Key equals dep.CommonDepartmentId
                                select new
                                {
                                    Key = dep.Name,
                                    Count = c.Count
                                }).ToList();
            return Json(valuesgroup1, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCpxCountByProjectTitle()
        {
            var valuesgroup = (from cpx in myapp.tbl_TaskCpx
                               where cpx.CpxStatus != "Rejected"
                               group cpx by cpx.ProjectTitle into cpxGroup
                               select new
                               {
                                   Key = cpxGroup.Key,
                                   Count = cpxGroup.Count(),
                               }).ToList();
            return Json(valuesgroup, JsonRequestBehavior.AllowGet);
        }
        public ActionResult NewRequest(int id = 0)
        {
            List<tbl_User> list = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).ToList();
            if (list.Count > 0)
            {
                ViewBag.CurrentUser = list[0];
            }
            else { ViewBag.CurrentUser = new tbl_User(); }
            ViewBag.CpaxId = id;
            return View();
        }
        public ActionResult ManageTaskApprovers()
        {
            return View();
        }
        public ActionResult ApproveCPX()
        {
            int CurrentUser = int.Parse(User.Identity.Name);
            var listofTaskApprove = myapp.tbl_TaskApproverMaster.Where(l => l.UserId == CurrentUser).ToList();
            ViewBag.IsTechApprover = listofTaskApprove.Where(l => l.ApproverLevel == 1).Count() > 0 ? true : false;
            return View();
        }
        public ActionResult MyCpxRequests()
        {
            return View();
        }
        public ActionResult SaveApproverMaster(tbl_TaskApproverMaster model)
        {
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            myapp.tbl_TaskApproverMaster.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateApproverMaster(tbl_TaskApproverMaster model)
        {
            var dbmodel = myapp.tbl_TaskApproverMaster.Where(l => l.ApproveListId == model.ApproveListId).SingleOrDefault();
            dbmodel.CreatedBy = User.Identity.Name;
            dbmodel.CreatedOn = DateTime.Now;
            dbmodel.IsActive = true;
            dbmodel.Amount = model.Amount;
            dbmodel.ApproverLevel = model.ApproverLevel;
            dbmodel.DepartmentId = model.DepartmentId;
            dbmodel.LocationId = model.LocationId;
            dbmodel.Operator = model.Operator;
            dbmodel.UserId = model.UserId;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetApproverMasterDetails(int id)
        {
            var model = myapp.tbl_TaskApproverMaster.Where(l => l.ApproveListId == id).SingleOrDefault();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteApproverMaster(int id)
        {
            var model = myapp.tbl_TaskApproverMaster.Where(l => l.ApproveListId == id).SingleOrDefault();
            myapp.tbl_TaskApproverMaster.Remove(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxbyTaskApproverMaster(JQueryDataTableParamModel param)
        {
            var listofapprovers = myapp.tbl_TaskApproverMaster.ToList();
            List<TaskApproverMasterViewModel> tasks = (from m in listofapprovers
                                                       join l in myapp.tbl_Location on m.LocationId equals l.LocationId
                                                       //join d in myapp.tbl_Department on m.Departmentid equals d.DepartmentId
                                                       //join lc in myapp.tbl_Location on m.CreatorLocationId equals lc.LocationId
                                                       //join dc in myapp.tbl_Department on m.CreatorDepartmentId equals dc.DepartmentId
                                                       join u in myapp.tbl_User on m.UserId equals u.EmpId
                                                       select new TaskApproverMasterViewModel
                                                       {
                                                           LocationId = m.LocationId,
                                                           LocationName = l.LocationName,
                                                           Amount = m.Amount,
                                                           ApproveListId = m.ApproveListId,
                                                           ApproverLevel = m.ApproverLevel,
                                                           CreatedBy = m.CreatedBy,
                                                           CreatedOn = m.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                                           DepartmentId = m.DepartmentId,
                                                           IsActive = m.IsActive,
                                                           Operator = m.Operator,
                                                           UserId = m.UserId.HasValue ? m.UserId.Value : 0,
                                                           UserName = u.FirstName
                                                       }).ToList();
            tasks = tasks.OrderByDescending(t => t.ApproveListId).ToList();
            IEnumerable<TaskApproverMasterViewModel> filteredCompanies;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = tasks
                   .Where(c => c.ApproveListId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.UserName != null && c.UserName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Amount != null && c.Amount.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.LocationName != null && c.LocationName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Operator != null && c.Operator.ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = tasks;
            }
            IEnumerable<TaskApproverMasterViewModel> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           select new[]
                                           {
                                               Convert.ToString(c.ApproveListId),
                                                   c.LocationName,
                                                   c.UserName,
                                                   c.Operator,
                                                   c.Amount.ToString(),
                                                   c.DepartmentId.ToString(),
                                                   c.ApproverLevel.ToString(),
                                                   Convert.ToString(c.ApproveListId)
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxbyMyCpxDetails(JQueryDataTableParamModel param)
        {
            int CurrentUser = int.Parse(User.Identity.Name);
            var list = myapp.tbl_TaskCpx.ToList();
            List<tbl_TaskCpx> listofapprovers = new List<tbl_TaskCpx>();
            if (!User.IsInRole("Admin"))
            {
                if (User.IsInRole("DepartmentManager"))
                {
                    List<tbl_User> userlist = myapp.tbl_User.Where(u => u.CustomUserId == User.Identity.Name).ToList();
                    int UserLocationId = userlist[0].LocationId.Value;
                    int UserDepartmentId = userlist[0].DepartmentId.Value;
                    listofapprovers = list.Where(l => l.CreatorLocationId == UserLocationId && l.CreatorDepartmentId == UserDepartmentId).ToList();
                }
                else
                {
                    listofapprovers = list.Where(l => l.CreatorId == CurrentUser).ToList();
                }
            }
            else
            {
                listofapprovers = list;
            }
            List<CpxViewModel> tasks = (from m in listofapprovers
                                        join l in myapp.tbl_Location on m.CreatorLocationId equals l.LocationId
                                        join d in myapp.tbl_Department on m.CreatorDepartmentId equals d.DepartmentId
                                        join dc in myapp.tbl_CommonDepartment on m.RequestForDepartment equals dc.CommonDepartmentId
                                        //join appr in myapp.tbl_TaskApprove on m.TaskCpxId equals appr.TaskId
                                        //where appr.IsApproved == false && appr.ApprovalLevel == Convert.ToString(m.CurrentApprovalLevel)
                                        join u in myapp.tbl_User on m.CreatorId equals u.EmpId
                                        //join u2 in myapp.tbl_User on appr.ApproveEmpId equals u2.EmpId
                                        select new CpxViewModel
                                        {
                                            Budgeted = m.Budgeted.HasValue ? (m.Budgeted.Value == true ? "Yes" : "No") : "No",
                                            CreatorDepartmentId = m.CreatorDepartmentId.ToString(),
                                            CreatorDepartmentName = d.DepartmentName,
                                            CreatorId = m.CreatorId.ToString(),
                                            CreatorLocationId = m.CreatorLocationId.ToString(),
                                            CreatorLocationName = l.LocationName,
                                            CreatorName = u.FirstName,
                                            EstimatedCost = m.EstimatedCost.HasValue ? m.EstimatedCost.Value : 0,
                                            IsNewItem = m.IsNewItem.HasValue ? (m.IsNewItem.Value == true ? "Yes" : "No") : "No",
                                            Item = (m.Item == null || m.Item == "Select") ? m.ItemDetails : m.Item,
                                            ItemDetails = m.ItemDetails,
                                            MainFeatures = m.MainFeatures,
                                            Model = m.Model,
                                            Priority = m.Prioriy,
                                            ProjectTitle = m.ProjectTitle,
                                            Qty = m.Qty.HasValue ? m.Qty.Value.ToString() : "0",
                                            RequestForDepartment = dc.Name,
                                            TaskCpxId = m.TaskCpxId,
                                            CpxStatus = m.CpxStatus,
                                            CreatedDate = m.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                            WorkDoneComments = m.WorkDoneComments,
                                            CpxRelatedTo = m.CpxRelatedTo,
                                            CpxRelatedToOther = m.CpxRelatedToOther,
                                            OldProductCost = m.OldProductCost,
                                            OldProductModel = m.OldProductModel,
                                            CPXType = m.CPXType,
                                            SupportCompany = m.SupportCompany,
                                            ReasonForReplacement = m.ReasonForReplacement,
                                            AMCLastRenewalDate = m.AMCLastRenewalDate.HasValue ? m.AMCLastRenewalDate.Value.ToString("dd/MM/yyyy") : "",
                                            AMCNextRenewalDate = m.AMCNextRenewalDate.HasValue ? m.AMCNextRenewalDate.Value.ToString("dd/MM/yyyy") : "",

                                        }).ToList();
            tasks = tasks.OrderByDescending(t => t.TaskCpxId).ToList();
            IEnumerable<CpxViewModel> filtered;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filtered = tasks
                   .Where(c => c.TaskCpxId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.CreatorName != null && c.CreatorName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.EstimatedCost.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.Item != null && c.Item.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.ItemDetails != null && c.ItemDetails.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.CreatorLocationName != null && c.CreatorLocationName.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.Priority != null && c.Priority.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.ProjectTitle != null && c.ProjectTitle.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.CpxStatus != null && c.CpxStatus.ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filtered = tasks;
            }
            IEnumerable<CpxViewModel> displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayed
                                           select new[]
                                           {
                                                   Convert.ToString(c.TaskCpxId),
                                                   c.CPXType,
                                                   c.CreatedDate,
                                                   c.CreatorLocationName+" "+c.CreatorDepartmentName,
                                                   c.CreatorName,
                                                   c.ProjectTitle,
                                                   c.RequestForDepartment,
                                                   c.Item,
                                                   c.Qty,
                                                   c.EstimatedCost.ToString(),
                                                   c.Budgeted,
                                                   c.IsNewItem,
                                                   c.Priority,
                                                   c.WorkDoneComments,
                                                   c.CpxStatus,
                                                   Convert.ToString(c.TaskCpxId)
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filtered.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxbyCpxDetails(JQueryDataTableParamModel param)
        {
            var listofapprovers = myapp.tbl_TaskCpx.Where(l => l.IsApproved == true).ToList();
            if (param.locationid != null && param.locationid != 0)
            {
                listofapprovers = listofapprovers.Where(m => m.CreatorLocationId == param.locationid).ToList();
            }
            if (param.departmentid != null && param.departmentid != 0)
            {
                listofapprovers = listofapprovers.Where(m => m.CreatorDepartmentId == param.departmentid).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                listofapprovers = listofapprovers.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (param.category != null && param.category != "")
            {
                listofapprovers = listofapprovers.Where(m => m.CpxRelatedTo == param.category).ToList();
            }
            if (param.FormType != null && param.FormType != "" && param.FormType != "Select Project")
            {
                listofapprovers = listofapprovers.Where(l => l.ProjectTitle == param.FormType).ToList();
            }
            List<CpxViewModel> tasks = (from m in listofapprovers
                                        join l in myapp.tbl_Location on m.CreatorLocationId equals l.LocationId
                                        join d in myapp.tbl_Department on m.CreatorDepartmentId equals d.DepartmentId
                                        join dc in myapp.tbl_CommonDepartment on m.RequestForDepartment equals dc.CommonDepartmentId
                                        join u in myapp.tbl_User on m.CreatorId equals u.EmpId
                                        select new CpxViewModel
                                        {
                                            Budgeted = m.Budgeted.HasValue ? (m.Budgeted.Value == true ? "Yes" : "No") : "No",
                                            CreatorDepartmentId = m.CreatorDepartmentId.ToString(),
                                            CreatorDepartmentName = d.DepartmentName,
                                            CreatorId = m.CreatorId.ToString(),
                                            CreatorLocationId = m.CreatorLocationId.ToString(),
                                            CreatorLocationName = l.LocationName,
                                            CreatorName = u.FirstName,
                                            EstimatedCost = m.EstimatedCost.HasValue ? m.EstimatedCost.Value : 0,
                                            IsNewItem = m.IsNewItem.HasValue ? (m.IsNewItem.Value == true ? "Yes" : "No") : "No",
                                            Item = (m.Item == null || m.Item == "Select") ? m.ItemDetails : m.Item,
                                            ItemDetails = m.ItemDetails,
                                            MainFeatures = m.MainFeatures,
                                            Model = m.Model,
                                            Priority = m.Prioriy,
                                            ProjectTitle = m.ProjectTitle,
                                            Qty = m.Qty.HasValue ? m.Qty.Value.ToString() : "0",
                                            RequestForDepartment = dc.Name,
                                            TaskCpxId = m.TaskCpxId,
                                            CpxStatus = m.CpxStatus,
                                            CreatedDate = m.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                            CpxRelatedTo = m.CpxRelatedTo,
                                            WorkDoneComments = m.WorkDoneComments,
                                            CpxRelatedToOther = m.CpxRelatedToOther,
                                            CPXType = m.CPXType
                                        }).ToList();
            tasks = tasks.OrderByDescending(t => t.TaskCpxId).ToList();
            IEnumerable<CpxViewModel> filtered;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filtered = tasks
                   .Where(c => c.TaskCpxId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.CreatorName != null && c.CreatorName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.EstimatedCost.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.Item != null && c.Item.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.ItemDetails != null && c.ItemDetails.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.CreatorLocationName != null && c.CreatorLocationName.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.Priority != null && c.Priority.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.ProjectTitle != null && c.ProjectTitle.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.CpxStatus != null && c.CpxStatus.ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filtered = tasks;
            }
            IEnumerable<CpxViewModel> displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayed
                                           select new[]
                                           {
                                                   Convert.ToString(c.TaskCpxId),
                                                   c.CPXType,
                                                   c.CpxRelatedTo=="Others"?c.CpxRelatedToOther:c.CpxRelatedTo,
                                                   c.CreatedDate,
                                                   c.CreatorLocationName+" "+c.CreatorDepartmentName,
                                                   c.CreatorName,
                                                   c.ProjectTitle,
                                                   c.RequestForDepartment,
                                                   c.Item,
                                                   c.Qty,
                                                   c.EstimatedCost.ToString(),
                                                   c.Budgeted,
                                                   c.IsNewItem,
                                                   c.Priority,
                                                   c.CpxStatus,
                                                   Convert.ToString(c.TaskCpxId)
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filtered.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxbyCpxDetailsAll(JQueryDataTableParamModel param)
        {
            var listofapprovers = myapp.tbl_TaskCpx.ToList();
            if (param.locationid != null && param.locationid != 0)
            {
                listofapprovers = listofapprovers.Where(m => m.CreatorLocationId == param.locationid).ToList();
            }
            if (param.departmentid != null && param.departmentid != 0)
            {
                listofapprovers = listofapprovers.Where(m => m.CreatorDepartmentId == param.departmentid).ToList();
            }
            if (param.category != null && param.category != "")
            {
                listofapprovers = listofapprovers.Where(m => m.CpxRelatedTo == param.category).ToList();
            }
            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                listofapprovers = listofapprovers.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            if (param.categoryid != null && param.categoryid != 0)
            {
                listofapprovers = listofapprovers.Where(l => l.CurrentApprovalLevel == param.categoryid).ToList();
            }
            if (param.FormType != null && param.FormType != "" && param.FormType != "Select Project")
            {
                listofapprovers = listofapprovers.Where(l => l.ProjectTitle == param.FormType).ToList();
            }
            List<CpxViewModel> tasks = (from m in listofapprovers
                                        join l in myapp.tbl_Location on m.CreatorLocationId equals l.LocationId
                                        join d in myapp.tbl_Department on m.CreatorDepartmentId equals d.DepartmentId
                                        join dc in myapp.tbl_CommonDepartment on m.RequestForDepartment equals dc.CommonDepartmentId
                                        join u in myapp.tbl_User on m.CreatorId equals u.EmpId
                                        //let approv = myapp.tbl_TaskApprove.Where(a => a.TaskId == m.TaskId && Convert.ToInt32(a.ApprovalLevel) == m.CurrentApprovalLevel).SingleOrDefault()
                                        select new CpxViewModel
                                        {
                                            Budgeted = m.Budgeted.HasValue ? (m.Budgeted.Value == true ? "Yes" : "No") : "No",
                                            CreatorDepartmentId = m.CreatorDepartmentId.ToString(),
                                            CreatorDepartmentName = d.DepartmentName,
                                            CreatorId = m.CreatorId.ToString(),
                                            CreatorLocationId = m.CreatorLocationId.ToString(),
                                            CreatorLocationName = l.LocationName,
                                            CreatorName = u.FirstName,
                                            EstimatedCost = m.EstimatedCost.HasValue ? m.EstimatedCost.Value : 0,
                                            IsNewItem = m.IsNewItem.HasValue ? (m.IsNewItem.Value == true ? "Yes" : "No") : "No",
                                            Item = (m.Item == null || m.Item == "Select") ? m.ItemDetails : m.Item,
                                            ItemDetails = m.ItemDetails,
                                            MainFeatures = m.MainFeatures,
                                            Model = m.Model,
                                            Priority = m.Prioriy,
                                            ProjectTitle = m.ProjectTitle,
                                            Qty = m.Qty.HasValue ? m.Qty.Value.ToString() : "0",
                                            RequestForDepartment = dc.Name,
                                            TaskCpxId = m.TaskCpxId,
                                            CpxStatus = m.CpxStatus,
                                            CurrentApproverLevel = m.CurrentApprovalLevel.HasValue ? m.CurrentApprovalLevel.Value.ToString() : "0",
                                            CurrentApprover = "",
                                            CreatedDate = m.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                            WorkDoneComments = m.WorkDoneComments,
                                            CpxRelatedTo = m.CpxRelatedTo,
                                            CpxRelatedToOther = m.CpxRelatedToOther,
                                            CPXType = m.CPXType
                                        }).ToList();
            tasks = tasks.OrderByDescending(t => t.TaskCpxId).ToList();
            IEnumerable<CpxViewModel> filtered;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filtered = tasks
                   .Where(c => c.TaskCpxId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.CreatorName != null && c.CreatorName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.EstimatedCost.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.Item != null && c.Item.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.ItemDetails != null && c.ItemDetails.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.CreatorLocationName != null && c.CreatorLocationName.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.Priority != null && c.Priority.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.ProjectTitle != null && c.ProjectTitle.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.CpxStatus != null && c.CpxStatus.ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filtered = tasks;
            }
            IEnumerable<CpxViewModel> displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayed
                                           select new[]
                                           {
                                                   Convert.ToString(c.TaskCpxId),
                                                   c.CPXType,
                                                   c.CpxRelatedTo=="Others"?c.CpxRelatedToOther:c.CpxRelatedTo,
                                                   c.CreatedDate,
                                                   c.CreatorLocationName+" "+c.CreatorDepartmentName,
                                                   c.CreatorName,
                                                   c.ProjectTitle,
                                                   c.RequestForDepartment,
                                                   c.Item,
                                                   c.Qty,
                                                   c.EstimatedCost.ToString(),
                                                   c.Budgeted,
                                                   c.IsNewItem,
                                                   c.Priority,
                                                   c.CpxStatus,
                                                  GetCurrentApproverName(c.TaskCpxId,c.CurrentApproverLevel),
                                                   Convert.ToString(c.TaskCpxId)
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filtered.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public string GetCurrentApproverName(int TaskId, string Level)
        {
            string Approver = "";
            var listcheck = myapp.tbl_TaskApprove.Where(au => au.TaskId == TaskId && au.ApprovalLevel == Level).ToList();
            if (listcheck.Count > 0)
            {
                foreach (var l in listcheck)
                {
                    var user = myapp.tbl_User.Where(u => u.EmpId == l.ApproveEmpId).SingleOrDefault();
                    if (user != null)
                    {
                        Approver += user.FirstName + ",";
                    }
                }
                return Approver.TrimEnd(',');
            }
            else
            {
                return "";
            }
        }
        public ActionResult AjaxbyCpxDetailsToApproveMaster(JQueryDataTableParamModel param)
        {
            var listofcpx = myapp.tbl_TaskCpx.ToList();
            int CurrentUser = int.Parse(User.Identity.Name);
            var listofTaskApprove = myapp.tbl_TaskApprove.Where(l => l.ApproveEmpId == CurrentUser).ToList();

            var filtercpx = (from m in listofcpx
                             join app in listofTaskApprove on m.TaskCpxId equals app.TaskId
                             where app.ApproveEmpId == CurrentUser
                             //&& (app.IsApproved == false || app.IsApproved == true)
                             && (app.IsApproved == true || app.ApprovalLevel == Convert.ToString(m.CurrentApprovalLevel))
                             select m).Distinct().ToList();

            List<CpxViewModel> tasks = (from m in filtercpx
                                        join l in myapp.tbl_Location on m.CreatorLocationId equals l.LocationId
                                        join d in myapp.tbl_Department on m.CreatorDepartmentId equals d.DepartmentId
                                        join dc in myapp.tbl_CommonDepartment on m.RequestForDepartment equals dc.CommonDepartmentId
                                        join u in myapp.tbl_User on m.CreatorId equals u.EmpId
                                        //join app in listofTaskApprove on m.TaskCpxId equals app.TaskId
                                        //where app.ApproveEmpId == CurrentUser
                                        let app = listofTaskApprove.Where(la => la.TaskId == m.TaskCpxId && la.IsApproved == false).FirstOrDefault()
                                        select new CpxViewModel
                                        {
                                            Budgeted = m.Budgeted.HasValue ? (m.Budgeted.Value == true ? "Yes" : "No") : "No",
                                            CreatorDepartmentId = m.CreatorDepartmentId.ToString(),
                                            CreatorDepartmentName = d.DepartmentName,
                                            CreatorId = m.CreatorId.ToString(),
                                            CreatorLocationId = m.CreatorLocationId.ToString(),
                                            CreatorLocationName = l.LocationName,
                                            CreatorName = u.FirstName,
                                            EstimatedCost = m.EstimatedCost.HasValue ? m.EstimatedCost.Value : 0,
                                            IsNewItem = m.IsNewItem.HasValue ? (m.IsNewItem.Value == true ? "Yes" : "No") : "No",
                                            Item = (m.Item == null || m.Item == "Select") ? m.ItemDetails : m.Item,
                                            ItemDetails = m.ItemDetails,
                                            MainFeatures = m.MainFeatures,
                                            Model = m.Model,
                                            Priority = m.Prioriy,
                                            ProjectTitle = m.ProjectTitle,
                                            Qty = m.Qty.HasValue ? m.Qty.Value.ToString() : "0",
                                            RequestForDepartment = dc.Name,
                                            TaskCpxId = m.TaskCpxId,
                                            CpxRelatedTo = m.CpxRelatedTo,
                                            CpxStatus = m.CpxStatus,
                                            MyApprovalStatus = (app != null && app.IsApproved.HasValue) ? ((app.IsApproved.Value == false || m.CpxStatus == "Rejected") ? "No" : "Yes") : "Yes",
                                            CpxRelatedToOther = m.CpxRelatedToOther,
                                            OldProductCost = m.OldProductCost,
                                            OldProductModel = m.OldProductModel,
                                            ReasonForReplacement = m.ReasonForReplacement,
                                            SupportCompany = m.SupportCompany,
                                            CPXType = m.CPXType
                                        }).ToList();

            if (param.status != null && param.status != "" && param.status != "All")
            {
                tasks = tasks.Where(l => l.MyApprovalStatus == param.status).ToList();
            }
            tasks = tasks.OrderByDescending(t => t.TaskCpxId).ToList();
            IEnumerable<CpxViewModel> filtered;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filtered = tasks
                   .Where(c => c.TaskCpxId.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                                c.CreatorName != null && c.CreatorName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                               c.EstimatedCost.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.Item != null && c.Item.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.ItemDetails != null && c.ItemDetails.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.CreatorLocationName != null && c.CreatorLocationName.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.Priority != null && c.Priority.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.ProjectTitle != null && c.ProjectTitle.ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filtered = tasks;
            }
            IEnumerable<CpxViewModel> displayed = filtered.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayed
                                           select new[]
                                           {
                                                   Convert.ToString(c.TaskCpxId),
                                                   c.CPXType,
                                                   c.CpxRelatedTo=="Others"?c.CpxRelatedToOther:c.CpxRelatedTo,
                                                   c.CreatorLocationName,
                                                   c.CreatorDepartmentName,
                                                   c.CreatorName,
                                                   c.ProjectTitle,
                                                   c.Item,
                                                   c.Qty,
                                                   c.EstimatedCost.ToString(),
                                                   c.Budgeted,
                                                   c.CpxStatus,
                                                   c.MyApprovalStatus,
                                                  c.MyApprovalStatus=="Yes"? "Yes-"+Convert.ToString(c.TaskCpxId):Convert.ToString(c.TaskCpxId)
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filtered.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateCpxDetails(CpxViewModel objcpx)
        {
            if (objcpx.TaskCpxId > 0)
            {

                var modelcpx = myapp.tbl_TaskCpx.Where(l => l.TaskCpxId == objcpx.TaskCpxId).SingleOrDefault();
                modelcpx.SupportCompany = objcpx.SupportCompany;
                modelcpx.ReasonForReplacement = objcpx.ReasonForReplacement;
                modelcpx.OldProductModel = objcpx.OldProductModel;
                if (objcpx.AMCLastRenewalDate != null && objcpx.AMCLastRenewalDate != "")
                {
                    modelcpx.AMCLastRenewalDate = ProjectConvert.ConverDateStringtoDatetime(objcpx.AMCLastRenewalDate);
                }
                if (objcpx.AMCNextRenewalDate != null && objcpx.AMCNextRenewalDate != "")
                {
                    modelcpx.AMCNextRenewalDate = ProjectConvert.ConverDateStringtoDatetime(objcpx.AMCNextRenewalDate);
                }
                modelcpx.CPXType = objcpx.CPXType;
                modelcpx.CreatorLocationId = int.Parse(objcpx.CreatorLocationId);
                modelcpx.CreatorDepartmentId = int.Parse(objcpx.CreatorDepartmentId);
                modelcpx.CreatorId = int.Parse(objcpx.CreatorId);
                modelcpx.ProjectTitle = objcpx.ProjectTitle;
                modelcpx.SubProjectTitle = objcpx.SubProjectTitle;
                modelcpx.Budgeted = Convert.ToBoolean(objcpx.Budgeted);
                modelcpx.ModifiedBy = User.Identity.Name;
                modelcpx.ModifiedOn = DateTime.Now;
                modelcpx.EstimatedCost = objcpx.EstimatedCost;

                modelcpx.IsNewItem = Convert.ToBoolean(objcpx.IsNewItem);
                modelcpx.Item = objcpx.Item;
                modelcpx.ItemDetails = objcpx.ItemDetails;
                modelcpx.MainFeatures = objcpx.MainFeatures;
                modelcpx.Model = objcpx.Model;

                modelcpx.Prioriy = objcpx.Priority;
                if (objcpx.Qty != null)
                    modelcpx.Qty = int.Parse(objcpx.Qty);
                modelcpx.RequestForDepartment = int.Parse(objcpx.RequestForDepartment);

                modelcpx.IsClosed = false;

                modelcpx.OldProductCost = objcpx.OldProductCost;
                modelcpx.CpxRelatedToOther = objcpx.CpxRelatedToOther;

                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveCpxDetails(CpxViewModel objcpx)
        {
            HrDataManage hrdm = new HrDataManage();
            //string rpt = hrdm.GetReportingMgr(User.Identity.Name, DateTime.Today, DateTime.Today);
            int CreatorLocationId = int.Parse(objcpx.CreatorLocationId);
            tbl_TaskCpx modelcpx = new tbl_TaskCpx();
            if (objcpx.TaskCpxId > 0)
            {
                modelcpx = myapp.tbl_TaskCpx.Where(l => l.TaskCpxId == objcpx.TaskCpxId).SingleOrDefault();
            }
            modelcpx.SupportCompany = objcpx.SupportCompany;
            modelcpx.ReasonForReplacement = objcpx.ReasonForReplacement;
            modelcpx.OldProductModel = objcpx.OldProductModel;
            if (objcpx.AMCLastRenewalDate != null && objcpx.AMCLastRenewalDate != "")
            {
                modelcpx.AMCLastRenewalDate = ProjectConvert.ConverDateStringtoDatetime(objcpx.AMCLastRenewalDate);
            }
            if (objcpx.AMCNextRenewalDate != null && objcpx.AMCNextRenewalDate != "")
            {
                modelcpx.AMCNextRenewalDate = ProjectConvert.ConverDateStringtoDatetime(objcpx.AMCNextRenewalDate);
            }
            modelcpx.CPXType = objcpx.CPXType;

            modelcpx.ProjectTitle = objcpx.ProjectTitle;
            modelcpx.SubProjectTitle = objcpx.SubProjectTitle;
            modelcpx.Budgeted = Convert.ToBoolean(objcpx.Budgeted);

            modelcpx.EstimatedCost = objcpx.EstimatedCost;
            modelcpx.IsNewItem = Convert.ToBoolean(objcpx.IsNewItem);
            modelcpx.Item = objcpx.Item;
            modelcpx.ItemDetails = objcpx.ItemDetails;
            modelcpx.MainFeatures = objcpx.MainFeatures;
            modelcpx.Model = objcpx.Model;
            modelcpx.ModifiedBy = User.Identity.Name;
            modelcpx.ModifiedOn = DateTime.Now;
            modelcpx.Prioriy = objcpx.Priority;
            if (objcpx.Qty != null)
                modelcpx.Qty = int.Parse(objcpx.Qty);
            modelcpx.RequestForDepartment = int.Parse(objcpx.RequestForDepartment);

            modelcpx.CpxRelatedTo = objcpx.CpxRelatedTo;
            //modelcpx.TaskCpxId = task.TaskId;
            modelcpx.TaskId = 1;
            modelcpx.IsClosed = false;

            modelcpx.OldProductCost = objcpx.OldProductCost;
            modelcpx.CpxRelatedToOther = objcpx.CpxRelatedToOther;
            if (objcpx.TaskCpxId > 0)
            {

                myapp.SaveChanges();
            }
            else
            {
                modelcpx.CreatorLocationId = int.Parse(objcpx.CreatorLocationId);
                modelcpx.CreatorDepartmentId = int.Parse(objcpx.CreatorDepartmentId);
                modelcpx.CreatorId = int.Parse(objcpx.CreatorId);
                modelcpx.IsActive = true;
                modelcpx.IsApproved = false;
                modelcpx.CreatedBy = User.Identity.Name;
                modelcpx.CreatedOn = DateTime.Now;
                modelcpx.CpxStatus = "Pending";
                modelcpx.CurrentApprovalLevel = 2;
                string currentuser = User.Identity.Name;
                var reporingmanager = myapp.tbl_User.Where(l => l.CustomUserId == currentuser).SingleOrDefault();
                string rpt = "";
                if (reporingmanager.ReportingManagerId != null)
                {
                    rpt = reporingmanager.ReportingManagerId.ToString();
                    if (rpt != User.Identity.Name)
                    {
                        modelcpx.CurrentApprovalLevel = 1;
                    }
                }
                myapp.tbl_TaskCpx.Add(modelcpx);
                myapp.SaveChanges();


                int checklocationid = CreatorLocationId;
                if (modelcpx.ProjectTitle != null && modelcpx.ProjectTitle != "")
                {
                    var locations = myapp.tbl_Location.Where(l => l.LocationName == modelcpx.ProjectTitle).ToList();
                    if (locations.Count > 0)
                    {
                        checklocationid = locations[0].LocationId;
                    }
                }
                //Approvers
                int relateddepartment = 0;
                switch (modelcpx.CpxRelatedTo)
                {
                    case "IT":
                        relateddepartment = 1;
                        break;
                    case "Biomedical":
                        relateddepartment = 2;
                        break;
                    case "Maintenance":
                        relateddepartment = 3;
                        break;
                    case "Maintenance Renovation":
                        relateddepartment = 11;
                        break;
                    case "Pharmacy":
                        relateddepartment = 4;
                        break;
                    case "Food & Beverage":
                        relateddepartment = 6;
                        break;
                    case "Brand & Communication":
                        relateddepartment = 7;
                        break;
                    case "OT":
                        relateddepartment = 8;
                        break;
                    case "Security and Transport":
                        relateddepartment = 9;
                        break;
                    case "House Keeping":
                        relateddepartment = 10;
                        break;
                    case "HR":
                        relateddepartment = 18;
                        break;
                    case "HICC":
                        relateddepartment = 12;
                        break;
                    case "Purchase":
                        relateddepartment = 13;
                        break;
                    case "OT-OBS":
                        relateddepartment = 14;
                        break;
                    case "OT-GYN":
                        relateddepartment = 15;
                        break;
                    case "OT-Others":
                        relateddepartment = 16;
                        break;
                    case "OT-Neonatal-and-Pediatric":
                        relateddepartment = 17;
                        break;
                    case "Vehicle":
                        relateddepartment = 19;
                        break;
                    case "Finance":
                        relateddepartment = 20;
                        break;
                    default:
                        relateddepartment = 5;
                        break;
                }

                List<tbl_TaskApproverMaster> listofapprovers = myapp.tbl_TaskApproverMaster.Where(l => l.IsActive == true && l.LocationId == checklocationid && l.DepartmentId == relateddepartment && l.ApproverLevel <= 1).ToList();
                if (objcpx.CpxRelatedTo == "Biomedical")
                {
                    listofapprovers = listofapprovers.Where(l => l.DepartmentId == 0 || l.DepartmentId == 2).ToList();
                }
                else
                {
                    listofapprovers = listofapprovers.Where(l => l.DepartmentId != 2).ToList();
                }
                foreach (var approver in listofapprovers)
                {

                    bool saveapprover = false;
                    if (approver.Operator == "lessthan" && objcpx.EstimatedCost < approver.Amount)
                    {
                        saveapprover = true;
                    }
                    if (approver.Operator == "GraterThan" && objcpx.EstimatedCost > approver.Amount)
                    {
                        saveapprover = true;
                    }
                    if (approver.Operator == "EqualTo" && approver.Amount == objcpx.EstimatedCost)
                    {
                        saveapprover = true;
                    }
                    if (approver.Operator == "Both")
                    {
                        saveapprover = true;
                    }

                    if (saveapprover)
                    {
                        tbl_TaskApprove modelapprove = new tbl_TaskApprove();
                        modelapprove.ApprovalLevel = approver.ApproverLevel.ToString();
                        modelapprove.ApproveComments = "";
                        modelapprove.ApproveEmpId = approver.UserId;
                        modelapprove.IsApproved = false;
                        modelapprove.TaskId = modelcpx.TaskCpxId;
                        myapp.tbl_TaskApprove.Add(modelapprove);
                        myapp.SaveChanges();
                    }
                }
                tbl_TaskApprove modelapprove1 = new tbl_TaskApprove();

                // Check Current User as approval and then set as approved
                try
                {
                    var listcheckapp = myapp.tbl_TaskApprove.Where(l => l.TaskId == modelcpx.TaskCpxId && l.ApproveEmpId == reporingmanager.EmpId).ToList();
                    if (listcheckapp.Count > 0)
                    {
                        foreach (var lcapp in listcheckapp)
                        {
                            lcapp.IsApproved = true;
                            lcapp.ApprovedDate = DateTime.Now;
                            lcapp.ApproveComments = "System";
                            myapp.SaveChanges();
                        }
                    }
                }
                catch { }


                //get Reporting Manager

                if (rpt != null && rpt != "" && rpt != User.Identity.Name && relateddepartment != 11)
                {
                    modelapprove1 = new tbl_TaskApprove();
                    modelapprove1.ApprovalLevel = "0";
                    modelapprove1.ApproveComments = "";
                    modelapprove1.ApproveEmpId = int.Parse(rpt.Replace("fh_", ""));
                    modelapprove1.IsApproved = false;
                    modelapprove1.TaskId = modelcpx.TaskCpxId;
                    myapp.tbl_TaskApprove.Add(modelapprove1);
                    myapp.SaveChanges();
                    var modelcpx1 = myapp.tbl_TaskCpx.Where(l => l.TaskCpxId == modelcpx.TaskCpxId).SingleOrDefault();
                    //var approvers = myapp.tbl_TaskApprove.Where(l => l.TaskId == modelcpx.TaskCpxId && l.ApprovalLevel == "-1").ToList();
                    //if (approvers.Count == 0)
                    //    modelcpx1.CurrentApprovalLevel = 0;
                    //else
                    modelcpx1.CurrentApprovalLevel = 0;
                    myapp.SaveChanges();
                    string ccemail = "";
                    string mobile = "";
                    string approvername = "";
                    int approverid = 0;
                    // var approvers = myapp.tbl_TaskApprove.Where(l => l.TaskId == modelcpx.TaskCpxId && l.ApprovalLevel == "1").ToList();
                    //foreach (var approver in approvers)
                    //{
                    var user = myapp.tbl_User.Where(l => l.EmpId == modelapprove1.ApproveEmpId).ToList();
                    if (user.Count > 0)
                    {
                        if (user[0].EmailId != null && user[0].EmailId != "")
                            ccemail = user[0].EmailId + ",";
                        if (user[0].PhoneNumber != null && user[0].PhoneNumber != "")
                        {
                            mobile = user[0].PhoneNumber;
                        }
                        approvername = user[0].FirstName;
                    }
                    approverid = modelapprove1.ApproveEmpId.HasValue ? modelapprove1.ApproveEmpId.Value : 0;
                    //}
                    SendEmailOnCpxForNextapprover(modelcpx.TaskCpxId, ccemail.TrimEnd(','), mobile, approvername, 0, approverid);
                }
                else
                {
                    var modelcpx1 = myapp.tbl_TaskCpx.Where(l => l.TaskCpxId == modelcpx.TaskCpxId).SingleOrDefault();
                    modelcpx1.CurrentApprovalLevel = 1;
                    //var approvers2 = myapp.tbl_TaskApprove.Where(l => l.TaskId == modelcpx.TaskCpxId && l.ApprovalLevel == "-1").ToList();
                    //if (approvers2.Count != 0)
                    //    modelcpx1.CurrentApprovalLevel = -1;
                    myapp.SaveChanges();
                    string ccemail = "";
                    string mobile = "";
                    string approvername = "";
                    int approverid = 0;
                    var approvallevelchk = modelcpx1.CurrentApprovalLevel.ToString();
                    var approvers = myapp.tbl_TaskApprove.Where(l => l.TaskId == modelcpx.TaskCpxId && l.ApprovalLevel == approvallevelchk).ToList();
                    foreach (var approver in approvers)
                    {
                        var user = myapp.tbl_User.Where(l => l.EmpId == approver.ApproveEmpId).ToList();
                        if (user.Count > 0)
                        {
                            if (user[0].EmailId != null && user[0].EmailId != "")
                                ccemail = user[0].EmailId + ",";
                            if (user[0].PhoneNumber != null && user[0].PhoneNumber != "")
                            {
                                mobile = user[0].PhoneNumber;
                            }
                            approvername = user[0].FirstName;
                        }
                        approverid = approver.ApproveEmpId.HasValue ? approver.ApproveEmpId.Value : 0;
                    }

                    SendEmailOnCpxForNextapprover(modelcpx.TaskCpxId, ccemail.TrimEnd(','), mobile, approvername, 1, approverid);
                }
            }
            return Json(modelcpx.TaskCpxId, JsonRequestBehavior.AllowGet);
        }
        public JsonResult VerifyAlreadyRequestedQty(string projectTitle, string department, string itemcode)
        {

            var model = myapp.tbl_TaskCpx.Where(l => l.ProjectTitle == projectTitle && l.CpxRelatedTo == department && l.ItemDetails == itemcode).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFullDetailsOfCpx(int id)
        {
            return Json(GetViewModel(id), JsonRequestBehavior.AllowGet);
        }
        public ActionResult UploadImage(int id, HttpPostedFileBase uploadDocument)
        {
            tbl_TaskCpxDoument doc = new tbl_TaskCpxDoument();
            var file = Path.GetFileNameWithoutExtension(uploadDocument.FileName);
            string FileName = Path.GetFileNameWithoutExtension(uploadDocument.FileName);

            //To Get File Extension  
            string FileExtension = Path.GetExtension(uploadDocument.FileName);

            //Add Current Date To Attached File Name  
            FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

            //Get Upload path from Web.Config file AppSettings.  
            string UploadPath = Server.MapPath("~/ExcelUplodes/");

            //Its Create complete path to store in server.  
            var ImagePath = UploadPath + FileName;

            //To copy and save file into server.  
            uploadDocument.SaveAs(ImagePath);

            doc.DocumentName = file;
            doc.DocumentPath = FileName;
            doc.TaskCpxId = id;
            doc.CreatedBy = User.Identity.Name;
            doc.CreatedOn = DateTime.Now;
            myapp.tbl_TaskCpxDoument.Add(doc);
            myapp.SaveChanges();
            return Json("Successfully Updated", JsonRequestBehavior.AllowGet);
        }
        public CpxViewModel GetViewModel(int id)
        {
            var modelcpx = myapp.tbl_TaskCpx.Where(l => l.TaskCpxId == id).SingleOrDefault();
            var modelloc = myapp.tbl_Location.Where(l => l.LocationId == modelcpx.CreatorLocationId).SingleOrDefault();
            var modeldept = myapp.tbl_Department.Where(l => l.DepartmentId == modelcpx.CreatorDepartmentId).SingleOrDefault();
            var modelcommondept = myapp.tbl_CommonDepartment.Where(l => l.CommonDepartmentId == modelcpx.RequestForDepartment).SingleOrDefault();
            var modeluser = myapp.tbl_User.Where(l => l.EmpId == modelcpx.CreatorId).SingleOrDefault();
            CpxViewModel cpxmodelfull = (new CpxViewModel
            {
                Budgeted = modelcpx.Budgeted.HasValue ? (modelcpx.Budgeted.Value == true ? "Yes" : "No") : "No",
                CreatorDepartmentId = modelcpx.CreatorDepartmentId.ToString(),
                CreatorDepartmentName = modeldept != null ? modeldept.DepartmentName : "",
                CreatorId = modelcpx.CreatorId.ToString(),
                CreatorLocationId = modelcpx.CreatorLocationId.ToString(),
                CreatorLocationName = modelloc != null ? modelloc.LocationName : "",
                CreatorName = modeluser != null ? modeluser.FirstName : "",
                CreatorEmail = modeluser != null ? modeluser.EmailId : "",
                EstimatedCost = modelcpx.EstimatedCost.HasValue ? modelcpx.EstimatedCost.Value : 0,
                IsNewItem = modelcpx.IsNewItem.HasValue ? (modelcpx.IsNewItem.Value == true ? "Yes" : "No") : "No",
                Item = modelcpx.Item,
                ItemDetails = modelcpx.ItemDetails,
                MainFeatures = modelcpx.MainFeatures,
                Model = modelcpx.Model,
                Priority = modelcpx.Prioriy,
                ProjectTitle = modelcpx.ProjectTitle,
                Qty = modelcpx.Qty.HasValue ? modelcpx.Qty.Value.ToString() : "0",
                RequestForDepartmentId = modelcpx.RequestForDepartment.HasValue ? modelcpx.RequestForDepartment.Value : 0,
                RequestForDepartment = modelcommondept != null ? modelcommondept.Name : "",
                TaskCpxId = modelcpx.TaskCpxId,
                CreatedDate = modelcpx.CreatedOn.HasValue ? modelcpx.CreatedOn.Value.ToString("dd/MM/yyyy") : "",
                CpxRelatedTo = modelcpx.CpxRelatedTo,
                CpxStatus = modelcpx.CpxStatus,
                WorkDoneComments = modelcpx.WorkDoneComments,
                CPXType = modelcpx.CPXType,
                AMCLastRenewalDate = modelcpx.AMCLastRenewalDate.HasValue ? ProjectConvert.ConverDateTimeToString(modelcpx.AMCLastRenewalDate.Value) : "",
                AMCNextRenewalDate = modelcpx.AMCNextRenewalDate.HasValue ? ProjectConvert.ConverDateTimeToString(modelcpx.AMCNextRenewalDate.Value) : "",
                SupportCompany = modelcpx.SupportCompany,
                ReasonForReplacement = modelcpx.ReasonForReplacement,
                OldProductModel = modelcpx.OldProductModel,
                OldProductCost = modelcpx.OldProductCost,
                CpxRelatedToOther = modelcpx.CpxRelatedToOther,
                CurrentApproverLevel = modelcpx.CurrentApprovalLevel.HasValue ? modelcpx.CurrentApprovalLevel.Value.ToString() : "0"
            });

            var approvers = myapp.tbl_TaskApprove.Where(l => l.TaskId == id).ToList();
            cpxmodelfull.ApprovalList = (from a in approvers
                                         join u in myapp.tbl_User on a.ApproveEmpId equals u.EmpId
                                         select new CpxApprovalViewModel
                                         {
                                             ApprovalLevel = a.ApprovalLevel,
                                             ApproveComments = a.ApproveComments,
                                             ApprovedDate = a.ApprovedDate.HasValue ? a.ApprovedDate.Value.ToString("dd/MM/yyyy") : "",
                                             ApproveEmpId = a.ApproveEmpId.Value,
                                             ApproveEmpName = u.FirstName,
                                             ApproveEmail = u.EmailId,
                                             IsApproved = a.IsApproved.HasValue ? a.IsApproved.Value.ToString() : "false"
                                         }).ToList();
            return cpxmodelfull;
        }
        public void SendEmailOnCpxForNextapprover(int id, string ccemail, string mobilenumber, string approvername = "", int approverlevel = 0, int approverid = 0)
        {
            CpxViewModel cpxmodelfull = GetViewModel(id);
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel
            {
                fromemail = "it_helpdesk@fernandez.foundation"
            };
            mailmodel.toemail = "capex@fernandez.foundation";
            //mailmodel.toemail = "ahmadali@fernandez.foundation";
            mailmodel.ccemail = ccemail.TrimEnd(',');
            //  mailmodel.subject = "Ticket " + task.TaskId + " " + task.Subject.Substring(0, 50) + " - " + task.CreatorLocationName + " " + task.CreatorDepartmentName;
            mailmodel.subject = "Cpx Request " + cpxmodelfull.TaskCpxId + " - " + cpxmodelfull.CreatorLocationName + " " + cpxmodelfull.CreatorDepartmentName + " " + cpxmodelfull.CreatorName + " " + cpxmodelfull.ProjectTitle;

            if (mailmodel.toemail != null && mailmodel.toemail != "")
            {
                string host = HttpContext.Request.Url.Host;
                string scheme = HttpContext.Request.Url.Scheme;
                string port = HttpContext.Request.Url.Port.ToString();
                string MainUrl = "";
                if (port != null && port != "")
                {
                    MainUrl = scheme + "://" + host + ":" + port;
                }
                else
                {
                    MainUrl = scheme + "://" + host;
                }

                string mailbody = "<p style='font-family:verdana;font-size:13px;'>Dear Sir, Please review and approve the CPX From </p>";
                mailbody += "<p style='font-family:verdana;font-size:12px;'>Cpx Details are: </p><table style='width:60%;margin:auto;border:solid 1px #a1a2a3;'>";
                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Task Id</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.TaskCpxId + "</td></tr>";
                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Requestor</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.CreatorLocationName + " " + cpxmodelfull.CreatorDepartmentName + " " + cpxmodelfull.CreatorName + "</td></tr>";
                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Request Date</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + DateTime.Now + "</td></tr>";
                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Project Title</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.ProjectTitle + "</td></tr>";
                if (cpxmodelfull.Item != null && cpxmodelfull.Item != "Select")
                {
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Item</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Item + "</td></tr>";
                }
                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Qty Required</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Qty + "</td></tr>";
                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Is New Item</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.IsNewItem + "</td></tr>";
                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Item Details</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.ItemDetails + "</td></tr>";
                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Make and Model</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Model + "</td></tr>";
                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Main Features</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.MainFeatures + "</td></tr>";
                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Estimated Cost</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.EstimatedCost + "</td></tr>";
                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Budgeted</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Budgeted + "</td></tr>";
                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Priority</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Priority + "</td></tr>";
                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Is Approved?</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>Pending For Approval</td></tr>";
                //if (approverlevel >= 2)
                //{
                mailbody += "<tr><td style='border-bottom:solid 1px #ddd;font-size:12px;color:green;text-align:center;font-family:verdana;' colspan='2'><a href='" + MainUrl + "/CPX/ApproveCpxRequest?id=" + cpxmodelfull.TaskCpxId + "&comments=Ok&approveid=" + approverid + "'>Click here to Approve</a></td></tr>";
                mailbody += "<tr><td style='border-bottom:solid 1px #ddd;font-size:12px;color:red;text-align:center;font-family:verdana;' colspan='2'><a href='" + MainUrl + "/CPX/RejectCpxRequest?id=" + cpxmodelfull.TaskCpxId + "&comments=Ok&approveid=" + approverid + "'>Click here to Reject</a></td></tr>";
                //}
                mailbody += "</table><br /><p style='font-family:verdana;font-size:12px;'>This is an auto generated mail,Don't reply to this.</p>";
                mailmodel.body = mailbody;
                mailmodel.filepath = "";
                mailmodel.fromname = "New Cpx Request Created";
                if (mailmodel.ccemail == null || mailmodel.ccemail == "")
                {
                    mailmodel.ccemail = "";
                    //if (cpxmodelfull.CpxRelatedTo == "IT")
                    //{
                    //    mailmodel.ccemail = "it@fernandez.foundation";
                    //}
                    //if (cpxmodelfull.CreatorEmail != null && cpxmodelfull.CreatorEmail != "")
                    //{
                    //    mailmodel.ccemail = cpxmodelfull.CreatorEmail;
                    //}
                }
                else
                {
                    //if (cpxmodelfull.CpxRelatedTo == "IT")
                    //{
                    //    mailmodel.ccemail = mailmodel.ccemail + ",it@fernandez.foundation";
                    //}
                    //if (cpxmodelfull.CreatorEmail != null && cpxmodelfull.CreatorEmail != "")
                    //{
                    //    mailmodel.ccemail = mailmodel.ccemail + "," + cpxmodelfull.CreatorEmail;
                    //}
                }


                cm.SendEmail(mailmodel);
            }
            if (mobilenumber != null && mobilenumber != "")
            {
                SendSms sms = new SendSms();
                string submob = "Dear " + approvername + ", you have a Capex request pending for approval, please login to infonet to approve the capex request.";
                sms.SendSmsToEmployee(mobilenumber, submob);
            }
        }

        public JsonResult ApproveCpxRequest(int id, string comments, string PreferredMakeModel = "", string ProductMainFeatures = "", string EstimatedCost = "", string Budgeted = "", string approveid = "")
        {
            var request = myapp.tbl_TaskCpx.Where(l => l.TaskCpxId == id).SingleOrDefault();
            var approver = myapp.tbl_TaskApprove.Where(l => l.TaskId == id).ToList();
            int CurrentUser = 0;
            bool Isnextapproval = false;
            if (approveid != "")
            {
                Isnextapproval = true;
                CurrentUser = int.Parse(approveid);
            }
            else
            {
                Isnextapproval = true;
                CurrentUser = int.Parse(User.Identity.Name);
            }
            approver = approver.Where(l => l.ApproveEmpId == CurrentUser).ToList();
            foreach (var approve in approver)
            {
                approve.ApprovedDate = DateTime.Now;
                approve.IsApproved = true;
                approve.ApproveComments = comments;
            }
            myapp.SaveChanges();

            if (PreferredMakeModel != null && PreferredMakeModel != "")
            {
                Isnextapproval = true;
                request.Model = PreferredMakeModel;
                myapp.SaveChanges();
            }
            if (ProductMainFeatures != null && ProductMainFeatures != "")
            {
                request.MainFeatures = ProductMainFeatures;
                myapp.SaveChanges();
            }
            if (EstimatedCost != null && EstimatedCost != "")
            {
                request.EstimatedCost = Convert.ToDecimal(EstimatedCost);
                myapp.SaveChanges();
            }
            if (Budgeted != null && Budgeted != "")
            {
                request.Budgeted = Budgeted == "Yes" ? true : false;
                myapp.SaveChanges();
            }
            if (Isnextapproval)
            {
                int checklocationid = request.CreatorLocationId.Value;
                if (request.ProjectTitle != null && request.ProjectTitle != "")
                {
                    var locations = myapp.tbl_Location.Where(l => l.LocationName == request.ProjectTitle).ToList();
                    if (locations.Count > 0)
                    {
                        checklocationid = locations[0].LocationId;
                    }
                }
                int relateddepartment = 0;
                switch (request.CpxRelatedTo)
                {
                    case "IT":
                        relateddepartment = 1;
                        break;
                    case "Biomedical":
                        relateddepartment = 2;
                        break;
                    case "Maintenance":
                        relateddepartment = 3;
                        break;
                    case "Maintenance Renovation":
                        relateddepartment = 11;
                        break;
                    case "Pharmacy":
                        relateddepartment = 4;
                        break;
                    case "Food & Beverage":
                        relateddepartment = 6;
                        break;
                    case "Brand & Communication":
                        relateddepartment = 7;
                        break;
                    case "OT":
                        relateddepartment = 8;
                        break;
                    case "Security and Transport":
                        relateddepartment = 9;
                        break;
                    case "HR":
                        relateddepartment = 18;
                        break;
                    case "House Keeping":
                        relateddepartment = 10;
                        break;
                    case "HICC":
                        relateddepartment = 12;
                        break;
                    case "Purchase":
                        relateddepartment = 13;
                        break;
                    case "OT-OBS":
                        relateddepartment = 14;
                        break;
                    case "OT-GYN":
                        relateddepartment = 15;
                        break;
                    case "OT-Others":
                        relateddepartment = 16;
                        break;
                    case "OT-Neonatal-and-Pediatric":
                        relateddepartment = 17;
                        break;
                    case "Vehicle":
                        relateddepartment = 19;
                        break;
                    case "Finance":
                        relateddepartment = 20;
                        break;
                    default:
                        relateddepartment = 5;
                        break;
                }
                //Approvers
                List<tbl_TaskApproverMaster> listofapprovers = myapp.tbl_TaskApproverMaster.Where(l => l.IsActive == true && (l.DepartmentId == relateddepartment || l.DepartmentId == 0) && l.LocationId == checklocationid && l.ApproverLevel > -1).ToList();
                if (request.CpxRelatedTo == "Biomedical")
                {
                    listofapprovers = listofapprovers.Where(l => l.DepartmentId == 0 || l.DepartmentId == 2).ToList();
                }
                else
                {
                    listofapprovers = listofapprovers.Where(l => l.DepartmentId != 2).ToList();
                }
                foreach (var approver1 in listofapprovers)
                {
                    bool saveapprover = false;
                    if (approver1.Operator == "lessthan" && request.EstimatedCost < approver1.Amount)
                    {
                        saveapprover = true;
                    }
                    if (approver1.Operator == "GraterThan" && request.EstimatedCost > approver1.Amount)
                    {
                        saveapprover = true;
                    }
                    if (approver1.Operator == "EqualTo" && approver1.Amount == request.EstimatedCost)
                    {
                        saveapprover = true;
                    }
                    if (approver1.Operator == "Both")
                    {
                        saveapprover = true;
                    }
                    if (relateddepartment == 5)
                    {
                        if (approver1.DepartmentId == 0 && approver1.ApproverLevel < 5)
                        {
                            saveapprover = false;
                        }
                    }

                    if (saveapprover)
                    {

                        tbl_TaskApprove modelapprove = new tbl_TaskApprove();
                        modelapprove.ApprovalLevel = approver1.ApproverLevel.ToString();
                        modelapprove.ApproveComments = "";
                        modelapprove.ApproveEmpId = approver1.UserId;
                        modelapprove.IsApproved = false;
                        modelapprove.TaskId = request.TaskCpxId;
                        var checkapprover = myapp.tbl_TaskApprove.Where(l => l.TaskId == request.TaskCpxId && l.ApproveEmpId == approver1.UserId && l.ApprovalLevel == modelapprove.ApprovalLevel).Count();
                        if (checkapprover == 0)
                        {
                            myapp.tbl_TaskApprove.Add(modelapprove);
                            myapp.SaveChanges();
                        }

                    }
                }
            }
            try
            {
                var approverslist = myapp.tbl_TaskApprove.Where(l => l.TaskId == id && l.IsApproved == true).Select(l => l.ApproveEmpId).ToList();
                //Check repeated User as approval and then set as approved
                var listcheckapp = myapp.tbl_TaskApprove.Where(l => l.TaskId == id && l.IsApproved == false && approverslist.Contains(l.ApproveEmpId)).ToList();
                if (listcheckapp.Count > 0)
                {
                    foreach (var lcapp in listcheckapp)
                    {
                        var listcheckapp2 = myapp.tbl_TaskApprove.Where(l => l.TaskId == id && l.IsApproved == false
                        && l.ApproveEmpId == lcapp.ApproveEmpId).ToList();
                        if (listcheckapp2.Count > 0)
                        {
                            foreach (var sublcap in listcheckapp2)
                            {
                                sublcap.IsApproved = true;
                                sublcap.ApprovedDate = DateTime.Now;
                                sublcap.ApproveComments = lcapp.ApproveComments;
                                myapp.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch { }


            var checkStatus = myapp.tbl_TaskApprove.Where(l => l.TaskId == id && l.IsApproved == false).ToList();
            if (checkStatus.Count == 0)
            {
                request.IsApproved = true;
                request.CpxStatus = "Approved";
                myapp.SaveChanges();
            }
            else
            {
                checkStatus = checkStatus.OrderBy(l => int.Parse(l.ApprovalLevel)).ToList();
                string ccemail = "";
                string mobile = "";
                string approvername = "";
                int nextapproverlevel = 0;
                int approverid = 0;
                foreach (var approvem in checkStatus)
                {
                    var user = myapp.tbl_User.Where(l => l.EmpId == approvem.ApproveEmpId).ToList();
                    if (user.Count > 0)
                    {
                        if (user[0].EmailId != null && user[0].EmailId != "")
                            ccemail = user[0].EmailId + ",";
                        if (user[0].PhoneNumber != null && user[0].PhoneNumber != "")
                        {
                            mobile = user[0].PhoneNumber;
                        }
                        approvername = user[0].FirstName;
                    }

                    request.CurrentApprovalLevel = int.Parse(approvem.ApprovalLevel);
                    nextapproverlevel = request.CurrentApprovalLevel.Value;
                    approverid = approvem.ApproveEmpId.HasValue ? approvem.ApproveEmpId.Value : 0;
                    request.CpxStatus = "In Progress";
                    myapp.SaveChanges();
                    break;
                }
                ccemail = ccemail.TrimEnd(',');
                SendEmailOnCpxForNextapprover(id, ccemail, mobile, approvername, nextapproverlevel, approverid);
                //
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ConfirmCpxRequest(int id, string comments)
        {
            var request = myapp.tbl_TaskCpx.Where(l => l.TaskCpxId == id).SingleOrDefault();
            request.CpxStatus = "Closed";
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult RejectCpxRequest(int id, string comments, string approveid = "")
        {
            var request = myapp.tbl_TaskCpx.Where(l => l.TaskCpxId == id).SingleOrDefault();
            var approver = myapp.tbl_TaskApprove.Where(l => l.TaskId == id).ToList();
            int CurrentUser = 0;
            if (approveid != "")
            {
                CurrentUser = int.Parse(approveid);
            }
            else
            {
                CurrentUser = int.Parse(User.Identity.Name);
            }
            approver = approver.Where(l => l.ApproveEmpId == CurrentUser).ToList();
            foreach (var approve in approver)
            {
                approve.ApprovedDate = DateTime.Now;
                approve.IsApproved = false;
                approve.ApproveComments = comments;
            }
            myapp.SaveChanges();
            //var checkStatus = myapp.tbl_TaskApprove.Where(l => l.TaskId == id && l.IsApproved == false).ToList();
            //if (checkStatus.Count == 0)
            //{
            request.CpxStatus = "Rejected";
            myapp.SaveChanges();
            //}
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            if (Request.Form["id"] != null && Request.Form["id"] != "")
            {
                string Id = Request.Form["id"].ToString();
                string checkpublic = Request.Form["public"].ToString();
                if (Request.Files.Count > 0)
                {
                    try
                    {
                        HttpFileCollectionBase files = Request.Files;
                        for (int i = 0; i < files.Count; i++)
                        {
                            HttpPostedFileBase file = files[i];
                            string fname;
                            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                            {
                                string[] testfiles = file.FileName.Split(new char[] { '\\' });
                                fname = testfiles[testfiles.Length - 1];
                            }
                            else
                            {
                                fname = file.FileName;
                            }
                            string fileName = System.IO.Path.GetFileName(file.FileName);
                            string guidid = Guid.NewGuid().ToString();
                            string path = System.IO.Path.Combine(Server.MapPath("~/ExcelUplodes/"), guidid + fileName);
                            file.SaveAs(path);
                            tbl_TaskCpxDoument tsk = new tbl_TaskCpxDoument
                            {
                                CreatedBy = User.Identity.Name,
                                CreatedOn = DateTime.Now,
                                DocumentName = fileName,
                                DocumentPath = guidid + fileName,
                                IsPrivate = checkpublic == "true" ? true : false,
                                TaskCpxId = int.Parse(Id)
                            };
                            myapp.tbl_TaskCpxDoument.Add(tsk);
                            myapp.SaveChanges();
                        }
                        // Returns message that successfully uploaded  
                        return Json("File Uploaded Successfully!");
                    }
                    catch (Exception ex)
                    {
                        return Json("Error occurred. Error details: " + ex.Message);
                    }
                }
                else
                {
                    return Json("No files selected.");
                }
            }
            else
            {
                return Json("Please select Id");
            }
        }

        public JsonResult GetListOfFiles(int id)
        {
            List<tbl_TaskCpxDoument> list = myapp.tbl_TaskCpxDoument.Where(l => l.TaskCpxId == id).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ApproveCpxRequestByPurchase(int id, string comments)
        {
            var request = myapp.tbl_TaskCpx.Where(l => l.TaskCpxId == id).SingleOrDefault();
            request.CpxStatus = "Purchase - In Progress";
            request.WorkDoneComments = comments;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult CloseCpxRequestByPurchase(int id, string comments)
        {
            var request = myapp.tbl_TaskCpx.Where(l => l.TaskCpxId == id).SingleOrDefault();
            request.WorkDoneComments = comments;
            request.CpxStatus = "Pending at Requestor";
            request.IsClosed = true;
            myapp.SaveChanges();

            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel
            {
                fromemail = "it_helpdesk@fernandez.foundation"
            };
            var useremail = myapp.tbl_User.Where(l => l.EmpId == request.CreatorId).SingleOrDefault();
            if (useremail.EmailId != null && useremail.EmailId != "")
            {
                CpxViewModel cpxmodelfull = GetViewModel(id);
                //mailmodel.toemail = "capex@fernandez.foundation";
                mailmodel.toemail = useremail.EmailId;
                mailmodel.ccemail = "capex@fernandez.foundation";
                //  mailmodel.subject = "Ticket " + task.TaskId + " " + task.Subject.Substring(0, 50) + " - " + task.CreatorLocationName + " " + task.CreatorDepartmentName;
                mailmodel.subject = "Cpx Request " + cpxmodelfull.TaskCpxId + " - " + cpxmodelfull.CreatorLocationName + " " + cpxmodelfull.CreatorDepartmentName + " " + cpxmodelfull.CreatorName + " " + cpxmodelfull.ProjectTitle;

                if (mailmodel.toemail != null && mailmodel.toemail != "")
                {
                    string mailbody = "<p style='font-family:verdana;font-size:15px;'>Dear " + cpxmodelfull.CreatorName + ", Your CPX request has been closed.</p>";
                    mailbody += "<p style='font-family:verdana;font-size:14px;'>Cpx Details are: </p><table style='width:60%;margin:auto;border:solid 1px #a1a2a3;'>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Task Id</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.TaskCpxId + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Requestor</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.CreatorLocationName + " " + cpxmodelfull.CreatorDepartmentName + " " + cpxmodelfull.CreatorName + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Request Date</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + DateTime.Now + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Project Title</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.ProjectTitle + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Item</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Item + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Qty Required</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Qty + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Is New Item</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.IsNewItem + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Item Details</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.ItemDetails + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Make and Model</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Model + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Main Features</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.MainFeatures + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Estimated Cost</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.EstimatedCost + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Budgeted</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Budgeted + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Priority</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Priority + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Comments</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + comments + "</td></tr>";
                    mailbody += "</table><br /><p style='font-family:verdana;font-size:12px;'>This is an auto generated mail,Don't reply to this.</p>";
                    mailmodel.body = mailbody;
                    mailmodel.filepath = "";
                    mailmodel.fromname = "Your CPX request has been closed";
                    if (mailmodel.ccemail == null || mailmodel.ccemail == "")
                    {
                        mailmodel.ccemail = "";
                    }

                    cm.SendEmail(mailmodel);
                }
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetApproversList(int id)
        {
            List<tbl_TaskApprove> approver = myapp.tbl_TaskApprove.Where(l => l.TaskId == id).ToList();

            var model = (from m in approver
                         join u in myapp.tbl_User on m.ApproveEmpId equals u.EmpId
                         let ApprovedDate = (m.ApprovedDate.HasValue ? m.ApprovedDate.Value.ToString("dd/MM/yyyy hh:mm tt") : "")
                         let ApprovedDate2 = m.ApprovedDate
                         select new
                         {
                             m.ApproveComments,
                             ApprovedDate,
                             ApprovedDate2,
                             m.IsApproved,
                             m.ApprovalLevel,
                             u.FirstName,
                             u.DepartmentName
                         }).ToList();
            var m1 = (from l in model where l.ApprovedDate2 != null select l).OrderBy(l => l.ApprovedDate2).ToList();

            var m2 = (from l in model where l.ApprovedDate2 == null select l).OrderBy(l => int.Parse(l.ApprovalLevel)).ToList();
            m1.AddRange(m2);
            return Json(m1, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintCpx(int id)
        {
            CpxViewModel cpxmodelfull = GetViewModel(id);
            return View(cpxmodelfull);
        }
        public ActionResult SendEmailForApproval()
        {
            var cpxrequests = myapp.tbl_TaskCpx.Where(l => l.IsApproved == false && l.CpxStatus != "Rejected" && l.CpxStatus != "Closed" && l.CpxStatus != "Approved").ToList();
            foreach (var request in cpxrequests)
            {
                string approverlevel = request.CurrentApprovalLevel.ToString();
                //Approvers
                var approver = myapp.tbl_TaskApprove.Where(l => l.TaskId == request.TaskCpxId && l.ApprovalLevel == approverlevel && l.IsApproved == false).ToList();
                foreach (var approvercheck in approver)
                {
                    CustomModel cm = new CustomModel();
                    MailModel mailmodel = new MailModel
                    {
                        fromemail = "it_helpdesk@fernandez.foundation"
                    };
                    var useremail = myapp.tbl_User.Where(l => l.EmpId == approvercheck.ApproveEmpId).SingleOrDefault();
                    if (useremail.EmailId != null && useremail.EmailId != "")
                    {
                        CpxViewModel cpxmodelfull = GetViewModel(request.TaskCpxId);
                        //mailmodel.toemail = "capex@fernandez.foundation";
                        mailmodel.toemail = useremail.EmailId;
                        mailmodel.ccemail = "capex@fernandez.foundation";
                        //  mailmodel.subject = "Ticket " + task.TaskId + " " + task.Subject.Substring(0, 50) + " - " + task.CreatorLocationName + " " + task.CreatorDepartmentName;
                        mailmodel.subject = "Cpx Request " + cpxmodelfull.TaskCpxId + " - " + cpxmodelfull.CreatorLocationName + " " + cpxmodelfull.CreatorDepartmentName + " " + cpxmodelfull.CreatorName + " " + cpxmodelfull.ProjectTitle;

                        if (mailmodel.toemail != null && mailmodel.toemail != "")
                        {
                            string host = HttpContext.Request.Url.Host;
                            string scheme = HttpContext.Request.Url.Scheme;
                            string port = HttpContext.Request.Url.Port.ToString();
                            string MainUrl = "";
                            if (port != null && port != "")
                            {
                                MainUrl = scheme + "://" + host + ":" + port;
                            }
                            else
                            {
                                MainUrl = scheme + "://" + host;
                            }
                            string mailbody = "<p style='font-family:verdana;font-size:13px;'>Dear " + useremail.FirstName + ", The CPX request has waiting for your approval.</p>";
                            mailbody += "<p style='font-family:verdana;font-size:12px;'>Cpx Details are: </p><table style='width:65%;margin:auto;border:solid 1px #a1a2a3;'>";
                            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Task Id</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.TaskCpxId + "</td></tr>";
                            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Requestor</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.CreatorLocationName + " " + cpxmodelfull.CreatorDepartmentName + " " + cpxmodelfull.CreatorName + "</td></tr>";
                            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Request Date</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + DateTime.Now + "</td></tr>";
                            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Project Title</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.ProjectTitle + "</td></tr>";
                            if (cpxmodelfull.Item != null && cpxmodelfull.Item != "Select")
                            {
                                mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Item</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Item + "</td></tr>";
                            }
                            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Qty Required</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Qty + "</td></tr>";
                            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Is New Item</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.IsNewItem + "</td></tr>";
                            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Item Details</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.ItemDetails + "</td></tr>";
                            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Make and Model</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Model + "</td></tr>";
                            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Main Features</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.MainFeatures + "</td></tr>";
                            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Estimated Cost</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.EstimatedCost + "</td></tr>";
                            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Budgeted</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Budgeted + "</td></tr>";
                            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Priority</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Priority + "</td></tr>";
                            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Is Approved?</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>Pending For Approval</td></tr>";
                            //if (int.Parse(approvercheck.ApprovalLevel) >= 2)
                            //{
                            mailbody += "<tr><td style='border-bottom:solid 1px #ddd;font-size:12px;color:green;text-align:center;font-family:verdana;' colspan='2'><a href='" + MainUrl + "/CPX/ApproveCpxRequest?id=" + cpxmodelfull.TaskCpxId + "&comments=Ok&approveid=" + approvercheck.ApproveEmpId + "'>Click here to Approve</a></td></tr>";
                            mailbody += "<tr><td style='border-bottom:solid 1px #ddd;font-size:12px;color:red;text-align:center;font-family:verdana;' colspan='2'><a href='" + MainUrl + "/CPX/RejectCpxRequest?id=" + cpxmodelfull.TaskCpxId + "&comments=Ok&approveid=" + approvercheck.ApproveEmpId + "'>Click here to Reject</a></td></tr>";
                            //}
                            mailbody += "</table><br /><p style='font-family:verdana;font-size:12px;'>This is an auto generated mail,Don't reply to this.</p>";
                            mailmodel.body = mailbody;
                            mailmodel.filepath = "";
                            mailmodel.fromname = "Cpx Request for approval";
                            if (mailmodel.ccemail == null || mailmodel.ccemail == "")
                            {
                                mailmodel.ccemail = "";
                            }

                            cm.SendEmail(mailmodel);
                        }
                    }
                    break;
                }
            }
            return Json("Suscess", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportExcel_CPX()
        {
            var listofapprovers = myapp.tbl_TaskCpx.Where(l => l.IsApproved == true).ToList();
            List<CpxViewModel> tasks = (from m in listofapprovers
                                        join l in myapp.tbl_Location on m.CreatorLocationId equals l.LocationId
                                        join d in myapp.tbl_Department on m.CreatorDepartmentId equals d.DepartmentId
                                        join dc in myapp.tbl_CommonDepartment on m.RequestForDepartment equals dc.CommonDepartmentId
                                        join u in myapp.tbl_User on m.CreatorId equals u.EmpId
                                        //let approv = myapp.tbl_TaskApprove.Where(a => a.TaskId == m.TaskId && Convert.ToInt32(a.ApprovalLevel) == m.CurrentApprovalLevel).SingleOrDefault()
                                        select new CpxViewModel
                                        {
                                            Budgeted = m.Budgeted.HasValue ? (m.Budgeted.Value == true ? "Yes" : "No") : "No",
                                            CreatorDepartmentId = m.CreatorDepartmentId.ToString(),
                                            CreatorDepartmentName = d.DepartmentName,
                                            CreatorId = m.CreatorId.ToString(),
                                            CreatorLocationId = m.CreatorLocationId.ToString(),
                                            CreatorLocationName = l.LocationName,
                                            CreatorName = u.FirstName,
                                            EstimatedCost = m.EstimatedCost.HasValue ? m.EstimatedCost.Value : 0,
                                            IsNewItem = m.IsNewItem.HasValue ? (m.IsNewItem.Value == true ? "Yes" : "No") : "No",
                                            Item = m.Item,
                                            ItemDetails = m.ItemDetails,
                                            MainFeatures = m.MainFeatures,
                                            Model = m.Model,
                                            Priority = m.Prioriy,
                                            ProjectTitle = m.ProjectTitle,
                                            Qty = m.Qty.HasValue ? m.Qty.Value.ToString() : "0",
                                            RequestForDepartment = dc.Name,
                                            TaskCpxId = m.TaskCpxId,
                                            CpxStatus = m.CpxStatus,
                                            CurrentApproverLevel = m.CurrentApprovalLevel.HasValue ? m.CurrentApprovalLevel.Value.ToString() : "0",
                                            CurrentApprover = "",
                                            CreatedDate = m.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                            WorkDoneComments = m.WorkDoneComments,
                                            CpxRelatedTo = m.CpxRelatedTo,
                                            CpxRelatedToOther = m.CpxRelatedToOther,
                                            CPXType = m.CPXType,
                                            SupportCompany = m.SupportCompany,
                                            ReasonForReplacement = m.ReasonForReplacement,
                                            OldProductCost = m.OldProductCost,
                                            OldProductModel = m.OldProductModel,
                                            AMCLastRenewalDate = m.AMCLastRenewalDate.HasValue ? m.AMCLastRenewalDate.Value.ToString("dd/MM/yyyy") : "",
                                            AMCNextRenewalDate = m.AMCNextRenewalDate.HasValue ? m.AMCNextRenewalDate.Value.ToString("dd/MM/yyyy") : ""
                                        }).ToList();
            System.Data.DataTable products = new System.Data.DataTable("EmployeesLogin");
            products.Columns.Add("CpxId", typeof(string));
            products.Columns.Add("Cpx Type", typeof(string));
            products.Columns.Add("Related To", typeof(string));
            products.Columns.Add("Created Date", typeof(string));
            products.Columns.Add("Location", typeof(string));

            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("Creator", typeof(string));
            products.Columns.Add("Cpx Related To", typeof(string));
            products.Columns.Add("Project Title", typeof(string));

            products.Columns.Add("Cpx Status", typeof(string));
            products.Columns.Add("Request For Department", typeof(string));
            products.Columns.Add("Item", typeof(string));

            products.Columns.Add("ItemDetails", typeof(string));
            products.Columns.Add("MainFeatures", typeof(string));
            products.Columns.Add("Model", typeof(string));

            products.Columns.Add("Priority", typeof(string));
            products.Columns.Add("Qty", typeof(string));
            products.Columns.Add("EstimatedCost", typeof(string));
            products.Columns.Add("WorkDoneComments", typeof(string));
            products.Columns.Add("Budgeted", typeof(string));
            products.Columns.Add("CurrentApprover", typeof(string));
            products.Columns.Add("SupportCompany", typeof(string));
            products.Columns.Add("ReasonForReplacement", typeof(string));
            products.Columns.Add("OldProductCost", typeof(string));
            products.Columns.Add("OldProductModel", typeof(string));
            products.Columns.Add("AMCLastRenewalDate", typeof(string));
            products.Columns.Add("AMCNextRenewalDate", typeof(string));
            foreach (CpxViewModel c in tasks)
            {
                products.Rows.Add(c.TaskCpxId.ToString(),
                     c.CPXType,
                    c.CpxRelatedTo == "Others" ? c.CpxRelatedToOther : c.CpxRelatedTo,
                                 c.CreatedDate,
                                 c.CreatorLocationName,
                                 c.CreatorDepartmentName,
                                 c.CreatorName,
                                 c.CpxRelatedTo,
                                 c.ProjectTitle,
                                 c.CpxStatus,
                                 c.RequestForDepartment, c.Item, c.ItemDetails, c.MainFeatures, c.Model,
                                 c.Priority,
                                 c.Qty,
                                 c.EstimatedCost.ToString(),
                                 c.WorkDoneComments,
                                 c.Budgeted,
                                 GetCurrentApproverName(c.TaskCpxId, c.CurrentApproverLevel),
                    c.SupportCompany,
                    c.ReasonForReplacement,
                     c.OldProductCost,
                     c.OldProductModel,
                     c.AMCLastRenewalDate,
                     c.AMCNextRenewalDate

                );
            }

            GridView grid = new GridView
            {
                GridLines = GridLines.Both,
                BorderStyle = BorderStyle.Solid
            };
            grid.HeaderStyle.BackColor = System.Drawing.Color.Teal;
            grid.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            string filename = "AllCPXs.xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + filename);
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return View("MyView");

        }
        public JsonResult ExportExcelCPX(string fromDate, string toDate, string type, int id = 0, int locationId = 0, int departmentId = 0)
        {
            List<tbl_TaskCpx> listofapprovers = new List<tbl_TaskCpx>();
            if (type != "All")
                listofapprovers = myapp.tbl_TaskCpx.Where(l => l.IsApproved == true).ToList();
            else
                listofapprovers = myapp.tbl_TaskCpx.ToList();
            if (locationId != null && locationId != 0)
            {
                listofapprovers = listofapprovers.Where(m => m.CreatorLocationId == locationId).ToList();
            }
            if (departmentId != null && departmentId != 0)
            {
                listofapprovers = listofapprovers.Where(m => m.CreatorDepartmentId == departmentId).ToList();
            }
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                listofapprovers = listofapprovers.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            List<CpxViewModel> tasks = (from m in listofapprovers
                                        join l in myapp.tbl_Location on m.CreatorLocationId equals l.LocationId
                                        join d in myapp.tbl_Department on m.CreatorDepartmentId equals d.DepartmentId
                                        join dc in myapp.tbl_CommonDepartment on m.RequestForDepartment equals dc.CommonDepartmentId
                                        join u in myapp.tbl_User on m.CreatorId equals u.EmpId
                                        select new CpxViewModel
                                        {
                                            Budgeted = m.Budgeted.HasValue ? (m.Budgeted.Value == true ? "Yes" : "No") : "No",
                                            CreatorDepartmentId = m.CreatorDepartmentId.ToString(),
                                            CreatorDepartmentName = d.DepartmentName,
                                            CreatorId = m.CreatorId.ToString(),
                                            CreatorLocationId = m.CreatorLocationId.ToString(),
                                            CreatorLocationName = l.LocationName,
                                            CreatorName = u.FirstName,
                                            EstimatedCost = m.EstimatedCost.HasValue ? m.EstimatedCost.Value : 0,
                                            IsNewItem = m.IsNewItem.HasValue ? (m.IsNewItem.Value == true ? "Yes" : "No") : "No",
                                            Item = m.Item,
                                            ItemDetails = m.ItemDetails,
                                            MainFeatures = m.MainFeatures,
                                            Model = m.Model,
                                            Priority = m.Prioriy,
                                            ProjectTitle = m.ProjectTitle,
                                            Qty = m.Qty.HasValue ? m.Qty.Value.ToString() : "0",
                                            RequestForDepartment = dc.Name,
                                            TaskCpxId = m.TaskCpxId,
                                            CpxStatus = m.CpxStatus,
                                            CurrentApproverLevel = m.CurrentApprovalLevel.HasValue ? m.CurrentApprovalLevel.Value.ToString() : "0",
                                            CurrentApprover = "",
                                            CreatedDate = m.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                            WorkDoneComments = m.WorkDoneComments,
                                            CpxRelatedTo = m.CpxRelatedTo,
                                            CpxRelatedToOther = m.CpxRelatedToOther,
                                            CPXType = m.CPXType,
                                            SupportCompany = m.SupportCompany,
                                            ReasonForReplacement = m.ReasonForReplacement,
                                            OldProductCost = m.OldProductCost,
                                            OldProductModel = m.OldProductModel,
                                            AMCLastRenewalDate = m.AMCLastRenewalDate.HasValue ? m.AMCLastRenewalDate.Value.ToString("dd/MM/yyyy") : "",
                                            AMCNextRenewalDate = m.AMCNextRenewalDate.HasValue ? m.AMCNextRenewalDate.Value.ToString("dd/MM/yyyy") : ""
                                        }).ToList();
            tasks = tasks.OrderByDescending(t => t.TaskCpxId).ToList();
            var products = new System.Data.DataTable("CPX");
            products.Columns.Add("Cpx Id", typeof(string));
            products.Columns.Add("Cpx Type", typeof(string));
            products.Columns.Add("Related To", typeof(string));
            products.Columns.Add("Cpx Date", typeof(string));
            products.Columns.Add("Location - Dept", typeof(string));
            products.Columns.Add("User", typeof(string));
            products.Columns.Add("project", typeof(string));
            products.Columns.Add("Req Department", typeof(string));
            products.Columns.Add("Item", typeof(string));
            //products.Columns.Add("AssignTo", typeof(string));
            products.Columns.Add("Qty", typeof(string));
            products.Columns.Add("Estimated Cost", typeof(string));
            products.Columns.Add("Budgeted", typeof(string));
            products.Columns.Add("Is New Item", typeof(string));
            products.Columns.Add("Priority", typeof(string));
            products.Columns.Add("Status", typeof(string));
            products.Columns.Add("CurrentApprover", typeof(string));
            products.Columns.Add("SupportCompany", typeof(string));
            products.Columns.Add("ReasonForReplacement", typeof(string));
            products.Columns.Add("OldProductCost", typeof(string));
            products.Columns.Add("OldProductModel", typeof(string));
            products.Columns.Add("AMCLastRenewalDate", typeof(string));
            products.Columns.Add("AMCNextRenewalDate", typeof(string));
            foreach (var c in tasks)
            {
                products.Rows.Add(
                    Convert.ToString(c.TaskCpxId),
                    c.CPXType,
                    c.CpxRelatedTo == "Others" ? c.CpxRelatedToOther : c.CpxRelatedTo,
                    c.CreatedDate,
                    c.CreatorLocationName + " " + c.CreatorDepartmentName,
                    c.CreatorName,
                    c.ProjectTitle,
                    c.RequestForDepartment,
                    c.Item,
                    c.Qty,
                    c.EstimatedCost.ToString(),
                    c.Budgeted,
                    c.IsNewItem,
                    c.Priority,
                    c.CpxStatus,
                    GetCurrentApproverName(c.TaskCpxId, c.CurrentApproverLevel),
                    c.SupportCompany,
                    c.ReasonForReplacement,
                     c.OldProductCost,
                     c.OldProductModel,
                     c.AMCLastRenewalDate,
                     c.AMCNextRenewalDate
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=PM.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ManageCPXApprovers(int id)
        {
            List<tbl_TaskApprove> approver = myapp.tbl_TaskApprove.Where(l => l.TaskId == id).ToList();
            var model = (from m in approver
                         join u in myapp.tbl_User on m.ApproveEmpId equals u.EmpId
                         let ApprovedDate = (m.ApprovedDate.HasValue ? m.ApprovedDate.Value.ToString("dd/MM/yyyy") : "")
                         select new TaskApproveViewModel
                         {
                             TaskApproveId = m.TaskApproveId,
                             ApproveComments = m.ApproveComments,
                             ApprovedDate = ApprovedDate,
                             IsApproved = m.IsApproved,
                             ApprovalLevel = m.ApprovalLevel,
                             FirstName = u.FirstName,
                             DepartmentName = u.DepartmentName,
                             EmpId = u.EmpId,

                         }).ToList();
            model = model.OrderBy(l => l.ApprovalLevel).ToList();
            ViewBag.TaskId = id;
            return View(model);
        }
        public ActionResult DeleteTaskApprovers(int id)
        {
            var approver = myapp.tbl_TaskApprove.Where(l => l.TaskApproveId == id).SingleOrDefault();
            myapp.tbl_TaskApprove.Remove(approver);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateTaskApprovers(int id, int empid)
        {
            var user = myapp.tbl_User.Where(l => l.UserId == empid).SingleOrDefault();
            var approver = myapp.tbl_TaskApprove.Where(l => l.TaskApproveId == id).SingleOrDefault();
            approver.ApproveEmpId = user.EmpId;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveTaskApprovers(int id, int empid, int approverlevl)
        {
            var user = myapp.tbl_User.Where(l => l.UserId == empid).SingleOrDefault();
            tbl_TaskApprove model = new tbl_TaskApprove();
            model.ApprovalLevel = approverlevl.ToString();
            model.ApproveComments = "";
            model.ApproveEmpId = user.EmpId;
            model.TaskId = id;
            model.IsApproved = false;
            myapp.tbl_TaskApprove.Add(model);
            myapp.SaveChanges();
            List<tbl_TaskApprove> approver = myapp.tbl_TaskApprove.Where(l => l.TaskId == id && l.IsApproved == false).ToList();
            if (approver.Count > 0)
            {
                approver = approver.OrderBy(l => int.Parse(l.ApprovalLevel)).ToList();
                var model2 = myapp.tbl_TaskCpx.Where(l => l.TaskCpxId == id).SingleOrDefault();
                model2.CurrentApprovalLevel = int.Parse(approver[0].ApprovalLevel);
                myapp.SaveChanges();
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExportExcelCPXReport(string fromDate, string toDate, string type, int id = 0, int locationId = 0, int departmentId = 0)
        {
            List<tbl_TaskCpx> listofapprovers = myapp.tbl_TaskCpx.ToList();

            if (locationId != null && locationId != 0)
            {
                listofapprovers = listofapprovers.Where(m => m.CreatorLocationId == locationId).ToList();
            }
            if (departmentId != null && departmentId != 0)
            {
                listofapprovers = listofapprovers.Where(m => m.CreatorDepartmentId == departmentId).ToList();
            }
            if (fromDate != null && fromDate != "" && toDate != null && toDate != "")
            {
                var FromDate = ProjectConvert.ConverDateStringtoDatetime(fromDate);
                var ToDate = ProjectConvert.ConverDateStringtoDatetime(toDate);
                listofapprovers = listofapprovers.Where(x => x.CreatedOn.Value.Date >= FromDate.Date).Where(x => x.CreatedOn.Value.Date <= ToDate.Date).ToList();
            }
            List<CpxViewModel> tasks = (from m in listofapprovers
                                        join l in myapp.tbl_Location on m.CreatorLocationId equals l.LocationId
                                        join d in myapp.tbl_Department on m.CreatorDepartmentId equals d.DepartmentId
                                        join dc in myapp.tbl_CommonDepartment on m.RequestForDepartment equals dc.CommonDepartmentId
                                        join u in myapp.tbl_User on m.CreatorId equals u.EmpId
                                        select new CpxViewModel
                                        {
                                            Budgeted = m.Budgeted.HasValue ? (m.Budgeted.Value == true ? "Yes" : "No") : "No",
                                            CreatorDepartmentId = m.CreatorDepartmentId.ToString(),
                                            CreatorDepartmentName = d.DepartmentName,
                                            CreatorId = m.CreatorId.ToString(),
                                            CreatorLocationId = m.CreatorLocationId.ToString(),
                                            CreatorLocationName = l.LocationName,
                                            CreatorName = u.FirstName,
                                            EstimatedCost = m.EstimatedCost.HasValue ? m.EstimatedCost.Value : 0,
                                            IsNewItem = m.IsNewItem.HasValue ? (m.IsNewItem.Value == true ? "Yes" : "No") : "No",
                                            Item = m.Item,
                                            ItemDetails = m.ItemDetails,
                                            MainFeatures = m.MainFeatures,
                                            Model = m.Model,
                                            Priority = m.Prioriy,
                                            ProjectTitle = m.ProjectTitle,
                                            Qty = m.Qty.HasValue ? m.Qty.Value.ToString() : "0",
                                            RequestForDepartment = dc.Name,
                                            TaskCpxId = m.TaskCpxId,
                                            CpxStatus = m.CpxStatus,
                                            CurrentApproverLevel = m.CurrentApprovalLevel.HasValue ? m.CurrentApprovalLevel.Value.ToString() : "0",
                                            CurrentApprover = "",
                                            CreatedDate = m.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                            WorkDoneComments = m.WorkDoneComments,
                                            CpxRelatedTo = m.CpxRelatedTo,
                                            CpxRelatedToOther = m.CpxRelatedToOther,
                                            CPXType = m.CPXType,
                                            SupportCompany = m.SupportCompany,
                                            ReasonForReplacement = m.ReasonForReplacement,
                                            OldProductCost = m.OldProductCost,
                                            OldProductModel = m.OldProductModel,
                                            AMCLastRenewalDate = m.AMCLastRenewalDate.HasValue ? m.AMCLastRenewalDate.Value.ToString("dd/MM/yyyy") : "",
                                            AMCNextRenewalDate = m.AMCNextRenewalDate.HasValue ? m.AMCNextRenewalDate.Value.ToString("dd/MM/yyyy") : ""
                                        }).ToList();
            tasks = tasks.OrderByDescending(t => t.TaskCpxId).ToList();
            var products = new System.Data.DataTable("CPX");
            products.Columns.Add("Cpx Id", typeof(string));
            products.Columns.Add("Cpx Type", typeof(string));
            products.Columns.Add("Related To", typeof(string));
            products.Columns.Add("Cpx Date", typeof(string));
            products.Columns.Add("Location - Dept", typeof(string));
            products.Columns.Add("User", typeof(string));
            products.Columns.Add("project", typeof(string));
            products.Columns.Add("Req Department", typeof(string));
            products.Columns.Add("Item", typeof(string));
            //products.Columns.Add("AssignTo", typeof(string));
            products.Columns.Add("Qty", typeof(string));
            products.Columns.Add("Estimated Cost", typeof(string));
            products.Columns.Add("Budgeted", typeof(string));
            products.Columns.Add("Is New Item", typeof(string));
            products.Columns.Add("Priority", typeof(string));
            products.Columns.Add("Status", typeof(string));
            products.Columns.Add("CurrentApprover", typeof(string));
            products.Columns.Add("Technical Admin", typeof(string));
            products.Columns.Add("Unit Admin", typeof(string));
            products.Columns.Add("Medial Director", typeof(string));
            products.Columns.Add("Finance Approve", typeof(string));
            products.Columns.Add("Final Approver 1", typeof(string));
            products.Columns.Add("Purchase Approver 1", typeof(string));
            foreach (var c in tasks)
            {
                products.Rows.Add(
                    Convert.ToString(c.TaskCpxId),
                    c.CPXType,
                    c.CpxRelatedTo == "Others" ? c.CpxRelatedToOther : c.CpxRelatedTo,
                    c.CreatedDate,
                    c.CreatorLocationName + " " + c.CreatorDepartmentName,
                    c.CreatorName,
                    c.ProjectTitle,
                    c.RequestForDepartment,
                    c.Item,
                    c.Qty,
                    c.EstimatedCost.ToString(),
                    c.Budgeted,
                    c.IsNewItem,
                    c.Priority,
                    c.CpxStatus,
                    GetCurrentApproverName(c.TaskCpxId, c.CurrentApproverLevel),
                    GetCurrentApproverName2(c.TaskCpxId, "1"),
                    GetCurrentApproverName2(c.TaskCpxId, "2"),
                    GetCurrentApproverName2(c.TaskCpxId, "3"),
                    GetCurrentApproverName2(c.TaskCpxId, "5"),
                    GetCurrentApproverName2(c.TaskCpxId, "4"),
                    GetCurrentApproverName2(c.TaskCpxId, "8")
                );
            }


            var grid = new GridView();
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachement; filename=PM.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public string GetCurrentApproverName2(int TaskId, string Level)
        {
            string Approver = "";
            var listcheck = myapp.tbl_TaskApprove.Where(au => au.TaskId == TaskId && au.ApprovalLevel == Level).ToList();
            if (listcheck.Count > 0)
            {
                foreach (var l in listcheck)
                {
                    var user = myapp.tbl_User.Where(u => u.EmpId == l.ApproveEmpId).SingleOrDefault();
                    if (user != null)
                    {
                        //Approver += user.FirstName + " " + (l.IsApproved == true ? "Approved," : "Pending,");
                        Approver += (l.IsApproved == true ? "Approved," : "Pending,");
                    }
                }
                return Approver.TrimEnd(',');
            }
            else
            {
                return "NA";
            }
        }
        public ActionResult ManageCpxItems()
        {
            return View();
        }
        public ActionResult getAllCpxItems(int LocationId, string department)
        {
            if (department != null && department.Contains("_and_"))
            {
                department = department.Replace("_and_", "&");
            }
            var model = myapp.tbl_TaskCpxItem.Where(l => l.LocationId == LocationId && l.CommonDepartment == department).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveCpxItems(tbl_TaskCpxItem model)
        {
            model.CreatedBy = User.Identity.Name;
            model.CreatedOn = DateTime.Now;
            model.IsActive = true;
            myapp.tbl_TaskCpxItem.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult UpdateCpxItems(tbl_TaskCpxItem model)
        {
            var dbmodel = myapp.tbl_TaskCpxItem.Where(l => l.Id == model.Id).SingleOrDefault();
            dbmodel.CreatedBy = User.Identity.Name;
            dbmodel.CreatedOn = DateTime.Now;
            dbmodel.IsActive = true;
            dbmodel.CommonDepartment = model.CommonDepartment;
            dbmodel.ItemName = model.ItemName;
            dbmodel.BudgetAmount = model.BudgetAmount;
            dbmodel.DepartmentId = model.DepartmentId;
            dbmodel.LocationId = model.LocationId;
            dbmodel.Description = model.Description;
            dbmodel.ItemType = model.ItemType;
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetCpxItemsDetails(int id)
        {
            var model = myapp.tbl_TaskCpxItem.Where(l => l.Id == id).SingleOrDefault();

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteCpxItemsDetails(int id)
        {
            var model = myapp.tbl_TaskCpxItem.Where(l => l.Id == id).SingleOrDefault();
            myapp.tbl_TaskCpxItem.Remove(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxbyCpxItemsDetails(JQueryDataTableParamModel param)
        {
            var tasks = myapp.tbl_TaskCpxItem.ToList();

            tasks = tasks.OrderByDescending(t => t.Id).ToList();
            var loclist = myapp.tbl_Location.ToList();
            IEnumerable<tbl_TaskCpxItem> filteredCompanies;
            //Check whether the companies should be filtered by keyword
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = tasks
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.ItemName != null && c.ItemName.ToString().ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Description != null && c.Description.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.CommonDepartment != null && c.CommonDepartment.ToLower().Contains(param.sSearch.ToLower())
                              // ||
                              //c.Operator != null && c.Operator.ToLower().Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = tasks;
            }
            IEnumerable<tbl_TaskCpxItem> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           join l in loclist on c.LocationId equals l.LocationId
                                           select new[]
                                           {
                                               Convert.ToString(c.Id),
                                               l.LocationName,
                                                   c.CommonDepartment,
                                                   c.ItemName,
                                                   c.Description,
                                                   c.ItemType,
                                                   c.BudgetAmount.HasValue?c.BudgetAmount.Value.ToString():"0",

                                                   //c.Amount.ToString(),
                                                   c.CreatedBy,
                                                   c.CreatedOn.HasValue?c.CreatedOn.Value.ToString("dd/MM/yyyy"):"",
                                                   Convert.ToString(c.Id)
                                               };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = tasks.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetQueriesOrCommentsonRequest(int id)
        {
            List<tbl_TaskCpxComment> list = myapp.tbl_TaskCpxComment.Where(l => l.TaskCpxId == id).ToList();
            var model = (from m in list
                         join u in myapp.tbl_User on m.CreatedBy equals u.CustomUserId
                         select new
                         {
                             CreatedBy = u.FirstName + " " + u.LastName,
                             CreatedOn = m.CreatedOn.HasValue ? m.CreatedOn.Value.ToString("dd/MM/yyyy hh:mm tt") : "",
                             Id = m.Id,
                             Notes = m.Notes,
                             TaskCpxId = m.TaskCpxId,
                             WorkflowLevel = m.WorkflowLevel

                         }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveQueriesOnComments(tbl_TaskCpxComment model)
        {
            string status = "Query";
            CpxViewModel cpxmodelfull = GetViewModel(model.TaskCpxId.Value);
            if (model.CreatedBy == "Approver")
            {


                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel
                {
                    fromemail = "it_helpdesk@fernandez.foundation"
                };
                mailmodel.toemail = cpxmodelfull.CreatorEmail;
                //mailmodel.toemail = "ahmadali@fernandez.foundation";
                mailmodel.ccemail = "capex@fernandez.foundation";
                //  mailmodel.subject = "Ticket " + task.TaskId + " " + task.Subject.Substring(0, 50) + " - " + task.CreatorLocationName + " " + task.CreatorDepartmentName;
                mailmodel.subject = "Query On : Cpx Request " + cpxmodelfull.TaskCpxId + " - " + cpxmodelfull.CreatorLocationName + " " + cpxmodelfull.CreatorDepartmentName + " " + cpxmodelfull.CreatorName + " " + cpxmodelfull.ProjectTitle;

                if (mailmodel.toemail != null && mailmodel.toemail != "")
                {
                    string host = HttpContext.Request.Url.Host;
                    string scheme = HttpContext.Request.Url.Scheme;
                    string port = HttpContext.Request.Url.Port.ToString();
                    string MainUrl = "";
                    if (port != null && port != "")
                    {
                        MainUrl = scheme + "://" + host + ":" + port;
                    }
                    else
                    {
                        MainUrl = scheme + "://" + host;
                    }

                    string mailbody = "<p style='font-family:verdana;font-size:13px;'>Dear Sir, Please respond to Query on the CPX From </p>";
                    mailbody += "<p style='font-family:verdana;font-size:12px;'>Cpx Details are: </p><table style='width:60%;margin:auto;border:solid 1px #a1a2a3;'>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Task Id</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.TaskCpxId + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Requestor</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.CreatorLocationName + " " + cpxmodelfull.CreatorDepartmentName + " " + cpxmodelfull.CreatorName + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Request Date</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + DateTime.Now + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Project Title</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.ProjectTitle + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Item</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Item + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Qty Required</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Qty + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Is New Item</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.IsNewItem + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Item Details</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.ItemDetails + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Make and Model</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Model + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Main Features</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.MainFeatures + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Estimated Cost</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.EstimatedCost + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Budgeted</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Budgeted + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Priority</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Priority + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Is Approved?</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>Query:" + model.Notes + "</td></tr>";

                    mailbody += "</table><br /><p style='font-family:verdana;font-size:12px;'>This is an auto generated mail,Don't reply to this.</p>";
                    mailmodel.body = mailbody;
                    mailmodel.filepath = "";
                    mailmodel.fromname = "Query On CPX Request";
                    if (mailmodel.ccemail == null || mailmodel.ccemail == "")
                    {
                        mailmodel.ccemail = "";

                    }
                    else
                    {

                        cm.SendEmail(mailmodel);
                    }
                }


            }
            else
            {
                status = "In Progress";
                var approveremail = cpxmodelfull.ApprovalList.Where(l => l.ApprovalLevel == cpxmodelfull.CurrentApproverLevel).FirstOrDefault().ApproveEmail;
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel
                {
                    fromemail = "it_helpdesk@fernandez.foundation"
                };
                mailmodel.toemail = approveremail;
                //mailmodel.toemail = "ahmadali@fernandez.foundation";
                mailmodel.ccemail = "capex@fernandez.foundation";
                //  mailmodel.subject = "Ticket " + task.TaskId + " " + task.Subject.Substring(0, 50) + " - " + task.CreatorLocationName + " " + task.CreatorDepartmentName;
                mailmodel.subject = "Query Response On : Cpx Request " + cpxmodelfull.TaskCpxId + " - " + cpxmodelfull.CreatorLocationName + " " + cpxmodelfull.CreatorDepartmentName + " " + cpxmodelfull.CreatorName + " " + cpxmodelfull.ProjectTitle;

                if (mailmodel.toemail != null && mailmodel.toemail != "")
                {
                    string host = HttpContext.Request.Url.Host;
                    string scheme = HttpContext.Request.Url.Scheme;
                    string port = HttpContext.Request.Url.Port.ToString();
                    string MainUrl = "";
                    if (port != null && port != "")
                    {
                        MainUrl = scheme + "://" + host + ":" + port;
                    }
                    else
                    {
                        MainUrl = scheme + "://" + host;
                    }

                    string mailbody = "<p style='font-family:verdana;font-size:13px;'>Dear Sir, Please respond to Query on the CPX From </p>";
                    mailbody += "<p style='font-family:verdana;font-size:12px;'>Cpx Details are: </p><table style='width:60%;margin:auto;border:solid 1px #a1a2a3;'>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Task Id</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.TaskCpxId + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Requestor</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.CreatorLocationName + " " + cpxmodelfull.CreatorDepartmentName + " " + cpxmodelfull.CreatorName + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Request Date</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + DateTime.Now + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Project Title</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.ProjectTitle + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Item</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Item + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Qty Required</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Qty + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Is New Item</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.IsNewItem + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Item Details</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.ItemDetails + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Make and Model</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Model + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Main Features</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.MainFeatures + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Estimated Cost</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.EstimatedCost + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Budgeted</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Budgeted + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Priority</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + cpxmodelfull.Priority + "</td></tr>";
                    mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Is Approved?</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>Query Response: " + model.Notes + "</td></tr>";

                    mailbody += "</table><br /><p style='font-family:verdana;font-size:12px;'>This is an auto generated mail,Don't reply to this.</p>";
                    mailmodel.body = mailbody;
                    mailmodel.filepath = "";
                    mailmodel.fromname = "Query Response On CPX Request";
                    if (mailmodel.ccemail == null || mailmodel.ccemail == "")
                    {
                        mailmodel.ccemail = "";

                    }
                    else
                    {

                        cm.SendEmail(mailmodel);
                    }
                }

            }
            var cpxdetails = myapp.tbl_TaskCpx.Where(l => l.TaskCpxId == model.TaskCpxId.Value).SingleOrDefault();
            cpxdetails.CpxStatus = status;
            myapp.SaveChanges();
            model.CreatedOn = DateTime.Now;
            model.CreatedBy = User.Identity.Name;

            myapp.tbl_TaskCpxComment.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Report()
        {
            int LastYear = DateTime.Now.Year - 1;
            int CurrentYear = DateTime.Now.Year;

            var listofapprovers = myapp.tbl_TaskCpx.Where(t => t.CreatedOn > new DateTime(LastYear, 4, 1) && t.CreatedOn < new DateTime(CurrentYear, 4, 1)
                          && t.CpxStatus != "Rejected").ToList();
            List<CpxViewModel> tasks = (from m in listofapprovers
                                        join l in myapp.tbl_Location on m.CreatorLocationId equals l.LocationId
                                        join d in myapp.tbl_Department on m.CreatorDepartmentId equals d.DepartmentId
                                        join dc in myapp.tbl_CommonDepartment on m.RequestForDepartment equals dc.CommonDepartmentId
                                        join u in myapp.tbl_User on m.CreatorId equals u.EmpId

                                        select new CpxViewModel
                                        {
                                            Budgeted = m.Budgeted.HasValue ? (m.Budgeted.Value == true ? "Yes" : "No") : "No",
                                            CreatorDepartmentId = m.CreatorDepartmentId.ToString(),
                                            CreatorDepartmentName = d.DepartmentName,
                                            CreatorId = m.CreatorId.ToString(),
                                            CreatorLocationId = m.CreatorLocationId.ToString(),
                                            CreatorLocationName = l.LocationName,
                                            CreatorName = u.FirstName,
                                            EstimatedCost = m.EstimatedCost.HasValue ? m.EstimatedCost.Value : 0,
                                            IsNewItem = m.IsNewItem.HasValue ? (m.IsNewItem.Value == true ? "Yes" : "No") : "No",
                                            Item = m.Item,
                                            ItemDetails = m.ItemDetails,
                                            MainFeatures = m.MainFeatures,
                                            Model = m.Model,
                                            Priority = m.Prioriy,
                                            ProjectTitle = m.ProjectTitle,
                                            Qty = m.Qty.HasValue ? m.Qty.Value.ToString() : "0",
                                            RequestForDepartment = dc.Name,
                                            TaskCpxId = m.TaskCpxId,
                                            CpxStatus = m.CpxStatus,
                                            CurrentApproverLevel = m.CurrentApprovalLevel.HasValue ? m.CurrentApprovalLevel.Value.ToString() : "0",
                                            CurrentApprover = "",
                                            CreatedDate = m.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                            WorkDoneComments = m.WorkDoneComments,
                                            CpxRelatedTo = m.CpxRelatedTo,
                                            CpxRelatedToOther = m.CpxRelatedToOther,
                                            CPXType = m.CPXType,
                                            SupportCompany = m.SupportCompany,
                                            ReasonForReplacement = m.ReasonForReplacement,
                                            OldProductCost = m.OldProductCost,
                                            OldProductModel = m.OldProductModel,
                                            AMCLastRenewalDate = m.AMCLastRenewalDate.HasValue ? m.AMCLastRenewalDate.Value.ToString("dd/MM/yyyy") : "",
                                            AMCNextRenewalDate = m.AMCNextRenewalDate.HasValue ? m.AMCNextRenewalDate.Value.ToString("dd/MM/yyyy") : ""
                                        }).ToList();
            System.Data.DataTable products = new System.Data.DataTable("EmployeesLogin");
            products.Columns.Add("CpxId", typeof(string));
            products.Columns.Add("Cpx Type", typeof(string));
            products.Columns.Add("Related To", typeof(string));
            products.Columns.Add("Created Date", typeof(string));
            products.Columns.Add("Location", typeof(string));

            products.Columns.Add("Department", typeof(string));
            products.Columns.Add("Creator", typeof(string));
            products.Columns.Add("Cpx Related To", typeof(string));
            products.Columns.Add("Project Title", typeof(string));

            products.Columns.Add("Cpx Status", typeof(string));
            products.Columns.Add("Request For Department", typeof(string));
            products.Columns.Add("Item", typeof(string));

            products.Columns.Add("ItemDetails", typeof(string));
            products.Columns.Add("MainFeatures", typeof(string));
            products.Columns.Add("Model", typeof(string));

            products.Columns.Add("Priority", typeof(string));
            products.Columns.Add("Qty", typeof(string));
            products.Columns.Add("EstimatedCost", typeof(string));
            products.Columns.Add("WorkDoneComments", typeof(string));
            products.Columns.Add("Budgeted", typeof(string));
            products.Columns.Add("CurrentApprover", typeof(string));
            products.Columns.Add("SupportCompany", typeof(string));
            products.Columns.Add("ReasonForReplacement", typeof(string));
            products.Columns.Add("OldProductCost", typeof(string));
            products.Columns.Add("OldProductModel", typeof(string));
            products.Columns.Add("AMCLastRenewalDate", typeof(string));
            products.Columns.Add("AMCNextRenewalDate", typeof(string));
            products.Columns.Add("In Charge", typeof(string));
            products.Columns.Add("HOD", typeof(string));
            products.Columns.Add("Technical Admin", typeof(string));
            products.Columns.Add("Unit Admin", typeof(string));
            products.Columns.Add("Medial Director", typeof(string));
            products.Columns.Add("Final Approver 1", typeof(string));
            products.Columns.Add("Finance Approve", typeof(string));
            products.Columns.Add("Final Approver 2", typeof(string));
            products.Columns.Add("Purchase Approver 1", typeof(string));
            products.Columns.Add("Purchase Approver 2", typeof(string));
            foreach (CpxViewModel c in tasks)
            {
                var approverslist = (from ap in myapp.tbl_TaskApprove
                                     join s in myapp.tbl_User on ap.ApproveEmpId equals s.EmpId
                                     where ap.TaskId == c.TaskCpxId
                                     select new CpxApproverModel
                                     {
                                        ApprovalLevel= ap.ApprovalLevel,
                                         Name = s.FirstName + " " + s.LastName,
                                         IsApproved = ap.IsApproved,
                                         ApprovedDate =ap.ApprovedDate,
                                         ApproveComments = ap.ApproveComments != null ? ap.ApproveComments : ""
                                     }).ToList();
                products.Rows.Add(c.TaskCpxId.ToString(),
                     c.CPXType,
                    c.CpxRelatedTo == "Others" ? c.CpxRelatedToOther : c.CpxRelatedTo,
                                 c.CreatedDate,
                                 c.CreatorLocationName,
                                 c.CreatorDepartmentName,
                                 c.CreatorName,
                                 c.CpxRelatedTo,
                                 c.ProjectTitle,
                                 c.CpxStatus,
                                 c.RequestForDepartment, c.Item, c.ItemDetails, c.MainFeatures, c.Model,
                                 c.Priority,
                                 c.Qty,
                                 c.EstimatedCost.ToString(),
                                 c.WorkDoneComments,
                                 c.Budgeted,
                                 GetCurrentApproverName(c.TaskCpxId, c.CurrentApproverLevel),
                    c.SupportCompany,
                    c.ReasonForReplacement,
                     c.OldProductCost,
                     c.OldProductModel,
                     c.AMCLastRenewalDate,
                     c.AMCNextRenewalDate,
                     GetApproverStringFromList(approverslist,"-1"),
                     GetApproverStringFromList(approverslist, "0"),
                       GetApproverStringFromList(approverslist, "1"),
                       GetApproverStringFromList(approverslist, "2"),
                       GetApproverStringFromList(approverslist, "3"),
                        GetApproverStringFromList(approverslist, "5"),
                         GetApproverStringFromList(approverslist, "4"),
                          GetApproverStringFromList(approverslist, "6"),
                          GetApproverStringFromList(approverslist, "8"),
                          GetApproverStringFromList(approverslist, "9")

                );
            }

            GridView grid = new GridView
            {
                GridLines = GridLines.Both,
                BorderStyle = BorderStyle.Solid
            };
            grid.HeaderStyle.BackColor = System.Drawing.Color.Teal;
            grid.HeaderStyle.ForeColor = System.Drawing.Color.White;
            grid.DataSource = products;
            grid.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            string filename = "AllCPXs.xls";
            Response.AddHeader("content-disposition", "attachment; filename=" + filename);
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View();
        }

        public string GetApproverStringFromList(List<CpxApproverModel> cpxApprovers,string level)
        {
            var check = cpxApprovers.Where(l => l.ApprovalLevel == level).FirstOrDefault();
            if (check != null)
                return check.Name + " (" + (check.IsApproved == true ? "Yes" : "No") + ") " + (check.ApprovedDate != null ? ProjectConvert.ConverDateTimeToString(check.ApprovedDate.Value) : "") + "" + check.ApproveComments;
            return "";
        }
    }
}