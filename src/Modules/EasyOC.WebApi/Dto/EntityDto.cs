using AutoMapper;
using Newtonsoft.Json.Linq;
using OrchardCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.WebApi
{
    [AutoMap(typeof(Entity), ReverseMap = true)]
    internal class EntityDto
    {
        public JObject Properties
        {
            get;
            set;
        } = new JObject();
    }
}
