using EasyOC.Dto;

namespace EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos
{
    public class GetAllTypeFilterInput : SimpleFilterAndPageQueryInput
    {
        public string Stereotype { get; set; }
    }
}
