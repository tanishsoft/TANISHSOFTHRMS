using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    public class CMSController : ApiController
    {

        MyIntranetAppEntities myapp = new MyIntranetAppEntities();

        [HttpPost]
        [Route("SaveTransaction")]
        public string SaveTransaction(TransactionViewModel model)
        {
            tbl_cm_Transaction dbModel = new tbl_cm_Transaction();
            dbModel.BillAddress = model.BillAddress;
            dbModel.CanteenId = model.CanteenId;
            dbModel.CardNumber = model.CardNumber;
            dbModel.DiscountValue = model.DiscountValue;
            dbModel.CreatedBy = model.CreatedBy;
            dbModel.CreatedOn = DateTime.Now;
            dbModel.ModifiedOn = DateTime.Now;
            dbModel.DiscountType = "Amount";
            dbModel.EmpId = model.EmpId;
            dbModel.FinalPrice = model.FinalPrice;
            dbModel.InPatientdRoomNo = model.InPatientdRoomNo;
            dbModel.IsActive = model.IsActive;
            dbModel.ModeOfPayment = model.ModeOfPayment;
            dbModel.ModifiedBy = model.CreatedBy;
            dbModel.NameOfTheCard = model.NameOfTheCard;
            dbModel.PatientId = model.PatientId;
            dbModel.RefundAmount = model.RefundAmount;
            dbModel.SalesEmpNotes = model.SalesEmpNotes;
            dbModel.SaleType = model.SaleType;
            dbModel.TaxAmount = model.TaxAmount;
            dbModel.TotalPaidAmount = model.TotalPaidAmount;
            dbModel.TotalPrice = model.TotalPrice;
            dbModel.TransactionCustomerType = model.TransactionCustomerType;
            dbModel.IsFreeMeal = false;
            if (model.EmpId != 0 && model.SaleType == "Emp")
            {
                dbModel.EmpName = (from V in myapp.tbl_User where V.UserId == model.EmpId select V.FirstName + " " + V.LastName).SingleOrDefault();
                dbModel.EmpMobile = (from V in myapp.tbl_User where V.UserId == model.EmpId select V.PhoneNumber).SingleOrDefault();
                dbModel.EmpEmail = (from V in myapp.tbl_User where V.UserId == model.EmpId select V.EmailId).SingleOrDefault();
            }
            var userId = model.CreatedBy;
            dbModel.SalesEmpId = (from V in myapp.tbl_User where V.CustomUserId == userId select V.UserId).SingleOrDefault();
            myapp.tbl_cm_Transaction.Add(dbModel);
            myapp.SaveChanges();
            if(model.TransactionItems!=null && model.TransactionItems.Count > 0)
            {
                for (int i = 0; i < model.TransactionItems.Count; i++)
                {
                    model.TransactionItems[i].IsActive = true;
                    model.TransactionItems[i].TransactionId = dbModel.TransactionId;
                }
                myapp.tbl_cm_TransactionItem.AddRange(model.TransactionItems);
                myapp.SaveChanges();
            }
            try
            {
                string mobilenumber = dbModel.EmpMobile;
                if (model.SaleType == "InPatient")
                {
                    var p = myapp.tbl_Patient.Where(l => l.MRNo == model.PatientId).FirstOrDefault();
                    if (p != null)
                    {
                        mobilenumber = p.MobileNumber;
                    }
                }
                SendSms sms = new SendSms();
                if (mobilenumber != null && mobilenumber != "" && model.ModeOfPayment != "Credit")
                {
                    string message = "Hello! Thank you for ordering from Fernz Cafe. We have received your payment of Rs " + model.TotalPaidAmount + ". Enjoy your healthy meal!";
                    sms.SendSmsToEmployee(mobilenumber, message);
                }
            }
            catch {
                return "Error";
            }
            return "Successfully Saved";
        }

        [HttpPost]
        [Route("UpdateTransaction")]
        public string UpdateTransaction(TransactionViewModel model)
        {
            tbl_cm_Transaction dbModel = myapp.tbl_cm_Transaction.Where(m => m.TransactionId == model.TransactionId).FirstOrDefault();
            dbModel.BillAddress = model.BillAddress;
            dbModel.CanteenId = model.CanteenId;
            dbModel.CardNumber = model.CardNumber;
            dbModel.DiscountValue = model.DiscountValue;          
            dbModel.ModifiedOn = DateTime.Now;
            dbModel.DiscountType = "Amount";
            dbModel.EmpId = model.EmpId;
            dbModel.FinalPrice = model.FinalPrice;
            dbModel.InPatientdRoomNo = model.InPatientdRoomNo;
            dbModel.IsActive = model.IsActive;
            dbModel.ModeOfPayment = model.ModeOfPayment;
            dbModel.ModifiedBy = model.ModifiedBy;
            dbModel.NameOfTheCard = model.NameOfTheCard;
            dbModel.PatientId = model.PatientId;
            dbModel.RefundAmount = model.RefundAmount;
            dbModel.SalesEmpNotes = model.SalesEmpNotes;
            dbModel.SaleType = model.SaleType;
            dbModel.TaxAmount = model.TaxAmount;
            dbModel.TotalPaidAmount = model.TotalPaidAmount;
            dbModel.TotalPrice = model.TotalPrice;
            dbModel.TransactionCustomerType = model.TransactionCustomerType;
            dbModel.IsFreeMeal = false;
            if (model.EmpId != 0 && model.SaleType == "Emp")
            {
                dbModel.EmpName = (from V in myapp.tbl_User where V.UserId == model.EmpId select V.FirstName + " " + V.LastName).SingleOrDefault();
                dbModel.EmpMobile = (from V in myapp.tbl_User where V.UserId == model.EmpId select V.PhoneNumber).SingleOrDefault();
                dbModel.EmpEmail = (from V in myapp.tbl_User where V.UserId == model.EmpId select V.EmailId).SingleOrDefault();
            }
            var userId = model.CreatedBy;
            dbModel.SalesEmpId = (from V in myapp.tbl_User where V.CustomUserId == userId select V.UserId).SingleOrDefault();
            myapp.tbl_cm_Transaction.Add(dbModel);
            myapp.SaveChanges();
            if (model.TransactionItems != null && model.TransactionItems.Count > 0)
            {
                var transactionId = model.TransactionItems[0].TransactionId;
                var items = myapp.tbl_cm_TransactionItem.Where(m => m.TransactionId == transactionId).ToList();
                myapp.tbl_cm_TransactionItem.RemoveRange(items);
                myapp.SaveChanges();
                for (int i = 0; i < model.TransactionItems.Count; i++)
                {
                    model.TransactionItems[i].IsActive = true;
                }
                myapp.tbl_cm_TransactionItem.AddRange(model.TransactionItems);
                myapp.SaveChanges();
            }
            try
            {
                string mobilenumber = dbModel.EmpMobile;
                if (model.SaleType == "InPatient")
                {
                    var p = myapp.tbl_Patient.Where(l => l.MRNo == model.PatientId).FirstOrDefault();
                    if (p != null)
                    {
                        mobilenumber = p.MobileNumber;
                    }
                }
                SendSms sms = new SendSms();
                if (mobilenumber != null && mobilenumber != "" && model.ModeOfPayment != "Credit")
                {
                    string message = "Hello! Thank you for ordering from Fernz Cafe. We have received your payment of Rs " + model.TotalPaidAmount + ". Enjoy your healthy meal!";
                    sms.SendSmsToEmployee(mobilenumber, message);
                }
            }
            catch
            {
                return "Error";
            }
            return "Successfully Saved";
        }

        [HttpGet]
        [Route("GetStore")]
        [AllowAnonymous]
        public List<tbl_cm_Store> GetStore()
        {
            var query = myapp.tbl_cm_Store.ToList();
            return query;
        }
        [HttpGet]
        [Route("GetItemSearch")]
        [AllowAnonymous]
        public List<tempTable> GetItemSearch(string searchTerm)
        {
            var query = myapp.tbl_cm_Item.Where(l => l.IsActive == true).ToList();
            query = query
                    .Where(c => c.ItemId.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                                 c.ItemName != null && c.ItemName.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                               c.ItemCode != null && c.ItemCode.ToString().ToLower().Contains(searchTerm.ToLower())

                               ).ToList();
            var resulst = (from q in query
                           select new CMSItemViewModel
                           {
                               ItemId = q.ItemId,
                               ItemName = q.ItemName,
                               AvailableStock = "0"
                           }).ToList();
            var finalresult = (from r in resulst
                               select new tempTable
                               {
                                   id= r.ItemId,
                                  value= r.ItemName
                               }).ToList();
            return finalresult;
        }
        [HttpGet]
        [Route("GetInPatientdSearch")]
        [AllowAnonymous]
        public List<tempTable> GetInPatientdSearch(string searchTerm)
        {
            var query = myapp.tbl_Patient.ToList();
            query = query
                    .Where(c => c.MRNo != null && c.MRNo.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                                 c.Name != null && c.Name.ToString().ToLower().Contains(searchTerm.ToLower())
                                ||
                               c.IPNo != null && c.IPNo.ToString().ToLower().Contains(searchTerm.ToLower())

                               ).ToList();
            var resulst = (from q in query
                           select new tempTable
                           {
                               valueid = q.MRNo,
                               value = q.MRNo + " " + q.Name
                           }).ToList();
            return resulst;
        }

    }
}