using EasyOC.DynamicWebApi.Attributes;
using EasyOC.OrchardCore.RDBMS.Models;
using EasyOC.OrchardCore.RDBMS.ViewModels;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.RDBMS.Services
{
    public interface IRDBMSAppService
    {
        Task ImportDeploymentPackageAsync(ImportJsonInupt model);
        Task<string> GenerateRecipeAsync(string tableName, string connectionConfigId);
        Task<IEnumerable<ConnectionConfigModel>> GetAllDbConnecton();
        IEnumerable<OrchardCoreBaseField> GetAllOrchardCoreBaseFields();
        Task<IEnumerable<DbTableInfoDto>> GetAllTablesAsync(QueryTablesDto queryTablesDto);
        JObject GetTableInfo(RDBMSMappingConfigViewModel config);
        Task<DbTableInfoDto> GetTableDetailsAsync(string connectionConfigId, string tableName);
        [NonDynamicMethod]
        Task<IFreeSql> GetFreeSqlAsync(string connectionConfigId);
    }
}


