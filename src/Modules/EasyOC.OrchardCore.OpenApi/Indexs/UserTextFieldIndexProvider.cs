using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.Indexing.SQL;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data;
using OrchardCore.Entities;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Users.Models;
using YesSql.Indexes;

namespace EasyOC.OrchardCore.OpenApi.Indexs
{

    public class UserTextFieldIndexProvider : IndexProvider<User>, IScopedIndexProvider, IIndexProvider
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly HashSet<string> _ignoredTypes = new HashSet<string>();
        private IContentDefinitionManager _contentDefinitionManager;

        public UserTextFieldIndexProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override void Describe(DescribeContext<User> context)
        {
            context.For<TextFieldIndex>()
                .Map(user =>
                {

                    var results = new List<TextFieldIndex>();
                    _contentDefinitionManager ??= ShellScope.Current.ServiceProvider.GetRequiredService<IContentDefinitionManager>();

                    // Search for TextField
                    var contentTypeDefinitions = _contentDefinitionManager
                                           .ListTypeDefinitions()
                                           .Where(x => x.GetSettings<ContentTypeSettings>().Stereotype == "CustomUserSettings");
                    foreach (var contentTypeDefinition in contentTypeDefinitions)
                    {
                        var contentItem = user.As<ContentItem>(contentTypeDefinition.Name);
                        var fieldDefinitions = contentTypeDefinitions.SelectMany(x => x.Parts.SelectMany(x => x.PartDefinition.Fields.Where(f => f.FieldDefinition.Name == nameof(TextField))))
                                        .ToArray();


                        // Lazy initialization because of ISession cyclic dependency
                        _contentDefinitionManager ??= _serviceProvider.GetRequiredService<IContentDefinitionManager>();

                        // Remove index records of soft deleted items.
                        //if (!contentItem.Published && !contentItem.Latest)
                        //{
                        //    return null;
                        //}

                        // Can we safely ignore this content item?
                        if (_ignoredTypes.Contains(contentItem.ContentType))
                        {
                            continue;
                        }



                        // This can occur when content items become orphaned, particularly layer widgets when a layer is removed, before its widgets have been unpublished.
                        if (contentTypeDefinition == null)
                        {
                            _ignoredTypes.Add(contentItem.ContentType);
                            continue; 
                        }


                        // This type doesn't have any TextField, ignore it
                        if (fieldDefinitions.Length == 0)
                        {
                            _ignoredTypes.Add(contentItem.ContentType);
                            continue;
                        }


                        foreach (var fieldDefinition in fieldDefinitions)
                        {
                            var jPart = (JObject)contentItem.Content[fieldDefinition.PartDefinition.Name];

                            if (jPart == null)
                            {
                                continue;
                            }

                            var jField = (JObject)jPart[fieldDefinition.Name];

                            if (jField == null)
                            {
                                continue;
                            }

                            var field = jField.ToObject<TextField>();

                            results.Add(new TextFieldIndex
                            {
                                Latest = contentItem.Latest,
                                Published = contentItem.Published,
                                ContentItemId = contentItem.ContentItemId,
                                ContentItemVersionId = contentItem.ContentItemVersionId,
                                ContentType = contentItem.ContentType,
                                ContentPart = fieldDefinition.PartDefinition.Name,
                                ContentField = fieldDefinition.Name,
                                Text = field.Text?.Substring(0, Math.Min(field.Text.Length, TextFieldIndex.MaxTextSize)),
                                BigText = field.Text
                            });
                        }

                    }

                    return results;
                });
        }
    }
}
