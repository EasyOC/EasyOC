using AutoMapper;
using OrchardCore.ContentFields.Fields;
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
            //CreateMap<UserPickerField, string[]>().ConvertUsing(s => s.UserIds);
            CreateMap<UserPickerField, string>().ConvertUsing(s => s.UserIds.FirstOrDefault());
            //CreateMap<ContentPickerField, string[]>().ConvertUsing(s => s.ContentItemIds);
            CreateMap<DateField, DateTime?>().ConvertUsing(s => s.Value);
            CreateMap<DateField, DateTime>().ConvertUsing(s => s.Value.To<DateTime>());
            CreateMap<DateTimeField, DateTime>().ConvertUsing(s => Convert.ToDateTime(s.Value));
            CreateMap<DateTimeField, DateTime>().ConvertUsing(s => Convert.ToDateTime(s.Value));
            CreateMap<TimeField, TimeSpan?>().ConvertUsing(s => s.Value);
            CreateMap<TimeField, TimeSpan>().ConvertUsing(s => s.Value ?? new TimeSpan());

            #endregion

        }
    }
}



