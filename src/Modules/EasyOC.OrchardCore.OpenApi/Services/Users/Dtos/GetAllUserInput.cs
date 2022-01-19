namespace EasyOC.OrchardCore.OpenApi.Dto
{
    public class GetAllUserInput : SimpleFilterAndPageQueryInput, ISortInfo
    {

        public string SelectedRole { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }

        public string DepartmentId { get; set; }
    }
}
