using EasyOC.DynamicWebApi;
using EasyOC.DynamicWebApi.Attributes;
using System;
using System.Reflection;

namespace EasyOC.Core.Dynamic
{
    public class ServiceLocalSelectController : ISelectController
    {
        public bool IsController(Type type)
        {
            return type.IsPublic && type.GetCustomAttribute<DynamicWebApiAttribute>() != null;
        }
    }
}



