using AutoMapper;
using System;

namespace EasyOC.OrchardCore.RDBMS.Models
{
    [AutoMap(typeof(Type))]
    public class TypeDto
    {
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
