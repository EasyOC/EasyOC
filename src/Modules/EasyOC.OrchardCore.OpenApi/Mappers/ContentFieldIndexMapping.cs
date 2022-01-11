using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.OpenApi.Mappers
{
    public class ContentFieldIndexMapping : Profile
    { 
        public ContentFieldIndexMapping()
        {
            //CreateMap<MultiTextField, string[]>().ConvertUsing(s => s.Values);

        }
    }
}
