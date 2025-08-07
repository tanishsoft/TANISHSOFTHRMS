using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using WebApplicationHsApp.DataModel;

namespace WebApplicationHsApp.Models.MobileApi
{
    public class NotificationSendModel
    {
        MyIntranetAppEntities myapp = new MyIntranetAppEntities();
        string serverKey = "AAAAjMxbTHI:APA91bF7MOc1PWTRQ46p01D-1nHLEJnvc39cf6bUGGrkuPsK5c4gPmV6TiJbn1BfuFwie9HhYAqBK0qlQ5R93iKJi2VOj3PIVRnTJnazsjfnXMJDsQqCJLuaoGOoOCAhZ1l2qXO7XiVXuVmjnqpmux9klBN3Ov3Q5A";
        string senderId = "604723956850";
        string webrequesturl = "https://fcm.googleapis.com/fcm/send";
        public string SendNotificationToAll(string message, string fromname)
        {
            string response = "Success";
            try
            {
                string deviceId = "";
                deviceId = "//topics/all";
                WebRequest tRequest = WebRequest.Create(webrequesturl);
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                var data = new
                {
                    to = deviceId,
                    notification = new
                    {
                        body = message,
                        title = fromname,
                        sound = "enabled",
                        click_action = "FCM_PLUGIN_ACTIVITY",
                        icon = "fcm_push_icon"
                    },
                    data = new
                    {                      
                        gotopage = "NotificationsPage",
                        body = message
                    },
                    priority = "high"
                };
                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                response = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response = ex.Message;
            }
            return response;
        }

        public string SendNotificationToSome(string page,string message, string fromuserid, string fromname, List<NotificationUserModel> deviceids)
        {
            string response = "Success";

            foreach (var deviceId in deviceids)
            {
                if (deviceId.DeviceId != null && deviceId.DeviceId != "")
                {
                    try
                    {
                        tbl_NotificationLog log = new tbl_NotificationLog();
                        log.ClickAction = page;
                        log.FromUserId = fromuserid;
                        log.IsRead = false;
                        log.ToUserId = deviceId.Userid;
                        log.Message = message;

                        WebRequest tRequest = WebRequest.Create(webrequesturl);
                        tRequest.Method = "post";
                        tRequest.ContentType = "application/json";
                        var data = new
                        {
                            to = deviceId.DeviceId,
                            notification = new
                            {
                                body = message,
                                title = fromname,
                                sound = "default",
                                click_action = "FCM_PLUGIN_ACTIVITY",
                                icon = "fcm_push_icon"
                            },
                            data = new
                            {
                                gotopage = page,                            
                                body = message
                            },
                            priority = "high"
                        };
                        var serializer = new JavaScriptSerializer();
                        var json = serializer.Serialize(data);
                        Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                        tRequest.Headers.Add(string.Format("Authorization: key={0}", serverKey));
                        tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                        tRequest.ContentLength = byteArray.Length;
                        using (Stream dataStream = tRequest.GetRequestStream())
                        {
                            dataStream.Write(byteArray, 0, byteArray.Length);
                            using (WebResponse tResponse = tRequest.GetResponse())
                            {
                                using (Stream dataStreamResponse = tResponse.GetResponseStream())
                                {
                                    using (StreamReader tReader = new StreamReader(dataStreamResponse))
                                    {
                                        String sResponseFromServer = tReader.ReadToEnd();
                                        response = sResponseFromServer;
                                        log.SentResponse = response;
                                        log.SentOn = DateTime.Now;
                                        myapp.tbl_NotificationLog.Add(log);
                                        myapp.SaveChanges();
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        response = ex.Message;
                    }
                }
            }

            return response;
        }
    }
}