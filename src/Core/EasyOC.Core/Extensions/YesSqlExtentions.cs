using YesSql;

namespace EasyOC
{
    public static class YesSqlExtentions
    {
        public static IQuery<T> Page<T>(this IQuery<T> source, PageReqest input)
            where T : class
        {
            return source.Skip(input.GetStartIndex()).Take(input.PageSize);
        }
    }
}
