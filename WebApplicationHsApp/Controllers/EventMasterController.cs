using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers
{
    public class EventMasterController : Controller
    {
        private MyIntranetAppEntities EventManagement = new MyIntranetAppEntities();
        // GET: EventMaster
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ManageMealType()
        {
            return View();
        }
        public ActionResult ManagePaymentType()
        {
            return View();
        }
        public ActionResult ManageDelegate()
        {
            return View();
        }
        public ActionResult GetActiveMealDetails(JQueryDataTableParamModel param)
        {
            List<tbl_MealType> query = (from d in EventManagement.tbl_MealType select d).ToList();
            int count = 0;
            IEnumerable<tbl_MealType> mealTypes;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                mealTypes = query
                    .Where(x => x.Name.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || x.Description.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || (string.IsNullOrEmpty(x.Document1) ? string.Empty : x.Document1).ToUpper().Contains(param.sSearch.ToUpper())
                    || x.IsActive.ToString().ToUpper().Contains(param.sSearch.ToUpper()));
            }
            else
            {
                mealTypes = query;
            }
            IEnumerable<tbl_MealType> meals = mealTypes.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in meals
                                               //where c.IsActive !=false
                                           select new object[] {
                             "0",
                             c.Name!=null?c.Name.ToString():"",
                             c.Description,
                             //string.IsNullOrEmpty(c.Document1)?string.Empty:c.Document1,
                             c.IsActive.HasValue?c.IsActive.ToString():"false",
                             c.MealTypeId.ToString() ,
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
        public JsonResult SaveMealDetails(tbl_MealType mealType)
        {
            string result = string.Empty;
            try
            {
                tbl_MealType meal = null;
                int mealTypeId = mealType.MealTypeId;


                if (mealType.MealTypeId > 0)
                {
                    meal = EventManagement.tbl_MealType.Where(l => l.MealTypeId == mealType.MealTypeId).SingleOrDefault();
                    meal.ModifiedOn = DateTime.Now;
                    meal.Document1 = "";
                    meal.IsActive = true;
                    meal.Name = mealType.Name;
                    meal.ModifiedBy = mealType.CreatedBy;
                }
                else
                {
                    mealType.CreatedOn = DateTime.Now;
                    mealType.IsActive = true;
                    EventManagement.tbl_MealType.Add(mealType);
                }
                EventManagement.SaveChanges();
                if (mealType.MealTypeId > 0 || meal.MealTypeId > 0)
                {
                    result = mealTypeId > 0 ? "Meal successfully updated" : "Meal successfully saved";
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMealDetails(int mealTypeId)
        {
            tbl_MealType model = EventManagement.tbl_MealType.Where(X => X.MealTypeId == mealTypeId).SingleOrDefault();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateMealStatus(int mealTypeId)
        {
            try
            {
                tbl_MealType meal = EventManagement.tbl_MealType.Where(x => x.MealTypeId == mealTypeId).FirstOrDefault();
                if (meal != null)
                {
                    EventManagement.tbl_MealType.Remove(meal);
                    EventManagement.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkMealStatus(string name, int mealTypeId)
        {
            bool status = true;
            if (mealTypeId > 0)
            {
                status = EventManagement.tbl_MealType.Select(x => x.Name.ToUpper() == name.Trim().ToUpper() && x.MealTypeId != mealTypeId).FirstOrDefault();
            }
            {
                status = EventManagement.tbl_MealType.Select(x => x.Name.ToUpper() == name.Trim().ToUpper()).FirstOrDefault();
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActivePaymentDetails(JQueryDataTableParamModel param)
        {
            List<tbl_Payment> query = (from d in EventManagement.tbl_Payment select d).ToList();
            int count = 0;
            IEnumerable<tbl_Payment> payments;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                payments = query.Where(x => x.Name.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                x.Description.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                x.IsCredit.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                x.IsDebit.ToString().ToUpper().Contains(param.sSearch.ToUpper()) ||
                x.IsActive.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                );
            }
            else
            {
                payments = query;
            }
            IEnumerable<tbl_Payment> paymentDetails = payments.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in paymentDetails
                                           select new object[] {
                             c.PaymentId.ToString() ,
                              c.Name,
                             c.Description,
                             c.IsCredit.HasValue?c.IsCredit.ToString():"false",
                             c.IsDebit.HasValue?c.IsDebit.ToString():"false",
                             c.IsActive.HasValue?c.IsActive.ToString():"false",
                             c.PaymentId.ToString() ,
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
        public JsonResult SavePaymentDetails(tbl_Payment payment)
        {
            string result = string.Empty;
            try
            {
                tbl_Payment tbl_Payment = null;
                int paymentId = payment.PaymentId;
                if (payment.PaymentId > 0)
                {
                    tbl_Payment = EventManagement.tbl_Payment.Where(l => l.PaymentId == payment.PaymentId).SingleOrDefault();
                    tbl_Payment.Name = payment.Name;
                    tbl_Payment.Description = payment.Description;
                    tbl_Payment.IsDebit = payment.IsDebit;
                    tbl_Payment.IsCredit = payment.IsCredit;
                    tbl_Payment.ModifiedOn = DateTime.Now;
                    tbl_Payment.ModifiedBy = payment.ModifiedBy;
                }
                else
                {
                    payment.CreatedOn = DateTime.Now;
                    payment.IsActive = true;
                    EventManagement.tbl_Payment.Add(payment);
                }
                EventManagement.SaveChanges();
                if (payment.PaymentId > 0 || tbl_Payment.PaymentId > 0)
                {
                    result = paymentId > 0 ? "payment successfully updated" : "payment successfully saved";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPaymentDetails(int PaymentId)
        {
            tbl_Payment model = EventManagement.tbl_Payment.Where(X => X.PaymentId == PaymentId).SingleOrDefault();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeletePayment(int PaymentId)
        {
            try
            {
                tbl_Payment payment = EventManagement.tbl_Payment.Where(x => x.PaymentId == PaymentId).FirstOrDefault();
                if (payment != null)
                {
                    EventManagement.tbl_Payment.Remove(payment);
                    EventManagement.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult checkPaymentStatus(string name, int paymentId)
        {
            bool status = true;
            if (paymentId > 0)
            {
                status = EventManagement.tbl_Payment.Select(x => x.Name.ToUpper() == name.Trim().ToUpper() && x.PaymentId != paymentId).FirstOrDefault();
            }
            {
                status = EventManagement.tbl_Payment.Select(x => x.Name.ToUpper() == name.Trim().ToUpper()).FirstOrDefault();
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetActiveDelegateDetails(JQueryDataTableParamModel param)
        {
            try
            {
                List<tbl_Delegate> query = (from d in EventManagement.tbl_Delegate select d).ToList();
                int count = 0;
                IEnumerable<tbl_Delegate> delegates;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    delegates = query.Where(x => x.Name.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || x.EmailId.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || x.PhoneNumber.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || x.Gender.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || x.Address.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || x.Qualification.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || x.Comments.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || x.Status.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || x.RegistrationNo.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || x.City.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || x.State.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || x.Title.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    || (string.IsNullOrEmpty(x.Document1) ? string.Empty : x.Document1.ToUpper()).Contains(param.sSearch.ToUpper())
                    || (string.IsNullOrEmpty(x.Document2) ? string.Empty : x.Document2.ToUpper()).Contains(param.sSearch.ToUpper())
                    || (string.IsNullOrEmpty(x.Document3) ? string.Empty : x.Document3.ToUpper()).Contains(param.sSearch.ToUpper())
                    || (string.IsNullOrEmpty(x.Document4) ? string.Empty : x.Document4.ToUpper()).Contains(param.sSearch.ToUpper())
                    || x.IsActive.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    );
                }
                else
                {
                    delegates = query;
                }
                IEnumerable<tbl_Delegate> delegateDetails = delegates.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<object[]> result = from x in delegateDetails
                                               select new object[] {
                             x.DelegateId.ToString() ,
                             string.IsNullOrEmpty(x.Title)?string.Empty:x.Title,
                            x.Name,
                            x.EmailId,
                            x.PhoneNumber,
                            x.Gender,
                            x.Address,
                            string.IsNullOrEmpty(x.State)?string.Empty:x.State,
                            string.IsNullOrEmpty(x.City)?string.Empty:x.City,
                             string.IsNullOrEmpty(x.RegistrationNo)?string.Empty:x.RegistrationNo,
                            x.Qualification,
                            x.Comments,
                            x.Status,
                            string.IsNullOrEmpty(x.Document1)?string.Empty:x.Document1,
                            string.IsNullOrEmpty(x.Document2)?string.Empty:x.Document2,
                            string.IsNullOrEmpty(x.Document3)?string.Empty:x.Document3,
                            string.IsNullOrEmpty(x.Document4)?string.Empty:x.Document4,
                            x.IsActive.HasValue?x.IsActive.ToString():"false",
                              x.DelegateId.ToString() ,
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
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult SaveDelegateDetails(tbl_Delegate _Delegate, HttpPostedFileBase[] httpPostedFileBase)
        {
            string result = string.Empty;
            try
            {
                Hashtable hashtable = new Hashtable
                {
                    { "Document1", string.Empty },
                    { "Document2", string.Empty },
                    { "Document3", string.Empty },
                    { "Document4", string.Empty }
                };
                tbl_Delegate tbl_Delegate = null;
                int delegateId = _Delegate.DelegateId;
                string path = Server.MapPath("~/UploadFiles/DelegateDocuments/");
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
                            if (file.FileName == _Delegate.Document1)
                            {
                                hashtable["Document1"] = fileName;
                                _Delegate.Document1 = fileName;
                            }
                            else if (file.FileName == _Delegate.Document2)
                            {
                                hashtable["Document2"] = fileName;
                                _Delegate.Document2 = fileName;
                            }
                            else if (file.FileName == _Delegate.Document3)
                            {
                                hashtable["Document3"] = fileName;
                                _Delegate.Document3 = fileName;
                            }
                            else if (file.FileName == _Delegate.Document4)
                            {
                                hashtable["Document4"] = fileName;
                                _Delegate.Document4 = fileName;
                            }
                            file.SaveAs(Path.Combine(Server.MapPath("~/UploadFiles/DelegateDocuments/"), fileName));
                        }
                    }
                }
                if (_Delegate.DelegateId > 0)
                {
                    tbl_Delegate = EventManagement.tbl_Delegate.Where(l => l.DelegateId == _Delegate.DelegateId).SingleOrDefault();
                    tbl_Delegate.Name = _Delegate.Name;
                    tbl_Delegate.EmailId = _Delegate.EmailId;
                    tbl_Delegate.PhoneNumber = _Delegate.PhoneNumber;
                    tbl_Delegate.Gender = _Delegate.Gender;
                    tbl_Delegate.Address = _Delegate.Address;
                    tbl_Delegate.Qualification = _Delegate.Qualification;
                    tbl_Delegate.Comments = _Delegate.Comments;
                    tbl_Delegate.State = _Delegate.State;
                    tbl_Delegate.City = _Delegate.City;
                    tbl_Delegate.Title = _Delegate.Title;
                    tbl_Delegate.RegistrationNo = _Delegate.RegistrationNo;
                    if (!string.IsNullOrEmpty(hashtable["Document1"].ToString()))
                    {
                        if (System.IO.File.Exists(path + tbl_Delegate.Document1) && !string.IsNullOrEmpty(tbl_Delegate.Document1))
                        {
                            System.IO.File.Delete(path + tbl_Delegate.Document1);
                        }

                        tbl_Delegate.Document1 = _Delegate.Document1;
                    }
                    if (!string.IsNullOrEmpty(hashtable["Document2"].ToString()))
                    {
                        if (System.IO.File.Exists(path + tbl_Delegate.Document2) && !string.IsNullOrEmpty(tbl_Delegate.Document2))
                        {
                            System.IO.File.Delete(path + tbl_Delegate.Document2);
                        }

                        tbl_Delegate.Document2 = _Delegate.Document2;
                    }
                    if (!string.IsNullOrEmpty(hashtable["Document3"].ToString()) && !string.IsNullOrEmpty(tbl_Delegate.Document3))
                    {
                        if (System.IO.File.Exists(path + tbl_Delegate.Document3) && !string.IsNullOrEmpty(tbl_Delegate.Document3))
                        {
                            System.IO.File.Delete(path + tbl_Delegate.Document3);
                        }

                        tbl_Delegate.Document3 = _Delegate.Document3;
                    }
                    if (!string.IsNullOrEmpty(hashtable["Document4"].ToString()) && !string.IsNullOrEmpty(tbl_Delegate.Document4))
                    {
                        if (System.IO.File.Exists(path + tbl_Delegate.Document4) && !string.IsNullOrEmpty(tbl_Delegate.Document4))
                        {
                            System.IO.File.Delete(path + tbl_Delegate.Document4);
                        }

                        tbl_Delegate.Document4 = _Delegate.Document4;
                    }
                    tbl_Delegate.ModifiedOn = DateTime.Now;
                    tbl_Delegate.ModifiedBy = _Delegate.CreatedBy;
                }
                else
                {
                    _Delegate.Document1 = string.IsNullOrEmpty(hashtable["Document1"].ToString()) ? string.Empty : hashtable["Document1"].ToString();
                    _Delegate.Document2 = string.IsNullOrEmpty(hashtable["Document2"].ToString()) ? string.Empty : hashtable["Document2"].ToString();
                    _Delegate.Document3 = string.IsNullOrEmpty(hashtable["Document3"].ToString()) ? string.Empty : hashtable["Document3"].ToString();
                    _Delegate.Document4 = string.IsNullOrEmpty(hashtable["Document4"].ToString()) ? string.Empty : hashtable["Document4"].ToString();
                    _Delegate.CreatedOn = DateTime.Now;
                    _Delegate.IsActive = true;
                    EventManagement.tbl_Delegate.Add(_Delegate);
                }
                EventManagement.SaveChanges();
                if (_Delegate.DelegateId > 0 || tbl_Delegate.DelegateId > 0)
                {
                    result = delegateId > 0 ? "Delegate successfully updated" : "Delegate successfully saved";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveDelegateDetailsNew(EventDelegateViewModel model)
        {
            string result = string.Empty;
            try
            {
                tbl_EventManage evmodel = EventManagement.tbl_EventManage.Where(X => X.EventId == model.EventId).SingleOrDefault();
                tbl_Delegate modelDelegate = new tbl_Delegate
                {
                    Address = model.Address,
                    City = model.City,
                    Comments = model.Comments,
                    CreatedBy = User.Identity.Name,
                    EmailId = model.EmailId,
                    Gender = model.Gender,
                    ModifiedBy = User.Identity.Name,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    Qualification = model.Qualification,
                    RegistrationNo = model.RegistrationNo,
                    State = model.State,
                    Status = model.Status,
                    Title = model.Title,
                    ModifiedOn = DateTime.Now,
                    Document1 = "",
                    Document2 = "",
                    Document3 = "",
                    Document4 = model.Document4,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    CompanyName = model.CompanyName
                };
                EventManagement.tbl_Delegate.Add(modelDelegate);
                EventManagement.SaveChanges();
                if (modelDelegate.DelegateId > 0)
                {
                    tbl_EventManageDelegate modeldelevt = new tbl_EventManageDelegate
                    {
                        BookingFromDate = evmodel.EventStartDate,
                        BookingToDate = evmodel.EventEndDate,
                        Comments = "",
                        Conference = model.Conference,
                        CreatedBy = User.Identity.Name,
                        CreatedOn = DateTime.Now,
                        DelegateId = modelDelegate.DelegateId,
                        Document1 = "",
                        Document2 = "",
                        EventId = model.EventId,
                        IsActive = true,
                        ModifiedBy = User.Identity.Name,
                        ModifiedOn = DateTime.Now,
                        Workshop = model.Workshop,
                        BankName = model.BankName,
                        CCdetails = model.CCdetails,
                        Discount = (model.Discount != null && model.Discount != "") ? Convert.ToDecimal(model.Discount) : 0,
                        FinalBlanceTopay = (model.FinalBlanceTopay != null && model.FinalBlanceTopay != "") ? Convert.ToDecimal(model.FinalBlanceTopay) : 0,
                        PaymentType = model.PaymentType,
                        TotalAmount = (model.TotalAmount != null && model.TotalAmount != "") ? Convert.ToDecimal(model.TotalAmount) : 0,
                        UTRTransaction = model.UTRTransaction
                    };
                    EventManagement.tbl_EventManageDelegate.Add(modeldelevt);
                    EventManagement.SaveChanges();
                    result = modeldelevt.EventDelegateId.ToString();
                    if (model.EventDelegateMethodModel != null && model.EventDelegateMethodModel.Count > 0)
                    {
                        foreach (EventManageDelegateMethodViewModel evtmethod in model.EventDelegateMethodModel)
                        {

                            tbl_EventManageDelegateMethod modelevtmethod = new tbl_EventManageDelegateMethod
                            {
                                EventId = model.EventId,
                                CreatedBy = User.Identity.Name,
                                CreatedOn = DateTime.Now,
                                DelegateId = modelDelegate.DelegateId,
                                EventDelegateId = modeldelevt.EventDelegateId,
                                EventDate = ProjectConvert.ConverDateStringtoDatetime(evtmethod.EventDate),
                                IsActive = true,
                                ConferenceAmount = evtmethod.ConferenceAmount,
                                IsConference = evtmethod.IsConference,
                                IsWorkshop = evtmethod.IsWorkshop,
                                WorkshopAmount = evtmethod.WorkshopAmount,
                                Amount = evtmethod.Amount
                            };
                            EventManagement.tbl_EventManageDelegateMethod.Add(modelevtmethod);
                            EventManagement.SaveChanges();
                        }
                    }
                    //if (model.EventDelegateMealTypeModel != null && model.EventDelegateMealTypeModel.Count > 0)
                    //{
                    //    foreach (EventManageDelegateMealTypeViewModel evtdelMt in model.EventDelegateMealTypeModel)
                    //    {

                    //        tbl_EventManageDelegateMealType modelevtdelmt = new tbl_EventManageDelegateMealType
                    //        {
                    //            EventId = model.EventId,
                    //            Comments = "",
                    //            CreatedBy = User.Identity.Name,
                    //            CreatedOn = DateTime.Now,
                    //            DelegateId = modelDelegate.DelegateId,
                    //            EventDate = ProjectConvert.ConverDateStringtoDatetime(evtdelMt.EventDate),
                    //            IsActive = true,
                    //            MealTypeId = evtdelMt.MealTypeId,
                    //            Qty = evtdelMt.Qty,
                    //            ModifiedBy = User.Identity.Name,
                    //            ModifiedOn = DateTime.Now                               
                    //        };
                    //        EventManagement.tbl_EventManageDelegateMealType.Add(modelevtdelmt);
                    //        EventManagement.SaveChanges();
                    //    }
                    //}
                }
                CustomModel cm = new CustomModel();
                MailModel mailmodel = new MailModel
                {
                    fromemail = "it_helpdesk@fernandez.foundation"
                };

                mailmodel.toemail = model.EmailId;
                mailmodel.ccemail = "ahmadali@fernandez.foundation";
                //  mailmodel.subject = "Ticket " + task.TaskId + " " + task.Subject.Substring(0, 50) + " - " + task.CreatorLocationName + " " + task.CreatorDepartmentName;
                mailmodel.subject = "Thank you for registering the Event " + evmodel.Name + " on " + evmodel.EventStartDate.Value.ToString("dd/MM/yyyy");

                if (mailmodel.toemail != null && mailmodel.toemail != "")
                {
                    string mailbody = "<p style='font-family:verdana;font-size:15px;'>Dear " + model.Name + ",</p>";
                    mailbody += "<p style='font-family:verdana;font-size:14px;'>Thank you for registering the Event. Please find below are the details,</p><table style='width:60%;margin:auto;border:solid 1px #a1a2a3;'>";
                    mailbody += "<tr><td style='border:solid 1px #a1a2a3;font-size:14px;'>Event Name</td><td  style='font-family:verdana;border:solid 1px #a1a2a3;font-size:14px;'>" + evmodel.Name + "</td></tr>";
                    mailbody += "<tr><td style='border:solid 1px #a1a2a3;font-size:14px;'>Event Date</td><td  style='font-family:verdana;border:solid 1px #a1a2a3;font-size:14px;'>" + evmodel.EventStartDate.Value.ToString("dd/MM/yyyy") + "</td></tr>";
                    mailbody += "<tr><td style='border:solid 1px #a1a2a3;font-size:14px;'>Venue</td><td  style='font-family:verdana;border:solid 1px #a1a2a3;font-size:14px;'>" + evmodel.EventLocation + "</td></tr>";
                    string date = evmodel.EventStartDate.Value.ToString("ddMMyyyy");
                    string barcode = evmodel.EventId + "-" + modelDelegate.DelegateId + "-" + result;
                    ViewBag.FullEventDelegateId = date + evmodel.EventId + result;
                    mailbody += "<tr><td style='border:solid 1px #a1a2a3;font-size:14px;'>Total Amount Paid</td><td  style='font-family:verdana;border:solid 1px #a1a2a3;font-size:14px;'>" + model.TotalAmount + "</td></tr>";
                    mailbody += "<tr><td style='border:solid 1px #a1a2a3;font-size:14px;'>Bar Co</td><td  style='font-family:verdana;border:solid 1px #a1a2a3;font-size:14px;'>" + model.TotalAmount + "</td></tr>";
                    // mailbody += "<tr><td style='border:solid 1px #a1a2a3;font-size:14px;'>Is Approved?</td><td  style='font-family:verdana;border:solid 1px #a1a2a3;font-size:14px;'>Pending For Approval</td></tr>";
                    mailbody += "</table><br /><p style='font-family:verdana;font-size:12px;'>This is an auto generated mail,Don't reply to this.</p>";
                    mailmodel.body = mailbody;
                    mailmodel.filepath = "";
                    mailmodel.fromname = "Delegate Registration for the event " + evmodel.Name;
                    if (mailmodel.ccemail == null || mailmodel.ccemail == "")
                    {
                        mailmodel.ccemail = "";
                    }

                    cm.SendEmail(mailmodel);
                }
                if (model.PhoneNumber != null && model.PhoneNumber != "")
                {
                    SendSms sms = new SendSms();
                    sms.SendSmsToEmployee(model.PhoneNumber, mailmodel.subject);
                }
                //result = modeldelevt. "Delegate successfully updated";

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDelegateDetails(int delagateId)
        {
            try
            {
                tbl_Delegate _Delegare = EventManagement.tbl_Delegate.Where(X => X.DelegateId == delagateId).SingleOrDefault();
                return Json(_Delegare, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public JsonResult DeleteDelegate(int delagateId)
        {
            try
            {
                tbl_Delegate _Delegare = EventManagement.tbl_Delegate.Where(x => x.DelegateId == delagateId).FirstOrDefault();
                if (_Delegare != null)
                {
                    EventManagement.tbl_Delegate.Remove(_Delegare);
                    EventManagement.SaveChanges();
                }
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public JsonResult checkDelegateStatus(string name, string EmailId, int delegateId)
        {
            bool status = true;
            if (delegateId > 0)
            {
                status = EventManagement.tbl_Delegate.Select(x => x.Name.ToUpper() == name.Trim().ToUpper() && x.EmailId.ToUpper() == EmailId.ToUpper() && x.DelegateId != delegateId).FirstOrDefault();
            }
            {
                status = EventManagement.tbl_Delegate.Select(x => x.Name.ToUpper() == name.Trim().ToUpper() && x.EmailId.ToUpper() == EmailId.ToUpper()).FirstOrDefault();
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPaymentAllDetails()
        {
            SelectList result = new SelectList(EventManagement.tbl_Payment, "PaymentId", "Name");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEventDetails()
        {
            SelectList result = new SelectList(EventManagement.tbl_Event, "EventId", "Name");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDelegateAllDetails()
        {
            SelectList result = new SelectList(EventManagement.tbl_Delegate, "DelegateId", "Name");
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}