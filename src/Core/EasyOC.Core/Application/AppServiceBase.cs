using AutoMapper;
using EasyOC.Core.DependencyInjection;
using EasyOC.DynamicWebApi;
using EasyOC.DynamicWebApi.Attributes;
using Jint;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.DisplayManagement.Notify;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Scripting;
using OrchardCore.Scripting.JavaScript;
using OrchardCore.Users;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EasyOC.Core.Application
{
    //Api, Identity.Application, Identity.External, Identity.TwoFactorRememberMe, Identity.TwoFactorUserId, OpenIddict.Server.AspNetCore, OpenIddict.Validation.AspNetCore, Wechat
    [DynamicWebApi, AllowAnonymous,IgnoreAntiforgeryToken]
    [Authorize(AuthenticationSchemes = "Api,Identity.Application")]
    public class AppServiceBase : IAppServcieBase, IDynamicWebApi
    {
        public IServiceProvider CurrentServiceProvider { get; }
        public AppServiceBase()
        {
            CurrentServiceProvider = ShellScope.Services;
            LazyServiceProvider = new EasyOCLazyServiceProvider(CurrentServiceProvider);
        }

        protected IEasyOCLazyServiceProvider LazyServiceProvider
        {
            get; set;
        }

        protected IMapper ObjectMapper => LazyServiceProvider.LazyGetRequiredService<IMapper>();
        protected IHttpContextAccessor HttpContextAccessor => LazyServiceProvider.LazyGetRequiredService<IHttpContextAccessor>();

        protected ClaimsPrincipal User => HttpContextAccessor.HttpContext?.User;

        protected IAuthorizationService AuthorizationService => LazyServiceProvider.LazyGetRequiredService<IAuthorizationService>();


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
        protected IContentManager ContentManager => LazyServiceProvider.LazyGetRequiredService<IContentManager>();
        public YesSql.ISession YesSession => LazyServiceProvider.LazyGetRequiredService<YesSql.ISession>();
        protected IFreeSql Fsql => LazyServiceProvider.LazyGetRequiredService<IFreeSql>();


        protected IScriptingManager ScriptingManager => LazyServiceProvider.LazyGetRequiredService<IScriptingManager>();

        protected JavaScriptScope JsScope =>
            LazyServiceProvider.LazyGetService<JavaScriptScope>((provider) =>
            {
                var engine = ScriptingManager.GetScriptingEngine("js");
                var scope = engine.CreateScope(ScriptingManager.GlobalMethodProviders.SelectMany(x => x.GetMethods()), ShellScope.Services, null, null);
                var jsScope = scope as JavaScriptScope;
                return jsScope;
            });
        protected Engine JsEngine => JsScope.Engine;


        protected UserManager<IUser> UserManager => LazyServiceProvider.LazyGetRequiredService<UserManager<IUser>>();

        protected Task<IUser> CurrentUserAsync => LazyServiceProvider.LazyGetService(UserManager.GetUserAsync(User));


        protected IContentDefinitionManager ContentDefinitionManager => LazyServiceProvider.LazyGetRequiredService<IContentDefinitionManager>();



    }
}



