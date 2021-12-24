using Jint;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Scripting;
using OrchardCore.Scripting.JavaScript;
using EasyOC.DynamicWebApi;
using EasyOC.DynamicWebApi.Attributes;
using System.Linq;
using Volo.Abp.DependencyInjection;

namespace EasyOC.Core.Application
{
    [DynamicWebApi, IgnoreAntiforgeryToken, AllowAnonymous]
    public class AppServcieBase : IAppServcieBase, IDynamicWebApi
    {

        public AppServcieBase()
        {
            var serviceProvider = ShellScope.Current.ServiceProvider;
            LazyServiceProvider = new AbpLazyServiceProvider(serviceProvider);
        }

        protected IAbpLazyServiceProvider LazyServiceProvider
        {
            get; set;
        } 


        #region Logger
        private ILoggerFactory LoggerFactory => LazyServiceProvider.LazyGetRequiredService<ILoggerFactory>();
        protected ILogger Logger => LazyServiceProvider.LazyGetService<ILogger>(provider => LoggerFactory?.CreateLogger(GetType().FullName) ?? NullLogger.Instance);
        #endregion
         
        protected INotifier Notifier => LazyServiceProvider.LazyGetRequiredService<INotifier>();


        #region IStringLocalizer
        protected IStringLocalizerFactory StringLocalizerFactory => LazyServiceProvider.LazyGetRequiredService<IStringLocalizerFactory>();
        protected IStringLocalizer S => LazyServiceProvider.LazyGetService<IStringLocalizer>(provider => StringLocalizerFactory?.Create(GetType()));

        #endregion

        #region IHtmlLocalizer

        protected IHtmlLocalizerFactory HtmlLocalizerFactory => LazyServiceProvider.LazyGetRequiredService<IHtmlLocalizerFactory>();

        protected IHtmlLocalizer H => LazyServiceProvider.LazyGetService<IHtmlLocalizer>(provider => HtmlLocalizerFactory?.Create(GetType()));

        #endregion

        protected YesSql.ISession YesSession => LazyServiceProvider.LazyGetRequiredService<YesSql.ISession>();


        protected IScriptingManager ScriptingManager => LazyServiceProvider.LazyGetRequiredService<IScriptingManager>();

        protected JavaScriptScope JSScope =>
            LazyServiceProvider.LazyGetService<JavaScriptScope>((provider) =>
            {
                var engine = ScriptingManager.GetScriptingEngine("js");
                var scope = engine.CreateScope(ScriptingManager.GlobalMethodProviders.SelectMany(x => x.GetMethods()), ShellScope.Services, null, null);
                var jsScope = scope as JavaScriptScope;
                return jsScope;
            });
        protected Engine JSEngine => JSScope.Engine;


    }
}



