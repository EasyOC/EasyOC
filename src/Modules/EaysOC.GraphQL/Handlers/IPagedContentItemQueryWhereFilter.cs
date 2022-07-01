using System;
using EasyOC.OrchardCore.DynamicTypeIndex.Index;
using FreeSql;
using OrchardCore.ContentManagement.Records;

namespace EaysOC.GraphQL.Handlers;

public interface IPagedContentItemQueryWhereFilter
{
    int OrderIndex { get; set; }

    ISelect<ContentItemIndex, DIndexBase> WhereFilter(ISelect<ContentItemIndex, DIndexBase> prepareQuery,
        string contentTypeName,Action<bool> resultCallBack, IFreeSql freeSql = null);
}