using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;
using System.Linq;
using System.Net;
using Microsoft.AspNet.Identity;

namespace WebApplicationHsApp.Controllers.MobileApi
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    //[Authorize]
    [RoutePrefix("api/user")]
    public class UserAccountController : ApiController
    {
        private MyIntranetAppEntities db = new MyIntranetAppEntities();
        private ApplicationUserManager userManager;
        public UserAccountController()
        {
            this.userManager = new ApplicationUserManager(
                new UserStore<ApplicationUser>(new ApplicationDbContext()));
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return this.userManager;
            }
        }


        private IAuthenticationManager Authentication
        {
            get
            {
                return Request.GetOwinContext().Authentication;
            }
        }

        // POST api/User/Login
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<HttpResponseMessage> LoginUser(LoginViewModel model)
        {

            var list = db.tbl_User.Where(l => l.CustomUserId == model.UserId).ToList();
            if (list.Count > 0)
            {
                if (list[0].IsActive == true && list[0].IsAppLogin == true)
                {
                    var request = HttpContext.Current.Request;
                    var tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "/Token";
                    using (var client = new HttpClient())
                    {
                        var requestParams = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "password"),
                new KeyValuePair<string, string>("username", model.UserId),
                new KeyValuePair<string, string>("password", model.Password)
            };
                        var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                        var tokenServiceResponse = await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
                        var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();
                        var responseCode = tokenServiceResponse.StatusCode;
                        var responseMsg = new HttpResponseMessage(responseCode)
                        {
                            Content = new StringContent(responseString, Encoding.UTF8, "application/json")
                        };
                        return responseMsg;
                    }
                }
            }

            var responseMsg1 = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            return responseMsg1;
        }


        // POST api/User/Logout
        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            this.Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return this.Ok(new { message = "Logout successful." });
        }
      
    }
}
