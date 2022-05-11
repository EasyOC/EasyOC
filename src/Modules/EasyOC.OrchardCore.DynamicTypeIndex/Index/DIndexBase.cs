using EasyOC.Core.Indexs;
using FreeSql.DataAnnotations;

namespace EasyOC.OrchardCore.DynamicTypeIndex.Index
{
    public class DIndexBase : FreeSqlDocumentIndex
    {
        [Column(IsPrimary = true)]
        public override int Id { get => base.Id; set { base.Id = value; } }
        public override int DocumentId { get; set; }


        [Column(StringLength = 26)]
        public string ContentItemId { get; set; }

    }
}
