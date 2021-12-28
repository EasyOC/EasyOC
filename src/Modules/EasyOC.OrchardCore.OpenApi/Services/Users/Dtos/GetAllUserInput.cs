namespace EasyOC.OrchardCore.OpenApi.Dto
{
    public class GetAllUserInput : PagedAndSortedRequest
    {
        public string SearchText { get; set; }

        public string SelectedRole { get; set; }

    }
}
