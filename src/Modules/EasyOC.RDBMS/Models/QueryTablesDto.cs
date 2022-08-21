namespace EasyOC.RDBMS.Models
{
    public class QueryTablesDto : QueryDto
    {
        public string ConnectionConfigId { get; set; }
        public bool DisableCache { get; set; }
    }
}



