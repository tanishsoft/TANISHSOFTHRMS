using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class InpatientDashBoardViewModel
    {
        public int BreakFastOrderedtoKitchen{ get; set; }
        public int BreakFastOrderedtoCafeteria { get; set; }
        public int BreakFastTotalDelivered { get; set; }
        public int BreakFastNotDelivered { get; set; }

        public int LunchOrderedtoKitchen { get; set; }
        public int LunchOrderedtoCafeteria { get; set; }
        public int LunchTotalDelivered { get; set; }
        public int LunchNotDelivered { get; set; }

        public int DinnerOrderedtoKitchen { get; set; }
        public int DinnerOrderedtoCafeteria { get; set; }
        public int DinnerTotalDelivered { get; set; }
        public int DinnerNotDelivered { get; set; }
    }

    public class DashboardTodayViewModel
    {
        public int? OrderId { get; set; }
        public int? PatientId { get; set; }
        public int? MealTypeId { get; set; }
        public string Location { get; set; }
        public string Floor { get; set; }
        public bool? IsDellvered { get; set; }
        public string AssignRoomNo { get; set; }
        public string MrNo { get; set; }
        public string RoomNo { get; set; }
        public string BedNo { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string DietName { get; set; }
        public string RequestType { get; set; }
        public string Notes { get; set; }
        public string User { get; set; }
        public string AttendantMeal { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? DietCreatedOn { get; set; }
    }

}