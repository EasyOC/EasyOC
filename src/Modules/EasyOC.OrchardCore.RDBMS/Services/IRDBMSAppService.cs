using EasyOC.OrchardCore.RDBMS.Models;
using EasyOC.OrchardCore.RDBMS.Services.Dto;
using EasyOC.OrchardCore.RDBMS.ViewModels;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace EasyOC.OrchardCore.RDBMS.Services;

public interface IRDBMSAppService
{
    Task<ContentItem> GetConnectionConfigAsync(string connectionConfigId);
    Task<IFreeSql> GetFreeSqlAsync(string connectionConfigId);
    /// <summary>
    /// Get all Connection Config
    /// </summary>
    /// <returns></returns>
    Task<IEnumerable<ConnectionConfigModel>> GetAllDbConnecton();
    /// <summary>
    /// 根据连接设置获取指定数据库的表信息
    /// </summary>
    /// <param name="queryTablesDto"></param>
    /// <returns></returns>
    Task<IEnumerable<DbTableInfoDto>> GetAllTablesAsync(QueryTablesDto queryTablesDto);
    /// <summary>
    ///
    /// </summary>
    /// <param name="connectionConfigId"></param>
    /// <param name="tableName"> 表名，如：dbo.table1 </param>
    /// <returns></returns>
    Task<DbTableInfoDto> GetTableDetailsAsync(string connectionConfigId, string tableName);
    Task<GenerateRecipeDto> GenerateRecipeAsync(string connectionConfigId, string tableName);
    IEnumerable<OrchardCoreBaseField> GetAllOrchardCoreBaseFields();
    /// <summary>
    /// 使用JSON更新类型
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    Task ImportDeploymentPackageAsync(ImportJsonInupt model);
    JObject GetTableInfo(RDBMSMappingConfigViewModel config);
}
