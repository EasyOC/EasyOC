using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class Audit_AuditTrailEventIndex
    {

        [JsonProperty, Column(IsPrimary = true)]
        public int Id { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int? DocumentId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string EventId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string Category { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string Name { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string CorrelationId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string UserId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string NormalizedUserName { get; set; }

        [JsonProperty]
        public DateTime? CreatedUtc { get; set; }

    }

}



