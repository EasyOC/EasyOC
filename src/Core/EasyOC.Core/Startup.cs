using EasyOC.Core.Dynamic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Modules;
using Panda.DynamicWebApi;
using System;
using System.IO;
using Volo.Abp.DependencyInjection;

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
            services.AddScoped<IAbpLazyServiceProvider>(sp=> new AbpLazyServiceProvider(sp));
            services.AddAutoMapper(GetType().Assembly); 

            // 注册Swagger生成器，定义一个和多个Swagger 文档
            services.AddSwaggerGen(options =>
            {
                options.CustomSchemaIds(type => type.FullName);
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                { Title = "EasyOC Manufacture ERP Dynamic WebApi", Version = "v1" });
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
            });
            //自定义配置
            services.AddDynamicWebApi((options) =>
            {
                // 指定全局默认的 api 前缀
                options.DefaultApiPrefix = "api";
                options.ActionRouteFactory = new ServiceActionRouteFactory();

            });
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyOC ManufactureERP WebApi");
            });
        }
    }

}



