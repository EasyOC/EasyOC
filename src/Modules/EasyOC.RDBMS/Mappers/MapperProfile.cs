using AutoMapper;
using EasyOC.RDBMS.Models;
using FreeSql.DatabaseModel;

namespace EasyOC.RDBMS.Mappers
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            #region   maping
            CreateMap<DbTableInfoDto, DbTableInfo>().ReverseMap();
            CreateMap<DbTableType, string>().ConvertUsing(type => type.ToString());
            CreateMap<DbIndexColumnInfo, DbIndexColumnInfoDto>().ReverseMap();
            CreateMap<DbIndexInfo, DbIndexInfoDto>().ReverseMap();
            CreateMap<DbForeignInfo, DbForeignInfoDto>().ReverseMap();


            #endregion

        }
    }
}



