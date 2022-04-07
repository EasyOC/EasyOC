using EasyOC.OrchardCore.DynamicTypeIndex.Index;
using EasyOC.OrchardCore.DynamicTypeIndex.Service.Dto;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.DynamicTypeIndex
{
    public interface IDynamicIndexAppService
    {
        List<DynamicIndexFieldItem> GenerateFields(string typeName);
        DynamicIndexConfigModel GetDefaultConfig(string typeName);
        Task<DynamicIndexConfigModel> GetDynamicIndexConfigAsync(string typeName);
        Task<string> GetIndexClassString(string typeName, bool syncStruct = false);
        Task<bool> RebuildIndexData(DynamicIndexConfigModel model);
        Task<bool> RebuildIndexData(string typeName);
        Task SyncTableStructAsync(DynamicIndexConfigModel model, bool dropTable = false);
        DynamicIndexConfigModel ToConfigModel(DynamicIndexConfigDataIndex storedConfig);
        Task<DynamicIndexConfigModel> UpdateDynamicIndexAsync([FromBody] DynamicIndexConfigModel model);
    }
}