using AutoMapper;
using EasyOC.OrchardCore.OpenApi.Model;

namespace EasyOC.OrchardCore.OpenApi.Mappers
{
    public class RoleClaimMapping : Profile
    {
        public RoleClaimMapping()
        {
            CreateMap<RoleClaimType, string>().ConvertUsing(s => s.ToString());

        }
    }
}
