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
    [DynamicWebApi, Authorize(AuthenticationSchemes = "Api"), IgnoreAntiforgeryToken, AllowAnonymous]
    public class AppServcieBase : IAppServcieBase, IDynamicWebApi
    {
        public IServiceProvider CurrentServiceProvider { get; }
        public AppServcieBase()
        {
            CurrentServiceProvider = ShellScope.Current.ServiceProvider;
            LazyServiceProvider = new EasyOCLazyServiceProvider(CurrentServiceProvider);
        }

        protected IEasyOCLazyServiceProvider LazyServiceProvider
        {
            get; set;
        }

        protected IMapper ObjectMapper => LazyServiceProvider.LazyGetRequiredService<IMapper>();
        protected IHttpContextAccessor HttpContextAccessor => LazyServiceProvider.LazyGetRequiredService<IHttpContextAccessor>();

        protected ClaimsPrincipal HttpUser => HttpContextAccessor.HttpContext.User;

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

        protected YesSql.ISession YesSession => LazyServiceProvider.LazyGetRequiredService<YesSql.ISession>();
        protected IFreeSql FreeSqlSession => LazyServiceProvider.LazyGetRequiredService<IFreeSql>();


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


        protected UserManager<IUser> UserManager => LazyServiceProvider.LazyGetRequiredService<UserManager<IUser>>();

        protected Task<IUser> CurrentUserAsync => LazyServiceProvider.LazyGetService(UserManager.GetUserAsync(HttpUser));

    }
}



