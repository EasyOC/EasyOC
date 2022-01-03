namespace EasyOC
{
    public class PageReqest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int GetStartIndex() { return (Page - 1) * PageSize; }
    }

    public class SimpleFilterAndPageQueryInput : PageReqest
    {
        public string Filter { get; set; }

    }

    public class PageAndOrderRequest : PageReqest
    {
        public OrderInfo OrderInfo { get; set; }

    }



}
