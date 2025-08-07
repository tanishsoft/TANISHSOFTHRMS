//using QRCoder;
using QRCoder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Drawing.Imaging;
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
    public class EventController : Controller
    {

        private MyIntranetAppEntities EventManagement = new MyIntranetAppEntities();
        // GET: Event/Event
        public ActionResult Index()
        {
            List<tbl_EventManage> list = EventManagement.tbl_EventManage.Where(l => l.IsActive == true).OrderByDescending(l => l.EventStartDate).ToList();
            return View(list);
        }
        // GET: Event
        public ActionResult EventDetails()
        {
            return View();
        }
        public ActionResult AddEvent()
        {
            return View();
        }
        public ActionResult EditEventDetails()
        {
            return View();
        }
        public ActionResult ViewEvent()
        {
            return View();
        }
        public ActionResult ViewDelegates(int id)
        {
            ViewBag.EventId = id;
            return View();
        }
        public ActionResult ViewPayments(int id)
        {
            ViewBag.EventId = id;
            return View();
        }
        #region ajax calls     
        /// <summary>
        ///  Getting the event Delegate categories.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public ActionResult GetActiveEventDetails(JQueryDataTableParamModel param)
        {
            List<tbl_EventManage> query = (from d in EventManagement.tbl_EventManage select d).OrderByDescending(l => l.EventStartDate).ToList();
            int count = 0;
            IEnumerable<tbl_EventManage> events;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                events = query
                    .Where(x => x.Name.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || x.Description.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || x.EventLocation.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || Convert.ToDateTime(x.EventStartDate).ToString("dd-MM-yyyy").ToUpper().Contains(param.sSearch.ToUpper())
                   || Convert.ToDateTime(x.EventEndDate).ToString("dd-MM-yyyy").ToUpper().Contains(param.sSearch.ToUpper())
                   || x.EventComments.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || x.EventStatus.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || x.NumberOfSeats.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || x.IsActive.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || x.Workshop.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || x.Conference.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   );
            }
            else
            {
                events = query;
            }
            IEnumerable<tbl_EventManage> displayedEvents = events.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedEvents
                                               //where c.IsActive !=false
                                           select new object[] {
                             "0",
                             c.Name!=null?c.Name.ToString():"",
                             c.EventStartDate!=null?Convert.ToDateTime(c.EventStartDate).ToString("dd-MM-yyyy"):"",
                              c.EventEndDate!=null?Convert.ToDateTime(c.EventEndDate).ToString("dd-MM-yyyy"):"",
                             c.EventLocation,
                             c.NumberOfSeats,
                             c.EventStatus,
                             c.Description,
                             c.Workshop.HasValue?c.Workshop.ToString():"false",
                             c.Conference.HasValue?c.Conference.ToString():"false",
                             c.IsActive.HasValue?c.IsActive.ToString():"false",
                             c.EventId.ToString() ,
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

        public ActionResult GetUpcomingEventDetails(JQueryDataTableParamModel param)
        {
            List<tbl_EventManage> query = (from d in EventManagement.tbl_EventManage select d).Where(l => l.EventStartDate >= DateTime.Now).ToList();
            int count = 0;
            IEnumerable<tbl_EventManage> events;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                events = query
                    .Where(x => x.Name.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || x.Description.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || x.EventLocation.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || Convert.ToDateTime(x.EventStartDate).ToString("dd-MM-yyyy").ToUpper().Contains(param.sSearch.ToUpper())
                   || Convert.ToDateTime(x.EventEndDate).ToString("dd-MM-yyyy").ToUpper().Contains(param.sSearch.ToUpper())
                   || x.EventComments.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || x.EventStatus.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || x.NumberOfSeats.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || x.IsActive.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || x.Workshop.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   || x.Conference.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                   );
            }
            else
            {
                events = query;
            }
            IEnumerable<tbl_EventManage> displayedEvents = events.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in displayedEvents
                                               //where c.IsActive !=false
                                           select new object[] {
                             "0",
                             c.Name.ToString(),
                             Convert.ToDateTime(c.EventStartDate).ToString("dd-MM-yyyy"),
                             Convert.ToDateTime(c.EventEndDate).ToString("dd-MM-yyyy"),
                             c.EventLocation,
                             c.NumberOfSeats,
                             c.EventStatus,
                             c.Description,
                             c.Workshop.HasValue?c.Workshop.ToString():"false",
                             c.Conference.HasValue?c.Conference.ToString():"false",
                             c.IsActive.HasValue?c.IsActive.ToString():"false",
                             c.EventId.ToString() ,
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
        public JsonResult GetAllEvents()
        {
            List<tbl_EventManage> list = EventManagement.tbl_EventManage.ToList();
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveEventDetails(tbl_EventManage _event, HttpPostedFileBase[] httpPostedFileBase)
        {
            string result = string.Empty;
            try
            {
                Hashtable hashtable = new Hashtable
                {
                    { "Document1", string.Empty },
                    { "Document2", string.Empty },
                    { "Document3", string.Empty }
                };
                tbl_EventManage tbl_EventManage = null;
                int eventId = _event.EventId;
                string path = Server.MapPath("~/Uploads/");
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
                            fileName = fileName.Replace(" ", "");
                            fileName = fileName.Replace("&", "and");
                            if (file.FileName == _event.Document1)
                            {
                                hashtable["Document1"] = fileName;
                                _event.Document1 = fileName;
                            }
                            else if (file.FileName == _event.Document2)
                            {
                                hashtable["Document2"] = fileName;
                                _event.Document2 = fileName;
                            }
                            else if (file.FileName == _event.Document3)
                            {
                                hashtable["Document3"] = fileName;
                                _event.Document3 = fileName;
                            }
                            file.SaveAs(Path.Combine(Server.MapPath("~/Uploads/"), fileName));
                        }
                    }
                }
                if (_event.EventId > 0)
                {
                    tbl_EventManage = EventManagement.tbl_EventManage.Where(l => l.EventId == _event.EventId).SingleOrDefault();
                    tbl_EventManage.Name = _event.Name;
                    tbl_EventManage.Description = _event.Description;
                    tbl_EventManage.EventComments = _event.EventComments;
                    tbl_EventManage.EventEndDate = _event.EventEndDate;
                    tbl_EventManage.EventStartDate = _event.EventStartDate;
                    tbl_EventManage.EventLocation = _event.EventLocation;
                    tbl_EventManage.EventStatus = _event.EventStatus;
                    tbl_EventManage.NumberOfSeats = _event.NumberOfSeats;
                    tbl_EventManage.Workshop = _event.Workshop;
                    tbl_EventManage.Conference = _event.Conference;
                    if (!string.IsNullOrEmpty(hashtable["Document1"].ToString()))
                    {
                        if (System.IO.File.Exists(path + tbl_EventManage.Document1) && !string.IsNullOrEmpty(tbl_EventManage.Document1))
                        {
                            System.IO.File.Delete(path + tbl_EventManage.Document1);
                        }

                        tbl_EventManage.Document1 = _event.Document1;
                    }
                    if (!string.IsNullOrEmpty(hashtable["Document2"].ToString()))
                    {
                        if (System.IO.File.Exists(path + tbl_EventManage.Document2) && !string.IsNullOrEmpty(tbl_EventManage.Document2))
                        {
                            System.IO.File.Delete(path + tbl_EventManage.Document2);
                        }

                        tbl_EventManage.Document2 = _event.Document2;
                    }
                    if (!string.IsNullOrEmpty(hashtable["Document3"].ToString()) && !string.IsNullOrEmpty(tbl_EventManage.Document3))
                    {
                        if (System.IO.File.Exists(path + tbl_EventManage.Document3) && !string.IsNullOrEmpty(tbl_EventManage.Document3))
                        {
                            System.IO.File.Delete(path + tbl_EventManage.Document3);
                        }

                        tbl_EventManage.Document3 = _event.Document3;
                    }
                    tbl_EventManage.ModifiedOn = DateTime.Now;
                    tbl_EventManage.ModifiedBy = _event.CreatedBy;
                }
                else
                {
                    _event.CreatedOn = DateTime.Now;
                    _event.IsActive = true;
                    EventManagement.tbl_EventManage.Add(_event);
                }

                EventManagement.SaveChanges();
                if (_event.EventId > 0 || tbl_EventManage.EventId > 0)
                {
                    result = (_event.EventId > 0 ? _event.EventId : tbl_EventManage.EventId).ToString();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SaveMealTypes(EventManageMealTypeViewModel model)
        {
            tbl_EventManageMealType eventmodel = new tbl_EventManageMealType();
            eventmodel.Comments = model.Comments;
            eventmodel.CreatedBy = User.Identity.Name;
            eventmodel.CreatedOn = DateTime.Now;
            eventmodel.EventDate = ProjectConvert.ConverDateStringtoDatetime(model.EventDate);
            eventmodel.EventId = model.EventId;
            eventmodel.EventMealTypeId = model.EventMealTypeId;
            eventmodel.IsActive = true;
            eventmodel.MealTypeId = model.MealTypeId;
            eventmodel.ModifiedBy = User.Identity.Name;
            eventmodel.ModifiedOn = DateTime.Now;
            if (eventmodel.EventMealTypeId < 1)
            {
                eventmodel.CreatedOn = DateTime.Now;
                EventManagement.tbl_EventManageMealType.Add(eventmodel);
            }
            int insertedRecords = EventManagement.SaveChanges();
            return Json(insertedRecords);
        }
        public JsonResult SaveConferenceandworkshop(EventMethodModel model)
        {
            tbl_EventManageMethod Con = new tbl_EventManageMethod();
            Con.Amount = model.Amount;
            Con.ConferenceAmount = model.ConferenceAmount;
            Con.EventDate = ProjectConvert.ConverDateStringtoDatetime(model.EventDate);
            Con.EventId = model.EventId;
            //Con.EventTypeMethodId = model.EventTypeMethodId;
            Con.WorkshopAmount = model.WorkshopAmount;
            Con.IsWorkshop = model.IsWorkshop;
            Con.IsConference = model.IsConference;
            Con.CreatedOn = DateTime.Now;
            Con.IsActive = true;
            Con.CreatedBy = User.Identity.Name;
            EventManagement.tbl_EventManageMethod.Add(Con);
            int insertedRecords = EventManagement.SaveChanges();
            return Json(insertedRecords);
        }
        public JsonResult GetconandworkshopFullDetails(int eventId)
        {
            List<tbl_EventManageMethod> List = EventManagement.tbl_EventManageMethod.Where(l => l.EventId == eventId).ToList();
            return Json(List, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMealTypes(int eventId)
        {
            SelectList selectListItems = null;
            if (eventId > 0)
            {
                selectListItems = new SelectList((from d in EventManagement.tbl_MealType
                                                  join tbl_EventManageMealType in EventManagement.tbl_EventManageMealType on d.MealTypeId equals tbl_EventManageMealType.MealTypeId
                                                  where tbl_EventManageMealType.EventId == eventId
                                                  select d).Distinct(), "MealTypeId", "Name");
            }
            else
            {
                selectListItems = new SelectList(EventManagement.tbl_MealType, "MealTypeId", "Name");
            }
            return Json(selectListItems, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMealDetails(int mealTypeId)
        {
            tbl_MealType model = EventManagement.tbl_MealType.Where(X => X.MealTypeId == mealTypeId).SingleOrDefault();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteEvent(int eventId)
        {
            try
            {
                tbl_EventManage _Event = EventManagement.tbl_EventManage.Where(x => x.EventId == eventId).FirstOrDefault();
                if (_Event != null)
                {
                    EventManagement.tbl_EventManage.Remove(_Event);
                    IQueryable<tbl_EventManageMealType> tbl_EventManageMealType = EventManagement.tbl_EventManageMealType.Where(x => x.EventId == eventId);
                    if (tbl_EventManageMealType != null)
                    {
                        foreach (tbl_EventManageMealType eventMealType in tbl_EventManageMealType)
                        {
                            if (eventMealType.EventId == eventId)
                            {
                                EventManagement.tbl_EventManageMealType.Remove(eventMealType);
                            }
                        }
                    }
                    EventManagement.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteMealType(int EventMealTypeId)
        {
            try
            {
                tbl_EventManageMealType tbl_EventManageMealType = EventManagement.tbl_EventManageMealType.Where(x => x.EventMealTypeId == EventMealTypeId).FirstOrDefault();
                if (tbl_EventManageMealType != null)
                {
                    EventManagement.tbl_EventManageMealType.Remove(tbl_EventManageMealType);
                }
                EventManagement.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEventDetails(int eventId)
        {
            tbl_EventManage model = EventManagement.tbl_EventManage.Where(X => X.EventId == eventId).SingleOrDefault();
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMealTypeDetails(int eventId)
        {
            IQueryable<tbl_EventManageMealType> model = EventManagement.tbl_EventManageMealType.Where(X => X.EventId == eventId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMealTypeFullDetails(int eventId)
        {
            List<tbl_MealType> MealType = EventManagement.tbl_MealType.ToList();
            List<tbl_EventManageMealType> model = EventManagement.tbl_EventManageMealType.Where(X => X.EventId == eventId).ToList();
            var data = (from mt in MealType
                        join m in model on mt.MealTypeId equals m.MealTypeId
                        select new
                        {
                            m.MealTypeId,
                            mt.Name
                        }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetDelegatePaymentDetails(JQueryDataTableParamModel param, int eventId)
        {
            List<tbl_EventManageDelegate> eventdelegates = (from d in EventManagement.tbl_EventManageDelegate where d.EventId == eventId select d).ToList();
            List<tbl_Delegate> delegates = (from d in EventManagement.tbl_Delegate select d).ToList();
            tbl_EventManage events = EventManagement.tbl_EventManage.Where(l => l.EventId == eventId).SingleOrDefault();
            List<tbl_User> users = EventManagement.tbl_User.Where(l => l.IsActive == true).ToList();
            //List<tbl_Payment> payments = (from d in EventManagement.tbl_Payment select d).ToList();
            var query = (from d in eventdelegates
                         join del in delegates on d.DelegateId equals del.DelegateId
                         select new EventDelegateViewModel()
                         {
                             Address = del.Address,
                             BankName = d.BankName,
                             BookingFromDate = events.EventStartDate.Value.ToString("dd/MM/yyyy"),
                             BookingToDate = events.EventEndDate.Value.ToString("dd/MM/yyyy"),
                             CCdetails = d.CCdetails,
                             City = del.City,
                             Comments = d.Comments,
                             CreatedBy = d.CreatedBy,
                             CreatedOn = d.CreatedOn,
                             DelegateId = d.DelegateId.Value,
                             Discount = d.Discount.Value.ToString(),
                             EmailId = del.EmailId,
                             EventId = d.EventId,
                             FinalBlanceTopay = d.FinalBlanceTopay.Value.ToString(),
                             Gender = del.Gender,
                             IsActive = d.IsActive,
                             Name = del.Name,
                             PaymentType = d.PaymentType,
                             PhoneNumber = del.PhoneNumber,
                             Qualification = del.Qualification,
                             RegistrationNo = del.RegistrationNo,
                             State = del.State,
                             Status = del.Status,
                             Title = del.Title,
                             TotalAmount = d.TotalAmount.Value.ToString(),
                             UTRTransaction = d.UTRTransaction,
                             EventName = events.Name

                         }).ToList();
            IEnumerable<EventDelegateViewModel> eventPayments;
            int count = 0;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                eventPayments = query
                    .Where(x => x.Name.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                     || x.PaymentType.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                     || x.TotalAmount.ToUpper().Contains(param.sSearch.ToUpper())
                     || x.PhoneNumber.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                     || x.EmailId.ToString().ToUpper().Contains(param.sSearch.ToUpper())

                       ).OrderByDescending(c => c.DelegateId);
            }
            else
            {
                eventPayments = query;
            }
            IEnumerable<EventDelegateViewModel> paymentDetails = eventPayments.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in eventPayments
                                           join u in users on c.CreatedBy equals u.CustomUserId
                                           select new object[] {
                            c.EventName,
                             c.DelegateId.ToString() ,
                             c.Name,
                             c.PhoneNumber,
                             c.EmailId,
                             c.PaymentType,
                             c.TotalAmount,
                            c.Discount,
                            c.FinalBlanceTopay,
                            c.BankName,
                            c.CCdetails,
                            c.UTRTransaction,
                            u.FirstName
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
        public JsonResult GetPaymentDetails(JQueryDataTableParamModel param, int eventId)
        {
            List<tbl_EventManagePayment> query = (from d in EventManagement.tbl_EventManagePayment select d).ToList();
            List<tbl_EventManage> events = (from d in EventManagement.tbl_EventManage select d).ToList();
            List<tbl_Payment> payments = (from d in EventManagement.tbl_Payment select d).ToList();
            IEnumerable<tbl_EventManagePayment> eventPayments;
            int count = 0;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                eventPayments = query
                    .Where(x => x.EventPaymentId.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                     || x.Amount.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                     || x.DateOfPayment.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                     || x.Remarks.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                     || x.IsActive.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                     || events.Where(y => y.EventId == x.EventId).FirstOrDefault() == null ? 1 == 1 : events.Where(y => y.EventId == x.EventId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                     || payments.Where(y => y.PaymentId == x.PaymentId).FirstOrDefault() == null ? 1 == 1 : payments.Where(y => y.PaymentId == x.PaymentId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                       ).OrderByDescending(c => c.PaymentId);
            }
            else
            {
                eventPayments = query;
            }
            IEnumerable<tbl_EventManagePayment> paymentDetails = eventPayments.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from c in eventPayments
                                           join tbl_EventManage in EventManagement.tbl_EventManage on c.EventId equals tbl_EventManage.EventId
                                           join tbl_Payment in EventManagement.tbl_Payment on c.PaymentId equals tbl_Payment.PaymentId
                                           where c.EventId == eventId
                                           select new object[] {
                             "0" ,
                             tbl_EventManage.Name ,
                             tbl_Payment.Name,
                             c.Amount,
                             Convert.ToDateTime(c.DateOfPayment).ToString("dd-MM-yyyy"),
                             c.Remarks,
                             c.IsActive.HasValue?c.IsActive.ToString():"false",
                             c.EventPaymentId.ToString() ,
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
        public ActionResult GetEventDelegateDetails(JQueryDataTableParamModel param, int eventId)
        {
            try
            {
                List<tbl_EventManageDelegate> query = (from d in EventManagement.tbl_EventManageDelegate select d).Where(l => l.EventId == eventId).ToList();
                List<tbl_EventManage> events = (from d in EventManagement.tbl_EventManage select d).Where(l => l.EventId == eventId).ToList();
                List<tbl_Delegate> delegates = (from d in EventManagement.tbl_Delegate select d).ToList();
                int count = 0;
                IEnumerable<tbl_EventManageDelegate> eventDelegates;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    eventDelegates = query.Where(x => x.Comments.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                      || events.Where(y => y.EventId == x.EventId).FirstOrDefault() == null ? 1 == 1 : events.Where(y => y.EventId == x.EventId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                     || delegates.Where(y => y.DelegateId == x.DelegateId).FirstOrDefault() == null ? 1 == 1 : delegates.Where(y => y.DelegateId == x.DelegateId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                     || Convert.ToDateTime(x.BookingFromDate).ToString("dd-MM-yyyy").Contains(param.sSearch.ToUpper())
                     || Convert.ToDateTime(x.BookingToDate).ToString("dd-MM-yyyy").Contains(param.sSearch.ToUpper())
                      || x.Workshop.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                      || x.Conference.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                      || x.IsActive.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    );
                }
                else
                {
                    eventDelegates = query;
                }
                IEnumerable<tbl_EventManageDelegate> eventDelegatesDetails = eventDelegates.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<object[]> result = from x in eventDelegatesDetails
                                               join ev in events on x.EventId equals ev.EventId
                                               join del in delegates on x.DelegateId equals del.DelegateId
                                               select new object[] {
                                    x.EventDelegateId.ToString() ,
                                    ev.Name,
                                    del.Name,
                                    Convert.ToDateTime(x.BookingFromDate).ToString("dd-MM-yyyy"),
                                    Convert.ToDateTime(x.BookingToDate).ToString("dd-MM-yyyy"),
                                    x.Comments,
                                    x.Workshop.HasValue?x.Workshop.ToString():"false",
                                    x.Conference.HasValue?x.Conference.ToString():"false",
                                    x.PaymentType,
                                    x.BankName,
                                    x.CCdetails,
                                     x.UTRTransaction,
                                    x.TotalAmount.HasValue? x.TotalAmount.ToString():"0",
                                     x.Discount.HasValue? x.Discount.ToString():"0",
                                    x.FinalBlanceTopay.HasValue? x.FinalBlanceTopay.ToString():"0",

                                    x.IsActive.HasValue?x.IsActive.ToString():"false",
                                    x.EventDelegateId.ToString() ,
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

        public ActionResult GetFullEventDelegateDetails(JQueryDataTableParamModel param, int eventId)
        {
            try
            {
                List<tbl_EventManageDelegate> query = (from d in EventManagement.tbl_EventManageDelegate select d).Where(l => l.EventId == eventId).ToList();
                List<tbl_EventManage> events = (from d in EventManagement.tbl_EventManage select d).Where(l => l.EventId == eventId).ToList();
                List<tbl_Delegate> delegates = (from d in EventManagement.tbl_Delegate select d).ToList();
                List<tbl_User> users = EventManagement.tbl_User.Where(l => l.IsActive == true).ToList();
                int count = 0;
                IEnumerable<tbl_EventManageDelegate> eventDelegates;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    eventDelegates = query.Where(x => x.Comments.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                      || events.Where(y => y.EventId == x.EventId).FirstOrDefault() == null ? 1 == 1 : events.Where(y => y.EventId == x.EventId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                     || delegates.Where(y => y.DelegateId == x.DelegateId).FirstOrDefault() == null ? 1 == 1 : delegates.Where(y => y.DelegateId == x.DelegateId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                     || Convert.ToDateTime(x.BookingFromDate).ToString("dd-MM-yyyy").Contains(param.sSearch.ToUpper())
                     || Convert.ToDateTime(x.BookingToDate).ToString("dd-MM-yyyy").Contains(param.sSearch.ToUpper())
                      || x.Workshop.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                      || x.Conference.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                      || x.IsActive.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    );
                }
                else
                {
                    eventDelegates = query;
                }
                IEnumerable<tbl_EventManageDelegate> eventDelegatesDetails = eventDelegates.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<object[]> result = from x in eventDelegatesDetails
                                               join ev in events on x.EventId equals ev.EventId
                                               join del in delegates on x.DelegateId equals del.DelegateId
                                               join u in users on x.CreatedBy equals u.CustomUserId
                                               select new object[] {
                                                   x.EventDelegateId.ToString(),
                                                   ev.Name,
                                                   del.Name,
                                                   x.BookingFromDate.HasValue?x.BookingFromDate.Value.ToString("dd-MM-yyyy"):"",
                                                   x.BookingToDate.HasValue?x.BookingToDate.Value.ToString("dd-MM-yyyy"):"",
                                                   x.Comments,
                                                   x.IsWelcomeKitReceived.HasValue?x.IsWelcomeKitReceived.Value.ToString():"false",
                                                   x.IsActive.HasValue? x.IsActive.ToString():"false",
                                                   del.Status,
                                                   u.FirstName,
                                                   (ev.EventStartDate.Value.ToString("ddMMyyyy")+ "-" + ev.EventId + "-" + del.DelegateId + "-" + x.EventDelegateId).ToString(),
                                                   x.EventDelegateId.ToString()
                                               };
                count = result.Count();
                return Json(new
                {
                    sEcho = param.sEcho,
                    iTotalRecords = query.Count(),
                    iTotalDisplayRecords = eventDelegates.Count(),
                    aaData = result
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult GetEventMealTypeHistory(JQueryDataTableParamModel param, int eventId)
        {
            try
            {
                List<tbl_EventManageDelegateMealTypeHistory> query = (from d in EventManagement.tbl_EventManageDelegateMealTypeHistory select d).Where(l => l.EventId == eventId && l.IsActive == true).ToList();
                List<tbl_EventManage> events = (from d in EventManagement.tbl_EventManage select d).Where(l => l.EventId == eventId).ToList();
                List<tbl_Delegate> delegates = (from d in EventManagement.tbl_Delegate select d).ToList();
                List<tbl_MealType> MealType = (from d in EventManagement.tbl_MealType select d).ToList();
                int count = 0;
                IEnumerable<tbl_EventManageDelegateMealTypeHistory> eventDelegates;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    eventDelegates = query.Where(x => x.Comments.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                      || events.Where(y => y.EventId == x.EventId).FirstOrDefault() == null ? 1 == 1 : events.Where(y => y.EventId == x.EventId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                     || delegates.Where(y => y.DelegateId == x.DelegateId).FirstOrDefault() == null ? 1 == 1 : delegates.Where(y => y.DelegateId == x.DelegateId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                      || x.IsActive.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    );
                }
                else
                {
                    eventDelegates = query;
                }
                IEnumerable<tbl_EventManageDelegateMealTypeHistory> eventDelegatesDetails = eventDelegates.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<object[]> result = from x in eventDelegatesDetails
                                               join ev in events on x.EventId equals ev.EventId
                                               join del in delegates on x.DelegateId equals del.DelegateId
                                               join mel in MealType on x.MealTypeId equals mel.MealTypeId
                                               select new object[] {
                                    x.EventDelegateMealTypeHistoryId.ToString() ,
                                    ev.Name,
                                    del.Name,
                                    mel.Name,
                                    Convert.ToDateTime(x.EventDate).ToString("dd-MM-yyyy"),
                                    x.Comments,
                                    x.IsActive.HasValue?x.IsActive.ToString():"false",
                                    x.EventDelegateMealTypeHistoryId.ToString() ,
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
        public ActionResult GetEventDelegateDetailsWelcome(JQueryDataTableParamModel param, int eventId)
        {
            try
            {
                List<tbl_EventManageDelegate> query = (from d in EventManagement.tbl_EventManageDelegate select d).Where(l => l.EventId == eventId && l.IsWelcomeKitReceived == true).ToList();
                List<tbl_EventManage> events = (from d in EventManagement.tbl_EventManage select d).Where(l => l.EventId == eventId).ToList();
                List<tbl_Delegate> delegates = (from d in EventManagement.tbl_Delegate select d).ToList();
                int count = 0;
                IEnumerable<tbl_EventManageDelegate> eventDelegates;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    eventDelegates = query.Where(x => x.Comments.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                      || events.Where(y => y.EventId == x.EventId).FirstOrDefault() == null ? 1 == 1 : events.Where(y => y.EventId == x.EventId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                     || delegates.Where(y => y.DelegateId == x.DelegateId).FirstOrDefault() == null ? 1 == 1 : delegates.Where(y => y.DelegateId == x.DelegateId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                     || Convert.ToDateTime(x.BookingFromDate).ToString("dd-MM-yyyy").Contains(param.sSearch.ToUpper())
                     || Convert.ToDateTime(x.BookingToDate).ToString("dd-MM-yyyy").Contains(param.sSearch.ToUpper())
                      || x.Workshop.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                      || x.Conference.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                      || x.IsActive.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                    );
                }
                else
                {
                    eventDelegates = query;
                }
                IEnumerable<tbl_EventManageDelegate> eventDelegatesDetails = eventDelegates.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<object[]> result = from x in eventDelegatesDetails
                                               join ev in events on x.EventId equals ev.EventId
                                               join del in delegates on x.DelegateId equals del.DelegateId
                                               select new object[] {
                                    x.EventDelegateId.ToString() ,
                                    ev.Name,
                                    del.Name,
                                    Convert.ToDateTime(x.BookingFromDate).ToString("dd-MM-yyyy"),
                                    Convert.ToDateTime(x.BookingToDate).ToString("dd-MM-yyyy"),
                                    x.Comments,
                                    x.Workshop.HasValue?x.Workshop.ToString():"false",
                                    x.Conference.HasValue?x.Conference.ToString():"false",
                                    x.PaymentType,
                                    x.FinalBlanceTopay.HasValue? x.FinalBlanceTopay.ToString():"0",

                                    x.IsWelcomeKitReceived.HasValue?x.IsWelcomeKitReceived.ToString():"false",
                                    x.EventDelegateId.ToString() ,
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
        public ActionResult GetEventDelegatePaymentDetails(JQueryDataTableParamModel param, int eventId)
        {
            try
            {
                List<tbl_EventManageDelegatePayment> query = (from d in EventManagement.tbl_EventManageDelegatePayment select d).ToList();
                List<tbl_EventManage> events = (from d in EventManagement.tbl_EventManage select d).ToList();
                List<tbl_Delegate> delegates = (from d in EventManagement.tbl_Delegate select d).ToList();
                List<tbl_Payment> payments = (from d in EventManagement.tbl_Payment select d).ToList();
                int count = 0;
                IEnumerable<tbl_EventManageDelegatePayment> eventDelegatePayments;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    eventDelegatePayments = query.Where(x => x.Remarks.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                                            || events.Where(y => y.EventId == x.EventId).FirstOrDefault() == null ? 1 == 1 : events.Where(y => y.EventId == x.EventId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                                            || delegates.Where(y => y.DelegateId == x.DelegateId).FirstOrDefault() == null ? 1 == 1 : delegates.Where(y => y.DelegateId == x.DelegateId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                                            || payments.Where(y => y.PaymentId == x.PaymentId).FirstOrDefault() == null ? 1 == 1 : payments.Where(y => y.PaymentId == x.PaymentId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                                            || x.Amount.ToString().Contains(param.sSearch.ToUpper())
                                            || Convert.ToDateTime(x.DateOfPayment).ToString("dd-MM-yyyy").Contains(param.sSearch.ToUpper())
                                            || x.IsActive.ToString().ToUpper().Contains(param.sSearch.ToUpper()));
                }
                else
                {
                    eventDelegatePayments = query;
                }
                IEnumerable<tbl_EventManageDelegatePayment> eventDelegatesDetails = eventDelegatePayments.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<object[]> result = from x in eventDelegatesDetails
                                               join tbl_EventManage in EventManagement.tbl_EventManage on x.EventId equals tbl_EventManage.EventId
                                               join tbl_Delegate in EventManagement.tbl_Delegate on x.DelegateId equals tbl_Delegate.DelegateId
                                               join tbl_Payment in EventManagement.tbl_Payment on x.PaymentId equals tbl_Payment.PaymentId
                                               where x.EventId == eventId
                                               select new object[] {
                                    x.EventDelegatePaymentId.ToString() ,
                                    tbl_EventManage.Name,
                                    tbl_Delegate.Name,
                                    tbl_Payment.Name,
                                    x.Amount,
                                    Convert.ToDateTime(x.DateOfPayment).ToString("dd-MM-yyyy"),
                                     x.Remarks,
                                    x.IsActive.HasValue?x.IsActive.ToString():"false",
                                    x.EventDelegatePaymentId.ToString() ,
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
        public ActionResult GetEventDelegateMealTypeDetails(JQueryDataTableParamModel param, int eventId)
        {
            List<tbl_EventManageDelegateMealType> query = (from d in EventManagement.tbl_EventManageDelegateMealType select d).ToList();
            List<tbl_EventManage> events = (from d in EventManagement.tbl_EventManage select d).ToList();
            List<tbl_Delegate> delegates = (from d in EventManagement.tbl_Delegate select d).ToList();
            List<tbl_MealType> mealTypes = (from d in EventManagement.tbl_MealType select d).ToList();
            int count = 0;
            IEnumerable<tbl_EventManageDelegateMealType> tbl_EventManageDelegateMealTypes;
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                tbl_EventManageDelegateMealTypes = query.Where(x => x.Comments.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                                            || events.Where(y => y.EventId == x.EventId).FirstOrDefault() == null ? 1 == 1 : events.Where(y => y.EventId == x.EventId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                                            || delegates.Where(y => y.DelegateId == x.DelegateId).FirstOrDefault() == null ? 1 == 1 : delegates.Where(y => y.DelegateId == x.DelegateId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                                            || mealTypes.Where(y => y.MealTypeId == x.MealTypeId).FirstOrDefault() == null ? 1 == 1 : mealTypes.Where(y => y.MealTypeId == x.MealTypeId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                                            || x.Qty.ToString().Contains(param.sSearch.ToUpper())
                                            || Convert.ToDateTime(x.EventDate).ToString("dd-MM-yyyy").Contains(param.sSearch.ToUpper())
                                            || x.IsActive.ToString().ToUpper().Contains(param.sSearch.ToUpper()));
            }
            else
            {
                tbl_EventManageDelegateMealTypes = query;
            }
            IEnumerable<tbl_EventManageDelegateMealType> eventDelegatesDetails = tbl_EventManageDelegateMealTypes.Skip(param.iDisplayStart).Take(param.iDisplayLength);
            IEnumerable<object[]> result = from x in eventDelegatesDetails
                                           join tbl_EventManage in EventManagement.tbl_EventManage on x.EventId equals tbl_EventManage.EventId
                                           join tbl_MealType in EventManagement.tbl_MealType on x.MealTypeId equals tbl_MealType.MealTypeId
                                           join tbl_Delegate in EventManagement.tbl_Delegate on x.DelegateId equals tbl_Delegate.DelegateId
                                           where x.EventId == eventId
                                           select new object[] {
                                    x.EventDelegateMealTypeId.ToString() ,
                                    tbl_EventManage.Name,
                                        tbl_MealType.Name,
                                    tbl_Delegate.Name,
                                    Convert.ToDateTime(x.EventDate).ToString("dd-MM-yyyy"),
                                     x.Qty,
                                     x.Comments,
                                    x.IsActive.HasValue?x.IsActive.ToString():"false",
                                    x.EventDelegateMealTypeId.ToString() ,
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
        public ActionResult GetEventDelegateMealTypeHistory(JQueryDataTableParamModel param, int eventId)
        {
            try
            {
                List<tbl_EventManageDelegateMealTypeHistory> query = (from d in EventManagement.tbl_EventManageDelegateMealTypeHistory select d).ToList();
                List<tbl_EventManage> events = (from d in EventManagement.tbl_EventManage select d).ToList();
                List<tbl_Delegate> delegates = (from d in EventManagement.tbl_Delegate select d).ToList();
                List<tbl_MealType> mealTypes = (from d in EventManagement.tbl_MealType select d).ToList();
                int count = 0;
                IEnumerable<tbl_EventManageDelegateMealTypeHistory> tbl_EventManageDelegateMealTypeHistories;
                if (!string.IsNullOrEmpty(param.sSearch))
                {
                    tbl_EventManageDelegateMealTypeHistories = query.Where(x => x.Comments.ToString().ToUpper().Contains(param.sSearch.ToUpper())
                                            || events.Where(y => y.EventId == x.EventId).FirstOrDefault() == null ? 1 == 1 : events.Where(y => y.EventId == x.EventId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                                            || delegates.Where(y => y.DelegateId == x.DelegateId).FirstOrDefault() == null ? 1 == 1 : delegates.Where(y => y.DelegateId == x.DelegateId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                                            || mealTypes.Where(y => y.MealTypeId == x.MealTypeId).FirstOrDefault() == null ? 1 == 1 : mealTypes.Where(y => y.MealTypeId == x.MealTypeId).FirstOrDefault().Name.ToUpper().Contains(param.sSearch.ToUpper())
                                            || x.Qty.ToString().Contains(param.sSearch.ToUpper())
                                            || Convert.ToDateTime(x.EventDate).ToString("dd-MM-yyyy").Contains(param.sSearch.ToUpper())
                                            || x.IsActive.ToString().ToUpper().Contains(param.sSearch.ToUpper()));
                }
                else
                {
                    tbl_EventManageDelegateMealTypeHistories = query;
                }
                IEnumerable<tbl_EventManageDelegateMealTypeHistory> eventDelegatesDetails = tbl_EventManageDelegateMealTypeHistories.Skip(param.iDisplayStart).Take(param.iDisplayLength);
                IEnumerable<object[]> result = from x in query
                                               join tbl_EventManage in EventManagement.tbl_EventManage on x.EventId equals tbl_EventManage.EventId
                                               join tbl_MealType in EventManagement.tbl_MealType on x.MealTypeId equals tbl_MealType.MealTypeId
                                               join tbl_Delegate in EventManagement.tbl_Delegate on x.DelegateId equals tbl_Delegate.DelegateId
                                               where x.EventId == eventId
                                               select new object[] {
                                    x.EventDelegateMealTypeHistoryId.ToString() ,
                                    tbl_EventManage.Name,
                                        tbl_MealType.Name,
                                    tbl_Delegate.Name,
                                    Convert.ToDateTime(x.EventDate).ToString("dd-MM-yyyy"),
                                     x.Qty,
                                     x.Comments,
                                    x.IsActive.HasValue?x.IsActive.ToString():"false",
                                    x.EventDelegateMealTypeHistoryId.ToString() ,
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
        public JsonResult GetDelegateId(int eventDelegateId)
        {
            IQueryable<int?> model = EventManagement.tbl_EventManageDelegate.Where(X => X.EventDelegateId == eventDelegateId).Select(x => x.DelegateId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetmealTypeId(int eventDelegateId)
        {
            IQueryable<int?> model = EventManagement.tbl_EventManageDelegateMealType.Where(X => X.DelegateId == eventDelegateId).Select(x => x.MealTypeId);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
        public ActionResult PrintIdCard(int Id)
        {
            int eventDelegateId = Id;
            string barcode = string.Empty;
            string updateimg = string.Empty;
            tbl_EventManageDelegate eventDelegate = EventManagement.tbl_EventManageDelegate.Where(x => x.EventDelegateId == eventDelegateId).FirstOrDefault();
            tbl_Delegate delegates = EventManagement.tbl_Delegate.Where(x => x.DelegateId == eventDelegate.DelegateId).FirstOrDefault();

            tbl_EventManage events = EventManagement.tbl_EventManage.Where(x => x.EventId == eventDelegate.EventId).FirstOrDefault();
            try
            {

                if (eventDelegate != null && delegates != null && events != null)
                {
                    ViewBag.DelegateName = delegates.Name;
                    ViewBag.Address = delegates.Address;
                    ViewBag.EventDelegateId = eventDelegateId;
                    //EventId-EventName-DelegateId-DelegateName-EventDeleteagteId-BookingFromDate-BookingTodate
                    string date = events.EventStartDate.Value.ToString("ddMMyyyy");
                    barcode = date + "-" + events.EventId + "-" + delegates.DelegateId + "-" + eventDelegate.EventDelegateId;
                    ViewBag.FullEventDelegateId = barcode;
                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    //    QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(barcode, QRCodeGenerator.ECCLevel.Q);
                    //    using (Bitmap bitMap = qrCode.GetGraphic(20))
                    //    {
                    //        string path = Server.MapPath("~/UploadFiles/");

                    //        bitMap.Save(path + "/" + eventDelegateId + "_barcode.png", ImageFormat.Png);
                    //        bitMap.Save(ms, ImageFormat.Png);
                    //        //updateimg = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    //        ViewBag.BarcodePath = "/UploadFiles/" + eventDelegateId + "_barcode.png";
                    //    }
                    //}

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(events);
        }
        public JsonResult GenerateBarcode(int eventDelegateId)
        {
            string barcode = string.Empty;
            string updateimg = string.Empty;
            try
            {
                tbl_EventManageDelegate eventDelegate = EventManagement.tbl_EventManageDelegate.Where(x => x.EventDelegateId == eventDelegateId).FirstOrDefault();
                tbl_Delegate delegates = EventManagement.tbl_Delegate.Where(x => x.DelegateId == eventDelegate.DelegateId).FirstOrDefault();
                tbl_EventManage events = EventManagement.tbl_EventManage.Where(x => x.EventId == eventDelegate.EventId).FirstOrDefault();
                if (eventDelegate != null && delegates != null && events != null)
                {
                    //EventId-EventName-DelegateId-DelegateName-EventDeleteagteId-BookingFromDate-BookingTodate
                    string date = events.EventStartDate.Value.ToString("ddMMyyyy");
                    barcode = events.EventId + "-" + delegates.DelegateId + "-" + eventDelegate.EventDelegateId;

                    //using (MemoryStream ms = new MemoryStream())
                    //{
                    //    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    //    QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(barcode, QRCodeGenerator.ECCLevel.Q);
                    //    using (Bitmap bitMap = qrCode.GetGraphic(20))
                    //    {
                    //        string path = Server.MapPath("~/UploadFiles/EventDocuments/");
                    //        bitMap.Save(path + "/barcode.png", ImageFormat.Png);
                    //        bitMap.Save(ms, ImageFormat.Png);
                    //        updateimg = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    //    }
                    //}

                    QRCodeGenerator QrGenerator = new QRCodeGenerator();
                    QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(eventDelegate.EventDelegateId.ToString(), QRCodeGenerator.ECCLevel.Q);
                    QRCode QrCode = new QRCode(QrCodeInfo);
                    Bitmap QrBitmap = QrCode.GetGraphic(60);
                    byte[] BitmapArray = QrBitmap.BitmapToByteArray();
                    string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
                    ViewBag.QrCodeUri = QrUri;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(updateimg, JsonRequestBehavior.AllowGet);
        }
        public void BarcodeRead(string barcode)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                //The Image is drawn based on length of Barcode text.
                using (Bitmap bitMap = new Bitmap(50, 30))
                {
                    //The Graphics library object is generated for the Image.
                    using (Graphics graphics = Graphics.FromImage(bitMap))
                    {
                        //The installed Barcode font.
                        Font oFont = new Font("IDAutomationHC39M", 16);
                        PointF point = new PointF(2f, 2f);

                        //White Brush is used to fill the Image with white color.
                        SolidBrush whiteBrush = new SolidBrush(Color.White);
                        graphics.FillRectangle(whiteBrush, 0, 0, bitMap.Width, bitMap.Height);

                        //Black Brush is used to draw the Barcode over the Image.
                        SolidBrush blackBrush = new SolidBrush(Color.Black);
                        graphics.DrawString("*" + barcode + "*", oFont, blackBrush, point);
                    }

                    //The Bitmap is saved to Memory Stream.
                    bitMap.Save(ms, ImageFormat.Png);

                    //The Image is finally converted to Base64 string.
                    ViewBag.BarcodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }


        }
        #endregion

        public JsonResult SaveEventPaymentDetails(tbl_EventManagePayment eventPayment, HttpPostedFileBase[] httpPostedFileBase)
        {
            string result = string.Empty;
            try
            {
                Hashtable hashtable = new Hashtable
                {
                    { "Document1", string.Empty },
                    { "Document2", string.Empty }
                };
                tbl_EventManagePayment tbl_EventPayment = null;
                int eventPaymentId = eventPayment.EventPaymentId;
                string path = Server.MapPath("~/UploadFiles/Event Payment Documents/");
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
                            if (file.FileName == eventPayment.Document1)
                            {
                                hashtable["Document1"] = fileName;
                                eventPayment.Document1 = fileName;
                            }
                            else if (file.FileName == eventPayment.Document2)
                            {
                                hashtable["Document2"] = fileName;
                                eventPayment.Document2 = fileName;
                            }

                            file.SaveAs(Path.Combine(Server.MapPath("~/UploadFiles/Event Payment Documents/"), fileName));
                        }
                    }
                }
                if (eventPayment.EventPaymentId > 0)
                {
                    tbl_EventPayment = EventManagement.tbl_EventManagePayment.Where(l => l.EventPaymentId == eventPayment.EventPaymentId).SingleOrDefault();
                    tbl_EventPayment.Amount = eventPayment.Amount;
                    tbl_EventPayment.DateOfPayment = eventPayment.DateOfPayment;
                    tbl_EventPayment.EventId = eventPayment.EventId;
                    tbl_EventPayment.PaymentId = eventPayment.PaymentId;
                    tbl_EventPayment.Remarks = eventPayment.Remarks;

                    if (!string.IsNullOrEmpty(hashtable["Document1"].ToString()))
                    {
                        if (System.IO.File.Exists(path + tbl_EventPayment.Document1) && !string.IsNullOrEmpty(tbl_EventPayment.Document1))
                        {
                            System.IO.File.Delete(path + tbl_EventPayment.Document1);
                        }

                        tbl_EventPayment.Document1 = eventPayment.Document1;
                    }
                    if (!string.IsNullOrEmpty(hashtable["Document2"].ToString()))
                    {
                        if (System.IO.File.Exists(path + tbl_EventPayment.Document2) && !string.IsNullOrEmpty(tbl_EventPayment.Document2))
                        {
                            System.IO.File.Delete(path + tbl_EventPayment.Document2);
                        }

                        tbl_EventPayment.Document2 = eventPayment.Document2;
                    }
                    tbl_EventPayment.ModifiedOn = DateTime.Now;
                    tbl_EventPayment.ModifiedBy = eventPayment.CreatedBy;
                }
                else
                {
                    eventPayment.CreatedOn = DateTime.Now;
                    eventPayment.IsActive = true;
                    EventManagement.tbl_EventManagePayment.Add(eventPayment);
                }

                EventManagement.SaveChanges();
                if (eventPayment.EventPaymentId > 0 || tbl_EventPayment.EventPaymentId > 0)
                {
                    result = eventPaymentId > 0 ? "Event Payment Successfully Updated" : "Event Payment Successfully Saved";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetEventDelegateMealHistory(string barcodeid)
        {
            string[] id = barcodeid.Split('-');
            string EventId = id[0];
            string DelegateId = id[1];
            string EventDelegateId = id[2];
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetDelegatemealcapture(int Eventdelegateid, int EventId)
        {
            //var list = EventManagement.tbl_EventManageDelegate.Join(EventManagement.tbl_Delegate,
            //                     EventDelegate => EventDelegate.DelegateId,
            //                     Delegate => Delegate.DelegateId,
            //                     (EventDelegate, Delegate) => new { EventDelegate = EventDelegate, Delegate = Delegate })
            //    .Where(l => l.EventDelegate.EventDelegateId == Eventdelegateid && l.e);
            var list = EventManagement.tbl_EventManageDelegate.Where(l => l.EventDelegateId == Eventdelegateid && l.EventId == EventId).ToList();
            if (list.Count > 0)
            {
                int DelegateId = list[0].DelegateId.Value;
                var delegatedet = EventManagement.tbl_Delegate.Where(l => l.DelegateId == DelegateId).SingleOrDefault();
                return Json(delegatedet, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetMealTypesformealcapture(int delegateid, string EventDate, int Eventid)
        {
            DateTime Date = ProjectConvert.ConverDateStringtoDatetime(EventDate);
            var eventmanage = EventManagement.tbl_EventManageMealType.Where(l => l.EventId == Eventid).ToList();
            eventmanage = eventmanage.Where(l => l.EventDate.Value.Date == Date.Date).ToList();
            List<int?> list = eventmanage.Select(l => l.MealTypeId).Distinct().ToList();
            List<tbl_MealType> Mealtype = (from m in EventManagement.tbl_MealType
                                           join l in list on m.MealTypeId equals l
                                           select m).Distinct().ToList();
            //var MealTypelist = EventManagement.tbl_EventManageDelegateMealType.Join(EventManagement.tbl_MealType,
            //                   DelegateMealType => DelegateMealType.MealTypeId,
            //                   MealType => MealType.MealTypeId,
            //                   (DelegateMealType, MealType) => new { DelegateMealType = DelegateMealType, id = MealType.MealTypeId, value = MealType.Name }).Where(l => l.DelegateMealType.EventId == Eventid && l.DelegateMealType.DelegateId == delegateid && l.DelegateMealType.EventDate == Date).GroupBy(x => x.id);
            return Json(Mealtype, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EDelegatewelcomekit(int EventdelegateId, int eventId)
        {
            string result = "";
            List<tbl_EventManageDelegate> list = EventManagement.tbl_EventManageDelegate.Where(l => l.EventId == eventId && l.EventDelegateId == EventdelegateId).ToList();
            if (list.Count() > 0)
            {
                if (list[0].IsWelcomeKitReceived != true)
                {
                    list[0].IsWelcomeKitReceived = true;
                    EventManagement.SaveChanges();
                    result = "Welcome kit received";
                }
                else
                {
                    result = "Welcome kit already received";
                }

            }
            else
            {
                result = "Delegate Not found";
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadDelegates(int hfileEventId, HttpPostedFileBase postedFile)
        {
            string filePath = string.Empty;
            if (postedFile != null)
            {
                string path = Server.MapPath("~/UploadFiles/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                filePath = path + Path.GetFileName(postedFile.FileName);
                string extension = Path.GetExtension(postedFile.FileName);
                postedFile.SaveAs(filePath);
                string conString03 = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = { 0 }; Extended Properties = 'Excel 8.0;HDR=YES";
                string conString07 = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                string conString = string.Empty;
                switch (extension)
                {
                    case ".xls": //Excel 97-03.
                        conString = conString03;
                        break;
                    case ".xlsx": //Excel 07 and above.
                        conString = conString07;
                        break;
                }

                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [Sheet1$]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }

                //Insert records to database table.
                var eventdetails = EventManagement.tbl_EventManage.Where(l => l.EventId == hfileEventId).SingleOrDefault();
                foreach (DataRow row in dt.Rows)
                {
                    EventDelegateExcelModel model = new EventDelegateExcelModel
                    {
                        RegNo = row["RegNo"].ToString(),
                        Delegate = row["Name"].ToString(),
                        Title = row["Title"].ToString(),
                        Organization = row["Organization"].ToString(),
                        Address = row["Address"].ToString(),
                        City = row["City"].ToString(),
                        Desig = row["Desig"].ToString(),
                        CellNumber = row["CellNumber"].ToString(),
                        EmailID = row["EmailID"].ToString(),
                        Amount = row["Amount"].ToString(),
                        ReceiptNo = row["ReceiptNo"].ToString(),
                        Modeofpayment = row["Modeofpayment"].ToString(),
                        UTRTransactionNo = row["UTRTransactionNo"].ToString(),
                        TotalDays = row["TotalDays"].ToString(),
                        Workshop = row["Workshop"].ToString(),
                        Conference = row["Conference"].ToString()
                    };

                    var delegatereg = EventManagement.tbl_Delegate.Where(l => l.PhoneNumber == model.CellNumber).FirstOrDefault();
                    int DelegateId = 0;
                    if (delegatereg == null)
                    {
                        tbl_Delegate modelDelegate = new tbl_Delegate
                        {
                            Address = model.Address,
                            City = model.City,
                            Comments = "",
                            CreatedBy = User.Identity.Name,
                            EmailId = model.EmailID,
                            Gender = "",
                            ModifiedBy = User.Identity.Name,
                            Name = model.Delegate,
                            PhoneNumber = model.CellNumber,
                            Qualification = "",
                            RegistrationNo = model.RegNo,
                            State = model.City,
                            Status = "",
                            Title = model.Title,
                            ModifiedOn = DateTime.Now,
                            Document1 = "",
                            Document2 = "",
                            Document3 = "",
                            Document4 = "",
                            CreatedOn = DateTime.Now,
                            IsActive = true
                        };
                        EventManagement.tbl_Delegate.Add(modelDelegate);
                        EventManagement.SaveChanges();
                        DelegateId = modelDelegate.DelegateId;
                    }
                    else
                    {
                        DelegateId = delegatereg.DelegateId;
                    }

                    if (DelegateId > 0)
                    {
                        var evntdeldetail = EventManagement.tbl_EventManageDelegate.Where(l => l.DelegateId == DelegateId && l.EventId == hfileEventId).Count();
                        if (evntdeldetail == 0)
                        {
                            tbl_EventManageDelegate modeldelevt = new tbl_EventManageDelegate
                            {
                                //BookingFromDate = ProjectConvert.ConverDateStringtoDatetime(model.BookingFromDate),
                                //BookingToDate = ProjectConvert.ConverDateStringtoDatetime(model.BookingToDate),
                                Comments = "",
                                Conference = model.Conference == "Yes" ? true : false,
                                CreatedBy = User.Identity.Name,
                                CreatedOn = DateTime.Now,
                                DelegateId = DelegateId,
                                Document1 = "",
                                Document2 = "",
                                BookingFromDate = eventdetails.EventStartDate,
                                BookingToDate = eventdetails.EventStartDate.Value.AddDays(int.Parse(model.TotalDays)),
                                EventId = hfileEventId,
                                IsActive = true,
                                ModifiedBy = User.Identity.Name,
                                ModifiedOn = DateTime.Now,
                                Workshop = model.Workshop == "Yes" ? true : false,
                                BankName = model.ReceiptNo,
                                CCdetails = "",
                                Discount = 0,
                                FinalBlanceTopay = (model.Amount != null && model.Amount != "") ? Convert.ToDecimal(model.Amount) : 0,
                                PaymentType = model.Modeofpayment,
                                TotalAmount = (model.Amount != null && model.Amount != "") ? Convert.ToDecimal(model.Amount) : 0,
                                UTRTransaction = model.UTRTransactionNo
                            };
                            EventManagement.tbl_EventManageDelegate.Add(modeldelevt);
                            EventManagement.SaveChanges();
                            string EventDelegateId = modeldelevt.EventDelegateId.ToString();
                            if (model.EmailID != null && model.EmailID != "")
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
                                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>Event Date</td><td style='border:1px solid #eee;font-family:Segoe UI'>" +( eventdetails.EventStartDate.HasValue ? eventdetails.EventStartDate.Value.ToString("dd/MM/yyyy hh:mm tt"):"" ) +" to "+ (eventdetails.EventEndDate.HasValue ? eventdetails.EventEndDate.Value.ToString("dd/MM/yyyy hh:mm tt") : "") + "</td></tr>";
                                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>Location</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + eventdetails.EventLocation + "</td></tr>";
                                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>Event Delegate Id</td><td style='border:1px solid #eee;font-family:Segoe UI'>" + EventDelegateId + "</td></tr>";
                                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>Registed By </td><td style='border:1px solid #eee;font-family:Segoe UI'>" + User.Identity.Name + "</td></tr>";
                                mealTable += "<tr><td style='border:1px solid #eee;font-family:Segoe UI'>QR Code</td><td style='border:1px solid #eee;font-family:Segoe UI'><img src='cid:MyImage' style='width:150px;' alt='" + EventDelegateId + "' /></td></tr>";
                                mealTable += "</table>";
                                string Subject = "";
                                string body = "Dear " + model.Delegate + " , <br/> Please find below are the event " + eventdetails.Name + " details. ";
                                body += mealTable;
                                Subject = "Registration Successfull for the event " + eventdetails.Name + "";
                                CustomModel cm = new CustomModel();
                                MailModel mailmodel = new MailModel
                                {
                                    fromemail = "info@fernandez.foundation",
                                    toemail = model.EmailID,
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
                        }
                    }
                    //entities.SaveChanges();
                }
            }
            return RedirectToAction("ViewDelegates", new
            {
                id = hfileEventId
            });
        }
       

        public static string FixBase64ForImage(string Image)
        {
            System.Text.StringBuilder sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", string.Empty); sbText.Replace(" ", string.Empty);
            return sbText.ToString();
        }
        public ActionResult EventWelcomeKit(int id)
        {
            ViewBag.EventId = id;
            return View();
        }
        public ActionResult EventMeals(int id)
        {
            ViewBag.EventId = id;
            return View();
        }
    }
}