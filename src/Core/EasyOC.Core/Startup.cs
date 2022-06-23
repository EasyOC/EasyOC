using EasyOC.Core.Authorization.Permissions;
using EasyOC.Core.Dynamic;
using EasyOC.Core.Swagger;
using EasyOC.DynamicWebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Modules;
using OrchardCore.Settings;
using System;
using System.Collections.Generic;
using System.IO;

namespace EasyOC.Core
{
    [RequireFeatures(Constants.EasyOCCoreModuleId)]
    public class Startup : StartupBase
    {
        private readonly IShellConfiguration _shellConfiguration;
        public Startup(IShellConfiguration shellConfiguration)
        {
            _shellConfiguration = shellConfiguration;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddFreeSql();
            //发现并注册所有引入了 AutoMapper 的程序集  ,但是这样没法触发后续加载的程序集
            //services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies().Where(a =>
            //    a.GetReferencedAssemblies().Any(x => x.FullName.StartsWith(nameof(AutoMapper))))
            //);
            services.AddAutoMapper(this.GetType().Assembly);

            // 注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(options =>
            {
                options.UseInlineDefinitionsForEnums();
                options.DocumentFilter<SwaggerDocumentFilter>();
                options.CustomSchemaIds(type => type.Name);
                options.ParameterFilter<SwaggerEnumParameterFilter>();
                options.SchemaFilter<SwaggerEnumSchemaFilter>();
                options.OperationFilter<SwaggerOperationIdFilter>();
                options.OperationFilter<SwaggerOperationFilter>();
                options.CustomDefaultSchemaIdSelector();
                var serviceProvider = ShellScope.Current.ServiceProvider;

                var siteService = serviceProvider.GetRequiredService<ISiteService>();

                var site = siteService.GetSiteSettingsAsync().GetAwaiter().GetResult();
                var baseUrl = site.BaseUrl;

                //var baseUrl = _shellConfiguration["AuthServer:Authority"].EnsureEndsWith('/');
                if (string.IsNullOrEmpty(baseUrl))
                {
                    baseUrl = "/";
                    //var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();
                    //var request = httpContextAccessor.HttpContext.Request;
                    //var tenantSettings = ShellScope.Context.Settings;
                    //baseUrl = $"{request.Scheme}://{request.Host}/{tenantSettings.RequestUrlPrefix}";
                }
                baseUrl = baseUrl.EnsureEndsWith('/');
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {

                    Type = SecuritySchemeType.OAuth2,
                    Description = "OpenID Connect",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Flows = new OpenApiOAuthFlows()
                    {
                        AuthorizationCode = new OpenApiOAuthFlow()
                        {
                            Scopes = new Dictionary<string, string>
                                {
                                    { "openid", "OpenID" },
                                    { "profile", "Profile" },
                                    { "roles", "Roles" },
                                    { "offline_access", "offline access" },
                                    // { "api", "Api" },
                                },
                            AuthorizationUrl = new Uri($"{baseUrl}connect/authorize", UriKind.RelativeOrAbsolute),
                            TokenUrl = new Uri($"{baseUrl}connect/token", UriKind.RelativeOrAbsolute),
                        },
                    }
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                        {
                            {
                                new OpenApiSecurityScheme
                                {
                                    Reference = new OpenApiReference
                                    {
                                        Type = ReferenceType.SecurityScheme,
                                        Id = "Bearer"
                                    }
                                },
                                Array.Empty<string>()
                            }
                        });
                options.SwaggerDoc("v1", new OpenApiInfo()
                { Title = "EasyOC Dynamic WebApi", Version = "v1" });
                //options.SchemaGeneratorOptions.
                // TODO:一定要返回true！
                options.DocInclusionPredicate((docName, description) => true);

                //xml 配置文档
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var xmlDocFiles = Directory.GetFiles(baseDirectory, "*.xml");
                foreach (var xmlFile in xmlDocFiles)
                {
                    var asName = Path.GetFileNameWithoutExtension(xmlFile);
                    if (File.Exists(Path.Combine(baseDirectory, asName + ".dll")))
                    {
                        var xmlPath = Path.Combine(baseDirectory, xmlFile);

                        options.IncludeXmlComments(xmlPath);
                    }
                }
            }).AddSwaggerGenNewtonsoftSupport();
            //自定义配置
            services.AddDynamicWebApi((options) =>
            {
                // 指定全局默认的 api 前缀
                options.DefaultApiPrefix = "api";
                options.ActionRouteFactory = new ServiceActionRouteFactory();
            });

            //授权处理
            services.AddScoped<IOrchardCorePermissionService, OrchardCorePermissionService>();

            //友好异常，同一返回值封装
            services.AddUnifyResult();
        }

        public override void Configure(IApplicationBuilder app,
            IEndpointRouteBuilder routes,
            IServiceProvider serviceProvider)
        {
            //routes.MapAreaControllerRoute(
            //    name: "Home",
            //    areaName: "EasyOC.Core",
            //    pattern: "Home/Index",
            //    defaults: new { controller = "Home", action = "Index" }
            //);
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});
            //启用中间件服务生成Swagger作为JSON终结点
            app.UseSwagger();
            //启用中间件服务对swagger-ui，指定Swagger JSON终结点
            app.UseSwaggerUI(options =>
            {
                var serviceProvider = app.ApplicationServices;
                var env = serviceProvider.GetRequiredService<IHostEnvironment>();
                if (env.IsDevelopment())
                {
                    options.OAuthClientId(_shellConfiguration["AuthServer:SwaggerClientId"]);
                    options.OAuthClientSecret(_shellConfiguration["AuthServer:SwaggerClientSecret"]);
                    options.OAuth2RedirectUrl(_shellConfiguration["AuthServer:SwaggerOAuth2RedirectUrl"]);
                }
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyOC WebApi");
                options.OAuthScopes("openid", "profile", "roles", "api","offline_access");

            });
        }
    }

}



