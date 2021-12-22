using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class IndexingTask
    {

        [JsonProperty, Column(IsPrimary = true)]
        public int Id { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ContentItemId { get; set; }

        [JsonProperty]
        public DateTime CreatedUtc { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int? Type { get; set; }

    }

}



