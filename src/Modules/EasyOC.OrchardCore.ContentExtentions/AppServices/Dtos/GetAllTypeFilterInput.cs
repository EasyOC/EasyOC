namespace EasyOC.OrchardCore.ContentExtentions.AppServices.Dtos
{
    public class GetAllTypeFilterInput : SimpleFilterAndPageQueryInput
    {
        public Stereotype? Stereotype { get; set; }
    }
    public enum Stereotype
    {
        All,
        OnlyEmpty,
        Widget,
        CustomUserSettings,
        MenuItem,
    }
}
