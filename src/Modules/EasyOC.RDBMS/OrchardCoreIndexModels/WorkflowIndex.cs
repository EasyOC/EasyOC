using FreeSql.DataAnnotations;
using Newtonsoft.Json;
using System;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class WorkflowIndex
    {

        [JsonProperty, Column(IsPrimary = true)]
        public int Id { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int? DocumentId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string WorkflowTypeId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string WorkflowId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string WorkflowStatus { get; set; }

        [JsonProperty]
        public DateTime? CreatedUtc { get; set; }

    }

}



