using EasyOC.OrchardCore.DynamicTypeIndex.Index;
using EasyOC.OrchardCore.DynamicTypeIndex.Models;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.DynamicTypeIndex
{
    public interface IDynamicIndexAppService
    {
        DynamicIndexConfigModel GetDefaultConfig(string typeName);
        Task<DynamicIndexConfigModel> GetDynamicIndexConfigAsync(string typeName);
        Task<DynamicIndexConfigModel> GetDynamicIndexConfigOrDefaultAsync(string typeName);
        Task<int> RebuildIndexData(DynamicIndexConfigModel model);
        Task<int> RebuildIndexData(string typeName);
        Task<Type> SyncTableStructAsync(DynamicIndexEntityInfo entityInfo);
        DynamicIndexConfigModel ToConfigModel(ContentItem storedConfig);
        Task<DynamicIndexConfigModel> UpdateDynamicIndexAsync([FromBody] DynamicIndexConfigModel model);
    }
}
