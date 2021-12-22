using Microsoft.Extensions.Localization;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentManagement.Metadata.Models;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.ContentTypes.Editors;
using OrchardCore.DisplayManagement.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lombiq.HelpfulExtensions.Extensions.CodeGeneration
{
    public class CodeGenerationDisplayDriver : ContentTypeDefinitionDisplayDriver
    {
        private readonly IStringLocalizer T;

        public CodeGenerationDisplayDriver(IStringLocalizer<CodeGenerationDisplayDriver> stringLocalizer) =>
            T = stringLocalizer;

        public override IDisplayResult Edit(ContentTypeDefinition model) =>
            Initialize<ContentTypeMigrationsViewModel>(
                "ContentTypeMigrations_Edit",
                viewModel => viewModel.MigrationCodeLazy = new Lazy<string>(() =>
                {
                    var codeBuilder = new StringBuilder();

                    // Building the code for the type.
                    var name = model.Name;
                    codeBuilder.AppendLine($"_contentDefinitionManager.AlterTypeDefinition(\"{name}\", type => type");
                    codeBuilder.AppendLine($"    .DisplayedAs(\"{model.DisplayName}\")");

                    GenerateCodeForSettings(codeBuilder, model.GetSettings<ContentTypeSettings>());
                    AddSettingsWithout<ContentTypeSettings>(codeBuilder, model.Settings, 4);
                    GenerateCodeForParts(codeBuilder, model.Parts);
                    codeBuilder.AppendLine(");");

                    GenerateCodeForPartsWithFields(codeBuilder, model.Parts);

                    return codeBuilder.ToString();
                }))
            .Location("Content:7");

        private void GenerateCodeForParts(StringBuilder codeBuilder, IEnumerable<ContentTypePartDefinition> parts)
        {
            foreach (var part in parts)
            {
                var partSettings = part.GetSettings<ContentTypePartSettings>();

                codeBuilder.AppendLine($"    .WithPart(\"{part.Name}\", part => part");

                var partStartingLength = codeBuilder.Length;

                AddWithLine(codeBuilder, nameof(partSettings.DisplayName), partSettings.DisplayName);
                AddWithLine(codeBuilder, nameof(partSettings.Description), partSettings.Description);
                AddWithLine(codeBuilder, nameof(partSettings.Position), partSettings.Position);
                AddWithLine(codeBuilder, nameof(partSettings.DisplayMode), partSettings.DisplayMode);
                AddWithLine(codeBuilder, nameof(partSettings.Editor), partSettings.Editor);

                AddSettingsWithout<ContentTypePartSettings>(codeBuilder, part.Settings, 8);

                // Checking if anything was added to the part's settings.
                if (codeBuilder.Length == partStartingLength)
                {
                    // Remove ", part => part" and the line break.
                    codeBuilder.Length -= 16;
                    codeBuilder.Append(")" + Environment.NewLine);
                }
                else
                {
                    codeBuilder.AppendLine("    )");
                }
            }
        }

        /// <summary>
        /// Building those parts that have fields separately (fields can't be configured inline in types).
        /// </summary>
        private void GenerateCodeForPartsWithFields(
            StringBuilder codeBuilder,
            IEnumerable<ContentTypePartDefinition> parts)
        {
            var partDefinitions = parts
                .Where(part => part.PartDefinition.Fields.Any())
                .Select(part => part.PartDefinition);
            foreach (var part in partDefinitions)
            {
                codeBuilder.AppendLine();
                codeBuilder.AppendLine($"_contentDefinitionManager.AlterPartDefinition(\"{part.Name}\", part => part");

                var partSettings = part.GetSettings<ContentPartSettings>();
                if (partSettings.Attachable) codeBuilder.AppendLine("    .Attachable()");
                if (partSettings.Reusable) codeBuilder.AppendLine("    .Reusable()");

                AddWithLine(codeBuilder, nameof(partSettings.DisplayName), partSettings.DisplayName);
                AddWithLine(codeBuilder, nameof(partSettings.Description), partSettings.Description);
                AddWithLine(codeBuilder, nameof(partSettings.DefaultPosition), partSettings.DefaultPosition);

                AddSettingsWithout<ContentPartSettings>(codeBuilder, part.Settings, 4);

                foreach (var field in part.Fields)
                {
                    codeBuilder.AppendLine($"    .WithField(\"{field.Name}\", field => field");
                    codeBuilder.AppendLine($"        .OfType(\"{field.FieldDefinition.Name}\")");

                    var fieldSettings = field.GetSettings<ContentPartFieldSettings>();
                    AddWithLine(codeBuilder, nameof(fieldSettings.DisplayName), fieldSettings.DisplayName);
                    AddWithLine(codeBuilder, nameof(fieldSettings.Description), fieldSettings.Description);
                    AddWithLine(codeBuilder, nameof(fieldSettings.Editor), fieldSettings.Editor);
                    AddWithLine(codeBuilder, nameof(fieldSettings.DisplayMode), fieldSettings.DisplayMode);
                    AddWithLine(codeBuilder, nameof(fieldSettings.Position), fieldSettings.Position);

                    AddSettingsWithout<ContentPartFieldSettings>(codeBuilder, field.Settings, 8);

                    codeBuilder.AppendLine("    )");
                }

                codeBuilder.AppendLine(");");
            }
        }

        private string ConvertJToken(JToken jToken)
        {
            switch (jToken)
            {
                case JValue jValue:
                    var value = jValue.Value;
                    return value switch
                    {
                        bool boolValue => boolValue ? "true" : "false",
                        string => $"\"{value}\"",
                        _ => value?.ToString()?.Replace(',', '.'), // Replace decimal commas.
                    };
                case JArray jArray:
                    return $"new[] {{ {string.Join(", ", jArray.Select(ConvertJToken))} }}";
                case JObject jObject:
                    // Using a quoted string so it doesn't mess up the syntax highlighting of the rest of the code.
                    return T["\"FIX ME! Couldn't determine the actual type to instantiate.\" {0}", jObject.ToString()];
                default:
                    throw new NotSupportedException($"Settings values of type {jToken.GetType()} are not supported.");
            }
        }

        private void AddSettingsWithout<T>(StringBuilder codeBuilder, JObject settings, int indentationDepth)
        {
            var indentation = string.Join(string.Empty, Enumerable.Repeat(" ", indentationDepth));

            var filteredSettings = ((IEnumerable<KeyValuePair<string, JToken>>)settings)
                .Where(setting => setting.Key != typeof(T).Name);
            foreach (var setting in filteredSettings)
            {
                var properties = setting.Value.Where(property => property is JProperty).Cast<JProperty>().ToArray();

                if (properties.Length == 0) continue;

                codeBuilder.AppendLine($"{indentation}.WithSettings(new {setting.Key}");
                codeBuilder.AppendLine(indentation + "{");

                // This doesn't support multi-level object hierarchies for settings but come on, who uses complex
                // settings objects?
                for (int i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    var propertyValue = ConvertJToken(property.Value);

                    if (propertyValue == null)
                    {
                        propertyValue = "\"\"";
                    }
                    else if (propertyValue.Contains(Environment.NewLine, StringComparison.OrdinalIgnoreCase))
                    {
                        propertyValue = "@" + propertyValue;
                    }

                    codeBuilder.AppendLine($"{indentation}    {property.Name} = {propertyValue},");
                }

                codeBuilder.AppendLine(indentation + "})");
            }
        }

        private static void GenerateCodeForSettings(StringBuilder codeBuilder, ContentTypeSettings contentTypeSettings)
        {
            if (contentTypeSettings.Creatable) codeBuilder.AppendLine("    .Creatable()");
            if (contentTypeSettings.Listable) codeBuilder.AppendLine("    .Listable()");
            if (contentTypeSettings.Draftable) codeBuilder.AppendLine("    .Draftable()");
            if (contentTypeSettings.Versionable) codeBuilder.AppendLine("    .Versionable()");
            if (contentTypeSettings.Securable) codeBuilder.AppendLine("    .Securable()");
            if (!string.IsNullOrEmpty(contentTypeSettings.Stereotype))
            {
                codeBuilder.AppendLine($"    .Stereotype(\"{contentTypeSettings.Stereotype}\")");
            }
        }

        private static void AddWithLine(StringBuilder codeBuilder, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                codeBuilder.AppendLine($"        .With{name}(\"{value}\")");
            }
        }
    }
}



