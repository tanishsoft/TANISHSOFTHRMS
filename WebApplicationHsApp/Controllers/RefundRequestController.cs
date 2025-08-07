using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;
using WebApplicationHsApp.OracleInterface;

namespace WebApplicationHsApp.Controllers
{
    [Authorize]
    public class RefundRequestController : Controller
    {
        private readonly MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        // GET: RefundRequest
        public ActionResult Index()
        {
            ViewBag.Type = "";
            ViewBag.IsAccounts = "No";
            ViewBag.IsApprover = "No";
            if (User.IsInRole("Admin"))
            {
                ViewBag.IsAccounts = "Yes";
                ViewBag.IsApprover = "Yes";
            }
            int userid = int.Parse(User.Identity.Name);
            var usercheck = myapp.tbl_User.Where(l => l.EmpId == userid).SingleOrDefault();
            var userlist = myapp.tbl_RefundRequestAccess.Where(l => l.EmpId == usercheck.UserId).ToList();

            if (userlist.Where(l => l.AccessType == "Accounts").Count() > 0 || (usercheck.DepartmentName == "Finance & Accounts"))
            {
                ViewBag.IsAccounts = "Yes";
            }
            if (userlist.Where(l => l.AccessType == "Approver").Count() > 0)
            {
                ViewBag.IsApprover = "Yes";
            }
            return View();
        }
        public ActionResult NewRequest(int id = 0)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult ManageAccess()
        {
            return View();
        }
        public ActionResult GetRefundRequests(int id)
        {
            var model = myapp.tbl_RefundRequest.Where(l => l.RefundRequestId == id).SingleOrDefault();
            RefundRequestModel dbmodel = new RefundRequestModel
            {
                Status = model.Status,
                Justification = model.Justification,
                Remarks = model.Remarks,
                AccountHolderName = model.AccountHolderName,
                RefundRequestId = model.RefundRequestId,
                AccountNumber = model.AccountNumber,
                BankName = model.BankName,
                Branch = model.Branch,
                CreatedBy = model.CreatedBy,
                CreatedOn = model.CreatedOn.HasValue ? model.CreatedOn.Value.ToString("dd/MM/yyyy") : "",
                IFSC = model.IFSC,
                ModeUsed = model.ModeUsed,
                ModeUsedDetails = model.ModeUsedDetails,
                ModifiedBy = model.ModifiedBy,
                ModifiedOn = model.ModifiedOn.HasValue ? model.ModifiedOn.Value.ToString("dd/MM/yyyy") : "",
                PatientIpNo = model.PatientIpNo,
                PatientMobile = model.PatientMobile,
                PatientEmail = model.PatientEmail,
                PatientMrNo = model.PatientMrNo,
                PatientName = model.PatientName,
                RefundAmount = model.RefundAmount.HasValue ? model.RefundAmount.Value : 0,
                RefundBillNo = model.RefundBillNo,
                LocationId = model.LocationId.HasValue ? model.LocationId.Value : 0,
                LocationName = "",
                IsActive = model.IsActive.HasValue ? model.IsActive.Value : false
            };
            return Json(dbmodel, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteManageAccess(int id)
        {
            var requestaccess = myapp.tbl_RefundRequestAccess.FirstOrDefault(l => l.Id == id);
            if (requestaccess != null)
            {
                myapp.tbl_RefundRequestAccess.Remove(requestaccess);
                myapp.SaveChanges();
            }
            return Json("Successfully Deleted", JsonRequestBehavior.AllowGet);
        }
        public ActionResult AjaxGetManageAccess(JQueryDataTableParamModel param)
        {
            List<tbl_RefundRequestAccess> query = myapp.tbl_RefundRequestAccess.ToList();


            IEnumerable<tbl_RefundRequestAccess> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.Id.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.AccessType != null && c.AccessType.Contains(param.sSearch.ToLower())

                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<tbl_RefundRequestAccess> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                           join a in myapp.tbl_Location on c.LocationId equals a.LocationId
                                           join e in myapp.tbl_User on c.EmpId equals e.UserId
                                           select new[] {
                                              c.Id.ToString(),
                                              a.LocationName,
                                              e.CustomUserId,
                                              e.FirstName+" "+e.LastName,
                                              c.AccessType,
                                              c.CreatedBy,
                                              c.CreatedOn.Value.ToString("dd/MM/yyyy"),
                                              c.Id.ToString(),
                                              };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult SaveManageAccess(int locid, int empid, string accesstype)
        {
            tbl_RefundRequestAccess m = new tbl_RefundRequestAccess();
            m.LocationId = locid;
            m.EmpId = empid;
            m.AccessType = accesstype;
            m.CreatedBy = User.Identity.Name;
            m.CreatedOn = DateTime.Now;
            m.IsActive = true;
            myapp.tbl_RefundRequestAccess.Add(m);
            myapp.SaveChanges();
            return Json("Successfully Added", JsonRequestBehavior.AllowGet);

        }
        public ActionResult AjaxGetRefundRequests(JQueryDataTableParamModel param)
        {
            List<tbl_RefundRequest> squery = myapp.tbl_RefundRequest.ToList();


            string IsAccounts = "No";
            string IsApprover = "No";
            if (User.IsInRole("Admin"))
            {
                IsAccounts = "Yes";
                IsApprover = "Yes";
            }
            int userid = int.Parse(User.Identity.Name);
            var usercheck = myapp.tbl_User.Where(l => l.EmpId == userid).SingleOrDefault();
            var userlist = myapp.tbl_RefundRequestAccess.Where(l => l.EmpId == usercheck.UserId).ToList();

            if (userlist.Where(l => l.AccessType == "Accounts").Count() > 0 || (usercheck.DepartmentName == "Finance & Accounts"))
            {
                IsAccounts = "Yes";
            }
            if (userlist.Where(l => l.AccessType == "Approver").Count() > 0)
            {
                IsApprover = "Yes";
                squery = (from s in squery
                          join ul in userlist on s.LocationId equals ul.LocationId
                          where s.Status != "Closed"
                          select s).ToList();
            }

            if (param.locationid != null && param.locationid > 0)
            {
                squery = squery.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (!(IsAccounts == "Yes" || IsApprover == "Yes"))
            {
                if (usercheck != null)
                {
                    int locationid = usercheck.LocationId.HasValue ? usercheck.LocationId.Value : 0;
                    int deptid = usercheck.DepartmentId.HasValue ? usercheck.DepartmentId.Value : 0;
                    squery = squery.Where(l => l.CreatorLocationId == locationid && l.CreatorDepartmentId == deptid).ToList();
                }


            }

            if (param.fromdate != null && param.fromdate != "" && param.todate != null && param.todate != "")
            {
                DateTime dtfrmdate = ProjectConvert.ConverDateStringtoDatetime(param.fromdate);
                DateTime dttodate = ProjectConvert.ConverDateStringtoDatetime(param.todate);
                if (dtfrmdate == dttodate)
                {
                    dttodate = dttodate.AddDays(1);
                }
                squery = (from t in squery
                          where t.CreatedOn.Value >= dtfrmdate && t.CreatedOn.Value <= dttodate
                          select t).ToList();
            }
            if (param.locationid != null && param.locationid != 0)
            {
                squery = squery.Where(l => l.LocationId == param.locationid).ToList();
            }
            if (param.category != null && param.category != "" && param.category.ToLower() != "select")
            {
                squery = squery.Where(l => l.Justification == param.category).ToList();
            }
            List<RefundRequestModel> query = (from m in squery
                                              join user in myapp.tbl_User on m.CreatedBy equals user.CustomUserId
                                              join l in myapp.tbl_Location on m.LocationId equals l.LocationId
                                              select new RefundRequestModel
                                              {
                                                  RefundRequestId = m.RefundRequestId,
                                                  AccountHolderName = m.AccountHolderName,
                                                  AccountNumber = m.AccountNumber,
                                                  BankName = m.BankName,
                                                  Branch = m.Branch,
                                                  CreatedBy = user.FirstName,
                                                  IFSC = m.IFSC,
                                                  IsActive = m.IsActive.HasValue ? m.IsActive.Value : false,
                                                  ModeUsed = m.ModeUsed,
                                                  ModeUsedDetails = m.ModeUsedDetails,
                                                  ModifiedBy = m.ModifiedBy,
                                                  ModifiedOn = m.ModifiedOn.HasValue ? m.ModifiedOn.Value.ToString("dd/MM/yyyy") : "",
                                                  PatientIpNo = m.PatientIpNo,
                                                  PatientMobile = m.PatientMobile,
                                                  PatientMrNo = m.PatientMrNo,
                                                  RefundAmount = m.RefundAmount.HasValue ? m.RefundAmount.Value : 0,
                                                  RefundBillNo = m.RefundBillNo,
                                                  Justification = m.Justification,

                                                  PatientName = m.PatientName,
                                                  Remarks = m.Remarks,
                                                  CreatedOn = ProjectConvert.ConverDateTimeToString(m.CreatedOn.Value),

                                                  Status = m.Status,
                                                  LocationId = m.LocationId.HasValue ? m.LocationId.Value : 0,
                                                  LocationName = l.LocationName,
                                                  LastAcceptedBy = m.LastAcceptedBy.HasValue && m.LastAcceptedBy > 0 ? myapp.tbl_User.FirstOrDefault(u1 => u1.EmpId == m.LastAcceptedBy).FirstName : "",
                                                  LastApprovedRejectedBy = m.LastApprovedRejectedBy.HasValue && m.LastAcceptedBy > 0 ? myapp.tbl_User.FirstOrDefault(u1 => u1.EmpId == m.LastApprovedRejectedBy).FirstName : "",

                                                  IsRequestApproved = GetIsRequestApproved(m)
                                              }).ToList();
            query = query.OrderByDescending(l => l.RefundRequestId).ToList();

            IEnumerable<RefundRequestModel> filteredCompanies;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                filteredCompanies = query
                   .Where(c => c.RefundRequestId.ToString().Contains(param.sSearch.ToLower())
                               ||
                                c.RefundAmount != null && c.RefundAmount.ToString().Contains(param.sSearch.ToLower())
                               ||
                              c.PatientMobile != null && c.PatientMobile.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.PatientMrNo != null && c.PatientMrNo.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.PatientIpNo != null && c.PatientIpNo.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.Justification != null && c.Justification.ToLower().Contains(param.sSearch.ToLower())
                                ||
                              c.PatientName != null && c.PatientName.ToLower().Contains(param.sSearch.ToLower())
                               ||
                              c.RefundBillNo != null && c.RefundBillNo.ToLower().Contains(param.sSearch.ToLower())
                              ||
                              c.LocationName != null && c.LocationName.ToLower().Contains(param.sSearch.ToLower())
                              );
            }
            else
            {
                filteredCompanies = query;
            }
            IEnumerable<RefundRequestModel> displayedCompanies = filteredCompanies.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<string[]> result = from c in displayedCompanies
                                               //join a in myapp.tbl_User on c.Approver equals a.CustomUserId
                                           select new[] {
                                              c.RefundRequestId.ToString(),
                                              c.LocationName,
                                              c.CreatedOn,
                                              c.PatientIpNo,
                                              c.PatientName,
                                              //c.PatientMrNo,
                                              //c.PatientMobile,
                                              c.RefundAmount.ToString(),
                                              c.Justification,
                                              c.RefundBillNo,
                                              //c.ModeUsed,
                                              //c.ModeUsedDetails,
                                              c.Status,
                                              c.CreatedBy,
                                              c.LastAcceptedBy,
                                              c.LastApprovedRejectedBy,
                                             c.RefundRequestId.ToString()+"_"+c.IsRequestApproved,
                                              };
            return Json(new
            {
                sEcho = param.sEcho,
                iTotalRecords = query.Count(),
                iTotalDisplayRecords = filteredCompanies.Count(),
                aaData = result
            }, JsonRequestBehavior.AllowGet);
        }
        public string GetIsRequestApproved(tbl_RefundRequest m)
        {
            string isRequestApproved = "N";
            if (m.LastApprovedRejectedBy != null && m.LastApprovedRejectedOn == null)
            {
                isRequestApproved = "Y";
            }
            return isRequestApproved;
        }
        public void CheckAndUpdateLogs(tbl_RefundRequest dbmodel, RefundRequestModel model)
        {
            List<tbl_RefundRequestComment> comments = new List<tbl_RefundRequestComment>();
            var comment = new tbl_RefundRequestComment
            {

                Comment = "",
                CommentedBy = User.Identity.Name,
                CommentedOn = DateTime.Now,
                CommentedType = "Modified",
                IsActive = true,
                RefundRequestId = dbmodel.RefundRequestId
            };
            if (dbmodel.LocationId != model.LocationId)
            {
                comment.Comment = "Location was modified";
                comments.Add(comment);
            }


            if (dbmodel.Justification != model.Justification)
            {
                comment.Comment = "Justification was modified from " + dbmodel.Justification + " to " + model.Justification;
                comments.Add(comment);
            }

            if (dbmodel.ModeUsed != model.ModeUsed)
            {
                comment.Comment = "ModeUsed was modified from " + dbmodel.ModeUsed + " to " + model.ModeUsed;
                comments.Add(comment);
            }
            if (dbmodel.ModeUsedDetails != model.ModeUsedDetails)
            {
                comment.Comment = "ModeUsedDetails was modified from " + dbmodel.ModeUsedDetails + " to " + model.ModeUsedDetails;
                comments.Add(comment);
            }
            if (dbmodel.PatientName != model.PatientName)
            {
                comment.Comment = "PatientName was modified from " + dbmodel.PatientName + " to " + model.PatientName;
                comments.Add(comment);
            }
            if (dbmodel.PatientIpNo != model.PatientIpNo)
            {
                comment.Comment = "PatientIpNo was modified from " + dbmodel.PatientIpNo + " to " + model.PatientIpNo;
                comments.Add(comment);
            }
            if (dbmodel.RefundAmount != model.RefundAmount)
            {
                comment.Comment = "RefundAmount was modified from " + dbmodel.RefundAmount + " to " + model.RefundAmount;
                comments.Add(comment);
            }
            if (dbmodel.RefundBillNo != model.RefundBillNo)
            {
                comment.Comment = "RefundBillNo was modified from " + dbmodel.RefundBillNo + " to " + model.RefundBillNo;
                comments.Add(comment);
            }
            if (dbmodel.AccountHolderName != model.AccountHolderName)
            {
                comment.Comment = "AccountHolderName was modified from " + dbmodel.AccountHolderName + " to " + model.AccountHolderName;
                comments.Add(comment);
            }
            if (dbmodel.IFSC != model.IFSC)
            {
                comment.Comment = "IFSC was modified from " + dbmodel.IFSC + " to " + model.IFSC;
                comments.Add(comment);
            }
            if (dbmodel.BankName != model.BankName)
            {
                comment.Comment = "BankName was modified from " + dbmodel.BankName + " to " + model.BankName;
                comments.Add(comment);
            }
            if (dbmodel.AccountNumber != model.AccountNumber)
            {
                comment.Comment = "AccountNumber was modified from " + dbmodel.AccountNumber + " to " + model.AccountNumber;
                comments.Add(comment);
            }
            if (dbmodel.Branch != model.Branch)
            {
                comment.Comment = "Branch was modified from " + dbmodel.Branch + " to " + model.Branch;
                comments.Add(comment);
            }
            if (dbmodel.Remarks != model.Remarks)
            {
                comment.Comment = "Remarks was modified from " + dbmodel.Remarks + " to " + model.Remarks;
                comments.Add(comment);
            }
            myapp.tbl_RefundRequestComment.AddRange(comments);
            myapp.SaveChanges();


        }
        public ActionResult SaveRefundRequest(RefundRequestModel model, HttpPostedFileBase[] Upload)
        {
            tbl_RefundRequest dbmodel = new tbl_RefundRequest();
            int userid = int.Parse(User.Identity.Name);
            string approveremailid = "";
            tbl_User approver = new tbl_User();
            var usercheck = myapp.tbl_User.Where(l => l.EmpId == userid).FirstOrDefault();
            var approvers = myapp.tbl_RefundRequestAccess.Where(l => l.LocationId == model.LocationId && l.AccessType == "Approver").ToList();
            if (model.RefundRequestId > 0)
            {
                dbmodel = myapp.tbl_RefundRequest.Where(l => l.RefundRequestId == model.RefundRequestId).SingleOrDefault();
                CheckAndUpdateLogs(dbmodel, model);
                dbmodel.LocationId = model.LocationId;
                dbmodel.Justification = model.Justification;
                dbmodel.ModeUsed = model.ModeUsed;
                dbmodel.ModeUsedDetails = model.ModeUsedDetails;
                dbmodel.ModifiedBy = User.Identity.Name;
                dbmodel.ModifiedOn = DateTime.Now;
                dbmodel.PatientName = model.PatientName;
                dbmodel.PatientEmail = model.PatientEmail;
                dbmodel.PatientIpNo = model.PatientIpNo;
                dbmodel.PatientMrNo = model.PatientMrNo;
                dbmodel.PatientMobile = model.PatientMobile;
                dbmodel.RefundAmount = model.RefundAmount;
                dbmodel.RefundBillNo = model.RefundBillNo;
                dbmodel.AccountHolderName = model.AccountHolderName;
                dbmodel.IFSC = model.IFSC;
                dbmodel.BankName = model.BankName;
                dbmodel.AccountNumber = model.AccountNumber;
                dbmodel.Branch = model.Branch;
                dbmodel.Remarks = model.Remarks;
                dbmodel.Status = model.Status;
                dbmodel.Column1 = model.Column1;
                myapp.SaveChanges();
            }
            else
            {
                dbmodel = new tbl_RefundRequest
                {
                    LocationId = model.LocationId,
                    Justification = model.Justification,
                    ModeUsed = model.ModeUsed,
                    ModeUsedDetails = model.ModeUsedDetails,

                    CreatedBy = User.Identity.Name,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    ModifiedBy = User.Identity.Name,
                    ModifiedOn = DateTime.Now,
                    Status = "Approval Waiting",
                    PatientName = model.PatientName,
                    PatientIpNo = model.PatientIpNo,
                    PatientMrNo = "",
                    PatientEmail = model.PatientEmail,
                    PatientMobile = model.PatientMobile,
                    RefundAmount = model.RefundAmount,
                    RefundBillNo = model.RefundBillNo,
                    AccountHolderName = model.AccountHolderName,
                    IFSC = model.IFSC,
                    BankName = model.BankName,
                    AccountNumber = model.AccountNumber,
                    Branch = model.Branch,
                    Remarks = model.Remarks,
                    Column1 = model.Column1

                };

                if (usercheck != null)
                {
                    dbmodel.CreatorLocationId = usercheck.LocationId;
                    dbmodel.CreatorDepartmentId = usercheck.DepartmentId;
                }
                if (approvers.Count > 0)
                {
                    int approveruserid = approvers[0].EmpId.Value;
                    approver = myapp.tbl_User.Where(l => l.UserId == approveruserid).FirstOrDefault();
                    dbmodel.LastApprovedRejectedBy = approver.EmpId;
                    approveremailid = approver.EmailId;
                }
                var checkCount = myapp.tbl_RefundRequest.Where(l => l.PatientIpNo == dbmodel.PatientIpNo
                  && l.RefundBillNo == dbmodel.RefundBillNo && l.RefundAmount == dbmodel.RefundAmount && l.PatientName == dbmodel.PatientName).Count();
                if (checkCount > 0)
                    return Json("Successfully Added", JsonRequestBehavior.AllowGet);
                myapp.tbl_RefundRequest.Add(dbmodel);
                myapp.SaveChanges();
            }
            if (Upload != null && Upload.Length > 0)
            {
                string serverpath = Server.MapPath("~/ExcelUplodes/");
                foreach (HttpPostedFileBase file in Upload)
                {
                    if (file != null)
                    {
                        string fileName = Path.GetFileName(file.FileName);
                        string guidid = "RefundRequest_" + dbmodel.RefundRequestId + "_doc_" + fileName;
                        string path = Path.Combine(serverpath, guidid);
                        file.SaveAs(path);
                        tbl_RefundRequestDocument doc = new tbl_RefundRequestDocument();
                        doc.Attachment = guidid;
                        doc.CreatedBy = User.Identity.Name;
                        doc.CreatedOn = DateTime.Now;
                        doc.IsActive = true;
                        doc.RefundRequestId = dbmodel.RefundRequestId;
                        doc.Title = file.FileName;
                        myapp.tbl_RefundRequestDocument.Add(doc);
                        myapp.SaveChanges();
                    }
                }
            }
            string toemail = "accounts@fernandez.foundation,saikrupal.mv@fernandez.foundation";
            string ccemail = "";
            int userid3 = int.Parse(User.Identity.Name);
            var usercheck2 = myapp.tbl_User.Where(l => l.EmpId == userid3).FirstOrDefault();
            if (usercheck2 != null && usercheck2.EmailId != null && usercheck2.EmailId != "")
            {
                ccemail = usercheck2.EmailId;
            }
            if (dbmodel.LocationId != null)
            {
                switch (dbmodel.LocationId.Value)
                {
                    case 1:
                        toemail = toemail + ",billing@fernandez.foundation";
                        ccemail = ccemail + ", neelesh_g@fernandez.foundation, drashraf@fernandez.foundation";
                        break;
                    case 2:
                        toemail = toemail + ",billing_hg@fernandez.foundation";
                        ccemail = ccemail + ",neelesh_g@fernandez.foundation";
                        break;
                    case 5:
                        toemail = toemail + ",narsaraju.bollepally@fernandez.foundation";
                        ccemail = ccemail + ",neelesh_g@fernandez.foundation";
                        break;
                    case 4:
                        ccemail = ccemail + ",neelesh_g@fernandez.foundation";
                        break;
                    case 9:
                        ccemail = ccemail + ",neelesh_g@fernandez.foundation, satyadurga_a@fernandez.foundation";
                        break;
                    case 20:
                        ccemail = ccemail + ", neelesh_g@fernandez.foundation";
                        break;
                }
            }
            string subject = "New Refund Request " + dbmodel.RefundRequestId + " - " + dbmodel.PatientIpNo + " " + dbmodel.PatientName + " " + dbmodel.RefundAmount;

            SendEmailOnRefundrequest(dbmodel, toemail, ccemail, subject, dbmodel.Status);
            if (approveremailid != null && approveremailid != "")
            {
                subject = "Approval Required On New Refund Request " + dbmodel.RefundRequestId + " - " + dbmodel.PatientIpNo + " " + dbmodel.PatientName + " " + dbmodel.RefundAmount;
                SendEmailOnRefundrequest(dbmodel, approveremailid, approveremailid, subject, dbmodel.Status, approver.EmpId.Value, true);
            }
            return Json("Successfully Added", JsonRequestBehavior.AllowGet);
        }
        public void SendEmailOnRefundrequest(tbl_RefundRequest dbmodel, string toemail, string ccemail, string subject, string status, int ApproverId = 0, bool IsApproval = false)
        {
            CustomModel cm = new CustomModel();
            MailModel mailmodel = new MailModel
            {
                fromemail = "it_helpdesk@fernandez.foundation"
            };
            mailmodel.toemail = toemail;
            mailmodel.subject = subject;
            if (mailmodel.toemail != null && mailmodel.toemail != "")
            {
                mailmodel.body = GetEmailBody(dbmodel, status, ApproverId, IsApproval);
                mailmodel.filepath = "";
                mailmodel.fromname = "Refund Request " + status;
                mailmodel.ccemail = ccemail;

                cm.SendEmail(mailmodel);

            }
        }
        public string GetEmailBody(tbl_RefundRequest dbmodel, string status, int ApproverId = 0, bool isapproval = false)
        {
            string mailbody = "<p style='font-family:verdana;font-size:13px;'>Dear Sir, Please find below are the Refund Request details :</p>";
            mailbody += "<table style='width:60%;margin:auto;border:solid 1px #a1a2a3;'>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Refund Request Id</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.RefundRequestId + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Date Of Submit</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.CreatedOn.Value.ToString("dd/MM/yyyy") + "</td></tr>";

            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>IP No/MR NO/Pharmacy NO</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.PatientIpNo + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Patient Name</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.PatientName + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Justification</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.Justification + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Mode Used</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.ModeUsed + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Mode Used Details</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.ModeUsedDetails + "</td></tr>";

            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Refund Amount</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.RefundAmount + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Remarks</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.Remarks + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Bill No</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.RefundBillNo + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Account Holder Name</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.AccountHolderName + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Account Number</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.AccountNumber + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Bank Name</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.BankName + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>Branch</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.Branch + "</td></tr>";
            mailbody += "<tr><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;'>IFSC</td><td style='font-family:verdana;border-bottom:solid 1px #ddd;font-size:12px;padding:5px;color:teal;'>" + dbmodel.IFSC + "</td></tr>";
            if (isapproval)
            {
                mailbody += "<tr><td style='border-bottom:solid 1px #ddd;font-size:12px;color:green;text-align:center;font-family:verdana;' colspan='2'><a href='https://infonet.fernandezhospital.com/RefundRequest/UpdateRequestByTeam?id=" + dbmodel.RefundRequestId + "&status=Approved&comments=Ok&userId=" + ApproverId + "'>Click here to Approve</a></td></tr>";
                mailbody += "<tr><td style='border-bottom:solid 1px #ddd;font-size:12px;color:red;text-align:center;font-family:verdana;' colspan='2'><a href='https://infonet.fernandezhospital.com/RefundRequest/UpdateRequestByTeam?id=" + dbmodel.RefundRequestId + "&status=Rejected&comments=Not-Ok&userId=" + ApproverId + "'>Click here to Reject</a></td></tr>";
            }
            mailbody += "</table><br /><p style='font-family:verdana;font-size:12px;'>This is an auto generated mail,Don't reply to this.</p>";
            return mailbody;
        }

        public ActionResult GetPatientDetailsforipnoOrMrno(string ipno, string mrno)
        {
            ConnectOracle connect = new ConnectOracle();
            var model = connect.GetPatientBasicDetails(ipno, mrno);

            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ConfirmRequest(int id, string comments = "")
        {
            var dbmodel = myapp.tbl_RefundRequest.Where(l => l.RefundRequestId == id).SingleOrDefault();
            dbmodel.Status = "Closed";
            myapp.SaveChanges();
            string toemail = "mohd.jameel@fernandez.foundation,saikrupal.mv@fernandez.foundation,accounts@fernandez.foundation";
            string ccemail = "";
            int userid3 = int.Parse(dbmodel.CreatedBy);
            var usercheck2 = myapp.tbl_User.Where(l => l.EmpId == userid3).FirstOrDefault();
            if (usercheck2 != null && usercheck2.EmailId != null && usercheck2.EmailId != "")
            {
                ccemail = usercheck2.EmailId;
            }
            if (dbmodel.LocationId != null)
            {
                switch (dbmodel.LocationId.Value)
                {
                    case 1:
                        ccemail = ccemail + ",drashraf@fernandez.foundation";
                        break;
                    case 2:
                    case 5:
                        ccemail = ccemail + "";
                        break;
                    case 4:
                        ccemail = ccemail + "";
                        break;
                    case 9:
                        ccemail = ccemail + ",satyadurga_a@fernandez.foundation";
                        break;
                    case 10:
                        ccemail = ccemail + ",priyanka.m@fernandez.foundation";
                        break;
                }
            }
            string subject = "Refund Request " + dbmodel.RefundRequestId + " - " + dbmodel.PatientIpNo + " " + dbmodel.PatientName + " has Closed ";

            SendEmailOnRefundrequest(dbmodel, toemail, ccemail, subject, dbmodel.Status);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult RejectRequest(int id, string comments = "", string approveid = "")
        {
            var dbmodel = myapp.tbl_RefundRequest.Where(l => l.RefundRequestId == id).SingleOrDefault();
            dbmodel.Status = "Rejected";
            dbmodel.LastApprovedRejectedBy = int.Parse(User.Identity.Name);
            dbmodel.LastApprovedRejectedOn = DateTime.Now;
            myapp.SaveChanges();
            string toemail = "mohd.jameel@fernandez.foundation,saikrupal.mv@fernandez.foundation,accounts@fernandez.foundation";
            string ccemail = "";
            int userid3 = int.Parse(dbmodel.CreatedBy);
            var usercheck2 = myapp.tbl_User.Where(l => l.EmpId == userid3).FirstOrDefault();
            if (usercheck2 != null && usercheck2.EmailId != null && usercheck2.EmailId != "")
            {
                ccemail = usercheck2.EmailId;
            }
            if (dbmodel.LocationId != null)
            {
                switch (dbmodel.LocationId.Value)
                {
                    case 1:
                        ccemail = ccemail + ",drashraf@fernandez.foundation";
                        break;
                    case 2:
                    case 5:
                        ccemail = ccemail + "";
                        break;
                    case 4:
                        ccemail = ccemail + "";
                        break;
                    case 9:
                        ccemail = ccemail + ",satyadurga_a@fernandez.foundation";
                        break;
                    case 10:
                        ccemail = ccemail + ",priyanka.m@fernandez.foundation";
                        break;
                }
            }
            string subject = "Refund Request " + dbmodel.RefundRequestId + " - " + dbmodel.PatientIpNo + " " + dbmodel.PatientName + " has Rejected ";

            SendEmailOnRefundrequest(dbmodel, toemail, ccemail, subject, dbmodel.Status);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFiles()
        {
            if (Request.Form["id"] != null && Request.Form["id"] != "")
            {
                string Id = Request.Form["id"].ToString();
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
                            tbl_RefundRequestDocument tsk = new tbl_RefundRequestDocument
                            {
                                CreatedBy = User.Identity.Name,
                                CreatedOn = DateTime.Now,
                                Title = fileName,
                                Attachment = guidid + fileName,
                                IsActive = true,
                                RefundRequestId = int.Parse(Id)
                            };
                            myapp.tbl_RefundRequestDocument.Add(tsk);
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
            List<tbl_RefundRequestDocument> list = myapp.tbl_RefundRequestDocument.Where(l => l.RefundRequestId == id).ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        public string UpdateRequestByTeam(int id, string status, string comments, int userId)
        {
            var dbmodel = myapp.tbl_RefundRequest.Where(l => l.RefundRequestId == id).SingleOrDefault();
            dbmodel.Status = status;
            if (status == "Accept")
            {
                dbmodel.LastAcceptedBy = userId;
                dbmodel.LastAcceptedOn = DateTime.Now;
            }
            else
            {
                dbmodel.LastApprovedRejectedBy = userId;
                dbmodel.LastApprovedRejectedOn = DateTime.Now;
            }
            dbmodel.Remarks = dbmodel.Remarks + "" + comments;
            myapp.SaveChanges();


            string toemail = "";
            string ccemail = "";
            int userid3 = int.Parse(dbmodel.CreatedBy);
            var usercheck2 = myapp.tbl_User.Where(l => l.EmpId == userid3).FirstOrDefault();
            if (usercheck2 != null && usercheck2.EmailId != null && usercheck2.EmailId != "")
            {
                ccemail = usercheck2.EmailId;
                toemail = usercheck2.EmailId;
            }
            string subject = "Refund Request " + dbmodel.RefundRequestId + " - " + dbmodel.PatientIpNo + " " + dbmodel.PatientName + " has In Progress ";

            SendEmailOnRefundrequest(dbmodel, toemail, ccemail, subject, dbmodel.Status);
            return "Success";
        }
        public JsonResult ApproveRequestByAccounts(int id, string status, string comments)
        {
            UpdateRequestByTeam(id, status, comments, int.Parse(User.Identity.Name));

            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult CloseRequestByAccounts(int id, string comments)
        {
            var dbmodel = myapp.tbl_RefundRequest.Where(l => l.RefundRequestId == id).SingleOrDefault();
            dbmodel.Remarks = dbmodel.Remarks + " " + comments;
            dbmodel.Status = "Closed";

            myapp.SaveChanges();

            tbl_RefundRequestComment m = new tbl_RefundRequestComment();
            m.Comment = comments;
            m.RefundRequestId = id;
            m.IsActive = true;
            m.CommentedType = "OnClose";
            m.CommentedBy = User.Identity.Name;
            m.CommentedOn = DateTime.Now;
            myapp.tbl_RefundRequestComment.Add(m);
            myapp.SaveChanges();
            string toemail = "mohd.jameel@fernandez.foundation,saikrupal.mv@fernandez.foundation,accounts@fernandez.foundation";
            string ccemail = "";
            int userid3 = int.Parse(dbmodel.CreatedBy);
            var usercheck2 = myapp.tbl_User.Where(l => l.EmpId == userid3).FirstOrDefault();
            if (usercheck2 != null && usercheck2.EmailId != null && usercheck2.EmailId != "")
            {
                ccemail = usercheck2.EmailId;
            }
            if (dbmodel.LocationId != null)
            {
                switch (dbmodel.LocationId.Value)
                {
                    case 1:
                        ccemail = ccemail + ",drashraf@fernandez.foundation";
                        break;
                    case 2:
                    case 5:
                        ccemail = ccemail + "";
                        break;
                    case 4:
                        ccemail = ccemail + "";
                        break;
                    case 9:
                        ccemail = ccemail + ",satyadurga_a@fernandez.foundation";
                        break;
                    case 10:
                        ccemail = ccemail + ",priyanka.m@fernandez.foundation";
                        break;
                }
            }
            string subject = "Refund Request " + dbmodel.RefundRequestId + " - " + dbmodel.PatientIpNo + " " + dbmodel.PatientName + " has Closed ";

            SendEmailOnRefundrequest(dbmodel, toemail, ccemail, subject, dbmodel.Status);
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetQueriesOrCommentsonRequest(int id)
        {
            List<tbl_RefundRequestComment> list = myapp.tbl_RefundRequestComment.Where(l => l.RefundRequestId == id).ToList();
            var model = (from m in list
                         join u in myapp.tbl_User on m.CommentedBy equals u.CustomUserId
                         select new
                         {
                             CreatedBy = u.FirstName + " " + u.LastName,
                             CreatedOn = m.CommentedOn.HasValue ? m.CommentedOn.Value.ToString("dd/MM/yyyy hh:mm tt") : "",
                             Id = m.RefundRequestCommentId,
                             Notes = m.Comment,
                             RefundRequestId = m.RefundRequestId,
                             CommentedType = m.CommentedType,

                         }).ToList();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveQueriesOnComments(tbl_RefundRequestComment model)
        {

            model.CommentedOn = DateTime.Now;
            model.CommentedBy = User.Identity.Name;

            myapp.tbl_RefundRequestComment.Add(model);
            myapp.SaveChanges();
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportToExcel(string FromDate, string ToDate, int LocationId = 0)
        {
            List<tbl_RefundRequest> squery = myapp.tbl_RefundRequest.ToList();


            string IsAccounts = "No";
            string IsApprover = "No";
            if (User.IsInRole("Admin"))
            {
                IsAccounts = "Yes";
                IsApprover = "Yes";
            }
            int userid = int.Parse(User.Identity.Name);
            var usercheck = myapp.tbl_User.Where(l => l.EmpId == userid).SingleOrDefault();
            var userlist = myapp.tbl_RefundRequestAccess.Where(l => l.EmpId == usercheck.UserId).ToList();
            if (userlist.Where(l => l.AccessType == "Accounts").Count() > 0)
            {
                IsAccounts = "Yes";
            }
            if (userlist.Where(l => l.AccessType == "Approver").Count() > 0)
            {
                IsAccounts = "Yes";
            }

            if (LocationId > 0)
            {
                squery = squery.Where(l => l.LocationId == LocationId).ToList();
            }
            if (IsAccounts == "Yes" || IsApprover == "Yes")
            {

            }
            else
            {

                if (usercheck != null)
                {
                    int locationid = usercheck.LocationId.HasValue ? usercheck.LocationId.Value : 0;
                    int deptid = usercheck.DepartmentId.HasValue ? usercheck.DepartmentId.Value : 0;
                    squery = squery.Where(l => l.CreatorLocationId == locationid && l.CreatorDepartmentId == deptid).ToList();
                }


            }

            if (FromDate != null && FromDate != "" && ToDate != null && ToDate != "")
            {
                DateTime dtfrmdate = ProjectConvert.ConverDateStringtoDatetime(FromDate);
                DateTime dttodate = ProjectConvert.ConverDateStringtoDatetime(ToDate);
                if (dtfrmdate == dttodate)
                {
                    dttodate = dttodate.AddDays(1);
                }
                squery = (from t in squery
                          where t.CreatedOn.Value >= dtfrmdate && t.CreatedOn.Value <= dttodate
                          select t).ToList();
            }
            List<RefundRequestModel> query = (from m in squery
                                              join user in myapp.tbl_User on m.CreatedBy equals user.CustomUserId
                                              join l in myapp.tbl_Location on m.LocationId equals l.LocationId
                                              select new RefundRequestModel
                                              {
                                                  RefundRequestId = m.RefundRequestId,
                                                  AccountHolderName = m.AccountHolderName,
                                                  AccountNumber = m.AccountNumber,
                                                  BankName = m.BankName,
                                                  Branch = m.Branch,
                                                  CreatedBy = m.CreatedBy,
                                                  IFSC = m.IFSC,
                                                  IsActive = m.IsActive.HasValue ? m.IsActive.Value : false,
                                                  ModeUsed = m.ModeUsed,
                                                  ModeUsedDetails = m.ModeUsedDetails,
                                                  ModifiedBy = m.ModifiedBy,
                                                  ModifiedOn = m.ModifiedOn.HasValue ? m.ModifiedOn.Value.ToString("dd/MM/yyyy") : "",
                                                  PatientIpNo = m.PatientIpNo,
                                                  PatientMobile = m.PatientMobile,
                                                  PatientMrNo = m.PatientMrNo,
                                                  RefundAmount = m.RefundAmount.HasValue ? m.RefundAmount.Value : 0,
                                                  RefundBillNo = m.RefundBillNo,
                                                  Justification = m.Justification,

                                                  PatientName = m.PatientName,
                                                  Remarks = m.Remarks,
                                                  CreatedOn = ProjectConvert.ConverDateTimeToString(m.CreatedOn.Value),

                                                  Status = m.Status,
                                                  LocationId = m.LocationId.HasValue ? m.LocationId.Value : 0,
                                                  LocationName = l.LocationName
                                              }).ToList();
            query = query.OrderByDescending(l => l.RefundRequestId).ToList();


            System.Data.DataTable products = new System.Data.DataTable("MyTasksDataTable");
            products.Columns.Add("Id", typeof(string));
            products.Columns.Add("Location", typeof(string));
            products.Columns.Add("MR/IP No", typeof(string));
            products.Columns.Add("Patient Name", typeof(string));
            products.Columns.Add("Date Of Submit", typeof(string));
            products.Columns.Add("Justification", typeof(string));
            products.Columns.Add("Refund Bill No", typeof(string));
            products.Columns.Add("RefundAmount", typeof(string));
            products.Columns.Add("Account HolderName", typeof(string));
            products.Columns.Add("Mode used by patient to pay amount", typeof(string));
            products.Columns.Add("Mode Used Details", typeof(string));
            products.Columns.Add("IFSC", typeof(string));
            products.Columns.Add("BankName", typeof(string));
            products.Columns.Add("AccountNumber", typeof(string));
            products.Columns.Add("Branch", typeof(string));
            products.Columns.Add("Remarks", typeof(string));
            products.Columns.Add("Status", typeof(string));
            foreach (var c in query)
            {
                products.Rows.Add(c.RefundRequestId,
                    c.LocationName,
                    c.PatientIpNo,
                     c.PatientName,
                    c.CreatedOn,
                    c.Justification,
                    c.RefundBillNo,
                    c.RefundAmount,
                    c.AccountHolderName,
                    c.ModeUsed,
                    c.ModeUsedDetails,
                    c.IFSC,
                    c.BankName,
                   c.AccountNumber,
                   c.Branch, c.Remarks,
                   c.Status
                );
                //}

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
            string filename = "Concessions_Data.xls";
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
        public ActionResult Dashboard()
        {
            var statusCount = myapp.tbl_RefundRequest.GroupBy(n => n.Status)
                            .Select(n => new
                            {
                                Status = n.Key,
                                Count = n.Count()
                            }).ToList();
            ViewBag.TotalRequests = statusCount.Sum(l => l.Count);
            ViewBag.OpenRequests = statusCount.Where(l => l.Status != "Closed" && l.Status != "Rejected").Sum(l => l.Count);
            ViewBag.RejectedRequests = statusCount.Where(l => l.Status == "Rejected").Sum(l => l.Count);
            ViewBag.CompletedRequests = statusCount.Where(l => l.Status == "Closed").Sum(l => l.Count);
            return View();
        }
        public ActionResult getDashboardData(string type)
        {
            if (type == "ByLocation")
            {
                var data = myapp.tbl_RefundRequest.GroupBy(n => n.LocationId)
                                 .Select(n => new
                                 {
                                     key = myapp.tbl_Location.FirstOrDefault(l => l.LocationId == n.Key).LocationName,
                                     count = n.Count()
                                 }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (type == "ByJustification")
            {
                var data = myapp.tbl_RefundRequest.GroupBy(n => n.Justification)
                                 .Select(n => new
                                 {
                                     key = n.Key,
                                     count = n.Count()
                                 }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            else if (type == "ByType")
            {
                var data = myapp.tbl_RefundRequest.GroupBy(n => n.Column1)
                                 .Select(n => new
                                 {
                                     key = n.Key,
                                     count = n.Count()
                                 }).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        public ActionResult Help()
        {
            return View();
        }
    }
}