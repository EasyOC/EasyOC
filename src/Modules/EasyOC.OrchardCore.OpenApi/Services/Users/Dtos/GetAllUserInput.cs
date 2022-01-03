namespace EasyOC.OrchardCore.OpenApi.Dto
{
    public class GetAllUserInput : SimpleFilterAndPageQueryInput
    {

        public string SelectedRole { get; set; }

        public OrderInfo OrderInfo { get; set; }

    }
}
