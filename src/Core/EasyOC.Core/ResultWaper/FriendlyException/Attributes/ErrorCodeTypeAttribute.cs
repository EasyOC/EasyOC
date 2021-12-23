

using System;

namespace Furion.FriendlyException
{
    /// <summary>
    /// 错误代码类型特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum)]
    public sealed class ErrorCodeTypeAttribute : Attribute
    {
    }
}