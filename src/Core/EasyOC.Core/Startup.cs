using AutoMapper;
using EasyOC.Core.Application;
using EasyOC.Core.Dynamic;
using EasyOC.Core.Swagger;
using EasyOC.DynamicWebApi;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasyOC.Core
{
    [RequireFeatures(Constants.EasyOCCoreModuleId)]
    public class Startup : StartupBase
    {

        public override void ConfigureServices(IServiceCollection services)
        { 
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // 注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(options =>
            {
                options.UseInlineDefinitionsForEnums();
                options.DocumentFilter<SwaggerDocumentFilter>();
                //options.CustomSchemaIds(type => type.Name); 
                options.ParameterFilter<SwaggerEnumParameterFilter>();
                options.SchemaFilter<SwaggerEnumSchemaFilter>();
                options.OperationFilter<SwaggerOperationIdFilter>();
                options.OperationFilter<SwaggerOperationFilter>();
                options.CustomDefaultSchemaIdSelector();
                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Description = "OpenID Connect",
                    Flows = new OpenApiOAuthFlows()
                    {
                        AuthorizationCode = new OpenApiOAuthFlow()
                        {
                            Scopes = new Dictionary<string, string>
                                {
                                { "openid", "OpenID" },
                                { "profile", "Profile" },
                                { "roles", "Roles" }
                                }
                        },
                    }
                });

                options.SwaggerDoc("v1", new OpenApiInfo()
                { Title = "EasyOC Dynamic WebApi", Version = "v1" });
                //options.SchemaGeneratorOptions.
                // TODO:一定要返回true！
                options.DocInclusionPredicate((docName, description) => true);

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
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyOC WebApi");
            });
        }
    }

}



