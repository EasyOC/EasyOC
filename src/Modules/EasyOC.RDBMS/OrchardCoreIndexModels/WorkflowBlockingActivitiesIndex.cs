using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class WorkflowBlockingActivitiesIndex
    {

        [JsonProperty, Column(IsPrimary = true)]
        public int Id { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int? DocumentId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ActivityId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string ActivityName { get; set; }

        [JsonProperty, Column(DbType = "BOOL")]
        public string ActivityIsStart { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string WorkflowTypeId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string WorkflowId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string WorkflowCorrelationId { get; set; }

    }

}



