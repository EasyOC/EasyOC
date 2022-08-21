using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;

namespace EasyOC.Excel.Models
{
    public class ImportExcelSettings : ContentPart
    {

        public TextField SheetName { get; set; }
        //public NumericField StartRowNumber { get; set; }
        //public TextField StartColumnName { get; set; }
        //public TextField EndColumnName { get; set; }
        //public TextField TargetContentType { get; set; }
        public TextField FieldsMappingConfig { get; set; }


    }
}



