using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class LinkFieldIndex
    {

        [JsonProperty, Column(IsPrimary = true)]
        public int Id { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int? DocumentId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContentItemId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContentItemVersionId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContentType { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContentPart { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContentField { get; set; }

        [JsonProperty, Column(DbType = "BOOL")]
        public string Published { get; set; }

        [JsonProperty, Column(DbType = "BOOL")]
        public string Latest { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string Url { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string BigUrl { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string Text { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string BigText { get; set; }

    }

}



