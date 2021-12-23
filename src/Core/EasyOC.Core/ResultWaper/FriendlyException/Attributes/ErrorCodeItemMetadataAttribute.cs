

using System;

namespace Furion.FriendlyException
{
    /// <summary>
    /// 异常元数据特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class ErrorCodeItemMetadataAttribute : Attribute
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="errorMessage">错误消息</param>
        /// <param name="args">格式化参数</param>
        public ErrorCodeItemMetadataAttribute(string errorMessage, params object[] args)
        {
            ErrorMessage = errorMessage;
            Args = args;
        }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        public object ErrorCode { get; set; }

        /// <summary>
        /// 格式化参数
        /// </summary>
        public object[] Args { get; set; }
    }
}