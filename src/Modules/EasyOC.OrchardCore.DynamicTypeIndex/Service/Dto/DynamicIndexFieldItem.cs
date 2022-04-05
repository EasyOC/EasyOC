using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.DynamicTypeIndex.Service.Dto
{
    public class DynamicIndexFieldItem
    {
        public string Name { get; internal set; }
        public bool EnableCalculation { get; set; }
        public string JSExpression { get; set; }
        public DbFiledOption DbFieldOption { get; set; }
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
        public bool IsIdentity { get; internal set; }
        public bool IsPrimaryKey { get; internal set; }
        public bool AddToTableIndex { get; internal set; }
    }

    public class ContentFieldOption
    {
        public string FieldName { get; set; }
        public string ValuePath { get; set; }
        public string ValueFullPath { get; set; }
        public string PartName { get; set; }
        public string FieldType { get; set; }
        public IEnumerable<string> DependsOn { get; set; } = new List<string>();
    }

}
