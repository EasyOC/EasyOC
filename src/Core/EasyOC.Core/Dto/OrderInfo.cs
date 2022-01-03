using EasyOC.Core.Swagger.Attributes;

namespace EasyOC
{
    public class OrderInfo
    {
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        //[SwaggerIgnore]
        //public bool HasOrder
        //{
        //    get { return !string.IsNullOrEmpty(SortOrder) && !string.IsNullOrEmpty(SortField); }
        //}
        //[SwaggerIgnore]
        //public string OrderStr
        //{
        //    get
        //    {
        //        if (HasOrder)
        //        {
        //            if (SortOrder == "ascend")
        //            {
        //                return $"{SortField} asc";

        //            }
        //            else
        //            {
        //                return $"{SortField} desc";
        //            }
        //        }
        //        else return string.Empty;
        //    }
        //}
    }
}
