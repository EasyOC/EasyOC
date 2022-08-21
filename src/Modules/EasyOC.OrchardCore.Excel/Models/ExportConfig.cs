using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace EasyOC.OrchardCore.Excel.Models
{
    public class ExportByGraphqlQueryOptions
    {
        public ExportType ExportType { get; set; }
        /// <summary>
        /// 当 ExportType= ContentType 时必须指定
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 当 ExportType= ContentType 时必须指定
        /// </summary>
        public string CustomQuery { get; set; }
        /// <summary>
        /// 如果不指定则导出全部数据字段
        /// </summary>
        public List<ColumnDefinition> Columns { get; set; } = new List<ColumnDefinition>();
        public string GraphQLQuery { get; set; }
        public JObject QueryParams { get; set; }
    }
    public class ColumnDefinition
    {
        public string DisplayName { get; set; }
        public string DataKey { get; set; }
    }
    public enum ExportType
    {
        /// <summary>
        /// 基于类型
        /// </summary>
        ContentType = 1,
        /// <summary>
        /// 基于自定义查询Schema定义
        /// </summary>
        CustomQuery = 2
    }
}
