using System;

namespace EasyOC.DynamicWebApi.Attributes
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method)]
    public class IgnoreWebApiMethodAttribute : Attribute
    {

    }
}
