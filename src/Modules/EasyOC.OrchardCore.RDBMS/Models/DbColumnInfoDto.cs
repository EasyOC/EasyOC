using AutoMapper;
using FreeSql.DatabaseModel;
using System;

namespace EasyOC.OrchardCore.RDBMS.Models
{
    [AutoMap(typeof(DbColumnInfo), ReverseMap = true)]
    public class DbColumnInfoDto
    {
        //
        // 摘要:
        //     列名
        public string Name
        {
            get;
            set;
        }

        //
        // 摘要:
        //     映射到 C# 类型
        public TypeDto CsType
        {
            get;
            set;
        }

        //
        // 摘要:
        //     数据库枚举类型int值
        public int DbType
        {
            get;
            set;
        }

        //
        // 摘要:
        //     数据库类型，字符串，varchar
        public string DbTypeText
        {
            get;
            set;
        }

        //
        // 摘要:
        //     数据库类型，字符串，varchar(255)
        public string DbTypeTextFull
        {
            get;
            set;
        }

        //
        // 摘要:
        //     最大长度
        public int MaxLength
        {
            get;
            set;
        }

        //
        // 摘要:
        //     主键
        public bool IsPrimary
        {
            get;
            set;
        }

        //
        // 摘要:
        //     自增标识
        public bool IsIdentity
        {
            get;
            set;
        }

        //
        // 摘要:
        //     是否可DBNull
        public bool IsNullable
        {
            get;
            set;
        }

        //
        // 摘要:
        //     备注
        public string Coment
        {
            get;
            set;
        }

        //
        // 摘要:
        //     数据库默认值
        public string DefaultValue
        {
            get;
            set;
        }

        //
        // 摘要:
        //     字段位置
        public int Position
        {
            get;
            set;
        }
    }
}



