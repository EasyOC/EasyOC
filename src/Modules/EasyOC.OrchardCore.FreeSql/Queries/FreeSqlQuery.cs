using OrchardCore.Queries;

namespace EasyOC.OrchardCore.FreeSql.Queries;

public class FreeSqlQuery : Query
{
    public FreeSqlQuery() : base("FreeSql")
    {
    }

    public bool EnablePaging { get; set; }
    public string Template { get; set; }
    public bool ReturnDocuments { get; set; }
}
