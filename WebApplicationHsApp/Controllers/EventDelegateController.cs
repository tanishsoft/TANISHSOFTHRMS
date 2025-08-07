using QRCoder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    public class EventDelegateController : Controller
    {
        private MyIntranetAppEntities EventManagement = new MyIntranetAppEntities();
        // GET: Event/EventDelegate
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult SaveEventDelegateDetails(tbl_EventManageDelegate eventDelegate, HttpPostedFileBase[] httpPostedFileBase)
        {
            string result = string.Empty;
            try
            {
                Hashtable hashtable = new Hashtable
                {
                    { "Document1", string.Empty },
                    { "Document2", string.Empty }
                };
                tbl_EventManageDelegate tbl_EventManageDelegate = null;
                int eventDelegateId = eventDelegate.EventDelegateId;
                string path = Server.MapPath("~/UploadFiles/Event Delegate Documents/");
                if (httpPostedFileBase != null && httpPostedFileBase.Length > 0)
                {
                    foreach (HttpPostedFileBase file in httpPostedFileBase)
                    {
                        if (file != null && !string.IsNullOrEmpty(file.FileName))
                        {
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string extension = Path.GetExtension(file.FileName);
                            fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                            if (file.FileName == eventDelegate.Document1)
                            {
                                hashtable["Document1"] = fileName;
                                eventDelegate.Document1 = fileName;
                            }
                            else if (file.FileName == eventDelegate.Document2)
                            {
                                hashtable["Document2"] = fileName;
                                eventDelegate.Document2 = fileName;
                            }
                            file.SaveAs(Path.Combine(Server.MapPath("~/UploadFiles/Event Delegate Documents/"), fileName));
                        }
                    }
                }
                if (eventDelegate.EventDelegateId > 0)
                {
                    tbl_EventManageDelegate = EventManagement.tbl_EventManageDelegate.Where(l => l.EventDelegateId == eventDelegate.EventDelegateId).SingleOrDefault();
                    tbl_EventManageDelegate.BookingFromDate = eventDelegate.BookingFromDate;
                    tbl_EventManageDelegate.BookingToDate = eventDelegate.BookingToDate;
                    tbl_EventManageDelegate.Comments = eventDelegate.Comments;
                    tbl_EventManageDelegate.DelegateId = eventDelegate.DelegateId;
                    tbl_EventManageDelegate.EventDelegateId = eventDelegate.EventDelegateId;
                    tbl_EventManageDelegate.EventId = eventDelegate.EventId;
                    tbl_EventManageDelegate.Workshop = eventDelegate.Workshop;
                    tbl_EventManageDelegate.Conference = eventDelegate.Conference;
                    tbl_EventManageDelegate.IsWelcomeKitReceived = false;
                    if (!string.IsNullOrEmpty(hashtable["Document1"].ToString()))
                    {
                        if (System.IO.File.Exists(path + tbl_EventManageDelegate.Document1) && !string.IsNullOrEmpty(tbl_EventManageDelegate.Document1))
                        {
                            System.IO.File.Delete(path + tbl_EventManageDelegate.Document1);
                        }

                        tbl_EventManageDelegate.Document1 = eventDelegate.Document1;
                    }
                    if (!string.IsNullOrEmpty(hashtable["Document2"].ToString()))
                    {
                        if (System.IO.File.Exists(path + tbl_EventManageDelegate.Document2) && !string.IsNullOrEmpty(tbl_EventManageDelegate.Document2))
                        {
                            System.IO.File.Delete(path + tbl_EventManageDelegate.Document2);
                        }

                        tbl_EventManageDelegate.Document2 = eventDelegate.Document2;
                    }
                    tbl_EventManageDelegate.ModifiedOn = DateTime.Now;
                    tbl_EventManageDelegate.ModifiedBy = eventDelegate.CreatedBy;
                }
                else
                {
                    eventDelegate.CreatedOn = DateTime.Now;
                    eventDelegate.IsActive = true;
                    eventDelegate.IsWelcomeKitReceived = false;
                    EventManagement.tbl_EventManageDelegate.Add(eventDelegate);
                }

                EventManagement.SaveChanges();
                if (eventDelegate.EventDelegateId > 0 || tbl_EventManageDelegate.EventDelegateId > 0)
                {

                    var tbl_Delegate = EventManagement.tbl_Delegate.Where(l => l.DelegateId == tbl_EventManageDelegate.DelegateId).SingleOrDefault();
                    var eventdetails = EventManagement.tbl_EventManage.Where(l => l.EventId == tbl_EventManageDelegate.EventId).SingleOrDefault();
                    string EventDelegateId = eventDelegate.EventDelegateId.ToString();
                    if (tbl_Delegate.EmailId != null && tbl_Delegate.EmailId != "" && tbl_Delegate != null && eventdetails != null)
                    {
                        QRCodeGenerator QrGenerator = new QRCodeGenerator();
                        QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(EventDelegateId, QRCodeGenerator.ECCLevel.Q);
                        QRCode QrCode = new QRCode(QrCodeInfo);
                        Bitmap QrBitmap = QrCode.GetGraphic(60);
                        byte[] BitmapArray = QrBitmap.BitmapToByteArray();
                        //string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));

                        //Byte[] bitmapData = Convert.FromBase64String(FixBase64ForImage(QrUri));
                        System.IO.MemoryStream streamBitmap = new System.IO.MemoryStream(BitmapArray);

                        var mealTable = "<table style='border:1px solid #eee;width: 60%;'>";
                        mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>Event Name</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + eventdetails.Name + "</td></tr>";
                        mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>Event Date</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + (eventdetails.EventStartDate.HasValue ? eventdetails.EventStartDate.Value.ToString("dd/MM/yyyy hh:mm tt") : "") + " to " + (eventdetails.EventEndDate.HasValue ? eventdetails.EventEndDate.Value.ToString("dd/MM/yyyy hh:mm tt") : "") + "</td></tr>";
                        mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>Location</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + eventdetails.EventLocation + "</td></tr>";
                        mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>Event Delegate Id</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + EventDelegateId + "</td></tr>";
                        mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>Registed By </td><td style='border:1px solid #eee;font-family:Segoe UI'>" + User.Identity.Name + "</td></tr>";
                        mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>QR Code</td><td style='border:1px solid #eee;font-family:Segoe UI'><img src='cid:MyImage' style='width:150px;' alt='" + EventDelegateId + "' /></td></tr>";
                        mealTable += "</table>";
                        string Subject = "";
                        string body = "Dear " + tbl_Delegate.Name + " , <br/> Please find below are the event " + eventdetails.Name + " details. ";
                        body += mealTable;
                        Subject = "Registration Successfull for the event " + eventdetails.Name + "";
                        CustomModel cm = new CustomModel();
                        MailModel mailmodel = new MailModel
                        {
                            fromemail = "info@fernandez.foundation",
                            toemail = tbl_Delegate.EmailId,
                            subject = Subject,
                            body = body,
                            filepath = "",
                            fromname = "",
                            ccemail = "ahmadali@fernandez.foundation"
                        };
                        var imageToInline = new LinkedResource(streamBitmap, MediaTypeNames.Image.Jpeg);
                        imageToInline.ContentId = "MyImage";
                        mailmodel.linkedResource = new List<LinkedResource>();
                        mailmodel.linkedResource.Add(imageToInline);
                        cm.SendEmail(mailmodel);
                    }
                    result = eventDelegateId > 0 ? "Event Delegate Successfully Updated" : "Event Delegate Successfully Saved";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveEventDelegatePaymentDetails(tbl_EventManageDelegatePayment eventDelegatePayment, HttpPostedFileBase[] httpPostedFileBase)
        {
            string result = string.Empty;
            try
            {
                Hashtable hashtable = new Hashtable
                {
                    { "Document1", string.Empty },
                    { "Document2", string.Empty }
                };
                tbl_EventManageDelegatePayment tbl_EventDelegatePayment = null;
                int eventDelegatePaymentId = eventDelegatePayment.EventDelegatePaymentId;
                string path = Server.MapPath("~/UploadFiles/Event Delegate Payment Documents/");
                if (httpPostedFileBase != null && httpPostedFileBase.Length > 0)
                {
                    foreach (HttpPostedFileBase file in httpPostedFileBase)
                    {
                        if (file != null && !string.IsNullOrEmpty(file.FileName))
                        {
                            if (!Directory.Exists(path))
                            {
                                Directory.CreateDirectory(path);
                            }
                            string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                            string extension = Path.GetExtension(file.FileName);
                            fileName = fileName + DateTime.Now.ToString("yymmssff") + extension;
                            if (file.FileName == eventDelegatePayment.Document1)
                            {
                                hashtable["Document1"] = fileName;
                                eventDelegatePayment.Document1 = fileName;
                            }
                            else if (file.FileName == eventDelegatePayment.Document2)
                            {
                                hashtable["Document2"] = fileName;
                                eventDelegatePayment.Document2 = fileName;
                            }

                            file.SaveAs(Path.Combine(Server.MapPath("~/UploadFiles/Event Delegate Payment Documents/"), fileName));
                        }
                    }
                }
                if (eventDelegatePayment.EventDelegatePaymentId > 0)
                {
                    tbl_EventDelegatePayment = EventManagement.tbl_EventManageDelegatePayment.Where(l => l.EventDelegatePaymentId == eventDelegatePayment.EventDelegatePaymentId).SingleOrDefault();
                    tbl_EventDelegatePayment.Amount = eventDelegatePayment.Amount;
                    tbl_EventDelegatePayment.DateOfPayment = eventDelegatePayment.DateOfPayment;
                    tbl_EventDelegatePayment.EventId = eventDelegatePayment.EventId;
                    tbl_EventDelegatePayment.DelegateId = eventDelegatePayment.DelegateId;
                    tbl_EventDelegatePayment.PaymentId = eventDelegatePayment.PaymentId;
                    tbl_EventDelegatePayment.Remarks = eventDelegatePayment.Remarks;
                    if (!string.IsNullOrEmpty(hashtable["Document1"].ToString()))
                    {
                        if (System.IO.File.Exists(path + tbl_EventDelegatePayment.Document1) && !string.IsNullOrEmpty(tbl_EventDelegatePayment.Document1))
                        {
                            System.IO.File.Delete(path + tbl_EventDelegatePayment.Document1);
                        }

                        tbl_EventDelegatePayment.Document1 = eventDelegatePayment.Document1;
                    }
                    if (!string.IsNullOrEmpty(hashtable["Document2"].ToString()))
                    {
                        if (System.IO.File.Exists(path + tbl_EventDelegatePayment.Document2) && !string.IsNullOrEmpty(tbl_EventDelegatePayment.Document2))
                        {
                            System.IO.File.Delete(path + tbl_EventDelegatePayment.Document2);
                        }

                        tbl_EventDelegatePayment.Document2 = eventDelegatePayment.Document2;
                    }
                    tbl_EventDelegatePayment.ModifiedOn = DateTime.Now;
                    tbl_EventDelegatePayment.ModifiedBy = eventDelegatePayment.CreatedBy;
                }
                else
                {
                    eventDelegatePayment.CreatedOn = DateTime.Now;
                    eventDelegatePayment.IsActive = true;
                    EventManagement.tbl_EventManageDelegatePayment.Add(eventDelegatePayment);
                }

                EventManagement.SaveChanges();
                if (eventDelegatePayment.EventDelegatePaymentId > 0 || tbl_EventDelegatePayment.EventDelegatePaymentId > 0)
                {
                    result = "Event Delegate Payment Successfully " + (eventDelegatePaymentId > 0 ? "Updated" : "Saved");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveDelegateMealType(tbl_EventManageDelegateMealType eventDelegateMealType)
        {
            string result = string.Empty;
            try
            {
                tbl_EventManageDelegateMealType tbl_EventDelegateMealType = null;
                int eventDelegateMealTypeId = eventDelegateMealType.EventDelegateMealTypeId;
                if (eventDelegateMealType.EventDelegateMealTypeId > 0)
                {
                    tbl_EventDelegateMealType = EventManagement.tbl_EventManageDelegateMealType.Where(l => l.EventDelegateMealTypeId == eventDelegateMealType.EventDelegateMealTypeId).SingleOrDefault();
                    tbl_EventDelegateMealType.Qty = eventDelegateMealType.Qty;
                    tbl_EventDelegateMealType.EventDate = eventDelegateMealType.EventDate;
                    tbl_EventDelegateMealType.EventId = eventDelegateMealType.EventId;
                    tbl_EventDelegateMealType.MealTypeId = eventDelegateMealType.MealTypeId;
                    tbl_EventDelegateMealType.DelegateId = eventDelegateMealType.DelegateId;
                    tbl_EventDelegateMealType.Comments = eventDelegateMealType.Comments;
                    tbl_EventDelegateMealType.ModifiedOn = DateTime.Now;
                    tbl_EventDelegateMealType.ModifiedBy = eventDelegateMealType.CreatedBy;
                }
                else
                {
                    eventDelegateMealType.CreatedOn = DateTime.Now;
                    eventDelegateMealType.IsActive = true;
                    EventManagement.tbl_EventManageDelegateMealType.Add(eventDelegateMealType);
                }
                EventManagement.SaveChanges();
                if (eventDelegateMealType.EventDelegateMealTypeId > 0 || tbl_EventDelegateMealType.EventDelegateMealTypeId > 0)
                {
                    result = "Delegate MealType Successfully " + (eventDelegateMealTypeId > 0 ? "Updated" : "Saved");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteEventDelegatePayment(int eventDelegatePaymentId)
        {
            try
            {
                var eventDelegatePayment = EventManagement.tbl_EventManageDelegatePayment.Where(x => x.EventDelegatePaymentId == eventDelegatePaymentId).FirstOrDefault();
                if (eventDelegatePayment != null)
                {
                    EventManagement.tbl_EventManageDelegatePayment.Remove(eventDelegatePayment);
                    EventManagement.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteDelegateMealType(int eventDelegateMealTypeId)
        {
            try
            {
                var eventDelegateMealType = EventManagement.tbl_EventManageDelegateMealType.Where(x => x.EventDelegateMealTypeId == eventDelegateMealTypeId).FirstOrDefault();
                if (eventDelegateMealType != null)
                {
                    EventManagement.tbl_EventManageDelegateMealType.Remove(eventDelegateMealType);
                    EventManagement.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveDelegateMealTypeHis(EventManageDelegateMealTypeHistoryModel Type)
        {
            string result = string.Empty;
            tbl_EventManageDelegateMealTypeHistory His = new tbl_EventManageDelegateMealTypeHistory();
            His.Comments = Type.Comments;
            His.CreatedBy = User.Identity.Name;
            His.CreatedOn = DateTime.Now;
            His.DelegateId = Type.DelegateId;
            His.EventDate = ProjectConvert.ConverDateStringtoDatetime(Type.EventDate);
            His.EventId = Type.EventId;
            His.IsActive = true;
            His.MealTypeId = Type.MealTypeId;
            His.Qty = Type.Qty;
            var List = EventManagement.tbl_EventManageDelegateMealTypeHistory.Where(l => l.DelegateId == His.DelegateId && l.EventId == His.EventId && l.EventDate == His.EventDate).ToList();
            if (List != null && List.Count() != 0)
            {
                result = "Meal type already exist";
            }
            else
            {
                EventManagement.tbl_EventManageDelegateMealTypeHistory.Add(His);
                EventManagement.SaveChanges();
                result = "Successfully inserted";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}