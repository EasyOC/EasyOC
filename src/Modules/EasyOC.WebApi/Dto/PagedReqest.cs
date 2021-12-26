using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.WebApi.Dto
{
    public class PagedReqest
    {
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 10;
        public int GetStartIndex() { return Page * PageSize; }
    }
}
