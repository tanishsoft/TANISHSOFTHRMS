using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using WebApplicationHsApp.DataModel;
using WebApplicationHsApp.Models;

namespace WebApplicationHsApp.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        #region Properties
        /// <summary>
        /// List of online users
        /// </summary>
        public readonly static List<UserViewModel> _Connections = new List<UserViewModel>();

        /// <summary>
        /// List of available chat rooms
        /// </summary>
        private readonly static List<RoomViewModel> _Rooms = new List<RoomViewModel>();

        /// <summary>
        /// Mapping SignalR connections to application users.
        /// (We don't want to share connectionId)
        /// </summary>
        private readonly static Dictionary<string, string> _ConnectionsMap = new Dictionary<string, string>();
        #endregion

        public void Send(string roomName, string message)
        {
            if (message.StartsWith("/private"))
                SendPrivate(message);
            else
                SendToRoom(roomName, message);
        }

        public void SendPrivate(string message)
        {
            // message format: /private(receiverName) Lorem ipsum...
            string[] split = message.Split(')');
            string receiver = split[0].Split('(')[1];
            string userId;
            if (_ConnectionsMap.TryGetValue(receiver, out userId))
            {
                // Who is the sender;
                var sender = _Connections.Where(u => u.CustomUserId == IdentityName).First();

                message = Regex.Replace(message, @"\/private\(.*?\)", string.Empty).Trim();

                // Build the message
                MessageViewModel messageViewModel = new MessageViewModel()
                {
                    From = sender.FirstName,
                    Avatar = sender.Avatar,
                    To = "",
                    Content = Regex.Replace(message, @"(?i)<(?!img|a|/a|/img).*?>", String.Empty),
                    Timestamp = DateTime.Now.ToLongTimeString()
                };

                // Send the message
                Clients.Client(userId).newMessage(messageViewModel);
                Clients.Caller.newMessage(messageViewModel);
            }
        }

        public void SendToRoom(string roomName, string message)
        {
            try
            {
                using (var db = new MyIntranetAppEntities())
                {
                    var user = db.tbl_User.Where(u => u.CustomUserId == IdentityName).FirstOrDefault();
                    var room = db.Rooms.Where(r => r.Name == roomName).FirstOrDefault();

                    // Create and save message in database
                    Message msg = new Message()
                    {
                        Content = Regex.Replace(message, @"(?i)<(?!img|a|/a|/img).*?>", String.Empty),
                        Timestamp = DateTime.Now,
                        FromUserId = user.UserId,
                        ToRoomId = room.Id,
                        CreatedBy = IdentityName
                    };
                    db.Messages.Add(msg);
                    db.SaveChanges();

                    // Broadcast the message
                    //var messageViewModel = Mapper.Map<Message, MessageViewModel>(msg);
                    MessageViewModel messageViewModel = new MessageViewModel
                    {
                        Avatar = "",
                        Content = msg.Content,
                        From = user.FirstName,
                        Timestamp = DateTime.Now.ToString(),
                        To = roomName,
                        ismine = "no"
                    };
                    Clients.Group(roomName).newMessage(messageViewModel);
                }
            }
            catch (Exception)
            {
                Clients.Caller.onError("Message not send!");
            }
        }

        public void Join(string roomName)
        {
            try
            {
                var user = _Connections.Where(u => u.CustomUserId == IdentityName).FirstOrDefault();
                if (user != null && user.CurrentRoom != roomName)
                {
                    // Remove user from others list
                    if (!string.IsNullOrEmpty(user.CurrentRoom))
                        Clients.OthersInGroup(user.CurrentRoom).removeUser(user);

                    // Join to new chat room
                    Leave(user.CurrentRoom);
                    Groups.Add(Context.ConnectionId, roomName);
                    user.CurrentRoom = roomName;

                    // Tell others to update their list of users
                    Clients.OthersInGroup(roomName).addUser(user);
                }
            }
            catch (Exception ex)
            {
                Clients.Caller.onError("You failed to join the chat room!" + ex.Message);
            }
        }

        private void Leave(string roomName)
        {
            Groups.Remove(Context.ConnectionId, roomName);
        }

        public void CreateRoom(string roomName)
        {
            try
            {
                int currentuserid = int.Parse(IdentityName);
                string touser = roomName.Split('_')[1];
                roomName = roomName + "_" + IdentityName;
                using (var db = new MyIntranetAppEntities())
                {

                    if (db.Rooms.Any(r => r.Name == roomName))
                    {
                        Clients.Caller.onError("Another chat room with this name exists");
                    }
                    else
                    {
                        var room = new Room()
                        {
                            Name = roomName,
                            AdminId = int.Parse(IdentityName),
                            FromUserName = IdentityName,
                            ToUserName = touser
                        };
                        db.Rooms.Add(room);
                        db.SaveChanges();
                        var userto = db.tbl_User.Where(u => u.CustomUserId == room.ToUserName).SingleOrDefault();
                        var userfrom = db.tbl_User.Where(u => u.CustomUserId == room.FromUserName).SingleOrDefault();
                        if (room != null)
                        {
                            RoomViewModel roomViewModel = new RoomViewModel
                            {
                                Id = room.Id,
                                Name = roomName,
                                FromUserName = userfrom.FirstName + " " + userfrom.DepartmentName,
                                ToUserName = userto.FirstName + " " + userto.DepartmentName
                            };
                            _Rooms.Add(roomViewModel);
                            Clients.All.addChatRoom(roomViewModel);
                        }
                    }
                }//using
            }
            catch (Exception ex)
            {
                Clients.Caller.onError("Couldn't create chat room: " + ex.Message);
            }
        }

        public void DeleteRoom(string roomName)
        {
            try
            {
                using (var db = new MyIntranetAppEntities())
                {
                    int ids = int.Parse(IdentityName);
                    // Delete from database
                    var room = db.Rooms.Where(r => r.Name == roomName && r.AdminId == ids).FirstOrDefault();
                    db.Rooms.Remove(room);
                    db.SaveChanges();

                    // Delete from list
                    var roomViewModel = _Rooms.First<RoomViewModel>(r => r.Name == roomName);
                    _Rooms.Remove(roomViewModel);

                    // Move users back to Lobby
                    Clients.Group(roomName).onRoomDeleted(string.Format("Room {0} has been deleted.\nYou are now moved to the Lobby!", roomName));

                    // Tell all users to update their room list
                    Clients.All.removeChatRoom(roomViewModel);
                }
            }
            catch (Exception)
            {
                Clients.Caller.onError("Can't delete this chat room.");
            }
        }

        public IEnumerable<MessageViewModel> GetMessageHistory(string roomName)
        {
            using (var db = new MyIntranetAppEntities())
            {
                int currentuserid = int.Parse(IdentityName);
                var users = db.tbl_User.Where(l => l.IsActive == true).ToList();
                var roomdet = db.Rooms.Where(l => l.Name == roomName).SingleOrDefault();
                var messageHistory = db.Messages.Where(m => m.ToRoomId == roomdet.Id)
                    .OrderByDescending(m => m.Timestamp)
                    .Take(150)
                    .AsEnumerable()
                    .Reverse()
                    .ToList();
                IEnumerable<MessageViewModel> model = (from m in messageHistory
                                                       select new MessageViewModel
                                                       {
                                                           Avatar = "",
                                                           Content = m.Content,
                                                           From = users.Where(l => l.UserId == m.FromUserId).SingleOrDefault().FirstName,
                                                           Timestamp = m.Timestamp.ToString(),
                                                           To = roomName,
                                                           ismine = (IdentityName != m.CreatedBy ? "yes" : "no")
                                                       }).ToList();
                return model;
            }
        }

        public IEnumerable<RoomViewModel> GetRooms()
        {
            List<RoomViewModel> listRooms = new List<RoomViewModel>();
            using (var db = new MyIntranetAppEntities())
            {
                int currentuserid = int.Parse(IdentityName);
                var roomslist = db.Rooms.Where(l => l.Name.Contains(IdentityName)).ToList();
                listRooms = (from m in roomslist
                             join u in db.tbl_User on m.FromUserName equals u.CustomUserId
                             join u1 in db.tbl_User on m.ToUserName equals u1.CustomUserId
                             select new RoomViewModel
                             {
                                 Id = m.Id,
                                 Name = m.Name,
                                 FromUserName = u.FirstName + " " + u.DepartmentName,
                                 ToUserName = u1.FirstName + " " + u1.DepartmentName
                             }).ToList();
            }

            return listRooms;

        }

        public IEnumerable<UserViewModel> GetUsers(string roomName)
        {
            using (var db = new MyIntranetAppEntities())
            {
                foreach (var v in _Connections)
                {
                    var user = db.tbl_User.Where(u => u.CustomUserId == v.CustomUserId).FirstOrDefault();
                    v.UserOnlineStatus = user.UserOnlineStatus;
                }
            }
            return _Connections.ToList();
        }

        #region OnConnected/OnDisconnected
        public override Task OnConnected()
        {
            using (var db = new MyIntranetAppEntities())
            {
                try
                {
                    var user = db.tbl_User.Where(u => u.CustomUserId == IdentityName).FirstOrDefault();

                    user.UserOnlineStatus = "Active";
                    db.SaveChanges();
                    var userViewModel = new UserViewModel
                    {
                        Avatar = "",
                        Comments = user.Comments,
                        FirstName = user.FirstName,
                        DepartmentName = user.DepartmentName,
                        Designation = user.Designation,
                        Gender = user.Gender,
                        CurrentRoom = "",
                        Extenstion = user.Extenstion,
                        LocationName = user.LocationName,
                        EmpId = user.EmpId,
                        CustomUserId = user.CustomUserId,
                        UserOnlineStatus = "Active"
                    };
                    //var userViewModel = Mapper.Map<ApplicationUser, UserViewModel>(user);
                    //userViewModel.Device = GetDevice();
                    userViewModel.CurrentRoom = "";

                    if (!_Connections.Any(u => u.CustomUserId == IdentityName))
                    {
                        _Connections.Add(userViewModel);
                        _ConnectionsMap.Add(IdentityName, Context.ConnectionId);
                    }

                    Clients.Caller.getProfileInfo(user.FirstName, "");
                }
                catch (Exception ex)
                {
                    Clients.Caller.onError("OnConnected:" + ex.Message);
                }
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            try
            {
                using (var db = new MyIntranetAppEntities())
                {
                    var user = _Connections.Where(u => u.CustomUserId == IdentityName).First();
                    _Connections.Remove(user);
                    var user1 = db.tbl_User.Where(u => u.CustomUserId == IdentityName).FirstOrDefault();
                    user1.UserOnlineStatus = "Offline";
                    db.SaveChanges();
                    // Tell other users to remove you from their list
                    Clients.OthersInGroup(user.CurrentRoom).removeUser(user);

                    // Remove mapping
                    _ConnectionsMap.Remove(user.FirstName);
                }
            }
            catch (Exception ex)
            {
                Clients.Caller.onError("OnDisconnected: " + ex.Message);
            }

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            var user = _Connections.Where(u => u.CustomUserId == IdentityName).FirstOrDefault();
            if (user != null)
                Clients.Caller.getProfileInfo(user.FirstName, "");

            return base.OnReconnected();
        }
        #endregion

        private string IdentityName
        {
            get { return Context.User.Identity.Name; }
        }

        private string GetDevice()
        {
            string device = Context.Headers.Get("Device");

            if (device != null && (device.Equals("Desktop") || device.Equals("Mobile")))
                return device;

            return "Web";
        }
    }
}