using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class UserIndex
    {

        [JsonProperty, Column(IsPrimary = true)]
        public int Id { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int? DocumentId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string NormalizedUserName { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string NormalizedEmail { get; set; }

        [JsonProperty, Column(DbType = "BOOL", IsNullable = false)]
        public string IsEnabled { get; set; } = "1";

        [JsonProperty, Column(DbType = "BOOL", IsNullable = false)]
        public string IsLockoutEnabled { get; set; } = "0";

        [JsonProperty]
        public DateTime? LockoutEndUtc { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int AccessFailedCount { get; set; } = 0;

        [JsonProperty, Column(StringLength = -2)]
        public string UserId { get; set; }

    }

}



