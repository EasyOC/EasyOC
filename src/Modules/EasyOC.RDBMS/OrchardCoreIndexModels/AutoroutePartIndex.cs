using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class AutoroutePartIndex
    {

        [JsonProperty, Column(IsPrimary = true)]
        public int Id { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int? DocumentId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContentItemId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContainedContentItemId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string JsonPath { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string Path { get; set; }

        [JsonProperty, Column(DbType = "BOOL")]
        public string Published { get; set; }

        [JsonProperty, Column(DbType = "BOOL")]
        public string Latest { get; set; }

    }

}



