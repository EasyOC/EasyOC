
using System;

namespace Furion.FriendlyException
{
    /// <summary>
    /// 异常复写特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public sealed class IfExceptionAttribute : Attribute
    {
          
        /// <summary>
        /// 错误编码
        /// </summary>
        public object ErrorCode { get; set; }

        /// <summary>
        /// 异常类型
        /// </summary>
        public Type ExceptionType { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }
         
    }
}
