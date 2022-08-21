using EasyOC.GraphQL.Queries;
using GraphQL.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.Apis.GraphQL;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.GraphQL;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.GraphQL;
using OrchardCore.ContentManagement.GraphQL.Queries.Types;
using OrchardCore.Media;
using OrchardCore.Media.Fields;
using OrchardCore.Media.GraphQL;
using OrchardCore.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyOC.GraphQL
{
    public class OverrideRegisteredObjectTypesProvider : ISchemaBuilder
    {


        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EOCLuceneQueryFieldTypeProvider> _logger;

        public OverrideRegisteredObjectTypesProvider(IHttpContextAccessor httpContextAccessor,
            ILogger<EOCLuceneQueryFieldTypeProvider> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public Task<string> GetIdentifierAsync()
        {
            var queryManager = _httpContextAccessor.HttpContext.RequestServices.GetService<IQueryManager>();
            return queryManager.GetIdentifierAsync();
        }

        public async Task BuildAsync(ISchema schema)
        {
            var meediaFieldType =  schema.FindType(nameof(MediaField)) as MediaFieldQueryObjectType;
            try
            {
                meediaFieldType.Field<ListGraphType<StringGraphType>, IEnumerable<string>>().Name("paths")
                    .Description("the media paths").PagingArguments()
                    .Resolve(x =>
                    {
                        if(x.Source.Paths == null)
                        {
                            return new List<string>();
                        }
                        return x.Page(x.Source.Paths);
                    }
                    );
                meediaFieldType.Fields.FirstOrDefault(x=>x.Name=="urls")?.Resolver=
                meediaFieldType .Field<ListGraphType<StringGraphType>, IEnumerable<string>>()
                    .Name("urls").Description("the absolute urls of the media items")
                    .PagingArguments()
                    .Resolve(
                    (x =>
                    {
                        if(x.Source.Paths == null)
                        {
                            return new List<string>();
                        }
                        IEnumerable<string> source = x.Page(x.Source.Paths);
                        IMediaFileStore mediaFileStore = ((GraphQLContext)x.UserContext).ServiceProvider.GetService<IMediaFileStore>();
                        Func<string, string> selector =(p => mediaFileStore.MapPathToPublicUrl(p));
                        return source.Select(selector);
                    }));
                // var mediaFieldType = schema.FindType(nameof(ContentPickerField));
                // mediaFieldType = new MediaFieldQueryObjectType();


                Console.WriteLine(schema);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
