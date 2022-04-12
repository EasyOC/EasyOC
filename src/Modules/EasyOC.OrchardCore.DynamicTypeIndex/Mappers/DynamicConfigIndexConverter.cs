using AutoMapper;
using EasyOC.OrchardCore.DynamicTypeIndex.Index;
using EasyOC.OrchardCore.DynamicTypeIndex.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOC.OrchardCore.DynamicTypeIndex.Mappers
{
    public class ContentItemToConfigConverter : IValueConverter<ContentItem, DynamicIndexConfigModel>
    {
        public DynamicIndexConfigModel Convert(ContentItem sourceMember, ResolutionContext context)
        {
            var part = sourceMember.As<DynamicIndexConfigSetting>();
            if (part == null)
            {
                return null;
            }
            var config = new DynamicIndexConfigModel();
            config.TableName = part.TableName.Text;
            config.TypeName = part.TypeName.Text;
            if (!part.ConfigData.Text.IsNullOrWhiteSpace())
            {
                config.Fields = JsonConvert.DeserializeObject<List<DynamicIndexFieldItem>>(part.ConfigData.Text);
            }
            if (!part.ConfigData.Text.IsNullOrWhiteSpace())
            {
                config.EntityInfo = JsonConvert.DeserializeObject<DynamicIndexEntityInfo>(part.EntityInfo.Text);
            }
            config.ContentItemId = sourceMember.ContentItemId;
            return config;
        }
    }
    //public class ConfigToContentItemConverter : IValueConverter<DynamicIndexConfigModel, ContentItem>
    //{
    //    public ContentItem Convert(DynamicIndexConfigModel sourceMember, ResolutionContext context)
    //    {
            
    //        var part = sourceMember.As<DynamicIndexConfigSetting>();
    //        if (part == null)
    //        {
    //            return null;
    //        }
    //        var config = new DynamicIndexConfigModel();
    //        config.TableName = part.TableName.Text;
    //        config.TypeName = part.TypeName.Text;
    //        if (!part.ConfigData.Text.IsNullOrWhiteSpace())
    //        {
    //            config.Fields = JsonConvert.DeserializeObject<List<DynamicIndexFieldItem>>(part.ConfigData.Text);
    //        }
    //        if (!part.ConfigData.Text.IsNullOrWhiteSpace())
    //        {
    //            config.EntityInfo = JsonConvert.DeserializeObject<DynamicIndexEntityInfo>(part.EntityInfo.Text);
    //        }
    //        config.ContentItemId = sourceMember.ContentItemId;
    //        return config;
    //    }
    //}
}
