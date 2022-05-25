using GraphQL.Types;

namespace EaysOC.GraphQL.Queries.Types
{
    public class ContentTypePickerInput:EnumerationGraphType
    {
        public PublicationStatusGraphType()
        {
            Name = "Status";
            Description = "publication status";
            AddValue("PUBLISHEDORLATEST", "published or latest version content item",
            PublicationStatusEnum.PublishedOrLatest);
            AddValue("PUBLISHED", "published content item version", PublicationStatusEnum.Published);
            AddValue("DRAFT", "draft content item version", PublicationStatusEnum.Draft);
            AddValue("LATEST", "the latest version, either published or draft", PublicationStatusEnum.Latest);
            AddValue("ALL", "all historical versions", PublicationStatusEnum.All);
        }
    }
}
