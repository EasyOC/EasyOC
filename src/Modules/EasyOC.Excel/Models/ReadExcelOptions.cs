using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.Excel.Models
{
    public class ReadExcelOptions
    {
        public string FilterExpression { get; set; }
        public string SheetName { get; set; }
        public string StartAddress { get; set; } 
    }
}
