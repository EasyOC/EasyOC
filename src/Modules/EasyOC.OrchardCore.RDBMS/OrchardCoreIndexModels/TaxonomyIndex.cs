using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class TaxonomyIndex
    {

        [JsonProperty, Column(IsPrimary = true)]
        public int Id { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int? DocumentId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string TaxonomyContentItemId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContentItemId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContentType { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContentPart { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContentField { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string TermContentItemId { get; set; }

        [JsonProperty, Column(DbType = "BOOL")]
        public string Published { get; set; } = "1";

        [JsonProperty, Column(DbType = "BOOL")]
        public string Latest { get; set; } = "0";

    }

}



