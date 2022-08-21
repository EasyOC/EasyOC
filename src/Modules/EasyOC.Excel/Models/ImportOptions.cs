using System.Collections.Generic;

namespace EasyOC.Excel.Models
{
    public class ImportOptions
    {
        public string TargetContentType { get; set; }
        public List<ImportColMapping> ColMappings { get; set; }
    }

    public class ImportColMapping
    {
        public string ColumnName { get; set; }
        public string FieldName { get; set; }
    }

}
