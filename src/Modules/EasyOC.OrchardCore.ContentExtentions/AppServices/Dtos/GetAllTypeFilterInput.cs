using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyOC.Dto;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos
{
    public class GetAllTypeFilterInput: SimpleFilterAndPageQueryInput
    {
        public string Stereotype { get; set; }
    }
}
