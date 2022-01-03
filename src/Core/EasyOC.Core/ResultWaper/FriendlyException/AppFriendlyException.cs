using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace EasyOC
{
    /// <summary>
    /// 自定义友好异常类
    /// </summary>
    public class AppFriendlyException : Exception
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AppFriendlyException() : base()
        {
        }
        public AppFriendlyException(SimpleError simpleError) : base()
        {
            var desc = simpleError.ToDescriptionOrString();
            switch (simpleError)
            {
                case SimpleError.PermissionDenied:
                    ErrorMessage = desc;
                    ErrorCode = StatusCodes.Status403Forbidden;

                    break;
                case SimpleError.ResourceNotFound:
                    ErrorMessage = desc;
                    ErrorCode = StatusCodes.Status404NotFound;
                    break;
                default:
                    ErrorMessage = desc; 
                    break;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        public AppFriendlyException(string message, object errorCode) : base(message)
        {
            ErrorMessage = message;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        /// <param name="innerException"></param>
        public AppFriendlyException(string message, object errorCode, Exception innerException) : base(message, innerException)
        {
            ErrorMessage = message;
            ErrorCode = errorCode;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        public AppFriendlyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        /// <summary>
        /// 错误码
        /// </summary>
        public object ErrorCode { get; set; }

        /// <summary>
        /// 错误消息（支持 Object 对象）
        /// </summary>
        public object ErrorMessage { get; set; }

        /// <summary>
        /// 状态码 <see cref="StatusCodes"/>
        /// </summary>
        public int StatusCode { get; set; } = StatusCodes.Status500InternalServerError;

        /// <summary>
        /// 是否是数据验证异常
        /// </summary>
        public bool ValidationException { get; set; } = false;
    }
    public enum SimpleError
    {
        [Description("Permission denied.")]
        PermissionDenied,
        [Description("Resource not found.")]
        ResourceNotFound,
    }
}
