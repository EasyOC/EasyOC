using FreeSql.DataAnnotations;
using OrchardCore.ContentManagement.Records;

namespace EasyOC.Core.Indexes
{
    [EOCTable(Name = "ContentItemIndex")]
    public class DIndexBase : FreeSqlDocumentIndex
    {
        [Column(IsPrimary = true)]
        public override int Id { get => base.Id; set { base.Id = value; } }
        public override int DocumentId { get; set; }

        [Column(StringLength = 26)]
        public string ContentItemId { get; set; }
        [Column(StringLength = 26)]
        public string ContentItemVersionId { get; set; }
        public bool Published { get; set; }
        public bool Latest { get; set; }
        [Column(StringLength = ContentItemIndex.MaxDisplayTextSize)]
        public string DisplayText { get; set; }
    }
}
