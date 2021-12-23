using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace EasyOC.Core.ResultWaper.Validation
{
    /// <summary>
    /// 验证信息元数据
    /// </summary>
    public sealed class ValidationMetadata
    {
        /// <summary>
        /// 验证结果
        /// </summary>
        public Dictionary<string, string[]> ValidationResult { get; internal set; }

        /// <summary>
        /// 异常消息
        /// </summary>
        public string Message { get; internal set; }

        /// <summary>
        /// 验证状态
        /// </summary>
        public ModelStateDictionary ModelState { get; internal set; }
    }
}