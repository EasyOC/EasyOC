using System;

namespace EasyOC.DynamicWebApi.Attributes
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class NonDynamicWebApiAttribute : Attribute
    {

    }
}
