using System;
using System.Collections.Generic;

namespace WebApplicationHsApp.Models
{
    public class MainEventModel
    {
        public int EventId { get; set; }
        public string EventTitle { get; set; }
        public string StartDateTime { get; set; }
        public string Location { get; set; }
        public Nullable<int> Duration { get; set; }
        public string Description { get; set; }
        public string OtherDetails { get; set; }
        public List<MainEventModelImages> MainEventModelImages { get; set; }
    }

    public class ListMainEventModel
    {
        public int Year { get; set; }
        public List<MainEventModel> MainEventModels { get; set; }
    }
    public class MainEventModelImages
    {
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        public bool isMainBanner { get; set; }
    }


}