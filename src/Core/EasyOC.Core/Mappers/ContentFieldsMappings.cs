using AutoMapper;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.Metadata.Models;
using System;
using System.Linq;

namespace EasyOC.Core.Mappers
{
    public class ContentFieldsMappings : Profile
    {
        public ContentFieldsMappings()
        {
            #region OC fileds maping

            CreateMap<MultiTextField, string[]>().ConvertUsing(s => s.Values);
            CreateMap<LinkField, string>().ConvertUsing(s => s == null ? default : s.Text);
            CreateMap<HtmlField, string>().ConvertUsing(s => s == null ? default : s.Html);
            CreateMap<TextField, string>().ConvertUsing(s => s == null ? default : s.Text);
            CreateMap<BooleanField, bool>().ConvertUsing(s => s == null ? default : s.Value);
            CreateMap<NumericField, decimal?>().ConvertUsing(s => s == null ? default : s.Value);
            CreateMap<NumericField, decimal>().ConvertUsing(s => s == null ? default : Convert.ToInt64(s.Value));
            CreateMap<NumericField, int?>().ConvertUsing(s => s == null ? default : Convert.ToInt32(s.Value));
            CreateMap<NumericField, int>().ConvertUsing(s => s == null ? default : s.Value.To<int>());
            CreateMap<NumericField, long?>().ConvertUsing(s => s == null ? default : Convert.ToInt64(s.Value));
            CreateMap<NumericField, long?>().ConvertUsing(s => s == null ? default : Convert.ToInt64(s.Value));
            CreateMap<UserPickerField, string[]>().ConvertUsing(s => s == null ? default : s.UserIds);
            CreateMap<UserPickerField, string>().ConvertUsing(s =>
             s != null && s.UserIds != null && s.UserIds.Length > 0 ?
                            s.UserIds.FirstOrDefault() : string.Empty);
            CreateMap<ContentPickerField, string[]>().ConvertUsing(s => s.ContentItemIds ?? Array.Empty<string>());
            CreateMap<ContentPickerField, string>().ConvertUsing(s =>
                                s != null && s.ContentItemIds != null && s.ContentItemIds.Length > 0 ?
                            s.ContentItemIds.FirstOrDefault() : string.Empty);
            CreateMap<DateField, DateTime?>().ConvertUsing(s => s == null ? default : s.Value);
            CreateMap<DateField, DateTime>().ConvertUsing(s => s == null ? default : s.Value.To<DateTime>());
            CreateMap<DateTimeField, DateTime>().ConvertUsing(s => s == null ? default : Convert.ToDateTime(s.Value));
            CreateMap<DateTimeField, DateTime>().ConvertUsing(s => s == null ? default : Convert.ToDateTime(s.Value));
            CreateMap<TimeField, TimeSpan?>().ConvertUsing(s => s == null ? default : s.Value);
            CreateMap<TimeField, TimeSpan>().ConvertUsing(s => s == null ? default : s.Value ?? new TimeSpan());
            CreateMap<JObject, object>().ConvertUsing(s => s == null ? default : s.ToObject<object>());
            CreateMap<JValue, object>().ConvertUsing(s => s.Value);
            CreateMap<ContentTypeDefinition, ContentTypeDefinitionDto>().ConvertUsing((s, t) => s == null ? default : s.ToDto());
            CreateMap<ContentPartDefinition, ContentPartDefinitionDto>().ConvertUsing((s, t) => s == null ? default : s.ToDto());
            #endregion

        }
    }
    public class JObjectConverter : IValueConverter<JObject, string>
    {
        public string Convert(JObject s, ResolutionContext context)
            => s == null ? default : s.ToString();
    }
}



