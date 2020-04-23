namespace MeetupApp.API.Helpers
{
    public class UserParams
    {
        public int UserId { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;
        public int PageNumber { get; set; } = 1;
        private int maxPageSize = 50;
        private int pageSize = 5;
        public string OrderBy { get; set; }
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > maxPageSize) ? maxPageSize : value; }
        }
        public bool Likers { get; set; }
        public bool Likees { get; set; }


    }
}