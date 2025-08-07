using System;
using System.Collections.Generic;
using System.Linq;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models.Appointment
{
    public class ManagePatientBills
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        public List<PatientBillViewModel> GetAllPatientBills()
        {
            var list = (from a in myapp.tbl_PatientBill.ToList()
                        join p in myapp.tbl_Patient on a.PatientId equals p.PatientId
                        select new PatientBillViewModel
                        {
                            BillId = a.BillId,
                            CardNumber = a.CardNumber,
                            CreatedBy = a.CreatedBy,
                            CreatedOn = a.CreatedOn.HasValue ? ProjectConvert.ConverDateTimeToString(a.CreatedOn.Value) : "",
                            DiscountType = a.DiscountType,
                            DiscountValue = a.DiscountValue,
                            FinalPrice = a.FinalPrice,
                            IsActive = a.IsActive,
                            ModeOfPayment = a.ModeOfPayment,
                            ModifiedBy = a.ModifiedBy,
                            ModifiedOn = a.ModifiedOn.HasValue ? ProjectConvert.ConverDateTimeToString(a.ModifiedOn.Value) : "",
                            NameOfTheCard = a.NameOfTheCard,
                            PatientId = a.PatientId,
                            PatientName = p.Name,
                            TaxAmount = a.TaxAmount,
                            TotalPrice = a.TotalPrice
                        }).ToList();
            return list;
        }
        public PatientBillViewModel GetPatientBillById(int id)
        {
            PatientBillViewModel model = new PatientBillViewModel();
            model = (from a in myapp.tbl_PatientBill.ToList()
                     join p in myapp.tbl_Patient on a.PatientId equals p.PatientId
                     where a.BillId == id
                     select new PatientBillViewModel
                     {
                         BillId = a.BillId,
                         CardNumber = a.CardNumber,
                         CreatedBy = a.CreatedBy,
                         CreatedOn = a.CreatedOn.HasValue ? ProjectConvert.ConverDateTimeToString(a.CreatedOn.Value) : "",
                         DiscountType = a.DiscountType,
                         DiscountValue = a.DiscountValue,
                         FinalPrice = a.FinalPrice,
                         IsActive = a.IsActive,
                         ModeOfPayment = a.ModeOfPayment,
                         ModifiedBy = a.ModifiedBy,
                         ModifiedOn = a.ModifiedOn.HasValue ? ProjectConvert.ConverDateTimeToString(a.ModifiedOn.Value) : "",
                         NameOfTheCard = a.NameOfTheCard,
                         PatientId = a.PatientId,
                         PatientName = p.Name,
                         TaxAmount = a.TaxAmount,
                         TotalPrice = a.TotalPrice
                     }).SingleOrDefault();
            if (model != null)
            {
                var pbillitems = myapp.tbl_PatientBillItem.Where(l => l.BillId == id).ToList();
                model.PatientBillItems = (from p in pbillitems
                                          select new PatientBillItemViewModel
                                          {

                                              BillId = p.BillId,
                                              BillItemId = p.BillItemId,
                                              IsActive = p.IsActive,
                                              IsCredit = p.IsCredit,
                                              IsDebit = p.IsDebit,
                                              PatientId = p.PatientId,
                                              ServiceName = p.ServiceName,
                                              ServiceDescription = p.ServiceDescription,
                                              ServicePrice = p.ServicePrice
                                          }).ToList();
            }
            return model;
        }

        public List<PatientViewModel> GetAllPatients()
        {
            var list = (from p in myapp.tbl_Patient
                        select new PatientViewModel
                        {
                            Address = p.Address,
                            Age = p.Age,
                            Area = p.Area,
                            CreatedBy = p.CreatedBy,
                            Email = p.Email,
                            Gender = p.Gender,
                            MobileNumber = p.MobileNumber,
                            ModifiedBy = p.ModifiedBy,
                            Name = p.Name,
                            ParentName1 = p.ParentName1,
                            ParentName2 = p.ParentName2,
                            PatientId = p.PatientId,
                            PatientRegisterType = p.PatientRegisterType,
                            Remarks = p.Remarks
                        }).ToList();
            return list;
        }
        public string SavePatientBill(PatientBillViewModel model, string user)
        {
            string Message = "Success";
            try
            {
                tbl_PatientBill pmodel = new tbl_PatientBill();
                pmodel.CardNumber = model.CardNumber;
                pmodel.CreatedBy = user;
                pmodel.CreatedOn = DateTime.Now;
                pmodel.DiscountType = model.DiscountType;
                pmodel.DiscountValue = model.DiscountValue;
                pmodel.FinalPrice = model.FinalPrice;
                pmodel.IsActive = true;
                pmodel.ModeOfPayment = model.ModeOfPayment;
                pmodel.ModifiedBy = user;
                pmodel.ModifiedOn = DateTime.Now;
                pmodel.NameOfTheCard = model.NameOfTheCard;
                pmodel.PatientId = model.PatientId;
                pmodel.TaxAmount = model.TaxAmount;
                pmodel.TotalPrice = model.TotalPrice;
                myapp.tbl_PatientBill.Add(pmodel);
                myapp.SaveChanges();

                foreach (var item in model.PatientBillItems)
                {
                    tbl_PatientBillItem itmodel = new tbl_PatientBillItem();
                    itmodel.BillId = pmodel.BillId;
                    itmodel.CreatedBy = user;
                    itmodel.CreatedOn = DateTime.Now;
                    itmodel.IsActive = true;
                    itmodel.IsCredit = item.IsCredit;
                    itmodel.IsDebit = item.IsDebit;
                    itmodel.ModifiedBy = user;
                    itmodel.ModifiedOn = DateTime.Now;
                    itmodel.PatientId = item.PatientId;
                    itmodel.ServiceDescription = item.ServiceDescription;
                    itmodel.ServiceName = item.ServiceName;
                    itmodel.ServicePrice = item.ServicePrice;
                    myapp.tbl_PatientBillItem.Add(itmodel);
                    myapp.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
            }
            return Message;
        }

        public List<ServiceList> GetAllServiceList()
        {
            List<ServiceList> model = new List<ServiceList>();
            model.Add(new ServiceList { Name = "Consultation", Price = 500, IsCredit = true, IsDebit = false, Description = "", SubItems = null });
            model.Add(new ServiceList { Name = "Procedure", Price = 1000, IsCredit = true, IsDebit = false, Description = "", SubItems = null });
            model.Add(new ServiceList { Name = "Refund", Price = 0, IsCredit = false, IsDebit = true, Description = "", SubItems = null });

            List<ServiceSubList> modelsub = new List<ServiceSubList>();
            modelsub.Add(new ServiceSubList { Name = "ADCO-CONTROMET 5MG/5ML", Price = 26.37 });
            modelsub.Add(new ServiceSubList { Name = "CLOPAMON 5MG/5ML", Price = 27.20 });
            modelsub.Add(new ServiceSubList { Name = "MAXOLON S 5MG/5ML", Price = 28.20 });
            modelsub.Add(new ServiceSubList { Name = "KETAZOL 200MG", Price = 143.65 });
            modelsub.Add(new ServiceSubList { Name = "NIZORAL 200MG", Price = 145.46 });
            modelsub.Add(new ServiceSubList { Name = "SANDOZ KETOCONAZOLE 200MG ", Price = 147.50 });
            model.Add(new ServiceList { Name = "Vaccination", Price = 0, IsCredit = true, IsDebit = false, Description = "", SubItems = modelsub });
            return model;
        }
    }

}