using System.ComponentModel.DataAnnotations;

namespace EasyOC.Excel.Workflow
{
    public class ExcelTaskViewModel
    {
        public string RowFilter { get; set; }
        [Display(Name = "文件位置")]
        public string FilePath { get; set; }
        [Display(Name = "变量名称")]
        public string PropertyName { get; set; }
        public string ExtraScripts { get; set; }
        public bool FromUpload { get; set; }


    }
}



