using EasyOC.OrchardCore.DynamicTypeIndex.Models;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using System;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.DynamicTypeIndex.Service
{
    public interface IDynamicIndexAppService
    {
        string GetDynamicIndexTableName(string typeName);
        DynamicIndexConfigModel GetDefaultConfig(string typeName);
        Task<DynamicIndexConfigModel> GetDynamicIndexConfigAsync(string typeName, bool withCache = true);
        Task<Type> GetDynamicIndexTypeAsync(DynamicIndexEntityInfo entityInfo);
        Task<Type> GetDynamicIndexTypeAsync(string typeFullName, bool withCache = true);
        Task<DynamicIndexConfigModel> GetDynamicIndexConfigOrDefaultAsync(string typeName);
        Task<int> RebuildIndexData(DynamicIndexConfigModel model);
        Task<int> RebuildIndexData(string typeName);
        Task<Type> SyncTableStructAsync(DynamicIndexEntityInfo entityInfo);
        DynamicIndexConfigModel ToConfigModel(ContentItem storedConfig);
        Task<DynamicIndexConfigModel> UpdateDynamicIndexAsync([FromBody] DynamicIndexConfigModel model);

        Task<AssemblyCSharpBuilder> GetIndexAssemblyBuilder(bool withOutCache = false);
    }
}
