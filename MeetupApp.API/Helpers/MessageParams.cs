namespace MeetupApp.API.Helpers
{
    public class MessageParams
    {
        public int UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        private int maxPageSize = 50;
        private int pageSize = 5;
        public string OrderBy { get; set; }
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }
        public string MessageContainer { get; set; } = "Unread";

    }
}