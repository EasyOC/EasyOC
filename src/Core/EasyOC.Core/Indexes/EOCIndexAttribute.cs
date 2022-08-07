using FreeSql.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.Core.Indexes
{
    //
    // 摘要:
    //     索引设置，如：[Index("{tablename}_idx_01", "name")]
    //
    // 参数:
    //   name:
    //     索引名
    //     v1.7.0 增加占位符 {TableName} 表名区分索引名 （解决 AsTable 分表 CodeFirst 导致索引名重复的问题）
    //
    //   fields:
    //     索引字段，为属性名以逗号分隔，如：Create_time ASC, Title ASC
    public class EOCIndexAttribute : IndexAttribute
    {
        public string Collection { get; set; } = string.Empty;
        /// <summary>
        ///  索引设置，如：[Index("{tablename}_idx_01", "name")]
        /// </summary>
        /// <param name="name"></param>
        /// <param name="fields"></param>
        public EOCIndexAttribute(string name,  params string[] fields)
            : base(name, BuildFields(fields))
        {

        }
        /// <summary>
        ///  索引设置，如：[Index("{tablename}_idx_01", "name")]
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isUnique"></param>
        /// <param name="fields"></param>
        public EOCIndexAttribute(string name, bool isUnique = false, params string[] fields)
            : base(name, BuildFields(fields), isUnique)
        {

        }

        private static string BuildFields(string[] fields)
        {
            if (fields != null)
            {
                return string.Join(",", fields);
            }
            else
            {
                return string.Empty;
            }
        }

    }
}
