using OrchardCore.Users.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrchardCore.ContentFields.Indexing.SQL;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data;
using OrchardCore.Entities;
using OrchardCore.Indexing;
using OrchardCore.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using YesSql.Indexes;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentFields.Fields;
using EasyOC.OrchardCore.OpenApi.Indexs;

namespace EasyOC.OrchardCore.OpenApi.Handlers
{
    public class UserEventHandler : UserEventHandlerBase
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IFreeSql _fsql;
        private readonly ILogger _logger;
        public UserEventHandler(
          IContentDefinitionManager contentDefinitionManager,
          ILogger<UserEventHandler> logger,
          IFreeSql fsql)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _logger = logger;
            _fsql = fsql;
        }

        private IEnumerable<ContentTypeDefinition> GetUserSettingsTypeDefinitions()
       => _contentDefinitionManager
           .ListTypeDefinitions()
           .Where(x => x.GetSettings<ContentTypeSettings>().Stereotype == "CustomUserSettings");


        private async Task UpdateIndex(UserContextBase context)
        {
            var user = context.User as User;


            foreach (var contentTypeDefinition in GetUserSettingsTypeDefinitions())
            {
                var contentItem = user.As<ContentItem>(contentTypeDefinition.Name);



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
                        SetContentFieldIndexValue(contentItem, contentTypePartDefinition, contentPartFieldDefinition, idx);
                        _fsql.Insert(idx);
                    }

                }
            }
        }

        private static void SetContentFieldIndexValue(ContentItem contentItem, ContentTypePartDefinition contentTypePartDefinition, ContentPartFieldDefinition contentPartFieldDefinition, ContentFieldIndex idx)
        {
            idx.Latest = true;
            idx.Published = true;
            idx.ContentItemId = contentItem.ContentItemId;
            idx.ContentItemVersionId = contentItem.ContentItemVersionId;
            idx.ContentType = contentItem.ContentType;
            idx.ContentPart = contentTypePartDefinition.Name;
            idx.ContentField = contentPartFieldDefinition.Name;
        }



        public override async Task CreatedAsync(UserCreateContext context)
        {

        }

        public override async Task UpdatedAsync(UserUpdateContext context)
        {
        }

        public override async Task DeletedAsync(UserDeleteContext context)
        {
        }
    }
}
