using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class ContentItemIndex
    {

        [JsonProperty, Column(IsPrimary = true)]
        public int Id { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int? DocumentId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContentItemId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContentItemVersionId { get; set; }

        [JsonProperty, Column(DbType = "BOOL")]
        public bool Latest { get; set; }

        [JsonProperty, Column(DbType = "BOOL")]
        public bool Published { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContentType { get; set; }

        [JsonProperty]
        public DateTime? ModifiedUtc { get; set; }

        [JsonProperty]
        public DateTime? PublishedUtc { get; set; }

        [JsonProperty]
        public DateTime? CreatedUtc { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string Owner { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string Author { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string DisplayText { get; set; }

    }

}



