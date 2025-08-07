using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class EventMethodModel
    {
        public int EventTypeMethodId { get; set; }
        public Nullable<int> EventId { get; set; }
        public string EventDate { get; set; }
        public Nullable<bool> IsWorkshop { get; set; }
        public Nullable<bool> IsConference { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<decimal> ConferenceAmount { get; set; }
        public Nullable<decimal> WorkshopAmount { get; set; }
    }

    public partial class EventManageMealTypeViewModel
    {
        public int EventMealTypeId { get; set; }
        public int EventId { get; set; }
        public int MealTypeId { get; set; }
        public string EventDate { get; set; }
        public string Comments { get; set; }
        public bool IsActive { get; set; }

    }
}