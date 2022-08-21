using AutoMapper;
using EasyOC.Core.Indexes;
using FreeSql.DataAnnotations;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using YesSql.Indexes;

namespace EasyOC.DynamicTypeIndex.Index
{

    public class DynamicIndexConfigSetting : ContentPart
    {
        public TextField TypeName { get; set; }
        public TextField TableName { get; set; }
        public TextField ConfigData { get; set; }
        public TextField EntityInfo { get; set; }
        public TextField EntityContent { get; set; }

    }

    [AutoMap(typeof(DynamicIndexConfigSetting), ReverseMap = true)]
    [EOCIndex("IDX_{tablename}_DocumentId",
     "TypeName,TableName")]
    [EOCTable]
    public class DynamicIndexConfigDataIndex : FreeSqlDocumentIndex
    {
        [Column(StringLength = 26)]
        public string ContentItemId { get; set; }
        public string TypeName { get; set; }

        public string TableName { get; set; }
        //public bool Enabled { get; set; }
        [Column(StringLength = -1)]
        public string ConfigData { get; set; }
    }

    public class DynamicIndexConfigDataIndexProvider : IndexProvider<ContentItem>
    {
        public readonly IMapper _mapper;

        public DynamicIndexConfigDataIndexProvider(IMapper mapper)
        {
            _mapper = mapper;
        }

        public override void Describe(DescribeContext<ContentItem> context)
        {
            context.For<DynamicIndexConfigDataIndex>()
                 .When(
                    contentItem => contentItem.ContentType == nameof(DynamicIndexConfigSetting)
                                   && contentItem.Has<DynamicIndexConfigSetting>())
                .Map(contentItem =>
                {
                    var part = contentItem.As<DynamicIndexConfigSetting>();

                    if (part != null)
                    {
                        var partIndex = _mapper.Map<DynamicIndexConfigDataIndex>(part);
                        partIndex.ContentItemId = contentItem.ContentItemId;
                        return partIndex;
                    }
                    return null;
                });
        }
    }
}
