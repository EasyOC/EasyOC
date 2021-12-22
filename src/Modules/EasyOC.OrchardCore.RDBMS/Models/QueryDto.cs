namespace EasyOC.OrchardCore.RDBMS.Models
{
    public class QueryDto
    {
        public string FilterText { get; set; }
        public int MaxResultCount { get; set; } = 20;
        public int SkipCount { get; set; } = 0;
    }
}



