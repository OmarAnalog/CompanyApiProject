namespace Shared.RequestFeatures
{
    public abstract class RequestParameters
    {
        const int maxPageSize = 50;
        private int pageSize = 10;
        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = Math.Min(value,maxPageSize);
            }
        }
        public int PageNumber { get; set; } = 1;
        public string? OrderBy { get; set; }
        public string? Fields { get; set; }

    }
}
