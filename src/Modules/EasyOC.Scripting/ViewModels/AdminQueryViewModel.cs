using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EasyOC.Scripting.ViewModels
{
    public class AdminQueryViewModel
    {
        public string DecodedQuery { get; set; }
        public bool ReturnDocuments { get; set; }
        public string Parameters { get; set; } = "{}";
        [BindNever]
        public double? Elapsed { get; set; }
        [BindNever]
        public object Result { get; set; }
    }
}