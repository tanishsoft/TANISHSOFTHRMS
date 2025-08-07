using System;

namespace WebApplicationHsApp.Models
{
    public class VideosViewedHistroyViewModel
    {
        public int AcademicsVideoId { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }

        public string DateOfViwed { get; set; }
        public int TotalNoOfViews { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }

    }

}