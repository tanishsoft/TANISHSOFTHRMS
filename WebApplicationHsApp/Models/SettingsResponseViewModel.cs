using System.Collections.Generic;

namespace WebApplicationHsApp.Models
{
    public class SettingsResponseViewModel
    {
        public int SettingsId { get; set; }
        public string SettingsValue { get; set; }
        public List<SettingsResponseAll> Comments { get; set; }
    }
    public class SettingsResponseAll
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedOn { get; set; }
        public string MobileNo { get; set; }
        public List<SettingsResponseAll> SubComments { get; set; }
    }
}