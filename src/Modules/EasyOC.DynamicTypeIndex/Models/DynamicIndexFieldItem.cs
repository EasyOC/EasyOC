using Newtonsoft.Json;
using System.Collections.Generic;

namespace EasyOC.DynamicTypeIndex.Models
{
    public class DynamicIndexFieldItem : DbFiledOption
    {
        public bool EnableCalculation { get; set; }
        public string JSExpression { get; set; }
        public ContentFieldOption ContentFieldOption { get; set; }
    }

    public class DbFiledOption
    {

        public string Name { get; set; }
        public bool IsSystem { get; set; }
        public bool Disabled { get; set; }
        public string CsTypeName { get; set; }
        /// <summary>
        /// Length less 0 means Unlimited
        /// 如果小于0 代表不限制长度
        /// </summary>
        public int Length { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsDefaultIndex { get; set; }
    }

    public class ContentFieldOption
    {
        public bool IsSelfField { get; set; }
        public string FieldName { get; set; }
        public string ValuePath { get; set; }
        public string ValueFullPath { get; set; }
        public string PartName { get; set; }
        public string FieldType { get; set; }
        public IEnumerable<string> DependsOn { get; set; } = new List<string>();
        public string DisplayName { get; set; }
    }

}
