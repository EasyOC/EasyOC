using EasyOC.Content;
using EasyOC.ContentExtensions.AppServices.Dtos;
using EasyOC.ContentExtensions.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.ContentExtensions.AppServices
{
    public interface IContentTypeManagementAppService
    {
        PagedResult<ContentPartDefinitionDto> GetAllParts(SimpleFilterAndPageQueryInput input);
        PagedResult<ContentTypeListItemDto> GetAllTypes(GetAllTypeFilterInput input);
        ContentPartDefinitionDto GetPartDefinition(string name, bool withSettings = false);
        ContentTypeDefinitionDto GetTypeDefinition(string name, bool withSettings = false);
        List<ContentFieldsMappingDto> GetFields(string typeName);
        Task<IEnumerable<QueryDefDto>> ListAllQueriesAsync();
        
        /// <summary>
        /// 使用JSON更新类型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        Task ImportDeploymentPackageAsync(ImportJsonInupt model);
    }
}
