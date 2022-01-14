using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.Indexing.SQL;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data;
using OrchardCore.Entities;
using OrchardCore.Environment.Shell.Scope;
using OrchardCore.Indexing;
using OrchardCore.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using YesSql.Indexes;

namespace EasyOC.OrchardCore.OpenApi.Indexs
{
    public class CustomUserSettingsIndexProvider : IndexProvider<User>,    IScopedIndexProvider

    {
        private readonly IServiceProvider _serviceProvider;
        private IContentDefinitionManager _contentDefinitionManager;
        private readonly ILogger _logger;
        public CustomUserSettingsIndexProvider(ILogger<CustomUserSettingsIndexProvider> logger, IServiceProvider serviceProvider)
        {

            _logger = logger;
            _serviceProvider = serviceProvider;
        }


        public override void Describe(DescribeContext<User> context)
        {
            context.For<ContentFieldIndex>().Map(user =>
            {
                _contentDefinitionManager ??= ShellScope.Current.ServiceProvider.GetRequiredService<IContentDefinitionManager>();
                var userSettings = _contentDefinitionManager
                  .ListTypeDefinitions()
                  .Where(x => x.GetSettings<ContentTypeSettings>().Stereotype == "CustomUserSettings");
                foreach (var contentTypeDefinition in userSettings)
                {
                    foreach (var contentTypePartDefinition in contentTypeDefinition.Parts)
                    {
                        var partName = contentTypePartDefinition.Name;
                        var partTypeName = contentTypePartDefinition.PartDefinition.Name;
                        foreach (var contentPartFieldDefinition in contentTypePartDefinition.PartDefinition.Fields)
                        {
                            var partFieldIndexSettings = contentPartFieldDefinition.GetSettings<ContentIndexSettings>();

                            if (!partFieldIndexSettings.Included)
                            {
                                continue;
                            }
                            var fieldDefinition = contentPartFieldDefinition.FieldDefinition;
                            var fieldTypeName = fieldDefinition.Name;


                            var contentItem = user.As<ContentItem>(contentTypeDefinition.Name);
                            var jPart = (JObject)contentItem.Content[partName];

                            if (jPart == null)
                            {
                                continue;
                            }

                            var jField = (JObject)jPart[contentPartFieldDefinition.Name];

                            if (jField == null)
                            {
                                continue;
                            }
                            ContentFieldIndex idx = null;

                            switch (contentPartFieldDefinition.FieldDefinition.Name)
                            {
                                case nameof(TextField):
                                    var tfield = jField.ToObject<TextField>();
                                    idx = new TextFieldIndex()
                                    {
                                        Text = tfield.Text?.Substring(0,
                                        Math.Min(tfield.Text.Length, TextFieldIndex.MaxTextSize)),
                                        BigText = tfield.Text
                                    };
                                    break;
                                case nameof(BooleanField):
                                    var bfield = jField.ToObject<BooleanField>();
                                    idx = new BooleanFieldIndex { Boolean = bfield.Value };
                                    break;
                                case nameof(DateField):
                                    var dfield = jField.ToObject<DateField>();
                                    idx = new DateFieldIndex { Date = dfield.Value };
                                    break;

                                case nameof(DateTimeField):
                                    var dtfield = jField.ToObject<DateTimeField>();
                                    idx = new DateTimeFieldIndex { DateTime = dtfield.Value };
                                    break;
                                case nameof(NumericField):
                                    var nfield = jField.ToObject<NumericField>();
                                    idx = new NumericFieldIndex { Numeric = nfield.Value };
                                    break;
                                default:
                                    continue;
                            }
                            idx.Latest = true;
                            idx.Published = true;
                            idx.ContentItemId = contentItem.ContentItemId;
                            idx.ContentItemVersionId = contentItem.ContentItemVersionId;
                            idx.ContentType = contentItem.ContentType;
                            idx.ContentPart = contentTypePartDefinition.Name;
                            idx.ContentField = contentPartFieldDefinition.Name;

                            return idx;


                        }

                    }
                }
                return null;
            });
        } 
    }
}
