using Microsoft.CodeAnalysis;
using FreeSql;
using Magicodes.ExporterAndImporter.Core.Extension;
using Microsoft.AspNetCore.Razor.Language.Syntax;
using System.Linq;
using Microsoft.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis.Syntax.InternalSyntax;
using NJsonSchema;
using OrchardCore.ContentManagement.Records;
using Pipelines.Sockets.Unofficial.Arenas;
using System.Linq.Dynamic.Core;
using System.Reactive.Linq;
using EasyOC.OrchardCore.FreeSql.Queries;
using System.Data;

public static class N0a2c76acdab44176b5d25209573c23d8
{
    public static EasyOC.OrchardCore.FreeSql.Queries.FreeSqlQueryResults Neac2eab2597245ea887588165926f4c8(IFreeSql freeSql, System.Collections.Generic.IDictionary<System.String, System.Object> parameters)
    {
        var query = _freeSql.Select<ContentItemIndex>().Where(x => x.Published);
        return new FreeSqlQueryResults{TotalCount = query.Count(), Items = query.ToList()};
    }
}
