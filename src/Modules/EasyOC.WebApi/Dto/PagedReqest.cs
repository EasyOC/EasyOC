
using EasyOC.Core.Swagger.Attributes;
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
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        [SwaggerIgnore]
        public bool HasOrder
        {
            get { return !string.IsNullOrEmpty(SortOrder) && !string.IsNullOrEmpty(SortField); }
        }
        [SwaggerIgnore]
        public string GetOrderStr
        {
            get
            {
                if (HasOrder)
                {
                    if (SortOrder == "ascend")
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
    }



}
