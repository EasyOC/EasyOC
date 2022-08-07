using AutoMapper;
using OrchardCore.ContentManagement;
using YesSql.Indexes;

namespace EasyOC.Core.Indexes
{
    public class IndexProviderBase<T, TPart> : IndexProvider<TPart>
        where T : IndexBaseModel, new()
        where TPart : ContentPart
    {
        protected readonly IMapper Mapper;

        public IndexProviderBase(IMapper mapper)
        {
            Mapper = mapper;
        }
        /// <summary>
        /// 可重写
        /// </summary>
        /// <param name="partItem"></param>
        /// <returns></returns>
        public virtual T GetMapIndex(TPart partItem)
        {
            var partModel = partItem;
            if (partModel == null)
            {
                return null;
            }
            else
            {
                var partIndex = new T()
                {
                    ContentItemId = partItem.ContentItem.ContentItemId,
                };
                return Mapper.Map(partModel, partIndex);
            }
        }
        public override void Describe(DescribeContext<TPart> context)
        {
            context.For<T>().Map(contentItem =>
            {
                return GetMapIndex(contentItem);
            });
        }
    }
}



