using AutoMapper;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement.Metadata.Models;
using System;
using System.Linq;

namespace EasyOC.Core.Mappers
{
    public class ContentFeildsMappings : Profile
    {
        public ContentFeildsMappings()
        {
            #region OC fileds maping

            CreateMap<MultiTextField, string[]>().ConvertUsing(s => s.Values);
            CreateMap<LinkField, string>().ConvertUsing(s => s.Text);
            CreateMap<HtmlField, string>().ConvertUsing(s => s.Html);
            CreateMap<TextField, string>().ConvertUsing(s => s.Text);
            CreateMap<BooleanField, bool>().ConvertUsing(s => s.Value);
            CreateMap<NumericField, decimal?>().ConvertUsing(s => s.Value);
            CreateMap<NumericField, decimal>().ConvertUsing(s => Convert.ToInt64(s.Value));
            CreateMap<NumericField, int?>().ConvertUsing(s => Convert.ToInt32(s.Value));
            CreateMap<NumericField, int>().ConvertUsing(s => s.Value.To<int>());
            CreateMap<NumericField, long?>().ConvertUsing(s => Convert.ToInt64(s.Value));
            CreateMap<NumericField, long?>().ConvertUsing(s => Convert.ToInt64(s.Value));
            CreateMap<UserPickerField, string[]>().ConvertUsing(s => s.UserIds);
            CreateMap<UserPickerField, string>().ConvertUsing(s => s.UserIds.FirstOrDefault());
            CreateMap<ContentPickerField, string[]>().ConvertUsing(s => s.ContentItemIds);
            CreateMap<ContentPickerField, string>().ConvertUsing(s => s.ContentItemIds.FirstOrDefault());
            CreateMap<DateField, DateTime?>().ConvertUsing(s => s.Value);
            CreateMap<DateField, DateTime>().ConvertUsing(s => s.Value.To<DateTime>());
            CreateMap<DateTimeField, DateTime>().ConvertUsing(s => Convert.ToDateTime(s.Value));
            CreateMap<DateTimeField, DateTime>().ConvertUsing(s => Convert.ToDateTime(s.Value));
            CreateMap<TimeField, TimeSpan?>().ConvertUsing(s => s.Value);
            CreateMap<TimeField, TimeSpan>().ConvertUsing(s => s.Value ?? new TimeSpan());
            CreateMap<JValue, object>().ConvertUsing(source => source.Value);
            CreateMap<ContentTypeDefinition, ContentTypeDefinitionDto>().ConvertUsing((s, t) => s.ToDto());
            CreateMap<ContentPartDefinition, ContentPartDefinitionDto>().ConvertUsing((s, t) => s.ToDto());
            #endregion

        }
    }
    public class JObjectConverter : IValueConverter<JObject, string>
    {
        public string Convert(JObject s, ResolutionContext context)
            => s != null ? s.ToString() : "";
    }
}



