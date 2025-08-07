using System;
using System.Web;

namespace WebApplicationHsApp.Models
{
    public static class AppUserDetails
    {
        public static void SetCookie(string key, string value)
        {
            HttpCookie myCookie = new HttpCookie(key.ToLower())
            {
                Value = value,
                Expires = DateTime.Now.AddDays(2)
            };
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }
        public static string Get(string key)
        {
            if (HttpContext.Current.Request.Cookies[key.ToLower()] != null)
            {
                return HttpContext.Current.Request.Cookies[key.ToLower()].Value.ToString();
            }
            return null;
        }
        public static void UpdateCookie(string key, string value)
        {
            if (HttpContext.Current.Request.Cookies[key.ToLower()] != null)
            {
                HttpContext.Current.Request.Cookies.Remove(key.ToLower());
            }
            HttpCookie myCookie = new HttpCookie(key.ToLower())
            {
                Value = value,
                Expires = DateTime.Now.AddDays(2)
            };
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }
    }
    public static class AppCookieKey
    {
        public static string LocationId = "_locationid";
    }
}
