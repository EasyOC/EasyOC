using AutoMapper;
using FreeSql.DatabaseModel;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EasyOC.RDBMS.Models
{
    [AutoMap(typeof(DbTableInfo), ReverseMap = true)]
    public class DbTableInfoDto
    {
        //
        // 摘要:
        //     唯一标识
        public string Id
        {
            get;
            set;
        }
        public int ColumnsCount { get; set; }
        //
        // 摘要:
        //     SqlServer下是Owner、PostgreSQL下是Schema、MySql下是数据库名
        public string Schema
        {
            get;
            set;
        }

        //
        // 摘要:
        //     表名
        public string Name
        {
            get;
            set;
        }

        //
        // 摘要:
        //     表备注，SqlServer下是扩展属性 MS_Description
        public string Comment
        {
            get;
            set;
        }

        //
        // 摘要:
        //     表/视图
        public string Type { get; set; }

        //
        // 摘要:
        //     列
        public List<DbColumnInfoDto> Columns
        {
            get;
            set;
        } = new List<DbColumnInfoDto>();


        //
        // 摘要:
        //     自增列
        //public List<DbColumnInfoDto> Identitys
        //{
        //    get;
        //    set;
        //} = new List<DbColumnInfoDto>();


        ////
        //// 摘要:
        ////     主键/组合
        //public List<DbColumnInfoDto> Primarys
        //{
        //    get;
        //    set;
        //} = new List<DbColumnInfoDto>();


        ////
        //// 摘要:
        ////     唯一键/组合
        //public Dictionary<string, DbIndexInfoDto> UniquesDict
        //{
        //    get;
        //    set;
        //} = new Dictionary<string, DbIndexInfoDto>();


        ////
        //// 摘要:
        ////     索引/组合
        //public Dictionary<string, DbIndexInfoDto> IndexesDict
        //{
        //    get;
        //    set;
        //} = new Dictionary<string, DbIndexInfoDto>();


        ////
        //// 摘要:
        ////     外键
        //public Dictionary<string, DbForeignInfoDto> ForeignsDict
        //{
        //    get;
        //    set;
        //} = new Dictionary<string, DbForeignInfoDto>();


        //public List<DbIndexInfoDto> Uniques => UniquesDict.Values.ToList();

        //public List<DbIndexInfoDto> Indexes => IndexesDict.Values.ToList();

        //public List<DbForeignInfoDto> Foreigns => ForeignsDict.Values.ToList();
    }
    [AutoMap(typeof(DbIndexColumnInfo), ReverseMap = true)]
    public class DbIndexColumnInfoDto
    {
        public DbColumnInfoDto Column
        {
            get;
            set;
        }

        public bool IsDesc
        {
            get;
            set;
        }
    }

    [AutoMap(typeof(DbIndexInfo), ReverseMap = true)]
    public class DbIndexInfoDto
    {
        public string Name
        {
            get;
            set;
        }

        public List<DbIndexColumnInfo> Columns
        {
            get;
        } = new List<DbIndexColumnInfo>();


        public bool IsUnique
        {
            get;
            set;
        }
    }

    [AutoMap(typeof(DbForeignInfo))]
    public class DbForeignInfoDto
    {
        [JsonIgnore]
        public DbTableInfoDto Table
        {
            get;
            set;
        }

        public List<DbColumnInfoDto> Columns
        {
            get;
            set;
        } = new List<DbColumnInfoDto>();

        [JsonIgnore]
        public DbTableInfoDto ReferencedTable
        {
            get;
            set;
        }
        [JsonIgnore]
        public List<DbColumnInfoDto> ReferencedColumns
        {
            get;
            set;
        } = new List<DbColumnInfoDto>();

    }
}



