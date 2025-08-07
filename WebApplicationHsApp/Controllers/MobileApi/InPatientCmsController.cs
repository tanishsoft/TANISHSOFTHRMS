using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    public class InPatientCmsController : ApiController
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        [HttpGet]
        [Route("GetCMSPDiet")]
        [AllowAnonymous]
        public List<tbl_cmp_Diet> GetCMSPDiet()
        {
            var query = myapp.tbl_cmp_Diet.Where(m=>m.IsActive==true).ToList();
            return query;
        }
        [HttpPost]
        [Route("SaveDiet")]
        [AllowAnonymous]
        public string SaveDiet(tbl_cmp_Diet model)
        {
            model.CreatedOn = DateTime.Now;
            myapp.tbl_cmp_Diet.Add(model);
            myapp.SaveChanges();
            return "Success";
        }
        [HttpPost]
        [Route("UpdateDiet")]
        [AllowAnonymous]
        public string UpdateDiet(tbl_cmp_Diet model)
        {
            tbl_cmp_Diet _diet = new tbl_cmp_Diet();
            if (model.DietId != 0)
            {
                _diet = myapp.tbl_cmp_Diet.Where(m => m.DietId == model.DietId).FirstOrDefault();
                _diet.ModifiedBy = model.ModifiedBy;
                _diet.ModifiedOn = DateTime.Now;
            }
            _diet.DietName = model.DietName;
            _diet.DietType = model.DietType;
            _diet.IsActive = model.IsActive;
            myapp.SaveChanges();
            return "Success";
        }
        [HttpGet]
        [Route("DeleteDiet")]
        [AllowAnonymous]
        public string DeleteDiet(int id)
        {
            var query = myapp.tbl_cmp_Diet.Where(m => m.DietId == id).ToList();
            if (query.Count > 0)
            {
                myapp.tbl_cmp_Diet.Remove(query[0]);
                myapp.SaveChanges();
            }
            return "Success";
        }
        [HttpGet]
        [Route("GetCMSPLocation")]
        [AllowAnonymous]
        public List<tempTable> GetCMSP_Location()
        {
            var query = myapp.tbl_Patient.Where(m => m.Bedallocated == true).GroupBy(n => new { n.LocationId, n.LocationName }).Select(g => new { g.Key.LocationId, g.Key.LocationName }).ToList();
            var result= (from q in query
                         select new tempTable
                         {
                             valueid = q.LocationId,
                             value = q.LocationName
                         }).ToList();
            return result;
        }
        [HttpGet]
        [Route("GetCMSPFloorByLocation")]
        [AllowAnonymous]
        public List<tempTable> GetCMSP_FloorByLocation(string id)
        {
            var query = myapp.tbl_Patient.Where(m => m.Bedallocated == true && m.LocationId == id).GroupBy(n => new { n.FloorId, n.FloorNo }).Select(g => new { g.Key.FloorId, g.Key.FloorNo }).ToList();
            var result = (from q in query
                          select new tempTable
                          {
                              valueid = q.FloorId,
                              value = q.FloorNo
                          }).ToList();
            return result;
        }
        [HttpGet]
        [Route("GetCMSPLocationLiqud")]
        [AllowAnonymous]
        public List<tempTable> GetCMSP_LocationLiqud()
        {
            var query = myapp.tbl_Patient.Where(m => m.Bedallocated == true && m.Remarks != "Normal").GroupBy(n => new { n.LocationId, n.LocationName }).Select(g => new { g.Key.LocationId, g.Key.LocationName }).ToList();
            var result = (from q in query
                          select new tempTable
                          {
                              valueid = q.LocationId,
                              value = q.LocationName
                          }).ToList();
            return result;
        }
        [HttpGet]
        [Route("GetCMSPFloorByLocationLiqud")]
        [AllowAnonymous]
        public List<tempTable> GetCMSP_FloorByLocationLiqud(string id)
        {
            var query = myapp.tbl_Patient.Where(m => m.Bedallocated == true && m.Remarks != "Normal" && m.LocationId == id).GroupBy(n => new { n.FloorId, n.FloorNo }).Select(g => new { g.Key.FloorId, g.Key.FloorNo }).ToList();
            var result = (from q in query
                          select new tempTable
                          {
                              valueid = q.FloorId,
                              value = q.FloorNo
                          }).ToList();
            return result;
        }
        [HttpGet]
        [Route("GetCMSPOrderPlacebyId")]
        [AllowAnonymous]
        public tbl_cmp_OrderRequest GetCMSP_OrderPlacebyId(int id)
        {
            var query = myapp.tbl_cmp_OrderRequest.Where(m => m.OrderRequestId == id).SingleOrDefault();
            return query;
        }
        [HttpPost]
        [Route("SaveCMSPOrderRequest")]
        public string SaveCMSPOrderRequest(OrderRequestViewModel model)
        {
            tbl_cmp_OrderRequest dbModel = new tbl_cmp_OrderRequest();
            var id = model.CreatedBy;
            var time = DateTime.Now.TimeOfDay;
            if (time > new TimeSpan(15, 30, 00) && time < new TimeSpan(16, 00, 00))
            {
                dbModel.DateOfOrder = DateTime.Now.AddDays(1);
            }
            else
            {
                dbModel.DateOfOrder = DateTime.Now;
            }
            dbModel.RequestUserId = (from V in myapp.tbl_User where V.CustomUserId == id select V.UserId).SingleOrDefault();
            dbModel.CreatedBy = model.CreatedBy;
            dbModel.CreatedOn = DateTime.Now;
            if (DateTime.Now.Hour == 8 || DateTime.Now.Hour == 11 || DateTime.Now.Hour == 13 || DateTime.Now.Hour == 15)
            {
                dbModel.RequestType = "To Kitchen";
            }
            else
            {
                dbModel.RequestType = "To Cafeteria";
            }

            if (model.RequestType == "To Kitchen")
                dbModel.CurrentStatus = InPatientWorkflow.InDietitian.ToString();
            else
                dbModel.CurrentStatus = InPatientWorkflow.InCafeteria.ToString();
            dbModel.MealTypeId = model.MealTypeId;
            dbModel.LocationId = model.LocationId;
            dbModel.FloorId = model.FloorId;
            dbModel.RequestType = model.RequestType;
            dbModel.RequestNotes = model.RequestNotes;
            myapp.tbl_cmp_OrderRequest.Add(dbModel);
            myapp.SaveChanges();
            if (model.orderRequestByPatients != null && model.orderRequestByPatients.Count > 0)
            {
                for (int i = 0; i < model.orderRequestByPatients.Count; i++)
                {
                    model.orderRequestByPatients[i].CreatedOn = DateTime.Now;
                    model.orderRequestByPatients[i].IsDelivered = false;
                }

                myapp.tbl_cmp_OrderRequestByPatient.AddRange(model.orderRequestByPatients);
                myapp.SaveChanges();
            }
            return "Successfully Saved";
        }


        [HttpPost]
        [Route("UpdateCMSPOrderRequest")]
        public string UpdateCMSPOrderRequest(OrderRequestViewModel model)
        {
            tbl_cmp_OrderRequest dbModel = myapp.tbl_cmp_OrderRequest.Where(m => m.OrderRequestId == model.OrderRequestId).FirstOrDefault();
            switch (model.ModifiedBy)
            {
                case "InDietitian":
                    dbModel.DietitianEmpId = int.Parse(model.ModifiedBy);
                    dbModel.DietitianActionDate = DateTime.Now;
                    break;
                case "InKitchen":
                    dbModel.KitchenEmpId = int.Parse(model.ModifiedBy);
                    dbModel.KitchenActionDate = DateTime.Now;
                    break;
                case "InCafeteria":
                    dbModel.CafeteriaEmpId = int.Parse(model.ModifiedBy);
                    dbModel.CafeteriaActionDate = DateTime.Now;
                    break;
                case "InSupervisor":
                    dbModel.SupervisorEmpId = int.Parse(model.ModifiedBy);
                    dbModel.SupervisorActionDate = DateTime.Now;
                    break;
            }
            var id = User.Identity.Name;
            dbModel.RequestUserId = (from V in myapp.tbl_User where V.CustomUserId == id select V.UserId).SingleOrDefault();
            dbModel.ModifiedBy = model.ModifiedBy;
            dbModel.ModifiedOn = DateTime.Now;
            dbModel.MealTypeId = model.MealTypeId;
            dbModel.RequestNotes = model.RequestNotes;
            dbModel.CurrentStatus = model.CurrentStatus;
            myapp.SaveChanges();
            if (model.orderRequestByPatients != null && model.orderRequestByPatients.Count > 0)
            {
                for (int i = 0; i < model.orderRequestByPatients.Count; i++)
                {
                    var ids = model.orderRequestByPatients[i].PatientDietId;
                    var dbModel1 = myapp.tbl_cmp_OrderRequestByPatient.Where(m => m.PatientDietId == ids).SingleOrDefault();
                    dbModel1.ModifiedBy = model.ModifiedBy;
                    dbModel1.ModifiedOn = DateTime.Now;
                    dbModel1.MealtypeId = model.orderRequestByPatients[i].MealtypeId;
                    dbModel1.DietId = model.orderRequestByPatients[i].DietId;
                    dbModel1.Notes = model.orderRequestByPatients[i].Notes;
                    dbModel1.DeliverDietId = model.orderRequestByPatients[i].DeliverDietId;
                    dbModel1.DeliverRoomNo = model.orderRequestByPatients[i].DeliverRoomNo;
                    dbModel1.ReasonToNotDeliver = model.orderRequestByPatients[i].ReasonToNotDeliver;
                    dbModel1.IsDelivered = model.orderRequestByPatients[i].IsDelivered;
                    dbModel1.OrderRequestId = model.orderRequestByPatients[i].OrderRequestId;
                    myapp.SaveChanges();
                }
            }
            return "Successfully Saved";
        }
    }
}