using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Apis.GraphQL;
using OrchardCore.Media;
using OrchardCore.Media.Fields;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EaysOC.GraphQL.Queries.Types
{
    public class MediaFieldQueryObjectType : ObjectGraphType<MediaField>
    {
        public MediaFieldQueryObjectType()
        {
            this.Name = "MediaField";
            this.Field<ListGraphType<StringGraphType>, IEnumerable<string>>().Name("paths")
                .Description("the media paths").PagingArguments<MediaField, IEnumerable<string>>()
                .Resolve((Func<ResolveFieldContext<MediaField>, IEnumerable<string>>)
                (x =>
                {
                    if(x.Source.Paths == null)
                    {
                        return new List<string>();
                    }
                    return x.Page<MediaField, string>((IEnumerable<string>)x.Source.Paths);
                })
                );
            this.Field<ListGraphType<StringGraphType>, IEnumerable<string>>()
                .Name("urls").Description("the absolute urls of the media items")
                .PagingArguments<MediaField, IEnumerable<string>>()
                .Resolve((Func<ResolveFieldContext<MediaField>, IEnumerable<string>>)
                (x =>
                {
                    if(x.Source.Paths == null)
                    {
                        return new List<string>();
                    }
                    IEnumerable<string> source = x.Page<MediaField, string>((IEnumerable<string>)x.Source.Paths);
                    IMediaFileStore mediaFileStore = ((GraphQLContext)x.UserContext).ServiceProvider.GetService<IMediaFileStore>();
                    Func<string, string> selector = (Func<string, string>)(p => mediaFileStore.MapPathToPublicUrl(p));
                    return source.Select<string, string>(selector);
                }));
        }
    }
}
