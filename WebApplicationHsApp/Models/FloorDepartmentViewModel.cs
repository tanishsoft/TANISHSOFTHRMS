using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public class FloorDepartmentViewModel
    {
        public int Id { get; set; }
        public int FloorId { get; set; }
        public string FloorName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public bool IsActive { get; set; }
        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public int BuildingId { get; set; }
        public string BuildingName { get; set; }
    }
}