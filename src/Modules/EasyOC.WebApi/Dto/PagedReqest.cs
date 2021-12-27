
using Newtonsoft.Json;

namespace EasyOC.WebApi.Dto
{
    public class PagedReqest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int GetStartIndex() { return (Page - 1) * PageSize; }
    }

    public class PagedAndSortedRequest : PagedReqest
    {
        [JsonProperty(PropertyName = "field")]
        public string SortField { get; set; }
        [JsonProperty(PropertyName = "order")]
        public QueryOrder? SortOrder { get; set; }
        public bool HasOrder()
        {
             return SortOrder.HasValue && !string.IsNullOrEmpty(SortField); 
        }
         
        public string GetOrderStr()
        {
            if (HasOrder())
            {
                if (SortOrder == QueryOrder.Ascend)
                {
                    return $"{SortField} asc";

                }
                else
                {
                    return $"{SortField} desc";
                }
            }
            else return string.Empty;
        }
    }

    public enum QueryOrder
    {
        Ascend,
        Descend,
    }

}
