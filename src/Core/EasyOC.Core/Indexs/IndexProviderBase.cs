using AutoMapper;
using OrchardCore.ContentManagement;
using YesSql.Indexes;

namespace EasyOC.Core.Indexs
{
    public class IndexProviderBase<T, Part> : IndexProvider<Part>
        where T : IndexBaseModel, new()
        where Part : ContentPart
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
        public virtual T GetMapIndex(Part partItem)
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
        public override void Describe(DescribeContext<Part> context)
        {
            context.For<T>().Map(contentItem =>
            {
                return GetMapIndex(contentItem);
            });
        }
    }
}



