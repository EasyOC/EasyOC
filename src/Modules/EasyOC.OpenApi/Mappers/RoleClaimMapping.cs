using AutoMapper;
using EasyOC.OpenApi.Model;

namespace EasyOC.OpenApi.Mappers
{
    public class RoleClaimMapping : Profile
    {
        public RoleClaimMapping()
        {
            CreateMap<RoleClaimType, string>().ConvertUsing(s => s.ToString());

        }
    }
}
