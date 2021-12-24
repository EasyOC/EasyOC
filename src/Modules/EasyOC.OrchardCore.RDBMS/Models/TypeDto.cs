using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.RDBMS.Models
{
    [AutoMap(typeof(Type))]
    public class TypeDto
    {
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
