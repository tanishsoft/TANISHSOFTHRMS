using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;
using WebApplicationHsApp.Models.CommonModels;
using WebApplicationHsApp.Models.MobileApi;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    [RoutePrefix("api/Content")]
    public class ContentController : ApiController
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        [HttpGet]
        [Route("GetSettings")]
        public string GetSettings(string key)
        {
            if (key == "holidays")
            {
                return "/Images/2018_holiday_list.jpg";
            }
            else
            {
                var list = myapp.tbl_Settings.Where(t => t.SettingKey == key).ToList();
                if (list.Count > 0)
                {
                    return list[0].SettingValue;
                }
                else
                {
                    return "";
                }
            }
        }
        [HttpGet]
        [Route("GetSettingsShort")]
        public string GetSettingsShort(string key)
        {
            var list = myapp.tbl_Settings.Where(t => t.SettingKey == key).ToList();
            if (list.Count > 0)
            {
                if (list[0].SettingValue.Length <= 600)
                    return list[0].SettingValue;
                else
                    return list[0].SettingValue.Substring(0, 600);
            }
            else
            {
                return "";
            }
        }
        [HttpGet]
        [Route("GetuserFname")]
        public string GetuserFname()
        {
            string username = User.Identity.Name;
            if (User.IsInRole("OutSource"))
            {

                var list = (from l in myapp.tbl_OutSourceUser where l.CustomUserId == username select l).SingleOrDefault();
                if (list != null)
                {
                    return list.FirstName;
                }
                else
                {
                    return "Test Admin";
                }
            }
            else
            {


                var list = (from l in myapp.tbl_User where l.CustomUserId == username select l).SingleOrDefault();
                if (list != null)
                {
                    return list.FirstName;
                }
                else
                {
                    return "Test Admin";
                }
            }
        }
        [HttpGet]
        [Route("GetuserInRole")]
        public bool GetuserInRole(string role)
        {
            string username = User.Identity.Name;
            if (User.IsInRole(role))
            {

                return true;
            }
            else
            {
                return false;
            }
        }
        [HttpGet]
        [Route("SaveResponseOnMdMessageReplay")]
        public string SaveResponseOnMdMessageReplay(int ParentId, string Message)
        {
            MdMessagesViewModel modelmd = new MdMessagesViewModel();
            modelmd.SaveResponseOnMdMessageReplay(ParentId, Message, User.Identity.Name);
            return "Thank you! your message has been successfully sent.";
        }
        [HttpGet]
        [Route("SaveResponseOnMdMessage")]
        public string SaveResponseOnMdMessage(string Mobile, string Message)
        {
            MdMessagesViewModel modelmd = new MdMessagesViewModel();
            modelmd.SaveResponseOnMdMessage(Mobile, Message, User.Identity.Name);
            return "Thank you! your message has been successfully sent.";
        }
        [HttpGet]
        [Route("GetSettingsResponses")]
        public List<SettingsResponseViewModel> GetSettingsResponses(string Key)
        {
            MdMessagesViewModel modelmd = new MdMessagesViewModel();
            return modelmd.GetSettingsResponses(Key);
        }
        [HttpGet]
        [Route("GetSettingsResponsesUser")]
        public List<SettingsResponseViewModel> GetSettingsResponsesUser(string Key)
        {
            MdMessagesViewModel modelmd = new MdMessagesViewModel();
            return modelmd.GetSettingsResponsesUser(Key, User.Identity.Name);
        }
        [HttpGet]
        [Route("GetAllEventsInHouse")]
        public List<ProtocolViewModel> GetAllEventsInHouse()
        {
            var list = myapp.tbl_Protocol.Where(l => l.PageName == "Events" && l.Category == "In-House").ToList();

            var list2 = (from l in list
                         select new ProtocolViewModel
                         {
                             Category = l.Category,
                             FilePath = l.FilePath,
                             Id = l.Id,
                             Name = l.Name,
                             FileType = l.FilePath.Contains(".pdf") ? ".PDF" : ".jpg"
                         }).ToList();
            return list2;
        }
        [HttpGet]
        [Route("GetAllEventsMedical")]
        public List<ProtocolViewModel> GetAllEventsMedical()
        {
            var list = myapp.tbl_Protocol.Where(l => l.PageName == "Events" && l.Category == "Medical").ToList();
            var list2 = (from l in list
                         select new ProtocolViewModel
                         {
                             Category = l.Category,
                             FilePath = l.FilePath,
                             Id = l.Id,
                             Name = l.Name,
                             FileType = l.FilePath.Contains(".pdf") ? ".PDF" : ".jpg"
                         }).ToList();
            return list2;
        }
    }
}
