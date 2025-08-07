using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models
{
    public class OrderRequestViewModel
    {
        public int OrderRequestId { get; set; }
        public Nullable<System.DateTime> DateOfOrder { get; set; }
        public Nullable<int> MealTypeId { get; set; }
        public string LocationId { get; set; }
        public string FloorId { get; set; }
        public Nullable<int> RequestUserId { get; set; }
        public string RequestNotes { get; set; }
        public string CurrentRole { get; set; }
        public string CurrentStatus { get; set; }
        public Nullable<bool> IsApprovedByDietTeam { get; set; }
        public string DietTeamNotes { get; set; }
        public Nullable<bool> IsApprovedByKitchenManager { get; set; }
        public string KitchenManagerNotes { get; set; }
        public string KitchenTeamNotes { get; set; }
        public string SupervisorNotes { get; set; }
        public string RequestType { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> DietitianEmpId { get; set; }
        public Nullable<System.DateTime> DietitianActionDate { get; set; }
        public Nullable<int> KitchenEmpId { get; set; }
        public Nullable<System.DateTime> KitchenActionDate { get; set; }
        public Nullable<int> SupervisorEmpId { get; set; }
        public Nullable<System.DateTime> SupervisorActionDate { get; set; }
        public Nullable<int> CafeteriaEmpId { get; set; }
        public Nullable<System.DateTime> CafeteriaActionDate { get; set; }

        public List<tbl_cmp_OrderRequestByPatient> orderRequestByPatients { get; set; }
    }
}