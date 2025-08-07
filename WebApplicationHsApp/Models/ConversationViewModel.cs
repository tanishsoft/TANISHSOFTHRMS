using System;

namespace WebApplicationHsApp.Models
{
    public class ConversationViewModel
    {
        public string ID { get; set; }
        public string LastActive { get; set; }
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
        public string UserImage { get; set; }
        public string LoginTime { get; set; }
    }
  
    public class MessageViewModel
    {
        public string ConnectionId { get; set; }
        public int id { get; set; }
        public int recipientID { get; set; }
        public int senderID { get; set; }
        public string contents { get; set; }
        public string timestamp { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public string Time { get; set; }
        public string UserImage { get; set; }
    }
  
}