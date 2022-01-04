using System.Collections.Generic;
using System.Linq;
using YesSql;

namespace EasyOC
{
    public static class DtoExtentions
    {

        public static bool HasOrder(this ISortInfo orderInfo)
        {
            return !string.IsNullOrEmpty(orderInfo.SortOrder) && !string.IsNullOrEmpty(orderInfo.SortField);
        }
        public static string GetOrderStr(this ISortInfo orderInfo)
        {
            if (orderInfo.HasOrder())
            {
                if (orderInfo.SortOrder == "ascend")
                {
                    return $"{orderInfo.SortField} asc";

                }
                else
                {
                    return $"{orderInfo.SortField} desc";
                }
            }
            else return string.Empty;
        } 
        public static ListType GetPageList<ListType, T>(this PageReqest input, ListType list, out int total)
              where ListType : IEnumerable<T>

        {
            total = list.Count();
            return (ListType)list.Skip(input.GetStartIndex()).Take(input.PageSize);
        }

        public static IEnumerable<T> GetPagedResult<T>(this IEnumerable<T> list, PageReqest input, out int total)
        {
            return input.GetPageList<IEnumerable<T>, T>(list, out total);
        }

        public static IEnumerable<T> Page<T>(this IEnumerable<T> source, PageReqest input, out int total)
        {
            return input.GetPageList<IEnumerable<T>, T>(source, out total);
        }
        public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> source, PageReqest input)
        {
            var result = input.GetPageList<IEnumerable<T>, T>(source, out var total);
            return result.ToPagedResult(total);
        }

        public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> source, int total)
        {
            return new PagedResult<T>(total, source);
        }


    }

}
