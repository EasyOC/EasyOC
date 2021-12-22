using FreeSql.DataAnnotations;
using Newtonsoft.Json;

namespace EasyOC.OrchardCoreIndexModels
{

    [JsonObject(MemberSerialization.OptIn), Table(DisableSyncStructure = true)]
    public partial class WorkflowTypeStartActivitiesIndex
    {

        [JsonProperty, Column(IsPrimary = true)]
        public int Id { get; set; }

        [JsonProperty, Column(DbType = "INT")]
        public int? DocumentId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string WorkflowTypeId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string Name { get; set; }

        [JsonProperty, Column(DbType = "BOOL")]
        public string IsEnabled { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string StartActivityId { get; set; }

        [JsonProperty, Column(StringLength = -2)]
        public string StartActivityName { get; set; }

    }

}



