using EasyOC.GraphQL.Abstractions;
using EasyOC.GraphQL.Handlers;
using EasyOC.GraphQL.Queries;
using EasyOC.GraphQL.Queries.Types;
using EasyOC.GraphQL.Servicies;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Apis;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.GraphQL;
using OrchardCore.ContentManagement.GraphQL.Mutations;
using OrchardCore.ContentManagement.GraphQL.Mutations.Types;
using OrchardCore.Modules;
using System.Linq;

namespace EasyOC.GraphQL
{

    [RequireFeatures("OrchardCore.ContentFields", "OrchardCore.Media")]

    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {


            services.ReplaceObjectGraphType<ContentPickerFieldQueryObjectType, ContentPickerField, EOCContentPickerFieldQueryObjectType>();
 

            services.AddSingleton<ISchemaBuilder, EOCLuceneQueryFieldTypeProvider>(); 

            services.AddSingleton<ISchemaBuilder, ContentItemByVersionQuery>();  
            //services.AddSingleton<ISchemaBuilder, EOCContentTypeQuery>(); 


            services.AddTransient<PagedContentItemsType>();
            services.AddSingleton<ISchemaBuilder, PagedContentItemsQuery>();
            
            services.AddScoped<IPagedContentItemQueryWhereFilter, DefaultPagedContentItemQueryWhereFilter>();

            //后台执行Graphql API
            services.AddScoped<IGraphqlExecuterService, GraphqlExecuterService>(); 

            services.AddTransient<CreateContentItemInputType>();
            services.AddGraphMutationType<CreateContentItemMutation>();
            services.AddGraphMutationType<DeleteContentItemMutation>();
            services.AddTransient<DeletionStatusObjectGraphType>();
            services.AddTransient<CreateContentItemInputType>();
        }
    }

}
