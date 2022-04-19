using AutoMapper;
using EasyOC.Core.Indexs;
using EasyOC.OrchardCore.OpenApi.Model;
using OrchardCore.ContentManagement;
using OrchardCore.Entities;
using YesSql.Indexes;
namespace EasyOC.OrchardCore.OpenApi.Indexs
{
    [AutoMap(typeof(VbenMenu), ReverseMap = true)]
    [EOCIndex("IDX_{tablename}_DocumentId",
        "ContentItemId,Published,Latest,MenuName,RoutePath,Component,MenuType")]
    [EOCTable]
    public class VbenMenuPartIndex : FreeSqlDocumentIndex
    {
        public string ContentItemId { get; set; }
        public bool Published { get; set; }
        public bool Latest { get; set; }
        public string MenuName { get; set; }
        public int OrderNo { get; set; }
        public string RoutePath { get; set; }
        public string Icon { get; set; }
        public string Component { get; set; }
        public bool IsExt { get; set; }
        public bool Keepalive { get; set; }
        public string ParentMenu { get; set; }
        public bool Show { get; set; }
        public string MenuType { get; set; }
        public string ExtentionData { get; set; }

    }
    public class VbenMenuPartIndexProvider : IndexProvider<ContentItem>
    {
        public readonly IMapper _mapper;

        public VbenMenuPartIndexProvider(IMapper mapper)
        {
            _mapper = mapper;
        }

        public override void Describe(DescribeContext<ContentItem> context)
        {
            context.For<VbenMenuPartIndex>()
                 .When(contentItem => contentItem.Has<VbenMenu>())
                 .Map(menu =>
                {
                    var menuPart = menu.ContentItem.As<VbenMenu>();

                    if (menuPart != null)
                    {

                        var menuPartIndex = _mapper.Map<VbenMenuPartIndex>(menuPart);
                        menuPartIndex.ContentItemId = menu.ContentItemId;
                        menuPartIndex.Published = menu.Published;
                        menuPartIndex.Latest = menu.Latest;
                        if (menuPartIndex.RoutePath is not null)
                        {
                            menuPartIndex.RoutePath = menuPartIndex.RoutePath.ToLower();
                        }
                        return menuPartIndex;

                    }
                    return null;
                });
        }
    }

}
