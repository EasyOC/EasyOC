namespace EasyOC.FreeSql.Queries
{
    public class SQLCounter
    {
        // [System.Diagnostics.DebuggerDisplay("Where = {Where}, From = {From}, OrderBy = {OrderBy}")]
        // public class QueryString : ICloneable
        // {
        //     public string Where { get; set; }
        //     public string From { get; set; }
        //     public string OrderBy { get; set; }
        //
        //     public QueryString Clone()
        //     {
        //         return new QueryString
        //         {
        //             Where = this.Where,
        //             From = this.From,
        //             OrderBy = this.OrderBy,
        //         };
        //     }
        //
        //     object ICloneable.Clone()
        //     {
        //         return this.Clone();
        //     }
        // }

        //         public static NHibernate.IQuery CreateCountQuery(QueryInfo qbr, NHibernate.ISession session)
        // {
        //     var qs = qbr.QueryString;
        //     var wc = qbr.Wc;
        //     var queryString = "select count(*) " + qs.From + qs.Where;
        //     Output(qbr, queryString);
        //     var query = session.CreateQuery(queryString);
        //     query.SetFirstResult(0).SetMaxResults(1);
        //     UT.SetParams(wc, query);
        //     return query;
        // }
        //
        // public static NHibernate.IQuery CreateSumQuery(QueryInfo qbr, NHibernate.ISession session, string expr)
        // {
        //     var qs = qbr.QueryString;
        //     var wc = qbr.Wc;
        //     var queryString = "select sum(" + expr + ") " + qs.From + qs.Where + " " + qs.OrderBy;
        //     Output(qbr, queryString);
        //     var query = session.CreateQuery(queryString);
        //     //query.SetFirstResult(0).SetMaxResults(1);
        //     UT.SetParams(wc, query);
        //     return query;
        // }
        //
        // public static NHibernate.IQuery CreateAverageQuery(QueryInfo qbr, ISession session, string expr)
        // {
        //     var qs = qbr.QueryString;
        //     var wc = qbr.Wc;
        //     var queryString = "select avg(" + expr + ") " + qs.From + qs.Where + " " + qs.OrderBy;
        //     Output(qbr, queryString);
        //     var query = session.CreateQuery(queryString);
        //     //query.SetFirstResult(0).SetMaxResults(1);
        //     UT.SetParams(wc, query);
        //     return query;
        // }
        //
        //
        // public static NHibernate.IQuery CreateExistsQuery(QueryInfo qbr, NHibernate.ISession session)
        // {
        //     var qs = qbr.QueryString;
        //     var wc = qbr.Wc;
        //     var queryString = "select top 1 1 " + qs.From + qs.Where + " " + qs.OrderBy;
        //     Output(qbr, queryString);
        //     var query = session.CreateQuery(queryString);
        //     //query.SetFirstResult(0).SetMaxResults(1);
        //     UT.SetParams(wc, query);
        //     return query;
        // }
        //
        //
        // public static NHibernate.IQuery CreateDataQuery(QueryInfo qbr, NHibernate.ISession session)
        // {
        //     var qs = qbr.QueryString;
        //     var wc = qbr.Wc;
        //     var queryString = qs.From + qs.Where + " " + qs.OrderBy;
        //     Output(qbr, queryString);
        //     var query = session.CreateQuery(queryString);
        //     UT.SetParams(wc, query);
        //     var pagination = qbr.Pagination;
        //     SetPagination(pagination, query);
        //     return query;
        // }
        //
        // private static void Output(QueryInfo qbr, string queryString)
        // {
        //     var parameters = qbr.Parameters;
        //     if (parameters == null)
        //     {
        //     }
        //     var paramsString = new string[parameters.Count];
        //     ;
        //     for (int i = 0; i < parameters.Count; i++)
        //     {
        //         var prefix = "\r\n[" + i.ToStringInvariant() + "]=";
        //         try
        //         {
        //             var p = parameters[i];
        //             paramsString[i] = prefix + (p == null ? "null" : p.ToString());
        //         }
        //         catch (Exception ex)
        //         {
        //             paramsString[i] = "(Error to call parameter[" + i + "].ToString() , " + ex.Message + ")";
        //         }
        //     }
        //     var result = string.Join(",", paramsString);
        //     System.Diagnostics.Trace.WriteLine("[Rhythm] - DataQuery HQL:\r\n queryString: " + queryString +
        //                                        "\r\n parameters:" + result);
        // }
        //
        // static void SetPagination(IPagination pagination, NHibernate.IQuery query)
        // {
        //     if (pagination != null)
        //     {
        //         if (pagination.SkipCount > -1)
        //         {
        //             query.SetFirstResult(pagination.SkipCount);
        //         }
        //         if (pagination.TakeCount > 0)
        //         {
        //             query.SetMaxResults(pagination.TakeCount);
        //         }
        //     }
        // }

    }
}
