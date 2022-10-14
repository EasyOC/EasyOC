using System;

namespace EasyOC.GraphQL.Models
{
    public class SchemaTypeModel
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string ArrayOf { get; set; }
        public string Description { get; set; }
    }
}