using Microsoft.AspNetCore.Mvc.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Handlers;
using OrchardCore.DisplayManagement.Notify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC
{
    public class PublishOptions
    {
        public INotifier Notifier { get; set; }
        public IHtmlLocalizer HtmlLocalizer { get; set; }
        /// <summary>
        /// 返回false 则不执行发布,
        /// 如果需要重新触发默认的 publish 事件，
        ///    需要更新 ：contentitem.published = false
        /// </summary>
        public Func<ContentItem, bool> BeforePublish { get; set; } 
        public Func<ContentValidateResult, bool> OnError { get; set; }
    }
}
