namespace WebApplicationHsApp.Models.MobileApi
{
    public class NotificationMobileModel
    {
        public int Id { get; set; }
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }
        public string Message { get; set; }
        public string ClickAction { get; set; }
        public string Params { get; set; }
        public string SentOn { get; set; }
        public bool IsRead { get; set; }
        public string SentResponse { get; set; }
    }
}